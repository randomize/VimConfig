namespace UnityEditor
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public sealed class Lightmapping
    {
        public static OnCompletedFunction completed;

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool Bake();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool BakeAllReflectionProbesSnapshots();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool BakeAsync();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool BakeLightProbesOnly();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool BakeLightProbesOnlyAsync();
        public static void BakeMultipleScenes(string[] paths)
        {
            if (paths.Length != 0)
            {
                for (int i = 0; i < paths.Length; i++)
                {
                    for (int k = i + 1; k < paths.Length; k++)
                    {
                        if (paths[i] == paths[k])
                        {
                            throw new Exception("no duplication of scenes is allowed");
                        }
                    }
                }
                SetLoadLevelForMultiLevelBake(true);
                string currentScene = EditorApplication.currentScene;
                bool isSceneDirty = false;
                if (string.IsNullOrEmpty(currentScene))
                {
                    isSceneDirty = EditorApplication.isSceneDirty;
                    EditorApplication.SaveScene("Temp/MultiLevelBakeTemp.unity", true);
                }
                else
                {
                    EditorApplication.SaveScene();
                }
                EditorApplication.OpenScene(paths[0]);
                for (int j = 1; j < paths.Length; j++)
                {
                    EditorApplication.OpenSceneAdditive(paths[j]);
                }
                giWorkflowMode = GIWorkflowMode.OnDemand;
                BakeMultipleScenes_Internal(paths);
                foreach (UnityEngine.Object obj2 in AssetDatabase.LoadAllAssetsAtPath(Path.GetDirectoryName(paths[0]) + "/" + Path.GetFileNameWithoutExtension(paths[0]) + "/LightmapSnapshot.asset"))
                {
                    LightmapSnapshot snapshot = obj2 as LightmapSnapshot;
                    if (snapshot != null)
                    {
                        EditorApplication.OpenScene(AssetDatabase.GUIDToAssetPath(snapshot.sceneGUID));
                        giWorkflowMode = GIWorkflowMode.OnDemand;
                        if (lightmapSnapshot != snapshot)
                        {
                            lightmapSnapshot = snapshot;
                            EditorApplication.SaveScene();
                        }
                    }
                }
                SetLoadLevelForMultiLevelBake(false);
                if (string.IsNullOrEmpty(currentScene))
                {
                    EditorApplication.OpenScene("Temp/MultiLevelBakeTemp.unity");
                    EditorApplication.currentScene = string.Empty;
                    if (isSceneDirty)
                    {
                        EditorApplication.MarkSceneDirty();
                    }
                }
                else
                {
                    EditorApplication.OpenScene(currentScene);
                }
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool BakeMultipleScenes_Internal(string[] paths);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool BakeReflectionProbe(ReflectionProbe probe, string path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool BakeReflectionProbeSnapshot(ReflectionProbe probe);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool BakeSelected();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool BakeSelectedAsync();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Cancel();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Clear();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ClearDiskCache();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void ClearSnapshot();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void GetTerrainGIChunks(Terrain terrain, ref int numChunksX, ref int numChunksY);
        private static void Internal_CallCompletedFunctions()
        {
            if (completed != null)
            {
                completed();
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void PrintStateToConsole();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void SetLoadLevelForMultiLevelBake(bool enable);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Tetrahedralize(Vector3[] positions, out int[] outIndices, out Vector3[] outPositions);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void UpdateCachePath();

        internal static bool bakedLightmapsEnabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal static float bounceBoost { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal static ConcurrentJobsType concurrentJobsType { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal static string diskCachePath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        internal static long diskCacheSize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        internal static bool enlightenForceUpdates { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal static bool enlightenForceWhiteAlbedo { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal static UnityEngine.FilterMode filterMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static GIWorkflowMode giWorkflowMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal static float indirectOutputScale { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool isRunning { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static LightmapSnapshot lightmapSnapshot { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal static bool openRLEnabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal static bool realtimeLightmapsEnabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal enum ConcurrentJobsType
        {
            Min,
            Low,
            High
        }

        public enum GIWorkflowMode
        {
            Iterative,
            OnDemand,
            Legacy
        }

        public delegate void OnCompletedFunction();
    }
}

