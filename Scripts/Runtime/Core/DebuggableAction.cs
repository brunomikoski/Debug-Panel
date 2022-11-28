using System;

namespace BrunoMikoski.DebugTools
{
    internal class DebuggableAction : DebuggableInvokableBase
    {
        private readonly Action action;

        public DebuggableAction(string path, Action action) : base(path)
        {
            this.action = action;
        }

        public DebuggableAction(string path, string subTitle, Action action) : base(path, subTitle)
        {
            this.action = action;
        }

        public DebuggableAction(string path, string subTitle, string spriteName, Action action) : base(path, subTitle, spriteName)
        {
            this.action = action;
        }

        public override bool Invoke()
        {
            if (action == null)
                return false;
            action.Invoke();
            return true;
        }
    }
}
