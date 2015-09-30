namespace System.Reflection
{
    using System;
    using System.Runtime.InteropServices;

    [AttributeUsage(AttributeTargets.Assembly, Inherited=false), ComVisible(true)]
    public sealed class AssemblyCopyrightAttribute : Attribute
    {
        private string m_copyright;

        public AssemblyCopyrightAttribute(string copyright)
        {
            this.m_copyright = copyright;
        }

        public string Copyright
        {
            get
            {
                return this.m_copyright;
            }
        }
    }
}

