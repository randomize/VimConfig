namespace System.Reflection
{
    using System;
    using System.Runtime.InteropServices;

    [AttributeUsage(AttributeTargets.Assembly, Inherited=false), ComVisible(true)]
    public sealed class AssemblyDelaySignAttribute : Attribute
    {
        private bool m_delaySign;

        public AssemblyDelaySignAttribute(bool delaySign)
        {
            this.m_delaySign = delaySign;
        }

        public bool DelaySign
        {
            get
            {
                return this.m_delaySign;
            }
        }
    }
}

