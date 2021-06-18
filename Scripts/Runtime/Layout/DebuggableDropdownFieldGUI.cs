using System;
using System.Collections.Generic;
using System.Reflection;
using BrunoMikoski.DebugTools.Core.Attributes;
using TMPro;
using UnityEngine;

#if SOC_ENABLED
using BrunoMikoski.ScriptableObjectCollections;
#endif


namespace BrunoMikoski.DebugTools.Layout
{
    public sealed class DebuggableDropdownFieldGUI : DebuggableFieldGUIBase
    {
        [SerializeField] 
        private TMP_Dropdown dropdown;

        private string[] availableValues;

        
#if SOC_ENABLED        
        private List<ScriptableObjectCollectionItem> allAvailableScriptableObjectsItems;
#endif

        public new void Initialize(object targetObject, FieldInfo fieldInfo,
            DebuggableFieldAttribute debuggableFieldAttribute)
        {
            base.Initialize(targetObject, fieldInfo, debuggableFieldAttribute);

            dropdown.options = new List<TMP_Dropdown.OptionData>();

            if (fieldInfo.FieldType.IsEnum)
            {
                Array values = Enum.GetValues(fieldInfo.FieldType);
                availableValues = new string[values.Length];
                for (int i = 0; i < availableValues.Length; i++)
                {
                    availableValues[i] = values.GetValue(i).ToString();
                    dropdown.options.Add(new TMP_Dropdown.OptionData(availableValues[i]));
                }

                dropdown.value = (int) fieldInfo.GetValue(targetObject);
            }
            else
            {
#if SOC_ENABLED
                if (fieldInfo.FieldType == typeof(ScriptableObjectCollectionItem) ||
                    typeof(ScriptableObjectCollectionItem).IsAssignableFrom(fieldInfo.FieldType))
                {
                    object fieldValue = fieldInfo.GetValue(targetObject);

                    allAvailableScriptableObjectsItems = new List<ScriptableObjectCollectionItem>();
                    ScriptableObjectCollectionItem selectedItem = null;
                    if (fieldValue != null)
                    {
                        selectedItem = (ScriptableObjectCollectionItem) fieldValue;
                        allAvailableScriptableObjectsItems.AddRange(selectedItem.Collection.Items);
                    }
                    else
                    {
                        allAvailableScriptableObjectsItems.AddRange(
                            CollectionsRegistry.Instance.GetAllCollectionItemsOfType(fieldInfo.FieldType));
                    }


                    availableValues = new string[allAvailableScriptableObjectsItems.Count + 1];
                    availableValues[0] = "Null";
                    dropdown.options.Add(new TMP_Dropdown.OptionData(availableValues[0]));
                    int selectedIndex = 0;


                    for (int i = 0; i < allAvailableScriptableObjectsItems.Count; i++)
                    {
                        ScriptableObjectCollectionItem scriptableObjectsItem = allAvailableScriptableObjectsItems[i];
                        dropdown.options.Add(new TMP_Dropdown.OptionData(scriptableObjectsItem.name));

                        if (selectedItem != null)
                        {
                            if (selectedItem == scriptableObjectsItem)
                                selectedIndex = i + 1;
                        }
                    }

                    dropdown.value = selectedIndex;
                }
#endif
                dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
            }
        }
        
        private void OnDestroy()
        {
            dropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
        }

        private void OnDropdownValueChanged(int selectedIndex)
        {
            if (fieldInfo.FieldType.IsEnum)
            {
                fieldInfo.SetValue(targetObject, selectedIndex);
            }
            else
            {
#if SOC_ENABLED
                if (selectedIndex == 0)
                {
                    fieldInfo.SetValue(targetObject, null);
                }
                else
                {
                    fieldInfo.SetValue(targetObject, allAvailableScriptableObjectsItems[selectedIndex - 1]);
                }
#endif
            }
        }

    }
}