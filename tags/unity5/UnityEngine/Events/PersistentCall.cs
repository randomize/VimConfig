namespace UnityEngine.Events
{
    using System;
    using System.Reflection;
    using UnityEngine;
    using UnityEngine.Serialization;

    [Serializable]
    internal class PersistentCall
    {
        [FormerlySerializedAs("arguments"), SerializeField]
        private ArgumentCache m_Arguments = new ArgumentCache();
        [FormerlySerializedAs("m_Enabled"), SerializeField, FormerlySerializedAs("enabled")]
        private UnityEventCallState m_CallState = UnityEventCallState.RuntimeOnly;
        [FormerlySerializedAs("methodName"), SerializeField]
        private string m_MethodName;
        [SerializeField, FormerlySerializedAs("mode")]
        private PersistentListenerMode m_Mode;
        [FormerlySerializedAs("instance"), SerializeField]
        private UnityEngine.Object m_Target;

        private static BaseInvokableCall GetObjectCall(UnityEngine.Object target, MethodInfo method, ArgumentCache arguments)
        {
            System.Type type = typeof(UnityEngine.Object);
            if (!string.IsNullOrEmpty(arguments.unityObjectArgumentAssemblyTypeName))
            {
                System.Type type1 = System.Type.GetType(arguments.unityObjectArgumentAssemblyTypeName, false);
                if (type1 != null)
                {
                    type = type1;
                }
                else
                {
                    type = typeof(UnityEngine.Object);
                }
            }
            System.Type type2 = typeof(CachedInvokableCall<>);
            System.Type[] typeArguments = new System.Type[] { type };
            System.Type[] types = new System.Type[] { typeof(UnityEngine.Object), typeof(MethodInfo), type };
            ConstructorInfo constructor = type2.MakeGenericType(typeArguments).GetConstructor(types);
            UnityEngine.Object unityObjectArgument = arguments.unityObjectArgument;
            if ((unityObjectArgument != null) && !type.IsAssignableFrom(unityObjectArgument.GetType()))
            {
                unityObjectArgument = null;
            }
            object[] parameters = new object[] { target, method, unityObjectArgument };
            return (constructor.Invoke(parameters) as BaseInvokableCall);
        }

        public BaseInvokableCall GetRuntimeCall(UnityEventBase theEvent)
        {
            if ((this.m_CallState != UnityEventCallState.RuntimeOnly) || Application.isPlaying)
            {
                if ((this.m_CallState == UnityEventCallState.Off) || (theEvent == null))
                {
                    return null;
                }
                MethodInfo theFunction = theEvent.FindMethod(this);
                if (theFunction != null)
                {
                    switch (this.m_Mode)
                    {
                        case PersistentListenerMode.EventDefined:
                            return theEvent.GetDelegate(this.target, theFunction);

                        case PersistentListenerMode.Void:
                            return new InvokableCall(this.target, theFunction);

                        case PersistentListenerMode.Object:
                            return GetObjectCall(this.target, theFunction, this.m_Arguments);

                        case PersistentListenerMode.Int:
                            return new CachedInvokableCall<int>(this.target, theFunction, this.m_Arguments.intArgument);

                        case PersistentListenerMode.Float:
                            return new CachedInvokableCall<float>(this.target, theFunction, this.m_Arguments.floatArgument);

                        case PersistentListenerMode.String:
                            return new CachedInvokableCall<string>(this.target, theFunction, this.m_Arguments.stringArgument);

                        case PersistentListenerMode.Bool:
                            return new CachedInvokableCall<bool>(this.target, theFunction, this.m_Arguments.boolArgument);
                    }
                }
            }
            return null;
        }

        public bool IsValid()
        {
            return ((this.target != null) && !string.IsNullOrEmpty(this.methodName));
        }

        public void RegisterPersistentListener(UnityEngine.Object ttarget, string mmethodName)
        {
            this.m_Target = ttarget;
            this.m_MethodName = mmethodName;
        }

        public void UnregisterPersistentListener()
        {
            this.m_MethodName = string.Empty;
            this.m_Target = null;
        }

        public ArgumentCache arguments
        {
            get
            {
                return this.m_Arguments;
            }
        }

        public UnityEventCallState callState
        {
            get
            {
                return this.m_CallState;
            }
            set
            {
                this.m_CallState = value;
            }
        }

        public string methodName
        {
            get
            {
                return this.m_MethodName;
            }
        }

        public PersistentListenerMode mode
        {
            get
            {
                return this.m_Mode;
            }
            set
            {
                this.m_Mode = value;
            }
        }

        public UnityEngine.Object target
        {
            get
            {
                return this.m_Target;
            }
        }
    }
}

