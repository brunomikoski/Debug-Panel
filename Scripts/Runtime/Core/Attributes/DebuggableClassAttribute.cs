using System;
using UnityEngine.Scripting;

namespace BrunoMikoski.DebugTools.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DebuggableClassAttribute : PreserveAttribute
    {
        private readonly string categoryName;
        public string CategoryName => categoryName;
        
        public DebuggableClassAttribute(string categoryName = DebugPanel.DEFAULT_CATEGORY_NAME)
        {
            this.categoryName = categoryName;
        }
    }
}