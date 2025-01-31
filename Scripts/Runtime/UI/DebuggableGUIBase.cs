using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace BrunoMikoski.DebugTools.GUI
{
    public abstract class DebuggableGUIBase : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        private const int TIME_TO_FAVORITE = 2;

        public abstract Type[] DisplayTypes { get; }

        [FormerlySerializedAs("favIconImage")]
        [SerializeField]
        protected Image iconImage;
        [SerializeField]
        protected TMP_Text display;
        [SerializeField]
        protected TMP_Text subTitle;
        [SerializeField]
        protected Selectable selectable;

        internal DebugPage DebugPage;
        
        private DebuggableItemBase debuggableItem;
        internal DebuggableItemBase DebuggableItem => debuggableItem;

        private bool isPointerDown;
        private float heldTime;
        private bool toggledFavorite;

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

            UpdateIconVisibility();

            SetSubtitle(debuggableItem.SubTitle);
        }

        public void SetIcon(Sprite targetIcon)
        {
            if (iconImage == null)
                return;
            
            iconImage.sprite = targetIcon;
            UpdateIconVisibility();
        }
        
        protected virtual void UpdateIconVisibility()
        {
            if (iconImage == null)
            { 
                iconImage.gameObject.SetActive(false);
                return;
            }
            
            iconImage.gameObject.SetActive(debuggableItem.IsFavorite);
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
            if (iconImage == null)
                return;
            
            debuggableItem.SetIsFavorite(!debuggableItem.IsFavorite);
            DebugPanel.UpdateDebuggableFavorite(debuggableItem);
            UpdateIconVisibility();
        }

        public void ShowPathAsSubtitle()
        {
            SetSubtitle(debuggableItem.ParentPath);
        }

        private void SetSubtitle(string targetSubtitle)
        {
            if (string.IsNullOrEmpty(targetSubtitle))
            {
                subTitle.gameObject.SetActive(false);
                return;
            }
            subTitle.gameObject.SetActive(true);
            subTitle.text = targetSubtitle;
        }
    }
}