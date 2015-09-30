namespace UnityEngine.EventSystems
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;
    using UnityEngine.Serialization;

    [AddComponentMenu("Event/Touch Input Module")]
    public class TouchInputModule : PointerInputModule
    {
        [FormerlySerializedAs("m_AllowActivationOnStandalone"), SerializeField]
        private bool m_ForceModuleActive;
        private Vector2 m_LastMousePosition;
        private Vector2 m_MousePosition;

        protected TouchInputModule()
        {
        }

        public override void DeactivateModule()
        {
            base.DeactivateModule();
            base.ClearSelection();
        }

        private void FakeTouches()
        {
            PointerInputModule.MouseButtonEventData eventData = this.GetMousePointerEventData(0).GetButtonState(PointerEventData.InputButton.Left).eventData;
            if (eventData.PressedThisFrame())
            {
                eventData.buttonData.delta = Vector2.zero;
            }
            this.ProcessTouchPress(eventData.buttonData, eventData.PressedThisFrame(), eventData.ReleasedThisFrame());
            if (Input.GetMouseButton(0))
            {
                this.ProcessMove(eventData.buttonData);
                this.ProcessDrag(eventData.buttonData);
            }
        }

        public override bool IsModuleSupported()
        {
            return (this.forceModuleActive || Input.touchSupported);
        }

        public override void Process()
        {
            if (this.UseFakeInput())
            {
                this.FakeTouches();
            }
            else
            {
                this.ProcessTouchEvents();
            }
        }

        private void ProcessTouchEvents()
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                bool flag;
                bool flag2;
                Touch input = Input.GetTouch(i);
                PointerEventData pointerEvent = base.GetTouchPointerEventData(input, out flag2, out flag);
                this.ProcessTouchPress(pointerEvent, flag2, flag);
                if (!flag)
                {
                    this.ProcessMove(pointerEvent);
                    this.ProcessDrag(pointerEvent);
                }
                else
                {
                    base.RemovePointerData(pointerEvent);
                }
            }
        }

        private void ProcessTouchPress(PointerEventData pointerEvent, bool pressed, bool released)
        {
            GameObject gameObject = pointerEvent.pointerCurrentRaycast.gameObject;
            if (pressed)
            {
                pointerEvent.eligibleForClick = true;
                pointerEvent.delta = Vector2.zero;
                pointerEvent.dragging = false;
                pointerEvent.useDragThreshold = true;
                pointerEvent.pressPosition = pointerEvent.position;
                pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;
                base.DeselectIfSelectionChanged(gameObject, pointerEvent);
                if (pointerEvent.pointerEnter != gameObject)
                {
                    base.HandlePointerExitAndEnter(pointerEvent, gameObject);
                    pointerEvent.pointerEnter = gameObject;
                }
                GameObject eventHandler = ExecuteEvents.ExecuteHierarchy<IPointerDownHandler>(gameObject, pointerEvent, ExecuteEvents.pointerDownHandler);
                if (eventHandler == null)
                {
                    eventHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
                }
                float unscaledTime = Time.unscaledTime;
                if (eventHandler == pointerEvent.lastPress)
                {
                    float num2 = unscaledTime - pointerEvent.clickTime;
                    if (num2 < 0.3f)
                    {
                        pointerEvent.clickCount++;
                    }
                    else
                    {
                        pointerEvent.clickCount = 1;
                    }
                    pointerEvent.clickTime = unscaledTime;
                }
                else
                {
                    pointerEvent.clickCount = 1;
                }
                pointerEvent.pointerPress = eventHandler;
                pointerEvent.rawPointerPress = gameObject;
                pointerEvent.clickTime = unscaledTime;
                pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(gameObject);
                if (pointerEvent.pointerDrag != null)
                {
                    ExecuteEvents.Execute<IInitializePotentialDragHandler>(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);
                }
            }
            if (released)
            {
                ExecuteEvents.Execute<IPointerUpHandler>(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);
                GameObject obj4 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
                if ((pointerEvent.pointerPress == obj4) && pointerEvent.eligibleForClick)
                {
                    ExecuteEvents.Execute<IPointerClickHandler>(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
                }
                else if ((pointerEvent.pointerDrag != null) && pointerEvent.dragging)
                {
                    ExecuteEvents.ExecuteHierarchy<IDropHandler>(gameObject, pointerEvent, ExecuteEvents.dropHandler);
                }
                pointerEvent.eligibleForClick = false;
                pointerEvent.pointerPress = null;
                pointerEvent.rawPointerPress = null;
                if ((pointerEvent.pointerDrag != null) && pointerEvent.dragging)
                {
                    ExecuteEvents.Execute<IEndDragHandler>(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);
                }
                pointerEvent.dragging = false;
                pointerEvent.pointerDrag = null;
                if (pointerEvent.pointerDrag != null)
                {
                    ExecuteEvents.Execute<IEndDragHandler>(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);
                }
                pointerEvent.pointerDrag = null;
                ExecuteEvents.ExecuteHierarchy<IPointerExitHandler>(pointerEvent.pointerEnter, pointerEvent, ExecuteEvents.pointerExitHandler);
                pointerEvent.pointerEnter = null;
            }
        }

        public override bool ShouldActivateModule()
        {
            if (base.ShouldActivateModule())
            {
                if (this.m_ForceModuleActive)
                {
                    return true;
                }
                if (this.UseFakeInput())
                {
                    Vector2 vector = this.m_MousePosition - this.m_LastMousePosition;
                    return (Input.GetMouseButtonDown(0) | (vector.sqrMagnitude > 0f));
                }
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch touch = Input.GetTouch(i);
                    if (((touch.phase == TouchPhase.Began) || (touch.phase == TouchPhase.Moved)) || (touch.phase == TouchPhase.Stationary))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(!this.UseFakeInput() ? "Input: Touch" : "Input: Faked");
            if (this.UseFakeInput())
            {
                PointerEventData lastPointerEventData = base.GetLastPointerEventData(-1);
                if (lastPointerEventData != null)
                {
                    builder.AppendLine(lastPointerEventData.ToString());
                }
            }
            else
            {
                foreach (KeyValuePair<int, PointerEventData> pair in base.m_PointerData)
                {
                    builder.AppendLine(pair.ToString());
                }
            }
            return builder.ToString();
        }

        public override void UpdateModule()
        {
            this.m_LastMousePosition = this.m_MousePosition;
            this.m_MousePosition = Input.mousePosition;
        }

        private bool UseFakeInput()
        {
            return !Input.touchSupported;
        }

        [Obsolete("allowActivationOnStandalone has been deprecated. Use forceModuleActive instead (UnityUpgradable) -> forceModuleActive")]
        public bool allowActivationOnStandalone
        {
            get
            {
                return this.m_ForceModuleActive;
            }
            set
            {
                this.m_ForceModuleActive = value;
            }
        }

        public bool forceModuleActive
        {
            get
            {
                return this.m_ForceModuleActive;
            }
            set
            {
                this.m_ForceModuleActive = value;
            }
        }
    }
}

