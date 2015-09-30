namespace System
{
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;

    [Serializable, ComVisible(true)]
    public class OperationCanceledException : SystemException
    {
        public OperationCanceledException() : base(Environment.GetResourceString("OperationCanceled"))
        {
            base.SetErrorCode(-2146233029);
        }

        public OperationCanceledException(string message) : base(message)
        {
            base.SetErrorCode(-2146233029);
        }

        protected OperationCanceledException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public OperationCanceledException(string message, Exception innerException) : base(message, innerException)
        {
            base.SetErrorCode(-2146233029);
        }
    }
}

