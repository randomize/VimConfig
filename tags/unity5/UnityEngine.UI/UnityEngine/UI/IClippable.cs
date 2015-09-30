namespace UnityEngine.UI
{
    using System;
    using UnityEngine;

    public interface IClippable
    {
        void Cull(Rect clipRect, bool validRect);
        void RecalculateClipping();
        void SetClipRect(Rect value, bool validRect);

        RectTransform rectTransform { get; }
    }
}

