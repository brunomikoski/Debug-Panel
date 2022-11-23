using UnityEngine;

namespace BrunoMikoski.DebugPanel
{
    internal class HotKeyManager : MonoBehaviour
    {
        [SerializeField]
        private DebugPanel debugPanel;
        
#if UNITY_EDITOR
        private void Update()
        {
            int count = debugPanel.ActiveDebuggableItems.Count;
            for (int i = 0; i < count; i++)
            {
                DebuggableItemBase debuggable = debugPanel.ActiveDebuggableItems[i];
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