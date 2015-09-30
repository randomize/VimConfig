namespace UnityEngine.UI
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Serialization;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct Navigation
    {
        [FormerlySerializedAs("mode"), SerializeField]
        private Mode m_Mode;
        [FormerlySerializedAs("selectOnUp"), SerializeField]
        private Selectable m_SelectOnUp;
        [SerializeField, FormerlySerializedAs("selectOnDown")]
        private Selectable m_SelectOnDown;
        [SerializeField, FormerlySerializedAs("selectOnLeft")]
        private Selectable m_SelectOnLeft;
        [FormerlySerializedAs("selectOnRight"), SerializeField]
        private Selectable m_SelectOnRight;
        public Mode mode
        {
            get
            {
                return this.m_Mode;
            }
            set
            {
                this.m_Mode = value;
            }
        }
        public Selectable selectOnUp
        {
            get
            {
                return this.m_SelectOnUp;
            }
            set
            {
                this.m_SelectOnUp = value;
            }
        }
        public Selectable selectOnDown
        {
            get
            {
                return this.m_SelectOnDown;
            }
            set
            {
                this.m_SelectOnDown = value;
            }
        }
        public Selectable selectOnLeft
        {
            get
            {
                return this.m_SelectOnLeft;
            }
            set
            {
                this.m_SelectOnLeft = value;
            }
        }
        public Selectable selectOnRight
        {
            get
            {
                return this.m_SelectOnRight;
            }
            set
            {
                this.m_SelectOnRight = value;
            }
        }
        public static Navigation defaultNavigation
        {
            get
            {
                return new Navigation { m_Mode = Mode.Automatic };
            }
        }
        [Flags]
        public enum Mode
        {
            None,
            Horizontal,
            Vertical,
            Automatic,
            Explicit
        }
    }
}

