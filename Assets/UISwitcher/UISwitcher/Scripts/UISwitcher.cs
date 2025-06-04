using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace UISwitcher
{
    public class UISwitcher : UINullableToggle
    {
        private readonly Vector2 _min = new(0, 0.5f);
        private readonly Vector2 _max = new(1, 0.5f);
        private readonly Vector2 _middle = new(0.5f, 0.5f);

        [SerializeField] private Graphic backgroundGraphic;
        [SerializeField] private Color onColor, offColor, nullColor;
        [SerializeField] private RectTransform tipRect;

        // Unity Events for triggering actions based on switch state
        [SerializeField] private UnityEvent onEnableEvent;
        [SerializeField] private UnityEvent onDisableEvent;
        [SerializeField] private UnityEvent onNullEvent;

        private Color backgroundColor
        {
            set
            {
                if (backgroundGraphic == null) return;
                backgroundGraphic.color = value;
            }
        }

        protected override void OnChanged(bool? obj)
        {
            if (obj.HasValue)
            {
                if (obj.Value)
                {
                    SetOn();
                    onEnableEvent.Invoke();
                }
                else
                {
                    SetOff();
                    onDisableEvent.Invoke();
                }
            }
            else
            {
                SetNull();
                onNullEvent.Invoke();
            }
        }

        private void SetOn()
        {
            SetAnchors(_max);
            backgroundColor = onColor;
        }

        private void SetOff()
        {
            SetAnchors(_min);
            backgroundColor = offColor;
        }

        private void SetNull()
        {
            SetAnchors(_middle);
            backgroundColor = nullColor;
        }

        private void SetAnchors(Vector2 anchor)
        {
            tipRect.anchorMin = anchor;
            tipRect.anchorMax = anchor;
            tipRect.pivot = anchor;
        }
    }
}
