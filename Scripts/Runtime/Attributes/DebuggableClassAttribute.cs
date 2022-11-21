using System;
using UnityEngine.Scripting;

namespace BrunoMikoski.DebugPanel.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DebuggableClassAttribute : PreserveAttribute
    {
        public string Path  { get; set; }
        
        public string SubTitle  { get; set; }
    }
}
