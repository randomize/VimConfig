namespace System.Deployment.Internal.Isolation.Manifest
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("55b2dec1-d0f6-4bf4-91b1-30f73ad8e4df")]
    internal interface IMuiResourceTypeIdIntEntry
    {
        MuiResourceTypeIdIntEntry AllData { get; }
        object StringIds { [return: MarshalAs(UnmanagedType.Interface)] get; }
        object IntegerIds { [return: MarshalAs(UnmanagedType.Interface)] get; }
    }
}

