namespace System
{
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;

    [Serializable, ComVisible(true)]
    public class AppDomainUnloadedException : SystemException
    {
        public AppDomainUnloadedException() : base(Environment.GetResourceString("Arg_AppDomainUnloadedException"))
        {
            base.SetErrorCode(-2146234348);
        }

        public AppDomainUnloadedException(string message) : base(message)
        {
            base.SetErrorCode(-2146234348);
        }

        protected AppDomainUnloadedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public AppDomainUnloadedException(string message, Exception innerException) : base(message, innerException)
        {
            base.SetErrorCode(-2146234348);
        }
    }
}

