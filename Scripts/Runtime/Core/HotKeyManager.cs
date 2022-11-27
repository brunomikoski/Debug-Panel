using UnityEngine;

namespace BrunoMikoski.DebugPanel
{
    [RequireComponent(typeof(DebugPanelService))]
    internal class HotKeyManager : MonoBehaviour
    {
        private bool hasCachedDebugPanel;
        private DebugPanelService cachedDebugPanelService;

        private DebugPanelService debugPanelService
        {
            get
            {
                if (!hasCachedDebugPanel)
                {
                    cachedDebugPanelService = GetComponent<DebugPanelService>();
                    hasCachedDebugPanel = true;
                }

                return cachedDebugPanelService;
            }
       }

#if UNITY_EDITOR
        private void Update()
        {
            int count = debugPanelService.ActiveDebuggableItems.Count;
            for (int i = 0; i < count; i++)
            {
                DebuggableItemBase debuggable = debugPanelService.ActiveDebuggableItems[i];
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