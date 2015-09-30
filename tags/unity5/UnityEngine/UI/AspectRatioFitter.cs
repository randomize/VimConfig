namespace UnityEngine.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    [ExecuteInEditMode, AddComponentMenu("Layout/Aspect Ratio Fitter", 0x8e), RequireComponent(typeof(RectTransform))]
    public class AspectRatioFitter : UIBehaviour, ILayoutController, ILayoutSelfController
    {
        [SerializeField]
        private AspectMode m_AspectMode;
        [SerializeField]
        private float m_AspectRatio = 1f;
        [NonSerialized]
        private RectTransform m_Rect;
        private DrivenRectTransformTracker m_Tracker;

        protected AspectRatioFitter()
        {
        }

        private Vector2 GetParentSize()
        {
            RectTransform parent = this.rectTransform.parent as RectTransform;
            if (parent == null)
            {
                return Vector2.zero;
            }
            return parent.rect.size;
        }

        private float GetSizeDeltaToProduceSize(float size, int axis)
        {
            return (size - (this.GetParentSize()[axis] * (this.rectTransform.anchorMax[axis] - this.rectTransform.anchorMin[axis])));
        }

        protected override void OnDisable()
        {
            this.m_Tracker.Clear();
            LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
            base.OnDisable();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.SetDirty();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            this.UpdateRect();
        }

        protected override void OnValidate()
        {
            this.m_AspectRatio = Mathf.Clamp(this.m_AspectRatio, 0.001f, 1000f);
            this.SetDirty();
        }

        protected void SetDirty()
        {
            if (this.IsActive())
            {
                this.UpdateRect();
            }
        }

        public virtual void SetLayoutHorizontal()
        {
        }

        public virtual void SetLayoutVertical()
        {
        }

        private void UpdateRect()
        {
            if (this.IsActive())
            {
                Vector2 zero;
                this.m_Tracker.Clear();
                switch (this.m_AspectMode)
                {
                    case AspectMode.None:
                        if (!Application.isPlaying)
                        {
                            this.m_AspectRatio = Mathf.Clamp((float) (this.rectTransform.rect.width / this.rectTransform.rect.height), (float) 0.001f, (float) 1000f);
                        }
                        return;

                    case AspectMode.WidthControlsHeight:
                        this.m_Tracker.Add(this, this.rectTransform, DrivenTransformProperties.SizeDeltaY);
                        this.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, this.rectTransform.rect.width / this.m_AspectRatio);
                        return;

                    case AspectMode.HeightControlsWidth:
                        this.m_Tracker.Add(this, this.rectTransform, DrivenTransformProperties.SizeDeltaX);
                        this.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, this.rectTransform.rect.height * this.m_AspectRatio);
                        return;

                    case AspectMode.FitInParent:
                    case AspectMode.EnvelopeParent:
                    {
                        this.m_Tracker.Add(this, this.rectTransform, DrivenTransformProperties.SizeDelta | DrivenTransformProperties.Anchors | DrivenTransformProperties.AnchoredPosition);
                        this.rectTransform.anchorMin = Vector2.zero;
                        this.rectTransform.anchorMax = Vector2.one;
                        this.rectTransform.anchoredPosition = Vector2.zero;
                        zero = Vector2.zero;
                        Vector2 parentSize = this.GetParentSize();
                        if (!(((parentSize.y * this.aspectRatio) < parentSize.x) ^ (this.m_AspectMode == AspectMode.FitInParent)))
                        {
                            zero.x = this.GetSizeDeltaToProduceSize(parentSize.y * this.aspectRatio, 0);
                            break;
                        }
                        zero.y = this.GetSizeDeltaToProduceSize(parentSize.x / this.aspectRatio, 1);
                        break;
                    }
                    default:
                        return;
                }
                this.rectTransform.sizeDelta = zero;
            }
        }

        public AspectMode aspectMode
        {
            get
            {
                return this.m_AspectMode;
            }
            set
            {
                if (SetPropertyUtility.SetStruct<AspectMode>(ref this.m_AspectMode, value))
                {
                    this.SetDirty();
                }
            }
        }

        public float aspectRatio
        {
            get
            {
                return this.m_AspectRatio;
            }
            set
            {
                if (SetPropertyUtility.SetStruct<float>(ref this.m_AspectRatio, value))
                {
                    this.SetDirty();
                }
            }
        }

        private RectTransform rectTransform
        {
            get
            {
                if (this.m_Rect == null)
                {
                    this.m_Rect = base.GetComponent<RectTransform>();
                }
                return this.m_Rect;
            }
        }

        public enum AspectMode
        {
            None,
            WidthControlsHeight,
            HeightControlsWidth,
            FitInParent,
            EnvelopeParent
        }
    }
}

