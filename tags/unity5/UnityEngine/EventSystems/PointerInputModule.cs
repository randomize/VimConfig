namespace UnityEngine.EventSystems
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEngine;

    public abstract class PointerInputModule : BaseInputModule
    {
        public const int kFakeTouchesId = -4;
        public const int kMouseLeftId = -1;
        public const int kMouseMiddleId = -3;
        public const int kMouseRightId = -2;
        private readonly MouseState m_MouseState = new MouseState();
        protected Dictionary<int, PointerEventData> m_PointerData = new Dictionary<int, PointerEventData>();

        protected PointerInputModule()
        {
        }

        protected void ClearSelection()
        {
            BaseEventData baseEventData = this.GetBaseEventData();
            foreach (PointerEventData data2 in this.m_PointerData.Values)
            {
                base.HandlePointerExitAndEnter(data2, null);
            }
            this.m_PointerData.Clear();
            base.eventSystem.SetSelectedGameObject(null, baseEventData);
        }

        protected void CopyFromTo(PointerEventData from, PointerEventData to)
        {
            to.position = from.position;
            to.delta = from.delta;
            to.scrollDelta = from.scrollDelta;
            to.pointerCurrentRaycast = from.pointerCurrentRaycast;
            to.pointerEnter = from.pointerEnter;
        }

        protected void DeselectIfSelectionChanged(GameObject currentOverGo, BaseEventData pointerEvent)
        {
            if (ExecuteEvents.GetEventHandler<ISelectHandler>(currentOverGo) != base.eventSystem.currentSelectedGameObject)
            {
                base.eventSystem.SetSelectedGameObject(null, pointerEvent);
            }
        }

        protected PointerEventData GetLastPointerEventData(int id)
        {
            PointerEventData data;
            this.GetPointerData(id, out data, false);
            return data;
        }

        protected virtual MouseState GetMousePointerEventData()
        {
            return this.GetMousePointerEventData(0);
        }

        protected virtual MouseState GetMousePointerEventData(int id)
        {
            PointerEventData data;
            PointerEventData data2;
            PointerEventData data3;
            bool flag = this.GetPointerData(-1, out data, true);
            data.Reset();
            if (flag)
            {
                data.position = Input.mousePosition;
            }
            Vector2 mousePosition = Input.mousePosition;
            data.delta = mousePosition - data.position;
            data.position = mousePosition;
            data.scrollDelta = Input.mouseScrollDelta;
            data.button = PointerEventData.InputButton.Left;
            base.eventSystem.RaycastAll(data, base.m_RaycastResultCache);
            RaycastResult result = BaseInputModule.FindFirstRaycast(base.m_RaycastResultCache);
            data.pointerCurrentRaycast = result;
            base.m_RaycastResultCache.Clear();
            this.GetPointerData(-2, out data2, true);
            this.CopyFromTo(data, data2);
            data2.button = PointerEventData.InputButton.Right;
            this.GetPointerData(-3, out data3, true);
            this.CopyFromTo(data, data3);
            data3.button = PointerEventData.InputButton.Middle;
            this.m_MouseState.SetButtonState(PointerEventData.InputButton.Left, StateForMouseButton(0), data);
            this.m_MouseState.SetButtonState(PointerEventData.InputButton.Right, StateForMouseButton(1), data2);
            this.m_MouseState.SetButtonState(PointerEventData.InputButton.Middle, StateForMouseButton(2), data3);
            return this.m_MouseState;
        }

        protected bool GetPointerData(int id, out PointerEventData data, bool create)
        {
            if (!this.m_PointerData.TryGetValue(id, out data) && create)
            {
                PointerEventData data2 = new PointerEventData(base.eventSystem) {
                    pointerId = id
                };
                data = data2;
                this.m_PointerData.Add(id, data);
                return true;
            }
            return false;
        }

        protected PointerEventData GetTouchPointerEventData(Touch input, out bool pressed, out bool released)
        {
            PointerEventData data;
            bool flag = this.GetPointerData(input.fingerId, out data, true);
            data.Reset();
            pressed = flag || (input.phase == TouchPhase.Began);
            released = (input.phase == TouchPhase.Canceled) || (input.phase == TouchPhase.Ended);
            if (flag)
            {
                data.position = input.position;
            }
            if (pressed)
            {
                data.delta = Vector2.zero;
            }
            else
            {
                data.delta = input.position - data.position;
            }
            data.position = input.position;
            data.button = PointerEventData.InputButton.Left;
            base.eventSystem.RaycastAll(data, base.m_RaycastResultCache);
            RaycastResult result = BaseInputModule.FindFirstRaycast(base.m_RaycastResultCache);
            data.pointerCurrentRaycast = result;
            base.m_RaycastResultCache.Clear();
            return data;
        }

        public override bool IsPointerOverGameObject(int pointerId)
        {
            PointerEventData lastPointerEventData = this.GetLastPointerEventData(pointerId);
            return ((lastPointerEventData != null) && (lastPointerEventData.pointerEnter != null));
        }

        protected virtual void ProcessDrag(PointerEventData pointerEvent)
        {
            bool flag = pointerEvent.IsPointerMoving();
            if ((flag && (pointerEvent.pointerDrag != null)) && (!pointerEvent.dragging && ShouldStartDrag(pointerEvent.pressPosition, pointerEvent.position, (float) base.eventSystem.pixelDragThreshold, pointerEvent.useDragThreshold)))
            {
                ExecuteEvents.Execute<IBeginDragHandler>(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.beginDragHandler);
                pointerEvent.dragging = true;
            }
            if ((pointerEvent.dragging && flag) && (pointerEvent.pointerDrag != null))
            {
                if (pointerEvent.pointerPress != pointerEvent.pointerDrag)
                {
                    ExecuteEvents.Execute<IPointerUpHandler>(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);
                    pointerEvent.eligibleForClick = false;
                    pointerEvent.pointerPress = null;
                    pointerEvent.rawPointerPress = null;
                }
                ExecuteEvents.Execute<IDragHandler>(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.dragHandler);
            }
        }

        protected virtual void ProcessMove(PointerEventData pointerEvent)
        {
            GameObject gameObject = pointerEvent.pointerCurrentRaycast.gameObject;
            base.HandlePointerExitAndEnter(pointerEvent, gameObject);
        }

        protected void RemovePointerData(PointerEventData data)
        {
            this.m_PointerData.Remove(data.pointerId);
        }

        private static bool ShouldStartDrag(Vector2 pressPos, Vector2 currentPos, float threshold, bool useDragThreshold)
        {
            if (!useDragThreshold)
            {
                return true;
            }
            Vector2 vector = pressPos - currentPos;
            return (vector.sqrMagnitude >= (threshold * threshold));
        }

        protected static PointerEventData.FramePressState StateForMouseButton(int buttonId)
        {
            bool mouseButtonDown = Input.GetMouseButtonDown(buttonId);
            bool mouseButtonUp = Input.GetMouseButtonUp(buttonId);
            if (mouseButtonDown && mouseButtonUp)
            {
                return PointerEventData.FramePressState.PressedAndReleased;
            }
            if (mouseButtonDown)
            {
                return PointerEventData.FramePressState.Pressed;
            }
            if (mouseButtonUp)
            {
                return PointerEventData.FramePressState.Released;
            }
            return PointerEventData.FramePressState.NotChanged;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("<b>Pointer Input Module of type: </b>" + base.GetType());
            builder.AppendLine();
            foreach (KeyValuePair<int, PointerEventData> pair in this.m_PointerData)
            {
                if (pair.Value != null)
                {
                    builder.AppendLine("<B>Pointer:</b> " + pair.Key);
                    builder.AppendLine(pair.Value.ToString());
                }
            }
            return builder.ToString();
        }

        protected class ButtonState
        {
            private PointerEventData.InputButton m_Button;
            private PointerInputModule.MouseButtonEventData m_EventData;

            public PointerEventData.InputButton button
            {
                get
                {
                    return this.m_Button;
                }
                set
                {
                    this.m_Button = value;
                }
            }

            public PointerInputModule.MouseButtonEventData eventData
            {
                get
                {
                    return this.m_EventData;
                }
                set
                {
                    this.m_EventData = value;
                }
            }
        }

        public class MouseButtonEventData
        {
            public PointerEventData buttonData;
            public PointerEventData.FramePressState buttonState;

            public bool PressedThisFrame()
            {
                return ((this.buttonState == PointerEventData.FramePressState.Pressed) || (this.buttonState == PointerEventData.FramePressState.PressedAndReleased));
            }

            public bool ReleasedThisFrame()
            {
                return ((this.buttonState == PointerEventData.FramePressState.Released) || (this.buttonState == PointerEventData.FramePressState.PressedAndReleased));
            }
        }

        protected class MouseState
        {
            private List<PointerInputModule.ButtonState> m_TrackedButtons = new List<PointerInputModule.ButtonState>();

            public bool AnyPressesThisFrame()
            {
                for (int i = 0; i < this.m_TrackedButtons.Count; i++)
                {
                    if (this.m_TrackedButtons[i].eventData.PressedThisFrame())
                    {
                        return true;
                    }
                }
                return false;
            }

            public bool AnyReleasesThisFrame()
            {
                for (int i = 0; i < this.m_TrackedButtons.Count; i++)
                {
                    if (this.m_TrackedButtons[i].eventData.ReleasedThisFrame())
                    {
                        return true;
                    }
                }
                return false;
            }

            public PointerInputModule.ButtonState GetButtonState(PointerEventData.InputButton button)
            {
                PointerInputModule.ButtonState item = null;
                for (int i = 0; i < this.m_TrackedButtons.Count; i++)
                {
                    if (this.m_TrackedButtons[i].button == button)
                    {
                        item = this.m_TrackedButtons[i];
                        break;
                    }
                }
                if (item == null)
                {
                    item = new PointerInputModule.ButtonState {
                        button = button,
                        eventData = new PointerInputModule.MouseButtonEventData()
                    };
                    this.m_TrackedButtons.Add(item);
                }
                return item;
            }

            public void SetButtonState(PointerEventData.InputButton button, PointerEventData.FramePressState stateForMouseButton, PointerEventData data)
            {
                PointerInputModule.ButtonState buttonState = this.GetButtonState(button);
                buttonState.eventData.buttonState = stateForMouseButton;
                buttonState.eventData.buttonData = data;
            }
        }
    }
}

