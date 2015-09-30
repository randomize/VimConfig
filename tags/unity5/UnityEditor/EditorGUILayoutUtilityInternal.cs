namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal sealed class EditorGUILayoutUtilityInternal : GUILayoutUtility
    {
        internal static GUILayoutGroup BeginLayoutArea(GUIStyle style, System.Type LayoutType)
        {
            return GUILayoutUtility.DoBeginLayoutArea(style, LayoutType);
        }

        internal static GUILayoutGroup topLevel
        {
            get
            {
                return GUILayoutUtility.topLevel;
            }
        }
    }
}

