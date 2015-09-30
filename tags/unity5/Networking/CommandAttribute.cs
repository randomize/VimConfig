namespace UnityEngine.Networking
{
    using System;

    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        public int channel;
    }
}

