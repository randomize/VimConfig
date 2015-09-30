namespace UnityEngine.EventSystems
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Text;
    using UnityEngine;
    using UnityEngine.Serialization;

    [AddComponentMenu("Event/Event System")]
    public class EventSystem : UIBehaviour
    {
        private BaseInputModule m_CurrentInputModule;
        private GameObject m_CurrentSelected;
        [SerializeField]
        private int m_DragThreshold = 5;
        private BaseEventData m_DummyData;
        [SerializeField, FormerlySerializedAs("m_Selected")]
        private GameObject m_FirstSelected;
        private bool m_SelectionGuard;
        [SerializeField]
        private bool m_sendNavigationEvents = true;
        private List<BaseInputModule> m_SystemInputModules = new List<BaseInputModule>();
        private static readonly Comparison<RaycastResult> s_RaycastComparer = new Comparison<RaycastResult>(EventSystem.RaycastComparer);

        protected EventSystem()
        {
        }

        private void ChangeEventModule(BaseInputModule module)
        {
            if (this.m_CurrentInputModule != module)
            {
                if (this.m_CurrentInputModule != null)
                {
                    this.m_CurrentInputModule.DeactivateModule();
                }
                if (module != null)
                {
                    module.ActivateModule();
                }
                this.m_CurrentInputModule = module;
            }
        }

        public bool IsPointerOverGameObject()
        {
            return this.IsPointerOverGameObject(-1);
        }

        public bool IsPointerOverGameObject(int pointerId)
        {
            if (this.m_CurrentInputModule == null)
            {
                return false;
            }
            return this.m_CurrentInputModule.IsPointerOverGameObject(pointerId);
        }

        protected override void OnDisable()
        {
            if (this.m_CurrentInputModule != null)
            {
                this.m_CurrentInputModule.DeactivateModule();
                this.m_CurrentInputModule = null;
            }
            if (current == this)
            {
                current = null;
            }
            base.OnDisable();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (current == null)
            {
                current = this;
            }
            else
            {
                Debug.LogWarning("Multiple EventSystems in scene... this is not supported");
            }
        }

        public void RaycastAll(PointerEventData eventData, List<RaycastResult> raycastResults)
        {
            raycastResults.Clear();
            List<BaseRaycaster> raycasters = RaycasterManager.GetRaycasters();
            for (int i = 0; i < raycasters.Count; i++)
            {
                BaseRaycaster raycaster = raycasters[i];
                if ((raycaster != null) && raycaster.IsActive())
                {
                    raycaster.Raycast(eventData, raycastResults);
                }
            }
            raycastResults.Sort(s_RaycastComparer);
        }

        private static int RaycastComparer(RaycastResult lhs, RaycastResult rhs)
        {
            if (lhs.module != rhs.module)
            {
                if (((lhs.module.eventCamera != null) && (rhs.module.eventCamera != null)) && (lhs.module.eventCamera.depth != rhs.module.eventCamera.depth))
                {
                    if (lhs.module.eventCamera.depth < rhs.module.eventCamera.depth)
                    {
                        return 1;
                    }
                    if (lhs.module.eventCamera.depth == rhs.module.eventCamera.depth)
                    {
                        return 0;
                    }
                    return -1;
                }
                if (lhs.module.sortOrderPriority != rhs.module.sortOrderPriority)
                {
                    return rhs.module.sortOrderPriority.CompareTo(lhs.module.sortOrderPriority);
                }
                if (lhs.module.renderOrderPriority != rhs.module.renderOrderPriority)
                {
                    return rhs.module.renderOrderPriority.CompareTo(lhs.module.renderOrderPriority);
                }
            }
            if (lhs.sortingLayer != rhs.sortingLayer)
            {
                int layerValueFromID = SortingLayer.GetLayerValueFromID(rhs.sortingLayer);
                int num2 = SortingLayer.GetLayerValueFromID(lhs.sortingLayer);
                return layerValueFromID.CompareTo(num2);
            }
            if (lhs.sortingOrder != rhs.sortingOrder)
            {
                return rhs.sortingOrder.CompareTo(lhs.sortingOrder);
            }
            if (lhs.depth != rhs.depth)
            {
                return rhs.depth.CompareTo(lhs.depth);
            }
            if (lhs.distance != rhs.distance)
            {
                return lhs.distance.CompareTo(rhs.distance);
            }
            return lhs.index.CompareTo(rhs.index);
        }

        public void SetSelectedGameObject(GameObject selected)
        {
            this.SetSelectedGameObject(selected, this.baseEventDataCache);
        }

        public void SetSelectedGameObject(GameObject selected, BaseEventData pointer)
        {
            if (this.m_SelectionGuard)
            {
                Debug.LogError("Attempting to select " + selected + "while already selecting an object.");
            }
            else
            {
                this.m_SelectionGuard = true;
                if (selected == this.m_CurrentSelected)
                {
                    this.m_SelectionGuard = false;
                }
                else
                {
                    ExecuteEvents.Execute<IDeselectHandler>(this.m_CurrentSelected, pointer, ExecuteEvents.deselectHandler);
                    this.m_CurrentSelected = selected;
                    ExecuteEvents.Execute<ISelectHandler>(this.m_CurrentSelected, pointer, ExecuteEvents.selectHandler);
                    this.m_SelectionGuard = false;
                }
            }
        }

        private void TickModules()
        {
            for (int i = 0; i < this.m_SystemInputModules.Count; i++)
            {
                if (this.m_SystemInputModules[i] != null)
                {
                    this.m_SystemInputModules[i].UpdateModule();
                }
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<b>Selected:</b>" + this.currentSelectedGameObject);
            builder.AppendLine();
            builder.AppendLine();
            builder.AppendLine((this.m_CurrentInputModule == null) ? "No module" : this.m_CurrentInputModule.ToString());
            return builder.ToString();
        }

        protected virtual void Update()
        {
            if (current == this)
            {
                this.TickModules();
                bool flag = false;
                for (int i = 0; i < this.m_SystemInputModules.Count; i++)
                {
                    BaseInputModule module = this.m_SystemInputModules[i];
                    if (module.IsModuleSupported() && module.ShouldActivateModule())
                    {
                        if (this.m_CurrentInputModule != module)
                        {
                            this.ChangeEventModule(module);
                            flag = true;
                        }
                        break;
                    }
                }
                if (this.m_CurrentInputModule == null)
                {
                    for (int j = 0; j < this.m_SystemInputModules.Count; j++)
                    {
                        BaseInputModule module2 = this.m_SystemInputModules[j];
                        if (module2.IsModuleSupported())
                        {
                            this.ChangeEventModule(module2);
                            flag = true;
                            break;
                        }
                    }
                }
                if (!flag && (this.m_CurrentInputModule != null))
                {
                    this.m_CurrentInputModule.Process();
                }
            }
        }

        public void UpdateModules()
        {
            base.GetComponents<BaseInputModule>(this.m_SystemInputModules);
            for (int i = this.m_SystemInputModules.Count - 1; i >= 0; i--)
            {
                if ((this.m_SystemInputModules[i] == null) || !this.m_SystemInputModules[i].IsActive())
                {
                    this.m_SystemInputModules.RemoveAt(i);
                }
            }
        }

        public bool alreadySelecting
        {
            get
            {
                return this.m_SelectionGuard;
            }
        }

        private BaseEventData baseEventDataCache
        {
            get
            {
                if (this.m_DummyData == null)
                {
                    this.m_DummyData = new BaseEventData(this);
                }
                return this.m_DummyData;
            }
        }

        public static EventSystem current
        {
            [CompilerGenerated]
            get
            {
                return <current>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                <current>k__BackingField = value;
            }
        }

        public BaseInputModule currentInputModule
        {
            get
            {
                return this.m_CurrentInputModule;
            }
        }

        public GameObject currentSelectedGameObject
        {
            get
            {
                return this.m_CurrentSelected;
            }
        }

        public GameObject firstSelectedGameObject
        {
            get
            {
                return this.m_FirstSelected;
            }
            set
            {
                this.m_FirstSelected = value;
            }
        }

        [Obsolete("lastSelectedGameObject is no longer supported")]
        public GameObject lastSelectedGameObject
        {
            get
            {
                return null;
            }
        }

        public int pixelDragThreshold
        {
            get
            {
                return this.m_DragThreshold;
            }
            set
            {
                this.m_DragThreshold = value;
            }
        }

        public bool sendNavigationEvents
        {
            get
            {
                return this.m_sendNavigationEvents;
            }
            set
            {
                this.m_sendNavigationEvents = value;
            }
        }
    }
}

