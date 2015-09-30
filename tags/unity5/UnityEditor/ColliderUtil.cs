namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal sealed class ColliderUtil
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Matrix4x4 CalculateCapsuleTransform(CapsuleCollider cc);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Vector3 GetCapsuleExtents(CapsuleCollider cc);
    }
}

