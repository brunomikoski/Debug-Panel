using System;
using UnityEngine;
using UnityEngine.UI;

namespace BrunoMikoski.DebugPanel.GUI
{
    public class DebuggableMethodGUI : DebuggableGUIBase
    {
        public override Type[] DisplayTypes => new[] { typeof(DebuggableMethod), typeof(DebuggableAction) };

        [SerializeField]
        private Button button;

        private DebuggableMethod debuggableMethod;
        private DebuggableAction debuggableAction;

        private void Awake()
        {
            button.onClick.AddListener(OnButtonClick);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(OnButtonClick);
        }

        public override void Initialize(DebuggableItemBase targetDebuggableItem, DebugPage targetDebugPage)
        {
            base.Initialize(targetDebuggableItem, targetDebugPage);
            if (targetDebuggableItem is DebuggableMethod targetDebuggableMethod)
                debuggableMethod = targetDebuggableMethod;
            else if (targetDebuggableItem is DebuggableAction targetDebuggableAction)
                debuggableAction = targetDebuggableAction;
        }

        private void OnButtonClick()
        {
            if (debuggableMethod != null)
                debuggableMethod.Invoke();
            else if(debuggableAction != null)
                debuggableAction.Invoke();


            if (DebugPanel.HideAfterInvoke)
                DebugPanel.Hide();
        }

    }
}
