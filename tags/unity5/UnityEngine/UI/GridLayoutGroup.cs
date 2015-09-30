namespace UnityEngine.UI
{
    using System;
    using UnityEngine;

    [AddComponentMenu("Layout/Grid Layout Group", 0x98)]
    public class GridLayoutGroup : LayoutGroup
    {
        [SerializeField]
        protected Vector2 m_CellSize = new Vector2(100f, 100f);
        [SerializeField]
        protected Constraint m_Constraint;
        [SerializeField]
        protected int m_ConstraintCount = 2;
        [SerializeField]
        protected Vector2 m_Spacing = Vector2.zero;
        [SerializeField]
        protected Axis m_StartAxis;
        [SerializeField]
        protected Corner m_StartCorner;

        protected GridLayoutGroup()
        {
        }

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            int num = 0;
            int num2 = 0;
            if (this.m_Constraint == Constraint.FixedColumnCount)
            {
                num = num2 = this.m_ConstraintCount;
            }
            else if (this.m_Constraint == Constraint.FixedRowCount)
            {
                num = num2 = Mathf.CeilToInt((((float) base.rectChildren.Count) / ((float) this.m_ConstraintCount)) - 0.001f);
            }
            else
            {
                num = 1;
                num2 = Mathf.CeilToInt(Mathf.Sqrt((float) base.rectChildren.Count));
            }
            base.SetLayoutInputForAxis((base.padding.horizontal + ((this.cellSize.x + this.spacing.x) * num)) - this.spacing.x, (base.padding.horizontal + ((this.cellSize.x + this.spacing.x) * num2)) - this.spacing.x, -1f, 0);
        }

        public override void CalculateLayoutInputVertical()
        {
            int constraintCount = 0;
            if (this.m_Constraint == Constraint.FixedColumnCount)
            {
                constraintCount = Mathf.CeilToInt((((float) base.rectChildren.Count) / ((float) this.m_ConstraintCount)) - 0.001f);
            }
            else if (this.m_Constraint == Constraint.FixedRowCount)
            {
                constraintCount = this.m_ConstraintCount;
            }
            else
            {
                float x = base.rectTransform.rect.size.x;
                int num3 = Mathf.Max(1, Mathf.FloorToInt((((x - base.padding.horizontal) + this.spacing.x) + 0.001f) / (this.cellSize.x + this.spacing.x)));
                constraintCount = Mathf.CeilToInt(((float) base.rectChildren.Count) / ((float) num3));
            }
            float totalMin = (base.padding.vertical + ((this.cellSize.y + this.spacing.y) * constraintCount)) - this.spacing.y;
            base.SetLayoutInputForAxis(totalMin, totalMin, -1f, 1);
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            this.constraintCount = this.constraintCount;
        }

        private void SetCellsAlongAxis(int axis)
        {
            if (axis == 0)
            {
                for (int i = 0; i < base.rectChildren.Count; i++)
                {
                    RectTransform rectTransform = base.rectChildren[i];
                    this.m_Tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDelta | DrivenTransformProperties.Anchors | DrivenTransformProperties.AnchoredPosition);
                    rectTransform.anchorMin = Vector2.up;
                    rectTransform.anchorMax = Vector2.up;
                    rectTransform.sizeDelta = this.cellSize;
                }
            }
            else
            {
                int num8;
                int num9;
                int num10;
                float x = base.rectTransform.rect.size.x;
                float y = base.rectTransform.rect.size.y;
                int constraintCount = 1;
                int num5 = 1;
                if (this.m_Constraint == Constraint.FixedColumnCount)
                {
                    constraintCount = this.m_ConstraintCount;
                    num5 = Mathf.CeilToInt((((float) base.rectChildren.Count) / ((float) constraintCount)) - 0.001f);
                }
                else if (this.m_Constraint == Constraint.FixedRowCount)
                {
                    num5 = this.m_ConstraintCount;
                    constraintCount = Mathf.CeilToInt((((float) base.rectChildren.Count) / ((float) num5)) - 0.001f);
                }
                else
                {
                    if ((this.cellSize.x + this.spacing.x) <= 0f)
                    {
                        constraintCount = 0x7fffffff;
                    }
                    else
                    {
                        constraintCount = Mathf.Max(1, Mathf.FloorToInt((((x - base.padding.horizontal) + this.spacing.x) + 0.001f) / (this.cellSize.x + this.spacing.x)));
                    }
                    if ((this.cellSize.y + this.spacing.y) <= 0f)
                    {
                        num5 = 0x7fffffff;
                    }
                    else
                    {
                        num5 = Mathf.Max(1, Mathf.FloorToInt((((y - base.padding.vertical) + this.spacing.y) + 0.001f) / (this.cellSize.y + this.spacing.y)));
                    }
                }
                int num6 = (int) (this.startCorner % Corner.LowerLeft);
                int num7 = (int) (this.startCorner / Corner.LowerLeft);
                if (this.startAxis == Axis.Horizontal)
                {
                    num8 = constraintCount;
                    num9 = Mathf.Clamp(constraintCount, 1, base.rectChildren.Count);
                    num10 = Mathf.Clamp(num5, 1, Mathf.CeilToInt(((float) base.rectChildren.Count) / ((float) num8)));
                }
                else
                {
                    num8 = num5;
                    num10 = Mathf.Clamp(num5, 1, base.rectChildren.Count);
                    num9 = Mathf.Clamp(constraintCount, 1, Mathf.CeilToInt(((float) base.rectChildren.Count) / ((float) num8)));
                }
                Vector2 vector = new Vector2((num9 * this.cellSize.x) + ((num9 - 1) * this.spacing.x), (num10 * this.cellSize.y) + ((num10 - 1) * this.spacing.y));
                Vector2 vector2 = new Vector2(base.GetStartOffset(0, vector.x), base.GetStartOffset(1, vector.y));
                for (int j = 0; j < base.rectChildren.Count; j++)
                {
                    int num12;
                    int num13;
                    if (this.startAxis == Axis.Horizontal)
                    {
                        num12 = j % num8;
                        num13 = j / num8;
                    }
                    else
                    {
                        num12 = j / num8;
                        num13 = j % num8;
                    }
                    if (num6 == 1)
                    {
                        num12 = (num9 - 1) - num12;
                    }
                    if (num7 == 1)
                    {
                        num13 = (num10 - 1) - num13;
                    }
                    base.SetChildAlongAxis(base.rectChildren[j], 0, vector2.x + ((this.cellSize[0] + this.spacing[0]) * num12), this.cellSize[0]);
                    base.SetChildAlongAxis(base.rectChildren[j], 1, vector2.y + ((this.cellSize[1] + this.spacing[1]) * num13), this.cellSize[1]);
                }
            }
        }

        public override void SetLayoutHorizontal()
        {
            this.SetCellsAlongAxis(0);
        }

        public override void SetLayoutVertical()
        {
            this.SetCellsAlongAxis(1);
        }

        public Vector2 cellSize
        {
            get
            {
                return this.m_CellSize;
            }
            set
            {
                base.SetProperty<Vector2>(ref this.m_CellSize, value);
            }
        }

        public Constraint constraint
        {
            get
            {
                return this.m_Constraint;
            }
            set
            {
                base.SetProperty<Constraint>(ref this.m_Constraint, value);
            }
        }

        public int constraintCount
        {
            get
            {
                return this.m_ConstraintCount;
            }
            set
            {
                base.SetProperty<int>(ref this.m_ConstraintCount, Mathf.Max(1, value));
            }
        }

        public Vector2 spacing
        {
            get
            {
                return this.m_Spacing;
            }
            set
            {
                base.SetProperty<Vector2>(ref this.m_Spacing, value);
            }
        }

        public Axis startAxis
        {
            get
            {
                return this.m_StartAxis;
            }
            set
            {
                base.SetProperty<Axis>(ref this.m_StartAxis, value);
            }
        }

        public Corner startCorner
        {
            get
            {
                return this.m_StartCorner;
            }
            set
            {
                base.SetProperty<Corner>(ref this.m_StartCorner, value);
            }
        }

        public enum Axis
        {
            Horizontal,
            Vertical
        }

        public enum Constraint
        {
            Flexible,
            FixedColumnCount,
            FixedRowCount
        }

        public enum Corner
        {
            UpperLeft,
            UpperRight,
            LowerLeft,
            LowerRight
        }
    }
}

