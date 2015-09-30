namespace UnityEngine
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public sealed class AudioHighPassFilter : Behaviour
    {
        public float cutoffFrequency { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("AudioHighPassFilter.highpassResonaceQ is obsolete. Use highpassResonanceQ instead (UnityUpgradable) -> highpassResonanceQ", true)]
        public float highpassResonaceQ
        {
            get
            {
                return this.highpassResonanceQ;
            }
            set
            {
            }
        }

        public float highpassResonanceQ { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

