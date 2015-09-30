namespace UnityEngine.UI
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public static class FontUpdateTracker
    {
        private static Dictionary<Font, List<Text>> m_Tracked = new Dictionary<Font, List<Text>>();

        private static void RebuildForFont(Font f)
        {
            List<Text> list;
            m_Tracked.TryGetValue(f, out list);
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].FontTextureChanged();
                }
            }
        }

        public static void TrackText(Text t)
        {
            if (t.font != null)
            {
                List<Text> list;
                m_Tracked.TryGetValue(t.font, out list);
                if (list == null)
                {
                    list = new List<Text>();
                    m_Tracked.Add(t.font, list);
                    Font.textureRebuilt += new Action<Font>(FontUpdateTracker.RebuildForFont);
                }
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] == t)
                    {
                        return;
                    }
                }
                list.Add(t);
            }
        }

        public static void UntrackText(Text t)
        {
            if (t.font != null)
            {
                List<Text> list;
                m_Tracked.TryGetValue(t.font, out list);
                if (list != null)
                {
                    list.Remove(t);
                    if (list.Count == 0)
                    {
                        m_Tracked.Remove(t.font);
                    }
                }
            }
        }
    }
}

