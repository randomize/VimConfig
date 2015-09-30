namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngineInternal;

    public sealed class AssetBundle : UnityEngine.Object
    {
        [Obsolete("This method is deprecated. Use GetAllAssetNames() instead.")]
        public string[] AllAssetNames()
        {
            return this.GetAllAssetNames();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool Contains(string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern AssetBundle CreateFromFile(string path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern AssetBundleCreateRequest CreateFromMemory(byte[] binary);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern AssetBundle CreateFromMemoryImmediate(byte[] binary);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string[] GetAllAssetNames();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string[] GetAllScenePaths();
        [Obsolete("Method Load has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAsset instead and check the documentation for details.", true)]
        public UnityEngine.Object Load(string name)
        {
            return null;
        }

        [Obsolete("Method Load has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAsset instead and check the documentation for details.", true)]
        public T Load<T>(string name) where T: UnityEngine.Object
        {
            return null;
        }

        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("Method Load has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAsset instead and check the documentation for details.", true), WrapperlessIcall, TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument)]
        public extern UnityEngine.Object Load(string name, System.Type type);
        [Obsolete("Method LoadAll has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAllAssets instead and check the documentation for details.", true)]
        public UnityEngine.Object[] LoadAll()
        {
            return null;
        }

        [Obsolete("Method LoadAll has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAllAssets instead and check the documentation for details.", true)]
        public T[] LoadAll<T>() where T: UnityEngine.Object
        {
            return null;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, Obsolete("Method LoadAll has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAllAssets instead and check the documentation for details.", true)]
        public extern UnityEngine.Object[] LoadAll(System.Type type);
        public UnityEngine.Object[] LoadAllAssets()
        {
            return this.LoadAllAssets(typeof(UnityEngine.Object));
        }

        public T[] LoadAllAssets<T>() where T: UnityEngine.Object
        {
            return Resources.ConvertObjects<T>(this.LoadAllAssets(typeof(T)));
        }

        public UnityEngine.Object[] LoadAllAssets(System.Type type)
        {
            if (type == null)
            {
                throw new NullReferenceException("The input type cannot be null.");
            }
            return this.LoadAssetWithSubAssets_Internal(string.Empty, type);
        }

        public AssetBundleRequest LoadAllAssetsAsync()
        {
            return this.LoadAllAssetsAsync(typeof(UnityEngine.Object));
        }

        public AssetBundleRequest LoadAllAssetsAsync<T>()
        {
            return this.LoadAllAssetsAsync(typeof(T));
        }

        public AssetBundleRequest LoadAllAssetsAsync(System.Type type)
        {
            if (type == null)
            {
                throw new NullReferenceException("The input type cannot be null.");
            }
            return this.LoadAssetWithSubAssetsAsync_Internal(string.Empty, type);
        }

        public UnityEngine.Object LoadAsset(string name)
        {
            return this.LoadAsset(name, typeof(UnityEngine.Object));
        }

        public T LoadAsset<T>(string name) where T: UnityEngine.Object
        {
            return (T) this.LoadAsset(name, typeof(T));
        }

        [TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument)]
        public UnityEngine.Object LoadAsset(string name, System.Type type)
        {
            if (name == null)
            {
                throw new NullReferenceException("The input asset name cannot be null.");
            }
            if (name.Length == 0)
            {
                throw new ArgumentException("The input asset name cannot be empty.");
            }
            if (type == null)
            {
                throw new NullReferenceException("The input type cannot be null.");
            }
            return this.LoadAsset_Internal(name, type);
        }

        [MethodImpl(MethodImplOptions.InternalCall), TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument), WrapperlessIcall]
        private extern UnityEngine.Object LoadAsset_Internal(string name, System.Type type);
        public AssetBundleRequest LoadAssetAsync(string name)
        {
            return this.LoadAssetAsync(name, typeof(UnityEngine.Object));
        }

        public AssetBundleRequest LoadAssetAsync<T>(string name)
        {
            return this.LoadAssetAsync(name, typeof(T));
        }

        public AssetBundleRequest LoadAssetAsync(string name, System.Type type)
        {
            if (name == null)
            {
                throw new NullReferenceException("The input asset name cannot be null.");
            }
            if (name.Length == 0)
            {
                throw new ArgumentException("The input asset name cannot be empty.");
            }
            if (type == null)
            {
                throw new NullReferenceException("The input type cannot be null.");
            }
            return this.LoadAssetAsync_Internal(name, type);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern AssetBundleRequest LoadAssetAsync_Internal(string name, System.Type type);
        public UnityEngine.Object[] LoadAssetWithSubAssets(string name)
        {
            return this.LoadAssetWithSubAssets(name, typeof(UnityEngine.Object));
        }

        public T[] LoadAssetWithSubAssets<T>(string name) where T: UnityEngine.Object
        {
            return Resources.ConvertObjects<T>(this.LoadAssetWithSubAssets(name, typeof(T)));
        }

        public UnityEngine.Object[] LoadAssetWithSubAssets(string name, System.Type type)
        {
            if (name == null)
            {
                throw new NullReferenceException("The input asset name cannot be null.");
            }
            if (name.Length == 0)
            {
                throw new ArgumentException("The input asset name cannot be empty.");
            }
            if (type == null)
            {
                throw new NullReferenceException("The input type cannot be null.");
            }
            return this.LoadAssetWithSubAssets_Internal(name, type);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern UnityEngine.Object[] LoadAssetWithSubAssets_Internal(string name, System.Type type);
        public AssetBundleRequest LoadAssetWithSubAssetsAsync(string name)
        {
            return this.LoadAssetWithSubAssetsAsync(name, typeof(UnityEngine.Object));
        }

        public AssetBundleRequest LoadAssetWithSubAssetsAsync<T>(string name)
        {
            return this.LoadAssetWithSubAssetsAsync(name, typeof(T));
        }

        public AssetBundleRequest LoadAssetWithSubAssetsAsync(string name, System.Type type)
        {
            if (name == null)
            {
                throw new NullReferenceException("The input asset name cannot be null.");
            }
            if (name.Length == 0)
            {
                throw new ArgumentException("The input asset name cannot be empty.");
            }
            if (type == null)
            {
                throw new NullReferenceException("The input type cannot be null.");
            }
            return this.LoadAssetWithSubAssetsAsync_Internal(name, type);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern AssetBundleRequest LoadAssetWithSubAssetsAsync_Internal(string name, System.Type type);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, Obsolete("Method LoadAsync has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAssetAsync instead and check the documentation for details.", true)]
        public extern AssetBundleRequest LoadAsync(string name, System.Type type);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Unload(bool unloadAllLoadedObjects);

        public UnityEngine.Object mainAsset { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

