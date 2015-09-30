namespace System.Text
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Threading;

    [Serializable, ComVisible(true)]
    public sealed class StringBuilder : ISerializable
    {
        private const string CapacityField = "Capacity";
        internal const int DefaultCapacity = 0x10;
        internal IntPtr m_currentThread;
        internal int m_MaxCapacity;
        internal volatile string m_StringValue;
        private const string MaxCapacityField = "m_MaxCapacity";
        private const string StringValueField = "m_StringValue";
        private const string ThreadIDField = "m_currentThread";

        public StringBuilder() : this(0x10)
        {
        }

        public StringBuilder(int capacity) : this(string.Empty, capacity)
        {
        }

        public StringBuilder(string value) : this(value, 0x10)
        {
        }

        public StringBuilder(int capacity, int maxCapacity)
        {
            this.m_currentThread = Thread.InternalGetCurrentThread();
            if (capacity > maxCapacity)
            {
                throw new ArgumentOutOfRangeException("capacity", Environment.GetResourceString("ArgumentOutOfRange_Capacity"));
            }
            if (maxCapacity < 1)
            {
                throw new ArgumentOutOfRangeException("maxCapacity", Environment.GetResourceString("ArgumentOutOfRange_SmallMaxCapacity"));
            }
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException("capacity", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("ArgumentOutOfRange_MustBePositive"), new object[] { "capacity" }));
            }
            if (capacity == 0)
            {
                capacity = Math.Min(0x10, maxCapacity);
            }
            this.m_StringValue = string.GetStringForStringBuilder(string.Empty, capacity);
            this.m_MaxCapacity = maxCapacity;
        }

        private StringBuilder(SerializationInfo info, StreamingContext context)
        {
            this.m_currentThread = Thread.InternalGetCurrentThread();
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            int capacity = 0;
            string str = null;
            int num2 = 0x7fffffff;
            bool flag = false;
            SerializationInfoEnumerator enumerator = info.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string name = enumerator.Name;
                if (name != null)
                {
                    if (!(name == "m_MaxCapacity"))
                    {
                        if (name == "m_StringValue")
                        {
                            goto Label_007B;
                        }
                        if (name == "Capacity")
                        {
                            goto Label_0089;
                        }
                    }
                    else
                    {
                        num2 = info.GetInt32("m_MaxCapacity");
                    }
                }
                continue;
            Label_007B:
                str = info.GetString("m_StringValue");
                continue;
            Label_0089:
                capacity = info.GetInt32("Capacity");
                flag = true;
            }
            if (str == null)
            {
                str = string.Empty;
            }
            if ((num2 < 1) || (str.Length > num2))
            {
                throw new SerializationException(Environment.GetResourceString("Serialization_StringBuilderMaxCapacity"));
            }
            if (!flag)
            {
                capacity = 0x10;
                if (capacity < str.Length)
                {
                    capacity = str.Length;
                }
                if (capacity > num2)
                {
                    capacity = num2;
                }
            }
            if (((capacity < 0) || (capacity < str.Length)) || (capacity > num2))
            {
                throw new SerializationException(Environment.GetResourceString("Serialization_StringBuilderCapacity"));
            }
            this.m_MaxCapacity = num2;
            this.m_StringValue = string.GetStringForStringBuilder(str, 0, str.Length, capacity);
        }

        public StringBuilder(string value, int capacity) : this(value, 0, (value != null) ? value.Length : 0, capacity)
        {
        }

        public StringBuilder(string value, int startIndex, int length, int capacity)
        {
            this.m_currentThread = Thread.InternalGetCurrentThread();
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException("capacity", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("ArgumentOutOfRange_MustBePositive"), new object[] { "capacity" }));
            }
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("ArgumentOutOfRange_MustBeNonNegNum"), new object[] { "length" }));
            }
            if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_StartIndex"));
            }
            if (value == null)
            {
                value = string.Empty;
            }
            if (startIndex > (value.Length - length))
            {
                throw new ArgumentOutOfRangeException("length", Environment.GetResourceString("ArgumentOutOfRange_IndexLength"));
            }
            this.m_MaxCapacity = 0x7fffffff;
            if (capacity == 0)
            {
                capacity = 0x10;
            }
            while (capacity < length)
            {
                capacity *= 2;
                if (capacity < 0)
                {
                    capacity = length;
                    break;
                }
            }
            this.m_StringValue = string.GetStringForStringBuilder(value, startIndex, length, capacity);
        }

        public StringBuilder Append(bool value)
        {
            return this.Append(value.ToString());
        }

        public StringBuilder Append(byte value)
        {
            return this.Append(value.ToString(CultureInfo.CurrentCulture));
        }

        public StringBuilder Append(char value)
        {
            string stringValue = this.m_StringValue;
            IntPtr currentThread = Thread.InternalGetCurrentThread();
            if (this.m_currentThread != currentThread)
            {
                stringValue = string.GetStringForStringBuilder(stringValue, stringValue.Capacity);
            }
            int length = stringValue.Length;
            if (!this.NeedsAllocation(stringValue, length + 1))
            {
                stringValue.AppendInPlace(value, length);
                this.ReplaceString(currentThread, stringValue);
                return this;
            }
            string newString = this.GetNewString(stringValue, length + 1);
            newString.AppendInPlace(value, length);
            this.ReplaceString(currentThread, newString);
            return this;
        }

        public StringBuilder Append(decimal value)
        {
            return this.Append(value.ToString(CultureInfo.CurrentCulture));
        }

        public StringBuilder Append(double value)
        {
            return this.Append(value.ToString(CultureInfo.CurrentCulture));
        }

        public StringBuilder Append(short value)
        {
            return this.Append(value.ToString(CultureInfo.CurrentCulture));
        }

        public StringBuilder Append(int value)
        {
            return this.Append(value.ToString(CultureInfo.CurrentCulture));
        }

        public StringBuilder Append(long value)
        {
            return this.Append(value.ToString(CultureInfo.CurrentCulture));
        }

        public StringBuilder Append(object value)
        {
            if (value == null)
            {
                return this;
            }
            return this.Append(value.ToString());
        }

        [CLSCompliant(false)]
        public StringBuilder Append(sbyte value)
        {
            return this.Append(value.ToString(CultureInfo.CurrentCulture));
        }

        public StringBuilder Append(float value)
        {
            return this.Append(value.ToString(CultureInfo.CurrentCulture));
        }

        public StringBuilder Append(string value)
        {
            if (value != null)
            {
                string stringValue = this.m_StringValue;
                IntPtr currentThread = Thread.InternalGetCurrentThread();
                if (this.m_currentThread != currentThread)
                {
                    stringValue = string.GetStringForStringBuilder(stringValue, stringValue.Capacity);
                }
                int length = stringValue.Length;
                int requiredLength = length + value.Length;
                if (this.NeedsAllocation(stringValue, requiredLength))
                {
                    string newString = this.GetNewString(stringValue, requiredLength);
                    newString.AppendInPlace(value, length);
                    this.ReplaceString(currentThread, newString);
                }
                else
                {
                    stringValue.AppendInPlace(value, length);
                    this.ReplaceString(currentThread, stringValue);
                }
            }
            return this;
        }

        [CLSCompliant(false)]
        public StringBuilder Append(ushort value)
        {
            return this.Append(value.ToString(CultureInfo.CurrentCulture));
        }

        [CLSCompliant(false)]
        public StringBuilder Append(uint value)
        {
            return this.Append(value.ToString(CultureInfo.CurrentCulture));
        }

        [CLSCompliant(false)]
        public StringBuilder Append(ulong value)
        {
            return this.Append(value.ToString(CultureInfo.CurrentCulture));
        }

        public StringBuilder Append(char[] value)
        {
            if (value != null)
            {
                IntPtr ptr;
                int length = value.Length;
                string threadSafeString = this.GetThreadSafeString(out ptr);
                int currentLength = threadSafeString.Length;
                int requiredLength = currentLength + value.Length;
                if (this.NeedsAllocation(threadSafeString, requiredLength))
                {
                    string newString = this.GetNewString(threadSafeString, requiredLength);
                    newString.AppendInPlace(value, 0, length, currentLength);
                    this.ReplaceString(ptr, newString);
                }
                else
                {
                    threadSafeString.AppendInPlace(value, 0, length, currentLength);
                    this.ReplaceString(ptr, threadSafeString);
                }
            }
            return this;
        }

        public StringBuilder Append(char value, int repeatCount)
        {
            if (repeatCount != 0)
            {
                IntPtr ptr;
                if (repeatCount < 0)
                {
                    throw new ArgumentOutOfRangeException("repeatCount", Environment.GetResourceString("ArgumentOutOfRange_NegativeCount"));
                }
                string threadSafeString = this.GetThreadSafeString(out ptr);
                int length = threadSafeString.Length;
                int requiredLength = length + repeatCount;
                if (requiredLength < 0)
                {
                    throw new OutOfMemoryException();
                }
                if (!this.NeedsAllocation(threadSafeString, requiredLength))
                {
                    threadSafeString.AppendInPlace(value, repeatCount, length);
                    this.ReplaceString(ptr, threadSafeString);
                    return this;
                }
                string newString = this.GetNewString(threadSafeString, requiredLength);
                newString.AppendInPlace(value, repeatCount, length);
                this.ReplaceString(ptr, newString);
            }
            return this;
        }

        internal unsafe StringBuilder Append(char* value, int count)
        {
            if (value != null)
            {
                IntPtr ptr;
                string threadSafeString = this.GetThreadSafeString(out ptr);
                int length = threadSafeString.Length;
                int requiredLength = length + count;
                if (this.NeedsAllocation(threadSafeString, requiredLength))
                {
                    string newString = this.GetNewString(threadSafeString, requiredLength);
                    newString.AppendInPlace(value, count, length);
                    this.ReplaceString(ptr, newString);
                }
                else
                {
                    threadSafeString.AppendInPlace(value, count, length);
                    this.ReplaceString(ptr, threadSafeString);
                }
            }
            return this;
        }

        public StringBuilder Append(char[] value, int startIndex, int charCount)
        {
            if (value == null)
            {
                if ((startIndex != 0) || (charCount != 0))
                {
                    throw new ArgumentNullException("value");
                }
                return this;
            }
            if (charCount != 0)
            {
                IntPtr ptr;
                if (startIndex < 0)
                {
                    throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_GenericPositive"));
                }
                if (charCount < 0)
                {
                    throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_GenericPositive"));
                }
                if (charCount > (value.Length - startIndex))
                {
                    throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_Index"));
                }
                string threadSafeString = this.GetThreadSafeString(out ptr);
                int length = threadSafeString.Length;
                int requiredLength = length + charCount;
                if (this.NeedsAllocation(threadSafeString, requiredLength))
                {
                    string newString = this.GetNewString(threadSafeString, requiredLength);
                    newString.AppendInPlace(value, startIndex, charCount, length);
                    this.ReplaceString(ptr, newString);
                }
                else
                {
                    threadSafeString.AppendInPlace(value, startIndex, charCount, length);
                    this.ReplaceString(ptr, threadSafeString);
                }
            }
            return this;
        }

        public StringBuilder Append(string value, int startIndex, int count)
        {
            IntPtr ptr;
            if (value == null)
            {
                if ((startIndex != 0) || (count != 0))
                {
                    throw new ArgumentNullException("value");
                }
                return this;
            }
            if (count <= 0)
            {
                if (count != 0)
                {
                    throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_GenericPositive"));
                }
                return this;
            }
            if ((startIndex < 0) || (startIndex > (value.Length - count)))
            {
                throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_Index"));
            }
            string threadSafeString = this.GetThreadSafeString(out ptr);
            int length = threadSafeString.Length;
            int requiredLength = length + count;
            if (this.NeedsAllocation(threadSafeString, requiredLength))
            {
                string newString = this.GetNewString(threadSafeString, requiredLength);
                newString.AppendInPlace(value, startIndex, count, length);
                this.ReplaceString(ptr, newString);
            }
            else
            {
                threadSafeString.AppendInPlace(value, startIndex, count, length);
                this.ReplaceString(ptr, threadSafeString);
            }
            return this;
        }

        public StringBuilder AppendFormat(string format, object arg0)
        {
            return this.AppendFormat(null, format, new object[] { arg0 });
        }

        public StringBuilder AppendFormat(string format, params object[] args)
        {
            return this.AppendFormat(null, format, args);
        }

        public StringBuilder AppendFormat(IFormatProvider provider, string format, params object[] args)
        {
            int num3;
            if ((format == null) || (args == null))
            {
                throw new ArgumentNullException((format == null) ? "format" : "args");
            }
            char[] chArray = format.ToCharArray(0, format.Length);
            int index = 0;
            int length = chArray.Length;
            char ch = '\0';
            ICustomFormatter formatter = null;
            if (provider != null)
            {
                formatter = (ICustomFormatter) provider.GetFormat(typeof(ICustomFormatter));
            }
        Label_004E:
            num3 = index;
            int num4 = index;
            while (index < length)
            {
                ch = chArray[index];
                index++;
                if (ch == '}')
                {
                    if ((index < length) && (chArray[index] == '}'))
                    {
                        index++;
                    }
                    else
                    {
                        FormatError();
                    }
                }
                if (ch == '{')
                {
                    if ((index < length) && (chArray[index] == '{'))
                    {
                        index++;
                    }
                    else
                    {
                        index--;
                        break;
                    }
                }
                chArray[num4++] = ch;
            }
            if (num4 > num3)
            {
                this.Append(chArray, num3, num4 - num3);
            }
            if (index == length)
            {
                return this;
            }
            index++;
            if (((index == length) || ((ch = chArray[index]) < '0')) || (ch > '9'))
            {
                FormatError();
            }
            int num5 = 0;
            do
            {
                num5 = ((num5 * 10) + ch) - 0x30;
                index++;
                if (index == length)
                {
                    FormatError();
                }
                ch = chArray[index];
            }
            while (((ch >= '0') && (ch <= '9')) && (num5 < 0xf4240));
            if (num5 >= args.Length)
            {
                throw new FormatException(Environment.GetResourceString("Format_IndexOutOfRange"));
            }
            while ((index < length) && ((ch = chArray[index]) == ' '))
            {
                index++;
            }
            bool flag = false;
            int num6 = 0;
            if (ch == ',')
            {
                index++;
                while ((index < length) && (chArray[index] == ' '))
                {
                    index++;
                }
                if (index == length)
                {
                    FormatError();
                }
                ch = chArray[index];
                if (ch == '-')
                {
                    flag = true;
                    index++;
                    if (index == length)
                    {
                        FormatError();
                    }
                    ch = chArray[index];
                }
                if ((ch < '0') || (ch > '9'))
                {
                    FormatError();
                }
                do
                {
                    num6 = ((num6 * 10) + ch) - 0x30;
                    index++;
                    if (index == length)
                    {
                        FormatError();
                    }
                    ch = chArray[index];
                }
                while (((ch >= '0') && (ch <= '9')) && (num6 < 0xf4240));
            }
            while ((index < length) && ((ch = chArray[index]) == ' '))
            {
                index++;
            }
            object arg = args[num5];
            string str = null;
            if (ch == ':')
            {
                index++;
                num3 = index;
                num4 = index;
                while (true)
                {
                    if (index == length)
                    {
                        FormatError();
                    }
                    ch = chArray[index];
                    index++;
                    switch (ch)
                    {
                        case '{':
                            if ((index < length) && (chArray[index] == '{'))
                            {
                                index++;
                            }
                            else
                            {
                                FormatError();
                            }
                            break;

                        case '}':
                            if ((index < length) && (chArray[index] == '}'))
                            {
                                index++;
                            }
                            else
                            {
                                index--;
                                if (num4 > num3)
                                {
                                    str = new string(chArray, num3, num4 - num3);
                                }
                                goto Label_0253;
                            }
                            break;
                    }
                    chArray[num4++] = ch;
                }
            }
        Label_0253:
            if (ch != '}')
            {
                FormatError();
            }
            index++;
            string str2 = null;
            if (formatter != null)
            {
                str2 = formatter.Format(str, arg, provider);
            }
            if (str2 == null)
            {
                if (arg is IFormattable)
                {
                    str2 = ((IFormattable) arg).ToString(str, provider);
                }
                else if (arg != null)
                {
                    str2 = arg.ToString();
                }
            }
            if (str2 == null)
            {
                str2 = string.Empty;
            }
            int repeatCount = num6 - str2.Length;
            if (!flag && (repeatCount > 0))
            {
                this.Append(' ', repeatCount);
            }
            this.Append(str2);
            if (flag && (repeatCount > 0))
            {
                this.Append(' ', repeatCount);
            }
            goto Label_004E;
        }

        public StringBuilder AppendFormat(string format, object arg0, object arg1)
        {
            return this.AppendFormat(null, format, new object[] { arg0, arg1 });
        }

        public StringBuilder AppendFormat(string format, object arg0, object arg1, object arg2)
        {
            return this.AppendFormat(null, format, new object[] { arg0, arg1, arg2 });
        }

        [ComVisible(false)]
        public StringBuilder AppendLine()
        {
            return this.Append(Environment.NewLine);
        }

        [ComVisible(false)]
        public StringBuilder AppendLine(string value)
        {
            this.Append(value);
            return this.Append(Environment.NewLine);
        }

        [ComVisible(false)]
        public unsafe void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
        {
            IntPtr ptr;
            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(Environment.GetResourceString("Arg_NegativeArgCount"), "count");
            }
            if (destinationIndex < 0)
            {
                throw new ArgumentOutOfRangeException(Environment.GetResourceString("ArgumentOutOfRange_MustBeNonNegNum", new object[] { "destinationIndex" }), "destinationIndex");
            }
            if (destinationIndex > (destination.Length - count))
            {
                throw new ArgumentException(Environment.GetResourceString("ArgumentOutOfRange_OffsetOut"));
            }
            string threadSafeString = this.GetThreadSafeString(out ptr);
            int length = threadSafeString.Length;
            if ((sourceIndex < 0) || (sourceIndex > length))
            {
                throw new ArgumentOutOfRangeException(Environment.GetResourceString("ArgumentOutOfRange_Index"));
            }
            if (sourceIndex > (length - count))
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_LongerThanSrcString"));
            }
            if (count != 0)
            {
                fixed (char* chRef = &(destination[destinationIndex]))
                {
                    fixed (char* str2 = ((char*) threadSafeString))
                    {
                        char* chPtr = str2;
                        char* chPtr2 = chPtr + sourceIndex;
                        Buffer.memcpyimpl((byte*) chPtr2, (byte*) chRef, count * 2);
                    }
                }
            }
        }

        public int EnsureCapacity(int capacity)
        {
            IntPtr ptr;
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException("capacity", Environment.GetResourceString("ArgumentOutOfRange_NeedPosCapacity"));
            }
            string threadSafeString = this.GetThreadSafeString(out ptr);
            if (!this.NeedsAllocation(threadSafeString, capacity))
            {
                return threadSafeString.Capacity;
            }
            string newString = this.GetNewString(threadSafeString, capacity);
            this.ReplaceString(ptr, newString);
            return newString.Capacity;
        }

        public bool Equals(StringBuilder sb)
        {
            if (sb == null)
            {
                return false;
            }
            return (((this.Capacity == sb.Capacity) && (this.MaxCapacity == sb.MaxCapacity)) && this.m_StringValue.Equals((string) sb.m_StringValue));
        }

        private static void FormatError()
        {
            throw new FormatException(Environment.GetResourceString("Format_InvalidString"));
        }

        private string GetNewString(string currentString, int requiredLength)
        {
            int maxCapacity = this.m_MaxCapacity;
            if (requiredLength < 0)
            {
                throw new OutOfMemoryException();
            }
            if (requiredLength > maxCapacity)
            {
                throw new ArgumentOutOfRangeException("requiredLength", Environment.GetResourceString("ArgumentOutOfRange_SmallCapacity"));
            }
            int capacity = currentString.Capacity * 2;
            if (capacity < requiredLength)
            {
                capacity = requiredLength;
            }
            if (capacity > maxCapacity)
            {
                capacity = maxCapacity;
            }
            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException("newCapacity", Environment.GetResourceString("ArgumentOutOfRange_NegativeCapacity"));
            }
            return string.GetStringForStringBuilder(currentString, capacity);
        }

        private string GetThreadSafeString(out IntPtr tid)
        {
            string stringValue = this.m_StringValue;
            tid = Thread.InternalGetCurrentThread();
            if (this.m_currentThread == tid)
            {
                return stringValue;
            }
            return string.GetStringForStringBuilder(stringValue, stringValue.Capacity);
        }

        public StringBuilder Insert(int index, bool value)
        {
            return this.Insert(index, value.ToString(), 1);
        }

        public StringBuilder Insert(int index, byte value)
        {
            return this.Insert(index, value.ToString(CultureInfo.CurrentCulture), 1);
        }

        public StringBuilder Insert(int index, char value)
        {
            return this.Insert(index, char.ToString(value), 1);
        }

        public StringBuilder Insert(int index, decimal value)
        {
            return this.Insert(index, value.ToString(CultureInfo.CurrentCulture), 1);
        }

        public StringBuilder Insert(int index, double value)
        {
            return this.Insert(index, value.ToString(CultureInfo.CurrentCulture), 1);
        }

        public StringBuilder Insert(int index, short value)
        {
            return this.Insert(index, value.ToString(CultureInfo.CurrentCulture), 1);
        }

        public StringBuilder Insert(int index, char[] value)
        {
            if (value == null)
            {
                return this.Insert(index, value, 0, 0);
            }
            return this.Insert(index, value, 0, value.Length);
        }

        public StringBuilder Insert(int index, int value)
        {
            return this.Insert(index, value.ToString(CultureInfo.CurrentCulture), 1);
        }

        public StringBuilder Insert(int index, long value)
        {
            return this.Insert(index, value.ToString(CultureInfo.CurrentCulture), 1);
        }

        public StringBuilder Insert(int index, object value)
        {
            if (value == null)
            {
                return this;
            }
            return this.Insert(index, value.ToString(), 1);
        }

        [CLSCompliant(false)]
        public StringBuilder Insert(int index, sbyte value)
        {
            return this.Insert(index, value.ToString(CultureInfo.CurrentCulture), 1);
        }

        public StringBuilder Insert(int index, float value)
        {
            return this.Insert(index, value.ToString(CultureInfo.CurrentCulture), 1);
        }

        public StringBuilder Insert(int index, string value)
        {
            if (value == null)
            {
                return this.Insert(index, value, 0);
            }
            return this.Insert(index, value, 1);
        }

        [CLSCompliant(false)]
        public StringBuilder Insert(int index, ushort value)
        {
            return this.Insert(index, value.ToString(CultureInfo.CurrentCulture), 1);
        }

        [CLSCompliant(false)]
        public StringBuilder Insert(int index, uint value)
        {
            return this.Insert(index, value.ToString(CultureInfo.CurrentCulture), 1);
        }

        [CLSCompliant(false)]
        public StringBuilder Insert(int index, ulong value)
        {
            return this.Insert(index, value.ToString(CultureInfo.CurrentCulture), 1);
        }

        public StringBuilder Insert(int index, string value, int count)
        {
            IntPtr ptr;
            string threadSafeString = this.GetThreadSafeString(out ptr);
            int length = threadSafeString.Length;
            if ((index < 0) || (index > length))
            {
                throw new ArgumentOutOfRangeException("index", Environment.GetResourceString("ArgumentOutOfRange_Index"));
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
            }
            if (((value != null) && (value.Length != 0)) && (count != 0))
            {
                int num2;
                try
                {
                    num2 = length + (value.Length * count);
                }
                catch (OverflowException)
                {
                    throw new OutOfMemoryException();
                }
                if (this.NeedsAllocation(threadSafeString, num2))
                {
                    string newString = this.GetNewString(threadSafeString, num2);
                    newString.InsertInPlace(index, value, count, length, num2);
                    this.ReplaceString(ptr, newString);
                }
                else
                {
                    threadSafeString.InsertInPlace(index, value, count, length, num2);
                    this.ReplaceString(ptr, threadSafeString);
                }
            }
            return this;
        }

        public StringBuilder Insert(int index, char[] value, int startIndex, int charCount)
        {
            IntPtr ptr;
            string threadSafeString = this.GetThreadSafeString(out ptr);
            int length = threadSafeString.Length;
            if ((index < 0) || (index > length))
            {
                throw new ArgumentOutOfRangeException("index", Environment.GetResourceString("ArgumentOutOfRange_Index"));
            }
            if (value == null)
            {
                if ((startIndex != 0) || (charCount != 0))
                {
                    throw new ArgumentNullException(Environment.GetResourceString("ArgumentNull_String"));
                }
                return this;
            }
            if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_StartIndex"));
            }
            if (charCount < 0)
            {
                throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_GenericPositive"));
            }
            if (startIndex > (value.Length - charCount))
            {
                throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_Index"));
            }
            if (charCount != 0)
            {
                int requiredLength = length + charCount;
                if (this.NeedsAllocation(threadSafeString, requiredLength))
                {
                    string newString = this.GetNewString(threadSafeString, requiredLength);
                    newString.InsertInPlace(index, value, startIndex, charCount, length, requiredLength);
                    this.ReplaceString(ptr, newString);
                }
                else
                {
                    threadSafeString.InsertInPlace(index, value, startIndex, charCount, length, requiredLength);
                    this.ReplaceString(ptr, threadSafeString);
                }
            }
            return this;
        }

        private bool NeedsAllocation(string currentString, int requiredLength)
        {
            return (currentString.ArrayLength <= requiredLength);
        }

        public StringBuilder Remove(int startIndex, int length)
        {
            IntPtr ptr;
            string threadSafeString = this.GetThreadSafeString(out ptr);
            int currentLength = threadSafeString.Length;
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length", Environment.GetResourceString("ArgumentOutOfRange_NegativeLength"));
            }
            if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_StartIndex"));
            }
            if (length > (currentLength - startIndex))
            {
                throw new ArgumentOutOfRangeException("index", Environment.GetResourceString("ArgumentOutOfRange_Index"));
            }
            threadSafeString.RemoveInPlace(startIndex, length, currentLength);
            this.ReplaceString(ptr, threadSafeString);
            return this;
        }

        public StringBuilder Replace(char oldChar, char newChar)
        {
            return this.Replace(oldChar, newChar, 0, this.Length);
        }

        public StringBuilder Replace(string oldValue, string newValue)
        {
            return this.Replace(oldValue, newValue, 0, this.Length);
        }

        public StringBuilder Replace(char oldChar, char newChar, int startIndex, int count)
        {
            IntPtr ptr;
            string threadSafeString = this.GetThreadSafeString(out ptr);
            int length = threadSafeString.Length;
            if (startIndex > length)
            {
                throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_Index"));
            }
            if ((count < 0) || (startIndex > (length - count)))
            {
                throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_Index"));
            }
            threadSafeString.ReplaceCharInPlace(oldChar, newChar, startIndex, count, length);
            this.ReplaceString(ptr, threadSafeString);
            return this;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern StringBuilder Replace(string oldValue, string newValue, int startIndex, int count);
        private void ReplaceString(IntPtr tid, string value)
        {
            this.m_currentThread = tid;
            this.m_StringValue = value;
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            info.AddValue("m_MaxCapacity", this.m_MaxCapacity);
            info.AddValue("Capacity", this.Capacity);
            info.AddValue("m_StringValue", this.m_StringValue);
            info.AddValue("m_currentThread", 0);
        }

        public override string ToString()
        {
            string stringValue = this.m_StringValue;
            if (this.m_currentThread != Thread.InternalGetCurrentThread())
            {
                return string.InternalCopy(stringValue);
            }
            if ((2 * stringValue.Length) < stringValue.ArrayLength)
            {
                return string.InternalCopy(stringValue);
            }
            stringValue.ClearPostNullChar();
            this.m_currentThread = IntPtr.Zero;
            return stringValue;
        }

        public string ToString(int startIndex, int length)
        {
            return this.m_StringValue.InternalSubStringWithChecks(startIndex, length, true);
        }

        [Conditional("_DEBUG")]
        private void VerifyClassInvariant()
        {
        }

        public int Capacity
        {
            get
            {
                return this.m_StringValue.Capacity;
            }
            set
            {
                IntPtr ptr;
                string threadSafeString = this.GetThreadSafeString(out ptr);
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", Environment.GetResourceString("ArgumentOutOfRange_NegativeCapacity"));
                }
                if (value < threadSafeString.Length)
                {
                    throw new ArgumentOutOfRangeException("value", Environment.GetResourceString("ArgumentOutOfRange_SmallCapacity"));
                }
                if (value > this.MaxCapacity)
                {
                    throw new ArgumentOutOfRangeException("value", Environment.GetResourceString("ArgumentOutOfRange_Capacity"));
                }
                int capacity = threadSafeString.Capacity;
                if (value != capacity)
                {
                    string stringForStringBuilder = string.GetStringForStringBuilder(threadSafeString, value);
                    this.ReplaceString(ptr, stringForStringBuilder);
                }
            }
        }

        public char this[int index]
        {
            get
            {
                return this.m_StringValue[index];
            }
            set
            {
                IntPtr ptr;
                string threadSafeString = this.GetThreadSafeString(out ptr);
                threadSafeString.SetChar(index, value);
                this.ReplaceString(ptr, threadSafeString);
            }
        }

        public int Length
        {
            get
            {
                return this.m_StringValue.Length;
            }
            set
            {
                IntPtr ptr;
                string threadSafeString = this.GetThreadSafeString(out ptr);
                if (value == 0)
                {
                    threadSafeString.SetLength(0);
                    this.ReplaceString(ptr, threadSafeString);
                }
                else
                {
                    int length = threadSafeString.Length;
                    int index = value;
                    if (index < 0)
                    {
                        throw new ArgumentOutOfRangeException("newlength", Environment.GetResourceString("ArgumentOutOfRange_NegativeLength"));
                    }
                    if (index > this.MaxCapacity)
                    {
                        throw new ArgumentOutOfRangeException("capacity", Environment.GetResourceString("ArgumentOutOfRange_SmallCapacity"));
                    }
                    if (index != length)
                    {
                        if (index <= threadSafeString.Capacity)
                        {
                            if (index > length)
                            {
                                for (int i = length; i < index; i++)
                                {
                                    threadSafeString.InternalSetCharNoBoundsCheck(i, '\0');
                                }
                            }
                            threadSafeString.InternalSetCharNoBoundsCheck(index, '\0');
                            threadSafeString.SetLength(index);
                            this.ReplaceString(ptr, threadSafeString);
                        }
                        else
                        {
                            int capacity = (index > threadSafeString.Capacity) ? index : threadSafeString.Capacity;
                            string stringForStringBuilder = string.GetStringForStringBuilder(threadSafeString, capacity);
                            stringForStringBuilder.SetLength(index);
                            this.ReplaceString(ptr, stringForStringBuilder);
                        }
                    }
                }
            }
        }

        public int MaxCapacity
        {
            get
            {
                return this.m_MaxCapacity;
            }
        }
    }
}

