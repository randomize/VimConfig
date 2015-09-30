namespace UnityEngine.EventSystems
{
    using System;

    public interface IUpdateSelectedHandler : IEventSystemHandler
    {
        void OnUpdateSelected(BaseEventData eventData);
    }
}

