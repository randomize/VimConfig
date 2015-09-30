namespace System
{
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Threading;

    [StructLayout(LayoutKind.Sequential), ComVisible(true)]
    public struct ModuleHandle
    {
        public static readonly ModuleHandle EmptyHandle;
        private IntPtr m_ptr;
        internal unsafe ModuleHandle(void* pModule)
        {
            this.m_ptr = new IntPtr(pModule);
        }

        internal void* Value
        {
            get
            {
                return this.m_ptr.ToPointer();
            }
        }
        internal bool IsNullHandle()
        {
            return (this.m_ptr.ToPointer() == null);
        }

        public override int GetHashCode()
        {
            return ValueType.GetHashCodeOfPtr(this.m_ptr);
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public override bool Equals(object obj)
        {
            if (!(obj is ModuleHandle))
            {
                return false;
            }
            ModuleHandle handle = (ModuleHandle) obj;
            return (handle.m_ptr == this.m_ptr);
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public bool Equals(ModuleHandle handle)
        {
            return (handle.m_ptr == this.m_ptr);
        }

        public static bool operator ==(ModuleHandle left, ModuleHandle right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ModuleHandle left, ModuleHandle right)
        {
            return !left.Equals(right);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern RuntimeTypeHandle GetCallerType(ref StackCrawlMark stackMark);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern unsafe void* GetDynamicMethod(void* module, string name, byte[] sig, Resolver resolver);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern int GetToken();
        internal static RuntimeTypeHandle[] CopyRuntimeTypeHandles(RuntimeTypeHandle[] inHandles)
        {
            if ((inHandles == null) || (inHandles.Length == 0))
            {
                return inHandles;
            }
            RuntimeTypeHandle[] handleArray = new RuntimeTypeHandle[inHandles.Length];
            for (int i = 0; i < inHandles.Length; i++)
            {
                handleArray[i] = inHandles[i];
            }
            return handleArray;
        }

        private void ValidateModulePointer()
        {
            if (this.IsNullHandle())
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_NullModuleHandle"));
            }
        }

        public RuntimeTypeHandle GetRuntimeTypeHandleFromMetadataToken(int typeToken)
        {
            return this.ResolveTypeHandle(typeToken);
        }

        public RuntimeTypeHandle ResolveTypeHandle(int typeToken)
        {
            return this.ResolveTypeHandle(typeToken, null, null);
        }

        public unsafe RuntimeTypeHandle ResolveTypeHandle(int typeToken, RuntimeTypeHandle[] typeInstantiationContext, RuntimeTypeHandle[] methodInstantiationContext)
        {
            this.ValidateModulePointer();
            if (!this.GetMetadataImport().IsValidToken(typeToken))
            {
                throw new ArgumentOutOfRangeException("metadataToken", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_InvalidToken", new object[] { typeToken, this }), new object[0]));
            }
            typeInstantiationContext = CopyRuntimeTypeHandles(typeInstantiationContext);
            methodInstantiationContext = CopyRuntimeTypeHandles(methodInstantiationContext);
            if ((typeInstantiationContext != null) && (typeInstantiationContext.Length != 0))
            {
                goto Label_00C0;
            }
            if ((methodInstantiationContext == null) || (methodInstantiationContext.Length == 0))
            {
                return this.ResolveType(typeToken, null, 0, null, 0);
            }
            int length = methodInstantiationContext.Length;
            fixed (RuntimeTypeHandle* handleRef = methodInstantiationContext)
            {
                return this.ResolveType(typeToken, null, 0, handleRef, length);
            Label_00C0:
                if ((methodInstantiationContext != null) && (methodInstantiationContext.Length != 0))
                {
                    goto Label_00F7;
                }
                int typeInstCount = typeInstantiationContext.Length;
                fixed (RuntimeTypeHandle* handleRef2 = typeInstantiationContext)
                {
                    int num3;
                    return this.ResolveType(typeToken, handleRef2, typeInstCount, null, 0);
                Label_00F7:
                    num3 = typeInstantiationContext.Length;
                    int methodInstCount = methodInstantiationContext.Length;
                    fixed (RuntimeTypeHandle* handleRef3 = typeInstantiationContext)
                    {
                        fixed (RuntimeTypeHandle* handleRef4 = methodInstantiationContext)
                        {
                            return this.ResolveType(typeToken, handleRef3, num3, handleRef4, methodInstCount);
                        }
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern unsafe RuntimeTypeHandle ResolveType(int typeToken, RuntimeTypeHandle* typeInstArgs, int typeInstCount, RuntimeTypeHandle* methodInstArgs, int methodInstCount);
        public RuntimeMethodHandle GetRuntimeMethodHandleFromMetadataToken(int methodToken)
        {
            return this.ResolveMethodHandle(methodToken);
        }

        public RuntimeMethodHandle ResolveMethodHandle(int methodToken)
        {
            return this.ResolveMethodHandle(methodToken, null, null);
        }

        public unsafe RuntimeMethodHandle ResolveMethodHandle(int methodToken, RuntimeTypeHandle[] typeInstantiationContext, RuntimeTypeHandle[] methodInstantiationContext)
        {
            this.ValidateModulePointer();
            if (!this.GetMetadataImport().IsValidToken(methodToken))
            {
                throw new ArgumentOutOfRangeException("metadataToken", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_InvalidToken", new object[] { methodToken, this }), new object[0]));
            }
            typeInstantiationContext = CopyRuntimeTypeHandles(typeInstantiationContext);
            methodInstantiationContext = CopyRuntimeTypeHandles(methodInstantiationContext);
            if ((typeInstantiationContext != null) && (typeInstantiationContext.Length != 0))
            {
                goto Label_00C0;
            }
            if ((methodInstantiationContext == null) || (methodInstantiationContext.Length == 0))
            {
                return this.ResolveMethod(methodToken, null, 0, null, 0);
            }
            int length = methodInstantiationContext.Length;
            fixed (RuntimeTypeHandle* handleRef = methodInstantiationContext)
            {
                return this.ResolveMethod(methodToken, null, 0, handleRef, length);
            Label_00C0:
                if ((methodInstantiationContext != null) && (methodInstantiationContext.Length != 0))
                {
                    goto Label_00F7;
                }
                int typeInstCount = typeInstantiationContext.Length;
                fixed (RuntimeTypeHandle* handleRef2 = typeInstantiationContext)
                {
                    int num3;
                    return this.ResolveMethod(methodToken, handleRef2, typeInstCount, null, 0);
                Label_00F7:
                    num3 = typeInstantiationContext.Length;
                    int methodInstCount = methodInstantiationContext.Length;
                    fixed (RuntimeTypeHandle* handleRef3 = typeInstantiationContext)
                    {
                        fixed (RuntimeTypeHandle* handleRef4 = methodInstantiationContext)
                        {
                            return this.ResolveMethod(methodToken, handleRef3, num3, handleRef4, methodInstCount);
                        }
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern unsafe RuntimeMethodHandle ResolveMethod(int methodToken, RuntimeTypeHandle* typeInstArgs, int typeInstCount, RuntimeTypeHandle* methodInstArgs, int methodInstCount);
        public RuntimeFieldHandle GetRuntimeFieldHandleFromMetadataToken(int fieldToken)
        {
            return this.ResolveFieldHandle(fieldToken);
        }

        public RuntimeFieldHandle ResolveFieldHandle(int fieldToken)
        {
            return this.ResolveFieldHandle(fieldToken, null, null);
        }

        public unsafe RuntimeFieldHandle ResolveFieldHandle(int fieldToken, RuntimeTypeHandle[] typeInstantiationContext, RuntimeTypeHandle[] methodInstantiationContext)
        {
            this.ValidateModulePointer();
            if (!this.GetMetadataImport().IsValidToken(fieldToken))
            {
                throw new ArgumentOutOfRangeException("metadataToken", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_InvalidToken", new object[] { fieldToken, this }), new object[0]));
            }
            typeInstantiationContext = CopyRuntimeTypeHandles(typeInstantiationContext);
            methodInstantiationContext = CopyRuntimeTypeHandles(methodInstantiationContext);
            if ((typeInstantiationContext != null) && (typeInstantiationContext.Length != 0))
            {
                goto Label_00C0;
            }
            if ((methodInstantiationContext == null) || (methodInstantiationContext.Length == 0))
            {
                return this.ResolveField(fieldToken, null, 0, null, 0);
            }
            int length = methodInstantiationContext.Length;
            fixed (RuntimeTypeHandle* handleRef = methodInstantiationContext)
            {
                return this.ResolveField(fieldToken, null, 0, handleRef, length);
            Label_00C0:
                if ((methodInstantiationContext != null) && (methodInstantiationContext.Length != 0))
                {
                    goto Label_00F7;
                }
                int typeInstCount = typeInstantiationContext.Length;
                fixed (RuntimeTypeHandle* handleRef2 = typeInstantiationContext)
                {
                    int num3;
                    return this.ResolveField(fieldToken, handleRef2, typeInstCount, null, 0);
                Label_00F7:
                    num3 = typeInstantiationContext.Length;
                    int methodInstCount = methodInstantiationContext.Length;
                    fixed (RuntimeTypeHandle* handleRef3 = typeInstantiationContext)
                    {
                        fixed (RuntimeTypeHandle* handleRef4 = methodInstantiationContext)
                        {
                            return this.ResolveField(fieldToken, handleRef3, num3, handleRef4, methodInstCount);
                        }
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern unsafe RuntimeFieldHandle ResolveField(int fieldToken, RuntimeTypeHandle* typeInstArgs, int typeInstCount, RuntimeTypeHandle* methodInstArgs, int methodInstCount);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern Module GetModule();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern unsafe void* _GetModuleTypeHandle();
        internal RuntimeTypeHandle GetModuleTypeHandle()
        {
            return new RuntimeTypeHandle(this._GetModuleTypeHandle());
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void _GetPEKind(out int peKind, out int machine);
        internal void GetPEKind(out PortableExecutableKinds peKind, out ImageFileMachine machine)
        {
            int num;
            int num2;
            this._GetPEKind(out num, out num2);
            peKind = (PortableExecutableKinds) num;
            machine = (ImageFileMachine) num2;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern int _GetMDStreamVersion();
        public int MDStreamVersion
        {
            get
            {
                return this._GetMDStreamVersion();
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern unsafe void* _GetMetadataImport();
        internal MetadataImport GetMetadataImport()
        {
            return new MetadataImport((IntPtr) this._GetMetadataImport());
        }

        static ModuleHandle()
        {
            EmptyHandle = new ModuleHandle(null);
        }
    }
}

