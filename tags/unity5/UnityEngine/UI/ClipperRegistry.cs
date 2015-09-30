namespace UnityEngine.UI
{
    using System;
    using UnityEngine.UI.Collections;

    public class ClipperRegistry
    {
        private readonly IndexedSet<IClipper> m_Clippers = new IndexedSet<IClipper>();
        private static ClipperRegistry s_Instance;

        protected ClipperRegistry()
        {
        }

        public void Cull()
        {
            for (int i = 0; i < this.m_Clippers.Count; i++)
            {
                this.m_Clippers[i].PerformClipping();
            }
        }

        public static void Register(IClipper c)
        {
            if (c != null)
            {
                instance.m_Clippers.Add(c);
            }
        }

        public static void Unregister(IClipper c)
        {
            instance.m_Clippers.Remove(c);
        }

        public static ClipperRegistry instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new ClipperRegistry();
                }
                return s_Instance;
            }
        }
    }
}

