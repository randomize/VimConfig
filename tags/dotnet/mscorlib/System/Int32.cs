namespace System
{
    using System.Globalization;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential), ComVisible(true)]
    public struct Int32 : IComparable, IFormattable, IConvertible, IComparable<int>, IEquatable<int>
    {
        public const int MaxValue = 0x7fffffff;
        public const int MinValue = -2147483648;
        internal int m_value;
        public int CompareTo(object value)
        {
            if (value == null)
            {
                return 1;
            }
            if (!(value is int))
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_MustBeInt32"));
            }
            int num = (int) value;
            if (this < num)
            {
                return -1;
            }
            if (this > num)
            {
                return 1;
            }
            return 0;
        }

        public int CompareTo(int value)
        {
            if (this < value)
            {
                return -1;
            }
            if (this > value)
            {
                return 1;
            }
            return 0;
        }

        public override bool Equals(object obj)
        {
            return ((obj is int) && (this == ((int) obj)));
        }

        public bool Equals(int obj)
        {
            return (this == obj);
        }

        public override int GetHashCode()
        {
            return this;
        }

        public override string ToString()
        {
            return Number.FormatInt32(this, null, NumberFormatInfo.CurrentInfo);
        }

        public string ToString(string format)
        {
            return Number.FormatInt32(this, format, NumberFormatInfo.CurrentInfo);
        }

        public string ToString(IFormatProvider provider)
        {
            return Number.FormatInt32(this, null, NumberFormatInfo.GetInstance(provider));
        }

        public string ToString(string format, IFormatProvider provider)
        {
            return Number.FormatInt32(this, format, NumberFormatInfo.GetInstance(provider));
        }

        public static int Parse(string s)
        {
            return Number.ParseInt32(s, NumberStyles.Integer, NumberFormatInfo.CurrentInfo);
        }

        public static int Parse(string s, NumberStyles style)
        {
            NumberFormatInfo.ValidateParseStyleInteger(style);
            return Number.ParseInt32(s, style, NumberFormatInfo.CurrentInfo);
        }

        public static int Parse(string s, IFormatProvider provider)
        {
            return Number.ParseInt32(s, NumberStyles.Integer, NumberFormatInfo.GetInstance(provider));
        }

        public static int Parse(string s, NumberStyles style, IFormatProvider provider)
        {
            NumberFormatInfo.ValidateParseStyleInteger(style);
            return Number.ParseInt32(s, style, NumberFormatInfo.GetInstance(provider));
        }

        public static bool TryParse(string s, out int result)
        {
            return Number.TryParseInt32(s, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out result);
        }

        public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out int result)
        {
            NumberFormatInfo.ValidateParseStyleInteger(style);
            return Number.TryParseInt32(s, style, NumberFormatInfo.GetInstance(provider), out result);
        }

        public TypeCode GetTypeCode()
        {
            return TypeCode.Int32;
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return Convert.ToBoolean(this);
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(this);
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(this);
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(this);
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(this);
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(this);
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return this;
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(this);
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt64(this);
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(this);
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(this);
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(this);
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return Convert.ToDecimal(this);
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            throw new InvalidCastException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("InvalidCast_FromTo"), new object[] { "Int32", "DateTime" }));
        }

        object IConvertible.ToType(Type type, IFormatProvider provider)
        {
            return Convert.DefaultToType(this, type, provider);
        }
    }
}

