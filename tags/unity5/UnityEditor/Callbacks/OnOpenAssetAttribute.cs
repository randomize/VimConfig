namespace UnityEditor.Callbacks
{
    using System;
    using UnityEditor;

    public sealed class OnOpenAssetAttribute : CallbackOrderAttribute
    {
        public OnOpenAssetAttribute()
        {
            base.m_CallbackOrder = 1;
        }

        public OnOpenAssetAttribute(int callbackOrder)
        {
            base.m_CallbackOrder = callbackOrder;
        }
    }
}

