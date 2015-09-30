namespace System
{
    using Microsoft.Win32;
    using System.Collections;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Resources;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Text;
    using System.Threading;

    [ComVisible(true)]
    public static class Environment
    {
        private static bool isUserNonInteractive;
        private static OperatingSystem m_os;
        private static OSName m_osname;
        private static ResourceHelper m_resHelper;
        private const int MaximumLength = 0x7fff;
        private const int MaxMachineNameLength = 0x100;
        private static IntPtr processWinStation;
        private static volatile bool s_CheckedOSW2k3;
        private static object s_InternalSyncObject;
        private static bool s_IsW2k3;

        private static void CheckEnvironmentVariableName(string variable)
        {
            if (variable == null)
            {
                throw new ArgumentNullException("variable");
            }
            if (variable.Length == 0)
            {
                throw new ArgumentException(GetResourceString("Argument_StringZeroLength"), "variable");
            }
            if (variable[0] == '\0')
            {
                throw new ArgumentException(GetResourceString("Argument_StringFirstCharIsZero"), "variable");
            }
            if (variable.Length >= 0x7fff)
            {
                throw new ArgumentException(GetResourceString("Argument_LongEnvVarValue"));
            }
            if (variable.IndexOf('=') != -1)
            {
                throw new ArgumentException(GetResourceString("Argument_IllegalEnvVarName"));
            }
        }

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void Exit(int exitCode)
        {
            ExitNative(exitCode);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void ExitNative(int exitCode);
        public static string ExpandEnvironmentVariables(string name)
        {
            int num2;
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (name.Length == 0)
            {
                return name;
            }
            string[] strArray = name.Split(new char[] { '%' });
            StringBuilder builder = new StringBuilder();
            int capacity = 100;
            StringBuilder lpDst = new StringBuilder(capacity);
            for (int i = 1; i < (strArray.Length - 1); i++)
            {
                if (strArray[i].Length != 0)
                {
                    lpDst.Length = 0;
                    string lpSrc = "%" + strArray[i] + "%";
                    num2 = Win32Native.ExpandEnvironmentStrings(lpSrc, lpDst, capacity);
                    if (num2 == 0)
                    {
                        Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                    }
                    while (num2 > capacity)
                    {
                        capacity = num2;
                        lpDst.Capacity = capacity;
                        lpDst.Length = 0;
                        num2 = Win32Native.ExpandEnvironmentStrings(lpSrc, lpDst, capacity);
                        if (num2 == 0)
                        {
                            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                        }
                    }
                    if (lpDst.ToString() != lpSrc)
                    {
                        builder.Append(strArray[i]);
                        builder.Append(';');
                    }
                }
            }
            new EnvironmentPermission(EnvironmentPermissionAccess.Read, builder.ToString()).Demand();
            lpDst.Length = 0;
            num2 = Win32Native.ExpandEnvironmentStrings(name, lpDst, capacity);
            if (num2 == 0)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }
            while (num2 > capacity)
            {
                capacity = num2;
                lpDst.Capacity = capacity;
                lpDst.Length = 0;
                num2 = Win32Native.ExpandEnvironmentStrings(name, lpDst, capacity);
                if (num2 == 0)
                {
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }
            }
            return lpDst.ToString();
        }

        [MethodImpl(MethodImplOptions.InternalCall), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static extern void FailFast(string message);
        public static string[] GetCommandLineArgs()
        {
            new EnvironmentPermission(EnvironmentPermissionAccess.Read, "Path").Demand();
            return GetCommandLineArgsNative();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern string[] GetCommandLineArgsNative();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern string GetCommandLineNative();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool GetCompatibilityFlag(CompatibilityFlag flag);
        public static string GetEnvironmentVariable(string variable)
        {
            if (variable == null)
            {
                throw new ArgumentNullException("variable");
            }
            new EnvironmentPermission(EnvironmentPermissionAccess.Read, variable).Demand();
            StringBuilder lpValue = new StringBuilder(0x80);
            int num = Win32Native.GetEnvironmentVariable(variable, lpValue, lpValue.Capacity);
            if ((num != 0) || (Marshal.GetLastWin32Error() != 0xcb))
            {
                while (num > lpValue.Capacity)
                {
                    lpValue.Capacity = num;
                    lpValue.Length = 0;
                    num = Win32Native.GetEnvironmentVariable(variable, lpValue, lpValue.Capacity);
                }
                return lpValue.ToString();
            }
            return null;
        }

        public static string GetEnvironmentVariable(string variable, EnvironmentVariableTarget target)
        {
            if (target == EnvironmentVariableTarget.Process)
            {
                return GetEnvironmentVariable(variable);
            }
            if (variable == null)
            {
                throw new ArgumentNullException("variable");
            }
            if (IsWin9X())
            {
                throw new NotSupportedException(GetResourceString("PlatformNotSupported_Win9x"));
            }
            new EnvironmentPermission(PermissionState.Unrestricted).Demand();
            if (target == EnvironmentVariableTarget.Machine)
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"System\CurrentControlSet\Control\Session Manager\Environment", false))
                {
                    if (key == null)
                    {
                        return null;
                    }
                    return (key.GetValue(variable) as string);
                }
            }
            if (target == EnvironmentVariableTarget.User)
            {
                using (RegistryKey key2 = Registry.CurrentUser.OpenSubKey("Environment", false))
                {
                    if (key2 == null)
                    {
                        return null;
                    }
                    return (key2.GetValue(variable) as string);
                }
            }
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, GetResourceString("Arg_EnumIllegalVal"), new object[] { (int) target }));
        }

        public static IDictionary GetEnvironmentVariables()
        {
            char[] chArray = nativeGetEnvironmentCharArray();
            if (chArray == null)
            {
                throw new OutOfMemoryException();
            }
            Hashtable hashtable = new Hashtable(20);
            StringBuilder builder = new StringBuilder();
            bool flag = true;
            for (int i = 0; i < chArray.Length; i++)
            {
                int startIndex = i;
                while ((chArray[i] != '=') && (chArray[i] != '\0'))
                {
                    i++;
                }
                if (chArray[i] != '\0')
                {
                    if ((i - startIndex) == 0)
                    {
                        while (chArray[i] != '\0')
                        {
                            i++;
                        }
                    }
                    else
                    {
                        string str = new string(chArray, startIndex, i - startIndex);
                        i++;
                        int num3 = i;
                        while (chArray[i] != '\0')
                        {
                            i++;
                        }
                        string str2 = new string(chArray, num3, i - num3);
                        hashtable[str] = str2;
                        if (flag)
                        {
                            flag = false;
                        }
                        else
                        {
                            builder.Append(';');
                        }
                        builder.Append(str);
                    }
                }
            }
            new EnvironmentPermission(EnvironmentPermissionAccess.Read, builder.ToString()).Demand();
            return hashtable;
        }

        public static IDictionary GetEnvironmentVariables(EnvironmentVariableTarget target)
        {
            if (target == EnvironmentVariableTarget.Process)
            {
                return GetEnvironmentVariables();
            }
            if (IsWin9X())
            {
                throw new NotSupportedException(GetResourceString("PlatformNotSupported_Win9x"));
            }
            new EnvironmentPermission(PermissionState.Unrestricted).Demand();
            if (target == EnvironmentVariableTarget.Machine)
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"System\CurrentControlSet\Control\Session Manager\Environment", false))
                {
                    return GetRegistryKeyNameValuePairs(key);
                }
            }
            if (target == EnvironmentVariableTarget.User)
            {
                using (RegistryKey key2 = Registry.CurrentUser.OpenSubKey("Environment", false))
                {
                    return GetRegistryKeyNameValuePairs(key2);
                }
            }
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, GetResourceString("Arg_EnumIllegalVal"), new object[] { (int) target }));
        }

        public static string GetFolderPath(SpecialFolder folder)
        {
            if (!Enum.IsDefined(typeof(SpecialFolder), folder))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, GetResourceString("Arg_EnumIllegalVal"), new object[] { (int) folder }));
            }
            StringBuilder lpszPath = new StringBuilder(260);
            Win32Native.SHGetFolderPath(IntPtr.Zero, (int) folder, IntPtr.Zero, 0, lpszPath);
            string path = lpszPath.ToString();
            new FileIOPermission(FileIOPermissionAccess.PathDiscovery, path).Demand();
            return path;
        }

        public static string[] GetLogicalDrives()
        {
            new EnvironmentPermission(PermissionState.Unrestricted).Demand();
            int logicalDrives = Win32Native.GetLogicalDrives();
            if (logicalDrives == 0)
            {
                __Error.WinIOError();
            }
            uint num2 = (uint) logicalDrives;
            int num3 = 0;
            while (num2 != 0)
            {
                if ((num2 & 1) != 0)
                {
                    num3++;
                }
                num2 = num2 >> 1;
            }
            string[] strArray = new string[num3];
            char[] chArray = new char[] { 'A', ':', '\\' };
            num2 = (uint) logicalDrives;
            num3 = 0;
            while (num2 != 0)
            {
                if ((num2 & 1) != 0)
                {
                    strArray[num3++] = new string(chArray);
                }
                num2 = num2 >> 1;
                chArray[0] = (char) (chArray[0] + '\x0001');
            }
            return strArray;
        }

        internal static IDictionary GetRegistryKeyNameValuePairs(RegistryKey registryKey)
        {
            Hashtable hashtable = new Hashtable(20);
            if (registryKey != null)
            {
                foreach (string str in registryKey.GetValueNames())
                {
                    string str2 = registryKey.GetValue(str, "").ToString();
                    hashtable.Add(str, str2);
                }
            }
            return hashtable;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern string GetResourceFromDefault(string key);
        internal static string GetResourceString(string key)
        {
            return GetResourceFromDefault(key);
        }

        internal static string GetResourceString(string key, params object[] values)
        {
            string resourceFromDefault = GetResourceFromDefault(key);
            return string.Format(CultureInfo.CurrentCulture, resourceFromDefault, values);
        }

        internal static string GetResourceStringLocal(string key)
        {
            if (m_resHelper == null)
            {
                InitResourceHelper();
            }
            return m_resHelper.GetResourceString(key);
        }

        internal static string GetStackTrace(Exception e, bool needFileInfo)
        {
            System.Diagnostics.StackTrace trace;
            if (e == null)
            {
                trace = new System.Diagnostics.StackTrace(needFileInfo);
            }
            else
            {
                trace = new System.Diagnostics.StackTrace(e, needFileInfo);
            }
            return trace.ToString(System.Diagnostics.StackTrace.TraceFormat.Normal);
        }

        private static void InitResourceHelper()
        {
            bool flag = false;
            bool flag2 = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                }
                finally
                {
                    Thread.BeginCriticalRegion();
                    flag = true;
                    Monitor.Enter(InternalSyncObject);
                    flag2 = true;
                }
                if (m_resHelper == null)
                {
                    ResourceHelper helper = new ResourceHelper();
                    Thread.MemoryBarrier();
                    m_resHelper = helper;
                }
            }
            finally
            {
                if (flag2)
                {
                    Monitor.Exit(InternalSyncObject);
                }
                if (flag)
                {
                    Thread.EndCriticalRegion();
                }
            }
        }

        internal static bool IsWin9X()
        {
            return (OSVersion.Platform == PlatformID.Win32Windows);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern char[] nativeGetEnvironmentCharArray();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern string nativeGetEnvironmentVariable(string variable);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int nativeGetExitCode();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int nativeGetTickCount();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern long nativeGetWorkingSet();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool nativeHasShutdownStarted();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool nativeIsWin9x();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void nativeSetExitCode(int exitCode);
        public static void SetEnvironmentVariable(string variable, string value)
        {
            CheckEnvironmentVariableName(variable);
            new EnvironmentPermission(PermissionState.Unrestricted).Demand();
            if (string.IsNullOrEmpty(value) || (value[0] == '\0'))
            {
                value = null;
            }
            else if (value.Length >= 0x7fff)
            {
                throw new ArgumentException(GetResourceString("Argument_LongEnvVarValue"));
            }
            if (!Win32Native.SetEnvironmentVariable(variable, value))
            {
                int errorCode = Marshal.GetLastWin32Error();
                switch (errorCode)
                {
                    case 0xcb:
                        return;

                    case 0xce:
                        throw new ArgumentException(GetResourceString("Argument_LongEnvVarValue"));
                }
                throw new ArgumentException(Win32Native.GetMessage(errorCode));
            }
        }

        public static void SetEnvironmentVariable(string variable, string value, EnvironmentVariableTarget target)
        {
            if (target == EnvironmentVariableTarget.Process)
            {
                SetEnvironmentVariable(variable, value);
                return;
            }
            CheckEnvironmentVariableName(variable);
            if (variable.Length >= 0xff)
            {
                throw new ArgumentException(GetResourceString("Argument_LongEnvVarName"));
            }
            if (IsWin9X())
            {
                throw new NotSupportedException(GetResourceString("PlatformNotSupported_Win9x"));
            }
            new EnvironmentPermission(PermissionState.Unrestricted).Demand();
            if (string.IsNullOrEmpty(value) || (value[0] == '\0'))
            {
                value = null;
            }
            if (target == EnvironmentVariableTarget.Machine)
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"System\CurrentControlSet\Control\Session Manager\Environment", true))
                {
                    if (key != null)
                    {
                        if (value == null)
                        {
                            key.DeleteValue(variable, false);
                        }
                        else
                        {
                            key.SetValue(variable, value);
                        }
                    }
                    goto Label_0101;
                }
            }
            if (target == EnvironmentVariableTarget.User)
            {
                using (RegistryKey key2 = Registry.CurrentUser.OpenSubKey("Environment", true))
                {
                    if (key2 != null)
                    {
                        if (value == null)
                        {
                            key2.DeleteValue(variable, false);
                        }
                        else
                        {
                            key2.SetValue(variable, value);
                        }
                    }
                    goto Label_0101;
                }
            }
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, GetResourceString("Arg_EnumIllegalVal"), new object[] { (int) target }));
        Label_0101:
            bool flag1 = Win32Native.SendMessageTimeout(new IntPtr(0xffff), 0x1a, IntPtr.Zero, "Environment", 0, 0x3e8, IntPtr.Zero) == IntPtr.Zero;
        }

        public static string CommandLine
        {
            get
            {
                new EnvironmentPermission(EnvironmentPermissionAccess.Read, "Path").Demand();
                return GetCommandLineNative();
            }
        }

        public static string CurrentDirectory
        {
            get
            {
                return Directory.GetCurrentDirectory();
            }
            set
            {
                Directory.SetCurrentDirectory(value);
            }
        }

        public static int ExitCode
        {
            get
            {
                return nativeGetExitCode();
            }
            set
            {
                nativeSetExitCode(value);
            }
        }

        public static bool HasShutdownStarted
        {
            get
            {
                return nativeHasShutdownStarted();
            }
        }

        private static object InternalSyncObject
        {
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
            get
            {
                if (s_InternalSyncObject == null)
                {
                    object obj2 = new object();
                    Interlocked.CompareExchange(ref s_InternalSyncObject, obj2, null);
                }
                return s_InternalSyncObject;
            }
        }

        internal static string InternalWindowsDirectory
        {
            get
            {
                StringBuilder sb = new StringBuilder(260);
                if (Win32Native.GetWindowsDirectory(sb, 260) == 0)
                {
                    __Error.WinIOError();
                }
                return sb.ToString();
            }
        }

        internal static bool IsW2k3
        {
            get
            {
                if (!s_CheckedOSW2k3)
                {
                    OperatingSystem oSVersion = OSVersion;
                    s_IsW2k3 = ((oSVersion.Platform == PlatformID.Win32NT) && (oSVersion.Version.Major == 5)) && (oSVersion.Version.Minor == 2);
                    s_CheckedOSW2k3 = true;
                }
                return s_IsW2k3;
            }
        }

        public static string MachineName
        {
            get
            {
                new EnvironmentPermission(EnvironmentPermissionAccess.Read, "COMPUTERNAME").Demand();
                StringBuilder nameBuffer = new StringBuilder(0x100);
                int bufferSize = 0x100;
                if (Win32Native.GetComputerName(nameBuffer, ref bufferSize) == 0)
                {
                    throw new InvalidOperationException(GetResourceString("InvalidOperation_ComputerName"));
                }
                return nameBuffer.ToString();
            }
        }

        public static string NewLine
        {
            get
            {
                return "\r\n";
            }
        }

        internal static OSName OSInfo
        {
            get
            {
                if (m_osname == OSName.Invalid)
                {
                    lock (InternalSyncObject)
                    {
                        Win32Native.OSVERSIONINFO osversioninfo;
                        if (m_osname == OSName.Invalid)
                        {
                            osversioninfo = new Win32Native.OSVERSIONINFO();
                            if (!Win32Native.GetVersionEx(osversioninfo))
                            {
                                throw new InvalidOperationException(GetResourceString("InvalidOperation_GetVersion"));
                            }
                            switch (osversioninfo.PlatformId)
                            {
                                case 1:
                                    switch (osversioninfo.MajorVersion)
                                    {
                                        case 4:
                                            goto Label_00BF;

                                        case 5:
                                            goto Label_00B6;
                                    }
                                    goto Label_00D9;

                                case 2:
                                    switch (osversioninfo.MajorVersion)
                                    {
                                        case 4:
                                            goto Label_0083;
                                    }
                                    goto Label_008F;

                                default:
                                    goto Label_00E2;
                            }
                            m_osname = OSName.Win2k;
                        }
                        goto Label_00F1;
                    Label_0083:
                        m_osname = OSName.Nt4;
                        goto Label_00F1;
                    Label_008F:
                        m_osname = OSName.WinNT;
                        goto Label_00F1;
                    Label_00B6:
                        m_osname = OSName.WinMe;
                        goto Label_00F1;
                    Label_00BF:
                        if (osversioninfo.MinorVersion == 0)
                        {
                            m_osname = OSName.Win95;
                        }
                        else
                        {
                            m_osname = OSName.Win98;
                        }
                        goto Label_00F1;
                    Label_00D9:
                        m_osname = OSName.Win9x;
                        goto Label_00F1;
                    Label_00E2:
                        m_osname = OSName.Unknown;
                    }
                }
            Label_00F1:
                return m_osname;
            }
        }

        public static OperatingSystem OSVersion
        {
            get
            {
                if (m_os == null)
                {
                    PlatformID winCE;
                    Win32Native.OSVERSIONINFO ver = new Win32Native.OSVERSIONINFO();
                    if (!Win32Native.GetVersionEx(ver))
                    {
                        throw new InvalidOperationException(GetResourceString("InvalidOperation_GetVersion"));
                    }
                    Win32Native.OSVERSIONINFOEX osversioninfoex = new Win32Native.OSVERSIONINFOEX();
                    if ((ver.PlatformId != 1) && !Win32Native.GetVersionEx(osversioninfoex))
                    {
                        throw new InvalidOperationException(GetResourceString("InvalidOperation_GetVersion"));
                    }
                    switch (ver.PlatformId)
                    {
                        case 0:
                            winCE = PlatformID.Win32S;
                            break;

                        case 1:
                            winCE = PlatformID.Win32Windows;
                            break;

                        case 2:
                            winCE = PlatformID.Win32NT;
                            break;

                        case 3:
                            winCE = PlatformID.WinCE;
                            break;

                        default:
                            throw new InvalidOperationException(GetResourceString("InvalidOperation_InvalidPlatformID"));
                    }
                    System.Version version = new System.Version(ver.MajorVersion, ver.MinorVersion, ver.BuildNumber, (osversioninfoex.ServicePackMajor << 0x10) | osversioninfoex.ServicePackMinor);
                    m_os = new OperatingSystem(winCE, version, ver.CSDVersion);
                }
                return m_os;
            }
        }

        public static int ProcessorCount
        {
            get
            {
                Win32Native.SYSTEM_INFO lpSystemInfo = new Win32Native.SYSTEM_INFO();
                Win32Native.GetSystemInfo(ref lpSystemInfo);
                return lpSystemInfo.dwNumberOfProcessors;
            }
        }

        internal static bool RunningOnWinNT
        {
            get
            {
                return (OSVersion.Platform == PlatformID.Win32NT);
            }
        }

        public static string StackTrace
        {
            get
            {
                new EnvironmentPermission(PermissionState.Unrestricted).Demand();
                return GetStackTrace(null, true);
            }
        }

        public static string SystemDirectory
        {
            get
            {
                StringBuilder sb = new StringBuilder(260);
                if (Win32Native.GetSystemDirectory(sb, 260) == 0)
                {
                    __Error.WinIOError();
                }
                string path = sb.ToString();
                new FileIOPermission(FileIOPermissionAccess.PathDiscovery, path).Demand();
                return path;
            }
        }

        public static int TickCount
        {
            get
            {
                return nativeGetTickCount();
            }
        }

        public static string UserDomainName
        {
            get
            {
                int num3;
                new EnvironmentPermission(EnvironmentPermissionAccess.Read, "UserDomain").Demand();
                byte[] sid = new byte[0x400];
                int length = sid.Length;
                StringBuilder domainName = new StringBuilder(0x400);
                int capacity = domainName.Capacity;
                if (OSVersion.Platform == PlatformID.Win32NT)
                {
                    if (Win32Native.GetUserNameEx(2, domainName, ref capacity) == 1)
                    {
                        string str = domainName.ToString();
                        int index = str.IndexOf('\\');
                        if (index != -1)
                        {
                            return str.Substring(0, index);
                        }
                    }
                    capacity = domainName.Capacity;
                }
                if (Win32Native.LookupAccountName(null, UserName, sid, ref length, domainName, ref capacity, out num3))
                {
                    return domainName.ToString();
                }
                if (Marshal.GetLastWin32Error() == 120)
                {
                    throw new PlatformNotSupportedException(GetResourceString("PlatformNotSupported_Win9x"));
                }
                throw new InvalidOperationException(GetResourceString("InvalidOperation_UserDomainName"));
            }
        }

        public static bool UserInteractive
        {
            get
            {
                if ((OSInfo & OSName.WinNT) == OSName.WinNT)
                {
                    IntPtr processWindowStation = Win32Native.GetProcessWindowStation();
                    if ((processWindowStation != IntPtr.Zero) && (processWinStation != processWindowStation))
                    {
                        int lpnLengthNeeded = 0;
                        Win32Native.USEROBJECTFLAGS pvBuffer = new Win32Native.USEROBJECTFLAGS();
                        if (Win32Native.GetUserObjectInformation(processWindowStation, 1, pvBuffer, Marshal.SizeOf(pvBuffer), ref lpnLengthNeeded) && ((pvBuffer.dwFlags & 1) == 0))
                        {
                            isUserNonInteractive = true;
                        }
                        processWinStation = processWindowStation;
                    }
                }
                return !isUserNonInteractive;
            }
        }

        public static string UserName
        {
            get
            {
                new EnvironmentPermission(EnvironmentPermissionAccess.Read, "UserName").Demand();
                StringBuilder lpBuffer = new StringBuilder(0x100);
                int capacity = lpBuffer.Capacity;
                Win32Native.GetUserName(lpBuffer, ref capacity);
                return lpBuffer.ToString();
            }
        }

        public static System.Version Version
        {
            get
            {
                return new System.Version("2.0.50727.8662");
            }
        }

        public static long WorkingSet
        {
            get
            {
                new EnvironmentPermission(PermissionState.Unrestricted).Demand();
                return nativeGetWorkingSet();
            }
        }

        [Serializable]
        internal enum OSName
        {
            Invalid = 0,
            Nt4 = 0x81,
            Unknown = 1,
            Win2k = 130,
            Win95 = 0x41,
            Win98 = 0x42,
            Win9x = 0x40,
            WinMe = 0x43,
            WinNT = 0x80
        }

        internal sealed class ResourceHelper
        {
            private Stack currentlyLoading;
            internal bool resourceManagerInited;
            private ResourceManager SystemResMgr;

            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
            internal string GetResourceString(string key)
            {
                if ((key == null) || (key.Length == 0))
                {
                    return "[Resource lookup failed - null or empty resource name]";
                }
                GetResourceStringUserData userData = new GetResourceStringUserData(this, key);
                RuntimeHelpers.TryCode code = new RuntimeHelpers.TryCode(this.GetResourceStringCode);
                RuntimeHelpers.CleanupCode backoutCode = new RuntimeHelpers.CleanupCode(this.GetResourceStringBackoutCode);
                RuntimeHelpers.ExecuteCodeWithGuaranteedCleanup(code, backoutCode, userData);
                return userData.m_retVal;
            }

            [PrePrepareMethod]
            private void GetResourceStringBackoutCode(object userDataIn, bool exceptionThrown)
            {
                GetResourceStringUserData data = (GetResourceStringUserData) userDataIn;
                Environment.ResourceHelper resourceHelper = data.m_resourceHelper;
                if (exceptionThrown && data.m_lockWasTaken)
                {
                    resourceHelper.SystemResMgr = null;
                    resourceHelper.currentlyLoading = null;
                }
                if (data.m_lockWasTaken)
                {
                    Monitor.Exit(resourceHelper);
                }
            }

            private void GetResourceStringCode(object userDataIn)
            {
                GetResourceStringUserData data = (GetResourceStringUserData) userDataIn;
                Environment.ResourceHelper resourceHelper = data.m_resourceHelper;
                string key = data.m_key;
                Monitor.ReliableEnter(resourceHelper, ref data.m_lockWasTaken);
                if (((resourceHelper.currentlyLoading != null) && (resourceHelper.currentlyLoading.Count > 0)) && resourceHelper.currentlyLoading.Contains(key))
                {
                    try
                    {
                        new StackTrace(true).ToString(StackTrace.TraceFormat.NoResourceLookup);
                    }
                    catch (StackOverflowException)
                    {
                    }
                    catch (NullReferenceException)
                    {
                    }
                    catch (OutOfMemoryException)
                    {
                    }
                    data.m_retVal = "[Resource lookup failed - infinite recursion or critical failure detected.]";
                }
                else
                {
                    if (resourceHelper.currentlyLoading == null)
                    {
                        resourceHelper.currentlyLoading = new Stack(4);
                    }
                    if (!resourceHelper.resourceManagerInited)
                    {
                        RuntimeHelpers.PrepareConstrainedRegions();
                        try
                        {
                        }
                        finally
                        {
                            RuntimeHelpers.RunClassConstructor(typeof(ResourceManager).TypeHandle);
                            RuntimeHelpers.RunClassConstructor(typeof(ResourceReader).TypeHandle);
                            RuntimeHelpers.RunClassConstructor(typeof(RuntimeResourceSet).TypeHandle);
                            RuntimeHelpers.RunClassConstructor(typeof(BinaryReader).TypeHandle);
                            resourceHelper.resourceManagerInited = true;
                        }
                    }
                    resourceHelper.currentlyLoading.Push(key);
                    if (resourceHelper.SystemResMgr == null)
                    {
                        resourceHelper.SystemResMgr = new ResourceManager("mscorlib", typeof(object).Assembly);
                    }
                    string str2 = resourceHelper.SystemResMgr.GetString(key, null);
                    resourceHelper.currentlyLoading.Pop();
                    data.m_retVal = str2;
                }
            }

            internal class GetResourceStringUserData
            {
                public string m_key;
                public bool m_lockWasTaken;
                public Environment.ResourceHelper m_resourceHelper;
                public string m_retVal;

                public GetResourceStringUserData(Environment.ResourceHelper resourceHelper, string key)
                {
                    this.m_resourceHelper = resourceHelper;
                    this.m_key = key;
                }
            }
        }

        [ComVisible(true)]
        public enum SpecialFolder
        {
            ApplicationData = 0x1a,
            CommonApplicationData = 0x23,
            CommonProgramFiles = 0x2b,
            Cookies = 0x21,
            Desktop = 0,
            DesktopDirectory = 0x10,
            Favorites = 6,
            History = 0x22,
            InternetCache = 0x20,
            LocalApplicationData = 0x1c,
            MyComputer = 0x11,
            MyDocuments = 5,
            MyMusic = 13,
            MyPictures = 0x27,
            Personal = 5,
            ProgramFiles = 0x26,
            Programs = 2,
            Recent = 8,
            SendTo = 9,
            StartMenu = 11,
            Startup = 7,
            System = 0x25,
            Templates = 0x15
        }
    }
}

