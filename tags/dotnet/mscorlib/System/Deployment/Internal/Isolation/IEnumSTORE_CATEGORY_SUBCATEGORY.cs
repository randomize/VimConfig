namespace System.Deployment.Internal.Isolation
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("19be1967-b2fc-4dc1-9627-f3cb6305d2a7"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IEnumSTORE_CATEGORY_SUBCATEGORY
    {
        uint Next([In] uint celt, [Out, MarshalAs(UnmanagedType.LPArray)] STORE_CATEGORY_SUBCATEGORY[] rgElements);
        void Skip([In] uint ulElements);
        void Reset();
        IEnumSTORE_CATEGORY_SUBCATEGORY Clone();
    }
}

