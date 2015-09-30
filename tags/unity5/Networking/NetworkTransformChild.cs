namespace UnityEngine.Networking
{
    using System;
    using UnityEditor;
    using UnityEngine;

    [AddComponentMenu("Network/NetworkTransformChild")]
    public class NetworkTransformChild : NetworkBehaviour
    {
        private const float k_LocalMovementThreshold = 1E-05f;
        private const float k_LocalRotationThreshold = 1E-05f;
        [SerializeField]
        private uint m_ChildIndex;
        [SerializeField]
        private NetworkTransform.ClientMoveCallback3D m_ClientMoveCallback3D;
        [SerializeField]
        private float m_InterpolateMovement = 0.5f;
        [SerializeField]
        private float m_InterpolateRotation = 0.5f;
        private float m_LastClientSendTime;
        private float m_LastClientSyncTime;
        private NetworkWriter m_LocalTransformWriter;
        [SerializeField]
        private float m_MovementThreshold = 0.001f;
        private Vector3 m_PrevPosition;
        private Quaternion m_PrevRotation;
        private NetworkTransform m_Root;
        [SerializeField]
        private NetworkTransform.CompressionSyncMode m_RotationSyncCompression;
        [SerializeField]
        private float m_SendInterval = 0.1f;
        [SerializeField]
        private NetworkTransform.AxisSyncMode m_SyncRotationAxis = NetworkTransform.AxisSyncMode.AxisXYZ;
        [SerializeField]
        private Transform m_Target;
        private Vector3 m_TargetSyncPosition;
        private Quaternion m_TargetSyncRotation3D;

        private void Awake()
        {
            this.m_PrevPosition = this.m_Target.localPosition;
            this.m_PrevRotation = this.m_Target.localRotation;
            if (base.localPlayerAuthority)
            {
                this.m_LocalTransformWriter = new NetworkWriter();
            }
        }

        private void FixedUpdate()
        {
            if (base.isServer)
            {
                this.FixedUpdateServer();
            }
            if (base.isClient)
            {
                this.FixedUpdateClient();
            }
        }

        private void FixedUpdateClient()
        {
            if ((((((this.m_LastClientSyncTime != 0f) && (NetworkServer.active || NetworkClient.active)) && (base.isServer || base.isClient)) && (this.GetNetworkSendInterval() != 0f)) && !base.hasAuthority) && (this.m_LastClientSyncTime != 0f))
            {
                this.m_Target.localPosition = Vector3.Lerp(this.m_Target.localPosition, this.m_TargetSyncPosition, this.m_InterpolateMovement);
                this.m_Target.localRotation = Quaternion.Slerp(this.m_Target.localRotation, this.m_TargetSyncRotation3D, this.m_InterpolateRotation);
            }
        }

        private void FixedUpdateServer()
        {
            if ((((base.syncVarDirtyBits == 0) && NetworkServer.active) && base.isServer) && (this.GetNetworkSendInterval() != 0f))
            {
                Vector3 vector = this.m_Target.localPosition - this.m_PrevPosition;
                if ((vector.sqrMagnitude >= this.movementThreshold) || (Quaternion.Angle(this.m_PrevRotation, this.m_Target.localRotation) >= this.movementThreshold))
                {
                    base.SetDirtyBit(1);
                }
            }
        }

        public override int GetNetworkChannel()
        {
            return 1;
        }

        public override float GetNetworkSendInterval()
        {
            return this.m_SendInterval;
        }

        internal static void HandleChildTransform(NetworkMessage netMsg)
        {
            NetworkInstanceId netId = netMsg.reader.ReadNetworkId();
            uint index = netMsg.reader.ReadPackedUInt32();
            NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Incoming, 0x10, "16:LocalChildTransform", 1);
            GameObject obj2 = NetworkServer.FindLocalObject(netId);
            if (obj2 == null)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("HandleChildTransform no gameObject");
                }
            }
            else
            {
                NetworkTransformChild[] components = obj2.GetComponents<NetworkTransformChild>();
                if ((components == null) || (components.Length == 0))
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError("HandleChildTransform no children");
                    }
                }
                else if (index >= components.Length)
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError("HandleChildTransform childIndex invalid");
                    }
                }
                else
                {
                    NetworkTransformChild child = components[index];
                    if (child == null)
                    {
                        if (LogFilter.logError)
                        {
                            Debug.LogError("HandleChildTransform null target");
                        }
                    }
                    else if (!child.localPlayerAuthority)
                    {
                        if (LogFilter.logError)
                        {
                            Debug.LogError("HandleChildTransform no localPlayerAuthority");
                        }
                    }
                    else if (!netMsg.conn.clientOwnedObjects.Contains(netId))
                    {
                        if (LogFilter.logWarn)
                        {
                            Debug.LogWarning("NetworkTransformChild netId:" + netId + " is not for a valid player");
                        }
                    }
                    else
                    {
                        child.UnserializeModeTransform(netMsg.reader, false);
                        child.m_LastClientSyncTime = Time.time;
                        if (!child.isClient)
                        {
                            child.m_Target.localPosition = child.m_TargetSyncPosition;
                            child.m_Target.localRotation = child.m_TargetSyncRotation3D;
                        }
                    }
                }
            }
        }

        private bool HasMoved()
        {
            Vector3 vector = this.m_Target.localPosition - this.m_PrevPosition;
            return ((vector.sqrMagnitude > 1E-05f) || (Quaternion.Angle(this.m_Target.localRotation, this.m_PrevRotation) > 1E-05f));
        }

        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            if ((!base.isServer || !NetworkServer.localClientActive) && (initialState || (reader.ReadPackedUInt32() != 0)))
            {
                this.UnserializeModeTransform(reader, initialState);
                this.m_LastClientSyncTime = Time.time;
            }
        }

        public override bool OnSerialize(NetworkWriter writer, bool initialState)
        {
            if (!initialState)
            {
                if (base.syncVarDirtyBits == 0)
                {
                    writer.WritePackedUInt32(0);
                    return false;
                }
                writer.WritePackedUInt32(1);
            }
            this.SerializeModeTransform(writer);
            return true;
        }

        private void OnValidate()
        {
            if (this.m_Target != null)
            {
                Transform parent = this.m_Target.parent;
                if (parent == null)
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError("NetworkTransformChild target cannot be the root transform.");
                    }
                    this.m_Target = null;
                    return;
                }
                while (parent.parent != null)
                {
                    parent = parent.parent;
                }
                this.m_Root = parent.gameObject.GetComponent<NetworkTransform>();
                if (this.m_Root == null)
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError("NetworkTransformChild root must have NetworkTransform");
                    }
                    this.m_Target = null;
                    return;
                }
            }
            this.m_ChildIndex = uint.MaxValue;
            NetworkTransformChild[] components = this.m_Root.GetComponents<NetworkTransformChild>();
            for (uint i = 0; i < components.Length; i++)
            {
                if (components[i] == this)
                {
                    this.m_ChildIndex = i;
                    break;
                }
            }
            if (this.m_ChildIndex == uint.MaxValue)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("NetworkTransformChild component must be a child in the same hierarchy");
                }
                this.m_Target = null;
            }
            if (this.m_SendInterval < 0f)
            {
                this.m_SendInterval = 0f;
            }
            if ((this.m_SyncRotationAxis < NetworkTransform.AxisSyncMode.None) || (this.m_SyncRotationAxis > NetworkTransform.AxisSyncMode.AxisXYZ))
            {
                this.m_SyncRotationAxis = NetworkTransform.AxisSyncMode.None;
            }
            if (this.movementThreshold < 0f)
            {
                this.movementThreshold = 0f;
            }
            if (this.interpolateRotation < 0f)
            {
                this.interpolateRotation = 0.01f;
            }
            if (this.interpolateRotation > 1f)
            {
                this.interpolateRotation = 1f;
            }
            if (this.interpolateMovement < 0f)
            {
                this.interpolateMovement = 0.01f;
            }
            if (this.interpolateMovement > 1f)
            {
                this.interpolateMovement = 1f;
            }
        }

        [Client]
        private void SendTransform()
        {
            if (this.HasMoved() && (ClientScene.readyConnection != null))
            {
                this.m_LocalTransformWriter.StartMessage(0x10);
                this.m_LocalTransformWriter.Write(base.netId);
                this.m_LocalTransformWriter.WritePackedUInt32(this.m_ChildIndex);
                this.SerializeModeTransform(this.m_LocalTransformWriter);
                this.m_PrevPosition = this.m_Target.localPosition;
                this.m_PrevRotation = this.m_Target.localRotation;
                this.m_LocalTransformWriter.FinishMessage();
                NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Outgoing, 0x10, "16:LocalChildTransform", 1);
                ClientScene.readyConnection.SendWriter(this.m_LocalTransformWriter, this.GetNetworkChannel());
            }
        }

        private void SerializeModeTransform(NetworkWriter writer)
        {
            writer.Write(this.m_Target.localPosition);
            if (this.m_SyncRotationAxis != NetworkTransform.AxisSyncMode.None)
            {
                NetworkTransform.SerializeRotation3D(writer, this.m_Target.localRotation, this.syncRotationAxis, this.rotationSyncCompression);
            }
            this.m_PrevPosition = this.m_Target.localPosition;
            this.m_PrevRotation = this.m_Target.localRotation;
        }

        private void UnserializeModeTransform(NetworkReader reader, bool initialState)
        {
            if (base.hasAuthority)
            {
                reader.ReadVector3();
                if (this.syncRotationAxis != NetworkTransform.AxisSyncMode.None)
                {
                    NetworkTransform.UnserializeRotation3D(reader, this.syncRotationAxis, this.rotationSyncCompression);
                }
            }
            else if (base.isServer && (this.m_ClientMoveCallback3D != null))
            {
                Vector3 position = reader.ReadVector3();
                Vector3 zero = Vector3.zero;
                Quaternion identity = Quaternion.identity;
                if (this.syncRotationAxis != NetworkTransform.AxisSyncMode.None)
                {
                    identity = NetworkTransform.UnserializeRotation3D(reader, this.syncRotationAxis, this.rotationSyncCompression);
                }
                if (this.m_ClientMoveCallback3D(ref position, ref zero, ref identity))
                {
                    this.m_TargetSyncPosition = position;
                    if (this.syncRotationAxis != NetworkTransform.AxisSyncMode.None)
                    {
                        this.m_TargetSyncRotation3D = identity;
                    }
                }
            }
            else
            {
                this.m_TargetSyncPosition = reader.ReadVector3();
                if (this.syncRotationAxis != NetworkTransform.AxisSyncMode.None)
                {
                    this.m_TargetSyncRotation3D = NetworkTransform.UnserializeRotation3D(reader, this.syncRotationAxis, this.rotationSyncCompression);
                }
            }
        }

        private void Update()
        {
            if (((base.hasAuthority && base.localPlayerAuthority) && !NetworkServer.active) && ((Time.time - this.m_LastClientSendTime) > this.GetNetworkSendInterval()))
            {
                this.SendTransform();
                this.m_LastClientSendTime = Time.time;
            }
        }

        public uint childIndex
        {
            get
            {
                return this.m_ChildIndex;
            }
        }

        public NetworkTransform.ClientMoveCallback3D clientMoveCallback3D
        {
            get
            {
                return this.m_ClientMoveCallback3D;
            }
            set
            {
                this.m_ClientMoveCallback3D = value;
            }
        }

        public float interpolateMovement
        {
            get
            {
                return this.m_InterpolateMovement;
            }
            set
            {
                this.m_InterpolateMovement = value;
            }
        }

        public float interpolateRotation
        {
            get
            {
                return this.m_InterpolateRotation;
            }
            set
            {
                this.m_InterpolateRotation = value;
            }
        }

        public float lastSyncTime
        {
            get
            {
                return this.m_LastClientSyncTime;
            }
        }

        public float movementThreshold
        {
            get
            {
                return this.m_MovementThreshold;
            }
            set
            {
                this.m_MovementThreshold = value;
            }
        }

        public NetworkTransform.CompressionSyncMode rotationSyncCompression
        {
            get
            {
                return this.m_RotationSyncCompression;
            }
            set
            {
                this.m_RotationSyncCompression = value;
            }
        }

        public float sendInterval
        {
            get
            {
                return this.m_SendInterval;
            }
            set
            {
                this.m_SendInterval = value;
            }
        }

        public NetworkTransform.AxisSyncMode syncRotationAxis
        {
            get
            {
                return this.m_SyncRotationAxis;
            }
            set
            {
                this.m_SyncRotationAxis = value;
            }
        }

        public Transform target
        {
            get
            {
                return this.m_Target;
            }
            set
            {
                this.m_Target = value;
                this.OnValidate();
            }
        }

        public Vector3 targetSyncPosition
        {
            get
            {
                return this.m_TargetSyncPosition;
            }
        }

        public Quaternion targetSyncRotation3D
        {
            get
            {
                return this.m_TargetSyncRotation3D;
            }
        }
    }
}

