using System;
using UnityEngine;
using UnityEngine.UI;

namespace BrunoMikoski.DebugPanel.GUI
{
    internal abstract class PickerFieldGUIBase : DebuggableFieldBaseGUI
    {
        [SerializeField] 
        protected Toggle toggle;
        
        private ToggleGroup cachedToggleGroup;
        private ToggleGroup ToggleGroup
        {
            get
            {
                if (cachedToggleGroup == null)
                    cachedToggleGroup = GetComponentInParent<ToggleGroup>();
                return cachedToggleGroup;
            }
        }

        protected virtual bool AllowToggleGroupToBeOff => false;

#if SOC_ENABLED
        public override Type[] DisplayTypes => new[] {typeof(DebuggableEnumItem), typeof(DebuggableSOCItem)};
#else
        public override Type[] DisplayTypes => new[] {typeof(DebuggableEnumItem)};
#endif
        

        protected virtual void Awake()
        {
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }
        protected virtual void OnDestroy()
        {
            toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
        }

        protected abstract void OnToggleValueChanged(bool isSelected);

        internal override void Initialize(DebuggableItemBase targetDebuggableItem, DebugPage targetDebugPage)
        {
            toggle.group = ToggleGroup;
            toggle.group.allowSwitchOff = AllowToggleGroupToBeOff;
            
            base.Initialize(targetDebuggableItem, targetDebugPage);
            UpdateDisplayValue();
        }
    }
}