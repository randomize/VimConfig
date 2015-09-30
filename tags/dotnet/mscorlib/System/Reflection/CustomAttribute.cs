namespace System.Reflection
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal static class CustomAttribute
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern unsafe object _CreateCaObject(void* pModule, void* pCtor, byte** ppBlob, byte* pEndBlob, int* pcNamedArgs);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern unsafe void _GetPropertyOrFieldData(IntPtr pModule, byte** ppBlobStart, byte* pBlobEnd, out string name, out bool bIsProperty, out Type type, out object value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void _ParseAttributeUsageAttribute(IntPtr pCa, int cCa, out int targets, out bool inherited, out bool allowMultiple);
        private static bool AttributeUsageCheck(RuntimeType attributeType, bool mustBeInheritable, object[] attributes, IList derivedAttributes)
        {
            AttributeUsageAttribute attributeUsage = null;
            if (mustBeInheritable)
            {
                attributeUsage = GetAttributeUsage(attributeType);
                if (!attributeUsage.Inherited)
                {
                    return false;
                }
            }
            if (derivedAttributes != null)
            {
                for (int i = 0; i < derivedAttributes.Count; i++)
                {
                    if (derivedAttributes[i].GetType() == attributeType)
                    {
                        if (attributeUsage == null)
                        {
                            attributeUsage = GetAttributeUsage(attributeType);
                        }
                        return attributeUsage.AllowMultiple;
                    }
                }
            }
            return true;
        }

        private static unsafe object CreateCaObject(Module module, RuntimeMethodHandle ctor, ref IntPtr blob, IntPtr blobEnd, out int namedArgs)
        {
            int num;
            byte* ppBlob = (byte*) blob;
            byte* pEndBlob = (byte*) blobEnd;
            object obj2 = _CreateCaObject(module.ModuleHandle.Value, (void*) ctor.Value, &ppBlob, pEndBlob, &num);
            blob = (IntPtr) ppBlob;
            namedArgs = num;
            return obj2;
        }

        internal static unsafe bool FilterCustomAttributeRecord(CustomAttributeRecord caRecord, MetadataImport scope, ref Assembly lastAptcaOkAssembly, Module decoratedModule, MetadataToken decoratedToken, RuntimeType attributeFilterType, bool mustBeInheritable, object[] attributes, IList derivedAttributes, out RuntimeType attributeType, out RuntimeMethodHandle ctor, out bool ctorHasParameters, out bool isVarArg)
        {
            ctor = new RuntimeMethodHandle();
            attributeType = null;
            ctorHasParameters = false;
            isVarArg = false;
            IntPtr ptr1 = (IntPtr) (((void*) caRecord.blob.Signature) + caRecord.blob.Length);
            attributeType = decoratedModule.ResolveType(scope.GetParentToken((int) caRecord.tkCtor), null, null) as RuntimeType;
            if (!attributeFilterType.IsAssignableFrom(attributeType))
            {
                return false;
            }
            if (!AttributeUsageCheck(attributeType, mustBeInheritable, attributes, derivedAttributes))
            {
                return false;
            }
            if ((attributeType.Assembly != lastAptcaOkAssembly) && !attributeType.Assembly.AptcaCheck(decoratedModule.Assembly))
            {
                return false;
            }
            lastAptcaOkAssembly = decoratedModule.Assembly;
            ConstArray methodSignature = scope.GetMethodSignature(caRecord.tkCtor);
            isVarArg = (methodSignature[0] & 5) != 0;
            ctorHasParameters = methodSignature[1] != 0;
            if (ctorHasParameters)
            {
                ctor = decoratedModule.ModuleHandle.ResolveMethodHandle((int) caRecord.tkCtor);
            }
            else
            {
                ctor = attributeType.GetTypeHandleInternal().GetDefaultConstructor();
                if (ctor.IsNullHandle() && !attributeType.IsValueType)
                {
                    throw new MissingMethodException(".ctor");
                }
            }
            if (ctor.IsNullHandle())
            {
                if (!attributeType.IsVisible && !attributeType.TypeHandle.IsVisibleFromModule(decoratedModule.ModuleHandle))
                {
                    return false;
                }
                return true;
            }
            if (ctor.IsVisibleFromModule(decoratedModule))
            {
                return true;
            }
            MetadataToken token = new MetadataToken();
            if (decoratedToken.IsParamDef)
            {
                token = new MetadataToken(scope.GetParentToken((int) decoratedToken));
                token = new MetadataToken(scope.GetParentToken((int) token));
            }
            else if ((decoratedToken.IsMethodDef || decoratedToken.IsProperty) || (decoratedToken.IsEvent || decoratedToken.IsFieldDef))
            {
                token = new MetadataToken(scope.GetParentToken((int) decoratedToken));
            }
            else if (decoratedToken.IsTypeDef)
            {
                token = decoratedToken;
            }
            return (token.IsTypeDef && ctor.IsVisibleFromType(decoratedModule.ModuleHandle.ResolveTypeHandle((int) token)));
        }

        internal static AttributeUsageAttribute GetAttributeUsage(RuntimeType decoratedAttribute)
        {
            Module module = decoratedAttribute.Module;
            MetadataImport metadataImport = module.MetadataImport;
            CustomAttributeRecord[] customAttributeRecords = CustomAttributeData.GetCustomAttributeRecords(module, decoratedAttribute.MetadataToken);
            AttributeUsageAttribute attribute = null;
            for (int i = 0; i < customAttributeRecords.Length; i++)
            {
                CustomAttributeRecord record = customAttributeRecords[i];
                RuntimeType type = module.ResolveType(metadataImport.GetParentToken((int) record.tkCtor), null, null) as RuntimeType;
                if (type == typeof(AttributeUsageAttribute))
                {
                    AttributeTargets targets;
                    bool flag;
                    bool flag2;
                    if (attribute != null)
                    {
                        throw new FormatException(string.Format(CultureInfo.CurrentUICulture, Environment.GetResourceString("Format_AttributeUsage"), new object[] { type }));
                    }
                    ParseAttributeUsageAttribute(record.blob, out targets, out flag, out flag2);
                    attribute = new AttributeUsageAttribute(targets, flag2, flag);
                }
            }
            if (attribute == null)
            {
                return AttributeUsageAttribute.Default;
            }
            return attribute;
        }

        internal static object[] GetCustomAttributes(Assembly assembly, RuntimeType caType)
        {
            int count = 0;
            Attribute[] sourceArray = PseudoCustomAttribute.GetCustomAttributes(assembly, caType, out count);
            object[] destinationArray = GetCustomAttributes(assembly.ManifestModule, assembly.AssemblyHandle.GetToken(), count, caType);
            if (count > 0)
            {
                Array.Copy(sourceArray, 0, destinationArray, destinationArray.Length - count, count);
            }
            return destinationArray;
        }

        internal static object[] GetCustomAttributes(Module module, RuntimeType caType)
        {
            int count = 0;
            Attribute[] sourceArray = PseudoCustomAttribute.GetCustomAttributes(module, caType, out count);
            object[] destinationArray = GetCustomAttributes(module, module.MetadataToken, count, caType);
            if (count > 0)
            {
                Array.Copy(sourceArray, 0, destinationArray, destinationArray.Length - count, count);
            }
            return destinationArray;
        }

        internal static object[] GetCustomAttributes(ParameterInfo parameter, RuntimeType caType)
        {
            int count = 0;
            Attribute[] sourceArray = PseudoCustomAttribute.GetCustomAttributes(parameter, caType, out count);
            object[] destinationArray = GetCustomAttributes(parameter.Member.Module, parameter.MetadataToken, count, caType);
            if (count > 0)
            {
                Array.Copy(sourceArray, 0, destinationArray, destinationArray.Length - count, count);
            }
            return destinationArray;
        }

        internal static object[] GetCustomAttributes(RuntimeConstructorInfo ctor, RuntimeType caType)
        {
            int count = 0;
            Attribute[] sourceArray = PseudoCustomAttribute.GetCustomAttributes(ctor, caType, true, out count);
            object[] destinationArray = GetCustomAttributes(ctor.Module, ctor.MetadataToken, count, caType);
            if (count > 0)
            {
                Array.Copy(sourceArray, 0, destinationArray, destinationArray.Length - count, count);
            }
            return destinationArray;
        }

        internal static object[] GetCustomAttributes(RuntimeEventInfo e, RuntimeType caType)
        {
            int count = 0;
            Attribute[] sourceArray = PseudoCustomAttribute.GetCustomAttributes(e, caType, out count);
            object[] destinationArray = GetCustomAttributes(e.Module, e.MetadataToken, count, caType);
            if (count > 0)
            {
                Array.Copy(sourceArray, 0, destinationArray, destinationArray.Length - count, count);
            }
            return destinationArray;
        }

        internal static object[] GetCustomAttributes(RuntimeFieldInfo field, RuntimeType caType)
        {
            int count = 0;
            Attribute[] sourceArray = PseudoCustomAttribute.GetCustomAttributes(field, caType, out count);
            object[] destinationArray = GetCustomAttributes(field.Module, field.MetadataToken, count, caType);
            if (count > 0)
            {
                Array.Copy(sourceArray, 0, destinationArray, destinationArray.Length - count, count);
            }
            return destinationArray;
        }

        internal static object[] GetCustomAttributes(RuntimePropertyInfo property, RuntimeType caType)
        {
            int count = 0;
            Attribute[] sourceArray = PseudoCustomAttribute.GetCustomAttributes(property, caType, out count);
            object[] destinationArray = GetCustomAttributes(property.Module, property.MetadataToken, count, caType);
            if (count > 0)
            {
                Array.Copy(sourceArray, 0, destinationArray, destinationArray.Length - count, count);
            }
            return destinationArray;
        }

        internal static object[] GetCustomAttributes(RuntimeMethodInfo method, RuntimeType caType, bool inherit)
        {
            if (method.IsGenericMethod && !method.IsGenericMethodDefinition)
            {
                method = method.GetGenericMethodDefinition() as RuntimeMethodInfo;
            }
            int count = 0;
            Attribute[] sourceArray = PseudoCustomAttribute.GetCustomAttributes(method, caType, true, out count);
            if (!inherit || (caType.IsSealed && !GetAttributeUsage(caType).Inherited))
            {
                object[] objArray = GetCustomAttributes(method.Module, method.MetadataToken, count, caType);
                if (count > 0)
                {
                    Array.Copy(sourceArray, 0, objArray, objArray.Length - count, count);
                }
                return objArray;
            }
            List<object> derivedAttributes = new List<object>();
            bool mustBeInheritable = false;
            Type elementType = (((caType == null) || caType.IsValueType) || caType.ContainsGenericParameters) ? typeof(object) : caType;
            while (count > 0)
            {
                derivedAttributes.Add(sourceArray[--count]);
            }
            while (method != null)
            {
                object[] objArray2 = GetCustomAttributes(method.Module, method.MetadataToken, 0, caType, mustBeInheritable, derivedAttributes);
                mustBeInheritable = true;
                for (int i = 0; i < objArray2.Length; i++)
                {
                    derivedAttributes.Add(objArray2[i]);
                }
                method = method.GetParentDefinition() as RuntimeMethodInfo;
            }
            object[] destinationArray = Array.CreateInstance(elementType, derivedAttributes.Count) as object[];
            Array.Copy(derivedAttributes.ToArray(), 0, destinationArray, 0, derivedAttributes.Count);
            return destinationArray;
        }

        internal static object[] GetCustomAttributes(RuntimeType type, RuntimeType caType, bool inherit)
        {
            if (type.GetElementType() != null)
            {
                if (!caType.IsValueType)
                {
                    return (object[]) Array.CreateInstance(caType, 0);
                }
                return new object[0];
            }
            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                type = type.GetGenericTypeDefinition() as RuntimeType;
            }
            int count = 0;
            Attribute[] sourceArray = PseudoCustomAttribute.GetCustomAttributes(type, caType, true, out count);
            if (!inherit || (caType.IsSealed && !GetAttributeUsage(caType).Inherited))
            {
                object[] objArray = GetCustomAttributes(type.Module, type.MetadataToken, count, caType);
                if (count > 0)
                {
                    Array.Copy(sourceArray, 0, objArray, objArray.Length - count, count);
                }
                return objArray;
            }
            List<object> derivedAttributes = new List<object>();
            bool mustBeInheritable = false;
            Type elementType = (((caType == null) || caType.IsValueType) || caType.ContainsGenericParameters) ? typeof(object) : caType;
            while (count > 0)
            {
                derivedAttributes.Add(sourceArray[--count]);
            }
            while ((type != typeof(object)) && (type != null))
            {
                object[] objArray2 = GetCustomAttributes(type.Module, type.MetadataToken, 0, caType, mustBeInheritable, derivedAttributes);
                mustBeInheritable = true;
                for (int i = 0; i < objArray2.Length; i++)
                {
                    derivedAttributes.Add(objArray2[i]);
                }
                type = type.BaseType as RuntimeType;
            }
            object[] destinationArray = Array.CreateInstance(elementType, derivedAttributes.Count) as object[];
            Array.Copy(derivedAttributes.ToArray(), 0, destinationArray, 0, derivedAttributes.Count);
            return destinationArray;
        }

        internal static object[] GetCustomAttributes(Module decoratedModule, int decoratedMetadataToken, int pcaCount, RuntimeType attributeFilterType)
        {
            return GetCustomAttributes(decoratedModule, decoratedMetadataToken, pcaCount, attributeFilterType, false, null);
        }

        internal static unsafe object[] GetCustomAttributes(Module decoratedModule, int decoratedMetadataToken, int pcaCount, RuntimeType attributeFilterType, bool mustBeInheritable, IList derivedAttributes)
        {
            if (decoratedModule.Assembly.ReflectionOnly)
            {
                throw new InvalidOperationException(Environment.GetResourceString("Arg_ReflectionOnlyCA"));
            }
            MetadataImport metadataImport = decoratedModule.MetadataImport;
            CustomAttributeRecord[] customAttributeRecords = CustomAttributeData.GetCustomAttributeRecords(decoratedModule, decoratedMetadataToken);
            Type elementType = (((attributeFilterType == null) || attributeFilterType.IsValueType) || attributeFilterType.ContainsGenericParameters) ? typeof(object) : attributeFilterType;
            if ((attributeFilterType == null) && (customAttributeRecords.Length == 0))
            {
                return (Array.CreateInstance(elementType, 0) as object[]);
            }
            object[] attributes = Array.CreateInstance(elementType, customAttributeRecords.Length) as object[];
            int length = 0;
            SecurityContextFrame frame = new SecurityContextFrame();
            frame.Push(decoratedModule.Assembly.InternalAssembly);
            Assembly lastAptcaOkAssembly = null;
            for (int i = 0; i < customAttributeRecords.Length; i++)
            {
                bool flag2;
                bool flag3;
                object obj2 = null;
                CustomAttributeRecord caRecord = customAttributeRecords[i];
                RuntimeMethodHandle ctor = new RuntimeMethodHandle();
                RuntimeType attributeType = null;
                int namedArgs = 0;
                IntPtr signature = caRecord.blob.Signature;
                IntPtr blobEnd = (IntPtr) (((void*) signature) + caRecord.blob.Length);
                if (FilterCustomAttributeRecord(caRecord, metadataImport, ref lastAptcaOkAssembly, decoratedModule, decoratedMetadataToken, attributeFilterType, mustBeInheritable, attributes, derivedAttributes, out attributeType, out ctor, out flag2, out flag3))
                {
                    if (!ctor.IsNullHandle())
                    {
                        ctor.CheckLinktimeDemands(decoratedModule, decoratedMetadataToken);
                    }
                    RuntimeConstructorInfo.CheckCanCreateInstance(attributeType, flag3);
                    if (flag2)
                    {
                        obj2 = CreateCaObject(decoratedModule, ctor, ref signature, blobEnd, out namedArgs);
                    }
                    else
                    {
                        obj2 = attributeType.TypeHandle.CreateCaInstance(ctor);
                        if (Marshal.ReadInt16(signature) != 1)
                        {
                            throw new CustomAttributeFormatException();
                        }
                        signature = (IntPtr) (((void*) signature) + 2);
                        namedArgs = Marshal.ReadInt16(signature);
                        signature = (IntPtr) (((void*) signature) + 2);
                    }
                    for (int j = 0; j < namedArgs; j++)
                    {
                        string str;
                        bool flag4;
                        Type type3;
                        object obj3;
                        IntPtr ptr1 = caRecord.blob.Signature;
                        GetPropertyOrFieldData(decoratedModule, ref signature, blobEnd, out str, out flag4, out type3, out obj3);
                        try
                        {
                            if (flag4)
                            {
                                if ((type3 == null) && (obj3 != null))
                                {
                                    type3 = (obj3.GetType() == typeof(RuntimeType)) ? typeof(Type) : obj3.GetType();
                                }
                                RuntimePropertyInfo property = null;
                                if (type3 == null)
                                {
                                    property = attributeType.GetProperty(str) as RuntimePropertyInfo;
                                }
                                else
                                {
                                    property = attributeType.GetProperty(str, type3, Type.EmptyTypes) as RuntimePropertyInfo;
                                }
                                RuntimeMethodInfo setMethod = property.GetSetMethod(true) as RuntimeMethodInfo;
                                if (setMethod.IsPublic)
                                {
                                    setMethod.MethodHandle.CheckLinktimeDemands(decoratedModule, decoratedMetadataToken);
                                    setMethod.Invoke(obj2, BindingFlags.Default, null, new object[] { obj3 }, null, true);
                                }
                            }
                            else
                            {
                                (attributeType.GetField(str) as RtFieldInfo).InternalSetValue(obj2, obj3, BindingFlags.Default, Type.DefaultBinder, null, false);
                            }
                        }
                        catch (Exception exception)
                        {
                            throw new CustomAttributeFormatException(string.Format(CultureInfo.CurrentUICulture, Environment.GetResourceString(flag4 ? "RFLCT.InvalidPropFail" : "RFLCT.InvalidFieldFail"), new object[] { str }), exception);
                        }
                    }
                    if (!signature.Equals(blobEnd))
                    {
                        throw new CustomAttributeFormatException();
                    }
                    attributes[length++] = obj2;
                }
            }
            frame.Pop();
            if ((length == customAttributeRecords.Length) && (pcaCount == 0))
            {
                return attributes;
            }
            if (length == 0)
            {
                Array.CreateInstance(elementType, 0);
            }
            object[] destinationArray = Array.CreateInstance(elementType, (int) (length + pcaCount)) as object[];
            Array.Copy(attributes, 0, destinationArray, 0, length);
            return destinationArray;
        }

        private static unsafe void GetPropertyOrFieldData(Module module, ref IntPtr blobStart, IntPtr blobEnd, out string name, out bool isProperty, out Type type, out object value)
        {
            byte* ppBlobStart = (byte*) blobStart;
            _GetPropertyOrFieldData((IntPtr) module.ModuleHandle.Value, &ppBlobStart, (byte*) blobEnd, out name, out isProperty, out type, out value);
            blobStart = (IntPtr) ppBlobStart;
        }

        internal static bool IsCustomAttributeDefined(Module decoratedModule, int decoratedMetadataToken, RuntimeType attributeFilterType)
        {
            return IsCustomAttributeDefined(decoratedModule, decoratedMetadataToken, attributeFilterType, false);
        }

        internal static bool IsCustomAttributeDefined(Module decoratedModule, int decoratedMetadataToken, RuntimeType attributeFilterType, bool mustBeInheritable)
        {
            if (decoratedModule.Assembly.ReflectionOnly)
            {
                throw new InvalidOperationException(Environment.GetResourceString("Arg_ReflectionOnlyCA"));
            }
            MetadataImport metadataImport = decoratedModule.MetadataImport;
            CustomAttributeRecord[] customAttributeRecords = CustomAttributeData.GetCustomAttributeRecords(decoratedModule, decoratedMetadataToken);
            Assembly lastAptcaOkAssembly = null;
            for (int i = 0; i < customAttributeRecords.Length; i++)
            {
                RuntimeType type;
                RuntimeMethodHandle handle;
                bool flag;
                bool flag2;
                CustomAttributeRecord caRecord = customAttributeRecords[i];
                if (FilterCustomAttributeRecord(caRecord, metadataImport, ref lastAptcaOkAssembly, decoratedModule, decoratedMetadataToken, attributeFilterType, mustBeInheritable, null, null, out type, out handle, out flag, out flag2))
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool IsDefined(Assembly assembly, RuntimeType caType)
        {
            return (PseudoCustomAttribute.IsDefined(assembly, caType) || IsCustomAttributeDefined(assembly.ManifestModule, assembly.AssemblyHandle.GetToken(), caType));
        }

        internal static bool IsDefined(Module module, RuntimeType caType)
        {
            return (PseudoCustomAttribute.IsDefined(module, caType) || IsCustomAttributeDefined(module, module.MetadataToken, caType));
        }

        internal static bool IsDefined(ParameterInfo parameter, RuntimeType caType)
        {
            return (PseudoCustomAttribute.IsDefined(parameter, caType) || IsCustomAttributeDefined(parameter.Member.Module, parameter.MetadataToken, caType));
        }

        internal static bool IsDefined(RuntimeConstructorInfo ctor, RuntimeType caType)
        {
            return (PseudoCustomAttribute.IsDefined(ctor, caType) || IsCustomAttributeDefined(ctor.Module, ctor.MetadataToken, caType));
        }

        internal static bool IsDefined(RuntimeEventInfo e, RuntimeType caType)
        {
            return (PseudoCustomAttribute.IsDefined(e, caType) || IsCustomAttributeDefined(e.Module, e.MetadataToken, caType));
        }

        internal static bool IsDefined(RuntimeFieldInfo field, RuntimeType caType)
        {
            return (PseudoCustomAttribute.IsDefined(field, caType) || IsCustomAttributeDefined(field.Module, field.MetadataToken, caType));
        }

        internal static bool IsDefined(RuntimePropertyInfo property, RuntimeType caType)
        {
            return (PseudoCustomAttribute.IsDefined(property, caType) || IsCustomAttributeDefined(property.Module, property.MetadataToken, caType));
        }

        internal static bool IsDefined(RuntimeMethodInfo method, RuntimeType caType, bool inherit)
        {
            if (PseudoCustomAttribute.IsDefined(method, caType))
            {
                return true;
            }
            if (IsCustomAttributeDefined(method.Module, method.MetadataToken, caType))
            {
                return true;
            }
            if (inherit)
            {
                method = method.GetParentDefinition() as RuntimeMethodInfo;
                while (method != null)
                {
                    if (IsCustomAttributeDefined(method.Module, method.MetadataToken, caType, inherit))
                    {
                        return true;
                    }
                    method = method.GetParentDefinition() as RuntimeMethodInfo;
                }
            }
            return false;
        }

        internal static bool IsDefined(RuntimeType type, RuntimeType caType, bool inherit)
        {
            if (type.GetElementType() == null)
            {
                if (PseudoCustomAttribute.IsDefined(type, caType))
                {
                    return true;
                }
                if (IsCustomAttributeDefined(type.Module, type.MetadataToken, caType))
                {
                    return true;
                }
                if (!inherit)
                {
                    return false;
                }
                type = type.BaseType as RuntimeType;
                while (type != null)
                {
                    if (IsCustomAttributeDefined(type.Module, type.MetadataToken, caType, inherit))
                    {
                        return true;
                    }
                    type = type.BaseType as RuntimeType;
                }
            }
            return false;
        }

        private static void ParseAttributeUsageAttribute(ConstArray ca, out AttributeTargets targets, out bool inherited, out bool allowMultiple)
        {
            int num;
            _ParseAttributeUsageAttribute(ca.Signature, ca.Length, out num, out inherited, out allowMultiple);
            targets = (AttributeTargets) num;
        }
    }
}

