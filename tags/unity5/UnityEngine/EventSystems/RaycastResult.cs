namespace UnityEngine.EventSystems
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public struct RaycastResult
    {
        private GameObject m_GameObject;
        public BaseRaycaster module;
        public float distance;
        public float index;
        public int depth;
        public int sortingLayer;
        public int sortingOrder;
        public Vector3 worldPosition;
        public Vector3 worldNormal;
        public Vector2 screenPosition;
        public GameObject gameObject
        {
            get
            {
                return this.m_GameObject;
            }
            set
            {
                this.m_GameObject = value;
            }
        }
        public bool isValid
        {
            get
            {
                return ((this.module != null) && (this.gameObject != null));
            }
        }
        public void Clear()
        {
            this.gameObject = null;
            this.module = null;
            this.distance = 0f;
            this.index = 0f;
            this.depth = 0;
            this.sortingLayer = 0;
            this.sortingOrder = 0;
            this.worldNormal = Vector3.up;
            this.worldPosition = Vector3.zero;
            this.screenPosition = Vector2.zero;
        }

        public override string ToString()
        {
            if (!this.isValid)
            {
                return string.Empty;
            }
            object[] objArray1 = new object[] { 
                "Name: ", this.gameObject, "\nmodule: ", this.module, "\nmodule camera: ", this.module.GetComponent<Camera>(), "\ndistance: ", this.distance, "\nindex: ", this.index, "\ndepth: ", this.depth, "\nworldNormal: ", this.worldNormal, "\nworldPosition: ", this.worldPosition, 
                "\nscreenPosition: ", this.screenPosition, "\nmodule.sortOrderPriority: ", this.module.sortOrderPriority, "\nmodule.renderOrderPriority: ", this.module.renderOrderPriority, "\nsortingLayer: ", this.sortingLayer, "\nsortingOrder: ", this.sortingOrder
             };
            return string.Concat(objArray1);
        }
    }
}

