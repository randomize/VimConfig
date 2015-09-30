namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEditor;
    using UnityEngine;

    public class NetworkConnection : IDisposable
    {
        public string address;
        public int connectionId = -1;
        public int hostId = -1;
        public bool isReady;
        private const int k_MaxMessageLogSize = 150;
        public float lastMessageTime;
        public bool logNetworkMessages;
        private ChannelBuffer[] m_Channels;
        private HashSet<NetworkInstanceId> m_ClientOwnedObjects;
        private bool m_Disposed;
        private NetworkMessageHandlers m_MessageHandlers;
        private Dictionary<short, NetworkMessageDelegate> m_MessageHandlersDict;
        private NetworkMessage m_MessageInfo = new NetworkMessage();
        private NetworkMessage m_NetMsg = new NetworkMessage();
        private Dictionary<short, PacketStat> m_PacketStats = new Dictionary<short, PacketStat>();
        private List<PlayerController> m_PlayerControllers = new List<PlayerController>();
        private HashSet<NetworkIdentity> m_VisList = new HashSet<NetworkIdentity>();
        private NetworkWriter m_Writer = new NetworkWriter();
        private static int s_MaxPacketStats = 0xff;

        internal void AddOwnedObject(NetworkIdentity obj)
        {
            if (this.m_ClientOwnedObjects == null)
            {
                this.m_ClientOwnedObjects = new HashSet<NetworkInstanceId>();
            }
            this.m_ClientOwnedObjects.Add(obj.netId);
        }

        internal void AddToVisList(NetworkIdentity uv)
        {
            this.m_VisList.Add(uv);
            NetworkServer.ShowForConnection(uv, this);
        }

        private bool CheckChannel(int channelId)
        {
            if (this.m_Channels == null)
            {
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning("Channels not initialized sending on id '" + channelId);
                }
                return false;
            }
            if ((channelId >= 0) && (channelId < this.m_Channels.Length))
            {
                return true;
            }
            if (LogFilter.logError)
            {
                Debug.LogError(string.Concat(new object[] { "Invalid channel when sending buffered data, '", channelId, "'. Current channel count is ", this.m_Channels.Length }));
            }
            return false;
        }

        public void Disconnect()
        {
            this.address = string.Empty;
            this.isReady = false;
            ClientScene.HandleClientDisconnect(this);
            if (this.hostId != -1)
            {
                byte num;
                NetworkTransport.Disconnect(this.hostId, this.connectionId, out num);
                this.RemoveObservers();
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.m_Disposed && (this.m_Channels != null))
            {
                for (int i = 0; i < this.m_Channels.Length; i++)
                {
                    this.m_Channels[i].Dispose();
                }
            }
            this.m_Channels = null;
            if (this.m_ClientOwnedObjects != null)
            {
                foreach (NetworkInstanceId id in this.m_ClientOwnedObjects)
                {
                    GameObject obj2 = NetworkServer.FindLocalObject(id);
                    if (obj2 != null)
                    {
                        obj2.GetComponent<NetworkIdentity>().ClearClientOwner();
                    }
                }
            }
            this.m_ClientOwnedObjects = null;
            this.m_Disposed = true;
        }

        ~NetworkConnection()
        {
            this.Dispose(false);
        }

        public void FlushChannels()
        {
            if (this.m_Channels != null)
            {
                foreach (ChannelBuffer buffer in this.m_Channels)
                {
                    buffer.CheckInternalBuffer();
                }
            }
        }

        internal bool GetPlayerController(short playerControllerId, out PlayerController playerController)
        {
            playerController = null;
            if (this.playerControllers.Count > 0)
            {
                for (int i = 0; i < this.playerControllers.Count; i++)
                {
                    if (this.playerControllers[i].IsValid && (this.playerControllers[i].playerControllerId == playerControllerId))
                    {
                        playerController = this.playerControllers[i];
                        return true;
                    }
                }
            }
            return false;
        }

        public virtual void GetStatsIn(out int numMsgs, out int numBytes)
        {
            numMsgs = 0;
            numBytes = 0;
            foreach (ChannelBuffer buffer in this.m_Channels)
            {
                numMsgs += buffer.numMsgsIn;
                numBytes += buffer.numBytesIn;
            }
        }

        public virtual void GetStatsOut(out int numMsgs, out int numBufferedMsgs, out int numBytes, out int lastBufferedPerSecond)
        {
            numMsgs = 0;
            numBufferedMsgs = 0;
            numBytes = 0;
            lastBufferedPerSecond = 0;
            foreach (ChannelBuffer buffer in this.m_Channels)
            {
                numMsgs += buffer.numMsgsOut;
                numBufferedMsgs += buffer.numBufferedMsgsOut;
                numBytes += buffer.numBytesOut;
                lastBufferedPerSecond += buffer.lastBufferedPerSecond;
            }
        }

        protected void HandleBytes(byte[] buffer, int receivedSize, int channelId)
        {
            NetworkReader reader = new NetworkReader(buffer);
            this.HandleReader(reader, receivedSize, channelId);
        }

        protected void HandleReader(NetworkReader reader, int receivedSize, int channelId)
        {
            while (reader.Position < receivedSize)
            {
                ushort count = reader.ReadUInt16();
                short key = reader.ReadInt16();
                byte[] buffer = reader.ReadBytes(count);
                NetworkReader reader2 = new NetworkReader(buffer);
                if (this.logNetworkMessages)
                {
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < count; i++)
                    {
                        builder.AppendFormat("{0:X2}", buffer[i]);
                        if (i > 150)
                        {
                            break;
                        }
                    }
                    Debug.Log(string.Concat(new object[] { "ConnectionRecv con:", this.connectionId, " bytes:", count, " msgId:", key, " ", builder }));
                }
                NetworkMessageDelegate delegate2 = null;
                if (this.m_MessageHandlersDict.ContainsKey(key))
                {
                    delegate2 = this.m_MessageHandlersDict[key];
                }
                if (delegate2 != null)
                {
                    this.m_NetMsg.msgType = key;
                    this.m_NetMsg.reader = reader2;
                    this.m_NetMsg.conn = this;
                    this.m_NetMsg.channelId = channelId;
                    delegate2(this.m_NetMsg);
                    this.lastMessageTime = Time.time;
                    NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Incoming, 0x1c, "msg", 1);
                    if (key > 0x2e)
                    {
                        NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Incoming, 0, key.ToString() + ":" + key.GetType().Name, 1);
                    }
                    if (this.m_PacketStats.ContainsKey(key))
                    {
                        PacketStat stat = this.m_PacketStats[key];
                        stat.count++;
                        stat.bytes += count;
                    }
                    else
                    {
                        PacketStat stat2 = new PacketStat {
                            msgType = key
                        };
                        stat2.count++;
                        stat2.bytes += count;
                        this.m_PacketStats[key] = stat2;
                    }
                }
                else
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError(string.Concat(new object[] { "Unknown message ID ", key, " connId:", this.connectionId }));
                    }
                    break;
                }
            }
        }

        public virtual void Initialize(string networkAddress, int networkHostId, int networkConnectionId, HostTopology hostTopology)
        {
            this.m_Writer = new NetworkWriter();
            this.address = networkAddress;
            this.hostId = networkHostId;
            this.connectionId = networkConnectionId;
            int channelCount = hostTopology.DefaultConfig.ChannelCount;
            int packetSize = hostTopology.DefaultConfig.PacketSize;
            this.m_Channels = new ChannelBuffer[channelCount];
            for (int i = 0; i < channelCount; i++)
            {
                ChannelQOS lqos = hostTopology.DefaultConfig.Channels[i];
                this.m_Channels[i] = new ChannelBuffer(this, packetSize, (byte) i, IsReliableQoS(lqos.QOS));
            }
        }

        public bool InvokeHandler(NetworkMessage netMsg)
        {
            if (this.m_MessageHandlersDict.ContainsKey(netMsg.msgType))
            {
                NetworkMessageDelegate delegate2 = this.m_MessageHandlersDict[netMsg.msgType];
                delegate2(netMsg);
                return true;
            }
            return false;
        }

        public bool InvokeHandler(short msgType, NetworkReader reader, int channelId)
        {
            if (!this.m_MessageHandlersDict.ContainsKey(msgType))
            {
                return false;
            }
            this.m_MessageInfo.msgType = msgType;
            this.m_MessageInfo.conn = this;
            this.m_MessageInfo.reader = reader;
            this.m_MessageInfo.channelId = channelId;
            NetworkMessageDelegate delegate2 = this.m_MessageHandlersDict[msgType];
            if (delegate2 == null)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("NetworkConnection InvokeHandler no handler for " + msgType);
                }
                return false;
            }
            delegate2(this.m_MessageInfo);
            return true;
        }

        public bool InvokeHandlerNoData(short msgType)
        {
            return this.InvokeHandler(msgType, null, 0);
        }

        private static bool IsReliableQoS(QosType qos)
        {
            return ((((qos == QosType.Reliable) || (qos == QosType.ReliableFragmented)) || (qos == QosType.ReliableSequenced)) || (qos == QosType.ReliableStateUpdate));
        }

        private void LogSend(byte[] bytes)
        {
            NetworkReader reader = new NetworkReader(bytes);
            ushort num = reader.ReadUInt16();
            ushort num2 = reader.ReadUInt16();
            StringBuilder builder = new StringBuilder();
            for (int i = 4; i < (4 + num); i++)
            {
                builder.AppendFormat("{0:X2}", bytes[i]);
                if (i > 150)
                {
                    break;
                }
            }
            Debug.Log(string.Concat(new object[] { "ConnectionSend con:", this.connectionId, " bytes:", num, " msgId:", num2, " ", builder }));
        }

        public void RegisterHandler(short msgType, NetworkMessageDelegate handler)
        {
            this.m_MessageHandlers.RegisterHandler(msgType, handler);
        }

        internal void RemoveFromVisList(NetworkIdentity uv, bool isDestroyed)
        {
            this.m_VisList.Remove(uv);
            if (!isDestroyed)
            {
                NetworkServer.HideForConnection(uv, this);
            }
        }

        internal void RemoveObservers()
        {
            foreach (NetworkIdentity identity in this.m_VisList)
            {
                identity.RemoveObserverInternal(this);
            }
            this.m_VisList.Clear();
        }

        internal void RemoveOwnedObject(NetworkIdentity obj)
        {
            if (this.m_ClientOwnedObjects != null)
            {
                this.m_ClientOwnedObjects.Remove(obj.netId);
            }
        }

        internal void RemovePlayerController(short playerControllerId)
        {
            for (int i = this.m_PlayerControllers.Count; i >= 0; i--)
            {
                if ((playerControllerId == i) && (playerControllerId == this.m_PlayerControllers[i].playerControllerId))
                {
                    this.m_PlayerControllers[i] = new PlayerController();
                    return;
                }
            }
            if (LogFilter.logError)
            {
                Debug.LogError("RemovePlayer player at playerControllerId " + playerControllerId + " not found");
            }
        }

        public void ResetStats()
        {
            for (short i = 0; i < s_MaxPacketStats; i = (short) (i + 1))
            {
                if (this.m_PacketStats.ContainsKey(i))
                {
                    PacketStat stat = this.m_PacketStats[i];
                    stat.count = 0;
                    stat.bytes = 0;
                    NetworkTransport.SetPacketStat(0, i, 0, 0);
                    NetworkTransport.SetPacketStat(1, i, 0, 0);
                }
            }
        }

        public virtual bool Send(short msgType, MessageBase msg)
        {
            return this.SendByChannel(msgType, msg, 0);
        }

        public virtual bool SendByChannel(short msgType, MessageBase msg, int channelId)
        {
            this.m_Writer.StartMessage(msgType);
            msg.Serialize(this.m_Writer);
            this.m_Writer.FinishMessage();
            return this.SendWriter(this.m_Writer, channelId);
        }

        public virtual bool SendBytes(byte[] bytes, int numBytes, int channelId)
        {
            if (this.logNetworkMessages)
            {
                this.LogSend(bytes);
            }
            return (this.CheckChannel(channelId) && this.m_Channels[channelId].SendBytes(bytes, numBytes));
        }

        public virtual bool SendUnreliable(short msgType, MessageBase msg)
        {
            return this.SendByChannel(msgType, msg, 1);
        }

        public virtual bool SendWriter(NetworkWriter writer, int channelId)
        {
            if (this.logNetworkMessages)
            {
                this.LogSend(writer.ToArray());
            }
            return (this.CheckChannel(channelId) && this.m_Channels[channelId].SendWriter(writer));
        }

        public bool SetChannelOption(int channelId, ChannelOption option, int value)
        {
            if (this.m_Channels == null)
            {
                return false;
            }
            return (((channelId >= 0) && (channelId < this.m_Channels.Length)) && this.m_Channels[channelId].SetOption(option, value));
        }

        internal void SetHandlers(NetworkMessageHandlers handlers)
        {
            this.m_MessageHandlers = handlers;
            this.m_MessageHandlersDict = handlers.GetHandlers();
        }

        public void SetMaxDelay(float seconds)
        {
            if (this.m_Channels != null)
            {
                foreach (ChannelBuffer buffer in this.m_Channels)
                {
                    buffer.maxDelay = seconds;
                }
            }
        }

        internal void SetPlayerController(PlayerController player)
        {
            while (player.playerControllerId >= this.m_PlayerControllers.Count)
            {
                this.m_PlayerControllers.Add(new PlayerController());
            }
            this.m_PlayerControllers[player.playerControllerId] = player;
        }

        public override string ToString()
        {
            object[] args = new object[] { this.hostId, this.connectionId, this.isReady, (this.m_Channels == null) ? 0 : this.m_Channels.Length };
            return string.Format("hostId: {0} connectionId: {1} isReady: {2} channel count: {3}", args);
        }

        public virtual void TransportRecieve(byte[] bytes, int numBytes, int channelId)
        {
            this.HandleBytes(bytes, numBytes, channelId);
        }

        public virtual bool TransportSend(byte[] bytes, int numBytes, int channelId, out byte error)
        {
            return NetworkTransport.Send(this.hostId, this.connectionId, channelId, bytes, numBytes, out error);
        }

        public void UnregisterHandler(short msgType)
        {
            this.m_MessageHandlers.UnregisterHandler(msgType);
        }

        public HashSet<NetworkInstanceId> clientOwnedObjects
        {
            get
            {
                return this.m_ClientOwnedObjects;
            }
        }

        internal Dictionary<short, PacketStat> packetStats
        {
            get
            {
                return this.m_PacketStats;
            }
        }

        public List<PlayerController> playerControllers
        {
            get
            {
                return this.m_PlayerControllers;
            }
        }

        internal HashSet<NetworkIdentity> visList
        {
            get
            {
                return this.m_VisList;
            }
        }

        public class PacketStat
        {
            public int bytes;
            public int count;
            public short msgType;

            public override string ToString()
            {
                object[] objArray1 = new object[] { MsgType.MsgTypeToString(this.msgType), ": count=", this.count, " bytes=", this.bytes };
                return string.Concat(objArray1);
            }
        }
    }
}

