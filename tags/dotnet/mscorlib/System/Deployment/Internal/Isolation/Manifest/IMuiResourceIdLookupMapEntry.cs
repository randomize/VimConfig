namespace System.Deployment.Internal.Isolation.Manifest
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("24abe1f7-a396-4a03-9adf-1d5b86a5569f"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMuiResourceIdLookupMapEntry
    {
        MuiResourceIdLookupMapEntry AllData { get; }
        uint Count { get; }
    }
}

