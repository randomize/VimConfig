namespace UnityEngine.Assertions.Must
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine.Assertions;

    [DebuggerStepThrough]
    public static class MustExtensions
    {
        [Conditional("UNITY_ASSERTIONS")]
        public static void MustBeApproximatelyEqual(this float actual, float expected)
        {
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(actual, expected);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void MustBeApproximatelyEqual(this float actual, float expected, float tolerance)
        {
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(actual, expected, tolerance);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void MustBeApproximatelyEqual(this float actual, float expected, string message)
        {
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(actual, expected, message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void MustBeApproximatelyEqual(this float actual, float expected, float tolerance, string message)
        {
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(expected, actual, tolerance, message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void MustBeEqual<T>(this T actual, T expected)
        {
            UnityEngine.Assertions.Assert.AreEqual<T>(actual, expected);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void MustBeEqual<T>(this T actual, T expected, string message)
        {
            UnityEngine.Assertions.Assert.AreEqual<T>(expected, actual, message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void MustBeFalse(this bool value)
        {
            UnityEngine.Assertions.Assert.IsFalse(value);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void MustBeFalse(this bool value, string message)
        {
            UnityEngine.Assertions.Assert.IsFalse(value, message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void MustBeNull<T>(this T expected) where T: class
        {
            UnityEngine.Assertions.Assert.IsNull<T>(expected);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void MustBeNull<T>(this T expected, string message) where T: class
        {
            UnityEngine.Assertions.Assert.IsNull<T>(expected, message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void MustBeTrue(this bool value)
        {
            UnityEngine.Assertions.Assert.IsTrue(value);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void MustBeTrue(this bool value, string message)
        {
            UnityEngine.Assertions.Assert.IsTrue(value, message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void MustNotBeApproximatelyEqual(this float actual, float expected)
        {
            UnityEngine.Assertions.Assert.AreNotApproximatelyEqual(expected, actual);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void MustNotBeApproximatelyEqual(this float actual, float expected, float tolerance)
        {
            UnityEngine.Assertions.Assert.AreNotApproximatelyEqual(expected, actual, tolerance);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void MustNotBeApproximatelyEqual(this float actual, float expected, string message)
        {
            UnityEngine.Assertions.Assert.AreNotApproximatelyEqual(expected, actual, message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void MustNotBeApproximatelyEqual(this float actual, float expected, float tolerance, string message)
        {
            UnityEngine.Assertions.Assert.AreNotApproximatelyEqual(expected, actual, tolerance, message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void MustNotBeEqual<T>(this T actual, T expected)
        {
            UnityEngine.Assertions.Assert.AreNotEqual<T>(actual, expected);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void MustNotBeEqual<T>(this T actual, T expected, string message)
        {
            UnityEngine.Assertions.Assert.AreNotEqual<T>(expected, actual, message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void MustNotBeNull<T>(this T expected) where T: class
        {
            UnityEngine.Assertions.Assert.IsNotNull<T>(expected);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void MustNotBeNull<T>(this T expected, string message) where T: class
        {
            UnityEngine.Assertions.Assert.IsNotNull<T>(expected, message);
        }
    }
}

