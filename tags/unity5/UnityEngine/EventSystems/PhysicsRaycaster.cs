namespace UnityEngine.EventSystems
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [RequireComponent(typeof(Camera)), AddComponentMenu("Event/Physics Raycaster")]
    public class PhysicsRaycaster : BaseRaycaster
    {
        [CompilerGenerated]
        private static Comparison<RaycastHit> <>f__am$cache2;
        protected const int kNoEventMaskSet = -1;
        protected Camera m_EventCamera;
        [SerializeField]
        protected LayerMask m_EventMask = -1;

        protected PhysicsRaycaster()
        {
        }

        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            if (this.eventCamera != null)
            {
                Ray ray = this.eventCamera.ScreenPointToRay((Vector3) eventData.position);
                float maxDistance = this.eventCamera.farClipPlane - this.eventCamera.nearClipPlane;
                RaycastHit[] array = Physics.RaycastAll(ray, maxDistance, this.finalEventMask);
                if (array.Length > 1)
                {
                    if (<>f__am$cache2 == null)
                    {
                        <>f__am$cache2 = (r1, r2) => r1.distance.CompareTo(r2.distance);
                    }
                    Array.Sort<RaycastHit>(array, <>f__am$cache2);
                }
                if (array.Length != 0)
                {
                    int index = 0;
                    int length = array.Length;
                    while (index < length)
                    {
                        RaycastResult item = new RaycastResult {
                            gameObject = array[index].collider.gameObject,
                            module = this,
                            distance = array[index].distance,
                            worldPosition = array[index].point,
                            worldNormal = array[index].normal,
                            screenPosition = eventData.position,
                            index = resultAppendList.Count,
                            sortingLayer = 0,
                            sortingOrder = 0
                        };
                        resultAppendList.Add(item);
                        index++;
                    }
                }
            }
        }

        public virtual int depth
        {
            get
            {
                return ((this.eventCamera == null) ? 0xffffff : ((int) this.eventCamera.depth));
            }
        }

        public override Camera eventCamera
        {
            get
            {
                if (this.m_EventCamera == null)
                {
                    this.m_EventCamera = base.GetComponent<Camera>();
                }
                if (this.m_EventCamera == null)
                {
                }
                return Camera.main;
            }
        }

        public LayerMask eventMask
        {
            get
            {
                return this.m_EventMask;
            }
            set
            {
                this.m_EventMask = value;
            }
        }

        public int finalEventMask
        {
            get
            {
                return ((this.eventCamera == null) ? -1 : (this.eventCamera.cullingMask & this.m_EventMask));
            }
        }
    }
}

