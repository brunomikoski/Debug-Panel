using System;
using System.Reflection;
using BrunoMikoski.DebugTools.Core.Attributes;
using TMPro;
using UnityEngine;

namespace BrunoMikoski.DebugTools.Layout
{
    public class DebuggableTextAreaGUI : DebuggableItemGUIBase
    {
        [SerializeField]
        private TMP_Text titleText;
        
        [SerializeField]
        private TMP_Text valueText;


        private string currentText;
        private float previousHeight;

        public void Initialize(object targetObject, FieldInfo fieldInfo, DebuggableTextAreaAttribute debuggableFieldAttribute)
        {
            this.targetObject = targetObject;
            this.fieldInfo = fieldInfo;
            string targetTitle = debuggableFieldAttribute.Title;
            if (string.IsNullOrEmpty(targetTitle))
                targetTitle = fieldInfo.Name;

            titleText.text = targetTitle;
        }

        private void OnEnable()
        {
            MarkAsDirty();
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }


        protected override void Update()
        {
            base.Update();
            TryUpdateText();
        }

        private void TryUpdateText()
        {
            string fieldValue = fieldInfo.GetValue(targetObject).ToString();
            if (string.Equals(currentText, fieldValue, StringComparison.Ordinal))
                return;

            valueText.SetText(fieldValue);
            currentText = fieldValue;

            if (Math.Abs(previousHeight - valueText.rectTransform.sizeDelta.y) < float.Epsilon)
                return;

            previousHeight = valueText.rectTransform.sizeDelta.y;

            MarkAsDirty();
        }
    }
}
