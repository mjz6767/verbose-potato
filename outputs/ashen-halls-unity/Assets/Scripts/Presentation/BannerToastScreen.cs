using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AshenHalls
{
    public sealed class BannerToastView
    {
        public bool Visible;
        public string Text;
        public string Subtitle;
        public string AccentHex;
        public Texture2D IconTexture;
        public Rect IconSource;
        public string Sigil;
        public string Outcome;
        public bool PowerCue;
        public int Intensity;
        public float RemainingSeconds;
        public float TotalSeconds;
        public float ImpactSeconds;
        public bool ReducedMotion;
    }

    public sealed class BannerToastBindings
    {
        public Func<BannerToastView> View;
    }

    public readonly struct BannerToastGeometry
    {
        public readonly Rect Panel;
        public readonly Rect Text;
        public readonly Rect Subtitle;
        public readonly Rect Outcome;
        public readonly Rect Icon;
        public readonly Rect Phase;
        public readonly Rect Accent;

        public BannerToastGeometry(Rect panel, Rect text, Rect subtitle, Rect outcome, Rect icon, Rect phase, Rect accent)
        {
            Panel = panel;
            Text = text;
            Subtitle = subtitle;
            Outcome = outcome;
            Icon = icon;
            Phase = phase;
            Accent = accent;
        }

        public bool Fits(float width, float height)
        {
            return FitsScreen(Panel, width, height)
                && FitsLocal(Text, Panel)
                && FitsLocal(Subtitle, Panel)
                && FitsLocal(Outcome, Panel)
                && FitsLocal(Icon, Panel)
                && FitsLocal(Phase, Panel)
                && FitsLocal(Accent, Panel);
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

    public static class BannerToastLayout
    {
        public static BannerToastGeometry Calculate(float width, float height)
        {
            return Calculate(width, height, true);
        }

        public static BannerToastGeometry Calculate(float width, float height, bool powerCue)
        {
            if (!powerCue)
            {
                float compactW = Mathf.Clamp(width * 0.28f, 340f, 520f);
                float compactH = 38f;
                Rect compactPanel = new Rect((width - compactW) * 0.5f, 7f, compactW, compactH);
                Rect compactAccent = new Rect(0f, compactH - 3f, compactW, 3f);
                Rect compactText = new Rect(14f, 1f, compactW - 28f, compactH - 6f);
                return new BannerToastGeometry(compactPanel, compactText, Rect.zero, Rect.zero, Rect.zero, Rect.zero, compactAccent);
            }

            float panelW = Mathf.Clamp(width * 0.42f, 500f, 720f);
            float panelH = 64f;
            float y = Mathf.Clamp(height * 0.01f, 6f, 8f);
            Rect panel = new Rect((width - panelW) * 0.5f, y, panelW, panelH);
            Rect accent = new Rect(0f, panelH - 4f, panelW, 4f);
            Rect icon = new Rect(7f, 7f, 48f, 48f);
            Rect phase = new Rect(panelW - 146f, 4f, 134f, 19f);
            Rect text = new Rect(66f, 2f, panelW - 224f, 23f);
            Rect subtitle = new Rect(66f, 24f, panelW - 78f, 15f);
            Rect outcome = new Rect(66f, 40f, panelW - 78f, 16f);
            return new BannerToastGeometry(panel, text, subtitle, outcome, icon, phase, accent);
        }

        public static float BannerProgress(BannerToastView view)
        {
            if (view == null || view.TotalSeconds <= 0f) return 1f;
            return 1f - Mathf.Clamp01(view.RemainingSeconds / view.TotalSeconds);
        }

        public static string PowerPhaseLabel(BannerToastView view)
        {
            float total = Mathf.Max(0.01f, view == null ? 0f : view.TotalSeconds);
            float elapsed = total - Mathf.Clamp(view == null ? 0f : view.RemainingSeconds, 0f, total);
            float impactAt = Mathf.Clamp(view == null ? 0f : view.ImpactSeconds, 0f, total);
            string phaseLabel;
            if (view != null && view.ReducedMotion)
            {
                phaseLabel = elapsed < Mathf.Min(0.18f, total * 0.45f) ? "IMPACT" : "AFTERMATH";
            }
            else
            {
                float invocationUntil = Mathf.Max(0.025f, impactAt * 0.52f);
                float aftermathAt = Mathf.Min(total, impactAt + Mathf.Clamp(total * 0.12f, 0.10f, 0.18f));
                phaseLabel = elapsed < invocationUntil
                    ? "INVOCATION"
                    : elapsed < impactAt
                        ? "RELEASE"
                        : elapsed < aftermathAt
                            ? "IMPACT"
                            : "AFTERMATH";
            }

            int intensity = Mathf.Clamp(view == null ? 1 : view.Intensity, 1, 3);
            string powerLabel = intensity >= 3 ? "EPIC" : intensity == 2 ? "GREATER" : "POWER";
            return $"{powerLabel} / {phaseLabel}";
        }
    }

    public sealed class BannerToastScreen : MonoBehaviour
    {
        private BannerToastBindings bindings;
        private Canvas canvas;
        private CanvasGroup canvasGroup;
        private RectTransform panel;
        private RectTransform accentTrack;
        private RectTransform accent;
        private RectTransform iconFrame;
        private Image icon;
        private Text sigil;
        private Text text;
        private Text subtitle;
        private Text outcome;
        private Text phase;
        private Font font;
        private float lastWidth = -1f;
        private float lastHeight = -1f;
        private bool? lastPowerCue;

        public void Bind(BannerToastBindings toastBindings)
        {
            bindings = toastBindings;
            Build();
            Refresh();
        }

        public void SetVisible(bool visible)
        {
            if (canvas != null && canvas.gameObject.activeSelf != visible)
            {
                canvas.gameObject.SetActive(visible);
            }
        }

        public void Refresh()
        {
            if (bindings == null || canvas == null) return;
            BannerToastView view = bindings.View == null ? null : bindings.View();
            if (view == null || !view.Visible || string.IsNullOrWhiteSpace(view.Text))
            {
                SetVisible(false);
                return;
            }

            if (!Mathf.Approximately(lastWidth, Screen.width) || !Mathf.Approximately(lastHeight, Screen.height))
            {
                ApplyLayout(view.PowerCue);
            }
            else if (!lastPowerCue.HasValue || lastPowerCue.Value != view.PowerCue)
            {
                ApplyLayout(view.PowerCue);
            }

            text.text = view.Text;
            subtitle.text = view.Subtitle ?? "";
            outcome.text = view.Outcome ?? "";
            Color cueAccent = ParseColor(view.AccentHex, Hex("d7a84e", 1f));
            accent.GetComponent<Image>().color = cueAccent;
            accentTrack.GetComponent<Image>().color = cueAccent.WithAlpha(0.22f);
            panel.GetComponent<Outline>().effectColor = cueAccent.WithAlpha(0.88f);
            text.color = view.PowerCue ? Hex("f3ead7", 1f) : cueAccent;
            text.fontSize = view.PowerCue ? 17 + Mathf.Clamp(view.Intensity, 1, 3) : 16;
            text.alignment = view.PowerCue ? TextAnchor.MiddleLeft : TextAnchor.MiddleCenter;
            subtitle.color = cueAccent;
            bool showIdentity = view.PowerCue;
            iconFrame.gameObject.SetActive(showIdentity);
            accentTrack.gameObject.SetActive(showIdentity);
            phase.gameObject.SetActive(showIdentity);
            subtitle.gameObject.SetActive(showIdentity && !string.IsNullOrWhiteSpace(subtitle.text));
            outcome.gameObject.SetActive(showIdentity && !string.IsNullOrWhiteSpace(outcome.text));
            if (showIdentity)
            {
                Sprite sprite = UiRuntime.AtlasSprite(view.IconTexture, view.IconSource);
                icon.sprite = sprite;
                icon.enabled = sprite != null;
                sigil.text = view.Sigil ?? "";
                sigil.color = cueAccent;
                sigil.alignment = sprite == null ? TextAnchor.MiddleCenter : TextAnchor.LowerRight;
                sigil.fontSize = sprite == null ? 12 : 9;
                outcome.color = Color.Lerp(cueAccent, Hex("f3ead7", 1f), 0.56f);
                phase.text = BannerToastLayout.PowerPhaseLabel(view);
                phase.fontSize = 9 + Mathf.Clamp(view.Intensity, 1, 3);
                phase.color = Color.Lerp(cueAccent, Hex("f3ead7", 1f), 0.32f + Mathf.Clamp(view.Intensity, 1, 3) * 0.12f);
                float progress = BannerToastLayout.BannerProgress(view);
                SetLocalRect(accent, new Rect(0f, panel.rect.height - 4f, Mathf.Max(2f, panel.rect.width * progress), 4f));
            }
            else
            {
                SetLocalRect(text.rectTransform, new Rect(14f, 1f, panel.rect.width - 28f, panel.rect.height - 6f));
            }
            canvasGroup.alpha = BannerAlpha(view);
            SetVisible(true);
        }

        private static float BannerAlpha(BannerToastView view)
        {
            if (view.ReducedMotion) return 1f;
            if (view.TotalSeconds <= 0f) return 1f;
            float shown = Mathf.Clamp01(view.RemainingSeconds / view.TotalSeconds);
            return Mathf.Clamp01(Mathf.InverseLerp(0f, 0.18f, shown));
        }

        private void Build()
        {
            if (canvas != null) return;
            EnsureEventSystem();
            font = UiRuntime.DefaultFont;

            canvas = UiRuntime.CreateOwnedRootCanvas(this, "Banner Toast Canvas");
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 45;
            CanvasScaler scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            canvasGroup = canvas.gameObject.AddComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            Stretch(canvas.GetComponent<RectTransform>());

            panel = AddPanel("Banner Toast", canvas.transform, Hex("171c20", 0.96f), Hex("d7a84e", 0.88f));
            accentTrack = AddImage("Accent Track", panel, Hex("d7a84e", 0.22f)).rectTransform;
            accent = AddImage("Accent", panel, Hex("d7a84e", 0.92f)).rectTransform;
            iconFrame = AddPanel("Power Icon Frame", panel, Hex("050708", 0.94f), Hex("d7a84e", 0.82f));
            iconFrame.GetComponent<Image>().raycastTarget = false;
            icon = AddImage("Power Icon", iconFrame, Color.white);
            icon.preserveAspect = true;
            icon.raycastTarget = false;
            sigil = AddText("Power Sigil", iconFrame, "", 9, Hex("d7a84e", 1f), TextAnchor.LowerRight);
            sigil.raycastTarget = false;
            text = AddText("Text", panel, "", 18, Hex("f3ead7", 1f), TextAnchor.MiddleCenter);
            subtitle = AddText("Subtitle", panel, "", 10, Hex("d7a84e", 1f), TextAnchor.MiddleLeft);
            subtitle.fontStyle = FontStyle.Normal;
            outcome = AddText("Outcome", panel, "", 10, Hex("f3ead7", 1f), TextAnchor.MiddleLeft);
            phase = AddText("Power Phase", panel, "", 10, Hex("f3ead7", 1f), TextAnchor.MiddleRight);
            phase.horizontalOverflow = HorizontalWrapMode.Overflow;
        }

        private void ApplyLayout(bool powerCue)
        {
            lastWidth = Screen.width;
            lastHeight = Screen.height;
            lastPowerCue = powerCue;
            BannerToastGeometry geometry = BannerToastLayout.Calculate(Screen.width, Screen.height, powerCue);
            SetScreenRect(panel, geometry.Panel);
            SetLocalRect(text.rectTransform, geometry.Text);
            SetLocalRect(subtitle.rectTransform, geometry.Subtitle);
            SetLocalRect(outcome.rectTransform, geometry.Outcome);
            SetLocalRect(iconFrame, geometry.Icon);
            SetLocalRect(phase.rectTransform, geometry.Phase);
            Stretch(icon.rectTransform, 3f);
            Stretch(sigil.rectTransform, 4f);
            SetLocalRect(accentTrack, geometry.Accent);
            SetLocalRect(accent, geometry.Accent);
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

        private Text AddText(string name, Transform parent, string value, int size, Color color, TextAnchor anchor)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            go.transform.SetParent(parent, false);
            Text label = go.GetComponent<Text>();
            label.font = font;
            label.text = value;
            label.fontSize = size;
            label.fontStyle = FontStyle.Bold;
            label.color = color;
            label.alignment = anchor;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Truncate;
            Shadow shadow = go.AddComponent<Shadow>();
            shadow.effectColor = Hex("020304", 0.72f);
            shadow.effectDistance = new Vector2(1f, -1f);
            return label;
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

        private static void Stretch(RectTransform rect, float padding = 0f)
        {
            if (rect == null) return;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(padding, padding);
            rect.offsetMax = new Vector2(-padding, -padding);
        }

        private static Color ParseColor(string value, Color fallback)
        {
            if (!string.IsNullOrWhiteSpace(value) && ColorUtility.TryParseHtmlString("#" + value.TrimStart('#'), out Color color)) return color;
            return fallback;
        }

        private static Color Hex(string value, float alpha)
        {
            ColorUtility.TryParseHtmlString("#" + value, out Color color);
            color.a = alpha;
            return color;
        }

        private static void EnsureEventSystem()
        {
            UiRuntime.EnsureEventSystemReady();
        }
    }
}
