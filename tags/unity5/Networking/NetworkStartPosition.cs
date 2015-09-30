namespace UnityEngine.Networking
{
    using System;
    using UnityEngine;

    [AddComponentMenu("Network/NetworkStartPosition"), DisallowMultipleComponent]
    public class NetworkStartPosition : MonoBehaviour
    {
        public void Awake()
        {
            NetworkManager.RegisterStartPosition(base.transform);
        }

        public void OnDestroy()
        {
            NetworkManager.UnRegisterStartPosition(base.transform);
        }
    }
}

