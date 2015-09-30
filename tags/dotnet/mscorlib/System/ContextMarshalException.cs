namespace System
{
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;

    [Serializable, ComVisible(true), Obsolete("ContextMarshalException is obsolete.")]
    public class ContextMarshalException : SystemException
    {
        public ContextMarshalException() : base(Environment.GetResourceString("Arg_ContextMarshalException"))
        {
            base.SetErrorCode(-2146233084);
        }

        public ContextMarshalException(string message) : base(message)
        {
            base.SetErrorCode(-2146233084);
        }

        protected ContextMarshalException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ContextMarshalException(string message, Exception inner) : base(message, inner)
        {
            base.SetErrorCode(-2146233084);
        }
    }
}

