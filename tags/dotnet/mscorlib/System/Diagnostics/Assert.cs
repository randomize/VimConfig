namespace System.Diagnostics
{
    using System;
    using System.Runtime.CompilerServices;

    internal static class Assert
    {
        private static int iFilterArraySize;
        private static int iNumOfFilters;
        private static AssertFilter[] ListOfFilters;

        static Assert()
        {
            AddFilter(new DefaultFilter());
        }

        public static void AddFilter(AssertFilter filter)
        {
            if (iFilterArraySize <= iNumOfFilters)
            {
                AssertFilter[] destinationArray = new AssertFilter[iFilterArraySize + 2];
                if (iNumOfFilters > 0)
                {
                    Array.Copy(ListOfFilters, destinationArray, iNumOfFilters);
                }
                iFilterArraySize += 2;
                ListOfFilters = destinationArray;
            }
            ListOfFilters[iNumOfFilters++] = filter;
        }

        public static void Check(bool condition, string conditionString, string message)
        {
            if (!condition)
            {
                Fail(conditionString, message);
            }
        }

        public static void Fail(string conditionString, string message)
        {
            StackTrace location = new StackTrace();
            int iNumOfFilters = Assert.iNumOfFilters;
            while (iNumOfFilters > 0)
            {
                AssertFilters filters = ListOfFilters[--iNumOfFilters].AssertFailure(conditionString, message, location);
                if (filters == AssertFilters.FailDebug)
                {
                    if (Debugger.IsAttached)
                    {
                        Debugger.Break();
                        return;
                    }
                    if (!Debugger.Launch())
                    {
                        throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_DebuggerLaunchFailed"));
                    }
                    break;
                }
                if (filters == AssertFilters.FailTerminate)
                {
                    Environment.Exit(-1);
                }
                else if (filters == AssertFilters.FailIgnore)
                {
                    return;
                }
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern int ShowDefaultAssertDialog(string conditionString, string message);
    }
}

