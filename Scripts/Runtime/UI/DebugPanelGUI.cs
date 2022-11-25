using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace BrunoMikoski.DebugPanel.GUI
{
    internal class DebugPanelGUI : MonoBehaviour
    {
        [SerializeField] 
        private TMP_Text title;


        private List<DebuggableGUIBase> displayingItems = new List<DebuggableGUIBase>();

        private DebugPage currentDebugPage;

        public DebugPage CurrentDebugPage => currentDebugPage;

        private DebuggableGUIBase[] templates;


        private void Awake()
        {
            templates = GetComponentsInChildren<DebuggableGUIBase>(true);
            for (int i = 0; i < templates.Length; i++)
                templates[i].gameObject.SetActive(false);
        }

        public void DisplayDebugPage(DebugPage targetDebugPage)
        {
            CleanUp();
            
            currentDebugPage = targetDebugPage;
            UpdateTitle();
            Generate();
        }

        public void UpdateTitle()
        {
            title.text = currentDebugPage.Title;
        }

        private void Generate()
        {
            List<DebuggableItemBase> displayItems = new List<DebuggableItemBase>(currentDebugPage.Items);

            foreach (DebugPage childPage in currentDebugPage.ChildPages)
            {
                DebuggablePageLink debuggableItemBase = new DebuggablePageLink(childPage.Title, childPage.SubTitle, childPage);
                debuggableItemBase.SetFinalFullPath($"{currentDebugPage.PagePath}{debuggableItemBase.Title}");
                displayItems.Add(debuggableItemBase);
            }

            displayItems = displayItems.OrderBy(baseItem => !baseItem.IsFavorite)
                .ThenBy(baseItem => !(baseItem is DebuggablePageLink)).ThenBy(baseItem => baseItem.Title).ToList();
            
            for (int i = 0; i < displayItems.Count; i++)
            {
                DebuggableItemBase debuggableItemBase = displayItems[i];
                if (!TryGetDisplayForDebugItem(debuggableItemBase, out DebuggableGUIBase debuggableGUI))
                    continue;

                debuggableGUI.Initialize(debuggableItemBase, currentDebugPage);
                debuggableGUI.gameObject.SetActive(true);
            }
        }

        private bool TryGetDisplayForDebugItem(DebuggableItemBase debuggableItemBase, out DebuggableGUIBase debuggableGUIBase)
        {
            List<DebuggableGUIBase> possibleDisplays = new List<DebuggableGUIBase>();
            for (int i = 0; i < templates.Length; i++)
            {
                DebuggableGUIBase template = templates[i];
                if (template.DisplayTypes.Contains(debuggableItemBase.GetType()))
                    possibleDisplays.Add(template);
            }

            if (possibleDisplays.Count == 1)
            {
                debuggableGUIBase = Instantiate(possibleDisplays[0], possibleDisplays[0].transform.parent);
                displayingItems.Add(debuggableGUIBase);
                return true;
            }
            if (possibleDisplays.Count > 1)
            {
                if (debuggableItemBase is DebuggableField debuggableField)
                {
                    for (int i = 0; i < possibleDisplays.Count; i++)
                    {
                        DebuggableGUIBase possibleDisplay = possibleDisplays[i];
                        if (possibleDisplay is DebuggableFieldGUIBase debuggableFieldGUI)
                        {
                            if (!debuggableFieldGUI.CanBeUsedForField(debuggableField.FieldInfo))
                                continue;
                        
                            debuggableGUIBase = Instantiate(debuggableFieldGUI, debuggableFieldGUI.transform.parent);
                            displayingItems.Add(debuggableGUIBase);
                            return true;
                        }
                    }
                }
            }

            debuggableGUIBase = null;
            return false;
        }

        private void CleanUp()
        {
            for (int i = 0; i < displayingItems.Count; i++)
                Destroy(displayingItems[i].gameObject);
            
            displayingItems.Clear();
        }

        public void ShowOnlyMatches(string searchValue, DebugPage parentPage)
        {
            for (int i = 0; i < displayingItems.Count; i++)
            {
                DebuggableGUIBase displayingItem = displayingItems[i];

                displayingItem.gameObject.SetActive(MatchSearch(parentPage, displayingItem, searchValue));
            }
        }
        
        private bool MatchSearch(DebugPage parentPage, DebuggableGUIBase displayingItem, string searchValue)
        {
            if (string.IsNullOrEmpty(searchValue))
                return true;

            if (displayingItem.DebuggableItem.FullPath.IndexOf(parentPage.PagePath, StringComparison.OrdinalIgnoreCase) == -1)
                return false;

            if (displayingItem.DebuggableItem.Path.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) > -1)
                return true;
            if (searchValue.IndexOf(displayingItem.DebuggableItem.Path, StringComparison.OrdinalIgnoreCase) > -1)
                return true;

            if (!string.IsNullOrEmpty(displayingItem.DebuggableItem.SubTitle))
            {
                if (displayingItem.DebuggableItem.SubTitle.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) > -1)
                    return true;

                if (searchValue.IndexOf(displayingItem.DebuggableItem.SubTitle, StringComparison.OrdinalIgnoreCase) > -1)
                    return true;
            }

            return false;
        }
        
    }
}
