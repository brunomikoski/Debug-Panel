using System.Collections;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BrunoMikoski.DebugPanel.GUI
{
    internal class MultilineDebuggableStringGUI : DebuggableFieldBaseGUI
    {
        [SerializeField]
        private TMP_Text displayText;
        [SerializeField]
        private LayoutGroup layoutGroup;


        protected override void UpdateDisplayValue()
        {
            string displayTextText = GetValue<string>();
            if (displayTextText.Length != displayText.text.Length)
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
