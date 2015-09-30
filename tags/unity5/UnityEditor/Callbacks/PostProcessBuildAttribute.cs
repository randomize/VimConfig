namespace UnityEditor.Callbacks
{
    using System;
    using UnityEditor;

    public sealed class PostProcessBuildAttribute : CallbackOrderAttribute
    {
        public PostProcessBuildAttribute()
        {
            base.m_CallbackOrder = 1;
        }

        public PostProcessBuildAttribute(int callbackOrder)
        {
            base.m_CallbackOrder = callbackOrder;
        }
    }
}

