namespace UnityEngine.Networking.NetworkSystem
{
    using System;
    using UnityEngine.Networking;

    internal class AnimationMessage : MessageBase
    {
        public NetworkInstanceId netId;
        public float normalizedTime;
        public byte[] parameters;
        public int stateHash;

        public override void Deserialize(NetworkReader reader)
        {
            this.netId = reader.ReadNetworkId();
            this.stateHash = (int) reader.ReadPackedUInt32();
            this.normalizedTime = reader.ReadSingle();
            this.parameters = reader.ReadBytesAndSize();
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(this.netId);
            writer.WritePackedUInt32((uint) this.stateHash);
            writer.Write(this.normalizedTime);
            writer.WriteBytesAndSize(this.parameters, this.parameters.Length);
        }
    }
}

