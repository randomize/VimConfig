namespace UnityEditor.Audio
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEditor;

    [StructLayout(LayoutKind.Sequential)]
    internal struct MixerEffectParameter
    {
        public string parameterName;
        public UnityEditor.GUID GUID;
    }
}

