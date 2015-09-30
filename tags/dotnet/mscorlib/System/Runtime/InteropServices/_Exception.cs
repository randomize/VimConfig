namespace System.Runtime.InteropServices
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    [ComVisible(true), Guid("b36b5c63-42ef-38bc-a07e-0b34c98f164a"), InterfaceType(ComInterfaceType.InterfaceIsDual), CLSCompliant(false)]
    public interface _Exception
    {
        string ToString();
        bool Equals(object obj);
        int GetHashCode();
        Type GetType();
        string Message { get; }
        Exception GetBaseException();
        string StackTrace { get; }
        string HelpLink { get; set; }
        string Source { get; set; }
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        void GetObjectData(SerializationInfo info, StreamingContext context);
        Exception InnerException { get; }
        MethodBase TargetSite { get; }
    }
}

