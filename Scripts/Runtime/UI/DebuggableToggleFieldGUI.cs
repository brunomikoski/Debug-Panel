using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace BrunoMikoski.DebugPanel.GUI
{
    public sealed class DebuggableToggleFieldGUI : DebuggableFieldBaseGUI
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

        public override void Initialize(DebuggableItemBase targetDebuggableItem, DebugPage targetDebugPage)
        {
            base.Initialize(targetDebuggableItem, targetDebugPage);

            // _contentsCanvasGroup.alpha = model.Interactable ? 1.0f : 0.3f;

            // Toggle
            // toggle.interactable = model.Interactable;
            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener(OnToggleValueChanged);

            int height = 76;
            // if (!string.IsNullOrEmpty(targetDebuggableItem.SubTitle) || targetDebuggableItem.Icon != null)
            if (!string.IsNullOrEmpty(targetDebuggableItem.SubTitle))
                height = 42;
            
            height += 36; // Padding
            height += 1; // Border
            contents.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            layoutElement.preferredHeight = height; // Set the preferred height for the recycler view.
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