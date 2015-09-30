namespace UnityEngine.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    [RequireComponent(typeof(RectTransform)), AddComponentMenu("UI/Scrollbar", 0x22)]
    public class Scrollbar : Selectable, IEventSystemHandler, IBeginDragHandler, IInitializePotentialDragHandler, IDragHandler, ICanvasElement
    {
        private bool isPointerDownAndNotDragging;
        private RectTransform m_ContainerRect;
        [SerializeField]
        private Direction m_Direction;
        [SerializeField]
        private RectTransform m_HandleRect;
        [SerializeField, Range(0f, 11f)]
        private int m_NumberOfSteps;
        private Vector2 m_Offset = Vector2.zero;
        [Space(6f), SerializeField]
        private ScrollEvent m_OnValueChanged = new ScrollEvent();
        private Coroutine m_PointerDownRepeat;
        [SerializeField, Range(0f, 1f)]
        private float m_Size = 0.2f;
        private DrivenRectTransformTracker m_Tracker;
        [Range(0f, 1f), SerializeField]
        private float m_Value;

        protected Scrollbar()
        {
        }

        [DebuggerHidden]
        protected IEnumerator ClickRepeat(PointerEventData eventData)
        {
            return new <ClickRepeat>c__Iterator5 { eventData = eventData, <$>eventData = eventData, <>f__this = this };
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

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            this.isPointerDownAndNotDragging = false;
            if (this.MayDrag(eventData) && (this.m_ContainerRect != null))
            {
                Vector2 vector;
                this.m_Offset = Vector2.zero;
                if (RectTransformUtility.RectangleContainsScreenPoint(this.m_HandleRect, eventData.position, eventData.enterEventCamera) && RectTransformUtility.ScreenPointToLocalPointInRectangle(this.m_HandleRect, eventData.position, eventData.pressEventCamera, out vector))
                {
                    this.m_Offset = vector - this.m_HandleRect.rect.center;
                }
            }
        }

        protected override void OnDisable()
        {
            this.m_Tracker.Clear();
            base.OnDisable();
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (this.MayDrag(eventData) && (this.m_ContainerRect != null))
            {
                this.UpdateDrag(eventData);
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
                this.isPointerDownAndNotDragging = true;
                this.m_PointerDownRepeat = base.StartCoroutine(this.ClickRepeat(eventData));
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            this.isPointerDownAndNotDragging = false;
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
            this.m_Size = Mathf.Clamp01(this.m_Size);
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

        private void Set(float input, bool sendCallback)
        {
            float num = this.m_Value;
            this.m_Value = Mathf.Clamp01(input);
            if (num != this.value)
            {
                this.UpdateVisuals();
                if (sendCallback)
                {
                    this.m_OnValueChanged.Invoke(this.value);
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
            if ((this.m_HandleRect != null) && (this.m_HandleRect.parent != null))
            {
                this.m_ContainerRect = this.m_HandleRect.parent.GetComponent<RectTransform>();
            }
            else
            {
                this.m_ContainerRect = null;
            }
        }

        private void UpdateDrag(PointerEventData eventData)
        {
            Vector2 vector;
            if (((eventData.button == PointerEventData.InputButton.Left) && (this.m_ContainerRect != null)) && RectTransformUtility.ScreenPointToLocalPointInRectangle(this.m_ContainerRect, eventData.position, eventData.pressEventCamera, out vector))
            {
                Vector2 vector2 = (vector - this.m_Offset) - this.m_ContainerRect.rect.position;
                Vector2 vector3 = vector2 - ((Vector2) ((this.m_HandleRect.rect.size - this.m_HandleRect.sizeDelta) * 0.5f));
                float num = (this.axis != Axis.Horizontal) ? this.m_ContainerRect.rect.height : this.m_ContainerRect.rect.width;
                float num2 = num * (1f - this.size);
                if (num2 > 0f)
                {
                    switch (this.m_Direction)
                    {
                        case Direction.LeftToRight:
                            this.Set(vector3.x / num2);
                            break;

                        case Direction.RightToLeft:
                            this.Set(1f - (vector3.x / num2));
                            break;

                        case Direction.BottomToTop:
                            this.Set(vector3.y / num2);
                            break;

                        case Direction.TopToBottom:
                            this.Set(1f - (vector3.y / num2));
                            break;
                    }
                }
            }
        }

        private void UpdateVisuals()
        {
            if (!Application.isPlaying)
            {
                this.UpdateCachedReferences();
            }
            this.m_Tracker.Clear();
            if (this.m_ContainerRect != null)
            {
                this.m_Tracker.Add(this, this.m_HandleRect, DrivenTransformProperties.Anchors);
                Vector2 zero = Vector2.zero;
                Vector2 one = Vector2.one;
                float num = this.value * (1f - this.size);
                if (this.reverseValue)
                {
                    zero[(int) this.axis] = (1f - num) - this.size;
                    one[(int) this.axis] = 1f - num;
                }
                else
                {
                    zero[(int) this.axis] = num;
                    one[(int) this.axis] = num + this.size;
                }
                this.m_HandleRect.anchorMin = zero;
                this.m_HandleRect.anchorMax = one;
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

        public int numberOfSteps
        {
            get
            {
                return this.m_NumberOfSteps;
            }
            set
            {
                if (SetPropertyUtility.SetStruct<int>(ref this.m_NumberOfSteps, value))
                {
                    this.Set(this.m_Value);
                    this.UpdateVisuals();
                }
            }
        }

        public ScrollEvent onValueChanged
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

        public float size
        {
            get
            {
                return this.m_Size;
            }
            set
            {
                if (SetPropertyUtility.SetStruct<float>(ref this.m_Size, Mathf.Clamp01(value)))
                {
                    this.UpdateVisuals();
                }
            }
        }

        private float stepSize
        {
            get
            {
                return ((this.m_NumberOfSteps <= 1) ? 0.1f : (1f / ((float) (this.m_NumberOfSteps - 1))));
            }
        }

        public float value
        {
            get
            {
                float num = this.m_Value;
                if (this.m_NumberOfSteps > 1)
                {
                    num = Mathf.Round(num * (this.m_NumberOfSteps - 1)) / ((float) (this.m_NumberOfSteps - 1));
                }
                return num;
            }
            set
            {
                this.Set(value);
            }
        }

        [CompilerGenerated]
        private sealed class <ClickRepeat>c__Iterator5 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal PointerEventData <$>eventData;
            internal Scrollbar <>f__this;
            internal float <axisCoordinate>__1;
            internal Vector2 <localMousePos>__0;
            internal PointerEventData eventData;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                    case 1:
                        if (this.<>f__this.isPointerDownAndNotDragging)
                        {
                            if (!RectTransformUtility.RectangleContainsScreenPoint(this.<>f__this.m_HandleRect, this.eventData.position, this.eventData.enterEventCamera) && RectTransformUtility.ScreenPointToLocalPointInRectangle(this.<>f__this.m_HandleRect, this.eventData.position, this.eventData.pressEventCamera, out this.<localMousePos>__0))
                            {
                                this.<axisCoordinate>__1 = (this.<>f__this.axis != Scrollbar.Axis.Horizontal) ? this.<localMousePos>__0.y : this.<localMousePos>__0.x;
                                if (this.<axisCoordinate>__1 < 0f)
                                {
                                    this.<>f__this.value -= this.<>f__this.size;
                                }
                                else
                                {
                                    this.<>f__this.value += this.<>f__this.size;
                                }
                            }
                            this.$current = new WaitForEndOfFrame();
                            this.$PC = 1;
                            return true;
                        }
                        this.<>f__this.StopCoroutine(this.<>f__this.m_PointerDownRepeat);
                        this.$PC = -1;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
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
        public class ScrollEvent : UnityEvent<float>
        {
        }
    }
}

