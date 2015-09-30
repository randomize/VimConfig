namespace UnityEngine.UI
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;

    [ExecuteInEditMode, AddComponentMenu("UI/2D Rect Mask", 13), RequireComponent(typeof(RectTransform))]
    public class RectMask2D : UIBehaviour, ICanvasRaycastFilter, IClipper
    {
        [NonSerialized]
        private List<RectMask2D> m_Clippers = new List<RectMask2D>();
        [NonSerialized]
        private List<IClippable> m_ClipTargets = new List<IClippable>();
        [NonSerialized]
        private Rect m_LastClipRectCanvasSpace;
        [NonSerialized]
        private bool m_LastClipRectValid;
        [NonSerialized]
        private RectTransform m_RectTransform;
        [NonSerialized]
        private bool m_ShouldRecalculateClipRects;
        [NonSerialized]
        private readonly RectangularVertexClipper m_VertexClipper = new RectangularVertexClipper();

        protected RectMask2D()
        {
        }

        public void AddClippable(IClippable clippable)
        {
            if (clippable != null)
            {
                if (!this.m_ClipTargets.Contains(clippable))
                {
                    this.m_ClipTargets.Add(clippable);
                }
                clippable.SetClipRect(this.m_LastClipRectCanvasSpace, this.m_LastClipRectValid);
                clippable.Cull(this.m_LastClipRectCanvasSpace, this.m_LastClipRectValid);
            }
        }

        public virtual bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(this.rectTransform, sp, eventCamera);
        }

        protected override void OnCanvasHierarchyChanged()
        {
            base.OnCanvasHierarchyChanged();
            this.m_ShouldRecalculateClipRects = true;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.m_ClipTargets.Clear();
            this.m_Clippers.Clear();
            ClipperRegistry.Unregister(this);
            MaskUtilities.Notify2DMaskStateChanged(this);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.m_ShouldRecalculateClipRects = true;
            ClipperRegistry.Register(this);
            MaskUtilities.Notify2DMaskStateChanged(this);
        }

        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();
            this.m_ShouldRecalculateClipRects = true;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            this.m_ShouldRecalculateClipRects = true;
            if (this.IsActive())
            {
                MaskUtilities.Notify2DMaskStateChanged(this);
            }
        }

        public virtual void PerformClipping()
        {
            if (this.m_ShouldRecalculateClipRects)
            {
                MaskUtilities.GetRectMasksForClip(this, this.m_Clippers);
                this.m_ShouldRecalculateClipRects = false;
            }
            bool validRect = true;
            Rect rect = Clipping.FindCullAndClipWorldRect(this.m_Clippers, out validRect);
            if (rect != this.m_LastClipRectCanvasSpace)
            {
                for (int j = 0; j < this.m_ClipTargets.Count; j++)
                {
                    this.m_ClipTargets[j].SetClipRect(rect, validRect);
                }
                this.m_LastClipRectCanvasSpace = rect;
                this.m_LastClipRectValid = validRect;
            }
            for (int i = 0; i < this.m_ClipTargets.Count; i++)
            {
                this.m_ClipTargets[i].Cull(this.m_LastClipRectCanvasSpace, this.m_LastClipRectValid);
            }
        }

        public void RemoveClippable(IClippable clippable)
        {
            if (clippable != null)
            {
                clippable.SetClipRect(new Rect(), false);
                this.m_ClipTargets.Remove(clippable);
            }
        }

        public Rect canvasRect
        {
            get
            {
                return this.m_VertexClipper.GetCanvasRect(this.rectTransform, base.GetComponentInParent<Canvas>());
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
    }
}

