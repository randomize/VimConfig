namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class RotationModuleUI : ModuleUI
    {
        private SerializedMinMaxCurve m_Curve;
        private static Texts s_Texts;

        public RotationModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "RotationModule", displayName)
        {
            base.m_ToolTip = "Controls the angular velocity of each particle during its lifetime.";
        }

        protected override void Init()
        {
            if (this.m_Curve == null)
            {
                if (s_Texts == null)
                {
                    s_Texts = new Texts();
                }
                this.m_Curve = new SerializedMinMaxCurve(this, s_Texts.rotation, ModuleUI.kUseSignedRange);
                this.m_Curve.m_RemapValue = 57.29578f;
            }
        }

        public override void OnInspectorGUI(ParticleSystem s)
        {
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            ModuleUI.GUIMinMaxCurve(s_Texts.rotation, this.m_Curve);
        }

        public override void UpdateCullingSupportedString(ref string text)
        {
            this.Init();
            if (!this.m_Curve.SupportsProcedural())
            {
                text = text + "\n\tLifetime rotation curve uses too many keys.";
            }
        }

        private class Texts
        {
            public GUIContent rotation = new GUIContent("Angular Velocity", "Controls the angular velocity of each particle during its lifetime.");
        }
    }
}

