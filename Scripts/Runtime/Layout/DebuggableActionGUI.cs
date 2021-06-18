using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BrunoMikoski.DebugTools.Layout
{
    public sealed class DebuggableActionGUI : DebuggableItemGUIBase
    {
        [SerializeField]
        private Button button;

        [SerializeField] 
        private TMP_Text label;

        [SerializeField] 
        private TMP_Text hotkeyText;

        private Action callback;

        public void Initialize(string displayName, Action action)
        {
            callback = action;
            label.text = displayName;
            button.onClick.AddListener(OnClick);
        }
        
        private void OnDestroy()
        {
            button.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            callback?.Invoke();
        }

        public void SetHumanReadableHotkey(string humanReadableHotKey)
        {
            if (!Application.isEditor)
                return;

            hotkeyText.text = humanReadableHotKey;
        }

    }
}
