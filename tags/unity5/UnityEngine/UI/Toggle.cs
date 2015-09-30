namespace UnityEngine.UI
{
    using System;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.Serialization;

    [AddComponentMenu("UI/Toggle", 0x1f), RequireComponent(typeof(RectTransform))]
    public class Toggle : Selectable, IEventSystemHandler, IPointerClickHandler, ISubmitHandler, ICanvasElement
    {
        public Graphic graphic;
        [SerializeField]
        private ToggleGroup m_Group;
        [Tooltip("Is the toggle currently on or off?"), SerializeField, FormerlySerializedAs("m_IsActive")]
        private bool m_IsOn;
        public ToggleEvent onValueChanged = new ToggleEvent();
        public ToggleTransition toggleTransition = ToggleTransition.Fade;

        protected Toggle()
        {
        }

        private void InternalToggle()
        {
            if (this.IsActive() && this.IsInteractable())
            {
                this.isOn = !this.isOn;
            }
        }

        protected override void OnDisable()
        {
            this.SetToggleGroup(null, false);
            base.OnDisable();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.SetToggleGroup(this.m_Group, false);
            this.PlayEffect(true);
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                this.InternalToggle();
            }
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            this.InternalToggle();
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            this.Set(this.m_IsOn, false);
            this.PlayEffect(this.toggleTransition == ToggleTransition.None);
            if ((PrefabUtility.GetPrefabType(this) != PrefabType.Prefab) && !Application.isPlaying)
            {
                CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
            }
        }

        private void PlayEffect(bool instant)
        {
            if (this.graphic != null)
            {
                if (!Application.isPlaying)
                {
                    this.graphic.canvasRenderer.SetAlpha(!this.m_IsOn ? 0f : 1f);
                }
                else
                {
                    this.graphic.CrossFadeAlpha(!this.m_IsOn ? 0f : 1f, !instant ? 0.1f : 0f, true);
                }
            }
        }

        public virtual void Rebuild(CanvasUpdate executing)
        {
            if (executing == CanvasUpdate.Prelayout)
            {
                this.onValueChanged.Invoke(this.m_IsOn);
            }
        }

        private void Set(bool value)
        {
            this.Set(value, true);
        }

        private void Set(bool value, bool sendCallback)
        {
            if (this.m_IsOn != value)
            {
                this.m_IsOn = value;
                if (((this.m_Group != null) && this.IsActive()) && (this.m_IsOn || (!this.m_Group.AnyTogglesOn() && !this.m_Group.allowSwitchOff)))
                {
                    this.m_IsOn = true;
                    this.m_Group.NotifyToggleOn(this);
                }
                this.PlayEffect(this.toggleTransition == ToggleTransition.None);
                if (sendCallback)
                {
                    this.onValueChanged.Invoke(this.m_IsOn);
                }
            }
        }

        private void SetToggleGroup(ToggleGroup newGroup, bool setMemberValue)
        {
            ToggleGroup group = this.m_Group;
            if (this.m_Group != null)
            {
                this.m_Group.UnregisterToggle(this);
            }
            if (setMemberValue)
            {
                this.m_Group = newGroup;
            }
            if ((this.m_Group != null) && this.IsActive())
            {
                this.m_Group.RegisterToggle(this);
            }
            if (((newGroup != null) && (newGroup != group)) && (this.isOn && this.IsActive()))
            {
                this.m_Group.NotifyToggleOn(this);
            }
        }

        protected override void Start()
        {
            this.PlayEffect(true);
        }

        Transform ICanvasElement.get_transform()
        {
            return base.transform;
        }

        bool ICanvasElement.IsDestroyed()
        {
            return base.IsDestroyed();
        }

        public ToggleGroup group
        {
            get
            {
                return this.m_Group;
            }
            set
            {
                this.m_Group = value;
                if (Application.isPlaying)
                {
                    this.SetToggleGroup(this.m_Group, true);
                    this.PlayEffect(true);
                }
            }
        }

        public bool isOn
        {
            get
            {
                return this.m_IsOn;
            }
            set
            {
                this.Set(value);
            }
        }

        [Serializable]
        public class ToggleEvent : UnityEvent<bool>
        {
        }

        public enum ToggleTransition
        {
            None,
            Fade
        }
    }
}

