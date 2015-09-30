namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Networking.Match;
    using UnityEngine.Networking.NetworkSystem;
    using UnityEngine.Networking.Types;

    public sealed class NetworkServer
    {
        private const int k_MaxEventsPerFrame = 500;
        private const int k_RemoveListInterval = 100;
        private ConnectionArray m_Connections = new ConnectionArray();
        private HostTopology m_HostTopology;
        private List<LocalClient> m_LocalClients = new List<LocalClient>();
        private float m_MaxDelay = 0.1f;
        private NetworkMessageHandlers m_MessageHandlers = new NetworkMessageHandlers();
        private byte[] m_MsgBuffer;
        private int m_RelaySlotId = -1;
        private HashSet<NetworkInstanceId> m_RemoveList;
        private int m_RemoveListCount;
        private bool m_SendPeerInfo = true;
        private int m_ServerId = -1;
        private int m_ServerPort = -1;
        private static bool s_Active;
        private static HashSet<int> s_ExternalConnections = new HashSet<int>();
        private static volatile NetworkServer s_Instance;
        private static bool s_LocalClientActive;
        private static System.Type s_NetworkConnectionClass = typeof(NetworkConnection);
        private static NetworkScene s_NetworkScene = new NetworkScene();
        private static RemovePlayerMessage s_RemovePlayerMessage = new RemovePlayerMessage();
        private static object s_Sync = new UnityEngine.Object();

        private NetworkServer()
        {
            NetworkTransport.Init();
            if (LogFilter.logDev)
            {
                Debug.Log("NetworkServer Created version " + UnityEngine.Networking.Version.Current);
            }
            this.m_MsgBuffer = new byte[0xc000];
            this.m_RemoveList = new HashSet<NetworkInstanceId>();
        }

        internal void ActivateLocalClientScene()
        {
            if (!s_LocalClientActive)
            {
                s_LocalClientActive = true;
                foreach (NetworkIdentity identity in objects.Values)
                {
                    if (!identity.isClient)
                    {
                        if (LogFilter.logDev)
                        {
                            Debug.Log(string.Concat(new object[] { "ActivateClientScene ", identity.netId, " ", identity.gameObject }));
                        }
                        ClientScene.SetLocalObject(identity.netId, identity.gameObject);
                        identity.OnStartClient();
                    }
                }
            }
        }

        public static bool AddExternalConnection(NetworkConnection conn)
        {
            return instance.AddExternalConnectionInternal(conn);
        }

        private bool AddExternalConnectionInternal(NetworkConnection conn)
        {
            if (this.m_Connections.Get(conn.connectionId) != null)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("AddExternalConnection failed, already connection for id:" + conn.connectionId);
                }
                return false;
            }
            if (LogFilter.logDebug)
            {
                Debug.Log("AddExternalConnection external connection " + conn.connectionId);
            }
            this.m_Connections.Add(conn.connectionId, conn);
            conn.SetHandlers(this.m_MessageHandlers);
            s_ExternalConnections.Add(conn.connectionId);
            return true;
        }

        internal int AddLocalClient(LocalClient localClient)
        {
            this.m_LocalClients.Add(localClient);
            ULocalConnectionToClient conn = new ULocalConnectionToClient(localClient);
            conn.SetHandlers(this.m_MessageHandlers);
            return this.m_Connections.AddLocal(conn);
        }

        public static bool AddPlayerForConnection(NetworkConnection conn, GameObject player, short playerControllerId)
        {
            return instance.InternalAddPlayerForConnection(conn, player, playerControllerId);
        }

        public static bool AddPlayerForConnection(NetworkConnection conn, GameObject player, short playerControllerId, NetworkHash128 assetId)
        {
            NetworkIdentity identity;
            if (GetNetworkIdentity(player, out identity))
            {
                identity.SetDynamicAssetId(assetId);
            }
            return instance.InternalAddPlayerForConnection(conn, player, playerControllerId);
        }

        private void CheckForNullObjects()
        {
            foreach (NetworkInstanceId id in objects.Keys)
            {
                NetworkIdentity identity = objects[id];
                if ((identity == null) || (identity.gameObject == null))
                {
                    this.m_RemoveList.Add(id);
                }
            }
            if (this.m_RemoveList.Count > 0)
            {
                foreach (NetworkInstanceId id2 in this.m_RemoveList)
                {
                    objects.Remove(id2);
                }
                this.m_RemoveList.Clear();
            }
        }

        private static bool CheckPlayerControllerIdForConnection(NetworkConnection conn, short playerControllerId)
        {
            if (playerControllerId < 0)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("AddPlayer: playerControllerId of " + playerControllerId + " is negative");
                }
                return false;
            }
            if (playerControllerId > 0x20)
            {
                if (LogFilter.logError)
                {
                    Debug.Log(string.Concat(new object[] { "AddPlayer: playerControllerId of ", playerControllerId, " is too high. max is ", 0x20 }));
                }
                return false;
            }
            if ((playerControllerId > 0x10) && LogFilter.logWarn)
            {
                Debug.LogWarning("AddPlayer: playerControllerId of " + playerControllerId + " is unusually high");
            }
            return true;
        }

        public static void ClearHandlers()
        {
            instance.m_MessageHandlers.ClearMessageHandlers();
        }

        public static void ClearLocalObjects()
        {
            objects.Clear();
        }

        public static void ClearSpawners()
        {
            NetworkScene.ClearSpawners();
        }

        public static bool Configure(HostTopology topology)
        {
            instance.m_HostTopology = topology;
            return true;
        }

        public static bool Configure(ConnectionConfig config, int maxConnections)
        {
            HostTopology topology = new HostTopology(config, maxConnections);
            return Configure(topology);
        }

        public static void Destroy(GameObject obj)
        {
            DestroyObject(obj);
        }

        private static void DestroyObject(GameObject obj)
        {
            if (obj == null)
            {
                if (LogFilter.logDev)
                {
                    Debug.Log("NetworkServer DestroyObject is null");
                }
            }
            else
            {
                NetworkIdentity identity;
                if (GetNetworkIdentity(obj, out identity))
                {
                    DestroyObject(identity, true);
                }
            }
        }

        private static void DestroyObject(NetworkIdentity uv, bool destroyServerObject)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("DestroyObject instance:" + uv.netId);
            }
            if (objects.ContainsKey(uv.netId))
            {
                objects.Remove(uv.netId);
            }
            if (uv.clientAuthorityOwner != null)
            {
                uv.clientAuthorityOwner.RemoveOwnedObject(uv);
            }
            NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Outgoing, 1, uv.assetId.ToString(), 1);
            ObjectDestroyMessage msg = new ObjectDestroyMessage {
                netId = uv.netId
            };
            SendToObservers(uv.gameObject, 1, msg);
            uv.ClearObservers();
            if (NetworkClient.active && s_LocalClientActive)
            {
                uv.OnNetworkDestroy();
                ClientScene.SetLocalObject(msg.netId, null);
            }
            if (destroyServerObject)
            {
                UnityEngine.Object.Destroy(uv.gameObject);
            }
            uv.SetNoServer();
        }

        public static void DestroyPlayersForConnection(NetworkConnection conn)
        {
            if (conn.playerControllers.Count == 0)
            {
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning("Empty player list given to NetworkServer.Destroy(), nothing to do.");
                }
            }
            else
            {
                if (conn.clientOwnedObjects != null)
                {
                    HashSet<NetworkInstanceId> set = new HashSet<NetworkInstanceId>(conn.clientOwnedObjects);
                    foreach (NetworkInstanceId id in set)
                    {
                        GameObject obj2 = FindLocalObject(id);
                        if (obj2 != null)
                        {
                            DestroyObject(obj2);
                        }
                    }
                }
                foreach (PlayerController controller in conn.playerControllers)
                {
                    if (controller.IsValid)
                    {
                        if (controller.unetView != null)
                        {
                            DestroyObject(controller.unetView, true);
                        }
                        controller.gameObject = null;
                    }
                }
                conn.playerControllers.Clear();
            }
        }

        public static void DisconnectAll()
        {
            instance.InternalDisconnectAll();
        }

        public static GameObject FindLocalObject(NetworkInstanceId netId)
        {
            return s_NetworkScene.FindLocalObject(netId);
        }

        private static void FinishPlayerForConnection(NetworkConnection conn, NetworkIdentity uv, GameObject playerGameObject)
        {
            if (uv.netId.IsEmpty())
            {
                Spawn(playerGameObject);
            }
            OwnerMessage msg = new OwnerMessage {
                netId = uv.netId,
                playerControllerId = uv.playerControllerId
            };
            conn.Send(4, msg);
        }

        private void GenerateConnectError(int error)
        {
            if (LogFilter.logError)
            {
                Debug.LogError("UNet Server Connect Error: " + error);
            }
            this.GenerateError(null, error);
        }

        private void GenerateDataError(NetworkConnection conn, int error)
        {
            NetworkError error2 = (NetworkError) error;
            if (LogFilter.logError)
            {
                Debug.LogError("UNet Server Data Error: " + error2);
            }
            this.GenerateError(conn, error);
        }

        private void GenerateDisconnectError(NetworkConnection conn, int error)
        {
            NetworkError error2 = (NetworkError) error;
            if (LogFilter.logError)
            {
                Debug.LogError(string.Concat(new object[] { "UNet Server Disconnect Error: ", error2, " conn:[", conn, "]:", conn.connectionId }));
            }
            this.GenerateError(conn, error);
        }

        private void GenerateError(NetworkConnection conn, int error)
        {
            if (this.m_MessageHandlers.GetHandler(0x22) != null)
            {
                ErrorMessage message = new ErrorMessage {
                    errorCode = error
                };
                NetworkWriter writer = new NetworkWriter();
                message.Serialize(writer);
                NetworkReader reader = new NetworkReader(writer);
                conn.InvokeHandler(0x22, reader, 0);
            }
        }

        public static Dictionary<short, NetworkConnection.PacketStat> GetConnectionStats()
        {
            ConnectionArray connections = instance.m_Connections;
            Dictionary<short, NetworkConnection.PacketStat> dictionary = new Dictionary<short, NetworkConnection.PacketStat>();
            for (int i = connections.LocalIndex; i < connections.Count; i++)
            {
                NetworkConnection connection = connections.Get(i);
                if (connection != null)
                {
                    foreach (short num2 in connection.packetStats.Keys)
                    {
                        if (dictionary.ContainsKey(num2))
                        {
                            NetworkConnection.PacketStat stat = dictionary[num2];
                            stat.count += connection.packetStats[num2].count;
                            stat.bytes += connection.packetStats[num2].bytes;
                            dictionary[num2] = stat;
                        }
                        else
                        {
                            dictionary[num2] = connection.packetStats[num2];
                        }
                    }
                }
            }
            return dictionary;
        }

        private static bool GetNetworkIdentity(GameObject go, out NetworkIdentity view)
        {
            view = go.GetComponent<NetworkIdentity>();
            if (view != null)
            {
                return true;
            }
            if (LogFilter.logError)
            {
                Debug.LogError("UNET failure. GameObject doesn't have NetworkIdentity.");
            }
            return false;
        }

        public static void GetStatsIn(out int numMsgs, out int numBytes)
        {
            numMsgs = 0;
            numBytes = 0;
            ConnectionArray connections = instance.m_Connections;
            for (int i = connections.LocalIndex; i < connections.Count; i++)
            {
                NetworkConnection connection = connections.Get(i);
                if (connection != null)
                {
                    int num2;
                    int num3;
                    connection.GetStatsIn(out num2, out num3);
                    numMsgs += num2;
                    numBytes += num3;
                }
            }
        }

        public static void GetStatsOut(out int numMsgs, out int numBufferedMsgs, out int numBytes, out int lastBufferedPerSecond)
        {
            numMsgs = 0;
            numBufferedMsgs = 0;
            numBytes = 0;
            lastBufferedPerSecond = 0;
            ConnectionArray connections = instance.m_Connections;
            for (int i = connections.LocalIndex; i < connections.Count; i++)
            {
                NetworkConnection connection = connections.Get(i);
                if (connection != null)
                {
                    int num2;
                    int num3;
                    int num4;
                    int num5;
                    connection.GetStatsOut(out num2, out num3, out num4, out num5);
                    numMsgs += num2;
                    numBufferedMsgs += num3;
                    numBytes += num4;
                    lastBufferedPerSecond += num5;
                }
            }
        }

        internal static void HideForConnection(NetworkIdentity uv, NetworkConnection conn)
        {
            ObjectDestroyMessage msg = new ObjectDestroyMessage {
                netId = uv.netId
            };
            conn.Send(13, msg);
        }

        internal bool InternalAddPlayerForConnection(NetworkConnection conn, GameObject playerGameObject, short playerControllerId)
        {
            NetworkIdentity identity;
            if (!GetNetworkIdentity(playerGameObject, out identity))
            {
                if (LogFilter.logError)
                {
                    Debug.Log("AddPlayer: playerGameObject has no NetworkIdentity. Please add a NetworkIdentity to " + playerGameObject);
                }
                return false;
            }
            if (!CheckPlayerControllerIdForConnection(conn, playerControllerId))
            {
                return false;
            }
            PlayerController playerController = null;
            GameObject gameObject = null;
            if (conn.GetPlayerController(playerControllerId, out playerController))
            {
                gameObject = playerController.gameObject;
            }
            if (gameObject != null)
            {
                if (LogFilter.logError)
                {
                    Debug.Log("AddPlayer: player object already exists for playerControllerId of " + playerControllerId);
                }
                return false;
            }
            PlayerController player = new PlayerController(playerGameObject, playerControllerId);
            conn.SetPlayerController(player);
            identity.SetConnectionToClient(conn, player.playerControllerId);
            SetClientReady(conn);
            if (!this.SetupLocalPlayerForConnection(conn, identity, player))
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log(string.Concat(new object[] { "Adding new playerGameObject object netId: ", playerGameObject.GetComponent<NetworkIdentity>().netId, " asset ID ", playerGameObject.GetComponent<NetworkIdentity>().assetId }));
                }
                FinishPlayerForConnection(conn, identity, playerGameObject);
                if (identity.localPlayerAuthority)
                {
                    identity.SetClientOwner(conn);
                }
            }
            return true;
        }

        internal void InternalDisconnectAll()
        {
            for (int i = this.m_Connections.LocalIndex; i < this.m_Connections.Count; i++)
            {
                NetworkConnection connection = this.m_Connections.Get(i);
                if (connection != null)
                {
                    connection.Disconnect();
                    connection.Dispose();
                }
            }
            s_Active = false;
            s_LocalClientActive = false;
        }

        internal bool InternalListen(string ipAddress, int serverPort)
        {
            if (this.m_HostTopology == null)
            {
                ConnectionConfig defaultConfig = new ConnectionConfig();
                defaultConfig.AddChannel(QosType.Reliable);
                defaultConfig.AddChannel(QosType.Unreliable);
                this.m_HostTopology = new HostTopology(defaultConfig, 8);
            }
            if (LogFilter.logDebug)
            {
                Debug.Log("Server Listen. port: " + serverPort);
            }
            if (string.IsNullOrEmpty(ipAddress))
            {
                this.m_ServerId = NetworkTransport.AddHost(this.m_HostTopology, serverPort);
            }
            else
            {
                this.m_ServerId = NetworkTransport.AddHost(this.m_HostTopology, serverPort, ipAddress);
            }
            if (this.m_ServerId == -1)
            {
                return false;
            }
            this.m_ServerPort = serverPort;
            s_Active = true;
            this.m_MessageHandlers.RegisterHandlerSafe(0x23, new NetworkMessageDelegate(NetworkServer.OnClientReadyMessage));
            this.m_MessageHandlers.RegisterHandlerSafe(5, new NetworkMessageDelegate(NetworkServer.OnCommandMessage));
            this.m_MessageHandlers.RegisterHandlerSafe(6, new NetworkMessageDelegate(NetworkTransform.HandleTransform));
            this.m_MessageHandlers.RegisterHandlerSafe(0x10, new NetworkMessageDelegate(NetworkTransformChild.HandleChildTransform));
            this.m_MessageHandlers.RegisterHandlerSafe(0x26, new NetworkMessageDelegate(NetworkServer.OnRemovePlayerMessage));
            this.m_MessageHandlers.RegisterHandlerSafe(40, new NetworkMessageDelegate(NetworkAnimator.OnAnimationServerMessage));
            this.m_MessageHandlers.RegisterHandlerSafe(0x29, new NetworkMessageDelegate(NetworkAnimator.OnAnimationParametersServerMessage));
            this.m_MessageHandlers.RegisterHandlerSafe(0x2a, new NetworkMessageDelegate(NetworkAnimator.OnAnimationTriggerServerMessage));
            return true;
        }

        internal void InternalListenRelay(string relayIp, int relayPort, NetworkID netGuid, SourceID sourceId, NodeID nodeId, int listenPort)
        {
            byte num;
            if (this.m_HostTopology == null)
            {
                ConnectionConfig defaultConfig = new ConnectionConfig();
                defaultConfig.AddChannel(QosType.Reliable);
                defaultConfig.AddChannel(QosType.Unreliable);
                this.m_HostTopology = new HostTopology(defaultConfig, 8);
            }
            this.m_ServerId = NetworkTransport.AddHost(this.m_HostTopology, listenPort);
            if (LogFilter.logDebug)
            {
                Debug.Log("Server Host Slot Id: " + this.m_ServerId);
            }
            Update();
            NetworkTransport.ConnectAsNetworkHost(this.m_ServerId, relayIp, relayPort, netGuid, sourceId, nodeId, out num);
            this.m_RelaySlotId = 0;
            if (LogFilter.logDebug)
            {
                Debug.Log("Relay Slot Id: " + this.m_RelaySlotId);
            }
            if (num != 0)
            {
                Debug.Log("ListenRelay Error: " + num);
            }
            s_Active = true;
            this.m_MessageHandlers.RegisterHandlerSafe(0x23, new NetworkMessageDelegate(NetworkServer.OnClientReadyMessage));
            this.m_MessageHandlers.RegisterHandlerSafe(5, new NetworkMessageDelegate(NetworkServer.OnCommandMessage));
            this.m_MessageHandlers.RegisterHandlerSafe(6, new NetworkMessageDelegate(NetworkTransform.HandleTransform));
            this.m_MessageHandlers.RegisterHandlerSafe(0x10, new NetworkMessageDelegate(NetworkTransformChild.HandleChildTransform));
            this.m_MessageHandlers.RegisterHandlerSafe(40, new NetworkMessageDelegate(NetworkAnimator.OnAnimationServerMessage));
            this.m_MessageHandlers.RegisterHandlerSafe(0x29, new NetworkMessageDelegate(NetworkAnimator.OnAnimationParametersServerMessage));
            this.m_MessageHandlers.RegisterHandlerSafe(0x2a, new NetworkMessageDelegate(NetworkAnimator.OnAnimationTriggerServerMessage));
        }

        internal bool InternalReplacePlayerForConnection(NetworkConnection conn, GameObject playerGameObject, short playerControllerId)
        {
            NetworkIdentity identity;
            PlayerController controller;
            if (!GetNetworkIdentity(playerGameObject, out identity))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("ReplacePlayer: playerGameObject has no NetworkIdentity. Please add a NetworkIdentity to " + playerGameObject);
                }
                return false;
            }
            if (!CheckPlayerControllerIdForConnection(conn, playerControllerId))
            {
                return false;
            }
            if (LogFilter.logDev)
            {
                Debug.Log("NetworkServer ReplacePlayer");
            }
            if (conn.GetPlayerController(playerControllerId, out controller))
            {
                controller.unetView.SetNotLocalPlayer();
            }
            PlayerController player = new PlayerController(playerGameObject, playerControllerId);
            conn.SetPlayerController(player);
            identity.SetConnectionToClient(conn, player.playerControllerId);
            if (LogFilter.logDev)
            {
                Debug.Log("NetworkServer ReplacePlayer setup local");
            }
            if (!this.SetupLocalPlayerForConnection(conn, identity, player))
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log(string.Concat(new object[] { "Replacing playerGameObject object netId: ", playerGameObject.GetComponent<NetworkIdentity>().netId, " asset ID ", playerGameObject.GetComponent<NetworkIdentity>().assetId }));
                }
                FinishPlayerForConnection(conn, identity, playerGameObject);
                if (identity.localPlayerAuthority)
                {
                    identity.SetClientOwner(conn);
                }
            }
            return true;
        }

        internal void InternalSetClientNotReady(NetworkConnection conn)
        {
            if (conn.isReady)
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log("PlayerNotReady " + conn);
                }
                conn.isReady = false;
                conn.RemoveObservers();
                NotReadyMessage msg = new NotReadyMessage();
                conn.Send(0x24, msg);
            }
        }

        internal void InternalSetMaxDelay(float seconds)
        {
            for (int i = this.m_Connections.LocalIndex; i < this.m_Connections.Count; i++)
            {
                NetworkConnection connection = this.m_Connections.Get(i);
                if (connection != null)
                {
                    connection.SetMaxDelay(seconds);
                }
            }
            this.m_MaxDelay = seconds;
        }

        internal void InternalUpdate()
        {
            byte num;
            NetworkEventType type;
            int num3;
            int num4;
            int num5;
            NetworkConnection @unsafe;
            if ((this.m_ServerId == -1) || !NetworkTransport.IsStarted)
            {
                return;
            }
            int num2 = 0;
            if (this.m_RelaySlotId != -1)
            {
                type = NetworkTransport.ReceiveRelayEventFromHost(this.m_ServerId, out num);
                if ((type != NetworkEventType.Nothing) && LogFilter.logDebug)
                {
                    Debug.Log("NetGroup event:" + type);
                }
                if ((type == NetworkEventType.ConnectEvent) && LogFilter.logDebug)
                {
                    Debug.Log("NetGroup server connected");
                }
                if ((type == NetworkEventType.DisconnectEvent) && LogFilter.logDebug)
                {
                    Debug.Log("NetGroup server disconnected");
                }
            }
        Label_008F:
            type = NetworkTransport.ReceiveFromHost(this.m_ServerId, out num3, out num4, this.m_MsgBuffer, (ushort) this.m_MsgBuffer.Length, out num5, out num);
            if ((type != NetworkEventType.Nothing) && LogFilter.logDev)
            {
                Debug.Log(string.Concat(new object[] { "Server event: host=", this.m_ServerId, " event=", type, " error=", num }));
            }
            switch (type)
            {
                case NetworkEventType.DataEvent:
                {
                    NetworkConnection conn = this.m_Connections.Get(num3);
                    if (num == 0)
                    {
                        NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Incoming, 0x1d, "msg", 1);
                        if (conn != null)
                        {
                            conn.TransportRecieve(this.m_MsgBuffer, num5, num4);
                        }
                        else if (LogFilter.logError)
                        {
                            Debug.LogError("Unknown connection data event?!?");
                        }
                        goto Label_0368;
                    }
                    this.GenerateDataError(conn, num);
                    return;
                }
                case NetworkEventType.ConnectEvent:
                {
                    string str;
                    int num6;
                    NetworkID kid;
                    NodeID eid;
                    byte num7;
                    if (LogFilter.logDebug)
                    {
                        Debug.Log("Server accepted client:" + num3);
                    }
                    if (num != 0)
                    {
                        this.GenerateConnectError(num);
                        return;
                    }
                    NetworkTransport.GetConnectionInfo(this.m_ServerId, num3, out str, out num6, out kid, out eid, out num7);
                    NetworkConnection connection = (NetworkConnection) Activator.CreateInstance(s_NetworkConnectionClass);
                    connection.SetHandlers(this.m_MessageHandlers);
                    connection.Initialize(str, this.m_ServerId, num3, this.m_HostTopology);
                    connection.SetMaxDelay(this.m_MaxDelay);
                    this.m_Connections.Add(num3, connection);
                    connection.InvokeHandlerNoData(0x20);
                    if (this.m_SendPeerInfo)
                    {
                        this.SendNetworkInfo(connection);
                    }
                    SendCrc(connection);
                    goto Label_0368;
                }
                case NetworkEventType.DisconnectEvent:
                    @unsafe = this.m_Connections.GetUnsafe(num3);
                    if ((num != 0) && (num != 6))
                    {
                        this.GenerateDisconnectError(@unsafe, num);
                        break;
                    }
                    break;

                case NetworkEventType.Nothing:
                    goto Label_0368;

                default:
                    if (LogFilter.logError)
                    {
                        Debug.LogError("Unknown network message type received: " + type);
                    }
                    goto Label_0368;
            }
            this.m_Connections.Remove(num3);
            if (@unsafe != null)
            {
                @unsafe.InvokeHandlerNoData(0x21);
                for (int j = 0; j < @unsafe.playerControllers.Count; j++)
                {
                    if ((@unsafe.playerControllers[j].gameObject != null) && LogFilter.logWarn)
                    {
                        Debug.LogWarning("Player not destroyed when connection disconnected.");
                    }
                }
                if (LogFilter.logDebug)
                {
                    Debug.Log("Server lost client:" + num3);
                }
                @unsafe.RemoveObservers();
                @unsafe.Dispose();
            }
            else if (LogFilter.logDebug)
            {
                Debug.Log("Connection is null in disconnect event");
            }
            if (this.m_SendPeerInfo)
            {
                this.SendNetworkInfo(@unsafe);
            }
        Label_0368:
            if (++num2 >= 500)
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log("kMaxEventsPerFrame hit (" + 500 + ")");
                }
            }
            else if (type != NetworkEventType.Nothing)
            {
                goto Label_008F;
            }
            this.UpdateServerObjects();
            for (int i = this.m_Connections.LocalIndex; i < this.m_Connections.Count; i++)
            {
                NetworkConnection connection4 = this.m_Connections.Get(i);
                if (connection4 != null)
                {
                    connection4.FlushChannels();
                }
            }
        }

        internal bool InvokeBytes(ULocalConnectionToServer conn, byte[] buffer, int numBytes, int channelId)
        {
            NetworkReader reader = new NetworkReader(buffer);
            reader.ReadInt16();
            short msgType = reader.ReadInt16();
            if (this.m_MessageHandlers.GetHandler(msgType) != null)
            {
                NetworkConnection connection = this.m_Connections.Get(conn.connectionId);
                if (connection != null)
                {
                    ((ULocalConnectionToClient) connection).InvokeHandler(msgType, reader, channelId);
                    return true;
                }
            }
            return false;
        }

        internal bool InvokeHandlerOnServer(ULocalConnectionToServer conn, short msgType, MessageBase msg, int channelId)
        {
            if (this.m_MessageHandlers.GetHandler(msgType) != null)
            {
                NetworkConnection connection = this.m_Connections.Get(conn.connectionId);
                if (connection != null)
                {
                    ULocalConnectionToClient client = (ULocalConnectionToClient) connection;
                    NetworkWriter writer = new NetworkWriter();
                    msg.Serialize(writer);
                    NetworkReader reader = new NetworkReader(writer);
                    client.InvokeHandler(msgType, reader, channelId);
                    return true;
                }
                if (LogFilter.logError)
                {
                    Debug.LogError("Local invoke: Failed to find local connection to invoke handler on [connectionId=" + conn.connectionId + "]");
                }
                return false;
            }
            if (LogFilter.logError)
            {
                Debug.LogError("Local invoke: Failed to find message handler for message ID " + msgType);
            }
            return false;
        }

        public static bool Listen(int serverPort)
        {
            return instance.InternalListen(null, serverPort);
        }

        public static bool Listen(string ipAddress, int serverPort)
        {
            return instance.InternalListen(ipAddress, serverPort);
        }

        public static bool Listen(MatchInfo matchInfo, int listenPort)
        {
            if (!matchInfo.usingRelay)
            {
                return instance.InternalListen(null, listenPort);
            }
            instance.InternalListenRelay(matchInfo.address, matchInfo.port, matchInfo.networkId, Utility.GetSourceID(), matchInfo.nodeId, listenPort);
            return true;
        }

        public static void ListenRelay(string relayIp, int relayPort, NetworkID netGuid, SourceID sourceId, NodeID nodeId)
        {
            instance.InternalListenRelay(relayIp, relayPort, netGuid, sourceId, nodeId, 0);
        }

        private static void OnClientReadyMessage(NetworkMessage netMsg)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("Default handler for ready message from " + netMsg.conn);
            }
            SetClientReady(netMsg.conn);
        }

        private static void OnCommandMessage(NetworkMessage netMsg)
        {
            int cmdHash = (int) netMsg.reader.ReadPackedUInt32();
            NetworkInstanceId netId = netMsg.reader.ReadNetworkId();
            GameObject obj2 = FindLocalObject(netId);
            if (obj2 == null)
            {
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning("Instance not found when handling Command message [netId=" + netId + "]");
                }
            }
            else
            {
                NetworkIdentity component = obj2.GetComponent<NetworkIdentity>();
                if (component == null)
                {
                    if (LogFilter.logWarn)
                    {
                        Debug.LogWarning("NetworkIdentity deleted when handling Command message [netId=" + netId + "]");
                    }
                }
                else
                {
                    bool flag = false;
                    foreach (PlayerController controller in netMsg.conn.playerControllers)
                    {
                        if ((controller.gameObject != null) && (controller.gameObject.GetComponent<NetworkIdentity>().netId == component.netId))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag && (component.clientAuthorityOwner != netMsg.conn))
                    {
                        if (LogFilter.logWarn)
                        {
                            Debug.LogWarning("Command for object without authority [netId=" + netId + "]");
                        }
                    }
                    else
                    {
                        if (LogFilter.logDev)
                        {
                            Debug.Log(string.Concat(new object[] { "OnCommandMessage for netId=", netId, " conn=", netMsg.conn }));
                        }
                        component.HandleCommand(cmdHash, netMsg.reader);
                    }
                }
            }
        }

        private static void OnRemovePlayerMessage(NetworkMessage netMsg)
        {
            netMsg.ReadMessage<RemovePlayerMessage>(s_RemovePlayerMessage);
            PlayerController playerController = null;
            netMsg.conn.GetPlayerController(s_RemovePlayerMessage.playerControllerId, out playerController);
            if (playerController != null)
            {
                netMsg.conn.RemovePlayerController(s_RemovePlayerMessage.playerControllerId);
                Destroy(playerController.gameObject);
            }
            else if (LogFilter.logError)
            {
                Debug.LogError("Received remove player message but could not find the player ID: " + s_RemovePlayerMessage.playerControllerId);
            }
        }

        public static void RegisterHandler(short msgType, NetworkMessageDelegate handler)
        {
            instance.m_MessageHandlers.RegisterHandler(msgType, handler);
        }

        public static void RemoveExternalConnection(int connectionId)
        {
            instance.RemoveExternalConnectionInternal(connectionId);
        }

        private bool RemoveExternalConnectionInternal(int connectionId)
        {
            if (!s_ExternalConnections.Contains(connectionId))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("RemoveExternalConnection failed, no connection for id:" + connectionId);
                }
                return false;
            }
            if (LogFilter.logDebug)
            {
                Debug.Log("RemoveExternalConnection external connection " + connectionId);
            }
            this.m_Connections.Remove(connectionId);
            return true;
        }

        public static bool ReplacePlayerForConnection(NetworkConnection conn, GameObject player, short playerControllerId)
        {
            return instance.InternalReplacePlayerForConnection(conn, player, playerControllerId);
        }

        public static bool ReplacePlayerForConnection(NetworkConnection conn, GameObject player, short playerControllerId, NetworkHash128 assetId)
        {
            NetworkIdentity identity;
            if (GetNetworkIdentity(player, out identity))
            {
                identity.SetDynamicAssetId(assetId);
            }
            return instance.InternalReplacePlayerForConnection(conn, player, playerControllerId);
        }

        public static void Reset()
        {
            NetworkDetailStats.ResetAll();
            NetworkTransport.Shutdown();
            NetworkTransport.Init();
            s_NetworkConnectionClass = typeof(NetworkConnection);
            s_Instance = null;
            s_Active = false;
            s_LocalClientActive = false;
            s_ExternalConnections = new HashSet<int>();
        }

        public static void ResetConnectionStats()
        {
            ConnectionArray connections = instance.m_Connections;
            for (int i = connections.LocalIndex; i < connections.Count; i++)
            {
                NetworkConnection connection = connections.Get(i);
                if (connection != null)
                {
                    connection.ResetStats();
                }
            }
        }

        public static bool SendByChannelToAll(short msgType, MessageBase msg, int channelId)
        {
            if (LogFilter.logDev)
            {
                Debug.Log("Server.SendByChannelToAll id:" + msgType);
            }
            ConnectionArray connections = instance.m_Connections;
            bool flag = true;
            for (int i = connections.LocalIndex; i < connections.Count; i++)
            {
                NetworkConnection connection = connections.Get(i);
                if (connection != null)
                {
                    flag &= connection.SendByChannel(msgType, msg, channelId);
                }
            }
            return flag;
        }

        public static bool SendByChannelToReady(GameObject contextObj, short msgType, MessageBase msg, int channelId)
        {
            if (LogFilter.logDev)
            {
                Debug.Log("Server.SendByChannelToReady msgType:" + msgType);
            }
            if (contextObj == null)
            {
                for (int j = s_Instance.m_Connections.LocalIndex; j < s_Instance.m_Connections.Count; j++)
                {
                    NetworkConnection connection = s_Instance.m_Connections.Get(j);
                    if ((connection != null) && connection.isReady)
                    {
                        connection.SendByChannel(msgType, msg, channelId);
                    }
                }
                return true;
            }
            bool flag = true;
            NetworkIdentity component = contextObj.GetComponent<NetworkIdentity>();
            int count = component.observers.Count;
            for (int i = 0; i < count; i++)
            {
                NetworkConnection connection2 = component.observers[i];
                if (connection2.isReady)
                {
                    flag &= connection2.SendByChannel(msgType, msg, channelId);
                }
            }
            return flag;
        }

        public static void SendBytesToPlayer(GameObject player, byte[] buffer, int numBytes, int channelId)
        {
            NetworkConnection connection;
            if (instance.m_Connections.ContainsPlayer(player, out connection))
            {
                connection.SendBytes(buffer, numBytes, channelId);
            }
        }

        public static void SendBytesToReady(GameObject contextObj, byte[] buffer, int numBytes, int channelId)
        {
            if (contextObj == null)
            {
                for (int i = s_Instance.m_Connections.LocalIndex; i < s_Instance.m_Connections.Count; i++)
                {
                    NetworkConnection connection = s_Instance.m_Connections.Get(i);
                    if ((connection != null) && connection.isReady)
                    {
                        connection.SendBytes(buffer, numBytes, channelId);
                    }
                }
            }
            else
            {
                NetworkIdentity component = contextObj.GetComponent<NetworkIdentity>();
                try
                {
                    int count = component.observers.Count;
                    for (int j = 0; j < count; j++)
                    {
                        NetworkConnection connection2 = component.observers[j];
                        if (connection2.isReady)
                        {
                            connection2.SendBytes(buffer, numBytes, channelId);
                        }
                    }
                }
                catch (NullReferenceException)
                {
                    if (LogFilter.logWarn)
                    {
                        Debug.LogWarning("SendBytesToReady object " + contextObj + " has not been spawned");
                    }
                }
            }
        }

        private static void SendCrc(NetworkConnection targetConnection)
        {
            if ((NetworkCRC.singleton != null) && NetworkCRC.scriptCRCCheck)
            {
                CRCMessage msg = new CRCMessage();
                List<CRCMessageEntry> list = new List<CRCMessageEntry>();
                foreach (string str in NetworkCRC.singleton.scripts.Keys)
                {
                    CRCMessageEntry item = new CRCMessageEntry {
                        name = str,
                        channel = (byte) NetworkCRC.singleton.scripts[str]
                    };
                    list.Add(item);
                }
                msg.scripts = list.ToArray();
                targetConnection.Send(14, msg);
            }
        }

        public void SendNetworkInfo(NetworkConnection targetConnection)
        {
            PeerListMessage msg = new PeerListMessage();
            List<PeerInfoMessage> list = new List<PeerInfoMessage>();
            for (int i = 0; i < this.m_Connections.Count; i++)
            {
                NetworkConnection connection = this.m_Connections.Get(i);
                if (connection != null)
                {
                    string str;
                    int num2;
                    NetworkID kid;
                    NodeID eid;
                    byte num3;
                    PeerInfoMessage item = new PeerInfoMessage();
                    NetworkTransport.GetConnectionInfo(this.m_ServerId, connection.connectionId, out str, out num2, out kid, out eid, out num3);
                    item.connectionId = connection.connectionId;
                    item.address = str;
                    item.port = num2;
                    item.isHost = false;
                    item.isYou = connection == targetConnection;
                    list.Add(item);
                }
            }
            if (localClientActive)
            {
                PeerInfoMessage message3 = new PeerInfoMessage {
                    address = "HOST",
                    port = this.m_ServerPort,
                    connectionId = 0,
                    isHost = true,
                    isYou = false
                };
                list.Add(message3);
            }
            msg.peers = list.ToArray();
            for (int j = 0; j < this.m_Connections.Count; j++)
            {
                NetworkConnection connection2 = this.m_Connections.Get(j);
                if (connection2 != null)
                {
                    connection2.Send(11, msg);
                }
            }
        }

        internal void SendSpawnMessage(NetworkIdentity uv, NetworkConnection conn)
        {
            if (!uv.serverOnly)
            {
                if (uv.sceneId.IsEmpty())
                {
                    ObjectSpawnMessage msg = new ObjectSpawnMessage {
                        netId = uv.netId,
                        assetId = uv.assetId,
                        position = uv.transform.position
                    };
                    NetworkWriter writer = new NetworkWriter();
                    uv.UNetSerializeAllVars(writer);
                    if (writer.Position > 0)
                    {
                        msg.payload = writer.ToArray();
                    }
                    if (conn != null)
                    {
                        conn.Send(3, msg);
                    }
                    else
                    {
                        SendToReady(uv.gameObject, 3, msg);
                    }
                    NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Outgoing, 3, uv.assetId.ToString(), 1);
                }
                else
                {
                    ObjectSpawnSceneMessage message2 = new ObjectSpawnSceneMessage {
                        netId = uv.netId,
                        sceneId = uv.sceneId,
                        position = uv.transform.position
                    };
                    NetworkWriter writer2 = new NetworkWriter();
                    uv.UNetSerializeAllVars(writer2);
                    if (writer2.Position > 0)
                    {
                        message2.payload = writer2.ToArray();
                    }
                    if (conn != null)
                    {
                        conn.Send(10, message2);
                    }
                    else
                    {
                        SendToReady(uv.gameObject, 3, message2);
                    }
                    NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Outgoing, 10, "sceneId", 1);
                }
            }
        }

        public static bool SendToAll(short msgType, MessageBase msg)
        {
            if (LogFilter.logDev)
            {
                Debug.Log("Server.SendToAll msgType:" + msgType);
            }
            ConnectionArray connections = instance.m_Connections;
            bool flag = true;
            for (int i = connections.LocalIndex; i < connections.Count; i++)
            {
                NetworkConnection connection = connections.Get(i);
                if (connection != null)
                {
                    flag &= connection.Send(msgType, msg);
                }
            }
            return flag;
        }

        public static void SendToClient(int connectionId, short msgType, MessageBase msg)
        {
            NetworkConnection connection = instance.m_Connections.Get(connectionId);
            if (connection != null)
            {
                connection.Send(msgType, msg);
            }
            else if (LogFilter.logError)
            {
                Debug.LogError("Failed to send message to connection ID '" + connectionId + ", not found in connection list");
            }
        }

        public static void SendToClientOfPlayer(GameObject player, short msgType, MessageBase msg)
        {
            NetworkConnection connection;
            if (instance.m_Connections.ContainsPlayer(player, out connection))
            {
                connection.Send(msgType, msg);
            }
            else if (LogFilter.logError)
            {
                Debug.LogError("Failed to send message to player object '" + player.name + ", not found in connection list");
            }
        }

        private static bool SendToObservers(GameObject contextObj, short msgType, MessageBase msg)
        {
            if (LogFilter.logDev)
            {
                Debug.Log("Server.SendToObservers id:" + msgType);
            }
            bool flag = true;
            NetworkIdentity component = contextObj.GetComponent<NetworkIdentity>();
            if ((component == null) || (component.observers == null))
            {
                return false;
            }
            int count = component.observers.Count;
            for (int i = 0; i < count; i++)
            {
                NetworkConnection connection = component.observers[i];
                flag &= connection.Send(msgType, msg);
            }
            return flag;
        }

        public static bool SendToReady(GameObject contextObj, short msgType, MessageBase msg)
        {
            if (LogFilter.logDev)
            {
                Debug.Log("Server.SendToReady id:" + msgType);
            }
            if (contextObj == null)
            {
                for (int j = s_Instance.m_Connections.LocalIndex; j < s_Instance.m_Connections.Count; j++)
                {
                    NetworkConnection connection = s_Instance.m_Connections.Get(j);
                    if ((connection != null) && connection.isReady)
                    {
                        connection.Send(msgType, msg);
                    }
                }
                return true;
            }
            bool flag = true;
            NetworkIdentity component = contextObj.GetComponent<NetworkIdentity>();
            if ((component == null) || (component.observers == null))
            {
                return false;
            }
            int count = component.observers.Count;
            for (int i = 0; i < count; i++)
            {
                NetworkConnection connection2 = component.observers[i];
                if (connection2.isReady)
                {
                    flag &= connection2.Send(msgType, msg);
                }
            }
            return flag;
        }

        public static bool SendUnreliableToAll(short msgType, MessageBase msg)
        {
            if (LogFilter.logDev)
            {
                Debug.Log("Server.SendUnreliableToAll msgType:" + msgType);
            }
            ConnectionArray connections = instance.m_Connections;
            bool flag = true;
            for (int i = connections.LocalIndex; i < connections.Count; i++)
            {
                NetworkConnection connection = connections.Get(i);
                if (connection != null)
                {
                    flag &= connection.SendUnreliable(msgType, msg);
                }
            }
            return flag;
        }

        public static bool SendUnreliableToReady(GameObject contextObj, short msgType, MessageBase msg)
        {
            if (LogFilter.logDev)
            {
                Debug.Log("Server.SendUnreliableToReady id:" + msgType);
            }
            if (contextObj == null)
            {
                for (int j = s_Instance.m_Connections.LocalIndex; j < s_Instance.m_Connections.Count; j++)
                {
                    NetworkConnection connection = s_Instance.m_Connections.Get(j);
                    if ((connection != null) && connection.isReady)
                    {
                        connection.SendUnreliable(msgType, msg);
                    }
                }
                return true;
            }
            bool flag = true;
            NetworkIdentity component = contextObj.GetComponent<NetworkIdentity>();
            int count = component.observers.Count;
            for (int i = 0; i < count; i++)
            {
                NetworkConnection connection2 = component.observers[i];
                if (connection2.isReady)
                {
                    flag &= connection2.SendUnreliable(msgType, msg);
                }
            }
            return flag;
        }

        public static void SendWriterToReady(GameObject contextObj, NetworkWriter writer, int channelId)
        {
            if (writer.AsArraySegment().Count > 0x7fff)
            {
                throw new UnityException("NetworkWriter used buffer is too big!");
            }
            SendBytesToReady(contextObj, writer.AsArraySegment().Array, writer.AsArraySegment().Count, channelId);
        }

        public static void SetAllClientsNotReady()
        {
            ConnectionArray connections = instance.m_Connections;
            for (int i = connections.LocalIndex; i < connections.Count; i++)
            {
                NetworkConnection conn = connections.Get(i);
                if (conn != null)
                {
                    SetClientNotReady(conn);
                }
            }
        }

        public static void SetClientNotReady(NetworkConnection conn)
        {
            instance.InternalSetClientNotReady(conn);
        }

        public static void SetClientReady(NetworkConnection conn)
        {
            instance.SetClientReadyInternal(conn);
        }

        internal void SetClientReadyInternal(NetworkConnection conn)
        {
            if (conn.isReady)
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log("SetClientReady conn " + conn.connectionId + " already ready");
                }
            }
            else
            {
                if ((conn.playerControllers.Count == 0) && LogFilter.logDebug)
                {
                    Debug.LogWarning("Ready with no player object");
                }
                conn.isReady = true;
                if (conn is ULocalConnectionToClient)
                {
                    if (LogFilter.logDev)
                    {
                        Debug.Log("NetworkServer Ready handling ULocalConnectionToClient");
                    }
                    foreach (NetworkIdentity identity in objects.Values)
                    {
                        if ((identity != null) && (identity.gameObject != null))
                        {
                            if (identity.OnCheckObserver(conn))
                            {
                                identity.AddObserver(conn);
                            }
                            if (!identity.isClient)
                            {
                                if (LogFilter.logDev)
                                {
                                    Debug.Log("LocalClient.SetSpawnObject calling OnStartClient");
                                }
                                identity.OnStartClient();
                            }
                        }
                    }
                }
                else
                {
                    ObjectSpawnFinishedMessage msg = new ObjectSpawnFinishedMessage {
                        state = 0
                    };
                    conn.Send(12, msg);
                    foreach (NetworkIdentity identity2 in objects.Values)
                    {
                        if (identity2 == null)
                        {
                            if (LogFilter.logWarn)
                            {
                                Debug.LogWarning("Invalid object found in server local object list (null NetworkIdentity).");
                            }
                        }
                        else
                        {
                            if (LogFilter.logDebug)
                            {
                                Debug.Log(string.Concat(new object[] { "Sending spawn message for current server objects name='", identity2.gameObject.name, "' netId=", identity2.netId }));
                            }
                            if (identity2.OnCheckObserver(conn))
                            {
                                identity2.AddObserver(conn);
                            }
                        }
                    }
                    msg.state = 1;
                    conn.Send(12, msg);
                }
            }
        }

        internal void SetLocalObjectOnServer(NetworkInstanceId netId, GameObject obj)
        {
            if (LogFilter.logDev)
            {
                Debug.Log(string.Concat(new object[] { "SetLocalObjectOnServer ", netId, " ", obj }));
            }
            s_NetworkScene.SetLocalObject(netId, obj, false, true);
        }

        public static void SetNetworkConnectionClass<T>() where T: NetworkConnection
        {
            s_NetworkConnectionClass = typeof(T);
        }

        private bool SetupLocalPlayerForConnection(NetworkConnection conn, NetworkIdentity uv, PlayerController newPlayerController)
        {
            if (LogFilter.logDev)
            {
                Debug.Log("NetworkServer SetupLocalPlayerForConnection netID:" + uv.netId);
            }
            ULocalConnectionToClient client = conn as ULocalConnectionToClient;
            if (client == null)
            {
                return false;
            }
            if (LogFilter.logDev)
            {
                Debug.Log("NetworkServer AddPlayer handling ULocalConnectionToClient");
            }
            if (uv.netId.IsEmpty())
            {
                uv.OnStartServer();
            }
            uv.RebuildObservers(true);
            this.SendSpawnMessage(uv, null);
            client.localClient.AddLocalPlayer(newPlayerController);
            uv.SetLocalPlayer(newPlayerController.playerControllerId);
            return true;
        }

        internal static void ShowForConnection(NetworkIdentity uv, NetworkConnection conn)
        {
            if (conn.isReady)
            {
                instance.SendSpawnMessage(uv, conn);
            }
        }

        public static void Shutdown()
        {
            if (s_Instance != null)
            {
                s_Instance.InternalDisconnectAll();
                if (s_Instance.m_ServerId != -1)
                {
                    NetworkTransport.RemoveHost(s_Instance.m_ServerId);
                    s_Instance.m_ServerId = -1;
                }
                s_Instance = null;
            }
            s_ExternalConnections = new HashSet<int>();
            s_Active = false;
            s_LocalClientActive = false;
        }

        public static void Spawn(GameObject obj)
        {
            instance.SpawnObject(obj);
        }

        public static void Spawn(GameObject obj, NetworkHash128 assetId)
        {
            NetworkIdentity identity;
            if (GetNetworkIdentity(obj, out identity))
            {
                identity.SetDynamicAssetId(assetId);
            }
            instance.SpawnObject(obj);
        }

        internal void SpawnObject(GameObject obj)
        {
            if (!active)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("SpawnObject for " + obj + ", NetworkServer is not active. Cannot spawn objects without an active server.");
                }
            }
            else
            {
                NetworkIdentity identity;
                if (!GetNetworkIdentity(obj, out identity))
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError(string.Concat(new object[] { "SpawnObject ", obj, " has no NetworkIdentity. Please add a NetworkIdentity to ", obj }));
                    }
                }
                else
                {
                    identity.OnStartServer();
                    if (LogFilter.logDebug)
                    {
                        Debug.Log(string.Concat(new object[] { "SpawnObject instance ID ", identity.netId, " asset ID ", identity.assetId }));
                    }
                    identity.RebuildObservers(true);
                }
            }
        }

        public static bool SpawnObjects()
        {
            if (active)
            {
                NetworkIdentity[] identityArray = Resources.FindObjectsOfTypeAll<NetworkIdentity>();
                foreach (NetworkIdentity identity in identityArray)
                {
                    if (((identity.gameObject.hideFlags != HideFlags.NotEditable) && (identity.gameObject.hideFlags != HideFlags.HideAndDontSave)) && !identity.sceneId.IsEmpty())
                    {
                        if (LogFilter.logDebug)
                        {
                            Debug.Log(string.Concat(new object[] { "SpawnObjects sceneId:", identity.sceneId, " name:", identity.gameObject.name }));
                        }
                        identity.gameObject.SetActive(true);
                    }
                }
                foreach (NetworkIdentity identity2 in identityArray)
                {
                    if ((((identity2.gameObject.hideFlags != HideFlags.NotEditable) && (identity2.gameObject.hideFlags != HideFlags.HideAndDontSave)) && (!identity2.sceneId.IsEmpty() && !identity2.isServer)) && (identity2.gameObject != null))
                    {
                        Spawn(identity2.gameObject);
                    }
                }
            }
            return true;
        }

        public static bool SpawnWithClientAuthority(GameObject obj, GameObject player)
        {
            NetworkIdentity component = player.GetComponent<NetworkIdentity>();
            if (component == null)
            {
                Debug.LogError("SpawnWithClientAuthority player object has no NetworkIdentity");
                return false;
            }
            if (component.connectionToClient == null)
            {
                Debug.LogError("SpawnWithClientAuthority player object is not a player.");
                return false;
            }
            return SpawnWithClientAuthority(obj, component.connectionToClient);
        }

        public static bool SpawnWithClientAuthority(GameObject obj, NetworkConnection conn)
        {
            Spawn(obj);
            NetworkIdentity component = obj.GetComponent<NetworkIdentity>();
            return (((component != null) && component.isServer) && component.AssignClientAuthority(conn));
        }

        public static bool SpawnWithClientAuthority(GameObject obj, NetworkHash128 assetId, NetworkConnection conn)
        {
            Spawn(obj, assetId);
            NetworkIdentity component = obj.GetComponent<NetworkIdentity>();
            return (((component != null) && component.isServer) && component.AssignClientAuthority(conn));
        }

        public static void UnregisterHandler(short msgType)
        {
            instance.m_MessageHandlers.UnregisterHandler(msgType);
        }

        public static void UnSpawn(GameObject obj)
        {
            UnSpawnObject(obj);
        }

        private static void UnSpawnObject(GameObject obj)
        {
            if (obj == null)
            {
                if (LogFilter.logDev)
                {
                    Debug.Log("NetworkServer UnspawnObject is null");
                }
            }
            else
            {
                NetworkIdentity identity;
                if (GetNetworkIdentity(obj, out identity))
                {
                    UnSpawnObject(identity);
                }
            }
        }

        private static void UnSpawnObject(NetworkIdentity uv)
        {
            DestroyObject(uv, false);
        }

        internal static void Update()
        {
            if (s_Instance != null)
            {
                s_Instance.InternalUpdate();
            }
        }

        internal void UpdateServerObjects()
        {
            foreach (NetworkIdentity identity in objects.Values)
            {
                try
                {
                    identity.UNetUpdate();
                }
                catch (NullReferenceException)
                {
                }
            }
            if ((this.m_RemoveListCount++ % 100) == 0)
            {
                this.CheckForNullObjects();
            }
        }

        public static bool active
        {
            get
            {
                return s_Active;
            }
        }

        public static List<NetworkConnection> connections
        {
            get
            {
                return instance.m_Connections.connections;
            }
        }

        public static Dictionary<short, NetworkMessageDelegate> handlers
        {
            get
            {
                return instance.m_MessageHandlers.GetHandlers();
            }
        }

        public static HostTopology hostTopology
        {
            get
            {
                return instance.m_HostTopology;
            }
        }

        internal static NetworkServer instance
        {
            get
            {
                if (s_Instance == null)
                {
                    object obj2 = s_Sync;
                    lock (obj2)
                    {
                        if (s_Instance == null)
                        {
                            s_Instance = new NetworkServer();
                        }
                    }
                }
                return s_Instance;
            }
        }

        public static bool localClientActive
        {
            get
            {
                return s_LocalClientActive;
            }
        }

        public static List<NetworkConnection> localConnections
        {
            get
            {
                return instance.m_Connections.localConnections;
            }
        }

        public static float maxDelay
        {
            get
            {
                return instance.m_MaxDelay;
            }
            set
            {
                instance.InternalSetMaxDelay(value);
            }
        }

        public static System.Type networkConnectionClass
        {
            get
            {
                return s_NetworkConnectionClass;
            }
        }

        public static int numChannels
        {
            get
            {
                return instance.m_HostTopology.DefaultConfig.ChannelCount;
            }
        }

        public static Dictionary<NetworkInstanceId, NetworkIdentity> objects
        {
            get
            {
                return s_NetworkScene.localObjects;
            }
        }

        public static bool sendPeerInfo
        {
            get
            {
                return instance.m_SendPeerInfo;
            }
            set
            {
                instance.m_SendPeerInfo = value;
            }
        }
    }
}

