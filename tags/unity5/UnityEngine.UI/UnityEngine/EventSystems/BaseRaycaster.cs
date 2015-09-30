namespace UnityEngine.EventSystems
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class BaseRaycaster : UIBehaviour
    {
        protected BaseRaycaster()
        {
        }

        protected override void OnDisable()
        {
            RaycasterManager.RemoveRaycasters(this);
            base.OnDisable();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RaycasterManager.AddRaycaster(this);
        }

        public abstract void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList);
        public override string ToString()
        {
            object[] objArray1 = new object[] { "Name: ", base.gameObject, "\neventCamera: ", this.eventCamera, "\nsortOrderPriority: ", this.sortOrderPriority, "\nrenderOrderPriority: ", this.renderOrderPriority };
            return string.Concat(objArray1);
        }

        public abstract Camera eventCamera { get; }

        [Obsolete("Please use sortOrderPriority and renderOrderPriority", false)]
        public virtual int priority
        {
            get
            {
                return 0;
            }
        }

        public virtual int renderOrderPriority
        {
            get
            {
                return -2147483648;
            }
        }

        public virtual int sortOrderPriority
        {
            get
            {
                return -2147483648;
            }
        }
    }
}

