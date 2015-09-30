namespace System.Threading
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    [ComVisible(true)]
    public class Overlapped
    {
        private OverlappedData m_overlappedData;

        public Overlapped()
        {
            this.m_overlappedData = OverlappedDataCache.GetOverlappedData(this);
        }

        [Obsolete("This constructor is not 64-bit compatible.  Use the constructor that takes an IntPtr for the event handle.  http://go.microsoft.com/fwlink/?linkid=14202")]
        public Overlapped(int offsetLo, int offsetHi, int hEvent, IAsyncResult ar) : this(offsetLo, offsetHi, new IntPtr(hEvent), ar)
        {
        }

        public Overlapped(int offsetLo, int offsetHi, IntPtr hEvent, IAsyncResult ar)
        {
            this.m_overlappedData = OverlappedDataCache.GetOverlappedData(this);
            this.m_overlappedData.m_nativeOverlapped.OffsetLow = offsetLo;
            this.m_overlappedData.m_nativeOverlapped.OffsetHigh = offsetHi;
            this.m_overlappedData.UserHandle = hEvent;
            this.m_overlappedData.m_asyncResult = ar;
        }

        [CLSCompliant(false)]
        public static unsafe void Free(NativeOverlapped* nativeOverlappedPtr)
        {
            if (nativeOverlappedPtr == null)
            {
                throw new ArgumentNullException("nativeOverlappedPtr");
            }
            Overlapped overlapped = OverlappedData.GetOverlappedFromNative(nativeOverlappedPtr).m_overlapped;
            OverlappedData.FreeNativeOverlapped(nativeOverlappedPtr);
            OverlappedData overlappedData = overlapped.m_overlappedData;
            overlapped.m_overlappedData = null;
            OverlappedDataCache.CacheOverlappedData(overlappedData);
        }

        [Obsolete("This method is not safe.  Use Pack (iocb, userData) instead.  http://go.microsoft.com/fwlink/?linkid=14202"), CLSCompliant(false)]
        public unsafe NativeOverlapped* Pack(IOCompletionCallback iocb)
        {
            return this.Pack(iocb, null);
        }

        [CLSCompliant(false), ComVisible(false)]
        public unsafe NativeOverlapped* Pack(IOCompletionCallback iocb, object userData)
        {
            return this.m_overlappedData.Pack(iocb, userData);
        }

        [CLSCompliant(false)]
        public static unsafe Overlapped Unpack(NativeOverlapped* nativeOverlappedPtr)
        {
            if (nativeOverlappedPtr == null)
            {
                throw new ArgumentNullException("nativeOverlappedPtr");
            }
            return OverlappedData.GetOverlappedFromNative(nativeOverlappedPtr).m_overlapped;
        }

        [Obsolete("This method is not safe.  Use UnsafePack (iocb, userData) instead.  http://go.microsoft.com/fwlink/?linkid=14202"), CLSCompliant(false), SecurityPermission(SecurityAction.LinkDemand, ControlEvidence=true, ControlPolicy=true)]
        public unsafe NativeOverlapped* UnsafePack(IOCompletionCallback iocb)
        {
            return this.UnsafePack(iocb, null);
        }

        [CLSCompliant(false), ComVisible(false), SecurityPermission(SecurityAction.LinkDemand, ControlEvidence=true, ControlPolicy=true)]
        public unsafe NativeOverlapped* UnsafePack(IOCompletionCallback iocb, object userData)
        {
            return this.m_overlappedData.UnsafePack(iocb, userData);
        }

        public IAsyncResult AsyncResult
        {
            get
            {
                return this.m_overlappedData.m_asyncResult;
            }
            set
            {
                this.m_overlappedData.m_asyncResult = value;
            }
        }

        [Obsolete("This property is not 64-bit compatible.  Use EventHandleIntPtr instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
        public int EventHandle
        {
            get
            {
                return this.m_overlappedData.UserHandle.ToInt32();
            }
            set
            {
                this.m_overlappedData.UserHandle = new IntPtr(value);
            }
        }

        [ComVisible(false)]
        public IntPtr EventHandleIntPtr
        {
            get
            {
                return this.m_overlappedData.UserHandle;
            }
            set
            {
                this.m_overlappedData.UserHandle = value;
            }
        }

        internal _IOCompletionCallback iocbHelper
        {
            get
            {
                return this.m_overlappedData.m_iocbHelper;
            }
        }

        public int OffsetHigh
        {
            get
            {
                return this.m_overlappedData.m_nativeOverlapped.OffsetHigh;
            }
            set
            {
                this.m_overlappedData.m_nativeOverlapped.OffsetHigh = value;
            }
        }

        public int OffsetLow
        {
            get
            {
                return this.m_overlappedData.m_nativeOverlapped.OffsetLow;
            }
            set
            {
                this.m_overlappedData.m_nativeOverlapped.OffsetLow = value;
            }
        }

        internal IOCompletionCallback UserCallback
        {
            get
            {
                return this.m_overlappedData.m_iocb;
            }
        }
    }
}

