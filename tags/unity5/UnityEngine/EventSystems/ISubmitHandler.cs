namespace UnityEngine.EventSystems
{
    using System;

    public interface ISubmitHandler : IEventSystemHandler
    {
        void OnSubmit(BaseEventData eventData);
    }
}

