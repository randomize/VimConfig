namespace UnityEngine.EventSystems
{
    using System;

    public interface IDropHandler : IEventSystemHandler
    {
        void OnDrop(PointerEventData eventData);
    }
}

