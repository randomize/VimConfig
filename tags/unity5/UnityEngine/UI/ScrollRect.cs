namespace UnityEngine.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    [SelectionBase, AddComponentMenu("UI/Scroll Rect", 0x25), ExecuteInEditMode, RequireComponent(typeof(RectTransform))]
    public class ScrollRect : UIBehaviour, IEventSystemHandler, IBeginDragHandler, IInitializePotentialDragHandler, IDragHandler, IEndDragHandler, IScrollHandler, ICanvasElement, ILayoutController, ILayoutGroup
    {
        [SerializeField]
        private RectTransform m_Content;
        private Bounds m_ContentBounds;
        private Vector2 m_ContentStartPosition = Vector2.zero;
        private readonly Vector3[] m_Corners = new Vector3[4];
        [SerializeField]
        private float m_DecelerationRate = 0.135f;
        private bool m_Dragging;
        [SerializeField]
        private float m_Elasticity = 0.1f;
        [NonSerialized]
        private bool m_HasRebuiltLayout;
        [SerializeField]
        private bool m_Horizontal = true;
        [SerializeField]
        private Scrollbar m_HorizontalScrollbar;
        private RectTransform m_HorizontalScrollbarRect;
        [SerializeField]
        private float m_HorizontalScrollbarSpacing;
        [SerializeField]
        private ScrollbarVisibility m_HorizontalScrollbarVisibility;
        private bool m_HSliderExpand;
        private float m_HSliderHeight;
        [SerializeField]
        private bool m_Inertia = true;
        [SerializeField]
        private MovementType m_MovementType = MovementType.Elastic;
        [SerializeField]
        private ScrollRectEvent m_OnValueChanged = new ScrollRectEvent();
        private Vector2 m_PointerStartLocalCursor = Vector2.zero;
        private Bounds m_PrevContentBounds;
        private Vector2 m_PrevPosition = Vector2.zero;
        private Bounds m_PrevViewBounds;
        [NonSerialized]
        private RectTransform m_Rect;
        [SerializeField]
        private float m_ScrollSensitivity = 1f;
        private DrivenRectTransformTracker m_Tracker;
        private Vector2 m_Velocity;
        [SerializeField]
        private bool m_Vertical = true;
        [SerializeField]
        private Scrollbar m_VerticalScrollbar;
        private RectTransform m_VerticalScrollbarRect;
        [SerializeField]
        private float m_VerticalScrollbarSpacing;
        [SerializeField]
        private ScrollbarVisibility m_VerticalScrollbarVisibility;
        private Bounds m_ViewBounds;
        [SerializeField]
        private RectTransform m_Viewport;
        private RectTransform m_ViewRect;
        private bool m_VSliderExpand;
        private float m_VSliderWidth;

        protected ScrollRect()
        {
        }

        private Vector2 CalculateOffset(Vector2 delta)
        {
            Vector2 zero = Vector2.zero;
            if (this.m_MovementType != MovementType.Unrestricted)
            {
                Vector2 min = this.m_ContentBounds.min;
                Vector2 max = this.m_ContentBounds.max;
                if (this.m_Horizontal)
                {
                    min.x += delta.x;
                    max.x += delta.x;
                    if (min.x > this.m_ViewBounds.min.x)
                    {
                        zero.x = this.m_ViewBounds.min.x - min.x;
                    }
                    else if (max.x < this.m_ViewBounds.max.x)
                    {
                        zero.x = this.m_ViewBounds.max.x - max.x;
                    }
                }
                if (!this.m_Vertical)
                {
                    return zero;
                }
                min.y += delta.y;
                max.y += delta.y;
                if (max.y < this.m_ViewBounds.max.y)
                {
                    zero.y = this.m_ViewBounds.max.y - max.y;
                    return zero;
                }
                if (min.y > this.m_ViewBounds.min.y)
                {
                    zero.y = this.m_ViewBounds.min.y - min.y;
                }
            }
            return zero;
        }

        private void EnsureLayoutHasRebuilt()
        {
            if (!this.m_HasRebuiltLayout && !CanvasUpdateRegistry.IsRebuildingLayout())
            {
                Canvas.ForceUpdateCanvases();
            }
        }

        private Bounds GetBounds()
        {
            if (this.m_Content == null)
            {
                return new Bounds();
            }
            Vector3 rhs = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            Matrix4x4 worldToLocalMatrix = this.viewRect.worldToLocalMatrix;
            this.m_Content.GetWorldCorners(this.m_Corners);
            for (int i = 0; i < 4; i++)
            {
                Vector3 lhs = worldToLocalMatrix.MultiplyPoint3x4(this.m_Corners[i]);
                rhs = Vector3.Min(lhs, rhs);
                vector2 = Vector3.Max(lhs, vector2);
            }
            Bounds bounds = new Bounds(rhs, Vector3.zero);
            bounds.Encapsulate(vector2);
            return bounds;
        }

        public override bool IsActive()
        {
            return (base.IsActive() && (this.m_Content != null));
        }

        protected virtual unsafe void LateUpdate()
        {
            if (this.m_Content != null)
            {
                this.EnsureLayoutHasRebuilt();
                this.UpdateScrollbarVisibility();
                this.UpdateBounds();
                float unscaledDeltaTime = Time.unscaledDeltaTime;
                Vector2 offset = this.CalculateOffset(Vector2.zero);
                if (!this.m_Dragging && ((offset != Vector2.zero) || (this.m_Velocity != Vector2.zero)))
                {
                    Vector2 anchoredPosition = this.m_Content.anchoredPosition;
                    for (int i = 0; i < 2; i++)
                    {
                        if ((this.m_MovementType == MovementType.Elastic) && (offset[i] != 0f))
                        {
                            float currentVelocity = this.m_Velocity[i];
                            anchoredPosition[i] = Mathf.SmoothDamp(this.m_Content.anchoredPosition[i], this.m_Content.anchoredPosition[i] + offset[i], ref currentVelocity, this.m_Elasticity, float.PositiveInfinity, unscaledDeltaTime);
                            this.m_Velocity[i] = currentVelocity;
                        }
                        else if (this.m_Inertia)
                        {
                            ref Vector2 vectorRef;
                            int num4;
                            ref Vector2 vectorRef2;
                            float num5 = vectorRef[num4];
                            (vectorRef = (Vector2) &this.m_Velocity)[num4 = i] = num5 * Mathf.Pow(this.m_DecelerationRate, unscaledDeltaTime);
                            if (Mathf.Abs(this.m_Velocity[i]) < 1f)
                            {
                                this.m_Velocity[i] = 0f;
                            }
                            num5 = vectorRef2[num4];
                            (vectorRef2 = (Vector2) &anchoredPosition)[num4 = i] = num5 + (this.m_Velocity[i] * unscaledDeltaTime);
                        }
                        else
                        {
                            this.m_Velocity[i] = 0f;
                        }
                    }
                    if (this.m_Velocity != Vector2.zero)
                    {
                        if (this.m_MovementType == MovementType.Clamped)
                        {
                            offset = this.CalculateOffset(anchoredPosition - this.m_Content.anchoredPosition);
                            anchoredPosition += offset;
                        }
                        this.SetContentAnchoredPosition(anchoredPosition);
                    }
                }
                if (this.m_Dragging && this.m_Inertia)
                {
                    Vector3 b = (Vector3) ((this.m_Content.anchoredPosition - this.m_PrevPosition) / unscaledDeltaTime);
                    this.m_Velocity = Vector3.Lerp((Vector3) this.m_Velocity, b, unscaledDeltaTime * 10f);
                }
                if (((this.m_ViewBounds != this.m_PrevViewBounds) || (this.m_ContentBounds != this.m_PrevContentBounds)) || (this.m_Content.anchoredPosition != this.m_PrevPosition))
                {
                    this.UpdateScrollbars(offset);
                    this.m_OnValueChanged.Invoke(this.normalizedPosition);
                    this.UpdatePrevData();
                }
            }
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if ((eventData.button == PointerEventData.InputButton.Left) && this.IsActive())
            {
                this.UpdateBounds();
                this.m_PointerStartLocalCursor = Vector2.zero;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(this.viewRect, eventData.position, eventData.pressEventCamera, out this.m_PointerStartLocalCursor);
                this.m_ContentStartPosition = this.m_Content.anchoredPosition;
                this.m_Dragging = true;
            }
        }

        protected override void OnDisable()
        {
            CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);
            if (this.m_HorizontalScrollbar != null)
            {
                this.m_HorizontalScrollbar.onValueChanged.RemoveListener(new UnityAction<float>(this.SetHorizontalNormalizedPosition));
            }
            if (this.m_VerticalScrollbar != null)
            {
                this.m_VerticalScrollbar.onValueChanged.RemoveListener(new UnityAction<float>(this.SetVerticalNormalizedPosition));
            }
            this.m_HasRebuiltLayout = false;
            this.m_Tracker.Clear();
            LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
            base.OnDisable();
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            Vector2 vector;
            if (((eventData.button == PointerEventData.InputButton.Left) && this.IsActive()) && RectTransformUtility.ScreenPointToLocalPointInRectangle(this.viewRect, eventData.position, eventData.pressEventCamera, out vector))
            {
                this.UpdateBounds();
                Vector2 vector2 = vector - this.m_PointerStartLocalCursor;
                Vector2 position = this.m_ContentStartPosition + vector2;
                Vector2 vector4 = this.CalculateOffset(position - this.m_Content.anchoredPosition);
                position += vector4;
                if (this.m_MovementType == MovementType.Elastic)
                {
                    if (vector4.x != 0f)
                    {
                        position.x -= RubberDelta(vector4.x, this.m_ViewBounds.size.x);
                    }
                    if (vector4.y != 0f)
                    {
                        position.y -= RubberDelta(vector4.y, this.m_ViewBounds.size.y);
                    }
                }
                this.SetContentAnchoredPosition(position);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (this.m_HorizontalScrollbar != null)
            {
                this.m_HorizontalScrollbar.onValueChanged.AddListener(new UnityAction<float>(this.SetHorizontalNormalizedPosition));
            }
            if (this.m_VerticalScrollbar != null)
            {
                this.m_VerticalScrollbar.onValueChanged.AddListener(new UnityAction<float>(this.SetVerticalNormalizedPosition));
            }
            CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                this.m_Dragging = false;
            }
        }

        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                this.m_Velocity = Vector2.zero;
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
            this.SetDirty();
        }

        public virtual void OnScroll(PointerEventData data)
        {
            if (this.IsActive())
            {
                this.EnsureLayoutHasRebuilt();
                this.UpdateBounds();
                Vector2 scrollDelta = data.scrollDelta;
                scrollDelta.y *= -1f;
                if (this.vertical && !this.horizontal)
                {
                    float introduced2 = Mathf.Abs(scrollDelta.x);
                    if (introduced2 > Mathf.Abs(scrollDelta.y))
                    {
                        scrollDelta.y = scrollDelta.x;
                    }
                    scrollDelta.x = 0f;
                }
                if (this.horizontal && !this.vertical)
                {
                    float introduced3 = Mathf.Abs(scrollDelta.y);
                    if (introduced3 > Mathf.Abs(scrollDelta.x))
                    {
                        scrollDelta.x = scrollDelta.y;
                    }
                    scrollDelta.y = 0f;
                }
                Vector2 position = this.m_Content.anchoredPosition + ((Vector2) (scrollDelta * this.m_ScrollSensitivity));
                if (this.m_MovementType == MovementType.Clamped)
                {
                    position += this.CalculateOffset(position - this.m_Content.anchoredPosition);
                }
                this.SetContentAnchoredPosition(position);
                this.UpdateBounds();
            }
        }

        protected override void OnValidate()
        {
            this.SetDirtyCaching();
        }

        public virtual void Rebuild(CanvasUpdate executing)
        {
            if (executing == CanvasUpdate.Prelayout)
            {
                this.UpdateCachedData();
            }
            if (executing == CanvasUpdate.PostLayout)
            {
                this.UpdateBounds();
                this.UpdateScrollbars(Vector2.zero);
                this.UpdatePrevData();
                this.m_HasRebuiltLayout = true;
            }
        }

        private static float RubberDelta(float overStretching, float viewSize)
        {
            return (((1f - (1f / (((Mathf.Abs(overStretching) * 0.55f) / viewSize) + 1f))) * viewSize) * Mathf.Sign(overStretching));
        }

        protected virtual void SetContentAnchoredPosition(Vector2 position)
        {
            if (!this.m_Horizontal)
            {
                position.x = this.m_Content.anchoredPosition.x;
            }
            if (!this.m_Vertical)
            {
                position.y = this.m_Content.anchoredPosition.y;
            }
            if (position != this.m_Content.anchoredPosition)
            {
                this.m_Content.anchoredPosition = position;
                this.UpdateBounds();
            }
        }

        protected void SetDirty()
        {
            if (this.IsActive())
            {
                LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
            }
        }

        protected void SetDirtyCaching()
        {
            if (this.IsActive())
            {
                CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
                LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
            }
        }

        private void SetHorizontalNormalizedPosition(float value)
        {
            this.SetNormalizedPosition(value, 0);
        }

        public virtual void SetLayoutHorizontal()
        {
            this.m_Tracker.Clear();
            if (this.m_HSliderExpand || this.m_VSliderExpand)
            {
                this.m_Tracker.Add(this, this.viewRect, DrivenTransformProperties.SizeDelta | DrivenTransformProperties.Anchors | DrivenTransformProperties.AnchoredPosition);
                this.viewRect.anchorMin = Vector2.zero;
                this.viewRect.anchorMax = Vector2.one;
                this.viewRect.sizeDelta = Vector2.zero;
                this.viewRect.anchoredPosition = Vector2.zero;
                LayoutRebuilder.ForceRebuildLayoutImmediate(this.content);
                this.m_ViewBounds = new Bounds((Vector3) this.viewRect.rect.center, (Vector3) this.viewRect.rect.size);
                this.m_ContentBounds = this.GetBounds();
            }
            if (this.m_VSliderExpand && this.vScrollingNeeded)
            {
                this.viewRect.sizeDelta = new Vector2(-(this.m_VSliderWidth + this.m_VerticalScrollbarSpacing), this.viewRect.sizeDelta.y);
                LayoutRebuilder.ForceRebuildLayoutImmediate(this.content);
                this.m_ViewBounds = new Bounds((Vector3) this.viewRect.rect.center, (Vector3) this.viewRect.rect.size);
                this.m_ContentBounds = this.GetBounds();
            }
            if (this.m_HSliderExpand && this.hScrollingNeeded)
            {
                this.viewRect.sizeDelta = new Vector2(this.viewRect.sizeDelta.x, -(this.m_HSliderHeight + this.m_HorizontalScrollbarSpacing));
                this.m_ViewBounds = new Bounds((Vector3) this.viewRect.rect.center, (Vector3) this.viewRect.rect.size);
                this.m_ContentBounds = this.GetBounds();
            }
            if ((this.m_VSliderExpand && this.vScrollingNeeded) && ((this.viewRect.sizeDelta.x == 0f) && (this.viewRect.sizeDelta.y < 0f)))
            {
                this.viewRect.sizeDelta = new Vector2(-(this.m_VSliderWidth + this.m_VerticalScrollbarSpacing), this.viewRect.sizeDelta.y);
            }
        }

        public virtual void SetLayoutVertical()
        {
            this.UpdateScrollbarLayout();
            this.m_ViewBounds = new Bounds((Vector3) this.viewRect.rect.center, (Vector3) this.viewRect.rect.size);
            this.m_ContentBounds = this.GetBounds();
        }

        private void SetNormalizedPosition(float value, int axis)
        {
            this.EnsureLayoutHasRebuilt();
            this.UpdateBounds();
            float num = this.m_ContentBounds.size[axis] - this.m_ViewBounds.size[axis];
            float num2 = this.m_ViewBounds.min[axis] - (value * num);
            float num3 = (this.m_Content.localPosition[axis] + num2) - this.m_ContentBounds.min[axis];
            Vector3 localPosition = this.m_Content.localPosition;
            if (Mathf.Abs((float) (localPosition[axis] - num3)) > 0.01f)
            {
                localPosition[axis] = num3;
                this.m_Content.localPosition = localPosition;
                this.m_Velocity[axis] = 0f;
                this.UpdateBounds();
            }
        }

        private void SetVerticalNormalizedPosition(float value)
        {
            this.SetNormalizedPosition(value, 1);
        }

        public virtual void StopMovement()
        {
            this.m_Velocity = Vector2.zero;
        }

        Transform ICanvasElement.get_transform()
        {
            return base.transform;
        }

        bool ICanvasElement.IsDestroyed()
        {
            return base.IsDestroyed();
        }

        private void UpdateBounds()
        {
            this.m_ViewBounds = new Bounds((Vector3) this.viewRect.rect.center, (Vector3) this.viewRect.rect.size);
            this.m_ContentBounds = this.GetBounds();
            if (this.m_Content != null)
            {
                Vector3 size = this.m_ContentBounds.size;
                Vector3 center = this.m_ContentBounds.center;
                Vector3 vector3 = this.m_ViewBounds.size - size;
                if (vector3.x > 0f)
                {
                    center.x -= vector3.x * (this.m_Content.pivot.x - 0.5f);
                    size.x = this.m_ViewBounds.size.x;
                }
                if (vector3.y > 0f)
                {
                    center.y -= vector3.y * (this.m_Content.pivot.y - 0.5f);
                    size.y = this.m_ViewBounds.size.y;
                }
                this.m_ContentBounds.size = size;
                this.m_ContentBounds.center = center;
            }
        }

        private void UpdateCachedData()
        {
            Transform transform = base.transform;
            this.m_HorizontalScrollbarRect = (this.m_HorizontalScrollbar != null) ? (this.m_HorizontalScrollbar.transform as RectTransform) : null;
            this.m_VerticalScrollbarRect = (this.m_VerticalScrollbar != null) ? (this.m_VerticalScrollbar.transform as RectTransform) : null;
            bool flag = this.viewRect.parent == transform;
            bool flag2 = (this.m_HorizontalScrollbarRect == null) || (this.m_HorizontalScrollbarRect.parent == transform);
            bool flag3 = (this.m_VerticalScrollbarRect == null) || (this.m_VerticalScrollbarRect.parent == transform);
            bool flag4 = (flag && flag2) && flag3;
            this.m_HSliderExpand = (flag4 && (this.m_HorizontalScrollbarRect != null)) && (this.horizontalScrollbarVisibility == ScrollbarVisibility.AutoHideAndExpandViewport);
            this.m_VSliderExpand = (flag4 && (this.m_VerticalScrollbarRect != null)) && (this.verticalScrollbarVisibility == ScrollbarVisibility.AutoHideAndExpandViewport);
            this.m_HSliderHeight = (this.m_HorizontalScrollbarRect != null) ? this.m_HorizontalScrollbarRect.rect.height : 0f;
            this.m_VSliderWidth = (this.m_VerticalScrollbarRect != null) ? this.m_VerticalScrollbarRect.rect.width : 0f;
        }

        private void UpdatePrevData()
        {
            if (this.m_Content == null)
            {
                this.m_PrevPosition = Vector2.zero;
            }
            else
            {
                this.m_PrevPosition = this.m_Content.anchoredPosition;
            }
            this.m_PrevViewBounds = this.m_ViewBounds;
            this.m_PrevContentBounds = this.m_ContentBounds;
        }

        private void UpdateScrollbarLayout()
        {
            if (this.m_VSliderExpand && (this.m_HorizontalScrollbar != null))
            {
                this.m_Tracker.Add(this, this.m_HorizontalScrollbarRect, DrivenTransformProperties.SizeDeltaX | DrivenTransformProperties.AnchorMaxX | DrivenTransformProperties.AnchorMinX | DrivenTransformProperties.AnchoredPositionX);
                this.m_HorizontalScrollbarRect.anchorMin = new Vector2(0f, this.m_HorizontalScrollbarRect.anchorMin.y);
                this.m_HorizontalScrollbarRect.anchorMax = new Vector2(1f, this.m_HorizontalScrollbarRect.anchorMax.y);
                this.m_HorizontalScrollbarRect.anchoredPosition = new Vector2(0f, this.m_HorizontalScrollbarRect.anchoredPosition.y);
                if (this.vScrollingNeeded)
                {
                    this.m_HorizontalScrollbarRect.sizeDelta = new Vector2(-(this.m_VSliderWidth + this.m_VerticalScrollbarSpacing), this.m_HorizontalScrollbarRect.sizeDelta.y);
                }
                else
                {
                    this.m_HorizontalScrollbarRect.sizeDelta = new Vector2(0f, this.m_HorizontalScrollbarRect.sizeDelta.y);
                }
            }
            if (this.m_HSliderExpand && (this.m_VerticalScrollbar != null))
            {
                this.m_Tracker.Add(this, this.m_VerticalScrollbarRect, DrivenTransformProperties.SizeDeltaY | DrivenTransformProperties.AnchorMaxY | DrivenTransformProperties.AnchorMinY | DrivenTransformProperties.AnchoredPositionY);
                this.m_VerticalScrollbarRect.anchorMin = new Vector2(this.m_VerticalScrollbarRect.anchorMin.x, 0f);
                this.m_VerticalScrollbarRect.anchorMax = new Vector2(this.m_VerticalScrollbarRect.anchorMax.x, 1f);
                this.m_VerticalScrollbarRect.anchoredPosition = new Vector2(this.m_VerticalScrollbarRect.anchoredPosition.x, 0f);
                if (this.hScrollingNeeded)
                {
                    this.m_VerticalScrollbarRect.sizeDelta = new Vector2(this.m_VerticalScrollbarRect.sizeDelta.x, -(this.m_HSliderHeight + this.m_HorizontalScrollbarSpacing));
                }
                else
                {
                    this.m_VerticalScrollbarRect.sizeDelta = new Vector2(this.m_VerticalScrollbarRect.sizeDelta.x, 0f);
                }
            }
        }

        private void UpdateScrollbars(Vector2 offset)
        {
            if (this.m_HorizontalScrollbar != null)
            {
                if (this.m_ContentBounds.size.x > 0f)
                {
                    this.m_HorizontalScrollbar.size = Mathf.Clamp01((this.m_ViewBounds.size.x - Mathf.Abs(offset.x)) / this.m_ContentBounds.size.x);
                }
                else
                {
                    this.m_HorizontalScrollbar.size = 1f;
                }
                this.m_HorizontalScrollbar.value = this.horizontalNormalizedPosition;
            }
            if (this.m_VerticalScrollbar != null)
            {
                if (this.m_ContentBounds.size.y > 0f)
                {
                    this.m_VerticalScrollbar.size = Mathf.Clamp01((this.m_ViewBounds.size.y - Mathf.Abs(offset.y)) / this.m_ContentBounds.size.y);
                }
                else
                {
                    this.m_VerticalScrollbar.size = 1f;
                }
                this.m_VerticalScrollbar.value = this.verticalNormalizedPosition;
            }
        }

        private void UpdateScrollbarVisibility()
        {
            if (((this.m_VerticalScrollbar != null) && (this.m_VerticalScrollbarVisibility != ScrollbarVisibility.Permanent)) && (this.m_VerticalScrollbar.gameObject.activeSelf != this.vScrollingNeeded))
            {
                this.m_VerticalScrollbar.gameObject.SetActive(this.vScrollingNeeded);
            }
            if (((this.m_HorizontalScrollbar != null) && (this.m_HorizontalScrollbarVisibility != ScrollbarVisibility.Permanent)) && (this.m_HorizontalScrollbar.gameObject.activeSelf != this.hScrollingNeeded))
            {
                this.m_HorizontalScrollbar.gameObject.SetActive(this.hScrollingNeeded);
            }
        }

        public RectTransform content
        {
            get
            {
                return this.m_Content;
            }
            set
            {
                this.m_Content = value;
            }
        }

        public float decelerationRate
        {
            get
            {
                return this.m_DecelerationRate;
            }
            set
            {
                this.m_DecelerationRate = value;
            }
        }

        public float elasticity
        {
            get
            {
                return this.m_Elasticity;
            }
            set
            {
                this.m_Elasticity = value;
            }
        }

        public bool horizontal
        {
            get
            {
                return this.m_Horizontal;
            }
            set
            {
                this.m_Horizontal = value;
            }
        }

        public float horizontalNormalizedPosition
        {
            get
            {
                this.UpdateBounds();
                if (this.m_ContentBounds.size.x <= this.m_ViewBounds.size.x)
                {
                    return ((this.m_ViewBounds.min.x <= this.m_ContentBounds.min.x) ? ((float) 0) : ((float) 1));
                }
                return ((this.m_ViewBounds.min.x - this.m_ContentBounds.min.x) / (this.m_ContentBounds.size.x - this.m_ViewBounds.size.x));
            }
            set
            {
                this.SetNormalizedPosition(value, 0);
            }
        }

        public Scrollbar horizontalScrollbar
        {
            get
            {
                return this.m_HorizontalScrollbar;
            }
            set
            {
                if (this.m_HorizontalScrollbar != null)
                {
                    this.m_HorizontalScrollbar.onValueChanged.RemoveListener(new UnityAction<float>(this.SetHorizontalNormalizedPosition));
                }
                this.m_HorizontalScrollbar = value;
                if (this.m_HorizontalScrollbar != null)
                {
                    this.m_HorizontalScrollbar.onValueChanged.AddListener(new UnityAction<float>(this.SetHorizontalNormalizedPosition));
                }
                this.SetDirtyCaching();
            }
        }

        public float horizontalScrollbarSpacing
        {
            get
            {
                return this.m_HorizontalScrollbarSpacing;
            }
            set
            {
                this.m_HorizontalScrollbarSpacing = value;
                this.SetDirty();
            }
        }

        public ScrollbarVisibility horizontalScrollbarVisibility
        {
            get
            {
                return this.m_HorizontalScrollbarVisibility;
            }
            set
            {
                this.m_HorizontalScrollbarVisibility = value;
                this.SetDirtyCaching();
            }
        }

        private bool hScrollingNeeded
        {
            get
            {
                if (Application.isPlaying)
                {
                    return (this.m_ContentBounds.size.x > (this.m_ViewBounds.size.x + 0.01f));
                }
                return true;
            }
        }

        public bool inertia
        {
            get
            {
                return this.m_Inertia;
            }
            set
            {
                this.m_Inertia = value;
            }
        }

        public MovementType movementType
        {
            get
            {
                return this.m_MovementType;
            }
            set
            {
                this.m_MovementType = value;
            }
        }

        public Vector2 normalizedPosition
        {
            get
            {
                return new Vector2(this.horizontalNormalizedPosition, this.verticalNormalizedPosition);
            }
            set
            {
                this.SetNormalizedPosition(value.x, 0);
                this.SetNormalizedPosition(value.y, 1);
            }
        }

        public ScrollRectEvent onValueChanged
        {
            get
            {
                return this.m_OnValueChanged;
            }
            set
            {
                this.m_OnValueChanged = value;
            }
        }

        private RectTransform rectTransform
        {
            get
            {
                if (this.m_Rect == null)
                {
                    this.m_Rect = base.GetComponent<RectTransform>();
                }
                return this.m_Rect;
            }
        }

        public float scrollSensitivity
        {
            get
            {
                return this.m_ScrollSensitivity;
            }
            set
            {
                this.m_ScrollSensitivity = value;
            }
        }

        public Vector2 velocity
        {
            get
            {
                return this.m_Velocity;
            }
            set
            {
                this.m_Velocity = value;
            }
        }

        public bool vertical
        {
            get
            {
                return this.m_Vertical;
            }
            set
            {
                this.m_Vertical = value;
            }
        }

        public float verticalNormalizedPosition
        {
            get
            {
                this.UpdateBounds();
                if (this.m_ContentBounds.size.y <= this.m_ViewBounds.size.y)
                {
                    return ((this.m_ViewBounds.min.y <= this.m_ContentBounds.min.y) ? ((float) 0) : ((float) 1));
                }
                return ((this.m_ViewBounds.min.y - this.m_ContentBounds.min.y) / (this.m_ContentBounds.size.y - this.m_ViewBounds.size.y));
            }
            set
            {
                this.SetNormalizedPosition(value, 1);
            }
        }

        public Scrollbar verticalScrollbar
        {
            get
            {
                return this.m_VerticalScrollbar;
            }
            set
            {
                if (this.m_VerticalScrollbar != null)
                {
                    this.m_VerticalScrollbar.onValueChanged.RemoveListener(new UnityAction<float>(this.SetVerticalNormalizedPosition));
                }
                this.m_VerticalScrollbar = value;
                if (this.m_VerticalScrollbar != null)
                {
                    this.m_VerticalScrollbar.onValueChanged.AddListener(new UnityAction<float>(this.SetVerticalNormalizedPosition));
                }
                this.SetDirtyCaching();
            }
        }

        public float verticalScrollbarSpacing
        {
            get
            {
                return this.m_VerticalScrollbarSpacing;
            }
            set
            {
                this.m_VerticalScrollbarSpacing = value;
                this.SetDirty();
            }
        }

        public ScrollbarVisibility verticalScrollbarVisibility
        {
            get
            {
                return this.m_VerticalScrollbarVisibility;
            }
            set
            {
                this.m_VerticalScrollbarVisibility = value;
                this.SetDirtyCaching();
            }
        }

        public RectTransform viewport
        {
            get
            {
                return this.m_Viewport;
            }
            set
            {
                this.m_Viewport = value;
                this.SetDirtyCaching();
            }
        }

        protected RectTransform viewRect
        {
            get
            {
                if (this.m_ViewRect == null)
                {
                    this.m_ViewRect = this.m_Viewport;
                }
                if (this.m_ViewRect == null)
                {
                    this.m_ViewRect = (RectTransform) base.transform;
                }
                return this.m_ViewRect;
            }
        }

        private bool vScrollingNeeded
        {
            get
            {
                if (Application.isPlaying)
                {
                    return (this.m_ContentBounds.size.y > (this.m_ViewBounds.size.y + 0.01f));
                }
                return true;
            }
        }

        public enum MovementType
        {
            Unrestricted,
            Elastic,
            Clamped
        }

        public enum ScrollbarVisibility
        {
            Permanent,
            AutoHide,
            AutoHideAndExpandViewport
        }

        [Serializable]
        public class ScrollRectEvent : UnityEvent<Vector2>
        {
        }
    }
}

