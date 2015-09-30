namespace UnityEngine
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
    public sealed class RequireComponent : Attribute
    {
        public System.Type m_Type0;
        public System.Type m_Type1;
        public System.Type m_Type2;

        public RequireComponent(System.Type requiredComponent)
        {
            this.m_Type0 = requiredComponent;
        }

        public RequireComponent(System.Type requiredComponent, System.Type requiredComponent2)
        {
            this.m_Type0 = requiredComponent;
            this.m_Type1 = requiredComponent2;
        }

        public RequireComponent(System.Type requiredComponent, System.Type requiredComponent2, System.Type requiredComponent3)
        {
            this.m_Type0 = requiredComponent;
            this.m_Type1 = requiredComponent2;
            this.m_Type2 = requiredComponent3;
        }
    }
}

