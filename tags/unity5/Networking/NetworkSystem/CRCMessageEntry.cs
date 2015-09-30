namespace UnityEngine.Networking.NetworkSystem
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct CRCMessageEntry
    {
        public string name;
        public byte channel;
    }
}

