using System;

namespace BrunoMikoski.DebugTools.GUI
{
    internal class PageLinkGUI : DebuggableGUIBase
    {
        public override Type[] DisplayTypes => new[] { typeof(DebuggablePageLink) };
        
        private DebuggablePageLink pageLink;

        
        internal override void Initialize(DebuggableItemBase targetDebuggableItem, DebugPage targetDebugPage)
        {
            pageLink = (DebuggablePageLink)targetDebuggableItem;
            UpdateIconVisibility();

            base.Initialize(targetDebuggableItem, targetDebugPage);
            SetIcon(pageLink.ToDebugPage.Icon);
            
            gameObject.SetActive(pageLink.ToDebugPage.HasContent);
        }

        protected override void OnClick()
        {
            base.OnClick();
            DebugPanel.DisplayPage(pageLink.ToDebugPage);
        }

        protected override void UpdateIconVisibility()
        {
            if (iconImage == null)
                return;
            
            if (iconImage.sprite == null)
            {
                iconImage.gameObject.SetActive(false);
                return;
            }

            if (iconImage.sprite == pageLink.ToDebugPage.Icon)
            {
                iconImage.gameObject.SetActive(true);
            }
            else
            {
                iconImage.gameObject.SetActive(pageLink.ToDebugPage.IsFavorite);
            }
        }

        protected override void ToggleFavorite()
        {
            if (iconImage == null)
                return;

            pageLink.ToDebugPage.SetIsFavorite(!pageLink.ToDebugPage.IsFavorite);
            DebugPanel.UpdateDebuggableFavorite(pageLink);
            UpdateIconVisibility();
        }
    }
}