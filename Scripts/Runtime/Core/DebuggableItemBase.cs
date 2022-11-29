using System;
using System.Linq;
using UnityEngine;

namespace BrunoMikoski.DebugTools
{
    internal abstract class DebuggableItemBase
    {
        public virtual bool IsFavorite
        {
            get
            {
                if (string.IsNullOrEmpty(FullPath))
                    return false;
                
                return PlayerPrefs.GetInt(FullPath, 0) == 1;
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

        private string parentPath;
        public string ParentPath => parentPath;


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
            PlayerPrefs.SetInt(FullPath, isFavorite ? 1 : 0);
        }

        internal void SetFinalFullPath(string targetFullPath)
        {
            fullPath = targetFullPath;

            parentPath = System.IO.Path.GetDirectoryName(targetFullPath)?.Replace("\\", "/");
        }
    }
}
