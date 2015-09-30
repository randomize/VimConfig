namespace System.Deployment.Internal.Isolation
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("b30352cf-23da-4577-9b3f-b4e6573be53b"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IEnumReferenceIdentity
    {
        uint Next([In] uint celt, [Out, MarshalAs(UnmanagedType.LPArray)] IReferenceIdentity[] ReferenceIdentity);
        void Skip(uint celt);
        void Reset();
        IEnumReferenceIdentity Clone();
    }
}

