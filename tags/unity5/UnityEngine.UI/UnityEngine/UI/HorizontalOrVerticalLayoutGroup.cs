namespace UnityEngine.UI
{
    using System;
    using UnityEngine;

    public abstract class HorizontalOrVerticalLayoutGroup : LayoutGroup
    {
        [SerializeField]
        protected bool m_ChildForceExpandHeight = true;
        [SerializeField]
        protected bool m_ChildForceExpandWidth = true;
        [SerializeField]
        protected float m_Spacing;

        protected HorizontalOrVerticalLayoutGroup()
        {
        }

        protected void CalcAlongAxis(int axis, bool isVertical)
        {
            float num = (axis != 0) ? ((float) base.padding.vertical) : ((float) base.padding.horizontal);
            float b = num;
            float num3 = num;
            float num4 = 0f;
            bool flag = isVertical ^ (axis == 1);
            for (int i = 0; i < base.rectChildren.Count; i++)
            {
                RectTransform rect = base.rectChildren[i];
                float minSize = LayoutUtility.GetMinSize(rect, axis);
                float preferredSize = LayoutUtility.GetPreferredSize(rect, axis);
                float flexibleSize = LayoutUtility.GetFlexibleSize(rect, axis);
                if ((axis != 0) ? this.childForceExpandHeight : this.childForceExpandWidth)
                {
                    flexibleSize = Mathf.Max(flexibleSize, 1f);
                }
                if (flag)
                {
                    b = Mathf.Max(minSize + num, b);
                    num3 = Mathf.Max(preferredSize + num, num3);
                    num4 = Mathf.Max(flexibleSize, num4);
                }
                else
                {
                    b += minSize + this.spacing;
                    num3 += preferredSize + this.spacing;
                    num4 += flexibleSize;
                }
            }
            if (!flag && (base.rectChildren.Count > 0))
            {
                b -= this.spacing;
                num3 -= this.spacing;
            }
            num3 = Mathf.Max(b, num3);
            base.SetLayoutInputForAxis(b, num3, num4, axis);
        }

        protected void SetChildrenAlongAxis(int axis, bool isVertical)
        {
            float num = base.rectTransform.rect.size[axis];
            if (isVertical ^ (axis == 1))
            {
                float num2 = num - ((axis != 0) ? ((float) base.padding.vertical) : ((float) base.padding.horizontal));
                for (int i = 0; i < base.rectChildren.Count; i++)
                {
                    RectTransform rect = base.rectChildren[i];
                    float minSize = LayoutUtility.GetMinSize(rect, axis);
                    float preferredSize = LayoutUtility.GetPreferredSize(rect, axis);
                    float flexibleSize = LayoutUtility.GetFlexibleSize(rect, axis);
                    if ((axis != 0) ? this.childForceExpandHeight : this.childForceExpandWidth)
                    {
                        flexibleSize = Mathf.Max(flexibleSize, 1f);
                    }
                    float requiredSpaceWithoutPadding = Mathf.Clamp(num2, minSize, (flexibleSize <= 0f) ? preferredSize : num);
                    float startOffset = base.GetStartOffset(axis, requiredSpaceWithoutPadding);
                    base.SetChildAlongAxis(rect, axis, startOffset, requiredSpaceWithoutPadding);
                }
            }
            else
            {
                float pos = (axis != 0) ? ((float) base.padding.top) : ((float) base.padding.left);
                if ((base.GetTotalFlexibleSize(axis) == 0f) && (base.GetTotalPreferredSize(axis) < num))
                {
                    pos = base.GetStartOffset(axis, base.GetTotalPreferredSize(axis) - ((axis != 0) ? ((float) base.padding.vertical) : ((float) base.padding.horizontal)));
                }
                float t = 0f;
                if (base.GetTotalMinSize(axis) != base.GetTotalPreferredSize(axis))
                {
                    t = Mathf.Clamp01((num - base.GetTotalMinSize(axis)) / (base.GetTotalPreferredSize(axis) - base.GetTotalMinSize(axis)));
                }
                float num11 = 0f;
                if ((num > base.GetTotalPreferredSize(axis)) && (base.GetTotalFlexibleSize(axis) > 0f))
                {
                    num11 = (num - base.GetTotalPreferredSize(axis)) / base.GetTotalFlexibleSize(axis);
                }
                for (int j = 0; j < base.rectChildren.Count; j++)
                {
                    RectTransform transform2 = base.rectChildren[j];
                    float a = LayoutUtility.GetMinSize(transform2, axis);
                    float b = LayoutUtility.GetPreferredSize(transform2, axis);
                    float num15 = LayoutUtility.GetFlexibleSize(transform2, axis);
                    if ((axis != 0) ? this.childForceExpandHeight : this.childForceExpandWidth)
                    {
                        num15 = Mathf.Max(num15, 1f);
                    }
                    float size = Mathf.Lerp(a, b, t) + (num15 * num11);
                    base.SetChildAlongAxis(transform2, axis, pos, size);
                    pos += size + this.spacing;
                }
            }
        }

        public bool childForceExpandHeight
        {
            get
            {
                return this.m_ChildForceExpandHeight;
            }
            set
            {
                base.SetProperty<bool>(ref this.m_ChildForceExpandHeight, value);
            }
        }

        public bool childForceExpandWidth
        {
            get
            {
                return this.m_ChildForceExpandWidth;
            }
            set
            {
                base.SetProperty<bool>(ref this.m_ChildForceExpandWidth, value);
            }
        }

        public float spacing
        {
            get
            {
                return this.m_Spacing;
            }
            set
            {
                base.SetProperty<float>(ref this.m_Spacing, value);
            }
        }
    }
}

