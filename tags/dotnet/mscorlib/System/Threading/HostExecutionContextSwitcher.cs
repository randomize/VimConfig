namespace System.Threading
{
    using System;
    using System.Runtime.ConstrainedExecution;

    internal class HostExecutionContextSwitcher
    {
        internal HostExecutionContext currentHostContext;
        internal ExecutionContext executionContext;
        internal HostExecutionContext previousHostContext;

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static void Undo(object switcherObject)
        {
            if (switcherObject != null)
            {
                HostExecutionContextManager currentHostExecutionContextManager = HostExecutionContextManager.GetCurrentHostExecutionContextManager();
                if (currentHostExecutionContextManager != null)
                {
                    currentHostExecutionContextManager.Revert(switcherObject);
                }
            }
        }
    }
}

