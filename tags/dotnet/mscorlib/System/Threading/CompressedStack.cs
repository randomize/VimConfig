namespace System.Threading
{
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Security.Permissions;

    [Serializable]
    public sealed class CompressedStack : ISerializable
    {
        internal static RuntimeHelpers.CleanupCode cleanupCode;
        private bool m_canSkipEvaluation;
        private SafeCompressedStackHandle m_csHandle;
        private PermissionListSet m_pls;
        internal static RuntimeHelpers.TryCode tryCode;

        internal CompressedStack(SafeCompressedStackHandle csHandle)
        {
            this.m_csHandle = csHandle;
        }

        private CompressedStack(SerializationInfo info, StreamingContext context)
        {
            this.m_pls = (PermissionListSet) info.GetValue("PLS", typeof(PermissionListSet));
        }

        private CompressedStack(SafeCompressedStackHandle csHandle, PermissionListSet pls)
        {
            this.m_csHandle = csHandle;
            this.m_pls = pls;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static CompressedStack Capture()
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return GetCompressedStack(ref lookForMyCaller);
        }

        internal bool CheckDemand(CodeAccessPermission demand, PermissionToken permToken, RuntimeMethodHandle rmh)
        {
            this.CompleteConstruction(null);
            if (this.PLS != null)
            {
                this.PLS.CheckDemand(demand, permToken, rmh);
            }
            return false;
        }

        internal bool CheckDemandNoHalt(CodeAccessPermission demand, PermissionToken permToken, RuntimeMethodHandle rmh)
        {
            this.CompleteConstruction(null);
            return ((this.PLS == null) || this.PLS.CheckDemand(demand, permToken, rmh));
        }

        internal bool CheckSetDemand(PermissionSet pset, RuntimeMethodHandle rmh)
        {
            this.CompleteConstruction(null);
            if (this.PLS == null)
            {
                return false;
            }
            return this.PLS.CheckSetDemand(pset, rmh);
        }

        internal bool CheckSetDemandWithModificationNoHalt(PermissionSet pset, out PermissionSet alteredDemandSet, RuntimeMethodHandle rmh)
        {
            alteredDemandSet = null;
            this.CompleteConstruction(null);
            return ((this.PLS == null) || this.PLS.CheckSetDemandWithModification(pset, out alteredDemandSet, rmh));
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        internal void CompleteConstruction(CompressedStack innerCS)
        {
            if (this.PLS == null)
            {
                PermissionListSet set = PermissionListSet.CreateCompressedState(this, innerCS);
                lock (this)
                {
                    if (this.PLS == null)
                    {
                        this.m_pls = set;
                    }
                }
            }
        }

        [ComVisible(false)]
        public CompressedStack CreateCopy()
        {
            return new CompressedStack(this.m_csHandle, this.m_pls);
        }

        internal void DemandFlagsOrGrantSet(int flags, PermissionSet grantSet)
        {
            this.CompleteConstruction(null);
            if (this.PLS != null)
            {
                this.PLS.DemandFlagsOrGrantSet(flags, grantSet);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        internal static extern void DestroyDCSList(SafeCompressedStackHandle compressedStack);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void DestroyDelayedCompressedStack(IntPtr unmanagedCompressedStack);
        [MethodImpl(MethodImplOptions.NoInlining), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode), StrongNameIdentityPermission(SecurityAction.LinkDemand, PublicKey="0x00000000000000000400000000000000")]
        public static CompressedStack GetCompressedStack()
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return GetCompressedStack(ref lookForMyCaller);
        }

        internal static CompressedStack GetCompressedStack(ref StackCrawlMark stackMark)
        {
            CompressedStack innerCS = null;
            if (CodeAccessSecurityEngine.QuickCheckForAllDemands())
            {
                return new CompressedStack(null) { CanSkipEvaluation = true };
            }
            if (CodeAccessSecurityEngine.AllDomainsHomogeneousWithNoStackModifiers())
            {
                return new CompressedStack(GetDelayedCompressedStack(ref stackMark, false)) { m_pls = PermissionListSet.CreateCompressedState_HG() };
            }
            CompressedStack stack = new CompressedStack(null);
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
            }
            finally
            {
                stack.CompressedStackHandle = GetDelayedCompressedStack(ref stackMark, true);
                if ((stack.CompressedStackHandle != null) && IsImmediateCompletionCandidate(stack.CompressedStackHandle, out innerCS))
                {
                    try
                    {
                        stack.CompleteConstruction(innerCS);
                    }
                    finally
                    {
                        DestroyDCSList(stack.CompressedStackHandle);
                    }
                }
            }
            return stack;
        }

        internal static CompressedStack GetCompressedStackThread()
        {
            ExecutionContext executionContextNoCreate = Thread.CurrentThread.GetExecutionContextNoCreate();
            if ((executionContextNoCreate != null) && (executionContextNoCreate.SecurityContext != null))
            {
                return executionContextNoCreate.SecurityContext.CompressedStack;
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int GetDCSCount(SafeCompressedStackHandle compressedStack);
        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        internal static extern SafeCompressedStackHandle GetDelayedCompressedStack(ref StackCrawlMark stackMark, bool walkStack);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern DomainCompressedStack GetDomainCompressedStack(SafeCompressedStackHandle compressedStack, int index);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void GetHomogeneousPLS(PermissionListSet hgPLS);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            this.CompleteConstruction(null);
            info.AddValue("PLS", this.m_pls);
        }

        internal void GetZoneAndOrigin(ArrayList zoneList, ArrayList originList, PermissionToken zoneToken, PermissionToken originToken)
        {
            this.CompleteConstruction(null);
            if (this.PLS != null)
            {
                this.PLS.GetZoneAndOrigin(zoneList, originList, zoneToken, originToken);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        internal static extern bool IsImmediateCompletionCandidate(SafeCompressedStackHandle compressedStack, out CompressedStack innerCS);
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        internal static void RestoreAppDomainStack(IntPtr appDomainStack)
        {
            Thread.CurrentThread.RestoreAppDomainStack(appDomainStack);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        public static void Run(CompressedStack compressedStack, ContextCallback callback, object state)
        {
            if (compressedStack == null)
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_NamedParamNull"), "compressedStack");
            }
            if (cleanupCode == null)
            {
                tryCode = new RuntimeHelpers.TryCode(CompressedStack.runTryCode);
                cleanupCode = new RuntimeHelpers.CleanupCode(CompressedStack.runFinallyCode);
            }
            CompressedStackRunData userData = new CompressedStackRunData(compressedStack, callback, state);
            RuntimeHelpers.ExecuteCodeWithGuaranteedCleanup(tryCode, cleanupCode, userData);
        }

        [PrePrepareMethod]
        internal static void runFinallyCode(object userData, bool exceptionThrown)
        {
            CompressedStackRunData data = (CompressedStackRunData) userData;
            data.cssw.Undo();
        }

        internal static void runTryCode(object userData)
        {
            CompressedStackRunData data = (CompressedStackRunData) userData;
            data.cssw = SetCompressedStack(data.cs, GetCompressedStackThread());
            data.callBack(data.state);
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        internal static IntPtr SetAppDomainStack(CompressedStack cs)
        {
            return Thread.CurrentThread.SetAppDomainStack((cs == null) ? null : cs.CompressedStackHandle);
        }

        internal static CompressedStackSwitcher SetCompressedStack(CompressedStack cs, CompressedStack prevCS)
        {
            CompressedStackSwitcher switcher = new CompressedStackSwitcher();
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                }
                finally
                {
                    SetCompressedStackThread(cs);
                    switcher.prev_CS = prevCS;
                    switcher.curr_CS = cs;
                    switcher.prev_ADStack = SetAppDomainStack(cs);
                }
            }
            catch
            {
                switcher.UndoNoThrow();
                throw;
            }
            return switcher;
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        internal static void SetCompressedStackThread(CompressedStack cs)
        {
            ExecutionContext executionContext = Thread.CurrentThread.ExecutionContext;
            if (executionContext.SecurityContext != null)
            {
                executionContext.SecurityContext.CompressedStack = cs;
            }
            else if (cs != null)
            {
                SecurityContext context2 = new SecurityContext {
                    CompressedStack = cs
                };
                executionContext.SecurityContext = context2;
            }
        }

        internal bool CanSkipEvaluation
        {
            get
            {
                return this.m_canSkipEvaluation;
            }
            private set
            {
                this.m_canSkipEvaluation = value;
            }
        }

        internal SafeCompressedStackHandle CompressedStackHandle
        {
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            get
            {
                return this.m_csHandle;
            }
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            private set
            {
                this.m_csHandle = value;
            }
        }

        internal PermissionListSet PLS
        {
            get
            {
                return this.m_pls;
            }
        }

        internal class CompressedStackRunData
        {
            internal ContextCallback callBack;
            internal CompressedStack cs;
            internal CompressedStackSwitcher cssw;
            internal object state;

            internal CompressedStackRunData(CompressedStack cs, ContextCallback cb, object state)
            {
                this.cs = cs;
                this.callBack = cb;
                this.state = state;
                this.cssw = new CompressedStackSwitcher();
            }
        }
    }
}

