namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public sealed class Gradient
    {
        internal IntPtr m_Ptr;
        public Gradient()
        {
            this.Init();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Init();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Cleanup();
        ~Gradient()
        {
            this.Cleanup();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern Color Evaluate(float time);
        public GradientColorKey[] colorKeys { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        public GradientAlphaKey[] alphaKeys { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        internal Color constantColor
        {
            get
            {
                Color color;
                this.INTERNAL_get_constantColor(out color);
                return color;
            }
            set
            {
                this.INTERNAL_set_constantColor(ref value);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_constantColor(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_constantColor(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetKeys(GradientColorKey[] colorKeys, GradientAlphaKey[] alphaKeys);
    }
}

