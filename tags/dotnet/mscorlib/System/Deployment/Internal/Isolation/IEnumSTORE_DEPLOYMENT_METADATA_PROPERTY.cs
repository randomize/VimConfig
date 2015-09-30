namespace System.Deployment.Internal.Isolation
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("5fa4f590-a416-4b22-ac79-7c3f0d31f303"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IEnumSTORE_DEPLOYMENT_METADATA_PROPERTY
    {
        uint Next([In] uint celt, [Out, MarshalAs(UnmanagedType.LPArray)] StoreOperationMetadataProperty[] AppIds);
        void Skip([In] uint celt);
        void Reset();
        IEnumSTORE_DEPLOYMENT_METADATA_PROPERTY Clone();
    }
}

