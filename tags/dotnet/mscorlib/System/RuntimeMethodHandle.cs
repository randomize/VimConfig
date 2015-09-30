namespace System
{
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using System.Threading;

    [Serializable, StructLayout(LayoutKind.Sequential), ComVisible(true)]
    public struct RuntimeMethodHandle : ISerializable
    {
        private IntPtr m_ptr;
        internal static RuntimeMethodHandle EmptyHandle
        {
            get
            {
                return new RuntimeMethodHandle(null);
            }
        }
        internal unsafe RuntimeMethodHandle(void* pMethod)
        {
            this.m_ptr = new IntPtr(pMethod);
        }

        internal RuntimeMethodHandle(IntPtr pMethod)
        {
            this.m_ptr = pMethod;
        }

        private RuntimeMethodHandle(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            MethodInfo info2 = (RuntimeMethodInfo) info.GetValue("MethodObj", typeof(RuntimeMethodInfo));
            this.m_ptr = info2.MethodHandle.Value;
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
            RuntimeMethodInfo methodBase = (RuntimeMethodInfo) RuntimeType.GetMethodBase(this);
            info.AddValue("MethodObj", methodBase, typeof(RuntimeMethodInfo));
        }

        public IntPtr Value
        {
            [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
            get
            {
                return this.m_ptr;
            }
        }
        public override int GetHashCode()
        {
            return ValueType.GetHashCodeOfPtr(this.m_ptr);
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public override bool Equals(object obj)
        {
            if (!(obj is RuntimeMethodHandle))
            {
                return false;
            }
            RuntimeMethodHandle handle = (RuntimeMethodHandle) obj;
            return (handle.m_ptr == this.m_ptr);
        }

        public static bool operator ==(RuntimeMethodHandle left, RuntimeMethodHandle right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RuntimeMethodHandle left, RuntimeMethodHandle right)
        {
            return !left.Equals(right);
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public bool Equals(RuntimeMethodHandle handle)
        {
            return (handle.m_ptr == this.m_ptr);
        }

        internal bool IsNullHandle()
        {
            return (this.m_ptr.ToPointer() == null);
        }

        [MethodImpl(MethodImplOptions.InternalCall), SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        public extern IntPtr GetFunctionPointer();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern unsafe void _CheckLinktimeDemands(void* module, int metadataToken);
        internal unsafe void CheckLinktimeDemands(Module module, int metadataToken)
        {
            this._CheckLinktimeDemands(module.ModuleHandle.Value, metadataToken);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern unsafe bool _IsVisibleFromModule(void* source);
        internal unsafe bool IsVisibleFromModule(Module source)
        {
            return this._IsVisibleFromModule(source.ModuleHandle.Value);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern bool _IsVisibleFromType(IntPtr source);
        internal bool IsVisibleFromType(RuntimeTypeHandle source)
        {
            return this._IsVisibleFromType(source.Value);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern unsafe void* _GetCurrentMethod(ref StackCrawlMark stackMark);
        internal static RuntimeMethodHandle GetCurrentMethod(ref StackCrawlMark stackMark)
        {
            return new RuntimeMethodHandle(_GetCurrentMethod(ref stackMark));
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern MethodAttributes GetAttributes();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern MethodImplAttributes GetImplAttributes();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern string ConstructInstantiation();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern RuntimeTypeHandle GetDeclaringType();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern int GetSlot();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern int GetMethodDef();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern string GetName();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern unsafe void* _GetUtf8Name();
        internal Utf8String GetUtf8Name()
        {
            return new Utf8String(this._GetUtf8Name());
        }

        [MethodImpl(MethodImplOptions.InternalCall), DebuggerStepThrough, DebuggerHidden]
        private extern object _InvokeMethodFast(object target, object[] arguments, ref SignatureStruct sig, MethodAttributes methodAttributes, RuntimeTypeHandle typeOwner);
        [DebuggerHidden, DebuggerStepThrough]
        internal object InvokeMethodFast(object target, object[] arguments, Signature sig, MethodAttributes methodAttributes, RuntimeTypeHandle typeOwner)
        {
            SignatureStruct signature = sig.m_signature;
            object obj2 = this._InvokeMethodFast(target, arguments, ref signature, methodAttributes, typeOwner);
            sig.m_signature = signature;
            return obj2;
        }

        [MethodImpl(MethodImplOptions.InternalCall), DebuggerHidden, DebuggerStepThrough]
        private extern object _InvokeConstructor(object[] args, ref SignatureStruct signature, IntPtr declaringType);
        [DebuggerStepThrough, DebuggerHidden]
        internal object InvokeConstructor(object[] args, SignatureStruct signature, RuntimeTypeHandle declaringType)
        {
            return this._InvokeConstructor(args, ref signature, declaringType.Value);
        }

        [MethodImpl(MethodImplOptions.InternalCall), DebuggerStepThrough, DebuggerHidden]
        private extern void _SerializationInvoke(object target, ref SignatureStruct declaringTypeSig, SerializationInfo info, StreamingContext context);
        [DebuggerStepThrough, DebuggerHidden]
        internal void SerializationInvoke(object target, SignatureStruct declaringTypeSig, SerializationInfo info, StreamingContext context)
        {
            this._SerializationInvoke(target, ref declaringTypeSig, info, context);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern bool IsILStub();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern RuntimeTypeHandle[] GetMethodInstantiation();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern bool HasMethodInstantiation();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern RuntimeMethodHandle GetInstantiatingStub(RuntimeTypeHandle declaringTypeHandle, RuntimeTypeHandle[] methodInstantiation);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern RuntimeMethodHandle GetUnboxingStub();
        internal RuntimeMethodHandle GetInstantiatingStubIfNeeded(RuntimeTypeHandle declaringTypeHandle)
        {
            return this.GetInstantiatingStub(declaringTypeHandle, null);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern RuntimeMethodHandle GetMethodFromCanonical(RuntimeTypeHandle declaringTypeHandle);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern bool IsGenericMethodDefinition();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern unsafe void* _GetTypicalMethodDefinition();
        internal RuntimeMethodHandle GetTypicalMethodDefinition()
        {
            return new RuntimeMethodHandle(this._GetTypicalMethodDefinition());
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern unsafe void* _StripMethodInstantiation();
        internal RuntimeMethodHandle StripMethodInstantiation()
        {
            return new RuntimeMethodHandle(this._StripMethodInstantiation());
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern bool IsDynamicMethod();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern Resolver GetResolver();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void Destroy();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern MethodBody _GetMethodBody(IntPtr declaringType);
        internal MethodBody GetMethodBody(RuntimeTypeHandle declaringType)
        {
            return this._GetMethodBody(declaringType.Value);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern bool IsConstructor();
    }
}

