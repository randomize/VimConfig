namespace System.Runtime.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting.Contexts;
    using System.Runtime.Remoting.Messaging;
    using System.Runtime.Serialization.Formatters;
    using System.Security;
    using System.Security.Permissions;
    using System.Threading;

    [ComVisible(true)]
    public sealed class FormatterServices
    {
        private static readonly Type[] advancedTypes = new Type[] { typeof(ObjRef), typeof(DelegateSerializationHolder), typeof(IEnvoyInfo), typeof(ISponsor) };
        internal static Dictionary<MemberHolder, MemberInfo[]> m_MemberInfoTable = new Dictionary<MemberHolder, MemberInfo[]>(0x20);
        private static Binder s_binder = Type.DefaultBinder;
        private static object s_FormatterServicesSyncObject = null;

        private FormatterServices()
        {
            throw new NotSupportedException();
        }

        private static bool CheckSerializable(RuntimeType type)
        {
            return type.IsSerializable;
        }

        public static void CheckTypeSecurity(Type t, TypeFilterLevel securityLevel)
        {
            if (securityLevel == TypeFilterLevel.Low)
            {
                for (int i = 0; i < advancedTypes.Length; i++)
                {
                    if (advancedTypes[i].IsAssignableFrom(t))
                    {
                        throw new SecurityException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Serialization_TypeSecurity"), new object[] { advancedTypes[i].FullName, t.FullName }));
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), SecurityCritical]
        private static extern bool GetEnableUnsafeTypeForwarders();
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        public static object[] GetObjectData(object obj, MemberInfo[] members)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            if (members == null)
            {
                throw new ArgumentNullException("members");
            }
            int length = members.Length;
            object[] objArray = new object[length];
            for (int i = 0; i < length; i++)
            {
                MemberInfo info = members[i];
                if (info == null)
                {
                    throw new ArgumentNullException("members", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("ArgumentNull_NullMember"), new object[] { i }));
                }
                if (info.MemberType != MemberTypes.Field)
                {
                    throw new SerializationException(Environment.GetResourceString("Serialization_UnknownMemberInfo"));
                }
                RtFieldInfo info2 = info as RtFieldInfo;
                if (info2 != null)
                {
                    objArray[i] = info2.InternalGetValue(obj, false);
                }
                else
                {
                    objArray[i] = ((SerializationFieldInfo) info).InternalGetValue(obj, false);
                }
            }
            return objArray;
        }

        private static bool GetParentTypes(Type parentType, out Type[] parentTypes, out int parentTypeCount)
        {
            parentTypes = null;
            parentTypeCount = 0;
            bool flag = true;
            for (Type type = parentType; type != typeof(object); type = type.BaseType)
            {
                if (type.IsInterface)
                {
                    continue;
                }
                string name = type.Name;
                for (int i = 0; flag && (i < parentTypeCount); i++)
                {
                    string str2 = parentTypes[i].Name;
                    if (((str2.Length == name.Length) && (str2[0] == name[0])) && (name == str2))
                    {
                        flag = false;
                        break;
                    }
                }
                if ((parentTypes == null) || (parentTypeCount == parentTypes.Length))
                {
                    Type[] destinationArray = new Type[Math.Max(parentTypeCount * 2, 12)];
                    if (parentTypes != null)
                    {
                        Array.Copy(parentTypes, 0, destinationArray, 0, parentTypeCount);
                    }
                    parentTypes = destinationArray;
                }
                parentTypes[parentTypeCount++] = type;
            }
            return flag;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        public static object GetSafeUninitializedObject(Type type)
        {
            object obj2;
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (!(type is RuntimeType))
            {
                throw new SerializationException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Serialization_InvalidType"), new object[] { type.ToString() }));
            }
            if (((type == typeof(ConstructionCall)) || (type == typeof(LogicalCallContext))) || (type == typeof(SynchronizationAttribute)))
            {
                return nativeGetUninitializedObject((RuntimeType) type);
            }
            try
            {
                obj2 = nativeGetSafeUninitializedObject((RuntimeType) type);
            }
            catch (SecurityException exception)
            {
                throw new SerializationException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Serialization_Security"), new object[] { type.FullName }), exception);
            }
            return obj2;
        }

        private static MemberInfo[] GetSerializableMembers(RuntimeType type)
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            int index = 0;
            for (int i = 0; i < fields.Length; i++)
            {
                if ((fields[i].Attributes & FieldAttributes.NotSerialized) != FieldAttributes.NotSerialized)
                {
                    index++;
                }
            }
            if (index == fields.Length)
            {
                return fields;
            }
            FieldInfo[] infoArray2 = new FieldInfo[index];
            index = 0;
            for (int j = 0; j < fields.Length; j++)
            {
                if ((fields[j].Attributes & FieldAttributes.NotSerialized) != FieldAttributes.NotSerialized)
                {
                    infoArray2[index] = fields[j];
                    index++;
                }
            }
            return infoArray2;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        public static MemberInfo[] GetSerializableMembers(Type type)
        {
            return GetSerializableMembers(type, new StreamingContext(StreamingContextStates.All));
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        public static MemberInfo[] GetSerializableMembers(Type type, StreamingContext context)
        {
            MemberInfo[] serializableMembers;
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (!(type is RuntimeType))
            {
                throw new SerializationException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Serialization_InvalidType"), new object[] { type.ToString() }));
            }
            MemberHolder key = new MemberHolder(type, context);
            if (m_MemberInfoTable.ContainsKey(key))
            {
                return m_MemberInfoTable[key];
            }
            lock (formatterServicesSyncObject)
            {
                if (m_MemberInfoTable.ContainsKey(key))
                {
                    return m_MemberInfoTable[key];
                }
                serializableMembers = InternalGetSerializableMembers((RuntimeType) type);
                m_MemberInfoTable[key] = serializableMembers;
            }
            return serializableMembers;
        }

        public static ISerializationSurrogate GetSurrogateForCyclicalReference(ISerializationSurrogate innerSurrogate)
        {
            if (innerSurrogate == null)
            {
                throw new ArgumentNullException("innerSurrogate");
            }
            return new SurrogateForCyclicalReference(innerSurrogate);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        public static Type GetTypeFromAssembly(Assembly assem, string name)
        {
            if (assem == null)
            {
                throw new ArgumentNullException("assem");
            }
            return assem.GetType(name, false, false);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        public static object GetUninitializedObject(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (!(type is RuntimeType))
            {
                throw new SerializationException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Serialization_InvalidType"), new object[] { type.ToString() }));
            }
            return nativeGetUninitializedObject((RuntimeType) type);
        }

        private static MemberInfo[] InternalGetSerializableMembers(RuntimeType type)
        {
            ArrayList list = null;
            if (type.IsInterface)
            {
                return new MemberInfo[0];
            }
            if (!CheckSerializable(type))
            {
                throw new SerializationException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Serialization_NonSerType"), new object[] { type.FullName, type.Module.Assembly.FullName }));
            }
            MemberInfo[] serializableMembers = GetSerializableMembers(type);
            RuntimeType baseType = (RuntimeType) type.BaseType;
            if ((baseType != null) && (baseType != typeof(object)))
            {
                Type[] parentTypes = null;
                int parentTypeCount = 0;
                bool flag = GetParentTypes(baseType, out parentTypes, out parentTypeCount);
                if (parentTypeCount <= 0)
                {
                    return serializableMembers;
                }
                list = new ArrayList();
                for (int i = 0; i < parentTypeCount; i++)
                {
                    baseType = (RuntimeType) parentTypes[i];
                    if (!CheckSerializable(baseType))
                    {
                        throw new SerializationException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Serialization_NonSerType"), new object[] { baseType.FullName, baseType.Module.Assembly.FullName }));
                    }
                    FieldInfo[] fields = baseType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                    string namePrefix = flag ? baseType.Name : baseType.FullName;
                    foreach (FieldInfo info in fields)
                    {
                        if (!info.IsNotSerialized)
                        {
                            list.Add(new SerializationFieldInfo((RuntimeFieldInfo) info, namePrefix));
                        }
                    }
                }
                if ((list != null) && (list.Count > 0))
                {
                    MemberInfo[] destinationArray = new MemberInfo[list.Count + serializableMembers.Length];
                    Array.Copy(serializableMembers, destinationArray, serializableMembers.Length);
                    list.CopyTo(destinationArray, serializableMembers.Length);
                    serializableMembers = destinationArray;
                }
            }
            return serializableMembers;
        }

        internal static Assembly LoadAssemblyFromString(string assemblyName)
        {
            return Assembly.Load(assemblyName);
        }

        internal static Assembly LoadAssemblyFromStringNoThrow(string assemblyName)
        {
            try
            {
                return LoadAssemblyFromString(assemblyName);
            }
            catch (Exception)
            {
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern object nativeGetSafeUninitializedObject(RuntimeType type);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern object nativeGetUninitializedObject(RuntimeType type);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        public static object PopulateObjectMembers(object obj, MemberInfo[] members, object[] data)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            if (members == null)
            {
                throw new ArgumentNullException("members");
            }
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (members.Length != data.Length)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_DataLengthDifferent"));
            }
            for (int i = 0; i < members.Length; i++)
            {
                MemberInfo fi = members[i];
                if (fi == null)
                {
                    throw new ArgumentNullException("members", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("ArgumentNull_NullMember"), new object[] { i }));
                }
                if (data[i] != null)
                {
                    if (fi.MemberType != MemberTypes.Field)
                    {
                        throw new SerializationException(Environment.GetResourceString("Serialization_UnknownMemberInfo"));
                    }
                    SerializationSetValue(fi, obj, data[i]);
                }
            }
            return obj;
        }

        internal static void SerializationSetValue(MemberInfo fi, object target, object value)
        {
            RtFieldInfo info = fi as RtFieldInfo;
            if (info != null)
            {
                info.InternalSetValue(target, value, BindingFlags.Default, s_binder, null, false);
            }
            else
            {
                ((SerializationFieldInfo) fi).InternalSetValue(target, value, BindingFlags.Default, s_binder, null, false, true);
            }
        }

        [SecuritySafeCritical]
        internal static bool UnsafeTypeForwardersIsEnabled()
        {
            return GetEnableUnsafeTypeForwarders();
        }

        private static object formatterServicesSyncObject
        {
            get
            {
                if (s_FormatterServicesSyncObject == null)
                {
                    object obj2 = new object();
                    Interlocked.CompareExchange(ref s_FormatterServicesSyncObject, obj2, null);
                }
                return s_FormatterServicesSyncObject;
            }
        }
    }
}

