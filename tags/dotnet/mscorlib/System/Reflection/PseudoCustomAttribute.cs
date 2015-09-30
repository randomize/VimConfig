namespace System.Reflection
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    internal static class PseudoCustomAttribute
    {
        private static Hashtable s_pca;
        private static int s_pcasCount;

        static PseudoCustomAttribute()
        {
            Type[] typeArray = new Type[] { typeof(FieldOffsetAttribute), typeof(SerializableAttribute), typeof(MarshalAsAttribute), typeof(ComImportAttribute), typeof(NonSerializedAttribute), typeof(InAttribute), typeof(OutAttribute), typeof(OptionalAttribute), typeof(DllImportAttribute), typeof(PreserveSigAttribute) };
            s_pcasCount = typeArray.Length;
            s_pca = new Hashtable(s_pcasCount);
            for (int i = 0; i < s_pcasCount; i++)
            {
                s_pca[typeArray[i]] = typeArray[i];
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern unsafe void _GetSecurityAttributes(void* module, int token, out object[] securityAttributes);
        internal static Attribute[] GetCustomAttributes(Assembly assembly, Type caType, out int count)
        {
            count = 0;
            bool flag = (caType == typeof(object)) || (caType == typeof(Attribute));
            if ((!flag && (s_pca[caType] == null)) && !IsSecurityAttribute(caType))
            {
                return new Attribute[0];
            }
            List<Attribute> list = new List<Attribute>();
            if (flag || IsSecurityAttribute(caType))
            {
                object[] objArray;
                GetSecurityAttributes(assembly.ManifestModule.ModuleHandle, assembly.AssemblyHandle.GetToken(), out objArray);
                if (objArray != null)
                {
                    foreach (object obj2 in objArray)
                    {
                        if ((caType == obj2.GetType()) || obj2.GetType().IsSubclassOf(caType))
                        {
                            list.Add((Attribute) obj2);
                        }
                    }
                }
            }
            count = list.Count;
            return list.ToArray();
        }

        internal static Attribute[] GetCustomAttributes(Module module, Type caType, out int count)
        {
            count = 0;
            return null;
        }

        internal static Attribute[] GetCustomAttributes(ParameterInfo parameter, Type caType, out int count)
        {
            count = 0;
            bool flag = (caType == typeof(object)) || (caType == typeof(Attribute));
            if (!flag && (s_pca[caType] == null))
            {
                return null;
            }
            Attribute[] attributeArray = new Attribute[s_pcasCount];
            Attribute customAttribute = null;
            if (flag || (caType == typeof(InAttribute)))
            {
                customAttribute = InAttribute.GetCustomAttribute(parameter);
                if (customAttribute != null)
                {
                    attributeArray[count++] = customAttribute;
                }
            }
            if (flag || (caType == typeof(OutAttribute)))
            {
                customAttribute = OutAttribute.GetCustomAttribute(parameter);
                if (customAttribute != null)
                {
                    attributeArray[count++] = customAttribute;
                }
            }
            if (flag || (caType == typeof(OptionalAttribute)))
            {
                customAttribute = OptionalAttribute.GetCustomAttribute(parameter);
                if (customAttribute != null)
                {
                    attributeArray[count++] = customAttribute;
                }
            }
            if (flag || (caType == typeof(MarshalAsAttribute)))
            {
                customAttribute = MarshalAsAttribute.GetCustomAttribute(parameter);
                if (customAttribute != null)
                {
                    attributeArray[count++] = customAttribute;
                }
            }
            return attributeArray;
        }

        internal static Attribute[] GetCustomAttributes(RuntimeEventInfo e, Type caType, out int count)
        {
            count = 0;
            return null;
        }

        internal static Attribute[] GetCustomAttributes(RuntimeFieldInfo field, Type caType, out int count)
        {
            count = 0;
            bool flag = (caType == typeof(object)) || (caType == typeof(Attribute));
            if (!flag && (s_pca[caType] == null))
            {
                return null;
            }
            Attribute[] attributeArray = new Attribute[s_pcasCount];
            Attribute customAttribute = null;
            if (flag || (caType == typeof(MarshalAsAttribute)))
            {
                customAttribute = MarshalAsAttribute.GetCustomAttribute(field);
                if (customAttribute != null)
                {
                    attributeArray[count++] = customAttribute;
                }
            }
            if (flag || (caType == typeof(FieldOffsetAttribute)))
            {
                customAttribute = FieldOffsetAttribute.GetCustomAttribute(field);
                if (customAttribute != null)
                {
                    attributeArray[count++] = customAttribute;
                }
            }
            if (flag || (caType == typeof(NonSerializedAttribute)))
            {
                customAttribute = NonSerializedAttribute.GetCustomAttribute(field);
                if (customAttribute != null)
                {
                    attributeArray[count++] = customAttribute;
                }
            }
            return attributeArray;
        }

        internal static Attribute[] GetCustomAttributes(RuntimePropertyInfo property, Type caType, out int count)
        {
            count = 0;
            return null;
        }

        internal static Attribute[] GetCustomAttributes(RuntimeConstructorInfo ctor, Type caType, bool includeSecCa, out int count)
        {
            count = 0;
            bool flag = (caType == typeof(object)) || (caType == typeof(Attribute));
            if ((!flag && (s_pca[caType] == null)) && !IsSecurityAttribute(caType))
            {
                return new Attribute[0];
            }
            List<Attribute> list = new List<Attribute>();
            if (includeSecCa && (flag || IsSecurityAttribute(caType)))
            {
                object[] objArray;
                GetSecurityAttributes(ctor.Module.ModuleHandle, ctor.MetadataToken, out objArray);
                if (objArray != null)
                {
                    foreach (object obj2 in objArray)
                    {
                        if ((caType == obj2.GetType()) || obj2.GetType().IsSubclassOf(caType))
                        {
                            list.Add((Attribute) obj2);
                        }
                    }
                }
            }
            count = list.Count;
            return list.ToArray();
        }

        internal static Attribute[] GetCustomAttributes(RuntimeMethodInfo method, Type caType, bool includeSecCa, out int count)
        {
            count = 0;
            bool flag = (caType == typeof(object)) || (caType == typeof(Attribute));
            if ((!flag && (s_pca[caType] == null)) && !IsSecurityAttribute(caType))
            {
                return new Attribute[0];
            }
            List<Attribute> list = new List<Attribute>();
            Attribute item = null;
            if (flag || (caType == typeof(DllImportAttribute)))
            {
                item = DllImportAttribute.GetCustomAttribute(method);
                if (item != null)
                {
                    list.Add(item);
                }
            }
            if (flag || (caType == typeof(PreserveSigAttribute)))
            {
                item = PreserveSigAttribute.GetCustomAttribute(method);
                if (item != null)
                {
                    list.Add(item);
                }
            }
            if (includeSecCa && (flag || IsSecurityAttribute(caType)))
            {
                object[] objArray;
                GetSecurityAttributes(method.Module.ModuleHandle, method.MetadataToken, out objArray);
                if (objArray != null)
                {
                    foreach (object obj2 in objArray)
                    {
                        if ((caType == obj2.GetType()) || obj2.GetType().IsSubclassOf(caType))
                        {
                            list.Add((Attribute) obj2);
                        }
                    }
                }
            }
            count = list.Count;
            return list.ToArray();
        }

        internal static Attribute[] GetCustomAttributes(RuntimeType type, Type caType, bool includeSecCa, out int count)
        {
            count = 0;
            bool flag = (caType == typeof(object)) || (caType == typeof(Attribute));
            if ((!flag && (s_pca[caType] == null)) && !IsSecurityAttribute(caType))
            {
                return new Attribute[0];
            }
            List<Attribute> list = new List<Attribute>();
            Attribute item = null;
            if (flag || (caType == typeof(SerializableAttribute)))
            {
                item = SerializableAttribute.GetCustomAttribute(type);
                if (item != null)
                {
                    list.Add(item);
                }
            }
            if (flag || (caType == typeof(ComImportAttribute)))
            {
                item = ComImportAttribute.GetCustomAttribute(type);
                if (item != null)
                {
                    list.Add(item);
                }
            }
            if ((includeSecCa && (flag || IsSecurityAttribute(caType))) && !type.IsGenericParameter)
            {
                object[] objArray;
                if (type.IsGenericType)
                {
                    type = (RuntimeType) type.GetGenericTypeDefinition();
                }
                GetSecurityAttributes(type.Module.ModuleHandle, type.MetadataToken, out objArray);
                if (objArray != null)
                {
                    foreach (object obj2 in objArray)
                    {
                        if ((caType == obj2.GetType()) || obj2.GetType().IsSubclassOf(caType))
                        {
                            list.Add((Attribute) obj2);
                        }
                    }
                }
            }
            count = list.Count;
            return list.ToArray();
        }

        internal static unsafe void GetSecurityAttributes(ModuleHandle module, int token, out object[] securityAttributes)
        {
            _GetSecurityAttributes(module.Value, token, out securityAttributes);
        }

        internal static bool IsDefined(Assembly assembly, Type caType)
        {
            int num;
            return (GetCustomAttributes(assembly, caType, out num).Length > 0);
        }

        internal static bool IsDefined(Module module, Type caType)
        {
            return false;
        }

        internal static bool IsDefined(ParameterInfo parameter, Type caType)
        {
            bool flag = (caType == typeof(object)) || (caType == typeof(Attribute));
            if (!flag && (s_pca[caType] == null))
            {
                return false;
            }
            return (((flag || (caType == typeof(InAttribute))) && InAttribute.IsDefined(parameter)) || (((flag || (caType == typeof(OutAttribute))) && OutAttribute.IsDefined(parameter)) || (((flag || (caType == typeof(OptionalAttribute))) && OptionalAttribute.IsDefined(parameter)) || ((flag || (caType == typeof(MarshalAsAttribute))) && MarshalAsAttribute.IsDefined(parameter)))));
        }

        internal static bool IsDefined(RuntimeConstructorInfo ctor, Type caType)
        {
            int num;
            bool flag = (caType == typeof(object)) || (caType == typeof(Attribute));
            if (!flag && (s_pca[caType] == null))
            {
                return false;
            }
            return ((flag || IsSecurityAttribute(caType)) && (GetCustomAttributes(ctor, caType, true, out num).Length != 0));
        }

        internal static bool IsDefined(RuntimeEventInfo e, Type caType)
        {
            return false;
        }

        internal static bool IsDefined(RuntimeFieldInfo field, Type caType)
        {
            bool flag = (caType == typeof(object)) || (caType == typeof(Attribute));
            if (!flag && (s_pca[caType] == null))
            {
                return false;
            }
            return (((flag || (caType == typeof(MarshalAsAttribute))) && MarshalAsAttribute.IsDefined(field)) || (((flag || (caType == typeof(FieldOffsetAttribute))) && FieldOffsetAttribute.IsDefined(field)) || ((flag || (caType == typeof(NonSerializedAttribute))) && NonSerializedAttribute.IsDefined(field))));
        }

        internal static bool IsDefined(RuntimeMethodInfo method, Type caType)
        {
            int num;
            bool flag = (caType == typeof(object)) || (caType == typeof(Attribute));
            if (!flag && (s_pca[caType] == null))
            {
                return false;
            }
            return (((flag || (caType == typeof(DllImportAttribute))) && DllImportAttribute.IsDefined(method)) || (((flag || (caType == typeof(PreserveSigAttribute))) && PreserveSigAttribute.IsDefined(method)) || ((flag || IsSecurityAttribute(caType)) && (GetCustomAttributes(method, caType, true, out num).Length != 0))));
        }

        internal static bool IsDefined(RuntimePropertyInfo property, Type caType)
        {
            return false;
        }

        internal static bool IsDefined(RuntimeType type, Type caType)
        {
            int num;
            bool flag = (caType == typeof(object)) || (caType == typeof(Attribute));
            if ((!flag && (s_pca[caType] == null)) && !IsSecurityAttribute(caType))
            {
                return false;
            }
            return (((flag || (caType == typeof(SerializableAttribute))) && SerializableAttribute.IsDefined(type)) || (((flag || (caType == typeof(ComImportAttribute))) && ComImportAttribute.IsDefined(type)) || ((flag || IsSecurityAttribute(caType)) && (GetCustomAttributes(type, caType, true, out num).Length != 0))));
        }

        internal static bool IsSecurityAttribute(Type type)
        {
            if (type != typeof(SecurityAttribute))
            {
                return type.IsSubclassOf(typeof(SecurityAttribute));
            }
            return true;
        }

        [Conditional("_DEBUG")]
        private static void VerifyPseudoCustomAttribute(Type pca)
        {
            CustomAttribute.GetAttributeUsage(pca as RuntimeType);
        }
    }
}

