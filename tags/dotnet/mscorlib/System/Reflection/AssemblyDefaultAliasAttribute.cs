namespace System.Reflection
{
    using System;
    using System.Runtime.InteropServices;

    [AttributeUsage(AttributeTargets.Assembly, Inherited=false), ComVisible(true)]
    public sealed class AssemblyDefaultAliasAttribute : Attribute
    {
        private string m_defaultAlias;

        public AssemblyDefaultAliasAttribute(string defaultAlias)
        {
            this.m_defaultAlias = defaultAlias;
        }

        public string DefaultAlias
        {
            get
            {
                return this.m_defaultAlias;
            }
        }
    }
}

