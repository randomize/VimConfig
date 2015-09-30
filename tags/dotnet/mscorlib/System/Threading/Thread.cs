namespace System.Threading
{
    using System;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting.Contexts;
    using System.Runtime.Remoting.Messaging;
    using System.Security.Permissions;
    using System.Security.Principal;

    [ComVisible(true), ClassInterface(ClassInterfaceType.None), ComDefaultInterface(typeof(_Thread))]
    public sealed class Thread : CriticalFinalizerObject, _Thread
    {
        private IntPtr DONT_USE_InternalThread;
        private Context m_Context;
        private CultureInfo m_CurrentCulture;
        private CultureInfo m_CurrentUICulture;
        private Delegate m_Delegate;
        private System.Threading.ExecutionContext m_ExecutionContext;
        private int m_ManagedThreadId;
        private string m_Name;
        private int m_Priority;
        private object m_ThreadStartArg;
        private int[] m_ThreadStaticsBits;
        private object[][] m_ThreadStaticsBuckets;
        private static LocalDataStoreMgr s_LocalDataStoreMgr = null;
        private static object s_SyncObject = new object();
        private const int STATICS_BUCKET_SIZE = 0x20;

        public Thread(ParameterizedThreadStart start)
        {
            if (start == null)
            {
                throw new ArgumentNullException("start");
            }
            this.SetStartHelper(start, 0);
        }

        public Thread(ThreadStart start)
        {
            if (start == null)
            {
                throw new ArgumentNullException("start");
            }
            this.SetStartHelper(start, 0);
        }

        public Thread(ParameterizedThreadStart start, int maxStackSize)
        {
            if (start == null)
            {
                throw new ArgumentNullException("start");
            }
            if (0 > maxStackSize)
            {
                throw new ArgumentOutOfRangeException("maxStackSize", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
            }
            this.SetStartHelper(start, maxStackSize);
        }

        public Thread(ThreadStart start, int maxStackSize)
        {
            if (start == null)
            {
                throw new ArgumentNullException("start");
            }
            if (0 > maxStackSize)
            {
                throw new ArgumentOutOfRangeException("maxStackSize", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
            }
            this.SetStartHelper(start, maxStackSize);
        }

        [SecurityPermission(SecurityAction.Demand, ControlThread=true)]
        public void Abort()
        {
            this.AbortInternal();
        }

        [SecurityPermission(SecurityAction.Demand, ControlThread=true)]
        public void Abort(object stateInfo)
        {
            this.AbortReason = stateInfo;
            this.AbortInternal();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void AbortInternal();
        [HostProtection(SecurityAction.LinkDemand, SharedState=true, ExternalThreading=true)]
        public static LocalDataStoreSlot AllocateDataSlot()
        {
            return LocalDataStoreManager.AllocateDataSlot();
        }

        [HostProtection(SecurityAction.LinkDemand, SharedState=true, ExternalThreading=true)]
        public static LocalDataStoreSlot AllocateNamedDataSlot(string name)
        {
            return LocalDataStoreManager.AllocateNamedDataSlot(name);
        }

        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail), HostProtection(SecurityAction.LinkDemand, Synchronization=true, ExternalThreading=true)]
        public static extern void BeginCriticalRegion();
        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail), SecurityPermission(SecurityAction.LinkDemand, ControlThread=true)]
        public static extern void BeginThreadAffinity();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void ClearAbortReason();
        private static object CompleteCrossContextCallback(InternalCrossContextDelegate ftnToCall, object[] args)
        {
            return ftnToCall(args);
        }

        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), HostProtection(SecurityAction.LinkDemand, Synchronization=true, ExternalThreading=true)]
        public static extern void EndCriticalRegion();
        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail), SecurityPermission(SecurityAction.LinkDemand, ControlThread=true)]
        public static extern void EndThreadAffinity();
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        ~Thread()
        {
            this.InternalFinalize();
        }

        private int FindSlot()
        {
            int num = 0;
            int num2 = 0;
            bool flag = false;
            if ((this.m_ThreadStaticsBits.Length != 0) && (this.m_ThreadStaticsBits.Length != ((this.m_ThreadStaticsBuckets.Length * 0x20) / 0x20)))
            {
                return 0;
            }
            int index = 0;
            while (index < this.m_ThreadStaticsBits.Length)
            {
                num2 = this.m_ThreadStaticsBits[index];
                if (num2 != 0)
                {
                    if ((num2 & 0xffff) != 0)
                    {
                        num2 &= 0xffff;
                    }
                    else
                    {
                        num2 = (num2 >> 0x10) & 0xffff;
                        num += 0x10;
                    }
                    if ((num2 & 0xff) != 0)
                    {
                        num2 &= 0xff;
                    }
                    else
                    {
                        num += 8;
                        num2 = (num2 >> 8) & 0xff;
                    }
                    int num4 = 0;
                    while (num4 < 8)
                    {
                        if ((num2 & (((int) 1) << num4)) != 0)
                        {
                            flag = true;
                            break;
                        }
                        num4++;
                    }
                    num += num4;
                    this.m_ThreadStaticsBits[index] &= ~(((int) 1) << num);
                    break;
                }
                index++;
            }
            if (flag)
            {
                num += 0x20 * index;
            }
            return num;
        }

        [HostProtection(SecurityAction.LinkDemand, SharedState=true, ExternalThreading=true)]
        public static void FreeNamedDataSlot(string name)
        {
            LocalDataStoreManager.FreeNamedDataSlot(name);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern object GetAbortReason();
        public System.Threading.ApartmentState GetApartmentState()
        {
            return (System.Threading.ApartmentState) this.GetApartmentStateNative();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int GetApartmentStateNative();
        [Obsolete("Thread.GetCompressedStack is no longer supported. Please use the System.Threading.CompressedStack class"), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode), StrongNameIdentityPermission(SecurityAction.LinkDemand, PublicKey="0x00000000000000000400000000000000")]
        public CompressedStack GetCompressedStack()
        {
            throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ThreadAPIsNotSupported"));
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern Context GetContextInternal(IntPtr id);
        internal Context GetCurrentContextInternal()
        {
            if (this.m_Context == null)
            {
                this.m_Context = Context.DefaultContext;
            }
            return this.m_Context;
        }

        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        private static extern Thread GetCurrentThreadNative();
        [HostProtection(SecurityAction.LinkDemand, SharedState=true, ExternalThreading=true)]
        public static object GetData(LocalDataStoreSlot slot)
        {
            LocalDataStoreManager.ValidateSlot(slot);
            LocalDataStore domainLocalStore = GetDomainLocalStore();
            if (domainLocalStore == null)
            {
                return null;
            }
            return domainLocalStore.GetData(slot);
        }

        public static AppDomain GetDomain()
        {
            if (CurrentThread.m_Context != null)
            {
                return CurrentThread.m_Context.AppDomain;
            }
            AppDomain fastDomainInternal = GetFastDomainInternal();
            if (fastDomainInternal == null)
            {
                fastDomainInternal = GetDomainInternal();
            }
            return fastDomainInternal;
        }

        public static int GetDomainID()
        {
            return GetDomain().GetId();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern AppDomain GetDomainInternal();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern LocalDataStore GetDomainLocalStore();
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        internal System.Threading.ExecutionContext GetExecutionContextNoCreate()
        {
            return this.m_ExecutionContext;
        }

        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        private static extern Thread GetFastCurrentThreadNative();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern AppDomain GetFastDomainInternal();
        [MethodImpl(MethodImplOptions.InternalCall), ComVisible(false)]
        public override extern int GetHashCode();
        internal IllogicalCallContext GetIllogicalCallContext()
        {
            return this.ExecutionContext.IllogicalCallContext;
        }

        [HostProtection(SecurityAction.LinkDemand, SharedState=true, ExternalThreading=true)]
        internal LogicalCallContext GetLogicalCallContext()
        {
            return this.ExecutionContext.LogicalCallContext;
        }

        [HostProtection(SecurityAction.LinkDemand, SharedState=true, ExternalThreading=true)]
        public static LocalDataStoreSlot GetNamedDataSlot(string name)
        {
            return LocalDataStoreManager.GetNamedDataSlot(name);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int GetPriorityNative();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int GetThreadStateNative();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void InformThreadNameChangeEx(Thread t, string name);
        internal object InternalCrossContextCallback(Context ctx, InternalCrossContextDelegate ftnToCall, object[] args)
        {
            return this.InternalCrossContextCallback(ctx, ctx.InternalContextID, 0, ftnToCall, args);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern object InternalCrossContextCallback(Context ctx, IntPtr ctxID, int appDomainID, InternalCrossContextDelegate ftnToCall, object[] args);
        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        private extern void InternalFinalize();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern IntPtr InternalGetCurrentThread();
        [SecurityPermission(SecurityAction.Demand, ControlThread=true)]
        public void Interrupt()
        {
            this.InterruptInternal();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void InterruptInternal();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern bool IsAliveNative();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern bool IsBackgroundNative();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern bool IsThreadpoolThreadNative();
        [HostProtection(SecurityAction.LinkDemand, Synchronization=true, ExternalThreading=true)]
        public void Join()
        {
            this.JoinInternal();
        }

        [HostProtection(SecurityAction.LinkDemand, Synchronization=true, ExternalThreading=true)]
        public bool Join(int millisecondsTimeout)
        {
            return this.JoinInternal(millisecondsTimeout);
        }

        [HostProtection(SecurityAction.LinkDemand, Synchronization=true, ExternalThreading=true)]
        public bool Join(TimeSpan timeout)
        {
            long totalMilliseconds = (long) timeout.TotalMilliseconds;
            if ((totalMilliseconds < -1L) || (totalMilliseconds > 0x7fffffffL))
            {
                throw new ArgumentOutOfRangeException("timeout", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegOrNegative1"));
            }
            return this.Join((int) totalMilliseconds);
        }

        [MethodImpl(MethodImplOptions.InternalCall), HostProtection(SecurityAction.LinkDemand, Synchronization=true, ExternalThreading=true)]
        private extern void JoinInternal();
        [MethodImpl(MethodImplOptions.InternalCall), HostProtection(SecurityAction.LinkDemand, Synchronization=true, ExternalThreading=true)]
        private extern bool JoinInternal(int millisecondsTimeout);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void MemoryBarrier();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool nativeGetSafeCulture(Thread t, int appDomainId, bool isUI, ref CultureInfo safeCulture);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool nativeSetThreadUILocale(int LCID);
        private static void RemoveDomainLocalStore(LocalDataStore dls)
        {
            if (dls != null)
            {
                LocalDataStoreManager.DeleteLocalDataStore(dls);
            }
        }

        private int ReserveSlot()
        {
            if (this.m_ThreadStaticsBuckets == null)
            {
                object[][] objArray = new object[1][];
                SetIsThreadStaticsArray(objArray);
                objArray[0] = new object[0x20];
                SetIsThreadStaticsArray(objArray[0]);
                int[] numArray = new int[(objArray.Length * 0x20) / 0x20];
                for (int k = 0; k < numArray.Length; k++)
                {
                    numArray[k] = -1;
                }
                numArray[0] &= -2;
                numArray[0] &= -3;
                this.m_ThreadStaticsBits = numArray;
                this.m_ThreadStaticsBuckets = objArray;
                return 1;
            }
            int num2 = this.FindSlot();
            if (num2 != 0)
            {
                return num2;
            }
            int length = this.m_ThreadStaticsBuckets.Length;
            int index = this.m_ThreadStaticsBits.Length;
            int num5 = this.m_ThreadStaticsBuckets.Length + 1;
            object[][] o = new object[num5][];
            SetIsThreadStaticsArray(o);
            int num6 = (num5 * 0x20) / 0x20;
            int[] destinationArray = new int[num6];
            Array.Copy(this.m_ThreadStaticsBuckets, o, this.m_ThreadStaticsBuckets.Length);
            for (int i = length; i < num5; i++)
            {
                o[i] = new object[0x20];
                SetIsThreadStaticsArray(o[i]);
            }
            Array.Copy(this.m_ThreadStaticsBits, destinationArray, this.m_ThreadStaticsBits.Length);
            for (int j = index; j < num6; j++)
            {
                destinationArray[j] = -1;
            }
            destinationArray[index] &= -2;
            this.m_ThreadStaticsBits = destinationArray;
            this.m_ThreadStaticsBuckets = o;
            return (length * 0x20);
        }

        [SecurityPermission(SecurityAction.Demand, ControlThread=true)]
        public static void ResetAbort()
        {
            Thread currentThread = CurrentThread;
            if ((currentThread.ThreadState & System.Threading.ThreadState.AbortRequested) == System.Threading.ThreadState.Running)
            {
                throw new ThreadStateException(Environment.GetResourceString("ThreadState_NoAbortRequested"));
            }
            currentThread.ResetAbortNative();
            currentThread.ClearAbortReason();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void ResetAbortNative();
        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        internal extern void RestoreAppDomainStack(IntPtr appDomainStack);
        [Obsolete("Thread.Resume has been deprecated.  Please use other classes in System.Threading, such as Monitor, Mutex, Event, and Semaphore, to synchronize Threads or protect resources.  http://go.microsoft.com/fwlink/?linkid=14202", false), SecurityPermission(SecurityAction.Demand, ControlThread=true)]
        public void Resume()
        {
            this.ResumeInternal();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void ResumeInternal();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void SetAbortReason(object o);
        [HostProtection(SecurityAction.LinkDemand, Synchronization=true, SelfAffectingThreading=true)]
        public void SetApartmentState(System.Threading.ApartmentState state)
        {
            if (!this.SetApartmentStateHelper(state, true))
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ApartmentStateSwitchFailed"));
            }
        }

        private bool SetApartmentStateHelper(System.Threading.ApartmentState state, bool fireMDAOnMismatch)
        {
            System.Threading.ApartmentState state2 = (System.Threading.ApartmentState) this.SetApartmentStateNative((int) state, fireMDAOnMismatch);
            if (((state != System.Threading.ApartmentState.Unknown) || (state2 != System.Threading.ApartmentState.MTA)) && (state2 != state))
            {
                return false;
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int SetApartmentStateNative(int state, bool fireMDAOnMismatch);
        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        internal extern IntPtr SetAppDomainStack(SafeCompressedStackHandle csHandle);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetBackgroundNative(bool isBackground);
        [Obsolete("Thread.SetCompressedStack is no longer supported. Please use the System.Threading.CompressedStack class"), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode), StrongNameIdentityPermission(SecurityAction.LinkDemand, PublicKey="0x00000000000000000400000000000000")]
        public void SetCompressedStack(CompressedStack stack)
        {
            throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ThreadAPIsNotSupported"));
        }

        [HostProtection(SecurityAction.LinkDemand, SharedState=true, ExternalThreading=true)]
        public static void SetData(LocalDataStoreSlot slot, object data)
        {
            LocalDataStore domainLocalStore = GetDomainLocalStore();
            if (domainLocalStore == null)
            {
                domainLocalStore = LocalDataStoreManager.CreateLocalDataStore();
                SetDomainLocalStore(domainLocalStore);
            }
            domainLocalStore.SetData(slot, data);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetDomainLocalStore(LocalDataStore dls);
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        internal void SetExecutionContext(System.Threading.ExecutionContext value)
        {
            this.m_ExecutionContext = value;
            if (value != null)
            {
                this.m_ExecutionContext.Thread = this;
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetIsThreadStaticsArray(object o);
        [HostProtection(SecurityAction.LinkDemand, SharedState=true, ExternalThreading=true)]
        internal LogicalCallContext SetLogicalCallContext(LogicalCallContext callCtx)
        {
            LogicalCallContext logicalCallContext = this.ExecutionContext.LogicalCallContext;
            this.ExecutionContext.LogicalCallContext = callCtx;
            return logicalCallContext;
        }

        private void SetPrincipalInternal(IPrincipal principal)
        {
            this.GetLogicalCallContext().SecurityData.Principal = principal;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetPriorityNative(int priority);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetStart(Delegate start, int maxStackSize);
        private void SetStartHelper(Delegate start, int maxStackSize)
        {
            ThreadHelper helper = new ThreadHelper(start);
            if (start is ThreadStart)
            {
                this.SetStart(new ThreadStart(helper.ThreadStart), maxStackSize);
            }
            else
            {
                this.SetStart(new ParameterizedThreadStart(helper.ThreadStart), maxStackSize);
            }
        }

        public static void Sleep(int millisecondsTimeout)
        {
            SleepInternal(millisecondsTimeout);
        }

        public static void Sleep(TimeSpan timeout)
        {
            long totalMilliseconds = (long) timeout.TotalMilliseconds;
            if ((totalMilliseconds < -1L) || (totalMilliseconds > 0x7fffffffL))
            {
                throw new ArgumentOutOfRangeException("timeout", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegOrNegative1"));
            }
            Sleep((int) totalMilliseconds);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SleepInternal(int millisecondsTimeout);
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), HostProtection(SecurityAction.LinkDemand, Synchronization=true, ExternalThreading=true)]
        public static void SpinWait(int iterations)
        {
            SpinWaitInternal(iterations);
        }

        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), HostProtection(SecurityAction.LinkDemand, Synchronization=true, ExternalThreading=true)]
        private static extern void SpinWaitInternal(int iterations);
        [MethodImpl(MethodImplOptions.NoInlining), HostProtection(SecurityAction.LinkDemand, Synchronization=true, ExternalThreading=true)]
        public void Start()
        {
            this.StartupSetApartmentStateInternal();
            if (this.m_Delegate != null)
            {
                ThreadHelper target = (ThreadHelper) this.m_Delegate.Target;
                System.Threading.ExecutionContext ec = System.Threading.ExecutionContext.Capture();
                System.Threading.ExecutionContext.ClearSyncContext(ec);
                target.SetExecutionContextHelper(ec);
            }
            IPrincipal principal = CallContext.Principal;
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            this.StartInternal(principal, ref lookForMyCaller);
        }

        [HostProtection(SecurityAction.LinkDemand, Synchronization=true, ExternalThreading=true)]
        public void Start(object parameter)
        {
            if (this.m_Delegate is ThreadStart)
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ThreadWrongThreadStart"));
            }
            this.m_ThreadStartArg = parameter;
            this.Start();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void StartInternal(IPrincipal principal, ref StackCrawlMark stackMark);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int StartupSetApartmentStateInternal();
        [Obsolete("Thread.Suspend has been deprecated.  Please use other classes in System.Threading, such as Monitor, Mutex, Event, and Semaphore, to synchronize Threads or protect resources.  http://go.microsoft.com/fwlink/?linkid=14202", false), SecurityPermission(SecurityAction.Demand, ControlThread=true), SecurityPermission(SecurityAction.Demand, ControlThread=true)]
        public void Suspend()
        {
            this.SuspendInternal();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SuspendInternal();
        void _Thread.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
        {
            throw new NotImplementedException();
        }

        void _Thread.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
        {
            throw new NotImplementedException();
        }

        void _Thread.GetTypeInfoCount(out uint pcTInfo)
        {
            throw new NotImplementedException();
        }

        void _Thread.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
        {
            throw new NotImplementedException();
        }

        [HostProtection(SecurityAction.LinkDemand, Synchronization=true, SelfAffectingThreading=true)]
        public bool TrySetApartmentState(System.Threading.ApartmentState state)
        {
            return this.SetApartmentStateHelper(state, false);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static byte VolatileRead(ref byte address)
        {
            byte num = address;
            MemoryBarrier();
            return num;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static double VolatileRead(ref double address)
        {
            double num = address;
            MemoryBarrier();
            return num;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static short VolatileRead(ref short address)
        {
            short num = address;
            MemoryBarrier();
            return num;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static int VolatileRead(ref int address)
        {
            int num = address;
            MemoryBarrier();
            return num;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static long VolatileRead(ref long address)
        {
            long num = address;
            MemoryBarrier();
            return num;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static IntPtr VolatileRead(ref IntPtr address)
        {
            IntPtr ptr = address;
            MemoryBarrier();
            return ptr;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static object VolatileRead(ref object address)
        {
            object obj2 = address;
            MemoryBarrier();
            return obj2;
        }

        [MethodImpl(MethodImplOptions.NoInlining), CLSCompliant(false)]
        public static sbyte VolatileRead(ref sbyte address)
        {
            sbyte num = address;
            MemoryBarrier();
            return num;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static float VolatileRead(ref float address)
        {
            float num = address;
            MemoryBarrier();
            return num;
        }

        [MethodImpl(MethodImplOptions.NoInlining), CLSCompliant(false)]
        public static ushort VolatileRead(ref ushort address)
        {
            ushort num = address;
            MemoryBarrier();
            return num;
        }

        [MethodImpl(MethodImplOptions.NoInlining), CLSCompliant(false)]
        public static uint VolatileRead(ref uint address)
        {
            uint num = address;
            MemoryBarrier();
            return num;
        }

        [MethodImpl(MethodImplOptions.NoInlining), CLSCompliant(false)]
        public static ulong VolatileRead(ref ulong address)
        {
            ulong num = address;
            MemoryBarrier();
            return num;
        }

        [MethodImpl(MethodImplOptions.NoInlining), CLSCompliant(false)]
        public static UIntPtr VolatileRead(ref UIntPtr address)
        {
            UIntPtr ptr = address;
            MemoryBarrier();
            return ptr;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void VolatileWrite(ref byte address, byte value)
        {
            MemoryBarrier();
            address = value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void VolatileWrite(ref double address, double value)
        {
            MemoryBarrier();
            address = value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void VolatileWrite(ref short address, short value)
        {
            MemoryBarrier();
            address = value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void VolatileWrite(ref int address, int value)
        {
            MemoryBarrier();
            address = value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void VolatileWrite(ref long address, long value)
        {
            MemoryBarrier();
            address = value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void VolatileWrite(ref IntPtr address, IntPtr value)
        {
            MemoryBarrier();
            address = value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void VolatileWrite(ref object address, object value)
        {
            MemoryBarrier();
            address = value;
        }

        [MethodImpl(MethodImplOptions.NoInlining), CLSCompliant(false)]
        public static void VolatileWrite(ref sbyte address, sbyte value)
        {
            MemoryBarrier();
            address = value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void VolatileWrite(ref float address, float value)
        {
            MemoryBarrier();
            address = value;
        }

        [MethodImpl(MethodImplOptions.NoInlining), CLSCompliant(false)]
        public static void VolatileWrite(ref ushort address, ushort value)
        {
            MemoryBarrier();
            address = value;
        }

        [MethodImpl(MethodImplOptions.NoInlining), CLSCompliant(false)]
        public static void VolatileWrite(ref uint address, uint value)
        {
            MemoryBarrier();
            address = value;
        }

        [MethodImpl(MethodImplOptions.NoInlining), CLSCompliant(false)]
        public static void VolatileWrite(ref ulong address, ulong value)
        {
            MemoryBarrier();
            address = value;
        }

        [MethodImpl(MethodImplOptions.NoInlining), CLSCompliant(false)]
        public static void VolatileWrite(ref UIntPtr address, UIntPtr value)
        {
            MemoryBarrier();
            address = value;
        }

        internal object AbortReason
        {
            get
            {
                object abortReason = null;
                try
                {
                    abortReason = this.GetAbortReason();
                }
                catch (Exception exception)
                {
                    throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ExceptionStateCrossAppDomain"), exception);
                }
                return abortReason;
            }
            set
            {
                this.SetAbortReason(value);
            }
        }

        [Obsolete("The ApartmentState property has been deprecated.  Use GetApartmentState, SetApartmentState or TrySetApartmentState instead.", false)]
        public System.Threading.ApartmentState ApartmentState
        {
            get
            {
                return (System.Threading.ApartmentState) this.GetApartmentStateNative();
            }
            [HostProtection(SecurityAction.LinkDemand, Synchronization=true, SelfAffectingThreading=true)]
            set
            {
                this.SetApartmentStateNative((int) value, true);
            }
        }

        public static Context CurrentContext
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
            get
            {
                return CurrentThread.GetCurrentContextInternal();
            }
        }

        public CultureInfo CurrentCulture
        {
            get
            {
                if (this.m_CurrentCulture != null)
                {
                    CultureInfo safeCulture = null;
                    if (nativeGetSafeCulture(this, GetDomainID(), false, ref safeCulture) && (safeCulture != null))
                    {
                        return safeCulture;
                    }
                }
                return CultureInfo.UserDefaultCulture;
            }
            [SecurityPermission(SecurityAction.Demand, ControlThread=true)]
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                CultureInfo.CheckNeutral(value);
                CultureInfo.nativeSetThreadLocale(value.LCID);
                value.StartCrossDomainTracking();
                this.m_CurrentCulture = value;
            }
        }

        public static IPrincipal CurrentPrincipal
        {
            get
            {
                lock (CurrentThread)
                {
                    IPrincipal threadPrincipal = CallContext.Principal;
                    if (threadPrincipal == null)
                    {
                        threadPrincipal = GetDomain().GetThreadPrincipal();
                        CallContext.Principal = threadPrincipal;
                    }
                    return threadPrincipal;
                }
            }
            [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.ControlPrincipal)]
            set
            {
                CallContext.Principal = value;
            }
        }

        public static Thread CurrentThread
        {
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
            get
            {
                Thread fastCurrentThreadNative = GetFastCurrentThreadNative();
                if (fastCurrentThreadNative == null)
                {
                    fastCurrentThreadNative = GetCurrentThreadNative();
                }
                return fastCurrentThreadNative;
            }
        }

        public CultureInfo CurrentUICulture
        {
            get
            {
                if (this.m_CurrentUICulture != null)
                {
                    CultureInfo safeCulture = null;
                    if (nativeGetSafeCulture(this, GetDomainID(), true, ref safeCulture) && (safeCulture != null))
                    {
                        return safeCulture;
                    }
                }
                return CultureInfo.UserDefaultUICulture;
            }
            [HostProtection(SecurityAction.LinkDemand, ExternalThreading=true)]
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                CultureInfo.VerifyCultureName(value, true);
                if (!nativeSetThreadUILocale(value.LCID))
                {
                    throw new ArgumentException(Environment.GetResourceString("Argument_InvalidResourceCultureName", new object[] { value.Name }));
                }
                value.StartCrossDomainTracking();
                this.m_CurrentUICulture = value;
            }
        }

        public System.Threading.ExecutionContext ExecutionContext
        {
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
            get
            {
                if ((this.m_ExecutionContext == null) && (this == CurrentThread))
                {
                    this.m_ExecutionContext = new System.Threading.ExecutionContext();
                    this.m_ExecutionContext.Thread = this;
                }
                return this.m_ExecutionContext;
            }
        }

        public bool IsAlive
        {
            get
            {
                return this.IsAliveNative();
            }
        }

        public bool IsBackground
        {
            get
            {
                return this.IsBackgroundNative();
            }
            [HostProtection(SecurityAction.LinkDemand, SelfAffectingThreading=true)]
            set
            {
                this.SetBackgroundNative(value);
            }
        }

        public bool IsThreadPoolThread
        {
            get
            {
                return this.IsThreadpoolThreadNative();
            }
        }

        private static LocalDataStoreMgr LocalDataStoreManager
        {
            get
            {
                if (s_LocalDataStoreMgr == null)
                {
                    lock (s_SyncObject)
                    {
                        if (s_LocalDataStoreMgr == null)
                        {
                            s_LocalDataStoreMgr = new LocalDataStoreMgr();
                        }
                    }
                }
                return s_LocalDataStoreMgr;
            }
        }

        public int ManagedThreadId
        {
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            get
            {
                return this.m_ManagedThreadId;
            }
        }

        public string Name
        {
            get
            {
                return this.m_Name;
            }
            [HostProtection(SecurityAction.LinkDemand, ExternalThreading=true)]
            set
            {
                lock (this)
                {
                    if (this.m_Name != null)
                    {
                        throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_WriteOnce"));
                    }
                    this.m_Name = value;
                    InformThreadNameChangeEx(this, this.m_Name);
                }
            }
        }

        public ThreadPriority Priority
        {
            get
            {
                return (ThreadPriority) this.GetPriorityNative();
            }
            [HostProtection(SecurityAction.LinkDemand, SelfAffectingThreading=true)]
            set
            {
                this.SetPriorityNative((int) value);
            }
        }

        public System.Threading.ThreadState ThreadState
        {
            get
            {
                return (System.Threading.ThreadState) this.GetThreadStateNative();
            }
        }
    }
}

