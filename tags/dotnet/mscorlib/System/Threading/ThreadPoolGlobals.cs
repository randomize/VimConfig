namespace System.Threading
{
    using System;
    using System.Security.Permissions;

    internal static class ThreadPoolGlobals
    {
        public static bool tpHosted = ThreadPool.IsThreadPoolHosted();
        public static uint tpQuantum = 2;
        public static ThreadPoolRequestQueue tpQueue = new ThreadPoolRequestQueue();
        public static int tpWarmupCount = (GetProcessorCount() * 2);
        public static bool vmTpInitialized;

        [EnvironmentPermission(SecurityAction.Assert, Read="NUMBER_OF_PROCESSORS")]
        internal static int GetProcessorCount()
        {
            return Environment.ProcessorCount;
        }
    }
}

