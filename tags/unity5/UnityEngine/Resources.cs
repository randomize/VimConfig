namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngineInternal;

    public sealed class Resources
    {
        internal static T[] ConvertObjects<T>(UnityEngine.Object[] rawObjects) where T: UnityEngine.Object
        {
            if (rawObjects == null)
            {
                return null;
            }
            T[] localArray = new T[rawObjects.Length];
            for (int i = 0; i < localArray.Length; i++)
            {
                localArray[i] = (T) rawObjects[i];
            }
            return localArray;
        }

        public static T[] FindObjectsOfTypeAll<T>() where T: UnityEngine.Object
        {
            return ConvertObjects<T>(FindObjectsOfTypeAll(typeof(T)));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, TypeInferenceRule(TypeInferenceRules.ArrayOfTypeReferencedByFirstArgument)]
        public static extern UnityEngine.Object[] FindObjectsOfTypeAll(System.Type type);
        public static T GetBuiltinResource<T>(string path) where T: UnityEngine.Object
        {
            return (T) GetBuiltinResource(typeof(T), path);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
        public static extern UnityEngine.Object GetBuiltinResource(System.Type type, string path);
        public static UnityEngine.Object Load(string path)
        {
            return Load(path, typeof(UnityEngine.Object));
        }

        public static T Load<T>(string path) where T: UnityEngine.Object
        {
            return (T) Load(path, typeof(T));
        }

        [MethodImpl(MethodImplOptions.InternalCall), TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument), WrapperlessIcall]
        public static extern UnityEngine.Object Load(string path, System.Type systemTypeInstance);
        public static UnityEngine.Object[] LoadAll(string path)
        {
            return LoadAll(path, typeof(UnityEngine.Object));
        }

        public static T[] LoadAll<T>(string path) where T: UnityEngine.Object
        {
            return ConvertObjects<T>(LoadAll(path, typeof(T)));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern UnityEngine.Object[] LoadAll(string path, System.Type systemTypeInstance);
        [Obsolete("Use AssetDatabase.LoadAssetAtPath<T>() instead (UnityUpgradable) -> * [UnityEditor] UnityEditor.AssetDatabase.LoadAssetAtPath<T>(*)", true)]
        public static T LoadAssetAtPath<T>(string assetPath) where T: UnityEngine.Object
        {
            return null;
        }

        [Obsolete("Use AssetDatabase.LoadAssetAtPath instead (UnityUpgradable) -> * [UnityEditor] UnityEditor.AssetDatabase.LoadAssetAtPath(*)", true), TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument)]
        public static UnityEngine.Object LoadAssetAtPath(string assetPath, System.Type type)
        {
            return null;
        }

        public static ResourceRequest LoadAsync(string path)
        {
            return LoadAsync(path, typeof(UnityEngine.Object));
        }

        public static ResourceRequest LoadAsync<T>(string path) where T: UnityEngine.Object
        {
            return LoadAsync(path, typeof(T));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern ResourceRequest LoadAsync(string path, System.Type type);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void UnloadAsset(UnityEngine.Object assetToUnload);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern AsyncOperation UnloadUnusedAssets();
    }
}

