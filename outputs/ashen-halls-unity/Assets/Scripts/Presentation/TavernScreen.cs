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
        public Func<string> VersionLine;
        public Texture2D BackdropArt;
        public Texture2D TitleArt;
        public Texture2D MenuIconAtlas;
        public Texture2D MenuScrollArt;
        public Texture2D MenuFocusArt;
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
        public Action<string, float> PlayTitleCue;
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

    public readonly struct TavernMenuScrollGeometry
    {
        public readonly Rect Bounds;
        public readonly Rect Sheet;
        public readonly Rect TopRoll;
        public readonly Rect BottomRoll;
        public readonly Rect Content;
        public readonly Rect Header;
        public readonly Rect Rule;

        public TavernMenuScrollGeometry(
            Rect bounds,
            Rect sheet,
            Rect topRoll,
            Rect bottomRoll,
            Rect content,
            Rect header,
            Rect rule)
        {
            Bounds = bounds;
            Sheet = sheet;
            TopRoll = topRoll;
            BottomRoll = bottomRoll;
            Content = content;
            Header = header;
            Rule = rule;
        }

        public bool Fits(float width, float height)
        {
            return Contains(Bounds, Sheet)
                && Contains(Bounds, TopRoll)
                && Contains(Bounds, BottomRoll)
                && Contains(Bounds, Content)
                && Contains(Content, Header)
                && Contains(Content, Rule)
                && Bounds.xMin >= 0f
                && Bounds.yMin >= 0f
                && Bounds.xMax <= width
                && Bounds.yMax <= height;
        }

        public bool ContainsContent(Rect rect)
        {
            return Contains(Content, rect);
        }

        private static bool Contains(Rect outer, Rect inner)
        {
            return inner.width > 0f
                && inner.height > 0f
                && inner.xMin >= outer.xMin - 0.01f
                && inner.yMin >= outer.yMin - 0.01f
                && inner.xMax <= outer.xMax + 0.01f
                && inner.yMax <= outer.yMax + 0.01f;
        }
    }

    public static class TavernScreenLayout
    {
        public static bool IsCompactMenu(float menuWidth)
        {
            return menuWidth < 240f;
        }

        public static TavernScreenGeometry Calculate(
            float width,
            float height,
            bool saveExists,
            bool developerTestingVisible = false,
            float artWidth = 1672f,
            float artHeight = 941f)
        {
            width = Mathf.Max(1f, width);
            height = Mathf.Max(1f, height);
            TitleBackdropProjection projection = TitleScreenPresentationRules.ProjectBackdrop(
                width,
                height,
                artWidth,
                artHeight);
            Rect logoSafeZone = projection.ProjectNormalized(TitleScreenPresentationRules.LogoSafeZoneNormalized);
            Rect menuSafeZone = projection.ProjectNormalized(TitleScreenPresentationRules.MenuSafeZoneNormalized);

            float titleX = Mathf.Clamp(width * 0.035f, 28f, 72f);
            float titleY = Mathf.Clamp(height * 0.06f, 34f, 54f);
            float desiredTitleW = Mathf.Clamp(
                Mathf.Min(width * 0.38f, height * 0.75f),
                280f,
                700f);
            float titleSafeW = Mathf.Max(280f, Mathf.Min(width - titleX - 24f, logoSafeZone.xMax - titleX));
            float titleW = Mathf.Min(desiredTitleW, titleSafeW);
            Rect title = new Rect(titleX, titleY, titleW, titleW / 3f);

            float desiredMenuW = Mathf.Clamp(
                Mathf.Min(width * 0.255f, height * 0.515f),
                256f,
                438f);
            float rightInset = Mathf.Clamp(width * 0.015f, 16f, 32f);
            float menuSafeW = Mathf.Max(176f, width - rightInset - menuSafeZone.xMin);
            float menuW = Mathf.Min(desiredMenuW, menuSafeW);
            float menuH = MenuHeight(menuW, saveExists, developerTestingVisible);
            float menuX = Mathf.Max(16f, width - menuW - rightInset);
            float menuY = Mathf.Clamp(height * 0.30f, 112f, height - menuH - 42f);
            Rect menu = new Rect(menuX, menuY, menuW, menuH);

            float settingsW = Mathf.Max(288f, menuW);
            float settingsH = 352f;
            float settingsX = menu.xMax - settingsW;
            float settingsY = Mathf.Clamp(menu.y, 24f, height - settingsH - 32f);
            Rect settings = new Rect(settingsX, settingsY, settingsW, settingsH);

            float testingW = Mathf.Clamp(width * 0.43f, 360f, 620f);
            float testingX = titleX;
            float testingY = Mathf.Clamp(height - 174f, 24f, height - 156f);
            Rect testing = new Rect(testingX, testingY, testingW, 132f);

            return new TavernScreenGeometry(title, menu, settings, testing);
        }

        public static IReadOnlyList<Rect> ButtonRects(bool saveExists, float menuWidth)
        {
            List<Rect> rects = new List<Rect>();
            bool compact = IsCompactMenu(menuWidth);
            TavernMenuScrollGeometry scroll = ScrollGeometry(menuWidth, MenuHeight(menuWidth, saveExists, false));
            float y = scroll.Rule.yMax + (compact ? 8f : 12f);
            float gap = compact ? 6f : 8f;
            float heroHeight = compact ? 48f : 54f;
            float regularHeight = compact ? 44f : 48f;
            rects.Add(new Rect(scroll.Content.x, y, scroll.Content.width, heroHeight));
            y += heroHeight + gap;
            rects.Add(new Rect(scroll.Content.x, y, scroll.Content.width, heroHeight));
            y += heroHeight + gap;
            rects.Add(new Rect(scroll.Content.x, y, scroll.Content.width, regularHeight));
            y += regularHeight + gap;
            rects.Add(new Rect(scroll.Content.x, y, scroll.Content.width, regularHeight));
            return rects;
        }

        public static Rect TestingButtonRect(
            bool saveExists,
            float menuWidth,
            bool developerTestingVisible)
        {
            if (!developerTestingVisible) return Rect.zero;
            bool compact = IsCompactMenu(menuWidth);
            IReadOnlyList<Rect> buttons = ButtonRects(saveExists, menuWidth);
            Rect lastButton = buttons[buttons.Count - 1];
            return new Rect(
                lastButton.x,
                lastButton.yMax + (compact ? 10f : 12f),
                lastButton.width,
                compact ? 44f : 48f);
        }

        public static TavernMenuScrollGeometry ScrollGeometry(float menuWidth, float menuHeight)
        {
            float width = Mathf.Max(1f, menuWidth);
            float height = Mathf.Max(1f, menuHeight);
            bool compact = IsCompactMenu(width);
            float rollHeight = RollHeight(width);
            float bodySide = BodySide(width);
            float contentX = ContentSideInset(width);
            float contentWidth = Mathf.Max(1f, width - contentX * 2f);
            float contentY = ContentTopInset(width);
            Rect bounds = new Rect(0f, 0f, width, height);
            Rect sheet = new Rect(bodySide, rollHeight * 0.5f, width - bodySide * 2f, height - rollHeight);
            Rect topRoll = new Rect(0f, 0f, width, rollHeight);
            Rect bottomRoll = new Rect(0f, height - rollHeight, width, rollHeight);
            Rect content = new Rect(
                contentX,
                contentY,
                contentWidth,
                Mathf.Max(1f, height - contentY * 2f));
            Rect header = new Rect(content.x, content.y, content.width, compact ? 24f : 30f);
            Rect rule = new Rect(content.x, header.yMax + 4f, content.width, 2f);
            return new TavernMenuScrollGeometry(bounds, sheet, topRoll, bottomRoll, content, header, rule);
        }

        public static float MenuHeight(float menuWidth, bool saveExists, bool developerTestingVisible)
        {
            bool compact = IsCompactMenu(menuWidth);
            IReadOnlyList<Rect> buttons = ButtonRectsForHeight(saveExists, menuWidth);
            float lastVisibleY = buttons[buttons.Count - 1].yMax;
            if (developerTestingVisible)
            {
                lastVisibleY += (compact ? 10f : 12f) + (compact ? 44f : 48f);
            }
            return Mathf.Ceil(
                lastVisibleY
                + (compact ? 10f : 14f)
                + Mathf.Max(RollHeight(menuWidth), TitleScreenPresentationRules.MenuScrollBottomInset));
        }

        private static IReadOnlyList<Rect> ButtonRectsForHeight(bool saveExists, float menuWidth)
        {
            List<Rect> rects = new List<Rect>();
            bool compact = IsCompactMenu(menuWidth);
            float rollHeight = RollHeight(menuWidth);
            float contentX = ContentSideInset(menuWidth);
            float contentWidth = Mathf.Max(1f, menuWidth - contentX * 2f);
            float headerHeight = compact ? 24f : 30f;
            float y = ContentTopInset(menuWidth) + headerHeight + 4f + 2f + (compact ? 8f : 12f);
            float gap = compact ? 6f : 8f;
            float heroHeight = compact ? 48f : 54f;
            float regularHeight = compact ? 44f : 48f;
            rects.Add(new Rect(contentX, y, contentWidth, heroHeight));
            y += heroHeight + gap;
            rects.Add(new Rect(contentX, y, contentWidth, heroHeight));
            y += heroHeight + gap;
            rects.Add(new Rect(contentX, y, contentWidth, regularHeight));
            y += regularHeight + gap;
            rects.Add(new Rect(contentX, y, contentWidth, regularHeight));
            return rects;
        }

        private static float RollHeight(float menuWidth)
        {
            return Mathf.Clamp(menuWidth * 0.075f, 16f, 26f);
        }

        private static float BodySide(float menuWidth)
        {
            return Mathf.Clamp(menuWidth * 0.032f, 6f, 14f);
        }

        private static float PaperPadding(float menuWidth)
        {
            return Mathf.Clamp(menuWidth * 0.045f, 8f, 18f);
        }

        private static float ContentSideInset(float menuWidth)
        {
            return Mathf.Max(
                BodySide(menuWidth) + PaperPadding(menuWidth),
                TitleScreenPresentationRules.MenuScrollSideInset);
        }

        private static float ContentTopInset(float menuWidth)
        {
            return Mathf.Max(
                RollHeight(menuWidth) + 8f,
                TitleScreenPresentationRules.MenuScrollTopInset);
        }

        public static IReadOnlyList<Rect> StormWindowRects(
            float width,
            float height,
            float artWidth,
            float artHeight)
        {
            TitleBackdropProjection projection = TitleScreenPresentationRules.ProjectBackdrop(
                width,
                height,
                artWidth,
                artHeight);
            Rect[] normalizedArtWindows =
            {
                new Rect(0.594f, 0.145f, 0.140f, 0.165f),
                new Rect(0.570f, 0.315f, 0.074f, 0.435f),
                new Rect(0.646f, 0.315f, 0.082f, 0.445f),
                new Rect(0.730f, 0.318f, 0.066f, 0.437f)
            };
            Rect[] projected = new Rect[normalizedArtWindows.Length];
            for (int i = 0; i < normalizedArtWindows.Length; i++)
            {
                projected[i] = projection.ProjectNormalized(normalizedArtWindows[i]);
            }
            return projected;
        }
    }

    public readonly struct TavernTitleAnimationFrame
    {
        public readonly float FaceAlpha;
        public readonly float GlowAlpha;
        public readonly float ShadowAlpha;
        public readonly float Scale;
        public readonly float UnderlineProgress;
        public readonly float UnderlineAlpha;
        public readonly float EmberIntensity;

        public TavernTitleAnimationFrame(
            float faceAlpha,
            float glowAlpha,
            float shadowAlpha,
            float scale,
            float underlineProgress,
            float underlineAlpha,
            float emberIntensity)
        {
            FaceAlpha = Mathf.Clamp01(faceAlpha);
            GlowAlpha = Mathf.Clamp01(glowAlpha);
            ShadowAlpha = Mathf.Clamp01(shadowAlpha);
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
                return new TavernTitleAnimationFrame(1f, 0.17f, 0.88f, 1f, 1f, 0.44f, 0f);
            }

            float elapsed = Mathf.Max(0f, elapsedSeconds);
            float face = Smooth01((elapsed - 0.08f) / 0.82f);
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

    internal sealed class TavernMenuFocusRelay : MonoBehaviour, IPointerEnterHandler, ISelectHandler
    {
        public TavernScreen Owner;
        public int ChoiceIndex;

        public void OnPointerEnter(PointerEventData eventData)
        {
            Owner?.FocusMenuChoice(ChoiceIndex, true, true);
        }

        public void OnSelect(BaseEventData eventData)
        {
            Owner?.FocusMenuChoice(ChoiceIndex, true, false);
        }
    }

    public sealed class TavernScreen : MonoBehaviour
    {
        private sealed class TitleChoiceVisual
        {
            public TitleMenuChoiceKind Kind;
            public bool Hero;
            public Button Button;
            public Image FocusPlaque;
            public Image IconFrame;
            public Image Icon;
            public Text Cursor;
            public RectTransform FocusRule;
            public Text Label;
        }

        private TavernScreenBindings bindings;
        private Canvas canvas;
        private RectTransform backdropRect;
        private RectTransform atmosphereLayer;
        private RectTransform titlePanel;
        private RectTransform menuPanel;
        private RectTransform menuAuthoredScrollShadow;
        private RectTransform menuAuthoredScrollFrame;
        private RectTransform menuScrollShadow;
        private RectTransform menuScrollSheet;
        private RectTransform menuScrollTopRoll;
        private RectTransform menuScrollBottomRoll;
        private RectTransform menuScrollTopHighlight;
        private RectTransform menuScrollTopShade;
        private RectTransform menuScrollBottomHighlight;
        private RectTransform menuScrollBottomShade;
        private RectTransform menuScrollLeftShade;
        private RectTransform menuScrollRightShade;
        private readonly List<RectTransform> menuScrollEndCaps = new List<RectTransform>();
        private RectTransform settingsPanel;
        private RectTransform testingPanel;
        private RectTransform stormLayer;
        private CanvasGroup menuCanvasGroup;
        private Image openingVeil;
        private Image colorGrade;
        private Image hearthGlow;
        private Image hearthFireboxFlicker;
        private Image gateGlow;
        private Image titleArtImage;
        private readonly List<RectTransform> stormWindows = new List<RectTransform>();
        private readonly List<Image> lightningFlashes = new List<Image>();
        private readonly List<RectTransform> rainDrops = new List<RectTransform>();
        private readonly List<Image> titleEmbers = new List<Image>();
        private readonly List<Image> atmosphereEmbers = new List<Image>();
        private readonly List<TitleChoiceVisual> titleChoices = new List<TitleChoiceVisual>();
        private Text ashShadowText;
        private Text ashGlowText;
        private Text ashText;
        private Text titleShadowText;
        private Text titleGlowText;
        private Text titleText;
        private RectTransform titleUnderlineGlow;
        private RectTransform titleUnderlineCore;
        private Text versionText;
        private Text menuTitleText;
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
        private bool lastDeveloperTestingVisible;
        private int lastPhysicalScreenWidth = -1;
        private int lastPhysicalScreenHeight = -1;
        private float titleAnimationStartedAt;
        private bool titleMotionInitialized;
        private bool lastTitleReducedMotion;
        private float previousTitleElapsed;
        private bool revealStrikePlayed;
        private bool revealChimePlayed;
        private int focusedMenuChoice = -1;
        private float lastMenuFocusCueAt = -10f;
        private Vector2 menuRestPosition;
        private Sprite softGlowSprite;
        private Sprite parchmentSprite;
        private Sprite authoredMenuScrollSprite;
        private Sprite authoredMenuFocusSprite;

        private void Update()
        {
            EnsureLayoutCurrent();
            UpdateStormMotion();
            UpdateTitleAnimation();
            UpdateOpeningPresentation();
            UpdateMenuFocusPresentation();
            UpdateAtmosphereMotion();
        }

        private void OnDestroy()
        {
            DestroyRuntimeSprite(softGlowSprite);
            DestroyRuntimeSprite(parchmentSprite);
            DestroyExternalSprite(authoredMenuScrollSprite);
            DestroyExternalSprite(authoredMenuFocusSprite);
            softGlowSprite = null;
            parchmentSprite = null;
            authoredMenuScrollSprite = null;
            authoredMenuFocusSprite = null;
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

            Vector2 canvasSize = CanvasSize();
            if (!Mathf.Approximately(lastWidth, canvasSize.x)
                || !Mathf.Approximately(lastHeight, canvasSize.y)
                || lastSaveExists != saveExists
                || lastDeveloperTestingVisible != devVisible)
            {
                ApplyLayout(saveExists, devVisible);
            }

            continueButton.gameObject.SetActive(TavernMenuRules.ShowContinue(saveExists));
            continueButton.interactable = TavernMenuRules.EnableContinue(saveExists);
            testingButton.gameObject.SetActive(devVisible);
            testingButtonText.text = "Beta Lab";
            continueButtonText.text = "Continue";
            newGameButtonText.text = "New Game";
            menuPanel.gameObject.SetActive(!settingsVisible);
            settingsPanel.gameObject.SetActive(settingsVisible);
            testingPanel.gameObject.SetActive(devVisible && testingVisible);
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
            ConfigureMenuNavigation(saveExists);
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
            scaler.referencePixelsPerUnit = TitleScreenPresentationRules.MenuScrollReferencePixelsPerUnit;
            canvas.gameObject.AddComponent<GraphicRaycaster>();
            Stretch(canvas.GetComponent<RectTransform>());

            Image baseImage = AddImage("Backdrop Base", canvas.transform, Hex("0e1114", 1f));
            Stretch(baseImage.rectTransform);
            if (bindings?.BackdropArt != null)
            {
                Image backdrop = AddImage("Grand Hearth Backdrop", canvas.transform, Color.white);
                backdropRect = backdrop.rectTransform;
                backdrop.sprite = Sprite.Create(bindings.BackdropArt, new Rect(0, 0, bindings.BackdropArt.width, bindings.BackdropArt.height), new Vector2(0.5f, 0.5f), 100f);
                backdrop.preserveAspect = false;
            }

            atmosphereLayer = new GameObject("Grand Hearth Atmosphere", typeof(RectTransform)).GetComponent<RectTransform>();
            atmosphereLayer.SetParent(canvas.transform, false);
            Stretch(atmosphereLayer);
            softGlowSprite = CreateSoftGlowSprite();
            hearthGlow = AddImage("Hearth Bloom", atmosphereLayer, Hex("f07a2f", 0.08f));
            hearthGlow.raycastTarget = false;
            hearthGlow.sprite = softGlowSprite;
            hearthGlow.preserveAspect = true;
            hearthFireboxFlicker = AddImage("Hearth Firebox Flicker", atmosphereLayer, Hex("ff9a45", 0.08f));
            hearthFireboxFlicker.raycastTarget = false;
            hearthFireboxFlicker.sprite = softGlowSprite;
            hearthFireboxFlicker.preserveAspect = true;
            gateGlow = AddImage("Storm Gate Bloom", atmosphereLayer, Hex("6ba9d3", 0.045f));
            gateGlow.raycastTarget = false;
            gateGlow.sprite = softGlowSprite;
            gateGlow.preserveAspect = true;
            for (int i = 0; i < 22; i++)
            {
                Image ember = AddImage(
                    "Grand Hearth Ember " + i,
                    atmosphereLayer,
                    Hex(i % 5 == 0 ? "fff0a1" : i % 2 == 0 ? "f7a54a" : "d85428", 0f));
                ember.raycastTarget = false;
                atmosphereEmbers.Add(ember);
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
            colorGrade = AddImage("Grand Hearth Color Grade", canvas.transform, Hex("030405", 0.16f));
            colorGrade.raycastTarget = false;
            Stretch(colorGrade.rectTransform);
            openingVeil = AddImage("Opening Shadow Veil", canvas.transform, Hex("010203", 0.94f));
            openingVeil.raycastTarget = false;
            Stretch(openingVeil.rectTransform);

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
            ConfigureForgedTitleLayer(ashShadowText, 12, 34);
            ConfigureForgedTitleLayer(ashGlowText, 12, 36);
            ConfigureForgedTitleLayer(ashText, 12, 34);
            ConfigureForgedTitleLayer(titleShadowText, 18, 50);
            ConfigureForgedTitleLayer(titleGlowText, 18, 52);
            ConfigureForgedTitleLayer(titleText, 18, 50);
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
            versionText = AddText("Version", canvas.transform, "", 10, Hex("b7aa90", 1f), TextAnchor.MiddleRight);

            TitleMenuScrollStyle scrollStyle = TitleScreenPresentationRules.MenuScrollStyle;
            authoredMenuScrollSprite = CreateExternalSlicedSprite(
                TitleScreenPresentationRules.SupportsMenuScrollArt(bindings?.MenuScrollArt)
                    ? bindings.MenuScrollArt
                    : null,
                new Rect(
                    0f,
                    0f,
                    TitleScreenPresentationRules.MenuScrollTextureWidth,
                    TitleScreenPresentationRules.MenuScrollTextureHeight),
                TitleScreenPresentationRules.MenuScrollSpriteBorder,
                TitleScreenPresentationRules.MenuScrollPixelsPerUnit,
                "Ashen Road Charter Frame");
            authoredMenuFocusSprite = CreateExternalSlicedSprite(
                TitleScreenPresentationRules.SupportsMenuFocusArt(bindings?.MenuFocusArt)
                    ? bindings.MenuFocusArt
                    : null,
                TitleScreenPresentationRules.MenuFocusSpriteRect,
                TitleScreenPresentationRules.MenuFocusSpriteBorder,
                TitleScreenPresentationRules.MenuFocusPixelsPerUnit,
                "Ashen Road Charter Focus Ribbon");
            bool useAuthoredScroll = authoredMenuScrollSprite != null;
            parchmentSprite = useAuthoredScroll ? null : CreateParchmentSprite();
            menuPanel = new GameObject("Title Menu Scroll", typeof(RectTransform)).GetComponent<RectTransform>();
            menuPanel.SetParent(canvas.transform, false);
            menuCanvasGroup = menuPanel.gameObject.AddComponent<CanvasGroup>();

            Image authoredShadow = AddImage("Authored Charter Shadow", menuPanel, Hex("020100", 0.64f));
            authoredShadow.raycastTarget = false;
            authoredShadow.sprite = authoredMenuScrollSprite;
            authoredShadow.type = useAuthoredScroll ? Image.Type.Sliced : Image.Type.Simple;
            authoredShadow.fillCenter = true;
            authoredShadow.preserveAspect = false;
            authoredShadow.gameObject.SetActive(useAuthoredScroll);
            menuAuthoredScrollShadow = authoredShadow.rectTransform;

            Image authoredFrame = AddImage("Authored Ashen Road Charter", menuPanel, Color.white);
            authoredFrame.raycastTarget = false;
            authoredFrame.sprite = authoredMenuScrollSprite;
            authoredFrame.type = useAuthoredScroll ? Image.Type.Sliced : Image.Type.Simple;
            authoredFrame.fillCenter = true;
            authoredFrame.preserveAspect = false;
            authoredFrame.gameObject.SetActive(useAuthoredScroll);
            menuAuthoredScrollFrame = authoredFrame.rectTransform;

            Image scrollShadow = AddImage("Scroll Shadow", menuPanel, scrollStyle.Shadow);
            scrollShadow.raycastTarget = false;
            menuScrollShadow = scrollShadow.rectTransform;

            Image scrollSheet = AddImage("Aged Parchment Sheet", menuPanel, scrollStyle.Paper);
            scrollSheet.raycastTarget = false;
            scrollSheet.sprite = parchmentSprite;
            menuScrollSheet = scrollSheet.rectTransform;
            Outline sheetOutline = scrollSheet.gameObject.AddComponent<Outline>();
            sheetOutline.effectColor = Alpha(scrollStyle.Edge, 0.90f);
            sheetOutline.effectDistance = new Vector2(1f, -1f);

            Image leftShade = AddImage("Parchment Left Age", menuPanel, Alpha(scrollStyle.Edge, 0.18f));
            leftShade.raycastTarget = false;
            menuScrollLeftShade = leftShade.rectTransform;
            Image rightShade = AddImage("Parchment Right Age", menuPanel, Alpha(scrollStyle.Edge, 0.18f));
            rightShade.raycastTarget = false;
            menuScrollRightShade = rightShade.rectTransform;

            Image topRoll = AddImage("Top Parchment Roll", menuPanel, scrollStyle.Roll);
            topRoll.raycastTarget = false;
            topRoll.sprite = parchmentSprite;
            menuScrollTopRoll = topRoll.rectTransform;
            Outline topRollOutline = topRoll.gameObject.AddComponent<Outline>();
            topRollOutline.effectColor = scrollStyle.Edge;
            topRollOutline.effectDistance = new Vector2(1f, -1f);
            Image topHighlight = AddImage("Top Roll Highlight", menuPanel, Alpha(scrollStyle.SelectionInk, 0.46f));
            topHighlight.raycastTarget = false;
            menuScrollTopHighlight = topHighlight.rectTransform;
            Image topShade = AddImage("Top Roll Shade", menuPanel, Alpha(scrollStyle.Edge, 0.42f));
            topShade.raycastTarget = false;
            menuScrollTopShade = topShade.rectTransform;

            Image bottomRoll = AddImage("Bottom Parchment Roll", menuPanel, scrollStyle.Roll);
            bottomRoll.raycastTarget = false;
            bottomRoll.sprite = parchmentSprite;
            menuScrollBottomRoll = bottomRoll.rectTransform;
            Outline bottomRollOutline = bottomRoll.gameObject.AddComponent<Outline>();
            bottomRollOutline.effectColor = scrollStyle.Edge;
            bottomRollOutline.effectDistance = new Vector2(1f, -1f);
            Image bottomHighlight = AddImage("Bottom Roll Highlight", menuPanel, Alpha(scrollStyle.SelectionInk, 0.36f));
            bottomHighlight.raycastTarget = false;
            menuScrollBottomHighlight = bottomHighlight.rectTransform;
            Image bottomShade = AddImage("Bottom Roll Shade", menuPanel, Alpha(scrollStyle.Edge, 0.52f));
            bottomShade.raycastTarget = false;
            menuScrollBottomShade = bottomShade.rectTransform;

            for (int i = 0; i < 4; i++)
            {
                Image endCap = AddImage("Rolled Parchment End " + i, menuPanel, Alpha(scrollStyle.Edge, 0.70f));
                endCap.raycastTarget = false;
                menuScrollEndCaps.Add(endCap.rectTransform);
            }
            SetProceduralMenuGraphicsActive(!useAuthoredScroll);

            menuTitleText = AddText("Menu Title", menuPanel, "Main Menu", 22, scrollStyle.Ink, TextAnchor.MiddleCenter);
            menuTitleText.font = UiRuntime.TitleFont ?? font;
            menuTitleText.fontStyle = FontStyle.Normal;
            DisableTextShadow(menuTitleText);
            menuRuleGlow = AddImage("Menu Rule Glow", menuPanel, Alpha(scrollStyle.Accent, 0.20f)).rectTransform;
            menuRuleGlow.GetComponent<Image>().raycastTarget = false;
            menuRuleCore = AddImage("Menu Rule Core", menuPanel, Alpha(scrollStyle.Accent, 0.82f)).rectTransform;
            menuRuleCore.GetComponent<Image>().raycastTarget = false;
            continueButton = AddTitleChoiceButton(
                "Continue",
                "Continue",
                TitleMenuChoiceKind.Continue,
                bindings?.Continue,
                true);
            continueButtonText = titleChoices[titleChoices.Count - 1].Label;
            newGameButton = AddTitleChoiceButton(
                "New Game",
                "New Game",
                TitleMenuChoiceKind.NewGame,
                bindings?.NewGame,
                true);
            newGameButtonText = titleChoices[titleChoices.Count - 1].Label;
            settingsButton = AddTitleChoiceButton(
                "Settings",
                "Settings",
                TitleMenuChoiceKind.Settings,
                bindings?.ToggleSettings,
                false);
            quitButton = AddTitleChoiceButton(
                "Exit Game",
                "Exit Game",
                TitleMenuChoiceKind.Exit,
                bindings?.Quit,
                false);
            testingButton = AddTitleChoiceButton(
                "Beta Lab",
                "Beta Lab",
                TitleMenuChoiceKind.BetaLab,
                bindings?.BetaLab,
                false);
            testingButtonText = titleChoices[titleChoices.Count - 1].Label;

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

        private void ApplyLayout(bool saveExists, bool developerTestingVisible = false)
        {
            Vector2 canvasSize = CanvasSize();
            float width = canvasSize.x;
            float height = canvasSize.y;
            lastWidth = width;
            lastHeight = height;
            lastSaveExists = saveExists;
            lastDeveloperTestingVisible = developerTestingVisible;
            lastPhysicalScreenWidth = Screen.width;
            lastPhysicalScreenHeight = Screen.height;
            float artWidth = bindings?.BackdropArt == null ? 16f : bindings.BackdropArt.width;
            float artHeight = bindings?.BackdropArt == null ? 9f : bindings.BackdropArt.height;
            TavernScreenGeometry geometry = TavernScreenLayout.Calculate(
                width,
                height,
                saveExists,
                developerTestingVisible,
                artWidth,
                artHeight);
            SetScreenRect(titlePanel, geometry.Title);
            SetScreenRect(menuPanel, geometry.Menu);
            menuRestPosition = menuPanel.anchoredPosition;
            SetScreenRect(settingsPanel, geometry.Settings);
            SetScreenRect(testingPanel, geometry.Testing);
            SetScreenRect(versionText.rectTransform, new Rect(width - 374f, height - 36f, 350f, 18f));
            TitleBackdropProjection backdropProjection = TitleScreenPresentationRules.ProjectBackdrop(
                width,
                height,
                artWidth,
                artHeight);
            SetScreenRect(backdropRect, backdropProjection.CoverRect);
            SetScreenRect(atmosphereLayer, new Rect(0f, 0f, width, height));
            SetScreenRect(stormLayer, new Rect(0f, 0f, width, height));
            SetLocalRect(
                hearthGlow.rectTransform,
                backdropProjection.ProjectNormalized(new Rect(-0.055f, 0.26f, 0.46f, 0.76f)));
            SetLocalRect(
                hearthFireboxFlicker.rectTransform,
                backdropProjection.ProjectNormalized(new Rect(0.005f, 0.46f, 0.23f, 0.36f)));
            SetLocalRect(
                gateGlow.rectTransform,
                backdropProjection.ProjectNormalized(new Rect(0.49f, 0.08f, 0.37f, 0.82f)));
            IReadOnlyList<Rect> windowRects = TavernScreenLayout.StormWindowRects(
                width,
                height,
                artWidth,
                artHeight);
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
            TavernMenuScrollGeometry scroll = TavernScreenLayout.ScrollGeometry(
                geometry.Menu.width,
                geometry.Menu.height);
            SetLocalRect(
                menuAuthoredScrollShadow,
                new Rect(3f, 5f, Mathf.Max(1f, scroll.Bounds.width - 3f), scroll.Bounds.height - 5f));
            SetLocalRect(menuAuthoredScrollFrame, scroll.Bounds);
            SetLocalRect(
                menuScrollShadow,
                new Rect(3f, 5f, Mathf.Max(1f, scroll.Bounds.width - 3f), scroll.Bounds.height - 5f));
            SetLocalRect(menuScrollSheet, scroll.Sheet);
            SetLocalRect(menuScrollTopRoll, scroll.TopRoll);
            SetLocalRect(menuScrollBottomRoll, scroll.BottomRoll);
            SetLocalRect(
                menuScrollLeftShade,
                new Rect(scroll.Sheet.x + 1f, scroll.Sheet.y + 3f, 4f, scroll.Sheet.height - 6f));
            SetLocalRect(
                menuScrollRightShade,
                new Rect(scroll.Sheet.xMax - 5f, scroll.Sheet.y + 3f, 4f, scroll.Sheet.height - 6f));
            SetLocalRect(
                menuScrollTopHighlight,
                new Rect(scroll.TopRoll.x + 8f, scroll.TopRoll.y + 3f, scroll.TopRoll.width - 16f, 2f));
            SetLocalRect(
                menuScrollTopShade,
                new Rect(scroll.TopRoll.x + 4f, scroll.TopRoll.yMax - 4f, scroll.TopRoll.width - 8f, 3f));
            SetLocalRect(
                menuScrollBottomHighlight,
                new Rect(scroll.BottomRoll.x + 8f, scroll.BottomRoll.y + 4f, scroll.BottomRoll.width - 16f, 2f));
            SetLocalRect(
                menuScrollBottomShade,
                new Rect(scroll.BottomRoll.x + 4f, scroll.BottomRoll.yMax - 4f, scroll.BottomRoll.width - 8f, 3f));
            float endCapWidth = Mathf.Clamp(scroll.TopRoll.height * 0.24f, 4f, 7f);
            SetLocalRect(
                menuScrollEndCaps[0],
                new Rect(scroll.TopRoll.x + 2f, scroll.TopRoll.y + 2f, endCapWidth, scroll.TopRoll.height - 4f));
            SetLocalRect(
                menuScrollEndCaps[1],
                new Rect(scroll.TopRoll.xMax - endCapWidth - 2f, scroll.TopRoll.y + 2f, endCapWidth, scroll.TopRoll.height - 4f));
            SetLocalRect(
                menuScrollEndCaps[2],
                new Rect(scroll.BottomRoll.x + 2f, scroll.BottomRoll.y + 2f, endCapWidth, scroll.BottomRoll.height - 4f));
            SetLocalRect(
                menuScrollEndCaps[3],
                new Rect(scroll.BottomRoll.xMax - endCapWidth - 2f, scroll.BottomRoll.y + 2f, endCapWidth, scroll.BottomRoll.height - 4f));
            SetLocalRect(menuTitleText.rectTransform, scroll.Header);
            SetLocalRect(
                menuRuleGlow,
                new Rect(scroll.Rule.x, scroll.Rule.y - 1f, scroll.Rule.width, 4f));
            SetLocalRect(menuRuleCore, scroll.Rule);

            IReadOnlyList<Rect> buttons = TavernScreenLayout.ButtonRects(saveExists, geometry.Menu.width);
            SetLocalRect(continueButton.GetComponent<RectTransform>(), buttons[0]);
            SetLocalRect(newGameButton.GetComponent<RectTransform>(), buttons[1]);
            SetLocalRect(settingsButton.GetComponent<RectTransform>(), buttons[2]);
            SetLocalRect(quitButton.GetComponent<RectTransform>(), buttons[3]);
            SetLocalRect(
                testingButton.GetComponent<RectTransform>(),
                TavernScreenLayout.TestingButtonRect(
                    saveExists,
                    geometry.Menu.width,
                    developerTestingVisible));
            bool compactMenu = TavernScreenLayout.IsCompactMenu(geometry.Menu.width);
            menuTitleText.text = "Main Menu";
            menuTitleText.resizeTextForBestFit = compactMenu;
            menuTitleText.resizeTextMinSize = 15;
            menuTitleText.resizeTextMaxSize = compactMenu ? 18 : 22;
            foreach (TitleChoiceVisual choice in titleChoices)
            {
                bool hasIcon = choice.Icon.sprite != null;
                choice.IconFrame.gameObject.SetActive(hasIcon);
                choice.Icon.gameObject.SetActive(hasIcon);
                SetLocalRect(
                    choice.IconFrame.rectTransform,
                    compactMenu ? new Rect(17f, 7f, 30f, 30f) : new Rect(23f, 5f, 42f, 42f));
                SetLocalRect(
                    choice.Icon.rectTransform,
                    compactMenu ? new Rect(19f, 9f, 26f, 26f) : new Rect(26f, 8f, 36f, 36f));
                choice.Label.rectTransform.offsetMin = new Vector2(compactMenu ? 47f : 72f, 4f);
                choice.Label.rectTransform.offsetMax = new Vector2(compactMenu ? -2f : -14f, -4f);
                SetLocalRect(
                    choice.Cursor.rectTransform,
                    compactMenu ? new Rect(2f, 6f, 13f, 32f) : new Rect(5f, 7f, 16f, 38f));
                choice.Label.fontSize = compactMenu ? 14 : choice.Hero ? 18 : 17;
                choice.Label.resizeTextForBestFit = false;
                choice.Label.horizontalOverflow = HorizontalWrapMode.Overflow;
                choice.Label.verticalOverflow = VerticalWrapMode.Truncate;
            }

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

        private void EnsureLayoutCurrent()
        {
            if (canvas == null || !canvas.gameObject.activeInHierarchy || bindings == null) return;
            Vector2 canvasSize = CanvasSize();
            bool saveExists = bindings.HasSavedGame != null && bindings.HasSavedGame();
            bool developerTestingVisible = bindings.DeveloperTestingVisible != null && bindings.DeveloperTestingVisible();
            bool physicalSizeChanged = lastPhysicalScreenWidth != Screen.width || lastPhysicalScreenHeight != Screen.height;
            if (Mathf.Approximately(lastWidth, canvasSize.x)
                && Mathf.Approximately(lastHeight, canvasSize.y)
                && lastSaveExists == saveExists
                && lastDeveloperTestingVisible == developerTestingVisible
                && !physicalSizeChanged)
            {
                return;
            }

            ApplyLayout(saveExists, developerTestingVisible);
            Refresh();
        }

        private Vector2 CanvasSize()
        {
            RectTransform root = canvas == null ? null : canvas.GetComponent<RectTransform>();
            if (root == null || root.rect.width <= 0f || root.rect.height <= 0f)
            {
                return new Vector2(Mathf.Max(1f, Screen.width), Mathf.Max(1f, Screen.height));
            }
            return root.rect.size;
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
            text.font = UiRuntime.TitleFont ?? UiRuntime.DialogueEmphasisFont ?? font;
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
            previousTitleElapsed = 0f;
            revealStrikePlayed = false;
            revealChimePlayed = false;
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
            if (TitleScreenPresentationRules.CrossedCue(
                    previousTitleElapsed,
                    elapsed,
                    TitleScreenPresentationRules.RevealStrikeAt,
                    reduced,
                    revealStrikePlayed))
            {
                revealStrikePlayed = true;
                bindings?.PlayTitleCue?.Invoke("impactlow", 0.14f);
            }
            if (TitleScreenPresentationRules.CrossedCue(
                    previousTitleElapsed,
                    elapsed,
                    TitleScreenPresentationRules.RevealChimeAt,
                    reduced,
                    revealChimePlayed))
            {
                revealChimePlayed = true;
                bindings?.PlayTitleCue?.Invoke("uiconfirm", 0.16f);
            }
            previousTitleElapsed = elapsed;
            float ashAlpha = Mathf.Clamp01(frame.FaceAlpha * 1.18f);
            Color ashFaceColor = Color.Lerp(Hex("a44c2a", ashAlpha), Hex("e7c477", ashAlpha), ashAlpha);
            Color faceColor = Color.Lerp(Hex("b9552b", frame.FaceAlpha), Hex("fff0d0", frame.FaceAlpha), frame.FaceAlpha);
            ashText.color = ashFaceColor;
            ashShadowText.color = Hex("090403", frame.ShadowAlpha * 0.92f);
            ashGlowText.color = Hex("ef6328", frame.GlowAlpha * 0.72f);
            titleText.color = faceColor;
            titleShadowText.color = Hex("090403", frame.ShadowAlpha);
            titleGlowText.color = Hex("ef6328", frame.GlowAlpha);
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

        private void UpdateOpeningPresentation()
        {
            if (canvas == null || !canvas.gameObject.activeInHierarchy) return;
            bool reduced = bindings?.ReducedMotion != null && bindings.ReducedMotion();
            float elapsed = reduced
                ? TavernTitleAnimationRules.RevealDuration
                : Mathf.Max(0f, Time.unscaledTime - titleAnimationStartedAt);
            TitleOpeningFrame frame = TitleScreenPresentationRules.Evaluate(elapsed, reduced);
            TitleHearthFlickerFrame hearth = TitleScreenPresentationRules.EvaluateHearthFlicker(elapsed, reduced);

            if (openingVeil != null)
            {
                openingVeil.color = Hex("010203", (1f - frame.BackdropReveal) * 0.94f);
            }
            if (menuCanvasGroup != null)
            {
                menuCanvasGroup.alpha = frame.MenuAlpha;
                bool menuInteractive = TitleScreenPresentationRules.MenuInteractive(frame);
                menuCanvasGroup.interactable = menuInteractive;
                menuCanvasGroup.blocksRaycasts = menuInteractive;
                menuPanel.anchoredPosition = menuRestPosition + new Vector2(0f, -frame.MenuRise);
            }
            if (colorGrade != null)
            {
                colorGrade.color = Hex("030405", frame.VignetteAlpha * 0.14f);
            }
            if (hearthGlow != null)
            {
                hearthGlow.color = Hex("f07a2f", 0.030f + frame.HearthPulse * 0.060f);
            }
            if (hearthFireboxFlicker != null)
            {
                hearthFireboxFlicker.color = Hex("ff9a45", 0.020f + hearth.FireboxGlow * 0.125f);
            }
            if (gateGlow != null)
            {
                float gateBreath = reduced ? 0.034f : 0.028f + Mathf.Sin(elapsed * 0.37f + 1.2f) * 0.010f;
                gateGlow.color = Hex("6ba9d3", Mathf.Max(0.012f, gateBreath));
            }

        }

        private void UpdateAtmosphereMotion()
        {
            if (atmosphereLayer == null || canvas == null || !canvas.gameObject.activeInHierarchy) return;
            bool reduced = bindings?.ReducedMotion != null && bindings.ReducedMotion();
            float now = Time.unscaledTime;
            Vector2 canvasSize = CanvasSize();
            for (int i = 0; i < atmosphereEmbers.Count; i++)
            {
                Image ember = atmosphereEmbers[i];
                if (ember == null) continue;
                bool visible = !reduced;
                if (ember.gameObject.activeSelf != visible) ember.gameObject.SetActive(visible);
                if (!visible) continue;

                float phase = Mathf.Repeat(now * (0.038f + (i % 5) * 0.006f) + i * 0.137f, 1f);
                float lane = Mathf.Repeat(i * 0.283f, 1f);
                float x = canvasSize.x * (0.025f + lane * 0.43f)
                    + Mathf.Sin(now * 0.61f + i * 1.73f) * canvasSize.x * 0.006f;
                float y = canvasSize.y * (0.84f - phase * 0.66f);
                float size = 1.5f + (i % 4) * 0.75f;
                SetLocalRect(ember.rectTransform, new Rect(x, y, size, size));
                float life = Mathf.Sin(phase * Mathf.PI);
                float hearthBias = 1f - lane * 0.48f;
                ember.color = Hex(
                    i % 5 == 0 ? "fff0a1" : i % 2 == 0 ? "f7a54a" : "d85428",
                    life * hearthBias * 0.34f);
            }
        }

        private void UpdateMenuFocusPresentation()
        {
            if (titleChoices.Count == 0 || canvas == null || !canvas.gameObject.activeInHierarchy) return;
            TitleMenuScrollStyle scrollStyle = TitleScreenPresentationRules.MenuScrollStyle;
            bool reduced = bindings?.ReducedMotion != null && bindings.ReducedMotion();
            TitleMenuFocusFrame focus = TitleScreenPresentationRules.EvaluateMenuFocus(Time.unscaledTime, reduced);
            for (int i = 0; i < titleChoices.Count; i++)
            {
                TitleChoiceVisual visual = titleChoices[i];
                if (visual?.Button == null) continue;
                bool selected = visual.Button.gameObject.activeInHierarchy && i == focusedMenuChoice;
                bool enabled = visual.Button.interactable;
                if (visual.FocusPlaque != null)
                {
                    visual.FocusPlaque.color = selected ? Color.white : Color.clear;
                }
                if (visual.Cursor != null)
                {
                    visual.Cursor.color = Alpha(scrollStyle.SelectionInk, selected ? focus.CursorAlpha : 0f);
                    visual.Cursor.rectTransform.localScale = Vector3.one * (selected ? focus.CursorScale : 0.9f);
                }
                if (visual.Icon != null)
                {
                    visual.Icon.color = selected
                        ? new Color(1f, 0.94f, 0.80f, 1f)
                        : new Color(1f, 1f, 1f, enabled ? 0.94f : 0.32f);
                }
                if (visual.IconFrame != null)
                {
                    visual.IconFrame.color = selected
                        ? Alpha(scrollStyle.SelectionInk, 0.16f)
                        : Alpha(scrollStyle.Ink, enabled ? 0.07f : 0.025f);
                }
                if (visual.FocusRule != null)
                {
                    Image ruleImage = visual.FocusRule.GetComponent<Image>();
                    if (ruleImage != null)
                    {
                        ruleImage.color = selected
                            ? Alpha(scrollStyle.SelectionInk, 0.72f)
                            : Alpha(scrollStyle.Accent, 0.18f);
                    }
                }
                if (visual.Label != null)
                {
                    visual.Label.color = selected
                        ? scrollStyle.SelectionInk
                        : Alpha(scrollStyle.Ink, enabled ? 1f : 0.46f);
                }
            }
        }

        internal void FocusMenuChoice(int choiceIndex, bool playCue, bool requestSelection)
        {
            if (choiceIndex < 0 || choiceIndex >= titleChoices.Count) return;
            TitleChoiceVisual visual = titleChoices[choiceIndex];
            if (visual?.Button == null || !visual.Button.gameObject.activeInHierarchy || !visual.Button.interactable) return;
            bool changed = focusedMenuChoice != choiceIndex;
            focusedMenuChoice = choiceIndex;
            if (requestSelection && EventSystem.current != null
                && EventSystem.current.currentSelectedGameObject != visual.Button.gameObject)
            {
                EventSystem.current.SetSelectedGameObject(visual.Button.gameObject);
            }
            if (changed && playCue && Time.unscaledTime - lastMenuFocusCueAt >= 0.08f)
            {
                lastMenuFocusCueAt = Time.unscaledTime;
                bindings?.PlayTitleCue?.Invoke("uitab", 0.18f);
            }
        }

        private void ConfigureMenuNavigation(bool saveExists)
        {
            if (settingsPanel != null && settingsPanel.gameObject.activeSelf)
            {
                if (EventSystem.current != null && closeSettingsButton != null)
                {
                    EventSystem.current.SetSelectedGameObject(closeSettingsButton.gameObject);
                }
                return;
            }
            if (testingPanel != null && testingPanel.gameObject.activeSelf)
            {
                if (EventSystem.current != null && betaLabButton != null)
                {
                    EventSystem.current.SetSelectedGameObject(betaLabButton.gameObject);
                }
                return;
            }

            List<int> active = new List<int>();
            for (int i = 0; i < titleChoices.Count; i++)
            {
                Button button = titleChoices[i]?.Button;
                if (button != null && button.gameObject.activeSelf && button.interactable) active.Add(i);
            }
            if (active.Count == 0) return;
            for (int i = 0; i < active.Count; i++)
            {
                int choiceIndex = active[i];
                Button button = titleChoices[choiceIndex].Button;
                Navigation navigation = button.navigation;
                navigation.mode = Navigation.Mode.Explicit;
                navigation.selectOnUp = titleChoices[active[(i - 1 + active.Count) % active.Count]].Button;
                navigation.selectOnDown = titleChoices[active[(i + 1) % active.Count]].Button;
                button.navigation = navigation;
            }

            if (!active.Contains(focusedMenuChoice))
            {
                int newGamePosition = active.FindIndex(index => titleChoices[index].Kind == TitleMenuChoiceKind.NewGame);
                focusedMenuChoice = saveExists || newGamePosition < 0 ? active[0] : active[newGamePosition];
            }
            if (canvas != null && canvas.gameObject.activeInHierarchy && EventSystem.current != null)
            {
                GameObject selected = EventSystem.current.currentSelectedGameObject;
                bool selectionIsTitleChoice = titleChoices.Exists(choice => choice?.Button != null && choice.Button.gameObject == selected && choice.Button.gameObject.activeInHierarchy);
                if (!selectionIsTitleChoice)
                {
                    EventSystem.current.SetSelectedGameObject(titleChoices[focusedMenuChoice].Button.gameObject);
                }
            }
        }

        private Button AddTitleChoiceButton(
            string name,
            string label,
            TitleMenuChoiceKind kind,
            Action action,
            bool hero)
        {
            TitleMenuScrollStyle scrollStyle = TitleScreenPresentationRules.MenuScrollStyle;
            bool useAuthoredRows = authoredMenuScrollSprite != null && authoredMenuFocusSprite != null;
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            go.transform.SetParent(menuPanel, false);
            Image background = go.GetComponent<Image>();
            background.color = useAuthoredRows ? Color.clear : Color.white;
            Outline outline = go.AddComponent<Outline>();
            outline.effectColor = Alpha(scrollStyle.Edge, hero ? 0.34f : 0.20f);
            outline.effectDistance = new Vector2(1f, -1f);
            outline.useGraphicAlpha = false;
            outline.enabled = !useAuthoredRows;

            Button button = go.GetComponent<Button>();
            button.targetGraphic = background;
            ColorBlock colors = button.colors;
            colors.normalColor = useAuthoredRows ? Color.clear : Alpha(scrollStyle.Ink, hero ? 0.10f : 0.045f);
            colors.highlightedColor = useAuthoredRows ? Color.clear : Alpha(scrollStyle.Selection, 0.96f);
            colors.selectedColor = colors.highlightedColor;
            colors.pressedColor = useAuthoredRows ? Alpha(scrollStyle.Selection, 0.24f) : scrollStyle.Edge;
            colors.disabledColor = useAuthoredRows ? Alpha(scrollStyle.Ink, 0.025f) : colors.disabledColor;
            colors.fadeDuration = 0.10f;
            button.colors = colors;
            if (action != null) button.onClick.AddListener(() => action());

            Image focusPlaque = AddImage("Focused Charter Ribbon", go.transform, Color.clear);
            focusPlaque.raycastTarget = false;
            focusPlaque.sprite = authoredMenuFocusSprite;
            focusPlaque.type = useAuthoredRows ? Image.Type.Sliced : Image.Type.Simple;
            focusPlaque.fillCenter = true;
            focusPlaque.preserveAspect = false;
            focusPlaque.gameObject.SetActive(useAuthoredRows);
            Stretch(focusPlaque.rectTransform, -4f, -2f);

            Text cursor = AddText("Forge Cursor", go.transform, "\u25C6", 17, Alpha(scrollStyle.SelectionInk, 0f), TextAnchor.MiddleCenter);
            cursor.font = UiRuntime.TitleFont ?? font;
            cursor.raycastTarget = false;
            DisableTextShadow(cursor);
            SetLocalRect(cursor.rectTransform, new Rect(5f, 7f, 16f, 38f));

            Image iconFrame = AddImage(
                "Relic Icon Backplate",
                go.transform,
                useAuthoredRows ? Alpha(scrollStyle.Ink, 0.07f) : Alpha(scrollStyle.Roll, 0.82f));
            iconFrame.raycastTarget = false;
            if (useAuthoredRows)
            {
                iconFrame.sprite = softGlowSprite;
                iconFrame.preserveAspect = true;
            }
            SetLocalRect(iconFrame.rectTransform, new Rect(24f, 6f, 40f, 40f));
            Outline iconOutline = iconFrame.gameObject.AddComponent<Outline>();
            iconOutline.effectColor = Alpha(scrollStyle.Edge, 0.58f);
            iconOutline.effectDistance = new Vector2(1f, -1f);
            iconOutline.enabled = !useAuthoredRows;

            Image icon = AddImage("Relic Icon", go.transform, Color.white);
            icon.raycastTarget = false;
            bool dedicatedIcons = TitleScreenPresentationRules.SupportsMenuIconArt(bindings?.MenuIconAtlas);
            int iconIndex = dedicatedIcons
                ? TitleScreenPresentationRules.MenuIconIndex(kind)
                : TitleScreenPresentationRules.LegacyMenuIconIndex(kind);
            if (bindings?.MenuIconAtlas != null && iconIndex >= 0)
            {
                int columns = TitleScreenPresentationRules.MenuIconColumns;
                int rows = dedicatedIcons ? TitleScreenPresentationRules.MenuIconRows : 4;
                float cellWidth = bindings.MenuIconAtlas.width / (float)columns;
                float cellHeight = bindings.MenuIconAtlas.height / (float)rows;
                int column = iconIndex % columns;
                int row = iconIndex / columns;
                icon.sprite = UiRuntime.AtlasSprite(
                    bindings.MenuIconAtlas,
                    new Rect(column * cellWidth, row * cellHeight, cellWidth, cellHeight));
                icon.preserveAspect = true;
            }
            SetLocalRect(icon.rectTransform, new Rect(27f, 7f, 36f, 38f));

            Text text = AddText("Label", go.transform, label, hero ? 18 : 17, scrollStyle.Ink, TextAnchor.MiddleLeft);
            text.font = UiRuntime.DialogueEmphasisFont ?? font;
            text.fontStyle = FontStyle.Normal;
            text.raycastTarget = false;
            DisableTextShadow(text);
            text.rectTransform.anchorMin = Vector2.zero;
            text.rectTransform.anchorMax = Vector2.one;
            text.rectTransform.offsetMin = new Vector2(72f, 4f);
            text.rectTransform.offsetMax = new Vector2(-14f, -4f);

            Image focusRule = AddImage("Focused Forge Rule", go.transform, Alpha(scrollStyle.Accent, 0.18f));
            focusRule.raycastTarget = false;
            RectTransform focusRuleRect = focusRule.rectTransform;
            focusRuleRect.anchorMin = new Vector2(0f, 0f);
            focusRuleRect.anchorMax = new Vector2(1f, 0f);
            focusRuleRect.pivot = new Vector2(0.5f, 0f);
            focusRuleRect.offsetMin = new Vector2(30f, 3f);
            focusRuleRect.offsetMax = new Vector2(-12f, 5f);

            int choiceIndex = titleChoices.Count;
            TavernMenuFocusRelay relay = go.AddComponent<TavernMenuFocusRelay>();
            relay.Owner = this;
            relay.ChoiceIndex = choiceIndex;
            titleChoices.Add(new TitleChoiceVisual
            {
                Kind = kind,
                Hero = hero,
                Button = button,
                FocusPlaque = focusPlaque,
                IconFrame = iconFrame,
                Icon = icon,
                Cursor = cursor,
                FocusRule = focusRuleRect,
                Label = text
            });
            return button;
        }

        private void SetProceduralMenuGraphicsActive(bool active)
        {
            SetActive(menuScrollShadow, active);
            SetActive(menuScrollSheet, active);
            SetActive(menuScrollTopRoll, active);
            SetActive(menuScrollBottomRoll, active);
            SetActive(menuScrollTopHighlight, active);
            SetActive(menuScrollTopShade, active);
            SetActive(menuScrollBottomHighlight, active);
            SetActive(menuScrollBottomShade, active);
            SetActive(menuScrollLeftShade, active);
            SetActive(menuScrollRightShade, active);
            foreach (RectTransform endCap in menuScrollEndCaps) SetActive(endCap, active);
        }

        private static void SetActive(RectTransform rect, bool active)
        {
            if (rect != null) rect.gameObject.SetActive(active);
        }

        private static Sprite CreateExternalSlicedSprite(
            Texture2D texture,
            Rect sourceRect,
            Vector4 border,
            float pixelsPerUnit,
            string spriteName)
        {
            if (texture == null
                || sourceRect.width <= border.x + border.z
                || sourceRect.height <= border.y + border.w
                || sourceRect.xMin < 0f
                || sourceRect.yMin < 0f
                || sourceRect.xMax > texture.width
                || sourceRect.yMax > texture.height)
            {
                return null;
            }

            texture.filterMode = FilterMode.Bilinear;
            texture.wrapMode = TextureWrapMode.Clamp;
            Sprite sprite = Sprite.Create(
                texture,
                sourceRect,
                new Vector2(0.5f, 0.5f),
                Mathf.Max(1f, pixelsPerUnit),
                0,
                SpriteMeshType.FullRect,
                border);
            sprite.name = spriteName;
            sprite.hideFlags = HideFlags.HideAndDontSave;
            return sprite;
        }

        private static Sprite CreateSoftGlowSprite()
        {
            const int size = 64;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                name = "Grand Hearth Soft Glow",
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.HideAndDontSave
            };
            Color[] pixels = new Color[size * size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float nx = (x + 0.5f) / size * 2f - 1f;
                    float ny = (y + 0.5f) / size * 2f - 1f;
                    float distance = Mathf.Sqrt(nx * nx + ny * ny);
                    float alpha = Mathf.Pow(Mathf.Clamp01(1f - distance), 2.25f);
                    pixels[y * size + x] = new Color(1f, 1f, 1f, alpha);
                }
            }
            texture.SetPixels(pixels);
            texture.Apply(false, true);
            Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
            sprite.name = "Grand Hearth Soft Glow";
            sprite.hideFlags = HideFlags.HideAndDontSave;
            return sprite;
        }

        private static Sprite CreateParchmentSprite()
        {
            const int size = 192;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                name = "Title Menu Parchment Grain",
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.HideAndDontSave
            };
            Color[] pixels = new Color[size * size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float broad = Mathf.PerlinNoise((x + 17f) * 0.071f, (y + 29f) * 0.071f);
                    float grain = Mathf.PerlinNoise((x + 43f) * 0.29f, (y + 7f) * 0.31f);
                    float fiber = Mathf.Sin(y * 0.83f + Mathf.Sin(x * 0.17f) * 1.9f) * 0.012f;
                    float brightness = Mathf.Clamp(0.83f + broad * 0.13f + grain * 0.045f + fiber, 0.78f, 1f);
                    pixels[y * size + x] = new Color(
                        brightness,
                        brightness * 0.985f,
                        brightness * 0.925f,
                        1f);
                }
            }
            texture.SetPixels(pixels);
            texture.Apply(false, true);
            Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
            sprite.name = "Title Menu Parchment Grain";
            sprite.hideFlags = HideFlags.HideAndDontSave;
            return sprite;
        }

        private void DestroyRuntimeSprite(Sprite sprite)
        {
            if (sprite == null) return;
            Texture2D texture = sprite.texture;
            if (Application.isPlaying)
            {
                Destroy(sprite);
                if (texture != null) Destroy(texture);
            }
            else
            {
                DestroyImmediate(sprite);
                if (texture != null) DestroyImmediate(texture);
            }
        }

        private void DestroyExternalSprite(Sprite sprite)
        {
            if (sprite == null) return;
            if (Application.isPlaying) Destroy(sprite);
            else DestroyImmediate(sprite);
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

        private static Color Alpha(Color color, float alpha)
        {
            color.a = Mathf.Clamp01(alpha);
            return color;
        }

        private static void EnsureEventSystem()
        {
            UiRuntime.EnsureEventSystemReady();
        }
    }
}
