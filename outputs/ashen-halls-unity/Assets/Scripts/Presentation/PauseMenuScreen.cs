using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AshenHalls
{
    public sealed class PauseMenuView
    {
        public string Title;
        public string RouteLine;
        public string SaveLine;
        public string AudioLine;
        public string SfxLine;
        public string MusicLine;
        public bool SettingsOpen;
        public bool ShowRetreat;
        public bool RetreatEnabled;
        public bool ConfirmRetreat;
        public bool ConfirmReturnToTavern;
        public bool ConfirmNewGame;
    }

    public sealed class PauseMenuScreenBindings
    {
        public Func<PauseMenuView> View;
        public Action Continue;
        public Action Save;
        public Action Load;
        public Action ToggleSettings;
        public Action ToggleAudio;
        public Action ToggleMusic;
        public Action VolumeDown;
        public Action VolumeUp;
        public Action MusicVolumeDown;
        public Action MusicVolumeUp;
        public Action ToggleReducedMotion;
        public Action RequestRetreat;
        public Action ConfirmRetreat;
        public Action RequestReturnToTavern;
        public Action ConfirmReturnToTavern;
        public Action RequestNewGame;
        public Action ConfirmNewGame;
    }

    public readonly struct PauseMenuGeometry
    {
        public readonly Rect Scrim;
        public readonly Rect Panel;

        public PauseMenuGeometry(Rect scrim, Rect panel)
        {
            Scrim = scrim;
            Panel = panel;
        }

        public bool Fits(float width, float height)
        {
            return FitsRect(Scrim, width, height)
                && FitsRect(Panel, width, height)
                && Panel.width >= 320f
                && Panel.height >= 340f;
        }

        private static bool FitsRect(Rect rect, float width, float height)
        {
            return rect.xMin >= 0f && rect.yMin >= 0f && rect.xMax <= width && rect.yMax <= height;
        }
    }

    public static class PauseMenuScreenLayout
    {
        public static PauseMenuGeometry Calculate(float width, float height, bool settingsOpen)
        {
            float panelW = Mathf.Clamp(width * 0.30f, 360f, 480f);
            float panelH = settingsOpen ? 610f : 390f;
            panelH = Mathf.Min(panelH, height - 72f);
            Rect panel = new Rect((width - panelW) * 0.5f, (height - panelH) * 0.5f, panelW, panelH);
            return new PauseMenuGeometry(new Rect(0f, 0f, width, height), panel);
        }

        public static Rect ButtonRect(float panelWidth, int index)
        {
            const float x = 22f;
            const float y = 104f;
            const float h = 34f;
            const float gap = 8f;
            return new Rect(x, y + index * (h + gap), panelWidth - x * 2f, h);
        }
    }

    public sealed class PauseMenuScreen : MonoBehaviour
    {
        private PauseMenuScreenBindings bindings;
        private Canvas canvas;
        private RectTransform scrim;
        private RectTransform panel;
        private Text titleText;
        private Text routeText;
        private Text saveText;
        private Text settingsText;
        private Text returnText;
        private Text newText;
        private Text statusText;
        private Button continueButton;
        private Button saveButton;
        private Button loadButton;
        private Button settingsButton;
        private Button returnButton;
        private Button newButton;
        private RectTransform settingsPanel;
        private Button audioButton;
        private Button volumeDownButton;
        private Button volumeUpButton;
        private Button musicVolumeDownButton;
        private Button musicVolumeUpButton;
        private Button musicValueButton;
        private Button reducedMotionButton;
        private Text audioText;
        private Text sfxText;
        private Text musicText;
        private Text reducedMotionText;
        private Font font;
        private float lastWidth = -1f;
        private float lastHeight = -1f;
        private bool lastSettingsOpen;
        private bool lastRefreshSucceeded;

        public bool IsReady => canvas != null && panel != null && continueButton != null && settingsPanel != null;
        public bool IsVisible => IsReady && UiRuntime.IsCanvasVisible(canvas);
        public bool HasRenderableGeometry => IsReady
            && lastRefreshSucceeded
            && UiRuntime.CanOwnModal(canvas, panel, null, continueButton);

        public void Bind(PauseMenuScreenBindings screenBindings)
        {
            bindings = screenBindings;
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
            PauseMenuView view = bindings.View == null ? null : bindings.View();
            if (view == null) return;

            if (!Mathf.Approximately(lastWidth, Screen.width) || !Mathf.Approximately(lastHeight, Screen.height) || lastSettingsOpen != view.SettingsOpen)
            {
                ApplyLayout(view.SettingsOpen);
            }

            titleText.text = string.IsNullOrEmpty(view.Title) ? "Menu" : view.Title;
            routeText.text = view.RouteLine ?? "";
            saveText.text = view.SaveLine ?? "";
            settingsText.text = view.SettingsOpen ? "Hide Settings" : "Settings";
            returnText.text = view.ShowRetreat
                ? view.ConfirmRetreat
                    ? $"Confirm Retreat ({CombatRetreatRules.SupplyCost} Supply)"
                    : view.RetreatEnabled
                        ? $"Retreat to Midgaard ({CombatRetreatRules.SupplyCost} Supply)"
                        : "Retreat (No Supplies)"
                : view.ConfirmReturnToTavern ? "Confirm Return" : "Return to Tavern";
            returnButton.interactable = !view.ShowRetreat || view.RetreatEnabled;
            newText.text = view.ConfirmNewGame ? "Confirm New Game" : "New Game";
            statusText.text = view.ConfirmRetreat
                ? "Spend one supply, abandon this fight and its loot, then recover at Temple Square."
                : view.ShowRetreat && !view.RetreatEnabled
                    ? "Retreat needs one supply. Continue fighting or restore the pre-fight checkpoint."
                    : view.ConfirmReturnToTavern || view.ConfirmNewGame
                        ? "This will leave the current run in memory. Save first if you want to keep it."
                        : "Esc closes this menu. Save and load use the campaign slot.";
            settingsPanel.gameObject.SetActive(view.SettingsOpen);
            audioText.text = view.AudioLine ?? "";
            sfxText.text = view.SfxLine ?? "";
            musicText.text = view.MusicLine ?? "";
            reducedMotionText.text = view.SettingsOpen ? "Reduced Motion" : "";
            Canvas.ForceUpdateCanvases();
            lastRefreshSucceeded = true;
        }

        private void Build()
        {
            EnsureEventSystem();
            font = UiRuntime.DefaultFont;
            canvas = UiRuntime.CreateOwnedRootCanvas(this, "Pause Menu Canvas");
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 30;
            CanvasScaler scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            canvas.gameObject.AddComponent<GraphicRaycaster>();
            Stretch(canvas.GetComponent<RectTransform>());

            scrim = AddImage("Scrim", canvas.transform, Hex("030506", 0.58f)).rectTransform;
            panel = AddPanel("Pause Menu", canvas.transform, Hex("10161b", 0.98f), Hex("d7a84e", 0.88f));
            titleText = AddText("Title", panel, "Menu", 24, Hex("f3ead7", 1f), TextAnchor.MiddleLeft);
            routeText = AddText("Route", panel, "", 11, Hex("b7aa90", 1f), TextAnchor.MiddleLeft);
            saveText = AddText("Save State", panel, "", 10, Hex("58b7a5", 1f), TextAnchor.MiddleLeft);
            statusText = AddText("Status", panel, "", 10, Hex("b7aa90", 1f), TextAnchor.UpperLeft);

            continueButton = AddButton("Continue", panel, "Continue", () => bindings?.Continue?.Invoke(), true);
            saveButton = AddButton("Save", panel, "Save", () => bindings?.Save?.Invoke(), false);
            loadButton = AddButton("Load", panel, "Load", () => bindings?.Load?.Invoke(), false);
            settingsButton = AddButton("Settings", panel, "Settings", () => bindings?.ToggleSettings?.Invoke(), false);
            settingsText = settingsButton.GetComponentInChildren<Text>();
            returnButton = AddButton("Return", panel, "Return to Tavern", RunReturnAction, false);
            returnText = returnButton.GetComponentInChildren<Text>();
            newButton = AddButton("New Game", panel, "New Game", RunNewGameAction, false);
            newText = newButton.GetComponentInChildren<Text>();

            settingsPanel = AddPanel("Settings Panel", panel, Hex("080b0d", 0.82f), Hex("58b7a5", 0.58f));
            audioButton = AddButton("Audio", settingsPanel, "Audio", () => bindings?.ToggleAudio?.Invoke(), false);
            audioText = audioButton.GetComponentInChildren<Text>();
            volumeDownButton = AddButton("Volume Down", settingsPanel, "- Volume", () => bindings?.VolumeDown?.Invoke(), false);
            volumeUpButton = AddButton("Volume Up", settingsPanel, "+ Volume", () => bindings?.VolumeUp?.Invoke(), false);
            Button sfxValue = AddButton("SFX Value", settingsPanel, "SFX", null, false);
            sfxValue.interactable = false;
            sfxText = sfxValue.GetComponentInChildren<Text>();
            musicVolumeDownButton = AddButton("Music Down", settingsPanel, "- Music", () => bindings?.MusicVolumeDown?.Invoke(), false);
            musicVolumeUpButton = AddButton("Music Up", settingsPanel, "+ Music", () => bindings?.MusicVolumeUp?.Invoke(), false);
            musicValueButton = AddButton("Music Toggle", settingsPanel, "Music", () => bindings?.ToggleMusic?.Invoke(), false);
            musicText = musicValueButton.GetComponentInChildren<Text>();
            reducedMotionButton = AddButton("Reduced Motion", settingsPanel, "Reduced Motion", () => bindings?.ToggleReducedMotion?.Invoke(), false);
            reducedMotionText = reducedMotionButton.GetComponentInChildren<Text>();
            settingsPanel.gameObject.SetActive(false);
        }

        private void RunReturnAction()
        {
            PauseMenuView view = bindings?.View == null ? null : bindings.View();
            if (view != null && view.ShowRetreat)
            {
                if (!view.RetreatEnabled) return;
                if (view.ConfirmRetreat) bindings?.ConfirmRetreat?.Invoke();
                else bindings?.RequestRetreat?.Invoke();
                return;
            }
            if (view != null && view.ConfirmReturnToTavern) bindings?.ConfirmReturnToTavern?.Invoke();
            else bindings?.RequestReturnToTavern?.Invoke();
        }

        private void RunNewGameAction()
        {
            PauseMenuView view = bindings?.View == null ? null : bindings.View();
            if (view != null && view.ConfirmNewGame) bindings?.ConfirmNewGame?.Invoke();
            else bindings?.RequestNewGame?.Invoke();
        }

        private void ApplyLayout(bool settingsOpen)
        {
            lastWidth = Screen.width;
            lastHeight = Screen.height;
            lastSettingsOpen = settingsOpen;
            PauseMenuGeometry geometry = PauseMenuScreenLayout.Calculate(Screen.width, Screen.height, settingsOpen);
            SetScreenRect(scrim, geometry.Scrim);
            SetScreenRect(panel, geometry.Panel);
            SetLocalRect(titleText.rectTransform, new Rect(22f, 18f, geometry.Panel.width - 44f, 30f));
            SetLocalRect(routeText.rectTransform, new Rect(24f, 50f, geometry.Panel.width - 48f, 18f));
            SetLocalRect(saveText.rectTransform, new Rect(24f, 72f, geometry.Panel.width - 48f, 18f));

            SetLocalRect(continueButton.GetComponent<RectTransform>(), PauseMenuScreenLayout.ButtonRect(geometry.Panel.width, 0));
            SetLocalRect(saveButton.GetComponent<RectTransform>(), PauseMenuScreenLayout.ButtonRect(geometry.Panel.width, 1));
            SetLocalRect(loadButton.GetComponent<RectTransform>(), PauseMenuScreenLayout.ButtonRect(geometry.Panel.width, 2));
            SetLocalRect(settingsButton.GetComponent<RectTransform>(), PauseMenuScreenLayout.ButtonRect(geometry.Panel.width, 3));
            SetLocalRect(returnButton.GetComponent<RectTransform>(), PauseMenuScreenLayout.ButtonRect(geometry.Panel.width, 4));
            SetLocalRect(newButton.GetComponent<RectTransform>(), PauseMenuScreenLayout.ButtonRect(geometry.Panel.width, 5));

            float statusY = settingsOpen ? geometry.Panel.height - 58f : geometry.Panel.height - 52f;
            SetLocalRect(statusText.rectTransform, new Rect(24f, statusY, geometry.Panel.width - 48f, 34f));
            SetLocalRect(settingsPanel, new Rect(22f, 356f, geometry.Panel.width - 44f, 184f));
            SetLocalRect(audioButton.GetComponent<RectTransform>(), new Rect(12f, 12f, settingsPanel.rect.width - 24f, 28f));
            float sideW = 82f;
            float valueW = Mathf.Max(90f, settingsPanel.rect.width - sideW * 2f - 40f);
            SetLocalRect(volumeDownButton.GetComponent<RectTransform>(), new Rect(12f, 48f, sideW, 28f));
            SetLocalRect(sfxText.transform.parent.GetComponent<RectTransform>(), new Rect(102f, 48f, valueW, 28f));
            SetLocalRect(volumeUpButton.GetComponent<RectTransform>(), new Rect(settingsPanel.rect.width - sideW - 12f, 48f, sideW, 28f));
            SetLocalRect(musicVolumeDownButton.GetComponent<RectTransform>(), new Rect(12f, 84f, sideW, 28f));
            SetLocalRect(musicText.transform.parent.GetComponent<RectTransform>(), new Rect(102f, 84f, valueW, 28f));
            SetLocalRect(musicVolumeUpButton.GetComponent<RectTransform>(), new Rect(settingsPanel.rect.width - sideW - 12f, 84f, sideW, 28f));
            SetLocalRect(reducedMotionButton.GetComponent<RectTransform>(), new Rect(12f, 124f, settingsPanel.rect.width - 24f, 28f));
        }

        private Button AddButton(string name, Transform parent, string label, Action action, bool hero)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(Outline));
            go.transform.SetParent(parent, false);
            Image image = go.GetComponent<Image>();
            image.color = hero ? Hex("243033", 0.98f) : Hex("151b20", 0.96f);
            Button button = go.GetComponent<Button>();
            button.targetGraphic = image;
            ColorBlock colors = button.colors;
            colors.normalColor = image.color;
            colors.highlightedColor = hero ? Hex("304146", 1f) : Hex("232a31", 1f);
            colors.pressedColor = Hex("0b1013", 1f);
            colors.disabledColor = Hex("0b0f12", 0.72f);
            colors.selectedColor = colors.highlightedColor;
            button.colors = colors;
            Outline outline = go.GetComponent<Outline>();
            outline.effectColor = hero ? Hex("58b7a5", 0.95f) : Hex("3c4544", 0.86f);
            outline.effectDistance = new Vector2(1f, -1f);
            if (action != null) button.onClick.AddListener(() => action());
            Text text = AddText("Label", go.transform, label, hero ? 14 : 12, Hex("f3ead7", 1f), TextAnchor.MiddleCenter);
            text.fontStyle = FontStyle.Bold;
            Stretch(text.rectTransform, 8f, 4f);
            return button;
        }

        private RectTransform AddPanel(string name, Transform parent, Color fill, Color border)
        {
            RectTransform rect = AddImage(name, parent, fill).rectTransform;
            Outline outline = rect.gameObject.AddComponent<Outline>();
            outline.effectColor = border;
            outline.effectDistance = new Vector2(1f, -1f);
            return rect;
        }

        private Image AddImage(string name, Transform parent, Color color)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(parent, false);
            Image image = go.GetComponent<Image>();
            image.color = color;
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
            if (size >= 20) text.fontStyle = FontStyle.Bold;
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

        private static void EnsureEventSystem()
        {
            UiRuntime.EnsureEventSystemReady();
        }
    }
}
