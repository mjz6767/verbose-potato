using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AshenHalls
{
    public sealed class TavernScreenBindings
    {
        public string Title;
        public string Subtitle;
        public Func<string> VersionLine;
        public Texture2D BackdropArt;
        public Texture2D TitleArt;
        public Func<bool> HasSavedGame;
        public Func<bool> SettingsVisible;
        public Func<bool> TestingVisible;
        public Func<bool> DeveloperTestingVisible;
        public Func<bool> AudioMuted;
        public Func<bool> MusicMuted;
        public Func<int> VolumePercent;
        public Func<int> MusicVolumePercent;
        public Func<bool> ReducedMotion;
        public Action Continue;
        public Action NewGame;
        public Action ToggleSettings;
        public Action ToggleTesting;
        public Action Quit;
        public Action CloseSettings;
        public Action ToggleAudio;
        public Action ToggleMusic;
        public Action VolumeDown;
        public Action VolumeUp;
        public Action MusicVolumeDown;
        public Action MusicVolumeUp;
        public Action ToggleReducedMotion;
        public Action BetaLab;
        public Action MartialLab;
        public Action KoboldLab;
    }

    public readonly struct TavernScreenGeometry
    {
        public readonly Rect Title;
        public readonly Rect Menu;
        public readonly Rect Settings;
        public readonly Rect Testing;

        public TavernScreenGeometry(Rect title, Rect menu, Rect settings, Rect testing)
        {
            Title = title;
            Menu = menu;
            Settings = settings;
            Testing = testing;
        }

        public bool Fits(float width, float height)
        {
            return FitsRect(Title, width, height)
                && FitsRect(Menu, width, height)
                && FitsRect(Settings, width, height)
                && FitsRect(Testing, width, height);
        }

        private static bool FitsRect(Rect rect, float width, float height)
        {
            return rect.xMin >= 0f && rect.yMin >= 0f && rect.xMax <= width && rect.yMax <= height;
        }
    }

    public static class TavernScreenLayout
    {
        public static TavernScreenGeometry Calculate(float width, float height, bool saveExists)
        {
            float titleW = Mathf.Clamp(width * 0.36f, 500f, 690f);
            float titleY = Mathf.Clamp(height * 0.075f, 48f, 92f);
            Rect title = new Rect(
                Mathf.Clamp(width * 0.045f, 30f, 92f),
                titleY,
                titleW,
                titleW / 3f);

            float menuW = Mathf.Clamp(width * 0.28f, 400f, 520f);
            float menuH = saveExists ? 414f : 356f;
            float menuX = Mathf.Clamp(width - menuW - Mathf.Clamp(width * 0.07f, 42f, 112f), 24f, width - menuW - 24f);
            float menuY = Mathf.Clamp(height * 0.20f, 96f, 170f);
            Rect menu = new Rect(menuX, menuY, menuW, menuH);
            if (menu.yMax > height - 88f) menu.y = Mathf.Max(72f, height - menu.height - 88f);

            float settingsW = 360f;
            float settingsH = 328f;
            float settingsX = menu.x - settingsW - 18f;
            if (settingsX < 24f) settingsX = Mathf.Clamp(menu.x, 24f, width - settingsW - 24f);
            float settingsY = settingsX < menu.x ? menu.y + 24f : Mathf.Min(menu.yMax + 12f, height - settingsH - 32f);
            Rect settings = new Rect(settingsX, Mathf.Max(24f, settingsY), settingsW, settingsH);

            float testingW = Mathf.Clamp(width - 96f, 360f, 560f);
            float testingX = menu.x + menu.width * 0.5f - testingW * 0.5f;
            float testingY = menu.yMax + 14f;
            if (testingY + 132f > height - 36f)
            {
                float sideX = menu.x - testingW - 18f;
                if (sideX >= 24f)
                {
                    testingX = sideX;
                    testingY = Mathf.Clamp(menu.yMax - 132f, 24f, height - 168f);
                }
                else
                {
                    testingY = Mathf.Clamp(height - 168f, 24f, height - 168f);
                }
            }
            Rect testing = new Rect(testingX, Mathf.Max(24f, testingY), testingW, 132f);
            return new TavernScreenGeometry(title, menu, settings, testing);
        }

        public static IReadOnlyList<Rect> ButtonRects(bool saveExists, float menuWidth)
        {
            List<Rect> rects = new List<Rect>();
            float y = 122f;
            float gap = 10f;
            float buttonW = menuWidth - 48f;
            if (saveExists)
            {
                rects.Add(new Rect(24f, y, buttonW, 48f));
                y += 58f;
            }

            rects.Add(new Rect(24f, y, buttonW, 64f));
            y += 64f + gap;
            rects.Add(new Rect(24f, y, buttonW, 42f));
            y += 42f + gap;
            rects.Add(new Rect(24f, y, buttonW, 42f));
            return rects;
        }

        public static Rect TestingButtonRect(float menuWidth, float menuHeight)
        {
            return new Rect(24f, menuHeight - 50f, menuWidth - 48f, 34f);
        }

        public static IReadOnlyList<Rect> StormWindowRects(float width, float height)
        {
            // These regions sit wholly inside the painted panes of the 16:9
            // tavern backdrop. Keeping weather inside them preserves the sense
            // that the party is sheltered while the storm remains alive outside.
            return new[]
            {
                new Rect(width * 0.494f, height * 0.132f, width * 0.064f, height * 0.244f),
                new Rect(width * 0.620f, height * 0.102f, width * 0.056f, height * 0.270f),
                new Rect(width * 0.724f, height * 0.104f, width * 0.060f, height * 0.264f),
                new Rect(width * 0.900f, height * 0.040f, width * 0.090f, height * 0.310f)
            };
        }
    }

    public readonly struct TavernTitleAnimationFrame
    {
        public readonly float FaceAlpha;
        public readonly float GlowAlpha;
        public readonly float ShadowAlpha;
        public readonly float SubtitleAlpha;
        public readonly float Scale;
        public readonly float UnderlineProgress;
        public readonly float UnderlineAlpha;
        public readonly float EmberIntensity;

        public TavernTitleAnimationFrame(
            float faceAlpha,
            float glowAlpha,
            float shadowAlpha,
            float subtitleAlpha,
            float scale,
            float underlineProgress,
            float underlineAlpha,
            float emberIntensity)
        {
            FaceAlpha = Mathf.Clamp01(faceAlpha);
            GlowAlpha = Mathf.Clamp01(glowAlpha);
            ShadowAlpha = Mathf.Clamp01(shadowAlpha);
            SubtitleAlpha = Mathf.Clamp01(subtitleAlpha);
            Scale = Mathf.Max(0.01f, scale);
            UnderlineProgress = Mathf.Clamp01(underlineProgress);
            UnderlineAlpha = Mathf.Clamp01(underlineAlpha);
            EmberIntensity = Mathf.Clamp01(emberIntensity);
        }
    }

    public static class TavernTitleAnimationRules
    {
        public const float RevealDuration = 1.45f;

        public static TavernTitleAnimationFrame Evaluate(float elapsedSeconds, bool reducedMotion)
        {
            if (reducedMotion)
            {
                return new TavernTitleAnimationFrame(1f, 0.17f, 0.88f, 1f, 1f, 1f, 0.44f, 0f);
            }

            float elapsed = Mathf.Max(0f, elapsedSeconds);
            float face = Smooth01((elapsed - 0.08f) / 0.82f);
            float subtitle = Smooth01((elapsed - 0.68f) / 0.56f);
            float underline = Smooth01((elapsed - 0.28f) / 0.74f);
            float strikeHeat = Mathf.Clamp01(1f - Mathf.Abs(elapsed - 0.72f) / 0.62f);
            float settledPulse = elapsed <= RevealDuration
                ? 0f
                : 0.5f + Mathf.Sin((elapsed - RevealDuration) * 1.55f) * 0.5f;
            float glow = 0.08f + face * 0.09f + strikeHeat * 0.31f + settledPulse * 0.035f;
            float underlineAlpha = underline * Mathf.Lerp(0.78f, 0.44f, Smooth01((elapsed - 0.92f) / 0.72f));
            float ember = face * Mathf.Lerp(0.62f, 0.11f, Smooth01((elapsed - 0.58f) / 1.55f));

            return new TavernTitleAnimationFrame(
                face,
                glow,
                face * 0.88f,
                subtitle,
                Mathf.Lerp(1.065f, 1f, face),
                underline,
                underlineAlpha,
                ember);
        }

        private static float Smooth01(float value)
        {
            value = Mathf.Clamp01(value);
            return value * value * (3f - 2f * value);
        }
    }

    public sealed class TavernScreen : MonoBehaviour
    {
        private TavernScreenBindings bindings;
        private Canvas canvas;
        private RectTransform titlePanel;
        private RectTransform menuPanel;
        private RectTransform settingsPanel;
        private RectTransform testingPanel;
        private RectTransform stormLayer;
        private Image titleArtImage;
        private readonly List<RectTransform> stormWindows = new List<RectTransform>();
        private readonly List<Image> lightningFlashes = new List<Image>();
        private readonly List<RectTransform> rainDrops = new List<RectTransform>();
        private readonly List<Image> titleEmbers = new List<Image>();
        private Text ashShadowText;
        private Text ashGlowText;
        private Text ashText;
        private Text titleShadowText;
        private Text titleGlowText;
        private Text titleText;
        private Text subtitleText;
        private RectTransform titleUnderlineGlow;
        private RectTransform titleUnderlineCore;
        private Text versionText;
        private Text menuEyebrowText;
        private Text menuTitleText;
        private Text menuHintText;
        private RectTransform menuRuleGlow;
        private RectTransform menuRuleCore;
        private Text settingsTitleText;
        private Text settingsHintText;
        private Text settingsStateText;
        private Text testingTitleText;
        private Text testingHintText;
        private Text audioButtonText;
        private Text sfxVolumeText;
        private Text musicVolumeText;
        private Text motionButtonText;
        private Text testingButtonText;
        private Text continueButtonText;
        private Text newGameButtonText;
        private Button continueButton;
        private Button newGameButton;
        private Button settingsButton;
        private Button testingButton;
        private Button quitButton;
        private Button volumeDownButton;
        private Button volumeUpButton;
        private Button musicVolumeDownButton;
        private Button musicVolumeUpButton;
        private Button musicValueButton;
        private Button closeSettingsButton;
        private Button betaLabButton;
        private Button martialLabButton;
        private Button koboldLabButton;
        private Font font;
        private float lastWidth = -1f;
        private float lastHeight = -1f;
        private bool lastSaveExists;
        private float titleAnimationStartedAt;
        private bool titleMotionInitialized;
        private bool lastTitleReducedMotion;

        private void Update()
        {
            UpdateStormMotion();
            UpdateTitleAnimation();
        }

        public void Bind(TavernScreenBindings screenBindings)
        {
            bindings = screenBindings;
            Build();
            Refresh();
        }

        public void SetVisible(bool visible)
        {
            if (canvas != null && canvas.gameObject.activeSelf != visible)
            {
                canvas.gameObject.SetActive(visible);
                if (visible) RestartTitleAnimation();
            }
        }

        public void Refresh()
        {
            if (bindings == null || canvas == null) return;
            bool saveExists = bindings.HasSavedGame != null && bindings.HasSavedGame();
            bool settingsVisible = bindings.SettingsVisible != null && bindings.SettingsVisible();
            bool testingVisible = bindings.TestingVisible != null && bindings.TestingVisible();
            bool devVisible = bindings.DeveloperTestingVisible != null && bindings.DeveloperTestingVisible();

            if (!Mathf.Approximately(lastWidth, Screen.width) || !Mathf.Approximately(lastHeight, Screen.height) || lastSaveExists != saveExists)
            {
                ApplyLayout(saveExists);
            }

            continueButton.gameObject.SetActive(saveExists);
            testingButton.gameObject.SetActive(devVisible);
            testingButtonText.text = testingVisible ? "Hide Beta Testing" : "Beta Testing";
            continueButtonText.text = "Continue the Old Road";
            newGameButtonText.text = saveExists ? "Begin a New Company" : "Begin the Old Road";
            settingsPanel.gameObject.SetActive(settingsVisible);
            testingPanel.gameObject.SetActive(devVisible && testingVisible);
            menuHintText.text = saveExists
                ? "The fire is banked. Your company can take up the road again."
                : "Gather a company by the fire, then step into the storm.";
            versionText.text = bindings.VersionLine == null ? "" : bindings.VersionLine();

            bool muted = bindings.AudioMuted != null && bindings.AudioMuted();
            bool musicMuted = bindings.MusicMuted != null && bindings.MusicMuted();
            int volume = bindings.VolumePercent == null ? 100 : Mathf.Clamp(bindings.VolumePercent(), 25, 100);
            int musicVolume = bindings.MusicVolumePercent == null ? 65 : Mathf.Clamp(bindings.MusicVolumePercent(), 25, 100);
            bool reduced = bindings.ReducedMotion != null && bindings.ReducedMotion();
            settingsStateText.text = $"Window {Screen.width} x {Screen.height}\nSFX {(muted ? "muted" : volume + "%")} / Music {(musicMuted ? "muted" : musicVolume + "%")} / Motion {(reduced ? "reduced" : "normal")}";
            audioButtonText.text = muted ? "Enable SFX" : "Mute SFX";
            sfxVolumeText.text = muted ? "SFX Muted" : $"SFX {volume}%";
            musicVolumeText.text = musicMuted ? "Music Muted" : $"Music {musicVolume}%";
            motionButtonText.text = reduced ? "Normal Motion" : "Reduced Motion";

            string title = string.IsNullOrWhiteSpace(bindings.Title) ? VersionInfo.ProductName : bindings.Title;
            SplitForgedTitle(title, out string ashLine, out string brimstoneLine);
            ashShadowText.text = ashLine;
            ashGlowText.text = ashLine;
            ashText.text = ashLine;
            titleShadowText.text = brimstoneLine;
            titleGlowText.text = brimstoneLine;
            titleText.text = brimstoneLine;
            subtitleText.text = SmallCaps(bindings.Subtitle);
        }

        private void Build()
        {
            EnsureEventSystem();
            font = UiRuntime.DefaultFont;

            canvas = UiRuntime.CreateOwnedRootCanvas(this, "Tavern Canvas");
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            CanvasScaler scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            canvas.gameObject.AddComponent<GraphicRaycaster>();
            Stretch(canvas.GetComponent<RectTransform>());

            Image baseImage = AddImage("Backdrop Base", canvas.transform, Hex("0e1114", 1f));
            Stretch(baseImage.rectTransform);
            if (bindings?.BackdropArt != null)
            {
                Image backdrop = AddImage("Tavern Backdrop", canvas.transform, Color.white);
                Stretch(backdrop.rectTransform);
                backdrop.sprite = Sprite.Create(bindings.BackdropArt, new Rect(0, 0, bindings.BackdropArt.width, bindings.BackdropArt.height), new Vector2(0.5f, 0.5f), 100f);
                backdrop.preserveAspect = false;
            }

            stormLayer = new GameObject("Storm Beyond The Glass", typeof(RectTransform)).GetComponent<RectTransform>();
            stormLayer.SetParent(canvas.transform, false);
            Stretch(stormLayer);
            for (int windowIndex = 0; windowIndex < 4; windowIndex++)
            {
                GameObject windowObject = new GameObject(
                    "Rain Beyond Window " + (windowIndex + 1),
                    typeof(RectTransform),
                    typeof(RectMask2D));
                RectTransform window = windowObject.GetComponent<RectTransform>();
                window.SetParent(stormLayer, false);
                stormWindows.Add(window);

                Image glassTint = AddImage("Storm Glass Tint", window, Hex("397896", 0.025f));
                glassTint.raycastTarget = false;
                Stretch(glassTint.rectTransform);

                Image lightning = AddImage("Distant Lightning", window, Hex("c7e7ff", 0f));
                lightning.raycastTarget = false;
                Stretch(lightning.rectTransform);
                lightningFlashes.Add(lightning);

                for (int lane = 0; lane < 8; lane++)
                {
                    int index = windowIndex * 8 + lane;
                    Image drop = AddImage(
                        "Exterior Rain " + index,
                        window,
                        Hex(index % 5 == 0 ? "d7efff" : "8bbdd5", 0.11f + (index % 4) * 0.024f));
                    drop.raycastTarget = false;
                    RectTransform dropRect = drop.rectTransform;
                    dropRect.pivot = new Vector2(0.5f, 0.5f);
                    dropRect.localEulerAngles = new Vector3(0f, 0f, -8f);
                    rainDrops.Add(dropRect);
                }
            }
            Image shade = AddImage("Tavern Color Grade", canvas.transform, Hex("030405", 0.14f));
            Stretch(shade.rectTransform);

            titlePanel = AddImage("Title Area", canvas.transform, Hex("020304", 0.02f)).rectTransform;
            if (bindings?.TitleArt != null)
            {
                titleArtImage = AddImage("Title Art", titlePanel, Color.white);
                titleArtImage.raycastTarget = false;
                titleArtImage.sprite = Sprite.Create(
                    bindings.TitleArt,
                    new Rect(0, 0, bindings.TitleArt.width, bindings.TitleArt.height),
                    new Vector2(0.5f, 0.5f),
                    100f);
                titleArtImage.preserveAspect = true;
                Stretch(titleArtImage.rectTransform);
            }
            titleUnderlineGlow = AddImage("Forge Line Glow", titlePanel, Hex("e45b25", 0f)).rectTransform;
            titleUnderlineGlow.GetComponent<Image>().raycastTarget = false;
            titleUnderlineCore = AddImage("Forge Line Core", titlePanel, Hex("ffd06a", 0f)).rectTransform;
            titleUnderlineCore.GetComponent<Image>().raycastTarget = false;
            for (int i = 0; i < 7; i++)
            {
                Image ember = AddImage("Title Ember " + i, titlePanel, Hex(i % 3 == 0 ? "fff0a1" : "e75b28", 0f));
                ember.raycastTarget = false;
                titleEmbers.Add(ember);
            }

            string titleValue = bindings?.Title ?? VersionInfo.ProductName;
            SplitForgedTitle(titleValue, out string ashLine, out string brimstoneLine);
            ashShadowText = AddText("Ash Emboss", titlePanel, ashLine, 34, Hex("090403", 0f), TextAnchor.MiddleCenter);
            ashGlowText = AddText("Ash Ember Halo", titlePanel, ashLine, 36, Hex("ef6328", 0f), TextAnchor.MiddleCenter);
            ashText = AddText("Ash Face", titlePanel, ashLine, 34, Hex("f3ead7", 0f), TextAnchor.MiddleCenter);
            titleShadowText = AddText("Brimstone Emboss", titlePanel, brimstoneLine, 50, Hex("090403", 0f), TextAnchor.MiddleCenter);
            titleGlowText = AddText("Brimstone Ember Halo", titlePanel, brimstoneLine, 52, Hex("ef6328", 0f), TextAnchor.MiddleCenter);
            titleText = AddText("Brimstone Face", titlePanel, brimstoneLine, 50, Hex("f3ead7", 0f), TextAnchor.MiddleCenter);
            ConfigureForgedTitleLayer(ashShadowText, 19, 34);
            ConfigureForgedTitleLayer(ashGlowText, 19, 36);
            ConfigureForgedTitleLayer(ashText, 19, 34);
            ConfigureForgedTitleLayer(titleShadowText, 28, 50);
            ConfigureForgedTitleLayer(titleGlowText, 28, 52);
            ConfigureForgedTitleLayer(titleText, 28, 50);
            DisableTextShadow(ashShadowText);
            DisableTextShadow(ashGlowText);
            DisableTextShadow(titleShadowText);
            DisableTextShadow(titleGlowText);
            Outline ashOutline = ashText.gameObject.AddComponent<Outline>();
            ashOutline.effectColor = Hex("3a130d", 0.78f);
            ashOutline.effectDistance = new Vector2(1f, -1f);
            Outline titleOutline = titleText.gameObject.AddComponent<Outline>();
            titleOutline.effectColor = Hex("3a130d", 0.88f);
            titleOutline.effectDistance = new Vector2(1.5f, -1.5f);
            subtitleText = AddText("Subtitle Text", titlePanel, SmallCaps(bindings?.Subtitle), 13, Hex("d7a84e", 1f), TextAnchor.MiddleCenter);
            subtitleText.font = UiRuntime.DialogueFont ?? font;
            subtitleText.fontStyle = FontStyle.Normal;
            subtitleText.raycastTarget = false;
            versionText = AddText("Version", canvas.transform, "", 10, Hex("b7aa90", 1f), TextAnchor.MiddleRight);

            menuPanel = AddPanel("Menu", canvas.transform, Hex("090a09", 0.88f), Hex("c69248", 0.72f));
            menuEyebrowText = AddText("Menu Eyebrow", menuPanel, "MIDGAARD  /  THE OLD ROAD", 10, Hex("b7aa90", 1f), TextAnchor.MiddleLeft);
            menuEyebrowText.font = UiRuntime.DialogueFont ?? font;
            menuEyebrowText.fontStyle = FontStyle.Normal;
            menuTitleText = AddText("Menu Title", menuPanel, "The Brimstone Hearth", 24, Hex("e7c477", 1f), TextAnchor.MiddleLeft);
            menuTitleText.font = UiRuntime.DialogueEmphasisFont ?? font;
            menuTitleText.fontStyle = FontStyle.Normal;
            menuHintText = AddText("Menu Hint", menuPanel, "", 12, Hex("e8dfcf", 1f), TextAnchor.UpperLeft);
            menuHintText.font = UiRuntime.DialogueFont ?? font;
            menuHintText.fontStyle = FontStyle.Normal;
            menuRuleGlow = AddImage("Menu Rule Glow", menuPanel, Hex("e45b25", 0.14f)).rectTransform;
            menuRuleGlow.GetComponent<Image>().raycastTarget = false;
            menuRuleCore = AddImage("Menu Rule Core", menuPanel, Hex("d7a84e", 0.68f)).rectTransform;
            menuRuleCore.GetComponent<Image>().raycastTarget = false;
            continueButton = AddButton("Continue", menuPanel, "Continue", bindings?.Continue, true);
            continueButtonText = continueButton.GetComponentInChildren<Text>();
            newGameButton = AddButton("New Game", menuPanel, "New Game", bindings?.NewGame, true);
            newGameButtonText = newGameButton.GetComponentInChildren<Text>();
            settingsButton = AddButton("Settings", menuPanel, "Settings", bindings?.ToggleSettings, false);
            quitButton = AddButton("Exit Game", menuPanel, "Leave Game", bindings?.Quit, false);
            testingButton = AddButton("Beta Testing", menuPanel, "Beta Testing", bindings?.ToggleTesting, false);
            testingButtonText = testingButton.GetComponentInChildren<Text>();

            settingsPanel = AddPanel("Settings", canvas.transform, Hex("080b0d", 0.96f), Hex("58b7a5", 0.86f));
            settingsTitleText = AddText("Settings Title", settingsPanel, "Settings", 20, Hex("58b7a5", 1f), TextAnchor.MiddleLeft);
            settingsHintText = AddText("Settings Hint", settingsPanel, "SFX, music, and motion settings apply immediately.", 11, Hex("b7aa90", 1f), TextAnchor.MiddleLeft);
            settingsStateText = AddText("Settings State", settingsPanel, "", 12, Hex("f3ead7", 1f), TextAnchor.MiddleLeft);
            Button audio = AddButton("Audio", settingsPanel, "Mute Audio", bindings?.ToggleAudio, false);
            audioButtonText = audio.GetComponentInChildren<Text>();
            volumeDownButton = AddButton("Volume Down", settingsPanel, "-", bindings?.VolumeDown, false);
            Button volume = AddButton("SFX Volume", settingsPanel, "SFX 100%", null, false);
            volume.interactable = false;
            sfxVolumeText = volume.GetComponentInChildren<Text>();
            volumeUpButton = AddButton("Volume Up", settingsPanel, "+", bindings?.VolumeUp, false);
            musicVolumeDownButton = AddButton("Music Volume Down", settingsPanel, "-", bindings?.MusicVolumeDown, false);
            musicValueButton = AddButton("Music Toggle", settingsPanel, "Music 65%", bindings?.ToggleMusic, false);
            musicVolumeText = musicValueButton.GetComponentInChildren<Text>();
            musicVolumeUpButton = AddButton("Music Volume Up", settingsPanel, "+", bindings?.MusicVolumeUp, false);
            Button motion = AddButton("Motion", settingsPanel, "Reduced Motion", bindings?.ToggleReducedMotion, false);
            motionButtonText = motion.GetComponentInChildren<Text>();
            closeSettingsButton = AddButton("Close", settingsPanel, "Close", bindings?.CloseSettings, false);

            testingPanel = AddPanel("Beta Testing Doors", canvas.transform, Hex("080b0d", 0.76f), Hex("8d6dcc", 0.50f));
            testingTitleText = AddText("Testing Title", testingPanel, "Beta Testing Doors", 16, Hex("8d6dcc", 1f), TextAnchor.MiddleLeft);
            testingHintText = AddText("Testing Hint", testingPanel, "Isolated labs for spell, martial, and route stress testing.", 10, Hex("b7aa90", 1f), TextAnchor.MiddleLeft);
            betaLabButton = AddButton("Beta Lab", testingPanel, "Beta Lab", bindings?.BetaLab, false);
            martialLabButton = AddButton("Martial Lab", testingPanel, "Martial Lab", bindings?.MartialLab, false);
            koboldLabButton = AddButton("Kobolds", testingPanel, "Kobolds", bindings?.KoboldLab, false);
            RestartTitleAnimation();
        }

        private void ApplyLayout(bool saveExists)
        {
            lastWidth = Screen.width;
            lastHeight = Screen.height;
            lastSaveExists = saveExists;
            TavernScreenGeometry geometry = TavernScreenLayout.Calculate(Screen.width, Screen.height, saveExists);
            SetScreenRect(titlePanel, geometry.Title);
            SetScreenRect(menuPanel, geometry.Menu);
            SetScreenRect(settingsPanel, geometry.Settings);
            SetScreenRect(testingPanel, geometry.Testing);
            SetScreenRect(versionText.rectTransform, new Rect(Screen.width - 476f, Screen.height - 42f, 430f, 20f));
            SetScreenRect(stormLayer, new Rect(0f, 0f, Screen.width, Screen.height));
            IReadOnlyList<Rect> windowRects = TavernScreenLayout.StormWindowRects(Screen.width, Screen.height);
            for (int i = 0; i < stormWindows.Count && i < windowRects.Count; i++)
            {
                SetLocalRect(stormWindows[i], windowRects[i]);
            }

            Rect ashArea = new Rect(
                geometry.Title.width * 0.20f,
                geometry.Title.height * 0.30f,
                geometry.Title.width * 0.60f,
                geometry.Title.height * 0.16f);
            Rect titleArea = new Rect(
                geometry.Title.width * 0.10f,
                geometry.Title.height * 0.43f,
                geometry.Title.width * 0.80f,
                geometry.Title.height * 0.25f);
            SetLocalRect(
                ashShadowText.rectTransform,
                new Rect(ashArea.x + 2f, ashArea.y + 2f, ashArea.width, ashArea.height));
            SetLocalRect(ashGlowText.rectTransform, ashArea);
            SetLocalRect(ashText.rectTransform, ashArea);
            SetLocalRect(
                titleShadowText.rectTransform,
                new Rect(titleArea.x + 3f, titleArea.y + 3f, titleArea.width, titleArea.height));
            SetLocalRect(titleGlowText.rectTransform, titleArea);
            SetLocalRect(titleText.rectTransform, titleArea);
            SetLocalRect(
                subtitleText.rectTransform,
                new Rect(
                    geometry.Title.width * 0.15f,
                    geometry.Title.height * 0.72f,
                    geometry.Title.width * 0.70f,
                    geometry.Title.height * 0.12f));
            SetLocalRect(menuEyebrowText.rectTransform, new Rect(24f, 14f, geometry.Menu.width - 48f, 16f));
            SetLocalRect(menuTitleText.rectTransform, new Rect(24f, 29f, geometry.Menu.width - 48f, 32f));
            SetLocalRect(menuRuleGlow, new Rect(24f, 65f, geometry.Menu.width - 48f, 5f));
            SetLocalRect(menuRuleCore, new Rect(24f, 67f, geometry.Menu.width - 48f, 1f));
            SetLocalRect(menuHintText.rectTransform, new Rect(30f, 76f, geometry.Menu.width - 60f, 38f));

            IReadOnlyList<Rect> buttons = TavernScreenLayout.ButtonRects(saveExists, geometry.Menu.width);
            int buttonIndex = 0;
            if (saveExists) SetLocalRect(continueButton.GetComponent<RectTransform>(), buttons[buttonIndex++]);
            SetLocalRect(newGameButton.GetComponent<RectTransform>(), buttons[buttonIndex++]);
            SetLocalRect(settingsButton.GetComponent<RectTransform>(), buttons[buttonIndex++]);
            SetLocalRect(quitButton.GetComponent<RectTransform>(), buttons[buttonIndex]);
            SetLocalRect(testingButton.GetComponent<RectTransform>(), TavernScreenLayout.TestingButtonRect(geometry.Menu.width, geometry.Menu.height));

            SetLocalRect(settingsTitleText.rectTransform, new Rect(18f, 14f, geometry.Settings.width - 36f, 26f));
            SetLocalRect(settingsHintText.rectTransform, new Rect(18f, 48f, geometry.Settings.width - 36f, 22f));
            SetLocalRect(settingsStateText.rectTransform, new Rect(18f, 76f, geometry.Settings.width - 36f, 44f));
            SetLocalRect(audioButtonText.transform.parent.GetComponent<RectTransform>(), new Rect(18f, 122f, geometry.Settings.width - 36f, 32f));
            SetLocalRect(volumeDownButton.GetComponent<RectTransform>(), new Rect(18f, 162f, 42f, 34f));
            SetLocalRect(sfxVolumeText.transform.parent.GetComponent<RectTransform>(), new Rect(68f, 162f, geometry.Settings.width - 136f, 34f));
            SetLocalRect(volumeUpButton.GetComponent<RectTransform>(), new Rect(geometry.Settings.width - 60f, 162f, 42f, 34f));
            SetLocalRect(musicVolumeDownButton.GetComponent<RectTransform>(), new Rect(18f, 204f, 42f, 34f));
            SetLocalRect(musicVolumeText.transform.parent.GetComponent<RectTransform>(), new Rect(68f, 204f, geometry.Settings.width - 136f, 34f));
            SetLocalRect(musicVolumeUpButton.GetComponent<RectTransform>(), new Rect(geometry.Settings.width - 60f, 204f, 42f, 34f));
            SetLocalRect(motionButtonText.transform.parent.GetComponent<RectTransform>(), new Rect(18f, 246f, geometry.Settings.width - 36f, 30f));
            SetLocalRect(closeSettingsButton.GetComponent<RectTransform>(), new Rect(18f, geometry.Settings.height - 48f, geometry.Settings.width - 36f, 30f));

            SetLocalRect(testingTitleText.rectTransform, new Rect(18f, 12f, geometry.Testing.width - 36f, 22f));
            SetLocalRect(testingHintText.rectTransform, new Rect(18f, 36f, geometry.Testing.width - 36f, 30f));
            float testerW = (geometry.Testing.width - 52f) / 3f;
            SetLocalRect(betaLabButton.GetComponent<RectTransform>(), new Rect(18f, geometry.Testing.height - 54f, testerW, 38f));
            SetLocalRect(martialLabButton.GetComponent<RectTransform>(), new Rect(26f + testerW, geometry.Testing.height - 54f, testerW, 38f));
            SetLocalRect(koboldLabButton.GetComponent<RectTransform>(), new Rect(34f + testerW * 2f, geometry.Testing.height - 54f, testerW, 38f));
        }

        private void ConfigureTitleLayer(Text text, int minSize, int maxSize)
        {
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = minSize;
            text.resizeTextMaxSize = maxSize;
            text.raycastTarget = false;
        }

        private void ConfigureForgedTitleLayer(Text text, int minSize, int maxSize)
        {
            ConfigureTitleLayer(text, minSize, maxSize);
            text.font = UiRuntime.DialogueEmphasisFont ?? font;
            text.fontStyle = FontStyle.Normal;
        }

        private static void DisableTextShadow(Text text)
        {
            Shadow shadow = text == null ? null : text.GetComponent<Shadow>();
            if (shadow != null) shadow.enabled = false;
        }

        private static void SplitForgedTitle(string value, out string ashLine, out string brimstoneLine)
        {
            string clean = string.IsNullOrWhiteSpace(value) ? VersionInfo.ProductName : value.Trim();
            int ampersand = clean.IndexOf('&');
            if (ampersand < 0)
            {
                ashLine = "";
                brimstoneLine = clean.ToUpperInvariant();
                return;
            }

            string lead = clean.Substring(0, ampersand).Trim().ToUpperInvariant();
            string tail = clean.Substring(ampersand + 1).Trim().ToUpperInvariant();
            ashLine = lead.Length <= 6 ? string.Join(" ", lead.ToCharArray()) : lead;
            brimstoneLine = string.IsNullOrEmpty(tail) ? "&" : "& " + tail;
        }

        private static string SmallCaps(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "" : value.Trim().ToUpperInvariant();
        }

        private void RestartTitleAnimation()
        {
            titleAnimationStartedAt = Time.unscaledTime;
            titleMotionInitialized = false;
        }

        private void UpdateTitleAnimation()
        {
            if (titlePanel == null || titleText == null || canvas == null || !canvas.gameObject.activeInHierarchy) return;

            bool reduced = bindings?.ReducedMotion != null && bindings.ReducedMotion();
            if (!titleMotionInitialized)
            {
                lastTitleReducedMotion = reduced;
                titleMotionInitialized = true;
            }
            else if (lastTitleReducedMotion != reduced)
            {
                lastTitleReducedMotion = reduced;
                if (!reduced) titleAnimationStartedAt = Time.unscaledTime;
            }

            float elapsed = reduced ? TavernTitleAnimationRules.RevealDuration : Time.unscaledTime - titleAnimationStartedAt;
            TavernTitleAnimationFrame frame = TavernTitleAnimationRules.Evaluate(elapsed, reduced);
            float ashAlpha = Mathf.Clamp01(frame.FaceAlpha * 1.18f);
            Color ashFaceColor = Color.Lerp(Hex("a44c2a", ashAlpha), Hex("e7c477", ashAlpha), ashAlpha);
            Color faceColor = Color.Lerp(Hex("b9552b", frame.FaceAlpha), Hex("fff0d0", frame.FaceAlpha), frame.FaceAlpha);
            ashText.color = ashFaceColor;
            ashShadowText.color = Hex("090403", frame.ShadowAlpha * 0.92f);
            ashGlowText.color = Hex("ef6328", frame.GlowAlpha * 0.72f);
            titleText.color = faceColor;
            titleShadowText.color = Hex("090403", frame.ShadowAlpha);
            titleGlowText.color = Hex("ef6328", frame.GlowAlpha);
            subtitleText.color = Hex("d7a84e", frame.SubtitleAlpha);
            ashText.rectTransform.localScale = Vector3.one * Mathf.Lerp(1.035f, 1f, ashAlpha);
            ashShadowText.rectTransform.localScale = ashText.rectTransform.localScale;
            ashGlowText.rectTransform.localScale = Vector3.one * Mathf.Lerp(1.052f, 1.012f, ashAlpha);
            titleText.rectTransform.localScale = Vector3.one * frame.Scale;
            titleShadowText.rectTransform.localScale = Vector3.one * frame.Scale;
            titleGlowText.rectTransform.localScale = Vector3.one * (frame.Scale + 0.018f);
            if (titleArtImage != null)
            {
                float plateWarmth = reduced ? 0.025f : 0.018f + Mathf.Sin(elapsed * 0.72f) * 0.012f;
                titleArtImage.color = Color.Lerp(Color.white, Hex("ffc16a", 1f), Mathf.Max(0f, plateWarmth));
            }

            float width = Mathf.Max(1f, titlePanel.rect.width);
            float height = Mathf.Max(1f, titlePanel.rect.height);
            float lineWidth = width * 0.46f * frame.UnderlineProgress;
            float lineX = width * 0.5f - lineWidth * 0.5f;
            float lineY = height * 0.695f;
            SetLocalRect(titleUnderlineGlow, new Rect(lineX, lineY - 2f, lineWidth, 7f));
            SetLocalRect(titleUnderlineCore, new Rect(lineX, lineY, lineWidth, 2f));
            titleUnderlineGlow.GetComponent<Image>().color = Hex("e45b25", frame.UnderlineAlpha * 0.30f);
            titleUnderlineCore.GetComponent<Image>().color = Hex("ffd06a", frame.UnderlineAlpha);

            for (int i = 0; i < titleEmbers.Count; i++)
            {
                Image ember = titleEmbers[i];
                bool show = !reduced && frame.EmberIntensity > 0.01f;
                if (ember.gameObject.activeSelf != show) ember.gameObject.SetActive(show);
                if (!show) continue;

                float phase = Mathf.Repeat(elapsed * (0.31f + i * 0.017f) + i * 0.163f, 1f);
                float lane = titleEmbers.Count <= 1 ? 0.5f : i / (float)(titleEmbers.Count - 1);
                float x = width * Mathf.Lerp(0.29f, 0.71f, lane)
                    + Mathf.Sin(elapsed * 1.7f + i * 1.91f) * width * 0.008f;
                float y = lineY - phase * height * 0.19f;
                float size = 2f + (i % 3);
                SetLocalRect(ember.rectTransform, new Rect(x - size * 0.5f, y - size * 0.5f, size, size));
                float lifeAlpha = Mathf.Sin(phase * Mathf.PI);
                ember.color = Hex(i % 3 == 0 ? "fff0a1" : "e75b28", frame.EmberIntensity * lifeAlpha * 0.72f);
            }
        }

        private void UpdateStormMotion()
        {
            if (stormLayer == null || canvas == null || !canvas.gameObject.activeInHierarchy) return;
            bool reduced = bindings?.ReducedMotion != null && bindings.ReducedMotion();
            if (stormLayer.gameObject.activeSelf == reduced) stormLayer.gameObject.SetActive(!reduced);
            if (reduced) return;

            float now = Time.unscaledTime;
            for (int i = 0; i < rainDrops.Count; i++)
            {
                RectTransform drop = rainDrops[i];
                if (drop == null) continue;
                RectTransform window = drop.parent as RectTransform;
                float width = Mathf.Max(1f, window == null ? 1f : window.rect.width);
                float height = Mathf.Max(1f, window == null ? 1f : window.rect.height);
                float speed = 0.28f + (i % 7) * 0.023f;
                float progress = Mathf.Repeat(now * speed + i * 0.137f, 1.16f) - 0.08f;
                float x = Mathf.Repeat(i * 0.271f + progress * 0.045f, 1f) * width;
                float y = progress * height;
                drop.anchorMin = new Vector2(0f, 1f);
                drop.anchorMax = new Vector2(0f, 1f);
                drop.anchoredPosition = new Vector2(x, -y);
                drop.sizeDelta = new Vector2(i % 6 == 0 ? 1.5f : 1f, 16f + (i % 5) * 5f);
            }

            float stormCycle = Mathf.Repeat(now, 13.7f);
            float firstFlash = Mathf.Clamp01(1f - Mathf.Abs(stormCycle - 0.22f) / 0.09f);
            float secondFlash = Mathf.Clamp01(1f - Mathf.Abs(stormCycle - 0.48f) / 0.055f);
            float lightning = Mathf.Max(firstFlash, secondFlash * 0.62f);
            for (int i = 0; i < lightningFlashes.Count; i++)
            {
                Image flash = lightningFlashes[i];
                if (flash == null) continue;
                float distance = 0.052f + (i % 3) * 0.012f;
                flash.color = Hex("c7e7ff", lightning * distance);
            }
        }

        private Button AddButton(string name, Transform parent, string label, Action action, bool hero)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            Image image = go.GetComponent<Image>();
            image.color = hero ? Hex("241711", 0.98f) : Hex("131615", 0.96f);
            Outline outline = go.AddComponent<Outline>();
            outline.effectColor = hero ? Hex("c69248", 0.52f) : Hex("7a6952", 0.24f);
            outline.effectDistance = new Vector2(1f, -1f);
            Button button = go.GetComponent<Button>();
            button.targetGraphic = image;
            ColorBlock colors = button.colors;
            colors.normalColor = image.color;
            colors.highlightedColor = hero ? Hex("3a2418", 1f) : Hex("242a27", 1f);
            colors.pressedColor = Hex("0b0d0c", 1f);
            colors.selectedColor = colors.highlightedColor;
            button.colors = colors;
            if (action != null) button.onClick.AddListener(() => action());

            if (hero)
            {
                Image edge = AddImage("Ember Edge", go.transform, Hex("e36d35", 0.82f));
                edge.raycastTarget = false;
                RectTransform edgeRect = edge.rectTransform;
                edgeRect.anchorMin = new Vector2(0f, 0f);
                edgeRect.anchorMax = new Vector2(0f, 1f);
                edgeRect.pivot = new Vector2(0f, 0.5f);
                edgeRect.offsetMin = new Vector2(0f, 6f);
                edgeRect.offsetMax = new Vector2(3f, -6f);
            }

            Text text = AddText("Label", go.transform, label, hero ? 16 : 14, Hex("f3ead7", 1f), TextAnchor.MiddleCenter);
            text.font = hero ? (UiRuntime.DialogueEmphasisFont ?? font) : font;
            text.fontStyle = hero ? FontStyle.Normal : FontStyle.Bold;
            Stretch(text.rectTransform, 12f, 4f);
            return button;
        }

        private RectTransform AddPanel(string name, Transform parent, Color fill, Color border)
        {
            RectTransform panel = AddImage(name, parent, fill).rectTransform;
            Outline outline = panel.gameObject.AddComponent<Outline>();
            outline.effectColor = border;
            outline.effectDistance = new Vector2(1f, -1f);
            return panel;
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
            Shadow shadow = go.AddComponent<Shadow>();
            shadow.effectColor = Hex("020304", size >= 20 ? 0.78f : 0.52f);
            shadow.effectDistance = size >= 20 ? new Vector2(2f, -2f) : new Vector2(1f, -1f);
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
