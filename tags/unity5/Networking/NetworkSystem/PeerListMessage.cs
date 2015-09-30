namespace UnityEngine.Networking.NetworkSystem
{
    using System;
    using UnityEngine.Networking;

    public class PeerListMessage : MessageBase
    {
        public PeerInfoMessage[] peers;

        public override void Deserialize(NetworkReader reader)
        {
            int num = reader.ReadUInt16();
            this.peers = new PeerInfoMessage[num];
            for (int i = 0; i < this.peers.Length; i++)
            {
                PeerInfoMessage message = new PeerInfoMessage();
                message.Deserialize(reader);
                this.peers[i] = message;
            }
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write((ushort) this.peers.Length);
            foreach (PeerInfoMessage message in this.peers)
            {
                message.Serialize(writer);
            }
        }
    }
}

