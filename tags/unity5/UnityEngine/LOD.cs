namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct LOD
    {
        public float screenRelativeTransitionHeight;
        public float fadeTransitionWidth;
        public Renderer[] renderers;
        public LOD(float screenRelativeTransitionHeight, Renderer[] renderers)
        {
            this.screenRelativeTransitionHeight = screenRelativeTransitionHeight;
            this.fadeTransitionWidth = 0f;
            this.renderers = renderers;
        }
    }
}

