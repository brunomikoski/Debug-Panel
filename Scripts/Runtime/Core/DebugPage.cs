using System;
using System.Collections.Generic;
using UnityEngine;

namespace BrunoMikoski.DebugTools
{
    internal class DebugPage
    {
        public bool IsFavorite
        {
            get
            {
                if (string.IsNullOrEmpty(PagePath))
                    return false;
                
                return PlayerPrefs.GetInt(PagePath, 0) == 1;
            }
        }

        
        private readonly string pagePath;
        public string PagePath => pagePath;

        private string title;
        public string Title => title;
        
        private string subTitle;
        public string SubTitle => subTitle;
        
        private Sprite icon;
        public Sprite Icon => icon;
        

        private List<DebuggableItemBase> items = new List<DebuggableItemBase>();
        public List<DebuggableItemBase> Items => items;

        private List<DebugPage> childPages = new List<DebugPage>();
        public List<DebugPage> ChildPages => childPages;

        private DebugPage parentPage;
        public DebugPage ParentPage => parentPage;

        private float lastScrollHeight = 1;
        public float LastScrollHeight => lastScrollHeight;

        public bool HasContent => items.Count > 0 || childPages.Count > 0;

        private bool isVisible;
        public bool IsVisible => isVisible;

        private int priority = 0;
        public int Priority => priority;

        public DebugPage(string targetPath, string targetTitle, string targetSubTitle, Sprite targetSprite = null, int targetPriority = 0)
        {
            pagePath = targetPath;
            title = targetTitle;
            subTitle = targetSubTitle;
            icon = targetSprite;
            priority = targetPriority;
        }

        public void AddItem(params DebuggableItemBase[] debuggables)
        {
            for (int i = 0; i < debuggables.Length; i++)
            {
                DebuggableItemBase debuggableItemBase = debuggables[i];

                if (AlreadyContains(debuggableItemBase))
                    continue;

                items.Add(debuggableItemBase);
            }
        }

        private bool AlreadyContains(DebuggableItemBase debuggableItemBase)
        {
            for (int i = 0; i < items.Count; i++)
            {
                DebuggableItemBase itemBase = items[i];
                if (itemBase.FullPath.Equals(debuggableItemBase.FullPath, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        public void AddItem(params DebuggableMethod[] debuggableMethods)
        {
            for (int i = 0; i < debuggableMethods.Length; i++)
            {
                DebuggableMethod debuggableMethod = debuggableMethods[i];

                if (AlreadyContains(debuggableMethod))
                    continue;

                items.Add(debuggableMethod);
            }
        }
        
        public void RemoveItem(params DebuggableItemBase[] debuggables)
        {
            for (int i = 0; i < debuggables.Length; i++)
            {
                items.Remove(debuggables[i]);
            }
        }
        
        public void AddItem(params DebuggableField[] debuggableFields)
        {
            for (int i = 0; i < debuggableFields.Length; i++)
            {
                items.Add(debuggableFields[i]);
            }
        }

        public void AddChildPage(DebugPage childPage, bool setParent = true)
        {
            childPages.Add(childPage);
            if (setParent)
                childPage.SetParentPage(this);
        }

        public void RemoveChildPage(DebugPage targetDebugPage)
        {
            childPages.Remove(targetDebugPage);
        }

        public void UpdateData(string targetSubTitle)
        {
            if (!string.IsNullOrEmpty(targetSubTitle))
                subTitle = targetSubTitle;
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

        public void SetIsFavorite(bool favorite)
        {
            PlayerPrefs.SetInt(PagePath, favorite ? 1 : 0);
        }

        public void SetVisibility(bool visible)
        {
            isVisible = visible;
        }
    }
}
