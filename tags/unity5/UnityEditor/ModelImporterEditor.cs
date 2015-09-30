namespace UnityEditor
{
    using System;

    [CustomEditor(typeof(ModelImporter)), CanEditMultipleObjects]
    internal class ModelImporterEditor : AssetImporterTabbedEditor
    {
        public override bool HasPreviewGUI()
        {
            return (base.HasPreviewGUI() && (base.targets.Length < 2));
        }

        internal override void OnEnable()
        {
            if (base.m_SubEditorTypes == null)
            {
                base.m_SubEditorTypes = new System.Type[] { typeof(ModelImporterModelEditor), typeof(ModelImporterRigEditor), typeof(ModelImporterClipEditor) };
                base.m_SubEditorNames = new string[] { "Model", "Rig", "Animations" };
            }
            base.OnEnable();
        }

        internal override bool showImportedObject
        {
            get
            {
                return (base.activeEditor is ModelImporterModelEditor);
            }
        }

        protected override bool useAssetDrawPreview
        {
            get
            {
                return false;
            }
        }
    }
}

