namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Internal;
    using UnityEngineInternal;

    public sealed class AssetDatabase
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void AddInstanceIDToAssetWithRandomFileId(int instanceIDToAdd, UnityEngine.Object assetObject, bool hide);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void AddObjectToAsset(UnityEngine.Object objectToAdd, string path);
        public static void AddObjectToAsset(UnityEngine.Object objectToAdd, UnityEngine.Object assetObject)
        {
            AddObjectToAsset_OBJ_Internal(objectToAdd, assetObject);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void AddObjectToAsset_OBJ_Internal(UnityEngine.Object newAsset, UnityEngine.Object sameAssetFile);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string AssetPathToGUID(string path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ClearLabels(UnityEngine.Object obj);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool Contains(int instanceID);
        public static bool Contains(UnityEngine.Object obj)
        {
            return Contains(obj.GetInstanceID());
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool CopyAsset(string path, string newPath);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void CreateAsset(UnityEngine.Object asset, string path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void CreateAssetFromObjects(UnityEngine.Object[] assets, string path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string CreateFolder(string parentFolder, string newFolderName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool DeleteAsset(string path);
        public static void ExportPackage(string assetPathName, string fileName)
        {
            ExportPackage(new string[] { assetPathName }, fileName, ExportPackageOptions.Default);
        }

        [ExcludeFromDocs]
        public static void ExportPackage(string[] assetPathNames, string fileName)
        {
            ExportPackageOptions flags = ExportPackageOptions.Default;
            ExportPackage(assetPathNames, fileName, flags);
        }

        public static void ExportPackage(string assetPathName, string fileName, ExportPackageOptions flags)
        {
            ExportPackage(new string[] { assetPathName }, fileName, flags);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ExportPackage(string[] assetPathNames, string fileName, [DefaultValue("ExportPackageOptions.Default")] ExportPackageOptions flags);
        public static string[] FindAssets(string filter)
        {
            return FindAssets(filter, null);
        }

        private static string[] FindAssets(SearchFilter searchFilter)
        {
            if ((searchFilter.folders != null) && (searchFilter.folders.Length > 0))
            {
                return SearchInFolders(searchFilter);
            }
            return SearchAllAssets(searchFilter);
        }

        public static string[] FindAssets(string filter, string[] searchInFolders)
        {
            SearchFilter filter2 = new SearchFilter();
            SearchUtility.ParseSearchString(filter, filter2);
            if (searchInFolders != null)
            {
                filter2.folders = searchInFolders;
            }
            return FindAssets(filter2);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GenerateUniqueAssetPath(string path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string[] GetAllAssetBundleNames();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string[] GetAllAssetBundleNamesWithoutVariant();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string[] GetAllAssetBundleVariants();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string[] GetAllAssetPaths();
        internal static Dictionary<string, float> GetAllLabels()
        {
            string[] strArray;
            float[] numArray;
            INTERNAL_GetAllLabels(out strArray, out numArray);
            Dictionary<string, float> dictionary = new Dictionary<string, float>(strArray.Length);
            for (int i = 0; i < strArray.Length; i++)
            {
                dictionary[strArray[i]] = numArray[i];
            }
            return dictionary;
        }

        [Obsolete("Method GetAssetBundleNames has been deprecated. Use GetAllAssetBundleNames instead.")]
        public string[] GetAssetBundleNames()
        {
            return GetAllAssetBundleNames();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern int GetAssetHashFromPath(string path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetAssetOrScenePath(UnityEngine.Object assetObject);
        public static string GetAssetPath(int instanceID)
        {
            return GetAssetPathFromInstanceID(instanceID);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetAssetPath(UnityEngine.Object assetObject);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern string GetAssetPathFromInstanceID(int instanceID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetAssetPathFromTextMetaFilePath(string path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string[] GetAssetPathsFromAssetBundle(string assetBundleName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string[] GetAssetPathsFromAssetBundleAndAssetName(string assetBundleName, string assetName);
        public static T GetBuiltinExtraResource<T>(string path) where T: UnityEngine.Object
        {
            return (T) GetBuiltinExtraResource(typeof(T), path);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
        public static extern UnityEngine.Object GetBuiltinExtraResource(System.Type type, string path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Texture GetCachedIcon(string path);
        private static string[] GetDependencies(string pathName)
        {
            return GetDependencies(new string[] { pathName });
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string[] GetDependencies(string[] pathNames);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern int GetInstanceIDFromGUID(string guid);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string[] GetLabels(UnityEngine.Object obj);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern int GetMainAssetInstanceID(string assetPath);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string[] GetSubFolders(string path);
        [Obsolete("GetTextMetaDataPathFromAssetPath has been renamed to GetTextMetaFilePathFromAssetPath (UnityUpgradable) -> GetTextMetaFilePathFromAssetPath(*)")]
        public static string GetTextMetaDataPathFromAssetPath(string path)
        {
            return null;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetTextMetaFilePathFromAssetPath(string path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string GetUniquePathNameAtSelectedPath(string fileName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string[] GetUnusedAssetBundleNames();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GUIDToAssetPath(string guid);
        [ExcludeFromDocs]
        public static void ImportAsset(string path)
        {
            ImportAssetOptions options = ImportAssetOptions.Default;
            ImportAsset(path, options);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ImportAsset(string path, [DefaultValue("ImportAssetOptions.Default")] ImportAssetOptions options);
        public static void ImportPackage(string packagePath, bool interactive)
        {
            string str;
            AssetsItem[] items = AssetServer.ImportPackageStep1(packagePath, out str);
            if (items != null)
            {
                if (interactive)
                {
                    PackageImport.ShowImportPackage(packagePath, items, str);
                }
                else
                {
                    AssetServer.ImportPackageStep2(items);
                }
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_GetAllLabels(out string[] labels, out float[] scores);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool IsForeignAsset(int instanceID);
        public static bool IsForeignAsset(UnityEngine.Object obj)
        {
            return IsForeignAsset(obj.GetInstanceID());
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool IsMainAsset(int instanceID);
        public static bool IsMainAsset(UnityEngine.Object obj)
        {
            return IsMainAsset(obj.GetInstanceID());
        }

        public static bool IsMetaFileOpenForEdit(UnityEngine.Object assetObject)
        {
            string str;
            return IsMetaFileOpenForEdit(assetObject, out str);
        }

        public static bool IsMetaFileOpenForEdit(UnityEngine.Object assetObject, out string message)
        {
            return IsOpenForEdit(GetTextMetaFilePathFromAssetPath(GetAssetOrScenePath(assetObject)), out message);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool IsNativeAsset(int instanceID);
        public static bool IsNativeAsset(UnityEngine.Object obj)
        {
            return IsNativeAsset(obj.GetInstanceID());
        }

        public static bool IsOpenForEdit(string assetOrMetaFilePath)
        {
            string str;
            return IsOpenForEdit(assetOrMetaFilePath, out str);
        }

        public static bool IsOpenForEdit(UnityEngine.Object assetObject)
        {
            return IsOpenForEdit(GetAssetOrScenePath(assetObject));
        }

        public static bool IsOpenForEdit(string assetOrMetaFilePath, out string message)
        {
            return AssetModificationProcessorInternal.IsOpenForEdit(assetOrMetaFilePath, out message);
        }

        public static bool IsOpenForEdit(UnityEngine.Object assetObject, out string message)
        {
            return IsOpenForEdit(GetAssetOrScenePath(assetObject), out message);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool IsSubAsset(int instanceID);
        public static bool IsSubAsset(UnityEngine.Object obj)
        {
            return IsSubAsset(obj.GetInstanceID());
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool IsValidFolder(string path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern UnityEngine.Object[] LoadAllAssetRepresentationsAtPath(string assetPath);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern UnityEngine.Object[] LoadAllAssetsAtPath(string assetPath);
        public static T LoadAssetAtPath<T>(string assetPath) where T: UnityEngine.Object
        {
            return (T) LoadAssetAtPath(assetPath, typeof(T));
        }

        [MethodImpl(MethodImplOptions.InternalCall), TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument), WrapperlessIcall]
        public static extern UnityEngine.Object LoadAssetAtPath(string assetPath, System.Type type);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern UnityEngine.Object LoadMainAssetAtPath(string assetPath);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string[] MatchLabelsPartial(UnityEngine.Object obj, string partial);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string MoveAsset(string oldPath, string newPath);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool MoveAssetToTrash(string path);
        [ExcludeFromDocs]
        public static bool OpenAsset(int instanceID)
        {
            int lineNumber = -1;
            return OpenAsset(instanceID, lineNumber);
        }

        [ExcludeFromDocs]
        public static bool OpenAsset(UnityEngine.Object target)
        {
            int lineNumber = -1;
            return OpenAsset(target, lineNumber);
        }

        public static bool OpenAsset(UnityEngine.Object[] objects)
        {
            bool flag = true;
            foreach (UnityEngine.Object obj2 in objects)
            {
                if (!OpenAsset(obj2))
                {
                    flag = false;
                }
            }
            return flag;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool OpenAsset(int instanceID, [DefaultValue("-1")] int lineNumber);
        public static bool OpenAsset(UnityEngine.Object target, [DefaultValue("-1")] int lineNumber)
        {
            return ((target != null) && OpenAsset(target.GetInstanceID(), lineNumber));
        }

        [ExcludeFromDocs]
        public static void Refresh()
        {
            ImportAssetOptions options = ImportAssetOptions.Default;
            Refresh(options);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Refresh([DefaultValue("ImportAssetOptions.Default")] ImportAssetOptions options);
        [Obsolete("Please use AssetDatabase.Refresh instead", true)]
        public static void RefreshDelayed()
        {
        }

        [Obsolete("Please use AssetDatabase.Refresh instead", true)]
        public static void RefreshDelayed(ImportAssetOptions options)
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool RemoveAssetBundleName(string assetBundleName, bool forceRemove);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void RemoveUnusedAssetBundleNames();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string RenameAsset(string pathName, string newName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SaveAssets();
        private static string[] SearchAllAssets(SearchFilter searchFilter)
        {
            HierarchyProperty property = new HierarchyProperty(HierarchyType.Assets);
            property.SetSearchFilter(searchFilter);
            property.Reset();
            List<string> list = new List<string>();
            while (property.Next(null))
            {
                list.Add(property.guid);
            }
            return list.ToArray();
        }

        private static string[] SearchInFolders(SearchFilter searchFilter)
        {
            HierarchyProperty property = new HierarchyProperty(HierarchyType.Assets);
            List<string> list = new List<string>();
            foreach (string str in searchFilter.folders)
            {
                property.SetSearchFilter(new SearchFilter());
                int mainAssetInstanceID = GetMainAssetInstanceID(str);
                if (property.Find(mainAssetInstanceID, null))
                {
                    property.SetSearchFilter(searchFilter);
                    int depth = property.depth;
                    int[] expanded = null;
                    while (property.NextWithDepthCheck(expanded, depth + 1))
                    {
                        list.Add(property.guid);
                    }
                }
                else
                {
                    Debug.LogWarning("AssetDatabase.FindAssets: Folder not found: '" + str + "'");
                }
            }
            return list.ToArray();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetLabels(UnityEngine.Object obj, string[] labels);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void StartAssetEditing();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void StopAssetEditing();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string ValidateMoveAsset(string oldPath, string newPath);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool WriteImportSettingsIfDirty(string path);

        internal static bool isLocked { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

