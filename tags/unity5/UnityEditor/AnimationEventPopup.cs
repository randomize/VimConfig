namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [EditorWindowTitle(title="Edit Animation Event", useTypeNameAsIconName=false)]
    internal class AnimationEventPopup : EditorWindow
    {
        [CompilerGenerated]
        private static Func<System.Reflection.ParameterInfo, System.Type> <>f__am$cache6;
        [CompilerGenerated]
        private static Func<Assembly, bool> <>f__am$cache7;
        private const string kAmbiguousPostFix = " (Function Is Ambiguous)";
        public const string kLogicGraphEventFunction = "LogicGraphEvent";
        private const string kNoneSelected = "(No Function Selected)";
        private const string kNotSupportedPostFix = " (Function Not Supported)";
        private AnimationClip m_Clip;
        private AnimationClipInfoProperties m_ClipInfo;
        private int m_EventIndex;
        private string m_LogicEventName;
        private EditorWindow m_Owner;
        private GameObject m_Root;

        private static void AddLogicGraphEventFunction(List<string> methods, List<System.Type> parameters)
        {
            methods.Insert(0, "LogicGraphEvent");
            parameters.Insert(0, typeof(string));
        }

        internal static void ClosePopup()
        {
            UnityEngine.Object[] objArray = Resources.FindObjectsOfTypeAll(typeof(AnimationEventPopup));
            AnimationEventPopup popup = (objArray.Length <= 0) ? null : ((AnimationEventPopup) objArray[0]);
            if (popup != null)
            {
                popup.Close();
            }
        }

        private static void CollectSupportedMethods(GameObject root, out List<string> supportedMethods, out List<System.Type> supportedMethodsParameter)
        {
            supportedMethods = new List<string>();
            supportedMethodsParameter = new List<System.Type>();
            MonoBehaviour[] components = root.GetComponents<MonoBehaviour>();
            HashSet<string> set = new HashSet<string>();
            foreach (MonoBehaviour behaviour in components)
            {
                if (behaviour != null)
                {
                    for (System.Type type = behaviour.GetType(); (type != typeof(MonoBehaviour)) && (type != null); type = type.BaseType)
                    {
                        foreach (MethodInfo info in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                        {
                            string name = info.Name;
                            if (IsSupportedMethodName(name))
                            {
                                System.Reflection.ParameterInfo[] parameters = info.GetParameters();
                                if (parameters.Length <= 1)
                                {
                                    if (parameters.Length == 1)
                                    {
                                        System.Type parameterType = parameters[0].ParameterType;
                                        if ((((parameterType != typeof(string)) && (parameterType != typeof(float))) && ((parameterType != typeof(int)) && (parameterType != typeof(AnimationEvent)))) && (((parameterType != typeof(UnityEngine.Object)) && !parameterType.IsSubclassOf(typeof(UnityEngine.Object))) && !parameterType.IsEnum))
                                        {
                                            continue;
                                        }
                                        supportedMethodsParameter.Add(parameterType);
                                    }
                                    else
                                    {
                                        supportedMethodsParameter.Add(null);
                                    }
                                    if (supportedMethods.Contains(name))
                                    {
                                        set.Add(name);
                                    }
                                    supportedMethods.Add(name);
                                }
                            }
                        }
                    }
                }
            }
            foreach (string str2 in set)
            {
                for (int i = 0; i < supportedMethods.Count; i++)
                {
                    if (supportedMethods[i].Equals(str2))
                    {
                        supportedMethods.RemoveAt(i);
                        supportedMethodsParameter.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        internal static int Create(GameObject root, AnimationClip clip, float time, EditorWindow owner)
        {
            AnimationEvent evt = new AnimationEvent {
                time = time
            };
            int num = InsertAnimationEvent(ref AnimationUtility.GetAnimationEvents(clip), clip, evt);
            AnimationEventPopup window = EditorWindow.GetWindow<AnimationEventPopup>(true);
            InitWindow(window);
            window.m_Root = root;
            window.m_Clip = clip;
            window.eventIndex = num;
            window.m_Owner = owner;
            return num;
        }

        private void DoEditLogicGraphEventParameters(AnimationEvent evt)
        {
            if (string.IsNullOrEmpty(this.m_LogicEventName))
            {
                this.m_LogicEventName = evt.stringParameter;
            }
            bool flag = EnterPressed();
            this.m_LogicEventName = EditorGUILayout.TextField("Event name", this.m_LogicEventName, new GUILayoutOption[0]);
            if ((this.m_LogicEventName != evt.stringParameter) && (this.m_LogicEventName.Trim() != string.Empty))
            {
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Set", EditorStyles.miniButton, new GUILayoutOption[0]) || flag)
                {
                    RenameAllReferencesToTheLogicGraphAnimationEventInCurrentScene(this.m_Root.GetComponent(typeof(Animation)) as Animation, evt.stringParameter, this.m_LogicEventName);
                    evt.stringParameter = this.m_LogicEventName;
                    this.LogicGraphEventParameterEditingDone(evt);
                    GUI.changed = true;
                }
                if (GUILayout.Button("Cancel", EditorStyles.miniButton, new GUILayoutOption[0]) || EscapePressed())
                {
                    this.LogicGraphEventParameterEditingDone(evt);
                }
                GUILayout.EndHorizontal();
            }
        }

        private static void DoEditRegularParameters(AnimationEvent evt, System.Type selectedParameter)
        {
            if ((selectedParameter == typeof(AnimationEvent)) || (selectedParameter == typeof(float)))
            {
                evt.floatParameter = EditorGUILayout.FloatField("Float", evt.floatParameter, new GUILayoutOption[0]);
            }
            if (selectedParameter.IsEnum)
            {
                evt.intParameter = EnumPopup("Enum", selectedParameter, evt.intParameter);
            }
            else if ((selectedParameter == typeof(AnimationEvent)) || (selectedParameter == typeof(int)))
            {
                evt.intParameter = EditorGUILayout.IntField("Int", evt.intParameter, new GUILayoutOption[0]);
            }
            if ((selectedParameter == typeof(AnimationEvent)) || (selectedParameter == typeof(string)))
            {
                evt.stringParameter = EditorGUILayout.TextField("String", evt.stringParameter, new GUILayoutOption[0]);
            }
            if (((selectedParameter == typeof(AnimationEvent)) || selectedParameter.IsSubclassOf(typeof(UnityEngine.Object))) || (selectedParameter == typeof(UnityEngine.Object)))
            {
                System.Type objType = typeof(UnityEngine.Object);
                if (selectedParameter != typeof(AnimationEvent))
                {
                    objType = selectedParameter;
                }
                bool allowSceneObjects = false;
                evt.objectReferenceParameter = EditorGUILayout.ObjectField(ObjectNames.NicifyVariableName(objType.Name), evt.objectReferenceParameter, objType, allowSceneObjects, new GUILayoutOption[0]);
            }
        }

        internal static void Edit(AnimationClipInfoProperties clipInfo, int index)
        {
            UnityEngine.Object[] objArray = Resources.FindObjectsOfTypeAll(typeof(AnimationEventPopup));
            AnimationEventPopup window = (objArray.Length <= 0) ? null : ((AnimationEventPopup) objArray[0]);
            if (window == null)
            {
                window = EditorWindow.GetWindow<AnimationEventPopup>(true);
                InitWindow(window);
            }
            window.m_Root = null;
            window.m_Clip = null;
            window.m_ClipInfo = clipInfo;
            window.eventIndex = index;
            window.Repaint();
        }

        internal static void Edit(GameObject root, AnimationClip clip, int index, EditorWindow owner)
        {
            UnityEngine.Object[] objArray = Resources.FindObjectsOfTypeAll(typeof(AnimationEventPopup));
            AnimationEventPopup window = (objArray.Length <= 0) ? null : ((AnimationEventPopup) objArray[0]);
            if (window == null)
            {
                window = EditorWindow.GetWindow<AnimationEventPopup>(true);
                InitWindow(window);
            }
            window.m_Root = root;
            window.m_Clip = clip;
            window.eventIndex = index;
            window.m_Owner = owner;
            window.Repaint();
        }

        private static bool EnterPressed()
        {
            return ((Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.Return));
        }

        public static int EnumPopup(string label, System.Type enumType, int selected)
        {
            if (!enumType.IsEnum)
            {
                throw new Exception("parameter _enum must be of type System.Enum");
            }
            string[] names = Enum.GetNames(enumType);
            int index = Array.IndexOf<string>(names, Enum.GetName(enumType, selected));
            index = EditorGUILayout.Popup(label, index, names, EditorStyles.popup, new GUILayoutOption[0]);
            if (index == -1)
            {
                return selected;
            }
            Enum enum2 = (Enum) Enum.Parse(enumType, names[index]);
            return Convert.ToInt32(enum2);
        }

        private static bool EscapePressed()
        {
            return ((Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.Escape));
        }

        public static string FormatEvent(GameObject root, AnimationEvent evt)
        {
            if (string.IsNullOrEmpty(evt.functionName))
            {
                return "(No Function Selected)";
            }
            if (IsLogicGraphEvent(evt))
            {
                return FormatLogicGraphEvent(evt);
            }
            if (IsSupportedMethodName(evt.functionName))
            {
                foreach (MonoBehaviour behaviour in root.GetComponents<MonoBehaviour>())
                {
                    if (behaviour != null)
                    {
                        System.Type type = behaviour.GetType();
                        if ((type != typeof(MonoBehaviour)) && ((type.BaseType == null) || (type.BaseType.Name != "GraphBehaviour")))
                        {
                            MethodInfo method = type.GetMethod(evt.functionName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                            if (method != null)
                            {
                                if (<>f__am$cache6 == null)
                                {
                                    <>f__am$cache6 = p => p.ParameterType;
                                }
                                IEnumerable<System.Type> paramTypes = method.GetParameters().Select<System.Reflection.ParameterInfo, System.Type>(<>f__am$cache6);
                                return (evt.functionName + FormatEventArguments(paramTypes, evt));
                            }
                        }
                    }
                }
            }
            return (evt.functionName + " (Function Not Supported)");
        }

        private static string FormatEventArguments(IEnumerable<System.Type> paramTypes, AnimationEvent evt)
        {
            if (!paramTypes.Any<System.Type>())
            {
                return " ( )";
            }
            if (paramTypes.Count<System.Type>() <= 1)
            {
                System.Type enumType = paramTypes.First<System.Type>();
                if (enumType == typeof(string))
                {
                    return (" ( \"" + evt.stringParameter + "\" )");
                }
                if (enumType == typeof(float))
                {
                    return (" ( " + evt.floatParameter + " )");
                }
                if (enumType == typeof(int))
                {
                    return (" ( " + evt.intParameter + " )");
                }
                if (enumType == typeof(int))
                {
                    return (" ( " + evt.intParameter + " )");
                }
                if (enumType.IsEnum)
                {
                    string[] textArray1 = new string[] { " ( ", enumType.Name, ".", Enum.GetName(enumType, evt.intParameter), " )" };
                    return string.Concat(textArray1);
                }
                if (enumType == typeof(AnimationEvent))
                {
                    object[] objArray1 = new object[] { " ( ", evt.floatParameter, " / ", evt.intParameter, " / \"", evt.stringParameter, "\" / ", (evt.objectReferenceParameter != null) ? evt.objectReferenceParameter.name : "null", " )" };
                    return string.Concat(objArray1);
                }
                if (enumType.IsSubclassOf(typeof(UnityEngine.Object)) || (enumType == typeof(UnityEngine.Object)))
                {
                    return (" ( " + ((evt.objectReferenceParameter != null) ? evt.objectReferenceParameter.name : "null") + " )");
                }
            }
            return " (Function Not Supported)";
        }

        private static string FormatLogicGraphEvent(AnimationEvent evt)
        {
            System.Type[] paramTypes = new System.Type[] { typeof(string) };
            return ("LogicGraphEvent" + FormatEventArguments(paramTypes, evt));
        }

        private static string GetEventNameForLogicGraphEvent(IEnumerable<AnimationEvent> events, AnimationEvent animEvent)
        {
            for (int i = 1; i < 0x3e8; i++)
            {
                <GetEventNameForLogicGraphEvent>c__AnonStorey39 storey = new <GetEventNameForLogicGraphEvent>c__AnonStorey39 {
                    name = "LogicGraphEvent" + i
                };
                if (!events.Any<AnimationEvent>(new Func<AnimationEvent, bool>(storey.<>m__58)))
                {
                    string str2 = "LogicGraphEvent" + i;
                    animEvent.stringParameter = str2;
                    return str2;
                }
            }
            return string.Empty;
        }

        internal static void InitWindow(AnimationEventPopup popup)
        {
            popup.minSize = new Vector2(400f, 140f);
            popup.maxSize = new Vector2(400f, 140f);
            popup.titleContent = EditorGUIUtility.TextContent("Edit Animation Event");
        }

        internal static int InsertAnimationEvent(ref AnimationEvent[] events, AnimationClip clip, AnimationEvent evt)
        {
            Undo.RegisterCompleteObjectUndo(clip, "Add Event");
            int length = events.Length;
            for (int i = 0; i < events.Length; i++)
            {
                if (events[i].time > evt.time)
                {
                    length = i;
                    break;
                }
            }
            ArrayUtility.Insert<AnimationEvent>(ref events, length, evt);
            AnimationUtility.SetAnimationEvents(clip, events);
            events = AnimationUtility.GetAnimationEvents(clip);
            if ((events[length].time != evt.time) || (events[length].functionName != events[length].functionName))
            {
                Debug.LogError("Failed insertion");
            }
            return length;
        }

        private static bool IsLogicGraphEvent(AnimationEvent evt)
        {
            return (evt.functionName == "LogicGraphEvent");
        }

        private static bool IsSupportedMethodName(string name)
        {
            return ((!(name == "Main") && !(name == "Start")) && (!(name == "Awake") && !(name == "Update")));
        }

        private void LogicGraphEventParameterEditingDone(AnimationEvent evt)
        {
            GUIUtility.keyboardControl = 0;
            this.m_LogicEventName = string.Empty;
            Event.current.Use();
        }

        private void OnDestroy()
        {
            if (this.m_Owner != null)
            {
                this.m_Owner.Focus();
            }
        }

        private void OnEnable()
        {
            base.titleContent = base.GetLocalizedTitleContent();
        }

        public void OnGUI()
        {
            AnimationEvent[] events = null;
            if (this.m_Clip != null)
            {
                events = AnimationUtility.GetAnimationEvents(this.m_Clip);
            }
            else if (this.m_ClipInfo != null)
            {
                events = this.m_ClipInfo.GetEvents();
            }
            if (((events != null) && (this.eventIndex >= 0)) && (this.eventIndex < events.Length))
            {
                GUI.changed = false;
                AnimationEvent evt = events[this.eventIndex];
                if (this.m_Root != null)
                {
                    List<string> list;
                    List<System.Type> list2;
                    CollectSupportedMethods(this.m_Root, out list, out list2);
                    List<string> list3 = new List<string>(list.Count);
                    for (int i = 0; i < list.Count; i++)
                    {
                        string str = " ( )";
                        if (list2[i] != null)
                        {
                            if (list2[i] == typeof(float))
                            {
                                str = " ( float )";
                            }
                            else if (list2[i] == typeof(int))
                            {
                                str = " ( int )";
                            }
                            else
                            {
                                str = string.Format(" ( {0} )", list2[i].Name);
                            }
                        }
                        list3.Add(list[i] + str);
                    }
                    int count = list.Count;
                    int index = list.IndexOf(evt.functionName);
                    if (index == -1)
                    {
                        index = list.Count;
                        list.Add(evt.functionName);
                        if (string.IsNullOrEmpty(evt.functionName))
                        {
                            list3.Add("(No Function Selected)");
                        }
                        else
                        {
                            list3.Add(evt.functionName + " (Function Not Supported)");
                        }
                        list2.Add(null);
                    }
                    EditorGUIUtility.labelWidth = 130f;
                    int num4 = index;
                    index = EditorGUILayout.Popup("Function: ", index, list3.ToArray(), new GUILayoutOption[0]);
                    if (((num4 != index) && (index != -1)) && (index != count))
                    {
                        evt.functionName = list[index];
                        evt.stringParameter = !IsLogicGraphEvent(evt) ? string.Empty : GetEventNameForLogicGraphEvent(events, evt);
                    }
                    System.Type selectedParameter = list2[index];
                    if (selectedParameter != null)
                    {
                        EditorGUILayout.Space();
                        if (selectedParameter == typeof(AnimationEvent))
                        {
                            EditorGUILayout.PrefixLabel("Event Data");
                        }
                        else
                        {
                            EditorGUILayout.PrefixLabel("Parameters");
                        }
                        if (IsLogicGraphEvent(evt))
                        {
                            this.DoEditLogicGraphEventParameters(evt);
                        }
                        else
                        {
                            DoEditRegularParameters(evt, selectedParameter);
                        }
                    }
                }
                else
                {
                    evt.functionName = EditorGUILayout.TextField(new GUIContent("Function"), evt.functionName, new GUILayoutOption[0]);
                    DoEditRegularParameters(evt, typeof(AnimationEvent));
                }
                if (GUI.changed)
                {
                    if (this.m_Clip != null)
                    {
                        Undo.RegisterCompleteObjectUndo(this.m_Clip, "Animation Event Change");
                        AnimationUtility.SetAnimationEvents(this.m_Clip, events);
                    }
                    else if (this.m_ClipInfo != null)
                    {
                        this.m_ClipInfo.SetEvent(this.m_EventIndex, evt);
                    }
                }
            }
        }

        private static void RenameAllReferencesToTheLogicGraphAnimationEventInCurrentScene(Animation animation, string oldName, string newName)
        {
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = asm => asm.GetName().Name == "UnityEditor.Graphs.LogicGraph";
            }
            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().Where<Assembly>(<>f__am$cache7).FirstOrDefault<Assembly>();
            if (assembly == null)
            {
                throw new Exception("Could not find the logic graph assembly in loaded assemblies.");
            }
            System.Type type = assembly.GetType("UnityEngine.Graphs.LogicGraph.OnAnimationEventNode");
            if (type == null)
            {
                throw new Exception("Failed to find type 'OnAnimationEventNode'.");
            }
            MethodInfo method = type.GetMethod("AnimationEventNameChanged");
            if (method == null)
            {
                throw new Exception("Could not find 'AnimationEventNameChanged' method.");
            }
            object[] parameters = new object[] { animation, oldName, newName };
            method.Invoke(null, parameters);
        }

        internal static void UpdateSelection(GameObject root, AnimationClip clip, int index, EditorWindow owner)
        {
            UnityEngine.Object[] objArray = Resources.FindObjectsOfTypeAll(typeof(AnimationEventPopup));
            AnimationEventPopup popup = (objArray.Length <= 0) ? null : ((AnimationEventPopup) objArray[0]);
            if (popup != null)
            {
                popup.m_Root = root;
                popup.m_Clip = clip;
                popup.eventIndex = index;
                popup.m_Owner = owner;
                popup.Repaint();
            }
        }

        public AnimationClipInfoProperties clipInfo
        {
            get
            {
                return this.m_ClipInfo;
            }
            set
            {
                this.m_ClipInfo = value;
            }
        }

        private int eventIndex
        {
            get
            {
                return this.m_EventIndex;
            }
            set
            {
                if (this.m_EventIndex != value)
                {
                    this.m_LogicEventName = string.Empty;
                }
                this.m_EventIndex = value;
            }
        }

        [CompilerGenerated]
        private sealed class <GetEventNameForLogicGraphEvent>c__AnonStorey39
        {
            internal string name;

            internal bool <>m__58(AnimationEvent evt)
            {
                return (AnimationEventPopup.IsLogicGraphEvent(evt) && (evt.stringParameter == this.name));
            }
        }
    }
}

