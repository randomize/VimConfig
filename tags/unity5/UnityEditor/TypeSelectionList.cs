namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class TypeSelectionList
    {
        private List<TypeSelection> m_TypeSelections;

        public TypeSelectionList(UnityEngine.Object[] objects)
        {
            Dictionary<string, List<UnityEngine.Object>> dictionary = new Dictionary<string, List<UnityEngine.Object>>();
            foreach (UnityEngine.Object obj2 in objects)
            {
                string typeName = ObjectNames.GetTypeName(obj2);
                if (!dictionary.ContainsKey(typeName))
                {
                    dictionary[typeName] = new List<UnityEngine.Object>();
                }
                dictionary[typeName].Add(obj2);
            }
            this.m_TypeSelections = new List<TypeSelection>();
            foreach (KeyValuePair<string, List<UnityEngine.Object>> pair in dictionary)
            {
                this.m_TypeSelections.Add(new TypeSelection(pair.Key, pair.Value.ToArray()));
            }
            this.m_TypeSelections.Sort();
        }

        public List<TypeSelection> typeSelections
        {
            get
            {
                return this.m_TypeSelections;
            }
        }
    }
}

