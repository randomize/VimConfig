namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class Selection
    {
        public static System.Action selectionChanged;

        internal static void Add(int instanceID)
        {
            List<int> list = new List<int>(instanceIDs);
            if (list.IndexOf(instanceID) < 0)
            {
                list.Add(instanceID);
                instanceIDs = list.ToArray();
            }
        }

        internal static void Add(UnityEngine.Object obj)
        {
            if (obj != null)
            {
                Add(obj.GetInstanceID());
            }
        }

        public static bool Contains(int instanceID)
        {
            return (Array.IndexOf<int>(instanceIDs, instanceID) != -1);
        }

        public static bool Contains(UnityEngine.Object obj)
        {
            return Contains(obj.GetInstanceID());
        }

        public static UnityEngine.Object[] GetFiltered(System.Type type, UnityEditor.SelectionMode mode)
        {
            ArrayList list = new ArrayList();
            if ((type == typeof(Component)) || type.IsSubclassOf(typeof(Component)))
            {
                foreach (Transform transform in GetTransforms(mode))
                {
                    Component component = transform.GetComponent(type);
                    if (component != null)
                    {
                        list.Add(component);
                    }
                }
            }
            else if ((type == typeof(GameObject)) || type.IsSubclassOf(typeof(GameObject)))
            {
                foreach (Transform transform2 in GetTransforms(mode))
                {
                    list.Add(transform2.gameObject);
                }
            }
            else
            {
                foreach (UnityEngine.Object obj2 in GetObjectsMode(mode))
                {
                    if ((obj2 != null) && ((obj2.GetType() == type) || obj2.GetType().IsSubclassOf(type)))
                    {
                        list.Add(obj2);
                    }
                }
            }
            return (UnityEngine.Object[]) list.ToArray(typeof(UnityEngine.Object));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern UnityEngine.Object[] GetObjectsMode(UnityEditor.SelectionMode mode);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Transform[] GetTransforms(UnityEditor.SelectionMode mode);
        private static void Internal_CallSelectionChanged()
        {
            if (selectionChanged != null)
            {
                selectionChanged();
            }
        }

        internal static void Remove(int instanceID)
        {
            List<int> list = new List<int>(instanceIDs);
            list.Remove(instanceID);
            instanceIDs = list.ToArray();
        }

        internal static void Remove(UnityEngine.Object obj)
        {
            if (obj != null)
            {
                Remove(obj.GetInstanceID());
            }
        }

        public static GameObject activeGameObject { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static int activeInstanceID { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static UnityEngine.Object activeObject { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static Transform activeTransform { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static string[] assetGUIDs { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        internal static string[] assetGUIDsDeepSelection { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static GameObject[] gameObjects { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int[] instanceIDs { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static UnityEngine.Object[] objects { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static Transform[] transforms { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

