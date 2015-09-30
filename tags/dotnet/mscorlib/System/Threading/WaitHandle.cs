namespace System.Threading
{
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Security.Permissions;

    [ComVisible(true)]
    public abstract class WaitHandle : MarshalByRefObject, IDisposable
    {
        private const int ERROR_TOO_MANY_POSTS = 0x12a;
        internal bool hasThreadAffinity = false;
        protected static readonly IntPtr InvalidHandle = Win32Native.INVALID_HANDLE_VALUE;
        private const int MAX_WAITHANDLES = 0x40;
        internal Microsoft.Win32.SafeHandles.SafeWaitHandle safeWaitHandle = null;
        private const int WAIT_ABANDONED = 0x80;
        private const int WAIT_FAILED = 0x7fffffff;
        private const int WAIT_OBJECT_0 = 0;
        private IntPtr waitHandle = InvalidHandle;
        public const int WaitTimeout = 0x102;

        protected WaitHandle()
        {
        }

        public virtual void Close()
        {
            this.Dispose(true);
            GC.nativeSuppressFinalize(this);
        }

        protected virtual void Dispose(bool explicitDisposing)
        {
            if (this.safeWaitHandle != null)
            {
                this.safeWaitHandle.Close();
            }
        }

        internal void SetHandleInternal(Microsoft.Win32.SafeHandles.SafeWaitHandle handle)
        {
            this.safeWaitHandle = handle;
            this.waitHandle = handle.DangerousGetHandle();
        }

        public static bool SignalAndWait(WaitHandle toSignal, WaitHandle toWaitOn)
        {
            return SignalAndWait(toSignal, toWaitOn, -1, false);
        }

        public static bool SignalAndWait(WaitHandle toSignal, WaitHandle toWaitOn, int millisecondsTimeout, bool exitContext)
        {
            if ((Environment.OSInfo & Environment.OSName.Win9x) != Environment.OSName.Invalid)
            {
                throw new PlatformNotSupportedException(Environment.GetResourceString("PlatformNotSupported_Win9x"));
            }
            if (toSignal == null)
            {
                throw new ArgumentNullException("toSignal");
            }
            if (toWaitOn == null)
            {
                throw new ArgumentNullException("toWaitOn");
            }
            if (-1 > millisecondsTimeout)
            {
                throw new ArgumentOutOfRangeException("millisecondsTimeout", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegOrNegative1"));
            }
            int num = SignalAndWaitOne(toSignal.safeWaitHandle, toWaitOn.safeWaitHandle, millisecondsTimeout, toWaitOn.hasThreadAffinity, exitContext);
            if ((0x7fffffff != num) && toSignal.hasThreadAffinity)
            {
                Thread.EndCriticalRegion();
                Thread.EndThreadAffinity();
            }
            if (0x80 == num)
            {
                throw new AbandonedMutexException();
            }
            if (0x12a == num)
            {
                throw new InvalidOperationException(Environment.GetResourceString("Threading.WaitHandleTooManyPosts"));
            }
            return (num == 0);
        }

        public static bool SignalAndWait(WaitHandle toSignal, WaitHandle toWaitOn, TimeSpan timeout, bool exitContext)
        {
            long totalMilliseconds = (long) timeout.TotalMilliseconds;
            if ((-1L > totalMilliseconds) || (0x7fffffffL < totalMilliseconds))
            {
                throw new ArgumentOutOfRangeException("timeout", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegOrNegative1"));
            }
            return SignalAndWait(toSignal, toWaitOn, (int) totalMilliseconds, exitContext);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int SignalAndWaitOne(Microsoft.Win32.SafeHandles.SafeWaitHandle waitHandleToSignal, Microsoft.Win32.SafeHandles.SafeWaitHandle waitHandleToWaitOn, int millisecondsTimeout, bool hasThreadAffinity, bool exitContext);
        void IDisposable.Dispose()
        {
            this.Dispose(true);
            GC.nativeSuppressFinalize(this);
        }

        public static bool WaitAll(WaitHandle[] waitHandles)
        {
            return WaitAll(waitHandles, -1, true);
        }

        public static bool WaitAll(WaitHandle[] waitHandles, int millisecondsTimeout)
        {
            return WaitAll(waitHandles, millisecondsTimeout, true);
        }

        public static bool WaitAll(WaitHandle[] waitHandles, TimeSpan timeout)
        {
            return WaitAll(waitHandles, timeout, true);
        }

        public static bool WaitAll(WaitHandle[] waitHandles, int millisecondsTimeout, bool exitContext)
        {
            if ((waitHandles == null) || (waitHandles.Length == 0))
            {
                throw new ArgumentNullException("waitHandles");
            }
            if (waitHandles.Length > 0x40)
            {
                throw new NotSupportedException(Environment.GetResourceString("NotSupported_MaxWaitHandles"));
            }
            if (-1 > millisecondsTimeout)
            {
                throw new ArgumentOutOfRangeException("millisecondsTimeout", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegOrNegative1"));
            }
            WaitHandle[] handleArray = new WaitHandle[waitHandles.Length];
            for (int i = 0; i < waitHandles.Length; i++)
            {
                WaitHandle proxy = waitHandles[i];
                if (proxy == null)
                {
                    throw new ArgumentNullException(Environment.GetResourceString("ArgumentNull_ArrayElement"));
                }
                if (RemotingServices.IsTransparentProxy(proxy))
                {
                    throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_WaitOnTransparentProxy"));
                }
                handleArray[i] = proxy;
            }
            int num2 = WaitMultiple(handleArray, millisecondsTimeout, exitContext, true);
            if ((0x80 <= num2) && ((0x80 + handleArray.Length) > num2))
            {
                throw new AbandonedMutexException();
            }
            for (int j = 0; j < handleArray.Length; j++)
            {
                GC.KeepAlive(handleArray[j]);
            }
            return (num2 != 0x102);
        }

        public static bool WaitAll(WaitHandle[] waitHandles, TimeSpan timeout, bool exitContext)
        {
            long totalMilliseconds = (long) timeout.TotalMilliseconds;
            if ((-1L > totalMilliseconds) || (0x7fffffffL < totalMilliseconds))
            {
                throw new ArgumentOutOfRangeException("timeout", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegOrNegative1"));
            }
            return WaitAll(waitHandles, (int) totalMilliseconds, exitContext);
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static int WaitAny(WaitHandle[] waitHandles)
        {
            return WaitAny(waitHandles, -1, true);
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static int WaitAny(WaitHandle[] waitHandles, int millisecondsTimeout)
        {
            return WaitAny(waitHandles, millisecondsTimeout, true);
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static int WaitAny(WaitHandle[] waitHandles, TimeSpan timeout)
        {
            return WaitAny(waitHandles, timeout, true);
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static int WaitAny(WaitHandle[] waitHandles, int millisecondsTimeout, bool exitContext)
        {
            if (waitHandles == null)
            {
                throw new ArgumentNullException("waitHandles");
            }
            if (0x40 < waitHandles.Length)
            {
                throw new NotSupportedException(Environment.GetResourceString("NotSupported_MaxWaitHandles"));
            }
            if (-1 > millisecondsTimeout)
            {
                throw new ArgumentOutOfRangeException("millisecondsTimeout", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegOrNegative1"));
            }
            WaitHandle[] handleArray = new WaitHandle[waitHandles.Length];
            for (int i = 0; i < waitHandles.Length; i++)
            {
                WaitHandle proxy = waitHandles[i];
                if (proxy == null)
                {
                    throw new ArgumentNullException(Environment.GetResourceString("ArgumentNull_ArrayElement"));
                }
                if (RemotingServices.IsTransparentProxy(proxy))
                {
                    throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_WaitOnTransparentProxy"));
                }
                handleArray[i] = proxy;
            }
            int num2 = WaitMultiple(handleArray, millisecondsTimeout, exitContext, false);
            for (int j = 0; j < handleArray.Length; j++)
            {
                GC.KeepAlive(handleArray[j]);
            }
            if ((0x80 > num2) || ((0x80 + handleArray.Length) <= num2))
            {
                return num2;
            }
            int location = num2 - 0x80;
            if ((0 <= location) && (location < handleArray.Length))
            {
                throw new AbandonedMutexException(location, handleArray[location]);
            }
            throw new AbandonedMutexException();
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static int WaitAny(WaitHandle[] waitHandles, TimeSpan timeout, bool exitContext)
        {
            long totalMilliseconds = (long) timeout.TotalMilliseconds;
            if ((-1L > totalMilliseconds) || (0x7fffffffL < totalMilliseconds))
            {
                throw new ArgumentOutOfRangeException("timeout", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegOrNegative1"));
            }
            return WaitAny(waitHandles, (int) totalMilliseconds, exitContext);
        }

        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        private static extern int WaitMultiple(WaitHandle[] waitHandles, int millisecondsTimeout, bool exitContext, bool WaitAll);
        public virtual bool WaitOne()
        {
            return this.WaitOne(-1, false);
        }

        public virtual bool WaitOne(int millisecondsTimeout)
        {
            return this.WaitOne(millisecondsTimeout, false);
        }

        public virtual bool WaitOne(TimeSpan timeout)
        {
            return this.WaitOne(timeout, false);
        }

        public virtual bool WaitOne(int millisecondsTimeout, bool exitContext)
        {
            if (millisecondsTimeout < -1)
            {
                throw new ArgumentOutOfRangeException("millisecondsTimeout", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegOrNegative1"));
            }
            return this.WaitOne((long) millisecondsTimeout, exitContext);
        }

        private bool WaitOne(long timeout, bool exitContext)
        {
            if (this.safeWaitHandle == null)
            {
                throw new ObjectDisposedException(null, Environment.GetResourceString("ObjectDisposed_Generic"));
            }
            int num = WaitOneNative(this.safeWaitHandle, (uint) timeout, this.hasThreadAffinity, exitContext);
            if (num == 0x80)
            {
                throw new AbandonedMutexException();
            }
            return (num != 0x102);
        }

        public virtual bool WaitOne(TimeSpan timeout, bool exitContext)
        {
            long totalMilliseconds = (long) timeout.TotalMilliseconds;
            if ((-1L > totalMilliseconds) || (0x7fffffffL < totalMilliseconds))
            {
                throw new ArgumentOutOfRangeException("timeout", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegOrNegative1"));
            }
            return this.WaitOne(totalMilliseconds, exitContext);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int WaitOneNative(Microsoft.Win32.SafeHandles.SafeWaitHandle waitHandle, uint millisecondsTimeout, bool hasThreadAffinity, bool exitContext);

        [Obsolete("Use the SafeWaitHandle property instead.")]
        public virtual IntPtr Handle
        {
            get
            {
                if (this.safeWaitHandle != null)
                {
                    return this.safeWaitHandle.DangerousGetHandle();
                }
                return InvalidHandle;
            }
            [SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.UnmanagedCode), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            set
            {
                if (value == InvalidHandle)
                {
                    this.safeWaitHandle.SetHandleAsInvalid();
                    this.safeWaitHandle = null;
                }
                else
                {
                    this.safeWaitHandle = new Microsoft.Win32.SafeHandles.SafeWaitHandle(value, true);
                }
                this.waitHandle = value;
            }
        }

        public Microsoft.Win32.SafeHandles.SafeWaitHandle SafeWaitHandle
        {
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode), SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                if (this.safeWaitHandle == null)
                {
                    this.safeWaitHandle = new Microsoft.Win32.SafeHandles.SafeWaitHandle(InvalidHandle, false);
                }
                return this.safeWaitHandle;
            }
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode), SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            set
            {
                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                }
                finally
                {
                    if (value == null)
                    {
                        this.safeWaitHandle = null;
                        this.waitHandle = InvalidHandle;
                    }
                    else
                    {
                        this.safeWaitHandle = value;
                        this.waitHandle = this.safeWaitHandle.DangerousGetHandle();
                    }
                }
            }
        }
    }
}

