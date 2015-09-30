namespace UnityEngine.Networking
{
    using System;

    internal class FloatConversion
    {
        public static double ToDouble(ulong value)
        {
            UIntFloat num = new UIntFloat {
                longValue = value
            };
            return num.doubleValue;
        }

        public static float ToSingle(uint value)
        {
            UIntFloat num = new UIntFloat {
                intValue = value
            };
            return num.floatValue;
        }
    }
}

