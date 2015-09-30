namespace UnityEditor
{
    using System;
    using UnityEngine;

    [Serializable]
    internal class TransformRotationGUI
    {
        private Vector3 m_EulerAngles;
        private Quaternion m_OldQuaternion = new Quaternion(1234f, 1000f, 4321f, -1000f);
        private SerializedProperty m_Rotation;
        private GUIContent rotationContent = new GUIContent("Rotation", "The local rotation of this Game Object relative to the parent.");
        private static int s_FoldoutHash = "Foldout".GetHashCode();
        private UnityEngine.Object[] targets;

        public void OnEnable(SerializedProperty m_Rotation, GUIContent label)
        {
            this.m_Rotation = m_Rotation;
            this.targets = m_Rotation.serializedObject.targetObjects;
            this.rotationContent = label;
        }

        public void RotationField()
        {
            this.RotationField(false);
        }

        public void RotationField(bool disabled)
        {
            Transform transform = this.targets[0] as Transform;
            Quaternion localRotation = transform.localRotation;
            if (((this.m_OldQuaternion.x != localRotation.x) || (this.m_OldQuaternion.y != localRotation.y)) || ((this.m_OldQuaternion.z != localRotation.z) || (this.m_OldQuaternion.w != localRotation.w)))
            {
                this.m_EulerAngles = transform.localEulerAngles;
                this.m_OldQuaternion = localRotation;
            }
            bool flag = false;
            for (int i = 1; i < this.targets.Length; i++)
            {
                Quaternion quaternion2 = (this.targets[i] as Transform).localRotation;
                flag |= (((quaternion2.x != localRotation.x) || (quaternion2.y != localRotation.y)) || (quaternion2.z != localRotation.z)) || (quaternion2.w != localRotation.w);
            }
            Rect totalPosition = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight * (!EditorGUIUtility.wideMode ? ((float) 2) : ((float) 1)), new GUILayoutOption[0]);
            GUIContent label = EditorGUI.BeginProperty(totalPosition, this.rotationContent, this.m_Rotation);
            EditorGUI.showMixedValue = flag;
            EditorGUI.BeginChangeCheck();
            int id = GUIUtility.GetControlID(s_FoldoutHash, EditorGUIUtility.native, totalPosition);
            totalPosition = EditorGUI.MultiFieldPrefixLabel(totalPosition, id, label, 3);
            totalPosition.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.BeginDisabledGroup(disabled);
            this.m_EulerAngles = EditorGUI.Vector3Field(totalPosition, GUIContent.none, this.m_EulerAngles);
            EditorGUI.EndDisabledGroup();
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(this.targets, "Inspector");
                foreach (Transform transform2 in this.targets)
                {
                    transform2.localEulerAngles = this.m_EulerAngles;
                    if (transform2.parent != null)
                    {
                        transform2.SendTransformChangedScale();
                    }
                }
                this.m_Rotation.serializedObject.SetIsDifferentCacheDirty();
                this.m_OldQuaternion = localRotation;
            }
            EditorGUI.showMixedValue = false;
            EditorGUI.EndProperty();
        }
    }
}

