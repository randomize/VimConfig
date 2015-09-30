namespace System.Deployment.Internal.Isolation.Manifest
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("54F198EC-A63A-45ea-A984-452F68D9B35B")]
    internal interface IProgIdRedirectionEntry
    {
        ProgIdRedirectionEntry AllData { get; }
        string ProgId { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        Guid RedirectedGuid { get; }
    }
}

