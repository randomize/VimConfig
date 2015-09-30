namespace UnityEngine.UI
{
    using System;
    using UnityEngine;

    [Obsolete("Not supported anymore.", true)]
    public interface IMask
    {
        bool Enabled();

        RectTransform rectTransform { get; }
    }
}

