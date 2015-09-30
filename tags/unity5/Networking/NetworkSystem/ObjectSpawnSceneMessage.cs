namespace UnityEngine.Networking.NetworkSystem
{
    using System;
    using UnityEngine;
    using UnityEngine.Networking;

    internal class ObjectSpawnSceneMessage : MessageBase
    {
        public NetworkInstanceId netId;
        public byte[] payload;
        public Vector3 position;
        public NetworkSceneId sceneId;

        public override void Deserialize(NetworkReader reader)
        {
            this.netId = reader.ReadNetworkId();
            this.sceneId = reader.ReadSceneId();
            this.position = reader.ReadVector3();
            this.payload = reader.ReadBytesAndSize();
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(this.netId);
            writer.Write(this.sceneId);
            writer.Write(this.position);
            writer.WriteBytesFull(this.payload);
        }
    }
}

