namespace UnityEditor.Audio
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEditor;

    [StructLayout(LayoutKind.Sequential)]
    internal struct MixerGroupView
    {
        public GUID[] guids;
        public string name;
    }
}

