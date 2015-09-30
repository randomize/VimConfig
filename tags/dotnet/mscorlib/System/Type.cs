namespace System
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;
    using System.Reflection.Cache;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Security.Permissions;
    using System.Threading;

    [Serializable, ClassInterface(ClassInterfaceType.None), ComVisible(true), ComDefaultInterface(typeof(_Type))]
    public abstract class Type : MemberInfo, _Type, IReflect
    {
        private static object defaultBinder;
        private const BindingFlags DefaultLookup = (BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        public static readonly char Delimiter = '.';
        public static readonly Type[] EmptyTypes = new Type[0];
        private static readonly Type enumType = typeof(Enum);
        public static readonly MemberFilter FilterAttribute;
        public static readonly MemberFilter FilterName;
        public static readonly MemberFilter FilterNameIgnoreCase;
        public static readonly object Missing = System.Reflection.Missing.Value;
        private static readonly Type objectType = typeof(object);
        private static readonly Type valueType = typeof(ValueType);

        static Type()
        {
            System.__Filters filters = new System.__Filters();
            FilterAttribute = new MemberFilter(filters.FilterAttribute);
            FilterName = new MemberFilter(filters.FilterName);
            FilterNameIgnoreCase = new MemberFilter(filters.FilterIgnoreCase);
        }

        protected Type()
        {
        }

        private static void CreateBinder()
        {
            if (defaultBinder == null)
            {
                object obj2 = new System.DefaultBinder();
                Interlocked.CompareExchange(ref defaultBinder, obj2, null);
            }
        }

        public override bool Equals(object o)
        {
            return (((o != null) && (o is Type)) && (this.UnderlyingSystemType == ((Type) o).UnderlyingSystemType));
        }

        public bool Equals(Type o)
        {
            if (o == null)
            {
                return false;
            }
            return (this.UnderlyingSystemType == o.UnderlyingSystemType);
        }

        public virtual Type[] FindInterfaces(TypeFilter filter, object filterCriteria)
        {
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }
            Type[] interfaces = this.GetInterfaces();
            int num = 0;
            for (int i = 0; i < interfaces.Length; i++)
            {
                if (!filter(interfaces[i], filterCriteria))
                {
                    interfaces[i] = null;
                }
                else
                {
                    num++;
                }
            }
            if (num == interfaces.Length)
            {
                return interfaces;
            }
            Type[] typeArray2 = new Type[num];
            num = 0;
            for (int j = 0; j < interfaces.Length; j++)
            {
                if (interfaces[j] != null)
                {
                    typeArray2[num++] = interfaces[j];
                }
            }
            return typeArray2;
        }

        public virtual MemberInfo[] FindMembers(MemberTypes memberType, BindingFlags bindingAttr, MemberFilter filter, object filterCriteria)
        {
            MethodInfo[] methods = null;
            ConstructorInfo[] constructors = null;
            FieldInfo[] fields = null;
            PropertyInfo[] properties = null;
            EventInfo[] events = null;
            Type[] nestedTypes = null;
            int index = 0;
            int num2 = 0;
            if ((memberType & MemberTypes.Method) != 0)
            {
                methods = this.GetMethods(bindingAttr);
                if (filter != null)
                {
                    for (index = 0; index < methods.Length; index++)
                    {
                        if (!filter(methods[index], filterCriteria))
                        {
                            methods[index] = null;
                        }
                        else
                        {
                            num2++;
                        }
                    }
                }
                else
                {
                    num2 += methods.Length;
                }
            }
            if ((memberType & MemberTypes.Constructor) != 0)
            {
                constructors = this.GetConstructors(bindingAttr);
                if (filter != null)
                {
                    for (index = 0; index < constructors.Length; index++)
                    {
                        if (!filter(constructors[index], filterCriteria))
                        {
                            constructors[index] = null;
                        }
                        else
                        {
                            num2++;
                        }
                    }
                }
                else
                {
                    num2 += constructors.Length;
                }
            }
            if ((memberType & MemberTypes.Field) != 0)
            {
                fields = this.GetFields(bindingAttr);
                if (filter != null)
                {
                    for (index = 0; index < fields.Length; index++)
                    {
                        if (!filter(fields[index], filterCriteria))
                        {
                            fields[index] = null;
                        }
                        else
                        {
                            num2++;
                        }
                    }
                }
                else
                {
                    num2 += fields.Length;
                }
            }
            if ((memberType & MemberTypes.Property) != 0)
            {
                properties = this.GetProperties(bindingAttr);
                if (filter != null)
                {
                    for (index = 0; index < properties.Length; index++)
                    {
                        if (!filter(properties[index], filterCriteria))
                        {
                            properties[index] = null;
                        }
                        else
                        {
                            num2++;
                        }
                    }
                }
                else
                {
                    num2 += properties.Length;
                }
            }
            if ((memberType & MemberTypes.Event) != 0)
            {
                events = this.GetEvents();
                if (filter != null)
                {
                    for (index = 0; index < events.Length; index++)
                    {
                        if (!filter(events[index], filterCriteria))
                        {
                            events[index] = null;
                        }
                        else
                        {
                            num2++;
                        }
                    }
                }
                else
                {
                    num2 += events.Length;
                }
            }
            if ((memberType & MemberTypes.NestedType) != 0)
            {
                nestedTypes = this.GetNestedTypes(bindingAttr);
                if (filter != null)
                {
                    for (index = 0; index < nestedTypes.Length; index++)
                    {
                        if (!filter(nestedTypes[index], filterCriteria))
                        {
                            nestedTypes[index] = null;
                        }
                        else
                        {
                            num2++;
                        }
                    }
                }
                else
                {
                    num2 += nestedTypes.Length;
                }
            }
            MemberInfo[] infoArray6 = new MemberInfo[num2];
            num2 = 0;
            if (methods != null)
            {
                for (index = 0; index < methods.Length; index++)
                {
                    if (methods[index] != null)
                    {
                        infoArray6[num2++] = methods[index];
                    }
                }
            }
            if (constructors != null)
            {
                for (index = 0; index < constructors.Length; index++)
                {
                    if (constructors[index] != null)
                    {
                        infoArray6[num2++] = constructors[index];
                    }
                }
            }
            if (fields != null)
            {
                for (index = 0; index < fields.Length; index++)
                {
                    if (fields[index] != null)
                    {
                        infoArray6[num2++] = fields[index];
                    }
                }
            }
            if (properties != null)
            {
                for (index = 0; index < properties.Length; index++)
                {
                    if (properties[index] != null)
                    {
                        infoArray6[num2++] = properties[index];
                    }
                }
            }
            if (events != null)
            {
                for (index = 0; index < events.Length; index++)
                {
                    if (events[index] != null)
                    {
                        infoArray6[num2++] = events[index];
                    }
                }
            }
            if (nestedTypes != null)
            {
                for (index = 0; index < nestedTypes.Length; index++)
                {
                    if (nestedTypes[index] != null)
                    {
                        infoArray6[num2++] = nestedTypes[index];
                    }
                }
            }
            return infoArray6;
        }

        public virtual int GetArrayRank()
        {
            throw new NotSupportedException(Environment.GetResourceString("NotSupported_SubclassOverride"));
        }

        protected abstract TypeAttributes GetAttributeFlagsImpl();
        [ComVisible(true)]
        public ConstructorInfo GetConstructor(Type[] types)
        {
            return this.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, types, null);
        }

        [ComVisible(true)]
        public ConstructorInfo GetConstructor(BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
        {
            if (types == null)
            {
                throw new ArgumentNullException("types");
            }
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i] == null)
                {
                    throw new ArgumentNullException("types");
                }
            }
            return this.GetConstructorImpl(bindingAttr, binder, CallingConventions.Any, types, modifiers);
        }

        [ComVisible(true)]
        public ConstructorInfo GetConstructor(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            if (types == null)
            {
                throw new ArgumentNullException("types");
            }
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i] == null)
                {
                    throw new ArgumentNullException("types");
                }
            }
            return this.GetConstructorImpl(bindingAttr, binder, callConvention, types, modifiers);
        }

        protected abstract ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers);
        [ComVisible(true)]
        public ConstructorInfo[] GetConstructors()
        {
            return this.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
        }

        [ComVisible(true)]
        public abstract ConstructorInfo[] GetConstructors(BindingFlags bindingAttr);
        internal virtual string GetDefaultMemberName()
        {
            string memberName = (string) base.Cache[CacheObjType.DefaultMember];
            if (memberName == null)
            {
                object[] customAttributes = this.GetCustomAttributes(typeof(DefaultMemberAttribute), true);
                if (customAttributes.Length > 1)
                {
                    throw new ExecutionEngineException(Environment.GetResourceString("ExecutionEngine_InvalidAttribute"));
                }
                if (customAttributes.Length == 0)
                {
                    return null;
                }
                memberName = ((DefaultMemberAttribute) customAttributes[0]).MemberName;
                base.Cache[CacheObjType.DefaultMember] = memberName;
            }
            return memberName;
        }

        public virtual MemberInfo[] GetDefaultMembers()
        {
            string name = (string) base.Cache[CacheObjType.DefaultMember];
            if (name == null)
            {
                CustomAttributeData data = null;
                for (Type type = this; type != null; type = type.BaseType)
                {
                    IList<CustomAttributeData> customAttributes = CustomAttributeData.GetCustomAttributes(type);
                    for (int i = 0; i < customAttributes.Count; i++)
                    {
                        if (customAttributes[i].Constructor.DeclaringType == typeof(DefaultMemberAttribute))
                        {
                            data = customAttributes[i];
                            break;
                        }
                    }
                    if (data != null)
                    {
                        break;
                    }
                }
                if (data == null)
                {
                    return new MemberInfo[0];
                }
                CustomAttributeTypedArgument argument = data.ConstructorArguments[0];
                name = argument.Value as string;
                base.Cache[CacheObjType.DefaultMember] = name;
            }
            MemberInfo[] member = this.GetMember(name);
            if (member == null)
            {
                member = new MemberInfo[0];
            }
            return member;
        }

        public abstract Type GetElementType();
        public EventInfo GetEvent(string name)
        {
            return this.GetEvent(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        }

        public abstract EventInfo GetEvent(string name, BindingFlags bindingAttr);
        public virtual EventInfo[] GetEvents()
        {
            return this.GetEvents(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        }

        public abstract EventInfo[] GetEvents(BindingFlags bindingAttr);
        public FieldInfo GetField(string name)
        {
            return this.GetField(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        }

        public abstract FieldInfo GetField(string name, BindingFlags bindingAttr);
        public FieldInfo[] GetFields()
        {
            return this.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        }

        public abstract FieldInfo[] GetFields(BindingFlags bindingAttr);
        public virtual Type[] GetGenericArguments()
        {
            throw new NotSupportedException(Environment.GetResourceString("NotSupported_SubclassOverride"));
        }

        public virtual Type[] GetGenericParameterConstraints()
        {
            if (!this.IsGenericParameter)
            {
                throw new InvalidOperationException(Environment.GetResourceString("Arg_NotGenericParameter"));
            }
            throw new InvalidOperationException();
        }

        public virtual Type GetGenericTypeDefinition()
        {
            throw new NotSupportedException(Environment.GetResourceString("NotSupported_SubclassOverride"));
        }

        public override int GetHashCode()
        {
            Type underlyingSystemType = this.UnderlyingSystemType;
            if (underlyingSystemType != this)
            {
                return underlyingSystemType.GetHashCode();
            }
            return base.GetHashCode();
        }

        public Type GetInterface(string name)
        {
            return this.GetInterface(name, false);
        }

        public abstract Type GetInterface(string name, bool ignoreCase);
        [ComVisible(true)]
        public virtual InterfaceMapping GetInterfaceMap(Type interfaceType)
        {
            throw new NotSupportedException(Environment.GetResourceString("NotSupported_SubclassOverride"));
        }

        public abstract Type[] GetInterfaces();
        public MemberInfo[] GetMember(string name)
        {
            return this.GetMember(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        }

        public virtual MemberInfo[] GetMember(string name, BindingFlags bindingAttr)
        {
            return this.GetMember(name, MemberTypes.All, bindingAttr);
        }

        public virtual MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
        {
            throw new NotSupportedException(Environment.GetResourceString("NotSupported_SubclassOverride"));
        }

        public MemberInfo[] GetMembers()
        {
            return this.GetMembers(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        }

        public abstract MemberInfo[] GetMembers(BindingFlags bindingAttr);
        public MethodInfo GetMethod(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            return this.GetMethodImpl(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, CallingConventions.Any, null, null);
        }

        public MethodInfo GetMethod(string name, Type[] types)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (types == null)
            {
                throw new ArgumentNullException("types");
            }
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i] == null)
                {
                    throw new ArgumentNullException("types");
                }
            }
            return this.GetMethodImpl(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, CallingConventions.Any, types, null);
        }

        public MethodInfo GetMethod(string name, BindingFlags bindingAttr)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            return this.GetMethodImpl(name, bindingAttr, null, CallingConventions.Any, null, null);
        }

        public MethodInfo GetMethod(string name, Type[] types, ParameterModifier[] modifiers)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (types == null)
            {
                throw new ArgumentNullException("types");
            }
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i] == null)
                {
                    throw new ArgumentNullException("types");
                }
            }
            return this.GetMethodImpl(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, CallingConventions.Any, types, modifiers);
        }

        public MethodInfo GetMethod(string name, BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (types == null)
            {
                throw new ArgumentNullException("types");
            }
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i] == null)
                {
                    throw new ArgumentNullException("types");
                }
            }
            return this.GetMethodImpl(name, bindingAttr, binder, CallingConventions.Any, types, modifiers);
        }

        public MethodInfo GetMethod(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (types == null)
            {
                throw new ArgumentNullException("types");
            }
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i] == null)
                {
                    throw new ArgumentNullException("types");
                }
            }
            return this.GetMethodImpl(name, bindingAttr, binder, callConvention, types, modifiers);
        }

        protected abstract MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers);
        public MethodInfo[] GetMethods()
        {
            return this.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        }

        public abstract MethodInfo[] GetMethods(BindingFlags bindingAttr);
        public Type GetNestedType(string name)
        {
            return this.GetNestedType(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        }

        public abstract Type GetNestedType(string name, BindingFlags bindingAttr);
        public Type[] GetNestedTypes()
        {
            return this.GetNestedTypes(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        }

        public abstract Type[] GetNestedTypes(BindingFlags bindingAttr);
        public PropertyInfo[] GetProperties()
        {
            return this.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        }

        public abstract PropertyInfo[] GetProperties(BindingFlags bindingAttr);
        public PropertyInfo GetProperty(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            return this.GetPropertyImpl(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, null, null, null);
        }

        public PropertyInfo GetProperty(string name, BindingFlags bindingAttr)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            return this.GetPropertyImpl(name, bindingAttr, null, null, null, null);
        }

        public PropertyInfo GetProperty(string name, Type[] types)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (types == null)
            {
                throw new ArgumentNullException("types");
            }
            return this.GetPropertyImpl(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, null, types, null);
        }

        public PropertyInfo GetProperty(string name, Type returnType)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (returnType == null)
            {
                throw new ArgumentNullException("returnType");
            }
            return this.GetPropertyImpl(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, returnType, null, null);
        }

        public PropertyInfo GetProperty(string name, Type returnType, Type[] types)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (types == null)
            {
                throw new ArgumentNullException("types");
            }
            return this.GetPropertyImpl(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, returnType, types, null);
        }

        public PropertyInfo GetProperty(string name, Type returnType, Type[] types, ParameterModifier[] modifiers)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (types == null)
            {
                throw new ArgumentNullException("types");
            }
            return this.GetPropertyImpl(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, returnType, types, modifiers);
        }

        public PropertyInfo GetProperty(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (types == null)
            {
                throw new ArgumentNullException("types");
            }
            return this.GetPropertyImpl(name, bindingAttr, binder, returnType, types, modifiers);
        }

        protected abstract PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers);
        internal virtual Type GetRootElementType()
        {
            Type elementType = this;
            while (elementType.HasElementType)
            {
                elementType = elementType.GetElementType();
            }
            return elementType;
        }

        public Type GetType()
        {
            return base.GetType();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Type GetType(string typeName)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return RuntimeType.PrivateGetType(typeName, false, false, ref lookForMyCaller);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Type GetType(string typeName, bool throwOnError)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return RuntimeType.PrivateGetType(typeName, throwOnError, false, ref lookForMyCaller);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Type GetType(string typeName, bool throwOnError, bool ignoreCase)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return RuntimeType.PrivateGetType(typeName, throwOnError, ignoreCase, ref lookForMyCaller);
        }

        public static Type[] GetTypeArray(object[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }
            Type[] typeArray = new Type[args.Length];
            for (int i = 0; i < typeArray.Length; i++)
            {
                if (args[i] == null)
                {
                    throw new ArgumentNullException();
                }
                typeArray[i] = args[i].GetType();
            }
            return typeArray;
        }

        public static TypeCode GetTypeCode(Type type)
        {
            if (type == null)
            {
                return TypeCode.Empty;
            }
            return type.GetTypeCodeInternal();
        }

        internal virtual TypeCode GetTypeCodeInternal()
        {
            Type type = this;
            if (!(type is SymbolType))
            {
                if (type is TypeBuilder)
                {
                    TypeBuilder builder = (TypeBuilder) type;
                    if (!builder.IsEnum)
                    {
                        return TypeCode.Object;
                    }
                }
                if (type != type.UnderlyingSystemType)
                {
                    return GetTypeCode(type.UnderlyingSystemType);
                }
            }
            return TypeCode.Object;
        }

        public static Type GetTypeFromCLSID(Guid clsid)
        {
            return RuntimeType.GetTypeFromCLSIDImpl(clsid, null, false);
        }

        public static Type GetTypeFromCLSID(Guid clsid, bool throwOnError)
        {
            return RuntimeType.GetTypeFromCLSIDImpl(clsid, null, throwOnError);
        }

        public static Type GetTypeFromCLSID(Guid clsid, string server)
        {
            return RuntimeType.GetTypeFromCLSIDImpl(clsid, server, false);
        }

        public static Type GetTypeFromCLSID(Guid clsid, string server, bool throwOnError)
        {
            return RuntimeType.GetTypeFromCLSIDImpl(clsid, server, throwOnError);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern Type GetTypeFromHandle(RuntimeTypeHandle handle);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static Type GetTypeFromProgID(string progID)
        {
            return RuntimeType.GetTypeFromProgIDImpl(progID, null, false);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static Type GetTypeFromProgID(string progID, bool throwOnError)
        {
            return RuntimeType.GetTypeFromProgIDImpl(progID, null, throwOnError);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static Type GetTypeFromProgID(string progID, string server)
        {
            return RuntimeType.GetTypeFromProgIDImpl(progID, server, false);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static Type GetTypeFromProgID(string progID, string server, bool throwOnError)
        {
            return RuntimeType.GetTypeFromProgIDImpl(progID, server, throwOnError);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern RuntimeTypeHandle GetTypeHandle(object o);
        internal virtual RuntimeTypeHandle GetTypeHandleInternal()
        {
            return this.TypeHandle;
        }

        protected abstract bool HasElementTypeImpl();
        internal virtual bool HasProxyAttributeImpl()
        {
            return false;
        }

        [DebuggerStepThrough, DebuggerHidden]
        public object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args)
        {
            return this.InvokeMember(name, invokeAttr, binder, target, args, null, null, null);
        }

        [DebuggerStepThrough, DebuggerHidden]
        public object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, CultureInfo culture)
        {
            return this.InvokeMember(name, invokeAttr, binder, target, args, null, culture, null);
        }

        public abstract object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters);
        protected abstract bool IsArrayImpl();
        public virtual bool IsAssignableFrom(Type c)
        {
            if (c != null)
            {
                try
                {
                    RuntimeType underlyingSystemType = c.UnderlyingSystemType as RuntimeType;
                    RuntimeType toType = this.UnderlyingSystemType as RuntimeType;
                    if ((underlyingSystemType == null) || (toType == null))
                    {
                        TypeBuilder builder = c as TypeBuilder;
                        if (builder != null)
                        {
                            if (TypeBuilder.IsTypeEqual(this, c))
                            {
                                return true;
                            }
                            if (builder.IsSubclassOf(this))
                            {
                                return true;
                            }
                            if (!this.IsInterface)
                            {
                                return false;
                            }
                            Type[] interfaces = builder.GetInterfaces();
                            for (int i = 0; i < interfaces.Length; i++)
                            {
                                if (TypeBuilder.IsTypeEqual(interfaces[i], this))
                                {
                                    return true;
                                }
                                if (interfaces[i].IsSubclassOf(this))
                                {
                                    return true;
                                }
                            }
                        }
                        return false;
                    }
                    return RuntimeType.CanCastTo(underlyingSystemType, toType);
                }
                catch (ArgumentException)
                {
                }
                if (this.IsInterface)
                {
                    Type[] typeArray2 = c.GetInterfaces();
                    for (int j = 0; j < typeArray2.Length; j++)
                    {
                        if (this == typeArray2[j])
                        {
                            return true;
                        }
                    }
                }
                else if (!this.IsGenericParameter)
                {
                    while (c != null)
                    {
                        if (c == this)
                        {
                            return true;
                        }
                        c = c.BaseType;
                    }
                }
                else
                {
                    Type[] genericParameterConstraints = this.GetGenericParameterConstraints();
                    for (int k = 0; k < genericParameterConstraints.Length; k++)
                    {
                        if (!genericParameterConstraints[k].IsAssignableFrom(c))
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        protected abstract bool IsByRefImpl();
        protected abstract bool IsCOMObjectImpl();
        protected virtual bool IsContextfulImpl()
        {
            return typeof(ContextBoundObject).IsAssignableFrom(this);
        }

        public virtual bool IsInstanceOfType(object o)
        {
            if (this is RuntimeType)
            {
                return this.IsInstanceOfType(o);
            }
            if (o == null)
            {
                return false;
            }
            if (RemotingServices.IsTransparentProxy(o))
            {
                return (null != RemotingServices.CheckCast(o, this));
            }
            if ((this.IsInterface && o.GetType().IsCOMObject) && (this is RuntimeType))
            {
                return ((RuntimeType) this).SupportsInterface(o);
            }
            return this.IsAssignableFrom(o.GetType());
        }

        protected virtual bool IsMarshalByRefImpl()
        {
            return typeof(MarshalByRefObject).IsAssignableFrom(this);
        }

        protected abstract bool IsPointerImpl();
        protected abstract bool IsPrimitiveImpl();
        [ComVisible(true)]
        public virtual bool IsSubclassOf(Type c)
        {
            Type baseType = this;
            if (baseType != c)
            {
                while (baseType != null)
                {
                    if (baseType == c)
                    {
                        return true;
                    }
                    baseType = baseType.BaseType;
                }
                return false;
            }
            return false;
        }

        protected virtual bool IsValueTypeImpl()
        {
            Type type = this;
            return (((type != valueType) && (type != enumType)) && this.IsSubclassOf(valueType));
        }

        public virtual Type MakeArrayType()
        {
            throw new NotSupportedException();
        }

        public virtual Type MakeArrayType(int rank)
        {
            throw new NotSupportedException();
        }

        public virtual Type MakeByRefType()
        {
            throw new NotSupportedException();
        }

        public virtual Type MakeGenericType(params Type[] typeArguments)
        {
            throw new NotSupportedException(Environment.GetResourceString("NotSupported_SubclassOverride"));
        }

        public virtual Type MakePointerType()
        {
            throw new NotSupportedException();
        }

        private bool QuickSerializationCastCheck()
        {
            for (Type type = this.UnderlyingSystemType; type != null; type = type.BaseType)
            {
                if ((type == typeof(Enum)) || (type == typeof(Delegate)))
                {
                    return true;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Type ReflectionOnlyGetType(string typeName, bool throwIfNotFound, bool ignoreCase)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return RuntimeType.PrivateGetType(typeName, throwIfNotFound, ignoreCase, true, ref lookForMyCaller);
        }

        internal static Type ResolveTypeRelativeTo(string typeName, int offset, int count, Type serverType)
        {
            Type type = ResolveTypeRelativeToBaseTypes(typeName, offset, count, serverType);
            if (type == null)
            {
                foreach (Type type2 in serverType.GetInterfaces())
                {
                    string fullName = type2.FullName;
                    if ((fullName.Length == count) && (string.CompareOrdinal(typeName, offset, fullName, 0, count) == 0))
                    {
                        return type2;
                    }
                }
            }
            return type;
        }

        internal static Type ResolveTypeRelativeToBaseTypes(string typeName, int offset, int count, Type serverType)
        {
            if ((typeName == null) || (serverType == null))
            {
                return null;
            }
            string fullName = serverType.FullName;
            if ((fullName.Length == count) && (string.CompareOrdinal(typeName, offset, fullName, 0, count) == 0))
            {
                return serverType;
            }
            return ResolveTypeRelativeToBaseTypes(typeName, offset, count, serverType.BaseType);
        }

        internal string SigToString()
        {
            Type elementType = this;
            while (elementType.HasElementType)
            {
                elementType = elementType.GetElementType();
            }
            if (elementType.IsNested)
            {
                return this.Name;
            }
            string str = this.ToString();
            if ((!elementType.IsPrimitive && (elementType != typeof(void))) && (elementType != typeof(TypedReference)))
            {
                return str;
            }
            return str.Substring("System.".Length);
        }

        void _Type.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
        {
            throw new NotImplementedException();
        }

        void _Type.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
        {
            throw new NotImplementedException();
        }

        void _Type.GetTypeInfoCount(out uint pcTInfo)
        {
            throw new NotImplementedException();
        }

        void _Type.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return ("Type: " + this.Name);
        }

        public abstract System.Reflection.Assembly Assembly { get; }

        public abstract string AssemblyQualifiedName { get; }

        public TypeAttributes Attributes
        {
            get
            {
                return this.GetAttributeFlagsImpl();
            }
        }

        public abstract Type BaseType { get; }

        public virtual bool ContainsGenericParameters
        {
            get
            {
                if (this.HasElementType)
                {
                    return this.GetRootElementType().ContainsGenericParameters;
                }
                if (this.IsGenericParameter)
                {
                    return true;
                }
                if (this.IsGenericType)
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

        public virtual MethodBase DeclaringMethod
        {
            get
            {
                return null;
            }
        }

        public override Type DeclaringType
        {
            get
            {
                return this;
            }
        }

        public static Binder DefaultBinder
        {
            get
            {
                if (defaultBinder == null)
                {
                    CreateBinder();
                }
                return (defaultBinder as Binder);
            }
        }

        public abstract string FullName { get; }

        public virtual System.Reflection.GenericParameterAttributes GenericParameterAttributes
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public virtual int GenericParameterPosition
        {
            get
            {
                throw new InvalidOperationException(Environment.GetResourceString("Arg_NotGenericParameter"));
            }
        }

        public abstract Guid GUID { get; }

        public bool HasElementType
        {
            get
            {
                return this.HasElementTypeImpl();
            }
        }

        internal bool HasProxyAttribute
        {
            get
            {
                return this.HasProxyAttributeImpl();
            }
        }

        public bool IsAbstract
        {
            get
            {
                return ((this.GetAttributeFlagsImpl() & TypeAttributes.Abstract) != TypeAttributes.AnsiClass);
            }
        }

        public bool IsAnsiClass
        {
            get
            {
                return ((this.GetAttributeFlagsImpl() & TypeAttributes.CustomFormatClass) == TypeAttributes.AnsiClass);
            }
        }

        public bool IsArray
        {
            get
            {
                return this.IsArrayImpl();
            }
        }

        public bool IsAutoClass
        {
            get
            {
                return ((this.GetAttributeFlagsImpl() & TypeAttributes.CustomFormatClass) == TypeAttributes.AutoClass);
            }
        }

        public bool IsAutoLayout
        {
            get
            {
                return ((this.GetAttributeFlagsImpl() & TypeAttributes.LayoutMask) == TypeAttributes.AnsiClass);
            }
        }

        public bool IsByRef
        {
            get
            {
                return this.IsByRefImpl();
            }
        }

        public bool IsClass
        {
            get
            {
                return (((this.GetAttributeFlagsImpl() & TypeAttributes.ClassSemanticsMask) == TypeAttributes.AnsiClass) && !this.IsSubclassOf(valueType));
            }
        }

        public bool IsCOMObject
        {
            get
            {
                return this.IsCOMObjectImpl();
            }
        }

        public bool IsContextful
        {
            get
            {
                return this.IsContextfulImpl();
            }
        }

        public bool IsEnum
        {
            get
            {
                return this.IsSubclassOf(enumType);
            }
        }

        public bool IsExplicitLayout
        {
            get
            {
                return ((this.GetAttributeFlagsImpl() & TypeAttributes.LayoutMask) == TypeAttributes.ExplicitLayout);
            }
        }

        public virtual bool IsGenericParameter
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsGenericType
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsGenericTypeDefinition
        {
            get
            {
                return false;
            }
        }

        public bool IsImport
        {
            get
            {
                return ((this.GetAttributeFlagsImpl() & TypeAttributes.Import) != TypeAttributes.AnsiClass);
            }
        }

        public bool IsInterface
        {
            get
            {
                if (this is RuntimeType)
                {
                    return this.GetTypeHandleInternal().IsInterface();
                }
                return ((this.GetAttributeFlagsImpl() & TypeAttributes.ClassSemanticsMask) == TypeAttributes.ClassSemanticsMask);
            }
        }

        public bool IsLayoutSequential
        {
            get
            {
                return ((this.GetAttributeFlagsImpl() & TypeAttributes.LayoutMask) == TypeAttributes.SequentialLayout);
            }
        }

        public bool IsMarshalByRef
        {
            get
            {
                return this.IsMarshalByRefImpl();
            }
        }

        public bool IsNested
        {
            get
            {
                return (this.DeclaringType != null);
            }
        }

        public bool IsNestedAssembly
        {
            get
            {
                return ((this.GetAttributeFlagsImpl() & TypeAttributes.NestedFamORAssem) == TypeAttributes.NestedAssembly);
            }
        }

        public bool IsNestedFamANDAssem
        {
            get
            {
                return ((this.GetAttributeFlagsImpl() & TypeAttributes.NestedFamORAssem) == TypeAttributes.NestedFamANDAssem);
            }
        }

        public bool IsNestedFamily
        {
            get
            {
                return ((this.GetAttributeFlagsImpl() & TypeAttributes.NestedFamORAssem) == TypeAttributes.NestedFamily);
            }
        }

        public bool IsNestedFamORAssem
        {
            get
            {
                return ((this.GetAttributeFlagsImpl() & TypeAttributes.NestedFamORAssem) == TypeAttributes.NestedFamORAssem);
            }
        }

        public bool IsNestedPrivate
        {
            get
            {
                return ((this.GetAttributeFlagsImpl() & TypeAttributes.NestedFamORAssem) == TypeAttributes.NestedPrivate);
            }
        }

        public bool IsNestedPublic
        {
            get
            {
                return ((this.GetAttributeFlagsImpl() & TypeAttributes.NestedFamORAssem) == TypeAttributes.NestedPublic);
            }
        }

        public bool IsNotPublic
        {
            get
            {
                return ((this.GetAttributeFlagsImpl() & TypeAttributes.NestedFamORAssem) == TypeAttributes.AnsiClass);
            }
        }

        public bool IsPointer
        {
            get
            {
                return this.IsPointerImpl();
            }
        }

        public bool IsPrimitive
        {
            get
            {
                return this.IsPrimitiveImpl();
            }
        }

        public bool IsPublic
        {
            get
            {
                return ((this.GetAttributeFlagsImpl() & TypeAttributes.NestedFamORAssem) == TypeAttributes.Public);
            }
        }

        public bool IsSealed
        {
            get
            {
                return ((this.GetAttributeFlagsImpl() & TypeAttributes.Sealed) != TypeAttributes.AnsiClass);
            }
        }

        public bool IsSerializable
        {
            get
            {
                if ((this.GetAttributeFlagsImpl() & TypeAttributes.Serializable) == TypeAttributes.AnsiClass)
                {
                    return this.QuickSerializationCastCheck();
                }
                return true;
            }
        }

        public bool IsSpecialName
        {
            get
            {
                return ((this.GetAttributeFlagsImpl() & TypeAttributes.SpecialName) != TypeAttributes.AnsiClass);
            }
        }

        internal virtual bool IsSzArray
        {
            get
            {
                return false;
            }
        }

        public bool IsUnicodeClass
        {
            get
            {
                return ((this.GetAttributeFlagsImpl() & TypeAttributes.CustomFormatClass) == TypeAttributes.UnicodeClass);
            }
        }

        public bool IsValueType
        {
            get
            {
                return this.IsValueTypeImpl();
            }
        }

        public bool IsVisible
        {
            get
            {
                return this.GetTypeHandleInternal().IsVisible();
            }
        }

        public override MemberTypes MemberType
        {
            get
            {
                return MemberTypes.TypeInfo;
            }
        }

        public abstract System.Reflection.Module Module { get; }

        public abstract string Namespace { get; }

        public override Type ReflectedType
        {
            get
            {
                return this;
            }
        }

        public virtual System.Runtime.InteropServices.StructLayoutAttribute StructLayoutAttribute
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public virtual RuntimeTypeHandle TypeHandle { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        [ComVisible(true)]
        public ConstructorInfo TypeInitializer
        {
            get
            {
                return this.GetConstructorImpl(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, CallingConventions.Any, EmptyTypes, null);
            }
        }

        public abstract Type UnderlyingSystemType { get; }
    }
}

