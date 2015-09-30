namespace System.Threading
{
    using System;
    using System.Runtime.InteropServices;

    internal class SafeCompressedStackHandle : SafeHandle
    {
        public SafeCompressedStackHandle() : base(IntPtr.Zero, true)
        {
        }

        protected override bool ReleaseHandle()
        {
            CompressedStack.DestroyDelayedCompressedStack(base.handle);
            base.handle = IntPtr.Zero;
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

