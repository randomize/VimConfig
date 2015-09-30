namespace System.Threading
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public sealed class RegisteredWaitHandle : MarshalByRefObject
    {
        private RegisteredWaitHandleSafe internalRegisteredWait = new RegisteredWaitHandleSafe();

        internal RegisteredWaitHandle()
        {
        }

        internal void SetHandle(IntPtr handle)
        {
            this.internalRegisteredWait.SetHandle(handle);
        }

        internal void SetWaitObject(WaitHandle waitObject)
        {
            this.internalRegisteredWait.SetWaitObject(waitObject);
        }

        [ComVisible(true)]
        public bool Unregister(WaitHandle waitObject)
        {
            return this.internalRegisteredWait.Unregister(waitObject);
        }
    }
}

