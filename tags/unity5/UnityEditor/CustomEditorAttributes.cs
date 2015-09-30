namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class CustomEditorAttributes
    {
        private static readonly List<MonoEditorType> kSCustomEditors = new List<MonoEditorType>();
        private static readonly List<MonoEditorType> kSCustomMultiEditors = new List<MonoEditorType>();
        private static bool s_Initialized;

        internal static System.Type FindCustomEditorType(UnityEngine.Object o, bool multiEdit)
        {
            return FindCustomEditorTypeByType(o.GetType(), multiEdit);
        }

        internal static System.Type FindCustomEditorTypeByType(System.Type type, bool multiEdit)
        {
            <FindCustomEditorTypeByType>c__AnonStorey2B storeyb = new <FindCustomEditorTypeByType>c__AnonStorey2B {
                type = type
            };
            if (!s_Initialized)
            {
                Assembly[] loadedAssemblies = EditorAssemblies.loadedAssemblies;
                for (int i = loadedAssemblies.Length - 1; i >= 0; i--)
                {
                    Rebuild(loadedAssemblies[i]);
                }
                s_Initialized = true;
            }
            List<MonoEditorType> source = !multiEdit ? kSCustomEditors : kSCustomMultiEditors;
            <FindCustomEditorTypeByType>c__AnonStorey2D storeyd = new <FindCustomEditorTypeByType>c__AnonStorey2D {
                pass = 0
            };
            while (storeyd.pass < 2)
            {
                <FindCustomEditorTypeByType>c__AnonStorey2C storeyc = new <FindCustomEditorTypeByType>c__AnonStorey2C {
                    <>f__ref$43 = storeyb,
                    <>f__ref$45 = storeyd,
                    inspected = storeyb.type
                };
                while (storeyc.inspected != null)
                {
                    MonoEditorType type2 = source.FirstOrDefault<MonoEditorType>(new Func<MonoEditorType, bool>(storeyc.<>m__3A));
                    if (type2 != null)
                    {
                        return type2.m_InspectorType;
                    }
                    storeyc.inspected = storeyc.inspected.BaseType;
                }
                storeyd.pass++;
            }
            return null;
        }

        internal static void Rebuild(Assembly assembly)
        {
            foreach (System.Type type in AssemblyHelper.GetTypesFromAssembly(assembly))
            {
                foreach (CustomEditor editor in type.GetCustomAttributes(typeof(CustomEditor), false))
                {
                    MonoEditorType item = new MonoEditorType();
                    if (editor.m_InspectedType == null)
                    {
                        Debug.Log("Can't load custom inspector " + type.Name + " because the inspected type is null.");
                    }
                    else if (!type.IsSubclassOf(typeof(Editor)))
                    {
                        if (((type.FullName != "TweakMode") || !type.IsEnum) || (editor.m_InspectedType.FullName != "BloomAndFlares"))
                        {
                            Debug.LogWarning(type.Name + " uses the CustomEditor attribute but does not inherit from Editor.\nYou must inherit from Editor. See the Editor class script documentation.");
                        }
                    }
                    else
                    {
                        item.m_InspectedType = editor.m_InspectedType;
                        item.m_InspectorType = type;
                        item.m_EditorForChildClasses = editor.m_EditorForChildClasses;
                        item.m_IsFallback = editor.isFallback;
                        kSCustomEditors.Add(item);
                        if (type.GetCustomAttributes(typeof(CanEditMultipleObjects), false).Length > 0)
                        {
                            kSCustomMultiEditors.Add(item);
                        }
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <FindCustomEditorTypeByType>c__AnonStorey2B
        {
            internal System.Type type;
        }

        [CompilerGenerated]
        private sealed class <FindCustomEditorTypeByType>c__AnonStorey2C
        {
            internal CustomEditorAttributes.<FindCustomEditorTypeByType>c__AnonStorey2B <>f__ref$43;
            internal CustomEditorAttributes.<FindCustomEditorTypeByType>c__AnonStorey2D <>f__ref$45;
            internal System.Type inspected;

            internal bool <>m__3A(CustomEditorAttributes.MonoEditorType x)
            {
                if ((this.<>f__ref$43.type != this.inspected) && !x.m_EditorForChildClasses)
                {
                    return false;
                }
                if ((this.<>f__ref$45.pass == 1) != x.m_IsFallback)
                {
                    return false;
                }
                return (this.inspected == x.m_InspectedType);
            }
        }

        [CompilerGenerated]
        private sealed class <FindCustomEditorTypeByType>c__AnonStorey2D
        {
            internal int pass;
        }

        private class MonoEditorType
        {
            public bool m_EditorForChildClasses;
            public System.Type m_InspectedType;
            public System.Type m_InspectorType;
            public bool m_IsFallback;
        }
    }
}

