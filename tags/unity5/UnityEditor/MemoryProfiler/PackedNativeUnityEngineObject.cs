namespace UnityEditor.MemoryProfiler
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    internal struct PackedNativeUnityEngineObject
    {
        public string name;
        public int instanceId;
        public int size;
        public int classId;
        internal ObjectFlags flags;
        public bool IsPersistent
        {
            get
            {
                return ((this.flags & ObjectFlags.IsPersistent) != ((ObjectFlags) 0));
            }
        }
        public bool IsDontDestroyOnLoad
        {
            get
            {
                return ((this.flags & ObjectFlags.IsDontDestroyOnLoad) != ((ObjectFlags) 0));
            }
        }
        internal enum ObjectFlags
        {
            IsDontDestroyOnLoad = 1,
            IsPersistent = 2
        }
    }
}

