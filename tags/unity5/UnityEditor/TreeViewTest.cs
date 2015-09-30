namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    internal class TreeViewTest
    {
        private BackendData m_BackendData;
        private TreeViewColumnHeader m_ColumnHeader;
        private EditorWindow m_EditorWindow;
        private GUIStyle m_HeaderStyle;
        private GUIStyle m_HeaderStyleRightAligned;
        private bool m_Lazy;
        private TreeView m_TreeView;

        public TreeViewTest(EditorWindow editorWindow, bool lazy)
        {
            this.m_EditorWindow = editorWindow;
            this.m_Lazy = lazy;
        }

        private string GetHeader()
        {
            object[] objArray1 = new object[] { !this.m_Lazy ? "FULL: " : "LAZY: ", "GUI items: ", this.GetNumItemsInTree(), "  (data items: ", this.GetNumItemsInData(), ")" };
            return string.Concat(objArray1);
        }

        public int GetNumItemsInData()
        {
            return this.m_BackendData.IDCounter;
        }

        public int GetNumItemsInTree()
        {
            LazyTestDataSource data = this.m_TreeView.data as LazyTestDataSource;
            if (data != null)
            {
                return data.itemCounter;
            }
            TestDataSource source2 = this.m_TreeView.data as TestDataSource;
            if (source2 != null)
            {
                return source2.itemCounter;
            }
            return -1;
        }

        public void Init(Rect rect, BackendData backendData)
        {
            if (this.m_TreeView == null)
            {
                ITreeViewDataSource source;
                this.m_BackendData = backendData;
                TreeViewState treeViewState = new TreeViewState {
                    columnWidths = new float[] { 250f, 90f, 93f, 98f, 74f, 78f }
                };
                this.m_TreeView = new TreeView(this.m_EditorWindow, treeViewState);
                ITreeViewGUI gui = new TestGUI(this.m_TreeView);
                ITreeViewDragging dragging = new TestDragging(this.m_TreeView, this.m_BackendData);
                if (this.m_Lazy)
                {
                    source = new LazyTestDataSource(this.m_TreeView, this.m_BackendData);
                }
                else
                {
                    source = new TestDataSource(this.m_TreeView, this.m_BackendData);
                }
                this.m_TreeView.Init(rect, source, gui, dragging);
                this.m_ColumnHeader = new TreeViewColumnHeader();
                this.m_ColumnHeader.columnWidths = treeViewState.columnWidths;
                this.m_ColumnHeader.minColumnWidth = 30f;
                this.m_ColumnHeader.columnRenderer = (Action<int, Rect>) Delegate.Combine(this.m_ColumnHeader.columnRenderer, new Action<int, Rect>(this.OnColumnRenderer));
            }
        }

        private void OnColumnRenderer(int column, Rect rect)
        {
            if (this.m_HeaderStyle == null)
            {
                this.m_HeaderStyle = new GUIStyle(EditorStyles.toolbarButton);
                this.m_HeaderStyle.padding.left = 4;
                this.m_HeaderStyle.alignment = TextAnchor.MiddleLeft;
                this.m_HeaderStyleRightAligned = new GUIStyle(EditorStyles.toolbarButton);
                this.m_HeaderStyleRightAligned.padding.right = 4;
                this.m_HeaderStyleRightAligned.alignment = TextAnchor.MiddleRight;
            }
            string[] strArray = new string[] { "Name", "Date Modified", "Size", "Kind", "Author", "Platform", "Faster", "Slower" };
            GUI.Label(rect, strArray[column], ((column % 2) != 0) ? this.m_HeaderStyleRightAligned : this.m_HeaderStyle);
        }

        public void OnGUI(Rect rect)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Keyboard, rect);
            Rect position = new Rect(rect.x, rect.y, rect.width, 17f);
            Rect screenRect = new Rect(rect.x, rect.yMax - 20f, rect.width, 20f);
            GUI.Label(position, string.Empty, EditorStyles.toolbar);
            this.m_ColumnHeader.OnGUI(position);
            Profiler.BeginSample("TREEVIEW");
            rect.y += position.height;
            rect.height -= position.height + screenRect.height;
            this.m_TreeView.OnEvent();
            this.m_TreeView.OnGUI(rect, controlID);
            Profiler.EndSample();
            GUILayout.BeginArea(screenRect, this.GetHeader(), EditorStyles.helpBox);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            this.m_BackendData.m_RecursiveFindParentsBelow = GUILayout.Toggle(this.m_BackendData.m_RecursiveFindParentsBelow, GUIContent.Temp("Recursive"), new GUILayoutOption[0]);
            if (GUILayout.Button("Ping", EditorStyles.miniButton, new GUILayoutOption[0]))
            {
                int id = this.GetNumItemsInData() / 2;
                this.m_TreeView.Frame(id, true, true);
                int[] selectedIDs = new int[] { id };
                this.m_TreeView.SetSelection(selectedIDs, false);
            }
            if (GUILayout.Button("Frame", EditorStyles.miniButton, new GUILayoutOption[0]))
            {
                int num5 = this.GetNumItemsInData() / 10;
                this.m_TreeView.Frame(num5, true, false);
                int[] numArray2 = new int[] { num5 };
                this.m_TreeView.SetSelection(numArray2, false);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        public class BackendData
        {
            private const int k_MaxChildren = 15;
            private const int k_MaxDepth = 12;
            private const int k_MinChildren = 3;
            private const float k_ProbOfLastDescendent = 0.5f;
            private int m_MaxItems = 0x2710;
            public bool m_RecursiveFindParentsBelow = true;
            private Foo m_Root;

            private void AddChildrenRecursive(Foo foo, int numChildren, bool force)
            {
                if (((this.IDCounter <= this.m_MaxItems) && (foo.depth < 12)) && (force || (UnityEngine.Random.value >= 0.5f)))
                {
                    if (foo.children == null)
                    {
                        foo.children = new List<Foo>(numChildren);
                    }
                    for (int i = 0; i < numChildren; i++)
                    {
                        int num2;
                        this.IDCounter = num2 = this.IDCounter + 1;
                        Foo item = new Foo("Tud" + this.IDCounter, foo.depth + 1, num2) {
                            parent = foo
                        };
                        foo.children.Add(item);
                    }
                    if (this.IDCounter <= this.m_MaxItems)
                    {
                        foreach (Foo foo3 in foo.children)
                        {
                            this.AddChildrenRecursive(foo3, UnityEngine.Random.Range(3, 15), false);
                        }
                    }
                }
            }

            public static Foo FindNodeRecursive(Foo item, int id)
            {
                if (item != null)
                {
                    if (item.id == id)
                    {
                        return item;
                    }
                    if (item.children == null)
                    {
                        return null;
                    }
                    foreach (Foo foo in item.children)
                    {
                        Foo foo2 = FindNodeRecursive(foo, id);
                        if (foo2 != null)
                        {
                            return foo2;
                        }
                    }
                }
                return null;
            }

            public void GenerateData(int maxNumItems)
            {
                this.m_MaxItems = maxNumItems;
                this.IDCounter = 1;
                this.m_Root = new Foo("Root", 0, 0);
                while (this.IDCounter < this.m_MaxItems)
                {
                    this.AddChildrenRecursive(this.m_Root, UnityEngine.Random.Range(3, 15), true);
                }
            }

            public HashSet<int> GetParentsBelow(int id)
            {
                Foo searchFromThis = FindNodeRecursive(this.root, id);
                if (searchFromThis == null)
                {
                    return new HashSet<int>();
                }
                if (this.m_RecursiveFindParentsBelow)
                {
                    return this.GetParentsBelowRecursive(searchFromThis);
                }
                return this.GetParentsBelowStackBased(searchFromThis);
            }

            private HashSet<int> GetParentsBelowRecursive(Foo searchFromThis)
            {
                HashSet<int> parentIDs = new HashSet<int>();
                GetParentsBelowRecursive(searchFromThis, parentIDs);
                return parentIDs;
            }

            private static void GetParentsBelowRecursive(Foo item, HashSet<int> parentIDs)
            {
                if (item.hasChildren)
                {
                    parentIDs.Add(item.id);
                    foreach (Foo foo in item.children)
                    {
                        GetParentsBelowRecursive(foo, parentIDs);
                    }
                }
            }

            private HashSet<int> GetParentsBelowStackBased(Foo searchFromThis)
            {
                Stack<Foo> stack = new Stack<Foo>();
                stack.Push(searchFromThis);
                HashSet<int> set = new HashSet<int>();
                while (stack.Count > 0)
                {
                    Foo foo = stack.Pop();
                    if (foo.hasChildren)
                    {
                        set.Add(foo.id);
                        foreach (Foo foo2 in foo.children)
                        {
                            stack.Push(foo2);
                        }
                    }
                }
                return set;
            }

            public void ReparentSelection(Foo parentItem, Foo insertAfterItem, List<Foo> draggedItems)
            {
                foreach (Foo foo in draggedItems)
                {
                    foo.parent.children.Remove(foo);
                    foo.parent = parentItem;
                }
                if (!parentItem.hasChildren)
                {
                    parentItem.children = new List<Foo>();
                }
                List<Foo> list = new List<Foo>(parentItem.children);
                int index = 0;
                if (parentItem == insertAfterItem)
                {
                    index = 0;
                }
                else
                {
                    int num2 = parentItem.children.IndexOf(insertAfterItem);
                    if (num2 >= 0)
                    {
                        index = num2 + 1;
                    }
                    else
                    {
                        Debug.LogError("Did not find insertAfterItem, should be a child of parentItem!!");
                    }
                }
                list.InsertRange(index, draggedItems);
                parentItem.children = list;
            }

            public int IDCounter { get; private set; }

            public Foo root
            {
                get
                {
                    return this.m_Root;
                }
            }

            public class Foo
            {
                public Foo(string name, int depth, int id)
                {
                    this.name = name;
                    this.depth = depth;
                    this.id = id;
                }

                public List<TreeViewTest.BackendData.Foo> children { get; set; }

                public int depth { get; set; }

                public bool hasChildren
                {
                    get
                    {
                        return ((this.children != null) && (this.children.Count > 0));
                    }
                }

                public int id { get; set; }

                public string name { get; set; }

                public TreeViewTest.BackendData.Foo parent { get; set; }
            }
        }

        public class FooTreeViewItem : TreeViewItem
        {
            public FooTreeViewItem(int id, int depth, TreeViewItem parent, string displayName, TreeViewTest.BackendData.Foo foo) : base(id, depth, parent, displayName)
            {
                this.foo = foo;
            }

            public TreeViewTest.BackendData.Foo foo { get; private set; }
        }

        private class LazyTestDataSource : LazyTreeViewDataSource
        {
            private TreeViewTest.BackendData m_Backend;

            public LazyTestDataSource(TreeView treeView, TreeViewTest.BackendData data) : base(treeView)
            {
                this.m_Backend = data;
                this.FetchData();
            }

            private void AddVisibleChildrenRecursive(TreeViewTest.BackendData.Foo source, TreeViewItem dest)
            {
                if (this.IsExpanded(source.id))
                {
                    if ((source.children != null) && (source.children.Count > 0))
                    {
                        dest.children = new List<TreeViewItem>(source.children.Count);
                        for (int i = 0; i < source.children.Count; i++)
                        {
                            TreeViewTest.BackendData.Foo foo = source.children[i];
                            dest.children[i] = new TreeViewTest.FooTreeViewItem(foo.id, dest.depth + 1, dest, foo.name, foo);
                            this.itemCounter++;
                            this.AddVisibleChildrenRecursive(foo, dest.children[i]);
                        }
                    }
                }
                else if (source.hasChildren)
                {
                    dest.children = new List<TreeViewItem> { new TreeViewItem(-1, -1, null, string.Empty) };
                }
            }

            public override bool CanBeParent(TreeViewItem item)
            {
                return item.hasChildren;
            }

            public override void FetchData()
            {
                this.itemCounter = 1;
                base.m_RootItem = new TreeViewTest.FooTreeViewItem(this.m_Backend.root.id, 0, null, this.m_Backend.root.name, this.m_Backend.root);
                this.AddVisibleChildrenRecursive(this.m_Backend.root, base.m_RootItem);
                base.m_VisibleRows = new List<TreeViewItem>();
                base.GetVisibleItemsRecursive(base.m_RootItem, base.m_VisibleRows);
                base.m_NeedRefreshVisibleFolders = false;
            }

            protected override HashSet<int> GetParentsAbove(int id)
            {
                HashSet<int> set = new HashSet<int>();
                for (TreeViewTest.BackendData.Foo foo = TreeViewTest.BackendData.FindNodeRecursive(this.m_Backend.root, id); foo != null; foo = foo.parent)
                {
                    if (foo.parent != null)
                    {
                        set.Add(foo.parent.id);
                    }
                }
                return set;
            }

            protected override HashSet<int> GetParentsBelow(int id)
            {
                return this.m_Backend.GetParentsBelow(id);
            }

            public int itemCounter { get; private set; }
        }

        private class TestDataSource : TreeViewDataSource
        {
            private TreeViewTest.BackendData m_Backend;

            public TestDataSource(TreeView treeView, TreeViewTest.BackendData data) : base(treeView)
            {
                this.m_Backend = data;
                this.FetchData();
            }

            private void AddChildrenRecursive(TreeViewTest.BackendData.Foo source, TreeViewItem dest)
            {
                if (source.hasChildren)
                {
                    dest.children = new List<TreeViewItem>(source.children.Count);
                    for (int i = 0; i < source.children.Count; i++)
                    {
                        TreeViewTest.BackendData.Foo foo = source.children[i];
                        dest.children[i] = new TreeViewTest.FooTreeViewItem(foo.id, dest.depth + 1, dest, foo.name, foo);
                        this.itemCounter++;
                        this.AddChildrenRecursive(foo, dest.children[i]);
                    }
                }
            }

            public override bool CanBeParent(TreeViewItem item)
            {
                return item.hasChildren;
            }

            public override void FetchData()
            {
                this.itemCounter = 1;
                base.m_RootItem = new TreeViewTest.FooTreeViewItem(this.m_Backend.root.id, 0, null, this.m_Backend.root.name, this.m_Backend.root);
                this.AddChildrenRecursive(this.m_Backend.root, base.m_RootItem);
                base.m_NeedRefreshVisibleFolders = true;
            }

            public int itemCounter { get; private set; }
        }

        public class TestDragging : TreeViewDragging
        {
            [CompilerGenerated]
            private static Func<TreeViewItem, bool> <>f__am$cache1;
            [CompilerGenerated]
            private static Func<TreeViewItem, TreeViewTest.BackendData.Foo> <>f__am$cache2;
            private const string k_GenericDragID = "FooDragging";
            private TreeViewTest.BackendData m_BackendData;

            public TestDragging(TreeView treeView, TreeViewTest.BackendData data) : base(treeView)
            {
                this.m_BackendData = data;
            }

            public override DragAndDropVisualMode DoDrag(TreeViewItem parentItem, TreeViewItem targetItem, bool perform, TreeViewDragging.DropPosition dropPos)
            {
                FooDragData genericData = DragAndDrop.GetGenericData("FooDragging") as FooDragData;
                TreeViewTest.FooTreeViewItem item = targetItem as TreeViewTest.FooTreeViewItem;
                TreeViewTest.FooTreeViewItem item2 = parentItem as TreeViewTest.FooTreeViewItem;
                if ((item2 == null) || (genericData == null))
                {
                    return DragAndDropVisualMode.None;
                }
                bool flag = this.ValidDrag(parentItem, genericData.m_DraggedItems);
                if (perform && flag)
                {
                    if (<>f__am$cache1 == null)
                    {
                        <>f__am$cache1 = x => x is TreeViewTest.FooTreeViewItem;
                    }
                    if (<>f__am$cache2 == null)
                    {
                        <>f__am$cache2 = x => ((TreeViewTest.FooTreeViewItem) x).foo;
                    }
                    List<TreeViewTest.BackendData.Foo> draggedItems = genericData.m_DraggedItems.Where<TreeViewItem>(<>f__am$cache1).Select<TreeViewItem, TreeViewTest.BackendData.Foo>(<>f__am$cache2).ToList<TreeViewTest.BackendData.Foo>();
                    this.m_BackendData.ReparentSelection(item2.foo, item.foo, draggedItems);
                    base.m_TreeView.ReloadData();
                }
                return (!flag ? DragAndDropVisualMode.None : DragAndDropVisualMode.Move);
            }

            private List<TreeViewItem> GetItemsFromIDs(IEnumerable<int> draggedItemIDs)
            {
                return TreeViewUtility.FindItemsInList(draggedItemIDs, base.m_TreeView.data.GetVisibleRows());
            }

            public override void StartDrag(TreeViewItem draggedNode, List<int> draggedItemIDs)
            {
                DragAndDrop.PrepareStartDrag();
                DragAndDrop.SetGenericData("FooDragging", new FooDragData(this.GetItemsFromIDs(draggedItemIDs)));
                DragAndDrop.objectReferences = new UnityEngine.Object[0];
                DragAndDrop.StartDrag(draggedItemIDs.Count + " Foo" + ((draggedItemIDs.Count <= 1) ? string.Empty : "s"));
            }

            private bool ValidDrag(TreeViewItem parent, List<TreeViewItem> draggedItems)
            {
                if (!parent.hasChildren)
                {
                    return false;
                }
                for (TreeViewItem item = parent; item != null; item = item.parent)
                {
                    if (draggedItems.Contains(item))
                    {
                        return false;
                    }
                }
                return true;
            }

            private class FooDragData
            {
                public List<TreeViewItem> m_DraggedItems;

                public FooDragData(List<TreeViewItem> draggedItems)
                {
                    this.m_DraggedItems = draggedItems;
                }
            }
        }

        private class TestGUI : TreeViewGUI
        {
            private Texture2D m_FolderIcon;
            private Texture2D m_Icon;
            private GUIStyle m_LabelStyle;
            private GUIStyle m_LabelStyleRightAlign;

            public TestGUI(TreeView treeView) : base(treeView)
            {
                this.m_FolderIcon = EditorGUIUtility.FindTexture(EditorResourcesUtility.folderIconName);
                this.m_Icon = EditorGUIUtility.FindTexture("boo Script Icon");
            }

            protected override void DrawIconAndLabel(Rect rect, TreeViewItem item, string label, bool selected, bool focused, bool useBoldFont, bool isPinging)
            {
                if (this.m_LabelStyle == null)
                {
                    this.m_LabelStyle = new GUIStyle(TreeViewGUI.s_Styles.lineStyle);
                    int num2 = 6;
                    this.m_LabelStyle.padding.right = num2;
                    this.m_LabelStyle.padding.left = num2;
                    this.m_LabelStyleRightAlign = new GUIStyle(TreeViewGUI.s_Styles.lineStyle);
                    num2 = 6;
                    this.m_LabelStyleRightAlign.padding.left = num2;
                    this.m_LabelStyleRightAlign.padding.right = num2;
                    this.m_LabelStyleRightAlign.alignment = TextAnchor.MiddleRight;
                }
                if ((isPinging || (this.columnWidths == null)) || (this.columnWidths.Length == 0))
                {
                    base.DrawIconAndLabel(rect, item, label, selected, focused, useBoldFont, isPinging);
                }
                else
                {
                    Rect rect2 = rect;
                    for (int i = 0; i < this.columnWidths.Length; i++)
                    {
                        rect2.width = this.columnWidths[i];
                        if (i == 0)
                        {
                            base.DrawIconAndLabel(rect2, item, label, selected, focused, useBoldFont, isPinging);
                        }
                        else
                        {
                            GUI.Label(rect2, "Zksdf SDFS DFASDF ", ((i % 2) != 0) ? this.m_LabelStyleRightAlign : this.m_LabelStyle);
                        }
                        rect2.x += rect2.width;
                    }
                }
            }

            protected override Texture GetIconForNode(TreeViewItem item)
            {
                return (!item.hasChildren ? this.m_Icon : this.m_FolderIcon);
            }

            protected override void RenameEnded()
            {
            }

            protected override void SyncFakeItem()
            {
            }

            private float[] columnWidths
            {
                get
                {
                    return base.m_TreeView.state.columnWidths;
                }
            }
        }

        internal class TreeViewColumnHeader
        {
            public TreeViewColumnHeader()
            {
                this.minColumnWidth = 10f;
                this.dragWidth = 6f;
            }

            public void OnGUI(Rect rect)
            {
                float x = rect.x;
                for (int i = 0; i < this.columnWidths.Length; i++)
                {
                    Rect rect2 = new Rect(x, rect.y, this.columnWidths[i], rect.height);
                    x += this.columnWidths[i];
                    Rect position = new Rect(x - (this.dragWidth / 2f), rect.y, 3f, rect.height);
                    float num4 = EditorGUI.MouseDeltaReader(position, true).x;
                    if (num4 != 0f)
                    {
                        this.columnWidths[i] += num4;
                        this.columnWidths[i] = Mathf.Max(this.columnWidths[i], this.minColumnWidth);
                    }
                    if (this.columnRenderer != null)
                    {
                        this.columnRenderer(i, rect2);
                    }
                    if (Event.current.type == EventType.Repaint)
                    {
                        EditorGUIUtility.AddCursorRect(position, MouseCursor.SplitResizeLeftRight);
                    }
                }
            }

            public Action<int, Rect> columnRenderer { get; set; }

            public float[] columnWidths { get; set; }

            public float dragWidth { get; set; }

            public float minColumnWidth { get; set; }
        }
    }
}

