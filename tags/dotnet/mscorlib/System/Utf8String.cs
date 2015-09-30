namespace System
{
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;

    [StructLayout(LayoutKind.Sequential)]
    internal struct Utf8String
    {
        private unsafe void* m_pStringHeap;
        private int m_StringHeapByteLength;
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern unsafe bool EqualsCaseSensitive(void* szLhs, void* szRhs, int cSz);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern unsafe bool EqualsCaseInsensitive(void* szLhs, void* szRhs, int cSz);
        private static unsafe int GetUtf8StringByteLength(void* pUtf8String)
        {
            int num = 0;
            for (byte* numPtr = (byte*) pUtf8String; numPtr[0] != 0; numPtr++)
            {
                num++;
            }
            return num;
        }

        internal unsafe Utf8String(void* pStringHeap)
        {
            this.m_pStringHeap = pStringHeap;
            if (pStringHeap != null)
            {
                this.m_StringHeapByteLength = GetUtf8StringByteLength(pStringHeap);
            }
            else
            {
                this.m_StringHeapByteLength = 0;
            }
        }

        internal unsafe Utf8String(void* pUtf8String, int cUtf8String)
        {
            this.m_pStringHeap = pUtf8String;
            this.m_StringHeapByteLength = cUtf8String;
        }

        internal unsafe bool Equals(Utf8String s)
        {
            if (this.m_pStringHeap == null)
            {
                return (s.m_StringHeapByteLength == 0);
            }
            return (((s.m_StringHeapByteLength == this.m_StringHeapByteLength) && (this.m_StringHeapByteLength != 0)) && EqualsCaseSensitive(s.m_pStringHeap, this.m_pStringHeap, this.m_StringHeapByteLength));
        }

        internal unsafe bool EqualsCaseInsensitive(Utf8String s)
        {
            if (this.m_pStringHeap == null)
            {
                return (s.m_StringHeapByteLength == 0);
            }
            return (((s.m_StringHeapByteLength == this.m_StringHeapByteLength) && (this.m_StringHeapByteLength != 0)) && EqualsCaseInsensitive(s.m_pStringHeap, this.m_pStringHeap, this.m_StringHeapByteLength));
        }

        public override unsafe string ToString()
        {
            byte* bytes = stackalloc byte[1 * this.m_StringHeapByteLength];
            byte* pStringHeap = (byte*) this.m_pStringHeap;
            for (int i = 0; i < this.m_StringHeapByteLength; i++)
            {
                bytes[i] = pStringHeap[0];
                pStringHeap++;
            }
            if (this.m_StringHeapByteLength == 0)
            {
                return "";
            }
            int charCount = Encoding.UTF8.GetCharCount(bytes, this.m_StringHeapByteLength);
            char* chars = (char*) stackalloc byte[(2 * charCount)];
            Encoding.UTF8.GetChars(bytes, this.m_StringHeapByteLength, chars, charCount);
            return new string(chars, 0, charCount);
        }
    }
}

