namespace UnityEngine.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    [ExecuteInEditMode, AddComponentMenu("Layout/Canvas Scaler", 0x65), RequireComponent(typeof(Canvas))]
    public class CanvasScaler : UIBehaviour
    {
        private const float kLogBase = 2f;
        private Canvas m_Canvas;
        [Tooltip("The pixels per inch to use for sprites that have a 'Pixels Per Unit' setting that matches the 'Reference Pixels Per Unit' setting."), SerializeField]
        protected float m_DefaultSpriteDPI = 96f;
        [SerializeField, Tooltip("The amount of pixels per unit to use for dynamically created bitmaps in the UI, such as Text.")]
        protected float m_DynamicPixelsPerUnit = 1f;
        [Tooltip("The DPI to assume if the screen DPI is not known."), SerializeField]
        protected float m_FallbackScreenDPI = 96f;
        [SerializeField, Tooltip("Determines if the scaling is using the width or height as reference, or a mix in between."), Range(0f, 1f)]
        protected float m_MatchWidthOrHeight;
        [SerializeField, Tooltip("The physical unit to specify positions and sizes in.")]
        protected Unit m_PhysicalUnit = Unit.Points;
        [NonSerialized]
        private float m_PrevReferencePixelsPerUnit = 100f;
        [NonSerialized]
        private float m_PrevScaleFactor = 1f;
        [Tooltip("If a sprite has this 'Pixels Per Unit' setting, then one pixel in the sprite will cover one unit in the UI."), SerializeField]
        protected float m_ReferencePixelsPerUnit = 100f;
        [SerializeField, Tooltip("The resolution the UI layout is designed for. If the screen resolution is larger, the UI will be scaled up, and if it's smaller, the UI will be scaled down. This is done in accordance with the Screen Match Mode.")]
        protected Vector2 m_ReferenceResolution = new Vector2(800f, 600f);
        [SerializeField, Tooltip("Scales all UI elements in the Canvas by this factor.")]
        protected float m_ScaleFactor = 1f;
        [Tooltip("A mode used to scale the canvas area if the aspect ratio of the current resolution doesn't fit the reference resolution."), SerializeField]
        protected ScreenMatchMode m_ScreenMatchMode;
        [SerializeField, Tooltip("Determines how UI elements in the Canvas are scaled.")]
        private ScaleMode m_UiScaleMode;

        protected CanvasScaler()
        {
        }

        protected virtual void Handle()
        {
            if ((this.m_Canvas != null) && this.m_Canvas.isRootCanvas)
            {
                if (this.m_Canvas.renderMode == RenderMode.WorldSpace)
                {
                    this.HandleWorldCanvas();
                }
                else
                {
                    switch (this.m_UiScaleMode)
                    {
                        case ScaleMode.ConstantPixelSize:
                            this.HandleConstantPixelSize();
                            break;

                        case ScaleMode.ScaleWithScreenSize:
                            this.HandleScaleWithScreenSize();
                            break;

                        case ScaleMode.ConstantPhysicalSize:
                            this.HandleConstantPhysicalSize();
                            break;
                    }
                }
            }
        }

        protected virtual void HandleConstantPhysicalSize()
        {
            float dpi = Screen.dpi;
            float num2 = (dpi != 0f) ? dpi : this.m_FallbackScreenDPI;
            float num3 = 1f;
            switch (this.m_PhysicalUnit)
            {
                case Unit.Centimeters:
                    num3 = 2.54f;
                    break;

                case Unit.Millimeters:
                    num3 = 25.4f;
                    break;

                case Unit.Inches:
                    num3 = 1f;
                    break;

                case Unit.Points:
                    num3 = 72f;
                    break;

                case Unit.Picas:
                    num3 = 6f;
                    break;
            }
            this.SetScaleFactor(num2 / num3);
            this.SetReferencePixelsPerUnit((this.m_ReferencePixelsPerUnit * num3) / this.m_DefaultSpriteDPI);
        }

        protected virtual void HandleConstantPixelSize()
        {
            this.SetScaleFactor(this.m_ScaleFactor);
            this.SetReferencePixelsPerUnit(this.m_ReferencePixelsPerUnit);
        }

        protected virtual void HandleScaleWithScreenSize()
        {
            Vector2 vector = new Vector2((float) Screen.width, (float) Screen.height);
            float scaleFactor = 0f;
            switch (this.m_ScreenMatchMode)
            {
                case ScreenMatchMode.MatchWidthOrHeight:
                {
                    float a = Mathf.Log(vector.x / this.m_ReferenceResolution.x, 2f);
                    float b = Mathf.Log(vector.y / this.m_ReferenceResolution.y, 2f);
                    float p = Mathf.Lerp(a, b, this.m_MatchWidthOrHeight);
                    scaleFactor = Mathf.Pow(2f, p);
                    break;
                }
                case ScreenMatchMode.Expand:
                    scaleFactor = Mathf.Min((float) (vector.x / this.m_ReferenceResolution.x), (float) (vector.y / this.m_ReferenceResolution.y));
                    break;

                case ScreenMatchMode.Shrink:
                    scaleFactor = Mathf.Max((float) (vector.x / this.m_ReferenceResolution.x), (float) (vector.y / this.m_ReferenceResolution.y));
                    break;
            }
            this.SetScaleFactor(scaleFactor);
            this.SetReferencePixelsPerUnit(this.m_ReferencePixelsPerUnit);
        }

        protected virtual void HandleWorldCanvas()
        {
            this.SetScaleFactor(this.m_DynamicPixelsPerUnit);
            this.SetReferencePixelsPerUnit(this.m_ReferencePixelsPerUnit);
        }

        protected override void OnDisable()
        {
            this.SetScaleFactor(1f);
            this.SetReferencePixelsPerUnit(100f);
            base.OnDisable();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.m_Canvas = base.GetComponent<Canvas>();
            this.Handle();
        }

        protected void SetReferencePixelsPerUnit(float referencePixelsPerUnit)
        {
            if (referencePixelsPerUnit != this.m_PrevReferencePixelsPerUnit)
            {
                this.m_Canvas.referencePixelsPerUnit = referencePixelsPerUnit;
                this.m_PrevReferencePixelsPerUnit = referencePixelsPerUnit;
            }
        }

        protected void SetScaleFactor(float scaleFactor)
        {
            if (scaleFactor != this.m_PrevScaleFactor)
            {
                this.m_Canvas.scaleFactor = scaleFactor;
                this.m_PrevScaleFactor = scaleFactor;
            }
        }

        protected virtual void Update()
        {
            this.Handle();
        }

        public float defaultSpriteDPI
        {
            get
            {
                return this.m_DefaultSpriteDPI;
            }
            set
            {
                this.m_DefaultSpriteDPI = value;
            }
        }

        public float dynamicPixelsPerUnit
        {
            get
            {
                return this.m_DynamicPixelsPerUnit;
            }
            set
            {
                this.m_DynamicPixelsPerUnit = value;
            }
        }

        public float fallbackScreenDPI
        {
            get
            {
                return this.m_FallbackScreenDPI;
            }
            set
            {
                this.m_FallbackScreenDPI = value;
            }
        }

        public float matchWidthOrHeight
        {
            get
            {
                return this.m_MatchWidthOrHeight;
            }
            set
            {
                this.m_MatchWidthOrHeight = value;
            }
        }

        public Unit physicalUnit
        {
            get
            {
                return this.m_PhysicalUnit;
            }
            set
            {
                this.m_PhysicalUnit = value;
            }
        }

        public float referencePixelsPerUnit
        {
            get
            {
                return this.m_ReferencePixelsPerUnit;
            }
            set
            {
                this.m_ReferencePixelsPerUnit = value;
            }
        }

        public Vector2 referenceResolution
        {
            get
            {
                return this.m_ReferenceResolution;
            }
            set
            {
                this.m_ReferenceResolution = value;
            }
        }

        public float scaleFactor
        {
            get
            {
                return this.m_ScaleFactor;
            }
            set
            {
                this.m_ScaleFactor = value;
            }
        }

        public ScreenMatchMode screenMatchMode
        {
            get
            {
                return this.m_ScreenMatchMode;
            }
            set
            {
                this.m_ScreenMatchMode = value;
            }
        }

        public ScaleMode uiScaleMode
        {
            get
            {
                return this.m_UiScaleMode;
            }
            set
            {
                this.m_UiScaleMode = value;
            }
        }

        public enum ScaleMode
        {
            ConstantPixelSize,
            ScaleWithScreenSize,
            ConstantPhysicalSize
        }

        public enum ScreenMatchMode
        {
            MatchWidthOrHeight,
            Expand,
            Shrink
        }

        public enum Unit
        {
            Centimeters,
            Millimeters,
            Inches,
            Points,
            Picas
        }
    }
}

