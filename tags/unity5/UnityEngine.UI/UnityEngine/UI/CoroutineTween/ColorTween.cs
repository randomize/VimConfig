namespace UnityEngine.UI.CoroutineTween
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;

    [StructLayout(LayoutKind.Sequential)]
    internal struct ColorTween : ITweenValue
    {
        private ColorTweenCallback m_Target;
        private Color m_StartColor;
        private Color m_TargetColor;
        private ColorTweenMode m_TweenMode;
        private float m_Duration;
        private bool m_IgnoreTimeScale;
        public Color startColor
        {
            get
            {
                return this.m_StartColor;
            }
            set
            {
                this.m_StartColor = value;
            }
        }
        public Color targetColor
        {
            get
            {
                return this.m_TargetColor;
            }
            set
            {
                this.m_TargetColor = value;
            }
        }
        public ColorTweenMode tweenMode
        {
            get
            {
                return this.m_TweenMode;
            }
            set
            {
                this.m_TweenMode = value;
            }
        }
        public float duration
        {
            get
            {
                return this.m_Duration;
            }
            set
            {
                this.m_Duration = value;
            }
        }
        public bool ignoreTimeScale
        {
            get
            {
                return this.m_IgnoreTimeScale;
            }
            set
            {
                this.m_IgnoreTimeScale = value;
            }
        }
        public void TweenValue(float floatPercentage)
        {
            if (this.ValidTarget())
            {
                Color color = Color.Lerp(this.m_StartColor, this.m_TargetColor, floatPercentage);
                if (this.m_TweenMode == ColorTweenMode.Alpha)
                {
                    color.r = this.m_StartColor.r;
                    color.g = this.m_StartColor.g;
                    color.b = this.m_StartColor.b;
                }
                else if (this.m_TweenMode == ColorTweenMode.RGB)
                {
                    color.a = this.m_StartColor.a;
                }
                this.m_Target.Invoke(color);
            }
        }

        public void AddOnChangedCallback(UnityAction<Color> callback)
        {
            if (this.m_Target == null)
            {
                this.m_Target = new ColorTweenCallback();
            }
            this.m_Target.AddListener(callback);
        }

        public bool GetIgnoreTimescale()
        {
            return this.m_IgnoreTimeScale;
        }

        public float GetDuration()
        {
            return this.m_Duration;
        }

        public bool ValidTarget()
        {
            return (this.m_Target != null);
        }
        public class ColorTweenCallback : UnityEvent<Color>
        {
        }

        public enum ColorTweenMode
        {
            All,
            RGB,
            Alpha
        }
    }
}

