namespace UnityEngine
{
    using System;

    [Obsolete("NotFlashValidatedAttribute was used for the Flash buildtarget, which is not supported anymore starting Unity 5.0", true), NotConverted, AttributeUsage(AttributeTargets.Interface | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Struct | AttributeTargets.Class)]
    public sealed class NotFlashValidatedAttribute : Attribute
    {
    }
}

