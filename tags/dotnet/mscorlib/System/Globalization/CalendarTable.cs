namespace System.Globalization
{
    using System;
    using System.Runtime.CompilerServices;

    internal class CalendarTable : BaseInfoTable
    {
        private unsafe CalendarTableData* m_calendars;
        private static CalendarTable m_defaultInstance = new CalendarTable("culture.nlp", true);

        internal CalendarTable(string fileName, bool fromAssembly) : base(fileName, fromAssembly)
        {
        }

        internal unsafe int ICURRENTERA(int id)
        {
            return this.m_calendars[id].iCurrentEra;
        }

        internal unsafe int IFORMATFLAGS(int id)
        {
            return this.m_calendars[id].iFormatFlags;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern string nativeGetEraName(int culture, int calID);
        internal unsafe string[] SABBREVDAYNAMES(int id)
        {
            return base.GetStringArray(this.m_calendars[id].saAbbrevDayNames);
        }

        internal unsafe string[] SABBREVENGERANAMES(int id)
        {
            return base.GetStringArray(this.m_calendars[id].saAbbrevEnglishEraNames);
        }

        internal unsafe string[] SABBREVERANAMES(int id)
        {
            return base.GetStringArray(this.m_calendars[id].saAbbrevEraNames);
        }

        internal unsafe string[] SABBREVMONTHNAMES(int id)
        {
            return base.GetStringArray(this.m_calendars[id].saAbbrevMonthNames);
        }

        internal unsafe string[] SDAYNAMES(int id)
        {
            return base.GetStringArray(this.m_calendars[id].saDayNames);
        }

        internal unsafe string[] SERANAMES(int id)
        {
            return base.GetStringArray(this.m_calendars[id].saEraNames);
        }

        internal unsafe int[][] SERARANGES(int id)
        {
            return base.GetWordArrayArray(this.m_calendars[id].waaEraRanges);
        }

        internal override unsafe void SetDataItemPointers()
        {
            base.m_itemSize = base.m_pCultureHeader.sizeCalendarItem;
            base.m_numItem = base.m_pCultureHeader.numCalendarItems;
            base.m_pDataPool = (ushort*) (base.m_pDataFileStart + base.m_pCultureHeader.offsetToDataPool);
            base.m_pItemData = (byte*) ((base.m_pDataFileStart + base.m_pCultureHeader.offsetToCalendarItemData) - base.m_itemSize);
            this.m_calendars = (CalendarTableData*) ((base.m_pDataFileStart + base.m_pCultureHeader.offsetToCalendarItemData) - sizeof(CalendarTableData));
        }

        internal unsafe string[] SLEAPYEARMONTHNAMES(int id)
        {
            return base.GetStringArray(this.m_calendars[id].saLeapYearMonthNames);
        }

        internal unsafe string[] SLONGDATE(int id)
        {
            return base.GetStringArray(this.m_calendars[id].saLongDate);
        }

        internal unsafe string SMONTHDAY(int id)
        {
            return base.GetStringPoolString(this.m_calendars[id].sMonthDay);
        }

        internal unsafe string[] SMONTHNAMES(int id)
        {
            return base.GetStringArray(this.m_calendars[id].saMonthNames);
        }

        internal unsafe string[] SSHORTDATE(int id)
        {
            return base.GetStringArray(this.m_calendars[id].saShortDate);
        }

        internal unsafe string[] SSUPERSHORTDAYNAMES(int id)
        {
            return base.GetStringArray(this.m_calendars[id].saSuperShortDayNames);
        }

        internal unsafe string[] SYEARMONTH(int id)
        {
            return base.GetStringArray(this.m_calendars[id].saYearMonth);
        }

        internal static CalendarTable Default
        {
            get
            {
                return m_defaultInstance;
            }
        }
    }
}

