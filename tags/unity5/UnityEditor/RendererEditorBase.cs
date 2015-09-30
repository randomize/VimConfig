namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Rendering;

    internal class RendererEditorBase : Editor
    {
        protected Probes m_Probes;
        private SerializedProperty m_SortingLayerID;
        private GUIContent m_SortingLayerStyle = EditorGUIUtility.TextContent("Sorting Layer");
        private SerializedProperty m_SortingOrder;
        private GUIContent m_SortingOrderStyle = EditorGUIUtility.TextContent("Order in Layer");

        protected void InitializeProbeFields()
        {
            this.m_Probes = new Probes();
            this.m_Probes.Initialize(base.serializedObject);
        }

        public virtual void OnEnable()
        {
            this.m_SortingOrder = base.serializedObject.FindProperty("m_SortingOrder");
            this.m_SortingLayerID = base.serializedObject.FindProperty("m_SortingLayerID");
        }

        protected void RenderProbeFields()
        {
            this.m_Probes.OnGUI((Renderer) this.target, false);
        }

        protected void RenderSortingLayerFields()
        {
            EditorGUILayout.Space();
            EditorGUILayout.SortingLayerField(this.m_SortingLayerStyle, this.m_SortingLayerID, EditorStyles.popup, EditorStyles.label);
            EditorGUILayout.PropertyField(this.m_SortingOrder, this.m_SortingOrderStyle, new GUILayoutOption[0]);
        }

        internal class Probes
        {
            private List<ReflectionProbeBlendInfo> m_BlendInfo = new List<ReflectionProbeBlendInfo>();
            private GUIContent m_DeferredNote = EditorGUIUtility.TextContent("In Deferred Shading, all objects receive shadows and get per-pixel reflection probes.");
            internal SerializedProperty m_ProbeAnchor;
            private GUIContent m_ProbeAnchorStyle = EditorGUIUtility.TextContent("Anchor Override|If set, the Renderer will use this Transform's position to sample light probes and find the matching reflection probe.");
            internal SerializedProperty m_ReflectionProbeUsage;
            private GUIContent m_ReflectionProbeUsageStyle = EditorGUIUtility.TextContent("Reflection Probes");
            internal SerializedProperty m_UseLightProbes;
            private GUIContent m_UseLightProbesStyle = EditorGUIUtility.TextContent("Use Light Probes");

            internal static string[] GetFieldsStringArray()
            {
                return new string[] { "m_UseLightProbes", "m_ReflectionProbeUsage", "m_ProbeAnchor" };
            }

            internal void Initialize(SerializedObject serializedObject)
            {
                this.Initialize(serializedObject, true);
            }

            internal void Initialize(SerializedObject serializedObject, bool initializeLightProbes)
            {
                if (initializeLightProbes)
                {
                    this.m_UseLightProbes = serializedObject.FindProperty("m_UseLightProbes");
                }
                this.m_ReflectionProbeUsage = serializedObject.FindProperty("m_ReflectionProbeUsage");
                this.m_ProbeAnchor = serializedObject.FindProperty("m_ProbeAnchor");
            }

            internal void OnGUI(Renderer renderer, bool useMiniStyle)
            {
                if (this.m_UseLightProbes != null)
                {
                    EditorGUI.BeginDisabledGroup(LightmapEditorSettings.IsLightmappedOrDynamicLightmappedForRendering(renderer));
                    if (!useMiniStyle)
                    {
                        EditorGUILayout.PropertyField(this.m_UseLightProbes, this.m_UseLightProbesStyle, new GUILayoutOption[0]);
                    }
                    else
                    {
                        ModuleUI.GUIToggle(this.m_UseLightProbesStyle, this.m_UseLightProbes);
                    }
                    EditorGUI.EndDisabledGroup();
                }
                if (!useMiniStyle)
                {
                    this.m_ReflectionProbeUsage.intValue = (int) ((ReflectionProbeUsage) EditorGUILayout.EnumPopup(this.m_ReflectionProbeUsageStyle, (ReflectionProbeUsage) this.m_ReflectionProbeUsage.intValue, new GUILayoutOption[0]));
                }
                else
                {
                    ModuleUI.GUIPopup(this.m_ReflectionProbeUsageStyle, this.m_ReflectionProbeUsage, Enum.GetNames(typeof(ReflectionProbeUsage)));
                }
                bool flag = this.m_ReflectionProbeUsage.intValue != 0;
                if (((this.m_UseLightProbes != null) && this.m_UseLightProbes.boolValue) || flag)
                {
                    EditorGUI.indentLevel++;
                    if (!useMiniStyle)
                    {
                        EditorGUILayout.PropertyField(this.m_ProbeAnchor, this.m_ProbeAnchorStyle, new GUILayoutOption[0]);
                    }
                    else
                    {
                        ModuleUI.GUIObject(this.m_ProbeAnchorStyle, this.m_ProbeAnchor);
                    }
                    if (flag)
                    {
                        if (SceneView.GetSceneViewRenderingPath() == RenderingPath.DeferredShading)
                        {
                            EditorGUILayout.HelpBox(this.m_DeferredNote.text, MessageType.Info);
                        }
                        else
                        {
                            renderer.GetClosestReflectionProbes(this.m_BlendInfo);
                            ShowClosestReflectionProbes(this.m_BlendInfo);
                        }
                    }
                    EditorGUI.indentLevel--;
                }
            }

            internal static void ShowClosestReflectionProbes(List<ReflectionProbeBlendInfo> blendInfos)
            {
                float num = 20f;
                float num2 = 60f;
                EditorGUI.BeginDisabledGroup(true);
                for (int i = 0; i < blendInfos.Count; i++)
                {
                    Rect rect = EditorGUI.IndentedRect(GUILayoutUtility.GetRect((float) 0f, (float) 16f));
                    float num4 = (rect.width - num) - num2;
                    Rect position = rect;
                    position.width = num;
                    GUI.Label(position, "#" + i, EditorStyles.miniLabel);
                    position.x += position.width;
                    position.width = num4;
                    ReflectionProbeBlendInfo info = blendInfos[i];
                    EditorGUI.ObjectField(position, info.probe, typeof(ReflectionProbe), true);
                    position.x += position.width;
                    position.width = num2;
                    ReflectionProbeBlendInfo info2 = blendInfos[i];
                    GUI.Label(position, "Weight " + info2.weight.ToString("f2"), EditorStyles.miniLabel);
                }
                EditorGUI.EndDisabledGroup();
            }
        }
    }
}

