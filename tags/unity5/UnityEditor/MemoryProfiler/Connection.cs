namespace UnityEditor.MemoryProfiler
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    internal struct Connection
    {
        public int from;
        public int to;
    }
}

