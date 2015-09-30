namespace UnityEngine.Audio
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class AudioMixerSnapshot : UnityEngine.Object
    {
        internal AudioMixerSnapshot()
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void TransitionTo(float timeToReach);

        public AudioMixer audioMixer { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

