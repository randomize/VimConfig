namespace UnityEngine.VR
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class InputTracking
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Vector3 GetLocalPosition(VRNode node);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Quaternion GetLocalRotation(VRNode node);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Recenter();
    }
}

