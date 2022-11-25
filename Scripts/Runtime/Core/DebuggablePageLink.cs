namespace BrunoMikoski.DebugPanel
{
    internal class DebuggablePageLink : DebuggableItemBase
    {
        public override bool IsFavorite => ToDebugPage.IsFavorite;

        private readonly DebugPage toDebugPage;
        public DebugPage ToDebugPage => toDebugPage;

        public DebuggablePageLink(string path, string subTitle, DebugPage toDebugPage) : base(path, subTitle)
        {
            this.toDebugPage = toDebugPage;
        }

        internal override void SetIsFavorite(bool isFavorite)
        {
            ToDebugPage.SetIsFavorite(isFavorite);
        }
    }
}