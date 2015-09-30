namespace UnityEngine.UI
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Serialization;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct ColorBlock
    {
        [FormerlySerializedAs("normalColor"), SerializeField]
        private Color m_NormalColor;
        [SerializeField, FormerlySerializedAs("m_SelectedColor"), FormerlySerializedAs("highlightedColor")]
        private Color m_HighlightedColor;
        [SerializeField, FormerlySerializedAs("pressedColor")]
        private Color m_PressedColor;
        [FormerlySerializedAs("disabledColor"), SerializeField]
        private Color m_DisabledColor;
        [Range(1f, 5f), SerializeField]
        private float m_ColorMultiplier;
        [SerializeField, FormerlySerializedAs("fadeDuration")]
        private float m_FadeDuration;
        public Color normalColor
        {
            get
            {
                return this.m_NormalColor;
            }
            set
            {
                this.m_NormalColor = value;
            }
        }
        public Color highlightedColor
        {
            get
            {
                return this.m_HighlightedColor;
            }
            set
            {
                this.m_HighlightedColor = value;
            }
        }
        public Color pressedColor
        {
            get
            {
                return this.m_PressedColor;
            }
            set
            {
                this.m_PressedColor = value;
            }
        }
        public Color disabledColor
        {
            get
            {
                return this.m_DisabledColor;
            }
            set
            {
                this.m_DisabledColor = value;
            }
        }
        public float colorMultiplier
        {
            get
            {
                return this.m_ColorMultiplier;
            }
            set
            {
                this.m_ColorMultiplier = value;
            }
        }
        public float fadeDuration
        {
            get
            {
                return this.m_FadeDuration;
            }
            set
            {
                this.m_FadeDuration = value;
            }
        }
        public static ColorBlock defaultColorBlock
        {
            get
            {
                return new ColorBlock { m_NormalColor = (Color) new Color32(0xff, 0xff, 0xff, 0xff), m_HighlightedColor = (Color) new Color32(0xf5, 0xf5, 0xf5, 0xff), m_PressedColor = (Color) new Color32(200, 200, 200, 0xff), m_DisabledColor = (Color) new Color32(200, 200, 200, 0x80), colorMultiplier = 1f, fadeDuration = 0.1f };
            }
        }
    }
}

