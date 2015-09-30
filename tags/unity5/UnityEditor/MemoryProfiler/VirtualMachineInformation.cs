namespace UnityEditor.MemoryProfiler
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    internal struct VirtualMachineInformation
    {
        public int pointerSize;
        public int objectHeaderSize;
        public int arrayHeaderSize;
        public int arrayBoundsOffsetInHeader;
        public int arraySizeOffsetInHeader;
        public int allocationGranularity;
    }
}

