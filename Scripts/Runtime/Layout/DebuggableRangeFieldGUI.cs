using System;
using System.Globalization;
using System.Reflection;
using BrunoMikoski.DebugTools.Core.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BrunoMikoski.DebugTools.Layout
{
    public sealed class DebuggableRangeFieldGUI : DebuggableItemBaseGUI
    {
        [SerializeField]
        private TMP_Text labelText;

        [SerializeField]
        private Slider slider;
        [SerializeField]
        private TMP_Text valueText;
        
        private object targetObject;
        private FieldInfo fieldInfo;
        private DebuggableFieldAttribute debuggableFieldAttribute;

        private void Awake()
        {
            slider.onValueChanged.AddListener(OnSliderValueChanged);
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

        public void Initialize(object targetObject, FieldInfo targetFieldInfo, DebuggableFieldAttribute targetFieldAttribute, RangeAttribute rangeAttribute)
        {
            this.targetObject = targetObject;
            fieldInfo = targetFieldInfo;
            debuggableFieldAttribute = targetFieldAttribute;
            labelText.text = fieldInfo.Name;

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
        }
    }
}
