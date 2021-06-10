using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BrunoMikoski.DebugTools.Layout
{
    public sealed class DebuggableActionGUI : DebuggableItemBaseGUI
    {
        [SerializeField]
        private Button button;

        [SerializeField] 
        private TMP_Text label;

        [SerializeField] 
        private TMP_Text hotkeyText;

        private Action callback;

        private void Awake()
        {
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

        public void Initialize(string displayName, Action action)
        {
            callback = action;
            label.text = displayName;
        }

        public void SetHumanReadableHotkey(string humanReadableHotKey)
        {
            hotkeyText.text = humanReadableHotKey;
        }
    }
}