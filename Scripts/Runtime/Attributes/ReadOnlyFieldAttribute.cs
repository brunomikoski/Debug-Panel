using System;
using UnityEngine.Scripting;

namespace BrunoMikoski.DebugPanel.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ReadOnlyFieldAttribute : PreserveAttribute
    {
        
    }
}