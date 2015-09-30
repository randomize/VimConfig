namespace UnityEngine.EventSystems
{
    using System;

    public interface IPointerExitHandler : IEventSystemHandler
    {
        void OnPointerExit(PointerEventData eventData);
    }
}

