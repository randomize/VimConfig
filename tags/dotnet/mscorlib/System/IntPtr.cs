namespace System
{
    using System.Globalization;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;

    [Serializable, StructLayout(LayoutKind.Sequential), ComVisible(true)]
    public struct IntPtr : ISerializable
    {
        private unsafe void* m_value;
        public static readonly IntPtr Zero;
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        internal unsafe bool IsNull()
        {
            return (this.m_value == null);
        }

        [ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
        public unsafe IntPtr(int value)
        {
            this.m_value = (void*) value;
        }

        [ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
        public unsafe IntPtr(long value)
        {
            this.m_value = (void*) ((int) value);
        }

        [CLSCompliant(false), ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
        public unsafe IntPtr(void* value)
        {
            this.m_value = value;
        }

        private unsafe IntPtr(SerializationInfo info, StreamingContext context)
        {
            long num = info.GetInt64("value");
            if ((Size == 4) && ((num > 0x7fffffffL) || (num < -2147483648L)))
            {
                throw new ArgumentException(Environment.GetResourceString("Serialization_InvalidPtrValue"));
            }
            this.m_value = (void*) num;
        }

        unsafe void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            info.AddValue("value", (long) ((int) this.m_value));
        }

        public override unsafe bool Equals(object obj)
        {
            if (obj is IntPtr)
            {
                IntPtr ptr = (IntPtr) obj;
                return (this.m_value == ptr.m_value);
            }
            return false;
        }

        public override unsafe int GetHashCode()
        {
            return (int) ((ulong) this.m_value);
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public unsafe int ToInt32()
        {
            return (int) this.m_value;
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public unsafe long ToInt64()
        {
            return (long) ((int) this.m_value);
        }

        public override unsafe string ToString()
        {
            int num = (int) this.m_value;
            return num.ToString(CultureInfo.InvariantCulture);
        }

        public unsafe string ToString(string format)
        {
            int num = (int) this.m_value;
            return num.ToString(format, CultureInfo.InvariantCulture);
        }

        [ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
        public static explicit operator IntPtr(int value)
        {
            return new IntPtr(value);
        }

        [ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
        public static explicit operator IntPtr(long value)
        {
            return new IntPtr(value);
        }

        [ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail), CLSCompliant(false)]
        public static unsafe explicit operator IntPtr(void* value)
        {
            return new IntPtr(value);
        }

        [CLSCompliant(false)]
        public static unsafe explicit operator void*(IntPtr value)
        {
            return value.ToPointer();
        }

        public static unsafe explicit operator int(IntPtr value)
        {
            return (int) value.m_value;
        }

        public static unsafe explicit operator long(IntPtr value)
        {
            return (long) ((int) value.m_value);
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static unsafe bool operator ==(IntPtr value1, IntPtr value2)
        {
            return (value1.m_value == value2.m_value);
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static unsafe bool operator !=(IntPtr value1, IntPtr value2)
        {
            return (value1.m_value != value2.m_value);
        }

        public static int Size
        {
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            get
            {
                return 4;
            }
        }
        [CLSCompliant(false), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public unsafe void* ToPointer()
        {
            return this.m_value;
        }
    }
}

