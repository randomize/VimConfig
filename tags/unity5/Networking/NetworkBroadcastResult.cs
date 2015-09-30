namespace UnityEngine.Networking
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct NetworkBroadcastResult
    {
        public string serverAddress;
        public byte[] broadcastData;
    }
}

