using System;
using System.Globalization;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace BrunoMikoski.DebugPanel.GUI
{
    internal sealed class DebuggableInputFieldGUI : DebuggableFieldGUIBase
    {
        private static Type[] DisplayableFieldInfoTypes => new[]
        {
            typeof(float), typeof(string), typeof(int), typeof(double), typeof(Vector3), typeof(Vector2),
            typeof(Vector4), typeof(Quaternion)
        };

        [SerializeField]
        private TMP_InputField inputField;

        private string cachedValidValue;
        
        private CultureInfo cultureInfo;


        private void Awake()
        {
            cultureInfo = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            cultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";
        }

        internal override void Initialize(DebuggableItemBase targetDebuggableItem, DebugPage targetDebugPage)
        {
            base.Initialize(targetDebuggableItem, targetDebugPage);
            inputField.onSubmit.AddListener(OnInputFieldSubmit);
        }

        private void OnDestroy()
        {
            inputField.onSubmit.RemoveListener(OnInputFieldSubmit);
        }

        private void OnInputFieldSubmit(string newValue)
        {
            ProcessNewValue(newValue);
        }


        protected override void UpdateDisplayValue()
        {
            string stringValue = GetValue<object>().ToString();
            if (!string.IsNullOrEmpty(stringValue))
            {
                inputField.text = stringValue;
                cachedValidValue = stringValue;
            }
        }

        protected override void Update()
        {
            if (debuggableField.FieldAttribute.UpdateEveryFrame && !inputField.isFocused)
                UpdateDisplayValue();
        }

        private void ProcessNewValue(string newValue)
        {
            if (!TrySetValueForField(newValue))
                TrySetValueForField(cachedValidValue);
        }

        private bool TrySetValueForField(string newValue)
        {
            FieldInfo fieldInfo = debuggableField.FieldInfo;
            
            if (fieldInfo.FieldType == typeof(float))
            {
                return SetValue(float.Parse(newValue));
            }

            if (fieldInfo.FieldType == typeof(string))
            {
                return SetValue(newValue);
            }

            if (fieldInfo.FieldType == typeof(int))
            {
                return SetValue(int.Parse(newValue));
            }

            if (fieldInfo.FieldType == typeof(double))
            {
                return SetValue(double.Parse(newValue));
            }

            if (fieldInfo.FieldType == typeof(Vector3))
            {
                string[] axisValue = newValue.Replace("(", "").Replace(")", "")
                    .Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
                
                if (axisValue.Length != 3)
                {
                    Debug.LogError("Invalid Vector3 value, requires 3 comma separated values");
                    return false;
                }

                try
                {
                    Vector3 finalValue = new Vector3
                    {
                        x = float.Parse(axisValue[0], NumberStyles.Any, cultureInfo), 
                        y = float.Parse(axisValue[1], NumberStyles.Any, cultureInfo), 
                        z = float.Parse(axisValue[2], NumberStyles.Any, cultureInfo)
                    };
                    return SetValue(finalValue);

                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    return false;
                }
            }

            if (fieldInfo.FieldType == typeof(Vector2))
            {
                string[] axisValue = newValue.Replace("(", "").Replace(")", "")
                    .Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
                
                if (axisValue.Length != 2)
                {
                    Debug.LogError("Invalid Vector2 value, requires 2 comma separated values");
                    return false;
                }

                try
                {
                    Vector2 finalValue = new Vector2
                    {
                        x = float.Parse(axisValue[0], NumberStyles.Any, cultureInfo), 
                        y = float.Parse(axisValue[1], NumberStyles.Any, cultureInfo), 
                    };
                
                    return SetValue(finalValue);

                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    return false;
                }
            }

            if (fieldInfo.FieldType == typeof(Vector4))
            {
                string[] axisValue = newValue.Replace("(", "").Replace(")", "")
                    .Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
                
                if (axisValue.Length != 4)
                {
                    Debug.LogError("Invalid Vector4 value, requires 4 comma separated values");
                    return false;
                }

                try
                {
                    Vector4 finalValue = new Vector4
                    {
                        x = float.Parse(axisValue[0], NumberStyles.Any, cultureInfo), 
                        y = float.Parse(axisValue[1], NumberStyles.Any, cultureInfo), 
                        z = float.Parse(axisValue[2], NumberStyles.Any, cultureInfo), 
                        w = float.Parse(axisValue[3], NumberStyles.Any, cultureInfo), 
                    };
                
                    return SetValue(finalValue);

                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    return false;
                }
            }

            if (fieldInfo.FieldType == typeof(Quaternion))
            {
                string[] axisValue = newValue.Replace("(", "").Replace(")", "")
                    .Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
                
                if (axisValue.Length != 4)
                {
                    Debug.LogError("Invalid Quaternion value, requires 4 comma separated values");
                    return false;
                }

                try
                {
                    Quaternion finalValue = new Quaternion()
                    {
                        x = float.Parse(axisValue[0], NumberStyles.Any, cultureInfo), 
                        y = float.Parse(axisValue[1], NumberStyles.Any, cultureInfo), 
                        z = float.Parse(axisValue[2], NumberStyles.Any, cultureInfo), 
                        w = float.Parse(axisValue[3], NumberStyles.Any, cultureInfo), 
                    };
                    return SetValue(finalValue);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    return false;
                }

            }

            Debug.LogError($"Unsupported type {fieldInfo.FieldType}");
            return false;
        }

        public override bool CanBeUsedForField(FieldInfo targetFieldInfo)
        {
            if (targetFieldInfo.FieldType == typeof(string))
            {
                if (targetFieldInfo.HasAttribute<MultilineAttribute>())
                    return false;
            }
            
            if (targetFieldInfo.FieldType == typeof(float) || targetFieldInfo.FieldType == typeof(int))
                if (targetFieldInfo.HasAttribute<RangeAttribute>())
                    return false;
            
            if (Array.IndexOf(DisplayableFieldInfoTypes, targetFieldInfo.FieldType) > -1)
                return true;

            return false;
        }

        protected override bool SetValue<T>(T targetValue)
        {
            bool result = base.SetValue(targetValue);
            if (result)
                UpdateDisplayValue();
            
            return result;
        }
    }
}
