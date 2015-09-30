namespace UnityEngine.UI
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class MaskUtilities
    {
        public static Transform FindRootSortOverrideCanvas(Transform start)
        {
            Transform parent = start;
            Transform transform2 = null;
            while (parent != null)
            {
                Canvas component = parent.GetComponent<Canvas>();
                if ((component != null) && component.overrideSorting)
                {
                    return parent;
                }
                if (component != null)
                {
                    transform2 = parent;
                }
                parent = parent.parent;
            }
            return transform2;
        }

        public static RectMask2D GetRectMaskForClippable(IClippable transform)
        {
            Transform parent = transform.rectTransform.parent;
            List<Component> results = ListPool<Component>.Get();
            while (parent != null)
            {
                parent.GetComponents(typeof(RectMask2D), results);
                for (int i = 0; i < results.Count; i++)
                {
                    if ((results[i] != null) && ((RectMask2D) results[i]).IsActive())
                    {
                        RectMask2D maskd = (RectMask2D) results[i];
                        ListPool<Component>.Release(results);
                        return maskd;
                    }
                }
                if (parent.GetComponent<Canvas>() != null)
                {
                    break;
                }
                parent = parent.parent;
            }
            ListPool<Component>.Release(results);
            return null;
        }

        public static void GetRectMasksForClip(RectMask2D clipper, List<RectMask2D> masks)
        {
            masks.Clear();
            Transform parent = clipper.transform;
            List<Component> results = ListPool<Component>.Get();
            while (parent != null)
            {
                parent.GetComponents(typeof(RectMask2D), results);
                for (int i = 0; i < results.Count; i++)
                {
                    if ((results[i] != null) && ((RectMask2D) results[i]).IsActive())
                    {
                        masks.Add((RectMask2D) results[i]);
                    }
                }
                if (parent.GetComponent<Canvas>() != null)
                {
                    break;
                }
                parent = parent.parent;
            }
            ListPool<Component>.Release(results);
        }

        public static int GetStencilDepth(Transform transform, Transform stopAfter)
        {
            int num = 0;
            if (transform != stopAfter)
            {
                Transform parent = transform.parent;
                List<Component> results = ListPool<Component>.Get();
                while (parent != null)
                {
                    parent.GetComponents(typeof(Mask), results);
                    for (int i = 0; i < results.Count; i++)
                    {
                        if (((results[i] != null) && ((Mask) results[i]).IsActive()) && ((Mask) results[i]).graphic.IsActive())
                        {
                            num++;
                            break;
                        }
                    }
                    if (parent == stopAfter)
                    {
                        break;
                    }
                    parent = parent.parent;
                }
                ListPool<Component>.Release(results);
            }
            return num;
        }

        public static void Notify2DMaskStateChanged(Component mask)
        {
            List<Component> results = ListPool<Component>.Get();
            mask.GetComponentsInChildren<Component>(results);
            for (int i = 0; i < results.Count; i++)
            {
                if ((results[i] != null) && (results[i].gameObject != mask.gameObject))
                {
                    IClippable clippable = results[i] as IClippable;
                    if (clippable != null)
                    {
                        clippable.RecalculateClipping();
                    }
                }
            }
            ListPool<Component>.Release(results);
        }

        public static void NotifyStencilStateChanged(Component mask)
        {
            List<Component> results = ListPool<Component>.Get();
            mask.GetComponentsInChildren<Component>(results);
            for (int i = 0; i < results.Count; i++)
            {
                if ((results[i] != null) && (results[i].gameObject != mask.gameObject))
                {
                    IMaskable maskable = results[i] as IMaskable;
                    if (maskable != null)
                    {
                        maskable.RecalculateMasking();
                    }
                }
            }
            ListPool<Component>.Release(results);
        }
    }
}

