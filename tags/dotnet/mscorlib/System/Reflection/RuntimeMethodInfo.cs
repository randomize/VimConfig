namespace System.Reflection
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection.Emit;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Security.Permissions;
    using System.Threading;

    [Serializable]
    internal sealed class RuntimeMethodInfo : MethodInfo, ISerializable
    {
        private System.Reflection.BindingFlags m_bindingFlags;
        private RuntimeType m_declaringType;
        private RuntimeMethodHandle m_handle;
        private uint m_invocationFlags;
        private MethodAttributes m_methodAttributes;
        private string m_name;
        private ParameterInfo[] m_parameters;
        private RuntimeType.RuntimeTypeCache m_reflectedTypeCache;
        private ParameterInfo m_returnParameter;
        private System.Signature m_signature;
        private string m_toString;

        internal RuntimeMethodInfo()
        {
        }

        internal RuntimeMethodInfo(RuntimeMethodHandle handle, RuntimeTypeHandle declaringTypeHandle, RuntimeType.RuntimeTypeCache reflectedTypeCache, MethodAttributes methodAttributes, System.Reflection.BindingFlags bindingFlags)
        {
            this.m_toString = null;
            this.m_bindingFlags = bindingFlags;
            this.m_handle = handle;
            this.m_reflectedTypeCache = reflectedTypeCache;
            this.m_parameters = null;
            this.m_methodAttributes = methodAttributes;
            this.m_declaringType = declaringTypeHandle.GetRuntimeType();
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        internal override bool CacheEquals(object o)
        {
            RuntimeMethodInfo info = o as RuntimeMethodInfo;
            if (info == null)
            {
                return false;
            }
            return info.m_handle.Equals(this.m_handle);
        }

        private void CheckConsistency(object target)
        {
            if (((this.m_methodAttributes & MethodAttributes.Static) != MethodAttributes.Static) && !this.m_declaringType.IsInstanceOfType(target))
            {
                if (target == null)
                {
                    throw new TargetException(Environment.GetResourceString("RFLCT.Targ_StatMethReqTarg"));
                }
                throw new TargetException(Environment.GetResourceString("RFLCT.Targ_ITargMismatch"));
            }
        }

        internal static string ConstructName(MethodBase mi)
        {
            string str = null;
            str = str + mi.Name;
            RuntimeMethodInfo info = mi as RuntimeMethodInfo;
            if ((info != null) && info.IsGenericMethod)
            {
                str = str + info.m_handle.ConstructInstantiation();
            }
            return (str + "(" + ConstructParameters(mi.GetParametersNoCopy(), mi.CallingConvention) + ")");
        }

        internal static string ConstructParameters(ParameterInfo[] parameters, CallingConventions callingConvention)
        {
            Type[] typeArray = new Type[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                typeArray[i] = parameters[i].ParameterType;
            }
            return ConstructParameters(typeArray, callingConvention);
        }

        internal static string ConstructParameters(Type[] parameters, CallingConventions callingConvention)
        {
            string str = "";
            string str2 = "";
            for (int i = 0; i < parameters.Length; i++)
            {
                Type type = parameters[i];
                str = str + str2 + type.SigToString();
                if (type.IsByRef)
                {
                    str = str.TrimEnd(new char[] { '&' }) + " ByRef";
                }
                str2 = ", ";
            }
            if ((callingConvention & CallingConventions.VarArgs) == CallingConventions.VarArgs)
            {
                str = str + str2 + "...";
            }
            return str;
        }

        public override bool Equals(object obj)
        {
            if (!this.IsGenericMethod)
            {
                return (obj == this);
            }
            RuntimeMethodInfo info = obj as RuntimeMethodInfo;
            RuntimeMethodHandle handle = this.GetMethodHandle().StripMethodInstantiation();
            RuntimeMethodHandle handle2 = info.GetMethodHandle().StripMethodInstantiation();
            if (handle != handle2)
            {
                return false;
            }
            if ((info == null) || !info.IsGenericMethod)
            {
                return false;
            }
            Type[] genericArguments = this.GetGenericArguments();
            Type[] typeArray2 = info.GetGenericArguments();
            if (genericArguments.Length != typeArray2.Length)
            {
                return false;
            }
            for (int i = 0; i < genericArguments.Length; i++)
            {
                if (genericArguments[i] != typeArray2[i])
                {
                    return false;
                }
            }
            if (info.IsGenericMethod)
            {
                if (this.DeclaringType != info.DeclaringType)
                {
                    return false;
                }
                if (this.ReflectedType != info.ReflectedType)
                {
                    return false;
                }
            }
            return true;
        }

        internal ParameterInfo[] FetchNonReturnParameters()
        {
            if (this.m_parameters == null)
            {
                this.m_parameters = ParameterInfo.GetParameters(this, this, this.Signature);
            }
            return this.m_parameters;
        }

        internal ParameterInfo FetchReturnParameter()
        {
            if (this.m_returnParameter == null)
            {
                this.m_returnParameter = ParameterInfo.GetReturnParameter(this, this, this.Signature);
            }
            return this.m_returnParameter;
        }

        public override MethodInfo GetBaseDefinition()
        {
            if ((!base.IsVirtual || base.IsStatic) || ((this.m_declaringType == null) || this.m_declaringType.IsInterface))
            {
                return this;
            }
            int slot = this.m_handle.GetSlot();
            Type declaringType = this.DeclaringType;
            Type type2 = this.DeclaringType;
            RuntimeMethodHandle methodHandle = new RuntimeMethodHandle();
            do
            {
                RuntimeTypeHandle typeHandleInternal = declaringType.GetTypeHandleInternal();
                if (typeHandleInternal.GetNumVirtuals() <= slot)
                {
                    break;
                }
                methodHandle = typeHandleInternal.GetMethodAt(slot);
                type2 = declaringType;
                declaringType = declaringType.BaseType;
            }
            while (declaringType != null);
            return (MethodInfo) RuntimeType.GetMethodBase(type2.GetTypeHandleInternal(), methodHandle);
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return CustomAttribute.GetCustomAttributes(this, typeof(object) as RuntimeType, inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
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
            return CustomAttribute.GetCustomAttributes(this, underlyingSystemType, inherit);
        }

        public override Type[] GetGenericArguments()
        {
            RuntimeType[] typeArray = null;
            RuntimeTypeHandle[] methodInstantiation = this.m_handle.GetMethodInstantiation();
            if (methodInstantiation != null)
            {
                typeArray = new RuntimeType[methodInstantiation.Length];
                for (int i = 0; i < methodInstantiation.Length; i++)
                {
                    typeArray[i] = methodInstantiation[i].GetRuntimeType();
                }
                return typeArray;
            }
            return new RuntimeType[0];
        }

        public override MethodInfo GetGenericMethodDefinition()
        {
            if (!this.IsGenericMethod)
            {
                throw new InvalidOperationException();
            }
            return (RuntimeType.GetMethodBase(this.m_declaringType.GetTypeHandleInternal(), this.m_handle.StripMethodInstantiation()) as MethodInfo);
        }

        public override int GetHashCode()
        {
            return this.GetMethodHandle().GetHashCode();
        }

        [ReflectionPermission(SecurityAction.Demand, Flags=ReflectionPermissionFlag.MemberAccess)]
        public override MethodBody GetMethodBody()
        {
            MethodBody methodBody = this.m_handle.GetMethodBody(this.ReflectedTypeHandle);
            if (methodBody != null)
            {
                methodBody.m_methodBase = this;
            }
            return methodBody;
        }

        internal override RuntimeMethodHandle GetMethodHandle()
        {
            return this.m_handle;
        }

        public override MethodImplAttributes GetMethodImplementationFlags()
        {
            return this.m_handle.GetImplAttributes();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            if (this.m_reflectedTypeCache.IsGlobal)
            {
                throw new NotSupportedException(Environment.GetResourceString("NotSupported_GlobalMethodSerialization"));
            }
            MemberInfoSerializationHolder.GetSerializationInfo(info, this.Name, this.ReflectedTypeHandle.GetRuntimeType(), this.ToString(), MemberTypes.Method, (this.IsGenericMethod & !this.IsGenericMethodDefinition) ? this.GetGenericArguments() : null);
        }

        internal override uint GetOneTimeFlags()
        {
            uint num = 0;
            if (this.ReturnType.IsByRef)
            {
                num = 2;
            }
            return (num | base.GetOneTimeFlags());
        }

        public override ParameterInfo[] GetParameters()
        {
            this.FetchNonReturnParameters();
            if (this.m_parameters.Length == 0)
            {
                return this.m_parameters;
            }
            ParameterInfo[] destinationArray = new ParameterInfo[this.m_parameters.Length];
            Array.Copy(this.m_parameters, destinationArray, this.m_parameters.Length);
            return destinationArray;
        }

        internal override ParameterInfo[] GetParametersNoCopy()
        {
            this.FetchNonReturnParameters();
            return this.m_parameters;
        }

        internal override MethodInfo GetParentDefinition()
        {
            if (!base.IsVirtual || this.m_declaringType.IsInterface)
            {
                return null;
            }
            Type baseType = this.m_declaringType.BaseType;
            if (baseType == null)
            {
                return null;
            }
            int slot = this.m_handle.GetSlot();
            if (baseType.GetTypeHandleInternal().GetNumVirtuals() <= slot)
            {
                return null;
            }
            return (MethodInfo) RuntimeType.GetMethodBase(baseType.GetTypeHandleInternal(), baseType.GetTypeHandleInternal().GetMethodAt(slot));
        }

        internal static MethodBase InternalGetCurrentMethod(ref StackCrawlMark stackMark)
        {
            RuntimeMethodHandle currentMethod = RuntimeMethodHandle.GetCurrentMethod(ref stackMark);
            if (currentMethod.IsNullHandle())
            {
                return null;
            }
            return RuntimeType.GetMethodBase(currentMethod.GetTypicalMethodDefinition());
        }

        [DebuggerHidden, DebuggerStepThrough]
        public override object Invoke(object obj, System.Reflection.BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
        {
            return this.Invoke(obj, invokeAttr, binder, parameters, culture, false);
        }

        [DebuggerStepThrough, DebuggerHidden]
        internal object Invoke(object obj, System.Reflection.BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture, bool skipVisibilityChecks)
        {
            int length = this.Signature.Arguments.Length;
            int num2 = (parameters != null) ? parameters.Length : 0;
            if ((this.m_invocationFlags & 1) == 0)
            {
                this.m_invocationFlags = this.GetOneTimeFlags();
            }
            if ((this.m_invocationFlags & 2) != 0)
            {
                this.ThrowNoInvokeException();
            }
            this.CheckConsistency(obj);
            if (length != num2)
            {
                throw new TargetParameterCountException(Environment.GetResourceString("Arg_ParmCnt"));
            }
            if (num2 > 0xffff)
            {
                throw new TargetParameterCountException(Environment.GetResourceString("NotSupported_TooManyArgs"));
            }
            if (!skipVisibilityChecks && ((this.m_invocationFlags & 0x24) != 0))
            {
                if ((this.m_invocationFlags & 0x20) != 0)
                {
                    CodeAccessPermission.DemandInternal(PermissionType.ReflectionMemberAccess);
                }
                if ((this.m_invocationFlags & 4) != 0)
                {
                    MethodBase.PerformSecurityCheck(obj, this.m_handle, this.m_declaringType.TypeHandle.Value, this.m_invocationFlags);
                }
            }
            RuntimeTypeHandle emptyHandle = RuntimeTypeHandle.EmptyHandle;
            if (!this.m_reflectedTypeCache.IsGlobal)
            {
                emptyHandle = this.m_declaringType.TypeHandle;
            }
            if (num2 == 0)
            {
                return this.m_handle.InvokeMethodFast(obj, null, this.Signature, this.m_methodAttributes, emptyHandle);
            }
            object[] arguments = base.CheckArguments(parameters, binder, invokeAttr, culture, this.Signature);
            object obj2 = this.m_handle.InvokeMethodFast(obj, arguments, this.Signature, this.m_methodAttributes, emptyHandle);
            for (int i = 0; i < num2; i++)
            {
                parameters[i] = arguments[i];
            }
            return obj2;
        }

        public override bool IsDefined(Type attributeType, bool inherit)
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
            return CustomAttribute.IsDefined(this, underlyingSystemType, inherit);
        }

        public override MethodInfo MakeGenericMethod(params Type[] methodInstantiation)
        {
            if (methodInstantiation == null)
            {
                throw new ArgumentNullException("methodInstantiation");
            }
            Type[] typeArray = new Type[methodInstantiation.Length];
            for (int i = 0; i < methodInstantiation.Length; i++)
            {
                typeArray[i] = methodInstantiation[i];
            }
            methodInstantiation = typeArray;
            if (!this.IsGenericMethodDefinition)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Arg_NotGenericMethodDefinition"), new object[] { this }));
            }
            for (int j = 0; j < methodInstantiation.Length; j++)
            {
                if (methodInstantiation[j] == null)
                {
                    throw new ArgumentNullException();
                }
                if (!(methodInstantiation[j] is RuntimeType))
                {
                    return MethodBuilderInstantiation.MakeGenericMethod(this, methodInstantiation);
                }
            }
            Type[] genericArguments = this.GetGenericArguments();
            RuntimeType.SanityCheckGenericArguments(methodInstantiation, genericArguments);
            RuntimeTypeHandle[] handleArray = new RuntimeTypeHandle[methodInstantiation.Length];
            for (int k = 0; k < methodInstantiation.Length; k++)
            {
                handleArray[k] = methodInstantiation[k].GetTypeHandleInternal();
            }
            MethodInfo methodBase = null;
            try
            {
                methodBase = RuntimeType.GetMethodBase(this.m_reflectedTypeCache.RuntimeTypeHandle, this.m_handle.GetInstantiatingStub(this.m_declaringType.GetTypeHandleInternal(), handleArray)) as MethodInfo;
            }
            catch (VerificationException exception)
            {
                RuntimeType.ValidateGenericArguments(this, methodInstantiation, exception);
                throw exception;
            }
            return methodBase;
        }

        private void ThrowNoInvokeException()
        {
            Type declaringType = this.DeclaringType;
            if (((declaringType == null) && this.Module.Assembly.ReflectionOnly) || (declaringType is ReflectionOnlyType))
            {
                throw new InvalidOperationException(Environment.GetResourceString("Arg_ReflectionOnlyInvoke"));
            }
            if (this.DeclaringType.GetRootElementType() == typeof(ArgIterator))
            {
                throw new NotSupportedException();
            }
            if ((this.CallingConvention & CallingConventions.VarArgs) == CallingConventions.VarArgs)
            {
                throw new NotSupportedException();
            }
            if (this.DeclaringType.ContainsGenericParameters || this.ContainsGenericParameters)
            {
                throw new InvalidOperationException(Environment.GetResourceString("Arg_UnboundGenParam"));
            }
            if (base.IsAbstract)
            {
                throw new MemberAccessException();
            }
            if (this.ReturnType.IsByRef)
            {
                throw new NotSupportedException(Environment.GetResourceString("NotSupported_ByRefReturn"));
            }
            throw new TargetException();
        }

        public override string ToString()
        {
            if (this.m_toString == null)
            {
                this.m_toString = this.ReturnType.SigToString() + " " + ConstructName(this);
            }
            return this.m_toString;
        }

        public override MethodAttributes Attributes
        {
            get
            {
                return this.m_methodAttributes;
            }
        }

        internal System.Reflection.BindingFlags BindingFlags
        {
            get
            {
                return this.m_bindingFlags;
            }
        }

        public override CallingConventions CallingConvention
        {
            get
            {
                return this.Signature.CallingConvention;
            }
        }

        public override bool ContainsGenericParameters
        {
            get
            {
                if ((this.DeclaringType != null) && this.DeclaringType.ContainsGenericParameters)
                {
                    return true;
                }
                if (this.IsGenericMethod)
                {
                    Type[] genericArguments = this.GetGenericArguments();
                    for (int i = 0; i < genericArguments.Length; i++)
                    {
                        if (genericArguments[i].ContainsGenericParameters)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public override Type DeclaringType
        {
            get
            {
                if (this.m_reflectedTypeCache.IsGlobal)
                {
                    return null;
                }
                return this.m_declaringType;
            }
        }

        public override bool IsGenericMethod
        {
            get
            {
                return this.m_handle.HasMethodInstantiation();
            }
        }

        public override bool IsGenericMethodDefinition
        {
            get
            {
                return this.m_handle.IsGenericMethodDefinition();
            }
        }

        internal override bool IsOverloaded
        {
            get
            {
                return (this.m_reflectedTypeCache.GetMethodList(MemberListType.CaseSensitive, this.Name).Count > 1);
            }
        }

        public override MemberTypes MemberType
        {
            get
            {
                return MemberTypes.Method;
            }
        }

        public override int MetadataToken
        {
            get
            {
                return this.m_handle.GetMethodDef();
            }
        }

        public override RuntimeMethodHandle MethodHandle
        {
            get
            {
                Type declaringType = this.DeclaringType;
                if (((declaringType == null) && this.Module.Assembly.ReflectionOnly) || (declaringType is ReflectionOnlyType))
                {
                    throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_NotAllowedInReflectionOnly"));
                }
                return this.m_handle;
            }
        }

        public override System.Reflection.Module Module
        {
            get
            {
                return this.m_declaringType.Module;
            }
        }

        public override string Name
        {
            get
            {
                if (this.m_name == null)
                {
                    this.m_name = this.m_handle.GetName();
                }
                return this.m_name;
            }
        }

        public override Type ReflectedType
        {
            get
            {
                if (this.m_reflectedTypeCache.IsGlobal)
                {
                    return null;
                }
                return this.m_reflectedTypeCache.RuntimeType;
            }
        }

        private RuntimeTypeHandle ReflectedTypeHandle
        {
            get
            {
                return this.m_reflectedTypeCache.RuntimeTypeHandle;
            }
        }

        public override ParameterInfo ReturnParameter
        {
            get
            {
                this.FetchReturnParameter();
                return this.m_returnParameter;
            }
        }

        public override Type ReturnType
        {
            get
            {
                return this.Signature.ReturnTypeHandle.GetRuntimeType();
            }
        }

        public override ICustomAttributeProvider ReturnTypeCustomAttributes
        {
            get
            {
                return this.ReturnParameter;
            }
        }

        internal System.Signature Signature
        {
            get
            {
                if (this.m_signature == null)
                {
                    this.m_signature = new System.Signature(this.m_handle, this.m_declaringType.GetTypeHandleInternal());
                }
                return this.m_signature;
            }
        }
    }
}

