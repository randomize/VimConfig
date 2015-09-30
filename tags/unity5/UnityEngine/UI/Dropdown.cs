namespace UnityEngine.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI.CoroutineTween;

    [AddComponentMenu("UI/Dropdown", 0x23), RequireComponent(typeof(RectTransform))]
    public class Dropdown : Selectable, IEventSystemHandler, IPointerClickHandler, ISubmitHandler, ICancelHandler
    {
        private TweenRunner<FloatTween> m_AlphaTweenRunner;
        private GameObject m_Blocker;
        [SerializeField]
        private Image m_CaptionImage;
        [SerializeField]
        private Text m_CaptionText;
        private GameObject m_Dropdown;
        [SerializeField]
        private Image m_ItemImage;
        private List<DropdownItem> m_Items = new List<DropdownItem>();
        [Space, SerializeField]
        private Text m_ItemText;
        [SerializeField, Space]
        private DropdownEvent m_OnValueChanged = new DropdownEvent();
        [SerializeField, Space]
        private OptionDataList m_Options = new OptionDataList();
        [SerializeField]
        private RectTransform m_Template;
        [Space, SerializeField]
        private int m_Value;
        private bool validTemplate;

        protected Dropdown()
        {
        }

        private DropdownItem AddItem(OptionData data, bool selected, DropdownItem itemTemplate, List<DropdownItem> items)
        {
            DropdownItem item = this.CreateItem(itemTemplate);
            item.rectTransform.SetParent(itemTemplate.rectTransform.parent, false);
            item.gameObject.SetActive(true);
            item.gameObject.name = "Item " + items.Count + ((data.text == null) ? string.Empty : (": " + data.text));
            if (item.toggle != null)
            {
                item.toggle.isOn = false;
            }
            if (item.text != null)
            {
                item.text.text = data.text;
            }
            if (item.image != null)
            {
                item.image.sprite = data.image;
                item.image.enabled = item.image.sprite != null;
            }
            items.Add(item);
            return item;
        }

        private void AlphaFadeList(float duration, float alpha)
        {
            CanvasGroup component = this.m_Dropdown.GetComponent<CanvasGroup>();
            this.AlphaFadeList(duration, component.alpha, alpha);
        }

        private void AlphaFadeList(float duration, float start, float end)
        {
            if (!end.Equals(start))
            {
                FloatTween info = new FloatTween {
                    duration = duration,
                    startValue = start,
                    targetValue = end
                };
                info.AddOnChangedCallback(new UnityAction<float>(this.SetAlpha));
                info.ignoreTimeScale = true;
                this.m_AlphaTweenRunner.StartTween(info);
            }
        }

        protected override void Awake()
        {
            if (Application.isPlaying)
            {
                this.m_AlphaTweenRunner = new TweenRunner<FloatTween>();
                this.m_AlphaTweenRunner.Init(this);
                if (this.m_CaptionImage != null)
                {
                    this.m_CaptionImage.enabled = this.m_CaptionImage.sprite != null;
                }
                if (this.m_Template != null)
                {
                    this.m_Template.gameObject.SetActive(false);
                }
            }
        }

        protected virtual GameObject CreateBlocker(Canvas rootCanvas)
        {
            GameObject obj2 = new GameObject("Blocker");
            RectTransform transform = obj2.AddComponent<RectTransform>();
            transform.SetParent(rootCanvas.transform, false);
            transform.anchorMin = Vector3.zero;
            transform.anchorMax = Vector3.one;
            transform.sizeDelta = Vector2.zero;
            Canvas canvas = obj2.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            Canvas component = this.m_Dropdown.GetComponent<Canvas>();
            canvas.sortingLayerID = component.sortingLayerID;
            canvas.sortingOrder = component.sortingOrder - 1;
            obj2.AddComponent<GraphicRaycaster>();
            obj2.AddComponent<Image>().color = Color.clear;
            obj2.AddComponent<Button>().onClick.AddListener(new UnityAction(this.Hide));
            return obj2;
        }

        protected virtual GameObject CreateDropdownList(GameObject template)
        {
            return UnityEngine.Object.Instantiate<GameObject>(template);
        }

        protected virtual DropdownItem CreateItem(DropdownItem itemTemplate)
        {
            return UnityEngine.Object.Instantiate<DropdownItem>(itemTemplate);
        }

        [DebuggerHidden]
        private IEnumerator DelayedDestroyDropdownList(float delay)
        {
            return new <DelayedDestroyDropdownList>c__Iterator2 { delay = delay, <$>delay = delay, <>f__this = this };
        }

        protected virtual void DestroyBlocker(GameObject blocker)
        {
            UnityEngine.Object.Destroy(blocker);
        }

        protected virtual void DestroyDropdownList(GameObject dropdownList)
        {
            UnityEngine.Object.Destroy(dropdownList);
        }

        protected virtual void DestroyItem(DropdownItem item)
        {
        }

        private static T GetOrAddComponent<T>(GameObject go) where T: Component
        {
            T component = go.GetComponent<T>();
            if (component == null)
            {
                component = go.AddComponent<T>();
            }
            return component;
        }

        public void Hide()
        {
            if (this.m_Dropdown != null)
            {
                this.AlphaFadeList(0.15f, 0f);
                base.StartCoroutine(this.DelayedDestroyDropdownList(0.15f));
            }
            if (this.m_Blocker != null)
            {
                this.DestroyBlocker(this.m_Blocker);
            }
            this.m_Blocker = null;
            this.Select();
        }

        public virtual void OnCancel(BaseEventData eventData)
        {
            this.Hide();
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            this.Show();
        }

        private void OnSelectItem(Toggle toggle)
        {
            if (!toggle.isOn)
            {
                toggle.isOn = true;
            }
            int num = -1;
            Transform transform = toggle.transform;
            Transform parent = transform.parent;
            for (int i = 0; i < parent.childCount; i++)
            {
                if (parent.GetChild(i) == transform)
                {
                    num = i - 1;
                    break;
                }
            }
            if (num >= 0)
            {
                this.value = num;
                this.Hide();
            }
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            this.Show();
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            if (this.IsActive())
            {
                this.Refresh();
            }
        }

        private void Refresh()
        {
            if (this.options.Count != 0)
            {
                OptionData data = this.options[Mathf.Clamp(this.m_Value, 0, this.options.Count - 1)];
                if (this.m_CaptionText != null)
                {
                    if ((data != null) && (data.text != null))
                    {
                        this.m_CaptionText.text = data.text;
                    }
                    else
                    {
                        this.m_CaptionText.text = string.Empty;
                    }
                }
                if (this.m_CaptionImage != null)
                {
                    if (data != null)
                    {
                        this.m_CaptionImage.sprite = data.image;
                    }
                    else
                    {
                        this.m_CaptionImage.sprite = null;
                    }
                    this.m_CaptionImage.enabled = this.m_CaptionImage.sprite != null;
                }
            }
        }

        private void SetAlpha(float alpha)
        {
            if (this.m_Dropdown != null)
            {
                this.m_Dropdown.GetComponent<CanvasGroup>().alpha = alpha;
            }
        }

        private void SetupTemplate()
        {
            this.validTemplate = false;
            if (this.m_Template == null)
            {
                UnityEngine.Debug.LogError("The dropdown template is not assigned. The template needs to be assigned and must have a child GameObject with a Toggle component serving as the item.", this);
            }
            else
            {
                GameObject gameObject = this.m_Template.gameObject;
                gameObject.SetActive(true);
                Toggle componentInChildren = this.m_Template.GetComponentInChildren<Toggle>();
                this.validTemplate = true;
                if ((componentInChildren == null) || (componentInChildren.transform == this.template))
                {
                    this.validTemplate = false;
                    UnityEngine.Debug.LogError("The dropdown template is not valid. The template must have a child GameObject with a Toggle component serving as the item.", this.template);
                }
                else if (componentInChildren.transform.parent is RectTransform)
                {
                    if ((this.itemText != null) && !this.itemText.transform.IsChildOf(componentInChildren.transform))
                    {
                        this.validTemplate = false;
                        UnityEngine.Debug.LogError("The dropdown template is not valid. The Item Text must be on the item GameObject or children of it.", this.template);
                    }
                    else if ((this.itemImage != null) && !this.itemImage.transform.IsChildOf(componentInChildren.transform))
                    {
                        this.validTemplate = false;
                        UnityEngine.Debug.LogError("The dropdown template is not valid. The Item Image must be on the item GameObject or children of it.", this.template);
                    }
                }
                else
                {
                    this.validTemplate = false;
                    UnityEngine.Debug.LogError("The dropdown template is not valid. The child GameObject with a Toggle component (the item) must have a RectTransform on its parent.", this.template);
                }
                if (!this.validTemplate)
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    DropdownItem item = componentInChildren.gameObject.AddComponent<DropdownItem>();
                    item.text = this.m_ItemText;
                    item.image = this.m_ItemImage;
                    item.toggle = componentInChildren;
                    item.rectTransform = (RectTransform) componentInChildren.transform;
                    Canvas orAddComponent = GetOrAddComponent<Canvas>(gameObject);
                    orAddComponent.overrideSorting = true;
                    orAddComponent.sortingOrder = 0x7530;
                    GetOrAddComponent<GraphicRaycaster>(gameObject);
                    GetOrAddComponent<CanvasGroup>(gameObject);
                    gameObject.SetActive(false);
                    this.validTemplate = true;
                }
            }
        }

        public void Show()
        {
            if ((this.IsActive() && this.IsInteractable()) && (this.m_Dropdown == null))
            {
                if (!this.validTemplate)
                {
                    this.SetupTemplate();
                    if (!this.validTemplate)
                    {
                        return;
                    }
                }
                List<Canvas> results = ListPool<Canvas>.Get();
                base.gameObject.GetComponentsInParent<Canvas>(false, results);
                if (results.Count != 0)
                {
                    Canvas rootCanvas = results[0];
                    ListPool<Canvas>.Release(results);
                    this.m_Template.gameObject.SetActive(true);
                    this.m_Dropdown = this.CreateDropdownList(this.m_Template.gameObject);
                    this.m_Dropdown.name = "Dropdown List";
                    this.m_Dropdown.SetActive(true);
                    RectTransform transform = this.m_Dropdown.transform as RectTransform;
                    transform.SetParent(this.m_Template.transform.parent, false);
                    DropdownItem componentInChildren = this.m_Dropdown.GetComponentInChildren<DropdownItem>();
                    RectTransform transform2 = componentInChildren.rectTransform.parent.gameObject.transform as RectTransform;
                    componentInChildren.rectTransform.gameObject.SetActive(true);
                    Rect rect = transform2.rect;
                    Rect rect2 = componentInChildren.rectTransform.rect;
                    Vector2 vector = (rect2.min - rect.min) + componentInChildren.rectTransform.localPosition;
                    Vector2 vector2 = (rect2.max - rect.max) + componentInChildren.rectTransform.localPosition;
                    Vector2 size = rect2.size;
                    this.m_Items.Clear();
                    Toggle toggle = null;
                    for (int i = 0; i < this.options.Count; i++)
                    {
                        <Show>c__AnonStorey6 storey = new <Show>c__AnonStorey6 {
                            <>f__this = this
                        };
                        OptionData data = this.options[i];
                        storey.item = this.AddItem(data, this.value == i, componentInChildren, this.m_Items);
                        if (storey.item != null)
                        {
                            storey.item.toggle.isOn = this.value == i;
                            storey.item.toggle.onValueChanged.AddListener(new UnityAction<bool>(storey.<>m__4));
                            if (storey.item.toggle.isOn)
                            {
                                storey.item.toggle.Select();
                            }
                            if (toggle != null)
                            {
                                Navigation navigation = toggle.navigation;
                                Navigation navigation2 = storey.item.toggle.navigation;
                                navigation.mode = Navigation.Mode.Explicit;
                                navigation2.mode = Navigation.Mode.Explicit;
                                navigation.selectOnDown = storey.item.toggle;
                                navigation.selectOnRight = storey.item.toggle;
                                navigation2.selectOnLeft = toggle;
                                navigation2.selectOnUp = toggle;
                                toggle.navigation = navigation;
                                storey.item.toggle.navigation = navigation2;
                            }
                            toggle = storey.item.toggle;
                        }
                    }
                    Vector2 sizeDelta = transform2.sizeDelta;
                    sizeDelta.y = ((size.y * this.m_Items.Count) + vector.y) - vector2.y;
                    transform2.sizeDelta = sizeDelta;
                    float num2 = transform.rect.height - transform2.rect.height;
                    if (num2 > 0f)
                    {
                        transform.sizeDelta = new Vector2(transform.sizeDelta.x, transform.sizeDelta.y - num2);
                    }
                    Vector3[] fourCornersArray = new Vector3[4];
                    transform.GetWorldCorners(fourCornersArray);
                    bool flag = false;
                    RectTransform transform3 = rootCanvas.transform as RectTransform;
                    for (int j = 0; j < 4; j++)
                    {
                        Vector3 point = transform3.InverseTransformPoint(fourCornersArray[j]);
                        if (!transform3.rect.Contains(point))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        RectTransformUtility.FlipLayoutOnAxis(transform, 0, false, false);
                        RectTransformUtility.FlipLayoutOnAxis(transform, 1, false, false);
                    }
                    for (int k = 0; k < this.m_Items.Count; k++)
                    {
                        RectTransform rectTransform = this.m_Items[k].rectTransform;
                        rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x, 0f);
                        rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x, 0f);
                        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, (vector.y + (size.y * ((this.m_Items.Count - 1) - k))) + (size.y * rectTransform.pivot.y));
                        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, size.y);
                    }
                    this.AlphaFadeList(0.15f, 0f, 1f);
                    this.m_Template.gameObject.SetActive(false);
                    componentInChildren.gameObject.SetActive(false);
                    this.m_Blocker = this.CreateBlocker(rootCanvas);
                }
            }
        }

        public Image captionImage
        {
            get
            {
                return this.m_CaptionImage;
            }
            set
            {
                this.m_CaptionImage = value;
                this.Refresh();
            }
        }

        public Text captionText
        {
            get
            {
                return this.m_CaptionText;
            }
            set
            {
                this.m_CaptionText = value;
                this.Refresh();
            }
        }

        public Image itemImage
        {
            get
            {
                return this.m_ItemImage;
            }
            set
            {
                this.m_ItemImage = value;
                this.Refresh();
            }
        }

        public Text itemText
        {
            get
            {
                return this.m_ItemText;
            }
            set
            {
                this.m_ItemText = value;
                this.Refresh();
            }
        }

        public DropdownEvent onValueChanged
        {
            get
            {
                return this.m_OnValueChanged;
            }
            set
            {
                this.m_OnValueChanged = value;
            }
        }

        public List<OptionData> options
        {
            get
            {
                return this.m_Options.options;
            }
            set
            {
                this.m_Options.options = value;
                this.Refresh();
            }
        }

        public RectTransform template
        {
            get
            {
                return this.m_Template;
            }
            set
            {
                this.m_Template = value;
                this.Refresh();
            }
        }

        public int value
        {
            get
            {
                return this.m_Value;
            }
            set
            {
                if (!Application.isPlaying || ((value != this.m_Value) && (this.options.Count != 0)))
                {
                    this.m_Value = Mathf.Clamp(value, 0, this.options.Count - 1);
                    this.Refresh();
                    this.m_OnValueChanged.Invoke(this.m_Value);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <DelayedDestroyDropdownList>c__Iterator2 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal float <$>delay;
            internal Dropdown <>f__this;
            internal int <i>__0;
            internal float delay;

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
                        this.$current = new WaitForSeconds(this.delay);
                        this.$PC = 1;
                        return true;

                    case 1:
                        this.<i>__0 = 0;
                        while (this.<i>__0 < this.<>f__this.m_Items.Count)
                        {
                            if (this.<>f__this.m_Items[this.<i>__0] != null)
                            {
                                this.<>f__this.DestroyItem(this.<>f__this.m_Items[this.<i>__0]);
                            }
                            this.<>f__this.m_Items.Clear();
                            this.<i>__0++;
                        }
                        if (this.<>f__this.m_Dropdown != null)
                        {
                            this.<>f__this.DestroyDropdownList(this.<>f__this.m_Dropdown);
                        }
                        this.<>f__this.m_Dropdown = null;
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

        [CompilerGenerated]
        private sealed class <Show>c__AnonStorey6
        {
            internal Dropdown <>f__this;
            internal Dropdown.DropdownItem item;

            internal void <>m__4(bool x)
            {
                this.<>f__this.OnSelectItem(this.item.toggle);
            }
        }

        [Serializable]
        public class DropdownEvent : UnityEvent<int>
        {
        }

        internal protected class DropdownItem : MonoBehaviour, IEventSystemHandler, IPointerEnterHandler, ICancelHandler
        {
            [SerializeField]
            private Image m_Image;
            [SerializeField]
            private RectTransform m_RectTransform;
            [SerializeField]
            private Text m_Text;
            [SerializeField]
            private Toggle m_Toggle;

            public virtual void OnCancel(BaseEventData eventData)
            {
                Dropdown componentInParent = base.GetComponentInParent<Dropdown>();
                if (componentInParent != null)
                {
                    componentInParent.Hide();
                }
            }

            public virtual void OnPointerEnter(PointerEventData eventData)
            {
                EventSystem.current.SetSelectedGameObject(base.gameObject);
            }

            public Image image
            {
                get
                {
                    return this.m_Image;
                }
                set
                {
                    this.m_Image = value;
                }
            }

            public RectTransform rectTransform
            {
                get
                {
                    return this.m_RectTransform;
                }
                set
                {
                    this.m_RectTransform = value;
                }
            }

            public Text text
            {
                get
                {
                    return this.m_Text;
                }
                set
                {
                    this.m_Text = value;
                }
            }

            public Toggle toggle
            {
                get
                {
                    return this.m_Toggle;
                }
                set
                {
                    this.m_Toggle = value;
                }
            }
        }

        [Serializable]
        public class OptionData
        {
            [SerializeField]
            private Sprite m_Image;
            [SerializeField]
            private string m_Text;

            public OptionData()
            {
            }

            public OptionData(string text)
            {
                this.text = text;
            }

            public OptionData(Sprite image)
            {
                this.image = image;
            }

            public OptionData(string text, Sprite image)
            {
                this.text = text;
                this.image = image;
            }

            public Sprite image
            {
                get
                {
                    return this.m_Image;
                }
                set
                {
                    this.m_Image = value;
                }
            }

            public string text
            {
                get
                {
                    return this.m_Text;
                }
                set
                {
                    this.m_Text = value;
                }
            }
        }

        [Serializable]
        public class OptionDataList
        {
            [SerializeField]
            private List<Dropdown.OptionData> m_Options;

            public OptionDataList()
            {
                this.options = new List<Dropdown.OptionData>();
            }

            public List<Dropdown.OptionData> options
            {
                get
                {
                    return this.m_Options;
                }
                set
                {
                    this.m_Options = value;
                }
            }
        }
    }
}

