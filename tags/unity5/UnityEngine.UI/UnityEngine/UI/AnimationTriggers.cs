namespace UnityEngine.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.Serialization;

    [Serializable]
    public class AnimationTriggers
    {
        private const string kDefaultDisabledAnimName = "Disabled";
        private const string kDefaultNormalAnimName = "Normal";
        private const string kDefaultPressedAnimName = "Pressed";
        private const string kDefaultSelectedAnimName = "Highlighted";
        [SerializeField, FormerlySerializedAs("disabledTrigger")]
        private string m_DisabledTrigger = "Disabled";
        [SerializeField, FormerlySerializedAs("highlightedTrigger"), FormerlySerializedAs("m_SelectedTrigger")]
        private string m_HighlightedTrigger = "Highlighted";
        [FormerlySerializedAs("normalTrigger"), SerializeField]
        private string m_NormalTrigger = "Normal";
        [FormerlySerializedAs("pressedTrigger"), SerializeField]
        private string m_PressedTrigger = "Pressed";

        public string disabledTrigger
        {
            get
            {
                return this.m_DisabledTrigger;
            }
            set
            {
                this.m_DisabledTrigger = value;
            }
        }

        public string highlightedTrigger
        {
            get
            {
                return this.m_HighlightedTrigger;
            }
            set
            {
                this.m_HighlightedTrigger = value;
            }
        }

        public string normalTrigger
        {
            get
            {
                return this.m_NormalTrigger;
            }
            set
            {
                this.m_NormalTrigger = value;
            }
        }

        public string pressedTrigger
        {
            get
            {
                return this.m_PressedTrigger;
            }
            set
            {
                this.m_PressedTrigger = value;
            }
        }
    }
}

