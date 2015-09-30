namespace System.Runtime.InteropServices
{
    using System;
    using System.Reflection;

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown), TypeLibImportClass(typeof(ParameterInfo)), CLSCompliant(false), ComVisible(true), Guid("993634C4-E47A-32CC-BE08-85F567DC27D6")]
    public interface _ParameterInfo
    {
        void GetTypeInfoCount(out uint pcTInfo);
        void GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo);
        void GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId);
        void Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr);
    }
}

