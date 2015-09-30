namespace System.Threading
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Security.Permissions;

    [SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.ControlPolicy | SecurityPermissionFlag.ControlEvidence)]
    public class SynchronizationContext
    {
        private SynchronizationContextProperties _props;

        public virtual SynchronizationContext CreateCopy()
        {
            return new SynchronizationContext();
        }

        private static int InvokeWaitMethodHelper(SynchronizationContext syncContext, IntPtr[] waitHandles, bool waitAll, int millisecondsTimeout)
        {
            return syncContext.Wait(waitHandles, waitAll, millisecondsTimeout);
        }

        public bool IsWaitNotificationRequired()
        {
            return ((this._props & SynchronizationContextProperties.RequireWaitNotification) != SynchronizationContextProperties.None);
        }

        public virtual void OperationCompleted()
        {
        }

        public virtual void OperationStarted()
        {
        }

        public virtual void Post(SendOrPostCallback d, object state)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(d.Invoke), state);
        }

        public virtual void Send(SendOrPostCallback d, object state)
        {
            d(state);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.ControlPolicy | SecurityPermissionFlag.ControlEvidence)]
        public static void SetSynchronizationContext(SynchronizationContext syncContext)
        {
            SetSynchronizationContext(syncContext, Thread.CurrentThread.ExecutionContext.SynchronizationContext);
        }

        internal static SynchronizationContextSwitcher SetSynchronizationContext(SynchronizationContext syncContext, SynchronizationContext prevSyncContext)
        {
            ExecutionContext executionContext = Thread.CurrentThread.ExecutionContext;
            SynchronizationContextSwitcher switcher = new SynchronizationContextSwitcher();
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                switcher._ec = executionContext;
                switcher.savedSC = prevSyncContext;
                switcher.currSC = syncContext;
                executionContext.SynchronizationContext = syncContext;
            }
            catch
            {
                switcher.UndoNoThrow();
                throw;
            }
            return switcher;
        }

        protected void SetWaitNotificationRequired()
        {
            RuntimeHelpers.PrepareDelegate(new WaitDelegate(this.Wait));
            this._props |= SynchronizationContextProperties.RequireWaitNotification;
        }

        [CLSCompliant(false), PrePrepareMethod, SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.ControlPolicy | SecurityPermissionFlag.ControlEvidence)]
        public virtual int Wait(IntPtr[] waitHandles, bool waitAll, int millisecondsTimeout)
        {
            if (waitHandles == null)
            {
                throw new ArgumentNullException("waitHandles");
            }
            return WaitHelper(waitHandles, waitAll, millisecondsTimeout);
        }

        [MethodImpl(MethodImplOptions.InternalCall), CLSCompliant(false), PrePrepareMethod, ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.ControlPolicy | SecurityPermissionFlag.ControlEvidence)]
        protected static extern int WaitHelper(IntPtr[] waitHandles, bool waitAll, int millisecondsTimeout);

        public static SynchronizationContext Current
        {
            get
            {
                ExecutionContext executionContextNoCreate = Thread.CurrentThread.GetExecutionContextNoCreate();
                if (executionContextNoCreate != null)
                {
                    return executionContextNoCreate.SynchronizationContext;
                }
                return null;
            }
        }
    }
}

