namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;

    internal class GameObjectsTreeViewDragging : TreeViewDragging
    {
        public GameObjectsTreeViewDragging(TreeView treeView) : base(treeView)
        {
            this.allowDragBetween = false;
        }

        public override DragAndDropVisualMode DoDrag(TreeViewItem parentItem, TreeViewItem targetItem, bool perform, TreeViewDragging.DropPosition dropPos)
        {
            HierarchyProperty property = new HierarchyProperty(HierarchyType.GameObjects);
            if ((parentItem == null) || (targetItem == null))
            {
                return InternalEditorUtility.HierarchyWindowDrag(null, perform, InternalEditorUtility.HierarchyDropMode.kHierarchyDropUpon);
            }
            if (this.allowDragBetween)
            {
                if (((dropPos == TreeViewDragging.DropPosition.Above) || (targetItem == null)) || !property.Find(targetItem.id, null))
                {
                    property = null;
                }
            }
            else if (((dropPos == TreeViewDragging.DropPosition.Above) || (parentItem == null)) || !property.Find(parentItem.id, null))
            {
                property = null;
            }
            InternalEditorUtility.HierarchyDropMode kHierarchyDragNormal = InternalEditorUtility.HierarchyDropMode.kHierarchyDragNormal;
            if (this.allowDragBetween)
            {
                kHierarchyDragNormal = (dropPos != TreeViewDragging.DropPosition.Upon) ? InternalEditorUtility.HierarchyDropMode.kHierarchyDropBetween : InternalEditorUtility.HierarchyDropMode.kHierarchyDropUpon;
            }
            if (((parentItem != null) && (parentItem == targetItem)) && (dropPos != TreeViewDragging.DropPosition.Above))
            {
                kHierarchyDragNormal |= InternalEditorUtility.HierarchyDropMode.kHierarchyDropAfterParent;
            }
            return InternalEditorUtility.HierarchyWindowDrag(property, perform, kHierarchyDragNormal);
        }

        public override void StartDrag(TreeViewItem draggedItem, List<int> draggedItemIDs)
        {
            DragAndDrop.PrepareStartDrag();
            draggedItemIDs = base.m_TreeView.SortIDsInVisiblityOrder(draggedItemIDs);
            DragAndDrop.objectReferences = ProjectWindowUtil.GetDragAndDropObjects(draggedItem.id, draggedItemIDs);
            DragAndDrop.paths = ProjectWindowUtil.GetDragAndDropPaths(draggedItem.id, draggedItemIDs);
            if (DragAndDrop.objectReferences.Length > 1)
            {
                DragAndDrop.StartDrag("<Multiple>");
            }
            else
            {
                DragAndDrop.StartDrag(ObjectNames.GetDragAndDropTitle(InternalEditorUtility.GetObjectFromInstanceID(draggedItem.id)));
            }
            if (base.m_TreeView.data is GameObjectTreeViewDataSource)
            {
                ((GameObjectTreeViewDataSource) base.m_TreeView.data).SetupChildParentReferencesIfNeeded();
            }
        }

        public bool allowDragBetween { get; set; }
    }
}

