using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BrunoMikoski.DebugPanel.Attributes;
using BrunoMikoski.DebugPanel.GUI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

#if SL_ENABLED
using BrunoMikoski.ServicesLocation;
#endif

namespace BrunoMikoski.DebugPanel
{
#if SL_ENABLED
    [ServiceImplementation]
#endif
    public class DebugPanelService : MonoBehaviour
    {
        private const string DEFAULT_CATEGORY_NAME = "General";
        
        [Header("References")]
        [SerializeField]
        private DebugPanelGUI debugPanelGUI;
        [SerializeField]
        private RectTransform root;
        [SerializeField]
        private CanvasGroup backdrop;
        [SerializeField]
        private Button backdropButton;
        [SerializeField]
        private Button closeButton;
        [SerializeField]
        private Button backButton;
        [SerializeField]
        private TMP_InputField searchInputField;
        [SerializeField]
        private ScrollRect scrollRect;

        [SerializeField]
        private FavoritesPageLinkGUI favoritesPageLink;

        private bool isVisible = true;

        [Header("Settings")]
        [SerializeField, Tooltip("After calling a Method, close the Debug Panel automatically")]
        private bool hideAfterInvoke;
        public bool HideAfterInvoke => hideAfterInvoke;
        [SerializeField] 
        private DebugPanelTriggerSettings triggerSettings;

        [SerializeField, Tooltip("Try to load new Debuggables after each new scene is loaded")] 
        private bool activeLoadDebuggable;

        private Dictionary<string, DebugPage> pathToDebugPage = new Dictionary<string, DebugPage>(100);
        private List<object> debuggables = new List<object>(1000);
        private List<DebuggableItemBase> activeDebuggableItems = new List<DebuggableItemBase>();
        internal List<DebuggableItemBase> ActiveDebuggableItems => activeDebuggableItems;

        private List<object> lifeTimeNonMonobehaviour = new List<object>();
        private List<DebuggableAction> lifeTimeDynamicActions = new List<DebuggableAction>(500);

        private string currentDisplayedPagePath;
        private DebugPage currentDisplayedPage;

        private DebugPage generalPage;
        private DebugPage searchResultsPage;
        private string previousSearchValue = "";
        private DebugPage activePageBeforeSearch;
        private DebugPage favoritesDebugPage;

        private void Awake()
        {
            SetVisible(false);
            backdropButton.onClick.AddListener(Hide);
            closeButton.onClick.AddListener(Hide);
            backButton.onClick.AddListener(OnClickBack);
            searchInputField.onValueChanged.AddListener(OnSearchValueChanged);
            scrollRect.onValueChanged.AddListener(OnScrollRectValueChanged);

#if !UNITY_EDITOR
            activeLoadDebuggable = false;
#endif
            
            if (activeLoadDebuggable)
                SceneManager.sceneLoaded += OnNewSceneLoaded;
        }

        private IEnumerator Start()
        {
            yield return null;
            if (activeLoadDebuggable)
                ReloadDebuggables();
        }
        
        private void OnDestroy()
        {
            backdropButton.onClick.RemoveListener(Hide);
            closeButton.onClick.RemoveListener(Hide);
            backButton.onClick.RemoveListener(OnClickBack);
            searchInputField.onValueChanged.RemoveListener(OnSearchValueChanged);
            scrollRect.onValueChanged.AddListener(OnScrollRectValueChanged);
            SceneManager.sceneLoaded -= OnNewSceneLoaded;
        }
        
        private void OnScrollRectValueChanged(Vector2 normalizedPosition)
        {
            if (currentDisplayedPage != null)
                currentDisplayedPage.SetLastKnowHeight(normalizedPosition.y);
        }
      
        private void OnNewSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (activeLoadDebuggable)
                ReloadDebuggables();
        }

        private void OnSearchValueChanged(string searchValue)
        {
            if (currentDisplayedPage != null && currentDisplayedPage != searchResultsPage)
                activePageBeforeSearch = currentDisplayedPage;
                
            if (previousSearchValue.Length > 0 && searchValue.Length == 0)
            {
                if (activePageBeforeSearch != null)
                    DisplayPage(activePageBeforeSearch);
                else
                    DisplayPage(DEFAULT_CATEGORY_NAME);
                    
                activePageBeforeSearch = null;
                return;
            }
            
            if (debugPanelGUI.CurrentDebugPage == null || debugPanelGUI.CurrentDebugPage != searchResultsPage)
                DisplayPage(searchResultsPage);
            
            searchResultsPage.SetTitle($"Searching: {searchValue}");
            debugPanelGUI.ShowOnlyMatches(searchValue, activePageBeforeSearch);
            debugPanelGUI.UpdateTitle();
            previousSearchValue = searchValue;
        }
        
        private void OnClickBack()
        {
            if (pathToDebugPage.TryGetValue(currentDisplayedPagePath, out DebugPage currentPage))
            {
                if (currentPage.HasParentPage())
                    DisplayPage(currentPage.ParentPage);
                else
                    DisplayGeneralPage();
            }
            else
            {
                if (currentDisplayedPage != null)
                {
                    if(currentDisplayedPage.HasParentPage())
                        DisplayPage(currentDisplayedPage.ParentPage);
                    else
                        DisplayGeneralPage();
                }
            }

            searchInputField.text = "";
        }

        private IEnumerator WaitToUpdateScrollPositionEnumerator()
        {
            yield return null;
            scrollRect.verticalNormalizedPosition = currentDisplayedPage.LastScrollHeight;
        }

        private void SetVisible(bool visible)
        {
            if (visible == isVisible)
                return;

            bool wasVisible = isVisible;
            
            root.gameObject.SetActive(visible);
            backdrop.gameObject.SetActive(visible);
            isVisible = visible;

            if (!wasVisible && isVisible)
                PrepareToDisplay();
        }

        private void PrepareToDisplay()
        {
            ReloadDebuggables();

            if (string.IsNullOrEmpty(currentDisplayedPagePath) || !pathToDebugPage.ContainsKey(currentDisplayedPagePath))
                currentDisplayedPagePath = $"{DEFAULT_CATEGORY_NAME}";
            DisplayPage(currentDisplayedPagePath);
        }

        private void DisplayGeneralPage()
        {
            DisplayPage(DEFAULT_CATEGORY_NAME);
        }
        
        private void DisplayPage(string pagePath)
        {
            if (!pagePath.EndsWith("/"))
                pagePath = $"{pagePath}/";
            
            if (!pathToDebugPage.TryGetValue(pagePath, out DebugPage targetDebugPage))
                return;

            DisplayPage(targetDebugPage);
        }

        internal void DisplayPage(DebugPage targetDebugPage)
        {
            currentDisplayedPagePath = targetDebugPage.PagePath;
            currentDisplayedPage = targetDebugPage;
            
            SetFavoritesButtonEnabled(currentDisplayedPage == generalPage);
            
            debugPanelGUI.DisplayDebugPage(targetDebugPage);


            backButton.gameObject.SetActive(targetDebugPage.HasParentPage());
            StartCoroutine(WaitToUpdateScrollPositionEnumerator());
        }

        private void SetFavoritesButtonEnabled(bool isEnabled)
        {
            if (!favoritesDebugPage.HasContent)
                isEnabled = false;
            
            favoritesPageLink.gameObject.SetActive(isEnabled);
        }

        public void RegisterDebuggable(object targetDebuggable)
        {
            lifeTimeNonMonobehaviour.Add(targetDebuggable);
        }

        public void UnregisterDebuggable(object targetDebuggable)
        {
            lifeTimeNonMonobehaviour.Remove(targetDebuggable);
        }

        public void AddAction(string path, Action targetAction)
        {
            AddDynamicAction(new DebuggableAction(path, targetAction));
        }

        public void AddAction(string path, string subTitle, Action targetAction)
        {
            AddDynamicAction(new DebuggableAction(path, subTitle, targetAction));
        }

        private void AddDynamicAction(DebuggableAction targetAction)
        {
            for (int i = 0; i < lifeTimeDynamicActions.Count; i++)
            {
                DebuggableAction dynamicAction = lifeTimeDynamicActions[i];
                if (dynamicAction.Path.Equals(targetAction.Path, StringComparison.Ordinal))
                    return;
            }
            lifeTimeDynamicActions.Add(targetAction);
        }

        public void ReloadDebuggables()
        {
            pathToDebugPage.Clear();
            debuggables.Clear();
            activeDebuggableItems.Clear();

            searchResultsPage = new DebugPage("Search Result", "", "");
            favoritesDebugPage = new DebugPage("Favorites", "Favorites", "List with all debuggables saved as favorites");
            
            debuggables.AddRange(GetDebuggableMonoBehaviours());
            debuggables.AddRange(lifeTimeNonMonobehaviour);

            for (int i = 0; i < debuggables.Count; i++)
            {
                object debuggableMonoBehaviour = debuggables[i];

                DebuggableClassAttribute debuggableClassAttribute = GetDebuggableClassAttribute(debuggableMonoBehaviour);

                if (debuggableClassAttribute == null)
                {
                    Debug.LogError($"Object {debuggableMonoBehaviour} is registered as a Debuggable but doesn't contains the [DebuggableClass] attribute, ignoring it");
                    continue;
                }

                string pagePath = debuggableClassAttribute.Path;
                if (string.IsNullOrEmpty(pagePath))
                    pagePath = debuggableMonoBehaviour.GetType().Name;

                DebugPage categoryPage = GetOrCreatePathByPath(pagePath, debuggableClassAttribute.SubTitle);
                
                List<DebuggableMethod> debuggableMethods =
                    GetDebuggableMethodsFromObject(debuggableMonoBehaviour, debuggableClassAttribute);

                for (int j = 0; j < debuggableMethods.Count; j++)
                {
                    DebuggableMethod debuggableMethod = debuggableMethods[j];
                    AddDebuggableToAppropriatedPath(debuggableMethod, categoryPage);
                    if (debuggableMethod.IsFavorite)
                        favoritesDebugPage.AddItem(debuggableMethod);
                }

                List<DebuggableField> debuggableFields =
                    GetDebuggableFieldsFromObject(debuggableMonoBehaviour, debuggableClassAttribute);

                for (int j = 0; j < debuggableFields.Count; j++)
                {
                    DebuggableField debuggableField = debuggableFields[j];
                    AddDebuggableToAppropriatedPath(debuggableField, categoryPage);
                    if (debuggableField.IsFavorite)
                        favoritesDebugPage.AddItem(debuggableField);
                }
            }

            for (int i = 0; i < lifeTimeDynamicActions.Count; i++)
                AddDebuggableToAppropriatedPath(lifeTimeDynamicActions[i], null);

            if (pathToDebugPage.TryGetValue($"{DEFAULT_CATEGORY_NAME}/", out generalPage))
            {
                searchResultsPage.SetParentPage(generalPage);
                favoritesDebugPage.SetParentPage(generalPage);
                favoritesPageLink.Initialize(new DebuggablePageLink("Favorites","List with all debuggables saved as favorites", favoritesDebugPage), generalPage);
            }
        }

        private void AddDebuggableToAppropriatedPath(DebuggableItemBase targetDebuggableBase, DebugPage parentPage)
        {
            DebugPage finalPage;
            
            int lastIndexOfPath = targetDebuggableBase.Path.LastIndexOf("/", StringComparison.Ordinal);
            if (lastIndexOfPath == -1)
            {
                if (parentPage == null)
                    finalPage = GetOrCreatePathByPath($"{DEFAULT_CATEGORY_NAME}/");
                else
                    finalPage = parentPage;
                
                finalPage.AddItem(targetDebuggableBase);
            }
            else
            {
                if (parentPage == null)
                {
                    string clearPath = targetDebuggableBase.Path.Substring(0, lastIndexOfPath);
                    finalPage = GetOrCreatePathByPath(clearPath);
                }
                else
                {
                    string clearPath = $"{parentPage.PagePath}/{targetDebuggableBase.Path.Substring(0, lastIndexOfPath)}";
                    finalPage = GetOrCreatePathByPath(clearPath);
                }
                finalPage.AddItem(targetDebuggableBase);

            }

            string fullPath = $"{finalPage.PagePath}{targetDebuggableBase.Path}".Replace("//", "/");
            targetDebuggableBase.SetFinalFullPath(fullPath);

            if (finalPage.IsFavorite)
                favoritesDebugPage.AddChildPage(finalPage);
                
            activeDebuggableItems.Add(targetDebuggableBase);
            searchResultsPage.AddItem(targetDebuggableBase);
        }

        private List<DebuggableField> GetDebuggableFieldsFromObject(object debuggableMonoBehaviours, DebuggableClassAttribute debuggableClassAttribute)
        {
            List<DebuggableField> dynamicFields = new List<DebuggableField>();
            Type type = debuggableMonoBehaviours.GetType();
            while (type != null)
            {
                FieldInfo[] fieldInfos = type.GetFields(
                    BindingFlags.Public
                    | BindingFlags.NonPublic
                    | BindingFlags.Instance
                    | BindingFlags.DeclaredOnly
                );
                
                for (int j = 0; j < fieldInfos.Length; j++)
                {
                    FieldInfo fieldInfo = fieldInfos[j];

                    if (!fieldInfo.TryGetAttribute(out DebuggableFieldAttribute attribute))
                        continue;

                    string path = attribute.Path;
                    if (string.IsNullOrEmpty(path))
                        path = $"{fieldInfo.Name}";
                    
                    dynamicFields.Add(new DebuggableField(path, fieldInfo, debuggableMonoBehaviours, debuggableClassAttribute, attribute));
                }

                type = type.BaseType;
            }

            return dynamicFields;
        }

        private DebugPage GetOrCreatePathByPath(string pagePath, string subTitle = "", string spriteName = "")
        {
            if (!pagePath.StartsWith($"{DEFAULT_CATEGORY_NAME}/"))
                pagePath = $"{DEFAULT_CATEGORY_NAME}/{pagePath}";
            
            string[] folders = pagePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            string pageFinalPath = "";
            DebugPage parentPage = null;
            DebugPage targetPage = null;
            for (int i = 0; i < folders.Length; i++)
            {
                string folder = folders[i];
                if (pageFinalPath.EndsWith($"{folder}/"))
                    continue;
                
                pageFinalPath += $"{folder}/";

                if (!pathToDebugPage.TryGetValue(pageFinalPath, out DebugPage resultPage))
                {
                    resultPage = new DebugPage(pageFinalPath, folder, "");
                    if (parentPage != null)
                        parentPage.AddChildPage(resultPage);

                    pathToDebugPage.Add(pageFinalPath, resultPage);
                }

                parentPage = resultPage;

                if (i == folders.Length - 1)
                {
                    resultPage.UpdateData(subTitle);
                    targetPage = resultPage;
                }
            }

            return targetPage;
        }

        private List<DebuggableMethod> GetDebuggableMethodsFromObject(object debuggableMonoBehaviours, DebuggableClassAttribute debuggableClassAttribute)
        {
            List<DebuggableMethod> dynamicMethods = new List<DebuggableMethod>();
            Type type = debuggableMonoBehaviours.GetType();
            while (type != null)
            {
                MethodInfo[] methods = type.GetMethods(
                    BindingFlags.Public
                    | BindingFlags.NonPublic
                    | BindingFlags.Instance
                    | BindingFlags.DeclaredOnly
                );
                
                for (int j = 0; j < methods.Length; j++)
                {
                    MethodInfo method = methods[j];
                    object[] attributes = method.GetCustomAttributes(typeof(DebuggableMethodAttribute), false);
                    if (attributes.Length <= 0)
                        continue;
                    
                    DebuggableMethodAttribute attribute = (DebuggableMethodAttribute) attributes[0];

                    string path = attribute.Path;
                    if (string.IsNullOrEmpty(path))
                        path = $"{method.Name}";

                    DebuggableMethod debuggableMethod = new DebuggableMethod(path, attribute.SubTitle, method, debuggableMonoBehaviours, debuggableClassAttribute, attribute);
                    debuggableMethod.AssignHotkey(attribute.Hotkey);
                    dynamicMethods.Add(debuggableMethod);
                }

                type = type.BaseType;
            }

            return dynamicMethods;
        }

        private DebuggableClassAttribute GetDebuggableClassAttribute(object target)
        {
            target.GetType()
                .GetCustomAttribute(typeof(DebuggableClassAttribute), true);

            object[] attributes = target.GetType().GetCustomAttributes(typeof(DebuggableClassAttribute), true);

            if (attributes.Length == 0)
                return null;
        
            DebuggableClassAttribute debuggableClassAttribute =
                (DebuggableClassAttribute) attributes[0];

            return debuggableClassAttribute;
        }
        
        private List<object> GetDebuggableMonoBehaviours()
        {
            List<object> debuggableMonoBehaviours = new List<object>();
            MonoBehaviour[] behavioursInScene = FindObjectsOfType<MonoBehaviour>();

            for (int i = 0; i < behavioursInScene.Length; ++i)
            {
                MonoBehaviour behaviour = behavioursInScene[i];
                if (behaviour.hideFlags == HideFlags.None)
                {
                    object[] attributes = behaviour.GetType().GetCustomAttributes(typeof(DebuggableClassAttribute), true);

                    if (attributes.Length > 0)
                        debuggableMonoBehaviours.Add(behaviour);
                }
            }

            return debuggableMonoBehaviours;
        }

#if UNITY_EDITOR
        private void Update()
        {
            CheckForHotkey();
        }
#endif
        
        private void CheckForHotkey()
        {
            if (!triggerSettings.IsTriggered()) 
                return;
            
            if (!isVisible)
                Show();
            else
                Hide();
        }

        public void Hide()
        {
            SetVisible(false);
            searchInputField.text = "";
        }

        public void Show()
        {
            SetVisible(true);
        }

        internal void UpdateDebuggableFavorite(DebuggableItemBase debuggableItem)
        {
            if (debuggableItem is DebuggablePageLink pageLink)
            {
                if (pageLink.IsFavorite)
                    favoritesDebugPage.AddChildPage(pageLink.ToDebugPage, false);
                else
                    favoritesDebugPage.RemoveChildPage(pageLink.ToDebugPage);
                
                return;
            }
            
            if (debuggableItem.IsFavorite)
            {
                favoritesDebugPage.AddItem(debuggableItem);
            }
            else
            {
                favoritesDebugPage.RemoveItem(debuggableItem);
            }
        }
    }
}
