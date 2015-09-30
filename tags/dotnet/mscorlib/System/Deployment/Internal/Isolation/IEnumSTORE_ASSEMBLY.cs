namespace System.Deployment.Internal.Isolation
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("a5c637bf-6eaa-4e5f-b535-55299657e33e")]
    internal interface IEnumSTORE_ASSEMBLY
    {
        uint Next([In] uint celt, [Out, MarshalAs(UnmanagedType.LPArray)] STORE_ASSEMBLY[] rgelt);
        void Skip([In] uint celt);
        void Reset();
        IEnumSTORE_ASSEMBLY Clone();
    }
}

