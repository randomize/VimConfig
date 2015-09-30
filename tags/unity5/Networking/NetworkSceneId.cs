namespace UnityEngine.Networking
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct NetworkSceneId
    {
        [SerializeField]
        private uint m_Value;
        public NetworkSceneId(uint value)
        {
            this.m_Value = value;
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
            return ((obj is NetworkSceneId) && (this == ((NetworkSceneId) obj)));
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
        public static bool operator ==(NetworkSceneId c1, NetworkSceneId c2)
        {
            return (c1.m_Value == c2.m_Value);
        }

        public static bool operator !=(NetworkSceneId c1, NetworkSceneId c2)
        {
            return (c1.m_Value != c2.m_Value);
        }
    }
}

