using System;
using UnityEngine.Scripting;

namespace BrunoMikoski.DebugTools.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class DebuggableFieldAttribute : PreserveAttribute
    {
        public bool ReadOnly => readOnly;
        private readonly bool readOnly;
        
        private readonly bool updateEveryFrame;
        public bool UpdateEveryFrame => updateEveryFrame;

        public DebuggableFieldAttribute()
        {
        }
        
        public DebuggableFieldAttribute(bool readOnly)
        {
            this.readOnly = readOnly;
        }
        
        public DebuggableFieldAttribute(bool readOnly, bool updateEveryFrame)
        {
            this.readOnly = readOnly;
            this.updateEveryFrame = updateEveryFrame;
        }
    }
}