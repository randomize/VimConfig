namespace System.Deployment.Internal.Isolation.Manifest
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("CB73147E-5FC2-4c31-B4E6-58D13DBE1A08")]
    internal interface IDescriptionMetadataEntry
    {
        DescriptionMetadataEntry AllData { get; }
        string Publisher { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        string Product { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        string SupportUrl { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        string IconFile { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        string ErrorReportUrl { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        string SuiteName { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
    }
}

