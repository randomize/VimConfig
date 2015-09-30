namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Internal;

    public sealed class EditorApplication
    {
        internal static CallbackFunction assetBundleNameChanged;
        internal static CallbackFunction assetLabelsChanged;
        public static CallbackFunction delayCall;
        private static CallbackFunction delayedCallback;
        internal static CallbackFunction globalEventHandler;
        public static CallbackFunction hierarchyWindowChanged;
        public static HierarchyWindowItemCallback hierarchyWindowItemOnGUI;
        public static CallbackFunction modifierKeysChanged;
        public static CallbackFunction playmodeStateChanged;
        public static CallbackFunction projectWindowChanged;
        public static ProjectWindowItemCallback projectWindowItemOnGUI;
        private static float s_DelayedCallbackTime;
        public static CallbackFunction searchChanged;
        public static CallbackFunction update;
        internal static CallbackFunction windowsReordered;

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Beep();
        internal static void CallDelayed(CallbackFunction function, float timeFromNow)
        {
            delayedCallback = function;
            s_DelayedCallbackTime = Time.realtimeSinceStartup + timeFromNow;
            update = (CallbackFunction) Delegate.Combine(update, new CallbackFunction(EditorApplication.CheckCallDelayed));
        }

        private static void CheckCallDelayed()
        {
            if (Time.realtimeSinceStartup > s_DelayedCallbackTime)
            {
                update = (CallbackFunction) Delegate.Remove(update, new CallbackFunction(EditorApplication.CheckCallDelayed));
                delayedCallback();
            }
        }

        public static void DirtyHierarchyWindowSorting()
        {
            foreach (SceneHierarchyWindow window in Resources.FindObjectsOfTypeAll(typeof(SceneHierarchyWindow)))
            {
                window.DirtySortingMethods();
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool ExecuteMenuItem(string menuItemPath);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool ExecuteMenuItemOnGameObjects(string menuItemPath, GameObject[] objects);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool ExecuteMenuItemWithTemporaryContext(string menuItemPath, UnityEngine.Object[] objects);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Exit(int returnValue);
        internal static void Internal_CallAssetBundleNameChanged()
        {
            if (assetBundleNameChanged != null)
            {
                assetBundleNameChanged();
            }
        }

        internal static void Internal_CallAssetLabelsHaveChanged()
        {
            if (assetLabelsChanged != null)
            {
                assetLabelsChanged();
            }
        }

        private static void Internal_CallDelayFunctions()
        {
            CallbackFunction delayCall = EditorApplication.delayCall;
            EditorApplication.delayCall = null;
            if (delayCall != null)
            {
                delayCall();
            }
        }

        private static void Internal_CallGlobalEventHandler()
        {
            if (globalEventHandler != null)
            {
                globalEventHandler();
            }
            WindowLayout.MaximizeKeyHandler();
            Event.current = null;
        }

        private static void Internal_CallHierarchyWindowHasChanged()
        {
            if (hierarchyWindowChanged != null)
            {
                hierarchyWindowChanged();
            }
        }

        private static void Internal_CallKeyboardModifiersChanged()
        {
            if (modifierKeysChanged != null)
            {
                modifierKeysChanged();
            }
        }

        private static void Internal_CallProjectWindowHasChanged()
        {
            if (projectWindowChanged != null)
            {
                projectWindowChanged();
            }
        }

        internal static void Internal_CallSearchHasChanged()
        {
            if (searchChanged != null)
            {
                searchChanged();
            }
        }

        private static void Internal_CallUpdateFunctions()
        {
            if (update != null)
            {
                update();
            }
        }

        private static void Internal_CallWindowsReordered()
        {
            if (windowsReordered != null)
            {
                windowsReordered();
            }
        }

        private static void Internal_PlaymodeStateChanged()
        {
            if (playmodeStateChanged != null)
            {
                playmodeStateChanged();
            }
        }

        private static void Internal_RepaintAllViews()
        {
            foreach (GUIView view in Resources.FindObjectsOfTypeAll(typeof(GUIView)))
            {
                view.Repaint();
            }
        }

        private static void Internal_SwitchSkin()
        {
            EditorGUIUtility.Internal_SwitchSkin();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern AsyncOperation LoadLevelAdditiveAsyncInPlayMode(string path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void LoadLevelAdditiveInPlayMode(string path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern AsyncOperation LoadLevelAsyncInPlayMode(string path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void LoadLevelInPlayMode(string path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void LockReloadAssemblies();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void MarkSceneDirty();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void NewEmptyScene();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void NewScene();
        public static void OpenProject(string projectPath, params string[] args)
        {
            OpenProjectInternal(projectPath, args);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void OpenProjectInternal(string projectPath, string[] args);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool OpenScene(string path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void OpenSceneAdditive(string path);
        public static void RepaintAnimationWindow()
        {
            foreach (AnimEditor editor in AnimEditor.GetAllAnimationWindows())
            {
                editor.Repaint();
            }
        }

        public static void RepaintHierarchyWindow()
        {
            foreach (SceneHierarchyWindow window in Resources.FindObjectsOfTypeAll(typeof(SceneHierarchyWindow)))
            {
                window.Repaint();
            }
        }

        public static void RepaintProjectWindow()
        {
            foreach (ProjectBrowser browser in ProjectBrowser.GetAllProjectBrowsers())
            {
                browser.Repaint();
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void ReportUNetWeaver(string filename, string msg, bool isError);
        internal static void RequestRepaintAllViews()
        {
            Internal_RepaintAllViews();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SaveAssets();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool SaveCurrentSceneIfUserWantsTo();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool SaveCurrentSceneIfUserWantsToForce();
        [ExcludeFromDocs]
        public static bool SaveScene()
        {
            bool saveAsCopy = false;
            return SaveScene(string.Empty, saveAsCopy);
        }

        [ExcludeFromDocs]
        public static bool SaveScene(string path)
        {
            bool saveAsCopy = false;
            return SaveScene(path, saveAsCopy);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool SaveScene([DefaultValue("\"\"")] string path, [DefaultValue("false")] bool saveAsCopy);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SetSceneRepaintDirty();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Step();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void UnlockReloadAssemblies();

        public static string applicationContentsPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string applicationPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string currentScene { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool isCompiling { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool isPaused { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool isPlaying { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool isPlayingOrWillChangePlaymode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool isRemoteConnected { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool isSceneDirty { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool isUpdating { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        internal static UnityEngine.Object renderSettings { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        internal static UnityEngine.Object tagManager { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static double timeSinceStartup { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public delegate void CallbackFunction();

        public delegate void HierarchyWindowItemCallback(int instanceID, Rect selectionRect);

        public delegate void ProjectWindowItemCallback(string guid, Rect selectionRect);
    }
}

