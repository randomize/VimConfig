namespace UnityEngine.UI
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.UI.Collections;

    public class CanvasUpdateRegistry
    {
        [CompilerGenerated]
        private static Predicate<ICanvasElement> <>f__am$cache6;
        [CompilerGenerated]
        private static Predicate<ICanvasElement> <>f__am$cache7;
        private readonly IndexedSet<ICanvasElement> m_GraphicRebuildQueue = new IndexedSet<ICanvasElement>();
        private readonly IndexedSet<ICanvasElement> m_LayoutRebuildQueue = new IndexedSet<ICanvasElement>();
        private bool m_PerformingGraphicUpdate;
        private bool m_PerformingLayoutUpdate;
        private static CanvasUpdateRegistry s_Instance;
        private static readonly Comparison<ICanvasElement> s_SortLayoutFunction = new Comparison<ICanvasElement>(CanvasUpdateRegistry.SortLayoutList);

        protected CanvasUpdateRegistry()
        {
            Canvas.willRenderCanvases += new Canvas.WillRenderCanvases(this.PerformUpdate);
        }

        private void InternalRegisterCanvasElementForGraphicRebuild(ICanvasElement element)
        {
            if (this.m_PerformingGraphicUpdate)
            {
                Debug.LogError(string.Format("Trying to add {0} for graphic rebuild while we are already inside a graphic rebuild loop. This is not supported.", element));
            }
            else
            {
                this.m_GraphicRebuildQueue.Add(element);
            }
        }

        private void InternalRegisterCanvasElementForLayoutRebuild(ICanvasElement element)
        {
            this.m_LayoutRebuildQueue.Add(element);
        }

        private void InternalUnRegisterCanvasElementForGraphicRebuild(ICanvasElement element)
        {
            if (this.m_PerformingGraphicUpdate)
            {
                Debug.LogError(string.Format("Trying to remove {0} from rebuild list while we are already inside a rebuild loop. This is not supported.", element));
            }
            else
            {
                instance.m_GraphicRebuildQueue.Remove(element);
            }
        }

        private void InternalUnRegisterCanvasElementForLayoutRebuild(ICanvasElement element)
        {
            if (this.m_PerformingLayoutUpdate)
            {
                Debug.LogError(string.Format("Trying to remove {0} from rebuild list while we are already inside a rebuild loop. This is not supported.", element));
            }
            else
            {
                instance.m_LayoutRebuildQueue.Remove(element);
            }
        }

        public static bool IsRebuildingGraphics()
        {
            return instance.m_PerformingGraphicUpdate;
        }

        public static bool IsRebuildingLayout()
        {
            return instance.m_PerformingLayoutUpdate;
        }

        private bool ObjectValidForUpdate(ICanvasElement element)
        {
            bool flag = element != null;
            if (element is UnityEngine.Object)
            {
                flag = element is UnityEngine.Object;
            }
            return flag;
        }

        private static int ParentCount(Transform child)
        {
            if (child == null)
            {
                return 0;
            }
            Transform parent = child.parent;
            int num = 0;
            while (parent != null)
            {
                num++;
                parent = parent.parent;
            }
            return num;
        }

        private void PerformUpdate()
        {
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = x => (x == null) || x.IsDestroyed();
            }
            this.m_LayoutRebuildQueue.RemoveAll(<>f__am$cache6);
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = x => (x == null) || x.IsDestroyed();
            }
            this.m_GraphicRebuildQueue.RemoveAll(<>f__am$cache7);
            this.m_PerformingLayoutUpdate = true;
            this.m_LayoutRebuildQueue.Sort(s_SortLayoutFunction);
            for (int i = 0; i <= 2; i++)
            {
                for (int k = 0; k < this.m_LayoutRebuildQueue.Count; k++)
                {
                    try
                    {
                        if (this.ObjectValidForUpdate(instance.m_LayoutRebuildQueue[k]))
                        {
                            instance.m_LayoutRebuildQueue[k].Rebuild((CanvasUpdate) i);
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.LogException(exception, instance.m_LayoutRebuildQueue[k].transform);
                    }
                }
            }
            instance.m_LayoutRebuildQueue.Clear();
            this.m_PerformingLayoutUpdate = false;
            ClipperRegistry.instance.Cull();
            this.m_PerformingGraphicUpdate = true;
            for (int j = 3; j < 5; j++)
            {
                for (int m = 0; m < instance.m_GraphicRebuildQueue.Count; m++)
                {
                    try
                    {
                        ICanvasElement element = instance.m_GraphicRebuildQueue[m];
                        if (this.ObjectValidForUpdate(element))
                        {
                            element.Rebuild((CanvasUpdate) j);
                        }
                    }
                    catch (Exception exception2)
                    {
                        Debug.LogException(exception2, instance.m_GraphicRebuildQueue[m].transform);
                    }
                }
            }
            instance.m_GraphicRebuildQueue.Clear();
            this.m_PerformingGraphicUpdate = false;
        }

        public static void RegisterCanvasElementForGraphicRebuild(ICanvasElement element)
        {
            instance.InternalRegisterCanvasElementForGraphicRebuild(element);
        }

        public static void RegisterCanvasElementForLayoutRebuild(ICanvasElement element)
        {
            instance.InternalRegisterCanvasElementForLayoutRebuild(element);
        }

        private static int SortLayoutList(ICanvasElement x, ICanvasElement y)
        {
            Transform child = x.transform;
            Transform transform = y.transform;
            return (ParentCount(child) - ParentCount(transform));
        }

        public static void UnRegisterCanvasElementForRebuild(ICanvasElement element)
        {
            instance.InternalUnRegisterCanvasElementForLayoutRebuild(element);
            instance.InternalUnRegisterCanvasElementForGraphicRebuild(element);
        }

        public static CanvasUpdateRegistry instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new CanvasUpdateRegistry();
                }
                return s_Instance;
            }
        }
    }
}

