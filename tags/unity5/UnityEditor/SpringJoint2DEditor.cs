namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(SpringJoint2D))]
    internal class SpringJoint2DEditor : AnchoredJoint2DEditor
    {
        public void OnSceneGUI()
        {
            SpringJoint2D target = (SpringJoint2D) this.target;
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

