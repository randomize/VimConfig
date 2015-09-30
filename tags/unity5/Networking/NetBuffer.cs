namespace UnityEngine.Networking
{
    using System;
    using UnityEngine;

    internal class NetBuffer
    {
        private const int k_BufferSizeWarning = 0x8000000;
        private const float k_GrowthFactor = 1.5f;
        private const int k_InitialSize = 0x40;
        private byte[] m_Buffer;
        private uint m_Pos;

        public NetBuffer()
        {
            this.m_Buffer = new byte[0x40];
        }

        public NetBuffer(byte[] buffer)
        {
            this.m_Buffer = buffer;
        }

        internal ArraySegment<byte> AsArraySegment()
        {
            return new ArraySegment<byte>(this.m_Buffer, 0, (int) this.m_Pos);
        }

        public void FinishMessage()
        {
            ushort num = (ushort) (this.m_Pos - 4);
            this.m_Buffer[0] = (byte) (num & 0xff);
            this.m_Buffer[1] = (byte) ((num >> 8) & 0xff);
        }

        public byte ReadByte()
        {
            if (this.m_Pos >= this.m_Buffer.Length)
            {
                throw new IndexOutOfRangeException("NetworkReader:ReadByte out of range:" + this.ToString());
            }
            return this.m_Buffer[this.m_Pos++];
        }

        public void ReadBytes(byte[] buffer, uint count)
        {
            if ((this.m_Pos + count) > this.m_Buffer.Length)
            {
                object[] objArray1 = new object[] { "NetworkReader:ReadBytes out of range: (", count, ") ", this.ToString() };
                throw new IndexOutOfRangeException(string.Concat(objArray1));
            }
            for (ushort i = 0; i < count; i = (ushort) (i + 1))
            {
                buffer[i] = this.m_Buffer[this.m_Pos + i];
            }
            this.m_Pos += count;
        }

        public void ReadChars(char[] buffer, uint count)
        {
            if ((this.m_Pos + count) > this.m_Buffer.Length)
            {
                object[] objArray1 = new object[] { "NetworkReader:ReadChars out of range: (", count, ") ", this.ToString() };
                throw new IndexOutOfRangeException(string.Concat(objArray1));
            }
            for (ushort i = 0; i < count; i = (ushort) (i + 1))
            {
                buffer[i] = (char) this.m_Buffer[this.m_Pos + i];
            }
            this.m_Pos += count;
        }

        public void Replace(byte[] buffer)
        {
            this.m_Buffer = buffer;
            this.m_Pos = 0;
        }

        public void SeekZero()
        {
            this.m_Pos = 0;
        }

        public override string ToString()
        {
            return string.Format("NetBuf sz:{0} pos:{1}", this.m_Buffer.Length, this.m_Pos);
        }

        public void WriteByte(byte value)
        {
            this.WriteCheckForSpace(1);
            this.m_Buffer[this.m_Pos] = value;
            this.m_Pos++;
        }

        public void WriteByte2(byte value0, byte value1)
        {
            this.WriteCheckForSpace(2);
            this.m_Buffer[this.m_Pos] = value0;
            this.m_Buffer[(int) ((IntPtr) (this.m_Pos + 1))] = value1;
            this.m_Pos += 2;
        }

        public void WriteByte4(byte value0, byte value1, byte value2, byte value3)
        {
            this.WriteCheckForSpace(4);
            this.m_Buffer[this.m_Pos] = value0;
            this.m_Buffer[(int) ((IntPtr) (this.m_Pos + 1))] = value1;
            this.m_Buffer[(int) ((IntPtr) (this.m_Pos + 2))] = value2;
            this.m_Buffer[(int) ((IntPtr) (this.m_Pos + 3))] = value3;
            this.m_Pos += 4;
        }

        public void WriteByte8(byte value0, byte value1, byte value2, byte value3, byte value4, byte value5, byte value6, byte value7)
        {
            this.WriteCheckForSpace(8);
            this.m_Buffer[this.m_Pos] = value0;
            this.m_Buffer[(int) ((IntPtr) (this.m_Pos + 1))] = value1;
            this.m_Buffer[(int) ((IntPtr) (this.m_Pos + 2))] = value2;
            this.m_Buffer[(int) ((IntPtr) (this.m_Pos + 3))] = value3;
            this.m_Buffer[(int) ((IntPtr) (this.m_Pos + 4))] = value4;
            this.m_Buffer[(int) ((IntPtr) (this.m_Pos + 5))] = value5;
            this.m_Buffer[(int) ((IntPtr) (this.m_Pos + 6))] = value6;
            this.m_Buffer[(int) ((IntPtr) (this.m_Pos + 7))] = value7;
            this.m_Pos += 8;
        }

        public void WriteBytes(byte[] buffer, ushort count)
        {
            this.WriteCheckForSpace(count);
            if (count == buffer.Length)
            {
                buffer.CopyTo(this.m_Buffer, (long) this.m_Pos);
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    this.m_Buffer[(int) ((IntPtr) (this.m_Pos + i))] = buffer[i];
                }
            }
            this.m_Pos += count;
        }

        public void WriteBytesAtOffset(byte[] buffer, ushort targetOffset, ushort count)
        {
            uint num = (uint) (count + targetOffset);
            this.WriteCheckForSpace((ushort) num);
            if ((targetOffset == 0) && (count == buffer.Length))
            {
                buffer.CopyTo(this.m_Buffer, (long) this.m_Pos);
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    this.m_Buffer[targetOffset + i] = buffer[i];
                }
            }
            if (num > this.m_Pos)
            {
                this.m_Pos = num;
            }
        }

        private void WriteCheckForSpace(ushort count)
        {
            if ((this.m_Pos + count) >= this.m_Buffer.Length)
            {
                int num = (int) (this.m_Buffer.Length * 1.5f);
                while ((this.m_Pos + count) >= num)
                {
                    num = (int) (num * 1.5f);
                    if (num > 0x8000000)
                    {
                        Debug.LogWarning("NetworkBuffer size is " + num + " bytes!");
                    }
                }
                byte[] array = new byte[num];
                this.m_Buffer.CopyTo(array, 0);
                this.m_Buffer = array;
            }
        }

        public uint Position
        {
            get
            {
                return this.m_Pos;
            }
        }
    }
}

