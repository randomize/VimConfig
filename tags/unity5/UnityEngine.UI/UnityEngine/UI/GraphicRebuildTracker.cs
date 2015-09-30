namespace UnityEngine.UI
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public static class GraphicRebuildTracker
    {
        private static IList<Graphic> m_Tracked = new IndexedSet<Graphic>();
        private static bool s_Initialized;

        private static void OnRebuildRequested()
        {
            StencilMaterial.ClearAll();
            for (int i = 0; i < m_Tracked.Count; i++)
            {
                m_Tracked[i].OnRebuildRequested();
            }
        }

        public static void TrackGraphic(Graphic g)
        {
            if (!s_Initialized)
            {
                CanvasRenderer.onRequestRebuild += new CanvasRenderer.OnRequestRebuild(GraphicRebuildTracker.OnRebuildRequested);
                s_Initialized = true;
            }
            m_Tracked.Add(g);
        }

        public static void UnTrackGraphic(Graphic g)
        {
            m_Tracked.Remove(g);
        }
    }
}

