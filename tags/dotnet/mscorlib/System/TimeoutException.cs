namespace System
{
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;

    [Serializable, ComVisible(true)]
    public class TimeoutException : SystemException
    {
        public TimeoutException() : base(Environment.GetResourceString("Arg_TimeoutException"))
        {
            base.SetErrorCode(-2146233083);
        }

        public TimeoutException(string message) : base(message)
        {
            base.SetErrorCode(-2146233083);
        }

        protected TimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public TimeoutException(string message, Exception innerException) : base(message, innerException)
        {
            base.SetErrorCode(-2146233083);
        }
    }
}

