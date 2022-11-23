using System;

namespace BrunoMikoski.DebugPanel.GUI
{
    internal class PageLinkGUI : DebuggableGUIBase
    {
        public override Type[] DisplayTypes => new[] { typeof(PageLink) };
        
        private PageLink pageLink;

        
        internal override void Initialize(DebuggableItemBase targetDebuggableItem, DebugPage targetDebugPage)
        {
            base.Initialize(targetDebuggableItem, targetDebugPage);
            pageLink = (PageLink)targetDebuggableItem;
        }

        protected override void OnClick()
        {
            base.OnClick();
            DebugPanel.DisplayPage(pageLink.ToDebugPage);
        }
    }
}