using System;
using System.Reflection;
using BrunoMikoski.DebugTools;
using UnityEngine;

namespace BrunoMikoski.DebugTools.GUI
{
    internal abstract class DebuggableFieldGUIBase : DebuggableGUIBase
    {
        protected DebuggableField debuggableField;
        public override Type[] DisplayTypes => new[] { typeof(DebuggableField) };


        internal override void Initialize(DebuggableItemBase targetDebuggableItem, DebugPage targetDebugPage)
        {
            base.Initialize(targetDebuggableItem, targetDebugPage);
            debuggableField = (DebuggableField)targetDebuggableItem;
            
            if (debuggableField.FieldAttribute.ReadOnly)
                SetAsReadOnly();

            UpdateDisplayValue();
        }

        protected virtual void SetAsReadOnly(){}

        protected abstract void UpdateDisplayValue();

        protected override void Update()
        {
            base.Update();
            if (debuggableField.FieldAttribute.UpdateEveryFrame)
                UpdateDisplayValue();
        }

        protected T GetValue<T>()
        {
            TryInvokeOwnerMethod(debuggableField.FieldAttribute.OnBeforeGetValueMethodName);
            object value = debuggableField.FieldInfo.GetValue(debuggableField.Owner);
            return (T)value;
        }

        private void TryInvokeOwnerMethod(string methodName)
        {
            if (string.IsNullOrEmpty(methodName))
                return;

            debuggableField.Owner.GetType().GetMethod(methodName,
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
                TryInvokeOwnerMethod(debuggableField.FieldAttribute.OnAfterSetValueMethodName);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public abstract bool CanBeUsedForField(FieldInfo targetFieldInfo);
    }
}