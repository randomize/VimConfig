namespace UnityEngine.UI
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;

    [StructLayout(LayoutKind.Sequential)]
    public struct LayoutRebuilder : ICanvasElement, IEquatable<LayoutRebuilder>
    {
        private readonly RectTransform m_ToRebuild;
        private readonly int m_CachedHashFromTransform;
        [CompilerGenerated]
        private static UnityAction<Component> <>f__am$cache2;
        [CompilerGenerated]
        private static UnityAction<Component> <>f__am$cache3;
        [CompilerGenerated]
        private static UnityAction<Component> <>f__am$cache4;
        [CompilerGenerated]
        private static UnityAction<Component> <>f__am$cache5;
        [CompilerGenerated]
        private static Predicate<Component> <>f__am$cache6;
        private LayoutRebuilder(RectTransform controller)
        {
            this.m_ToRebuild = controller;
            this.m_CachedHashFromTransform = this.m_ToRebuild.GetHashCode();
        }

        static LayoutRebuilder()
        {
            RectTransform.reapplyDrivenProperties += new UnityEngine.RectTransform.ReapplyDrivenProperties(LayoutRebuilder.ReapplyDrivenProperties);
        }

        void ICanvasElement.Rebuild(CanvasUpdate executing)
        {
            if (executing == CanvasUpdate.Layout)
            {
                if (<>f__am$cache2 == null)
                {
                    <>f__am$cache2 = new UnityAction<Component>(LayoutRebuilder.<Rebuild>m__8);
                }
                this.PerformLayoutCalculation(this.m_ToRebuild, <>f__am$cache2);
                if (<>f__am$cache3 == null)
                {
                    <>f__am$cache3 = new UnityAction<Component>(LayoutRebuilder.<Rebuild>m__9);
                }
                this.PerformLayoutControl(this.m_ToRebuild, <>f__am$cache3);
                if (<>f__am$cache4 == null)
                {
                    <>f__am$cache4 = new UnityAction<Component>(LayoutRebuilder.<Rebuild>m__A);
                }
                this.PerformLayoutCalculation(this.m_ToRebuild, <>f__am$cache4);
                if (<>f__am$cache5 == null)
                {
                    <>f__am$cache5 = new UnityAction<Component>(LayoutRebuilder.<Rebuild>m__B);
                }
                this.PerformLayoutControl(this.m_ToRebuild, <>f__am$cache5);
            }
        }

        private static void ReapplyDrivenProperties(RectTransform driven)
        {
            MarkLayoutForRebuild(driven);
        }

        public Transform transform
        {
            get
            {
                return this.m_ToRebuild;
            }
        }
        public bool IsDestroyed()
        {
            return (this.m_ToRebuild == null);
        }

        private static void StripDisabledBehavioursFromList(List<Component> components)
        {
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = e => (e is Behaviour) && !((Behaviour) e).isActiveAndEnabled;
            }
            components.RemoveAll(<>f__am$cache6);
        }

        public static void ForceRebuildLayoutImmediate(RectTransform layoutRoot)
        {
            LayoutRebuilder rebuilder = new LayoutRebuilder(layoutRoot);
            ((ICanvasElement) rebuilder).Rebuild(CanvasUpdate.Layout);
        }

        private void PerformLayoutControl(RectTransform rect, UnityAction<Component> action)
        {
            if (rect != null)
            {
                List<Component> results = ListPool<Component>.Get();
                rect.GetComponents(typeof(ILayoutController), results);
                StripDisabledBehavioursFromList(results);
                if (results.Count > 0)
                {
                    for (int i = 0; i < results.Count; i++)
                    {
                        if (results[i] is ILayoutSelfController)
                        {
                            action(results[i]);
                        }
                    }
                    for (int j = 0; j < results.Count; j++)
                    {
                        if (!(results[j] is ILayoutSelfController))
                        {
                            action(results[j]);
                        }
                    }
                    for (int k = 0; k < rect.childCount; k++)
                    {
                        this.PerformLayoutControl(rect.GetChild(k) as RectTransform, action);
                    }
                }
                ListPool<Component>.Release(results);
            }
        }

        private void PerformLayoutCalculation(RectTransform rect, UnityAction<Component> action)
        {
            if (rect != null)
            {
                List<Component> results = ListPool<Component>.Get();
                rect.GetComponents(typeof(ILayoutElement), results);
                StripDisabledBehavioursFromList(results);
                if (results.Count > 0)
                {
                    for (int i = 0; i < rect.childCount; i++)
                    {
                        this.PerformLayoutCalculation(rect.GetChild(i) as RectTransform, action);
                    }
                    for (int j = 0; j < results.Count; j++)
                    {
                        action(results[j]);
                    }
                }
                ListPool<Component>.Release(results);
            }
        }

        public static void MarkLayoutForRebuild(RectTransform rect)
        {
            if (rect != null)
            {
                RectTransform layoutRoot = rect;
                while (true)
                {
                    RectTransform parent = layoutRoot.parent as RectTransform;
                    if (!ValidLayoutGroup(parent))
                    {
                        break;
                    }
                    layoutRoot = parent;
                }
                if ((layoutRoot != rect) || ValidController(layoutRoot))
                {
                    MarkLayoutRootForRebuild(layoutRoot);
                }
            }
        }

        private static bool ValidLayoutGroup(RectTransform parent)
        {
            if (parent == null)
            {
                return false;
            }
            List<Component> results = ListPool<Component>.Get();
            parent.GetComponents(typeof(ILayoutGroup), results);
            StripDisabledBehavioursFromList(results);
            bool flag = results.Count > 0;
            ListPool<Component>.Release(results);
            return flag;
        }

        private static bool ValidController(RectTransform layoutRoot)
        {
            if (layoutRoot == null)
            {
                return false;
            }
            List<Component> results = ListPool<Component>.Get();
            layoutRoot.GetComponents(typeof(ILayoutController), results);
            StripDisabledBehavioursFromList(results);
            bool flag = results.Count > 0;
            ListPool<Component>.Release(results);
            return flag;
        }

        private static void MarkLayoutRootForRebuild(RectTransform controller)
        {
            if (controller != null)
            {
                CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(new LayoutRebuilder(controller));
            }
        }

        public bool Equals(LayoutRebuilder other)
        {
            return (this.m_ToRebuild == other.m_ToRebuild);
        }

        public override int GetHashCode()
        {
            return this.m_CachedHashFromTransform;
        }

        public override string ToString()
        {
            return ("(Layout Rebuilder for) " + this.m_ToRebuild);
        }

        [CompilerGenerated]
        private static void <Rebuild>m__8(Component e)
        {
            (e as ILayoutElement).CalculateLayoutInputHorizontal();
        }

        [CompilerGenerated]
        private static void <Rebuild>m__9(Component e)
        {
            (e as ILayoutController).SetLayoutHorizontal();
        }

        [CompilerGenerated]
        private static void <Rebuild>m__A(Component e)
        {
            (e as ILayoutElement).CalculateLayoutInputVertical();
        }

        [CompilerGenerated]
        private static void <Rebuild>m__B(Component e)
        {
            (e as ILayoutController).SetLayoutVertical();
        }
    }
}

