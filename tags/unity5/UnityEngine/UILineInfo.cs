namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct UILineInfo
    {
        public int startCharIdx;
        public int height;
    }
}

