namespace UnityEngine.Networking
{
    using System;

    public class MsgType
    {
        public const short AddPlayer = 0x25;
        public const short Animation = 40;
        public const short AnimationParameters = 0x29;
        public const short AnimationTrigger = 0x2a;
        public const short Command = 5;
        public const short Connect = 0x20;
        public const short CRC = 14;
        public const short Disconnect = 0x21;
        public const short Error = 0x22;
        public const short Highest = 0x2e;
        internal const short HLAPIMsg = 0x1c;
        internal const short HLAPIPending = 0x1f;
        internal const short HLAPIResend = 30;
        public const short InternalHighest = 0x1f;
        internal const short LLAPIMsg = 0x1d;
        public const short LobbyAddPlayerFailed = 0x2d;
        public const short LobbyReadyToBegin = 0x2b;
        public const short LobbyReturnToLobby = 0x2e;
        public const short LobbySceneLoaded = 0x2c;
        public const short LocalChildTransform = 0x10;
        public const short LocalClientAuthority = 15;
        public const short LocalPlayerTransform = 6;
        internal static string[] msgLabels = new string[] { 
            "none", "ObjectDestroy", "Rpc", "ObjectSpawn", "Owner", "Command", "LocalPlayerTransform", "SyncEvent", "UpdateVars", "SyncList", "ObjectSpawnScene", "NetworkInfo", "SpawnFinished", "ObjectHide", "CRC", "LocalClientAuthority", 
            "LocalChildTransform", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 
            "Connect", "Disconnect", "Error", "Ready", "NotReady", "AddPlayer", "RemovePlayer", "Scene", "Animation", "AnimationParams", "AnimationTrigger", "LobbyReadyToBegin", "LobbySceneLoaded", "LobbyAddPlayerFailed", "LobbyReturnToLobby"
         };
        public const short NetworkInfo = 11;
        public const short NotReady = 0x24;
        public const short ObjectDestroy = 1;
        public const short ObjectHide = 13;
        public const short ObjectSpawn = 3;
        public const short ObjectSpawnScene = 10;
        public const short Owner = 4;
        public const short Ready = 0x23;
        public const short RemovePlayer = 0x26;
        public const short Rpc = 2;
        public const short Scene = 0x27;
        public const short SpawnFinished = 12;
        public const short SyncEvent = 7;
        public const short SyncList = 9;
        public const short UpdateVars = 8;
        internal const short UserMessage = 0;

        public static string MsgTypeToString(short value)
        {
            if ((value < 0) || (value > 0x2e))
            {
                return string.Empty;
            }
            string str = msgLabels[value];
            if (string.IsNullOrEmpty(str))
            {
                str = "[" + value + "]";
            }
            return str;
        }
    }
}

