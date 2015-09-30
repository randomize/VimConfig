namespace System.Threading
{
    using System;
    using System.Runtime.InteropServices;

    internal class IUnknownSafeHandle : SafeHandle
    {
        public IUnknownSafeHandle() : base(IntPtr.Zero, true)
        {
        }

        internal object Clone()
        {
            IUnknownSafeHandle clonedContext = new IUnknownSafeHandle();
            if (!this.IsInvalid)
            {
                HostExecutionContextManager.CloneHostSecurityContext(this, clonedContext);
            }
            return clonedContext;
        }

        protected override bool ReleaseHandle()
        {
            HostExecutionContextManager.ReleaseHostSecurityContext(base.handle);
            return true;
        }

        public override bool IsInvalid
        {
            get
            {
                return (base.handle == IntPtr.Zero);
            }
        }
    }
}

