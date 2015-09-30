namespace UnityEngine.Networking
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class SyncList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
    {
        private NetworkBehaviour m_Behaviour;
        private SyncListChanged<T> m_Callback;
        private int m_CmdHash;
        private List<T> m_Objects;

        protected SyncList()
        {
            this.m_Objects = new List<T>();
        }

        public void Add(T item)
        {
            this.m_Objects.Add(item);
            this.SendMsg(Operation<T>.OP_ADD, this.m_Objects.Count, item);
        }

        internal void AddInternal(T item)
        {
            this.m_Objects.Add(item);
        }

        public void Clear()
        {
            this.m_Objects.Clear();
            this.SendMsg(Operation<T>.OP_CLEAR, 0);
        }

        public bool Contains(T item)
        {
            return this.m_Objects.Contains(item);
        }

        public void CopyTo(T[] array, int index)
        {
            this.m_Objects.CopyTo(array, index);
        }

        protected abstract T DeserializeItem(NetworkReader reader);
        public void Dirty(int index)
        {
            this.SendMsg(Operation<T>.OP_DIRTY, index, this.m_Objects[index]);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.m_Objects.GetEnumerator();
        }

        public void HandleMsg(NetworkReader reader)
        {
            byte num = reader.ReadByte();
            int index = (int) reader.ReadPackedUInt32();
            T item = this.DeserializeItem(reader);
            switch (((Operation<T>) num))
            {
                case Operation<T>.OP_ADD:
                    this.m_Objects.Add(item);
                    break;

                case Operation<T>.OP_CLEAR:
                    this.m_Objects.Clear();
                    break;

                case Operation<T>.OP_INSERT:
                    this.m_Objects.Insert(index, item);
                    break;

                case Operation<T>.OP_REMOVE:
                    this.m_Objects.Remove(item);
                    break;

                case Operation<T>.OP_REMOVEAT:
                    this.m_Objects.RemoveAt(index);
                    break;

                case Operation<T>.OP_SET:
                case Operation<T>.OP_DIRTY:
                    this.m_Objects[index] = item;
                    break;
            }
            if (this.m_Callback != null)
            {
                this.m_Callback((Operation<T>) num, index);
            }
        }

        public int IndexOf(T item)
        {
            return this.m_Objects.IndexOf(item);
        }

        public void InitializeBehaviour(NetworkBehaviour beh, int cmdHash)
        {
            this.m_Behaviour = beh;
            this.m_CmdHash = cmdHash;
        }

        public void Insert(int index, T item)
        {
            this.m_Objects.Insert(index, item);
            this.SendMsg(Operation<T>.OP_INSERT, index, item);
        }

        public bool Remove(T item)
        {
            bool flag = this.m_Objects.Remove(item);
            if (flag)
            {
                this.SendMsg(Operation<T>.OP_REMOVE, 0, item);
            }
            return flag;
        }

        public void RemoveAt(int index)
        {
            this.m_Objects.RemoveAt(index);
            this.SendMsg(Operation<T>.OP_REMOVEAT, index);
        }

        private void SendMsg(Operation<T> op, int itemIndex)
        {
            this.SendMsg(op, itemIndex, default(T));
        }

        private void SendMsg(Operation<T> op, int itemIndex, T item)
        {
            if (this.m_Behaviour == null)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("SyncList not initialized");
                }
            }
            else
            {
                NetworkIdentity component = this.m_Behaviour.GetComponent<NetworkIdentity>();
                if (component == null)
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError("SyncList no NetworkIdentity");
                    }
                }
                else
                {
                    NetworkWriter writer = new NetworkWriter();
                    writer.StartMessage(9);
                    writer.Write(component.netId);
                    writer.WritePackedUInt32((uint) this.m_CmdHash);
                    writer.Write((byte) op);
                    writer.WritePackedUInt32((uint) itemIndex);
                    this.SerializeItem(writer, item);
                    writer.FinishMessage();
                    NetworkServer.SendWriterToReady(component.gameObject, writer, 0);
                    NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Outgoing, 9, op.ToString(), 1);
                    if ((this.m_Behaviour.isServer && this.m_Behaviour.isClient) && (this.m_Callback != null))
                    {
                        this.m_Callback(op, itemIndex);
                    }
                }
            }
        }

        protected abstract void SerializeItem(NetworkWriter writer, T item);
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public SyncListChanged<T> Callback
        {
            get
            {
                return this.m_Callback;
            }
            set
            {
                this.m_Callback = value;
            }
        }

        public int Count
        {
            get
            {
                return this.m_Objects.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public T this[int i]
        {
            get
            {
                return this.m_Objects[i];
            }
            set
            {
                this.m_Objects[i] = value;
                this.SendMsg(Operation<T>.OP_SET, i, value);
            }
        }

        public enum Operation
        {
            public const SyncList<T>.Operation OP_ADD = SyncList<T>.Operation.OP_ADD;,
            public const SyncList<T>.Operation OP_CLEAR = SyncList<T>.Operation.OP_CLEAR;,
            public const SyncList<T>.Operation OP_DIRTY = SyncList<T>.Operation.OP_DIRTY;,
            public const SyncList<T>.Operation OP_INSERT = SyncList<T>.Operation.OP_INSERT;,
            public const SyncList<T>.Operation OP_REMOVE = SyncList<T>.Operation.OP_REMOVE;,
            public const SyncList<T>.Operation OP_REMOVEAT = SyncList<T>.Operation.OP_REMOVEAT;,
            public const SyncList<T>.Operation OP_SET = SyncList<T>.Operation.OP_SET;
        }

        public delegate void SyncListChanged(SyncList<T>.Operation op, int itemIndex);
    }
}

