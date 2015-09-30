namespace System.Collections.Generic
{
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;

    [Serializable, TypeDependency("System.Collections.Generic.GenericComparer`1")]
    public abstract class Comparer<T> : IComparer, IComparer<T>
    {
        private static Comparer<T> defaultComparer;

        protected Comparer()
        {
        }

        public abstract int Compare(T x, T y);
        private static Comparer<T> CreateComparer()
        {
            Type c = typeof(T);
            if (typeof(IComparable<T>).IsAssignableFrom(c))
            {
                return (Comparer<T>) typeof(GenericComparer<int>).TypeHandle.CreateInstanceForAnotherGenericParameter(c);
            }
            if (c.IsGenericType && (c.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                Type type2 = c.GetGenericArguments()[0];
                if (typeof(IComparable<>).MakeGenericType(new Type[] { type2 }).IsAssignableFrom(type2))
                {
                    return (Comparer<T>) typeof(NullableComparer<int>).TypeHandle.CreateInstanceForAnotherGenericParameter(type2);
                }
            }
            return new ObjectComparer<T>();
        }

        int IComparer.Compare(object x, object y)
        {
            if (x == null)
            {
                if (y != null)
                {
                    return -1;
                }
                return 0;
            }
            if (y == null)
            {
                return 1;
            }
            if ((x is T) && (y is T))
            {
                return this.Compare((T) x, (T) y);
            }
            ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArgumentForComparison);
            return 0;
        }

        public static Comparer<T> Default
        {
            get
            {
                Comparer<T> defaultComparer = Comparer<T>.defaultComparer;
                if (defaultComparer == null)
                {
                    defaultComparer = Comparer<T>.CreateComparer();
                    Comparer<T>.defaultComparer = defaultComparer;
                }
                return defaultComparer;
            }
        }
    }
}

