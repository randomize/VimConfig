namespace UnityEngine.UI
{
    using System;
    using System.Collections.Generic;

    [Obsolete("Use BaseMeshEffect instead", true)]
    public abstract class BaseVertexEffect
    {
        protected BaseVertexEffect()
        {
        }

        [Obsolete("Use BaseMeshEffect.ModifyMeshes instead", true)]
        public abstract void ModifyVertices(List<UIVertex> vertices);
    }
}

