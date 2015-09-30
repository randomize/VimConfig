namespace System.Deployment.Internal.Isolation
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("b840a2f5-a497-4a6d-9038-cd3ec2fbd222"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IEnumSTORE_CATEGORY
    {
        uint Next([In] uint celt, [Out, MarshalAs(UnmanagedType.LPArray)] STORE_CATEGORY[] rgElements);
        void Skip([In] uint ulElements);
        void Reset();
        IEnumSTORE_CATEGORY Clone();
    }
}

