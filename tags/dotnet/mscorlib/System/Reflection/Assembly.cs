namespace System.Reflection
{
    using Microsoft.Win32;
    using System;
    using System.Collections;
    using System.Configuration.Assemblies;
    using System.Globalization;
    using System.IO;
    using System.Reflection.Cache;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Permissions;
    using System.Security.Policy;
    using System.Security.Util;
    using System.Text;
    using System.Threading;

    [Serializable, ClassInterface(ClassInterfaceType.None), ComVisible(true), ComDefaultInterface(typeof(_Assembly))]
    public class Assembly : _Assembly, IEvidenceFactory, ICustomAttributeProvider, ISerializable
    {
        private IntPtr m__assembly;
        internal AssemblyBuilderData m__assemblyData;
        private InternalCache m__cachedData;
        private const string s_localFilePrefix = "file:";

        private event ModuleResolveEventHandler _ModuleResolve;

        public event ModuleResolveEventHandler ModuleResolve
        {
            [SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain=true)] add
            {
                Assembly internalAssembly = this.InternalAssembly;
                internalAssembly._ModuleResolve = (ModuleResolveEventHandler) Delegate.Combine(internalAssembly._ModuleResolve, value);
            }
            [SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain=true)] remove
            {
                Assembly internalAssembly = this.InternalAssembly;
                internalAssembly._ModuleResolve = (ModuleResolveEventHandler) Delegate.Remove(internalAssembly._ModuleResolve, value);
            }
        }

        internal Assembly()
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern Type[] _GetExportedTypes();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern string _GetFullName();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern long _GetHostContext();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern string _GetLocation();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern Module _GetModule(string name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern AssemblyName[] _GetReferencedAssemblies();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern unsafe byte* _GetResource(string resourceName, out ulong length, ref StackCrawlMark stackMark, bool skipSecurityCheck);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern Type _GetType(string name, bool throwOnError, bool ignoreCase);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int _nAddFileToInMemoryFileList(string strFileName, Module module);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void _nAddStandAloneResource(string strName, string strFileName, string strFullFileName, int attribute);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Module _nDefineDynamicModule(Assembly containingAssembly, bool emitSymbolInfo, string filename, ref StackCrawlMark stackMark);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern string _nGetCodeBase(bool fCopiedName);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern unsafe void* _nGetEntryPoint();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern System.Security.Policy.Evidence _nGetEvidence();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern AssemblyNameFlags _nGetFlags();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void _nGetGrantSet(out PermissionSet newGrant, out PermissionSet newDenied);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern AssemblyHashAlgorithm _nGetHashAlgorithm();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern string _nGetImageRuntimeVersion();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern Module _nGetInMemoryAssemblyModule();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern string _nGetLocale();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int _nGetManifestResourceInfo(string resourceName, out Assembly assemblyRef, out string fileName, ref StackCrawlMark stackMark);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern string[] _nGetManifestResourceNames();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern Module[] _nGetModules(bool loadIfNotFound, bool getResourceModules);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern Module _nGetOnDiskAssemblyModule();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern byte[] _nGetPublicKey();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern string _nGetSimpleName();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void _nGetVersion(out int majVer, out int minVer, out int buildNum, out int revNum);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern bool _nGlobalAssemblyCache();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern bool _nIsDynamic();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Assembly _nLoad(AssemblyName fileName, string codeBase, System.Security.Policy.Evidence assemblySecurity, Assembly locationHint, ref StackCrawlMark stackMark, bool throwOnFileNotFound, bool forIntrospection);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern Module _nLoadModule(string moduleName, byte[] rawModule, byte[] rawSymbolStore, System.Security.Policy.Evidence securityEvidence);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void _nPrepareForSavingManifestToDisk(Module assemblyModule);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern bool _nReflection();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int _nSaveExportedType(string strComTypeName, int tkAssemblyRef, int tkTypeDef, TypeAttributes flags);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void _nSaveManifestToDisk(string strFileName, int entryPoint, int fileKind, int portableExecutableKind, int ImageFileMachine);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void _nSavePermissionRequests(byte[] required, byte[] optional, byte[] refused);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int _nSaveToFileList(string strFileName);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int _nSetHashValue(int tkFile, string strFullFileName);
        private static void AddStrongName(System.Security.Policy.Evidence evidence, byte[] blob, string strSimpleName, int major, int minor, int build, int revision, Assembly assembly)
        {
            StrongName id = new StrongName(new StrongNamePublicKeyBlob(blob), strSimpleName, new Version(major, minor, build, revision), assembly);
            evidence.AddHost(id);
        }

        private static void AddX509Certificate(System.Security.Policy.Evidence evidence, byte[] cert)
        {
            evidence.AddHost(new Publisher(new X509Certificate(cert)));
        }

        internal bool AptcaCheck(Assembly sourceAssembly)
        {
            return this.AssemblyHandle.AptcaCheck(sourceAssembly.AssemblyHandle);
        }

        internal ProcessorArchitecture ComputeProcArchIndex()
        {
            Module manifestModule = this.ManifestModule;
            if ((manifestModule != null) && (manifestModule.MDStreamVersion > 0x10000))
            {
                PortableExecutableKinds kinds;
                ImageFileMachine machine;
                this.ManifestModule.GetPEKind(out kinds, out machine);
                if ((kinds & PortableExecutableKinds.PE32Plus) == PortableExecutableKinds.PE32Plus)
                {
                    ImageFileMachine machine2 = machine;
                    if (machine2 == ImageFileMachine.I386)
                    {
                        if ((kinds & PortableExecutableKinds.ILOnly) == PortableExecutableKinds.ILOnly)
                        {
                            return ProcessorArchitecture.MSIL;
                        }
                    }
                    else
                    {
                        if (machine2 == ImageFileMachine.IA64)
                        {
                            return ProcessorArchitecture.IA64;
                        }
                        if (machine2 == ImageFileMachine.AMD64)
                        {
                            return ProcessorArchitecture.Amd64;
                        }
                    }
                }
                else if (machine == ImageFileMachine.I386)
                {
                    if (((kinds & PortableExecutableKinds.Required32Bit) != PortableExecutableKinds.Required32Bit) && ((kinds & PortableExecutableKinds.ILOnly) == PortableExecutableKinds.ILOnly))
                    {
                        return ProcessorArchitecture.MSIL;
                    }
                    return ProcessorArchitecture.X86;
                }
            }
            return ProcessorArchitecture.None;
        }

        public object CreateInstance(string typeName)
        {
            return this.CreateInstance(typeName, false, BindingFlags.Public | BindingFlags.Instance, null, null, null, null);
        }

        public object CreateInstance(string typeName, bool ignoreCase)
        {
            return this.CreateInstance(typeName, ignoreCase, BindingFlags.Public | BindingFlags.Instance, null, null, null, null);
        }

        public object CreateInstance(string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes)
        {
            Type type = this.GetType(typeName, false, ignoreCase);
            if (type == null)
            {
                return null;
            }
            return Activator.CreateInstance(type, bindingAttr, binder, args, culture, activationAttributes);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string CreateQualifiedName(string assemblyName, string typeName);
        private static System.Security.Policy.Evidence CreateSecurityIdentity(Assembly asm, string strUrl, int zone, byte[] cert, byte[] publicKeyBlob, string strSimpleName, int major, int minor, int build, int revision, byte[] serializedEvidence, System.Security.Policy.Evidence additionalEvidence)
        {
            System.Security.Policy.Evidence evidence = new System.Security.Policy.Evidence();
            if (zone != -1)
            {
                evidence.AddHost(new Zone((SecurityZone) zone));
            }
            if (strUrl != null)
            {
                evidence.AddHost(new Url(strUrl, true));
                if (string.Compare(strUrl, 0, "file:", 0, 5, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    evidence.AddHost(Site.CreateFromUrl(strUrl));
                }
            }
            if (cert != null)
            {
                AddX509Certificate(evidence, cert);
            }
            if ((asm != null) && RuntimeEnvironment.FromGlobalAccessCache(asm))
            {
                evidence.AddHost(new GacInstalled());
            }
            if (serializedEvidence != null)
            {
                DecodeSerializedEvidence(evidence, serializedEvidence);
            }
            if ((publicKeyBlob != null) && (publicKeyBlob.Length != 0))
            {
                AddStrongName(evidence, publicKeyBlob, strSimpleName, major, minor, build, revision, asm);
            }
            if ((asm != null) && !asm.nIsDynamic())
            {
                evidence.AddHost(new Hash(asm));
            }
            if (additionalEvidence != null)
            {
                evidence.MergeWithNoDuplicates(additionalEvidence);
            }
            if (asm != null)
            {
                HostSecurityManager hostSecurityManager = AppDomain.CurrentDomain.HostSecurityManager;
                if ((hostSecurityManager.Flags & HostSecurityManagerOptions.HostAssemblyEvidence) == HostSecurityManagerOptions.HostAssemblyEvidence)
                {
                    return hostSecurityManager.ProvideAssemblyEvidence(asm, evidence);
                }
            }
            return evidence;
        }

        private static IPermission CreateWebPermission(string codeBase)
        {
            Assembly assembly = Load("System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            Type enumType = assembly.GetType("System.Net.NetworkAccess", true);
            IPermission permission = null;
            if (enumType.IsEnum && enumType.IsVisible)
            {
                object[] args = new object[2];
                args[0] = (Enum) Enum.Parse(enumType, "Connect", true);
                if (args[0] != null)
                {
                    args[1] = codeBase;
                    enumType = assembly.GetType("System.Net.WebPermission", true);
                    if (enumType.IsVisible)
                    {
                        permission = (IPermission) Activator.CreateInstance(enumType, args);
                    }
                }
            }
            if (permission == null)
            {
                throw new ExecutionEngineException();
            }
            return permission;
        }

        private static bool CulturesEqual(CultureInfo refCI, CultureInfo defCI)
        {
            bool flag = defCI.Equals(CultureInfo.InvariantCulture);
            if ((refCI == null) || refCI.Equals(CultureInfo.InvariantCulture))
            {
                return flag;
            }
            return (!flag && defCI.Equals(refCI));
        }

        private static void DecodeSerializedEvidence(System.Security.Policy.Evidence evidence, byte[] serializedEvidence)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            System.Security.Policy.Evidence evidence2 = null;
            PermissionSet set = new PermissionSet(false);
            set.SetPermission(new SecurityPermission(SecurityPermissionFlag.SerializationFormatter));
            set.PermitOnly();
            set.Assert();
            try
            {
                using (MemoryStream stream = new MemoryStream(serializedEvidence))
                {
                    evidence2 = (System.Security.Policy.Evidence) formatter.Deserialize(stream);
                }
            }
            catch
            {
            }
            if (evidence2 != null)
            {
                IEnumerator assemblyEnumerator = evidence2.GetAssemblyEnumerator();
                while (assemblyEnumerator.MoveNext())
                {
                    object current = assemblyEnumerator.Current;
                    evidence.AddAssembly(current);
                }
            }
        }

        private static void DemandPermission(string codeBase, bool havePath, int demandFlag)
        {
            FileIOPermissionAccess pathDiscovery = FileIOPermissionAccess.PathDiscovery;
            switch (demandFlag)
            {
                case 1:
                    pathDiscovery = FileIOPermissionAccess.Read;
                    break;

                case 2:
                    pathDiscovery = FileIOPermissionAccess.PathDiscovery | FileIOPermissionAccess.Read;
                    break;

                case 3:
                    CreateWebPermission(AssemblyName.EscapeCodeBase(codeBase)).Demand();
                    return;
            }
            if (!havePath)
            {
                codeBase = new URLString(codeBase, true).GetFileName();
            }
            codeBase = Path.GetFullPathInternal(codeBase);
            new FileIOPermission(pathDiscovery, codeBase).Demand();
        }

        private static AssemblyName EnumerateCache(AssemblyName partialName)
        {
            new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Assert();
            partialName.Version = null;
            ArrayList alAssems = new ArrayList();
            Fusion.ReadCache(alAssems, partialName.FullName, 2);
            IEnumerator enumerator = alAssems.GetEnumerator();
            AssemblyName name = null;
            CultureInfo cultureInfo = partialName.CultureInfo;
            while (enumerator.MoveNext())
            {
                AssemblyName name2 = new AssemblyName((string) enumerator.Current);
                if (CulturesEqual(cultureInfo, name2.CultureInfo))
                {
                    if (name == null)
                    {
                        name = name2;
                    }
                    else if (name2.Version > name.Version)
                    {
                        name = name2;
                    }
                }
            }
            return name;
        }

        public override bool Equals(object o)
        {
            if (o == null)
            {
                return false;
            }
            if (!(o is Assembly))
            {
                return false;
            }
            Assembly internalAssembly = o as Assembly;
            internalAssembly = internalAssembly.InternalAssembly;
            return (this.InternalAssembly == internalAssembly);
        }

        public static Assembly GetAssembly(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            Module module = type.Module;
            if (module == null)
            {
                return null;
            }
            return module.Assembly;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Assembly GetCallingAssembly()
        {
            StackCrawlMark lookForMyCallersCaller = StackCrawlMark.LookForMyCallersCaller;
            return nGetExecutingAssembly(ref lookForMyCallersCaller);
        }

        public virtual object[] GetCustomAttributes(bool inherit)
        {
            return CustomAttribute.GetCustomAttributes(this, typeof(object) as RuntimeType);
        }

        public virtual object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            if (attributeType == null)
            {
                throw new ArgumentNullException("attributeType");
            }
            RuntimeType underlyingSystemType = attributeType.UnderlyingSystemType as RuntimeType;
            if (underlyingSystemType == null)
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_MustBeType"), "attributeType");
            }
            return CustomAttribute.GetCustomAttributes(this, underlyingSystemType);
        }

        public static Assembly GetEntryAssembly()
        {
            AppDomainManager domainManager = AppDomain.CurrentDomain.DomainManager;
            if (domainManager == null)
            {
                domainManager = new AppDomainManager();
            }
            return domainManager.EntryAssembly;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Assembly GetExecutingAssembly()
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return nGetExecutingAssembly(ref lookForMyCaller);
        }

        public virtual Type[] GetExportedTypes()
        {
            return this.InternalAssembly._GetExportedTypes();
        }

        public virtual FileStream GetFile(string name)
        {
            Module module = this.GetModule(name);
            if (module == null)
            {
                return null;
            }
            return new FileStream(module.InternalGetFullyQualifiedName(), FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public virtual FileStream[] GetFiles()
        {
            return this.GetFiles(false);
        }

        public virtual FileStream[] GetFiles(bool getResourceModules)
        {
            Module[] moduleArray = this.nGetModules(true, getResourceModules);
            int length = moduleArray.Length;
            FileStream[] streamArray = new FileStream[length];
            for (int i = 0; i < length; i++)
            {
                streamArray[i] = new FileStream(moduleArray[i].InternalGetFullyQualifiedName(), FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            return streamArray;
        }

        internal string GetFullName()
        {
            return this.InternalAssembly._GetFullName();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private long GetHostContext()
        {
            return this.InternalAssembly._GetHostContext();
        }

        public Module[] GetLoadedModules()
        {
            return this.nGetModules(false, false);
        }

        public Module[] GetLoadedModules(bool getResourceModules)
        {
            return this.nGetModules(false, getResourceModules);
        }

        internal CultureInfo GetLocale()
        {
            string name = this.nGetLocale();
            if (name == null)
            {
                return CultureInfo.InvariantCulture;
            }
            return new CultureInfo(name);
        }

        internal string GetLocation()
        {
            return this.InternalAssembly._GetLocation();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public virtual ManifestResourceInfo GetManifestResourceInfo(string resourceName)
        {
            Assembly assembly;
            string str;
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            int num = this.nGetManifestResourceInfo(resourceName, out assembly, out str, ref lookForMyCaller);
            if (num == -1)
            {
                return null;
            }
            return new ManifestResourceInfo(assembly, str, (ResourceLocation) num);
        }

        public virtual string[] GetManifestResourceNames()
        {
            return this.nGetManifestResourceNames();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public virtual Stream GetManifestResourceStream(string name)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return this.GetManifestResourceStream(name, ref lookForMyCaller, false);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public virtual Stream GetManifestResourceStream(Type type, string name)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return this.GetManifestResourceStream(type, name, false, ref lookForMyCaller);
        }

        internal virtual unsafe Stream GetManifestResourceStream(string name, ref StackCrawlMark stackMark, bool skipSecurityCheck)
        {
            ulong length = 0L;
            byte* pointer = this.GetResource(name, out length, ref stackMark, skipSecurityCheck);
            if (pointer == null)
            {
                return null;
            }
            if (length > 0x7fffffffffffffffL)
            {
                throw new NotImplementedException(Environment.GetResourceString("NotImplemented_ResourcesLongerThan2^63"));
            }
            return new UnmanagedMemoryStream(pointer, (long) length, (long) length, FileAccess.Read, true);
        }

        internal virtual Stream GetManifestResourceStream(Type type, string name, bool skipSecurityCheck, ref StackCrawlMark stackMark)
        {
            StringBuilder builder = new StringBuilder();
            if (type == null)
            {
                if (name == null)
                {
                    throw new ArgumentNullException("type");
                }
            }
            else
            {
                string str = type.Namespace;
                if (str != null)
                {
                    builder.Append(str);
                    if (name != null)
                    {
                        builder.Append(Type.Delimiter);
                    }
                }
            }
            if (name != null)
            {
                builder.Append(name);
            }
            return this.GetManifestResourceStream(builder.ToString(), ref stackMark, skipSecurityCheck);
        }

        public Module GetModule(string name)
        {
            return this.GetModuleInternal(name);
        }

        internal virtual Module GetModuleInternal(string name)
        {
            return this.InternalAssembly._GetModule(name);
        }

        public Module[] GetModules()
        {
            return this.nGetModules(true, false);
        }

        public Module[] GetModules(bool getResourceModules)
        {
            return this.nGetModules(true, getResourceModules);
        }

        public virtual AssemblyName GetName()
        {
            return this.GetName(false);
        }

        public virtual AssemblyName GetName(bool copiedName)
        {
            AssemblyName name = new AssemblyName();
            string codeBase = this.nGetCodeBase(copiedName);
            this.VerifyCodeBaseDiscovery(codeBase);
            name.Init(this.nGetSimpleName(), this.nGetPublicKey(), null, this.GetVersion(), this.GetLocale(), this.nGetHashAlgorithm(), AssemblyVersionCompatibility.SameMachine, codeBase, this.nGetFlags() | AssemblyNameFlags.PublicKey, null);
            name.ProcessorArchitecture = this.ComputeProcArchIndex();
            return name;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            UnitySerializationHolder.GetUnitySerializationInfo(info, 6, this.FullName, this);
        }

        internal PermissionSet GetPermissionSet()
        {
            PermissionSet set;
            PermissionSet set2;
            this.nGetGrantSet(out set, out set2);
            if (set == null)
            {
                set = new PermissionSet(PermissionState.Unrestricted);
            }
            return set;
        }

        public AssemblyName[] GetReferencedAssemblies()
        {
            return this.InternalAssembly._GetReferencedAssemblies();
        }

        private unsafe byte* GetResource(string resourceName, out ulong length, ref StackCrawlMark stackMark, bool skipSecurityCheck)
        {
            return this.InternalAssembly._GetResource(resourceName, out length, ref stackMark, skipSecurityCheck);
        }

        public Assembly GetSatelliteAssembly(CultureInfo culture)
        {
            return this.InternalGetSatelliteAssembly(culture, null, true);
        }

        public Assembly GetSatelliteAssembly(CultureInfo culture, Version version)
        {
            return this.InternalGetSatelliteAssembly(culture, version, true);
        }

        public virtual Type GetType(string name)
        {
            return this.GetType(name, false, false);
        }

        public virtual Type GetType(string name, bool throwOnError)
        {
            return this.GetType(name, throwOnError, false);
        }

        public Type GetType(string name, bool throwOnError, bool ignoreCase)
        {
            return this.InternalAssembly._GetType(name, throwOnError, ignoreCase);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public virtual Type[] GetTypes()
        {
            Module[] moduleArray = this.nGetModules(true, false);
            int length = moduleArray.Length;
            int num2 = 0;
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            Type[][] typeArray = new Type[length][];
            for (int i = 0; i < length; i++)
            {
                typeArray[i] = moduleArray[i].GetTypesInternal(ref lookForMyCaller);
                num2 += typeArray[i].Length;
            }
            int destinationIndex = 0;
            Type[] destinationArray = new Type[num2];
            for (int j = 0; j < length; j++)
            {
                int num6 = typeArray[j].Length;
                Array.Copy(typeArray[j], 0, destinationArray, destinationIndex, num6);
                destinationIndex += num6;
            }
            return destinationArray;
        }

        internal Version GetVersion()
        {
            int num;
            int num2;
            int num3;
            int num4;
            this.nGetVersion(out num, out num2, out num3, out num4);
            return new Version(num, num2, num3, num4);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal Assembly InternalGetSatelliteAssembly(CultureInfo culture, Version version, bool throwOnFileNotFound)
        {
            if (culture == null)
            {
                throw new ArgumentNullException("culture");
            }
            AssemblyName fileName = new AssemblyName();
            fileName.SetPublicKey(this.nGetPublicKey());
            fileName.Flags = this.nGetFlags() | AssemblyNameFlags.PublicKey;
            if (version == null)
            {
                fileName.Version = this.GetVersion();
            }
            else
            {
                fileName.Version = version;
            }
            fileName.CultureInfo = culture;
            fileName.Name = this.nGetSimpleName() + ".resources";
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            Assembly assembly = nLoad(fileName, null, null, this, ref lookForMyCaller, throwOnFileNotFound, false);
            if (assembly == this)
            {
                throw new FileNotFoundException(string.Format(culture, Environment.GetResourceString("IO.FileNotFound_FileName"), new object[] { fileName.Name }));
            }
            return assembly;
        }

        internal static Assembly InternalLoad(AssemblyName assemblyRef, System.Security.Policy.Evidence assemblySecurity, ref StackCrawlMark stackMark, bool forIntrospection)
        {
            if (assemblyRef == null)
            {
                throw new ArgumentNullException("assemblyRef");
            }
            assemblyRef = (AssemblyName) assemblyRef.Clone();
            if (assemblySecurity != null)
            {
                new SecurityPermission(SecurityPermissionFlag.ControlEvidence).Demand();
            }
            string strA = VerifyCodeBase(assemblyRef.CodeBase);
            if (strA != null)
            {
                if (string.Compare(strA, 0, "file:", 0, 5, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    CreateWebPermission(assemblyRef.EscapedCodeBase).Demand();
                }
                else
                {
                    URLString str2 = new URLString(strA, true);
                    new FileIOPermission(FileIOPermissionAccess.PathDiscovery | FileIOPermissionAccess.Read, str2.GetFileName()).Demand();
                }
            }
            return nLoad(assemblyRef, strA, assemblySecurity, null, ref stackMark, true, forIntrospection);
        }

        internal static Assembly InternalLoad(string assemblyString, System.Security.Policy.Evidence assemblySecurity, ref StackCrawlMark stackMark, bool forIntrospection)
        {
            if (assemblyString == null)
            {
                throw new ArgumentNullException("assemblyString");
            }
            if ((assemblyString.Length == 0) || (assemblyString[0] == '\0'))
            {
                throw new ArgumentException(Environment.GetResourceString("Format_StringZeroLength"));
            }
            AssemblyName assemblyRef = new AssemblyName();
            Assembly assembly = null;
            assemblyRef.Name = assemblyString;
            if (assemblyRef.nInit(out assembly, forIntrospection, true) == -2146234297)
            {
                return assembly;
            }
            return InternalLoad(assemblyRef, assemblySecurity, ref stackMark, forIntrospection);
        }

        private static Assembly InternalLoadFrom(string assemblyFile, System.Security.Policy.Evidence securityEvidence, byte[] hashValue, AssemblyHashAlgorithm hashAlgorithm, bool forIntrospection, ref StackCrawlMark stackMark)
        {
            if (assemblyFile == null)
            {
                throw new ArgumentNullException("assemblyFile");
            }
            AssemblyName assemblyRef = new AssemblyName {
                CodeBase = assemblyFile
            };
            assemblyRef.SetHashControl(hashValue, hashAlgorithm);
            return InternalLoad(assemblyRef, securityEvidence, ref stackMark, forIntrospection);
        }

        [FileIOPermission(SecurityAction.Assert, Unrestricted=true)]
        private bool IsAssemblyUnderAppBase()
        {
            string location = this.GetLocation();
            if (string.IsNullOrEmpty(location))
            {
                return true;
            }
            FileIOAccess access = new FileIOAccess(Path.GetFullPathInternal(location));
            FileIOAccess operand = new FileIOAccess(Path.GetFullPathInternal(AppDomain.CurrentDomain.BaseDirectory));
            return access.IsSubsetOf(operand);
        }

        public virtual bool IsDefined(Type attributeType, bool inherit)
        {
            if (attributeType == null)
            {
                throw new ArgumentNullException("attributeType");
            }
            RuntimeType underlyingSystemType = attributeType.UnderlyingSystemType as RuntimeType;
            if (underlyingSystemType == null)
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_MustBeType"), "caType");
            }
            return CustomAttribute.IsDefined(this, underlyingSystemType);
        }

        private static bool IsSimplyNamed(AssemblyName partialName)
        {
            byte[] publicKeyToken = partialName.GetPublicKeyToken();
            if ((publicKeyToken != null) && (publicKeyToken.Length == 0))
            {
                return true;
            }
            publicKeyToken = partialName.GetPublicKey();
            return ((publicKeyToken != null) && (publicKeyToken.Length == 0));
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern bool IsStrongNameVerified();
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Assembly Load(AssemblyName assemblyRef)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return InternalLoad(assemblyRef, null, ref lookForMyCaller, false);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Assembly Load(string assemblyString)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return InternalLoad(assemblyString, null, ref lookForMyCaller, false);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Assembly Load(byte[] rawAssembly)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return nLoadImage(rawAssembly, null, null, ref lookForMyCaller, false);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Assembly Load(AssemblyName assemblyRef, System.Security.Policy.Evidence assemblySecurity)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return InternalLoad(assemblyRef, assemblySecurity, ref lookForMyCaller, false);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Assembly Load(string assemblyString, System.Security.Policy.Evidence assemblySecurity)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return InternalLoad(assemblyString, assemblySecurity, ref lookForMyCaller, false);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Assembly Load(byte[] rawAssembly, byte[] rawSymbolStore)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return nLoadImage(rawAssembly, rawSymbolStore, null, ref lookForMyCaller, false);
        }

        [MethodImpl(MethodImplOptions.NoInlining), SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.ControlEvidence)]
        public static Assembly Load(byte[] rawAssembly, byte[] rawSymbolStore, System.Security.Policy.Evidence securityEvidence)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return nLoadImage(rawAssembly, rawSymbolStore, securityEvidence, ref lookForMyCaller, false);
        }

        public static Assembly LoadFile(string path)
        {
            new FileIOPermission(FileIOPermissionAccess.PathDiscovery | FileIOPermissionAccess.Read, path).Demand();
            return nLoadFile(path, null);
        }

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.ControlEvidence)]
        public static Assembly LoadFile(string path, System.Security.Policy.Evidence securityEvidence)
        {
            new FileIOPermission(FileIOPermissionAccess.PathDiscovery | FileIOPermissionAccess.Read, path).Demand();
            return nLoadFile(path, securityEvidence);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Assembly LoadFrom(string assemblyFile)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return InternalLoadFrom(assemblyFile, null, null, AssemblyHashAlgorithm.None, false, ref lookForMyCaller);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Assembly LoadFrom(string assemblyFile, System.Security.Policy.Evidence securityEvidence)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return InternalLoadFrom(assemblyFile, securityEvidence, null, AssemblyHashAlgorithm.None, false, ref lookForMyCaller);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Assembly LoadFrom(string assemblyFile, System.Security.Policy.Evidence securityEvidence, byte[] hashValue, AssemblyHashAlgorithm hashAlgorithm)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return InternalLoadFrom(assemblyFile, securityEvidence, hashValue, hashAlgorithm, false, ref lookForMyCaller);
        }

        public Module LoadModule(string moduleName, byte[] rawModule)
        {
            return this.nLoadModule(moduleName, rawModule, null, this.Evidence);
        }

        public Module LoadModule(string moduleName, byte[] rawModule, byte[] rawSymbolStore)
        {
            return this.nLoadModule(moduleName, rawModule, rawSymbolStore, this.Evidence);
        }

        [MethodImpl(MethodImplOptions.NoInlining), Obsolete("This method has been deprecated. Please use Assembly.Load() instead. http://go.microsoft.com/fwlink/?linkid=14202")]
        public static Assembly LoadWithPartialName(string partialName)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return LoadWithPartialNameInternal(partialName, null, ref lookForMyCaller);
        }

        [MethodImpl(MethodImplOptions.NoInlining), Obsolete("This method has been deprecated. Please use Assembly.Load() instead. http://go.microsoft.com/fwlink/?linkid=14202")]
        public static Assembly LoadWithPartialName(string partialName, System.Security.Policy.Evidence securityEvidence)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return LoadWithPartialNameInternal(partialName, securityEvidence, ref lookForMyCaller);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static unsafe IntPtr LoadWithPartialNameHack(string partialName, bool cropPublicKey)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            Assembly assembly = null;
            AssemblyName name = new AssemblyName(partialName);
            if (!IsSimplyNamed(name))
            {
                if (cropPublicKey)
                {
                    name.SetPublicKey(null);
                    name.SetPublicKeyToken(null);
                }
                AssemblyName assemblyRef = EnumerateCache(name);
                if (assemblyRef != null)
                {
                    assembly = InternalLoad(assemblyRef, null, ref lookForMyCaller, false);
                }
            }
            if (assembly == null)
            {
                return IntPtr.Zero;
            }
            return (IntPtr) assembly.AssemblyHandle.Value;
        }

        internal static Assembly LoadWithPartialNameInternal(string partialName, System.Security.Policy.Evidence securityEvidence, ref StackCrawlMark stackMark)
        {
            if (securityEvidence != null)
            {
                new SecurityPermission(SecurityPermissionFlag.ControlEvidence).Demand();
            }
            Assembly assembly = null;
            AssemblyName fileName = new AssemblyName(partialName);
            try
            {
                assembly = nLoad(fileName, null, securityEvidence, null, ref stackMark, true, false);
            }
            catch (Exception exception)
            {
                if (exception.IsTransient)
                {
                    throw exception;
                }
                if (IsSimplyNamed(fileName))
                {
                    return null;
                }
                AssemblyName assemblyRef = EnumerateCache(fileName);
                if (assemblyRef != null)
                {
                    return InternalLoad(assemblyRef, securityEvidence, ref stackMark, false);
                }
            }
            return assembly;
        }

        internal int nAddFileToInMemoryFileList(string strFileName, Module module)
        {
            if (module != null)
            {
                module = module.InternalModule;
            }
            return this.InternalAssembly._nAddFileToInMemoryFileList(strFileName, module);
        }

        internal void nAddStandAloneResource(string strName, string strFileName, string strFullFileName, int attribute)
        {
            this.InternalAssembly._nAddStandAloneResource(strName, strFileName, strFullFileName, attribute);
        }

        internal static Module nDefineDynamicModule(Assembly containingAssembly, bool emitSymbolInfo, string filename, ref StackCrawlMark stackMark)
        {
            return _nDefineDynamicModule(containingAssembly.InternalAssembly, emitSymbolInfo, filename, ref stackMark);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern string nDefineVersionInfoResource(string filename, string title, string iconFilename, string description, string copyright, string trademark, string company, string product, string productVersion, string fileVersion, int lcid, bool isDll);
        internal string nGetCodeBase(bool fCopiedName)
        {
            return this.InternalAssembly._nGetCodeBase(fCopiedName);
        }

        private RuntimeMethodHandle nGetEntryPoint()
        {
            return new RuntimeMethodHandle(this._nGetEntryPoint());
        }

        internal System.Security.Policy.Evidence nGetEvidence()
        {
            return this.InternalAssembly._nGetEvidence();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern Assembly nGetExecutingAssembly(ref StackCrawlMark stackMark);
        internal AssemblyNameFlags nGetFlags()
        {
            return this.InternalAssembly._nGetFlags();
        }

        internal void nGetGrantSet(out PermissionSet newGrant, out PermissionSet newDenied)
        {
            this.InternalAssembly._nGetGrantSet(out newGrant, out newDenied);
        }

        internal AssemblyHashAlgorithm nGetHashAlgorithm()
        {
            return this.InternalAssembly._nGetHashAlgorithm();
        }

        private string nGetImageRuntimeVersion()
        {
            return this.InternalAssembly._nGetImageRuntimeVersion();
        }

        internal Module nGetInMemoryAssemblyModule()
        {
            return this.InternalAssembly._nGetInMemoryAssemblyModule();
        }

        private string nGetLocale()
        {
            return this.InternalAssembly._nGetLocale();
        }

        private int nGetManifestResourceInfo(string resourceName, out Assembly assemblyRef, out string fileName, ref StackCrawlMark stackMark)
        {
            return this.InternalAssembly._nGetManifestResourceInfo(resourceName, out assemblyRef, out fileName, ref stackMark);
        }

        internal string[] nGetManifestResourceNames()
        {
            return this.InternalAssembly._nGetManifestResourceNames();
        }

        internal virtual Module[] nGetModules(bool loadIfNotFound, bool getResourceModules)
        {
            return this.InternalAssembly._nGetModules(loadIfNotFound, getResourceModules);
        }

        internal Module nGetOnDiskAssemblyModule()
        {
            return this.InternalAssembly._nGetOnDiskAssemblyModule();
        }

        internal byte[] nGetPublicKey()
        {
            return this.InternalAssembly._nGetPublicKey();
        }

        internal string nGetSimpleName()
        {
            return this.InternalAssembly._nGetSimpleName();
        }

        internal void nGetVersion(out int majVer, out int minVer, out int buildNum, out int revNum)
        {
            this.InternalAssembly._nGetVersion(out majVer, out minVer, out buildNum, out revNum);
        }

        private bool nGlobalAssemblyCache()
        {
            return this.InternalAssembly._nGlobalAssemblyCache();
        }

        internal bool nIsDynamic()
        {
            return this.InternalAssembly._nIsDynamic();
        }

        private static Assembly nLoad(AssemblyName fileName, string codeBase, System.Security.Policy.Evidence assemblySecurity, Assembly locationHint, ref StackCrawlMark stackMark, bool throwOnFileNotFound, bool forIntrospection)
        {
            if (locationHint != null)
            {
                locationHint = locationHint.InternalAssembly;
            }
            return _nLoad(fileName, codeBase, assemblySecurity, locationHint, ref stackMark, throwOnFileNotFound, forIntrospection);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern Assembly nLoadFile(string path, System.Security.Policy.Evidence evidence);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern Assembly nLoadImage(byte[] rawAssembly, byte[] rawSymbolStore, System.Security.Policy.Evidence evidence, ref StackCrawlMark stackMark, bool fIntrospection);
        private Module nLoadModule(string moduleName, byte[] rawModule, byte[] rawSymbolStore, System.Security.Policy.Evidence securityEvidence)
        {
            return this.InternalAssembly._nLoadModule(moduleName, rawModule, rawSymbolStore, securityEvidence);
        }

        internal void nPrepareForSavingManifestToDisk(Module assemblyModule)
        {
            if (assemblyModule != null)
            {
                assemblyModule = assemblyModule.InternalModule;
            }
            this.InternalAssembly._nPrepareForSavingManifestToDisk(assemblyModule);
        }

        internal bool nReflection()
        {
            return this.InternalAssembly._nReflection();
        }

        internal int nSaveExportedType(string strComTypeName, int tkAssemblyRef, int tkTypeDef, TypeAttributes flags)
        {
            return this.InternalAssembly._nSaveExportedType(strComTypeName, tkAssemblyRef, tkTypeDef, flags);
        }

        internal void nSaveManifestToDisk(string strFileName, int entryPoint, int fileKind, int portableExecutableKind, int ImageFileMachine)
        {
            this.InternalAssembly._nSaveManifestToDisk(strFileName, entryPoint, fileKind, portableExecutableKind, ImageFileMachine);
        }

        internal void nSavePermissionRequests(byte[] required, byte[] optional, byte[] refused)
        {
            this.InternalAssembly._nSavePermissionRequests(required, optional, refused);
        }

        internal int nSaveToFileList(string strFileName)
        {
            return this.InternalAssembly._nSaveToFileList(strFileName);
        }

        internal int nSetHashValue(int tkFile, string strFullFileName)
        {
            return this.InternalAssembly._nSetHashValue(tkFile, strFullFileName);
        }

        internal void OnCacheClear(object sender, ClearCacheEventArgs cacheEventArgs)
        {
            this.m_cachedData = null;
        }

        private Module OnModuleResolveEvent(string moduleName)
        {
            ModuleResolveEventHandler moduleResolveEvent = this.ModuleResolveEvent;
            if (moduleResolveEvent != null)
            {
                Delegate[] invocationList = moduleResolveEvent.GetInvocationList();
                int length = invocationList.Length;
                for (int i = 0; i < length; i++)
                {
                    Module module = ((ModuleResolveEventHandler) invocationList[i])(this, new ResolveEventArgs(moduleName));
                    if (module != null)
                    {
                        return module.InternalModule;
                    }
                }
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Assembly ReflectionOnlyLoad(string assemblyString)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return InternalLoad(assemblyString, null, ref lookForMyCaller, true);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Assembly ReflectionOnlyLoad(byte[] rawAssembly)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return nLoadImage(rawAssembly, null, null, ref lookForMyCaller, true);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Assembly ReflectionOnlyLoadFrom(string assemblyFile)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return InternalLoadFrom(assemblyFile, null, null, AssemblyHashAlgorithm.None, true, ref lookForMyCaller);
        }

        Type _Assembly.GetType()
        {
            return base.GetType();
        }

        public override string ToString()
        {
            string fullName = this.FullName;
            if (fullName == null)
            {
                return base.ToString();
            }
            return fullName;
        }

        internal static string VerifyCodeBase(string codebase)
        {
            if (codebase == null)
            {
                return null;
            }
            int length = codebase.Length;
            if (length == 0)
            {
                return null;
            }
            int index = codebase.IndexOf(':');
            if ((((index != -1) && ((index + 2) < length)) && ((codebase[index + 1] == '/') || (codebase[index + 1] == '\\'))) && ((codebase[index + 2] == '/') || (codebase[index + 2] == '\\')))
            {
                return codebase;
            }
            if (((length > 2) && (codebase[0] == '\\')) && (codebase[1] == '\\'))
            {
                return ("file://" + codebase);
            }
            return ("file:///" + Path.GetFullPathInternal(codebase));
        }

        private void VerifyCodeBaseDiscovery(string codeBase)
        {
            if ((codeBase != null) && (string.Compare(codeBase, 0, "file:", 0, 5, StringComparison.OrdinalIgnoreCase) == 0))
            {
                URLString str = new URLString(codeBase, true);
                new FileIOPermission(FileIOPermissionAccess.PathDiscovery, str.GetFileName()).Demand();
            }
        }

        internal System.AssemblyHandle AssemblyHandle
        {
            get
            {
                return new System.AssemblyHandle((void*) this.m_assembly);
            }
        }

        internal InternalCache Cache
        {
            get
            {
                InternalCache cachedData = this.m_cachedData;
                if (cachedData == null)
                {
                    cachedData = new InternalCache("Assembly");
                    this.m_cachedData = cachedData;
                    GC.ClearCache += new ClearCacheHandler(this.OnCacheClear);
                }
                return cachedData;
            }
        }

        public virtual string CodeBase
        {
            get
            {
                string codeBase = this.nGetCodeBase(false);
                this.VerifyCodeBaseDiscovery(codeBase);
                return codeBase;
            }
        }

        public virtual MethodInfo EntryPoint
        {
            get
            {
                RuntimeMethodHandle methodHandle = this.nGetEntryPoint();
                if (!methodHandle.IsNullHandle())
                {
                    return (MethodInfo) RuntimeType.GetMethodBase(methodHandle);
                }
                return null;
            }
        }

        public virtual string EscapedCodeBase
        {
            get
            {
                return AssemblyName.EscapeCodeBase(this.CodeBase);
            }
        }

        public virtual System.Security.Policy.Evidence Evidence
        {
            [SecurityPermission(SecurityAction.Demand, ControlEvidence=true)]
            get
            {
                return this.nGetEvidence().Copy();
            }
        }

        public virtual string FullName
        {
            get
            {
                string fullName = (string) this.Cache[CacheObjType.AssemblyName];
                if (fullName == null)
                {
                    fullName = this.GetFullName();
                    if (fullName != null)
                    {
                        this.Cache[CacheObjType.AssemblyName] = fullName;
                    }
                }
                return fullName;
            }
        }

        public bool GlobalAssemblyCache
        {
            get
            {
                return this.nGlobalAssemblyCache();
            }
        }

        [ComVisible(false)]
        public long HostContext
        {
            get
            {
                return this.GetHostContext();
            }
        }

        [ComVisible(false)]
        public virtual string ImageRuntimeVersion
        {
            get
            {
                return this.nGetImageRuntimeVersion();
            }
        }

        internal virtual Assembly InternalAssembly
        {
            get
            {
                return this;
            }
        }

        public virtual string Location
        {
            get
            {
                string location = this.GetLocation();
                if (location != null)
                {
                    new FileIOPermission(FileIOPermissionAccess.PathDiscovery, location).Demand();
                }
                return location;
            }
        }

        internal IntPtr m_assembly
        {
            get
            {
                return this.InternalAssembly.m__assembly;
            }
            set
            {
                this.InternalAssembly.m__assembly = value;
            }
        }

        internal AssemblyBuilderData m_assemblyData
        {
            get
            {
                return this.InternalAssembly.m__assemblyData;
            }
            set
            {
                this.InternalAssembly.m__assemblyData = value;
            }
        }

        internal InternalCache m_cachedData
        {
            get
            {
                return this.InternalAssembly.m__cachedData;
            }
            set
            {
                this.InternalAssembly.m__cachedData = value;
            }
        }

        [ComVisible(false)]
        public Module ManifestModule
        {
            get
            {
                ModuleHandle manifestModule = this.AssemblyHandle.GetManifestModule();
                if (manifestModule.IsNullHandle())
                {
                    return null;
                }
                return manifestModule.GetModule();
            }
        }

        private ModuleResolveEventHandler ModuleResolveEvent
        {
            get
            {
                return this.InternalAssembly._ModuleResolve;
            }
        }

        [ComVisible(false)]
        public virtual bool ReflectionOnly
        {
            get
            {
                return this.nReflection();
            }
        }
    }
}

