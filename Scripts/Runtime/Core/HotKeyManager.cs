using System;
using System.Collections.Generic;
using UnityEngine;

namespace BrunoMikoski.DebugTools.Core
{
    public sealed class HotKeyManager : MonoBehaviour
    {
        private Dictionary<string, DebuggableActionHotKeyData> hotkeyToData = new Dictionary<string, DebuggableActionHotKeyData>();

        public DebuggableActionHotKeyData AddHotkeyToCallback(string hotkey, string displayName, Action targetCallback)
        {
            if (hotkeyToData.ContainsKey(hotkey))
            {
                Debug.LogError($"Hotkey already in use by another Action, ignoring it");
                return null;
            }

            if (hotkeyToData.ContainsKey(hotkey))
            {
                Debug.LogError($"Duplicated hotkey between {displayName} and {hotkeyToData[hotkey].DisplayName}");
                return null;
            }
            
            DebuggableActionHotKeyData hotkeyData = new DebuggableActionHotKeyData(hotkey, displayName, targetCallback);
            if (!hotkeyData.IsValidShortcut)
            {
                Debug.LogError($"Invalid Hotkey setup for {displayName}");
                return null;
            }

            hotkeyToData.Add(hotkey, hotkeyData);
            return hotkeyData;
        }

        public void Clear()
        {
            hotkeyToData.Clear();
        }

        private void Update()
        {
            foreach (var keyToData in hotkeyToData)
            {
                keyToData.Value.TryTrigger();
            }
        }
    }
}