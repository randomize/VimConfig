namespace System.Deployment.Internal.Isolation
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("6eaf5ace-7917-4f3c-b129-e046a9704766"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IReferenceIdentity
    {
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string GetAttribute([In, MarshalAs(UnmanagedType.LPWStr)] string Namespace, [In, MarshalAs(UnmanagedType.LPWStr)] string Name);
        void SetAttribute([In, MarshalAs(UnmanagedType.LPWStr)] string Namespace, [In, MarshalAs(UnmanagedType.LPWStr)] string Name, [In, MarshalAs(UnmanagedType.LPWStr)] string Value);
        IEnumIDENTITY_ATTRIBUTE EnumAttributes();
        IReferenceIdentity Clone([In] IntPtr cDeltas, [In, MarshalAs(UnmanagedType.LPArray)] IDENTITY_ATTRIBUTE[] Deltas);
    }
}

