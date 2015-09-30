namespace UnityEngine.UI
{
    using System;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    [RequireComponent(typeof(RectTransform)), AddComponentMenu("UI/Slider", 0x21)]
    public class Slider : Selectable, IEventSystemHandler, IInitializePotentialDragHandler, IDragHandler, ICanvasElement
    {
        [Space, SerializeField]
        private Direction m_Direction;
        private RectTransform m_FillContainerRect;
        private Image m_FillImage;
        [SerializeField]
        private RectTransform m_FillRect;
        private Transform m_FillTransform;
        private RectTransform m_HandleContainerRect;
        [SerializeField]
        private RectTransform m_HandleRect;
        private Transform m_HandleTransform;
        [SerializeField]
        private float m_MaxValue = 1f;
        [SerializeField]
        private float m_MinValue;
        private Vector2 m_Offset = Vector2.zero;
        [SerializeField, Space]
        private SliderEvent m_OnValueChanged = new SliderEvent();
        private DrivenRectTransformTracker m_Tracker;
        [SerializeField]
        protected float m_Value;
        [SerializeField]
        private bool m_WholeNumbers;

        protected Slider()
        {
        }

        private float ClampValue(float input)
        {
            float f = Mathf.Clamp(input, this.minValue, this.maxValue);
            if (this.wholeNumbers)
            {
                f = Mathf.Round(f);
            }
            return f;
        }

        public override Selectable FindSelectableOnDown()
        {
            if ((base.navigation.mode == Navigation.Mode.Automatic) && (this.axis == Axis.Vertical))
            {
                return null;
            }
            return base.FindSelectableOnDown();
        }

        public override Selectable FindSelectableOnLeft()
        {
            if ((base.navigation.mode == Navigation.Mode.Automatic) && (this.axis == Axis.Horizontal))
            {
                return null;
            }
            return base.FindSelectableOnLeft();
        }

        public override Selectable FindSelectableOnRight()
        {
            if ((base.navigation.mode == Navigation.Mode.Automatic) && (this.axis == Axis.Horizontal))
            {
                return null;
            }
            return base.FindSelectableOnRight();
        }

        public override Selectable FindSelectableOnUp()
        {
            if ((base.navigation.mode == Navigation.Mode.Automatic) && (this.axis == Axis.Vertical))
            {
                return null;
            }
            return base.FindSelectableOnUp();
        }

        private bool MayDrag(PointerEventData eventData)
        {
            return ((this.IsActive() && this.IsInteractable()) && (eventData.button == PointerEventData.InputButton.Left));
        }

        protected override void OnDidApplyAnimationProperties()
        {
            this.m_Value = this.ClampValue(this.m_Value);
            float normalizedValue = this.normalizedValue;
            if (this.m_FillContainerRect != null)
            {
                if ((this.m_FillImage != null) && (this.m_FillImage.type == Image.Type.Filled))
                {
                    normalizedValue = this.m_FillImage.fillAmount;
                }
                else
                {
                    normalizedValue = !this.reverseValue ? this.m_FillRect.anchorMax[(int) this.axis] : (1f - this.m_FillRect.anchorMin[(int) this.axis]);
                }
            }
            else if (this.m_HandleContainerRect != null)
            {
                normalizedValue = !this.reverseValue ? this.m_HandleRect.anchorMin[(int) this.axis] : (1f - this.m_HandleRect.anchorMin[(int) this.axis]);
            }
            this.UpdateVisuals();
            if (normalizedValue != this.normalizedValue)
            {
                this.onValueChanged.Invoke(this.m_Value);
            }
        }

        protected override void OnDisable()
        {
            this.m_Tracker.Clear();
            base.OnDisable();
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (this.MayDrag(eventData))
            {
                this.UpdateDrag(eventData, eventData.pressEventCamera);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.UpdateCachedReferences();
            this.Set(this.m_Value, false);
            this.UpdateVisuals();
        }

        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }

        public override void OnMove(AxisEventData eventData)
        {
            if (!this.IsActive() || !this.IsInteractable())
            {
                base.OnMove(eventData);
            }
            else
            {
                switch (eventData.moveDir)
                {
                    case MoveDirection.Left:
                        if ((this.axis != Axis.Horizontal) || (this.FindSelectableOnLeft() != null))
                        {
                            base.OnMove(eventData);
                            break;
                        }
                        this.Set(!this.reverseValue ? (this.value - this.stepSize) : (this.value + this.stepSize));
                        break;

                    case MoveDirection.Up:
                        if ((this.axis != Axis.Vertical) || (this.FindSelectableOnUp() != null))
                        {
                            base.OnMove(eventData);
                            break;
                        }
                        this.Set(!this.reverseValue ? (this.value + this.stepSize) : (this.value - this.stepSize));
                        break;

                    case MoveDirection.Right:
                        if ((this.axis != Axis.Horizontal) || (this.FindSelectableOnRight() != null))
                        {
                            base.OnMove(eventData);
                            break;
                        }
                        this.Set(!this.reverseValue ? (this.value + this.stepSize) : (this.value - this.stepSize));
                        break;

                    case MoveDirection.Down:
                        if ((this.axis != Axis.Vertical) || (this.FindSelectableOnDown() != null))
                        {
                            base.OnMove(eventData);
                            break;
                        }
                        this.Set(!this.reverseValue ? (this.value - this.stepSize) : (this.value + this.stepSize));
                        break;
                }
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (this.MayDrag(eventData))
            {
                base.OnPointerDown(eventData);
                this.m_Offset = Vector2.zero;
                if ((this.m_HandleContainerRect != null) && RectTransformUtility.RectangleContainsScreenPoint(this.m_HandleRect, eventData.position, eventData.enterEventCamera))
                {
                    Vector2 vector;
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(this.m_HandleRect, eventData.position, eventData.pressEventCamera, out vector))
                    {
                        this.m_Offset = vector;
                    }
                }
                else
                {
                    this.UpdateDrag(eventData, eventData.pressEventCamera);
                }
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            if (this.IsActive())
            {
                this.UpdateVisuals();
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            if (this.wholeNumbers)
            {
                this.m_MinValue = Mathf.Round(this.m_MinValue);
                this.m_MaxValue = Mathf.Round(this.m_MaxValue);
            }
            if (this.IsActive())
            {
                this.UpdateCachedReferences();
                this.Set(this.m_Value, false);
                this.UpdateVisuals();
            }
            if ((PrefabUtility.GetPrefabType(this) != PrefabType.Prefab) && !Application.isPlaying)
            {
                CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
            }
        }

        public virtual void Rebuild(CanvasUpdate executing)
        {
            if (executing == CanvasUpdate.Prelayout)
            {
                this.onValueChanged.Invoke(this.value);
            }
        }

        private void Set(float input)
        {
            this.Set(input, true);
        }

        protected virtual void Set(float input, bool sendCallback)
        {
            float num = this.ClampValue(input);
            if (this.m_Value != num)
            {
                this.m_Value = num;
                this.UpdateVisuals();
                if (sendCallback)
                {
                    this.m_OnValueChanged.Invoke(num);
                }
            }
        }

        public void SetDirection(Direction direction, bool includeRectLayouts)
        {
            Axis axis = this.axis;
            bool reverseValue = this.reverseValue;
            this.direction = direction;
            if (includeRectLayouts)
            {
                if (this.axis != axis)
                {
                    RectTransformUtility.FlipLayoutAxes(base.transform as RectTransform, true, true);
                }
                if (this.reverseValue != reverseValue)
                {
                    RectTransformUtility.FlipLayoutOnAxis(base.transform as RectTransform, (int) this.axis, true, true);
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

        private void UpdateCachedReferences()
        {
            if (this.m_FillRect != null)
            {
                this.m_FillTransform = this.m_FillRect.transform;
                this.m_FillImage = this.m_FillRect.GetComponent<Image>();
                if (this.m_FillTransform.parent != null)
                {
                    this.m_FillContainerRect = this.m_FillTransform.parent.GetComponent<RectTransform>();
                }
            }
            else
            {
                this.m_FillContainerRect = null;
                this.m_FillImage = null;
            }
            if (this.m_HandleRect != null)
            {
                this.m_HandleTransform = this.m_HandleRect.transform;
                if (this.m_HandleTransform.parent != null)
                {
                    this.m_HandleContainerRect = this.m_HandleTransform.parent.GetComponent<RectTransform>();
                }
            }
            else
            {
                this.m_HandleContainerRect = null;
            }
        }

        private void UpdateDrag(PointerEventData eventData, Camera cam)
        {
            RectTransform handleContainerRect;
            Vector2 vector;
            if (this.m_HandleContainerRect != null)
            {
                handleContainerRect = this.m_HandleContainerRect;
            }
            else
            {
                handleContainerRect = this.m_FillContainerRect;
            }
            if (((handleContainerRect != null) && (handleContainerRect.rect.size[(int) this.axis] > 0f)) && RectTransformUtility.ScreenPointToLocalPointInRectangle(handleContainerRect, eventData.position, cam, out vector))
            {
                vector -= handleContainerRect.rect.position;
                Vector2 vector3 = vector - this.m_Offset;
                float num = Mathf.Clamp01(vector3[(int) this.axis] / handleContainerRect.rect.size[(int) this.axis]);
                this.normalizedValue = !this.reverseValue ? num : (1f - num);
            }
        }

        private void UpdateVisuals()
        {
            if (!Application.isPlaying)
            {
                this.UpdateCachedReferences();
            }
            this.m_Tracker.Clear();
            if (this.m_FillContainerRect != null)
            {
                this.m_Tracker.Add(this, this.m_FillRect, DrivenTransformProperties.Anchors);
                Vector2 zero = Vector2.zero;
                Vector2 one = Vector2.one;
                if ((this.m_FillImage != null) && (this.m_FillImage.type == Image.Type.Filled))
                {
                    this.m_FillImage.fillAmount = this.normalizedValue;
                }
                else if (this.reverseValue)
                {
                    zero[(int) this.axis] = 1f - this.normalizedValue;
                }
                else
                {
                    one[(int) this.axis] = this.normalizedValue;
                }
                this.m_FillRect.anchorMin = zero;
                this.m_FillRect.anchorMax = one;
            }
            if (this.m_HandleContainerRect != null)
            {
                this.m_Tracker.Add(this, this.m_HandleRect, DrivenTransformProperties.Anchors);
                Vector2 vector3 = Vector2.zero;
                Vector2 vector4 = Vector2.one;
                float num = !this.reverseValue ? this.normalizedValue : (1f - this.normalizedValue);
                vector4[(int) this.axis] = num;
                vector3[(int) this.axis] = num;
                this.m_HandleRect.anchorMin = vector3;
                this.m_HandleRect.anchorMax = vector4;
            }
        }

        private Axis axis
        {
            get
            {
                return (((this.m_Direction != Direction.LeftToRight) && (this.m_Direction != Direction.RightToLeft)) ? Axis.Vertical : Axis.Horizontal);
            }
        }

        public Direction direction
        {
            get
            {
                return this.m_Direction;
            }
            set
            {
                if (SetPropertyUtility.SetStruct<Direction>(ref this.m_Direction, value))
                {
                    this.UpdateVisuals();
                }
            }
        }

        public RectTransform fillRect
        {
            get
            {
                return this.m_FillRect;
            }
            set
            {
                if (SetPropertyUtility.SetClass<RectTransform>(ref this.m_FillRect, value))
                {
                    this.UpdateCachedReferences();
                    this.UpdateVisuals();
                }
            }
        }

        public RectTransform handleRect
        {
            get
            {
                return this.m_HandleRect;
            }
            set
            {
                if (SetPropertyUtility.SetClass<RectTransform>(ref this.m_HandleRect, value))
                {
                    this.UpdateCachedReferences();
                    this.UpdateVisuals();
                }
            }
        }

        public float maxValue
        {
            get
            {
                return this.m_MaxValue;
            }
            set
            {
                if (SetPropertyUtility.SetStruct<float>(ref this.m_MaxValue, value))
                {
                    this.Set(this.m_Value);
                    this.UpdateVisuals();
                }
            }
        }

        public float minValue
        {
            get
            {
                return this.m_MinValue;
            }
            set
            {
                if (SetPropertyUtility.SetStruct<float>(ref this.m_MinValue, value))
                {
                    this.Set(this.m_Value);
                    this.UpdateVisuals();
                }
            }
        }

        public float normalizedValue
        {
            get
            {
                if (Mathf.Approximately(this.minValue, this.maxValue))
                {
                    return 0f;
                }
                return Mathf.InverseLerp(this.minValue, this.maxValue, this.value);
            }
            set
            {
                this.value = Mathf.Lerp(this.minValue, this.maxValue, value);
            }
        }

        public SliderEvent onValueChanged
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

        private bool reverseValue
        {
            get
            {
                return ((this.m_Direction == Direction.RightToLeft) || (this.m_Direction == Direction.TopToBottom));
            }
        }

        private float stepSize
        {
            get
            {
                return (!this.wholeNumbers ? ((this.maxValue - this.minValue) * 0.1f) : 1f);
            }
        }

        public virtual float value
        {
            get
            {
                if (this.wholeNumbers)
                {
                    return Mathf.Round(this.m_Value);
                }
                return this.m_Value;
            }
            set
            {
                this.Set(value);
            }
        }

        public bool wholeNumbers
        {
            get
            {
                return this.m_WholeNumbers;
            }
            set
            {
                if (SetPropertyUtility.SetStruct<bool>(ref this.m_WholeNumbers, value))
                {
                    this.Set(this.m_Value);
                    this.UpdateVisuals();
                }
            }
        }

        private enum Axis
        {
            Horizontal,
            Vertical
        }

        public enum Direction
        {
            LeftToRight,
            RightToLeft,
            BottomToTop,
            TopToBottom
        }

        [Serializable]
        public class SliderEvent : UnityEvent<float>
        {
        }
    }
}

