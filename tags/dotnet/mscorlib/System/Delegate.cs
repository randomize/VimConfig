namespace System
{
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;

    [Serializable, ClassInterface(ClassInterfaceType.AutoDual), ComVisible(true)]
    public abstract class Delegate : ICloneable, ISerializable
    {
        internal MethodBase _methodBase;
        internal IntPtr _methodPtr;
        internal IntPtr _methodPtrAux;
        internal object _target;

        private Delegate()
        {
        }

        protected Delegate(object target, string method)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            if (!this.BindToMethodName(target, Type.GetTypeHandle(target), method, DelegateBindingFlags.ClosedDelegateOnly | DelegateBindingFlags.InstanceMethodOnly))
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_DlgtTargMeth"));
            }
        }

        protected Delegate(Type target, string method)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            if (!(target is RuntimeType))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeType"), "target");
            }
            if (target.IsGenericType && target.ContainsGenericParameters)
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_UnboundGenParam"), "target");
            }
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            this.BindToMethodName(null, target.TypeHandle, method, DelegateBindingFlags.CaselessMatching | DelegateBindingFlags.OpenDelegateOnly | DelegateBindingFlags.StaticMethodOnly);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern IntPtr AdjustTarget(object target, IntPtr methodPtr);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern bool BindToMethodInfo(object target, RuntimeMethodHandle method, RuntimeTypeHandle methodType, DelegateBindingFlags flags);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern bool BindToMethodName(object target, RuntimeTypeHandle methodType, string method, DelegateBindingFlags flags);
        public virtual object Clone()
        {
            return base.MemberwiseClone();
        }

        [ComVisible(true)]
        public static Delegate Combine(params Delegate[] delegates)
        {
            if ((delegates == null) || (delegates.Length == 0))
            {
                return null;
            }
            Delegate a = delegates[0];
            for (int i = 1; i < delegates.Length; i++)
            {
                a = Combine(a, delegates[i]);
            }
            return a;
        }

        public static Delegate Combine(Delegate a, Delegate b)
        {
            if (a == null)
            {
                return b;
            }
            return a.CombineImpl(b);
        }

        protected virtual Delegate CombineImpl(Delegate d)
        {
            throw new MulticastNotSupportedException(Environment.GetResourceString("Multicast_Combine"));
        }

        public static Delegate CreateDelegate(Type type, MethodInfo method)
        {
            return CreateDelegate(type, method, true);
        }

        public static Delegate CreateDelegate(Type type, object firstArgument, MethodInfo method)
        {
            return CreateDelegate(type, firstArgument, method, true);
        }

        internal static Delegate CreateDelegate(Type type, object target, RuntimeMethodHandle method)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (!(type is RuntimeType))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeType"), "type");
            }
            if (method.IsNullHandle())
            {
                throw new ArgumentNullException("method");
            }
            Type baseType = type.BaseType;
            if ((baseType == null) || (baseType != typeof(MulticastDelegate)))
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_MustBeDelegate"), "type");
            }
            Delegate delegate2 = InternalAlloc(type.TypeHandle);
            if (!delegate2.BindToMethodInfo(target, method, method.GetDeclaringType(), DelegateBindingFlags.RelaxedSignature))
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_DlgtTargMeth"));
            }
            return delegate2;
        }

        public static Delegate CreateDelegate(Type type, object target, string method)
        {
            return CreateDelegate(type, target, method, false, true);
        }

        public static Delegate CreateDelegate(Type type, MethodInfo method, bool throwOnBindFailure)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (!(type is RuntimeType))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeType"), "type");
            }
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            if (!(method is RuntimeMethodInfo))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeMethodInfo"), "method");
            }
            Type baseType = type.BaseType;
            if ((baseType == null) || (baseType != typeof(MulticastDelegate)))
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_MustBeDelegate"), "type");
            }
            Delegate delegate2 = InternalAlloc(type.TypeHandle);
            if (delegate2.BindToMethodInfo(null, method.MethodHandle, method.DeclaringType.TypeHandle, DelegateBindingFlags.RelaxedSignature | DelegateBindingFlags.OpenDelegateOnly))
            {
                return delegate2;
            }
            if (throwOnBindFailure)
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_DlgtTargMeth"));
            }
            return null;
        }

        public static Delegate CreateDelegate(Type type, Type target, string method)
        {
            return CreateDelegate(type, target, method, false, true);
        }

        public static Delegate CreateDelegate(Type type, object firstArgument, MethodInfo method, bool throwOnBindFailure)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (!(type is RuntimeType))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeType"), "type");
            }
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            if (!(method is RuntimeMethodInfo))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeMethodInfo"), "method");
            }
            Type baseType = type.BaseType;
            if ((baseType == null) || (baseType != typeof(MulticastDelegate)))
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_MustBeDelegate"), "type");
            }
            Delegate delegate2 = InternalAlloc(type.TypeHandle);
            if (delegate2.BindToMethodInfo(firstArgument, method.MethodHandle, method.DeclaringType.TypeHandle, DelegateBindingFlags.RelaxedSignature))
            {
                return delegate2;
            }
            if (throwOnBindFailure)
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_DlgtTargMeth"));
            }
            return null;
        }

        public static Delegate CreateDelegate(Type type, object target, string method, bool ignoreCase)
        {
            return CreateDelegate(type, target, method, ignoreCase, true);
        }

        public static Delegate CreateDelegate(Type type, Type target, string method, bool ignoreCase)
        {
            return CreateDelegate(type, target, method, ignoreCase, true);
        }

        public static Delegate CreateDelegate(Type type, object target, string method, bool ignoreCase, bool throwOnBindFailure)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (!(type is RuntimeType))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeType"), "type");
            }
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            Type baseType = type.BaseType;
            if ((baseType == null) || (baseType != typeof(MulticastDelegate)))
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_MustBeDelegate"), "type");
            }
            Delegate delegate2 = InternalAlloc(type.TypeHandle);
            if (delegate2.BindToMethodName(target, Type.GetTypeHandle(target), method, (DelegateBindingFlags.NeverCloseOverNull | DelegateBindingFlags.ClosedDelegateOnly | DelegateBindingFlags.InstanceMethodOnly) | (ignoreCase ? DelegateBindingFlags.CaselessMatching : ((DelegateBindingFlags) 0))))
            {
                return delegate2;
            }
            if (throwOnBindFailure)
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_DlgtTargMeth"));
            }
            return null;
        }

        public static Delegate CreateDelegate(Type type, Type target, string method, bool ignoreCase, bool throwOnBindFailure)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (!(type is RuntimeType))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeType"), "type");
            }
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            if (!(target is RuntimeType))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeType"), "target");
            }
            if (target.IsGenericType && target.ContainsGenericParameters)
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_UnboundGenParam"), "target");
            }
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            Type baseType = type.BaseType;
            if ((baseType == null) || (baseType != typeof(MulticastDelegate)))
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_MustBeDelegate"), "type");
            }
            Delegate delegate2 = InternalAlloc(type.TypeHandle);
            if (delegate2.BindToMethodName(null, target.TypeHandle, method, (DelegateBindingFlags.OpenDelegateOnly | DelegateBindingFlags.StaticMethodOnly) | (ignoreCase ? DelegateBindingFlags.CaselessMatching : ((DelegateBindingFlags) 0))))
            {
                return delegate2;
            }
            if (throwOnBindFailure)
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_DlgtTargMeth"));
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void DelegateConstruct(object target, IntPtr slot);
        public object DynamicInvoke(params object[] args)
        {
            return this.DynamicInvokeImpl(args);
        }

        protected virtual object DynamicInvokeImpl(object[] args)
        {
            RuntimeMethodHandle methodHandle = new RuntimeMethodHandle(this.GetInvokeMethod());
            RuntimeMethodInfo methodBase = (RuntimeMethodInfo) RuntimeType.GetMethodBase(Type.GetTypeHandle(this), methodHandle);
            return methodBase.Invoke(this, BindingFlags.Default, null, args, null, true);
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !InternalEqualTypes(this, obj))
            {
                return false;
            }
            Delegate delegate2 = (Delegate) obj;
            if (((this._target == delegate2._target) && (this._methodPtr == delegate2._methodPtr)) && (this._methodPtrAux == delegate2._methodPtrAux))
            {
                return true;
            }
            if (this._methodPtrAux.IsNull())
            {
                if (!delegate2._methodPtrAux.IsNull())
                {
                    return false;
                }
                if (this._target != delegate2._target)
                {
                    return false;
                }
            }
            else
            {
                if (delegate2._methodPtrAux.IsNull())
                {
                    return false;
                }
                if (this._methodPtrAux == delegate2._methodPtrAux)
                {
                    return true;
                }
            }
            if ((this._methodBase != null) && (delegate2._methodBase != null))
            {
                return this._methodBase.Equals(delegate2._methodBase);
            }
            return this.FindMethodHandle().Equals(delegate2.FindMethodHandle());
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern RuntimeMethodHandle FindMethodHandle();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern IntPtr GetCallStub(IntPtr methodPtr);
        public override int GetHashCode()
        {
            return base.GetType().GetHashCode();
        }

        public virtual Delegate[] GetInvocationList()
        {
            return new Delegate[] { this };
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern IntPtr GetInvokeMethod();
        protected virtual MethodInfo GetMethodImpl()
        {
            if (this._methodBase == null)
            {
                RuntimeMethodHandle methodHandle = this.FindMethodHandle();
                RuntimeTypeHandle declaringType = methodHandle.GetDeclaringType();
                if ((declaringType.IsGenericTypeDefinition() || declaringType.HasInstantiation()) && ((methodHandle.GetAttributes() & MethodAttributes.Static) == MethodAttributes.PrivateScope))
                {
                    if (!(this._methodPtrAux == IntPtr.Zero))
                    {
                        declaringType = base.GetType().GetMethod("Invoke").GetParameters()[0].ParameterType.TypeHandle;
                    }
                    else
                    {
                        Type baseType = this._target.GetType();
                        Type genericTypeDefinition = declaringType.GetRuntimeType().GetGenericTypeDefinition();
                        while (true)
                        {
                            if (baseType.IsGenericType && (baseType.GetGenericTypeDefinition() == genericTypeDefinition))
                            {
                                break;
                            }
                            baseType = baseType.BaseType;
                        }
                        declaringType = baseType.TypeHandle;
                    }
                }
                this._methodBase = (MethodInfo) RuntimeType.GetMethodBase(declaringType, methodHandle);
            }
            return (MethodInfo) this._methodBase;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern IntPtr GetMulticastInvoke();
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotSupportedException();
        }

        internal virtual object GetTarget()
        {
            if (!this._methodPtrAux.IsNull())
            {
                return null;
            }
            return this._target;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern IntPtr GetUnmanagedCallSite();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern MulticastDelegate InternalAlloc(RuntimeTypeHandle type);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern MulticastDelegate InternalAllocLike(Delegate d);
        internal static Delegate InternalCreateDelegate(Type type, object firstArgument, MethodInfo method)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            Type baseType = type.BaseType;
            if ((baseType == null) || (baseType != typeof(MulticastDelegate)))
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_MustBeDelegate"), "type");
            }
            Delegate delegate2 = InternalAlloc(type.TypeHandle);
            if (!delegate2.BindToMethodInfo(firstArgument, method.MethodHandle, method.DeclaringType.TypeHandle, DelegateBindingFlags.RelaxedSignature | DelegateBindingFlags.SkipSecurityChecks))
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_DlgtTargMeth"));
            }
            return delegate2;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool InternalEqualTypes(object a, object b);
        public static bool operator ==(Delegate d1, Delegate d2)
        {
            if (d1 == null)
            {
                return (d2 == null);
            }
            return d1.Equals(d2);
        }

        public static bool operator !=(Delegate d1, Delegate d2)
        {
            if (d1 == null)
            {
                return (d2 != null);
            }
            return !d1.Equals(d2);
        }

        public static Delegate Remove(Delegate source, Delegate value)
        {
            if (source == null)
            {
                return null;
            }
            if (value == null)
            {
                return source;
            }
            if (!InternalEqualTypes(source, value))
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_DlgtTypeMis"));
            }
            return source.RemoveImpl(value);
        }

        public static Delegate RemoveAll(Delegate source, Delegate value)
        {
            Delegate delegate2 = null;
            do
            {
                delegate2 = source;
                source = Remove(source, value);
            }
            while (delegate2 != source);
            return delegate2;
        }

        protected virtual Delegate RemoveImpl(Delegate d)
        {
            if (!d.Equals(this))
            {
                return this;
            }
            return null;
        }

        public MethodInfo Method
        {
            get
            {
                return this.GetMethodImpl();
            }
        }

        public object Target
        {
            get
            {
                return this.GetTarget();
            }
        }
    }
}

