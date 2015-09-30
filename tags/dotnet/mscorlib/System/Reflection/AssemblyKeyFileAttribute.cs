namespace System.Reflection
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true), AttributeUsage(AttributeTargets.Assembly, Inherited=false)]
    public sealed class AssemblyKeyFileAttribute : Attribute
    {
        private string m_keyFile;

        public AssemblyKeyFileAttribute(string keyFile)
        {
            this.m_keyFile = keyFile;
        }

        public string KeyFile
        {
            get
            {
                return this.m_keyFile;
            }
        }
    }
}

