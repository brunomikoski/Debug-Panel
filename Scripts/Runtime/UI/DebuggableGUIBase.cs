using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BrunoMikoski.DebugPanel.GUI
{
    public abstract class DebuggableGUIBase : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        private const int TIME_TO_FAVORITE = 2;

        public abstract Type[] DisplayTypes { get; }

        [SerializeField]
        protected Image favIconImage;
        [SerializeField]
        protected TMP_Text display;
        [SerializeField]
        protected TMP_Text subTitle;

        internal DebugPage DebugPage;
        
        private DebuggableItemBase debuggableItem;
        internal DebuggableItemBase DebuggableItem => debuggableItem;

        private bool isPointerDown;
        private float heldTime;
        private bool toggledFavorite;

        
        private DebugPanel cachedDebugPanel;
        protected DebugPanel DebugPanel
        {
            get
            {
                if (cachedDebugPanel == null)
                    cachedDebugPanel = GetComponentInParent<DebugPanel>();
                return cachedDebugPanel;
            }
        }

        private DebugPanelGUI cachedDebugPanelGUI;
        internal DebugPanelGUI DebugPanelGUI
        {
            get
            {
                if (cachedDebugPanelGUI == null)
                    cachedDebugPanelGUI = GetComponentInParent<DebugPanelGUI>();
                return cachedDebugPanelGUI;
            }
        }

        internal virtual void Initialize(DebuggableItemBase targetDebuggableItem, DebugPage targetDebugPage)
        {
            debuggableItem = targetDebuggableItem;
            DebugPage = targetDebugPage;
            display.text = debuggableItem.Title;

            UpdateFavorite();

            if (!string.IsNullOrEmpty(debuggableItem.SubTitle))
                subTitle.text = debuggableItem.SubTitle;
            else
                subTitle.gameObject.SetActive(false);
        }

        protected virtual void UpdateFavorite()
        {
            if (favIconImage == null)
                return;
            
            favIconImage.gameObject.SetActive(debuggableItem.IsFavorite);
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            isPointerDown = true;
            heldTime = 0;
            toggledFavorite = false;
        }
        
        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            isPointerDown = false;
            if (toggledFavorite)
                eventData.Use();
                
        }
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (eventData.used)
                return;
            
            OnClick();
        }

        protected virtual void OnClick(){}

        protected virtual void Update()
        {
            if (isPointerDown)
            {
                heldTime += Time.deltaTime;

                if (heldTime > TIME_TO_FAVORITE)
                {
                    heldTime = 0;
                    ToggleFavorite();
                    

                    toggledFavorite = true;
                }
            }
        }

        protected virtual void ToggleFavorite()
        {
            if (favIconImage == null)
                return;
            
            debuggableItem.SetIsFavorite(!debuggableItem.IsFavorite);
            DebugPanel.UpdateDebuggableFavorite(debuggableItem);
            UpdateFavorite();
        }
    }
}
