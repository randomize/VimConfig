namespace System.Reflection
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;

    internal static class Associates
    {
        internal static RuntimeMethodInfo AssignAssociates(int tkMethod, RuntimeTypeHandle declaredTypeHandle, RuntimeTypeHandle reflectedTypeHandle)
        {
            if (MetadataToken.IsNullToken(tkMethod))
            {
                return null;
            }
            bool flag = !declaredTypeHandle.Equals(reflectedTypeHandle);
            RuntimeMethodHandle methodHandle = declaredTypeHandle.GetModuleHandle().ResolveMethodHandle(tkMethod, declaredTypeHandle.GetInstantiation(), new RuntimeTypeHandle[0]);
            MethodAttributes attributes = methodHandle.GetAttributes();
            bool flag2 = (attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Private;
            bool flag3 = (attributes & MethodAttributes.Virtual) != MethodAttributes.PrivateScope;
            if (flag)
            {
                if (flag2)
                {
                    return null;
                }
                if (flag3 && ((declaredTypeHandle.GetAttributes() & TypeAttributes.ClassSemanticsMask) == TypeAttributes.AnsiClass))
                {
                    int slot = methodHandle.GetSlot();
                    methodHandle = reflectedTypeHandle.GetMethodAt(slot);
                }
            }
            MethodAttributes attributes2 = attributes & MethodAttributes.MemberAccessMask;
            RuntimeMethodInfo methodBase = RuntimeType.GetMethodBase(reflectedTypeHandle, methodHandle) as RuntimeMethodInfo;
            if (methodBase == null)
            {
                methodBase = reflectedTypeHandle.GetRuntimeType().Module.ResolveMethod(tkMethod, null, null) as RuntimeMethodInfo;
            }
            return methodBase;
        }

        internal static unsafe void AssignAssociates(AssociateRecord* associates, int cAssociates, RuntimeTypeHandle declaringTypeHandle, RuntimeTypeHandle reflectedTypeHandle, out RuntimeMethodInfo addOn, out RuntimeMethodInfo removeOn, out RuntimeMethodInfo fireOn, out RuntimeMethodInfo getter, out RuntimeMethodInfo setter, out MethodInfo[] other, out bool composedOfAllPrivateMethods, out BindingFlags bindingFlags)
        {
            RuntimeMethodInfo info2;
            RuntimeMethodInfo info3;
            RuntimeMethodInfo info4;
            setter = (RuntimeMethodInfo) (info2 = null);
            getter = info3 = info2;
            fireOn = info4 = info3;
            addOn = removeOn = info4;
            other = null;
            Attributes attributes = Attributes.ComposedOfNoStaticMembers | Attributes.ComposedOfNoPublicMembers | Attributes.ComposedOfAllPrivateMethods | Attributes.ComposedOfAllVirtualMethods;
            while (reflectedTypeHandle.IsGenericVariable())
            {
                reflectedTypeHandle = reflectedTypeHandle.GetRuntimeType().BaseType.GetTypeHandleInternal();
            }
            bool isInherited = !declaringTypeHandle.Equals(reflectedTypeHandle);
            ArrayList list = new ArrayList();
            for (int i = 0; i < cAssociates; i++)
            {
                RuntimeMethodInfo info = AssignAssociates(associates[i].MethodDefToken, declaringTypeHandle, reflectedTypeHandle);
                if (info != null)
                {
                    MethodAttributes attributes2 = info.Attributes;
                    bool flag2 = (attributes2 & MethodAttributes.MemberAccessMask) == MethodAttributes.Private;
                    bool flag3 = (attributes2 & MethodAttributes.Virtual) != MethodAttributes.PrivateScope;
                    MethodAttributes attributes3 = attributes2 & MethodAttributes.MemberAccessMask;
                    bool flag4 = attributes3 == MethodAttributes.Public;
                    bool flag5 = (attributes2 & MethodAttributes.Static) != MethodAttributes.PrivateScope;
                    if (flag4)
                    {
                        attributes &= ~Attributes.ComposedOfNoPublicMembers;
                        attributes &= ~Attributes.ComposedOfAllPrivateMethods;
                    }
                    else if (!flag2)
                    {
                        attributes &= ~Attributes.ComposedOfAllPrivateMethods;
                    }
                    if (flag5)
                    {
                        attributes &= ~Attributes.ComposedOfNoStaticMembers;
                    }
                    if (!flag3)
                    {
                        attributes &= ~Attributes.ComposedOfAllVirtualMethods;
                    }
                    if (associates[i].Semantics == MethodSemanticsAttributes.Setter)
                    {
                        setter = info;
                    }
                    else if (associates[i].Semantics == MethodSemanticsAttributes.Getter)
                    {
                        getter = info;
                    }
                    else if (associates[i].Semantics == MethodSemanticsAttributes.Fire)
                    {
                        fireOn = info;
                    }
                    else if (associates[i].Semantics == MethodSemanticsAttributes.AddOn)
                    {
                        addOn = info;
                    }
                    else if (associates[i].Semantics == MethodSemanticsAttributes.RemoveOn)
                    {
                        removeOn = info;
                    }
                    else
                    {
                        list.Add(info);
                    }
                }
            }
            bool isPublic = (attributes & Attributes.ComposedOfNoPublicMembers) == 0;
            bool isStatic = (attributes & Attributes.ComposedOfNoStaticMembers) == 0;
            bindingFlags = RuntimeType.FilterPreCalculate(isPublic, isInherited, isStatic);
            composedOfAllPrivateMethods = (attributes & Attributes.ComposedOfAllPrivateMethods) != 0;
            other = (MethodInfo[]) list.ToArray(typeof(MethodInfo));
        }

        internal static bool IncludeAccessor(MethodInfo associate, bool nonPublic)
        {
            if (associate == null)
            {
                return false;
            }
            return (nonPublic || associate.IsPublic);
        }

        [Flags]
        internal enum Attributes
        {
            ComposedOfAllPrivateMethods = 2,
            ComposedOfAllVirtualMethods = 1,
            ComposedOfNoPublicMembers = 4,
            ComposedOfNoStaticMembers = 8
        }
    }
}

