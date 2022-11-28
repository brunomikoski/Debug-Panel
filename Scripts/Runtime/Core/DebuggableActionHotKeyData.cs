using System;
using UnityEngine;

namespace BrunoMikoski.DebugTools
{
    [Serializable]
    internal class DebuggableActionHotKeyData
    {
        [SerializeField]
        private string hotKey;
        
        private readonly KeyCode[] keyCodes = new KeyCode[0];
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
                && (!alt || alt && (Input.GetKey(KeyCode.AltGr) || Input.GetKey(KeyCode.LeftAlt) ||
                                    Input.GetKey(KeyCode.RightAlt)))
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

        private bool IsAnyOfKeyCodesDown()
        {
            for (int i = 0; i < keyCodes.Length; i++)
            {
                if (Input.GetKeyDown(keyCodes[i]))
                    return true;
            }

            return false;
        }
    }
}