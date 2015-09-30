namespace UnityEngine
{
    using System;

    public sealed class AndroidJavaException : Exception
    {
        internal AndroidJavaException(string message) : base(message)
        {
        }
    }
}

