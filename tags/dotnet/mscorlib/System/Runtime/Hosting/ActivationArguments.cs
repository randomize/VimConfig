namespace System.Runtime.Hosting
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, ComVisible(true)]
    public sealed class ActivationArguments
    {
        private bool m_activateInstance;
        private string[] m_activationData;
        private string m_appFullName;
        private string[] m_appManifestPaths;
        private bool m_useFusionActivationContext;

        private ActivationArguments()
        {
        }

        public ActivationArguments(System.ActivationContext activationData) : this(activationData, null)
        {
        }

        public ActivationArguments(System.ApplicationIdentity applicationIdentity) : this(applicationIdentity, null)
        {
        }

        public ActivationArguments(System.ActivationContext activationContext, string[] activationData)
        {
            if (activationContext == null)
            {
                throw new ArgumentNullException("activationContext");
            }
            this.m_appFullName = activationContext.Identity.FullName;
            this.m_appManifestPaths = activationContext.ManifestPaths;
            this.m_activationData = activationData;
            this.m_useFusionActivationContext = true;
        }

        public ActivationArguments(System.ApplicationIdentity applicationIdentity, string[] activationData)
        {
            if (applicationIdentity == null)
            {
                throw new ArgumentNullException("applicationIdentity");
            }
            this.m_appFullName = applicationIdentity.FullName;
            this.m_activationData = activationData;
        }

        internal ActivationArguments(string appFullName, string[] appManifestPaths, string[] activationData)
        {
            if (appFullName == null)
            {
                throw new ArgumentNullException("appFullName");
            }
            this.m_appFullName = appFullName;
            this.m_appManifestPaths = appManifestPaths;
            this.m_activationData = activationData;
            this.m_useFusionActivationContext = true;
        }

        internal bool ActivateInstance
        {
            get
            {
                return this.m_activateInstance;
            }
            set
            {
                this.m_activateInstance = value;
            }
        }

        public System.ActivationContext ActivationContext
        {
            get
            {
                if (!this.UseFusionActivationContext)
                {
                    return null;
                }
                if (this.m_appManifestPaths == null)
                {
                    return new System.ActivationContext(new System.ApplicationIdentity(this.m_appFullName));
                }
                return new System.ActivationContext(new System.ApplicationIdentity(this.m_appFullName), this.m_appManifestPaths);
            }
        }

        public string[] ActivationData
        {
            get
            {
                return this.m_activationData;
            }
        }

        internal string ApplicationFullName
        {
            get
            {
                return this.m_appFullName;
            }
        }

        public System.ApplicationIdentity ApplicationIdentity
        {
            get
            {
                return new System.ApplicationIdentity(this.m_appFullName);
            }
        }

        internal string[] ApplicationManifestPaths
        {
            get
            {
                return this.m_appManifestPaths;
            }
        }

        internal bool UseFusionActivationContext
        {
            get
            {
                return this.m_useFusionActivationContext;
            }
        }
    }
}

