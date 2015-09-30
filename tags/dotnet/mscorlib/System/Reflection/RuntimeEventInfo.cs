namespace System.Reflection
{
    using System;
    using System.Collections;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    [Serializable]
    internal sealed class RuntimeEventInfo : EventInfo, ISerializable
    {
        private RuntimeMethodInfo m_addMethod;
        private System.Reflection.BindingFlags m_bindingFlags;
        private RuntimeType m_declaringType;
        private EventAttributes m_flags;
        private string m_name;
        private MethodInfo[] m_otherMethod;
        private RuntimeMethodInfo m_raiseMethod;
        private RuntimeType.RuntimeTypeCache m_reflectedTypeCache;
        private RuntimeMethodInfo m_removeMethod;
        private int m_token;
        private unsafe void* m_utf8name;

        internal RuntimeEventInfo()
        {
        }

        internal unsafe RuntimeEventInfo(int tkEvent, RuntimeType declaredType, RuntimeType.RuntimeTypeCache reflectedTypeCache, out bool isPrivate)
        {
            RuntimeMethodInfo info;
            MetadataImport metadataImport = declaredType.Module.MetadataImport;
            this.m_token = tkEvent;
            this.m_reflectedTypeCache = reflectedTypeCache;
            this.m_declaringType = declaredType;
            RuntimeTypeHandle typeHandleInternal = declaredType.GetTypeHandleInternal();
            RuntimeTypeHandle runtimeTypeHandle = reflectedTypeCache.RuntimeTypeHandle;
            metadataImport.GetEventProps(tkEvent, out this.m_utf8name, out this.m_flags);
            int associatesCount = metadataImport.GetAssociatesCount(tkEvent);
            AssociateRecord* result = (AssociateRecord*) stackalloc byte[(sizeof(AssociateRecord) * associatesCount)];
            metadataImport.GetAssociates(tkEvent, result, associatesCount);
            Associates.AssignAssociates(result, associatesCount, typeHandleInternal, runtimeTypeHandle, out this.m_addMethod, out this.m_removeMethod, out this.m_raiseMethod, out info, out info, out this.m_otherMethod, out isPrivate, out this.m_bindingFlags);
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        internal override bool CacheEquals(object o)
        {
            RuntimeEventInfo info = o as RuntimeEventInfo;
            if (info == null)
            {
                return false;
            }
            return ((info.m_token == this.m_token) && this.m_declaringType.GetTypeHandleInternal().GetModuleHandle().Equals(info.m_declaringType.GetTypeHandleInternal().GetModuleHandle()));
        }

        public override MethodInfo GetAddMethod(bool nonPublic)
        {
            if (!Associates.IncludeAccessor(this.m_addMethod, nonPublic))
            {
                return null;
            }
            return this.m_addMethod;
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

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            MemberInfoSerializationHolder.GetSerializationInfo(info, this.Name, this.ReflectedType, null, MemberTypes.Event);
        }

        public override MethodInfo[] GetOtherMethods(bool nonPublic)
        {
            ArrayList list = new ArrayList();
            if (this.m_otherMethod == null)
            {
                return new MethodInfo[0];
            }
            for (int i = 0; i < this.m_otherMethod.Length; i++)
            {
                if (Associates.IncludeAccessor(this.m_otherMethod[i], nonPublic))
                {
                    list.Add(this.m_otherMethod[i]);
                }
            }
            return (list.ToArray(typeof(MethodInfo)) as MethodInfo[]);
        }

        public override MethodInfo GetRaiseMethod(bool nonPublic)
        {
            if (!Associates.IncludeAccessor(this.m_raiseMethod, nonPublic))
            {
                return null;
            }
            return this.m_raiseMethod;
        }

        public override MethodInfo GetRemoveMethod(bool nonPublic)
        {
            if (!Associates.IncludeAccessor(this.m_removeMethod, nonPublic))
            {
                return null;
            }
            return this.m_removeMethod;
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
            if ((this.m_addMethod == null) || (this.m_addMethod.GetParametersNoCopy().Length == 0))
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_NoPublicAddMethod"));
            }
            return (this.m_addMethod.GetParametersNoCopy()[0].ParameterType.SigToString() + " " + this.Name);
        }

        public override EventAttributes Attributes
        {
            get
            {
                return this.m_flags;
            }
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
                return this.m_declaringType;
            }
        }

        public override MemberTypes MemberType
        {
            get
            {
                return MemberTypes.Event;
            }
        }

        public override int MetadataToken
        {
            get
            {
                return this.m_token;
            }
        }

        public override System.Reflection.Module Module
        {
            get
            {
                return this.m_declaringType.Module;
            }
        }

        public override string Name
        {
            get
            {
                if (this.m_name == null)
                {
                    this.m_name = new Utf8String(this.m_utf8name).ToString();
                }
                return this.m_name;
            }
        }

        public override Type ReflectedType
        {
            get
            {
                return this.m_reflectedTypeCache.RuntimeType;
            }
        }
    }
}

