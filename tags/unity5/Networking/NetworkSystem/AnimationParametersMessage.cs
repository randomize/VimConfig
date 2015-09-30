namespace UnityEngine.Networking.NetworkSystem
{
    using System;
    using UnityEngine.Networking;

    internal class AnimationParametersMessage : MessageBase
    {
        public NetworkInstanceId netId;
        public byte[] parameters;

        public override void Deserialize(NetworkReader reader)
        {
            this.netId = reader.ReadNetworkId();
            this.parameters = reader.ReadBytesAndSize();
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(this.netId);
            writer.WriteBytesAndSize(this.parameters, this.parameters.Length);
        }
    }
}

