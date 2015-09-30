namespace UnityEngine.UI
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.Serialization;

    [AddComponentMenu("Event/Graphic Raycaster"), RequireComponent(typeof(Canvas))]
    public class GraphicRaycaster : BaseRaycaster
    {
        [CompilerGenerated]
        private static Comparison<Graphic> <>f__am$cache6;
        protected const int kNoEventMaskSet = -1;
        [SerializeField]
        protected LayerMask m_BlockingMask = -1;
        [FormerlySerializedAs("blockingObjects"), SerializeField]
        private BlockingObjects m_BlockingObjects;
        private Canvas m_Canvas;
        [FormerlySerializedAs("ignoreReversedGraphics"), SerializeField]
        private bool m_IgnoreReversedGraphics = true;
        [NonSerialized]
        private List<Graphic> m_RaycastResults = new List<Graphic>();
        [NonSerialized]
        private static readonly List<Graphic> s_SortedGraphics = new List<Graphic>();

        protected GraphicRaycaster()
        {
        }

        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            if (this.canvas != null)
            {
                Vector2 vector;
                if (this.eventCamera == null)
                {
                    vector = new Vector2(eventData.position.x / ((float) Screen.width), eventData.position.y / ((float) Screen.height));
                }
                else
                {
                    vector = this.eventCamera.ScreenToViewportPoint((Vector3) eventData.position);
                }
                if (((vector.x >= 0f) && (vector.x <= 1f)) && ((vector.y >= 0f) && (vector.y <= 1f)))
                {
                    float maxValue = float.MaxValue;
                    Ray ray = new Ray();
                    if (this.eventCamera != null)
                    {
                        ray = this.eventCamera.ScreenPointToRay((Vector3) eventData.position);
                    }
                    if ((this.canvas.renderMode != RenderMode.ScreenSpaceOverlay) && (this.blockingObjects != BlockingObjects.None))
                    {
                        RaycastHit hit;
                        float maxDistance = 100f;
                        if (this.eventCamera != null)
                        {
                            maxDistance = this.eventCamera.farClipPlane - this.eventCamera.nearClipPlane;
                        }
                        if (((this.blockingObjects == BlockingObjects.ThreeD) || (this.blockingObjects == BlockingObjects.All)) && Physics.Raycast(ray, out hit, maxDistance, (int) this.m_BlockingMask))
                        {
                            maxValue = hit.distance;
                        }
                        if ((this.blockingObjects == BlockingObjects.TwoD) || (this.blockingObjects == BlockingObjects.All))
                        {
                            RaycastHit2D hitd = Physics2D.Raycast(ray.origin, ray.direction, maxDistance, (int) this.m_BlockingMask);
                            if (hitd.collider != null)
                            {
                                maxValue = hitd.fraction * maxDistance;
                            }
                        }
                    }
                    this.m_RaycastResults.Clear();
                    Raycast(this.canvas, this.eventCamera, eventData.position, this.m_RaycastResults);
                    for (int i = 0; i < this.m_RaycastResults.Count; i++)
                    {
                        GameObject gameObject = this.m_RaycastResults[i].gameObject;
                        bool flag = true;
                        if (this.ignoreReversedGraphics)
                        {
                            if (this.eventCamera == null)
                            {
                                Vector3 rhs = (Vector3) (gameObject.transform.rotation * Vector3.forward);
                                flag = Vector3.Dot(Vector3.forward, rhs) > 0f;
                            }
                            else
                            {
                                Vector3 lhs = (Vector3) (this.eventCamera.transform.rotation * Vector3.forward);
                                Vector3 vector4 = (Vector3) (gameObject.transform.rotation * Vector3.forward);
                                flag = Vector3.Dot(lhs, vector4) > 0f;
                            }
                        }
                        if (flag)
                        {
                            float num4 = 0f;
                            if ((this.eventCamera == null) || (this.canvas.renderMode == RenderMode.ScreenSpaceOverlay))
                            {
                                num4 = 0f;
                            }
                            else
                            {
                                Transform transform = gameObject.transform;
                                Vector3 forward = transform.forward;
                                num4 = Vector3.Dot(forward, transform.position - ray.origin) / Vector3.Dot(forward, ray.direction);
                                if (num4 < 0f)
                                {
                                    continue;
                                }
                            }
                            if (num4 < maxValue)
                            {
                                RaycastResult item = new RaycastResult {
                                    gameObject = gameObject,
                                    module = this,
                                    distance = num4,
                                    screenPosition = eventData.position,
                                    index = resultAppendList.Count,
                                    depth = this.m_RaycastResults[i].depth,
                                    sortingLayer = this.canvas.cachedSortingLayerValue,
                                    sortingOrder = this.canvas.sortingOrder
                                };
                                resultAppendList.Add(item);
                            }
                        }
                    }
                }
            }
        }

        private static void Raycast(Canvas canvas, Camera eventCamera, Vector2 pointerPosition, List<Graphic> results)
        {
            IList<Graphic> graphicsForCanvas = GraphicRegistry.GetGraphicsForCanvas(canvas);
            s_SortedGraphics.Clear();
            for (int i = 0; i < graphicsForCanvas.Count; i++)
            {
                Graphic item = graphicsForCanvas[i];
                if (((item.depth != -1) && item.raycastTarget) && (RectTransformUtility.RectangleContainsScreenPoint(item.rectTransform, pointerPosition, eventCamera) && item.Raycast(pointerPosition, eventCamera)))
                {
                    s_SortedGraphics.Add(item);
                }
            }
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = (g1, g2) => g2.depth.CompareTo(g1.depth);
            }
            s_SortedGraphics.Sort(<>f__am$cache6);
            for (int j = 0; j < s_SortedGraphics.Count; j++)
            {
                results.Add(s_SortedGraphics[j]);
            }
        }

        public BlockingObjects blockingObjects
        {
            get
            {
                return this.m_BlockingObjects;
            }
            set
            {
                this.m_BlockingObjects = value;
            }
        }

        private Canvas canvas
        {
            get
            {
                if (this.m_Canvas == null)
                {
                    this.m_Canvas = base.GetComponent<Canvas>();
                }
                return this.m_Canvas;
            }
        }

        public override Camera eventCamera
        {
            get
            {
                if ((this.canvas.renderMode == RenderMode.ScreenSpaceOverlay) || ((this.canvas.renderMode == RenderMode.ScreenSpaceCamera) && (this.canvas.worldCamera == null)))
                {
                    return null;
                }
                return ((this.canvas.worldCamera == null) ? Camera.main : this.canvas.worldCamera);
            }
        }

        public bool ignoreReversedGraphics
        {
            get
            {
                return this.m_IgnoreReversedGraphics;
            }
            set
            {
                this.m_IgnoreReversedGraphics = value;
            }
        }

        public override int renderOrderPriority
        {
            get
            {
                if (this.canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    return this.canvas.renderOrder;
                }
                return base.renderOrderPriority;
            }
        }

        public override int sortOrderPriority
        {
            get
            {
                if (this.canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    return this.canvas.sortingOrder;
                }
                return base.sortOrderPriority;
            }
        }

        public enum BlockingObjects
        {
            None,
            TwoD,
            ThreeD,
            All
        }
    }
}

