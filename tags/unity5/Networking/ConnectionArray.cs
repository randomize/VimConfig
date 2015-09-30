namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal class ConnectionArray
    {
        private List<NetworkConnection> m_Connections = new List<NetworkConnection>();
        private List<NetworkConnection> m_LocalConnections = new List<NetworkConnection>();

        public int Add(int connId, NetworkConnection conn)
        {
            if (connId < 0)
            {
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning("ConnectionArray Add bad id " + connId);
                }
                return -1;
            }
            if ((connId < this.m_Connections.Count) && (this.m_Connections[connId] != null))
            {
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning("ConnectionArray Add dupe at " + connId);
                }
                return -1;
            }
            while (connId > (this.m_Connections.Count - 1))
            {
                this.m_Connections.Add(null);
            }
            this.m_Connections[connId] = conn;
            return connId;
        }

        public int AddLocal(NetworkConnection conn)
        {
            this.m_LocalConnections.Add(conn);
            int num = -this.m_LocalConnections.Count;
            conn.connectionId = num;
            return num;
        }

        public bool ContainsPlayer(GameObject player, out NetworkConnection conn)
        {
            conn = null;
            if (player != null)
            {
                for (int i = this.LocalIndex; i < this.m_Connections.Count; i++)
                {
                    conn = this.Get(i);
                    if (conn != null)
                    {
                        for (int j = 0; j < conn.playerControllers.Count; j++)
                        {
                            if (conn.playerControllers[j].IsValid && (conn.playerControllers[j].gameObject == player))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public NetworkConnection Get(int connId)
        {
            if (connId < 0)
            {
                return this.m_LocalConnections[Mathf.Abs(connId) - 1];
            }
            if ((connId >= 0) && (connId <= this.m_Connections.Count))
            {
                return this.m_Connections[connId];
            }
            if (LogFilter.logWarn)
            {
                Debug.LogWarning("ConnectionArray Get invalid index " + connId);
            }
            return null;
        }

        public NetworkConnection GetUnsafe(int connId)
        {
            if ((connId >= 0) && (connId <= this.m_Connections.Count))
            {
                return this.m_Connections[connId];
            }
            return null;
        }

        public void Remove(int connId)
        {
            if (connId < 0)
            {
                this.m_LocalConnections[Mathf.Abs(connId) - 1] = null;
            }
            else if ((connId < 0) || (connId > this.m_Connections.Count))
            {
                if (LogFilter.logWarn)
                {
                    Debug.LogWarning("ConnectionArray Remove invalid index " + connId);
                }
            }
            else
            {
                this.m_Connections[connId] = null;
            }
        }

        internal List<NetworkConnection> connections
        {
            get
            {
                return this.m_Connections;
            }
        }

        public int Count
        {
            get
            {
                return this.m_Connections.Count;
            }
        }

        internal List<NetworkConnection> localConnections
        {
            get
            {
                return this.m_LocalConnections;
            }
        }

        public int LocalIndex
        {
            get
            {
                return -this.m_LocalConnections.Count;
            }
        }
    }
}

