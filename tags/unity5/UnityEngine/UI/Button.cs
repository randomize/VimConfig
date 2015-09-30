namespace UnityEngine.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.Serialization;

    [AddComponentMenu("UI/Button", 30)]
    public class Button : Selectable, IEventSystemHandler, IPointerClickHandler, ISubmitHandler
    {
        [SerializeField, FormerlySerializedAs("onClick")]
        private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();

        protected Button()
        {
        }

        [DebuggerHidden]
        private IEnumerator OnFinishSubmit()
        {
            return new <OnFinishSubmit>c__Iterator1 { <>f__this = this };
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                this.Press();
            }
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            this.Press();
            if (this.IsActive() && this.IsInteractable())
            {
                this.DoStateTransition(Selectable.SelectionState.Pressed, false);
                base.StartCoroutine(this.OnFinishSubmit());
            }
        }

        private void Press()
        {
            if (this.IsActive() && this.IsInteractable())
            {
                this.m_OnClick.Invoke();
            }
        }

        public ButtonClickedEvent onClick
        {
            get
            {
                return this.m_OnClick;
            }
            set
            {
                this.m_OnClick = value;
            }
        }

        [CompilerGenerated]
        private sealed class <OnFinishSubmit>c__Iterator1 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Button <>f__this;
            internal float <elapsedTime>__1;
            internal float <fadeTime>__0;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<fadeTime>__0 = this.<>f__this.colors.fadeDuration;
                        this.<elapsedTime>__1 = 0f;
                        break;

                    case 1:
                        break;

                    default:
                        goto Label_009E;
                }
                if (this.<elapsedTime>__1 < this.<fadeTime>__0)
                {
                    this.<elapsedTime>__1 += Time.unscaledDeltaTime;
                    this.$current = null;
                    this.$PC = 1;
                    return true;
                }
                this.<>f__this.DoStateTransition(this.<>f__this.currentSelectionState, false);
                this.$PC = -1;
            Label_009E:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [Serializable]
        public class ButtonClickedEvent : UnityEvent
        {
        }
    }
}

