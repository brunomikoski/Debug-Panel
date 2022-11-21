using System;
using UnityEngine;
using UnityEngine.UI;

namespace BrunoMikoski.DebugPanel.GUI
{
    public class PageLinkGUI : DebuggableGUIBase
    {
        public override Type[] DisplayTypes => new[] { typeof(PageLink) };
        
        [SerializeField]
        private Button button;

        private PageLink pageLink;


        private void Awake()
        {
            button.onClick.AddListener(OnButtonClick);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(OnButtonClick);
        }
        
        public override void Initialize(DebuggableItemBase targetDebuggableItem, DebugPage targetDebugPage)
        {
            base.Initialize(targetDebuggableItem, targetDebugPage);
            pageLink = (PageLink)targetDebuggableItem;
        }

        private void OnButtonClick()
        {
            DebugPanel.DisplayPage(pageLink.ToDebugPage);
        }
    }
}