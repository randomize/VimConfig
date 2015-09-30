namespace UnityEngine.Networking
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct NetworkHash128
    {
        public byte i0;
        public byte i1;
        public byte i2;
        public byte i3;
        public byte i4;
        public byte i5;
        public byte i6;
        public byte i7;
        public byte i8;
        public byte i9;
        public byte i10;
        public byte i11;
        public byte i12;
        public byte i13;
        public byte i14;
        public byte i15;
        public void Reset()
        {
            this.i0 = 0;
            this.i1 = 0;
            this.i2 = 0;
            this.i3 = 0;
            this.i4 = 0;
            this.i5 = 0;
            this.i6 = 0;
            this.i7 = 0;
            this.i8 = 0;
            this.i9 = 0;
            this.i10 = 0;
            this.i11 = 0;
            this.i12 = 0;
            this.i13 = 0;
            this.i14 = 0;
            this.i15 = 0;
        }

        public bool IsValid()
        {
            return ((((((((((((((((this.i0 | this.i1) | this.i2) | this.i3) | this.i4) | this.i5) | this.i6) | this.i7) | this.i8) | this.i9) | this.i10) | this.i11) | this.i12) | this.i13) | this.i14) | this.i15) != 0);
        }

        private static int HexToNumber(char c)
        {
            if ((c >= '0') && (c <= '9'))
            {
                return (c - '0');
            }
            if ((c >= 'a') && (c <= 'f'))
            {
                return ((c - 'a') + 10);
            }
            if ((c >= 'A') && (c <= 'F'))
            {
                return ((c - 'A') + 10);
            }
            return 0;
        }

        public static NetworkHash128 Parse(string text)
        {
            NetworkHash128 hash;
            int length = text.Length;
            if (length < 0x20)
            {
                string str = string.Empty;
                for (int i = 0; i < (0x20 - length); i++)
                {
                    str = str + "0";
                }
                text = str + text;
            }
            hash.i0 = (byte) ((HexToNumber(text[0]) * 0x10) + HexToNumber(text[1]));
            hash.i1 = (byte) ((HexToNumber(text[2]) * 0x10) + HexToNumber(text[3]));
            hash.i2 = (byte) ((HexToNumber(text[4]) * 0x10) + HexToNumber(text[5]));
            hash.i3 = (byte) ((HexToNumber(text[6]) * 0x10) + HexToNumber(text[7]));
            hash.i4 = (byte) ((HexToNumber(text[8]) * 0x10) + HexToNumber(text[9]));
            hash.i5 = (byte) ((HexToNumber(text[10]) * 0x10) + HexToNumber(text[11]));
            hash.i6 = (byte) ((HexToNumber(text[12]) * 0x10) + HexToNumber(text[13]));
            hash.i7 = (byte) ((HexToNumber(text[14]) * 0x10) + HexToNumber(text[15]));
            hash.i8 = (byte) ((HexToNumber(text[0x10]) * 0x10) + HexToNumber(text[0x11]));
            hash.i9 = (byte) ((HexToNumber(text[0x12]) * 0x10) + HexToNumber(text[0x13]));
            hash.i10 = (byte) ((HexToNumber(text[20]) * 0x10) + HexToNumber(text[0x15]));
            hash.i11 = (byte) ((HexToNumber(text[0x16]) * 0x10) + HexToNumber(text[0x17]));
            hash.i12 = (byte) ((HexToNumber(text[0x18]) * 0x10) + HexToNumber(text[0x19]));
            hash.i13 = (byte) ((HexToNumber(text[0x1a]) * 0x10) + HexToNumber(text[0x1b]));
            hash.i14 = (byte) ((HexToNumber(text[0x1c]) * 0x10) + HexToNumber(text[0x1d]));
            hash.i15 = (byte) ((HexToNumber(text[30]) * 0x10) + HexToNumber(text[0x1f]));
            return hash;
        }

        public override string ToString()
        {
            object[] args = new object[] { this.i0, this.i1, this.i2, this.i3, this.i4, this.i5, this.i6, this.i7, this.i8, this.i9, this.i10, this.i11, this.i12, this.i13, this.i14, this.i15 };
            return string.Format("{0:x}{1:x}{2:x}{3:x}{4:x}{5:x}{6:x}{7:x}{8:x}{9:x}{10:x}{11:x}{12:x}{13:x}{14:x}{15:x}", args);
        }
    }
}

