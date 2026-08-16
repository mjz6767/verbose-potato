using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AshenHalls
{
    public sealed class HelpOverlayView
    {
        public string Title;
        public string Subtitle;
        public string[] Lines;
    }

    public sealed class HelpOverlayBindings
    {
        public Func<HelpOverlayView> View;
        public Action Close;
    }

    public readonly struct HelpOverlayGeometry
    {
        public readonly Rect Backdrop;
        public readonly Rect Panel;
        public readonly Rect Body;
        public readonly Rect CloseButton;

        public HelpOverlayGeometry(Rect backdrop, Rect panel, Rect body, Rect closeButton)
        {
            Backdrop = backdrop;
            Panel = panel;
            Body = body;
            CloseButton = closeButton;
        }

        public bool Fits(float width, float height)
        {
            return FitsRect(Backdrop, width, height)
                && FitsRect(Panel, width, height)
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

    public static class HelpOverlayLayout
    {
        public static HelpOverlayGeometry Calculate(float width, float height)
        {
            float panelW = Mathf.Min(Mathf.Max(560f, width * 0.48f), width - 72f);
            float panelH = Mathf.Min(Mathf.Max(390f, height * 0.56f), height - 88f);
            Rect backdrop = new Rect(0f, 0f, width, height);
            Rect panel = new Rect((width - panelW) * 0.5f, (height - panelH) * 0.5f, panelW, panelH);
            Rect body = new Rect(26f, 104f, panelW - 52f, panelH - 168f);
            Rect closeButton = new Rect(panelW - 138f, panelH - 48f, 112f, 30f);
            return new HelpOverlayGeometry(backdrop, panel, body, closeButton);
        }
    }

    public static class HelpOverlayContent
    {
        public static HelpOverlayView Build(GameMode mode, bool developerTestingVisible, int summonedTreeDuration, string homeTownName)
        {
            if (mode == GameMode.Combat)
            {
                return new HelpOverlayView
                {
                    Title = "Combat Help",
                    Subtitle = "Turn tactics, abilities, and spell targeting",
                    Lines = new[]
                    {
                        "WASD / arrows quick-step while the board cursor is closed; with it open, WASD / arrows / left stick move the cursor.",
                        "U / Backspace: undo this turn's movement before committing an action.",
                        "1 / Z: Move cursor. F: Attack or Shoot cursor. Rangers shoot by default unless engaged.",
                        "C: Ability. Casters open the Spellbook; martial classes open skills. Esc or right-click cancels an armed target without spending the action.",
                        "Tab / E / right bumper: next legal target. Q / left bumper: previous. The top face button returns to combat commands.",
                        "Enter / Space / controller Submit confirms an open cursor; with no cursor, Space remains End Turn.",
                        "G: Guard. H: Elixir. I: Armory; Growth is review-only until combat ends. Esc: Menu. Campaign fights can retreat for one supply.",
                        "Standing still focuses casting: lower MP, longer reach, and harder hits.",
                        "Tree Cover lasts " + Math.Max(1, summonedTreeDuration) + " rounds. It blocks arrows/direct bolts, but arcing spells can pass over it.",
                        "Hover tiles and targets for range, line-of-sight, cover, and damage notes."
                    }
                };
            }

            if (mode == GameMode.Explore)
            {
                return new HelpOverlayView
                {
                    Title = "Exploration Help",
                    Subtitle = homeTownName + ", the Old Road, and contextual use",
                    Lines = new[]
                    {
                        "Local Map: WASD / arrows move one tile. Click adjacent tiles to walk.",
                        "Space / E: use the highlighted nearby target: talk, loot, enter, recall, or descend.",
                        "Q: Details. Tab / gamepad Y: Local/Region. Region Map: WASD/arrows/left stick, drag, or wheel pan; Home/gamepad X finds the party.",
                        "I: Armory. Its Growth tab previews and spends earned points. J: Journal. C: spell reference.",
                        "P or Esc: Menu for save, load, settings, return, or new game.",
                        "You begin among the patrons in Town Hall's Grand Hearth: follow NEXT to leave through its storm doors and begin the journey.",
                        "Midgaard begins the sewer contract: speak with the king, gather supplies, then clear the rat den.",
                        "East and west gates are pass-through roads. North and south gates are sealed for now.",
                        "If a path feels blocked, bump the object or use Space/E beside it."
                    }
                };
            }

            if (mode == GameMode.Muster)
            {
                return new HelpOverlayView
                {
                    Title = "Party Setup Help",
                    Subtitle = "Four-person starter party",
                    Lines = new[]
                    {
                        "Begin starts the current party.",
                        "Quick Start uses Warrior, Ranger, Mage, and Priest.",
                        "Advanced choices let you adjust class, race, name, look, gear, color, and stats.",
                        "Attributes use a 50-point budget.",
                        "Later levels earn points that can be previewed and spent in I > Growth.",
                        "Reroll Gear changes starting equipment. Reroll Look changes visual identity.",
                        "The first slice is balanced around melee, ranged pressure, elemental magic, and healing."
                    }
                };
            }

            if (mode == GameMode.Victory || mode == GameMode.Defeat)
            {
                return new HelpOverlayView
                {
                    Title = mode == GameMode.Victory ? "Victory Screen Help" : "Defeat Screen Help",
                    Subtitle = "End-state actions",
                    Lines = new[]
                    {
                        "New Party returns to the tavern muster.",
                        "Victory also offers Tavern to return to the first screen.",
                        "Development builds may show Beta Lab for isolated combat testing.",
                        "F1 or Esc closes this Help overlay."
                    }
                };
            }

            return new HelpOverlayView
            {
                Title = "Tavern Help",
                Subtitle = "Starting, continuing, and testing",
                Lines = TavernLines(developerTestingVisible)
            };
        }

        private static string[] TavernLines(bool developerTestingVisible)
        {
            string[] normal =
            {
                "Continue appears when a campaign save exists.",
                "Begin the Old Road opens the four-person muster before Town Hall's Grand Hearth.",
                "Quick Start accepts the default company; Begin uses the current muster choices.",
                "Settings controls audio volume and reduced motion.",
                "F5 saves and F9 loads during gameplay. Esc opens the gameplay menu."
            };

            if (!developerTestingVisible) return normal;

            return normal.Concat(new[]
            {
                "Development builds also show Beta Testing for combat, martial, and route labs.",
                "T toggles the testing panel from the tavern."
            }).ToArray();
        }
    }

    public sealed class HelpOverlayScreen : MonoBehaviour
    {
        private HelpOverlayBindings bindings;
        private Canvas canvas;
        private RectTransform backdrop;
        private RectTransform panel;
        private RectTransform bodyPanel;
        private Button closeButton;
        private Text titleText;
        private Text subtitleText;
        private Text bodyText;
        private Text hintText;
        private Font font;
        private float lastWidth = -1f;
        private float lastHeight = -1f;
        private bool lastRefreshSucceeded;

        public bool IsReady => canvas != null && panel != null && closeButton != null && bodyPanel != null;
        public bool IsVisible => IsReady && UiRuntime.IsCanvasVisible(canvas);
        public bool HasRenderableGeometry => IsReady
            && lastRefreshSucceeded
            && UiRuntime.CanOwnModal(canvas, panel, null, closeButton);

        public void Bind(HelpOverlayBindings screenBindings)
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
            HelpOverlayView view = bindings.View == null ? null : bindings.View();
            if (view == null) return;
            if (!Mathf.Approximately(lastWidth, Screen.width) || !Mathf.Approximately(lastHeight, Screen.height))
            {
                ApplyLayout();
            }

            titleText.text = string.IsNullOrWhiteSpace(view.Title) ? "Help" : view.Title;
            subtitleText.text = view.Subtitle ?? "";
            bodyText.text = string.Join("\n", (view.Lines ?? Array.Empty<string>()).Where(line => !string.IsNullOrWhiteSpace(line)).Select(line => "- " + line));
            hintText.text = "Esc, F1, or Close";
            Canvas.ForceUpdateCanvases();
            lastRefreshSucceeded = true;
        }

        private void Build()
        {
            EnsureEventSystem();
            font = UiRuntime.DefaultFont;
            canvas = UiRuntime.CreateOwnedRootCanvas(this, "Help Overlay Canvas");
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 34;
            CanvasScaler scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            canvas.gameObject.AddComponent<GraphicRaycaster>();
            Stretch(canvas.GetComponent<RectTransform>());

            backdrop = AddImage("Backdrop", canvas.transform, Hex("020303", 0.58f)).rectTransform;
            panel = AddPanel("Help Panel", canvas.transform, Hex("10161b", 0.98f), Hex("58b7a5", 0.92f));
            titleText = AddText("Title", panel, "Help", 24, Hex("f3ead7", 1f), TextAnchor.MiddleLeft);
            subtitleText = AddText("Subtitle", panel, "", 12, Hex("b7aa90", 1f), TextAnchor.MiddleLeft);
            bodyPanel = AddPanel("Body", panel, Hex("080b0d", 0.78f), Hex("3c4544", 0.88f));
            bodyText = AddText("Body Text", bodyPanel, "", 13, Hex("f3ead7", 1f), TextAnchor.UpperLeft);
            hintText = AddText("Hint", panel, "", 10, Hex("b7aa90", 1f), TextAnchor.MiddleLeft);
            closeButton = AddButton("Close", panel, "Close", () => bindings?.Close?.Invoke());
        }

        private void ApplyLayout()
        {
            lastWidth = Screen.width;
            lastHeight = Screen.height;
            HelpOverlayGeometry geometry = HelpOverlayLayout.Calculate(Screen.width, Screen.height);
            SetScreenRect(backdrop, geometry.Backdrop);
            SetScreenRect(panel, geometry.Panel);
            SetLocalRect(titleText.rectTransform, new Rect(26f, 20f, geometry.Panel.width - 52f, 30f));
            SetLocalRect(subtitleText.rectTransform, new Rect(28f, 54f, geometry.Panel.width - 56f, 22f));
            SetLocalRect(bodyPanel, geometry.Body);
            SetLocalRect(bodyText.rectTransform, new Rect(14f, 12f, geometry.Body.width - 28f, geometry.Body.height - 24f));
            SetLocalRect(hintText.rectTransform, new Rect(28f, geometry.Panel.height - 44f, geometry.CloseButton.x - 42f, 20f));
            SetLocalRect(closeButton.GetComponent<RectTransform>(), geometry.CloseButton);
        }

        private Button AddButton(string name, Transform parent, string label, Action action)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(Outline));
            go.transform.SetParent(parent, false);
            Image image = go.GetComponent<Image>();
            image.color = Hex("172126", 0.98f);
            Button button = go.GetComponent<Button>();
            button.targetGraphic = image;
            ColorBlock colors = button.colors;
            colors.normalColor = image.color;
            colors.highlightedColor = Hex("24363a", 1f);
            colors.pressedColor = Hex("0b1013", 1f);
            colors.selectedColor = colors.highlightedColor;
            button.colors = colors;
            Outline outline = go.GetComponent<Outline>();
            outline.effectColor = Hex("58b7a5", 0.82f);
            outline.effectDistance = new Vector2(1f, -1f);
            if (action != null) button.onClick.AddListener(() => action());
            Text text = AddText("Label", go.transform, label, 12, Hex("f3ead7", 1f), TextAnchor.MiddleCenter);
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
            SetLocalRect(rect, area);
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
