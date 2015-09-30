namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class RotationByVelocityModuleUI : ModuleUI
    {
        private SerializedMinMaxCurve m_Curve;
        private SerializedProperty m_Range;
        private static Texts s_Texts;

        public RotationByVelocityModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "RotationBySpeedModule", displayName)
        {
            base.m_ToolTip = "Controls the angular velocity of each particle based on its speed.";
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
                this.m_Range = base.GetProperty("range");
            }
        }

        public override void OnInspectorGUI(ParticleSystem s)
        {
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            ModuleUI.GUIMinMaxCurve(s_Texts.rotation, this.m_Curve);
            ModuleUI.GUIMinMaxRange(s_Texts.velocityRange, this.m_Range);
        }

        public override void UpdateCullingSupportedString(ref string text)
        {
            text = text + "\n\tRotation by Speed is enabled.";
        }

        private class Texts
        {
            public GUIContent rotation = new GUIContent("Angular Velocity", "Controls the angular velocity of each particle based on its speed.");
            public GUIContent velocityRange = new GUIContent("Speed Range", "Remaps speed in the defined range to an angular velocity.");
        }
    }
}

