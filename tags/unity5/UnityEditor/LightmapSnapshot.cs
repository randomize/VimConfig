namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class LightmapSnapshot : UnityEngine.Object
    {
        internal string sceneGUID { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

