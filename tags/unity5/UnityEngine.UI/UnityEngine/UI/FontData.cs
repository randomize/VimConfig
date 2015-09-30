namespace UnityEngine.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.Serialization;

    [Serializable]
    public class FontData : ISerializationCallbackReceiver
    {
        [FormerlySerializedAs("alignment"), SerializeField]
        private TextAnchor m_Alignment;
        [SerializeField]
        private bool m_BestFit;
        [SerializeField, FormerlySerializedAs("font")]
        private Font m_Font;
        [FormerlySerializedAs("fontSize"), SerializeField]
        private int m_FontSize;
        [SerializeField, FormerlySerializedAs("fontStyle")]
        private FontStyle m_FontStyle;
        [SerializeField]
        private HorizontalWrapMode m_HorizontalOverflow;
        [SerializeField]
        private float m_LineSpacing;
        [SerializeField]
        private int m_MaxSize;
        [SerializeField]
        private int m_MinSize;
        [FormerlySerializedAs("richText"), SerializeField]
        private bool m_RichText;
        [SerializeField]
        private VerticalWrapMode m_VerticalOverflow;

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            this.m_FontSize = Mathf.Clamp(this.m_FontSize, 0, 300);
            this.m_MinSize = Mathf.Clamp(this.m_MinSize, 0, 300);
            this.m_MaxSize = Mathf.Clamp(this.m_MaxSize, 0, 300);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        public TextAnchor alignment
        {
            get
            {
                return this.m_Alignment;
            }
            set
            {
                this.m_Alignment = value;
            }
        }

        public bool bestFit
        {
            get
            {
                return this.m_BestFit;
            }
            set
            {
                this.m_BestFit = value;
            }
        }

        public static FontData defaultFontData
        {
            get
            {
                return new FontData { m_FontSize = 14, m_LineSpacing = 1f, m_FontStyle = FontStyle.Normal, m_BestFit = false, m_MinSize = 10, m_MaxSize = 40, m_Alignment = TextAnchor.UpperLeft, m_HorizontalOverflow = HorizontalWrapMode.Wrap, m_VerticalOverflow = VerticalWrapMode.Truncate, m_RichText = true };
            }
        }

        public Font font
        {
            get
            {
                return this.m_Font;
            }
            set
            {
                this.m_Font = value;
            }
        }

        public int fontSize
        {
            get
            {
                return this.m_FontSize;
            }
            set
            {
                this.m_FontSize = value;
            }
        }

        public FontStyle fontStyle
        {
            get
            {
                return this.m_FontStyle;
            }
            set
            {
                this.m_FontStyle = value;
            }
        }

        public HorizontalWrapMode horizontalOverflow
        {
            get
            {
                return this.m_HorizontalOverflow;
            }
            set
            {
                this.m_HorizontalOverflow = value;
            }
        }

        public float lineSpacing
        {
            get
            {
                return this.m_LineSpacing;
            }
            set
            {
                this.m_LineSpacing = value;
            }
        }

        public int maxSize
        {
            get
            {
                return this.m_MaxSize;
            }
            set
            {
                this.m_MaxSize = value;
            }
        }

        public int minSize
        {
            get
            {
                return this.m_MinSize;
            }
            set
            {
                this.m_MinSize = value;
            }
        }

        public bool richText
        {
            get
            {
                return this.m_RichText;
            }
            set
            {
                this.m_RichText = value;
            }
        }

        public VerticalWrapMode verticalOverflow
        {
            get
            {
                return this.m_VerticalOverflow;
            }
            set
            {
                this.m_VerticalOverflow = value;
            }
        }
    }
}

