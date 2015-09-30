namespace UnityEngine.UI.CoroutineTween
{
    using System;

    internal interface ITweenValue
    {
        void TweenValue(float floatPercentage);
        bool ValidTarget();

        float duration { get; }

        bool ignoreTimeScale { get; }
    }
}

