using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.Controls;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
#endif

namespace BrunoMikoski.DebugTools
{
    internal static class InputCompat
    {
        public static bool GetKey(KeyCode key)
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current == null)
                return false;

            if (TryMapKeyCode(key, out Key mapped))
                return Keyboard.current[mapped].isPressed;

            return false;
#elif ENABLE_LEGACY_INPUT_MANAGER
            return Input.GetKey(key);
#else
            return false;
#endif
        }

        public static bool GetKeyDown(KeyCode key)
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current == null)
                return false;

            if (TryMapKeyCode(key, out Key mapped))
                return Keyboard.current[mapped].wasPressedThisFrame;

            return false;
#elif ENABLE_LEGACY_INPUT_MANAGER
            return Input.GetKeyDown(key);
#else
            return false;
#endif
        }

        public static int TouchCount()
        {
#if ENABLE_INPUT_SYSTEM
            if (!EnhancedTouchSupport.enabled)
                EnhancedTouchSupport.Enable();

            ReadOnlyArray<Touch> active = Touch.activeTouches;
            return active.Count;

#elif ENABLE_LEGACY_INPUT_MANAGER
            return Input.touchCount;
#else
            return 0;
#endif
        }

#if ENABLE_INPUT_SYSTEM
        private static bool TryMapKeyCode(KeyCode keyCode, out Key key)
        {
            switch (keyCode)
            {
                // Letters A-Z
                case KeyCode.A: key = Key.A; return true;
                case KeyCode.B: key = Key.B; return true;
                case KeyCode.C: key = Key.C; return true;
                case KeyCode.D: key = Key.D; return true;
                case KeyCode.E: key = Key.E; return true;
                case KeyCode.F: key = Key.F; return true;
                case KeyCode.G: key = Key.G; return true;
                case KeyCode.H: key = Key.H; return true;
                case KeyCode.I: key = Key.I; return true;
                case KeyCode.J: key = Key.J; return true;
                case KeyCode.K: key = Key.K; return true;
                case KeyCode.L: key = Key.L; return true;
                case KeyCode.M: key = Key.M; return true;
                case KeyCode.N: key = Key.N; return true;
                case KeyCode.O: key = Key.O; return true;
                case KeyCode.P: key = Key.P; return true;
                case KeyCode.Q: key = Key.Q; return true;
                case KeyCode.R: key = Key.R; return true;
                case KeyCode.S: key = Key.S; return true;
                case KeyCode.T: key = Key.T; return true;
                case KeyCode.U: key = Key.U; return true;
                case KeyCode.V: key = Key.V; return true;
                case KeyCode.W: key = Key.W; return true;
                case KeyCode.X: key = Key.X; return true;
                case KeyCode.Y: key = Key.Y; return true;
                case KeyCode.Z: key = Key.Z; return true;

                // Number row 0-9
                case KeyCode.Alpha0: key = Key.Digit0; return true;
                case KeyCode.Alpha1: key = Key.Digit1; return true;
                case KeyCode.Alpha2: key = Key.Digit2; return true;
                case KeyCode.Alpha3: key = Key.Digit3; return true;
                case KeyCode.Alpha4: key = Key.Digit4; return true;
                case KeyCode.Alpha5: key = Key.Digit5; return true;
                case KeyCode.Alpha6: key = Key.Digit6; return true;
                case KeyCode.Alpha7: key = Key.Digit7; return true;
                case KeyCode.Alpha8: key = Key.Digit8; return true;
                case KeyCode.Alpha9: key = Key.Digit9; return true;

                // Numpad 0-9
                case KeyCode.Keypad0: key = Key.Numpad0; return true;
                case KeyCode.Keypad1: key = Key.Numpad1; return true;
                case KeyCode.Keypad2: key = Key.Numpad2; return true;
                case KeyCode.Keypad3: key = Key.Numpad3; return true;
                case KeyCode.Keypad4: key = Key.Numpad4; return true;
                case KeyCode.Keypad5: key = Key.Numpad5; return true;
                case KeyCode.Keypad6: key = Key.Numpad6; return true;
                case KeyCode.Keypad7: key = Key.Numpad7; return true;
                case KeyCode.Keypad8: key = Key.Numpad8; return true;
                case KeyCode.Keypad9: key = Key.Numpad9; return true;

                // Common controls
                case KeyCode.Space: key = Key.Space; return true;
                case KeyCode.Escape: key = Key.Escape; return true;
                case KeyCode.Return: key = Key.Enter; return true;
                case KeyCode.KeypadEnter: key = Key.NumpadEnter; return true;
                case KeyCode.Tab: key = Key.Tab; return true;
                case KeyCode.Backspace: key = Key.Backspace; return true;

                // Modifiers
                case KeyCode.LeftShift: key = Key.LeftShift; return true;
                case KeyCode.RightShift: key = Key.RightShift; return true;
                case KeyCode.LeftControl: key = Key.LeftCtrl; return true;
                case KeyCode.RightControl: key = Key.RightCtrl; return true;
                case KeyCode.LeftAlt: key = Key.LeftAlt; return true;
                case KeyCode.RightAlt: key = Key.RightAlt; return true;
                case KeyCode.AltGr: key = Key.AltGr; return true;

                // Map Apple/Command to Meta to keep things simple and portable
                case KeyCode.LeftCommand: key = Key.LeftMeta; return true;
                case KeyCode.RightCommand: key = Key.RightMeta; return true;
            }

            // Try direct enum parse as a fallback (covers F-keys, arrows, etc.)
            if (System.Enum.TryParse<Key>(keyCode.ToString(), true, out key))
                return true;

            // Special handling for F1..F15 if name differences ever occur
            if (keyCode >= KeyCode.F1 && keyCode <= KeyCode.F15)
            {
                int fn = (int)keyCode - (int)KeyCode.F1 + 1;
                if (System.Enum.TryParse<Key>("F" + fn, out key))
                    return true;
            }

            key = default;
            return false;
        }
#endif
    }
}
