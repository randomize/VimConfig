namespace UnityEngine.Networking.NetworkSystem
{
    using System;
    using UnityEngine.Networking;

    internal class OverrideTransformMessage : MessageBase
    {
        public NetworkInstanceId netId;
        public byte[] payload;
        public bool teleport;
        public int time;

        public override void Deserialize(NetworkReader reader)
        {
            this.netId = reader.ReadNetworkId();
            this.payload = reader.ReadBytesAndSize();
            this.teleport = reader.ReadBoolean();
            this.time = (int) reader.ReadPackedUInt32();
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(this.netId);
            writer.WriteBytesFull(this.payload);
            writer.Write(this.teleport);
            writer.WritePackedUInt32((uint) this.time);
        }
    }
}

