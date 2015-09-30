namespace UnityEngine.EventSystems
{
    using System;

    public interface IEndDragHandler : IEventSystemHandler
    {
        void OnEndDrag(PointerEventData eventData);
    }
}

