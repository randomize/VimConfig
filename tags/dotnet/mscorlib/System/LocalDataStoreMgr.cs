namespace System
{
    using System.Collections;
    using System.Runtime.CompilerServices;
    using System.Threading;

    internal class LocalDataStoreMgr
    {
        private const byte DataSlotOccupied = 1;
        private const int InitialSlotTableSize = 0x40;
        private const int LargeSlotTableSizeIncrease = 0x80;
        private int m_FirstAvailableSlot;
        private Hashtable m_KeyToSlotMap = new Hashtable();
        private ArrayList m_ManagedLocalDataStores = new ArrayList();
        private byte[] m_SlotInfoTable = new byte[0x40];
        private const int SlotTableDoubleThreshold = 0x200;

        public LocalDataStoreSlot AllocateDataSlot()
        {
            LocalDataStoreSlot slot2;
            bool tookLock = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                LocalDataStoreSlot slot;
                int num3;
                Monitor.ReliableEnter(this, ref tookLock);
                int length = this.m_SlotInfoTable.Length;
                if (this.m_FirstAvailableSlot < length)
                {
                    slot = new LocalDataStoreSlot(this, this.m_FirstAvailableSlot);
                    this.m_SlotInfoTable[this.m_FirstAvailableSlot] = 1;
                    int index = this.m_FirstAvailableSlot + 1;
                    while (index < length)
                    {
                        if ((this.m_SlotInfoTable[index] & 1) == 0)
                        {
                            break;
                        }
                        index++;
                    }
                    this.m_FirstAvailableSlot = index;
                    return slot;
                }
                if (length < 0x200)
                {
                    num3 = length * 2;
                }
                else
                {
                    num3 = length + 0x80;
                }
                byte[] destinationArray = new byte[num3];
                Array.Copy(this.m_SlotInfoTable, destinationArray, length);
                this.m_SlotInfoTable = destinationArray;
                slot = new LocalDataStoreSlot(this, length);
                this.m_SlotInfoTable[length] = 1;
                this.m_FirstAvailableSlot = length + 1;
                slot2 = slot;
            }
            finally
            {
                if (tookLock)
                {
                    Monitor.Exit(this);
                }
            }
            return slot2;
        }

        public LocalDataStoreSlot AllocateNamedDataSlot(string name)
        {
            LocalDataStoreSlot slot2;
            bool tookLock = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                Monitor.ReliableEnter(this, ref tookLock);
                LocalDataStoreSlot slot = this.AllocateDataSlot();
                this.m_KeyToSlotMap.Add(name, slot);
                slot2 = slot;
            }
            finally
            {
                if (tookLock)
                {
                    Monitor.Exit(this);
                }
            }
            return slot2;
        }

        public LocalDataStore CreateLocalDataStore()
        {
            LocalDataStore store = new LocalDataStore(this, this.m_SlotInfoTable.Length);
            bool tookLock = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                Monitor.ReliableEnter(this, ref tookLock);
                this.m_ManagedLocalDataStores.Add(store);
            }
            finally
            {
                if (tookLock)
                {
                    Monitor.Exit(this);
                }
            }
            return store;
        }

        public void DeleteLocalDataStore(LocalDataStore store)
        {
            bool tookLock = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                Monitor.ReliableEnter(this, ref tookLock);
                this.m_ManagedLocalDataStores.Remove(store);
            }
            finally
            {
                if (tookLock)
                {
                    Monitor.Exit(this);
                }
            }
        }

        internal void FreeDataSlot(int slot)
        {
            bool tookLock = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                Monitor.ReliableEnter(this, ref tookLock);
                for (int i = 0; i < this.m_ManagedLocalDataStores.Count; i++)
                {
                    ((LocalDataStore) this.m_ManagedLocalDataStores[i]).SetDataInternal(slot, null, false);
                }
                this.m_SlotInfoTable[slot] = 0;
                if (slot < this.m_FirstAvailableSlot)
                {
                    this.m_FirstAvailableSlot = slot;
                }
            }
            finally
            {
                if (tookLock)
                {
                    Monitor.Exit(this);
                }
            }
        }

        public void FreeNamedDataSlot(string name)
        {
            bool tookLock = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                Monitor.ReliableEnter(this, ref tookLock);
                this.m_KeyToSlotMap.Remove(name);
            }
            finally
            {
                if (tookLock)
                {
                    Monitor.Exit(this);
                }
            }
        }

        public LocalDataStoreSlot GetNamedDataSlot(string name)
        {
            LocalDataStoreSlot slot2;
            bool tookLock = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                Monitor.ReliableEnter(this, ref tookLock);
                LocalDataStoreSlot slot = (LocalDataStoreSlot) this.m_KeyToSlotMap[name];
                if (slot == null)
                {
                    return this.AllocateNamedDataSlot(name);
                }
                slot2 = slot;
            }
            finally
            {
                if (tookLock)
                {
                    Monitor.Exit(this);
                }
            }
            return slot2;
        }

        internal int GetSlotTableLength()
        {
            return this.m_SlotInfoTable.Length;
        }

        public void ValidateSlot(LocalDataStoreSlot slot)
        {
            if ((slot == null) || (slot.Manager != this))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_ALSInvalidSlot"));
            }
        }
    }
}

