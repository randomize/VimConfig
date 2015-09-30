namespace System.Runtime.Serialization
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    [ComVisible(true)]
    public interface IObjectReference
    {
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        object GetRealObject(StreamingContext context);
    }
}

