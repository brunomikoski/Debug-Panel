using System;

namespace BrunoMikoski.DebugPanel.GUI
{
    internal class PageLinkGUI : DebuggableGUIBase
    {
        public override Type[] DisplayTypes => new[] { typeof(DebuggablePageLink) };
        
        private DebuggablePageLink pageLink;

        
        internal override void Initialize(DebuggableItemBase targetDebuggableItem, DebugPage targetDebugPage)
        {
            pageLink = (DebuggablePageLink)targetDebuggableItem;
            base.Initialize(targetDebuggableItem, targetDebugPage);
        }

        protected override void OnClick()
        {
            base.OnClick();
            DebugPanelService.DisplayPage(pageLink.ToDebugPage);
        }

        protected override void UpdateFavorite()
        {
            base.UpdateFavorite();
            if (favIconImage == null)
                return;
                
            favIconImage.gameObject.SetActive(pageLink.ToDebugPage.IsFavorite);
        }

        protected override void ToggleFavorite()
        {
            if (favIconImage == null)
                return;

            pageLink.ToDebugPage.SetIsFavorite(!pageLink.ToDebugPage.IsFavorite);
            DebugPanelService.UpdateDebuggableFavorite(pageLink);
            UpdateFavorite();
        }
    }
}