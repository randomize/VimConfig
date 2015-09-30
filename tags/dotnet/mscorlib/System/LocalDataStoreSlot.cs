namespace System
{
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    [ComVisible(true)]
    public sealed class LocalDataStoreSlot
    {
        private static LdsSyncHelper m_helper = new LdsSyncHelper();
        private LocalDataStoreMgr m_mgr;
        private int m_slot;

        internal LocalDataStoreSlot(LocalDataStoreMgr mgr, int slot)
        {
            this.m_mgr = mgr;
            this.m_slot = slot;
        }

        ~LocalDataStoreSlot()
        {
            int slot = this.m_slot;
            bool tookLock = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                Monitor.ReliableEnter(this.m_mgr, ref tookLock);
                this.m_slot = -1;
                this.m_mgr.FreeDataSlot(slot);
            }
            finally
            {
                if (tookLock)
                {
                    Monitor.Exit(this.m_mgr);
                }
            }
        }

        internal bool IsValid()
        {
            return (m_helper.Get(ref this.m_slot) != -1);
        }

        internal LocalDataStoreMgr Manager
        {
            get
            {
                return this.m_mgr;
            }
        }

        internal int Slot
        {
            get
            {
                return this.m_slot;
            }
        }
    }
}

