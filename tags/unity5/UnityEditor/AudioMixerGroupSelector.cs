namespace UnityEditor
{
    using mscorlib;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor.Audio;
    using UnityEngine;

    internal class AudioMixerGroupSelector : PopupWindowContent
    {
        private const int kNoneItemID = 0;
        private bool m_FocusSearchFilter;
        private AudioMixerController m_IgnoreThisController;
        private AudioMixerGroupController m_OriginalSelection;
        private bool m_RecalculateWindowSize;
        private string m_SearchFilter;
        private Action<AudioMixerGroupController> m_SelectionCallback;
        private TreeView m_TreeView;
        private TreeViewState m_TreeViewState;
        private Vector2 m_WindowSize = new Vector2(250f, 5f);
        public static string s_NoneText = "Audio Listener";

        public AudioMixerGroupSelector(AudioMixerGroupController originalSelection, AudioMixerController ignoreThisController, Action<AudioMixerGroupController> selectionCallback)
        {
            this.m_OriginalSelection = originalSelection;
            this.m_SelectionCallback = selectionCallback;
            this.m_IgnoreThisController = ignoreThisController;
        }

        private void Cancel()
        {
            if (this.m_SelectionCallback != null)
            {
                this.m_SelectionCallback(this.m_OriginalSelection);
            }
            base.editorWindow.Close();
            GUI.changed = true;
            GUIUtility.ExitGUI();
        }

        private void FilterSettingsChanged()
        {
        }

        private AudioMixerGroupController GetGroupByID(int id)
        {
            TreeViewDataSourceForMixers data = this.m_TreeView.data as TreeViewDataSourceForMixers;
            return data.GetGroup(id);
        }

        public override Vector2 GetWindowSize()
        {
            if (this.m_RecalculateWindowSize)
            {
                Vector2 totalSize = this.m_TreeView.gui.GetTotalSize(this.m_TreeView.data.GetVisibleRows());
                float num = 120f;
                this.m_WindowSize.x = Math.Max(num, totalSize.x);
                float num2 = 7f;
                float max = 600f;
                float min = 18f;
                this.m_WindowSize.y = Mathf.Clamp(totalSize.y + num2, min, max);
                this.m_RecalculateWindowSize = false;
            }
            return this.m_WindowSize;
        }

        private void HandleKeyboard()
        {
            if (Event.current.type == EventType.KeyDown)
            {
                switch (Event.current.keyCode)
                {
                    case KeyCode.Return:
                    case KeyCode.KeypadEnter:
                        Event.current.Use();
                        base.editorWindow.Close();
                        GUI.changed = true;
                        GUIUtility.ExitGUI();
                        break;
                }
            }
        }

        private void InitTreeView(Rect rect)
        {
            TreeViewDataSourceForMixers mixers;
            this.m_TreeViewState = new TreeViewState();
            this.m_TreeView = new TreeView(base.editorWindow, this.m_TreeViewState);
            GroupTreeViewGUI gui = new GroupTreeViewGUI(this.m_TreeView);
            mixers = new TreeViewDataSourceForMixers(this.m_TreeView, this.m_IgnoreThisController) {
                onVisibleRowsChanged = (System.Action) Delegate.Combine(mixers.onVisibleRowsChanged, new System.Action(gui.CalculateRowRects)),
                onVisibleRowsChanged = (System.Action) Delegate.Combine(mixers.onVisibleRowsChanged, new System.Action(this.ResizeWindow))
            };
            this.m_TreeView.deselectOnUnhandledMouseDown = true;
            this.m_TreeView.Init(rect, mixers, gui, null);
            this.m_TreeView.ReloadData();
            this.m_TreeView.selectionChangedCallback = new Action<int[]>(this.OnItemSelectionChanged);
            this.m_TreeView.itemDoubleClickedCallback = new Action<int>(this.OnItemDoubleClicked);
            int[] selectedIDs = new int[] { (this.m_OriginalSelection == null) ? 0 : this.m_OriginalSelection.GetInstanceID() };
            this.m_TreeView.SetSelection(selectedIDs, true);
        }

        public override void OnGUI(Rect rect)
        {
            if (this.m_TreeView == null)
            {
                this.InitTreeView(rect);
            }
            Rect treeViewRect = new Rect(rect.x, 0f, rect.width, rect.height);
            this.HandleKeyboard();
            this.TreeViewArea(treeViewRect);
            if ((Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.Escape))
            {
                this.Cancel();
            }
        }

        private void OnItemDoubleClicked(int id)
        {
            base.editorWindow.Close();
        }

        private void OnItemSelectionChanged(int[] selection)
        {
            if (this.m_SelectionCallback != null)
            {
                AudioMixerGroupController groupByID = null;
                if (selection.Length > 0)
                {
                    groupByID = this.GetGroupByID(selection[0]);
                }
                this.m_SelectionCallback(groupByID);
            }
        }

        private void Repaint()
        {
            base.editorWindow.Repaint();
        }

        private void ResizeWindow()
        {
            this.m_RecalculateWindowSize = true;
        }

        private void SearchArea(Rect toolbarRect)
        {
            GUI.Label(toolbarRect, GUIContent.none, "ObjectPickerToolbar");
            bool flag = (Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.Escape);
            GUI.SetNextControlName("SearchFilter");
            string str = EditorGUI.SearchField(new Rect(5f, 5f, toolbarRect.width - 10f, 15f), this.m_SearchFilter);
            if (flag && (Event.current.type == EventType.Used))
            {
                if (this.m_SearchFilter == string.Empty)
                {
                    this.Cancel();
                }
                this.m_FocusSearchFilter = true;
            }
            if ((str != this.m_SearchFilter) || this.m_FocusSearchFilter)
            {
                this.m_SearchFilter = str;
                this.FilterSettingsChanged();
                this.Repaint();
            }
            if (this.m_FocusSearchFilter)
            {
                EditorGUI.FocusTextInControl("SearchFilter");
                this.m_FocusSearchFilter = false;
            }
        }

        private void TreeViewArea(Rect treeViewRect)
        {
            int controlID = GUIUtility.GetControlID("Tree".GetHashCode(), FocusType.Keyboard);
            GUIUtility.keyboardControl = controlID;
            if (this.m_TreeView.data.GetVisibleRows().Count > 0)
            {
                this.m_TreeView.OnGUI(treeViewRect, controlID);
            }
            else
            {
                EditorGUI.BeginDisabledGroup(true);
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.FlexibleSpace();
                GUILayout.Label(GUIContent.Temp("No Audio Mixers available"), new GUILayoutOption[0]);
                GUILayout.FlexibleSpace();
                EditorGUI.EndDisabledGroup();
            }
        }

        internal class GroupTreeViewGUI : TreeViewGUI
        {
            private readonly Texture2D k_AudioGroupIcon;
            private readonly Texture2D k_AudioListenerIcon;
            private const float k_HeaderHeight = 20f;
            private const float k_SpaceBetween = 25f;
            private List<Rect> m_RowRects;

            public GroupTreeViewGUI(TreeView treeView) : base(treeView)
            {
                this.k_AudioGroupIcon = EditorGUIUtility.FindTexture("AudioMixerGroup Icon");
                this.k_AudioListenerIcon = EditorGUIUtility.FindTexture("AudioListener Icon");
                this.m_RowRects = new List<Rect>();
            }

            public void CalculateRowRects()
            {
                float width = GUIClip.visibleRect.width;
                List<TreeViewItem> visibleRows = base.m_TreeView.data.GetVisibleRows();
                this.m_RowRects = new List<Rect>(visibleRows.Count);
                float y = 0f;
                for (int i = 0; i < visibleRows.Count; i++)
                {
                    bool flag = this.IsController(visibleRows[i]);
                    y += !flag ? 0f : 25f;
                    float height = base.k_LineHeight;
                    this.m_RowRects.Add(new Rect(0f, y, width, height));
                    y += height;
                }
            }

            public override void GetFirstAndLastRowVisible(List<TreeViewItem> rows, float topPixel, float heightInPixels, out int firstRowVisible, out int lastRowVisible)
            {
                if (rows.Count != this.m_RowRects.Count)
                {
                    Debug.LogError("Mismatch in state: rows vs cached rects");
                }
                int num = -1;
                int num2 = -1;
                for (int i = 0; i < this.m_RowRects.Count; i++)
                {
                    Rect rect = this.m_RowRects[i];
                    if (rect.y > topPixel)
                    {
                        Rect rect2 = this.m_RowRects[i];
                        if (rect2.y < (topPixel + heightInPixels))
                        {
                            goto Label_009D;
                        }
                    }
                    Rect rect3 = this.m_RowRects[i];
                    System.Boolean ReflectorVariable0 = true;
                    goto Label_009E;
                Label_009D:
                    ReflectorVariable0 = false;
                Label_009E:
                    if (ReflectorVariable0 ? ((rect3.yMax > topPixel) && (this.m_RowRects[i].yMax < (topPixel + heightInPixels))) : true)
                    {
                        if (num == -1)
                        {
                            num = i;
                        }
                        num2 = i;
                    }
                }
                if ((num != -1) && (num2 != -1))
                {
                    firstRowVisible = num;
                    lastRowVisible = num2;
                }
                else
                {
                    firstRowVisible = 0;
                    lastRowVisible = rows.Count - 1;
                }
            }

            public override float GetHeightOfLastRow()
            {
                Rect rect = this.m_RowRects[this.m_RowRects.Count - 1];
                return rect.height;
            }

            protected override Texture GetIconForNode(TreeViewItem item)
            {
                if ((item != null) && (item.icon != null))
                {
                    return item.icon;
                }
                if (item.id == 0)
                {
                    return this.k_AudioListenerIcon;
                }
                return this.k_AudioGroupIcon;
            }

            public override int GetNumRowsOnPageUpDown(TreeViewItem fromItem, bool pageUp, float heightOfTreeView)
            {
                return (int) Mathf.Floor(heightOfTreeView / base.k_LineHeight);
            }

            public override float GetTopPixelOfRow(int row, List<TreeViewItem> rows)
            {
                Rect rect = this.m_RowRects[row];
                return rect.y;
            }

            public override Vector2 GetTotalSize(List<TreeViewItem> rows)
            {
                if (this.m_RowRects.Count == 0)
                {
                    return new Vector2(1f, 1f);
                }
                Rect rect = this.m_RowRects[this.m_RowRects.Count - 1];
                return new Vector2(base.GetMaxWidth(rows) + 10f, rect.yMax);
            }

            private bool IsController(TreeViewItem item)
            {
                return ((item.parent == base.m_TreeView.data.root) && (item.id != 0));
            }

            public override Rect OnRowGUI(TreeViewItem item, int row, float rowWidth, bool selected, bool focused)
            {
                Rect rect = this.m_RowRects[row];
                float num = base.k_FoldoutWidth;
                if (item.id == 0)
                {
                    base.k_FoldoutWidth = 0f;
                }
                this.DoNodeGUI(rect, item, selected, focused, false);
                if (item.id == 0)
                {
                    base.k_FoldoutWidth = num;
                }
                bool flag = item.parent == base.m_TreeView.data.root;
                bool flag2 = item.id == 0;
                if (flag && !flag2)
                {
                    AudioMixerController controller = (item.userData as AudioMixerGroupController).controller;
                    GUI.Label(new Rect(rect.x + 2f, rect.y - 18f, rect.width, 18f), GUIContent.Temp(controller.name), EditorStyles.boldLabel);
                }
                return rect;
            }

            protected override void RenameEnded()
            {
            }

            protected override void SyncFakeItem()
            {
            }
        }

        internal class TreeViewDataSourceForMixers : TreeViewDataSource
        {
            public TreeViewDataSourceForMixers(TreeView treeView, AudioMixerController ignoreController) : base(treeView)
            {
                base.showRootNode = false;
                base.rootIsCollapsable = false;
                this.ignoreThisController = ignoreController;
            }

            private void AddChildrenRecursive(AudioMixerGroupController group, TreeViewItem item)
            {
                item.children = new List<TreeViewItem>(group.children.Length);
                for (int i = 0; i < item.children.Count; i++)
                {
                    item.children.Add(new TreeViewItem(group.children[i].GetInstanceID(), item.depth + 1, item, group.children[i].name));
                    item.children[i].userData = group.children[i];
                    this.AddChildrenRecursive(group.children[i], item.children[i]);
                }
            }

            private TreeViewItem BuildSubTree(AudioMixerController controller)
            {
                AudioMixerGroupController masterGroup = controller.masterGroup;
                TreeViewItem item = new TreeViewItem(masterGroup.GetInstanceID(), 0, base.m_RootItem, masterGroup.name) {
                    userData = masterGroup
                };
                this.AddChildrenRecursive(masterGroup, item);
                return item;
            }

            public override bool CanBeMultiSelected(TreeViewItem item)
            {
                return false;
            }

            public override void FetchData()
            {
                int depth = -1;
                base.m_RootItem = new TreeViewItem(0x3c34eb12, depth, null, "InvisibleRoot");
                base.expandedIDs.Add(base.m_RootItem.id);
                HierarchyProperty property = new HierarchyProperty(HierarchyType.Assets);
                SearchFilter filter = new SearchFilter();
                filter.classNames = new string[] { "AudioMixerController" };
                property.SetSearchFilter(filter);
                List<AudioMixerController> list = new List<AudioMixerController>();
                while (property.Next(null))
                {
                    AudioMixerController pptrValue = property.pptrValue as AudioMixerController;
                    bool flag = AudioMixerController.CheckForCyclicReferences(this.ignoreThisController, pptrValue.outputAudioMixerGroup);
                    if (((pptrValue != null) && (pptrValue != this.ignoreThisController)) && !flag)
                    {
                        list.Add(pptrValue);
                    }
                }
                List<TreeViewItem> list2 = new List<TreeViewItem> {
                    new TreeViewItem(0, 0, base.m_RootItem, AudioMixerGroupSelector.s_NoneText)
                };
                foreach (AudioMixerController controller2 in list)
                {
                    list2.Add(this.BuildSubTree(controller2));
                }
                base.m_RootItem.children = list2;
                this.SetExpandedIDs(base.expandedIDs.ToArray());
            }

            public AudioMixerGroupController GetGroup(int instanceID)
            {
                TreeViewItem item = base.m_TreeView.FindNode(instanceID);
                if (item != null)
                {
                    return (AudioMixerGroupController) item.userData;
                }
                return null;
            }

            public override bool IsRenamingItemAllowed(TreeViewItem item)
            {
                return false;
            }

            public AudioMixerController ignoreThisController { get; private set; }
        }
    }
}

