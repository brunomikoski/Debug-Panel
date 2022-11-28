using System;
using UnityEngine;

namespace BrunoMikoski.DebugTools
{
    [Serializable]
    public class DebugPanelTriggerSettings
    {
        [Header("Mobile")]
        [SerializeField]
        private int fingerCount = 3;
        [SerializeField]
        private float seconds = 2;

        [Header("Keyboard")] 
        [SerializeField]
        private string hotkey = "&0";
        
        private bool hotkeyCreated;
        private DebuggableActionHotKeyData cachedHotKeyData;
        private DebuggableActionHotKeyData HotKeyData
        {
            get
            {
                if (!hotkeyCreated)
                {
                    cachedHotKeyData = new DebuggableActionHotKeyData(hotkey);
                    hotkeyCreated = true;
                }

                return cachedHotKeyData;
            }
        }

        private float touchHeldTime;


        public bool IsTriggered()
        {
            if (TouchBasedInputTriggered() || HotKeyData.IsTriggered())
            {
                return true;
            }

            return false;
        }

        private bool TouchBasedInputTriggered()
        {
            if (Input.touchCount == fingerCount)
            {
                touchHeldTime += Time.deltaTime;
                if (touchHeldTime >= seconds)
                {
                    touchHeldTime = 0;
                    return true;
                }
            }
            else
            {
                touchHeldTime = 0;
            }

            return false;
        }
    }
}