namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal abstract class TreeViewGUI : ITreeViewGUI
    {
        protected float k_BaseIndent;
        protected float k_BottomRowMargin;
        protected float k_FoldoutWidth;
        protected float k_HalfDropBetweenHeight;
        protected float k_IconWidth;
        protected float k_IndentWidth;
        protected float k_LineHeight;
        protected float k_SpaceBetweenIconAndText;
        protected float k_TopRowMargin;
        protected Rect m_DraggingInsertionMarkerRect;
        protected PingData m_Ping;
        protected TreeView m_TreeView;
        protected bool m_UseHorizontalScroll;
        protected static Styles s_Styles;

        public TreeViewGUI(TreeView treeView)
        {
            this.m_Ping = new PingData();
            this.k_LineHeight = 16f;
            this.k_BaseIndent = 2f;
            this.k_IndentWidth = 14f;
            this.k_FoldoutWidth = 12f;
            this.k_IconWidth = 16f;
            this.k_SpaceBetweenIconAndText = 2f;
            this.k_HalfDropBetweenHeight = 4f;
            this.m_TreeView = treeView;
        }

        public TreeViewGUI(TreeView treeView, bool useHorizontalScroll)
        {
            this.m_Ping = new PingData();
            this.k_LineHeight = 16f;
            this.k_BaseIndent = 2f;
            this.k_IndentWidth = 14f;
            this.k_FoldoutWidth = 12f;
            this.k_IconWidth = 16f;
            this.k_SpaceBetweenIconAndText = 2f;
            this.k_HalfDropBetweenHeight = 4f;
            this.m_TreeView = treeView;
            this.m_UseHorizontalScroll = useHorizontalScroll;
        }

        public virtual void BeginPingNode(TreeViewItem item, float topPixelOfRow, float availableWidth)
        {
            <BeginPingNode>c__AnonStorey24 storey = new <BeginPingNode>c__AnonStorey24 {
                item = item,
                <>f__this = this
            };
            if ((storey.item != null) && (topPixelOfRow >= 0f))
            {
                <BeginPingNode>c__AnonStorey25 storey2 = new <BeginPingNode>c__AnonStorey25 {
                    <>f__ref$36 = storey,
                    <>f__this = this
                };
                this.m_Ping.m_TimeStart = Time.realtimeSinceStartup;
                this.m_Ping.m_PingStyle = s_Styles.ping;
                GUIContent content = GUIContent.Temp(storey.item.displayName);
                Vector2 vector = this.m_Ping.m_PingStyle.CalcSize(content);
                this.m_Ping.m_ContentRect = new Rect(this.GetContentIndent(storey.item), topPixelOfRow, ((this.k_IconWidth + this.k_SpaceBetweenIconAndText) + vector.x) + this.iconTotalPadding, vector.y);
                this.m_Ping.m_AvailableWidth = availableWidth;
                storey2.useBoldFont = storey.item.displayName.Equals("Assets");
                this.m_Ping.m_ContentDraw = new Action<Rect>(storey2.<>m__30);
                this.m_TreeView.Repaint();
            }
        }

        public virtual bool BeginRename(TreeViewItem item, float delay)
        {
            return this.GetRenameOverlay().BeginRename(item.displayName, item.id, delay);
        }

        public virtual void BeginRowGUI()
        {
            this.InitStyles();
            this.m_DraggingInsertionMarkerRect.x = -1f;
            this.SyncFakeItem();
            if (Event.current.type != EventType.Repaint)
            {
                this.DoRenameOverlay();
            }
        }

        protected virtual void ClearRenameAndNewNodeState()
        {
            this.m_TreeView.data.RemoveFakeItem();
            this.GetRenameOverlay().Clear();
        }

        protected virtual Rect DoFoldout(TreeViewItem item, Rect rowRect)
        {
            float foldoutIndent = this.GetFoldoutIndent(item);
            Rect position = new Rect(foldoutIndent, rowRect.y, this.k_FoldoutWidth, this.k_FoldoutWidth);
            EditorGUI.BeginChangeCheck();
            bool expand = GUI.Toggle(position, this.m_TreeView.data.IsExpanded(item), GUIContent.none, s_Styles.foldout);
            if (EditorGUI.EndChangeCheck())
            {
                if (Event.current.alt)
                {
                    this.m_TreeView.data.SetExpandedWithChildren(item, expand);
                }
                else
                {
                    this.m_TreeView.data.SetExpanded(item, expand);
                }
                if (expand)
                {
                    this.m_TreeView.UserExpandedNode(item);
                }
            }
            return position;
        }

        protected virtual void DoNodeGUI(Rect rect, TreeViewItem item, bool selected, bool focused, bool useBoldFont)
        {
            EditorGUIUtility.SetIconSize(new Vector2(this.k_IconWidth, this.k_IconWidth));
            float foldoutIndent = this.GetFoldoutIndent(item);
            int itemControlID = TreeView.GetItemControlID(item);
            bool flag = false;
            if (this.m_TreeView.dragging != null)
            {
                flag = (this.m_TreeView.dragging.GetDropTargetControlID() == itemControlID) && this.m_TreeView.data.CanBeParent(item);
            }
            bool flag2 = this.IsRenaming(item.id);
            bool flag3 = this.m_TreeView.data.IsExpandable(item);
            if (flag2 && (Event.current.type == EventType.Repaint))
            {
                float num3 = (((foldoutIndent + this.k_FoldoutWidth) + this.k_IconWidth) + this.iconTotalPadding) - 1f;
                this.GetRenameOverlay().editFieldRect = new Rect(rect.x + num3, rect.y, rect.width - num3, rect.height);
            }
            if (Event.current.type == EventType.Repaint)
            {
                string displayName = item.displayName;
                if (flag2)
                {
                    selected = false;
                    displayName = string.Empty;
                }
                if (selected)
                {
                    s_Styles.lineStyle.Draw(rect, false, false, true, focused);
                }
                if (flag)
                {
                    s_Styles.lineStyle.Draw(rect, GUIContent.none, true, true, false, false);
                }
                this.DrawIconAndLabel(rect, item, displayName, selected, focused, useBoldFont, false);
                if ((this.m_TreeView.dragging != null) && (this.m_TreeView.dragging.GetRowMarkerControlID() == itemControlID))
                {
                    this.m_DraggingInsertionMarkerRect = new Rect((rect.x + foldoutIndent) + this.k_FoldoutWidth, rect.y, rect.width - foldoutIndent, rect.height);
                }
            }
            if (flag3)
            {
                this.DoFoldout(item, rect);
            }
            EditorGUIUtility.SetIconSize(Vector2.zero);
        }

        public virtual void DoRenameOverlay()
        {
            if (this.GetRenameOverlay().IsRenaming() && !this.GetRenameOverlay().OnGUI())
            {
                this.EndRename();
            }
        }

        protected virtual void DrawIconAndLabel(Rect rect, TreeViewItem item, string label, bool selected, bool focused, bool useBoldFont, bool isPinging)
        {
            if (!isPinging)
            {
                float contentIndent = this.GetContentIndent(item);
                rect.x += contentIndent;
                rect.width -= contentIndent;
            }
            GUIStyle style = !useBoldFont ? s_Styles.lineStyle : s_Styles.lineBoldStyle;
            style.padding.left = (int) ((this.k_IconWidth + this.iconTotalPadding) + this.k_SpaceBetweenIconAndText);
            style.Draw(rect, label, false, false, selected, focused);
            Rect position = rect;
            position.width = this.k_IconWidth;
            position.height = this.k_IconWidth;
            position.x += this.iconLeftPadding;
            Texture iconForNode = this.GetIconForNode(item);
            if (iconForNode != null)
            {
                GUI.DrawTexture(position, iconForNode);
            }
            if (this.iconOverlayGUI != null)
            {
                Rect rect3 = rect;
                rect3.width = this.k_IconWidth + this.iconTotalPadding;
                this.iconOverlayGUI(item, rect3);
            }
        }

        public virtual void EndPingNode()
        {
            this.m_Ping.m_TimeStart = -1f;
        }

        public virtual void EndRename()
        {
            if (this.GetRenameOverlay().HasKeyboardFocus())
            {
                this.m_TreeView.GrabKeyboardFocus();
            }
            this.RenameEnded();
            this.ClearRenameAndNewNodeState();
        }

        public virtual void EndRowGUI()
        {
            if ((this.m_DraggingInsertionMarkerRect.x >= 0f) && (Event.current.type == EventType.Repaint))
            {
                if (this.m_TreeView.dragging.drawRowMarkerAbove)
                {
                    s_Styles.insertionAbove.Draw(this.m_DraggingInsertionMarkerRect, false, false, false, false);
                }
                else
                {
                    s_Styles.insertion.Draw(this.m_DraggingInsertionMarkerRect, false, false, false, false);
                }
            }
            if (Event.current.type == EventType.Repaint)
            {
                this.DoRenameOverlay();
            }
            this.HandlePing();
        }

        public virtual float GetContentIndent(TreeViewItem item)
        {
            return (this.GetFoldoutIndent(item) + this.k_FoldoutWidth);
        }

        public virtual void GetFirstAndLastRowVisible(List<TreeViewItem> rows, float topPixel, float heightInPixels, out int firstRowVisible, out int lastRowVisible)
        {
            firstRowVisible = (int) Mathf.Floor(topPixel / this.k_LineHeight);
            lastRowVisible = firstRowVisible + ((int) Mathf.Ceil(heightInPixels / this.k_LineHeight));
            firstRowVisible = Mathf.Max(firstRowVisible, 0);
            lastRowVisible = Mathf.Min(lastRowVisible, rows.Count - 1);
        }

        public virtual float GetFoldoutIndent(TreeViewItem item)
        {
            if (this.m_TreeView.isSearching)
            {
                return this.k_BaseIndent;
            }
            return (this.k_BaseIndent + (item.depth * this.indentWidth));
        }

        public virtual float GetHeightOfLastRow()
        {
            return this.k_LineHeight;
        }

        protected abstract Texture GetIconForNode(TreeViewItem item);
        protected float GetMaxWidth(List<TreeViewItem> rows)
        {
            this.InitStyles();
            float num = 1f;
            foreach (TreeViewItem item in rows)
            {
                float num3;
                float num4;
                float num2 = 0f;
                num2 += this.GetContentIndent(item);
                if (item.icon != null)
                {
                    num2 += this.k_IconWidth;
                }
                s_Styles.lineStyle.CalcMinMaxWidth(GUIContent.Temp(item.displayName), out num3, out num4);
                num2 += num4;
                num2 += this.k_BaseIndent;
                if (num2 > num)
                {
                    num = num2;
                }
            }
            return num;
        }

        public virtual int GetNumRowsOnPageUpDown(TreeViewItem fromItem, bool pageUp, float heightOfTreeView)
        {
            return (int) Mathf.Floor(heightOfTreeView / this.k_LineHeight);
        }

        protected RenameOverlay GetRenameOverlay()
        {
            return this.m_TreeView.state.renameOverlay;
        }

        public virtual float GetTopPixelOfRow(int row, List<TreeViewItem> rows)
        {
            return ((row * this.k_LineHeight) + this.topRowMargin);
        }

        public virtual Vector2 GetTotalSize(List<TreeViewItem> rows)
        {
            this.InitStyles();
            float x = 1f;
            if (this.m_UseHorizontalScroll)
            {
                x = this.GetMaxWidth(rows);
            }
            return new Vector2(x, ((rows.Count * this.k_LineHeight) + this.topRowMargin) + this.bottomRowMargin);
        }

        private void HandlePing()
        {
            this.m_Ping.HandlePing();
            if (this.m_Ping.isPinging)
            {
                this.m_TreeView.Repaint();
            }
        }

        protected virtual void InitStyles()
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
        }

        protected virtual bool IsRenaming(int id)
        {
            return ((this.GetRenameOverlay().IsRenaming() && (this.GetRenameOverlay().userData == id)) && !this.GetRenameOverlay().isWaitingForDelay);
        }

        public virtual Rect OnRowGUI(TreeViewItem item, int row, float rowWidth, bool selected, bool focused)
        {
            Rect rect = new Rect(0f, this.GetTopPixelOfRow(row, this.m_TreeView.data.GetVisibleRows()), rowWidth, this.k_LineHeight);
            this.DoNodeGUI(rect, item, selected, focused, false);
            return rect;
        }

        protected abstract void RenameEnded();
        protected virtual void SyncFakeItem()
        {
        }

        public virtual float bottomRowMargin
        {
            get
            {
                return this.k_BottomRowMargin;
            }
        }

        public float halfDropBetweenHeight
        {
            get
            {
                return this.k_HalfDropBetweenHeight;
            }
        }

        public float iconLeftPadding { get; set; }

        public Action<TreeViewItem, Rect> iconOverlayGUI { get; set; }

        public float iconRightPadding { get; set; }

        public float iconTotalPadding
        {
            get
            {
                return (this.iconLeftPadding + this.iconRightPadding);
            }
        }

        protected float indentWidth
        {
            get
            {
                return (this.k_IndentWidth + this.iconTotalPadding);
            }
        }

        public virtual float topRowMargin
        {
            get
            {
                return this.k_TopRowMargin;
            }
        }

        [CompilerGenerated]
        private sealed class <BeginPingNode>c__AnonStorey24
        {
            internal TreeViewGUI <>f__this;
            internal TreeViewItem item;
        }

        [CompilerGenerated]
        private sealed class <BeginPingNode>c__AnonStorey25
        {
            internal TreeViewGUI.<BeginPingNode>c__AnonStorey24 <>f__ref$36;
            internal TreeViewGUI <>f__this;
            internal bool useBoldFont;

            internal void <>m__30(Rect r)
            {
                this.<>f__this.DrawIconAndLabel(r, this.<>f__ref$36.item, this.<>f__ref$36.item.displayName, false, false, this.useBoldFont, true);
            }
        }

        internal class Styles
        {
            public GUIContent content = new GUIContent(EditorGUIUtility.FindTexture(EditorResourcesUtility.folderIconName));
            public GUIStyle foldout = "IN Foldout";
            public GUIStyle insertion = "PR Insertion";
            public GUIStyle insertionAbove = "PR Insertion Above";
            public GUIStyle lineBoldStyle = new GUIStyle("PR Label");
            public GUIStyle lineStyle = new GUIStyle("PR Label");
            public GUIStyle ping = new GUIStyle("PR Ping");
            public GUIStyle toolbarButton = "ToolbarButton";

            public Styles()
            {
                this.lineStyle.alignment = TextAnchor.MiddleLeft;
                this.lineBoldStyle.alignment = TextAnchor.MiddleLeft;
                this.lineBoldStyle.font = EditorStyles.boldLabel.font;
                this.lineBoldStyle.fontStyle = EditorStyles.boldLabel.fontStyle;
                this.ping.padding.left = 0x10;
                this.ping.padding.right = 0x10;
                this.ping.fixedHeight = 16f;
            }
        }
    }
}

