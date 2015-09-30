namespace UnityEngine.EventSystems
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [AddComponentMenu("Event/Physics 2D Raycaster"), RequireComponent(typeof(Camera))]
    public class Physics2DRaycaster : PhysicsRaycaster
    {
        protected Physics2DRaycaster()
        {
        }

        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            if (this.eventCamera != null)
            {
                Ray ray = this.eventCamera.ScreenPointToRay((Vector3) eventData.position);
                float distance = this.eventCamera.farClipPlane - this.eventCamera.nearClipPlane;
                RaycastHit2D[] hitdArray = Physics2D.RaycastAll(ray.origin, ray.direction, distance, base.finalEventMask);
                if (hitdArray.Length != 0)
                {
                    int index = 0;
                    int length = hitdArray.Length;
                    while (index < length)
                    {
                        SpriteRenderer component = hitdArray[index].collider.gameObject.GetComponent<SpriteRenderer>();
                        RaycastResult item = new RaycastResult {
                            gameObject = hitdArray[index].collider.gameObject,
                            module = this,
                            distance = Vector3.Distance(this.eventCamera.transform.position, hitdArray[index].transform.position),
                            worldPosition = (Vector3) hitdArray[index].point,
                            worldNormal = (Vector3) hitdArray[index].normal,
                            screenPosition = eventData.position,
                            index = resultAppendList.Count,
                            sortingLayer = (component == null) ? 0 : component.sortingLayerID,
                            sortingOrder = (component == null) ? 0 : component.sortingOrder
                        };
                        resultAppendList.Add(item);
                        index++;
                    }
                }
            }
        }
    }
}

