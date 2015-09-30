namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal class PackageImport : EditorWindow, ISerializationCallbackReceiver
    {
        [SerializeField]
        private AssetsItem[] m_Assets;
        [SerializeField]
        private List<string> m_EnabledFolders;
        [SerializeField]
        private string m_PackageIconPath;
        [SerializeField]
        private string m_PackageName;
        [NonSerialized]
        private PackageImportTreeView m_Tree;
        [SerializeField]
        private TreeViewState m_TreeViewState;
        private static Constants ms_Constants;
        private static readonly char[] s_InvalidPathChars = Path.GetInvalidPathChars();
        private static string s_LastPreviewPath;
        private static Texture2D s_PackageIcon;
        private static Texture2D s_Preview;

        public PackageImport()
        {
            base.minSize = new Vector2(350f, 350f);
        }

        private void BottomArea()
        {
            GUILayout.BeginVertical(ms_Constants.bottomBarBg, new GUILayoutOption[0]);
            GUILayout.Space(8f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(10f);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(50f) };
            if (GUILayout.Button(EditorGUIUtility.TextContent("All"), options))
            {
                this.m_Tree.SetAllEnabled(1);
            }
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(50f) };
            if (GUILayout.Button(EditorGUIUtility.TextContent("None"), optionArray2))
            {
                this.m_Tree.SetAllEnabled(0);
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(EditorGUIUtility.TextContent("Cancel"), new GUILayoutOption[0]))
            {
                base.Close();
                GUIUtility.ExitGUI();
            }
            if (GUILayout.Button(EditorGUIUtility.TextContent("Import"), new GUILayoutOption[0]))
            {
                if (this.m_Assets != null)
                {
                    AssetServer.ImportPackageStep2(this.m_Assets);
                }
                base.Close();
                GUIUtility.ExitGUI();
            }
            GUILayout.Space(10f);
            GUILayout.EndHorizontal();
            GUILayout.Space(5f);
            GUILayout.EndVertical();
        }

        private void DestroyCreatedIcons()
        {
            if (s_Preview != null)
            {
                UnityEngine.Object.DestroyImmediate(s_Preview);
                s_Preview = null;
                s_LastPreviewPath = null;
            }
            if (s_PackageIcon != null)
            {
                UnityEngine.Object.DestroyImmediate(s_PackageIcon);
                s_PackageIcon = null;
            }
        }

        public static void DrawTexture(Rect r, Texture2D tex, bool useDropshadow)
        {
            if (tex != null)
            {
                float width = tex.width;
                float height = tex.height;
                if ((width >= height) && (width > r.width))
                {
                    height = (height * r.width) / width;
                    width = r.width;
                }
                else if ((height > width) && (height > r.height))
                {
                    width = (width * r.height) / height;
                    height = r.height;
                }
                float x = r.x + Mathf.Round((r.width - width) / 2f);
                float y = r.y + Mathf.Round((r.height - height) / 2f);
                r = new Rect(x, y, width, height);
                if (useDropshadow && (Event.current.type == EventType.Repaint))
                {
                    Rect position = new RectOffset(1, 1, 1, 1).Remove(ms_Constants.textureIconDropShadow.border.Add(r));
                    ms_Constants.textureIconDropShadow.Draw(position, GUIContent.none, false, false, false, false);
                }
                GUI.DrawTexture(r, tex, ScaleMode.ScaleToFit, true);
            }
        }

        public static Texture2D GetPreview(string previewPath)
        {
            if (previewPath != s_LastPreviewPath)
            {
                s_LastPreviewPath = previewPath;
                LoadTexture(previewPath, ref s_Preview);
            }
            return s_Preview;
        }

        public static bool HasInvalidCharInFilePath(string filePath)
        {
            char ch;
            int num;
            return HasInvalidCharInFilePath(filePath, out ch, out num);
        }

        private static bool HasInvalidCharInFilePath(string filePath, out char invalidChar, out int invalidCharIndex)
        {
            for (int i = 0; i < filePath.Length; i++)
            {
                char ch = filePath[i];
                if (s_InvalidPathChars.Contains<char>(ch))
                {
                    invalidChar = ch;
                    invalidCharIndex = i;
                    return true;
                }
            }
            invalidChar = ' ';
            invalidCharIndex = -1;
            return false;
        }

        private void Init(string packagePath, AssetsItem[] items, string packageIconPath)
        {
            this.DestroyCreatedIcons();
            this.m_TreeViewState = null;
            this.m_Tree = null;
            this.m_EnabledFolders = null;
            this.m_Assets = items;
            this.m_PackageName = Path.GetFileNameWithoutExtension(packagePath);
            this.m_PackageIconPath = packageIconPath;
            base.Repaint();
        }

        private static bool IsAllFilePathsValid(AssetsItem[] assetItems, out string errorMessage)
        {
            foreach (AssetsItem item in assetItems)
            {
                char ch;
                int num2;
                if ((item.assetIsDir != 1) && HasInvalidCharInFilePath(item.pathName, out ch, out num2))
                {
                    errorMessage = string.Format("Invalid character found in file path: '{0}'. Invalid ascii value: {1} (at character index {2}).", item.pathName, (int) ch, num2);
                    return false;
                }
            }
            errorMessage = string.Empty;
            return true;
        }

        private static void LoadTexture(string filepath, ref Texture2D texture)
        {
            if (texture == null)
            {
                texture = new Texture2D(0x80, 0x80);
            }
            byte[] data = null;
            try
            {
                data = File.ReadAllBytes(filepath);
            }
            catch
            {
            }
            if (((filepath == string.Empty) || (data == null)) || !texture.LoadImage(data))
            {
                Color[] pixels = texture.GetPixels();
                for (int i = 0; i < pixels.Length; i++)
                {
                    pixels[i] = new Color(0.5f, 0.5f, 0.5f, 0f);
                }
                texture.SetPixels(pixels);
                texture.Apply();
            }
        }

        public void OnAfterDeserialize()
        {
        }

        public void OnBeforeSerialize()
        {
            if (this.m_Tree != null)
            {
                if (this.m_EnabledFolders == null)
                {
                    this.m_EnabledFolders = new List<string>();
                }
                this.m_EnabledFolders.Clear();
                this.m_Tree.GetEnabledFolders(this.m_EnabledFolders);
            }
        }

        private void OnDisable()
        {
            this.DestroyCreatedIcons();
        }

        public void OnGUI()
        {
            if (ms_Constants == null)
            {
                ms_Constants = new Constants();
            }
            if (this.m_Assets != null)
            {
                if (this.m_TreeViewState == null)
                {
                    this.m_TreeViewState = new TreeViewState();
                }
                if (this.m_Tree == null)
                {
                    this.m_Tree = new PackageImportTreeView(this.m_Assets, this.m_EnabledFolders, this.m_TreeViewState, this, new Rect());
                }
                if (this.m_Assets.Length > 0)
                {
                    this.TopArea();
                    this.m_Tree.OnGUI(GUILayoutUtility.GetRect(1f, 9999f, (float) 1f, (float) 99999f));
                    this.BottomArea();
                }
                else
                {
                    GUILayout.Label("Nothing to import!", EditorStyles.boldLabel, new GUILayoutOption[0]);
                    GUILayout.Label("All assets from this package are already in your project.", "WordWrappedLabel", new GUILayoutOption[0]);
                    GUILayout.FlexibleSpace();
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("OK", new GUILayoutOption[0]))
                    {
                        base.Close();
                        GUIUtility.ExitGUI();
                    }
                    GUILayout.EndHorizontal();
                }
            }
        }

        public static void ShowImportPackage(string packagePath, AssetsItem[] items, string packageIconPath)
        {
            if (ValidateInput(items))
            {
                EditorWindow.GetWindow<PackageImport>(true, "Importing package").Init(packagePath, items, packageIconPath);
            }
        }

        private void TopArea()
        {
            Rect rect2;
            if ((s_PackageIcon == null) && !string.IsNullOrEmpty(this.m_PackageIconPath))
            {
                LoadTexture(this.m_PackageIconPath, ref s_PackageIcon);
            }
            bool flag = s_PackageIcon != null;
            float height = !flag ? 52f : 84f;
            Rect position = GUILayoutUtility.GetRect(base.position.width, height);
            GUI.Label(position, GUIContent.none, ms_Constants.topBarBg);
            if (flag)
            {
                Rect r = new Rect(position.x + 10f, position.y + 10f, 64f, 64f);
                DrawTexture(r, s_PackageIcon, true);
                rect2 = new Rect(r.xMax + 10f, r.yMin, position.width, r.height);
            }
            else
            {
                rect2 = new Rect(position.x + 5f, position.yMin, position.width, position.height);
            }
            GUI.Label(rect2, this.m_PackageName, ms_Constants.title);
        }

        private static bool ValidateInput(AssetsItem[] items)
        {
            string str;
            if (!IsAllFilePathsValid(items, out str))
            {
                str = str + "\nDo you want to import the valid file paths of the package or cancel importing?";
                return EditorUtility.DisplayDialog("Invalid file path found", str, "Import", "Cancel importing");
            }
            return true;
        }

        internal class Constants
        {
            public GUIContent badgeNew = EditorGUIUtility.IconContent("AS Badge New");
            public GUIStyle bottomBarBg = "ProjectBrowserBottomBarBg";
            public GUIStyle ConsoleEntryBackEven = "CN EntryBackEven";
            public GUIStyle ConsoleEntryBackOdd = "CN EntryBackOdd";
            public Color lineColor = (!EditorGUIUtility.isProSkin ? new Color(0.4f, 0.4f, 0.4f) : new Color(0.1f, 0.1f, 0.1f));
            public GUIStyle textureIconDropShadow = "ProjectBrowserTextureIconDropShadow";
            public GUIStyle title = new GUIStyle(EditorStyles.largeLabel);
            public GUIStyle topBarBg = new GUIStyle("ProjectBrowserHeaderBgTop");

            public Constants()
            {
                this.topBarBg.fixedHeight = 0f;
                int num = 2;
                this.topBarBg.border.bottom = num;
                this.topBarBg.border.top = num;
                this.title.fontStyle = FontStyle.Bold;
                this.title.alignment = TextAnchor.MiddleLeft;
            }
        }
    }
}

