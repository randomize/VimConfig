namespace System.Runtime.InteropServices
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Security.Permissions;

    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode=true), SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
    public abstract class SafeHandle : CriticalFinalizerObject, IDisposable
    {
        private bool _fullyInitialized;
        private bool _ownsHandle;
        private int _state;
        protected IntPtr handle;

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected SafeHandle(IntPtr invalidHandleValue, bool ownsHandle)
        {
            this.handle = invalidHandleValue;
            this._state = 4;
            this._ownsHandle = ownsHandle;
            if (!ownsHandle)
            {
                GC.SuppressFinalize(this);
            }
            this._fullyInitialized = true;
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public void Close()
        {
            this.Dispose(true);
        }

        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public extern void DangerousAddRef(ref bool success);
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public IntPtr DangerousGetHandle()
        {
            return this.handle;
        }

        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public extern void DangerousRelease();
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public void Dispose()
        {
            this.Dispose(true);
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.InternalDispose();
            }
            else
            {
                this.InternalFinalize();
            }
        }

        ~SafeHandle()
        {
            this.Dispose(false);
        }

        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        private extern void InternalDispose();
        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        private extern void InternalFinalize();
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected abstract bool ReleaseHandle();
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected void SetHandle(IntPtr handle)
        {
            this.handle = handle;
        }

        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public extern void SetHandleAsInvalid();

        public bool IsClosed
        {
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            get
            {
                return ((this._state & 1) == 1);
            }
        }

        public abstract bool IsInvalid { [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)] get; }
    }
}

