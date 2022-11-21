using System.Reflection;
using TMPro;
using UnityEngine;

namespace BrunoMikoski.DebugPanel.GUI
{
    public class MultilineDebuggableStringGUI : DebuggableFieldBaseGUI
    {
        [SerializeField]
        private TMP_InputField inputField;

        protected override void UpdateDisplayValue()
        {
            inputField.text = GetValue<string>();
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
