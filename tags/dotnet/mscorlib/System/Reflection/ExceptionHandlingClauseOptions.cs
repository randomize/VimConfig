namespace System.Reflection
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true), Flags]
    public enum ExceptionHandlingClauseOptions
    {
        Clause = 0,
        Fault = 4,
        Filter = 1,
        Finally = 2
    }
}

