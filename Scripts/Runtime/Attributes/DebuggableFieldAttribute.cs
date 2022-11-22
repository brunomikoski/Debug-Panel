using System;
using UnityEngine.Scripting;

namespace BrunoMikoski.DebugPanel.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DebuggableFieldAttribute : PreserveAttribute
    {
        public string Path  { get; set; }
        
        public string SubTitle  { get; set; }

        public bool UpdateEveryFrame  { get; set; }

        public string OnAfterSetValueMethodName  { get; set; }
        
        public string OnBeforeGetValueMethodName { get; set; }
    }
}
