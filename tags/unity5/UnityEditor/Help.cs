namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class Help
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void BrowseURL(string url);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetHelpURLForObject(UnityEngine.Object obj);
        internal static string GetNiceHelpNameForObject(UnityEngine.Object obj)
        {
            return GetNiceHelpNameForObject(obj, true);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string GetNiceHelpNameForObject(UnityEngine.Object obj, bool defaultToMonoBehaviour);
        public static bool HasHelpForObject(UnityEngine.Object obj)
        {
            return HasHelpForObject(obj, true);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool HasHelpForObject(UnityEngine.Object obj, bool defaultToMonoBehaviour);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ShowHelpForObject(UnityEngine.Object obj);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ShowHelpPage(string page);
    }
}

