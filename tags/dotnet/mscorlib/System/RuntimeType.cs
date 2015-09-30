namespace System
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Activation;
    using System.Runtime.Remoting.Messaging;
    using System.Runtime.Remoting.Proxies;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Security.Permissions;
    using System.Text;
    using System.Threading;

    [Serializable]
    internal class RuntimeType : Type, ISerializable, ICloneable
    {
        private const BindingFlags BinderGetSetField = (BindingFlags.SetField | BindingFlags.GetField);
        private const BindingFlags BinderGetSetProperty = (BindingFlags.SetProperty | BindingFlags.GetProperty);
        private const BindingFlags BinderNonCreateInstance = (BindingFlags.SetProperty | BindingFlags.GetProperty | BindingFlags.SetField | BindingFlags.GetField | BindingFlags.InvokeMethod);
        private const BindingFlags BinderNonFieldGetSet = 0xfff300;
        private const BindingFlags BinderSetInvokeField = (BindingFlags.SetField | BindingFlags.InvokeMethod);
        private const BindingFlags BinderSetInvokeProperty = (BindingFlags.SetProperty | BindingFlags.InvokeMethod);
        private const BindingFlags ClassicBindingMask = (BindingFlags.PutRefDispProperty | BindingFlags.PutDispProperty | BindingFlags.SetProperty | BindingFlags.GetProperty | BindingFlags.InvokeMethod);
        private static bool forceInvokingWithEnUS = ForceEnUSLcidComInvoking();
        private const BindingFlags InvocationMask = (BindingFlags.PutRefDispProperty | BindingFlags.PutDispProperty | BindingFlags.SetProperty | BindingFlags.GetProperty | BindingFlags.SetField | BindingFlags.GetField | BindingFlags.CreateInstance | BindingFlags.InvokeMethod);
        private IntPtr m_cache;
        private RuntimeTypeHandle m_handle;
        private const BindingFlags MemberBindingMask = 0xff;
        private static ActivatorCache s_ActivatorCache;
        private static OleAutBinder s_ForwardCallBinder;
        private static TypeCacheQueue s_typeCache = null;
        private static Type s_typedRef = typeof(TypedReference);

        internal RuntimeType()
        {
        }

        private RuntimeType(RuntimeTypeHandle typeHandle)
        {
            this.m_handle = typeHandle;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern object _CreateEnum(IntPtr enumType, long value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern object AllocateObjectForByRef(RuntimeTypeHandle type, object value);
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        internal override bool CacheEquals(object o)
        {
            RuntimeType type = o as RuntimeType;
            if (type == null)
            {
                return false;
            }
            return type.m_handle.Equals(this.m_handle);
        }

        internal static bool CanCastTo(RuntimeType fromType, RuntimeType toType)
        {
            return fromType.GetTypeHandleInternal().CanCastTo(toType.GetTypeHandleInternal());
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool CanValueSpecialCast(IntPtr valueType, IntPtr targetType);
        internal object CheckValue(object value, Binder binder, CultureInfo culture, BindingFlags invokeAttr)
        {
            if (this.IsInstanceOfType(value))
            {
                return value;
            }
            bool isByRef = base.IsByRef;
            if (isByRef)
            {
                Type elementType = this.GetElementType();
                if (elementType.IsInstanceOfType(value) || (value == null))
                {
                    return AllocateObjectForByRef(elementType.TypeHandle, value);
                }
            }
            else
            {
                if (value == null)
                {
                    return value;
                }
                if (this == s_typedRef)
                {
                    return value;
                }
            }
            bool flag2 = (base.IsPointer || base.IsEnum) || base.IsPrimitive;
            if (flag2)
            {
                Type pointerType;
                Pointer pointer = value as Pointer;
                if (pointer != null)
                {
                    pointerType = pointer.GetPointerType();
                }
                else
                {
                    pointerType = value.GetType();
                }
                if (CanValueSpecialCast(pointerType.TypeHandle.Value, this.TypeHandle.Value))
                {
                    if (pointer != null)
                    {
                        return pointer.GetPointerValue();
                    }
                    return value;
                }
            }
            if ((invokeAttr & BindingFlags.ExactBinding) == BindingFlags.ExactBinding)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentUICulture, Environment.GetResourceString("Arg_ObjObjEx"), new object[] { value.GetType(), this }));
            }
            if ((binder != null) && (binder != Type.DefaultBinder))
            {
                value = binder.ChangeType(value, this, culture);
                if (this.IsInstanceOfType(value))
                {
                    return value;
                }
                if (isByRef)
                {
                    Type type3 = this.GetElementType();
                    if (type3.IsInstanceOfType(value) || (value == null))
                    {
                        return AllocateObjectForByRef(type3.TypeHandle, value);
                    }
                }
                else if (value == null)
                {
                    return value;
                }
                if (flag2)
                {
                    Type type;
                    Pointer pointer2 = value as Pointer;
                    if (pointer2 != null)
                    {
                        type = pointer2.GetPointerType();
                    }
                    else
                    {
                        type = value.GetType();
                    }
                    if (CanValueSpecialCast(type.TypeHandle.Value, this.TypeHandle.Value))
                    {
                        if (pointer2 != null)
                        {
                            return pointer2.GetPointerValue();
                        }
                        return value;
                    }
                }
            }
            throw new ArgumentException(string.Format(CultureInfo.CurrentUICulture, Environment.GetResourceString("Arg_ObjObjEx"), new object[] { value.GetType(), this }));
        }

        public object Clone()
        {
            return this;
        }

        internal static object CreateEnum(RuntimeTypeHandle enumType, long value)
        {
            return _CreateEnum(enumType.Value, value);
        }

        internal void CreateInstanceCheckThis()
        {
            if (this is ReflectionOnlyType)
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_ReflectionOnlyInvoke"));
            }
            if (this.ContainsGenericParameters)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Acc_CreateGenericEx"), new object[] { this }));
            }
            Type rootElementType = this.GetRootElementType();
            if (rootElementType == typeof(ArgIterator))
            {
                throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Acc_CreateArgIterator"), new object[0]));
            }
            if (rootElementType == typeof(void))
            {
                throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Acc_CreateVoid"), new object[0]));
            }
        }

        [DebuggerStepThrough, DebuggerHidden]
        internal object CreateInstanceImpl(bool publicOnly)
        {
            return this.CreateInstanceImpl(publicOnly, false, true);
        }

        [DebuggerHidden, DebuggerStepThrough]
        internal object CreateInstanceImpl(bool publicOnly, bool skipVisibilityChecks, bool fillCache)
        {
            RuntimeTypeHandle typeHandle = this.TypeHandle;
            ActivatorCache cache = s_ActivatorCache;
            if (cache != null)
            {
                ActivatorCacheEntry entry = cache.GetEntry(this);
                if (entry != null)
                {
                    if ((publicOnly && (entry.m_ctor != null)) && ((entry.m_hCtorMethodHandle.GetAttributes() & MethodAttributes.MemberAccessMask) != MethodAttributes.Public))
                    {
                        throw new MissingMethodException(Environment.GetResourceString("Arg_NoDefCTor"));
                    }
                    object obj2 = typeHandle.Allocate();
                    if (entry.m_ctor != null)
                    {
                        if (!skipVisibilityChecks && entry.m_bNeedSecurityCheck)
                        {
                            MethodBase.PerformSecurityCheck(obj2, entry.m_hCtorMethodHandle, this.TypeHandle.Value, 0x10000000);
                        }
                        try
                        {
                            entry.m_ctor(obj2);
                        }
                        catch (Exception exception)
                        {
                            throw new TargetInvocationException(exception);
                        }
                    }
                    return obj2;
                }
            }
            return this.CreateInstanceSlow(publicOnly, fillCache);
        }

        internal object CreateInstanceImpl(BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes)
        {
            this.CreateInstanceCheckThis();
            object obj2 = null;
            try
            {
                try
                {
                    MethodBase base2;
                    if (activationAttributes != null)
                    {
                        ActivationServices.PushActivationAttributes(this, activationAttributes);
                    }
                    if (args == null)
                    {
                        args = new object[0];
                    }
                    int length = args.Length;
                    if (binder == null)
                    {
                        binder = Type.DefaultBinder;
                    }
                    if ((((length == 0) && ((bindingAttr & BindingFlags.Public) != BindingFlags.Default)) && ((bindingAttr & BindingFlags.Instance) != BindingFlags.Default)) && (this.IsGenericCOMObjectImpl() || this.IsSubclassOf(typeof(ValueType))))
                    {
                        return this.CreateInstanceImpl((bindingAttr & BindingFlags.NonPublic) == BindingFlags.Default);
                    }
                    MethodBase[] constructors = this.GetConstructors(bindingAttr);
                    ArrayList list = new ArrayList(constructors.Length);
                    Type[] argumentTypes = new Type[length];
                    for (int i = 0; i < length; i++)
                    {
                        if (args[i] != null)
                        {
                            argumentTypes[i] = args[i].GetType();
                        }
                    }
                    for (int j = 0; j < constructors.Length; j++)
                    {
                        MethodBase base1 = constructors[j];
                        if (FilterApplyMethodBaseInfo(constructors[j], bindingAttr, null, CallingConventions.Any, argumentTypes, false))
                        {
                            list.Add(constructors[j]);
                        }
                    }
                    MethodBase[] array = new MethodBase[list.Count];
                    list.CopyTo(array);
                    if ((array != null) && (array.Length == 0))
                    {
                        array = null;
                    }
                    if (array == null)
                    {
                        if (activationAttributes != null)
                        {
                            ActivationServices.PopActivationAttributes(this);
                            activationAttributes = null;
                        }
                        throw new MissingMethodException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("MissingConstructor_Name"), new object[] { this.FullName }));
                    }
                    if (((length == 0) && (array.Length == 1)) && ((bindingAttr & BindingFlags.OptionalParamBinding) == BindingFlags.Default))
                    {
                        return Activator.CreateInstance(this, true);
                    }
                    object state = null;
                    try
                    {
                        base2 = binder.BindToMethod(bindingAttr, array, ref args, null, culture, null, out state);
                    }
                    catch (MissingMethodException)
                    {
                        base2 = null;
                    }
                    if (base2 == null)
                    {
                        if (activationAttributes != null)
                        {
                            ActivationServices.PopActivationAttributes(this);
                            activationAttributes = null;
                        }
                        throw new MissingMethodException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("MissingConstructor_Name"), new object[] { this.FullName }));
                    }
                    if (typeof(Delegate).IsAssignableFrom(base2.DeclaringType))
                    {
                        new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
                    }
                    obj2 = ((ConstructorInfo) base2).Invoke(bindingAttr, binder, args, culture);
                    if (state != null)
                    {
                        binder.ReorderArgumentArray(ref args, state);
                    }
                    return obj2;
                }
                finally
                {
                    if (activationAttributes != null)
                    {
                        ActivationServices.PopActivationAttributes(this);
                        activationAttributes = null;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return obj2;
        }

        private object CreateInstanceSlow(bool publicOnly, bool fillCache)
        {
            RuntimeMethodHandle emptyHandle = RuntimeMethodHandle.EmptyHandle;
            bool bNeedSecurityCheck = true;
            bool canBeCached = false;
            bool noCheck = false;
            this.CreateInstanceCheckThis();
            if (!fillCache)
            {
                noCheck = true;
            }
            object obj2 = RuntimeTypeHandle.CreateInstance(this, publicOnly, noCheck, ref canBeCached, ref emptyHandle, ref bNeedSecurityCheck);
            if (canBeCached && fillCache)
            {
                ActivatorCache cache = s_ActivatorCache;
                if (cache == null)
                {
                    cache = new ActivatorCache();
                    Thread.MemoryBarrier();
                    s_ActivatorCache = cache;
                }
                ActivatorCacheEntry ace = new ActivatorCacheEntry(this, emptyHandle, bNeedSecurityCheck);
                Thread.MemoryBarrier();
                cache.SetEntry(ace);
            }
            return obj2;
        }

        public override bool Equals(object obj)
        {
            return (obj == this);
        }

        private static bool FilterApplyBase(MemberInfo memberInfo, BindingFlags bindingFlags, bool isPublic, bool isNonProtectedInternal, bool isStatic, string name, bool prefixLookup)
        {
            if (isPublic)
            {
                if ((bindingFlags & BindingFlags.Public) == BindingFlags.Default)
                {
                    return false;
                }
            }
            else if ((bindingFlags & BindingFlags.NonPublic) == BindingFlags.Default)
            {
                return false;
            }
            bool flag = memberInfo.DeclaringType != memberInfo.ReflectedType;
            if (((bindingFlags & BindingFlags.DeclaredOnly) != BindingFlags.Default) && flag)
            {
                return false;
            }
            if ((memberInfo.MemberType != MemberTypes.TypeInfo) && (memberInfo.MemberType != MemberTypes.NestedType))
            {
                if (isStatic)
                {
                    if (((bindingFlags & BindingFlags.FlattenHierarchy) == BindingFlags.Default) && flag)
                    {
                        return false;
                    }
                    if ((bindingFlags & BindingFlags.Static) == BindingFlags.Default)
                    {
                        return false;
                    }
                }
                else if ((bindingFlags & BindingFlags.Instance) == BindingFlags.Default)
                {
                    return false;
                }
            }
            if (prefixLookup && !FilterApplyPrefixLookup(memberInfo, name, (bindingFlags & BindingFlags.IgnoreCase) != BindingFlags.Default))
            {
                return false;
            }
            if (((((bindingFlags & BindingFlags.DeclaredOnly) == BindingFlags.Default) && flag) && (isNonProtectedInternal && ((bindingFlags & BindingFlags.NonPublic) != BindingFlags.Default))) && (!isStatic && ((bindingFlags & BindingFlags.Instance) != BindingFlags.Default)))
            {
                MethodInfo info = memberInfo as MethodInfo;
                if (info == null)
                {
                    return false;
                }
                if (!info.IsVirtual && !info.IsAbstract)
                {
                    return false;
                }
            }
            return true;
        }

        private static bool FilterApplyMethodBaseInfo(MethodBase methodBase, BindingFlags bindingFlags, CallingConventions callConv, Type[] argumentTypes)
        {
            if ((callConv & CallingConventions.Any) == 0)
            {
                if (((callConv & CallingConventions.VarArgs) != 0) && ((methodBase.CallingConvention & CallingConventions.VarArgs) == 0))
                {
                    return false;
                }
                if (((callConv & CallingConventions.Standard) != 0) && ((methodBase.CallingConvention & CallingConventions.Standard) == 0))
                {
                    return false;
                }
            }
            if (argumentTypes != null)
            {
                ParameterInfo[] parametersNoCopy = methodBase.GetParametersNoCopy();
                if (argumentTypes.Length != parametersNoCopy.Length)
                {
                    if ((bindingFlags & (BindingFlags.SetProperty | BindingFlags.GetProperty | BindingFlags.CreateInstance | BindingFlags.InvokeMethod)) == BindingFlags.Default)
                    {
                        return false;
                    }
                    bool flag = false;
                    if (argumentTypes.Length > parametersNoCopy.Length)
                    {
                        if ((methodBase.CallingConvention & CallingConventions.VarArgs) == 0)
                        {
                            flag = true;
                        }
                    }
                    else if ((bindingFlags & BindingFlags.OptionalParamBinding) == BindingFlags.Default)
                    {
                        flag = true;
                    }
                    else if (!parametersNoCopy[argumentTypes.Length].IsOptional)
                    {
                        flag = true;
                    }
                    if (flag)
                    {
                        if (parametersNoCopy.Length != 0)
                        {
                            if (argumentTypes.Length < (parametersNoCopy.Length - 1))
                            {
                                return false;
                            }
                            ParameterInfo info = parametersNoCopy[parametersNoCopy.Length - 1];
                            if (!info.ParameterType.IsArray)
                            {
                                return false;
                            }
                            if (info.IsDefined(typeof(ParamArrayAttribute), false))
                            {
                                goto Label_0108;
                            }
                        }
                        return false;
                    }
                }
                else if (((bindingFlags & BindingFlags.ExactBinding) != BindingFlags.Default) && ((bindingFlags & BindingFlags.InvokeMethod) == BindingFlags.Default))
                {
                    for (int i = 0; i < parametersNoCopy.Length; i++)
                    {
                        if ((argumentTypes[i] != null) && (parametersNoCopy[i].ParameterType != argumentTypes[i]))
                        {
                            return false;
                        }
                    }
                }
            }
        Label_0108:
            return true;
        }

        private static bool FilterApplyMethodBaseInfo(MethodBase methodBase, BindingFlags bindingFlags, string name, CallingConventions callConv, Type[] argumentTypes, bool prefixLookup)
        {
            BindingFlags flags;
            bindingFlags ^= BindingFlags.DeclaredOnly;
            RuntimeMethodInfo info = methodBase as RuntimeMethodInfo;
            if (info == null)
            {
                RuntimeConstructorInfo info2 = methodBase as RuntimeConstructorInfo;
                flags = info2.BindingFlags;
            }
            else
            {
                flags = info.BindingFlags;
            }
            return ((((bindingFlags & flags) == flags) && (!prefixLookup || FilterApplyPrefixLookup(methodBase, name, (bindingFlags & BindingFlags.IgnoreCase) != BindingFlags.Default))) && FilterApplyMethodBaseInfo(methodBase, bindingFlags, callConv, argumentTypes));
        }

        private static bool FilterApplyPrefixLookup(MemberInfo memberInfo, string name, bool ignoreCase)
        {
            if (ignoreCase)
            {
                if (!memberInfo.Name.ToLower(CultureInfo.InvariantCulture).StartsWith(name, StringComparison.Ordinal))
                {
                    return false;
                }
            }
            else if (!memberInfo.Name.StartsWith(name, StringComparison.Ordinal))
            {
                return false;
            }
            return true;
        }

        private static bool FilterApplyType(Type type, BindingFlags bindingFlags, string name, bool prefixLookup, string ns)
        {
            bool isPublic = type.IsNestedPublic || type.IsPublic;
            bool isStatic = false;
            if (!FilterApplyBase(type, bindingFlags, isPublic, type.IsNestedAssembly, isStatic, name, prefixLookup))
            {
                return false;
            }
            if ((ns != null) && !type.Namespace.Equals(ns))
            {
                return false;
            }
            return true;
        }

        private static void FilterHelper(BindingFlags bindingFlags, ref string name, out bool ignoreCase, out MemberListType listType)
        {
            bool flag;
            FilterHelper(bindingFlags, ref name, false, out flag, out ignoreCase, out listType);
        }

        private static void FilterHelper(BindingFlags bindingFlags, ref string name, bool allowPrefixLookup, out bool prefixLookup, out bool ignoreCase, out MemberListType listType)
        {
            prefixLookup = false;
            ignoreCase = false;
            if (name != null)
            {
                if ((bindingFlags & BindingFlags.IgnoreCase) != BindingFlags.Default)
                {
                    name = name.ToLower(CultureInfo.InvariantCulture);
                    ignoreCase = true;
                    listType = MemberListType.CaseInsensitive;
                }
                else
                {
                    listType = MemberListType.CaseSensitive;
                }
                if (allowPrefixLookup && name.EndsWith("*", StringComparison.Ordinal))
                {
                    name = name.Substring(0, name.Length - 1);
                    prefixLookup = true;
                    listType = MemberListType.All;
                }
            }
            else
            {
                listType = MemberListType.All;
            }
        }

        internal static BindingFlags FilterPreCalculate(bool isPublic, bool isInherited, bool isStatic)
        {
            BindingFlags flags = isPublic ? BindingFlags.Public : BindingFlags.NonPublic;
            if (isInherited)
            {
                flags |= BindingFlags.DeclaredOnly;
                if (isStatic)
                {
                    return (flags | (BindingFlags.FlattenHierarchy | BindingFlags.Static));
                }
                return (flags | BindingFlags.Instance);
            }
            if (isStatic)
            {
                return (flags | BindingFlags.Static);
            }
            return (flags | BindingFlags.Instance);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool ForceEnUSLcidComInvoking();
        private object ForwardCallToInvokeMember(string memberName, BindingFlags flags, object target, int[] aWrapperTypes, ref MessageData msgData)
        {
            ParameterModifier[] modifiers = null;
            object obj2 = null;
            Message msg = new Message();
            msg.InitFields(msgData);
            MethodInfo methodBase = (MethodInfo) msg.GetMethodBase();
            object[] args = msg.Args;
            int length = args.Length;
            ParameterInfo[] parametersNoCopy = methodBase.GetParametersNoCopy();
            if (length > 0)
            {
                ParameterModifier modifier = new ParameterModifier(length);
                for (int j = 0; j < length; j++)
                {
                    if (parametersNoCopy[j].ParameterType.IsByRef)
                    {
                        modifier[j] = true;
                    }
                }
                modifiers = new ParameterModifier[] { modifier };
                if (aWrapperTypes != null)
                {
                    this.WrapArgsForInvokeCall(args, aWrapperTypes);
                }
            }
            if (methodBase.ReturnType == typeof(void))
            {
                flags |= BindingFlags.IgnoreReturn;
            }
            try
            {
                obj2 = this.InvokeMember(memberName, flags, null, target, args, modifiers, null, null);
            }
            catch (TargetInvocationException exception)
            {
                throw exception.InnerException;
            }
            for (int i = 0; i < length; i++)
            {
                if (modifiers[0][i] && (args[i] != null))
                {
                    Type elementType = parametersNoCopy[i].ParameterType.GetElementType();
                    if (elementType != args[i].GetType())
                    {
                        args[i] = this.ForwardCallBinder.ChangeType(args[i], elementType, null);
                    }
                }
            }
            if (obj2 != null)
            {
                Type returnType = methodBase.ReturnType;
                if (returnType != obj2.GetType())
                {
                    obj2 = this.ForwardCallBinder.ChangeType(obj2, returnType, null);
                }
            }
            RealProxy.PropagateOutParameters(msg, args, obj2);
            return obj2;
        }

        public override int GetArrayRank()
        {
            if (!this.IsArrayImpl())
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_HasToBeArrayClass"));
            }
            return this.GetTypeHandleInternal().GetArrayRank();
        }

        protected override TypeAttributes GetAttributeFlagsImpl()
        {
            return this.m_handle.GetAttributes();
        }

        private ConstructorInfo[] GetConstructorCandidates(string name, BindingFlags bindingAttr, CallingConventions callConv, Type[] types, bool allowPrefixLookup)
        {
            bool flag;
            bool flag2;
            MemberListType type;
            FilterHelper(bindingAttr, ref name, allowPrefixLookup, out flag, out flag2, out type);
            List<ConstructorInfo> list = new List<ConstructorInfo>();
            CerArrayList<RuntimeConstructorInfo> constructorList = this.Cache.GetConstructorList(type, name);
            bindingAttr ^= BindingFlags.DeclaredOnly;
            for (int i = 0; i < constructorList.Count; i++)
            {
                RuntimeConstructorInfo methodBase = constructorList[i];
                if ((((bindingAttr & methodBase.BindingFlags) == methodBase.BindingFlags) && FilterApplyMethodBaseInfo(methodBase, bindingAttr, callConv, types)) && (!flag || FilterApplyPrefixLookup(methodBase, name, flag2)))
                {
                    list.Add(methodBase);
                }
            }
            return list.ToArray();
        }

        protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            ConstructorInfo[] match = this.GetConstructorCandidates(null, bindingAttr, CallingConventions.Any, types, false);
            if (binder == null)
            {
                binder = Type.DefaultBinder;
            }
            if (match.Length == 0)
            {
                return null;
            }
            if ((types.Length == 0) && (match.Length == 1))
            {
                ParameterInfo[] parametersNoCopy = match[0].GetParametersNoCopy();
                if ((parametersNoCopy == null) || (parametersNoCopy.Length == 0))
                {
                    return match[0];
                }
            }
            if ((bindingAttr & BindingFlags.ExactBinding) != BindingFlags.Default)
            {
                return (DefaultBinder.ExactBinding(match, types, modifiers) as ConstructorInfo);
            }
            return (binder.SelectMethod(bindingAttr, match, types, modifiers) as ConstructorInfo);
        }

        [ComVisible(true)]
        public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
        {
            return this.GetConstructorCandidates(null, bindingAttr, CallingConventions.Any, null, false);
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

        public override Type GetElementType()
        {
            return this.GetTypeHandleInternal().GetElementType().GetRuntimeType();
        }

        public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
        {
            bool flag;
            MemberListType type;
            if (name == null)
            {
                throw new ArgumentNullException();
            }
            FilterHelper(bindingAttr, ref name, out flag, out type);
            CerArrayList<RuntimeEventInfo> eventList = this.Cache.GetEventList(type, name);
            EventInfo info = null;
            bindingAttr ^= BindingFlags.DeclaredOnly;
            for (int i = 0; i < eventList.Count; i++)
            {
                RuntimeEventInfo info2 = eventList[i];
                if ((bindingAttr & info2.BindingFlags) == info2.BindingFlags)
                {
                    if (info != null)
                    {
                        throw new AmbiguousMatchException(Environment.GetResourceString("RFLCT.Ambiguous"));
                    }
                    info = info2;
                }
            }
            return info;
        }

        private EventInfo[] GetEventCandidates(string name, BindingFlags bindingAttr, bool allowPrefixLookup)
        {
            bool flag;
            bool flag2;
            MemberListType type;
            FilterHelper(bindingAttr, ref name, allowPrefixLookup, out flag, out flag2, out type);
            List<EventInfo> list = new List<EventInfo>();
            CerArrayList<RuntimeEventInfo> eventList = this.Cache.GetEventList(type, name);
            bindingAttr ^= BindingFlags.DeclaredOnly;
            for (int i = 0; i < eventList.Count; i++)
            {
                RuntimeEventInfo memberInfo = eventList[i];
                if (((bindingAttr & memberInfo.BindingFlags) == memberInfo.BindingFlags) && (!flag || FilterApplyPrefixLookup(memberInfo, name, flag2)))
                {
                    list.Add(memberInfo);
                }
            }
            return list.ToArray();
        }

        public override EventInfo[] GetEvents(BindingFlags bindingAttr)
        {
            return this.GetEventCandidates(null, bindingAttr, false);
        }

        public override FieldInfo GetField(string name, BindingFlags bindingAttr)
        {
            bool flag;
            MemberListType type;
            if (name == null)
            {
                throw new ArgumentNullException();
            }
            FilterHelper(bindingAttr, ref name, out flag, out type);
            CerArrayList<RuntimeFieldInfo> fieldList = this.Cache.GetFieldList(type, name);
            FieldInfo info = null;
            bindingAttr ^= BindingFlags.DeclaredOnly;
            bool flag2 = false;
            for (int i = 0; i < fieldList.Count; i++)
            {
                RuntimeFieldInfo info2 = fieldList[i];
                if ((bindingAttr & info2.BindingFlags) == info2.BindingFlags)
                {
                    if (info != null)
                    {
                        if (info2.DeclaringType == info.DeclaringType)
                        {
                            throw new AmbiguousMatchException(Environment.GetResourceString("RFLCT.Ambiguous"));
                        }
                        if (info.DeclaringType.IsInterface && info2.DeclaringType.IsInterface)
                        {
                            flag2 = true;
                        }
                    }
                    if (((info == null) || info2.DeclaringType.IsSubclassOf(info.DeclaringType)) || info.DeclaringType.IsInterface)
                    {
                        info = info2;
                    }
                }
            }
            if (flag2 && info.DeclaringType.IsInterface)
            {
                throw new AmbiguousMatchException(Environment.GetResourceString("RFLCT.Ambiguous"));
            }
            return info;
        }

        private FieldInfo[] GetFieldCandidates(string name, BindingFlags bindingAttr, bool allowPrefixLookup)
        {
            bool flag;
            bool flag2;
            MemberListType type;
            FilterHelper(bindingAttr, ref name, allowPrefixLookup, out flag, out flag2, out type);
            List<FieldInfo> list = new List<FieldInfo>();
            CerArrayList<RuntimeFieldInfo> fieldList = this.Cache.GetFieldList(type, name);
            bindingAttr ^= BindingFlags.DeclaredOnly;
            for (int i = 0; i < fieldList.Count; i++)
            {
                RuntimeFieldInfo memberInfo = fieldList[i];
                if (((bindingAttr & memberInfo.BindingFlags) == memberInfo.BindingFlags) && (!flag || FilterApplyPrefixLookup(memberInfo, name, flag2)))
                {
                    list.Add(memberInfo);
                }
            }
            return list.ToArray();
        }

        internal static FieldInfo GetFieldInfo(RuntimeFieldHandle fieldHandle)
        {
            return GetFieldInfo(fieldHandle.GetApproxDeclaringType(), fieldHandle);
        }

        internal static FieldInfo GetFieldInfo(RuntimeTypeHandle reflectedTypeHandle, RuntimeFieldHandle fieldHandle)
        {
            if (reflectedTypeHandle.IsNullHandle())
            {
                reflectedTypeHandle = fieldHandle.GetApproxDeclaringType();
            }
            else
            {
                RuntimeTypeHandle approxDeclaringType = fieldHandle.GetApproxDeclaringType();
                if (!reflectedTypeHandle.Equals(approxDeclaringType) && (!fieldHandle.AcquiresContextFromThis() || !approxDeclaringType.GetCanonicalHandle().Equals(reflectedTypeHandle.GetCanonicalHandle())))
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_ResolveFieldHandle"), new object[] { reflectedTypeHandle.GetRuntimeType().ToString(), approxDeclaringType.GetRuntimeType().ToString() }));
                }
            }
            return reflectedTypeHandle.GetRuntimeType().Cache.GetField(fieldHandle);
        }

        public override FieldInfo[] GetFields(BindingFlags bindingAttr)
        {
            return this.GetFieldCandidates(null, bindingAttr, false);
        }

        public override Type[] GetGenericArguments()
        {
            Type[] typeArray = null;
            RuntimeTypeHandle[] instantiation = this.GetRootElementType().GetTypeHandleInternal().GetInstantiation();
            if (instantiation != null)
            {
                typeArray = new Type[instantiation.Length];
                for (int i = 0; i < instantiation.Length; i++)
                {
                    typeArray[i] = instantiation[i].GetRuntimeType();
                }
                return typeArray;
            }
            return new Type[0];
        }

        public override Type[] GetGenericParameterConstraints()
        {
            if (!this.IsGenericParameter)
            {
                throw new InvalidOperationException(Environment.GetResourceString("Arg_NotGenericParameter"));
            }
            RuntimeTypeHandle[] constraints = this.m_handle.GetConstraints();
            Type[] typeArray = new Type[constraints.Length];
            for (int i = 0; i < typeArray.Length; i++)
            {
                typeArray[i] = constraints[i].GetRuntimeType();
            }
            return typeArray;
        }

        public override Type GetGenericTypeDefinition()
        {
            if (!this.IsGenericType)
            {
                throw new InvalidOperationException();
            }
            return this.m_handle.GetGenericTypeDefinition().GetRuntimeType();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void GetGUID(ref Guid result);
        public override int GetHashCode()
        {
            return (int) this.GetTypeHandleInternal().Value;
        }

        public override Type GetInterface(string fullname, bool ignoreCase)
        {
            string str;
            string str2;
            MemberListType type;
            if (fullname == null)
            {
                throw new ArgumentNullException();
            }
            BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Public;
            bindingFlags &= ~BindingFlags.Static;
            if (ignoreCase)
            {
                bindingFlags |= BindingFlags.IgnoreCase;
            }
            SplitName(fullname, out str, out str2);
            FilterHelper(bindingFlags, ref str, out ignoreCase, out type);
            CerArrayList<RuntimeType> interfaceList = this.Cache.GetInterfaceList(type, str);
            RuntimeType type2 = null;
            for (int i = 0; i < interfaceList.Count; i++)
            {
                RuntimeType type3 = interfaceList[i];
                if (FilterApplyType(type3, bindingFlags, str, false, str2))
                {
                    if (type2 != null)
                    {
                        throw new AmbiguousMatchException(Environment.GetResourceString("RFLCT.Ambiguous"));
                    }
                    type2 = type3;
                }
            }
            return type2;
        }

        public override InterfaceMapping GetInterfaceMap(Type ifaceType)
        {
            InterfaceMapping mapping;
            if (this.IsGenericParameter)
            {
                throw new InvalidOperationException(Environment.GetResourceString("Arg_GenericParameter"));
            }
            if (ifaceType == null)
            {
                throw new ArgumentNullException("ifaceType");
            }
            if (!(ifaceType is RuntimeType))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeType"), "ifaceType");
            }
            RuntimeType type = ifaceType as RuntimeType;
            RuntimeTypeHandle typeHandleInternal = type.GetTypeHandleInternal();
            int firstSlotForInterface = this.GetTypeHandleInternal().GetFirstSlotForInterface(type.GetTypeHandleInternal());
            int interfaceMethodSlots = typeHandleInternal.GetInterfaceMethodSlots();
            int num3 = 0;
            for (int i = 0; i < interfaceMethodSlots; i++)
            {
                if ((typeHandleInternal.GetMethodAt(i).GetAttributes() & MethodAttributes.Static) != MethodAttributes.PrivateScope)
                {
                    num3++;
                }
            }
            int num5 = interfaceMethodSlots - num3;
            mapping.InterfaceType = ifaceType;
            mapping.TargetType = this;
            mapping.InterfaceMethods = new MethodInfo[num5];
            mapping.TargetMethods = new MethodInfo[num5];
            for (int j = 0; j < interfaceMethodSlots; j++)
            {
                RuntimeMethodHandle methodAt = typeHandleInternal.GetMethodAt(j);
                if ((typeHandleInternal.GetMethodAt(j).GetAttributes() & MethodAttributes.Static) == MethodAttributes.PrivateScope)
                {
                    int interfaceMethodImplementationSlot;
                    if (typeHandleInternal.HasInstantiation() && !typeHandleInternal.IsGenericTypeDefinition())
                    {
                        methodAt = methodAt.GetInstantiatingStubIfNeeded(typeHandleInternal);
                    }
                    MethodBase methodBase = GetMethodBase(typeHandleInternal, methodAt);
                    mapping.InterfaceMethods[j] = (MethodInfo) methodBase;
                    if (firstSlotForInterface == -1)
                    {
                        interfaceMethodImplementationSlot = this.GetTypeHandleInternal().GetInterfaceMethodImplementationSlot(typeHandleInternal, methodAt);
                    }
                    else
                    {
                        interfaceMethodImplementationSlot = firstSlotForInterface + j;
                    }
                    if (interfaceMethodImplementationSlot != -1)
                    {
                        RuntimeTypeHandle declaringTypeHandle = this.GetTypeHandleInternal();
                        RuntimeMethodHandle methodHandle = declaringTypeHandle.GetMethodAt(interfaceMethodImplementationSlot);
                        if (declaringTypeHandle.HasInstantiation() && !declaringTypeHandle.IsGenericTypeDefinition())
                        {
                            methodHandle = methodHandle.GetInstantiatingStubIfNeeded(declaringTypeHandle);
                        }
                        MethodBase base3 = GetMethodBase(declaringTypeHandle, methodHandle);
                        mapping.TargetMethods[j] = (MethodInfo) base3;
                    }
                }
            }
            return mapping;
        }

        public override Type[] GetInterfaces()
        {
            CerArrayList<RuntimeType> interfaceList = this.Cache.GetInterfaceList(MemberListType.All, null);
            Type[] target = new Type[interfaceList.Count];
            for (int i = 0; i < interfaceList.Count; i++)
            {
                JitHelpers.UnsafeSetArrayElement(target, i, interfaceList[i]);
            }
            return target;
        }

        public override MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
        {
            if (name == null)
            {
                throw new ArgumentNullException();
            }
            MethodInfo[] sourceArray = new MethodInfo[0];
            ConstructorInfo[] infoArray2 = new ConstructorInfo[0];
            PropertyInfo[] infoArray3 = new PropertyInfo[0];
            EventInfo[] infoArray4 = new EventInfo[0];
            FieldInfo[] infoArray5 = new FieldInfo[0];
            Type[] typeArray = new Type[0];
            if ((type & MemberTypes.Method) != 0)
            {
                sourceArray = this.GetMethodCandidates(name, bindingAttr, CallingConventions.Any, null, true);
            }
            if ((type & MemberTypes.Constructor) != 0)
            {
                infoArray2 = this.GetConstructorCandidates(name, bindingAttr, CallingConventions.Any, null, true);
            }
            if ((type & MemberTypes.Property) != 0)
            {
                infoArray3 = this.GetPropertyCandidates(name, bindingAttr, null, true);
            }
            if ((type & MemberTypes.Event) != 0)
            {
                infoArray4 = this.GetEventCandidates(name, bindingAttr, true);
            }
            if ((type & MemberTypes.Field) != 0)
            {
                infoArray5 = this.GetFieldCandidates(name, bindingAttr, true);
            }
            if ((type & (MemberTypes.NestedType | MemberTypes.TypeInfo)) != 0)
            {
                typeArray = this.GetNestedTypeCandidates(name, bindingAttr, true);
            }
            switch (type)
            {
                case MemberTypes.Constructor:
                    return infoArray2;

                case MemberTypes.Event:
                    return infoArray4;

                case MemberTypes.Field:
                    return infoArray5;

                case MemberTypes.Method:
                    return sourceArray;

                case (MemberTypes.Method | MemberTypes.Constructor):
                {
                    MethodBase[] baseArray = new MethodBase[sourceArray.Length + infoArray2.Length];
                    Array.Copy(sourceArray, baseArray, sourceArray.Length);
                    Array.Copy(infoArray2, 0, baseArray, sourceArray.Length, infoArray2.Length);
                    return baseArray;
                }
                case MemberTypes.Property:
                    return infoArray3;

                case MemberTypes.TypeInfo:
                    return typeArray;

                case MemberTypes.NestedType:
                    return typeArray;
            }
            MemberInfo[] destinationArray = new MemberInfo[((((sourceArray.Length + infoArray2.Length) + infoArray3.Length) + infoArray4.Length) + infoArray5.Length) + typeArray.Length];
            int destinationIndex = 0;
            if (sourceArray.Length > 0)
            {
                Array.Copy(sourceArray, 0, destinationArray, destinationIndex, sourceArray.Length);
            }
            destinationIndex += sourceArray.Length;
            if (infoArray2.Length > 0)
            {
                Array.Copy(infoArray2, 0, destinationArray, destinationIndex, infoArray2.Length);
            }
            destinationIndex += infoArray2.Length;
            if (infoArray3.Length > 0)
            {
                Array.Copy(infoArray3, 0, destinationArray, destinationIndex, infoArray3.Length);
            }
            destinationIndex += infoArray3.Length;
            if (infoArray4.Length > 0)
            {
                Array.Copy(infoArray4, 0, destinationArray, destinationIndex, infoArray4.Length);
            }
            destinationIndex += infoArray4.Length;
            if (infoArray5.Length > 0)
            {
                Array.Copy(infoArray5, 0, destinationArray, destinationIndex, infoArray5.Length);
            }
            destinationIndex += infoArray5.Length;
            if (typeArray.Length > 0)
            {
                Array.Copy(typeArray, 0, destinationArray, destinationIndex, typeArray.Length);
            }
            destinationIndex += typeArray.Length;
            return destinationArray;
        }

        public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
        {
            MethodInfo[] sourceArray = this.GetMethodCandidates(null, bindingAttr, CallingConventions.Any, null, false);
            ConstructorInfo[] infoArray2 = this.GetConstructorCandidates(null, bindingAttr, CallingConventions.Any, null, false);
            PropertyInfo[] infoArray3 = this.GetPropertyCandidates(null, bindingAttr, null, false);
            EventInfo[] infoArray4 = this.GetEventCandidates(null, bindingAttr, false);
            FieldInfo[] infoArray5 = this.GetFieldCandidates(null, bindingAttr, false);
            Type[] typeArray = this.GetNestedTypeCandidates(null, bindingAttr, false);
            MemberInfo[] destinationArray = new MemberInfo[((((sourceArray.Length + infoArray2.Length) + infoArray3.Length) + infoArray4.Length) + infoArray5.Length) + typeArray.Length];
            int destinationIndex = 0;
            Array.Copy(sourceArray, 0, destinationArray, destinationIndex, sourceArray.Length);
            destinationIndex += sourceArray.Length;
            Array.Copy(infoArray2, 0, destinationArray, destinationIndex, infoArray2.Length);
            destinationIndex += infoArray2.Length;
            Array.Copy(infoArray3, 0, destinationArray, destinationIndex, infoArray3.Length);
            destinationIndex += infoArray3.Length;
            Array.Copy(infoArray4, 0, destinationArray, destinationIndex, infoArray4.Length);
            destinationIndex += infoArray4.Length;
            Array.Copy(infoArray5, 0, destinationArray, destinationIndex, infoArray5.Length);
            destinationIndex += infoArray5.Length;
            Array.Copy(typeArray, 0, destinationArray, destinationIndex, typeArray.Length);
            destinationIndex += typeArray.Length;
            return destinationArray;
        }

        internal static MethodBase GetMethodBase(RuntimeMethodHandle methodHandle)
        {
            return GetMethodBase(RuntimeTypeHandle.EmptyHandle, methodHandle);
        }

        internal static MethodBase GetMethodBase(ModuleHandle scope, int typeMetadataToken)
        {
            return GetMethodBase(scope.ResolveMethodHandle(typeMetadataToken));
        }

        internal static MethodBase GetMethodBase(System.Reflection.Module scope, int typeMetadataToken)
        {
            return GetMethodBase(scope.GetModuleHandle(), typeMetadataToken);
        }

        internal static MethodBase GetMethodBase(RuntimeTypeHandle reflectedTypeHandle, RuntimeMethodHandle methodHandle)
        {
            if (methodHandle.IsDynamicMethod())
            {
                Resolver resolver = methodHandle.GetResolver();
                if (resolver != null)
                {
                    return resolver.GetDynamicMethod();
                }
                return null;
            }
            Type runtimeType = methodHandle.GetDeclaringType().GetRuntimeType();
            RuntimeType c = reflectedTypeHandle.GetRuntimeType();
            RuntimeTypeHandle[] methodInstantiation = null;
            bool flag = false;
            if (c == null)
            {
                c = runtimeType as RuntimeType;
            }
            if (c.IsArray)
            {
                MethodBase[] baseArray = c.GetMember(methodHandle.GetName(), MemberTypes.Method | MemberTypes.Constructor, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance) as MethodBase[];
                bool flag2 = false;
                for (int i = 0; i < baseArray.Length; i++)
                {
                    if (baseArray[i].GetMethodHandle() == methodHandle)
                    {
                        flag2 = true;
                    }
                }
                if (!flag2)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_ResolveMethodHandle"), new object[] { c.ToString(), runtimeType.ToString() }));
                }
                runtimeType = c;
            }
            else if (!runtimeType.IsAssignableFrom(c))
            {
                if (!runtimeType.IsGenericType)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_ResolveMethodHandle"), new object[] { c.ToString(), runtimeType.ToString() }));
                }
                Type genericTypeDefinition = runtimeType.GetGenericTypeDefinition();
                Type baseType = c;
                while (baseType != null)
                {
                    Type type5 = baseType;
                    if (type5.IsGenericType && !baseType.IsGenericTypeDefinition)
                    {
                        type5 = type5.GetGenericTypeDefinition();
                    }
                    if (type5.Equals(genericTypeDefinition))
                    {
                        break;
                    }
                    baseType = baseType.BaseType;
                }
                if (baseType == null)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_ResolveMethodHandle"), new object[] { c.ToString(), runtimeType.ToString() }));
                }
                runtimeType = baseType;
                methodInstantiation = methodHandle.GetMethodInstantiation();
                bool flag3 = methodHandle.IsGenericMethodDefinition();
                methodHandle = methodHandle.GetMethodFromCanonical(runtimeType.GetTypeHandleInternal());
                if (!flag3)
                {
                    flag = true;
                }
            }
            if (runtimeType.IsValueType)
            {
                methodHandle = methodHandle.GetUnboxingStub();
            }
            if (flag || ((runtimeType.GetTypeHandleInternal().HasInstantiation() && !runtimeType.GetTypeHandleInternal().IsGenericTypeDefinition()) && !methodHandle.HasMethodInstantiation()))
            {
                methodHandle = methodHandle.GetInstantiatingStub(runtimeType.GetTypeHandleInternal(), methodInstantiation);
            }
            if (methodHandle.IsConstructor())
            {
                return c.Cache.GetConstructor(runtimeType.GetTypeHandleInternal(), methodHandle);
            }
            if (methodHandle.HasMethodInstantiation() && !methodHandle.IsGenericMethodDefinition())
            {
                return c.Cache.GetGenericMethodInfo(methodHandle);
            }
            return c.Cache.GetMethod(runtimeType.GetTypeHandleInternal(), methodHandle);
        }

        private MethodInfo[] GetMethodCandidates(string name, BindingFlags bindingAttr, CallingConventions callConv, Type[] types, bool allowPrefixLookup)
        {
            bool flag;
            bool flag2;
            MemberListType type;
            FilterHelper(bindingAttr, ref name, allowPrefixLookup, out flag, out flag2, out type);
            List<MethodInfo> list = new List<MethodInfo>();
            CerArrayList<RuntimeMethodInfo> methodList = this.Cache.GetMethodList(type, name);
            bindingAttr ^= BindingFlags.DeclaredOnly;
            for (int i = 0; i < methodList.Count; i++)
            {
                RuntimeMethodInfo methodBase = methodList[i];
                if ((((bindingAttr & methodBase.BindingFlags) == methodBase.BindingFlags) && FilterApplyMethodBaseInfo(methodBase, bindingAttr, callConv, types)) && (!flag || FilterApplyPrefixLookup(methodBase, name, flag2)))
                {
                    list.Add(methodBase);
                }
            }
            return list.ToArray();
        }

        protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConv, Type[] types, ParameterModifier[] modifiers)
        {
            MethodInfo[] match = this.GetMethodCandidates(name, bindingAttr, callConv, types, false);
            if (match.Length == 0)
            {
                return null;
            }
            if ((types == null) || (types.Length == 0))
            {
                if (match.Length == 1)
                {
                    return match[0];
                }
                if (types == null)
                {
                    for (int i = 1; i < match.Length; i++)
                    {
                        MethodInfo info = match[i];
                        if (!DefaultBinder.CompareMethodSigAndName(info, match[0]))
                        {
                            throw new AmbiguousMatchException(Environment.GetResourceString("RFLCT.Ambiguous"));
                        }
                    }
                    return (DefaultBinder.FindMostDerivedNewSlotMeth(match, match.Length) as MethodInfo);
                }
            }
            if (binder == null)
            {
                binder = Type.DefaultBinder;
            }
            return (binder.SelectMethod(bindingAttr, match, types, modifiers) as MethodInfo);
        }

        public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
        {
            return this.GetMethodCandidates(null, bindingAttr, CallingConventions.Any, null, false);
        }

        public override Type GetNestedType(string fullname, BindingFlags bindingAttr)
        {
            bool flag;
            string str;
            string str2;
            MemberListType type;
            if (fullname == null)
            {
                throw new ArgumentNullException();
            }
            bindingAttr &= ~BindingFlags.Static;
            SplitName(fullname, out str, out str2);
            FilterHelper(bindingAttr, ref str, out flag, out type);
            CerArrayList<RuntimeType> nestedTypeList = this.Cache.GetNestedTypeList(type, str);
            RuntimeType type2 = null;
            for (int i = 0; i < nestedTypeList.Count; i++)
            {
                RuntimeType type3 = nestedTypeList[i];
                if (FilterApplyType(type3, bindingAttr, str, false, str2))
                {
                    if (type2 != null)
                    {
                        throw new AmbiguousMatchException(Environment.GetResourceString("RFLCT.Ambiguous"));
                    }
                    type2 = type3;
                }
            }
            return type2;
        }

        private Type[] GetNestedTypeCandidates(string fullname, BindingFlags bindingAttr, bool allowPrefixLookup)
        {
            bool flag;
            bool flag2;
            string str;
            string str2;
            MemberListType type;
            bindingAttr &= ~BindingFlags.Static;
            SplitName(fullname, out str, out str2);
            FilterHelper(bindingAttr, ref str, allowPrefixLookup, out flag, out flag2, out type);
            List<Type> list = new List<Type>();
            CerArrayList<RuntimeType> nestedTypeList = this.Cache.GetNestedTypeList(type, str);
            for (int i = 0; i < nestedTypeList.Count; i++)
            {
                RuntimeType type2 = nestedTypeList[i];
                if (FilterApplyType(type2, bindingAttr, str, flag, str2))
                {
                    list.Add(type2);
                }
            }
            return list.ToArray();
        }

        public override Type[] GetNestedTypes(BindingFlags bindingAttr)
        {
            return this.GetNestedTypeCandidates(null, bindingAttr, false);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            UnitySerializationHolder.GetUnitySerializationInfo(info, this);
        }

        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
        {
            return this.GetPropertyCandidates(null, bindingAttr, null, false);
        }

        private PropertyInfo[] GetPropertyCandidates(string name, BindingFlags bindingAttr, Type[] types, bool allowPrefixLookup)
        {
            bool flag;
            bool flag2;
            MemberListType type;
            FilterHelper(bindingAttr, ref name, allowPrefixLookup, out flag, out flag2, out type);
            List<PropertyInfo> list = new List<PropertyInfo>();
            CerArrayList<RuntimePropertyInfo> propertyList = this.Cache.GetPropertyList(type, name);
            bindingAttr ^= BindingFlags.DeclaredOnly;
            for (int i = 0; i < propertyList.Count; i++)
            {
                RuntimePropertyInfo memberInfo = propertyList[i];
                if ((((bindingAttr & memberInfo.BindingFlags) == memberInfo.BindingFlags) && (!flag || FilterApplyPrefixLookup(memberInfo, name, flag2))) && ((types == null) || (memberInfo.GetIndexParameters().Length == types.Length)))
                {
                    list.Add(memberInfo);
                }
            }
            return list.ToArray();
        }

        protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
        {
            if (name == null)
            {
                throw new ArgumentNullException();
            }
            PropertyInfo[] match = this.GetPropertyCandidates(name, bindingAttr, types, false);
            if (binder == null)
            {
                binder = Type.DefaultBinder;
            }
            if (match.Length == 0)
            {
                return null;
            }
            if ((types == null) || (types.Length == 0))
            {
                if (match.Length == 1)
                {
                    if ((returnType != null) && (returnType != match[0].PropertyType))
                    {
                        return null;
                    }
                    return match[0];
                }
                if (returnType == null)
                {
                    throw new AmbiguousMatchException(Environment.GetResourceString("RFLCT.Ambiguous"));
                }
            }
            if ((bindingAttr & BindingFlags.ExactBinding) != BindingFlags.Default)
            {
                return DefaultBinder.ExactPropertyBinding(match, returnType, types, modifiers);
            }
            return binder.SelectProperty(bindingAttr, match, returnType, types, modifiers);
        }

        internal static PropertyInfo GetPropertyInfo(RuntimeTypeHandle reflectedTypeHandle, int tkProperty)
        {
            RuntimePropertyInfo info = null;
            CerArrayList<RuntimePropertyInfo> propertyList = reflectedTypeHandle.GetRuntimeType().Cache.GetPropertyList(MemberListType.All, null);
            for (int i = 0; i < propertyList.Count; i++)
            {
                info = propertyList[i];
                if (info.MetadataToken == tkProperty)
                {
                    return info;
                }
            }
            throw new SystemException();
        }

        internal override TypeCode GetTypeCodeInternal()
        {
            TypeCode typeCode = this.Cache.TypeCode;
            if (typeCode == TypeCode.Empty)
            {
                switch (this.GetTypeHandleInternal().GetCorElementType())
                {
                    case CorElementType.Boolean:
                        typeCode = TypeCode.Boolean;
                        break;

                    case CorElementType.Char:
                        typeCode = TypeCode.Char;
                        break;

                    case CorElementType.I1:
                        typeCode = TypeCode.SByte;
                        break;

                    case CorElementType.U1:
                        typeCode = TypeCode.Byte;
                        break;

                    case CorElementType.I2:
                        typeCode = TypeCode.Int16;
                        break;

                    case CorElementType.U2:
                        typeCode = TypeCode.UInt16;
                        break;

                    case CorElementType.I4:
                        typeCode = TypeCode.Int32;
                        break;

                    case CorElementType.U4:
                        typeCode = TypeCode.UInt32;
                        break;

                    case CorElementType.I8:
                        typeCode = TypeCode.Int64;
                        break;

                    case CorElementType.U8:
                        typeCode = TypeCode.UInt64;
                        break;

                    case CorElementType.R4:
                        typeCode = TypeCode.Single;
                        break;

                    case CorElementType.R8:
                        typeCode = TypeCode.Double;
                        break;

                    case CorElementType.String:
                        typeCode = TypeCode.String;
                        break;

                    case CorElementType.ValueType:
                        if (this != Convert.ConvertTypes[15])
                        {
                            if (this == Convert.ConvertTypes[0x10])
                            {
                                typeCode = TypeCode.DateTime;
                            }
                            else if (base.IsEnum)
                            {
                                typeCode = Type.GetTypeCode(Enum.GetUnderlyingType(this));
                            }
                            else
                            {
                                typeCode = TypeCode.Object;
                            }
                        }
                        else
                        {
                            typeCode = TypeCode.Decimal;
                        }
                        break;

                    default:
                        if (this == Convert.ConvertTypes[2])
                        {
                            typeCode = TypeCode.DBNull;
                        }
                        else if (this == Convert.ConvertTypes[0x12])
                        {
                            typeCode = TypeCode.String;
                        }
                        else
                        {
                            typeCode = TypeCode.Object;
                        }
                        break;
                }
                this.Cache.TypeCode = typeCode;
            }
            return typeCode;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern Type GetTypeFromCLSIDImpl(Guid clsid, string server, bool throwOnError);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern Type GetTypeFromProgIDImpl(string progID, string server, bool throwOnError);
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        internal override RuntimeTypeHandle GetTypeHandleInternal()
        {
            return this.m_handle;
        }

        protected override bool HasElementTypeImpl()
        {
            if (!base.IsArray && !base.IsPointer)
            {
                return base.IsByRef;
            }
            return true;
        }

        internal override bool HasProxyAttributeImpl()
        {
            return this.GetTypeHandleInternal().HasProxyAttribute();
        }

        internal void InvalidateCachedNestedType()
        {
            this.Cache.InvalidateCachedNestedType();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern object InvokeDispMethod(string name, BindingFlags invokeAttr, object target, object[] args, bool[] byrefModifiers, int culture, string[] namedParameters);
        [DebuggerStepThrough, DebuggerHidden]
        public override object InvokeMember(string name, BindingFlags bindingFlags, Binder binder, object target, object[] providedArgs, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParams)
        {
            if (this.IsGenericParameter)
            {
                throw new InvalidOperationException(Environment.GetResourceString("Arg_GenericParameter"));
            }
            if ((bindingFlags & (BindingFlags.PutRefDispProperty | BindingFlags.PutDispProperty | BindingFlags.SetProperty | BindingFlags.GetProperty | BindingFlags.SetField | BindingFlags.GetField | BindingFlags.CreateInstance | BindingFlags.InvokeMethod)) == BindingFlags.Default)
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_NoAccessSpec"), "bindingFlags");
            }
            if ((bindingFlags & 0xff) == BindingFlags.Default)
            {
                bindingFlags |= BindingFlags.Public | BindingFlags.Instance;
                if ((bindingFlags & BindingFlags.CreateInstance) == BindingFlags.Default)
                {
                    bindingFlags |= BindingFlags.Static;
                }
            }
            if (namedParams != null)
            {
                if (providedArgs != null)
                {
                    if (namedParams.Length > providedArgs.Length)
                    {
                        throw new ArgumentException(Environment.GetResourceString("Arg_NamedParamTooBig"), "namedParams");
                    }
                }
                else if (namedParams.Length != 0)
                {
                    throw new ArgumentException(Environment.GetResourceString("Arg_NamedParamTooBig"), "namedParams");
                }
            }
            if ((target != null) && target.GetType().IsCOMObject)
            {
                int lCID;
                if ((bindingFlags & (BindingFlags.PutRefDispProperty | BindingFlags.PutDispProperty | BindingFlags.SetProperty | BindingFlags.GetProperty | BindingFlags.InvokeMethod)) == BindingFlags.Default)
                {
                    throw new ArgumentException(Environment.GetResourceString("Arg_COMAccess"), "bindingFlags");
                }
                if (((bindingFlags & BindingFlags.GetProperty) != BindingFlags.Default) && (((bindingFlags & (BindingFlags.PutRefDispProperty | BindingFlags.PutDispProperty | BindingFlags.SetProperty | BindingFlags.GetProperty | BindingFlags.InvokeMethod)) & ~(BindingFlags.GetProperty | BindingFlags.InvokeMethod)) != BindingFlags.Default))
                {
                    throw new ArgumentException(Environment.GetResourceString("Arg_PropSetGet"), "bindingFlags");
                }
                if (((bindingFlags & BindingFlags.InvokeMethod) != BindingFlags.Default) && (((bindingFlags & (BindingFlags.PutRefDispProperty | BindingFlags.PutDispProperty | BindingFlags.SetProperty | BindingFlags.GetProperty | BindingFlags.InvokeMethod)) & ~(BindingFlags.GetProperty | BindingFlags.InvokeMethod)) != BindingFlags.Default))
                {
                    throw new ArgumentException(Environment.GetResourceString("Arg_PropSetInvoke"), "bindingFlags");
                }
                if (((bindingFlags & BindingFlags.SetProperty) != BindingFlags.Default) && (((bindingFlags & (BindingFlags.PutRefDispProperty | BindingFlags.PutDispProperty | BindingFlags.SetProperty | BindingFlags.GetProperty | BindingFlags.InvokeMethod)) & ~BindingFlags.SetProperty) != BindingFlags.Default))
                {
                    throw new ArgumentException(Environment.GetResourceString("Arg_COMPropSetPut"), "bindingFlags");
                }
                if (((bindingFlags & BindingFlags.PutDispProperty) != BindingFlags.Default) && (((bindingFlags & (BindingFlags.PutRefDispProperty | BindingFlags.PutDispProperty | BindingFlags.SetProperty | BindingFlags.GetProperty | BindingFlags.InvokeMethod)) & ~BindingFlags.PutDispProperty) != BindingFlags.Default))
                {
                    throw new ArgumentException(Environment.GetResourceString("Arg_COMPropSetPut"), "bindingFlags");
                }
                if (((bindingFlags & BindingFlags.PutRefDispProperty) != BindingFlags.Default) && (((bindingFlags & (BindingFlags.PutRefDispProperty | BindingFlags.PutDispProperty | BindingFlags.SetProperty | BindingFlags.GetProperty | BindingFlags.InvokeMethod)) & ~BindingFlags.PutRefDispProperty) != BindingFlags.Default))
                {
                    throw new ArgumentException(Environment.GetResourceString("Arg_COMPropSetPut"), "bindingFlags");
                }
                if (RemotingServices.IsTransparentProxy(target))
                {
                    return ((MarshalByRefObject) target).InvokeMember(name, bindingFlags, binder, providedArgs, modifiers, culture, namedParams);
                }
                if (name == null)
                {
                    throw new ArgumentNullException("name");
                }
                bool[] byrefModifiers = (modifiers == null) ? null : modifiers[0].IsByRefArray;
                if (culture == null)
                {
                    lCID = forceInvokingWithEnUS ? 0x409 : Thread.CurrentThread.CurrentCulture.LCID;
                }
                else
                {
                    lCID = culture.LCID;
                }
                return this.InvokeDispMethod(name, bindingFlags, target, providedArgs, byrefModifiers, lCID, namedParams);
            }
            if ((namedParams != null) && (Array.IndexOf<string>(namedParams, null) != -1))
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_NamedParamNull"), "namedParams");
            }
            int num2 = (providedArgs != null) ? providedArgs.Length : 0;
            if (binder == null)
            {
                binder = Type.DefaultBinder;
            }
            Binder defaultBinder = Type.DefaultBinder;
            if ((bindingFlags & BindingFlags.CreateInstance) != BindingFlags.Default)
            {
                if (((bindingFlags & BindingFlags.CreateInstance) != BindingFlags.Default) && ((bindingFlags & (BindingFlags.SetProperty | BindingFlags.GetProperty | BindingFlags.SetField | BindingFlags.GetField | BindingFlags.InvokeMethod)) != BindingFlags.Default))
                {
                    throw new ArgumentException(Environment.GetResourceString("Arg_CreatInstAccess"), "bindingFlags");
                }
                return Activator.CreateInstance(this, bindingFlags, binder, providedArgs, culture);
            }
            if ((bindingFlags & (BindingFlags.PutRefDispProperty | BindingFlags.PutDispProperty)) != BindingFlags.Default)
            {
                bindingFlags |= BindingFlags.SetProperty;
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if ((name.Length == 0) || name.Equals("[DISPID=0]"))
            {
                name = this.GetDefaultMemberName();
                if (name == null)
                {
                    name = "ToString";
                }
            }
            bool flag = (bindingFlags & BindingFlags.GetField) != BindingFlags.Default;
            bool flag2 = (bindingFlags & BindingFlags.SetField) != BindingFlags.Default;
            if (flag || flag2)
            {
                if (flag)
                {
                    if (flag2)
                    {
                        throw new ArgumentException(Environment.GetResourceString("Arg_FldSetGet"), "bindingFlags");
                    }
                    if ((bindingFlags & BindingFlags.SetProperty) != BindingFlags.Default)
                    {
                        throw new ArgumentException(Environment.GetResourceString("Arg_FldGetPropSet"), "bindingFlags");
                    }
                }
                else
                {
                    if (providedArgs == null)
                    {
                        throw new ArgumentNullException("providedArgs");
                    }
                    if ((bindingFlags & BindingFlags.GetProperty) != BindingFlags.Default)
                    {
                        throw new ArgumentException(Environment.GetResourceString("Arg_FldSetPropGet"), "bindingFlags");
                    }
                    if ((bindingFlags & BindingFlags.InvokeMethod) != BindingFlags.Default)
                    {
                        throw new ArgumentException(Environment.GetResourceString("Arg_FldSetInvoke"), "bindingFlags");
                    }
                }
                FieldInfo info = null;
                FieldInfo[] match = this.GetMember(name, MemberTypes.Field, bindingFlags) as FieldInfo[];
                if (match.Length == 1)
                {
                    info = match[0];
                }
                else if (match.Length > 0)
                {
                    info = binder.BindToField(bindingFlags, match, flag ? Empty.Value : providedArgs[0], culture);
                }
                if (info != null)
                {
                    if (info.FieldType.IsArray || (info.FieldType == typeof(Array)))
                    {
                        int num3;
                        if ((bindingFlags & BindingFlags.GetField) != BindingFlags.Default)
                        {
                            num3 = num2;
                        }
                        else
                        {
                            num3 = num2 - 1;
                        }
                        if (num3 > 0)
                        {
                            int[] indices = new int[num3];
                            for (int i = 0; i < num3; i++)
                            {
                                try
                                {
                                    indices[i] = ((IConvertible) providedArgs[i]).ToInt32(null);
                                }
                                catch (InvalidCastException)
                                {
                                    throw new ArgumentException(Environment.GetResourceString("Arg_IndexMustBeInt"));
                                }
                            }
                            Array array = (Array) info.GetValue(target);
                            if ((bindingFlags & BindingFlags.GetField) != BindingFlags.Default)
                            {
                                return array.GetValue(indices);
                            }
                            array.SetValue(providedArgs[num3], indices);
                            return null;
                        }
                    }
                    if (flag)
                    {
                        if (num2 != 0)
                        {
                            throw new ArgumentException(Environment.GetResourceString("Arg_FldGetArgErr"), "bindingFlags");
                        }
                        return info.GetValue(target);
                    }
                    if (num2 != 1)
                    {
                        throw new ArgumentException(Environment.GetResourceString("Arg_FldSetArgErr"), "bindingFlags");
                    }
                    info.SetValue(target, providedArgs[0], bindingFlags, binder, culture);
                    return null;
                }
                if ((bindingFlags & 0xfff300) == BindingFlags.Default)
                {
                    throw new MissingFieldException(this.FullName, name);
                }
            }
            bool flag3 = (bindingFlags & BindingFlags.GetProperty) != BindingFlags.Default;
            bool flag4 = (bindingFlags & BindingFlags.SetProperty) != BindingFlags.Default;
            if (flag3 || flag4)
            {
                if (flag3)
                {
                    if (flag4)
                    {
                        throw new ArgumentException(Environment.GetResourceString("Arg_PropSetGet"), "bindingFlags");
                    }
                }
                else if ((bindingFlags & BindingFlags.InvokeMethod) != BindingFlags.Default)
                {
                    throw new ArgumentException(Environment.GetResourceString("Arg_PropSetInvoke"), "bindingFlags");
                }
            }
            MethodInfo[] infoArray2 = null;
            MethodInfo info2 = null;
            if ((bindingFlags & BindingFlags.InvokeMethod) != BindingFlags.Default)
            {
                MethodInfo[] infoArray3 = this.GetMember(name, MemberTypes.Method, bindingFlags) as MethodInfo[];
                ArrayList list = null;
                for (int j = 0; j < infoArray3.Length; j++)
                {
                    MethodInfo methodBase = infoArray3[j];
                    if (FilterApplyMethodBaseInfo(methodBase, bindingFlags, null, CallingConventions.Any, new Type[num2], false))
                    {
                        if (info2 == null)
                        {
                            info2 = methodBase;
                        }
                        else
                        {
                            if (list == null)
                            {
                                list = new ArrayList(infoArray3.Length);
                                list.Add(info2);
                            }
                            list.Add(methodBase);
                        }
                    }
                }
                if (list != null)
                {
                    infoArray2 = new MethodInfo[list.Count];
                    list.CopyTo(infoArray2);
                }
            }
            if (((info2 == null) && flag3) || flag4)
            {
                PropertyInfo[] infoArray4 = this.GetMember(name, MemberTypes.Property, bindingFlags) as PropertyInfo[];
                ArrayList list2 = null;
                for (int k = 0; k < infoArray4.Length; k++)
                {
                    MethodInfo setMethod = null;
                    if (flag4)
                    {
                        setMethod = infoArray4[k].GetSetMethod(true);
                    }
                    else
                    {
                        setMethod = infoArray4[k].GetGetMethod(true);
                    }
                    if ((setMethod != null) && FilterApplyMethodBaseInfo(setMethod, bindingFlags, null, CallingConventions.Any, new Type[num2], false))
                    {
                        if (info2 == null)
                        {
                            info2 = setMethod;
                        }
                        else
                        {
                            if (list2 == null)
                            {
                                list2 = new ArrayList(infoArray4.Length);
                                list2.Add(info2);
                            }
                            list2.Add(setMethod);
                        }
                    }
                }
                if (list2 != null)
                {
                    infoArray2 = new MethodInfo[list2.Count];
                    list2.CopyTo(infoArray2);
                }
            }
            if (info2 == null)
            {
                throw new MissingMethodException(this.FullName, name);
            }
            if (((infoArray2 == null) && (num2 == 0)) && ((info2.GetParametersNoCopy().Length == 0) && ((bindingFlags & BindingFlags.OptionalParamBinding) == BindingFlags.Default)))
            {
                return info2.Invoke(target, bindingFlags, binder, providedArgs, culture);
            }
            if (infoArray2 == null)
            {
                infoArray2 = new MethodInfo[] { info2 };
            }
            if (providedArgs == null)
            {
                providedArgs = new object[0];
            }
            object state = null;
            MethodBase base2 = null;
            try
            {
                base2 = binder.BindToMethod(bindingFlags, infoArray2, ref providedArgs, modifiers, culture, namedParams, out state);
            }
            catch (MissingMethodException)
            {
            }
            if (base2 == null)
            {
                throw new MissingMethodException(this.FullName, name);
            }
            object obj3 = ((MethodInfo) base2).Invoke(target, bindingFlags, binder, providedArgs, culture);
            if (state != null)
            {
                binder.ReorderArgumentArray(ref providedArgs, state);
            }
            return obj3;
        }

        protected override bool IsArrayImpl()
        {
            CorElementType corElementType = this.GetTypeHandleInternal().GetCorElementType();
            if (corElementType != CorElementType.Array)
            {
                return (corElementType == CorElementType.SzArray);
            }
            return true;
        }

        protected override bool IsByRefImpl()
        {
            return (this.GetTypeHandleInternal().GetCorElementType() == CorElementType.ByRef);
        }

        protected override bool IsCOMObjectImpl()
        {
            return this.GetTypeHandleInternal().IsComObject(false);
        }

        protected override bool IsContextfulImpl()
        {
            return this.GetTypeHandleInternal().IsContextful();
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

        internal bool IsGenericCOMObjectImpl()
        {
            return this.m_handle.IsComObject(true);
        }

        public override bool IsInstanceOfType(object o)
        {
            return this.GetTypeHandleInternal().IsInstanceOfType(o);
        }

        protected override bool IsPointerImpl()
        {
            return (this.GetTypeHandleInternal().GetCorElementType() == CorElementType.Ptr);
        }

        protected override bool IsPrimitiveImpl()
        {
            CorElementType corElementType = this.GetTypeHandleInternal().GetCorElementType();
            if (((corElementType < CorElementType.Boolean) || (corElementType > CorElementType.R8)) && (corElementType != CorElementType.I))
            {
                return (corElementType == CorElementType.U);
            }
            return true;
        }

        [ComVisible(true)]
        public override bool IsSubclassOf(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            for (Type type2 = this.BaseType; type2 != null; type2 = type2.BaseType)
            {
                if (type2 == type)
                {
                    return true;
                }
            }
            return ((type == typeof(object)) && (type != this));
        }

        public override Type MakeArrayType()
        {
            return this.m_handle.MakeSZArray().GetRuntimeType();
        }

        public override Type MakeArrayType(int rank)
        {
            if (rank <= 0)
            {
                throw new IndexOutOfRangeException();
            }
            return this.m_handle.MakeArray(rank).GetRuntimeType();
        }

        public override Type MakeByRefType()
        {
            return this.m_handle.MakeByRef().GetRuntimeType();
        }

        public override Type MakeGenericType(Type[] instantiation)
        {
            if (instantiation == null)
            {
                throw new ArgumentNullException("instantiation");
            }
            Type[] typeArray = new Type[instantiation.Length];
            for (int i = 0; i < instantiation.Length; i++)
            {
                typeArray[i] = instantiation[i];
            }
            instantiation = typeArray;
            if (!this.IsGenericTypeDefinition)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Arg_NotGenericTypeDefinition"), new object[] { this }));
            }
            for (int j = 0; j < instantiation.Length; j++)
            {
                if (instantiation[j] == null)
                {
                    throw new ArgumentNullException();
                }
                if (!(instantiation[j] is RuntimeType))
                {
                    return new TypeBuilderInstantiation(this, instantiation);
                }
            }
            Type[] genericArguments = this.GetGenericArguments();
            SanityCheckGenericArguments(instantiation, genericArguments);
            RuntimeTypeHandle[] inst = new RuntimeTypeHandle[instantiation.Length];
            for (int k = 0; k < instantiation.Length; k++)
            {
                inst[k] = instantiation[k].GetTypeHandleInternal();
            }
            Type runtimeType = null;
            try
            {
                runtimeType = this.m_handle.Instantiate(inst).GetRuntimeType();
            }
            catch (TypeLoadException exception)
            {
                ValidateGenericArguments(this, instantiation, exception);
                throw exception;
            }
            return runtimeType;
        }

        public override Type MakePointerType()
        {
            return this.m_handle.MakePointer().GetRuntimeType();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void PrepareMemberInfoCache(RuntimeTypeHandle rt);
        internal static Type PrivateGetType(string typeName, bool throwOnError, bool ignoreCase, ref StackCrawlMark stackMark)
        {
            return PrivateGetType(typeName, throwOnError, ignoreCase, false, ref stackMark);
        }

        internal static Type PrivateGetType(string typeName, bool throwOnError, bool ignoreCase, bool reflectionOnly, ref StackCrawlMark stackMark)
        {
            if (typeName == null)
            {
                throw new ArgumentNullException("TypeName");
            }
            return RuntimeTypeHandle.GetTypeByName(typeName, throwOnError, ignoreCase, reflectionOnly, ref stackMark).GetRuntimeType();
        }

        internal static void SanityCheckGenericArguments(Type[] genericArguments, Type[] genericParamters)
        {
            if (genericArguments == null)
            {
                throw new ArgumentNullException();
            }
            for (int i = 0; i < genericArguments.Length; i++)
            {
                if (genericArguments[i] == null)
                {
                    throw new ArgumentNullException();
                }
                ThrowIfTypeNeverValidGenericArgument(genericArguments[i]);
            }
            if (genericArguments.Length != genericParamters.Length)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_NotEnoughGenArguments", new object[] { genericArguments.Length, genericParamters.Length }), new object[0]));
            }
        }

        private static void SplitName(string fullname, out string name, out string ns)
        {
            name = null;
            ns = null;
            if (fullname != null)
            {
                int length = fullname.LastIndexOf(".", StringComparison.Ordinal);
                if (length != -1)
                {
                    ns = fullname.Substring(0, length);
                    int num2 = (fullname.Length - ns.Length) - 1;
                    if (num2 != 0)
                    {
                        name = fullname.Substring(length + 1, num2);
                    }
                    else
                    {
                        name = "";
                    }
                }
                else
                {
                    name = fullname;
                }
            }
        }

        internal bool SupportsInterface(object o)
        {
            return this.TypeHandle.SupportsInterface(o);
        }

        private static void ThrowIfTypeNeverValidGenericArgument(Type type)
        {
            if ((type.IsPointer || type.IsByRef) || (type == typeof(void)))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_NeverValidGenericArgument"), new object[] { type.ToString() }));
            }
        }

        public override string ToString()
        {
            return this.Cache.GetToString();
        }

        internal static void ValidateGenericArguments(MemberInfo definition, Type[] genericArguments, Exception e)
        {
            RuntimeTypeHandle[] typeContext = null;
            RuntimeTypeHandle[] methodContext = null;
            Type[] typeArray = null;
            if (definition is Type)
            {
                typeArray = ((Type) definition).GetGenericArguments();
                typeContext = new RuntimeTypeHandle[genericArguments.Length];
                for (int j = 0; j < genericArguments.Length; j++)
                {
                    typeContext[j] = genericArguments[j].GetTypeHandleInternal();
                }
            }
            else
            {
                MethodInfo info = (MethodInfo) definition;
                typeArray = info.GetGenericArguments();
                methodContext = new RuntimeTypeHandle[genericArguments.Length];
                for (int k = 0; k < genericArguments.Length; k++)
                {
                    methodContext[k] = genericArguments[k].GetTypeHandleInternal();
                }
                Type declaringType = info.DeclaringType;
                if (declaringType != null)
                {
                    typeContext = declaringType.GetTypeHandleInternal().GetInstantiation();
                }
            }
            for (int i = 0; i < genericArguments.Length; i++)
            {
                Type type3 = genericArguments[i];
                Type type4 = typeArray[i];
                if (!type4.GetTypeHandleInternal().SatisfiesConstraints(typeContext, methodContext, type3.GetTypeHandleInternal()))
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_GenConstraintViolation"), new object[] { i.ToString(CultureInfo.CurrentCulture), type3.ToString(), definition.ToString(), type4.ToString() }), e);
                }
            }
        }

        private void WrapArgsForInvokeCall(object[] aArgs, int[] aWrapperTypes)
        {
            int length = aArgs.Length;
            for (int i = 0; i < length; i++)
            {
                if (aWrapperTypes[i] == 0)
                {
                    continue;
                }
                if ((aWrapperTypes[i] & 0x10000) != 0)
                {
                    ConstructorInfo constructor;
                    Type elementType = null;
                    bool flag = false;
                    switch ((((DispatchWrapperType) aWrapperTypes[i]) & ~DispatchWrapperType.SafeArray))
                    {
                        case DispatchWrapperType.Currency:
                            elementType = typeof(CurrencyWrapper);
                            break;

                        case DispatchWrapperType.BStr:
                            elementType = typeof(BStrWrapper);
                            flag = true;
                            break;

                        case DispatchWrapperType.Unknown:
                            elementType = typeof(UnknownWrapper);
                            break;

                        case DispatchWrapperType.Dispatch:
                            elementType = typeof(DispatchWrapper);
                            break;

                        case DispatchWrapperType.Error:
                            elementType = typeof(ErrorWrapper);
                            break;
                    }
                    Array array = (Array) aArgs[i];
                    int num3 = array.Length;
                    object[] objArray = (object[]) Array.CreateInstance(elementType, num3);
                    if (flag)
                    {
                        constructor = elementType.GetConstructor(new Type[] { typeof(string) });
                    }
                    else
                    {
                        constructor = elementType.GetConstructor(new Type[] { typeof(object) });
                    }
                    for (int j = 0; j < num3; j++)
                    {
                        if (flag)
                        {
                            objArray[j] = constructor.Invoke(new object[] { (string) array.GetValue(j) });
                        }
                        else
                        {
                            objArray[j] = constructor.Invoke(new object[] { array.GetValue(j) });
                        }
                    }
                    aArgs[i] = objArray;
                    continue;
                }
                switch (((DispatchWrapperType) aWrapperTypes[i]))
                {
                    case DispatchWrapperType.Currency:
                        aArgs[i] = new CurrencyWrapper(aArgs[i]);
                        break;

                    case DispatchWrapperType.BStr:
                        aArgs[i] = new BStrWrapper((string) aArgs[i]);
                        break;

                    case DispatchWrapperType.Unknown:
                        aArgs[i] = new UnknownWrapper(aArgs[i]);
                        break;

                    case DispatchWrapperType.Dispatch:
                        aArgs[i] = new DispatchWrapper(aArgs[i]);
                        break;

                    case DispatchWrapperType.Error:
                        aArgs[i] = new ErrorWrapper(aArgs[i]);
                        break;
                }
            }
        }

        public override System.Reflection.Assembly Assembly
        {
            get
            {
                return this.GetTypeHandleInternal().GetAssemblyHandle().GetAssembly();
            }
        }

        public override string AssemblyQualifiedName
        {
            get
            {
                if (!this.IsGenericTypeDefinition && this.ContainsGenericParameters)
                {
                    return null;
                }
                return System.Reflection.Assembly.CreateQualifiedName(this.Assembly.FullName, this.FullName);
            }
        }

        public override Type BaseType
        {
            get
            {
                if (base.IsInterface)
                {
                    return null;
                }
                if (!this.m_handle.IsGenericVariable())
                {
                    return this.m_handle.GetBaseTypeHandle().GetRuntimeType();
                }
                Type[] genericParameterConstraints = this.GetGenericParameterConstraints();
                Type type = typeof(object);
                for (int i = 0; i < genericParameterConstraints.Length; i++)
                {
                    Type type2 = genericParameterConstraints[i];
                    if (!type2.IsInterface)
                    {
                        if (type2.IsGenericParameter)
                        {
                            System.Reflection.GenericParameterAttributes attributes = type2.GenericParameterAttributes & System.Reflection.GenericParameterAttributes.SpecialConstraintMask;
                            if (((attributes & System.Reflection.GenericParameterAttributes.ReferenceTypeConstraint) == System.Reflection.GenericParameterAttributes.None) && ((attributes & System.Reflection.GenericParameterAttributes.NotNullableValueTypeConstraint) == System.Reflection.GenericParameterAttributes.None))
                            {
                                continue;
                            }
                        }
                        type = type2;
                    }
                }
                if (type == typeof(object))
                {
                    System.Reflection.GenericParameterAttributes attributes2 = this.GenericParameterAttributes & System.Reflection.GenericParameterAttributes.SpecialConstraintMask;
                    if ((attributes2 & System.Reflection.GenericParameterAttributes.NotNullableValueTypeConstraint) != System.Reflection.GenericParameterAttributes.None)
                    {
                        type = typeof(ValueType);
                    }
                }
                return type;
            }
        }

        private RuntimeTypeCache Cache
        {
            get
            {
                if (this.m_cache.IsNull())
                {
                    IntPtr gCHandle = this.m_handle.GetGCHandle(GCHandleType.WeakTrackResurrection);
                    if (!Interlocked.CompareExchange(ref this.m_cache, gCHandle, IntPtr.Zero).IsNull())
                    {
                        this.m_handle.FreeGCHandle(gCHandle);
                    }
                }
                RuntimeTypeCache cache = GCHandle.InternalGet(this.m_cache) as RuntimeTypeCache;
                if (cache == null)
                {
                    cache = new RuntimeTypeCache(this);
                    RuntimeTypeCache cache2 = GCHandle.InternalCompareExchange(this.m_cache, cache, null, false) as RuntimeTypeCache;
                    if (cache2 != null)
                    {
                        cache = cache2;
                    }
                    if (s_typeCache == null)
                    {
                        s_typeCache = new TypeCacheQueue();
                    }
                }
                return cache;
            }
        }

        public override bool ContainsGenericParameters
        {
            get
            {
                return this.GetRootElementType().GetTypeHandleInternal().ContainsGenericVariables();
            }
        }

        public override MethodBase DeclaringMethod
        {
            get
            {
                if (!this.IsGenericParameter)
                {
                    throw new InvalidOperationException(Environment.GetResourceString("Arg_NotGenericParameter"));
                }
                RuntimeMethodHandle declaringMethod = this.GetTypeHandleInternal().GetDeclaringMethod();
                if (declaringMethod.IsNullHandle())
                {
                    return null;
                }
                return GetMethodBase(declaringMethod.GetDeclaringType(), declaringMethod);
            }
        }

        public override Type DeclaringType
        {
            get
            {
                return this.Cache.GetEnclosingType();
            }
        }

        internal bool DomainInitialized
        {
            get
            {
                return this.Cache.DomainInitialized;
            }
            set
            {
                this.Cache.DomainInitialized = value;
            }
        }

        private OleAutBinder ForwardCallBinder
        {
            get
            {
                if (s_ForwardCallBinder == null)
                {
                    s_ForwardCallBinder = new OleAutBinder();
                }
                return s_ForwardCallBinder;
            }
        }

        public override string FullName
        {
            get
            {
                return this.Cache.GetFullName();
            }
        }

        public override System.Reflection.GenericParameterAttributes GenericParameterAttributes
        {
            get
            {
                System.Reflection.GenericParameterAttributes attributes;
                if (!this.IsGenericParameter)
                {
                    throw new InvalidOperationException(Environment.GetResourceString("Arg_NotGenericParameter"));
                }
                this.GetTypeHandleInternal().GetModuleHandle().GetMetadataImport().GetGenericParamProps(this.MetadataToken, out attributes);
                return attributes;
            }
        }

        public override int GenericParameterPosition
        {
            get
            {
                if (!this.IsGenericParameter)
                {
                    throw new InvalidOperationException(Environment.GetResourceString("Arg_NotGenericParameter"));
                }
                return this.m_handle.GetGenericVariableIndex();
            }
        }

        public override Guid GUID
        {
            get
            {
                Guid result = new Guid();
                this.GetGUID(ref result);
                return result;
            }
        }

        public override bool IsGenericParameter
        {
            get
            {
                return this.m_handle.IsGenericVariable();
            }
        }

        public override bool IsGenericType
        {
            get
            {
                return (!base.HasElementType && this.GetTypeHandleInternal().HasInstantiation());
            }
        }

        public override bool IsGenericTypeDefinition
        {
            get
            {
                return this.m_handle.IsGenericTypeDefinition();
            }
        }

        internal override bool IsSzArray
        {
            get
            {
                return (this.GetTypeHandleInternal().GetCorElementType() == CorElementType.SzArray);
            }
        }

        public override MemberTypes MemberType
        {
            get
            {
                if (!base.IsPublic && !base.IsNotPublic)
                {
                    return MemberTypes.NestedType;
                }
                return MemberTypes.TypeInfo;
            }
        }

        public override int MetadataToken
        {
            get
            {
                return this.m_handle.GetToken();
            }
        }

        public override System.Reflection.Module Module
        {
            get
            {
                return this.GetTypeHandleInternal().GetModuleHandle().GetModule();
            }
        }

        public override string Name
        {
            get
            {
                return this.Cache.GetName();
            }
        }

        public override string Namespace
        {
            get
            {
                string nameSpace = this.Cache.GetNameSpace();
                if ((nameSpace != null) && (nameSpace.Length != 0))
                {
                    return nameSpace;
                }
                return null;
            }
        }

        public override Type ReflectedType
        {
            get
            {
                return this.DeclaringType;
            }
        }

        public override System.Runtime.InteropServices.StructLayoutAttribute StructLayoutAttribute
        {
            get
            {
                return (System.Runtime.InteropServices.StructLayoutAttribute) System.Runtime.InteropServices.StructLayoutAttribute.GetCustomAttribute(this);
            }
        }

        public override RuntimeTypeHandle TypeHandle
        {
            get
            {
                return this.m_handle;
            }
        }

        public override Type UnderlyingSystemType
        {
            get
            {
                return this;
            }
        }

        private class ActivatorCache
        {
            private RuntimeType.ActivatorCacheEntry[] cache = new RuntimeType.ActivatorCacheEntry[0x10];
            private const int CACHE_SIZE = 0x10;
            private PermissionSet delegateCreatePermissions;
            private ConstructorInfo delegateCtorInfo;
            private int hash_counter;

            internal RuntimeType.ActivatorCacheEntry GetEntry(Type t)
            {
                int index = this.hash_counter;
                for (int i = 0; i < 0x10; i++)
                {
                    RuntimeType.ActivatorCacheEntry ace = this.cache[index];
                    if ((ace != null) && (ace.m_type == t))
                    {
                        if (!ace.m_bFullyInitialized)
                        {
                            this.InitializeCacheEntry(ace);
                        }
                        return ace;
                    }
                    index = (index + 1) & 15;
                }
                return null;
            }

            private void InitializeCacheEntry(RuntimeType.ActivatorCacheEntry ace)
            {
                if (!ace.m_type.IsValueType)
                {
                    if (this.delegateCtorInfo == null)
                    {
                        this.InitializeDelegateCreator();
                    }
                    this.delegateCreatePermissions.Assert();
                    object[] parameters = new object[2];
                    parameters[1] = ace.m_hCtorMethodHandle.GetFunctionPointer();
                    CtorDelegate delegate2 = (CtorDelegate) this.delegateCtorInfo.Invoke(parameters);
                    Thread.MemoryBarrier();
                    ace.m_ctor = delegate2;
                }
                ace.m_bFullyInitialized = true;
            }

            private void InitializeDelegateCreator()
            {
                PermissionSet set = new PermissionSet(PermissionState.None);
                set.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.MemberAccess));
                set.AddPermission(new SecurityPermission(SecurityPermissionFlag.UnmanagedCode));
                Thread.MemoryBarrier();
                this.delegateCreatePermissions = set;
                ConstructorInfo constructor = typeof(CtorDelegate).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) });
                Thread.MemoryBarrier();
                this.delegateCtorInfo = constructor;
            }

            internal void SetEntry(RuntimeType.ActivatorCacheEntry ace)
            {
                int index = (this.hash_counter - 1) & 15;
                this.hash_counter = index;
                this.cache[index] = ace;
            }
        }

        private class ActivatorCacheEntry
        {
            internal bool m_bFullyInitialized;
            internal bool m_bNeedSecurityCheck;
            internal CtorDelegate m_ctor;
            internal RuntimeMethodHandle m_hCtorMethodHandle;
            internal Type m_type;

            internal ActivatorCacheEntry(Type t, RuntimeMethodHandle rmh, bool bNeedSecurityCheck)
            {
                this.m_type = t;
                this.m_bNeedSecurityCheck = bNeedSecurityCheck;
                this.m_hCtorMethodHandle = rmh;
            }
        }

        [Flags]
        private enum DispatchWrapperType
        {
            BStr = 0x20,
            Currency = 0x10,
            Dispatch = 2,
            Error = 8,
            Record = 4,
            SafeArray = 0x10000,
            Unknown = 1
        }

        [Serializable]
        internal class RuntimeTypeCache
        {
            private bool m_bIsDomainInitialized;
            private MemberInfoCache<RuntimeConstructorInfo> m_constructorInfoCache;
            private System.RuntimeType m_enclosingType;
            private MemberInfoCache<RuntimeEventInfo> m_eventInfoCache;
            private MemberInfoCache<RuntimeFieldInfo> m_fieldInfoCache;
            private string m_fullname;
            private MemberInfoCache<System.RuntimeType> m_interfaceCache;
            private bool m_isGlobal;
            private MemberInfoCache<RuntimeMethodInfo> m_methodInfoCache;
            private string m_name;
            private string m_namespace;
            private MemberInfoCache<System.RuntimeType> m_nestedClassesCache;
            private MemberInfoCache<RuntimePropertyInfo> m_propertyInfoCache;
            private System.RuntimeType m_runtimeType;
            private System.RuntimeTypeHandle m_runtimeTypeHandle;
            private string m_toString;
            private System.TypeCode m_typeCode = System.TypeCode.Empty;
            private WhatsCached m_whatsCached;
            private static bool s_dontrunhack;
            private static CerHashtable<RuntimeMethodInfo, RuntimeMethodInfo> s_methodInstantiations;

            internal RuntimeTypeCache(System.RuntimeType runtimeType)
            {
                this.m_runtimeType = runtimeType;
                this.m_runtimeTypeHandle = runtimeType.GetTypeHandleInternal();
                this.m_isGlobal = this.m_runtimeTypeHandle.GetModuleHandle().GetModuleTypeHandle().Equals(this.m_runtimeTypeHandle);
                s_dontrunhack = true;
                Prejitinit_HACK();
            }

            private string ConstructName(ref string name, bool nameSpace, bool fullinst, bool assembly)
            {
                if (name == null)
                {
                    name = this.RuntimeTypeHandle.ConstructName(nameSpace, fullinst, assembly);
                }
                return name;
            }

            internal MethodBase GetConstructor(System.RuntimeTypeHandle declaringType, RuntimeMethodHandle constructor)
            {
                this.GetMemberCache<RuntimeConstructorInfo>(ref this.m_constructorInfoCache);
                return this.m_constructorInfoCache.AddMethod(declaringType, constructor, CacheType.Constructor);
            }

            internal CerArrayList<RuntimeConstructorInfo> GetConstructorList(MemberListType listType, string name)
            {
                return this.GetMemberList<RuntimeConstructorInfo>(ref this.m_constructorInfoCache, listType, name, CacheType.Constructor);
            }

            internal System.RuntimeType GetEnclosingType()
            {
                if ((this.m_whatsCached & WhatsCached.EnclosingType) == WhatsCached.Nothing)
                {
                    this.m_enclosingType = this.RuntimeTypeHandle.GetDeclaringType().GetRuntimeType();
                    this.m_whatsCached |= WhatsCached.EnclosingType;
                }
                return this.m_enclosingType;
            }

            internal CerArrayList<RuntimeEventInfo> GetEventList(MemberListType listType, string name)
            {
                return this.GetMemberList<RuntimeEventInfo>(ref this.m_eventInfoCache, listType, name, CacheType.Event);
            }

            internal FieldInfo GetField(RuntimeFieldHandle field)
            {
                this.GetMemberCache<RuntimeFieldInfo>(ref this.m_fieldInfoCache);
                return this.m_fieldInfoCache.AddField(field);
            }

            internal CerArrayList<RuntimeFieldInfo> GetFieldList(MemberListType listType, string name)
            {
                return this.GetMemberList<RuntimeFieldInfo>(ref this.m_fieldInfoCache, listType, name, CacheType.Field);
            }

            internal string GetFullName()
            {
                if (!this.m_runtimeType.IsGenericTypeDefinition && this.m_runtimeType.ContainsGenericParameters)
                {
                    return null;
                }
                return this.ConstructName(ref this.m_fullname, true, true, false);
            }

            internal MethodInfo GetGenericMethodInfo(RuntimeMethodHandle genericMethod)
            {
                if (s_methodInstantiations == null)
                {
                    Interlocked.CompareExchange<CerHashtable<RuntimeMethodInfo, RuntimeMethodInfo>>(ref s_methodInstantiations, new CerHashtable<RuntimeMethodInfo, RuntimeMethodInfo>(), null);
                }
                RuntimeMethodInfo info = new RuntimeMethodInfo(genericMethod, genericMethod.GetDeclaringType(), this, genericMethod.GetAttributes(), ~BindingFlags.Default);
                RuntimeMethodInfo info2 = null;
                info2 = s_methodInstantiations[info];
                if (info2 != null)
                {
                    return info2;
                }
                bool tookLock = false;
                bool flag2 = false;
                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                    Monitor.ReliableEnter(s_methodInstantiations, ref tookLock);
                    info2 = s_methodInstantiations[info];
                    if (info2 != null)
                    {
                        return info2;
                    }
                    s_methodInstantiations.Preallocate(1);
                    flag2 = true;
                }
                finally
                {
                    if (flag2)
                    {
                        s_methodInstantiations[info] = info;
                    }
                    if (tookLock)
                    {
                        Monitor.Exit(s_methodInstantiations);
                    }
                }
                return info;
            }

            internal CerArrayList<System.RuntimeType> GetInterfaceList(MemberListType listType, string name)
            {
                return this.GetMemberList<System.RuntimeType>(ref this.m_interfaceCache, listType, name, CacheType.Interface);
            }

            private MemberInfoCache<T> GetMemberCache<T>(ref MemberInfoCache<T> m_cache) where T: MemberInfo
            {
                MemberInfoCache<T> cache = m_cache;
                if (cache == null)
                {
                    MemberInfoCache<T> cache2 = new MemberInfoCache<T>(this);
                    cache = Interlocked.CompareExchange<MemberInfoCache<T>>(ref m_cache, cache2, null);
                    if (cache == null)
                    {
                        cache = cache2;
                    }
                }
                return cache;
            }

            private CerArrayList<T> GetMemberList<T>(ref MemberInfoCache<T> m_cache, MemberListType listType, string name, CacheType cacheType) where T: MemberInfo
            {
                return this.GetMemberCache<T>(ref m_cache).GetMemberList(listType, name, cacheType);
            }

            internal MethodBase GetMethod(System.RuntimeTypeHandle declaringType, RuntimeMethodHandle method)
            {
                this.GetMemberCache<RuntimeMethodInfo>(ref this.m_methodInfoCache);
                return this.m_methodInfoCache.AddMethod(declaringType, method, CacheType.Method);
            }

            internal CerArrayList<RuntimeMethodInfo> GetMethodList(MemberListType listType, string name)
            {
                return this.GetMemberList<RuntimeMethodInfo>(ref this.m_methodInfoCache, listType, name, CacheType.Method);
            }

            internal string GetName()
            {
                return this.ConstructName(ref this.m_name, false, false, false);
            }

            internal string GetNameSpace()
            {
                if (this.m_namespace == null)
                {
                    Type rootElementType = this.m_runtimeType.GetRootElementType();
                    while (rootElementType.IsNested)
                    {
                        rootElementType = rootElementType.DeclaringType;
                    }
                    this.m_namespace = rootElementType.GetTypeHandleInternal().GetModuleHandle().GetMetadataImport().GetNamespace(rootElementType.MetadataToken).ToString();
                }
                return this.m_namespace;
            }

            internal CerArrayList<System.RuntimeType> GetNestedTypeList(MemberListType listType, string name)
            {
                return this.GetMemberList<System.RuntimeType>(ref this.m_nestedClassesCache, listType, name, CacheType.NestedType);
            }

            internal CerArrayList<RuntimePropertyInfo> GetPropertyList(MemberListType listType, string name)
            {
                return this.GetMemberList<RuntimePropertyInfo>(ref this.m_propertyInfoCache, listType, name, CacheType.Property);
            }

            internal string GetToString()
            {
                return this.ConstructName(ref this.m_toString, true, false, false);
            }

            internal void InvalidateCachedNestedType()
            {
                this.m_nestedClassesCache = null;
            }

            internal static void Prejitinit_HACK()
            {
                if (!s_dontrunhack)
                {
                    RuntimeHelpers.PrepareConstrainedRegions();
                    try
                    {
                    }
                    finally
                    {
                        MemberInfoCache<RuntimeMethodInfo> cache = new MemberInfoCache<RuntimeMethodInfo>(null);
                        CerArrayList<RuntimeMethodInfo> list = null;
                        cache.Insert(ref list, "dummy", MemberListType.All);
                        MemberInfoCache<RuntimeConstructorInfo> cache2 = new MemberInfoCache<RuntimeConstructorInfo>(null);
                        CerArrayList<RuntimeConstructorInfo> list2 = null;
                        cache2.Insert(ref list2, "dummy", MemberListType.All);
                        MemberInfoCache<RuntimeFieldInfo> cache3 = new MemberInfoCache<RuntimeFieldInfo>(null);
                        CerArrayList<RuntimeFieldInfo> list3 = null;
                        cache3.Insert(ref list3, "dummy", MemberListType.All);
                        MemberInfoCache<System.RuntimeType> cache4 = new MemberInfoCache<System.RuntimeType>(null);
                        CerArrayList<System.RuntimeType> list4 = null;
                        cache4.Insert(ref list4, "dummy", MemberListType.All);
                        MemberInfoCache<RuntimePropertyInfo> cache5 = new MemberInfoCache<RuntimePropertyInfo>(null);
                        CerArrayList<RuntimePropertyInfo> list5 = null;
                        cache5.Insert(ref list5, "dummy", MemberListType.All);
                        MemberInfoCache<RuntimeEventInfo> cache6 = new MemberInfoCache<RuntimeEventInfo>(null);
                        CerArrayList<RuntimeEventInfo> list6 = null;
                        cache6.Insert(ref list6, "dummy", MemberListType.All);
                    }
                }
            }

            internal bool DomainInitialized
            {
                get
                {
                    return this.m_bIsDomainInitialized;
                }
                set
                {
                    this.m_bIsDomainInitialized = value;
                }
            }

            internal bool IsGlobal
            {
                [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
                get
                {
                    return this.m_isGlobal;
                }
            }

            internal System.RuntimeType RuntimeType
            {
                get
                {
                    return this.m_runtimeType;
                }
            }

            internal System.RuntimeTypeHandle RuntimeTypeHandle
            {
                get
                {
                    return this.m_runtimeTypeHandle;
                }
            }

            internal System.TypeCode TypeCode
            {
                get
                {
                    return this.m_typeCode;
                }
                set
                {
                    this.m_typeCode = value;
                }
            }

            internal enum CacheType
            {
                Method,
                Constructor,
                Field,
                Property,
                Event,
                Interface,
                NestedType
            }

            [StructLayout(LayoutKind.Sequential)]
            private struct Filter
            {
                private Utf8String m_name;
                private MemberListType m_listType;
                public unsafe Filter(byte* pUtf8Name, int cUtf8Name, MemberListType listType)
                {
                    this.m_name = new Utf8String((void*) pUtf8Name, cUtf8Name);
                    this.m_listType = listType;
                }

                public bool Match(Utf8String name)
                {
                    if (this.m_listType == MemberListType.CaseSensitive)
                    {
                        return this.m_name.Equals(name);
                    }
                    if (this.m_listType == MemberListType.CaseInsensitive)
                    {
                        return this.m_name.EqualsCaseInsensitive(name);
                    }
                    return true;
                }
            }

            [Serializable]
            private class MemberInfoCache<T> where T: MemberInfo
            {
                private bool m_cacheComplete;
                private CerHashtable<string, CerArrayList<T>> m_cisMemberInfos;
                private CerHashtable<string, CerArrayList<T>> m_csMemberInfos;
                private CerArrayList<T> m_root;
                private RuntimeType.RuntimeTypeCache m_runtimeTypeCache;

                static MemberInfoCache()
                {
                    RuntimeType.PrepareMemberInfoCache(typeof(RuntimeType.RuntimeTypeCache.MemberInfoCache<T>).TypeHandle);
                }

                internal MemberInfoCache(RuntimeType.RuntimeTypeCache runtimeTypeCache)
                {
                    Mda.MemberInfoCacheCreation();
                    this.m_runtimeTypeCache = runtimeTypeCache;
                    this.m_cacheComplete = false;
                }

                private static void AddElementTypes(Type template, IList<Type> types)
                {
                    if (template.HasElementType)
                    {
                        RuntimeType.RuntimeTypeCache.MemberInfoCache<T>.AddElementTypes(template.GetElementType(), types);
                        for (int i = 0; i < types.Count; i++)
                        {
                            if (template.IsArray)
                            {
                                if (template.IsSzArray)
                                {
                                    types[i] = types[i].MakeArrayType();
                                }
                                else
                                {
                                    types[i] = types[i].MakeArrayType(template.GetArrayRank());
                                }
                            }
                            else if (template.IsPointer)
                            {
                                types[i] = types[i].MakePointerType();
                            }
                        }
                    }
                }

                internal FieldInfo AddField(RuntimeFieldHandle field)
                {
                    List<RuntimeFieldInfo> list = new List<RuntimeFieldInfo>(1);
                    FieldAttributes attributes = field.GetAttributes();
                    bool isPublic = (attributes & FieldAttributes.FieldAccessMask) == FieldAttributes.Public;
                    bool isStatic = (attributes & FieldAttributes.Static) != FieldAttributes.PrivateScope;
                    bool isInherited = field.GetApproxDeclaringType().Value != this.ReflectedTypeHandle.Value;
                    BindingFlags bindingFlags = RuntimeType.FilterPreCalculate(isPublic, isInherited, isStatic);
                    list.Add(new RtFieldInfo(field, this.ReflectedType, this.m_runtimeTypeCache, bindingFlags));
                    CerArrayList<T> list2 = new CerArrayList<T>((List<T>) list);
                    this.Insert(ref list2, null, MemberListType.HandleToInfo);
                    return (FieldInfo) list2[0];
                }

                internal MethodBase AddMethod(RuntimeTypeHandle declaringType, RuntimeMethodHandle method, RuntimeType.RuntimeTypeCache.CacheType cacheType)
                {
                    object obj2 = null;
                    MethodAttributes methodAttributes = method.GetAttributes();
                    bool isPublic = (methodAttributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public;
                    bool isStatic = (methodAttributes & MethodAttributes.Static) != MethodAttributes.PrivateScope;
                    bool isInherited = declaringType.Value != this.ReflectedTypeHandle.Value;
                    BindingFlags bindingFlags = RuntimeType.FilterPreCalculate(isPublic, isInherited, isStatic);
                    switch (cacheType)
                    {
                        case RuntimeType.RuntimeTypeCache.CacheType.Method:
                        {
                            List<RuntimeMethodInfo> list = new List<RuntimeMethodInfo>(1) {
                                new RuntimeMethodInfo(method, declaringType, this.m_runtimeTypeCache, methodAttributes, bindingFlags)
                            };
                            obj2 = list;
                            break;
                        }
                        case RuntimeType.RuntimeTypeCache.CacheType.Constructor:
                        {
                            List<RuntimeConstructorInfo> list2 = new List<RuntimeConstructorInfo>(1) {
                                new RuntimeConstructorInfo(method, declaringType, this.m_runtimeTypeCache, methodAttributes, bindingFlags)
                            };
                            obj2 = list2;
                            break;
                        }
                    }
                    CerArrayList<T> list3 = new CerArrayList<T>((List<T>) obj2);
                    this.Insert(ref list3, null, MemberListType.HandleToInfo);
                    return (MethodBase) list3[0];
                }

                internal CerArrayList<T> GetMemberList(MemberListType listType, string name, RuntimeType.RuntimeTypeCache.CacheType cacheType)
                {
                    CerArrayList<T> list = null;
                    switch (listType)
                    {
                        case MemberListType.All:
                            if (!this.m_cacheComplete)
                            {
                                return this.Populate(null, listType, cacheType);
                            }
                            return this.m_root;

                        case MemberListType.CaseSensitive:
                            if (this.m_csMemberInfos != null)
                            {
                                list = this.m_csMemberInfos[name];
                                if (list == null)
                                {
                                    return this.Populate(name, listType, cacheType);
                                }
                                return list;
                            }
                            return this.Populate(name, listType, cacheType);
                    }
                    if (this.m_cisMemberInfos == null)
                    {
                        return this.Populate(name, listType, cacheType);
                    }
                    list = this.m_cisMemberInfos[name];
                    if (list == null)
                    {
                        return this.Populate(name, listType, cacheType);
                    }
                    return list;
                }

                [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
                internal void Insert(ref CerArrayList<T> list, string name, MemberListType listType)
                {
                    bool tookLock = false;
                    bool flag2 = false;
                    RuntimeHelpers.PrepareConstrainedRegions();
                    try
                    {
                        Monitor.ReliableEnter(this, ref tookLock);
                        if (listType == MemberListType.CaseSensitive)
                        {
                            if (this.m_csMemberInfos == null)
                            {
                                this.m_csMemberInfos = new CerHashtable<string, CerArrayList<T>>();
                            }
                            else
                            {
                                this.m_csMemberInfos.Preallocate(1);
                            }
                        }
                        else if (listType == MemberListType.CaseInsensitive)
                        {
                            if (this.m_cisMemberInfos == null)
                            {
                                this.m_cisMemberInfos = new CerHashtable<string, CerArrayList<T>>();
                            }
                            else
                            {
                                this.m_cisMemberInfos.Preallocate(1);
                            }
                        }
                        if (this.m_root == null)
                        {
                            this.m_root = new CerArrayList<T>(list.Count);
                        }
                        else
                        {
                            this.m_root.Preallocate(list.Count);
                        }
                        flag2 = true;
                    }
                    finally
                    {
                        try
                        {
                            if (flag2)
                            {
                                if (listType == MemberListType.CaseSensitive)
                                {
                                    CerArrayList<T> list2 = this.m_csMemberInfos[name];
                                    if (list2 == null)
                                    {
                                        this.MergeWithGlobalList(list);
                                        this.m_csMemberInfos[name] = list;
                                    }
                                    else
                                    {
                                        list = list2;
                                    }
                                }
                                else if (listType == MemberListType.CaseInsensitive)
                                {
                                    CerArrayList<T> list3 = this.m_cisMemberInfos[name];
                                    if (list3 == null)
                                    {
                                        this.MergeWithGlobalList(list);
                                        this.m_cisMemberInfos[name] = list;
                                    }
                                    else
                                    {
                                        list = list3;
                                    }
                                }
                                else
                                {
                                    this.MergeWithGlobalList(list);
                                }
                                if (listType == MemberListType.All)
                                {
                                    this.m_cacheComplete = true;
                                }
                            }
                        }
                        finally
                        {
                            if (tookLock)
                            {
                                Monitor.Exit(this);
                            }
                        }
                    }
                }

                [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
                private void MergeWithGlobalList(CerArrayList<T> list)
                {
                    int count = this.m_root.Count;
                    for (int i = 0; i < list.Count; i++)
                    {
                        T local = list[i];
                        T o = default(T);
                        for (int j = 0; j < count; j++)
                        {
                            o = this.m_root[j];
                            if (local.CacheEquals(o))
                            {
                                list.Replace(i, o);
                                break;
                            }
                        }
                        if (list[i] != o)
                        {
                            this.m_root.Add(local);
                        }
                    }
                }

                private unsafe CerArrayList<T> Populate(string name, MemberListType listType, RuntimeType.RuntimeTypeCache.CacheType cacheType)
                {
                    if (((name == null) || (name.Length == 0)) || (((cacheType == RuntimeType.RuntimeTypeCache.CacheType.Constructor) && (name.FirstChar != '.')) && (name.FirstChar != '*')))
                    {
                        RuntimeType.RuntimeTypeCache.Filter filter = new RuntimeType.RuntimeTypeCache.Filter(null, 0, listType);
                        List<T> list = null;
                        switch (cacheType)
                        {
                            case RuntimeType.RuntimeTypeCache.CacheType.Method:
                                list = this.PopulateMethods(filter) as List<T>;
                                break;

                            case RuntimeType.RuntimeTypeCache.CacheType.Constructor:
                                list = this.PopulateConstructors(filter) as List<T>;
                                break;

                            case RuntimeType.RuntimeTypeCache.CacheType.Field:
                                list = this.PopulateFields(filter) as List<T>;
                                break;

                            case RuntimeType.RuntimeTypeCache.CacheType.Property:
                                list = this.PopulateProperties(filter) as List<T>;
                                break;

                            case RuntimeType.RuntimeTypeCache.CacheType.Event:
                                list = this.PopulateEvents(filter) as List<T>;
                                break;

                            case RuntimeType.RuntimeTypeCache.CacheType.Interface:
                                list = this.PopulateInterfaces(filter) as List<T>;
                                break;

                            case RuntimeType.RuntimeTypeCache.CacheType.NestedType:
                                list = this.PopulateNestedClasses(filter) as List<T>;
                                break;
                        }
                        CerArrayList<T> list2 = new CerArrayList<T>(list);
                        this.Insert(ref list2, name, listType);
                        return list2;
                    }
                    fixed (char* str = ((char*) name))
                    {
                        char* chars = str;
                        int byteCount = Encoding.UTF8.GetByteCount(chars, name.Length);
                        byte* bytes = stackalloc byte[1 * byteCount];
                        Encoding.UTF8.GetBytes(chars, name.Length, bytes, byteCount);
                        RuntimeType.RuntimeTypeCache.Filter filter2 = new RuntimeType.RuntimeTypeCache.Filter(bytes, byteCount, listType);
                        List<T> list3 = null;
                        switch (cacheType)
                        {
                            case RuntimeType.RuntimeTypeCache.CacheType.Method:
                                list3 = this.PopulateMethods(filter2) as List<T>;
                                break;

                            case RuntimeType.RuntimeTypeCache.CacheType.Constructor:
                                list3 = this.PopulateConstructors(filter2) as List<T>;
                                break;

                            case RuntimeType.RuntimeTypeCache.CacheType.Field:
                                list3 = this.PopulateFields(filter2) as List<T>;
                                break;

                            case RuntimeType.RuntimeTypeCache.CacheType.Property:
                                list3 = this.PopulateProperties(filter2) as List<T>;
                                break;

                            case RuntimeType.RuntimeTypeCache.CacheType.Event:
                                list3 = this.PopulateEvents(filter2) as List<T>;
                                break;

                            case RuntimeType.RuntimeTypeCache.CacheType.Interface:
                                list3 = this.PopulateInterfaces(filter2) as List<T>;
                                break;

                            case RuntimeType.RuntimeTypeCache.CacheType.NestedType:
                                list3 = this.PopulateNestedClasses(filter2) as List<T>;
                                break;
                        }
                        CerArrayList<T> list4 = new CerArrayList<T>(list3);
                        this.Insert(ref list4, name, listType);
                        return list4;
                    }
                }

                private List<RuntimeConstructorInfo> PopulateConstructors(RuntimeType.RuntimeTypeCache.Filter filter)
                {
                    List<RuntimeConstructorInfo> list = new List<RuntimeConstructorInfo>();
                    if (!this.ReflectedType.IsGenericParameter)
                    {
                        RuntimeTypeHandle reflectedTypeHandle = this.ReflectedTypeHandle;
                        bool flag = reflectedTypeHandle.HasInstantiation() && !reflectedTypeHandle.IsGenericTypeDefinition();
                        RuntimeTypeHandle.IntroducedMethodEnumerator enumerator = reflectedTypeHandle.IntroducedMethods.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            RuntimeMethodHandle current = enumerator.Current;
                            if (filter.Match(current.GetUtf8Name()))
                            {
                                MethodAttributes methodAttributes = current.GetAttributes();
                                if (((methodAttributes & MethodAttributes.RTSpecialName) != MethodAttributes.PrivateScope) && !current.IsILStub())
                                {
                                    bool isPublic = (methodAttributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public;
                                    bool isStatic = (methodAttributes & MethodAttributes.Static) != MethodAttributes.PrivateScope;
                                    bool isInherited = false;
                                    BindingFlags bindingFlags = RuntimeType.FilterPreCalculate(isPublic, isInherited, isStatic);
                                    RuntimeMethodHandle handle = flag ? current.GetInstantiatingStubIfNeeded(reflectedTypeHandle) : current;
                                    RuntimeConstructorInfo item = new RuntimeConstructorInfo(handle, this.ReflectedTypeHandle, this.m_runtimeTypeCache, methodAttributes, bindingFlags);
                                    list.Add(item);
                                }
                            }
                        }
                    }
                    return list;
                }

                private List<RuntimeEventInfo> PopulateEvents(RuntimeType.RuntimeTypeCache.Filter filter)
                {
                    Hashtable csEventInfos = new Hashtable();
                    RuntimeTypeHandle reflectedTypeHandle = this.ReflectedTypeHandle;
                    List<RuntimeEventInfo> list = new List<RuntimeEventInfo>();
                    if ((reflectedTypeHandle.GetAttributes() & TypeAttributes.ClassSemanticsMask) != TypeAttributes.ClassSemanticsMask)
                    {
                        while (reflectedTypeHandle.IsGenericVariable())
                        {
                            reflectedTypeHandle = reflectedTypeHandle.GetRuntimeType().BaseType.GetTypeHandleInternal();
                        }
                        while (!reflectedTypeHandle.IsNullHandle())
                        {
                            this.PopulateEvents(filter, reflectedTypeHandle, csEventInfos, list);
                            reflectedTypeHandle = reflectedTypeHandle.GetBaseTypeHandle();
                        }
                        return list;
                    }
                    this.PopulateEvents(filter, reflectedTypeHandle, csEventInfos, list);
                    return list;
                }

                private unsafe void PopulateEvents(RuntimeType.RuntimeTypeCache.Filter filter, RuntimeTypeHandle declaringTypeHandle, Hashtable csEventInfos, List<RuntimeEventInfo> list)
                {
                    int token = declaringTypeHandle.GetToken();
                    if (!MetadataToken.IsNullToken(token))
                    {
                        MetadataImport metadataImport = declaringTypeHandle.GetModuleHandle().GetMetadataImport();
                        int count = metadataImport.EnumEventsCount(token);
                        int* result = stackalloc int[count];
                        metadataImport.EnumEvents(token, result, count);
                        this.PopulateEvents(filter, declaringTypeHandle, metadataImport, result, count, csEventInfos, list);
                    }
                }

                private unsafe void PopulateEvents(RuntimeType.RuntimeTypeCache.Filter filter, RuntimeTypeHandle declaringTypeHandle, MetadataImport scope, int* tkAssociates, int cAssociates, Hashtable csEventInfos, List<RuntimeEventInfo> list)
                {
                    for (int i = 0; i < cAssociates; i++)
                    {
                        int mdToken = tkAssociates[i];
                        Utf8String name = scope.GetName(mdToken);
                        if (filter.Match(name))
                        {
                            bool flag;
                            RuntimeEventInfo item = new RuntimeEventInfo(mdToken, declaringTypeHandle.GetRuntimeType(), this.m_runtimeTypeCache, out flag);
                            if ((declaringTypeHandle.Equals(this.m_runtimeTypeCache.RuntimeTypeHandle) || !flag) && (csEventInfos[item.Name] == null))
                            {
                                csEventInfos[item.Name] = item;
                                list.Add(item);
                            }
                        }
                    }
                }

                private List<RuntimeFieldInfo> PopulateFields(RuntimeType.RuntimeTypeCache.Filter filter)
                {
                    List<RuntimeFieldInfo> list = new List<RuntimeFieldInfo>();
                    RuntimeTypeHandle reflectedTypeHandle = this.ReflectedTypeHandle;
                    while (reflectedTypeHandle.IsGenericVariable())
                    {
                        reflectedTypeHandle = reflectedTypeHandle.GetRuntimeType().BaseType.GetTypeHandleInternal();
                    }
                    while (!reflectedTypeHandle.IsNullHandle())
                    {
                        this.PopulateRtFields(filter, reflectedTypeHandle, list);
                        this.PopulateLiteralFields(filter, reflectedTypeHandle, list);
                        reflectedTypeHandle = reflectedTypeHandle.GetBaseTypeHandle();
                    }
                    if (this.ReflectedType.IsGenericParameter)
                    {
                        Type[] typeArray = this.ReflectedTypeHandle.GetRuntimeType().BaseType.GetInterfaces();
                        for (int i = 0; i < typeArray.Length; i++)
                        {
                            this.PopulateLiteralFields(filter, typeArray[i].GetTypeHandleInternal(), list);
                            this.PopulateRtFields(filter, typeArray[i].GetTypeHandleInternal(), list);
                        }
                        return list;
                    }
                    RuntimeTypeHandle[] interfaces = this.ReflectedTypeHandle.GetInterfaces();
                    if (interfaces != null)
                    {
                        for (int j = 0; j < interfaces.Length; j++)
                        {
                            this.PopulateLiteralFields(filter, interfaces[j], list);
                            this.PopulateRtFields(filter, interfaces[j], list);
                        }
                    }
                    return list;
                }

                private List<RuntimeType> PopulateInterfaces(RuntimeType.RuntimeTypeCache.Filter filter)
                {
                    List<RuntimeType> list = new List<RuntimeType>();
                    RuntimeTypeHandle reflectedTypeHandle = this.ReflectedTypeHandle;
                    if (!reflectedTypeHandle.IsGenericVariable())
                    {
                        RuntimeTypeHandle[] interfaces = this.ReflectedTypeHandle.GetInterfaces();
                        if (interfaces != null)
                        {
                            for (int k = 0; k < interfaces.Length; k++)
                            {
                                RuntimeType runtimeType = interfaces[k].GetRuntimeType();
                                if (filter.Match(runtimeType.GetTypeHandleInternal().GetUtf8Name()))
                                {
                                    list.Add(runtimeType);
                                }
                            }
                        }
                        if (this.ReflectedType.IsSzArray)
                        {
                            Type elementType = this.ReflectedType.GetElementType();
                            if (elementType.IsPointer)
                            {
                                return list;
                            }
                            Type type3 = typeof(IList<>).MakeGenericType(new Type[] { elementType });
                            if (!type3.IsAssignableFrom(this.ReflectedType))
                            {
                                return list;
                            }
                            if (filter.Match(type3.GetTypeHandleInternal().GetUtf8Name()))
                            {
                                list.Add(type3 as RuntimeType);
                            }
                            Type[] typeArray = type3.GetInterfaces();
                            for (int m = 0; m < typeArray.Length; m++)
                            {
                                Type type4 = typeArray[m];
                                if (type4.IsGenericType && filter.Match(type4.GetTypeHandleInternal().GetUtf8Name()))
                                {
                                    list.Add(typeArray[m] as RuntimeType);
                                }
                            }
                        }
                        return list;
                    }
                    List<RuntimeType> list2 = new List<RuntimeType>();
                    foreach (Type type5 in reflectedTypeHandle.GetRuntimeType().GetGenericParameterConstraints())
                    {
                        if (type5.IsInterface)
                        {
                            list2.Add(type5 as RuntimeType);
                        }
                        Type[] typeArray3 = type5.GetInterfaces();
                        for (int n = 0; n < typeArray3.Length; n++)
                        {
                            list2.Add(typeArray3[n] as RuntimeType);
                        }
                    }
                    Hashtable hashtable = new Hashtable();
                    for (int i = 0; i < list2.Count; i++)
                    {
                        Type key = list2[i];
                        if (!hashtable.Contains(key))
                        {
                            hashtable[key] = key;
                        }
                    }
                    Type[] array = new Type[hashtable.Values.Count];
                    hashtable.Values.CopyTo(array, 0);
                    for (int j = 0; j < array.Length; j++)
                    {
                        if (filter.Match(array[j].GetTypeHandleInternal().GetUtf8Name()))
                        {
                            list.Add(array[j] as RuntimeType);
                        }
                    }
                    return list;
                }

                private unsafe void PopulateLiteralFields(RuntimeType.RuntimeTypeCache.Filter filter, RuntimeTypeHandle declaringTypeHandle, List<RuntimeFieldInfo> list)
                {
                    int token = declaringTypeHandle.GetToken();
                    if (!MetadataToken.IsNullToken(token))
                    {
                        MetadataImport metadataImport = declaringTypeHandle.GetModuleHandle().GetMetadataImport();
                        int count = metadataImport.EnumFieldsCount(token);
                        int* result = stackalloc int[count];
                        metadataImport.EnumFields(token, result, count);
                        for (int i = 0; i < count; i++)
                        {
                            int mdToken = result[i];
                            Utf8String name = metadataImport.GetName(mdToken);
                            if (filter.Match(name))
                            {
                                FieldAttributes attributes;
                                metadataImport.GetFieldDefProps(mdToken, out attributes);
                                FieldAttributes attributes2 = attributes & FieldAttributes.FieldAccessMask;
                                if ((attributes & FieldAttributes.Literal) != FieldAttributes.PrivateScope)
                                {
                                    bool isInherited = !declaringTypeHandle.Equals(this.ReflectedTypeHandle);
                                    if (!isInherited || (attributes2 != FieldAttributes.Private))
                                    {
                                        bool isPublic = attributes2 == FieldAttributes.Public;
                                        bool isStatic = (attributes & FieldAttributes.Static) != FieldAttributes.PrivateScope;
                                        BindingFlags bindingFlags = RuntimeType.FilterPreCalculate(isPublic, isInherited, isStatic);
                                        RuntimeFieldInfo item = new MdFieldInfo(mdToken, attributes, declaringTypeHandle, this.m_runtimeTypeCache, bindingFlags);
                                        list.Add(item);
                                    }
                                }
                            }
                        }
                    }
                }

                private unsafe List<RuntimeMethodInfo> PopulateMethods(RuntimeType.RuntimeTypeCache.Filter filter)
                {
                    List<RuntimeMethodInfo> list = new List<RuntimeMethodInfo>();
                    RuntimeTypeHandle reflectedTypeHandle = this.ReflectedTypeHandle;
                    if ((reflectedTypeHandle.GetAttributes() & TypeAttributes.ClassSemanticsMask) != TypeAttributes.ClassSemanticsMask)
                    {
                        while (reflectedTypeHandle.IsGenericVariable())
                        {
                            reflectedTypeHandle = reflectedTypeHandle.GetRuntimeType().BaseType.GetTypeHandleInternal();
                        }
                        bool* flagPtr = (bool*) stackalloc byte[(1 * reflectedTypeHandle.GetNumVirtuals())];
                        bool isValueType = reflectedTypeHandle.GetRuntimeType().IsValueType;
                        while (!reflectedTypeHandle.IsNullHandle())
                        {
                            bool flag7 = reflectedTypeHandle.HasInstantiation() && !reflectedTypeHandle.IsGenericTypeDefinition();
                            int numVirtuals = reflectedTypeHandle.GetNumVirtuals();
                            RuntimeTypeHandle.IntroducedMethodEnumerator enumerator4 = reflectedTypeHandle.IntroducedMethods.GetEnumerator();
                            while (enumerator4.MoveNext())
                            {
                                RuntimeMethodHandle current = enumerator4.Current;
                                if (filter.Match(current.GetUtf8Name()))
                                {
                                    MethodAttributes methodAttributes = current.GetAttributes();
                                    MethodAttributes attributes3 = methodAttributes & MethodAttributes.MemberAccessMask;
                                    if (((methodAttributes & MethodAttributes.RTSpecialName) == MethodAttributes.PrivateScope) && !current.IsILStub())
                                    {
                                        bool flag8 = false;
                                        int slot = 0;
                                        if ((methodAttributes & MethodAttributes.Virtual) != MethodAttributes.PrivateScope)
                                        {
                                            slot = current.GetSlot();
                                            flag8 = slot < numVirtuals;
                                        }
                                        bool flag9 = attributes3 == MethodAttributes.Private;
                                        bool flag10 = flag8 & flag9;
                                        bool isInherited = reflectedTypeHandle.Value != this.ReflectedTypeHandle.Value;
                                        if ((!isInherited || !flag9) || flag10)
                                        {
                                            if (flag8)
                                            {
                                                if (*(((sbyte*) (flagPtr + slot))) != 0)
                                                {
                                                    continue;
                                                }
                                                *((sbyte*) (flagPtr + slot)) = 1;
                                            }
                                            else if (isValueType && ((methodAttributes & (MethodAttributes.Abstract | MethodAttributes.Virtual)) != MethodAttributes.PrivateScope))
                                            {
                                                continue;
                                            }
                                            bool isPublic = attributes3 == MethodAttributes.Public;
                                            bool isStatic = (methodAttributes & MethodAttributes.Static) != MethodAttributes.PrivateScope;
                                            BindingFlags bindingFlags = RuntimeType.FilterPreCalculate(isPublic, isInherited, isStatic);
                                            RuntimeMethodHandle handle = flag7 ? current.GetInstantiatingStubIfNeeded(reflectedTypeHandle) : current;
                                            RuntimeMethodInfo item = new RuntimeMethodInfo(handle, reflectedTypeHandle, this.m_runtimeTypeCache, methodAttributes, bindingFlags);
                                            list.Add(item);
                                        }
                                    }
                                }
                            }
                            reflectedTypeHandle = reflectedTypeHandle.GetBaseTypeHandle();
                        }
                        return list;
                    }
                    bool flag2 = reflectedTypeHandle.HasInstantiation() && !reflectedTypeHandle.IsGenericTypeDefinition();
                    RuntimeTypeHandle.IntroducedMethodEnumerator enumerator = reflectedTypeHandle.IntroducedMethods.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        RuntimeMethodHandle handle2 = enumerator.Current;
                        if (filter.Match(handle2.GetUtf8Name()))
                        {
                            MethodAttributes attributes = handle2.GetAttributes();
                            bool flag3 = (attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public;
                            bool flag4 = (attributes & MethodAttributes.Static) != MethodAttributes.PrivateScope;
                            bool flag5 = false;
                            BindingFlags flags = RuntimeType.FilterPreCalculate(flag3, flag5, flag4);
                            if (((attributes & MethodAttributes.RTSpecialName) == MethodAttributes.PrivateScope) && !handle2.IsILStub())
                            {
                                RuntimeMethodHandle handle3 = flag2 ? handle2.GetInstantiatingStubIfNeeded(reflectedTypeHandle) : handle2;
                                RuntimeMethodInfo info = new RuntimeMethodInfo(handle3, reflectedTypeHandle, this.m_runtimeTypeCache, attributes, flags);
                                list.Add(info);
                            }
                        }
                    }
                    return list;
                }

                private unsafe List<RuntimeType> PopulateNestedClasses(RuntimeType.RuntimeTypeCache.Filter filter)
                {
                    List<RuntimeType> list = new List<RuntimeType>();
                    RuntimeTypeHandle reflectedTypeHandle = this.ReflectedTypeHandle;
                    if (reflectedTypeHandle.IsGenericVariable())
                    {
                        while (reflectedTypeHandle.IsGenericVariable())
                        {
                            reflectedTypeHandle = reflectedTypeHandle.GetRuntimeType().BaseType.GetTypeHandleInternal();
                        }
                    }
                    int token = reflectedTypeHandle.GetToken();
                    if (!MetadataToken.IsNullToken(token))
                    {
                        ModuleHandle moduleHandle = reflectedTypeHandle.GetModuleHandle();
                        MetadataImport metadataImport = moduleHandle.GetMetadataImport();
                        int count = metadataImport.EnumNestedTypesCount(token);
                        int* result = stackalloc int[count];
                        metadataImport.EnumNestedTypes(token, result, count);
                        for (int i = 0; i < count; i++)
                        {
                            RuntimeTypeHandle handle3 = new RuntimeTypeHandle();
                            try
                            {
                                handle3 = moduleHandle.ResolveTypeHandle(result[i]);
                            }
                            catch (TypeLoadException)
                            {
                                continue;
                            }
                            if (filter.Match(handle3.GetRuntimeType().GetTypeHandleInternal().GetUtf8Name()))
                            {
                                list.Add(handle3.GetRuntimeType());
                            }
                        }
                    }
                    return list;
                }

                private List<RuntimePropertyInfo> PopulateProperties(RuntimeType.RuntimeTypeCache.Filter filter)
                {
                    Hashtable csPropertyInfos = new Hashtable();
                    RuntimeTypeHandle reflectedTypeHandle = this.ReflectedTypeHandle;
                    List<RuntimePropertyInfo> list = new List<RuntimePropertyInfo>();
                    if ((reflectedTypeHandle.GetAttributes() & TypeAttributes.ClassSemanticsMask) != TypeAttributes.ClassSemanticsMask)
                    {
                        while (reflectedTypeHandle.IsGenericVariable())
                        {
                            reflectedTypeHandle = reflectedTypeHandle.GetRuntimeType().BaseType.GetTypeHandleInternal();
                        }
                        while (!reflectedTypeHandle.IsNullHandle())
                        {
                            this.PopulateProperties(filter, reflectedTypeHandle, csPropertyInfos, list);
                            reflectedTypeHandle = reflectedTypeHandle.GetBaseTypeHandle();
                        }
                        return list;
                    }
                    this.PopulateProperties(filter, reflectedTypeHandle, csPropertyInfos, list);
                    return list;
                }

                private unsafe void PopulateProperties(RuntimeType.RuntimeTypeCache.Filter filter, RuntimeTypeHandle declaringTypeHandle, Hashtable csPropertyInfos, List<RuntimePropertyInfo> list)
                {
                    int token = declaringTypeHandle.GetToken();
                    if (!MetadataToken.IsNullToken(token))
                    {
                        MetadataImport metadataImport = declaringTypeHandle.GetModuleHandle().GetMetadataImport();
                        int count = metadataImport.EnumPropertiesCount(token);
                        int* result = stackalloc int[count];
                        metadataImport.EnumProperties(token, result, count);
                        this.PopulateProperties(filter, declaringTypeHandle, result, count, csPropertyInfos, list);
                    }
                }

                private unsafe void PopulateProperties(RuntimeType.RuntimeTypeCache.Filter filter, RuntimeTypeHandle declaringTypeHandle, int* tkAssociates, int cProperties, Hashtable csPropertyInfos, List<RuntimePropertyInfo> list)
                {
                    for (int i = 0; i < cProperties; i++)
                    {
                        int mdToken = tkAssociates[i];
                        Utf8String name = declaringTypeHandle.GetRuntimeType().Module.MetadataImport.GetName(mdToken);
                        if (filter.Match(name))
                        {
                            bool flag;
                            RuntimePropertyInfo item = new RuntimePropertyInfo(mdToken, declaringTypeHandle.GetRuntimeType(), this.m_runtimeTypeCache, out flag);
                            if (declaringTypeHandle.Equals(this.m_runtimeTypeCache.RuntimeTypeHandle) || !flag)
                            {
                                List<RuntimePropertyInfo> list2 = csPropertyInfos[item.Name] as List<RuntimePropertyInfo>;
                                if (list2 == null)
                                {
                                    list2 = new List<RuntimePropertyInfo>();
                                    csPropertyInfos[item.Name] = list2;
                                }
                                else
                                {
                                    for (int j = 0; j < list2.Count; j++)
                                    {
                                        if (item.EqualsSig(list2[j]))
                                        {
                                            list2 = null;
                                            break;
                                        }
                                    }
                                }
                                if (list2 != null)
                                {
                                    list2.Add(item);
                                    list.Add(item);
                                }
                            }
                        }
                    }
                }

                private unsafe void PopulateRtFields(RuntimeType.RuntimeTypeCache.Filter filter, RuntimeTypeHandle declaringTypeHandle, List<RuntimeFieldInfo> list)
                {
                    int** result = (int**) stackalloc byte[(sizeof(int*) * 0x40)];
                    int count = 0x40;
                    if (!declaringTypeHandle.GetFields(result, &count))
                    {
                        fixed (int** numPtrRef = new int*[count])
                        {
                            declaringTypeHandle.GetFields(numPtrRef, &count);
                            this.PopulateRtFields(filter, numPtrRef, count, declaringTypeHandle, list);
                        }
                    }
                    else if (count > 0)
                    {
                        this.PopulateRtFields(filter, result, count, declaringTypeHandle, list);
                    }
                }

                private unsafe void PopulateRtFields(RuntimeType.RuntimeTypeCache.Filter filter, int** ppFieldHandles, int count, RuntimeTypeHandle declaringTypeHandle, List<RuntimeFieldInfo> list)
                {
                    bool flag = declaringTypeHandle.HasInstantiation() && !declaringTypeHandle.ContainsGenericVariables();
                    bool isInherited = !declaringTypeHandle.Equals(this.ReflectedTypeHandle);
                    for (int i = 0; i < count; i++)
                    {
                        RuntimeFieldHandle staticFieldForGenericType = new RuntimeFieldHandle(*((void**) (ppFieldHandles + i)));
                        if (filter.Match(staticFieldForGenericType.GetUtf8Name()))
                        {
                            FieldAttributes attributes = staticFieldForGenericType.GetAttributes();
                            FieldAttributes attributes2 = attributes & FieldAttributes.FieldAccessMask;
                            if (!isInherited || (attributes2 != FieldAttributes.Private))
                            {
                                bool isPublic = attributes2 == FieldAttributes.Public;
                                bool isStatic = (attributes & FieldAttributes.Static) != FieldAttributes.PrivateScope;
                                BindingFlags bindingFlags = RuntimeType.FilterPreCalculate(isPublic, isInherited, isStatic);
                                if (flag && isStatic)
                                {
                                    staticFieldForGenericType = staticFieldForGenericType.GetStaticFieldForGenericType(declaringTypeHandle);
                                }
                                RuntimeFieldInfo item = new RtFieldInfo(staticFieldForGenericType, declaringTypeHandle.GetRuntimeType(), this.m_runtimeTypeCache, bindingFlags);
                                list.Add(item);
                            }
                        }
                    }
                }

                internal RuntimeType ReflectedType
                {
                    get
                    {
                        return this.ReflectedTypeHandle.GetRuntimeType();
                    }
                }

                internal RuntimeTypeHandle ReflectedTypeHandle
                {
                    get
                    {
                        return this.m_runtimeTypeCache.RuntimeTypeHandle;
                    }
                }
            }

            internal enum WhatsCached
            {
                Nothing,
                EnclosingType
            }
        }

        private class TypeCacheQueue
        {
            private object[] liveCache = new object[4];
            private const int QUEUE_SIZE = 4;

            internal TypeCacheQueue()
            {
            }
        }
    }
}

