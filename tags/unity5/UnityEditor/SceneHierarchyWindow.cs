namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    [EditorWindowTitle(title="Hierarchy", useTypeNameAsIconName=true)]
    internal class SceneHierarchyWindow : SearchableEditorWindow
    {
        private bool m_AllowAlphaNumericalSort;
        [SerializeField]
        private string m_CurrentSortMethod = string.Empty;
        [NonSerialized]
        private bool m_DidSelectSearchResult;
        [NonSerialized]
        private int m_LastFramedID = -1;
        private Dictionary<string, BaseHierarchySort> m_SortingObjects;
        private TreeView m_TreeView;
        private int m_TreeViewKeyboardControlID;
        [SerializeField]
        private TreeViewState m_TreeViewState;
        public static bool s_Debug = false;
        private static List<SceneHierarchyWindow> s_SceneHierarchyWindow = new List<SceneHierarchyWindow>();
        private static Styles s_Styles;
        private const float toolbarHeight = 17f;

        private void AddCreateGameObjectItemsToMenu(GenericMenu menu, UnityEngine.Object[] context, bool includeCreateEmptyChild)
        {
            foreach (string str in Unsupported.GetSubmenus("GameObject"))
            {
                UnityEngine.Object[] temporaryContext = context;
                if (includeCreateEmptyChild || (str.ToLower() != "GameObject/Create Empty Child".ToLower()))
                {
                    if (str.EndsWith("..."))
                    {
                        temporaryContext = null;
                    }
                    if (str.ToLower() == "GameObject/Center On Children".ToLower())
                    {
                        return;
                    }
                    MenuUtils.ExtractMenuItemWithPath(str, menu, str.Substring(11), temporaryContext);
                }
            }
        }

        private void Awake()
        {
            base.m_HierarchyType = HierarchyType.GameObjects;
        }

        private void CopyGO()
        {
            Unsupported.CopyGameObjectsToPasteboard();
        }

        private void CreateGameObjectPopup()
        {
            Rect position = GUILayoutUtility.GetRect(s_Styles.createContent, EditorStyles.toolbarDropDown, null);
            if (Event.current.type == EventType.Repaint)
            {
                EditorStyles.toolbarDropDown.Draw(position, s_Styles.createContent, false, false, false, false);
            }
            if ((Event.current.type == EventType.MouseDown) && position.Contains(Event.current.mousePosition))
            {
                GUIUtility.hotControl = 0;
                GenericMenu menu = new GenericMenu();
                this.AddCreateGameObjectItemsToMenu(menu, null, true);
                menu.DropDown(position);
                Event.current.Use();
            }
        }

        private void DeleteGO()
        {
            Unsupported.DeleteGameObjectSelection();
        }

        public void DirtySortingMethods()
        {
            this.m_AllowAlphaNumericalSort = EditorPrefs.GetBool("AllowAlphaNumericHierarchy", false);
            this.SetUpSortMethodLists();
            this.treeView.SetSelection(this.treeView.GetSelection(), true);
            this.treeView.ReloadData();
        }

        private float DoSearchResultPathGUI()
        {
            if (!base.hasSearchFilter)
            {
                return 0f;
            }
            GUILayout.FlexibleSpace();
            Rect rect = EditorGUILayout.BeginVertical(EditorStyles.inspectorBig, new GUILayoutOption[0]);
            GUILayout.Label("Path:", new GUILayoutOption[0]);
            if (this.m_TreeView.HasSelection())
            {
                int instanceID = this.m_TreeView.GetSelection()[0];
                IHierarchyProperty property = new HierarchyProperty(HierarchyType.GameObjects);
                property.Find(instanceID, null);
                if (property.isValid)
                {
                    do
                    {
                        EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                        GUILayout.Label(property.icon, new GUILayoutOption[0]);
                        GUILayout.Label(property.name, new GUILayoutOption[0]);
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndHorizontal();
                    }
                    while (property.Parent());
                }
            }
            EditorGUILayout.EndVertical();
            GUILayout.Space(0f);
            return rect.height;
        }

        private void DoToolbar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
            this.CreateGameObjectPopup();
            GUILayout.Space(6f);
            if (s_Debug)
            {
                GUILayout.Label(string.Empty + this.m_TreeView.data.GetVisibleRows().Count, EditorStyles.miniLabel, new GUILayoutOption[0]);
                GUILayout.Space(6f);
            }
            GUILayout.FlexibleSpace();
            Event current = Event.current;
            if ((base.hasSearchFilterFocus && (current.type == EventType.KeyDown)) && ((current.keyCode == KeyCode.DownArrow) || (current.keyCode == KeyCode.UpArrow)))
            {
                GUIUtility.keyboardControl = this.m_TreeViewKeyboardControlID;
                if (this.treeView.IsLastClickedPartOfVisibleRows())
                {
                    this.treeView.Frame(this.treeView.state.lastClickedID, true, false);
                    this.m_DidSelectSearchResult = !string.IsNullOrEmpty(base.m_SearchFilter);
                }
                else
                {
                    this.treeView.OffsetSelection(1);
                }
                current.Use();
            }
            base.SearchFieldGUI();
            GUILayout.Space(6f);
            if (this.hasSortMethods)
            {
                if (Application.isPlaying && ((GameObjectTreeViewDataSource) this.treeView.data).isFetchAIssue)
                {
                    GUILayout.Toggle(false, s_Styles.fetchWarning, s_Styles.MiniButton, new GUILayoutOption[0]);
                }
                this.SortMethodsDropDown();
            }
            GUILayout.EndHorizontal();
        }

        private void DoTreeView(float searchPathHeight)
        {
            Rect treeViewRect = this.treeViewRect;
            treeViewRect.height -= searchPathHeight;
            this.treeView.OnGUI(treeViewRect, this.m_TreeViewKeyboardControlID);
        }

        private void DuplicateGO()
        {
            Unsupported.DuplicateGameObjectsUsingPasteboard();
        }

        private void ExecuteCommands()
        {
            Event current = Event.current;
            if ((current.type == EventType.ExecuteCommand) || (current.type == EventType.ValidateCommand))
            {
                bool flag = current.type == EventType.ExecuteCommand;
                if ((current.commandName == "Delete") || (current.commandName == "SoftDelete"))
                {
                    if (flag)
                    {
                        this.DeleteGO();
                    }
                    current.Use();
                    GUIUtility.ExitGUI();
                }
                else if (current.commandName == "Duplicate")
                {
                    if (flag)
                    {
                        this.DuplicateGO();
                    }
                    current.Use();
                    GUIUtility.ExitGUI();
                }
                else if (current.commandName == "Copy")
                {
                    if (flag)
                    {
                        this.CopyGO();
                    }
                    current.Use();
                    GUIUtility.ExitGUI();
                }
                else if (current.commandName == "Paste")
                {
                    if (flag)
                    {
                        this.PasteGO();
                    }
                    current.Use();
                    GUIUtility.ExitGUI();
                }
                else if (current.commandName == "SelectAll")
                {
                    if (flag)
                    {
                        this.SelectAll();
                    }
                    current.Use();
                    GUIUtility.ExitGUI();
                }
                else if (current.commandName == "FrameSelected")
                {
                    if (current.type == EventType.ExecuteCommand)
                    {
                        this.FrameObjectPrivate(Selection.activeInstanceID, true, true);
                    }
                    current.Use();
                    GUIUtility.ExitGUI();
                }
                else if (current.commandName == "Find")
                {
                    if (current.type == EventType.ExecuteCommand)
                    {
                        base.FocusSearchField();
                    }
                    current.Use();
                }
            }
        }

        public void FrameObject(int instanceID, bool ping)
        {
            this.FrameObjectPrivate(instanceID, true, ping);
        }

        private void FrameObjectPrivate(int instanceID, bool frame, bool ping)
        {
            if (instanceID != 0)
            {
                if (this.m_LastFramedID != instanceID)
                {
                    this.treeView.EndPing();
                }
                this.SetSearchFilter(string.Empty, SearchableEditorWindow.SearchMode.All, true);
                this.m_LastFramedID = instanceID;
                this.treeView.Frame(instanceID, frame, ping);
                this.FrameObjectPrivate(InternalEditorUtility.GetGameObjectInstanceIDFromComponent(instanceID), frame, ping);
            }
        }

        public static List<SceneHierarchyWindow> GetAllSceneHierarchyWindows()
        {
            return s_SceneHierarchyWindow;
        }

        public UnityEngine.Object[] GetCurrentVisibleObjects()
        {
            List<TreeViewItem> visibleRows = this.m_TreeView.data.GetVisibleRows();
            UnityEngine.Object[] objArray = new UnityEngine.Object[visibleRows.Count];
            for (int i = 0; i < visibleRows.Count; i++)
            {
                objArray[i] = ((GameObjectTreeViewItem) visibleRows[i]).objectPPTR;
            }
            return objArray;
        }

        private string GetNameForType(System.Type type)
        {
            return type.Name;
        }

        private void HandleContextClick()
        {
            Event current = Event.current;
            if (current.type == EventType.ContextClick)
            {
                current.Use();
                GenericMenu menu = new GenericMenu();
                menu.AddItem(EditorGUIUtility.TextContent("Copy"), false, new GenericMenu.MenuFunction(this.CopyGO));
                menu.AddItem(EditorGUIUtility.TextContent("Paste"), false, new GenericMenu.MenuFunction(this.PasteGO));
                menu.AddSeparator(string.Empty);
                if (!base.hasSearchFilter && (this.m_TreeViewState.selectedIDs.Count == 1))
                {
                    menu.AddItem(EditorGUIUtility.TextContent("Rename"), false, new GenericMenu.MenuFunction(this.RenameGO));
                }
                else
                {
                    menu.AddDisabledItem(EditorGUIUtility.TextContent("Rename"));
                }
                menu.AddItem(EditorGUIUtility.TextContent("Duplicate"), false, new GenericMenu.MenuFunction(this.DuplicateGO));
                menu.AddItem(EditorGUIUtility.TextContent("Delete"), false, new GenericMenu.MenuFunction(this.DeleteGO));
                menu.AddSeparator(string.Empty);
                bool flag = false;
                if (this.m_TreeViewState.selectedIDs.Count == 1)
                {
                    GameObjectTreeViewItem item = this.treeView.FindNode(this.m_TreeViewState.selectedIDs[0]) as GameObjectTreeViewItem;
                    if (item != null)
                    {
                        <HandleContextClick>c__AnonStorey38 storey = new <HandleContextClick>c__AnonStorey38 {
                            prefab = PrefabUtility.GetPrefabParent(item.objectPPTR)
                        };
                        if (storey.prefab != null)
                        {
                            menu.AddItem(EditorGUIUtility.TextContent("Select Prefab"), false, new GenericMenu.MenuFunction(storey.<>m__53));
                            flag = true;
                        }
                    }
                }
                if (!flag)
                {
                    menu.AddDisabledItem(EditorGUIUtility.TextContent("Select Prefab"));
                }
                menu.AddSeparator(string.Empty);
                this.AddCreateGameObjectItemsToMenu(menu, Selection.objects, false);
                menu.ShowAsContext();
            }
        }

        private void Init()
        {
            if (this.m_TreeView == null)
            {
                if (this.m_TreeViewState == null)
                {
                    this.m_TreeViewState = new TreeViewState();
                }
                this.m_TreeView = new TreeView(this, this.m_TreeViewState);
                this.m_TreeView.itemDoubleClickedCallback = (Action<int>) Delegate.Combine(this.m_TreeView.itemDoubleClickedCallback, new Action<int>(this.TreeViewItemDoubleClicked));
                this.m_TreeView.selectionChangedCallback = (Action<int[]>) Delegate.Combine(this.m_TreeView.selectionChangedCallback, new Action<int[]>(this.TreeViewSelectionChanged));
                this.m_TreeView.onGUIRowCallback = (Action<int, Rect>) Delegate.Combine(this.m_TreeView.onGUIRowCallback, new Action<int, Rect>(this.OnGUIAssetCallback));
                this.m_TreeView.dragEndedCallback = (Action<int[], bool>) Delegate.Combine(this.m_TreeView.dragEndedCallback, new Action<int[], bool>(this.OnDragEndedCallback));
                this.m_TreeView.deselectOnUnhandledMouseDown = true;
                GameObjectTreeViewDataSource data = new GameObjectTreeViewDataSource(this.m_TreeView, 0, false, false);
                GameObjectsTreeViewDragging dragging = new GameObjectsTreeViewDragging(this.m_TreeView);
                GameObjectTreeViewGUI gui = new GameObjectTreeViewGUI(this.m_TreeView, false);
                this.m_TreeView.Init(this.treeViewRect, data, gui, dragging);
                data.searchMode = (int) base.m_SearchMode;
                data.searchString = base.m_SearchFilter;
                this.m_AllowAlphaNumericalSort = EditorPrefs.GetBool("AllowAlphaNumericHierarchy", false) || InternalEditorUtility.inBatchMode;
                this.SetUpSortMethodLists();
                this.m_TreeView.ReloadData();
            }
        }

        private void OnBecameVisible()
        {
            this.ReloadData();
        }

        public override void OnDisable()
        {
            EditorApplication.projectWindowChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.projectWindowChanged, new EditorApplication.CallbackFunction(this.ReloadData));
            EditorApplication.searchChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.searchChanged, new EditorApplication.CallbackFunction(this.SearchChanged));
            s_SceneHierarchyWindow.Remove(this);
        }

        private void OnDragEndedCallback(int[] draggedInstanceIds, bool draggedItemsFromOwnTreeView)
        {
            if ((draggedInstanceIds != null) && draggedItemsFromOwnTreeView)
            {
                this.ReloadData();
                this.treeView.SetSelection(draggedInstanceIds, true);
                this.treeView.NotifyListenersThatSelectionChanged();
                base.Repaint();
                GUIUtility.ExitGUI();
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            base.titleContent = base.GetLocalizedTitleContent();
            s_SceneHierarchyWindow.Add(this);
            EditorApplication.projectWindowChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.projectWindowChanged, new EditorApplication.CallbackFunction(this.ReloadData));
            EditorApplication.searchChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.searchChanged, new EditorApplication.CallbackFunction(this.SearchChanged));
        }

        private void OnEvent()
        {
            this.treeView.OnEvent();
        }

        private void OnGUI()
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            this.m_TreeViewKeyboardControlID = GUIUtility.GetControlID(FocusType.Keyboard);
            this.OnEvent();
            Rect rect = new Rect(0f, 0f, base.position.width, base.position.height);
            Event current = Event.current;
            if ((current.type == EventType.MouseDown) && rect.Contains(current.mousePosition))
            {
                this.treeView.EndPing();
            }
            this.DoToolbar();
            float searchPathHeight = this.DoSearchResultPathGUI();
            this.DoTreeView(searchPathHeight);
            this.ExecuteCommands();
            this.HandleContextClick();
        }

        private void OnGUIAssetCallback(int instanceID, Rect rect)
        {
            if (EditorApplication.hierarchyWindowItemOnGUI != null)
            {
                EditorApplication.hierarchyWindowItemOnGUI(instanceID, rect);
            }
        }

        private void OnHierarchyChange()
        {
            this.treeView.EndNameEditing(false);
            this.ReloadData();
        }

        private void OnLostFocus()
        {
            this.treeView.EndNameEditing(true);
            EditorGUI.EndEditingActiveTextField();
        }

        private void OnSelectionChange()
        {
            this.treeView.SetSelection(Selection.instanceIDs, true);
            base.Repaint();
        }

        private void PasteGO()
        {
            Unsupported.PasteGameObjectsFromPasteboard();
        }

        public void ReloadData()
        {
            this.treeView.ReloadData();
        }

        private void RenameGO()
        {
            this.treeView.BeginNameEditing(0f);
        }

        public void SearchChanged()
        {
            GameObjectTreeViewDataSource data = (GameObjectTreeViewDataSource) this.treeView.data;
            if ((data.searchMode != base.searchMode) || (data.searchString != base.m_SearchFilter))
            {
                data.searchMode = (int) base.searchMode;
                data.searchString = base.m_SearchFilter;
                if (base.m_SearchFilter == string.Empty)
                {
                    this.treeView.Frame(Selection.activeInstanceID, true, false);
                }
                this.ReloadData();
            }
        }

        private void SelectAll()
        {
            int[] visibleRowIDs = this.treeView.GetVisibleRowIDs();
            this.treeView.SetSelection(visibleRowIDs, false);
            this.TreeViewSelectionChanged(visibleRowIDs);
        }

        internal void SelectNext()
        {
            this.m_TreeView.OffsetSelection(1);
        }

        internal void SelectPrevious()
        {
            this.m_TreeView.OffsetSelection(-1);
        }

        public void SetExpandedRecursive(int id, bool expand)
        {
            TreeViewItem item = this.treeView.data.FindItem(id);
            if (item == null)
            {
                this.ReloadData();
                item = this.treeView.data.FindItem(id);
            }
            if (item != null)
            {
                this.treeView.data.SetExpandedWithChildren(item, expand);
            }
        }

        internal override void SetSearchFilter(string searchFilter, SearchableEditorWindow.SearchMode searchMode, bool setAll)
        {
            base.SetSearchFilter(searchFilter, searchMode, setAll);
            if (this.m_DidSelectSearchResult && string.IsNullOrEmpty(searchFilter))
            {
                this.m_DidSelectSearchResult = false;
                this.FrameObjectPrivate(Selection.activeInstanceID, true, false);
                if (GUIUtility.keyboardControl == 0)
                {
                    GUIUtility.keyboardControl = this.m_TreeViewKeyboardControlID;
                }
            }
        }

        private void SetSortFunction(string sortTypeName)
        {
            if (!this.m_SortingObjects.ContainsKey(sortTypeName))
            {
                Debug.LogError("Invalid search type name: " + sortTypeName);
            }
            else
            {
                this.currentSortMethod = sortTypeName;
                if (this.treeView.GetSelection().Any<int>())
                {
                    this.treeView.Frame(this.treeView.GetSelection().First<int>(), true, false);
                }
                this.treeView.ReloadData();
            }
        }

        public void SetSortFunction(System.Type sortType)
        {
            this.SetSortFunction(this.GetNameForType(sortType));
        }

        private void SetUpSortMethodLists()
        {
            this.m_SortingObjects = new Dictionary<string, BaseHierarchySort>();
            foreach (Assembly assembly in EditorAssemblies.loadedAssemblies)
            {
                IEnumerator<BaseHierarchySort> enumerator = AssemblyHelper.FindImplementors<BaseHierarchySort>(assembly).GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        BaseHierarchySort current = enumerator.Current;
                        if ((current.GetType() != typeof(AlphabeticalSort)) || this.m_AllowAlphaNumericalSort)
                        {
                            string nameForType = this.GetNameForType(current.GetType());
                            this.m_SortingObjects.Add(nameForType, current);
                        }
                    }
                }
                finally
                {
                    if (enumerator == null)
                    {
                    }
                    enumerator.Dispose();
                }
            }
            this.currentSortMethod = this.m_CurrentSortMethod;
        }

        private void SortFunctionCallback(SceneHierarchySortingWindow.InputData data)
        {
            this.SetSortFunction(data.m_TypeName);
        }

        private void SortMethodsDropDown()
        {
            if (this.hasSortMethods)
            {
                GUIContent defaultSortingContent = this.m_SortingObjects[this.currentSortMethod].content;
                if (defaultSortingContent == null)
                {
                    defaultSortingContent = s_Styles.defaultSortingContent;
                    defaultSortingContent.tooltip = this.currentSortMethod;
                }
                Rect position = GUILayoutUtility.GetRect(defaultSortingContent, EditorStyles.toolbarButton);
                if (EditorGUI.ButtonMouseDown(position, defaultSortingContent, FocusType.Passive, EditorStyles.toolbarButton))
                {
                    List<SceneHierarchySortingWindow.InputData> list = new List<SceneHierarchySortingWindow.InputData>();
                    foreach (KeyValuePair<string, BaseHierarchySort> pair in this.m_SortingObjects)
                    {
                        SceneHierarchySortingWindow.InputData item = new SceneHierarchySortingWindow.InputData {
                            m_TypeName = pair.Key,
                            m_Name = ObjectNames.NicifyVariableName(pair.Key),
                            m_Selected = pair.Key == this.m_CurrentSortMethod
                        };
                        list.Add(item);
                    }
                    if (SceneHierarchySortingWindow.ShowAtPosition(new Vector2(position.x, position.y + position.height), list, new SceneHierarchySortingWindow.OnSelectCallback(this.SortFunctionCallback)))
                    {
                        GUIUtility.ExitGUI();
                    }
                }
            }
        }

        private void TreeViewItemDoubleClicked(int instanceID)
        {
            SceneView.FrameLastActiveSceneView();
        }

        private void TreeViewSelectionChanged(int[] ids)
        {
            Selection.instanceIDs = ids;
            this.m_DidSelectSearchResult = !string.IsNullOrEmpty(base.m_SearchFilter);
        }

        private string currentSortMethod
        {
            get
            {
                return this.m_CurrentSortMethod;
            }
            set
            {
                this.m_CurrentSortMethod = value;
                if (!this.m_SortingObjects.ContainsKey(this.m_CurrentSortMethod))
                {
                    this.m_CurrentSortMethod = this.GetNameForType(typeof(TransformSort));
                }
                GameObjectTreeViewDataSource data = (GameObjectTreeViewDataSource) this.treeView.data;
                data.sortingState.sortingObject = this.m_SortingObjects[this.m_CurrentSortMethod];
                GameObjectsTreeViewDragging dragging = (GameObjectsTreeViewDragging) this.treeView.dragging;
                dragging.allowDragBetween = !data.sortingState.implementsCompare;
            }
        }

        private bool hasSortMethods
        {
            get
            {
                return (this.m_SortingObjects.Count > 1);
            }
        }

        private TreeView treeView
        {
            get
            {
                if (this.m_TreeView == null)
                {
                    this.Init();
                }
                return this.m_TreeView;
            }
        }

        private Rect treeViewRect
        {
            get
            {
                return new Rect(0f, 17f, base.position.width, base.position.height - 17f);
            }
        }

        [CompilerGenerated]
        private sealed class <HandleContextClick>c__AnonStorey38
        {
            internal UnityEngine.Object prefab;

            internal void <>m__53()
            {
                Selection.activeObject = this.prefab;
                EditorGUIUtility.PingObject(this.prefab.GetInstanceID());
            }
        }

        private class Styles
        {
            public GUIContent createContent = new GUIContent("Create");
            public GUIContent defaultSortingContent = new GUIContent(EditorGUIUtility.FindTexture("CustomSorting"));
            public GUIContent fetchWarning = new GUIContent(string.Empty, EditorGUIUtility.FindTexture("console.warnicon.sml"), "The current sorting method is taking a lot of time. Consider using 'Transform Sort' in playmode for better performance.");
            private const string kCustomSorting = "CustomSorting";
            private const string kWarningMessage = "The current sorting method is taking a lot of time. Consider using 'Transform Sort' in playmode for better performance.";
            private const string kWarningSymbol = "console.warnicon.sml";
            public GUIStyle MiniButton = "ToolbarButton";
        }
    }
}

