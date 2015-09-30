namespace System
{
    using System.Collections;
    using System.Reflection;
    using System.Security.Permissions;

    internal class AppDomainInitializerInfo
    {
        internal ItemInfo[] Info = null;

        internal AppDomainInitializerInfo(AppDomainInitializer init)
        {
            if (init != null)
            {
                ArrayList list = new ArrayList();
                ArrayList list2 = new ArrayList();
                list2.Add(init);
                int num = 0;
                while (list2.Count > num)
                {
                    Delegate[] invocationList = ((AppDomainInitializer) list2[num++]).GetInvocationList();
                    for (int i = 0; i < invocationList.Length; i++)
                    {
                        if (!invocationList[i].Method.IsStatic)
                        {
                            if (invocationList[i].Target != null)
                            {
                                AppDomainInitializer target = invocationList[i].Target as AppDomainInitializer;
                                if (target == null)
                                {
                                    throw new ArgumentException(Environment.GetResourceString("Arg_MustBeStatic"), invocationList[i].Method.ReflectedType.FullName + "::" + invocationList[i].Method.Name);
                                }
                                list2.Add(target);
                            }
                        }
                        else
                        {
                            ItemInfo info = new ItemInfo {
                                TargetTypeAssembly = invocationList[i].Method.ReflectedType.Module.Assembly.FullName,
                                TargetTypeName = invocationList[i].Method.ReflectedType.FullName,
                                MethodName = invocationList[i].Method.Name
                            };
                            list.Add(info);
                        }
                    }
                }
                this.Info = (ItemInfo[]) list.ToArray(typeof(ItemInfo));
            }
        }

        internal AppDomainInitializer Unwrap()
        {
            if (this.Info == null)
            {
                return null;
            }
            AppDomainInitializer a = null;
            new ReflectionPermission(ReflectionPermissionFlag.MemberAccess).Assert();
            for (int i = 0; i < this.Info.Length; i++)
            {
                Assembly assembly = Assembly.Load(this.Info[i].TargetTypeAssembly);
                AppDomainInitializer b = (AppDomainInitializer) Delegate.CreateDelegate(typeof(AppDomainInitializer), assembly.GetType(this.Info[i].TargetTypeName), this.Info[i].MethodName);
                if (a == null)
                {
                    a = b;
                }
                else
                {
                    a = (AppDomainInitializer) Delegate.Combine(a, b);
                }
            }
            return a;
        }

        internal class ItemInfo
        {
            public string MethodName;
            public string TargetTypeAssembly;
            public string TargetTypeName;
        }
    }
}

