namespace UnityEditor
{
    using System;
    using UnityEngine;
    using UnityEngine.Rendering;

    [CustomEditor(typeof(ShaderVariantCollection))]
    internal class ShaderVariantCollectionInspector : Editor
    {
        private SerializedProperty m_Shaders;

        private void AddVariantMenuSelected(object userData, string[] options, int selected)
        {
            AddVariantMenuData data = (AddVariantMenuData) userData;
            char[] separator = new char[] { ' ' };
            string[] keywords = data.keywords[selected].Split(separator);
            ShaderVariantCollection.ShaderVariant variant = new ShaderVariantCollection.ShaderVariant(data.shader, (PassType) data.types[selected], keywords);
            Undo.RecordObject(data.collection, "Add variant");
            data.collection.Add(variant);
        }

        private void DisplayAddVariantsMenu(Rect rect, Shader shader, ShaderVariantCollection collection)
        {
            AddVariantMenuData userData = new AddVariantMenuData {
                shader = shader,
                collection = collection
            };
            ShaderUtil.GetShaderVariantEntries(shader, collection, out userData.types, out userData.keywords);
            if (userData.keywords.Length == 0)
            {
                EditorApplication.Beep();
            }
            else
            {
                string[] options = new string[userData.keywords.Length];
                for (int i = 0; i < userData.keywords.Length; i++)
                {
                    options[i] = ((PassType) userData.types[i]) + "/" + (!string.IsNullOrEmpty(userData.keywords[i]) ? userData.keywords[i] : "<no keywords>");
                }
                EditorUtility.DisplayCustomMenu(rect, options, null, new EditorUtility.SelectMenuItemFunction(this.AddVariantMenuSelected), userData);
            }
        }

        private void DrawShaderEntry(int shaderIndex)
        {
            SerializedProperty arrayElementAtIndex = this.m_Shaders.GetArrayElementAtIndex(shaderIndex);
            Shader objectReferenceValue = (Shader) arrayElementAtIndex.FindPropertyRelative("first").objectReferenceValue;
            SerializedProperty property2 = arrayElementAtIndex.FindPropertyRelative("second.variants");
            using (new GUILayout.HorizontalScope(new GUILayoutOption[0]))
            {
                Rect r = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.boldLabel);
                Rect position = GetAddRemoveButtonRect(r);
                r.xMax = position.x;
                GUI.Label(r, objectReferenceValue.name, EditorStyles.boldLabel);
                if (GUI.Button(position, Styles.iconRemove, Styles.invisibleButton))
                {
                    this.m_Shaders.DeleteArrayElementAtIndex(shaderIndex);
                    return;
                }
            }
            for (int i = 0; i < property2.arraySize; i++)
            {
                SerializedProperty property3 = property2.GetArrayElementAtIndex(i);
                string stringValue = property3.FindPropertyRelative("keywords").stringValue;
                if (string.IsNullOrEmpty(stringValue))
                {
                    stringValue = "<no keywords>";
                }
                PassType intValue = (PassType) property3.FindPropertyRelative("passType").intValue;
                Rect rect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.miniLabel);
                Rect rect4 = GetAddRemoveButtonRect(rect);
                rect.xMax = rect4.x;
                GUI.Label(rect, intValue + " " + stringValue, EditorStyles.miniLabel);
                if (GUI.Button(rect4, Styles.iconRemove, Styles.invisibleButton))
                {
                    property2.DeleteArrayElementAtIndex(i);
                }
            }
            Rect addRemoveButtonRect = GetAddRemoveButtonRect(GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.miniLabel));
            if (GUI.Button(addRemoveButtonRect, Styles.iconAdd, Styles.invisibleButton))
            {
                this.DisplayAddVariantsMenu(addRemoveButtonRect, objectReferenceValue, this.target as ShaderVariantCollection);
            }
        }

        private static Rect GetAddRemoveButtonRect(Rect r)
        {
            Vector2 vector = Styles.invisibleButton.CalcSize(Styles.iconRemove);
            return new Rect(r.xMax - vector.x, r.y + ((int) ((r.height / 2f) - (vector.y / 2f))), vector.x, vector.y);
        }

        public virtual void OnEnable()
        {
            this.m_Shaders = base.serializedObject.FindProperty("m_Shaders");
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            for (int i = 0; i < this.m_Shaders.arraySize; i++)
            {
                this.DrawShaderEntry(i);
            }
            if (GUILayout.Button("Add shader", new GUILayoutOption[0]))
            {
                ObjectSelector.get.Show(null, typeof(Shader), null, false);
                ObjectSelector.get.objectSelectorID = "ShaderVariantSelector".GetHashCode();
                GUIUtility.ExitGUI();
            }
            if (((Event.current.type == EventType.ExecuteCommand) && (Event.current.commandName == "ObjectSelectorClosed")) && (ObjectSelector.get.objectSelectorID == "ShaderVariantSelector".GetHashCode()))
            {
                Shader currentObject = ObjectSelector.GetCurrentObject() as Shader;
                if (currentObject != null)
                {
                    ShaderUtil.AddNewShaderToCollection(currentObject, this.target as ShaderVariantCollection);
                }
                Event.current.Use();
                GUIUtility.ExitGUI();
            }
            base.serializedObject.ApplyModifiedProperties();
        }

        internal class AddVariantMenuData
        {
            public ShaderVariantCollection collection;
            public string[] keywords;
            public Shader shader;
            public int[] types;
        }

        private class Styles
        {
            public static GUIContent iconAdd = EditorGUIUtility.IconContent("Toolbar Plus", "Add variant");
            public static GUIContent iconRemove = EditorGUIUtility.IconContent("Toolbar Minus", "Remove entry");
            public static GUIStyle invisibleButton = "InvisibleButton";
        }
    }
}

