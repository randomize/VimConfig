namespace System.Runtime.InteropServices
{
    using System;
    using System.Reflection;

    [Guid("D002E9BA-D9E3-3749-B1D3-D565A08B13E7"), CLSCompliant(false), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), TypeLibImportClass(typeof(Module)), ComVisible(true)]
    public interface _Module
    {
        void GetTypeInfoCount(out uint pcTInfo);
        void GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo);
        void GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId);
        void Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr);
    }
}

