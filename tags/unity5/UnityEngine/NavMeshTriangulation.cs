namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct NavMeshTriangulation
    {
        public Vector3[] vertices;
        public int[] indices;
        public int[] areas;
        [Obsolete("Use areas instead.")]
        public int[] layers
        {
            get
            {
                return this.areas;
            }
        }
    }
}

