namespace UnityEngine.EventSystems
{
    using System;
    using UnityEngine;

    public class BaseEventData
    {
        private readonly EventSystem m_EventSystem;
        private bool m_Used;

        public BaseEventData(EventSystem eventSystem)
        {
            this.m_EventSystem = eventSystem;
        }

        public void Reset()
        {
            this.m_Used = false;
        }

        public void Use()
        {
            this.m_Used = true;
        }

        public BaseInputModule currentInputModule
        {
            get
            {
                return this.m_EventSystem.currentInputModule;
            }
        }

        public GameObject selectedObject
        {
            get
            {
                return this.m_EventSystem.currentSelectedGameObject;
            }
            set
            {
                this.m_EventSystem.SetSelectedGameObject(value, this);
            }
        }

        public bool used
        {
            get
            {
                return this.m_Used;
            }
        }
    }
}

