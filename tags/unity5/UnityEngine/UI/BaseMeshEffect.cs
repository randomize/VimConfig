namespace UnityEngine.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    [ExecuteInEditMode]
    public abstract class BaseMeshEffect : UIBehaviour, IMeshModifier
    {
        [NonSerialized]
        private Graphic m_Graphic;

        protected BaseMeshEffect()
        {
        }

        public abstract void ModifyMesh(Mesh mesh);
        protected override void OnDidApplyAnimationProperties()
        {
            if (this.graphic != null)
            {
                this.graphic.SetVerticesDirty();
            }
            base.OnDidApplyAnimationProperties();
        }

        protected override void OnDisable()
        {
            if (this.graphic != null)
            {
                this.graphic.SetVerticesDirty();
            }
            base.OnDisable();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (this.graphic != null)
            {
                this.graphic.SetVerticesDirty();
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            if (this.graphic != null)
            {
                this.graphic.SetVerticesDirty();
            }
        }

        protected Graphic graphic
        {
            get
            {
                if (this.m_Graphic == null)
                {
                    this.m_Graphic = base.GetComponent<Graphic>();
                }
                return this.m_Graphic;
            }
        }
    }
}

