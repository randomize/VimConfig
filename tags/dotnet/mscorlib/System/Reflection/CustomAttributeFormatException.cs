namespace System.Reflection
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;

    [Serializable, ComVisible(true)]
    public class CustomAttributeFormatException : FormatException
    {
        public CustomAttributeFormatException() : base(Environment.GetResourceString("Arg_CustomAttributeFormatException"))
        {
            base.SetErrorCode(-2146232827);
        }

        public CustomAttributeFormatException(string message) : base(message)
        {
            base.SetErrorCode(-2146232827);
        }

        protected CustomAttributeFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public CustomAttributeFormatException(string message, Exception inner) : base(message, inner)
        {
            base.SetErrorCode(-2146232827);
        }
    }
}

