namespace UnityEngine.EventSystems
{
    using System;
    using UnityEngine;

    public abstract class UIBehaviour : MonoBehaviour
    {
        protected UIBehaviour()
        {
        }

        protected virtual void Awake()
        {
        }

        public virtual bool IsActive()
        {
            return base.isActiveAndEnabled;
        }

        public bool IsDestroyed()
        {
            return (this == null);
        }

        protected virtual void OnBeforeTransformParentChanged()
        {
        }

        protected virtual void OnCanvasGroupChanged()
        {
        }

        protected virtual void OnCanvasHierarchyChanged()
        {
        }

        protected virtual void OnDestroy()
        {
        }

        protected virtual void OnDidApplyAnimationProperties()
        {
        }

        protected virtual void OnDisable()
        {
        }

        protected virtual void OnEnable()
        {
        }

        protected virtual void OnRectTransformDimensionsChange()
        {
        }

        protected virtual void OnTransformParentChanged()
        {
        }

        protected virtual void OnValidate()
        {
        }

        protected virtual void Reset()
        {
        }

        protected virtual void Start()
        {
        }
    }
}

