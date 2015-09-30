namespace System.Reflection
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Threading;

    [Serializable, ComDefaultInterface(typeof(_MethodBase)), ComVisible(true), ClassInterface(ClassInterfaceType.None), PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust")]
    public abstract class MethodBase : MemberInfo, _MethodBase
    {
        protected MethodBase()
        {
        }

        internal object[] CheckArguments(object[] parameters, Binder binder, BindingFlags invokeAttr, CultureInfo culture, Signature sig)
        {
            int num = (parameters != null) ? parameters.Length : 0;
            object[] objArray = new object[num];
            ParameterInfo[] parametersNoCopy = null;
            for (int i = 0; i < num; i++)
            {
                object o = parameters[i];
                RuntimeTypeHandle handle = sig.Arguments[i];
                if (o == Type.Missing)
                {
                    if (parametersNoCopy == null)
                    {
                        parametersNoCopy = this.GetParametersNoCopy();
                    }
                    if (parametersNoCopy[i].DefaultValue == DBNull.Value)
                    {
                        throw new ArgumentException(Environment.GetResourceString("Arg_VarMissNull"), "parameters");
                    }
                    o = parametersNoCopy[i].DefaultValue;
                }
                if (handle.IsInstanceOfType(o))
                {
                    objArray[i] = o;
                }
                else
                {
                    objArray[i] = handle.GetRuntimeType().CheckValue(o, binder, culture, invokeAttr);
                }
            }
            return objArray;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static MethodBase GetCurrentMethod()
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return RuntimeMethodInfo.InternalGetCurrentMethod(ref lookForMyCaller);
        }

        [ComVisible(true)]
        public virtual Type[] GetGenericArguments()
        {
            throw new NotSupportedException(Environment.GetResourceString("NotSupported_SubclassOverride"));
        }

        [ReflectionPermission(SecurityAction.Demand, Flags=ReflectionPermissionFlag.MemberAccess)]
        public virtual MethodBody GetMethodBody()
        {
            throw new InvalidOperationException();
        }

        public static MethodBase GetMethodFromHandle(RuntimeMethodHandle handle)
        {
            if (handle.IsNullHandle())
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidHandle"));
            }
            MethodBase methodBase = RuntimeType.GetMethodBase(handle);
            if ((methodBase.DeclaringType != null) && methodBase.DeclaringType.IsGenericType)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_MethodDeclaringTypeGeneric"), new object[] { methodBase, methodBase.DeclaringType.GetGenericTypeDefinition() }));
            }
            return methodBase;
        }

        [ComVisible(false)]
        public static MethodBase GetMethodFromHandle(RuntimeMethodHandle handle, RuntimeTypeHandle declaringType)
        {
            if (handle.IsNullHandle())
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidHandle"));
            }
            return RuntimeType.GetMethodBase(declaringType, handle);
        }

        internal virtual RuntimeMethodHandle GetMethodHandle()
        {
            return this.MethodHandle;
        }

        public abstract MethodImplAttributes GetMethodImplementationFlags();
        internal virtual uint GetOneTimeFlags()
        {
            RuntimeMethodHandle methodHandle = this.MethodHandle;
            uint num = 0;
            Type declaringType = this.DeclaringType;
            if ((this.ContainsGenericParameters || ((declaringType != null) && declaringType.ContainsGenericParameters)) || (((this.CallingConvention & CallingConventions.VarArgs) == CallingConventions.VarArgs) || ((this.Attributes & MethodAttributes.RequireSecObject) == MethodAttributes.RequireSecObject)))
            {
                num |= 2;
            }
            else
            {
                AssemblyBuilderData assemblyData = this.Module.Assembly.m_assemblyData;
                if ((assemblyData != null) && ((assemblyData.m_access & AssemblyBuilderAccess.Run) == 0))
                {
                    num |= 2;
                }
            }
            if (num == 0)
            {
                num |= GetSpecialSecurityFlags(methodHandle);
                if ((num & 4) == 0)
                {
                    if (((this.Attributes & MethodAttributes.MemberAccessMask) != MethodAttributes.Public) || ((declaringType != null) && !declaringType.IsVisible))
                    {
                        num |= 4;
                    }
                    else if (this.IsGenericMethod)
                    {
                        Type[] genericArguments = this.GetGenericArguments();
                        for (int i = 0; i < genericArguments.Length; i++)
                        {
                            if (!genericArguments[i].IsVisible)
                            {
                                num |= 4;
                                break;
                            }
                        }
                    }
                }
            }
            num |= this.GetOneTimeSpecificFlags();
            return (num | 1);
        }

        internal virtual uint GetOneTimeSpecificFlags()
        {
            return 0;
        }

        public abstract ParameterInfo[] GetParameters();
        internal virtual ParameterInfo[] GetParametersNoCopy()
        {
            return this.GetParameters();
        }

        internal virtual Type[] GetParameterTypes()
        {
            ParameterInfo[] parametersNoCopy = this.GetParametersNoCopy();
            Type[] typeArray = null;
            typeArray = new Type[parametersNoCopy.Length];
            for (int i = 0; i < parametersNoCopy.Length; i++)
            {
                typeArray[i] = parametersNoCopy[i].ParameterType;
            }
            return typeArray;
        }

        internal virtual Type GetReturnType()
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern uint GetSpecialSecurityFlags(RuntimeMethodHandle method);
        [DebuggerStepThrough, DebuggerHidden]
        public object Invoke(object obj, object[] parameters)
        {
            return this.Invoke(obj, BindingFlags.Default, null, parameters, null);
        }

        public abstract object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void PerformSecurityCheck(object obj, RuntimeMethodHandle method, IntPtr parent, uint invocationFlags);
        void _MethodBase.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
        {
            throw new NotImplementedException();
        }

        Type _MethodBase.GetType()
        {
            return base.GetType();
        }

        void _MethodBase.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
        {
            throw new NotImplementedException();
        }

        void _MethodBase.GetTypeInfoCount(out uint pcTInfo)
        {
            throw new NotImplementedException();
        }

        void _MethodBase.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
        {
            throw new NotImplementedException();
        }

        public abstract MethodAttributes Attributes { get; }

        public virtual CallingConventions CallingConvention
        {
            get
            {
                return CallingConventions.Standard;
            }
        }

        public virtual bool ContainsGenericParameters
        {
            get
            {
                return false;
            }
        }

        public bool IsAbstract
        {
            get
            {
                return ((this.Attributes & MethodAttributes.Abstract) != MethodAttributes.PrivateScope);
            }
        }

        public bool IsAssembly
        {
            get
            {
                return ((this.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Assembly);
            }
        }

        [ComVisible(true)]
        public bool IsConstructor
        {
            get
            {
                return (((this.Attributes & MethodAttributes.RTSpecialName) != MethodAttributes.PrivateScope) && this.Name.Equals(ConstructorInfo.ConstructorName));
            }
        }

        public bool IsFamily
        {
            get
            {
                return ((this.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Family);
            }
        }

        public bool IsFamilyAndAssembly
        {
            get
            {
                return ((this.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.FamANDAssem);
            }
        }

        public bool IsFamilyOrAssembly
        {
            get
            {
                return ((this.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.FamORAssem);
            }
        }

        public bool IsFinal
        {
            get
            {
                return ((this.Attributes & MethodAttributes.Final) != MethodAttributes.PrivateScope);
            }
        }

        public virtual bool IsGenericMethod
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsGenericMethodDefinition
        {
            get
            {
                return false;
            }
        }

        public bool IsHideBySig
        {
            get
            {
                return ((this.Attributes & MethodAttributes.HideBySig) != MethodAttributes.PrivateScope);
            }
        }

        internal virtual bool IsOverloaded
        {
            get
            {
                throw new NotSupportedException(Environment.GetResourceString("InvalidOperation_Method"));
            }
        }

        public bool IsPrivate
        {
            get
            {
                return ((this.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Private);
            }
        }

        public bool IsPublic
        {
            get
            {
                return ((this.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public);
            }
        }

        public bool IsSpecialName
        {
            get
            {
                return ((this.Attributes & MethodAttributes.SpecialName) != MethodAttributes.PrivateScope);
            }
        }

        public bool IsStatic
        {
            get
            {
                return ((this.Attributes & MethodAttributes.Static) != MethodAttributes.PrivateScope);
            }
        }

        public bool IsVirtual
        {
            get
            {
                return ((this.Attributes & MethodAttributes.Virtual) != MethodAttributes.PrivateScope);
            }
        }

        public abstract RuntimeMethodHandle MethodHandle { get; }

        bool _MethodBase.IsAbstract
        {
            get
            {
                return this.IsAbstract;
            }
        }

        bool _MethodBase.IsAssembly
        {
            get
            {
                return this.IsAssembly;
            }
        }

        bool _MethodBase.IsConstructor
        {
            get
            {
                return this.IsConstructor;
            }
        }

        bool _MethodBase.IsFamily
        {
            get
            {
                return this.IsFamily;
            }
        }

        bool _MethodBase.IsFamilyAndAssembly
        {
            get
            {
                return this.IsFamilyAndAssembly;
            }
        }

        bool _MethodBase.IsFamilyOrAssembly
        {
            get
            {
                return this.IsFamilyOrAssembly;
            }
        }

        bool _MethodBase.IsFinal
        {
            get
            {
                return this.IsFinal;
            }
        }

        bool _MethodBase.IsHideBySig
        {
            get
            {
                return this.IsHideBySig;
            }
        }

        bool _MethodBase.IsPrivate
        {
            get
            {
                return this.IsPrivate;
            }
        }

        bool _MethodBase.IsPublic
        {
            get
            {
                return this.IsPublic;
            }
        }

        bool _MethodBase.IsSpecialName
        {
            get
            {
                return this.IsSpecialName;
            }
        }

        bool _MethodBase.IsStatic
        {
            get
            {
                return this.IsStatic;
            }
        }

        bool _MethodBase.IsVirtual
        {
            get
            {
                return this.IsVirtual;
            }
        }
    }
}

