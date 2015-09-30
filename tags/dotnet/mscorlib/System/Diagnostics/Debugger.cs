namespace System.Diagnostics
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;

    [ComVisible(true)]
    public sealed class Debugger
    {
        public static readonly string DefaultCategory;

        public static void Break()
        {
            if (!IsDebuggerAttached())
            {
                try
                {
                    new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
                }
                catch (SecurityException)
                {
                    return;
                }
            }
            BreakInternal();
        }

        private static void BreakCanThrow()
        {
            if (!IsDebuggerAttached())
            {
                new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
            }
            BreakInternal();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void BreakInternal();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool IsDebuggerAttached();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool IsLogging();
        public static bool Launch()
        {
            if (IsDebuggerAttached())
            {
                return true;
            }
            try
            {
                new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
            }
            catch (SecurityException)
            {
                return false;
            }
            return LaunchInternal();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool LaunchInternal();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void Log(int level, string category, string message);

        public static bool IsAttached
        {
            get
            {
                return IsDebuggerAttached();
            }
        }
    }
}

