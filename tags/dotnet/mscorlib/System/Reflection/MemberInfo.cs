namespace System.Reflection
{
    using System;
    using System.Reflection.Cache;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Threading;

    [Serializable, ComVisible(true), ClassInterface(ClassInterfaceType.None), ComDefaultInterface(typeof(_MemberInfo)), PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust")]
    public abstract class MemberInfo : ICustomAttributeProvider, _MemberInfo
    {
        internal const uint INVOCATION_FLAGS_CONSTRUCTOR_INVOKE = 0x10000000;
        internal const uint INVOCATION_FLAGS_CONTAINS_STACK_POINTERS = 0x100;
        internal const uint INVOCATION_FLAGS_FIELD_SPECIAL_CAST = 0x20;
        internal const uint INVOCATION_FLAGS_INITIALIZED = 1;
        internal const uint INVOCATION_FLAGS_IS_CTOR = 0x10;
        internal const uint INVOCATION_FLAGS_IS_DELEGATE_CTOR = 0x80;
        internal const uint INVOCATION_FLAGS_NEED_SECURITY = 4;
        internal const uint INVOCATION_FLAGS_NO_CTOR_INVOKE = 8;
        internal const uint INVOCATION_FLAGS_NO_INVOKE = 2;
        internal const uint INVOCATION_FLAGS_RISKY_METHOD = 0x20;
        internal const uint INVOCATION_FLAGS_SECURITY_IMPOSED = 0x40;
        internal const uint INVOCATION_FLAGS_SPECIAL_FIELD = 0x10;
        internal const uint INVOCATION_FLAGS_UNKNOWN = 0;
        private InternalCache m_cachedData;

        protected MemberInfo()
        {
        }

        internal virtual bool CacheEquals(object o)
        {
            throw new NotImplementedException();
        }

        public abstract object[] GetCustomAttributes(bool inherit);
        public abstract object[] GetCustomAttributes(Type attributeType, bool inherit);
        public abstract bool IsDefined(Type attributeType, bool inherit);
        internal void OnCacheClear(object sender, ClearCacheEventArgs cacheEventArgs)
        {
            this.m_cachedData = null;
        }

        void _MemberInfo.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
        {
            throw new NotImplementedException();
        }

        Type _MemberInfo.GetType()
        {
            return base.GetType();
        }

        void _MemberInfo.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
        {
            throw new NotImplementedException();
        }

        void _MemberInfo.GetTypeInfoCount(out uint pcTInfo)
        {
            throw new NotImplementedException();
        }

        void _MemberInfo.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
        {
            throw new NotImplementedException();
        }

        internal InternalCache Cache
        {
            get
            {
                InternalCache cachedData = this.m_cachedData;
                if (cachedData == null)
                {
                    cachedData = new InternalCache("MemberInfo");
                    InternalCache cache2 = Interlocked.CompareExchange<InternalCache>(ref this.m_cachedData, cachedData, null);
                    if (cache2 != null)
                    {
                        cachedData = cache2;
                    }
                    GC.ClearCache += new ClearCacheHandler(this.OnCacheClear);
                }
                return cachedData;
            }
        }

        public abstract Type DeclaringType { get; }

        public abstract MemberTypes MemberType { get; }

        public virtual int MetadataToken
        {
            get
            {
                throw new InvalidOperationException();
            }
        }

        internal virtual int MetadataTokenInternal
        {
            get
            {
                return this.MetadataToken;
            }
        }

        public virtual System.Reflection.Module Module
        {
            get
            {
                if (!(this is Type))
                {
                    throw new NotImplementedException();
                }
                return ((Type) this).Module;
            }
        }

        public abstract string Name { get; }

        public abstract Type ReflectedType { get; }
    }
}

