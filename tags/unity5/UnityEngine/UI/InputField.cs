namespace UnityEngine.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.Serialization;

    [AddComponentMenu("UI/Input Field", 0x1f)]
    public class InputField : Selectable, IEventSystemHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IUpdateSelectedHandler, ISubmitHandler, ICanvasElement
    {
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map0;
        private RectTransform caretRectTrans;
        private const string kEmailSpecialCharacters = "!#$%&'*+-/=?^_`{|}~";
        private const float kHScrollSpeed = 0.05f;
        private static readonly char[] kSeparators = new char[] { ' ', '.', ',' };
        private const float kVScrollSpeed = 0.1f;
        private bool m_AllowInput;
        [FormerlySerializedAs("asteriskChar"), SerializeField]
        private char m_AsteriskChar = '*';
        private Coroutine m_BlinkCoroutine;
        private float m_BlinkStartTime;
        private CanvasRenderer m_CachedInputRenderer;
        [SerializeField, Range(0f, 4f)]
        private float m_CaretBlinkRate = 0.85f;
        protected int m_CaretPosition;
        protected int m_CaretSelectPosition;
        protected bool m_CaretVisible;
        [SerializeField, FormerlySerializedAs("characterLimit")]
        private int m_CharacterLimit;
        [SerializeField, FormerlySerializedAs("validation")]
        private CharacterValidation m_CharacterValidation;
        [SerializeField]
        private ContentType m_ContentType;
        protected UIVertex[] m_CursorVerts;
        private Coroutine m_DragCoroutine;
        private bool m_DragPositionOutOfBounds;
        protected int m_DrawEnd;
        protected int m_DrawStart;
        [FormerlySerializedAs("onSubmit"), FormerlySerializedAs("m_OnSubmit"), SerializeField]
        private SubmitEvent m_EndEdit = new SubmitEvent();
        private bool m_HasDoneFocusTransition;
        [SerializeField, FormerlySerializedAs("hideMobileInput")]
        private bool m_HideMobileInput;
        private TextGenerator m_InputTextCache;
        [FormerlySerializedAs("inputType"), SerializeField]
        private InputType m_InputType;
        protected static TouchScreenKeyboard m_Keyboard;
        [SerializeField, FormerlySerializedAs("keyboardType")]
        private TouchScreenKeyboardType m_KeyboardType;
        [SerializeField]
        private LineType m_LineType;
        [NonSerialized]
        protected Mesh m_Mesh;
        [SerializeField, FormerlySerializedAs("onValidateInput")]
        private OnValidateInput m_OnValidateInput;
        [SerializeField, FormerlySerializedAs("onValueChange")]
        private OnChangeEvent m_OnValueChange = new OnChangeEvent();
        private string m_OriginalText = string.Empty;
        [SerializeField]
        protected Graphic m_Placeholder;
        private bool m_PreventFontCallback;
        private Event m_ProcessingEvent = new Event();
        [SerializeField, FormerlySerializedAs("selectionColor")]
        private Color m_SelectionColor = new Color(0.6588235f, 0.8078431f, 1f, 0.7529412f);
        private bool m_ShouldActivateNextUpdate;
        [FormerlySerializedAs("mValue"), SerializeField]
        protected string m_Text = string.Empty;
        [FormerlySerializedAs("text"), SerializeField]
        protected Text m_TextComponent;
        private bool m_UpdateDrag;
        private bool m_WasCanceled;

        protected InputField()
        {
        }

        public void ActivateInputField()
        {
            if (((this.m_TextComponent != null) && (this.m_TextComponent.font != null)) && (this.IsActive() && this.IsInteractable()))
            {
                if ((this.isFocused && (m_Keyboard != null)) && !m_Keyboard.active)
                {
                    m_Keyboard.active = true;
                    m_Keyboard.text = this.m_Text;
                }
                this.m_ShouldActivateNextUpdate = true;
            }
        }

        private void ActivateInputFieldInternal()
        {
            if (EventSystem.current.currentSelectedGameObject != base.gameObject)
            {
                EventSystem.current.SetSelectedGameObject(base.gameObject);
            }
            if (TouchScreenKeyboard.isSupported)
            {
                if (Input.touchSupported)
                {
                    TouchScreenKeyboard.hideInput = this.shouldHideMobileInput;
                }
                m_Keyboard = (this.inputType != InputType.Password) ? TouchScreenKeyboard.Open(this.m_Text, this.keyboardType, this.inputType == InputType.AutoCorrect, this.multiLine) : TouchScreenKeyboard.Open(this.m_Text, this.keyboardType, false, this.multiLine, true);
            }
            else
            {
                Input.imeCompositionMode = IMECompositionMode.On;
                this.OnFocus();
            }
            this.m_AllowInput = true;
            this.m_OriginalText = this.text;
            this.m_WasCanceled = false;
            this.SetCaretVisible();
            this.UpdateLabel();
        }

        protected virtual void Append(char input)
        {
            if (this.InPlaceEditing())
            {
                if (this.onValidateInput != null)
                {
                    input = this.onValidateInput(this.text, this.caretPositionInternal, input);
                }
                else if (this.characterValidation != CharacterValidation.None)
                {
                    input = this.Validate(this.text, this.caretPositionInternal, input);
                }
                if (input != '\0')
                {
                    this.Insert(input);
                }
            }
        }

        protected virtual void Append(string input)
        {
            if (this.InPlaceEditing())
            {
                int num = 0;
                int length = input.Length;
                while (num < length)
                {
                    char ch = input[num];
                    if (ch >= ' ')
                    {
                        this.Append(ch);
                    }
                    num++;
                }
            }
        }

        private void AssignPositioningIfNeeded()
        {
            if (((this.m_TextComponent != null) && (this.caretRectTrans != null)) && ((((this.caretRectTrans.localPosition != this.m_TextComponent.rectTransform.localPosition) || (this.caretRectTrans.localRotation != this.m_TextComponent.rectTransform.localRotation)) || ((this.caretRectTrans.localScale != this.m_TextComponent.rectTransform.localScale) || (this.caretRectTrans.anchorMin != this.m_TextComponent.rectTransform.anchorMin))) || (((this.caretRectTrans.anchorMax != this.m_TextComponent.rectTransform.anchorMax) || (this.caretRectTrans.anchoredPosition != this.m_TextComponent.rectTransform.anchoredPosition)) || ((this.caretRectTrans.sizeDelta != this.m_TextComponent.rectTransform.sizeDelta) || (this.caretRectTrans.pivot != this.m_TextComponent.rectTransform.pivot)))))
            {
                this.caretRectTrans.localPosition = this.m_TextComponent.rectTransform.localPosition;
                this.caretRectTrans.localRotation = this.m_TextComponent.rectTransform.localRotation;
                this.caretRectTrans.localScale = this.m_TextComponent.rectTransform.localScale;
                this.caretRectTrans.anchorMin = this.m_TextComponent.rectTransform.anchorMin;
                this.caretRectTrans.anchorMax = this.m_TextComponent.rectTransform.anchorMax;
                this.caretRectTrans.anchoredPosition = this.m_TextComponent.rectTransform.anchoredPosition;
                this.caretRectTrans.sizeDelta = this.m_TextComponent.rectTransform.sizeDelta;
                this.caretRectTrans.pivot = this.m_TextComponent.rectTransform.pivot;
            }
        }

        private void Backspace()
        {
            if (this.hasSelection)
            {
                this.Delete();
                this.SendOnValueChangedAndUpdateLabel();
            }
            else if (this.caretPositionInternal > 0)
            {
                this.m_Text = this.text.Remove(this.caretPositionInternal - 1, 1);
                int num = this.caretPositionInternal - 1;
                this.caretPositionInternal = num;
                this.caretSelectPositionInternal = num;
                this.SendOnValueChangedAndUpdateLabel();
            }
        }

        [DebuggerHidden]
        private IEnumerator CaretBlink()
        {
            return new <CaretBlink>c__Iterator3 { <>f__this = this };
        }

        protected void ClampPos(ref int pos)
        {
            if (pos < 0)
            {
                pos = 0;
            }
            else if (pos > this.text.Length)
            {
                pos = this.text.Length;
            }
        }

        private void CreateCursorVerts()
        {
            this.m_CursorVerts = new UIVertex[4];
            for (int i = 0; i < this.m_CursorVerts.Length; i++)
            {
                this.m_CursorVerts[i] = UIVertex.simpleVert;
                this.m_CursorVerts[i].color = this.m_TextComponent.color;
                this.m_CursorVerts[i].uv0 = Vector2.zero;
            }
        }

        public void DeactivateInputField()
        {
            if (this.m_AllowInput)
            {
                this.m_HasDoneFocusTransition = false;
                this.m_AllowInput = false;
                if ((this.m_TextComponent != null) && this.IsInteractable())
                {
                    if (this.m_WasCanceled)
                    {
                        this.text = this.m_OriginalText;
                    }
                    if (m_Keyboard != null)
                    {
                        m_Keyboard.active = false;
                        m_Keyboard = null;
                    }
                    this.m_CaretPosition = this.m_CaretSelectPosition = 0;
                    this.SendOnSubmit();
                    Input.imeCompositionMode = IMECompositionMode.Auto;
                }
                this.MarkGeometryAsDirty();
            }
        }

        private void Delete()
        {
            if (this.caretPositionInternal != this.caretSelectPositionInternal)
            {
                if (this.caretPositionInternal < this.caretSelectPositionInternal)
                {
                    this.m_Text = this.text.Substring(0, this.caretPositionInternal) + this.text.Substring(this.caretSelectPositionInternal, this.text.Length - this.caretSelectPositionInternal);
                    this.caretSelectPositionInternal = this.caretPositionInternal;
                }
                else
                {
                    this.m_Text = this.text.Substring(0, this.caretSelectPositionInternal) + this.text.Substring(this.caretPositionInternal, this.text.Length - this.caretPositionInternal);
                    this.caretPositionInternal = this.caretSelectPositionInternal;
                }
            }
        }

        private int DetermineCharacterLine(int charPos, TextGenerator generator)
        {
            if (!this.multiLine)
            {
                return 0;
            }
            for (int i = 0; i < (generator.lineCount - 1); i++)
            {
                UILineInfo info = generator.lines[i + 1];
                if (info.startCharIdx > charPos)
                {
                    return i;
                }
            }
            return (generator.lineCount - 1);
        }

        protected override void DoStateTransition(Selectable.SelectionState state, bool instant)
        {
            if (this.m_HasDoneFocusTransition)
            {
                state = Selectable.SelectionState.Highlighted;
            }
            else if (state == Selectable.SelectionState.Pressed)
            {
                this.m_HasDoneFocusTransition = true;
            }
            base.DoStateTransition(state, instant);
        }

        private void EnforceContentType()
        {
            switch (this.contentType)
            {
                case ContentType.Standard:
                    this.m_InputType = InputType.Standard;
                    this.m_KeyboardType = TouchScreenKeyboardType.Default;
                    this.m_CharacterValidation = CharacterValidation.None;
                    return;

                case ContentType.Autocorrected:
                    this.m_InputType = InputType.AutoCorrect;
                    this.m_KeyboardType = TouchScreenKeyboardType.Default;
                    this.m_CharacterValidation = CharacterValidation.None;
                    return;

                case ContentType.IntegerNumber:
                    this.m_LineType = LineType.SingleLine;
                    this.m_InputType = InputType.Standard;
                    this.m_KeyboardType = TouchScreenKeyboardType.NumberPad;
                    this.m_CharacterValidation = CharacterValidation.Integer;
                    return;

                case ContentType.DecimalNumber:
                    this.m_LineType = LineType.SingleLine;
                    this.m_InputType = InputType.Standard;
                    this.m_KeyboardType = TouchScreenKeyboardType.NumbersAndPunctuation;
                    this.m_CharacterValidation = CharacterValidation.Decimal;
                    return;

                case ContentType.Alphanumeric:
                    this.m_LineType = LineType.SingleLine;
                    this.m_InputType = InputType.Standard;
                    this.m_KeyboardType = TouchScreenKeyboardType.ASCIICapable;
                    this.m_CharacterValidation = CharacterValidation.Alphanumeric;
                    return;

                case ContentType.Name:
                    this.m_LineType = LineType.SingleLine;
                    this.m_InputType = InputType.Standard;
                    this.m_KeyboardType = TouchScreenKeyboardType.Default;
                    this.m_CharacterValidation = CharacterValidation.Name;
                    return;

                case ContentType.EmailAddress:
                    this.m_LineType = LineType.SingleLine;
                    this.m_InputType = InputType.Standard;
                    this.m_KeyboardType = TouchScreenKeyboardType.EmailAddress;
                    this.m_CharacterValidation = CharacterValidation.EmailAddress;
                    return;

                case ContentType.Password:
                    this.m_LineType = LineType.SingleLine;
                    this.m_InputType = InputType.Password;
                    this.m_KeyboardType = TouchScreenKeyboardType.Default;
                    this.m_CharacterValidation = CharacterValidation.None;
                    return;

                case ContentType.Pin:
                    this.m_LineType = LineType.SingleLine;
                    this.m_InputType = InputType.Password;
                    this.m_KeyboardType = TouchScreenKeyboardType.NumberPad;
                    this.m_CharacterValidation = CharacterValidation.Integer;
                    return;
            }
        }

        private int FindtNextWordBegin()
        {
            if ((this.caretSelectPositionInternal + 1) >= this.text.Length)
            {
                return this.text.Length;
            }
            int num = this.text.IndexOfAny(kSeparators, this.caretSelectPositionInternal + 1);
            if (num == -1)
            {
                return this.text.Length;
            }
            num++;
            return num;
        }

        private int FindtPrevWordBegin()
        {
            if ((this.caretSelectPositionInternal - 2) < 0)
            {
                return 0;
            }
            int num = this.text.LastIndexOfAny(kSeparators, this.caretSelectPositionInternal - 2);
            if (num == -1)
            {
                return 0;
            }
            num++;
            return num;
        }

        private void ForwardSpace()
        {
            if (this.hasSelection)
            {
                this.Delete();
                this.SendOnValueChangedAndUpdateLabel();
            }
            else if (this.caretPositionInternal < this.text.Length)
            {
                this.m_Text = this.text.Remove(this.caretPositionInternal, 1);
                this.SendOnValueChangedAndUpdateLabel();
            }
        }

        private void GenerateCursor(VertexHelper vbo, Vector2 roundingOffset)
        {
            if (this.m_CaretVisible)
            {
                if (this.m_CursorVerts == null)
                {
                    this.CreateCursorVerts();
                }
                float num = 1f;
                float fontSize = this.m_TextComponent.fontSize;
                int charPos = Mathf.Max(0, this.caretPositionInternal - this.m_DrawStart);
                TextGenerator cachedTextGenerator = this.m_TextComponent.cachedTextGenerator;
                if (cachedTextGenerator != null)
                {
                    if (this.m_TextComponent.resizeTextForBestFit)
                    {
                        fontSize = ((float) cachedTextGenerator.fontSizeUsedForBestFit) / this.m_TextComponent.pixelsPerUnit;
                    }
                    Vector2 zero = Vector2.zero;
                    if (((cachedTextGenerator.characterCountVisible + 1) > charPos) || (charPos == 0))
                    {
                        UICharInfo info = cachedTextGenerator.characters[charPos];
                        zero.x = info.cursorPos.x;
                        zero.y = info.cursorPos.y;
                    }
                    zero.x /= this.m_TextComponent.pixelsPerUnit;
                    if (zero.x > this.m_TextComponent.rectTransform.rect.xMax)
                    {
                        zero.x = this.m_TextComponent.rectTransform.rect.xMax;
                    }
                    int endLine = this.DetermineCharacterLine(charPos, cachedTextGenerator);
                    float num5 = this.SumLineHeights(endLine, cachedTextGenerator);
                    zero.y = this.m_TextComponent.rectTransform.rect.yMax - (num5 / this.m_TextComponent.pixelsPerUnit);
                    this.m_CursorVerts[0].position = new Vector3(zero.x, zero.y - fontSize, 0f);
                    this.m_CursorVerts[1].position = new Vector3(zero.x + num, zero.y - fontSize, 0f);
                    this.m_CursorVerts[2].position = new Vector3(zero.x + num, zero.y, 0f);
                    this.m_CursorVerts[3].position = new Vector3(zero.x, zero.y, 0f);
                    if (roundingOffset != Vector2.zero)
                    {
                        for (int i = 0; i < this.m_CursorVerts.Length; i++)
                        {
                            UIVertex vertex = this.m_CursorVerts[i];
                            vertex.position.x += roundingOffset.x;
                            vertex.position.y += roundingOffset.y;
                        }
                    }
                    vbo.AddUIVertexQuad(this.m_CursorVerts);
                    zero.y = Screen.height - zero.y;
                    Input.compositionCursorPos = zero;
                }
            }
        }

        private void GenerateHightlight(VertexHelper vbo, Vector2 roundingOffset)
        {
            int charPos = Mathf.Max(0, this.caretPositionInternal - this.m_DrawStart);
            int num2 = Mathf.Max(0, this.caretSelectPositionInternal - this.m_DrawStart);
            if (charPos > num2)
            {
                int num3 = charPos;
                charPos = num2;
                num2 = num3;
            }
            num2--;
            TextGenerator cachedTextGenerator = this.m_TextComponent.cachedTextGenerator;
            int line = this.DetermineCharacterLine(charPos, cachedTextGenerator);
            float fontSize = this.m_TextComponent.fontSize;
            if (this.m_TextComponent.resizeTextForBestFit)
            {
                fontSize = ((float) cachedTextGenerator.fontSizeUsedForBestFit) / this.m_TextComponent.pixelsPerUnit;
            }
            if ((this.cachedInputTextGenerator != null) && (this.cachedInputTextGenerator.lines.Count > 0))
            {
                UILineInfo info3 = this.cachedInputTextGenerator.lines[0];
                fontSize = info3.height;
            }
            if (this.m_TextComponent.resizeTextForBestFit && (this.cachedInputTextGenerator != null))
            {
                fontSize = this.cachedInputTextGenerator.fontSizeUsedForBestFit;
            }
            int lineEndPosition = GetLineEndPosition(cachedTextGenerator, line);
            UIVertex simpleVert = UIVertex.simpleVert;
            simpleVert.uv0 = Vector2.zero;
            simpleVert.color = this.selectionColor;
            for (int i = charPos; (i <= num2) && (i < cachedTextGenerator.characterCountVisible); i++)
            {
                if (((i + 1) == lineEndPosition) || (i == num2))
                {
                    UICharInfo info = cachedTextGenerator.characters[charPos];
                    UICharInfo info2 = cachedTextGenerator.characters[i];
                    float num8 = this.SumLineHeights(line, cachedTextGenerator);
                    Vector2 vector = new Vector2(info.cursorPos.x / this.m_TextComponent.pixelsPerUnit, this.m_TextComponent.rectTransform.rect.yMax - (num8 / this.m_TextComponent.pixelsPerUnit));
                    Vector2 vector2 = new Vector2((info2.cursorPos.x + info2.charWidth) / this.m_TextComponent.pixelsPerUnit, vector.y - (fontSize / this.m_TextComponent.pixelsPerUnit));
                    if ((vector2.x > this.m_TextComponent.rectTransform.rect.xMax) || (vector2.x < this.m_TextComponent.rectTransform.rect.xMin))
                    {
                        vector2.x = this.m_TextComponent.rectTransform.rect.xMax;
                    }
                    int currentVertCount = vbo.currentVertCount;
                    simpleVert.position = new Vector3(vector.x, vector2.y, 0f) + roundingOffset;
                    vbo.AddVert(simpleVert);
                    simpleVert.position = new Vector3(vector2.x, vector2.y, 0f) + roundingOffset;
                    vbo.AddVert(simpleVert);
                    simpleVert.position = new Vector3(vector2.x, vector.y, 0f) + roundingOffset;
                    vbo.AddVert(simpleVert);
                    simpleVert.position = new Vector3(vector.x, vector.y, 0f) + roundingOffset;
                    vbo.AddVert(simpleVert);
                    vbo.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
                    vbo.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);
                    charPos = i + 1;
                    line++;
                    lineEndPosition = GetLineEndPosition(cachedTextGenerator, line);
                }
            }
        }

        protected int GetCharacterIndexFromPosition(Vector2 pos)
        {
            TextGenerator cachedTextGenerator = this.m_TextComponent.cachedTextGenerator;
            if (cachedTextGenerator.lineCount == 0)
            {
                return 0;
            }
            int unclampedCharacterLineFromPosition = this.GetUnclampedCharacterLineFromPosition(pos, cachedTextGenerator);
            if (unclampedCharacterLineFromPosition < 0)
            {
                return 0;
            }
            if (unclampedCharacterLineFromPosition >= cachedTextGenerator.lineCount)
            {
                return cachedTextGenerator.characterCountVisible;
            }
            UILineInfo info2 = cachedTextGenerator.lines[unclampedCharacterLineFromPosition];
            int startCharIdx = info2.startCharIdx;
            int lineEndPosition = GetLineEndPosition(cachedTextGenerator, unclampedCharacterLineFromPosition);
            for (int i = startCharIdx; i < lineEndPosition; i++)
            {
                if (i >= cachedTextGenerator.characterCountVisible)
                {
                    return lineEndPosition;
                }
                UICharInfo info = cachedTextGenerator.characters[i];
                Vector2 vector = (Vector2) (info.cursorPos / this.m_TextComponent.pixelsPerUnit);
                float num5 = pos.x - vector.x;
                float num6 = (vector.x + (info.charWidth / this.m_TextComponent.pixelsPerUnit)) - pos.x;
                if (num5 < num6)
                {
                    return i;
                }
            }
            return lineEndPosition;
        }

        private static int GetLineEndPosition(TextGenerator gen, int line)
        {
            line = Mathf.Max(line, 0);
            if ((line + 1) < gen.lines.Count)
            {
                UILineInfo info = gen.lines[line + 1];
                return info.startCharIdx;
            }
            return gen.characterCountVisible;
        }

        private static int GetLineStartPosition(TextGenerator gen, int line)
        {
            line = Mathf.Clamp(line, 0, gen.lines.Count - 1);
            UILineInfo info = gen.lines[line];
            return info.startCharIdx;
        }

        private string GetSelectedString()
        {
            if (!this.hasSelection)
            {
                return string.Empty;
            }
            int caretPositionInternal = this.caretPositionInternal;
            int caretSelectPositionInternal = this.caretSelectPositionInternal;
            if (caretPositionInternal > caretSelectPositionInternal)
            {
                int num3 = caretPositionInternal;
                caretPositionInternal = caretSelectPositionInternal;
                caretSelectPositionInternal = num3;
            }
            return this.text.Substring(caretPositionInternal, caretSelectPositionInternal - caretPositionInternal);
        }

        private int GetUnclampedCharacterLineFromPosition(Vector2 pos, TextGenerator generator)
        {
            if (!this.multiLine)
            {
                return 0;
            }
            float yMax = this.m_TextComponent.rectTransform.rect.yMax;
            if (pos.y > yMax)
            {
                return -1;
            }
            for (int i = 0; i < generator.lineCount; i++)
            {
                UILineInfo info = generator.lines[i];
                float num3 = ((float) info.height) / this.m_TextComponent.pixelsPerUnit;
                if ((pos.y <= yMax) && (pos.y > (yMax - num3)))
                {
                    return i;
                }
                yMax -= num3;
            }
            return generator.lineCount;
        }

        private bool InPlaceEditing()
        {
            return !TouchScreenKeyboard.isSupported;
        }

        private void Insert(char c)
        {
            string str = c.ToString();
            this.Delete();
            if ((this.characterLimit <= 0) || (this.text.Length < this.characterLimit))
            {
                this.m_Text = this.text.Insert(this.m_CaretPosition, str);
                this.caretSelectPositionInternal = this.caretPositionInternal += str.Length;
                this.SendOnValueChanged();
            }
        }

        private bool IsSelectionVisible()
        {
            if ((this.m_DrawStart > this.caretPositionInternal) || (this.m_DrawStart > this.caretSelectPositionInternal))
            {
                return false;
            }
            return ((this.m_DrawEnd >= this.caretPositionInternal) && (this.m_DrawEnd >= this.caretSelectPositionInternal));
        }

        private bool IsValidChar(char c)
        {
            if (c == '\x007f')
            {
                return false;
            }
            if ((c != '\t') && (c != '\n'))
            {
                return this.m_TextComponent.font.HasCharacter(c);
            }
            return true;
        }

        protected EditState KeyPressed(Event evt)
        {
            char ch;
            EventModifiers modifiers = evt.modifiers;
            RuntimePlatform platform = Application.platform;
            bool ctrl = (((platform != RuntimePlatform.OSXEditor) && (platform != RuntimePlatform.OSXPlayer)) && (platform != RuntimePlatform.OSXWebPlayer)) ? ((modifiers & EventModifiers.Control) != EventModifiers.None) : ((modifiers & EventModifiers.Command) != EventModifiers.None);
            bool shift = (modifiers & EventModifiers.Shift) != EventModifiers.None;
            bool flag4 = (modifiers & EventModifiers.Alt) != EventModifiers.None;
            bool flag5 = (ctrl && !flag4) && !shift;
            KeyCode keyCode = evt.keyCode;
            switch (keyCode)
            {
                case KeyCode.KeypadEnter:
                    break;

                case KeyCode.UpArrow:
                    this.MoveUp(shift);
                    return EditState.Continue;

                case KeyCode.DownArrow:
                    this.MoveDown(shift);
                    return EditState.Continue;

                case KeyCode.RightArrow:
                    this.MoveRight(shift, ctrl);
                    return EditState.Continue;

                case KeyCode.LeftArrow:
                    this.MoveLeft(shift, ctrl);
                    return EditState.Continue;

                case KeyCode.Home:
                    this.MoveTextStart(shift);
                    return EditState.Continue;

                case KeyCode.End:
                    this.MoveTextEnd(shift);
                    return EditState.Continue;

                case KeyCode.A:
                    if (!flag5)
                    {
                        goto Label_0205;
                    }
                    this.SelectAll();
                    return EditState.Continue;

                case KeyCode.C:
                    if (!flag5)
                    {
                        goto Label_0205;
                    }
                    if (this.inputType == InputType.Password)
                    {
                        clipboard = string.Empty;
                    }
                    else
                    {
                        clipboard = this.GetSelectedString();
                    }
                    return EditState.Continue;

                case KeyCode.V:
                    if (!flag5)
                    {
                        goto Label_0205;
                    }
                    this.Append(clipboard);
                    return EditState.Continue;

                case KeyCode.X:
                    if (!flag5)
                    {
                        goto Label_0205;
                    }
                    if (this.inputType == InputType.Password)
                    {
                        clipboard = string.Empty;
                    }
                    else
                    {
                        clipboard = this.GetSelectedString();
                    }
                    this.Delete();
                    this.SendOnValueChangedAndUpdateLabel();
                    return EditState.Continue;

                default:
                    if (keyCode != KeyCode.Backspace)
                    {
                        if (keyCode == KeyCode.Return)
                        {
                            break;
                        }
                        if (keyCode == KeyCode.Escape)
                        {
                            this.m_WasCanceled = true;
                            return EditState.Finish;
                        }
                        if (keyCode == KeyCode.Delete)
                        {
                            this.ForwardSpace();
                            return EditState.Continue;
                        }
                        goto Label_0205;
                    }
                    this.Backspace();
                    return EditState.Continue;
            }
            if (this.lineType != LineType.MultiLineNewline)
            {
                return EditState.Finish;
            }
        Label_0205:
            ch = evt.character;
            if (this.multiLine || (((ch != '\t') && (ch != '\r')) && (ch != '\n')))
            {
                if ((ch == '\r') || (ch == '\x0003'))
                {
                    ch = '\n';
                }
                if (this.IsValidChar(ch))
                {
                    this.Append(ch);
                }
                if ((ch == '\0') && (Input.compositionString.Length > 0))
                {
                    this.UpdateLabel();
                }
            }
            return EditState.Continue;
        }

        protected virtual void LateUpdate()
        {
            if (this.m_ShouldActivateNextUpdate)
            {
                if (!this.isFocused)
                {
                    this.ActivateInputFieldInternal();
                    this.m_ShouldActivateNextUpdate = false;
                    return;
                }
                this.m_ShouldActivateNextUpdate = false;
            }
            if (!this.InPlaceEditing() && this.isFocused)
            {
                this.AssignPositioningIfNeeded();
                if ((m_Keyboard == null) || !m_Keyboard.active)
                {
                    if ((m_Keyboard != null) && m_Keyboard.wasCanceled)
                    {
                        this.m_WasCanceled = true;
                    }
                    this.OnDeselect(null);
                }
                else
                {
                    string text = m_Keyboard.text;
                    if (this.m_Text != text)
                    {
                        this.m_Text = string.Empty;
                        for (int i = 0; i < text.Length; i++)
                        {
                            char addedChar = text[i];
                            switch (addedChar)
                            {
                                case '\r':
                                case '\x0003':
                                    addedChar = '\n';
                                    break;
                            }
                            if (this.onValidateInput != null)
                            {
                                addedChar = this.onValidateInput(this.m_Text, this.m_Text.Length, addedChar);
                            }
                            else if (this.characterValidation != CharacterValidation.None)
                            {
                                addedChar = this.Validate(this.m_Text, this.m_Text.Length, addedChar);
                            }
                            if ((this.lineType == LineType.MultiLineSubmit) && (addedChar == '\n'))
                            {
                                m_Keyboard.text = this.m_Text;
                                this.OnDeselect(null);
                                return;
                            }
                            if (addedChar != '\0')
                            {
                                this.m_Text = this.m_Text + addedChar;
                            }
                        }
                        if ((this.characterLimit > 0) && (this.m_Text.Length > this.characterLimit))
                        {
                            this.m_Text = this.m_Text.Substring(0, this.characterLimit);
                        }
                        int length = this.m_Text.Length;
                        this.caretSelectPositionInternal = length;
                        this.caretPositionInternal = length;
                        if (this.m_Text != text)
                        {
                            m_Keyboard.text = this.m_Text;
                        }
                        this.SendOnValueChangedAndUpdateLabel();
                    }
                    if (m_Keyboard.done)
                    {
                        if (m_Keyboard.wasCanceled)
                        {
                            this.m_WasCanceled = true;
                        }
                        this.OnDeselect(null);
                    }
                }
            }
        }

        private int LineDownCharacterPosition(int originalPos, bool goToLastChar)
        {
            if (originalPos >= this.cachedInputTextGenerator.characterCountVisible)
            {
                return this.text.Length;
            }
            UICharInfo info = this.cachedInputTextGenerator.characters[originalPos];
            int num = this.DetermineCharacterLine(originalPos, this.cachedInputTextGenerator);
            if ((num + 1) >= this.cachedInputTextGenerator.lineCount)
            {
                return (!goToLastChar ? originalPos : this.text.Length);
            }
            int lineEndPosition = GetLineEndPosition(this.cachedInputTextGenerator, num + 1);
            UILineInfo info2 = this.cachedInputTextGenerator.lines[num + 1];
            for (int i = info2.startCharIdx; i < lineEndPosition; i++)
            {
                UICharInfo info3 = this.cachedInputTextGenerator.characters[i];
                if (info3.cursorPos.x >= info.cursorPos.x)
                {
                    return i;
                }
            }
            return lineEndPosition;
        }

        private int LineUpCharacterPosition(int originalPos, bool goToFirstChar)
        {
            if (originalPos >= this.cachedInputTextGenerator.characterCountVisible)
            {
                return 0;
            }
            UICharInfo info = this.cachedInputTextGenerator.characters[originalPos];
            int num = this.DetermineCharacterLine(originalPos, this.cachedInputTextGenerator);
            if ((num - 1) < 0)
            {
                return (!goToFirstChar ? originalPos : 0);
            }
            UILineInfo info2 = this.cachedInputTextGenerator.lines[num];
            int num2 = info2.startCharIdx - 1;
            UILineInfo info3 = this.cachedInputTextGenerator.lines[num - 1];
            for (int i = info3.startCharIdx; i < num2; i++)
            {
                UICharInfo info4 = this.cachedInputTextGenerator.characters[i];
                if (info4.cursorPos.x >= info.cursorPos.x)
                {
                    return i;
                }
            }
            return num2;
        }

        private void MarkGeometryAsDirty()
        {
            if (Application.isPlaying && (PrefabUtility.GetPrefabObject(base.gameObject) == null))
            {
                CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
            }
        }

        private bool MayDrag(PointerEventData eventData)
        {
            return (((this.IsActive() && this.IsInteractable()) && ((eventData.button == PointerEventData.InputButton.Left) && (this.m_TextComponent != null))) && (m_Keyboard == null));
        }

        [DebuggerHidden]
        private IEnumerator MouseDragOutsideRect(PointerEventData eventData)
        {
            return new <MouseDragOutsideRect>c__Iterator4 { eventData = eventData, <$>eventData = eventData, <>f__this = this };
        }

        private void MoveDown(bool shift)
        {
            this.MoveDown(shift, true);
        }

        private void MoveDown(bool shift, bool goToLastChar)
        {
            int num2;
            if (this.hasSelection && !shift)
            {
                num2 = Mathf.Max(this.caretPositionInternal, this.caretSelectPositionInternal);
                this.caretSelectPositionInternal = num2;
                this.caretPositionInternal = num2;
            }
            int num = !this.multiLine ? this.text.Length : this.LineDownCharacterPosition(this.caretSelectPositionInternal, goToLastChar);
            if (shift)
            {
                this.caretSelectPositionInternal = num;
            }
            else
            {
                num2 = num;
                this.caretSelectPositionInternal = num2;
                this.caretPositionInternal = num2;
            }
        }

        private void MoveLeft(bool shift, bool ctrl)
        {
            int num2;
            if (this.hasSelection && !shift)
            {
                num2 = Mathf.Min(this.caretPositionInternal, this.caretSelectPositionInternal);
                this.caretSelectPositionInternal = num2;
                this.caretPositionInternal = num2;
            }
            else
            {
                int num;
                if (ctrl)
                {
                    num = this.FindtPrevWordBegin();
                }
                else
                {
                    num = this.caretSelectPositionInternal - 1;
                }
                if (shift)
                {
                    this.caretSelectPositionInternal = num;
                }
                else
                {
                    num2 = num;
                    this.caretPositionInternal = num2;
                    this.caretSelectPositionInternal = num2;
                }
            }
        }

        private void MoveRight(bool shift, bool ctrl)
        {
            int num2;
            if (this.hasSelection && !shift)
            {
                num2 = Mathf.Max(this.caretPositionInternal, this.caretSelectPositionInternal);
                this.caretSelectPositionInternal = num2;
                this.caretPositionInternal = num2;
            }
            else
            {
                int num;
                if (ctrl)
                {
                    num = this.FindtNextWordBegin();
                }
                else
                {
                    num = this.caretSelectPositionInternal + 1;
                }
                if (shift)
                {
                    this.caretSelectPositionInternal = num;
                }
                else
                {
                    num2 = num;
                    this.caretPositionInternal = num2;
                    this.caretSelectPositionInternal = num2;
                }
            }
        }

        public void MoveTextEnd(bool shift)
        {
            int length = this.text.Length;
            if (shift)
            {
                this.caretSelectPositionInternal = length;
            }
            else
            {
                this.caretPositionInternal = length;
                this.caretSelectPositionInternal = this.caretPositionInternal;
            }
            this.UpdateLabel();
        }

        public void MoveTextStart(bool shift)
        {
            int num = 0;
            if (shift)
            {
                this.caretSelectPositionInternal = num;
            }
            else
            {
                this.caretPositionInternal = num;
                this.caretSelectPositionInternal = this.caretPositionInternal;
            }
            this.UpdateLabel();
        }

        private void MoveUp(bool shift)
        {
            this.MoveUp(shift, true);
        }

        private void MoveUp(bool shift, bool goToFirstChar)
        {
            int num2;
            if (this.hasSelection && !shift)
            {
                num2 = Mathf.Min(this.caretPositionInternal, this.caretSelectPositionInternal);
                this.caretSelectPositionInternal = num2;
                this.caretPositionInternal = num2;
            }
            int num = !this.multiLine ? 0 : this.LineUpCharacterPosition(this.caretSelectPositionInternal, goToFirstChar);
            if (shift)
            {
                this.caretSelectPositionInternal = num;
            }
            else
            {
                num2 = num;
                this.caretPositionInternal = num2;
                this.caretSelectPositionInternal = num2;
            }
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (this.MayDrag(eventData))
            {
                this.m_UpdateDrag = true;
            }
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            this.DeactivateInputField();
            base.OnDeselect(eventData);
        }

        protected override void OnDisable()
        {
            this.m_BlinkCoroutine = null;
            this.DeactivateInputField();
            if (this.m_TextComponent != null)
            {
                this.m_TextComponent.UnregisterDirtyVerticesCallback(new UnityAction(this.MarkGeometryAsDirty));
                this.m_TextComponent.UnregisterDirtyVerticesCallback(new UnityAction(this.UpdateLabel));
            }
            CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);
            if (this.m_CachedInputRenderer != null)
            {
                this.m_CachedInputRenderer.SetMesh(null);
            }
            if (this.m_Mesh != null)
            {
                UnityEngine.Object.DestroyImmediate(this.m_Mesh);
            }
            this.m_Mesh = null;
            base.OnDisable();
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (this.MayDrag(eventData))
            {
                Vector2 vector;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(this.textComponent.rectTransform, eventData.position, eventData.pressEventCamera, out vector);
                this.caretSelectPositionInternal = this.GetCharacterIndexFromPosition(vector) + this.m_DrawStart;
                this.MarkGeometryAsDirty();
                this.m_DragPositionOutOfBounds = !RectTransformUtility.RectangleContainsScreenPoint(this.textComponent.rectTransform, eventData.position, eventData.pressEventCamera);
                if (this.m_DragPositionOutOfBounds && (this.m_DragCoroutine == null))
                {
                    this.m_DragCoroutine = base.StartCoroutine(this.MouseDragOutsideRect(eventData));
                }
                eventData.Use();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (this.m_Text == null)
            {
                this.m_Text = string.Empty;
            }
            this.m_DrawStart = 0;
            this.m_DrawEnd = this.m_Text.Length;
            if (this.m_TextComponent != null)
            {
                this.m_TextComponent.RegisterDirtyVerticesCallback(new UnityAction(this.MarkGeometryAsDirty));
                this.m_TextComponent.RegisterDirtyVerticesCallback(new UnityAction(this.UpdateLabel));
                this.UpdateLabel();
            }
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (this.MayDrag(eventData))
            {
                this.m_UpdateDrag = false;
            }
        }

        private void OnFillVBO(Mesh vbo)
        {
            using (VertexHelper helper = new VertexHelper())
            {
                if (!this.isFocused)
                {
                    helper.FillMesh(vbo);
                }
                else
                {
                    Rect rect = this.m_TextComponent.rectTransform.rect;
                    Vector2 size = rect.size;
                    Vector2 textAnchorPivot = Text.GetTextAnchorPivot(this.m_TextComponent.alignment);
                    Vector2 zero = Vector2.zero;
                    zero.x = Mathf.Lerp(rect.xMin, rect.xMax, textAnchorPivot.x);
                    zero.y = Mathf.Lerp(rect.yMin, rect.yMax, textAnchorPivot.y);
                    Vector2 roundingOffset = (this.m_TextComponent.PixelAdjustPoint(zero) - zero) + Vector2.Scale(size, textAnchorPivot);
                    roundingOffset.x -= Mathf.Floor(0.5f + roundingOffset.x);
                    roundingOffset.y -= Mathf.Floor(0.5f + roundingOffset.y);
                    if (!this.hasSelection)
                    {
                        this.GenerateCursor(helper, roundingOffset);
                    }
                    else
                    {
                        this.GenerateHightlight(helper, roundingOffset);
                    }
                    helper.FillMesh(vbo);
                }
            }
        }

        protected void OnFocus()
        {
            this.SelectAll();
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                this.ActivateInputField();
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (this.MayDrag(eventData))
            {
                EventSystem.current.SetSelectedGameObject(base.gameObject, eventData);
                bool allowInput = this.m_AllowInput;
                base.OnPointerDown(eventData);
                if (!this.InPlaceEditing() && ((m_Keyboard == null) || !m_Keyboard.active))
                {
                    this.OnSelect(eventData);
                }
                else
                {
                    if (allowInput)
                    {
                        Vector2 pos = this.ScreenToLocal(eventData.position);
                        int num = this.GetCharacterIndexFromPosition(pos) + this.m_DrawStart;
                        this.caretPositionInternal = num;
                        this.caretSelectPositionInternal = num;
                    }
                    this.UpdateLabel();
                    eventData.Use();
                }
            }
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            this.ActivateInputField();
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            if ((this.IsActive() && this.IsInteractable()) && !this.isFocused)
            {
                this.m_ShouldActivateNextUpdate = true;
            }
        }

        public virtual void OnUpdateSelected(BaseEventData eventData)
        {
            if (this.isFocused)
            {
                bool flag = false;
                while (Event.PopEvent(this.m_ProcessingEvent))
                {
                    if (this.m_ProcessingEvent.rawType == EventType.KeyDown)
                    {
                        flag = true;
                        if (this.KeyPressed(this.m_ProcessingEvent) == EditState.Finish)
                        {
                            this.DeactivateInputField();
                            break;
                        }
                    }
                    EventType type = this.m_ProcessingEvent.type;
                    if ((type == EventType.ValidateCommand) || (type == EventType.ExecuteCommand))
                    {
                        string commandName = this.m_ProcessingEvent.commandName;
                        if (commandName != null)
                        {
                            int num;
                            if (<>f__switch$map0 == null)
                            {
                                Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
                                dictionary.Add("SelectAll", 0);
                                <>f__switch$map0 = dictionary;
                            }
                            if (<>f__switch$map0.TryGetValue(commandName, out num) && (num == 0))
                            {
                                this.SelectAll();
                                flag = true;
                            }
                        }
                    }
                }
                if (flag)
                {
                    this.UpdateLabel();
                }
                eventData.Use();
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            this.EnforceContentType();
            if (this.IsActive())
            {
                this.UpdateLabel();
                if (this.m_AllowInput)
                {
                    this.SetCaretActive();
                }
            }
        }

        public void ProcessEvent(Event e)
        {
            this.KeyPressed(e);
        }

        public virtual void Rebuild(CanvasUpdate update)
        {
            if (update == CanvasUpdate.LatePreRender)
            {
                this.UpdateGeometry();
            }
        }

        public Vector2 ScreenToLocal(Vector2 screen)
        {
            Canvas canvas = this.m_TextComponent.canvas;
            if (canvas == null)
            {
                return screen;
            }
            Vector3 zero = Vector3.zero;
            if (canvas.renderMode == UnityEngine.RenderMode.ScreenSpaceOverlay)
            {
                zero = this.m_TextComponent.transform.InverseTransformPoint((Vector3) screen);
            }
            else if (canvas.worldCamera != null)
            {
                float num;
                Ray ray = canvas.worldCamera.ScreenPointToRay((Vector3) screen);
                new Plane(this.m_TextComponent.transform.forward, this.m_TextComponent.transform.position).Raycast(ray, out num);
                zero = this.m_TextComponent.transform.InverseTransformPoint(ray.GetPoint(num));
            }
            return new Vector2(zero.x, zero.y);
        }

        protected void SelectAll()
        {
            this.caretPositionInternal = this.text.Length;
            this.caretSelectPositionInternal = 0;
        }

        protected void SendOnSubmit()
        {
            if (this.onEndEdit != null)
            {
                this.onEndEdit.Invoke(this.m_Text);
            }
        }

        private void SendOnValueChanged()
        {
            if (this.onValueChange != null)
            {
                this.onValueChange.Invoke(this.text);
            }
        }

        private void SendOnValueChangedAndUpdateLabel()
        {
            this.SendOnValueChanged();
            this.UpdateLabel();
        }

        private void SetCaretActive()
        {
            if (this.m_AllowInput)
            {
                if (this.m_CaretBlinkRate > 0f)
                {
                    if (this.m_BlinkCoroutine == null)
                    {
                        this.m_BlinkCoroutine = base.StartCoroutine(this.CaretBlink());
                    }
                }
                else
                {
                    this.m_CaretVisible = true;
                }
            }
        }

        private void SetCaretVisible()
        {
            if (this.m_AllowInput)
            {
                this.m_CaretVisible = true;
                this.m_BlinkStartTime = Time.unscaledTime;
                this.SetCaretActive();
            }
        }

        private void SetDrawRangeToContainCaretPosition(int caretPos)
        {
            Vector2 size = this.cachedInputTextGenerator.rectExtents.size;
            if (this.multiLine)
            {
                IList<UILineInfo> lines = this.cachedInputTextGenerator.lines;
                int line = this.DetermineCharacterLine(caretPos, this.cachedInputTextGenerator);
                int y = (int) size.y;
                if (this.m_DrawEnd <= caretPos)
                {
                    this.m_DrawEnd = GetLineEndPosition(this.cachedInputTextGenerator, line);
                    for (int i = line; (i >= 0) && (i < lines.Count); i--)
                    {
                        UILineInfo info = lines[i];
                        y -= info.height;
                        if (y < 0)
                        {
                            break;
                        }
                        this.m_DrawStart = GetLineStartPosition(this.cachedInputTextGenerator, i);
                    }
                    return;
                }
                if (this.m_DrawStart > caretPos)
                {
                    this.m_DrawStart = GetLineStartPosition(this.cachedInputTextGenerator, line);
                }
                int num4 = this.DetermineCharacterLine(this.m_DrawStart, this.cachedInputTextGenerator);
                int num5 = num4;
                this.m_DrawEnd = GetLineEndPosition(this.cachedInputTextGenerator, num5);
                UILineInfo info2 = lines[num5];
                y -= info2.height;
                while (true)
                {
                    while (num5 < (lines.Count - 1))
                    {
                        num5++;
                        UILineInfo info3 = lines[num5];
                        if (y < info3.height)
                        {
                            return;
                        }
                        this.m_DrawEnd = GetLineEndPosition(this.cachedInputTextGenerator, num5);
                        UILineInfo info4 = lines[num5];
                        y -= info4.height;
                    }
                    if (num4 <= 0)
                    {
                        return;
                    }
                    num4--;
                    UILineInfo info5 = lines[num4];
                    if (y < info5.height)
                    {
                        return;
                    }
                    this.m_DrawStart = GetLineStartPosition(this.cachedInputTextGenerator, num4);
                    UILineInfo info6 = lines[num4];
                    y -= info6.height;
                }
            }
            IList<UICharInfo> characters = this.cachedInputTextGenerator.characters;
            if (this.m_DrawEnd > this.cachedInputTextGenerator.characterCountVisible)
            {
                this.m_DrawEnd = this.cachedInputTextGenerator.characterCountVisible;
            }
            float num6 = 0f;
            if ((caretPos <= this.m_DrawEnd) && ((caretPos != this.m_DrawEnd) || (this.m_DrawStart <= 0)))
            {
                if (caretPos < this.m_DrawStart)
                {
                    this.m_DrawStart = caretPos;
                }
                this.m_DrawEnd = this.m_DrawStart;
            }
            else
            {
                this.m_DrawEnd = caretPos;
                this.m_DrawStart = this.m_DrawEnd - 1;
                while (this.m_DrawStart >= 0)
                {
                    UICharInfo info7 = characters[this.m_DrawStart];
                    if ((num6 + info7.charWidth) > size.x)
                    {
                        break;
                    }
                    UICharInfo info8 = characters[this.m_DrawStart];
                    num6 += info8.charWidth;
                    this.m_DrawStart--;
                }
                this.m_DrawStart++;
            }
            while (this.m_DrawEnd < this.cachedInputTextGenerator.characterCountVisible)
            {
                UICharInfo info9 = characters[this.m_DrawEnd];
                num6 += info9.charWidth;
                if (num6 > size.x)
                {
                    break;
                }
                this.m_DrawEnd++;
            }
        }

        private void SetToCustom()
        {
            if (this.contentType != ContentType.Custom)
            {
                this.contentType = ContentType.Custom;
            }
        }

        private void SetToCustomIfContentTypeIsNot(params ContentType[] allowedContentTypes)
        {
            if (this.contentType != ContentType.Custom)
            {
                for (int i = 0; i < allowedContentTypes.Length; i++)
                {
                    if (this.contentType == allowedContentTypes[i])
                    {
                        return;
                    }
                }
                this.contentType = ContentType.Custom;
            }
        }

        private float SumLineHeights(int endLine, TextGenerator generator)
        {
            float num = 0f;
            for (int i = 0; i < endLine; i++)
            {
                UILineInfo info = generator.lines[i];
                num += info.height;
            }
            return num;
        }

        Transform ICanvasElement.get_transform()
        {
            return base.transform;
        }

        bool ICanvasElement.IsDestroyed()
        {
            return base.IsDestroyed();
        }

        private void UpdateGeometry()
        {
            if (Application.isPlaying && this.shouldHideMobileInput)
            {
                if ((this.m_CachedInputRenderer == null) && (this.m_TextComponent != null))
                {
                    GameObject obj2 = new GameObject(base.transform.name + " Input Caret") {
                        hideFlags = HideFlags.DontSave
                    };
                    obj2.transform.SetParent(this.m_TextComponent.transform.parent);
                    obj2.transform.SetAsFirstSibling();
                    obj2.layer = base.gameObject.layer;
                    this.caretRectTrans = obj2.AddComponent<RectTransform>();
                    this.m_CachedInputRenderer = obj2.AddComponent<CanvasRenderer>();
                    this.m_CachedInputRenderer.SetMaterial(Graphic.defaultGraphicMaterial, (Texture) null);
                    obj2.AddComponent<LayoutElement>().ignoreLayout = true;
                    this.AssignPositioningIfNeeded();
                }
                if (this.m_CachedInputRenderer != null)
                {
                    this.OnFillVBO(this.mesh);
                    this.m_CachedInputRenderer.SetMesh(this.mesh);
                }
            }
        }

        protected void UpdateLabel()
        {
            if (((this.m_TextComponent != null) && (this.m_TextComponent.font != null)) && !this.m_PreventFontCallback)
            {
                string text;
                string str2;
                this.m_PreventFontCallback = true;
                if (Input.compositionString.Length > 0)
                {
                    text = this.text.Substring(0, this.m_CaretPosition) + Input.compositionString + this.text.Substring(this.m_CaretPosition);
                }
                else
                {
                    text = this.text;
                }
                if (this.inputType == InputType.Password)
                {
                    str2 = new string(this.asteriskChar, text.Length);
                }
                else
                {
                    str2 = text;
                }
                bool flag = string.IsNullOrEmpty(text);
                if (this.m_Placeholder != null)
                {
                    this.m_Placeholder.enabled = flag;
                }
                if (!this.m_AllowInput)
                {
                    this.m_DrawStart = 0;
                    this.m_DrawEnd = this.m_Text.Length;
                }
                if (!flag)
                {
                    Vector2 size = this.m_TextComponent.rectTransform.rect.size;
                    TextGenerationSettings generationSettings = this.m_TextComponent.GetGenerationSettings(size);
                    generationSettings.generateOutOfBounds = true;
                    this.cachedInputTextGenerator.Populate(str2, generationSettings);
                    this.SetDrawRangeToContainCaretPosition(this.caretSelectPositionInternal);
                    str2 = str2.Substring(this.m_DrawStart, Mathf.Min(this.m_DrawEnd, str2.Length) - this.m_DrawStart);
                    this.SetCaretVisible();
                }
                this.m_TextComponent.text = str2;
                this.MarkGeometryAsDirty();
                this.m_PreventFontCallback = false;
            }
        }

        protected char Validate(string text, int pos, char ch)
        {
            if ((this.characterValidation == CharacterValidation.None) || !base.enabled)
            {
                return ch;
            }
            if ((this.characterValidation == CharacterValidation.Integer) || (this.characterValidation == CharacterValidation.Decimal))
            {
                if (((pos != 0) || (text.Length <= 0)) || (text[0] != '-'))
                {
                    if ((ch >= '0') && (ch <= '9'))
                    {
                        return ch;
                    }
                    if ((ch == '-') && (pos == 0))
                    {
                        return ch;
                    }
                    if (((ch == '.') && (this.characterValidation == CharacterValidation.Decimal)) && !text.Contains("."))
                    {
                        return ch;
                    }
                }
            }
            else if (this.characterValidation == CharacterValidation.Alphanumeric)
            {
                if ((ch >= 'A') && (ch <= 'Z'))
                {
                    return ch;
                }
                if ((ch >= 'a') && (ch <= 'z'))
                {
                    return ch;
                }
                if ((ch >= '0') && (ch <= '9'))
                {
                    return ch;
                }
            }
            else if (this.characterValidation == CharacterValidation.Name)
            {
                char ch2 = (text.Length <= 0) ? ' ' : text[Mathf.Clamp(pos, 0, text.Length - 1)];
                char ch3 = (text.Length <= 0) ? '\n' : text[Mathf.Clamp(pos + 1, 0, text.Length - 1)];
                if (char.IsLetter(ch))
                {
                    if (char.IsLower(ch) && (ch2 == ' '))
                    {
                        return char.ToUpper(ch);
                    }
                    if ((char.IsUpper(ch) && (ch2 != ' ')) && (ch2 != '\''))
                    {
                        return char.ToLower(ch);
                    }
                    return ch;
                }
                if (ch != '\'')
                {
                    if ((((ch == ' ') && (ch2 != ' ')) && ((ch2 != '\'') && (ch3 != ' '))) && (ch3 != '\''))
                    {
                        return ch;
                    }
                }
                else if (((ch2 != ' ') && (ch2 != '\'')) && ((ch3 != '\'') && !text.Contains("'")))
                {
                    return ch;
                }
            }
            else if (this.characterValidation == CharacterValidation.EmailAddress)
            {
                if ((ch >= 'A') && (ch <= 'Z'))
                {
                    return ch;
                }
                if ((ch >= 'a') && (ch <= 'z'))
                {
                    return ch;
                }
                if ((ch >= '0') && (ch <= '9'))
                {
                    return ch;
                }
                if ((ch == '@') && (text.IndexOf('@') == -1))
                {
                    return ch;
                }
                if ("!#$%&'*+-/=?^_`{|}~".IndexOf(ch) != -1)
                {
                    return ch;
                }
                if (ch == '.')
                {
                    char ch4 = (text.Length <= 0) ? ' ' : text[Mathf.Clamp(pos, 0, text.Length - 1)];
                    char ch5 = (text.Length <= 0) ? '\n' : text[Mathf.Clamp(pos + 1, 0, text.Length - 1)];
                    if ((ch4 != '.') && (ch5 != '.'))
                    {
                        return ch;
                    }
                }
            }
            return '\0';
        }

        public char asteriskChar
        {
            get
            {
                return this.m_AsteriskChar;
            }
            set
            {
                SetPropertyUtility.SetStruct<char>(ref this.m_AsteriskChar, value);
            }
        }

        protected TextGenerator cachedInputTextGenerator
        {
            get
            {
                if (this.m_InputTextCache == null)
                {
                    this.m_InputTextCache = new TextGenerator();
                }
                return this.m_InputTextCache;
            }
        }

        public float caretBlinkRate
        {
            get
            {
                return this.m_CaretBlinkRate;
            }
            set
            {
                if (SetPropertyUtility.SetStruct<float>(ref this.m_CaretBlinkRate, value) && this.m_AllowInput)
                {
                    this.SetCaretActive();
                }
            }
        }

        public int caretPosition
        {
            get
            {
                return (this.m_CaretSelectPosition + Input.compositionString.Length);
            }
            set
            {
                this.selectionAnchorPosition = value;
                this.selectionFocusPosition = value;
            }
        }

        protected int caretPositionInternal
        {
            get
            {
                return (this.m_CaretPosition + Input.compositionString.Length);
            }
            set
            {
                this.m_CaretPosition = value;
                this.ClampPos(ref this.m_CaretPosition);
            }
        }

        [Obsolete("caretSelectPosition has been deprecated. Use selectionFocusPosition instead (UnityUpgradable) -> selectionFocusPosition", true)]
        public int caretSelectPosition
        {
            get
            {
                return this.selectionFocusPosition;
            }
            protected set
            {
                this.selectionFocusPosition = value;
            }
        }

        protected int caretSelectPositionInternal
        {
            get
            {
                return (this.m_CaretSelectPosition + Input.compositionString.Length);
            }
            set
            {
                this.m_CaretSelectPosition = value;
                this.ClampPos(ref this.m_CaretSelectPosition);
            }
        }

        public int characterLimit
        {
            get
            {
                return this.m_CharacterLimit;
            }
            set
            {
                SetPropertyUtility.SetStruct<int>(ref this.m_CharacterLimit, value);
            }
        }

        public CharacterValidation characterValidation
        {
            get
            {
                return this.m_CharacterValidation;
            }
            set
            {
                if (SetPropertyUtility.SetStruct<CharacterValidation>(ref this.m_CharacterValidation, value))
                {
                    this.SetToCustom();
                }
            }
        }

        private static string clipboard
        {
            get
            {
                return GUIUtility.systemCopyBuffer;
            }
            set
            {
                GUIUtility.systemCopyBuffer = value;
            }
        }

        public ContentType contentType
        {
            get
            {
                return this.m_ContentType;
            }
            set
            {
                if (SetPropertyUtility.SetStruct<ContentType>(ref this.m_ContentType, value))
                {
                    this.EnforceContentType();
                }
            }
        }

        private bool hasSelection
        {
            get
            {
                return (this.caretPositionInternal != this.caretSelectPositionInternal);
            }
        }

        public InputType inputType
        {
            get
            {
                return this.m_InputType;
            }
            set
            {
                if (SetPropertyUtility.SetStruct<InputType>(ref this.m_InputType, value))
                {
                    this.SetToCustom();
                }
            }
        }

        public bool isFocused
        {
            get
            {
                return this.m_AllowInput;
            }
        }

        public TouchScreenKeyboardType keyboardType
        {
            get
            {
                return this.m_KeyboardType;
            }
            set
            {
                if (SetPropertyUtility.SetStruct<TouchScreenKeyboardType>(ref this.m_KeyboardType, value))
                {
                    this.SetToCustom();
                }
            }
        }

        public LineType lineType
        {
            get
            {
                return this.m_LineType;
            }
            set
            {
                if (SetPropertyUtility.SetStruct<LineType>(ref this.m_LineType, value))
                {
                    ContentType[] allowedContentTypes = new ContentType[2];
                    allowedContentTypes[1] = ContentType.Autocorrected;
                    this.SetToCustomIfContentTypeIsNot(allowedContentTypes);
                }
            }
        }

        protected Mesh mesh
        {
            get
            {
                if (this.m_Mesh == null)
                {
                    this.m_Mesh = new Mesh();
                }
                return this.m_Mesh;
            }
        }

        public bool multiLine
        {
            get
            {
                return ((this.m_LineType == LineType.MultiLineNewline) || (this.lineType == LineType.MultiLineSubmit));
            }
        }

        public SubmitEvent onEndEdit
        {
            get
            {
                return this.m_EndEdit;
            }
            set
            {
                SetPropertyUtility.SetClass<SubmitEvent>(ref this.m_EndEdit, value);
            }
        }

        public OnValidateInput onValidateInput
        {
            get
            {
                return this.m_OnValidateInput;
            }
            set
            {
                SetPropertyUtility.SetClass<OnValidateInput>(ref this.m_OnValidateInput, value);
            }
        }

        public OnChangeEvent onValueChange
        {
            get
            {
                return this.m_OnValueChange;
            }
            set
            {
                SetPropertyUtility.SetClass<OnChangeEvent>(ref this.m_OnValueChange, value);
            }
        }

        public Graphic placeholder
        {
            get
            {
                return this.m_Placeholder;
            }
            set
            {
                SetPropertyUtility.SetClass<Graphic>(ref this.m_Placeholder, value);
            }
        }

        public int selectionAnchorPosition
        {
            get
            {
                return (this.m_CaretPosition + Input.compositionString.Length);
            }
            set
            {
                if (Input.compositionString.Length == 0)
                {
                    this.m_CaretPosition = value;
                    this.ClampPos(ref this.m_CaretPosition);
                }
            }
        }

        public Color selectionColor
        {
            get
            {
                return this.m_SelectionColor;
            }
            set
            {
                SetPropertyUtility.SetColor(ref this.m_SelectionColor, value);
            }
        }

        public int selectionFocusPosition
        {
            get
            {
                return (this.m_CaretSelectPosition + Input.compositionString.Length);
            }
            set
            {
                if (Input.compositionString.Length == 0)
                {
                    this.m_CaretSelectPosition = value;
                    this.ClampPos(ref this.m_CaretSelectPosition);
                }
            }
        }

        public bool shouldHideMobileInput
        {
            get
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.IPhonePlayer:
                    case RuntimePlatform.Android:
                    case RuntimePlatform.BB10Player:
                        return this.m_HideMobileInput;
                }
                return true;
            }
            set
            {
                SetPropertyUtility.SetStruct<bool>(ref this.m_HideMobileInput, value);
            }
        }

        public string text
        {
            get
            {
                if (((m_Keyboard != null) && m_Keyboard.active) && (!this.InPlaceEditing() && (EventSystem.current.currentSelectedGameObject == base.gameObject)))
                {
                    return m_Keyboard.text;
                }
                return this.m_Text;
            }
            set
            {
                if (this.text != value)
                {
                    this.m_Text = value;
                    if (!Application.isPlaying)
                    {
                        this.SendOnValueChangedAndUpdateLabel();
                    }
                    else
                    {
                        if (m_Keyboard != null)
                        {
                            m_Keyboard.text = this.m_Text;
                        }
                        if (this.m_CaretPosition > this.m_Text.Length)
                        {
                            this.m_CaretPosition = this.m_CaretSelectPosition = this.m_Text.Length;
                        }
                        this.SendOnValueChangedAndUpdateLabel();
                    }
                }
            }
        }

        public Text textComponent
        {
            get
            {
                return this.m_TextComponent;
            }
            set
            {
                SetPropertyUtility.SetClass<Text>(ref this.m_TextComponent, value);
            }
        }

        public bool wasCanceled
        {
            get
            {
                return this.m_WasCanceled;
            }
        }

        [CompilerGenerated]
        private sealed class <CaretBlink>c__Iterator3 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal InputField <>f__this;
            internal float <blinkPeriod>__0;
            internal bool <blinkState>__1;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<>f__this.m_CaretVisible = true;
                        this.$current = null;
                        this.$PC = 1;
                        goto Label_010B;

                    case 1:
                    case 2:
                        if (this.<>f__this.isFocused && (this.<>f__this.m_CaretBlinkRate > 0f))
                        {
                            this.<blinkPeriod>__0 = 1f / this.<>f__this.m_CaretBlinkRate;
                            this.<blinkState>__1 = ((Time.unscaledTime - this.<>f__this.m_BlinkStartTime) % this.<blinkPeriod>__0) < (this.<blinkPeriod>__0 / 2f);
                            if (this.<>f__this.m_CaretVisible != this.<blinkState>__1)
                            {
                                this.<>f__this.m_CaretVisible = this.<blinkState>__1;
                                this.<>f__this.UpdateGeometry();
                            }
                            this.$current = null;
                            this.$PC = 2;
                            goto Label_010B;
                        }
                        this.<>f__this.m_BlinkCoroutine = null;
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_010B:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <MouseDragOutsideRect>c__Iterator4 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal PointerEventData <$>eventData;
            internal InputField <>f__this;
            internal float <delay>__2;
            internal Vector2 <localMousePos>__0;
            internal Rect <rect>__1;
            internal PointerEventData eventData;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                    case 1:
                        if (this.<>f__this.m_UpdateDrag && this.<>f__this.m_DragPositionOutOfBounds)
                        {
                            RectTransformUtility.ScreenPointToLocalPointInRectangle(this.<>f__this.textComponent.rectTransform, this.eventData.position, this.eventData.pressEventCamera, out this.<localMousePos>__0);
                            this.<rect>__1 = this.<>f__this.textComponent.rectTransform.rect;
                            if (this.<>f__this.multiLine)
                            {
                                if (this.<localMousePos>__0.y > this.<rect>__1.yMax)
                                {
                                    this.<>f__this.MoveUp(true, true);
                                }
                                else if (this.<localMousePos>__0.y < this.<rect>__1.yMin)
                                {
                                    this.<>f__this.MoveDown(true, true);
                                }
                            }
                            else if (this.<localMousePos>__0.x < this.<rect>__1.xMin)
                            {
                                this.<>f__this.MoveLeft(true, false);
                            }
                            else if (this.<localMousePos>__0.x > this.<rect>__1.xMax)
                            {
                                this.<>f__this.MoveRight(true, false);
                            }
                            this.<>f__this.UpdateLabel();
                            this.<delay>__2 = !this.<>f__this.multiLine ? 0.05f : 0.1f;
                            this.$current = new WaitForSeconds(this.<delay>__2);
                            this.$PC = 1;
                            return true;
                        }
                        this.<>f__this.m_DragCoroutine = null;
                        this.$PC = -1;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        public enum CharacterValidation
        {
            None,
            Integer,
            Decimal,
            Alphanumeric,
            Name,
            EmailAddress
        }

        public enum ContentType
        {
            Standard,
            Autocorrected,
            IntegerNumber,
            DecimalNumber,
            Alphanumeric,
            Name,
            EmailAddress,
            Password,
            Pin,
            Custom
        }

        protected enum EditState
        {
            Continue,
            Finish
        }

        public enum InputType
        {
            Standard,
            AutoCorrect,
            Password
        }

        public enum LineType
        {
            SingleLine,
            MultiLineSubmit,
            MultiLineNewline
        }

        [Serializable]
        public class OnChangeEvent : UnityEvent<string>
        {
        }

        public delegate char OnValidateInput(string text, int charIndex, char addedChar);

        [Serializable]
        public class SubmitEvent : UnityEvent<string>
        {
        }
    }
}

