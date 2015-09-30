namespace UnityEngine.UI
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Serialization;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct SpriteState
    {
        [FormerlySerializedAs("highlightedSprite"), FormerlySerializedAs("m_SelectedSprite"), SerializeField]
        private Sprite m_HighlightedSprite;
        [SerializeField, FormerlySerializedAs("pressedSprite")]
        private Sprite m_PressedSprite;
        [FormerlySerializedAs("disabledSprite"), SerializeField]
        private Sprite m_DisabledSprite;
        public Sprite highlightedSprite
        {
            get
            {
                return this.m_HighlightedSprite;
            }
            set
            {
                this.m_HighlightedSprite = value;
            }
        }
        public Sprite pressedSprite
        {
            get
            {
                return this.m_PressedSprite;
            }
            set
            {
                this.m_PressedSprite = value;
            }
        }
        public Sprite disabledSprite
        {
            get
            {
                return this.m_DisabledSprite;
            }
            set
            {
                this.m_DisabledSprite = value;
            }
        }
    }
}

