namespace System.Diagnostics
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, ComVisible(true), AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Constructor, Inherited=false)]
    public sealed class DebuggerHiddenAttribute : Attribute
    {
    }
}

