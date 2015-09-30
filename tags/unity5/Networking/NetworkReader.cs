namespace UnityEngine.Networking
{
    using System;
    using System.Text;
    using UnityEngine;

    public class NetworkReader
    {
        private const int k_InitialStringBufferSize = 0x400;
        private const int k_MaxStringLength = 0x8000;
        private NetBuffer m_buf;
        private static Encoding s_Encoding;
        private static byte[] s_StringReaderBuffer;

        public NetworkReader()
        {
            this.m_buf = new NetBuffer();
            Initialize();
        }

        public NetworkReader(NetworkWriter writer)
        {
            this.m_buf = new NetBuffer(writer.AsArray());
            Initialize();
        }

        public NetworkReader(byte[] buffer)
        {
            this.m_buf = new NetBuffer(buffer);
            Initialize();
        }

        private static void Initialize()
        {
            if (s_Encoding == null)
            {
                s_StringReaderBuffer = new byte[0x400];
                s_Encoding = new UTF8Encoding();
            }
        }

        public bool ReadBoolean()
        {
            return (this.m_buf.ReadByte() == 1);
        }

        public byte ReadByte()
        {
            return this.m_buf.ReadByte();
        }

        public byte[] ReadBytes(int count)
        {
            if (count < 0)
            {
                throw new IndexOutOfRangeException("NetworkReader ReadBytes " + count);
            }
            byte[] buffer = new byte[count];
            this.m_buf.ReadBytes(buffer, (uint) count);
            return buffer;
        }

        public byte[] ReadBytesAndSize()
        {
            ushort count = this.ReadUInt16();
            if (count == 0)
            {
                return null;
            }
            return this.ReadBytes(count);
        }

        public char ReadChar()
        {
            return (char) this.m_buf.ReadByte();
        }

        public Color ReadColor()
        {
            return new Color(this.ReadSingle(), this.ReadSingle(), this.ReadSingle(), this.ReadSingle());
        }

        public Color32 ReadColor32()
        {
            return new Color32(this.ReadByte(), this.ReadByte(), this.ReadByte(), this.ReadByte());
        }

        public double ReadDouble()
        {
            return FloatConversion.ToDouble(this.ReadUInt64());
        }

        public GameObject ReadGameObject()
        {
            GameObject obj2;
            NetworkInstanceId netId = this.ReadNetworkId();
            if (netId.IsEmpty())
            {
                return null;
            }
            if (NetworkServer.active)
            {
                obj2 = NetworkServer.FindLocalObject(netId);
            }
            else
            {
                obj2 = ClientScene.FindLocalObject(netId);
            }
            if ((obj2 == null) && LogFilter.logDebug)
            {
                Debug.Log("ReadGameObject netId:" + netId + "go: null");
            }
            return obj2;
        }

        public short ReadInt16()
        {
            ushort num = 0;
            num = (ushort) (num | this.m_buf.ReadByte());
            num = (ushort) (num | ((ushort) (this.m_buf.ReadByte() << 8)));
            return (short) num;
        }

        public int ReadInt32()
        {
            uint num = 0;
            num |= this.m_buf.ReadByte();
            num |= (uint) (this.m_buf.ReadByte() << 8);
            num |= (uint) (this.m_buf.ReadByte() << 0x10);
            num |= (uint) (this.m_buf.ReadByte() << 0x18);
            return (int) num;
        }

        public long ReadInt64()
        {
            ulong num = 0L;
            ulong num2 = this.m_buf.ReadByte();
            num |= num2;
            num2 = (ulong) (this.m_buf.ReadByte() << 8);
            num |= num2;
            num2 = (ulong) (this.m_buf.ReadByte() << 0x10);
            num |= num2;
            num2 = (ulong) (this.m_buf.ReadByte() << 0x18);
            num |= num2;
            num2 = (ulong) (this.m_buf.ReadByte() << 0x20);
            num |= num2;
            num2 = (ulong) (this.m_buf.ReadByte() << 40);
            num |= num2;
            num2 = (ulong) (this.m_buf.ReadByte() << 0x30);
            num |= num2;
            num2 = (ulong) (this.m_buf.ReadByte() << 0x38);
            num |= num2;
            return (long) num;
        }

        public Matrix4x4 ReadMatrix4x4()
        {
            return new Matrix4x4 { m00 = this.ReadSingle(), m01 = this.ReadSingle(), m02 = this.ReadSingle(), m03 = this.ReadSingle(), m10 = this.ReadSingle(), m11 = this.ReadSingle(), m12 = this.ReadSingle(), m13 = this.ReadSingle(), m20 = this.ReadSingle(), m21 = this.ReadSingle(), m22 = this.ReadSingle(), m23 = this.ReadSingle(), m30 = this.ReadSingle(), m31 = this.ReadSingle(), m32 = this.ReadSingle(), m33 = this.ReadSingle() };
        }

        public TMsg ReadMessage<TMsg>() where TMsg: MessageBase, new()
        {
            TMsg local = Activator.CreateInstance<TMsg>();
            local.Deserialize(this);
            return local;
        }

        public NetworkHash128 ReadNetworkHash128()
        {
            NetworkHash128 hash;
            hash.i0 = this.ReadByte();
            hash.i1 = this.ReadByte();
            hash.i2 = this.ReadByte();
            hash.i3 = this.ReadByte();
            hash.i4 = this.ReadByte();
            hash.i5 = this.ReadByte();
            hash.i6 = this.ReadByte();
            hash.i7 = this.ReadByte();
            hash.i8 = this.ReadByte();
            hash.i9 = this.ReadByte();
            hash.i10 = this.ReadByte();
            hash.i11 = this.ReadByte();
            hash.i12 = this.ReadByte();
            hash.i13 = this.ReadByte();
            hash.i14 = this.ReadByte();
            hash.i15 = this.ReadByte();
            return hash;
        }

        public NetworkInstanceId ReadNetworkId()
        {
            return new NetworkInstanceId(this.ReadPackedUInt32());
        }

        public NetworkIdentity ReadNetworkIdentity()
        {
            NetworkInstanceId netId = this.ReadNetworkId();
            if (!netId.IsEmpty())
            {
                GameObject obj2;
                if (NetworkServer.active)
                {
                    obj2 = NetworkServer.FindLocalObject(netId);
                }
                else
                {
                    obj2 = ClientScene.FindLocalObject(netId);
                }
                if (obj2 != null)
                {
                    return obj2.GetComponent<NetworkIdentity>();
                }
                if (LogFilter.logDebug)
                {
                    Debug.Log("ReadNetworkIdentity netId:" + netId + "go: null");
                }
            }
            return null;
        }

        public uint ReadPackedUInt32()
        {
            byte num = this.ReadByte();
            if (num < 0xf1)
            {
                return num;
            }
            byte num2 = this.ReadByte();
            if ((num >= 0xf1) && (num <= 0xf8))
            {
                return (uint) ((240 + (0x100 * (num - 0xf1))) + num2);
            }
            byte num3 = this.ReadByte();
            if (num == 0xf9)
            {
                return (uint) ((0x8f0 + (0x100 * num2)) + num3);
            }
            byte num4 = this.ReadByte();
            if (num == 250)
            {
                return (uint) ((num2 + (num3 << 8)) + (num4 << 0x10));
            }
            byte num5 = this.ReadByte();
            if (num < 0xfb)
            {
                throw new IndexOutOfRangeException("ReadPackedUInt32() failure: " + num);
            }
            return (uint) (((num2 + (num3 << 8)) + (num4 << 0x10)) + (num5 << 0x18));
        }

        public ulong ReadPackedUInt64()
        {
            byte num = this.ReadByte();
            if (num < 0xf1)
            {
                return (ulong) num;
            }
            byte num2 = this.ReadByte();
            if ((num >= 0xf1) && (num <= 0xf8))
            {
                return ((((ulong) 240L) + (((ulong) 0x100L) * (num - 0xf1L))) + num2);
            }
            byte num3 = this.ReadByte();
            if (num == 0xf9)
            {
                return ((((ulong) 0x8f0L) + (0x100L * num2)) + num3);
            }
            byte num4 = this.ReadByte();
            if (num == 250)
            {
                return (ulong) ((num2 + (num3 << 8)) + (num4 << 0x10));
            }
            byte num5 = this.ReadByte();
            if (num == 0xfb)
            {
                return (ulong) (((num2 + (num3 << 8)) + (num4 << 0x10)) + (num5 << 0x18));
            }
            byte num6 = this.ReadByte();
            if (num == 0xfc)
            {
                return (ulong) ((((num2 + (num3 << 8)) + (num4 << 0x10)) + (num5 << 0x18)) + (num6 << 0x20));
            }
            byte num7 = this.ReadByte();
            if (num == 0xfd)
            {
                return (ulong) (((((num2 + (num3 << 8)) + (num4 << 0x10)) + (num5 << 0x18)) + (num6 << 0x20)) + (num7 << 40));
            }
            byte num8 = this.ReadByte();
            if (num == 0xfe)
            {
                return (ulong) ((((((num2 + (num3 << 8)) + (num4 << 0x10)) + (num5 << 0x18)) + (num6 << 0x20)) + (num7 << 40)) + (num8 << 0x30));
            }
            byte num9 = this.ReadByte();
            if (num != 0xff)
            {
                throw new IndexOutOfRangeException("ReadPackedUInt64() failure: " + num);
            }
            return (ulong) (((((((num2 + (num3 << 8)) + (num4 << 0x10)) + (num5 << 0x18)) + (num6 << 0x20)) + (num7 << 40)) + (num8 << 0x30)) + (num9 << 0x38));
        }

        public Plane ReadPlane()
        {
            return new Plane(this.ReadVector3(), this.ReadSingle());
        }

        public Quaternion ReadQuaternion()
        {
            return new Quaternion(this.ReadSingle(), this.ReadSingle(), this.ReadSingle(), this.ReadSingle());
        }

        public Ray ReadRay()
        {
            return new Ray(this.ReadVector3(), this.ReadVector3());
        }

        public Rect ReadRect()
        {
            return new Rect(this.ReadSingle(), this.ReadSingle(), this.ReadSingle(), this.ReadSingle());
        }

        public sbyte ReadSByte()
        {
            return (sbyte) this.m_buf.ReadByte();
        }

        public NetworkSceneId ReadSceneId()
        {
            return new NetworkSceneId(this.ReadPackedUInt32());
        }

        public float ReadSingle()
        {
            return FloatConversion.ToSingle(this.ReadUInt32());
        }

        public string ReadString()
        {
            ushort count = this.ReadUInt16();
            if (count == 0)
            {
                return string.Empty;
            }
            if (count >= 0x8000)
            {
                throw new IndexOutOfRangeException("ReadString() too long: " + count);
            }
            while (count > s_StringReaderBuffer.Length)
            {
                s_StringReaderBuffer = new byte[s_StringReaderBuffer.Length * 2];
            }
            this.m_buf.ReadBytes(s_StringReaderBuffer, count);
            return new string(s_Encoding.GetChars(s_StringReaderBuffer, 0, count));
        }

        public Transform ReadTransform()
        {
            NetworkInstanceId netId = this.ReadNetworkId();
            if (!netId.IsEmpty())
            {
                GameObject obj2 = ClientScene.FindLocalObject(netId);
                if (obj2 != null)
                {
                    return obj2.transform;
                }
                if (LogFilter.logDebug)
                {
                    Debug.Log("ReadTransform netId:" + netId);
                }
            }
            return null;
        }

        public ushort ReadUInt16()
        {
            ushort num = 0;
            num = (ushort) (num | this.m_buf.ReadByte());
            return (ushort) (num | ((ushort) (this.m_buf.ReadByte() << 8)));
        }

        public uint ReadUInt32()
        {
            uint num = 0;
            num |= this.m_buf.ReadByte();
            num |= (uint) (this.m_buf.ReadByte() << 8);
            num |= (uint) (this.m_buf.ReadByte() << 0x10);
            return (num | ((uint) (this.m_buf.ReadByte() << 0x18)));
        }

        public ulong ReadUInt64()
        {
            ulong num = 0L;
            ulong num2 = this.m_buf.ReadByte();
            num |= num2;
            num2 = (ulong) (this.m_buf.ReadByte() << 8);
            num |= num2;
            num2 = (ulong) (this.m_buf.ReadByte() << 0x10);
            num |= num2;
            num2 = (ulong) (this.m_buf.ReadByte() << 0x18);
            num |= num2;
            num2 = (ulong) (this.m_buf.ReadByte() << 0x20);
            num |= num2;
            num2 = (ulong) (this.m_buf.ReadByte() << 40);
            num |= num2;
            num2 = (ulong) (this.m_buf.ReadByte() << 0x30);
            num |= num2;
            num2 = (ulong) (this.m_buf.ReadByte() << 0x38);
            return (num | num2);
        }

        public Vector2 ReadVector2()
        {
            return new Vector2(this.ReadSingle(), this.ReadSingle());
        }

        public Vector3 ReadVector3()
        {
            return new Vector3(this.ReadSingle(), this.ReadSingle(), this.ReadSingle());
        }

        public Vector4 ReadVector4()
        {
            return new Vector4(this.ReadSingle(), this.ReadSingle(), this.ReadSingle(), this.ReadSingle());
        }

        internal void Replace(byte[] buffer)
        {
            this.m_buf.Replace(buffer);
        }

        public void SeekZero()
        {
            this.m_buf.SeekZero();
        }

        public override string ToString()
        {
            return this.m_buf.ToString();
        }

        public uint Position
        {
            get
            {
                return this.m_buf.Position;
            }
        }
    }
}

