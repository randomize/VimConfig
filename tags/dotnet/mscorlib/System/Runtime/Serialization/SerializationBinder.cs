namespace System.Runtime.Serialization
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, ComVisible(true)]
    public abstract class SerializationBinder
    {
        protected SerializationBinder()
        {
        }

        public abstract Type BindToType(string assemblyName, string typeName);
    }
}

