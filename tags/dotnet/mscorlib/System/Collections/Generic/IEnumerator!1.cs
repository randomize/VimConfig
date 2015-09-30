namespace System.Collections.Generic
{
    using System;
    using System.Collections;

    public interface IEnumerator<T> : IDisposable, IEnumerator
    {
        T Current { get; }
    }
}

