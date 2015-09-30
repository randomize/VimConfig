namespace UnityEngine.EventSystems
{
    using System;

    public interface IPointerEnterHandler : IEventSystemHandler
    {
        void OnPointerEnter(PointerEventData eventData);
    }
}

