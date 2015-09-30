namespace UnityEngine.EventSystems
{
    using System;
    using UnityEngine;
    using UnityEngine.Serialization;

    [AddComponentMenu("Event/Standalone Input Module")]
    public class StandaloneInputModule : PointerInputModule
    {
        [SerializeField]
        private string m_CancelButton = "Cancel";
        private int m_ConsecutiveMoveCount;
        [SerializeField, FormerlySerializedAs("m_AllowActivationOnMobileDevice")]
        private bool m_ForceModuleActive;
        [SerializeField]
        private string m_HorizontalAxis = "Horizontal";
        [SerializeField]
        private float m_InputActionsPerSecond = 10f;
        private Vector2 m_LastMousePosition;
        private Vector2 m_LastMoveVector;
        private Vector2 m_MousePosition;
        private float m_PrevActionTime;
        [SerializeField]
        private float m_RepeatDelay = 0.5f;
        [SerializeField]
        private string m_SubmitButton = "Submit";
        [SerializeField]
        private string m_VerticalAxis = "Vertical";

        protected StandaloneInputModule()
        {
        }

        public override void ActivateModule()
        {
            base.ActivateModule();
            this.m_MousePosition = Input.mousePosition;
            this.m_LastMousePosition = Input.mousePosition;
            GameObject currentSelectedGameObject = base.eventSystem.currentSelectedGameObject;
            if (currentSelectedGameObject == null)
            {
                currentSelectedGameObject = base.eventSystem.firstSelectedGameObject;
            }
            base.eventSystem.SetSelectedGameObject(currentSelectedGameObject, this.GetBaseEventData());
        }

        public override void DeactivateModule()
        {
            base.DeactivateModule();
            base.ClearSelection();
        }

        private Vector2 GetRawMoveVector()
        {
            Vector2 zero = Vector2.zero;
            zero.x = Input.GetAxisRaw(this.m_HorizontalAxis);
            zero.y = Input.GetAxisRaw(this.m_VerticalAxis);
            if (Input.GetButtonDown(this.m_HorizontalAxis))
            {
                if (zero.x < 0f)
                {
                    zero.x = -1f;
                }
                if (zero.x > 0f)
                {
                    zero.x = 1f;
                }
            }
            if (Input.GetButtonDown(this.m_VerticalAxis))
            {
                if (zero.y < 0f)
                {
                    zero.y = -1f;
                }
                if (zero.y > 0f)
                {
                    zero.y = 1f;
                }
            }
            return zero;
        }

        public override bool IsModuleSupported()
        {
            return (this.m_ForceModuleActive || Input.mousePresent);
        }

        public override void Process()
        {
            bool flag = this.SendUpdateEventToSelectedObject();
            if (base.eventSystem.sendNavigationEvents)
            {
                if (!flag)
                {
                    flag |= this.SendMoveEventToSelectedObject();
                }
                if (!flag)
                {
                    this.SendSubmitEventToSelectedObject();
                }
            }
            this.ProcessMouseEvent();
        }

        protected void ProcessMouseEvent()
        {
            this.ProcessMouseEvent(0);
        }

        protected void ProcessMouseEvent(int id)
        {
            PointerInputModule.MouseState mousePointerEventData = this.GetMousePointerEventData(id);
            PointerInputModule.MouseButtonEventData eventData = mousePointerEventData.GetButtonState(PointerEventData.InputButton.Left).eventData;
            this.ProcessMousePress(eventData);
            this.ProcessMove(eventData.buttonData);
            this.ProcessDrag(eventData.buttonData);
            this.ProcessMousePress(mousePointerEventData.GetButtonState(PointerEventData.InputButton.Right).eventData);
            this.ProcessDrag(mousePointerEventData.GetButtonState(PointerEventData.InputButton.Right).eventData.buttonData);
            this.ProcessMousePress(mousePointerEventData.GetButtonState(PointerEventData.InputButton.Middle).eventData);
            this.ProcessDrag(mousePointerEventData.GetButtonState(PointerEventData.InputButton.Middle).eventData.buttonData);
            if (!Mathf.Approximately(eventData.buttonData.scrollDelta.sqrMagnitude, 0f))
            {
                ExecuteEvents.ExecuteHierarchy<IScrollHandler>(ExecuteEvents.GetEventHandler<IScrollHandler>(eventData.buttonData.pointerCurrentRaycast.gameObject), eventData.buttonData, ExecuteEvents.scrollHandler);
            }
        }

        protected void ProcessMousePress(PointerInputModule.MouseButtonEventData data)
        {
            PointerEventData buttonData = data.buttonData;
            GameObject gameObject = buttonData.pointerCurrentRaycast.gameObject;
            if (data.PressedThisFrame())
            {
                buttonData.eligibleForClick = true;
                buttonData.delta = Vector2.zero;
                buttonData.dragging = false;
                buttonData.useDragThreshold = true;
                buttonData.pressPosition = buttonData.position;
                buttonData.pointerPressRaycast = buttonData.pointerCurrentRaycast;
                base.DeselectIfSelectionChanged(gameObject, buttonData);
                GameObject eventHandler = ExecuteEvents.ExecuteHierarchy<IPointerDownHandler>(gameObject, buttonData, ExecuteEvents.pointerDownHandler);
                if (eventHandler == null)
                {
                    eventHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
                }
                float unscaledTime = Time.unscaledTime;
                if (eventHandler == buttonData.lastPress)
                {
                    float num2 = unscaledTime - buttonData.clickTime;
                    if (num2 < 0.3f)
                    {
                        buttonData.clickCount++;
                    }
                    else
                    {
                        buttonData.clickCount = 1;
                    }
                    buttonData.clickTime = unscaledTime;
                }
                else
                {
                    buttonData.clickCount = 1;
                }
                buttonData.pointerPress = eventHandler;
                buttonData.rawPointerPress = gameObject;
                buttonData.clickTime = unscaledTime;
                buttonData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(gameObject);
                if (buttonData.pointerDrag != null)
                {
                    ExecuteEvents.Execute<IInitializePotentialDragHandler>(buttonData.pointerDrag, buttonData, ExecuteEvents.initializePotentialDrag);
                }
            }
            if (data.ReleasedThisFrame())
            {
                ExecuteEvents.Execute<IPointerUpHandler>(buttonData.pointerPress, buttonData, ExecuteEvents.pointerUpHandler);
                GameObject obj4 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
                if ((buttonData.pointerPress == obj4) && buttonData.eligibleForClick)
                {
                    ExecuteEvents.Execute<IPointerClickHandler>(buttonData.pointerPress, buttonData, ExecuteEvents.pointerClickHandler);
                }
                else if ((buttonData.pointerDrag != null) && buttonData.dragging)
                {
                    ExecuteEvents.ExecuteHierarchy<IDropHandler>(gameObject, buttonData, ExecuteEvents.dropHandler);
                }
                buttonData.eligibleForClick = false;
                buttonData.pointerPress = null;
                buttonData.rawPointerPress = null;
                if ((buttonData.pointerDrag != null) && buttonData.dragging)
                {
                    ExecuteEvents.Execute<IEndDragHandler>(buttonData.pointerDrag, buttonData, ExecuteEvents.endDragHandler);
                }
                buttonData.dragging = false;
                buttonData.pointerDrag = null;
                if (gameObject != buttonData.pointerEnter)
                {
                    base.HandlePointerExitAndEnter(buttonData, null);
                    base.HandlePointerExitAndEnter(buttonData, gameObject);
                }
            }
        }

        protected bool SendMoveEventToSelectedObject()
        {
            float unscaledTime = Time.unscaledTime;
            Vector2 rawMoveVector = this.GetRawMoveVector();
            if (Mathf.Approximately(rawMoveVector.x, 0f) && Mathf.Approximately(rawMoveVector.y, 0f))
            {
                this.m_ConsecutiveMoveCount = 0;
                return false;
            }
            bool flag = Input.GetButtonDown(this.m_HorizontalAxis) || Input.GetButtonDown(this.m_VerticalAxis);
            bool flag2 = Vector2.Dot(rawMoveVector, this.m_LastMoveVector) > 0f;
            if (!flag)
            {
                if (flag2 && (this.m_ConsecutiveMoveCount == 1))
                {
                    flag = unscaledTime > (this.m_PrevActionTime + this.m_RepeatDelay);
                }
                else
                {
                    flag = unscaledTime > (this.m_PrevActionTime + (1f / this.m_InputActionsPerSecond));
                }
            }
            if (!flag)
            {
                return false;
            }
            AxisEventData eventData = this.GetAxisEventData(rawMoveVector.x, rawMoveVector.y, 0.6f);
            ExecuteEvents.Execute<IMoveHandler>(base.eventSystem.currentSelectedGameObject, eventData, ExecuteEvents.moveHandler);
            if (!flag2)
            {
                this.m_ConsecutiveMoveCount = 0;
            }
            this.m_ConsecutiveMoveCount++;
            this.m_PrevActionTime = unscaledTime;
            this.m_LastMoveVector = rawMoveVector;
            return eventData.used;
        }

        protected bool SendSubmitEventToSelectedObject()
        {
            if (base.eventSystem.currentSelectedGameObject == null)
            {
                return false;
            }
            BaseEventData baseEventData = this.GetBaseEventData();
            if (Input.GetButtonDown(this.m_SubmitButton))
            {
                ExecuteEvents.Execute<ISubmitHandler>(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.submitHandler);
            }
            if (Input.GetButtonDown(this.m_CancelButton))
            {
                ExecuteEvents.Execute<ICancelHandler>(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.cancelHandler);
            }
            return baseEventData.used;
        }

        protected bool SendUpdateEventToSelectedObject()
        {
            if (base.eventSystem.currentSelectedGameObject == null)
            {
                return false;
            }
            BaseEventData baseEventData = this.GetBaseEventData();
            ExecuteEvents.Execute<IUpdateSelectedHandler>(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.updateSelectedHandler);
            return baseEventData.used;
        }

        public override bool ShouldActivateModule()
        {
            if (!base.ShouldActivateModule())
            {
                return false;
            }
            bool forceModuleActive = this.m_ForceModuleActive;
            Input.GetButtonDown(this.m_SubmitButton);
            forceModuleActive |= Input.GetButtonDown(this.m_CancelButton);
            forceModuleActive |= !Mathf.Approximately(Input.GetAxisRaw(this.m_HorizontalAxis), 0f);
            forceModuleActive |= !Mathf.Approximately(Input.GetAxisRaw(this.m_VerticalAxis), 0f);
            Vector2 vector = this.m_MousePosition - this.m_LastMousePosition;
            forceModuleActive |= vector.sqrMagnitude > 0f;
            return (forceModuleActive | Input.GetMouseButtonDown(0));
        }

        public override void UpdateModule()
        {
            this.m_LastMousePosition = this.m_MousePosition;
            this.m_MousePosition = Input.mousePosition;
        }

        [Obsolete("allowActivationOnMobileDevice has been deprecated. Use forceModuleActive instead (UnityUpgradable) -> forceModuleActive")]
        public bool allowActivationOnMobileDevice
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

        public string cancelButton
        {
            get
            {
                return this.m_CancelButton;
            }
            set
            {
                this.m_CancelButton = value;
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

        public string horizontalAxis
        {
            get
            {
                return this.m_HorizontalAxis;
            }
            set
            {
                this.m_HorizontalAxis = value;
            }
        }

        public float inputActionsPerSecond
        {
            get
            {
                return this.m_InputActionsPerSecond;
            }
            set
            {
                this.m_InputActionsPerSecond = value;
            }
        }

        [Obsolete("Mode is no longer needed on input module as it handles both mouse and keyboard simultaneously.", false)]
        public InputMode inputMode
        {
            get
            {
                return InputMode.Mouse;
            }
        }

        public float repeatDelay
        {
            get
            {
                return this.m_RepeatDelay;
            }
            set
            {
                this.m_RepeatDelay = value;
            }
        }

        public string submitButton
        {
            get
            {
                return this.m_SubmitButton;
            }
            set
            {
                this.m_SubmitButton = value;
            }
        }

        public string verticalAxis
        {
            get
            {
                return this.m_VerticalAxis;
            }
            set
            {
                this.m_VerticalAxis = value;
            }
        }

        [Obsolete("Mode is no longer needed on input module as it handles both mouse and keyboard simultaneously.", false)]
        public enum InputMode
        {
            Mouse,
            Buttons
        }
    }
}

