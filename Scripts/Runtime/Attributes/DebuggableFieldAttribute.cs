using System;
using UnityEngine.Scripting;

namespace BrunoMikoski.DebugPanel.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DebuggableFieldAttribute : PreserveAttribute
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
        /// If the value of this field should be updated on every frame.
        /// </summary>
        public bool UpdateEveryFrame  { get; set; }

        /// <summary>
        /// Method Name to be called after the value of this field change
        /// </summary>
        public string OnAfterSetValueMethodName  { get; set; }

        /// <summary>
        /// Method Name to be called before the value of this field change
        /// </summary>
        public string OnBeforeGetValueMethodName { get; set; }
    }
}
