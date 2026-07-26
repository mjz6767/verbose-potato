using System;
using UnityEngine;
using UnityEngine.UI;

namespace AshenHalls
{
    public sealed class LootPopupView
    {
        public bool Visible;
        public bool CanReview;
        public string Title;
        public string ItemName;
        public string ItemType;
        public string Rarity;
        public string TraitLine;
        public string EquipNote;
        public string Outcome;
        public string Comparison;
        public string IconLabel;
        public string AccentHex;
        public Texture2D IconTexture;
        public Rect IconUv;
        public int Gold;
        public int Supplies;
        public int Elixirs;
        public float SecondsRemaining;
    }

    public sealed class LootPopupBindings
    {
        public Func<LootPopupView> View;
        public Action Dismiss;
        public Action ReviewInventory;
    }

    public readonly struct LootPopupGeometry
    {
        public readonly Rect Backdrop;
        public readonly Rect Panel;
        public readonly Rect Icon;
        public readonly Rect Eyebrow;
        public readonly Rect ItemTitle;
        public readonly Rect ResourceRow;
        public readonly Rect Body;
        public readonly Rect Outcome;
        public readonly Rect ReviewButton;
        public readonly Rect DismissButton;

        public LootPopupGeometry(
            Rect backdrop,
            Rect panel,
            Rect icon,
            Rect eyebrow,
            Rect itemTitle,
            Rect resourceRow,
            Rect body,
            Rect outcome,
            Rect reviewButton,
            Rect dismissButton)
        {
            Backdrop = backdrop;
            Panel = panel;
            Icon = icon;
            Eyebrow = eyebrow;
            ItemTitle = itemTitle;
            ResourceRow = resourceRow;
            Body = body;
            Outcome = outcome;
            ReviewButton = reviewButton;
            DismissButton = dismissButton;
        }

        public bool Fits(float width, float height)
        {
            return FitsScreen(Backdrop, width, height)
                && FitsScreen(Panel, width, height)
                && FitsLocal(Icon, Panel)
                && FitsLocal(Eyebrow, Panel)
                && FitsLocal(ItemTitle, Panel)
                && FitsLocal(ResourceRow, Panel)
                && FitsLocal(Body, Panel)
                && FitsLocal(Outcome, Panel)
                && FitsLocal(ReviewButton, Panel)
                && FitsLocal(DismissButton, Panel);
        }

        private static bool FitsScreen(Rect rect, float width, float height)
        {
            return rect.xMin >= 0f && rect.yMin >= 0f && rect.xMax <= width && rect.yMax <= height;
        }

        private static bool FitsLocal(Rect rect, Rect parent)
        {
            return rect.xMin >= 0f && rect.yMin >= 0f && rect.xMax <= parent.width && rect.yMax <= parent.height;
        }
    }

    public static class LootPopupLayout
    {
        public static LootPopupGeometry Calculate(float width, float height)
        {
            float panelW = Mathf.Min(Mathf.Max(760f, width * 0.56f), Mathf.Min(980f, width - 48f));
            float panelH = Mathf.Clamp(height * 0.50f, 360f, 440f);
            Rect backdrop = new Rect(0f, 0f, width, height);
            Rect panel = new Rect((width - panelW) * 0.5f, (height - panelH) * 0.5f, panelW, panelH);
            Rect icon = new Rect(24f, 76f, 116f, 116f);
            Rect eyebrow = new Rect(158f, 47f, panelW - 182f, 18f);
            Rect itemTitle = new Rect(158f, 68f, panelW - 182f, 38f);
            Rect resourceRow = new Rect(158f, 112f, panelW - 182f, 30f);
            Rect body = new Rect(158f, 151f, panelW - 182f, 62f);
            Rect outcome = new Rect(24f, 226f, panelW - 48f, panelH - 290f);
            Rect review = new Rect(panelW - 286f, panelH - 48f, 142f, 32f);
            Rect dismiss = new Rect(panelW - 134f, panelH - 48f, 110f, 32f);
            return new LootPopupGeometry(backdrop, panel, icon, eyebrow, itemTitle, resourceRow, body, outcome, review, dismiss);
        }
    }

    public sealed class LootPopupScreen : MonoBehaviour
    {
        private LootPopupBindings bindings;
        private Canvas canvas;
        private RectTransform backdrop;
        private RectTransform panel;
        private RectTransform iconPanel;
        private RectTransform resourceRow;
        private RectTransform accentStrip;
        private RectTransform outcomePanel;
        private RawImage iconImage;
        private Text headerText;
        private Text itemTitleText;
        private Text eyebrowText;
        private Text iconFallbackText;
        private Text bodyText;
        private Text outcomeTitleText;
        private Text outcomeDetailText;
        private Text goldText;
        private Text suppliesText;
        private Text elixirText;
        private Text timerText;
        private Button reviewButton;
        private Button dismissButton;
        private Font font;
        private float lastWidth = -1f;
        private float lastHeight = -1f;
        private bool lastRefreshSucceeded;

        public bool IsReady => canvas != null && panel != null && dismissButton != null && iconPanel != null;
        public bool IsVisible => IsReady && UiRuntime.IsCanvasVisible(canvas);
        public bool HasRenderableGeometry => IsReady
            && lastRefreshSucceeded
            && UiRuntime.CanOwnModal(canvas, panel, null, dismissButton);
        public bool HasRealIconForTest => iconImage != null && iconImage.gameObject.activeSelf && iconImage.texture != null;
        public bool HasReviewActionForTest => reviewButton != null && reviewButton.gameObject.activeSelf && reviewButton.interactable;
        public string PrimaryActionLabelForTest => dismissButton == null ? "" : dismissButton.GetComponentInChildren<Text>()?.text ?? "";

        public void Bind(LootPopupBindings popupBindings)
        {
            bindings = popupBindings;
            Build();
            SetVisible(false);
            Refresh();
        }

        public void SetVisible(bool visible)
        {
            if (visible) UiRuntime.EnsureEventSystemReady();
            UiRuntime.SetCanvasVisible(canvas, visible);
        }

        public void Refresh()
        {
            lastRefreshSucceeded = false;
            if (bindings == null || canvas == null) return;
            LootPopupView view = bindings.View == null ? null : bindings.View();
            if (view == null || !view.Visible)
            {
                SetVisible(false);
                return;
            }

            if (!Mathf.Approximately(lastWidth, Screen.width) || !Mathf.Approximately(lastHeight, Screen.height)) ApplyLayout();

            Color accent = ParseColor(view.AccentHex, Hex("d7a84e", 1f));
            panel.GetComponent<Outline>().effectColor = accent.WithAlpha(0.92f);
            iconPanel.GetComponent<Outline>().effectColor = accent.WithAlpha(0.88f);
            outcomePanel.GetComponent<Outline>().effectColor = accent.WithAlpha(0.60f);
            accentStrip.GetComponent<Image>().color = accent;
            headerText.text = string.IsNullOrWhiteSpace(view.Title) ? "Loot recovered" : view.Title;
            eyebrowText.text = $"{SafeUpper(view.Rarity, "Common")} {SafeUpper(view.ItemType, "Item")}  •  ACQUIRED";
            eyebrowText.color = accent;
            itemTitleText.text = string.IsNullOrWhiteSpace(view.ItemName) ? "Recovered supplies" : view.ItemName;
            bodyText.text = BuildBody(view);
            outcomeTitleText.text = string.IsNullOrWhiteSpace(view.Outcome) ? "Stored in inventory" : view.Outcome;
            outcomeTitleText.color = accent;
            outcomeDetailText.text = view.EquipNote ?? "";

            RefreshResourceChip(goldText, view.Gold, view.Gold == 1 ? "+1 gold" : $"+{view.Gold} gold");
            RefreshResourceChip(suppliesText, view.Supplies, view.Supplies == 1 ? "+1 supply" : $"+{view.Supplies} supplies");
            RefreshResourceChip(elixirText, view.Elixirs, view.Elixirs == 1 ? "+1 elixir" : $"+{view.Elixirs} elixirs");
            LayoutResourceChips();

            bool hasIcon = view.IconTexture != null && view.IconUv.width > 0f && view.IconUv.height > 0f;
            iconImage.gameObject.SetActive(hasIcon);
            iconFallbackText.gameObject.SetActive(!hasIcon);
            if (hasIcon)
            {
                iconImage.texture = view.IconTexture;
                iconImage.uvRect = view.IconUv;
                iconImage.color = Color.white;
            }
            iconFallbackText.text = string.IsNullOrWhiteSpace(view.IconLabel) ? "LOOT" : view.IconLabel;

            reviewButton.gameObject.SetActive(view.CanReview);
            reviewButton.interactable = view.CanReview;
            timerText.text = view.SecondsRemaining > 0.5f ? $"{Mathf.CeilToInt(view.SecondsRemaining)}s" : "";
            Canvas.ForceUpdateCanvases();
            lastRefreshSucceeded = true;
        }

        public void InvokeReviewForTest()
        {
            if (reviewButton != null && reviewButton.gameObject.activeSelf && reviewButton.interactable) bindings?.ReviewInventory?.Invoke();
        }

        private static string BuildBody(LootPopupView view)
        {
            string trait = string.IsNullOrWhiteSpace(view.TraitLine) ? "Serviceable adventuring gear." : view.TraitLine;
            string comparison = string.IsNullOrWhiteSpace(view.Comparison) ? "" : "\n" + view.Comparison;
            return trait + comparison;
        }

        private static string SafeUpper(string value, string fallback)
        {
            return (string.IsNullOrWhiteSpace(value) ? fallback : value.Trim()).ToUpperInvariant();
        }

        private void Build()
        {
            if (canvas != null) return;
            UiRuntime.EnsureEventSystemReady();
            font = UiRuntime.DefaultFont;
            canvas = UiRuntime.CreateOwnedRootCanvas(this, "Loot Popup Canvas");
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 28;
            CanvasScaler scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            canvas.gameObject.AddComponent<GraphicRaycaster>();
            Stretch(canvas.GetComponent<RectTransform>());

            backdrop = AddImage("Backdrop", canvas.transform, Hex("020303", 0.66f)).rectTransform;
            panel = AddPanel("Loot Popup", canvas.transform, Hex("11171b", 0.995f), Hex("d7a84e", 0.92f));
            accentStrip = AddImage("Accent Strip", panel, Hex("d7a84e", 1f)).rectTransform;
            headerText = AddText("Header", panel, "", 13, Hex("d7a84e", 1f), TextAnchor.MiddleLeft);
            iconPanel = AddPanel("Item Art", panel, Hex("050708", 0.92f), Hex("d7a84e", 0.78f));
            iconImage = AddRawImage("Item Icon", iconPanel);
            iconFallbackText = AddText("Item Icon Fallback", iconPanel, "LOOT", 13, Hex("f3ead7", 1f), TextAnchor.MiddleCenter);
            eyebrowText = AddText("Eyebrow", panel, "", 10, Hex("d7a84e", 1f), TextAnchor.MiddleLeft);
            itemTitleText = AddText("Item Name", panel, "", 18, Hex("f3ead7", 1f), TextAnchor.UpperLeft);
            itemTitleText.fontStyle = FontStyle.Bold;

            resourceRow = new GameObject("Resource Row", typeof(RectTransform)).GetComponent<RectTransform>();
            resourceRow.SetParent(panel, false);
            goldText = AddChip("Gold", resourceRow, Hex("d7a84e", 0.72f));
            suppliesText = AddChip("Supplies", resourceRow, Hex("7f9d5b", 0.72f));
            elixirText = AddChip("Elixirs", resourceRow, Hex("58b7a5", 0.72f));
            bodyText = AddText("Item Details", panel, "", 11, Hex("e1dacb", 1f), TextAnchor.UpperLeft);

            outcomePanel = AddPanel("Loot Outcome", panel, Hex("080b0d", 0.86f), Hex("d7a84e", 0.58f));
            outcomeTitleText = AddText("Outcome", outcomePanel, "", 12, Hex("d7a84e", 1f), TextAnchor.MiddleLeft);
            outcomeTitleText.fontStyle = FontStyle.Bold;
            outcomeDetailText = AddText("Outcome Detail", outcomePanel, "", 10, Hex("b7aa90", 1f), TextAnchor.UpperLeft);
            timerText = AddText("Timer", panel, "", 10, Hex("b7aa90", 1f), TextAnchor.MiddleRight);
            reviewButton = AddButton("Review Equipment", panel, "Review equipment", () => bindings?.ReviewInventory?.Invoke(), Hex("58b7a5", 0.86f));
            dismissButton = AddButton("Continue", panel, "Continue", () => bindings?.Dismiss?.Invoke(), Hex("d7a84e", 0.86f));
        }

        private void ApplyLayout()
        {
            lastWidth = Screen.width;
            lastHeight = Screen.height;
            LootPopupGeometry geometry = LootPopupLayout.Calculate(Screen.width, Screen.height);
            SetScreenRect(backdrop, geometry.Backdrop);
            SetScreenRect(panel, geometry.Panel);
            SetLocalRect(accentStrip, new Rect(0f, 0f, 5f, geometry.Panel.height));
            SetLocalRect(headerText.rectTransform, new Rect(24f, 14f, geometry.Panel.width - 48f, 24f));
            SetLocalRect(iconPanel, geometry.Icon);
            Stretch(iconImage.rectTransform, 7f, 7f);
            Stretch(iconFallbackText.rectTransform, 7f, 7f);
            SetLocalRect(eyebrowText.rectTransform, geometry.Eyebrow);
            SetLocalRect(itemTitleText.rectTransform, geometry.ItemTitle);
            SetLocalRect(resourceRow, geometry.ResourceRow);
            SetLocalRect(bodyText.rectTransform, geometry.Body);
            SetLocalRect(outcomePanel, geometry.Outcome);
            SetLocalRect(outcomeTitleText.rectTransform, new Rect(14f, 8f, geometry.Outcome.width - 28f, 20f));
            SetLocalRect(outcomeDetailText.rectTransform, new Rect(14f, 30f, geometry.Outcome.width - 28f, Mathf.Max(20f, geometry.Outcome.height - 36f)));
            SetLocalRect(reviewButton.GetComponent<RectTransform>(), geometry.ReviewButton);
            SetLocalRect(dismissButton.GetComponent<RectTransform>(), geometry.DismissButton);
            SetLocalRect(timerText.rectTransform, new Rect(24f, geometry.DismissButton.y, 48f, geometry.DismissButton.height));
            LayoutResourceChips();
        }

        private void RefreshResourceChip(Text text, int amount, string value)
        {
            bool visible = amount > 0;
            text.transform.parent.gameObject.SetActive(visible);
            text.text = visible ? value : "";
        }

        private void LayoutResourceChips()
        {
            if (resourceRow == null) return;
            Text[] chips = { goldText, suppliesText, elixirText };
            int visible = 0;
            foreach (Text chip in chips)
            {
                if (chip != null && chip.transform.parent.gameObject.activeSelf) visible++;
            }
            if (visible == 0)
            {
                resourceRow.gameObject.SetActive(false);
                return;
            }

            resourceRow.gameObject.SetActive(true);
            float gap = 8f;
            float chipW = Mathf.Min(130f, (resourceRow.rect.width - gap * (visible - 1)) / visible);
            int column = 0;
            foreach (Text chip in chips)
            {
                if (chip == null || !chip.transform.parent.gameObject.activeSelf) continue;
                SetLocalRect(chip.transform.parent.GetComponent<RectTransform>(), new Rect(column * (chipW + gap), 0f, chipW, resourceRow.rect.height));
                column++;
            }
        }

        private Text AddChip(string name, Transform parent, Color border)
        {
            RectTransform chip = AddPanel(name, parent, Hex("080b0d", 0.82f), border);
            Text text = AddText("Label", chip, "", 10, Hex("f3ead7", 1f), TextAnchor.MiddleCenter);
            text.fontStyle = FontStyle.Bold;
            Stretch(text.rectTransform, 6f, 2f);
            return text;
        }

        private Button AddButton(string name, Transform parent, string label, Action action, Color border)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(Outline));
            go.transform.SetParent(parent, false);
            Image image = go.GetComponent<Image>();
            image.color = Hex("151a1f", 0.96f);
            Button button = go.GetComponent<Button>();
            button.targetGraphic = image;
            ColorBlock colors = button.colors;
            colors.normalColor = image.color;
            colors.highlightedColor = Hex("263139", 1f);
            colors.pressedColor = Hex("0b1013", 1f);
            colors.disabledColor = Hex("0b0f12", 0.70f);
            colors.selectedColor = colors.highlightedColor;
            button.colors = colors;
            Outline outline = go.GetComponent<Outline>();
            outline.effectColor = border;
            outline.effectDistance = new Vector2(1f, -1f);
            if (action != null) button.onClick.AddListener(() => action());

            Text text = AddText("Label", go.transform, label, 11, Hex("f3ead7", 1f), TextAnchor.MiddleCenter);
            text.fontStyle = FontStyle.Bold;
            Stretch(text.rectTransform, 6f, 3f);
            return button;
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

        private RawImage AddRawImage(string name, Transform parent)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
            go.transform.SetParent(parent, false);
            RawImage image = go.GetComponent<RawImage>();
            image.raycastTarget = false;
            return image;
        }

        private Text AddText(string name, Transform parent, string value, int size, Color color, TextAnchor anchor)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            go.transform.SetParent(parent, false);
            Text text = go.GetComponent<Text>();
            text.font = font;
            text.text = value;
            text.fontSize = size;
            text.color = color;
            text.alignment = anchor;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            return text;
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

        private static void Stretch(RectTransform rect, float xPadding = 0f, float yPadding = 0f)
        {
            if (rect == null) return;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(xPadding, yPadding);
            rect.offsetMax = new Vector2(-xPadding, -yPadding);
        }

        private static Color Hex(string value, float alpha)
        {
            ColorUtility.TryParseHtmlString("#" + value, out Color color);
            color.a = alpha;
            return color;
        }

        private static Color ParseColor(string hex, Color fallback)
        {
            if (string.IsNullOrWhiteSpace(hex)) return fallback;
            if (!hex.StartsWith("#")) hex = "#" + hex;
            return ColorUtility.TryParseHtmlString(hex, out Color color) ? color : fallback;
        }
    }
}
