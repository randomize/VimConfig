namespace System.Globalization
{
    using Microsoft.Win32;
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Threading;

    [Serializable, ComVisible(true)]
    public class CompareInfo : IDeserializationCallback
    {
        internal const int CHT_CHS_LCID_COMPAREINFO_KEY_FLAG = -2147483648;
        private int culture;
        [NonSerialized]
        private static int fFindNLSStringSupported;
        private const int HongKongCultureId = 0xc04;
        [NonSerialized]
        private System.Globalization.CultureTableRecord m_cultureTableRecord;
        [OptionalField(VersionAdded=2)]
        private string m_name;
        [NonSerialized]
        internal unsafe void* m_pSortingTable;
        [NonSerialized]
        private int m_sortingLCID;
        private const int NORM_IGNORECASE = 1;
        private const int NORM_IGNOREKANATYPE = 0x10000;
        private const int NORM_IGNORENONSPACE = 2;
        private const int NORM_IGNORESYMBOLS = 4;
        private const int NORM_IGNOREWIDTH = 0x20000;
        private static object s_InternalSyncObject;
        private const int SORT_STRINGSORT = 0x1000;
        private const int TraditionalChineseCultureId = 0x7c04;
        private const CompareOptions ValidCompareMaskOffFlags = ~(CompareOptions.StringSort | CompareOptions.IgnoreWidth | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase);
        private const CompareOptions ValidHashCodeOfStringMaskOffFlags = ~(CompareOptions.IgnoreWidth | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase);
        private const CompareOptions ValidIndexMaskOffFlags = ~(CompareOptions.IgnoreWidth | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase);
        private int win32LCID;

        internal unsafe CompareInfo(GlobalizationAssembly ga, int culture)
        {
            if (culture < 0)
            {
                throw new ArgumentOutOfRangeException("culture", Environment.GetResourceString("ArgumentOutOfRange_NeedPosNum"));
            }
            this.m_sortingLCID = this.GetSortingLCID(culture);
            if (!this.IsSynthetic)
            {
                this.m_pSortingTable = InitializeCompareInfo(GlobalizationAssembly.DefaultInstance.pNativeGlobalizationAssembly, this.m_sortingLCID);
            }
            this.culture = culture;
        }

        internal static void ClearDefaultAssemblyCache()
        {
            lock (InternalSyncObject)
            {
                GlobalizationAssembly.DefaultInstance.compareInfoCache = new Hashtable(4);
            }
        }

        public virtual int Compare(string string1, string string2)
        {
            return this.Compare(string1, string2, CompareOptions.None);
        }

        public virtual unsafe int Compare(string string1, string string2, CompareOptions options)
        {
            if (options == CompareOptions.OrdinalIgnoreCase)
            {
                return string.Compare(string1, string2, StringComparison.OrdinalIgnoreCase);
            }
            if ((options & CompareOptions.Ordinal) != CompareOptions.None)
            {
                if (options != CompareOptions.Ordinal)
                {
                    throw new ArgumentException(Environment.GetResourceString("Argument_CompareOptionOrdinal"), "options");
                }
                if (string1 == null)
                {
                    if (string2 == null)
                    {
                        return 0;
                    }
                    return -1;
                }
                if (string2 == null)
                {
                    return 1;
                }
                return string.nativeCompareOrdinal(string1, string2, false);
            }
            if ((options & ~(CompareOptions.StringSort | CompareOptions.IgnoreWidth | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase)) != CompareOptions.None)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidFlag"), "options");
            }
            if (string1 == null)
            {
                if (string2 == null)
                {
                    return 0;
                }
                return -1;
            }
            if (string2 == null)
            {
                return 1;
            }
            if (!this.IsSynthetic)
            {
                return Compare(this.m_pSortingTable, this.m_sortingLCID, string1, string2, options);
            }
            if (options == CompareOptions.Ordinal)
            {
                return Compare(CultureInfo.InvariantCulture.CompareInfo.m_pSortingTable, this.m_sortingLCID, string1, string2, options);
            }
            return nativeCompareString(this.m_sortingLCID, string1, 0, string1.Length, string2, 0, string2.Length, GetNativeCompareFlags(options));
        }

        public virtual int Compare(string string1, int offset1, string string2, int offset2)
        {
            return this.Compare(string1, offset1, string2, offset2, CompareOptions.None);
        }

        public virtual int Compare(string string1, int offset1, string string2, int offset2, CompareOptions options)
        {
            return this.Compare(string1, offset1, (string1 == null) ? 0 : (string1.Length - offset1), string2, offset2, (string2 == null) ? 0 : (string2.Length - offset2), options);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern unsafe int Compare(void* pSortingTable, int sortingLCID, string string1, string string2, CompareOptions options);
        public virtual int Compare(string string1, int offset1, int length1, string string2, int offset2, int length2)
        {
            return this.Compare(string1, offset1, length1, string2, offset2, length2, CompareOptions.None);
        }

        public virtual unsafe int Compare(string string1, int offset1, int length1, string string2, int offset2, int length2, CompareOptions options)
        {
            if (options == CompareOptions.OrdinalIgnoreCase)
            {
                int num = string.Compare(string1, offset1, string2, offset2, (length1 < length2) ? length1 : length2, StringComparison.OrdinalIgnoreCase);
                if ((length1 == length2) || (num != 0))
                {
                    return num;
                }
                if (length1 <= length2)
                {
                    return -1;
                }
                return 1;
            }
            if ((length1 < 0) || (length2 < 0))
            {
                throw new ArgumentOutOfRangeException((length1 < 0) ? "length1" : "length2", Environment.GetResourceString("ArgumentOutOfRange_NeedPosNum"));
            }
            if ((offset1 < 0) || (offset2 < 0))
            {
                throw new ArgumentOutOfRangeException((offset1 < 0) ? "offset1" : "offset2", Environment.GetResourceString("ArgumentOutOfRange_NeedPosNum"));
            }
            if (offset1 > (((string1 == null) ? 0 : string1.Length) - length1))
            {
                throw new ArgumentOutOfRangeException("string1", Environment.GetResourceString("ArgumentOutOfRange_OffsetLength"));
            }
            if (offset2 > (((string2 == null) ? 0 : string2.Length) - length2))
            {
                throw new ArgumentOutOfRangeException("string2", Environment.GetResourceString("ArgumentOutOfRange_OffsetLength"));
            }
            if ((options & CompareOptions.Ordinal) != CompareOptions.None)
            {
                if (options != CompareOptions.Ordinal)
                {
                    throw new ArgumentException(Environment.GetResourceString("Argument_CompareOptionOrdinal"), "options");
                }
                if (string1 == null)
                {
                    if (string2 == null)
                    {
                        return 0;
                    }
                    return -1;
                }
                if (string2 != null)
                {
                    int num2 = string.nativeCompareOrdinalEx(string1, offset1, string2, offset2, (length1 < length2) ? length1 : length2);
                    if ((length1 == length2) || (num2 != 0))
                    {
                        return num2;
                    }
                    if (length1 <= length2)
                    {
                        return -1;
                    }
                }
                return 1;
            }
            if ((options & ~(CompareOptions.StringSort | CompareOptions.IgnoreWidth | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase)) != CompareOptions.None)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidFlag"), "options");
            }
            if (string1 == null)
            {
                if (string2 == null)
                {
                    return 0;
                }
                return -1;
            }
            if (string2 == null)
            {
                return 1;
            }
            if (!this.IsSynthetic)
            {
                return CompareRegion(this.m_pSortingTable, this.m_sortingLCID, string1, offset1, length1, string2, offset2, length2, options);
            }
            if (options == CompareOptions.Ordinal)
            {
                return CompareRegion(CultureInfo.InvariantCulture.CompareInfo.m_pSortingTable, this.m_sortingLCID, string1, offset1, length1, string2, offset2, length2, options);
            }
            return nativeCompareString(this.m_sortingLCID, string1, offset1, length1, string2, offset2, length2, GetNativeCompareFlags(options));
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern unsafe int CompareRegion(void* pSortingTable, int sortingLCID, string string1, int offset1, int length1, string string2, int offset2, int length2, CompareOptions options);
        public override bool Equals(object value)
        {
            CompareInfo info = value as CompareInfo;
            if (info == null)
            {
                return false;
            }
            return ((this.m_sortingLCID == info.m_sortingLCID) && this.Name.Equals(info.Name));
        }

        private static unsafe int FindNLSStringWrap(int lcid, int flags, string src, int start, int cchSrc, string value, int cchValue)
        {
            int num = -1;
            fixed (char* str = ((char*) src))
            {
                char* chPtr = str;
                fixed (char* str2 = ((char*) value))
                {
                    char* lpStringValue = str2;
                    if (1 == fFindNLSStringSupported)
                    {
                        num = Win32Native.FindNLSString(lcid, flags, chPtr + start, cchSrc, lpStringValue, cchValue, IntPtr.Zero);
                    }
                    else
                    {
                        try
                        {
                            num = Win32Native.FindNLSString(lcid, flags, chPtr + start, cchSrc, lpStringValue, cchValue, IntPtr.Zero);
                            fFindNLSStringSupported = 1;
                        }
                        catch (EntryPointNotFoundException)
                        {
                            num = fFindNLSStringSupported = -2;
                        }
                    }
                }
            }
            return num;
        }

        public static CompareInfo GetCompareInfo(int culture)
        {
            return GetCompareInfoWithPrefixedLcid(culture, 0);
        }

        public static CompareInfo GetCompareInfo(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            return GetCompareInfoByName(name, null);
        }

        public static CompareInfo GetCompareInfo(int culture, Assembly assembly)
        {
            return GetCompareInfoWithPrefixedLcid(culture, assembly, 0);
        }

        public static CompareInfo GetCompareInfo(string name, Assembly assembly)
        {
            if ((name == null) || (assembly == null))
            {
                throw new ArgumentNullException((name == null) ? "name" : "assembly");
            }
            if (assembly != typeof(object).Module.Assembly)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_OnlyMscorlib"));
            }
            return GetCompareInfoByName(name, assembly);
        }

        private static CompareInfo GetCompareInfoByName(string name, Assembly assembly)
        {
            CompareInfo compareInfoWithPrefixedLcid;
            CultureInfo cultureInfo = CultureInfo.GetCultureInfo(name);
            if (cultureInfo.IsNeutralCulture && !System.Globalization.CultureTableRecord.IsCustomCultureId(cultureInfo.cultureID))
            {
                if (cultureInfo.cultureID == 0x7c04)
                {
                    cultureInfo = CultureInfo.GetCultureInfo(0xc04);
                }
                else
                {
                    cultureInfo = CultureInfo.GetCultureInfo(cultureInfo.CompareInfoId);
                }
            }
            int compareInfoId = cultureInfo.CompareInfoId;
            if ((cultureInfo.Name == "zh-CHS") || (cultureInfo.Name == "zh-CHT"))
            {
                compareInfoId |= -2147483648;
            }
            if (assembly != null)
            {
                compareInfoWithPrefixedLcid = GetCompareInfoWithPrefixedLcid(compareInfoId, assembly, -2147483648);
            }
            else
            {
                compareInfoWithPrefixedLcid = GetCompareInfoWithPrefixedLcid(compareInfoId, -2147483648);
            }
            compareInfoWithPrefixedLcid.m_name = cultureInfo.SortName;
            return compareInfoWithPrefixedLcid;
        }

        internal static CompareInfo GetCompareInfoWithPrefixedLcid(int cultureKey, int prefix)
        {
            int cultureId = cultureKey & ~prefix;
            if (System.Globalization.CultureTableRecord.IsCustomCultureId(cultureId))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_CustomCultureCannotBePassedByNumber", new object[] { "culture" }));
            }
            object obj2 = GlobalizationAssembly.DefaultInstance.compareInfoCache[cultureKey];
            if (obj2 == null)
            {
                lock (InternalSyncObject)
                {
                    obj2 = GlobalizationAssembly.DefaultInstance.compareInfoCache[cultureKey];
                    if (obj2 == null)
                    {
                        obj2 = new CompareInfo(GlobalizationAssembly.DefaultInstance, cultureId);
                        Thread.MemoryBarrier();
                        GlobalizationAssembly.DefaultInstance.compareInfoCache[cultureKey] = obj2;
                    }
                }
            }
            return (CompareInfo) obj2;
        }

        private static CompareInfo GetCompareInfoWithPrefixedLcid(int cultureKey, Assembly assembly, int prefix)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }
            int cultureId = cultureKey & ~prefix;
            if (System.Globalization.CultureTableRecord.IsCustomCultureId(cultureId))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_CustomCultureCannotBePassedByNumber", new object[] { "culture" }));
            }
            if (assembly != typeof(object).Module.Assembly)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_OnlyMscorlib"));
            }
            GlobalizationAssembly globalizationAssembly = GlobalizationAssembly.GetGlobalizationAssembly(assembly);
            object obj2 = globalizationAssembly.compareInfoCache[cultureKey];
            if (obj2 == null)
            {
                lock (InternalSyncObject)
                {
                    obj2 = globalizationAssembly.compareInfoCache[cultureKey];
                    if (obj2 == null)
                    {
                        obj2 = new CompareInfo(globalizationAssembly, cultureId);
                        Thread.MemoryBarrier();
                        globalizationAssembly.compareInfoCache[cultureKey] = obj2;
                    }
                }
            }
            return (CompareInfo) obj2;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        internal unsafe int GetHashCodeOfString(string source, CompareOptions options)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if ((options & ~(CompareOptions.IgnoreWidth | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase)) != CompareOptions.None)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidFlag"), "options");
            }
            if (source.Length == 0)
            {
                return 0;
            }
            if (this.IsSynthetic)
            {
                return CultureInfo.InvariantCulture.CompareInfo.GetHashCodeOfString(source, options);
            }
            return nativeGetGlobalizedHashCode(this.m_pSortingTable, source, (int) options, this.m_sortingLCID);
        }

        internal static int GetNativeCompareFlags(CompareOptions options)
        {
            int num = 0;
            if ((options & CompareOptions.IgnoreCase) != CompareOptions.None)
            {
                num |= 1;
            }
            if ((options & CompareOptions.IgnoreKanaType) != CompareOptions.None)
            {
                num |= 0x10000;
            }
            if ((options & CompareOptions.IgnoreNonSpace) != CompareOptions.None)
            {
                num |= 2;
            }
            if ((options & CompareOptions.IgnoreSymbols) != CompareOptions.None)
            {
                num |= 4;
            }
            if ((options & CompareOptions.IgnoreWidth) != CompareOptions.None)
            {
                num |= 0x20000;
            }
            if ((options & CompareOptions.StringSort) != CompareOptions.None)
            {
                num |= 0x1000;
            }
            return num;
        }

        internal int GetSortingLCID(int culture)
        {
            int compareInfoId = 0;
            CultureInfo cultureInfo = CultureInfo.GetCultureInfo(culture);
            if (cultureInfo.m_cultureTableRecord.IsSynthetic)
            {
                return culture;
            }
            compareInfoId = cultureInfo.CompareInfoId;
            int sortID = CultureInfo.GetSortID(culture);
            if (sortID == 0)
            {
                return compareInfoId;
            }
            if (!cultureInfo.m_cultureTableRecord.IsValidSortID(sortID))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_CultureNotSupported"), new object[] { culture }), "culture");
            }
            return (compareInfoId | (sortID << 0x10));
        }

        public virtual unsafe SortKey GetSortKey(string source)
        {
            if (this.IsSynthetic)
            {
                return new SortKey(this.m_sortingLCID, source, CompareOptions.None);
            }
            return new SortKey(this.m_pSortingTable, this.m_sortingLCID, source, CompareOptions.None);
        }

        public virtual unsafe SortKey GetSortKey(string source, CompareOptions options)
        {
            if (this.IsSynthetic)
            {
                return new SortKey(this.m_sortingLCID, source, options);
            }
            return new SortKey(this.m_pSortingTable, this.m_sortingLCID, source, options);
        }

        public virtual int IndexOf(string source, char value)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return this.IndexOf(source, value, 0, source.Length, CompareOptions.None);
        }

        public virtual int IndexOf(string source, string value)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return this.IndexOf(source, value, 0, source.Length, CompareOptions.None);
        }

        public virtual int IndexOf(string source, char value, CompareOptions options)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return this.IndexOf(source, value, 0, source.Length, options);
        }

        public virtual int IndexOf(string source, char value, int startIndex)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return this.IndexOf(source, value, startIndex, source.Length - startIndex, CompareOptions.None);
        }

        public virtual int IndexOf(string source, string value, CompareOptions options)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return this.IndexOf(source, value, 0, source.Length, options);
        }

        public virtual int IndexOf(string source, string value, int startIndex)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return this.IndexOf(source, value, startIndex, source.Length - startIndex, CompareOptions.None);
        }

        public virtual int IndexOf(string source, char value, int startIndex, CompareOptions options)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return this.IndexOf(source, value, startIndex, source.Length - startIndex, options);
        }

        public virtual int IndexOf(string source, char value, int startIndex, int count)
        {
            return this.IndexOf(source, value, startIndex, count, CompareOptions.None);
        }

        public virtual int IndexOf(string source, string value, int startIndex, CompareOptions options)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return this.IndexOf(source, value, startIndex, source.Length - startIndex, options);
        }

        public virtual int IndexOf(string source, string value, int startIndex, int count)
        {
            return this.IndexOf(source, value, startIndex, count, CompareOptions.None);
        }

        public virtual unsafe int IndexOf(string source, char value, int startIndex, int count, CompareOptions options)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if ((startIndex < 0) || (startIndex > source.Length))
            {
                throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_Index"));
            }
            if ((count < 0) || (startIndex > (source.Length - count)))
            {
                throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_Count"));
            }
            if (options == CompareOptions.OrdinalIgnoreCase)
            {
                return TextInfo.nativeIndexOfCharOrdinalIgnoreCase(TextInfo.InvariantNativeTextInfo, source, value, startIndex, count);
            }
            if (((options & ~(CompareOptions.IgnoreWidth | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase)) != CompareOptions.None) && (options != CompareOptions.Ordinal))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidFlag"), "options");
            }
            if (!this.IsSynthetic)
            {
                return IndexOfChar(this.m_pSortingTable, this.m_sortingLCID, source, value, startIndex, count, (int) options);
            }
            if (options == CompareOptions.Ordinal)
            {
                return IndexOfChar(CultureInfo.InvariantCulture.CompareInfo.m_pSortingTable, this.m_sortingLCID, source, value, startIndex, count, (int) options);
            }
            return this.SyntheticIndexOf(source, new string(value, 1), startIndex, count, GetNativeCompareFlags(options));
        }

        public virtual unsafe int IndexOf(string source, string value, int startIndex, int count, CompareOptions options)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (startIndex > source.Length)
            {
                throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_Index"));
            }
            if (source.Length == 0)
            {
                if (value.Length == 0)
                {
                    return 0;
                }
                return -1;
            }
            if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_Index"));
            }
            if ((count < 0) || (startIndex > (source.Length - count)))
            {
                throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_Count"));
            }
            if (options == CompareOptions.OrdinalIgnoreCase)
            {
                return TextInfo.IndexOfStringOrdinalIgnoreCase(source, value, startIndex, count);
            }
            if (((options & ~(CompareOptions.IgnoreWidth | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase)) != CompareOptions.None) && (options != CompareOptions.Ordinal))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidFlag"), "options");
            }
            if (!this.IsSynthetic)
            {
                return IndexOfString(this.m_pSortingTable, this.m_sortingLCID, source, value, startIndex, count, (int) options);
            }
            if (options == CompareOptions.Ordinal)
            {
                return IndexOfString(CultureInfo.InvariantCulture.CompareInfo.m_pSortingTable, this.m_sortingLCID, source, value, startIndex, count, (int) options);
            }
            return this.SyntheticIndexOf(source, value, startIndex, count, GetNativeCompareFlags(options));
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern unsafe int IndexOfChar(void* pSortingTable, int sortingLCID, string source, char value, int startIndex, int count, int options);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern unsafe int IndexOfString(void* pSortingTable, int sortingLCID, string source, string value, int startIndex, int count, int options);
        private static unsafe void* InitializeCompareInfo(void* pNativeGlobalizationAssembly, int sortingLCID)
        {
            void* voidPtr = null;
            bool tookLock = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                Monitor.ReliableEnter(typeof(System.Globalization.CultureTableRecord), ref tookLock);
                voidPtr = InitializeNativeCompareInfo(pNativeGlobalizationAssembly, sortingLCID);
            }
            finally
            {
                if (tookLock)
                {
                    Monitor.Exit(typeof(System.Globalization.CultureTableRecord));
                }
            }
            return voidPtr;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern unsafe void* InitializeNativeCompareInfo(void* pNativeGlobalizationAssembly, int sortingLCID);
        public virtual bool IsPrefix(string source, string prefix)
        {
            return this.IsPrefix(source, prefix, CompareOptions.None);
        }

        public virtual unsafe bool IsPrefix(string source, string prefix, CompareOptions options)
        {
            if ((source == null) || (prefix == null))
            {
                throw new ArgumentNullException((source == null) ? "source" : "prefix", Environment.GetResourceString("ArgumentNull_String"));
            }
            if (prefix.Length == 0)
            {
                return true;
            }
            if (options == CompareOptions.OrdinalIgnoreCase)
            {
                return source.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
            }
            if (((options & ~(CompareOptions.IgnoreWidth | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase)) != CompareOptions.None) && (options != CompareOptions.Ordinal))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidFlag"), "options");
            }
            if (!this.IsSynthetic)
            {
                return nativeIsPrefix(this.m_pSortingTable, this.m_sortingLCID, source, prefix, options);
            }
            if (options == CompareOptions.Ordinal)
            {
                return nativeIsPrefix(CultureInfo.InvariantCulture.CompareInfo.m_pSortingTable, this.m_sortingLCID, source, prefix, options);
            }
            return this.SyntheticIsPrefix(source, 0, source.Length, prefix, GetNativeCompareFlags(options));
        }

        [ComVisible(false)]
        public static bool IsSortable(char ch)
        {
            return IsSortable(ch.ToString());
        }

        [ComVisible(false)]
        public static unsafe bool IsSortable(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (text.Length == 0)
            {
                return false;
            }
            return nativeIsSortable(CultureInfo.InvariantCulture.CompareInfo.m_pSortingTable, text);
        }

        public virtual bool IsSuffix(string source, string suffix)
        {
            return this.IsSuffix(source, suffix, CompareOptions.None);
        }

        public virtual unsafe bool IsSuffix(string source, string suffix, CompareOptions options)
        {
            if ((source == null) || (suffix == null))
            {
                throw new ArgumentNullException((source == null) ? "source" : "suffix", Environment.GetResourceString("ArgumentNull_String"));
            }
            if (suffix.Length == 0)
            {
                return true;
            }
            if (options == CompareOptions.OrdinalIgnoreCase)
            {
                return source.EndsWith(suffix, StringComparison.OrdinalIgnoreCase);
            }
            if (((options & ~(CompareOptions.IgnoreWidth | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase)) != CompareOptions.None) && (options != CompareOptions.Ordinal))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidFlag"), "options");
            }
            if (!this.IsSynthetic)
            {
                return nativeIsSuffix(this.m_pSortingTable, this.m_sortingLCID, source, suffix, options);
            }
            if (options == CompareOptions.Ordinal)
            {
                return nativeIsSuffix(CultureInfo.InvariantCulture.CompareInfo.m_pSortingTable, this.m_sortingLCID, source, suffix, options);
            }
            return (this.SyntheticIsSuffix(source, source.Length - 1, source.Length, suffix, GetNativeCompareFlags(options)) >= 0);
        }

        public virtual int LastIndexOf(string source, char value)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return this.LastIndexOf(source, value, source.Length - 1, source.Length, CompareOptions.None);
        }

        public virtual int LastIndexOf(string source, string value)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return this.LastIndexOf(source, value, source.Length - 1, source.Length, CompareOptions.None);
        }

        public virtual int LastIndexOf(string source, char value, CompareOptions options)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return this.LastIndexOf(source, value, source.Length - 1, source.Length, options);
        }

        public virtual int LastIndexOf(string source, char value, int startIndex)
        {
            return this.LastIndexOf(source, value, startIndex, startIndex + 1, CompareOptions.None);
        }

        public virtual int LastIndexOf(string source, string value, CompareOptions options)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return this.LastIndexOf(source, value, source.Length - 1, source.Length, options);
        }

        public virtual int LastIndexOf(string source, string value, int startIndex)
        {
            return this.LastIndexOf(source, value, startIndex, startIndex + 1, CompareOptions.None);
        }

        public virtual int LastIndexOf(string source, char value, int startIndex, CompareOptions options)
        {
            return this.LastIndexOf(source, value, startIndex, startIndex + 1, options);
        }

        public virtual int LastIndexOf(string source, char value, int startIndex, int count)
        {
            return this.LastIndexOf(source, value, startIndex, count, CompareOptions.None);
        }

        public virtual int LastIndexOf(string source, string value, int startIndex, CompareOptions options)
        {
            return this.LastIndexOf(source, value, startIndex, startIndex + 1, options);
        }

        public virtual int LastIndexOf(string source, string value, int startIndex, int count)
        {
            return this.LastIndexOf(source, value, startIndex, count, CompareOptions.None);
        }

        public virtual unsafe int LastIndexOf(string source, char value, int startIndex, int count, CompareOptions options)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if ((((options & ~(CompareOptions.IgnoreWidth | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase)) != CompareOptions.None) && (options != CompareOptions.Ordinal)) && (options != CompareOptions.OrdinalIgnoreCase))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidFlag"), "options");
            }
            if ((source.Length == 0) && ((startIndex == -1) || (startIndex == 0)))
            {
                return -1;
            }
            if ((startIndex < 0) || (startIndex > source.Length))
            {
                throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_Index"));
            }
            if (startIndex == source.Length)
            {
                startIndex--;
                if (count > 0)
                {
                    count--;
                }
            }
            if ((count < 0) || (((startIndex - count) + 1) < 0))
            {
                throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_Count"));
            }
            if (options == CompareOptions.OrdinalIgnoreCase)
            {
                return TextInfo.nativeLastIndexOfCharOrdinalIgnoreCase(TextInfo.InvariantNativeTextInfo, source, value, startIndex, count);
            }
            if (!this.IsSynthetic)
            {
                return LastIndexOfChar(this.m_pSortingTable, this.m_sortingLCID, source, value, startIndex, count, (int) options);
            }
            if (options == CompareOptions.Ordinal)
            {
                return LastIndexOfChar(CultureInfo.InvariantCulture.CompareInfo.m_pSortingTable, this.m_sortingLCID, source, value, startIndex, count, (int) options);
            }
            return this.SyntheticLastIndexOf(source, new string(value, 1), startIndex, count, GetNativeCompareFlags(options));
        }

        public virtual unsafe int LastIndexOf(string source, string value, int startIndex, int count, CompareOptions options)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if ((((options & ~(CompareOptions.IgnoreWidth | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase)) != CompareOptions.None) && (options != CompareOptions.Ordinal)) && (options != CompareOptions.OrdinalIgnoreCase))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidFlag"), "options");
            }
            if ((source.Length == 0) && ((startIndex == -1) || (startIndex == 0)))
            {
                if (value.Length != 0)
                {
                    return -1;
                }
                return 0;
            }
            if ((startIndex < 0) || (startIndex > source.Length))
            {
                throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_Index"));
            }
            if (startIndex == source.Length)
            {
                startIndex--;
                if (count > 0)
                {
                    count--;
                }
                if (((value.Length == 0) && (count >= 0)) && (((startIndex - count) + 1) >= 0))
                {
                    return startIndex;
                }
            }
            if ((count < 0) || (((startIndex - count) + 1) < 0))
            {
                throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_Count"));
            }
            if (options == CompareOptions.OrdinalIgnoreCase)
            {
                return TextInfo.LastIndexOfStringOrdinalIgnoreCase(source, value, startIndex, count);
            }
            if (!this.IsSynthetic)
            {
                return LastIndexOfString(this.m_pSortingTable, this.m_sortingLCID, source, value, startIndex, count, (int) options);
            }
            if (options == CompareOptions.Ordinal)
            {
                return LastIndexOfString(CultureInfo.InvariantCulture.CompareInfo.m_pSortingTable, this.m_sortingLCID, source, value, startIndex, count, (int) options);
            }
            return this.SyntheticLastIndexOf(source, value, startIndex, count, GetNativeCompareFlags(options));
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern unsafe int LastIndexOfChar(void* pSortingTable, int sortingLCID, string source, char value, int startIndex, int count, int options);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern unsafe int LastIndexOfString(void* pSortingTable, int sortingLCID, string source, string value, int startIndex, int count, int options);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int nativeCompareString(int lcid, string string1, int offset1, int length1, string string2, int offset2, int length2, int flags);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern unsafe byte[] nativeCreateSortKey(void* pSortingFile, string pString, int dwFlags, int win32LCID);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern unsafe int nativeGetGlobalizedHashCode(void* pSortingFile, string pString, int dwFlags, int win32LCID);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern unsafe bool nativeIsPrefix(void* pSortingTable, int sortingLCID, string source, string prefix, CompareOptions options);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern unsafe bool nativeIsSortable(void* pSortingFile, string pString);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern unsafe bool nativeIsSuffix(void* pSortingTable, int sortingLCID, string source, string prefix, CompareOptions options);
        private unsafe void OnDeserialized()
        {
            if (this.m_sortingLCID <= 0)
            {
                this.m_sortingLCID = this.GetSortingLCID(this.culture);
            }
            if ((this.m_pSortingTable == null) && !this.IsSynthetic)
            {
                this.m_pSortingTable = InitializeCompareInfo(GlobalizationAssembly.DefaultInstance.pNativeGlobalizationAssembly, this.m_sortingLCID);
            }
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext ctx)
        {
            this.OnDeserialized();
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext ctx)
        {
            this.culture = -1;
            this.m_sortingLCID = -1;
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext ctx)
        {
            this.win32LCID = this.m_sortingLCID;
        }

        internal void SetName(string name)
        {
            this.m_name = name;
        }

        private int SyntheticIndexOf(string source, string value, int start, int length, int nativeCompareFlags)
        {
            if (fFindNLSStringSupported >= 0)
            {
                int num = FindNLSStringWrap(this.m_sortingLCID, nativeCompareFlags | 0x400000, source, start, length, value, value.Length);
                if (num > -1)
                {
                    return (num + start);
                }
                if (num == -1)
                {
                    return num;
                }
            }
            for (int i = 0; i < length; i++)
            {
                if (this.SyntheticIsPrefix(source, start + i, length - i, value, nativeCompareFlags))
                {
                    return (start + i);
                }
            }
            return -1;
        }

        private bool SyntheticIsPrefix(string source, int start, int length, string prefix, int nativeCompareFlags)
        {
            if (fFindNLSStringSupported >= 0)
            {
                int num = FindNLSStringWrap(this.m_sortingLCID, nativeCompareFlags | 0x100000, source, start, length, prefix, prefix.Length);
                if (num >= -1)
                {
                    return (num != -1);
                }
            }
            for (int i = 1; i <= length; i++)
            {
                if (nativeCompareString(this.m_sortingLCID, prefix, 0, prefix.Length, source, start, i, nativeCompareFlags) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        private int SyntheticIsSuffix(string source, int end, int length, string suffix, int nativeCompareFlags)
        {
            if (fFindNLSStringSupported >= 0)
            {
                int num = FindNLSStringWrap(this.m_sortingLCID, nativeCompareFlags | 0x200000, source, 0, length, suffix, suffix.Length);
                if (num >= -1)
                {
                    return num;
                }
            }
            for (int i = 0; i < length; i++)
            {
                if (nativeCompareString(this.m_sortingLCID, suffix, 0, suffix.Length, source, end - i, i + 1, nativeCompareFlags) == 0)
                {
                    return (end - i);
                }
            }
            return -1;
        }

        private int SyntheticLastIndexOf(string source, string value, int startIndex, int length, int nativeCompareFlags)
        {
            if (fFindNLSStringSupported >= 0)
            {
                int num = FindNLSStringWrap(this.m_sortingLCID, nativeCompareFlags | 0x800000, source, (startIndex - length) + 1, length, value, value.Length);
                if (num > -1)
                {
                    return (num + ((startIndex - length) + 1));
                }
                if (num == -1)
                {
                    return num;
                }
            }
            for (int i = 0; i < length; i++)
            {
                int num3 = this.SyntheticIsSuffix(source, startIndex - i, length - i, value, nativeCompareFlags);
                if (num3 >= 0)
                {
                    return num3;
                }
            }
            return -1;
        }

        void IDeserializationCallback.OnDeserialization(object sender)
        {
            this.OnDeserialized();
        }

        public override string ToString()
        {
            return ("CompareInfo - " + this.culture);
        }

        internal System.Globalization.CultureTableRecord CultureTableRecord
        {
            get
            {
                if (this.m_cultureTableRecord == null)
                {
                    this.m_cultureTableRecord = CultureInfo.GetCultureInfo(this.m_sortingLCID).m_cultureTableRecord;
                }
                return this.m_cultureTableRecord;
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

        private bool IsSynthetic
        {
            get
            {
                return this.CultureTableRecord.IsSynthetic;
            }
        }

        public int LCID
        {
            get
            {
                return this.culture;
            }
        }

        [ComVisible(false)]
        public virtual string Name
        {
            get
            {
                if (this.m_name == null)
                {
                    this.m_name = CultureInfo.GetCultureInfo(this.culture).SortName;
                }
                return this.m_name;
            }
        }
    }
}

