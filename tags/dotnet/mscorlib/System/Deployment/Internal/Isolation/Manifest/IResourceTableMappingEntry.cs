namespace System.Deployment.Internal.Isolation.Manifest
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("70A4ECEE-B195-4c59-85BF-44B6ACA83F07"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IResourceTableMappingEntry
    {
        ResourceTableMappingEntry AllData { get; }
        string id { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        string FinalStringMapped { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
    }
}

