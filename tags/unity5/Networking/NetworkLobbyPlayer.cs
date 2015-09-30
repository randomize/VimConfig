namespace UnityEngine.Networking
{
    using System;
    using UnityEngine;
    using UnityEngine.Networking.NetworkSystem;

    [AddComponentMenu("Network/NetworkLobbyPlayer"), DisallowMultipleComponent]
    public class NetworkLobbyPlayer : NetworkBehaviour
    {
        private bool m_ReadyToBegin;
        private byte m_Slot;
        [SerializeField]
        public bool ShowLobbyGUI = true;

        public virtual void OnClientEnterLobby()
        {
        }

        public virtual void OnClientExitLobby()
        {
        }

        public virtual void OnClientReady(bool readyState)
        {
        }

        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            if (reader.ReadPackedUInt32() != 0)
            {
                this.m_Slot = reader.ReadByte();
                this.m_ReadyToBegin = reader.ReadBoolean();
            }
        }

        private void OnGUI()
        {
            if (this.ShowLobbyGUI)
            {
                NetworkLobbyManager singleton = NetworkManager.singleton as NetworkLobbyManager;
                if (singleton != null)
                {
                    if (!singleton.showLobbyGUI)
                    {
                        return;
                    }
                    if (Application.loadedLevelName != singleton.lobbyScene)
                    {
                        return;
                    }
                }
                Rect position = new Rect((float) (100 + (this.m_Slot * 100)), 200f, 90f, 20f);
                if (base.isLocalPlayer)
                {
                    GUI.Label(position, " [ You ]");
                    if (this.m_ReadyToBegin)
                    {
                        position.y += 25f;
                        if (GUI.Button(position, "Ready"))
                        {
                            this.SendNotReadyToBeginMessage();
                        }
                    }
                    else
                    {
                        position.y += 25f;
                        if (GUI.Button(position, "Not Ready"))
                        {
                            this.SendReadyToBeginMessage();
                        }
                        position.y += 25f;
                        if (GUI.Button(position, "Remove"))
                        {
                            ClientScene.RemovePlayer(base.GetComponent<NetworkIdentity>().playerControllerId);
                        }
                    }
                }
                else
                {
                    GUI.Label(position, "Player [" + base.netId + "]");
                    position.y += 25f;
                    GUI.Label(position, "Ready [" + this.m_ReadyToBegin + "]");
                }
            }
        }

        private void OnLevelWasLoaded()
        {
            NetworkLobbyManager singleton = NetworkManager.singleton as NetworkLobbyManager;
            if (((singleton == null) || (Application.loadedLevelName != singleton.lobbyScene)) && base.isLocalPlayer)
            {
                this.SendSceneLoadedMessage();
            }
        }

        public override bool OnSerialize(NetworkWriter writer, bool initialState)
        {
            writer.WritePackedUInt32(1);
            writer.Write(this.m_Slot);
            writer.Write(this.m_ReadyToBegin);
            return true;
        }

        public override void OnStartClient()
        {
            NetworkLobbyManager singleton = NetworkManager.singleton as NetworkLobbyManager;
            if (singleton != null)
            {
                singleton.lobbySlots[this.m_Slot] = this;
                this.m_ReadyToBegin = false;
                this.OnClientEnterLobby();
            }
            else
            {
                Debug.LogError("No Lobby for LobbyPlayer");
            }
        }

        public void RemovePlayer()
        {
            if (base.isLocalPlayer && !this.m_ReadyToBegin)
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log("NetworkLobbyPlayer RemovePlayer");
                }
                ClientScene.RemovePlayer(base.GetComponent<NetworkIdentity>().playerControllerId);
            }
        }

        public void SendNotReadyToBeginMessage()
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkLobbyPlayer SendReadyToBeginMessage");
            }
            NetworkLobbyManager singleton = NetworkManager.singleton as NetworkLobbyManager;
            if (singleton != null)
            {
                LobbyReadyToBeginMessage msg = new LobbyReadyToBeginMessage {
                    slotId = (byte) base.playerControllerId,
                    readyState = false
                };
                singleton.client.Send(0x2b, msg);
            }
        }

        public void SendReadyToBeginMessage()
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkLobbyPlayer SendReadyToBeginMessage");
            }
            NetworkLobbyManager singleton = NetworkManager.singleton as NetworkLobbyManager;
            if (singleton != null)
            {
                LobbyReadyToBeginMessage msg = new LobbyReadyToBeginMessage {
                    slotId = (byte) base.playerControllerId,
                    readyState = true
                };
                singleton.client.Send(0x2b, msg);
            }
        }

        public void SendSceneLoadedMessage()
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("NetworkLobbyPlayer SendSceneLoadedMessage");
            }
            NetworkLobbyManager singleton = NetworkManager.singleton as NetworkLobbyManager;
            if (singleton != null)
            {
                IntegerMessage msg = new IntegerMessage(base.playerControllerId);
                singleton.client.Send(0x2c, msg);
            }
        }

        private void Start()
        {
            UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
        }

        public bool readyToBegin
        {
            get
            {
                return this.m_ReadyToBegin;
            }
            set
            {
                this.m_ReadyToBegin = value;
            }
        }

        public byte slot
        {
            get
            {
                return this.m_Slot;
            }
            set
            {
                this.m_Slot = value;
            }
        }
    }
}

