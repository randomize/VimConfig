namespace UnityEngine.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    [ExecuteInEditMode, AddComponentMenu("Layout/Content Size Fitter", 0x8d), RequireComponent(typeof(RectTransform))]
    public class ContentSizeFitter : UIBehaviour, ILayoutController, ILayoutSelfController
    {
        [SerializeField]
        protected FitMode m_HorizontalFit;
        [NonSerialized]
        private RectTransform m_Rect;
        private DrivenRectTransformTracker m_Tracker;
        [SerializeField]
        protected FitMode m_VerticalFit;

        protected ContentSizeFitter()
        {
        }

        private void HandleSelfFittingAlongAxis(int axis)
        {
            FitMode mode = (axis != 0) ? this.verticalFit : this.horizontalFit;
            if (mode != FitMode.Unconstrained)
            {
                this.m_Tracker.Add(this, this.rectTransform, (axis != 0) ? DrivenTransformProperties.SizeDeltaY : DrivenTransformProperties.SizeDeltaX);
                if (mode == FitMode.MinSize)
                {
                    this.rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis) axis, LayoutUtility.GetMinSize(this.m_Rect, axis));
                }
                else
                {
                    this.rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis) axis, LayoutUtility.GetPreferredSize(this.m_Rect, axis));
                }
            }
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
            this.SetDirty();
        }

        protected override void OnValidate()
        {
            this.SetDirty();
        }

        protected void SetDirty()
        {
            if (this.IsActive())
            {
                LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
            }
        }

        public virtual void SetLayoutHorizontal()
        {
            this.m_Tracker.Clear();
            this.HandleSelfFittingAlongAxis(0);
        }

        public virtual void SetLayoutVertical()
        {
            this.HandleSelfFittingAlongAxis(1);
        }

        public FitMode horizontalFit
        {
            get
            {
                return this.m_HorizontalFit;
            }
            set
            {
                if (SetPropertyUtility.SetStruct<FitMode>(ref this.m_HorizontalFit, value))
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

        public FitMode verticalFit
        {
            get
            {
                return this.m_VerticalFit;
            }
            set
            {
                if (SetPropertyUtility.SetStruct<FitMode>(ref this.m_VerticalFit, value))
                {
                    this.SetDirty();
                }
            }
        }

        public enum FitMode
        {
            Unconstrained,
            MinSize,
            PreferredSize
        }
    }
}

