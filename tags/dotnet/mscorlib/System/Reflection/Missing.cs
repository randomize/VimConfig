namespace System.Reflection
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    [Serializable, ComVisible(true)]
    public sealed class Missing : ISerializable
    {
        public static readonly Missing Value = new Missing();

        private Missing()
        {
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            UnitySerializationHolder.GetUnitySerializationInfo(info, this);
        }
    }
}

