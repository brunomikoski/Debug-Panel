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


namespace BrunoMikoski.DebugPanel
{
    public class DebugPanel : MonoBehaviour
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

        private bool isVisible = true;

        [Header("Settings")]
        [SerializeField]
        private bool hideAfterInvoke;
        public bool HideAfterInvoke => hideAfterInvoke;
        [SerializeField] 
        private DebugPanelTriggerSettings triggerSettings;

        [SerializeField] 
        private bool activeLoadDebuggable;

        private Dictionary<string, DebugPage> pathToDebugPage = new Dictionary<string, DebugPage>(100);
        private List<object> debuggables = new List<object>(1000);
        private List<DebuggableItemBase> activeDebuggableItems = new List<DebuggableItemBase>();
        public List<DebuggableItemBase> ActiveDebuggableItems => activeDebuggableItems;

        private List<object> lifeTimeNonMonobehaviour = new List<object>();
        private List<DebuggableAction> lifeTimeDynamicActions = new List<DebuggableAction>(500);

        private string currentDisplayedPagePath;
        private DebugPage currentDisplayedPage;

        private DebugPage searchResultsPage;
        private string previousSearchValue = "";

        private void Awake()
        {
            SetVisible(false, false);
            backdropButton.onClick.AddListener(Close);
            closeButton.onClick.AddListener(Close);
            backButton.onClick.AddListener(OnClickBack);
            searchInputField.onValueChanged.AddListener(OnSearchValueChanged);
            if (activeLoadDebuggable)
                SceneManager.sceneLoaded += OnNewSceneLoaded;
        }

        private IEnumerator Start()
        {
            yield return null;
            if (activeLoadDebuggable)
                ReloadDebuggables();
        }
        
        private void OnNewSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (activeLoadDebuggable)
                ReloadDebuggables();
        }
        
        private void OnDestroy()
        {
            backdropButton.onClick.RemoveListener(Close);
            closeButton.onClick.RemoveListener(Close);
            backButton.onClick.RemoveListener(OnClickBack);
            searchInputField.onValueChanged.RemoveListener(OnSearchValueChanged);
            SceneManager.sceneLoaded -= OnNewSceneLoaded;
        }


        private void OnSearchValueChanged(string searchValue)
        {
            if (previousSearchValue.Length > 0 && searchValue.Length == 0)
            {
                DisplayPage(DEFAULT_CATEGORY_NAME);
                return;
            }
            
            if (debugPanelGUI.CurrentDebugPage == null || debugPanelGUI.CurrentDebugPage != searchResultsPage)
                DisplayPage(searchResultsPage);
            
            searchResultsPage.SetTitle($"Searching: {searchValue}");
            debugPanelGUI.ShowOnlyMatches(searchValue);
            debugPanelGUI.UpdateTitle();
            previousSearchValue = searchValue;
        }

        private void Close()
        {
            searchInputField.text = "";
            SetVisible(false);
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

            StartCoroutine(WaitToUpdateScrollPositionEnumerator());
        }

        private IEnumerator WaitToUpdateScrollPositionEnumerator()
        {
            yield return null;
            if (currentDisplayedPage != null && currentDisplayedPage.LastScrollHeight.HasValue)
                scrollRect.verticalNormalizedPosition = currentDisplayedPage.LastScrollHeight.Value;
        }

        private void SetVisible(bool visible, bool animated = true)
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

        public void DisplayPage(DebugPage debugPage)
        {
            if (currentDisplayedPage != null)
                currentDisplayedPage.SetLastKnowHeight(scrollRect.verticalNormalizedPosition);
            
            currentDisplayedPagePath = debugPage.PagePath;
            currentDisplayedPage = debugPage;
            debugPanelGUI.DisplayDebugPage(debugPage);

            backButton.gameObject.SetActive(debugPage.HasParentPage());
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

            searchResultsPage = new DebugPage("Search Result", "", "", "");
            
            debuggables.AddRange(GetDebuggableMonoBehaviours());
            debuggables.AddRange(lifeTimeNonMonobehaviour);

            for (int i = 0; i < debuggables.Count; i++)
            {
                object debuggableMonoBehaviour = debuggables[i];

                DebuggableClassAttribute debuggableClassAttribute = GetDebuggableClassAttribute(debuggableMonoBehaviour);

                string pagePath = debuggableClassAttribute.Path;
                if (string.IsNullOrEmpty(pagePath))
                    pagePath = debuggableMonoBehaviour.GetType().Name;

                DebugPage categoryPage = GetOrCreatePathByPath(pagePath, debuggableClassAttribute.SubTitle);
                
                List<DebuggableMethod> debuggableMethods =
                    GetDebuggableMethodsFromObject(debuggableMonoBehaviour, debuggableClassAttribute);

                for (int j = 0; j < debuggableMethods.Count; j++)
                    AddDebuggableToAppropriatedPath(debuggableMethods[j], categoryPage);

                List<DebuggableField> debuggableFields =
                    GetDebuggableFieldsFromObject(debuggableMonoBehaviour, debuggableClassAttribute);

                for (int j = 0; j < debuggableFields.Count; j++)
                    AddDebuggableToAppropriatedPath(debuggableFields[j], categoryPage);
            }

            for (int i = 0; i < lifeTimeDynamicActions.Count; i++)
                AddDebuggableToAppropriatedPath(lifeTimeDynamicActions[i], null);

            if (pathToDebugPage.TryGetValue($"{DEFAULT_CATEGORY_NAME}/", out DebugPage generalPage))
                searchResultsPage.SetParentPage(generalPage);
        }

        public void AddDebuggableToAppropriatedPath(DebuggableItemBase targetDebuggableBase, DebugPage parentPage)
        {
            int lastIndexOfPath = targetDebuggableBase.Path.LastIndexOf("/", StringComparison.Ordinal);
            if (lastIndexOfPath == -1)
            {
                if (parentPage == null)
                {
                    DebugPage generalPage = GetOrCreatePathByPath($"{DEFAULT_CATEGORY_NAME}/");
                    generalPage.AddItem(targetDebuggableBase);
                }
                else
                {
                    parentPage.AddItem(targetDebuggableBase);
                }
            }
            else
            {
                if (parentPage == null)
                {
                    string clearPath = targetDebuggableBase.Path.Substring(0, lastIndexOfPath);
                    DebugPage categoryPage = GetOrCreatePathByPath(clearPath);
                    categoryPage.AddItem(targetDebuggableBase);
                }
                else
                {
                    string clearPath = $"{parentPage.PagePath}/{targetDebuggableBase.Path.Substring(0, lastIndexOfPath)}";
                    DebugPage resultPage = GetOrCreatePathByPath(clearPath);
                    resultPage.AddItem(targetDebuggableBase);
                }
            }

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

                    string path = attribute.Title;
                    if (string.IsNullOrEmpty(path))
                        path = $"{debuggableClassAttribute.Path}/{fieldInfo.Name}";
                    
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
                pageFinalPath += $"{folder}/";

                if (!pathToDebugPage.TryGetValue(pageFinalPath, out DebugPage resultPage))
                {
                    resultPage = new DebugPage(pageFinalPath, folder, "", "");
                    if (parentPage != null)
                        parentPage.AddChildPage(resultPage);

                    pathToDebugPage.Add(pageFinalPath, resultPage);
                }

                parentPage = resultPage;

                if (i == folders.Length - 1)
                {
                    resultPage.UpdateData(subTitle, spriteName);
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
                        path = $"{debuggableClassAttribute.Path}/{method.Name}";

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


        private void Update()
        {
            if (triggerSettings.IsTriggered())
            {
                if (!isVisible)
                    Show();
                else
                    Hide();
            }
        }

        public void Hide()
        {
            SetVisible(false);
        }

        public void Show()
        {
            SetVisible(true);
        }
    }
}
