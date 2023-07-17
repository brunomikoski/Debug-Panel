using System;
using JetBrains.Annotations;
using UnityEngine.Scripting;

namespace BrunoMikoski.DebugTools
{
    [AttributeUsage(AttributeTargets.Method)]
    [MeansImplicitUse(ImplicitUseKindFlags.Access)]
    public class ProfilableMethodAttribute : PreserveAttribute
    {
        public bool KeepHistory = true;

        public string SubTitle = String.Empty;
        public string Hotkey  = String.Empty;
        public string Path  = String.Empty;
        public int ExecutionCount = 1000;

    }
}