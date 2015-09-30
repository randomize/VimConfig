namespace System
{
    using System.Runtime.CompilerServices;
    using System.Threading;

    internal class LocalDataStore
    {
        private int DONT_USE_InternalStore;
        private object[] m_DataTable;
        private LocalDataStoreMgr m_Manager;

        public LocalDataStore(LocalDataStoreMgr mgr, int InitialCapacity)
        {
            if (mgr == null)
            {
                throw new ArgumentNullException("mgr");
            }
            this.m_Manager = mgr;
            this.m_DataTable = new object[InitialCapacity];
        }

        public object GetData(LocalDataStoreSlot slot)
        {
            object obj2 = null;
            this.m_Manager.ValidateSlot(slot);
            int index = slot.Slot;
            if (index >= 0)
            {
                if (index >= this.m_DataTable.Length)
                {
                    return null;
                }
                obj2 = this.m_DataTable[index];
            }
            if (!slot.IsValid())
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_SlotHasBeenFreed"));
            }
            return obj2;
        }

        private void SetCapacity(int capacity)
        {
            if (capacity < this.m_DataTable.Length)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_ALSInvalidCapacity"));
            }
            object[] destinationArray = new object[capacity];
            Array.Copy(this.m_DataTable, destinationArray, this.m_DataTable.Length);
            this.m_DataTable = destinationArray;
        }

        public void SetData(LocalDataStoreSlot slot, object data)
        {
            this.m_Manager.ValidateSlot(slot);
            bool tookLock = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                Monitor.ReliableEnter(this.m_Manager, ref tookLock);
                if (!slot.IsValid())
                {
                    throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_SlotHasBeenFreed"));
                }
                this.SetDataInternal(slot.Slot, data, true);
            }
            finally
            {
                if (tookLock)
                {
                    Monitor.Exit(this.m_Manager);
                }
            }
        }

        internal void SetDataInternal(int slot, object data, bool bAlloc)
        {
            if (slot >= this.m_DataTable.Length)
            {
                if (!bAlloc)
                {
                    return;
                }
                this.SetCapacity(this.m_Manager.GetSlotTableLength());
            }
            this.m_DataTable[slot] = data;
        }
    }
}

