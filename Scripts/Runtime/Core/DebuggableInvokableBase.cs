namespace BrunoMikoski.DebugTools
{
    internal abstract class DebuggableInvokableBase : DebuggableItemBase
    {
        private string hotkey;
        public string Hotkey => hotkey;

        private DebuggableActionHotKeyData hotkeyData;

        public DebuggableActionHotKeyData HotkeyData => hotkeyData;

        public void AssignHotkey(string targetShortcut)
        {
            hotkey = targetShortcut;
            if (!string.IsNullOrEmpty(hotkey))
                hotkeyData = new DebuggableActionHotKeyData(hotkey);
        }

        protected DebuggableInvokableBase(string path) : base(path)
        {
        }

        protected DebuggableInvokableBase(string path, string subTitle) : base(path, subTitle)
        {
        }

        protected DebuggableInvokableBase(string path, string subTitle, string spriteName) : base(path, subTitle)
        {
        }

        public abstract bool Invoke();
    }
}