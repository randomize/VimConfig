// WARNING:Skipped nested type: UnityEngine.InternalStaticBatchingUtility+SortGO
// WARNING:Skipped nested type: UnityEngine.Animation+Enumerator
// WARNING:Skipped nested type: UnityEngine.GUI+ScrollViewState
// WARNING:Skipped nested type: UnityEngine.GUILayout+LayoutedWindow
// WARNING:Skipped nested type: UnityEngine.GUILayoutUtility+LayoutCache
// WARNING:Skipped nested type: UnityEngine.Terrain+Renderer
// WARNING:Skipped nested type: UnityEngine.Transform+Enumerator

// INFO:MMCSReflector::ImportedAssembly: UnityEngine, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null

namespace UnityEngine {
	public class	AndroidJavaObject: AndroidJNIHelper, IDisposable
	{
		protected void DebugPrint(string msg){}
		protected void DebugPrint(string call, string methodName, string signature, Object[] args){}
		private void _AndroidJavaObject(string className, Object[] args){}
		protected virtual void Finalize(){}
		protected virtual void Dispose(bool disposing){}
		protected void _Dispose(){}
		protected void _Call(string methodName, Object[] args){}
		protected ReturnType _Call(string methodName, Object[] args){}
		protected FieldType _Get(string fieldName){}
		protected void _Set(string fieldName, FieldType val){}
		protected void _CallStatic(string methodName, Object[] args){}
		protected ReturnType _CallStatic(string methodName, Object[] args){}
		protected FieldType _GetStatic(string fieldName){}
		protected void _SetStatic(string fieldName, FieldType val){}
		protected IntPtr _GetRawObject(){}
		protected IntPtr _GetRawClass(){}
		protected IntPtr GetCachedMethodID(string methodName, Object[] args, bool isStatic){}
		protected IntPtr GetCachedMethodID(string methodName, Object[] args, bool isStatic){}
		protected IntPtr GetCachedFieldID(string fieldName, bool isStatic){}
		protected static int GetSignatureHash(Object[] args){}
		protected static int GetSignatureHash(Object[] args){}
		public sealed virtual void Dispose(){}
		public void Call(string methodName, Object[] args){}
		public void CallStatic(string methodName, Object[] args){}
		public FieldType Get(string fieldName){}
		public void Set(string fieldName, FieldType val){}
		public FieldType GetStatic(string fieldName){}
		public void SetStatic(string fieldName, FieldType val){}
		public IntPtr GetRawObject(){}
		public IntPtr GetRawClass(){}
		public ReturnType Call(string methodName, Object[] args){}
		public ReturnType CallStatic(string methodName, Object[] args){}
		public AndroidJavaObject(IntPtr jobject){}
		protected AndroidJavaObject(){}
		public AndroidJavaObject(string className, Object[] args){}
		private static AndroidJavaObject(){}
		private bool m_disposed;
		protected IntPtr m_jobject;
		protected IntPtr m_jclass;
		protected Dictionary<Int32, IntPtr> m_methodIDs;
		protected Dictionary<Int32, IntPtr> m_fieldIDs;
		private static bool enableDebugPrints;
	}

	public class	AndroidJavaClass: AndroidJavaObject, IDisposable
	{
		private void _AndroidJavaClass(string className){}
		public AndroidJavaClass(IntPtr jclass){}
		public AndroidJavaClass(string className){}
	}

	public class	ThreadSafeAttribute: Attribute, _Attribute
	{
		public ThreadSafeAttribute(){}
	}

	public class	ConstructorSafeAttribute: Attribute, _Attribute
	{
		public ConstructorSafeAttribute(){}
	}

	public class	ImplementedInActionScriptAttribute: Attribute, _Attribute
	{
		public ImplementedInActionScriptAttribute(){}
	}

	public sealed abstract	class	Social: Object
	{
		public static void LoadUsers(String[] userIDs, Action<IUserProfile[]> callback){}
		public static void ReportProgress(string achievementID, double progress, Action<Boolean> callback){}
		public static void LoadAchievementDescriptions(Action<IAchievementDescription[]> callback){}
		public static void LoadAchievements(Action<IAchievement[]> callback){}
		public static void ReportScore(long score, string board, Action<Boolean> callback){}
		public static void LoadScores(string leaderboardID, Action<IScore[]> callback){}
		public static ILeaderboard CreateLeaderboard(){}
		public static IAchievement CreateAchievement(){}
		public static void ShowAchievementsUI(){}
		public static void ShowLeaderboardUI(){}
		public static ISocialPlatform Active{ get	{} set	{} }
		public static ILocalUser localUser{ get	{} }
	}

	public sealed class	Security: Object
	{
		private static MethodInfo GetUnityCrossDomainHelperMethod(string methodname){}
		public static bool PrefetchSocketPolicy(string ip, int atPort){}
		public static bool PrefetchSocketPolicy(string ip, int atPort, int timeout){}
		public Security(){}
	}

	public sealed abstract	class	Types: Object
	{
		public static Type GetType(string typeName, string assemblyName){}
	}

	public class	StackTraceUtility: Object
	{
		internal static void SetProjectFolder(string folder){}
		public static string ExtractStackTrace(){}
		private static bool IsSystemStacktraceType(System.Object name){}
		public static string ExtractStringFromException(System.Object exception){}
		internal static void ExtractStringFromExceptionInternal(System.Object exceptiono, out string message, out string stackTrace){}
		internal static string PostprocessStacktrace(string oldString, bool stripEngineInternalInformation){}
		internal static string ExtractFormattedStackTrace(StackTrace stackTrace){}
		public StackTraceUtility(){}
		private static StackTraceUtility(){}
		private static string projectFolder;
	}

	public class	UnityException: SystemException, ISerializable, _Exception
	{
		public UnityException(){}
		public UnityException(string message){}
		public UnityException(string message, Exception innerException){}
		protected UnityException(SerializationInfo info, StreamingContext context){}
		private string unityStackTrace;
		private const int Result = null;
	}

	public class	MissingComponentException: SystemException, ISerializable, _Exception
	{
		public MissingComponentException(){}
		public MissingComponentException(string message){}
		public MissingComponentException(string message, Exception innerException){}
		protected MissingComponentException(SerializationInfo info, StreamingContext context){}
		private string unityStackTrace;
		private const int Result = null;
	}

	public class	UnassignedReferenceException: SystemException, ISerializable, _Exception
	{
		public UnassignedReferenceException(){}
		public UnassignedReferenceException(string message){}
		public UnassignedReferenceException(string message, Exception innerException){}
		protected UnassignedReferenceException(SerializationInfo info, StreamingContext context){}
		private string unityStackTrace;
		private const int Result = null;
	}

	public class	MissingReferenceException: SystemException, ISerializable, _Exception
	{
		public MissingReferenceException(){}
		public MissingReferenceException(string message){}
		public MissingReferenceException(string message, Exception innerException){}
		protected MissingReferenceException(SerializationInfo info, StreamingContext context){}
		private string unityStackTrace;
		private const int Result = null;
	}

	public class	TextEditor: Object
	{
		private void ClearCursorPos(){}
		public void OnFocus(){}
		public void OnLostFocus(){}
		private void GrabGraphicalCursorPos(){}
		public bool HandleKeyEvent(Event e){}
		public bool DeleteLineBack(){}
		public bool DeleteWordBack(){}
		public bool DeleteWordForward(){}
		public bool Delete(){}
		public bool CanPaste(){}
		public bool Backspace(){}
		public void SelectAll(){}
		public void SelectNone(){}
		public bool DeleteSelection(){}
		public void ReplaceSelection(string replace){}
		public void Insert(char c){}
		public void MoveSelectionToAltCursor(){}
		public void MoveRight(){}
		public void MoveLeft(){}
		public void MoveUp(){}
		public void MoveDown(){}
		public void MoveLineStart(){}
		public void MoveLineEnd(){}
		public void MoveGraphicalLineStart(){}
		public void MoveGraphicalLineEnd(){}
		public void MoveTextStart(){}
		public void MoveTextEnd(){}
		public void MoveParagraphForward(){}
		public void MoveParagraphBackward(){}
		public void MoveCursorToPosition(Vector2 cursorPosition){}
		public void MoveAltCursorToPosition(Vector2 cursorPosition){}
		public bool IsOverSelection(Vector2 cursorPosition){}
		public void SelectToPosition(Vector2 cursorPosition){}
		public void SelectLeft(){}
		public void SelectRight(){}
		public void SelectUp(){}
		public void SelectDown(){}
		public void SelectTextEnd(){}
		public void SelectTextStart(){}
		public void MouseDragSelectsWholeWords(bool on){}
		public void DblClickSnap(DblClickSnapping snapping){}
		private int GetGraphicalLineStart(int p){}
		private int GetGraphicalLineEnd(int p){}
		private int FindNextSeperator(int startPos){}
		private static bool isLetterLikeChar(char c){}
		private int FindPrevSeperator(int startPos){}
		public void MoveWordRight(){}
		public void MoveToStartOfNextWord(){}
		public void MoveToEndOfPreviousWord(){}
		public void SelectToStartOfNextWord(){}
		public void SelectToEndOfPreviousWord(){}
		private CharacterType ClassifyChar(char c){}
		public int FindStartOfNextWord(int p){}
		private int FindEndOfPreviousWord(int p){}
		public void MoveWordLeft(){}
		public void SelectWordRight(){}
		public void SelectWordLeft(){}
		public void ExpandSelectGraphicalLineStart(){}
		public void ExpandSelectGraphicalLineEnd(){}
		public void SelectGraphicalLineStart(){}
		public void SelectGraphicalLineEnd(){}
		public void SelectParagraphForward(){}
		public void SelectParagraphBackward(){}
		public void SelectCurrentWord(){}
		private int FindEndOfClassification(int p, int dir){}
		public void SelectCurrentParagraph(){}
		public void DrawCursor(string text){}
		private bool PerformOperation(TextEditOp operation){}
		public void SaveBackup(){}
		public void Undo(){}
		public bool Cut(){}
		public void Copy(){}
		public bool Paste(){}
		private static void MapKey(string key, TextEditOp action){}
		private void InitKeyActions(){}
		public void ClampPos(){}
		public TextEditor(){}
		public bool hasSelection{ get	{} }
		public string SelectedText{ get	{} }
		public int pos;
		public int selectPos;
		public int controlID;
		public GUIContent content;
		public GUIStyle style;
		public Rect position;
		public bool multiline;
		public bool hasHorizontalCursorPos;
		public bool isPasswordField;
		internal bool m_HasFocus;
		public Vector2 scrollOffset;
		public Vector2 graphicalCursorPos;
		public Vector2 graphicalSelectCursorPos;
		private bool m_MouseDragSelectsWholeWords;
		private int m_DblClickInitPos;
		private DblClickSnapping m_DblClickSnap;
		private bool m_bJustSelected;
		private int m_iAltCursorPos;
		private string oldText;
		private int oldPos;
		private int oldSelectPos;
		private static Hashtable s_Keyactions;
	}

	public class	TrackedReference: Object
	{
		public virtual bool Equals(System.Object o){}
		public virtual int GetHashCode(){}
		protected TrackedReference(){}
		private IntPtr m_Ptr;
	}

	public sealed class	WWW: Object, IDisposable
	{
		private static String[] FlattenedHeadersFrom(Hashtable headers){}
		internal static Dictionary<String, String> ParseHTTPHeaderString(string input){}
		public sealed virtual void Dispose(){}
		protected virtual void Finalize(){}
		private void DestroyWWW(bool cancel){}
		public void InitWWW(string url, Byte[] postData, String[] iHeaders){}
		public static string EscapeURL(string s){}
		public static string EscapeURL(string s, Encoding e){}
		public static string UnEscapeURL(string s){}
		public static string UnEscapeURL(string s, Encoding e){}
		private Encoding GetTextEncoder(){}
		private Texture2D GetTexture(bool markNonReadable){}
		public AudioClip GetAudioClip(bool threeD){}
		public AudioClip GetAudioClip(bool threeD, bool stream){}
		public AudioClip GetAudioClip(bool threeD, bool stream, AudioType audioType){}
		public void LoadImageIntoTexture(Texture2D tex){}
		public static string GetURL(string url){}
		public static Texture2D GetTextureFromURL(string url){}
		public void LoadUnityWeb(){}
		public static WWW LoadFromCacheOrDownload(string url, int version){}
		public static WWW LoadFromCacheOrDownload(string url, int version, uint crc){}
		public WWW(string url){}
		public WWW(string url, WWWForm form){}
		public WWW(string url, Byte[] postData){}
		public WWW(string url, Byte[] postData, Hashtable headers){}
		internal WWW(string url, int version, uint crc){}
		public Dictionary<String, String> responseHeaders{ get	{} }
		string responseHeadersString{ get	{} }
		public string text{ get	{} }
		public string data{ get	{} }
		public Byte[] bytes{ get	{} }
		public int size{ get	{} }
		public string error{ get	{} }
		public Texture2D texture{ get	{} }
		public Texture2D textureNonReadable{ get	{} }
		public AudioClip audioClip{ get	{} }
		public MovieTexture movie{ get	{} }
		public bool isDone{ get	{} }
		public float progress{ get	{} }
		public float uploadProgress{ get	{} }
		public AudioClip oggVorbis{ get	{} }
		public string url{ get	{} }
		public AssetBundle assetBundle{ get	{} }
		public ThreadPriority threadPriority{ get	{} set	{} }
		private IntPtr m_Ptr;
	}

	public sealed class	AnimationEvent: Object
	{
		private void Create(){}
		protected virtual void Finalize(){}
		private void Destroy(){}
		public AnimationEvent(){}
		public string data{ get	{} set	{} }
		public string stringParameter{ get	{} set	{} }
		public float floatParameter{ get	{} set	{} }
		public int intParameter{ get	{} set	{} }
		public System.Object objectReferenceParameter{ get	{} set	{} }
		public string functionName{ get	{} set	{} }
		public float time{ get	{} set	{} }
		public SendMessageOptions messageOptions{ get	{} set	{} }
		public AnimationState animationState{ get	{} }
		private IntPtr m_Ptr;
		private int m_OwnsData;
	}

	public sealed class	AnimationClip: Motion
	{
		private static void Internal_CreateAnimationClip(AnimationClip self){}
		public void SetCurve(string relativePath, Type type, string propertyName, AnimationCurve curve){}
		public void EnsureQuaternionContinuity(){}
		private static void INTERNAL_CALL_EnsureQuaternionContinuity(AnimationClip self){}
		public void ClearCurves(){}
		private static void INTERNAL_CALL_ClearCurves(AnimationClip self){}
		public void AddEvent(AnimationEvent evt){}
		private void INTERNAL_get_localBounds(out Bounds value){}
		private void INTERNAL_set_localBounds(ref Bounds value){}
		public AnimationClip(){}
		public float length{ get	{} }
		float startTime{ get	{} }
		float stopTime{ get	{} }
		public float frameRate{ get	{} }
		public WrapMode wrapMode{ get	{} set	{} }
		public Bounds localBounds{ get	{} set	{} }
	}

	public sealed class	AnimationCurve: Object
	{
		private void Cleanup(){}
		protected virtual void Finalize(){}
		public float Evaluate(float time){}
		public int AddKey(float time, float value){}
		public int AddKey(Keyframe key){}
		private int AddKey_Internal(Keyframe key){}
		private static int INTERNAL_CALL_AddKey_Internal(AnimationCurve self, ref Keyframe key){}
		public int MoveKey(int index, Keyframe key){}
		private static int INTERNAL_CALL_MoveKey(AnimationCurve self, int index, ref Keyframe key){}
		public void RemoveKey(int index){}
		private void SetKeys(Keyframe[] keys){}
		private Keyframe GetKey_Internal(int index){}
		private Keyframe[] GetKeys(){}
		public void SmoothTangents(int index, float weight){}
		public static AnimationCurve Linear(float timeStart, float valueStart, float timeEnd, float valueEnd){}
		public static AnimationCurve EaseInOut(float timeStart, float valueStart, float timeEnd, float valueEnd){}
		private void Init(Keyframe[] keys){}
		public AnimationCurve(Keyframe[] keys){}
		public AnimationCurve(){}
		public Keyframe[] keys{ get	{} set	{} }
		public Keyframe this[int index] { get	{} }
		public int length{ get	{} }
		public WrapMode preWrapMode{ get	{} set	{} }
		public WrapMode postWrapMode{ get	{} set	{} }
		internal IntPtr m_Ptr;
	}

	public sealed class	Animation: Behaviour, IEnumerable
	{
		public void Stop(){}
		private static void INTERNAL_CALL_Stop(Animation self){}
		public void Stop(string name){}
		private void Internal_StopByName(string name){}
		public void Rewind(string name){}
		private void Internal_RewindByName(string name){}
		public void Rewind(){}
		private static void INTERNAL_CALL_Rewind(Animation self){}
		public void Sample(){}
		private static void INTERNAL_CALL_Sample(Animation self){}
		public bool IsPlaying(string name){}
		public bool Play(){}
		public bool Play(PlayMode mode){}
		public bool Play(string animation, PlayMode mode){}
		public bool Play(string animation){}
		public void CrossFade(string animation, float fadeLength, PlayMode mode){}
		public void CrossFade(string animation, float fadeLength){}
		public void CrossFade(string animation){}
		public void Blend(string animation, float targetWeight, float fadeLength){}
		public void Blend(string animation, float targetWeight){}
		public void Blend(string animation){}
		public AnimationState CrossFadeQueued(string animation, float fadeLength, QueueMode queue, PlayMode mode){}
		public AnimationState CrossFadeQueued(string animation, float fadeLength, QueueMode queue){}
		public AnimationState CrossFadeQueued(string animation, float fadeLength){}
		public AnimationState CrossFadeQueued(string animation){}
		public AnimationState PlayQueued(string animation, QueueMode queue, PlayMode mode){}
		public AnimationState PlayQueued(string animation, QueueMode queue){}
		public AnimationState PlayQueued(string animation){}
		public void AddClip(AnimationClip clip, string newName){}
		public void AddClip(AnimationClip clip, string newName, int firstFrame, int lastFrame, bool addLoopFrame){}
		public void AddClip(AnimationClip clip, string newName, int firstFrame, int lastFrame){}
		public void RemoveClip(AnimationClip clip){}
		public void RemoveClip(string clipName){}
		public int GetClipCount(){}
		private void RemoveClip2(string clipName){}
		private bool PlayDefaultAnimation(PlayMode mode){}
		public bool Play(AnimationPlayMode mode){}
		public bool Play(string animation, AnimationPlayMode mode){}
		public void SyncLayer(int layer){}
		private static void INTERNAL_CALL_SyncLayer(Animation self, int layer){}
		public sealed virtual IEnumerator GetEnumerator(){}
		internal AnimationState GetState(string name){}
		internal AnimationState GetStateAtIndex(int index){}
		internal int GetStateCount(){}
		public AnimationClip GetClip(string name){}
		private void INTERNAL_get_localBounds(out Bounds value){}
		private void INTERNAL_set_localBounds(ref Bounds value){}
		public Animation(){}
		public AnimationClip clip{ get	{} set	{} }
		public bool playAutomatically{ get	{} set	{} }
		public WrapMode wrapMode{ get	{} set	{} }
		public bool isPlaying{ get	{} }
		public AnimationState this[string name] { get	{} }
		public bool animatePhysics{ get	{} set	{} }
		public bool animateOnlyIfVisible{ get	{} set	{} }
		public AnimationCullingType cullingType{ get	{} set	{} }
		public Bounds localBounds{ get	{} set	{} }
	}

	public sealed class	AnimationState: TrackedReference
	{
		public void AddMixingTransform(Transform mix, bool recursive){}
		public void AddMixingTransform(Transform mix){}
		public void RemoveMixingTransform(Transform mix){}
		public AnimationState(){}
		public bool enabled{ get	{} set	{} }
		public float weight{ get	{} set	{} }
		public WrapMode wrapMode{ get	{} set	{} }
		public float time{ get	{} set	{} }
		public float normalizedTime{ get	{} set	{} }
		public float speed{ get	{} set	{} }
		public float normalizedSpeed{ get	{} set	{} }
		public float length{ get	{} }
		public int layer{ get	{} set	{} }
		public AnimationClip clip{ get	{} }
		public string name{ get	{} set	{} }
		public AnimationBlendMode blendMode{ get	{} set	{} }
	}

	public sealed class	Animator: Behaviour
	{
		public AnimatorStateInfo GetNextAnimatorStateInfo(int layerIndex){}
		public AnimatorTransitionInfo GetAnimatorTransitionInfo(int layerIndex){}
		public AnimationInfo[] GetCurrentAnimationClipState(int layerIndex){}
		public AnimationInfo[] GetNextAnimationClipState(int layerIndex){}
		public bool IsInTransition(int layerIndex){}
		public void MatchTarget(Vector3 matchPosition, Quaternion matchRotation, AvatarTarget targetBodyPart, MatchTargetWeightMask weightMask, float startNormalizedTime, float targetNormalizedTime){}
		public void MatchTarget(Vector3 matchPosition, Quaternion matchRotation, AvatarTarget targetBodyPart, MatchTargetWeightMask weightMask, float startNormalizedTime){}
		private static void INTERNAL_CALL_MatchTarget(Animator self, ref Vector3 matchPosition, ref Quaternion matchRotation, AvatarTarget targetBodyPart, ref MatchTargetWeightMask weightMask, float startNormalizedTime, float targetNormalizedTime){}
		public void InterruptMatchTarget(bool completeMatch){}
		public void InterruptMatchTarget(){}
		public void ForceStateNormalizedTime(float normalizedTime){}
		public void SetTarget(AvatarTarget targetIndex, float targetNormalizedTime){}
		public bool IsControlled(Transform transform){}
		public Transform GetBoneTransform(HumanBodyBones humanBoneId){}
		public static int StringToHash(string name){}
		private void SetFloatString(string name, float value){}
		private void SetFloatID(int id, float value){}
		private float GetFloatString(string name){}
		private float GetFloatID(int id){}
		private void SetBoolString(string name, bool value){}
		private void SetBoolID(int id, bool value){}
		private bool GetBoolString(string name){}
		private bool GetBoolID(int id){}
		private void SetIntegerString(string name, int value){}
		private void SetIntegerID(int id, int value){}
		private int GetIntegerString(string name){}
		private int GetIntegerID(int id){}
		private void SetVectorString(string name, Vector3 value){}
		private static void INTERNAL_CALL_SetVectorString(Animator self, string name, ref Vector3 value){}
		private void SetVectorID(int id, Vector3 value){}
		private static void INTERNAL_CALL_SetVectorID(Animator self, int id, ref Vector3 value){}
		private Vector3 GetVectorString(string name){}
		private Vector3 GetVectorID(int id){}
		private void SetQuaternionString(string name, Quaternion value){}
		private static void INTERNAL_CALL_SetQuaternionString(Animator self, string name, ref Quaternion value){}
		private void SetQuaternionID(int id, Quaternion value){}
		private static void INTERNAL_CALL_SetQuaternionID(Animator self, int id, ref Quaternion value){}
		private Quaternion GetQuaternionString(string name){}
		private Quaternion GetQuaternionID(int id){}
		private bool IsParameterControlledByCurveString(string name){}
		private bool IsParameterControlledByCurveID(int id){}
		private void SetFloatStringDamp(string name, float value, float dampTime, float deltaTime){}
		private void SetFloatIDDamp(int id, float value, float dampTime, float deltaTime){}
		internal void WriteDefaultPose(){}
		internal void Update(float deltaTime){}
		internal void PlaybackRecordedFrame(float time){}
		internal void StartRecording(){}
		internal void StopRecording(){}
		public float GetFloat(string name){}
		public float GetFloat(int id){}
		public void SetFloat(string name, float value){}
		public void SetFloat(string name, float value, float dampTime, float deltaTime){}
		public void SetFloat(int id, float value){}
		public void SetFloat(int id, float value, float dampTime, float deltaTime){}
		public bool GetBool(string name){}
		public bool GetBool(int id){}
		public void SetBool(string name, bool value){}
		public void SetBool(int id, bool value){}
		public int GetInteger(string name){}
		public int GetInteger(int id){}
		public void SetInteger(string name, int value){}
		public void SetInteger(int id, int value){}
		public Vector3 GetVector(string name){}
		public Vector3 GetVector(int id){}
		public void SetVector(string name, Vector3 value){}
		public void SetVector(int id, Vector3 value){}
		public Quaternion GetQuaternion(string name){}
		public Quaternion GetQuaternion(int id){}
		public void SetQuaternion(string name, Quaternion value){}
		public void SetQuaternion(int id, Quaternion value){}
		public bool IsParameterControlledByCurve(string name){}
		public bool IsParameterControlledByCurve(int id){}
		private void INTERNAL_get_rootPosition(out Vector3 value){}
		private void INTERNAL_set_rootPosition(ref Vector3 value){}
		private void INTERNAL_get_rootRotation(out Quaternion value){}
		private void INTERNAL_set_rootRotation(ref Quaternion value){}
		private void INTERNAL_get_bodyPosition(out Vector3 value){}
		private void INTERNAL_set_bodyPosition(ref Vector3 value){}
		private void INTERNAL_get_bodyRotation(out Quaternion value){}
		private void INTERNAL_set_bodyRotation(ref Quaternion value){}
		public Vector3 GetIKPosition(AvatarIKGoal goal){}
		public void SetIKPosition(AvatarIKGoal goal, Vector3 goalPosition){}
		private static void INTERNAL_CALL_SetIKPosition(Animator self, AvatarIKGoal goal, ref Vector3 goalPosition){}
		public Quaternion GetIKRotation(AvatarIKGoal goal){}
		public void SetIKRotation(AvatarIKGoal goal, Quaternion goalRotation){}
		private static void INTERNAL_CALL_SetIKRotation(Animator self, AvatarIKGoal goal, ref Quaternion goalRotation){}
		public float GetIKPositionWeight(AvatarIKGoal goal){}
		public void SetIKPositionWeight(AvatarIKGoal goal, float value){}
		public float GetIKRotationWeight(AvatarIKGoal goal){}
		public void SetIKRotationWeight(AvatarIKGoal goal, float value){}
		public void SetLookAtPosition(Vector3 lookAtPosition){}
		private static void INTERNAL_CALL_SetLookAtPosition(Animator self, ref Vector3 lookAtPosition){}
		public void SetLookAtWeight(float weight, float bodyWeight, float headWeight, float eyesWeight, float clampWeight){}
		public void SetLookAtWeight(float weight, float bodyWeight, float headWeight, float eyesWeight){}
		public void SetLookAtWeight(float weight, float bodyWeight, float headWeight){}
		public void SetLookAtWeight(float weight, float bodyWeight){}
		public void SetLookAtWeight(float weight){}
		public string GetLayerName(int layerIndex){}
		public float GetLayerWeight(int layerIndex){}
		public void SetLayerWeight(int layerIndex, float weight){}
		public AnimatorStateInfo GetCurrentAnimatorStateInfo(int layerIndex){}
		public Animator(){}
		public bool isHuman{ get	{} }
		public float humanScale{ get	{} }
		public Vector3 deltaPosition{ get	{} }
		public Quaternion deltaRotation{ get	{} }
		public Vector3 rootPosition{ get	{} set	{} }
		public Quaternion rootRotation{ get	{} set	{} }
		public bool applyRootMotion{ get	{} set	{} }
		public bool animatePhysics{ get	{} set	{} }
		public float gravityWeight{ get	{} }
		public Vector3 bodyPosition{ get	{} set	{} }
		public Quaternion bodyRotation{ get	{} set	{} }
		public bool stabilizeFeet{ get	{} set	{} }
		public int layerCount{ get	{} }
		public float feetPivotActive{ get	{} set	{} }
		public float pivotWeight{ get	{} }
		public Vector3 pivotPosition{ get	{} }
		public bool isMatchingTarget{ get	{} }
		public float speed{ get	{} set	{} }
		public Vector3 targetPosition{ get	{} }
		public Quaternion targetRotation{ get	{} }
		public AnimatorCullingMode cullingMode{ get	{} set	{} }
		public Avatar avatar{ get	{} set	{} }
		public bool layersAffectMassCenter{ get	{} set	{} }
		bool supportsOnAnimatorMove{ get	{} }
	}

	public sealed class	Avatar: Object
	{
		internal void SetMuscleMinMax(int muscleId, float min, float max){}
		internal void SetParameter(int parameterId, float value){}
		public Avatar(){}
		public bool isValid{ get	{} }
		public bool isHuman{ get	{} }
	}

	public sealed class	AudioSettings: Object
	{
		public static void SetDSPBufferSize(int bufferLength, int numBuffers){}
		public static void GetDSPBufferSize(out int bufferLength, out int numBuffers){}
		public AudioSettings(){}
		public static AudioSpeakerMode driverCaps{ get	{} }
		public static AudioSpeakerMode speakerMode{ get	{} set	{} }
		public static int outputSampleRate{ get	{} set	{} }
	}

	public sealed class	AudioClip: Object
	{
		public void GetData(Single[] data, int offsetSamples){}
		public void SetData(Single[] data, int offsetSamples){}
		public static AudioClip Create(string name, int lengthSamples, int channels, int frequency, bool _3D, bool stream){}
		public static AudioClip Create(string name, int lengthSamples, int channels, int frequency, bool _3D, bool stream, PCMReaderCallback pcmreadercallback){}
		public static AudioClip Create(string name, int lengthSamples, int channels, int frequency, bool _3D, bool stream, PCMReaderCallback pcmreadercallback, PCMSetPositionCallback pcmsetpositioncallback){}
		private void InvokePCMReaderCallback_Internal(Single[] data){}
		private void InvokePCMSetPositionCallback_Internal(int position){}
		private static AudioClip Construct_Internal(){}
		private void Init_Internal(string name, int lengthSamples, int channels, int frequency, bool _3D, bool stream){}
		public AudioClip(){}
		public float length{ get	{} }
		public int samples{ get	{} }
		public int channels{ get	{} }
		public int frequency{ get	{} }
		public bool isReadyToPlay{ get	{} }
		event	PCMReaderCallback m_PCMReaderCallback;
		event	PCMSetPositionCallback m_PCMSetPositionCallback;
	}

	public sealed class	AudioListener: Behaviour
	{
		private static void GetOutputDataHelper(Single[] samples, int channel){}
		private static void GetSpectrumDataHelper(Single[] samples, int channel, FFTWindow window){}
		public static Single[] GetOutputData(int numSamples, int channel){}
		public static void GetOutputData(Single[] samples, int channel){}
		public static Single[] GetSpectrumData(int numSamples, int channel, FFTWindow window){}
		public static void GetSpectrumData(Single[] samples, int channel, FFTWindow window){}
		public AudioListener(){}
		public static float volume{ get	{} set	{} }
		public static bool pause{ get	{} set	{} }
		public AudioVelocityUpdateMode velocityUpdateMode{ get	{} set	{} }
	}

	public sealed class	Microphone: Object
	{
		public static AudioClip Start(string deviceName, bool loop, int lengthSec, int frequency){}
		public static void End(string deviceName){}
		public static bool IsRecording(string deviceName){}
		public static int GetPosition(string deviceName){}
		public static void GetDeviceCaps(string deviceName, out int minFreq, out int maxFreq){}
		public Microphone(){}
		public static String[] devices{ get	{} }
	}

	public sealed class	AudioSource: Behaviour
	{
		public void Play(ulong delay){}
		public void Play(){}
		public void Stop(){}
		private static void INTERNAL_CALL_Stop(AudioSource self){}
		public void Pause(){}
		private static void INTERNAL_CALL_Pause(AudioSource self){}
		public void PlayOneShot(AudioClip clip, float volumeScale){}
		public void PlayOneShot(AudioClip clip){}
		public static void PlayClipAtPoint(AudioClip clip, Vector3 position){}
		public static void PlayClipAtPoint(AudioClip clip, Vector3 position, float volume){}
		private void GetOutputDataHelper(Single[] samples, int channel){}
		public Single[] GetOutputData(int numSamples, int channel){}
		public void GetOutputData(Single[] samples, int channel){}
		private void GetSpectrumDataHelper(Single[] samples, int channel, FFTWindow window){}
		public Single[] GetSpectrumData(int numSamples, int channel, FFTWindow window){}
		public void GetSpectrumData(Single[] samples, int channel, FFTWindow window){}
		public AudioSource(){}
		public float volume{ get	{} set	{} }
		public float pitch{ get	{} set	{} }
		public float time{ get	{} set	{} }
		public int timeSamples{ get	{} set	{} }
		public AudioClip clip{ get	{} set	{} }
		public bool isPlaying{ get	{} }
		public bool loop{ get	{} set	{} }
		public bool ignoreListenerVolume{ get	{} set	{} }
		public bool playOnAwake{ get	{} set	{} }
		public AudioVelocityUpdateMode velocityUpdateMode{ get	{} set	{} }
		public float panLevel{ get	{} set	{} }
		public bool bypassEffects{ get	{} set	{} }
		public float dopplerLevel{ get	{} set	{} }
		public float spread{ get	{} set	{} }
		public int priority{ get	{} set	{} }
		public bool mute{ get	{} set	{} }
		public float minDistance{ get	{} set	{} }
		public float maxDistance{ get	{} set	{} }
		public float pan{ get	{} set	{} }
		public AudioRolloffMode rolloffMode{ get	{} set	{} }
		public float minVolume{ get	{} set	{} }
		public float maxVolume{ get	{} set	{} }
		public float rolloffFactor{ get	{} set	{} }
	}

	public sealed class	AudioReverbZone: Behaviour
	{
		public AudioReverbZone(){}
		public float minDistance{ get	{} set	{} }
		public float maxDistance{ get	{} set	{} }
		public AudioReverbPreset reverbPreset{ get	{} set	{} }
		public int room{ get	{} set	{} }
		public int roomHF{ get	{} set	{} }
		public int roomLF{ get	{} set	{} }
		public float decayTime{ get	{} set	{} }
		public float decayHFRatio{ get	{} set	{} }
		public int reflections{ get	{} set	{} }
		public float reflectionsDelay{ get	{} set	{} }
		public int reverb{ get	{} set	{} }
		public float reverbDelay{ get	{} set	{} }
		public float HFReference{ get	{} set	{} }
		public float LFReference{ get	{} set	{} }
		public float roomRolloffFactor{ get	{} set	{} }
		public float diffusion{ get	{} set	{} }
		public float density{ get	{} set	{} }
	}

	public sealed class	AudioLowPassFilter: Behaviour
	{
		public AudioLowPassFilter(){}
		public float cutoffFrequency{ get	{} set	{} }
		public float lowpassResonaceQ{ get	{} set	{} }
	}

	public sealed class	AudioHighPassFilter: Behaviour
	{
		public AudioHighPassFilter(){}
		public float cutoffFrequency{ get	{} set	{} }
		public float highpassResonaceQ{ get	{} set	{} }
	}

	public sealed class	AudioDistortionFilter: Behaviour
	{
		public AudioDistortionFilter(){}
		public float distortionLevel{ get	{} set	{} }
	}

	public sealed class	AudioEchoFilter: Behaviour
	{
		public AudioEchoFilter(){}
		public float delay{ get	{} set	{} }
		public float decayRatio{ get	{} set	{} }
		public float dryMix{ get	{} set	{} }
		public float wetMix{ get	{} set	{} }
	}

	public sealed class	AudioChorusFilter: Behaviour
	{
		public AudioChorusFilter(){}
		public float dryMix{ get	{} set	{} }
		public float wetMix1{ get	{} set	{} }
		public float wetMix2{ get	{} set	{} }
		public float wetMix3{ get	{} set	{} }
		public float delay{ get	{} set	{} }
		public float rate{ get	{} set	{} }
		public float depth{ get	{} set	{} }
		public float feedback{ get	{} set	{} }
	}

	public sealed class	AudioReverbFilter: Behaviour
	{
		public AudioReverbFilter(){}
		public AudioReverbPreset reverbPreset{ get	{} set	{} }
		public float dryLevel{ get	{} set	{} }
		public float room{ get	{} set	{} }
		public float roomHF{ get	{} set	{} }
		public float roomRolloff{ get	{} set	{} }
		public float decayTime{ get	{} set	{} }
		public float decayHFRatio{ get	{} set	{} }
		public float reflectionsLevel{ get	{} set	{} }
		public float reflectionsDelay{ get	{} set	{} }
		public float reverbLevel{ get	{} set	{} }
		public float reverbDelay{ get	{} set	{} }
		public float diffusion{ get	{} set	{} }
		public float density{ get	{} set	{} }
		public float hfReference{ get	{} set	{} }
		public float roomLF{ get	{} set	{} }
		public float lFReference{ get	{} set	{} }
	}

	public sealed class	PlayerPrefs: Object
	{
		private static bool TrySetInt(string key, int value){}
		private static bool TrySetFloat(string key, float value){}
		private static bool TrySetSetString(string key, string value){}
		public static void SetInt(string key, int value){}
		public static int GetInt(string key, int defaultValue){}
		public static int GetInt(string key){}
		public static void SetFloat(string key, float value){}
		public static float GetFloat(string key, float defaultValue){}
		public static float GetFloat(string key){}
		public static void SetString(string key, string value){}
		public static string GetString(string key, string defaultValue){}
		public static string GetString(string key){}
		public static bool HasKey(string key){}
		public static void DeleteKey(string key){}
		public static void DeleteAll(){}
		public static void Save(){}
		public PlayerPrefs(){}
	}

	public sealed class	PlayerPrefsException: Exception, ISerializable, _Exception
	{
		public PlayerPrefsException(string error){}
	}

	public sealed class	SystemInfo: Object
	{
		public static bool SupportsRenderTextureFormat(RenderTextureFormat format){}
		public SystemInfo(){}
		public static string operatingSystem{ get	{} }
		public static string processorType{ get	{} }
		public static int processorCount{ get	{} }
		public static int systemMemorySize{ get	{} }
		public static int graphicsMemorySize{ get	{} }
		public static string graphicsDeviceName{ get	{} }
		public static string graphicsDeviceVendor{ get	{} }
		public static int graphicsDeviceID{ get	{} }
		public static int graphicsDeviceVendorID{ get	{} }
		public static string graphicsDeviceVersion{ get	{} }
		public static int graphicsShaderLevel{ get	{} }
		public static int graphicsPixelFillrate{ get	{} }
		public static bool supportsShadows{ get	{} }
		public static bool supportsRenderTextures{ get	{} }
		public static bool supportsImageEffects{ get	{} }
		public static bool supports3DTextures{ get	{} }
		public static bool supportsComputeShaders{ get	{} }
		public static bool supportsInstancing{ get	{} }
		public static int supportedRenderTargetCount{ get	{} }
		public static bool supportsVertexPrograms{ get	{} }
		public static string deviceUniqueIdentifier{ get	{} }
		public static string deviceName{ get	{} }
		public static string deviceModel{ get	{} }
		public static bool supportsAccelerometer{ get	{} }
		public static bool supportsGyroscope{ get	{} }
		public static bool supportsLocationService{ get	{} }
		public static bool supportsVibration{ get	{} }
		public static DeviceType deviceType{ get	{} }
	}

	public sealed class	WaitForSeconds: YieldInstruction
	{
		public WaitForSeconds(float seconds){}
		internal float m_Seconds;
	}

	public sealed class	WaitForFixedUpdate: YieldInstruction
	{
		public WaitForFixedUpdate(){}
	}

	public sealed class	WaitForEndOfFrame: YieldInstruction
	{
		public WaitForEndOfFrame(){}
	}

	public sealed class	Coroutine: YieldInstruction
	{
		private void ReleaseCoroutine(){}
		protected virtual void Finalize(){}
		private Coroutine(){}
		private IntPtr m_Ptr;
	}

	public sealed class	RequireComponent: Attribute, _Attribute
	{
		public RequireComponent(Type requiredComponent){}
		public RequireComponent(Type requiredComponent, Type requiredComponent2){}
		public RequireComponent(Type requiredComponent, Type requiredComponent2, Type requiredComponent3){}
		public Type m_Type0;
		public Type m_Type1;
		public Type m_Type2;
	}

	public sealed class	AddComponentMenu: Attribute, _Attribute
	{
		public AddComponentMenu(string menuName){}
		public string componentMenu{ get	{} }
		private string m_AddComponentMenu;
	}

	public sealed class	ContextMenu: Attribute, _Attribute
	{
		public ContextMenu(string name){}
		public string menuItem{ get	{} }
		private string m_ItemName;
	}

	public sealed class	ExecuteInEditMode: Attribute, _Attribute
	{
		public ExecuteInEditMode(){}
	}

	public sealed class	HideInInspector: Attribute, _Attribute
	{
		public HideInInspector(){}
	}

	public class	ScriptableObject: Object
	{
		private static void Internal_CreateScriptableObject(ScriptableObject self){}
		public void SetDirty(){}
		private static void INTERNAL_CALL_SetDirty(ScriptableObject self){}
		public static ScriptableObject CreateInstance(string className){}
		public static ScriptableObject CreateInstance(Type type){}
		private static ScriptableObject CreateInstanceFromType(Type type){}
		public static T CreateInstance(){}
		public ScriptableObject(){}
	}

	public sealed class	Resources: Object
	{
		public static Object[] FindObjectsOfTypeAll(Type type){}
		public static System.Object Load(string path){}
		public static System.Object Load(string path, Type systemTypeInstance){}
		public static Object[] LoadAll(string path, Type systemTypeInstance){}
		public static Object[] LoadAll(string path){}
		public static System.Object GetBuiltinResource(Type type, string path){}
		public static System.Object LoadAssetAtPath(string assetPath, Type type){}
		public static void UnloadAsset(System.Object assetToUnload){}
		public static AsyncOperation UnloadUnusedAssets(){}
		public Resources(){}
	}

	public sealed class	AssetBundleCreateRequest: AsyncOperation
	{
		public AssetBundleCreateRequest(){}
		public AssetBundle assetBundle{ get	{} }
	}

	public sealed class	AssetBundleRequest: AsyncOperation
	{
		public AssetBundleRequest(){}
		public System.Object asset{ get	{} }
		private AssetBundle m_AssetBundle;
		private string m_Path;
		private Type m_Type;
	}

	public sealed class	AssetBundle: Object
	{
		public static AssetBundleCreateRequest CreateFromMemory(Byte[] binary){}
		public static AssetBundle CreateFromFile(string path){}
		public bool Contains(string name){}
		public System.Object Load(string name){}
		public System.Object Load(string name, Type type){}
		public AssetBundleRequest LoadAsync(string name, Type type){}
		public Object[] LoadAll(Type type){}
		public Object[] LoadAll(){}
		public void Unload(bool unloadAllLoadedObjects){}
		public AssetBundle(){}
		public System.Object mainAsset{ get	{} }
	}

	public sealed class	SerializePrivateVariables: Attribute, _Attribute
	{
		public SerializePrivateVariables(){}
	}

	public sealed class	SerializeField: Attribute, _Attribute
	{
		public SerializeField(){}
	}

	public sealed class	Profiler: Object
	{
		public static void AddFramesFromFile(string file){}
		public static void BeginSample(string name){}
		public static void BeginSample(string name, System.Object targetObject){}
		private static void BeginSampleOnly(string name){}
		public static void EndSample(){}
		public static int GetRuntimeMemorySize(System.Object o){}
		public static uint GetMonoHeapSize(){}
		public static uint GetMonoUsedSize(){}
		public Profiler(){}
		public static bool supported{ get	{} }
		public static string logFile{ get	{} set	{} }
		public static bool enableBinaryLog{ get	{} set	{} }
		public static bool enabled{ get	{} set	{} }
		public static uint usedHeapSize{ get	{} }
	}

	public sealed class	Cursor: Object
	{
		private static void SetCursor(Texture2D texture, CursorMode cursorMode){}
		public static void SetCursor(Texture2D texture, Vector2 hotspot, CursorMode cursorMode){}
		private static void INTERNAL_CALL_SetCursor(Texture2D texture, ref Vector2 hotspot, CursorMode cursorMode){}
		public Cursor(){}
	}

	public sealed class	OcclusionArea: Component
	{
		private void INTERNAL_get_center(out Vector3 value){}
		private void INTERNAL_set_center(ref Vector3 value){}
		private void INTERNAL_get_size(out Vector3 value){}
		private void INTERNAL_set_size(ref Vector3 value){}
		public OcclusionArea(){}
		public Vector3 center{ get	{} set	{} }
		public Vector3 size{ get	{} set	{} }
	}

	public sealed class	OcclusionPortal: Component
	{
		public OcclusionPortal(){}
		public bool open{ get	{} set	{} }
	}

	public sealed class	RenderSettings: Object
	{
		private static void INTERNAL_get_fogColor(out Color value){}
		private static void INTERNAL_set_fogColor(ref Color value){}
		private static void INTERNAL_get_ambientLight(out Color value){}
		private static void INTERNAL_set_ambientLight(ref Color value){}
		public RenderSettings(){}
		public static bool fog{ get	{} set	{} }
		public static FogMode fogMode{ get	{} set	{} }
		public static Color fogColor{ get	{} set	{} }
		public static float fogDensity{ get	{} set	{} }
		public static float fogStartDistance{ get	{} set	{} }
		public static float fogEndDistance{ get	{} set	{} }
		public static Color ambientLight{ get	{} set	{} }
		public static float haloStrength{ get	{} set	{} }
		public static float flareStrength{ get	{} set	{} }
		public static Material skybox{ get	{} set	{} }
	}

	public sealed class	QualitySettings: Object
	{
		public static int GetQualityLevel(){}
		public static void SetQualityLevel(int index, bool applyExpensiveChanges){}
		public static void SetQualityLevel(int index){}
		public static void IncreaseLevel(bool applyExpensiveChanges){}
		public static void IncreaseLevel(){}
		public static void DecreaseLevel(bool applyExpensiveChanges){}
		public static void DecreaseLevel(){}
		public QualitySettings(){}
		public static String[] names{ get	{} }
		public static QualityLevel currentLevel{ get	{} set	{} }
		public static int pixelLightCount{ get	{} set	{} }
		public static ShadowProjection shadowProjection{ get	{} set	{} }
		public static int shadowCascades{ get	{} set	{} }
		public static float shadowDistance{ get	{} set	{} }
		public static int masterTextureLimit{ get	{} set	{} }
		public static AnisotropicFiltering anisotropicFiltering{ get	{} set	{} }
		public static float lodBias{ get	{} set	{} }
		public static int maximumLODLevel{ get	{} set	{} }
		public static int particleRaycastBudget{ get	{} set	{} }
		public static bool softVegetation{ get	{} set	{} }
		public static int maxQueuedFrames{ get	{} set	{} }
		public static int vSyncCount{ get	{} set	{} }
		public static int antiAliasing{ get	{} set	{} }
		public static ColorSpace desiredColorSpace{ get	{} }
		public static ColorSpace activeColorSpace{ get	{} }
		public static BlendWeights blendWeights{ get	{} set	{} }
	}

	public sealed class	Shader: Object
	{
		public static Shader Find(string name){}
		internal static Shader FindBuiltin(string name){}
		public static void EnableKeyword(string keyword){}
		public static void DisableKeyword(string keyword){}
		public static void SetGlobalColor(string propertyName, Color color){}
		private static void INTERNAL_CALL_SetGlobalColor(string propertyName, ref Color color){}
		public static void SetGlobalVector(string propertyName, Vector4 vec){}
		public static void SetGlobalFloat(string propertyName, float value){}
		public static void SetGlobalTexture(string propertyName, Texture tex){}
		public static void SetGlobalMatrix(string propertyName, Matrix4x4 mat){}
		private static void INTERNAL_CALL_SetGlobalMatrix(string propertyName, ref Matrix4x4 mat){}
		public static void SetGlobalTexGenMode(string propertyName, TexGenMode mode){}
		public static void SetGlobalTextureMatrixName(string propertyName, string matrixName){}
		public static void SetGlobalBuffer(string propertyName, ComputeBuffer buffer){}
		public static int PropertyToID(string name){}
		public static void WarmupAllShaders(){}
		public Shader(){}
		public bool isSupported{ get	{} }
		public int maximumLOD{ get	{} set	{} }
		public static int globalMaximumLOD{ get	{} set	{} }
		public int renderQueue{ get	{} }
	}

	public class	Texture: Object
	{
		public static void SetGlobalAnisotropicFilteringLimits(int forcedMin, int globalMax){}
		private static int Internal_GetWidth(Texture mono){}
		private static int Internal_GetHeight(Texture mono){}
		private static void Internal_GetTexelSize(Texture tex, out Vector2 output){}
		public IntPtr GetNativeTexturePtr(){}
		public int GetNativeTextureID(){}
		public Texture(){}
		public static int masterTextureLimit{ get	{} set	{} }
		public static AnisotropicFiltering anisotropicFiltering{ get	{} set	{} }
		public virtual int width{ get	{} set	{} }
		public virtual int height{ get	{} set	{} }
		public FilterMode filterMode{ get	{} set	{} }
		public int anisoLevel{ get	{} set	{} }
		public TextureWrapMode wrapMode{ get	{} set	{} }
		public float mipMapBias{ get	{} set	{} }
		public Vector2 texelSize{ get	{} }
	}

	public sealed class	Texture2D: Texture
	{
		private static void Internal_Create(Texture2D mono, int width, int height, TextureFormat format, bool mipmap, bool linear){}
		public void SetPixel(int x, int y, Color color){}
		private static void INTERNAL_CALL_SetPixel(Texture2D self, int x, int y, ref Color color){}
		public Color GetPixel(int x, int y){}
		public Color GetPixelBilinear(float u, float v){}
		public void SetPixels(Color[] colors){}
		public void SetPixels(Color[] colors, int miplevel){}
		public void SetPixels(int x, int y, int blockWidth, int blockHeight, Color[] colors, int miplevel){}
		public void SetPixels(int x, int y, int blockWidth, int blockHeight, Color[] colors){}
		public void SetPixels32(Color32[] colors, int miplevel){}
		public void SetPixels32(Color32[] colors){}
		public bool LoadImage(Byte[] data){}
		public Color[] GetPixels(){}
		public Color[] GetPixels(int miplevel){}
		public Color[] GetPixels(int x, int y, int blockWidth, int blockHeight, int miplevel){}
		public Color[] GetPixels(int x, int y, int blockWidth, int blockHeight){}
		public Color32[] GetPixels32(int miplevel){}
		public Color32[] GetPixels32(){}
		public void Apply(bool updateMipmaps, bool makeNoLongerReadable){}
		public void Apply(bool updateMipmaps){}
		public void Apply(){}
		public bool Resize(int width, int height, TextureFormat format, bool hasMipMap){}
		public bool Resize(int width, int height){}
		private bool Internal_ResizeWH(int width, int height){}
		public void Compress(bool highQuality){}
		private static void INTERNAL_CALL_Compress(Texture2D self, bool highQuality){}
		public Rect[] PackTextures(Texture2D[] textures, int padding, int maximumAtlasSize, bool makeNoLongerReadable){}
		public Rect[] PackTextures(Texture2D[] textures, int padding, int maximumAtlasSize){}
		public Rect[] PackTextures(Texture2D[] textures, int padding){}
		public void ReadPixels(Rect source, int destX, int destY, bool recalculateMipMaps){}
		public void ReadPixels(Rect source, int destX, int destY){}
		private static void INTERNAL_CALL_ReadPixels(Texture2D self, ref Rect source, int destX, int destY, bool recalculateMipMaps){}
		public Byte[] EncodeToPNG(){}
		public Texture2D(int width, int height){}
		public Texture2D(int width, int height, TextureFormat format, bool mipmap){}
		public Texture2D(int width, int height, TextureFormat format, bool mipmap, bool linear){}
		public int mipmapCount{ get	{} }
		public TextureFormat format{ get	{} }
	}

	public sealed class	Cubemap: Texture
	{
		public void SetPixel(CubemapFace face, int x, int y, Color color){}
		private static void INTERNAL_CALL_SetPixel(Cubemap self, CubemapFace face, int x, int y, ref Color color){}
		public Color GetPixel(CubemapFace face, int x, int y){}
		public Color[] GetPixels(CubemapFace face, int miplevel){}
		public Color[] GetPixels(CubemapFace face){}
		public void SetPixels(Color[] colors, CubemapFace face, int miplevel){}
		public void SetPixels(Color[] colors, CubemapFace face){}
		public void Apply(bool updateMipmaps, bool makeNoLongerReadable){}
		public void Apply(bool updateMipmaps){}
		public void Apply(){}
		private static void Internal_Create(Cubemap mono, int size, TextureFormat format, bool mipmap){}
		public void SmoothEdges(int smoothRegionWidthInPixels){}
		public void SmoothEdges(){}
		public Cubemap(int size, TextureFormat format, bool mipmap){}
		public TextureFormat format{ get	{} }
	}

	public sealed class	Texture3D: Texture
	{
		public Color[] GetPixels(int miplevel){}
		public Color[] GetPixels(){}
		public void SetPixels(Color[] colors, int miplevel){}
		public void SetPixels(Color[] colors){}
		public void Apply(bool updateMipmaps){}
		public void Apply(){}
		private static void Internal_Create(Texture3D mono, int width, int height, int depth, TextureFormat format, bool mipmap){}
		public Texture3D(int width, int height, int depth, TextureFormat format, bool mipmap){}
		public int depth{ get	{} }
		public TextureFormat format{ get	{} }
	}

	public sealed class	MeshFilter: Component
	{
		public MeshFilter(){}
		public Mesh mesh{ get	{} set	{} }
		public Mesh sharedMesh{ get	{} set	{} }
	}

	public sealed class	Mesh: Object
	{
		private static void Internal_Create(Mesh mono){}
		public void Clear(bool keepVertexLayout){}
		public void Clear(){}
		private void INTERNAL_get_bounds(out Bounds value){}
		private void INTERNAL_set_bounds(ref Bounds value){}
		public void RecalculateBounds(){}
		public void RecalculateNormals(){}
		public void Optimize(){}
		public Int32[] GetTriangles(int submesh){}
		public void SetTriangles(Int32[] triangles, int submesh){}
		public Int32[] GetIndices(int submesh){}
		public void SetIndices(Int32[] indices, MeshTopology topology, int submesh){}
		public MeshTopology GetTopology(int submesh){}
		public void SetTriangleStrip(Int32[] triangles, int submesh){}
		public Int32[] GetTriangleStrip(int submesh){}
		public void CombineMeshes(CombineInstance[] combine, bool mergeSubMeshes, bool useMatrices){}
		public void CombineMeshes(CombineInstance[] combine, bool mergeSubMeshes){}
		public void CombineMeshes(CombineInstance[] combine){}
		public void MarkDynamic(){}
		public Mesh(){}
		public bool isReadable{ get	{} }
		bool canAccess{ get	{} }
		public Vector3[] vertices{ get	{} set	{} }
		public Vector3[] normals{ get	{} set	{} }
		public Vector4[] tangents{ get	{} set	{} }
		public Vector2[] uv{ get	{} set	{} }
		public Vector2[] uv2{ get	{} set	{} }
		public Vector2[] uv1{ get	{} set	{} }
		public Bounds bounds{ get	{} set	{} }
		public Color[] colors{ get	{} set	{} }
		public Color32[] colors32{ get	{} set	{} }
		public Int32[] triangles{ get	{} set	{} }
		public int vertexCount{ get	{} }
		public int subMeshCount{ get	{} set	{} }
		public BoneWeight[] boneWeights{ get	{} set	{} }
		public Matrix4x4[] bindposes{ get	{} set	{} }
	}

	public class	SkinnedMeshRenderer: Renderer
	{
		private void INTERNAL_get_localBounds(out Bounds value){}
		private void INTERNAL_set_localBounds(ref Bounds value){}
		public void BakeMesh(Mesh mesh){}
		public SkinnedMeshRenderer(){}
		public Transform[] bones{ get	{} set	{} }
		public Transform rootBone{ get	{} set	{} }
		public SkinQuality quality{ get	{} set	{} }
		public Mesh sharedMesh{ get	{} set	{} }
		public bool skinNormals{ get	{} set	{} }
		public bool updateWhenOffscreen{ get	{} set	{} }
		public Bounds localBounds{ get	{} set	{} }
		Transform actualRootBone{ get	{} }
	}

	public class	Material: Object
	{
		public void SetColor(string propertyName, Color color){}
		private static void INTERNAL_CALL_SetColor(Material self, string propertyName, ref Color color){}
		public Color GetColor(string propertyName){}
		public void SetVector(string propertyName, Vector4 vector){}
		public Vector4 GetVector(string propertyName){}
		public void SetTexture(string propertyName, Texture texture){}
		public Texture GetTexture(string propertyName){}
		private static void Internal_GetTextureOffset(Material mat, string name, out Vector2 output){}
		private static void Internal_GetTextureScale(Material mat, string name, out Vector2 output){}
		private static void Internal_GetTexturePivot(Material mat, string name, out Vector2 output){}
		public void SetTextureOffset(string propertyName, Vector2 offset){}
		private static void INTERNAL_CALL_SetTextureOffset(Material self, string propertyName, ref Vector2 offset){}
		public Vector2 GetTextureOffset(string propertyName){}
		public void SetTextureScale(string propertyName, Vector2 scale){}
		private static void INTERNAL_CALL_SetTextureScale(Material self, string propertyName, ref Vector2 scale){}
		public Vector2 GetTextureScale(string propertyName){}
		public void SetMatrix(string propertyName, Matrix4x4 matrix){}
		private static void INTERNAL_CALL_SetMatrix(Material self, string propertyName, ref Matrix4x4 matrix){}
		public Matrix4x4 GetMatrix(string propertyName){}
		public void SetFloat(string propertyName, float value){}
		public float GetFloat(string propertyName){}
		public void SetBuffer(string propertyName, ComputeBuffer buffer){}
		public bool HasProperty(string propertyName){}
		public string GetTag(string tag, bool searchFallbacks, string defaultValue){}
		public string GetTag(string tag, bool searchFallbacks){}
		public void Lerp(Material start, Material end, float t){}
		public bool SetPass(int pass){}
		public static Material Create(string scriptContents){}
		private static void Internal_CreateWithString(Material mono, string contents){}
		private static void Internal_CreateWithShader(Material mono, Shader shader){}
		private static void Internal_CreateWithMaterial(Material mono, Material source){}
		public void CopyPropertiesFromMaterial(Material mat){}
		public Material(string contents){}
		public Material(Shader shader){}
		public Material(Material source){}
		public Shader shader{ get	{} set	{} }
		public Color color{ get	{} set	{} }
		public Texture mainTexture{ get	{} set	{} }
		public Vector2 mainTextureOffset{ get	{} set	{} }
		public Vector2 mainTextureScale{ get	{} set	{} }
		public int passCount{ get	{} }
		public int renderQueue{ get	{} set	{} }
	}

	public sealed class	Flare: Object
	{
		public Flare(){}
	}

	public sealed class	LensFlare: Behaviour
	{
		private void INTERNAL_get_color(out Color value){}
		private void INTERNAL_set_color(ref Color value){}
		public LensFlare(){}
		public Flare flare{ get	{} set	{} }
		public float brightness{ get	{} set	{} }
		public Color color{ get	{} set	{} }
	}

	public class	Renderer: Component
	{
		internal void SetSubsetIndex(int index, int subSetIndexForMaterial){}
		private void INTERNAL_get_worldToLocalMatrix(out Matrix4x4 value){}
		private void INTERNAL_get_localToWorldMatrix(out Matrix4x4 value){}
		private void INTERNAL_get_lightmapTilingOffset(out Vector4 value){}
		private void INTERNAL_set_lightmapTilingOffset(ref Vector4 value){}
		public void SetPropertyBlock(MaterialPropertyBlock properties){}
		public void Render(int material){}
		public Renderer(){}
		Transform staticBatchRootTransform{ get	{} set	{} }
		int staticBatchIndex{ get	{} }
		public bool isPartOfStaticBatch{ get	{} }
		public Matrix4x4 worldToLocalMatrix{ get	{} }
		public Matrix4x4 localToWorldMatrix{ get	{} }
		public bool enabled{ get	{} set	{} }
		public bool castShadows{ get	{} set	{} }
		public bool receiveShadows{ get	{} set	{} }
		public Material material{ get	{} set	{} }
		public Material sharedMaterial{ get	{} set	{} }
		public Material[] sharedMaterials{ get	{} set	{} }
		public Material[] materials{ get	{} set	{} }
		public Bounds bounds{ get	{} }
		public int lightmapIndex{ get	{} set	{} }
		public Vector4 lightmapTilingOffset{ get	{} set	{} }
		public bool isVisible{ get	{} }
		public bool useLightProbes{ get	{} set	{} }
		public Transform lightProbeAnchor{ get	{} set	{} }
	}

	public sealed class	Projector: Behaviour
	{
		public Projector(){}
		public float nearClipPlane{ get	{} set	{} }
		public float farClipPlane{ get	{} set	{} }
		public float fieldOfView{ get	{} set	{} }
		public float aspectRatio{ get	{} set	{} }
		public bool isOrthoGraphic{ get	{} set	{} }
		public bool orthographic{ get	{} set	{} }
		public float orthographicSize{ get	{} set	{} }
		public float orthoGraphicSize{ get	{} set	{} }
		public int ignoreLayers{ get	{} set	{} }
		public Material material{ get	{} set	{} }
	}

	public sealed class	Skybox: Behaviour
	{
		public Skybox(){}
		public Material material{ get	{} set	{} }
	}

	public sealed class	TextMesh: Component
	{
		public TextMesh(){}
		public string text{ get	{} set	{} }
		public Font font{ get	{} set	{} }
		public int fontSize{ get	{} set	{} }
		public FontStyle fontStyle{ get	{} set	{} }
		public float offsetZ{ get	{} set	{} }
		public TextAlignment alignment{ get	{} set	{} }
		public TextAnchor anchor{ get	{} set	{} }
		public float characterSize{ get	{} set	{} }
		public float lineSpacing{ get	{} set	{} }
		public float tabSize{ get	{} set	{} }
		public bool richText{ get	{} set	{} }
	}

	public sealed class	ParticleEmitter: Component
	{
		private void INTERNAL_get_worldVelocity(out Vector3 value){}
		private void INTERNAL_set_worldVelocity(ref Vector3 value){}
		private void INTERNAL_get_localVelocity(out Vector3 value){}
		private void INTERNAL_set_localVelocity(ref Vector3 value){}
		private void INTERNAL_get_rndVelocity(out Vector3 value){}
		private void INTERNAL_set_rndVelocity(ref Vector3 value){}
		public void ClearParticles(){}
		private static void INTERNAL_CALL_ClearParticles(ParticleEmitter self){}
		public void Emit(){}
		public void Emit(int count){}
		public void Emit(Vector3 pos, Vector3 velocity, float size, float energy, Color color){}
		public void Emit(Vector3 pos, Vector3 velocity, float size, float energy, Color color, float rotation, float angularVelocity){}
		private void Emit2(int count){}
		private void Emit3(ref InternalEmitParticleArguments args){}
		public void Simulate(float deltaTime){}
		public ParticleEmitter(){}
		public bool emit{ get	{} set	{} }
		public float minSize{ get	{} set	{} }
		public float maxSize{ get	{} set	{} }
		public float minEnergy{ get	{} set	{} }
		public float maxEnergy{ get	{} set	{} }
		public float minEmission{ get	{} set	{} }
		public float maxEmission{ get	{} set	{} }
		public float emitterVelocityScale{ get	{} set	{} }
		public Vector3 worldVelocity{ get	{} set	{} }
		public Vector3 localVelocity{ get	{} set	{} }
		public Vector3 rndVelocity{ get	{} set	{} }
		public bool useWorldSpace{ get	{} set	{} }
		public bool rndRotation{ get	{} set	{} }
		public float angularVelocity{ get	{} set	{} }
		public float rndAngularVelocity{ get	{} set	{} }
		public Particle[] particles{ get	{} set	{} }
		public int particleCount{ get	{} }
		public bool enabled{ get	{} set	{} }
	}

	public sealed class	ParticleAnimator: Component
	{
		private void INTERNAL_get_worldRotationAxis(out Vector3 value){}
		private void INTERNAL_set_worldRotationAxis(ref Vector3 value){}
		private void INTERNAL_get_localRotationAxis(out Vector3 value){}
		private void INTERNAL_set_localRotationAxis(ref Vector3 value){}
		private void INTERNAL_get_rndForce(out Vector3 value){}
		private void INTERNAL_set_rndForce(ref Vector3 value){}
		private void INTERNAL_get_force(out Vector3 value){}
		private void INTERNAL_set_force(ref Vector3 value){}
		public ParticleAnimator(){}
		public bool doesAnimateColor{ get	{} set	{} }
		public Vector3 worldRotationAxis{ get	{} set	{} }
		public Vector3 localRotationAxis{ get	{} set	{} }
		public float sizeGrow{ get	{} set	{} }
		public Vector3 rndForce{ get	{} set	{} }
		public Vector3 force{ get	{} set	{} }
		public float damping{ get	{} set	{} }
		public bool autodestruct{ get	{} set	{} }
		public Color[] colorAnimation{ get	{} set	{} }
	}

	public sealed class	TrailRenderer: Renderer
	{
		public TrailRenderer(){}
		public float time{ get	{} set	{} }
		public float startWidth{ get	{} set	{} }
		public float endWidth{ get	{} set	{} }
		public bool autodestruct{ get	{} set	{} }
	}

	public sealed class	ParticleRenderer: Renderer
	{
		public ParticleRenderer(){}
		public ParticleRenderMode particleRenderMode{ get	{} set	{} }
		public float lengthScale{ get	{} set	{} }
		public float velocityScale{ get	{} set	{} }
		public float cameraVelocityScale{ get	{} set	{} }
		public float maxParticleSize{ get	{} set	{} }
		public int uvAnimationXTile{ get	{} set	{} }
		public int uvAnimationYTile{ get	{} set	{} }
		public float uvAnimationCycles{ get	{} set	{} }
		public int animatedTextureCount{ get	{} set	{} }
		public float maxPartileSize{ get	{} set	{} }
		public Rect[] uvTiles{ get	{} set	{} }
		public AnimationCurve widthCurve{ get	{} set	{} }
		public AnimationCurve heightCurve{ get	{} set	{} }
		public AnimationCurve rotationCurve{ get	{} set	{} }
	}

	public sealed class	LineRenderer: Renderer
	{
		public void SetWidth(float start, float end){}
		private static void INTERNAL_CALL_SetWidth(LineRenderer self, float start, float end){}
		public void SetColors(Color start, Color end){}
		private static void INTERNAL_CALL_SetColors(LineRenderer self, ref Color start, ref Color end){}
		public void SetVertexCount(int count){}
		private static void INTERNAL_CALL_SetVertexCount(LineRenderer self, int count){}
		public void SetPosition(int index, Vector3 position){}
		private static void INTERNAL_CALL_SetPosition(LineRenderer self, int index, ref Vector3 position){}
		public LineRenderer(){}
		public bool useWorldSpace{ get	{} set	{} }
	}

	public sealed class	MaterialPropertyBlock: Object
	{
		internal void InitBlock(){}
		internal void DestroyBlock(){}
		protected virtual void Finalize(){}
		public void AddFloat(string name, float value){}
		public void AddFloat(int nameID, float value){}
		public void AddVector(string name, Vector4 value){}
		public void AddVector(int nameID, Vector4 value){}
		private static void INTERNAL_CALL_AddVector(MaterialPropertyBlock self, int nameID, ref Vector4 value){}
		public void AddColor(string name, Color value){}
		public void AddColor(int nameID, Color value){}
		private static void INTERNAL_CALL_AddColor(MaterialPropertyBlock self, int nameID, ref Color value){}
		public void AddMatrix(string name, Matrix4x4 value){}
		public void AddMatrix(int nameID, Matrix4x4 value){}
		private static void INTERNAL_CALL_AddMatrix(MaterialPropertyBlock self, int nameID, ref Matrix4x4 value){}
		public void Clear(){}
		public MaterialPropertyBlock(){}
		private IntPtr m_Ptr;
	}

	public sealed class	Graphics: Object
	{
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex){}
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera){}
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer){}
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties){}
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex){}
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera){}
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer){}
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties){}
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, bool castShadows, bool receiveShadows){}
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, bool castShadows, bool receiveShadows){}
		private static void Internal_DrawMeshTR(ref Internal_DrawMeshTRArguments arguments, bool castShadows, bool receiveShadows){}
		private static void Internal_DrawMeshMatrix(ref Internal_DrawMeshMatrixArguments arguments, bool castShadows, bool receiveShadows){}
		public static void DrawMeshNow(Mesh mesh, Vector3 position, Quaternion rotation){}
		public static void DrawMeshNow(Mesh mesh, Vector3 position, Quaternion rotation, int materialIndex){}
		public static void DrawMeshNow(Mesh mesh, Matrix4x4 matrix){}
		public static void DrawMeshNow(Mesh mesh, Matrix4x4 matrix, int materialIndex){}
		private static void Internal_DrawMeshNow1(Mesh mesh, Vector3 position, Quaternion rotation, int materialIndex){}
		private static void INTERNAL_CALL_Internal_DrawMeshNow1(Mesh mesh, ref Vector3 position, ref Quaternion rotation, int materialIndex){}
		private static void Internal_DrawMeshNow2(Mesh mesh, Matrix4x4 matrix, int materialIndex){}
		private static void INTERNAL_CALL_Internal_DrawMeshNow2(Mesh mesh, ref Matrix4x4 matrix, int materialIndex){}
		public static void DrawProcedural(MeshTopology topology, int vertexCount, int instanceCount){}
		public static void DrawProcedural(MeshTopology topology, int vertexCount){}
		public static void DrawProceduralIndirect(MeshTopology topology, ComputeBuffer bufferWithArgs, int argsOffset){}
		public static void DrawProceduralIndirect(MeshTopology topology, ComputeBuffer bufferWithArgs){}
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation){}
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, int materialIndex){}
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix){}
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, int materialIndex){}
		public static void DrawTexture(Rect screenRect, Texture texture){}
		public static void DrawTexture(Rect screenRect, Texture texture, Material mat){}
		public static void DrawTexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder){}
		public static void DrawTexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Material mat){}
		public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder){}
		public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Material mat){}
		public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Color color){}
		public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Color color, Material mat){}
		internal static void DrawTexture(ref InternalDrawTextureArguments arguments){}
		public static void Blit(Texture source, RenderTexture dest){}
		public static void Blit(Texture source, RenderTexture dest, Material mat){}
		public static void Blit(Texture source, RenderTexture dest, Material mat, int pass){}
		public static void Blit(Texture source, Material mat){}
		public static void Blit(Texture source, Material mat, int pass){}
		private static void Internal_BlitMaterial(Texture source, RenderTexture dest, Material mat, int pass, bool setRT){}
		public static void BlitMultiTap(Texture source, RenderTexture dest, Material mat, Vector2[] offsets){}
		private static void Internal_BlitMultiTap(Texture source, RenderTexture dest, Material mat, Vector2[] offsets){}
		public static void SetRenderTarget(RenderTexture rt){}
		public static void SetRenderTarget(RenderBuffer colorBuffer, RenderBuffer depthBuffer){}
		public static void SetRenderTarget(RenderBuffer[] colorBuffers, RenderBuffer depthBuffer){}
		private static void Internal_SetRT(RenderTexture rt){}
		private static void Internal_SetRTBuffer(out RenderBuffer colorBuffer, out RenderBuffer depthBuffer){}
		private static void Internal_SetRTBuffers(RenderBuffer[] colorBuffers, out RenderBuffer depthBuffer){}
		private static void GetActiveColorBuffer(out RenderBuffer res){}
		private static void GetActiveDepthBuffer(out RenderBuffer res){}
		public static void SetRandomWriteTarget(int index, RenderTexture uav){}
		public static void SetRandomWriteTarget(int index, ComputeBuffer uav){}
		public static void ClearRandomWriteTargets(){}
		private static void Internal_SetRandomWriteTargetRT(int index, RenderTexture uav){}
		private static void Internal_SetRandomWriteTargetBuffer(int index, ComputeBuffer uav){}
		internal static void SetupVertexLights(Light[] lights){}
		public Graphics(){}
		public static RenderBuffer activeColorBuffer{ get	{} }
		public static RenderBuffer activeDepthBuffer{ get	{} }
		public static string deviceName{ get	{} }
		public static string deviceVendor{ get	{} }
		public static string deviceVersion{ get	{} }
		public static bool supportsVertexProgram{ get	{} }
	}

	public sealed class	LightmapData: Object
	{
		public LightmapData(){}
		public Texture2D lightmapFar{ get	{} set	{} }
		public Texture2D lightmap{ get	{} set	{} }
		public Texture2D lightmapNear{ get	{} set	{} }
		private Texture2D m_Lightmap;
		private Texture2D m_IndirectLightmap;
	}

	public sealed class	LightProbes: Object
	{
		public void GetInterpolatedLightProbe(Vector3 position, Renderer renderer, Single[] coefficients){}
		private static void INTERNAL_CALL_GetInterpolatedLightProbe(LightProbes self, ref Vector3 position, Renderer renderer, Single[] coefficients){}
		public LightProbes(){}
		public Vector3[] positions{ get	{} }
		public Single[] coefficients{ get	{} set	{} }
		public int count{ get	{} }
		public int cellCount{ get	{} }
	}

	public sealed class	LightmapSettings: Object
	{
		public LightmapSettings(){}
		public static LightmapData[] lightmaps{ get	{} set	{} }
		public static LightmapsMode lightmapsMode{ get	{} set	{} }
		public static ColorSpace bakedColorSpace{ get	{} set	{} }
		public static LightProbes lightProbes{ get	{} set	{} }
	}

	public sealed class	GeometryUtility: Object
	{
		public static Plane[] CalculateFrustumPlanes(Camera camera){}
		public static Plane[] CalculateFrustumPlanes(Matrix4x4 worldToProjectionMatrix){}
		private static void Internal_ExtractPlanes(Plane[] planes, Matrix4x4 worldToProjectionMatrix){}
		private static void INTERNAL_CALL_Internal_ExtractPlanes(Plane[] planes, ref Matrix4x4 worldToProjectionMatrix){}
		public static bool TestPlanesAABB(Plane[] planes, Bounds bounds){}
		private static bool INTERNAL_CALL_TestPlanesAABB(Plane[] planes, ref Bounds bounds){}
		public GeometryUtility(){}
	}

	public sealed class	Screen: Object
	{
		public static void SetResolution(int width, int height, bool fullscreen, int preferredRefreshRate){}
		public static void SetResolution(int width, int height, bool fullscreen){}
		public Screen(){}
		public static Resolution[] resolutions{ get	{} }
		public static Resolution[] GetResolution{ get	{} }
		public static Resolution currentResolution{ get	{} }
		public static bool showCursor{ get	{} set	{} }
		public static bool lockCursor{ get	{} set	{} }
		public static int width{ get	{} }
		public static int height{ get	{} }
		public static float dpi{ get	{} }
		public static bool fullScreen{ get	{} set	{} }
		public static bool autorotateToPortrait{ get	{} set	{} }
		public static bool autorotateToPortraitUpsideDown{ get	{} set	{} }
		public static bool autorotateToLandscapeLeft{ get	{} set	{} }
		public static bool autorotateToLandscapeRight{ get	{} set	{} }
		public static ScreenOrientation orientation{ get	{} set	{} }
		public static int sleepTimeout{ get	{} set	{} }
	}

	public sealed class	SleepTimeout: Object
	{
		public SleepTimeout(){}
		public const int NeverSleep = null;
		public const int SystemSetting = null;
	}

	public sealed class	RenderTexture: Texture
	{
		private static void Internal_CreateRenderTexture(RenderTexture rt){}
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format, RenderTextureReadWrite readWrite){}
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format){}
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer){}
		public static RenderTexture GetTemporary(int width, int height){}
		public static void ReleaseTemporary(RenderTexture temp){}
		private static int Internal_GetWidth(RenderTexture mono){}
		private static void Internal_SetWidth(RenderTexture mono, int width){}
		private static int Internal_GetHeight(RenderTexture mono){}
		private static void Internal_SetHeight(RenderTexture mono, int width){}
		private static void Internal_SetSRGBReadWrite(RenderTexture mono, bool sRGB){}
		public bool Create(){}
		private static bool INTERNAL_CALL_Create(RenderTexture self){}
		public void Release(){}
		private static void INTERNAL_CALL_Release(RenderTexture self){}
		public bool IsCreated(){}
		private static bool INTERNAL_CALL_IsCreated(RenderTexture self){}
		public void DiscardContents(){}
		private static void INTERNAL_CALL_DiscardContents(RenderTexture self){}
		private void GetColorBuffer(out RenderBuffer res){}
		private void GetDepthBuffer(out RenderBuffer res){}
		public void SetGlobalShaderProperty(string propertyName){}
		private static void Internal_GetTexelOffset(RenderTexture tex, out Vector2 output){}
		public Vector2 GetTexelOffset(){}
		public void SetBorderColor(Color color){}
		public RenderTexture(int width, int height, int depth, RenderTextureFormat format, RenderTextureReadWrite readWrite){}
		public RenderTexture(int width, int height, int depth, RenderTextureFormat format){}
		public RenderTexture(int width, int height, int depth){}
		public virtual int width{ get	{} set	{} }
		public virtual int height{ get	{} set	{} }
		public int depth{ get	{} set	{} }
		public bool isPowerOfTwo{ get	{} set	{} }
		public bool sRGB{ get	{} }
		public RenderTextureFormat format{ get	{} set	{} }
		public bool useMipMap{ get	{} set	{} }
		public bool isCubemap{ get	{} set	{} }
		public bool isVolume{ get	{} set	{} }
		public int volumeDepth{ get	{} set	{} }
		public bool enableRandomWrite{ get	{} set	{} }
		public RenderBuffer colorBuffer{ get	{} }
		public RenderBuffer depthBuffer{ get	{} }
		public static RenderTexture active{ get	{} set	{} }
		public static bool enabled{ get	{} set	{} }
	}

	public sealed class	MovieTexture: Texture
	{
		public void Play(){}
		private static void INTERNAL_CALL_Play(MovieTexture self){}
		public void Stop(){}
		private static void INTERNAL_CALL_Stop(MovieTexture self){}
		public void Pause(){}
		private static void INTERNAL_CALL_Pause(MovieTexture self){}
		public MovieTexture(){}
		public AudioClip audioClip{ get	{} }
		public bool loop{ get	{} set	{} }
		public bool isPlaying{ get	{} }
		public bool isReadyToPlay{ get	{} }
		public float duration{ get	{} }
	}

	public sealed class	GL: Object
	{
		public static void Vertex3(float x, float y, float z){}
		public static void Vertex(Vector3 v){}
		private static void INTERNAL_CALL_Vertex(ref Vector3 v){}
		public static void Color(Color c){}
		private static void INTERNAL_CALL_Color(ref Color c){}
		public static void TexCoord(Vector3 v){}
		private static void INTERNAL_CALL_TexCoord(ref Vector3 v){}
		public static void TexCoord2(float x, float y){}
		public static void TexCoord3(float x, float y, float z){}
		public static void MultiTexCoord2(int unit, float x, float y){}
		public static void MultiTexCoord3(int unit, float x, float y, float z){}
		public static void MultiTexCoord(int unit, Vector3 v){}
		private static void INTERNAL_CALL_MultiTexCoord(int unit, ref Vector3 v){}
		public static void Begin(int mode){}
		public static void End(){}
		public static void LoadOrtho(){}
		public static void LoadPixelMatrix(){}
		private static void LoadPixelMatrixArgs(float left, float right, float bottom, float top){}
		public static void LoadPixelMatrix(float left, float right, float bottom, float top){}
		public static void Viewport(Rect pixelRect){}
		private static void INTERNAL_CALL_Viewport(ref Rect pixelRect){}
		public static void LoadProjectionMatrix(Matrix4x4 mat){}
		private static void INTERNAL_CALL_LoadProjectionMatrix(ref Matrix4x4 mat){}
		public static void LoadIdentity(){}
		private static void INTERNAL_get_modelview(out Matrix4x4 value){}
		private static void INTERNAL_set_modelview(ref Matrix4x4 value){}
		public static void MultMatrix(Matrix4x4 mat){}
		private static void INTERNAL_CALL_MultMatrix(ref Matrix4x4 mat){}
		public static void PushMatrix(){}
		public static void PopMatrix(){}
		public static Matrix4x4 GetGPUProjectionMatrix(Matrix4x4 proj, bool renderIntoTexture){}
		private static Matrix4x4 INTERNAL_CALL_GetGPUProjectionMatrix(ref Matrix4x4 proj, bool renderIntoTexture){}
		public static void SetRevertBackfacing(bool revertBackFaces){}
		public static void Clear(bool clearDepth, bool clearColor, Color backgroundColor){}
		private static void INTERNAL_CALL_Clear(bool clearDepth, bool clearColor, ref Color backgroundColor){}
		public static void ClearWithSkybox(bool clearDepth, Camera camera){}
		public static void InvalidateState(){}
		public static void IssuePluginEvent(int eventID){}
		public GL(){}
		public static Matrix4x4 modelview{ get	{} set	{} }
		public static bool wireframe{ get	{} set	{} }
		public const int TRIANGLES = null;
		public const int TRIANGLE_STRIP = null;
		public const int QUADS = null;
		public const int LINES = null;
	}

	public class	GUIElement: Behaviour
	{
		public bool HitTest(Vector3 screenPosition, Camera camera){}
		public bool HitTest(Vector3 screenPosition){}
		private static bool INTERNAL_CALL_HitTest(GUIElement self, ref Vector3 screenPosition, Camera camera){}
		public Rect GetScreenRect(Camera camera){}
		public Rect GetScreenRect(){}
		public GUIElement(){}
	}

	public sealed class	GUITexture: GUIElement
	{
		private void INTERNAL_get_color(out Color value){}
		private void INTERNAL_set_color(ref Color value){}
		private void INTERNAL_get_pixelInset(out Rect value){}
		private void INTERNAL_set_pixelInset(ref Rect value){}
		public GUITexture(){}
		public Color color{ get	{} set	{} }
		public Texture texture{ get	{} set	{} }
		public Rect pixelInset{ get	{} set	{} }
		public RectOffset border{ get	{} set	{} }
	}

	public sealed class	GUIText: GUIElement
	{
		private void Internal_GetPixelOffset(out Vector2 output){}
		private void Internal_SetPixelOffset(Vector2 p){}
		private static void INTERNAL_CALL_Internal_SetPixelOffset(GUIText self, ref Vector2 p){}
		public GUIText(){}
		public string text{ get	{} set	{} }
		public Material material{ get	{} set	{} }
		public Vector2 pixelOffset{ get	{} set	{} }
		public Font font{ get	{} set	{} }
		public TextAlignment alignment{ get	{} set	{} }
		public TextAnchor anchor{ get	{} set	{} }
		public float lineSpacing{ get	{} set	{} }
		public float tabSize{ get	{} set	{} }
		public int fontSize{ get	{} set	{} }
		public FontStyle fontStyle{ get	{} set	{} }
		public bool richText{ get	{} set	{} }
	}

	public sealed class	Font: Object
	{
		public bool HasCharacter(char c){}
		public void RequestCharactersInTexture(string characters, int size, FontStyle style){}
		public void RequestCharactersInTexture(string characters, int size){}
		public void RequestCharactersInTexture(string characters){}
		private void InvokeFontTextureRebuildCallback_Internal(){}
		public bool GetCharacterInfo(char ch, out CharacterInfo info, int size, FontStyle style){}
		public bool GetCharacterInfo(char ch, out CharacterInfo info, int size){}
		public bool GetCharacterInfo(char ch, out CharacterInfo info){}
		public Font(){}
		public Material material{ get	{} set	{} }
		public String[] fontNames{ get	{} set	{} }
		public CharacterInfo[] characterInfo{ get	{} set	{} }
		public FontTextureRebuildCallback textureRebuildCallback{ get	{} set	{} }
		event	FontTextureRebuildCallback m_FontTextureRebuildCallback;
	}

	public sealed class	GUILayer: Behaviour
	{
		public GUIElement HitTest(Vector3 screenPosition){}
		private static GUIElement INTERNAL_CALL_HitTest(GUILayer self, ref Vector3 screenPosition){}
		public GUILayer(){}
	}

	public sealed class	MeshRenderer: Renderer
	{
		public MeshRenderer(){}
	}

	public sealed class	StaticBatchingUtility: Object
	{
		public static void Combine(GameObject staticBatchRoot){}
		public static void Combine(GameObject[] gos, GameObject staticBatchRoot){}
		internal static Mesh InternalCombineVertices(MeshInstance[] meshes, string meshName){}
		internal static void InternalCombineIndices(SubMeshInstance[] submeshes, bool generateStrips, ref Mesh combinedMesh){}
		public StaticBatchingUtility(){}
	}

	public sealed class	ImageEffectTransformsToLDR: Attribute, _Attribute
	{
		public ImageEffectTransformsToLDR(){}
	}

	public sealed class	ImageEffectOpaque: Attribute, _Attribute
	{
		public ImageEffectOpaque(){}
	}

	public sealed class	LODGroup: Component
	{
		private void INTERNAL_get_localReferencePoint(out Vector3 value){}
		private void INTERNAL_set_localReferencePoint(ref Vector3 value){}
		public void RecalculateBounds(){}
		public void SetLODS(LOD[] scriptingLODs){}
		public void ForceLOD(int index){}
		public LODGroup(){}
		public Vector3 localReferencePoint{ get	{} set	{} }
		public float size{ get	{} set	{} }
		public int lodCount{ get	{} }
		public bool enabled{ get	{} set	{} }
	}

	public sealed class	Gradient: Object
	{
		private void Init(){}
		private void Cleanup(){}
		protected virtual void Finalize(){}
		public Color Evaluate(float time){}
		private void INTERNAL_get_constantColor(out Color value){}
		private void INTERNAL_set_constantColor(ref Color value){}
		public void SetKeys(GradientColorKey[] colorKeys, GradientAlphaKey[] alphaKeys){}
		public Gradient(){}
		GradientColorKey[] colorKeys{ get	{} set	{} }
		GradientAlphaKey[] alphaKeys{ get	{} set	{} }
		Color constantColor{ get	{} set	{} }
		private IntPtr m_Ptr;
	}

	public class	GUI: Object
	{
		public static void BringWindowToFront(int windowID){}
		public static void BringWindowToBack(int windowID){}
		public static void FocusWindow(int windowID){}
		public static void UnfocusWindow(){}
		internal static void BeginWindows(int skinMode, int editorWindowInstanceID){}
		private static void Internal_BeginWindows(){}
		internal static void EndWindows(){}
		private static void Internal_EndWindows(){}
		public static string PasswordField(Rect position, string password, char maskChar, int maxLength){}
		public static string PasswordField(Rect position, string password, char maskChar, GUIStyle style){}
		public static string PasswordField(Rect position, string password, char maskChar, int maxLength, GUIStyle style){}
		internal static string PasswordFieldGetStrToShow(string password, char maskChar){}
		public static string TextArea(Rect position, string text){}
		public static string TextArea(Rect position, string text, int maxLength){}
		public static string TextArea(Rect position, string text, GUIStyle style){}
		public static string TextArea(Rect position, string text, int maxLength, GUIStyle style){}
		private static string TextArea(Rect position, GUIContent content, int maxLength, GUIStyle style){}
		internal static void DoTextField(Rect position, int id, GUIContent content, bool multiline, int maxLength, GUIStyle style){}
		public static void SetNextControlName(string name){}
		public static string GetNameOfFocusedControl(){}
		public static void FocusControl(string name){}
		public static bool Toggle(Rect position, bool value, string text){}
		public static bool Toggle(Rect position, bool value, Texture image){}
		public static bool Toggle(Rect position, bool value, GUIContent content){}
		public static bool Toggle(Rect position, bool value, string text, GUIStyle style){}
		public static bool Toggle(Rect position, bool value, Texture image, GUIStyle style){}
		public static bool Toggle(Rect position, bool value, GUIContent content, GUIStyle style){}
		internal static bool DoToggle(Rect position, int id, bool value, GUIContent content, IntPtr style){}
		private static bool INTERNAL_CALL_DoToggle(ref Rect position, int id, bool value, GUIContent content, IntPtr style){}
		public static int Toolbar(Rect position, int selected, String[] texts){}
		public static int Toolbar(Rect position, int selected, Texture[] images){}
		public static int Toolbar(Rect position, int selected, GUIContent[] content){}
		public static int Toolbar(Rect position, int selected, String[] texts, GUIStyle style){}
		public static int Toolbar(Rect position, int selected, Texture[] images, GUIStyle style){}
		public static int Toolbar(Rect position, int selected, GUIContent[] contents, GUIStyle style){}
		public static int SelectionGrid(Rect position, int selected, String[] texts, int xCount){}
		public static int SelectionGrid(Rect position, int selected, Texture[] images, int xCount){}
		public static int SelectionGrid(Rect position, int selected, GUIContent[] content, int xCount){}
		public static int SelectionGrid(Rect position, int selected, String[] texts, int xCount, GUIStyle style){}
		public static int SelectionGrid(Rect position, int selected, Texture[] images, int xCount, GUIStyle style){}
		public static int SelectionGrid(Rect position, int selected, GUIContent[] contents, int xCount, GUIStyle style){}
		internal static void FindStyles(ref GUIStyle style, out GUIStyle firstStyle, out GUIStyle midStyle, out GUIStyle lastStyle, string first, string mid, string last){}
		internal static int CalcTotalHorizSpacing(int xCount, GUIStyle style, GUIStyle firstStyle, GUIStyle midStyle, GUIStyle lastStyle){}
		private static int DoButtonGrid(Rect position, int selected, GUIContent[] contents, int xCount, GUIStyle style, GUIStyle firstStyle, GUIStyle midStyle, GUIStyle lastStyle){}
		private static Rect[] CalcMouseRects(Rect position, int count, int xCount, float elemWidth, float elemHeight, GUIStyle style, GUIStyle firstStyle, GUIStyle midStyle, GUIStyle lastStyle, bool addBorders){}
		private static int GetButtonGridMouseSelection(Rect[] buttonRects, Vector2 mousePos, bool findNearest){}
		public static float HorizontalSlider(Rect position, float value, float leftValue, float rightValue){}
		public static float HorizontalSlider(Rect position, float value, float leftValue, float rightValue, GUIStyle slider, GUIStyle thumb){}
		public static float VerticalSlider(Rect position, float value, float topValue, float bottomValue){}
		public static float VerticalSlider(Rect position, float value, float topValue, float bottomValue, GUIStyle slider, GUIStyle thumb){}
		public static float Slider(Rect position, float value, float size, float start, float end, GUIStyle slider, GUIStyle thumb, bool horiz, int id){}
		public static float HorizontalScrollbar(Rect position, float value, float size, float leftValue, float rightValue){}
		public static float HorizontalScrollbar(Rect position, float value, float size, float leftValue, float rightValue, GUIStyle style){}
		internal static void InternalRepaintEditorWindow(){}
		internal static bool ScrollerRepeatButton(int scrollerID, Rect rect, GUIStyle style){}
		public static float VerticalScrollbar(Rect position, float value, float size, float topValue, float bottomValue){}
		public static float VerticalScrollbar(Rect position, float value, float size, float topValue, float bottomValue, GUIStyle style){}
		private static float Scroller(Rect position, float value, float size, float leftValue, float rightValue, GUIStyle slider, GUIStyle thumb, GUIStyle leftButton, GUIStyle rightButton, bool horiz){}
		public static void BeginGroup(Rect position){}
		public static void BeginGroup(Rect position, string text){}
		public static void BeginGroup(Rect position, Texture image){}
		public static void BeginGroup(Rect position, GUIContent content){}
		public static void BeginGroup(Rect position, GUIStyle style){}
		public static void BeginGroup(Rect position, string text, GUIStyle style){}
		public static void BeginGroup(Rect position, Texture image, GUIStyle style){}
		public static void BeginGroup(Rect position, GUIContent content, GUIStyle style){}
		public static void EndGroup(){}
		public static Vector2 BeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect){}
		public static Vector2 BeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical){}
		public static Vector2 BeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar){}
		public static Vector2 BeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar){}
		protected static Vector2 DoBeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background){}
		internal static Vector2 BeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background){}
		public static void EndScrollView(){}
		public static void EndScrollView(bool handleScrollWheel){}
		internal static ScrollViewState GetTopScrollView(){}
		public static void ScrollTo(Rect position){}
		public static Rect Window(int id, Rect clientRect, WindowFunction func, string text){}
		public static Rect Window(int id, Rect clientRect, WindowFunction func, Texture image){}
		public static Rect Window(int id, Rect clientRect, WindowFunction func, GUIContent content){}
		public static Rect Window(int id, Rect clientRect, WindowFunction func, string text, GUIStyle style){}
		public static Rect Window(int id, Rect clientRect, WindowFunction func, Texture image, GUIStyle style){}
		public static Rect Window(int id, Rect clientRect, WindowFunction func, GUIContent title, GUIStyle style){}
		internal static void CallWindowDelegate(WindowFunction func, int id, GUISkin _skin, int forceRect, float width, float height, GUIStyle style){}
		private static Rect DoWindow(int id, Rect clientRect, WindowFunction func, GUIContent title, GUIStyle style, GUISkin skin, bool forceRectOnLayout){}
		private static Rect INTERNAL_CALL_DoWindow(int id, ref Rect clientRect, WindowFunction func, GUIContent title, GUIStyle style, GUISkin skin, bool forceRectOnLayout){}
		public static void DragWindow(Rect position){}
		private static void INTERNAL_CALL_DragWindow(ref Rect position){}
		public static void DragWindow(){}
		private static void INTERNAL_get_color(out Color value){}
		private static void INTERNAL_set_color(ref Color value){}
		private static void INTERNAL_get_backgroundColor(out Color value){}
		private static void INTERNAL_set_backgroundColor(ref Color value){}
		private static void INTERNAL_get_contentColor(out Color value){}
		private static void INTERNAL_set_contentColor(ref Color value){}
		private static string Internal_GetTooltip(){}
		private static void Internal_SetTooltip(string value){}
		private static string Internal_GetMouseTooltip(){}
		public static void Label(Rect position, string text){}
		public static void Label(Rect position, Texture image){}
		public static void Label(Rect position, GUIContent content){}
		public static void Label(Rect position, string text, GUIStyle style){}
		public static void Label(Rect position, Texture image, GUIStyle style){}
		public static void Label(Rect position, GUIContent content, GUIStyle style){}
		private static void DoLabel(Rect position, GUIContent content, IntPtr style){}
		private static void INTERNAL_CALL_DoLabel(ref Rect position, GUIContent content, IntPtr style){}
		private static void InitializeGUIClipTexture(){}
		public static void DrawTexture(Rect position, Texture image, ScaleMode scaleMode, bool alphaBlend){}
		public static void DrawTexture(Rect position, Texture image, ScaleMode scaleMode){}
		public static void DrawTexture(Rect position, Texture image){}
		public static void DrawTexture(Rect position, Texture image, ScaleMode scaleMode, bool alphaBlend, float imageAspect){}
		public static void DrawTextureWithTexCoords(Rect position, Texture image, Rect texCoords){}
		public static void DrawTextureWithTexCoords(Rect position, Texture image, Rect texCoords, bool alphaBlend){}
		public static void Box(Rect position, string text){}
		public static void Box(Rect position, Texture image){}
		public static void Box(Rect position, GUIContent content){}
		public static void Box(Rect position, string text, GUIStyle style){}
		public static void Box(Rect position, Texture image, GUIStyle style){}
		public static void Box(Rect position, GUIContent content, GUIStyle style){}
		public static bool Button(Rect position, string text){}
		public static bool Button(Rect position, Texture image){}
		public static bool Button(Rect position, GUIContent content){}
		public static bool Button(Rect position, string text, GUIStyle style){}
		public static bool Button(Rect position, Texture image, GUIStyle style){}
		public static bool Button(Rect position, GUIContent content, GUIStyle style){}
		private static bool DoButton(Rect position, GUIContent content, IntPtr style){}
		private static bool INTERNAL_CALL_DoButton(ref Rect position, GUIContent content, IntPtr style){}
		public static bool RepeatButton(Rect position, string text){}
		public static bool RepeatButton(Rect position, Texture image){}
		public static bool RepeatButton(Rect position, GUIContent content){}
		public static bool RepeatButton(Rect position, string text, GUIStyle style){}
		public static bool RepeatButton(Rect position, Texture image, GUIStyle style){}
		public static bool RepeatButton(Rect position, GUIContent content, GUIStyle style){}
		private static bool DoRepeatButton(Rect position, GUIContent content, GUIStyle style, FocusType focusType){}
		public static string TextField(Rect position, string text){}
		public static string TextField(Rect position, string text, int maxLength){}
		public static string TextField(Rect position, string text, GUIStyle style){}
		public static string TextField(Rect position, string text, int maxLength, GUIStyle style){}
		public static string PasswordField(Rect position, string password, char maskChar){}
		public GUI(){}
		private static GUI(){}
		DateTime nextScrollStepTime{ get	{} set	{} }
		int scrollTroughSide{ get	{} set	{} }
		public static GUISkin skin{ get	{} set	{} }
		public static Color color{ get	{} set	{} }
		public static Color backgroundColor{ get	{} set	{} }
		public static Color contentColor{ get	{} set	{} }
		public static bool changed{ get	{} set	{} }
		public static bool enabled{ get	{} set	{} }
		public static Matrix4x4 matrix{ get	{} set	{} }
		public static string tooltip{ get	{} set	{} }
		string mouseTooltip{ get	{} }
		Rect tooltipRect{ get	{} set	{} }
		public static int depth{ get	{} set	{} }
		Material blendMaterial{ get	{} }
		Material blitMaterial{ get	{} }
		bool usePageScrollbars{ get	{} }
		private static float scrollStepSize;
		private static int scrollControlID;
		private static GUISkin s_Skin;
		internal static Rect s_ToolTipRect;
		private static int boxHash;
		private static int repeatButtonHash;
		private static int toggleHash;
		private static int buttonGridHash;
		private static int sliderHash;
		private static int beginGroupHash;
		private static int scrollviewHash;
		private static Stack s_ScrollViewStates;
		private static DateTime <nextScrollStepTime>k__BackingField;
		private static int <scrollTroughSide>k__BackingField;
	}

	public sealed class	GUILayout: Object
	{
		public static void BeginArea(Rect screenRect){}
		public static void BeginArea(Rect screenRect, string text){}
		public static void BeginArea(Rect screenRect, Texture image){}
		public static void BeginArea(Rect screenRect, GUIContent content){}
		public static void BeginArea(Rect screenRect, GUIStyle style){}
		public static void BeginArea(Rect screenRect, string text, GUIStyle style){}
		public static void BeginArea(Rect screenRect, Texture image, GUIStyle style){}
		public static void BeginArea(Rect screenRect, GUIContent content, GUIStyle style){}
		public static void EndArea(){}
		public static Vector2 BeginScrollView(Vector2 scrollPosition, GUILayoutOption[] options){}
		public static Vector2 BeginScrollView(Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, GUILayoutOption[] options){}
		public static Vector2 BeginScrollView(Vector2 scrollPosition, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUILayoutOption[] options){}
		public static Vector2 BeginScrollView(Vector2 scrollPosition, GUIStyle style){}
		public static Vector2 BeginScrollView(Vector2 scrollPosition, GUIStyle style, GUILayoutOption[] options){}
		public static Vector2 BeginScrollView(Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUILayoutOption[] options){}
		public static Vector2 BeginScrollView(Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background, GUILayoutOption[] options){}
		public static void EndScrollView(){}
		internal static void EndScrollView(bool handleScrollWheel){}
		public static Rect Window(int id, Rect screenRect, WindowFunction func, string text, GUILayoutOption[] options){}
		public static Rect Window(int id, Rect screenRect, WindowFunction func, Texture image, GUILayoutOption[] options){}
		public static Rect Window(int id, Rect screenRect, WindowFunction func, GUIContent content, GUILayoutOption[] options){}
		public static Rect Window(int id, Rect screenRect, WindowFunction func, string text, GUIStyle style, GUILayoutOption[] options){}
		public static Rect Window(int id, Rect screenRect, WindowFunction func, Texture image, GUIStyle style, GUILayoutOption[] options){}
		public static Rect Window(int id, Rect screenRect, WindowFunction func, GUIContent content, GUIStyle style, GUILayoutOption[] options){}
		private static Rect DoWindow(int id, Rect screenRect, WindowFunction func, GUIContent content, GUIStyle style, GUILayoutOption[] options){}
		public static GUILayoutOption Width(float width){}
		public static GUILayoutOption MinWidth(float minWidth){}
		public static GUILayoutOption MaxWidth(float maxWidth){}
		public static GUILayoutOption Height(float height){}
		public static GUILayoutOption MinHeight(float minHeight){}
		public static GUILayoutOption MaxHeight(float maxHeight){}
		public static GUILayoutOption ExpandWidth(bool expand){}
		public static GUILayoutOption ExpandHeight(bool expand){}
		public static void Label(Texture image, GUILayoutOption[] options){}
		public static void Label(string text, GUILayoutOption[] options){}
		public static void Label(GUIContent content, GUILayoutOption[] options){}
		public static void Label(Texture image, GUIStyle style, GUILayoutOption[] options){}
		public static void Label(string text, GUIStyle style, GUILayoutOption[] options){}
		public static void Label(GUIContent content, GUIStyle style, GUILayoutOption[] options){}
		private static void DoLabel(GUIContent content, GUIStyle style, GUILayoutOption[] options){}
		public static void Box(Texture image, GUILayoutOption[] options){}
		public static void Box(string text, GUILayoutOption[] options){}
		public static void Box(GUIContent content, GUILayoutOption[] options){}
		public static void Box(Texture image, GUIStyle style, GUILayoutOption[] options){}
		public static void Box(string text, GUIStyle style, GUILayoutOption[] options){}
		public static void Box(GUIContent content, GUIStyle style, GUILayoutOption[] options){}
		private static void DoBox(GUIContent content, GUIStyle style, GUILayoutOption[] options){}
		public static bool Button(Texture image, GUILayoutOption[] options){}
		public static bool Button(string text, GUILayoutOption[] options){}
		public static bool Button(GUIContent content, GUILayoutOption[] options){}
		public static bool Button(Texture image, GUIStyle style, GUILayoutOption[] options){}
		public static bool Button(string text, GUIStyle style, GUILayoutOption[] options){}
		public static bool Button(GUIContent content, GUIStyle style, GUILayoutOption[] options){}
		private static bool DoButton(GUIContent content, GUIStyle style, GUILayoutOption[] options){}
		public static bool RepeatButton(Texture image, GUILayoutOption[] options){}
		public static bool RepeatButton(string text, GUILayoutOption[] options){}
		public static bool RepeatButton(GUIContent content, GUILayoutOption[] options){}
		public static bool RepeatButton(Texture image, GUIStyle style, GUILayoutOption[] options){}
		public static bool RepeatButton(string text, GUIStyle style, GUILayoutOption[] options){}
		public static bool RepeatButton(GUIContent content, GUIStyle style, GUILayoutOption[] options){}
		private static bool DoRepeatButton(GUIContent content, GUIStyle style, GUILayoutOption[] options){}
		public static string TextField(string text, GUILayoutOption[] options){}
		public static string TextField(string text, int maxLength, GUILayoutOption[] options){}
		public static string TextField(string text, GUIStyle style, GUILayoutOption[] options){}
		public static string TextField(string text, int maxLength, GUIStyle style, GUILayoutOption[] options){}
		public static string PasswordField(string password, char maskChar, GUILayoutOption[] options){}
		public static string PasswordField(string password, char maskChar, int maxLength, GUILayoutOption[] options){}
		public static string PasswordField(string password, char maskChar, GUIStyle style, GUILayoutOption[] options){}
		public static string PasswordField(string password, char maskChar, int maxLength, GUIStyle style, GUILayoutOption[] options){}
		public static string TextArea(string text, GUILayoutOption[] options){}
		public static string TextArea(string text, int maxLength, GUILayoutOption[] options){}
		public static string TextArea(string text, GUIStyle style, GUILayoutOption[] options){}
		public static string TextArea(string text, int maxLength, GUIStyle style, GUILayoutOption[] options){}
		private static string DoTextField(string text, int maxLength, bool multiline, GUIStyle style, GUILayoutOption[] options){}
		public static bool Toggle(bool value, Texture image, GUILayoutOption[] options){}
		public static bool Toggle(bool value, string text, GUILayoutOption[] options){}
		public static bool Toggle(bool value, GUIContent content, GUILayoutOption[] options){}
		public static bool Toggle(bool value, Texture image, GUIStyle style, GUILayoutOption[] options){}
		public static bool Toggle(bool value, string text, GUIStyle style, GUILayoutOption[] options){}
		public static bool Toggle(bool value, GUIContent content, GUIStyle style, GUILayoutOption[] options){}
		private static bool DoToggle(bool value, GUIContent content, GUIStyle style, GUILayoutOption[] options){}
		public static int Toolbar(int selected, String[] texts, GUILayoutOption[] options){}
		public static int Toolbar(int selected, Texture[] images, GUILayoutOption[] options){}
		public static int Toolbar(int selected, GUIContent[] content, GUILayoutOption[] options){}
		public static int Toolbar(int selected, String[] texts, GUIStyle style, GUILayoutOption[] options){}
		public static int Toolbar(int selected, Texture[] images, GUIStyle style, GUILayoutOption[] options){}
		public static int Toolbar(int selected, GUIContent[] contents, GUIStyle style, GUILayoutOption[] options){}
		public static int SelectionGrid(int selected, String[] texts, int xCount, GUILayoutOption[] options){}
		public static int SelectionGrid(int selected, Texture[] images, int xCount, GUILayoutOption[] options){}
		public static int SelectionGrid(int selected, GUIContent[] content, int xCount, GUILayoutOption[] options){}
		public static int SelectionGrid(int selected, String[] texts, int xCount, GUIStyle style, GUILayoutOption[] options){}
		public static int SelectionGrid(int selected, Texture[] images, int xCount, GUIStyle style, GUILayoutOption[] options){}
		public static int SelectionGrid(int selected, GUIContent[] contents, int xCount, GUIStyle style, GUILayoutOption[] options){}
		public static float HorizontalSlider(float value, float leftValue, float rightValue, GUILayoutOption[] options){}
		public static float HorizontalSlider(float value, float leftValue, float rightValue, GUIStyle slider, GUIStyle thumb, GUILayoutOption[] options){}
		private static float DoHorizontalSlider(float value, float leftValue, float rightValue, GUIStyle slider, GUIStyle thumb, GUILayoutOption[] options){}
		public static float VerticalSlider(float value, float leftValue, float rightValue, GUILayoutOption[] options){}
		public static float VerticalSlider(float value, float leftValue, float rightValue, GUIStyle slider, GUIStyle thumb, GUILayoutOption[] options){}
		private static float DoVerticalSlider(float value, float leftValue, float rightValue, GUIStyle slider, GUIStyle thumb, GUILayoutOption[] options){}
		public static float HorizontalScrollbar(float value, float size, float leftValue, float rightValue, GUILayoutOption[] options){}
		public static float HorizontalScrollbar(float value, float size, float leftValue, float rightValue, GUIStyle style, GUILayoutOption[] options){}
		public static float VerticalScrollbar(float value, float size, float topValue, float bottomValue, GUILayoutOption[] options){}
		public static float VerticalScrollbar(float value, float size, float topValue, float bottomValue, GUIStyle style, GUILayoutOption[] options){}
		public static void Space(float pixels){}
		public static void FlexibleSpace(){}
		public static void BeginHorizontal(GUILayoutOption[] options){}
		public static void BeginHorizontal(GUIStyle style, GUILayoutOption[] options){}
		public static void BeginHorizontal(string text, GUIStyle style, GUILayoutOption[] options){}
		public static void BeginHorizontal(Texture image, GUIStyle style, GUILayoutOption[] options){}
		public static void BeginHorizontal(GUIContent content, GUIStyle style, GUILayoutOption[] options){}
		public static void EndHorizontal(){}
		public static void BeginVertical(GUILayoutOption[] options){}
		public static void BeginVertical(GUIStyle style, GUILayoutOption[] options){}
		public static void BeginVertical(string text, GUIStyle style, GUILayoutOption[] options){}
		public static void BeginVertical(Texture image, GUIStyle style, GUILayoutOption[] options){}
		public static void BeginVertical(GUIContent content, GUIStyle style, GUILayoutOption[] options){}
		public static void EndVertical(){}
		public GUILayout(){}
	}

	public class	GUILayoutUtility: Object
	{
		internal static LayoutCache SelectIDList(int instanceID, bool isWindow){}
		internal static void Begin(int instanceID){}
		internal static void BeginWindow(int windowID, GUIStyle style, GUILayoutOption[] options){}
		public static void BeginGroup(string GroupName){}
		public static void EndGroup(string groupName){}
		internal static void Layout(){}
		internal static void LayoutFromEditorWindow(){}
		internal static float LayoutFromInspector(float width){}
		internal static void LayoutFreeGroup(GUILayoutGroup toplevel){}
		private static void LayoutSingleGroup(GUILayoutGroup i){}
		private static Rect Internal_GetWindowRect(int windowID){}
		private static void Internal_MoveWindow(int windowID, Rect r){}
		private static void INTERNAL_CALL_Internal_MoveWindow(int windowID, ref Rect r){}
		internal static Rect GetWindowsBounds(){}
		private static GUILayoutGroup CreateGUILayoutGroupInstanceOfType(Type LayoutType){}
		internal static GUILayoutGroup BeginLayoutGroup(GUIStyle style, GUILayoutOption[] options, Type LayoutType){}
		internal static void EndLayoutGroup(){}
		internal static GUILayoutGroup BeginLayoutArea(GUIStyle style, Type LayoutType){}
		internal static GUILayoutGroup DoBeginLayoutArea(GUIStyle style, Type LayoutType){}
		public static Rect GetRect(GUIContent content, GUIStyle style){}
		public static Rect GetRect(GUIContent content, GUIStyle style, GUILayoutOption[] options){}
		private static Rect DoGetRect(GUIContent content, GUIStyle style, GUILayoutOption[] options){}
		public static Rect GetRect(float width, float height){}
		public static Rect GetRect(float width, float height, GUIStyle style){}
		public static Rect GetRect(float width, float height, GUILayoutOption[] options){}
		public static Rect GetRect(float width, float height, GUIStyle style, GUILayoutOption[] options){}
		public static Rect GetRect(float minWidth, float maxWidth, float minHeight, float maxHeight){}
		public static Rect GetRect(float minWidth, float maxWidth, float minHeight, float maxHeight, GUIStyle style){}
		public static Rect GetRect(float minWidth, float maxWidth, float minHeight, float maxHeight, GUILayoutOption[] options){}
		public static Rect GetRect(float minWidth, float maxWidth, float minHeight, float maxHeight, GUIStyle style, GUILayoutOption[] options){}
		private static Rect DoGetRect(float minWidth, float maxWidth, float minHeight, float maxHeight, GUIStyle style, GUILayoutOption[] options){}
		public static Rect GetLastRect(){}
		public static Rect GetAspectRect(float aspect){}
		public static Rect GetAspectRect(float aspect, GUIStyle style){}
		public static Rect GetAspectRect(float aspect, GUILayoutOption[] options){}
		public static Rect GetAspectRect(float aspect, GUIStyle style, GUILayoutOption[] options){}
		private static Rect DoGetAspectRect(float aspect, GUIStyle style, GUILayoutOption[] options){}
		public GUILayoutUtility(){}
		private static GUILayoutUtility(){}
		GUILayoutGroup topLevel{ get	{} }
		GUIStyle spaceStyle{ get	{} }
		private static Hashtable storedLayouts;
		private static Hashtable storedWindows;
		internal static LayoutCache current;
		private static Rect kDummyRect;
		private static GUIStyle s_SpaceStyle;
	}

	public sealed class	GUILayoutOption: Object
	{
		internal GUILayoutOption(Type type, System.Object value){}
		internal Type type;
		internal System.Object value;
	}

	public sealed class	ExitGUIException: Exception, ISerializable, _Exception
	{
		public ExitGUIException(){}
	}

	public class	GUIUtility: Object
	{
		public static int GetControlID(FocusType focus){}
		public static int GetControlID(int hint, FocusType focus){}
		public static int GetControlID(GUIContent contents, FocusType focus){}
		public static int GetControlID(FocusType focus, Rect position){}
		public static int GetControlID(int hint, FocusType focus, Rect position){}
		public static int GetControlID(GUIContent contents, FocusType focus, Rect position){}
		private static int Internal_GetNextControlID2(int hint, FocusType focusType, Rect rect){}
		private static int INTERNAL_CALL_Internal_GetNextControlID2(int hint, FocusType focusType, ref Rect rect){}
		public static System.Object GetStateObject(Type t, int controlID){}
		public static System.Object QueryStateObject(Type t, int controlID){}
		internal static int GetPermanentControlID(){}
		private static int Internal_GetHotControl(){}
		private static void Internal_SetHotControl(int value){}
		internal static void UpdateUndoName(){}
		public static void ExitGUI(){}
		internal static void SetDidGUIWindowsEatLastEvent(bool value){}
		private static System.Object Internal_LoadSkin(int skinNo, Type type){}
		internal static GUISkin GetDefaultSkin(){}
		internal static GUISkin GetBuiltinSkin(int skin){}
		internal static void BeginGUI(int skinMode, int instanceID, int useGUILayout){}
		private static void Internal_ExitGUI(){}
		internal static void EndGUI(int layoutType){}
		internal static bool EndGUIFromException(Exception exception){}
		internal static void CheckOnGUI(){}
		internal static int Internal_GetGUIDepth(){}
		public static Vector2 GUIToScreenPoint(Vector2 guiPoint){}
		internal static Rect GUIToScreenRect(Rect guiRect){}
		public static Vector2 ScreenToGUIPoint(Vector2 screenPoint){}
		public static void RotateAroundPivot(float angle, Vector2 pivotPoint){}
		public static void ScaleAroundPivot(Vector2 scale, Vector2 pivotPoint){}
		public GUIUtility(){}
		private static GUIUtility(){}
		public static int hotControl{ get	{} set	{} }
		public static int keyboardControl{ get	{} set	{} }
		string systemCopyBuffer{ get	{} set	{} }
		bool mouseUsed{ get	{} set	{} }
		bool textFieldInput{ get	{} set	{} }
		private static int s_SkinMode;
		internal static int s_OriginalID;
		internal static Vector2 s_EditorScreenPointOffset;
		internal static bool s_HasKeyboardFocus;
	}

	public sealed class	Handheld: Object
	{
		public static bool PlayFullScreenMovie(string path, Color bgColor, FullScreenMovieControlMode controlMode, FullScreenMovieScalingMode scalingMode){}
		public static bool PlayFullScreenMovie(string path, Color bgColor, FullScreenMovieControlMode controlMode){}
		public static bool PlayFullScreenMovie(string path, Color bgColor){}
		public static bool PlayFullScreenMovie(string path){}
		private static bool INTERNAL_CALL_PlayFullScreenMovie(string path, ref Color bgColor, FullScreenMovieControlMode controlMode, FullScreenMovieScalingMode scalingMode){}
		public static void Vibrate(){}
		internal static void SetActivityIndicatorStyleImpl(int style){}
		public static void SetActivityIndicatorStyle(iOSActivityIndicatorStyle style){}
		public static void SetActivityIndicatorStyle(AndroidActivityIndicatorStyle style){}
		public static int GetActivityIndicatorStyle(){}
		public static void StartActivityIndicator(){}
		public static void StopActivityIndicator(){}
		public Handheld(){}
		public static bool use32BitDisplayBuffer{ get	{} set	{} }
	}

	public sealed class	TouchScreenKeyboard: Object
	{
		private void Destroy(){}
		protected virtual void Finalize(){}
		private void TouchScreenKeyboard_InternalConstructorHelper(TouchScreenKeyboard_InternalConstructorHelperArguments arguments){}
		public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline, bool secure, bool alert){}
		public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline, bool secure){}
		public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline){}
		public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection){}
		public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType){}
		public static TouchScreenKeyboard Open(string text){}
		public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline, bool secure, bool alert, string textPlaceholder){}
		public TouchScreenKeyboard(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline, bool secure, bool alert, string textPlaceholder){}
		public string text{ get	{} set	{} }
		public static bool hideInput{ get	{} set	{} }
		public bool active{ get	{} set	{} }
		public bool done{ get	{} }
		public bool wasCanceled{ get	{} }
		public static bool autorotateToPortrait{ get	{} set	{} }
		public static bool autorotateToPortraitUpsideDown{ get	{} set	{} }
		public static bool autorotateToLandscapeLeft{ get	{} set	{} }
		public static bool autorotateToLandscapeRight{ get	{} set	{} }
		public static Rect area{ get	{} }
		public static bool visible{ get	{} }
		private IntPtr keyboardWrapper;
	}

	public sealed class	iPhone: Object
	{
		public static void SetNoBackupFlag(string path){}
		public static void ResetNoBackupFlag(string path){}
		public iPhone(){}
		public static iPhoneGeneration generation{ get	{} }
	}

	public sealed class	GUISettings: Object
	{
		private static float Internal_GetCursorFlashSpeed(){}
		public GUISettings(){}
		public bool doubleClickSelectsWord{ get	{} set	{} }
		public bool tripleClickSelectsLine{ get	{} set	{} }
		public Color cursorColor{ get	{} set	{} }
		public float cursorFlashSpeed{ get	{} set	{} }
		public Color selectionColor{ get	{} set	{} }
		private bool m_DoubleClickSelectsWord;
		private bool m_TripleClickSelectsLine;
		private Color m_CursorColor;
		private float m_CursorFlashSpeed;
		private Color m_SelectionColor;
	}

	public sealed class	GUISkin: ScriptableObject
	{
		internal void Apply(){}
		private void BuildStyleCache(){}
		public GUIStyle GetStyle(string styleName){}
		public GUIStyle FindStyle(string styleName){}
		internal void MakeCurrent(){}
		private void OnValidate(){}
		public IEnumerator GetEnumerator(){}
		public GUISkin(){}
		public Font font{ get	{} set	{} }
		public GUIStyle box{ get	{} set	{} }
		public GUIStyle label{ get	{} set	{} }
		public GUIStyle textField{ get	{} set	{} }
		public GUIStyle textArea{ get	{} set	{} }
		public GUIStyle button{ get	{} set	{} }
		public GUIStyle toggle{ get	{} set	{} }
		public GUIStyle window{ get	{} set	{} }
		public GUIStyle horizontalSlider{ get	{} set	{} }
		public GUIStyle horizontalSliderThumb{ get	{} set	{} }
		public GUIStyle verticalSlider{ get	{} set	{} }
		public GUIStyle verticalSliderThumb{ get	{} set	{} }
		public GUIStyle horizontalScrollbar{ get	{} set	{} }
		public GUIStyle horizontalScrollbarThumb{ get	{} set	{} }
		public GUIStyle horizontalScrollbarLeftButton{ get	{} set	{} }
		public GUIStyle horizontalScrollbarRightButton{ get	{} set	{} }
		public GUIStyle verticalScrollbar{ get	{} set	{} }
		public GUIStyle verticalScrollbarThumb{ get	{} set	{} }
		public GUIStyle verticalScrollbarUpButton{ get	{} set	{} }
		public GUIStyle verticalScrollbarDownButton{ get	{} set	{} }
		public GUIStyle scrollView{ get	{} set	{} }
		public GUIStyle[] customStyles{ get	{} set	{} }
		public GUISettings settings{ get	{} }
		GUIStyle error{ get	{} }
		private Font m_Font;
		private GUIStyle m_box;
		private GUIStyle m_button;
		private GUIStyle m_toggle;
		private GUIStyle m_label;
		private GUIStyle m_textField;
		private GUIStyle m_textArea;
		private GUIStyle m_window;
		private GUIStyle m_horizontalSlider;
		private GUIStyle m_horizontalSliderThumb;
		private GUIStyle m_verticalSlider;
		private GUIStyle m_verticalSliderThumb;
		private GUIStyle m_horizontalScrollbar;
		private GUIStyle m_horizontalScrollbarThumb;
		private GUIStyle m_horizontalScrollbarLeftButton;
		private GUIStyle m_horizontalScrollbarRightButton;
		private GUIStyle m_verticalScrollbar;
		private GUIStyle m_verticalScrollbarThumb;
		private GUIStyle m_verticalScrollbarUpButton;
		private GUIStyle m_verticalScrollbarDownButton;
		private GUIStyle m_ScrollView;
		internal GUIStyle[] m_CustomStyles;
		private GUISettings m_Settings;
		private Hashtable styles;
		internal static GUIStyle ms_Error;
		internal static SkinChangedDelegate m_SkinChanged;
		internal static GUISkin current;
	}

	public sealed class	GUIContent: Object
	{
		internal static GUIContent Temp(string t){}
		internal static GUIContent Temp(Texture i){}
		internal static GUIContent Temp(string t, Texture i){}
		internal static GUIContent[] Temp(String[] texts){}
		internal static GUIContent[] Temp(Texture[] images){}
		public GUIContent(){}
		public GUIContent(string text){}
		public GUIContent(Texture image){}
		public GUIContent(string text, Texture image){}
		public GUIContent(string text, string tooltip){}
		public GUIContent(Texture image, string tooltip){}
		public GUIContent(string text, Texture image, string tooltip){}
		public GUIContent(GUIContent src){}
		private static GUIContent(){}
		public string text{ get	{} set	{} }
		public Texture image{ get	{} set	{} }
		public string tooltip{ get	{} set	{} }
		int hash{ get	{} }
		private string m_Text;
		private Texture m_Image;
		private string m_Tooltip;
		public static GUIContent none;
		private static GUIContent s_Text;
		private static GUIContent s_Image;
		private static GUIContent s_TextImage;
	}

	public sealed class	GUIStyleState: Object
	{
		protected virtual void Finalize(){}
		private void Init(){}
		private void Cleanup(){}
		private void INTERNAL_get_textColor(out Color value){}
		private void INTERNAL_set_textColor(ref Color value){}
		public GUIStyleState(){}
		internal GUIStyleState(GUIStyle sourceStyle, IntPtr source){}
		public Texture2D background{ get	{} set	{} }
		public Color textColor{ get	{} set	{} }
		internal IntPtr m_Ptr;
		private GUIStyle m_SourceStyle;
	}

	public sealed class	RectOffset: Object
	{
		protected virtual void Finalize(){}
		private void Init(){}
		private void Cleanup(){}
		public Rect Add(Rect rect){}
		private static Rect INTERNAL_CALL_Add(RectOffset self, ref Rect rect){}
		public Rect Remove(Rect rect){}
		private static Rect INTERNAL_CALL_Remove(RectOffset self, ref Rect rect){}
		public virtual string ToString(){}
		public RectOffset(){}
		internal RectOffset(GUIStyle sourceStyle, IntPtr source){}
		public RectOffset(int left, int right, int top, int bottom){}
		public int left{ get	{} set	{} }
		public int right{ get	{} set	{} }
		public int top{ get	{} set	{} }
		public int bottom{ get	{} set	{} }
		public int horizontal{ get	{} }
		public int vertical{ get	{} }
		internal IntPtr m_Ptr;
		private GUIStyle m_SourceStyle;
	}

	public sealed class	GUIStyle: Object
	{
		private static void Internal_Draw(IntPtr target, Rect position, GUIContent content, bool isHover, bool isActive, bool on, bool hasKeyboardFocus){}
		private static void Internal_Draw(GUIContent content, ref Internal_DrawArguments arguments){}
		public void Draw(Rect position, bool isHover, bool isActive, bool on, bool hasKeyboardFocus){}
		public void Draw(Rect position, string text, bool isHover, bool isActive, bool on, bool hasKeyboardFocus){}
		public void Draw(Rect position, Texture image, bool isHover, bool isActive, bool on, bool hasKeyboardFocus){}
		public void Draw(Rect position, GUIContent content, bool isHover, bool isActive, bool on, bool hasKeyboardFocus){}
		public void Draw(Rect position, GUIContent content, int controlID){}
		public void Draw(Rect position, GUIContent content, int controlID, bool on){}
		private static void Internal_Draw2(IntPtr style, Rect position, GUIContent content, int controlID, bool on){}
		private static void INTERNAL_CALL_Internal_Draw2(IntPtr style, ref Rect position, GUIContent content, int controlID, bool on){}
		private static float Internal_GetCursorFlashOffset(){}
		private static void Internal_DrawCursor(IntPtr target, Rect position, GUIContent content, int pos, Color cursorColor){}
		private static void INTERNAL_CALL_Internal_DrawCursor(IntPtr target, ref Rect position, GUIContent content, int pos, ref Color cursorColor){}
		public void DrawCursor(Rect position, GUIContent content, int controlID, int Character){}
		private static void Internal_DrawWithTextSelection(GUIContent content, ref Internal_DrawWithTextSelectionArguments arguments){}
		internal void DrawWithTextSelection(Rect position, GUIContent content, int controlID, int firstSelectedCharacter, int lastSelectedCharacter, bool drawSelectionAsComposition){}
		public void DrawWithTextSelection(Rect position, GUIContent content, int controlID, int firstSelectedCharacter, int lastSelectedCharacter){}
		internal static void SetDefaultFont(Font font){}
		public Vector2 GetCursorPixelPosition(Rect position, GUIContent content, int cursorStringIndex){}
		internal static void Internal_GetCursorPixelPosition(IntPtr target, Rect position, GUIContent content, int cursorStringIndex, out Vector2 ret){}
		private static void INTERNAL_CALL_Internal_GetCursorPixelPosition(IntPtr target, ref Rect position, GUIContent content, int cursorStringIndex, out Vector2 ret){}
		public int GetCursorStringIndex(Rect position, GUIContent content, Vector2 cursorPixelPosition){}
		internal static int Internal_GetCursorStringIndex(IntPtr target, Rect position, GUIContent content, Vector2 cursorPixelPosition){}
		private static int INTERNAL_CALL_Internal_GetCursorStringIndex(IntPtr target, ref Rect position, GUIContent content, ref Vector2 cursorPixelPosition){}
		internal int GetNumCharactersThatFitWithinWidth(string text, float width){}
		internal static int Internal_GetNumCharactersThatFitWithinWidth(IntPtr target, string text, float width){}
		public Vector2 CalcSize(GUIContent content){}
		internal static void Internal_CalcSize(IntPtr target, GUIContent content, out Vector2 ret){}
		public Vector2 CalcScreenSize(Vector2 contentSize){}
		public float CalcHeight(GUIContent content, float width){}
		private static float Internal_CalcHeight(IntPtr target, GUIContent content, float width){}
		public void CalcMinMaxWidth(GUIContent content, out float minWidth, out float maxWidth){}
		private static void Internal_CalcMinMaxWidth(IntPtr target, GUIContent content, out float minWidth, out float maxWidth){}
		public virtual string ToString(){}
		protected virtual void Finalize(){}
		private void Init(){}
		private void InitCopy(GUIStyle other){}
		private void Cleanup(){}
		private IntPtr GetStyleStatePtr(int idx){}
		private void AssignStyleState(int idx, IntPtr srcStyleState){}
		private IntPtr GetRectOffsetPtr(int idx){}
		private void AssignRectOffset(int idx, IntPtr srcRectOffset){}
		private void INTERNAL_get_contentOffset(out Vector2 value){}
		private void INTERNAL_set_contentOffset(ref Vector2 value){}
		private void INTERNAL_get_Internal_clipOffset(out Vector2 value){}
		private void INTERNAL_set_Internal_clipOffset(ref Vector2 value){}
		private static float Internal_GetLineHeight(IntPtr target){}
		public GUIStyle(){}
		public GUIStyle(GUIStyle other){}
		private static GUIStyle(){}
		public string name{ get	{} set	{} }
		public GUIStyleState normal{ get	{} set	{} }
		public GUIStyleState hover{ get	{} set	{} }
		public GUIStyleState active{ get	{} set	{} }
		public GUIStyleState onNormal{ get	{} set	{} }
		public GUIStyleState onHover{ get	{} set	{} }
		public GUIStyleState onActive{ get	{} set	{} }
		public GUIStyleState focused{ get	{} set	{} }
		public GUIStyleState onFocused{ get	{} set	{} }
		public RectOffset border{ get	{} set	{} }
		public RectOffset margin{ get	{} set	{} }
		public RectOffset padding{ get	{} set	{} }
		public RectOffset overflow{ get	{} set	{} }
		public ImagePosition imagePosition{ get	{} set	{} }
		public TextAnchor alignment{ get	{} set	{} }
		public bool wordWrap{ get	{} set	{} }
		public TextClipping clipping{ get	{} set	{} }
		public Vector2 contentOffset{ get	{} set	{} }
		public Vector2 clipOffset{ get	{} set	{} }
		Vector2 Internal_clipOffset{ get	{} set	{} }
		public float fixedWidth{ get	{} set	{} }
		public float fixedHeight{ get	{} set	{} }
		public bool stretchWidth{ get	{} set	{} }
		public bool stretchHeight{ get	{} set	{} }
		public Font font{ get	{} set	{} }
		public int fontSize{ get	{} set	{} }
		public FontStyle fontStyle{ get	{} set	{} }
		public bool richText{ get	{} set	{} }
		public float lineHeight{ get	{} }
		public static GUIStyle none{ get	{} }
		public bool isHeightDependantOnWidth{ get	{} }
		internal IntPtr m_Ptr;
		private GUIStyleState m_Normal;
		private GUIStyleState m_Hover;
		private GUIStyleState m_Active;
		private GUIStyleState m_Focused;
		private GUIStyleState m_OnNormal;
		private GUIStyleState m_OnHover;
		private GUIStyleState m_OnActive;
		private GUIStyleState m_OnFocused;
		private RectOffset m_Border;
		private RectOffset m_Padding;
		private RectOffset m_Margin;
		private RectOffset m_Overflow;
		internal static bool showKeyboardFocus;
		private static GUIStyle s_None;
	}

	public sealed class	Event: Object
	{
		private void Init(){}
		protected virtual void Finalize(){}
		private void Cleanup(){}
		private void InitCopy(Event other){}
		private void InitPtr(IntPtr ptr){}
		public EventType GetTypeForControl(int controlID){}
		private void Internal_SetMousePosition(Vector2 value){}
		private static void INTERNAL_CALL_Internal_SetMousePosition(Event self, ref Vector2 value){}
		private void Internal_GetMousePosition(out Vector2 value){}
		private void Internal_SetMouseDelta(Vector2 value){}
		private static void INTERNAL_CALL_Internal_SetMouseDelta(Event self, ref Vector2 value){}
		private void Internal_GetMouseDelta(out Vector2 value){}
		private static void Internal_SetNativeEvent(IntPtr ptr){}
		private static void Internal_MakeMasterEventCurrent(){}
		public void Use(){}
		public static Event KeyboardEvent(string key){}
		public virtual int GetHashCode(){}
		public virtual bool Equals(System.Object obj){}
		public virtual string ToString(){}
		public Event(){}
		public Event(Event other){}
		private Event(IntPtr ptr){}
		public EventType rawType{ get	{} }
		public EventType type{ get	{} set	{} }
		public Vector2 mousePosition{ get	{} set	{} }
		public Vector2 delta{ get	{} set	{} }
		public Ray mouseRay{ get	{} set	{} }
		public int button{ get	{} set	{} }
		public EventModifiers modifiers{ get	{} set	{} }
		public float pressure{ get	{} set	{} }
		public int clickCount{ get	{} set	{} }
		public char character{ get	{} set	{} }
		public string commandName{ get	{} set	{} }
		public KeyCode keyCode{ get	{} set	{} }
		public bool shift{ get	{} set	{} }
		public bool control{ get	{} set	{} }
		public bool alt{ get	{} set	{} }
		public bool command{ get	{} set	{} }
		public bool capsLock{ get	{} set	{} }
		public bool numeric{ get	{} set	{} }
		public bool functionKey{ get	{} }
		public static Event current{ get	{} set	{} }
		public bool isKey{ get	{} }
		public bool isMouse{ get	{} }
		private IntPtr m_Ptr;
		private static Event s_Current;
		private static Event s_MasterEvent;
		private static Dictionary<String, Int32> <>f__switch$map0;
	}

	public sealed class	Gizmos: Object
	{
		public static void DrawRay(Ray r){}
		public static void DrawRay(Vector3 from, Vector3 direction){}
		public static void DrawLine(Vector3 from, Vector3 to){}
		private static void INTERNAL_CALL_DrawLine(ref Vector3 from, ref Vector3 to){}
		public static void DrawWireSphere(Vector3 center, float radius){}
		private static void INTERNAL_CALL_DrawWireSphere(ref Vector3 center, float radius){}
		public static void DrawSphere(Vector3 center, float radius){}
		private static void INTERNAL_CALL_DrawSphere(ref Vector3 center, float radius){}
		public static void DrawWireCube(Vector3 center, Vector3 size){}
		private static void INTERNAL_CALL_DrawWireCube(ref Vector3 center, ref Vector3 size){}
		public static void DrawCube(Vector3 center, Vector3 size){}
		private static void INTERNAL_CALL_DrawCube(ref Vector3 center, ref Vector3 size){}
		public static void DrawIcon(Vector3 center, string name, bool allowScaling){}
		public static void DrawIcon(Vector3 center, string name){}
		private static void INTERNAL_CALL_DrawIcon(ref Vector3 center, string name, bool allowScaling){}
		public static void DrawGUITexture(Rect screenRect, Texture texture){}
		public static void DrawGUITexture(Rect screenRect, Texture texture, Material mat){}
		public static void DrawGUITexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Material mat){}
		public static void DrawGUITexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder){}
		private static void INTERNAL_CALL_DrawGUITexture(ref Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Material mat){}
		private static void INTERNAL_get_color(out Color value){}
		private static void INTERNAL_set_color(ref Color value){}
		private static void INTERNAL_get_matrix(out Matrix4x4 value){}
		private static void INTERNAL_set_matrix(ref Matrix4x4 value){}
		public static void DrawFrustum(Vector3 center, float fov, float maxRange, float minRange, float aspect){}
		private static void INTERNAL_CALL_DrawFrustum(ref Vector3 center, float fov, float maxRange, float minRange, float aspect){}
		public Gizmos(){}
		public static Color color{ get	{} set	{} }
		public static Matrix4x4 matrix{ get	{} set	{} }
	}

	public sealed class	iPhoneInput: Object
	{
		public static iPhoneTouch GetTouch(int index){}
		public static iPhoneAccelerationEvent GetAccelerationEvent(int index){}
		public iPhoneInput(){}
		public static iPhoneAccelerationEvent[] accelerationEvents{ get	{} }
		public static iPhoneTouch[] touches{ get	{} }
		public static int touchCount{ get	{} }
		public static bool multiTouchEnabled{ get	{} set	{} }
		public static int accelerationEventCount{ get	{} }
		public static Vector3 acceleration{ get	{} }
		public static iPhoneOrientation orientation{ get	{} }
		public static LocationInfo lastLocation{ get	{} }
	}

	public sealed class	iPhoneSettings: Object
	{
		public static void StartLocationServiceUpdates(float desiredAccuracyInMeters, float updateDistanceInMeters){}
		public static void StartLocationServiceUpdates(float desiredAccuracyInMeters){}
		public static void StartLocationServiceUpdates(){}
		public static void StopLocationServiceUpdates(){}
		public iPhoneSettings(){}
		public static iPhoneScreenOrientation screenOrientation{ get	{} set	{} }
		public static bool verticalOrientation{ get	{} set	{} }
		public static bool screenCanDarken{ get	{} set	{} }
		public static string uniqueIdentifier{ get	{} }
		public static string name{ get	{} }
		public static string model{ get	{} }
		public static string systemName{ get	{} }
		public static string systemVersion{ get	{} }
		public static iPhoneNetworkReachability internetReachability{ get	{} }
		public static iPhoneGeneration generation{ get	{} }
		public static LocationServiceStatus locationServiceStatus{ get	{} }
		public static bool locationServiceEnabledByUser{ get	{} }
	}

	public sealed class	iPhoneKeyboard: Object
	{
		private void Destroy(){}
		protected virtual void Finalize(){}
		private void iPhoneKeyboard_InternalConstructorHelper(iPhoneKeyboard_InternalConstructorHelperArguments arguments){}
		public static iPhoneKeyboard Open(string text, iPhoneKeyboardType keyboardType, bool autocorrection, bool multiline, bool secure, bool alert){}
		public static iPhoneKeyboard Open(string text, iPhoneKeyboardType keyboardType, bool autocorrection, bool multiline, bool secure){}
		public static iPhoneKeyboard Open(string text, iPhoneKeyboardType keyboardType, bool autocorrection, bool multiline){}
		public static iPhoneKeyboard Open(string text, iPhoneKeyboardType keyboardType, bool autocorrection){}
		public static iPhoneKeyboard Open(string text, iPhoneKeyboardType keyboardType){}
		public static iPhoneKeyboard Open(string text){}
		public static iPhoneKeyboard Open(string text, iPhoneKeyboardType keyboardType, bool autocorrection, bool multiline, bool secure, bool alert, string textPlaceholder){}
		public iPhoneKeyboard(string text, iPhoneKeyboardType keyboardType, bool autocorrection, bool multiline, bool secure, bool alert, string textPlaceholder){}
		public string text{ get	{} set	{} }
		public static bool hideInput{ get	{} set	{} }
		public bool active{ get	{} set	{} }
		public bool done{ get	{} }
		public static bool autorotateToPortrait{ get	{} set	{} }
		public static bool autorotateToPortraitUpsideDown{ get	{} set	{} }
		public static bool autorotateToLandscapeLeft{ get	{} set	{} }
		public static bool autorotateToLandscapeRight{ get	{} set	{} }
		public static Rect area{ get	{} }
		public static bool visible{ get	{} }
		private IntPtr keyboardWrapper;
	}

	public sealed class	iPhoneUtils: Object
	{
		public static void PlayMovie(string path, Color bgColor, int controlMode, int scalingMode){}
		private static void INTERNAL_CALL_PlayMovie(string path, ref Color bgColor, int controlMode, int scalingMode){}
		public static void PlayMovie(string path, Color bgColor, iPhoneMovieControlMode controlMode, iPhoneMovieScalingMode scalingMode){}
		public static void PlayMovie(string path, Color bgColor, iPhoneMovieControlMode controlMode){}
		public static void PlayMovie(string path, Color bgColor){}
		public static void PlayMovieURL(string url, Color bgColor, int controlMode, int scalingMode){}
		private static void INTERNAL_CALL_PlayMovieURL(string url, ref Color bgColor, int controlMode, int scalingMode){}
		public static void PlayMovieURL(string url, Color bgColor, iPhoneMovieControlMode controlMode, iPhoneMovieScalingMode scalingMode){}
		public static void PlayMovieURL(string url, Color bgColor, iPhoneMovieControlMode controlMode){}
		public static void PlayMovieURL(string url, Color bgColor){}
		public static void Vibrate(){}
		public iPhoneUtils(){}
		public static bool isApplicationGenuine{ get	{} }
		public static bool isApplicationGenuineAvailable{ get	{} }
	}

	public sealed class	ADError: Object
	{
		public ADError(){}
		public ADErrorCode code{ get	{} }
		public string description{ get	{} }
		public string reason{ get	{} }
		private ADErrorCode m_Code;
		private string m_Description;
		private string m_Reason;
	}

	public sealed class	ADBannerView: Object
	{
		public void Show(){}
		public void Hide(){}
		private void INTERNAL_get_position(out Vector2 value){}
		private void INTERNAL_set_position(ref Vector2 value){}
		public static Vector2 GetSizeFromSizeIdentifier(ADSizeIdentifier identifier){}
		public void CancelAction(){}
		private void Destroy(){}
		protected virtual void Finalize(){}
		public void InitWrapper(){}
		public ADBannerView(){}
		public ADSizeIdentifier[] requiredSizeIdentifiers{ get	{} set	{} }
		public ADSizeIdentifier currentSizeIdentifier{ get	{} set	{} }
		public bool autoSize{ get	{} set	{} }
		public Vector2 position{ get	{} set	{} }
		public ADPosition autoPosition{ get	{} set	{} }
		public bool visible{ get	{} }
		public bool loaded{ get	{} }
		public bool actionInProgress{ get	{} }
		public ADError error{ get	{} }
		private IntPtr iAdWrapper;
	}

	public sealed class	ADInterstitialAd: Object
	{
		public void CancelAction(){}
		public bool Present(){}
		private void Destroy(){}
		protected virtual void Finalize(){}
		private void InitWrapper(){}
		public ADInterstitialAd(){}
		public bool loaded{ get	{} }
		public bool actionInProgress{ get	{} }
		public bool visible{ get	{} }
		public ADError error{ get	{} }
		private IntPtr iAdWrapper;
	}

	public sealed class	LocalNotification: Object
	{
		private double GetFireDate(){}
		private void SetFireDate(double dt){}
		private void Destroy(){}
		protected virtual void Finalize(){}
		private void InitWrapper(){}
		public LocalNotification(){}
		private static LocalNotification(){}
		public DateTime fireDate{ get	{} set	{} }
		public string timeZone{ get	{} set	{} }
		public CalendarUnit repeatInterval{ get	{} set	{} }
		public CalendarIdentifier repeatCalendar{ get	{} set	{} }
		public string alertBody{ get	{} set	{} }
		public string alertAction{ get	{} set	{} }
		public bool hasAction{ get	{} set	{} }
		public string alertLaunchImage{ get	{} set	{} }
		public int applicationIconBadgeNumber{ get	{} set	{} }
		public string soundName{ get	{} set	{} }
		public static string defaultSoundName{ get	{} }
		public IDictionary userInfo{ get	{} set	{} }
		private IntPtr notificationWrapper;
		private static long m_NSReferenceDateTicks;
	}

	public sealed class	RemoteNotification: Object
	{
		private void Destroy(){}
		protected virtual void Finalize(){}
		private RemoteNotification(){}
		public string alertBody{ get	{} }
		public bool hasAction{ get	{} }
		public int applicationIconBadgeNumber{ get	{} }
		public string soundName{ get	{} }
		public IDictionary userInfo{ get	{} }
		private IntPtr notificationWrapper;
	}

	public sealed class	NotificationServices: Object
	{
		public static LocalNotification GetLocalNotification(int index){}
		public static void ScheduleLocalNotification(LocalNotification notification){}
		public static void PresentLocalNotificationNow(LocalNotification notification){}
		public static void CancelLocalNotification(LocalNotification notification){}
		public static void CancelAllLocalNotifications(){}
		public static RemoteNotification GetRemoteNotification(int index){}
		public static void ClearLocalNotifications(){}
		public static void ClearRemoteNotifications(){}
		public static void RegisterForRemoteNotificationTypes(RemoteNotificationType notificationTypes){}
		public static void UnregisterForRemoteNotifications(){}
		public NotificationServices(){}
		public static int localNotificationCount{ get	{} }
		public static LocalNotification[] localNotifications{ get	{} }
		public static LocalNotification[] scheduledLocalNotifications{ get	{} }
		public static int remoteNotificationCount{ get	{} }
		public static RemoteNotification[] remoteNotifications{ get	{} }
		public static RemoteNotificationType enabledRemoteNotificationTypes{ get	{} }
		public static Byte[] deviceToken{ get	{} }
		public static string registrationError{ get	{} }
	}

	public sealed class	LightProbeGroup: Component
	{
		public LightProbeGroup(){}
		public Vector3[] probePositions{ get	{} set	{} }
	}

	public sealed class	Ping: Object
	{
		public void DestroyPing(){}
		protected virtual void Finalize(){}
		public Ping(string address){}
		public bool isDone{ get	{} }
		public int time{ get	{} }
		public string ip{ get	{} }
		private IntPtr pingWrapper;
	}

	public sealed class	NetworkView: Behaviour
	{
		private static void Internal_RPC(NetworkView view, string name, RPCMode mode, Object[] args){}
		private static void Internal_RPC_Target(NetworkView view, string name, NetworkPlayer target, Object[] args){}
		public void RPC(string name, RPCMode mode, Object[] args){}
		public void RPC(string name, NetworkPlayer target, Object[] args){}
		private void Internal_GetViewID(out NetworkViewID viewID){}
		private void Internal_SetViewID(NetworkViewID viewID){}
		private static void INTERNAL_CALL_Internal_SetViewID(NetworkView self, ref NetworkViewID viewID){}
		public bool SetScope(NetworkPlayer player, bool relevancy){}
		public static NetworkView Find(NetworkViewID viewID){}
		private static NetworkView INTERNAL_CALL_Find(ref NetworkViewID viewID){}
		public NetworkView(){}
		public Component observed{ get	{} set	{} }
		public NetworkStateSynchronization stateSynchronization{ get	{} set	{} }
		public NetworkViewID viewID{ get	{} set	{} }
		public int group{ get	{} set	{} }
		public bool isMine{ get	{} }
		public NetworkPlayer owner{ get	{} }
	}

	public sealed class	Network: Object
	{
		public static bool HavePublicAddress(){}
		public static NetworkConnectionError InitializeServer(int connections, int listenPort, bool useNat){}
		private static NetworkConnectionError Internal_InitializeServerDeprecated(int connections, int listenPort){}
		public static NetworkConnectionError InitializeServer(int connections, int listenPort){}
		public static void InitializeSecurity(){}
		private static NetworkConnectionError Internal_ConnectToSingleIP(string IP, int remotePort, int localPort, string password){}
		private static NetworkConnectionError Internal_ConnectToSingleIP(string IP, int remotePort, int localPort){}
		private static NetworkConnectionError Internal_ConnectToGuid(string guid, string password){}
		private static NetworkConnectionError Internal_ConnectToIPs(String[] IP, int remotePort, int localPort, string password){}
		private static NetworkConnectionError Internal_ConnectToIPs(String[] IP, int remotePort, int localPort){}
		public static NetworkConnectionError Connect(string IP, int remotePort){}
		public static NetworkConnectionError Connect(string IP, int remotePort, string password){}
		public static NetworkConnectionError Connect(String[] IPs, int remotePort){}
		public static NetworkConnectionError Connect(String[] IPs, int remotePort, string password){}
		public static NetworkConnectionError Connect(string GUID){}
		public static NetworkConnectionError Connect(string GUID, string password){}
		public static NetworkConnectionError Connect(HostData hostData){}
		public static NetworkConnectionError Connect(HostData hostData, string password){}
		public static void Disconnect(int timeout){}
		public static void Disconnect(){}
		public static void CloseConnection(NetworkPlayer target, bool sendDisconnectionNotification){}
		private static int Internal_GetPlayer(){}
		private static void Internal_AllocateViewID(out NetworkViewID viewID){}
		public static NetworkViewID AllocateViewID(){}
		public static System.Object Instantiate(System.Object prefab, Vector3 position, Quaternion rotation, int group){}
		private static System.Object INTERNAL_CALL_Instantiate(System.Object prefab, ref Vector3 position, ref Quaternion rotation, int group){}
		public static void Destroy(NetworkViewID viewID){}
		private static void INTERNAL_CALL_Destroy(ref NetworkViewID viewID){}
		public static void Destroy(GameObject gameObject){}
		public static void DestroyPlayerObjects(NetworkPlayer playerID){}
		private static void Internal_RemoveRPCs(NetworkPlayer playerID, NetworkViewID viewID, uint channelMask){}
		private static void INTERNAL_CALL_Internal_RemoveRPCs(NetworkPlayer playerID, ref NetworkViewID viewID, uint channelMask){}
		public static void RemoveRPCs(NetworkPlayer playerID){}
		public static void RemoveRPCs(NetworkPlayer playerID, int group){}
		public static void RemoveRPCs(NetworkViewID viewID){}
		public static void RemoveRPCsInGroup(int group){}
		public static void SetLevelPrefix(int prefix){}
		public static int GetLastPing(NetworkPlayer player){}
		public static int GetAveragePing(NetworkPlayer player){}
		public static void SetReceivingEnabled(NetworkPlayer player, int group, bool enabled){}
		private static void Internal_SetSendingGlobal(int group, bool enabled){}
		private static void Internal_SetSendingSpecific(NetworkPlayer player, int group, bool enabled){}
		public static void SetSendingEnabled(int group, bool enabled){}
		public static void SetSendingEnabled(NetworkPlayer player, int group, bool enabled){}
		private static void Internal_GetTime(out double t){}
		public static ConnectionTesterStatus TestConnection(bool forceTest){}
		public static ConnectionTesterStatus TestConnection(){}
		public static ConnectionTesterStatus TestConnectionNAT(bool forceTest){}
		public static ConnectionTesterStatus TestConnectionNAT(){}
		public Network(){}
		public static string incomingPassword{ get	{} set	{} }
		public static NetworkLogLevel logLevel{ get	{} set	{} }
		public static NetworkPlayer[] connections{ get	{} }
		public static NetworkPlayer player{ get	{} }
		public static bool isClient{ get	{} }
		public static bool isServer{ get	{} }
		public static NetworkPeerType peerType{ get	{} }
		public static float sendRate{ get	{} set	{} }
		public static bool isMessageQueueRunning{ get	{} set	{} }
		public static double time{ get	{} }
		public static int minimumAllocatableViewIDs{ get	{} set	{} }
		public static bool useNat{ get	{} set	{} }
		public static string natFacilitatorIP{ get	{} set	{} }
		public static int natFacilitatorPort{ get	{} set	{} }
		public static string connectionTesterIP{ get	{} set	{} }
		public static int connectionTesterPort{ get	{} set	{} }
		public static int maxConnections{ get	{} set	{} }
		public static string proxyIP{ get	{} set	{} }
		public static int proxyPort{ get	{} set	{} }
		public static bool useProxy{ get	{} set	{} }
		public static string proxyPassword{ get	{} set	{} }
	}

	public sealed class	BitStream: Object
	{
		private void Serializeb(ref int value){}
		private void Serializec(ref char value){}
		private void Serializes(ref short value){}
		private void Serializei(ref int value){}
		private void Serializef(ref float value, float maximumDelta){}
		private void Serializeq(ref Quaternion value, float maximumDelta){}
		private static void INTERNAL_CALL_Serializeq(BitStream self, ref Quaternion value, float maximumDelta){}
		private void Serializev(ref Vector3 value, float maximumDelta){}
		private static void INTERNAL_CALL_Serializev(BitStream self, ref Vector3 value, float maximumDelta){}
		private void Serializen(ref NetworkViewID viewID){}
		private static void INTERNAL_CALL_Serializen(BitStream self, ref NetworkViewID viewID){}
		public void Serialize(ref bool value){}
		public void Serialize(ref char value){}
		public void Serialize(ref short value){}
		public void Serialize(ref int value){}
		public void Serialize(ref float value){}
		public void Serialize(ref float value, float maxDelta){}
		public void Serialize(ref Quaternion value){}
		public void Serialize(ref Quaternion value, float maxDelta){}
		public void Serialize(ref Vector3 value){}
		public void Serialize(ref Vector3 value, float maxDelta){}
		public void Serialize(ref NetworkPlayer value){}
		public void Serialize(ref NetworkViewID viewID){}
		private void Serialize(ref string value){}
		public BitStream(){}
		public bool isReading{ get	{} }
		public bool isWriting{ get	{} }
		private IntPtr m_Ptr;
	}

	public sealed class	RPC: Attribute, _Attribute
	{
		public RPC(){}
	}

	public sealed class	HostData: Object
	{
		public HostData(){}
		public bool useNat{ get	{} set	{} }
		public string gameType{ get	{} set	{} }
		public string gameName{ get	{} set	{} }
		public int connectedPlayers{ get	{} set	{} }
		public int playerLimit{ get	{} set	{} }
		public String[] ip{ get	{} set	{} }
		public int port{ get	{} set	{} }
		public bool passwordProtected{ get	{} set	{} }
		public string comment{ get	{} set	{} }
		public string guid{ get	{} set	{} }
		private int m_Nat;
		private string m_GameType;
		private string m_GameName;
		private int m_ConnectedPlayers;
		private int m_PlayerLimit;
		private String[] m_IP;
		private int m_Port;
		private int m_PasswordProtected;
		private string m_Comment;
		private string m_GUID;
	}

	public sealed class	MasterServer: Object
	{
		public static void RequestHostList(string gameTypeName){}
		public static HostData[] PollHostList(){}
		public static void RegisterHost(string gameTypeName, string gameName, string comment){}
		public static void RegisterHost(string gameTypeName, string gameName){}
		public static void UnregisterHost(){}
		public static void ClearHostList(){}
		public MasterServer(){}
		public static string ipAddress{ get	{} set	{} }
		public static int port{ get	{} set	{} }
		public static int updateRate{ get	{} set	{} }
		public static bool dedicatedServer{ get	{} set	{} }
	}

	public class	Physics: Object
	{
		private static bool INTERNAL_CALL_CheckSphere(ref Vector3 position, float radius, int layerMask){}
		public static bool CheckCapsule(Vector3 start, Vector3 end, float radius, int layermask){}
		public static bool CheckCapsule(Vector3 start, Vector3 end, float radius){}
		private static bool INTERNAL_CALL_CheckCapsule(ref Vector3 start, ref Vector3 end, float radius, int layermask){}
		public static void IgnoreCollision(Collider collider1, Collider collider2, bool ignore){}
		public static void IgnoreCollision(Collider collider1, Collider collider2){}
		public static void IgnoreLayerCollision(int layer1, int layer2, bool ignore){}
		public static void IgnoreLayerCollision(int layer1, int layer2){}
		public static bool GetIgnoreLayerCollision(int layer1, int layer2){}
		private static void INTERNAL_get_gravity(out Vector3 value){}
		private static void INTERNAL_set_gravity(ref Vector3 value){}
		private static bool Internal_Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float distance, int layermask){}
		private static bool INTERNAL_CALL_Internal_Raycast(ref Vector3 origin, ref Vector3 direction, out RaycastHit hitInfo, float distance, int layermask){}
		private static bool Internal_CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo, float distance, int layermask){}
		private static bool INTERNAL_CALL_Internal_CapsuleCast(ref Vector3 point1, ref Vector3 point2, float radius, ref Vector3 direction, out RaycastHit hitInfo, float distance, int layermask){}
		private static bool Internal_RaycastTest(Vector3 origin, Vector3 direction, float distance, int layermask){}
		private static bool INTERNAL_CALL_Internal_RaycastTest(ref Vector3 origin, ref Vector3 direction, float distance, int layermask){}
		public static bool Raycast(Vector3 origin, Vector3 direction, float distance){}
		public static bool Raycast(Vector3 origin, Vector3 direction){}
		public static bool Raycast(Vector3 origin, Vector3 direction, float distance, int layerMask){}
		public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float distance){}
		public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo){}
		public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float distance, int layerMask){}
		public static bool Raycast(Ray ray, float distance){}
		public static bool Raycast(Ray ray){}
		public static bool Raycast(Ray ray, float distance, int layerMask){}
		public static bool Raycast(Ray ray, out RaycastHit hitInfo, float distance){}
		public static bool Raycast(Ray ray, out RaycastHit hitInfo){}
		public static bool Raycast(Ray ray, out RaycastHit hitInfo, float distance, int layerMask){}
		public static RaycastHit[] RaycastAll(Ray ray, float distance){}
		public static RaycastHit[] RaycastAll(Ray ray){}
		public static RaycastHit[] RaycastAll(Ray ray, float distance, int layerMask){}
		public static RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction, float distance, int layermask){}
		public static RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction, float distance){}
		public static RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction){}
		private static RaycastHit[] INTERNAL_CALL_RaycastAll(ref Vector3 origin, ref Vector3 direction, float distance, int layermask){}
		public static bool Linecast(Vector3 start, Vector3 end){}
		public static bool Linecast(Vector3 start, Vector3 end, int layerMask){}
		public static bool Linecast(Vector3 start, Vector3 end, out RaycastHit hitInfo){}
		public static bool Linecast(Vector3 start, Vector3 end, out RaycastHit hitInfo, int layerMask){}
		public static Collider[] OverlapSphere(Vector3 position, float radius, int layerMask){}
		public static Collider[] OverlapSphere(Vector3 position, float radius){}
		private static Collider[] INTERNAL_CALL_OverlapSphere(ref Vector3 position, float radius, int layerMask){}
		public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float distance){}
		public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction){}
		public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float distance, int layerMask){}
		public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo, float distance){}
		public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo){}
		public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo, float distance, int layerMask){}
		public static bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo, float distance){}
		public static bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo){}
		public static bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo, float distance, int layerMask){}
		public static bool SphereCast(Ray ray, float radius, float distance){}
		public static bool SphereCast(Ray ray, float radius){}
		public static bool SphereCast(Ray ray, float radius, float distance, int layerMask){}
		public static bool SphereCast(Ray ray, float radius, out RaycastHit hitInfo, float distance){}
		public static bool SphereCast(Ray ray, float radius, out RaycastHit hitInfo){}
		public static bool SphereCast(Ray ray, float radius, out RaycastHit hitInfo, float distance, int layerMask){}
		public static RaycastHit[] CapsuleCastAll(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float distance, int layermask){}
		public static RaycastHit[] CapsuleCastAll(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float distance){}
		public static RaycastHit[] CapsuleCastAll(Vector3 point1, Vector3 point2, float radius, Vector3 direction){}
		private static RaycastHit[] INTERNAL_CALL_CapsuleCastAll(ref Vector3 point1, ref Vector3 point2, float radius, ref Vector3 direction, float distance, int layermask){}
		public static RaycastHit[] SphereCastAll(Vector3 origin, float radius, Vector3 direction, float distance){}
		public static RaycastHit[] SphereCastAll(Vector3 origin, float radius, Vector3 direction){}
		public static RaycastHit[] SphereCastAll(Vector3 origin, float radius, Vector3 direction, float distance, int layerMask){}
		public static RaycastHit[] SphereCastAll(Ray ray, float radius, float distance){}
		public static RaycastHit[] SphereCastAll(Ray ray, float radius){}
		public static RaycastHit[] SphereCastAll(Ray ray, float radius, float distance, int layerMask){}
		public static bool CheckSphere(Vector3 position, float radius, int layerMask){}
		public static bool CheckSphere(Vector3 position, float radius){}
		public Physics(){}
		public static Vector3 gravity{ get	{} set	{} }
		public static float minPenetrationForPenalty{ get	{} set	{} }
		public static float bounceThreshold{ get	{} set	{} }
		public static float bounceTreshold{ get	{} set	{} }
		public static float sleepVelocity{ get	{} set	{} }
		public static float sleepAngularVelocity{ get	{} set	{} }
		public static float maxAngularVelocity{ get	{} set	{} }
		public static int solverIterationCount{ get	{} set	{} }
		public static float penetrationPenaltyForce{ get	{} set	{} }
		public const int kIgnoreRaycastLayer = null;
		public const int kDefaultRaycastLayers = null;
		public const int kAllLayers = null;
	}

	public sealed class	Rigidbody: Component
	{
		private void INTERNAL_get_position(out Vector3 value){}
		private void INTERNAL_set_position(ref Vector3 value){}
		private void INTERNAL_get_rotation(out Quaternion value){}
		private void INTERNAL_set_rotation(ref Quaternion value){}
		public void MovePosition(Vector3 position){}
		private static void INTERNAL_CALL_MovePosition(Rigidbody self, ref Vector3 position){}
		public void MoveRotation(Quaternion rot){}
		private static void INTERNAL_CALL_MoveRotation(Rigidbody self, ref Quaternion rot){}
		public void Sleep(){}
		private static void INTERNAL_CALL_Sleep(Rigidbody self){}
		public bool IsSleeping(){}
		private static bool INTERNAL_CALL_IsSleeping(Rigidbody self){}
		public void WakeUp(){}
		private static void INTERNAL_CALL_WakeUp(Rigidbody self){}
		public bool SweepTest(Vector3 direction, out RaycastHit hitInfo, float distance){}
		public bool SweepTest(Vector3 direction, out RaycastHit hitInfo){}
		private static bool INTERNAL_CALL_SweepTest(Rigidbody self, ref Vector3 direction, out RaycastHit hitInfo, float distance){}
		public RaycastHit[] SweepTestAll(Vector3 direction, float distance){}
		public RaycastHit[] SweepTestAll(Vector3 direction){}
		private static RaycastHit[] INTERNAL_CALL_SweepTestAll(Rigidbody self, ref Vector3 direction, float distance){}
		public void SetMaxAngularVelocity(float a){}
		private void INTERNAL_get_velocity(out Vector3 value){}
		private void INTERNAL_set_velocity(ref Vector3 value){}
		private void INTERNAL_get_angularVelocity(out Vector3 value){}
		private void INTERNAL_set_angularVelocity(ref Vector3 value){}
		public void SetDensity(float density){}
		private static void INTERNAL_CALL_SetDensity(Rigidbody self, float density){}
		public void AddForce(Vector3 force, ForceMode mode){}
		public void AddForce(Vector3 force){}
		private static void INTERNAL_CALL_AddForce(Rigidbody self, ref Vector3 force, ForceMode mode){}
		public void AddForce(float x, float y, float z){}
		public void AddForce(float x, float y, float z, ForceMode mode){}
		public void AddRelativeForce(Vector3 force, ForceMode mode){}
		public void AddRelativeForce(Vector3 force){}
		private static void INTERNAL_CALL_AddRelativeForce(Rigidbody self, ref Vector3 force, ForceMode mode){}
		public void AddRelativeForce(float x, float y, float z){}
		public void AddRelativeForce(float x, float y, float z, ForceMode mode){}
		public void AddTorque(Vector3 torque, ForceMode mode){}
		public void AddTorque(Vector3 torque){}
		private static void INTERNAL_CALL_AddTorque(Rigidbody self, ref Vector3 torque, ForceMode mode){}
		public void AddTorque(float x, float y, float z){}
		public void AddTorque(float x, float y, float z, ForceMode mode){}
		public void AddRelativeTorque(Vector3 torque, ForceMode mode){}
		public void AddRelativeTorque(Vector3 torque){}
		private static void INTERNAL_CALL_AddRelativeTorque(Rigidbody self, ref Vector3 torque, ForceMode mode){}
		public void AddRelativeTorque(float x, float y, float z){}
		public void AddRelativeTorque(float x, float y, float z, ForceMode mode){}
		public void AddForceAtPosition(Vector3 force, Vector3 position, ForceMode mode){}
		public void AddForceAtPosition(Vector3 force, Vector3 position){}
		private static void INTERNAL_CALL_AddForceAtPosition(Rigidbody self, ref Vector3 force, ref Vector3 position, ForceMode mode){}
		public void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius, float upwardsModifier, ForceMode mode){}
		public void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius, float upwardsModifier){}
		public void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius){}
		private static void INTERNAL_CALL_AddExplosionForce(Rigidbody self, float explosionForce, ref Vector3 explosionPosition, float explosionRadius, float upwardsModifier, ForceMode mode){}
		public Vector3 ClosestPointOnBounds(Vector3 position){}
		private static Vector3 INTERNAL_CALL_ClosestPointOnBounds(Rigidbody self, ref Vector3 position){}
		public Vector3 GetRelativePointVelocity(Vector3 relativePoint){}
		private static Vector3 INTERNAL_CALL_GetRelativePointVelocity(Rigidbody self, ref Vector3 relativePoint){}
		public Vector3 GetPointVelocity(Vector3 worldPoint){}
		private static Vector3 INTERNAL_CALL_GetPointVelocity(Rigidbody self, ref Vector3 worldPoint){}
		private void INTERNAL_get_centerOfMass(out Vector3 value){}
		private void INTERNAL_set_centerOfMass(ref Vector3 value){}
		private void INTERNAL_get_worldCenterOfMass(out Vector3 value){}
		private void INTERNAL_get_inertiaTensorRotation(out Quaternion value){}
		private void INTERNAL_set_inertiaTensorRotation(ref Quaternion value){}
		private void INTERNAL_get_inertiaTensor(out Vector3 value){}
		private void INTERNAL_set_inertiaTensor(ref Vector3 value){}
		public Rigidbody(){}
		public Vector3 velocity{ get	{} set	{} }
		public Vector3 angularVelocity{ get	{} set	{} }
		public float drag{ get	{} set	{} }
		public float angularDrag{ get	{} set	{} }
		public float mass{ get	{} set	{} }
		public bool useGravity{ get	{} set	{} }
		public bool isKinematic{ get	{} set	{} }
		public bool freezeRotation{ get	{} set	{} }
		public RigidbodyConstraints constraints{ get	{} set	{} }
		public CollisionDetectionMode collisionDetectionMode{ get	{} set	{} }
		public Vector3 centerOfMass{ get	{} set	{} }
		public Vector3 worldCenterOfMass{ get	{} }
		public Quaternion inertiaTensorRotation{ get	{} set	{} }
		public Vector3 inertiaTensor{ get	{} set	{} }
		public bool detectCollisions{ get	{} set	{} }
		public bool useConeFriction{ get	{} set	{} }
		public Vector3 position{ get	{} set	{} }
		public Quaternion rotation{ get	{} set	{} }
		public RigidbodyInterpolation interpolation{ get	{} set	{} }
		public int solverIterationCount{ get	{} set	{} }
		public float sleepVelocity{ get	{} set	{} }
		public float sleepAngularVelocity{ get	{} set	{} }
		public float maxAngularVelocity{ get	{} set	{} }
	}

	public class	Joint: Component
	{
		private void INTERNAL_get_axis(out Vector3 value){}
		private void INTERNAL_set_axis(ref Vector3 value){}
		private void INTERNAL_get_anchor(out Vector3 value){}
		private void INTERNAL_set_anchor(ref Vector3 value){}
		public Joint(){}
		public Rigidbody connectedBody{ get	{} set	{} }
		public Vector3 axis{ get	{} set	{} }
		public Vector3 anchor{ get	{} set	{} }
		public float breakForce{ get	{} set	{} }
		public float breakTorque{ get	{} set	{} }
	}

	public sealed class	HingeJoint: Joint
	{
		private void INTERNAL_get_motor(out JointMotor value){}
		private void INTERNAL_set_motor(ref JointMotor value){}
		private void INTERNAL_get_limits(out JointLimits value){}
		private void INTERNAL_set_limits(ref JointLimits value){}
		private void INTERNAL_get_spring(out JointSpring value){}
		private void INTERNAL_set_spring(ref JointSpring value){}
		public HingeJoint(){}
		public JointMotor motor{ get	{} set	{} }
		public JointLimits limits{ get	{} set	{} }
		public JointSpring spring{ get	{} set	{} }
		public bool useMotor{ get	{} set	{} }
		public bool useLimits{ get	{} set	{} }
		public bool useSpring{ get	{} set	{} }
		public float velocity{ get	{} }
		public float angle{ get	{} }
	}

	public sealed class	SpringJoint: Joint
	{
		public SpringJoint(){}
		public float spring{ get	{} set	{} }
		public float damper{ get	{} set	{} }
		public float minDistance{ get	{} set	{} }
		public float maxDistance{ get	{} set	{} }
	}

	public sealed class	FixedJoint: Joint
	{
		public FixedJoint(){}
	}

	public sealed class	CharacterJoint: Joint
	{
		private void INTERNAL_get_swingAxis(out Vector3 value){}
		private void INTERNAL_set_swingAxis(ref Vector3 value){}
		private void INTERNAL_get_lowTwistLimit(out SoftJointLimit value){}
		private void INTERNAL_set_lowTwistLimit(ref SoftJointLimit value){}
		private void INTERNAL_get_highTwistLimit(out SoftJointLimit value){}
		private void INTERNAL_set_highTwistLimit(ref SoftJointLimit value){}
		private void INTERNAL_get_swing1Limit(out SoftJointLimit value){}
		private void INTERNAL_set_swing1Limit(ref SoftJointLimit value){}
		private void INTERNAL_get_swing2Limit(out SoftJointLimit value){}
		private void INTERNAL_set_swing2Limit(ref SoftJointLimit value){}
		private void INTERNAL_get_targetRotation(out Quaternion value){}
		private void INTERNAL_set_targetRotation(ref Quaternion value){}
		private void INTERNAL_get_targetAngularVelocity(out Vector3 value){}
		private void INTERNAL_set_targetAngularVelocity(ref Vector3 value){}
		private void INTERNAL_get_rotationDrive(out JointDrive value){}
		private void INTERNAL_set_rotationDrive(ref JointDrive value){}
		public CharacterJoint(){}
		public Vector3 swingAxis{ get	{} set	{} }
		public SoftJointLimit lowTwistLimit{ get	{} set	{} }
		public SoftJointLimit highTwistLimit{ get	{} set	{} }
		public SoftJointLimit swing1Limit{ get	{} set	{} }
		public SoftJointLimit swing2Limit{ get	{} set	{} }
		public Quaternion targetRotation{ get	{} set	{} }
		public Vector3 targetAngularVelocity{ get	{} set	{} }
		public JointDrive rotationDrive{ get	{} set	{} }
	}

	public sealed class	ConfigurableJoint: Joint
	{
		private void INTERNAL_get_angularYZDrive(out JointDrive value){}
		private void INTERNAL_set_angularYZDrive(ref JointDrive value){}
		private void INTERNAL_get_slerpDrive(out JointDrive value){}
		private void INTERNAL_set_slerpDrive(ref JointDrive value){}
		private void INTERNAL_get_secondaryAxis(out Vector3 value){}
		private void INTERNAL_set_secondaryAxis(ref Vector3 value){}
		private void INTERNAL_get_linearLimit(out SoftJointLimit value){}
		private void INTERNAL_set_linearLimit(ref SoftJointLimit value){}
		private void INTERNAL_get_lowAngularXLimit(out SoftJointLimit value){}
		private void INTERNAL_set_lowAngularXLimit(ref SoftJointLimit value){}
		private void INTERNAL_get_highAngularXLimit(out SoftJointLimit value){}
		private void INTERNAL_set_highAngularXLimit(ref SoftJointLimit value){}
		private void INTERNAL_get_angularYLimit(out SoftJointLimit value){}
		private void INTERNAL_set_angularYLimit(ref SoftJointLimit value){}
		private void INTERNAL_get_angularZLimit(out SoftJointLimit value){}
		private void INTERNAL_set_angularZLimit(ref SoftJointLimit value){}
		private void INTERNAL_get_targetPosition(out Vector3 value){}
		private void INTERNAL_set_targetPosition(ref Vector3 value){}
		private void INTERNAL_get_targetVelocity(out Vector3 value){}
		private void INTERNAL_set_targetVelocity(ref Vector3 value){}
		private void INTERNAL_get_xDrive(out JointDrive value){}
		private void INTERNAL_set_xDrive(ref JointDrive value){}
		private void INTERNAL_get_yDrive(out JointDrive value){}
		private void INTERNAL_set_yDrive(ref JointDrive value){}
		private void INTERNAL_get_zDrive(out JointDrive value){}
		private void INTERNAL_set_zDrive(ref JointDrive value){}
		private void INTERNAL_get_targetRotation(out Quaternion value){}
		private void INTERNAL_set_targetRotation(ref Quaternion value){}
		private void INTERNAL_get_targetAngularVelocity(out Vector3 value){}
		private void INTERNAL_set_targetAngularVelocity(ref Vector3 value){}
		private void INTERNAL_get_angularXDrive(out JointDrive value){}
		private void INTERNAL_set_angularXDrive(ref JointDrive value){}
		public ConfigurableJoint(){}
		public Vector3 secondaryAxis{ get	{} set	{} }
		public ConfigurableJointMotion xMotion{ get	{} set	{} }
		public ConfigurableJointMotion yMotion{ get	{} set	{} }
		public ConfigurableJointMotion zMotion{ get	{} set	{} }
		public ConfigurableJointMotion angularXMotion{ get	{} set	{} }
		public ConfigurableJointMotion angularYMotion{ get	{} set	{} }
		public ConfigurableJointMotion angularZMotion{ get	{} set	{} }
		public SoftJointLimit linearLimit{ get	{} set	{} }
		public SoftJointLimit lowAngularXLimit{ get	{} set	{} }
		public SoftJointLimit highAngularXLimit{ get	{} set	{} }
		public SoftJointLimit angularYLimit{ get	{} set	{} }
		public SoftJointLimit angularZLimit{ get	{} set	{} }
		public Vector3 targetPosition{ get	{} set	{} }
		public Vector3 targetVelocity{ get	{} set	{} }
		public JointDrive xDrive{ get	{} set	{} }
		public JointDrive yDrive{ get	{} set	{} }
		public JointDrive zDrive{ get	{} set	{} }
		public Quaternion targetRotation{ get	{} set	{} }
		public Vector3 targetAngularVelocity{ get	{} set	{} }
		public RotationDriveMode rotationDriveMode{ get	{} set	{} }
		public JointDrive angularXDrive{ get	{} set	{} }
		public JointDrive angularYZDrive{ get	{} set	{} }
		public JointDrive slerpDrive{ get	{} set	{} }
		public JointProjectionMode projectionMode{ get	{} set	{} }
		public float projectionDistance{ get	{} set	{} }
		public float projectionAngle{ get	{} set	{} }
		public bool configuredInWorldSpace{ get	{} set	{} }
		public bool swapBodies{ get	{} set	{} }
	}

	public sealed class	ConstantForce: Behaviour
	{
		private void INTERNAL_get_force(out Vector3 value){}
		private void INTERNAL_set_force(ref Vector3 value){}
		private void INTERNAL_get_relativeForce(out Vector3 value){}
		private void INTERNAL_set_relativeForce(ref Vector3 value){}
		private void INTERNAL_get_torque(out Vector3 value){}
		private void INTERNAL_set_torque(ref Vector3 value){}
		private void INTERNAL_get_relativeTorque(out Vector3 value){}
		private void INTERNAL_set_relativeTorque(ref Vector3 value){}
		public ConstantForce(){}
		public Vector3 force{ get	{} set	{} }
		public Vector3 relativeForce{ get	{} set	{} }
		public Vector3 torque{ get	{} set	{} }
		public Vector3 relativeTorque{ get	{} set	{} }
	}

	public class	Collider: Component
	{
		public Vector3 ClosestPointOnBounds(Vector3 position){}
		private static Vector3 INTERNAL_CALL_ClosestPointOnBounds(Collider self, ref Vector3 position){}
		private void INTERNAL_get_bounds(out Bounds value){}
		private static bool Internal_Raycast(Collider col, Ray ray, out RaycastHit hitInfo, float distance){}
		private static bool INTERNAL_CALL_Internal_Raycast(Collider col, ref Ray ray, out RaycastHit hitInfo, float distance){}
		public bool Raycast(Ray ray, out RaycastHit hitInfo, float distance){}
		public Collider(){}
		public bool enabled{ get	{} set	{} }
		public Rigidbody attachedRigidbody{ get	{} }
		public bool isTrigger{ get	{} set	{} }
		public PhysicMaterial material{ get	{} set	{} }
		public PhysicMaterial sharedMaterial{ get	{} set	{} }
		public Bounds bounds{ get	{} }
	}

	public sealed class	BoxCollider: Collider
	{
		private void INTERNAL_get_center(out Vector3 value){}
		private void INTERNAL_set_center(ref Vector3 value){}
		private void INTERNAL_get_size(out Vector3 value){}
		private void INTERNAL_set_size(ref Vector3 value){}
		public BoxCollider(){}
		public Vector3 center{ get	{} set	{} }
		public Vector3 size{ get	{} set	{} }
		public Vector3 extents{ get	{} set	{} }
	}

	public sealed class	SphereCollider: Collider
	{
		private void INTERNAL_get_center(out Vector3 value){}
		private void INTERNAL_set_center(ref Vector3 value){}
		public SphereCollider(){}
		public Vector3 center{ get	{} set	{} }
		public float radius{ get	{} set	{} }
	}

	public sealed class	MeshCollider: Collider
	{
		public MeshCollider(){}
		public Mesh mesh{ get	{} set	{} }
		public Mesh sharedMesh{ get	{} set	{} }
		public bool convex{ get	{} set	{} }
		public bool smoothSphereCollisions{ get	{} set	{} }
	}

	public sealed class	CapsuleCollider: Collider
	{
		private void INTERNAL_get_center(out Vector3 value){}
		private void INTERNAL_set_center(ref Vector3 value){}
		public CapsuleCollider(){}
		public Vector3 center{ get	{} set	{} }
		public float radius{ get	{} set	{} }
		public float height{ get	{} set	{} }
		public int direction{ get	{} set	{} }
	}

	public sealed class	RaycastCollider: Collider
	{
		private void INTERNAL_get_center(out Vector3 value){}
		private void INTERNAL_set_center(ref Vector3 value){}
		public RaycastCollider(){}
		public Vector3 center{ get	{} set	{} }
		public float length{ get	{} set	{} }
	}

	public sealed class	WheelCollider: Collider
	{
		private void INTERNAL_get_center(out Vector3 value){}
		private void INTERNAL_set_center(ref Vector3 value){}
		private void INTERNAL_get_suspensionSpring(out JointSpring value){}
		private void INTERNAL_set_suspensionSpring(ref JointSpring value){}
		private void INTERNAL_get_forwardFriction(out WheelFrictionCurve value){}
		private void INTERNAL_set_forwardFriction(ref WheelFrictionCurve value){}
		private void INTERNAL_get_sidewaysFriction(out WheelFrictionCurve value){}
		private void INTERNAL_set_sidewaysFriction(ref WheelFrictionCurve value){}
		public bool GetGroundHit(out WheelHit hit){}
		public WheelCollider(){}
		public Vector3 center{ get	{} set	{} }
		public float radius{ get	{} set	{} }
		public float suspensionDistance{ get	{} set	{} }
		public JointSpring suspensionSpring{ get	{} set	{} }
		public float mass{ get	{} set	{} }
		public WheelFrictionCurve forwardFriction{ get	{} set	{} }
		public WheelFrictionCurve sidewaysFriction{ get	{} set	{} }
		public float motorTorque{ get	{} set	{} }
		public float brakeTorque{ get	{} set	{} }
		public float steerAngle{ get	{} set	{} }
		public bool isGrounded{ get	{} }
		public float rpm{ get	{} }
	}

	public sealed class	PhysicMaterial: Object
	{
		private static void Internal_CreateDynamicsMaterial(PhysicMaterial mat, string name){}
		private void INTERNAL_get_frictionDirection2(out Vector3 value){}
		private void INTERNAL_set_frictionDirection2(ref Vector3 value){}
		public PhysicMaterial(){}
		public PhysicMaterial(string name){}
		public float dynamicFriction{ get	{} set	{} }
		public float staticFriction{ get	{} set	{} }
		public float bounciness{ get	{} set	{} }
		public float bouncyness{ get	{} set	{} }
		public Vector3 frictionDirection2{ get	{} set	{} }
		public float dynamicFriction2{ get	{} set	{} }
		public float staticFriction2{ get	{} set	{} }
		public PhysicMaterialCombine frictionCombine{ get	{} set	{} }
		public PhysicMaterialCombine bounceCombine{ get	{} set	{} }
		public Vector3 frictionDirection{ get	{} set	{} }
	}

	public class	Collision: Object
	{
		public virtual IEnumerator GetEnumerator(){}
		public Collision(){}
		public Vector3 relativeVelocity{ get	{} }
		public Rigidbody rigidbody{ get	{} }
		public Collider collider{ get	{} }
		public Transform transform{ get	{} }
		public GameObject gameObject{ get	{} }
		public ContactPoint[] contacts{ get	{} }
		public Vector3 impactForceSum{ get	{} }
		public Vector3 frictionForceSum{ get	{} }
		public Component other{ get	{} }
		private Vector3 m_RelativeVelocity;
		private Rigidbody m_Rigidbody;
		private Collider m_Collider;
		private ContactPoint[] m_Contacts;
	}

	public sealed class	ControllerColliderHit: Object
	{
		public ControllerColliderHit(){}
		public CharacterController controller{ get	{} }
		public Collider collider{ get	{} }
		public Rigidbody rigidbody{ get	{} }
		public GameObject gameObject{ get	{} }
		public Transform transform{ get	{} }
		public Vector3 point{ get	{} }
		public Vector3 normal{ get	{} }
		public Vector3 moveDirection{ get	{} }
		public float moveLength{ get	{} }
		bool push{ get	{} set	{} }
		private CharacterController m_Controller;
		private Collider m_Collider;
		private Vector3 m_Point;
		private Vector3 m_Normal;
		private Vector3 m_MoveDirection;
		private float m_MoveLength;
		private int m_Push;
	}

	public sealed class	CharacterController: Collider
	{
		public bool SimpleMove(Vector3 speed){}
		private static bool INTERNAL_CALL_SimpleMove(CharacterController self, ref Vector3 speed){}
		public CollisionFlags Move(Vector3 motion){}
		private static CollisionFlags INTERNAL_CALL_Move(CharacterController self, ref Vector3 motion){}
		private void INTERNAL_get_velocity(out Vector3 value){}
		private void INTERNAL_get_center(out Vector3 value){}
		private void INTERNAL_set_center(ref Vector3 value){}
		public CharacterController(){}
		public bool isGrounded{ get	{} }
		public Vector3 velocity{ get	{} }
		public CollisionFlags collisionFlags{ get	{} }
		public float radius{ get	{} set	{} }
		public float height{ get	{} set	{} }
		public Vector3 center{ get	{} set	{} }
		public float slopeLimit{ get	{} set	{} }
		public float stepOffset{ get	{} set	{} }
		public bool detectCollisions{ get	{} set	{} }
	}

	public class	Cloth: Component
	{
		private void INTERNAL_get_externalAcceleration(out Vector3 value){}
		private void INTERNAL_set_externalAcceleration(ref Vector3 value){}
		private void INTERNAL_get_randomAcceleration(out Vector3 value){}
		private void INTERNAL_set_randomAcceleration(ref Vector3 value){}
		public Cloth(){}
		public float bendingStiffness{ get	{} set	{} }
		public float stretchingStiffness{ get	{} set	{} }
		public float damping{ get	{} set	{} }
		public float thickness{ get	{} set	{} }
		public Vector3 externalAcceleration{ get	{} set	{} }
		public Vector3 randomAcceleration{ get	{} set	{} }
		public bool useGravity{ get	{} set	{} }
		public bool selfCollision{ get	{} set	{} }
		public bool enabled{ get	{} set	{} }
		public Vector3[] vertices{ get	{} }
		public Vector3[] normals{ get	{} }
	}

	public sealed class	InteractiveCloth: Cloth
	{
		public void AddForceAtPosition(Vector3 force, Vector3 position, float radius, ForceMode mode){}
		public void AddForceAtPosition(Vector3 force, Vector3 position, float radius){}
		private static void INTERNAL_CALL_AddForceAtPosition(InteractiveCloth self, ref Vector3 force, ref Vector3 position, float radius, ForceMode mode){}
		public void AttachToCollider(Collider collider, bool tearable, bool twoWayInteraction){}
		public void AttachToCollider(Collider collider, bool tearable){}
		public void AttachToCollider(Collider collider){}
		public void DetachFromCollider(Collider collider){}
		public InteractiveCloth(){}
		public Mesh mesh{ get	{} set	{} }
		public float friction{ get	{} set	{} }
		public float density{ get	{} set	{} }
		public float pressure{ get	{} set	{} }
		public float collisionResponse{ get	{} set	{} }
		public float tearFactor{ get	{} set	{} }
		public float attachmentTearFactor{ get	{} set	{} }
		public float attachmentResponse{ get	{} set	{} }
		public bool isTeared{ get	{} }
	}

	public sealed class	SkinnedCloth: Cloth
	{
		public void SetEnabledFading(bool enabled, float interpolationTime){}
		public void SetEnabledFading(bool enabled){}
		public SkinnedCloth(){}
		public ClothSkinningCoefficient[] coefficients{ get	{} set	{} }
		public float worldVelocityScale{ get	{} set	{} }
		public float worldAccelerationScale{ get	{} set	{} }
	}

	public sealed class	ClothRenderer: Renderer
	{
		public ClothRenderer(){}
		public bool pauseWhenNotVisible{ get	{} set	{} }
	}

	public sealed class	ParticleSystem: Component
	{
		private void INTERNAL_get_startColor(out Color value){}
		private void INTERNAL_set_startColor(ref Color value){}
		public void SetParticles(Particle[] particles, int size){}
		public int GetParticles(Particle[] particles){}
		private void Internal_Simulate(float t, bool restart){}
		private void Internal_Play(){}
		private void Internal_Stop(){}
		private void Internal_Pause(){}
		private void Internal_Clear(){}
		private bool Internal_IsAlive(){}
		public void Simulate(float t, bool withChildren){}
		public void Simulate(float t){}
		public void Simulate(float t, bool withChildren, bool restart){}
		public void Play(){}
		public void Play(bool withChildren){}
		public void Stop(){}
		public void Stop(bool withChildren){}
		public void Pause(){}
		public void Pause(bool withChildren){}
		public void Clear(){}
		public void Clear(bool withChildren){}
		public bool IsAlive(){}
		public bool IsAlive(bool withChildren){}
		public void Emit(int count){}
		private static void INTERNAL_CALL_Emit(ParticleSystem self, int count){}
		public void Emit(Vector3 position, Vector3 velocity, float size, float lifetime, Color32 color){}
		public void Emit(Particle particle){}
		private void Internal_Emit(ref Particle particle){}
		internal static ParticleSystem[] GetParticleSystems(ParticleSystem root){}
		private static void GetDirectParticleSystemChildrenRecursive(Transform transform, List<ParticleSystem> particleSystems){}
		internal void SetupDefaultType(int type){}
		public ParticleSystem(){}
		public float startDelay{ get	{} set	{} }
		public bool isPlaying{ get	{} }
		public bool isStopped{ get	{} }
		public bool isPaused{ get	{} }
		public bool loop{ get	{} set	{} }
		public bool playOnAwake{ get	{} set	{} }
		public float time{ get	{} set	{} }
		public float duration{ get	{} }
		public float playbackSpeed{ get	{} set	{} }
		public int particleCount{ get	{} }
		public bool enableEmission{ get	{} set	{} }
		public float emissionRate{ get	{} set	{} }
		public float startSpeed{ get	{} set	{} }
		public float startSize{ get	{} set	{} }
		public Color startColor{ get	{} set	{} }
		public float startRotation{ get	{} set	{} }
		public float startLifetime{ get	{} set	{} }
		public float gravityModifier{ get	{} set	{} }
		public uint randomSeed{ get	{} set	{} }
	}

	public sealed class	ParticleSystemRenderer: Renderer
	{
		public ParticleSystemRenderer(){}
		public ParticleSystemRenderMode renderMode{ get	{} set	{} }
		public float lengthScale{ get	{} set	{} }
		public float velocityScale{ get	{} set	{} }
		public float cameraVelocityScale{ get	{} set	{} }
		public float maxParticleSize{ get	{} set	{} }
		public Mesh mesh{ get	{} set	{} }
		bool editorEnabled{ get	{} set	{} }
	}

	public class	TextAsset: Object
	{
		public virtual string ToString(){}
		public TextAsset(){}
		public string text{ get	{} }
		public Byte[] bytes{ get	{} }
	}

	public sealed class	ProceduralPropertyDescription: Object
	{
		public ProceduralPropertyDescription(){}
		public string name;
		public string group;
		public ProceduralPropertyType type;
		public bool hasRange;
		public float minimum;
		public float maximum;
		public float step;
		public String[] enumOptions;
	}

	public sealed class	ProceduralMaterial: Material
	{
		public ProceduralPropertyDescription[] GetProceduralPropertyDescriptions(){}
		public bool HasProceduralProperty(string inputName){}
		public bool GetProceduralBoolean(string inputName){}
		public void SetProceduralBoolean(string inputName, bool value){}
		public float GetProceduralFloat(string inputName){}
		public void SetProceduralFloat(string inputName, float value){}
		public Vector4 GetProceduralVector(string inputName){}
		public void SetProceduralVector(string inputName, Vector4 value){}
		private static void INTERNAL_CALL_SetProceduralVector(ProceduralMaterial self, string inputName, ref Vector4 value){}
		public Color GetProceduralColor(string inputName){}
		public void SetProceduralColor(string inputName, Color value){}
		private static void INTERNAL_CALL_SetProceduralColor(ProceduralMaterial self, string inputName, ref Color value){}
		public int GetProceduralEnum(string inputName){}
		public void SetProceduralEnum(string inputName, int value){}
		public Texture2D GetProceduralTexture(string inputName){}
		public void SetProceduralTexture(string inputName, Texture2D value){}
		public bool IsProceduralPropertyCached(string inputName){}
		public void CacheProceduralProperty(string inputName, bool value){}
		public void ClearCache(){}
		public void RebuildTextures(){}
		public void RebuildTexturesImmediately(){}
		public static void StopRebuilds(){}
		public Texture[] GetGeneratedTextures(){}
		public ProceduralMaterial(){}
		public ProceduralCacheSize cacheSize{ get	{} set	{} }
		public int animationUpdateRate{ get	{} set	{} }
		public bool isProcessing{ get	{} }
		public bool isLoadTimeGenerated{ get	{} }
		public ProceduralLoadingBehavior loadingBehavior{ get	{} }
		public static bool isSupported{ get	{} }
		public static ProceduralProcessorUsage substanceProcessorUsage{ get	{} set	{} }
	}

	public sealed class	TreePrototype: Object
	{
		public TreePrototype(){}
		public GameObject prefab{ get	{} set	{} }
		public float bendFactor{ get	{} set	{} }
		private GameObject m_Prefab;
		private float m_BendFactor;
	}

	public sealed class	DetailPrototype: Object
	{
		public DetailPrototype(){}
		public GameObject prototype{ get	{} set	{} }
		public Texture2D prototypeTexture{ get	{} set	{} }
		public float minWidth{ get	{} set	{} }
		public float maxWidth{ get	{} set	{} }
		public float minHeight{ get	{} set	{} }
		public float maxHeight{ get	{} set	{} }
		public float noiseSpread{ get	{} set	{} }
		public float bendFactor{ get	{} set	{} }
		public Color healthyColor{ get	{} set	{} }
		public Color dryColor{ get	{} set	{} }
		public DetailRenderMode renderMode{ get	{} set	{} }
		public bool usePrototypeMesh{ get	{} set	{} }
		private GameObject m_Prototype;
		private Texture2D m_PrototypeTexture;
		private Color m_HealthyColor;
		private Color m_DryColor;
		private float m_MinWidth;
		private float m_MaxWidth;
		private float m_MinHeight;
		private float m_MaxHeight;
		private float m_NoiseSpread;
		private float m_BendFactor;
		private int m_RenderMode;
		private int m_UsePrototypeMesh;
	}

	public sealed class	SplatPrototype: Object
	{
		public SplatPrototype(){}
		public Texture2D texture{ get	{} set	{} }
		public Texture2D normalMap{ get	{} set	{} }
		public Vector2 tileSize{ get	{} set	{} }
		public Vector2 tileOffset{ get	{} set	{} }
		private Texture2D m_Texture;
		private Texture2D m_NormalMap;
		private Vector2 m_TileSize;
		private Vector2 m_TileOffset;
	}

	public sealed class	TerrainData: Object
	{
		private Texture2D GetAlphamapTexture(int index){}
		internal bool HasTreeInstances(){}
		internal void AddTree(out TreeInstance tree){}
		internal int RemoveTrees(Vector2 position, float radius, int prototypeIndex){}
		private static int INTERNAL_CALL_RemoveTrees(TerrainData self, ref Vector2 position, float radius, int prototypeIndex){}
		internal bool HasUser(GameObject user){}
		internal void AddUser(GameObject user){}
		internal void RemoveUser(GameObject user){}
		private void INTERNAL_get_size(out Vector3 value){}
		private void INTERNAL_set_size(ref Vector3 value){}
		public float GetHeight(int x, int y){}
		public float GetInterpolatedHeight(float x, float y){}
		public Single[,] GetHeights(int xBase, int yBase, int width, int height){}
		public void SetHeights(int xBase, int yBase, Single[,] heights){}
		private void Internal_SetHeights(int xBase, int yBase, int width, int height, Single[,] heights){}
		private void Internal_SetHeightsDelayLOD(int xBase, int yBase, int width, int height, Single[,] heights){}
		internal Int32[] ComputeDelayedLod(){}
		internal void SetHeightsDelayLOD(int xBase, int yBase, Single[,] heights){}
		public float GetSteepness(float x, float y){}
		public Vector3 GetInterpolatedNormal(float x, float y){}
		internal int GetAdjustedSize(int size){}
		private void INTERNAL_get_wavingGrassTint(out Color value){}
		private void INTERNAL_set_wavingGrassTint(ref Color value){}
		public void SetDetailResolution(int detailResolution, int resolutionPerPatch){}
		internal void ResetDirtyDetails(){}
		public void RefreshPrototypes(){}
		public Int32[] GetSupportedLayers(int xBase, int yBase, int totalWidth, int totalHeight){}
		public Int32[,] GetDetailLayer(int xBase, int yBase, int width, int height, int layer){}
		public void SetDetailLayer(int xBase, int yBase, int layer, Int32[,] details){}
		private void Internal_SetDetailLayer(int xBase, int yBase, int totalWidth, int totalHeight, int detailIndex, Int32[,] data){}
		internal void RemoveTreePrototype(int index){}
		internal void RecalculateTreePositions(){}
		internal void RemoveDetailPrototype(int index){}
		public Single[,,] GetAlphamaps(int x, int y, int width, int height){}
		public void SetAlphamaps(int x, int y, Single[,,] map){}
		private void Internal_SetAlphamaps(int x, int y, int width, int height, Single[,,] map){}
		internal void RecalculateBasemapIfDirty(){}
		internal void SetBasemapDirty(bool dirty){}
		public TerrainData(){}
		public int heightmapWidth{ get	{} }
		public int heightmapHeight{ get	{} }
		public int heightmapResolution{ get	{} set	{} }
		public Vector3 heightmapScale{ get	{} }
		public Vector3 size{ get	{} set	{} }
		public float wavingGrassStrength{ get	{} set	{} }
		public float wavingGrassAmount{ get	{} set	{} }
		public float wavingGrassSpeed{ get	{} set	{} }
		public Color wavingGrassTint{ get	{} set	{} }
		public int detailWidth{ get	{} }
		public int detailHeight{ get	{} }
		public int detailResolution{ get	{} }
		int detailResolutionPerPatch{ get	{} }
		public DetailPrototype[] detailPrototypes{ get	{} set	{} }
		public TreeInstance[] treeInstances{ get	{} set	{} }
		public TreePrototype[] treePrototypes{ get	{} set	{} }
		public int alphamapLayers{ get	{} }
		public int alphamapResolution{ get	{} set	{} }
		public int alphamapWidth{ get	{} }
		public int alphamapHeight{ get	{} }
		public int baseMapResolution{ get	{} set	{} }
		int alphamapTextureCount{ get	{} }
		Texture2D[] alphamapTextures{ get	{} }
		public SplatPrototype[] splatPrototypes{ get	{} set	{} }
	}

	public sealed class	Terrain: MonoBehaviour
	{
		private void SetLightmapIndex(int value){}
		private void ShiftLightmapIndex(int offset){}
		public float SampleHeight(Vector3 worldPosition){}
		public static GameObject CreateTerrainGameObject(TerrainData assignTerrain){}
		private static void ReconnectTerrainData(){}
		private static void SetLightmapIndexOnAllTerrains(int lightmapIndex){}
		internal void ApplyDelayedHeightmapModification(){}
		private void FlushDirty(){}
		private static void CullAllTerrains(int cullingMask){}
		private static void CullAllTerrainsShadowCaster(Light light){}
		public void AddTreeInstance(TreeInstance instance){}
		public void SetNeighbors(Terrain left, Terrain top, Terrain right, Terrain bottom){}
		public Vector3 GetPosition(){}
		public void Flush(){}
		private void GarbageCollectRenderers(){}
		internal void RemoveTrees(Vector2 position, float radius, int prototypeIndex){}
		private void OnTerrainChanged(TerrainChangedFlags flags){}
		private void OnEnable(){}
		private void OnDisable(){}
		private TerrainRenderer GetTerrainRendererDontCreate(){}
		private Renderer GetRenderer(){}
		public Terrain(){}
		private static Terrain(){}
		public static Terrain activeTerrain{ get	{} }
		public TerrainData terrainData{ get	{} set	{} }
		public float treeDistance{ get	{} set	{} }
		public float treeBillboardDistance{ get	{} set	{} }
		public float treeCrossFadeLength{ get	{} set	{} }
		public int treeMaximumFullLODCount{ get	{} set	{} }
		public float detailObjectDistance{ get	{} set	{} }
		public float detailObjectDensity{ get	{} set	{} }
		public float heightmapPixelError{ get	{} set	{} }
		public int heightmapMaximumLOD{ get	{} set	{} }
		public float basemapDistance{ get	{} set	{} }
		public float splatmapDistance{ get	{} set	{} }
		public int lightmapIndex{ get	{} set	{} }
		int lightmapSize{ get	{} set	{} }
		public bool castShadows{ get	{} set	{} }
		public Material materialTemplate{ get	{} set	{} }
		bool drawTreesAndFoliage{ get	{} set	{} }
		public TerrainRenderFlags editorRenderFlags{ get	{} set	{} }
		private TerrainData m_TerrainData;
		private float m_TreeDistance;
		private float m_TreeBillboardDistance;
		private float m_TreeCrossFadeLength;
		private int m_TreeMaximumFullLODCount;
		private float m_DetailObjectDistance;
		private float m_DetailObjectDensity;
		private float m_HeightmapPixelError;
		private float m_SplatMapDistance;
		private int m_HeightmapMaximumLOD;
		private bool m_CastShadows;
		private int m_LightmapIndex;
		private int m_LightmapSize;
		private bool m_DrawTreesAndFoliage;
		private Material m_MaterialTemplate;
		private Terrain m_LeftNeighbor;
		private Terrain m_RightNeighbor;
		private Terrain m_BottomNeighbor;
		private Terrain m_TopNeighbor;
		private Vector3 m_Position;
		private TerrainRenderFlags m_EditorRenderFlags;
		private ArrayList renderers;
		private TerrainChangedFlags dirtyFlags;
		internal static ArrayList ms_ActiveTerrains;
		private static ArrayList ms_TempCulledTerrains;
		private static Terrain ms_ActiveTerrain;
	}

	public sealed class	Tree: Component
	{
		public Tree(){}
		public ScriptableObject data{ get	{} set	{} }
	}

	public sealed class	TerrainCollider: Collider
	{
		public TerrainCollider(){}
		public TerrainData terrainData{ get	{} set	{} }
	}

	public sealed class	WWWForm: Object
	{
		public void AddField(string fieldName, string value){}
		public void AddField(string fieldName, string value, Encoding e){}
		public void AddField(string fieldName, int i){}
		public void AddBinaryData(string fieldName, Byte[] contents, string fileName){}
		public void AddBinaryData(string fieldName, Byte[] contents){}
		public void AddBinaryData(string fieldName, Byte[] contents, string fileName, string mimeType){}
		public WWWForm(){}
		public Hashtable headers{ get	{} }
		public Byte[] data{ get	{} }
		private ArrayList formData;
		private ArrayList fieldNames;
		private ArrayList fileNames;
		private ArrayList types;
		private Byte[] boundary;
		private bool containsFiles;
	}

	public sealed class	Caching: Object
	{
		public static bool Authorize(string name, string domain, long size, string signature){}
		public static bool Authorize(string name, string domain, long size, int expiration, string signature){}
		public static bool Authorize(string name, string domain, int size, int expiration, string signature){}
		public static bool Authorize(string name, string domain, int size, string signature){}
		public static bool CleanCache(){}
		public static bool CleanNamedCache(string name){}
		public static bool DeleteFromCache(string url){}
		public static int GetVersionFromCache(string url){}
		public static bool IsVersionCached(string url, int version){}
		public static bool MarkAsUsed(string url, int version){}
		public static void SetNoBackupFlag(string url, int version){}
		public static void ResetNoBackupFlag(string url, int version){}
		public Caching(){}
		public static CacheIndex[] index{ get	{} }
		public static long spaceFree{ get	{} }
		public static long maximumAvailableDiskSpace{ get	{} set	{} }
		public static long spaceOccupied{ get	{} }
		public static int spaceAvailable{ get	{} }
		public static int spaceUsed{ get	{} }
		public static int expirationDelay{ get	{} set	{} }
		public static bool enabled{ get	{} set	{} }
		public static bool ready{ get	{} }
	}

	public class	AsyncOperation: YieldInstruction
	{
		private void InternalDestroy(){}
		protected virtual void Finalize(){}
		public AsyncOperation(){}
		public bool isDone{ get	{} }
		public float progress{ get	{} }
		public int priority{ get	{} set	{} }
		public bool allowSceneActivation{ get	{} set	{} }
		private IntPtr m_Ptr;
	}

	public sealed class	Application: Object
	{
		internal static void ReplyToUserAuthorizationRequest(bool reply){}
		private static int GetUserAuthorizationRequestMode_Internal(){}
		internal static UserAuthorization GetUserAuthorizationRequestMode(){}
		public static void Quit(){}
		public static void CancelQuit(){}
		public static void LoadLevel(int index){}
		public static void LoadLevel(string name){}
		public static AsyncOperation LoadLevelAsync(int index){}
		public static AsyncOperation LoadLevelAsync(string levelName){}
		public static AsyncOperation LoadLevelAdditiveAsync(int index){}
		public static AsyncOperation LoadLevelAdditiveAsync(string levelName){}
		private static AsyncOperation LoadLevelAsync(string monoLevelName, int index, bool additive, bool mustCompleteNextFrame){}
		public static void LoadLevelAdditive(int index){}
		public static void LoadLevelAdditive(string name){}
		private static float GetStreamProgressForLevelByName(string levelName){}
		public static float GetStreamProgressForLevel(int levelIndex){}
		public static float GetStreamProgressForLevel(string levelName){}
		private static bool CanStreamedLevelBeLoadedByName(string levelName){}
		public static bool CanStreamedLevelBeLoaded(int levelIndex){}
		public static bool CanStreamedLevelBeLoaded(string levelName){}
		public static void CaptureScreenshot(string filename, int superSize){}
		public static void CaptureScreenshot(string filename){}
		internal static bool HasProLicense(){}
		internal static bool HasAdvancedLicense(){}
		public static void DontDestroyOnLoad(System.Object mono){}
		private static string ObjectToJSString(System.Object o){}
		public static void ExternalCall(string functionName, Object[] args){}
		private static string BuildInvocationForArguments(string functionName, Object[] args){}
		public static void ExternalEval(string script){}
		private static void Internal_ExternalCall(string script){}
		internal static int GetBuildUnityVersion(){}
		internal static int GetNumericUnityVersion(string version){}
		public static void OpenURL(string url){}
		public static void CommitSuicide(int mode){}
		public static void RegisterLogCallback(LogCallback handler){}
		public static void RegisterLogCallbackThreaded(LogCallback handler){}
		private static void CallLogCallback(string logString, string stackTrace, LogType type){}
		private static void SetLogCallbackDefined(bool defined, bool threaded){}
		public static AsyncOperation RequestUserAuthorization(UserAuthorization mode){}
		public static bool HasUserAuthorization(UserAuthorization mode){}
		internal static void ReplyToUserAuthorizationRequest(bool reply, bool remember){}
		public Application(){}
		public static int loadedLevel{ get	{} }
		public static string loadedLevelName{ get	{} }
		public static bool isLoadingLevel{ get	{} }
		public static int levelCount{ get	{} }
		public static int streamedBytes{ get	{} }
		public static bool isPlaying{ get	{} }
		public static bool isEditor{ get	{} }
		public static bool isWebPlayer{ get	{} }
		public static RuntimePlatform platform{ get	{} }
		public static bool runInBackground{ get	{} set	{} }
		public static bool isPlayer{ get	{} }
		public static string dataPath{ get	{} }
		public static string streamingAssetsPath{ get	{} }
		public static string persistentDataPath{ get	{} }
		public static string temporaryCachePath{ get	{} }
		public static string srcValue{ get	{} }
		public static string absoluteURL{ get	{} }
		public static string absoluteUrl{ get	{} }
		public static string unityVersion{ get	{} }
		public static bool webSecurityEnabled{ get	{} }
		public static string webSecurityHostUrl{ get	{} }
		public static int targetFrameRate{ get	{} set	{} }
		public static SystemLanguage systemLanguage{ get	{} }
		public static ThreadPriority backgroundLoadingPriority{ get	{} set	{} }
		public static NetworkReachability internetReachability{ get	{} }
		public static bool genuine{ get	{} }
		public static bool genuineCheckAvailable{ get	{} }
		private static LogCallback s_LogCallback;
	}

	public class	Behaviour: Component
	{
		public Behaviour(){}
		public bool enabled{ get	{} set	{} }
	}

	public sealed class	Camera: Behaviour
	{
		public Vector3 WorldToViewportPoint(Vector3 position){}
		private static Vector3 INTERNAL_CALL_WorldToViewportPoint(Camera self, ref Vector3 position){}
		public Vector3 ViewportToWorldPoint(Vector3 position){}
		private static Vector3 INTERNAL_CALL_ViewportToWorldPoint(Camera self, ref Vector3 position){}
		public Vector3 ScreenToWorldPoint(Vector3 position){}
		private static Vector3 INTERNAL_CALL_ScreenToWorldPoint(Camera self, ref Vector3 position){}
		public Vector3 ScreenToViewportPoint(Vector3 position){}
		private static Vector3 INTERNAL_CALL_ScreenToViewportPoint(Camera self, ref Vector3 position){}
		public Vector3 ViewportToScreenPoint(Vector3 position){}
		private static Vector3 INTERNAL_CALL_ViewportToScreenPoint(Camera self, ref Vector3 position){}
		public Ray ViewportPointToRay(Vector3 position){}
		private static Ray INTERNAL_CALL_ViewportPointToRay(Camera self, ref Vector3 position){}
		public Ray ScreenPointToRay(Vector3 position){}
		private static Ray INTERNAL_CALL_ScreenPointToRay(Camera self, ref Vector3 position){}
		public float GetScreenWidth(){}
		public float GetScreenHeight(){}
		public void DoClear(){}
		public void Render(){}
		public void RenderWithShader(Shader shader, string replacementTag){}
		public void SetReplacementShader(Shader shader, string replacementTag){}
		public void ResetReplacementShader(){}
		private static void INTERNAL_CALL_ResetReplacementShader(Camera self){}
		public void RenderDontRestore(){}
		public static void SetupCurrent(Camera cur){}
		public bool RenderToCubemap(Cubemap cubemap){}
		public bool RenderToCubemap(Cubemap cubemap, int faceMask){}
		public bool RenderToCubemap(RenderTexture cubemap){}
		public bool RenderToCubemap(RenderTexture cubemap, int faceMask){}
		private bool Internal_RenderToCubemapRT(RenderTexture cubemap, int faceMask){}
		private bool Internal_RenderToCubemapTexture(Cubemap cubemap, int faceMask){}
		public void CopyFrom(Camera other){}
		internal bool IsFiltered(GameObject go){}
		private void INTERNAL_get_backgroundColor(out Color value){}
		private void INTERNAL_set_backgroundColor(ref Color value){}
		private void INTERNAL_get_rect(out Rect value){}
		private void INTERNAL_set_rect(ref Rect value){}
		private void INTERNAL_get_pixelRect(out Rect value){}
		private void INTERNAL_set_pixelRect(ref Rect value){}
		private void INTERNAL_get_cameraToWorldMatrix(out Matrix4x4 value){}
		private void INTERNAL_get_worldToCameraMatrix(out Matrix4x4 value){}
		private void INTERNAL_set_worldToCameraMatrix(ref Matrix4x4 value){}
		public void ResetWorldToCameraMatrix(){}
		private static void INTERNAL_CALL_ResetWorldToCameraMatrix(Camera self){}
		private void INTERNAL_get_projectionMatrix(out Matrix4x4 value){}
		private void INTERNAL_set_projectionMatrix(ref Matrix4x4 value){}
		public void ResetProjectionMatrix(){}
		private static void INTERNAL_CALL_ResetProjectionMatrix(Camera self){}
		public void ResetAspect(){}
		private static void INTERNAL_CALL_ResetAspect(Camera self){}
		private void INTERNAL_get_velocity(out Vector3 value){}
		public Vector3 WorldToScreenPoint(Vector3 position){}
		private static Vector3 INTERNAL_CALL_WorldToScreenPoint(Camera self, ref Vector3 position){}
		public Camera(){}
		public float fov{ get	{} set	{} }
		public float near{ get	{} set	{} }
		public float far{ get	{} set	{} }
		public float fieldOfView{ get	{} set	{} }
		public float nearClipPlane{ get	{} set	{} }
		public float farClipPlane{ get	{} set	{} }
		public RenderingPath renderingPath{ get	{} set	{} }
		public RenderingPath actualRenderingPath{ get	{} }
		public bool hdr{ get	{} set	{} }
		public float orthographicSize{ get	{} set	{} }
		public bool orthographic{ get	{} set	{} }
		public TransparencySortMode transparencySortMode{ get	{} set	{} }
		public bool isOrthoGraphic{ get	{} set	{} }
		public float depth{ get	{} set	{} }
		public float aspect{ get	{} set	{} }
		public int cullingMask{ get	{} set	{} }
		public Color backgroundColor{ get	{} set	{} }
		public Rect rect{ get	{} set	{} }
		public Rect pixelRect{ get	{} set	{} }
		public RenderTexture targetTexture{ get	{} set	{} }
		public float pixelWidth{ get	{} }
		public float pixelHeight{ get	{} }
		public Matrix4x4 cameraToWorldMatrix{ get	{} }
		public Matrix4x4 worldToCameraMatrix{ get	{} set	{} }
		public Matrix4x4 projectionMatrix{ get	{} set	{} }
		public Vector3 velocity{ get	{} }
		public CameraClearFlags clearFlags{ get	{} set	{} }
		public static Camera main{ get	{} }
		public static Camera current{ get	{} }
		public static Camera[] allCameras{ get	{} }
		public static Camera mainCamera{ get	{} }
		public bool useOcclusionCulling{ get	{} set	{} }
		public Single[] layerCullDistances{ get	{} set	{} }
		public bool layerCullSpherical{ get	{} set	{} }
		public DepthTextureMode depthTextureMode{ get	{} set	{} }
	}

	public sealed class	ComputeShader: Object
	{
		public int FindKernel(string name){}
		public void SetFloat(string name, float val){}
		public void SetInt(string name, int val){}
		public void SetVector(string name, Vector4 val){}
		private static void INTERNAL_CALL_SetVector(ComputeShader self, string name, ref Vector4 val){}
		public void SetFloats(string name, Single[] values){}
		private void Internal_SetFloats(string name, Single[] values){}
		public void SetInts(string name, Int32[] values){}
		private void Internal_SetInts(string name, Int32[] values){}
		public void SetTexture(int kernelIndex, string name, Texture texture){}
		public void SetBuffer(int kernelIndex, string name, ComputeBuffer buffer){}
		public void Dispatch(int kernelIndex, int threadsX, int threadsY, int threadsZ){}
		public ComputeShader(){}
	}

	public sealed class	ComputeBuffer: Object, IDisposable
	{
		protected virtual void Finalize(){}
		public sealed virtual void Dispose(){}
		private void Dispose(bool disposing){}
		private static void InitBuffer(ComputeBuffer buf, int count, int stride, ComputeBufferType type){}
		private static void DestroyBuffer(ComputeBuffer buf){}
		public void Release(){}
		public void SetData(Array data){}
		private void InternalSetData(Array data, int elemSize){}
		public void GetData(Array data){}
		private void InternalGetData(Array data, int elemSize){}
		public static void CopyCount(ComputeBuffer src, ComputeBuffer dst, int dstOffset){}
		public ComputeBuffer(int count, int stride){}
		public ComputeBuffer(int count, int stride, ComputeBufferType type){}
		public int count{ get	{} }
		public int stride{ get	{} }
		private IntPtr m_Ptr;
	}

	public sealed class	Debug: Object
	{
		public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration, bool depthTest){}
		public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration){}
		public static void DrawLine(Vector3 start, Vector3 end, Color color){}
		public static void DrawLine(Vector3 start, Vector3 end){}
		private static void INTERNAL_CALL_DrawLine(ref Vector3 start, ref Vector3 end, ref Color color, float duration, bool depthTest){}
		public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration){}
		public static void DrawRay(Vector3 start, Vector3 dir, Color color){}
		public static void DrawRay(Vector3 start, Vector3 dir){}
		public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration, bool depthTest){}
		public static void Break(){}
		public static void DebugBreak(){}
		private static void Internal_Log(int level, string msg, System.Object obj){}
		private static void Internal_LogException(Exception exception, System.Object obj){}
		public static void Log(System.Object message){}
		public static void Log(System.Object message, System.Object context){}
		public static void LogError(System.Object message){}
		public static void LogError(System.Object message, System.Object context){}
		public static void LogException(Exception exception){}
		public static void LogException(Exception exception, System.Object context){}
		internal static void LogPlayerBuildError(string message, string file, int line, int column){}
		public static void LogWarning(System.Object message){}
		public static void LogWarning(System.Object message, System.Object context){}
		internal static void OpenConsoleFile(){}
		public Debug(){}
		public static bool isDebugBuild{ get	{} }
	}

	public sealed class	NotConvertedAttribute: Attribute, _Attribute
	{
		public NotConvertedAttribute(){}
	}

	public sealed class	NotFlashValidatedAttribute: Attribute, _Attribute
	{
		public NotFlashValidatedAttribute(){}
	}

	public sealed class	NotRenamedAttribute: Attribute, _Attribute
	{
		public NotRenamedAttribute(){}
	}

	public class	MonoBehaviour: Behaviour
	{
		private void Internal_CancelInvokeAll(){}
		private bool Internal_IsInvokingAll(){}
		public void Invoke(string methodName, float time){}
		public void InvokeRepeating(string methodName, float time, float repeatRate){}
		public void CancelInvoke(){}
		public void CancelInvoke(string methodName){}
		public bool IsInvoking(string methodName){}
		public bool IsInvoking(){}
		public Coroutine StartCoroutine(IEnumerator routine){}
		public Coroutine StartCoroutine_Auto(IEnumerator routine){}
		public Coroutine StartCoroutine(string methodName, System.Object value){}
		public Coroutine StartCoroutine(string methodName){}
		public void StopCoroutine(string methodName){}
		public void StopAllCoroutines(){}
		public static void print(System.Object message){}
		public MonoBehaviour(){}
		public bool useGUILayout{ get	{} set	{} }
	}

	public sealed class	Gyroscope: Object
	{
		private static Vector3 rotationRate_Internal(int idx){}
		private static Vector3 rotationRateUnbiased_Internal(int idx){}
		private static Vector3 gravity_Internal(int idx){}
		private static Vector3 userAcceleration_Internal(int idx){}
		private static Quaternion attitude_Internal(int idx){}
		private static bool getEnabled_Internal(int idx){}
		private static void setEnabled_Internal(int idx, bool enabled){}
		private static float getUpdateInterval_Internal(int idx){}
		private static void setUpdateInterval_Internal(int idx, float interval){}
		internal Gyroscope(int index){}
		public Vector3 rotationRate{ get	{} }
		public Vector3 rotationRateUnbiased{ get	{} }
		public Vector3 gravity{ get	{} }
		public Vector3 userAcceleration{ get	{} }
		public Quaternion attitude{ get	{} }
		public bool enabled{ get	{} set	{} }
		public float updateInterval{ get	{} set	{} }
		private int m_GyroIndex;
	}

	public sealed class	LocationService: Object
	{
		public void Start(float desiredAccuracyInMeters, float updateDistanceInMeters){}
		public void Start(float desiredAccuracyInMeters){}
		public void Start(){}
		public void Stop(){}
		public LocationService(){}
		public bool isEnabledByUser{ get	{} }
		public LocationServiceStatus status{ get	{} }
		public LocationInfo lastData{ get	{} }
	}

	public sealed class	Compass: Object
	{
		public Compass(){}
		public float magneticHeading{ get	{} }
		public float trueHeading{ get	{} }
		public Vector3 rawVector{ get	{} }
		public double timestamp{ get	{} }
		public bool enabled{ get	{} set	{} }
	}

	public sealed class	Input: Object
	{
		private static int mainGyroIndex_Internal(){}
		private static bool GetKeyInt(int key){}
		private static bool GetKeyString(string name){}
		private static bool GetKeyUpInt(int key){}
		private static bool GetKeyUpString(string name){}
		private static bool GetKeyDownInt(int key){}
		private static bool GetKeyDownString(string name){}
		public static float GetAxis(string axisName){}
		public static float GetAxisRaw(string axisName){}
		public static bool GetButton(string buttonName){}
		public static bool GetButtonDown(string buttonName){}
		public static bool GetButtonUp(string buttonName){}
		public static bool GetKey(string name){}
		public static bool GetKey(KeyCode key){}
		public static bool GetKeyDown(string name){}
		public static bool GetKeyDown(KeyCode key){}
		public static bool GetKeyUp(string name){}
		public static bool GetKeyUp(KeyCode key){}
		public static String[] GetJoystickNames(){}
		public static bool GetMouseButton(int button){}
		public static bool GetMouseButtonDown(int button){}
		public static bool GetMouseButtonUp(int button){}
		public static void ResetInputAxes(){}
		public static AccelerationEvent GetAccelerationEvent(int index){}
		public static Touch GetTouch(int index){}
		public static Quaternion GetRotation(int deviceID){}
		public static Vector3 GetPosition(int deviceID){}
		private static void INTERNAL_get_compositionCursorPos(out Vector2 value){}
		private static void INTERNAL_set_compositionCursorPos(ref Vector2 value){}
		public Input(){}
		private static Input(){}
		public static bool compensateSensors{ get	{} set	{} }
		public static bool isGyroAvailable{ get	{} }
		public static Gyroscope gyro{ get	{} }
		public static Vector3 mousePosition{ get	{} }
		public static bool anyKey{ get	{} }
		public static bool anyKeyDown{ get	{} }
		public static string inputString{ get	{} }
		public static Vector3 acceleration{ get	{} }
		public static AccelerationEvent[] accelerationEvents{ get	{} }
		public static int accelerationEventCount{ get	{} }
		public static Touch[] touches{ get	{} }
		public static int touchCount{ get	{} }
		public static bool eatKeyPressOnTextFieldFocus{ get	{} set	{} }
		public static bool multiTouchEnabled{ get	{} set	{} }
		public static LocationService location{ get	{} }
		public static Compass compass{ get	{} }
		public static DeviceOrientation deviceOrientation{ get	{} }
		public static IMECompositionMode imeCompositionMode{ get	{} set	{} }
		public static string compositionString{ get	{} }
		public static Vector2 compositionCursorPos{ get	{} set	{} }
		public static bool imeIsSelected{ get	{} }
		private static Gyroscope m_MainGyro;
		private static LocationService locationServiceInstance;
		private static Compass compassInstance;
	}

	public class	Object: Object
	{
		public virtual bool Equals(System.Object o){}
		public virtual int GetHashCode(){}
		private static bool CompareBaseObjects(System.Object lhs, System.Object rhs){}
		public int GetInstanceID(){}
		private static System.Object Internal_CloneSingle(System.Object data){}
		private static System.Object Internal_InstantiateSingle(System.Object data, Vector3 pos, Quaternion rot){}
		private static System.Object INTERNAL_CALL_Internal_InstantiateSingle(System.Object data, ref Vector3 pos, ref Quaternion rot){}
		public static System.Object Instantiate(System.Object original, Vector3 position, Quaternion rotation){}
		public static System.Object Instantiate(System.Object original){}
		private static void CheckNullArgument(System.Object arg, string message){}
		public static void Destroy(System.Object obj, float t){}
		public static void Destroy(System.Object obj){}
		public static void DestroyImmediate(System.Object obj, bool allowDestroyingAssets){}
		public static void DestroyImmediate(System.Object obj){}
		public static Object[] FindObjectsOfType(Type type){}
		public static System.Object FindObjectOfType(Type type){}
		public static void DontDestroyOnLoad(System.Object target){}
		public static void DestroyObject(System.Object obj, float t){}
		public static void DestroyObject(System.Object obj){}
		public static Object[] FindSceneObjectsOfType(Type type){}
		public static Object[] FindObjectsOfTypeIncludingAssets(Type type){}
		public static Object[] FindObjectsOfTypeAll(Type type){}
		public virtual string ToString(){}
		public Object(){}
		public string name{ get	{} set	{} }
		public HideFlags hideFlags{ get	{} set	{} }
		private ReferenceData m_UnityRuntimeReferenceData;
		private string m_UnityRuntimeErrorString;
	}

	public class	Component: Object
	{
		public Component GetComponent(Type type){}
		public T GetComponent(){}
		public Component GetComponent(string type){}
		public Component GetComponentInChildren(Type t){}
		public T GetComponentInChildren(){}
		public Component[] GetComponentsInChildren(Type t){}
		public Component[] GetComponentsInChildren(Type t, bool includeInactive){}
		public T[] GetComponentsInChildren(bool includeInactive){}
		public T[] GetComponentsInChildren(){}
		public Component[] GetComponents(Type type){}
		private Component[] GetComponentsWithCorrectReturnType(Type type){}
		public T[] GetComponents(){}
		public bool CompareTag(string tag){}
		public void SendMessageUpwards(string methodName, System.Object value, SendMessageOptions options){}
		public void SendMessageUpwards(string methodName, System.Object value){}
		public void SendMessageUpwards(string methodName){}
		public void SendMessageUpwards(string methodName, SendMessageOptions options){}
		public void SendMessage(string methodName, System.Object value, SendMessageOptions options){}
		public void SendMessage(string methodName, System.Object value){}
		public void SendMessage(string methodName){}
		public void SendMessage(string methodName, SendMessageOptions options){}
		public void BroadcastMessage(string methodName, System.Object parameter, SendMessageOptions options){}
		public void BroadcastMessage(string methodName, System.Object parameter){}
		public void BroadcastMessage(string methodName){}
		public void BroadcastMessage(string methodName, SendMessageOptions options){}
		public Component(){}
		public Transform transform{ get	{} }
		public Rigidbody rigidbody{ get	{} }
		public Camera camera{ get	{} }
		public Light light{ get	{} }
		public Animation animation{ get	{} }
		public ConstantForce constantForce{ get	{} }
		public Renderer renderer{ get	{} }
		public AudioSource audio{ get	{} }
		public GUIText guiText{ get	{} }
		public NetworkView networkView{ get	{} }
		public GUIElement guiElement{ get	{} }
		public GUITexture guiTexture{ get	{} }
		public Collider collider{ get	{} }
		public HingeJoint hingeJoint{ get	{} }
		public ParticleEmitter particleEmitter{ get	{} }
		public ParticleSystem particleSystem{ get	{} }
		public GameObject gameObject{ get	{} }
		public bool active{ get	{} set	{} }
		public string tag{ get	{} set	{} }
	}

	public sealed class	Light: Behaviour
	{
		private void INTERNAL_get_color(out Color value){}
		private void INTERNAL_set_color(ref Color value){}
		private void INTERNAL_get_areaSize(out Vector2 value){}
		private void INTERNAL_set_areaSize(ref Vector2 value){}
		public static Light[] GetLights(LightType type, int layer){}
		public Light(){}
		public LightType type{ get	{} set	{} }
		public Color color{ get	{} set	{} }
		public float intensity{ get	{} set	{} }
		public LightShadows shadows{ get	{} set	{} }
		public float shadowStrength{ get	{} set	{} }
		public float shadowBias{ get	{} set	{} }
		public float shadowSoftness{ get	{} set	{} }
		public float shadowSoftnessFade{ get	{} set	{} }
		public float range{ get	{} set	{} }
		public float spotAngle{ get	{} set	{} }
		public Texture cookie{ get	{} set	{} }
		public Flare flare{ get	{} set	{} }
		public LightRenderMode renderMode{ get	{} set	{} }
		public int cullingMask{ get	{} set	{} }
		public Vector2 areaSize{ get	{} set	{} }
		public static int pixelLightCount{ get	{} set	{} }
		public float shadowConstantBias{ get	{} set	{} }
		public float shadowObjectSizeBias{ get	{} set	{} }
		public bool attenuate{ get	{} set	{} }
	}

	public sealed class	GameObject: Object
	{
		public void SampleAnimation(AnimationClip animation, float time){}
		public void PlayAnimation(AnimationClip animation){}
		public void StopAnimation(){}
		public static GameObject Find(string name){}
		public static GameObject CreatePrimitive(PrimitiveType type){}
		public Component GetComponent(Type type){}
		public T GetComponent(){}
		public Component GetComponent(string type){}
		private Component GetComponentByName(string type){}
		public Component GetComponentInChildren(Type type){}
		public T GetComponentInChildren(){}
		public Component[] GetComponents(Type type){}
		public T[] GetComponents(){}
		public Component[] GetComponentsInChildren(Type type){}
		public Component[] GetComponentsInChildren(Type type, bool includeInactive){}
		public T[] GetComponentsInChildren(bool includeInactive){}
		public T[] GetComponentsInChildren(){}
		private Component[] GetComponentsInternal(Type type, bool isGenericTypeArray, bool recursive, bool includeInactive){}
		public void SetActive(bool value){}
		public void SetActiveRecursively(bool state){}
		public bool CompareTag(string tag){}
		public static GameObject FindGameObjectWithTag(string tag){}
		public static GameObject FindWithTag(string tag){}
		public static GameObject[] FindGameObjectsWithTag(string tag){}
		public void SendMessageUpwards(string methodName, System.Object value, SendMessageOptions options){}
		public void SendMessageUpwards(string methodName, System.Object value){}
		public void SendMessageUpwards(string methodName){}
		public void SendMessageUpwards(string methodName, SendMessageOptions options){}
		public void SendMessage(string methodName, System.Object value, SendMessageOptions options){}
		public void SendMessage(string methodName, System.Object value){}
		public void SendMessage(string methodName){}
		public void SendMessage(string methodName, SendMessageOptions options){}
		public void BroadcastMessage(string methodName, System.Object parameter, SendMessageOptions options){}
		public void BroadcastMessage(string methodName, System.Object parameter){}
		public void BroadcastMessage(string methodName){}
		public void BroadcastMessage(string methodName, SendMessageOptions options){}
		public Component AddComponent(string className){}
		public Component AddComponent(Type componentType){}
		private Component Internal_AddComponentWithType(Type componentType){}
		public T AddComponent(){}
		private static void Internal_CreateGameObject(GameObject mono, string name){}
		public GameObject(string name){}
		public GameObject(){}
		public GameObject(string name, Type[] components){}
		public bool isStatic{ get	{} set	{} }
		bool isStaticBatchable{ get	{} }
		public Transform transform{ get	{} }
		public Rigidbody rigidbody{ get	{} }
		public Camera camera{ get	{} }
		public Light light{ get	{} }
		public Animation animation{ get	{} }
		public ConstantForce constantForce{ get	{} }
		public Renderer renderer{ get	{} }
		public AudioSource audio{ get	{} }
		public GUIText guiText{ get	{} }
		public NetworkView networkView{ get	{} }
		public GUIElement guiElement{ get	{} }
		public GUITexture guiTexture{ get	{} }
		public Collider collider{ get	{} }
		public HingeJoint hingeJoint{ get	{} }
		public ParticleEmitter particleEmitter{ get	{} }
		public ParticleSystem particleSystem{ get	{} }
		public int layer{ get	{} set	{} }
		public bool active{ get	{} set	{} }
		public bool activeSelf{ get	{} }
		public bool activeInHierarchy{ get	{} }
		public string tag{ get	{} set	{} }
		public GameObject gameObject{ get	{} }
	}

	public sealed class	Transform: Component, IEnumerable
	{
		public bool IsChildOf(Transform parent){}
		public Transform FindChild(string name){}
		public sealed virtual IEnumerator GetEnumerator(){}
		public void RotateAround(Vector3 axis, float angle){}
		private static void INTERNAL_CALL_RotateAround(Transform self, ref Vector3 axis, float angle){}
		public void RotateAroundLocal(Vector3 axis, float angle){}
		private static void INTERNAL_CALL_RotateAroundLocal(Transform self, ref Vector3 axis, float angle){}
		public Transform GetChild(int index){}
		public int GetChildCount(){}
		internal bool IsNonUniformScaleTransform(){}
		private void INTERNAL_get_position(out Vector3 value){}
		private void INTERNAL_set_position(ref Vector3 value){}
		private void INTERNAL_get_localPosition(out Vector3 value){}
		private void INTERNAL_set_localPosition(ref Vector3 value){}
		private void INTERNAL_get_localEulerAngles(out Vector3 value){}
		private void INTERNAL_set_localEulerAngles(ref Vector3 value){}
		private void INTERNAL_get_rotation(out Quaternion value){}
		private void INTERNAL_set_rotation(ref Quaternion value){}
		private void INTERNAL_get_localRotation(out Quaternion value){}
		private void INTERNAL_set_localRotation(ref Quaternion value){}
		private void INTERNAL_get_localScale(out Vector3 value){}
		private void INTERNAL_set_localScale(ref Vector3 value){}
		private void INTERNAL_get_worldToLocalMatrix(out Matrix4x4 value){}
		private void INTERNAL_get_localToWorldMatrix(out Matrix4x4 value){}
		public void Translate(Vector3 translation){}
		public void Translate(Vector3 translation, Space relativeTo){}
		public void Translate(float x, float y, float z){}
		public void Translate(float x, float y, float z, Space relativeTo){}
		public void Translate(Vector3 translation, Transform relativeTo){}
		public void Translate(float x, float y, float z, Transform relativeTo){}
		public void Rotate(Vector3 eulerAngles){}
		public void Rotate(Vector3 eulerAngles, Space relativeTo){}
		public void Rotate(float xAngle, float yAngle, float zAngle){}
		public void Rotate(float xAngle, float yAngle, float zAngle, Space relativeTo){}
		public void Rotate(Vector3 axis, float angle){}
		public void Rotate(Vector3 axis, float angle, Space relativeTo){}
		public void RotateAround(Vector3 point, Vector3 axis, float angle){}
		public void LookAt(Transform target){}
		public void LookAt(Transform target, Vector3 worldUp){}
		public void LookAt(Vector3 worldPosition, Vector3 worldUp){}
		public void LookAt(Vector3 worldPosition){}
		private static void INTERNAL_CALL_LookAt(Transform self, ref Vector3 worldPosition, ref Vector3 worldUp){}
		public Vector3 TransformDirection(Vector3 direction){}
		private static Vector3 INTERNAL_CALL_TransformDirection(Transform self, ref Vector3 direction){}
		public Vector3 TransformDirection(float x, float y, float z){}
		public Vector3 InverseTransformDirection(Vector3 direction){}
		private static Vector3 INTERNAL_CALL_InverseTransformDirection(Transform self, ref Vector3 direction){}
		public Vector3 InverseTransformDirection(float x, float y, float z){}
		public Vector3 TransformPoint(Vector3 position){}
		private static Vector3 INTERNAL_CALL_TransformPoint(Transform self, ref Vector3 position){}
		public Vector3 TransformPoint(float x, float y, float z){}
		public Vector3 InverseTransformPoint(Vector3 position){}
		private static Vector3 INTERNAL_CALL_InverseTransformPoint(Transform self, ref Vector3 position){}
		public Vector3 InverseTransformPoint(float x, float y, float z){}
		public void DetachChildren(){}
		public Transform Find(string name){}
		internal void SendTransformChangedScale(){}
		private void INTERNAL_get_lossyScale(out Vector3 value){}
		private Transform(){}
		public Vector3 position{ get	{} set	{} }
		public Vector3 localPosition{ get	{} set	{} }
		public Vector3 eulerAngles{ get	{} set	{} }
		public Vector3 localEulerAngles{ get	{} set	{} }
		public Vector3 right{ get	{} set	{} }
		public Vector3 up{ get	{} set	{} }
		public Vector3 forward{ get	{} set	{} }
		public Quaternion rotation{ get	{} set	{} }
		public Quaternion localRotation{ get	{} set	{} }
		public Vector3 localScale{ get	{} set	{} }
		public Transform parent{ get	{} set	{} }
		public Matrix4x4 worldToLocalMatrix{ get	{} }
		public Matrix4x4 localToWorldMatrix{ get	{} }
		public Transform root{ get	{} }
		public int childCount{ get	{} }
		public Vector3 lossyScale{ get	{} }
	}

	public sealed class	Time: Object
	{
		public Time(){}
		public static float time{ get	{} }
		public static float timeSinceLevelLoad{ get	{} }
		public static float deltaTime{ get	{} }
		public static float fixedTime{ get	{} }
		public static float fixedDeltaTime{ get	{} set	{} }
		public static float maximumDeltaTime{ get	{} set	{} }
		public static float smoothDeltaTime{ get	{} }
		public static float timeScale{ get	{} set	{} }
		public static int frameCount{ get	{} }
		public static int renderedFrameCount{ get	{} }
		public static float realtimeSinceStartup{ get	{} }
		public static int captureFramerate{ get	{} set	{} }
	}

	public sealed class	Random: Object
	{
		public static float Range(float min, float max){}
		public static int Range(int min, int max){}
		private static int RandomRangeInt(int min, int max){}
		private static void GetRandomUnitCircle(out Vector2 output){}
		public static float RandomRange(float min, float max){}
		public static int RandomRange(int min, int max){}
		public Random(){}
		public static int seed{ get	{} set	{} }
		public static float value{ get	{} }
		public static Vector3 insideUnitSphere{ get	{} }
		public static Vector2 insideUnitCircle{ get	{} }
		public static Vector3 onUnitSphere{ get	{} }
		public static Quaternion rotation{ get	{} }
		public static Quaternion rotationUniform{ get	{} }
	}

	public class	YieldInstruction: Object
	{
		public YieldInstruction(){}
	}

	public sealed class	WebCamTexture: Texture
	{
		private void Internal_CreateWebCamTexture(string device, int requestedWidth, int requestedHeight, int maxFramerate){}
		public void Play(){}
		private static void INTERNAL_CALL_Play(WebCamTexture self){}
		public void Pause(){}
		private static void INTERNAL_CALL_Pause(WebCamTexture self){}
		public void Stop(){}
		private static void INTERNAL_CALL_Stop(WebCamTexture self){}
		public Color GetPixel(int x, int y){}
		public Color[] GetPixels(){}
		public Color[] GetPixels(int x, int y, int blockWidth, int blockHeight){}
		public Color32[] GetPixels32(Color32[] colors){}
		public Color32[] GetPixels32(){}
		public WebCamTexture(string deviceName, int requestedWidth, int requestedHeight, int requestedFPS){}
		public WebCamTexture(string deviceName, int requestedWidth, int requestedHeight){}
		public WebCamTexture(string deviceName){}
		public WebCamTexture(int requestedWidth, int requestedHeight, int requestedFPS){}
		public WebCamTexture(int requestedWidth, int requestedHeight){}
		public WebCamTexture(){}
		public bool isPlaying{ get	{} }
		public string deviceName{ get	{} set	{} }
		public float requestedFPS{ get	{} set	{} }
		public int requestedWidth{ get	{} set	{} }
		public int requestedHeight{ get	{} set	{} }
		public static WebCamDevice[] devices{ get	{} }
		public int videoRotationAngle{ get	{} }
		public bool didUpdateThisFrame{ get	{} }
	}

	public class	AndroidJNIHelper: Object
	{
		public static IntPtr GetConstructorID(IntPtr javaClass, string signature){}
		public static IntPtr GetConstructorID(IntPtr javaClass){}
		public static IntPtr GetMethodID(IntPtr javaClass, string methodName, string signature, bool isStatic){}
		public static IntPtr GetMethodID(IntPtr javaClass, string methodName, string signature){}
		public static IntPtr GetMethodID(IntPtr javaClass, string methodName){}
		public static IntPtr GetFieldID(IntPtr javaClass, string fieldName, string signature, bool isStatic){}
		public static IntPtr GetFieldID(IntPtr javaClass, string fieldName, string signature){}
		public static IntPtr GetFieldID(IntPtr javaClass, string fieldName){}
		public static IntPtr CreateJavaRunnable(AndroidJavaRunnable runnable){}
		public static IntPtr ConvertToJNIArray(Array array){}
		public static jvalue[] CreateJNIArgArray(Object[] args){}
		public static IntPtr GetConstructorID(IntPtr jclass, Object[] args){}
		public static IntPtr GetMethodID(IntPtr jclass, string methodName, Object[] args, bool isStatic){}
		public static string GetSignature(System.Object obj){}
		public static string GetSignature(Object[] args){}
		public static ArrayType ConvertFromJNIArray(IntPtr array){}
		public static IntPtr GetMethodID(IntPtr jclass, string methodName, Object[] args, bool isStatic){}
		public static IntPtr GetFieldID(IntPtr jclass, string fieldName, bool isStatic){}
		public static string GetSignature(Object[] args){}
		public AndroidJNIHelper(){}
		public static bool debug{ get	{} set	{} }
	}

	public sealed class	AndroidJNI: Object
	{
		public static char GetCharArrayElement(IntPtr array, int index){}
		public static short GetShortArrayElement(IntPtr array, int index){}
		public static int GetIntArrayElement(IntPtr array, int index){}
		public static long GetLongArrayElement(IntPtr array, int index){}
		public static float GetFloatArrayElement(IntPtr array, int index){}
		public static double GetDoubleArrayElement(IntPtr array, int index){}
		public static IntPtr GetObjectArrayElement(IntPtr array, int index){}
		public static void SetBooleanArrayElement(IntPtr array, int index, byte val){}
		public static void SetByteArrayElement(IntPtr array, int index, sbyte val){}
		public static void SetCharArrayElement(IntPtr array, int index, char val){}
		public static void SetShortArrayElement(IntPtr array, int index, short val){}
		public static void SetIntArrayElement(IntPtr array, int index, int val){}
		public static void SetLongArrayElement(IntPtr array, int index, long val){}
		public static void SetFloatArrayElement(IntPtr array, int index, float val){}
		public static void SetDoubleArrayElement(IntPtr array, int index, double val){}
		public static void SetObjectArrayElement(IntPtr array, int index, IntPtr obj){}
		public static void SetLongField(IntPtr obj, IntPtr fieldID, long val){}
		public static void SetFloatField(IntPtr obj, IntPtr fieldID, float val){}
		public static void SetDoubleField(IntPtr obj, IntPtr fieldID, double val){}
		public static string CallStaticStringMethod(IntPtr clazz, IntPtr methodID, jvalue[] args){}
		public static IntPtr CallStaticObjectMethod(IntPtr clazz, IntPtr methodID, jvalue[] args){}
		public static int CallStaticIntMethod(IntPtr clazz, IntPtr methodID, jvalue[] args){}
		public static bool CallStaticBooleanMethod(IntPtr clazz, IntPtr methodID, jvalue[] args){}
		public static short CallStaticShortMethod(IntPtr clazz, IntPtr methodID, jvalue[] args){}
		public static byte CallStaticByteMethod(IntPtr clazz, IntPtr methodID, jvalue[] args){}
		public static char CallStaticCharMethod(IntPtr clazz, IntPtr methodID, jvalue[] args){}
		public static float CallStaticFloatMethod(IntPtr clazz, IntPtr methodID, jvalue[] args){}
		public static double CallStaticDoubleMethod(IntPtr clazz, IntPtr methodID, jvalue[] args){}
		public static long CallStaticLongMethod(IntPtr clazz, IntPtr methodID, jvalue[] args){}
		public static void CallStaticVoidMethod(IntPtr clazz, IntPtr methodID, jvalue[] args){}
		public static string GetStaticStringField(IntPtr clazz, IntPtr fieldID){}
		public static IntPtr GetStaticObjectField(IntPtr clazz, IntPtr fieldID){}
		public static bool GetStaticBooleanField(IntPtr clazz, IntPtr fieldID){}
		public static byte GetStaticByteField(IntPtr clazz, IntPtr fieldID){}
		public static char GetStaticCharField(IntPtr clazz, IntPtr fieldID){}
		public static short GetStaticShortField(IntPtr clazz, IntPtr fieldID){}
		public static long GetStaticIntField(IntPtr clazz, IntPtr fieldID){}
		public static long GetStaticLongField(IntPtr clazz, IntPtr fieldID){}
		public static float GetStaticFloatField(IntPtr clazz, IntPtr fieldID){}
		public static double GetStaticDoubleField(IntPtr clazz, IntPtr fieldID){}
		public static void SetStaticStringField(IntPtr clazz, IntPtr fieldID, string val){}
		public static void SetStaticObjectField(IntPtr clazz, IntPtr fieldID, IntPtr val){}
		public static void SetStaticBooleanField(IntPtr clazz, IntPtr fieldID, bool val){}
		public static void SetStaticByteField(IntPtr clazz, IntPtr fieldID, byte val){}
		public static void SetStaticCharField(IntPtr clazz, IntPtr fieldID, char val){}
		public static void SetStaticShortField(IntPtr clazz, IntPtr fieldID, short val){}
		public static void SetStaticIntField(IntPtr clazz, IntPtr fieldID, int val){}
		public static void SetStaticLongField(IntPtr clazz, IntPtr fieldID, long val){}
		public static void SetStaticFloatField(IntPtr clazz, IntPtr fieldID, float val){}
		public static void SetStaticDoubleField(IntPtr clazz, IntPtr fieldID, double val){}
		public static IntPtr ToBooleanArray(Boolean[] array){}
		public static IntPtr ToByteArray(Byte[] array){}
		public static IntPtr ToCharArray(Char[] array){}
		public static IntPtr ToShortArray(Int16[] array){}
		public static IntPtr ToIntArray(Int32[] array){}
		public static IntPtr ToLongArray(Int64[] array){}
		public static IntPtr ToFloatArray(Single[] array){}
		public static IntPtr ToDoubleArray(Double[] array){}
		public static IntPtr ToObjectArray(IntPtr[] array){}
		public static Boolean[] FromBooleanArray(IntPtr array){}
		public static Byte[] FromByteArray(IntPtr array){}
		public static Char[] FromCharArray(IntPtr array){}
		public static Int16[] FromShortArray(IntPtr array){}
		public static Int32[] FromIntArray(IntPtr array){}
		public static Int64[] FromLongArray(IntPtr array){}
		public static Single[] FromFloatArray(IntPtr array){}
		public static Double[] FromDoubleArray(IntPtr array){}
		public static IntPtr[] FromObjectArray(IntPtr array){}
		public static int GetArrayLength(IntPtr array){}
		public static IntPtr NewBooleanArray(int size){}
		public static IntPtr NewByteArray(int size){}
		public static IntPtr NewCharArray(int size){}
		public static IntPtr NewShortArray(int size){}
		public static IntPtr NewIntArray(int size){}
		public static IntPtr NewLongArray(int size){}
		public static IntPtr NewFloatArray(int size){}
		public static IntPtr NewDoubleArray(int size){}
		public static IntPtr NewObjectArray(int size, IntPtr clazz, IntPtr obj){}
		public static bool GetBooleanArrayElement(IntPtr array, int index){}
		public static byte GetByteArrayElement(IntPtr array, int index){}
		public static int AttachCurrentThread(){}
		public static int DetachCurrentThread(){}
		public static int GetVersion(){}
		public static IntPtr FindClass(string name){}
		public static IntPtr FromReflectedMethod(IntPtr refMethod){}
		public static IntPtr FromReflectedField(IntPtr refField){}
		public static IntPtr ToReflectedMethod(IntPtr clazz, IntPtr methodID, bool isStatic){}
		public static IntPtr ToReflectedField(IntPtr clazz, IntPtr fieldID, bool isStatic){}
		public static IntPtr GetSuperclass(IntPtr clazz){}
		public static bool IsAssignableFrom(IntPtr clazz1, IntPtr clazz2){}
		public static int Throw(IntPtr obj){}
		public static int ThrowNew(IntPtr clazz, string message){}
		public static IntPtr ExceptionOccurred(){}
		public static void ExceptionDescribe(){}
		public static void ExceptionClear(){}
		public static void FatalError(string message){}
		public static int PushLocalFrame(int capacity){}
		public static IntPtr PopLocalFrame(IntPtr result){}
		public static IntPtr NewGlobalRef(IntPtr obj){}
		public static void DeleteGlobalRef(IntPtr obj){}
		public static IntPtr NewLocalRef(IntPtr obj){}
		public static void DeleteLocalRef(IntPtr obj){}
		public static bool IsSameObject(IntPtr obj1, IntPtr obj2){}
		public static int EnsureLocalCapacity(int capacity){}
		public static IntPtr AllocObject(IntPtr clazz){}
		public static IntPtr NewObject(IntPtr clazz, IntPtr methodID, jvalue[] args){}
		public static IntPtr GetObjectClass(IntPtr obj){}
		public static bool IsInstanceOf(IntPtr obj, IntPtr clazz){}
		public static IntPtr GetMethodID(IntPtr clazz, string name, string sig){}
		public static IntPtr GetFieldID(IntPtr clazz, string name, string sig){}
		public static IntPtr GetStaticMethodID(IntPtr clazz, string name, string sig){}
		public static IntPtr GetStaticFieldID(IntPtr clazz, string name, string sig){}
		public static IntPtr NewStringUTF(string bytes){}
		public static int GetStringUTFLength(IntPtr str){}
		public static string GetStringUTFChars(IntPtr str){}
		public static string CallStringMethod(IntPtr obj, IntPtr methodID, jvalue[] args){}
		public static IntPtr CallObjectMethod(IntPtr obj, IntPtr methodID, jvalue[] args){}
		public static int CallIntMethod(IntPtr obj, IntPtr methodID, jvalue[] args){}
		public static bool CallBooleanMethod(IntPtr obj, IntPtr methodID, jvalue[] args){}
		public static short CallShortMethod(IntPtr obj, IntPtr methodID, jvalue[] args){}
		public static byte CallByteMethod(IntPtr obj, IntPtr methodID, jvalue[] args){}
		public static char CallCharMethod(IntPtr obj, IntPtr methodID, jvalue[] args){}
		public static float CallFloatMethod(IntPtr obj, IntPtr methodID, jvalue[] args){}
		public static double CallDoubleMethod(IntPtr obj, IntPtr methodID, jvalue[] args){}
		public static long CallLongMethod(IntPtr obj, IntPtr methodID, jvalue[] args){}
		public static void CallVoidMethod(IntPtr obj, IntPtr methodID, jvalue[] args){}
		public static string GetStringField(IntPtr obj, IntPtr fieldID){}
		public static IntPtr GetObjectField(IntPtr obj, IntPtr fieldID){}
		public static bool GetBooleanField(IntPtr obj, IntPtr fieldID){}
		public static byte GetByteField(IntPtr obj, IntPtr fieldID){}
		public static char GetCharField(IntPtr obj, IntPtr fieldID){}
		public static short GetShortField(IntPtr obj, IntPtr fieldID){}
		public static int GetIntField(IntPtr obj, IntPtr fieldID){}
		public static long GetLongField(IntPtr obj, IntPtr fieldID){}
		public static float GetFloatField(IntPtr obj, IntPtr fieldID){}
		public static double GetDoubleField(IntPtr obj, IntPtr fieldID){}
		public static void SetStringField(IntPtr obj, IntPtr fieldID, string val){}
		public static void SetObjectField(IntPtr obj, IntPtr fieldID, IntPtr val){}
		public static void SetBooleanField(IntPtr obj, IntPtr fieldID, bool val){}
		public static void SetByteField(IntPtr obj, IntPtr fieldID, byte val){}
		public static void SetCharField(IntPtr obj, IntPtr fieldID, char val){}
		public static void SetShortField(IntPtr obj, IntPtr fieldID, short val){}
		public static void SetIntField(IntPtr obj, IntPtr fieldID, int val){}
		public AndroidJNI(){}
	}

	public sealed class	NavMeshAgent: Behaviour
	{
		public bool SetDestination(Vector3 target){}
		private static bool INTERNAL_CALL_SetDestination(NavMeshAgent self, ref Vector3 target){}
		private void INTERNAL_get_destination(out Vector3 value){}
		private void INTERNAL_set_destination(ref Vector3 value){}
		private void INTERNAL_get_velocity(out Vector3 value){}
		private void INTERNAL_set_velocity(ref Vector3 value){}
		private void INTERNAL_get_nextPosition(out Vector3 value){}
		private void INTERNAL_set_nextPosition(ref Vector3 value){}
		private void INTERNAL_get_steeringTarget(out Vector3 value){}
		private void INTERNAL_get_desiredVelocity(out Vector3 value){}
		public void ActivateCurrentOffMeshLink(bool activated){}
		internal OffMeshLinkData GetCurrentOffMeshLinkDataInternal(){}
		internal OffMeshLinkData GetNextOffMeshLinkDataInternal(){}
		public void CompleteOffMeshLink(){}
		private void INTERNAL_get_pathEndPosition(out Vector3 value){}
		public void Move(Vector3 offset){}
		private static void INTERNAL_CALL_Move(NavMeshAgent self, ref Vector3 offset){}
		public void Stop(bool stopUpdates){}
		public void Stop(){}
		public void Resume(){}
		public void ResetPath(){}
		public bool SetPath(NavMeshPath path){}
		internal void CopyPathTo(NavMeshPath path){}
		public bool FindClosestEdge(out NavMeshHit hit){}
		public bool Raycast(Vector3 targetPosition, out NavMeshHit hit){}
		private static bool INTERNAL_CALL_Raycast(NavMeshAgent self, ref Vector3 targetPosition, out NavMeshHit hit){}
		public bool CalculatePath(Vector3 targetPosition, NavMeshPath path){}
		private bool CalculatePathInternal(Vector3 targetPosition, NavMeshPath path){}
		private static bool INTERNAL_CALL_CalculatePathInternal(NavMeshAgent self, ref Vector3 targetPosition, NavMeshPath path){}
		public bool SamplePathPosition(int passableMask, float maxDistance, out NavMeshHit hit){}
		public void SetLayerCost(int layer, float cost){}
		public float GetLayerCost(int layer){}
		public NavMeshAgent(){}
		public Vector3 destination{ get	{} set	{} }
		public float stoppingDistance{ get	{} set	{} }
		public Vector3 velocity{ get	{} set	{} }
		public Vector3 nextPosition{ get	{} set	{} }
		public Vector3 steeringTarget{ get	{} }
		public Vector3 desiredVelocity{ get	{} }
		public float remainingDistance{ get	{} }
		public float baseOffset{ get	{} set	{} }
		public bool isOnOffMeshLink{ get	{} }
		public OffMeshLinkData currentOffMeshLinkData{ get	{} }
		public OffMeshLinkData nextOffMeshLinkData{ get	{} }
		public bool autoTraverseOffMeshLink{ get	{} set	{} }
		public bool autoRepath{ get	{} set	{} }
		public bool hasPath{ get	{} }
		public bool pathPending{ get	{} }
		public bool isPathStale{ get	{} }
		public NavMeshPathStatus pathStatus{ get	{} }
		public Vector3 pathEndPosition{ get	{} }
		public NavMeshPath path{ get	{} set	{} }
		public int walkableMask{ get	{} set	{} }
		public float speed{ get	{} set	{} }
		public float angularSpeed{ get	{} set	{} }
		public float acceleration{ get	{} set	{} }
		public bool updatePosition{ get	{} set	{} }
		public bool updateRotation{ get	{} set	{} }
		public float radius{ get	{} set	{} }
		public float height{ get	{} set	{} }
		public ObstacleAvoidanceType obstacleAvoidanceType{ get	{} set	{} }
		public int avoidancePriority{ get	{} set	{} }
	}

	public sealed class	NavMesh: Object
	{
		public static bool Raycast(Vector3 sourcePosition, Vector3 targetPosition, out NavMeshHit hit, int passableMask){}
		private static bool INTERNAL_CALL_Raycast(ref Vector3 sourcePosition, ref Vector3 targetPosition, out NavMeshHit hit, int passableMask){}
		public static bool CalculatePath(Vector3 sourcePosition, Vector3 targetPosition, int passableMask, NavMeshPath path){}
		private static bool CalculatePathInternal(Vector3 sourcePosition, Vector3 targetPosition, int passableMask, NavMeshPath path){}
		private static bool INTERNAL_CALL_CalculatePathInternal(ref Vector3 sourcePosition, ref Vector3 targetPosition, int passableMask, NavMeshPath path){}
		public static bool FindClosestEdge(Vector3 sourcePosition, out NavMeshHit hit, int passableMask){}
		private static bool INTERNAL_CALL_FindClosestEdge(ref Vector3 sourcePosition, out NavMeshHit hit, int passableMask){}
		public static bool SamplePosition(Vector3 sourcePosition, out NavMeshHit hit, float maxDistance, int allowedMask){}
		private static bool INTERNAL_CALL_SamplePosition(ref Vector3 sourcePosition, out NavMeshHit hit, float maxDistance, int allowedMask){}
		public static void SetLayerCost(int layer, float cost){}
		public static float GetLayerCost(int layer){}
		public static int GetNavMeshLayerFromName(string layerName){}
		public static void Triangulate(out Vector3[] vertices, out Int32[] indices){}
		public NavMesh(){}
	}

	public sealed class	OffMeshLink: Component
	{
		public OffMeshLink(){}
		public bool activated{ get	{} set	{} }
		public bool occupied{ get	{} }
		public float costOverride{ get	{} set	{} }
	}

	public sealed class	NavMeshPath: Object
	{
		private void DestroyNavMeshPath(){}
		protected virtual void Finalize(){}
		private Vector3[] CalculateCornersInternal(){}
		private void ClearCornersInternal(){}
		public void ClearCorners(){}
		private void CalculateCorners(){}
		public NavMeshPath(){}
		public Vector3[] corners{ get	{} }
		public NavMeshPathStatus status{ get	{} }
		private IntPtr m_Ptr;
		private Vector3[] m_corners;
	}

	public sealed class	NavMeshObstacle: Behaviour
	{
		private void INTERNAL_get_velocity(out Vector3 value){}
		private void INTERNAL_set_velocity(ref Vector3 value){}
		public NavMeshObstacle(){}
		public float height{ get	{} set	{} }
		public float radius{ get	{} set	{} }
		public Vector3 velocity{ get	{} set	{} }
	}

	public sealed class	AndroidInput: Object
	{
		public static Touch GetSecondaryTouch(int index){}
		public AndroidInput(){}
		public static int touchCountSecondary{ get	{} }
		public static bool secondaryTouchEnabled{ get	{} }
		public static int secondaryTouchWidth{ get	{} }
		public static int secondaryTouchHeight{ get	{} }
	}

	public abstract	class	PropertyAttribute: Attribute, _Attribute
	{
		protected PropertyAttribute(){}
	}

	public sealed class	RangeAttribute: PropertyAttribute, _Attribute
	{
		public RangeAttribute(float min, float max){}
		public readonly float min;
		public readonly float max;
	}

	public sealed class	MultilineAttribute: PropertyAttribute, _Attribute
	{
		public MultilineAttribute(){}
		public MultilineAttribute(int lines){}
		public readonly int lines;
	}

	public class	Motion: Object
	{
		public bool ValidateIfRetargetable(bool showWarning){}
		public Motion(){}
		public float averageDuration{ get	{} }
		public float averageAngularSpeed{ get	{} }
		public Vector3 averageSpeed{ get	{} }
		public float apparentSpeed{ get	{} }
		public bool isLooping{ get	{} }
		public bool isAnimatorMotion{ get	{} }
		public bool isHumanMotion{ get	{} }
	}

	delegate void PCMReaderCallback(Single[] data);

	delegate void PCMSetPositionCallback(int position);

	delegate void FontTextureRebuildCallback();

	delegate void WindowFunction(int id);

	delegate void SkinChangedDelegate();

	delegate void LogCallback(string condition, string stackTrace, LogType type);

	delegate void AndroidJavaRunnable();

}

namespace AOT {
	public class	MonoPInvokeCallbackAttribute: Attribute, _Attribute
	{
		public MonoPInvokeCallbackAttribute(Type type){}
	}

}

namespace UnityEngine.SocialPlatforms.GameCenter {
	public class	GameCenterPlatform: Local, ISocialPlatform
	{
		public static void ResetAllAchievements(Action<Boolean> callback){}
		public static void ShowDefaultAchievementCompletionBanner(bool value){}
		public static void ShowLeaderboardUI(string leaderboardID, TimeScope timeScope){}
		public GameCenterPlatform(){}
	}

}

namespace UnityEngine.SocialPlatforms.Impl {
	public class	LocalUser: UserProfile, IUserProfile, ILocalUser
	{
		public sealed virtual void Authenticate(Action<Boolean> callback){}
		public sealed virtual void LoadFriends(Action<Boolean> callback){}
		public void SetFriends(IUserProfile[] friends){}
		public void SetAuthenticated(bool value){}
		public void SetUnderage(bool value){}
		public LocalUser(){}
		public sealed virtual IUserProfile[] friends{ get	{} }
		public sealed virtual bool authenticated{ get	{} }
		public sealed virtual bool underage{ get	{} }
		private IUserProfile[] m_Friends;
		private bool m_Authenticated;
		private bool m_Underage;
	}

	public class	UserProfile: Object, IUserProfile
	{
		public virtual string ToString(){}
		public void SetUserName(string name){}
		public void SetUserID(string id){}
		public void SetImage(Texture2D image){}
		public void SetIsFriend(bool value){}
		public void SetState(UserState state){}
		public UserProfile(){}
		public UserProfile(string name, string id, bool friend){}
		public UserProfile(string name, string id, bool friend, UserState state, Texture2D image){}
		public sealed virtual string userName{ get	{} }
		public sealed virtual string id{ get	{} }
		public sealed virtual bool isFriend{ get	{} }
		public sealed virtual UserState state{ get	{} }
		public sealed virtual Texture2D image{ get	{} }
		protected string m_UserName;
		protected string m_ID;
		protected bool m_IsFriend;
		protected UserState m_State;
		protected Texture2D m_Image;
	}

	public class	Achievement: Object, IAchievement
	{
		public virtual string ToString(){}
		public sealed virtual void ReportProgress(Action<Boolean> callback){}
		public void SetCompleted(bool value){}
		public void SetHidden(bool value){}
		public void SetLastReportedDate(DateTime date){}
		public Achievement(string id, double percentCompleted, bool completed, bool hidden, DateTime lastReportedDate){}
		public Achievement(string id, double percent){}
		public Achievement(){}
		public sealed virtual string id{ get	{} set	{} }
		public sealed virtual double percentCompleted{ get	{} set	{} }
		public sealed virtual bool completed{ get	{} }
		public sealed virtual bool hidden{ get	{} }
		public sealed virtual DateTime lastReportedDate{ get	{} }
		private bool m_Completed;
		private bool m_Hidden;
		private DateTime m_LastReportedDate;
		private string <id>k__BackingField;
		private double <percentCompleted>k__BackingField;
	}

	public class	AchievementDescription: Object, IAchievementDescription
	{
		public virtual string ToString(){}
		public void SetImage(Texture2D image){}
		public AchievementDescription(string id, string title, Texture2D image, string achievedDescription, string unachievedDescription, bool hidden, int points){}
		public sealed virtual string id{ get	{} set	{} }
		public sealed virtual string title{ get	{} }
		public sealed virtual Texture2D image{ get	{} }
		public sealed virtual string achievedDescription{ get	{} }
		public sealed virtual string unachievedDescription{ get	{} }
		public sealed virtual bool hidden{ get	{} }
		public sealed virtual int points{ get	{} }
		private string m_Title;
		private Texture2D m_Image;
		private string m_AchievedDescription;
		private string m_UnachievedDescription;
		private bool m_Hidden;
		private int m_Points;
		private string <id>k__BackingField;
	}

	public class	Score: Object, IScore
	{
		public virtual string ToString(){}
		public sealed virtual void ReportScore(Action<Boolean> callback){}
		public void SetDate(DateTime date){}
		public void SetFormattedValue(string value){}
		public void SetUserID(string userID){}
		public void SetRank(int rank){}
		public Score(){}
		public Score(string leaderboardID, long value){}
		public Score(string leaderboardID, long value, string userID, DateTime date, string formattedValue, int rank){}
		public sealed virtual string leaderboardID{ get	{} set	{} }
		public sealed virtual long value{ get	{} set	{} }
		public sealed virtual DateTime date{ get	{} }
		public sealed virtual string formattedValue{ get	{} }
		public sealed virtual string userID{ get	{} }
		public sealed virtual int rank{ get	{} }
		private DateTime m_Date;
		private string m_FormattedValue;
		private string m_UserID;
		private int m_Rank;
		private string <leaderboardID>k__BackingField;
		private long <value>k__BackingField;
	}

	public class	Leaderboard: Object, ILeaderboard
	{
		public sealed virtual void SetUserFilter(String[] userIDs){}
		public virtual string ToString(){}
		public sealed virtual void LoadScores(Action<Boolean> callback){}
		public void SetLocalUserScore(IScore score){}
		public void SetMaxRange(uint maxRange){}
		public void SetScores(IScore[] scores){}
		public void SetTitle(string title){}
		public String[] GetUserFilter(){}
		public Leaderboard(){}
		public sealed virtual bool loading{ get	{} }
		public sealed virtual string id{ get	{} set	{} }
		public sealed virtual UserScope userScope{ get	{} set	{} }
		public sealed virtual Range range{ get	{} set	{} }
		public sealed virtual TimeScope timeScope{ get	{} set	{} }
		public sealed virtual IScore localUserScore{ get	{} }
		public sealed virtual uint maxRange{ get	{} }
		public sealed virtual IScore[] scores{ get	{} }
		public sealed virtual string title{ get	{} }
		private bool m_Loading;
		private IScore m_LocalUserScore;
		private uint m_MaxRange;
		private IScore[] m_Scores;
		private string m_Title;
		private String[] m_UserIDs;
		private string <id>k__BackingField;
		private UserScope <userScope>k__BackingField;
		private Range <range>k__BackingField;
		private TimeScope <timeScope>k__BackingField;
	}

}

namespace UnityEngine.SocialPlatforms {
	public class	Local: Object, ISocialPlatform
	{
		private sealed virtual void UnityEngine.SocialPlatforms.ISocialPlatform.Authenticate(ILocalUser user, Action<Boolean> callback){}
		private sealed virtual void UnityEngine.SocialPlatforms.ISocialPlatform.LoadFriends(ILocalUser user, Action<Boolean> callback){}
		private sealed virtual void UnityEngine.SocialPlatforms.ISocialPlatform.LoadScores(ILeaderboard board, Action<Boolean> callback){}
		private sealed virtual bool UnityEngine.SocialPlatforms.ISocialPlatform.GetLoading(ILeaderboard board){}
		public sealed virtual void LoadUsers(String[] userIDs, Action<IUserProfile[]> callback){}
		public sealed virtual void ReportProgress(string id, double progress, Action<Boolean> callback){}
		public sealed virtual void LoadAchievementDescriptions(Action<IAchievementDescription[]> callback){}
		public sealed virtual void LoadAchievements(Action<IAchievement[]> callback){}
		public sealed virtual void ReportScore(long score, string board, Action<Boolean> callback){}
		public sealed virtual void LoadScores(string leaderboardID, Action<IScore[]> callback){}
		private void SortScores(Leaderboard board){}
		private void SetLocalPlayerScore(Leaderboard board){}
		public sealed virtual void ShowAchievementsUI(){}
		public sealed virtual void ShowLeaderboardUI(){}
		public sealed virtual ILeaderboard CreateLeaderboard(){}
		public sealed virtual IAchievement CreateAchievement(){}
		private bool VerifyUser(){}
		private void PopulateStaticData(){}
		private Texture2D CreateDummyTexture(int width, int height){}
		private static int <SortScores>m__0(Score s1, Score s2){}
		public Local(){}
		private static Local(){}
		public sealed virtual ILocalUser localUser{ get	{} }
		private List<UserProfile> m_Friends;
		private List<UserProfile> m_Users;
		private List<AchievementDescription> m_AchievementDescriptions;
		private List<Achievement> m_Achievements;
		private List<Leaderboard> m_Leaderboards;
		private Texture2D m_DefaultTexture;
		private static LocalUser m_LocalUser;
		private static Comparison<Score> <>f__am$cache7;
	}

	public interface ISocialPlatform	{
		void LoadUsers(String[] userIDs, Action<IUserProfile[]> callback);
		void ReportProgress(string achievementID, double progress, Action<Boolean> callback);
		void LoadAchievementDescriptions(Action<IAchievementDescription[]> callback);
		void LoadAchievements(Action<IAchievement[]> callback);
		IAchievement CreateAchievement();
		void ReportScore(long score, string board, Action<Boolean> callback);
		void LoadScores(string leaderboardID, Action<IScore[]> callback);
		ILeaderboard CreateLeaderboard();
		void ShowAchievementsUI();
		void ShowLeaderboardUI();
		void Authenticate(ILocalUser user, Action<Boolean> callback);
		void LoadFriends(ILocalUser user, Action<Boolean> callback);
		void LoadScores(ILeaderboard board, Action<Boolean> callback);
		bool GetLoading(ILeaderboard board);
		ILocalUser localUser{ get; }
	}

	public interface ILocalUser: IUserProfile
	{
		void Authenticate(Action<Boolean> callback);
		void LoadFriends(Action<Boolean> callback);
		IUserProfile[] friends{ get; }
		bool authenticated{ get; }
		bool underage{ get; }
	}

	public interface IUserProfile	{
		string userName{ get; }
		string id{ get; }
		bool isFriend{ get; }
		UserState state{ get; }
		Texture2D image{ get; }
	}

	public interface IAchievement	{
		void ReportProgress(Action<Boolean> callback);
		string id{ get; set; }
		double percentCompleted{ get; set; }
		bool completed{ get; }
		bool hidden{ get; }
		DateTime lastReportedDate{ get; }
	}

	public interface IAchievementDescription	{
		string id{ get; set; }
		string title{ get; }
		Texture2D image{ get; }
		string achievedDescription{ get; }
		string unachievedDescription{ get; }
		bool hidden{ get; }
		int points{ get; }
	}

	public interface IScore	{
		void ReportScore(Action<Boolean> callback);
		string leaderboardID{ get; set; }
		long value{ get; set; }
		DateTime date{ get; }
		string formattedValue{ get; }
		string userID{ get; }
		int rank{ get; }
	}

	public interface ILeaderboard	{
		void SetUserFilter(String[] userIDs);
		void LoadScores(Action<Boolean> callback);
		bool loading{ get; }
		string id{ get; set; }
		UserScope userScope{ get; set; }
		Range range{ get; set; }
		TimeScope timeScope{ get; set; }
		IScore localUserScore{ get; }
		uint maxRange{ get; }
		IScore[] scores{ get; }
		string title{ get; }
	}

}

namespace UnityEngine.Serialization {
	public class	UnitySurrogateSelector: Object, ISurrogateSelector
	{
		public sealed virtual ISerializationSurrogate GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector){}
		public sealed virtual void ChainSelector(ISurrogateSelector selector){}
		public sealed virtual ISurrogateSelector GetNextSelector(){}
		public UnitySurrogateSelector(){}
	}

}

namespace UnityEngineInternal {
	public class	TypeInferenceRuleAttribute: Attribute, _Attribute
	{
		public virtual string ToString(){}
		public TypeInferenceRuleAttribute(TypeInferenceRules rule){}
		public TypeInferenceRuleAttribute(string rule){}
		private readonly string _rule;
	}

	public sealed class	Reproduction: Object
	{
		public static void CaptureScreenshot(){}
		public Reproduction(){}
	}

}

namespace UnityEngine.Flash {
	public sealed class	ActionScript: Object
	{
		public static void Import(string package){}
		public static void Statement(string formatString, Object[] arguments){}
		public static T Expression(string formatString, Object[] arguments){}
		public ActionScript(){}
	}

	public sealed class	FlashPlayer: Object
	{
		internal static string GetUnityAppConstants(string name){}
		public FlashPlayer(){}
		public static string TargetVersion{ get	{} }
		public static string TargetSwfVersion{ get	{} }
	}

}

