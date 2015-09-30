namespace UnityEngine.UI
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI.Collections;

    public class GraphicRegistry
    {
        private readonly Dictionary<Canvas, IndexedSet<Graphic>> m_Graphics = new Dictionary<Canvas, IndexedSet<Graphic>>();
        private static readonly List<Graphic> s_EmptyList = new List<Graphic>();
        private static GraphicRegistry s_Instance;

        protected GraphicRegistry()
        {
        }

        public static IList<Graphic> GetGraphicsForCanvas(Canvas canvas)
        {
            IndexedSet<Graphic> set;
            if (instance.m_Graphics.TryGetValue(canvas, out set))
            {
                return set;
            }
            return s_EmptyList;
        }

        public static void RegisterGraphicForCanvas(Canvas c, Graphic graphic)
        {
            if (c != null)
            {
                IndexedSet<Graphic> set;
                instance.m_Graphics.TryGetValue(c, out set);
                if (set != null)
                {
                    set.Add(graphic);
                }
                else
                {
                    set = new IndexedSet<Graphic> {
                        graphic
                    };
                    instance.m_Graphics.Add(c, set);
                }
            }
        }

        public static void UnregisterGraphicForCanvas(Canvas c, Graphic graphic)
        {
            IndexedSet<Graphic> set;
            if ((c != null) && instance.m_Graphics.TryGetValue(c, out set))
            {
                set.Remove(graphic);
            }
        }

        public static GraphicRegistry instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new GraphicRegistry();
                }
                return s_Instance;
            }
        }
    }
}

