namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [RequireComponent(typeof(NetworkIdentity)), AddComponentMenu("Network/NetworkProximityChecker")]
    public class NetworkProximityChecker : NetworkBehaviour
    {
        public CheckMethod checkMethod;
        public bool forceHidden;
        private float m_VisUpdateTime;
        public int visRange = 10;
        public float visUpdateInterval = 1f;

        public override bool OnCheckObserver(NetworkConnection newObserver)
        {
            if (this.forceHidden)
            {
                return false;
            }
            GameObject gameObject = null;
            foreach (PlayerController controller in newObserver.playerControllers)
            {
                if ((controller != null) && (controller.gameObject != null))
                {
                    gameObject = controller.gameObject;
                    break;
                }
            }
            if (gameObject == null)
            {
                return false;
            }
            Vector3 vector2 = gameObject.transform.position - base.transform.position;
            return (vector2.magnitude < this.visRange);
        }

        public override bool OnRebuildObservers(HashSet<NetworkConnection> observers, bool initial)
        {
            if (this.forceHidden)
            {
                NetworkIdentity component = base.GetComponent<NetworkIdentity>();
                if (component.connectionToClient != null)
                {
                    observers.Add(component.connectionToClient);
                }
                return true;
            }
            CheckMethod checkMethod = this.checkMethod;
            if (checkMethod != CheckMethod.Physics3D)
            {
                if (checkMethod != CheckMethod.Physics2D)
                {
                    return false;
                }
            }
            else
            {
                foreach (Collider collider in Physics.OverlapSphere(base.transform.position, (float) this.visRange))
                {
                    NetworkIdentity identity2 = collider.GetComponent<NetworkIdentity>();
                    if ((identity2 != null) && (identity2.connectionToClient != null))
                    {
                        observers.Add(identity2.connectionToClient);
                    }
                }
                return true;
            }
            foreach (Collider2D colliderd in Physics2D.OverlapCircleAll(base.transform.position, (float) this.visRange))
            {
                NetworkIdentity identity3 = colliderd.GetComponent<NetworkIdentity>();
                if ((identity3 != null) && (identity3.connectionToClient != null))
                {
                    observers.Add(identity3.connectionToClient);
                }
            }
            return true;
        }

        public override void OnSetLocalVisibility(bool vis)
        {
            SetVis(base.gameObject, vis);
        }

        private static void SetVis(GameObject go, bool vis)
        {
            foreach (Renderer renderer in go.GetComponents<Renderer>())
            {
                renderer.enabled = vis;
            }
            for (int i = 0; i < go.transform.childCount; i++)
            {
                SetVis(go.transform.GetChild(i).gameObject, vis);
            }
        }

        private void Update()
        {
            if (NetworkServer.active && ((Time.time - this.m_VisUpdateTime) > this.visUpdateInterval))
            {
                base.GetComponent<NetworkIdentity>().RebuildObservers(false);
                this.m_VisUpdateTime = Time.time;
            }
        }

        public enum CheckMethod
        {
            Physics3D,
            Physics2D
        }
    }
}

