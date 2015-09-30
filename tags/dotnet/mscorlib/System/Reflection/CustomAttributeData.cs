namespace System.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.InteropServices;

    [Serializable, ComVisible(true)]
    public sealed class CustomAttributeData
    {
        private ConstructorInfo m_ctor;
        private CustomAttributeCtorParameter[] m_ctorParams;
        private MemberInfo[] m_members;
        private IList<CustomAttributeNamedArgument> m_namedArgs;
        private CustomAttributeNamedParameter[] m_namedParams;
        private Module m_scope;
        private IList<CustomAttributeTypedArgument> m_typedCtorArgs;

        internal CustomAttributeData(Attribute attribute)
        {
            if (attribute is DllImportAttribute)
            {
                this.Init((DllImportAttribute) attribute);
            }
            else if (attribute is FieldOffsetAttribute)
            {
                this.Init((FieldOffsetAttribute) attribute);
            }
            else if (attribute is MarshalAsAttribute)
            {
                this.Init((MarshalAsAttribute) attribute);
            }
            else
            {
                this.Init(attribute);
            }
        }

        internal CustomAttributeData(Module scope, CustomAttributeRecord caRecord)
        {
            this.m_scope = scope;
            this.m_ctor = (ConstructorInfo) RuntimeType.GetMethodBase(scope, (int) caRecord.tkCtor);
            ParameterInfo[] parametersNoCopy = this.m_ctor.GetParametersNoCopy();
            this.m_ctorParams = new CustomAttributeCtorParameter[parametersNoCopy.Length];
            for (int i = 0; i < parametersNoCopy.Length; i++)
            {
                this.m_ctorParams[i] = new CustomAttributeCtorParameter(InitCustomAttributeType(parametersNoCopy[i].ParameterType, scope));
            }
            FieldInfo[] fields = this.m_ctor.DeclaringType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo[] properties = this.m_ctor.DeclaringType.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            this.m_namedParams = new CustomAttributeNamedParameter[properties.Length + fields.Length];
            for (int j = 0; j < fields.Length; j++)
            {
                this.m_namedParams[j] = new CustomAttributeNamedParameter(fields[j].Name, CustomAttributeEncoding.Field, InitCustomAttributeType(fields[j].FieldType, scope));
            }
            for (int k = 0; k < properties.Length; k++)
            {
                this.m_namedParams[k + fields.Length] = new CustomAttributeNamedParameter(properties[k].Name, CustomAttributeEncoding.Property, InitCustomAttributeType(properties[k].PropertyType, scope));
            }
            this.m_members = new MemberInfo[fields.Length + properties.Length];
            fields.CopyTo(this.m_members, 0);
            properties.CopyTo(this.m_members, fields.Length);
            CustomAttributeEncodedArgument.ParseAttributeArguments(caRecord.blob, ref this.m_ctorParams, ref this.m_namedParams, this.m_scope);
        }

        public override bool Equals(object obj)
        {
            return (obj == this);
        }

        internal static CustomAttributeTypedArgument Filter(IList<CustomAttributeData> attrs, Type caType, int parameter)
        {
            for (int i = 0; i < attrs.Count; i++)
            {
                if (attrs[i].Constructor.DeclaringType == caType)
                {
                    return attrs[i].ConstructorArguments[parameter];
                }
            }
            return new CustomAttributeTypedArgument();
        }

        internal static CustomAttributeTypedArgument Filter(IList<CustomAttributeData> attrs, Type caType, string name)
        {
            for (int i = 0; i < attrs.Count; i++)
            {
                if (attrs[i].Constructor.DeclaringType == caType)
                {
                    IList<CustomAttributeNamedArgument> namedArguments = attrs[i].NamedArguments;
                    for (int j = 0; j < namedArguments.Count; j++)
                    {
                        CustomAttributeNamedArgument argument = namedArguments[j];
                        if (argument.MemberInfo.Name.Equals(name))
                        {
                            CustomAttributeNamedArgument argument2 = namedArguments[j];
                            return argument2.TypedValue;
                        }
                    }
                }
            }
            return new CustomAttributeTypedArgument();
        }

        internal static unsafe CustomAttributeRecord[] GetCustomAttributeRecords(Module module, int targetToken)
        {
            MetadataImport metadataImport = module.MetadataImport;
            int count = metadataImport.EnumCustomAttributesCount(targetToken);
            int* result = stackalloc int[count];
            metadataImport.EnumCustomAttributes(targetToken, result, count);
            CustomAttributeRecord[] recordArray = new CustomAttributeRecord[count];
            for (int i = 0; i < count; i++)
            {
                metadataImport.GetCustomAttributeProps(result[i], out recordArray[i].tkCtor.Value, out recordArray[i].blob);
            }
            return recordArray;
        }

        public static IList<CustomAttributeData> GetCustomAttributes(Assembly target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            return GetCustomAttributes(target.ManifestModule, target.AssemblyHandle.GetToken());
        }

        public static IList<CustomAttributeData> GetCustomAttributes(MemberInfo target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            IList<CustomAttributeData> customAttributes = GetCustomAttributes(target.Module, target.MetadataToken);
            int count = 0;
            Attribute[] attributeArray = null;
            if (target is RuntimeType)
            {
                attributeArray = PseudoCustomAttribute.GetCustomAttributes((RuntimeType) target, typeof(object), false, out count);
            }
            else if (target is RuntimeMethodInfo)
            {
                attributeArray = PseudoCustomAttribute.GetCustomAttributes((RuntimeMethodInfo) target, typeof(object), false, out count);
            }
            else if (target is RuntimeFieldInfo)
            {
                attributeArray = PseudoCustomAttribute.GetCustomAttributes((RuntimeFieldInfo) target, typeof(object), out count);
            }
            if (count == 0)
            {
                return customAttributes;
            }
            CustomAttributeData[] array = new CustomAttributeData[customAttributes.Count + count];
            customAttributes.CopyTo(array, count);
            for (int i = 0; i < count; i++)
            {
                if (!PseudoCustomAttribute.IsSecurityAttribute(attributeArray[i].GetType()))
                {
                    array[i] = new CustomAttributeData(attributeArray[i]);
                }
            }
            return Array.AsReadOnly<CustomAttributeData>(array);
        }

        public static IList<CustomAttributeData> GetCustomAttributes(Module target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            if (target.IsResourceInternal())
            {
                return new List<CustomAttributeData>();
            }
            return GetCustomAttributes(target, target.MetadataToken);
        }

        public static IList<CustomAttributeData> GetCustomAttributes(ParameterInfo target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            IList<CustomAttributeData> customAttributes = GetCustomAttributes(target.Member.Module, target.MetadataToken);
            int count = 0;
            Attribute[] attributeArray = PseudoCustomAttribute.GetCustomAttributes(target, typeof(object), out count);
            if (count == 0)
            {
                return customAttributes;
            }
            CustomAttributeData[] array = new CustomAttributeData[customAttributes.Count + count];
            customAttributes.CopyTo(array, count);
            for (int i = 0; i < count; i++)
            {
                array[i] = new CustomAttributeData(attributeArray[i]);
            }
            return Array.AsReadOnly<CustomAttributeData>(array);
        }

        private static IList<CustomAttributeData> GetCustomAttributes(Module module, int tkTarget)
        {
            CustomAttributeRecord[] customAttributeRecords = GetCustomAttributeRecords(module, tkTarget);
            CustomAttributeData[] array = new CustomAttributeData[customAttributeRecords.Length];
            for (int i = 0; i < customAttributeRecords.Length; i++)
            {
                array[i] = new CustomAttributeData(module, customAttributeRecords[i]);
            }
            return Array.AsReadOnly<CustomAttributeData>(array);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private void Init(object pca)
        {
            this.m_ctor = pca.GetType().GetConstructors(BindingFlags.Public | BindingFlags.Instance)[0];
            this.m_typedCtorArgs = Array.AsReadOnly<CustomAttributeTypedArgument>(new CustomAttributeTypedArgument[0]);
            this.m_namedArgs = Array.AsReadOnly<CustomAttributeNamedArgument>(new CustomAttributeNamedArgument[0]);
        }

        private void Init(DllImportAttribute dllImport)
        {
            Type type = typeof(DllImportAttribute);
            this.m_ctor = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)[0];
            CustomAttributeTypedArgument[] array = new CustomAttributeTypedArgument[] { new CustomAttributeTypedArgument(dllImport.Value) };
            this.m_typedCtorArgs = Array.AsReadOnly<CustomAttributeTypedArgument>(array);
            CustomAttributeNamedArgument[] argumentArray2 = new CustomAttributeNamedArgument[] { new CustomAttributeNamedArgument(type.GetField("EntryPoint"), dllImport.EntryPoint), new CustomAttributeNamedArgument(type.GetField("CharSet"), dllImport.CharSet), new CustomAttributeNamedArgument(type.GetField("ExactSpelling"), dllImport.ExactSpelling), new CustomAttributeNamedArgument(type.GetField("SetLastError"), dllImport.SetLastError), new CustomAttributeNamedArgument(type.GetField("PreserveSig"), dllImport.PreserveSig), new CustomAttributeNamedArgument(type.GetField("CallingConvention"), dllImport.CallingConvention), new CustomAttributeNamedArgument(type.GetField("BestFitMapping"), dllImport.BestFitMapping), new CustomAttributeNamedArgument(type.GetField("ThrowOnUnmappableChar"), dllImport.ThrowOnUnmappableChar) };
            this.m_namedArgs = Array.AsReadOnly<CustomAttributeNamedArgument>(argumentArray2);
        }

        private void Init(FieldOffsetAttribute fieldOffset)
        {
            this.m_ctor = typeof(FieldOffsetAttribute).GetConstructors(BindingFlags.Public | BindingFlags.Instance)[0];
            CustomAttributeTypedArgument[] array = new CustomAttributeTypedArgument[] { new CustomAttributeTypedArgument(fieldOffset.Value) };
            this.m_typedCtorArgs = Array.AsReadOnly<CustomAttributeTypedArgument>(array);
            this.m_namedArgs = Array.AsReadOnly<CustomAttributeNamedArgument>(new CustomAttributeNamedArgument[0]);
        }

        private void Init(MarshalAsAttribute marshalAs)
        {
            Type type = typeof(MarshalAsAttribute);
            this.m_ctor = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)[0];
            CustomAttributeTypedArgument[] array = new CustomAttributeTypedArgument[] { new CustomAttributeTypedArgument(marshalAs.Value) };
            this.m_typedCtorArgs = Array.AsReadOnly<CustomAttributeTypedArgument>(array);
            int num = 3;
            if (marshalAs.MarshalType != null)
            {
                num++;
            }
            if (marshalAs.MarshalTypeRef != null)
            {
                num++;
            }
            if (marshalAs.MarshalCookie != null)
            {
                num++;
            }
            num++;
            num++;
            if (marshalAs.SafeArrayUserDefinedSubType != null)
            {
                num++;
            }
            CustomAttributeNamedArgument[] argumentArray = new CustomAttributeNamedArgument[num];
            num = 0;
            argumentArray[num++] = new CustomAttributeNamedArgument(type.GetField("ArraySubType"), marshalAs.ArraySubType);
            argumentArray[num++] = new CustomAttributeNamedArgument(type.GetField("SizeParamIndex"), marshalAs.SizeParamIndex);
            argumentArray[num++] = new CustomAttributeNamedArgument(type.GetField("SizeConst"), marshalAs.SizeConst);
            argumentArray[num++] = new CustomAttributeNamedArgument(type.GetField("IidParameterIndex"), marshalAs.IidParameterIndex);
            argumentArray[num++] = new CustomAttributeNamedArgument(type.GetField("SafeArraySubType"), marshalAs.SafeArraySubType);
            if (marshalAs.MarshalType != null)
            {
                argumentArray[num++] = new CustomAttributeNamedArgument(type.GetField("MarshalType"), marshalAs.MarshalType);
            }
            if (marshalAs.MarshalTypeRef != null)
            {
                argumentArray[num++] = new CustomAttributeNamedArgument(type.GetField("MarshalTypeRef"), marshalAs.MarshalTypeRef);
            }
            if (marshalAs.MarshalCookie != null)
            {
                argumentArray[num++] = new CustomAttributeNamedArgument(type.GetField("MarshalCookie"), marshalAs.MarshalCookie);
            }
            if (marshalAs.SafeArrayUserDefinedSubType != null)
            {
                argumentArray[num++] = new CustomAttributeNamedArgument(type.GetField("SafeArrayUserDefinedSubType"), marshalAs.SafeArrayUserDefinedSubType);
            }
            this.m_namedArgs = Array.AsReadOnly<CustomAttributeNamedArgument>(argumentArray);
        }

        private static CustomAttributeType InitCustomAttributeType(Type parameterType, Module scope)
        {
            CustomAttributeEncoding encodedType = TypeToCustomAttributeEncoding(parameterType);
            CustomAttributeEncoding undefined = CustomAttributeEncoding.Undefined;
            CustomAttributeEncoding encodedEnumType = CustomAttributeEncoding.Undefined;
            string enumName = null;
            if (encodedType == CustomAttributeEncoding.Array)
            {
                parameterType = parameterType.GetElementType();
                undefined = TypeToCustomAttributeEncoding(parameterType);
            }
            if ((encodedType == CustomAttributeEncoding.Enum) || (undefined == CustomAttributeEncoding.Enum))
            {
                encodedEnumType = TypeToCustomAttributeEncoding(Enum.GetUnderlyingType(parameterType));
                if (parameterType.Module == scope)
                {
                    enumName = parameterType.FullName;
                }
                else
                {
                    enumName = parameterType.AssemblyQualifiedName;
                }
            }
            return new CustomAttributeType(encodedType, undefined, encodedEnumType, enumName);
        }

        public override string ToString()
        {
            string str = "";
            for (int i = 0; i < this.ConstructorArguments.Count; i++)
            {
                str = str + string.Format(CultureInfo.CurrentCulture, (i == 0) ? "{0}" : ", {0}", new object[] { this.ConstructorArguments[i] });
            }
            string str2 = "";
            for (int j = 0; j < this.NamedArguments.Count; j++)
            {
                str2 = str2 + string.Format(CultureInfo.CurrentCulture, ((j == 0) && (str.Length == 0)) ? "{0}" : ", {0}", new object[] { this.NamedArguments[j] });
            }
            return string.Format(CultureInfo.CurrentCulture, "[{0}({1}{2})]", new object[] { this.Constructor.DeclaringType.FullName, str, str2 });
        }

        private static CustomAttributeEncoding TypeToCustomAttributeEncoding(Type type)
        {
            if (type == typeof(int))
            {
                return CustomAttributeEncoding.Int32;
            }
            if (type.IsEnum)
            {
                return CustomAttributeEncoding.Enum;
            }
            if (type == typeof(string))
            {
                return CustomAttributeEncoding.String;
            }
            if (type == typeof(Type))
            {
                return CustomAttributeEncoding.Type;
            }
            if (type == typeof(object))
            {
                return CustomAttributeEncoding.Object;
            }
            if (type.IsArray)
            {
                return CustomAttributeEncoding.Array;
            }
            if (type == typeof(char))
            {
                return CustomAttributeEncoding.Char;
            }
            if (type == typeof(bool))
            {
                return CustomAttributeEncoding.Boolean;
            }
            if (type == typeof(byte))
            {
                return CustomAttributeEncoding.Byte;
            }
            if (type == typeof(sbyte))
            {
                return CustomAttributeEncoding.SByte;
            }
            if (type == typeof(short))
            {
                return CustomAttributeEncoding.Int16;
            }
            if (type == typeof(ushort))
            {
                return CustomAttributeEncoding.UInt16;
            }
            if (type == typeof(uint))
            {
                return CustomAttributeEncoding.UInt32;
            }
            if (type == typeof(long))
            {
                return CustomAttributeEncoding.Int64;
            }
            if (type == typeof(ulong))
            {
                return CustomAttributeEncoding.UInt64;
            }
            if (type == typeof(float))
            {
                return CustomAttributeEncoding.Float;
            }
            if (type == typeof(double))
            {
                return CustomAttributeEncoding.Double;
            }
            if (type.IsClass)
            {
                return CustomAttributeEncoding.Object;
            }
            if (type.IsInterface)
            {
                return CustomAttributeEncoding.Object;
            }
            if (!type.IsValueType)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidKindOfTypeForCA"), "type");
            }
            return CustomAttributeEncoding.Undefined;
        }

        [ComVisible(true)]
        public ConstructorInfo Constructor
        {
            get
            {
                return this.m_ctor;
            }
        }

        [ComVisible(true)]
        public IList<CustomAttributeTypedArgument> ConstructorArguments
        {
            get
            {
                if (this.m_typedCtorArgs == null)
                {
                    CustomAttributeTypedArgument[] array = new CustomAttributeTypedArgument[this.m_ctorParams.Length];
                    for (int i = 0; i < array.Length; i++)
                    {
                        CustomAttributeEncodedArgument customAttributeEncodedArgument = this.m_ctorParams[i].CustomAttributeEncodedArgument;
                        array[i] = new CustomAttributeTypedArgument(this.m_scope, this.m_ctorParams[i].CustomAttributeEncodedArgument);
                    }
                    this.m_typedCtorArgs = Array.AsReadOnly<CustomAttributeTypedArgument>(array);
                }
                return this.m_typedCtorArgs;
            }
        }

        public IList<CustomAttributeNamedArgument> NamedArguments
        {
            get
            {
                if (this.m_namedArgs == null)
                {
                    if (this.m_namedParams == null)
                    {
                        return null;
                    }
                    int num = 0;
                    for (int i = 0; i < this.m_namedParams.Length; i++)
                    {
                        if (this.m_namedParams[i].EncodedArgument.CustomAttributeType.EncodedType != CustomAttributeEncoding.Undefined)
                        {
                            num++;
                        }
                    }
                    CustomAttributeNamedArgument[] array = new CustomAttributeNamedArgument[num];
                    int index = 0;
                    int num4 = 0;
                    while (index < this.m_namedParams.Length)
                    {
                        if (this.m_namedParams[index].EncodedArgument.CustomAttributeType.EncodedType != CustomAttributeEncoding.Undefined)
                        {
                            array[num4++] = new CustomAttributeNamedArgument(this.m_members[index], new CustomAttributeTypedArgument(this.m_scope, this.m_namedParams[index].EncodedArgument));
                        }
                        index++;
                    }
                    this.m_namedArgs = Array.AsReadOnly<CustomAttributeNamedArgument>(array);
                }
                return this.m_namedArgs;
            }
        }
    }
}

