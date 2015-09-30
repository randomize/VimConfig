namespace UnityEngine.UI
{
    using System;
    using UnityEngine;

    public interface IMeshModifier
    {
        void ModifyMesh(Mesh verts);
    }
}

