namespace UnityEngine.Networking
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    [DisallowMultipleComponent, AddComponentMenu("Network/NetworkTransform")]
    public class NetworkTransform : NetworkBehaviour
    {
        private const float k_LocalMovementThreshold = 1E-05f;
        private const float k_LocalRotationThreshold = 1E-05f;
        private const float k_LocalVelocityThreshold = 1E-05f;
        private const float k_MoveAheadRatio = 0.1f;
        private CharacterController m_CharacterController;
        [SerializeField]
        private ClientMoveCallback2D m_ClientMoveCallback2D;
        [SerializeField]
        private ClientMoveCallback3D m_ClientMoveCallback3D;
        private Vector3 m_FixedPosDiff;
        private bool m_Grounded = true;
        [SerializeField]
        private float m_InterpolateMovement = 1f;
        [SerializeField]
        private float m_InterpolateRotation = 1f;
        private float m_LastClientSendTime;
        private float m_LastClientSyncTime;
        private NetworkWriter m_LocalTransformWriter;
        [SerializeField]
        private float m_MovementTheshold = 0.001f;
        private Vector3 m_PrevPosition;
        private Quaternion m_PrevRotation;
        private float m_PrevRotation2D;
        private float m_PrevVelocity;
        private Rigidbody2D m_RigidBody2D;
        private Rigidbody m_RigidBody3D;
        [SerializeField]
        private CompressionSyncMode m_RotationSyncCompression;
        [SerializeField]
        private float m_SendInterval = 0.1f;
        [SerializeField]
        private float m_SnapThreshold = 5f;
        [SerializeField]
        private AxisSyncMode m_SyncRotationAxis = AxisSyncMode.AxisXYZ;
        [SerializeField]
        private bool m_SyncSpin;
        private float m_TargetSyncAngularVelocity2D;
        private Vector3 m_TargetSyncAngularVelocity3D;
        private Vector3 m_TargetSyncPosition;
        private float m_TargetSyncRotation2D;
        private Quaternion m_TargetSyncRotation3D;
        private Vector3 m_TargetSyncVelocity;
        [SerializeField]
        private TransformSyncMode m_TransformSyncMode;

        private void Awake()
        {
            this.m_RigidBody3D = base.GetComponent<Rigidbody>();
            this.m_RigidBody2D = base.GetComponent<Rigidbody2D>();
            this.m_CharacterController = base.GetComponent<CharacterController>();
            this.m_PrevPosition = base.transform.position;
            this.m_PrevRotation = base.transform.rotation;
            this.m_PrevVelocity = 0f;
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
            if (((((this.m_LastClientSyncTime != 0f) && (NetworkServer.active || NetworkClient.active)) && (base.isServer || base.isClient)) && (this.GetNetworkSendInterval() != 0f)) && !base.hasAuthority)
            {
                switch (this.transformSyncMode)
                {
                    case TransformSyncMode.SyncNone:
                        return;

                    case TransformSyncMode.SyncTransform:
                        return;

                    case TransformSyncMode.SyncRigidbody2D:
                        this.InterpolateTransformMode2D();
                        break;

                    case TransformSyncMode.SyncRigidbody3D:
                        this.InterpolateTransformMode3D();
                        break;

                    case TransformSyncMode.SyncCharacterController:
                        this.InterpolateTransformModeCharacterController();
                        break;
                }
            }
        }

        private void FixedUpdateServer()
        {
            if ((((base.syncVarDirtyBits == 0) && NetworkServer.active) && base.isServer) && (this.GetNetworkSendInterval() != 0f))
            {
                Vector3 vector = base.transform.position - this.m_PrevPosition;
                if ((vector.magnitude >= this.movementTheshold) || (Quaternion.Angle(this.m_PrevRotation, base.transform.rotation) >= this.movementTheshold))
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

        public static void HandleTransform(NetworkMessage netMsg)
        {
            NetworkInstanceId netId = netMsg.reader.ReadNetworkId();
            NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Incoming, 6, "6:LocalPlayerTransform", 1);
            GameObject obj2 = NetworkServer.FindLocalObject(netId);
            if (obj2 == null)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("HandleTransform no gameObject");
                }
            }
            else
            {
                NetworkTransform component = obj2.GetComponent<NetworkTransform>();
                if (component == null)
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError("HandleTransform null target");
                    }
                }
                else if (!component.localPlayerAuthority)
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError("HandleTransform no localPlayerAuthority");
                    }
                }
                else if (netMsg.conn.clientOwnedObjects == null)
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError("HandleTransform object not owned by connection");
                    }
                }
                else if (!netMsg.conn.clientOwnedObjects.Contains(netId))
                {
                    if (LogFilter.logWarn)
                    {
                        Debug.LogWarning("HandleTransform netId:" + netId + " is not for a valid player");
                    }
                }
                else
                {
                    switch (component.transformSyncMode)
                    {
                        case TransformSyncMode.SyncNone:
                            return;

                        case TransformSyncMode.SyncTransform:
                            component.UnserializeModeTransform(netMsg.reader, false);
                            break;

                        case TransformSyncMode.SyncRigidbody2D:
                            component.UnserializeMode2D(netMsg.reader, false);
                            break;

                        case TransformSyncMode.SyncRigidbody3D:
                            component.UnserializeMode3D(netMsg.reader, false);
                            break;

                        case TransformSyncMode.SyncCharacterController:
                            component.UnserializeModeCharacterController(netMsg.reader, false);
                            break;
                    }
                    component.m_LastClientSyncTime = Time.time;
                }
            }
        }

        private bool HasMoved()
        {
            float magnitude = 0f;
            if (this.m_RigidBody3D != null)
            {
                Vector3 vector = this.m_RigidBody3D.position - this.m_PrevPosition;
                magnitude = vector.magnitude;
            }
            else if (this.m_RigidBody2D != null)
            {
                Vector2 vector2 = this.m_RigidBody2D.position - this.m_PrevPosition;
                magnitude = vector2.magnitude;
            }
            else
            {
                Vector3 vector3 = base.transform.position - this.m_PrevPosition;
                magnitude = vector3.magnitude;
            }
            if (magnitude > 1E-05f)
            {
                return true;
            }
            if (this.m_RigidBody3D != null)
            {
                magnitude = Quaternion.Angle(this.m_RigidBody3D.rotation, this.m_PrevRotation);
            }
            else if (this.m_RigidBody2D != null)
            {
                magnitude = Math.Abs((float) (this.m_RigidBody2D.rotation - this.m_PrevRotation2D));
            }
            else
            {
                magnitude = Quaternion.Angle(base.transform.rotation, this.m_PrevRotation);
            }
            if (magnitude > 1E-05f)
            {
                return true;
            }
            if (this.m_RigidBody3D != null)
            {
                magnitude = Mathf.Abs((float) (this.m_RigidBody3D.velocity.sqrMagnitude - this.m_PrevVelocity));
            }
            else if (this.m_RigidBody2D != null)
            {
                magnitude = Mathf.Abs((float) (this.m_RigidBody2D.velocity.sqrMagnitude - this.m_PrevVelocity));
            }
            return (magnitude > 1E-05f);
        }

        private void InterpolateTransformMode2D()
        {
            if (this.m_InterpolateMovement != 0f)
            {
                Vector2 velocity = this.m_RigidBody2D.velocity;
                Vector2 vector2 = (Vector2) (((this.m_TargetSyncPosition - this.m_RigidBody2D.position) * this.m_InterpolateMovement) / this.GetNetworkSendInterval());
                if (!this.m_Grounded && (vector2.y < 0f))
                {
                    vector2.y = velocity.y;
                }
                this.m_RigidBody2D.velocity = vector2;
            }
            if (this.interpolateRotation != 0f)
            {
                float num = this.m_RigidBody2D.rotation % 360f;
                if (num < 0f)
                {
                    num += 360f;
                }
                Quaternion quaternion = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(0f, 0f, this.m_TargetSyncRotation2D), (Time.fixedDeltaTime * this.interpolateRotation) / this.GetNetworkSendInterval());
                this.m_RigidBody2D.MoveRotation(quaternion.eulerAngles.z);
                this.m_TargetSyncRotation2D += (this.m_TargetSyncAngularVelocity2D * Time.fixedDeltaTime) * 0.1f;
            }
            this.m_TargetSyncPosition += (Vector3) ((this.m_TargetSyncVelocity * Time.fixedDeltaTime) * 0.1f);
        }

        private void InterpolateTransformMode3D()
        {
            if (this.m_InterpolateMovement != 0f)
            {
                Vector3 vector = (Vector3) (((this.m_TargetSyncPosition - this.m_RigidBody3D.position) * this.m_InterpolateMovement) / this.GetNetworkSendInterval());
                this.m_RigidBody3D.velocity = vector;
            }
            if (this.interpolateRotation != 0f)
            {
                this.m_RigidBody3D.MoveRotation(Quaternion.Slerp(this.m_RigidBody3D.rotation, this.m_TargetSyncRotation3D, Time.fixedDeltaTime * this.interpolateRotation));
            }
            this.m_TargetSyncPosition += (Vector3) ((this.m_TargetSyncVelocity * Time.fixedDeltaTime) * 0.1f);
        }

        private void InterpolateTransformModeCharacterController()
        {
            if ((this.m_FixedPosDiff != Vector3.zero) || (this.m_TargetSyncRotation3D != base.transform.rotation))
            {
                if (this.m_InterpolateMovement != 0f)
                {
                    this.m_CharacterController.Move((Vector3) (this.m_FixedPosDiff * this.m_InterpolateMovement));
                }
                if (this.interpolateRotation != 0f)
                {
                    base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.m_TargetSyncRotation3D, (Time.fixedDeltaTime * this.interpolateRotation) * 10f);
                }
                if ((Time.time - this.m_LastClientSyncTime) > this.GetNetworkSendInterval())
                {
                    this.m_FixedPosDiff = Vector3.zero;
                    Vector3 motion = this.m_TargetSyncPosition - base.transform.position;
                    this.m_CharacterController.Move(motion);
                }
            }
        }

        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            if ((!base.isServer || !NetworkServer.localClientActive) && (initialState || (reader.ReadPackedUInt32() != 0)))
            {
                switch (this.transformSyncMode)
                {
                    case TransformSyncMode.SyncNone:
                        return;

                    case TransformSyncMode.SyncTransform:
                        this.UnserializeModeTransform(reader, initialState);
                        break;

                    case TransformSyncMode.SyncRigidbody2D:
                        this.UnserializeMode2D(reader, initialState);
                        break;

                    case TransformSyncMode.SyncRigidbody3D:
                        this.UnserializeMode3D(reader, initialState);
                        break;

                    case TransformSyncMode.SyncCharacterController:
                        this.UnserializeModeCharacterController(reader, initialState);
                        break;
                }
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
            switch (this.transformSyncMode)
            {
                case TransformSyncMode.SyncNone:
                    return false;

                case TransformSyncMode.SyncTransform:
                    this.SerializeModeTransform(writer);
                    break;

                case TransformSyncMode.SyncRigidbody2D:
                    this.SerializeMode2D(writer);
                    break;

                case TransformSyncMode.SyncRigidbody3D:
                    this.SerializeMode3D(writer);
                    break;

                case TransformSyncMode.SyncCharacterController:
                    this.SerializeModeCharacterController(writer);
                    break;
            }
            return true;
        }

        private void OnValidate()
        {
            if ((this.m_TransformSyncMode < TransformSyncMode.SyncNone) || (this.m_TransformSyncMode > TransformSyncMode.SyncCharacterController))
            {
                this.m_TransformSyncMode = TransformSyncMode.SyncTransform;
            }
            if (this.m_SendInterval < 0f)
            {
                this.m_SendInterval = 0f;
            }
            if ((this.m_SyncRotationAxis < AxisSyncMode.None) || (this.m_SyncRotationAxis > AxisSyncMode.AxisXYZ))
            {
                this.m_SyncRotationAxis = AxisSyncMode.None;
            }
            if (this.m_MovementTheshold < 0f)
            {
                this.m_MovementTheshold = 0f;
            }
            if (this.m_SnapThreshold < 0f)
            {
                this.m_SnapThreshold = 0.01f;
            }
            if (this.m_InterpolateRotation < 0f)
            {
                this.m_InterpolateRotation = 0.01f;
            }
            if (this.m_InterpolateMovement < 0f)
            {
                this.m_InterpolateMovement = 0.01f;
            }
        }

        private static float ReadAngle(NetworkReader reader, CompressionSyncMode compression)
        {
            switch (compression)
            {
                case CompressionSyncMode.None:
                    return reader.ReadSingle();

                case CompressionSyncMode.Low:
                    return (float) reader.ReadInt16();

                case CompressionSyncMode.High:
                    return (float) reader.ReadInt16();
            }
            return 0f;
        }

        [Client]
        private void SendTransform()
        {
            if (this.HasMoved() && (ClientScene.readyConnection != null))
            {
                this.m_LocalTransformWriter.StartMessage(6);
                this.m_LocalTransformWriter.Write(base.netId);
                switch (this.transformSyncMode)
                {
                    case TransformSyncMode.SyncNone:
                        return;

                    case TransformSyncMode.SyncTransform:
                        this.SerializeModeTransform(this.m_LocalTransformWriter);
                        break;

                    case TransformSyncMode.SyncRigidbody2D:
                        this.SerializeMode2D(this.m_LocalTransformWriter);
                        break;

                    case TransformSyncMode.SyncRigidbody3D:
                        this.SerializeMode3D(this.m_LocalTransformWriter);
                        break;

                    case TransformSyncMode.SyncCharacterController:
                        this.SerializeModeCharacterController(this.m_LocalTransformWriter);
                        break;
                }
                if (this.m_RigidBody3D != null)
                {
                    this.m_PrevPosition = this.m_RigidBody3D.position;
                    this.m_PrevRotation = this.m_RigidBody3D.rotation;
                    this.m_PrevVelocity = this.m_RigidBody3D.velocity.sqrMagnitude;
                }
                else if (this.m_RigidBody2D != null)
                {
                    this.m_PrevPosition = (Vector3) this.m_RigidBody2D.position;
                    this.m_PrevRotation2D = this.m_RigidBody2D.rotation;
                    this.m_PrevVelocity = this.m_RigidBody2D.velocity.sqrMagnitude;
                }
                else
                {
                    this.m_PrevPosition = base.transform.position;
                    this.m_PrevRotation = base.transform.rotation;
                }
                this.m_LocalTransformWriter.FinishMessage();
                NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Outgoing, 6, "6:LocalPlayerTransform", 1);
                ClientScene.readyConnection.SendWriter(this.m_LocalTransformWriter, this.GetNetworkChannel());
            }
        }

        private void SerializeMode2D(NetworkWriter writer)
        {
            if (base.isServer && (this.m_LastClientSyncTime != 0f))
            {
                writer.Write((Vector2) this.m_TargetSyncPosition);
                SerializeVelocity2D(writer, this.m_TargetSyncVelocity, CompressionSyncMode.None);
                if (this.syncRotationAxis != AxisSyncMode.None)
                {
                    float rot = this.m_TargetSyncRotation2D % 360f;
                    if (rot < 0f)
                    {
                        rot += 360f;
                    }
                    SerializeRotation2D(writer, rot, this.rotationSyncCompression);
                }
            }
            else
            {
                writer.Write(this.m_RigidBody2D.position);
                SerializeVelocity2D(writer, this.m_RigidBody2D.velocity, CompressionSyncMode.None);
                if (this.syncRotationAxis != AxisSyncMode.None)
                {
                    float num2 = this.m_RigidBody2D.rotation % 360f;
                    if (num2 < 0f)
                    {
                        num2 += 360f;
                    }
                    SerializeRotation2D(writer, num2, this.rotationSyncCompression);
                }
            }
            if (this.m_SyncSpin)
            {
                SerializeSpin2D(writer, this.m_RigidBody2D.angularVelocity, this.rotationSyncCompression);
            }
            this.m_PrevPosition = (Vector3) this.m_RigidBody2D.position;
            this.m_PrevRotation = base.transform.rotation;
            this.m_PrevVelocity = this.m_RigidBody2D.velocity.sqrMagnitude;
        }

        private void SerializeMode3D(NetworkWriter writer)
        {
            if (base.isServer && (this.m_LastClientSyncTime != 0f))
            {
                writer.Write(this.m_TargetSyncPosition);
                SerializeVelocity3D(writer, this.m_TargetSyncVelocity, CompressionSyncMode.None);
                if (this.syncRotationAxis != AxisSyncMode.None)
                {
                    SerializeRotation3D(writer, this.m_TargetSyncRotation3D, this.syncRotationAxis, this.rotationSyncCompression);
                }
            }
            else
            {
                writer.Write(this.m_RigidBody3D.position);
                SerializeVelocity3D(writer, this.m_RigidBody3D.velocity, CompressionSyncMode.None);
                if (this.syncRotationAxis != AxisSyncMode.None)
                {
                    SerializeRotation3D(writer, this.m_RigidBody3D.rotation, this.syncRotationAxis, this.rotationSyncCompression);
                }
            }
            if (this.m_SyncSpin)
            {
                SerializeSpin3D(writer, this.m_RigidBody3D.angularVelocity, this.syncRotationAxis, this.rotationSyncCompression);
            }
            this.m_PrevPosition = this.m_RigidBody3D.position;
            this.m_PrevRotation = base.transform.rotation;
            this.m_PrevVelocity = this.m_RigidBody3D.velocity.sqrMagnitude;
        }

        private void SerializeModeCharacterController(NetworkWriter writer)
        {
            if (base.isServer && (this.m_LastClientSyncTime != 0f))
            {
                writer.Write(this.m_TargetSyncPosition);
                if (this.syncRotationAxis != AxisSyncMode.None)
                {
                    SerializeRotation3D(writer, this.m_TargetSyncRotation3D, this.syncRotationAxis, this.rotationSyncCompression);
                }
            }
            else
            {
                writer.Write(base.transform.position);
                if (this.syncRotationAxis != AxisSyncMode.None)
                {
                    SerializeRotation3D(writer, base.transform.rotation, this.syncRotationAxis, this.rotationSyncCompression);
                }
            }
            this.m_PrevPosition = base.transform.position;
            this.m_PrevRotation = base.transform.rotation;
            this.m_PrevVelocity = 0f;
        }

        private void SerializeModeTransform(NetworkWriter writer)
        {
            writer.Write(base.transform.position);
            if (this.m_SyncRotationAxis != AxisSyncMode.None)
            {
                SerializeRotation3D(writer, base.transform.rotation, this.syncRotationAxis, this.rotationSyncCompression);
            }
            this.m_PrevPosition = base.transform.position;
            this.m_PrevRotation = base.transform.rotation;
            this.m_PrevVelocity = 0f;
        }

        public static void SerializeRotation2D(NetworkWriter writer, float rot, CompressionSyncMode compression)
        {
            WriteAngle(writer, rot, compression);
        }

        public static void SerializeRotation3D(NetworkWriter writer, Quaternion rot, AxisSyncMode mode, CompressionSyncMode compression)
        {
            switch (mode)
            {
                case AxisSyncMode.AxisX:
                    WriteAngle(writer, rot.eulerAngles.x, compression);
                    break;

                case AxisSyncMode.AxisY:
                    WriteAngle(writer, rot.eulerAngles.y, compression);
                    break;

                case AxisSyncMode.AxisZ:
                    WriteAngle(writer, rot.eulerAngles.z, compression);
                    break;

                case AxisSyncMode.AxisXY:
                    WriteAngle(writer, rot.eulerAngles.x, compression);
                    WriteAngle(writer, rot.eulerAngles.y, compression);
                    break;

                case AxisSyncMode.AxisXZ:
                    WriteAngle(writer, rot.eulerAngles.x, compression);
                    WriteAngle(writer, rot.eulerAngles.z, compression);
                    break;

                case AxisSyncMode.AxisYZ:
                    WriteAngle(writer, rot.eulerAngles.y, compression);
                    WriteAngle(writer, rot.eulerAngles.z, compression);
                    break;

                case AxisSyncMode.AxisXYZ:
                    WriteAngle(writer, rot.eulerAngles.x, compression);
                    WriteAngle(writer, rot.eulerAngles.y, compression);
                    WriteAngle(writer, rot.eulerAngles.z, compression);
                    break;
            }
        }

        public static void SerializeSpin2D(NetworkWriter writer, float angularVelocity, CompressionSyncMode compression)
        {
            WriteAngle(writer, angularVelocity, compression);
        }

        public static void SerializeSpin3D(NetworkWriter writer, Vector3 angularVelocity, AxisSyncMode mode, CompressionSyncMode compression)
        {
            switch (mode)
            {
                case AxisSyncMode.AxisX:
                    WriteAngle(writer, angularVelocity.x, compression);
                    break;

                case AxisSyncMode.AxisY:
                    WriteAngle(writer, angularVelocity.y, compression);
                    break;

                case AxisSyncMode.AxisZ:
                    WriteAngle(writer, angularVelocity.z, compression);
                    break;

                case AxisSyncMode.AxisXY:
                    WriteAngle(writer, angularVelocity.x, compression);
                    WriteAngle(writer, angularVelocity.y, compression);
                    break;

                case AxisSyncMode.AxisXZ:
                    WriteAngle(writer, angularVelocity.x, compression);
                    WriteAngle(writer, angularVelocity.z, compression);
                    break;

                case AxisSyncMode.AxisYZ:
                    WriteAngle(writer, angularVelocity.y, compression);
                    WriteAngle(writer, angularVelocity.z, compression);
                    break;

                case AxisSyncMode.AxisXYZ:
                    WriteAngle(writer, angularVelocity.x, compression);
                    WriteAngle(writer, angularVelocity.y, compression);
                    WriteAngle(writer, angularVelocity.z, compression);
                    break;
            }
        }

        public static void SerializeVelocity2D(NetworkWriter writer, Vector2 velocity, CompressionSyncMode compression)
        {
            writer.Write(velocity);
        }

        public static void SerializeVelocity3D(NetworkWriter writer, Vector3 velocity, CompressionSyncMode compression)
        {
            writer.Write(velocity);
        }

        private void UnserializeMode2D(NetworkReader reader, bool initialState)
        {
            if (base.hasAuthority)
            {
                Vector2 vector = reader.ReadVector2();
                Vector2 vector2 = reader.ReadVector2();
                float angle = 0f;
                if (this.syncRotationAxis != AxisSyncMode.None)
                {
                    angle = UnserializeRotation2D(reader, this.rotationSyncCompression);
                }
                if (this.syncSpin)
                {
                    UnserializeSpin2D(reader, this.rotationSyncCompression);
                }
                Vector2 vector5 = vector - this.m_RigidBody2D.position;
                if (Mathf.Abs(vector5.magnitude) > this.m_SnapThreshold)
                {
                    base.transform.position = (Vector3) vector;
                    this.m_RigidBody2D.MoveRotation(angle);
                    this.m_RigidBody2D.velocity = vector2;
                }
            }
            else if (this.m_RigidBody2D != null)
            {
                if (base.isServer && (this.m_ClientMoveCallback2D != null))
                {
                    Vector2 position = reader.ReadVector2();
                    Vector2 velocity = reader.ReadVector2();
                    float rotation = 0f;
                    if (this.syncRotationAxis != AxisSyncMode.None)
                    {
                        rotation = UnserializeRotation2D(reader, this.rotationSyncCompression);
                    }
                    if (!this.m_ClientMoveCallback2D(ref position, ref velocity, ref rotation))
                    {
                        return;
                    }
                    this.m_TargetSyncPosition = (Vector3) position;
                    this.m_TargetSyncVelocity = (Vector3) velocity;
                    if (this.syncRotationAxis != AxisSyncMode.None)
                    {
                        this.m_TargetSyncRotation2D = rotation;
                    }
                }
                else
                {
                    this.m_TargetSyncPosition = (Vector3) reader.ReadVector2();
                    this.m_TargetSyncVelocity = (Vector3) reader.ReadVector2();
                    if (this.syncRotationAxis != AxisSyncMode.None)
                    {
                        this.m_TargetSyncRotation2D = UnserializeRotation2D(reader, this.rotationSyncCompression);
                    }
                }
                if (this.syncSpin)
                {
                    this.m_TargetSyncAngularVelocity2D = UnserializeSpin2D(reader, this.rotationSyncCompression);
                }
                if (base.isServer && !base.isClient)
                {
                    base.transform.position = this.m_TargetSyncPosition;
                    this.m_RigidBody2D.MoveRotation(this.m_TargetSyncRotation2D);
                    this.m_RigidBody2D.velocity = this.m_TargetSyncVelocity;
                }
                else if (this.GetNetworkSendInterval() == 0f)
                {
                    base.transform.position = this.m_TargetSyncPosition;
                    this.m_RigidBody2D.velocity = this.m_TargetSyncVelocity;
                    if (this.syncRotationAxis != AxisSyncMode.None)
                    {
                        this.m_RigidBody2D.MoveRotation(this.m_TargetSyncRotation2D);
                    }
                    if (this.syncSpin)
                    {
                        this.m_RigidBody2D.angularVelocity = this.m_TargetSyncAngularVelocity2D;
                    }
                }
                else
                {
                    Vector2 vector6 = this.m_RigidBody2D.position - this.m_TargetSyncPosition;
                    if (vector6.magnitude > this.snapThreshold)
                    {
                        this.m_RigidBody2D.position = this.m_TargetSyncPosition;
                        this.m_RigidBody2D.velocity = this.m_TargetSyncVelocity;
                    }
                    if ((this.interpolateRotation == 0f) && (this.syncRotationAxis != AxisSyncMode.None))
                    {
                        this.m_RigidBody2D.rotation = this.m_TargetSyncRotation2D;
                        if (this.syncSpin)
                        {
                            this.m_RigidBody2D.angularVelocity = this.m_TargetSyncAngularVelocity2D;
                        }
                    }
                    if (this.m_InterpolateMovement == 0f)
                    {
                        this.m_RigidBody2D.position = this.m_TargetSyncPosition;
                    }
                    if (initialState)
                    {
                        this.m_RigidBody2D.rotation = this.m_TargetSyncRotation2D;
                    }
                }
            }
        }

        private void UnserializeMode3D(NetworkReader reader, bool initialState)
        {
            if (base.hasAuthority)
            {
                Vector3 position = reader.ReadVector3();
                Vector3 vector2 = reader.ReadVector3();
                Quaternion identity = Quaternion.identity;
                if (this.syncRotationAxis != AxisSyncMode.None)
                {
                    identity = UnserializeRotation3D(reader, this.syncRotationAxis, this.rotationSyncCompression);
                }
                if (this.syncSpin)
                {
                    UnserializeSpin3D(reader, this.syncRotationAxis, this.rotationSyncCompression);
                }
                Vector3 vector5 = position - this.m_RigidBody3D.position;
                if (Mathf.Abs(vector5.magnitude) > this.m_SnapThreshold)
                {
                    this.m_RigidBody3D.MovePosition(position);
                    this.m_RigidBody3D.MoveRotation(identity);
                    this.m_RigidBody3D.velocity = vector2;
                }
            }
            else if (this.m_RigidBody3D != null)
            {
                if (base.isServer && (this.m_ClientMoveCallback3D != null))
                {
                    Vector3 vector3 = reader.ReadVector3();
                    Vector3 velocity = reader.ReadVector3();
                    Quaternion rotation = Quaternion.identity;
                    if (this.syncRotationAxis != AxisSyncMode.None)
                    {
                        rotation = UnserializeRotation3D(reader, this.syncRotationAxis, this.rotationSyncCompression);
                    }
                    if (!this.m_ClientMoveCallback3D(ref vector3, ref velocity, ref rotation))
                    {
                        return;
                    }
                    this.m_TargetSyncPosition = vector3;
                    this.m_TargetSyncVelocity = velocity;
                    if (this.syncRotationAxis != AxisSyncMode.None)
                    {
                        this.m_TargetSyncRotation3D = rotation;
                    }
                }
                else
                {
                    this.m_TargetSyncPosition = reader.ReadVector3();
                    this.m_TargetSyncVelocity = reader.ReadVector3();
                    if (this.syncRotationAxis != AxisSyncMode.None)
                    {
                        this.m_TargetSyncRotation3D = UnserializeRotation3D(reader, this.syncRotationAxis, this.rotationSyncCompression);
                    }
                }
                if (this.syncSpin)
                {
                    this.m_TargetSyncAngularVelocity3D = UnserializeSpin3D(reader, this.syncRotationAxis, this.rotationSyncCompression);
                }
                if (base.isServer && !base.isClient)
                {
                    this.m_RigidBody3D.MovePosition(this.m_TargetSyncPosition);
                    this.m_RigidBody3D.MoveRotation(this.m_TargetSyncRotation3D);
                    this.m_RigidBody3D.velocity = this.m_TargetSyncVelocity;
                }
                else if (this.GetNetworkSendInterval() == 0f)
                {
                    this.m_RigidBody3D.MovePosition(this.m_TargetSyncPosition);
                    this.m_RigidBody3D.velocity = this.m_TargetSyncVelocity;
                    if (this.syncRotationAxis != AxisSyncMode.None)
                    {
                        this.m_RigidBody3D.MoveRotation(this.m_TargetSyncRotation3D);
                    }
                    if (this.syncSpin)
                    {
                        this.m_RigidBody3D.angularVelocity = this.m_TargetSyncAngularVelocity3D;
                    }
                }
                else
                {
                    Vector3 vector6 = this.m_RigidBody3D.position - this.m_TargetSyncPosition;
                    if (vector6.magnitude > this.snapThreshold)
                    {
                        this.m_RigidBody3D.position = this.m_TargetSyncPosition;
                        this.m_RigidBody3D.velocity = this.m_TargetSyncVelocity;
                    }
                    if ((this.interpolateRotation == 0f) && (this.syncRotationAxis != AxisSyncMode.None))
                    {
                        this.m_RigidBody3D.rotation = this.m_TargetSyncRotation3D;
                        if (this.syncSpin)
                        {
                            this.m_RigidBody3D.angularVelocity = this.m_TargetSyncAngularVelocity3D;
                        }
                    }
                    if (this.m_InterpolateMovement == 0f)
                    {
                        this.m_RigidBody3D.position = this.m_TargetSyncPosition;
                    }
                    if (initialState && (this.syncRotationAxis != AxisSyncMode.None))
                    {
                        this.m_RigidBody3D.rotation = this.m_TargetSyncRotation3D;
                    }
                }
            }
        }

        private void UnserializeModeCharacterController(NetworkReader reader, bool initialState)
        {
            if (base.hasAuthority)
            {
                Vector3 vector = reader.ReadVector3();
                Quaternion identity = Quaternion.identity;
                if (this.syncRotationAxis != AxisSyncMode.None)
                {
                    identity = UnserializeRotation3D(reader, this.syncRotationAxis, this.rotationSyncCompression);
                }
                Vector3 vector4 = vector - base.transform.position;
                if (Mathf.Abs(vector4.magnitude) > this.m_SnapThreshold)
                {
                    base.transform.position = vector;
                    base.transform.rotation = identity;
                }
            }
            else if (this.m_CharacterController != null)
            {
                this.m_TargetSyncPosition = reader.ReadVector3();
                Vector3 vector2 = this.m_TargetSyncPosition - base.transform.position;
                Vector3 vector3 = (Vector3) (vector2 / this.GetNetworkSendInterval());
                this.m_FixedPosDiff = (Vector3) (vector3 * Time.fixedDeltaTime);
                if (this.syncRotationAxis != AxisSyncMode.None)
                {
                    this.m_TargetSyncRotation3D = UnserializeRotation3D(reader, this.syncRotationAxis, this.rotationSyncCompression);
                }
                if (base.isServer && !base.isClient)
                {
                    base.transform.position = this.m_TargetSyncPosition;
                    base.transform.rotation = this.m_TargetSyncRotation3D;
                }
                else if (this.GetNetworkSendInterval() == 0f)
                {
                    base.transform.position = this.m_TargetSyncPosition;
                    if (this.syncRotationAxis != AxisSyncMode.None)
                    {
                        base.transform.rotation = this.m_TargetSyncRotation3D;
                    }
                }
                else
                {
                    Vector3 vector5 = base.transform.position - this.m_TargetSyncPosition;
                    if (vector5.magnitude > this.snapThreshold)
                    {
                        base.transform.position = this.m_TargetSyncPosition;
                    }
                    if ((this.interpolateRotation == 0f) && (this.syncRotationAxis != AxisSyncMode.None))
                    {
                        base.transform.rotation = this.m_TargetSyncRotation3D;
                    }
                    if (this.m_InterpolateMovement == 0f)
                    {
                        base.transform.position = this.m_TargetSyncPosition;
                    }
                    if (initialState && (this.syncRotationAxis != AxisSyncMode.None))
                    {
                        base.transform.rotation = this.m_TargetSyncRotation3D;
                    }
                }
            }
        }

        private void UnserializeModeTransform(NetworkReader reader, bool initialState)
        {
            if (base.hasAuthority)
            {
                Vector3 vector = reader.ReadVector3();
                Quaternion identity = Quaternion.identity;
                if (this.syncRotationAxis != AxisSyncMode.None)
                {
                    identity = UnserializeRotation3D(reader, this.syncRotationAxis, this.rotationSyncCompression);
                }
                Vector3 vector4 = vector - base.transform.position;
                if (Mathf.Abs(vector4.magnitude) > this.m_SnapThreshold)
                {
                    base.transform.position = vector;
                    base.transform.rotation = identity;
                }
            }
            else if (base.isServer && (this.m_ClientMoveCallback3D != null))
            {
                Vector3 position = reader.ReadVector3();
                Vector3 zero = Vector3.zero;
                Quaternion rotation = Quaternion.identity;
                if (this.syncRotationAxis != AxisSyncMode.None)
                {
                    rotation = UnserializeRotation3D(reader, this.syncRotationAxis, this.rotationSyncCompression);
                }
                if (this.m_ClientMoveCallback3D(ref position, ref zero, ref rotation))
                {
                    base.transform.position = position;
                    if (this.syncRotationAxis != AxisSyncMode.None)
                    {
                        base.transform.rotation = rotation;
                    }
                }
            }
            else
            {
                base.transform.position = reader.ReadVector3();
                if (this.syncRotationAxis != AxisSyncMode.None)
                {
                    base.transform.rotation = UnserializeRotation3D(reader, this.syncRotationAxis, this.rotationSyncCompression);
                }
            }
        }

        public static float UnserializeRotation2D(NetworkReader reader, CompressionSyncMode compression)
        {
            return ReadAngle(reader, compression);
        }

        public static Quaternion UnserializeRotation3D(NetworkReader reader, AxisSyncMode mode, CompressionSyncMode compression)
        {
            Quaternion identity = Quaternion.identity;
            Vector3 zero = Vector3.zero;
            switch (mode)
            {
                case AxisSyncMode.None:
                    return identity;

                case AxisSyncMode.AxisX:
                    zero.Set(ReadAngle(reader, compression), 0f, 0f);
                    identity.eulerAngles = zero;
                    return identity;

                case AxisSyncMode.AxisY:
                    zero.Set(0f, ReadAngle(reader, compression), 0f);
                    identity.eulerAngles = zero;
                    return identity;

                case AxisSyncMode.AxisZ:
                    zero.Set(0f, 0f, ReadAngle(reader, compression));
                    identity.eulerAngles = zero;
                    return identity;

                case AxisSyncMode.AxisXY:
                    zero.Set(ReadAngle(reader, compression), ReadAngle(reader, compression), 0f);
                    identity.eulerAngles = zero;
                    return identity;

                case AxisSyncMode.AxisXZ:
                    zero.Set(ReadAngle(reader, compression), 0f, ReadAngle(reader, compression));
                    identity.eulerAngles = zero;
                    return identity;

                case AxisSyncMode.AxisYZ:
                    zero.Set(0f, ReadAngle(reader, compression), ReadAngle(reader, compression));
                    identity.eulerAngles = zero;
                    return identity;

                case AxisSyncMode.AxisXYZ:
                    zero.Set(ReadAngle(reader, compression), ReadAngle(reader, compression), ReadAngle(reader, compression));
                    identity.eulerAngles = zero;
                    return identity;
            }
            return identity;
        }

        public static float UnserializeSpin2D(NetworkReader reader, CompressionSyncMode compression)
        {
            return ReadAngle(reader, compression);
        }

        public static Vector3 UnserializeSpin3D(NetworkReader reader, AxisSyncMode mode, CompressionSyncMode compression)
        {
            Vector3 zero = Vector3.zero;
            switch (mode)
            {
                case AxisSyncMode.None:
                    return zero;

                case AxisSyncMode.AxisX:
                    zero.Set(ReadAngle(reader, compression), 0f, 0f);
                    return zero;

                case AxisSyncMode.AxisY:
                    zero.Set(0f, ReadAngle(reader, compression), 0f);
                    return zero;

                case AxisSyncMode.AxisZ:
                    zero.Set(0f, 0f, ReadAngle(reader, compression));
                    return zero;

                case AxisSyncMode.AxisXY:
                    zero.Set(ReadAngle(reader, compression), ReadAngle(reader, compression), 0f);
                    return zero;

                case AxisSyncMode.AxisXZ:
                    zero.Set(ReadAngle(reader, compression), 0f, ReadAngle(reader, compression));
                    return zero;

                case AxisSyncMode.AxisYZ:
                    zero.Set(0f, ReadAngle(reader, compression), ReadAngle(reader, compression));
                    return zero;

                case AxisSyncMode.AxisXYZ:
                    zero.Set(ReadAngle(reader, compression), ReadAngle(reader, compression), ReadAngle(reader, compression));
                    return zero;
            }
            return zero;
        }

        public static Vector3 UnserializeVelocity2D(NetworkReader reader, CompressionSyncMode compression)
        {
            return (Vector3) reader.ReadVector2();
        }

        public static Vector3 UnserializeVelocity3D(NetworkReader reader, CompressionSyncMode compression)
        {
            return reader.ReadVector3();
        }

        private void Update()
        {
            if (((base.hasAuthority && base.localPlayerAuthority) && !NetworkServer.active) && ((Time.time - this.m_LastClientSendTime) > this.GetNetworkSendInterval()))
            {
                this.SendTransform();
                this.m_LastClientSendTime = Time.time;
            }
        }

        private static void WriteAngle(NetworkWriter writer, float angle, CompressionSyncMode compression)
        {
            switch (compression)
            {
                case CompressionSyncMode.None:
                    writer.Write(angle);
                    break;

                case CompressionSyncMode.Low:
                    writer.Write((short) angle);
                    break;

                case CompressionSyncMode.High:
                    writer.Write((short) angle);
                    break;
            }
        }

        public CharacterController characterContoller
        {
            get
            {
                return this.m_CharacterController;
            }
        }

        public ClientMoveCallback2D clientMoveCallback2D
        {
            get
            {
                return this.m_ClientMoveCallback2D;
            }
            set
            {
                this.m_ClientMoveCallback2D = value;
            }
        }

        public ClientMoveCallback3D clientMoveCallback3D
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

        public bool grounded
        {
            get
            {
                return this.m_Grounded;
            }
            set
            {
                this.m_Grounded = value;
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

        public float movementTheshold
        {
            get
            {
                return this.m_MovementTheshold;
            }
            set
            {
                this.m_MovementTheshold = value;
            }
        }

        public Rigidbody2D rigidbody2D
        {
            get
            {
                return this.m_RigidBody2D;
            }
        }

        public Rigidbody rigidbody3D
        {
            get
            {
                return this.m_RigidBody3D;
            }
        }

        public CompressionSyncMode rotationSyncCompression
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

        public float snapThreshold
        {
            get
            {
                return this.m_SnapThreshold;
            }
            set
            {
                this.m_SnapThreshold = value;
            }
        }

        public AxisSyncMode syncRotationAxis
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

        public bool syncSpin
        {
            get
            {
                return this.m_SyncSpin;
            }
            set
            {
                this.m_SyncSpin = value;
            }
        }

        public Vector3 targetSyncPosition
        {
            get
            {
                return this.m_TargetSyncPosition;
            }
        }

        public float targetSyncRotation2D
        {
            get
            {
                return this.m_TargetSyncRotation2D;
            }
        }

        public Quaternion targetSyncRotation3D
        {
            get
            {
                return this.m_TargetSyncRotation3D;
            }
        }

        public Vector3 targetSyncVelocity
        {
            get
            {
                return this.m_TargetSyncVelocity;
            }
        }

        public TransformSyncMode transformSyncMode
        {
            get
            {
                return this.m_TransformSyncMode;
            }
            set
            {
                this.m_TransformSyncMode = value;
            }
        }

        public enum AxisSyncMode
        {
            None,
            AxisX,
            AxisY,
            AxisZ,
            AxisXY,
            AxisXZ,
            AxisYZ,
            AxisXYZ
        }

        public delegate bool ClientMoveCallback2D(ref Vector2 position, ref Vector2 velocity, ref float rotation);

        public delegate bool ClientMoveCallback3D(ref Vector3 position, ref Vector3 velocity, ref Quaternion rotation);

        public enum CompressionSyncMode
        {
            None,
            Low,
            High
        }

        public enum TransformSyncMode
        {
            SyncNone,
            SyncTransform,
            SyncRigidbody2D,
            SyncRigidbody3D,
            SyncCharacterController
        }
    }
}

