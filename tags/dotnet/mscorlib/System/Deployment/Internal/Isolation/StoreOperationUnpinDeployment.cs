namespace System.Deployment.Internal.Isolation
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct StoreOperationUnpinDeployment
    {
        [MarshalAs(UnmanagedType.U4)]
        public uint Size;
        [MarshalAs(UnmanagedType.U4)]
        public OpFlags Flags;
        [MarshalAs(UnmanagedType.Interface)]
        public IDefinitionAppId Application;
        public IntPtr Reference;
        public StoreOperationUnpinDeployment(IDefinitionAppId app, StoreApplicationReference reference)
        {
            this.Size = (uint) Marshal.SizeOf(typeof(StoreOperationUnpinDeployment));
            this.Flags = OpFlags.Nothing;
            this.Application = app;
            this.Reference = reference.ToIntPtr();
        }

        public void Destroy()
        {
            StoreApplicationReference.Destroy(this.Reference);
        }
        public enum Disposition
        {
            Failed,
            Unpinned
        }

        [Flags]
        public enum OpFlags
        {
            Nothing
        }
    }
}

