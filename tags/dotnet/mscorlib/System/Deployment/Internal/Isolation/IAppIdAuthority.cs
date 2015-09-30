namespace System.Deployment.Internal.Isolation
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("8c87810c-2541-4f75-b2d0-9af515488e23"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAppIdAuthority
    {
        IDefinitionAppId TextToDefinition([In] uint Flags, [In, MarshalAs(UnmanagedType.LPWStr)] string Identity);
        IReferenceAppId TextToReference([In] uint Flags, [In, MarshalAs(UnmanagedType.LPWStr)] string Identity);
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string DefinitionToText([In] uint Flags, [In] IDefinitionAppId DefinitionAppId);
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string ReferenceToText([In] uint Flags, [In] IReferenceAppId ReferenceAppId);
        [return: MarshalAs(UnmanagedType.Bool)]
        bool AreDefinitionsEqual([In] uint Flags, [In] IDefinitionAppId Definition1, [In] IDefinitionAppId Definition2);
        [return: MarshalAs(UnmanagedType.Bool)]
        bool AreReferencesEqual([In] uint Flags, [In] IReferenceAppId Reference1, [In] IReferenceAppId Reference2);
        [return: MarshalAs(UnmanagedType.Bool)]
        bool AreTextualDefinitionsEqual([In] uint Flags, [In, MarshalAs(UnmanagedType.LPWStr)] string AppIdLeft, [In, MarshalAs(UnmanagedType.LPWStr)] string AppIdRight);
        [return: MarshalAs(UnmanagedType.Bool)]
        bool AreTextualReferencesEqual([In] uint Flags, [In, MarshalAs(UnmanagedType.LPWStr)] string AppIdLeft, [In, MarshalAs(UnmanagedType.LPWStr)] string AppIdRight);
        [return: MarshalAs(UnmanagedType.Bool)]
        bool DoesDefinitionMatchReference([In] uint Flags, [In] IDefinitionAppId DefinitionIdentity, [In] IReferenceAppId ReferenceIdentity);
        [return: MarshalAs(UnmanagedType.Bool)]
        bool DoesTextualDefinitionMatchTextualReference([In] uint Flags, [In, MarshalAs(UnmanagedType.LPWStr)] string Definition, [In, MarshalAs(UnmanagedType.LPWStr)] string Reference);
        ulong HashReference([In] uint Flags, [In] IReferenceAppId ReferenceIdentity);
        ulong HashDefinition([In] uint Flags, [In] IDefinitionAppId DefinitionIdentity);
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string GenerateDefinitionKey([In] uint Flags, [In] IDefinitionAppId DefinitionIdentity);
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string GenerateReferenceKey([In] uint Flags, [In] IReferenceAppId ReferenceIdentity);
        IDefinitionAppId CreateDefinition();
        IReferenceAppId CreateReference();
    }
}

