namespace UnityEngine.UI
{
    using System;

    public interface ILayoutElement
    {
        void CalculateLayoutInputHorizontal();
        void CalculateLayoutInputVertical();

        float flexibleHeight { get; }

        float flexibleWidth { get; }

        int layoutPriority { get; }

        float minHeight { get; }

        float minWidth { get; }

        float preferredHeight { get; }

        float preferredWidth { get; }
    }
}

