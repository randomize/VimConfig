namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal sealed class LocalClient : NetworkClient
    {
        private const int k_InitialFreeMessagePoolSize = 0x40;
        private bool m_Connected;
        private Stack<InternalMsg> m_FreeMessages;
        private List<InternalMsg> m_InternalMsgs = new List<InternalMsg>();
        private List<InternalMsg> m_InternalMsgs2 = new List<InternalMsg>();
        private NetworkServer m_LocalServer;
        private NetworkMessage s_InternalMessage = new NetworkMessage();

        internal void AddLocalPlayer(PlayerController localPlayer)
        {
            if (LogFilter.logDev)
            {
                Debug.Log(string.Concat(new object[] { "Local client AddLocalPlayer ", localPlayer.gameObject.name, " conn=", base.m_Connection.connectionId }));
            }
            base.m_Connection.isReady = true;
            base.m_Connection.SetPlayerController(localPlayer);
            NetworkIdentity unetView = localPlayer.unetView;
            if (unetView != null)
            {
                ClientScene.SetLocalObject(unetView.netId, localPlayer.gameObject);
                unetView.SetConnectionToServer(base.m_Connection);
            }
            ClientScene.InternalAddPlayer(unetView, localPlayer.playerControllerId);
        }

        public override void Disconnect()
        {
            ClientScene.HandleClientDisconnect(base.m_Connection);
            if (this.m_Connected)
            {
                this.PostInternalMessage(0x21);
                this.m_Connected = false;
            }
            base.m_AsyncConnect = NetworkClient.ConnectState.Disconnected;
        }

        internal void InternalConnectLocalServer()
        {
            if (this.m_FreeMessages == null)
            {
                this.m_FreeMessages = new Stack<InternalMsg>();
                for (int i = 0; i < 0x40; i++)
                {
                    InternalMsg item = new InternalMsg();
                    this.m_FreeMessages.Push(item);
                }
            }
            this.m_LocalServer = NetworkServer.instance;
            base.m_Connection = new ULocalConnectionToServer(this.m_LocalServer);
            base.SetHandlers(base.m_Connection);
            base.m_Connection.connectionId = this.m_LocalServer.AddLocalClient(this);
            base.m_AsyncConnect = NetworkClient.ConnectState.Connected;
            NetworkClient.SetActive(true);
            base.RegisterSystemHandlers(true);
            this.PostInternalMessage(0x20);
            this.m_Connected = true;
        }

        internal void InvokeBytesOnClient(byte[] buffer, int channelId)
        {
            this.PostInternalMessage(buffer, channelId);
        }

        internal void InvokeHandlerOnClient(short msgType, MessageBase msg, int channelId)
        {
            NetworkWriter writer = new NetworkWriter();
            writer.StartMessage(msgType);
            msg.Serialize(writer);
            writer.FinishMessage();
            this.InvokeBytesOnClient(writer.AsArray(), channelId);
        }

        private void PostInternalMessage(short msgType)
        {
            NetworkWriter writer = new NetworkWriter();
            writer.StartMessage(msgType);
            writer.FinishMessage();
            this.PostInternalMessage(writer.AsArray(), 0);
        }

        private void PostInternalMessage(byte[] buffer, int channelId)
        {
            InternalMsg msg;
            if (this.m_FreeMessages.Count == 0)
            {
                msg = new InternalMsg();
            }
            else
            {
                msg = this.m_FreeMessages.Pop();
            }
            msg.buffer = buffer;
            msg.channelId = channelId;
            this.m_InternalMsgs.Add(msg);
        }

        private void ProcessInternalMessages()
        {
            if (this.m_InternalMsgs.Count != 0)
            {
                List<InternalMsg> internalMsgs = this.m_InternalMsgs;
                this.m_InternalMsgs = this.m_InternalMsgs2;
                foreach (InternalMsg msg in internalMsgs)
                {
                    if (this.s_InternalMessage.reader == null)
                    {
                        this.s_InternalMessage.reader = new NetworkReader(msg.buffer);
                    }
                    else
                    {
                        this.s_InternalMessage.reader.Replace(msg.buffer);
                    }
                    this.s_InternalMessage.reader.ReadInt16();
                    this.s_InternalMessage.channelId = msg.channelId;
                    this.s_InternalMessage.conn = base.connection;
                    this.s_InternalMessage.msgType = this.s_InternalMessage.reader.ReadInt16();
                    base.m_Connection.InvokeHandler(this.s_InternalMessage);
                    this.m_FreeMessages.Push(msg);
                    base.connection.lastMessageTime = Time.time;
                }
                this.m_InternalMsgs = internalMsgs;
                this.m_InternalMsgs.Clear();
                foreach (InternalMsg msg2 in this.m_InternalMsgs2)
                {
                    this.m_InternalMsgs.Add(msg2);
                }
                this.m_InternalMsgs2.Clear();
            }
        }

        internal override void Update()
        {
            this.ProcessInternalMessages();
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct InternalMsg
        {
            internal byte[] buffer;
            internal int channelId;
        }
    }
}

