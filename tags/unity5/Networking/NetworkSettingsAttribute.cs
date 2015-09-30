namespace UnityEngine.Networking
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class NetworkSettingsAttribute : Attribute
    {
        public int channel;
        public float sendInterval = 0.1f;
    }
}

