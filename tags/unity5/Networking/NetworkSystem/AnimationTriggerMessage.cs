namespace UnityEngine.Networking.NetworkSystem
{
    using System;
    using UnityEngine.Networking;

    internal class AnimationTriggerMessage : MessageBase
    {
        public int hash;
        public NetworkInstanceId netId;

        public override void Deserialize(NetworkReader reader)
        {
            this.netId = reader.ReadNetworkId();
            this.hash = (int) reader.ReadPackedUInt32();
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(this.netId);
            writer.WritePackedUInt32((uint) this.hash);
        }
    }
}

