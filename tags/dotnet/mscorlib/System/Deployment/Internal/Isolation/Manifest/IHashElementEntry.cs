namespace System.Deployment.Internal.Isolation.Manifest
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("9D46FB70-7B54-4f4f-9331-BA9E87833FF5")]
    internal interface IHashElementEntry
    {
        HashElementEntry AllData { get; }
        uint index { get; }
        byte Transform { get; }
        object TransformMetadata { [return: MarshalAs(UnmanagedType.Interface)] get; }
        byte DigestMethod { get; }
        object DigestValue { [return: MarshalAs(UnmanagedType.Interface)] get; }
        string Xml { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
    }
}

