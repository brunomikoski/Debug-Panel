using System;
using System.Reflection;
using UnityEngine;

namespace BrunoMikoski.DebugPanel.GUI
{
    public abstract class DebuggableFieldBaseGUI : DebuggableGUIBase
    {
        protected DebuggableField debuggableField;
        public override Type[] DisplayTypes => new[] { typeof(DebuggableField) };


        public override void Initialize(DebuggableItemBase targetDebuggableItem, DebugPage targetDebugPage)
        {
            base.Initialize(targetDebuggableItem, targetDebugPage);
            debuggableField = (DebuggableField)targetDebuggableItem;
            UpdateDisplayValue();
        }

        protected abstract void UpdateDisplayValue();

        protected override void Update()
        {
            base.Update();
            if (debuggableField.FieldAttribute.UpdateEveryFrame)
                UpdateDisplayValue();
        }

        protected T GetValue<T>()
        {
            TryExecuteBeforeGetValueMethod();
            return (T)debuggableField.FieldInfo.GetValue(debuggableField.Owner);
        }

        private void TryExecuteBeforeGetValueMethod()
        {
            if (string.IsNullOrEmpty(debuggableField.FieldAttribute.OnBeforeGetValueMethodName))
                return;
            
            debuggableField.Owner.GetType().GetMethod(debuggableField.FieldAttribute.OnBeforeGetValueMethodName,
                    BindingFlags.Public
                    | BindingFlags.NonPublic
                    | BindingFlags.Instance
                    | BindingFlags.DeclaredOnly)
                ?.Invoke(debuggableField.Owner, null);
        }

        protected virtual bool SetValue<T>(T targetValue)
        {
            try
            {
                debuggableField.FieldInfo.SetValue(debuggableField.Owner, targetValue);
                TryExecuteOnValueChangedMethod();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        private void TryExecuteOnValueChangedMethod()
        {
            if (string.IsNullOrEmpty(debuggableField.FieldAttribute.OnAfterSetValueMethodName)) 
                return;

            debuggableField.Owner.GetType().GetMethod(debuggableField.FieldAttribute.OnAfterSetValueMethodName,
                    BindingFlags.Public
                    | BindingFlags.NonPublic
                    | BindingFlags.Instance
                    | BindingFlags.DeclaredOnly)
                ?.Invoke(debuggableField.Owner, null);
        }

        public abstract bool CanBeUsedForField(FieldInfo targetFieldInfo);
    }
}