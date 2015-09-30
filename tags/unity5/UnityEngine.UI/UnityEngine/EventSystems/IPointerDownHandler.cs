namespace UnityEngine.EventSystems
{
    using System;

    public interface IPointerDownHandler : IEventSystemHandler
    {
        void OnPointerDown(PointerEventData eventData);
    }
}

