namespace UnityEngine.Networking
{
    using System;
    using UnityEngine;

    public class PlayerController
    {
        public GameObject gameObject;
        internal const short kMaxLocalPlayers = 8;
        public const int MaxPlayersPerClient = 0x20;
        public short playerControllerId;
        public NetworkIdentity unetView;

        public PlayerController()
        {
            this.playerControllerId = -1;
        }

        internal PlayerController(GameObject go, short playerControllerId)
        {
            this.playerControllerId = -1;
            this.gameObject = go;
            this.unetView = go.GetComponent<NetworkIdentity>();
            this.playerControllerId = playerControllerId;
        }

        public override string ToString()
        {
            return string.Format("ID={0} NetworkIdentity NetID={1} Player={2}", this.playerControllerId, (this.unetView == null) ? "null" : this.unetView.netId.ToString(), (this.gameObject == null) ? "null" : this.gameObject.name);
        }

        public bool IsValid
        {
            get
            {
                return (this.playerControllerId != -1);
            }
        }
    }
}

