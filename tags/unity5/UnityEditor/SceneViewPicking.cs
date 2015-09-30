namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    internal class SceneViewPicking
    {
        private static List<GameObject> GetAllOverlapping(Vector2 position)
        {
            List<GameObject> source = new List<GameObject>();
            while (true)
            {
                GameObject item = HandleUtility.PickGameObject(position, false, source.ToArray());
                if ((source.Count > 0) && (item == source.Last<GameObject>()))
                {
                    Debug.LogError("GetAllOverlapping failed, could not ignore game object '" + item.name + "' when picking");
                    return source;
                }
                if (item == null)
                {
                    return source;
                }
                source.Add(item);
            }
        }

        public static GameObject PickGameObject(Vector2 mousePosition)
        {
            List<GameObject> allOverlapping = GetAllOverlapping(mousePosition);
            if (allOverlapping.Count == 0)
            {
                return null;
            }
            GameObject obj2 = HandleUtility.FindSelectionBase(allOverlapping[0]);
            int index = allOverlapping.IndexOf(Selection.activeGameObject);
            if (obj2 != null)
            {
                if ((obj2 != Selection.activeGameObject) && (index == -1))
                {
                    return obj2;
                }
                if ((obj2 == Selection.activeGameObject) && (index != 0))
                {
                    return HandleUtility.PickGameObject(mousePosition, false);
                }
                if (((index + 1) < allOverlapping.Count) && (allOverlapping[index + 1] == obj2))
                {
                    index++;
                }
            }
            if ((index + 1) < allOverlapping.Count)
            {
                return allOverlapping[index + 1];
            }
            if (obj2 != null)
            {
                return obj2;
            }
            return HandleUtility.PickGameObject(mousePosition, false);
        }
    }
}

