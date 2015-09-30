namespace UnityEngine.Networking
{
    using System;

    public class LogFilter
    {
        public static FilterLevel current = FilterLevel.Info;
        public const int Debug = 1;
        internal const int Developer = 0;
        public const int Error = 4;
        public const int Fatal = 5;
        public const int Info = 2;
        private static int s_CurrentLogLevel = 2;
        public const int Warn = 3;

        public static int currentLogLevel
        {
            get
            {
                return s_CurrentLogLevel;
            }
            set
            {
                s_CurrentLogLevel = value;
            }
        }

        public static bool logDebug
        {
            get
            {
                return (s_CurrentLogLevel <= 1);
            }
        }

        internal static bool logDev
        {
            get
            {
                return (s_CurrentLogLevel <= 0);
            }
        }

        public static bool logError
        {
            get
            {
                return (s_CurrentLogLevel <= 4);
            }
        }

        public static bool logFatal
        {
            get
            {
                return (s_CurrentLogLevel <= 5);
            }
        }

        public static bool logInfo
        {
            get
            {
                return (s_CurrentLogLevel <= 2);
            }
        }

        public static bool logWarn
        {
            get
            {
                return (s_CurrentLogLevel <= 3);
            }
        }

        public enum FilterLevel
        {
            Developer,
            Debug,
            Info,
            Warn,
            Error,
            Fatal
        }
    }
}

