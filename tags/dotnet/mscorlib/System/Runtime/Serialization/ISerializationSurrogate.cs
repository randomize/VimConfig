namespace System.Runtime.Serialization
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    [ComVisible(true)]
    public interface ISerializationSurrogate
    {
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        void GetObjectData(object obj, SerializationInfo info, StreamingContext context);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector);
    }
}

