namespace UnityEngine.UI
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.Serialization;
    using UnityEngine.UI.CoroutineTween;

    [RequireComponent(typeof(CanvasRenderer)), RequireComponent(typeof(RectTransform)), ExecuteInEditMode, DisallowMultipleComponent]
    public abstract class Graphic : UIBehaviour, ICanvasElement
    {
        [NonSerialized]
        private Canvas m_Canvas;
        [NonSerialized]
        private CanvasRenderer m_CanvasRender;
        [SerializeField]
        private Color m_Color = Color.white;
        [NonSerialized]
        private readonly TweenRunner<ColorTween> m_ColorTweenRunner;
        [FormerlySerializedAs("m_Mat"), SerializeField]
        protected Material m_Material;
        [NonSerialized]
        private bool m_MaterialDirty;
        [NonSerialized]
        protected UnityAction m_OnDirtyLayoutCallback;
        [NonSerialized]
        protected UnityAction m_OnDirtyMaterialCallback;
        [NonSerialized]
        protected UnityAction m_OnDirtyVertsCallback;
        [SerializeField]
        private bool m_RaycastTarget = true;
        [NonSerialized]
        private RectTransform m_RectTransform;
        [NonSerialized]
        private bool m_VertsDirty;
        protected static Material s_DefaultUI;
        [NonSerialized]
        protected static Mesh s_Mesh;
        protected static Texture2D s_WhiteTexture;

        protected Graphic()
        {
            if (this.m_ColorTweenRunner == null)
            {
                this.m_ColorTweenRunner = new TweenRunner<ColorTween>();
            }
            this.m_ColorTweenRunner.Init(this);
        }

        private void CacheCanvas()
        {
            List<Canvas> results = ListPool<Canvas>.Get();
            base.gameObject.GetComponentsInParent<Canvas>(false, results);
            if (results.Count > 0)
            {
                this.m_Canvas = results[0];
            }
            ListPool<Canvas>.Release(results);
        }

        private static Color CreateColorFromAlpha(float alpha)
        {
            Color black = Color.black;
            black.a = alpha;
            return black;
        }

        public void CrossFadeAlpha(float alpha, float duration, bool ignoreTimeScale)
        {
            this.CrossFadeColor(CreateColorFromAlpha(alpha), duration, ignoreTimeScale, true, false);
        }

        public void CrossFadeColor(Color targetColor, float duration, bool ignoreTimeScale, bool useAlpha)
        {
            this.CrossFadeColor(targetColor, duration, ignoreTimeScale, useAlpha, true);
        }

        private void CrossFadeColor(Color targetColor, float duration, bool ignoreTimeScale, bool useAlpha, bool useRGB)
        {
            if (((this.canvasRenderer != null) && (useRGB || useAlpha)) && !this.canvasRenderer.GetColor().Equals(targetColor))
            {
                ColorTween.ColorTweenMode mode = (!useRGB || !useAlpha) ? (!useRGB ? ColorTween.ColorTweenMode.Alpha : ColorTween.ColorTweenMode.RGB) : ColorTween.ColorTweenMode.All;
                ColorTween info = new ColorTween {
                    duration = duration,
                    startColor = this.canvasRenderer.GetColor(),
                    targetColor = targetColor
                };
                info.AddOnChangedCallback(new UnityAction<Color>(this.canvasRenderer.SetColor));
                info.ignoreTimeScale = ignoreTimeScale;
                info.tweenMode = mode;
                this.m_ColorTweenRunner.StartTween(info);
            }
        }

        public Rect GetPixelAdjustedRect()
        {
            if ((this.canvas != null) && this.canvas.pixelPerfect)
            {
                return RectTransformUtility.PixelAdjustRect(this.rectTransform, this.canvas);
            }
            return this.rectTransform.rect;
        }

        protected override void OnBeforeTransformParentChanged()
        {
            GraphicRegistry.UnregisterGraphicForCanvas(this.canvas, this);
            LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
        }

        protected override void OnCanvasHierarchyChanged()
        {
            Canvas c = this.m_Canvas;
            this.CacheCanvas();
            if (c != this.m_Canvas)
            {
                GraphicRegistry.UnregisterGraphicForCanvas(c, this);
                GraphicRegistry.RegisterGraphicForCanvas(this.canvas, this);
            }
        }

        protected override void OnDidApplyAnimationProperties()
        {
            this.SetAllDirty();
        }

        protected override void OnDisable()
        {
            GraphicRebuildTracker.UnTrackGraphic(this);
            GraphicRegistry.UnregisterGraphicForCanvas(this.canvas, this);
            CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);
            if (this.canvasRenderer != null)
            {
                this.canvasRenderer.Clear();
            }
            LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
            base.OnDisable();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.CacheCanvas();
            GraphicRegistry.RegisterGraphicForCanvas(this.canvas, this);
            GraphicRebuildTracker.TrackGraphic(this);
            if (s_WhiteTexture == null)
            {
                s_WhiteTexture = Texture2D.whiteTexture;
            }
            this.SetAllDirty();
        }

        [Obsolete("Use OnPopulateMesh instead.", true)]
        protected virtual void OnFillVBO(List<UIVertex> vbo)
        {
        }

        protected virtual void OnPopulateMesh(Mesh m)
        {
            Rect pixelAdjustedRect = this.GetPixelAdjustedRect();
            Vector4 vector = new Vector4(pixelAdjustedRect.x, pixelAdjustedRect.y, pixelAdjustedRect.x + pixelAdjustedRect.width, pixelAdjustedRect.y + pixelAdjustedRect.height);
            Color32 color = this.color;
            using (VertexHelper helper = new VertexHelper())
            {
                helper.AddVert(new Vector3(vector.x, vector.y), color, new Vector2(0f, 0f));
                helper.AddVert(new Vector3(vector.x, vector.w), color, new Vector2(0f, 1f));
                helper.AddVert(new Vector3(vector.z, vector.w), color, new Vector2(1f, 1f));
                helper.AddVert(new Vector3(vector.z, vector.y), color, new Vector2(1f, 0f));
                helper.AddTriangle(0, 1, 2);
                helper.AddTriangle(2, 3, 0);
                helper.FillMesh(m);
            }
        }

        public virtual void OnRebuildRequested()
        {
            foreach (MonoBehaviour behaviour in base.gameObject.GetComponents<MonoBehaviour>())
            {
                MethodInfo method = behaviour.GetType().GetMethod("OnValidate", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (method != null)
                {
                    method.Invoke(behaviour, null);
                }
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
            if (base.gameObject.activeInHierarchy)
            {
                if (CanvasUpdateRegistry.IsRebuildingLayout())
                {
                    this.SetVerticesDirty();
                }
                else
                {
                    this.SetVerticesDirty();
                    this.SetLayoutDirty();
                }
            }
        }

        protected override void OnTransformParentChanged()
        {
            if (this.IsActive())
            {
                this.CacheCanvas();
                GraphicRegistry.RegisterGraphicForCanvas(this.canvas, this);
                this.SetAllDirty();
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            this.SetAllDirty();
        }

        public Vector2 PixelAdjustPoint(Vector2 point)
        {
            if ((this.canvas != null) && this.canvas.pixelPerfect)
            {
                return RectTransformUtility.PixelAdjustPoint(point, base.transform, this.canvas);
            }
            return point;
        }

        public virtual bool Raycast(Vector2 sp, Camera eventCamera)
        {
            if (!base.isActiveAndEnabled)
            {
                return false;
            }
            Transform parent = base.transform;
            List<Component> results = ListPool<Component>.Get();
            bool flag = false;
            while (parent != null)
            {
                parent.GetComponents<Component>(results);
                for (int i = 0; i < results.Count; i++)
                {
                    ICanvasRaycastFilter filter = results[i] as ICanvasRaycastFilter;
                    if (filter != null)
                    {
                        bool flag2 = true;
                        CanvasGroup group = results[i] as CanvasGroup;
                        if (group != null)
                        {
                            if (!flag && group.ignoreParentGroups)
                            {
                                flag = true;
                                flag2 = filter.IsRaycastLocationValid(sp, eventCamera);
                            }
                            else if (!flag)
                            {
                                flag2 = filter.IsRaycastLocationValid(sp, eventCamera);
                            }
                        }
                        else
                        {
                            flag2 = filter.IsRaycastLocationValid(sp, eventCamera);
                        }
                        if (!flag2)
                        {
                            ListPool<Component>.Release(results);
                            return false;
                        }
                    }
                }
                parent = parent.parent;
            }
            ListPool<Component>.Release(results);
            return true;
        }

        public virtual void Rebuild(CanvasUpdate update)
        {
            if (!this.canvasRenderer.cull)
            {
                if (update == CanvasUpdate.PreRender)
                {
                    if (this.m_VertsDirty)
                    {
                        this.UpdateGeometry();
                        this.m_VertsDirty = false;
                    }
                    if (this.m_MaterialDirty)
                    {
                        this.UpdateMaterial();
                        this.m_MaterialDirty = false;
                    }
                }
            }
        }

        public void RegisterDirtyLayoutCallback(UnityAction action)
        {
            this.m_OnDirtyLayoutCallback = (UnityAction) Delegate.Combine(this.m_OnDirtyLayoutCallback, action);
        }

        public void RegisterDirtyMaterialCallback(UnityAction action)
        {
            this.m_OnDirtyMaterialCallback = (UnityAction) Delegate.Combine(this.m_OnDirtyMaterialCallback, action);
        }

        public void RegisterDirtyVerticesCallback(UnityAction action)
        {
            this.m_OnDirtyVertsCallback = (UnityAction) Delegate.Combine(this.m_OnDirtyVertsCallback, action);
        }

        public virtual void SetAllDirty()
        {
            this.SetLayoutDirty();
            this.SetVerticesDirty();
            this.SetMaterialDirty();
        }

        public virtual void SetLayoutDirty()
        {
            if (this.IsActive())
            {
                LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
                if (this.m_OnDirtyLayoutCallback != null)
                {
                    this.m_OnDirtyLayoutCallback();
                }
            }
        }

        public virtual void SetMaterialDirty()
        {
            if (this.IsActive())
            {
                this.m_MaterialDirty = true;
                CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
                if (this.m_OnDirtyMaterialCallback != null)
                {
                    this.m_OnDirtyMaterialCallback();
                }
            }
        }

        public virtual void SetNativeSize()
        {
        }

        public virtual void SetVerticesDirty()
        {
            if (this.IsActive())
            {
                this.m_VertsDirty = true;
                CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
                if (this.m_OnDirtyVertsCallback != null)
                {
                    this.m_OnDirtyVertsCallback();
                }
            }
        }

        Transform ICanvasElement.get_transform()
        {
            return base.transform;
        }

        bool ICanvasElement.IsDestroyed()
        {
            return base.IsDestroyed();
        }

        public void UnregisterDirtyLayoutCallback(UnityAction action)
        {
            this.m_OnDirtyLayoutCallback = (UnityAction) Delegate.Remove(this.m_OnDirtyLayoutCallback, action);
        }

        public void UnregisterDirtyMaterialCallback(UnityAction action)
        {
            this.m_OnDirtyMaterialCallback = (UnityAction) Delegate.Remove(this.m_OnDirtyMaterialCallback, action);
        }

        public void UnregisterDirtyVerticesCallback(UnityAction action)
        {
            this.m_OnDirtyVertsCallback = (UnityAction) Delegate.Remove(this.m_OnDirtyVertsCallback, action);
        }

        protected virtual void UpdateGeometry()
        {
            if (((this.rectTransform != null) && (this.rectTransform.rect.width >= 0f)) && (this.rectTransform.rect.height >= 0f))
            {
                this.OnPopulateMesh(workerMesh);
            }
            List<Component> results = ListPool<Component>.Get();
            base.GetComponents(typeof(IMeshModifier), results);
            for (int i = 0; i < results.Count; i++)
            {
                ((IMeshModifier) results[i]).ModifyMesh(workerMesh);
            }
            ListPool<Component>.Release(results);
            this.canvasRenderer.SetMesh(workerMesh);
        }

        protected virtual void UpdateMaterial()
        {
            if (this.IsActive())
            {
                this.canvasRenderer.materialCount = 1;
                this.canvasRenderer.SetMaterial(this.materialForRendering, 0);
                this.canvasRenderer.SetTexture(this.mainTexture);
            }
        }

        public Canvas canvas
        {
            get
            {
                if (this.m_Canvas == null)
                {
                    this.CacheCanvas();
                }
                return this.m_Canvas;
            }
        }

        public CanvasRenderer canvasRenderer
        {
            get
            {
                if (this.m_CanvasRender == null)
                {
                    this.m_CanvasRender = base.GetComponent<CanvasRenderer>();
                }
                return this.m_CanvasRender;
            }
        }

        public Color color
        {
            get
            {
                return this.m_Color;
            }
            set
            {
                if (SetPropertyUtility.SetColor(ref this.m_Color, value))
                {
                    this.SetVerticesDirty();
                }
            }
        }

        public static Material defaultGraphicMaterial
        {
            get
            {
                if (s_DefaultUI == null)
                {
                    s_DefaultUI = Canvas.GetDefaultCanvasMaterial();
                }
                return s_DefaultUI;
            }
        }

        public virtual Material defaultMaterial
        {
            get
            {
                return defaultGraphicMaterial;
            }
        }

        public int depth
        {
            get
            {
                return this.canvasRenderer.absoluteDepth;
            }
        }

        public virtual Texture mainTexture
        {
            get
            {
                return s_WhiteTexture;
            }
        }

        public virtual Material material
        {
            get
            {
                return ((this.m_Material == null) ? this.defaultMaterial : this.m_Material);
            }
            set
            {
                if (this.m_Material != value)
                {
                    this.m_Material = value;
                    this.SetMaterialDirty();
                }
            }
        }

        public virtual Material materialForRendering
        {
            get
            {
                List<Component> results = ListPool<Component>.Get();
                base.GetComponents(typeof(IMaterialModifier), results);
                Material baseMaterial = this.material;
                for (int i = 0; i < results.Count; i++)
                {
                    baseMaterial = (results[i] as IMaterialModifier).GetModifiedMaterial(baseMaterial);
                }
                ListPool<Component>.Release(results);
                return baseMaterial;
            }
        }

        public bool raycastTarget
        {
            get
            {
                return this.m_RaycastTarget;
            }
            set
            {
                this.m_RaycastTarget = value;
            }
        }

        public RectTransform rectTransform
        {
            get
            {
                if (this.m_RectTransform == null)
                {
                }
                return (this.m_RectTransform = base.GetComponent<RectTransform>());
            }
        }

        protected static Mesh workerMesh
        {
            get
            {
                if (s_Mesh == null)
                {
                    s_Mesh = new Mesh();
                    s_Mesh.name = "Shared UI Mesh";
                    s_Mesh.hideFlags = HideFlags.HideAndDontSave;
                }
                return s_Mesh;
            }
        }
    }
}

