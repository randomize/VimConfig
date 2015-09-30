namespace System.Globalization
{
    using Microsoft.Win32;
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Threading;

    [Serializable, ComVisible(true)]
    public class CultureInfo : ICloneable, IFormatProvider
    {
        internal System.Globalization.Calendar calendar;
        internal System.Globalization.CompareInfo compareInfo;
        internal int cultureID;
        internal DateTimeFormatInfo dateTimeInfo;
        internal const int LCID_INSTALLED = 1;
        internal const int LCID_SUPPORTED = 2;
        internal const int LOCALE_CUSTOM_DEFAULT = 0xc00;
        internal const int LOCALE_CUSTOM_UNSPECIFIED = 0x1000;
        internal const int LOCALE_INVARIANT = 0x7f;
        private const int LOCALE_NEUTRAL = 0;
        internal const int LOCALE_SYSTEM_DEFAULT = 0x800;
        internal const int LOCALE_TRADITIONAL_SPANISH = 0x40a;
        internal const int LOCALE_USER_DEFAULT = 0x400;
        [NonSerialized]
        private CultureInfo m_consoleFallbackCulture;
        [NonSerialized]
        private int m_createdDomainID;
        [NonSerialized]
        internal CultureTableRecord m_cultureTableRecord;
        private int m_dataItem;
        private static CultureInfo m_InstalledUICultureInfo;
        private static CultureInfo m_InvariantCultureInfo;
        [NonSerialized]
        internal bool m_isInherited;
        internal bool m_isReadOnly;
        [NonSerialized]
        private bool m_isSafeCrossDomain;
        private static Hashtable m_LcidCachedCultures;
        internal string m_name;
        private static Hashtable m_NameCachedCultures;
        [NonSerialized]
        private string m_nonSortName;
        [NonSerialized]
        private CultureInfo m_parent;
        [NonSerialized]
        private string m_sortName;
        private static CultureInfo m_userDefaultCulture;
        private static CultureInfo m_userDefaultUICulture;
        private bool m_useUserOverride;
        internal NumberFormatInfo numInfo;
        internal const int sr_CultureID = 0x7c1a;
        internal const int sr_SP_Latn_CultureID = 0x81a;
        internal System.Globalization.TextInfo textInfo;
        internal const int zh_CHS_CultureID = 4;
        internal const int zh_CHT_CultureID = 0x7c04;

        static CultureInfo()
        {
            if (m_InvariantCultureInfo == null)
            {
                CultureInfo info = new CultureInfo(0x7f, false) {
                    m_isReadOnly = true
                };
                m_InvariantCultureInfo = info;
            }
            m_userDefaultCulture = m_userDefaultUICulture = m_InvariantCultureInfo;
            m_userDefaultCulture = InitUserDefaultCulture();
            m_userDefaultUICulture = InitUserDefaultUICulture();
        }

        public CultureInfo(int culture) : this(culture, true)
        {
        }

        public CultureInfo(string name) : this(name, true)
        {
        }

        public CultureInfo(int culture, bool useUserOverride)
        {
            if (culture < 0)
            {
                throw new ArgumentOutOfRangeException("culture", Environment.GetResourceString("ArgumentOutOfRange_NeedPosNum"));
            }
            switch (culture)
            {
                case 0x800:
                case 0xc00:
                case 0x1000:
                case 0:
                case 0x400:
                    throw new ArgumentException(Environment.GetResourceString("Argument_CultureNotSupported", new object[] { culture }), "culture");
            }
            this.cultureID = culture;
            this.m_cultureTableRecord = CultureTableRecord.GetCultureTableRecord(this.cultureID, useUserOverride);
            this.m_name = this.m_cultureTableRecord.ActualName;
            this.m_isInherited = base.GetType() != typeof(CultureInfo);
        }

        public CultureInfo(string name, bool useUserOverride)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name", Environment.GetResourceString("ArgumentNull_String"));
            }
            this.m_cultureTableRecord = CultureTableRecord.GetCultureTableRecord(name, useUserOverride);
            this.cultureID = this.m_cultureTableRecord.ActualCultureID;
            this.m_name = this.m_cultureTableRecord.ActualName;
            this.m_isInherited = base.GetType() != typeof(CultureInfo);
        }

        internal CultureInfo(string cultureName, string textAndCompareCultureName)
        {
            if (cultureName == null)
            {
                throw new ArgumentNullException("cultureName", Environment.GetResourceString("ArgumentNull_String"));
            }
            this.m_cultureTableRecord = CultureTableRecord.GetCultureTableRecord(cultureName, false);
            this.cultureID = this.m_cultureTableRecord.ActualCultureID;
            this.m_name = this.m_cultureTableRecord.ActualName;
            CultureInfo cultureInfo = GetCultureInfo(textAndCompareCultureName);
            this.compareInfo = cultureInfo.CompareInfo;
            this.textInfo = cultureInfo.TextInfo;
        }

        internal static void CheckDomainSafetyObject(object obj, object container)
        {
            if (obj.GetType().Assembly != typeof(CultureInfo).Assembly)
            {
                throw new InvalidOperationException(string.Format(CurrentCulture, Environment.GetResourceString("InvalidOperation_SubclassedObject"), new object[] { obj.GetType(), container.GetType() }));
            }
        }

        internal static void CheckNeutral(CultureInfo culture)
        {
            if (culture.IsNeutralCulture)
            {
                throw new NotSupportedException(Environment.GetResourceString("Argument_CultureInvalidFormat", new object[] { culture.m_name }));
            }
        }

        public void ClearCachedData()
        {
            m_userDefaultUICulture = null;
            m_userDefaultCulture = null;
            RegionInfo.m_currentRegionInfo = null;
            TimeZone.ResetTimeZone();
            m_LcidCachedCultures = null;
            m_NameCachedCultures = null;
            CultureTableRecord.ResetCustomCulturesCache();
            System.Globalization.CompareInfo.ClearDefaultAssemblyCache();
        }

        public virtual object Clone()
        {
            CultureInfo info = (CultureInfo) base.MemberwiseClone();
            info.m_isReadOnly = false;
            if (!info.IsNeutralCulture)
            {
                if (!this.m_isInherited)
                {
                    if (this.dateTimeInfo != null)
                    {
                        info.dateTimeInfo = (DateTimeFormatInfo) this.dateTimeInfo.Clone();
                    }
                    if (this.numInfo != null)
                    {
                        info.numInfo = (NumberFormatInfo) this.numInfo.Clone();
                    }
                }
                else
                {
                    info.DateTimeFormat = (DateTimeFormatInfo) this.DateTimeFormat.Clone();
                    info.NumberFormat = (NumberFormatInfo) this.NumberFormat.Clone();
                }
            }
            if (this.textInfo != null)
            {
                info.textInfo = (System.Globalization.TextInfo) this.textInfo.Clone();
            }
            if (this.calendar != null)
            {
                info.calendar = (System.Globalization.Calendar) this.calendar.Clone();
            }
            return info;
        }

        public static CultureInfo CreateSpecificCulture(string name)
        {
            CultureInfo info;
            try
            {
                info = new CultureInfo(name);
            }
            catch (ArgumentException exception)
            {
                info = null;
                for (int i = 0; i < name.Length; i++)
                {
                    if ('-' == name[i])
                    {
                        try
                        {
                            info = new CultureInfo(name.Substring(0, i));
                            break;
                        }
                        catch (ArgumentException)
                        {
                            throw exception;
                        }
                    }
                }
                if (info == null)
                {
                    throw exception;
                }
            }
            if (!info.IsNeutralCulture)
            {
                return info;
            }
            if ((info.LCID & 0x3ff) == 4)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_NoSpecificCulture"));
            }
            return new CultureInfo(info.m_cultureTableRecord.SSPECIFICCULTURE);
        }

        public override bool Equals(object value)
        {
            if (object.ReferenceEquals(this, value))
            {
                return true;
            }
            CultureInfo info = value as CultureInfo;
            if (info == null)
            {
                return false;
            }
            return (this.Name.Equals(info.Name) && this.CompareInfo.Equals(info.CompareInfo));
        }

        internal static System.Globalization.Calendar GetCalendarInstance(int calType)
        {
            if (calType == 1)
            {
                return new GregorianCalendar();
            }
            return GetCalendarInstanceRare(calType);
        }

        internal static System.Globalization.Calendar GetCalendarInstanceRare(int calType)
        {
            switch (calType)
            {
                case 2:
                case 9:
                case 10:
                case 11:
                case 12:
                    return new GregorianCalendar((GregorianCalendarTypes) calType);

                case 3:
                    return new JapaneseCalendar();

                case 4:
                    return new TaiwanCalendar();

                case 5:
                    return new KoreanCalendar();

                case 6:
                    return new HijriCalendar();

                case 7:
                    return new ThaiBuddhistCalendar();

                case 8:
                    return new HebrewCalendar();

                case 14:
                    return new JapaneseLunisolarCalendar();

                case 15:
                    return new ChineseLunisolarCalendar();

                case 20:
                    return new KoreanLunisolarCalendar();

                case 0x15:
                    return new TaiwanLunisolarCalendar();

                case 0x16:
                    return new PersianCalendar();

                case 0x17:
                    return new UmAlQuraCalendar();
            }
            return new GregorianCalendar();
        }

        [ComVisible(false)]
        public CultureInfo GetConsoleFallbackUICulture()
        {
            CultureInfo consoleFallbackCulture = this.m_consoleFallbackCulture;
            if (consoleFallbackCulture == null)
            {
                consoleFallbackCulture = GetCultureInfo(this.m_cultureTableRecord.SCONSOLEFALLBACKNAME);
                consoleFallbackCulture.m_isReadOnly = true;
                this.m_consoleFallbackCulture = consoleFallbackCulture;
            }
            return consoleFallbackCulture;
        }

        private static CultureInfo GetCultureByLCIDOrName(int preferLCID, string fallbackToString)
        {
            CultureInfo info = null;
            if ((preferLCID & 0x3ff) != 0)
            {
                try
                {
                    info = new CultureInfo(preferLCID);
                }
                catch (ArgumentException)
                {
                }
            }
            if (((info == null) && (fallbackToString != null)) && (fallbackToString.Length > 0))
            {
                try
                {
                    info = new CultureInfo(fallbackToString);
                }
                catch (ArgumentException)
                {
                }
            }
            return info;
        }

        public static CultureInfo GetCultureInfo(int culture)
        {
            if (culture <= 0)
            {
                throw new ArgumentOutOfRangeException("culture", Environment.GetResourceString("ArgumentOutOfRange_NeedPosNum"));
            }
            CultureInfo info = GetCultureInfoHelper(culture, null, null);
            if (info == null)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_CultureNotSupported", new object[] { culture }), "culture");
            }
            return info;
        }

        public static CultureInfo GetCultureInfo(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            CultureInfo info = GetCultureInfoHelper(0, name, null);
            if (info == null)
            {
                throw new ArgumentException(string.Format(CurrentCulture, Environment.GetResourceString("Argument_InvalidCultureName"), new object[] { name }), "name");
            }
            return info;
        }

        public static CultureInfo GetCultureInfo(string name, string altName)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (altName == null)
            {
                throw new ArgumentNullException("altName");
            }
            CultureInfo info = GetCultureInfoHelper(-1, name, altName);
            if (info == null)
            {
                throw new ArgumentException(string.Format(CurrentCulture, Environment.GetResourceString("Argument_OneOfCulturesNotSupported"), new object[] { name, altName }), "name");
            }
            return info;
        }

        public static CultureInfo GetCultureInfoByIetfLanguageTag(string name)
        {
            if ("zh-CHT".Equals(name, StringComparison.OrdinalIgnoreCase) || "zh-CHS".Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(string.Format(CurrentCulture, Environment.GetResourceString("Argument_CultureIetfNotSupported"), new object[] { name }), "name");
            }
            CultureInfo cultureInfo = GetCultureInfo(name);
            if ((GetSortID(cultureInfo.cultureID) != 0) || (cultureInfo.cultureID == 0x40a))
            {
                throw new ArgumentException(string.Format(CurrentCulture, Environment.GetResourceString("Argument_CultureIetfNotSupported"), new object[] { name }), "name");
            }
            return cultureInfo;
        }

        internal static CultureInfo GetCultureInfoHelper(int lcid, string name, string altName)
        {
            CultureInfo info;
            Hashtable nameCachedCultures = m_NameCachedCultures;
            if (name != null)
            {
                name = CultureTableRecord.AnsiToLower(name);
            }
            if (altName != null)
            {
                altName = CultureTableRecord.AnsiToLower(altName);
            }
            if (nameCachedCultures == null)
            {
                nameCachedCultures = Hashtable.Synchronized(new Hashtable());
            }
            else if (lcid == -1)
            {
                info = (CultureInfo) nameCachedCultures[name + ((char) 0xfffd) + altName];
                if (info != null)
                {
                    return info;
                }
            }
            else if (lcid == 0)
            {
                info = (CultureInfo) nameCachedCultures[name];
                if (info != null)
                {
                    return info;
                }
            }
            Hashtable lcidCachedCultures = m_LcidCachedCultures;
            if (lcidCachedCultures == null)
            {
                lcidCachedCultures = Hashtable.Synchronized(new Hashtable());
            }
            else if (lcid > 0)
            {
                info = (CultureInfo) lcidCachedCultures[lcid];
                if (info != null)
                {
                    return info;
                }
            }
            try
            {
                switch (lcid)
                {
                    case -1:
                        info = new CultureInfo(name, altName);
                        goto Label_010A;

                    case 0:
                        info = new CultureInfo(name, false);
                        goto Label_010A;
                }
                if ((m_userDefaultCulture != null) && (m_userDefaultCulture.LCID == lcid))
                {
                    info = (CultureInfo) m_userDefaultCulture.Clone();
                    info.m_cultureTableRecord = info.m_cultureTableRecord.CloneWithUserOverride(false);
                }
                else
                {
                    info = new CultureInfo(lcid, false);
                }
            }
            catch (ArgumentException)
            {
                return null;
            }
        Label_010A:
            info.m_isReadOnly = true;
            if (lcid == -1)
            {
                nameCachedCultures[name + ((char) 0xfffd) + altName] = info;
                info.TextInfo.SetReadOnlyState(true);
            }
            else
            {
                if (!CultureTable.IsNewNeutralChineseCulture(info))
                {
                    lcidCachedCultures[info.LCID] = info;
                }
                string str = CultureTableRecord.AnsiToLower(info.m_name);
                nameCachedCultures[str] = info;
            }
            if (-1 != lcid)
            {
                m_LcidCachedCultures = lcidCachedCultures;
            }
            m_NameCachedCultures = nameCachedCultures;
            return info;
        }

        public static CultureInfo[] GetCultures(System.Globalization.CultureTypes types)
        {
            return CultureTable.Default.GetCultures(types);
        }

        public virtual object GetFormat(Type formatType)
        {
            if (formatType == typeof(NumberFormatInfo))
            {
                return this.NumberFormat;
            }
            if (formatType == typeof(DateTimeFormatInfo))
            {
                return this.DateTimeFormat;
            }
            return null;
        }

        public override int GetHashCode()
        {
            return (this.Name.GetHashCode() + this.CompareInfo.GetHashCode());
        }

        internal static int GetLangID(int culture)
        {
            return (culture & 0xffff);
        }

        internal static unsafe int GetNativeSortKey(int lcid, int flags, string source, int cchSrc, out byte[] sortKey)
        {
            int num;
            sortKey = null;
            if (string.IsNullOrEmpty(source) || (cchSrc == 0))
            {
                sortKey = new byte[0];
                source = "\0";
                cchSrc = 1;
            }
            fixed (char* str = ((char*) source))
            {
                char* src = str;
                num = Win32Native.LCMapStringW(lcid, flags | 0x400, src, cchSrc, null, 0);
                if (num == 0)
                {
                    return -1;
                }
                if (sortKey == null)
                {
                    byte[] buffer;
                    sortKey = new byte[num];
                    if (((buffer = sortKey) != null) && (buffer.Length != 0))
                    {
                        goto Label_006D;
                    }
                    fixed (byte* numRef = null)
                    {
                        goto Label_0076;
                    Label_006D:
                        numRef = buffer;
                    Label_0076:
                        num = Win32Native.LCMapStringW(lcid, flags | 0x400, src, cchSrc, (char*) numRef, num);
                    }
                }
            }
            return num;
        }

        internal static int GetSortID(int lcid)
        {
            return ((lcid >> 0x10) & 15);
        }

        internal static int GetSubLangID(int culture)
        {
            return ((culture >> 10) & 0x3f);
        }

        private static unsafe CultureInfo InitUserDefaultCulture()
        {
            int num;
            string fallbackToString = nativeGetUserDefaultLCID(&num, 0x400);
            CultureInfo cultureByLCIDOrName = GetCultureByLCIDOrName(num, fallbackToString);
            if (cultureByLCIDOrName == null)
            {
                fallbackToString = nativeGetUserDefaultLCID(&num, 0x800);
                cultureByLCIDOrName = GetCultureByLCIDOrName(num, fallbackToString);
                if (cultureByLCIDOrName == null)
                {
                    return InvariantCulture;
                }
            }
            cultureByLCIDOrName.m_isReadOnly = true;
            return cultureByLCIDOrName;
        }

        private static unsafe CultureInfo InitUserDefaultUICulture()
        {
            int num;
            string fallbackToString = nativeGetUserDefaultUILanguage(&num);
            if ((num == UserDefaultCulture.LCID) || (fallbackToString == UserDefaultCulture.Name))
            {
                return UserDefaultCulture;
            }
            CultureInfo cultureByLCIDOrName = GetCultureByLCIDOrName(num, fallbackToString);
            if (cultureByLCIDOrName == null)
            {
                fallbackToString = nativeGetSystemDefaultUILanguage(&num);
                cultureByLCIDOrName = GetCultureByLCIDOrName(num, fallbackToString);
            }
            if (cultureByLCIDOrName == null)
            {
                return InvariantCulture;
            }
            cultureByLCIDOrName.m_isReadOnly = true;
            return cultureByLCIDOrName;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool IsValidLCID(int LCID, int flag);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool IsWin9xInstalledCulture(string cultureKey, int LCID);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool nativeEnumSystemLocales(out int[] localesArray);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool nativeFileExists(string fileName);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool nativeGetCultureData(int lcid, ref CultureData cultureData);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern string nativeGetCultureName(int lcid, bool useSNameLCType, bool getMonthName);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int nativeGetCurrentCalendar();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool nativeGetDTFIUserValues(int lcid, ref DTFIUserOverrideValues values);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern string nativeGetLocaleInfo(int LCID, int field);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool nativeGetNFIUserValues(int lcid, NumberFormatInfo nfi);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern unsafe int* nativeGetStaticInt32DataTable(int type, out int tableSize);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern unsafe string nativeGetSystemDefaultUILanguage(int* LCID);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern unsafe string nativeGetUserDefaultLCID(int* LCID, int lcidType);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern unsafe string nativeGetUserDefaultUILanguage(int* LCID);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern string nativeGetWindowsDirectory();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool nativeSetThreadLocale(int LCID);
        [OnDeserialized]
        private void OnDeserialized(StreamingContext ctx)
        {
            if ((this.m_name != null) && (this.cultureID != 0x40a))
            {
                this.m_cultureTableRecord = CultureTableRecord.GetCultureTableRecord(this.m_name, this.m_useUserOverride);
            }
            else
            {
                this.m_cultureTableRecord = CultureTableRecord.GetCultureTableRecord(this.cultureID, this.m_useUserOverride);
            }
            this.m_isInherited = base.GetType() != typeof(CultureInfo);
            if (this.m_name == null)
            {
                this.m_name = this.m_cultureTableRecord.ActualName;
            }
            if (base.GetType().Assembly == typeof(CultureInfo).Assembly)
            {
                if (this.textInfo != null)
                {
                    CheckDomainSafetyObject(this.textInfo, this);
                }
                if (this.compareInfo != null)
                {
                    CheckDomainSafetyObject(this.compareInfo, this);
                }
            }
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext ctx)
        {
            this.m_name = this.m_cultureTableRecord.CultureName;
            this.m_useUserOverride = this.m_cultureTableRecord.UseUserOverride;
            this.m_dataItem = this.m_cultureTableRecord.EverettDataItem();
        }

        public static CultureInfo ReadOnly(CultureInfo ci)
        {
            if (ci == null)
            {
                throw new ArgumentNullException("ci");
            }
            if (ci.IsReadOnly)
            {
                return ci;
            }
            CultureInfo info = (CultureInfo) ci.MemberwiseClone();
            if (!ci.IsNeutralCulture)
            {
                if (!ci.m_isInherited)
                {
                    if (ci.dateTimeInfo != null)
                    {
                        info.dateTimeInfo = DateTimeFormatInfo.ReadOnly(ci.dateTimeInfo);
                    }
                    if (ci.numInfo != null)
                    {
                        info.numInfo = NumberFormatInfo.ReadOnly(ci.numInfo);
                    }
                }
                else
                {
                    info.DateTimeFormat = DateTimeFormatInfo.ReadOnly(ci.DateTimeFormat);
                    info.NumberFormat = NumberFormatInfo.ReadOnly(ci.NumberFormat);
                }
            }
            if (ci.textInfo != null)
            {
                info.textInfo = System.Globalization.TextInfo.ReadOnly(ci.textInfo);
            }
            if (ci.calendar != null)
            {
                info.calendar = System.Globalization.Calendar.ReadOnly(ci.calendar);
            }
            info.m_isReadOnly = true;
            return info;
        }

        internal void StartCrossDomainTracking()
        {
            if (this.m_createdDomainID == 0)
            {
                if (base.GetType() == typeof(CultureInfo))
                {
                    this.m_isSafeCrossDomain = true;
                }
                Thread.MemoryBarrier();
                this.m_createdDomainID = Thread.GetDomainID();
            }
        }

        public override string ToString()
        {
            return this.m_name;
        }

        internal static bool VerifyCultureName(CultureInfo culture, bool throwException)
        {
            if (culture.m_isInherited)
            {
                string name = culture.Name;
                for (int i = 0; i < name.Length; i++)
                {
                    char c = name[i];
                    if ((!char.IsLetterOrDigit(c) && (c != '-')) && (c != '_'))
                    {
                        if (throwException)
                        {
                            throw new ArgumentException(Environment.GetResourceString("Argument_InvalidResourceCultureName", new object[] { name }));
                        }
                        return false;
                    }
                }
            }
            return true;
        }

        private void VerifyWritable()
        {
            if (this.m_isReadOnly)
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ReadOnly"));
            }
        }

        public virtual System.Globalization.Calendar Calendar
        {
            get
            {
                if (this.calendar == null)
                {
                    System.Globalization.Calendar calendarInstance = GetCalendarInstance(this.m_cultureTableRecord.ICALENDARTYPE);
                    Thread.MemoryBarrier();
                    calendarInstance.SetReadOnlyState(this.m_isReadOnly);
                    this.calendar = calendarInstance;
                }
                return this.calendar;
            }
        }

        public virtual System.Globalization.CompareInfo CompareInfo
        {
            get
            {
                if (this.compareInfo == null)
                {
                    int cultureID;
                    if (this.IsNeutralCulture && !CultureTableRecord.IsCustomCultureId(this.cultureID))
                    {
                        cultureID = this.cultureID;
                    }
                    else
                    {
                        cultureID = this.CompareInfoId;
                    }
                    if ((this.Name == "zh-CHS") || (this.Name == "zh-CHT"))
                    {
                        cultureID |= -2147483648;
                    }
                    System.Globalization.CompareInfo compareInfoWithPrefixedLcid = System.Globalization.CompareInfo.GetCompareInfoWithPrefixedLcid(cultureID, -2147483648);
                    compareInfoWithPrefixedLcid.SetName(this.SortName);
                    this.compareInfo = compareInfoWithPrefixedLcid;
                }
                return this.compareInfo;
            }
        }

        internal int CompareInfoId
        {
            get
            {
                if (this.cultureID == 0x40a)
                {
                    return 0x40a;
                }
                if (GetSortID(this.cultureID) != 0)
                {
                    return this.cultureID;
                }
                return (int) this.m_cultureTableRecord.ICOMPAREINFO;
            }
        }

        internal int CreatedDomainID
        {
            get
            {
                return this.m_createdDomainID;
            }
        }

        [ComVisible(false)]
        public System.Globalization.CultureTypes CultureTypes
        {
            get
            {
                System.Globalization.CultureTypes types = 0;
                if (this.m_cultureTableRecord.IsNeutralCulture)
                {
                    types |= System.Globalization.CultureTypes.NeutralCultures;
                }
                else
                {
                    types |= System.Globalization.CultureTypes.SpecificCultures;
                }
                if (this.m_cultureTableRecord.IsSynthetic)
                {
                    types |= System.Globalization.CultureTypes.WindowsOnlyCultures | System.Globalization.CultureTypes.InstalledWin32Cultures;
                }
                else
                {
                    if (CultureTable.IsInstalledLCID(this.cultureID))
                    {
                        types |= System.Globalization.CultureTypes.InstalledWin32Cultures;
                    }
                    if (!this.m_cultureTableRecord.IsCustomCulture || this.m_cultureTableRecord.IsReplacementCulture)
                    {
                        types |= System.Globalization.CultureTypes.FrameworkCultures;
                    }
                }
                if (this.m_cultureTableRecord.IsCustomCulture)
                {
                    types |= System.Globalization.CultureTypes.UserCustomCulture;
                    if (this.m_cultureTableRecord.IsReplacementCulture)
                    {
                        types |= System.Globalization.CultureTypes.ReplacementCultures;
                    }
                }
                return types;
            }
        }

        public static CultureInfo CurrentCulture
        {
            get
            {
                return Thread.CurrentThread.CurrentCulture;
            }
        }

        public static CultureInfo CurrentUICulture
        {
            get
            {
                return Thread.CurrentThread.CurrentUICulture;
            }
        }

        public virtual DateTimeFormatInfo DateTimeFormat
        {
            get
            {
                if (this.dateTimeInfo == null)
                {
                    CheckNeutral(this);
                    DateTimeFormatInfo info = new DateTimeFormatInfo(this.m_cultureTableRecord, GetLangID(this.cultureID), this.Calendar) {
                        m_isReadOnly = this.m_isReadOnly
                    };
                    Thread.MemoryBarrier();
                    this.dateTimeInfo = info;
                }
                return this.dateTimeInfo;
            }
            set
            {
                this.VerifyWritable();
                if (value == null)
                {
                    throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_Obj"));
                }
                this.dateTimeInfo = value;
            }
        }

        public virtual string DisplayName
        {
            get
            {
                if (this.m_cultureTableRecord.IsCustomCulture)
                {
                    if (!this.m_cultureTableRecord.IsReplacementCulture)
                    {
                        return this.m_cultureTableRecord.SNATIVEDISPLAYNAME;
                    }
                    if (this.m_cultureTableRecord.IsSynthetic)
                    {
                        return this.m_cultureTableRecord.CultureNativeDisplayName;
                    }
                    return Environment.GetResourceString("Globalization.ci_" + this.m_name);
                }
                if (this.m_cultureTableRecord.IsSynthetic)
                {
                    return this.m_cultureTableRecord.CultureNativeDisplayName;
                }
                return Environment.GetResourceString("Globalization.ci_" + this.m_name);
            }
        }

        public virtual string EnglishName
        {
            get
            {
                return this.m_cultureTableRecord.SENGDISPLAYNAME;
            }
        }

        [ComVisible(false)]
        public string IetfLanguageTag
        {
            get
            {
                if (CultureTable.IsOldNeutralChineseCulture(this))
                {
                    if (this.LCID == 0x7c04)
                    {
                        return "zh-Hant";
                    }
                    if (this.LCID == 4)
                    {
                        return "zh-Hans";
                    }
                }
                return this.Name;
            }
        }

        public static CultureInfo InstalledUICulture
        {
            get
            {
                CultureInfo installedUICultureInfo = m_InstalledUICultureInfo;
                if (installedUICultureInfo == null)
                {
                    int num;
                    string fallbackToString = nativeGetSystemDefaultUILanguage(&num);
                    installedUICultureInfo = GetCultureByLCIDOrName(num, fallbackToString);
                    if (installedUICultureInfo == null)
                    {
                        installedUICultureInfo = new CultureInfo(0x7f, true);
                    }
                    installedUICultureInfo.m_isReadOnly = true;
                    m_InstalledUICultureInfo = installedUICultureInfo;
                }
                return installedUICultureInfo;
            }
        }

        public static CultureInfo InvariantCulture
        {
            get
            {
                return m_InvariantCultureInfo;
            }
        }

        public virtual bool IsNeutralCulture
        {
            get
            {
                return this.m_cultureTableRecord.IsNeutralCulture;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return this.m_isReadOnly;
            }
        }

        internal bool IsSafeCrossDomain
        {
            get
            {
                return this.m_isSafeCrossDomain;
            }
        }

        [ComVisible(false)]
        public virtual int KeyboardLayoutId
        {
            get
            {
                return this.m_cultureTableRecord.IINPUTLANGUAGEHANDLE;
            }
        }

        public virtual int LCID
        {
            get
            {
                return this.cultureID;
            }
        }

        public virtual string Name
        {
            get
            {
                if (this.m_nonSortName == null)
                {
                    this.m_nonSortName = this.m_cultureTableRecord.CultureName;
                }
                return this.m_nonSortName;
            }
        }

        public virtual string NativeName
        {
            get
            {
                return this.m_cultureTableRecord.SNATIVEDISPLAYNAME;
            }
        }

        public virtual NumberFormatInfo NumberFormat
        {
            get
            {
                CheckNeutral(this);
                if (this.numInfo == null)
                {
                    NumberFormatInfo info = new NumberFormatInfo(this.m_cultureTableRecord) {
                        isReadOnly = this.m_isReadOnly
                    };
                    this.numInfo = info;
                }
                return this.numInfo;
            }
            set
            {
                this.VerifyWritable();
                if (value == null)
                {
                    throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_Obj"));
                }
                this.numInfo = value;
            }
        }

        public virtual System.Globalization.Calendar[] OptionalCalendars
        {
            get
            {
                int[] iOPTIONALCALENDARS = this.m_cultureTableRecord.IOPTIONALCALENDARS;
                System.Globalization.Calendar[] calendarArray = new System.Globalization.Calendar[iOPTIONALCALENDARS.Length];
                for (int i = 0; i < calendarArray.Length; i++)
                {
                    calendarArray[i] = GetCalendarInstance(iOPTIONALCALENDARS[i]);
                }
                return calendarArray;
            }
        }

        public virtual CultureInfo Parent
        {
            get
            {
                if (this.m_parent == null)
                {
                    try
                    {
                        int iPARENT = this.m_cultureTableRecord.IPARENT;
                        if (iPARENT == 0x7f)
                        {
                            this.m_parent = InvariantCulture;
                        }
                        else if (CultureTableRecord.IsCustomCultureId(iPARENT) || CultureTable.IsOldNeutralChineseCulture(this))
                        {
                            this.m_parent = new CultureInfo(this.m_cultureTableRecord.SPARENT);
                        }
                        else
                        {
                            this.m_parent = new CultureInfo(iPARENT, this.m_cultureTableRecord.UseUserOverride);
                        }
                    }
                    catch (ArgumentException)
                    {
                        this.m_parent = InvariantCulture;
                    }
                }
                return this.m_parent;
            }
        }

        internal string SortName
        {
            get
            {
                if (this.m_sortName == null)
                {
                    if (CultureTableRecord.IsCustomCultureId(this.cultureID))
                    {
                        CultureInfo cultureInfo = GetCultureInfo(this.CompareInfoId);
                        if (CultureTableRecord.IsCustomCultureId(cultureInfo.cultureID))
                        {
                            this.m_sortName = this.m_cultureTableRecord.SNAME;
                        }
                        else
                        {
                            this.m_sortName = cultureInfo.SortName;
                        }
                    }
                    else
                    {
                        this.m_sortName = this.m_name;
                    }
                }
                return this.m_sortName;
            }
        }

        public virtual System.Globalization.TextInfo TextInfo
        {
            get
            {
                if (this.textInfo == null)
                {
                    System.Globalization.TextInfo info = new System.Globalization.TextInfo(this.m_cultureTableRecord);
                    info.SetReadOnlyState(this.m_isReadOnly);
                    this.textInfo = info;
                }
                return this.textInfo;
            }
        }

        public virtual string ThreeLetterISOLanguageName
        {
            get
            {
                return this.m_cultureTableRecord.SISO639LANGNAME2;
            }
        }

        public virtual string ThreeLetterWindowsLanguageName
        {
            get
            {
                return this.m_cultureTableRecord.SABBREVLANGNAME;
            }
        }

        public virtual string TwoLetterISOLanguageName
        {
            get
            {
                return this.m_cultureTableRecord.SISO639LANGNAME;
            }
        }

        internal static CultureInfo UserDefaultCulture
        {
            get
            {
                CultureInfo userDefaultCulture = m_userDefaultCulture;
                if (userDefaultCulture == null)
                {
                    m_userDefaultCulture = InvariantCulture;
                    userDefaultCulture = InitUserDefaultCulture();
                    m_userDefaultCulture = userDefaultCulture;
                }
                return userDefaultCulture;
            }
        }

        internal static CultureInfo UserDefaultUICulture
        {
            get
            {
                CultureInfo userDefaultUICulture = m_userDefaultUICulture;
                if (userDefaultUICulture == null)
                {
                    m_userDefaultUICulture = InvariantCulture;
                    userDefaultUICulture = InitUserDefaultUICulture();
                    m_userDefaultUICulture = userDefaultUICulture;
                }
                return userDefaultUICulture;
            }
        }

        public bool UseUserOverride
        {
            get
            {
                return this.m_cultureTableRecord.UseUserOverride;
            }
        }
    }
}

