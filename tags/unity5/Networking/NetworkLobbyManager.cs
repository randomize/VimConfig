namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Networking.NetworkSystem;

    [AddComponentMenu("Network/NetworkLobbyManager")]
    public class NetworkLobbyManager : NetworkManager
    {
        public NetworkLobbyPlayer[] lobbySlots;
        [SerializeField]
        private GameObject m_GamePlayerPrefab;
        [SerializeField]
        private NetworkLobbyPlayer m_LobbyPlayerPrefab;
        [SerializeField]
        private string m_LobbyScene = string.Empty;
        [SerializeField]
        private int m_MaxPlayers = 4;
        [SerializeField]
        private int m_MaxPlayersPerConnection = 1;
        [SerializeField]
        private int m_MinPlayers;
        private List<PendingPlayer> m_PendingPlayers = new List<PendingPlayer>();
        [SerializeField]
        private string m_PlayScene = string.Empty;
        [SerializeField]
        private bool m_ShowLobbyGUI = true;
        private static LobbyReadyToBeginMessage s_LobbyReadyToBeginMessage = new LobbyReadyToBeginMessage();
        private static LobbyReadyToBeginMessage s_ReadyToBeginMessage = new LobbyReadyToBeginMessage();
        private static IntegerMessage s_SceneLoadedMessage = new IntegerMessage();

        private void CallOnClientEnterLobby()
        {
            this.OnLobbyClientEnter();
            foreach (NetworkLobbyPlayer player in this.lobbySlots)
            {
                if (player != null)
                {
                    player.readyToBegin = false;
                    player.OnClientEnterLobby();
                }
            }
        }

        private void CallOnClientExitLobby()
        {
            this.OnLobbyClientExit();
            foreach (NetworkLobbyPlayer player in this.lobbySlots)
            {
                if (player != null)
                {
                    player.OnClientExitLobby();
                }
            }
        }

        private static bool CheckConnectionIsReadyToBegin(NetworkConnection conn)
        {
            foreach (PlayerController controller in conn.playerControllers)
            {
                if (controller.IsValid && !controller.gameObject.GetComponent<NetworkLobbyPlayer>().readyToBegin)
                {
                    return false;
                }
            }
            return true;
        }

        public void CheckReadyToBegin()
        {
            if (Application.loadedLevelName == this.m_LobbyScene)
            {
                int num = 0;
                foreach (NetworkConnection connection in NetworkServer.connections)
                {
                    if (connection != null)
                    {
                        if (CheckConnectionIsReadyToBegin(connection))
                        {
                            num++;
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                foreach (NetworkConnection connection2 in NetworkServer.localConnections)
                {
                    if (connection2 != null)
                    {
                        if (CheckConnectionIsReadyToBegin(connection2))
                        {
                            num++;
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                if ((this.m_MinPlayers <= 0) || (num >= this.m_MinPlayers))
                {
                    this.m_PendingPlayers.Clear();
                    this.OnLobbyServerPlayersReady();
                }
            }
        }

        private byte FindSlot()
        {
            for (byte i = 0; i < this.maxPlayers; i = (byte) (i + 1))
            {
                if (this.lobbySlots[i] == null)
                {
                    return i;
                }
            }
            return 0xff;
        }

        private void OnClientAddPlayerFailedMessage(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkLobbyManager Add Player failed.");
            }
            this.OnLobbyClientAddPlayerFailed();
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            this.OnLobbyClientConnect(conn);
            this.CallOnClientEnterLobby();
            base.OnClientConnect(conn);
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            this.OnLobbyClientDisconnect(conn);
            base.OnClientDisconnect(conn);
        }

        private void OnClientReadyToBegin(NetworkMessage netMsg)
        {
            netMsg.ReadMessage<LobbyReadyToBeginMessage>(s_LobbyReadyToBeginMessage);
            if (s_LobbyReadyToBeginMessage.slotId >= this.lobbySlots.Count<NetworkLobbyPlayer>())
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("NetworkLobbyManager OnClientReadyToBegin invalid lobby slot " + s_LobbyReadyToBeginMessage.slotId);
                }
            }
            else
            {
                NetworkLobbyPlayer player = this.lobbySlots[s_LobbyReadyToBeginMessage.slotId];
                if ((player == null) || (player.gameObject == null))
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError("NetworkLobbyManager OnClientReadyToBegin no player at lobby slot " + s_LobbyReadyToBeginMessage.slotId);
                    }
                }
                else
                {
                    player.readyToBegin = s_LobbyReadyToBeginMessage.readyState;
                    player.OnClientReady(s_LobbyReadyToBeginMessage.readyState);
                }
            }
        }

        public override void OnClientSceneChanged(NetworkConnection conn)
        {
            if (Application.loadedLevelName == this.lobbyScene)
            {
                if (base.client.isConnected)
                {
                    this.CallOnClientEnterLobby();
                }
            }
            else
            {
                this.CallOnClientExitLobby();
            }
            base.OnClientSceneChanged(conn);
            this.OnLobbyClientSceneChanged(conn);
        }

        private void OnGUI()
        {
            if (this.showLobbyGUI && (Application.loadedLevelName == this.m_LobbyScene))
            {
                Rect position = new Rect(90f, 180f, 500f, 150f);
                GUI.Box(position, "Players:");
                if (NetworkClient.active)
                {
                    Rect rect2 = new Rect(100f, 300f, 120f, 20f);
                    if (GUI.Button(rect2, "Add Player"))
                    {
                        this.TryToAddPlayer();
                    }
                }
            }
        }

        public virtual void OnLobbyClientAddPlayerFailed()
        {
        }

        public virtual void OnLobbyClientConnect(NetworkConnection conn)
        {
        }

        public virtual void OnLobbyClientDisconnect(NetworkConnection conn)
        {
        }

        public virtual void OnLobbyClientEnter()
        {
        }

        public virtual void OnLobbyClientExit()
        {
        }

        public virtual void OnLobbyClientSceneChanged(NetworkConnection conn)
        {
        }

        public virtual void OnLobbyServerConnect(NetworkConnection conn)
        {
        }

        public virtual GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn, short playerControllerId)
        {
            return null;
        }

        public virtual GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
        {
            return null;
        }

        public virtual void OnLobbyServerDisconnect(NetworkConnection conn)
        {
        }

        public virtual void OnLobbyServerPlayerRemoved(NetworkConnection conn, short playerControllerId)
        {
        }

        public virtual void OnLobbyServerPlayersReady()
        {
            this.ServerChangeScene(this.m_PlayScene);
        }

        public virtual void OnLobbyServerSceneChanged(string sceneName)
        {
        }

        public virtual bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
        {
            return true;
        }

        public virtual void OnLobbyStartClient(NetworkClient lobbyClient)
        {
        }

        public virtual void OnLobbyStartHost()
        {
        }

        public virtual void OnLobbyStartServer()
        {
        }

        public virtual void OnLobbyStopClient()
        {
        }

        public virtual void OnLobbyStopHost()
        {
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            if (Application.loadedLevelName == this.m_LobbyScene)
            {
                int num = 0;
                foreach (PlayerController controller in conn.playerControllers)
                {
                    if (controller.IsValid)
                    {
                        num++;
                    }
                }
                if (num >= this.maxPlayersPerConnection)
                {
                    if (LogFilter.logWarn)
                    {
                        Debug.LogWarning("NetworkLobbyManager no more players for this connection.");
                    }
                    EmptyMessage msg = new EmptyMessage();
                    conn.Send(0x2d, msg);
                }
                else
                {
                    byte index = this.FindSlot();
                    if (index == 0xff)
                    {
                        if (LogFilter.logWarn)
                        {
                            Debug.LogWarning("NetworkLobbyManager no space for more players");
                        }
                        EmptyMessage message2 = new EmptyMessage();
                        conn.Send(0x2d, message2);
                    }
                    else
                    {
                        GameObject obj2 = this.OnLobbyServerCreateLobbyPlayer(conn, playerControllerId);
                        if (obj2 == null)
                        {
                            obj2 = (GameObject) UnityEngine.Object.Instantiate(this.lobbyPlayerPrefab.gameObject, Vector3.zero, Quaternion.identity);
                        }
                        NetworkLobbyPlayer component = obj2.GetComponent<NetworkLobbyPlayer>();
                        component.slot = index;
                        this.lobbySlots[index] = component;
                        NetworkServer.AddPlayerForConnection(conn, obj2, playerControllerId);
                    }
                }
            }
        }

        public override void OnServerConnect(NetworkConnection conn)
        {
            if (base.numPlayers >= this.maxPlayers)
            {
                conn.Disconnect();
            }
            else if (Application.loadedLevelName != this.m_LobbyScene)
            {
                conn.Disconnect();
            }
            else
            {
                base.OnServerConnect(conn);
                this.OnLobbyServerConnect(conn);
            }
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);
            this.OnLobbyServerDisconnect(conn);
        }

        private void OnServerReadyToBeginMessage(NetworkMessage netMsg)
        {
            PlayerController controller;
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkLobbyManager OnServerReadyToBeginMessage");
            }
            netMsg.ReadMessage<LobbyReadyToBeginMessage>(s_ReadyToBeginMessage);
            if (!netMsg.conn.GetPlayerController(s_ReadyToBeginMessage.slotId, out controller))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("NetworkLobbyManager OnServerReadyToBeginMessage invalid playerControllerId " + s_ReadyToBeginMessage.slotId);
                }
            }
            else
            {
                NetworkLobbyPlayer component = controller.gameObject.GetComponent<NetworkLobbyPlayer>();
                component.readyToBegin = s_ReadyToBeginMessage.readyState;
                LobbyReadyToBeginMessage msg = new LobbyReadyToBeginMessage {
                    slotId = component.slot,
                    readyState = s_ReadyToBeginMessage.readyState
                };
                NetworkServer.SendToReady(null, 0x2b, msg);
                this.CheckReadyToBegin();
            }
        }

        public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
        {
            short playerControllerId = player.playerControllerId;
            byte slot = player.gameObject.GetComponent<NetworkLobbyPlayer>().slot;
            this.lobbySlots[slot] = null;
            base.OnServerRemovePlayer(conn, player);
            foreach (NetworkLobbyPlayer player2 in this.lobbySlots)
            {
                if (player2 != null)
                {
                    player2.GetComponent<NetworkLobbyPlayer>().readyToBegin = false;
                    s_LobbyReadyToBeginMessage.slotId = player2.slot;
                    s_LobbyReadyToBeginMessage.readyState = false;
                    NetworkServer.SendToReady(null, 0x2b, s_LobbyReadyToBeginMessage);
                }
            }
            this.OnLobbyServerPlayerRemoved(conn, playerControllerId);
        }

        private void OnServerReturnToLobbyMessage(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkLobbyManager OnServerReturnToLobbyMessage");
            }
            this.ServerReturnToLobby();
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            if (sceneName != this.m_LobbyScene)
            {
                foreach (PendingPlayer player in this.m_PendingPlayers)
                {
                    this.SceneLoadedForPlayer(player.conn, player.lobbyPlayer);
                }
                this.m_PendingPlayers.Clear();
            }
            this.OnLobbyServerSceneChanged(sceneName);
        }

        private void OnServerSceneLoadedMessage(NetworkMessage netMsg)
        {
            PlayerController controller;
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkLobbyManager OnSceneLoadedMessage");
            }
            netMsg.ReadMessage<IntegerMessage>(s_SceneLoadedMessage);
            if (!netMsg.conn.GetPlayerController((short) s_SceneLoadedMessage.value, out controller))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("NetworkLobbyManager OnServerSceneLoadedMessage invalid playerControllerId " + s_SceneLoadedMessage.value);
                }
            }
            else
            {
                this.SceneLoadedForPlayer(netMsg.conn, controller.gameObject);
            }
        }

        public override void OnStartClient(NetworkClient lobbyClient)
        {
            if (this.lobbySlots.Length == 0)
            {
                this.lobbySlots = new NetworkLobbyPlayer[this.maxPlayers];
            }
            if ((this.m_LobbyPlayerPrefab == null) || (this.m_LobbyPlayerPrefab.gameObject == null))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("NetworkLobbyManager no LobbyPlayer prefab is registered. Please add a LobbyPlayer prefab.");
                }
            }
            else
            {
                ClientScene.RegisterPrefab(this.m_LobbyPlayerPrefab.gameObject);
            }
            if (this.m_GamePlayerPrefab == null)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("NetworkLobbyManager no GamePlayer prefab is registered. Please add a GamePlayer prefab.");
                }
            }
            else
            {
                ClientScene.RegisterPrefab(this.m_GamePlayerPrefab);
            }
            lobbyClient.RegisterHandler(0x2b, new NetworkMessageDelegate(this.OnClientReadyToBegin));
            lobbyClient.RegisterHandler(0x2d, new NetworkMessageDelegate(this.OnClientAddPlayerFailedMessage));
            this.OnLobbyStartClient(lobbyClient);
        }

        public override void OnStartHost()
        {
            this.OnLobbyStartHost();
        }

        public override void OnStartServer()
        {
            if (this.lobbySlots.Length == 0)
            {
                this.lobbySlots = new NetworkLobbyPlayer[this.maxPlayers];
            }
            NetworkServer.RegisterHandler(0x2b, new NetworkMessageDelegate(this.OnServerReadyToBeginMessage));
            NetworkServer.RegisterHandler(0x2c, new NetworkMessageDelegate(this.OnServerSceneLoadedMessage));
            NetworkServer.RegisterHandler(0x2e, new NetworkMessageDelegate(this.OnServerReturnToLobbyMessage));
            this.OnLobbyStartServer();
        }

        public override void OnStopClient()
        {
            this.OnLobbyStopClient();
            this.CallOnClientExitLobby();
        }

        public override void OnStopHost()
        {
            this.OnLobbyStopHost();
        }

        private void OnValidate()
        {
            if (this.m_MaxPlayers <= 0)
            {
                this.m_MaxPlayers = 1;
            }
            if (this.m_MaxPlayersPerConnection <= 0)
            {
                this.m_MaxPlayersPerConnection = 1;
            }
            if (this.m_MaxPlayersPerConnection > this.maxPlayers)
            {
                this.m_MaxPlayersPerConnection = this.maxPlayers;
            }
            if (this.m_MinPlayers < 0)
            {
                this.m_MinPlayers = 0;
            }
            if (this.m_MinPlayers > this.m_MaxPlayers)
            {
                this.m_MinPlayers = this.m_MaxPlayers;
            }
        }

        private void SceneLoadedForPlayer(NetworkConnection conn, GameObject lobbyPlayerGameObject)
        {
            if (lobbyPlayerGameObject.GetComponent<NetworkLobbyPlayer>() != null)
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log(string.Concat(new object[] { "NetworkLobby SceneLoadedForPlayer scene:", Application.loadedLevelName, " ", conn }));
                }
                if (Application.loadedLevelName == this.m_LobbyScene)
                {
                    PendingPlayer player2;
                    player2.conn = conn;
                    player2.lobbyPlayer = lobbyPlayerGameObject;
                    this.m_PendingPlayers.Add(player2);
                }
                else
                {
                    short playerControllerId = lobbyPlayerGameObject.GetComponent<NetworkIdentity>().playerControllerId;
                    GameObject gamePlayer = this.OnLobbyServerCreateGamePlayer(conn, playerControllerId);
                    if (gamePlayer == null)
                    {
                        Transform startPosition = base.GetStartPosition();
                        if (startPosition != null)
                        {
                            gamePlayer = (GameObject) UnityEngine.Object.Instantiate(this.gamePlayerPrefab, startPosition.position, startPosition.rotation);
                        }
                        else
                        {
                            gamePlayer = (GameObject) UnityEngine.Object.Instantiate(this.gamePlayerPrefab, Vector3.zero, Quaternion.identity);
                        }
                    }
                    if (this.OnLobbyServerSceneLoadedForPlayer(lobbyPlayerGameObject, gamePlayer))
                    {
                        NetworkServer.ReplacePlayerForConnection(conn, gamePlayer, playerControllerId);
                    }
                }
            }
        }

        public bool SendReturnToLobby()
        {
            if ((base.client == null) || !base.client.isConnected)
            {
                return false;
            }
            EmptyMessage msg = new EmptyMessage();
            base.client.Send(0x2e, msg);
            return true;
        }

        public override void ServerChangeScene(string sceneName)
        {
            if (sceneName == this.m_LobbyScene)
            {
                foreach (NetworkLobbyPlayer player in this.lobbySlots)
                {
                    if (player != null)
                    {
                        PlayerController controller;
                        NetworkIdentity component = player.GetComponent<NetworkIdentity>();
                        if (component.connectionToClient.GetPlayerController(component.playerControllerId, out controller))
                        {
                            NetworkServer.Destroy(controller.gameObject);
                        }
                        if (NetworkServer.active)
                        {
                            player.GetComponent<NetworkLobbyPlayer>().readyToBegin = false;
                            NetworkServer.ReplacePlayerForConnection(component.connectionToClient, player.gameObject, component.playerControllerId);
                        }
                    }
                }
            }
            base.ServerChangeScene(sceneName);
        }

        public void ServerReturnToLobby()
        {
            if (!NetworkServer.active)
            {
                Debug.Log("ServerReturnToLobby called on client");
            }
            else
            {
                this.ServerChangeScene(this.m_LobbyScene);
            }
        }

        public void TryToAddPlayer()
        {
            if (!NetworkClient.active)
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log("NetworkLobbyManager NetworkClient not active!");
                }
            }
            else
            {
                short playerControllerId = -1;
                List<PlayerController> playerControllers = NetworkClient.allClients[0].connection.playerControllers;
                if (playerControllers.Count < this.maxPlayers)
                {
                    playerControllerId = (short) playerControllers.Count;
                }
                else
                {
                    for (short i = 0; i < this.maxPlayers; i = (short) (i + 1))
                    {
                        if (!playerControllers[i].IsValid)
                        {
                            playerControllerId = i;
                            break;
                        }
                    }
                }
                if (LogFilter.logDebug)
                {
                    Debug.Log(string.Concat(new object[] { "NetworkLobbyManager TryToAddPlayer controllerId ", playerControllerId, " ready:", ClientScene.ready }));
                }
                if (playerControllerId == -1)
                {
                    if (LogFilter.logDebug)
                    {
                        Debug.Log("NetworkLobbyManager No Space!");
                    }
                }
                else if (ClientScene.ready)
                {
                    ClientScene.AddPlayer(playerControllerId);
                }
                else
                {
                    ClientScene.AddPlayer(NetworkClient.allClients[0].connection, playerControllerId);
                }
            }
        }

        public GameObject gamePlayerPrefab
        {
            get
            {
                return this.m_GamePlayerPrefab;
            }
            set
            {
                this.m_GamePlayerPrefab = value;
            }
        }

        public NetworkLobbyPlayer lobbyPlayerPrefab
        {
            get
            {
                return this.m_LobbyPlayerPrefab;
            }
            set
            {
                this.m_LobbyPlayerPrefab = value;
            }
        }

        public string lobbyScene
        {
            get
            {
                return this.m_LobbyScene;
            }
            set
            {
                this.m_LobbyScene = value;
                base.offlineScene = value;
            }
        }

        public int maxPlayers
        {
            get
            {
                return this.m_MaxPlayers;
            }
            set
            {
                this.m_MaxPlayers = value;
            }
        }

        public int maxPlayersPerConnection
        {
            get
            {
                return this.m_MaxPlayersPerConnection;
            }
            set
            {
                this.m_MaxPlayersPerConnection = value;
            }
        }

        public int minPlayers
        {
            get
            {
                return this.m_MinPlayers;
            }
            set
            {
                this.m_MinPlayers = value;
            }
        }

        public string playScene
        {
            get
            {
                return this.m_PlayScene;
            }
            set
            {
                this.m_PlayScene = value;
            }
        }

        public bool showLobbyGUI
        {
            get
            {
                return this.m_ShowLobbyGUI;
            }
            set
            {
                this.m_ShowLobbyGUI = value;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PendingPlayer
        {
            public NetworkConnection conn;
            public GameObject lobbyPlayer;
        }
    }
}

