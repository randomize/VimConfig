namespace UnityEditor.MemoryProfiler
{
    using System;

    [Serializable]
    internal class PackedMemorySnapshot
    {
        public Connection[] connections;
        public PackedGCHandle[] gcHandles;
        public ManagedMemorySection[] managedHeapSections;
        public PackedNativeUnityEngineObject[] nativeObjects;
        public PackedNativeType[] nativeTypes;
        public ManagedMemorySection[] stacks;
        public TypeDescription[] typeDescriptions;
        public VirtualMachineInformation virtualMachineInformation = new VirtualMachineInformation();
    }
}

