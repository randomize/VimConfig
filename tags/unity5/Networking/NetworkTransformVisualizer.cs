namespace UnityEngine.Networking
{
    using System;
    using System.ComponentModel;
    using UnityEngine;

    [AddComponentMenu("Network/NetworkTransformVisualizer"), EditorBrowsable(EditorBrowsableState.Never), RequireComponent(typeof(NetworkTransform)), DisallowMultipleComponent]
    public class NetworkTransformVisualizer : NetworkBehaviour
    {
        private NetworkTransform m_NetworkTransform;
        private GameObject m_Visualizer;
        [SerializeField]
        private GameObject m_VisualizerPrefab;
        private static Material s_LineMaterial;

        private static void CreateLineMaterial()
        {
            if (s_LineMaterial == null)
            {
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                if (shader == null)
                {
                    Debug.LogWarning("Could not find Colored builtin shader");
                }
                else
                {
                    s_LineMaterial = new Material(shader);
                    s_LineMaterial.hideFlags = HideFlags.HideAndDontSave;
                    s_LineMaterial.SetInt("_ZWrite", 0);
                }
            }
        }

        private void DrawRotationInterpolation()
        {
            Quaternion identity = Quaternion.identity;
            if (this.m_NetworkTransform.rigidbody3D != null)
            {
                identity = this.m_NetworkTransform.targetSyncRotation3D;
            }
            if (this.m_NetworkTransform.rigidbody2D != null)
            {
                identity = Quaternion.Euler(0f, 0f, this.m_NetworkTransform.targetSyncRotation2D);
            }
            if (identity != Quaternion.identity)
            {
                GL.Begin(1);
                GL.Color(Color.yellow);
                GL.Vertex3(base.transform.position.x, base.transform.position.y, base.transform.position.z);
                Vector3 vector = base.transform.position + base.transform.right;
                GL.Vertex3(vector.x, vector.y, vector.z);
                GL.End();
                GL.Begin(1);
                GL.Color(Color.green);
                GL.Vertex3(base.transform.position.x, base.transform.position.y, base.transform.position.z);
                Vector3 vector2 = (Vector3) (identity * Vector3.right);
                Vector3 vector3 = base.transform.position + vector2;
                GL.Vertex3(vector3.x, vector3.y, vector3.z);
                GL.End();
            }
        }

        [ClientCallback]
        private void FixedUpdate()
        {
            if ((((this.m_Visualizer != null) && (NetworkServer.active || NetworkClient.active)) && (base.isServer || base.isClient)) && (!base.hasAuthority || !this.m_NetworkTransform.localPlayerAuthority))
            {
                this.m_Visualizer.transform.position = this.m_NetworkTransform.targetSyncPosition;
                if ((this.m_NetworkTransform.rigidbody3D != null) && (this.m_Visualizer.GetComponent<Rigidbody>() != null))
                {
                    this.m_Visualizer.GetComponent<Rigidbody>().velocity = this.m_NetworkTransform.targetSyncVelocity;
                }
                if ((this.m_NetworkTransform.rigidbody2D != null) && (this.m_Visualizer.GetComponent<Rigidbody2D>() != null))
                {
                    this.m_Visualizer.GetComponent<Rigidbody2D>().velocity = this.m_NetworkTransform.targetSyncVelocity;
                }
                Quaternion identity = Quaternion.identity;
                if (this.m_NetworkTransform.rigidbody3D != null)
                {
                    identity = this.m_NetworkTransform.targetSyncRotation3D;
                }
                if (this.m_NetworkTransform.rigidbody2D != null)
                {
                    identity = Quaternion.Euler(0f, 0f, this.m_NetworkTransform.targetSyncRotation2D);
                }
                this.m_Visualizer.transform.rotation = identity;
            }
        }

        private void OnDestroy()
        {
            if (this.m_Visualizer != null)
            {
                UnityEngine.Object.Destroy(this.m_Visualizer);
            }
        }

        private void OnRenderObject()
        {
            if (((this.m_Visualizer != null) && (!this.m_NetworkTransform.localPlayerAuthority || !base.hasAuthority)) && (this.m_NetworkTransform.lastSyncTime != 0f))
            {
                s_LineMaterial.SetPass(0);
                GL.Begin(1);
                GL.Color(Color.white);
                GL.Vertex3(base.transform.position.x, base.transform.position.y, base.transform.position.z);
                GL.Vertex3(this.m_NetworkTransform.targetSyncPosition.x, this.m_NetworkTransform.targetSyncPosition.y, this.m_NetworkTransform.targetSyncPosition.z);
                GL.End();
                this.DrawRotationInterpolation();
            }
        }

        public override void OnStartClient()
        {
            if (this.m_VisualizerPrefab != null)
            {
                this.m_NetworkTransform = base.GetComponent<NetworkTransform>();
                CreateLineMaterial();
                this.m_Visualizer = (GameObject) UnityEngine.Object.Instantiate(this.m_VisualizerPrefab, base.transform.position, Quaternion.identity);
            }
        }

        public override void OnStartLocalPlayer()
        {
            if ((this.m_Visualizer != null) && (this.m_NetworkTransform.localPlayerAuthority || base.isServer))
            {
                UnityEngine.Object.Destroy(this.m_Visualizer);
            }
        }

        public GameObject visualizerPrefab
        {
            get
            {
                return this.m_VisualizerPrefab;
            }
            set
            {
                this.m_VisualizerPrefab = value;
            }
        }
    }
}

