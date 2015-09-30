namespace System.Deployment.Internal.Isolation.Manifest
{
    using System;
    using System.Deployment.Internal.Isolation;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("AB1ED79F-943E-407d-A80B-0744E3A95B28")]
    internal interface IMetadataSectionEntry
    {
        MetadataSectionEntry AllData { get; }
        uint SchemaVersion { get; }
        uint ManifestFlags { get; }
        uint UsagePatterns { get; }
        IDefinitionIdentity CdfIdentity { get; }
        string LocalPath { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        uint HashAlgorithm { get; }
        object ManifestHash { [return: MarshalAs(UnmanagedType.Interface)] get; }
        string ContentType { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        string RuntimeImageVersion { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        object MvidValue { [return: MarshalAs(UnmanagedType.Interface)] get; }
        IDescriptionMetadataEntry DescriptionData { get; }
        IDeploymentMetadataEntry DeploymentData { get; }
        IDependentOSMetadataEntry DependentOSData { get; }
        string defaultPermissionSetID { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        string RequestedExecutionLevel { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        bool RequestedExecutionLevelUIAccess { get; }
        IReferenceIdentity ResourceTypeResourcesDependency { get; }
        IReferenceIdentity ResourceTypeManifestResourcesDependency { get; }
        string KeyInfoElement { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
    }
}

