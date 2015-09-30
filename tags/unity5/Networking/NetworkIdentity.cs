namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Networking.NetworkSystem;

    [AddComponentMenu("Network/NetworkIdentity"), DisallowMultipleComponent, ExecuteInEditMode]
    public sealed class NetworkIdentity : MonoBehaviour
    {
        [SerializeField]
        private NetworkHash128 m_AssetId;
        private NetworkConnection m_ClientAuthorityOwner;
        private NetworkConnection m_ConnectionToClient;
        private NetworkConnection m_ConnectionToServer;
        private bool m_HasAuthority;
        private bool m_IsClient;
        private bool m_IsLocalPlayer;
        private bool m_IsServer;
        [SerializeField]
        private bool m_LocalPlayerAuthority;
        private NetworkInstanceId m_NetId;
        private NetworkBehaviour[] m_NetworkBehaviours;
        private HashSet<int> m_ObserverConnections;
        private List<NetworkConnection> m_Observers;
        private short m_PlayerId = -1;
        [SerializeField]
        private NetworkSceneId m_SceneId;
        [SerializeField]
        private bool m_ServerOnly;
        private static uint s_NextNetworkId = 1;
        private static NetworkWriter s_UpdateWriter = new NetworkWriter();

        internal void AddObserver(NetworkConnection conn)
        {
            if (this.m_Observers != null)
            {
                if (this.m_ObserverConnections.Contains(conn.connectionId))
                {
                    if (LogFilter.logDebug)
                    {
                        Debug.Log(string.Concat(new object[] { "Duplicate observer ", conn.address, " added for ", base.gameObject }));
                    }
                }
                else
                {
                    if (LogFilter.logDev)
                    {
                        Debug.Log(string.Concat(new object[] { "Added observer ", conn.address, " added for ", base.gameObject }));
                    }
                    this.m_Observers.Add(conn);
                    this.m_ObserverConnections.Add(conn.connectionId);
                    conn.AddToVisList(this);
                }
            }
        }

        private void AssignAssetID(GameObject prefab)
        {
            string assetPath = AssetDatabase.GetAssetPath(prefab);
            this.m_AssetId = NetworkHash128.Parse(AssetDatabase.AssetPathToGUID(assetPath));
        }

        public bool AssignClientAuthority(NetworkConnection conn)
        {
            if (!this.isServer)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("AssignClientAuthority can only be call on the server for spawned objects.");
                }
                return false;
            }
            if (!this.localPlayerAuthority)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("AssignClientAuthority can only be used for NetworkIdentity component with LocalPlayerAuthority set.");
                }
                return false;
            }
            if ((this.m_ClientAuthorityOwner != null) && (conn != this.m_ClientAuthorityOwner))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("AssignClientAuthority for " + base.gameObject + " already has an owner. Use RemoveClientAuthority() first.");
                }
                return false;
            }
            if (conn == null)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("AssignClientAuthority for " + base.gameObject + " owner cannot be null. Use RemoveClientAuthority() instead.");
                }
                return false;
            }
            this.m_ClientAuthorityOwner = conn;
            this.m_ClientAuthorityOwner.AddOwnedObject(this);
            ClientAuthorityMessage msg = new ClientAuthorityMessage {
                netId = this.netId,
                authority = true
            };
            conn.Send(15, msg);
            return true;
        }

        private void CacheBehaviours()
        {
            if (this.m_NetworkBehaviours == null)
            {
                this.m_NetworkBehaviours = base.GetComponents<NetworkBehaviour>();
            }
        }

        internal void ClearClientOwner()
        {
            this.m_ClientAuthorityOwner = null;
        }

        internal void ClearObservers()
        {
            if (this.m_Observers != null)
            {
                int count = this.m_Observers.Count;
                for (int i = 0; i < count; i++)
                {
                    this.m_Observers[i].RemoveFromVisList(this, true);
                }
                this.m_Observers.Clear();
                this.m_ObserverConnections.Clear();
            }
        }

        public void ForceSceneId(int newSceneId)
        {
            this.m_SceneId = new NetworkSceneId((uint) newSceneId);
        }

        internal static NetworkInstanceId GetNextNetworkId()
        {
            uint num = s_NextNetworkId;
            s_NextNetworkId++;
            return new NetworkInstanceId(num);
        }

        internal void HandleClientAuthority(bool authority)
        {
            if (!this.localPlayerAuthority)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("HandleClientAuthority " + base.gameObject + " does not have localPlayerAuthority");
                }
            }
            else
            {
                this.m_HasAuthority = authority;
                if (authority)
                {
                    this.OnStartAuthority();
                }
                else
                {
                    this.OnStopAuthority();
                }
            }
        }

        internal void HandleCommand(int cmdHash, NetworkReader reader)
        {
            if (base.gameObject == null)
            {
                string cmdHashHandlerName = NetworkBehaviour.GetCmdHashHandlerName(cmdHash);
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning(string.Concat(new object[] { "Command [", cmdHashHandlerName, "] received for deleted object [netId=", this.netId, "]" }));
                }
            }
            else
            {
                bool flag = false;
                for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
                {
                    NetworkBehaviour behaviour = this.m_NetworkBehaviours[i];
                    if (behaviour.InvokeCommand(cmdHash, reader))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    string str2 = NetworkBehaviour.GetCmdHashHandlerName(cmdHash);
                    if (LogFilter.logError)
                    {
                        Debug.LogError(string.Concat(new object[] { "Found no receiver for incoming command [", str2, "] on ", base.gameObject, ",  the server and client should have the same NetworkBehaviour instances [netId=", this.netId, "]." }));
                    }
                }
                NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Incoming, 5, NetworkBehaviour.GetCmdHashCmdName(cmdHash), 1);
            }
        }

        internal void HandleRPC(int cmdHash, NetworkReader reader)
        {
            if (base.gameObject == null)
            {
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning(string.Concat(new object[] { "ClientRpc [", NetworkBehaviour.GetCmdHashHandlerName(cmdHash), "] received for deleted object ", this.netId }));
                }
            }
            else if (this.m_NetworkBehaviours.Length == 0)
            {
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning("No receiver found for ClientRpc [" + NetworkBehaviour.GetCmdHashHandlerName(cmdHash) + "]. Does the script with the function inherit NetworkBehaviour?");
                }
            }
            else
            {
                for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
                {
                    NetworkBehaviour behaviour = this.m_NetworkBehaviours[i];
                    if (behaviour.InvokeRPC(cmdHash, reader))
                    {
                        NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Incoming, 2, NetworkBehaviour.GetCmdHashRpcName(cmdHash), 1);
                        return;
                    }
                }
                string invoker = NetworkBehaviour.GetInvoker(cmdHash);
                if (invoker == null)
                {
                    invoker = "[unknown:" + cmdHash + "]";
                }
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning(string.Concat(new object[] { "Failed to invoke RPC ", invoker, "(", cmdHash, ") on netID ", this.netId }));
                }
                NetworkBehaviour.DumpInvokers();
            }
        }

        internal void HandleSyncEvent(int cmdHash, NetworkReader reader)
        {
            if (base.gameObject == null)
            {
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning(string.Concat(new object[] { "SyncEvent [", NetworkBehaviour.GetCmdHashHandlerName(cmdHash), "] received for deleted object ", this.netId }));
                }
            }
            else
            {
                for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
                {
                    NetworkBehaviour behaviour = this.m_NetworkBehaviours[i];
                    if (behaviour.InvokeSyncEvent(cmdHash, reader))
                    {
                        NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Incoming, 7, NetworkBehaviour.GetCmdHashEventName(cmdHash), 1);
                        break;
                    }
                }
            }
        }

        internal void HandleSyncList(int cmdHash, NetworkReader reader)
        {
            if (base.gameObject == null)
            {
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning(string.Concat(new object[] { "SyncList [", NetworkBehaviour.GetCmdHashHandlerName(cmdHash), "] received for deleted object ", this.netId }));
                }
            }
            else
            {
                for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
                {
                    NetworkBehaviour behaviour = this.m_NetworkBehaviours[i];
                    if (behaviour.InvokeSyncList(cmdHash, reader))
                    {
                        NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Incoming, 9, NetworkBehaviour.GetCmdHashListName(cmdHash), 1);
                        break;
                    }
                }
            }
        }

        internal bool OnCheckObserver(NetworkConnection conn)
        {
            for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
            {
                NetworkBehaviour behaviour = this.m_NetworkBehaviours[i];
                try
                {
                    if (!behaviour.OnCheckObserver(conn))
                    {
                        return false;
                    }
                }
                catch (Exception exception)
                {
                    Debug.LogError("Exception in OnCheckObserver:" + exception.Message + " " + exception.StackTrace);
                }
            }
            return true;
        }

        private void OnDestroy()
        {
            if (this.m_IsServer)
            {
                NetworkServer.Destroy(base.gameObject);
            }
        }

        internal void OnNetworkDestroy()
        {
            for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
            {
                this.m_NetworkBehaviours[i].OnNetworkDestroy();
            }
            this.m_IsServer = false;
        }

        internal void OnSetLocalVisibility(bool vis)
        {
            for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
            {
                NetworkBehaviour behaviour = this.m_NetworkBehaviours[i];
                try
                {
                    behaviour.OnSetLocalVisibility(vis);
                }
                catch (Exception exception)
                {
                    Debug.LogError("Exception in OnSetLocalVisibility:" + exception.Message + " " + exception.StackTrace);
                }
            }
        }

        internal void OnStartAuthority()
        {
            for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
            {
                NetworkBehaviour behaviour = this.m_NetworkBehaviours[i];
                try
                {
                    behaviour.OnStartAuthority();
                }
                catch (Exception exception)
                {
                    Debug.LogError("Exception in OnStartAuthority:" + exception.Message + " " + exception.StackTrace);
                }
            }
        }

        internal void OnStartClient()
        {
            if (!this.m_IsClient)
            {
                this.m_IsClient = true;
            }
            this.CacheBehaviours();
            if (LogFilter.logDev)
            {
                Debug.Log(string.Concat(new object[] { "OnStartClient ", base.gameObject, " GUID:", this.netId, " localPlayerAuthority:", this.localPlayerAuthority }));
            }
            for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
            {
                NetworkBehaviour behaviour = this.m_NetworkBehaviours[i];
                try
                {
                    behaviour.PreStartClient();
                    behaviour.OnStartClient();
                }
                catch (Exception exception)
                {
                    Debug.LogError("Exception in OnStartClient:" + exception.Message + " " + exception.StackTrace);
                }
            }
        }

        internal void OnStartServer()
        {
            if (!this.m_IsServer)
            {
                this.m_IsServer = true;
                if (this.m_LocalPlayerAuthority)
                {
                    this.m_HasAuthority = false;
                }
                else
                {
                    this.m_HasAuthority = true;
                }
                this.m_Observers = new List<NetworkConnection>();
                this.m_ObserverConnections = new HashSet<int>();
                this.CacheBehaviours();
                if (this.netId.IsEmpty())
                {
                    this.m_NetId = GetNextNetworkId();
                }
                else
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError(string.Concat(new object[] { "Object has non-zero netId ", this.netId, " for ", base.gameObject, " !!1" }));
                    }
                    return;
                }
                if (LogFilter.logDev)
                {
                    Debug.Log(string.Concat(new object[] { "OnStartServer ", base.gameObject, " GUID:", this.netId }));
                }
                NetworkServer.instance.SetLocalObjectOnServer(this.netId, base.gameObject);
                for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
                {
                    NetworkBehaviour behaviour = this.m_NetworkBehaviours[i];
                    try
                    {
                        behaviour.OnStartServer();
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError("Exception in OnStartServer:" + exception.Message + " " + exception.StackTrace);
                    }
                }
                if (NetworkClient.active && NetworkServer.localClientActive)
                {
                    ClientScene.SetLocalObject(this.netId, base.gameObject);
                    this.OnStartClient();
                }
                if (this.hasAuthority)
                {
                    this.OnStartAuthority();
                }
            }
        }

        internal void OnStopAuthority()
        {
            for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
            {
                NetworkBehaviour behaviour = this.m_NetworkBehaviours[i];
                try
                {
                    behaviour.OnStopAuthority();
                }
                catch (Exception exception)
                {
                    Debug.LogError("Exception in OnStopAuthority:" + exception.Message + " " + exception.StackTrace);
                }
            }
        }

        internal void OnUpdateVars(NetworkReader reader, bool initialState)
        {
            if (initialState && (this.m_NetworkBehaviours == null))
            {
                this.m_NetworkBehaviours = base.GetComponents<NetworkBehaviour>();
            }
            for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
            {
                NetworkBehaviour behaviour = this.m_NetworkBehaviours[i];
                uint position = reader.Position;
                behaviour.OnDeserialize(reader, initialState);
                if ((reader.Position - position) > 1)
                {
                    NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Incoming, 8, behaviour.GetType().Name, 1);
                }
            }
        }

        private void OnValidate()
        {
            this.SetupIDs();
        }

        public void RebuildObservers(bool initialize)
        {
            if (this.m_Observers != null)
            {
                bool flag = false;
                bool flag2 = false;
                HashSet<NetworkConnection> observers = new HashSet<NetworkConnection>();
                HashSet<NetworkConnection> set2 = new HashSet<NetworkConnection>(this.m_Observers);
                for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
                {
                    NetworkBehaviour behaviour = this.m_NetworkBehaviours[i];
                    flag2 |= behaviour.OnRebuildObservers(observers, initialize);
                }
                if (!flag2)
                {
                    if (initialize)
                    {
                        foreach (NetworkConnection connection in NetworkServer.connections)
                        {
                            if ((connection != null) && connection.isReady)
                            {
                                this.AddObserver(connection);
                            }
                        }
                        foreach (NetworkConnection connection2 in NetworkServer.localConnections)
                        {
                            if ((connection2 != null) && connection2.isReady)
                            {
                                this.AddObserver(connection2);
                            }
                        }
                    }
                }
                else
                {
                    foreach (NetworkConnection connection3 in observers)
                    {
                        if (initialize || !set2.Contains(connection3))
                        {
                            connection3.AddToVisList(this);
                            if (LogFilter.logDebug)
                            {
                                Debug.Log(string.Concat(new object[] { "New Observer for ", base.gameObject, " ", connection3 }));
                            }
                            flag = true;
                        }
                    }
                    foreach (NetworkConnection connection4 in set2)
                    {
                        if (!observers.Contains(connection4))
                        {
                            connection4.RemoveFromVisList(this, false);
                            if (LogFilter.logDebug)
                            {
                                Debug.Log(string.Concat(new object[] { "Removed Observer for ", base.gameObject, " ", connection4 }));
                            }
                            flag = true;
                        }
                    }
                    if (initialize)
                    {
                        foreach (NetworkConnection connection5 in NetworkServer.localConnections)
                        {
                            if (!observers.Contains(connection5))
                            {
                                this.OnSetLocalVisibility(false);
                            }
                        }
                    }
                    if (flag)
                    {
                        this.m_Observers = new List<NetworkConnection>(observers);
                        this.m_ObserverConnections.Clear();
                        foreach (NetworkConnection connection6 in this.m_Observers)
                        {
                            this.m_ObserverConnections.Add(connection6.connectionId);
                        }
                    }
                }
            }
        }

        public bool RemoveClientAuthority(NetworkConnection conn)
        {
            if (!this.isServer)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("RemoveClientAuthority can only be call on the server for spawned objects.");
                }
                return false;
            }
            if (this.connectionToClient != null)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("RemoveClientAuthority cannot remove authority for a player object");
                }
                return false;
            }
            if (this.m_ClientAuthorityOwner == null)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("RemoveClientAuthority for " + base.gameObject + " has no clientAuthority owner.");
                }
                return false;
            }
            if (this.m_ClientAuthorityOwner != conn)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("RemoveClientAuthority for " + base.gameObject + " has different owner.");
                }
                return false;
            }
            this.m_ClientAuthorityOwner.RemoveOwnedObject(this);
            this.m_ClientAuthorityOwner = null;
            ClientAuthorityMessage msg = new ClientAuthorityMessage {
                netId = this.netId,
                authority = false
            };
            conn.Send(15, msg);
            return true;
        }

        internal void RemoveObserver(NetworkConnection conn)
        {
            if (this.m_Observers != null)
            {
                this.m_Observers.Remove(conn);
                this.m_ObserverConnections.Remove(conn.connectionId);
                conn.RemoveFromVisList(this, false);
            }
        }

        internal void RemoveObserverInternal(NetworkConnection conn)
        {
            if (this.m_Observers != null)
            {
                this.m_Observers.Remove(conn);
                this.m_ObserverConnections.Remove(conn.connectionId);
            }
        }

        internal void SetClientOwner(NetworkConnection conn)
        {
            if ((this.m_ClientAuthorityOwner != null) && LogFilter.logError)
            {
                Debug.LogError("SetClientOwner m_ClientAuthorityOwner already set!");
            }
            this.m_ClientAuthorityOwner = conn;
            this.m_ClientAuthorityOwner.AddOwnedObject(this);
        }

        internal void SetConnectionToClient(NetworkConnection conn, short newPlayerControllerId)
        {
            this.m_PlayerId = newPlayerControllerId;
            this.m_ConnectionToClient = conn;
        }

        internal void SetConnectionToServer(NetworkConnection conn)
        {
            this.m_ConnectionToServer = conn;
        }

        internal void SetDynamicAssetId(NetworkHash128 newAssetId)
        {
            if (!this.m_AssetId.IsValid() || this.m_AssetId.Equals(newAssetId))
            {
                this.m_AssetId = newAssetId;
            }
            else if (LogFilter.logWarn)
            {
                Debug.LogWarning("SetDynamicAssetId object already has an assetId <" + this.m_AssetId + ">");
            }
        }

        internal void SetLocalPlayer(short localPlayerControllerId)
        {
            this.m_IsLocalPlayer = true;
            this.m_PlayerId = localPlayerControllerId;
            if (this.localPlayerAuthority)
            {
                this.m_HasAuthority = true;
            }
            for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
            {
                NetworkBehaviour behaviour = this.m_NetworkBehaviours[i];
                behaviour.OnStartLocalPlayer();
                if (this.localPlayerAuthority)
                {
                    behaviour.OnStartAuthority();
                }
            }
        }

        internal void SetNetworkInstanceId(NetworkInstanceId newNetId)
        {
            this.m_NetId = newNetId;
        }

        internal void SetNoServer()
        {
            this.m_IsServer = false;
            this.SetNetworkInstanceId(NetworkInstanceId.Zero);
        }

        internal void SetNotLocalPlayer()
        {
            this.m_IsLocalPlayer = false;
            this.m_HasAuthority = false;
        }

        private void SetupIDs()
        {
            if (this.ThisIsAPrefab())
            {
                if (LogFilter.logDev)
                {
                    Debug.Log("This is a prefab: " + base.gameObject.name);
                }
                this.AssignAssetID(base.gameObject);
            }
            else
            {
                GameObject obj2;
                if (this.ThisIsASceneObjectWithPrefabParent(out obj2))
                {
                    if (LogFilter.logDev)
                    {
                        Debug.Log("This is a scene object with prefab link: " + base.gameObject.name);
                    }
                    this.AssignAssetID(obj2);
                }
                else
                {
                    if (LogFilter.logDev)
                    {
                        Debug.Log("This is a pure scene object: " + base.gameObject.name);
                    }
                    this.m_AssetId.Reset();
                }
            }
        }

        private bool ThisIsAPrefab()
        {
            return (PrefabUtility.GetPrefabType(base.gameObject) == PrefabType.Prefab);
        }

        private bool ThisIsASceneObjectWithPrefabParent(out GameObject prefab)
        {
            prefab = null;
            if (PrefabUtility.GetPrefabType(base.gameObject) != PrefabType.None)
            {
                prefab = (GameObject) PrefabUtility.GetPrefabParent(base.gameObject);
                if (prefab != null)
                {
                    return true;
                }
                if (LogFilter.logError)
                {
                    Debug.LogError("Failed to find prefab parent for scene object [name:" + base.gameObject.name + "]");
                }
            }
            return false;
        }

        internal void UNetSerializeAllVars(NetworkWriter writer)
        {
            for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
            {
                this.m_NetworkBehaviours[i].OnSerialize(writer, true);
            }
        }

        internal static void UNetStaticUpdate()
        {
            NetworkServer.Update();
            NetworkClient.UpdateClients();
            NetworkManager.UpdateScene();
            NetworkDetailStats.NewProfilerTick(Time.time);
        }

        internal void UNetUpdate()
        {
            uint num = 0;
            for (int i = 0; i < this.m_NetworkBehaviours.Length; i++)
            {
                int dirtyChannel = this.m_NetworkBehaviours[i].GetDirtyChannel();
                if (dirtyChannel != -1)
                {
                    num |= ((uint) 1) << dirtyChannel;
                }
            }
            if (num != 0)
            {
                for (int j = 0; j < NetworkServer.numChannels; j++)
                {
                    if ((num & (((int) 1) << j)) != 0)
                    {
                        s_UpdateWriter.StartMessage(8);
                        s_UpdateWriter.Write(this.netId);
                        bool flag = false;
                        for (int k = 0; k < this.m_NetworkBehaviours.Length; k++)
                        {
                            NetworkBehaviour behaviour2 = this.m_NetworkBehaviours[k];
                            if (behaviour2.GetDirtyChannel() != j)
                            {
                                behaviour2.OnSerialize(s_UpdateWriter, false);
                            }
                            else if (behaviour2.OnSerialize(s_UpdateWriter, false))
                            {
                                behaviour2.ClearAllDirtyBits();
                                NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Outgoing, 8, behaviour2.GetType().Name, 1);
                                flag = true;
                            }
                        }
                        if (flag)
                        {
                            s_UpdateWriter.FinishMessage();
                            NetworkServer.SendWriterToReady(base.gameObject, s_UpdateWriter, j);
                        }
                    }
                }
            }
        }

        internal void UpdateClientServer(bool isClientFlag, bool isServerFlag)
        {
            this.m_IsClient |= isClientFlag;
            this.m_IsServer |= isServerFlag;
        }

        public NetworkHash128 assetId
        {
            get
            {
                if (!this.m_AssetId.IsValid())
                {
                    this.SetupIDs();
                }
                return this.m_AssetId;
            }
        }

        public NetworkConnection clientAuthorityOwner
        {
            get
            {
                return this.m_ClientAuthorityOwner;
            }
        }

        public NetworkConnection connectionToClient
        {
            get
            {
                return this.m_ConnectionToClient;
            }
        }

        public NetworkConnection connectionToServer
        {
            get
            {
                return this.m_ConnectionToServer;
            }
        }

        public bool hasAuthority
        {
            get
            {
                return this.m_HasAuthority;
            }
        }

        public bool isClient
        {
            get
            {
                return this.m_IsClient;
            }
        }

        public bool isLocalPlayer
        {
            get
            {
                return this.m_IsLocalPlayer;
            }
        }

        public bool isServer
        {
            get
            {
                if (!this.m_IsServer)
                {
                    return false;
                }
                return (NetworkServer.active && this.m_IsServer);
            }
        }

        public bool localPlayerAuthority
        {
            get
            {
                return this.m_LocalPlayerAuthority;
            }
            set
            {
                this.m_LocalPlayerAuthority = value;
            }
        }

        public NetworkInstanceId netId
        {
            get
            {
                return this.m_NetId;
            }
        }

        public ReadOnlyCollection<NetworkConnection> observers
        {
            get
            {
                if (this.m_Observers == null)
                {
                    return null;
                }
                return new ReadOnlyCollection<NetworkConnection>(this.m_Observers);
            }
        }

        public short playerControllerId
        {
            get
            {
                return this.m_PlayerId;
            }
        }

        public NetworkSceneId sceneId
        {
            get
            {
                return this.m_SceneId;
            }
        }

        public bool serverOnly
        {
            get
            {
                return this.m_ServerOnly;
            }
            set
            {
                this.m_ServerOnly = value;
            }
        }
    }
}

