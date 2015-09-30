namespace UnityEditor
{
    using System;
    using UnityEngine;

    [Serializable]
    internal class SpriteRect
    {
        [SerializeField]
        public SpriteAlignment m_Alignment;
        [SerializeField]
        public Vector4 m_Border;
        [SerializeField]
        public string m_Name = string.Empty;
        [SerializeField]
        public string m_OriginalName = string.Empty;
        [SerializeField]
        public Vector2 m_Pivot = Vector2.zero;
        [SerializeField]
        public Rect m_Rect;
    }
}

