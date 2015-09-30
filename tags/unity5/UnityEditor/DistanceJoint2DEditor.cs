namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomEditor(typeof(DistanceJoint2D)), CanEditMultipleObjects]
    internal class DistanceJoint2DEditor : AnchoredJoint2DEditor
    {
        public void OnSceneGUI()
        {
            DistanceJoint2D target = (DistanceJoint2D) this.target;
            if (target.enabled)
            {
                Vector3 anchor = Joint2DEditorBase.TransformPoint(target.transform, (Vector3) target.anchor);
                Vector3 connectedAnchor = (Vector3) target.connectedAnchor;
                if (target.connectedBody != null)
                {
                    connectedAnchor = Joint2DEditorBase.TransformPoint(target.connectedBody.transform, connectedAnchor);
                }
                Joint2DEditorBase.DrawDistanceGizmo(anchor, connectedAnchor, target.distance);
                base.OnSceneGUI();
            }
        }
    }
}

