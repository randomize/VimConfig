namespace System.Deployment.Internal.Isolation.Manifest
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("1E0422A1-F0D2-44ae-914B-8A2DECCFD22B"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICLRSurrogateEntry
    {
        CLRSurrogateEntry AllData { get; }
        Guid Clsid { get; }
        string RuntimeVersion { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        string ClassName { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
    }
}

