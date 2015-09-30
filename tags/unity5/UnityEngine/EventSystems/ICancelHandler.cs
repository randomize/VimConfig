namespace UnityEngine.EventSystems
{
    using System;

    public interface ICancelHandler : IEventSystemHandler
    {
        void OnCancel(BaseEventData eventData);
    }
}

