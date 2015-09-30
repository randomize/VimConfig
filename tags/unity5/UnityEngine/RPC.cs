namespace UnityEngine
{
    using System;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple=true), Obsolete("NetworkView RPC functions are deprecated. Refer to the new Multiplayer Networking system.")]
    public sealed class RPC : Attribute
    {
    }
}

