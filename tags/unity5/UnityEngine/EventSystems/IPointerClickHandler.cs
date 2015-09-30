namespace UnityEngine.EventSystems
{
    using System;

    public interface IPointerClickHandler : IEventSystemHandler
    {
        void OnPointerClick(PointerEventData eventData);
    }
}

