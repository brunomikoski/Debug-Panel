namespace BrunoMikoski.DebugTools
{
    public readonly struct DebugCommandInfo
    {
        public readonly string FullPath;
        public readonly string Title;
        public readonly string SubTitle;

        public DebugCommandInfo(string fullPath, string title, string subTitle)
        {
            FullPath = fullPath;
            Title = title;
            SubTitle = subTitle;
        }
    }
}
