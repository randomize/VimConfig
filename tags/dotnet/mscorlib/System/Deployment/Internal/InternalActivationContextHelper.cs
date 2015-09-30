namespace System.Deployment.Internal
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(false)]
    public static class InternalActivationContextHelper
    {
        public static object GetActivationContextData(ActivationContext appInfo)
        {
            return appInfo.ActivationContextData;
        }

        public static object GetApplicationComponentManifest(ActivationContext appInfo)
        {
            return appInfo.ApplicationComponentManifest;
        }

        public static byte[] GetApplicationManifestBytes(ActivationContext appInfo)
        {
            if (appInfo == null)
            {
                throw new ArgumentNullException("appInfo");
            }
            return appInfo.GetApplicationManifestBytes();
        }

        public static object GetDeploymentComponentManifest(ActivationContext appInfo)
        {
            return appInfo.DeploymentComponentManifest;
        }

        public static byte[] GetDeploymentManifestBytes(ActivationContext appInfo)
        {
            if (appInfo == null)
            {
                throw new ArgumentNullException("appInfo");
            }
            return appInfo.GetDeploymentManifestBytes();
        }

        public static bool IsFirstRun(ActivationContext appInfo)
        {
            return (appInfo.LastApplicationStateResult == ActivationContext.ApplicationStateDisposition.RunningFirstTime);
        }

        public static void PrepareForExecution(ActivationContext appInfo)
        {
            appInfo.PrepareForExecution();
        }
    }
}

