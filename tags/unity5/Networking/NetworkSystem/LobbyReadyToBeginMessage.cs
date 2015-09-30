namespace UnityEngine.Networking.NetworkSystem
{
    using System;
    using UnityEngine.Networking;

    internal class LobbyReadyToBeginMessage : MessageBase
    {
        public bool readyState;
        public byte slotId;

        public override void Deserialize(NetworkReader reader)
        {
            this.slotId = reader.ReadByte();
            this.readyState = reader.ReadBoolean();
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(this.slotId);
            writer.Write(this.readyState);
        }
    }
}

