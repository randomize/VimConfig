namespace System.Deployment.Internal.Isolation
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("81c85208-fe61-4c15-b5bb-ff5ea66baad9")]
    internal interface IManifestInformation
    {
        void get_FullPath([MarshalAs(UnmanagedType.LPWStr)] out string FullPath);
    }
}

