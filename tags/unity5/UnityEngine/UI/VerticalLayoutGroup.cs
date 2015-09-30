namespace UnityEngine.UI
{
    using System;
    using UnityEngine;

    [AddComponentMenu("Layout/Vertical Layout Group", 0x97)]
    public class VerticalLayoutGroup : HorizontalOrVerticalLayoutGroup
    {
        protected VerticalLayoutGroup()
        {
        }

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            base.CalcAlongAxis(0, true);
        }

        public override void CalculateLayoutInputVertical()
        {
            base.CalcAlongAxis(1, true);
        }

        public override void SetLayoutHorizontal()
        {
            base.SetChildrenAlongAxis(0, true);
        }

        public override void SetLayoutVertical()
        {
            base.SetChildrenAlongAxis(1, true);
        }
    }
}

