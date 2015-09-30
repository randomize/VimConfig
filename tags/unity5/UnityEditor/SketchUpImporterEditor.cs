namespace UnityEditor
{
    using System;

    [CanEditMultipleObjects, CustomEditor(typeof(SketchUpImporter))]
    internal class SketchUpImporterEditor : ModelImporterEditor
    {
        internal override void OnEnable()
        {
            if (base.m_SubEditorTypes == null)
            {
                base.m_SubEditorTypes = new System.Type[] { typeof(SketchUpImporterModelEditor), typeof(ModelImporterRigEditor), typeof(ModelImporterClipEditor) };
                base.m_SubEditorNames = new string[] { "Sketch Up", "Rig", "Animations" };
            }
            base.OnEnable();
        }

        internal override bool showImportedObject
        {
            get
            {
                return (base.activeEditor is SketchUpImporterModelEditor);
            }
        }
    }
}

