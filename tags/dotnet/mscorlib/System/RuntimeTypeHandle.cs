namespace System
{
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Threading;

    [Serializable, StructLayout(LayoutKind.Sequential), ComVisible(true)]
    public struct RuntimeTypeHandle : ISerializable
    {
        private const int MAX_CLASS_NAME = 0x400;
        internal static readonly RuntimeTypeHandle EmptyHandle;
        private IntPtr m_ptr;
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern bool IsInstanceOfType(object o);
        internal static unsafe IntPtr GetTypeHelper(IntPtr th, IntPtr pGenericArgs, int cGenericArgs, IntPtr pModifiers, int cModifiers)
        {
            Type runtimeType = new RuntimeTypeHandle(th.ToPointer()).GetRuntimeType();
            if (runtimeType == null)
            {
                return th;
            }
            if (cGenericArgs > 0)
            {
                Type[] typeArguments = new Type[cGenericArgs];
                void** voidPtr = (void**) pGenericArgs.ToPointer();
                for (int i = 0; i < typeArguments.Length; i++)
                {
                    RuntimeTypeHandle handle = new RuntimeTypeHandle((void*) Marshal.ReadIntPtr((IntPtr) voidPtr, i * sizeof(void*)));
                    typeArguments[i] = Type.GetTypeFromHandle(handle);
                    if (typeArguments[i] == null)
                    {
                        return IntPtr.Zero;
                    }
                }
                runtimeType = runtimeType.MakeGenericType(typeArguments);
            }
            if (cModifiers > 0)
            {
                int* numPtr = (int*) pModifiers.ToPointer();
                for (int j = 0; j < cModifiers; j++)
                {
                    if (((byte) Marshal.ReadInt32((IntPtr) numPtr, j * 4)) == 15)
                    {
                        runtimeType = runtimeType.MakePointerType();
                    }
                    else if (((byte) Marshal.ReadInt32((IntPtr) numPtr, j * 4)) == 0x10)
                    {
                        runtimeType = runtimeType.MakeByRefType();
                    }
                    else if (((byte) Marshal.ReadInt32((IntPtr) numPtr, j * 4)) == 0x1d)
                    {
                        runtimeType = runtimeType.MakeArrayType();
                    }
                    else
                    {
                        runtimeType = runtimeType.MakeArrayType(Marshal.ReadInt32((IntPtr) numPtr, ++j * 4));
                    }
                }
            }
            return runtimeType.GetTypeHandleInternal().Value;
        }

        public static bool operator ==(RuntimeTypeHandle left, object right)
        {
            return left.Equals(right);
        }

        public static bool operator ==(object left, RuntimeTypeHandle right)
        {
            return right.Equals(left);
        }

        public static bool operator !=(RuntimeTypeHandle left, object right)
        {
            return !left.Equals(right);
        }

        public static bool operator !=(object left, RuntimeTypeHandle right)
        {
            return !right.Equals(left);
        }

        public override int GetHashCode()
        {
            return ValueType.GetHashCodeOfPtr(this.m_ptr);
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public override bool Equals(object obj)
        {
            if (!(obj is RuntimeTypeHandle))
            {
                return false;
            }
            RuntimeTypeHandle handle = (RuntimeTypeHandle) obj;
            return (handle.m_ptr == this.m_ptr);
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public bool Equals(RuntimeTypeHandle handle)
        {
            return (handle.m_ptr == this.m_ptr);
        }

        public IntPtr Value
        {
            get
            {
                return this.m_ptr;
            }
        }
        internal unsafe RuntimeTypeHandle(void* rth)
        {
            this.m_ptr = new IntPtr(rth);
        }

        internal bool IsNullHandle()
        {
            return (this.m_ptr.ToPointer() == null);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern object CreateInstance(RuntimeType type, bool publicOnly, bool noCheck, ref bool canBeCached, ref RuntimeMethodHandle ctor, ref bool bNeedSecurityCheck);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern object CreateCaInstance(RuntimeMethodHandle ctor);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern object Allocate();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern object CreateInstanceForAnotherGenericParameter(Type genericParameter);
        internal RuntimeType GetRuntimeType()
        {
            if (!this.IsNullHandle())
            {
                return (RuntimeType) Type.GetTypeFromHandle(this);
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern CorElementType GetCorElementType();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern unsafe void* _GetAssemblyHandle();
        internal AssemblyHandle GetAssemblyHandle()
        {
            return new AssemblyHandle(this._GetAssemblyHandle());
        }

        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        private extern unsafe void* _GetModuleHandle();
        [CLSCompliant(false), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public ModuleHandle GetModuleHandle()
        {
            return new ModuleHandle(this._GetModuleHandle());
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern unsafe void* _GetBaseTypeHandle();
        internal RuntimeTypeHandle GetBaseTypeHandle()
        {
            return new RuntimeTypeHandle(this._GetBaseTypeHandle());
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern TypeAttributes GetAttributes();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern unsafe void* _GetElementType();
        internal RuntimeTypeHandle GetElementType()
        {
            return new RuntimeTypeHandle(this._GetElementType());
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern RuntimeTypeHandle GetCanonicalHandle();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern int GetArrayRank();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern int GetToken();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern unsafe void* _GetMethodAt(int slot);
        internal RuntimeMethodHandle GetMethodAt(int slot)
        {
            return new RuntimeMethodHandle(this._GetMethodAt(slot));
        }

        internal IntroducedMethodEnumerator IntroducedMethods
        {
            get
            {
                return new IntroducedMethodEnumerator(this);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern RuntimeMethodHandle GetFirstIntroducedMethod(RuntimeTypeHandle type);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern RuntimeMethodHandle GetNextIntroducedMethod(RuntimeMethodHandle method);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern unsafe bool GetFields(int** result, int* count);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern RuntimeTypeHandle[] GetInterfaces();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern RuntimeTypeHandle[] GetConstraints();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern IntPtr GetGCHandle(GCHandleType type);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void FreeGCHandle(IntPtr handle);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern unsafe void* _GetMethodFromToken(int tkMethodDef);
        internal RuntimeMethodHandle GetMethodFromToken(int tkMethodDef)
        {
            return new RuntimeMethodHandle(this._GetMethodFromToken(tkMethodDef));
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern int GetNumVirtuals();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern int GetInterfaceMethodSlots();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int GetFirstSlotForInterface(IntPtr interfaceHandle);
        internal int GetFirstSlotForInterface(RuntimeTypeHandle interfaceHandle)
        {
            return this.GetFirstSlotForInterface(interfaceHandle.Value);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int GetInterfaceMethodImplementationSlot(IntPtr interfaceHandle, IntPtr interfaceMethodHandle);
        internal int GetInterfaceMethodImplementationSlot(RuntimeTypeHandle interfaceHandle, RuntimeMethodHandle interfaceMethodHandle)
        {
            return this.GetInterfaceMethodImplementationSlot(interfaceHandle.Value, interfaceMethodHandle.Value);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern bool IsComObject(bool isGenericCOM);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern bool IsContextful();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern bool IsInterface();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern bool IsVisible();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern bool _IsVisibleFromModule(IntPtr module);
        internal unsafe bool IsVisibleFromModule(ModuleHandle module)
        {
            return this._IsVisibleFromModule((IntPtr) module.Value);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern bool HasProxyAttribute();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern string ConstructName(bool nameSpace, bool fullInst, bool assembly);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern unsafe void* _GetUtf8Name();
        internal Utf8String GetUtf8Name()
        {
            return new Utf8String(this._GetUtf8Name());
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern bool CanCastTo(IntPtr target);
        internal bool CanCastTo(RuntimeTypeHandle target)
        {
            return this.CanCastTo(target.Value);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern RuntimeTypeHandle GetDeclaringType();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern unsafe void* _GetDeclaringMethod();
        internal RuntimeMethodHandle GetDeclaringMethod()
        {
            return new RuntimeMethodHandle(this._GetDeclaringMethod());
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern unsafe void* _GetDefaultConstructor();
        internal RuntimeMethodHandle GetDefaultConstructor()
        {
            return new RuntimeMethodHandle(this._GetDefaultConstructor());
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern bool SupportsInterface(object target);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern unsafe void* _GetTypeByName(string name, bool throwOnError, bool ignoreCase, bool reflectionOnly, ref StackCrawlMark stackMark, bool loadTypeFromPartialName);
        internal static RuntimeTypeHandle GetTypeByName(string name, bool throwOnError, bool ignoreCase, bool reflectionOnly, ref StackCrawlMark stackMark)
        {
            if ((name != null) && (name.Length != 0))
            {
                return new RuntimeTypeHandle(_GetTypeByName(name, throwOnError, ignoreCase, reflectionOnly, ref stackMark, false));
            }
            if (throwOnError)
            {
                throw new TypeLoadException(Environment.GetResourceString("Arg_TypeLoadNullStr"));
            }
            return new RuntimeTypeHandle();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern unsafe void* _GetTypeByNameUsingCARules(string name, IntPtr scope);
        internal static unsafe Type GetTypeByNameUsingCARules(string name, Module scope)
        {
            if ((name == null) || (name.Length == 0))
            {
                throw new ArgumentException();
            }
            RuntimeTypeHandle handle2 = new RuntimeTypeHandle(_GetTypeByNameUsingCARules(name, (IntPtr) scope.GetModuleHandle().Value));
            return handle2.GetRuntimeType();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern RuntimeTypeHandle[] GetInstantiation();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern unsafe void* _Instantiate(RuntimeTypeHandle[] inst);
        internal RuntimeTypeHandle Instantiate(RuntimeTypeHandle[] inst)
        {
            return new RuntimeTypeHandle(this._Instantiate(inst));
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern unsafe void* _MakeArray(int rank);
        internal RuntimeTypeHandle MakeArray(int rank)
        {
            return new RuntimeTypeHandle(this._MakeArray(rank));
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern unsafe void* _MakeSZArray();
        internal RuntimeTypeHandle MakeSZArray()
        {
            return new RuntimeTypeHandle(this._MakeSZArray());
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern unsafe void* _MakeByRef();
        internal RuntimeTypeHandle MakeByRef()
        {
            return new RuntimeTypeHandle(this._MakeByRef());
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern unsafe void* _MakePointer();
        internal RuntimeTypeHandle MakePointer()
        {
            return new RuntimeTypeHandle(this._MakePointer());
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern bool HasInstantiation();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern unsafe void* _GetGenericTypeDefinition();
        internal RuntimeTypeHandle GetGenericTypeDefinition()
        {
            return new RuntimeTypeHandle(this._GetGenericTypeDefinition());
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern bool IsGenericTypeDefinition();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern bool IsGenericVariable();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern int GetGenericVariableIndex();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern bool ContainsGenericVariables();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern bool SatisfiesConstraints(RuntimeTypeHandle[] typeContext, RuntimeTypeHandle[] methodContext, RuntimeTypeHandle toType);
        private RuntimeTypeHandle(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            Type type = (RuntimeType) info.GetValue("TypeObj", typeof(RuntimeType));
            this.m_ptr = type.TypeHandle.Value;
            if (this.m_ptr.ToPointer() == null)
            {
                throw new SerializationException(Environment.GetResourceString("Serialization_InsufficientState"));
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            if (this.m_ptr.ToPointer() == null)
            {
                throw new SerializationException(Environment.GetResourceString("Serialization_InvalidFieldState"));
            }
            RuntimeType typeFromHandle = (RuntimeType) Type.GetTypeFromHandle(this);
            info.AddValue("TypeObj", typeFromHandle, typeof(RuntimeType));
        }

        static RuntimeTypeHandle()
        {
            EmptyHandle = new RuntimeTypeHandle(null);
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct IntroducedMethodEnumerator
        {
            private RuntimeMethodHandle _method;
            private bool _firstCall;
            internal IntroducedMethodEnumerator(RuntimeTypeHandle type)
            {
                this._method = RuntimeTypeHandle.GetFirstIntroducedMethod(type);
                this._firstCall = true;
            }

            public bool MoveNext()
            {
                if (this._firstCall)
                {
                    this._firstCall = false;
                }
                else if (!this._method.IsNullHandle())
                {
                    this._method = RuntimeTypeHandle.GetNextIntroducedMethod(this._method);
                }
                return !this._method.IsNullHandle();
            }

            public RuntimeMethodHandle Current
            {
                get
                {
                    return this._method;
                }
            }
            public RuntimeTypeHandle.IntroducedMethodEnumerator GetEnumerator()
            {
                return this;
            }
        }
    }
}

