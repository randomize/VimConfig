namespace UnityEngine.Sprites
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public sealed class DataUtility
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Vector4 GetInnerUV(Sprite sprite);
        public static Vector2 GetMinSize(Sprite sprite)
        {
            Vector2 vector;
            Internal_GetMinSize(sprite, out vector);
            return vector;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Vector4 GetOuterUV(Sprite sprite);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Vector4 GetPadding(Sprite sprite);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_GetMinSize(Sprite sprite, out Vector2 output);
    }
}

