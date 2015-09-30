namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class GameObjectTreeViewGUI : TreeViewGUI
    {
        protected static GameObjectStyles s_GOStyles;

        public GameObjectTreeViewGUI(TreeView treeView, bool useHorizontalScroll) : base(treeView, useHorizontalScroll)
        {
            base.k_IconWidth = 0f;
            base.k_TopRowMargin = 4f;
        }

        public override bool BeginRename(TreeViewItem item, float delay)
        {
            GameObjectTreeViewItem item2 = item as GameObjectTreeViewItem;
            if ((item2.objectPPTR.hideFlags & HideFlags.NotEditable) != HideFlags.None)
            {
                Debug.LogWarning("Unable to rename a GameObject with HideFlags.NotEditable.");
                return false;
            }
            return base.BeginRename(item, delay);
        }

        protected override void DrawIconAndLabel(Rect rect, TreeViewItem item, string label, bool selected, bool focused, bool useBoldFont, bool isPinging)
        {
            if (!isPinging)
            {
                float contentIndent = this.GetContentIndent(item);
                rect.x += contentIndent;
                rect.width -= contentIndent;
            }
            GameObjectTreeViewItem item2 = item as GameObjectTreeViewItem;
            int colorCode = item2.colorCode;
            if (string.IsNullOrEmpty(item.displayName))
            {
                if (item2.objectPPTR != null)
                {
                    item2.displayName = item2.objectPPTR.name;
                }
                else
                {
                    item2.displayName = "deleted gameobject";
                }
                label = item2.displayName;
            }
            GUIStyle lineStyle = TreeViewGUI.s_Styles.lineStyle;
            if (!item2.shouldDisplay)
            {
                lineStyle = s_GOStyles.disabledLabel;
            }
            else if ((colorCode & 3) == 0)
            {
                lineStyle = (colorCode >= 4) ? s_GOStyles.disabledLabel : TreeViewGUI.s_Styles.lineStyle;
            }
            else if ((colorCode & 3) == 1)
            {
                lineStyle = (colorCode >= 4) ? s_GOStyles.disabledPrefabLabel : s_GOStyles.prefabLabel;
            }
            else if ((colorCode & 3) == 2)
            {
                lineStyle = (colorCode >= 4) ? s_GOStyles.disabledBrokenPrefabLabel : s_GOStyles.brokenPrefabLabel;
            }
            lineStyle.padding.left = (int) base.k_SpaceBetweenIconAndText;
            lineStyle.Draw(rect, label, false, false, selected, focused);
        }

        protected override Texture GetIconForNode(TreeViewItem item)
        {
            return null;
        }

        protected override void InitStyles()
        {
            base.InitStyles();
            if (s_GOStyles == null)
            {
                s_GOStyles = new GameObjectStyles();
            }
        }

        protected override void RenameEnded()
        {
            string name = !string.IsNullOrEmpty(base.GetRenameOverlay().name) ? base.GetRenameOverlay().name : base.GetRenameOverlay().originalName;
            int userData = base.GetRenameOverlay().userData;
            if (base.GetRenameOverlay().userAcceptedRename)
            {
                ObjectNames.SetNameSmartWithInstanceID(userData, name);
                TreeViewItem item = base.m_TreeView.data.FindItem(userData);
                if (item != null)
                {
                    item.displayName = name;
                }
                EditorApplication.RepaintAnimationWindow();
            }
        }

        private enum GameObjectColorType
        {
            Normal,
            Prefab,
            BrokenPrefab,
            Count
        }

        internal class GameObjectStyles
        {
            public GUIStyle brokenPrefabLabel = new GUIStyle("PR BrokenPrefabLabel");
            public GUIStyle disabledBrokenPrefabLabel = new GUIStyle("PR DisabledBrokenPrefabLabel");
            public GUIStyle disabledLabel = new GUIStyle("PR DisabledLabel");
            public GUIStyle disabledPrefabLabel = new GUIStyle("PR DisabledPrefabLabel");
            public GUIStyle prefabLabel = new GUIStyle("PR PrefabLabel");

            public GameObjectStyles()
            {
                this.disabledLabel.alignment = TextAnchor.MiddleLeft;
                this.prefabLabel.alignment = TextAnchor.MiddleLeft;
                this.disabledPrefabLabel.alignment = TextAnchor.MiddleLeft;
                this.brokenPrefabLabel.alignment = TextAnchor.MiddleLeft;
                this.disabledBrokenPrefabLabel.alignment = TextAnchor.MiddleLeft;
            }
        }
    }
}

