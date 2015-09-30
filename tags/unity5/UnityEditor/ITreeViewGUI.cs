namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal interface ITreeViewGUI
    {
        void BeginPingNode(TreeViewItem item, float topPixelOfRow, float availableWidth);
        bool BeginRename(TreeViewItem item, float delay);
        void BeginRowGUI();
        void EndPingNode();
        void EndRename();
        void EndRowGUI();
        float GetContentIndent(TreeViewItem item);
        void GetFirstAndLastRowVisible(List<TreeViewItem> rows, float topPixel, float heightInPixels, out int firstRowVisible, out int lastRowVisible);
        float GetHeightOfLastRow();
        int GetNumRowsOnPageUpDown(TreeViewItem fromItem, bool pageUp, float heightOfTreeView);
        float GetTopPixelOfRow(int row, List<TreeViewItem> rows);
        Vector2 GetTotalSize(List<TreeViewItem> rows);
        Rect OnRowGUI(TreeViewItem item, int row, float rowWidth, bool selected, bool focused);

        float bottomRowMargin { get; }

        float halfDropBetweenHeight { get; }

        float topRowMargin { get; }
    }
}

