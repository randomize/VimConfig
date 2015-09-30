namespace System.Deployment.Internal.Isolation.Manifest
{
    using System;
    using System.Deployment.Internal.Isolation;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("5A7A54D7-5AD5-418e-AB7A-CF823A8D48D0")]
    internal interface ISubcategoryMembershipEntry
    {
        SubcategoryMembershipEntry AllData { get; }
        string Subcategory { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        ISection CategoryMembershipData { get; }
    }
}

