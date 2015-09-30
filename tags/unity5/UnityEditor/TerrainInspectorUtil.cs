namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal sealed class TerrainInspectorUtil
    {
        public static bool CheckTreeDistance(TerrainData terrainData, Vector3 position, int treeIndex, float distanceBias)
        {
            return INTERNAL_CALL_CheckTreeDistance(terrainData, ref position, treeIndex, distanceBias);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetPrototypeCount(TerrainData terrainData);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Vector3 GetPrototypeExtent(TerrainData terrainData, int prototypeIndex);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern float GetTreePlacementSize(TerrainData terrainData, int prototypeIndex, float spacing, float treeCount);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_CheckTreeDistance(TerrainData terrainData, ref Vector3 position, int treeIndex, float distanceBias);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool PrototypeIsRenderable(TerrainData terrainData, int prototypeIndex);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void RefreshPhysicsInEditMode();
    }
}

