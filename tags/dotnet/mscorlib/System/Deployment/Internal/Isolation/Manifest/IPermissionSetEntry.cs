namespace System.Deployment.Internal.Isolation.Manifest
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("EBE5A1ED-FEBC-42c4-A9E1-E087C6E36635")]
    internal interface IPermissionSetEntry
    {
        PermissionSetEntry AllData { get; }
        string Id { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        string XmlSegment { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
    }
}

