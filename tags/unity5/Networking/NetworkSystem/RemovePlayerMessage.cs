namespace UnityEngine.Networking.NetworkSystem
{
    using System;
    using UnityEngine.Networking;

    public class RemovePlayerMessage : MessageBase
    {
        public short playerControllerId;

        public override void Deserialize(NetworkReader reader)
        {
            this.playerControllerId = (short) reader.ReadUInt16();
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write((ushort) this.playerControllerId);
        }
    }
}

