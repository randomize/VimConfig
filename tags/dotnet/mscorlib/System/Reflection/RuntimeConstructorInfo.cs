namespace System.Reflection
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Security.Permissions;

    [Serializable]
    internal sealed class RuntimeConstructorInfo : ConstructorInfo, ISerializable
    {
        private System.Reflection.BindingFlags m_bindingFlags;
        private RuntimeType m_declaringType;
        private RuntimeMethodHandle m_handle;
        private uint m_invocationFlags;
        private MethodAttributes m_methodAttributes;
        private ParameterInfo[] m_parameters;
        private RuntimeType.RuntimeTypeCache m_reflectedTypeCache;
        private System.Signature m_signature;
        private string m_toString;

        internal RuntimeConstructorInfo()
        {
        }

        internal RuntimeConstructorInfo(RuntimeMethodHandle handle, RuntimeTypeHandle declaringTypeHandle, RuntimeType.RuntimeTypeCache reflectedTypeCache, MethodAttributes methodAttributes, System.Reflection.BindingFlags bindingFlags)
        {
            this.m_bindingFlags = bindingFlags;
            this.m_handle = handle;
            this.m_reflectedTypeCache = reflectedTypeCache;
            this.m_declaringType = declaringTypeHandle.GetRuntimeType();
            this.m_parameters = null;
            this.m_toString = null;
            this.m_methodAttributes = methodAttributes;
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        internal override bool CacheEquals(object o)
        {
            RuntimeConstructorInfo info = o as RuntimeConstructorInfo;
            if (info == null)
            {
                return false;
            }
            return info.m_handle.Equals(this.m_handle);
        }

        internal static void CheckCanCreateInstance(Type declaringType, bool isVarArg)
        {
            if (declaringType == null)
            {
                throw new ArgumentNullException("declaringType");
            }
            if (declaringType is ReflectionOnlyType)
            {
                throw new InvalidOperationException(Environment.GetResourceString("Arg_ReflectionOnlyInvoke"));
            }
            if (declaringType.IsInterface)
            {
                throw new MemberAccessException(string.Format(CultureInfo.CurrentUICulture, Environment.GetResourceString("Acc_CreateInterfaceEx"), new object[] { declaringType }));
            }
            if (declaringType.IsAbstract)
            {
                throw new MemberAccessException(string.Format(CultureInfo.CurrentUICulture, Environment.GetResourceString("Acc_CreateAbstEx"), new object[] { declaringType }));
            }
            if (declaringType.GetRootElementType() == typeof(ArgIterator))
            {
                throw new NotSupportedException();
            }
            if (isVarArg)
            {
                throw new NotSupportedException();
            }
            if (declaringType.ContainsGenericParameters)
            {
                throw new MemberAccessException(string.Format(CultureInfo.CurrentUICulture, Environment.GetResourceString("Acc_CreateGenericEx"), new object[] { declaringType }));
            }
            if (declaringType == typeof(void))
            {
                throw new MemberAccessException(Environment.GetResourceString("Access_Void"));
            }
        }

        private void CheckConsistency(object target)
        {
            if (((target != null) || !base.IsStatic) && !this.m_declaringType.IsInstanceOfType(target))
            {
                if (target == null)
                {
                    throw new TargetException(Environment.GetResourceString("RFLCT.Targ_StatMethReqTarg"));
                }
                throw new TargetException(Environment.GetResourceString("RFLCT.Targ_ITargMismatch"));
            }
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return CustomAttribute.GetCustomAttributes(this, typeof(object) as RuntimeType);
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
            return CustomAttribute.GetCustomAttributes(this, underlyingSystemType);
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
            MemberInfoSerializationHolder.GetSerializationInfo(info, this.Name, this.ReflectedTypeHandle.GetRuntimeType(), this.ToString(), MemberTypes.Constructor);
        }

        internal override uint GetOneTimeSpecificFlags()
        {
            uint num = 0x10;
            if (((this.DeclaringType != null) && this.DeclaringType.IsAbstract) || base.IsStatic)
            {
                return (num | 8);
            }
            if (this.DeclaringType == typeof(void))
            {
                return (num | 2);
            }
            if (typeof(Delegate).IsAssignableFrom(this.DeclaringType))
            {
                num |= 0x80;
            }
            return num;
        }

        public override ParameterInfo[] GetParameters()
        {
            ParameterInfo[] parametersNoCopy = this.GetParametersNoCopy();
            if (parametersNoCopy.Length == 0)
            {
                return parametersNoCopy;
            }
            ParameterInfo[] destinationArray = new ParameterInfo[parametersNoCopy.Length];
            Array.Copy(parametersNoCopy, destinationArray, parametersNoCopy.Length);
            return destinationArray;
        }

        internal override ParameterInfo[] GetParametersNoCopy()
        {
            if (this.m_parameters == null)
            {
                this.m_parameters = ParameterInfo.GetParameters(this, this, this.Signature);
            }
            return this.m_parameters;
        }

        internal override Type GetReturnType()
        {
            return this.Signature.ReturnTypeHandle.GetRuntimeType();
        }

        [DebuggerHidden, DebuggerStepThrough]
        public override object Invoke(System.Reflection.BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
        {
            RuntimeTypeHandle typeHandle = this.m_declaringType.TypeHandle;
            if (this.m_invocationFlags == 0)
            {
                this.m_invocationFlags = this.GetOneTimeFlags();
            }
            if ((this.m_invocationFlags & 0x10a) != 0)
            {
                this.ThrowNoInvokeException();
            }
            if ((this.m_invocationFlags & 0xa4) != 0)
            {
                if ((this.m_invocationFlags & 0x20) != 0)
                {
                    CodeAccessPermission.DemandInternal(PermissionType.ReflectionMemberAccess);
                }
                if ((this.m_invocationFlags & 4) != 0)
                {
                    MethodBase.PerformSecurityCheck(null, this.m_handle, this.m_declaringType.TypeHandle.Value, this.m_invocationFlags & 0x10000000);
                }
                if ((this.m_invocationFlags & 0x80) != 0)
                {
                    new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
                }
            }
            int length = this.Signature.Arguments.Length;
            int num2 = (parameters != null) ? parameters.Length : 0;
            if (length != num2)
            {
                throw new TargetParameterCountException(Environment.GetResourceString("Arg_ParmCnt"));
            }
            RuntimeHelpers.RunClassConstructor(typeHandle);
            if (num2 <= 0)
            {
                return this.m_handle.InvokeConstructor(null, (SignatureStruct) this.Signature, typeHandle);
            }
            object[] args = base.CheckArguments(parameters, binder, invokeAttr, culture, this.Signature);
            object obj2 = this.m_handle.InvokeConstructor(args, (SignatureStruct) this.Signature, typeHandle);
            for (int i = 0; i < num2; i++)
            {
                parameters[i] = args[i];
            }
            return obj2;
        }

        [DebuggerHidden, DebuggerStepThrough]
        public override object Invoke(object obj, System.Reflection.BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
        {
            if (this.m_invocationFlags == 0)
            {
                this.m_invocationFlags = this.GetOneTimeFlags();
            }
            if ((this.m_invocationFlags & 2) != 0)
            {
                this.ThrowNoInvokeException();
            }
            this.CheckConsistency(obj);
            if (obj != null)
            {
                new SecurityPermission(SecurityPermissionFlag.SkipVerification).Demand();
            }
            if ((this.m_invocationFlags & 0x24) != 0)
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
            int length = this.Signature.Arguments.Length;
            int num2 = (parameters != null) ? parameters.Length : 0;
            if (length != num2)
            {
                throw new TargetParameterCountException(Environment.GetResourceString("Arg_ParmCnt"));
            }
            if (num2 <= 0)
            {
                return this.m_handle.InvokeMethodFast(obj, null, this.Signature, this.m_methodAttributes, (this.DeclaringType != null) ? this.DeclaringType.TypeHandle : RuntimeTypeHandle.EmptyHandle);
            }
            object[] arguments = base.CheckArguments(parameters, binder, invokeAttr, culture, this.Signature);
            object obj2 = this.m_handle.InvokeMethodFast(obj, arguments, this.Signature, this.m_methodAttributes, (this.ReflectedType != null) ? this.ReflectedType.TypeHandle : RuntimeTypeHandle.EmptyHandle);
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
            return CustomAttribute.IsDefined(this, underlyingSystemType);
        }

        internal void SerializationInvoke(object target, SerializationInfo info, StreamingContext context)
        {
            this.MethodHandle.SerializationInvoke(target, (SignatureStruct) this.Signature, info, context);
        }

        internal void ThrowNoInvokeException()
        {
            CheckCanCreateInstance(this.DeclaringType, (this.CallingConvention & CallingConventions.VarArgs) == CallingConventions.VarArgs);
            if ((this.Attributes & MethodAttributes.Static) == MethodAttributes.Static)
            {
                throw new MemberAccessException(Environment.GetResourceString("Acc_NotClassInit"));
            }
            throw new TargetException();
        }

        public override string ToString()
        {
            if (this.m_toString == null)
            {
                this.m_toString = "Void " + RuntimeMethodInfo.ConstructName(this);
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

        public override Type DeclaringType
        {
            get
            {
                if (!this.m_reflectedTypeCache.IsGlobal)
                {
                    return this.m_declaringType;
                }
                return null;
            }
        }

        internal override bool IsOverloaded
        {
            get
            {
                return (this.m_reflectedTypeCache.GetConstructorList(MemberListType.CaseSensitive, this.Name).Count > 1);
            }
        }

        [ComVisible(true)]
        public override MemberTypes MemberType
        {
            get
            {
                return MemberTypes.Constructor;
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
                return this.m_declaringType.GetTypeHandleInternal().GetModuleHandle().GetModule();
            }
        }

        public override string Name
        {
            get
            {
                return this.m_handle.GetName();
            }
        }

        public override Type ReflectedType
        {
            get
            {
                if (!this.m_reflectedTypeCache.IsGlobal)
                {
                    return this.m_reflectedTypeCache.RuntimeType;
                }
                return null;
            }
        }

        private RuntimeTypeHandle ReflectedTypeHandle
        {
            get
            {
                return this.m_reflectedTypeCache.RuntimeTypeHandle;
            }
        }

        private System.Signature Signature
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

