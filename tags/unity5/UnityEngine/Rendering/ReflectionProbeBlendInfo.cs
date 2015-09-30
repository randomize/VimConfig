namespace UnityEngine.Rendering
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public struct ReflectionProbeBlendInfo
    {
        public ReflectionProbe probe;
        public float weight;
    }
}

