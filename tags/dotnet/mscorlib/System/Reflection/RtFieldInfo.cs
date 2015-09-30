namespace System.Reflection
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.Serialization;

    [Serializable]
    internal sealed class RtFieldInfo : RuntimeFieldInfo, ISerializable
    {
        private FieldAttributes m_fieldAttributes;
        private RuntimeFieldHandle m_fieldHandle;
        private RuntimeType m_fieldType;
        private uint m_invocationFlags;
        private string m_name;

        internal RtFieldInfo()
        {
        }

        internal RtFieldInfo(RuntimeFieldHandle handle, RuntimeType declaringType, RuntimeType.RuntimeTypeCache reflectedTypeCache, BindingFlags bindingFlags) : base(reflectedTypeCache, declaringType, bindingFlags)
        {
            this.m_fieldHandle = handle;
            this.m_fieldAttributes = this.m_fieldHandle.GetAttributes();
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        internal override bool CacheEquals(object o)
        {
            RtFieldInfo info = o as RtFieldInfo;
            if (info == null)
            {
                return false;
            }
            return info.m_fieldHandle.Equals(this.m_fieldHandle);
        }

        private void CheckConsistency(object target)
        {
            if (((this.m_fieldAttributes & FieldAttributes.Static) != FieldAttributes.Static) && !base.m_declaringType.IsInstanceOfType(target))
            {
                if (target == null)
                {
                    throw new TargetException(Environment.GetResourceString("RFLCT.Targ_StatFldReqTarg"));
                }
                throw new ArgumentException(string.Format(CultureInfo.CurrentUICulture, Environment.GetResourceString("Arg_FieldDeclTarget"), new object[] { this.Name, base.m_declaringType, target.GetType() }));
            }
        }

        internal override RuntimeFieldHandle GetFieldHandle()
        {
            return this.m_fieldHandle;
        }

        private void GetOneTimeFlags()
        {
            Type declaringType = this.DeclaringType;
            uint num = 0;
            if ((((declaringType != null) && declaringType.ContainsGenericParameters) || ((declaringType == null) && this.Module.Assembly.ReflectionOnly)) || (declaringType is ReflectionOnlyType))
            {
                num |= 2;
            }
            else
            {
                AssemblyBuilderData assemblyData = this.Module.Assembly.m_assemblyData;
                if ((assemblyData != null) && ((assemblyData.m_access & AssemblyBuilderAccess.Run) == 0))
                {
                    num |= 2;
                }
            }
            if (num == 0)
            {
                if ((this.m_fieldAttributes & FieldAttributes.InitOnly) != FieldAttributes.PrivateScope)
                {
                    num |= 0x10;
                }
                if ((this.m_fieldAttributes & FieldAttributes.HasFieldRVA) != FieldAttributes.PrivateScope)
                {
                    num |= 0x10;
                }
                if (((this.m_fieldAttributes & FieldAttributes.FieldAccessMask) != FieldAttributes.Public) || ((declaringType != null) && !declaringType.IsVisible))
                {
                    num |= 4;
                }
                Type fieldType = this.FieldType;
                if ((fieldType.IsPointer || fieldType.IsEnum) || fieldType.IsPrimitive)
                {
                    num |= 0x20;
                }
            }
            num |= 1;
            this.m_invocationFlags = num;
        }

        public override Type[] GetOptionalCustomModifiers()
        {
            return new Signature(this.m_fieldHandle, base.DeclaringTypeHandle).GetCustomModifiers(1, false);
        }

        public override object GetRawConstantValue()
        {
            throw new InvalidOperationException();
        }

        public override Type[] GetRequiredCustomModifiers()
        {
            return new Signature(this.m_fieldHandle, base.DeclaringTypeHandle).GetCustomModifiers(1, true);
        }

        public override object GetValue(object obj)
        {
            return this.InternalGetValue(obj, true);
        }

        [DebuggerStepThrough, DebuggerHidden]
        public override object GetValueDirect(TypedReference obj)
        {
            if (obj.IsNull)
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_TypedReference_Null"));
            }
            return this.m_fieldHandle.GetValueDirect(this.FieldType.TypeHandle, obj, (this.DeclaringType == null) ? RuntimeTypeHandle.EmptyHandle : this.DeclaringType.TypeHandle);
        }

        [DebuggerStepThrough, DebuggerHidden]
        internal object InternalGetValue(object obj, bool doVisibilityCheck)
        {
            return this.InternalGetValue(obj, doVisibilityCheck, true);
        }

        [DebuggerHidden, DebuggerStepThrough]
        internal object InternalGetValue(object obj, bool doVisibilityCheck, bool doCheckConsistency)
        {
            RuntimeType declaringType = this.DeclaringType as RuntimeType;
            if ((this.m_invocationFlags & 1) == 0)
            {
                this.GetOneTimeFlags();
            }
            if ((this.m_invocationFlags & 2) != 0)
            {
                if ((declaringType != null) && this.DeclaringType.ContainsGenericParameters)
                {
                    throw new InvalidOperationException(Environment.GetResourceString("Arg_UnboundGenField"));
                }
                if (((declaringType != null) || !this.Module.Assembly.ReflectionOnly) && !(declaringType is ReflectionOnlyType))
                {
                    throw new FieldAccessException();
                }
                throw new InvalidOperationException(Environment.GetResourceString("Arg_ReflectionOnlyField"));
            }
            if (doCheckConsistency)
            {
                this.CheckConsistency(obj);
            }
            RuntimeTypeHandle typeHandle = this.FieldType.TypeHandle;
            if (doVisibilityCheck && ((this.m_invocationFlags & 4) != 0))
            {
                PerformVisibilityCheckOnField(this.m_fieldHandle.Value, obj, base.m_declaringType.TypeHandle.Value, this.m_fieldAttributes, this.m_invocationFlags & 0xffffffef);
            }
            bool domainInitialized = false;
            if (declaringType == null)
            {
                return this.m_fieldHandle.GetValue(obj, typeHandle, RuntimeTypeHandle.EmptyHandle, ref domainInitialized);
            }
            domainInitialized = declaringType.DomainInitialized;
            object obj2 = this.m_fieldHandle.GetValue(obj, typeHandle, this.DeclaringType.TypeHandle, ref domainInitialized);
            declaringType.DomainInitialized = domainInitialized;
            return obj2;
        }

        [DebuggerStepThrough, DebuggerHidden]
        internal void InternalSetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture, bool doVisibilityCheck)
        {
            this.InternalSetValue(obj, value, invokeAttr, binder, culture, doVisibilityCheck, true);
        }

        [DebuggerHidden, DebuggerStepThrough]
        internal void InternalSetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture, bool doVisibilityCheck, bool doCheckConsistency)
        {
            RuntimeType declaringType = this.DeclaringType as RuntimeType;
            if ((this.m_invocationFlags & 1) == 0)
            {
                this.GetOneTimeFlags();
            }
            if ((this.m_invocationFlags & 2) != 0)
            {
                if ((declaringType != null) && declaringType.ContainsGenericParameters)
                {
                    throw new InvalidOperationException(Environment.GetResourceString("Arg_UnboundGenField"));
                }
                if (((declaringType != null) || !this.Module.Assembly.ReflectionOnly) && !(declaringType is ReflectionOnlyType))
                {
                    throw new FieldAccessException();
                }
                throw new InvalidOperationException(Environment.GetResourceString("Arg_ReflectionOnlyField"));
            }
            if (doCheckConsistency)
            {
                this.CheckConsistency(obj);
            }
            value = ((RuntimeType) this.FieldType).CheckValue(value, binder, culture, invokeAttr);
            if (doVisibilityCheck && ((this.m_invocationFlags & 20) != 0))
            {
                PerformVisibilityCheckOnField(this.m_fieldHandle.Value, obj, base.m_declaringType.TypeHandle.Value, this.m_fieldAttributes, this.m_invocationFlags);
            }
            bool domainInitialized = false;
            if (declaringType == null)
            {
                this.m_fieldHandle.SetValue(obj, value, this.FieldType.TypeHandle, this.m_fieldAttributes, RuntimeTypeHandle.EmptyHandle, ref domainInitialized);
            }
            else
            {
                domainInitialized = declaringType.DomainInitialized;
                this.m_fieldHandle.SetValue(obj, value, this.FieldType.TypeHandle, this.m_fieldAttributes, this.DeclaringType.TypeHandle, ref domainInitialized);
                declaringType.DomainInitialized = domainInitialized;
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void PerformVisibilityCheckOnField(IntPtr field, object target, IntPtr declaringType, FieldAttributes attr, uint invocationFlags);
        [DebuggerHidden, DebuggerStepThrough]
        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture)
        {
            this.InternalSetValue(obj, value, invokeAttr, binder, culture, true);
        }

        [DebuggerStepThrough, DebuggerHidden]
        public override void SetValueDirect(TypedReference obj, object value)
        {
            if (obj.IsNull)
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_TypedReference_Null"));
            }
            this.m_fieldHandle.SetValueDirect(this.FieldType.TypeHandle, obj, value, (this.DeclaringType == null) ? RuntimeTypeHandle.EmptyHandle : this.DeclaringType.TypeHandle);
        }

        public override FieldAttributes Attributes
        {
            get
            {
                return this.m_fieldAttributes;
            }
        }

        public override RuntimeFieldHandle FieldHandle
        {
            get
            {
                Type declaringType = this.DeclaringType;
                if (((declaringType == null) && this.Module.Assembly.ReflectionOnly) || (declaringType is ReflectionOnlyType))
                {
                    throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_NotAllowedInReflectionOnly"));
                }
                return this.m_fieldHandle;
            }
        }

        public override Type FieldType
        {
            get
            {
                if (this.m_fieldType == null)
                {
                    this.m_fieldType = new Signature(this.m_fieldHandle, base.DeclaringTypeHandle).FieldTypeHandle.GetRuntimeType();
                }
                return this.m_fieldType;
            }
        }

        public override int MetadataToken
        {
            get
            {
                return this.m_fieldHandle.GetToken();
            }
        }

        public override System.Reflection.Module Module
        {
            get
            {
                return this.m_fieldHandle.GetApproxDeclaringType().GetModuleHandle().GetModule();
            }
        }

        public override string Name
        {
            get
            {
                if (this.m_name == null)
                {
                    this.m_name = this.m_fieldHandle.GetName();
                }
                return this.m_name;
            }
        }
    }
}

