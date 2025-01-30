using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BrunoMikoski.DebugTools.GUI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace BrunoMikoski.DebugTools
{
    public class DebugPanel : MonoBehaviour
    {
        private static List<object> LIFE_TIME_NON_MONOBEHAVIOUR = new List<object>(50);
        private static List<DebuggableAction> LIFE_TIME_DYNAMIC_ACTIONS = new List<DebuggableAction>(100);
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
        private Sprite[] knowSprites;

        
        [Header("Settings")]
        [SerializeField, Tooltip("After calling a Method, close the Debug Panel automatically")]
        private bool hideAfterInvoke;
        public static bool HideAfterInvoke
        {
            get
            {
                if (!TryGetInstance())
                {
#if UNITY_EDITOR
                    Debug.LogWarning("DebugPanel is not available anywhere, make sure you add the prefab");
#endif
                    return false; 
                }
                return Instance.hideAfterInvoke;
            }
        }

        [SerializeField]
        private bool showOverlayWhenOpen = true;
        [SerializeField]
        private float timescaleWhileOpen = 1;
        
        [SerializeField] 
        private DebugPanelTriggerSettings triggerSettings;
        [SerializeField]
        private bool keepAlive;
        [SerializeField, Tooltip("Try to load new Debuggables after each new scene is loaded")] 
        private bool activeLoadDebuggable;

        private Dictionary<string, DebugPage> pathToDebugPage = new Dictionary<string, DebugPage>(100);
        private List<object> debuggables = new List<object>(1000);
        private List<DebuggableItemBase> activeDebuggableItems = new List<DebuggableItemBase>();
        internal List<DebuggableItemBase> ActiveDebuggableItems => activeDebuggableItems;

        private string currentDisplayedPagePath;
        private DebugPage currentDisplayedPage;
        private DebugPage searchResultsPage;
        private string previousSearchValue = "";
        private DebugPage activePageBeforeSearch;
        private DebugPage favoritesDebugPage;
        private DebugPage profilablesDebugPage;
        private bool isVisible = true;
        
        private static bool hasCachedInstance;
        private static DebugPanel cachedInstance;
        private static DebugPanel Instance
        {
            get
            {
                TryGetInstance();
                return cachedInstance;
            }
        }

        private float previousTimeScale;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void SupportDomainReload()
        {
            LIFE_TIME_NON_MONOBEHAVIOUR = new List<object>(50);
            LIFE_TIME_DYNAMIC_ACTIONS = new List<DebuggableAction>(100);
            hasCachedInstance = default;
            cachedInstance = null;
        }

        protected virtual void Awake()
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

            if (keepAlive)
                DontDestroyOnLoad(this.gameObject);
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

        private static bool TryGetInstance()
        {
            if (hasCachedInstance) 
                return true;
            

#if UNITY_6000_0_OR_NEWER
            cachedInstance = FindFirstObjectByType<DebugPanel>();
#else
            cachedInstance = FindObjectOfType<DebugPanel>();
#endif

            if (cachedInstance != null)
                hasCachedInstance = true;

            return hasCachedInstance;
        }
        
        public static void RegisterDebuggable(object targetDebuggable)
        {
            LIFE_TIME_NON_MONOBEHAVIOUR.Add(targetDebuggable);
        }

        public static void UnregisterDebuggable(object targetDebuggable)
        {
            LIFE_TIME_NON_MONOBEHAVIOUR.Remove(targetDebuggable);
        }

        public static void AddAction(string path, Action targetAction)
        {
            AddDynamicAction(new DebuggableAction(path, targetAction));
        }

        public static void AddAction(string path, string subTitle, Action targetAction)
        {
            AddDynamicAction(new DebuggableAction(path, subTitle, targetAction));
        }
        
        public static void Hide()
        {
            if (!TryGetInstance())
            {
#if UNITY_EDITOR
                Debug.LogWarning("DebugPanel is not available anywhere, make sure you add the prefab");
#endif
                return;
            }
            
            Instance.SetVisible(false);
            Instance.searchInputField.text = "";
        }

        public static void Show()
        {
            if (!TryGetInstance())
            {
#if UNITY_EDITOR
                Debug.LogWarning("DebugPanel is not available anywhere, make sure you add the prefab");
#endif
                return;
            }
            
            Instance.SetVisible(true);
        }
        
        public static void ReloadDebuggables()
        {
            if (!TryGetInstance())
            {
#if UNITY_EDITOR
                Debug.LogWarning("DebugPanel is not available anywhere, make sure you add the prefab");
#endif
                return;
            }
            
            Instance.ReloadDebuggablesInternal();
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

        protected virtual void SetVisible(bool visible)
        {
            if (visible == isVisible)
                return;

            bool wasVisible = isVisible;
            
            root.gameObject.SetActive(visible);
            if (showOverlayWhenOpen)
                backdrop.gameObject.SetActive(visible);
            
            isVisible = visible;

            if (!wasVisible && isVisible)
            {
                if (!Mathf.Approximately(timescaleWhileOpen, 1))
                {
                    previousTimeScale = Time.timeScale;
                    Time.timeScale = timescaleWhileOpen;
                }
                
                PrepareToDisplay();
            }
            else if (wasVisible && !isVisible)
            {
                if (!Mathf.Approximately(timescaleWhileOpen, 1))
                    Time.timeScale = previousTimeScale;
            }
        }

        protected virtual void PrepareToDisplay()
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

            DisplayPageInternal(targetDebugPage);
        }

        internal static void DisplayPage(DebugPage targetDebugPage)
        {
            if (!TryGetInstance())
            {
#if UNITY_EDITOR
                Debug.LogWarning("DebugPanel is not available anywhere, make sure you add the prefab");
#endif
                return;
            }

            Instance.DisplayPageInternal(targetDebugPage);
        }

        private void DisplayPageInternal(DebugPage targetDebugPage)
        {
            OnScrollRectValueChanged(scrollRect.normalizedPosition);
            
            currentDisplayedPagePath = targetDebugPage.PagePath;
            currentDisplayedPage = targetDebugPage;
            
            debugPanelGUI.DisplayDebugPage(targetDebugPage);

            backButton.gameObject.SetActive(targetDebugPage.HasParentPage());
            StartCoroutine(WaitToUpdateScrollPositionEnumerator());
        }

        private static void AddDynamicAction(DebuggableAction targetAction)
        {
            for (int i = 0; i < LIFE_TIME_DYNAMIC_ACTIONS.Count; i++)
            {
                DebuggableAction dynamicAction = LIFE_TIME_DYNAMIC_ACTIONS[i];
                if (dynamicAction.Path.Equals(targetAction.Path, StringComparison.Ordinal))
                    return;
            }
            LIFE_TIME_DYNAMIC_ACTIONS.Add(targetAction);
        }

        private void ReloadDebuggablesInternal()
        {
            pathToDebugPage.Clear();
            debuggables.Clear();
            activeDebuggableItems.Clear();

            searchResultsPage = GetOrCreatePageByPath("Search Result", "", null, false);
            favoritesDebugPage = GetOrCreatePageByPath("Favorites", "Favorites", GetSpriteByName("favorite"), true, -100);
            profilablesDebugPage = GetOrCreatePageByPath("Profilables", "Profilable Methods", GetSpriteByName("performance"), true, -90);
            
            debuggables.AddRange(GetDebuggableMonoBehaviours());
            debuggables.AddRange(LIFE_TIME_NON_MONOBEHAVIOUR);

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

                DebugPage categoryPage = GetOrCreatePageByPath(pagePath, debuggableClassAttribute.SubTitle);
                
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
                
                
                List<ProfilableMethod> profilableMethods = GetProfilableMethodsFromObject(debuggableMonoBehaviour, debuggableClassAttribute);
                if (profilableMethods.Count > 0)
                {
                    DebugPage profilableCategoryPage;
                    for (int j = 0; j < profilableMethods.Count; j++)
                    {
                        ProfilableMethod profilableMethod = profilableMethods[j];
                        profilableCategoryPage = GetOrCreatePageByPath($"General/Profilables/{pagePath}/{profilableMethod.Title}");
                        AddDebuggableToAppropriatedPath(profilableMethod, profilableCategoryPage);
                        if (profilableMethod.IsFavorite)
                            profilablesDebugPage.AddItem(profilableMethod);
                    }
                }
            }

            for (int i = 0; i < LIFE_TIME_DYNAMIC_ACTIONS.Count; i++)
                AddDebuggableToAppropriatedPath(LIFE_TIME_DYNAMIC_ACTIONS[i], null);
        }

        private Sprite GetSpriteByName(string targetSpriteName)
        {
            if (knowSprites == null || knowSprites.Length == 0)
                return null;

            for (int i = 0; i < knowSprites.Length; i++)
            {
                Sprite sprite = knowSprites[i];
                if (sprite.name.Equals(targetSpriteName, StringComparison.Ordinal))
                    return sprite;
            }
            return null;
        }

        private void AddDebuggableToAppropriatedPath(DebuggableItemBase targetDebuggableBase, DebugPage parentPage)
        {
            DebugPage finalPage;
            
            int lastIndexOfPath = targetDebuggableBase.Path.LastIndexOf("/", StringComparison.Ordinal);
            if (lastIndexOfPath == -1)
            {
                if (parentPage == null)
                    finalPage = GetOrCreatePageByPath($"{DEFAULT_CATEGORY_NAME}/");
                else
                    finalPage = parentPage;
                
                finalPage.AddItem(targetDebuggableBase);
            }
            else
            {
                if (parentPage == null)
                {
                    string clearPath = targetDebuggableBase.Path.Substring(0, lastIndexOfPath);
                    finalPage = GetOrCreatePageByPath(clearPath);
                }
                else
                {
                    string clearPath = $"{parentPage.PagePath}/{targetDebuggableBase.Path.Substring(0, lastIndexOfPath)}";
                    finalPage = GetOrCreatePageByPath(clearPath);
                }
                finalPage.AddItem(targetDebuggableBase);

            }

            string fullPath = $"{finalPage.PagePath}{targetDebuggableBase.Path}".Replace("//", "/");
            if (targetDebuggableBase is DebuggableAction debuggableAction)
                fullPath = $"{DEFAULT_CATEGORY_NAME}/{debuggableAction.Path}";
            
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

        private DebugPage GetOrCreatePageByPath(string pagePath, string subTitle = "", Sprite targetSprite = null, bool visible = true, int priority = 0)
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
                    resultPage = new DebugPage(pageFinalPath, folder, subTitle, targetSprite, priority);
                    resultPage.SetVisibility(visible);
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

        private List<ProfilableMethod> GetProfilableMethodsFromObject(object debuggableMonoBehaviours,
            DebuggableClassAttribute debuggableClassAttribute)
        {
            List<ProfilableMethod> profilableMethods = new List<ProfilableMethod>();
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
                    object[] attributes = method.GetCustomAttributes(typeof(ProfilableMethodAttribute), false);
                    if (attributes.Length <= 0)
                        continue;
                    
                    ProfilableMethodAttribute attribute = (ProfilableMethodAttribute) attributes[0];

                    string path = attribute.Path;
                    if (string.IsNullOrEmpty(path))
                        path = $"{method.Name}";

                    ProfilableMethod debuggableMethod = new ProfilableMethod(path, attribute.SubTitle, method, debuggableMonoBehaviours, debuggableClassAttribute, attribute, attribute.ExecutionCount);
                    debuggableMethod.AssignHotkey(attribute.Hotkey);
                    profilableMethods.Add(debuggableMethod);
                }

                type = type.BaseType;
            }

            return profilableMethods;
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

#if UNITY_6000_0_OR_NEWER
            MonoBehaviour[] behavioursInScene = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
#else
            MonoBehaviour[] behavioursInScene = FindObjectsOfType<MonoBehaviour>();
#endif

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
            CheckForToggle();
        }
        
        private void CheckForToggle()
        {
            if (!triggerSettings.IsTriggered()) 
                return;
            
            if (!isVisible)
                Show();
            else
                Hide();
        }

        internal static void UpdateDebuggableFavorite(DebuggableItemBase debuggableItem)
        {
            if (!TryGetInstance())
            {
#if UNITY_EDITOR
                Debug.LogWarning("DebugPanel is not available anywhere, make sure you add the prefab");
#endif
                return;
            }
            
            if (debuggableItem is DebuggablePageLink pageLink)
            {
                if (pageLink.IsFavorite)
                    Instance.favoritesDebugPage.AddChildPage(pageLink.ToDebugPage, false);
                else
                    Instance.favoritesDebugPage.RemoveChildPage(pageLink.ToDebugPage);
                
                return;
            }
            
            if (debuggableItem.IsFavorite)
            {
                Instance.favoritesDebugPage.AddItem(debuggableItem);
            }
            else
            {
                Instance.favoritesDebugPage.RemoveItem(debuggableItem);
            }
        }
    }
}