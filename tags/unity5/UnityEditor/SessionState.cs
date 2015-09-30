namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal sealed class SessionState
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void EraseBool(string key);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void EraseFloat(string key);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void EraseInt(string key);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void EraseIntArray(string key);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void EraseString(string key);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void EraseVector3(string key);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool GetBool(string key, bool defaultValue);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern float GetFloat(string key, float defaultValue);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern int GetInt(string key, int defaultValue);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern int[] GetIntArray(string key, int[] defaultValue);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string GetString(string key, string defaultValue);
        internal static Vector3 GetVector3(string key, Vector3 defaultValue)
        {
            return INTERNAL_CALL_GetVector3(key, ref defaultValue);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Vector3 INTERNAL_CALL_GetVector3(string key, ref Vector3 defaultValue);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetVector3(string key, ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SetBool(string key, bool value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SetFloat(string key, float value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SetInt(string key, int value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SetIntArray(string key, int[] value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SetString(string key, string value);
        internal static void SetVector3(string key, Vector3 value)
        {
            INTERNAL_CALL_SetVector3(key, ref value);
        }
    }
}

