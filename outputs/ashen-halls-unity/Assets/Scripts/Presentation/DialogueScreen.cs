using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AshenHalls
{
    public sealed class DialogueChoiceView
    {
        public string Id;
        public string Label;
        public string Hint;
        public bool Enabled = true;
        public bool Primary;
    }

    public sealed class DialogueScreenView
    {
        public string Title;
        public string Speaker;
        public string Focus;
        public string Body;
        public string AccentHex;
        public string PageLabel;
        public string ContinueLabel;
        public Texture2D PortraitTexture;
        public Rect PortraitSource;
        public DialogueChoiceView[] Choices = Array.Empty<DialogueChoiceView>();
    }

    public sealed class DialogueScreenBindings
    {
        public Func<DialogueScreenView> View;
        public Action Advance;
        public Action<string> Choose;
    }

    public static class DialoguePresentationRules
    {
        private static readonly Color DialoguePanel = new Color32(0x1a, 0x20, 0x26, 0xff);
        private static readonly Color WarmInk = new Color32(0xf3, 0xea, 0xd7, 0xff);

        public static Color ReadableAccent(Color accent)
        {
            if (accent.a <= 0f) accent = new Color(0.84f, 0.66f, 0.31f, 1f);
            Color.RGBToHSV(accent, out float hue, out float saturation, out float value);
            saturation = Mathf.Max(0.12f, saturation);
            value = Mathf.Max(0.58f, value);
            Color readable = Color.HSVToRGB(hue, saturation, value);
            readable.a = accent.a;
            return readable;
        }

        public static Color ReadableTextAccent(Color accent)
        {
            Color readable = ReadableAccent(accent);
            float alpha = readable.a;
            readable.a = 1f;
            for (int i = 0; i < 8 && ContrastRatio(readable, DialoguePanel) < 4.5f; i++)
            {
                readable = Color.Lerp(readable, WarmInk, 0.18f);
            }
            readable.a = alpha;
            return readable;
        }

        public static float ContrastRatio(Color first, Color second)
        {
            float bright = Mathf.Max(RelativeLuminance(first), RelativeLuminance(second));
            float dark = Mathf.Min(RelativeLuminance(first), RelativeLuminance(second));
            return (bright + 0.05f) / (dark + 0.05f);
        }

        private static float RelativeLuminance(Color color)
        {
            return 0.2126f * LinearChannel(color.r)
                + 0.7152f * LinearChannel(color.g)
                + 0.0722f * LinearChannel(color.b);
        }

        private static float LinearChannel(float channel)
        {
            channel = Mathf.Clamp01(channel);
            return channel <= 0.04045f
                ? channel / 12.92f
                : Mathf.Pow((channel + 0.055f) / 1.055f, 2.4f);
        }
    }

    public readonly struct DialogueScreenGeometry
    {
        public readonly Rect Backdrop;
        public readonly Rect Panel;
        public readonly Rect Portrait;
        public readonly Rect Body;
        public readonly Rect CloseButton;

        public DialogueScreenGeometry(Rect backdrop, Rect panel, Rect portrait, Rect body, Rect closeButton)
        {
            Backdrop = backdrop;
            Panel = panel;
            Portrait = portrait;
            Body = body;
            CloseButton = closeButton;
        }

        public bool Fits(float width, float height)
        {
            return FitsRect(Backdrop, width, height)
                && FitsRect(Panel, width, height)
                && FitsLocal(Portrait, Panel)
                && FitsLocal(Body, Panel)
                && FitsLocal(CloseButton, Panel);
        }

        private static bool FitsRect(Rect rect, float width, float height)
        {
            return rect.xMin >= 0f && rect.yMin >= 0f && rect.xMax <= width && rect.yMax <= height;
        }

        private static bool FitsLocal(Rect rect, Rect parent)
        {
            return rect.xMin >= 0f && rect.yMin >= 0f && rect.xMax <= parent.width && rect.yMax <= parent.height;
        }
    }

    public static class DialogueScreenLayout
    {
        public static DialogueScreenGeometry Calculate(float width, float height)
        {
            return Calculate(width, height, 0);
        }

        public static DialogueScreenGeometry Calculate(float width, float height, int choiceCount)
        {
            int visibleChoices = Mathf.Clamp(choiceCount, 0, 4);
            float panelW = Mathf.Min(Mathf.Max(760f, width * 0.60f), Mathf.Min(1160f, width - 48f));
            float desiredHeight = visibleChoices >= 4
                ? 548f
                : visibleChoices >= 3
                    ? 500f
                    : visibleChoices > 0 ? 458f : 420f;
            float panelH = Mathf.Min(desiredHeight, height - 64f);
            Rect backdrop = new Rect(0f, 0f, width, height);
            Rect panel = new Rect((width - panelW) * 0.5f, (height - panelH) * 0.5f, panelW, panelH);
            float portraitSize = Mathf.Clamp(panelH * 0.25f, 112f, 128f);
            Rect portrait = new Rect(24f, 64f, portraitSize, portraitSize);
            float textX = portrait.xMax + 18f;
            float bodyHeight = visibleChoices > 0
                ? Mathf.Clamp(panelH * 0.27f, 112f, 132f)
                : panelH - 174f;
            Rect body = new Rect(textX, 96f, panelW - textX - 24f, bodyHeight);
            Rect closeButton = new Rect(panelW - 174f, panelH - 54f, 148f, 38f);
            return new DialogueScreenGeometry(backdrop, panel, portrait, body, closeButton);
        }
    }

    public sealed class DialogueScreen : MonoBehaviour
    {
        private DialogueScreenBindings bindings;
        private Canvas canvas;
        private CanvasGroup canvasGroup;
        private RectTransform backdrop;
        private RectTransform panel;
        private RectTransform portrait;
        private RectTransform bodyPanel;
        private ScrollRect bodyScroll;
        private RectTransform choicesPanel;
        private readonly Button[] choiceButtons = new Button[4];
        private RectTransform accentStrip;
        private Button closeButton;
        private Text titleText;
        private Text speakerText;
        private Text bodyText;
        private Text hintText;
        private Text portraitText;
        private Image portraitArt;
        private Text pageText;
        private Font font;
        private Font emphasisFont;
        private float lastWidth = -1f;
        private float lastHeight = -1f;
        private float bodyViewportWidth;
        private float bodyViewportHeight;
        private int lastChoiceCount = -1;
        private int selectedChoiceIndex;
        private bool lastRefreshSucceeded;

        public bool IsReady => canvas != null
            && panel != null
            && bodyPanel != null
            && bodyScroll != null
            && bodyText != null
            && closeButton != null;
        public bool IsVisible => IsReady && UiRuntime.IsCanvasVisible(canvas);
        public bool CanOwnModal => IsReady
            && lastRefreshSucceeded
            && UiRuntime.CanOwnModal(canvas, panel, canvasGroup, closeButton);
        public bool HasRenderableGeometry => CanOwnModal;
        public bool HasScrollableBody => IsReady && bodyScroll.viewport == bodyPanel && bodyScroll.content == bodyText.rectTransform;
        public bool IsInteractiveAndVisible => CanOwnModal;
        public bool HasPortraitArt => IsReady && portraitArt != null && portraitArt.enabled && portraitArt.sprite != null;

        public void Bind(DialogueScreenBindings screenBindings)
        {
            bindings = screenBindings;
            Build();
            SetVisible(false);
            Refresh();
        }

        public bool SetVisible(bool visible)
        {
            if (visible) UiRuntime.EnsureEventSystemReady();
            bool changed = UiRuntime.SetCanvasVisible(canvas, visible);
            if (canvasGroup != null)
            {
                canvasGroup.alpha = visible ? 1f : 0f;
                canvasGroup.interactable = visible;
                canvasGroup.blocksRaycasts = visible;
            }
            if (changed && visible)
            {
                lastWidth = -1f;
                lastHeight = -1f;
                EnsureChoiceSelection();
            }
            return changed;
        }

        public void SetSuppressedByImguiFallback(bool suppressed)
        {
            if (canvasGroup == null) return;
            canvasGroup.alpha = suppressed ? 0f : 1f;
            canvasGroup.interactable = !suppressed;
            canvasGroup.blocksRaycasts = !suppressed;
        }

        public void Refresh()
        {
            lastRefreshSucceeded = false;
            if (bindings == null || canvas == null) return;
            DialogueScreenView view = bindings.View == null ? null : bindings.View();
            if (view == null) return;
            DialogueChoiceView[] choices = view.Choices ?? Array.Empty<DialogueChoiceView>();
            int choiceCount = Mathf.Min(choiceButtons.Length, choices.Length);
            bool choiceCountChanged = lastChoiceCount != choiceCount;
            if (choiceCountChanged) selectedChoiceIndex = 0;
            if (!Mathf.Approximately(lastWidth, Screen.width)
                || !Mathf.Approximately(lastHeight, Screen.height)
                || choiceCountChanged)
            {
                ApplyLayout(choiceCount);
            }

            Color accent = DialoguePresentationRules.ReadableAccent(ParseColor(view.AccentHex, Hex("d7a84e", 1f)));
            Color titleAccent = DialoguePresentationRules.ReadableTextAccent(accent);
            Image panelImage = panel.GetComponent<Image>();
            if (panelImage != null) panelImage.color = Hex("1a2026", 0.98f);
            Outline outline = panel.GetComponent<Outline>();
            if (outline != null) outline.effectColor = accent;
            accentStrip.GetComponent<Image>().color = accent;
            portrait.GetComponent<Outline>().effectColor = WithAlpha(accent, 0.84f);
            Outline bodyOutline = bodyPanel.GetComponent<Outline>();
            if (bodyOutline != null) bodyOutline.effectColor = WithAlpha(accent, 0.28f);
            titleText.color = titleAccent;
            titleText.text = string.IsNullOrWhiteSpace(view.Title) ? "Midgaard" : view.Title;
            speakerText.text = string.IsNullOrWhiteSpace(view.Speaker) ? "Traveler" : view.Speaker;
            bodyText.text = string.IsNullOrWhiteSpace(view.Body) ? "..." : view.Body;
            Canvas.ForceUpdateCanvases();
            UpdateBodyContentLayout();
            bodyScroll.verticalNormalizedPosition = 1f;
            Sprite portraitSprite = UiRuntime.AtlasSprite(view.PortraitTexture, view.PortraitSource);
            portraitArt.sprite = portraitSprite;
            portraitArt.enabled = portraitSprite != null;
            portraitText.gameObject.SetActive(portraitSprite == null);
            portraitText.text = PortraitInitials(view.Speaker, view.Focus);
            pageText.text = view.PageLabel ?? "";
            SetButtonLabel(closeButton, string.IsNullOrWhiteSpace(view.ContinueLabel) ? "Continue" : view.ContinueLabel);
            choicesPanel.gameObject.SetActive(choiceCount > 0);
            for (int i = 0; i < choiceButtons.Length; i++)
            {
                bool visible = i < choiceCount && choices[i] != null;
                choiceButtons[i].gameObject.SetActive(visible);
                if (!visible) continue;
                choiceButtons[i].interactable = choices[i].Enabled;
                SetButtonLabel(choiceButtons[i], $"{i + 1}.  {choices[i].Label}");
                Text choiceLabel = choiceButtons[i].GetComponentInChildren<Text>();
                if (choiceLabel != null)
                {
                    choiceLabel.color = choices[i].Enabled
                        ? choices[i].Primary ? titleAccent : Hex("f3ead7", 1f)
                        : Hex("81796c", 1f);
                    choiceLabel.fontStyle = choices[i].Primary ? FontStyle.Bold : FontStyle.Normal;
                }
                Outline choiceOutline = choiceButtons[i].GetComponent<Outline>();
                if (choiceOutline != null)
                {
                    choiceOutline.effectColor = choices[i].Enabled && choices[i].Primary
                        ? WithAlpha(accent, 0.92f)
                        : Hex("56636a", 0.62f);
                }
            }
            EnsureChoiceSelection();
            UpdateChoiceHint(view);
            Canvas.ForceUpdateCanvases();
            lastRefreshSucceeded = true;
        }

        private void Build()
        {
            EnsureEventSystem();
            font = UiRuntime.DialogueFont;
            emphasisFont = UiRuntime.DialogueEmphasisFont;
            canvas = UiRuntime.CreateOwnedRootCanvas(this, "Dialogue Canvas");
            UiRuntime.ConfigureOverlayCanvas(canvas, 200);
            UiRuntime.SetCanvasVisible(canvas, false);
            canvasGroup = canvas.gameObject.AddComponent<CanvasGroup>();
            Stretch(canvas.GetComponent<RectTransform>());

            backdrop = AddImage("Backdrop", canvas.transform, Hex("020303", 0.52f)).rectTransform;
            panel = AddPanel("Dialogue Panel", canvas.transform, Hex("1a2026", 0.98f), Hex("d7a84e", 0.92f));
            accentStrip = AddImage("Accent Strip", panel, Hex("d7a84e", 1f)).rectTransform;
            titleText = AddText("Title", panel, "", 14, Hex("d7a84e", 1f), TextAnchor.MiddleLeft, true);
            speakerText = AddText("Speaker", panel, "", 24, Hex("f3ead7", 1f), TextAnchor.MiddleLeft, true);
            portrait = AddPanel("Portrait", panel, Hex("050708", 0.86f), Hex("d7a84e", 0.84f));
            portraitArt = AddImage("Portrait Art", portrait, Color.white);
            portraitArt.preserveAspect = true;
            portraitArt.raycastTarget = false;
            portraitText = AddText("Portrait Text", portrait, "", 22, Hex("f3ead7", 1f), TextAnchor.MiddleCenter, true);
            bodyPanel = AddImage("Body", panel, Hex("0b1013", 0.58f)).rectTransform;
            bodyPanel.gameObject.AddComponent<RectMask2D>();
            bodyScroll = bodyPanel.gameObject.AddComponent<ScrollRect>();
            bodyScroll.horizontal = false;
            bodyScroll.vertical = true;
            bodyScroll.inertia = true;
            bodyScroll.movementType = ScrollRect.MovementType.Clamped;
            bodyScroll.scrollSensitivity = 32f;
            bodyText = AddText("Body Text", bodyPanel, "", 18, Hex("f3ead7", 1f), TextAnchor.UpperLeft);
            bodyText.lineSpacing = 1.10f;
            bodyText.verticalOverflow = VerticalWrapMode.Overflow;
            bodyScroll.viewport = bodyPanel;
            bodyScroll.content = bodyText.rectTransform;
            choicesPanel = AddImage("Choices", panel, Hex("080b0d", 0.30f)).rectTransform;
            for (int i = 0; i < choiceButtons.Length; i++)
            {
                int captured = i;
                choiceButtons[i] = AddButton("Choice " + (i + 1), choicesPanel, "Choice", () => InvokeChoice(captured));
                AddChoiceSelectionEvents(choiceButtons[i], captured);
                Text choiceText = choiceButtons[i].GetComponentInChildren<Text>();
                if (choiceText != null)
                {
                    choiceText.alignment = TextAnchor.MiddleLeft;
                    choiceText.fontStyle = FontStyle.Normal;
                }
            }
            choicesPanel.gameObject.SetActive(false);
            hintText = AddText("Hint", panel, "", 13, Hex("b7aa90", 1f), TextAnchor.MiddleLeft);
            pageText = AddText("Page", panel, "", 12, Hex("b7aa90", 1f), TextAnchor.MiddleRight);
            closeButton = AddButton("Continue", panel, "Continue", () => bindings?.Advance?.Invoke());
        }

        private void ApplyLayout(int choiceCount)
        {
            lastWidth = Screen.width;
            lastHeight = Screen.height;
            lastChoiceCount = choiceCount;
            DialogueScreenGeometry geometry = DialogueScreenLayout.Calculate(Screen.width, Screen.height, choiceCount);
            SetScreenRect(backdrop, geometry.Backdrop);
            SetScreenRect(panel, geometry.Panel);
            SetLocalRect(accentStrip, new Rect(0f, 0f, 4f, geometry.Panel.height));
            SetLocalRect(titleText.rectTransform, new Rect(24f, 13f, geometry.Panel.width - 48f, 20f));
            SetLocalRect(portrait, geometry.Portrait);
            SetLocalRect(portraitArt.rectTransform, new Rect(5f, 5f, geometry.Portrait.width - 10f, geometry.Portrait.height - 10f));
            SetLocalRect(portraitText.rectTransform, new Rect(0f, 0f, geometry.Portrait.width, geometry.Portrait.height));
            float textX = geometry.Portrait.xMax + 18f;
            SetLocalRect(speakerText.rectTransform, new Rect(textX, 49f, geometry.Panel.width - textX - 24f, 38f));
            Rect bodyRect = geometry.Body;
            if (choiceCount > 0)
            {
                const float rowGap = 8f;
                float choicesY = Mathf.Max(geometry.Portrait.yMax + 14f, bodyRect.yMax + 14f);
                float choicesHeight = Mathf.Max(72f, geometry.Panel.height - 68f - choicesY);
                SetLocalRect(choicesPanel, new Rect(24f, choicesY, geometry.Panel.width - 48f, choicesHeight));
                float choiceHeight = Mathf.Clamp(
                    (choicesHeight - 16f - Mathf.Max(0, choiceCount - 1) * rowGap) / Mathf.Max(1, choiceCount),
                    44f,
                    52f);
                for (int i = 0; i < choiceButtons.Length; i++)
                {
                    SetLocalRect(
                        choiceButtons[i].GetComponent<RectTransform>(),
                        new Rect(8f, 8f + i * (choiceHeight + rowGap), geometry.Panel.width - 64f, choiceHeight));
                }
            }
            else
            {
                SetLocalRect(choicesPanel, new Rect(bodyRect.x, bodyRect.yMax, bodyRect.width, 0f));
            }
            SetLocalRect(bodyPanel, bodyRect);
            bodyViewportWidth = bodyRect.width;
            bodyViewportHeight = bodyRect.height;
            UpdateBodyContentLayout();
            SetLocalRect(closeButton.GetComponent<RectTransform>(), geometry.CloseButton);
            SetLocalRect(hintText.rectTransform, new Rect(24f, geometry.Panel.height - 43f, geometry.CloseButton.x - 96f, 20f));
            SetLocalRect(pageText.rectTransform, new Rect(geometry.CloseButton.x - 70f, geometry.Panel.height - 42f, 58f, 18f));
        }

        private void UpdateBodyContentLayout()
        {
            if (bodyText == null || bodyViewportWidth <= 0f || bodyViewportHeight <= 0f) return;
            RectTransform content = bodyText.rectTransform;
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(0f, 1f);
            content.pivot = new Vector2(0f, 1f);
            content.anchoredPosition = new Vector2(12f, -10f);
            float width = Mathf.Max(40f, bodyViewportWidth - 24f);
            bodyText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            float height = Mathf.Max(bodyViewportHeight - 20f, bodyText.preferredHeight + 4f);
            bodyText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }

        public void InvokeContinueForTest()
        {
            if (!IsReady || !closeButton.interactable) throw new InvalidOperationException("Dialogue Continue button is not ready.");
            closeButton.onClick.Invoke();
        }

        public int VisibleChoiceCountForTest
        {
            get
            {
                int count = 0;
                for (int i = 0; i < choiceButtons.Length; i++)
                {
                    if (choiceButtons[i] != null && choiceButtons[i].gameObject.activeSelf) count++;
                }
                return count;
            }
        }

        public void InvokeChoiceForTest(int index)
        {
            if (index < 0 || index >= choiceButtons.Length || !choiceButtons[index].gameObject.activeSelf)
                throw new InvalidOperationException("Dialogue choice is not visible.");
            if (!choiceButtons[index].interactable)
                throw new InvalidOperationException("Dialogue choice is disabled.");
            choiceButtons[index].onClick.Invoke();
        }

        public int SelectedChoiceIndexForTest => selectedChoiceIndex;
        public FontStyle ChoiceFontStyleForTest(int index)
        {
            if (index < 0 || index >= choiceButtons.Length || choiceButtons[index] == null) return FontStyle.Normal;
            Text label = choiceButtons[index].GetComponentInChildren<Text>();
            return label == null ? FontStyle.Normal : label.fontStyle;
        }

        public float ChoiceOutlineAlphaForTest(int index)
        {
            if (index < 0 || index >= choiceButtons.Length || choiceButtons[index] == null) return 0f;
            Outline outline = choiceButtons[index].GetComponent<Outline>();
            return outline == null ? 0f : outline.effectColor.a;
        }

        public string BodyFontNameForTest => bodyText != null && bodyText.font != null ? bodyText.font.name : "";
        public int BodyFontSizeForTest => bodyText == null ? 0 : bodyText.fontSize;
        public FontStyle BodyFontStyleForTest => bodyText == null ? FontStyle.Normal : bodyText.fontStyle;
        public string SpeakerFontNameForTest => speakerText != null && speakerText.font != null ? speakerText.font.name : "";
        public float SpeakerHeightForTest => speakerText == null ? 0f : speakerText.rectTransform.rect.height;
        public float SpeakerPreferredHeightForTest => speakerText == null ? 0f : speakerText.preferredHeight;

        public void MoveChoiceSelection(int delta)
        {
            int count = VisibleChoiceCountForTest;
            if (count <= 0 || delta == 0) return;
            int direction = delta < 0 ? -1 : 1;
            int candidate = Mathf.Clamp(selectedChoiceIndex, 0, count - 1);
            for (int attempts = 0; attempts < count; attempts++)
            {
                candidate = (candidate + direction + count) % count;
                if (!choiceButtons[candidate].interactable) continue;
                selectedChoiceIndex = candidate;
                SelectChoiceButton();
                return;
            }
        }

        public void InvokeSelectedChoice()
        {
            int count = VisibleChoiceCountForTest;
            if (count <= 0) return;
            EnsureChoiceSelection();
            if (selectedChoiceIndex < 0
                || selectedChoiceIndex >= count
                || !choiceButtons[selectedChoiceIndex].interactable) return;
            InvokeChoice(selectedChoiceIndex);
        }

        private void InvokeChoice(int index)
        {
            DialogueScreenView view = bindings?.View == null ? null : bindings.View();
            DialogueChoiceView[] choices = view?.Choices ?? Array.Empty<DialogueChoiceView>();
            if (index < 0 || index >= choices.Length || choices[index] == null || !choices[index].Enabled) return;
            bindings?.Choose?.Invoke(choices[index].Id);
        }

        private void EnsureChoiceSelection()
        {
            int count = VisibleChoiceCountForTest;
            if (count <= 0) return;
            selectedChoiceIndex = Mathf.Clamp(selectedChoiceIndex, 0, count - 1);
            if (!choiceButtons[selectedChoiceIndex].interactable)
            {
                int enabled = -1;
                for (int i = 0; i < count; i++)
                {
                    if (!choiceButtons[i].interactable) continue;
                    enabled = i;
                    break;
                }
                if (enabled < 0) return;
                selectedChoiceIndex = enabled;
            }
            SelectChoiceButton();
        }

        private void SelectChoiceButton()
        {
            if (!IsVisible || EventSystem.current == null) return;
            int count = VisibleChoiceCountForTest;
            if (selectedChoiceIndex < 0 || selectedChoiceIndex >= count) return;
            EventSystem.current.SetSelectedGameObject(choiceButtons[selectedChoiceIndex].gameObject);
            UpdateChoiceHint(bindings?.View == null ? null : bindings.View());
        }

        private void AddChoiceSelectionEvents(Button button, int index)
        {
            if (button == null) return;
            EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();
            AddEventTrigger(trigger, EventTriggerType.PointerEnter, _ => PreviewChoice(index));
            AddEventTrigger(trigger, EventTriggerType.Select, _ => PreviewChoice(index));
        }

        private static void AddEventTrigger(EventTrigger trigger, EventTriggerType eventType, Action<BaseEventData> callback)
        {
            if (trigger == null || callback == null) return;
            EventTrigger.Entry entry = new EventTrigger.Entry { eventID = eventType };
            entry.callback.AddListener(data => callback(data));
            trigger.triggers.Add(entry);
        }

        private void PreviewChoice(int index)
        {
            if (index < 0 || index >= choiceButtons.Length) return;
            Button button = choiceButtons[index];
            if (button == null || !button.gameObject.activeSelf) return;
            selectedChoiceIndex = index;
            UpdateChoiceHint(bindings?.View == null ? null : bindings.View());
        }

        private void UpdateChoiceHint(DialogueScreenView view)
        {
            if (hintText == null) return;
            DialogueChoiceView[] choices = view?.Choices ?? Array.Empty<DialogueChoiceView>();
            if (choices.Length == 0)
            {
                hintText.text = "";
                return;
            }

            int index = Mathf.Clamp(selectedChoiceIndex, 0, Mathf.Min(choiceButtons.Length, choices.Length) - 1);
            DialogueChoiceView selected = index >= 0 && index < choices.Length ? choices[index] : null;
            hintText.text = selected == null ? "" : selected.Hint ?? "";
        }

        private Button AddButton(string name, Transform parent, string label, Action action)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(Outline));
            go.transform.SetParent(parent, false);
            Image image = go.GetComponent<Image>();
            image.color = Hex("10161a", 0.96f);
            Button button = go.GetComponent<Button>();
            button.targetGraphic = image;
            ColorBlock colors = button.colors;
            colors.normalColor = image.color;
            colors.highlightedColor = Hex("25323a", 1f);
            colors.pressedColor = Hex("0b1013", 1f);
            colors.disabledColor = Hex("0b0f12", 0.70f);
            colors.selectedColor = colors.highlightedColor;
            button.colors = colors;
            Outline outline = go.GetComponent<Outline>();
            outline.effectColor = Hex("56636a", 0.62f);
            outline.effectDistance = new Vector2(1f, -1f);
            if (action != null) button.onClick.AddListener(() => action());

            Text text = AddText("Label", go.transform, label, 16, Hex("f3ead7", 1f), TextAnchor.MiddleLeft, true);
            text.fontStyle = FontStyle.Normal;
            Stretch(text.rectTransform, 16f, 4f);
            return button;
        }

        private static void SetButtonLabel(Button button, string label)
        {
            if (button == null) return;
            Text text = button.GetComponentInChildren<Text>();
            if (text != null) text.text = label ?? "";
        }

        private RectTransform AddPanel(string name, Transform parent, Color fill, Color border)
        {
            RectTransform root = AddImage(name, parent, fill).rectTransform;
            Outline outline = root.gameObject.AddComponent<Outline>();
            outline.effectColor = border;
            outline.effectDistance = new Vector2(1f, -1f);
            return root;
        }

        private Image AddImage(string name, Transform parent, Color color)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(parent, false);
            Image image = go.GetComponent<Image>();
            image.color = color;
            return image;
        }

        private Text AddText(
            string name,
            Transform parent,
            string value,
            int size,
            Color color,
            TextAnchor anchor,
            bool emphasized = false)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            go.transform.SetParent(parent, false);
            Text text = go.GetComponent<Text>();
            text.font = emphasized && emphasisFont != null ? emphasisFont : font;
            text.text = value;
            text.fontSize = size;
            text.color = color;
            text.alignment = anchor;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.fontStyle = FontStyle.Normal;
            return text;
        }

        private static string PortraitInitials(string speaker, string focus)
        {
            string source = string.IsNullOrWhiteSpace(speaker) ? focus : speaker;
            if (string.IsNullOrWhiteSpace(source)) return "AH";
            string[] parts = source.Split(new[] { ' ', '-', '_' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return "AH";
            string first = parts[0].Substring(0, 1).ToUpperInvariant();
            string second = parts.Length > 1 ? parts[1].Substring(0, 1).ToUpperInvariant() : "";
            return first + second;
        }

        private static void SetScreenRect(RectTransform rect, Rect area)
        {
            if (rect == null) return;
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(area.x, -area.y);
            rect.sizeDelta = new Vector2(area.width, area.height);
        }

        private static void SetLocalRect(RectTransform rect, Rect area)
        {
            if (rect == null) return;
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(area.x, -area.y);
            rect.sizeDelta = new Vector2(area.width, area.height);
        }

        private static void Stretch(RectTransform rect, float insetX = 0f, float insetY = 0f)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(insetX, insetY);
            rect.offsetMax = new Vector2(-insetX, -insetY);
        }

        private static Color Hex(string hex, float alpha)
        {
            if (hex.StartsWith("#")) hex = hex.Substring(1);
            byte r = Convert.ToByte(hex.Substring(0, 2), 16);
            byte g = Convert.ToByte(hex.Substring(2, 2), 16);
            byte b = Convert.ToByte(hex.Substring(4, 2), 16);
            return new Color32(r, g, b, (byte)Mathf.RoundToInt(Mathf.Clamp01(alpha) * 255f));
        }

        private static Color ParseColor(string hex, Color fallback)
        {
            if (string.IsNullOrWhiteSpace(hex)) return fallback;
            return ColorUtility.TryParseHtmlString(hex.StartsWith("#") ? hex : "#" + hex, out Color color) ? color : fallback;
        }

        private static Color WithAlpha(Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

        private static void EnsureEventSystem()
        {
            UiRuntime.EnsureEventSystemReady();
        }
    }
}
