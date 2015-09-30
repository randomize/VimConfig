namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [AddComponentMenu("Network/NetworkDiscovery"), DisallowMultipleComponent]
    public class NetworkDiscovery : MonoBehaviour
    {
        private const int k_MaxBroadcastMsgSize = 0x400;
        [SerializeField]
        private string m_BroadcastData = "HELLO";
        [SerializeField]
        private int m_BroadcastInterval = 0x3e8;
        [SerializeField]
        private int m_BroadcastKey = 0x8ae;
        [SerializeField]
        private int m_BroadcastPort = 0xbaa1;
        private Dictionary<string, NetworkBroadcastResult> m_BroadcastsReceived;
        [SerializeField]
        private int m_BroadcastSubVersion = 1;
        [SerializeField]
        private int m_BroadcastVersion = 1;
        private HostTopology m_DefaultTopology;
        private int m_HostId = -1;
        private bool m_IsClient;
        private bool m_IsServer;
        private byte[] m_MsgInBuffer;
        private byte[] m_MsgOutBuffer;
        [SerializeField]
        private int m_OffsetX;
        [SerializeField]
        private int m_OffsetY;
        private bool m_Running;
        [SerializeField]
        private bool m_ShowGUI = true;
        [SerializeField]
        private bool m_UseNetworkManager = true;

        private static string BytesToString(byte[] bytes)
        {
            char[] dst = new char[bytes.Length / 2];
            Buffer.BlockCopy(bytes, 0, dst, 0, bytes.Length);
            return new string(dst);
        }

        public bool Initialize()
        {
            if (this.m_BroadcastData.Length >= 0x400)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("NetworkDiscovery Initialize - data too large. max is " + 0x400);
                }
                return false;
            }
            if (!NetworkTransport.IsStarted)
            {
                NetworkTransport.Init();
            }
            if (this.m_UseNetworkManager && (NetworkManager.singleton != null))
            {
                object[] objArray1 = new object[] { "NetworkManager:", NetworkManager.singleton.networkAddress, ":", NetworkManager.singleton.networkPort };
                this.m_BroadcastData = string.Concat(objArray1);
                if (LogFilter.logInfo)
                {
                    Debug.Log("NetwrokDiscovery set broadbast data to:" + this.m_BroadcastData);
                }
            }
            this.m_MsgOutBuffer = StringToBytes(this.m_BroadcastData);
            this.m_MsgInBuffer = new byte[0x400];
            this.m_BroadcastsReceived = new Dictionary<string, NetworkBroadcastResult>();
            ConnectionConfig defaultConfig = new ConnectionConfig();
            defaultConfig.AddChannel(QosType.Unreliable);
            this.m_DefaultTopology = new HostTopology(defaultConfig, 1);
            if (this.m_IsServer)
            {
                this.StartAsServer();
            }
            if (this.m_IsClient)
            {
                this.StartAsClient();
            }
            return true;
        }

        private void OnGUI()
        {
            if (this.m_ShowGUI)
            {
                int num = 10 + this.m_OffsetX;
                int num2 = 40 + this.m_OffsetY;
                if (this.m_MsgInBuffer == null)
                {
                    if (GUI.Button(new Rect((float) num, (float) num2, 200f, 20f), "Initialize Broadcast"))
                    {
                        this.Initialize();
                    }
                }
                else
                {
                    string str = string.Empty;
                    if (this.m_IsServer)
                    {
                        str = " (server)";
                    }
                    if (this.m_IsClient)
                    {
                        str = " (client)";
                    }
                    GUI.Label(new Rect((float) num, (float) num2, 200f, 20f), "initialized" + str);
                    num2 += 0x18;
                    if (this.m_Running)
                    {
                        if (GUI.Button(new Rect((float) num, (float) num2, 200f, 20f), "Stop"))
                        {
                            this.StopBroadcast();
                        }
                        num2 += 0x18;
                        if (this.m_BroadcastsReceived != null)
                        {
                            foreach (string str2 in this.m_BroadcastsReceived.Keys)
                            {
                                NetworkBroadcastResult result = this.m_BroadcastsReceived[str2];
                                if (GUI.Button(new Rect((float) num, (float) (num2 + 20), 200f, 20f), "Game at " + str2) && this.m_UseNetworkManager)
                                {
                                    char[] separator = new char[] { ':' };
                                    string[] strArray = BytesToString(result.broadcastData).Split(separator);
                                    if (((strArray.Length == 3) && (strArray[0] == "NetworkManager")) && ((NetworkManager.singleton != null) && (NetworkManager.singleton.client == null)))
                                    {
                                        NetworkManager.singleton.networkAddress = strArray[1];
                                        NetworkManager.singleton.networkPort = Convert.ToInt32(strArray[2]);
                                        NetworkManager.singleton.StartClient();
                                    }
                                }
                                num2 += 0x18;
                            }
                        }
                    }
                    else
                    {
                        if (GUI.Button(new Rect((float) num, (float) num2, 200f, 20f), "Start Broadcasting"))
                        {
                            this.StartAsServer();
                        }
                        num2 += 0x18;
                        if (GUI.Button(new Rect((float) num, (float) num2, 200f, 20f), "Listen for Broadcast"))
                        {
                            this.StartAsClient();
                        }
                        num2 += 0x18;
                    }
                }
            }
        }

        public virtual void OnReceivedBroadcast(string fromAddress, string data)
        {
        }

        public bool StartAsClient()
        {
            byte num;
            if ((this.m_HostId != -1) || this.m_Running)
            {
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning("NetworkDiscovery StartAsClient already started");
                }
                return false;
            }
            this.m_HostId = NetworkTransport.AddHost(this.m_DefaultTopology, this.m_BroadcastPort);
            if (this.m_HostId == -1)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("NetworkDiscovery StartAsClient - addHost failed");
                }
                return false;
            }
            NetworkTransport.SetBroadcastCredentials(this.m_HostId, this.m_BroadcastKey, this.m_BroadcastVersion, this.m_BroadcastSubVersion, out num);
            this.m_Running = true;
            this.m_IsClient = true;
            if (LogFilter.logDebug)
            {
                Debug.Log("StartAsClient Discovery listening");
            }
            return true;
        }

        public bool StartAsServer()
        {
            byte num;
            if ((this.m_HostId != -1) || this.m_Running)
            {
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning("NetworkDiscovery StartAsServer already started");
                }
                return false;
            }
            this.m_HostId = NetworkTransport.AddHost(this.m_DefaultTopology, 0);
            if (this.m_HostId == -1)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("NetworkDiscovery StartAsServer - addHost failed");
                }
                return false;
            }
            if (!NetworkTransport.StartBroadcastDiscovery(this.m_HostId, this.m_BroadcastPort, this.m_BroadcastKey, this.m_BroadcastVersion, this.m_BroadcastSubVersion, this.m_MsgOutBuffer, this.m_MsgOutBuffer.Length, this.m_BroadcastInterval, out num))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("NetworkDiscovery StartBroadcast failed err: " + num);
                }
                return false;
            }
            this.m_Running = true;
            this.m_IsServer = true;
            if (LogFilter.logDebug)
            {
                Debug.Log("StartAsServer Discovery broadcasting");
            }
            UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
            return true;
        }

        public void StopBroadcast()
        {
            if (this.m_HostId == -1)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("NetworkDiscovery StopBroadcast not initialized");
                }
            }
            else if (!this.m_Running)
            {
                Debug.LogWarning("NetworkDiscovery StopBroadcast not started");
            }
            else
            {
                if (this.m_IsServer)
                {
                    NetworkTransport.StopBroadcastDiscovery();
                }
                NetworkTransport.RemoveHost(this.m_HostId);
                this.m_HostId = -1;
                this.m_Running = false;
                this.m_IsServer = false;
                this.m_IsClient = false;
                this.m_MsgInBuffer = null;
                this.m_BroadcastsReceived = null;
                if (LogFilter.logDebug)
                {
                    Debug.Log("Stopped Discovery broadcasting");
                }
            }
        }

        private static byte[] StringToBytes(string str)
        {
            byte[] dst = new byte[str.Length * 2];
            Buffer.BlockCopy(str.ToCharArray(), 0, dst, 0, dst.Length);
            return dst;
        }

        private void Update()
        {
            if ((this.m_HostId != -1) && !this.m_IsServer)
            {
                NetworkEventType type;
                do
                {
                    int num;
                    int num2;
                    int num3;
                    byte num4;
                    type = NetworkTransport.ReceiveFromHost(this.m_HostId, out num, out num2, this.m_MsgInBuffer, 0x400, out num3, out num4);
                    if (type == NetworkEventType.BroadcastEvent)
                    {
                        string str;
                        int num5;
                        NetworkTransport.GetBroadcastConnectionMessage(this.m_HostId, this.m_MsgInBuffer, 0x400, out num3, out num4);
                        NetworkTransport.GetBroadcastConnectionInfo(this.m_HostId, out str, out num5, out num4);
                        NetworkBroadcastResult result = new NetworkBroadcastResult {
                            serverAddress = str,
                            broadcastData = new byte[num3]
                        };
                        Buffer.BlockCopy(this.m_MsgInBuffer, 0, result.broadcastData, 0, num3);
                        this.m_BroadcastsReceived[str] = result;
                        this.OnReceivedBroadcast(str, BytesToString(this.m_MsgInBuffer));
                    }
                }
                while (type != NetworkEventType.Nothing);
            }
        }

        public string broadcastData
        {
            get
            {
                return this.m_BroadcastData;
            }
            set
            {
                this.m_BroadcastData = value;
            }
        }

        public int broadcastInterval
        {
            get
            {
                return this.m_BroadcastInterval;
            }
            set
            {
                this.m_BroadcastInterval = value;
            }
        }

        public int broadcastKey
        {
            get
            {
                return this.m_BroadcastKey;
            }
            set
            {
                this.m_BroadcastKey = value;
            }
        }

        public int broadcastPort
        {
            get
            {
                return this.m_BroadcastPort;
            }
            set
            {
                this.m_BroadcastPort = value;
            }
        }

        public Dictionary<string, NetworkBroadcastResult> broadcastsReceived
        {
            get
            {
                return this.m_BroadcastsReceived;
            }
        }

        public int broadcastSubVersion
        {
            get
            {
                return this.m_BroadcastSubVersion;
            }
            set
            {
                this.m_BroadcastSubVersion = value;
            }
        }

        public int broadcastVersion
        {
            get
            {
                return this.m_BroadcastVersion;
            }
            set
            {
                this.m_BroadcastVersion = value;
            }
        }

        public int hostId
        {
            get
            {
                return this.m_HostId;
            }
            set
            {
                this.m_HostId = value;
            }
        }

        public bool isClient
        {
            get
            {
                return this.m_IsClient;
            }
            set
            {
                this.m_IsClient = value;
            }
        }

        public bool isServer
        {
            get
            {
                return this.m_IsServer;
            }
            set
            {
                this.m_IsServer = value;
            }
        }

        public int offsetX
        {
            get
            {
                return this.m_OffsetX;
            }
            set
            {
                this.m_OffsetX = value;
            }
        }

        public int offsetY
        {
            get
            {
                return this.m_OffsetY;
            }
            set
            {
                this.m_OffsetY = value;
            }
        }

        public bool running
        {
            get
            {
                return this.m_Running;
            }
            set
            {
                this.m_Running = value;
            }
        }

        public bool showGUI
        {
            get
            {
                return this.m_ShowGUI;
            }
            set
            {
                this.m_ShowGUI = value;
            }
        }

        public bool useNetworkManager
        {
            get
            {
                return this.m_UseNetworkManager;
            }
            set
            {
                this.m_UseNetworkManager = value;
            }
        }
    }
}

