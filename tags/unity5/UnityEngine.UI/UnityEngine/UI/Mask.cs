namespace UnityEngine.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.Rendering;
    using UnityEngine.Serialization;

    [ExecuteInEditMode, AddComponentMenu("UI/Mask", 13), RequireComponent(typeof(RectTransform))]
    public class Mask : UIBehaviour, ICanvasRaycastFilter, IMaterialModifier
    {
        [NonSerialized]
        private Graphic m_Graphic;
        [NonSerialized]
        private Material m_MaskMaterial;
        [NonSerialized]
        private RectTransform m_RectTransform;
        [FormerlySerializedAs("m_ShowGraphic"), SerializeField]
        private bool m_ShowMaskGraphic = true;
        [NonSerialized]
        private Material m_UnmaskMaterial;

        protected Mask()
        {
        }

        public virtual Material GetModifiedMaterial(Material baseMaterial)
        {
            if (this.graphic == null)
            {
                return baseMaterial;
            }
            Transform stopAfter = MaskUtilities.FindRootSortOverrideCanvas(base.transform);
            int stencilDepth = MaskUtilities.GetStencilDepth(base.transform, stopAfter);
            if (stencilDepth >= 8)
            {
                Debug.LogError("Attempting to use a stencil mask with depth > 8", base.gameObject);
                return baseMaterial;
            }
            int num2 = ((int) 1) << stencilDepth;
            if (num2 == 1)
            {
                Material material = StencilMaterial.Add(baseMaterial, 1, StencilOp.Replace, CompareFunction.Always, !this.m_ShowMaskGraphic ? ((ColorWriteMask) 0) : ColorWriteMask.All);
                StencilMaterial.Remove(this.m_MaskMaterial);
                this.m_MaskMaterial = material;
                Material material2 = StencilMaterial.Add(baseMaterial, 1, StencilOp.Zero, CompareFunction.Always, 0);
                StencilMaterial.Remove(this.m_UnmaskMaterial);
                this.m_UnmaskMaterial = material2;
                this.graphic.canvasRenderer.popMaterialCount = 1;
                this.graphic.canvasRenderer.SetPopMaterial(this.m_UnmaskMaterial, 0);
                return this.m_MaskMaterial;
            }
            Material material3 = StencilMaterial.Add(baseMaterial, num2 | (num2 - 1), StencilOp.Replace, CompareFunction.Equal, !this.m_ShowMaskGraphic ? ((ColorWriteMask) 0) : ColorWriteMask.All, num2 - 1, num2 | (num2 - 1));
            StencilMaterial.Remove(this.m_MaskMaterial);
            this.m_MaskMaterial = material3;
            this.graphic.canvasRenderer.hasPopInstruction = true;
            Material material4 = StencilMaterial.Add(baseMaterial, num2 - 1, StencilOp.Replace, CompareFunction.Equal, 0, num2 - 1, num2 | (num2 - 1));
            StencilMaterial.Remove(this.m_UnmaskMaterial);
            this.m_UnmaskMaterial = material4;
            this.graphic.canvasRenderer.popMaterialCount = 1;
            this.graphic.canvasRenderer.SetPopMaterial(this.m_UnmaskMaterial, 0);
            return this.m_MaskMaterial;
        }

        public virtual bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(this.rectTransform, sp, eventCamera);
        }

        [Obsolete("use Mask.enabled instead", true)]
        public virtual bool MaskEnabled()
        {
            throw new NotSupportedException();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (this.graphic != null)
            {
                this.graphic.SetMaterialDirty();
                this.graphic.canvasRenderer.hasPopInstruction = false;
                this.graphic.canvasRenderer.popMaterialCount = 0;
            }
            StencilMaterial.Remove(this.m_MaskMaterial);
            this.m_MaskMaterial = null;
            StencilMaterial.Remove(this.m_UnmaskMaterial);
            this.m_UnmaskMaterial = null;
            MaskUtilities.NotifyStencilStateChanged(this);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (this.graphic != null)
            {
                this.graphic.canvasRenderer.hasPopInstruction = true;
                this.graphic.SetMaterialDirty();
            }
            MaskUtilities.NotifyStencilStateChanged(this);
        }

        [Obsolete("Not used anymore.")]
        public virtual void OnSiblingGraphicEnabledDisabled()
        {
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            if (this.IsActive())
            {
                if (this.graphic != null)
                {
                    this.graphic.SetMaterialDirty();
                }
                MaskUtilities.NotifyStencilStateChanged(this);
            }
        }

        public Graphic graphic
        {
            get
            {
                if (this.m_Graphic == null)
                {
                }
                return (this.m_Graphic = base.GetComponent<Graphic>());
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

        public bool showMaskGraphic
        {
            get
            {
                return this.m_ShowMaskGraphic;
            }
            set
            {
                if (this.m_ShowMaskGraphic != value)
                {
                    this.m_ShowMaskGraphic = value;
                    if (this.graphic != null)
                    {
                        this.graphic.SetMaterialDirty();
                    }
                }
            }
        }
    }
}

