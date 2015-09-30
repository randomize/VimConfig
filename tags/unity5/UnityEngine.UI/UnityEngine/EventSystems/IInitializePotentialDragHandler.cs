namespace UnityEngine.EventSystems
{
    using System;

    public interface IInitializePotentialDragHandler : IEventSystemHandler
    {
        void OnInitializePotentialDrag(PointerEventData eventData);
    }
}

