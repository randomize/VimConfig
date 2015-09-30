namespace System
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration.Assemblies;
    using System.Deployment.Internal.Isolation.Manifest;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.Hosting;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Channels;
    using System.Runtime.Remoting.Contexts;
    using System.Security;
    using System.Security.Permissions;
    using System.Security.Policy;
    using System.Security.Principal;
    using System.Security.Util;
    using System.Text;
    using System.Threading;

    [ComDefaultInterface(typeof(_AppDomain)), ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public sealed class AppDomain : MarshalByRefObject, _AppDomain, IEvidenceFactory
    {
        private System.ActivationContext _activationContext;
        private System.ApplicationIdentity _applicationIdentity;
        private System.Security.Policy.ApplicationTrust _applicationTrust;
        private Context _DefaultContext;
        private IPrincipal _DefaultPrincipal;
        private AppDomainManager _domainManager;
        private EventHandler _domainUnload;
        private IntPtr _dummyField;
        private AppDomainSetup _FusionStore;
        private bool _HasSetPolicy;
        private Hashtable _LocalStore;
        private object[] _Policies;
        private PrincipalPolicy _PrincipalPolicy;
        private EventHandler _processExit;
        private DomainSpecificRemotingData _RemotingData;
        private System.Security.Policy.Evidence _SecurityIdentity;
        private UnhandledExceptionEventHandler _unhandledException;

        public event AssemblyLoadEventHandler AssemblyLoad;

        public event ResolveEventHandler AssemblyResolve;

        public event EventHandler DomainUnload
        {
            [SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain=true)] add
            {
                if (value != null)
                {
                    RuntimeHelpers.PrepareDelegate(value);
                    lock (this)
                    {
                        this._domainUnload = (EventHandler) Delegate.Combine(this._domainUnload, value);
                    }
                }
            }
            [SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain=true)] remove
            {
                lock (this)
                {
                    this._domainUnload = (EventHandler) Delegate.Remove(this._domainUnload, value);
                }
            }
        }

        public event EventHandler ProcessExit
        {
            [SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain=true)] add
            {
                if (value != null)
                {
                    RuntimeHelpers.PrepareDelegate(value);
                    lock (this)
                    {
                        this._processExit = (EventHandler) Delegate.Combine(this._processExit, value);
                    }
                }
            }
            [SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain=true)] remove
            {
                lock (this)
                {
                    this._processExit = (EventHandler) Delegate.Remove(this._processExit, value);
                }
            }
        }

        public event ResolveEventHandler ReflectionOnlyAssemblyResolve;

        public event ResolveEventHandler ResourceResolve;

        public event ResolveEventHandler TypeResolve;

        public event UnhandledExceptionEventHandler UnhandledException
        {
            [SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain=true)] add
            {
                if (value != null)
                {
                    RuntimeHelpers.PrepareDelegate(value);
                    lock (this)
                    {
                        this._unhandledException = (UnhandledExceptionEventHandler) Delegate.Combine(this._unhandledException, value);
                    }
                }
            }
            [SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain=true)] remove
            {
                lock (this)
                {
                    this._unhandledException = (UnhandledExceptionEventHandler) Delegate.Remove(this._unhandledException, value);
                }
            }
        }

        private AppDomain()
        {
            throw new NotSupportedException(Environment.GetResourceString("NotSupported_Constructor"));
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int _nExecuteAssembly(Assembly assembly, string[] args);
        private int ActivateApplication()
        {
            return (int) Activator.CreateInstance(CurrentDomain.ActivationContext).Unwrap();
        }

        [Obsolete("AppDomain.AppendPrivatePath has been deprecated. Please investigate the use of AppDomainSetup.PrivateBinPath instead. http://go.microsoft.com/fwlink/?linkid=14202"), SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain=true)]
        public void AppendPrivatePath(string path)
        {
            if ((path != null) && (path.Length != 0))
            {
                string str = this.FusionStore.Value[5];
                StringBuilder builder = new StringBuilder();
                if ((str != null) && (str.Length > 0))
                {
                    builder.Append(str);
                    if ((str[str.Length - 1] != Path.PathSeparator) && (path[0] != Path.PathSeparator))
                    {
                        builder.Append(Path.PathSeparator);
                    }
                }
                builder.Append(path);
                string str2 = builder.ToString();
                this.InternalSetPrivateBinPath(str2);
            }
        }

        [ComVisible(false)]
        public string ApplyPolicy(string assemblyName)
        {
            AssemblyName an = new AssemblyName(assemblyName);
            byte[] publicKeyToken = an.GetPublicKeyToken();
            if (publicKeyToken == null)
            {
                publicKeyToken = an.GetPublicKey();
            }
            if ((publicKeyToken != null) && (publicKeyToken.Length != 0))
            {
                return this.nApplyPolicy(an);
            }
            return assemblyName;
        }

        [Obsolete("AppDomain.ClearPrivatePath has been deprecated. Please investigate the use of AppDomainSetup.PrivateBinPath instead. http://go.microsoft.com/fwlink/?linkid=14202"), SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain=true)]
        public void ClearPrivatePath()
        {
            this.InternalSetPrivateBinPath(string.Empty);
        }

        [Obsolete("AppDomain.ClearShadowCopyPath has been deprecated. Please investigate the use of AppDomainSetup.ShadowCopyDirectories instead. http://go.microsoft.com/fwlink/?linkid=14202"), SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain=true)]
        public void ClearShadowCopyPath()
        {
            this.InternalSetShadowCopyPath(string.Empty);
        }

        public ObjectHandle CreateComInstanceFrom(string assemblyName, string typeName)
        {
            if (this == null)
            {
                throw new NullReferenceException();
            }
            return Activator.CreateComInstanceFrom(assemblyName, typeName);
        }

        public ObjectHandle CreateComInstanceFrom(string assemblyFile, string typeName, byte[] hashValue, AssemblyHashAlgorithm hashAlgorithm)
        {
            if (this == null)
            {
                throw new NullReferenceException();
            }
            return Activator.CreateComInstanceFrom(assemblyFile, typeName, hashValue, hashAlgorithm);
        }

        internal void CreateDefaultContext()
        {
            lock (this)
            {
                if (this._DefaultContext == null)
                {
                    this._DefaultContext = Context.CreateDefaultContext();
                }
            }
        }

        public static AppDomain CreateDomain(string friendlyName)
        {
            return CreateDomain(friendlyName, null, null);
        }

        public static AppDomain CreateDomain(string friendlyName, System.Security.Policy.Evidence securityInfo)
        {
            return CreateDomain(friendlyName, securityInfo, null);
        }

        [SecurityPermission(SecurityAction.Demand, ControlAppDomain=true)]
        public static AppDomain CreateDomain(string friendlyName, System.Security.Policy.Evidence securityInfo, AppDomainSetup info)
        {
            AppDomainManager domainManager = CurrentDomain.DomainManager;
            if (domainManager != null)
            {
                return domainManager.CreateDomain(friendlyName, securityInfo, info);
            }
            if (friendlyName == null)
            {
                throw new ArgumentNullException(Environment.GetResourceString("ArgumentNull_String"));
            }
            if (securityInfo != null)
            {
                new SecurityPermission(SecurityPermissionFlag.ControlEvidence).Demand();
            }
            return nCreateDomain(friendlyName, info, securityInfo, (securityInfo == null) ? CurrentDomain.InternalEvidence : null, CurrentDomain.GetSecurityDescriptor());
        }

        public static AppDomain CreateDomain(string friendlyName, System.Security.Policy.Evidence securityInfo, AppDomainSetup info, PermissionSet grantSet, params StrongName[] fullTrustAssemblies)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            if (info.ApplicationBase == null)
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_AppDomainSandboxAPINeedsExplicitAppBase"));
            }
            info.ApplicationTrust = new System.Security.Policy.ApplicationTrust(grantSet, fullTrustAssemblies);
            return CreateDomain(friendlyName, securityInfo, info);
        }

        public static AppDomain CreateDomain(string friendlyName, System.Security.Policy.Evidence securityInfo, string appBasePath, string appRelativeSearchPath, bool shadowCopyFiles)
        {
            AppDomainSetup info = new AppDomainSetup {
                ApplicationBase = appBasePath,
                PrivateBinPath = appRelativeSearchPath
            };
            if (shadowCopyFiles)
            {
                info.ShadowCopyFiles = "true";
            }
            return CreateDomain(friendlyName, securityInfo, info);
        }

        public static AppDomain CreateDomain(string friendlyName, System.Security.Policy.Evidence securityInfo, string appBasePath, string appRelativeSearchPath, bool shadowCopyFiles, AppDomainInitializer adInit, string[] adInitArgs)
        {
            AppDomainSetup info = new AppDomainSetup {
                ApplicationBase = appBasePath,
                PrivateBinPath = appRelativeSearchPath,
                AppDomainInitializer = adInit,
                AppDomainInitializerArguments = adInitArgs
            };
            if (shadowCopyFiles)
            {
                info.ShadowCopyFiles = "true";
            }
            return CreateDomain(friendlyName, securityInfo, info);
        }

        private AppDomainManager CreateDomainManager(string domainManagerAssemblyName, string domainManagerTypeName)
        {
            AppDomainManager manager = null;
            try
            {
                manager = this.CreateInstanceAndUnwrap(domainManagerAssemblyName, domainManagerTypeName) as AppDomainManager;
            }
            catch (FileNotFoundException)
            {
            }
            catch (TypeLoadException)
            {
            }
            finally
            {
                if (manager == null)
                {
                    throw new TypeLoadException(Environment.GetResourceString("Argument_NoDomainManager"));
                }
            }
            return manager;
        }

        public ObjectHandle CreateInstance(string assemblyName, string typeName)
        {
            if (this == null)
            {
                throw new NullReferenceException();
            }
            if (assemblyName == null)
            {
                throw new ArgumentNullException("assemblyName");
            }
            return Activator.CreateInstance(assemblyName, typeName);
        }

        public ObjectHandle CreateInstance(string assemblyName, string typeName, object[] activationAttributes)
        {
            if (this == null)
            {
                throw new NullReferenceException();
            }
            if (assemblyName == null)
            {
                throw new ArgumentNullException("assemblyName");
            }
            return Activator.CreateInstance(assemblyName, typeName, activationAttributes);
        }

        public ObjectHandle CreateInstance(string assemblyName, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, System.Security.Policy.Evidence securityAttributes)
        {
            if (this == null)
            {
                throw new NullReferenceException();
            }
            if (assemblyName == null)
            {
                throw new ArgumentNullException("assemblyName");
            }
            return Activator.CreateInstance(assemblyName, typeName, ignoreCase, bindingAttr, binder, args, culture, activationAttributes, securityAttributes);
        }

        public object CreateInstanceAndUnwrap(string assemblyName, string typeName)
        {
            ObjectHandle handle = this.CreateInstance(assemblyName, typeName);
            if (handle == null)
            {
                return null;
            }
            return handle.Unwrap();
        }

        public object CreateInstanceAndUnwrap(string assemblyName, string typeName, object[] activationAttributes)
        {
            ObjectHandle handle = this.CreateInstance(assemblyName, typeName, activationAttributes);
            if (handle == null)
            {
                return null;
            }
            return handle.Unwrap();
        }

        public object CreateInstanceAndUnwrap(string assemblyName, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, System.Security.Policy.Evidence securityAttributes)
        {
            ObjectHandle handle = this.CreateInstance(assemblyName, typeName, ignoreCase, bindingAttr, binder, args, culture, activationAttributes, securityAttributes);
            if (handle == null)
            {
                return null;
            }
            return handle.Unwrap();
        }

        public ObjectHandle CreateInstanceFrom(string assemblyFile, string typeName)
        {
            if (this == null)
            {
                throw new NullReferenceException();
            }
            return Activator.CreateInstanceFrom(assemblyFile, typeName);
        }

        public ObjectHandle CreateInstanceFrom(string assemblyFile, string typeName, object[] activationAttributes)
        {
            if (this == null)
            {
                throw new NullReferenceException();
            }
            return Activator.CreateInstanceFrom(assemblyFile, typeName, activationAttributes);
        }

        public ObjectHandle CreateInstanceFrom(string assemblyFile, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, System.Security.Policy.Evidence securityAttributes)
        {
            if (this == null)
            {
                throw new NullReferenceException();
            }
            return Activator.CreateInstanceFrom(assemblyFile, typeName, ignoreCase, bindingAttr, binder, args, culture, activationAttributes, securityAttributes);
        }

        public object CreateInstanceFromAndUnwrap(string assemblyName, string typeName)
        {
            ObjectHandle handle = this.CreateInstanceFrom(assemblyName, typeName);
            if (handle == null)
            {
                return null;
            }
            return handle.Unwrap();
        }

        public object CreateInstanceFromAndUnwrap(string assemblyName, string typeName, object[] activationAttributes)
        {
            ObjectHandle handle = this.CreateInstanceFrom(assemblyName, typeName, activationAttributes);
            if (handle == null)
            {
                return null;
            }
            return handle.Unwrap();
        }

        public object CreateInstanceFromAndUnwrap(string assemblyName, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, System.Security.Policy.Evidence securityAttributes)
        {
            ObjectHandle handle = this.CreateInstanceFrom(assemblyName, typeName, ignoreCase, bindingAttr, binder, args, culture, activationAttributes, securityAttributes);
            if (handle == null)
            {
                return null;
            }
            return handle.Unwrap();
        }

        internal void CreateRemotingData()
        {
            lock (this)
            {
                if (this._RemotingData == null)
                {
                    this._RemotingData = new DomainSpecificRemotingData();
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return this.InternalDefineDynamicAssembly(name, access, null, null, null, null, null, ref lookForMyCaller, null);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, IEnumerable<CustomAttributeBuilder> assemblyAttributes)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return this.InternalDefineDynamicAssembly(name, access, null, null, null, null, null, ref lookForMyCaller, assemblyAttributes);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, System.Security.Policy.Evidence evidence)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return this.InternalDefineDynamicAssembly(name, access, null, evidence, null, null, null, ref lookForMyCaller, null);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, string dir)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return this.InternalDefineDynamicAssembly(name, access, dir, null, null, null, null, ref lookForMyCaller, null);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, string dir, System.Security.Policy.Evidence evidence)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return this.InternalDefineDynamicAssembly(name, access, dir, evidence, null, null, null, ref lookForMyCaller, null);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, PermissionSet requiredPermissions, PermissionSet optionalPermissions, PermissionSet refusedPermissions)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return this.InternalDefineDynamicAssembly(name, access, null, null, requiredPermissions, optionalPermissions, refusedPermissions, ref lookForMyCaller, null);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, System.Security.Policy.Evidence evidence, PermissionSet requiredPermissions, PermissionSet optionalPermissions, PermissionSet refusedPermissions)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return this.InternalDefineDynamicAssembly(name, access, null, evidence, requiredPermissions, optionalPermissions, refusedPermissions, ref lookForMyCaller, null);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, string dir, PermissionSet requiredPermissions, PermissionSet optionalPermissions, PermissionSet refusedPermissions)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return this.InternalDefineDynamicAssembly(name, access, dir, null, requiredPermissions, optionalPermissions, refusedPermissions, ref lookForMyCaller, null);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, string dir, System.Security.Policy.Evidence evidence, PermissionSet requiredPermissions, PermissionSet optionalPermissions, PermissionSet refusedPermissions)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return this.InternalDefineDynamicAssembly(name, access, dir, evidence, requiredPermissions, optionalPermissions, refusedPermissions, ref lookForMyCaller, null);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, string dir, System.Security.Policy.Evidence evidence, PermissionSet requiredPermissions, PermissionSet optionalPermissions, PermissionSet refusedPermissions, bool isSynchronized)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return this.InternalDefineDynamicAssembly(name, access, dir, evidence, requiredPermissions, optionalPermissions, refusedPermissions, ref lookForMyCaller, null);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, string dir, System.Security.Policy.Evidence evidence, PermissionSet requiredPermissions, PermissionSet optionalPermissions, PermissionSet refusedPermissions, bool isSynchronized, IEnumerable<CustomAttributeBuilder> assemblyAttributes)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return this.InternalDefineDynamicAssembly(name, access, dir, evidence, requiredPermissions, optionalPermissions, refusedPermissions, ref lookForMyCaller, assemblyAttributes);
        }

        private static object Deserialize(byte[] blob)
        {
            if (blob == null)
            {
                return null;
            }
            if (blob[0] == 0)
            {
                SecurityElement topElement = new Parser(blob, Tokenizer.ByteTokenEncoding.UTF8Tokens, 1).GetTopElement();
                if (topElement.Tag.Equals("IPermission") || topElement.Tag.Equals("Permission"))
                {
                    IPermission permission = XMLUtil.CreatePermission(topElement, PermissionState.None, false);
                    if (permission == null)
                    {
                        return null;
                    }
                    permission.FromXml(topElement);
                    return permission;
                }
                if (topElement.Tag.Equals("PermissionSet"))
                {
                    PermissionSet set = new PermissionSet();
                    set.FromXml(topElement, false, false);
                    return set;
                }
                if (topElement.Tag.Equals("PermissionToken"))
                {
                    PermissionToken token = new PermissionToken();
                    token.FromXml(topElement);
                    return token;
                }
                return null;
            }
            using (MemoryStream stream = new MemoryStream(blob, 1, blob.Length - 1))
            {
                return CrossAppDomainSerializer.DeserializeObject(stream);
            }
        }

        public void DoCallBack(CrossAppDomainDelegate callBackDelegate)
        {
            if (callBackDelegate == null)
            {
                throw new ArgumentNullException("callBackDelegate");
            }
            callBackDelegate();
        }

        private void EnableResolveAssembliesForIntrospection()
        {
            AppDomain currentDomain = CurrentDomain;
            currentDomain.ReflectionOnlyAssemblyResolve = (ResolveEventHandler) Delegate.Combine(currentDomain.ReflectionOnlyAssemblyResolve, new ResolveEventHandler(this.ResolveAssemblyForIntrospection));
        }

        public int ExecuteAssembly(string assemblyFile)
        {
            return this.ExecuteAssembly(assemblyFile, null, null);
        }

        public int ExecuteAssembly(string assemblyFile, System.Security.Policy.Evidence assemblySecurity)
        {
            return this.ExecuteAssembly(assemblyFile, assemblySecurity, null);
        }

        public int ExecuteAssembly(string assemblyFile, System.Security.Policy.Evidence assemblySecurity, string[] args)
        {
            Assembly assembly = Assembly.LoadFrom(assemblyFile, assemblySecurity);
            if (args == null)
            {
                args = new string[0];
            }
            return this.nExecuteAssembly(assembly, args);
        }

        public int ExecuteAssembly(string assemblyFile, System.Security.Policy.Evidence assemblySecurity, string[] args, byte[] hashValue, AssemblyHashAlgorithm hashAlgorithm)
        {
            Assembly assembly = Assembly.LoadFrom(assemblyFile, assemblySecurity, hashValue, hashAlgorithm);
            if (args == null)
            {
                args = new string[0];
            }
            return this.nExecuteAssembly(assembly, args);
        }

        public int ExecuteAssemblyByName(string assemblyName)
        {
            return this.ExecuteAssemblyByName(assemblyName, null, null);
        }

        public int ExecuteAssemblyByName(string assemblyName, System.Security.Policy.Evidence assemblySecurity)
        {
            return this.ExecuteAssemblyByName(assemblyName, assemblySecurity, null);
        }

        public int ExecuteAssemblyByName(AssemblyName assemblyName, System.Security.Policy.Evidence assemblySecurity, params string[] args)
        {
            Assembly assembly = Assembly.Load(assemblyName, assemblySecurity);
            if (args == null)
            {
                args = new string[0];
            }
            return this.nExecuteAssembly(assembly, args);
        }

        public int ExecuteAssemblyByName(string assemblyName, System.Security.Policy.Evidence assemblySecurity, params string[] args)
        {
            Assembly assembly = Assembly.Load(assemblyName, assemblySecurity);
            if (args == null)
            {
                args = new string[0];
            }
            return this.nExecuteAssembly(assembly, args);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern uint GetAppDomainId();
        public Assembly[] GetAssemblies()
        {
            return this.nGetAssemblies(false);
        }

        [Obsolete("AppDomain.GetCurrentThreadId has been deprecated because it does not provide a stable Id when managed threads are running on fibers (aka lightweight threads). To get a stable identifier for a managed thread, use the ManagedThreadId property on Thread.  http://go.microsoft.com/fwlink/?linkid=14202", false), DllImport("kernel32.dll")]
        public static extern int GetCurrentThreadId();
        public object GetData(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            switch (AppDomainSetup.Locate(name))
            {
                case 0:
                    return this.FusionStore.ApplicationBase;

                case 1:
                    return this.FusionStore.ConfigurationFile;

                case 2:
                    return this.FusionStore.DynamicBase;

                case 3:
                    return this.FusionStore.DeveloperPath;

                case 4:
                    return this.FusionStore.ApplicationName;

                case 5:
                    return this.FusionStore.PrivateBinPath;

                case 6:
                    return this.FusionStore.PrivateBinPathProbe;

                case 7:
                    return this.FusionStore.ShadowCopyDirectories;

                case 8:
                    return this.FusionStore.ShadowCopyFiles;

                case 9:
                    return this.FusionStore.CachePath;

                case 10:
                    return this.FusionStore.LicenseFile;

                case 11:
                    return this.FusionStore.DisallowPublisherPolicy;

                case 12:
                    return this.FusionStore.DisallowCodeDownload;

                case 13:
                    return this.FusionStore.DisallowBindingRedirects;

                case 14:
                    return this.FusionStore.DisallowApplicationBaseProbing;

                case 15:
                    return this.FusionStore.GetConfigurationBytes();

                case -1:
                {
                    if (name.Equals(AppDomainSetup.LoaderOptimizationKey))
                    {
                        return this.FusionStore.LoaderOptimization;
                    }
                    object[] objArray = (object[]) this.LocalStore[name];
                    if (objArray == null)
                    {
                        return null;
                    }
                    if (objArray[1] != null)
                    {
                        ((IPermission) objArray[1]).Demand();
                    }
                    return objArray[0];
                }
            }
            return null;
        }

        internal Context GetDefaultContext()
        {
            if (this._DefaultContext == null)
            {
                this.CreateDefaultContext();
            }
            return this._DefaultContext;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern AppDomain GetDefaultDomain();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern string GetDynamicDir();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern IntPtr GetFusionContext();
        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        internal extern int GetId();
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        internal static int GetIdForUnload(AppDomain domain)
        {
            if (RemotingServices.IsTransparentProxy(domain))
            {
                return RemotingServices.GetServerDomainIdForProxy(domain);
            }
            return domain.Id;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern string GetOrInternString(string str);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern IntPtr GetSecurityDescriptor();
        internal IPrincipal GetThreadPrincipal()
        {
            IPrincipal principal = null;
            IPrincipal principal2;
            lock (this)
            {
                if (this._DefaultPrincipal == null)
                {
                    switch (this._PrincipalPolicy)
                    {
                        case PrincipalPolicy.UnauthenticatedPrincipal:
                            principal = new GenericPrincipal(new GenericIdentity("", ""), new string[] { "" });
                            goto Label_0073;

                        case PrincipalPolicy.NoPrincipal:
                            principal = null;
                            goto Label_0073;

                        case PrincipalPolicy.WindowsPrincipal:
                            principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
                            goto Label_0073;
                    }
                    principal = null;
                }
                else
                {
                    principal = this._DefaultPrincipal;
                }
            Label_0073:
                principal2 = principal;
            }
            return principal2;
        }

        public Type GetType()
        {
            return base.GetType();
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        private static AppDomain InternalCreateDomain(string imageLocation)
        {
            AppDomainSetup info = InternalCreateDomainSetup(imageLocation);
            return CreateDomain("Validator", null, info);
        }

        private static AppDomainSetup InternalCreateDomainSetup(string imageLocation)
        {
            int num = imageLocation.LastIndexOf('\\');
            AppDomainSetup setup = new AppDomainSetup {
                ApplicationBase = imageLocation.Substring(0, num + 1)
            };
            StringBuilder builder = new StringBuilder(imageLocation.Substring(num + 1));
            builder.Append(AppDomainSetup.ConfigurationExtension);
            setup.ConfigurationFile = builder.ToString();
            return setup;
        }

        internal ObjectHandle InternalCreateInstanceFromWithNoSecurity(string assemblyName, string typeName)
        {
            PermissionSet.s_fullTrust.Assert();
            return this.CreateInstanceFrom(assemblyName, typeName);
        }

        internal ObjectHandle InternalCreateInstanceFromWithNoSecurity(string assemblyName, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, System.Security.Policy.Evidence securityAttributes)
        {
            PermissionSet.s_fullTrust.Assert();
            return this.CreateInstanceFrom(assemblyName, typeName, ignoreCase, bindingAttr, binder, args, culture, activationAttributes, securityAttributes);
        }

        internal ObjectHandle InternalCreateInstanceWithNoSecurity(string assemblyName, string typeName)
        {
            PermissionSet.s_fullTrust.Assert();
            return this.CreateInstance(assemblyName, typeName);
        }

        internal ObjectHandle InternalCreateInstanceWithNoSecurity(string assemblyName, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, System.Security.Policy.Evidence securityAttributes)
        {
            PermissionSet.s_fullTrust.Assert();
            return this.CreateInstance(assemblyName, typeName, ignoreCase, bindingAttr, binder, args, culture, activationAttributes, securityAttributes);
        }

        internal AssemblyBuilder InternalDefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, string dir, System.Security.Policy.Evidence evidence, PermissionSet requiredPermissions, PermissionSet optionalPermissions, PermissionSet refusedPermissions, ref StackCrawlMark stackMark, IEnumerable<CustomAttributeBuilder> unsafeAssemblyAttributes)
        {
            lock (typeof(AssemblyBuilderLock))
            {
                AssemblyBuilder builder2;
                if (name == null)
                {
                    throw new ArgumentNullException("name");
                }
                if (((access != AssemblyBuilderAccess.Run) && (access != AssemblyBuilderAccess.Save)) && ((access != AssemblyBuilderAccess.RunAndSave) && (access != AssemblyBuilderAccess.ReflectionOnly)))
                {
                    throw new ArgumentException(Environment.GetResourceString("Arg_EnumIllegalVal", new object[] { (int) access }), "access");
                }
                if (name.KeyPair != null)
                {
                    name.SetPublicKey(name.KeyPair.PublicKey);
                }
                if (evidence != null)
                {
                    new SecurityPermission(SecurityPermissionFlag.ControlEvidence).Demand();
                }
                List<CustomAttributeBuilder> list = null;
                DynamicAssemblyFlags none = DynamicAssemblyFlags.None;
                if (unsafeAssemblyAttributes != null)
                {
                    list = new List<CustomAttributeBuilder>(unsafeAssemblyAttributes);
                    foreach (CustomAttributeBuilder builder in list)
                    {
                        if (builder.m_con.DeclaringType == typeof(SecurityTransparentAttribute))
                        {
                            none |= DynamicAssemblyFlags.Transparent;
                        }
                    }
                }
                builder2 = new AssemblyBuilder((AssemblyBuilder) this.nCreateDynamicAssembly(name, evidence, ref stackMark, requiredPermissions, optionalPermissions, refusedPermissions, access, none)) {
                    m_assemblyData = new AssemblyBuilderData(builder2, name.Name, access, dir)
                };
                builder2.m_assemblyData.AddPermissionRequests(requiredPermissions, optionalPermissions, refusedPermissions);
                if (list != null)
                {
                    foreach (CustomAttributeBuilder builder3 in list)
                    {
                        builder2.SetCustomAttribute(builder3);
                    }
                }
                builder2.m_assemblyData.GetInMemoryAssemblyModule();
                return builder2;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static object InternalRemotelySetupRemoteDomain(IntPtr contextId, int domainId, string friendlyName, AppDomainSetup setup, IntPtr parentSecurityDescriptor, char[] serProvidedEvidence, char[] serCreatorEvidence, byte[] serializedEvidence, AppDomainInitializerInfo initializerInfo)
        {
            InternalCrossContextDelegate ftnToCall = new InternalCrossContextDelegate(AppDomain.InternalRemotelySetupRemoteDomainHelper);
            object[] args = new object[] { friendlyName, setup, parentSecurityDescriptor, serProvidedEvidence, serCreatorEvidence, serializedEvidence, initializerInfo };
            return Thread.CurrentThread.InternalCrossContextCallback(null, contextId, domainId, ftnToCall, args);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static object InternalRemotelySetupRemoteDomainHelper(object[] args)
        {
            string friendlyName = (string) args[0];
            AppDomainSetup copy = (AppDomainSetup) args[1];
            IntPtr parentSecurityDescriptor = (IntPtr) args[2];
            char[] chArray = (char[]) args[3];
            char[] chArray2 = (char[]) args[4];
            byte[] buffer = (byte[]) args[5];
            AppDomainInitializerInfo info = (AppDomainInitializerInfo) args[6];
            AppDomain appDomain = Thread.CurrentContext.AppDomain;
            AppDomainSetup setup2 = new AppDomainSetup(copy, false);
            appDomain.SetupFusionStore(setup2);
            System.Security.Policy.Evidence providedSecurityInfo = null;
            System.Security.Policy.Evidence creatorsSecurityInfo = null;
            if (buffer == null)
            {
                if (chArray != null)
                {
                    providedSecurityInfo = new System.Security.Policy.Evidence(chArray);
                }
                if (chArray2 != null)
                {
                    creatorsSecurityInfo = new System.Security.Policy.Evidence(chArray2);
                }
            }
            else
            {
                EvidenceCollection evidences = (EvidenceCollection) CrossAppDomainSerializer.DeserializeObject(new MemoryStream(buffer));
                providedSecurityInfo = evidences.ProvidedSecurityInfo;
                creatorsSecurityInfo = evidences.CreatorsSecurityInfo;
            }
            appDomain.nSetupFriendlyName(friendlyName);
            if ((copy != null) && copy.SandboxInterop)
            {
                appDomain.nSetDisableInterfaceCache();
            }
            appDomain.SetDomainManager(providedSecurityInfo, creatorsSecurityInfo, parentSecurityDescriptor, true);
            if (info != null)
            {
                setup2.AppDomainInitializer = info.Unwrap();
            }
            RunInitializer(setup2);
            ObjectHandle handle = null;
            AppDomainSetup fusionStore = appDomain.FusionStore;
            if ((fusionStore.ActivationArguments != null) && fusionStore.ActivationArguments.ActivateInstance)
            {
                handle = Activator.CreateInstance(appDomain.ActivationContext);
            }
            return RemotingServices.MarshalInternal(handle, null, null);
        }

        internal void InternalSetCachePath(string path)
        {
            IntPtr fusionContext = this.GetFusionContext();
            this.FusionStore.CachePath = path;
            AppDomainSetup.UpdateContextProperty(fusionContext, AppDomainSetup.CachePathKey, this.FusionStore.Value[9]);
        }

        private void InternalSetDomainContext(string imageLocation)
        {
            this.SetupFusionStore(InternalCreateDomainSetup(imageLocation));
        }

        internal void InternalSetDynamicBase(string path)
        {
            IntPtr fusionContext = this.GetFusionContext();
            this.FusionStore.DynamicBase = path;
            AppDomainSetup.UpdateContextProperty(fusionContext, AppDomainSetup.DynamicBaseKey, this.FusionStore.Value[2]);
        }

        internal void InternalSetPrivateBinPath(string path)
        {
            AppDomainSetup.UpdateContextProperty(this.GetFusionContext(), AppDomainSetup.PrivateBinPathKey, path);
            this.FusionStore.PrivateBinPath = path;
        }

        internal void InternalSetShadowCopyFiles()
        {
            AppDomainSetup.UpdateContextProperty(this.GetFusionContext(), AppDomainSetup.ShadowCopyFilesKey, "true");
            this.FusionStore.ShadowCopyFiles = "true";
        }

        internal void InternalSetShadowCopyPath(string path)
        {
            AppDomainSetup.UpdateContextProperty(this.GetFusionContext(), AppDomainSetup.ShadowCopyDirectoriesKey, path);
            this.FusionStore.ShadowCopyDirectories = path;
        }

        public bool IsDefaultAppDomain()
        {
            return (this == GetDefaultDomain());
        }

        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        internal static extern bool IsDomainIdValid(int id);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool IsFinalizingForUnload();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern string IsStringInterned(string str);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern bool IsUnloadingForcedFinalize();
        [MethodImpl(MethodImplOptions.NoInlining)]
        public Assembly Load(AssemblyName assemblyRef)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return Assembly.InternalLoad(assemblyRef, null, ref lookForMyCaller, false);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public Assembly Load(string assemblyString)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return Assembly.InternalLoad(assemblyString, null, ref lookForMyCaller, false);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public Assembly Load(byte[] rawAssembly)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return Assembly.nLoadImage(rawAssembly, null, null, ref lookForMyCaller, false);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public Assembly Load(byte[] rawAssembly, byte[] rawSymbolStore)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return Assembly.nLoadImage(rawAssembly, rawSymbolStore, null, ref lookForMyCaller, false);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public Assembly Load(AssemblyName assemblyRef, System.Security.Policy.Evidence assemblySecurity)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return Assembly.InternalLoad(assemblyRef, assemblySecurity, ref lookForMyCaller, false);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public Assembly Load(string assemblyString, System.Security.Policy.Evidence assemblySecurity)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return Assembly.InternalLoad(assemblyString, assemblySecurity, ref lookForMyCaller, false);
        }

        [MethodImpl(MethodImplOptions.NoInlining), SecurityPermission(SecurityAction.Demand, ControlEvidence=true)]
        public Assembly Load(byte[] rawAssembly, byte[] rawSymbolStore, System.Security.Policy.Evidence securityEvidence)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return Assembly.nLoadImage(rawAssembly, rawSymbolStore, securityEvidence, ref lookForMyCaller, false);
        }

        private static byte[] MarshalObject(object o)
        {
            CodeAccessPermission.AssertAllPossible();
            return Serialize(o);
        }

        private static byte[] MarshalObjects(object o1, object o2, out byte[] blob2)
        {
            CodeAccessPermission.AssertAllPossible();
            byte[] buffer = Serialize(o1);
            blob2 = Serialize(o2);
            return buffer;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern string nApplyPolicy(AssemblyName an);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void nChangeSecurityPolicy();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern AppDomain nCreateDomain(string friendlyName, AppDomainSetup setup, System.Security.Policy.Evidence providedSecurityInfo, System.Security.Policy.Evidence creatorsSecurityInfo, IntPtr parentSecurityDescriptor);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern Assembly nCreateDynamicAssembly(AssemblyName name, System.Security.Policy.Evidence identity, ref StackCrawlMark stackMark, PermissionSet requiredPermissions, PermissionSet optionalPermissions, PermissionSet refusedPermissions, AssemblyBuilderAccess access, DynamicAssemblyFlags flags);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern ObjRef nCreateInstance(string friendlyName, AppDomainSetup setup, System.Security.Policy.Evidence providedSecurityInfo, System.Security.Policy.Evidence creatorsSecurityInfo, IntPtr parentSecurityDescriptor);
        internal int nExecuteAssembly(Assembly assembly, string[] args)
        {
            return this._nExecuteAssembly(assembly.InternalAssembly, args);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern Assembly[] nGetAssemblies(bool forIntrospection);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern string nGetDomainManagerAsm();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern string nGetDomainManagerType();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern string nGetFriendlyName();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void nGetGrantSet(out PermissionSet granted, out PermissionSet denied);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern bool nIsDefaultAppDomainForSecurity();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void nSetDisableInterfaceCache();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void nSetHostSecurityManagerFlags(HostSecurityManagerOptions flags);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void nSetSecurityHomogeneousFlag();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void nSetupDomainSecurity(System.Security.Policy.Evidence appDomainEvidence, IntPtr creatorsSecurityDescriptor, bool publishAppDomain);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void nSetupFriendlyName(string friendlyName);
        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.MayCorruptAppDomain, Cer.MayFail)]
        internal static extern void nUnload(int domainInternal);
        private void OnAssemblyLoadEvent(Assembly LoadedAssembly)
        {
            AssemblyLoadEventHandler assemblyLoad = this.AssemblyLoad;
            if (assemblyLoad != null)
            {
                AssemblyLoadEventArgs args = new AssemblyLoadEventArgs(LoadedAssembly);
                assemblyLoad(this, args);
            }
        }

        private Assembly OnAssemblyResolveEvent(string assemblyFullName)
        {
            ResolveEventHandler assemblyResolve = this.AssemblyResolve;
            if (assemblyResolve != null)
            {
                Delegate[] invocationList = assemblyResolve.GetInvocationList();
                int length = invocationList.Length;
                for (int i = 0; i < length; i++)
                {
                    Assembly assembly = ((ResolveEventHandler) invocationList[i])(this, new ResolveEventArgs(assemblyFullName));
                    if (assembly != null)
                    {
                        return assembly.InternalAssembly;
                    }
                }
            }
            return null;
        }

        private Assembly OnReflectionOnlyAssemblyResolveEvent(string assemblyFullName)
        {
            ResolveEventHandler reflectionOnlyAssemblyResolve = this.ReflectionOnlyAssemblyResolve;
            if (reflectionOnlyAssemblyResolve != null)
            {
                Delegate[] invocationList = reflectionOnlyAssemblyResolve.GetInvocationList();
                int length = invocationList.Length;
                for (int i = 0; i < length; i++)
                {
                    Assembly assembly = ((ResolveEventHandler) invocationList[i])(this, new ResolveEventArgs(assemblyFullName));
                    if (assembly != null)
                    {
                        return assembly.InternalAssembly;
                    }
                }
            }
            return null;
        }

        private Assembly OnResourceResolveEvent(string resourceName)
        {
            ResolveEventHandler resourceResolve = this.ResourceResolve;
            if (resourceResolve != null)
            {
                Delegate[] invocationList = resourceResolve.GetInvocationList();
                int length = invocationList.Length;
                for (int i = 0; i < length; i++)
                {
                    Assembly assembly = ((ResolveEventHandler) invocationList[i])(this, new ResolveEventArgs(resourceName));
                    if (assembly != null)
                    {
                        return assembly.InternalAssembly;
                    }
                }
            }
            return null;
        }

        private Assembly OnTypeResolveEvent(string typeName)
        {
            ResolveEventHandler typeResolve = this.TypeResolve;
            if (typeResolve != null)
            {
                Delegate[] invocationList = typeResolve.GetInvocationList();
                int length = invocationList.Length;
                for (int i = 0; i < length; i++)
                {
                    Assembly assembly = ((ResolveEventHandler) invocationList[i])(this, new ResolveEventArgs(typeName));
                    if (assembly != null)
                    {
                        return assembly.InternalAssembly;
                    }
                }
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void PublishAnonymouslyHostedDynamicMethodsAssembly(Assembly assembly);
        public Assembly[] ReflectionOnlyGetAssemblies()
        {
            return this.nGetAssemblies(true);
        }

        private static object RemotelySetupRemoteDomain(AppDomain appDomainProxy, string friendlyName, AppDomainSetup setup, System.Security.Policy.Evidence providedSecurityInfo, System.Security.Policy.Evidence creatorsSecurityInfo, IntPtr parentSecurityDescriptor)
        {
            IntPtr ptr;
            int num;
            RemotingServices.GetServerContextAndDomainIdForProxy(appDomainProxy, out ptr, out num);
            if (ptr == IntPtr.Zero)
            {
                throw new AppDomainUnloadedException();
            }
            EvidenceCollection evidences = null;
            if ((providedSecurityInfo != null) || (creatorsSecurityInfo != null))
            {
                evidences = new EvidenceCollection {
                    ProvidedSecurityInfo = providedSecurityInfo,
                    CreatorsSecurityInfo = creatorsSecurityInfo
                };
            }
            bool flag = false;
            char[] serProvidedEvidence = null;
            char[] serCreatorEvidence = null;
            byte[] serializedEvidence = null;
            AppDomainInitializerInfo initializerInfo = null;
            if (providedSecurityInfo != null)
            {
                serProvidedEvidence = PolicyManager.MakeEvidenceArray(providedSecurityInfo, true);
                if (serProvidedEvidence == null)
                {
                    flag = true;
                }
            }
            if ((creatorsSecurityInfo != null) && !flag)
            {
                serCreatorEvidence = PolicyManager.MakeEvidenceArray(creatorsSecurityInfo, true);
                if (serCreatorEvidence == null)
                {
                    flag = true;
                }
            }
            if ((evidences != null) && flag)
            {
                serProvidedEvidence = (char[]) (serCreatorEvidence = null);
                serializedEvidence = CrossAppDomainSerializer.SerializeObject(evidences).GetBuffer();
            }
            if ((setup != null) && (setup.AppDomainInitializer != null))
            {
                initializerInfo = new AppDomainInitializerInfo(setup.AppDomainInitializer);
            }
            return InternalRemotelySetupRemoteDomain(ptr, num, friendlyName, setup, parentSecurityDescriptor, serProvidedEvidence, serCreatorEvidence, serializedEvidence, initializerInfo);
        }

        private Assembly ResolveAssemblyForIntrospection(object sender, ResolveEventArgs args)
        {
            return Assembly.ReflectionOnlyLoad(this.ApplyPolicy(args.Name));
        }

        private void RunDomainManagerPostInitialization(AppDomainManager domainManager)
        {
            HostExecutionContextManager hostExecutionContextManager = domainManager.HostExecutionContextManager;
            System.Security.HostSecurityManager hostSecurityManager = domainManager.HostSecurityManager;
            if ((hostSecurityManager != null) && ((hostSecurityManager.Flags & HostSecurityManagerOptions.HostPolicyLevel) == HostSecurityManagerOptions.HostPolicyLevel))
            {
                PolicyLevel domainPolicy = hostSecurityManager.DomainPolicy;
                if (domainPolicy != null)
                {
                    this.SetAppDomainPolicy(domainPolicy);
                }
            }
        }

        private static void RunInitializer(AppDomainSetup setup)
        {
            if (setup.AppDomainInitializer != null)
            {
                string[] args = null;
                if (setup.AppDomainInitializerArguments != null)
                {
                    args = (string[]) setup.AppDomainInitializerArguments.Clone();
                }
                setup.AppDomainInitializer(args);
            }
        }

        private static byte[] Serialize(object o)
        {
            if (o == null)
            {
                return null;
            }
            if (o is ISecurityEncodable)
            {
                SecurityElement element = ((ISecurityEncodable) o).ToXml();
                MemoryStream stream = new MemoryStream(0x1000);
                stream.WriteByte(0);
                StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);
                element.ToWriter(writer);
                writer.Flush();
                return stream.ToArray();
            }
            MemoryStream stm = new MemoryStream();
            stm.WriteByte(1);
            CrossAppDomainSerializer.SerializeObject(o, stm);
            return stm.ToArray();
        }

        [SecurityPermission(SecurityAction.LinkDemand, ControlDomainPolicy=true)]
        public void SetAppDomainPolicy(PolicyLevel domainPolicy)
        {
            if (domainPolicy == null)
            {
                throw new ArgumentNullException("domainPolicy");
            }
            lock (this)
            {
                if (this._HasSetPolicy)
                {
                    throw new PolicyException(Environment.GetResourceString("Policy_PolicyAlreadySet"));
                }
                this._HasSetPolicy = true;
                this.nChangeSecurityPolicy();
            }
            SecurityManager.PolicyManager.AddLevel(domainPolicy);
        }

        [Obsolete("AppDomain.SetCachePath has been deprecated. Please investigate the use of AppDomainSetup.CachePath instead. http://go.microsoft.com/fwlink/?linkid=14202"), SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain=true)]
        public void SetCachePath(string path)
        {
            this.InternalSetCachePath(path);
        }

        [SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain=true)]
        public void SetData(string name, object data)
        {
            this.SetDataHelper(name, data, null);
        }

        [SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain=true)]
        public void SetData(string name, object data, IPermission permission)
        {
            this.SetDataHelper(name, data, permission);
        }

        private void SetDataHelper(string name, object data, IPermission permission)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (name.Equals("IgnoreSystemPolicy"))
            {
                lock (this)
                {
                    if (!this._HasSetPolicy)
                    {
                        throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_SetData"));
                    }
                }
                new PermissionSet(PermissionState.Unrestricted).Demand();
            }
            int index = AppDomainSetup.Locate(name);
            if (index == -1)
            {
                this.LocalStore[name] = new object[] { data, permission };
            }
            else
            {
                if (permission != null)
                {
                    throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_SetData"));
                }
                switch (index)
                {
                    case 2:
                        this.FusionStore.DynamicBase = (string) data;
                        return;

                    case 3:
                        this.FusionStore.DeveloperPath = (string) data;
                        return;

                    case 7:
                        this.FusionStore.ShadowCopyDirectories = (string) data;
                        return;

                    case 11:
                        if (data == null)
                        {
                            this.FusionStore.DisallowPublisherPolicy = false;
                            return;
                        }
                        this.FusionStore.DisallowPublisherPolicy = true;
                        return;

                    case 12:
                        if (data == null)
                        {
                            this.FusionStore.DisallowCodeDownload = false;
                            return;
                        }
                        this.FusionStore.DisallowCodeDownload = true;
                        return;

                    case 13:
                        if (data == null)
                        {
                            this.FusionStore.DisallowBindingRedirects = false;
                            return;
                        }
                        this.FusionStore.DisallowBindingRedirects = true;
                        return;

                    case 14:
                        if (data == null)
                        {
                            this.FusionStore.DisallowApplicationBaseProbing = false;
                            return;
                        }
                        this.FusionStore.DisallowApplicationBaseProbing = true;
                        return;

                    case 15:
                        this.FusionStore.SetConfigurationBytes((byte[]) data);
                        return;
                }
                this.FusionStore.Value[index] = (string) data;
            }
        }

        private void SetDefaultDomainManager(string fullName, string[] manifestPaths, string[] activationData)
        {
            if (fullName != null)
            {
                this.FusionStore.ActivationArguments = new ActivationArguments(fullName, manifestPaths, activationData);
            }
            this.SetDomainManager(null, null, IntPtr.Zero, false);
        }

        private void SetDomainManager(System.Security.Policy.Evidence providedSecurityInfo, System.Security.Policy.Evidence creatorsSecurityInfo, IntPtr parentSecurityDescriptor, bool publishAppDomain)
        {
            string domainManagerAssemblyName = nGetDomainManagerAsm();
            string domainManagerTypeName = nGetDomainManagerType();
            if ((domainManagerAssemblyName != null) && (domainManagerTypeName != null))
            {
                this._domainManager = this.CreateDomainManager(domainManagerAssemblyName, domainManagerTypeName);
            }
            AppDomainSetup fusionStore = this.FusionStore;
            if (this._domainManager != null)
            {
                this._domainManager.InitializeNewDomain(fusionStore);
                if ((this._domainManager.InitializationFlags & AppDomainManagerInitializationOptions.RegisterWithHost) == AppDomainManagerInitializationOptions.RegisterWithHost)
                {
                    this._domainManager.nRegisterWithHost();
                }
            }
            if (fusionStore.ActivationArguments != null)
            {
                System.ActivationContext activationContext = null;
                System.ApplicationIdentity applicationIdentity = null;
                string[] activationData = null;
                CmsUtils.CreateActivationContext(fusionStore.ActivationArguments.ApplicationFullName, fusionStore.ActivationArguments.ApplicationManifestPaths, fusionStore.ActivationArguments.UseFusionActivationContext, out applicationIdentity, out activationContext);
                activationData = fusionStore.ActivationArguments.ActivationData;
                providedSecurityInfo = CmsUtils.MergeApplicationEvidence(providedSecurityInfo, applicationIdentity, activationContext, activationData, fusionStore.ApplicationTrust);
                this.SetupApplicationHelper(providedSecurityInfo, creatorsSecurityInfo, applicationIdentity, activationContext, activationData);
            }
            else
            {
                System.Security.Policy.ApplicationTrust applicationTrust = fusionStore.ApplicationTrust;
                if (applicationTrust != null)
                {
                    this.SetupDomainSecurityForApplication(applicationTrust.ApplicationIdentity, applicationTrust);
                }
            }
            System.Security.Policy.Evidence inputEvidence = (providedSecurityInfo != null) ? providedSecurityInfo : creatorsSecurityInfo;
            if (this._domainManager != null)
            {
                System.Security.HostSecurityManager hostSecurityManager = this._domainManager.HostSecurityManager;
                if (hostSecurityManager != null)
                {
                    nSetHostSecurityManagerFlags(hostSecurityManager.Flags);
                    if ((hostSecurityManager.Flags & HostSecurityManagerOptions.HostAppDomainEvidence) == HostSecurityManagerOptions.HostAppDomainEvidence)
                    {
                        inputEvidence = hostSecurityManager.ProvideAppDomainEvidence(inputEvidence);
                    }
                }
            }
            this._SecurityIdentity = inputEvidence;
            this.nSetupDomainSecurity(inputEvidence, parentSecurityDescriptor, publishAppDomain);
            if (this._domainManager != null)
            {
                this.RunDomainManagerPostInitialization(this._domainManager);
            }
        }

        [Obsolete("AppDomain.SetDynamicBase has been deprecated. Please investigate the use of AppDomainSetup.DynamicBase instead. http://go.microsoft.com/fwlink/?linkid=14202"), SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain=true)]
        public void SetDynamicBase(string path)
        {
            this.InternalSetDynamicBase(path);
        }

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.ControlPrincipal)]
        public void SetPrincipalPolicy(PrincipalPolicy policy)
        {
            this._PrincipalPolicy = policy;
        }

        [Obsolete("AppDomain.SetShadowCopyFiles has been deprecated. Please investigate the use of AppDomainSetup.ShadowCopyFiles instead. http://go.microsoft.com/fwlink/?linkid=14202"), SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain=true)]
        public void SetShadowCopyFiles()
        {
            this.InternalSetShadowCopyFiles();
        }

        [Obsolete("AppDomain.SetShadowCopyPath has been deprecated. Please investigate the use of AppDomainSetup.ShadowCopyDirectories instead. http://go.microsoft.com/fwlink/?linkid=14202"), SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain=true)]
        public void SetShadowCopyPath(string path)
        {
            this.InternalSetShadowCopyPath(path);
        }

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.ControlPrincipal)]
        public void SetThreadPrincipal(IPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException("principal");
            }
            lock (this)
            {
                if (this._DefaultPrincipal != null)
                {
                    throw new PolicyException(Environment.GetResourceString("Policy_PrincipalTwice"));
                }
                this._DefaultPrincipal = principal;
            }
        }

        private void SetupApplicationHelper(System.Security.Policy.Evidence providedSecurityInfo, System.Security.Policy.Evidence creatorsSecurityInfo, System.ApplicationIdentity appIdentity, System.ActivationContext activationContext, string[] activationData)
        {
            System.Security.Policy.ApplicationTrust appTrust = CurrentDomain.HostSecurityManager.DetermineApplicationTrust(providedSecurityInfo, creatorsSecurityInfo, new TrustManagerContext());
            if ((appTrust == null) || !appTrust.IsApplicationTrustedToRun)
            {
                throw new PolicyException(Environment.GetResourceString("Policy_NoExecutionPermission"), -2146233320, null);
            }
            if (activationContext != null)
            {
                this.SetupDomainForApplication(activationContext, activationData);
            }
            this.SetupDomainSecurityForApplication(appIdentity, appTrust);
        }

        private void SetupDomain(bool allowRedirects, string path, string configFile)
        {
            lock (this)
            {
                if (this._FusionStore == null)
                {
                    AppDomainSetup info = new AppDomainSetup();
                    if (path != null)
                    {
                        info.Value[0] = path;
                    }
                    if (configFile != null)
                    {
                        info.Value[1] = configFile;
                    }
                    if (!allowRedirects)
                    {
                        info.DisallowBindingRedirects = true;
                    }
                    this.SetupFusionStore(info);
                }
            }
        }

        private void SetupDomainForApplication(System.ActivationContext activationContext, string[] activationData)
        {
            if (this.IsDefaultAppDomain())
            {
                AppDomainSetup fusionStore = this.FusionStore;
                fusionStore.ActivationArguments = new ActivationArguments(activationContext, activationData);
                string entryPointFullPath = CmsUtils.GetEntryPointFullPath(activationContext);
                if (!string.IsNullOrEmpty(entryPointFullPath))
                {
                    fusionStore.SetupDefaultApplicationBase(entryPointFullPath);
                }
                else
                {
                    fusionStore.ApplicationBase = activationContext.ApplicationDirectory;
                }
                this.SetupFusionStore(fusionStore);
            }
            activationContext.PrepareForExecution();
            activationContext.SetApplicationState(System.ActivationContext.ApplicationState.Starting);
            activationContext.SetApplicationState(System.ActivationContext.ApplicationState.Running);
            IPermission permission = null;
            string dataDirectory = activationContext.DataDirectory;
            if ((dataDirectory != null) && (dataDirectory.Length > 0))
            {
                permission = new FileIOPermission(FileIOPermissionAccess.PathDiscovery, dataDirectory);
            }
            this.SetData("DataDirectory", dataDirectory, permission);
            this._activationContext = activationContext;
        }

        private void SetupDomainSecurityForApplication(System.ApplicationIdentity appIdentity, System.Security.Policy.ApplicationTrust appTrust)
        {
            this._applicationIdentity = appIdentity;
            this._applicationTrust = appTrust;
            nSetSecurityHomogeneousFlag();
        }

        private void SetupFusionStore(AppDomainSetup info)
        {
            if ((info.Value[0] == null) || (info.Value[1] == null))
            {
                AppDomain defaultDomain = GetDefaultDomain();
                if (this == defaultDomain)
                {
                    info.SetupDefaultApplicationBase(RuntimeEnvironment.GetModuleFileName());
                }
                else
                {
                    if (info.Value[1] == null)
                    {
                        info.ConfigurationFile = defaultDomain.FusionStore.Value[1];
                    }
                    if (info.Value[0] == null)
                    {
                        info.ApplicationBase = defaultDomain.FusionStore.Value[0];
                    }
                    if (info.Value[4] == null)
                    {
                        info.ApplicationName = defaultDomain.FusionStore.Value[4];
                    }
                }
            }
            if (info.Value[5] == null)
            {
                info.PrivateBinPath = Environment.nativeGetEnvironmentVariable(AppDomainSetup.PrivateBinPathEnvironmentVariable);
            }
            if (info.DeveloperPath == null)
            {
                info.DeveloperPath = RuntimeEnvironment.GetDeveloperPath();
            }
            IntPtr fusionContext = this.GetFusionContext();
            info.SetupFusionContext(fusionContext);
            if (info.LoaderOptimization != LoaderOptimization.NotSpecified)
            {
                this.UpdateLoaderOptimization((int) info.LoaderOptimization);
            }
            this._FusionStore = info;
        }

        private void SetupLoaderOptimization(LoaderOptimization policy)
        {
            if (policy != LoaderOptimization.NotSpecified)
            {
                this.FusionStore.LoaderOptimization = policy;
                this.UpdateLoaderOptimization((int) this.FusionStore.LoaderOptimization);
            }
        }

        void _AppDomain.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
        {
            throw new NotImplementedException();
        }

        void _AppDomain.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
        {
            throw new NotImplementedException();
        }

        void _AppDomain.GetTypeInfoCount(out uint pcTInfo)
        {
            throw new NotImplementedException();
        }

        void _AppDomain.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            string str = this.nGetFriendlyName();
            if (str != null)
            {
                builder.Append(Environment.GetResourceString("Loader_Name") + str);
                builder.Append(Environment.NewLine);
            }
            if ((this._Policies == null) || (this._Policies.Length == 0))
            {
                builder.Append(Environment.GetResourceString("Loader_NoContextPolicies") + Environment.NewLine);
            }
            else
            {
                builder.Append(Environment.GetResourceString("Loader_ContextPolicies") + Environment.NewLine);
                for (int i = 0; i < this._Policies.Length; i++)
                {
                    builder.Append(this._Policies[i]);
                    builder.Append(Environment.NewLine);
                }
            }
            return builder.ToString();
        }

        private void TurnOnBindingRedirects()
        {
            this._FusionStore.DisallowBindingRedirects = false;
        }

        [ReliabilityContract(Consistency.MayCorruptAppDomain, Cer.MayFail), SecurityPermission(SecurityAction.Demand, ControlAppDomain=true)]
        public static void Unload(AppDomain domain)
        {
            if (domain == null)
            {
                throw new ArgumentNullException("domain");
            }
            try
            {
                int idForUnload = GetIdForUnload(domain);
                if (idForUnload == 0)
                {
                    throw new CannotUnloadAppDomainException();
                }
                nUnload(idForUnload);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        private static object UnmarshalObject(byte[] blob)
        {
            CodeAccessPermission.AssertAllPossible();
            return Deserialize(blob);
        }

        private static object UnmarshalObjects(byte[] blob1, byte[] blob2, out object o2)
        {
            CodeAccessPermission.AssertAllPossible();
            object obj2 = Deserialize(blob1);
            o2 = Deserialize(blob2);
            return obj2;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void UpdateLoaderOptimization(int optimization);

        public System.ActivationContext ActivationContext
        {
            [SecurityPermission(SecurityAction.LinkDemand, ControlDomainPolicy=true)]
            get
            {
                return this._activationContext;
            }
        }

        public System.ApplicationIdentity ApplicationIdentity
        {
            [SecurityPermission(SecurityAction.LinkDemand, ControlDomainPolicy=true)]
            get
            {
                return this._applicationIdentity;
            }
        }

        public System.Security.Policy.ApplicationTrust ApplicationTrust
        {
            [SecurityPermission(SecurityAction.LinkDemand, ControlDomainPolicy=true)]
            get
            {
                return this._applicationTrust;
            }
        }

        public string BaseDirectory
        {
            get
            {
                return this.FusionStore.ApplicationBase;
            }
        }

        public static AppDomain CurrentDomain
        {
            get
            {
                return Thread.GetDomain();
            }
        }

        public AppDomainManager DomainManager
        {
            [SecurityPermission(SecurityAction.LinkDemand, ControlDomainPolicy=true)]
            get
            {
                return this._domainManager;
            }
        }

        public string DynamicDirectory
        {
            get
            {
                string dynamicDir = this.GetDynamicDir();
                if (dynamicDir != null)
                {
                    new FileIOPermission(FileIOPermissionAccess.PathDiscovery, dynamicDir).Demand();
                }
                return dynamicDir;
            }
        }

        public System.Security.Policy.Evidence Evidence
        {
            [SecurityPermission(SecurityAction.Demand, ControlEvidence=true)]
            get
            {
                if (this._SecurityIdentity == null)
                {
                    if (this.IsDefaultAppDomain())
                    {
                        Assembly entryAssembly = Assembly.GetEntryAssembly();
                        if (entryAssembly != null)
                        {
                            return entryAssembly.Evidence;
                        }
                        return new System.Security.Policy.Evidence();
                    }
                    if (this.nIsDefaultAppDomainForSecurity())
                    {
                        return GetDefaultDomain().Evidence;
                    }
                }
                System.Security.Policy.Evidence internalEvidence = this.InternalEvidence;
                if (internalEvidence != null)
                {
                    return internalEvidence.Copy();
                }
                return internalEvidence;
            }
        }

        public string FriendlyName
        {
            get
            {
                return this.nGetFriendlyName();
            }
        }

        internal AppDomainSetup FusionStore
        {
            get
            {
                return this._FusionStore;
            }
        }

        internal System.Security.HostSecurityManager HostSecurityManager
        {
            get
            {
                System.Security.HostSecurityManager hostSecurityManager = null;
                AppDomainManager domainManager = CurrentDomain.DomainManager;
                if (domainManager != null)
                {
                    hostSecurityManager = domainManager.HostSecurityManager;
                }
                if (hostSecurityManager == null)
                {
                    hostSecurityManager = new System.Security.HostSecurityManager();
                }
                return hostSecurityManager;
            }
        }

        public int Id
        {
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            get
            {
                return this.GetId();
            }
        }

        internal System.Security.Policy.Evidence InternalEvidence
        {
            get
            {
                return this._SecurityIdentity;
            }
        }

        private Hashtable LocalStore
        {
            get
            {
                if (this._LocalStore == null)
                {
                    this._LocalStore = Hashtable.Synchronized(new Hashtable());
                }
                return this._LocalStore;
            }
        }

        public string RelativeSearchPath
        {
            get
            {
                return this.FusionStore.PrivateBinPath;
            }
        }

        internal DomainSpecificRemotingData RemotingData
        {
            get
            {
                if (this._RemotingData == null)
                {
                    this.CreateRemotingData();
                }
                return this._RemotingData;
            }
        }

        public AppDomainSetup SetupInformation
        {
            get
            {
                return new AppDomainSetup(this.FusionStore, true);
            }
        }

        public bool ShadowCopyFiles
        {
            get
            {
                string shadowCopyFiles = this.FusionStore.ShadowCopyFiles;
                return ((shadowCopyFiles != null) && (string.Compare(shadowCopyFiles, "true", StringComparison.OrdinalIgnoreCase) == 0));
            }
        }

        internal class AssemblyBuilderLock
        {
        }

        [Serializable]
        private class EvidenceCollection
        {
            public Evidence CreatorsSecurityInfo;
            public Evidence ProvidedSecurityInfo;
        }
    }
}

