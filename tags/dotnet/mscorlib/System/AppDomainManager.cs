namespace System
{
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.Hosting;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;
    using System.Security.Policy;
    using System.Threading;

    [ComVisible(true), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure), SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.Infrastructure)]
    public class AppDomainManager : MarshalByRefObject
    {
        private System.Runtime.Hosting.ApplicationActivator m_appActivator;
        private Assembly m_entryAssembly;
        private AppDomainManagerInitializationOptions m_flags;

        public virtual bool CheckSecuritySettings(SecurityState state)
        {
            return false;
        }

        public virtual AppDomain CreateDomain(string friendlyName, Evidence securityInfo, AppDomainSetup appDomainInfo)
        {
            return CreateDomainHelper(friendlyName, securityInfo, appDomainInfo);
        }

        [SecurityPermission(SecurityAction.Demand, ControlAppDomain=true), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        protected static AppDomain CreateDomainHelper(string friendlyName, Evidence securityInfo, AppDomainSetup appDomainInfo)
        {
            if (friendlyName == null)
            {
                throw new ArgumentNullException(Environment.GetResourceString("ArgumentNull_String"));
            }
            if (securityInfo != null)
            {
                new SecurityPermission(SecurityPermissionFlag.ControlEvidence).Demand();
            }
            return AppDomain.nCreateDomain(friendlyName, appDomainInfo, securityInfo, (securityInfo == null) ? AppDomain.CurrentDomain.InternalEvidence : null, AppDomain.CurrentDomain.GetSecurityDescriptor());
        }

        public virtual void InitializeNewDomain(AppDomainSetup appDomainInfo)
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Assembly nGetEntryAssembly();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void nRegisterWithHost();

        public virtual System.Runtime.Hosting.ApplicationActivator ApplicationActivator
        {
            get
            {
                if (this.m_appActivator == null)
                {
                    this.m_appActivator = new System.Runtime.Hosting.ApplicationActivator();
                }
                return this.m_appActivator;
            }
        }

        internal static AppDomainManager CurrentAppDomainManager
        {
            get
            {
                return AppDomain.CurrentDomain.DomainManager;
            }
        }

        public virtual Assembly EntryAssembly
        {
            get
            {
                if (this.m_entryAssembly == null)
                {
                    AppDomain currentDomain = AppDomain.CurrentDomain;
                    if (currentDomain.IsDefaultAppDomain() && (currentDomain.ActivationContext != null))
                    {
                        ManifestRunner runner = new ManifestRunner(currentDomain, currentDomain.ActivationContext);
                        this.m_entryAssembly = runner.EntryAssembly;
                    }
                    else
                    {
                        this.m_entryAssembly = nGetEntryAssembly();
                    }
                }
                return this.m_entryAssembly;
            }
        }

        public virtual System.Threading.HostExecutionContextManager HostExecutionContextManager
        {
            get
            {
                return System.Threading.HostExecutionContextManager.GetInternalHostExecutionContextManager();
            }
        }

        public virtual System.Security.HostSecurityManager HostSecurityManager
        {
            get
            {
                return null;
            }
        }

        public AppDomainManagerInitializationOptions InitializationFlags
        {
            get
            {
                return this.m_flags;
            }
            set
            {
                this.m_flags = value;
            }
        }
    }
}

