namespace System.Runtime.Serialization
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, ComVisible(true)]
    public class SerializationException : SystemException
    {
        private static string _nullMessage = Environment.GetResourceString("Arg_SerializationException");

        public SerializationException() : base(_nullMessage)
        {
            base.SetErrorCode(-2146233076);
        }

        public SerializationException(string message) : base(message)
        {
            base.SetErrorCode(-2146233076);
        }

        protected SerializationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public SerializationException(string message, Exception innerException) : base(message, innerException)
        {
            base.SetErrorCode(-2146233076);
        }
    }
}

