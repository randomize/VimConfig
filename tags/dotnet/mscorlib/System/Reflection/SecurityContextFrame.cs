namespace System.Reflection
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct SecurityContextFrame
    {
        private IntPtr m_GSCookie;
        private IntPtr __VFN_table;
        private IntPtr m_Next;
        private IntPtr m_Assembly;
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void Push(Assembly assembly);
        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public extern void Pop();
    }
}

