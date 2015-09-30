namespace UnityEngine.EventSystems
{
    using System;

    public interface IDragHandler : IEventSystemHandler
    {
        void OnDrag(PointerEventData eventData);
    }
}

