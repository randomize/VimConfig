namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Networking.Match;
    using UnityEngine.Networking.NetworkSystem;

    public class NetworkClient
    {
        private const int k_MaxEventsPerFrame = 500;
        protected ConnectState m_AsyncConnect;
        private int m_ClientConnectionId = -1;
        private int m_ClientId = -1;
        protected NetworkConnection m_Connection;
        private HostTopology m_HostTopology;
        private NetworkMessageHandlers m_MessageHandlers = new NetworkMessageHandlers();
        private byte[] m_MsgBuffer;
        private NetworkReader m_MsgReader;
        private System.Type m_NetworkConnectionClass = typeof(NetworkConnection);
        private float m_PacketLoss;
        private PeerInfoMessage[] m_Peers;
        private EndPoint m_RemoteEndPoint;
        private string m_RequestedServerHost = string.Empty;
        private string m_ServerIp = string.Empty;
        private int m_ServerPort;
        private int m_SimulatedLatency;
        private int m_StatResetTime;
        private bool m_UseSimulator;
        private static List<NetworkClient> s_Clients = new List<NetworkClient>();
        private static CRCMessage s_CRCMessage = new CRCMessage();
        private static bool s_IsActive;
        private static PeerListMessage s_PeerListMessage = new PeerListMessage();

        public NetworkClient()
        {
            if (LogFilter.logDev)
            {
                Debug.Log("Client created version " + UnityEngine.Networking.Version.Current);
            }
            this.m_MsgBuffer = new byte[0xc000];
            this.m_MsgReader = new NetworkReader(this.m_MsgBuffer);
            AddClient(this);
        }

        internal static void AddClient(NetworkClient client)
        {
            s_Clients.Add(client);
        }

        public bool Configure(HostTopology topology)
        {
            this.m_HostTopology = topology;
            return true;
        }

        public bool Configure(ConnectionConfig config, int maxConnections)
        {
            HostTopology topology = new HostTopology(config, maxConnections);
            return this.Configure(topology);
        }

        public void Connect(EndPoint secureTunnelEndPoint)
        {
            this.PrepareForConnect();
            if (LogFilter.logDebug)
            {
                Debug.Log("Client Connect to remoteSockAddr");
            }
            if (secureTunnelEndPoint == null)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("Connect failed: null endpoint passed in");
                }
                this.m_AsyncConnect = ConnectState.Failed;
            }
            else if ((secureTunnelEndPoint.AddressFamily != AddressFamily.InterNetwork) && (secureTunnelEndPoint.AddressFamily != AddressFamily.InterNetworkV6))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("Connect failed: Endpoint AddressFamily must be either InterNetwork or InterNetworkV6");
                }
                this.m_AsyncConnect = ConnectState.Failed;
            }
            else
            {
                string fullName = secureTunnelEndPoint.GetType().FullName;
                if (fullName == "System.Net.IPEndPoint")
                {
                    IPEndPoint point = (IPEndPoint) secureTunnelEndPoint;
                    this.Connect(point.Address.ToString(), point.Port);
                }
                else if (fullName != "UnityEngine.XboxOne.XboxOneEndPoint")
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError("Connect failed: invalid Endpoint (not IPEndPoint or XboxOneEndPoint)");
                    }
                    this.m_AsyncConnect = ConnectState.Failed;
                }
                else
                {
                    byte error = 0;
                    this.m_RemoteEndPoint = secureTunnelEndPoint;
                    this.m_AsyncConnect = ConnectState.Connecting;
                    try
                    {
                        this.m_ClientConnectionId = NetworkTransport.ConnectEndPoint(this.m_ClientId, this.m_RemoteEndPoint, 0, out error);
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError("Connect failed: Exception when trying to connect to EndPoint: " + exception);
                    }
                    if ((this.m_ClientConnectionId == 0) && LogFilter.logError)
                    {
                        Debug.LogError("Connect failed: Unable to connect to EndPoint (" + error + ")");
                    }
                    this.m_Connection = (NetworkConnection) Activator.CreateInstance(this.m_NetworkConnectionClass);
                    this.m_Connection.SetHandlers(this.m_MessageHandlers);
                    this.m_Connection.Initialize(this.m_ServerIp, this.m_ClientId, this.m_ClientConnectionId, this.m_HostTopology);
                }
            }
        }

        public void Connect(MatchInfo matchInfo)
        {
            this.PrepareForConnect();
            this.ConnectWithRelay(matchInfo);
        }

        public void Connect(string serverIp, int serverPort)
        {
            this.PrepareForConnect();
            if (LogFilter.logDebug)
            {
                Debug.Log(string.Concat(new object[] { "Client Connect: ", serverIp, ":", serverPort }));
            }
            string hostNameOrAddress = serverIp;
            this.m_ServerPort = serverPort;
            if (serverIp.Equals("127.0.0.1") || serverIp.Equals("localhost"))
            {
                this.m_ServerIp = "127.0.0.1";
                this.m_AsyncConnect = ConnectState.Resolved;
            }
            else if ((serverIp.IndexOf(":") != -1) && IsValidIpV6(serverIp))
            {
                this.m_ServerIp = serverIp;
                this.m_AsyncConnect = ConnectState.Resolved;
            }
            else
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log("Async DNS START:" + hostNameOrAddress);
                }
                this.m_RequestedServerHost = hostNameOrAddress;
                this.m_AsyncConnect = ConnectState.Resolving;
                Dns.BeginGetHostAddresses(hostNameOrAddress, new AsyncCallback(NetworkClient.GetHostAddressesCallback), this);
            }
        }

        private void ConnectWithRelay(MatchInfo info)
        {
            byte num;
            this.m_AsyncConnect = ConnectState.Connecting;
            this.Update();
            this.m_ClientConnectionId = NetworkTransport.ConnectToNetworkPeer(this.m_ClientId, info.address, info.port, 0, 0, info.networkId, Utility.GetSourceID(), info.nodeId, out num);
            this.m_Connection = (NetworkConnection) Activator.CreateInstance(this.m_NetworkConnectionClass);
            this.m_Connection.SetHandlers(this.m_MessageHandlers);
            this.m_Connection.Initialize(info.address, this.m_ClientId, this.m_ClientConnectionId, this.m_HostTopology);
            if (num != 0)
            {
                Debug.LogError("ConnectToNetworkPeer Error: " + num);
            }
        }

        public void ConnectWithSimulator(string serverIp, int serverPort, int latency, float packetLoss)
        {
            this.m_UseSimulator = true;
            this.m_SimulatedLatency = latency;
            this.m_PacketLoss = packetLoss;
            this.Connect(serverIp, serverPort);
        }

        internal void ContinueConnect()
        {
            byte num;
            if (this.m_UseSimulator)
            {
                int outMinDelay = this.m_SimulatedLatency / 3;
                if (outMinDelay < 1)
                {
                    outMinDelay = 1;
                }
                if (LogFilter.logDebug)
                {
                    Debug.Log(string.Concat(new object[] { "Connect Using Simulator ", this.m_SimulatedLatency / 3, "/", this.m_SimulatedLatency }));
                }
                ConnectionSimulatorConfig conf = new ConnectionSimulatorConfig(outMinDelay, this.m_SimulatedLatency, outMinDelay, this.m_SimulatedLatency, this.m_PacketLoss);
                this.m_ClientConnectionId = NetworkTransport.ConnectWithSimulator(this.m_ClientId, this.m_ServerIp, this.m_ServerPort, 0, out num, conf);
            }
            else
            {
                this.m_ClientConnectionId = NetworkTransport.Connect(this.m_ClientId, this.m_ServerIp, this.m_ServerPort, 0, out num);
            }
            this.m_Connection = (NetworkConnection) Activator.CreateInstance(this.m_NetworkConnectionClass);
            this.m_Connection.SetHandlers(this.m_MessageHandlers);
            this.m_Connection.Initialize(this.m_ServerIp, this.m_ClientId, this.m_ClientConnectionId, this.m_HostTopology);
        }

        public virtual void Disconnect()
        {
            this.m_AsyncConnect = ConnectState.Disconnected;
            ClientScene.HandleClientDisconnect(this.m_Connection);
            if (this.m_Connection != null)
            {
                this.m_Connection.Disconnect();
                this.m_Connection.Dispose();
                this.m_Connection = null;
            }
        }

        private void GenerateConnectError(int error)
        {
            if (LogFilter.logError)
            {
                Debug.LogError("UNet Client Error Connect Error: " + error);
            }
            this.GenerateError(error);
        }

        private void GenerateDataError(int error)
        {
            NetworkError error2 = (NetworkError) error;
            if (LogFilter.logError)
            {
                Debug.LogError("UNet Client Data Error: " + error2);
            }
            this.GenerateError(error);
        }

        private void GenerateDisconnectError(int error)
        {
            NetworkError error2 = (NetworkError) error;
            if (LogFilter.logError)
            {
                Debug.LogError("UNet Client Disconnect Error: " + error2);
            }
            this.GenerateError(error);
        }

        private void GenerateError(int error)
        {
            NetworkMessageDelegate handler = this.m_MessageHandlers.GetHandler(0x22);
            if (handler == null)
            {
                handler = this.m_MessageHandlers.GetHandler(0x22);
            }
            if (handler != null)
            {
                ErrorMessage message = new ErrorMessage {
                    errorCode = error
                };
                byte[] buffer = new byte[200];
                NetworkWriter writer = new NetworkWriter(buffer);
                message.Serialize(writer);
                NetworkReader reader = new NetworkReader(buffer);
                NetworkMessage netMsg = new NetworkMessage {
                    msgType = 0x22,
                    reader = reader,
                    conn = this.m_Connection,
                    channelId = 0
                };
                handler(netMsg);
            }
        }

        public Dictionary<short, NetworkConnection.PacketStat> GetConnectionStats()
        {
            if (this.m_Connection == null)
            {
                return null;
            }
            return this.m_Connection.packetStats;
        }

        internal static void GetHostAddressesCallback(IAsyncResult ar)
        {
            try
            {
                IPAddress[] addressArray = Dns.EndGetHostAddresses(ar);
                NetworkClient asyncState = (NetworkClient) ar.AsyncState;
                if (addressArray.Length == 0)
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError("DNS lookup failed for:" + asyncState.m_RequestedServerHost);
                    }
                    asyncState.m_AsyncConnect = ConnectState.Failed;
                }
                else
                {
                    asyncState.m_ServerIp = addressArray[0].ToString();
                    asyncState.m_AsyncConnect = ConnectState.Resolved;
                    if (LogFilter.logDebug)
                    {
                        Debug.Log("Async DNS Result:" + asyncState.m_ServerIp + " for " + asyncState.m_RequestedServerHost + ": " + asyncState.m_ServerIp);
                    }
                }
            }
            catch (SocketException exception)
            {
                NetworkClient client2 = (NetworkClient) ar.AsyncState;
                if (LogFilter.logError)
                {
                    Debug.LogError("DNS resolution failed: " + exception.ErrorCode);
                }
                if (LogFilter.logDebug)
                {
                    Debug.Log("Exception:" + exception);
                }
                client2.m_AsyncConnect = ConnectState.Failed;
            }
        }

        public int GetRTT()
        {
            byte num;
            if (this.m_ClientId == -1)
            {
                return 0;
            }
            return NetworkTransport.GetCurrentRtt(this.m_ClientId, this.m_ClientConnectionId, out num);
        }

        public void GetStatsIn(out int numMsgs, out int numBytes)
        {
            numMsgs = 0;
            numBytes = 0;
            if (this.m_Connection != null)
            {
                this.m_Connection.GetStatsIn(out numMsgs, out numBytes);
            }
        }

        public void GetStatsOut(out int numMsgs, out int numBufferedMsgs, out int numBytes, out int lastBufferedPerSecond)
        {
            numMsgs = 0;
            numBufferedMsgs = 0;
            numBytes = 0;
            lastBufferedPerSecond = 0;
            if (this.m_Connection != null)
            {
                this.m_Connection.GetStatsOut(out numMsgs, out numBufferedMsgs, out numBytes, out lastBufferedPerSecond);
            }
        }

        public static Dictionary<short, NetworkConnection.PacketStat> GetTotalConnectionStats()
        {
            Dictionary<short, NetworkConnection.PacketStat> dictionary = new Dictionary<short, NetworkConnection.PacketStat>();
            foreach (NetworkClient client in s_Clients)
            {
                Dictionary<short, NetworkConnection.PacketStat> connectionStats = client.GetConnectionStats();
                foreach (short num in connectionStats.Keys)
                {
                    if (dictionary.ContainsKey(num))
                    {
                        NetworkConnection.PacketStat stat = dictionary[num];
                        stat.count += connectionStats[num].count;
                        stat.bytes += connectionStats[num].bytes;
                        dictionary[num] = stat;
                    }
                    else
                    {
                        dictionary[num] = connectionStats[num];
                    }
                }
            }
            return dictionary;
        }

        private static bool IsValidIpV6(string address)
        {
            foreach (char ch in address)
            {
                if ((((ch != ':') && ((ch < '0') || (ch > '9'))) && ((ch < 'a') || (ch > 'f'))) && ((ch < 'A') || (ch > 'F')))
                {
                    return false;
                }
            }
            return true;
        }

        private void OnCRC(NetworkMessage netMsg)
        {
            netMsg.ReadMessage<CRCMessage>(s_CRCMessage);
            NetworkCRC.Validate(s_CRCMessage.scripts, this.numChannels);
        }

        private void OnPeerInfo(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("OnPeerInfo");
            }
            netMsg.ReadMessage<PeerListMessage>(s_PeerListMessage);
            this.m_Peers = s_PeerListMessage.peers;
        }

        private void PrepareForConnect()
        {
            SetActive(true);
            this.RegisterSystemHandlers(false);
            if (this.m_HostTopology == null)
            {
                ConnectionConfig defaultConfig = new ConnectionConfig();
                defaultConfig.AddChannel(QosType.Reliable);
                defaultConfig.AddChannel(QosType.Unreliable);
                this.m_HostTopology = new HostTopology(defaultConfig, 8);
            }
            if (this.m_UseSimulator)
            {
                int minTimeout = (this.m_SimulatedLatency / 3) - 1;
                if (minTimeout < 1)
                {
                    minTimeout = 1;
                }
                int maxTimeout = this.m_SimulatedLatency * 3;
                if (LogFilter.logDebug)
                {
                    Debug.Log(string.Concat(new object[] { "AddHost Using Simulator ", minTimeout, "/", maxTimeout }));
                }
                this.m_ClientId = NetworkTransport.AddHostWithSimulator(this.m_HostTopology, minTimeout, maxTimeout, 0);
            }
            else
            {
                this.m_ClientId = NetworkTransport.AddHost(this.m_HostTopology, 0);
            }
        }

        public void RegisterHandler(short msgType, NetworkMessageDelegate handler)
        {
            this.m_MessageHandlers.RegisterHandler(msgType, handler);
        }

        public void RegisterHandlerSafe(short msgType, NetworkMessageDelegate handler)
        {
            this.m_MessageHandlers.RegisterHandlerSafe(msgType, handler);
        }

        internal void RegisterSystemHandlers(bool localClient)
        {
            this.RegisterHandlerSafe(11, new NetworkMessageDelegate(this.OnPeerInfo));
            ClientScene.RegisterSystemHandlers(this, localClient);
            this.RegisterHandlerSafe(14, new NetworkMessageDelegate(this.OnCRC));
        }

        internal static void RemoveClient(NetworkClient client)
        {
            s_Clients.Remove(client);
        }

        public void ResetConnectionStats()
        {
            if (this.m_Connection != null)
            {
                this.m_Connection.ResetStats();
            }
        }

        public bool Send(short msgType, MessageBase msg)
        {
            if (this.m_Connection != null)
            {
                if (this.m_AsyncConnect != ConnectState.Connected)
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError("NetworkClient Send when not connected to a server");
                    }
                    return false;
                }
                NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Outgoing, 0, msgType.ToString() + ":" + msg.GetType().Name, 1);
                return this.m_Connection.Send(msgType, msg);
            }
            if (LogFilter.logError)
            {
                Debug.LogError("NetworkClient Send with no connection");
            }
            return false;
        }

        public bool SendByChannel(short msgType, MessageBase msg, int channelId)
        {
            NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Outgoing, 0, msgType.ToString() + ":" + msg.GetType().Name, 1);
            if (this.m_Connection != null)
            {
                if (this.m_AsyncConnect == ConnectState.Connected)
                {
                    return this.m_Connection.SendByChannel(msgType, msg, channelId);
                }
                if (LogFilter.logError)
                {
                    Debug.LogError("NetworkClient SendByChannel when not connected to a server");
                }
                return false;
            }
            if (LogFilter.logError)
            {
                Debug.LogError("NetworkClient SendByChannel with no connection");
            }
            return false;
        }

        public bool SendBytes(byte[] data, int numBytes, int channelId)
        {
            if (this.m_Connection != null)
            {
                if (this.m_AsyncConnect == ConnectState.Connected)
                {
                    return this.m_Connection.SendBytes(data, numBytes, channelId);
                }
                if (LogFilter.logError)
                {
                    Debug.LogError("NetworkClient SendBytes when not connected to a server");
                }
                return false;
            }
            if (LogFilter.logError)
            {
                Debug.LogError("NetworkClient SendBytes with no connection");
            }
            return false;
        }

        public bool SendUnreliable(short msgType, MessageBase msg)
        {
            if (this.m_Connection != null)
            {
                if (this.m_AsyncConnect != ConnectState.Connected)
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError("NetworkClient SendUnreliable when not connected to a server");
                    }
                    return false;
                }
                NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Outgoing, 0, msgType.ToString() + ":" + msg.GetType().Name, 1);
                return this.m_Connection.SendUnreliable(msgType, msg);
            }
            if (LogFilter.logError)
            {
                Debug.LogError("NetworkClient SendUnreliable with no connection");
            }
            return false;
        }

        public bool SendWriter(NetworkWriter writer, int channelId)
        {
            if (this.m_Connection != null)
            {
                if (this.m_AsyncConnect == ConnectState.Connected)
                {
                    return this.m_Connection.SendWriter(writer, channelId);
                }
                if (LogFilter.logError)
                {
                    Debug.LogError("NetworkClient SendWriter when not connected to a server");
                }
                return false;
            }
            if (LogFilter.logError)
            {
                Debug.LogError("NetworkClient SendWriter with no connection");
            }
            return false;
        }

        internal static void SetActive(bool state)
        {
            if (!s_IsActive && state)
            {
                NetworkTransport.Init();
            }
            s_IsActive = state;
        }

        internal void SetHandlers(NetworkConnection conn)
        {
            conn.SetHandlers(this.m_MessageHandlers);
        }

        public void SetMaxDelay(float seconds)
        {
            if (this.m_Connection == null)
            {
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning("SetMaxDelay failed, not connected.");
                }
            }
            else
            {
                this.m_Connection.SetMaxDelay(seconds);
            }
        }

        public void SetNetworkConnectionClass<T>() where T: NetworkConnection
        {
            this.m_NetworkConnectionClass = typeof(T);
        }

        public void Shutdown()
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("Shutting down client " + this.m_ClientId);
            }
            this.m_ClientId = -1;
            RemoveClient(this);
            if (s_Clients.Count == 0)
            {
                SetActive(false);
            }
        }

        public static void ShutdownAll()
        {
            while (s_Clients.Count != 0)
            {
                s_Clients[0].Shutdown();
            }
            s_Clients = new List<NetworkClient>();
            s_IsActive = false;
            ClientScene.Shutdown();
            NetworkDetailStats.ResetAll();
        }

        public void UnregisterHandler(short msgType)
        {
            this.m_MessageHandlers.UnregisterHandler(msgType);
        }

        internal virtual void Update()
        {
            int num;
            int num2;
            int num3;
            byte num4;
            int num5;
            if (this.m_ClientId == -1)
            {
                return;
            }
            switch (this.m_AsyncConnect)
            {
                case ConnectState.None:
                case ConnectState.Resolving:
                case ConnectState.Disconnected:
                    return;

                case ConnectState.Resolved:
                    this.m_AsyncConnect = ConnectState.Connecting;
                    this.ContinueConnect();
                    return;

                case ConnectState.Failed:
                    this.GenerateConnectError(11);
                    this.m_AsyncConnect = ConnectState.Disconnected;
                    return;

                default:
                    if ((this.m_Connection != null) && (((int) Time.time) != this.m_StatResetTime))
                    {
                        this.m_Connection.ResetStats();
                        this.m_StatResetTime = (int) Time.time;
                    }
                    break;
            }
        Label_0094:
            num5 = 0;
            NetworkEventType type = NetworkTransport.ReceiveFromHost(this.m_ClientId, out num, out num2, this.m_MsgBuffer, (ushort) this.m_MsgBuffer.Length, out num3, out num4);
            if ((type != NetworkEventType.Nothing) && LogFilter.logDev)
            {
                Debug.Log(string.Concat(new object[] { "Client event: host=", this.m_ClientId, " event=", type, " error=", num4 }));
            }
            switch (type)
            {
                case NetworkEventType.DataEvent:
                    if (num4 == 0)
                    {
                        NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Incoming, 0x1d, "msg", 1);
                        this.m_MsgReader.SeekZero();
                        this.m_Connection.TransportRecieve(this.m_MsgBuffer, num3, num2);
                        break;
                    }
                    this.GenerateDataError(num4);
                    return;

                case NetworkEventType.ConnectEvent:
                    if (LogFilter.logDebug)
                    {
                        Debug.Log("Client connected");
                    }
                    if (num4 != 0)
                    {
                        this.GenerateConnectError(num4);
                        return;
                    }
                    this.m_AsyncConnect = ConnectState.Connected;
                    this.m_Connection.InvokeHandlerNoData(0x20);
                    break;

                case NetworkEventType.DisconnectEvent:
                    if (LogFilter.logDebug)
                    {
                        Debug.Log("Client disconnected");
                    }
                    this.m_AsyncConnect = ConnectState.Disconnected;
                    if (num4 != 0)
                    {
                        this.GenerateDisconnectError(num4);
                    }
                    ClientScene.HandleClientDisconnect(this.m_Connection);
                    this.m_Connection.InvokeHandlerNoData(0x21);
                    break;

                case NetworkEventType.Nothing:
                    break;

                default:
                    if (LogFilter.logError)
                    {
                        Debug.LogError("Unknown network message type received: " + type);
                    }
                    break;
            }
            if (++num5 >= 500)
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log("MaxEventsPerFrame hit (" + 500 + ")");
                }
            }
            else if ((this.m_ClientId != -1) && (type != NetworkEventType.Nothing))
            {
                goto Label_0094;
            }
            if ((this.m_Connection != null) && (this.m_AsyncConnect == ConnectState.Connected))
            {
                this.m_Connection.FlushChannels();
            }
        }

        internal static void UpdateClients()
        {
            for (int i = 0; i < s_Clients.Count; i++)
            {
                if (s_Clients[i] != null)
                {
                    s_Clients[i].Update();
                }
                else
                {
                    s_Clients.RemoveAt(i);
                }
            }
        }

        public static bool active
        {
            get
            {
                return s_IsActive;
            }
        }

        public static List<NetworkClient> allClients
        {
            get
            {
                return s_Clients;
            }
        }

        public NetworkConnection connection
        {
            get
            {
                return this.m_Connection;
            }
        }

        public Dictionary<short, NetworkMessageDelegate> handlers
        {
            get
            {
                return this.m_MessageHandlers.GetHandlers();
            }
        }

        public HostTopology hostTopology
        {
            get
            {
                return this.m_HostTopology;
            }
        }

        public bool isConnected
        {
            get
            {
                return (this.m_AsyncConnect == ConnectState.Connected);
            }
        }

        public System.Type networkConnectionClass
        {
            get
            {
                return this.m_NetworkConnectionClass;
            }
        }

        public int numChannels
        {
            get
            {
                return this.m_HostTopology.DefaultConfig.ChannelCount;
            }
        }

        public PeerInfoMessage[] peers
        {
            get
            {
                return this.m_Peers;
            }
        }

        public string serverIp
        {
            get
            {
                return this.m_ServerIp;
            }
        }

        public int serverPort
        {
            get
            {
                return this.m_ServerPort;
            }
        }

        protected enum ConnectState
        {
            None,
            Resolving,
            Resolved,
            Connecting,
            Connected,
            Disconnected,
            Failed
        }
    }
}

