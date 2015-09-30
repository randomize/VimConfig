namespace System.Reflection
{
    using System;
    using System.Runtime.CompilerServices;

    [Serializable]
    internal struct CustomAttributeEncodedArgument
    {
        private CustomAttributeEncodedArgument[] m_arrayValue;
        private long m_primitiveValue;
        private string m_stringValue;
        private System.Reflection.CustomAttributeType m_type;

        internal static unsafe void ParseAttributeArguments(ConstArray attributeBlob, ref CustomAttributeCtorParameter[] customAttributeCtorParameters, ref CustomAttributeNamedParameter[] customAttributeNamedParameters, Module customAttributeModule)
        {
            if (customAttributeModule == null)
            {
                throw new ArgumentNullException("customAttributeModule");
            }
            if (customAttributeNamedParameters == null)
            {
                customAttributeNamedParameters = new CustomAttributeNamedParameter[0];
            }
            CustomAttributeCtorParameter[] parameterArray = customAttributeCtorParameters;
            CustomAttributeNamedParameter[] customAttributeTypedArgument = customAttributeNamedParameters;
            ParseAttributeArguments(attributeBlob.Signature, attributeBlob.Length, ref parameterArray, ref customAttributeTypedArgument, (IntPtr) customAttributeModule.Assembly.AssemblyHandle.Value);
            customAttributeCtorParameters = parameterArray;
            customAttributeNamedParameters = customAttributeTypedArgument;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void ParseAttributeArguments(IntPtr pCa, int cCa, ref CustomAttributeCtorParameter[] CustomAttributeCtorParameters, ref CustomAttributeNamedParameter[] CustomAttributeTypedArgument, IntPtr assembly);

        public CustomAttributeEncodedArgument[] ArrayValue
        {
            get
            {
                return this.m_arrayValue;
            }
        }

        public System.Reflection.CustomAttributeType CustomAttributeType
        {
            get
            {
                return this.m_type;
            }
        }

        public long PrimitiveValue
        {
            get
            {
                return this.m_primitiveValue;
            }
        }

        public string StringValue
        {
            get
            {
                return this.m_stringValue;
            }
        }
    }
}

