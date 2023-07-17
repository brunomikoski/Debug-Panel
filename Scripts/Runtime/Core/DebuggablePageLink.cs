namespace BrunoMikoski.DebugTools
{
    internal class DebuggablePageLink : DebuggableItemBase
    {
        public override bool IsFavorite => ToDebugPage.IsFavorite;

        private readonly DebugPage toDebugPage;
        public DebugPage ToDebugPage => toDebugPage;

        public DebuggablePageLink(string path, string subTitle, DebugPage toDebugPage, int priority) : base(path, subTitle, priority)
        {
            this.toDebugPage = toDebugPage;
        }

        internal override void SetIsFavorite(bool isFavorite)
        {
            ToDebugPage.SetIsFavorite(isFavorite);
        }
    }
}