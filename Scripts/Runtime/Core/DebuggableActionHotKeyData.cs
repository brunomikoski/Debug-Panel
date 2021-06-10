using System;
using UnityEngine;

namespace BrunoMikoski.DebugTools.Core
{
    public class DebuggableActionHotKeyData
    {
        private readonly KeyCode keyCode;
        private readonly bool alt;
        private readonly bool shift;
        private readonly bool ctrl;
        
        private readonly bool isValidShortcut;
        public bool IsValidShortcut => isValidShortcut;

        private readonly string displayName;
        public string DisplayName => displayName;

        private readonly Action callback;

        private readonly string humanReadableHotKey;
        public string HumanReadableHotKey => humanReadableHotKey;

        public DebuggableActionHotKeyData(string textHotKey, string targetDisplayName, Action targetCallback)
        {
            ctrl = textHotKey.Contains("%");
            shift = textHotKey.Contains("#");
            alt = textHotKey.Contains("&");
            textHotKey = textHotKey.Replace("%", "").Replace("#", "").Replace("&", "");

            if (Enum.TryParse(textHotKey.ToUpperInvariant(), out keyCode))
                isValidShortcut = true;

            if (!isValidShortcut)
                return;
            
            displayName = targetDisplayName;
            callback = targetCallback;

            humanReadableHotKey = "";
            if (ctrl)
                humanReadableHotKey += "Ctrl+";
            if (alt)
                humanReadableHotKey += "Alt+";
            if (shift)
                humanReadableHotKey += "Shift+";

            humanReadableHotKey += keyCode.ToString();
        }


        private bool IsTriggered()
        {
            if (!isValidShortcut)
                return false;

            if (Input.GetKeyDown(keyCode) 
                && (!alt || alt && (Input.GetKey(KeyCode.AltGr) || Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)))
                && (!shift || shift && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))) 
                && (!ctrl || ctrl && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) 
                                                                        || Input.GetKey(KeyCode.LeftApple) 
                                                                        || Input.GetKey(KeyCode.RightApple) 
                                                                        || Input.GetKey(KeyCode.LeftCommand) 
                                                                        || Input.GetKey(KeyCode.RightCommand))))
            {
                return true;
            }

            return false;
        }

        public void TryTrigger()
        {
            if (!IsTriggered())
                return;
            
            callback.Invoke();
        }
    }
}