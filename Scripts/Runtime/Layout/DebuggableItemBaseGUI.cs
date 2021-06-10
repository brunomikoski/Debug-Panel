using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BrunoMikoski.DebugTools.Layout
{
    public abstract class DebuggableItemBaseGUI : MonoBehaviour
    {
        [Header("Base Layout Groups")]
        [SerializeField]
        private LayoutGroup groupLayoutGroup;
        [SerializeField]
        private LayoutGroup contentLayoutGroup;
        
        private bool isDirty;
        private bool isUpdating;

        protected void MarkAsDirty()
        {
            isDirty = true;
        }

        protected virtual void Update()
        {
            UpdateIfDirty();
        }

        private void UpdateIfDirty()
        {
            if (!isDirty)
                return;

            if (isUpdating)
                return;

            isDirty = false;
            StartCoroutine(UpdateLayoutsEnumerator());
        }

        private IEnumerator UpdateLayoutsEnumerator()
        {
            isUpdating = true;
            groupLayoutGroup.enabled = false;
            contentLayoutGroup.enabled = false;
            yield return null;
            groupLayoutGroup.enabled = true;
            yield return null;
            contentLayoutGroup.enabled = true;
            isUpdating = false;
        }
    }
}
