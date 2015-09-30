namespace System.Deployment.Internal.Isolation
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("07662534-750b-4ed5-9cfb-1c5bc5acfd07")]
    internal interface IStateManager
    {
        void PrepareApplicationState([In] UIntPtr Inputs, ref UIntPtr Outputs);
        void SetApplicationRunningState([In] uint Flags, [In] IActContext Context, [In] uint RunningState, out uint Disposition);
        void GetApplicationStateFilesystemLocation([In] uint Flags, [In] IDefinitionAppId Appidentity, [In] IDefinitionIdentity ComponentIdentity, [In] UIntPtr Coordinates, [MarshalAs(UnmanagedType.LPWStr)] out string Path);
        void Scavenge([In] uint Flags, out uint Disposition);
    }
}

