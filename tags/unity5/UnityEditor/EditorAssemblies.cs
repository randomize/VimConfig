namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal static class EditorAssemblies
    {
        [CompilerGenerated]
        private static Func<Assembly, IEnumerable<System.Type>> <>f__am$cache3;
        internal static List<RuntimeInitializeClassInfo> m_RuntimeInitializeClassInfoList;
        internal static int m_TotalNumRuntimeInitializeMethods;

        private static RuntimeInitializeClassInfo[] GetRuntimeInitializeClassInfos()
        {
            if (m_RuntimeInitializeClassInfoList == null)
            {
                return null;
            }
            return m_RuntimeInitializeClassInfoList.ToArray();
        }

        private static int GetTotalNumRuntimeInitializeMethods()
        {
            return m_TotalNumRuntimeInitializeMethods;
        }

        private static void ProcessEditorInitializeOnLoad(System.Type type)
        {
            try
            {
                RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            }
            catch (TypeInitializationException exception)
            {
                Debug.LogError(exception.InnerException);
            }
        }

        private static void ProcessInitializeOnLoadAttributes()
        {
            m_TotalNumRuntimeInitializeMethods = 0;
            m_RuntimeInitializeClassInfoList = new List<RuntimeInitializeClassInfo>();
            IEnumerator<System.Type> enumerator = loadedTypes.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    System.Type current = enumerator.Current;
                    if (current.IsDefined(typeof(InitializeOnLoadAttribute), false))
                    {
                        ProcessEditorInitializeOnLoad(current);
                    }
                    ProcessStaticMethodAttributes(current);
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
        }

        private static void ProcessStaticMethodAttributes(System.Type type)
        {
            List<string> methodNames = null;
            List<RuntimeInitializeLoadType> loadTypes = null;
            MethodInfo[] methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            for (int i = 0; i < methods.GetLength(0); i++)
            {
                MethodInfo element = methods[i];
                if (Attribute.IsDefined(element, typeof(RuntimeInitializeOnLoadMethodAttribute)))
                {
                    RuntimeInitializeLoadType afterSceneLoad = RuntimeInitializeLoadType.AfterSceneLoad;
                    object[] customAttributes = element.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if ((customAttributes != null) && (customAttributes.Length > 0))
                    {
                        afterSceneLoad = ((RuntimeInitializeOnLoadMethodAttribute) customAttributes[0]).loadType;
                    }
                    if (methodNames == null)
                    {
                        methodNames = new List<string>();
                        loadTypes = new List<RuntimeInitializeLoadType>();
                    }
                    methodNames.Add(element.Name);
                    loadTypes.Add(afterSceneLoad);
                }
                if (Attribute.IsDefined(element, typeof(InitializeOnLoadMethodAttribute)))
                {
                    element.Invoke(null, null);
                }
            }
            if (methodNames != null)
            {
                StoreRuntimeInitializeClassInfo(type, methodNames, loadTypes);
            }
        }

        private static void SetLoadedEditorAssemblies(Assembly[] assemblies)
        {
            loadedAssemblies = assemblies;
            ProcessInitializeOnLoadAttributes();
        }

        private static void StoreRuntimeInitializeClassInfo(System.Type type, List<string> methodNames, List<RuntimeInitializeLoadType> loadTypes)
        {
            RuntimeInitializeClassInfo item = new RuntimeInitializeClassInfo {
                assemblyName = type.Assembly.GetName().Name.ToString(),
                className = type.ToString(),
                methodNames = methodNames.ToArray(),
                loadTypes = loadTypes.ToArray()
            };
            m_RuntimeInitializeClassInfoList.Add(item);
            m_TotalNumRuntimeInitializeMethods += methodNames.Count;
        }

        internal static IEnumerable<System.Type> SubclassesOf(System.Type parent)
        {
            <SubclassesOf>c__AnonStorey2E storeye = new <SubclassesOf>c__AnonStorey2E {
                parent = parent
            };
            return loadedTypes.Where<System.Type>(new Func<System.Type, bool>(storeye.<>m__3C));
        }

        internal static Assembly[] loadedAssemblies
        {
            [CompilerGenerated]
            get
            {
                return <loadedAssemblies>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                <loadedAssemblies>k__BackingField = value;
            }
        }

        internal static IEnumerable<System.Type> loadedTypes
        {
            get
            {
                if (<>f__am$cache3 == null)
                {
                    <>f__am$cache3 = (Func<Assembly, IEnumerable<System.Type>>) (assembly => AssemblyHelper.GetTypesFromAssembly(assembly));
                }
                return loadedAssemblies.SelectMany<Assembly, System.Type>(<>f__am$cache3);
            }
        }

        [CompilerGenerated]
        private sealed class <SubclassesOf>c__AnonStorey2E
        {
            internal System.Type parent;

            internal bool <>m__3C(System.Type klass)
            {
                return klass.IsSubclassOf(this.parent);
            }
        }
    }
}

