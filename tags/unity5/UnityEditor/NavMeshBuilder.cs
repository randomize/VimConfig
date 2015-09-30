namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class NavMeshBuilder
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void BuildNavMesh();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void BuildNavMeshAsync();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Cancel();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ClearAllNavMeshes();

        public static bool isRunning { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static UnityEngine.Object navMeshSettingsObject { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

