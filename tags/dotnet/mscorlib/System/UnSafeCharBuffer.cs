namespace System
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct UnSafeCharBuffer
    {
        private unsafe char* m_buffer;
        private int m_totalSize;
        private int m_length;
        public unsafe UnSafeCharBuffer(char* buffer, int bufferSize)
        {
            this.m_buffer = buffer;
            this.m_totalSize = bufferSize;
            this.m_length = 0;
        }

        public unsafe void AppendString(string stringToAppend)
        {
            if (!string.IsNullOrEmpty(stringToAppend))
            {
                if ((this.m_totalSize - this.m_length) < stringToAppend.Length)
                {
                    throw new IndexOutOfRangeException();
                }
                fixed (char* str = ((char*) stringToAppend))
                {
                    char* chPtr = str;
                    Buffer.memcpyimpl((byte*) chPtr, (byte*) (this.m_buffer + this.m_length), stringToAppend.Length * 2);
                }
                this.m_length += stringToAppend.Length;
            }
        }

        public int Length
        {
            get
            {
                return this.m_length;
            }
        }
    }
}

