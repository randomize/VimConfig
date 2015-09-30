namespace UnityEngine.EventSystems
{
    using System;

    public interface IBeginDragHandler : IEventSystemHandler
    {
        void OnBeginDrag(PointerEventData eventData);
    }
}

