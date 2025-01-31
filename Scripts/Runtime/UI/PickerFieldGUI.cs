using System;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BrunoMikoski.DebugTools.GUI
{
    internal sealed class PickerFieldGUI : DebuggableFieldGUIBase
    {
        [SerializeField]
        private TMP_Text displayField;

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
            selectable.interactable = false;
        }

        private void OnButtonClick()
        {
            DebugPage pickerOptionsDebugPage = new DebugPage($"{DebugPage.PagePath}{displayField.text}",
                $"{selectableType.Name} Options", "");
            
            pickerOptionsDebugPage.SetParentPage(DebugPage);

            
            for (int i = 0; i < Enum.GetNames(selectableType).Length; i++)
            {
                string enumDisplayName = Enum.GetNames(selectableType)[i];
                DebuggableEnumItem debuggableEnumItem = new DebuggableEnumItem(enumDisplayName,
                    debuggableField.FieldInfo, debuggableField.Owner, debuggableField.ClassAttribute,
                    debuggableField.FieldAttribute);
                
                pickerOptionsDebugPage.AddItem(debuggableEnumItem);
            }
            
            DebugPanel.DisplayPage(pickerOptionsDebugPage);
        }

        internal override void Initialize(DebuggableItemBase targetDebuggableItem, DebugPage targetDebugPage)
        {
            debuggableField = (DebuggableField)targetDebuggableItem;
            selectableType = debuggableField.FieldInfo.FieldType;

            base.Initialize(targetDebuggableItem, targetDebugPage);
        }

        protected override void UpdateDisplayValue()
        {
            displayField.text = GetValue<Enum>().ToString();
        }

        protected override bool SetValue<T>(T targetValue)
        {
            return false;
        }

        public override bool CanBeUsedForField(FieldInfo targetFieldInfo)
        {
            if (targetFieldInfo.FieldType.IsEnum)
                return true;
            return false;
        }
    }
}