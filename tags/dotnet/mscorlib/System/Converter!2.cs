namespace System
{
    using System.Runtime.CompilerServices;

    public delegate TOutput Converter<TInput, TOutput>(TInput input);
}

