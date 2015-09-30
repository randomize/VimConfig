namespace System.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Cache;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Threading;

    [Serializable, ClassInterface(ClassInterfaceType.None), ComVisible(true), ComDefaultInterface(typeof(_ParameterInfo))]
    public class ParameterInfo : _ParameterInfo, ICustomAttributeProvider
    {
        private IntPtr _importer;
        private int _token;
        protected ParameterAttributes AttrsImpl;
        private bool bExtraConstChecked;
        protected Type ClassImpl;
        protected object DefaultValueImpl;
        private InternalCache m_cachedData;
        [NonSerialized]
        private volatile bool m_nameIsCached;
        [NonSerialized]
        private readonly bool m_noDefaultValue;
        [NonSerialized]
        private MetadataImport m_scope;
        [NonSerialized]
        private Signature m_signature;
        [NonSerialized]
        private int m_tkParamDef;
        protected MemberInfo MemberImpl;
        protected string NameImpl;
        private static Type ParameterInfoType = typeof(ParameterInfo);
        protected int PositionImpl;
        private static readonly Type s_CustomConstantAttributeType = typeof(CustomConstantAttribute);
        private static readonly Type s_DecimalConstantAttributeType = typeof(DecimalConstantAttribute);

        protected ParameterInfo()
        {
            this.m_nameIsCached = true;
            this.m_noDefaultValue = true;
        }

        internal ParameterInfo(ParameterInfo accessor, MethodBuilderInstantiation method) : this(accessor, (MemberInfo) method)
        {
            this.m_signature = accessor.m_signature;
            if (this.ClassImpl.IsGenericParameter)
            {
                this.ClassImpl = method.GetGenericArguments()[this.ClassImpl.GenericParameterPosition];
            }
        }

        private ParameterInfo(ParameterInfo accessor, MemberInfo member)
        {
            this.MemberImpl = member;
            this.NameImpl = accessor.Name;
            this.m_nameIsCached = true;
            this.ClassImpl = accessor.ParameterType;
            this.PositionImpl = accessor.Position;
            this.AttrsImpl = accessor.Attributes;
            this.m_tkParamDef = System.Reflection.MetadataToken.IsNullToken(accessor.MetadataToken) ? 0x8000000 : accessor.MetadataToken;
            this.m_scope = accessor.m_scope;
        }

        internal ParameterInfo(ParameterInfo accessor, RuntimePropertyInfo property) : this(accessor, (MemberInfo) property)
        {
            this.m_signature = property.Signature;
        }

        internal ParameterInfo(MethodInfo owner, string name, RuntimeType parameterType, int position)
        {
            this.MemberImpl = owner;
            this.NameImpl = name;
            this.m_nameIsCached = true;
            this.m_noDefaultValue = true;
            this.ClassImpl = parameterType;
            this.PositionImpl = position;
            this.AttrsImpl = ParameterAttributes.None;
            this.m_tkParamDef = 0x8000000;
            this.m_scope = MetadataImport.EmptyImport;
        }

        private ParameterInfo(Signature signature, MetadataImport scope, int tkParamDef, int position, ParameterAttributes attributes, MemberInfo member)
        {
            this.PositionImpl = position;
            this.MemberImpl = member;
            this.m_signature = signature;
            this.m_tkParamDef = System.Reflection.MetadataToken.IsNullToken(tkParamDef) ? 0x8000000 : tkParamDef;
            this.m_scope = scope;
            this.AttrsImpl = attributes;
            this.ClassImpl = null;
            this.NameImpl = null;
        }

        public virtual object[] GetCustomAttributes(bool inherit)
        {
            if (this.IsLegacyParameterInfo)
            {
                return null;
            }
            if (System.Reflection.MetadataToken.IsNullToken(this.m_tkParamDef))
            {
                return new object[0];
            }
            return CustomAttribute.GetCustomAttributes(this, typeof(object) as RuntimeType);
        }

        public virtual object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            if (this.IsLegacyParameterInfo)
            {
                return null;
            }
            if (attributeType == null)
            {
                throw new ArgumentNullException("attributeType");
            }
            if (System.Reflection.MetadataToken.IsNullToken(this.m_tkParamDef))
            {
                return new object[0];
            }
            RuntimeType underlyingSystemType = attributeType.UnderlyingSystemType as RuntimeType;
            if (underlyingSystemType == null)
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_MustBeType"), "attributeType");
            }
            return CustomAttribute.GetCustomAttributes(this, underlyingSystemType);
        }

        internal object GetDefaultValue(bool raw)
        {
            object missing = null;
            if (!this.m_noDefaultValue)
            {
                if (this.ParameterType == typeof(DateTime))
                {
                    if (raw)
                    {
                        CustomAttributeTypedArgument argument = CustomAttributeData.Filter(CustomAttributeData.GetCustomAttributes(this), typeof(DateTimeConstantAttribute), 0);
                        if (argument.ArgumentType != null)
                        {
                            return new DateTime((long) argument.Value);
                        }
                    }
                    else
                    {
                        object[] customAttributes = this.GetCustomAttributes(typeof(DateTimeConstantAttribute), false);
                        if ((customAttributes != null) && (customAttributes.Length != 0))
                        {
                            return ((DateTimeConstantAttribute) customAttributes[0]).Value;
                        }
                    }
                }
                if (!System.Reflection.MetadataToken.IsNullToken(this.m_tkParamDef))
                {
                    missing = MdConstant.GetValue(this.m_scope, this.m_tkParamDef, this.ParameterType.GetTypeHandleInternal(), raw);
                }
                if (missing == DBNull.Value)
                {
                    if (raw)
                    {
                        IList<CustomAttributeData> attrs = CustomAttributeData.GetCustomAttributes(this);
                        CustomAttributeTypedArgument argument2 = CustomAttributeData.Filter(attrs, s_CustomConstantAttributeType, "Value");
                        if (argument2.ArgumentType == null)
                        {
                            argument2 = CustomAttributeData.Filter(attrs, s_DecimalConstantAttributeType, "Value");
                            if (argument2.ArgumentType == null)
                            {
                                for (int i = 0; i < attrs.Count; i++)
                                {
                                    if (attrs[i].Constructor.DeclaringType == s_DecimalConstantAttributeType)
                                    {
                                        ParameterInfo[] parameters = attrs[i].Constructor.GetParameters();
                                        if (parameters.Length != 0)
                                        {
                                            if (parameters[2].ParameterType == typeof(uint))
                                            {
                                                IList<CustomAttributeTypedArgument> constructorArguments = attrs[i].ConstructorArguments;
                                                CustomAttributeTypedArgument argument3 = constructorArguments[4];
                                                int lo = (int) ((uint) argument3.Value);
                                                CustomAttributeTypedArgument argument4 = constructorArguments[3];
                                                int mid = (int) ((uint) argument4.Value);
                                                CustomAttributeTypedArgument argument5 = constructorArguments[2];
                                                int hi = (int) ((uint) argument5.Value);
                                                CustomAttributeTypedArgument argument6 = constructorArguments[1];
                                                byte num5 = (byte) argument6.Value;
                                                CustomAttributeTypedArgument argument7 = constructorArguments[0];
                                                byte scale = (byte) argument7.Value;
                                                argument2 = new CustomAttributeTypedArgument(new decimal(lo, mid, hi, num5 != 0, scale));
                                            }
                                            else
                                            {
                                                IList<CustomAttributeTypedArgument> list3 = attrs[i].ConstructorArguments;
                                                CustomAttributeTypedArgument argument8 = list3[4];
                                                int num7 = (int) argument8.Value;
                                                CustomAttributeTypedArgument argument9 = list3[3];
                                                int num8 = (int) argument9.Value;
                                                CustomAttributeTypedArgument argument10 = list3[2];
                                                int num9 = (int) argument10.Value;
                                                CustomAttributeTypedArgument argument11 = list3[1];
                                                byte num10 = (byte) argument11.Value;
                                                CustomAttributeTypedArgument argument12 = list3[0];
                                                byte num11 = (byte) argument12.Value;
                                                argument2 = new CustomAttributeTypedArgument(new decimal(num7, num8, num9, num10 != 0, num11));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (argument2.ArgumentType != null)
                        {
                            missing = argument2.Value;
                        }
                    }
                    else
                    {
                        object[] objArray2 = this.GetCustomAttributes(s_CustomConstantAttributeType, false);
                        if (objArray2.Length != 0)
                        {
                            missing = ((CustomConstantAttribute) objArray2[0]).Value;
                        }
                        else
                        {
                            objArray2 = this.GetCustomAttributes(s_DecimalConstantAttributeType, false);
                            if (objArray2.Length != 0)
                            {
                                missing = ((DecimalConstantAttribute) objArray2[0]).Value;
                            }
                        }
                    }
                }
                if ((missing == DBNull.Value) && this.IsOptional)
                {
                    missing = Type.Missing;
                }
            }
            return missing;
        }

        public virtual Type[] GetOptionalCustomModifiers()
        {
            if (this.IsLegacyParameterInfo)
            {
                return new Type[0];
            }
            return this.m_signature.GetCustomModifiers(this.PositionImpl + 1, false);
        }

        internal static ParameterInfo[] GetParameters(MethodBase method, MemberInfo member, Signature sig)
        {
            ParameterInfo info;
            return GetParameters(method, member, sig, out info, false);
        }

        internal static unsafe ParameterInfo[] GetParameters(MethodBase method, MemberInfo member, Signature sig, out ParameterInfo returnParameter, bool fetchReturnParameter)
        {
            RuntimeMethodHandle methodHandle = method.GetMethodHandle();
            returnParameter = null;
            int length = sig.Arguments.Length;
            ParameterInfo[] infoArray = fetchReturnParameter ? null : new ParameterInfo[length];
            int methodDef = methodHandle.GetMethodDef();
            int count = 0;
            if (!System.Reflection.MetadataToken.IsNullToken(methodDef))
            {
                MetadataImport metadataImport = methodHandle.GetDeclaringType().GetModuleHandle().GetMetadataImport();
                count = metadataImport.EnumParamsCount(methodDef);
                int* result = stackalloc int[count];
                metadataImport.EnumParams(methodDef, result, count);
                for (uint i = 0; i < count; i++)
                {
                    ParameterAttributes attributes;
                    int num5;
                    int parameterToken = result[(int) ((int*) i)];
                    metadataImport.GetParamDefProps(parameterToken, out num5, out attributes);
                    num5--;
                    if (fetchReturnParameter && (num5 == -1))
                    {
                        returnParameter = new ParameterInfo(sig, metadataImport, parameterToken, num5, attributes, member);
                    }
                    else if (!fetchReturnParameter && (num5 >= 0))
                    {
                        infoArray[num5] = new ParameterInfo(sig, metadataImport, parameterToken, num5, attributes, member);
                    }
                }
            }
            if (fetchReturnParameter)
            {
                if (returnParameter == null)
                {
                    returnParameter = new ParameterInfo(sig, MetadataImport.EmptyImport, 0, -1, ParameterAttributes.None, member);
                }
                return infoArray;
            }
            if (count < (infoArray.Length + 1))
            {
                for (int j = 0; j < infoArray.Length; j++)
                {
                    if (infoArray[j] == null)
                    {
                        infoArray[j] = new ParameterInfo(sig, MetadataImport.EmptyImport, 0, j, ParameterAttributes.None, member);
                    }
                }
            }
            return infoArray;
        }

        public virtual Type[] GetRequiredCustomModifiers()
        {
            if (this.IsLegacyParameterInfo)
            {
                return new Type[0];
            }
            return this.m_signature.GetCustomModifiers(this.PositionImpl + 1, true);
        }

        internal static ParameterInfo GetReturnParameter(MethodBase method, MemberInfo member, Signature sig)
        {
            ParameterInfo info;
            GetParameters(method, member, sig, out info, true);
            return info;
        }

        public virtual bool IsDefined(Type attributeType, bool inherit)
        {
            if (this.IsLegacyParameterInfo)
            {
                return false;
            }
            if (attributeType == null)
            {
                throw new ArgumentNullException("attributeType");
            }
            if (System.Reflection.MetadataToken.IsNullToken(this.m_tkParamDef))
            {
                return false;
            }
            RuntimeType underlyingSystemType = attributeType.UnderlyingSystemType as RuntimeType;
            if (underlyingSystemType == null)
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_MustBeType"), "attributeType");
            }
            return CustomAttribute.IsDefined(this, underlyingSystemType);
        }

        internal void OnCacheClear(object sender, ClearCacheEventArgs cacheEventArgs)
        {
            this.m_cachedData = null;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            ParameterInfo returnParameter = null;
            if (this.MemberImpl == null)
            {
                throw new SerializationException(Environment.GetResourceString("Serialization_InsufficientState"));
            }
            ParameterInfo[] indexParameters = null;
            MemberTypes memberType = this.MemberImpl.MemberType;
            if ((memberType != MemberTypes.Constructor) && (memberType != MemberTypes.Method))
            {
                if (memberType != MemberTypes.Property)
                {
                    throw new SerializationException(Environment.GetResourceString("Serialization_NoParameterInfo"));
                }
            }
            else
            {
                if (this.PositionImpl != -1)
                {
                    indexParameters = ((MethodBase) this.MemberImpl).GetParametersNoCopy();
                    if ((indexParameters == null) || (this.PositionImpl >= indexParameters.Length))
                    {
                        throw new SerializationException(Environment.GetResourceString("Serialization_BadParameterInfo"));
                    }
                    returnParameter = indexParameters[this.PositionImpl];
                }
                else
                {
                    if (this.MemberImpl.MemberType != MemberTypes.Method)
                    {
                        throw new SerializationException(Environment.GetResourceString("Serialization_BadParameterInfo"));
                    }
                    returnParameter = ((MethodInfo) this.MemberImpl).ReturnParameter;
                }
                goto Label_0104;
            }
            indexParameters = ((PropertyInfo) this.MemberImpl).GetIndexParameters();
            if (((indexParameters == null) || (this.PositionImpl <= -1)) || (this.PositionImpl >= indexParameters.Length))
            {
                throw new SerializationException(Environment.GetResourceString("Serialization_BadParameterInfo"));
            }
            returnParameter = indexParameters[this.PositionImpl];
        Label_0104:
            this.m_tkParamDef = returnParameter.m_tkParamDef;
            this.m_scope = returnParameter.m_scope;
            this.m_signature = returnParameter.m_signature;
            this.m_nameIsCached = true;
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            Type parameterType = this.ParameterType;
            string name = this.Name;
            this.DefaultValueImpl = this.DefaultValue;
            this._importer = IntPtr.Zero;
            this._token = this.m_tkParamDef;
            this.bExtraConstChecked = false;
        }

        internal void SetAttributes(ParameterAttributes attributes)
        {
            this.AttrsImpl = attributes;
        }

        internal void SetName(string name)
        {
            this.NameImpl = name;
        }

        void _ParameterInfo.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
        {
            throw new NotImplementedException();
        }

        void _ParameterInfo.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
        {
            throw new NotImplementedException();
        }

        void _ParameterInfo.GetTypeInfoCount(out uint pcTInfo)
        {
            throw new NotImplementedException();
        }

        void _ParameterInfo.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return (this.ParameterType.SigToString() + " " + this.Name);
        }

        public virtual ParameterAttributes Attributes
        {
            get
            {
                return this.AttrsImpl;
            }
        }

        internal InternalCache Cache
        {
            get
            {
                InternalCache cachedData = this.m_cachedData;
                if (cachedData == null)
                {
                    cachedData = new InternalCache("ParameterInfo");
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

        public virtual object DefaultValue
        {
            get
            {
                return this.GetDefaultValue(false);
            }
        }

        public bool IsIn
        {
            get
            {
                return ((this.Attributes & ParameterAttributes.In) != ParameterAttributes.None);
            }
        }

        public bool IsLcid
        {
            get
            {
                return ((this.Attributes & ParameterAttributes.Lcid) != ParameterAttributes.None);
            }
        }

        private bool IsLegacyParameterInfo
        {
            get
            {
                return (base.GetType() != typeof(ParameterInfo));
            }
        }

        public bool IsOptional
        {
            get
            {
                return ((this.Attributes & ParameterAttributes.Optional) != ParameterAttributes.None);
            }
        }

        public bool IsOut
        {
            get
            {
                return ((this.Attributes & ParameterAttributes.Out) != ParameterAttributes.None);
            }
        }

        public bool IsRetval
        {
            get
            {
                return ((this.Attributes & ParameterAttributes.Retval) != ParameterAttributes.None);
            }
        }

        public virtual MemberInfo Member
        {
            get
            {
                return this.MemberImpl;
            }
        }

        public int MetadataToken
        {
            get
            {
                return this.m_tkParamDef;
            }
        }

        public virtual string Name
        {
            get
            {
                if (!this.m_nameIsCached)
                {
                    if (!System.Reflection.MetadataToken.IsNullToken(this.m_tkParamDef))
                    {
                        string str = this.m_scope.GetName(this.m_tkParamDef).ToString();
                        this.NameImpl = str;
                    }
                    this.m_nameIsCached = true;
                }
                return this.NameImpl;
            }
        }

        public virtual Type ParameterType
        {
            get
            {
                if ((this.ClassImpl == null) && (base.GetType() == typeof(ParameterInfo)))
                {
                    RuntimeTypeHandle returnTypeHandle;
                    if (this.PositionImpl == -1)
                    {
                        returnTypeHandle = this.m_signature.ReturnTypeHandle;
                    }
                    else
                    {
                        returnTypeHandle = this.m_signature.Arguments[this.PositionImpl];
                    }
                    this.ClassImpl = returnTypeHandle.GetRuntimeType();
                }
                return this.ClassImpl;
            }
        }

        public virtual int Position
        {
            get
            {
                return this.PositionImpl;
            }
        }

        public virtual object RawDefaultValue
        {
            get
            {
                return this.GetDefaultValue(true);
            }
        }

        [Flags]
        private enum WhatIsCached
        {
            All = 7,
            DefaultValue = 4,
            Name = 1,
            Nothing = 0,
            ParameterType = 2
        }
    }
}

