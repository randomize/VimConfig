namespace UnityEngine.Networking
{
    using System;

    public class NetworkMessage
    {
        public int channelId;
        public NetworkConnection conn;
        public short msgType;
        public NetworkReader reader;

        public static string Dump(byte[] payload, int sz)
        {
            string str = "[";
            for (int i = 0; i < sz; i++)
            {
                str = str + payload[i] + " ";
            }
            return (str + "]");
        }

        public TMsg ReadMessage<TMsg>() where TMsg: MessageBase, new()
        {
            TMsg local = Activator.CreateInstance<TMsg>();
            local.Deserialize(this.reader);
            return local;
        }

        public void ReadMessage<TMsg>(TMsg msg) where TMsg: MessageBase
        {
            msg.Deserialize(this.reader);
        }
    }
}

