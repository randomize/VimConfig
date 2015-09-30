namespace System
{
    using System.Collections;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.InteropServices;

    [Serializable, AttributeUsage(AttributeTargets.All, Inherited=true, AllowMultiple=false), ComVisible(true), ClassInterface(ClassInterfaceType.None), ComDefaultInterface(typeof(_Attribute))]
    public abstract class Attribute : _Attribute
    {
        protected Attribute()
        {
        }

        private static void AddAttributesToList(ArrayList attributeList, Attribute[] attributes, Hashtable types)
        {
            for (int i = 0; i < attributes.Length; i++)
            {
                Type type = attributes[i].GetType();
                AttributeUsageAttribute attributeUsage = (AttributeUsageAttribute) types[type];
                if (attributeUsage == null)
                {
                    attributeUsage = InternalGetAttributeUsage(type);
                    types[type] = attributeUsage;
                    if (attributeUsage.Inherited)
                    {
                        attributeList.Add(attributes[i]);
                    }
                }
                else if (attributeUsage.Inherited && attributeUsage.AllowMultiple)
                {
                    attributeList.Add(attributes[i]);
                }
            }
        }

        private static void CopyToArrayList(ArrayList attributeList, Attribute[] attributes, Hashtable types)
        {
            for (int i = 0; i < attributes.Length; i++)
            {
                attributeList.Add(attributes[i]);
                Type key = attributes[i].GetType();
                if (!types.Contains(key))
                {
                    types[key] = InternalGetAttributeUsage(key);
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            RuntimeType type = (RuntimeType) base.GetType();
            RuntimeType type2 = (RuntimeType) obj.GetType();
            if (type2 != type)
            {
                return false;
            }
            object obj2 = this;
            FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < fields.Length; i++)
            {
                object obj3 = ((RuntimeFieldInfo) fields[i]).GetValue(obj2);
                object obj4 = ((RuntimeFieldInfo) fields[i]).GetValue(obj);
                if (obj3 == null)
                {
                    if (obj4 != null)
                    {
                        return false;
                    }
                }
                else if (!obj3.Equals(obj4))
                {
                    return false;
                }
            }
            return true;
        }

        public static Attribute GetCustomAttribute(Assembly element, Type attributeType)
        {
            return GetCustomAttribute(element, attributeType, true);
        }

        public static Attribute GetCustomAttribute(MemberInfo element, Type attributeType)
        {
            return GetCustomAttribute(element, attributeType, true);
        }

        public static Attribute GetCustomAttribute(Module element, Type attributeType)
        {
            return GetCustomAttribute(element, attributeType, true);
        }

        public static Attribute GetCustomAttribute(ParameterInfo element, Type attributeType)
        {
            return GetCustomAttribute(element, attributeType, true);
        }

        public static Attribute GetCustomAttribute(Assembly element, Type attributeType, bool inherit)
        {
            Attribute[] attributeArray = GetCustomAttributes(element, attributeType, inherit);
            if ((attributeArray == null) || (attributeArray.Length == 0))
            {
                return null;
            }
            if (attributeArray.Length != 1)
            {
                throw new AmbiguousMatchException(Environment.GetResourceString("RFLCT.AmbigCust"));
            }
            return attributeArray[0];
        }

        public static Attribute GetCustomAttribute(MemberInfo element, Type attributeType, bool inherit)
        {
            Attribute[] attributeArray = GetCustomAttributes(element, attributeType, inherit);
            if ((attributeArray == null) || (attributeArray.Length == 0))
            {
                return null;
            }
            if (attributeArray.Length != 1)
            {
                throw new AmbiguousMatchException(Environment.GetResourceString("RFLCT.AmbigCust"));
            }
            return attributeArray[0];
        }

        public static Attribute GetCustomAttribute(Module element, Type attributeType, bool inherit)
        {
            Attribute[] attributeArray = GetCustomAttributes(element, attributeType, inherit);
            if ((attributeArray == null) || (attributeArray.Length == 0))
            {
                return null;
            }
            if (attributeArray.Length != 1)
            {
                throw new AmbiguousMatchException(Environment.GetResourceString("RFLCT.AmbigCust"));
            }
            return attributeArray[0];
        }

        public static Attribute GetCustomAttribute(ParameterInfo element, Type attributeType, bool inherit)
        {
            Attribute[] attributeArray = GetCustomAttributes(element, attributeType, inherit);
            if ((attributeArray == null) || (attributeArray.Length == 0))
            {
                return null;
            }
            if (attributeArray.Length == 0)
            {
                return null;
            }
            if (attributeArray.Length != 1)
            {
                throw new AmbiguousMatchException(Environment.GetResourceString("RFLCT.AmbigCust"));
            }
            return attributeArray[0];
        }

        public static Attribute[] GetCustomAttributes(Assembly element)
        {
            return GetCustomAttributes(element, true);
        }

        public static Attribute[] GetCustomAttributes(MemberInfo element)
        {
            return GetCustomAttributes(element, true);
        }

        public static Attribute[] GetCustomAttributes(Module element)
        {
            return GetCustomAttributes(element, true);
        }

        public static Attribute[] GetCustomAttributes(ParameterInfo element)
        {
            return GetCustomAttributes(element, true);
        }

        public static Attribute[] GetCustomAttributes(Assembly element, bool inherit)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (Attribute[]) element.GetCustomAttributes(typeof(Attribute), inherit);
        }

        public static Attribute[] GetCustomAttributes(Assembly element, Type attributeType)
        {
            return GetCustomAttributes(element, attributeType, true);
        }

        public static Attribute[] GetCustomAttributes(MemberInfo element, bool inherit)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            MemberTypes memberType = element.MemberType;
            if (memberType != MemberTypes.Event)
            {
                if (memberType == MemberTypes.Property)
                {
                    return InternalGetCustomAttributes((PropertyInfo) element, typeof(Attribute), inherit);
                }
                return (element.GetCustomAttributes(typeof(Attribute), inherit) as Attribute[]);
            }
            return InternalGetCustomAttributes((EventInfo) element, typeof(Attribute), inherit);
        }

        public static Attribute[] GetCustomAttributes(MemberInfo element, Type type)
        {
            return GetCustomAttributes(element, type, true);
        }

        public static Attribute[] GetCustomAttributes(Module element, bool inherit)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (Attribute[]) element.GetCustomAttributes(typeof(Attribute), inherit);
        }

        public static Attribute[] GetCustomAttributes(Module element, Type attributeType)
        {
            return GetCustomAttributes(element, attributeType, true);
        }

        public static Attribute[] GetCustomAttributes(ParameterInfo element, bool inherit)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            MemberInfo member = element.Member;
            if ((member.MemberType == MemberTypes.Method) && inherit)
            {
                return InternalParamGetCustomAttributes((MethodInfo) member, element, null, inherit);
            }
            return (element.GetCustomAttributes(typeof(Attribute), inherit) as Attribute[]);
        }

        public static Attribute[] GetCustomAttributes(ParameterInfo element, Type attributeType)
        {
            return GetCustomAttributes(element, attributeType, true);
        }

        public static Attribute[] GetCustomAttributes(Assembly element, Type attributeType, bool inherit)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            if (attributeType == null)
            {
                throw new ArgumentNullException("attributeType");
            }
            if (!attributeType.IsSubclassOf(typeof(Attribute)) && (attributeType != typeof(Attribute)))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_MustHaveAttributeBaseClass"));
            }
            return (Attribute[]) element.GetCustomAttributes(attributeType, inherit);
        }

        public static Attribute[] GetCustomAttributes(MemberInfo element, Type type, bool inherit)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (!type.IsSubclassOf(typeof(Attribute)) && (type != typeof(Attribute)))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_MustHaveAttributeBaseClass"));
            }
            MemberTypes memberType = element.MemberType;
            if (memberType != MemberTypes.Event)
            {
                if (memberType == MemberTypes.Property)
                {
                    return InternalGetCustomAttributes((PropertyInfo) element, type, inherit);
                }
                return (element.GetCustomAttributes(type, inherit) as Attribute[]);
            }
            return InternalGetCustomAttributes((EventInfo) element, type, inherit);
        }

        public static Attribute[] GetCustomAttributes(Module element, Type attributeType, bool inherit)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            if (attributeType == null)
            {
                throw new ArgumentNullException("attributeType");
            }
            if (!attributeType.IsSubclassOf(typeof(Attribute)) && (attributeType != typeof(Attribute)))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_MustHaveAttributeBaseClass"));
            }
            return (Attribute[]) element.GetCustomAttributes(attributeType, inherit);
        }

        public static Attribute[] GetCustomAttributes(ParameterInfo element, Type attributeType, bool inherit)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            if (attributeType == null)
            {
                throw new ArgumentNullException("attributeType");
            }
            if (!attributeType.IsSubclassOf(typeof(Attribute)) && (attributeType != typeof(Attribute)))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_MustHaveAttributeBaseClass"));
            }
            MemberInfo member = element.Member;
            if ((member.MemberType == MemberTypes.Method) && inherit)
            {
                return InternalParamGetCustomAttributes((MethodInfo) member, element, attributeType, inherit);
            }
            return (element.GetCustomAttributes(attributeType, inherit) as Attribute[]);
        }

        public override int GetHashCode()
        {
            Type type = base.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            object obj2 = null;
            for (int i = 0; i < fields.Length; i++)
            {
                obj2 = fields[i].GetValue(this);
                if (obj2 != null)
                {
                    break;
                }
            }
            if (obj2 != null)
            {
                return obj2.GetHashCode();
            }
            return type.GetHashCode();
        }

        private static EventInfo GetParentDefinition(EventInfo ev)
        {
            MethodInfo addMethod = ev.GetAddMethod(true);
            if (addMethod != null)
            {
                addMethod = addMethod.GetParentDefinition();
                if (addMethod != null)
                {
                    return addMethod.DeclaringType.GetEvent(ev.Name);
                }
            }
            return null;
        }

        private static PropertyInfo GetParentDefinition(PropertyInfo property)
        {
            MethodInfo getMethod = property.GetGetMethod(true);
            if (getMethod == null)
            {
                getMethod = property.GetSetMethod(true);
            }
            if (getMethod != null)
            {
                getMethod = getMethod.GetParentDefinition();
                if (getMethod != null)
                {
                    return getMethod.DeclaringType.GetProperty(property.Name, property.PropertyType);
                }
            }
            return null;
        }

        private static AttributeUsageAttribute InternalGetAttributeUsage(Type type)
        {
            object[] customAttributes = type.GetCustomAttributes(typeof(AttributeUsageAttribute), false);
            if (customAttributes.Length == 1)
            {
                return (AttributeUsageAttribute) customAttributes[0];
            }
            if (customAttributes.Length != 0)
            {
                throw new FormatException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Format_AttributeUsage"), new object[] { type }));
            }
            return AttributeUsageAttribute.Default;
        }

        private static Attribute[] InternalGetCustomAttributes(EventInfo element, Type type, bool inherit)
        {
            Attribute[] customAttributes = (Attribute[]) element.GetCustomAttributes(type, inherit);
            if (!inherit)
            {
                return customAttributes;
            }
            Hashtable types = new Hashtable(11);
            ArrayList attributeList = new ArrayList();
            CopyToArrayList(attributeList, customAttributes, types);
            for (EventInfo info = GetParentDefinition(element); info != null; info = GetParentDefinition(info))
            {
                customAttributes = GetCustomAttributes(info, type, false);
                AddAttributesToList(attributeList, customAttributes, types);
            }
            return (Attribute[]) attributeList.ToArray(type);
        }

        private static Attribute[] InternalGetCustomAttributes(PropertyInfo element, Type type, bool inherit)
        {
            Attribute[] customAttributes = (Attribute[]) element.GetCustomAttributes(type, inherit);
            if (!inherit)
            {
                return customAttributes;
            }
            Hashtable types = new Hashtable(11);
            ArrayList attributeList = new ArrayList();
            CopyToArrayList(attributeList, customAttributes, types);
            for (PropertyInfo info = GetParentDefinition(element); info != null; info = GetParentDefinition(info))
            {
                customAttributes = GetCustomAttributes(info, type, false);
                AddAttributesToList(attributeList, customAttributes, types);
            }
            return (Attribute[]) attributeList.ToArray(type);
        }

        private static bool InternalIsDefined(EventInfo element, Type attributeType, bool inherit)
        {
            if (element.IsDefined(attributeType, inherit))
            {
                return true;
            }
            if (inherit)
            {
                if (!InternalGetAttributeUsage(attributeType).Inherited)
                {
                    return false;
                }
                for (EventInfo info = GetParentDefinition(element); info != null; info = GetParentDefinition(info))
                {
                    if (info.IsDefined(attributeType, false))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool InternalIsDefined(PropertyInfo element, Type attributeType, bool inherit)
        {
            if (element.IsDefined(attributeType, inherit))
            {
                return true;
            }
            if (inherit)
            {
                if (!InternalGetAttributeUsage(attributeType).Inherited)
                {
                    return false;
                }
                for (PropertyInfo info = GetParentDefinition(element); info != null; info = GetParentDefinition(info))
                {
                    if (info.IsDefined(attributeType, false))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static Attribute[] InternalParamGetCustomAttributes(MethodInfo method, ParameterInfo param, Type type, bool inherit)
        {
            ArrayList list = new ArrayList();
            if (type == null)
            {
                type = typeof(Attribute);
            }
            object[] customAttributes = param.GetCustomAttributes(type, false);
            for (int i = 0; i < customAttributes.Length; i++)
            {
                Type type2 = customAttributes[i].GetType();
                if (!InternalGetAttributeUsage(type2).AllowMultiple)
                {
                    list.Add(type2);
                }
            }
            Attribute[] destinationArray = null;
            if (customAttributes.Length == 0)
            {
                destinationArray = (Attribute[]) Array.CreateInstance(type, 0);
            }
            else
            {
                destinationArray = (Attribute[]) customAttributes;
            }
            if (method.DeclaringType != null)
            {
                if (!inherit)
                {
                    return destinationArray;
                }
                int position = param.Position;
                method = method.GetParentDefinition();
                while (method != null)
                {
                    param = method.GetParameters()[position];
                    customAttributes = param.GetCustomAttributes(type, false);
                    int length = 0;
                    for (int j = 0; j < customAttributes.Length; j++)
                    {
                        Type type3 = customAttributes[j].GetType();
                        AttributeUsageAttribute attributeUsage = InternalGetAttributeUsage(type3);
                        if (attributeUsage.Inherited && !list.Contains(type3))
                        {
                            if (!attributeUsage.AllowMultiple)
                            {
                                list.Add(type3);
                            }
                            length++;
                        }
                        else
                        {
                            customAttributes[j] = null;
                        }
                    }
                    Attribute[] attributeArray2 = (Attribute[]) Array.CreateInstance(type, length);
                    length = 0;
                    for (int k = 0; k < customAttributes.Length; k++)
                    {
                        if (customAttributes[k] != null)
                        {
                            attributeArray2[length] = (Attribute) customAttributes[k];
                            length++;
                        }
                    }
                    Attribute[] sourceArray = destinationArray;
                    destinationArray = (Attribute[]) Array.CreateInstance(type, (int) (sourceArray.Length + length));
                    Array.Copy(sourceArray, destinationArray, sourceArray.Length);
                    int num6 = sourceArray.Length;
                    for (int m = 0; m < attributeArray2.Length; m++)
                    {
                        destinationArray[num6 + m] = attributeArray2[m];
                    }
                    method = method.GetParentDefinition();
                }
            }
            return destinationArray;
        }

        private static bool InternalParamIsDefined(MethodInfo method, ParameterInfo param, Type type, bool inherit)
        {
            if (param.IsDefined(type, false))
            {
                return true;
            }
            if ((method.DeclaringType != null) && inherit)
            {
                int position = param.Position;
                method = method.GetParentDefinition();
                while (method != null)
                {
                    param = method.GetParameters()[position];
                    object[] customAttributes = param.GetCustomAttributes(type, false);
                    for (int i = 0; i < customAttributes.Length; i++)
                    {
                        AttributeUsageAttribute attributeUsage = InternalGetAttributeUsage(customAttributes[i].GetType());
                        if ((customAttributes[i] is Attribute) && attributeUsage.Inherited)
                        {
                            return true;
                        }
                    }
                    method = method.GetParentDefinition();
                }
            }
            return false;
        }

        public virtual bool IsDefaultAttribute()
        {
            return false;
        }

        public static bool IsDefined(Assembly element, Type attributeType)
        {
            return IsDefined(element, attributeType, true);
        }

        public static bool IsDefined(MemberInfo element, Type attributeType)
        {
            return IsDefined(element, attributeType, true);
        }

        public static bool IsDefined(Module element, Type attributeType)
        {
            return IsDefined(element, attributeType, false);
        }

        public static bool IsDefined(ParameterInfo element, Type attributeType)
        {
            return IsDefined(element, attributeType, true);
        }

        public static bool IsDefined(Assembly element, Type attributeType, bool inherit)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            if (attributeType == null)
            {
                throw new ArgumentNullException("attributeType");
            }
            if (!attributeType.IsSubclassOf(typeof(Attribute)) && (attributeType != typeof(Attribute)))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_MustHaveAttributeBaseClass"));
            }
            return element.IsDefined(attributeType, false);
        }

        public static bool IsDefined(MemberInfo element, Type attributeType, bool inherit)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            if (attributeType == null)
            {
                throw new ArgumentNullException("attributeType");
            }
            if (!attributeType.IsSubclassOf(typeof(Attribute)) && (attributeType != typeof(Attribute)))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_MustHaveAttributeBaseClass"));
            }
            MemberTypes memberType = element.MemberType;
            if (memberType != MemberTypes.Event)
            {
                if (memberType == MemberTypes.Property)
                {
                    return InternalIsDefined((PropertyInfo) element, attributeType, inherit);
                }
                return element.IsDefined(attributeType, inherit);
            }
            return InternalIsDefined((EventInfo) element, attributeType, inherit);
        }

        public static bool IsDefined(Module element, Type attributeType, bool inherit)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            if (attributeType == null)
            {
                throw new ArgumentNullException("attributeType");
            }
            if (!attributeType.IsSubclassOf(typeof(Attribute)) && (attributeType != typeof(Attribute)))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_MustHaveAttributeBaseClass"));
            }
            return element.IsDefined(attributeType, false);
        }

        public static bool IsDefined(ParameterInfo element, Type attributeType, bool inherit)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            if (attributeType == null)
            {
                throw new ArgumentNullException("attributeType");
            }
            if (!attributeType.IsSubclassOf(typeof(Attribute)) && (attributeType != typeof(Attribute)))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_MustHaveAttributeBaseClass"));
            }
            MemberInfo member = element.Member;
            MemberTypes memberType = member.MemberType;
            if (memberType == MemberTypes.Constructor)
            {
                return element.IsDefined(attributeType, false);
            }
            if (memberType != MemberTypes.Method)
            {
                if (memberType != MemberTypes.Property)
                {
                    throw new ArgumentException(Environment.GetResourceString("Argument_InvalidParamInfo"));
                }
                return element.IsDefined(attributeType, false);
            }
            return InternalParamIsDefined((MethodInfo) member, element, attributeType, inherit);
        }

        public virtual bool Match(object obj)
        {
            return this.Equals(obj);
        }

        void _Attribute.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
        {
            throw new NotImplementedException();
        }

        void _Attribute.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
        {
            throw new NotImplementedException();
        }

        void _Attribute.GetTypeInfoCount(out uint pcTInfo)
        {
            throw new NotImplementedException();
        }

        void _Attribute.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
        {
            throw new NotImplementedException();
        }

        public virtual object TypeId
        {
            get
            {
                return base.GetType();
            }
        }
    }
}

