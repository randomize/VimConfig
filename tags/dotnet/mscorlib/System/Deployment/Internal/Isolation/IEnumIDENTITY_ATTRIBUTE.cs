namespace System.Deployment.Internal.Isolation
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("9cdaae75-246e-4b00-a26d-b9aec137a3eb"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IEnumIDENTITY_ATTRIBUTE
    {
        uint Next([In] uint celt, [Out, MarshalAs(UnmanagedType.LPArray)] IDENTITY_ATTRIBUTE[] rgAttributes);
        IntPtr CurrentIntoBuffer([In] IntPtr Available, [Out, MarshalAs(UnmanagedType.LPArray)] byte[] Data);
        void Skip([In] uint celt);
        void Reset();
        IEnumIDENTITY_ATTRIBUTE Clone();
    }
}

