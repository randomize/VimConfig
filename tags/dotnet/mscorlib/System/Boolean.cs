namespace System
{
    using System.Globalization;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential), ComVisible(true)]
    public struct Boolean : IComparable, IConvertible, IComparable<bool>, IEquatable<bool>
    {
        internal const int True = 1;
        internal const int False = 0;
        internal const string TrueLiteral = "True";
        internal const string FalseLiteral = "False";
        private bool m_value;
        private static char[] m_trimmableChars;
        public static readonly string TrueString;
        public static readonly string FalseString;
        public override int GetHashCode()
        {
            if (!this)
            {
                return 0;
            }
            return 1;
        }

        public override string ToString()
        {
            if (!this)
            {
                return "False";
            }
            return "True";
        }

        public string ToString(IFormatProvider provider)
        {
            if (!this)
            {
                return "False";
            }
            return "True";
        }

        public override bool Equals(object obj)
        {
            return ((obj is bool) && (this == ((bool) obj)));
        }

        public bool Equals(bool obj)
        {
            return (this == obj);
        }

        public int CompareTo(object obj)
        {
            if (obj != null)
            {
                if (!(obj is bool))
                {
                    throw new ArgumentException(Environment.GetResourceString("Arg_MustBeBoolean"));
                }
                if (this == ((bool) obj))
                {
                    return 0;
                }
                if (!this)
                {
                    return -1;
                }
            }
            return 1;
        }

        public int CompareTo(bool value)
        {
            if (this == value)
            {
                return 0;
            }
            if (!this)
            {
                return -1;
            }
            return 1;
        }

        public static bool Parse(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            bool result = false;
            if (!TryParse(value, out result))
            {
                throw new FormatException(Environment.GetResourceString("Format_BadBoolean"));
            }
            return result;
        }

        public static bool TryParse(string value, out bool result)
        {
            result = false;
            if (value != null)
            {
                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    result = true;
                    return true;
                }
                if ("False".Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    result = false;
                    return true;
                }
                if (m_trimmableChars == null)
                {
                    char[] destinationArray = new char[string.WhitespaceChars.Length + 1];
                    Array.Copy(string.WhitespaceChars, destinationArray, string.WhitespaceChars.Length);
                    destinationArray[destinationArray.Length - 1] = '\0';
                    m_trimmableChars = destinationArray;
                }
                value = value.Trim(m_trimmableChars);
                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    result = true;
                    return true;
                }
                if ("False".Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    result = false;
                    return true;
                }
            }
            return false;
        }

        public TypeCode GetTypeCode()
        {
            return TypeCode.Boolean;
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return this;
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            throw new InvalidCastException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("InvalidCast_FromTo"), new object[] { "Boolean", "Char" }));
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
            return Convert.ToInt32(this);
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
            throw new InvalidCastException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("InvalidCast_FromTo"), new object[] { "Boolean", "DateTime" }));
        }

        object IConvertible.ToType(Type type, IFormatProvider provider)
        {
            return Convert.DefaultToType(this, type, provider);
        }

        static Boolean()
        {
            TrueString = "True";
            FalseString = "False";
        }
    }
}

