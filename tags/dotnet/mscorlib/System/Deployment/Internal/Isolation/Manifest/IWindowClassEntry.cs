namespace System.Deployment.Internal.Isolation.Manifest
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("8AD3FC86-AFD3-477a-8FD5-146C291195BA"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IWindowClassEntry
    {
        WindowClassEntry AllData { get; }
        string ClassName { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        string HostDll { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        bool fVersioned { get; }
    }
}

