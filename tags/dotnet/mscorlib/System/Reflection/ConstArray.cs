namespace System.Reflection
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    internal struct ConstArray
    {
        internal int m_length;
        internal IntPtr m_constArray;
        public IntPtr Signature
        {
            get
            {
                return this.m_constArray;
            }
        }
        public int Length
        {
            get
            {
                return this.m_length;
            }
        }
        public byte this[int index]
        {
            get
            {
                if ((index < 0) || (index >= this.m_length))
                {
                    throw new IndexOutOfRangeException();
                }
                return *(((byte*) (this.m_constArray.ToPointer() + index)));
            }
        }
    }
}

