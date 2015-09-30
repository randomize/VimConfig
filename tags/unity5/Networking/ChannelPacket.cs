namespace UnityEngine.Networking
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    internal struct ChannelPacket
    {
        private int m_Position;
        private byte[] m_Buffer;
        private bool m_IsReliable;
        public ChannelPacket(int packetSize, bool isReliable)
        {
            this.m_Position = 0;
            this.m_Buffer = new byte[packetSize];
            this.m_IsReliable = isReliable;
        }

        public void Reset()
        {
            this.m_Position = 0;
        }

        public bool IsEmpty()
        {
            return (this.m_Position == 0);
        }

        public void Write(byte[] bytes, int numBytes)
        {
            Array.Copy(bytes, 0, this.m_Buffer, this.m_Position, numBytes);
            this.m_Position += numBytes;
        }

        public bool HasSpace(int numBytes)
        {
            return ((this.m_Position + numBytes) < this.m_Buffer.Length);
        }

        public bool SendToTransport(NetworkConnection conn, int channelId)
        {
            byte num;
            bool flag = true;
            if (!conn.TransportSend(this.m_Buffer, (ushort) this.m_Position, channelId, out num) && (!this.m_IsReliable || (num != 4)))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError(string.Concat(new object[] { "Failed to send internal buffer channel:", channelId, " bytesToSend:", this.m_Position }));
                }
                flag = false;
            }
            if (num != 0)
            {
                if (this.m_IsReliable && (num == 4))
                {
                    NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Outgoing, 30, "msg", 1);
                    return false;
                }
                if (LogFilter.logError)
                {
                    Debug.LogError(string.Concat(new object[] { "Send Error: ", num, " channel:", channelId, " bytesToSend:", this.m_Position }));
                }
                flag = false;
            }
            this.m_Position = 0;
            return flag;
        }
    }
}

