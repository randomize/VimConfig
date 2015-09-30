namespace UnityEditor
{
    using System;

    [AttributeUsage(AttributeTargets.Class, Inherited=false, AllowMultiple=false)]
    public sealed class CustomPreviewAttribute : Attribute
    {
        internal System.Type m_Type;

        public CustomPreviewAttribute(System.Type type)
        {
            this.m_Type = type;
        }
    }
}

