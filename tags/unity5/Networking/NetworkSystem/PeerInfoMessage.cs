namespace UnityEngine.Networking.NetworkSystem
{
    using System;
    using UnityEngine.Networking;

    public class PeerInfoMessage : MessageBase
    {
        public string address;
        public int connectionId;
        public bool isHost;
        public bool isYou;
        public int port;

        public override void Deserialize(NetworkReader reader)
        {
            this.connectionId = (int) reader.ReadPackedUInt32();
            this.address = reader.ReadString();
            this.port = (int) reader.ReadPackedUInt32();
            this.isHost = reader.ReadBoolean();
            this.isYou = reader.ReadBoolean();
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.WritePackedUInt32((uint) this.connectionId);
            writer.Write(this.address);
            writer.WritePackedUInt32((uint) this.port);
            writer.Write(this.isHost);
            writer.Write(this.isYou);
        }
    }
}

