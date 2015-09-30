namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class GameObjectTreeViewDataSource : LazyTreeViewDataSource
    {
        [CompilerGenerated]
        private static Func<TreeViewItem, GameObject> <>f__am$cache7;
        private const double k_FetchDelta = 0.1;
        private const HierarchyType k_HierarchyType = HierarchyType.GameObjects;
        private const double k_LongFetchTime = 0.05;
        private const int k_MaxDelayedFetch = 5;
        private int m_DelayedFetches;
        private double m_LastFetchTime;
        private bool m_NeedsChildParentReferenceSetup;
        private readonly int m_RootInstanceID;
        private int m_SearchMode;
        private string m_SearchString;
        public SortingState sortingState;

        public GameObjectTreeViewDataSource(TreeView treeView, int rootInstanceID, bool showRootNode, bool rootNodeIsCollapsable) : base(treeView)
        {
            this.m_SearchString = string.Empty;
            this.sortingState = new SortingState();
            this.m_RootInstanceID = rootInstanceID;
            this.showRootNode = showRootNode;
            base.rootIsCollapsable = rootNodeIsCollapsable;
        }

        private List<TreeViewItem> CalcVisibleItems(HierarchyProperty property, bool hasSearchString)
        {
            int depth = property.depth;
            int[] expanded = base.expandedIDs.ToArray();
            List<TreeViewItem> list = new List<TreeViewItem>();
            while (property.NextWithDepthCheck(expanded, depth))
            {
                int num = this.GetAdjustedItemDepth(hasSearchString, depth, property.depth);
                GameObjectTreeViewItem item = this.CreateTreeViewItem(property, hasSearchString, num, true);
                list.Add(item);
            }
            return list;
        }

        private GameObjectTreeViewItem CreateTreeViewItem(HierarchyProperty property, bool hasSearchString, int depth, bool shouldDisplay)
        {
            GameObjectTreeViewItem item = new GameObjectTreeViewItem(property.instanceID, depth, null, string.Empty) {
                colorCode = property.colorCode,
                objectPPTR = property.pptrValue,
                shouldDisplay = shouldDisplay
            };
            if (!hasSearchString && property.hasChildren)
            {
                item.children = LazyTreeViewDataSource.CreateChildListForCollapsedParent();
            }
            return item;
        }

        public override void FetchData()
        {
            Profiler.BeginSample("SceneHierarchyWindow.FetchData");
            int depth = 0;
            double timeSinceStartup = EditorApplication.timeSinceStartup;
            HierarchyProperty property = new HierarchyProperty(HierarchyType.GameObjects);
            property.Reset();
            property.alphaSorted = this.IsUsingAlphaSort();
            if (this.m_RootInstanceID != 0)
            {
                bool flag = property.Find(this.m_RootInstanceID, null);
                string displayName = !flag ? "RootOfSceneHierarchy" : property.name;
                base.m_RootItem = new GameObjectTreeViewItem(this.m_RootInstanceID, depth, null, displayName);
                if (!flag)
                {
                    Debug.LogError("Root gameobject with id " + this.m_RootInstanceID + " not found!!");
                }
            }
            else
            {
                base.m_RootItem = new GameObjectTreeViewItem(this.m_RootInstanceID, depth, null, "RootOfSceneHierarchy");
            }
            if (!base.showRootNode)
            {
                this.SetExpanded(base.m_RootItem, true);
            }
            bool hasSearchString = !string.IsNullOrEmpty(this.m_SearchString);
            if (hasSearchString)
            {
                property.SetSearchFilter(this.m_SearchString, this.m_SearchMode);
            }
            base.m_VisibleRows = this.CalcVisibleItems(property, hasSearchString);
            this.m_NeedsChildParentReferenceSetup = true;
            base.m_NeedRefreshVisibleFolders = false;
            if ((this.sortingState.sortingObject != null) && this.sortingState.implementsCompare)
            {
                this.SortVisibleRows();
            }
            double num3 = EditorApplication.timeSinceStartup;
            double num4 = num3 - timeSinceStartup;
            double num5 = num3 - this.m_LastFetchTime;
            if ((num5 > 0.1) && (num4 > 0.05))
            {
                this.m_DelayedFetches++;
            }
            else
            {
                this.m_DelayedFetches = 0;
            }
            this.m_LastFetchTime = timeSinceStartup;
            base.m_TreeView.SetSelection(Selection.instanceIDs, false);
            if (SceneHierarchyWindow.s_Debug)
            {
                Debug.Log(string.Concat(new object[] { "Fetch time: ", num4 * 1000.0, " ms, alphaSort = ", this.IsUsingAlphaSort() }));
            }
            Profiler.EndSample();
        }

        public override TreeViewItem FindItem(int id)
        {
            this.RevealItem(id);
            this.SetupChildParentReferencesIfNeeded();
            return base.FindItem(id);
        }

        private int GetAdjustedItemDepth(bool hasSearchString, int minDepth, int adjPropertyDepth)
        {
            return (!hasSearchString ? (adjPropertyDepth - minDepth) : 0);
        }

        protected override HashSet<int> GetParentsAbove(int id)
        {
            HashSet<int> set = new HashSet<int>();
            IHierarchyProperty property = new HierarchyProperty(HierarchyType.GameObjects);
            if (property.Find(id, null))
            {
                while (property.Parent())
                {
                    set.Add(property.instanceID);
                }
            }
            return set;
        }

        protected override HashSet<int> GetParentsBelow(int id)
        {
            HashSet<int> set = new HashSet<int>();
            IHierarchyProperty property = new HierarchyProperty(HierarchyType.GameObjects);
            if (property.Find(id, null))
            {
                set.Add(id);
                int depth = property.depth;
                while (property.Next(null) && (property.depth > depth))
                {
                    if (property.hasChildren)
                    {
                        set.Add(property.instanceID);
                    }
                }
            }
            return set;
        }

        private bool IsUsingAlphaSort()
        {
            return (this.sortingState.sortingObject.GetType() == typeof(AlphabeticalSort));
        }

        private void RebuildVisibilityTree(TreeViewItem item, List<TreeViewItem> visibleItems)
        {
            if ((item != null) && item.hasChildren)
            {
                for (int i = 0; i < item.children.Count; i++)
                {
                    if (item.children[i] != null)
                    {
                        visibleItems.Add(item.children[i]);
                        this.RebuildVisibilityTree(item.children[i], visibleItems);
                    }
                }
            }
        }

        internal void SetupChildParentReferencesIfNeeded()
        {
            if (this.m_NeedsChildParentReferenceSetup)
            {
                this.m_NeedsChildParentReferenceSetup = false;
                TreeViewUtility.SetChildParentReferences(this.GetVisibleRows(), base.m_RootItem);
            }
        }

        private void SortChildrenRecursively(TreeViewItem item, BaseHierarchySort comparer)
        {
            if ((item != null) && item.hasChildren)
            {
                if (<>f__am$cache7 == null)
                {
                    <>f__am$cache7 = x => (x as GameObjectTreeViewItem).objectPPTR as GameObject;
                }
                item.children = item.children.OrderBy<TreeViewItem, GameObject>(<>f__am$cache7, comparer).ToList<TreeViewItem>();
                for (int i = 0; i < item.children.Count; i++)
                {
                    this.SortChildrenRecursively(item.children[i], comparer);
                }
            }
        }

        private void SortVisibleRows()
        {
            this.SetupChildParentReferencesIfNeeded();
            this.SortChildrenRecursively(base.m_RootItem, this.sortingState.sortingObject);
            base.m_VisibleRows.Clear();
            this.RebuildVisibilityTree(base.m_RootItem, base.m_VisibleRows);
        }

        public bool isFetchAIssue
        {
            get
            {
                return (this.m_DelayedFetches >= 5);
            }
        }

        public int searchMode
        {
            get
            {
                return this.m_SearchMode;
            }
            set
            {
                this.m_SearchMode = value;
            }
        }

        public string searchString
        {
            get
            {
                return this.m_SearchString;
            }
            set
            {
                this.m_SearchString = value;
            }
        }

        public class SortingState
        {
            private BaseHierarchySort m_HierarchySort;
            private bool m_ImplementsCompare;

            public bool implementsCompare
            {
                get
                {
                    return this.m_ImplementsCompare;
                }
            }

            public BaseHierarchySort sortingObject
            {
                get
                {
                    return this.m_HierarchySort;
                }
                set
                {
                    this.m_HierarchySort = value;
                    if (this.m_HierarchySort != null)
                    {
                        this.m_ImplementsCompare = this.m_HierarchySort.GetType().GetMethod("Compare").DeclaringType != typeof(BaseHierarchySort);
                    }
                }
            }
        }
    }
}

