namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Internal;

    public sealed class PrefabUtility
    {
        public static PrefabInstanceUpdated prefabInstanceUpdated;

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern UnityEngine.Object CreateEmptyPrefab(string path);
        [ExcludeFromDocs]
        public static GameObject CreatePrefab(string path, GameObject go)
        {
            ReplacePrefabOptions options = ReplacePrefabOptions.Default;
            return CreatePrefab(path, go, options);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern GameObject CreatePrefab(string path, GameObject go, [DefaultValue("ReplacePrefabOptions.Default")] ReplacePrefabOptions options);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void DisconnectPrefabInstance(UnityEngine.Object targetObject);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern GameObject FindPrefabRoot(GameObject source);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern GameObject FindRootGameObjectWithSameParentPrefab(GameObject target);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern GameObject FindValidUploadPrefabInstanceRoot(GameObject target);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern UnityEngine.Object GetPrefabObject(UnityEngine.Object targetObject);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern UnityEngine.Object GetPrefabParent(UnityEngine.Object source);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern PrefabType GetPrefabType(UnityEngine.Object target);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern PropertyModification[] GetPropertyModifications(UnityEngine.Object targetPrefab);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern UnityEngine.Object InstantiateAttachedAsset(UnityEngine.Object targetObject);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern UnityEngine.Object InstantiatePrefab(UnityEngine.Object target);
        private static void Internal_CallPrefabInstanceUpdated(GameObject instance)
        {
            if (prefabInstanceUpdated != null)
            {
                prefabInstanceUpdated(instance);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool IsComponentAddedToPrefabInstance(UnityEngine.Object source);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void MergeAllPrefabInstances(UnityEngine.Object targetObject);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool ReconnectToLastPrefab(GameObject go);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void RecordPrefabInstancePropertyModifications(UnityEngine.Object targetObject);
        [ExcludeFromDocs]
        public static GameObject ReplacePrefab(GameObject go, UnityEngine.Object targetPrefab)
        {
            ReplacePrefabOptions options = ReplacePrefabOptions.Default;
            return ReplacePrefab(go, targetPrefab, options);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern GameObject ReplacePrefab(GameObject go, UnityEngine.Object targetPrefab, [DefaultValue("ReplacePrefabOptions.Default")] ReplacePrefabOptions options);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool ResetToPrefabState(UnityEngine.Object obj);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool RevertPrefabInstance(GameObject go);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetPropertyModifications(UnityEngine.Object targetPrefab, PropertyModification[] modifications);

        public delegate void PrefabInstanceUpdated(GameObject instance);
    }
}

