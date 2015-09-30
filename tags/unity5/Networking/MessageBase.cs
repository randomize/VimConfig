namespace UnityEngine.Networking
{
    using System;

    public abstract class MessageBase
    {
        protected MessageBase()
        {
        }

        public virtual void Deserialize(NetworkReader reader)
        {
        }

        public virtual void Serialize(NetworkWriter writer)
        {
        }
    }
}

