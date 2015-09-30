namespace UnityEditor.Audio
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEditor;

    [StructLayout(LayoutKind.Sequential)]
    internal struct ExposedAudioParameter
    {
        public GUID guid;
        public string name;
    }
}

