namespace System
{
    using System.Runtime.InteropServices;

    [AttributeUsage(AttributeTargets.Method), ComVisible(true)]
    public sealed class STAThreadAttribute : Attribute
    {
    }
}

