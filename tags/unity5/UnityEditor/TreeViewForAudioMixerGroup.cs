namespace UnityEditor
{
    using mscorlib;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor.Audio;
    using UnityEditorInternal;
    using UnityEngine;

    internal static class TreeViewForAudioMixerGroup
    {
        private static readonly int kNoneItemID;
        private static string s_NoneText = "None";

        public static void CreateAndSetTreeView(ObjectTreeForSelector.TreeSelectorData data)
        {
            TreeViewDataSourceForMixers mixers;
            AudioMixerController objectFromInstanceID = InternalEditorUtility.GetObjectFromInstanceID(data.userData) as AudioMixerController;
            TreeView treeView = new TreeView(data.editorWindow, data.state);
            GroupTreeViewGUI gui = new GroupTreeViewGUI(treeView);
            mixers = new TreeViewDataSourceForMixers(treeView, objectFromInstanceID) {
                onVisibleRowsChanged = (System.Action) Delegate.Combine(mixers.onVisibleRowsChanged, new System.Action(gui.CalculateRowRects))
            };
            treeView.deselectOnUnhandledMouseDown = false;
            treeView.Init(data.treeViewRect, mixers, gui, null);
            data.objectTreeForSelector.SetTreeView(treeView);
        }

        private class GroupTreeViewGUI : TreeViewGUI
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
                if (!base.m_TreeView.isSearching)
                {
                    float width = GUIClip.visibleRect.width;
                    List<TreeViewItem> visibleRows = base.m_TreeView.data.GetVisibleRows();
                    this.m_RowRects = new List<Rect>(visibleRows.Count);
                    float y = 2f;
                    for (int i = 0; i < visibleRows.Count; i++)
                    {
                        bool flag = this.IsController(visibleRows[i]);
                        y += !flag ? 0f : 25f;
                        float height = base.k_LineHeight;
                        this.m_RowRects.Add(new Rect(0f, y, width, height));
                        y += height;
                    }
                }
            }

            public override void GetFirstAndLastRowVisible(List<TreeViewItem> rows, float topPixel, float heightInPixels, out int firstRowVisible, out int lastRowVisible)
            {
                if (base.m_TreeView.isSearching)
                {
                    base.GetFirstAndLastRowVisible(rows, topPixel, heightInPixels, out firstRowVisible, out lastRowVisible);
                }
                else
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
                                goto Label_00BB;
                            }
                        }
                        Rect rect3 = this.m_RowRects[i];
                        System.Boolean ReflectorVariable0 = true;
                        goto Label_00BC;
                    Label_00BB:
                        ReflectorVariable0 = false;
                    Label_00BC:
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
            }

            public override float GetHeightOfLastRow()
            {
                if (base.m_TreeView.isSearching)
                {
                    return base.GetHeightOfLastRow();
                }
                Rect rect = this.m_RowRects[this.m_RowRects.Count - 1];
                return rect.height;
            }

            protected override Texture GetIconForNode(TreeViewItem item)
            {
                if ((item != null) && (item.icon != null))
                {
                    return item.icon;
                }
                if (item.id == TreeViewForAudioMixerGroup.kNoneItemID)
                {
                    return this.k_AudioListenerIcon;
                }
                return this.k_AudioGroupIcon;
            }

            public override int GetNumRowsOnPageUpDown(TreeViewItem fromItem, bool pageUp, float heightOfTreeView)
            {
                if (base.m_TreeView.isSearching)
                {
                    return base.GetNumRowsOnPageUpDown(fromItem, pageUp, heightOfTreeView);
                }
                return (int) Mathf.Floor(heightOfTreeView / base.k_LineHeight);
            }

            public override float GetTopPixelOfRow(int row, List<TreeViewItem> rows)
            {
                if (base.m_TreeView.isSearching)
                {
                    return base.GetTopPixelOfRow(row, rows);
                }
                Rect rect = this.m_RowRects[row];
                return rect.y;
            }

            public override Vector2 GetTotalSize(List<TreeViewItem> rows)
            {
                if (base.m_TreeView.isSearching)
                {
                    Vector2 totalSize = base.GetTotalSize(rows);
                    totalSize.x = 1f;
                    return totalSize;
                }
                if (this.m_RowRects.Count == 0)
                {
                    return new Vector2(1f, 1f);
                }
                Rect rect = this.m_RowRects[this.m_RowRects.Count - 1];
                return new Vector2(1f, rect.yMax);
            }

            private bool IsController(TreeViewItem item)
            {
                return ((item.parent == base.m_TreeView.data.root) && (item.id != TreeViewForAudioMixerGroup.kNoneItemID));
            }

            public override Rect OnRowGUI(TreeViewItem item, int row, float rowWidth, bool selected, bool focused)
            {
                if (base.m_TreeView.isSearching)
                {
                    return base.OnRowGUI(item, row, rowWidth, selected, focused);
                }
                Rect rect = this.m_RowRects[row];
                this.DoNodeGUI(rect, item, selected, focused, false);
                bool flag = item.parent == base.m_TreeView.data.root;
                bool flag2 = item.id == TreeViewForAudioMixerGroup.kNoneItemID;
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

        private class TreeViewDataSourceForMixers : TreeViewDataSource
        {
            public TreeViewDataSourceForMixers(TreeView treeView, AudioMixerController ignoreController) : base(treeView)
            {
                base.showRootNode = false;
                base.rootIsCollapsable = false;
                this.ignoreThisController = ignoreController;
                base.alwaysAddFirstItemToSearchResult = true;
            }

            private void AddChildrenRecursive(AudioMixerGroupController group, TreeViewItem item)
            {
                item.children = new List<TreeViewItem>(group.children.Length);
                for (int i = 0; i < group.children.Length; i++)
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
                this.SetExpanded(base.m_RootItem.id, true);
                List<int> allowedInstanceIDs = ObjectSelector.get.allowedInstanceIDs;
                HierarchyProperty property = new HierarchyProperty(HierarchyType.Assets);
                SearchFilter filter = new SearchFilter();
                filter.classNames = new string[] { "AudioMixerController" };
                property.SetSearchFilter(filter);
                List<AudioMixerController> list2 = new List<AudioMixerController>();
                while (property.Next(null))
                {
                    AudioMixerController pptrValue = property.pptrValue as AudioMixerController;
                    if (this.ShouldShowController(pptrValue, allowedInstanceIDs))
                    {
                        list2.Add(pptrValue);
                    }
                }
                List<TreeViewItem> list3 = new List<TreeViewItem> {
                    new TreeViewItem(TreeViewForAudioMixerGroup.kNoneItemID, 0, base.m_RootItem, TreeViewForAudioMixerGroup.s_NoneText)
                };
                foreach (AudioMixerController controller2 in list2)
                {
                    list3.Add(this.BuildSubTree(controller2));
                }
                base.m_RootItem.children = list3;
                if (list2.Count == 1)
                {
                    base.m_TreeView.data.SetExpandedWithChildren(base.m_RootItem, true);
                }
                base.m_NeedRefreshVisibleFolders = true;
            }

            public override bool IsRenamingItemAllowed(TreeViewItem item)
            {
                return false;
            }

            private bool ShouldShowController(AudioMixerController controller, List<int> allowedInstanceIDs)
            {
                if (controller == null)
                {
                    return false;
                }
                if ((allowedInstanceIDs != null) && (allowedInstanceIDs.Count > 0))
                {
                    return allowedInstanceIDs.Contains(controller.GetInstanceID());
                }
                return true;
            }

            public AudioMixerController ignoreThisController { get; private set; }
        }
    }
}

