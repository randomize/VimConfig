namespace UnityEngine.UI
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.Serialization;

    [DisallowMultipleComponent, ExecuteInEditMode, AddComponentMenu("UI/Selectable", 70), SelectionBase]
    public class Selectable : UIBehaviour, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, ISelectHandler, IDeselectHandler, IMoveHandler
    {
        [SerializeField, FormerlySerializedAs("animationTriggers")]
        private AnimationTriggers m_AnimationTriggers = new AnimationTriggers();
        private readonly List<CanvasGroup> m_CanvasGroupCache = new List<CanvasGroup>();
        [FormerlySerializedAs("colors"), SerializeField]
        private ColorBlock m_Colors = ColorBlock.defaultColorBlock;
        private SelectionState m_CurrentSelectionState;
        private bool m_GroupsAllowInteraction = true;
        [SerializeField, Tooltip("Can the Selectable be interacted with?")]
        private bool m_Interactable = true;
        [SerializeField, FormerlySerializedAs("navigation")]
        private Navigation m_Navigation = Navigation.defaultNavigation;
        [FormerlySerializedAs("spriteState"), SerializeField]
        private SpriteState m_SpriteState;
        [FormerlySerializedAs("highlightGraphic"), FormerlySerializedAs("m_HighlightGraphic"), SerializeField]
        private Graphic m_TargetGraphic;
        [FormerlySerializedAs("transition"), SerializeField]
        private Transition m_Transition = Transition.ColorTint;
        private static List<Selectable> s_List = new List<Selectable>();

        protected Selectable()
        {
        }

        protected override void Awake()
        {
            if (this.m_TargetGraphic == null)
            {
                this.m_TargetGraphic = base.GetComponent<Graphic>();
            }
        }

        private void DoSpriteSwap(Sprite newSprite)
        {
            if (this.image != null)
            {
                this.image.overrideSprite = newSprite;
            }
        }

        protected virtual void DoStateTransition(SelectionState state, bool instant)
        {
            Color normalColor;
            Sprite highlightedSprite;
            string normalTrigger;
            switch (state)
            {
                case SelectionState.Normal:
                    normalColor = this.m_Colors.normalColor;
                    highlightedSprite = null;
                    normalTrigger = this.m_AnimationTriggers.normalTrigger;
                    break;

                case SelectionState.Highlighted:
                    normalColor = this.m_Colors.highlightedColor;
                    highlightedSprite = this.m_SpriteState.highlightedSprite;
                    normalTrigger = this.m_AnimationTriggers.highlightedTrigger;
                    break;

                case SelectionState.Pressed:
                    normalColor = this.m_Colors.pressedColor;
                    highlightedSprite = this.m_SpriteState.pressedSprite;
                    normalTrigger = this.m_AnimationTriggers.pressedTrigger;
                    break;

                case SelectionState.Disabled:
                    normalColor = this.m_Colors.disabledColor;
                    highlightedSprite = this.m_SpriteState.disabledSprite;
                    normalTrigger = this.m_AnimationTriggers.disabledTrigger;
                    break;

                default:
                    normalColor = Color.black;
                    highlightedSprite = null;
                    normalTrigger = string.Empty;
                    break;
            }
            if (base.gameObject.activeInHierarchy)
            {
                switch (this.m_Transition)
                {
                    case Transition.ColorTint:
                        this.StartColorTween((Color) (normalColor * this.m_Colors.colorMultiplier), instant);
                        break;

                    case Transition.SpriteSwap:
                        this.DoSpriteSwap(highlightedSprite);
                        break;

                    case Transition.Animation:
                        this.TriggerAnimation(normalTrigger);
                        break;
                }
            }
        }

        private void EvaluateAndTransitionToSelectionState(BaseEventData eventData)
        {
            if (this.IsActive())
            {
                this.UpdateSelectionState(eventData);
                this.InternalEvaluateAndTransitionToSelectionState(false);
            }
        }

        public Selectable FindSelectable(Vector3 dir)
        {
            dir = dir.normalized;
            Vector3 vector = (Vector3) (Quaternion.Inverse(base.transform.rotation) * dir);
            Vector3 vector2 = base.transform.TransformPoint(GetPointOnRectEdge(base.transform as RectTransform, vector));
            float negativeInfinity = float.NegativeInfinity;
            Selectable selectable = null;
            for (int i = 0; i < s_List.Count; i++)
            {
                Selectable selectable2 = s_List[i];
                if (((selectable2 != this) && (selectable2 != null)) && (selectable2.IsInteractable() && (selectable2.navigation.mode != Navigation.Mode.None)))
                {
                    RectTransform transform = selectable2.transform as RectTransform;
                    Vector3 position = (transform == null) ? Vector3.zero : ((Vector3) transform.rect.center);
                    Vector3 rhs = selectable2.transform.TransformPoint(position) - vector2;
                    float num3 = Vector3.Dot(dir, rhs);
                    if (num3 > 0f)
                    {
                        float num4 = num3 / rhs.sqrMagnitude;
                        if (num4 > negativeInfinity)
                        {
                            negativeInfinity = num4;
                            selectable = selectable2;
                        }
                    }
                }
            }
            return selectable;
        }

        public virtual Selectable FindSelectableOnDown()
        {
            if (this.m_Navigation.mode == Navigation.Mode.Explicit)
            {
                return this.m_Navigation.selectOnDown;
            }
            if ((this.m_Navigation.mode & Navigation.Mode.Vertical) != Navigation.Mode.None)
            {
                return this.FindSelectable((Vector3) (base.transform.rotation * Vector3.down));
            }
            return null;
        }

        public virtual Selectable FindSelectableOnLeft()
        {
            if (this.m_Navigation.mode == Navigation.Mode.Explicit)
            {
                return this.m_Navigation.selectOnLeft;
            }
            if ((this.m_Navigation.mode & Navigation.Mode.Horizontal) != Navigation.Mode.None)
            {
                return this.FindSelectable((Vector3) (base.transform.rotation * Vector3.left));
            }
            return null;
        }

        public virtual Selectable FindSelectableOnRight()
        {
            if (this.m_Navigation.mode == Navigation.Mode.Explicit)
            {
                return this.m_Navigation.selectOnRight;
            }
            if ((this.m_Navigation.mode & Navigation.Mode.Horizontal) != Navigation.Mode.None)
            {
                return this.FindSelectable((Vector3) (base.transform.rotation * Vector3.right));
            }
            return null;
        }

        public virtual Selectable FindSelectableOnUp()
        {
            if (this.m_Navigation.mode == Navigation.Mode.Explicit)
            {
                return this.m_Navigation.selectOnUp;
            }
            if ((this.m_Navigation.mode & Navigation.Mode.Vertical) != Navigation.Mode.None)
            {
                return this.FindSelectable((Vector3) (base.transform.rotation * Vector3.up));
            }
            return null;
        }

        private static Vector3 GetPointOnRectEdge(RectTransform rect, Vector2 dir)
        {
            if (rect == null)
            {
                return Vector3.zero;
            }
            if (dir != Vector2.zero)
            {
                dir = (Vector2) (dir / Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y)));
            }
            dir = rect.rect.center + Vector2.Scale(rect.rect.size, (Vector2) (dir * 0.5f));
            return (Vector3) dir;
        }

        protected virtual void InstantClearState()
        {
            string normalTrigger = this.m_AnimationTriggers.normalTrigger;
            this.isPointerInside = false;
            this.isPointerDown = false;
            this.hasSelection = false;
            switch (this.m_Transition)
            {
                case Transition.ColorTint:
                    this.StartColorTween(Color.white, true);
                    break;

                case Transition.SpriteSwap:
                    this.DoSpriteSwap(null);
                    break;

                case Transition.Animation:
                    this.TriggerAnimation(normalTrigger);
                    break;
            }
        }

        private void InternalEvaluateAndTransitionToSelectionState(bool instant)
        {
            SelectionState currentSelectionState = this.m_CurrentSelectionState;
            if (this.IsActive() && !this.IsInteractable())
            {
                currentSelectionState = SelectionState.Disabled;
            }
            this.DoStateTransition(currentSelectionState, instant);
        }

        protected bool IsHighlighted(BaseEventData eventData)
        {
            if (!this.IsActive())
            {
                return false;
            }
            if (this.IsPressed())
            {
                return false;
            }
            bool hasSelection = this.hasSelection;
            if (eventData is PointerEventData)
            {
                PointerEventData data = eventData as PointerEventData;
                return (hasSelection | ((((this.isPointerDown && !this.isPointerInside) && (data.pointerPress == base.gameObject)) || ((!this.isPointerDown && this.isPointerInside) && (data.pointerPress == base.gameObject))) || ((!this.isPointerDown && this.isPointerInside) && (data.pointerPress == null))));
            }
            return (hasSelection | this.isPointerInside);
        }

        public virtual bool IsInteractable()
        {
            return (this.m_GroupsAllowInteraction && this.m_Interactable);
        }

        protected bool IsPressed()
        {
            if (!this.IsActive())
            {
                return false;
            }
            return (this.isPointerInside && this.isPointerDown);
        }

        [Obsolete("Is Pressed no longer requires eventData", false)]
        protected bool IsPressed(BaseEventData eventData)
        {
            return this.IsPressed();
        }

        private void Navigate(AxisEventData eventData, Selectable sel)
        {
            if ((sel != null) && sel.IsActive())
            {
                eventData.selectedObject = sel.gameObject;
            }
        }

        protected override void OnCanvasGroupChanged()
        {
            bool flag = true;
            for (Transform transform = base.transform; transform != null; transform = transform.parent)
            {
                transform.GetComponents<CanvasGroup>(this.m_CanvasGroupCache);
                bool flag2 = false;
                for (int i = 0; i < this.m_CanvasGroupCache.Count; i++)
                {
                    if (!this.m_CanvasGroupCache[i].interactable)
                    {
                        flag = false;
                        flag2 = true;
                    }
                    if (this.m_CanvasGroupCache[i].ignoreParentGroups)
                    {
                        flag2 = true;
                    }
                }
                if (flag2)
                {
                    break;
                }
            }
            if (flag != this.m_GroupsAllowInteraction)
            {
                this.m_GroupsAllowInteraction = flag;
                this.OnSetProperty();
            }
        }

        public virtual void OnDeselect(BaseEventData eventData)
        {
            this.hasSelection = false;
            this.EvaluateAndTransitionToSelectionState(eventData);
        }

        protected override void OnDidApplyAnimationProperties()
        {
            this.OnSetProperty();
        }

        protected override void OnDisable()
        {
            s_List.Remove(this);
            this.InstantClearState();
            base.OnDisable();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            s_List.Add(this);
            SelectionState normal = SelectionState.Normal;
            if (this.hasSelection)
            {
                normal = SelectionState.Highlighted;
            }
            this.m_CurrentSelectionState = normal;
            this.InternalEvaluateAndTransitionToSelectionState(true);
        }

        public virtual void OnMove(AxisEventData eventData)
        {
            switch (eventData.moveDir)
            {
                case MoveDirection.Left:
                    this.Navigate(eventData, this.FindSelectableOnLeft());
                    break;

                case MoveDirection.Up:
                    this.Navigate(eventData, this.FindSelectableOnUp());
                    break;

                case MoveDirection.Right:
                    this.Navigate(eventData, this.FindSelectableOnRight());
                    break;

                case MoveDirection.Down:
                    this.Navigate(eventData, this.FindSelectableOnDown());
                    break;
            }
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (this.IsInteractable() && (this.navigation.mode != Navigation.Mode.None))
                {
                    EventSystem.current.SetSelectedGameObject(base.gameObject, eventData);
                }
                this.isPointerDown = true;
                this.EvaluateAndTransitionToSelectionState(eventData);
            }
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            this.isPointerInside = true;
            this.EvaluateAndTransitionToSelectionState(eventData);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            this.isPointerInside = false;
            this.EvaluateAndTransitionToSelectionState(eventData);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                this.isPointerDown = false;
                this.EvaluateAndTransitionToSelectionState(eventData);
            }
        }

        public virtual void OnSelect(BaseEventData eventData)
        {
            this.hasSelection = true;
            this.EvaluateAndTransitionToSelectionState(eventData);
        }

        private void OnSetProperty()
        {
            if (!Application.isPlaying)
            {
                this.InternalEvaluateAndTransitionToSelectionState(true);
            }
            else
            {
                this.InternalEvaluateAndTransitionToSelectionState(false);
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            this.m_Colors.fadeDuration = Mathf.Max(this.m_Colors.fadeDuration, 0f);
            if (base.isActiveAndEnabled)
            {
                this.DoSpriteSwap(null);
                this.StartColorTween(Color.white, true);
                this.TriggerAnimation(this.m_AnimationTriggers.normalTrigger);
                this.InternalEvaluateAndTransitionToSelectionState(true);
            }
        }

        protected override void Reset()
        {
            this.m_TargetGraphic = base.GetComponent<Graphic>();
        }

        public virtual void Select()
        {
            if (!EventSystem.current.alreadySelecting)
            {
                EventSystem.current.SetSelectedGameObject(base.gameObject);
            }
        }

        private void StartColorTween(Color targetColor, bool instant)
        {
            if (this.m_TargetGraphic != null)
            {
                this.m_TargetGraphic.CrossFadeColor(targetColor, !instant ? this.m_Colors.fadeDuration : 0f, true, true);
            }
        }

        private void TriggerAnimation(string triggername)
        {
            if (((this.animator != null) && this.animator.isActiveAndEnabled) && ((this.animator.runtimeAnimatorController != null) && !string.IsNullOrEmpty(triggername)))
            {
                this.animator.ResetTrigger(this.m_AnimationTriggers.normalTrigger);
                this.animator.ResetTrigger(this.m_AnimationTriggers.pressedTrigger);
                this.animator.ResetTrigger(this.m_AnimationTriggers.highlightedTrigger);
                this.animator.ResetTrigger(this.m_AnimationTriggers.disabledTrigger);
                this.animator.SetTrigger(triggername);
            }
        }

        protected void UpdateSelectionState(BaseEventData eventData)
        {
            if (this.IsPressed())
            {
                this.m_CurrentSelectionState = SelectionState.Pressed;
            }
            else if (this.IsHighlighted(eventData))
            {
                this.m_CurrentSelectionState = SelectionState.Highlighted;
            }
            else
            {
                this.m_CurrentSelectionState = SelectionState.Normal;
            }
        }

        public static List<Selectable> allSelectables
        {
            get
            {
                return s_List;
            }
        }

        public AnimationTriggers animationTriggers
        {
            get
            {
                return this.m_AnimationTriggers;
            }
            set
            {
                if (SetPropertyUtility.SetClass<AnimationTriggers>(ref this.m_AnimationTriggers, value))
                {
                    this.OnSetProperty();
                }
            }
        }

        public Animator animator
        {
            get
            {
                return base.GetComponent<Animator>();
            }
        }

        public ColorBlock colors
        {
            get
            {
                return this.m_Colors;
            }
            set
            {
                if (SetPropertyUtility.SetStruct<ColorBlock>(ref this.m_Colors, value))
                {
                    this.OnSetProperty();
                }
            }
        }

        protected SelectionState currentSelectionState
        {
            get
            {
                return this.m_CurrentSelectionState;
            }
        }

        private bool hasSelection { get; set; }

        public Image image
        {
            get
            {
                return (this.m_TargetGraphic as Image);
            }
            set
            {
                this.m_TargetGraphic = value;
            }
        }

        public bool interactable
        {
            get
            {
                return this.m_Interactable;
            }
            set
            {
                if (SetPropertyUtility.SetStruct<bool>(ref this.m_Interactable, value))
                {
                    this.OnSetProperty();
                }
            }
        }

        private bool isPointerDown { get; set; }

        private bool isPointerInside { get; set; }

        public Navigation navigation
        {
            get
            {
                return this.m_Navigation;
            }
            set
            {
                if (SetPropertyUtility.SetStruct<Navigation>(ref this.m_Navigation, value))
                {
                    this.OnSetProperty();
                }
            }
        }

        public SpriteState spriteState
        {
            get
            {
                return this.m_SpriteState;
            }
            set
            {
                if (SetPropertyUtility.SetStruct<SpriteState>(ref this.m_SpriteState, value))
                {
                    this.OnSetProperty();
                }
            }
        }

        public Graphic targetGraphic
        {
            get
            {
                return this.m_TargetGraphic;
            }
            set
            {
                if (SetPropertyUtility.SetClass<Graphic>(ref this.m_TargetGraphic, value))
                {
                    this.OnSetProperty();
                }
            }
        }

        public Transition transition
        {
            get
            {
                return this.m_Transition;
            }
            set
            {
                if (SetPropertyUtility.SetStruct<Transition>(ref this.m_Transition, value))
                {
                    this.OnSetProperty();
                }
            }
        }

        protected enum SelectionState
        {
            Normal,
            Highlighted,
            Pressed,
            Disabled
        }

        public enum Transition
        {
            None,
            ColorTint,
            SpriteSwap,
            Animation
        }
    }
}

