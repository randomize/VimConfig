namespace UnityEngine.Networking
{
    using System;
    using System.Runtime.InteropServices;

    internal class ULocalConnectionToClient : NetworkConnection
    {
        private LocalClient m_LocalClient;

        public ULocalConnectionToClient(LocalClient localClient)
        {
            base.address = "localClient";
            this.m_LocalClient = localClient;
        }

        public override void GetStatsIn(out int numMsgs, out int numBytes)
        {
            numMsgs = 0;
            numBytes = 0;
        }

        public override void GetStatsOut(out int numMsgs, out int numBufferedMsgs, out int numBytes, out int lastBufferedPerSecond)
        {
            numMsgs = 0;
            numBufferedMsgs = 0;
            numBytes = 0;
            lastBufferedPerSecond = 0;
        }

        public override bool Send(short msgType, MessageBase msg)
        {
            this.m_LocalClient.InvokeHandlerOnClient(msgType, msg, 0);
            return true;
        }

        public override bool SendByChannel(short msgType, MessageBase msg, int channelId)
        {
            this.m_LocalClient.InvokeHandlerOnClient(msgType, msg, channelId);
            return true;
        }

        public override bool SendBytes(byte[] bytes, int numBytes, int channelId)
        {
            this.m_LocalClient.InvokeBytesOnClient(bytes, channelId);
            return true;
        }

        public override bool SendUnreliable(short msgType, MessageBase msg)
        {
            this.m_LocalClient.InvokeHandlerOnClient(msgType, msg, 1);
            return true;
        }

        public override bool SendWriter(NetworkWriter writer, int channelId)
        {
            this.m_LocalClient.InvokeBytesOnClient(writer.AsArray(), channelId);
            return true;
        }

        public LocalClient localClient
        {
            get
            {
                return this.m_LocalClient;
            }
        }
    }
}

