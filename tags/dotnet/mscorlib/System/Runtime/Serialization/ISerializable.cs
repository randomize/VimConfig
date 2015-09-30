namespace System.Runtime.Serialization
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    [ComVisible(true)]
    public interface ISerializable
    {
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        void GetObjectData(SerializationInfo info, StreamingContext context);
    }
}

