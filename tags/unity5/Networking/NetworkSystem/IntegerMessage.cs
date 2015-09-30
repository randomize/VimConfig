namespace UnityEngine.Networking.NetworkSystem
{
    using System;
    using UnityEngine.Networking;

    public class IntegerMessage : MessageBase
    {
        public int value;

        public IntegerMessage()
        {
        }

        public IntegerMessage(int v)
        {
            this.value = v;
        }

        public override void Deserialize(NetworkReader reader)
        {
            this.value = (int) reader.ReadPackedUInt32();
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.WritePackedUInt32((uint) this.value);
        }
    }
}

