namespace BrunoMikoski.DebugPanel
{
    public class PageLink : DebuggableItemBase
    {
        private readonly DebugPage fromDebugPage;
        private readonly DebugPage toDebugPage;
        public DebugPage ToDebugPage => toDebugPage;
        
        public PageLink(string path, DebugPage fromDebugPage, DebugPage toDebugPage) : base(path)
        {
            this.fromDebugPage = fromDebugPage;
            this.toDebugPage = toDebugPage;
        }

        public PageLink(string path, string subTitle, DebugPage fromDebugPage, DebugPage toDebugPage) : base(path, subTitle)
        {
            this.fromDebugPage = fromDebugPage;
            this.toDebugPage = toDebugPage;
        }

        public PageLink(string path, string subTitle, string spriteName, DebugPage fromDebugPage, DebugPage toDebugPage) : base(path, subTitle, spriteName)
        {
            this.fromDebugPage = fromDebugPage;
            this.toDebugPage = toDebugPage;
        }

        // public PageLink(string pageName)
        // {
        //     this.title = pageName;
        // }
        //
        // public PageLink(string targetTitle, string targetSubTitle, string targetSpriteName, DebugPage originPage,
        //     DebugPage targetPage)
        // {
        //     title = targetTitle;
        //     subTitle = targetSubTitle;
        //     spriteName = targetSpriteName;
        //     fromDebugPage = originPage;
        //     toDebugPage = targetPage;
        // }
    }
}