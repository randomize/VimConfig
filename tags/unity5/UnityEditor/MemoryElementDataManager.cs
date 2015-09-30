namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEditorInternal;

    internal class MemoryElementDataManager
    {
        private static List<MemoryElement> GenerateObjectTypeGroups(UnityEditor.ObjectInfo[] memory, ObjectTypeFilter filter)
        {
            List<MemoryElement> list = new List<MemoryElement>();
            MemoryElement item = null;
            foreach (UnityEditor.ObjectInfo info in memory)
            {
                if (GetObjectTypeFilter(info) == filter)
                {
                    if ((item == null) || (info.className != item.name))
                    {
                        item = new MemoryElement(info.className);
                        list.Add(item);
                    }
                    item.AddChild(new MemoryElement(info, true));
                }
            }
            list.Sort(new Comparison<MemoryElement>(MemoryElementDataManager.SortByMemorySize));
            foreach (MemoryElement element2 in list)
            {
                element2.children.Sort(new Comparison<MemoryElement>(MemoryElementDataManager.SortByMemorySize));
                if ((filter == ObjectTypeFilter.Other) && !HasValidNames(element2.children))
                {
                    element2.children.Clear();
                }
            }
            return list;
        }

        private static ObjectTypeFilter GetObjectTypeFilter(UnityEditor.ObjectInfo info)
        {
            switch (info.reason)
            {
                case 1:
                    return ObjectTypeFilter.BuiltinResource;

                case 2:
                    return ObjectTypeFilter.DontSave;

                case 3:
                case 8:
                case 9:
                    return ObjectTypeFilter.Asset;

                case 10:
                    return ObjectTypeFilter.Other;
            }
            return ObjectTypeFilter.Scene;
        }

        public static MemoryElement GetTreeRoot(ObjectMemoryInfo[] memoryObjectList, int[] referencesIndices)
        {
            UnityEditor.ObjectInfo[] array = new UnityEditor.ObjectInfo[memoryObjectList.Length];
            for (int i = 0; i < memoryObjectList.Length; i++)
            {
                array[i] = new UnityEditor.ObjectInfo { instanceId = memoryObjectList[i].instanceId, memorySize = memoryObjectList[i].memorySize, reason = memoryObjectList[i].reason, name = memoryObjectList[i].name, className = memoryObjectList[i].className };
            }
            int num2 = 0;
            for (int j = 0; j < memoryObjectList.Length; j++)
            {
                for (int k = 0; k < memoryObjectList[j].count; k++)
                {
                    int index = referencesIndices[k + num2];
                    if (array[index].referencedBy == null)
                    {
                        array[index].referencedBy = new List<UnityEditor.ObjectInfo>();
                    }
                    array[index].referencedBy.Add(array[j]);
                }
                num2 += memoryObjectList[j].count;
            }
            MemoryElement element = new MemoryElement();
            Array.Sort<UnityEditor.ObjectInfo>(array, new Comparison<UnityEditor.ObjectInfo>(MemoryElementDataManager.SortByMemoryClassName));
            element.AddChild(new MemoryElement("Scene Memory", GenerateObjectTypeGroups(array, ObjectTypeFilter.Scene)));
            element.AddChild(new MemoryElement("Assets", GenerateObjectTypeGroups(array, ObjectTypeFilter.Asset)));
            element.AddChild(new MemoryElement("Builtin Resources", GenerateObjectTypeGroups(array, ObjectTypeFilter.BuiltinResource)));
            element.AddChild(new MemoryElement("Not Saved", GenerateObjectTypeGroups(array, ObjectTypeFilter.DontSave)));
            element.AddChild(new MemoryElement("Other", GenerateObjectTypeGroups(array, ObjectTypeFilter.Other)));
            element.children.Sort(new Comparison<MemoryElement>(MemoryElementDataManager.SortByMemorySize));
            return element;
        }

        private static bool HasValidNames(List<MemoryElement> memory)
        {
            for (int i = 0; i < memory.Count; i++)
            {
                if (!string.IsNullOrEmpty(memory[i].name))
                {
                    return true;
                }
            }
            return false;
        }

        private static int SortByMemoryClassName(UnityEditor.ObjectInfo x, UnityEditor.ObjectInfo y)
        {
            return y.className.CompareTo(x.className);
        }

        private static int SortByMemorySize(MemoryElement x, MemoryElement y)
        {
            return y.totalMemory.CompareTo(x.totalMemory);
        }

        private enum ObjectTypeFilter
        {
            Scene,
            Asset,
            BuiltinResource,
            DontSave,
            Other
        }
    }
}

