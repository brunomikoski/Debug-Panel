using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace BrunoMikoski.DebugPanel.GUI
{
    internal sealed class DebuggableToggleFieldGUI : DebuggableFieldBaseGUI
    {
        [SerializeField]
        private LayoutElement layoutElement;
        [SerializeField]
        private RectTransform contents;
        [SerializeField]
        private CanvasGroup contentsCanvasGroup;
        [SerializeField]
        private Toggle toggle;

        private static Type[] DisplayableFieldInfoTypes => new[] { typeof(bool) };

        internal override void Initialize(DebuggableItemBase targetDebuggableItem, DebugPage targetDebugPage)
        {
            base.Initialize(targetDebuggableItem, targetDebugPage);

            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener(OnToggleValueChanged);

            int height = 76;
            if (!string.IsNullOrEmpty(targetDebuggableItem.SubTitle))
                height = 42;
            
            height += 36;
            height += 1;
            contents.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            layoutElement.preferredHeight = height;
        }

        private void OnToggleValueChanged(bool newValue)
        {
            SetValue(newValue);
        }

        protected override void UpdateDisplayValue()
        {
            toggle.SetIsOnWithoutNotify(GetValue<bool>());
        }

        public override bool CanBeUsedForField(FieldInfo targetFieldInfo)
        {
            return targetFieldInfo.FieldType == typeof(bool); 
        }
    }
}