namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public sealed class AssetBundleRequest : AsyncOperation
    {
        internal AssetBundle m_AssetBundle;
        internal string m_Path;
        internal System.Type m_Type;
        public UnityEngine.Object asset
        {
            get
            {
                return this.m_AssetBundle.LoadAsset(this.m_Path, this.m_Type);
            }
        }
        public UnityEngine.Object[] allAssets
        {
            get
            {
                return this.m_AssetBundle.LoadAssetWithSubAssets_Internal(this.m_Path, this.m_Type);
            }
        }
    }
}

