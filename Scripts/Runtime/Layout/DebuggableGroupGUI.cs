using System;
using System.Collections.Generic;
using System.Reflection;
using BrunoMikoski.DebugTools.Core;
using BrunoMikoski.DebugTools.Core.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace BrunoMikoski.DebugTools.Layout
{
    public sealed class DebuggableGroupGUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text groupName;


        [SerializeField] 
        private Button foldoutButton;

        [SerializeField] 
        private RectTransform foldoutIconRectTransform;

        [SerializeField] 
        private RectTransform scrollRectContextRectTransform;

        [Header("Prefabs")]
        [SerializeField] 
        private DebuggableActionGUI debuggableActionGUIPrefab;
        [SerializeField] 
        private DebuggableFieldGUI debuggableFieldGUIPrefab;
        [SerializeField] 
        private DebuggableRangeFieldGUI debuggableRangeFieldGUIPrefab;
        [SerializeField] 
        private DebuggableDropdownFieldGUI debuggableDropdownFieldGUIPrefab;
        [SerializeField] 
        private DebuggableTextAreaGUI debuggableTextAreaGUIPrefab;
        [SerializeField] 
        private DebuggableToggleFieldGUI debuggableToggleFieldGUIPrefab;

        private string IsExpandedStorageKey => $"{Application.productName}.{groupName.text}.IsExpanded";
        private bool IsExpanded
        {
            get
            {
                int isExpanded = PlayerPrefs.GetInt(IsExpandedStorageKey, -1);
                if (isExpanded == -1)
                {
                    if (groupName.text.Equals(DebugPanel.DEFAULT_CATEGORY_NAME, StringComparison.Ordinal))
                        return true;
                }

                return isExpanded == 1;
            }
        }

        private Dictionary<string, DebuggableItemGUIBase> nameToDebuggableActionGUICache =
            new Dictionary<string, DebuggableItemGUIBase>();
        public Dictionary<string, DebuggableItemGUIBase> NameToDebuggableActionGUICache => nameToDebuggableActionGUICache;

        private void Awake()
        {
            debuggableActionGUIPrefab.gameObject.SetActive(false);
            debuggableFieldGUIPrefab.gameObject.SetActive(false);
            debuggableRangeFieldGUIPrefab.gameObject.SetActive(false);
            debuggableDropdownFieldGUIPrefab.gameObject.SetActive(false);
            debuggableTextAreaGUIPrefab.gameObject.SetActive(false);
            debuggableToggleFieldGUIPrefab.gameObject.SetActive(false);
            foldoutButton.onClick.AddListener(OnToggleFoldout);
            SetExpanded(IsExpanded, false);
        }

        private void OnDestroy()
        {
            foldoutButton.onClick.RemoveListener(OnToggleFoldout);
        }
        
        private void SetExpanded(bool isExpanded, bool updateChildren = true)
        {
            if (updateChildren)
            {
                foreach (var nameToDebuggableAction in nameToDebuggableActionGUICache)
                {
                    nameToDebuggableAction.Value.gameObject.SetActive(isExpanded);
                }
            }

            PlayerPrefs.SetInt(IsExpandedStorageKey, isExpanded ? 1 : 0);
            if (isExpanded)
            {
                foldoutIconRectTransform.localEulerAngles = new Vector3(0, 0, -45);
                foldoutIconRectTransform.anchoredPosition = new Vector2(-263, 12);
            }
            else
            {
                foldoutIconRectTransform.localEulerAngles = new Vector3(0, 0, 45);
                foldoutIconRectTransform.anchoredPosition = new Vector2(-266, -7);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRectContextRectTransform);
        }

        private void OnToggleFoldout()
        {
            SetExpanded(!IsExpanded);
        }

        public void SetDisplayName(string targetCategoryName)
        {
            groupName.text = targetCategoryName;
        }

        public void AddDebuggableAction(string displayName, MethodInfo method, object targetObj,
            DebuggableActionAttribute debuggableActionAttribute)
        {
            if (nameToDebuggableActionGUICache.ContainsKey(displayName))
            {
                Debug.LogError($"Action with the same name {displayName} already exist on group: {groupName.text}, ignoring it");
                return;
            }

            Action targetCallback = () => { method.Invoke(targetObj, new object[] { }); };

            DebuggableActionGUI debuggableActionInstance =
                Instantiate(debuggableActionGUIPrefab, debuggableActionGUIPrefab.transform.parent);

            debuggableActionInstance.gameObject.SetActive(IsExpanded);
            debuggableActionInstance.Initialize(displayName, targetCallback);

            if (!string.IsNullOrEmpty(debuggableActionAttribute.Hotkey))
            {
                DebuggableActionHotKeyData hotKeyData =
                    DebugPanel.HotKeyManager.AddHotkeyToCallback(debuggableActionAttribute.Hotkey, displayName,
                        targetCallback);
                if (hotKeyData != null)
                {
                    debuggableActionInstance.SetHumanReadableHotkey(hotKeyData.HumanReadableHotKey);
                }
            }

            nameToDebuggableActionGUICache.Add(displayName, debuggableActionInstance);
        }

        public void AddDebuggableAction(string displayName, Action callback)
        {
            if (nameToDebuggableActionGUICache.ContainsKey(displayName))
            {
                Debug.LogError($"Action with the same name {displayName} already exist on group: {groupName.text}, ignoring it");
                return;
            }

            DebuggableActionGUI debuggableActionInstance =
                Instantiate(debuggableActionGUIPrefab, debuggableActionGUIPrefab.transform.parent);

            debuggableActionInstance.gameObject.SetActive(IsExpanded);
            debuggableActionInstance.Initialize(displayName, callback);

            nameToDebuggableActionGUICache.Add(displayName, debuggableActionInstance);
        }

        public void AddDebuggableDropdownField(object targetObject, FieldInfo fieldInfo, DebuggableFieldAttribute debuggableFieldAttribute)
        {
            if (nameToDebuggableActionGUICache.ContainsKey(fieldInfo.Name))
            {
                Debug.LogError($"Field with the same name {fieldInfo.Name} already exist on group: {groupName.text}, ignoring it");
                return;
            }
            
            DebuggableDropdownFieldGUI debuggableFieldGUI =
                Instantiate(debuggableDropdownFieldGUIPrefab, debuggableDropdownFieldGUIPrefab.transform.parent);
            debuggableFieldGUI.gameObject.SetActive(IsExpanded);
            debuggableFieldGUI.Initialize(targetObject, fieldInfo, debuggableFieldAttribute); 
            nameToDebuggableActionGUICache.Add(fieldInfo.Name, debuggableFieldGUI);
        }
        
        public void AddDebuggableToggleField(object targetObject, FieldInfo fieldInfo, DebuggableFieldAttribute debuggableFieldAttribute)
        {
            if (nameToDebuggableActionGUICache.ContainsKey(fieldInfo.Name))
            {
                Debug.LogError($"Field with the same name {fieldInfo.Name} already exist on group: {groupName.text}, ignoring it");
                return;
            }
            
            DebuggableToggleFieldGUI debuggableFieldGUI =
                Instantiate(debuggableToggleFieldGUIPrefab, debuggableToggleFieldGUIPrefab.transform.parent);
            debuggableFieldGUI.gameObject.SetActive(IsExpanded);
            debuggableFieldGUI.Initialize(targetObject, fieldInfo, debuggableFieldAttribute); 
            nameToDebuggableActionGUICache.Add(fieldInfo.Name, debuggableFieldGUI);
        }
        
        public void AddDebuggableField(object targetObject, FieldInfo fieldInfo, DebuggableFieldAttribute debuggableFieldAttribute)
        {
            if (nameToDebuggableActionGUICache.ContainsKey(fieldInfo.Name))
            {
                Debug.LogError($"Field with the same name {fieldInfo.Name} already exist on group: {groupName.text}, ignoring it");
                return;
            }

            DebuggableFieldGUI debuggableFieldGUI =
                Instantiate(debuggableFieldGUIPrefab, debuggableFieldGUIPrefab.transform.parent);
            debuggableFieldGUI.gameObject.SetActive(IsExpanded);
            debuggableFieldGUI.Initialize(targetObject, fieldInfo, debuggableFieldAttribute); 
            nameToDebuggableActionGUICache.Add(fieldInfo.Name, debuggableFieldGUI);
        }

        public void AddDebuggableRangeField(object targetObject, FieldInfo fieldInfo, DebuggableFieldAttribute debuggableFieldAttribute, RangeAttribute rangeAttribute)
        {
            if (nameToDebuggableActionGUICache.ContainsKey(fieldInfo.Name))
            {
                Debug.LogError($"Field with the same name {fieldInfo.Name} already exist on group: {groupName.text}, ignoring it");
                return;
            }

            DebuggableRangeFieldGUI debuggableRangeFieldGUI = Instantiate(
                debuggableRangeFieldGUIPrefab,
                debuggableRangeFieldGUIPrefab
                    .transform.parent);
            
            debuggableRangeFieldGUI.gameObject.SetActive(IsExpanded);
            debuggableRangeFieldGUI.Initialize(targetObject, fieldInfo, debuggableFieldAttribute, rangeAttribute); 
            nameToDebuggableActionGUICache.Add(fieldInfo.Name, debuggableRangeFieldGUI);
        }

        public void AddDebuggableTextArea(object targetObject, FieldInfo fieldInfo, DebuggableTextAreaAttribute debuggableFieldAttribute)
        {
            if (nameToDebuggableActionGUICache.ContainsKey(fieldInfo.Name))
            {
                Debug.LogError($"Field with the same name {fieldInfo.Name} already exist on group: {groupName.text}, ignoring it");
                return;
            }

            if (fieldInfo.FieldType != typeof(string))
            {
                Debug.LogError($"{typeof(DebuggableTextAreaAttribute)} only works with string variables");
                return;
            }
            
            DebuggableTextAreaGUI debuggableFieldGUI =
                Instantiate(debuggableTextAreaGUIPrefab, debuggableTextAreaGUIPrefab.transform.parent);
            debuggableFieldGUI.gameObject.SetActive(IsExpanded);
            debuggableFieldGUI.Initialize(targetObject, fieldInfo, debuggableFieldAttribute); 
            nameToDebuggableActionGUICache.Add(fieldInfo.Name, debuggableFieldGUI);
        }


        public void ResetSearch()
        {
            SetExpanded(IsExpanded);
        }
    }
}
