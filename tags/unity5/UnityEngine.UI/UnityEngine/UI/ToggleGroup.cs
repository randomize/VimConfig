namespace UnityEngine.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.EventSystems;

    [AddComponentMenu("UI/Toggle Group", 0x20)]
    public class ToggleGroup : UIBehaviour
    {
        [CompilerGenerated]
        private static Predicate<Toggle> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<Toggle, bool> <>f__am$cache3;
        [SerializeField]
        private bool m_AllowSwitchOff;
        private List<Toggle> m_Toggles = new List<Toggle>();

        protected ToggleGroup()
        {
        }

        public IEnumerable<Toggle> ActiveToggles()
        {
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = x => x.isOn;
            }
            return this.m_Toggles.Where<Toggle>(<>f__am$cache3);
        }

        public bool AnyTogglesOn()
        {
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = x => x.isOn;
            }
            return (this.m_Toggles.Find(<>f__am$cache2) != null);
        }

        public void NotifyToggleOn(Toggle toggle)
        {
            this.ValidateToggleIsInGroup(toggle);
            for (int i = 0; i < this.m_Toggles.Count; i++)
            {
                if (this.m_Toggles[i] != toggle)
                {
                    this.m_Toggles[i].isOn = false;
                }
            }
        }

        public void RegisterToggle(Toggle toggle)
        {
            if (!this.m_Toggles.Contains(toggle))
            {
                this.m_Toggles.Add(toggle);
            }
        }

        public void SetAllTogglesOff()
        {
            bool allowSwitchOff = this.m_AllowSwitchOff;
            this.m_AllowSwitchOff = true;
            for (int i = 0; i < this.m_Toggles.Count; i++)
            {
                this.m_Toggles[i].isOn = false;
            }
            this.m_AllowSwitchOff = allowSwitchOff;
        }

        public void UnregisterToggle(Toggle toggle)
        {
            if (this.m_Toggles.Contains(toggle))
            {
                this.m_Toggles.Remove(toggle);
            }
        }

        private void ValidateToggleIsInGroup(Toggle toggle)
        {
            if ((toggle == null) || !this.m_Toggles.Contains(toggle))
            {
                object[] args = new object[] { toggle, this };
                throw new ArgumentException(string.Format("Toggle {0} is not part of ToggleGroup {1}", args));
            }
        }

        public bool allowSwitchOff
        {
            get
            {
                return this.m_AllowSwitchOff;
            }
            set
            {
                this.m_AllowSwitchOff = value;
            }
        }
    }
}

