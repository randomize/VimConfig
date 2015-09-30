namespace UnityEngine.EventSystems
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Text;
    using UnityEngine;

    public class PointerEventData : BaseEventData
    {
        public List<GameObject> hovered;
        private GameObject m_PointerPress;

        public PointerEventData(EventSystem eventSystem) : base(eventSystem)
        {
            this.hovered = new List<GameObject>();
            this.eligibleForClick = false;
            this.pointerId = -1;
            this.position = Vector2.zero;
            this.delta = Vector2.zero;
            this.pressPosition = Vector2.zero;
            this.clickTime = 0f;
            this.clickCount = 0;
            this.scrollDelta = Vector2.zero;
            this.useDragThreshold = true;
            this.dragging = false;
            this.button = InputButton.Left;
        }

        public bool IsPointerMoving()
        {
            return (this.delta.sqrMagnitude > 0f);
        }

        public bool IsScrolling()
        {
            return (this.scrollDelta.sqrMagnitude > 0f);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<b>Position</b>: " + this.position);
            builder.AppendLine("<b>delta</b>: " + this.delta);
            builder.AppendLine("<b>eligibleForClick</b>: " + this.eligibleForClick);
            builder.AppendLine("<b>pointerEnter</b>: " + this.pointerEnter);
            builder.AppendLine("<b>pointerPress</b>: " + this.pointerPress);
            builder.AppendLine("<b>lastPointerPress</b>: " + this.lastPress);
            builder.AppendLine("<b>pointerDrag</b>: " + this.pointerDrag);
            builder.AppendLine("<b>Use Drag Threshold</b>: " + this.useDragThreshold);
            builder.AppendLine("<b>Current Rayast:</b>");
            builder.AppendLine(this.pointerCurrentRaycast.ToString());
            builder.AppendLine("<b>Press Rayast:</b>");
            builder.AppendLine(this.pointerPressRaycast.ToString());
            return builder.ToString();
        }

        public InputButton button { get; set; }

        public int clickCount { get; set; }

        public float clickTime { get; set; }

        public Vector2 delta { get; set; }

        public bool dragging { get; set; }

        public bool eligibleForClick { get; set; }

        public Camera enterEventCamera
        {
            get
            {
                return ((this.pointerCurrentRaycast.module != null) ? this.pointerCurrentRaycast.module.eventCamera : null);
            }
        }

        public GameObject lastPress { get; private set; }

        public RaycastResult pointerCurrentRaycast { get; set; }

        public GameObject pointerDrag { get; set; }

        public GameObject pointerEnter { get; set; }

        public int pointerId { get; set; }

        public GameObject pointerPress
        {
            get
            {
                return this.m_PointerPress;
            }
            set
            {
                if (this.m_PointerPress != value)
                {
                    this.lastPress = this.m_PointerPress;
                    this.m_PointerPress = value;
                }
            }
        }

        public RaycastResult pointerPressRaycast { get; set; }

        public Vector2 position { get; set; }

        public Camera pressEventCamera
        {
            get
            {
                return ((this.pointerPressRaycast.module != null) ? this.pointerPressRaycast.module.eventCamera : null);
            }
        }

        public Vector2 pressPosition { get; set; }

        public GameObject rawPointerPress { get; set; }

        public Vector2 scrollDelta { get; set; }

        public bool useDragThreshold { get; set; }

        [Obsolete("Use either pointerCurrentRaycast.worldNormal or pointerPressRaycast.worldNormal")]
        public Vector3 worldNormal { get; set; }

        [Obsolete("Use either pointerCurrentRaycast.worldPosition or pointerPressRaycast.worldPosition")]
        public Vector3 worldPosition { get; set; }

        public enum FramePressState
        {
            Pressed,
            Released,
            PressedAndReleased,
            NotChanged
        }

        public enum InputButton
        {
            Left,
            Right,
            Middle
        }
    }
}

