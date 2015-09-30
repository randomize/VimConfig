namespace UnityEngine.Networking.NetworkSystem
{
    using System;
    using UnityEngine.Networking;

    public class AddPlayerMessage : MessageBase
    {
        public byte[] msgData;
        public int msgSize;
        public short playerControllerId;

        public override void Deserialize(NetworkReader reader)
        {
            this.playerControllerId = (short) reader.ReadUInt16();
            this.msgData = reader.ReadBytesAndSize();
            if (this.msgData == null)
            {
                this.msgSize = 0;
            }
            else
            {
                this.msgSize = this.msgData.Length;
            }
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write((ushort) this.playerControllerId);
            writer.WriteBytesAndSize(this.msgData, this.msgSize);
        }
    }
}

