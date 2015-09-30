namespace System.Deployment.Internal.Isolation.Manifest
{
    using System;
    using System.Deployment.Internal.Isolation;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("A2A55FAD-349B-469b-BF12-ADC33D14A937")]
    internal interface IFileEntry
    {
        FileEntry AllData { get; }
        string Name { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        uint HashAlgorithm { get; }
        string LoadFrom { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        string SourcePath { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        string ImportPath { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        string SourceName { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        string Location { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        object HashValue { [return: MarshalAs(UnmanagedType.Interface)] get; }
        ulong Size { get; }
        string Group { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        uint Flags { get; }
        IMuiResourceMapEntry MuiMapping { get; }
        uint WritableType { get; }
        ISection HashElements { get; }
    }
}

