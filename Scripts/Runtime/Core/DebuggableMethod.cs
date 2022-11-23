using System.Reflection;
using BrunoMikoski.DebugPanel.Attributes;

namespace BrunoMikoski.DebugPanel
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
            
            method.Invoke(owner, new object[] { });
            return true;
        }
    }
}
