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
                    Subtitle = "Controls and tactics",
                    Lines = new[]
                    {
                        "WASD / arrows: walk, or move an open board cursor. Left stick moves the cursor.",
                        "U / Backspace: undo movement before taking an action.",
                        "1 / Z: Move. F: Attack / Shoot. Rangers shoot unless engaged.",
                        "C: Spells / Skills. Esc / right-click: cancel targeting without spending the action.",
                        "Tab / E / right bumper: next legal target. Q / left bumper: previous. Top face button: commands.",
                        "Enter / Space / Submit: confirm the open cursor. Space with no cursor: End Turn.",
                        "G: Guard. H: Elixir. I: Armory. Esc: Menu. Retreat costs 1 supply; Growth unlocks after combat.",
                        "Stand still for lower spell costs, longer reach, and stronger hits.",
                        "Verdant Shelter lasts " + Math.Max(1, summonedTreeDuration) + " rounds, blocking arrows and direct bolts. Arcing spells pass over it.",
                        "Hover a tile or target for range, cover, and damage."
                    }
                };
            }

            if (mode == GameMode.Explore)
            {
                return new HelpOverlayView
                {
                    Title = "Exploration Help",
                    Subtitle = "Travel and interaction",
                    Lines = new[]
                    {
                        "WASD / arrows / left stick: walk. Hold to keep moving, or click an adjacent tile.",
                        "Space / E / A: talk, loot, or use the highlighted nearby object.",
                        "Q: Details. Tab / Y: Local / Region map.",
                        "Region map: keys, stick, drag, or wheel pan. Space / E / A marks a charted route; Home / gamepad X finds the party.",
                        "I: Armory and Growth. J: Journal. C: spell reference. P / Esc: Menu.",
                        "Follow NEXT: first leave Town Hall's Grand Hearth through the storm doors.",
                        "East and west gates lead to the roads; north and south remain sealed."
                    }
                };
            }

            if (mode == GameMode.Muster)
            {
                return new HelpOverlayView
                {
                    Title = "Party Setup Help",
                    Subtitle = "Create four companions",
                    Lines = new[]
                    {
                        "Select a companion, then choose a race and class.",
                        "Attributes & details opens stats, talents, appearance, and equipment. Assign all 50 attribute points to continue.",
                        "Reroll Gear changes starting equipment; Reroll Look changes appearance.",
                        "Begin starts with your choices. Quick Start uses Warrior, Ranger, Mage, and Priest.",
                        "Spend points earned later in Armory > Growth."
                    }
                };
            }

            if (mode == GameMode.Victory || mode == GameMode.Defeat)
            {
                return new HelpOverlayView
                {
                    Title = mode == GameMode.Victory ? "Victory Screen Help" : "Defeat Screen Help",
                    Subtitle = "",
                    Lines = mode == GameMode.Victory
                        ? new[] { "New Party opens character creation. Tavern returns to the title screen." }
                        : new[] { "Tavern → Continue loads the last saved checkpoint. New Party opens character creation." }
                };
            }

            return new HelpOverlayView
            {
                Title = "Tavern Help",
                Subtitle = "Campaign and settings",
                Lines = TavernLines(developerTestingVisible)
            };
        }

        private static string[] TavernLines(bool developerTestingVisible)
        {
            string[] normal =
            {
                "Continue loads your saved campaign. New Game opens character creation.",
                "Settings controls music, sound effects, and Reduced Motion across campaigns.",
                "During gameplay: F5 saves, F9 loads, and Esc opens the menu."
            };

            if (!developerTestingVisible) return normal;

            return normal.Concat(new[]
            {
                "The separate Beta Development build adds a direct Beta Lab row for spell and combat testing.",
                "In the lab, F10 or controller Back focuses responsive test controls; arrows/left stick choose and Enter/A activates.",
                "T opens the broader combat, martial, and route testing panel from the tavern."
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
        private RectTransform bodyViewport;
        private ScrollRect bodyScroll;
        private Scrollbar bodyScrollbar;
        private Button closeButton;
        private Text titleText;
        private Text subtitleText;
        private Text bodyText;
        private Text hintText;
        private Font font;
        private float lastWidth = -1f;
        private float lastHeight = -1f;
        private bool lastRefreshSucceeded;
        private string lastBody = "";
        private GameObject previousSelection;

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
            bool wasVisible = IsVisible;
            if (visible && !wasVisible && EventSystem.current != null)
                previousSelection = EventSystem.current.currentSelectedGameObject;
            UiRuntime.SetCanvasVisible(canvas, visible);
            if (visible && !wasVisible)
            {
                bodyScroll.verticalNormalizedPosition = 1f;
                if (EventSystem.current != null && !EventSystem.current.alreadySelecting)
                    EventSystem.current.SetSelectedGameObject(closeButton.gameObject);
            }
            else if (!visible && wasVisible && EventSystem.current != null && !EventSystem.current.alreadySelecting)
            {
                EventSystem.current.SetSelectedGameObject(previousSelection != null && previousSelection.activeInHierarchy ? previousSelection : null);
                previousSelection = null;
            }
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
            string body = string.Join("\n\n", (view.Lines ?? Array.Empty<string>()).Where(line => !string.IsNullOrWhiteSpace(line)).Select(line => "- " + line));
            bool contentChanged = body != lastBody;
            bodyText.text = body;
            lastBody = body;
            bodyText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Max(bodyViewport.rect.height, bodyText.preferredHeight));
            if (contentChanged) bodyScroll.verticalNormalizedPosition = 1f;
            hintText.text = bodyText.preferredHeight > bodyViewport.rect.height
                ? "Wheel / PgUp / PgDn: scroll   ·   Esc / F1: close"
                : "Esc / F1: close";
            Canvas.ForceUpdateCanvases();
            lastRefreshSucceeded = true;
        }

        private void Update()
        {
            if (!IsVisible || bodyScroll == null) return;
            if (Input.GetKeyDown(KeyCode.Home)) bodyScroll.verticalNormalizedPosition = 1f;
            else if (Input.GetKeyDown(KeyCode.End)) bodyScroll.verticalNormalizedPosition = 0f;
            else
            {
                float pixels = 0f;
                bool scrollbarSelected = EventSystem.current != null
                    && EventSystem.current.currentSelectedGameObject == bodyScrollbar.gameObject;
                if (Input.GetKeyDown(KeyCode.PageDown)) pixels = bodyViewport.rect.height * 0.85f;
                else if (Input.GetKeyDown(KeyCode.PageUp)) pixels = -bodyViewport.rect.height * 0.85f;
                else if (!scrollbarSelected && Input.GetKeyDown(KeyCode.DownArrow)) pixels = 36f;
                else if (!scrollbarSelected && Input.GetKeyDown(KeyCode.UpArrow)) pixels = -36f;
                float overflow = bodyText.rectTransform.rect.height - bodyViewport.rect.height;
                if (overflow > 0f && pixels != 0f)
                    bodyScroll.verticalNormalizedPosition = Mathf.Clamp01(bodyScroll.verticalNormalizedPosition - pixels / overflow);
            }
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
            bodyViewport = AddImage("Body Viewport", bodyPanel, Color.clear).rectTransform;
            bodyViewport.gameObject.AddComponent<RectMask2D>();
            bodyText = AddText("Body Text", bodyViewport, "", 13, Hex("f3ead7", 1f), TextAnchor.UpperLeft);
            bodyScroll = bodyPanel.gameObject.AddComponent<ScrollRect>();
            bodyScroll.viewport = bodyViewport;
            bodyScroll.content = bodyText.rectTransform;
            bodyScroll.horizontal = false;
            bodyScroll.vertical = true;
            bodyScroll.movementType = ScrollRect.MovementType.Clamped;
            bodyScroll.inertia = false;
            bodyScroll.scrollSensitivity = 36f;
            Image scrollTrack = AddImage("Help Scrollbar", bodyPanel, Hex("172126", 1f));
            bodyScrollbar = scrollTrack.gameObject.AddComponent<Scrollbar>();
            Image scrollHandle = AddImage("Handle", scrollTrack.transform, Hex("58b7a5", 0.92f));
            Stretch(scrollHandle.rectTransform);
            bodyScrollbar.handleRect = scrollHandle.rectTransform;
            bodyScrollbar.targetGraphic = scrollHandle;
            bodyScrollbar.direction = Scrollbar.Direction.BottomToTop;
            bodyScroll.verticalScrollbar = bodyScrollbar;
            hintText = AddText("Hint", panel, "", 10, Hex("b7aa90", 1f), TextAnchor.MiddleLeft);
            closeButton = AddButton("Close", panel, "Close", () => bindings?.Close?.Invoke());
            Navigation closeNavigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnUp = bodyScrollbar };
            closeButton.navigation = closeNavigation;
            Navigation scrollNavigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnRight = closeButton };
            bodyScrollbar.navigation = scrollNavigation;
        }

        private void ApplyLayout()
        {
            ApplyLayout(Screen.width, Screen.height);
        }

        private void ApplyLayout(float width, float height)
        {
            lastWidth = width;
            lastHeight = height;
            HelpOverlayGeometry geometry = HelpOverlayLayout.Calculate(width, height);
            SetScreenRect(backdrop, geometry.Backdrop);
            SetScreenRect(panel, geometry.Panel);
            SetLocalRect(titleText.rectTransform, new Rect(26f, 20f, geometry.Panel.width - 52f, 30f));
            SetLocalRect(subtitleText.rectTransform, new Rect(28f, 54f, geometry.Panel.width - 56f, 22f));
            SetLocalRect(bodyPanel, geometry.Body);
            SetLocalRect(bodyViewport, new Rect(14f, 12f, geometry.Body.width - 54f, geometry.Body.height - 24f));
            SetLocalRect(bodyText.rectTransform, new Rect(0f, 0f, bodyViewport.rect.width, bodyViewport.rect.height));
            SetLocalRect(bodyScrollbar.GetComponent<RectTransform>(), new Rect(geometry.Body.width - 30f, 12f, 18f, geometry.Body.height - 24f));
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
