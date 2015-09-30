namespace UnityEngine.Networking.NetworkSystem
{
    using System;
    using UnityEngine.Networking;

    public class ErrorMessage : MessageBase
    {
        public int errorCode;

        public override void Deserialize(NetworkReader reader)
        {
            this.errorCode = reader.ReadUInt16();
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write((ushort) this.errorCode);
        }
    }
}

