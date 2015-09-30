namespace UnityEngine
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), EditorBrowsable(EditorBrowsableState.Never), Obsolete("Use AnimatorClipInfo instead (UnityUpgradable) -> AnimatorClipInfo", true)]
    public struct AnimationInfo
    {
        public AnimationClip clip
        {
            get
            {
                return null;
            }
        }
        public float weight
        {
            get
            {
                return 0f;
            }
        }
    }
}

