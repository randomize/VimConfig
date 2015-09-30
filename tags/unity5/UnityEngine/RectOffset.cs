namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public sealed class RectOffset
    {
        [NonSerialized]
        internal IntPtr m_Ptr;
        private readonly GUIStyle m_SourceStyle;
        public RectOffset()
        {
            this.Init();
        }

        internal RectOffset(GUIStyle sourceStyle, IntPtr source)
        {
            this.m_SourceStyle = sourceStyle;
            this.m_Ptr = source;
        }

        public RectOffset(int left, int right, int top, int bottom)
        {
            this.Init();
            this.left = left;
            this.right = right;
            this.top = top;
            this.bottom = bottom;
        }

        ~RectOffset()
        {
            if (this.m_SourceStyle == null)
            {
                this.Cleanup();
            }
        }

        public override string ToString()
        {
            object[] args = new object[] { this.left, this.right, this.top, this.bottom };
            return UnityString.Format("RectOffset (l:{0} r:{1} t:{2} b:{3})", args);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Init();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Cleanup();
        public int left { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        public int right { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        public int top { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        public int bottom { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        public int horizontal { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public int vertical { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public Rect Add(Rect rect)
        {
            return INTERNAL_CALL_Add(this, ref rect);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Rect INTERNAL_CALL_Add(RectOffset self, ref Rect rect);
        public Rect Remove(Rect rect)
        {
            return INTERNAL_CALL_Remove(this, ref rect);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Rect INTERNAL_CALL_Remove(RectOffset self, ref Rect rect);
    }
}

