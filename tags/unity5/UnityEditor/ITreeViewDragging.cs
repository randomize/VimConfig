namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal interface ITreeViewDragging
    {
        void DragCleanup(bool revertExpanded);
        bool DragElement(TreeViewItem targetItem, Rect targetItemRect, bool firstItem);
        int GetDropTargetControlID();
        int GetRowMarkerControlID();
        void StartDrag(TreeViewItem draggedItem, List<int> draggedItemIDs);

        bool drawRowMarkerAbove { get; set; }
    }
}

