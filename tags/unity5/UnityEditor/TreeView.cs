namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditorInternal;
    using UnityEngine;

    internal class TreeView
    {
        [CompilerGenerated]
        private static Func<TreeViewItem, int> <>f__am$cache16;
        [CompilerGenerated]
        private static Func<TreeViewItem, int> <>f__am$cache17;
        private const float kSpaceForScrollBar = 16f;
        private bool m_AllowRenameOnMouseUp = true;
        private List<int> m_DragSelection = new List<int>();
        private EditorWindow m_EditorWindow;
        private bool m_GrabKeyboardFocus;
        private bool m_HadFocusLastEvent;
        private int m_KeyboardControlID;
        private Rect m_TotalRect;
        private bool m_UseScrollView = true;

        public TreeView(EditorWindow editorWindow, TreeViewState treeViewState)
        {
            this.m_EditorWindow = editorWindow;
            this.state = treeViewState;
        }

        public bool BeginNameEditing(float delay)
        {
            if (this.state.selectedIDs.Count == 0)
            {
                return false;
            }
            List<TreeViewItem> visibleRows = this.data.GetVisibleRows();
            TreeViewItem item = null;
            <BeginNameEditing>c__AnonStorey6E storeye = new <BeginNameEditing>c__AnonStorey6E();
            using (List<int>.Enumerator enumerator = this.state.selectedIDs.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    storeye.id = enumerator.Current;
                    TreeViewItem item2 = visibleRows.Find(new Predicate<TreeViewItem>(storeye.<>m__F9));
                    if (item == null)
                    {
                        item = item2;
                    }
                    else if (item2 != null)
                    {
                        return false;
                    }
                }
            }
            return (((item != null) && this.data.IsRenamingItemAllowed(item)) && this.gui.BeginRename(item, delay));
        }

        public void EndNameEditing(bool acceptChanges)
        {
            if (this.state.renameOverlay.IsRenaming())
            {
                this.state.renameOverlay.EndRename(acceptChanges);
                this.gui.EndRename();
            }
        }

        public void EndPing()
        {
            this.gui.EndPingNode();
        }

        private float EnsureRowIsVisible(int row, List<TreeViewItem> rows)
        {
            float max = -1f;
            if (row >= 0)
            {
                max = this.gui.GetTopPixelOfRow(row, rows);
                float min = (max - this.m_TotalRect.height) + this.gui.GetHeightOfLastRow();
                this.state.scrollPos.y = Mathf.Clamp(this.state.scrollPos.y, min, max);
                return max;
            }
            return max;
        }

        public TreeViewItem FindNode(int id)
        {
            return this.data.FindItem(id);
        }

        public void Frame(int id, bool frame, bool ping)
        {
            float topPixelOfRow = -1f;
            TreeViewItem item = null;
            if (frame)
            {
                this.RevealNode(id);
                List<TreeViewItem> visibleRows = this.data.GetVisibleRows();
                int indexOfID = GetIndexOfID(visibleRows, id);
                if (indexOfID >= 0)
                {
                    item = visibleRows[indexOfID];
                    topPixelOfRow = this.gui.GetTopPixelOfRow(indexOfID, visibleRows);
                    this.EnsureRowIsVisible(indexOfID, visibleRows);
                }
            }
            if (ping)
            {
                if (topPixelOfRow == -1f)
                {
                    List<TreeViewItem> items = this.data.GetVisibleRows();
                    int row = GetIndexOfID(items, id);
                    if (row >= 0)
                    {
                        item = items[row];
                        topPixelOfRow = this.gui.GetTopPixelOfRow(row, items);
                    }
                }
                if ((topPixelOfRow >= 0f) && (item != null))
                {
                    float num4 = (this.GetContentSize().y <= this.m_TotalRect.height) ? 0f : -16f;
                    this.gui.BeginPingNode(item, topPixelOfRow, this.m_TotalRect.width + num4);
                }
            }
        }

        public Vector2 GetContentSize()
        {
            return this.gui.GetTotalSize(this.data.GetVisibleRows());
        }

        private bool GetFirstAndLastSelected(List<TreeViewItem> items, out int firstIndex, out int lastIndex)
        {
            firstIndex = -1;
            lastIndex = -1;
            for (int i = 0; i < items.Count; i++)
            {
                if (this.state.selectedIDs.Contains(items[i].id))
                {
                    if (firstIndex == -1)
                    {
                        firstIndex = i;
                    }
                    lastIndex = i;
                }
            }
            return ((firstIndex != -1) && (lastIndex != -1));
        }

        internal static int GetIndexOfID(List<TreeViewItem> items, int id)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].id == id)
                {
                    return i;
                }
            }
            return -1;
        }

        internal static int GetItemControlID(TreeViewItem item)
        {
            return (((item == null) ? 0 : item.id) + 0x989680);
        }

        private List<int> GetNewSelection(TreeViewItem clickedItem, bool keepMultiSelection, bool useShiftAsActionKey)
        {
            List<TreeViewItem> visibleRows = this.data.GetVisibleRows();
            List<int> allInstanceIDs = new List<int>(visibleRows.Count);
            for (int i = 0; i < visibleRows.Count; i++)
            {
                allInstanceIDs.Add(visibleRows[i].id);
            }
            List<int> selectedIDs = this.state.selectedIDs;
            int lastClickedID = this.state.lastClickedID;
            bool allowMultiSelection = this.data.CanBeMultiSelected(clickedItem);
            return InternalEditorUtility.GetNewSelection(clickedItem.id, allInstanceIDs, selectedIDs, lastClickedID, keepMultiSelection, useShiftAsActionKey, allowMultiSelection);
        }

        public int[] GetSelection()
        {
            return this.state.selectedIDs.ToArray();
        }

        public Rect GetTotalRect()
        {
            return this.m_TotalRect;
        }

        public int[] GetVisibleRowIDs()
        {
            if (<>f__am$cache16 == null)
            {
                <>f__am$cache16 = item => item.id;
            }
            return this.data.GetVisibleRows().Select<TreeViewItem, int>(<>f__am$cache16).ToArray<int>();
        }

        public void GrabKeyboardFocus()
        {
            this.m_GrabKeyboardFocus = true;
        }

        private void HandleUnusedEvents()
        {
            EventType type = Event.current.type;
            switch (type)
            {
                case EventType.DragUpdated:
                    if ((this.dragging != null) && this.m_TotalRect.Contains(Event.current.mousePosition))
                    {
                        this.dragging.DragElement(null, new Rect(), false);
                        this.Repaint();
                        Event.current.Use();
                    }
                    return;

                case EventType.DragPerform:
                    if ((this.dragging != null) && this.m_TotalRect.Contains(Event.current.mousePosition))
                    {
                        this.m_DragSelection.Clear();
                        this.dragging.DragElement(null, new Rect(), false);
                        this.Repaint();
                        Event.current.Use();
                    }
                    return;

                case EventType.DragExited:
                    if (this.dragging != null)
                    {
                        this.m_DragSelection.Clear();
                        this.dragging.DragCleanup(true);
                        this.Repaint();
                    }
                    return;

                case EventType.ContextClick:
                    if (this.m_TotalRect.Contains(Event.current.mousePosition) && (this.contextClickOutsideItemsCallback != null))
                    {
                        this.contextClickOutsideItemsCallback();
                    }
                    return;
            }
            if ((type == EventType.MouseDown) && ((this.deselectOnUnhandledMouseDown && (Event.current.button == 0)) && (this.m_TotalRect.Contains(Event.current.mousePosition) && (this.state.selectedIDs.Count > 0))))
            {
                this.SetSelection(new int[0], false);
                this.NotifyListenersThatSelectionChanged();
            }
        }

        private void HandleUnusedMouseEventsForNode(Rect rect, TreeViewItem item, bool firstItem)
        {
            int itemControlID = GetItemControlID(item);
            Event current = Event.current;
            EventType typeForControl = current.GetTypeForControl(itemControlID);
            switch (typeForControl)
            {
                case EventType.MouseDown:
                    if (!rect.Contains(Event.current.mousePosition))
                    {
                        return;
                    }
                    if (Event.current.button != 0)
                    {
                        if (Event.current.button == 1)
                        {
                            bool keepMultiSelection = true;
                            this.SelectionClick(item, keepMultiSelection);
                        }
                        return;
                    }
                    GUIUtility.keyboardControl = this.m_KeyboardControlID;
                    this.Repaint();
                    if (Event.current.clickCount != 2)
                    {
                        this.m_DragSelection = this.GetNewSelection(item, true, false);
                        GUIUtility.hotControl = itemControlID;
                        DragAndDropDelay stateObject = (DragAndDropDelay) GUIUtility.GetStateObject(typeof(DragAndDropDelay), itemControlID);
                        stateObject.mouseDownPosition = Event.current.mousePosition;
                    }
                    else if (this.itemDoubleClickedCallback != null)
                    {
                        this.itemDoubleClickedCallback(item.id);
                    }
                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == itemControlID)
                    {
                        GUIUtility.hotControl = 0;
                        this.m_DragSelection.Clear();
                        current.Use();
                        if (rect.Contains(current.mousePosition))
                        {
                            float contentIndent = this.gui.GetContentIndent(item);
                            Rect rect2 = new Rect(rect.x + contentIndent, rect.y, rect.width - contentIndent, rect.height);
                            List<int> selectedIDs = this.state.selectedIDs;
                            if (((!this.m_AllowRenameOnMouseUp || (selectedIDs == null)) || ((selectedIDs.Count != 1) || (selectedIDs[0] != item.id))) || (!rect2.Contains(current.mousePosition) || EditorGUIUtility.HasHolddownKeyModifiers(current)))
                            {
                                this.SelectionClick(item, false);
                                return;
                            }
                            this.BeginNameEditing(0.5f);
                        }
                    }
                    return;

                case EventType.MouseDrag:
                    if ((GUIUtility.hotControl == itemControlID) && (this.dragging != null))
                    {
                        DragAndDropDelay delay2 = (DragAndDropDelay) GUIUtility.GetStateObject(typeof(DragAndDropDelay), itemControlID);
                        if (delay2.CanStartDrag())
                        {
                            this.dragging.StartDrag(item, this.m_DragSelection);
                            GUIUtility.hotControl = 0;
                        }
                        current.Use();
                    }
                    return;

                default:
                    switch (typeForControl)
                    {
                        case EventType.DragUpdated:
                        case EventType.DragPerform:
                            if ((this.dragging != null) && this.dragging.DragElement(item, rect, firstItem))
                            {
                                GUIUtility.hotControl = 0;
                            }
                            return;

                        case EventType.ContextClick:
                            if (rect.Contains(current.mousePosition) && (this.contextClickItemCallback != null))
                            {
                                this.contextClickItemCallback(item.id);
                            }
                            return;

                        default:
                            return;
                    }
                    break;
            }
            current.Use();
        }

        public bool HasFocus()
        {
            return ((this.m_EditorWindow != null) && (this.m_EditorWindow.m_Parent.hasFocus && (GUIUtility.keyboardControl == this.m_KeyboardControlID)));
        }

        public bool HasSelection()
        {
            return (this.state.selectedIDs.Count<int>() > 0);
        }

        public void Init(Rect rect, ITreeViewDataSource data, ITreeViewGUI gui, ITreeViewDragging dragging)
        {
            this.data = data;
            this.gui = gui;
            this.dragging = dragging;
            this.m_TotalRect = rect;
        }

        public bool IsLastClickedPartOfVisibleRows()
        {
            List<TreeViewItem> visibleRows = this.data.GetVisibleRows();
            if (visibleRows.Count == 0)
            {
                return false;
            }
            return (GetIndexOfID(visibleRows, this.state.lastClickedID) >= 0);
        }

        private bool IsSameAsCurrentSelection(int[] selectedIDs)
        {
            if (selectedIDs.Length != this.state.selectedIDs.Count)
            {
                return false;
            }
            for (int i = 0; i < selectedIDs.Length; i++)
            {
                if (selectedIDs[i] != this.state.selectedIDs[i])
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsSelected(int id)
        {
            return this.state.selectedIDs.Contains(id);
        }

        public bool IsVisible(int id)
        {
            return (GetIndexOfID(this.data.GetVisibleRows(), id) >= 0);
        }

        private void KeyboardGUI()
        {
            if ((this.m_KeyboardControlID == GUIUtility.keyboardControl) && GUI.enabled)
            {
                if (this.keyboardInputCallback != null)
                {
                    this.keyboardInputCallback();
                }
                if (Event.current.type == EventType.KeyDown)
                {
                    switch (Event.current.keyCode)
                    {
                        case KeyCode.KeypadEnter:
                        case KeyCode.Return:
                            if ((Application.platform == RuntimePlatform.OSXEditor) && this.BeginNameEditing(0f))
                            {
                                Event.current.Use();
                            }
                            return;

                        case KeyCode.UpArrow:
                            Event.current.Use();
                            this.OffsetSelection(-1);
                            return;

                        case KeyCode.DownArrow:
                            Event.current.Use();
                            this.OffsetSelection(1);
                            return;

                        case KeyCode.RightArrow:
                            foreach (int num2 in this.state.selectedIDs)
                            {
                                TreeViewItem item2 = this.data.FindItem(num2);
                                if (item2 != null)
                                {
                                    if (this.data.IsExpandable(item2) && !this.data.IsExpanded(item2))
                                    {
                                        if (Event.current.alt)
                                        {
                                            this.data.SetExpandedWithChildren(item2, true);
                                        }
                                        else
                                        {
                                            this.data.SetExpanded(item2, true);
                                        }
                                        this.UserExpandedNode(item2);
                                    }
                                    else if (item2.hasChildren && (this.state.selectedIDs.Count == 1))
                                    {
                                        this.SelectionClick(item2.children[0], false);
                                    }
                                }
                            }
                            Event.current.Use();
                            return;

                        case KeyCode.LeftArrow:
                            foreach (int num in this.state.selectedIDs)
                            {
                                TreeViewItem item = this.data.FindItem(num);
                                if (item != null)
                                {
                                    if (this.data.IsExpandable(item) && this.data.IsExpanded(item))
                                    {
                                        if (Event.current.alt)
                                        {
                                            this.data.SetExpandedWithChildren(item, false);
                                        }
                                        else
                                        {
                                            this.data.SetExpanded(item, false);
                                        }
                                    }
                                    else if (((item.parent != null) && (this.state.selectedIDs.Count == 1)) && this.data.GetVisibleRows().Contains(item.parent))
                                    {
                                        this.SelectionClick(item.parent, false);
                                    }
                                }
                            }
                            Event.current.Use();
                            return;

                        case KeyCode.Home:
                            Event.current.Use();
                            this.OffsetSelection(-1000000);
                            return;

                        case KeyCode.End:
                            Event.current.Use();
                            this.OffsetSelection(0xf4240);
                            return;

                        case KeyCode.PageUp:
                        {
                            Event.current.Use();
                            TreeViewItem fromItem = this.data.FindItem(this.state.lastClickedID);
                            if (fromItem != null)
                            {
                                int num3 = this.gui.GetNumRowsOnPageUpDown(fromItem, true, this.m_TotalRect.height);
                                this.OffsetSelection(-num3);
                            }
                            return;
                        }
                        case KeyCode.PageDown:
                        {
                            Event.current.Use();
                            TreeViewItem item4 = this.data.FindItem(this.state.lastClickedID);
                            if (item4 != null)
                            {
                                int offset = this.gui.GetNumRowsOnPageUpDown(item4, true, this.m_TotalRect.height);
                                this.OffsetSelection(offset);
                            }
                            return;
                        }
                        case KeyCode.F2:
                            if ((Application.platform == RuntimePlatform.WindowsEditor) && this.BeginNameEditing(0f))
                            {
                                Event.current.Use();
                            }
                            return;
                    }
                    if ((Event.current.keyCode > KeyCode.A) && (Event.current.keyCode < KeyCode.Z))
                    {
                    }
                }
            }
        }

        public void NotifyListenersThatDragEnded(int[] draggedIDs, bool draggedItemsFromOwnTreeView)
        {
            if (this.dragEndedCallback != null)
            {
                this.dragEndedCallback(draggedIDs, draggedItemsFromOwnTreeView);
            }
        }

        public void NotifyListenersThatSelectionChanged()
        {
            if (this.selectionChangedCallback != null)
            {
                this.selectionChangedCallback(this.state.selectedIDs.ToArray());
            }
        }

        public void OffsetSelection(int offset)
        {
            List<TreeViewItem> visibleRows = this.data.GetVisibleRows();
            if (visibleRows.Count != 0)
            {
                Event.current.Use();
                int row = Mathf.Clamp(GetIndexOfID(visibleRows, this.state.lastClickedID) + offset, 0, visibleRows.Count - 1);
                this.EnsureRowIsVisible(row, visibleRows);
                this.SelectionByKey(visibleRows[row]);
            }
        }

        public void OnEvent()
        {
            this.state.renameOverlay.OnEvent();
        }

        public void OnGUI(Rect rect, int keyboardControlID)
        {
            int num;
            int num2;
            this.m_TotalRect = rect;
            this.m_KeyboardControlID = keyboardControlID;
            Event current = Event.current;
            if (this.m_GrabKeyboardFocus || ((current.type == EventType.MouseDown) && this.m_TotalRect.Contains(current.mousePosition)))
            {
                this.m_GrabKeyboardFocus = false;
                GUIUtility.keyboardControl = this.m_KeyboardControlID;
                this.m_AllowRenameOnMouseUp = true;
                this.Repaint();
            }
            bool focused = this.HasFocus();
            if ((focused != this.m_HadFocusLastEvent) && (current.type != EventType.Layout))
            {
                this.m_HadFocusLastEvent = focused;
                if (focused && (current.type == EventType.MouseDown))
                {
                    this.m_AllowRenameOnMouseUp = false;
                }
            }
            List<TreeViewItem> visibleRows = this.data.GetVisibleRows();
            Vector2 totalSize = this.gui.GetTotalSize(visibleRows);
            Rect viewRect = new Rect(0f, 0f, totalSize.x, totalSize.y);
            if (this.m_UseScrollView)
            {
                this.state.scrollPos = GUI.BeginScrollView(this.m_TotalRect, this.state.scrollPos, viewRect);
            }
            this.gui.BeginRowGUI();
            float topPixel = !this.m_UseScrollView ? 0f : this.state.scrollPos.y;
            this.gui.GetFirstAndLastRowVisible(visibleRows, topPixel, this.m_TotalRect.height, out num, out num2);
            for (int i = num; i <= num2; i++)
            {
                TreeViewItem item = visibleRows[i];
                bool selected = (this.m_DragSelection.Count <= 0) ? this.state.selectedIDs.Contains(item.id) : this.m_DragSelection.Contains(item.id);
                Rect rect3 = this.gui.OnRowGUI(item, i, Mathf.Max(GUIClip.visibleRect.width, viewRect.width), selected, focused);
                if (this.onGUIRowCallback != null)
                {
                    float contentIndent = this.gui.GetContentIndent(item);
                    Rect rect4 = new Rect(rect3.x + contentIndent, rect3.y, rect3.width - contentIndent, rect3.height);
                    this.onGUIRowCallback(item.id, rect4);
                }
                this.HandleUnusedMouseEventsForNode(rect3, visibleRows[i], i == 0);
            }
            this.gui.EndRowGUI();
            if (this.m_UseScrollView)
            {
                GUI.EndScrollView();
            }
            this.HandleUnusedEvents();
            this.KeyboardGUI();
        }

        public void ReloadData()
        {
            this.data.ReloadData();
            this.Repaint();
        }

        public void RemoveSelection()
        {
            if (this.state.selectedIDs.Count > 0)
            {
                this.state.selectedIDs.Clear();
                this.NotifyListenersThatSelectionChanged();
            }
        }

        public void Repaint()
        {
            if (this.m_EditorWindow != null)
            {
                this.m_EditorWindow.Repaint();
            }
        }

        private void RevealNode(int id)
        {
            if (!this.IsVisible(id))
            {
                TreeViewItem item = this.FindNode(id);
                if (item != null)
                {
                    for (TreeViewItem item2 = item.parent; item2 != null; item2 = item2.parent)
                    {
                        this.data.SetExpanded(item2, true);
                    }
                }
            }
        }

        private void SelectionByKey(TreeViewItem itemSelected)
        {
            this.state.selectedIDs = this.GetNewSelection(itemSelected, false, true);
            this.state.lastClickedID = itemSelected.id;
            this.NotifyListenersThatSelectionChanged();
        }

        private void SelectionClick(TreeViewItem itemClicked, bool keepMultiSelection)
        {
            this.state.selectedIDs = this.GetNewSelection(itemClicked, keepMultiSelection, false);
            this.state.lastClickedID = (itemClicked == null) ? 0 : itemClicked.id;
            this.NotifyListenersThatSelectionChanged();
        }

        public void SetSelection(int[] selectedIDs, bool revealSelectionAndFrameLastSelected)
        {
            <SetSelection>c__AnonStorey6D storeyd = new <SetSelection>c__AnonStorey6D {
                selectedIDs = selectedIDs
            };
            if (storeyd.selectedIDs.Length > 0)
            {
                if (revealSelectionAndFrameLastSelected)
                {
                    foreach (int num in storeyd.selectedIDs)
                    {
                        this.RevealNode(num);
                    }
                }
                this.state.selectedIDs = new List<int>(storeyd.selectedIDs);
                if (this.state.selectedIDs.IndexOf(this.state.lastClickedID) < 0)
                {
                    if (<>f__am$cache17 == null)
                    {
                        <>f__am$cache17 = item => item.id;
                    }
                    List<int> list2 = this.data.GetVisibleRows().Where<TreeViewItem>(new Func<TreeViewItem, bool>(storeyd.<>m__F7)).Select<TreeViewItem, int>(<>f__am$cache17).ToList<int>();
                    if (list2.Count > 0)
                    {
                        this.state.lastClickedID = list2[list2.Count - 1];
                    }
                    else
                    {
                        this.state.lastClickedID = 0;
                    }
                }
                if (revealSelectionAndFrameLastSelected)
                {
                    this.Frame(this.state.lastClickedID, true, false);
                }
            }
            else
            {
                this.state.selectedIDs.Clear();
                this.state.lastClickedID = 0;
            }
        }

        public void SetUseScrollView(bool useScrollView)
        {
            this.m_UseScrollView = useScrollView;
        }

        public List<int> SortIDsInVisiblityOrder(List<int> ids)
        {
            if (ids.Count <= 1)
            {
                return ids;
            }
            List<TreeViewItem> visibleRows = this.data.GetVisibleRows();
            List<int> second = new List<int>();
            for (int i = 0; i < visibleRows.Count; i++)
            {
                int id = visibleRows[i].id;
                for (int j = 0; j < ids.Count; j++)
                {
                    if (ids[j] == id)
                    {
                        second.Add(id);
                        break;
                    }
                }
            }
            if (ids.Count != second.Count)
            {
                second.AddRange(ids.Except<int>(second));
                if (ids.Count != second.Count)
                {
                    Debug.LogError(string.Concat(new object[] { "SortIDsInVisiblityOrder failed: ", ids.Count, " != ", second.Count }));
                }
            }
            return second;
        }

        public void UserExpandedNode(TreeViewItem item)
        {
        }

        public Action<int> contextClickItemCallback { get; set; }

        public System.Action contextClickOutsideItemsCallback { get; set; }

        public ITreeViewDataSource data { get; set; }

        public bool deselectOnUnhandledMouseDown { get; set; }

        public Action<int[], bool> dragEndedCallback { get; set; }

        public ITreeViewDragging dragging { get; set; }

        public System.Action expandedStateChanged { get; set; }

        public ITreeViewGUI gui { get; set; }

        public bool isSearching
        {
            get
            {
                return !string.IsNullOrEmpty(this.state.searchString);
            }
        }

        public Action<int> itemDoubleClickedCallback { get; set; }

        public System.Action keyboardInputCallback { get; set; }

        public Action<int, Rect> onGUIRowCallback { get; set; }

        public Action<string> searchChanged { get; set; }

        public string searchString
        {
            get
            {
                return this.state.searchString;
            }
            set
            {
                this.state.searchString = value;
                this.data.OnSearchChanged();
                if (this.searchChanged != null)
                {
                    this.searchChanged(this.state.searchString);
                }
            }
        }

        public Action<int[]> selectionChangedCallback { get; set; }

        public TreeViewState state { get; set; }

        [CompilerGenerated]
        private sealed class <BeginNameEditing>c__AnonStorey6E
        {
            internal int id;

            internal bool <>m__F9(TreeViewItem i)
            {
                return (i.id == this.id);
            }
        }

        [CompilerGenerated]
        private sealed class <SetSelection>c__AnonStorey6D
        {
            internal int[] selectedIDs;

            internal bool <>m__F7(TreeViewItem item)
            {
                return this.selectedIDs.Contains<int>(item.id);
            }
        }
    }
}

