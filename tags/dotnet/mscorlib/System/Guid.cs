namespace System
{
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential), ComVisible(true)]
    public struct Guid : IFormattable, IComparable, IComparable<Guid>, IEquatable<Guid>
    {
        public static readonly Guid Empty;
        private int _a;
        private short _b;
        private short _c;
        private byte _d;
        private byte _e;
        private byte _f;
        private byte _g;
        private byte _h;
        private byte _i;
        private byte _j;
        private byte _k;
        public Guid(byte[] b)
        {
            if (b == null)
            {
                throw new ArgumentNullException("b");
            }
            if (b.Length != 0x10)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Arg_GuidArrayCtor"), new object[] { "16" }));
            }
            this._a = (((b[3] << 0x18) | (b[2] << 0x10)) | (b[1] << 8)) | b[0];
            this._b = (short) ((b[5] << 8) | b[4]);
            this._c = (short) ((b[7] << 8) | b[6]);
            this._d = b[8];
            this._e = b[9];
            this._f = b[10];
            this._g = b[11];
            this._h = b[12];
            this._i = b[13];
            this._j = b[14];
            this._k = b[15];
        }

        [CLSCompliant(false)]
        public Guid(uint a, ushort b, ushort c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k)
        {
            this._a = (int) a;
            this._b = (short) b;
            this._c = (short) c;
            this._d = d;
            this._e = e;
            this._f = f;
            this._g = g;
            this._h = h;
            this._i = i;
            this._j = j;
            this._k = k;
        }

        private Guid(bool blank)
        {
            this._a = 0;
            this._b = 0;
            this._c = 0;
            this._d = 0;
            this._e = 0;
            this._f = 0;
            this._g = 0;
            this._h = 0;
            this._i = 0;
            this._j = 0;
            this._k = 0;
            if (!blank)
            {
                this.CompleteGuid();
            }
        }

        public Guid(string g)
        {
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }
            int startIndex = 0;
            int parsePos = 0;
            try
            {
                int num2;
                long num3;
                if (g.IndexOf('-', 0) >= 0)
                {
                    string str = g.Trim();
                    if (str[0] == '{')
                    {
                        if ((str.Length != 0x26) || (str[0x25] != '}'))
                        {
                            throw new FormatException(Environment.GetResourceString("Format_GuidInvLen"));
                        }
                        startIndex = 1;
                    }
                    else if (str[0] == '(')
                    {
                        if ((str.Length != 0x26) || (str[0x25] != ')'))
                        {
                            throw new FormatException(Environment.GetResourceString("Format_GuidInvLen"));
                        }
                        startIndex = 1;
                    }
                    else if (str.Length != 0x24)
                    {
                        throw new FormatException(Environment.GetResourceString("Format_GuidInvLen"));
                    }
                    if (((str[8 + startIndex] != '-') || (str[13 + startIndex] != '-')) || ((str[0x12 + startIndex] != '-') || (str[0x17 + startIndex] != '-')))
                    {
                        throw new FormatException(Environment.GetResourceString("Format_GuidDashes"));
                    }
                    parsePos = startIndex;
                    this._a = TryParse(str, ref parsePos, 8);
                    parsePos++;
                    this._b = (short) TryParse(str, ref parsePos, 4);
                    parsePos++;
                    this._c = (short) TryParse(str, ref parsePos, 4);
                    parsePos++;
                    num2 = TryParse(str, ref parsePos, 4);
                    parsePos++;
                    startIndex = parsePos;
                    num3 = ParseNumbers.StringToLong(str, 0x10, 0x2000, ref parsePos);
                    if ((parsePos - startIndex) != 12)
                    {
                        throw new FormatException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Format_GuidInvLen"), new object[0]));
                    }
                    this._d = (byte) (num2 >> 8);
                    this._e = (byte) num2;
                    num2 = (int) (num3 >> 0x20);
                    this._f = (byte) (num2 >> 8);
                    this._g = (byte) num2;
                    num2 = (int) num3;
                    this._h = (byte) (num2 >> 0x18);
                    this._i = (byte) (num2 >> 0x10);
                    this._j = (byte) (num2 >> 8);
                    this._k = (byte) num2;
                }
                else if (g.IndexOf('{', 0) >= 0)
                {
                    int num5 = 0;
                    int length = 0;
                    g = EatAllWhitespace(g);
                    if (g[0] != '{')
                    {
                        throw new FormatException(Environment.GetResourceString("Format_GuidBrace"));
                    }
                    if (!IsHexPrefix(g, 1))
                    {
                        throw new FormatException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Format_GuidHexPrefix"), new object[] { "{0xdddddddd, etc}" }));
                    }
                    num5 = 3;
                    length = g.IndexOf(',', num5) - num5;
                    if (length <= 0)
                    {
                        throw new FormatException(Environment.GetResourceString("Format_GuidComma"));
                    }
                    this._a = ParseNumbers.StringToInt(g.Substring(num5, length), 0x10, 0x1000);
                    if (!IsHexPrefix(g, (num5 + length) + 1))
                    {
                        throw new FormatException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Format_GuidHexPrefix"), new object[] { "{0xdddddddd, 0xdddd, etc}" }));
                    }
                    num5 = (num5 + length) + 3;
                    length = g.IndexOf(',', num5) - num5;
                    if (length <= 0)
                    {
                        throw new FormatException(Environment.GetResourceString("Format_GuidComma"));
                    }
                    this._b = (short) ParseNumbers.StringToInt(g.Substring(num5, length), 0x10, 0x1000);
                    if (!IsHexPrefix(g, (num5 + length) + 1))
                    {
                        throw new FormatException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Format_GuidHexPrefix"), new object[] { "{0xdddddddd, 0xdddd, 0xdddd, etc}" }));
                    }
                    num5 = (num5 + length) + 3;
                    length = g.IndexOf(',', num5) - num5;
                    if (length <= 0)
                    {
                        throw new FormatException(Environment.GetResourceString("Format_GuidComma"));
                    }
                    this._c = (short) ParseNumbers.StringToInt(g.Substring(num5, length), 0x10, 0x1000);
                    if ((g.Length <= ((num5 + length) + 1)) || (g[(num5 + length) + 1] != '{'))
                    {
                        throw new FormatException(Environment.GetResourceString("Format_GuidBrace"));
                    }
                    length++;
                    byte[] buffer = new byte[8];
                    for (int i = 0; i < 8; i++)
                    {
                        if (!IsHexPrefix(g, (num5 + length) + 1))
                        {
                            throw new FormatException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Format_GuidHexPrefix"), new object[] { "{... { ... 0xdd, ...}}" }));
                        }
                        num5 = (num5 + length) + 3;
                        if (i < 7)
                        {
                            length = g.IndexOf(',', num5) - num5;
                            if (length <= 0)
                            {
                                throw new FormatException(Environment.GetResourceString("Format_GuidComma"));
                            }
                        }
                        else
                        {
                            length = g.IndexOf('}', num5) - num5;
                            if (length <= 0)
                            {
                                throw new FormatException(Environment.GetResourceString("Format_GuidBraceAfterLastNumber"));
                            }
                        }
                        uint num8 = (uint) Convert.ToInt32(g.Substring(num5, length), 0x10);
                        if (num8 > 0xff)
                        {
                            throw new FormatException(Environment.GetResourceString("Overflow_Byte"));
                        }
                        buffer[i] = (byte) num8;
                    }
                    this._d = buffer[0];
                    this._e = buffer[1];
                    this._f = buffer[2];
                    this._g = buffer[3];
                    this._h = buffer[4];
                    this._i = buffer[5];
                    this._j = buffer[6];
                    this._k = buffer[7];
                    if ((((num5 + length) + 1) >= g.Length) || (g[(num5 + length) + 1] != '}'))
                    {
                        throw new FormatException(Environment.GetResourceString("Format_GuidEndBrace"));
                    }
                    if (((num5 + length) + 1) != (g.Length - 1))
                    {
                        throw new FormatException(Environment.GetResourceString("Format_ExtraJunkAtEnd"));
                    }
                }
                else
                {
                    string s = g.Trim();
                    if (s.Length != 0x20)
                    {
                        throw new FormatException(Environment.GetResourceString("Format_GuidInvLen"));
                    }
                    for (int j = 0; j < s.Length; j++)
                    {
                        char c = s[j];
                        if ((c < '0') || (c > '9'))
                        {
                            char ch2 = char.ToUpper(c, CultureInfo.InvariantCulture);
                            if ((ch2 < 'A') || (ch2 > 'F'))
                            {
                                throw new FormatException(Environment.GetResourceString("Format_GuidInvalidChar"));
                            }
                        }
                    }
                    this._a = ParseNumbers.StringToInt(s.Substring(startIndex, 8), 0x10, 0x1000);
                    startIndex += 8;
                    this._b = (short) ParseNumbers.StringToInt(s.Substring(startIndex, 4), 0x10, 0x1000);
                    startIndex += 4;
                    this._c = (short) ParseNumbers.StringToInt(s.Substring(startIndex, 4), 0x10, 0x1000);
                    startIndex += 4;
                    num2 = (short) ParseNumbers.StringToInt(s.Substring(startIndex, 4), 0x10, 0x1000);
                    startIndex += 4;
                    parsePos = startIndex;
                    num3 = ParseNumbers.StringToLong(s, 0x10, startIndex, ref parsePos);
                    if ((parsePos - startIndex) != 12)
                    {
                        throw new FormatException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Format_GuidInvLen"), new object[0]));
                    }
                    this._d = (byte) (num2 >> 8);
                    this._e = (byte) num2;
                    num2 = (int) (num3 >> 0x20);
                    this._f = (byte) (num2 >> 8);
                    this._g = (byte) num2;
                    num2 = (int) num3;
                    this._h = (byte) (num2 >> 0x18);
                    this._i = (byte) (num2 >> 0x10);
                    this._j = (byte) (num2 >> 8);
                    this._k = (byte) num2;
                }
            }
            catch (IndexOutOfRangeException)
            {
                throw new FormatException(Environment.GetResourceString("Format_GuidUnrecognized"));
            }
        }

        public Guid(int a, short b, short c, byte[] d)
        {
            if (d == null)
            {
                throw new ArgumentNullException("d");
            }
            if (d.Length != 8)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Arg_GuidArrayCtor"), new object[] { "8" }));
            }
            this._a = a;
            this._b = b;
            this._c = c;
            this._d = d[0];
            this._e = d[1];
            this._f = d[2];
            this._g = d[3];
            this._h = d[4];
            this._i = d[5];
            this._j = d[6];
            this._k = d[7];
        }

        public Guid(int a, short b, short c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k)
        {
            this._a = a;
            this._b = b;
            this._c = c;
            this._d = d;
            this._e = e;
            this._f = f;
            this._g = g;
            this._h = h;
            this._i = i;
            this._j = j;
            this._k = k;
        }

        private static int TryParse(string str, ref int parsePos, int requiredLength)
        {
            int num = parsePos;
            int num2 = ParseNumbers.StringToInt(str, 0x10, 0x2000, ref parsePos);
            if ((parsePos - num) != requiredLength)
            {
                throw new FormatException(Environment.GetResourceString("Format_GuidInvalidChar"));
            }
            return num2;
        }

        private static string EatAllWhitespace(string str)
        {
            int length = 0;
            char[] chArray = new char[str.Length];
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (!char.IsWhiteSpace(c))
                {
                    chArray[length++] = c;
                }
            }
            return new string(chArray, 0, length);
        }

        private static bool IsHexPrefix(string str, int i)
        {
            return ((str[i] == '0') && (char.ToLower(str[i + 1], CultureInfo.InvariantCulture) == 'x'));
        }

        public byte[] ToByteArray()
        {
            return new byte[] { ((byte) this._a), ((byte) (this._a >> 8)), ((byte) (this._a >> 0x10)), ((byte) (this._a >> 0x18)), ((byte) this._b), ((byte) (this._b >> 8)), ((byte) this._c), ((byte) (this._c >> 8)), this._d, this._e, this._f, this._g, this._h, this._i, this._j, this._k };
        }

        public override string ToString()
        {
            return this.ToString("D", null);
        }

        public override int GetHashCode()
        {
            return ((this._a ^ ((this._b << 0x10) | ((ushort) this._c))) ^ ((this._f << 0x18) | this._k));
        }

        public override bool Equals(object o)
        {
            if ((o == null) || !(o is Guid))
            {
                return false;
            }
            Guid guid = (Guid) o;
            if (guid._a != this._a)
            {
                return false;
            }
            if (guid._b != this._b)
            {
                return false;
            }
            if (guid._c != this._c)
            {
                return false;
            }
            if (guid._d != this._d)
            {
                return false;
            }
            if (guid._e != this._e)
            {
                return false;
            }
            if (guid._f != this._f)
            {
                return false;
            }
            if (guid._g != this._g)
            {
                return false;
            }
            if (guid._h != this._h)
            {
                return false;
            }
            if (guid._i != this._i)
            {
                return false;
            }
            if (guid._j != this._j)
            {
                return false;
            }
            if (guid._k != this._k)
            {
                return false;
            }
            return true;
        }

        public bool Equals(Guid g)
        {
            if (g._a != this._a)
            {
                return false;
            }
            if (g._b != this._b)
            {
                return false;
            }
            if (g._c != this._c)
            {
                return false;
            }
            if (g._d != this._d)
            {
                return false;
            }
            if (g._e != this._e)
            {
                return false;
            }
            if (g._f != this._f)
            {
                return false;
            }
            if (g._g != this._g)
            {
                return false;
            }
            if (g._h != this._h)
            {
                return false;
            }
            if (g._i != this._i)
            {
                return false;
            }
            if (g._j != this._j)
            {
                return false;
            }
            if (g._k != this._k)
            {
                return false;
            }
            return true;
        }

        private int GetResult(uint me, uint them)
        {
            if (me < them)
            {
                return -1;
            }
            return 1;
        }

        public int CompareTo(object value)
        {
            if (value == null)
            {
                return 1;
            }
            if (!(value is Guid))
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_MustBeGuid"));
            }
            Guid guid = (Guid) value;
            if (guid._a != this._a)
            {
                return this.GetResult((uint) this._a, (uint) guid._a);
            }
            if (guid._b != this._b)
            {
                return this.GetResult((uint) this._b, (uint) guid._b);
            }
            if (guid._c != this._c)
            {
                return this.GetResult((uint) this._c, (uint) guid._c);
            }
            if (guid._d != this._d)
            {
                return this.GetResult(this._d, guid._d);
            }
            if (guid._e != this._e)
            {
                return this.GetResult(this._e, guid._e);
            }
            if (guid._f != this._f)
            {
                return this.GetResult(this._f, guid._f);
            }
            if (guid._g != this._g)
            {
                return this.GetResult(this._g, guid._g);
            }
            if (guid._h != this._h)
            {
                return this.GetResult(this._h, guid._h);
            }
            if (guid._i != this._i)
            {
                return this.GetResult(this._i, guid._i);
            }
            if (guid._j != this._j)
            {
                return this.GetResult(this._j, guid._j);
            }
            if (guid._k != this._k)
            {
                return this.GetResult(this._k, guid._k);
            }
            return 0;
        }

        public int CompareTo(Guid value)
        {
            if (value._a != this._a)
            {
                return this.GetResult((uint) this._a, (uint) value._a);
            }
            if (value._b != this._b)
            {
                return this.GetResult((uint) this._b, (uint) value._b);
            }
            if (value._c != this._c)
            {
                return this.GetResult((uint) this._c, (uint) value._c);
            }
            if (value._d != this._d)
            {
                return this.GetResult(this._d, value._d);
            }
            if (value._e != this._e)
            {
                return this.GetResult(this._e, value._e);
            }
            if (value._f != this._f)
            {
                return this.GetResult(this._f, value._f);
            }
            if (value._g != this._g)
            {
                return this.GetResult(this._g, value._g);
            }
            if (value._h != this._h)
            {
                return this.GetResult(this._h, value._h);
            }
            if (value._i != this._i)
            {
                return this.GetResult(this._i, value._i);
            }
            if (value._j != this._j)
            {
                return this.GetResult(this._j, value._j);
            }
            if (value._k != this._k)
            {
                return this.GetResult(this._k, value._k);
            }
            return 0;
        }

        public static bool operator ==(Guid a, Guid b)
        {
            if (a._a != b._a)
            {
                return false;
            }
            if (a._b != b._b)
            {
                return false;
            }
            if (a._c != b._c)
            {
                return false;
            }
            if (a._d != b._d)
            {
                return false;
            }
            if (a._e != b._e)
            {
                return false;
            }
            if (a._f != b._f)
            {
                return false;
            }
            if (a._g != b._g)
            {
                return false;
            }
            if (a._h != b._h)
            {
                return false;
            }
            if (a._i != b._i)
            {
                return false;
            }
            if (a._j != b._j)
            {
                return false;
            }
            if (a._k != b._k)
            {
                return false;
            }
            return true;
        }

        public static bool operator !=(Guid a, Guid b)
        {
            return !(a == b);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void CompleteGuid();
        public static Guid NewGuid()
        {
            return new Guid(false);
        }

        public string ToString(string format)
        {
            return this.ToString(format, null);
        }

        private static char HexToChar(int a)
        {
            a &= 15;
            return ((a > 9) ? ((char) ((a - 10) + 0x61)) : ((char) (a + 0x30)));
        }

        private static int HexsToChars(char[] guidChars, int offset, int a, int b)
        {
            guidChars[offset++] = HexToChar(a >> 4);
            guidChars[offset++] = HexToChar(a);
            guidChars[offset++] = HexToChar(b >> 4);
            guidChars[offset++] = HexToChar(b);
            return offset;
        }

        public string ToString(string format, IFormatProvider provider)
        {
            char[] chArray;
            if ((format == null) || (format.Length == 0))
            {
                format = "D";
            }
            int offset = 0;
            int length = 0x26;
            bool flag = true;
            if (format.Length != 1)
            {
                throw new FormatException(Environment.GetResourceString("Format_InvalidGuidFormatSpecification"));
            }
            char ch = format[0];
            switch (ch)
            {
                case 'D':
                case 'd':
                    chArray = new char[0x24];
                    length = 0x24;
                    break;

                case 'N':
                case 'n':
                    chArray = new char[0x20];
                    length = 0x20;
                    flag = false;
                    break;

                case 'B':
                case 'b':
                    chArray = new char[0x26];
                    chArray[offset++] = '{';
                    chArray[0x25] = '}';
                    break;

                default:
                    if ((ch != 'P') && (ch != 'p'))
                    {
                        throw new FormatException(Environment.GetResourceString("Format_InvalidGuidFormatSpecification"));
                    }
                    chArray = new char[0x26];
                    chArray[offset++] = '(';
                    chArray[0x25] = ')';
                    break;
            }
            offset = HexsToChars(chArray, offset, this._a >> 0x18, this._a >> 0x10);
            offset = HexsToChars(chArray, offset, this._a >> 8, this._a);
            if (flag)
            {
                chArray[offset++] = '-';
            }
            offset = HexsToChars(chArray, offset, this._b >> 8, this._b);
            if (flag)
            {
                chArray[offset++] = '-';
            }
            offset = HexsToChars(chArray, offset, this._c >> 8, this._c);
            if (flag)
            {
                chArray[offset++] = '-';
            }
            offset = HexsToChars(chArray, offset, this._d, this._e);
            if (flag)
            {
                chArray[offset++] = '-';
            }
            offset = HexsToChars(chArray, offset, this._f, this._g);
            offset = HexsToChars(chArray, offset, this._h, this._i);
            offset = HexsToChars(chArray, offset, this._j, this._k);
            return new string(chArray, 0, length);
        }

        static Guid()
        {
            Empty = new Guid();
        }
    }
}

