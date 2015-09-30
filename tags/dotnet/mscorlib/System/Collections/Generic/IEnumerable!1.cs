namespace System.Collections.Generic
{
    using System.Collections;
    using System.Runtime.CompilerServices;

    [TypeDependency("System.SZArrayHelper")]
    public interface IEnumerable<T> : IEnumerable
    {
        IEnumerator<T> GetEnumerator();
    }
}

