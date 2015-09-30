namespace System.Deployment.Internal.Isolation
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("f3549d9c-fc73-4793-9c00-1cd204254c0c"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IEnumDefinitionIdentity
    {
        uint Next([In] uint celt, [Out, MarshalAs(UnmanagedType.LPArray)] IDefinitionIdentity[] DefinitionIdentity);
        void Skip([In] uint celt);
        void Reset();
        IEnumDefinitionIdentity Clone();
    }
}

