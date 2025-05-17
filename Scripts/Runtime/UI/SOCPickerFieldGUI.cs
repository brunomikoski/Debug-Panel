// #undef SOC_ENABLED

using System.Reflection;
using TMPro;
using UnityEngine;
using System;
using System.Collections.Generic;
using BrunoMikoski.DebugTools;
using BrunoMikoski.DebugTools.GUI;
using UnityEngine.UI;

#if SOC_ENABLED
using BrunoMikoski.ScriptableObjectCollections;
#endif


namespace BrunoMikoski.DebugTools.GUI
{
    internal sealed class SOCPickerFieldGUI : DebuggableFieldGUIBase
    {
#if SOC_ENABLED
        [SerializeField]
        private Button button;

        
        private Type selectableType;

        private void Awake()
        {
            button.onClick.AddListener(OnButtonClick);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(OnButtonClick);
        }

        protected override void SetAsReadOnly()
        {
            base.SetAsReadOnly();
            button.enabled = false;
        }

        internal override void Initialize(DebuggableItemBase targetDebuggableItem, DebugPage targetDebugPage)
        {
            base.Initialize(targetDebuggableItem, targetDebugPage);
            selectableType = debuggableField.FieldInfo.FieldType;
        }

        private void OnButtonClick()
        {
            DebugPage pickerOptionsDebugPage = new DebugPage($"{DebugPage.PagePath}{display.text}",
                $"{selectableType.Name} Options", "");
            
            pickerOptionsDebugPage.SetParentPage(DebugPage);

            List<ScriptableObjectCollection> collection = CollectionsRegistry.Instance.GetCollectionsByItemType(selectableType);

            for (int i = 0; i < collection.Count; i++)
            {
                ScriptableObjectCollection scriptableObjectCollection = collection[i];
                for (int j = 0; j < scriptableObjectCollection.Items.Count; j++)
                {
                    ScriptableObject scriptableObjectCollectionItem = scriptableObjectCollection.Items[j];
                    DebuggableSOCItem debuggableSOCItem = new DebuggableSOCItem(scriptableObjectCollectionItem.name,
                        debuggableField.FieldInfo, debuggableField.Owner, debuggableField.ClassAttribute,
                        debuggableField.FieldAttribute, scriptableObjectCollectionItem);
                    
                    pickerOptionsDebugPage.AddItem(debuggableSOCItem);

                }
            }
            
            DebugPanel.DisplayPage(pickerOptionsDebugPage);
        }

        protected override void UpdateDisplayValue()
        {
            string displayValue = "None";

            ScriptableObjectCollectionItem collectionItem = GetValue<ScriptableObjectCollectionItem>();
            if (collectionItem != null)
                displayValue = collectionItem.name;
            
            display.text = displayValue;
        }

        public override bool CanBeUsedForField(FieldInfo targetFieldInfo)
        {
            return typeof(ScriptableObjectCollectionItem).IsAssignableFrom(targetFieldInfo.FieldType);
        }
#else
        protected override void UpdateDisplayValue()
        {
            
        }

        public override bool CanBeUsedForField(FieldInfo targetFieldInfo)
        {
            return false;
        }
#endif
    }
}