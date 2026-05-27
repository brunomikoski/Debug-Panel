using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace BrunoMikoski.DebugTools.GUI
{
    [DisallowMultipleComponent]
    public sealed class ScrollToItemOnSelection : MonoBehaviour, ISelectHandler
    {
        private const float PUSH_TO_NEXT_PAGE_PERCENTAGE = 0.8f;
        private const float SCROLL_DURATION = 0.25f;

        private static readonly Vector3[] s_Corners = new Vector3[4];

        [SerializeField]
        private bool onlyOnGamepad = true;

        [SerializeField]
        private bool deferOneFrame = true;

        private RectTransform itemRectTransform;
        private ScrollRect scrollRect;
        private RectTransform scrollRectRectTransform;
        private Coroutine scrollRoutine;

        private void Awake()
        {
            itemRectTransform = transform as RectTransform;
            CacheScrollRect();
        }

        private void CacheScrollRect()
        {
            scrollRect = GetComponentInParent<ScrollRect>();
            if (scrollRect != null)
                scrollRectRectTransform = scrollRect.transform as RectTransform;
        }

        void ISelectHandler.OnSelect(BaseEventData eventData)
        {
            if (onlyOnGamepad && !IsCurrentInputGamepad())
                return;

            if (scrollRect == null)
                CacheScrollRect();

            if (scrollRect == null)
                return;

            if (deferOneFrame)
            {
                if (!isActiveAndEnabled)
                    return;

                StartCoroutine(DeferredScroll());
            }
            else
            {
                ScrollToItem();
            }
        }

        private IEnumerator DeferredScroll()
        {
            yield return null;
            Canvas.ForceUpdateCanvases();
            ScrollToItem();
        }

        private void ScrollToItem()
        {
            RectTransform viewport = scrollRect.viewport != null ? scrollRect.viewport : scrollRectRectTransform;
            RectTransform content = scrollRect.content;

            if (viewport == null || content == null)
                return;

            Rect viewRect = viewport.rect;
            Bounds itemBounds = TransformBoundsTo(itemRectTransform, viewport);
            Bounds contentBounds = TransformBoundsTo(content, viewport);

            if (scrollRect.vertical)
            {
                float hiddenHeight = contentBounds.size.y - viewRect.height;
                if (hiddenHeight > 0f)
                {
                    float offsetY = viewRect.center.y - itemBounds.center.y;
                    float pushOffset = (viewRect.height * 0.5f) * PUSH_TO_NEXT_PAGE_PERCENTAGE;

                    if (Mathf.Abs(offsetY) > pushOffset)
                    {
                        float scrollPos = scrollRect.verticalNormalizedPosition -
                                          (offsetY * (PUSH_TO_NEXT_PAGE_PERCENTAGE * 2.0f) / hiddenHeight);
                        StartScroll(true, Mathf.Clamp01(scrollPos));
                    }
                }
            }

            if (scrollRect.horizontal)
            {
                float hiddenWidth = contentBounds.size.x - viewRect.width;
                if (hiddenWidth > 0f)
                {
                    float offsetX = viewRect.center.x - itemBounds.center.x;
                    float pushOffset = (viewRect.width * 0.5f) * PUSH_TO_NEXT_PAGE_PERCENTAGE;

                    if (Mathf.Abs(offsetX) > pushOffset)
                    {
                        float scrollPos = scrollRect.horizontalNormalizedPosition -
                                          (offsetX * (PUSH_TO_NEXT_PAGE_PERCENTAGE * 2.0f) / hiddenWidth);
                        StartScroll(false, Mathf.Clamp01(scrollPos));
                    }
                }
            }
        }

        private void StartScroll(bool vertical, float targetNormalized)
        {
            if (scrollRoutine != null)
                StopCoroutine(scrollRoutine);

            scrollRoutine = StartCoroutine(ScrollRoutine(vertical, targetNormalized));
        }

        private IEnumerator ScrollRoutine(bool vertical, float targetNormalized)
        {
            float start = vertical ? scrollRect.verticalNormalizedPosition : scrollRect.horizontalNormalizedPosition;
            float elapsed = 0f;

            while (elapsed < SCROLL_DURATION)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / SCROLL_DURATION));
                float value = Mathf.Lerp(start, targetNormalized, t);

                if (vertical)
                    scrollRect.verticalNormalizedPosition = value;
                else
                    scrollRect.horizontalNormalizedPosition = value;

                yield return null;
            }

            if (vertical)
                scrollRect.verticalNormalizedPosition = targetNormalized;
            else
                scrollRect.horizontalNormalizedPosition = targetNormalized;

            scrollRoutine = null;
        }

        private static bool IsCurrentInputGamepad()
        {
#if ENABLE_INPUT_SYSTEM
            Gamepad gamepad = Gamepad.current;
            if (gamepad == null)
                return false;

            return gamepad.lastUpdateTime >= (Mouse.current != null ? Mouse.current.lastUpdateTime : 0)
                && gamepad.lastUpdateTime >= (Keyboard.current != null ? Keyboard.current.lastUpdateTime : 0);
#else
            return Input.GetJoystickNames().Length > 0;
#endif
        }

        private static Bounds TransformBoundsTo(RectTransform source, Transform target)
        {
            if (source == null)
                return new Bounds();

            source.GetWorldCorners(s_Corners);

            Vector3 vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            Matrix4x4 matrix = target.worldToLocalMatrix;
            for (int i = 0; i < 4; i++)
            {
                Vector3 v = matrix.MultiplyPoint3x4(s_Corners[i]);
                vMin = Vector3.Min(v, vMin);
                vMax = Vector3.Max(v, vMax);
            }

            Bounds bounds = new Bounds(vMin, Vector3.zero);
            bounds.Encapsulate(vMax);
            return bounds;
        }
    }
}
