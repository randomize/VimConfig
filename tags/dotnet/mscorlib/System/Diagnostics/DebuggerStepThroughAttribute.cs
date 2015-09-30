namespace System.Diagnostics
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, ComVisible(true), AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Struct | AttributeTargets.Class, Inherited=false)]
    public sealed class DebuggerStepThroughAttribute : Attribute
    {
    }
}

