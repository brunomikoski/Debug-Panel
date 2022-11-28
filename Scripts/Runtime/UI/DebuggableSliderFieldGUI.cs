using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BrunoMikoski.DebugTools.GUI
{
    internal sealed class DebuggableSliderFieldGUI : DebuggableFieldGUIBase
    {
        [SerializeField]
        private TMP_InputField inputField;

        [SerializeField]
        private Slider slider;

        private RangeAttribute rangeAttribute;

        internal override void Initialize(DebuggableItemBase targetDebuggableItem, DebugPage targetDebugPage)
        {
            debuggableField = (DebuggableField)targetDebuggableItem;
            debuggableField.FieldInfo.TryGetAttribute(out rangeAttribute);
            slider.minValue = rangeAttribute.min;
            slider.maxValue = rangeAttribute.max;
            slider.onValueChanged.AddListener(OnSliderValueChanged);
            inputField.onSubmit.AddListener(OnInputSubmitted);
            
            if (debuggableField.FieldInfo.FieldType == typeof(float))
                inputField.contentType = TMP_InputField.ContentType.DecimalNumber;
            else
                inputField.contentType = TMP_InputField.ContentType.IntegerNumber;

            base.Initialize(targetDebuggableItem, targetDebugPage);
        }

        private void OnDestroy()
        {
            slider.onValueChanged.RemoveListener(OnSliderValueChanged);
            inputField.onSubmit.RemoveListener(OnInputSubmitted);
        }

        protected override void SetAsReadOnly()
        {
            base.SetAsReadOnly();
            slider.interactable = false;
            inputField.interactable = false;
        }

        private void OnInputSubmitted(string stringValue)
        {
            if (debuggableField.FieldInfo.FieldType == typeof(float))
            {
                if (float.TryParse(stringValue, out float floatResult))
                {
                    SetValue(Mathf.Clamp(floatResult, rangeAttribute.min, rangeAttribute.max));
                }
            }
            else
            {
                if (int.TryParse(stringValue, out int intResult))
                {
                    SetValue((int)Mathf.Clamp(intResult, rangeAttribute.min, rangeAttribute.max));
                }
            }
            UpdateDisplayValue();
        }
        private void OnSliderValueChanged(float newValue)
        {
            if (debuggableField.FieldInfo.FieldType == typeof(float))
            {
                SetValue(newValue);
            }
            else
            {
                SetValue(Mathf.RoundToInt(newValue));
            }
            UpdateDisplayValue();
        }
        

        protected override void UpdateDisplayValue()
        {
            string stringValue = GetValue<object>().ToString();
            if (string.IsNullOrEmpty(stringValue))
                return;
                
            inputField.text = stringValue;

            if (debuggableField.FieldInfo.FieldType == typeof(float))
            {
                float value = GetValue<float>();
                slider.SetValueWithoutNotify(value);
            }
            else
            {
                int value = GetValue<int>();
                slider.SetValueWithoutNotify(value);
            }
        }

        public override bool CanBeUsedForField(FieldInfo targetFieldInfo)
        {
            if (targetFieldInfo.FieldType == typeof(float) || targetFieldInfo.FieldType == typeof(int))
                return targetFieldInfo.HasAttribute<RangeAttribute>();

            return false;
        }
    }
}
