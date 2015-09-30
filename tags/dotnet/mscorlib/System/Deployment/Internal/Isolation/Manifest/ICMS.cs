namespace System.Deployment.Internal.Isolation.Manifest
{
    using System.Deployment.Internal.Isolation;
    using System.Runtime.InteropServices;

    [ComImport, Guid("a504e5b0-8ccf-4cb4-9902-c9d1b9abd033"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICMS
    {
        IDefinitionIdentity Identity { get; }
        ISection FileSection { get; }
        ISection CategoryMembershipSection { get; }
        ISection COMRedirectionSection { get; }
        ISection ProgIdRedirectionSection { get; }
        ISection CLRSurrogateSection { get; }
        ISection AssemblyReferenceSection { get; }
        ISection WindowClassSection { get; }
        ISection StringSection { get; }
        ISection EntryPointSection { get; }
        ISection PermissionSetSection { get; }
        ISectionEntry MetadataSectionEntry { get; }
        ISection AssemblyRequestSection { get; }
        ISection RegistryKeySection { get; }
        ISection DirectorySection { get; }
        ISection FileAssociationSection { get; }
        ISection EventSection { get; }
        ISection EventMapSection { get; }
        ISection EventTagSection { get; }
        ISection CounterSetSection { get; }
        ISection CounterSection { get; }
    }
}

