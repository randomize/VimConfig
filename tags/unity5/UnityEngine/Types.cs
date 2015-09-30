namespace UnityEngine
{
    using System;
    using System.Reflection;

    public static class Types
    {
        public static System.Type GetType(string typeName, string assemblyName)
        {
            try
            {
                return Assembly.Load(assemblyName).GetType(typeName);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}

