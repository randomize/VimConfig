namespace System.Runtime.Serialization
{
    using System;

    internal sealed class SurrogateForCyclicalReference : ISerializationSurrogate
    {
        private ISerializationSurrogate innerSurrogate;

        internal SurrogateForCyclicalReference(ISerializationSurrogate innerSurrogate)
        {
            if (innerSurrogate == null)
            {
                throw new ArgumentNullException("innerSurrogate");
            }
            this.innerSurrogate = innerSurrogate;
        }

        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            this.innerSurrogate.GetObjectData(obj, info, context);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            return this.innerSurrogate.SetObjectData(obj, info, context, selector);
        }
    }
}

