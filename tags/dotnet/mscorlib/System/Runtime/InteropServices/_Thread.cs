namespace System.Runtime.InteropServices
{
    using System;
    using System.Threading;

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown), CLSCompliant(false), TypeLibImportClass(typeof(Thread)), ComVisible(true), Guid("C281C7F1-4AA9-3517-961A-463CFED57E75")]
    public interface _Thread
    {
        void GetTypeInfoCount(out uint pcTInfo);
        void GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo);
        void GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId);
        void Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr);
    }
}

