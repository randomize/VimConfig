namespace System
{
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    [Serializable, ComVisible(true)]
    public class ObjectDisposedException : InvalidOperationException
    {
        private string objectName;

        private ObjectDisposedException() : this(null, Environment.GetResourceString("ObjectDisposed_Generic"))
        {
        }

        public ObjectDisposedException(string objectName) : this(objectName, Environment.GetResourceString("ObjectDisposed_Generic"))
        {
        }

        protected ObjectDisposedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.objectName = info.GetString("ObjectName");
        }

        public ObjectDisposedException(string message, Exception innerException) : base(message, innerException)
        {
            base.SetErrorCode(-2146232798);
        }

        public ObjectDisposedException(string objectName, string message) : base(message)
        {
            base.SetErrorCode(-2146232798);
            this.objectName = objectName;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ObjectName", this.ObjectName, typeof(string));
        }

        public override string Message
        {
            get
            {
                string objectName = this.ObjectName;
                if ((objectName == null) || (objectName.Length == 0))
                {
                    return base.Message;
                }
                return (base.Message + Environment.NewLine + string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("ObjectDisposed_ObjectName_Name"), new object[] { objectName }));
            }
        }

        public string ObjectName
        {
            get
            {
                if (this.objectName == null)
                {
                    return string.Empty;
                }
                return this.objectName;
            }
        }
    }
}

