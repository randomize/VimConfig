namespace System.Deployment.Internal.Isolation
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("d91e12d8-98ed-47fa-9936-39421283d59b"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IDefinitionAppId
    {
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string get_SubscriptionId();
        void put_SubscriptionId([In, MarshalAs(UnmanagedType.LPWStr)] string Subscription);
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string get_Codebase();
        void put_Codebase([In, MarshalAs(UnmanagedType.LPWStr)] string CodeBase);
        IEnumDefinitionIdentity EnumAppPath();
        void SetAppPath([In] uint cIDefinitionIdentity, [In, MarshalAs(UnmanagedType.LPArray)] IDefinitionIdentity[] DefinitionIdentity);
    }
}

