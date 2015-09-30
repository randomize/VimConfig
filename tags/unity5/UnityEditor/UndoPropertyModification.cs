namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct UndoPropertyModification
    {
        public PropertyModification previousValue;
        public PropertyModification currentValue;
        private int m_KeepPrefabOverride;
        public bool keepPrefabOverride
        {
            get
            {
                return (this.m_KeepPrefabOverride != 0);
            }
            set
            {
                this.m_KeepPrefabOverride = !value ? 0 : 1;
            }
        }
    }
}

