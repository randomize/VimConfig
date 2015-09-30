namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public sealed class WaitForSeconds : YieldInstruction
    {
        internal float m_Seconds;
        public WaitForSeconds(float seconds)
        {
            this.m_Seconds = seconds;
        }
    }
}

