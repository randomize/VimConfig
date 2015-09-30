namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal class ProjectBrowserColumnOneTreeViewGUI : AssetsTreeViewGUI
    {
        private const float k_DistBetweenRootTypes = 15f;
        private Texture2D k_FavoriteFilterIcon;
        private Texture2D k_FavoriteFolderIcon;
        private Texture2D k_FavoritesIcon;
        private bool m_IsCreatingSavedFilter;

        public ProjectBrowserColumnOneTreeViewGUI(TreeView treeView) : base(treeView)
        {
            this.k_FavoritesIcon = EditorGUIUtility.FindTexture("Favorite Icon");
            this.k_FavoriteFolderIcon = EditorGUIUtility.FindTexture("FolderFavorite Icon");
            this.k_FavoriteFilterIcon = EditorGUIUtility.FindTexture("Search Icon");
        }

        internal virtual void BeginCreateSavedFilter(SearchFilter filter)
        {
            string displayName = "New Saved Search";
            this.m_IsCreatingSavedFilter = true;
            int id = SavedSearchFilters.AddSavedFilter(displayName, filter, GetListAreaGridSize());
            base.m_TreeView.Frame(id, true, false);
            base.m_TreeView.state.renameOverlay.BeginRename(displayName, id, 0f);
        }

        public override void GetFirstAndLastRowVisible(List<TreeViewItem> rows, float topPixel, float heightInPixels, out int firstRowVisible, out int lastRowVisible)
        {
            firstRowVisible = (int) Mathf.Floor(topPixel / base.k_LineHeight);
            lastRowVisible = firstRowVisible + ((int) Mathf.Ceil(heightInPixels / base.k_LineHeight));
            float num = 15f / base.k_LineHeight;
            firstRowVisible -= (int) Mathf.Ceil(2f * num);
            lastRowVisible += (int) Mathf.Ceil(2f * num);
            firstRowVisible = Mathf.Max(firstRowVisible, 0);
            lastRowVisible = Mathf.Min(lastRowVisible, rows.Count - 1);
        }

        public override float GetHeightOfLastRow()
        {
            return base.k_LineHeight;
        }

        protected override Texture GetIconForNode(TreeViewItem item)
        {
            if ((item != null) && (item.icon != null))
            {
                return item.icon;
            }
            SearchFilterTreeItem item2 = item as SearchFilterTreeItem;
            if (item2 == null)
            {
                return base.GetIconForNode(item);
            }
            if (this.IsVisibleRootNode(item))
            {
                return this.k_FavoritesIcon;
            }
            if (item2.isFolder)
            {
                return this.k_FavoriteFolderIcon;
            }
            return this.k_FavoriteFilterIcon;
        }

        public static float GetListAreaGridSize()
        {
            float listAreaGridSize = -1f;
            if (ProjectBrowser.s_LastInteractedProjectBrowser != null)
            {
                listAreaGridSize = ProjectBrowser.s_LastInteractedProjectBrowser.listAreaGridSize;
            }
            return listAreaGridSize;
        }

        public override int GetNumRowsOnPageUpDown(TreeViewItem fromItem, bool pageUp, float heightOfTreeView)
        {
            return (((int) Mathf.Floor(heightOfTreeView / base.k_LineHeight)) - 1);
        }

        public override float GetTopPixelOfRow(int row, List<TreeViewItem> rows)
        {
            float num = row * base.k_LineHeight;
            TreeViewItem item = rows[row];
            if (ProjectBrowser.GetItemType(item.id) == ProjectBrowser.ItemType.Asset)
            {
                num += 15f;
            }
            return num;
        }

        public override Vector2 GetTotalSize(List<TreeViewItem> rows)
        {
            Vector2 totalSize = base.GetTotalSize(rows);
            totalSize.y += 15f;
            return totalSize;
        }

        private bool IsVisibleRootNode(TreeViewItem item)
        {
            return (base.m_TreeView.data as ProjectBrowserColumnOneTreeViewDataSource).IsVisibleRootNode(item);
        }

        public override Rect OnRowGUI(TreeViewItem item, int row, float rowWidth, bool selected, bool focused)
        {
            float y = row * base.k_LineHeight;
            if (ProjectBrowser.GetItemType(item.id) == ProjectBrowser.ItemType.Asset)
            {
                y += 15f;
            }
            Rect rect = new Rect(0f, y, rowWidth, base.k_LineHeight);
            bool useBoldFont = this.IsVisibleRootNode(item);
            this.DoNodeGUI(rect, item, selected, focused, useBoldFont);
            return rect;
        }

        protected override void RenameEnded()
        {
            int userData = base.GetRenameOverlay().userData;
            ProjectBrowser.ItemType itemType = ProjectBrowser.GetItemType(userData);
            if (this.m_IsCreatingSavedFilter)
            {
                this.m_IsCreatingSavedFilter = false;
                if (base.GetRenameOverlay().userAcceptedRename)
                {
                    SavedSearchFilters.SetName(userData, base.GetRenameOverlay().name);
                    int[] selectedIDs = new int[] { userData };
                    base.m_TreeView.SetSelection(selectedIDs, true);
                }
                else
                {
                    SavedSearchFilters.RemoveSavedFilter(userData);
                }
            }
            else if (itemType == ProjectBrowser.ItemType.SavedFilter)
            {
                if (base.GetRenameOverlay().userAcceptedRename)
                {
                    SavedSearchFilters.SetName(userData, base.GetRenameOverlay().name);
                }
            }
            else
            {
                base.RenameEnded();
                if (base.GetRenameOverlay().userAcceptedRename)
                {
                    base.m_TreeView.NotifyListenersThatSelectionChanged();
                }
            }
        }
    }
}

