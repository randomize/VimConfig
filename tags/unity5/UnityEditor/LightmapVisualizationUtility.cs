namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngineInternal;

    internal sealed class LightmapVisualizationUtility
    {
        public static void DrawTextureWithUVOverlay(Texture2D texture, GameObject gameObject, Rect drawableArea, Rect position, GITextureType textureType, bool drawSpecularUV)
        {
            INTERNAL_CALL_DrawTextureWithUVOverlay(texture, gameObject, ref drawableArea, ref position, textureType, drawSpecularUV);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Texture2D GetGITexture(GITextureType textureType);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Vector4 GetLightmapTilingOffset(LightmapType lightmapType);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_DrawTextureWithUVOverlay(Texture2D texture, GameObject gameObject, ref Rect drawableArea, ref Rect position, GITextureType textureType, bool drawSpecularUV);
    }
}

