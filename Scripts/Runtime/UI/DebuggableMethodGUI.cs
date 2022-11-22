using System;

namespace BrunoMikoski.DebugPanel.GUI
{
    public class DebuggableMethodGUI : DebuggableGUIBase
    {
        public override Type[] DisplayTypes => new[] { typeof(DebuggableMethod), typeof(DebuggableAction) };

        private DebuggableMethod debuggableMethod;
        private DebuggableAction debuggableAction;
        

        public override void Initialize(DebuggableItemBase targetDebuggableItem, DebugPage targetDebugPage)
        {
            base.Initialize(targetDebuggableItem, targetDebugPage);
            if (targetDebuggableItem is DebuggableMethod targetDebuggableMethod)
                debuggableMethod = targetDebuggableMethod;
            else if (targetDebuggableItem is DebuggableAction targetDebuggableAction)
                debuggableAction = targetDebuggableAction;
        }

        protected override void OnClick()
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
