namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(MeshRenderer))]
    internal class MeshRendererEditor : RendererEditorBase
    {
        private string[] m_ExcludedProperties;

        private static void DisplayMaterialWarning(SerializedObject obj, SerializedProperty property)
        {
            MeshFilter component = ((MeshRenderer) obj.targetObject).GetComponent<MeshFilter>();
            if (((component != null) && (component.sharedMesh != null)) && (property.arraySize > component.sharedMesh.subMeshCount))
            {
                EditorGUILayout.HelpBox("This renderer has more materials than the Mesh has submeshes. Multiple materials will be applied to the same submesh, which costs performance. Consider using multiple shader passes.", MessageType.Warning, true);
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            base.InitializeProbeFields();
            List<string> list = new List<string> { "m_LightmapParameters" };
            list.AddRange(RendererEditorBase.Probes.GetFieldsStringArray());
            this.m_ExcludedProperties = list.ToArray();
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            Editor.DrawPropertiesExcluding(base.serializedObject, this.m_ExcludedProperties);
            SerializedProperty property = base.serializedObject.FindProperty("m_Materials");
            if (!property.hasMultipleDifferentValues)
            {
                DisplayMaterialWarning(base.serializedObject, property);
            }
            base.RenderProbeFields();
            base.serializedObject.ApplyModifiedProperties();
        }
    }
}

