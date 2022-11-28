using System;
using System.Reflection;

namespace BrunoMikoski.DebugTools.GUI
{
    internal sealed class PickerEnumFieldGUI : PickerFieldGUIBase
    {
        private Type enumType;
        private Enum enumValue;
        private DebuggableEnumItem debuggableEnumItem;

        protected override void UpdateDisplayValue()
        {
            Enum value = (Enum)Enum.Parse(enumType, display.text);

            enumValue = GetValue<Enum>();
            bool isCurrentItem = Equals(value, enumValue);
            toggle.SetIsOnWithoutNotify(isCurrentItem);
        }

        protected override void OnToggleValueChanged(bool isSelected)
        {
            if (!isSelected) 
                return;

            Enum value = (Enum)Enum.Parse(enumType, display.text);
 
            enumValue = GetValue<Enum>();
            bool isCurrentItem = Equals(value, enumValue);
            toggle.SetIsOnWithoutNotify(isCurrentItem);
            SetValue(value);
        }

        internal override void Initialize(DebuggableItemBase targetDebuggableItem, DebugPage targetDebugPage)
        {
            debuggableEnumItem = (DebuggableEnumItem) targetDebuggableItem;
            enumType = debuggableEnumItem.FieldInfo.FieldType;
            base.Initialize(targetDebuggableItem, targetDebugPage);
            display.text = debuggableEnumItem.Title;
        }

        public override bool CanBeUsedForField(FieldInfo targetFieldInfo)
        {
            return targetFieldInfo.FieldType.IsEnum;
        }
    }
}