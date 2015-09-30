namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct GradientColorKey
    {
        public Color color;
        public float time;
        public GradientColorKey(Color col, float time)
        {
            this.color = col;
            this.time = time;
        }
    }
}

