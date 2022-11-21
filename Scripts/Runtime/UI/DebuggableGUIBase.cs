using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BrunoMikoski.DebugPanel.GUI
{
    public abstract class DebuggableGUIBase : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private const int TIME_TO_FAVORITE = 2;

        public abstract Type[] DisplayTypes { get; }

        [SerializeField]
        protected Image favIconImage;
        [SerializeField]
        protected TMP_Text display;
        [SerializeField]
        protected TMP_Text subTitle;

        protected DebugPage DebugPage;
        
        private DebuggableItemBase debuggableItem;
        public DebuggableItemBase DebuggableItem => debuggableItem;

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

        public virtual void Initialize(DebuggableItemBase targetDebuggableItem, DebugPage targetDebugPage)
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

        private void UpdateFavorite()
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
            {
                eventData.Use();
            }
        }
        
        protected virtual void Update()
        {
            if (isPointerDown)
            {
                heldTime += Time.deltaTime;

                if (heldTime > TIME_TO_FAVORITE)
                {
                    heldTime = 0;
                    if (favIconImage != null)
                    {
                        debuggableItem.SetIsFavorite(!debuggableItem.IsFavorite);
                        UpdateFavorite();
                    }

                    toggledFavorite = true;
                }
            }
        }

    }
}
