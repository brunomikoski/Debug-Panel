using System;
using System.Globalization;
using System.Reflection;
using BrunoMikoski.DebugTools.Core.Attributes;
using TMPro;
using UnityEngine;

namespace BrunoMikoski.DebugTools.Layout
{
    public sealed class DebuggableFieldGUI : DebuggableFieldGUIBase
    {
        [SerializeField]
        private TMP_InputField valueInput;

        public new void Initialize(object targetObject, FieldInfo fieldInfo,
            DebuggableFieldAttribute debuggableFieldAttribute)
        {
            base.Initialize(targetObject, fieldInfo, debuggableFieldAttribute);
            
            valueInput.text = fieldInfo.GetValue(targetObject).ToString();

            if (debuggableFieldAttribute.ReadOnly)
            {
                valueInput.interactable = false;
            }
            
            valueInput.onSubmit.AddListener(OnSubmitValue);
        }
        
        private void OnDestroy()
        {
            valueInput.onSubmit.RemoveListener(OnSubmitValue);
        }

        private void OnSubmitValue(string newValue)
        {
            CultureInfo cultureInfo = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            cultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";
            
            if (fieldInfo.FieldType == typeof(float))
            {
                fieldInfo.SetValue(targetObject, float.Parse(newValue));
            }
            else if (fieldInfo.FieldType == typeof(string))
            {
                fieldInfo.SetValue(targetObject, newValue);
            }
            else if (fieldInfo.FieldType == typeof(int))
            {
                fieldInfo.SetValue(targetObject, int.Parse(newValue));
            }
            else if (fieldInfo.FieldType == typeof(double))
            {
                fieldInfo.SetValue(targetObject, double.Parse(newValue));
            }
            else if (fieldInfo.FieldType == typeof(Vector3))
            {
                string[] axisValue = newValue.Replace("(", "").Replace(")", "")
                    .Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
                
                if (axisValue.Length != 3)
                {
                    Debug.LogError("Invalid Vector3 value, requires 3 comma separated values");
                    return;
                }

                Vector3 finalValue = new Vector3
                {
                    x = float.Parse(axisValue[0], NumberStyles.Any, cultureInfo), 
                    y = float.Parse(axisValue[1], NumberStyles.Any, cultureInfo), 
                    z = float.Parse(axisValue[2], NumberStyles.Any, cultureInfo)
                };
                
                fieldInfo.SetValue(targetObject, finalValue);
            }
            else if (fieldInfo.FieldType == typeof(Vector2))
            {
                string[] axisValue = newValue.Replace("(", "").Replace(")", "")
                    .Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
                
                if (axisValue.Length != 2)
                {
                    Debug.LogError("Invalid Vector2 value, requires 2 comma separated values");
                    return;
                }

                Vector2 finalValue = new Vector2
                {
                    x = float.Parse(axisValue[0], NumberStyles.Any, cultureInfo), 
                    y = float.Parse(axisValue[1], NumberStyles.Any, cultureInfo), 
                };
                
                fieldInfo.SetValue(targetObject, finalValue);
            }
            else if (fieldInfo.FieldType == typeof(Vector4))
            {
                string[] axisValue = newValue.Replace("(", "").Replace(")", "")
                    .Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
                
                if (axisValue.Length != 4)
                {
                    Debug.LogError("Invalid Vector4 value, requires 4 comma separated values");
                    return;
                }

                Vector4 finalValue = new Vector4
                {
                    x = float.Parse(axisValue[0], NumberStyles.Any, cultureInfo), 
                    y = float.Parse(axisValue[1], NumberStyles.Any, cultureInfo), 
                    z = float.Parse(axisValue[2], NumberStyles.Any, cultureInfo), 
                    w = float.Parse(axisValue[3], NumberStyles.Any, cultureInfo), 
                };
                
                fieldInfo.SetValue(targetObject, finalValue);
            }
            else if (fieldInfo.FieldType == typeof(Quaternion))
            {
                string[] axisValue = newValue.Replace("(", "").Replace(")", "")
                    .Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
                
                if (axisValue.Length != 4)
                {
                    Debug.LogError("Invalid Quaternion value, requires 4 comma separated values");
                    return;
                }

                Quaternion finalValue = new Quaternion()
                {
                    x = float.Parse(axisValue[0], NumberStyles.Any, cultureInfo), 
                    y = float.Parse(axisValue[1], NumberStyles.Any, cultureInfo), 
                    z = float.Parse(axisValue[2], NumberStyles.Any, cultureInfo), 
                    w = float.Parse(axisValue[3], NumberStyles.Any, cultureInfo), 
                };
                
                fieldInfo.SetValue(targetObject, finalValue);
            }
            else
            {
                Debug.LogError($"Unsupported type {fieldInfo.FieldType}");
            }
        }

        protected override void Update()
        {
            if (!debuggableFieldAttribute.UpdateEveryFrame)
                return;
            
            valueInput.text = fieldInfo.GetValue(targetObject).ToString();
        }
    }
}
