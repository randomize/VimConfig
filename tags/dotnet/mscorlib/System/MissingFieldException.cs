namespace System
{
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;

    [Serializable, ComVisible(true)]
    public class MissingFieldException : MissingMemberException, ISerializable
    {
        public MissingFieldException() : base(Environment.GetResourceString("Arg_MissingFieldException"))
        {
            base.SetErrorCode(-2146233071);
        }

        public MissingFieldException(string message) : base(message)
        {
            base.SetErrorCode(-2146233071);
        }

        protected MissingFieldException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public MissingFieldException(string message, Exception inner) : base(message, inner)
        {
            base.SetErrorCode(-2146233071);
        }

        public MissingFieldException(string className, string fieldName)
        {
            base.ClassName = className;
            base.MemberName = fieldName;
        }

        private MissingFieldException(string className, string fieldName, byte[] signature)
        {
            base.ClassName = className;
            base.MemberName = fieldName;
            base.Signature = signature;
        }

        public override string Message
        {
            get
            {
                if (base.ClassName == null)
                {
                    return base.Message;
                }
                return string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("MissingField_Name", new object[] { ((base.Signature != null) ? (MissingMemberException.FormatSignature(base.Signature) + " ") : "") + base.ClassName + "." + base.MemberName }), new object[0]);
            }
        }
    }
}

