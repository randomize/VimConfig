namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal class NetworkScene
    {
        private Dictionary<NetworkInstanceId, NetworkIdentity> m_LocalObjects = new Dictionary<NetworkInstanceId, NetworkIdentity>();
        private static Dictionary<NetworkHash128, GameObject> s_GuidToPrefab = new Dictionary<NetworkHash128, GameObject>();
        private static Dictionary<NetworkHash128, SpawnDelegate> s_SpawnHandlers = new Dictionary<NetworkHash128, SpawnDelegate>();
        private static Dictionary<NetworkHash128, UnSpawnDelegate> s_UnspawnHandlers = new Dictionary<NetworkHash128, UnSpawnDelegate>();

        internal void ClearLocalObjects()
        {
            this.m_LocalObjects.Clear();
        }

        internal static void ClearSpawners()
        {
            s_GuidToPrefab.Clear();
            s_SpawnHandlers.Clear();
            s_UnspawnHandlers.Clear();
        }

        internal void DestroyAllClientObjects()
        {
            foreach (NetworkInstanceId id in this.m_LocalObjects.Keys)
            {
                NetworkIdentity identity = this.m_LocalObjects[id];
                if ((identity != null) && (identity.gameObject != null))
                {
                    if (identity.sceneId.IsEmpty())
                    {
                        UnityEngine.Object.Destroy(identity.gameObject);
                    }
                    else
                    {
                        identity.gameObject.SetActive(false);
                    }
                }
            }
            this.ClearLocalObjects();
        }

        internal void DumpAllClientObjects()
        {
            foreach (NetworkInstanceId id in this.m_LocalObjects.Keys)
            {
                NetworkIdentity identity = this.m_LocalObjects[id];
                if (identity != null)
                {
                    Debug.Log(string.Concat(new object[] { "ID:", id, " OBJ:", identity.gameObject, " AS:", identity.assetId }));
                }
                else
                {
                    Debug.Log("ID:" + id + " OBJ: null");
                }
            }
        }

        internal GameObject FindLocalObject(NetworkInstanceId netId)
        {
            if (this.m_LocalObjects.ContainsKey(netId))
            {
                NetworkIdentity identity = this.m_LocalObjects[netId];
                if (identity != null)
                {
                    return identity.gameObject;
                }
            }
            return null;
        }

        internal bool GetNetworkIdentity(NetworkInstanceId netId, out NetworkIdentity uv)
        {
            if (this.m_LocalObjects.ContainsKey(netId) && (this.m_LocalObjects[netId] != null))
            {
                uv = this.m_LocalObjects[netId];
                return true;
            }
            uv = null;
            return false;
        }

        internal static bool GetPrefab(NetworkHash128 assetId, out GameObject prefab)
        {
            if (!assetId.IsValid())
            {
                prefab = null;
                return false;
            }
            if (s_GuidToPrefab.ContainsKey(assetId) && (s_GuidToPrefab[assetId] != null))
            {
                prefab = s_GuidToPrefab[assetId];
                return true;
            }
            prefab = null;
            return false;
        }

        internal static bool GetSpawnHandler(NetworkHash128 assetId, out SpawnDelegate handler)
        {
            if (s_SpawnHandlers.ContainsKey(assetId))
            {
                handler = s_SpawnHandlers[assetId];
                return true;
            }
            handler = null;
            return false;
        }

        internal static bool InvokeUnSpawnHandler(NetworkHash128 assetId, GameObject obj)
        {
            if (s_UnspawnHandlers.ContainsKey(assetId) && (s_UnspawnHandlers[assetId] != null))
            {
                UnSpawnDelegate delegate2 = s_UnspawnHandlers[assetId];
                delegate2(obj);
                return true;
            }
            return false;
        }

        internal static void RegisterPrefab(GameObject prefab)
        {
            NetworkIdentity component = prefab.GetComponent<NetworkIdentity>();
            if (component != null)
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log(string.Concat(new object[] { "Registering prefab '", prefab.name, "' as asset:", component.assetId }));
                }
                s_GuidToPrefab[component.assetId] = prefab;
            }
            else if (LogFilter.logError)
            {
                Debug.LogError("Could not register '" + prefab.name + "' since it contains no NetworkIdentity component");
            }
        }

        internal static void RegisterPrefab(GameObject prefab, SpawnDelegate spawnHandler, UnSpawnDelegate unspawnHandler)
        {
            NetworkIdentity component = prefab.GetComponent<NetworkIdentity>();
            if (component == null)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("Could not register '" + prefab.name + "' since it contains no NetworkIdentity component");
                }
            }
            else if ((spawnHandler == null) || (unspawnHandler == null))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("RegisterPrefab custom spawn function null for " + component.assetId);
                }
            }
            else if (!component.assetId.IsValid())
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("RegisterPrefab game object " + prefab.name + " has no prefab. Use RegisterSpawnHandler() instead?");
                }
            }
            else
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log(string.Concat(new object[] { "Registering custom prefab '", prefab.name, "' as asset:", component.assetId, " ", spawnHandler.Method.Name, "/", unspawnHandler.Method.Name }));
                }
                s_SpawnHandlers[component.assetId] = spawnHandler;
                s_UnspawnHandlers[component.assetId] = unspawnHandler;
            }
        }

        internal static void RegisterSpawnHandler(NetworkHash128 assetId, SpawnDelegate spawnHandler, UnSpawnDelegate unspawnHandler)
        {
            if ((spawnHandler == null) || (unspawnHandler == null))
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("RegisterSpawnHandler custom spawn function null for " + assetId);
                }
            }
            else
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log(string.Concat(new object[] { "RegisterSpawnHandler asset '", assetId, "' ", spawnHandler.Method.Name, "/", unspawnHandler.Method.Name }));
                }
                s_SpawnHandlers[assetId] = spawnHandler;
                s_UnspawnHandlers[assetId] = unspawnHandler;
            }
        }

        internal bool RemoveLocalObject(NetworkInstanceId netId)
        {
            return this.m_LocalObjects.Remove(netId);
        }

        internal bool RemoveLocalObjectAndDestroy(NetworkInstanceId netId)
        {
            if (this.m_LocalObjects.ContainsKey(netId))
            {
                NetworkIdentity identity = this.m_LocalObjects[netId];
                UnityEngine.Object.Destroy(identity.gameObject);
                return this.m_LocalObjects.Remove(netId);
            }
            return false;
        }

        internal void SetLocalObject(NetworkInstanceId netId, GameObject obj, bool isClient, bool isServer)
        {
            if (LogFilter.logDev)
            {
                Debug.Log(string.Concat(new object[] { "SetLocalObject ", netId, " ", obj }));
            }
            if (obj == null)
            {
                this.m_LocalObjects[netId] = null;
            }
            else
            {
                NetworkIdentity component = null;
                if (this.m_LocalObjects.ContainsKey(netId))
                {
                    component = this.m_LocalObjects[netId];
                }
                if (component == null)
                {
                    component = obj.GetComponent<NetworkIdentity>();
                    this.m_LocalObjects[netId] = component;
                }
                component.UpdateClientServer(isClient, isServer);
            }
        }

        internal void Shutdown()
        {
            this.ClearLocalObjects();
            ClearSpawners();
        }

        internal static void UnregisterPrefab(GameObject prefab)
        {
            NetworkIdentity component = prefab.GetComponent<NetworkIdentity>();
            if (component == null)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("Could not unregister '" + prefab.name + "' since it contains no NetworkIdentity component");
                }
            }
            else
            {
                s_SpawnHandlers.Remove(component.assetId);
                s_UnspawnHandlers.Remove(component.assetId);
            }
        }

        public static void UnregisterSpawnHandler(NetworkHash128 assetId)
        {
            s_SpawnHandlers.Remove(assetId);
            s_UnspawnHandlers.Remove(assetId);
        }

        internal static Dictionary<NetworkHash128, GameObject> guidToPrefab
        {
            get
            {
                return s_GuidToPrefab;
            }
        }

        internal Dictionary<NetworkInstanceId, NetworkIdentity> localObjects
        {
            get
            {
                return this.m_LocalObjects;
            }
        }

        internal static Dictionary<NetworkHash128, SpawnDelegate> spawnHandlers
        {
            get
            {
                return s_SpawnHandlers;
            }
        }

        internal static Dictionary<NetworkHash128, UnSpawnDelegate> unspawnHandlers
        {
            get
            {
                return s_UnspawnHandlers;
            }
        }
    }
}

