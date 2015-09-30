namespace System
{
    internal static class LOGIC
    {
        internal static bool BIJECTION(bool p, bool q)
        {
            return (IMPLIES(p, q) && IMPLIES(q, p));
        }

        internal static bool IMPLIES(bool p, bool q)
        {
            if (p)
            {
                return q;
            }
            return true;
        }
    }
}

