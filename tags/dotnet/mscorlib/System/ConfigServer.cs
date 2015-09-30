namespace System
{
    using System.Runtime.CompilerServices;

    internal static class ConfigServer
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void RunParser(IConfigHandler factory, string fileName);
    }
}

