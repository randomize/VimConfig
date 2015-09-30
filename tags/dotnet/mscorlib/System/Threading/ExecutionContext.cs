namespace System.Threading
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.Remoting.Messaging;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Security.Permissions;

    [Serializable]
    public sealed class ExecutionContext : ISerializable
    {
        private System.Threading.HostExecutionContext _hostExecutionContext;
        private System.Runtime.Remoting.Messaging.IllogicalCallContext _illogicalCallContext;
        private System.Runtime.Remoting.Messaging.LogicalCallContext _logicalCallContext;
        private System.Security.SecurityContext _securityContext;
        private System.Threading.SynchronizationContext _syncContext;
        private System.Threading.Thread _thread;
        internal static RuntimeHelpers.CleanupCode cleanupCode;
        internal bool isFlowSuppressed;
        internal bool isNewCapture;
        internal static RuntimeHelpers.TryCode tryCode;

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        internal ExecutionContext()
        {
        }

        private ExecutionContext(SerializationInfo info, StreamingContext context)
        {
            SerializationInfoEnumerator enumerator = info.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Name.Equals("LogicalCallContext"))
                {
                    this._logicalCallContext = (System.Runtime.Remoting.Messaging.LogicalCallContext) enumerator.Value;
                }
            }
            this.Thread = System.Threading.Thread.CurrentThread;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static ExecutionContext Capture()
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return Capture(ref lookForMyCaller);
        }

        internal static ExecutionContext Capture(ref StackCrawlMark stackMark)
        {
            if (IsFlowSuppressed())
            {
                return null;
            }
            ExecutionContext executionContextNoCreate = System.Threading.Thread.CurrentThread.GetExecutionContextNoCreate();
            ExecutionContext context2 = new ExecutionContext {
                isNewCapture = true,
                SecurityContext = System.Security.SecurityContext.Capture(executionContextNoCreate, ref stackMark)
            };
            if (context2.SecurityContext != null)
            {
                context2.SecurityContext.ExecutionContext = context2;
            }
            context2._hostExecutionContext = HostExecutionContextManager.CaptureHostExecutionContext();
            if (executionContextNoCreate != null)
            {
                context2._syncContext = (executionContextNoCreate._syncContext == null) ? null : executionContextNoCreate._syncContext.CreateCopy();
                if (executionContextNoCreate._logicalCallContext != null)
                {
                    System.Runtime.Remoting.Messaging.LogicalCallContext logicalCallContext = executionContextNoCreate.LogicalCallContext;
                    context2.LogicalCallContext = (System.Runtime.Remoting.Messaging.LogicalCallContext) logicalCallContext.Clone();
                }
            }
            return context2;
        }

        internal static void ClearSyncContext(ExecutionContext ec)
        {
            if (ec != null)
            {
                ec.SynchronizationContext = null;
            }
        }

        public ExecutionContext CreateCopy()
        {
            if (!this.isNewCapture)
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_CannotCopyUsedContext"));
            }
            ExecutionContext context = new ExecutionContext {
                isNewCapture = true,
                _syncContext = (this._syncContext == null) ? null : this._syncContext.CreateCopy(),
                _hostExecutionContext = (this._hostExecutionContext == null) ? null : this._hostExecutionContext.CreateCopy()
            };
            if (this._securityContext != null)
            {
                context._securityContext = this._securityContext.CreateCopy();
                context._securityContext.ExecutionContext = context;
            }
            if (this._logicalCallContext != null)
            {
                System.Runtime.Remoting.Messaging.LogicalCallContext logicalCallContext = this.LogicalCallContext;
                context.LogicalCallContext = (System.Runtime.Remoting.Messaging.LogicalCallContext) logicalCallContext.Clone();
            }
            if (this._illogicalCallContext != null)
            {
                System.Runtime.Remoting.Messaging.IllogicalCallContext illogicalCallContext = this.IllogicalCallContext;
                context.IllogicalCallContext = (System.Runtime.Remoting.Messaging.IllogicalCallContext) illogicalCallContext.Clone();
            }
            return context;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            if (this._logicalCallContext != null)
            {
                info.AddValue("LogicalCallContext", this._logicalCallContext, typeof(System.Runtime.Remoting.Messaging.LogicalCallContext));
            }
        }

        internal bool IsDefaultFTContext()
        {
            if (this._hostExecutionContext != null)
            {
                return false;
            }
            if (this._syncContext != null)
            {
                return false;
            }
            if ((this._securityContext != null) && !this._securityContext.IsDefaultFTSecurityContext())
            {
                return false;
            }
            if ((this._logicalCallContext != null) && this._logicalCallContext.HasInfo)
            {
                return false;
            }
            if ((this._illogicalCallContext != null) && this._illogicalCallContext.HasUserData)
            {
                return false;
            }
            return true;
        }

        public static bool IsFlowSuppressed()
        {
            ExecutionContext executionContextNoCreate = System.Threading.Thread.CurrentThread.GetExecutionContextNoCreate();
            if (executionContextNoCreate == null)
            {
                return false;
            }
            return executionContextNoCreate.isFlowSuppressed;
        }

        public static void RestoreFlow()
        {
            ExecutionContext executionContextNoCreate = System.Threading.Thread.CurrentThread.GetExecutionContextNoCreate();
            if ((executionContextNoCreate == null) || !executionContextNoCreate.isFlowSuppressed)
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_CannotRestoreUnsupressedFlow"));
            }
            executionContextNoCreate.isFlowSuppressed = false;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        public static void Run(ExecutionContext executionContext, ContextCallback callback, object state)
        {
            if (executionContext == null)
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_NullContext"));
            }
            if (!executionContext.isNewCapture)
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_NotNewCaptureContext"));
            }
            executionContext.isNewCapture = false;
            ExecutionContext executionContextNoCreate = System.Threading.Thread.CurrentThread.GetExecutionContextNoCreate();
            if (((executionContextNoCreate == null) || executionContextNoCreate.IsDefaultFTContext()) && (System.Security.SecurityContext.CurrentlyInDefaultFTSecurityContext(executionContextNoCreate) && executionContext.IsDefaultFTContext()))
            {
                callback(state);
            }
            else
            {
                RunInternal(executionContext, callback, state);
            }
        }

        [PrePrepareMethod]
        internal static void runFinallyCode(object userData, bool exceptionThrown)
        {
            ExecutionContextRunData data = (ExecutionContextRunData) userData;
            data.ecsw.Undo();
        }

        internal static void RunInternal(ExecutionContext executionContext, ContextCallback callback, object state)
        {
            if (cleanupCode == null)
            {
                tryCode = new RuntimeHelpers.TryCode(ExecutionContext.runTryCode);
                cleanupCode = new RuntimeHelpers.CleanupCode(ExecutionContext.runFinallyCode);
            }
            ExecutionContextRunData userData = new ExecutionContextRunData(executionContext, callback, state);
            RuntimeHelpers.ExecuteCodeWithGuaranteedCleanup(tryCode, cleanupCode, userData);
        }

        internal static void runTryCode(object userData)
        {
            ExecutionContextRunData data = (ExecutionContextRunData) userData;
            data.ecsw = SetExecutionContext(data.ec);
            data.callBack(data.state);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static ExecutionContextSwitcher SetExecutionContext(ExecutionContext executionContext)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            ExecutionContextSwitcher switcher = new ExecutionContextSwitcher {
                thread = System.Threading.Thread.CurrentThread,
                prevEC = System.Threading.Thread.CurrentThread.GetExecutionContextNoCreate(),
                currEC = executionContext
            };
            System.Threading.Thread.CurrentThread.SetExecutionContext(executionContext);
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                if (executionContext == null)
                {
                    return switcher;
                }
                System.Security.SecurityContext securityContext = executionContext.SecurityContext;
                if (securityContext != null)
                {
                    System.Security.SecurityContext prevSecurityContext = (switcher.prevEC != null) ? switcher.prevEC.SecurityContext : null;
                    switcher.scsw = System.Security.SecurityContext.SetSecurityContext(securityContext, prevSecurityContext, ref lookForMyCaller);
                }
                else if (!System.Security.SecurityContext.CurrentlyInDefaultFTSecurityContext(switcher.prevEC))
                {
                    System.Security.SecurityContext context3 = (switcher.prevEC != null) ? switcher.prevEC.SecurityContext : null;
                    switcher.scsw = System.Security.SecurityContext.SetSecurityContext(System.Security.SecurityContext.FullTrustSecurityContext, context3, ref lookForMyCaller);
                }
                System.Threading.SynchronizationContext synchronizationContext = executionContext.SynchronizationContext;
                if (synchronizationContext != null)
                {
                    System.Threading.SynchronizationContext prevSyncContext = (switcher.prevEC != null) ? switcher.prevEC.SynchronizationContext : null;
                    switcher.sysw = System.Threading.SynchronizationContext.SetSynchronizationContext(synchronizationContext, prevSyncContext);
                }
                System.Threading.HostExecutionContext hostExecutionContext = executionContext.HostExecutionContext;
                if (hostExecutionContext != null)
                {
                    switcher.hecsw = HostExecutionContextManager.SetHostExecutionContextInternal(hostExecutionContext);
                }
            }
            catch
            {
                switcher.UndoNoThrow();
                throw;
            }
            return switcher;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        public static AsyncFlowControl SuppressFlow()
        {
            if (IsFlowSuppressed())
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_CannotSupressFlowMultipleTimes"));
            }
            AsyncFlowControl control = new AsyncFlowControl();
            control.Setup();
            return control;
        }

        internal System.Threading.HostExecutionContext HostExecutionContext
        {
            get
            {
                return this._hostExecutionContext;
            }
            set
            {
                this._hostExecutionContext = value;
            }
        }

        internal System.Runtime.Remoting.Messaging.IllogicalCallContext IllogicalCallContext
        {
            get
            {
                if (this._illogicalCallContext == null)
                {
                    this._illogicalCallContext = new System.Runtime.Remoting.Messaging.IllogicalCallContext();
                }
                return this._illogicalCallContext;
            }
            set
            {
                this._illogicalCallContext = value;
            }
        }

        internal System.Runtime.Remoting.Messaging.LogicalCallContext LogicalCallContext
        {
            get
            {
                if (this._logicalCallContext == null)
                {
                    this._logicalCallContext = new System.Runtime.Remoting.Messaging.LogicalCallContext();
                }
                return this._logicalCallContext;
            }
            set
            {
                this._logicalCallContext = value;
            }
        }

        internal System.Security.SecurityContext SecurityContext
        {
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            get
            {
                return this._securityContext;
            }
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            set
            {
                this._securityContext = value;
                if (value != null)
                {
                    this._securityContext.ExecutionContext = this;
                }
            }
        }

        internal System.Threading.SynchronizationContext SynchronizationContext
        {
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            get
            {
                return this._syncContext;
            }
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            set
            {
                this._syncContext = value;
            }
        }

        internal System.Threading.Thread Thread
        {
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            set
            {
                this._thread = value;
            }
        }

        internal class ExecutionContextRunData
        {
            internal ContextCallback callBack;
            internal ExecutionContext ec;
            internal ExecutionContextSwitcher ecsw;
            internal object state;

            internal ExecutionContextRunData(ExecutionContext executionContext, ContextCallback cb, object state)
            {
                this.ec = executionContext;
                this.callBack = cb;
                this.state = state;
                this.ecsw = new ExecutionContextSwitcher();
            }
        }
    }
}

