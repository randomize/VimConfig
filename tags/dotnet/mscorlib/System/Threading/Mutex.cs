namespace System.Threading
{
    using Microsoft.Win32;
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security.AccessControl;
    using System.Security.Permissions;

    [ComVisible(true), HostProtection(SecurityAction.LinkDemand, Synchronization=true, ExternalThreading=true)]
    public sealed class Mutex : WaitHandle
    {
        private static bool dummyBool;
        private const int WAIT_ABANDONED_0 = 0x80;
        private const uint WAIT_FAILED = uint.MaxValue;
        private const int WAIT_OBJECT_0 = 0;

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public Mutex() : this(false, null, out dummyBool)
        {
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        private Mutex(SafeWaitHandle handle)
        {
            base.SetHandleInternal(handle);
            base.hasThreadAffinity = true;
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public Mutex(bool initiallyOwned) : this(initiallyOwned, null, out dummyBool)
        {
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public Mutex(bool initiallyOwned, string name) : this(initiallyOwned, name, out dummyBool)
        {
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public Mutex(bool initiallyOwned, string name, out bool createdNew) : this(initiallyOwned, name, out createdNew, null)
        {
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public unsafe Mutex(bool initiallyOwned, string name, out bool createdNew, MutexSecurity mutexSecurity)
        {
            if ((name != null) && (260 < name.Length))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_WaitHandleNameTooLong", new object[] { name }));
            }
            Win32Native.SECURITY_ATTRIBUTES secAttrs = null;
            if (mutexSecurity != null)
            {
                secAttrs = new Win32Native.SECURITY_ATTRIBUTES {
                    nLength = Marshal.SizeOf(secAttrs)
                };
                byte[] securityDescriptorBinaryForm = mutexSecurity.GetSecurityDescriptorBinaryForm();
                byte* pDest = stackalloc byte[1 * securityDescriptorBinaryForm.Length];
                Buffer.memcpy(securityDescriptorBinaryForm, 0, pDest, 0, securityDescriptorBinaryForm.Length);
                secAttrs.pSecurityDescriptor = pDest;
            }
            SafeWaitHandle mutexHandle = null;
            bool newMutex = false;
            RuntimeHelpers.CleanupCode backoutCode = new RuntimeHelpers.CleanupCode(this.MutexCleanupCode);
            MutexCleanupInfo cleanupInfo = new MutexCleanupInfo(mutexHandle, false);
            RuntimeHelpers.ExecuteCodeWithGuaranteedCleanup(delegate (object userData) {
                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                }
                finally
                {
                    if (initiallyOwned)
                    {
                        cleanupInfo.inCriticalRegion = true;
                        Thread.BeginThreadAffinity();
                        Thread.BeginCriticalRegion();
                    }
                }
                int errorCode = 0;
                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                }
                finally
                {
                    errorCode = CreateMutexHandle(initiallyOwned, name, secAttrs, out mutexHandle);
                }
                if (mutexHandle.IsInvalid)
                {
                    mutexHandle.SetHandleAsInvalid();
                    if (((name != null) && (name.Length != 0)) && (6 == errorCode))
                    {
                        throw new WaitHandleCannotBeOpenedException(Environment.GetResourceString("Threading.WaitHandleCannotBeOpenedException_InvalidHandle", new object[] { name }));
                    }
                    __Error.WinIOError(errorCode, name);
                }
                newMutex = errorCode != 0xb7;
                this.SetHandleInternal(mutexHandle);
                this.hasThreadAffinity = true;
            }, backoutCode, cleanupInfo);
            createdNew = newMutex;
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        private static int CreateMutexHandle(bool initiallyOwned, string name, Win32Native.SECURITY_ATTRIBUTES securityAttribute, out SafeWaitHandle mutexHandle)
        {
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
        Label_0006:
            flag2 = false;
            flag3 = false;
            mutexHandle = Win32Native.CreateMutex(securityAttribute, initiallyOwned, name);
            int num = Marshal.GetLastWin32Error();
            if (!mutexHandle.IsInvalid || (num != 5))
            {
                return num;
            }
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                }
                finally
                {
                    Thread.BeginThreadAffinity();
                    flag = true;
                }
                mutexHandle = Win32Native.OpenMutex(0x100001, false, name);
                if (!mutexHandle.IsInvalid)
                {
                    num = 0xb7;
                    if (Environment.IsW2k3)
                    {
                        SafeWaitHandle handle = Win32Native.OpenMutex(0x100001, false, name);
                        if (!handle.IsInvalid)
                        {
                            RuntimeHelpers.PrepareConstrainedRegions();
                            try
                            {
                                uint num2 = 0;
                                IntPtr ptr = mutexHandle.DangerousGetHandle();
                                IntPtr ptr2 = handle.DangerousGetHandle();
                                IntPtr[] handles = new IntPtr[] { ptr, ptr2 };
                                num2 = Win32Native.WaitForMultipleObjects(2, handles, true, 0);
                                GC.KeepAlive(handles);
                                if (num2 == uint.MaxValue)
                                {
                                    if (Marshal.GetLastWin32Error() != 0x57)
                                    {
                                        mutexHandle.Dispose();
                                        flag3 = true;
                                    }
                                }
                                else
                                {
                                    flag2 = true;
                                    if ((num2 >= 0) && (num2 < 2))
                                    {
                                        Win32Native.ReleaseMutex(mutexHandle);
                                        Win32Native.ReleaseMutex(handle);
                                    }
                                    else if ((num2 >= 0x80) && (num2 < 130))
                                    {
                                        Win32Native.ReleaseMutex(mutexHandle);
                                        Win32Native.ReleaseMutex(handle);
                                    }
                                    mutexHandle.Dispose();
                                }
                                goto Label_0166;
                            }
                            finally
                            {
                                handle.Dispose();
                            }
                        }
                        mutexHandle.Dispose();
                        flag3 = true;
                    }
                }
                else
                {
                    num = Marshal.GetLastWin32Error();
                }
            }
            finally
            {
                if (flag)
                {
                    Thread.EndThreadAffinity();
                }
            }
        Label_0166:
            if ((flag2 || flag3) || (num == 2))
            {
                goto Label_0006;
            }
            if (num == 0)
            {
                num = 0xb7;
            }
            return num;
        }

        public MutexSecurity GetAccessControl()
        {
            return new MutexSecurity(base.safeWaitHandle, AccessControlSections.Group | AccessControlSections.Owner | AccessControlSections.Access);
        }

        [PrePrepareMethod]
        private void MutexCleanupCode(object userData, bool exceptionThrown)
        {
            MutexCleanupInfo info = (MutexCleanupInfo) userData;
            if (!base.hasThreadAffinity)
            {
                if ((info.mutexHandle != null) && !info.mutexHandle.IsInvalid)
                {
                    if (info.inCriticalRegion)
                    {
                        Win32Native.ReleaseMutex(info.mutexHandle);
                    }
                    info.mutexHandle.Dispose();
                }
                if (info.inCriticalRegion)
                {
                    Thread.EndCriticalRegion();
                    Thread.EndThreadAffinity();
                }
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static Mutex OpenExisting(string name)
        {
            return OpenExisting(name, MutexRights.Synchronize | MutexRights.Modify);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static Mutex OpenExisting(string name, MutexRights rights)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name", Environment.GetResourceString("ArgumentNull_WithParamName"));
            }
            if (name.Length == 0)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_EmptyName"), "name");
            }
            if (260 < name.Length)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_WaitHandleNameTooLong", new object[] { name }));
            }
            SafeWaitHandle handle = Win32Native.OpenMutex((int) rights, false, name);
            int errorCode = 0;
            if (handle.IsInvalid)
            {
                errorCode = Marshal.GetLastWin32Error();
                if ((2 == errorCode) || (0x7b == errorCode))
                {
                    throw new WaitHandleCannotBeOpenedException();
                }
                if (((name != null) && (name.Length != 0)) && (6 == errorCode))
                {
                    throw new WaitHandleCannotBeOpenedException(Environment.GetResourceString("Threading.WaitHandleCannotBeOpenedException_InvalidHandle", new object[] { name }));
                }
                __Error.WinIOError(errorCode, name);
            }
            return new Mutex(handle);
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public void ReleaseMutex()
        {
            if (!Win32Native.ReleaseMutex(base.safeWaitHandle))
            {
                throw new ApplicationException(Environment.GetResourceString("Arg_SynchronizationLockException"));
            }
            Thread.EndCriticalRegion();
            Thread.EndThreadAffinity();
        }

        public void SetAccessControl(MutexSecurity mutexSecurity)
        {
            if (mutexSecurity == null)
            {
                throw new ArgumentNullException("mutexSecurity");
            }
            mutexSecurity.Persist(base.safeWaitHandle);
        }

        internal class MutexCleanupInfo
        {
            internal bool inCriticalRegion;
            internal SafeWaitHandle mutexHandle;

            internal MutexCleanupInfo(SafeWaitHandle mutexHandle, bool inCriticalRegion)
            {
                this.mutexHandle = mutexHandle;
                this.inCriticalRegion = inCriticalRegion;
            }
        }
    }
}

