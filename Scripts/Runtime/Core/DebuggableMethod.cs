using System.Reflection;
using BrunoMikoski.DebugTools;
using UnityEngine;

namespace BrunoMikoski.DebugTools
{
    internal class DebuggableMethod : DebuggableInvokableBase
    {
        private MethodInfo method;
        private object owner;
        private DebuggableClassAttribute classOwner;
        private DebuggableMethodAttribute attribute;
        public DebuggableMethodAttribute Attribute => attribute;


        public DebuggableMethod(string path, MethodInfo method, object owner, DebuggableClassAttribute classOwner, DebuggableMethodAttribute attribute) : base(path)
        {
            this.method = method;
            this.owner = owner;
            this.classOwner = classOwner;
            this.attribute = attribute;
        }

        public DebuggableMethod(string path, string subTitle, MethodInfo method, object owner, DebuggableClassAttribute classOwner, DebuggableMethodAttribute attribute) : base(path, subTitle)
        {
            this.method = method;
            this.owner = owner;
            this.classOwner = classOwner;
            this.attribute = attribute;
        }

        public DebuggableMethod(string path, string subTitle, string spriteName, MethodInfo method, object owner, DebuggableClassAttribute classOwner, DebuggableMethodAttribute attribute) : base(path, subTitle, spriteName)
        {
            this.method = method;
            this.owner = owner;
            this.classOwner = classOwner;
            this.attribute = attribute;
        }

        public override bool Invoke()
        {
            if (method == null || owner == null)
                return false;

            if (!IsMethodEnabled())
            {
                return false;
            }

            method.Invoke(owner, new object[] { });

            if (attribute.CloseDebugPanelAfterExecution || DebugPanel.HideAfterInvoke)
            {
                DebugPanel.Hide();
            }
            return true;
        }

        public bool IsMethodEnabled()
        {
            if (string.IsNullOrEmpty(attribute.ValidateMethodEnabled))
            {
                return true;
            }

            MethodInfo validateMethod = owner.GetType().GetMethod(attribute.ValidateMethodEnabled, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (validateMethod != null)
            {
                return (bool) validateMethod.Invoke(owner, new object[] { });
            }

            Debug.LogError($"Validation method {attribute.ValidateMethodEnabled} for DebuggableMethod {method.Name} not found in {owner.GetType().Name} or not accessible or not returning a bool, ignoring validation method");
            return true;
        }
    }
}