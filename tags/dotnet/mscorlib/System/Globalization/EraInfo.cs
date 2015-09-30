namespace System.Globalization
{
    using System;

    [Serializable]
    internal class EraInfo
    {
        internal int era;
        internal int maxEraYear;
        internal int minEraYear;
        internal long ticks;
        internal int yearOffset;

        internal EraInfo(int era, long ticks, int yearOffset, int minEraYear, int maxEraYear)
        {
            this.era = era;
            this.ticks = ticks;
            this.yearOffset = yearOffset;
            this.minEraYear = minEraYear;
            this.maxEraYear = maxEraYear;
        }
    }
}

