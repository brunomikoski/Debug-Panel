using System;
using UnityEngine;

namespace BrunoMikoski.DebugPanel
{
    internal abstract class DebuggableItemBase
    {
        public bool IsFavorite
        {
            get => PlayerPrefs.GetInt(Path, 0) == 1;
            private set => PlayerPrefs.SetInt(Path, value ? 1 : 0);
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

        public void SetIsFavorite(bool isFavorite)
        {
            IsFavorite = isFavorite;
        }

        public void SetFinalFullPath(string targetFullPath)
        {
            this.fullPath = targetFullPath;
        }
    }
}
