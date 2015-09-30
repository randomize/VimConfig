namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), Obsolete("this API is not for public use.")]
    public struct CacheIndex
    {
        public string name;
        public int bytesUsed;
        public int expires;
    }
}

