namespace UnityEditor.MemoryProfiler
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    internal struct FieldDescription
    {
        public string name;
        public int offset;
        public int typeIndex;
        public bool isStatic;
    }
}

