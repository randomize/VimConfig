namespace System.Runtime.Serialization
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    [ComVisible(true)]
    public interface ISurrogateSelector
    {
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        void ChainSelector(ISurrogateSelector selector);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        ISurrogateSelector GetNextSelector();
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        ISerializationSurrogate GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector);
    }
}

