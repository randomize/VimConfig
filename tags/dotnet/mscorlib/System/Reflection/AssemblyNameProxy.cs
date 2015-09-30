namespace System.Reflection
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class AssemblyNameProxy : MarshalByRefObject
    {
        public AssemblyName GetAssemblyName(string assemblyFile)
        {
            return AssemblyName.nGetFileInformation(assemblyFile);
        }
    }
}

