namespace UnityEngine.Networking.NetworkSystem
{
    using System;
    using UnityEngine.Networking;

    public class EmptyMessage : MessageBase
    {
        public override void Deserialize(NetworkReader reader)
        {
        }

        public override void Serialize(NetworkWriter writer)
        {
        }
    }
}

