using System.Reflection;
using BrunoMikoski.DebugTools.Core.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace BrunoMikoski.DebugTools.Layout
{
    public sealed class DebuggableToggleFieldGUI : DebuggableFieldGUIBase
    {
        [SerializeField]
        private Toggle toggle;

        public new void Initialize(object targetObject, FieldInfo fieldInfo,
            DebuggableFieldAttribute debuggableFieldAttribute)
        {
            base.Initialize(targetObject, fieldInfo, debuggableFieldAttribute);
            
            toggle.isOn = (bool)fieldInfo.GetValue(targetObject);

            if (debuggableFieldAttribute.ReadOnly)
                toggle.interactable = false;
            
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }
        
        private void OnDestroy()
        {
            toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
        }
        
        private void OnToggleValueChanged(bool toggleValue)
        {
            fieldInfo.SetValue(targetObject, toggleValue);
        }
    }
}