using System;
using TMPro;
using UnityEngine;

namespace BrunoMikoski.DebugTools.GUI
{
    internal class DebuggableMethodGUI : DebuggableGUIBase
    {
        [SerializeField]
        private TMP_Text hotKeyText;
        
        public override Type[] DisplayTypes => new[] { typeof(DebuggableMethod), typeof(DebuggableAction) };

        private DebuggableInvokableBase debuggableInvokable;

        internal override void Initialize(DebuggableItemBase targetDebuggableItem, DebugPage targetDebugPage)
        {
            base.Initialize(targetDebuggableItem, targetDebugPage);

            debuggableInvokable = (DebuggableInvokableBase)targetDebuggableItem; 
            if (debuggableInvokable.HotkeyData != null && !string.IsNullOrEmpty(debuggableInvokable.HotkeyData.HumanReadableHotKey))
                hotKeyText.text = debuggableInvokable.HotkeyData.HumanReadableHotKey;
        }

        protected override void OnClick()
        {
            debuggableInvokable.Invoke();
        }

        private void OnEnable()
        {
            if (selectable != null && debuggableInvokable is DebuggableMethod debuggableMethod)
            {
                selectable.interactable = debuggableMethod.IsMethodEnabled();
            }
        }
    }
}