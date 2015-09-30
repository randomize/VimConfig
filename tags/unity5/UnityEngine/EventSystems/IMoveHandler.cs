namespace UnityEngine.EventSystems
{
    using System;

    public interface IMoveHandler : IEventSystemHandler
    {
        void OnMove(AxisEventData eventData);
    }
}

