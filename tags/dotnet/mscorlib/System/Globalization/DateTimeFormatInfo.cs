namespace System.Globalization
{
    using Microsoft.Win32;
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading;

    [Serializable, ComVisible(true)]
    public sealed class DateTimeFormatInfo : ICloneable, IFormatProvider
    {
        internal string[] abbreviatedDayNames;
        internal string[] abbreviatedMonthNames;
        internal string[] allLongDatePatterns;
        internal string[] allLongTimePatterns;
        internal string[] allShortDatePatterns;
        internal string[] allShortTimePatterns;
        [NonSerialized]
        internal string[] allYearMonthPatterns;
        internal string amDesignator;
        internal bool bUseCalendarInfo;
        internal const int CAL_SCALNAME = 2;
        internal System.Globalization.Calendar calendar;
        internal int calendarWeekRule;
        internal const string ChineseHourSuff = "时";
        internal const string CJKDaySuff = "日";
        internal const string CJKHourSuff = "時";
        internal const string CJKMinuteSuff = "分";
        internal const string CJKMonthSuff = "月";
        internal const string CJKSecondSuff = "秒";
        internal const string CJKYearSuff = "年";
        private int CultureID;
        internal string dateSeparator;
        private const string dateSeparatorOrTimeZoneOffset = "-";
        [OptionalField(VersionAdded=3)]
        internal string dateTimeOffsetPattern;
        internal string[] dayNames;
        private const int DEFAULT_ALL_DATETIMES_SIZE = 0x84;
        internal const string EnglishLangName = "en";
        internal int firstDayOfWeek;
        [OptionalField(VersionAdded=2)]
        internal DateTimeFormatFlags formatFlags;
        internal string fullDateTimePattern;
        internal string generalLongTimePattern;
        internal string generalShortTimePattern;
        [OptionalField(VersionAdded=2)]
        internal string[] genitiveMonthNames;
        internal const DateTimeStyles InvalidDateTimeStyles = ~(DateTimeStyles.RoundtripKind | DateTimeStyles.AssumeUniversal | DateTimeStyles.AssumeLocal | DateTimeStyles.AdjustToUniversal | DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AllowWhiteSpaces);
        private const string invariantDateSeparator = "/";
        private static DateTimeFormatInfo invariantInfo;
        private const string invariantTimeSeparator = ":";
        internal const string JapaneseLangName = "ja";
        internal const string KoreanDaySuff = "일";
        internal const string KoreanHourSuff = "시";
        internal const string KoreanLangName = "ko";
        internal const string KoreanMinuteSuff = "분";
        internal const string KoreanMonthSuff = "월";
        internal const string KoreanSecondSuff = "초";
        internal const string KoreanYearSuff = "년";
        [OptionalField(VersionAdded=2)]
        internal string[] leapYearMonthNames;
        internal const string LocalTimeMark = "T";
        internal string longDatePattern;
        internal string longTimePattern;
        internal string[] m_abbrevEnglishEraNames;
        internal string[] m_abbrevEraNames;
        private static Hashtable m_calendarNativeNames;
        [NonSerialized]
        internal System.Globalization.CompareInfo m_compareInfo;
        [NonSerialized]
        internal CultureTableRecord m_cultureTableRecord;
        internal string[] m_dateWords;
        [NonSerialized]
        private TokenHashValue[] m_dtfiTokenHash;
        internal string[] m_eraNames;
        [OptionalField(VersionAdded=2)]
        internal string[] m_genitiveAbbreviatedMonthNames;
        internal bool m_isDefaultCalendar;
        internal bool m_isReadOnly;
        private static DateTimeFormatInfo m_jajpDTFI = null;
        [NonSerialized]
        internal string m_langName;
        [OptionalField(VersionAdded=2)]
        internal string m_name;
        [NonSerialized]
        private bool m_scanDateWords;
        [OptionalField(VersionAdded=2)]
        internal string[] m_superShortDayNames;
        private bool m_useUserOverride;
        private static DateTimeFormatInfo m_zhtwDTFI = null;
        internal string monthDayPattern;
        internal string[] monthNames;
        private static char[] MonthSpaces = new char[] { ' ', '\x00a0' };
        private int nDataItem;
        internal int[] optionalCalendars;
        internal string pmDesignator;
        internal const string rfc1123Pattern = "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'";
        private static object s_InternalSyncObject;
        private const int SECOND_PRIME = 0xc5;
        internal string shortDatePattern;
        internal string shortTimePattern;
        internal const string sortableDateTimePattern = "yyyy'-'MM'-'dd'T'HH':'mm':'ss";
        internal string timeSeparator;
        private const int TOKEN_HASH_SIZE = 0xc7;
        internal const string universalSortableDateTimePattern = "yyyy'-'MM'-'dd HH':'mm':'ss'Z'";
        internal string yearMonthPattern;

        public DateTimeFormatInfo()
        {
            this.firstDayOfWeek = -1;
            this.calendarWeekRule = -1;
            this.formatFlags = ~DateTimeFormatFlags.None;
            this.m_cultureTableRecord = CultureInfo.InvariantCulture.m_cultureTableRecord;
            this.m_isDefaultCalendar = true;
            this.calendar = GregorianCalendar.GetDefaultInstance();
            this.InitializeOverridableProperties();
        }

        internal DateTimeFormatInfo(CultureTableRecord cultureTable, int cultureID, System.Globalization.Calendar cal)
        {
            this.firstDayOfWeek = -1;
            this.calendarWeekRule = -1;
            this.formatFlags = ~DateTimeFormatFlags.None;
            this.m_cultureTableRecord = cultureTable;
            this.Calendar = cal;
        }

        internal string[] AddDefaultFormat(string[] datePatterns, string defaultDateFormat)
        {
            string[] destinationArray = new string[datePatterns.Length + 1];
            destinationArray[0] = defaultDateFormat;
            Array.Copy(datePatterns, 0, destinationArray, 1, datePatterns.Length);
            this.m_scanDateWords = true;
            return destinationArray;
        }

        private void AddMonthNames(TokenHashValue[] temp, string monthPostfix)
        {
            for (int i = 1; i <= 13; i++)
            {
                string monthName = this.GetMonthName(i);
                if (monthName.Length > 0)
                {
                    if (monthPostfix != null)
                    {
                        this.InsertHash(temp, monthName + monthPostfix, TokenType.MonthToken, i);
                    }
                    else
                    {
                        this.InsertHash(temp, monthName, TokenType.MonthToken, i);
                    }
                }
                monthName = this.GetAbbreviatedMonthName(i);
                this.InsertHash(temp, monthName, TokenType.MonthToken, i);
            }
        }

        private static int CalendarIdToCultureId(int calendarId)
        {
            switch (calendarId)
            {
                case 2:
                    return 0x429;

                case 3:
                    return 0x411;

                case 4:
                    return 0x404;

                case 5:
                    return 0x412;

                case 6:
                case 10:
                case 0x17:
                    return 0x401;

                case 7:
                    return 0x41e;

                case 8:
                    return 0x40d;

                case 9:
                    return 0x1401;

                case 11:
                case 12:
                    return 0x801;
            }
            return 0;
        }

        private void CheckNullValue(string[] values, int length)
        {
            for (int i = 0; i < length; i++)
            {
                if (values[i] == null)
                {
                    throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_ArrayValue"));
                }
            }
        }

        private void ClearTokenHashTable(bool scanDateWords)
        {
            this.m_dtfiTokenHash = null;
            this.m_dateWords = null;
            this.m_scanDateWords = scanDateWords;
            this.formatFlags = ~DateTimeFormatFlags.None;
        }

        public object Clone()
        {
            DateTimeFormatInfo info = (DateTimeFormatInfo) base.MemberwiseClone();
            info.calendar = (System.Globalization.Calendar) this.Calendar.Clone();
            info.m_isReadOnly = false;
            return info;
        }

        internal TokenHashValue[] CreateTokenHashTable()
        {
            TokenHashValue[] dtfiTokenHash = this.m_dtfiTokenHash;
            if (dtfiTokenHash == null)
            {
                dtfiTokenHash = new TokenHashValue[0xc7];
                this.InsertHash(dtfiTokenHash, ",", TokenType.IgnorableSymbol, 0);
                this.InsertHash(dtfiTokenHash, ".", TokenType.IgnorableSymbol, 0);
                this.InsertHash(dtfiTokenHash, this.TimeSeparator, TokenType.SEP_Time, 0);
                this.InsertHash(dtfiTokenHash, this.AMDesignator, TokenType.SEP_Am | TokenType.Am, 0);
                this.InsertHash(dtfiTokenHash, this.PMDesignator, TokenType.SEP_Pm | TokenType.Pm, 1);
                if (this.CultureName.Equals("sq-AL"))
                {
                    this.InsertHash(dtfiTokenHash, "." + this.AMDesignator, TokenType.SEP_Am | TokenType.Am, 0);
                    this.InsertHash(dtfiTokenHash, "." + this.PMDesignator, TokenType.SEP_Pm | TokenType.Pm, 1);
                }
                this.InsertHash(dtfiTokenHash, "年", TokenType.SEP_YearSuff, 0);
                this.InsertHash(dtfiTokenHash, "년", TokenType.SEP_YearSuff, 0);
                this.InsertHash(dtfiTokenHash, "月", TokenType.SEP_MonthSuff, 0);
                this.InsertHash(dtfiTokenHash, "월", TokenType.SEP_MonthSuff, 0);
                this.InsertHash(dtfiTokenHash, "日", TokenType.SEP_DaySuff, 0);
                this.InsertHash(dtfiTokenHash, "일", TokenType.SEP_DaySuff, 0);
                this.InsertHash(dtfiTokenHash, "時", TokenType.SEP_HourSuff, 0);
                this.InsertHash(dtfiTokenHash, "时", TokenType.SEP_HourSuff, 0);
                this.InsertHash(dtfiTokenHash, "分", TokenType.SEP_MinuteSuff, 0);
                this.InsertHash(dtfiTokenHash, "秒", TokenType.SEP_SecondSuff, 0);
                if (this.LanguageName.Equals("ko"))
                {
                    this.InsertHash(dtfiTokenHash, "시", TokenType.SEP_HourSuff, 0);
                    this.InsertHash(dtfiTokenHash, "분", TokenType.SEP_MinuteSuff, 0);
                    this.InsertHash(dtfiTokenHash, "초", TokenType.SEP_SecondSuff, 0);
                }
                if (this.CultureName.Equals("ky-KG"))
                {
                    this.InsertHash(dtfiTokenHash, "-", TokenType.IgnorableSymbol, 0);
                }
                else
                {
                    this.InsertHash(dtfiTokenHash, "-", TokenType.SEP_DateOrOffset, 0);
                }
                string[] clonedAllLongDatePatterns = null;
                DateTimeFormatInfoScanner scanner = null;
                if (!this.m_scanDateWords)
                {
                    clonedAllLongDatePatterns = this.ClonedAllLongDatePatterns;
                }
                if (this.m_scanDateWords || this.m_cultureTableRecord.IsSynthetic)
                {
                    scanner = new DateTimeFormatInfoScanner();
                    this.m_dateWords = clonedAllLongDatePatterns = scanner.GetDateWordsOfDTFI(this);
                    DateTimeFormatFlags formatFlags = this.FormatFlags;
                    this.m_scanDateWords = false;
                }
                else
                {
                    clonedAllLongDatePatterns = this.DateWords;
                }
                bool flag = false;
                string monthPostfix = null;
                if (clonedAllLongDatePatterns != null)
                {
                    for (int num = 0; num < clonedAllLongDatePatterns.Length; num++)
                    {
                        switch (clonedAllLongDatePatterns[num][0])
                        {
                            case 0xe000:
                                monthPostfix = clonedAllLongDatePatterns[num].Substring(1);
                                this.AddMonthNames(dtfiTokenHash, monthPostfix);
                                break;

                            case 0xe001:
                            {
                                string str = clonedAllLongDatePatterns[num].Substring(1);
                                this.InsertHash(dtfiTokenHash, str, TokenType.IgnorableSymbol, 0);
                                if (this.DateSeparator.Trim(null).Equals(str))
                                {
                                    flag = true;
                                }
                                break;
                            }
                            default:
                                this.InsertHash(dtfiTokenHash, clonedAllLongDatePatterns[num], TokenType.DateWordToken, 0);
                                if (this.CultureName.Equals("eu-ES"))
                                {
                                    this.InsertHash(dtfiTokenHash, "." + clonedAllLongDatePatterns[num], TokenType.DateWordToken, 0);
                                }
                                break;
                        }
                    }
                }
                if (!flag)
                {
                    this.InsertHash(dtfiTokenHash, this.DateSeparator, TokenType.SEP_Date, 0);
                }
                this.AddMonthNames(dtfiTokenHash, null);
                for (int i = 1; i <= 13; i++)
                {
                    this.InsertHash(dtfiTokenHash, this.GetAbbreviatedMonthName(i), TokenType.MonthToken, i);
                }
                if (this.CultureName.Equals("gl-ES"))
                {
                    for (int num3 = 1; num3 <= 13; num3++)
                    {
                        string monthName = this.GetMonthName(num3);
                        if (monthName.Length > 0)
                        {
                            this.InsertHash(dtfiTokenHash, monthName + "de", TokenType.MonthToken, num3);
                        }
                    }
                }
                if ((this.FormatFlags & DateTimeFormatFlags.UseGenitiveMonth) != DateTimeFormatFlags.None)
                {
                    for (int num4 = 1; num4 <= 13; num4++)
                    {
                        string str4 = this.internalGetMonthName(num4, MonthNameStyles.Genitive, false);
                        this.InsertHash(dtfiTokenHash, str4, TokenType.MonthToken, num4);
                    }
                }
                if ((this.FormatFlags & DateTimeFormatFlags.UseLeapYearMonth) != DateTimeFormatFlags.None)
                {
                    for (int num5 = 1; num5 <= 13; num5++)
                    {
                        string str5 = this.internalGetMonthName(num5, MonthNameStyles.LeapYear, false);
                        this.InsertHash(dtfiTokenHash, str5, TokenType.MonthToken, num5);
                    }
                }
                for (int j = 0; j < 7; j++)
                {
                    string dayName = this.GetDayName((DayOfWeek) j);
                    this.InsertHash(dtfiTokenHash, dayName, TokenType.DayOfWeekToken, j);
                    dayName = this.GetAbbreviatedDayName((DayOfWeek) j);
                    this.InsertHash(dtfiTokenHash, dayName, TokenType.DayOfWeekToken, j);
                }
                int[] eras = this.calendar.Eras;
                for (int k = 1; k <= eras.Length; k++)
                {
                    this.InsertHash(dtfiTokenHash, this.GetEraName(k), TokenType.EraToken, k);
                    this.InsertHash(dtfiTokenHash, this.GetAbbreviatedEraName(k), TokenType.EraToken, k);
                }
                if (this.LanguageName.Equals("ja"))
                {
                    for (int num8 = 0; num8 < 7; num8++)
                    {
                        string str7 = "(" + this.GetAbbreviatedDayName((DayOfWeek) num8) + ")";
                        this.InsertHash(dtfiTokenHash, str7, TokenType.DayOfWeekToken, num8);
                    }
                    if (this.Calendar.GetType() != typeof(JapaneseCalendar))
                    {
                        DateTimeFormatInfo japaneseCalendarDTFI = GetJapaneseCalendarDTFI();
                        for (int num9 = 1; num9 <= japaneseCalendarDTFI.Calendar.Eras.Length; num9++)
                        {
                            this.InsertHash(dtfiTokenHash, japaneseCalendarDTFI.GetEraName(num9), TokenType.JapaneseEraToken, num9);
                            this.InsertHash(dtfiTokenHash, japaneseCalendarDTFI.GetAbbreviatedEraName(num9), TokenType.JapaneseEraToken, num9);
                            this.InsertHash(dtfiTokenHash, japaneseCalendarDTFI.AbbreviatedEnglishEraNames[num9 - 1], TokenType.JapaneseEraToken, num9);
                        }
                    }
                }
                else if (this.CultureName.Equals("zh-TW"))
                {
                    DateTimeFormatInfo taiwanCalendarDTFI = GetTaiwanCalendarDTFI();
                    for (int num10 = 1; num10 <= taiwanCalendarDTFI.Calendar.Eras.Length; num10++)
                    {
                        if (taiwanCalendarDTFI.GetEraName(num10).Length > 0)
                        {
                            this.InsertHash(dtfiTokenHash, taiwanCalendarDTFI.GetEraName(num10), TokenType.TEraToken, num10);
                        }
                    }
                }
                this.InsertHash(dtfiTokenHash, InvariantInfo.AMDesignator, TokenType.SEP_Am | TokenType.Am, 0);
                this.InsertHash(dtfiTokenHash, InvariantInfo.PMDesignator, TokenType.SEP_Pm | TokenType.Pm, 1);
                for (int m = 1; m <= 12; m++)
                {
                    string abbreviatedMonthName = InvariantInfo.GetMonthName(m);
                    this.InsertHash(dtfiTokenHash, abbreviatedMonthName, TokenType.MonthToken, m);
                    abbreviatedMonthName = InvariantInfo.GetAbbreviatedMonthName(m);
                    this.InsertHash(dtfiTokenHash, abbreviatedMonthName, TokenType.MonthToken, m);
                }
                for (int n = 0; n < 7; n++)
                {
                    string abbreviatedDayName = InvariantInfo.GetDayName((DayOfWeek) n);
                    this.InsertHash(dtfiTokenHash, abbreviatedDayName, TokenType.DayOfWeekToken, n);
                    abbreviatedDayName = InvariantInfo.GetAbbreviatedDayName((DayOfWeek) n);
                    this.InsertHash(dtfiTokenHash, abbreviatedDayName, TokenType.DayOfWeekToken, n);
                }
                for (int num13 = 0; num13 < this.AbbreviatedEnglishEraNames.Length; num13++)
                {
                    this.InsertHash(dtfiTokenHash, this.AbbreviatedEnglishEraNames[num13], TokenType.EraToken, num13 + 1);
                }
                this.InsertHash(dtfiTokenHash, "T", TokenType.SEP_LocalTimeMark, 0);
                this.InsertHash(dtfiTokenHash, "GMT", TokenType.TimeZoneToken, 0);
                this.InsertHash(dtfiTokenHash, "Z", TokenType.TimeZoneToken, 0);
                this.InsertHash(dtfiTokenHash, "/", TokenType.SEP_Date, 0);
                this.InsertHash(dtfiTokenHash, ":", TokenType.SEP_Time, 0);
                this.m_dtfiTokenHash = dtfiTokenHash;
            }
            return dtfiTokenHash;
        }

        public string GetAbbreviatedDayName(DayOfWeek dayofweek)
        {
            if ((dayofweek < DayOfWeek.Sunday) || (dayofweek > DayOfWeek.Saturday))
            {
                throw new ArgumentOutOfRangeException("dayofweek", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("ArgumentOutOfRange_Range"), new object[] { DayOfWeek.Sunday, DayOfWeek.Saturday }));
            }
            return this.GetAbbreviatedDayOfWeekNames()[(int) dayofweek];
        }

        private string[] GetAbbreviatedDayOfWeekNames()
        {
            if ((this.abbreviatedDayNames == null) && (this.abbreviatedDayNames == null))
            {
                string[] sABBREVDAYNAMES = null;
                if (!this.m_isDefaultCalendar)
                {
                    sABBREVDAYNAMES = CalendarTable.Default.SABBREVDAYNAMES(this.Calendar.ID);
                }
                if (((sABBREVDAYNAMES == null) || (sABBREVDAYNAMES.Length == 0)) || (sABBREVDAYNAMES[0].Length == 0))
                {
                    sABBREVDAYNAMES = this.m_cultureTableRecord.SABBREVDAYNAMES;
                }
                Thread.MemoryBarrier();
                this.abbreviatedDayNames = sABBREVDAYNAMES;
            }
            return this.abbreviatedDayNames;
        }

        public string GetAbbreviatedEraName(int era)
        {
            if (this.AbbreviatedEraNames.Length == 0)
            {
                return this.GetEraName(era);
            }
            if (era == 0)
            {
                era = this.Calendar.CurrentEraValue;
            }
            if ((--era >= this.m_abbrevEraNames.Length) || (era < 0))
            {
                throw new ArgumentOutOfRangeException("era", Environment.GetResourceString("ArgumentOutOfRange_InvalidEraValue"));
            }
            return this.m_abbrevEraNames[era];
        }

        public string GetAbbreviatedMonthName(int month)
        {
            if ((month < 1) || (month > 13))
            {
                throw new ArgumentOutOfRangeException("month", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("ArgumentOutOfRange_Range"), new object[] { 1, 13 }));
            }
            return this.GetAbbreviatedMonthNames()[month - 1];
        }

        private string[] GetAbbreviatedMonthNames()
        {
            if ((this.abbreviatedMonthNames == null) && (this.abbreviatedMonthNames == null))
            {
                string[] sABBREVMONTHNAMES = null;
                if (!this.m_isDefaultCalendar)
                {
                    sABBREVMONTHNAMES = CalendarTable.Default.SABBREVMONTHNAMES(this.Calendar.ID);
                }
                if (((sABBREVMONTHNAMES == null) || (sABBREVMONTHNAMES.Length == 0)) || (sABBREVMONTHNAMES[0].Length == 0))
                {
                    sABBREVMONTHNAMES = this.m_cultureTableRecord.SABBREVMONTHNAMES;
                }
                Thread.MemoryBarrier();
                this.abbreviatedMonthNames = sABBREVMONTHNAMES;
            }
            return this.abbreviatedMonthNames;
        }

        public string[] GetAllDateTimePatterns()
        {
            ArrayList list = new ArrayList(0x84);
            for (int i = 0; i < DateTimeFormat.allStandardFormats.Length; i++)
            {
                string[] allDateTimePatterns = this.GetAllDateTimePatterns(DateTimeFormat.allStandardFormats[i]);
                for (int j = 0; j < allDateTimePatterns.Length; j++)
                {
                    list.Add(allDateTimePatterns[j]);
                }
            }
            string[] array = new string[list.Count];
            list.CopyTo(0, array, 0, list.Count);
            return array;
        }

        public string[] GetAllDateTimePatterns(char format)
        {
            switch (format)
            {
                case 'D':
                    return this.ClonedAllLongDatePatterns;

                case 'F':
                    return this.GetCombinedPatterns(this.ClonedAllLongDatePatterns, this.ClonedAllLongTimePatterns, " ");

                case 'G':
                    return this.GetCombinedPatterns(this.ClonedAllShortDatePatterns, this.ClonedAllLongTimePatterns, " ");

                case 'M':
                case 'm':
                    return new string[] { this.MonthDayPattern };

                case 'O':
                case 'o':
                    return new string[] { "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK" };

                case 'R':
                case 'r':
                    return new string[] { "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'" };

                case 'T':
                    return this.ClonedAllLongTimePatterns;

                case 'U':
                    return this.GetCombinedPatterns(this.ClonedAllLongDatePatterns, this.ClonedAllLongTimePatterns, " ");

                case 'd':
                    return this.ClonedAllShortDatePatterns;

                case 'f':
                    return this.GetCombinedPatterns(this.ClonedAllLongDatePatterns, this.ClonedAllShortTimePatterns, " ");

                case 'g':
                    return this.GetCombinedPatterns(this.ClonedAllShortDatePatterns, this.ClonedAllShortTimePatterns, " ");

                case 'Y':
                case 'y':
                    return this.ClonedAllYearMonthPatterns;

                case 's':
                    return new string[] { "yyyy'-'MM'-'dd'T'HH':'mm':'ss" };

                case 't':
                    return this.ClonedAllShortTimePatterns;

                case 'u':
                    return new string[] { this.UniversalSortableDateTimePattern };
            }
            throw new ArgumentException(Environment.GetResourceString("Argument_BadFormatSpecifier"), "format");
        }

        internal static string GetCalendarInfo(int culture, int calendar, int calType)
        {
            int capacity = Win32Native.GetCalendarInfo(culture, calendar, calType, null, 0, IntPtr.Zero);
            if (capacity > 0)
            {
                StringBuilder lpCalData = new StringBuilder(capacity);
                capacity = Win32Native.GetCalendarInfo(culture, calendar, calType, lpCalData, capacity, IntPtr.Zero);
                if (capacity > 0)
                {
                    return lpCalData.ToString(0, capacity - 1);
                }
            }
            return null;
        }

        private string GetCalendarNativeNameFallback(int calendarId)
        {
            if (m_calendarNativeNames == null)
            {
                lock (InternalSyncObject)
                {
                    if (m_calendarNativeNames == null)
                    {
                        m_calendarNativeNames = new Hashtable();
                    }
                }
            }
            string str = (string) m_calendarNativeNames[calendarId];
            if (str != null)
            {
                return str;
            }
            string str2 = string.Empty;
            int cultureId = CalendarIdToCultureId(calendarId);
            if (cultureId != 0)
            {
                string[] sNATIVECALNAMES = new CultureTableRecord(cultureId, false).SNATIVECALNAMES;
                int index = this.calendar.ID - 1;
                if (((index < sNATIVECALNAMES.Length) && (sNATIVECALNAMES[index].Length > 0)) && (sNATIVECALNAMES[index][0] != 0xfeff))
                {
                    str2 = sNATIVECALNAMES[index];
                }
            }
            lock (InternalSyncObject)
            {
                if (m_calendarNativeNames[calendarId] == null)
                {
                    m_calendarNativeNames[calendarId] = str2;
                }
            }
            return str2;
        }

        internal string[] GetCombinedPatterns(string[] patterns1, string[] patterns2, string connectString)
        {
            string[] strArray = new string[patterns1.Length * patterns2.Length];
            for (int i = 0; i < patterns1.Length; i++)
            {
                for (int j = 0; j < patterns2.Length; j++)
                {
                    strArray[(i * patterns2.Length) + j] = patterns1[i] + connectString + patterns2[j];
                }
            }
            return strArray;
        }

        public string GetDayName(DayOfWeek dayofweek)
        {
            if ((dayofweek < DayOfWeek.Sunday) || (dayofweek > DayOfWeek.Saturday))
            {
                throw new ArgumentOutOfRangeException("dayofweek", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("ArgumentOutOfRange_Range"), new object[] { DayOfWeek.Sunday, DayOfWeek.Saturday }));
            }
            return this.GetDayOfWeekNames()[(int) dayofweek];
        }

        private string[] GetDayOfWeekNames()
        {
            if ((this.dayNames == null) && (this.dayNames == null))
            {
                string[] sDAYNAMES = null;
                if (!this.m_isDefaultCalendar)
                {
                    sDAYNAMES = CalendarTable.Default.SDAYNAMES(this.Calendar.ID);
                }
                if (((sDAYNAMES == null) || (sDAYNAMES.Length == 0)) || (sDAYNAMES[0].Length == 0))
                {
                    sDAYNAMES = this.m_cultureTableRecord.SDAYNAMES;
                }
                Thread.MemoryBarrier();
                this.dayNames = sDAYNAMES;
            }
            return this.dayNames;
        }

        public int GetEra(string eraName)
        {
            if (eraName == null)
            {
                throw new ArgumentNullException("eraName", Environment.GetResourceString("ArgumentNull_String"));
            }
            for (int i = 0; i < this.EraNames.Length; i++)
            {
                if ((this.m_eraNames[i].Length > 0) && (string.Compare(eraName, this.m_eraNames[i], true, CultureInfo.CurrentCulture) == 0))
                {
                    return (i + 1);
                }
            }
            for (int j = 0; j < this.AbbreviatedEraNames.Length; j++)
            {
                if (string.Compare(eraName, this.m_abbrevEraNames[j], true, CultureInfo.CurrentCulture) == 0)
                {
                    return (j + 1);
                }
            }
            for (int k = 0; k < this.AbbreviatedEnglishEraNames.Length; k++)
            {
                if (string.Compare(eraName, this.m_abbrevEnglishEraNames[k], true, CultureInfo.InvariantCulture) == 0)
                {
                    return (k + 1);
                }
            }
            return -1;
        }

        public string GetEraName(int era)
        {
            if (era == 0)
            {
                era = this.Calendar.CurrentEraValue;
            }
            if ((--era >= this.EraNames.Length) || (era < 0))
            {
                throw new ArgumentOutOfRangeException("era", Environment.GetResourceString("ArgumentOutOfRange_InvalidEraValue"));
            }
            return this.m_eraNames[era];
        }

        public object GetFormat(Type formatType)
        {
            if (formatType != typeof(DateTimeFormatInfo))
            {
                return null;
            }
            return this;
        }

        public static DateTimeFormatInfo GetInstance(IFormatProvider provider)
        {
            DateTimeFormatInfo dateTimeInfo;
            CultureInfo info2 = provider as CultureInfo;
            if ((info2 != null) && !info2.m_isInherited)
            {
                dateTimeInfo = info2.dateTimeInfo;
                if (dateTimeInfo != null)
                {
                    return dateTimeInfo;
                }
                return info2.DateTimeFormat;
            }
            dateTimeInfo = provider as DateTimeFormatInfo;
            if (dateTimeInfo != null)
            {
                return dateTimeInfo;
            }
            if (provider != null)
            {
                dateTimeInfo = provider.GetFormat(typeof(DateTimeFormatInfo)) as DateTimeFormatInfo;
                if (dateTimeInfo != null)
                {
                    return dateTimeInfo;
                }
            }
            return CurrentInfo;
        }

        internal static DateTimeFormatInfo GetJapaneseCalendarDTFI()
        {
            DateTimeFormatInfo jajpDTFI = m_jajpDTFI;
            if (jajpDTFI == null)
            {
                jajpDTFI = new CultureInfo("ja-JP", false).DateTimeFormat;
                jajpDTFI.Calendar = JapaneseCalendar.GetDefaultInstance();
                m_jajpDTFI = jajpDTFI;
            }
            return jajpDTFI;
        }

        private string GetLongDatePattern(int calID)
        {
            if (!this.m_isDefaultCalendar)
            {
                return CalendarTable.Default.SLONGDATE(calID)[0];
            }
            return this.m_cultureTableRecord.SLONGDATE;
        }

        public string GetMonthName(int month)
        {
            if ((month < 1) || (month > 13))
            {
                throw new ArgumentOutOfRangeException("month", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("ArgumentOutOfRange_Range"), new object[] { 1, 13 }));
            }
            return this.GetMonthNames()[month - 1];
        }

        private string[] GetMonthNames()
        {
            if (this.monthNames == null)
            {
                string[] sMONTHNAMES = null;
                if (!this.m_isDefaultCalendar)
                {
                    sMONTHNAMES = CalendarTable.Default.SMONTHNAMES(this.Calendar.ID);
                }
                if (((sMONTHNAMES == null) || (sMONTHNAMES.Length == 0)) || (sMONTHNAMES[0].Length == 0))
                {
                    sMONTHNAMES = this.m_cultureTableRecord.SMONTHNAMES;
                }
                Thread.MemoryBarrier();
                this.monthNames = sMONTHNAMES;
            }
            return this.monthNames;
        }

        internal string GetShortDatePattern(int calID)
        {
            if (!this.m_isDefaultCalendar)
            {
                return CalendarTable.Default.SSHORTDATE(calID)[0];
            }
            return this.m_cultureTableRecord.SSHORTDATE;
        }

        [ComVisible(false)]
        public string GetShortestDayName(DayOfWeek dayOfWeek)
        {
            if ((dayOfWeek < DayOfWeek.Sunday) || (dayOfWeek > DayOfWeek.Saturday))
            {
                throw new ArgumentOutOfRangeException("dayOfWeek", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("ArgumentOutOfRange_Range"), new object[] { DayOfWeek.Sunday, DayOfWeek.Saturday }));
            }
            return this.internalGetSuperShortDayNames()[(int) dayOfWeek];
        }

        internal static DateTimeFormatInfo GetTaiwanCalendarDTFI()
        {
            DateTimeFormatInfo zhtwDTFI = m_zhtwDTFI;
            if (zhtwDTFI == null)
            {
                zhtwDTFI = new CultureInfo("zh-TW", false).DateTimeFormat;
                zhtwDTFI.Calendar = TaiwanCalendar.GetDefaultInstance();
                m_zhtwDTFI = zhtwDTFI;
            }
            return zhtwDTFI;
        }

        private string GetYearMonthPattern(int calID)
        {
            if (!this.m_isDefaultCalendar)
            {
                return CalendarTable.Default.SYEARMONTH(calID)[0];
            }
            return this.m_cultureTableRecord.SYEARMONTHS[0];
        }

        private void InitializeOverridableProperties()
        {
            if (this.amDesignator == null)
            {
                this.amDesignator = this.m_cultureTableRecord.S1159;
            }
            if (this.pmDesignator == null)
            {
                this.pmDesignator = this.m_cultureTableRecord.S2359;
            }
            if (this.longTimePattern == null)
            {
                this.longTimePattern = this.m_cultureTableRecord.STIMEFORMAT;
            }
            if (this.firstDayOfWeek == -1)
            {
                this.firstDayOfWeek = this.m_cultureTableRecord.IFIRSTDAYOFWEEK;
            }
            if (this.calendarWeekRule == -1)
            {
                this.calendarWeekRule = this.m_cultureTableRecord.IFIRSTWEEKOFYEAR;
            }
            if (this.yearMonthPattern == null)
            {
                this.yearMonthPattern = this.GetYearMonthPattern(this.calendar.ID);
            }
            if (this.shortDatePattern == null)
            {
                this.shortDatePattern = this.GetShortDatePattern(this.calendar.ID);
            }
            if (this.longDatePattern == null)
            {
                this.longDatePattern = this.GetLongDatePattern(this.calendar.ID);
            }
        }

        private void InsertAtCurrentHashNode(TokenHashValue[] hashTable, string str, char ch, TokenType tokenType, int tokenValue, int pos, int hashcode, int hashProbe)
        {
            TokenHashValue value2 = hashTable[hashcode];
            hashTable[hashcode] = new TokenHashValue(str, tokenType, tokenValue);
            while (++pos < 0xc7)
            {
                hashcode += hashProbe;
                if (hashcode >= 0xc7)
                {
                    hashcode -= 0xc7;
                }
                TokenHashValue value3 = hashTable[hashcode];
                if ((value3 == null) || (char.ToLower(value3.tokenString[0], CultureInfo.CurrentCulture) == ch))
                {
                    hashTable[hashcode] = value2;
                    if (value3 == null)
                    {
                        return;
                    }
                    value2 = value3;
                }
            }
        }

        private void InsertHash(TokenHashValue[] hashTable, string str, TokenType tokenType, int tokenValue)
        {
            TokenHashValue value2;
            if ((str == null) || (str.Length == 0))
            {
                return;
            }
            int pos = 0;
            if (char.IsWhiteSpace(str[0]) || char.IsWhiteSpace(str[str.Length - 1]))
            {
                str = str.Trim(null);
                if (str.Length == 0)
                {
                    return;
                }
            }
            char ch = char.ToLower(str[0], CultureInfo.CurrentCulture);
            int index = ch % '\x00c7';
            int hashProbe = 1 + (ch % '\x00c5');
        Label_0068:
            value2 = hashTable[index];
            if (value2 == null)
            {
                hashTable[index] = new TokenHashValue(str, tokenType, tokenValue);
            }
            else
            {
                if ((str.Length >= value2.tokenString.Length) && (string.Compare(str, 0, value2.tokenString, 0, value2.tokenString.Length, true, CultureInfo.CurrentCulture) == 0))
                {
                    if (str.Length > value2.tokenString.Length)
                    {
                        this.InsertAtCurrentHashNode(hashTable, str, ch, tokenType, tokenValue, pos, index, hashProbe);
                        return;
                    }
                    if ((tokenType & TokenType.SeparatorTokenMask) != (value2.tokenType & TokenType.SeparatorTokenMask))
                    {
                        value2.tokenType |= tokenType;
                        if (tokenValue != 0)
                        {
                            value2.tokenValue = tokenValue;
                        }
                    }
                }
                pos++;
                index += hashProbe;
                if (index >= 0xc7)
                {
                    index -= 0xc7;
                }
                if (pos < 0xc7)
                {
                    goto Label_0068;
                }
            }
        }

        private string[] internalGetGenitiveMonthNames(bool abbreviated)
        {
            if (abbreviated)
            {
                if (this.m_genitiveAbbreviatedMonthNames == null)
                {
                    if (this.m_isDefaultCalendar)
                    {
                        string[] sABBREVMONTHGENITIVENAMES = this.m_cultureTableRecord.SABBREVMONTHGENITIVENAMES;
                        if (sABBREVMONTHGENITIVENAMES.Length > 0)
                        {
                            this.m_genitiveAbbreviatedMonthNames = sABBREVMONTHGENITIVENAMES;
                        }
                        else
                        {
                            this.m_genitiveAbbreviatedMonthNames = this.GetAbbreviatedMonthNames();
                        }
                    }
                    else
                    {
                        this.m_genitiveAbbreviatedMonthNames = this.GetAbbreviatedMonthNames();
                    }
                }
                return this.m_genitiveAbbreviatedMonthNames;
            }
            if (this.genitiveMonthNames == null)
            {
                if (this.m_isDefaultCalendar)
                {
                    string[] sMONTHGENITIVENAMES = this.m_cultureTableRecord.SMONTHGENITIVENAMES;
                    if (sMONTHGENITIVENAMES.Length > 0)
                    {
                        this.genitiveMonthNames = sMONTHGENITIVENAMES;
                    }
                    else
                    {
                        this.genitiveMonthNames = this.GetMonthNames();
                    }
                }
                else
                {
                    this.genitiveMonthNames = this.GetMonthNames();
                }
            }
            return this.genitiveMonthNames;
        }

        internal string[] internalGetLeapYearMonthNames()
        {
            if (this.leapYearMonthNames == null)
            {
                if (this.m_isDefaultCalendar)
                {
                    this.leapYearMonthNames = this.GetMonthNames();
                }
                else
                {
                    string[] strArray = CalendarTable.Default.SLEAPYEARMONTHNAMES(this.Calendar.ID);
                    if (strArray.Length > 0)
                    {
                        this.leapYearMonthNames = strArray;
                    }
                    else
                    {
                        this.leapYearMonthNames = this.GetMonthNames();
                    }
                }
            }
            return this.leapYearMonthNames;
        }

        internal string internalGetMonthName(int month, MonthNameStyles style, bool abbreviated)
        {
            string[] strArray = null;
            switch (style)
            {
                case MonthNameStyles.Genitive:
                    strArray = this.internalGetGenitiveMonthNames(abbreviated);
                    break;

                case MonthNameStyles.LeapYear:
                    strArray = this.internalGetLeapYearMonthNames();
                    break;

                default:
                    strArray = abbreviated ? this.GetAbbreviatedMonthNames() : this.GetMonthNames();
                    break;
            }
            if ((month < 1) || (month > strArray.Length))
            {
                throw new ArgumentOutOfRangeException("month", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("ArgumentOutOfRange_Range"), new object[] { 1, strArray.Length }));
            }
            return strArray[month - 1];
        }

        private string[] internalGetSuperShortDayNames()
        {
            if ((this.m_superShortDayNames == null) && (this.m_superShortDayNames == null))
            {
                string[] sSUPERSHORTDAYNAMES = null;
                if (!this.m_isDefaultCalendar)
                {
                    sSUPERSHORTDAYNAMES = CalendarTable.Default.SSUPERSHORTDAYNAMES(this.Calendar.ID);
                }
                if (((sSUPERSHORTDAYNAMES == null) || (sSUPERSHORTDAYNAMES.Length == 0)) || (sSUPERSHORTDAYNAMES[0].Length == 0))
                {
                    sSUPERSHORTDAYNAMES = this.m_cultureTableRecord.SSUPERSHORTDAYNAMES;
                }
                Thread.MemoryBarrier();
                this.m_superShortDayNames = sSUPERSHORTDAYNAMES;
            }
            return this.m_superShortDayNames;
        }

        private static bool IsHebrewChar(char ch)
        {
            return ((ch >= '֐') && (ch <= '׿'));
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext ctx)
        {
            if (CultureTableRecord.IsCustomCultureId(this.CultureID))
            {
                this.m_cultureTableRecord = CultureTableRecord.GetCultureTableRecord(this.m_name, this.m_useUserOverride);
            }
            else
            {
                this.m_cultureTableRecord = CultureTableRecord.GetCultureTableRecord(this.CultureID, this.m_useUserOverride);
            }
            if (this.calendar == null)
            {
                this.calendar = (System.Globalization.Calendar) GregorianCalendar.GetDefaultInstance().Clone();
                this.calendar.SetReadOnlyState(this.m_isReadOnly);
            }
            else
            {
                CultureInfo.CheckDomainSafetyObject(this.calendar, this);
            }
            this.InitializeOverridableProperties();
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext ctx)
        {
            this.CultureID = this.m_cultureTableRecord.CultureID;
            this.m_useUserOverride = this.m_cultureTableRecord.UseUserOverride;
            this.nDataItem = this.m_cultureTableRecord.EverettDataItem();
            if (CultureTableRecord.IsCustomCultureId(this.CultureID))
            {
                this.m_name = this.CultureName;
            }
        }

        public static DateTimeFormatInfo ReadOnly(DateTimeFormatInfo dtfi)
        {
            if (dtfi == null)
            {
                throw new ArgumentNullException("dtfi", Environment.GetResourceString("ArgumentNull_Obj"));
            }
            if (dtfi.IsReadOnly)
            {
                return dtfi;
            }
            DateTimeFormatInfo info = (DateTimeFormatInfo) dtfi.MemberwiseClone();
            info.Calendar = System.Globalization.Calendar.ReadOnly(info.Calendar);
            info.m_isReadOnly = true;
            return info;
        }

        [ComVisible(false)]
        public void SetAllDateTimePatterns(string[] patterns, char format)
        {
            this.VerifyWritable();
            if (patterns == null)
            {
                throw new ArgumentNullException("patterns", Environment.GetResourceString("ArgumentNull_Array"));
            }
            if (patterns.Length == 0)
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_ArrayZeroError"), "patterns");
            }
            for (int i = 0; i < patterns.Length; i++)
            {
                if (patterns[i] == null)
                {
                    throw new ArgumentNullException(Environment.GetResourceString("ArgumentNull_ArrayValue"));
                }
            }
            switch (format)
            {
                case 'd':
                    this.ShortDatePattern = patterns[0];
                    this.allShortDatePatterns = patterns;
                    return;

                case 't':
                    this.ShortTimePattern = patterns[0];
                    this.allShortTimePatterns = patterns;
                    return;

                case 'y':
                case 'Y':
                    this.yearMonthPattern = patterns[0];
                    this.allYearMonthPatterns = patterns;
                    return;

                case 'D':
                    this.LongDatePattern = patterns[0];
                    this.allLongDatePatterns = patterns;
                    return;

                case 'T':
                    this.LongTimePattern = patterns[0];
                    this.allLongTimePatterns = patterns;
                    return;
            }
            throw new ArgumentException(Environment.GetResourceString("Argument_BadFormatSpecifier"), "format");
        }

        internal void SetDefaultPatternAsFirstItem(string[] patterns, string defaultPattern)
        {
            if (patterns != null)
            {
                for (int i = 0; i < patterns.Length; i++)
                {
                    if (patterns[i].Equals(defaultPattern))
                    {
                        if (i != 0)
                        {
                            string str = patterns[i];
                            for (int j = i; j > 0; j--)
                            {
                                patterns[j] = patterns[j - 1];
                            }
                            patterns[0] = str;
                        }
                        return;
                    }
                }
            }
        }

        internal bool Tokenize(TokenType TokenMask, out TokenType tokenType, out int tokenValue, ref __DTString str)
        {
            tokenType = TokenType.UnknownToken;
            tokenValue = 0;
            char current = str.m_current;
            bool flag = char.IsLetter(current);
            if (flag)
            {
                bool flag2;
                current = char.ToLower(current, CultureInfo.CurrentCulture);
                if ((IsHebrewChar(current) && (TokenMask == TokenType.RegularTokenMask)) && TryParseHebrewNumber(ref str, out flag2, out tokenValue))
                {
                    if (flag2)
                    {
                        tokenType = TokenType.UnknownToken;
                        return false;
                    }
                    tokenType = TokenType.HebrewNumber;
                    return true;
                }
            }
            int index = current % '\x00c7';
            int num2 = 1 + (current % '\x00c5');
            int num3 = str.len - str.Index;
            int num4 = 0;
            TokenHashValue[] dtfiTokenHash = this.m_dtfiTokenHash;
            if (dtfiTokenHash == null)
            {
                dtfiTokenHash = this.CreateTokenHashTable();
            }
            do
            {
                TokenHashValue value2 = dtfiTokenHash[index];
                if (value2 == null)
                {
                    break;
                }
                if (((value2.tokenType & TokenMask) > ((TokenType) 0)) && (value2.tokenString.Length <= num3))
                {
                    if (string.Compare(str.Value, str.Index, value2.tokenString, 0, value2.tokenString.Length, true, CultureInfo.CurrentCulture) == 0)
                    {
                        int num5;
                        if (flag && ((num5 = str.Index + value2.tokenString.Length) < str.len))
                        {
                            char c = str.Value[num5];
                            if (char.IsLetter(c))
                            {
                                return false;
                            }
                        }
                        tokenType = value2.tokenType & TokenMask;
                        tokenValue = value2.tokenValue;
                        str.Advance(value2.tokenString.Length);
                        return true;
                    }
                    if ((value2.tokenType == TokenType.MonthToken) && this.HasSpacesInMonthNames)
                    {
                        int matchLength = 0;
                        if (str.MatchSpecifiedWords(value2.tokenString, true, ref matchLength))
                        {
                            tokenType = value2.tokenType & TokenMask;
                            tokenValue = value2.tokenValue;
                            str.Advance(matchLength);
                            return true;
                        }
                    }
                    else if ((value2.tokenType == TokenType.DayOfWeekToken) && this.HasSpacesInDayNames)
                    {
                        int num7 = 0;
                        if (str.MatchSpecifiedWords(value2.tokenString, true, ref num7))
                        {
                            tokenType = value2.tokenType & TokenMask;
                            tokenValue = value2.tokenValue;
                            str.Advance(num7);
                            return true;
                        }
                    }
                }
                num4++;
                index += num2;
                if (index >= 0xc7)
                {
                    index -= 0xc7;
                }
            }
            while (num4 < 0xc7);
            return false;
        }

        private static bool TryParseHebrewNumber(ref __DTString str, out bool badFormat, out int number)
        {
            HebrewNumberParsingState state;
            number = -1;
            badFormat = false;
            int index = str.Index;
            if (!HebrewNumber.IsDigit(str.Value[index]))
            {
                return false;
            }
            HebrewNumberParsingContext context = new HebrewNumberParsingContext(0);
            do
            {
                state = HebrewNumber.ParseByChar(str.Value[index++], ref context);
                switch (state)
                {
                    case HebrewNumberParsingState.InvalidHebrewNumber:
                    case HebrewNumberParsingState.NotHebrewDigit:
                        return false;
                }
            }
            while ((index < str.Value.Length) && (state != HebrewNumberParsingState.FoundEndOfHebrewNumber));
            if (state != HebrewNumberParsingState.FoundEndOfHebrewNumber)
            {
                return false;
            }
            str.Advance(index - str.Index);
            number = context.result;
            return true;
        }

        internal static void ValidateStyles(DateTimeStyles style, string parameterName)
        {
            if ((style & ~(DateTimeStyles.RoundtripKind | DateTimeStyles.AssumeUniversal | DateTimeStyles.AssumeLocal | DateTimeStyles.AdjustToUniversal | DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AllowWhiteSpaces)) != DateTimeStyles.None)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidDateTimeStyles"), parameterName);
            }
            if (((style & DateTimeStyles.AssumeLocal) != DateTimeStyles.None) && ((style & DateTimeStyles.AssumeUniversal) != DateTimeStyles.None))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_ConflictingDateTimeStyles"), parameterName);
            }
            if (((style & DateTimeStyles.RoundtripKind) != DateTimeStyles.None) && ((style & (DateTimeStyles.AssumeUniversal | DateTimeStyles.AssumeLocal | DateTimeStyles.AdjustToUniversal)) != DateTimeStyles.None))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_ConflictingDateTimeRoundtripStyles"), parameterName);
            }
        }

        private void VerifyWritable()
        {
            if (this.m_isReadOnly)
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ReadOnly"));
            }
        }

        internal bool YearMonthAdjustment(ref int year, ref int month, bool parsedMonthName)
        {
            if ((this.FormatFlags & DateTimeFormatFlags.UseHebrewRule) != DateTimeFormatFlags.None)
            {
                if (year < 0x3e8)
                {
                    year += 0x1388;
                }
                if ((year < this.Calendar.GetYear(this.Calendar.MinSupportedDateTime)) || (year > this.Calendar.GetYear(this.Calendar.MaxSupportedDateTime)))
                {
                    return false;
                }
                if (parsedMonthName && !this.Calendar.IsLeapYear(year))
                {
                    if (month >= 8)
                    {
                        month--;
                    }
                    else if (month == 7)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public string[] AbbreviatedDayNames
        {
            get
            {
                return (string[]) this.GetAbbreviatedDayOfWeekNames().Clone();
            }
            set
            {
                this.VerifyWritable();
                if (value == null)
                {
                    throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_Array"));
                }
                if (value.Length != 7)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_InvalidArrayLength"), new object[] { 7 }), "value");
                }
                this.CheckNullValue(value, value.Length);
                this.ClearTokenHashTable(true);
                this.abbreviatedDayNames = value;
            }
        }

        internal string[] AbbreviatedEnglishEraNames
        {
            get
            {
                if (this.m_abbrevEnglishEraNames == null)
                {
                    this.m_abbrevEnglishEraNames = CalendarTable.Default.SABBREVENGERANAMES(this.Calendar.ID);
                }
                return this.m_abbrevEnglishEraNames;
            }
        }

        internal string[] AbbreviatedEraNames
        {
            get
            {
                if (this.m_abbrevEraNames == null)
                {
                    if (this.Calendar.ID == 4)
                    {
                        string eraName = this.GetEraName(1);
                        if (eraName.Length > 0)
                        {
                            if (eraName.Length == 4)
                            {
                                this.m_abbrevEraNames = new string[] { eraName.Substring(2, 2) };
                            }
                            else
                            {
                                this.m_abbrevEraNames = new string[] { eraName };
                            }
                        }
                        else
                        {
                            this.m_abbrevEraNames = new string[0];
                        }
                    }
                    else if (this.Calendar.ID == 1)
                    {
                        this.m_abbrevEraNames = new string[] { this.m_cultureTableRecord.SABBREVADERA };
                    }
                    else
                    {
                        this.m_abbrevEraNames = CalendarTable.Default.SABBREVERANAMES(this.Calendar.ID);
                    }
                }
                return this.m_abbrevEraNames;
            }
        }

        [ComVisible(false)]
        public string[] AbbreviatedMonthGenitiveNames
        {
            get
            {
                return (string[]) this.internalGetGenitiveMonthNames(true).Clone();
            }
            set
            {
                this.VerifyWritable();
                if (value == null)
                {
                    throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_Array"));
                }
                if (value.Length != 13)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_InvalidArrayLength"), new object[] { 13 }), "value");
                }
                this.CheckNullValue(value, value.Length - 1);
                this.ClearTokenHashTable(true);
                this.m_genitiveAbbreviatedMonthNames = value;
            }
        }

        public string[] AbbreviatedMonthNames
        {
            get
            {
                return (string[]) this.GetAbbreviatedMonthNames().Clone();
            }
            set
            {
                this.VerifyWritable();
                if (value == null)
                {
                    throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_Array"));
                }
                if (value.Length != 13)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_InvalidArrayLength"), new object[] { 13 }), "value");
                }
                this.CheckNullValue(value, value.Length - 1);
                this.ClearTokenHashTable(true);
                this.abbreviatedMonthNames = value;
            }
        }

        public string AMDesignator
        {
            get
            {
                return this.amDesignator;
            }
            set
            {
                this.VerifyWritable();
                if (value == null)
                {
                    throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_String"));
                }
                this.ClearTokenHashTable(true);
                this.amDesignator = value;
            }
        }

        public System.Globalization.Calendar Calendar
        {
            get
            {
                return this.calendar;
            }
            set
            {
                this.VerifyWritable();
                if (value == null)
                {
                    throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_Obj"));
                }
                if (value != this.calendar)
                {
                    CultureInfo.CheckDomainSafetyObject(value, this);
                    for (int i = 0; i < this.OptionalCalendars.Length; i++)
                    {
                        if (this.OptionalCalendars[i] == value.ID)
                        {
                            this.ClearTokenHashTable(false);
                            this.m_isDefaultCalendar = value.ID == 1;
                            if (this.calendar != null)
                            {
                                this.m_eraNames = null;
                                this.m_abbrevEraNames = null;
                                this.m_abbrevEnglishEraNames = null;
                                this.shortDatePattern = null;
                                this.yearMonthPattern = null;
                                this.monthDayPattern = null;
                                this.longDatePattern = null;
                                this.dayNames = null;
                                this.abbreviatedDayNames = null;
                                this.m_superShortDayNames = null;
                                this.monthNames = null;
                                this.abbreviatedMonthNames = null;
                                this.genitiveMonthNames = null;
                                this.m_genitiveAbbreviatedMonthNames = null;
                                this.leapYearMonthNames = null;
                                this.formatFlags = ~DateTimeFormatFlags.None;
                                this.fullDateTimePattern = null;
                                this.generalShortTimePattern = null;
                                this.generalLongTimePattern = null;
                                this.allShortDatePatterns = null;
                                this.allLongDatePatterns = null;
                                this.allYearMonthPatterns = null;
                                this.dateTimeOffsetPattern = null;
                            }
                            this.calendar = value;
                            if (this.m_cultureTableRecord.UseCurrentCalendar(value.ID))
                            {
                                DTFIUserOverrideValues values = new DTFIUserOverrideValues();
                                this.m_cultureTableRecord.GetDTFIOverrideValues(ref values);
                                if (((this.m_cultureTableRecord.SLONGDATE != values.longDatePattern) || (this.m_cultureTableRecord.SSHORTDATE != values.shortDatePattern)) || ((this.m_cultureTableRecord.STIMEFORMAT != values.longTimePattern) || (this.m_cultureTableRecord.SYEARMONTH != values.yearMonthPattern)))
                                {
                                    this.m_scanDateWords = true;
                                }
                                this.amDesignator = values.amDesignator;
                                this.pmDesignator = values.pmDesignator;
                                this.longTimePattern = values.longTimePattern;
                                this.firstDayOfWeek = values.firstDayOfWeek;
                                this.calendarWeekRule = values.calendarWeekRule;
                                this.shortDatePattern = values.shortDatePattern;
                                this.longDatePattern = values.longDatePattern;
                                this.yearMonthPattern = values.yearMonthPattern;
                                if ((this.yearMonthPattern == null) || (this.yearMonthPattern.Length == 0))
                                {
                                    this.yearMonthPattern = this.GetYearMonthPattern(value.ID);
                                    return;
                                }
                            }
                            else
                            {
                                this.InitializeOverridableProperties();
                            }
                            return;
                        }
                    }
                    throw new ArgumentOutOfRangeException("value", Environment.GetResourceString("Argument_InvalidCalendar"));
                }
            }
        }

        public System.Globalization.CalendarWeekRule CalendarWeekRule
        {
            get
            {
                return (System.Globalization.CalendarWeekRule) this.calendarWeekRule;
            }
            set
            {
                this.VerifyWritable();
                if ((value < System.Globalization.CalendarWeekRule.FirstDay) || (value > System.Globalization.CalendarWeekRule.FirstFourDayWeek))
                {
                    throw new ArgumentOutOfRangeException("value", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("ArgumentOutOfRange_Range"), new object[] { System.Globalization.CalendarWeekRule.FirstDay, System.Globalization.CalendarWeekRule.FirstFourDayWeek }));
                }
                this.calendarWeekRule = (int) value;
            }
        }

        internal string[] ClonedAllLongDatePatterns
        {
            get
            {
                if (this.allLongDatePatterns == null)
                {
                    string[] patterns = null;
                    if (!this.m_isDefaultCalendar)
                    {
                        patterns = CalendarTable.Default.SLONGDATE(this.Calendar.ID);
                        if (patterns == null)
                        {
                            patterns = this.m_cultureTableRecord.SLONGDATES;
                        }
                    }
                    else
                    {
                        patterns = this.m_cultureTableRecord.SLONGDATES;
                    }
                    Thread.MemoryBarrier();
                    this.SetDefaultPatternAsFirstItem(patterns, this.LongDatePattern);
                    this.allLongDatePatterns = patterns;
                }
                if (this.allLongDatePatterns[0].Equals(this.LongDatePattern))
                {
                    return (string[]) this.allLongDatePatterns.Clone();
                }
                return this.AddDefaultFormat(this.allLongDatePatterns, this.LongDatePattern);
            }
        }

        internal string[] ClonedAllLongTimePatterns
        {
            get
            {
                if (this.allLongTimePatterns == null)
                {
                    this.allLongTimePatterns = this.m_cultureTableRecord.STIMEFORMATS;
                    this.SetDefaultPatternAsFirstItem(this.allLongTimePatterns, this.LongTimePattern);
                }
                if (this.allLongTimePatterns[0].Equals(this.LongTimePattern))
                {
                    return (string[]) this.allLongTimePatterns.Clone();
                }
                return this.AddDefaultFormat(this.allLongTimePatterns, this.LongTimePattern);
            }
        }

        internal string[] ClonedAllShortDatePatterns
        {
            get
            {
                if (this.allShortDatePatterns == null)
                {
                    string[] patterns = null;
                    if (!this.m_isDefaultCalendar)
                    {
                        patterns = CalendarTable.Default.SSHORTDATE(this.Calendar.ID);
                        if (patterns == null)
                        {
                            patterns = this.m_cultureTableRecord.SSHORTDATES;
                        }
                    }
                    else
                    {
                        patterns = this.m_cultureTableRecord.SSHORTDATES;
                    }
                    Thread.MemoryBarrier();
                    this.SetDefaultPatternAsFirstItem(patterns, this.ShortDatePattern);
                    this.allShortDatePatterns = patterns;
                }
                if (this.allShortDatePatterns[0].Equals(this.ShortDatePattern))
                {
                    return (string[]) this.allShortDatePatterns.Clone();
                }
                return this.AddDefaultFormat(this.allShortDatePatterns, this.ShortDatePattern);
            }
        }

        internal string[] ClonedAllShortTimePatterns
        {
            get
            {
                if (this.allShortTimePatterns == null)
                {
                    this.allShortTimePatterns = this.m_cultureTableRecord.SSHORTTIMES;
                    this.SetDefaultPatternAsFirstItem(this.allShortTimePatterns, this.ShortTimePattern);
                }
                if (this.allShortTimePatterns[0].Equals(this.ShortTimePattern))
                {
                    return (string[]) this.allShortTimePatterns.Clone();
                }
                return this.AddDefaultFormat(this.allShortTimePatterns, this.ShortTimePattern);
            }
        }

        internal string[] ClonedAllYearMonthPatterns
        {
            get
            {
                if (this.allYearMonthPatterns == null)
                {
                    string[] patterns = null;
                    if (!this.m_isDefaultCalendar)
                    {
                        patterns = CalendarTable.Default.SYEARMONTH(this.Calendar.ID);
                        if (patterns == null)
                        {
                            patterns = this.m_cultureTableRecord.SYEARMONTHS;
                        }
                    }
                    else
                    {
                        patterns = this.m_cultureTableRecord.SYEARMONTHS;
                    }
                    Thread.MemoryBarrier();
                    this.SetDefaultPatternAsFirstItem(patterns, this.YearMonthPattern);
                    this.allYearMonthPatterns = patterns;
                }
                if (this.allYearMonthPatterns[0].Equals(this.YearMonthPattern))
                {
                    return (string[]) this.allYearMonthPatterns.Clone();
                }
                return this.AddDefaultFormat(this.allYearMonthPatterns, this.YearMonthPattern);
            }
        }

        internal System.Globalization.CompareInfo CompareInfo
        {
            get
            {
                if (this.m_compareInfo == null)
                {
                    if (CultureTableRecord.IsCustomCultureId(this.CultureId))
                    {
                        this.m_compareInfo = System.Globalization.CompareInfo.GetCompareInfo((int) this.m_cultureTableRecord.ICOMPAREINFO);
                    }
                    else
                    {
                        this.m_compareInfo = System.Globalization.CompareInfo.GetCompareInfo(this.CultureId);
                    }
                }
                return this.m_compareInfo;
            }
        }

        internal int CultureId
        {
            get
            {
                return this.m_cultureTableRecord.CultureID;
            }
        }

        internal string CultureName
        {
            get
            {
                if (this.m_name == null)
                {
                    this.m_name = this.m_cultureTableRecord.SNAME;
                }
                return this.m_name;
            }
        }

        public static DateTimeFormatInfo CurrentInfo
        {
            get
            {
                CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
                if (!currentCulture.m_isInherited)
                {
                    DateTimeFormatInfo dateTimeInfo = currentCulture.dateTimeInfo;
                    if (dateTimeInfo != null)
                    {
                        return dateTimeInfo;
                    }
                }
                return (DateTimeFormatInfo) currentCulture.GetFormat(typeof(DateTimeFormatInfo));
            }
        }

        public string DateSeparator
        {
            get
            {
                if (this.dateSeparator == null)
                {
                    this.dateSeparator = this.m_cultureTableRecord.SDATE;
                }
                return this.dateSeparator;
            }
            set
            {
                this.VerifyWritable();
                if (value == null)
                {
                    throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_String"));
                }
                this.ClearTokenHashTable(true);
                this.dateSeparator = value;
            }
        }

        internal string DateTimeOffsetPattern
        {
            get
            {
                if (this.dateTimeOffsetPattern == null)
                {
                    this.dateTimeOffsetPattern = this.ShortDatePattern + " " + this.LongTimePattern + " zzz";
                }
                return this.dateTimeOffsetPattern;
            }
        }

        internal string[] DateWords
        {
            get
            {
                if (this.m_dateWords == null)
                {
                    this.m_dateWords = this.m_cultureTableRecord.SDATEWORDS;
                }
                return this.m_dateWords;
            }
        }

        public string[] DayNames
        {
            get
            {
                return (string[]) this.GetDayOfWeekNames().Clone();
            }
            set
            {
                this.VerifyWritable();
                if (value == null)
                {
                    throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_Array"));
                }
                if (value.Length != 7)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_InvalidArrayLength"), new object[] { 7 }), "value");
                }
                this.CheckNullValue(value, value.Length);
                this.ClearTokenHashTable(true);
                this.dayNames = value;
            }
        }

        internal string[] EraNames
        {
            get
            {
                if (this.m_eraNames == null)
                {
                    if (this.Calendar.ID == 1)
                    {
                        this.m_eraNames = new string[] { this.m_cultureTableRecord.SADERA };
                    }
                    else if (this.Calendar.ID != 4)
                    {
                        this.m_eraNames = CalendarTable.Default.SERANAMES(this.Calendar.ID);
                    }
                    else
                    {
                        this.m_eraNames = new string[] { CalendarTable.nativeGetEraName(0x404, this.Calendar.ID) };
                    }
                }
                return this.m_eraNames;
            }
        }

        public DayOfWeek FirstDayOfWeek
        {
            get
            {
                return (DayOfWeek) this.firstDayOfWeek;
            }
            set
            {
                this.VerifyWritable();
                if ((value < DayOfWeek.Sunday) || (value > DayOfWeek.Saturday))
                {
                    throw new ArgumentOutOfRangeException("value", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("ArgumentOutOfRange_Range"), new object[] { DayOfWeek.Sunday, DayOfWeek.Saturday }));
                }
                this.firstDayOfWeek = (int) value;
            }
        }

        internal DateTimeFormatFlags FormatFlags
        {
            get
            {
                if (this.formatFlags == ~DateTimeFormatFlags.None)
                {
                    if (this.m_scanDateWords || this.m_cultureTableRecord.IsSynthetic)
                    {
                        this.formatFlags = DateTimeFormatFlags.None;
                        this.formatFlags |= (DateTimeFormatFlags) ((int) DateTimeFormatInfoScanner.GetFormatFlagGenitiveMonth(this.MonthNames, this.internalGetGenitiveMonthNames(false), this.AbbreviatedMonthNames, this.internalGetGenitiveMonthNames(true)));
                        this.formatFlags |= (DateTimeFormatFlags) ((int) DateTimeFormatInfoScanner.GetFormatFlagUseSpaceInMonthNames(this.MonthNames, this.internalGetGenitiveMonthNames(false), this.AbbreviatedMonthNames, this.internalGetGenitiveMonthNames(true)));
                        this.formatFlags |= (DateTimeFormatFlags) ((int) DateTimeFormatInfoScanner.GetFormatFlagUseSpaceInDayNames(this.DayNames, this.AbbreviatedDayNames));
                        this.formatFlags |= (DateTimeFormatFlags) ((int) DateTimeFormatInfoScanner.GetFormatFlagUseHebrewCalendar(this.Calendar.ID));
                    }
                    else if (this.m_isDefaultCalendar)
                    {
                        this.formatFlags = this.m_cultureTableRecord.IFORMATFLAGS;
                    }
                    else
                    {
                        this.formatFlags = (DateTimeFormatFlags) CalendarTable.Default.IFORMATFLAGS(this.Calendar.ID);
                    }
                }
                return this.formatFlags;
            }
        }

        public string FullDateTimePattern
        {
            get
            {
                if (this.fullDateTimePattern == null)
                {
                    this.fullDateTimePattern = this.LongDatePattern + " " + this.LongTimePattern;
                }
                return this.fullDateTimePattern;
            }
            set
            {
                this.VerifyWritable();
                if (value == null)
                {
                    throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_String"));
                }
                this.fullDateTimePattern = value;
            }
        }

        internal string GeneralLongTimePattern
        {
            get
            {
                if (this.generalLongTimePattern == null)
                {
                    this.generalLongTimePattern = this.ShortDatePattern + " " + this.LongTimePattern;
                }
                return this.generalLongTimePattern;
            }
        }

        internal string GeneralShortTimePattern
        {
            get
            {
                if (this.generalShortTimePattern == null)
                {
                    this.generalShortTimePattern = this.ShortDatePattern + " " + this.ShortTimePattern;
                }
                return this.generalShortTimePattern;
            }
        }

        internal bool HasForceTwoDigitYears
        {
            get
            {
                switch (this.calendar.ID)
                {
                    case 3:
                    case 4:
                        return true;
                }
                return false;
            }
        }

        internal bool HasSpacesInDayNames
        {
            get
            {
                return ((this.FormatFlags & DateTimeFormatFlags.UseSpacesInDayNames) != DateTimeFormatFlags.None);
            }
        }

        internal bool HasSpacesInMonthNames
        {
            get
            {
                return ((this.FormatFlags & DateTimeFormatFlags.UseSpacesInMonthNames) != DateTimeFormatFlags.None);
            }
        }

        internal bool HasYearMonthAdjustment
        {
            get
            {
                return ((this.FormatFlags & DateTimeFormatFlags.UseHebrewRule) != DateTimeFormatFlags.None);
            }
        }

        private static object InternalSyncObject
        {
            get
            {
                if (s_InternalSyncObject == null)
                {
                    object obj2 = new object();
                    Interlocked.CompareExchange(ref s_InternalSyncObject, obj2, null);
                }
                return s_InternalSyncObject;
            }
        }

        public static DateTimeFormatInfo InvariantInfo
        {
            get
            {
                if (invariantInfo == null)
                {
                    DateTimeFormatInfo info = new DateTimeFormatInfo();
                    info.Calendar.SetReadOnlyState(true);
                    info.m_isReadOnly = true;
                    invariantInfo = info;
                }
                return invariantInfo;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return this.m_isReadOnly;
            }
        }

        internal string LanguageName
        {
            get
            {
                if (this.m_langName == null)
                {
                    this.m_langName = this.m_cultureTableRecord.SISO639LANGNAME;
                }
                return this.m_langName;
            }
        }

        public string LongDatePattern
        {
            get
            {
                return this.longDatePattern;
            }
            set
            {
                this.VerifyWritable();
                if (value == null)
                {
                    throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_String"));
                }
                this.ClearTokenHashTable(true);
                this.SetDefaultPatternAsFirstItem(this.allLongDatePatterns, value);
                this.longDatePattern = value;
                this.fullDateTimePattern = null;
            }
        }

        public string LongTimePattern
        {
            get
            {
                return this.longTimePattern;
            }
            set
            {
                this.VerifyWritable();
                if (value == null)
                {
                    throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_String"));
                }
                this.longTimePattern = value;
                this.fullDateTimePattern = null;
                this.generalLongTimePattern = null;
                this.dateTimeOffsetPattern = null;
            }
        }

        public string MonthDayPattern
        {
            get
            {
                if (this.monthDayPattern == null)
                {
                    string sMONTHDAY;
                    if (this.m_isDefaultCalendar)
                    {
                        sMONTHDAY = this.m_cultureTableRecord.SMONTHDAY;
                    }
                    else
                    {
                        sMONTHDAY = CalendarTable.Default.SMONTHDAY(this.Calendar.ID);
                        if (sMONTHDAY.Length == 0)
                        {
                            sMONTHDAY = this.m_cultureTableRecord.SMONTHDAY;
                        }
                    }
                    this.monthDayPattern = sMONTHDAY;
                }
                return this.monthDayPattern;
            }
            set
            {
                this.VerifyWritable();
                if (value == null)
                {
                    throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_String"));
                }
                this.monthDayPattern = value;
            }
        }

        [ComVisible(false)]
        public string[] MonthGenitiveNames
        {
            get
            {
                return (string[]) this.internalGetGenitiveMonthNames(false).Clone();
            }
            set
            {
                this.VerifyWritable();
                if (value == null)
                {
                    throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_Array"));
                }
                if (value.Length != 13)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_InvalidArrayLength"), new object[] { 13 }), "value");
                }
                this.CheckNullValue(value, value.Length - 1);
                this.genitiveMonthNames = value;
                this.ClearTokenHashTable(true);
            }
        }

        public string[] MonthNames
        {
            get
            {
                return (string[]) this.GetMonthNames().Clone();
            }
            set
            {
                this.VerifyWritable();
                if (value == null)
                {
                    throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_Array"));
                }
                if (value.Length != 13)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_InvalidArrayLength"), new object[] { 13 }), "value");
                }
                this.CheckNullValue(value, value.Length - 1);
                this.monthNames = value;
                this.ClearTokenHashTable(true);
            }
        }

        [ComVisible(false)]
        public string NativeCalendarName
        {
            get
            {
                if (this.Calendar.ID == 4)
                {
                    string str = GetCalendarInfo(0x404, 4, 2);
                    if (str == null)
                    {
                        str = CalendarTable.nativeGetEraName(0x404, 4);
                        if (str == null)
                        {
                            str = string.Empty;
                        }
                    }
                    return str;
                }
                string[] sNATIVECALNAMES = this.m_cultureTableRecord.SNATIVECALNAMES;
                int index = this.calendar.ID - 1;
                if (index < sNATIVECALNAMES.Length)
                {
                    if (sNATIVECALNAMES[index].Length <= 0)
                    {
                        return this.GetCalendarNativeNameFallback(this.calendar.ID);
                    }
                    if (sNATIVECALNAMES[index][0] != 0xfeff)
                    {
                        return sNATIVECALNAMES[index];
                    }
                }
                return string.Empty;
            }
        }

        internal int[] OptionalCalendars
        {
            get
            {
                if (this.optionalCalendars == null)
                {
                    this.optionalCalendars = this.m_cultureTableRecord.IOPTIONALCALENDARS;
                }
                return this.optionalCalendars;
            }
        }

        public string PMDesignator
        {
            get
            {
                return this.pmDesignator;
            }
            set
            {
                this.VerifyWritable();
                if (value == null)
                {
                    throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_String"));
                }
                this.ClearTokenHashTable(true);
                this.pmDesignator = value;
            }
        }

        public string RFC1123Pattern
        {
            get
            {
                return "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'";
            }
        }

        public string ShortDatePattern
        {
            get
            {
                return this.shortDatePattern;
            }
            set
            {
                this.VerifyWritable();
                if (value == null)
                {
                    throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_String"));
                }
                this.SetDefaultPatternAsFirstItem(this.allShortDatePatterns, value);
                this.shortDatePattern = value;
                this.generalLongTimePattern = null;
                this.generalShortTimePattern = null;
                this.dateTimeOffsetPattern = null;
            }
        }

        [ComVisible(false)]
        public string[] ShortestDayNames
        {
            get
            {
                return (string[]) this.internalGetSuperShortDayNames().Clone();
            }
            set
            {
                this.VerifyWritable();
                if (value == null)
                {
                    throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_Array"));
                }
                if (value.Length != 7)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_InvalidArrayLength"), new object[] { 7 }), "value");
                }
                this.CheckNullValue(value, value.Length);
                this.m_superShortDayNames = value;
            }
        }

        public string ShortTimePattern
        {
            get
            {
                if (this.shortTimePattern == null)
                {
                    this.shortTimePattern = this.m_cultureTableRecord.SSHORTTIME;
                }
                return this.shortTimePattern;
            }
            set
            {
                this.VerifyWritable();
                if (value == null)
                {
                    throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_String"));
                }
                this.shortTimePattern = value;
                this.generalShortTimePattern = null;
            }
        }

        public string SortableDateTimePattern
        {
            get
            {
                return "yyyy'-'MM'-'dd'T'HH':'mm':'ss";
            }
        }

        public string TimeSeparator
        {
            get
            {
                if (this.timeSeparator == null)
                {
                    this.timeSeparator = this.m_cultureTableRecord.STIME;
                }
                return this.timeSeparator;
            }
            set
            {
                this.VerifyWritable();
                if (value == null)
                {
                    throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_String"));
                }
                this.ClearTokenHashTable(true);
                this.timeSeparator = value;
            }
        }

        public string UniversalSortableDateTimePattern
        {
            get
            {
                return "yyyy'-'MM'-'dd HH':'mm':'ss'Z'";
            }
        }

        public string YearMonthPattern
        {
            get
            {
                return this.yearMonthPattern;
            }
            set
            {
                this.VerifyWritable();
                if (value == null)
                {
                    throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_String"));
                }
                this.yearMonthPattern = value;
                this.SetDefaultPatternAsFirstItem(this.allYearMonthPatterns, this.yearMonthPattern);
            }
        }
    }
}

