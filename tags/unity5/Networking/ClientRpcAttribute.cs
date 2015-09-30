namespace UnityEngine.Networking
{
    using System;

    [AttributeUsage(AttributeTargets.Method)]
    public class ClientRpcAttribute : Attribute
    {
        public int channel;
    }
}

