using System;
using System.Collections.Generic;
using System.Reflection;
using BrunoMikoski.DebugTools.Core;
using BrunoMikoski.DebugTools.Core.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using Object = UnityEngine.Object;

namespace BrunoMikoski.DebugTools.Layout
{
    public sealed class DebugPanelWindow : MonoBehaviour
    {
        private readonly struct DirectCallbackData
        {
            public readonly string label;
            public readonly string categoryName;
            public readonly Action callback;

            public DirectCallbackData(string targetLabel, string targetCategoryName, Action targetCallback)
            {
                label = targetLabel;
                categoryName = targetCategoryName;
                callback = targetCallback;
            }
        }
        
        [SerializeField] 
        private TMP_InputField searchField;

        [SerializeField] 
        private Button closeButton;

        [SerializeField] 
        private RectTransform scrollRectContextRectTransform;
        
        private Dictionary<string, DebuggableGroupGUI> categoryNameToDebuggableGroupCache =
            new Dictionary<string, DebuggableGroupGUI>();

        [Header("Prefabs")]
        [SerializeField]
        private DebuggableGroupGUI debuggableGroupGUIPrefab;

        private List<object> additionalDebuggableObjects = new List<object>(512);
        private List<object> debuggableObjects = new List<object>(1024);
        private List<DirectCallbackData> directCallbacks = new List<DirectCallbackData>();
        
        private string LastSearchStorageKey => $"{Application.productName}.LastSearch";

        private void Awake()
        {
            closeButton.onClick.AddListener(OnClickCloseWindow);
            debuggableGroupGUIPrefab.gameObject.SetActive(false);
            searchField.onValueChanged.AddListener(OnSearchChanged);
            RegisterDebuggables();
        }

        private void OnDestroy()
        {
            closeButton.onClick.RemoveListener(OnClickCloseWindow);
            searchField.onValueChanged.RemoveListener(OnSearchChanged);
        }
        
        private void OnSearchChanged(string newValue)
        {
            if (string.IsNullOrEmpty(searchField.text))
            {
                foreach (var nameToGroup in categoryNameToDebuggableGroupCache)
                {
                    nameToGroup.Value.ResetSearch();
                    nameToGroup.Value.gameObject.SetActive(true);
                }
            }
            else
            {
                foreach (var nameToGroup in categoryNameToDebuggableGroupCache)
                {
                    bool isGroupEnabled = false;

                    if (nameToGroup.Key.IndexOf(newValue, StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        isGroupEnabled = true;
                    }
                    else
                    {
                        foreach (var nameToGUIs in nameToGroup.Value.NameToDebuggableActionGUICache)
                        {
                            if (nameToGUIs.Key.IndexOf(newValue, StringComparison.OrdinalIgnoreCase) > -1)
                            {
                                isGroupEnabled = true;
                                nameToGUIs.Value.gameObject.SetActive(true);
                            }
                            else
                            {
                                nameToGUIs.Value.gameObject.SetActive(false);
                            }
                        }
                    }
                    
                    nameToGroup.Value.gameObject.SetActive(isGroupEnabled);
                }
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRectContextRectTransform);
            PlayerPrefs.SetString(LastSearchStorageKey, searchField.text);
        }
        
        private void OnClickCloseWindow()
        {
            DebugPanel.CloseWindow();
        }

        public void Open()
        {
            gameObject.SetActive(true);
            
            if (DebugPanel.RefreshOnOpen)
                RegisterDebuggables();
            
            string lastSearch = PlayerPrefs.GetString(LastSearchStorageKey, String.Empty);
            if (!string.IsNullOrEmpty(lastSearch))
            {
                searchField.text = lastSearch;
                OnSearchChanged(searchField.text);
            }
        }

        internal void RegisterDebuggables()
        {
            Cleanup();
            DebugPanel.HotKeyManager.Clear();
            debuggableObjects.Clear();
            debuggableObjects.AddRange(FindObjectsOfType<MonoBehaviour>());
            debuggableObjects.AddRange(additionalDebuggableObjects);
            
            for (int i = 0; i < debuggableObjects.Count; ++i)
            {
                object behaviour = debuggableObjects[i];

                if (behaviour is Object obj)
                {
                    if (obj.hideFlags != HideFlags.None)
                        continue;
                }

                object[] attributes = behaviour.GetType().GetCustomAttributes(typeof(DebuggableClassAttribute), true);

                if (attributes.Length == 0)
                    continue;

                DebuggableClassAttribute debuggableClassAttribute = (DebuggableClassAttribute) attributes[0];

                DebuggableGroupGUI targetDebuggableGroup =
                    GetOrCreateDebuggableGroup(debuggableClassAttribute.CategoryName);

                Type type = behaviour.GetType();
                while (type != null)
                {
                    RegisterDebuggableActions(type, behaviour, targetDebuggableGroup);
                    RegisterDebuggableFields(type, behaviour, targetDebuggableGroup);
                    RegisterDebuggableTextAreas(type, behaviour, targetDebuggableGroup);
                    type = type.BaseType;
                }
            }

            for (int i = 0; i < directCallbacks.Count; i++)
            {
                DirectCallbackData directCallbackData = directCallbacks[i];
                
                DebuggableGroupGUI targetDebuggableGroup = GetOrCreateDebuggableGroup(directCallbackData.categoryName);

                targetDebuggableGroup.AddDebuggableAction(directCallbackData.label, directCallbackData.callback);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRectContextRectTransform);
        }

        private void RegisterDebuggableTextAreas(Type type, object targetObject, DebuggableGroupGUI targetDebuggableGroup)
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.Public
                                                | BindingFlags.NonPublic
                                                | BindingFlags.Instance
                                                | BindingFlags.DeclaredOnly);
            
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo fieldInfo = fields[i];
                object[] fieldsAttributes = fieldInfo.GetCustomAttributes(typeof(DebuggableTextAreaAttribute), true);
                if (fieldsAttributes.Length == 0)
                    continue;

                DebuggableTextAreaAttribute debuggableFieldAttribute = (DebuggableTextAreaAttribute) fieldsAttributes[0];
                targetDebuggableGroup.AddDebuggableTextArea(targetObject, fieldInfo, debuggableFieldAttribute);
            }
        }

        private void RegisterDebuggableFields(Type type, object behaviour, DebuggableGroupGUI targetDebuggableGroup)
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.Public
                                                | BindingFlags.NonPublic
                                                | BindingFlags.Instance
                                                | BindingFlags.DeclaredOnly);

            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo fieldInfo = fields[i];
                object[] fieldsAttributes = fieldInfo.GetCustomAttributes(typeof(DebuggableFieldAttribute), true);
                if (fieldsAttributes.Length == 0)
                    continue;

                DebuggableFieldAttribute debuggableFieldAttribute = (DebuggableFieldAttribute) fieldsAttributes[0];
                targetDebuggableGroup.AddDebuggableField(behaviour, fieldInfo, debuggableFieldAttribute);
            }
        }

        private static void RegisterDebuggableActions(Type type, object behaviour, DebuggableGroupGUI targetDebuggableGroup)
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
                object[] methodAttributes = method.GetCustomAttributes(typeof(DebuggableActionAttribute), false);
                if (methodAttributes.Length == 0)
                    continue;

                DebuggableActionAttribute methodDebuggableActionAttribute = (DebuggableActionAttribute) methodAttributes[0];

                string displayName = methodDebuggableActionAttribute.Caption;
                if (string.IsNullOrEmpty(displayName))
                    displayName = method.Name;

                targetDebuggableGroup.AddDebuggableAction(displayName, method, behaviour,
                    methodDebuggableActionAttribute);
            }
        }

        private DebuggableGroupGUI GetOrCreateDebuggableGroup(string targetCategoryName)
        {
            if (!categoryNameToDebuggableGroupCache.TryGetValue(targetCategoryName, out DebuggableGroupGUI debuggableGroupGUI))
            {
                debuggableGroupGUI = Instantiate(debuggableGroupGUIPrefab, debuggableGroupGUIPrefab.transform.parent);
                debuggableGroupGUI.SetDisplayName(targetCategoryName);
                debuggableGroupGUI.gameObject.SetActive(true);
                categoryNameToDebuggableGroupCache.Add(targetCategoryName, debuggableGroupGUI);
            }

            return debuggableGroupGUI;
        }

        public void Close()
        {
            Cleanup();
            gameObject.SetActive(false);
        }

        private void Cleanup()
        {
            foreach (var groupNameToGroupGUI in categoryNameToDebuggableGroupCache)
            {
                Destroy(groupNameToGroupGUI.Value.gameObject);
            }

            categoryNameToDebuggableGroupCache.Clear();
        }

        public void RegisterDebuggableObject(object targetObject)
        {
            additionalDebuggableObjects.Add(targetObject);
        }

        public void UnregisterDebuggableObject(object targetObject)
        {
            additionalDebuggableObjects.Remove(targetObject);
        }

        public void AddDirectCallback(string targetLabel, string groupName, Action targetCallback)
        {
            directCallbacks.Add(new DirectCallbackData(targetLabel, groupName, targetCallback));
        }
    }
}
