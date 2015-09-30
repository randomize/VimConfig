namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct MonoReloadableIntPtr
    {
        internal IntPtr m_IntPtr;
    }
}

