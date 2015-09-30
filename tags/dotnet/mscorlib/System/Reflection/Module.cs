namespace System.Reflection
{
    using System;
    using System.Collections;
    using System.Diagnostics.SymbolStore;
    using System.Globalization;
    using System.IO;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Permissions;
    using System.Threading;

    [Serializable, ClassInterface(ClassInterfaceType.None), ComVisible(true), ComDefaultInterface(typeof(_Module))]
    public class Module : _Module, ISerializable, ICustomAttributeProvider
    {
        private const BindingFlags DefaultLookup = (BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        public static readonly TypeFilter FilterTypeName;
        public static readonly TypeFilter FilterTypeNameIgnoreCase;
        internal MethodToken m__EntryPoint;
        internal ISymbolWriter m__iSymWriter;
        internal ModuleBuilderData m__moduleData;
        internal IntPtr m__pData;
        private IntPtr m__pFields;
        private IntPtr m__pGlobals;
        internal IntPtr m__pInternalSymWriter;
        private IntPtr m__pRefClass;
        private System.RuntimeType m__runtimeType;
        internal ArrayList m__TypeBuilderList;

        static Module()
        {
            System.Reflection.__Filters filters = new System.Reflection.__Filters();
            FilterTypeName = new TypeFilter(filters.FilterTypeName);
            FilterTypeNameIgnoreCase = new TypeFilter(filters.FilterTypeNameIgnoreCase);
        }

        internal Module()
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern System.Reflection.Assembly _GetAssemblyInternal();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern IntPtr _GetHINSTANCE();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern X509Certificate _GetSignerCertificateInternal();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern Type _GetTypeInternal(string className, bool ignoreCase, bool throwOnError);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern Type[] _GetTypesInternal(ref StackCrawlMark stackMark);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void _InternalAddResource(string strName, byte[] resBytes, int resByteCount, int tkFile, int attribute, int portableExecutableKind, int imageFileMachine);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void _InternalDefineNativeResourceBytes(byte[] resource, int portableExecutableKind, int imageFileMachine);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void _InternalDefineNativeResourceFile(string strFilename, int portableExecutableKind, int ImageFileMachine);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern string _InternalGetFullyQualifiedName();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int _InternalGetMemberRef(Module refedModule, int tr, int defToken);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int _InternalGetMemberRefFromSignature(int tr, string methodName, byte[] signature, int length);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int _InternalGetMemberRefOfFieldInfo(int tkType, IntPtr interfaceHandle, int tkField);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int _InternalGetMemberRefOfMethodInfo(int tr, IntPtr method);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern string _InternalGetName();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int _InternalGetStringConstant(string str);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int _InternalGetTypeSpecTokenWithBytes(byte[] signature, int length);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int _InternalGetTypeToken(string strFullName, Module refedModule, string strRefedModuleFileName, int tkResolution);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern Type _InternalLoadInMemoryTypeByName(string className);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void _InternalPreSavePEFile(int portableExecutableKind, int imageFileMachine);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void _InternalSavePEFile(string fileName, int entryPoint, int isExe, bool isManifestFile);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void _InternalSetFieldRVAContent(int fdToken, byte[] data, int length);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void _InternalSetModuleProps(string strModuleName);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void _InternalSetResourceCounts(int resCount);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern bool _IsResourceInternal();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int _nativeGetArrayMethodToken(int tkTypeSpec, string methodName, byte[] signature, int sigLength, int baseToken);
        private static RuntimeTypeHandle[] ConvertToTypeHandleArray(Type[] genericArguments)
        {
            if (genericArguments == null)
            {
                return null;
            }
            int length = genericArguments.Length;
            RuntimeTypeHandle[] handleArray = new RuntimeTypeHandle[length];
            for (int i = 0; i < length; i++)
            {
                Type underlyingSystemType = genericArguments[i];
                if (underlyingSystemType == null)
                {
                    throw new ArgumentException(Environment.GetResourceString("Argument_InvalidGenericInstArray"));
                }
                underlyingSystemType = underlyingSystemType.UnderlyingSystemType;
                if (underlyingSystemType == null)
                {
                    throw new ArgumentException(Environment.GetResourceString("Argument_InvalidGenericInstArray"));
                }
                if (!(underlyingSystemType is System.RuntimeType))
                {
                    throw new ArgumentException(Environment.GetResourceString("Argument_InvalidGenericInstArray"));
                }
                handleArray[i] = underlyingSystemType.GetTypeHandleInternal();
            }
            return handleArray;
        }

        public override bool Equals(object o)
        {
            if (o == null)
            {
                return false;
            }
            if (!(o is Module))
            {
                return false;
            }
            Module internalModule = o as Module;
            internalModule = internalModule.InternalModule;
            return (this.InternalModule == internalModule);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public virtual Type[] FindTypes(TypeFilter filter, object filterCriteria)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            Type[] typesInternal = this.GetTypesInternal(ref lookForMyCaller);
            int num = 0;
            for (int i = 0; i < typesInternal.Length; i++)
            {
                if ((filter != null) && !filter(typesInternal[i], filterCriteria))
                {
                    typesInternal[i] = null;
                }
                else
                {
                    num++;
                }
            }
            if (num == typesInternal.Length)
            {
                return typesInternal;
            }
            Type[] typeArray2 = new Type[num];
            num = 0;
            for (int j = 0; j < typesInternal.Length; j++)
            {
                if (typesInternal[j] != null)
                {
                    typeArray2[num++] = typesInternal[j];
                }
            }
            return typeArray2;
        }

        internal virtual System.Reflection.Assembly GetAssemblyInternal()
        {
            return this.InternalModule._GetAssemblyInternal();
        }

        public virtual object[] GetCustomAttributes(bool inherit)
        {
            return CustomAttribute.GetCustomAttributes(this, typeof(object) as System.RuntimeType);
        }

        public virtual object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            if (attributeType == null)
            {
                throw new ArgumentNullException("attributeType");
            }
            System.RuntimeType underlyingSystemType = attributeType.UnderlyingSystemType as System.RuntimeType;
            if (underlyingSystemType == null)
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_MustBeType"), "attributeType");
            }
            return CustomAttribute.GetCustomAttributes(this, underlyingSystemType);
        }

        public FieldInfo GetField(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            return this.GetField(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        }

        public FieldInfo GetField(string name, BindingFlags bindingAttr)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            return this.InternalGetField(name, bindingAttr);
        }

        public FieldInfo[] GetFields()
        {
            if (this.RuntimeType == null)
            {
                return new FieldInfo[0];
            }
            return this.RuntimeType.GetFields();
        }

        public FieldInfo[] GetFields(BindingFlags bindingFlags)
        {
            if (this.RuntimeType == null)
            {
                return new FieldInfo[0];
            }
            return this.RuntimeType.GetFields(bindingFlags);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        internal IntPtr GetHINSTANCE()
        {
            return this.InternalModule._GetHINSTANCE();
        }

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

        protected virtual MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            if (this.RuntimeType == null)
            {
                return null;
            }
            if (types == null)
            {
                return this.RuntimeType.GetMethod(name, bindingAttr);
            }
            return this.RuntimeType.GetMethod(name, bindingAttr, binder, callConvention, types, modifiers);
        }

        public MethodInfo[] GetMethods()
        {
            if (this.RuntimeType == null)
            {
                return new MethodInfo[0];
            }
            return this.RuntimeType.GetMethods();
        }

        public MethodInfo[] GetMethods(BindingFlags bindingFlags)
        {
            if (this.RuntimeType == null)
            {
                return new MethodInfo[0];
            }
            return this.RuntimeType.GetMethods(bindingFlags);
        }

        internal unsafe System.ModuleHandle GetModuleHandle()
        {
            return new System.ModuleHandle((void*) this.m_pData);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            UnitySerializationHolder.GetUnitySerializationInfo(info, 5, this.ScopeName, this.GetAssemblyInternal());
        }

        public void GetPEKind(out PortableExecutableKinds peKind, out ImageFileMachine machine)
        {
            this.GetModuleHandle().GetPEKind(out peKind, out machine);
        }

        public X509Certificate GetSignerCertificate()
        {
            return this.GetSignerCertificateInternal();
        }

        internal X509Certificate GetSignerCertificateInternal()
        {
            return this.InternalModule._GetSignerCertificateInternal();
        }

        [ComVisible(true)]
        public virtual Type GetType(string className)
        {
            return this.GetType(className, false, false);
        }

        [ComVisible(true)]
        public virtual Type GetType(string className, bool ignoreCase)
        {
            return this.GetType(className, false, ignoreCase);
        }

        [ComVisible(true)]
        public virtual Type GetType(string className, bool throwOnError, bool ignoreCase)
        {
            return this.GetTypeInternal(className, throwOnError, ignoreCase);
        }

        internal Type GetTypeInternal(string className, bool ignoreCase, bool throwOnError)
        {
            return this.InternalModule._GetTypeInternal(className, ignoreCase, throwOnError);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public virtual Type[] GetTypes()
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return this.GetTypesInternal(ref lookForMyCaller);
        }

        internal Type[] GetTypesInternal(ref StackCrawlMark stackMark)
        {
            return this.InternalModule._GetTypesInternal(ref stackMark);
        }

        internal void InternalAddResource(string strName, byte[] resBytes, int resByteCount, int tkFile, int attribute, int portableExecutableKind, int imageFileMachine)
        {
            this.InternalModule._InternalAddResource(strName, resBytes, resByteCount, tkFile, attribute, portableExecutableKind, imageFileMachine);
        }

        internal void InternalDefineNativeResourceBytes(byte[] resource, int portableExecutableKind, int imageFileMachine)
        {
            this.InternalModule._InternalDefineNativeResourceBytes(resource, portableExecutableKind, imageFileMachine);
        }

        internal void InternalDefineNativeResourceFile(string strFilename, int portableExecutableKind, int ImageFileMachine)
        {
            this.InternalModule._InternalDefineNativeResourceFile(strFilename, portableExecutableKind, ImageFileMachine);
        }

        private FieldInfo InternalGetField(string name, BindingFlags bindingAttr)
        {
            if (this.RuntimeType == null)
            {
                return null;
            }
            return this.RuntimeType.GetField(name, bindingAttr);
        }

        internal string InternalGetFullyQualifiedName()
        {
            return this.InternalModule._InternalGetFullyQualifiedName();
        }

        internal int InternalGetMemberRef(Module refedModule, int tr, int defToken)
        {
            return this.InternalModule._InternalGetMemberRef(refedModule.InternalModule, tr, defToken);
        }

        internal int InternalGetMemberRefFromSignature(int tr, string methodName, byte[] signature, int length)
        {
            return this.InternalModule._InternalGetMemberRefFromSignature(tr, methodName, signature, length);
        }

        internal int InternalGetMemberRefOfFieldInfo(int tkType, RuntimeTypeHandle declaringType, int tkField)
        {
            return this.InternalModule._InternalGetMemberRefOfFieldInfo(tkType, declaringType.Value, tkField);
        }

        internal int InternalGetMemberRefOfMethodInfo(int tr, RuntimeMethodHandle method)
        {
            return this.InternalModule._InternalGetMemberRefOfMethodInfo(tr, method.Value);
        }

        private string InternalGetName()
        {
            return this.InternalModule._InternalGetName();
        }

        internal int InternalGetStringConstant(string str)
        {
            return this.InternalModule._InternalGetStringConstant(str);
        }

        internal int InternalGetTypeSpecTokenWithBytes(byte[] signature, int length)
        {
            return this.InternalModule._InternalGetTypeSpecTokenWithBytes(signature, length);
        }

        internal int InternalGetTypeToken(string strFullName, Module refedModule, string strRefedModuleFileName, int tkResolution)
        {
            return this.InternalModule._InternalGetTypeToken(strFullName, refedModule.InternalModule, strRefedModuleFileName, tkResolution);
        }

        internal Type InternalLoadInMemoryTypeByName(string className)
        {
            return this.InternalModule._InternalLoadInMemoryTypeByName(className);
        }

        internal void InternalPreSavePEFile(int portableExecutableKind, int imageFileMachine)
        {
            this.InternalModule._InternalPreSavePEFile(portableExecutableKind, imageFileMachine);
        }

        internal void InternalSavePEFile(string fileName, MethodToken entryPoint, int isExe, bool isManifestFile)
        {
            this.InternalModule._InternalSavePEFile(fileName, entryPoint.Token, isExe, isManifestFile);
        }

        internal void InternalSetFieldRVAContent(int fdToken, byte[] data, int length)
        {
            this.InternalModule._InternalSetFieldRVAContent(fdToken, data, length);
        }

        internal void InternalSetModuleProps(string strModuleName)
        {
            this.InternalModule._InternalSetModuleProps(strModuleName);
        }

        internal void InternalSetResourceCounts(int resCount)
        {
            this.InternalModule._InternalSetResourceCounts(resCount);
        }

        public virtual bool IsDefined(Type attributeType, bool inherit)
        {
            if (attributeType == null)
            {
                throw new ArgumentNullException("attributeType");
            }
            System.RuntimeType underlyingSystemType = attributeType.UnderlyingSystemType as System.RuntimeType;
            if (underlyingSystemType == null)
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_MustBeType"), "caType");
            }
            return CustomAttribute.IsDefined(this, underlyingSystemType);
        }

        internal virtual bool IsDynamic()
        {
            return false;
        }

        public bool IsResource()
        {
            return this.IsResourceInternal();
        }

        internal bool IsResourceInternal()
        {
            return this.InternalModule._IsResourceInternal();
        }

        internal int nativeGetArrayMethodToken(int tkTypeSpec, string methodName, byte[] signature, int sigLength, int baseToken)
        {
            return this.InternalModule._nativeGetArrayMethodToken(tkTypeSpec, methodName, signature, sigLength, baseToken);
        }

        public FieldInfo ResolveField(int metadataToken)
        {
            return this.ResolveField(metadataToken, null, null);
        }

        public unsafe FieldInfo ResolveField(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            FieldInfo fieldInfo;
            System.Reflection.MetadataToken token = new System.Reflection.MetadataToken(metadataToken);
            if (!this.MetadataImport.IsValidToken((int) token))
            {
                throw new ArgumentOutOfRangeException("metadataToken", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_InvalidToken", new object[] { token, this }), new object[0]));
            }
            RuntimeTypeHandle[] typeInstantiationContext = ConvertToTypeHandleArray(genericTypeArguments);
            RuntimeTypeHandle[] methodInstantiationContext = ConvertToTypeHandleArray(genericMethodArguments);
            try
            {
                RuntimeFieldHandle fieldHandle = new RuntimeFieldHandle();
                if (!token.IsFieldDef)
                {
                    if (!token.IsMemberRef)
                    {
                        throw new ArgumentException("metadataToken", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_ResolveField"), new object[] { token, this }));
                    }
                    if (*(((byte*) this.MetadataImport.GetMemberRefProps((int) token).Signature.ToPointer())) != 6)
                    {
                        throw new ArgumentException("metadataToken", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_ResolveField"), new object[] { token, this }));
                    }
                    fieldHandle = this.GetModuleHandle().ResolveFieldHandle((int) token, typeInstantiationContext, methodInstantiationContext);
                }
                fieldHandle = this.GetModuleHandle().ResolveFieldHandle(metadataToken, typeInstantiationContext, methodInstantiationContext);
                Type runtimeType = fieldHandle.GetApproxDeclaringType().GetRuntimeType();
                if (runtimeType.IsGenericType || runtimeType.IsArray)
                {
                    int parentToken = this.GetModuleHandle().GetMetadataImport().GetParentToken(metadataToken);
                    runtimeType = this.ResolveType(parentToken, genericTypeArguments, genericMethodArguments);
                }
                fieldInfo = System.RuntimeType.GetFieldInfo(runtimeType.GetTypeHandleInternal(), fieldHandle);
            }
            catch (MissingFieldException)
            {
                fieldInfo = this.ResolveLiteralField((int) token, genericTypeArguments, genericMethodArguments);
            }
            catch (BadImageFormatException exception)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_BadImageFormatExceptionResolve"), exception);
            }
            return fieldInfo;
        }

        internal FieldInfo ResolveLiteralField(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            FieldInfo field;
            System.Reflection.MetadataToken token = new System.Reflection.MetadataToken(metadataToken);
            if (!this.MetadataImport.IsValidToken((int) token))
            {
                throw new ArgumentOutOfRangeException("metadataToken", string.Format(CultureInfo.CurrentUICulture, Environment.GetResourceString("Argument_InvalidToken", new object[] { token, this }), new object[0]));
            }
            string name = this.MetadataImport.GetName((int) token).ToString();
            int parentToken = this.MetadataImport.GetParentToken((int) token);
            Type type = this.ResolveType(parentToken, genericTypeArguments, genericMethodArguments);
            type.GetFields();
            try
            {
                field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            }
            catch
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_ResolveField"), new object[] { token, this }), "metadataToken");
            }
            return field;
        }

        public MemberInfo ResolveMember(int metadataToken)
        {
            return this.ResolveMember(metadataToken, null, null);
        }

        public unsafe MemberInfo ResolveMember(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            System.Reflection.MetadataToken token = new System.Reflection.MetadataToken(metadataToken);
            if (token.IsProperty)
            {
                throw new ArgumentException(Environment.GetResourceString("InvalidOperation_PropertyInfoNotAvailable"));
            }
            if (token.IsEvent)
            {
                throw new ArgumentException(Environment.GetResourceString("InvalidOperation_EventInfoNotAvailable"));
            }
            if (token.IsMethodSpec || token.IsMethodDef)
            {
                return this.ResolveMethod(metadataToken, genericTypeArguments, genericMethodArguments);
            }
            if (token.IsFieldDef)
            {
                return this.ResolveField(metadataToken, genericTypeArguments, genericMethodArguments);
            }
            if ((token.IsTypeRef || token.IsTypeDef) || token.IsTypeSpec)
            {
                return this.ResolveType(metadataToken, genericTypeArguments, genericMethodArguments);
            }
            if (!token.IsMemberRef)
            {
                throw new ArgumentException("metadataToken", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_ResolveMember", new object[] { token, this }), new object[0]));
            }
            if (!this.MetadataImport.IsValidToken((int) token))
            {
                throw new ArgumentOutOfRangeException("metadataToken", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_InvalidToken", new object[] { token, this }), new object[0]));
            }
            if (*(((byte*) this.MetadataImport.GetMemberRefProps((int) token).Signature.ToPointer())) != 6)
            {
                return this.ResolveMethod((int) token, genericTypeArguments, genericMethodArguments);
            }
            return this.ResolveField((int) token, genericTypeArguments, genericMethodArguments);
        }

        public MethodBase ResolveMethod(int metadataToken)
        {
            return this.ResolveMethod(metadataToken, null, null);
        }

        public unsafe MethodBase ResolveMethod(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            MethodBase methodBase;
            System.Reflection.MetadataToken token = new System.Reflection.MetadataToken(metadataToken);
            if (!this.MetadataImport.IsValidToken((int) token))
            {
                throw new ArgumentOutOfRangeException("metadataToken", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_InvalidToken", new object[] { token, this }), new object[0]));
            }
            RuntimeTypeHandle[] typeInstantiationContext = ConvertToTypeHandleArray(genericTypeArguments);
            RuntimeTypeHandle[] methodInstantiationContext = ConvertToTypeHandleArray(genericMethodArguments);
            try
            {
                if (!token.IsMethodDef && !token.IsMethodSpec)
                {
                    if (!token.IsMemberRef)
                    {
                        throw new ArgumentException("metadataToken", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_ResolveMethod", new object[] { token, this }), new object[0]));
                    }
                    if (*(((byte*) this.MetadataImport.GetMemberRefProps((int) token).Signature.ToPointer())) == 6)
                    {
                        throw new ArgumentException("metadataToken", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_ResolveMethod"), new object[] { token, this }));
                    }
                }
                RuntimeMethodHandle methodHandle = this.GetModuleHandle().ResolveMethodHandle((int) token, typeInstantiationContext, methodInstantiationContext);
                Type runtimeType = methodHandle.GetDeclaringType().GetRuntimeType();
                if (runtimeType.IsGenericType || runtimeType.IsArray)
                {
                    System.Reflection.MetadataToken token2 = new System.Reflection.MetadataToken(this.MetadataImport.GetParentToken((int) token));
                    if (token.IsMethodSpec)
                    {
                        token2 = new System.Reflection.MetadataToken(this.MetadataImport.GetParentToken((int) token2));
                    }
                    runtimeType = this.ResolveType((int) token2, genericTypeArguments, genericMethodArguments);
                }
                methodBase = System.RuntimeType.GetMethodBase(runtimeType.GetTypeHandleInternal(), methodHandle);
            }
            catch (BadImageFormatException exception)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_BadImageFormatExceptionResolve"), exception);
            }
            return methodBase;
        }

        public byte[] ResolveSignature(int metadataToken)
        {
            ConstArray memberRefProps;
            System.Reflection.MetadataToken token = new System.Reflection.MetadataToken(metadataToken);
            if (!this.MetadataImport.IsValidToken((int) token))
            {
                throw new ArgumentOutOfRangeException("metadataToken", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_InvalidToken", new object[] { token, this }), new object[0]));
            }
            if (((!token.IsMemberRef && !token.IsMethodDef) && (!token.IsTypeSpec && !token.IsSignature)) && !token.IsFieldDef)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_InvalidToken", new object[] { token, this }), new object[0]), "metadataToken");
            }
            if (token.IsMemberRef)
            {
                memberRefProps = this.MetadataImport.GetMemberRefProps(metadataToken);
            }
            else
            {
                memberRefProps = this.MetadataImport.GetSignatureFromToken(metadataToken);
            }
            byte[] buffer = new byte[memberRefProps.Length];
            for (int i = 0; i < memberRefProps.Length; i++)
            {
                buffer[i] = memberRefProps[i];
            }
            return buffer;
        }

        public string ResolveString(int metadataToken)
        {
            System.Reflection.MetadataToken token = new System.Reflection.MetadataToken(metadataToken);
            if (!token.IsString)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentUICulture, Environment.GetResourceString("Argument_ResolveString"), new object[] { metadataToken, this.ToString() }));
            }
            if (!this.MetadataImport.IsValidToken((int) token))
            {
                throw new ArgumentOutOfRangeException("metadataToken", string.Format(CultureInfo.CurrentUICulture, Environment.GetResourceString("Argument_InvalidToken", new object[] { token, this }), new object[0]));
            }
            string userString = this.MetadataImport.GetUserString(metadataToken);
            if (userString == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentUICulture, Environment.GetResourceString("Argument_ResolveString"), new object[] { metadataToken, this.ToString() }));
            }
            return userString;
        }

        public Type ResolveType(int metadataToken)
        {
            return this.ResolveType(metadataToken, null, null);
        }

        public Type ResolveType(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            Type type2;
            System.Reflection.MetadataToken token = new System.Reflection.MetadataToken(metadataToken);
            if (token.IsGlobalTypeDefToken)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_ResolveModuleType"), new object[] { token }), "metadataToken");
            }
            if (!this.MetadataImport.IsValidToken((int) token))
            {
                throw new ArgumentOutOfRangeException("metadataToken", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_InvalidToken", new object[] { token, this }), new object[0]));
            }
            if ((!token.IsTypeDef && !token.IsTypeSpec) && !token.IsTypeRef)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_ResolveType"), new object[] { token, this }), "metadataToken");
            }
            RuntimeTypeHandle[] typeInstantiationContext = ConvertToTypeHandleArray(genericTypeArguments);
            RuntimeTypeHandle[] methodInstantiationContext = ConvertToTypeHandleArray(genericMethodArguments);
            try
            {
                Type runtimeType = this.GetModuleHandle().ResolveTypeHandle(metadataToken, typeInstantiationContext, methodInstantiationContext).GetRuntimeType();
                if (runtimeType == null)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_ResolveType"), new object[] { token, this }), "metadataToken");
                }
                type2 = runtimeType;
            }
            catch (BadImageFormatException exception)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_BadImageFormatExceptionResolve"), exception);
            }
            return type2;
        }

        void _Module.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
        {
            throw new NotImplementedException();
        }

        void _Module.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
        {
            throw new NotImplementedException();
        }

        void _Module.GetTypeInfoCount(out uint pcTInfo)
        {
            throw new NotImplementedException();
        }

        void _Module.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return this.ScopeName;
        }

        public System.Reflection.Assembly Assembly
        {
            get
            {
                return this.GetAssemblyInternal();
            }
        }

        public virtual string FullyQualifiedName
        {
            get
            {
                string fullyQualifiedName = this.InternalGetFullyQualifiedName();
                if (fullyQualifiedName != null)
                {
                    bool flag = true;
                    try
                    {
                        Path.GetFullPathInternal(fullyQualifiedName);
                    }
                    catch (ArgumentException)
                    {
                        flag = false;
                    }
                    if (flag)
                    {
                        new FileIOPermission(FileIOPermissionAccess.PathDiscovery, fullyQualifiedName).Demand();
                    }
                }
                return fullyQualifiedName;
            }
        }

        internal virtual Module InternalModule
        {
            get
            {
                return this;
            }
        }

        internal MethodToken m_EntryPoint
        {
            get
            {
                return this.InternalModule.m__EntryPoint;
            }
            set
            {
                this.InternalModule.m__EntryPoint = value;
            }
        }

        internal ISymbolWriter m_iSymWriter
        {
            get
            {
                return this.InternalModule.m__iSymWriter;
            }
            set
            {
                this.InternalModule.m__iSymWriter = value;
            }
        }

        internal ModuleBuilderData m_moduleData
        {
            get
            {
                return this.InternalModule.m__moduleData;
            }
            set
            {
                this.InternalModule.m__moduleData = value;
            }
        }

        internal IntPtr m_pData
        {
            get
            {
                return this.InternalModule.m__pData;
            }
        }

        private IntPtr m_pFields
        {
            get
            {
                return this.InternalModule.m__pFields;
            }
        }

        private IntPtr m_pGlobals
        {
            get
            {
                return this.InternalModule.m__pGlobals;
            }
        }

        internal IntPtr m_pInternalSymWriter
        {
            get
            {
                return this.InternalModule.m__pInternalSymWriter;
            }
        }

        private IntPtr m_pRefClass
        {
            get
            {
                return this.InternalModule.m__pRefClass;
            }
        }

        private System.RuntimeType m_runtimeType
        {
            get
            {
                return this.InternalModule.m__runtimeType;
            }
            set
            {
                this.InternalModule.m__runtimeType = value;
            }
        }

        internal ArrayList m_TypeBuilderList
        {
            get
            {
                return this.InternalModule.m__TypeBuilderList;
            }
            set
            {
                this.InternalModule.m__TypeBuilderList = value;
            }
        }

        public int MDStreamVersion
        {
            get
            {
                return this.GetModuleHandle().MDStreamVersion;
            }
        }

        internal System.Reflection.MetadataImport MetadataImport
        {
            get
            {
                return this.ModuleHandle.GetMetadataImport();
            }
        }

        public int MetadataToken
        {
            get
            {
                return this.GetModuleHandle().GetToken();
            }
        }

        public System.ModuleHandle ModuleHandle
        {
            get
            {
                return new System.ModuleHandle((void*) this.m_pData);
            }
        }

        public Guid ModuleVersionId
        {
            get
            {
                Guid guid;
                this.MetadataImport.GetScopeProps(out guid);
                return guid;
            }
        }

        public string Name
        {
            get
            {
                string fullyQualifiedName = this.InternalGetFullyQualifiedName();
                int num = fullyQualifiedName.LastIndexOf('\\');
                if (num == -1)
                {
                    return fullyQualifiedName;
                }
                return new string(fullyQualifiedName.ToCharArray(), num + 1, (fullyQualifiedName.Length - num) - 1);
            }
        }

        internal System.RuntimeType RuntimeType
        {
            get
            {
                if (this.m_runtimeType == null)
                {
                    this.m_runtimeType = this.GetModuleHandle().GetModuleTypeHandle().GetRuntimeType();
                }
                return this.m_runtimeType;
            }
        }

        public string ScopeName
        {
            get
            {
                return this.InternalGetName();
            }
        }
    }
}

