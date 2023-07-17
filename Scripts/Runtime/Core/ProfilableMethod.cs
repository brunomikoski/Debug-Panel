using System.Reflection;

namespace BrunoMikoski.DebugTools
{
    internal class ProfilableMethod : DebuggableInvokableBase
    {
        internal MethodInfo Method;
        internal object Owner;
        internal DebuggableClassAttribute ClassOwner;
        internal ProfilableMethodAttribute Attribute;
        internal int ExecutionCount = 1000;

        public ProfilableMethod(string path, string subTitle, MethodInfo method, object owner,
            DebuggableClassAttribute classOwner, ProfilableMethodAttribute attribute, int executionCount) : base(path,
            subTitle)
        {
            Method = method;
            Owner = owner;
            ClassOwner = classOwner;
            Attribute = attribute;
            ExecutionCount = executionCount;
        }

        public override bool Invoke()
        {
            if (Method == null || Owner == null)
                return false;

            Method.Invoke(Owner, new object[] { });
            return true;
        }
    }
}