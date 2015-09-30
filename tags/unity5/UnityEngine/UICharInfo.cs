namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct UICharInfo
    {
        public Vector2 cursorPos;
        public float charWidth;
    }
}

