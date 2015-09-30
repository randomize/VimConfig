namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using UnityEditor.ProjectWindowCallback;
    using UnityEditorInternal;
    using UnityEngine;

    public class ProjectWindowUtil
    {
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map1D;
        internal static string k_DraggingFavoriteGenericData = "DraggingFavorite";
        internal static int k_FavoritesStartInstanceID = 0x3b9aca00;
        internal static string k_IsFolderGenericData = "IsFolder";

        private static void CreateAnimatorController()
        {
            Texture2D image = EditorGUIUtility.IconContent("AnimatorController Icon").image as Texture2D;
            StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<DoCreateAnimatorController>(), "New Animator Controller.controller", image, null);
        }

        public static void CreateAsset(UnityEngine.Object asset, string pathName)
        {
            StartNameEditingIfProjectWindowExists(asset.GetInstanceID(), ScriptableObject.CreateInstance<DoCreateNewAsset>(), pathName, AssetPreview.GetMiniThumbnail(asset), null);
        }

        private static void CreateAudioMixer()
        {
            Texture2D image = EditorGUIUtility.IconContent("AudioMixerController Icon").image as Texture2D;
            StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<DoCreateAudioMixer>(), "NewAudioMixer.mixer", image, null);
        }

        public static void CreateFolder()
        {
            StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<DoCreateFolder>(), "New Folder", EditorGUIUtility.IconContent(EditorResourcesUtility.emptyFolderIconName).image as Texture2D, null);
        }

        [UnityEditor.MenuItem("Assets/Create/GUI Skin", false, 0x259)]
        public static void CreateNewGUISkin()
        {
            GUISkin dest = ScriptableObject.CreateInstance<GUISkin>();
            GUISkin builtinResource = Resources.GetBuiltinResource(typeof(GUISkin), "GameSkin/GameSkin.guiskin") as GUISkin;
            if (builtinResource != null)
            {
                EditorUtility.CopySerialized(builtinResource, dest);
            }
            else
            {
                Debug.LogError("Internal error: unable to load builtin GUIskin");
            }
            CreateAsset(dest, "New GUISkin.guiskin");
        }

        public static void CreatePrefab()
        {
            StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<DoCreatePrefab>(), "New Prefab.prefab", EditorGUIUtility.IconContent("Prefab Icon").image as Texture2D, null);
        }

        private static void CreateScriptAsset(string templatePath, string destName)
        {
            Texture2D icon = null;
            string extension = Path.GetExtension(destName);
            if (extension != null)
            {
                int num;
                if (<>f__switch$map1D == null)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(4);
                    dictionary.Add(".js", 0);
                    dictionary.Add(".cs", 1);
                    dictionary.Add(".boo", 2);
                    dictionary.Add(".shader", 3);
                    <>f__switch$map1D = dictionary;
                }
                if (<>f__switch$map1D.TryGetValue(extension, out num))
                {
                    switch (num)
                    {
                        case 0:
                            icon = EditorGUIUtility.IconContent("js Script Icon").image as Texture2D;
                            goto Label_0105;

                        case 1:
                            icon = EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;
                            goto Label_0105;

                        case 2:
                            icon = EditorGUIUtility.IconContent("boo Script Icon").image as Texture2D;
                            goto Label_0105;

                        case 3:
                            icon = EditorGUIUtility.IconContent("Shader Icon").image as Texture2D;
                            goto Label_0105;
                    }
                }
            }
            icon = EditorGUIUtility.IconContent("TextAsset Icon").image as Texture2D;
        Label_0105:
            StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<DoCreateScriptAsset>(), destName, icon, templatePath);
        }

        internal static UnityEngine.Object CreateScriptAssetFromTemplate(string pathName, string resourceFile)
        {
            string fullPath = Path.GetFullPath(pathName);
            StreamReader reader = new StreamReader(resourceFile);
            string input = reader.ReadToEnd();
            reader.Close();
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
            input = Regex.Replace(input, "#NAME#", fileNameWithoutExtension);
            string replacement = Regex.Replace(fileNameWithoutExtension, " ", string.Empty);
            input = Regex.Replace(input, "#SCRIPTNAME#", replacement);
            if (char.IsUpper(replacement, 0))
            {
                replacement = char.ToLower(replacement[0]) + replacement.Substring(1);
                input = Regex.Replace(input, "#SCRIPTNAME_LOWER#", replacement);
            }
            else
            {
                replacement = "my" + char.ToUpper(replacement[0]) + replacement.Substring(1);
                input = Regex.Replace(input, "#SCRIPTNAME_LOWER#", replacement);
            }
            bool flag = true;
            bool throwOnInvalidBytes = false;
            UTF8Encoding encoding = new UTF8Encoding(flag, throwOnInvalidBytes);
            bool append = false;
            StreamWriter writer = new StreamWriter(fullPath, append, encoding);
            writer.Write(input);
            writer.Close();
            AssetDatabase.ImportAsset(pathName);
            return AssetDatabase.LoadAssetAtPath(pathName, typeof(UnityEngine.Object));
        }

        internal static void DuplicateSelectedAssets()
        {
            AssetDatabase.Refresh();
            UnityEngine.Object[] objects = Selection.objects;
            bool flag = true;
            foreach (UnityEngine.Object obj2 in objects)
            {
                AnimationClip clip = obj2 as AnimationClip;
                if (((clip == null) || ((clip.hideFlags & HideFlags.NotEditable) == HideFlags.None)) || !AssetDatabase.Contains(clip))
                {
                    flag = false;
                }
            }
            ArrayList list = new ArrayList();
            bool flag2 = false;
            if (flag)
            {
                foreach (UnityEngine.Object obj3 in objects)
                {
                    AnimationClip source = obj3 as AnimationClip;
                    if ((source != null) && ((source.hideFlags & HideFlags.NotEditable) != HideFlags.None))
                    {
                        string path = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(Path.GetDirectoryName(AssetDatabase.GetAssetPath(obj3)), source.name) + ".anim");
                        AnimationClip dest = new AnimationClip();
                        EditorUtility.CopySerialized(source, dest);
                        AssetDatabase.CreateAsset(dest, path);
                        list.Add(path);
                    }
                }
            }
            else
            {
                foreach (UnityEngine.Object obj4 in Selection.GetFiltered(typeof(UnityEngine.Object), UnityEditor.SelectionMode.Assets))
                {
                    string assetPath = AssetDatabase.GetAssetPath(obj4);
                    string newPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
                    if (newPath.Length != 0)
                    {
                        flag2 |= !AssetDatabase.CopyAsset(assetPath, newPath);
                    }
                    else
                    {
                        flag2 |= true;
                    }
                    if (!flag2)
                    {
                        list.Add(newPath);
                    }
                }
            }
            AssetDatabase.Refresh();
            UnityEngine.Object[] objArray6 = new UnityEngine.Object[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                objArray6[i] = AssetDatabase.LoadMainAssetAtPath(list[i] as string);
            }
            Selection.objects = objArray6;
        }

        internal static void EndNameEditAction(UnityEditor.ProjectWindowCallback.EndNameEditAction action, int instanceId, string pathName, string resourceFile)
        {
            pathName = AssetDatabase.GenerateUniqueAssetPath(pathName);
            if (action != null)
            {
                action.Action(instanceId, pathName, resourceFile);
                action.CleanUp();
            }
        }

        internal static void FrameObjectInProjectWindow(int instanceID)
        {
            ProjectBrowser projectBrowserIfExists = GetProjectBrowserIfExists();
            if (projectBrowserIfExists != null)
            {
                projectBrowserIfExists.FrameObject(instanceID, false);
            }
        }

        internal static string GetActiveFolderPath()
        {
            ProjectBrowser projectBrowserIfExists = GetProjectBrowserIfExists();
            if (projectBrowserIfExists == null)
            {
                return "Assets";
            }
            return projectBrowserIfExists.GetActiveFolderPath();
        }

        public static string[] GetBaseFolders(string[] folders)
        {
            if (folders.Length < 2)
            {
                return folders;
            }
            List<string> list = new List<string>();
            List<string> list2 = new List<string>(folders);
            list2.Sort();
            string item = list2[0];
            list.Add(item);
            for (int i = 1; i < list2.Count; i++)
            {
                if (list2[i].IndexOf(item) < 0)
                {
                    list.Add(list2[i]);
                    item = list2[i];
                }
            }
            return list.ToArray();
        }

        internal static UnityEngine.Object[] GetDragAndDropObjects(int draggedInstanceID, List<int> selectedInstanceIDs)
        {
            if (selectedInstanceIDs.Contains(draggedInstanceID))
            {
                UnityEngine.Object[] objArray = new UnityEngine.Object[selectedInstanceIDs.Count];
                for (int i = 0; i < selectedInstanceIDs.Count; i++)
                {
                    objArray[i] = InternalEditorUtility.GetObjectFromInstanceID(selectedInstanceIDs[i]);
                }
                return objArray;
            }
            return new UnityEngine.Object[] { InternalEditorUtility.GetObjectFromInstanceID(draggedInstanceID) };
        }

        internal static string[] GetDragAndDropPaths(int draggedInstanceID, List<int> selectedInstanceIDs)
        {
            List<string> list = new List<string>();
            foreach (int num in selectedInstanceIDs)
            {
                if (AssetDatabase.IsMainAsset(num))
                {
                    string item = AssetDatabase.GetAssetPath(num);
                    list.Add(item);
                }
            }
            string assetPath = AssetDatabase.GetAssetPath(draggedInstanceID);
            if (string.IsNullOrEmpty(assetPath))
            {
                return new string[0];
            }
            if (list.Contains(assetPath))
            {
                return list.ToArray();
            }
            return new string[] { assetPath };
        }

        private static ProjectBrowser GetProjectBrowserIfExists()
        {
            return ProjectBrowser.s_LastInteractedProjectBrowser;
        }

        internal static bool IsFavoritesItem(int instanceID)
        {
            return (instanceID >= k_FavoritesStartInstanceID);
        }

        public static void ShowCreatedAsset(UnityEngine.Object o)
        {
            Selection.activeObject = o;
            if (o != null)
            {
                FrameObjectInProjectWindow(o.GetInstanceID());
            }
        }

        internal static void StartDrag(int draggedInstanceID, List<int> selectedInstanceIDs)
        {
            DragAndDrop.PrepareStartDrag();
            string title = string.Empty;
            if (IsFavoritesItem(draggedInstanceID))
            {
                DragAndDrop.SetGenericData(k_DraggingFavoriteGenericData, draggedInstanceID);
                DragAndDrop.objectReferences = new UnityEngine.Object[0];
            }
            else
            {
                bool isFolder = false;
                HierarchyProperty property = new HierarchyProperty(HierarchyType.Assets);
                if (property.Find(draggedInstanceID, null))
                {
                    isFolder = property.isFolder;
                }
                DragAndDrop.objectReferences = GetDragAndDropObjects(draggedInstanceID, selectedInstanceIDs);
                DragAndDrop.SetGenericData(k_IsFolderGenericData, !isFolder ? string.Empty : "isFolder");
                string[] dragAndDropPaths = GetDragAndDropPaths(draggedInstanceID, selectedInstanceIDs);
                if (dragAndDropPaths.Length > 0)
                {
                    DragAndDrop.paths = dragAndDropPaths;
                }
                if (DragAndDrop.objectReferences.Length > 1)
                {
                    title = "<Multiple>";
                }
                else
                {
                    title = ObjectNames.GetDragAndDropTitle(InternalEditorUtility.GetObjectFromInstanceID(draggedInstanceID));
                }
            }
            DragAndDrop.StartDrag(title);
        }

        public static void StartNameEditingIfProjectWindowExists(int instanceID, UnityEditor.ProjectWindowCallback.EndNameEditAction endAction, string pathName, Texture2D icon, string resourceFile)
        {
            ProjectBrowser projectBrowserIfExists = GetProjectBrowserIfExists();
            if (projectBrowserIfExists != null)
            {
                projectBrowserIfExists.Focus();
                projectBrowserIfExists.BeginPreimportedNameEditing(instanceID, endAction, pathName, icon, resourceFile);
                projectBrowserIfExists.Repaint();
            }
            else
            {
                if (!pathName.StartsWith("assets/", StringComparison.CurrentCultureIgnoreCase))
                {
                    pathName = "Assets/" + pathName;
                }
                EndNameEditAction(endAction, instanceID, pathName, resourceFile);
                Selection.activeObject = EditorUtility.InstanceIDToObject(instanceID);
            }
        }
    }
}

