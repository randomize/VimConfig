namespace System
{
    using System.Runtime.InteropServices;
    using System.Text;

    [Serializable, StructLayout(LayoutKind.Sequential), ComVisible(true)]
    public struct TimeSpan : IComparable, IComparable<TimeSpan>, IEquatable<TimeSpan>
    {
        public const long TicksPerMillisecond = 0x2710L;
        private const double MillisecondsPerTick = 0.0001;
        public const long TicksPerSecond = 0x989680L;
        private const double SecondsPerTick = 1E-07;
        public const long TicksPerMinute = 0x23c34600L;
        private const double MinutesPerTick = 1.6666666666666667E-09;
        public const long TicksPerHour = 0x861c46800L;
        private const double HoursPerTick = 2.7777777777777777E-11;
        public const long TicksPerDay = 0xc92a69c000L;
        private const double DaysPerTick = 1.1574074074074074E-12;
        private const int MillisPerSecond = 0x3e8;
        private const int MillisPerMinute = 0xea60;
        private const int MillisPerHour = 0x36ee80;
        private const int MillisPerDay = 0x5265c00;
        private const long MaxSeconds = 0xd6bf94d5e5L;
        private const long MinSeconds = -922337203685L;
        private const long MaxMilliSeconds = 0x346dc5d638865L;
        private const long MinMilliSeconds = -922337203685477L;
        public static readonly TimeSpan Zero;
        public static readonly TimeSpan MaxValue;
        public static readonly TimeSpan MinValue;
        internal long _ticks;
        public TimeSpan(long ticks)
        {
            this._ticks = ticks;
        }

        public TimeSpan(int hours, int minutes, int seconds)
        {
            this._ticks = TimeToTicks(hours, minutes, seconds);
        }

        public TimeSpan(int days, int hours, int minutes, int seconds) : this(days, hours, minutes, seconds, 0)
        {
        }

        public TimeSpan(int days, int hours, int minutes, int seconds, int milliseconds)
        {
            long num = ((((((days * 0xe10L) * 0x18L) + (hours * 0xe10L)) + (minutes * 60L)) + seconds) * 0x3e8L) + milliseconds;
            if ((num > 0x346dc5d638865L) || (num < -922337203685477L))
            {
                throw new ArgumentOutOfRangeException(null, Environment.GetResourceString("Overflow_TimeSpanTooLong"));
            }
            this._ticks = num * 0x2710L;
        }

        public long Ticks
        {
            get
            {
                return this._ticks;
            }
        }
        public int Days
        {
            get
            {
                return (int) (this._ticks / 0xc92a69c000L);
            }
        }
        public int Hours
        {
            get
            {
                return (int) ((this._ticks / 0x861c46800L) % 0x18L);
            }
        }
        public int Milliseconds
        {
            get
            {
                return (int) ((this._ticks / 0x2710L) % 0x3e8L);
            }
        }
        public int Minutes
        {
            get
            {
                return (int) ((this._ticks / 0x23c34600L) % 60L);
            }
        }
        public int Seconds
        {
            get
            {
                return (int) ((this._ticks / 0x989680L) % 60L);
            }
        }
        public double TotalDays
        {
            get
            {
                return (this._ticks * 1.1574074074074074E-12);
            }
        }
        public double TotalHours
        {
            get
            {
                return (this._ticks * 2.7777777777777777E-11);
            }
        }
        public double TotalMilliseconds
        {
            get
            {
                double num = this._ticks * 0.0001;
                if (num > 922337203685477)
                {
                    return 922337203685477;
                }
                if (num < -922337203685477)
                {
                    return -922337203685477;
                }
                return num;
            }
        }
        public double TotalMinutes
        {
            get
            {
                return (this._ticks * 1.6666666666666667E-09);
            }
        }
        public double TotalSeconds
        {
            get
            {
                return (this._ticks * 1E-07);
            }
        }
        public TimeSpan Add(TimeSpan ts)
        {
            long ticks = this._ticks + ts._ticks;
            if (((this._ticks >> 0x3f) == (ts._ticks >> 0x3f)) && ((this._ticks >> 0x3f) != (ticks >> 0x3f)))
            {
                throw new OverflowException(Environment.GetResourceString("Overflow_TimeSpanTooLong"));
            }
            return new TimeSpan(ticks);
        }

        public static int Compare(TimeSpan t1, TimeSpan t2)
        {
            if (t1._ticks > t2._ticks)
            {
                return 1;
            }
            if (t1._ticks < t2._ticks)
            {
                return -1;
            }
            return 0;
        }

        public int CompareTo(object value)
        {
            if (value == null)
            {
                return 1;
            }
            if (!(value is TimeSpan))
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_MustBeTimeSpan"));
            }
            long num = ((TimeSpan) value)._ticks;
            if (this._ticks > num)
            {
                return 1;
            }
            if (this._ticks < num)
            {
                return -1;
            }
            return 0;
        }

        public int CompareTo(TimeSpan value)
        {
            long num = value._ticks;
            if (this._ticks > num)
            {
                return 1;
            }
            if (this._ticks < num)
            {
                return -1;
            }
            return 0;
        }

        public static TimeSpan FromDays(double value)
        {
            return Interval(value, 0x5265c00);
        }

        public TimeSpan Duration()
        {
            if (this._ticks == MinValue._ticks)
            {
                throw new OverflowException(Environment.GetResourceString("Overflow_Duration"));
            }
            return new TimeSpan((this._ticks >= 0L) ? this._ticks : -this._ticks);
        }

        public override bool Equals(object value)
        {
            return ((value is TimeSpan) && (this._ticks == ((TimeSpan) value)._ticks));
        }

        public bool Equals(TimeSpan obj)
        {
            return (this._ticks == obj._ticks);
        }

        public static bool Equals(TimeSpan t1, TimeSpan t2)
        {
            return (t1._ticks == t2._ticks);
        }

        public override int GetHashCode()
        {
            return (((int) this._ticks) ^ ((int) (this._ticks >> 0x20)));
        }

        public static TimeSpan FromHours(double value)
        {
            return Interval(value, 0x36ee80);
        }

        private static TimeSpan Interval(double value, int scale)
        {
            if (double.IsNaN(value))
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_CannotBeNaN"));
            }
            double num = value * scale;
            double num2 = num + ((value >= 0.0) ? 0.5 : -0.5);
            if ((num2 > 922337203685477) || (num2 < -922337203685477))
            {
                throw new OverflowException(Environment.GetResourceString("Overflow_TimeSpanTooLong"));
            }
            return new TimeSpan(((long) num2) * 0x2710L);
        }

        public static TimeSpan FromMilliseconds(double value)
        {
            return Interval(value, 1);
        }

        public static TimeSpan FromMinutes(double value)
        {
            return Interval(value, 0xea60);
        }

        public TimeSpan Negate()
        {
            if (this._ticks == MinValue._ticks)
            {
                throw new OverflowException(Environment.GetResourceString("Overflow_NegateTwosCompNum"));
            }
            return new TimeSpan(-this._ticks);
        }

        public static TimeSpan Parse(string s)
        {
            StringParser parser2 = new StringParser();
            return new TimeSpan(parser2.Parse(s));
        }

        public static bool TryParse(string s, out TimeSpan result)
        {
            long num;
            StringParser parser2 = new StringParser();
            if (parser2.TryParse(s, out num))
            {
                result = new TimeSpan(num);
                return true;
            }
            result = Zero;
            return false;
        }

        public static TimeSpan FromSeconds(double value)
        {
            return Interval(value, 0x3e8);
        }

        public TimeSpan Subtract(TimeSpan ts)
        {
            long ticks = this._ticks - ts._ticks;
            if (((this._ticks >> 0x3f) != (ts._ticks >> 0x3f)) && ((this._ticks >> 0x3f) != (ticks >> 0x3f)))
            {
                throw new OverflowException(Environment.GetResourceString("Overflow_TimeSpanTooLong"));
            }
            return new TimeSpan(ticks);
        }

        public static TimeSpan FromTicks(long value)
        {
            return new TimeSpan(value);
        }

        internal static long TimeToTicks(int hour, int minute, int second)
        {
            long num = ((hour * 0xe10L) + (minute * 60L)) + second;
            if ((num > 0xd6bf94d5e5L) || (num < -922337203685L))
            {
                throw new ArgumentOutOfRangeException(null, Environment.GetResourceString("Overflow_TimeSpanTooLong"));
            }
            return (num * 0x989680L);
        }

        private string IntToString(int n, int digits)
        {
            return ParseNumbers.IntToString(n, 10, digits, '0', 0);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            int num = (int) (this._ticks / 0xc92a69c000L);
            long num2 = this._ticks % 0xc92a69c000L;
            if (this._ticks < 0L)
            {
                builder.Append("-");
                num = -num;
                num2 = -num2;
            }
            if (num != 0)
            {
                builder.Append(num);
                builder.Append(".");
            }
            builder.Append(this.IntToString((int) ((num2 / 0x861c46800L) % 0x18L), 2));
            builder.Append(":");
            builder.Append(this.IntToString((int) ((num2 / 0x23c34600L) % 60L), 2));
            builder.Append(":");
            builder.Append(this.IntToString((int) ((num2 / 0x989680L) % 60L), 2));
            int n = (int) (num2 % 0x989680L);
            if (n != 0)
            {
                builder.Append(".");
                builder.Append(this.IntToString(n, 7));
            }
            return builder.ToString();
        }

        public static TimeSpan operator -(TimeSpan t)
        {
            if (t._ticks == MinValue._ticks)
            {
                throw new OverflowException(Environment.GetResourceString("Overflow_NegateTwosCompNum"));
            }
            return new TimeSpan(-t._ticks);
        }

        public static TimeSpan operator -(TimeSpan t1, TimeSpan t2)
        {
            return t1.Subtract(t2);
        }

        public static TimeSpan operator +(TimeSpan t)
        {
            return t;
        }

        public static TimeSpan operator +(TimeSpan t1, TimeSpan t2)
        {
            return t1.Add(t2);
        }

        public static bool operator ==(TimeSpan t1, TimeSpan t2)
        {
            return (t1._ticks == t2._ticks);
        }

        public static bool operator !=(TimeSpan t1, TimeSpan t2)
        {
            return (t1._ticks != t2._ticks);
        }

        public static bool operator <(TimeSpan t1, TimeSpan t2)
        {
            return (t1._ticks < t2._ticks);
        }

        public static bool operator <=(TimeSpan t1, TimeSpan t2)
        {
            return (t1._ticks <= t2._ticks);
        }

        public static bool operator >(TimeSpan t1, TimeSpan t2)
        {
            return (t1._ticks > t2._ticks);
        }

        public static bool operator >=(TimeSpan t1, TimeSpan t2)
        {
            return (t1._ticks >= t2._ticks);
        }

        static TimeSpan()
        {
            Zero = new TimeSpan(0L);
            MaxValue = new TimeSpan(0x7fffffffffffffffL);
            MinValue = new TimeSpan(-9223372036854775808L);
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct StringParser
        {
            private string str;
            private char ch;
            private int pos;
            private int len;
            private ParseError error;
            internal void NextChar()
            {
                if (this.pos < this.len)
                {
                    this.pos++;
                }
                this.ch = (this.pos < this.len) ? this.str[this.pos] : '\0';
            }

            internal char NextNonDigit()
            {
                for (int i = this.pos; i < this.len; i++)
                {
                    char ch = this.str[i];
                    if ((ch < '0') || (ch > '9'))
                    {
                        return ch;
                    }
                }
                return '\0';
            }

            internal long Parse(string s)
            {
                long num;
                if (this.TryParse(s, out num))
                {
                    return num;
                }
                switch (this.error)
                {
                    case ParseError.Format:
                        throw new FormatException(Environment.GetResourceString("Format_InvalidString"));

                    case ParseError.Overflow:
                        throw new OverflowException(Environment.GetResourceString("Overflow_TimeSpanTooLong"));

                    case ParseError.OverflowHoursMinutesSeconds:
                        throw new OverflowException(Environment.GetResourceString("Overflow_TimeSpanElementTooLarge"));

                    case ParseError.ArgumentNull:
                        throw new ArgumentNullException("s");
                }
                return 0L;
            }

            internal bool TryParse(string s, out long value)
            {
                long num;
                value = 0L;
                if (s == null)
                {
                    this.error = ParseError.ArgumentNull;
                    return false;
                }
                this.str = s;
                this.len = s.Length;
                this.pos = -1;
                this.NextChar();
                this.SkipBlanks();
                bool flag = false;
                if (this.ch == '-')
                {
                    flag = true;
                    this.NextChar();
                }
                if (this.NextNonDigit() == ':')
                {
                    if (!this.ParseTime(out num))
                    {
                        return false;
                    }
                }
                else
                {
                    int num2;
                    if (!this.ParseInt(0xa2e3ff, out num2))
                    {
                        return false;
                    }
                    num = num2 * 0xc92a69c000L;
                    if (this.ch == '.')
                    {
                        long num3;
                        this.NextChar();
                        if (!this.ParseTime(out num3))
                        {
                            return false;
                        }
                        num += num3;
                    }
                }
                if (flag)
                {
                    num = -num;
                    if (num > 0L)
                    {
                        this.error = ParseError.Overflow;
                        return false;
                    }
                }
                else if (num < 0L)
                {
                    this.error = ParseError.Overflow;
                    return false;
                }
                this.SkipBlanks();
                if (this.pos < this.len)
                {
                    this.error = ParseError.Format;
                    return false;
                }
                value = num;
                return true;
            }

            internal bool ParseInt(int max, out int i)
            {
                i = 0;
                int pos = this.pos;
                while ((this.ch >= '0') && (this.ch <= '9'))
                {
                    if ((((long) i) & 0xf0000000L) != 0L)
                    {
                        this.error = ParseError.Overflow;
                        return false;
                    }
                    i = ((i * 10) + this.ch) - 0x30;
                    if (i < 0)
                    {
                        this.error = ParseError.Overflow;
                        return false;
                    }
                    this.NextChar();
                }
                if (pos == this.pos)
                {
                    this.error = ParseError.Format;
                    return false;
                }
                if (i > max)
                {
                    this.error = ParseError.Overflow;
                    return false;
                }
                return true;
            }

            internal bool ParseTime(out long time)
            {
                int num;
                time = 0L;
                if (!this.ParseInt(0x17, out num))
                {
                    if (this.error == ParseError.Overflow)
                    {
                        this.error = ParseError.OverflowHoursMinutesSeconds;
                    }
                    return false;
                }
                time = num * 0x861c46800L;
                if (this.ch != ':')
                {
                    this.error = ParseError.Format;
                    return false;
                }
                this.NextChar();
                if (!this.ParseInt(0x3b, out num))
                {
                    if (this.error == ParseError.Overflow)
                    {
                        this.error = ParseError.OverflowHoursMinutesSeconds;
                    }
                    return false;
                }
                time += num * 0x23c34600L;
                if (this.ch == ':')
                {
                    this.NextChar();
                    if (this.ch != '.')
                    {
                        if (!this.ParseInt(0x3b, out num))
                        {
                            if (this.error == ParseError.Overflow)
                            {
                                this.error = ParseError.OverflowHoursMinutesSeconds;
                            }
                            return false;
                        }
                        time += num * 0x989680L;
                    }
                    if (this.ch == '.')
                    {
                        this.NextChar();
                        int num2 = 0x989680;
                        while (((num2 > 1) && (this.ch >= '0')) && (this.ch <= '9'))
                        {
                            num2 /= 10;
                            time += (this.ch - '0') * num2;
                            this.NextChar();
                        }
                    }
                }
                return true;
            }

            internal void SkipBlanks()
            {
                while ((this.ch == ' ') || (this.ch == '\t'))
                {
                    this.NextChar();
                }
            }
            private enum ParseError
            {
                ArgumentNull = 4,
                Format = 1,
                Overflow = 2,
                OverflowHoursMinutesSeconds = 3
            }
        }
    }
}

