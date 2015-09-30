namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class StaticBatchingUtility
    {
        public static void Combine(GameObject staticBatchRoot)
        {
            InternalStaticBatchingUtility.Combine(staticBatchRoot);
        }

        public static void Combine(GameObject[] gos, GameObject staticBatchRoot)
        {
            InternalStaticBatchingUtility.Combine(gos, staticBatchRoot);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void InternalCombineIndices(MeshSubsetCombineUtility.SubMeshInstance[] submeshes, [Writable] Mesh combinedMesh);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern Mesh InternalCombineVertices(MeshSubsetCombineUtility.MeshInstance[] meshes, string meshName);
    }
}

