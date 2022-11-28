using UnityEngine;

namespace BrunoMikoski.DebugTools
{
    internal class HotKeyManager : MonoBehaviour
    {
        private bool hasCachedDebugPanel;
        private DebugPanel cachedDebugPanel;

        private DebugPanel DebugPanel
        {
            get
            {
                if (!hasCachedDebugPanel)
                {
                    cachedDebugPanel = GetComponent<DebugPanel>();
                    hasCachedDebugPanel = true;
                }

                return cachedDebugPanel;
            }
       }

#if UNITY_EDITOR
        private void Update()
        {
            int count = DebugPanel.ActiveDebuggableItems.Count;
            for (int i = 0; i < count; i++)
            {
                DebuggableItemBase debuggable = DebugPanel.ActiveDebuggableItems[i];
                if (debuggable is DebuggableInvokableBase invokableBase)
                {
                    if (string.IsNullOrEmpty(invokableBase.Hotkey))
                        continue;

                    if (invokableBase.HotkeyData.IsTriggered())
                    {
                        invokableBase.Invoke();
                        Debug.Log($"Invoking {invokableBase.Path} by hotkey");
                    }
                }
            }
        }
#endif
    }
}