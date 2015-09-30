namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using UnityEngine;
    using UnityEngine.Networking.Match;
    using UnityEngine.Networking.NetworkSystem;
    using UnityEngine.Networking.Types;

    [AddComponentMenu("Network/NetworkManager")]
    public class NetworkManager : MonoBehaviour
    {
        public NetworkClient client;
        public bool isNetworkActive;
        [SerializeField]
        private bool m_AutoCreatePlayer = true;
        [SerializeField]
        private List<QosType> m_Channels = new List<QosType>();
        [SerializeField]
        private ConnectionConfig m_ConnectionConfig;
        [SerializeField]
        private bool m_CustomConfig;
        [SerializeField]
        private bool m_DontDestroyOnLoad = true;
        private EndPoint m_EndPoint;
        [SerializeField]
        private LogFilter.FilterLevel m_LogLevel = LogFilter.FilterLevel.Info;
        [SerializeField]
        private string m_MatchHost = "mm.unet.unity3d.com";
        [SerializeField]
        private int m_MatchPort = 0x1bb;
        [SerializeField]
        private int m_MaxConnections = 4;
        [SerializeField]
        private float m_MaxDelay = 0.01f;
        [SerializeField]
        private string m_NetworkAddress = "localhost";
        [SerializeField]
        private int m_NetworkPort = 0x1e61;
        [SerializeField]
        private string m_OfflineScene = string.Empty;
        [SerializeField]
        private string m_OnlineScene = string.Empty;
        [SerializeField]
        private float m_PacketLossPercentage;
        [SerializeField]
        private GameObject m_PlayerPrefab;
        [SerializeField]
        private PlayerSpawnMethod m_PlayerSpawnMethod;
        [SerializeField]
        private bool m_RunInBackground = true;
        [SerializeField]
        private bool m_ScriptCRCCheck = true;
        [SerializeField]
        private bool m_SendPeerInfo;
        [SerializeField]
        private string m_ServerBindAddress = string.Empty;
        [SerializeField]
        private bool m_ServerBindToIP;
        [SerializeField]
        private int m_SimulatedLatency = 1;
        [SerializeField]
        private List<GameObject> m_SpawnPrefabs = new List<GameObject>();
        [SerializeField]
        private bool m_UseSimulator;
        public List<MatchDesc> matches;
        public MatchInfo matchInfo;
        public NetworkMatch matchMaker;
        public string matchName = "default";
        public uint matchSize = 4;
        public static string networkSceneName = string.Empty;
        private static AddPlayerMessage s_AddPlayerMessage = new AddPlayerMessage();
        private static string s_Address;
        private static NetworkConnection s_ClientReadyConnection;
        private static ErrorMessage s_ErrorMessage = new ErrorMessage();
        private static AsyncOperation s_LoadingSceneAsync;
        private static RemovePlayerMessage s_RemovePlayerMessage = new RemovePlayerMessage();
        private static int s_StartPositionIndex;
        private static List<Transform> s_StartPositions = new List<Transform>();
        public static NetworkManager singleton;

        private void Awake()
        {
            LogFilter.currentLogLevel = (int) this.m_LogLevel;
            if (this.m_DontDestroyOnLoad)
            {
                if (singleton != null)
                {
                    if (LogFilter.logWarn)
                    {
                        Debug.LogWarning("Multiple NetworkManagers detected in the scene. Only one NetworkManager can exist at a time. The duplicate NetworkManager will not be used.");
                    }
                    UnityEngine.Object.Destroy(base.gameObject);
                    return;
                }
                if (LogFilter.logDev)
                {
                    Debug.Log("NetworkManager created singleton (DontDestroyOnLoad)");
                }
                singleton = this;
                UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
            }
            else
            {
                if (LogFilter.logDev)
                {
                    Debug.Log("NetworkManager created singleton (ForScene)");
                }
                singleton = this;
            }
            if (this.m_NetworkAddress != string.Empty)
            {
                s_Address = this.m_NetworkAddress;
            }
            else if (s_Address != string.Empty)
            {
                this.m_NetworkAddress = s_Address;
            }
        }

        internal void ClientChangeScene(string newSceneName, bool forceReload)
        {
            if (string.IsNullOrEmpty(newSceneName))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("ClientChangeScene empty scene name");
                }
            }
            else
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log("ClientChangeScene newSceneName:" + newSceneName + " networkSceneName:" + networkSceneName);
                }
                if ((newSceneName != networkSceneName) || forceReload)
                {
                    s_LoadingSceneAsync = Application.LoadLevelAsync(newSceneName);
                    networkSceneName = newSceneName;
                }
            }
        }

        private NetworkClient ConnectLocalClient()
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkManager StartHost port:" + this.m_NetworkPort);
            }
            this.m_NetworkAddress = "localhost";
            this.client = ClientScene.ConnectLocalServer();
            this.RegisterClientMessages(this.client);
            return this.client;
        }

        private void FinishLoadScene()
        {
            if (this.client != null)
            {
                if (s_ClientReadyConnection != null)
                {
                    this.OnClientConnect(s_ClientReadyConnection);
                    s_ClientReadyConnection = null;
                }
            }
            else if (LogFilter.logDev)
            {
                Debug.Log("FinishLoadScene client is null");
            }
            if (NetworkServer.active)
            {
                NetworkServer.SpawnObjects();
                this.OnServerSceneChanged(networkSceneName);
            }
            if (this.IsClientConnected() && (this.client != null))
            {
                this.RegisterClientMessages(this.client);
                this.OnClientSceneChanged(this.client.connection);
            }
        }

        public Transform GetStartPosition()
        {
            if (s_StartPositions.Count > 0)
            {
                for (int i = s_StartPositions.Count - 1; i >= 0; i--)
                {
                    if (s_StartPositions[i] == null)
                    {
                        s_StartPositions.RemoveAt(i);
                    }
                }
            }
            if ((this.m_PlayerSpawnMethod == PlayerSpawnMethod.Random) && (s_StartPositions.Count > 0))
            {
                int num2 = UnityEngine.Random.Range(0, s_StartPositions.Count);
                return s_StartPositions[num2];
            }
            if ((this.m_PlayerSpawnMethod != PlayerSpawnMethod.RoundRobin) || (s_StartPositions.Count <= 0))
            {
                return null;
            }
            if (s_StartPositionIndex >= s_StartPositions.Count)
            {
                s_StartPositionIndex = 0;
            }
            Transform transform = s_StartPositions[s_StartPositionIndex];
            s_StartPositionIndex++;
            return transform;
        }

        public bool IsClientConnected()
        {
            return ((this.client != null) && this.client.isConnected);
        }

        public virtual void OnClientConnect(NetworkConnection conn)
        {
            if (string.IsNullOrEmpty(this.m_OnlineScene) || (this.m_OnlineScene == this.m_OfflineScene))
            {
                ClientScene.Ready(conn);
                if (this.m_AutoCreatePlayer)
                {
                    ClientScene.AddPlayer(0);
                }
            }
        }

        internal void OnClientConnectInternal(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkManager:OnClientConnectInternal");
            }
            netMsg.conn.SetMaxDelay(this.m_MaxDelay);
            if (string.IsNullOrEmpty(this.m_OnlineScene) || (this.m_OnlineScene == this.m_OfflineScene))
            {
                this.OnClientConnect(netMsg.conn);
            }
            else
            {
                s_ClientReadyConnection = netMsg.conn;
            }
        }

        public virtual void OnClientDisconnect(NetworkConnection conn)
        {
            this.StopClient();
        }

        internal void OnClientDisconnectInternal(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkManager:OnClientDisconnectInternal");
            }
            if (this.m_OfflineScene != string.Empty)
            {
                this.ClientChangeScene(this.m_OfflineScene, false);
            }
            this.OnClientDisconnect(netMsg.conn);
        }

        public virtual void OnClientError(NetworkConnection conn, int errorCode)
        {
        }

        internal void OnClientErrorInternal(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkManager:OnClientErrorInternal");
            }
            netMsg.ReadMessage<ErrorMessage>(s_ErrorMessage);
            this.OnClientError(netMsg.conn, s_ErrorMessage.errorCode);
        }

        public virtual void OnClientNotReady(NetworkConnection conn)
        {
        }

        internal void OnClientNotReadyMessageInternal(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkManager:OnClientNotReadyMessageInternal");
            }
            ClientScene.SetNotReady();
            this.OnClientNotReady(netMsg.conn);
        }

        public virtual void OnClientSceneChanged(NetworkConnection conn)
        {
            ClientScene.Ready(conn);
            if (this.m_AutoCreatePlayer)
            {
                bool flag = ClientScene.localPlayers.Count == 0;
                bool flag2 = false;
                foreach (PlayerController controller in ClientScene.localPlayers)
                {
                    if (controller.gameObject != null)
                    {
                        flag2 = true;
                        break;
                    }
                }
                if (!flag2)
                {
                    flag = true;
                }
                if (flag)
                {
                    ClientScene.AddPlayer(0);
                }
            }
        }

        internal void OnClientSceneInternal(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkManager:OnClientSceneInternal");
            }
            string newSceneName = netMsg.reader.ReadString();
            if (this.IsClientConnected() && !NetworkServer.active)
            {
                this.ClientChangeScene(newSceneName, true);
            }
        }

        private void OnDestroy()
        {
            if (LogFilter.logDev)
            {
                Debug.Log("NetworkManager destroyed");
            }
        }

        public virtual void OnMatchCreate(CreateMatchResponse matchInfo)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkManager OnMatchCreate " + matchInfo);
            }
            if (matchInfo.success)
            {
                Utility.SetAccessTokenForNetwork(matchInfo.networkId, new NetworkAccessToken(matchInfo.accessTokenString));
                this.StartHost(new MatchInfo(matchInfo));
            }
            else if (LogFilter.logError)
            {
                Debug.LogError("Create Failed:" + matchInfo);
            }
        }

        public void OnMatchJoined(JoinMatchResponse matchInfo)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkManager OnMatchJoined ");
            }
            if (matchInfo.success)
            {
                Utility.SetAccessTokenForNetwork(matchInfo.networkId, new NetworkAccessToken(matchInfo.accessTokenString));
                this.StartClient(new MatchInfo(matchInfo));
            }
            else if (LogFilter.logError)
            {
                Debug.LogError("Join Failed:" + matchInfo);
            }
        }

        public virtual void OnMatchList(ListMatchResponse matchList)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkManager OnMatchList ");
            }
            this.matches = matchList.matches;
        }

        public virtual void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            this.OnServerAddPlayerInternal(conn, playerControllerId);
        }

        public virtual void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
        {
            this.OnServerAddPlayerInternal(conn, playerControllerId);
        }

        private void OnServerAddPlayerInternal(NetworkConnection conn, short playerControllerId)
        {
            if (this.m_PlayerPrefab == null)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("The PlayerPrefab is empty on the NetworkManager. Please setup a PlayerPrefab object.");
                }
            }
            else if (this.m_PlayerPrefab.GetComponent<NetworkIdentity>() == null)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("The PlayerPrefab does not have a NetworkIdentity. Please add a NetworkIdentity to the player prefab.");
                }
            }
            else if (((playerControllerId < conn.playerControllers.Count) && conn.playerControllers[playerControllerId].IsValid) && (conn.playerControllers[playerControllerId].gameObject != null))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("There is already a player at that playerControllerId for this connections.");
                }
            }
            else
            {
                GameObject obj2;
                Transform startPosition = this.GetStartPosition();
                if (startPosition != null)
                {
                    obj2 = (GameObject) UnityEngine.Object.Instantiate(this.m_PlayerPrefab, startPosition.position, startPosition.rotation);
                }
                else
                {
                    obj2 = (GameObject) UnityEngine.Object.Instantiate(this.m_PlayerPrefab, Vector3.zero, Quaternion.identity);
                }
                NetworkServer.AddPlayerForConnection(conn, obj2, playerControllerId);
            }
        }

        internal void OnServerAddPlayerMessageInternal(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkManager:OnServerAddPlayerMessageInternal");
            }
            netMsg.ReadMessage<AddPlayerMessage>(s_AddPlayerMessage);
            if (s_AddPlayerMessage.msgSize != 0)
            {
                NetworkReader extraMessageReader = new NetworkReader(s_AddPlayerMessage.msgData);
                this.OnServerAddPlayer(netMsg.conn, s_AddPlayerMessage.playerControllerId, extraMessageReader);
            }
            else
            {
                this.OnServerAddPlayer(netMsg.conn, s_AddPlayerMessage.playerControllerId);
            }
        }

        public virtual void OnServerConnect(NetworkConnection conn)
        {
        }

        internal void OnServerConnectInternal(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkManager:OnServerConnectInternal");
            }
            netMsg.conn.SetMaxDelay(this.m_MaxDelay);
            if ((networkSceneName != string.Empty) && (networkSceneName != this.m_OfflineScene))
            {
                StringMessage msg = new StringMessage(networkSceneName);
                netMsg.conn.Send(0x27, msg);
            }
            this.OnServerConnect(netMsg.conn);
        }

        public virtual void OnServerDisconnect(NetworkConnection conn)
        {
            NetworkServer.DestroyPlayersForConnection(conn);
        }

        internal void OnServerDisconnectInternal(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkManager:OnServerDisconnectInternal");
            }
            this.OnServerDisconnect(netMsg.conn);
        }

        public virtual void OnServerError(NetworkConnection conn, int errorCode)
        {
        }

        internal void OnServerErrorInternal(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkManager:OnServerErrorInternal");
            }
            netMsg.ReadMessage<ErrorMessage>(s_ErrorMessage);
            this.OnServerError(netMsg.conn, s_ErrorMessage.errorCode);
        }

        public virtual void OnServerReady(NetworkConnection conn)
        {
            if ((conn.playerControllers.Count == 0) && LogFilter.logDebug)
            {
                Debug.Log("Ready with no player object");
            }
            NetworkServer.SetClientReady(conn);
        }

        internal void OnServerReadyMessageInternal(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkManager:OnServerReadyMessageInternal");
            }
            this.OnServerReady(netMsg.conn);
        }

        public virtual void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
        {
            if (player.gameObject != null)
            {
                NetworkServer.Destroy(player.gameObject);
            }
        }

        internal void OnServerRemovePlayerMessageInternal(NetworkMessage netMsg)
        {
            PlayerController controller;
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkManager:OnServerRemovePlayerMessageInternal");
            }
            netMsg.ReadMessage<RemovePlayerMessage>(s_RemovePlayerMessage);
            netMsg.conn.GetPlayerController(s_RemovePlayerMessage.playerControllerId, out controller);
            this.OnServerRemovePlayer(netMsg.conn, controller);
            netMsg.conn.RemovePlayerController(s_RemovePlayerMessage.playerControllerId);
        }

        public virtual void OnServerSceneChanged(string sceneName)
        {
        }

        public virtual void OnStartClient(NetworkClient client)
        {
        }

        public virtual void OnStartHost()
        {
        }

        public virtual void OnStartServer()
        {
        }

        public virtual void OnStopClient()
        {
        }

        public virtual void OnStopHost()
        {
        }

        public virtual void OnStopServer()
        {
        }

        private void OnValidate()
        {
            if (this.m_SimulatedLatency < 1)
            {
                this.m_SimulatedLatency = 1;
            }
            if (this.m_SimulatedLatency > 500)
            {
                this.m_SimulatedLatency = 500;
            }
            if (this.m_PacketLossPercentage < 0f)
            {
                this.m_PacketLossPercentage = 0f;
            }
            if (this.m_PacketLossPercentage > 99f)
            {
                this.m_PacketLossPercentage = 99f;
            }
            if (this.m_MaxConnections <= 0)
            {
                this.m_MaxConnections = 1;
            }
            if (this.m_MaxConnections > 0x7d00)
            {
                this.m_MaxConnections = 0x7d00;
            }
            if ((this.m_PlayerPrefab != null) && (this.m_PlayerPrefab.GetComponent<NetworkIdentity>() == null))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("NetworkManager - playerPrefab must have a NetworkIdentity.");
                }
                this.m_PlayerPrefab = null;
            }
        }

        internal void RegisterClientMessages(NetworkClient client)
        {
            client.RegisterHandler(0x20, new NetworkMessageDelegate(this.OnClientConnectInternal));
            client.RegisterHandler(0x21, new NetworkMessageDelegate(this.OnClientDisconnectInternal));
            client.RegisterHandler(0x24, new NetworkMessageDelegate(this.OnClientNotReadyMessageInternal));
            client.RegisterHandler(0x22, new NetworkMessageDelegate(this.OnClientErrorInternal));
            client.RegisterHandler(0x27, new NetworkMessageDelegate(this.OnClientSceneInternal));
            if (this.m_PlayerPrefab != null)
            {
                ClientScene.RegisterPrefab(this.m_PlayerPrefab);
            }
            foreach (GameObject obj2 in this.m_SpawnPrefabs)
            {
                if (obj2 != null)
                {
                    ClientScene.RegisterPrefab(obj2);
                }
            }
        }

        internal void RegisterServerMessages()
        {
            NetworkServer.RegisterHandler(0x20, new NetworkMessageDelegate(this.OnServerConnectInternal));
            NetworkServer.RegisterHandler(0x21, new NetworkMessageDelegate(this.OnServerDisconnectInternal));
            NetworkServer.RegisterHandler(0x23, new NetworkMessageDelegate(this.OnServerReadyMessageInternal));
            NetworkServer.RegisterHandler(0x25, new NetworkMessageDelegate(this.OnServerAddPlayerMessageInternal));
            NetworkServer.RegisterHandler(0x26, new NetworkMessageDelegate(this.OnServerRemovePlayerMessageInternal));
            NetworkServer.RegisterHandler(0x22, new NetworkMessageDelegate(this.OnServerErrorInternal));
        }

        public static void RegisterStartPosition(Transform start)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("RegisterStartPosition:" + start);
            }
            s_StartPositions.Add(start);
        }

        public virtual void ServerChangeScene(string newSceneName)
        {
            if (string.IsNullOrEmpty(newSceneName))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("ServerChangeScene empty scene name");
                }
            }
            else
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log("ServerChangeScene " + newSceneName);
                }
                NetworkServer.SetAllClientsNotReady();
                networkSceneName = newSceneName;
                s_LoadingSceneAsync = Application.LoadLevelAsync(newSceneName);
                StringMessage msg = new StringMessage(networkSceneName);
                NetworkServer.SendToAll(0x27, msg);
                s_StartPositionIndex = 0;
                s_StartPositions.Clear();
            }
        }

        public void SetMatchHost(string newHost, int port, bool https)
        {
            if (this.matchMaker == null)
            {
                this.matchMaker = base.gameObject.AddComponent<NetworkMatch>();
            }
            if ((newHost == "localhost") || (newHost == "127.0.0.1"))
            {
                newHost = Environment.MachineName;
            }
            string str = "http://";
            if (https)
            {
                str = "https://";
            }
            if (LogFilter.logDebug)
            {
                Debug.Log("SetMatchHost:" + newHost);
            }
            this.m_MatchHost = newHost;
            this.m_MatchPort = port;
            object[] objArray1 = new object[] { str, this.m_MatchHost, ":", this.m_MatchPort };
            this.matchMaker.baseUri = new Uri(string.Concat(objArray1));
        }

        public static void Shutdown()
        {
            if (singleton != null)
            {
                s_StartPositions.Clear();
                s_StartPositionIndex = 0;
                s_ClientReadyConnection = null;
                singleton.StopHost();
                singleton = null;
            }
        }

        public NetworkClient StartClient()
        {
            return this.StartClient(null, null);
        }

        public NetworkClient StartClient(MatchInfo matchInfo)
        {
            return this.StartClient(matchInfo, null);
        }

        public NetworkClient StartClient(MatchInfo info, ConnectionConfig config)
        {
            this.matchInfo = info;
            if (this.m_RunInBackground)
            {
                Application.runInBackground = true;
            }
            this.isNetworkActive = true;
            this.client = new NetworkClient();
            if (config != null)
            {
                this.client.Configure(config, 1);
            }
            else if (this.m_CustomConfig && (this.m_ConnectionConfig != null))
            {
                this.m_ConnectionConfig.Channels.Clear();
                foreach (QosType type in this.m_Channels)
                {
                    this.m_ConnectionConfig.AddChannel(type);
                }
                this.client.Configure(this.m_ConnectionConfig, this.m_MaxConnections);
            }
            this.RegisterClientMessages(this.client);
            if (this.matchInfo != null)
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log("NetworkManager StartClient match: " + this.matchInfo);
                }
                this.client.Connect(this.matchInfo);
            }
            else if (this.m_EndPoint != null)
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log("NetworkManager StartClient using provided SecureTunnel");
                }
                this.client.Connect(this.m_EndPoint);
            }
            else
            {
                if (string.IsNullOrEmpty(this.m_NetworkAddress))
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError("Must set the Network Address field in the manager");
                    }
                    return null;
                }
                if (LogFilter.logDebug)
                {
                    Debug.Log(string.Concat(new object[] { "NetworkManager StartClient address:", this.m_NetworkAddress, " port:", this.m_NetworkPort }));
                }
                if (this.m_UseSimulator)
                {
                    this.client.ConnectWithSimulator(this.m_NetworkAddress, this.m_NetworkPort, this.m_SimulatedLatency, this.m_PacketLossPercentage);
                }
                else
                {
                    this.client.Connect(this.m_NetworkAddress, this.m_NetworkPort);
                }
            }
            this.OnStartClient(this.client);
            s_Address = this.m_NetworkAddress;
            return this.client;
        }

        public virtual NetworkClient StartHost()
        {
            this.OnStartHost();
            if (this.StartServer())
            {
                NetworkClient client = this.ConnectLocalClient();
                this.OnServerConnect(client.connection);
                this.OnStartClient(client);
                return client;
            }
            return null;
        }

        public virtual NetworkClient StartHost(MatchInfo info)
        {
            this.OnStartHost();
            this.matchInfo = info;
            if (this.StartServer(info))
            {
                NetworkClient client = this.ConnectLocalClient();
                this.OnServerConnect(client.connection);
                this.OnStartClient(client);
                return client;
            }
            return null;
        }

        public virtual NetworkClient StartHost(ConnectionConfig config, int maxConnections)
        {
            this.OnStartHost();
            if (this.StartServer(config, maxConnections))
            {
                NetworkClient client = this.ConnectLocalClient();
                this.OnServerConnect(client.connection);
                this.OnStartClient(client);
                return client;
            }
            return null;
        }

        public void StartMatchMaker()
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkManager StartMatchMaker");
            }
            this.SetMatchHost(this.m_MatchHost, this.m_MatchPort, true);
        }

        public bool StartServer()
        {
            return this.StartServer(null);
        }

        public bool StartServer(MatchInfo info)
        {
            return this.StartServer(info, null, -1);
        }

        public bool StartServer(ConnectionConfig config, int maxConnections)
        {
            return this.StartServer(null, config, maxConnections);
        }

        private bool StartServer(MatchInfo info, ConnectionConfig config, int maxConnections)
        {
            this.OnStartServer();
            if (this.m_RunInBackground)
            {
                Application.runInBackground = true;
            }
            NetworkCRC.scriptCRCCheck = this.scriptCRCCheck;
            if ((this.m_CustomConfig && (this.m_ConnectionConfig != null)) && (config == null))
            {
                this.m_ConnectionConfig.Channels.Clear();
                foreach (QosType type in this.m_Channels)
                {
                    this.m_ConnectionConfig.AddChannel(type);
                }
                NetworkServer.Configure(this.m_ConnectionConfig, this.m_MaxConnections);
            }
            this.RegisterServerMessages();
            NetworkServer.sendPeerInfo = this.m_SendPeerInfo;
            if (config != null)
            {
                NetworkServer.Configure(config, maxConnections);
            }
            if (info != null)
            {
                if (!NetworkServer.Listen(info, this.m_NetworkPort))
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError("StartServer listen failed.");
                    }
                    return false;
                }
            }
            else if (this.m_ServerBindToIP && !string.IsNullOrEmpty(this.m_ServerBindAddress))
            {
                if (!NetworkServer.Listen(this.m_ServerBindAddress, this.m_NetworkPort))
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError("StartServer listen on " + this.m_ServerBindAddress + " failed.");
                    }
                    return false;
                }
            }
            else if (!NetworkServer.Listen(this.m_NetworkPort))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("StartServer listen failed.");
                }
                return false;
            }
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkManager StartServer port:" + this.m_NetworkPort);
            }
            this.isNetworkActive = true;
            if (((this.m_OnlineScene != string.Empty) && (this.m_OnlineScene != Application.loadedLevelName)) && (this.m_OnlineScene != this.m_OfflineScene))
            {
                this.ServerChangeScene(this.m_OnlineScene);
            }
            else
            {
                NetworkServer.SpawnObjects();
            }
            return true;
        }

        public void StopClient()
        {
            this.OnStopClient();
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkManager StopClient");
            }
            this.isNetworkActive = false;
            if (this.client != null)
            {
                this.client.Disconnect();
                this.client.Shutdown();
                this.client = null;
            }
            this.StopMatchMaker();
            ClientScene.DestroyAllClientObjects();
            if (this.m_OfflineScene != string.Empty)
            {
                this.ClientChangeScene(this.m_OfflineScene, false);
            }
        }

        public void StopHost()
        {
            this.OnStopHost();
            this.StopServer();
            this.StopClient();
        }

        public void StopMatchMaker()
        {
            if (this.matchMaker != null)
            {
                UnityEngine.Object.Destroy(this.matchMaker);
                this.matchMaker = null;
            }
            this.matchInfo = null;
            this.matches = null;
        }

        public void StopServer()
        {
            if (NetworkServer.active)
            {
                this.OnStopServer();
                if (LogFilter.logDebug)
                {
                    Debug.Log("NetworkManager StopServer");
                }
                this.isNetworkActive = false;
                NetworkServer.Shutdown();
                this.StopMatchMaker();
                if (this.m_OfflineScene != string.Empty)
                {
                    this.ServerChangeScene(this.m_OfflineScene);
                }
            }
        }

        public static void UnRegisterStartPosition(Transform start)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("UnRegisterStartPosition:" + start);
            }
            s_StartPositions.Remove(start);
        }

        internal static void UpdateScene()
        {
            if (((singleton != null) && (s_LoadingSceneAsync != null)) && s_LoadingSceneAsync.isDone)
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log("ClientChangeScene done readyCon:" + s_ClientReadyConnection);
                }
                singleton.FinishLoadScene();
                s_LoadingSceneAsync.allowSceneActivation = true;
                s_LoadingSceneAsync = null;
            }
        }

        public bool autoCreatePlayer
        {
            get
            {
                return this.m_AutoCreatePlayer;
            }
            set
            {
                this.m_AutoCreatePlayer = value;
            }
        }

        public List<QosType> channels
        {
            get
            {
                return this.m_Channels;
            }
        }

        public ConnectionConfig connectionConfig
        {
            get
            {
                if (this.m_ConnectionConfig == null)
                {
                    this.m_ConnectionConfig = new ConnectionConfig();
                }
                return this.m_ConnectionConfig;
            }
        }

        public bool customConfig
        {
            get
            {
                return this.m_CustomConfig;
            }
            set
            {
                this.m_CustomConfig = value;
            }
        }

        public bool dontDestroyOnLoad
        {
            get
            {
                return this.m_DontDestroyOnLoad;
            }
            set
            {
                this.m_DontDestroyOnLoad = value;
            }
        }

        public LogFilter.FilterLevel logLevel
        {
            get
            {
                return this.m_LogLevel;
            }
            set
            {
                this.m_LogLevel = value;
                LogFilter.currentLogLevel = (int) value;
            }
        }

        public string matchHost
        {
            get
            {
                return this.m_MatchHost;
            }
            set
            {
                this.m_MatchHost = value;
            }
        }

        public int matchPort
        {
            get
            {
                return this.m_MatchPort;
            }
            set
            {
                this.m_MatchPort = value;
            }
        }

        public int maxConnections
        {
            get
            {
                return this.m_MaxConnections;
            }
            set
            {
                this.m_MaxConnections = value;
            }
        }

        public float maxDelay
        {
            get
            {
                return this.m_MaxDelay;
            }
            set
            {
                this.m_MaxDelay = value;
            }
        }

        public string networkAddress
        {
            get
            {
                return this.m_NetworkAddress;
            }
            set
            {
                this.m_NetworkAddress = value;
            }
        }

        public int networkPort
        {
            get
            {
                return this.m_NetworkPort;
            }
            set
            {
                this.m_NetworkPort = value;
            }
        }

        public int numPlayers
        {
            get
            {
                int num = 0;
                foreach (NetworkConnection connection in NetworkServer.connections)
                {
                    if (connection != null)
                    {
                        foreach (PlayerController controller in connection.playerControllers)
                        {
                            if (controller.IsValid)
                            {
                                num++;
                            }
                        }
                    }
                }
                foreach (NetworkConnection connection2 in NetworkServer.localConnections)
                {
                    if (connection2 != null)
                    {
                        foreach (PlayerController controller2 in connection2.playerControllers)
                        {
                            if (controller2.IsValid)
                            {
                                num++;
                            }
                        }
                    }
                }
                return num;
            }
        }

        public string offlineScene
        {
            get
            {
                return this.m_OfflineScene;
            }
            set
            {
                this.m_OfflineScene = value;
            }
        }

        public string onlineScene
        {
            get
            {
                return this.m_OnlineScene;
            }
            set
            {
                this.m_OnlineScene = value;
            }
        }

        public float packetLossPercentage
        {
            get
            {
                return this.m_PacketLossPercentage;
            }
            set
            {
                this.m_PacketLossPercentage = value;
            }
        }

        public GameObject playerPrefab
        {
            get
            {
                return this.m_PlayerPrefab;
            }
            set
            {
                this.m_PlayerPrefab = value;
            }
        }

        public PlayerSpawnMethod playerSpawnMethod
        {
            get
            {
                return this.m_PlayerSpawnMethod;
            }
            set
            {
                this.m_PlayerSpawnMethod = value;
            }
        }

        public bool runInBackground
        {
            get
            {
                return this.m_RunInBackground;
            }
            set
            {
                this.m_RunInBackground = value;
            }
        }

        public bool scriptCRCCheck
        {
            get
            {
                return this.m_ScriptCRCCheck;
            }
            set
            {
                this.m_ScriptCRCCheck = value;
            }
        }

        public EndPoint secureTunnelEndpoint
        {
            get
            {
                return this.m_EndPoint;
            }
            set
            {
                this.m_EndPoint = value;
            }
        }

        public bool sendPeerInfo
        {
            get
            {
                return this.m_SendPeerInfo;
            }
            set
            {
                this.m_SendPeerInfo = value;
            }
        }

        public string serverBindAddress
        {
            get
            {
                return this.m_ServerBindAddress;
            }
            set
            {
                this.m_ServerBindAddress = value;
            }
        }

        public bool serverBindToIP
        {
            get
            {
                return this.m_ServerBindToIP;
            }
            set
            {
                this.m_ServerBindToIP = value;
            }
        }

        public int simulatedLatency
        {
            get
            {
                return this.m_SimulatedLatency;
            }
            set
            {
                this.m_SimulatedLatency = value;
            }
        }

        public List<GameObject> spawnPrefabs
        {
            get
            {
                return this.m_SpawnPrefabs;
            }
        }

        public List<Transform> startPositions
        {
            get
            {
                return s_StartPositions;
            }
        }

        public bool useSimulator
        {
            get
            {
                return this.m_UseSimulator;
            }
            set
            {
                this.m_UseSimulator = value;
            }
        }
    }
}

