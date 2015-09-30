namespace UnityEngine
{
    using System;

    public interface ISerializationCallbackReceiver
    {
        void OnAfterDeserialize();
        void OnBeforeSerialize();
    }
}

