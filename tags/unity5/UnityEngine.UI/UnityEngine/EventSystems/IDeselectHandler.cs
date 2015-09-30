namespace UnityEngine.EventSystems
{
    using System;

    public interface IDeselectHandler : IEventSystemHandler
    {
        void OnDeselect(BaseEventData eventData);
    }
}

