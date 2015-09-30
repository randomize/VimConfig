namespace System
{
    using System.Diagnostics;

    internal sealed class ASSERT : Exception
    {
        private static void Assert()
        {
            Assert(false, null, null);
        }

        private static void Assert(bool condition)
        {
            Assert(condition, null, null);
        }

        private static void Assert(string message)
        {
            Assert(false, message, null);
        }

        private static void Assert(bool condition, string message)
        {
            Assert(condition, message, null);
        }

        private static void Assert(string message, string detailedMessage)
        {
            Assert(false, message, detailedMessage);
        }

        private static void Assert(bool condition, string message, string detailedMessage)
        {
        }

        private static bool AssertIsFriend(Type[] friends, StackTrace st)
        {
            Type declaringType = st.GetFrame(1).GetMethod().DeclaringType;
            Type type2 = st.GetFrame(2).GetMethod().DeclaringType;
            bool flag = true;
            foreach (Type type3 in friends)
            {
                if ((type2 != type3) && (type2 != declaringType))
                {
                    flag = false;
                }
            }
            if (flag)
            {
                Assert(false, Environment.GetResourceString("RtType.InvalidCaller"), st.ToString());
            }
            return true;
        }

        [Conditional("_DEBUG")]
        internal static void CONSISTENCY_CHECK(bool condition)
        {
            Assert(condition);
        }

        [Conditional("_DEBUG")]
        internal static void CONSISTENCY_CHECK(bool condition, string message)
        {
            Assert(condition, message);
        }

        [Conditional("_DEBUG")]
        internal static void CONSISTENCY_CHECK(bool condition, string message, string detailedMessage)
        {
            Assert(condition, message, detailedMessage);
        }

        [Conditional("_DEBUG")]
        internal static void FRIEND(Type[] friends)
        {
            StackTrace st = new StackTrace();
            AssertIsFriend(friends, st);
        }

        [Conditional("_DEBUG")]
        internal static void FRIEND(string ns)
        {
            StackTrace trace = new StackTrace();
            string text1 = trace.GetFrame(1).GetMethod().DeclaringType.Namespace;
            string str = trace.GetFrame(2).GetMethod().DeclaringType.Namespace;
            Assert(str.Equals(str) || str.Equals(ns), Environment.GetResourceString("RtType.InvalidCaller"), trace.ToString());
        }

        [Conditional("_DEBUG")]
        internal static void FRIEND(Type friend)
        {
            StackTrace st = new StackTrace();
            AssertIsFriend(new Type[] { friend }, st);
        }

        [Conditional("_DEBUG")]
        internal static void NOT_IMPLEMENTED()
        {
            Assert();
        }

        [Conditional("_DEBUG")]
        internal static void NOT_IMPLEMENTED(string message)
        {
            Assert(message);
        }

        [Conditional("_DEBUG")]
        internal static void NOT_IMPLEMENTED(string message, string detailedMessage)
        {
            Assert(message, detailedMessage);
        }

        [Conditional("_DEBUG")]
        internal static void POSTCONDITION(bool condition)
        {
            Assert(condition);
        }

        [Conditional("_DEBUG")]
        internal static void POSTCONDITION(bool condition, string message)
        {
            Assert(condition, message);
        }

        [Conditional("_DEBUG")]
        internal static void POSTCONDITION(bool condition, string message, string detailedMessage)
        {
            Assert(condition, message, detailedMessage);
        }

        [Conditional("_DEBUG")]
        internal static void PRECONDITION(bool condition)
        {
            Assert(condition);
        }

        [Conditional("_DEBUG")]
        internal static void PRECONDITION(bool condition, string message)
        {
            Assert(condition, message);
        }

        [Conditional("_DEBUG")]
        internal static void PRECONDITION(bool condition, string message, string detailedMessage)
        {
            Assert(condition, message, detailedMessage);
        }

        [Conditional("_DEBUG")]
        internal static void SIMPLIFYING_ASSUMPTION(bool condition)
        {
            Assert(condition);
        }

        [Conditional("_DEBUG")]
        internal static void SIMPLIFYING_ASSUMPTION(bool condition, string message)
        {
            Assert(condition, message);
        }

        [Conditional("_DEBUG")]
        internal static void SIMPLIFYING_ASSUMPTION(bool condition, string message, string detailedMessage)
        {
            Assert(condition, message, detailedMessage);
        }

        [Conditional("_DEBUG")]
        internal static void UNREACHABLE()
        {
            Assert();
        }

        [Conditional("_DEBUG")]
        internal static void UNREACHABLE(string message)
        {
            Assert(message);
        }

        [Conditional("_DEBUG")]
        internal static void UNREACHABLE(string message, string detailedMessage)
        {
            Assert(message, detailedMessage);
        }
    }
}

