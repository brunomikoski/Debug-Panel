using System;
using BrunoMikoski.DebugTools.Layout;
using UnityEngine;
using UnityEngine.UI;

namespace BrunoMikoski.DebugTools.Core
{
    [DefaultExecutionOrder(-1000)]
    public sealed class DebugPanel : MonoBehaviour
    {
        internal const string DEFAULT_CATEGORY_NAME = "Generic";

        private static DebugPanel instance;
        private static DebugPanel Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<DebugPanel>();
                return instance;
            }
        }

        [Header("Settings")]
        [SerializeField]
        private bool dontDestroyOnLoad = true;
        
        [Header("References")]
        [SerializeField]
        private DebugPanelWindow debugPanelWindow;

        [SerializeField] 
        private HotKeyManager hotKeyManager;
        public static HotKeyManager HotKeyManager => Instance.hotKeyManager;

        [SerializeField]
        private Button openDebugPanelWindowButton;


        private void Awake()
        {
            openDebugPanelWindowButton.onClick.AddListener(OnClickOpenDebugPanel);
            debugPanelWindow.Close();
            openDebugPanelWindowButton.gameObject.SetActive(true);
            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            openDebugPanelWindowButton.onClick.RemoveListener(OnClickOpenDebugPanel);
        }

        private void OnClickOpenDebugPanel()
        {
            debugPanelWindow.Open();
            openDebugPanelWindowButton.gameObject.SetActive(false);
        }

        public static void CloseWindow()
        {
            if (!Application.isPlaying)
                return;

            if (Instance == null)
                return;
            
            Instance.debugPanelWindow.Close();
            Instance.openDebugPanelWindowButton.gameObject.SetActive(true);

        }

        public static void RegisterDebuggableObject(object targetObject)
        {
            if (!Application.isPlaying)
                return;

            if (Instance == null)
                return;

            Instance.debugPanelWindow.RegisterDebuggableObject(targetObject);
        }

        public static void UnregisterDebuggableObject(object targetObject)
        {
            if (!Application.isPlaying)
                return;
            
            if (Instance == null)
                return;

            Instance.debugPanelWindow.UnregisterDebuggableObject(targetObject);

        }
        
        public static void AddAction(string targetLabel, Action targetCallback)
        {
            AddAction(targetLabel, DEFAULT_CATEGORY_NAME, targetCallback);
        }

        public static void AddAction(string targetLabel, string groupName, Action targetCallback)
        {
            Instance.debugPanelWindow.AddDirectCallback(targetLabel, groupName, targetCallback);
        }
    }
}
