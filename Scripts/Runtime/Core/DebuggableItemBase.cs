using System;
using UnityEngine;

namespace BrunoMikoski.DebugTools
{
    internal abstract class DebuggableItemBase
    {
        public virtual bool IsFavorite
        {
            get
            {
                if (string.IsNullOrEmpty(fullPath))
                    return false;
                
                return PlayerPrefs.GetInt(fullPath, 0) == 1;
            }
        }

        private string path;
        public string Path => path;

        private string title;
        public string Title => title;

        private string subTitle;
        public string SubTitle => subTitle;

        private string fullPath;
        public string FullPath => fullPath;


        protected DebuggableItemBase(string path)
        {
            this.path = path;

            string[] split = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            title = split[split.Length - 1];
        }

        protected DebuggableItemBase(string path, string subTitle) : this(path)
        {
            this.subTitle = subTitle;
        }

        internal virtual void SetIsFavorite(bool isFavorite)
        {
            PlayerPrefs.SetInt(fullPath, isFavorite ? 1 : 0);
        }

        internal void SetFinalFullPath(string targetFullPath)
        {
            this.fullPath = targetFullPath;
        }
    }
}
