namespace System
{
    using System.Configuration.Assemblies;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Activation;
    using System.Security.Permissions;
    using System.Security.Policy;
    using System.Threading;

    [ComVisible(true), ComDefaultInterface(typeof(_Activator)), ClassInterface(ClassInterfaceType.None)]
    public sealed class Activator : _Activator
    {
        internal const BindingFlags ConLookup = (BindingFlags.Public | BindingFlags.Instance);
        internal const BindingFlags ConstructorDefault = (BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance);
        internal const int LookupMask = 0xff;

        private Activator()
        {
        }

        public static ObjectHandle CreateComInstanceFrom(string assemblyName, string typeName)
        {
            return CreateComInstanceFrom(assemblyName, typeName, null, AssemblyHashAlgorithm.None);
        }

        public static ObjectHandle CreateComInstanceFrom(string assemblyName, string typeName, byte[] hashValue, AssemblyHashAlgorithm hashAlgorithm)
        {
            Assembly assembly = Assembly.LoadFrom(assemblyName, null, hashValue, hashAlgorithm);
            Type type = assembly.GetType(typeName, true, false);
            object[] customAttributes = type.GetCustomAttributes(typeof(ComVisibleAttribute), false);
            if ((customAttributes.Length > 0) && !((ComVisibleAttribute) customAttributes[0]).Value)
            {
                throw new TypeLoadException(Environment.GetResourceString("Argument_TypeMustBeVisibleFromCom"));
            }
            if (assembly == null)
            {
                return null;
            }
            object o = CreateInstance(type, BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance, null, null, null, null);
            if (o == null)
            {
                return null;
            }
            return new ObjectHandle(o);
        }

        public static T CreateInstance<T>()
        {
            bool bNeedSecurityCheck = true;
            bool canBeCached = false;
            RuntimeMethodHandle emptyHandle = RuntimeMethodHandle.EmptyHandle;
            return (T) RuntimeTypeHandle.CreateInstance(typeof(T) as RuntimeType, true, true, ref canBeCached, ref emptyHandle, ref bNeedSecurityCheck);
        }

        public static ObjectHandle CreateInstance(ActivationContext activationContext)
        {
            AppDomainManager domainManager = AppDomain.CurrentDomain.DomainManager;
            if (domainManager == null)
            {
                domainManager = new AppDomainManager();
            }
            return domainManager.ApplicationActivator.CreateInstance(activationContext);
        }

        public static object CreateInstance(Type type)
        {
            return CreateInstance(type, false);
        }

        public static ObjectHandle CreateInstance(ActivationContext activationContext, string[] activationCustomData)
        {
            AppDomainManager domainManager = AppDomain.CurrentDomain.DomainManager;
            if (domainManager == null)
            {
                domainManager = new AppDomainManager();
            }
            return domainManager.ApplicationActivator.CreateInstance(activationContext, activationCustomData);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static ObjectHandle CreateInstance(string assemblyName, string typeName)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return CreateInstance(assemblyName, typeName, false, BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance, null, null, null, null, null, ref lookForMyCaller);
        }

        public static object CreateInstance(Type type, params object[] args)
        {
            return CreateInstance(type, BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance, null, args, null, null);
        }

        public static object CreateInstance(Type type, bool nonPublic)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            RuntimeType underlyingSystemType = type.UnderlyingSystemType as RuntimeType;
            if (underlyingSystemType == null)
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_MustBeType"), "type");
            }
            return underlyingSystemType.CreateInstanceImpl(!nonPublic);
        }

        [PermissionSet(SecurityAction.LinkDemand, Unrestricted=true)]
        public static ObjectHandle CreateInstance(AppDomain domain, string assemblyName, string typeName)
        {
            if (domain == null)
            {
                throw new ArgumentNullException("domain");
            }
            return domain.InternalCreateInstanceWithNoSecurity(assemblyName, typeName);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static ObjectHandle CreateInstance(string assemblyName, string typeName, object[] activationAttributes)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return CreateInstance(assemblyName, typeName, false, BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance, null, null, null, activationAttributes, null, ref lookForMyCaller);
        }

        public static object CreateInstance(Type type, object[] args, object[] activationAttributes)
        {
            return CreateInstance(type, BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance, null, args, null, activationAttributes);
        }

        public static object CreateInstance(Type type, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture)
        {
            return CreateInstance(type, bindingAttr, binder, args, culture, null);
        }

        public static object CreateInstance(Type type, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (type is TypeBuilder)
            {
                throw new NotSupportedException(Environment.GetResourceString("NotSupported_CreateInstanceWithTypeBuilder"));
            }
            if ((bindingAttr & 0xff) == BindingFlags.Default)
            {
                bindingAttr |= BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance;
            }
            if ((activationAttributes != null) && (activationAttributes.Length > 0))
            {
                if (!type.IsMarshalByRef)
                {
                    throw new NotSupportedException(Environment.GetResourceString("NotSupported_ActivAttrOnNonMBR"));
                }
                if (!type.IsContextful && ((activationAttributes.Length > 1) || !(activationAttributes[0] is UrlAttribute)))
                {
                    throw new NotSupportedException(Environment.GetResourceString("NotSupported_NonUrlAttrOnMBR"));
                }
            }
            RuntimeType underlyingSystemType = type.UnderlyingSystemType as RuntimeType;
            if (underlyingSystemType == null)
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_MustBeType"), "type");
            }
            return underlyingSystemType.CreateInstanceImpl(bindingAttr, binder, args, culture, activationAttributes);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static ObjectHandle CreateInstance(string assemblyName, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, Evidence securityInfo)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return CreateInstance(assemblyName, typeName, ignoreCase, bindingAttr, binder, args, culture, activationAttributes, securityInfo, ref lookForMyCaller);
        }

        [PermissionSet(SecurityAction.LinkDemand, Unrestricted=true)]
        public static ObjectHandle CreateInstance(AppDomain domain, string assemblyName, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, Evidence securityAttributes)
        {
            if (domain == null)
            {
                throw new ArgumentNullException("domain");
            }
            return domain.InternalCreateInstanceWithNoSecurity(assemblyName, typeName, ignoreCase, bindingAttr, binder, args, culture, activationAttributes, securityAttributes);
        }

        internal static ObjectHandle CreateInstance(string assemblyName, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, Evidence securityInfo, ref StackCrawlMark stackMark)
        {
            Assembly assembly;
            if (assemblyName == null)
            {
                assembly = Assembly.nGetExecutingAssembly(ref stackMark);
            }
            else
            {
                assembly = Assembly.InternalLoad(assemblyName, securityInfo, ref stackMark, false);
            }
            if (assembly == null)
            {
                return null;
            }
            object o = CreateInstance(assembly.GetType(typeName, true, ignoreCase), bindingAttr, binder, args, culture, activationAttributes);
            if (o == null)
            {
                return null;
            }
            return new ObjectHandle(o);
        }

        public static ObjectHandle CreateInstanceFrom(string assemblyFile, string typeName)
        {
            return CreateInstanceFrom(assemblyFile, typeName, null);
        }

        [PermissionSet(SecurityAction.LinkDemand, Unrestricted=true)]
        public static ObjectHandle CreateInstanceFrom(AppDomain domain, string assemblyFile, string typeName)
        {
            if (domain == null)
            {
                throw new ArgumentNullException("domain");
            }
            return domain.InternalCreateInstanceFromWithNoSecurity(assemblyFile, typeName);
        }

        public static ObjectHandle CreateInstanceFrom(string assemblyFile, string typeName, object[] activationAttributes)
        {
            return CreateInstanceFrom(assemblyFile, typeName, false, BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance, null, null, null, activationAttributes, null);
        }

        public static ObjectHandle CreateInstanceFrom(string assemblyFile, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, Evidence securityInfo)
        {
            object o = CreateInstance(Assembly.LoadFrom(assemblyFile, securityInfo).GetType(typeName, true, ignoreCase), bindingAttr, binder, args, culture, activationAttributes);
            if (o == null)
            {
                return null;
            }
            return new ObjectHandle(o);
        }

        [PermissionSet(SecurityAction.LinkDemand, Unrestricted=true)]
        public static ObjectHandle CreateInstanceFrom(AppDomain domain, string assemblyFile, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, Evidence securityAttributes)
        {
            if (domain == null)
            {
                throw new ArgumentNullException("domain");
            }
            return domain.InternalCreateInstanceFromWithNoSecurity(assemblyFile, typeName, ignoreCase, bindingAttr, binder, args, culture, activationAttributes, securityAttributes);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.RemotingConfiguration)]
        public static object GetObject(Type type, string url)
        {
            return GetObject(type, url, null);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.RemotingConfiguration)]
        public static object GetObject(Type type, string url, object state)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return RemotingServices.Connect(type, url, state);
        }

        internal static object InternalCreateInstanceWithNoMemberAccessCheck(Type type, bool nonPublic)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            RuntimeType underlyingSystemType = type.UnderlyingSystemType as RuntimeType;
            if (underlyingSystemType == null)
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_MustBeType"), "type");
            }
            return underlyingSystemType.CreateInstanceImpl(!nonPublic, false, false);
        }

        [Conditional("_DEBUG")]
        private static void Log(bool test, string title, string success, string failure)
        {
            if (test)
            {
            }
        }

        void _Activator.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
        {
            throw new NotImplementedException();
        }

        void _Activator.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
        {
            throw new NotImplementedException();
        }

        void _Activator.GetTypeInfoCount(out uint pcTInfo)
        {
            throw new NotImplementedException();
        }

        void _Activator.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
        {
            throw new NotImplementedException();
        }
    }
}

