namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Networking.NetworkSystem;

    public class ClientScene
    {
        private static ClientAuthorityMessage s_ClientAuthorityMessage = new ClientAuthorityMessage();
        private static bool s_IsReady;
        private static bool s_IsSpawnFinished;
        private static List<PlayerController> s_LocalPlayers = new List<PlayerController>();
        private static NetworkScene s_NetworkScene = new NetworkScene();
        private static ObjectDestroyMessage s_ObjectDestroyMessage = new ObjectDestroyMessage();
        private static ObjectSpawnFinishedMessage s_ObjectSpawnFinishedMessage = new ObjectSpawnFinishedMessage();
        private static ObjectSpawnMessage s_ObjectSpawnMessage = new ObjectSpawnMessage();
        private static ObjectSpawnSceneMessage s_ObjectSpawnSceneMessage = new ObjectSpawnSceneMessage();
        private static OwnerMessage s_OwnerMessage = new OwnerMessage();
        private static List<PendingOwner> s_PendingOwnerIds = new List<PendingOwner>();
        private static NetworkConnection s_ReadyConnection;
        private static Dictionary<NetworkSceneId, NetworkIdentity> s_SpawnableObjects;

        public static bool AddPlayer(short playerControllerId)
        {
            return AddPlayer(null, playerControllerId);
        }

        public static bool AddPlayer(NetworkConnection readyConn, short playerControllerId)
        {
            return AddPlayer(readyConn, playerControllerId, null);
        }

        public static bool AddPlayer(NetworkConnection readyConn, short playerControllerId, MessageBase extraMessage)
        {
            PlayerController controller;
            if (playerControllerId < 0)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("ClientScene::AddPlayer: playerControllerId of " + playerControllerId + " is negative");
                }
                return false;
            }
            if (playerControllerId > 0x20)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError(string.Concat(new object[] { "ClientScene::AddPlayer: playerControllerId of ", playerControllerId, " is too high, max is ", 0x20 }));
                }
                return false;
            }
            if ((playerControllerId > 0x10) && LogFilter.logWarn)
            {
                Debug.LogWarning("ClientScene::AddPlayer: playerControllerId of " + playerControllerId + " is unusually high");
            }
            while (playerControllerId >= s_LocalPlayers.Count)
            {
                s_LocalPlayers.Add(new PlayerController());
            }
            if (readyConn == null)
            {
                if (!s_IsReady)
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError("Must call AddPlayer() with a connection the first time to become ready.");
                    }
                    return false;
                }
            }
            else
            {
                s_IsReady = true;
                s_ReadyConnection = readyConn;
            }
            if ((s_ReadyConnection.GetPlayerController(playerControllerId, out controller) && controller.IsValid) && (controller.gameObject != null))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("ClientScene::AddPlayer: playerControllerId of " + playerControllerId + " already in use.");
                }
                return false;
            }
            if (LogFilter.logDebug)
            {
                Debug.Log(string.Concat(new object[] { "ClientScene::AddPlayer() for ID ", playerControllerId, " called with connection [", s_ReadyConnection, "]" }));
            }
            AddPlayerMessage msg = new AddPlayerMessage {
                playerControllerId = playerControllerId
            };
            if (extraMessage != null)
            {
                NetworkWriter writer = new NetworkWriter();
                extraMessage.Serialize(writer);
                msg.msgData = writer.ToArray();
                msg.msgSize = writer.Position;
            }
            s_ReadyConnection.Send(0x25, msg);
            return true;
        }

        private static void ApplySpawnPayload(NetworkIdentity uv, Vector3 position, byte[] payload, NetworkInstanceId netId, GameObject newGameObject)
        {
            uv.transform.position = position;
            if ((payload != null) && (payload.Length > 0))
            {
                NetworkReader reader = new NetworkReader(payload);
                uv.OnUpdateVars(reader, true);
            }
            if (newGameObject != null)
            {
                newGameObject.SetActive(true);
                uv.SetNetworkInstanceId(netId);
                SetLocalObject(netId, newGameObject);
                if (s_IsSpawnFinished)
                {
                    uv.OnStartClient();
                    CheckForOwner(uv);
                }
            }
        }

        private static void CheckForOwner(NetworkIdentity uv)
        {
            for (int i = 0; i < s_PendingOwnerIds.Count; i++)
            {
                PendingOwner owner = s_PendingOwnerIds[i];
                if (owner.netId == uv.netId)
                {
                    uv.SetConnectionToServer(s_ReadyConnection);
                    uv.SetLocalPlayer(owner.playerControllerId);
                    if (LogFilter.logDev)
                    {
                        Debug.Log("ClientScene::OnOwnerMessage - player=" + uv.gameObject.name);
                    }
                    if (s_ReadyConnection.connectionId < 0)
                    {
                        if (LogFilter.logError)
                        {
                            Debug.LogError("Owner message received on a local client.");
                        }
                        return;
                    }
                    InternalAddPlayer(uv, owner.playerControllerId);
                    s_PendingOwnerIds.RemoveAt(i);
                    break;
                }
            }
        }

        public static void ClearSpawners()
        {
            NetworkScene.ClearSpawners();
        }

        public static NetworkClient ConnectLocalServer()
        {
            LocalClient client = new LocalClient();
            NetworkServer.instance.ActivateLocalClientScene();
            client.InternalConnectLocalServer();
            return client;
        }

        public static void DestroyAllClientObjects()
        {
            s_NetworkScene.DestroyAllClientObjects();
        }

        public static GameObject FindLocalObject(NetworkInstanceId netId)
        {
            return s_NetworkScene.FindLocalObject(netId);
        }

        internal static bool GetPlayerController(short playerControllerId, out PlayerController player)
        {
            player = null;
            if (playerControllerId >= localPlayers.Count)
            {
                if (LogFilter.logWarn)
                {
                    Debug.Log("ClientScene::GetPlayer: no local player found for: " + playerControllerId);
                }
                return false;
            }
            if (localPlayers[playerControllerId] == null)
            {
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning("ClientScene::GetPlayer: local player is null for: " + playerControllerId);
                }
                return false;
            }
            player = localPlayers[playerControllerId];
            return (player.gameObject != null);
        }

        internal static string GetStringForAssetId(NetworkHash128 assetId)
        {
            GameObject obj2;
            SpawnDelegate delegate2;
            if (NetworkScene.GetPrefab(assetId, out obj2))
            {
                return obj2.name;
            }
            if (NetworkScene.GetSpawnHandler(assetId, out delegate2))
            {
                return delegate2.Method.Name;
            }
            return "unknown";
        }

        internal static void HandleClientDisconnect(NetworkConnection conn)
        {
            if ((s_ReadyConnection == conn) && s_IsReady)
            {
                s_IsReady = false;
                s_ReadyConnection = null;
            }
        }

        internal static void InternalAddPlayer(NetworkIdentity view, short playerControllerId)
        {
            if (LogFilter.logDebug)
            {
                Debug.LogWarning("ClientScene::InternalAddPlayer: playerControllerId : " + playerControllerId);
            }
            if (playerControllerId >= s_LocalPlayers.Count)
            {
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning("ClientScene::InternalAddPlayer: playerControllerId higher than expected: " + playerControllerId);
                }
                while (playerControllerId >= s_LocalPlayers.Count)
                {
                    s_LocalPlayers.Add(new PlayerController());
                }
            }
            PlayerController player = new PlayerController {
                gameObject = view.gameObject,
                playerControllerId = playerControllerId,
                unetView = view
            };
            s_LocalPlayers[playerControllerId] = player;
            s_ReadyConnection.SetPlayerController(player);
        }

        private static void OnClientAuthority(NetworkMessage netMsg)
        {
            NetworkIdentity identity;
            netMsg.ReadMessage<ClientAuthorityMessage>(s_ClientAuthorityMessage);
            if (LogFilter.logDebug)
            {
                Debug.Log(string.Concat(new object[] { "ClientScene::OnClientAuthority for  connectionId=", netMsg.conn.connectionId, " netId: ", s_ClientAuthorityMessage.netId }));
            }
            if (s_NetworkScene.GetNetworkIdentity(s_ClientAuthorityMessage.netId, out identity))
            {
                identity.HandleClientAuthority(s_ClientAuthorityMessage.authority);
            }
        }

        private static void OnLocalClientObjectDestroy(NetworkMessage netMsg)
        {
            netMsg.ReadMessage<ObjectDestroyMessage>(s_ObjectDestroyMessage);
            if (LogFilter.logDebug)
            {
                Debug.Log("ClientScene::OnLocalObjectObjDestroy netId:" + s_ObjectDestroyMessage.netId);
            }
            s_NetworkScene.RemoveLocalObject(s_ObjectDestroyMessage.netId);
        }

        private static void OnLocalClientObjectHide(NetworkMessage netMsg)
        {
            NetworkIdentity identity;
            netMsg.ReadMessage<ObjectDestroyMessage>(s_ObjectDestroyMessage);
            if (LogFilter.logDebug)
            {
                Debug.Log("ClientScene::OnLocalObjectObjHide netId:" + s_ObjectDestroyMessage.netId);
            }
            if (s_NetworkScene.GetNetworkIdentity(s_ObjectDestroyMessage.netId, out identity))
            {
                identity.OnSetLocalVisibility(false);
            }
        }

        private static void OnLocalClientObjectSpawn(NetworkMessage netMsg)
        {
            NetworkIdentity identity;
            netMsg.ReadMessage<ObjectSpawnMessage>(s_ObjectSpawnMessage);
            if (s_NetworkScene.GetNetworkIdentity(s_ObjectSpawnMessage.netId, out identity))
            {
                identity.OnSetLocalVisibility(true);
            }
        }

        private static void OnLocalClientObjectSpawnScene(NetworkMessage netMsg)
        {
            NetworkIdentity identity;
            netMsg.ReadMessage<ObjectSpawnSceneMessage>(s_ObjectSpawnSceneMessage);
            if (s_NetworkScene.GetNetworkIdentity(s_ObjectSpawnSceneMessage.netId, out identity))
            {
                identity.OnSetLocalVisibility(true);
            }
        }

        private static void OnObjectDestroy(NetworkMessage netMsg)
        {
            NetworkIdentity identity;
            netMsg.ReadMessage<ObjectDestroyMessage>(s_ObjectDestroyMessage);
            if (LogFilter.logDebug)
            {
                Debug.Log("ClientScene::OnObjDestroy netId:" + s_ObjectDestroyMessage.netId);
            }
            if (s_NetworkScene.GetNetworkIdentity(s_ObjectDestroyMessage.netId, out identity))
            {
                NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Incoming, 1, GetStringForAssetId(identity.assetId), 1);
                identity.OnNetworkDestroy();
                if (!NetworkScene.InvokeUnSpawnHandler(identity.assetId, identity.gameObject))
                {
                    if (identity.sceneId.IsEmpty())
                    {
                        UnityEngine.Object.Destroy(identity.gameObject);
                    }
                    else
                    {
                        identity.gameObject.SetActive(false);
                        s_SpawnableObjects[identity.sceneId] = identity;
                    }
                }
                s_NetworkScene.RemoveLocalObject(s_ObjectDestroyMessage.netId);
            }
            else if (LogFilter.logDebug)
            {
                Debug.LogWarning("Did not find target for destroy message for " + s_ObjectDestroyMessage.netId);
            }
        }

        private static void OnObjectSpawn(NetworkMessage netMsg)
        {
            netMsg.ReadMessage<ObjectSpawnMessage>(s_ObjectSpawnMessage);
            if (!s_ObjectSpawnMessage.assetId.IsValid())
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("OnObjSpawn netId: " + s_ObjectSpawnMessage.netId + " has invalid asset Id");
                }
            }
            else
            {
                NetworkIdentity component;
                if (LogFilter.logDebug)
                {
                    Debug.Log(string.Concat(new object[] { "Client spawn handler instantiating [netId:", s_ObjectSpawnMessage.netId, " asset ID:", s_ObjectSpawnMessage.assetId, " pos:", s_ObjectSpawnMessage.position, "]" }));
                }
                NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Incoming, 3, GetStringForAssetId(s_ObjectSpawnMessage.assetId), 1);
                if (s_NetworkScene.GetNetworkIdentity(s_ObjectSpawnMessage.netId, out component))
                {
                    ApplySpawnPayload(component, s_ObjectSpawnMessage.position, s_ObjectSpawnMessage.payload, s_ObjectSpawnMessage.netId, null);
                }
                else
                {
                    GameObject obj2;
                    if (NetworkScene.GetPrefab(s_ObjectSpawnMessage.assetId, out obj2))
                    {
                        GameObject newGameObject = (GameObject) UnityEngine.Object.Instantiate(obj2, s_ObjectSpawnMessage.position, Quaternion.identity);
                        component = newGameObject.GetComponent<NetworkIdentity>();
                        if (component == null)
                        {
                            if (LogFilter.logError)
                            {
                                Debug.LogError("Client object spawned for " + s_ObjectSpawnMessage.assetId + " does not have a NetworkIdentity");
                            }
                        }
                        else
                        {
                            ApplySpawnPayload(component, s_ObjectSpawnMessage.position, s_ObjectSpawnMessage.payload, s_ObjectSpawnMessage.netId, newGameObject);
                        }
                    }
                    else
                    {
                        SpawnDelegate delegate2;
                        if (NetworkScene.GetSpawnHandler(s_ObjectSpawnMessage.assetId, out delegate2))
                        {
                            GameObject obj4 = delegate2(s_ObjectSpawnMessage.position, s_ObjectSpawnMessage.assetId);
                            if (obj4 == null)
                            {
                                if (LogFilter.logWarn)
                                {
                                    Debug.LogWarning("Client spawn handler for " + s_ObjectSpawnMessage.assetId + " returned null");
                                }
                            }
                            else
                            {
                                component = obj4.GetComponent<NetworkIdentity>();
                                if (component == null)
                                {
                                    if (LogFilter.logError)
                                    {
                                        Debug.LogError("Client object spawned for " + s_ObjectSpawnMessage.assetId + " does not have a network identity");
                                    }
                                }
                                else
                                {
                                    component.SetDynamicAssetId(s_ObjectSpawnMessage.assetId);
                                    ApplySpawnPayload(component, s_ObjectSpawnMessage.position, s_ObjectSpawnMessage.payload, s_ObjectSpawnMessage.netId, obj4);
                                }
                            }
                        }
                        else if (LogFilter.logError)
                        {
                            Debug.LogError(string.Concat(new object[] { "Failed to spawn server object, assetId=", s_ObjectSpawnMessage.assetId, " netId=", s_ObjectSpawnMessage.netId }));
                        }
                    }
                }
            }
        }

        private static void OnObjectSpawnFinished(NetworkMessage netMsg)
        {
            netMsg.ReadMessage<ObjectSpawnFinishedMessage>(s_ObjectSpawnFinishedMessage);
            if (LogFilter.logDebug)
            {
                Debug.Log("SpawnFinished:" + s_ObjectSpawnFinishedMessage.state);
            }
            if (s_ObjectSpawnFinishedMessage.state == 0)
            {
                PrepareToSpawnSceneObjects();
                s_IsSpawnFinished = false;
            }
            else
            {
                foreach (NetworkIdentity identity in objects.Values)
                {
                    if (!identity.isClient)
                    {
                        identity.OnStartClient();
                        CheckForOwner(identity);
                    }
                }
                s_IsSpawnFinished = true;
            }
        }

        private static void OnObjectSpawnScene(NetworkMessage netMsg)
        {
            NetworkIdentity identity;
            netMsg.ReadMessage<ObjectSpawnSceneMessage>(s_ObjectSpawnSceneMessage);
            if (LogFilter.logDebug)
            {
                Debug.Log(string.Concat(new object[] { "Client spawn scene handler instantiating [netId:", s_ObjectSpawnSceneMessage.netId, " sceneId:", s_ObjectSpawnSceneMessage.sceneId, " pos:", s_ObjectSpawnSceneMessage.position }));
            }
            NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Incoming, 10, "sceneId", 1);
            if (s_NetworkScene.GetNetworkIdentity(s_ObjectSpawnSceneMessage.netId, out identity))
            {
                ApplySpawnPayload(identity, s_ObjectSpawnSceneMessage.position, s_ObjectSpawnSceneMessage.payload, s_ObjectSpawnSceneMessage.netId, identity.gameObject);
            }
            else
            {
                NetworkIdentity uv = SpawnSceneObject(s_ObjectSpawnSceneMessage.sceneId);
                if (uv == null)
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError("Spawn scene object not found for " + s_ObjectSpawnSceneMessage.sceneId);
                    }
                }
                else
                {
                    if (LogFilter.logDebug)
                    {
                        Debug.Log(string.Concat(new object[] { "Client spawn for [netId:", s_ObjectSpawnSceneMessage.netId, "] [sceneId:", s_ObjectSpawnSceneMessage.sceneId, "] obj:", uv.gameObject.name }));
                    }
                    ApplySpawnPayload(uv, s_ObjectSpawnSceneMessage.position, s_ObjectSpawnSceneMessage.payload, s_ObjectSpawnSceneMessage.netId, uv.gameObject);
                }
            }
        }

        private static void OnOwnerMessage(NetworkMessage netMsg)
        {
            PlayerController controller;
            NetworkIdentity identity;
            netMsg.ReadMessage<OwnerMessage>(s_OwnerMessage);
            if (LogFilter.logDebug)
            {
                Debug.Log(string.Concat(new object[] { "ClientScene::OnOwnerMessage - connectionId=", netMsg.conn.connectionId, " netId: ", s_OwnerMessage.netId }));
            }
            if (netMsg.conn.GetPlayerController(s_OwnerMessage.playerControllerId, out controller))
            {
                controller.unetView.SetNotLocalPlayer();
            }
            if (s_NetworkScene.GetNetworkIdentity(s_OwnerMessage.netId, out identity))
            {
                identity.SetConnectionToServer(netMsg.conn);
                identity.SetLocalPlayer(s_OwnerMessage.playerControllerId);
                InternalAddPlayer(identity, s_OwnerMessage.playerControllerId);
            }
            else
            {
                PendingOwner item = new PendingOwner {
                    netId = s_OwnerMessage.netId,
                    playerControllerId = s_OwnerMessage.playerControllerId
                };
                s_PendingOwnerIds.Add(item);
            }
        }

        private static void OnRPCMessage(NetworkMessage netMsg)
        {
            NetworkIdentity identity;
            int cmdHash = (int) netMsg.reader.ReadPackedUInt32();
            NetworkInstanceId netId = netMsg.reader.ReadNetworkId();
            if (LogFilter.logDebug)
            {
                Debug.Log(string.Concat(new object[] { "ClientScene::OnRPCMessage hash:", cmdHash, " netId:", netId }));
            }
            if (s_NetworkScene.GetNetworkIdentity(netId, out identity))
            {
                identity.HandleRPC(cmdHash, netMsg.reader);
            }
            else if (LogFilter.logWarn)
            {
                Debug.LogWarning("Did not find target for RPC message for " + netId);
            }
        }

        private static void OnSyncEventMessage(NetworkMessage netMsg)
        {
            NetworkIdentity identity;
            int cmdHash = (int) netMsg.reader.ReadPackedUInt32();
            NetworkInstanceId netId = netMsg.reader.ReadNetworkId();
            if (LogFilter.logDebug)
            {
                Debug.Log("ClientScene::OnSyncEventMessage " + netId);
            }
            if (s_NetworkScene.GetNetworkIdentity(netId, out identity))
            {
                identity.HandleSyncEvent(cmdHash, netMsg.reader);
            }
            else if (LogFilter.logWarn)
            {
                Debug.LogWarning("Did not find target for SyncEvent message for " + netId);
            }
            NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Outgoing, 7, NetworkBehaviour.GetCmdHashHandlerName(cmdHash), 1);
        }

        private static void OnSyncListMessage(NetworkMessage netMsg)
        {
            NetworkIdentity identity;
            NetworkInstanceId netId = netMsg.reader.ReadNetworkId();
            int cmdHash = (int) netMsg.reader.ReadPackedUInt32();
            if (LogFilter.logDebug)
            {
                Debug.Log("ClientScene::OnSyncListMessage " + netId);
            }
            if (s_NetworkScene.GetNetworkIdentity(netId, out identity))
            {
                identity.HandleSyncList(cmdHash, netMsg.reader);
            }
            else if (LogFilter.logWarn)
            {
                Debug.LogWarning("Did not find target for SyncList message for " + netId);
            }
            NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Outgoing, 9, NetworkBehaviour.GetCmdHashHandlerName(cmdHash), 1);
        }

        private static void OnUpdateVarsMessage(NetworkMessage netMsg)
        {
            NetworkIdentity identity;
            NetworkInstanceId netId = netMsg.reader.ReadNetworkId();
            if (LogFilter.logDev)
            {
                Debug.Log(string.Concat(new object[] { "ClientScene::OnUpdateVarsMessage ", netId, " channel:", netMsg.channelId }));
            }
            if (s_NetworkScene.GetNetworkIdentity(netId, out identity))
            {
                identity.OnUpdateVars(netMsg.reader, false);
            }
            else if (LogFilter.logWarn)
            {
                Debug.LogWarning("Did not find target for sync message for " + netId);
            }
        }

        internal static void PrepareToSpawnSceneObjects()
        {
            s_SpawnableObjects = new Dictionary<NetworkSceneId, NetworkIdentity>();
            foreach (NetworkIdentity identity in Resources.FindObjectsOfTypeAll<NetworkIdentity>())
            {
                if ((!identity.gameObject.activeSelf && (identity.gameObject.hideFlags != HideFlags.NotEditable)) && ((identity.gameObject.hideFlags != HideFlags.HideAndDontSave) && !identity.sceneId.IsEmpty()))
                {
                    s_SpawnableObjects[identity.sceneId] = identity;
                    if (LogFilter.logDebug)
                    {
                        Debug.Log("ClientScene::PrepareSpawnObjects sceneId:" + identity.sceneId);
                    }
                }
            }
        }

        public static bool Ready(NetworkConnection conn)
        {
            if (s_IsReady)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("A connection has already been set as ready. There can only be one.");
                }
                return false;
            }
            if (LogFilter.logDebug)
            {
                Debug.Log("ClientScene::Ready() called with connection [" + conn + "]");
            }
            if (conn != null)
            {
                ReadyMessage msg = new ReadyMessage();
                conn.Send(0x23, msg);
                s_IsReady = true;
                s_ReadyConnection = conn;
                s_ReadyConnection.isReady = true;
                return true;
            }
            if (LogFilter.logError)
            {
                Debug.LogError("Ready() called with invalid connection object: conn=null");
            }
            return false;
        }

        public static void RegisterPrefab(GameObject prefab)
        {
            NetworkScene.RegisterPrefab(prefab);
        }

        public static void RegisterPrefab(GameObject prefab, SpawnDelegate spawnHandler, UnSpawnDelegate unspawnHandler)
        {
            NetworkScene.RegisterPrefab(prefab, spawnHandler, unspawnHandler);
        }

        public static void RegisterSpawnHandler(NetworkHash128 assetId, SpawnDelegate spawnHandler, UnSpawnDelegate unspawnHandler)
        {
            NetworkScene.RegisterSpawnHandler(assetId, spawnHandler, unspawnHandler);
        }

        internal static void RegisterSystemHandlers(NetworkClient client, bool localClient)
        {
            if (localClient)
            {
                client.RegisterHandlerSafe(1, new NetworkMessageDelegate(ClientScene.OnLocalClientObjectDestroy));
                client.RegisterHandlerSafe(13, new NetworkMessageDelegate(ClientScene.OnLocalClientObjectHide));
                client.RegisterHandlerSafe(3, new NetworkMessageDelegate(ClientScene.OnLocalClientObjectSpawn));
                client.RegisterHandlerSafe(10, new NetworkMessageDelegate(ClientScene.OnLocalClientObjectSpawnScene));
                client.RegisterHandlerSafe(15, new NetworkMessageDelegate(ClientScene.OnClientAuthority));
            }
            else
            {
                client.RegisterHandlerSafe(3, new NetworkMessageDelegate(ClientScene.OnObjectSpawn));
                client.RegisterHandlerSafe(10, new NetworkMessageDelegate(ClientScene.OnObjectSpawnScene));
                client.RegisterHandlerSafe(12, new NetworkMessageDelegate(ClientScene.OnObjectSpawnFinished));
                client.RegisterHandlerSafe(1, new NetworkMessageDelegate(ClientScene.OnObjectDestroy));
                client.RegisterHandlerSafe(13, new NetworkMessageDelegate(ClientScene.OnObjectDestroy));
                client.RegisterHandlerSafe(8, new NetworkMessageDelegate(ClientScene.OnUpdateVarsMessage));
                client.RegisterHandlerSafe(4, new NetworkMessageDelegate(ClientScene.OnOwnerMessage));
                client.RegisterHandlerSafe(9, new NetworkMessageDelegate(ClientScene.OnSyncListMessage));
                client.RegisterHandlerSafe(40, new NetworkMessageDelegate(NetworkAnimator.OnAnimationClientMessage));
                client.RegisterHandlerSafe(0x29, new NetworkMessageDelegate(NetworkAnimator.OnAnimationParametersClientMessage));
                client.RegisterHandlerSafe(15, new NetworkMessageDelegate(ClientScene.OnClientAuthority));
            }
            client.RegisterHandlerSafe(2, new NetworkMessageDelegate(ClientScene.OnRPCMessage));
            client.RegisterHandlerSafe(7, new NetworkMessageDelegate(ClientScene.OnSyncEventMessage));
            client.RegisterHandlerSafe(0x2a, new NetworkMessageDelegate(NetworkAnimator.OnAnimationTriggerClientMessage));
        }

        public static bool RemovePlayer(short playerControllerId)
        {
            PlayerController controller;
            if (LogFilter.logDebug)
            {
                Debug.Log(string.Concat(new object[] { "ClientScene::RemovePlayer() for ID ", playerControllerId, " called with connection [", s_ReadyConnection, "]" }));
            }
            if (s_ReadyConnection.GetPlayerController(playerControllerId, out controller))
            {
                RemovePlayerMessage msg = new RemovePlayerMessage {
                    playerControllerId = playerControllerId
                };
                s_ReadyConnection.Send(0x26, msg);
                s_ReadyConnection.RemovePlayerController(playerControllerId);
                s_LocalPlayers[playerControllerId] = new PlayerController();
                UnityEngine.Object.Destroy(controller.gameObject);
                return true;
            }
            if (LogFilter.logError)
            {
                Debug.LogError("Failed to find player ID " + playerControllerId);
            }
            return false;
        }

        public static void SetLocalObject(NetworkInstanceId netId, GameObject obj)
        {
            s_NetworkScene.SetLocalObject(netId, obj, s_IsSpawnFinished, false);
        }

        internal static void SetNotReady()
        {
            s_IsReady = false;
        }

        internal static void Shutdown()
        {
            s_NetworkScene.Shutdown();
            s_LocalPlayers = new List<PlayerController>();
            s_PendingOwnerIds = new List<PendingOwner>();
            s_SpawnableObjects = null;
            s_ReadyConnection = null;
            s_IsReady = false;
            s_IsSpawnFinished = false;
            NetworkTransport.Shutdown();
            NetworkTransport.Init();
        }

        internal static NetworkIdentity SpawnSceneObject(NetworkSceneId sceneId)
        {
            if (s_SpawnableObjects.ContainsKey(sceneId))
            {
                NetworkIdentity identity = s_SpawnableObjects[sceneId];
                s_SpawnableObjects.Remove(sceneId);
                return identity;
            }
            return null;
        }

        public static void UnregisterPrefab(GameObject prefab)
        {
            NetworkScene.UnregisterPrefab(prefab);
        }

        public static void UnregisterSpawnHandler(NetworkHash128 assetId)
        {
            NetworkScene.UnregisterSpawnHandler(assetId);
        }

        public static List<PlayerController> localPlayers
        {
            get
            {
                return s_LocalPlayers;
            }
        }

        public static Dictionary<NetworkInstanceId, NetworkIdentity> objects
        {
            get
            {
                return s_NetworkScene.localObjects;
            }
        }

        public static Dictionary<NetworkHash128, GameObject> prefabs
        {
            get
            {
                return NetworkScene.guidToPrefab;
            }
        }

        public static bool ready
        {
            get
            {
                return s_IsReady;
            }
        }

        public static NetworkConnection readyConnection
        {
            get
            {
                return s_ReadyConnection;
            }
        }

        public static Dictionary<NetworkSceneId, NetworkIdentity> spawnableObjects
        {
            get
            {
                return s_SpawnableObjects;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PendingOwner
        {
            public NetworkInstanceId netId;
            public short playerControllerId;
        }
    }
}

