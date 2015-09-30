namespace UnityEngine.UI
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public static class Clipping
    {
        public static Rect FindCullAndClipWorldRect(List<RectMask2D> rectMaskParents, out bool validRect)
        {
            if (rectMaskParents.Count == 0)
            {
                validRect = false;
                return new Rect();
            }
            Rect canvasRect = rectMaskParents[0].canvasRect;
            for (int i = 0; i < rectMaskParents.Count; i++)
            {
                canvasRect = RectIntersect(canvasRect, rectMaskParents[i].canvasRect);
            }
            if ((canvasRect.width <= 0f) || (canvasRect.height <= 0f))
            {
                validRect = false;
                return new Rect();
            }
            Vector3 vector = new Vector3(canvasRect.x, canvasRect.y, 0f);
            Vector3 vector2 = new Vector3(canvasRect.x + canvasRect.width, canvasRect.y + canvasRect.height, 0f);
            validRect = true;
            return new Rect(vector.x, vector.y, vector2.x - vector.x, vector2.y - vector.y);
        }

        private static Rect RectIntersect(Rect a, Rect b)
        {
            float x = Mathf.Max(a.x, b.x);
            float num2 = Mathf.Min((float) (a.x + a.width), (float) (b.x + b.width));
            float y = Mathf.Max(a.y, b.y);
            float num4 = Mathf.Min((float) (a.y + a.height), (float) (b.y + b.height));
            if ((num2 >= x) && (num4 >= y))
            {
                return new Rect(x, y, num2 - x, num4 - y);
            }
            return new Rect(0f, 0f, 0f, 0f);
        }
    }
}

