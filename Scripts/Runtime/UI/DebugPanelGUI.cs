using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace BrunoMikoski.DebugPanel.GUI
{
    public class DebugPanelGUI : MonoBehaviour
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

            for (int i = 0; i < currentDebugPage.ChildPages.Count; i++)
            {
                DebugPage childPage = currentDebugPage.ChildPages[i];
                displayItems.Add(new PageLink(childPage.Title, childPage.SubTitle, childPage.SpriteName, currentDebugPage, childPage));
            }

            displayItems = displayItems.OrderBy(baseItem => !baseItem.IsFavorite)
                .ThenBy(baseItem => !(baseItem is PageLink)).ThenBy(baseItem => baseItem.Title).ToList();
            
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
                        if (possibleDisplay is DebuggableFieldBaseGUI debuggableFieldGUI)
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

        public void ShowOnlyMatches(string searchValue)
        {
            for (int i = 0; i < displayingItems.Count; i++)
            {
                DebuggableGUIBase displayingItem = displayingItems[i];

                displayingItem.gameObject.SetActive(MatchSearch(displayingItem, searchValue));
            }
        }
        
        private bool MatchSearch(DebuggableGUIBase displayingItem, string searchValue)
        {
            if (string.IsNullOrEmpty(searchValue))
                return true;

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
