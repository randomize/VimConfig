namespace System.Deployment.Internal.Isolation.Manifest
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal class MuiResourceMapEntry : IDisposable
    {
        [MarshalAs(UnmanagedType.SysInt)]
        public IntPtr ResourceTypeIdInt;
        public uint ResourceTypeIdIntSize;
        [MarshalAs(UnmanagedType.SysInt)]
        public IntPtr ResourceTypeIdString;
        public uint ResourceTypeIdStringSize;
        ~MuiResourceMapEntry()
        {
            this.Dispose(false);
        }

        void IDisposable.Dispose()
        {
            this.Dispose(true);
        }

        public void Dispose(bool fDisposing)
        {
            if (this.ResourceTypeIdInt != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(this.ResourceTypeIdInt);
                this.ResourceTypeIdInt = IntPtr.Zero;
            }
            if (this.ResourceTypeIdString != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(this.ResourceTypeIdString);
                this.ResourceTypeIdString = IntPtr.Zero;
            }
            if (fDisposing)
            {
                GC.SuppressFinalize(this);
            }
        }
    }
}

