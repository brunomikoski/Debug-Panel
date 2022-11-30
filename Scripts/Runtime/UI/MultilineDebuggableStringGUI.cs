using System.Collections;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BrunoMikoski.DebugTools.GUI
{
    internal class MultilineDebuggableStringGUI : DebuggableFieldGUIBase
    {
        [SerializeField]
        private TMP_InputField displayText;
        [SerializeField]
        private LayoutGroup layoutGroup;
        [SerializeField]
        private Button shareButton;

        private void Awake()
        {
#if NATIVE_SHARE_ENABLED && UNITY_ANDROID || UNITY_IOS
            shareButton.onClick.AddListener(OnClickShareButton);
            shareButton.gameObject.SetActive(true);
#else
            shareButton.gameObject.SetActive(false);
#endif
        }

        private void OnDestroy()
        {
            shareButton.onClick.RemoveListener(OnClickShareButton);
        }

        private void OnClickShareButton()
        {
#if NATIVE_SHARE_ENABLED
            new NativeShare().SetText(displayText.text).SetTitle($"{display.text} content").Share();
#endif
        }

        protected override void UpdateDisplayValue()
        {
            string displayTextText = GetValue<string>();
            if (!string.IsNullOrEmpty(displayTextText) && displayTextText.Length != displayText.text.Length)
            {
                displayText.text = displayTextText;
                DebugPanelGUI.StartCoroutine(ToggleLayoutGroupEnumerator());
            }
        }

        private IEnumerator ToggleLayoutGroupEnumerator()
        {
            layoutGroup.enabled = false;
            yield return null;
            layoutGroup.enabled = true;
        }

        public override bool CanBeUsedForField(FieldInfo targetFieldInfo)
        {
            if (targetFieldInfo.FieldType == typeof(string))
            {
                if (targetFieldInfo.HasAttribute<MultilineAttribute>())
                    return true;
            }

            return false;
        }
    }
}
