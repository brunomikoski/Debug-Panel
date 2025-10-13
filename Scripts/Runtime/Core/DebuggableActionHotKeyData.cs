using System;
using UnityEngine;

namespace BrunoMikoski.DebugTools
{
    [Serializable]
    internal class DebuggableActionHotKeyData
    {
        [SerializeField]
        private string hotKey;
        
        private readonly KeyCode[] keyCodes = Array.Empty<KeyCode>();
        private readonly bool alt;
        private readonly bool shift;
        private readonly bool ctrl;

        private readonly bool isValidShortcut;
        public bool IsValidShortcut => isValidShortcut;

        private readonly string displayName;
        public string DisplayName => displayName;

        private readonly string humanReadableHotKey;
        public string HumanReadableHotKey => humanReadableHotKey;

        public DebuggableActionHotKeyData(string textHotKey)
        {
            ctrl = textHotKey.Contains("%");
            shift = textHotKey.Contains("#");
            alt = textHotKey.Contains("&");
            textHotKey = textHotKey.Replace("%", "").Replace("#", "").Replace("&", "");


            if (int.TryParse(textHotKey, out int intValue))
            {
                keyCodes = new KeyCode[2];

                if (Enum.TryParse($"Alpha{intValue}", out KeyCode alphaKeyCode))
                {
                    if (alphaKeyCode != KeyCode.None)
                    {
                        keyCodes[0] = alphaKeyCode;
                        isValidShortcut = true;
                    }
                }

                if (Enum.TryParse($"Keypad{intValue}", out KeyCode keypadKeyCode))
                {
                    if (alphaKeyCode != KeyCode.None)
                    {
                        keyCodes[1] = keypadKeyCode;
                        isValidShortcut = true;
                    }
                }
            }
            else
            {
                if (Enum.TryParse(textHotKey.ToUpperInvariant(), out KeyCode keyCode))
                {
                    if (keyCode != KeyCode.None)
                    {
                        keyCodes = new[] {keyCode};
                        isValidShortcut = true;
                    }
                }

            }

            if (!isValidShortcut)
                return;

            humanReadableHotKey = "";
            if (ctrl)
                humanReadableHotKey += "Ctrl+";
            if (alt)
                humanReadableHotKey += "Alt+";
            if (shift)
                humanReadableHotKey += "Shift+";

            humanReadableHotKey += string.Join(" or ", keyCodes);
        }


        public bool IsTriggered()
        {
            if (!isValidShortcut)
                return false;

            if (IsAnyOfKeyCodesDown()
                && (!alt || alt && (InputCompat.GetKey(KeyCode.AltGr) || InputCompat.GetKey(KeyCode.LeftAlt) ||
                                    InputCompat.GetKey(KeyCode.RightAlt)))
                && (!shift || shift && (InputCompat.GetKey(KeyCode.LeftShift) || InputCompat.GetKey(KeyCode.RightShift)))
                && (!ctrl || ctrl && (InputCompat.GetKey(KeyCode.LeftControl) || InputCompat.GetKey(KeyCode.RightControl)
                                                                        || InputCompat.GetKey(KeyCode.LeftApple)
                                                                        || InputCompat.GetKey(KeyCode.RightApple)
                                                                        || InputCompat.GetKey(KeyCode.LeftCommand)
                                                                        || InputCompat.GetKey(KeyCode.RightCommand))))
            {
                return true;
            }

            return false;
        }

        private bool IsAnyOfKeyCodesDown()
        {
            for (int i = 0; i < keyCodes.Length; i++)
            {
                if (InputCompat.GetKeyDown(keyCodes[i]))
                    return true;
            }

            return false;
        }
    }
}
