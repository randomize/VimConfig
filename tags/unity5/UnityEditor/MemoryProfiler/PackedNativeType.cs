namespace UnityEditor.MemoryProfiler
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    internal struct PackedNativeType
    {
        public string name;
        public int baseClassId;
    }
}

