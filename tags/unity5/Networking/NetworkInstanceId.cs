namespace UnityEngine.Networking
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct NetworkInstanceId
    {
        [SerializeField]
        private readonly uint m_Value;
        public static NetworkInstanceId Invalid;
        internal static NetworkInstanceId Zero;
        public NetworkInstanceId(uint value)
        {
            this.m_Value = value;
        }

        static NetworkInstanceId()
        {
            Invalid = new NetworkInstanceId(uint.MaxValue);
            Zero = new NetworkInstanceId(0);
        }

        public bool IsEmpty()
        {
            return (this.m_Value == 0);
        }

        public override int GetHashCode()
        {
            return (int) this.m_Value;
        }

        public override bool Equals(object obj)
        {
            return ((obj is NetworkInstanceId) && (this == ((NetworkInstanceId) obj)));
        }

        public override string ToString()
        {
            return this.m_Value.ToString();
        }

        public uint Value
        {
            get
            {
                return this.m_Value;
            }
        }
        public static bool operator ==(NetworkInstanceId c1, NetworkInstanceId c2)
        {
            return (c1.m_Value == c2.m_Value);
        }

        public static bool operator !=(NetworkInstanceId c1, NetworkInstanceId c2)
        {
            return (c1.m_Value != c2.m_Value);
        }
    }
}

