namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public sealed class ResourceRequest : AsyncOperation
    {
        internal string m_Path;
        internal System.Type m_Type;
        public UnityEngine.Object asset
        {
            get
            {
                return Resources.Load(this.m_Path, this.m_Type);
            }
        }
    }
}

