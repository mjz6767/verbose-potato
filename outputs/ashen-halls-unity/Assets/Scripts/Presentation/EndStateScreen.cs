using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AshenHalls
{
    public sealed class EndStateView
    {
        public bool Victory;
        public bool ShowTavernButton;
        public bool ShowBetaLabButton;
        public string Title;
        public string Subtitle;
        public string SummaryTitle;
        public string SummaryLine;
        public string[] PartyRows;
        public string RouteTitle;
        public string[] RouteRows;
        public string Footer;
    }

    public sealed class EndStateScreenBindings
    {
        public Func<EndStateView> View;
        public Action NewParty;
        public Action ReturnToTavern;
        public Action BetaLab;
    }

    public readonly struct EndStateGeometry
    {
        public readonly Rect Backdrop;
        public readonly Rect Panel;
        public readonly Rect Hero;
        public readonly Rect Summary;
        public readonly Rect Route;
        public readonly Rect PrimaryButton;
        public readonly Rect SecondaryButton;
        public readonly Rect TertiaryButton;
        public readonly Rect Footer;

        public EndStateGeometry(
            Rect backdrop,
            Rect panel,
            Rect hero,
            Rect summary,
            Rect route,
            Rect primaryButton,
            Rect secondaryButton,
            Rect tertiaryButton,
            Rect footer)
        {
            Backdrop = backdrop;
            Panel = panel;
            Hero = hero;
            Summary = summary;
            Route = route;
            PrimaryButton = primaryButton;
            SecondaryButton = secondaryButton;
            TertiaryButton = tertiaryButton;
            Footer = footer;
        }

        public bool Fits(float width, float height)
        {
            return FitsRect(Backdrop, width, height)
                && FitsRect(Panel, width, height)
                && FitsLocal(Hero, Panel)
                && FitsLocal(Summary, Panel)
                && FitsLocal(Route, Panel)
                && FitsLocal(PrimaryButton, Panel)
                && FitsLocal(SecondaryButton, Panel)
                && FitsLocal(TertiaryButton, Panel)
                && FitsLocal(Footer, Panel)
                && Summary.height >= 160f
                && Route.height >= 160f;
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

    public static class EndStateScreenLayout
    {
        public static EndStateGeometry Calculate(float width, float height)
        {
            float panelW = Mathf.Min(Mathf.Max(900f, width * 0.72f), width - 64f);
            float panelH = Mathf.Min(Mathf.Max(520f, height * 0.72f), height - 72f);
            Rect backdrop = new Rect(0f, 0f, width, height);
            Rect panel = new Rect((width - panelW) * 0.5f, (height - panelH) * 0.5f, panelW, panelH);
            Rect hero = new Rect(30f, 28f, panelW - 60f, 146f);
            float columnW = (panelW - 76f) * 0.5f;
            float columnH = panelH - 286f;
            Rect summary = new Rect(30f, 196f, columnW, columnH);
            Rect route = new Rect(summary.xMax + 16f, 196f, columnW, columnH);
            float buttonY = panelH - 72f;
            Rect primary = new Rect(30f, buttonY, 164f, 42f);
            Rect secondary = new Rect(208f, buttonY, 150f, 42f);
            Rect tertiary = new Rect(372f, buttonY, 150f, 42f);
            Rect footer = new Rect(540f, buttonY + 2f, panelW - 570f, 38f);
            return new EndStateGeometry(backdrop, panel, hero, summary, route, primary, secondary, tertiary, footer);
        }
    }

    public static class EndStateContent
    {
        public static EndStateView BuildVictory(
            string homeTownName,
            int living,
            int partyCount,
            int averageLevel,
            int gold,
            int depth,
            string[] partyRows,
            bool showBetaLab)
        {
            return new EndStateView
            {
                Victory = true,
                ShowTavernButton = true,
                ShowBetaLabButton = showBetaLab,
                Title = "The Old Road Is Sealed",
                Subtitle = "Vhal Rakh's meteor crown breaks above the ritual heart. " + homeTownName + " has one more dawn.",
                SummaryTitle = "Party Ledger",
                SummaryLine = $"Survivors {living}/{partyCount} / Avg level {Mathf.Max(1, averageLevel)} / Gold {gold} / Depth {depth}",
                PartyRows = partyRows ?? Array.Empty<string>(),
                RouteTitle = "Beta Route Complete",
                RouteRows = new[]
                {
                    "I  Midgaard Cisterns",
                    "II Kobold Smoke",
                    "III Bone Road",
                    "IV Glass and Ash",
                    "V  Red Gate",
                    "VI Meteor Crown"
                },
                Footer = "Next passes can turn this scaffold into hand-authored dungeons, NPC quests, and multi-phase boss rules."
            };
        }

        public static EndStateView BuildDefeat(string homeTownName, string[] partyRows)
        {
            return new EndStateView
            {
                Victory = false,
                ShowTavernButton = true,
                ShowBetaLabButton = false,
                Title = "The Party Has Fallen",
                Subtitle = "A new oath may yet be sworn. The old road waits beyond " + homeTownName + ".",
                SummaryTitle = "Final Ledger",
                SummaryLine = "No one remains standing.",
                PartyRows = partyRows ?? Array.Empty<string>(),
                RouteTitle = "What Carries Forward",
                RouteRows = new[]
                {
                    "Return to the Tavern and Continue from the last checkpoint.",
                    "Try a new party mix.",
                    "Guard before enemy pressure peaks.",
                    "Use elixirs before the final collapse.",
                    "Rangers should keep distance; casters can shape terrain."
                },
                Footer = "Tavern offers the last checkpoint or a fresh party."
            };
        }
    }

    public sealed class EndStateScreen : MonoBehaviour
    {
        private EndStateScreenBindings bindings;
        private Canvas canvas;
        private RectTransform backdrop;
        private RectTransform panel;
        private RectTransform heroPanel;
        private RectTransform summaryPanel;
        private RectTransform routePanel;
        private Text titleText;
        private Text subtitleText;
        private Text summaryTitleText;
        private Text summaryLineText;
        private Text partyText;
        private Text routeTitleText;
        private Text routeText;
        private Text footerText;
        private Button newPartyButton;
        private Button tavernButton;
        private Button betaLabButton;
        private Font font;
        private float lastWidth = -1f;
        private float lastHeight = -1f;

        public void Bind(EndStateScreenBindings screenBindings)
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
            }
        }

        public void Refresh()
        {
            if (bindings == null || canvas == null) return;
            EndStateView view = bindings.View == null ? null : bindings.View();
            if (view == null) return;

            if (!Mathf.Approximately(lastWidth, Screen.width) || !Mathf.Approximately(lastHeight, Screen.height))
            {
                ApplyLayout();
            }

            titleText.text = string.IsNullOrWhiteSpace(view.Title) ? (view.Victory ? "Victory" : "Defeat") : view.Title;
            subtitleText.text = view.Subtitle ?? "";
            summaryTitleText.text = view.SummaryTitle ?? "Ledger";
            summaryLineText.text = view.SummaryLine ?? "";
            partyText.text = FormatRows(view.PartyRows);
            routeTitleText.text = view.RouteTitle ?? "";
            routeText.text = FormatRows(view.RouteRows);
            footerText.text = view.Footer ?? "";
            tavernButton.gameObject.SetActive(view.ShowTavernButton);
            betaLabButton.gameObject.SetActive(view.ShowBetaLabButton);

            Color accent = view.Victory ? Hex("d7a84e", 0.92f) : Hex("d2545f", 0.92f);
            heroPanel.GetComponent<Outline>().effectColor = accent;
            panel.GetComponent<Outline>().effectColor = accent;
            summaryPanel.GetComponent<Outline>().effectColor = view.Victory ? Hex("58b7a5", 0.72f) : Hex("d2545f", 0.66f);
            routePanel.GetComponent<Outline>().effectColor = view.Victory ? Hex("8d6dcc", 0.72f) : Hex("d7a84e", 0.64f);
        }

        private static string FormatRows(string[] rows)
        {
            string[] clean = rows == null
                ? Array.Empty<string>()
                : rows.Where(row => !string.IsNullOrWhiteSpace(row)).ToArray();
            return clean.Length == 0 ? "" : string.Join("\n", clean.Select(row => "- " + row));
        }

        private void Build()
        {
            EnsureEventSystem();
            font = UiRuntime.DefaultFont;
            canvas = UiRuntime.CreateOwnedRootCanvas(this, "End State Canvas");
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            CanvasScaler scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            canvas.gameObject.AddComponent<GraphicRaycaster>();
            Stretch(canvas.GetComponent<RectTransform>());

            backdrop = AddImage("Backdrop", canvas.transform, Hex("030405", 1f)).rectTransform;
            panel = AddPanel("End State Panel", canvas.transform, Hex("10161b", 0.98f), Hex("d7a84e", 0.9f));
            heroPanel = AddPanel("Hero", panel, Hex("080b0d", 0.92f), Hex("d7a84e", 0.9f));
            summaryPanel = AddPanel("Summary", panel, Hex("080b0d", 0.86f), Hex("58b7a5", 0.72f));
            routePanel = AddPanel("Route", panel, Hex("080b0d", 0.86f), Hex("8d6dcc", 0.72f));

            titleText = AddText("Title", heroPanel, "", 28, Hex("f3ead7", 1f), TextAnchor.MiddleLeft);
            subtitleText = AddText("Subtitle", heroPanel, "", 13, Hex("b7aa90", 1f), TextAnchor.UpperLeft);
            summaryTitleText = AddText("Summary Title", summaryPanel, "", 18, Hex("d7a84e", 1f), TextAnchor.MiddleLeft);
            summaryLineText = AddText("Summary Line", summaryPanel, "", 12, Hex("58b7a5", 1f), TextAnchor.MiddleLeft);
            partyText = AddText("Party Rows", summaryPanel, "", 12, Hex("f3ead7", 1f), TextAnchor.UpperLeft);
            routeTitleText = AddText("Route Title", routePanel, "", 18, Hex("d7a84e", 1f), TextAnchor.MiddleLeft);
            routeText = AddText("Route Rows", routePanel, "", 12, Hex("f3ead7", 1f), TextAnchor.UpperLeft);
            footerText = AddText("Footer", panel, "", 10, Hex("b7aa90", 1f), TextAnchor.MiddleLeft);

            newPartyButton = AddButton("New Party", panel, "New Party", () => bindings?.NewParty?.Invoke(), true);
            tavernButton = AddButton("Tavern", panel, "Tavern", () => bindings?.ReturnToTavern?.Invoke(), false);
            betaLabButton = AddButton("Beta Lab", panel, "Beta Lab", () => bindings?.BetaLab?.Invoke(), false);
        }

        private void ApplyLayout()
        {
            lastWidth = Screen.width;
            lastHeight = Screen.height;
            EndStateGeometry geometry = EndStateScreenLayout.Calculate(Screen.width, Screen.height);
            SetScreenRect(backdrop, geometry.Backdrop);
            SetScreenRect(panel, geometry.Panel);
            SetLocalRect(heroPanel, geometry.Hero);
            SetLocalRect(summaryPanel, geometry.Summary);
            SetLocalRect(routePanel, geometry.Route);
            SetLocalRect(newPartyButton.GetComponent<RectTransform>(), geometry.PrimaryButton);
            SetLocalRect(tavernButton.GetComponent<RectTransform>(), geometry.SecondaryButton);
            SetLocalRect(betaLabButton.GetComponent<RectTransform>(), geometry.TertiaryButton);
            SetLocalRect(footerText.rectTransform, geometry.Footer);

            SetLocalRect(titleText.rectTransform, new Rect(24f, 18f, geometry.Hero.width - 48f, 36f));
            SetLocalRect(subtitleText.rectTransform, new Rect(26f, 64f, geometry.Hero.width - 52f, 58f));
            SetLocalRect(summaryTitleText.rectTransform, new Rect(18f, 14f, geometry.Summary.width - 36f, 26f));
            SetLocalRect(summaryLineText.rectTransform, new Rect(20f, 46f, geometry.Summary.width - 40f, 22f));
            SetLocalRect(partyText.rectTransform, new Rect(20f, 78f, geometry.Summary.width - 40f, geometry.Summary.height - 94f));
            SetLocalRect(routeTitleText.rectTransform, new Rect(18f, 14f, geometry.Route.width - 36f, 26f));
            SetLocalRect(routeText.rectTransform, new Rect(20f, 52f, geometry.Route.width - 40f, geometry.Route.height - 70f));
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
            if (size >= 18) text.fontStyle = FontStyle.Bold;
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
