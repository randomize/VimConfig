namespace System.Reflection
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    [Serializable, ComVisible(true), ClassInterface(ClassInterfaceType.None), ComDefaultInterface(typeof(_MethodInfo)), PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust")]
    public abstract class MethodInfo : MethodBase, _MethodInfo
    {
        protected MethodInfo()
        {
        }

        public abstract MethodInfo GetBaseDefinition();
        [ComVisible(true)]
        public override Type[] GetGenericArguments()
        {
            throw new NotSupportedException(Environment.GetResourceString("NotSupported_SubclassOverride"));
        }

        [ComVisible(true)]
        public virtual MethodInfo GetGenericMethodDefinition()
        {
            throw new NotSupportedException(Environment.GetResourceString("NotSupported_SubclassOverride"));
        }

        internal virtual MethodInfo GetParentDefinition()
        {
            return null;
        }

        internal override Type GetReturnType()
        {
            return this.ReturnType;
        }

        public virtual MethodInfo MakeGenericMethod(params Type[] typeArguments)
        {
            throw new NotSupportedException(Environment.GetResourceString("NotSupported_SubclassOverride"));
        }

        void _MethodInfo.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
        {
            throw new NotImplementedException();
        }

        Type _MethodInfo.GetType()
        {
            return base.GetType();
        }

        void _MethodInfo.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
        {
            throw new NotImplementedException();
        }

        void _MethodInfo.GetTypeInfoCount(out uint pcTInfo)
        {
            throw new NotImplementedException();
        }

        void _MethodInfo.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
        {
            throw new NotImplementedException();
        }

        public override bool ContainsGenericParameters
        {
            get
            {
                return false;
            }
        }

        public override bool IsGenericMethod
        {
            get
            {
                return false;
            }
        }

        public override bool IsGenericMethodDefinition
        {
            get
            {
                return false;
            }
        }

        public override MemberTypes MemberType
        {
            get
            {
                return MemberTypes.Method;
            }
        }

        public virtual ParameterInfo ReturnParameter
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual Type ReturnType
        {
            get
            {
                return this.GetReturnType();
            }
        }

        public abstract ICustomAttributeProvider ReturnTypeCustomAttributes { get; }
    }
}

