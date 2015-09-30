namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct SkeletonBone
    {
        public string name;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public int transformModified;
    }
}

