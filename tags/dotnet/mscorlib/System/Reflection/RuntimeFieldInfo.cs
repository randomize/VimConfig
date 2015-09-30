namespace System.Reflection
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    internal abstract class RuntimeFieldInfo : FieldInfo
    {
        private System.Reflection.BindingFlags m_bindingFlags;
        protected RuntimeType m_declaringType;
        protected RuntimeType.RuntimeTypeCache m_reflectedTypeCache;

        protected RuntimeFieldInfo()
        {
        }

        protected RuntimeFieldInfo(RuntimeType.RuntimeTypeCache reflectedTypeCache, RuntimeType declaringType, System.Reflection.BindingFlags bindingFlags)
        {
            this.m_bindingFlags = bindingFlags;
            this.m_declaringType = declaringType;
            this.m_reflectedTypeCache = reflectedTypeCache;
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return CustomAttribute.GetCustomAttributes(this, typeof(object) as RuntimeType);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            if (attributeType == null)
            {
                throw new ArgumentNullException("attributeType");
            }
            RuntimeType underlyingSystemType = attributeType.UnderlyingSystemType as RuntimeType;
            if (underlyingSystemType == null)
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_MustBeType"), "attributeType");
            }
            return CustomAttribute.GetCustomAttributes(this, underlyingSystemType);
        }

        internal virtual RuntimeFieldHandle GetFieldHandle()
        {
            return this.FieldHandle;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            MemberInfoSerializationHolder.GetSerializationInfo(info, this.Name, this.ReflectedType, this.ToString(), MemberTypes.Field);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            if (attributeType == null)
            {
                throw new ArgumentNullException("attributeType");
            }
            RuntimeType underlyingSystemType = attributeType.UnderlyingSystemType as RuntimeType;
            if (underlyingSystemType == null)
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_MustBeType"), "attributeType");
            }
            return CustomAttribute.IsDefined(this, underlyingSystemType);
        }

        public override string ToString()
        {
            return (this.FieldType.SigToString() + " " + this.Name);
        }

        internal System.Reflection.BindingFlags BindingFlags
        {
            get
            {
                return this.m_bindingFlags;
            }
        }

        public override Type DeclaringType
        {
            get
            {
                if (!this.m_reflectedTypeCache.IsGlobal)
                {
                    return this.m_declaringType;
                }
                return null;
            }
        }

        internal RuntimeTypeHandle DeclaringTypeHandle
        {
            get
            {
                Type declaringType = this.DeclaringType;
                if (declaringType == null)
                {
                    return this.Module.GetModuleHandle().GetModuleTypeHandle();
                }
                return declaringType.GetTypeHandleInternal();
            }
        }

        public override MemberTypes MemberType
        {
            get
            {
                return MemberTypes.Field;
            }
        }

        public override Type ReflectedType
        {
            get
            {
                if (!this.m_reflectedTypeCache.IsGlobal)
                {
                    return this.m_reflectedTypeCache.RuntimeType;
                }
                return null;
            }
        }

        private RuntimeTypeHandle ReflectedTypeHandle
        {
            get
            {
                return this.m_reflectedTypeCache.RuntimeTypeHandle;
            }
        }
    }
}

