namespace UnityEngine.EventSystems
{
    using System;

    public interface IPointerUpHandler : IEventSystemHandler
    {
        void OnPointerUp(PointerEventData eventData);
    }
}

