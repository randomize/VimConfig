namespace UnityEngine.UI
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public static class LayoutUtility
    {
        [CompilerGenerated]
        private static Func<ILayoutElement, float> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<ILayoutElement, float> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<ILayoutElement, float> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<ILayoutElement, float> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<ILayoutElement, float> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<ILayoutElement, float> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<ILayoutElement, float> <>f__am$cache6;
        [CompilerGenerated]
        private static Func<ILayoutElement, float> <>f__am$cache7;

        public static float GetFlexibleHeight(RectTransform rect)
        {
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = e => e.flexibleHeight;
            }
            return GetLayoutProperty(rect, <>f__am$cache7, 0f);
        }

        public static float GetFlexibleSize(RectTransform rect, int axis)
        {
            if (axis == 0)
            {
                return GetFlexibleWidth(rect);
            }
            return GetFlexibleHeight(rect);
        }

        public static float GetFlexibleWidth(RectTransform rect)
        {
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = e => e.flexibleWidth;
            }
            return GetLayoutProperty(rect, <>f__am$cache3, 0f);
        }

        public static float GetLayoutProperty(RectTransform rect, Func<ILayoutElement, float> property, float defaultValue)
        {
            ILayoutElement element;
            return GetLayoutProperty(rect, property, defaultValue, out element);
        }

        public static float GetLayoutProperty(RectTransform rect, Func<ILayoutElement, float> property, float defaultValue, out ILayoutElement source)
        {
            source = null;
            if (rect == null)
            {
                return 0f;
            }
            float num = defaultValue;
            int num2 = -2147483648;
            List<Component> results = ListPool<Component>.Get();
            rect.GetComponents(typeof(ILayoutElement), results);
            for (int i = 0; i < results.Count; i++)
            {
                ILayoutElement arg = results[i] as ILayoutElement;
                if (!(arg is Behaviour) || ((Behaviour) arg).isActiveAndEnabled)
                {
                    int layoutPriority = arg.layoutPriority;
                    if (layoutPriority >= num2)
                    {
                        float num5 = property(arg);
                        if (num5 >= 0f)
                        {
                            if (layoutPriority > num2)
                            {
                                num = num5;
                                num2 = layoutPriority;
                                source = arg;
                            }
                            else if (num5 > num)
                            {
                                num = num5;
                                source = arg;
                            }
                        }
                    }
                }
            }
            ListPool<Component>.Release(results);
            return num;
        }

        public static float GetMinHeight(RectTransform rect)
        {
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = e => e.minHeight;
            }
            return GetLayoutProperty(rect, <>f__am$cache4, 0f);
        }

        public static float GetMinSize(RectTransform rect, int axis)
        {
            if (axis == 0)
            {
                return GetMinWidth(rect);
            }
            return GetMinHeight(rect);
        }

        public static float GetMinWidth(RectTransform rect)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = e => e.minWidth;
            }
            return GetLayoutProperty(rect, <>f__am$cache0, 0f);
        }

        public static float GetPreferredHeight(RectTransform rect)
        {
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = e => e.minHeight;
            }
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = e => e.preferredHeight;
            }
            return Mathf.Max(GetLayoutProperty(rect, <>f__am$cache5, 0f), GetLayoutProperty(rect, <>f__am$cache6, 0f));
        }

        public static float GetPreferredSize(RectTransform rect, int axis)
        {
            if (axis == 0)
            {
                return GetPreferredWidth(rect);
            }
            return GetPreferredHeight(rect);
        }

        public static float GetPreferredWidth(RectTransform rect)
        {
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = e => e.minWidth;
            }
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = e => e.preferredWidth;
            }
            return Mathf.Max(GetLayoutProperty(rect, <>f__am$cache1, 0f), GetLayoutProperty(rect, <>f__am$cache2, 0f));
        }
    }
}

