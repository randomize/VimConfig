namespace System
{
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    [Serializable, ComVisible(true)]
    public class TypeLoadException : SystemException, ISerializable
    {
        private string AssemblyName;
        private string ClassName;
        private string MessageArg;
        internal int ResourceId;

        public TypeLoadException() : base(Environment.GetResourceString("Arg_TypeLoadException"))
        {
            base.SetErrorCode(-2146233054);
        }

        public TypeLoadException(string message) : base(message)
        {
            base.SetErrorCode(-2146233054);
        }

        protected TypeLoadException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            this.ClassName = info.GetString("TypeLoadClassName");
            this.AssemblyName = info.GetString("TypeLoadAssemblyName");
            this.MessageArg = info.GetString("TypeLoadMessageArg");
            this.ResourceId = info.GetInt32("TypeLoadResourceID");
        }

        public TypeLoadException(string message, Exception inner) : base(message, inner)
        {
            base.SetErrorCode(-2146233054);
        }

        private TypeLoadException(string className, string assemblyName, string messageArg, int resourceId) : base(null)
        {
            base.SetErrorCode(-2146233054);
            this.ClassName = className;
            this.AssemblyName = assemblyName;
            this.MessageArg = messageArg;
            this.ResourceId = resourceId;
            this.SetMessageField();
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            base.GetObjectData(info, context);
            info.AddValue("TypeLoadClassName", this.ClassName, typeof(string));
            info.AddValue("TypeLoadAssemblyName", this.AssemblyName, typeof(string));
            info.AddValue("TypeLoadMessageArg", this.MessageArg, typeof(string));
            info.AddValue("TypeLoadResourceID", this.ResourceId);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern string GetTypeLoadExceptionMessage(int resourceId);
        private void SetMessageField()
        {
            if (base._message == null)
            {
                if ((this.ClassName == null) && (this.ResourceId == 0))
                {
                    base._message = Environment.GetResourceString("Arg_TypeLoadException");
                }
                else
                {
                    if (this.AssemblyName == null)
                    {
                        this.AssemblyName = Environment.GetResourceString("IO_UnknownFileName");
                    }
                    if (this.ClassName == null)
                    {
                        this.ClassName = Environment.GetResourceString("IO_UnknownFileName");
                    }
                    base._message = string.Format(CultureInfo.CurrentCulture, GetTypeLoadExceptionMessage(this.ResourceId), new object[] { this.ClassName, this.AssemblyName, this.MessageArg });
                }
            }
        }

        public override string Message
        {
            get
            {
                this.SetMessageField();
                return base._message;
            }
        }

        public string TypeName
        {
            get
            {
                if (this.ClassName == null)
                {
                    return string.Empty;
                }
                return this.ClassName;
            }
        }
    }
}

