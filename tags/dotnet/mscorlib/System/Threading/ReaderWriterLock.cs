namespace System.Threading
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    [ComVisible(true), HostProtection(SecurityAction.LinkDemand, Synchronization=true, ExternalThreading=true)]
    public sealed class ReaderWriterLock : CriticalFinalizerObject
    {
        private int _dwLLockID;
        private int _dwState;
        private int _dwULockID;
        private int _dwWriterID;
        private int _dwWriterSeqNum;
        private IntPtr _hObjectHandle;
        private IntPtr _hReaderEvent;
        private IntPtr _hWriterEvent;
        private short _wWriterLevel;

        public ReaderWriterLock()
        {
            this.PrivateInitialize();
        }

        public void AcquireReaderLock(int millisecondsTimeout)
        {
            this.AcquireReaderLockInternal(millisecondsTimeout);
        }

        public void AcquireReaderLock(TimeSpan timeout)
        {
            long totalMilliseconds = (long) timeout.TotalMilliseconds;
            if ((totalMilliseconds < -1L) || (totalMilliseconds > 0x7fffffffL))
            {
                throw new ArgumentOutOfRangeException("timeout", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegOrNegative1"));
            }
            this.AcquireReaderLockInternal((int) totalMilliseconds);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void AcquireReaderLockInternal(int millisecondsTimeout);
        public void AcquireWriterLock(int millisecondsTimeout)
        {
            this.AcquireWriterLockInternal(millisecondsTimeout);
        }

        public void AcquireWriterLock(TimeSpan timeout)
        {
            long totalMilliseconds = (long) timeout.TotalMilliseconds;
            if ((totalMilliseconds < -1L) || (totalMilliseconds > 0x7fffffffL))
            {
                throw new ArgumentOutOfRangeException("timeout", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegOrNegative1"));
            }
            this.AcquireWriterLockInternal((int) totalMilliseconds);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void AcquireWriterLockInternal(int millisecondsTimeout);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool AnyWritersSince(int seqNum);
        public void DowngradeFromWriterLock(ref LockCookie lockCookie)
        {
            this.DowngradeFromWriterLockInternal(ref lockCookie);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void DowngradeFromWriterLockInternal(ref LockCookie lockCookie);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void FCallReleaseLock(ref LockCookie result);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void FCallUpgradeToWriterLock(ref LockCookie result, int millisecondsTimeout);
        ~ReaderWriterLock()
        {
            this.PrivateDestruct();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void PrivateDestruct();
        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        private extern bool PrivateGetIsReaderLockHeld();
        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        private extern bool PrivateGetIsWriterLockHeld();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int PrivateGetWriterSeqNum();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void PrivateInitialize();
        public LockCookie ReleaseLock()
        {
            LockCookie result = new LockCookie();
            this.FCallReleaseLock(ref result);
            return result;
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public void ReleaseReaderLock()
        {
            this.ReleaseReaderLockInternal();
        }

        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        private extern void ReleaseReaderLockInternal();
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public void ReleaseWriterLock()
        {
            this.ReleaseWriterLockInternal();
        }

        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        private extern void ReleaseWriterLockInternal();
        public void RestoreLock(ref LockCookie lockCookie)
        {
            this.RestoreLockInternal(ref lockCookie);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void RestoreLockInternal(ref LockCookie lockCookie);
        public LockCookie UpgradeToWriterLock(int millisecondsTimeout)
        {
            LockCookie result = new LockCookie();
            this.FCallUpgradeToWriterLock(ref result, millisecondsTimeout);
            return result;
        }

        public LockCookie UpgradeToWriterLock(TimeSpan timeout)
        {
            long totalMilliseconds = (long) timeout.TotalMilliseconds;
            if ((totalMilliseconds < -1L) || (totalMilliseconds > 0x7fffffffL))
            {
                throw new ArgumentOutOfRangeException("timeout", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegOrNegative1"));
            }
            return this.UpgradeToWriterLock((int) totalMilliseconds);
        }

        public bool IsReaderLockHeld
        {
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            get
            {
                return this.PrivateGetIsReaderLockHeld();
            }
        }

        public bool IsWriterLockHeld
        {
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            get
            {
                return this.PrivateGetIsWriterLockHeld();
            }
        }

        public int WriterSeqNum
        {
            get
            {
                return this.PrivateGetWriterSeqNum();
            }
        }
    }
}

