namespace UnityEngine.EventSystems
{
    using System;

    public interface IScrollHandler : IEventSystemHandler
    {
        void OnScroll(PointerEventData eventData);
    }
}

