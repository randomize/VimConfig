namespace System.Runtime.Remoting
{
    using System;
    using System.Runtime.InteropServices;

    [Guid("C460E2B4-E199-412a-8456-84DC3E4838C3"), ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IObjectHandle
    {
        object Unwrap();
    }
}

