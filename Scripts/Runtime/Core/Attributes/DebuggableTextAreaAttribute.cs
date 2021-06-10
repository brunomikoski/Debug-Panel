using System;
using UnityEngine.Scripting;

namespace BrunoMikoski.DebugTools.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class DebuggableTextAreaAttribute : PreserveAttribute
    {
        private readonly string title;
        public string Title => title;

        public DebuggableTextAreaAttribute()
        {
        }

        public DebuggableTextAreaAttribute(string targetTitle)
        {
            title = targetTitle;
        }
    }
}