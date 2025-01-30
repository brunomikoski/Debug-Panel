using System;
using JetBrains.Annotations;
using UnityEngine.Scripting;

namespace BrunoMikoski.DebugTools
{
    [AttributeUsage(AttributeTargets.Method)]
    [MeansImplicitUse(ImplicitUseKindFlags.Access)]
    public class DebuggableMethodAttribute : PreserveAttribute
    {
        /// <summary>
        /// Used to define the Path to this object, you can create a full path using /
        /// <example>Path = "Level Design/Player Tweaks/Hero"</example>
        /// </summary>
        public string Path  { get; set; }
        /// <summary>
        /// Use this to display useful information about this class, this is also used for searching so you could add tags here to simplify search
        /// </summary>
        public string SubTitle  { get; set; }

        /// <summary>
        /// Hotkey used to trigger this method after is loaded into the DebugPanel
        /// <example>Hotkey = "%#a" (Cmd/Ctrl+Shift+a)</example>
        /// <see>
        ///     <cref>https://docs.unity3d.com/ScriptReference/MenuItem.html</cref>
        /// </see>
        /// </summary>
        public string Hotkey  { get; set; }
        
        public bool CloseDebugPanelAfterExecution { get; set; }

        /// <summary>
        /// Validation method to be checked, if this method is enabled or not
        /// </summary>
        public string ValidateMethodEnabled { get; set; }
    }
}