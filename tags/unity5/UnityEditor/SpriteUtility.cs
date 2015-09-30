namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    internal static class SpriteUtility
    {
        [CompilerGenerated]
        private static Comparison<UnityEngine.Object> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<UnityEngine.Object, bool> <>f__am$cache2;
        private static GameObject s_SceneDragObject;

        public static void AddAnimationToGO(GameObject go, Sprite[] frames)
        {
            if ((frames != null) && (frames.Length > 0))
            {
                SpriteRenderer component = go.GetComponent<SpriteRenderer>();
                if (component == null)
                {
                    Debug.LogWarning("There should be a SpriteRenderer in dragged object");
                    component = go.AddComponent<SpriteRenderer>();
                }
                component.sprite = frames[0];
                if (frames.Length > 1)
                {
                    Analytics.Event("Sprite Drag and Drop", "Drop multiple sprites to scene", "null", 1);
                    if (!CreateAnimation(go, frames))
                    {
                        Debug.LogError("Failed to create animation for dragged object");
                    }
                }
                else
                {
                    Analytics.Event("Sprite Drag and Drop", "Drop single sprite to scene", "null", 1);
                }
            }
        }

        private static void AddSpriteAnimationToClip(AnimationClip newClip, UnityEngine.Object[] frames)
        {
            newClip.frameRate = 12f;
            ObjectReferenceKeyframe[] keyframes = new ObjectReferenceKeyframe[frames.Length];
            for (int i = 0; i < keyframes.Length; i++)
            {
                keyframes[i] = new ObjectReferenceKeyframe();
                keyframes[i].value = RemapObjectToSprite(frames[i]);
                keyframes[i].time = ((float) i) / newClip.frameRate;
            }
            EditorCurveBinding binding = EditorCurveBinding.PPtrCurve(string.Empty, typeof(SpriteRenderer), "m_Sprite");
            AnimationUtility.SetObjectReferenceCurve(newClip, binding, keyframes);
        }

        private static bool CreateAnimation(GameObject gameObject, UnityEngine.Object[] frames)
        {
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = (a, b) => EditorUtility.NaturalCompare(a.name, b.name);
            }
            Array.Sort<UnityEngine.Object>(frames, <>f__am$cache1);
            if (AnimationWindowUtility.EnsureActiveAnimationPlayer(gameObject) == null)
            {
                return false;
            }
            Animator closestAnimatorInParents = AnimationWindowUtility.GetClosestAnimatorInParents(gameObject.transform);
            if (closestAnimatorInParents == null)
            {
                return false;
            }
            bool flag = false;
            int index = 0;
            while (index < frames[0].name.Length)
            {
                char ch = frames[0].name.ElementAt<char>(index);
                for (int i = 1; i < frames.Length; i++)
                {
                    if (frames[i].name.ElementAt<char>(index) != ch)
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    break;
                }
                index++;
            }
            string str = frames[0].name.Substring(0, index);
            if (string.IsNullOrEmpty(str))
            {
                str = "New Animation";
            }
            AnimationClip newClip = AnimationWindowUtility.CreateNewClipAtPath(Path.Combine(Path.GetDirectoryName(AssetDatabase.GetAssetPath(frames[0])), str + ".anim"));
            if (newClip == null)
            {
                return false;
            }
            AddSpriteAnimationToClip(newClip, frames);
            return AnimationWindowUtility.AddClipToAnimatorComponent(closestAnimatorInParents, newClip);
        }

        public static GameObject CreateDragGO(Sprite frame, Vector3 position)
        {
            string name = !string.IsNullOrEmpty(frame.name) ? frame.name : "Sprite";
            GameObject obj2 = new GameObject(GameObjectUtility.GetUniqueNameForSibling(null, name));
            obj2.AddComponent<SpriteRenderer>().sprite = frame;
            obj2.transform.position = position;
            return obj2;
        }

        public static GameObject DropSpriteToSceneToCreateGO(Sprite sprite, Vector3 position)
        {
            GameObject obj2 = new GameObject(!string.IsNullOrEmpty(sprite.name) ? sprite.name : "Sprite");
            obj2.AddComponent<SpriteRenderer>().sprite = sprite;
            obj2.transform.position = position;
            Selection.activeObject = obj2;
            return obj2;
        }

        private static void ForcedImportFor(string newPath)
        {
            try
            {
                AssetDatabase.StartAssetEditing();
                AssetDatabase.ImportAsset(newPath);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }

        private static Sprite GenerateDefaultSprite(Texture2D texture)
        {
            string assetPath = AssetDatabase.GetAssetPath(texture);
            TextureImporter atPath = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if ((atPath.textureType != TextureImporterType.Sprite) && (atPath.textureType != TextureImporterType.Advanced))
            {
                return null;
            }
            if (atPath.spriteImportMode == SpriteImportMode.None)
            {
                if (atPath.textureType == TextureImporterType.Advanced)
                {
                    return null;
                }
                atPath.spriteImportMode = SpriteImportMode.Single;
                AssetDatabase.WriteImportSettingsIfDirty(assetPath);
                ForcedImportFor(assetPath);
            }
            UnityEngine.Object obj2 = null;
            try
            {
                if (<>f__am$cache2 == null)
                {
                    <>f__am$cache2 = t => t is Sprite;
                }
                obj2 = AssetDatabase.LoadAllAssetsAtPath(assetPath).First<UnityEngine.Object>(<>f__am$cache2);
            }
            catch (Exception)
            {
                Debug.LogWarning("Texture being dragged has no Sprites.");
            }
            return (obj2 as Sprite);
        }

        private static Vector3 GetDefaultInstantiatePosition()
        {
            Vector3 zero = Vector3.zero;
            if (SceneView.lastActiveSceneView == null)
            {
                return zero;
            }
            if (SceneView.lastActiveSceneView.in2DMode)
            {
                zero = SceneView.lastActiveSceneView.camera.transform.position;
                zero.z = 0f;
                return zero;
            }
            return SceneView.lastActiveSceneView.cameraTargetPosition;
        }

        public static Sprite[] GetSpriteFromDraggedPathsOrObjects()
        {
            List<Sprite> list = new List<Sprite>();
            foreach (UnityEngine.Object obj2 in DragAndDrop.objectReferences)
            {
                if (AssetDatabase.Contains(obj2))
                {
                    if (obj2 is Sprite)
                    {
                        list.Add(obj2 as Sprite);
                    }
                    else if (obj2 is Texture2D)
                    {
                        list.AddRange(TextureToSprites(obj2 as Texture2D));
                    }
                }
            }
            if (list.Count > 0)
            {
                return list.ToArray();
            }
            Sprite sprite = HandleExternalDrag(Event.current.type == EventType.DragPerform);
            if (sprite != null)
            {
                return new Sprite[] { sprite };
            }
            return null;
        }

        public static Sprite[] GetSpritesFromDraggedObjects()
        {
            List<Sprite> list = new List<Sprite>();
            foreach (UnityEngine.Object obj2 in DragAndDrop.objectReferences)
            {
                if (obj2.GetType() == typeof(Sprite))
                {
                    list.Add(obj2 as Sprite);
                }
                else if (obj2.GetType() == typeof(Texture2D))
                {
                    list.AddRange(TextureToSprites(obj2 as Texture2D));
                }
            }
            return list.ToArray();
        }

        private static Sprite HandleExternalDrag(bool perform)
        {
            if (DragAndDrop.paths.Length == 0)
            {
                return null;
            }
            string path = DragAndDrop.paths[0];
            if (!ValidPathForTextureAsset(path))
            {
                return null;
            }
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            if (!perform)
            {
                return null;
            }
            string to = AssetDatabase.GenerateUniqueAssetPath(Path.Combine("Assets", FileUtil.GetLastPathNameComponent(path)));
            if (to.Length <= 0)
            {
                return null;
            }
            FileUtil.CopyFileOrDirectory(path, to);
            ForcedImportFor(to);
            return GenerateDefaultSprite(AssetDatabase.LoadMainAssetAtPath(to) as Texture2D);
        }

        public static bool HandleMultipleSpritesDragIntoHierarchy(IHierarchyProperty property, Sprite[] sprites, bool perform)
        {
            GameObject gameObject = null;
            if ((property == null) || (property.pptrValue == null))
            {
                if (perform)
                {
                    Analytics.Event("Sprite Drag and Drop", "Drop multiple sprites to empty hierarchy", "null", 1);
                    gameObject = new GameObject {
                        name = !string.IsNullOrEmpty(sprites[0].name) ? sprites[0].name : "Sprite"
                    };
                    gameObject.transform.position = GetDefaultInstantiatePosition();
                }
            }
            else
            {
                gameObject = property.pptrValue as GameObject;
                if (perform)
                {
                    Analytics.Event("Sprite Drag and Drop", "Drop multiple sprites to gameobject", "null", 1);
                }
            }
            if (perform)
            {
                SpriteRenderer component = gameObject.GetComponent<SpriteRenderer>();
                if (component == null)
                {
                    component = gameObject.AddComponent<SpriteRenderer>();
                }
                if (component == null)
                {
                    return true;
                }
                if (component.sprite == null)
                {
                    component.sprite = sprites[0];
                }
                CreateAnimation(gameObject, sprites);
                Selection.activeGameObject = gameObject;
            }
            return true;
        }

        public static bool HandleSingleSpriteDragIntoHierarchy(IHierarchyProperty property, Sprite sprite, bool perform)
        {
            GameObject pptrValue = null;
            if ((property != null) && (property.pptrValue != null))
            {
                pptrValue = property.pptrValue as GameObject;
            }
            if (perform)
            {
                Vector3 defaultInstantiatePosition = GetDefaultInstantiatePosition();
                GameObject obj4 = DropSpriteToSceneToCreateGO(sprite, defaultInstantiatePosition);
                if (pptrValue != null)
                {
                    Analytics.Event("Sprite Drag and Drop", "Drop single sprite to existing gameobject", "null", 1);
                    obj4.transform.parent = pptrValue.transform;
                    obj4.transform.localPosition = Vector3.zero;
                }
                else
                {
                    Analytics.Event("Sprite Drag and Drop", "Drop single sprite to empty hierarchy", "null", 1);
                }
                Selection.activeGameObject = obj4;
            }
            return true;
        }

        public static void OnSceneDrag(SceneView sceneView)
        {
            Event current = Event.current;
            if ((((current.type == EventType.DragUpdated) || (current.type == EventType.DragPerform)) || (current.type == EventType.DragExited)) && ((sceneView.in2DMode || (HandleUtility.PickGameObject(Event.current.mousePosition, true) == null)) || ((DragAndDrop.objectReferences.Length != 1) || !(DragAndDrop.objectReferences[0] is Texture))))
            {
                EventType type = current.type;
                if (type == EventType.DragUpdated)
                {
                    if (s_SceneDragObject == null)
                    {
                        Sprite[] spriteFromDraggedPathsOrObjects = GetSpriteFromDraggedPathsOrObjects();
                        if ((spriteFromDraggedPathsOrObjects == null) || (spriteFromDraggedPathsOrObjects.Length == 0))
                        {
                            return;
                        }
                        Sprite sprite = spriteFromDraggedPathsOrObjects[0];
                        if (sprite == null)
                        {
                            return;
                        }
                        s_SceneDragObject = CreateDragGO(spriteFromDraggedPathsOrObjects[0], Vector3.zero);
                        HandleUtility.ignoreRaySnapObjects = s_SceneDragObject.GetComponentsInChildren<Transform>();
                        s_SceneDragObject.hideFlags = HideFlags.HideInHierarchy;
                        s_SceneDragObject.name = sprite.name;
                    }
                    Vector3 zero = Vector3.zero;
                    zero = HandleUtility.GUIPointToWorldRay(current.mousePosition).GetPoint(10f);
                    if (sceneView.in2DMode)
                    {
                        zero.z = 0f;
                    }
                    else
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        object obj3 = HandleUtility.RaySnap(HandleUtility.GUIPointToWorldRay(current.mousePosition));
                        if (obj3 != null)
                        {
                            RaycastHit hit = (RaycastHit) obj3;
                            zero = hit.point;
                        }
                    }
                    s_SceneDragObject.transform.position = zero;
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    current.Use();
                }
                else if (type == EventType.DragPerform)
                {
                    Sprite[] frames = GetSpriteFromDraggedPathsOrObjects();
                    if (frames != null)
                    {
                        AddAnimationToGO(s_SceneDragObject, frames);
                        Undo.RegisterCreatedObjectUndo(s_SceneDragObject, "Create Sprite");
                        Selection.activeGameObject = s_SceneDragObject;
                        s_SceneDragObject.hideFlags = HideFlags.None;
                        s_SceneDragObject = null;
                        current.Use();
                    }
                }
                else if ((type == EventType.DragExited) && (s_SceneDragObject != null))
                {
                    UnityEngine.Object.DestroyImmediate(s_SceneDragObject, false);
                    HandleUtility.ignoreRaySnapObjects = null;
                    s_SceneDragObject = null;
                    current.Use();
                }
            }
        }

        public static Sprite RemapObjectToSprite(UnityEngine.Object obj)
        {
            if (obj is Sprite)
            {
                return (Sprite) obj;
            }
            if (obj is Texture2D)
            {
                UnityEngine.Object[] objArray = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(obj));
                for (int i = 0; i < objArray.Length; i++)
                {
                    if (objArray[i].GetType() == typeof(Sprite))
                    {
                        return (objArray[i] as Sprite);
                    }
                }
            }
            return null;
        }

        public static Sprite TextureToSprite(Texture2D tex)
        {
            Sprite[] spriteArray = TextureToSprites(tex);
            if (spriteArray.Length > 0)
            {
                return spriteArray[0];
            }
            return null;
        }

        public static Sprite[] TextureToSprites(Texture2D tex)
        {
            UnityEngine.Object[] objArray = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(tex));
            List<Sprite> list = new List<Sprite>();
            for (int i = 0; i < objArray.Length; i++)
            {
                if (objArray[i].GetType() == typeof(Sprite))
                {
                    list.Add(objArray[i] as Sprite);
                }
            }
            if (list.Count > 0)
            {
                return list.ToArray();
            }
            return new Sprite[] { GenerateDefaultSprite(tex) };
        }

        private static bool ValidPathForTextureAsset(string path)
        {
            string str = FileUtil.GetPathExtension(path).ToLower();
            return ((((((str == "jpg") || (str == "jpeg")) || ((str == "tif") || (str == "tiff"))) || (((str == "tga") || (str == "gif")) || ((str == "png") || (str == "psd")))) || ((((str == "bmp") || (str == "iff")) || ((str == "pict") || (str == "pic"))) || ((str == "pct") || (str == "exr")))) || (str == "hdr"));
        }
    }
}

