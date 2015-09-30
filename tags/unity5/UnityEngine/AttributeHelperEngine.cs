namespace UnityEngine
{
    using System;
    using System.Collections.Generic;

    internal class AttributeHelperEngine
    {
        private static bool CheckIsEditorScript(System.Type klass)
        {
            while ((klass != null) && (klass != typeof(MonoBehaviour)))
            {
                if (klass.GetCustomAttributes(typeof(ExecuteInEditMode), false).Length != 0)
                {
                    return true;
                }
                klass = klass.BaseType;
            }
            return false;
        }

        private static System.Type GetParentTypeDisallowingMultipleInclusion(System.Type type)
        {
            Stack<System.Type> stack = new Stack<System.Type>();
            while ((type != null) && (type != typeof(MonoBehaviour)))
            {
                stack.Push(type);
                type = type.BaseType;
            }
            System.Type type2 = null;
            while (stack.Count > 0)
            {
                type2 = stack.Pop();
                if (type2.GetCustomAttributes(typeof(DisallowMultipleComponent), false).Length != 0)
                {
                    return type2;
                }
            }
            return null;
        }

        private static System.Type[] GetRequiredComponents(System.Type klass)
        {
            List<System.Type> list = null;
            while ((klass != null) && (klass != typeof(MonoBehaviour)))
            {
                RequireComponent[] customAttributes = (RequireComponent[]) klass.GetCustomAttributes(typeof(RequireComponent), false);
                System.Type baseType = klass.BaseType;
                foreach (RequireComponent component in customAttributes)
                {
                    if (((list == null) && (customAttributes.Length == 1)) && (baseType == typeof(MonoBehaviour)))
                    {
                        return new System.Type[] { component.m_Type0, component.m_Type1, component.m_Type2 };
                    }
                    if (list == null)
                    {
                        list = new List<System.Type>();
                    }
                    if (component.m_Type0 != null)
                    {
                        list.Add(component.m_Type0);
                    }
                    if (component.m_Type1 != null)
                    {
                        list.Add(component.m_Type1);
                    }
                    if (component.m_Type2 != null)
                    {
                        list.Add(component.m_Type2);
                    }
                }
                klass = baseType;
            }
            if (list == null)
            {
                return null;
            }
            return list.ToArray();
        }
    }
}

