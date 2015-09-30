namespace System.Deployment.Internal.Isolation.Manifest
{
    using System;
    using System.Deployment.Internal.Isolation;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("FD47B733-AFBC-45e4-B7C2-BBEB1D9F766C")]
    internal interface IAssemblyReferenceEntry
    {
        AssemblyReferenceEntry AllData { get; }
        IReferenceIdentity ReferenceIdentity { get; }
        uint Flags { get; }
        IAssemblyReferenceDependentAssemblyEntry DependentAssembly { get; }
    }
}

