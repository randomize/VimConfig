namespace UnityEditor.Animations
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class AvatarMask : UnityEngine.Object
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern AvatarMask();
        internal void Copy(AvatarMask other)
        {
            for (int i = 0; i < this.humanoidBodyPartCount; i++)
            {
                this.SetHumanoidBodyPartActive(i, other.GetHumanoidBodyPartActive(i));
            }
            this.transformCount = other.transformCount;
            for (int j = 0; j < other.transformCount; j++)
            {
                this.SetTransformPath(j, other.GetTransformPath(j));
                this.SetTransformActive(j, other.GetTransformActive(j));
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool GetHumanoidBodyPartActive(int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool GetTransformActive(int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string GetTransformPath(int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetHumanoidBodyPartActive(int index, bool value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetTransformActive(int index, bool value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetTransformPath(int index, string path);

        internal bool hasFeetIK { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int humanoidBodyPartCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int transformCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

