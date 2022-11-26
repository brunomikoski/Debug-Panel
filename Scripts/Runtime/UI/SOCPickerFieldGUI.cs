// #undef SOC_ENABLED

using System.Reflection;
using TMPro;
using UnityEngine;

#if SOC_ENABLED
using System;
using System.Collections.Generic;
using BrunoMikoski.ScriptableObjectCollections;
using UnityEngine.UI;

namespace BrunoMikoski.DebugPanel.GUI
{
    internal sealed class SOCPickerFieldGUI : DebuggableFieldGUIBase
    {
        [SerializeField]
        private Button button;

        [SerializeField] 
        private TMP_Text displayField;

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
            DebugPage pickerOptionsDebugPage = new DebugPage($"{DebugPage.PagePath}{displayField.text}",
                $"{selectableType.Name} Options", "");
            
            pickerOptionsDebugPage.SetParentPage(DebugPage);

            List<ScriptableObjectCollection> collection = CollectionsRegistry.Instance.GetCollectionsByItemType(selectableType);

            for (int i = 0; i < collection.Count; i++)
            {
                ScriptableObjectCollection scriptableObjectCollection = collection[i];
                for (int j = 0; j < scriptableObjectCollection.Items.Count; j++)
                {
                    ScriptableObjectCollectionItem scriptableObjectCollectionItem = scriptableObjectCollection.Items[j];
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
            
            displayField.text = displayValue;
        }

        public override bool CanBeUsedForField(FieldInfo targetFieldInfo)
        {
            return typeof(ScriptableObjectCollectionItem).IsAssignableFrom(targetFieldInfo.FieldType);
        }
    }
}
#else
namespace BrunoMikoski.DebugPanel.GUI
{
    internal sealed class SOCPickerFieldGUI : DebuggableFieldGUIBase
    {
        [SerializeField] 
        private TMP_Text displayField;

        protected override void UpdateDisplayValue()
        {
            
        }

        public override bool CanBeUsedForField(FieldInfo targetFieldInfo)
        {
            return false;
        }
    }
}
#endif