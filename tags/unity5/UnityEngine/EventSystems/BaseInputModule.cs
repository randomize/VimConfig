namespace UnityEngine.EventSystems
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [RequireComponent(typeof(EventSystem))]
    public abstract class BaseInputModule : UIBehaviour
    {
        private AxisEventData m_AxisEventData;
        private BaseEventData m_BaseEventData;
        private EventSystem m_EventSystem;
        [NonSerialized]
        protected List<RaycastResult> m_RaycastResultCache = new List<RaycastResult>();

        protected BaseInputModule()
        {
        }

        public virtual void ActivateModule()
        {
        }

        public virtual void DeactivateModule()
        {
        }

        protected static MoveDirection DetermineMoveDirection(float x, float y)
        {
            return DetermineMoveDirection(x, y, 0.6f);
        }

        protected static MoveDirection DetermineMoveDirection(float x, float y, float deadZone)
        {
            Vector2 vector = new Vector2(x, y);
            if (vector.sqrMagnitude < (deadZone * deadZone))
            {
                return MoveDirection.None;
            }
            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                if (x > 0f)
                {
                    return MoveDirection.Right;
                }
                return MoveDirection.Left;
            }
            if (y > 0f)
            {
                return MoveDirection.Up;
            }
            return MoveDirection.Down;
        }

        protected static GameObject FindCommonRoot(GameObject g1, GameObject g2)
        {
            if ((g1 != null) && (g2 != null))
            {
                for (Transform transform = g1.transform; transform != null; transform = transform.parent)
                {
                    for (Transform transform2 = g2.transform; transform2 != null; transform2 = transform2.parent)
                    {
                        if (transform == transform2)
                        {
                            return transform.gameObject;
                        }
                    }
                }
            }
            return null;
        }

        protected static RaycastResult FindFirstRaycast(List<RaycastResult> candidates)
        {
            for (int i = 0; i < candidates.Count; i++)
            {
                RaycastResult result = candidates[i];
                if (result.gameObject != null)
                {
                    return candidates[i];
                }
            }
            return new RaycastResult();
        }

        protected virtual AxisEventData GetAxisEventData(float x, float y, float moveDeadZone)
        {
            if (this.m_AxisEventData == null)
            {
                this.m_AxisEventData = new AxisEventData(this.eventSystem);
            }
            this.m_AxisEventData.Reset();
            this.m_AxisEventData.moveVector = new Vector2(x, y);
            this.m_AxisEventData.moveDir = DetermineMoveDirection(x, y, moveDeadZone);
            return this.m_AxisEventData;
        }

        protected virtual BaseEventData GetBaseEventData()
        {
            if (this.m_BaseEventData == null)
            {
                this.m_BaseEventData = new BaseEventData(this.eventSystem);
            }
            this.m_BaseEventData.Reset();
            return this.m_BaseEventData;
        }

        protected void HandlePointerExitAndEnter(PointerEventData currentPointerData, GameObject newEnterTarget)
        {
            if ((newEnterTarget == null) || (currentPointerData.pointerEnter == null))
            {
                for (int i = 0; i < currentPointerData.hovered.Count; i++)
                {
                    ExecuteEvents.Execute<IPointerExitHandler>(currentPointerData.hovered[i], currentPointerData, ExecuteEvents.pointerExitHandler);
                }
                currentPointerData.hovered.Clear();
                if (newEnterTarget == null)
                {
                    currentPointerData.pointerEnter = newEnterTarget;
                    return;
                }
            }
            if ((currentPointerData.pointerEnter != newEnterTarget) || (newEnterTarget == null))
            {
                GameObject obj2 = FindCommonRoot(currentPointerData.pointerEnter, newEnterTarget);
                if (currentPointerData.pointerEnter != null)
                {
                    for (Transform transform = currentPointerData.pointerEnter.transform; transform != null; transform = transform.parent)
                    {
                        if ((obj2 != null) && (obj2.transform == transform))
                        {
                            break;
                        }
                        ExecuteEvents.Execute<IPointerExitHandler>(transform.gameObject, currentPointerData, ExecuteEvents.pointerExitHandler);
                        currentPointerData.hovered.Remove(transform.gameObject);
                    }
                }
                currentPointerData.pointerEnter = newEnterTarget;
                if (newEnterTarget != null)
                {
                    for (Transform transform2 = newEnterTarget.transform; (transform2 != null) && (transform2.gameObject != obj2); transform2 = transform2.parent)
                    {
                        ExecuteEvents.Execute<IPointerEnterHandler>(transform2.gameObject, currentPointerData, ExecuteEvents.pointerEnterHandler);
                        currentPointerData.hovered.Add(transform2.gameObject);
                    }
                }
            }
        }

        public virtual bool IsModuleSupported()
        {
            return true;
        }

        public virtual bool IsPointerOverGameObject(int pointerId)
        {
            return false;
        }

        protected override void OnDisable()
        {
            this.m_EventSystem.UpdateModules();
            base.OnDisable();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.m_EventSystem = base.GetComponent<EventSystem>();
            this.m_EventSystem.UpdateModules();
        }

        public abstract void Process();
        public virtual bool ShouldActivateModule()
        {
            return (base.enabled && base.gameObject.activeInHierarchy);
        }

        public virtual void UpdateModule()
        {
        }

        protected EventSystem eventSystem
        {
            get
            {
                return this.m_EventSystem;
            }
        }
    }
}

