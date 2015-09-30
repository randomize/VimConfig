namespace UnityEngine.Analytics
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class UnityAnalyticsManager
    {
        public static string deviceUniqueIdentifier { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool enabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool initializeOnStartup { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static string testConfigUrl { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static string testEventUrl { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool testMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static string unityAdsId { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool unityAdsTrackingEnabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

