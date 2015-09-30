namespace UnityEditor
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class ObjectNames
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetClassName(UnityEngine.Object obj);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetDragAndDropTitle(UnityEngine.Object obj);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetInspectorTitle(UnityEngine.Object obj);
        [Obsolete("Please use GetInspectorTitle instead")]
        public static string GetPropertyEditorTitle(UnityEngine.Object obj)
        {
            return GetInspectorTitle(obj);
        }

        internal static string GetTypeName(UnityEngine.Object obj)
        {
            string path = AssetDatabase.GetAssetPath(obj).ToLower();
            if (path.EndsWith(".unity"))
            {
                return "Scene";
            }
            if (path.EndsWith(".guiskin"))
            {
                return "GUI Skin";
            }
            if (Directory.Exists(AssetDatabase.GetAssetPath(obj)))
            {
                return "Folder";
            }
            if (obj.GetType() == typeof(UnityEngine.Object))
            {
                return (Path.GetExtension(path) + " File");
            }
            return GetClassName(obj);
        }

        [Obsolete("Please use NicifyVariableName instead")]
        public static string MangleVariableName(string name)
        {
            return NicifyVariableName(name);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string NicifyVariableName(string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetNameSmart(UnityEngine.Object obj, string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SetNameSmartWithInstanceID(int instanceID, string name);
    }
}

