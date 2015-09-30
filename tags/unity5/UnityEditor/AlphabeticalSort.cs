namespace UnityEditor
{
    using System;
    using UnityEngine;

    public class AlphabeticalSort : BaseHierarchySort
    {
        private readonly GUIContent m_Content = new GUIContent(EditorGUIUtility.FindTexture("AlphabeticalSorting"), "Alphabetical Order");

        public override int Compare(GameObject lhs, GameObject rhs)
        {
            if (lhs == rhs)
            {
                return 0;
            }
            if (lhs == null)
            {
                return -1;
            }
            if (rhs == null)
            {
                return 1;
            }
            return EditorUtility.NaturalCompareObjectNames(lhs, rhs);
        }

        public override GUIContent content
        {
            get
            {
                return this.m_Content;
            }
        }
    }
}

