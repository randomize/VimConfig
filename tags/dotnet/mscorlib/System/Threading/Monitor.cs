namespace System.Threading
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    [ComVisible(true), HostProtection(SecurityAction.LinkDemand, Synchronization=true, ExternalThreading=true)]
    public static class Monitor
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void Enter(object obj);
        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static extern void Exit(object obj);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void ObjPulse(object obj);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void ObjPulseAll(object obj);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool ObjWait(bool exitContext, int millisecondsTimeout, object obj);
        public static void Pulse(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            ObjPulse(obj);
        }

        public static void PulseAll(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            ObjPulseAll(obj);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void ReliableEnter(object obj, ref bool tookLock);
        public static bool TryEnter(object obj)
        {
            return TryEnterTimeout(obj, 0);
        }

        public static bool TryEnter(object obj, int millisecondsTimeout)
        {
            return TryEnterTimeout(obj, millisecondsTimeout);
        }

        public static bool TryEnter(object obj, TimeSpan timeout)
        {
            long totalMilliseconds = (long) timeout.TotalMilliseconds;
            if ((totalMilliseconds < -1L) || (totalMilliseconds > 0x7fffffffL))
            {
                throw new ArgumentOutOfRangeException("timeout", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegOrNegative1"));
            }
            return TryEnterTimeout(obj, (int) totalMilliseconds);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool TryEnterTimeout(object obj, int timeout);
        public static bool Wait(object obj)
        {
            return Wait(obj, -1, false);
        }

        public static bool Wait(object obj, int millisecondsTimeout)
        {
            return Wait(obj, millisecondsTimeout, false);
        }

        public static bool Wait(object obj, TimeSpan timeout)
        {
            long totalMilliseconds = (long) timeout.TotalMilliseconds;
            if ((totalMilliseconds < -1L) || (totalMilliseconds > 0x7fffffffL))
            {
                throw new ArgumentOutOfRangeException("timeout", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegOrNegative1"));
            }
            return Wait(obj, (int) totalMilliseconds, false);
        }

        public static bool Wait(object obj, int millisecondsTimeout, bool exitContext)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            return ObjWait(exitContext, millisecondsTimeout, obj);
        }

        public static bool Wait(object obj, TimeSpan timeout, bool exitContext)
        {
            long totalMilliseconds = (long) timeout.TotalMilliseconds;
            if ((totalMilliseconds < -1L) || (totalMilliseconds > 0x7fffffffL))
            {
                throw new ArgumentOutOfRangeException("timeout", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegOrNegative1"));
            }
            return Wait(obj, (int) totalMilliseconds, exitContext);
        }
    }
}

