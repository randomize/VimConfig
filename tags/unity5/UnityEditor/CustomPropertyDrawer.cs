namespace UnityEditor
{
    using System;

    [AttributeUsage(AttributeTargets.Class, Inherited=false, AllowMultiple=true)]
    public sealed class CustomPropertyDrawer : Attribute
    {
        internal System.Type m_Type;
        internal bool m_UseForChildren;

        public CustomPropertyDrawer(System.Type type)
        {
            this.m_Type = type;
        }

        public CustomPropertyDrawer(System.Type type, bool useForChildren)
        {
            this.m_Type = type;
            this.m_UseForChildren = useForChildren;
        }
    }
}

