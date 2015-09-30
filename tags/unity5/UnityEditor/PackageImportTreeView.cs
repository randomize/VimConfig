namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    internal class PackageImportTreeView
    {
        private List<PackageImportTreeViewItem> m_Selection = new List<PackageImportTreeViewItem>();
        private TreeView m_TreeView;
        private static readonly bool s_UseFoldouts = true;

        public PackageImportTreeView(AssetsItem[] items, List<string> enabledFolders, TreeViewState treeViewState, EditorWindow editorWindow, Rect startRect)
        {
            this.m_TreeView = new TreeView(editorWindow, treeViewState);
            PackageImportTreeViewDataSource data = new PackageImportTreeViewDataSource(this.m_TreeView, items, enabledFolders);
            PackageImportTreeViewGUI gui = new PackageImportTreeViewGUI(this.m_TreeView);
            this.m_TreeView.Init(startRect, data, gui, null);
            this.m_TreeView.ReloadData();
            this.m_TreeView.selectionChangedCallback = (Action<int[]>) Delegate.Combine(this.m_TreeView.selectionChangedCallback, new Action<int[]>(this.SelectionChanged));
            gui.itemWasToggled = (Action<PackageImportTreeViewItem>) Delegate.Combine(gui.itemWasToggled, new Action<PackageImportTreeViewItem>(this.ItemWasToggled));
            this.ComputeEnabledStateForFolders();
        }

        private void ComputeEnabledStateForFolders()
        {
            PackageImportTreeViewItem root = this.m_TreeView.data.root as PackageImportTreeViewItem;
            HashSet<PackageImportTreeViewItem> done = new HashSet<PackageImportTreeViewItem> {
                root
            };
            this.RecursiveComputeEnabledStateForFolders(root, done);
        }

        private void EnableChildrenRecursive(TreeViewItem parentItem, int enabled)
        {
            if (parentItem.hasChildren)
            {
                foreach (TreeViewItem item in parentItem.children)
                {
                    PackageImportTreeViewItem item2 = item as PackageImportTreeViewItem;
                    item2.item.enabled = enabled;
                    this.EnableChildrenRecursive(item2, enabled);
                }
            }
        }

        public void GetEnabledFolders(List<string> folderPaths)
        {
            this.GetEnabledFoldersRecursive(this.m_TreeView.data.root, folderPaths);
        }

        private void GetEnabledFoldersRecursive(TreeViewItem parentItem, List<string> folderPaths)
        {
            if (parentItem.hasChildren)
            {
                foreach (TreeViewItem item in parentItem.children)
                {
                    PackageImportTreeViewItem item2 = item as PackageImportTreeViewItem;
                    if (item2.isFolder && (item2.item.enabled > 0))
                    {
                        folderPaths.Add(item2.item.pathName);
                    }
                    this.GetEnabledFoldersRecursive(item2, folderPaths);
                }
            }
        }

        private Amount GetFolderChildrenEnabledState(PackageImportTreeViewItem folder)
        {
            if (!folder.isFolder)
            {
                Debug.LogError("Should be a folder item!");
            }
            if (!folder.hasChildren)
            {
                return Amount.None;
            }
            Amount notSet = Amount.NotSet;
            PackageImportTreeViewItem item = folder.children[0] as PackageImportTreeViewItem;
            int enabled = item.item.enabled;
            for (int i = 1; i < folder.children.Count; i++)
            {
                if (enabled != (folder.children[i] as PackageImportTreeViewItem).item.enabled)
                {
                    notSet = Amount.Mixed;
                    break;
                }
            }
            if (notSet == Amount.NotSet)
            {
                notSet = (enabled != 1) ? Amount.None : Amount.All;
            }
            return notSet;
        }

        public AssetsItem GetSingleSelection()
        {
            if ((this.m_Selection != null) && (this.m_Selection.Count == 1))
            {
                return this.m_Selection[0].item;
            }
            return null;
        }

        private void ItemWasToggled(PackageImportTreeViewItem pitem)
        {
            if (this.m_Selection.Count <= 1)
            {
                this.EnableChildrenRecursive(pitem, pitem.item.enabled);
            }
            else
            {
                foreach (PackageImportTreeViewItem item in this.m_Selection)
                {
                    item.item.enabled = pitem.item.enabled;
                }
            }
            this.ComputeEnabledStateForFolders();
        }

        public void OnGUI(Rect rect)
        {
            if (Event.current.type == EventType.ScrollWheel)
            {
                PopupWindowWithoutFocus.Hide();
            }
            int controlID = GUIUtility.GetControlID(FocusType.Keyboard);
            this.m_TreeView.OnGUI(rect, controlID);
            if ((((Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.Space)) && ((this.m_Selection != null) && (this.m_Selection.Count > 0))) && (GUIUtility.keyboardControl == controlID))
            {
                int num2 = (this.m_Selection[0].item.enabled != 0) ? 0 : 1;
                this.m_Selection[0].item.enabled = num2;
                this.ItemWasToggled(this.m_Selection[0]);
                Event.current.Use();
            }
        }

        private void RecursiveComputeEnabledStateForFolders(PackageImportTreeViewItem pitem, HashSet<PackageImportTreeViewItem> done)
        {
            if (pitem.isFolder)
            {
                if (pitem.hasChildren)
                {
                    foreach (TreeViewItem item in pitem.children)
                    {
                        this.RecursiveComputeEnabledStateForFolders(item as PackageImportTreeViewItem, done);
                    }
                }
                if (!done.Contains(pitem))
                {
                    Amount folderChildrenEnabledState = this.GetFolderChildrenEnabledState(pitem);
                    pitem.item.enabled = (int) folderChildrenEnabledState;
                    if (folderChildrenEnabledState == Amount.Mixed)
                    {
                        done.Add(pitem);
                        for (PackageImportTreeViewItem item2 = pitem.parent as PackageImportTreeViewItem; item2 != null; item2 = item2.parent as PackageImportTreeViewItem)
                        {
                            if (!done.Contains(item2))
                            {
                                item2.item.enabled = 2;
                                done.Add(item2);
                            }
                        }
                    }
                }
            }
        }

        private void SelectionChanged(int[] selectedIDs)
        {
            this.m_Selection = new List<PackageImportTreeViewItem>();
            foreach (TreeViewItem item in this.m_TreeView.data.GetVisibleRows())
            {
                if (selectedIDs.Contains<int>(item.id))
                {
                    PackageImportTreeViewItem item2 = item as PackageImportTreeViewItem;
                    if (item2 != null)
                    {
                        this.m_Selection.Add(item2);
                    }
                }
            }
            if ((this.m_Selection.Count == 1) && !string.IsNullOrEmpty(this.m_Selection[0].item.previewPath))
            {
                PackageImportTreeViewGUI gui = this.m_TreeView.gui as PackageImportTreeViewGUI;
                gui.showPreviewForID = this.m_Selection[0].id;
            }
            else
            {
                PopupWindowWithoutFocus.Hide();
            }
        }

        public void SetAllEnabled(int enabled)
        {
            this.EnableChildrenRecursive(this.m_TreeView.data.root, enabled);
            this.ComputeEnabledStateForFolders();
        }

        public enum Amount
        {
            All = 1,
            Mixed = 2,
            None = 0,
            NotSet = -1
        }

        private class PackageImportTreeViewDataSource : TreeViewDataSource
        {
            public AssetsItem[] m_AssetItems;
            public List<string> m_EnabledFolders;

            public PackageImportTreeViewDataSource(TreeView treeView, AssetsItem[] assetItems, List<string> enabledFolders) : base(treeView)
            {
                this.m_AssetItems = assetItems;
                this.m_EnabledFolders = enabledFolders;
                base.rootIsCollapsable = false;
                base.showRootNode = false;
            }

            private TreeViewItem EnsureFolderPath(string folderPath, Dictionary<string, AssetsItem> packageFolders, Dictionary<string, PackageImportTreeView.PackageImportTreeViewItem> treeViewFolders, bool initExpandedState)
            {
                TreeViewItem item = TreeViewUtility.FindItem(folderPath.GetHashCode(), base.m_RootItem);
                if (item != null)
                {
                    return item;
                }
                char[] separator = new char[] { '/' };
                string[] strArray = folderPath.Split(separator);
                string key = string.Empty;
                TreeViewItem rootItem = base.m_RootItem;
                for (int i = 0; i < strArray.Length; i++)
                {
                    string displayName = strArray[i];
                    if (key != string.Empty)
                    {
                        key = key + '/';
                    }
                    key = key + displayName;
                    if (key != "Assets")
                    {
                        PackageImportTreeView.PackageImportTreeViewItem item3;
                        int hashCode = key.GetHashCode();
                        if (treeViewFolders.TryGetValue(key, out item3))
                        {
                            rootItem = item3;
                        }
                        else
                        {
                            AssetsItem item5;
                            int depth = i - 1;
                            PackageImportTreeView.PackageImportTreeViewItem child = new PackageImportTreeView.PackageImportTreeViewItem(hashCode, depth, rootItem, displayName) {
                                isFolder = true
                            };
                            if (packageFolders.TryGetValue(key, out item5))
                            {
                                child.item = item5;
                            }
                            if (child.item == null)
                            {
                                AssetsItem item6 = new AssetsItem {
                                    assetIsDir = 1,
                                    pathName = key,
                                    exportedAssetPath = key,
                                    enabled = (this.m_EnabledFolders != null) ? (!this.m_EnabledFolders.Contains(key) ? 0 : 1) : 1,
                                    guid = AssetDatabase.AssetPathToGUID(key),
                                    previewPath = string.Empty
                                };
                                child.item = item6;
                                child.item.exists = !string.IsNullOrEmpty(child.item.guid) ? 1 : 0;
                            }
                            rootItem.AddChild(child);
                            rootItem = child;
                            if (initExpandedState)
                            {
                                base.m_TreeView.state.expandedIDs.Add(hashCode);
                            }
                            treeViewFolders[key] = child;
                        }
                    }
                }
                return rootItem;
            }

            public override void FetchData()
            {
                int depth = -1;
                base.m_RootItem = new PackageImportTreeView.PackageImportTreeViewItem("Assets".GetHashCode(), depth, null, "InvisibleAssetsFolder");
                ((PackageImportTreeView.PackageImportTreeViewItem) base.m_RootItem).isFolder = true;
                bool initExpandedState = base.m_TreeView.state.expandedIDs.Count == 0;
                if (initExpandedState)
                {
                    base.m_TreeView.state.expandedIDs.Add(base.m_RootItem.id);
                }
                Dictionary<string, AssetsItem> packageFolders = new Dictionary<string, AssetsItem>();
                foreach (AssetsItem item in this.m_AssetItems)
                {
                    if (item.assetIsDir == 1)
                    {
                        packageFolders[item.pathName] = item;
                    }
                }
                Dictionary<string, PackageImportTreeView.PackageImportTreeViewItem> treeViewFolders = new Dictionary<string, PackageImportTreeView.PackageImportTreeViewItem>();
                foreach (AssetsItem item2 in this.m_AssetItems)
                {
                    if ((item2.assetIsDir != 1) && !PackageImport.HasInvalidCharInFilePath(item2.pathName))
                    {
                        string fileName = Path.GetFileName(item2.pathName);
                        string directoryName = Path.GetDirectoryName(item2.pathName);
                        TreeViewItem parent = this.EnsureFolderPath(directoryName, packageFolders, treeViewFolders, initExpandedState);
                        if (parent != null)
                        {
                            PackageImportTreeView.PackageImportTreeViewItem child = new PackageImportTreeView.PackageImportTreeViewItem(item2.pathName.GetHashCode(), parent.depth + 1, parent, fileName) {
                                item = item2
                            };
                            parent.AddChild(child);
                        }
                    }
                }
                if (initExpandedState)
                {
                    base.m_TreeView.state.expandedIDs.Sort();
                }
            }

            public override bool IsExpandable(TreeViewItem item)
            {
                if (!PackageImportTreeView.s_UseFoldouts)
                {
                    return false;
                }
                return base.IsExpandable(item);
            }

            public override bool IsRenamingItemAllowed(TreeViewItem item)
            {
                return false;
            }
        }

        private class PackageImportTreeViewGUI : TreeViewGUI
        {
            public Action<PackageImportTreeView.PackageImportTreeViewItem> itemWasToggled;

            public PackageImportTreeViewGUI(TreeView treeView) : base(treeView)
            {
                base.k_BaseIndent = 4f;
                if (!PackageImportTreeView.s_UseFoldouts)
                {
                    base.k_FoldoutWidth = 0f;
                }
            }

            private void DoIconAndText(TreeViewItem item, Rect contentRect, bool selected, bool focused)
            {
                EditorGUIUtility.SetIconSize(new Vector2(base.k_IconWidth, base.k_IconWidth));
                GUIStyle lineStyle = TreeViewGUI.s_Styles.lineStyle;
                lineStyle.padding.left = 0;
                if (Event.current.type == EventType.Repaint)
                {
                    lineStyle.Draw(contentRect, GUIContent.Temp(item.displayName, this.GetIconForNode(item)), false, false, selected, focused);
                }
                EditorGUIUtility.SetIconSize(Vector2.zero);
            }

            private void DoPreviewPopup(PackageImportTreeView.PackageImportTreeViewItem pitem, Rect rowRect)
            {
                if (((Event.current.type == EventType.MouseDown) && rowRect.Contains(Event.current.mousePosition)) && !PopupWindowWithoutFocus.IsVisible())
                {
                    this.showPreviewForID = pitem.id;
                }
                if ((pitem.id == this.showPreviewForID) && (Event.current.type != EventType.Layout))
                {
                    this.showPreviewForID = 0;
                    if (!string.IsNullOrEmpty(pitem.item.previewPath))
                    {
                        Texture2D preview = PackageImport.GetPreview(pitem.item.previewPath);
                        Rect activatorRect = rowRect;
                        activatorRect.width = EditorGUIUtility.currentViewWidth;
                        PopupLocationHelper.PopupLocation[] locationPriorityOrder = new PopupLocationHelper.PopupLocation[3];
                        locationPriorityOrder[0] = PopupLocationHelper.PopupLocation.Right;
                        locationPriorityOrder[1] = PopupLocationHelper.PopupLocation.Left;
                        PopupWindowWithoutFocus.Show(activatorRect, new PackageImportTreeView.PreviewPopup(preview), locationPriorityOrder);
                    }
                }
            }

            private void DoToggle(PackageImportTreeView.PackageImportTreeViewItem pitem, Rect toggleRect)
            {
                EditorGUI.BeginChangeCheck();
                Toggle(pitem, toggleRect);
                if (EditorGUI.EndChangeCheck())
                {
                    if ((base.m_TreeView.GetSelection().Length <= 1) || !base.m_TreeView.GetSelection().Contains<int>(pitem.id))
                    {
                        int[] selectedIDs = new int[] { pitem.id };
                        base.m_TreeView.SetSelection(selectedIDs, false);
                        base.m_TreeView.NotifyListenersThatSelectionChanged();
                    }
                    if (this.itemWasToggled != null)
                    {
                        this.itemWasToggled(pitem);
                    }
                    Event.current.Use();
                }
            }

            protected override Texture GetIconForNode(TreeViewItem item)
            {
                PackageImportTreeView.PackageImportTreeViewItem item2 = item as PackageImportTreeView.PackageImportTreeViewItem;
                if (item2.isFolder)
                {
                    return Constants.folderIcon;
                }
                Texture cachedIcon = AssetDatabase.GetCachedIcon(item2.item.pathName);
                if (cachedIcon != null)
                {
                    return cachedIcon;
                }
                return InternalEditorUtility.GetIconForFile(item2.item.pathName);
            }

            public override Rect OnRowGUI(TreeViewItem item, int row, float rowWidth, bool selected, bool focused)
            {
                Rect position = new Rect(0f, row * base.k_LineHeight, rowWidth, base.k_LineHeight);
                base.k_IndentWidth = 18f;
                base.k_FoldoutWidth = 18f;
                PackageImportTreeView.PackageImportTreeViewItem pitem = item as PackageImportTreeView.PackageImportTreeViewItem;
                bool flag = Event.current.type == EventType.Repaint;
                if (selected && flag)
                {
                    TreeViewGUI.s_Styles.lineStyle.Draw(position, false, false, true, focused);
                }
                if (base.m_TreeView.data.IsExpandable(item))
                {
                    this.DoFoldout(item, position);
                }
                Rect toggleRect = new Rect((base.k_BaseIndent + (item.depth * base.indentWidth)) + base.k_FoldoutWidth, position.y, 18f, position.height);
                this.DoToggle(pitem, toggleRect);
                Rect contentRect = new Rect(toggleRect.xMax, position.y, position.width, position.height);
                this.DoIconAndText(item, contentRect, selected, focused);
                this.DoPreviewPopup(pitem, position);
                if ((pitem.item.exists == 0) && flag)
                {
                    Texture image = Constants.badgeNew.image;
                    GUI.DrawTexture(new Rect((position.xMax - image.width) - 6f, position.y + ((position.height - image.height) / 2f), (float) image.width, (float) image.height), image);
                }
                return position;
            }

            protected override void RenameEnded()
            {
            }

            private static void Toggle(PackageImportTreeView.PackageImportTreeViewItem pitem, Rect toggleRect)
            {
                bool flag = pitem.item.enabled > 0;
                GUIStyle toggle = EditorStyles.toggle;
                if (pitem.isFolder && (pitem.item.enabled == 2))
                {
                    toggle = EditorStyles.toggleMixed;
                }
                bool flag3 = GUI.Toggle(toggleRect, flag, GUIContent.none, toggle);
                if (flag3 != flag)
                {
                    pitem.item.enabled = !flag3 ? 0 : 1;
                }
            }

            public int showPreviewForID { get; set; }

            internal static class Constants
            {
                public static GUIContent badgeNew = EditorGUIUtility.IconContent("AS Badge New");
                public static Texture2D folderIcon = EditorGUIUtility.FindTexture(EditorResourcesUtility.folderIconName);
            }
        }

        private class PackageImportTreeViewItem : TreeViewItem
        {
            public PackageImportTreeViewItem(int id, int depth, TreeViewItem parent, string displayName) : base(id, depth, parent, displayName)
            {
                this.item = this.item;
            }

            public bool isFolder { get; set; }

            public AssetsItem item { get; set; }
        }

        private class PreviewPopup : PopupWindowContent
        {
            private readonly Vector2 kPreviewSize = new Vector2(128f, 128f);
            private readonly Texture2D m_Preview;

            public PreviewPopup(Texture2D preview)
            {
                this.m_Preview = preview;
            }

            public override Vector2 GetWindowSize()
            {
                return this.kPreviewSize;
            }

            public override void OnGUI(Rect rect)
            {
                PackageImport.DrawTexture(rect, this.m_Preview, false);
            }
        }
    }
}

