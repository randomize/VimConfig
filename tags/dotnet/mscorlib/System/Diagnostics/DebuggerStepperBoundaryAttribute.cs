namespace System.Diagnostics
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, Inherited=false), ComVisible(true)]
    public sealed class DebuggerStepperBoundaryAttribute : Attribute
    {
    }
}

