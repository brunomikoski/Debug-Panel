using System.Collections.Generic;

namespace BrunoMikoski.DebugPanel
{
    public class DebugPage
    {
        private readonly string pagePath;
        public string PagePath => pagePath;

        private string title;
        public string Title => title;
        
        private string subTitle;
        public string SubTitle => subTitle;

        private string spriteName;
        public string SpriteName => spriteName;


        private List<DebuggableItemBase> items = new List<DebuggableItemBase>();
        public List<DebuggableItemBase> Items => items;

        private List<DebugPage> childPages = new List<DebugPage>();
        public List<DebugPage> ChildPages => childPages;

        private DebugPage parentPage;
        public DebugPage ParentPage => parentPage;

        private float? lastScrollHeight;
        public float? LastScrollHeight => lastScrollHeight;

        public DebugPage(string targetPath, string targetTitle, string targetSubTitle, string targetSpriteName)
        {
            pagePath = targetPath;
            title = targetTitle;
            subTitle = targetSubTitle;
            spriteName = targetSpriteName;
        }

        public void AddItem(params DebuggableItemBase[] debuggables)
        {
            items.AddRange(debuggables);

        }
        public void AddItem(params DebuggableMethod[] debuggableMethods)
        {
            items.AddRange(debuggableMethods);
        }
        
        public void AddItem(params DebuggableField[] debuggableFields)
        {
            items.AddRange(debuggableFields);
        }

        public void AddChildPage(DebugPage childPage)
        {
            childPages.Add(childPage);
            childPage.SetParentPage(this);
        }

        public void UpdateData(string targetSubTitle, string targetSpriteName)
        {
            if (!string.IsNullOrEmpty(targetSubTitle))
                subTitle = targetSubTitle;

            if (!string.IsNullOrEmpty(targetSpriteName))
                spriteName = targetSpriteName;
        }

        public void SetParentPage(DebugPage parentPage)
        {
            this.parentPage = parentPage;
        }

        public bool HasParentPage()
        {
            return parentPage != null;
        }

        public void SetTitle(string targetTitle)
        {
            title = targetTitle;
        }

        public void SetLastKnowHeight(float normPosition)
        {
            lastScrollHeight = normPosition;
        }
    }
}
