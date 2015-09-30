namespace System.Deployment.Internal.Isolation.Manifest
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("397927f5-10f2-4ecb-bfe1-3c264212a193"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMuiResourceMapEntry
    {
        MuiResourceMapEntry AllData { get; }
        object ResourceTypeIdInt { [return: MarshalAs(UnmanagedType.Interface)] get; }
        object ResourceTypeIdString { [return: MarshalAs(UnmanagedType.Interface)] get; }
    }
}

