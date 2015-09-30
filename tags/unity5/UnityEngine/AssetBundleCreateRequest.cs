namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class AssetBundleCreateRequest : AsyncOperation
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void DisableCompatibilityChecks();

        public AssetBundle assetBundle { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

