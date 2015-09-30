namespace UnityEngine.Networking.NetworkSystem
{
    using System;
    using UnityEngine.Networking;

    internal class CRCMessage : MessageBase
    {
        public CRCMessageEntry[] scripts;

        public override void Deserialize(NetworkReader reader)
        {
            int num = reader.ReadUInt16();
            this.scripts = new CRCMessageEntry[num];
            for (int i = 0; i < this.scripts.Length; i++)
            {
                this.scripts[i] = new CRCMessageEntry { name = reader.ReadString(), channel = reader.ReadByte() };
            }
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write((ushort) this.scripts.Length);
            foreach (CRCMessageEntry entry in this.scripts)
            {
                writer.Write(entry.name);
                writer.Write(entry.channel);
            }
        }
    }
}

