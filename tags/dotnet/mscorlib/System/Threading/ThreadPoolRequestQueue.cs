namespace System.Threading
{
    using System;
    using System.Runtime.CompilerServices;

    internal sealed class ThreadPoolRequestQueue
    {
        private uint tpCount;
        private _ThreadPoolWaitCallback tpHead;
        private object tpSync = new object();
        private _ThreadPoolWaitCallback tpTail;

        public _ThreadPoolWaitCallback DeQueue()
        {
            bool flag = false;
            _ThreadPoolWaitCallback callback = null;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
            }
            finally
            {
                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                    Monitor.Enter(this.tpSync);
                    flag = true;
                }
                catch (Exception)
                {
                }
                if (flag)
                {
                    _ThreadPoolWaitCallback tpHead = this.tpHead;
                    if (tpHead != null)
                    {
                        callback = tpHead;
                        this.tpHead = tpHead._next;
                        this.tpCount--;
                        if (this.tpCount == 0)
                        {
                            this.tpTail = null;
                            ThreadPool.ClearAppDomainRequestActive();
                        }
                    }
                    Monitor.Exit(this.tpSync);
                }
            }
            return callback;
        }

        public uint EnQueue(_ThreadPoolWaitCallback tpcallBack)
        {
            uint tpCount = 0;
            bool flag = false;
            bool flag2 = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
            }
            finally
            {
                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                    Monitor.Enter(this.tpSync);
                    flag = true;
                }
                catch (Exception)
                {
                }
                if (flag)
                {
                    if (this.tpCount == 0)
                    {
                        flag2 = ThreadPool.SetAppDomainRequestActive();
                    }
                    this.tpCount++;
                    tpCount = this.tpCount;
                    if (this.tpHead == null)
                    {
                        this.tpHead = tpcallBack;
                        this.tpTail = tpcallBack;
                    }
                    else
                    {
                        this.tpTail._next = tpcallBack;
                        this.tpTail = tpcallBack;
                    }
                    Monitor.Exit(this.tpSync);
                    if (flag2)
                    {
                        ThreadPool.SetNativeTpEvent();
                    }
                }
            }
            return tpCount;
        }

        public uint GetQueueCount()
        {
            return this.tpCount;
        }
    }
}

