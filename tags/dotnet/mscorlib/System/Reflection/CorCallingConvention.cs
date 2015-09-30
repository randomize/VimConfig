namespace System.Reflection
{
    using System;

    [Serializable]
    internal enum CorCallingConvention : byte
    {
        Default = 0,
        Field = 6,
        GenericInstance = 10,
        LocalSig = 7,
        Property = 8,
        Unmanaged = 9,
        Vararg = 5
    }
}

