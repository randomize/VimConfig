namespace UnityEngine
{
    using System;

    [NotConverted, AttributeUsage(AttributeTargets.Interface | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Struct | AttributeTargets.Class), Obsolete("NotRenamedAttribute was used for the Flash buildtarget, which is not supported anymore starting Unity 5.0", true)]
    public sealed class NotRenamedAttribute : Attribute
    {
    }
}

