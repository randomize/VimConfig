namespace UnityEditor.MemoryProfiler
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    internal struct ManagedMemorySection
    {
        public byte[] bytes;
        public ulong startAddress;
    }
}

