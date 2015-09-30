namespace UnityEngine.UI
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.Serialization;

    [RequireComponent(typeof(RectTransform)), ExecuteInEditMode, DisallowMultipleComponent]
    public abstract class LayoutGroup : UIBehaviour, ILayoutElement, ILayoutController, ILayoutGroup
    {
        [SerializeField, FormerlySerializedAs("m_Alignment")]
        protected TextAnchor m_ChildAlignment;
        [SerializeField]
        protected RectOffset m_Padding = new RectOffset();
        [NonSerialized]
        private RectTransform m_Rect;
        [NonSerialized]
        private List<RectTransform> m_RectChildren = new List<RectTransform>();
        private Vector2 m_TotalFlexibleSize = Vector2.zero;
        private Vector2 m_TotalMinSize = Vector2.zero;
        private Vector2 m_TotalPreferredSize = Vector2.zero;
        protected DrivenRectTransformTracker m_Tracker;

        protected LayoutGroup()
        {
            if (this.m_Padding == null)
            {
                this.m_Padding = new RectOffset();
            }
        }

        public virtual void CalculateLayoutInputHorizontal()
        {
            this.m_RectChildren.Clear();
            for (int i = 0; i < this.rectTransform.childCount; i++)
            {
                RectTransform child = this.rectTransform.GetChild(i) as RectTransform;
                if (child != null)
                {
                    ILayoutIgnorer component = child.GetComponent(typeof(ILayoutIgnorer)) as ILayoutIgnorer;
                    if (child.gameObject.activeInHierarchy && ((component == null) || !component.ignoreLayout))
                    {
                        this.m_RectChildren.Add(child);
                    }
                }
            }
            this.m_Tracker.Clear();
        }

        public abstract void CalculateLayoutInputVertical();
        protected float GetStartOffset(int axis, float requiredSpaceWithoutPadding)
        {
            float num = requiredSpaceWithoutPadding + ((axis != 0) ? ((float) this.padding.vertical) : ((float) this.padding.horizontal));
            float num2 = this.rectTransform.rect.size[axis];
            float num3 = num2 - num;
            float num4 = 0f;
            if (axis == 0)
            {
                num4 = ((float) (this.childAlignment % TextAnchor.MiddleLeft)) * 0.5f;
            }
            else
            {
                num4 = ((float) (this.childAlignment / TextAnchor.MiddleLeft)) * 0.5f;
            }
            return (((axis != 0) ? ((float) this.padding.top) : ((float) this.padding.left)) + (num3 * num4));
        }

        protected float GetTotalFlexibleSize(int axis)
        {
            return this.m_TotalFlexibleSize[axis];
        }

        protected float GetTotalMinSize(int axis)
        {
            return this.m_TotalMinSize[axis];
        }

        protected float GetTotalPreferredSize(int axis)
        {
            return this.m_TotalPreferredSize[axis];
        }

        protected override void OnDidApplyAnimationProperties()
        {
            this.SetDirty();
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
            base.OnRectTransformDimensionsChange();
            if (this.isRootLayoutGroup)
            {
                this.SetDirty();
            }
        }

        protected virtual void OnTransformChildrenChanged()
        {
            this.SetDirty();
        }

        protected override void OnValidate()
        {
            this.SetDirty();
        }

        protected void SetChildAlongAxis(RectTransform rect, int axis, float pos, float size)
        {
            if (rect != null)
            {
                this.m_Tracker.Add(this, rect, DrivenTransformProperties.SizeDelta | DrivenTransformProperties.Anchors | DrivenTransformProperties.AnchoredPosition);
                rect.SetInsetAndSizeFromParentEdge((axis != 0) ? RectTransform.Edge.Top : RectTransform.Edge.Left, pos, size);
            }
        }

        protected void SetDirty()
        {
            if (this.IsActive())
            {
                LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
            }
        }

        public abstract void SetLayoutHorizontal();
        protected void SetLayoutInputForAxis(float totalMin, float totalPreferred, float totalFlexible, int axis)
        {
            this.m_TotalMinSize[axis] = totalMin;
            this.m_TotalPreferredSize[axis] = totalPreferred;
            this.m_TotalFlexibleSize[axis] = totalFlexible;
        }

        public abstract void SetLayoutVertical();
        protected void SetProperty<T>(ref T currentValue, T newValue)
        {
            if (((((T) currentValue) != null) || (newValue != null)) && ((((T) currentValue) == null) || !currentValue.Equals(newValue)))
            {
                currentValue = newValue;
                this.SetDirty();
            }
        }

        public TextAnchor childAlignment
        {
            get
            {
                return this.m_ChildAlignment;
            }
            set
            {
                this.SetProperty<TextAnchor>(ref this.m_ChildAlignment, value);
            }
        }

        public virtual float flexibleHeight
        {
            get
            {
                return this.GetTotalFlexibleSize(1);
            }
        }

        public virtual float flexibleWidth
        {
            get
            {
                return this.GetTotalFlexibleSize(0);
            }
        }

        private bool isRootLayoutGroup
        {
            get
            {
                return ((base.transform.parent == null) || (base.transform.parent.GetComponent(typeof(ILayoutGroup)) == null));
            }
        }

        public virtual int layoutPriority
        {
            get
            {
                return 0;
            }
        }

        public virtual float minHeight
        {
            get
            {
                return this.GetTotalMinSize(1);
            }
        }

        public virtual float minWidth
        {
            get
            {
                return this.GetTotalMinSize(0);
            }
        }

        public RectOffset padding
        {
            get
            {
                return this.m_Padding;
            }
            set
            {
                this.SetProperty<RectOffset>(ref this.m_Padding, value);
            }
        }

        public virtual float preferredHeight
        {
            get
            {
                return this.GetTotalPreferredSize(1);
            }
        }

        public virtual float preferredWidth
        {
            get
            {
                return this.GetTotalPreferredSize(0);
            }
        }

        protected List<RectTransform> rectChildren
        {
            get
            {
                return this.m_RectChildren;
            }
        }

        protected RectTransform rectTransform
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
    }
}

