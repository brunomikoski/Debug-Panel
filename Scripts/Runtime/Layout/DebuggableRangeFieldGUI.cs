using System;
using System.Globalization;
using System.Reflection;
using BrunoMikoski.DebugTools.Core.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BrunoMikoski.DebugTools.Layout
{
    public sealed class DebuggableRangeFieldGUI : DebuggableFieldGUIBase
    {
        [SerializeField]
        private Slider slider;
        [SerializeField]
        private TMP_Text valueText;

        public void Initialize(object targetObject, FieldInfo targetFieldInfo, DebuggableFieldAttribute targetFieldAttribute, RangeAttribute rangeAttribute)
        {
            base.Initialize(targetObject, targetFieldInfo, targetFieldAttribute);

            slider.minValue = rangeAttribute.min;
            slider.maxValue = rangeAttribute.max;
            if (targetFieldInfo.FieldType == typeof(float))
            {
                slider.value = (float)fieldInfo.GetValue(targetObject);
            }
            else
            {
                slider.value = (int)fieldInfo.GetValue(targetObject);
            }

            if (debuggableFieldAttribute.ReadOnly)
            {
                slider.interactable = false;
            }
            
            slider.onValueChanged.AddListener(OnSliderValueChanged);
            valueText.text = slider.value.ToString();

        }
        
        private void OnDestroy()
        {
            slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
        
        private void OnSliderValueChanged(float targetValue)
        {
            CultureInfo cultureInfo = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            cultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";
            
            if (fieldInfo.FieldType == typeof(float))
            {
                fieldInfo.SetValue(targetObject, slider.value);
                valueText.text = $"{slider.value:#0.0}";
            }
            else if (fieldInfo.FieldType == typeof(int))
            {
                int intValue = Convert.ToInt32(slider.value);
                fieldInfo.SetValue(targetObject, intValue);
                valueText.text = intValue.ToString();
            }
        }
    }
}
