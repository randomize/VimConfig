namespace System.Deployment.Internal
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(false)]
    public static class InternalApplicationIdentityHelper
    {
        public static object GetInternalAppId(ApplicationIdentity id)
        {
            return id.Identity;
        }
    }
}

