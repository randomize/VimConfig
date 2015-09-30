namespace System.Threading
{
    using System;

    internal class _ThreadPoolWaitOrTimerCallback
    {
        private static ContextCallback _ccbf = new ContextCallback(_ThreadPoolWaitOrTimerCallback.WaitOrTimerCallback_Context_f);
        private static ContextCallback _ccbt = new ContextCallback(_ThreadPoolWaitOrTimerCallback.WaitOrTimerCallback_Context_t);
        private ExecutionContext _executionContext;
        private object _state;
        private WaitOrTimerCallback _waitOrTimerCallback;

        internal _ThreadPoolWaitOrTimerCallback(WaitOrTimerCallback waitOrTimerCallback, object state, bool compressStack, ref StackCrawlMark stackMark)
        {
            this._waitOrTimerCallback = waitOrTimerCallback;
            this._state = state;
            if (compressStack && !ExecutionContext.IsFlowSuppressed())
            {
                this._executionContext = ExecutionContext.Capture(ref stackMark);
                ExecutionContext.ClearSyncContext(this._executionContext);
            }
        }

        internal static void PerformWaitOrTimerCallback(object state, bool timedOut)
        {
            _ThreadPoolWaitOrTimerCallback callback = (_ThreadPoolWaitOrTimerCallback) state;
            if (callback._executionContext == null)
            {
                callback._waitOrTimerCallback(callback._state, timedOut);
            }
            else if (timedOut)
            {
                ExecutionContext.Run(callback._executionContext.CreateCopy(), _ccbt, callback);
            }
            else
            {
                ExecutionContext.Run(callback._executionContext.CreateCopy(), _ccbf, callback);
            }
        }

        private static void WaitOrTimerCallback_Context(object state, bool timedOut)
        {
            _ThreadPoolWaitOrTimerCallback callback = (_ThreadPoolWaitOrTimerCallback) state;
            callback._waitOrTimerCallback(callback._state, timedOut);
        }

        private static void WaitOrTimerCallback_Context_f(object state)
        {
            WaitOrTimerCallback_Context(state, false);
        }

        private static void WaitOrTimerCallback_Context_t(object state)
        {
            WaitOrTimerCallback_Context(state, true);
        }
    }
}

