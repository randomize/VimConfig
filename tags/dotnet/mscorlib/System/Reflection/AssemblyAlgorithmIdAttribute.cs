namespace System.Reflection
{
    using System;
    using System.Configuration.Assemblies;
    using System.Runtime.InteropServices;

    [ComVisible(true), AttributeUsage(AttributeTargets.Assembly, Inherited=false)]
    public sealed class AssemblyAlgorithmIdAttribute : Attribute
    {
        private uint m_algId;

        public AssemblyAlgorithmIdAttribute(AssemblyHashAlgorithm algorithmId)
        {
            this.m_algId = (uint) algorithmId;
        }

        [CLSCompliant(false)]
        public AssemblyAlgorithmIdAttribute(uint algorithmId)
        {
            this.m_algId = algorithmId;
        }

        [CLSCompliant(false)]
        public uint AlgorithmId
        {
            get
            {
                return this.m_algId;
            }
        }
    }
}

