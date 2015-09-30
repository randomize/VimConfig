namespace System
{
    using System.Reflection;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct SignatureStruct
    {
        internal RuntimeTypeHandle[] m_arguments;
        internal unsafe void* m_sig;
        internal unsafe void* m_pCallTarget;
        internal CallingConventions m_managedCallingConvention;
        internal int m_csig;
        internal int m_numVirtualFixedArgs;
        internal int m_64bitpad;
        internal RuntimeMethodHandle m_pMethod;
        internal RuntimeTypeHandle m_declaringType;
        internal RuntimeTypeHandle m_returnTypeORfieldType;
        public unsafe SignatureStruct(RuntimeMethodHandle method, RuntimeTypeHandle[] arguments, RuntimeTypeHandle returnType, CallingConventions callingConvention)
        {
            this.m_pMethod = method;
            this.m_arguments = arguments;
            this.m_returnTypeORfieldType = returnType;
            this.m_managedCallingConvention = callingConvention;
            this.m_sig = null;
            this.m_pCallTarget = null;
            this.m_csig = 0;
            this.m_numVirtualFixedArgs = 0;
            this.m_64bitpad = 0;
            this.m_declaringType = new RuntimeTypeHandle();
        }
    }
}

