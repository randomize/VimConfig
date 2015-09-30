namespace System.Deployment.Internal.Isolation.Manifest
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("3903B11B-FBE8-477c-825F-DB828B5FD174"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICOMServerEntry
    {
        COMServerEntry AllData { get; }
        Guid Clsid { get; }
        uint Flags { get; }
        Guid ConfiguredGuid { get; }
        Guid ImplementedClsid { get; }
        Guid TypeLibrary { get; }
        uint ThreadingModel { get; }
        string RuntimeVersion { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        string HostFile { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
    }
}

