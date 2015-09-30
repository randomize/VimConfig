namespace UnityEngine.EventSystems
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class AxisEventData : BaseEventData
    {
        public AxisEventData(EventSystem eventSystem) : base(eventSystem)
        {
            this.moveVector = Vector2.zero;
            this.moveDir = MoveDirection.None;
        }

        public MoveDirection moveDir { get; set; }

        public Vector2 moveVector { get; set; }
    }
}

