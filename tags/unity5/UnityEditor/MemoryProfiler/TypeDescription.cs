namespace UnityEditor.MemoryProfiler
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    internal struct TypeDescription
    {
        public string name;
        public string assembly;
        public FieldDescription[] fields;
        public byte[] staticFieldBytes;
        public int baseOrElementTypeIndex;
        public int size;
        public ulong typeInfoAddress;
        public int typeIndex;
        internal TypeFlags flags;
        public bool IsValueType
        {
            get
            {
                return ((this.flags & TypeFlags.kValueType) != TypeFlags.kNone);
            }
        }
        public bool IsArray
        {
            get
            {
                return ((this.flags & TypeFlags.kArray) != TypeFlags.kNone);
            }
        }
        public int ArrayRank
        {
            get
            {
                return (((int) (this.flags & -65536)) >> 0x10);
            }
        }
        internal enum TypeFlags
        {
            kArray = 2,
            kArrayRankMask = -65536,
            kNone = 0,
            kValueType = 1
        }
    }
}

