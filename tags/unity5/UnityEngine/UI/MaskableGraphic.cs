namespace UnityEngine.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Rendering;

    public abstract class MaskableGraphic : Graphic, IMaskable, IClippable, IMaterialModifier
    {
        private readonly Vector3[] m_Corners = new Vector3[4];
        [NonSerialized, Obsolete("Not used anymore.", true)]
        protected bool m_IncludeForMasking;
        [NonSerialized]
        private bool m_Maskable = true;
        [NonSerialized]
        protected Material m_MaskMaterial;
        [SerializeField]
        private CullStateChangedEvent m_OnCullStateChanged = new CullStateChangedEvent();
        [NonSerialized]
        private RectMask2D m_ParentMask;
        [NonSerialized, Obsolete("Not used anymore", true)]
        protected bool m_ShouldRecalculate = true;
        [NonSerialized]
        protected bool m_ShouldRecalculateStencil = true;
        [NonSerialized]
        protected int m_StencilValue;

        protected MaskableGraphic()
        {
        }

        public virtual void Cull(Rect clipRect, bool validRect)
        {
            if (base.canvasRenderer.hasMoved)
            {
                bool flag = !validRect || !clipRect.Overlaps(this.canvasRect);
                bool flag2 = base.canvasRenderer.cull != flag;
                base.canvasRenderer.cull = flag;
                if (flag2)
                {
                    this.m_OnCullStateChanged.Invoke(flag);
                    this.SetVerticesDirty();
                }
            }
        }

        public virtual Material GetModifiedMaterial(Material baseMaterial)
        {
            Material baseMat = baseMaterial;
            if (this.m_ShouldRecalculateStencil)
            {
                Transform stopAfter = MaskUtilities.FindRootSortOverrideCanvas(base.transform);
                this.m_StencilValue = !this.maskable ? 0 : MaskUtilities.GetStencilDepth(base.transform, stopAfter);
                this.m_ShouldRecalculateStencil = false;
            }
            if ((this.m_StencilValue > 0) && (base.GetComponent<Mask>() == null))
            {
                Material material2 = StencilMaterial.Add(baseMat, (((int) 1) << this.m_StencilValue) - 1, StencilOp.Keep, CompareFunction.Equal, ColorWriteMask.All, (((int) 1) << this.m_StencilValue) - 1, 0);
                StencilMaterial.Remove(this.m_MaskMaterial);
                this.m_MaskMaterial = material2;
                baseMat = this.m_MaskMaterial;
            }
            return baseMat;
        }

        protected override void OnCanvasHierarchyChanged()
        {
            base.OnCanvasHierarchyChanged();
            this.m_ShouldRecalculateStencil = true;
            this.UpdateClipParent();
            this.SetMaterialDirty();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.m_ShouldRecalculateStencil = true;
            this.SetMaterialDirty();
            this.UpdateClipParent();
            StencilMaterial.Remove(this.m_MaskMaterial);
            this.m_MaskMaterial = null;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.m_ShouldRecalculateStencil = true;
            this.UpdateClipParent();
            this.SetMaterialDirty();
        }

        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();
            this.m_ShouldRecalculateStencil = true;
            this.UpdateClipParent();
            this.SetMaterialDirty();
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            this.m_ShouldRecalculateStencil = true;
            this.UpdateClipParent();
            this.SetMaterialDirty();
        }

        [Obsolete("Not used anymore.", true)]
        public virtual void ParentMaskStateChanged()
        {
        }

        public virtual void RecalculateClipping()
        {
            this.UpdateClipParent();
        }

        public virtual void RecalculateMasking()
        {
            this.m_ShouldRecalculateStencil = true;
            this.SetMaterialDirty();
        }

        public virtual void SetClipRect(Rect clipRect, bool validRect)
        {
            if (validRect)
            {
                base.canvasRenderer.EnableRectClipping(clipRect);
            }
            else
            {
                base.canvasRenderer.DisableRectClipping();
            }
        }

        RectTransform IClippable.get_rectTransform()
        {
            return base.rectTransform;
        }

        private void UpdateClipParent()
        {
            RectMask2D maskd = (!this.maskable || !this.IsActive()) ? null : MaskUtilities.GetRectMaskForClippable(this);
            if ((maskd != this.m_ParentMask) && (this.m_ParentMask != null))
            {
                this.m_ParentMask.RemoveClippable(this);
            }
            if (maskd != null)
            {
                maskd.AddClippable(this);
            }
            this.m_ParentMask = maskd;
        }

        private Rect canvasRect
        {
            get
            {
                base.rectTransform.GetWorldCorners(this.m_Corners);
                if (base.canvas != null)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        this.m_Corners[i] = base.canvas.transform.InverseTransformPoint(this.m_Corners[i]);
                    }
                }
                return new Rect(this.m_Corners[0].x, this.m_Corners[0].y, this.m_Corners[2].x - this.m_Corners[0].x, this.m_Corners[2].y - this.m_Corners[0].y);
            }
        }

        public bool maskable
        {
            get
            {
                return this.m_Maskable;
            }
            set
            {
                if (value != this.m_Maskable)
                {
                    this.m_Maskable = value;
                    this.m_ShouldRecalculateStencil = true;
                    this.SetMaterialDirty();
                }
            }
        }

        public CullStateChangedEvent onCullStateChanged
        {
            get
            {
                return this.m_OnCullStateChanged;
            }
            set
            {
                this.m_OnCullStateChanged = value;
            }
        }

        [Serializable]
        public class CullStateChangedEvent : UnityEvent<bool>
        {
        }
    }
}

