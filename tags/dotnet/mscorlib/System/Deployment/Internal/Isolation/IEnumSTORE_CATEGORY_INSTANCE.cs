namespace System.Deployment.Internal.Isolation
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("5ba7cb30-8508-4114-8c77-262fcda4fadb"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IEnumSTORE_CATEGORY_INSTANCE
    {
        uint Next([In] uint ulElements, [Out, MarshalAs(UnmanagedType.LPArray)] STORE_CATEGORY_INSTANCE[] rgInstances);
        void Skip([In] uint ulElements);
        void Reset();
        IEnumSTORE_CATEGORY_INSTANCE Clone();
    }
}

