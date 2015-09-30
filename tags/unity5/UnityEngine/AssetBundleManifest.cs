namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class AssetBundleManifest : UnityEngine.Object
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string[] GetAllAssetBundles();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string[] GetAllAssetBundlesWithVariant();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string[] GetAllDependencies(string assetBundleName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern Hash128 GetAssetBundleHash(string assetBundleName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string[] GetDirectDependencies(string assetBundleName);
    }
}

