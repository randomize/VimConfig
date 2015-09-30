namespace UnityEngine.UI
{
    using UnityEngine;

    public interface IMaterialModifier
    {
        Material GetModifiedMaterial(Material baseMaterial);
    }
}

