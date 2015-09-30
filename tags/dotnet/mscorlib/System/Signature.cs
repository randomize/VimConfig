namespace System
{
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal class Signature
    {
        internal SignatureStruct m_signature;

        public Signature(RuntimeFieldHandle fieldHandle, RuntimeTypeHandle declaringTypeHandle)
        {
            SignatureStruct signature = new SignatureStruct();
            GetSignature(ref signature, null, 0, fieldHandle, new RuntimeMethodHandle(null), declaringTypeHandle);
            this.m_signature = signature;
        }

        public Signature(RuntimeMethodHandle methodHandle, RuntimeTypeHandle declaringTypeHandle)
        {
            SignatureStruct signature = new SignatureStruct();
            GetSignature(ref signature, null, 0, new RuntimeFieldHandle(null), methodHandle, declaringTypeHandle);
            this.m_signature = signature;
        }

        public unsafe Signature(void* pCorSig, int cCorSig, RuntimeTypeHandle declaringTypeHandle)
        {
            SignatureStruct signature = new SignatureStruct();
            GetSignature(ref signature, pCorSig, cCorSig, new RuntimeFieldHandle(null), new RuntimeMethodHandle(null), declaringTypeHandle);
            this.m_signature = signature;
        }

        public Signature(RuntimeMethodHandle method, RuntimeTypeHandle[] arguments, RuntimeTypeHandle returnType, CallingConventions callingConvention)
        {
            SignatureStruct signature = new SignatureStruct(method, arguments, returnType, callingConvention);
            GetSignatureForDynamicMethod(ref signature, method);
            this.m_signature = signature;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern unsafe void _GetSignature(ref SignatureStruct signature, void* pCorSig, int cCorSig, IntPtr fieldHandle, IntPtr methodHandle, IntPtr declaringTypeHandle);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool CompareSig(ref SignatureStruct left, RuntimeTypeHandle typeLeft, ref SignatureStruct right, RuntimeTypeHandle typeRight);
        internal static bool DiffSigs(Signature sig1, RuntimeTypeHandle typeHandle1, Signature sig2, RuntimeTypeHandle typeHandle2)
        {
            SignatureStruct left = (SignatureStruct) sig1;
            SignatureStruct right = (SignatureStruct) sig2;
            return CompareSig(ref left, typeHandle1, ref right, typeHandle2);
        }

        public Type[] GetCustomModifiers(int position, bool required)
        {
            RuntimeTypeHandle[] handleArray = null;
            RuntimeTypeHandle[] optional = null;
            SignatureStruct signature = (SignatureStruct) this;
            GetCustomModifiers(ref signature, position, out handleArray, out optional);
            Type[] typeArray = new Type[required ? handleArray.Length : optional.Length];
            if (required)
            {
                for (int j = 0; j < typeArray.Length; j++)
                {
                    typeArray[j] = handleArray[j].GetRuntimeType();
                }
                return typeArray;
            }
            for (int i = 0; i < typeArray.Length; i++)
            {
                typeArray[i] = optional[i].GetRuntimeType();
            }
            return typeArray;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void GetCustomModifiers(ref SignatureStruct signature, int parameter, out RuntimeTypeHandle[] required, out RuntimeTypeHandle[] optional);
        private static unsafe void GetSignature(ref SignatureStruct signature, void* pCorSig, int cCorSig, RuntimeFieldHandle fieldHandle, RuntimeMethodHandle methodHandle, RuntimeTypeHandle declaringTypeHandle)
        {
            _GetSignature(ref signature, pCorSig, cCorSig, fieldHandle.Value, methodHandle.Value, declaringTypeHandle.Value);
        }

        internal static void GetSignatureForDynamicMethod(ref SignatureStruct signature, RuntimeMethodHandle methodHandle)
        {
            _GetSignature(ref signature, null, 0, IntPtr.Zero, methodHandle.Value, IntPtr.Zero);
        }

        public static implicit operator SignatureStruct(Signature pThis)
        {
            return pThis.m_signature;
        }

        internal RuntimeTypeHandle[] Arguments
        {
            get
            {
                return this.m_signature.m_arguments;
            }
        }

        internal CallingConventions CallingConvention
        {
            get
            {
                return (this.m_signature.m_managedCallingConvention & 0xff);
            }
        }

        internal RuntimeTypeHandle FieldTypeHandle
        {
            get
            {
                return this.m_signature.m_returnTypeORfieldType;
            }
        }

        internal RuntimeTypeHandle ReturnTypeHandle
        {
            get
            {
                return this.m_signature.m_returnTypeORfieldType;
            }
        }

        internal enum MdSigCallingConvention : byte
        {
            C = 1,
            CallConvMask = 15,
            Default = 0,
            ExplicitThis = 0x40,
            FastCall = 4,
            Field = 6,
            GenericInst = 10,
            Generics = 0x10,
            HasThis = 0x20,
            LocalSig = 7,
            Max = 11,
            Property = 8,
            StdCall = 2,
            ThisCall = 3,
            Unmgd = 9,
            Vararg = 5
        }
    }
}

