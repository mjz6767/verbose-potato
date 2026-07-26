using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AshenHalls
{
    public sealed class ExplorationHudPartyMemberView
    {
        public string Name;
        public string ClassLine;
        public string ColorHex;
        public int Hp;
        public int MaxHp;
        public int Mana;
        public int MaxMana;
    }

    public sealed class ExplorationHudLogView
    {
        public string Text;
        public string Tone;
    }

    public sealed class ExplorationHudView
    {
        public string Title;
        public string RouteLine;
        public string FocusHint;
        public string Gold;
        public string Supplies;
        public string Elixirs;
        public bool DetailsOpen;
        public string ViewLabel;
        public string ZoneName;
        public string ZoneDetail;
        public string DangerLabel;
        public string LookLine;
        public string ObjectiveLine;
        public string ObjectiveSummary;
        public string WaypointLine;
        public string NearbyLine;
        public string GrowthLine;
        public bool HasAction;
        public string ActionLabel;
        public string ActionTarget;
        public IReadOnlyList<ExplorationHudPartyMemberView> Party = Array.Empty<ExplorationHudPartyMemberView>();
        public IReadOnlyList<ExplorationHudLogView> Logs = Array.Empty<ExplorationHudLogView>();
    }

    public sealed class ExplorationHudScreenBindings
    {
        public Func<ExplorationHudView> View;
        public Action UseContextual;
        public Action OpenParty;
        public Action OpenJournal;
        public Action ToggleDetails;
        public Action ToggleView;
        public Action OpenMenu;
    }

    public readonly struct ExplorationHudGeometry
    {
        public readonly Rect Top;
        public readonly Rect Side;
        public readonly Rect Command;

        public ExplorationHudGeometry(Rect top, Rect side, Rect command)
        {
            Top = top;
            Side = side;
            Command = command;
        }

        public bool Fits(float width, float height)
        {
            return FitsRect(Top, width, height)
                && FitsRect(Side, width, height)
                && FitsRect(Command, width, height)
                && Top.yMax <= Side.yMin + 24f
                && Side.yMax <= Command.yMin - 4f;
        }

        private static bool FitsRect(Rect rect, float width, float height)
        {
            return rect.xMin >= 0f && rect.yMin >= 0f && rect.xMax <= width && rect.yMax <= height;
        }
    }

    public static class ExplorationHudScreenLayout
    {
        public const float DetailsBoardGap = 10f;
        public const float MinimumReservedBoardWidth = 420f;
        public const int MinimumEyebrowFontSize = 11;
        public const int MinimumVitalFontSize = 9;
        public const int MinimumBodyFontSize = 12;
        public const int MinimumCommandFontSize = 13;
        public const int MinimumTitleFontSize = 18;

        public static float InterfaceScale(float width, float height)
        {
            float widthScale = width / 1280f;
            float heightScale = height / 720f;
            return Mathf.Clamp(Mathf.Min(widthScale, heightScale), 1f, 1.25f);
        }

        public static float Scale(float value, float width, float height)
        {
            return value * InterfaceScale(width, height);
        }

        public static int FontSize(int baseSize, float width, float height)
        {
            return Mathf.Max(baseSize, Mathf.RoundToInt(baseSize * InterfaceScale(width, height)));
        }

        public static ExplorationHudGeometry Calculate(float width, float height, bool detailsOpen)
        {
            float scale = InterfaceScale(width, height);
            float topMargin = 8f * scale;
            float topHeight = 48f * scale;
            float commandMargin = 10f * scale;
            float commandHeight = 68f * scale;
            Rect top = new Rect(topMargin, topMargin, width - topMargin * 2f, topHeight);
            Rect command = new Rect(12f * scale, height - commandHeight - commandMargin, width - 24f * scale, commandHeight);
            float sideTop = top.yMax + 12f * scale;
            float sideBottom = command.yMin - 12f * scale;
            Rect side;
            if (detailsOpen)
            {
                float sideW = Mathf.Clamp(width * 0.22f, 330f * scale, 410f * scale);
                float sideH = Mathf.Max(330f * scale, sideBottom - sideTop);
                side = new Rect(width - sideW - 18f * scale, sideTop, sideW, sideH);
            }
            else
            {
                float railW = Mathf.Clamp(width * 0.18f, 270f * scale, 330f * scale);
                float sideH = Mathf.Max(360f * scale, sideBottom - sideTop);
                side = new Rect(width - railW - 18f * scale, sideTop, railW, sideH);
            }

            return new ExplorationHudGeometry(top, side, command);
        }

        public static Rect ReserveDetailsFromBoard(Rect board, float width, float height, bool detailsOpen)
        {
            if (board.width <= 0f || board.height <= 0f) return board;

            Rect side = Calculate(width, height, detailsOpen).Side;
            float desiredRight = side.xMin - DetailsBoardGap;
            if (board.xMax <= desiredRight) return board;

            float minRight = board.xMin + Mathf.Min(MinimumReservedBoardWidth, Mathf.Max(0f, desiredRight - board.xMin));
            float newRight = Mathf.Max(minRight, desiredRight);
            newRight = Mathf.Min(newRight, board.xMax);
            return new Rect(board.x, board.y, Mathf.Max(0f, newRight - board.x), board.height);
        }

        public static Rect[] CommandButtons(float width)
        {
            float scale = Mathf.Clamp(width / 1256f, 1f, 1.25f);
            float gap = 8f * scale;
            float x = 10f * scale;
            float y = 8f * scale;
            float h = 52f * scale;
            float menuW = Mathf.Clamp(width * 0.12f, 128f * scale, 176f * scale);
            float actionW = Mathf.Clamp(width - menuW - gap - 20f * scale, 280f * scale, Mathf.Max(280f * scale, width - menuW - gap - 20f * scale));
            return new[]
            {
                new Rect(x, y, actionW, h),
                new Rect(x + actionW + gap, y, menuW, h)
            };
        }
    }

    public sealed class ExplorationHudScreen : MonoBehaviour
    {
        private readonly List<PartyRow> partyRows = new List<PartyRow>();
        private readonly List<LogRow> logRows = new List<LogRow>();
        private ExplorationHudScreenBindings bindings;
        private Canvas canvas;
        private CanvasGroup canvasGroup;
        private RectTransform topPanel;
        private RectTransform sidePanel;
        private RectTransform commandPanel;
        private Text titleText;
        private Text routeText;
        private Text focusText;
        private Text goldText;
        private Text suppliesText;
        private Text elixirsText;
        private Text sideTitleText;
        private Text sideDangerText;
        private Text waypointTitleText;
        private Text objectiveTitleText;
        private Text nearbyTitleText;
        private Text sideDetailText;
        private Text lookText;
        private Text objectiveText;
        private Text growthText;
        private Text partyTitleText;
        private Text latestTitleText;
        private Text actionLabelText;
        private Text actionTargetText;
        private Text detailsButtonText;
        private Button actionButton;
        private Button menuButton;
        private Button detailsButton;
        private Text menuButtonText;
        private Font font;
        private float lastWidth = -1f;
        private float lastHeight = -1f;
        private bool lastDetailsOpen;
        private int detailLogCapacity = 3;

        public bool IsReady => canvas != null && commandPanel != null && actionButton != null;
        public bool IsVisible => IsReady && UiRuntime.IsCanvasVisible(canvas);
        public bool HasVisibleCompactGuidance => IsVisible
            && !lastDetailsOpen
            && objectiveText != null
            && objectiveText.gameObject.activeInHierarchy
            && !string.IsNullOrWhiteSpace(objectiveText.text)
            && sideDetailText != null
            && !string.IsNullOrWhiteSpace(sideDetailText.text);
        public int VisiblePartyRows => partyRows.Count(row => row.Root != null && row.Root.gameObject.activeInHierarchy);
        public bool HasExpandedResourceLabelsForTest => goldText != null
            && suppliesText != null
            && elixirsText != null
            && goldText.text.StartsWith("Gold\n", StringComparison.Ordinal)
            && suppliesText.text.StartsWith("Supplies\n", StringComparison.Ordinal)
            && elixirsText.text.StartsWith("Elixirs\n", StringComparison.Ordinal);
        public int NumericPartyVitalRowsForTest => partyRows.Count(row =>
            row.Root != null
            && row.Root.gameObject.activeInHierarchy
            && row.HpText != null
            && row.ManaText != null
            && row.HpText.text.StartsWith("HP ", StringComparison.Ordinal)
            && row.ManaText.text.StartsWith("MP ", StringComparison.Ordinal));

        public void Bind(ExplorationHudScreenBindings screenBindings)
        {
            bindings = screenBindings;
            Build();
            Refresh();
        }

        public bool SetVisible(bool visible)
        {
            bool changed = UiRuntime.SetCanvasVisible(canvas, visible);
            if (changed && visible)
            {
                lastWidth = -1f;
                lastHeight = -1f;
            }
            return changed;
        }

        public void SetUnderlay(bool underlay)
        {
            if (canvasGroup == null) return;
            canvasGroup.alpha = underlay ? 0.42f : 1f;
            canvasGroup.interactable = !underlay;
            canvasGroup.blocksRaycasts = !underlay;
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
            if (bindings == null || canvas == null) return;
            ExplorationHudView view = bindings.View == null ? null : bindings.View();
            if (view == null) return;

            if (!Mathf.Approximately(lastWidth, Screen.width) || !Mathf.Approximately(lastHeight, Screen.height) || lastDetailsOpen != view.DetailsOpen)
            {
                ApplyLayout(view.DetailsOpen);
            }

            titleText.text = string.IsNullOrEmpty(view.Title) ? VersionInfo.ProductName : view.Title;
            routeText.text = view.RouteLine ?? "";
            focusText.text = view.FocusHint ?? "";
            goldText.text = "Gold\n" + (view.Gold ?? "0");
            suppliesText.text = "Supplies\n" + (view.Supplies ?? "0");
            elixirsText.text = "Elixirs\n" + (view.Elixirs ?? "0");
            sideTitleText.text = string.IsNullOrEmpty(view.ZoneName) ? "Location" : view.ZoneName;
            sideDangerText.text = view.DangerLabel ?? "";
            sideDetailText.text = view.DetailsOpen ? view.ZoneDetail ?? "" : view.WaypointLine ?? "";
            lookText.text = view.DetailsOpen ? view.LookLine ?? "" : view.NearbyLine ?? "";
            objectiveText.text = view.DetailsOpen ? view.ObjectiveLine ?? "" : view.ObjectiveSummary ?? "";
            growthText.text = view.GrowthLine ?? "";
            actionLabelText.text = view.HasAction ? view.ActionLabel ?? "Use" : "No Action";
            actionTargetText.text = view.HasAction ? view.ActionTarget ?? "" : "Nothing nearby";
            actionButton.interactable = view.HasAction;
            detailsButtonText.text = view.DetailsOpen ? "Close" : "Details";
            menuButtonText.text = "Menu";
            SetModeObjectsVisible(view.DetailsOpen);

            IReadOnlyList<ExplorationHudPartyMemberView> party = view.Party ?? Array.Empty<ExplorationHudPartyMemberView>();
            for (int i = 0; i < partyRows.Count; i++)
            {
                bool visible = i < party.Count;
                partyRows[i].Root.gameObject.SetActive(visible);
                if (!visible) continue;
                ExplorationHudPartyMemberView member = party[i];
                partyRows[i].Name.text = member.Name ?? "";
                partyRows[i].ClassLine.text = member.ClassLine ?? "";
                partyRows[i].Accent.color = ParseColor(member.ColorHex, Hex("58b7a5", 1f));
                partyRows[i].HpText.text = $"HP {Mathf.Max(0, member.Hp)}/{Mathf.Max(0, member.MaxHp)}";
                partyRows[i].ManaText.text = $"MP {Mathf.Max(0, member.Mana)}/{Mathf.Max(0, member.MaxMana)}";
                SetFill(partyRows[i].HpFill, member.Hp, member.MaxHp);
                SetFill(partyRows[i].ManaFill, member.Mana, member.MaxMana);
            }

            IReadOnlyList<ExplorationHudLogView> logs = view.Logs ?? Array.Empty<ExplorationHudLogView>();
            for (int i = 0; i < logRows.Count; i++)
            {
                bool visible = view.DetailsOpen && i < logs.Count && i < detailLogCapacity;
                logRows[i].Root.gameObject.SetActive(visible);
                if (!visible) continue;
                logRows[i].Text.text = logs[i].Text ?? "";
                logRows[i].Stripe.color = ToneColor(logs[i].Tone);
            }
        }

        private void Build()
        {
            EnsureEventSystem();
            font = UiRuntime.DefaultFont;

            canvas = UiRuntime.CreateOwnedRootCanvas(this, "Exploration HUD Canvas");
            UiRuntime.ConfigureOverlayCanvas(canvas, 20);
            canvasGroup = canvas.gameObject.AddComponent<CanvasGroup>();
            Stretch(canvas.GetComponent<RectTransform>());

            topPanel = AddPanel("Top Chrome", canvas.transform, Hex("080d10", 0.78f), Hex("3c4544", 0.56f));
            titleText = AddText("Title", topPanel, VersionInfo.ProductName, ExplorationHudScreenLayout.MinimumTitleFontSize, Hex("f3ead7", 1f), TextAnchor.MiddleLeft);
            titleText.resizeTextForBestFit = true;
            titleText.resizeTextMinSize = 14;
            titleText.resizeTextMaxSize = ExplorationHudScreenLayout.MinimumTitleFontSize;
            routeText = AddText("Route", topPanel, "", ExplorationHudScreenLayout.MinimumBodyFontSize, Hex("d0c5ae", 1f), TextAnchor.MiddleCenter);
            focusText = AddText("Focus", topPanel, "", ExplorationHudScreenLayout.MinimumBodyFontSize, Hex("66c9b6", 1f), TextAnchor.MiddleRight);
            goldText = AddText("Gold", topPanel, "", ExplorationHudScreenLayout.MinimumEyebrowFontSize, Hex("f3ead7", 1f), TextAnchor.MiddleCenter);
            suppliesText = AddText("Supplies", topPanel, "", ExplorationHudScreenLayout.MinimumEyebrowFontSize, Hex("f3ead7", 1f), TextAnchor.MiddleCenter);
            elixirsText = AddText("Elixirs", topPanel, "", ExplorationHudScreenLayout.MinimumEyebrowFontSize, Hex("f3ead7", 1f), TextAnchor.MiddleCenter);

            sidePanel = AddPanel("Location Panel", canvas.transform, Hex("080b0d", 0.86f), Hex("58b7a5", 0.72f));
            sideTitleText = AddText("Location Title", sidePanel, "Location", ExplorationHudScreenLayout.MinimumTitleFontSize, Hex("e3ba63", 1f), TextAnchor.MiddleLeft);
            sideDangerText = AddText("Danger", sidePanel, "", ExplorationHudScreenLayout.MinimumEyebrowFontSize, Hex("66c9b6", 1f), TextAnchor.MiddleLeft);
            waypointTitleText = AddText("Waypoint Title", sidePanel, "NEXT", ExplorationHudScreenLayout.MinimumEyebrowFontSize, Hex("e3ba63", 1f), TextAnchor.MiddleLeft);
            objectiveTitleText = AddText("Objective Title", sidePanel, "OBJECTIVE", ExplorationHudScreenLayout.MinimumEyebrowFontSize, Hex("66c9b6", 1f), TextAnchor.MiddleLeft);
            nearbyTitleText = AddText("Nearby Title", sidePanel, "NEARBY", ExplorationHudScreenLayout.MinimumEyebrowFontSize, Hex("d0c5ae", 1f), TextAnchor.MiddleLeft);
            sideDetailText = AddText("Detail", sidePanel, "", ExplorationHudScreenLayout.MinimumBodyFontSize, Hex("d0c5ae", 1f), TextAnchor.UpperLeft);
            lookText = AddText("Look", sidePanel, "", ExplorationHudScreenLayout.MinimumBodyFontSize, Hex("f3ead7", 1f), TextAnchor.UpperLeft);
            objectiveText = AddText("Objective", sidePanel, "", ExplorationHudScreenLayout.MinimumBodyFontSize, Hex("f3ead7", 1f), TextAnchor.UpperLeft);
            growthText = AddText("Growth", sidePanel, "", ExplorationHudScreenLayout.MinimumEyebrowFontSize, Hex("d0c5ae", 1f), TextAnchor.UpperLeft);
            partyTitleText = AddText("Party Title", sidePanel, "Party", ExplorationHudScreenLayout.MinimumBodyFontSize, Hex("66c9b6", 1f), TextAnchor.MiddleLeft);
            latestTitleText = AddText("Latest Title", sidePanel, "Latest", ExplorationHudScreenLayout.MinimumBodyFontSize, Hex("e3ba63", 1f), TextAnchor.MiddleLeft);
            detailsButton = AddButton("Details", sidePanel, "Details", bindings?.ToggleDetails, false);
            detailsButtonText = detailsButton.GetComponentInChildren<Text>();

            for (int i = 0; i < 4; i++) partyRows.Add(CreatePartyRow(sidePanel, i));
            for (int i = 0; i < 3; i++) logRows.Add(CreateLogRow(sidePanel, i));

            commandPanel = AddPanel("Command Bar", canvas.transform, Hex("080b0d", 0.72f), Hex("3c4544", 0.50f));
            actionButton = AddButton("Use Action", commandPanel, "", bindings?.UseContextual, true);
            actionLabelText = actionButton.GetComponentInChildren<Text>();
            actionTargetText = AddText("Use Target", actionButton.transform, "", ExplorationHudScreenLayout.MinimumEyebrowFontSize, Hex("d0c5ae", 1f), TextAnchor.LowerCenter);
            menuButton = AddButton("Menu", commandPanel, "Menu", bindings?.OpenMenu, false);
            menuButtonText = menuButton.GetComponentInChildren<Text>();
        }

        private void ApplyLayout(bool detailsOpen)
        {
            lastWidth = Screen.width;
            lastHeight = Screen.height;
            lastDetailsOpen = detailsOpen;
            ExplorationHudGeometry geometry = ExplorationHudScreenLayout.Calculate(Screen.width, Screen.height, detailsOpen);
            float scale = ExplorationHudScreenLayout.InterfaceScale(Screen.width, Screen.height);
            SetScreenRect(topPanel, geometry.Top);
            SetScreenRect(sidePanel, geometry.Side);
            SetScreenRect(commandPanel, geometry.Command);
            ApplyResponsiveTypography();

            float resourceW = 78f * scale;
            float resourceGap = 6f * scale;
            float resourcesW = resourceW * 3f + resourceGap * 2f;
            float resourcesX = geometry.Top.width - resourcesW - 10f * scale;
            float headerX = 14f * scale;
            float headerW = Mathf.Max(360f * scale, resourcesX - headerX - 14f * scale);
            float titleW = headerW * 0.31f;
            float routeW = headerW * 0.40f;
            SetLocalRect(titleText.rectTransform, new Rect(headerX, 6f * scale, titleW, 36f * scale));
            SetLocalRect(routeText.rectTransform, new Rect(headerX + titleW, 6f * scale, routeW, 36f * scale));
            SetLocalRect(focusText.rectTransform, new Rect(headerX + titleW + routeW, 6f * scale, headerW - titleW - routeW, 36f * scale));
            SetLocalRect(goldText.rectTransform, new Rect(resourcesX, 6f * scale, resourceW, 36f * scale));
            SetLocalRect(suppliesText.rectTransform, new Rect(resourcesX + resourceW + resourceGap, 6f * scale, resourceW, 36f * scale));
            SetLocalRect(elixirsText.rectTransform, new Rect(resourcesX + (resourceW + resourceGap) * 2f, 6f * scale, resourceW, 36f * scale));

            float sidePad = 14f * scale;
            float innerW = geometry.Side.width - sidePad * 2f;
            SetLocalRect(sideTitleText.rectTransform, new Rect(sidePad, 8f * scale, innerW, 25f * scale));
            SetLocalRect(sideDangerText.rectTransform, new Rect(sidePad, 35f * scale, innerW, 18f * scale));
            if (detailsOpen)
            {
                SetLocalRect(sideDetailText.rectTransform, new Rect(sidePad, 58f * scale, innerW, 30f * scale));
                SetLocalRect(lookText.rectTransform, new Rect(sidePad, 92f * scale, innerW, 44f * scale));
                SetLocalRect(objectiveText.rectTransform, new Rect(sidePad, 144f * scale, innerW, 82f * scale));
                SetLocalRect(growthText.rectTransform, new Rect(sidePad, 232f * scale, innerW, 32f * scale));
                SetLocalRect(partyTitleText.rectTransform, new Rect(sidePad, 270f * scale, innerW, 20f * scale));
                float partyStartY = 294f * scale;
                float partyStep = 34f * scale;
                for (int i = 0; i < partyRows.Count; i++)
                {
                    SetLocalRect(partyRows[i].Root, new Rect(sidePad, partyStartY + i * partyStep, innerW, 30f * scale));
                    LayoutPartyRow(partyRows[i], innerW, 30f * scale, scale);
                }

                float latestY = partyStartY + partyRows.Count * partyStep + 6f * scale;
                float buttonY = geometry.Side.height - 42f * scale;
                SetLocalRect(latestTitleText.rectTransform, new Rect(sidePad, latestY, innerW, 20f * scale));
                float logsStart = latestY + 24f * scale;
                float logsBottom = buttonY - 8f * scale;
                float idealLogStep = 32f * scale;
                detailLogCapacity = Mathf.Clamp(Mathf.FloorToInt((logsBottom - logsStart + 4f * scale) / idealLogStep), 0, logRows.Count);
                float logStep = detailLogCapacity <= 0
                    ? idealLogStep
                    : Mathf.Min(38f * scale, (logsBottom - logsStart) / detailLogCapacity);
                float logHeight = Mathf.Max(24f * scale, logStep - 4f * scale);
                for (int i = 0; i < logRows.Count; i++)
                {
                    SetLocalRect(logRows[i].Root, new Rect(sidePad, logsStart + i * logStep, innerW, logHeight));
                    LayoutLogRow(logRows[i], innerW, logHeight, scale);
                }
                SetLocalRect(detailsButton.GetComponent<RectTransform>(), new Rect(sidePad, buttonY, innerW, 32f * scale));
            }
            else
            {
                detailLogCapacity = 0;
                SetLocalRect(waypointTitleText.rectTransform, new Rect(sidePad, 62f * scale, innerW, 16f * scale));
                SetLocalRect(sideDetailText.rectTransform, new Rect(sidePad, 80f * scale, innerW, 48f * scale));
                SetLocalRect(objectiveTitleText.rectTransform, new Rect(sidePad, 136f * scale, innerW, 16f * scale));
                SetLocalRect(objectiveText.rectTransform, new Rect(sidePad, 154f * scale, innerW, 88f * scale));
                SetLocalRect(nearbyTitleText.rectTransform, new Rect(sidePad, 250f * scale, innerW, 16f * scale));
                SetLocalRect(lookText.rectTransform, new Rect(sidePad, 268f * scale, innerW, 58f * scale));
                SetLocalRect(partyTitleText.rectTransform, new Rect(sidePad, 334f * scale, innerW, 20f * scale));
                float compactPartyStartY = 358f * scale;
                float compactPartyStep = 34f * scale;
                for (int i = 0; i < partyRows.Count; i++)
                {
                    SetLocalRect(partyRows[i].Root, new Rect(sidePad, compactPartyStartY + i * compactPartyStep, innerW, 30f * scale));
                    LayoutPartyRow(partyRows[i], innerW, 30f * scale, scale);
                }
                SetLocalRect(detailsButton.GetComponent<RectTransform>(), new Rect(sidePad, geometry.Side.height - 42f * scale, innerW, 32f * scale));
            }

            Rect[] buttons = ExplorationHudScreenLayout.CommandButtons(geometry.Command.width);
            SetLocalRect(actionButton.GetComponent<RectTransform>(), buttons[0]);
            SetLocalRect(actionLabelText.rectTransform, new Rect(12f * scale, 5f * scale, buttons[0].width - 24f * scale, 24f * scale));
            actionLabelText.alignment = TextAnchor.MiddleCenter;
            SetLocalRect(actionTargetText.rectTransform, new Rect(12f * scale, 29f * scale, buttons[0].width - 24f * scale, 18f * scale));
            SetLocalRect(menuButton.GetComponent<RectTransform>(), buttons[1]);
        }

        private void ApplyResponsiveTypography()
        {
            int titleSize = ExplorationHudScreenLayout.FontSize(ExplorationHudScreenLayout.MinimumTitleFontSize, Screen.width, Screen.height);
            int bodySize = ExplorationHudScreenLayout.FontSize(ExplorationHudScreenLayout.MinimumBodyFontSize, Screen.width, Screen.height);
            int eyebrowSize = ExplorationHudScreenLayout.FontSize(ExplorationHudScreenLayout.MinimumEyebrowFontSize, Screen.width, Screen.height);
            int commandSize = ExplorationHudScreenLayout.FontSize(ExplorationHudScreenLayout.MinimumCommandFontSize, Screen.width, Screen.height);
            titleText.fontSize = titleSize;
            titleText.resizeTextMinSize = Mathf.Max(14, titleSize - 4);
            titleText.resizeTextMaxSize = titleSize;
            routeText.fontSize = bodySize;
            focusText.fontSize = bodySize;
            goldText.fontSize = eyebrowSize;
            suppliesText.fontSize = eyebrowSize;
            elixirsText.fontSize = eyebrowSize;
            sideTitleText.fontSize = titleSize;
            sideDangerText.fontSize = eyebrowSize;
            waypointTitleText.fontSize = eyebrowSize;
            objectiveTitleText.fontSize = eyebrowSize;
            nearbyTitleText.fontSize = eyebrowSize;
            sideDetailText.fontSize = bodySize;
            lookText.fontSize = bodySize;
            objectiveText.fontSize = bodySize;
            growthText.fontSize = eyebrowSize;
            partyTitleText.fontSize = bodySize;
            latestTitleText.fontSize = bodySize;
            detailsButtonText.fontSize = commandSize;
            actionLabelText.fontSize = commandSize + 1;
            actionTargetText.fontSize = eyebrowSize;
            menuButtonText.fontSize = commandSize;
            foreach (PartyRow row in partyRows)
            {
                row.Name.fontSize = bodySize;
                row.ClassLine.fontSize = Mathf.Max(10, eyebrowSize - 1);
                row.HpText.fontSize = Mathf.Max(ExplorationHudScreenLayout.MinimumVitalFontSize, eyebrowSize - 2);
                row.ManaText.fontSize = Mathf.Max(ExplorationHudScreenLayout.MinimumVitalFontSize, eyebrowSize - 2);
            }
            foreach (LogRow row in logRows) row.Text.fontSize = eyebrowSize;
        }

        private void SetModeObjectsVisible(bool detailsOpen)
        {
            waypointTitleText.gameObject.SetActive(!detailsOpen);
            objectiveTitleText.gameObject.SetActive(!detailsOpen);
            nearbyTitleText.gameObject.SetActive(!detailsOpen);
            lookText.gameObject.SetActive(true);
            objectiveText.gameObject.SetActive(true);
            growthText.gameObject.SetActive(detailsOpen);
            partyTitleText.gameObject.SetActive(true);
            latestTitleText.gameObject.SetActive(detailsOpen && detailLogCapacity > 0);
        }

        private PartyRow CreatePartyRow(Transform parent, int index)
        {
            RectTransform root = AddPanel("Party Row " + index, parent, Hex("151b20", 0.86f), Hex("3c4544", 0.45f));
            Image accent = AddImage("Accent", root, Hex("58b7a5", 1f));
            Text name = AddText("Name", root, "", ExplorationHudScreenLayout.MinimumBodyFontSize, Hex("f3ead7", 1f), TextAnchor.MiddleLeft);
            Text classLine = AddText("Class", root, "", 10, Hex("d0c5ae", 1f), TextAnchor.MiddleLeft);
            Image hpBg = AddImage("Hp Bg", root, Hex("050708", 0.85f));
            Image hpFill = AddImage("Hp Fill", hpBg.transform, Hex("b94b56", 1f));
            Image manaBg = AddImage("Mana Bg", root, Hex("050708", 0.85f));
            Image manaFill = AddImage("Mana Fill", manaBg.transform, Hex("58b7a5", 1f));
            Text hpText = AddText("Hp Text", root, "HP 0/0", ExplorationHudScreenLayout.MinimumVitalFontSize, Hex("f3ead7", 1f), TextAnchor.MiddleCenter);
            Text manaText = AddText("Mana Text", root, "MP 0/0", ExplorationHudScreenLayout.MinimumVitalFontSize, Hex("f3ead7", 1f), TextAnchor.MiddleCenter);
            hpText.fontStyle = FontStyle.Bold;
            manaText.fontStyle = FontStyle.Bold;
            return new PartyRow(root, accent, name, classLine, hpBg.rectTransform, hpFill.rectTransform, hpText, manaBg.rectTransform, manaFill.rectTransform, manaText);
        }

        private LogRow CreateLogRow(Transform parent, int index)
        {
            RectTransform root = AddPanel("Log Row " + index, parent, Hex("151b20", 0.82f), Hex("3c4544", 0.35f));
            Image stripe = AddImage("Stripe", root, Hex("7f9d5b", 1f));
            Text text = AddText("Text", root, "", ExplorationHudScreenLayout.MinimumEyebrowFontSize, Hex("f3ead7", 1f), TextAnchor.UpperLeft);
            return new LogRow(root, stripe, text);
        }

        private static void LayoutPartyRow(PartyRow row, float width, float height, float scale)
        {
            SetLocalRect(row.Accent.rectTransform, new Rect(0f, 0f, 4f * scale, height));
            float vitalsW = 116f * scale;
            float vitalsX = width - vitalsW - 8f * scale;
            SetLocalRect(row.Name.rectTransform, new Rect(10f * scale, 2f * scale, Mathf.Max(72f * scale, vitalsX - 16f * scale), 15f * scale));
            SetLocalRect(row.ClassLine.rectTransform, new Rect(10f * scale, 16f * scale, Mathf.Max(72f * scale, vitalsX - 16f * scale), 12f * scale));
            SetLocalRect(row.HpBg, new Rect(vitalsX, 3f * scale, vitalsW, 11f * scale));
            SetLocalRect(row.HpText.rectTransform, new Rect(vitalsX, 2f * scale, vitalsW, 12f * scale));
            SetLocalRect(row.ManaBg, new Rect(vitalsX, 16f * scale, vitalsW, 11f * scale));
            SetLocalRect(row.ManaText.rectTransform, new Rect(vitalsX, 15f * scale, vitalsW, 12f * scale));
            Stretch(row.HpFill);
            Stretch(row.ManaFill);
        }

        private static void LayoutLogRow(LogRow row, float width, float height, float scale)
        {
            SetLocalRect(row.Stripe.rectTransform, new Rect(0f, 0f, 4f * scale, height));
            SetLocalRect(row.Text.rectTransform, new Rect(10f * scale, 3f * scale, width - 16f * scale, Mathf.Max(18f * scale, height - 6f * scale)));
        }

        private Button AddButton(string name, Transform parent, string label, Action action, bool hero)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            Image image = go.GetComponent<Image>();
            image.color = hero ? Hex("1a2026", 0.98f) : Hex("151a1f", 0.96f);
            Button button = go.GetComponent<Button>();
            button.targetGraphic = image;
            ColorBlock colors = button.colors;
            colors.normalColor = image.color;
            colors.highlightedColor = hero ? Hex("2d3440", 1f) : Hex("232a31", 1f);
            colors.pressedColor = Hex("0b1013", 1f);
            colors.disabledColor = Hex("0b0f12", 0.76f);
            colors.selectedColor = colors.highlightedColor;
            button.colors = colors;
            if (action != null) button.onClick.AddListener(() => action());

            Text text = AddText(
                "Label",
                go.transform,
                label,
                hero ? ExplorationHudScreenLayout.MinimumCommandFontSize + 1 : ExplorationHudScreenLayout.MinimumCommandFontSize,
                Hex("f3ead7", 1f),
                TextAnchor.MiddleCenter);
            text.fontStyle = FontStyle.Bold;
            Stretch(text.rectTransform, 8f, hero ? 10f : 4f);
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
            if (size >= 14) text.fontStyle = FontStyle.Bold;
            return text;
        }

        private static void SetFill(RectTransform fill, int value, int max)
        {
            if (fill == null) return;
            float ratio = max <= 0 ? 0f : Mathf.Clamp01((float)Mathf.Max(0, value) / max);
            fill.anchorMin = new Vector2(0f, 0f);
            fill.anchorMax = new Vector2(ratio, 1f);
            fill.offsetMin = Vector2.zero;
            fill.offsetMax = Vector2.zero;
        }

        private static Color ToneColor(string tone)
        {
            if (string.Equals(tone, "Warn", StringComparison.OrdinalIgnoreCase)) return Hex("c65c3b", 1f);
            if (string.Equals(tone, "Good", StringComparison.OrdinalIgnoreCase)) return Hex("58b7a5", 1f);
            return Hex("7f9d5b", 1f);
        }

        private static Color ParseColor(string hex, Color fallback)
        {
            if (string.IsNullOrWhiteSpace(hex)) return fallback;
            if (!ColorUtility.TryParseHtmlString(hex.StartsWith("#") ? hex : "#" + hex, out Color color)) return fallback;
            return color;
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

        private readonly struct PartyRow
        {
            public readonly RectTransform Root;
            public readonly Image Accent;
            public readonly Text Name;
            public readonly Text ClassLine;
            public readonly RectTransform HpBg;
            public readonly RectTransform HpFill;
            public readonly Text HpText;
            public readonly RectTransform ManaBg;
            public readonly RectTransform ManaFill;
            public readonly Text ManaText;

            public PartyRow(
                RectTransform root,
                Image accent,
                Text name,
                Text classLine,
                RectTransform hpBg,
                RectTransform hpFill,
                Text hpText,
                RectTransform manaBg,
                RectTransform manaFill,
                Text manaText)
            {
                Root = root;
                Accent = accent;
                Name = name;
                ClassLine = classLine;
                HpBg = hpBg;
                HpFill = hpFill;
                HpText = hpText;
                ManaBg = manaBg;
                ManaFill = manaFill;
                ManaText = manaText;
            }
        }

        private readonly struct LogRow
        {
            public readonly RectTransform Root;
            public readonly Image Stripe;
            public readonly Text Text;

            public LogRow(RectTransform root, Image stripe, Text text)
            {
                Root = root;
                Stripe = stripe;
                Text = text;
            }
        }
    }
}
