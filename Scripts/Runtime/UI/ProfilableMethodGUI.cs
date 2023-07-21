using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

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

            public string AsCSVString()
            {
                StringBuilder csvBuilder = new StringBuilder();
                csvBuilder.AppendLine("MedianTime,MeanTime,MinTime,MaxTime,Range,TotalTime,NumberOfRuns,ExecutionDateTime"); // CSV header
                for (int i = 0; i < history.Count; i++)
                {
                    PerformanceTester.PerformanceResult item = history[i];
                    csvBuilder.AppendLine(
                        $"{item.MedianTime},{item.MeanTime},{item.MinTime},{item.MaxTime},{item.Range},{item.TotalTime},{item.NumberOfRuns},{DateTime.FromBinary(item.ExecutionDateTime)}");
                }

                return csvBuilder.ToString();
            }
        }
        
        
        [SerializeField]
        private TMP_InputField displayText;
        [SerializeField]
        private LayoutGroup layoutGroup;
        [SerializeField]
        private Button runButton;
        [SerializeField]
        private GraphGUI graphGUI;
        [SerializeField]
        private Button clearDataButton;
        [SerializeField]
        private Button copyDataButton;
        
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
                UpdateDisplay();
            }
        }

        private void Awake()
        {
            runButton.onClick.AddListener(OnClickRunButton);
            clearDataButton.onClick.AddListener(OnClickClear);
            copyDataButton.onClick.AddListener(OnClickCopy);
        }


        private void OnDestroy()
        {
            runButton.onClick.RemoveListener(OnClickRunButton);
            clearDataButton.onClick.RemoveListener(OnClickClear);
            copyDataButton.onClick.RemoveListener(OnClickCopy);
        }

        private void OnClickCopy()
        {
            string finalPrint = "------- JSON -------";
            finalPrint += "\n" + JsonUtility.ToJson(profilerHistory);
            Debug.Log(finalPrint);
            finalPrint += "\n------- CSV -------";
            finalPrint += "\n" + profilerHistory.AsCSVString();

            GUIUtility.systemCopyBuffer = finalPrint;
        }

        private void OnClickClear()
        {
            profilerHistory.history.Clear();
            SaveHistory();
            UpdateDisplay();            
        }

        private void OnClickRunButton()
        {
            if (profilableMethod.Method == null || profilableMethod.Owner == null)
                return;

            PerformanceTester.PerformanceResult profileResults = PerformanceTester.RunTest(() =>
            {
                profilableMethod.Method.Invoke(profilableMethod.Owner, new object[] { });
            }, profilableMethod.ExecutionCount);
            
            profilerHistory.Add(profileResults);
            SaveHistory();
            UpdateDisplay();
            Debug.Log(profileResults.ToString());
        }
        
        private void UpdateDisplay()
        {
            PerformanceTester.PerformanceResult lastResult = profilerHistory.history[^1];
            SetDisplayText(lastResult);

            if (profilerHistory.history.Count > 1)
            {
                List<Vector2> points = new List<Vector2>();
                for (int i = 0; i < profilerHistory.history.Count; i++)
                {
                    PerformanceTester.PerformanceResult performanceResult = profilerHistory.history[i];

                    points.Add(new Vector2(i, (float) performanceResult.MedianTime));
                }
                graphGUI.SetPoints(points);
            }
        }

        private void SaveHistory()
        {
            string json = JsonUtility.ToJson(profilerHistory);
            PlayerPrefs.SetString(StorageKey, json);
        }

        private void SetDisplayText(PerformanceTester.PerformanceResult performanceResult)
        {
            if (profilerHistory.history.Count > 1)
            {
                int indexOf = profilerHistory.history.IndexOf(performanceResult);
                PerformanceTester.PerformanceResult previous = profilerHistory.history[indexOf - 1];

                SetDisplayText(performanceResult.StringResultComparedTo(previous));
            }
            else
            {
                SetDisplayText(performanceResult.ToString());
            }
        }
        
        private void SetDisplayText(string targetText)
        {
            displayText.text = targetText;
            DebugPanelGUI.StartCoroutine(ToggleLayoutGroupEnumerator());
        }

        private IEnumerator ToggleLayoutGroupEnumerator()
        {
            layoutGroup.enabled = false;
            yield return null;
            layoutGroup.enabled = true;
        }
    }
}