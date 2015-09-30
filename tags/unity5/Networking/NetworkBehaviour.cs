namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    [AddComponentMenu(""), RequireComponent(typeof(NetworkIdentity))]
    public class NetworkBehaviour : MonoBehaviour
    {
        private const float k_DefaultSendInterval = 0.1f;
        private float m_LastSendTime;
        private NetworkIdentity m_MyView;
        private uint m_SyncVarDirtyBits;
        private bool m_SyncVarGuard;
        private static Dictionary<int, Invoker> s_CmdHandlerDelegates = new Dictionary<int, Invoker>();

        public void ClearAllDirtyBits()
        {
            this.m_LastSendTime = Time.time;
            this.m_SyncVarDirtyBits = 0;
        }

        internal bool ContainsCommandDelegate(int cmdHash)
        {
            return s_CmdHandlerDelegates.ContainsKey(cmdHash);
        }

        internal static void DumpInvokers()
        {
            Debug.Log("DumpInvokers size:" + s_CmdHandlerDelegates.Count);
            foreach (KeyValuePair<int, Invoker> pair in s_CmdHandlerDelegates)
            {
                Debug.Log(string.Concat(new object[] { "  Invoker:", pair.Value.invokeClass, ":", pair.Value.invokeFunction.Method.Name, " ", pair.Value.invokeType, " ", pair.Key }));
            }
        }

        internal static string GetCmdHashCmdName(int cmdHash)
        {
            return GetCmdHashPrefixName(cmdHash, "InvokeCmd");
        }

        internal static string GetCmdHashEventName(int cmdHash)
        {
            return GetCmdHashPrefixName(cmdHash, "InvokeSyncEvent");
        }

        internal static string GetCmdHashHandlerName(int cmdHash)
        {
            if (!s_CmdHandlerDelegates.ContainsKey(cmdHash))
            {
                return cmdHash.ToString();
            }
            Invoker invoker = s_CmdHandlerDelegates[cmdHash];
            return (invoker.invokeType + ":" + invoker.invokeFunction.Method.Name);
        }

        internal static string GetCmdHashListName(int cmdHash)
        {
            return GetCmdHashPrefixName(cmdHash, "InvokeSyncList");
        }

        private static string GetCmdHashPrefixName(int cmdHash, string prefix)
        {
            if (!s_CmdHandlerDelegates.ContainsKey(cmdHash))
            {
                return cmdHash.ToString();
            }
            Invoker invoker = s_CmdHandlerDelegates[cmdHash];
            string name = invoker.invokeFunction.Method.Name;
            if (name.IndexOf(prefix) > -1)
            {
                name = name.Substring(prefix.Length);
            }
            return name;
        }

        internal static string GetCmdHashRpcName(int cmdHash)
        {
            return GetCmdHashPrefixName(cmdHash, "InvokeRpc");
        }

        internal int GetDirtyChannel()
        {
            if (((Time.time - this.m_LastSendTime) > this.GetNetworkSendInterval()) && (this.m_SyncVarDirtyBits != 0))
            {
                return this.GetNetworkChannel();
            }
            return -1;
        }

        internal static string GetInvoker(int cmdHash)
        {
            if (!s_CmdHandlerDelegates.ContainsKey(cmdHash))
            {
                return null;
            }
            Invoker invoker = s_CmdHandlerDelegates[cmdHash];
            return invoker.DebugString();
        }

        public virtual int GetNetworkChannel()
        {
            return 0;
        }

        public virtual float GetNetworkSendInterval()
        {
            return 0.1f;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual bool InvokeCommand(int cmdHash, NetworkReader reader)
        {
            return this.InvokeCommandDelegate(cmdHash, reader);
        }

        internal bool InvokeCommandDelegate(int cmdHash, NetworkReader reader)
        {
            if (!s_CmdHandlerDelegates.ContainsKey(cmdHash))
            {
                return false;
            }
            Invoker invoker = s_CmdHandlerDelegates[cmdHash];
            if (invoker.invokeType != UNetInvokeType.Command)
            {
                return false;
            }
            if ((base.GetType() != invoker.invokeClass) && !base.GetType().IsSubclassOf(invoker.invokeClass))
            {
                return false;
            }
            invoker.invokeFunction(this, reader);
            return true;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual bool InvokeRPC(int cmdHash, NetworkReader reader)
        {
            return this.InvokeRpcDelegate(cmdHash, reader);
        }

        internal bool InvokeRpcDelegate(int cmdHash, NetworkReader reader)
        {
            if (!s_CmdHandlerDelegates.ContainsKey(cmdHash))
            {
                return false;
            }
            Invoker invoker = s_CmdHandlerDelegates[cmdHash];
            if (invoker.invokeType != UNetInvokeType.ClientRpc)
            {
                return false;
            }
            if ((base.GetType() != invoker.invokeClass) && !base.GetType().IsSubclassOf(invoker.invokeClass))
            {
                return false;
            }
            invoker.invokeFunction(this, reader);
            return true;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual bool InvokeSyncEvent(int cmdHash, NetworkReader reader)
        {
            return this.InvokeSyncEventDelegate(cmdHash, reader);
        }

        internal bool InvokeSyncEventDelegate(int cmdHash, NetworkReader reader)
        {
            if (!s_CmdHandlerDelegates.ContainsKey(cmdHash))
            {
                return false;
            }
            Invoker invoker = s_CmdHandlerDelegates[cmdHash];
            if (invoker.invokeType != UNetInvokeType.SyncEvent)
            {
                return false;
            }
            invoker.invokeFunction(this, reader);
            return true;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual bool InvokeSyncList(int cmdHash, NetworkReader reader)
        {
            return this.InvokeSyncListDelegate(cmdHash, reader);
        }

        internal bool InvokeSyncListDelegate(int cmdHash, NetworkReader reader)
        {
            if (!s_CmdHandlerDelegates.ContainsKey(cmdHash))
            {
                return false;
            }
            Invoker invoker = s_CmdHandlerDelegates[cmdHash];
            if (invoker.invokeType != UNetInvokeType.SyncList)
            {
                return false;
            }
            if (base.GetType() != invoker.invokeClass)
            {
                return false;
            }
            invoker.invokeFunction(this, reader);
            return true;
        }

        public virtual bool OnCheckObserver(NetworkConnection conn)
        {
            return true;
        }

        public virtual void OnDeserialize(NetworkReader reader, bool initialState)
        {
            if (!initialState)
            {
                reader.ReadPackedUInt32();
            }
        }

        public virtual void OnNetworkDestroy()
        {
        }

        public virtual bool OnRebuildObservers(HashSet<NetworkConnection> observers, bool initialize)
        {
            return false;
        }

        public virtual bool OnSerialize(NetworkWriter writer, bool initialState)
        {
            if (!initialState)
            {
                writer.WritePackedUInt32(0);
            }
            return false;
        }

        public virtual void OnSetLocalVisibility(bool vis)
        {
        }

        public virtual void OnStartAuthority()
        {
        }

        public virtual void OnStartClient()
        {
        }

        public virtual void OnStartLocalPlayer()
        {
        }

        public virtual void OnStartServer()
        {
        }

        public virtual void OnStopAuthority()
        {
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void PreStartClient()
        {
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected static void RegisterCommandDelegate(System.Type invokeClass, int cmdHash, CmdDelegate func)
        {
            if (!s_CmdHandlerDelegates.ContainsKey(cmdHash))
            {
                Invoker invoker = new Invoker {
                    invokeType = UNetInvokeType.Command,
                    invokeClass = invokeClass,
                    invokeFunction = func
                };
                s_CmdHandlerDelegates[cmdHash] = invoker;
                if (LogFilter.logDev)
                {
                    Debug.Log(string.Concat(new object[] { "RegisterCommandDelegate hash:", cmdHash, " ", func.Method.Name }));
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected static void RegisterEventDelegate(System.Type invokeClass, int cmdHash, CmdDelegate func)
        {
            if (!s_CmdHandlerDelegates.ContainsKey(cmdHash))
            {
                Invoker invoker = new Invoker {
                    invokeType = UNetInvokeType.SyncEvent,
                    invokeClass = invokeClass,
                    invokeFunction = func
                };
                s_CmdHandlerDelegates[cmdHash] = invoker;
                if (LogFilter.logDev)
                {
                    Debug.Log(string.Concat(new object[] { "RegisterEventDelegate hash:", cmdHash, " ", func.Method.Name }));
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected static void RegisterRpcDelegate(System.Type invokeClass, int cmdHash, CmdDelegate func)
        {
            if (!s_CmdHandlerDelegates.ContainsKey(cmdHash))
            {
                Invoker invoker = new Invoker {
                    invokeType = UNetInvokeType.ClientRpc,
                    invokeClass = invokeClass,
                    invokeFunction = func
                };
                s_CmdHandlerDelegates[cmdHash] = invoker;
                if (LogFilter.logDev)
                {
                    Debug.Log(string.Concat(new object[] { "RegisterRpcDelegate hash:", cmdHash, " ", func.Method.Name }));
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected static void RegisterSyncListDelegate(System.Type invokeClass, int cmdHash, CmdDelegate func)
        {
            if (!s_CmdHandlerDelegates.ContainsKey(cmdHash))
            {
                Invoker invoker = new Invoker {
                    invokeType = UNetInvokeType.SyncList,
                    invokeClass = invokeClass,
                    invokeFunction = func
                };
                s_CmdHandlerDelegates[cmdHash] = invoker;
                if (LogFilter.logDev)
                {
                    Debug.Log(string.Concat(new object[] { "RegisterSyncListDelegate hash:", cmdHash, " ", func.Method.Name }));
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void SendCommandInternal(NetworkWriter writer, int channelId, string cmdName)
        {
            if (!this.isLocalPlayer && !this.hasAuthority)
            {
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning("Trying to send command for object without authority.");
                }
            }
            else if (ClientScene.readyConnection == null)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("Send command attempted with no client running [client=" + this.connectionToServer + "].");
                }
            }
            else
            {
                writer.FinishMessage();
                ClientScene.readyConnection.SendWriter(writer, channelId);
                NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Outgoing, 5, cmdName, 1);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void SendEventInternal(NetworkWriter writer, int channelId, string eventName)
        {
            if (!NetworkServer.active)
            {
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning("SendEvent no server?");
                }
            }
            else
            {
                writer.FinishMessage();
                NetworkServer.SendWriterToReady(base.gameObject, writer, channelId);
                NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Outgoing, 7, eventName, 1);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void SendRPCInternal(NetworkWriter writer, int channelId, string rpcName)
        {
            if (!this.isServer)
            {
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning("ClientRpc call on un-spawned object");
                }
            }
            else
            {
                writer.FinishMessage();
                NetworkServer.SendWriterToReady(base.gameObject, writer, channelId);
                NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Outgoing, 2, rpcName, 1);
            }
        }

        public void SetDirtyBit(uint dirtyBit)
        {
            this.m_SyncVarDirtyBits |= dirtyBit;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void SetSyncVar<T>(T value, ref T fieldValue, uint dirtyBit)
        {
            if (!value.Equals((T) fieldValue))
            {
                if (LogFilter.logDev)
                {
                    Debug.Log(string.Concat(new object[] { "SetSyncVar ", base.GetType().Name, " bit [", dirtyBit, "] ", (T) fieldValue, "->", value }));
                }
                this.SetDirtyBit(dirtyBit);
                fieldValue = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void SetSyncVarGameObject(GameObject newGameObject, ref GameObject gameObjectField, uint dirtyBit, ref NetworkInstanceId netIdField)
        {
            if (!this.m_SyncVarGuard)
            {
                NetworkInstanceId netId = new NetworkInstanceId();
                if (newGameObject != null)
                {
                    NetworkIdentity component = newGameObject.GetComponent<NetworkIdentity>();
                    if (component != null)
                    {
                        netId = component.netId;
                        if (netId.IsEmpty() && LogFilter.logWarn)
                        {
                            Debug.LogWarning("SetSyncVarGameObject GameObject " + newGameObject + " has a zero netId. Maybe it is not spawned yet?");
                        }
                    }
                }
                NetworkInstanceId id2 = new NetworkInstanceId();
                if (gameObjectField != null)
                {
                    id2 = gameObjectField.GetComponent<NetworkIdentity>().netId;
                }
                if (netId != id2)
                {
                    if (LogFilter.logDev)
                    {
                        Debug.Log(string.Concat(new object[] { "SetSyncVar GameObject ", base.GetType().Name, " bit [", dirtyBit, "] netfieldId:", id2, "->", netId }));
                    }
                    this.SetDirtyBit(dirtyBit);
                    gameObjectField = newGameObject;
                    netIdField = netId;
                }
            }
        }

        public NetworkConnection connectionToClient
        {
            get
            {
                return this.myView.connectionToClient;
            }
        }

        public NetworkConnection connectionToServer
        {
            get
            {
                return this.myView.connectionToServer;
            }
        }

        public bool hasAuthority
        {
            get
            {
                return this.myView.hasAuthority;
            }
        }

        public bool isClient
        {
            get
            {
                return this.myView.isClient;
            }
        }

        public bool isLocalPlayer
        {
            get
            {
                return this.myView.isLocalPlayer;
            }
        }

        public bool isServer
        {
            get
            {
                return this.myView.isServer;
            }
        }

        public bool localPlayerAuthority
        {
            get
            {
                return this.myView.localPlayerAuthority;
            }
        }

        private NetworkIdentity myView
        {
            get
            {
                if (this.m_MyView == null)
                {
                    this.m_MyView = base.GetComponent<NetworkIdentity>();
                    if ((this.m_MyView == null) && LogFilter.logError)
                    {
                        Debug.LogError("There is no NetworkIdentity on this object. Please add one.");
                    }
                }
                return this.m_MyView;
            }
        }

        public NetworkInstanceId netId
        {
            get
            {
                return this.myView.netId;
            }
        }

        public short playerControllerId
        {
            get
            {
                return this.myView.playerControllerId;
            }
        }

        protected uint syncVarDirtyBits
        {
            get
            {
                return this.m_SyncVarDirtyBits;
            }
        }

        protected bool syncVarHookGuard
        {
            get
            {
                return this.m_SyncVarGuard;
            }
            set
            {
                this.m_SyncVarGuard = value;
            }
        }

        protected delegate void CmdDelegate(NetworkBehaviour obj, NetworkReader reader);

        protected delegate void EventDelegate(List<Delegate> targets, NetworkReader reader);

        protected class Invoker
        {
            public System.Type invokeClass;
            public NetworkBehaviour.CmdDelegate invokeFunction;
            public NetworkBehaviour.UNetInvokeType invokeType;

            public string DebugString()
            {
                object[] objArray1 = new object[] { this.invokeType, ":", this.invokeClass, ":", this.invokeFunction.Method.Name };
                return string.Concat(objArray1);
            }
        }

        protected enum UNetInvokeType
        {
            Command,
            ClientRpc,
            SyncEvent,
            SyncList
        }
    }
}

