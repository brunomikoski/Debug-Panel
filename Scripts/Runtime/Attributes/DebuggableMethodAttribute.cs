using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Scripting;

namespace BrunoMikoski.DebugPanel.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    [MeansImplicitUse(ImplicitUseKindFlags.Access)]
    public class DebuggableMethodAttribute : PreserveAttribute
    {
        public string Path  { get; set; }
        public string SubTitle  { get; set; }
        public string Hotkey  { get; set; }
        public RuntimePlatform[] Platforms { get; set; }
    }
}
