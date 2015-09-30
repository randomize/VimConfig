namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEditorInternal;
    using UnityEngine;

    internal class FrameDebuggerWindow : EditorWindow
    {
        private const float kDetailsMargin = 4f;
        private const float kMinDetailsWidth = 200f;
        private const float kMinListWidth = 200f;
        private const float kMinPreviewSize = 64f;
        private const float kMinWindowWidth = 240f;
        private const int kNeedToRepaintFrames = 4;
        private const float kResizerWidth = 5f;
        private const float kScrollbarWidth = 16f;
        [NonSerialized]
        private int m_FrameEventsHash;
        [SerializeField]
        private float m_ListWidth = 300f;
        private Material m_Material;
        private int m_PrevEventsCount;
        private int m_PrevEventsLimit;
        public Vector2 m_PreviewDir = new Vector2(120f, -20f);
        private PreviewRenderUtility m_PreviewUtility;
        private int m_RepaintFrames = 4;
        [NonSerialized]
        private float m_RTBlackLevel;
        [NonSerialized]
        private int m_RTChannel;
        [NonSerialized]
        private int m_RTIndex;
        [NonSerialized]
        private float m_RTWhiteLevel = 1f;
        [NonSerialized]
        private FrameDebuggerTreeView m_Tree;
        [SerializeField]
        private TreeViewState m_TreeViewState;
        private Material m_WireMaterial;
        private static Styles ms_Styles;
        private static List<FrameDebuggerWindow> s_FrameDebuggers = new List<FrameDebuggerWindow>();
        public static readonly string[] s_FrameEventTypeNames = new string[] { 
            "Clear (nothing)", "Clear (color)", "Clear (Z)", "Clear (color+Z)", "Clear (stencil)", "Clear (color+stencil)", "Clear (Z+stencil)", "Clear (color+Z+stencil)", "SetRenderTarget", "Resolve Color", "Resolve Depth", "Grab RenderTexture", "Static Batch", "Dynamic Batch", "Draw Mesh", "Draw Dynamic", 
            "Draw GL", "GPU Skinning", "Draw Procedural", "Compute Shader", "Plugin Event"
         };

        public FrameDebuggerWindow()
        {
            base.position = new Rect(50f, 50f, 600f, 350f);
            base.minSize = new Vector2(400f, 200f);
        }

        internal void ChangeFrameEventLimit(int newLimit)
        {
            if ((newLimit > 0) && (newLimit <= FrameDebuggerUtility.count))
            {
                if ((newLimit != FrameDebuggerUtility.limit) && (newLimit > 0))
                {
                    GameObject gameObjectForEvent = GetGameObjectForEvent(newLimit - 1);
                    if (gameObjectForEvent != null)
                    {
                        EditorGUIUtility.PingObject(gameObjectForEvent);
                    }
                }
                FrameDebuggerUtility.limit = newLimit;
                if (this.m_Tree != null)
                {
                    this.m_Tree.SelectFrameEventIndex(newLimit);
                }
            }
        }

        private void ClickEnableFrameDebugger()
        {
            if (GraphicsSupportsFrameDebugger())
            {
                if ((!FrameDebuggerUtility.enabled && EditorApplication.isPlaying) && !EditorApplication.isPaused)
                {
                    EditorApplication.isPaused = true;
                }
                FrameDebuggerUtility.enabled = !FrameDebuggerUtility.enabled;
                if (FrameDebuggerUtility.enabled)
                {
                    GameView view = (GameView) WindowLayout.FindEditorWindowOfType(typeof(GameView));
                    if (view != null)
                    {
                        view.ShowTab();
                    }
                }
                this.m_PrevEventsLimit = FrameDebuggerUtility.limit;
                this.m_PrevEventsCount = FrameDebuggerUtility.count;
            }
        }

        private static void DisableFrameDebugger()
        {
            if (FrameDebuggerUtility.enabled)
            {
                EditorApplication.SetSceneRepaintDirty();
            }
            FrameDebuggerUtility.enabled = false;
        }

        private void DrawCurrentEvent(Rect rect, FrameDebuggerEvent[] descs)
        {
            int index = FrameDebuggerUtility.limit - 1;
            if ((index >= 0) && (index < descs.Length))
            {
                GUILayout.BeginArea(rect);
                FrameDebuggerEvent cur = descs[index];
                this.DrawRenderTargetControls(cur);
                GUILayout.Label(string.Format("Event #{0}: {1}", index + 1, s_FrameEventTypeNames[(int) cur.type]), EditorStyles.boldLabel, new GUILayoutOption[0]);
                if ((cur.vertexCount > 0) || (cur.indexCount > 0))
                {
                    int frameEventShaderID = FrameDebuggerUtility.GetFrameEventShaderID(index);
                    if (frameEventShaderID != 0)
                    {
                        Shader shader = EditorUtility.InstanceIDToObject(frameEventShaderID) as Shader;
                        if (shader != null)
                        {
                            int frameEventShaderPassIndex = FrameDebuggerUtility.GetFrameEventShaderPassIndex(index);
                            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                            object[] objArray1 = new object[] { "Shader: ", shader.name, " pass #", frameEventShaderPassIndex };
                            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                            if (GUILayout.Button(string.Concat(objArray1), GUI.skin.label, options))
                            {
                                EditorGUIUtility.PingObject(shader);
                                Event.current.Use();
                            }
                            GUILayout.Label(FrameDebuggerUtility.GetFrameEventShaderKeywords(index), EditorStyles.miniLabel, new GUILayoutOption[0]);
                            GUILayout.EndHorizontal();
                            this.DrawStates();
                        }
                    }
                    if (!this.DrawEventMesh(index, cur))
                    {
                        GUILayout.Label("Vertices: " + cur.vertexCount, new GUILayoutOption[0]);
                        GUILayout.Label("Indices: " + cur.indexCount, new GUILayoutOption[0]);
                    }
                }
                GUILayout.EndArea();
            }
        }

        private bool DrawEventMesh(int curEventIndex, FrameDebuggerEvent curEvent)
        {
            int frameEventMeshID = FrameDebuggerUtility.GetFrameEventMeshID(curEventIndex);
            if (frameEventMeshID == 0)
            {
                return false;
            }
            Mesh mesh = EditorUtility.InstanceIDToObject(frameEventMeshID) as Mesh;
            if (mesh == null)
            {
                return false;
            }
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandHeight(true) };
            Rect position = GUILayoutUtility.GetRect((float) 10f, (float) 10f, options);
            if ((position.width >= 64f) && (position.height >= 64f))
            {
                GameObject gameObjectForEvent = GetGameObjectForEvent(curEventIndex);
                Rect meshInfoRect = position;
                meshInfoRect.yMin = meshInfoRect.yMax - (EditorGUIUtility.singleLineHeight * 2f);
                Rect rect3 = meshInfoRect;
                meshInfoRect.xMin = meshInfoRect.center.x;
                rect3.xMax = rect3.center.x;
                if (Event.current.type == EventType.MouseDown)
                {
                    if (meshInfoRect.Contains(Event.current.mousePosition))
                    {
                        EditorGUIUtility.PingObject(frameEventMeshID);
                        Event.current.Use();
                    }
                    if ((gameObjectForEvent != null) && rect3.Contains(Event.current.mousePosition))
                    {
                        EditorGUIUtility.PingObject(gameObjectForEvent.GetInstanceID());
                        Event.current.Use();
                    }
                }
                this.m_PreviewDir = PreviewGUI.Drag2D(this.m_PreviewDir, position);
                if (Event.current.type == EventType.Repaint)
                {
                    int frameEventMeshSubset = FrameDebuggerUtility.GetFrameEventMeshSubset(curEventIndex);
                    this.DrawMeshPreview(curEvent, position, meshInfoRect, mesh, frameEventMeshSubset);
                    if (gameObjectForEvent != null)
                    {
                        EditorGUI.DropShadowLabel(rect3, gameObjectForEvent.name);
                    }
                }
            }
            return true;
        }

        private void DrawEventsTree(Rect rect)
        {
            this.m_Tree.OnGUI(rect);
        }

        private void DrawMeshPreview(FrameDebuggerEvent curEvent, Rect previewRect, Rect meshInfoRect, Mesh mesh, int meshSubset)
        {
            if (this.m_PreviewUtility == null)
            {
                this.m_PreviewUtility = new PreviewRenderUtility();
                this.m_PreviewUtility.m_CameraFieldOfView = 30f;
            }
            if (this.m_Material == null)
            {
                this.m_Material = EditorGUIUtility.GetBuiltinExtraResource(typeof(Material), "Default-Material.mat") as Material;
            }
            if (this.m_WireMaterial == null)
            {
                this.m_WireMaterial = ModelInspector.CreateWireframeMaterial();
            }
            this.m_PreviewUtility.BeginPreview(previewRect, "preBackground");
            ModelInspector.RenderMeshPreview(mesh, this.m_PreviewUtility, this.m_Material, this.m_WireMaterial, this.m_PreviewDir, meshSubset);
            this.m_PreviewUtility.EndAndDrawPreview(previewRect);
            string name = mesh.name;
            if (string.IsNullOrEmpty(name))
            {
                name = "<no name>";
            }
            object[] objArray1 = new object[] { name, " subset ", meshSubset, "\n", curEvent.vertexCount, " verts, ", curEvent.indexCount, " indices" };
            string text = string.Concat(objArray1);
            EditorGUI.DropShadowLabel(meshInfoRect, text);
        }

        private void DrawRenderTargetControls(FrameDebuggerEvent cur)
        {
            if ((cur.rtWidth > 0) && (cur.rtHeight > 0))
            {
                bool disabled = (cur.rtFormat == 1) || (cur.rtFormat == 3);
                bool flag2 = cur.rtHasDepthTexture != 0;
                short rtCount = cur.rtCount;
                if (flag2)
                {
                    rtCount = (short) (rtCount + 1);
                }
                GUILayout.Label("RenderTarget: " + cur.rtName, EditorStyles.boldLabel, new GUILayoutOption[0]);
                GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
                EditorGUI.BeginChangeCheck();
                EditorGUI.BeginDisabledGroup(rtCount <= 1);
                GUIContent[] displayedOptions = new GUIContent[rtCount];
                for (int i = 0; i < cur.rtCount; i++)
                {
                    displayedOptions[i] = Styles.mrtLabels[i];
                }
                if (flag2)
                {
                    displayedOptions[cur.rtCount] = Styles.depthLabel;
                }
                int num3 = Mathf.Clamp(this.m_RTIndex, 0, rtCount - 1);
                bool flag3 = num3 != this.m_RTIndex;
                this.m_RTIndex = num3;
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(70f) };
                this.m_RTIndex = EditorGUILayout.Popup(this.m_RTIndex, displayedOptions, EditorStyles.toolbarPopup, options);
                EditorGUI.EndDisabledGroup();
                GUILayout.Space(10f);
                EditorGUI.BeginDisabledGroup(disabled);
                GUILayout.Label(Styles.channelHeader, EditorStyles.miniLabel, new GUILayoutOption[0]);
                this.m_RTChannel = GUILayout.Toolbar(this.m_RTChannel, Styles.channelLabels, EditorStyles.toolbarButton, new GUILayoutOption[0]);
                EditorGUI.EndDisabledGroup();
                GUILayout.Space(10f);
                GUILayout.Label(Styles.levelsHeader, EditorStyles.miniLabel, new GUILayoutOption[0]);
                GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.MaxWidth(200f) };
                EditorGUILayout.MinMaxSlider(ref this.m_RTBlackLevel, ref this.m_RTWhiteLevel, 0f, 1f, optionArray2);
                if (EditorGUI.EndChangeCheck() || flag3)
                {
                    Vector4 zero = Vector4.zero;
                    if (this.m_RTChannel == 1)
                    {
                        zero.x = 1f;
                    }
                    else if (this.m_RTChannel == 2)
                    {
                        zero.y = 1f;
                    }
                    else if (this.m_RTChannel == 3)
                    {
                        zero.z = 1f;
                    }
                    else if (this.m_RTChannel == 4)
                    {
                        zero.w = 1f;
                    }
                    else
                    {
                        zero = Vector4.one;
                    }
                    int rTIndex = this.m_RTIndex;
                    if (rTIndex >= cur.rtCount)
                    {
                        rTIndex = -1;
                    }
                    FrameDebuggerUtility.SetRenderTargetDisplayOptions(rTIndex, zero, this.m_RTBlackLevel, this.m_RTWhiteLevel);
                    this.RepaintAllNeededThings();
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.Label(string.Format("{0}x{1} {2}", cur.rtWidth, cur.rtHeight, (RenderTextureFormat) cur.rtFormat), new GUILayoutOption[0]);
                if (cur.rtDim == 4)
                {
                    GUILayout.Label("Rendering into cubemap", new GUILayoutOption[0]);
                }
                if ((cur.rtFormat == 3) && SystemInfo.graphicsDeviceVersion.StartsWith("Direct3D 9"))
                {
                    EditorGUILayout.HelpBox("Rendering into shadowmap on DX9, can't visualize it in the game view properly", MessageType.Info, true);
                }
            }
        }

        private void DrawStates()
        {
            FrameDebuggerBlendState frameBlendState = FrameDebuggerUtility.GetFrameBlendState();
            FrameDebuggerRasterState frameRasterState = FrameDebuggerUtility.GetFrameRasterState();
            FrameDebuggerDepthState frameDepthState = FrameDebuggerUtility.GetFrameDepthState();
            string str = string.Empty;
            if (frameBlendState.renderTargetWriteMask == 0)
            {
                str = "0";
            }
            else
            {
                if ((frameBlendState.renderTargetWriteMask & 2) != 0)
                {
                    str = str + "R";
                }
                if ((frameBlendState.renderTargetWriteMask & 4) != 0)
                {
                    str = str + "G";
                }
                if ((frameBlendState.renderTargetWriteMask & 8) != 0)
                {
                    str = str + "B";
                }
                if ((frameBlendState.renderTargetWriteMask & 1) != 0)
                {
                    str = str + "A";
                }
            }
            object[] args = new object[] { frameBlendState.srcBlend, frameBlendState.dstBlend, frameBlendState.srcBlendAlpha, frameBlendState.dstBlendAlpha, str };
            GUILayout.Label(string.Format("Blend {0} {1}, {2} {3} ColorMask {4}", args), EditorStyles.miniLabel, new GUILayoutOption[0]);
            object[] objArray2 = new object[] { frameDepthState.depthFunc, (frameDepthState.depthWrite != 0) ? "On" : "Off", frameRasterState.cullMode, frameRasterState.slopeScaledDepthBias, frameRasterState.depthBias };
            GUILayout.Label(string.Format("ZTest {0} ZWrite {1} Cull {2} Offset {3}, {4}", objArray2), EditorStyles.miniLabel, new GUILayoutOption[0]);
        }

        private bool DrawToolbar(FrameDebuggerEvent[] descs)
        {
            bool flag = false;
            GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginDisabledGroup(!GraphicsSupportsFrameDebugger());
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinWidth(80f) };
            GUILayout.Toggle(FrameDebuggerUtility.enabled, styles.recordButton, EditorStyles.toolbarButton, options);
            EditorGUI.EndDisabledGroup();
            if (EditorGUI.EndChangeCheck())
            {
                this.ClickEnableFrameDebugger();
                flag = true;
            }
            GUI.enabled = FrameDebuggerUtility.enabled;
            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginDisabledGroup(FrameDebuggerUtility.count <= 1);
            int newLimit = EditorGUILayout.IntSlider(FrameDebuggerUtility.limit, 1, FrameDebuggerUtility.count, new GUILayoutOption[0]);
            EditorGUI.EndDisabledGroup();
            if (EditorGUI.EndChangeCheck())
            {
                this.ChangeFrameEventLimit(newLimit);
            }
            GUILayout.Label(" of " + FrameDebuggerUtility.count, EditorStyles.miniLabel, new GUILayoutOption[0]);
            EditorGUI.BeginDisabledGroup(newLimit <= 1);
            if (GUILayout.Button(styles.prevFrame, EditorStyles.toolbarButton, new GUILayoutOption[0]))
            {
                this.ChangeFrameEventLimit(newLimit - 1);
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(newLimit >= FrameDebuggerUtility.count);
            if (GUILayout.Button(styles.nextFrame, EditorStyles.toolbarButton, new GUILayoutOption[0]))
            {
                this.ChangeFrameEventLimit(newLimit + 1);
            }
            if (((this.m_PrevEventsLimit == this.m_PrevEventsCount) && (FrameDebuggerUtility.count != this.m_PrevEventsCount)) && (FrameDebuggerUtility.limit == this.m_PrevEventsLimit))
            {
                this.ChangeFrameEventLimit(FrameDebuggerUtility.count);
            }
            this.m_PrevEventsLimit = FrameDebuggerUtility.limit;
            this.m_PrevEventsCount = FrameDebuggerUtility.count;
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();
            return flag;
        }

        public void EnableIfNeeded()
        {
            if (!FrameDebuggerUtility.enabled)
            {
                this.m_RTChannel = 0;
                this.m_RTIndex = 0;
                this.m_RTBlackLevel = 0f;
                this.m_RTWhiteLevel = 1f;
                this.ClickEnableFrameDebugger();
                this.RepaintOnLimitChange();
            }
        }

        internal static GameObject GetGameObjectForEvent(int eventIndex)
        {
            GameObject gameObject = null;
            Component component = EditorUtility.InstanceIDToObject(FrameDebuggerUtility.GetFrameEventRendererID(eventIndex)) as Component;
            if (component != null)
            {
                gameObject = component.gameObject;
            }
            return gameObject;
        }

        private static bool GraphicsSupportsFrameDebugger()
        {
            return SystemInfo.graphicsMultiThreaded;
        }

        internal void OnDidOpenScene()
        {
            DisableFrameDebugger();
        }

        internal void OnDisable()
        {
            if (this.m_WireMaterial != null)
            {
                UnityEngine.Object.DestroyImmediate(this.m_WireMaterial, true);
            }
            if (this.m_PreviewUtility != null)
            {
                this.m_PreviewUtility.Cleanup();
                this.m_PreviewUtility = null;
            }
            s_FrameDebuggers.Remove(this);
            EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.playmodeStateChanged, new EditorApplication.CallbackFunction(this.OnPlayModeStateChanged));
            DisableFrameDebugger();
        }

        internal void OnEnable()
        {
            base.autoRepaintOnSceneChange = true;
            s_FrameDebuggers.Add(this);
            EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.playmodeStateChanged, new EditorApplication.CallbackFunction(this.OnPlayModeStateChanged));
            this.m_RepaintFrames = 4;
        }

        internal void OnGUI()
        {
            FrameDebuggerEvent[] frameEvents = FrameDebuggerUtility.GetFrameEvents();
            if (this.m_TreeViewState == null)
            {
                this.m_TreeViewState = new TreeViewState();
            }
            if (this.m_Tree == null)
            {
                this.m_Tree = new FrameDebuggerTreeView(frameEvents, this.m_TreeViewState, this, new Rect());
                this.m_FrameEventsHash = FrameDebuggerUtility.eventsHash;
                this.m_Tree.m_DataSource.SetExpandedWithChildren(this.m_Tree.m_DataSource.root, true);
            }
            if (FrameDebuggerUtility.eventsHash != this.m_FrameEventsHash)
            {
                this.m_Tree.m_DataSource.SetEvents(frameEvents);
                this.m_FrameEventsHash = FrameDebuggerUtility.eventsHash;
            }
            int limit = FrameDebuggerUtility.limit;
            bool flag = this.DrawToolbar(frameEvents);
            if (!FrameDebuggerUtility.enabled)
            {
                GUI.enabled = true;
                if (!GraphicsSupportsFrameDebugger())
                {
                    EditorGUILayout.HelpBox("Frame Debugger requires multi-threaded renderer. Usually Unity uses that; if it does not try starting with -force-gfx-mt command line argument.", MessageType.Warning, true);
                }
                EditorGUILayout.HelpBox("Frame Debugger lets you step through draw calls and see how exactly frame is rendered. Click Enable!", MessageType.Info, true);
            }
            else
            {
                float fixedHeight = EditorStyles.toolbar.fixedHeight;
                Rect dragRect = new Rect(this.m_ListWidth, fixedHeight, 5f, base.position.height - fixedHeight);
                dragRect = EditorGUIUtility.HandleHorizontalSplitter(dragRect, base.position.width, 200f, 200f);
                this.m_ListWidth = dragRect.x;
                Rect rect = new Rect(0f, fixedHeight, this.m_ListWidth, base.position.height - fixedHeight);
                Rect rect3 = new Rect(this.m_ListWidth + 4f, fixedHeight + 4f, (base.position.width - this.m_ListWidth) - 8f, (base.position.height - fixedHeight) - 8f);
                this.DrawEventsTree(rect);
                EditorGUIUtility.DrawHorizontalSplitter(dragRect);
                this.DrawCurrentEvent(rect3, frameEvents);
            }
            if (flag || (limit != FrameDebuggerUtility.limit))
            {
                this.RepaintOnLimitChange();
            }
            if (this.m_RepaintFrames > 0)
            {
                this.m_Tree.SelectFrameEventIndex(FrameDebuggerUtility.limit);
                this.RepaintAllNeededThings();
                this.m_RepaintFrames--;
            }
        }

        private void OnPlayModeStateChanged()
        {
            this.RepaintOnLimitChange();
        }

        internal static void RepaintAll()
        {
            foreach (FrameDebuggerWindow window in s_FrameDebuggers)
            {
                window.Repaint();
            }
        }

        private void RepaintAllNeededThings()
        {
            EditorApplication.SetSceneRepaintDirty();
            base.Repaint();
        }

        private void RepaintOnLimitChange()
        {
            this.m_RepaintFrames = 4;
            this.RepaintAllNeededThings();
        }

        [UnityEditor.MenuItem("Window/Frame Debugger", false, 0x834)]
        public static FrameDebuggerWindow ShowFrameDebuggerWindow()
        {
            FrameDebuggerWindow window = EditorWindow.GetWindow(typeof(FrameDebuggerWindow)) as FrameDebuggerWindow;
            if (window != null)
            {
                window.titleContent = EditorGUIUtility.TextContent("Frame Debug");
                window.EnableIfNeeded();
            }
            return window;
        }

        public static Styles styles
        {
            get
            {
                if (ms_Styles == null)
                {
                }
                return (ms_Styles = new Styles());
            }
        }

        internal class Styles
        {
            public GUIStyle background = "OL Box";
            public static readonly GUIContent channelHeader = EditorGUIUtility.TextContent("Channels|Which render target color channels to show");
            public static readonly GUIContent[] channelLabels = new GUIContent[] { EditorGUIUtility.TextContent("All|Show all (RGB) color channels"), EditorGUIUtility.TextContent("R|Show red channel only"), EditorGUIUtility.TextContent("G|Show green channel only"), EditorGUIUtility.TextContent("B|Show blue channel only"), EditorGUIUtility.TextContent("A|Show alpha channel only") };
            public static readonly GUIContent depthLabel = EditorGUIUtility.TextContent("Depth|Show depth buffer");
            public GUIStyle entryEven = "OL EntryBackEven";
            public GUIStyle entryOdd = "OL EntryBackOdd";
            public GUIStyle header = "OL title";
            public GUIContent[] headerContent;
            public static readonly GUIContent levelsHeader = EditorGUIUtility.TextContent("Levels|Render target display black/white intensity levels");
            public static readonly GUIContent[] mrtLabels = new GUIContent[] { EditorGUIUtility.TextContent("RT 0|Show render target #0"), EditorGUIUtility.TextContent("RT 1|Show render target #1"), EditorGUIUtility.TextContent("RT 2|Show render target #2"), EditorGUIUtility.TextContent("RT 3|Show render target #3"), EditorGUIUtility.TextContent("RT 4|Show render target #4"), EditorGUIUtility.TextContent("RT 5|Show render target #5"), EditorGUIUtility.TextContent("RT 6|Show render target #6"), EditorGUIUtility.TextContent("RT 7|Show render target #7") };
            public GUIContent nextFrame = new GUIContent(EditorGUIUtility.IconContent("Profiler.NextFrame", "|Go one frame forwards"));
            public GUIContent prevFrame = new GUIContent(EditorGUIUtility.IconContent("Profiler.PrevFrame", "|Go back one frame"));
            public GUIContent recordButton = new GUIContent(EditorGUIUtility.TextContent("Record|Record profiling information"));
            public GUIStyle rowText = "OL Label";
            public GUIStyle rowTextRight = new GUIStyle("OL Label");
            public static readonly string[] s_ColumnNames = new string[] { "#", "Type", "Vertices", "Indices" };

            public Styles()
            {
                this.rowTextRight.alignment = TextAnchor.MiddleRight;
                this.recordButton.text = "Enable";
                this.recordButton.tooltip = "Enable Frame Debugging";
                this.prevFrame.tooltip = "Previous event";
                this.nextFrame.tooltip = "Next event";
                this.headerContent = new GUIContent[s_ColumnNames.Length];
                for (int i = 0; i < this.headerContent.Length; i++)
                {
                    this.headerContent[i] = EditorGUIUtility.TextContent(s_ColumnNames[i]);
                }
            }
        }
    }
}

