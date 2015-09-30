namespace UnityEngine.Networking
{
    using System;

    [AttributeUsage(AttributeTargets.Event)]
    public class SyncEventAttribute : Attribute
    {
        public int channel;
    }
}

