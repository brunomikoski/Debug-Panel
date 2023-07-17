using System;
using System.Collections;
using System.Collections.Generic;
using BrunoMikoski.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BrunoMikoski.DebugTools.GUI
{
    internal class ProfilableMethodGUI : DebuggableMethodGUI
    {
        [Serializable]
        public class ProfilerHistory
        {
            [SerializeField]
            public List<PerformanceTester.PerformanceResult> history = new List<PerformanceTester.PerformanceResult>();
            
            public float maxMedianTime = float.MinValue;
            public void Add(PerformanceTester.PerformanceResult performanceResult)
            {
                history.Add(performanceResult);
                if (performanceResult.MedianTime > maxMedianTime)
                    maxMedianTime = (float) performanceResult.MedianTime;
            }
        }
        
        
        [SerializeField]
        private TMP_InputField displayText;
        [SerializeField]
        private LayoutGroup layoutGroup;
        [SerializeField]
        private Button runButton;
        
        private ProfilableMethod profilableMethod;

        public override Type[] DisplayTypes => new[] { typeof(ProfilableMethod) };


        private ProfilerHistory profilerHistory;

        private string StorageKey => $"{DebugPage.PagePath}/{profilableMethod.Method.Name}.History";

        internal override void Initialize(DebuggableItemBase targetDebuggableItem, DebugPage targetDebugPage)
        {
            base.Initialize(targetDebuggableItem, targetDebugPage);
            profilableMethod = (ProfilableMethod)targetDebuggableItem;


            string storedDataJson = PlayerPrefs.GetString(StorageKey, "");
            if (string.IsNullOrEmpty(storedDataJson))
            {
                profilerHistory = new ProfilerHistory();
            }
            else
            {
                profilerHistory = JsonUtility.FromJson<ProfilerHistory>(storedDataJson);
            }
        }

        private void Awake()
        {
            runButton.onClick.AddListener(OnClickRunButton);
        }

        private void OnDestroy()
        {
            runButton.onClick.RemoveListener(OnClickRunButton);
        }

        private void OnClickRunButton()
        {
            if (profilableMethod.Method == null || profilableMethod.Owner == null)
                return;


            PerformanceTester.PerformanceResult profileResults = PerformanceTester.RunTest(() =>
            {
                profilableMethod.Method.Invoke(profilableMethod.Owner, new object[] { });
            }, profilableMethod.ExecutionCount);
            
            
            SetDisplayText(profileResults.ToString());
            profilerHistory.Add(profileResults);
            StoreHistory();
        }

        private void StoreHistory()
        {
            string json = JsonUtility.ToJson(profilerHistory);
            PlayerPrefs.SetString(StorageKey, json);
        }

        private void SetDisplayText(string targetText)
        {
            if (!string.IsNullOrEmpty(targetText) && targetText.Length != displayText.text.Length)
            {
                displayText.text = targetText;
                DebugPanelGUI.StartCoroutine(ToggleLayoutGroupEnumerator());
            }
        }

        private IEnumerator ToggleLayoutGroupEnumerator()
        {
            layoutGroup.enabled = false;
            yield return null;
            layoutGroup.enabled = true;
        }
    }
}