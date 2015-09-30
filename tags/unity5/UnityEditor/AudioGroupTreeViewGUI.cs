namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor.Audio;
    using UnityEngine;

    internal class AudioGroupTreeViewGUI : TreeViewGUI
    {
        private readonly float column1Width;
        private readonly Texture2D k_VisibleON;
        public AudioMixerController m_Controller;
        public Action<AudioMixerTreeViewNode, bool> NodeWasToggled;

        public AudioGroupTreeViewGUI(TreeView treeView) : base(treeView)
        {
            this.column1Width = 20f;
            this.k_VisibleON = EditorGUIUtility.FindTexture("VisibilityOn");
            base.k_BaseIndent = this.column1Width;
            base.k_IconWidth = 0f;
            base.k_TopRowMargin = base.k_BottomRowMargin = 2f;
        }

        protected override Texture GetIconForNode(TreeViewItem node)
        {
            if ((node != null) && (node.icon != null))
            {
                return node.icon;
            }
            return null;
        }

        public override Rect OnRowGUI(TreeViewItem node, int row, float rowWidth, bool selected, bool focused)
        {
            Event current = Event.current;
            Rect rect = new Rect(0f, (row * base.k_LineHeight) + base.k_TopRowMargin, rowWidth, base.k_LineHeight);
            this.DoNodeGUI(rect, node, selected, focused, false);
            if (this.m_Controller != null)
            {
                AudioMixerTreeViewNode node2 = node as AudioMixerTreeViewNode;
                if (node2 == null)
                {
                    return rect;
                }
                bool visible = this.m_Controller.CurrentViewContainsGroup(node2.group.groupID);
                float num2 = 3f;
                Rect position = new Rect(rect.x + num2, rect.y, 16f, 16f);
                Rect rect3 = new Rect(position.x + 1f, position.y + 1f, position.width - 2f, position.height - 2f);
                int userColorIndex = node2.group.userColorIndex;
                if (userColorIndex > 0)
                {
                    EditorGUI.DrawRect(new Rect(rect.x, rect3.y, 2f, rect3.height), AudioMixerColorCodes.GetColor(userColorIndex));
                }
                EditorGUI.DrawRect(rect3, new Color(0.5f, 0.5f, 0.5f, 0.2f));
                if (visible)
                {
                    GUI.DrawTexture(position, this.k_VisibleON);
                }
                Rect rect4 = new Rect(2f, rect.y, rect.height, rect.height);
                if (((current.type == EventType.MouseUp) && (current.button == 0)) && (rect4.Contains(current.mousePosition) && (this.NodeWasToggled != null)))
                {
                    this.NodeWasToggled(node2, !visible);
                }
                if ((current.type == EventType.ContextClick) && position.Contains(current.mousePosition))
                {
                    this.OpenGroupContextMenu(node2, visible);
                    current.Use();
                }
            }
            return rect;
        }

        private void OpenGroupContextMenu(AudioMixerTreeViewNode audioNode, bool visible)
        {
            AudioMixerGroupController[] controllerArray;
            <OpenGroupContextMenu>c__AnonStorey5B storeyb = new <OpenGroupContextMenu>c__AnonStorey5B {
                audioNode = audioNode,
                visible = visible,
                <>f__this = this
            };
            GenericMenu menu = new GenericMenu();
            if (this.NodeWasToggled != null)
            {
                menu.AddItem(new GUIContent(!storeyb.visible ? "Show Group" : "Hide group"), false, new GenericMenu.MenuFunction(storeyb.<>m__9E));
            }
            menu.AddSeparator(string.Empty);
            if (this.m_Controller.CachedSelection.Contains(storeyb.audioNode.group))
            {
                controllerArray = this.m_Controller.CachedSelection.ToArray();
            }
            else
            {
                controllerArray = new AudioMixerGroupController[] { storeyb.audioNode.group };
            }
            AudioMixerColorCodes.AddColorItemsToGenericMenu(menu, controllerArray);
            menu.ShowAsContext();
        }

        protected override void RenameEnded()
        {
            if (base.GetRenameOverlay().userAcceptedRename)
            {
                string name = !string.IsNullOrEmpty(base.GetRenameOverlay().name) ? base.GetRenameOverlay().name : base.GetRenameOverlay().originalName;
                int userData = base.GetRenameOverlay().userData;
                AudioMixerTreeViewNode node = base.m_TreeView.FindNode(userData) as AudioMixerTreeViewNode;
                if (node != null)
                {
                    ObjectNames.SetNameSmartWithInstanceID(userData, name);
                    foreach (AudioMixerEffectController controller in node.group.effects)
                    {
                        controller.ClearCachedDisplayName();
                    }
                    base.m_TreeView.ReloadData();
                    if (this.m_Controller != null)
                    {
                        this.m_Controller.OnSubAssetChanged();
                    }
                }
            }
        }

        protected override void SyncFakeItem()
        {
        }

        [CompilerGenerated]
        private sealed class <OpenGroupContextMenu>c__AnonStorey5B
        {
            internal AudioGroupTreeViewGUI <>f__this;
            internal AudioMixerTreeViewNode audioNode;
            internal bool visible;

            internal void <>m__9E()
            {
                this.<>f__this.NodeWasToggled(this.audioNode, !this.visible);
            }
        }
    }
}

