namespace UnityEngine.UI
{
    using System;
    using UnityEngine;

    public interface ICanvasElement
    {
        bool IsDestroyed();
        void Rebuild(CanvasUpdate executing);

        Transform transform { get; }
    }
}

