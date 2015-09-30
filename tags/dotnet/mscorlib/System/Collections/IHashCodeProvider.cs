namespace System.Collections
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true), Obsolete("Please use IEqualityComparer instead.")]
    public interface IHashCodeProvider
    {
        int GetHashCode(object obj);
    }
}

