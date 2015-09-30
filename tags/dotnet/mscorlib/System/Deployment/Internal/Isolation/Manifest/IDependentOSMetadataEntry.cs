namespace System.Deployment.Internal.Isolation.Manifest
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("CF168CF4-4E8F-4d92-9D2A-60E5CA21CF85")]
    internal interface IDependentOSMetadataEntry
    {
        DependentOSMetadataEntry AllData { get; }
        string SupportUrl { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        string Description { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        ushort MajorVersion { get; }
        ushort MinorVersion { get; }
        ushort BuildNumber { get; }
        byte ServicePackMajor { get; }
        byte ServicePackMinor { get; }
    }
}

