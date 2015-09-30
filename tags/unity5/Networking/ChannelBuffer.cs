namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    internal class ChannelBuffer : IDisposable
    {
        private const int k_MaxFreePacketCount = 0x200;
        private const int k_MaxPendingPacketCount = 0x10;
        private const int k_PacketHeaderReserveSize = 100;
        private byte m_ChannelId;
        private NetworkConnection m_Connection;
        private ChannelPacket m_CurrentPacket;
        private bool m_Disposed;
        private bool m_IsBroken;
        private bool m_IsReliable;
        private float m_LastBufferedMessageCountTimer = Time.time;
        private float m_LastFlushTime;
        private int m_MaxPacketSize;
        private int m_MaxPendingPacketCount;
        private List<ChannelPacket> m_PendingPackets;
        public float maxDelay = 0.01f;
        internal static int pendingPacketCount;
        private static List<ChannelPacket> s_FreePackets;
        private static NetworkWriter s_SendWriter = new NetworkWriter();

        public ChannelBuffer(NetworkConnection conn, int bufferSize, byte cid, bool isReliable)
        {
            this.m_Connection = conn;
            this.m_MaxPacketSize = bufferSize - 100;
            this.m_CurrentPacket = new ChannelPacket(this.m_MaxPacketSize, isReliable);
            this.m_ChannelId = cid;
            this.m_MaxPendingPacketCount = 0x10;
            this.m_IsReliable = isReliable;
            if (isReliable)
            {
                this.m_PendingPackets = new List<ChannelPacket>();
                if (s_FreePackets == null)
                {
                    s_FreePackets = new List<ChannelPacket>();
                }
            }
        }

        private ChannelPacket AllocPacket()
        {
            NetworkDetailStats.SetStat(NetworkDetailStats.NetworkDirection.Outgoing, 0x1f, "msg", pendingPacketCount);
            if (s_FreePackets.Count == 0)
            {
                return new ChannelPacket(this.m_MaxPacketSize, this.m_IsReliable);
            }
            ChannelPacket packet = s_FreePackets[0];
            s_FreePackets.RemoveAt(0);
            packet.Reset();
            return packet;
        }

        public void CheckInternalBuffer()
        {
            if (((Time.time - this.m_LastFlushTime) > this.maxDelay) && !this.m_CurrentPacket.IsEmpty())
            {
                this.SendInternalBuffer();
                this.m_LastFlushTime = Time.time;
            }
            if ((Time.time - this.m_LastBufferedMessageCountTimer) > 1f)
            {
                this.lastBufferedPerSecond = this.numBufferedPerSecond;
                this.numBufferedPerSecond = 0;
                this.m_LastBufferedMessageCountTimer = Time.time;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if ((!this.m_Disposed && disposing) && (this.m_PendingPackets != null))
            {
                foreach (ChannelPacket packet in this.m_PendingPackets)
                {
                    pendingPacketCount--;
                    if (s_FreePackets.Count < 0x200)
                    {
                        s_FreePackets.Add(packet);
                    }
                }
                this.m_PendingPackets.Clear();
            }
            this.m_Disposed = true;
        }

        private static void FreePacket(ChannelPacket packet)
        {
            NetworkDetailStats.SetStat(NetworkDetailStats.NetworkDirection.Outgoing, 0x1f, "msg", pendingPacketCount);
            if (s_FreePackets.Count < 0x200)
            {
                s_FreePackets.Add(packet);
            }
        }

        private void QueuePacket()
        {
            pendingPacketCount++;
            this.m_PendingPackets.Add(this.m_CurrentPacket);
            this.m_CurrentPacket = this.AllocPacket();
        }

        public bool Send(short msgType, MessageBase msg)
        {
            s_SendWriter.StartMessage(msgType);
            msg.Serialize(s_SendWriter);
            s_SendWriter.FinishMessage();
            this.numMsgsOut++;
            return this.SendWriter(s_SendWriter);
        }

        internal bool SendBytes(byte[] bytes, int bytesToSend)
        {
            NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Outgoing, 0x1c, "msg", 1);
            if (bytesToSend > this.m_MaxPacketSize)
            {
                byte num;
                if (LogFilter.logDebug)
                {
                    Debug.Log(string.Concat(new object[] { "SendBytes large packet: ", bytesToSend, "channel ", this.m_ChannelId }));
                }
                if (this.m_Connection.TransportSend(bytes, bytesToSend, this.m_ChannelId, out num))
                {
                    return true;
                }
                if (LogFilter.logError)
                {
                    Debug.LogError("Failed to send big message");
                }
                return false;
            }
            if (!this.m_CurrentPacket.HasSpace(bytesToSend))
            {
                if (this.m_IsReliable)
                {
                    if (this.m_PendingPackets.Count == 0)
                    {
                        if (!this.m_CurrentPacket.SendToTransport(this.m_Connection, this.m_ChannelId))
                        {
                            this.QueuePacket();
                        }
                        this.m_CurrentPacket.Write(bytes, bytesToSend);
                        return true;
                    }
                    if (this.m_PendingPackets.Count >= this.m_MaxPendingPacketCount)
                    {
                        if (!this.m_IsBroken && LogFilter.logError)
                        {
                            Debug.LogError("ChannelBuffer buffer limit of " + this.m_PendingPackets.Count + " packets reached.");
                        }
                        this.m_IsBroken = true;
                        return false;
                    }
                    this.QueuePacket();
                    this.m_CurrentPacket.Write(bytes, bytesToSend);
                    return true;
                }
                if (!this.m_CurrentPacket.SendToTransport(this.m_Connection, this.m_ChannelId))
                {
                    if (LogFilter.logError)
                    {
                        Debug.Log("ChannelBuffer SendBytes no space on unreliable channel " + this.m_ChannelId);
                    }
                    return false;
                }
                this.m_CurrentPacket.Write(bytes, bytesToSend);
                return true;
            }
            this.m_CurrentPacket.Write(bytes, bytesToSend);
            if (this.maxDelay == 0f)
            {
                return this.SendInternalBuffer();
            }
            return true;
        }

        public bool SendInternalBuffer()
        {
            NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Outgoing, 0x1d, "msg", 1);
            if (!this.m_IsReliable || (this.m_PendingPackets.Count <= 0))
            {
                return this.m_CurrentPacket.SendToTransport(this.m_Connection, this.m_ChannelId);
            }
            while (this.m_PendingPackets.Count > 0)
            {
                ChannelPacket packet = this.m_PendingPackets[0];
                if (!packet.SendToTransport(this.m_Connection, this.m_ChannelId))
                {
                    break;
                }
                pendingPacketCount--;
                this.m_PendingPackets.RemoveAt(0);
                FreePacket(packet);
                if (this.m_IsBroken && (this.m_PendingPackets.Count < (this.m_MaxPendingPacketCount / 2)))
                {
                    if (LogFilter.logWarn)
                    {
                        Debug.LogWarning("ChannelBuffer recovered from overflow but data was lost.");
                    }
                    this.m_IsBroken = false;
                }
            }
            return true;
        }

        public bool SendWriter(NetworkWriter writer)
        {
            return this.SendBytes(writer.AsArraySegment().Array, writer.AsArraySegment().Count);
        }

        public bool SetOption(ChannelOption option, int value)
        {
            if (option != ChannelOption.MaxPendingBuffers)
            {
                return false;
            }
            if (!this.m_IsReliable)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("Cannot set MaxPendingBuffers on unreliable channel " + this.m_ChannelId);
                }
                return false;
            }
            if ((value < 0) || (value >= 0x200))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError(string.Concat(new object[] { "Invalid MaxPendingBuffers for channel ", this.m_ChannelId, ". Must be greater than zero and less than ", 0x200 }));
                }
                return false;
            }
            this.m_MaxPendingPacketCount = value;
            return true;
        }

        public int lastBufferedPerSecond { get; private set; }

        public int numBufferedMsgsOut { get; private set; }

        public int numBufferedPerSecond { get; private set; }

        public int numBytesIn { get; private set; }

        public int numBytesOut { get; private set; }

        public int numMsgsIn { get; private set; }

        public int numMsgsOut { get; private set; }
    }
}

