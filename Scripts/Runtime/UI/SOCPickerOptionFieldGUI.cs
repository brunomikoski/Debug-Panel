// #undef SOC_ENABLED

using System.Reflection;
using BrunoMikoski.DebugTools;
using BrunoMikoski.DebugTools.GUI;

#if SOC_ENABLED
using BrunoMikoski.ScriptableObjectCollections;
#endif

namespace BrunoMikoski.DebugTools.GUI
{
    internal sealed class SOCPickerOptionFieldGUI : PickerFieldGUIBase
    {
#if SOC_ENABLED
        private DebuggableSOCItem debuggableSOCItem;

        protected override bool AllowToggleGroupToBeOff => true;

        protected override void UpdateDisplayValue()
        {
            ScriptableObjectCollectionItem fieldValue = GetValue<ScriptableObjectCollectionItem>();

            toggle.SetIsOnWithoutNotify(fieldValue != null && fieldValue == debuggableSOCItem.CollectionItem);
        }

        protected override void OnToggleValueChanged(bool isSelected)
        {
            toggle.SetIsOnWithoutNotify(isSelected);
            if (isSelected)
                SetValue(debuggableSOCItem.CollectionItem);
        }

        internal override void Initialize(DebuggableItemBase targetDebuggableItem, DebugPage targetDebugPage)
        {
            debuggableSOCItem = (DebuggableSOCItem) targetDebuggableItem;
            base.Initialize(targetDebuggableItem, targetDebugPage);
            display.text = debuggableSOCItem.CollectionItem.name;
        }

        public override bool CanBeUsedForField(FieldInfo targetFieldInfo)
        {
            return typeof(ScriptableObjectCollectionItem).IsAssignableFrom(targetFieldInfo.FieldType);
        }
#else
        protected override bool AllowToggleGroupToBeOff => true;

        protected override void UpdateDisplayValue()
        {
        }

        protected override void OnToggleValueChanged(bool isSelected)
        {
        }

        public override bool CanBeUsedForField(FieldInfo targetFieldInfo)
        {
            return false;
        }
#endif
    }
}
