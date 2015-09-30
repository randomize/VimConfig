namespace UnityEngine.UI
{
    using System;
    using UnityEngine;

    internal static class Misc
    {
        public static void Destroy(UnityEngine.Object obj)
        {
            if (obj != null)
            {
                if (Application.isPlaying)
                {
                    if (obj is GameObject)
                    {
                        GameObject obj2 = obj as GameObject;
                        obj2.transform.parent = null;
                    }
                    UnityEngine.Object.Destroy(obj);
                }
                else
                {
                    UnityEngine.Object.DestroyImmediate(obj);
                }
            }
        }

        public static void DestroyImmediate(UnityEngine.Object obj)
        {
            if (obj != null)
            {
                if (Application.isEditor)
                {
                    UnityEngine.Object.DestroyImmediate(obj);
                }
                else
                {
                    UnityEngine.Object.Destroy(obj);
                }
            }
        }
    }
}

