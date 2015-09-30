namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [Serializable]
    internal class SpriteRectCache : ScriptableObject
    {
        [SerializeField]
        public List<SpriteRect> m_Rects;

        public void AddRect(SpriteRect r)
        {
            if (this.m_Rects != null)
            {
                this.m_Rects.Add(r);
            }
        }

        public void ClearAll()
        {
            if (this.m_Rects != null)
            {
                this.m_Rects.Clear();
            }
        }

        public bool Contains(SpriteRect spriteRect)
        {
            return ((this.m_Rects != null) && this.m_Rects.Contains(spriteRect));
        }

        public int GetIndex(SpriteRect spriteRect)
        {
            <GetIndex>c__AnonStorey9E storeye = new <GetIndex>c__AnonStorey9E {
                spriteRect = spriteRect
            };
            if (this.m_Rects != null)
            {
                return this.m_Rects.FindIndex(new Predicate<SpriteRect>(storeye.<>m__1D9));
            }
            return 0;
        }

        private void OnEnable()
        {
            if (this.m_Rects == null)
            {
                this.m_Rects = new List<SpriteRect>();
            }
        }

        public SpriteRect RectAt(int i)
        {
            if (i >= this.Count)
            {
                return null;
            }
            return this.m_Rects[i];
        }

        public void RemoveRect(SpriteRect r)
        {
            if (this.m_Rects != null)
            {
                this.m_Rects.Remove(r);
            }
        }

        public int Count
        {
            get
            {
                if (this.m_Rects != null)
                {
                    return this.m_Rects.Count;
                }
                return 0;
            }
        }

        [CompilerGenerated]
        private sealed class <GetIndex>c__AnonStorey9E
        {
            internal SpriteRect spriteRect;

            internal bool <>m__1D9(SpriteRect p)
            {
                return p.Equals(this.spriteRect);
            }
        }
    }
}

