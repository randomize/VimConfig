namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class Collider2DEditorBase : ColliderEditorBase
    {
        protected void BeginColliderInspector()
        {
            base.serializedObject.Update();
            EditorGUI.BeginDisabledGroup(base.targets.Length > 1);
            base.InspectorEditButtonGUI();
            EditorGUI.EndDisabledGroup();
        }

        protected void CheckColliderErrorState()
        {
            switch ((this.target as Collider2D).errorState)
            {
                case ColliderErrorState2D.NoShapes:
                    EditorGUILayout.HelpBox("The collider did not create any collision shapes as they all failed verification.  This could be because they were deemed too small or the vertices were too close.  Vertices can also become close under certain rotations or very small scaling.", MessageType.Warning);
                    break;

                case ColliderErrorState2D.RemovedShapes:
                    EditorGUILayout.HelpBox("The collider created collision shape(s) but some were removed as they failed verification.  This could be because they were deemed too small or the vertices were too close.  Vertices can also become close under certain rotations or very small scaling.", MessageType.Warning);
                    break;
            }
        }

        protected void EndColliderInspector()
        {
            base.serializedObject.ApplyModifiedProperties();
        }

        internal override void OnForceReloadInspector()
        {
            base.OnForceReloadInspector();
            if (base.editingCollider)
            {
                base.ForceQuitEditMode();
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            this.CheckColliderErrorState();
            Effector2DEditor.CheckEffectorWarnings(this.target as Collider2D);
        }
    }
}

