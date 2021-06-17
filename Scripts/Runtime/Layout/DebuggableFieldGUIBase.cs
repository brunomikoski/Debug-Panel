using System.Reflection;
using BrunoMikoski.DebugTools.Core.Attributes;
using TMPro;
using UnityEngine;

namespace BrunoMikoski.DebugTools.Layout
{
    public abstract class DebuggableFieldGUIBase : DebuggableItemGUIBase
    {
        [SerializeField]
        private TMP_Text labelText;
        
        protected DebuggableFieldAttribute debuggableFieldAttribute;

        protected void Initialize(object targetObject, FieldInfo targetFieldInfo, DebuggableFieldAttribute targetFieldAttribute)
        {
            base.Initialize(targetObject, targetFieldInfo);

            this.targetObject = targetObject;
            fieldInfo = targetFieldInfo;
            debuggableFieldAttribute = targetFieldAttribute;
            
            labelText.text = fieldInfo.Name;
        }
    }
}