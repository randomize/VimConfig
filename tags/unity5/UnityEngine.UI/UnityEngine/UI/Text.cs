namespace UnityEngine.UI
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [AddComponentMenu("UI/Text", 10)]
    public class Text : MaskableGraphic, ILayoutElement
    {
        [NonSerialized]
        protected bool m_DisableFontTextureRebuiltCallback;
        [SerializeField]
        private FontData m_FontData = FontData.defaultFontData;
        private readonly UIVertex[] m_TempVerts = new UIVertex[4];
        [TextArea(3, 10), SerializeField]
        protected string m_Text = string.Empty;
        private TextGenerator m_TextCache;
        private TextGenerator m_TextCacheForLayout;
        protected static Material s_DefaultText;

        protected Text()
        {
        }

        public virtual void CalculateLayoutInputHorizontal()
        {
        }

        public virtual void CalculateLayoutInputVertical()
        {
        }

        public void FontTextureChanged()
        {
            if (this == null)
            {
                FontUpdateTracker.UntrackText(this);
            }
            else if (!this.m_DisableFontTextureRebuiltCallback)
            {
                this.cachedTextGenerator.Invalidate();
                if (this.IsActive())
                {
                    if (CanvasUpdateRegistry.IsRebuildingGraphics() || CanvasUpdateRegistry.IsRebuildingLayout())
                    {
                        this.UpdateGeometry();
                    }
                    else
                    {
                        this.SetAllDirty();
                    }
                }
            }
        }

        public TextGenerationSettings GetGenerationSettings(Vector2 extents)
        {
            TextGenerationSettings settings = new TextGenerationSettings {
                generationExtents = extents
            };
            if ((this.font != null) && this.font.dynamic)
            {
                settings.fontSize = this.m_FontData.fontSize;
                settings.resizeTextMinSize = this.m_FontData.minSize;
                settings.resizeTextMaxSize = this.m_FontData.maxSize;
            }
            settings.textAnchor = this.m_FontData.alignment;
            settings.scaleFactor = this.pixelsPerUnit;
            settings.color = base.color;
            settings.font = this.font;
            settings.pivot = base.rectTransform.pivot;
            settings.richText = this.m_FontData.richText;
            settings.lineSpacing = this.m_FontData.lineSpacing;
            settings.fontStyle = this.m_FontData.fontStyle;
            settings.resizeTextForBestFit = this.m_FontData.bestFit;
            settings.updateBounds = false;
            settings.horizontalOverflow = this.m_FontData.horizontalOverflow;
            settings.verticalOverflow = this.m_FontData.verticalOverflow;
            return settings;
        }

        public static Vector2 GetTextAnchorPivot(TextAnchor anchor)
        {
            switch (anchor)
            {
                case TextAnchor.UpperLeft:
                    return new Vector2(0f, 1f);

                case TextAnchor.UpperCenter:
                    return new Vector2(0.5f, 1f);

                case TextAnchor.UpperRight:
                    return new Vector2(1f, 1f);

                case TextAnchor.MiddleLeft:
                    return new Vector2(0f, 0.5f);

                case TextAnchor.MiddleCenter:
                    return new Vector2(0.5f, 0.5f);

                case TextAnchor.MiddleRight:
                    return new Vector2(1f, 0.5f);

                case TextAnchor.LowerLeft:
                    return new Vector2(0f, 0f);

                case TextAnchor.LowerCenter:
                    return new Vector2(0.5f, 0f);

                case TextAnchor.LowerRight:
                    return new Vector2(1f, 0f);
            }
            return Vector2.zero;
        }

        protected override void OnDisable()
        {
            FontUpdateTracker.UntrackText(this);
            base.OnDisable();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.cachedTextGenerator.Invalidate();
            FontUpdateTracker.TrackText(this);
        }

        protected override void OnPopulateMesh(Mesh toFill)
        {
            if (this.font != null)
            {
                this.m_DisableFontTextureRebuiltCallback = true;
                Vector2 size = base.rectTransform.rect.size;
                TextGenerationSettings generationSettings = this.GetGenerationSettings(size);
                this.cachedTextGenerator.Populate(this.text, generationSettings);
                Rect rect = base.rectTransform.rect;
                Vector2 textAnchorPivot = GetTextAnchorPivot(this.m_FontData.alignment);
                Vector2 zero = Vector2.zero;
                zero.x = (textAnchorPivot.x != 1f) ? rect.xMin : rect.xMax;
                zero.y = (textAnchorPivot.y != 0f) ? rect.yMax : rect.yMin;
                Vector2 vector4 = base.PixelAdjustPoint(zero) - zero;
                IList<UIVertex> verts = this.cachedTextGenerator.verts;
                float num = 1f / this.pixelsPerUnit;
                int num2 = verts.Count - 4;
                using (VertexHelper helper = new VertexHelper())
                {
                    if (vector4 != Vector2.zero)
                    {
                        for (int i = 0; i < num2; i++)
                        {
                            int index = i & 3;
                            this.m_TempVerts[index] = verts[i];
                            this.m_TempVerts[index].position = (Vector3) (this.m_TempVerts[index].position * num);
                            this.m_TempVerts[index].position.x += vector4.x;
                            this.m_TempVerts[index].position.y += vector4.y;
                            if (index == 3)
                            {
                                helper.AddUIVertexQuad(this.m_TempVerts);
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < num2; j++)
                        {
                            int num6 = j & 3;
                            this.m_TempVerts[num6] = verts[j];
                            this.m_TempVerts[num6].position = (Vector3) (this.m_TempVerts[num6].position * num);
                            if (num6 == 3)
                            {
                                helper.AddUIVertexQuad(this.m_TempVerts);
                            }
                        }
                    }
                    helper.FillMesh(toFill);
                }
                this.m_DisableFontTextureRebuiltCallback = false;
            }
        }

        public override void OnRebuildRequested()
        {
            FontUpdateTracker.UntrackText(this);
            FontUpdateTracker.TrackText(this);
            this.cachedTextGenerator.Invalidate();
            base.OnRebuildRequested();
        }

        protected override void Reset()
        {
            this.font = UnityEngine.Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        protected override void UpdateGeometry()
        {
            if (this.font != null)
            {
                base.UpdateGeometry();
            }
        }

        public TextAnchor alignment
        {
            get
            {
                return this.m_FontData.alignment;
            }
            set
            {
                if (this.m_FontData.alignment != value)
                {
                    this.m_FontData.alignment = value;
                    this.SetVerticesDirty();
                    this.SetLayoutDirty();
                }
            }
        }

        public TextGenerator cachedTextGenerator
        {
            get
            {
                if (this.m_TextCache == null)
                {
                }
                return (this.m_TextCache = (this.m_Text.Length == 0) ? new TextGenerator() : new TextGenerator(this.m_Text.Length));
            }
        }

        public TextGenerator cachedTextGeneratorForLayout
        {
            get
            {
                if (this.m_TextCacheForLayout == null)
                {
                }
                return (this.m_TextCacheForLayout = new TextGenerator());
            }
        }

        public virtual float flexibleHeight
        {
            get
            {
                return -1f;
            }
        }

        public virtual float flexibleWidth
        {
            get
            {
                return -1f;
            }
        }

        public Font font
        {
            get
            {
                return this.m_FontData.font;
            }
            set
            {
                if (this.m_FontData.font != value)
                {
                    FontUpdateTracker.UntrackText(this);
                    this.m_FontData.font = value;
                    FontUpdateTracker.TrackText(this);
                    this.SetAllDirty();
                }
            }
        }

        public int fontSize
        {
            get
            {
                return this.m_FontData.fontSize;
            }
            set
            {
                if (this.m_FontData.fontSize != value)
                {
                    this.m_FontData.fontSize = value;
                    this.SetVerticesDirty();
                    this.SetLayoutDirty();
                }
            }
        }

        public FontStyle fontStyle
        {
            get
            {
                return this.m_FontData.fontStyle;
            }
            set
            {
                if (this.m_FontData.fontStyle != value)
                {
                    this.m_FontData.fontStyle = value;
                    this.SetVerticesDirty();
                    this.SetLayoutDirty();
                }
            }
        }

        public HorizontalWrapMode horizontalOverflow
        {
            get
            {
                return this.m_FontData.horizontalOverflow;
            }
            set
            {
                if (this.m_FontData.horizontalOverflow != value)
                {
                    this.m_FontData.horizontalOverflow = value;
                    this.SetVerticesDirty();
                    this.SetLayoutDirty();
                }
            }
        }

        public virtual int layoutPriority
        {
            get
            {
                return 0;
            }
        }

        public float lineSpacing
        {
            get
            {
                return this.m_FontData.lineSpacing;
            }
            set
            {
                if (this.m_FontData.lineSpacing != value)
                {
                    this.m_FontData.lineSpacing = value;
                    this.SetVerticesDirty();
                    this.SetLayoutDirty();
                }
            }
        }

        public override Texture mainTexture
        {
            get
            {
                if (((this.font != null) && (this.font.material != null)) && (this.font.material.mainTexture != null))
                {
                    return this.font.material.mainTexture;
                }
                if (base.m_Material != null)
                {
                    return base.m_Material.mainTexture;
                }
                return base.mainTexture;
            }
        }

        public virtual float minHeight
        {
            get
            {
                return 0f;
            }
        }

        public virtual float minWidth
        {
            get
            {
                return 0f;
            }
        }

        public float pixelsPerUnit
        {
            get
            {
                Canvas canvas = base.canvas;
                if (canvas != null)
                {
                    if ((this.font == null) || this.font.dynamic)
                    {
                        return canvas.scaleFactor;
                    }
                    if ((this.m_FontData.fontSize > 0) && (this.font.fontSize > 0))
                    {
                        return (((float) this.font.fontSize) / ((float) this.m_FontData.fontSize));
                    }
                }
                return 1f;
            }
        }

        public virtual float preferredHeight
        {
            get
            {
                TextGenerationSettings generationSettings = this.GetGenerationSettings(new Vector2(base.rectTransform.rect.size.x, 0f));
                return (this.cachedTextGeneratorForLayout.GetPreferredHeight(this.m_Text, generationSettings) / this.pixelsPerUnit);
            }
        }

        public virtual float preferredWidth
        {
            get
            {
                TextGenerationSettings generationSettings = this.GetGenerationSettings(Vector2.zero);
                return (this.cachedTextGeneratorForLayout.GetPreferredWidth(this.m_Text, generationSettings) / this.pixelsPerUnit);
            }
        }

        public bool resizeTextForBestFit
        {
            get
            {
                return this.m_FontData.bestFit;
            }
            set
            {
                if (this.m_FontData.bestFit != value)
                {
                    this.m_FontData.bestFit = value;
                    this.SetVerticesDirty();
                    this.SetLayoutDirty();
                }
            }
        }

        public int resizeTextMaxSize
        {
            get
            {
                return this.m_FontData.maxSize;
            }
            set
            {
                if (this.m_FontData.maxSize != value)
                {
                    this.m_FontData.maxSize = value;
                    this.SetVerticesDirty();
                    this.SetLayoutDirty();
                }
            }
        }

        public int resizeTextMinSize
        {
            get
            {
                return this.m_FontData.minSize;
            }
            set
            {
                if (this.m_FontData.minSize != value)
                {
                    this.m_FontData.minSize = value;
                    this.SetVerticesDirty();
                    this.SetLayoutDirty();
                }
            }
        }

        public bool supportRichText
        {
            get
            {
                return this.m_FontData.richText;
            }
            set
            {
                if (this.m_FontData.richText != value)
                {
                    this.m_FontData.richText = value;
                    this.SetVerticesDirty();
                    this.SetLayoutDirty();
                }
            }
        }

        public virtual string text
        {
            get
            {
                return this.m_Text;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (!string.IsNullOrEmpty(this.m_Text))
                    {
                        this.m_Text = string.Empty;
                        this.SetVerticesDirty();
                    }
                }
                else if (this.m_Text != value)
                {
                    this.m_Text = value;
                    this.SetVerticesDirty();
                    this.SetLayoutDirty();
                }
            }
        }

        public VerticalWrapMode verticalOverflow
        {
            get
            {
                return this.m_FontData.verticalOverflow;
            }
            set
            {
                if (this.m_FontData.verticalOverflow != value)
                {
                    this.m_FontData.verticalOverflow = value;
                    this.SetVerticesDirty();
                    this.SetLayoutDirty();
                }
            }
        }
    }
}

