namespace UnityEngine.EventSystems
{
    using System;

    public interface ISelectHandler : IEventSystemHandler
    {
        void OnSelect(BaseEventData eventData);
    }
}

