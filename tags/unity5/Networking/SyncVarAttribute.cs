namespace UnityEngine.Networking
{
    using System;

    [AttributeUsage(AttributeTargets.Field)]
    public class SyncVarAttribute : Attribute
    {
        public string hook;
    }
}

