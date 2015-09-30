namespace System.Threading
{
    using System;

    internal class _ThreadPoolWaitCallback
    {
        internal static ContextCallback _ccb = new ContextCallback(_ThreadPoolWaitCallback.WaitCallback_Context);
        private ExecutionContext _executionContext;
        protected internal _ThreadPoolWaitCallback _next;
        private object _state;
        private WaitCallback _waitCallback;

        internal _ThreadPoolWaitCallback(WaitCallback waitCallback, object state, bool compressStack, ref StackCrawlMark stackMark)
        {
            this._waitCallback = waitCallback;
            this._state = state;
            if (compressStack && !ExecutionContext.IsFlowSuppressed())
            {
                this._executionContext = ExecutionContext.Capture(ref stackMark);
                ExecutionContext.ClearSyncContext(this._executionContext);
            }
        }

        internal static void PerformWaitCallback(object state)
        {
            int num = 0;
            _ThreadPoolWaitCallback tpWaitCallBack = null;
            int tickCount = Environment.TickCount;
        Label_000A:
            tpWaitCallBack = ThreadPoolGlobals.tpQueue.DeQueue();
            if (tpWaitCallBack != null)
            {
                ThreadPool.CompleteThreadPoolRequest(ThreadPoolGlobals.tpQueue.GetQueueCount());
                PerformWaitCallbackInternal(tpWaitCallBack);
                num = Environment.TickCount - tickCount;
                if ((num <= ThreadPoolGlobals.tpQuantum) || !ThreadPool.ShouldReturnToVm())
                {
                    goto Label_000A;
                }
            }
        }

        internal static void PerformWaitCallbackInternal(_ThreadPoolWaitCallback tpWaitCallBack)
        {
            if (tpWaitCallBack._executionContext == null)
            {
                tpWaitCallBack._waitCallback(tpWaitCallBack._state);
            }
            else
            {
                ExecutionContext.Run(tpWaitCallBack._executionContext, _ccb, tpWaitCallBack);
            }
        }

        internal static void WaitCallback_Context(object state)
        {
            _ThreadPoolWaitCallback callback = (_ThreadPoolWaitCallback) state;
            callback._waitCallback(callback._state);
        }
    }
}

