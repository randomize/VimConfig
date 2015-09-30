namespace UnityEngine.UI
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public static class DefaultControls
    {
        private const float kThickHeight = 30f;
        private const float kThinHeight = 20f;
        private const float kWidth = 160f;
        private static Color s_DefaultSelectableColor = new Color(1f, 1f, 1f, 1f);
        private static Vector2 s_ImageElementSize = new Vector2(100f, 100f);
        private static Color s_PanelColor = new Color(1f, 1f, 1f, 0.392f);
        private static Color s_TextColor = new Color(0.1960784f, 0.1960784f, 0.1960784f, 1f);
        private static Vector2 s_ThickElementSize = new Vector2(160f, 30f);
        private static Vector2 s_ThinElementSize = new Vector2(160f, 20f);

        public static GameObject CreateButton(Resources resources)
        {
            GameObject parent = CreateUIElementRoot("Button", s_ThickElementSize);
            GameObject child = new GameObject("Text");
            SetParentAndAlign(child, parent);
            Image image = parent.AddComponent<Image>();
            image.sprite = resources.standard;
            image.type = Image.Type.Sliced;
            image.color = s_DefaultSelectableColor;
            SetDefaultColorTransitionValues(parent.AddComponent<Button>());
            Text lbl = child.AddComponent<Text>();
            lbl.text = "Button";
            lbl.alignment = TextAnchor.MiddleCenter;
            SetDefaultTextValues(lbl);
            RectTransform component = child.GetComponent<RectTransform>();
            component.anchorMin = Vector2.zero;
            component.anchorMax = Vector2.one;
            component.sizeDelta = Vector2.zero;
            return parent;
        }

        public static GameObject CreateDropdown(Resources resources)
        {
            GameObject parent = CreateUIElementRoot("Dropdown", s_ThickElementSize);
            GameObject obj3 = CreateUIObject("Label", parent);
            GameObject obj4 = CreateUIObject("Arrow", parent);
            GameObject obj5 = CreateUIObject("Template", parent);
            GameObject obj6 = CreateUIObject("Viewport", obj5);
            GameObject obj7 = CreateUIObject("Content", obj6);
            GameObject obj8 = CreateUIObject("Item", obj7);
            GameObject obj9 = CreateUIObject("Item Background", obj8);
            GameObject obj10 = CreateUIObject("Item Checkmark", obj8);
            GameObject obj11 = CreateUIObject("Item Label", obj8);
            GameObject child = CreateScrollbar(resources);
            child.name = "Scrollbar";
            SetParentAndAlign(child, obj5);
            Scrollbar component = child.GetComponent<Scrollbar>();
            component.SetDirection(Scrollbar.Direction.BottomToTop, true);
            RectTransform transform = child.GetComponent<RectTransform>();
            transform.anchorMin = Vector2.right;
            transform.anchorMax = Vector2.one;
            transform.pivot = Vector2.one;
            transform.sizeDelta = new Vector2(transform.sizeDelta.x, 0f);
            Text lbl = obj11.AddComponent<Text>();
            SetDefaultTextValues(lbl);
            lbl.alignment = TextAnchor.MiddleLeft;
            Image image = obj9.AddComponent<Image>();
            image.color = (Color) new Color32(0xf5, 0xf5, 0xf5, 0xff);
            Image image2 = obj10.AddComponent<Image>();
            image2.sprite = resources.checkmark;
            Toggle toggle = obj8.AddComponent<Toggle>();
            toggle.targetGraphic = image;
            toggle.graphic = image2;
            toggle.isOn = true;
            Image image3 = obj5.AddComponent<Image>();
            image3.sprite = resources.standard;
            image3.type = Image.Type.Sliced;
            ScrollRect rect = obj5.AddComponent<ScrollRect>();
            rect.content = (RectTransform) obj7.transform;
            rect.viewport = (RectTransform) obj6.transform;
            rect.horizontal = false;
            rect.movementType = ScrollRect.MovementType.Clamped;
            rect.verticalScrollbar = component;
            rect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            rect.verticalScrollbarSpacing = -3f;
            obj6.AddComponent<Mask>().showMaskGraphic = false;
            Image image4 = obj6.AddComponent<Image>();
            image4.sprite = resources.mask;
            image4.type = Image.Type.Sliced;
            Text text2 = obj3.AddComponent<Text>();
            SetDefaultTextValues(text2);
            text2.text = "Option A";
            text2.alignment = TextAnchor.MiddleLeft;
            obj4.AddComponent<Image>().sprite = resources.dropdown;
            Image image6 = parent.AddComponent<Image>();
            image6.sprite = resources.standard;
            image6.color = s_DefaultSelectableColor;
            image6.type = Image.Type.Sliced;
            Dropdown slider = parent.AddComponent<Dropdown>();
            slider.targetGraphic = image6;
            SetDefaultColorTransitionValues(slider);
            slider.template = obj5.GetComponent<RectTransform>();
            slider.captionText = text2;
            slider.itemText = lbl;
            lbl.text = "Option A";
            Dropdown.OptionData item = new Dropdown.OptionData {
                text = "Option A"
            };
            slider.options.Add(item);
            item = new Dropdown.OptionData {
                text = "Option B"
            };
            slider.options.Add(item);
            item = new Dropdown.OptionData {
                text = "Option C"
            };
            slider.options.Add(item);
            RectTransform transform2 = obj3.GetComponent<RectTransform>();
            transform2.anchorMin = Vector2.zero;
            transform2.anchorMax = Vector2.one;
            transform2.offsetMin = new Vector2(10f, 6f);
            transform2.offsetMax = new Vector2(-25f, -7f);
            RectTransform transform3 = obj4.GetComponent<RectTransform>();
            transform3.anchorMin = new Vector2(1f, 0.5f);
            transform3.anchorMax = new Vector2(1f, 0.5f);
            transform3.sizeDelta = new Vector2(20f, 20f);
            transform3.anchoredPosition = new Vector2(-15f, 0f);
            RectTransform transform4 = obj5.GetComponent<RectTransform>();
            transform4.anchorMin = new Vector2(0f, 0f);
            transform4.anchorMax = new Vector2(1f, 0f);
            transform4.pivot = new Vector2(0.5f, 1f);
            transform4.anchoredPosition = new Vector2(0f, 2f);
            transform4.sizeDelta = new Vector2(0f, 150f);
            RectTransform transform5 = obj6.GetComponent<RectTransform>();
            transform5.anchorMin = new Vector2(0f, 0f);
            transform5.anchorMax = new Vector2(1f, 1f);
            transform5.sizeDelta = new Vector2(-18f, 0f);
            transform5.pivot = new Vector2(0f, 1f);
            RectTransform transform6 = obj7.GetComponent<RectTransform>();
            transform6.anchorMin = new Vector2(0f, 1f);
            transform6.anchorMax = new Vector2(1f, 1f);
            transform6.pivot = new Vector2(0.5f, 1f);
            transform6.anchoredPosition = new Vector2(0f, 0f);
            transform6.sizeDelta = new Vector2(0f, 28f);
            RectTransform transform7 = obj8.GetComponent<RectTransform>();
            transform7.anchorMin = new Vector2(0f, 0.5f);
            transform7.anchorMax = new Vector2(1f, 0.5f);
            transform7.sizeDelta = new Vector2(0f, 20f);
            RectTransform transform8 = obj9.GetComponent<RectTransform>();
            transform8.anchorMin = Vector2.zero;
            transform8.anchorMax = Vector2.one;
            transform8.sizeDelta = Vector2.zero;
            RectTransform transform9 = obj10.GetComponent<RectTransform>();
            transform9.anchorMin = new Vector2(0f, 0.5f);
            transform9.anchorMax = new Vector2(0f, 0.5f);
            transform9.sizeDelta = new Vector2(20f, 20f);
            transform9.anchoredPosition = new Vector2(10f, 0f);
            RectTransform transform10 = obj11.GetComponent<RectTransform>();
            transform10.anchorMin = Vector2.zero;
            transform10.anchorMax = Vector2.one;
            transform10.offsetMin = new Vector2(20f, 1f);
            transform10.offsetMax = new Vector2(-10f, -2f);
            obj5.SetActive(false);
            return parent;
        }

        public static GameObject CreateImage(Resources resources)
        {
            GameObject obj2 = CreateUIElementRoot("Image", s_ImageElementSize);
            obj2.AddComponent<Image>();
            return obj2;
        }

        public static GameObject CreateInputField(Resources resources)
        {
            GameObject parent = CreateUIElementRoot("InputField", s_ThickElementSize);
            GameObject obj3 = CreateUIObject("Placeholder", parent);
            GameObject obj4 = CreateUIObject("Text", parent);
            Image image = parent.AddComponent<Image>();
            image.sprite = resources.inputField;
            image.type = Image.Type.Sliced;
            image.color = s_DefaultSelectableColor;
            InputField slider = parent.AddComponent<InputField>();
            SetDefaultColorTransitionValues(slider);
            Text lbl = obj4.AddComponent<Text>();
            lbl.text = string.Empty;
            lbl.supportRichText = false;
            SetDefaultTextValues(lbl);
            Text text2 = obj3.AddComponent<Text>();
            text2.text = "Enter text...";
            text2.fontStyle = FontStyle.Italic;
            Color color = lbl.color;
            color.a *= 0.5f;
            text2.color = color;
            RectTransform component = obj4.GetComponent<RectTransform>();
            component.anchorMin = Vector2.zero;
            component.anchorMax = Vector2.one;
            component.sizeDelta = Vector2.zero;
            component.offsetMin = new Vector2(10f, 6f);
            component.offsetMax = new Vector2(-10f, -7f);
            RectTransform transform2 = obj3.GetComponent<RectTransform>();
            transform2.anchorMin = Vector2.zero;
            transform2.anchorMax = Vector2.one;
            transform2.sizeDelta = Vector2.zero;
            transform2.offsetMin = new Vector2(10f, 6f);
            transform2.offsetMax = new Vector2(-10f, -7f);
            slider.textComponent = lbl;
            slider.placeholder = text2;
            return parent;
        }

        public static GameObject CreatePanel(Resources resources)
        {
            GameObject obj2 = CreateUIElementRoot("Panel", s_ThickElementSize);
            RectTransform component = obj2.GetComponent<RectTransform>();
            component.anchorMin = Vector2.zero;
            component.anchorMax = Vector2.one;
            component.anchoredPosition = Vector2.zero;
            component.sizeDelta = Vector2.zero;
            Image image = obj2.AddComponent<Image>();
            image.sprite = resources.background;
            image.type = Image.Type.Sliced;
            image.color = s_PanelColor;
            return obj2;
        }

        public static GameObject CreateRawImage(Resources resources)
        {
            GameObject obj2 = CreateUIElementRoot("RawImage", s_ImageElementSize);
            obj2.AddComponent<RawImage>();
            return obj2;
        }

        public static GameObject CreateScrollbar(Resources resources)
        {
            GameObject parent = CreateUIElementRoot("Scrollbar", s_ThinElementSize);
            GameObject obj3 = CreateUIObject("Sliding Area", parent);
            GameObject obj4 = CreateUIObject("Handle", obj3);
            Image image = parent.AddComponent<Image>();
            image.sprite = resources.background;
            image.type = Image.Type.Sliced;
            image.color = s_DefaultSelectableColor;
            Image image2 = obj4.AddComponent<Image>();
            image2.sprite = resources.standard;
            image2.type = Image.Type.Sliced;
            image2.color = s_DefaultSelectableColor;
            RectTransform component = obj3.GetComponent<RectTransform>();
            component.sizeDelta = new Vector2(-20f, -20f);
            component.anchorMin = Vector2.zero;
            component.anchorMax = Vector2.one;
            RectTransform transform2 = obj4.GetComponent<RectTransform>();
            transform2.sizeDelta = new Vector2(20f, 20f);
            Scrollbar slider = parent.AddComponent<Scrollbar>();
            slider.handleRect = transform2;
            slider.targetGraphic = image2;
            SetDefaultColorTransitionValues(slider);
            return parent;
        }

        public static GameObject CreateScrollView(Resources resources)
        {
            GameObject parent = CreateUIElementRoot("Scroll View", new Vector2(200f, 200f));
            GameObject obj3 = CreateUIObject("Viewport", parent);
            GameObject obj4 = CreateUIObject("Content", obj3);
            GameObject child = CreateScrollbar(resources);
            child.name = "Scrollbar Horizontal";
            SetParentAndAlign(child, parent);
            RectTransform component = child.GetComponent<RectTransform>();
            component.anchorMin = Vector2.zero;
            component.anchorMax = Vector2.right;
            component.pivot = Vector2.zero;
            component.sizeDelta = new Vector2(0f, component.sizeDelta.y);
            GameObject obj6 = CreateScrollbar(resources);
            obj6.name = "Scrollbar Vertical";
            SetParentAndAlign(obj6, parent);
            obj6.GetComponent<Scrollbar>().SetDirection(Scrollbar.Direction.BottomToTop, true);
            RectTransform transform2 = obj6.GetComponent<RectTransform>();
            transform2.anchorMin = Vector2.right;
            transform2.anchorMax = Vector2.one;
            transform2.pivot = Vector2.one;
            transform2.sizeDelta = new Vector2(transform2.sizeDelta.x, 0f);
            RectTransform transform3 = obj3.GetComponent<RectTransform>();
            transform3.anchorMin = Vector2.zero;
            transform3.anchorMax = Vector2.one;
            transform3.sizeDelta = Vector2.zero;
            transform3.pivot = Vector2.up;
            RectTransform transform4 = obj4.GetComponent<RectTransform>();
            transform4.anchorMin = Vector2.up;
            transform4.anchorMax = Vector2.one;
            transform4.sizeDelta = new Vector2(0f, 300f);
            transform4.pivot = Vector2.up;
            ScrollRect rect = parent.AddComponent<ScrollRect>();
            rect.content = transform4;
            rect.viewport = transform3;
            rect.horizontalScrollbar = child.GetComponent<Scrollbar>();
            rect.verticalScrollbar = obj6.GetComponent<Scrollbar>();
            rect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            rect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            rect.horizontalScrollbarSpacing = -3f;
            rect.verticalScrollbarSpacing = -3f;
            Image image = parent.AddComponent<Image>();
            image.sprite = resources.background;
            image.type = Image.Type.Sliced;
            image.color = s_PanelColor;
            obj3.AddComponent<Mask>().showMaskGraphic = false;
            Image image2 = obj3.AddComponent<Image>();
            image2.sprite = resources.mask;
            image2.type = Image.Type.Sliced;
            return parent;
        }

        public static GameObject CreateSlider(Resources resources)
        {
            GameObject parent = CreateUIElementRoot("Slider", s_ThinElementSize);
            GameObject obj3 = CreateUIObject("Background", parent);
            GameObject obj4 = CreateUIObject("Fill Area", parent);
            GameObject obj5 = CreateUIObject("Fill", obj4);
            GameObject obj6 = CreateUIObject("Handle Slide Area", parent);
            GameObject obj7 = CreateUIObject("Handle", obj6);
            Image image = obj3.AddComponent<Image>();
            image.sprite = resources.background;
            image.type = Image.Type.Sliced;
            image.color = s_DefaultSelectableColor;
            RectTransform component = obj3.GetComponent<RectTransform>();
            component.anchorMin = new Vector2(0f, 0.25f);
            component.anchorMax = new Vector2(1f, 0.75f);
            component.sizeDelta = new Vector2(0f, 0f);
            RectTransform transform2 = obj4.GetComponent<RectTransform>();
            transform2.anchorMin = new Vector2(0f, 0.25f);
            transform2.anchorMax = new Vector2(1f, 0.75f);
            transform2.anchoredPosition = new Vector2(-5f, 0f);
            transform2.sizeDelta = new Vector2(-20f, 0f);
            Image image2 = obj5.AddComponent<Image>();
            image2.sprite = resources.standard;
            image2.type = Image.Type.Sliced;
            image2.color = s_DefaultSelectableColor;
            obj5.GetComponent<RectTransform>().sizeDelta = new Vector2(10f, 0f);
            RectTransform transform4 = obj6.GetComponent<RectTransform>();
            transform4.sizeDelta = new Vector2(-20f, 0f);
            transform4.anchorMin = new Vector2(0f, 0f);
            transform4.anchorMax = new Vector2(1f, 1f);
            Image image3 = obj7.AddComponent<Image>();
            image3.sprite = resources.knob;
            image3.color = s_DefaultSelectableColor;
            obj7.GetComponent<RectTransform>().sizeDelta = new Vector2(20f, 0f);
            Slider slider = parent.AddComponent<Slider>();
            slider.fillRect = obj5.GetComponent<RectTransform>();
            slider.handleRect = obj7.GetComponent<RectTransform>();
            slider.targetGraphic = image3;
            slider.direction = Slider.Direction.LeftToRight;
            SetDefaultColorTransitionValues(slider);
            return parent;
        }

        public static GameObject CreateText(Resources resources)
        {
            GameObject obj2 = CreateUIElementRoot("Text", s_ThickElementSize);
            Text lbl = obj2.AddComponent<Text>();
            lbl.text = "New Text";
            SetDefaultTextValues(lbl);
            return obj2;
        }

        public static GameObject CreateToggle(Resources resources)
        {
            GameObject parent = CreateUIElementRoot("Toggle", s_ThinElementSize);
            GameObject obj3 = CreateUIObject("Background", parent);
            GameObject obj4 = CreateUIObject("Checkmark", obj3);
            GameObject obj5 = CreateUIObject("Label", parent);
            Toggle slider = parent.AddComponent<Toggle>();
            slider.isOn = true;
            Image image = obj3.AddComponent<Image>();
            image.sprite = resources.standard;
            image.type = Image.Type.Sliced;
            image.color = s_DefaultSelectableColor;
            Image image2 = obj4.AddComponent<Image>();
            image2.sprite = resources.checkmark;
            Text lbl = obj5.AddComponent<Text>();
            lbl.text = "Toggle";
            SetDefaultTextValues(lbl);
            slider.graphic = image2;
            slider.targetGraphic = image;
            SetDefaultColorTransitionValues(slider);
            RectTransform component = obj3.GetComponent<RectTransform>();
            component.anchorMin = new Vector2(0f, 1f);
            component.anchorMax = new Vector2(0f, 1f);
            component.anchoredPosition = new Vector2(10f, -10f);
            component.sizeDelta = new Vector2(20f, 20f);
            RectTransform transform2 = obj4.GetComponent<RectTransform>();
            transform2.anchorMin = new Vector2(0.5f, 0.5f);
            transform2.anchorMax = new Vector2(0.5f, 0.5f);
            transform2.anchoredPosition = Vector2.zero;
            transform2.sizeDelta = new Vector2(20f, 20f);
            RectTransform transform3 = obj5.GetComponent<RectTransform>();
            transform3.anchorMin = new Vector2(0f, 0f);
            transform3.anchorMax = new Vector2(1f, 1f);
            transform3.offsetMin = new Vector2(23f, 1f);
            transform3.offsetMax = new Vector2(-5f, -2f);
            return parent;
        }

        private static GameObject CreateUIElementRoot(string name, Vector2 size)
        {
            GameObject obj2 = new GameObject(name);
            obj2.AddComponent<RectTransform>().sizeDelta = size;
            return obj2;
        }

        private static GameObject CreateUIObject(string name, GameObject parent)
        {
            GameObject child = new GameObject(name);
            child.AddComponent<RectTransform>();
            SetParentAndAlign(child, parent);
            return child;
        }

        private static void SetDefaultColorTransitionValues(Selectable slider)
        {
            ColorBlock colors = slider.colors;
            colors.highlightedColor = new Color(0.882f, 0.882f, 0.882f);
            colors.pressedColor = new Color(0.698f, 0.698f, 0.698f);
            colors.disabledColor = new Color(0.521f, 0.521f, 0.521f);
        }

        private static void SetDefaultTextValues(Text lbl)
        {
            lbl.color = s_TextColor;
        }

        private static void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            Transform transform = go.transform;
            for (int i = 0; i < transform.childCount; i++)
            {
                SetLayerRecursively(transform.GetChild(i).gameObject, layer);
            }
        }

        private static void SetParentAndAlign(GameObject child, GameObject parent)
        {
            if (parent != null)
            {
                child.transform.SetParent(parent.transform, false);
                SetLayerRecursively(child, parent.layer);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Resources
        {
            public Sprite standard;
            public Sprite background;
            public Sprite inputField;
            public Sprite knob;
            public Sprite checkmark;
            public Sprite dropdown;
            public Sprite mask;
        }
    }
}

