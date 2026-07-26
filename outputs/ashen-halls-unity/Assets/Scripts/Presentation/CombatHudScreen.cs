using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AshenHalls
{
    public sealed class CombatHudUnitView
    {
        public string Name;
        public string Header;
        public string StateLine;
        public string StatusLine;
        public string AccentHex;
        public int Hp;
        public int MaxHp;
        public int Mana;
        public int MaxMana;
    }

    public sealed class CombatHudCommandView
    {
        public ActionMode Mode;
        public string Label;
        public string Hotkey;
        public string SubLabel;
        public string Tooltip;
        public string DisabledReason;
        public Texture2D IconTexture;
        public Rect IconSource;
        public bool Enabled;
        public bool Selected;
        public bool Promoted;
    }

    public sealed class CombatHudLogView
    {
        public string Text;
        public string Tone;
    }

    public sealed class CombatHudTurnView
    {
        public string Name;
        public string AccentHex;
        public bool Active;
        public bool StartsNextRound;
    }

    public sealed class CombatHudView
    {
        public string Title;
        public string RouteLine;
        public string Gold;
        public string Supplies;
        public string Elixirs;
        public string RoundLine;
        public string PhaseLine;
        public string TacticalLine;
        public string CommandPrompt;
        public bool PlayerTurn;
        public bool TimelineExpanded;
        public bool CanUndoMove;
        public bool CanCancelTarget;
        public string CancelTargetLabel;
        public string TargetTitle;
        public CombatHudUnitView ActiveUnit;
        public CombatHudUnitView TargetUnit;
        public IReadOnlyList<CombatHudCommandView> Commands = Array.Empty<CombatHudCommandView>();
        public bool GuardEnabled;
        public bool ElixirEnabled;
        public string GuardReason;
        public string ElixirReason;
        public IReadOnlyList<CombatHudTurnView> Turns = Array.Empty<CombatHudTurnView>();
        public IReadOnlyList<CombatHudLogView> Logs = Array.Empty<CombatHudLogView>();
    }

    public sealed class CombatHudScreenBindings
    {
        public Func<CombatHudView> View;
        public Action<ActionMode> RunCommand;
        public Action<ActionMode> RunUtility;
        public Action UndoMove;
        public Action CancelTarget;
        public Action ToggleTimeline;
        public Action OpenMenu;
    }

    public readonly struct CombatHudGeometry
    {
        public readonly Rect Top;
        public readonly Rect Side;
        public readonly Rect Command;

        public CombatHudGeometry(Rect top, Rect side, Rect command)
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
                && Top.yMax <= Side.yMin
                && Side.yMax <= height - 8f
                && Command.yMin >= height - 124f;
        }

        private static bool FitsRect(Rect rect, float width, float height)
        {
            return rect.xMin >= 0f && rect.yMin >= 0f && rect.xMax <= width && rect.yMax <= height;
        }
    }

    public readonly struct CombatHudUnitCardGeometry
    {
        public readonly Rect Title;
        public readonly Rect Name;
        public readonly Rect Header;
        public readonly Rect State;
        public readonly Rect Hp;
        public readonly Rect Mana;
        public readonly Rect Status;
        public readonly bool ShowsMana;

        public CombatHudUnitCardGeometry(Rect title, Rect name, Rect header, Rect state, Rect hp, Rect mana, Rect status, bool showsMana)
        {
            Title = title;
            Name = name;
            Header = header;
            State = state;
            Hp = hp;
            Mana = mana;
            Status = status;
            ShowsMana = showsMana;
        }

        public bool Fits(float width, float height)
        {
            bool inside = FitsRect(Title, width, height)
                && FitsRect(Name, width, height)
                && FitsRect(Header, width, height)
                && FitsRect(State, width, height)
                && FitsRect(Hp, width, height)
                && FitsRect(Status, width, height)
                && (!ShowsMana || FitsRect(Mana, width, height));
            bool ordered = Title.yMax <= Name.yMin
                && Name.yMax <= Header.yMin
                && Header.yMax <= State.yMin
                && State.yMax <= Hp.yMin
                && Hp.yMax <= (ShowsMana ? Mana.yMin : Status.yMin)
                && (!ShowsMana || Mana.yMax <= Status.yMin);
            return inside && ordered;
        }

        private static bool FitsRect(Rect rect, float width, float height)
        {
            return rect.xMin >= 0f && rect.yMin >= 0f && rect.xMax <= width && rect.yMax <= height;
        }
    }

    public static class CombatHudScreenLayout
    {
        public static float SideRailWidth(float width)
        {
            return Mathf.Clamp(width * 0.22f, 344f, 420f);
        }

        public static CombatHudGeometry Calculate(float width, float height)
        {
            float sideW = SideRailWidth(width);
            Rect top = new Rect(12f, 10f, width - 24f, 58f);
            Rect side = new Rect(width - sideW - 12f, 78f, sideW, Mathf.Max(280f, height - 90f));
            Rect command = new Rect(12f, height - 116f, side.x - 24f, 104f);
            return new CombatHudGeometry(top, side, command);
        }

        public static Rect[] CommandButtons(float width, bool promoteEndTurn)
        {
            const float padding = 8f;
            const float gap = 7f;
            const float groupGap = 16f;
            float endTurnBonus = promoteEndTurn ? Mathf.Clamp(width * 0.09f, 60f, 120f) : 0f;
            const int commandCount = 6;
            float gapsWidth = gap * 4f + groupGap;
            float buttonW = Mathf.Max(72f, (width - padding * 2f - endTurnBonus - gapsWidth) / commandCount);
            float buttonY = 28f;
            float buttonH = 66f;
            Rect[] rects = new Rect[commandCount];
            float x = padding;
            for (int i = 0; i < commandCount; i++)
            {
                float w = i == commandCount - 1 && promoteEndTurn ? buttonW + endTurnBonus : buttonW;
                rects[i] = new Rect(x, buttonY, w, buttonH);
                x += w + (i == 2 ? groupGap : gap);
            }
            return rects;
        }

        public static Rect CommandPrompt(float width, bool showUndoMove, bool showCancelTarget)
        {
            int contextButtons = (showUndoMove ? 1 : 0) + (showCancelTarget ? 1 : 0);
            float reserved = 24f + contextButtons * 138f;
            return new Rect(12f, 4f, Mathf.Max(120f, width - reserved), 20f);
        }

        public static Rect UndoMoveButton(float width, bool showCancelTarget)
        {
            return ContextButton(width, showCancelTarget ? 1 : 0);
        }

        public static Rect CancelTargetButton(float width)
        {
            return ContextButton(width, 0);
        }

        private static Rect ContextButton(float width, int slotFromRight)
        {
            return new Rect(width - 142f - slotFromRight * 138f, 3f, 132f, 22f);
        }

        public static void SidePanels(Rect side, bool timelineExpanded, out Rect active, out Rect target, out Rect timeline)
        {
            float gap = 10f;
            float timelineH = timelineExpanded
                ? Mathf.Clamp(side.height * 0.42f, 190f, Mathf.Max(190f, side.height - 330f))
                : 110f;
            float activeH = Mathf.Clamp(side.height * 0.28f, 168f, 230f);
            float targetH = Mathf.Clamp(side.height * 0.24f, 146f, 206f);
            float availableCardH = Mathf.Max(0f, side.height - timelineH - gap * 2f);
            float requestedCardH = activeH + targetH;
            if (requestedCardH > availableCardH)
            {
                float activeShare = requestedCardH <= 0f ? 0.54f : activeH / requestedCardH;
                activeH = availableCardH * activeShare;
                targetH = availableCardH - activeH;
            }

            active = new Rect(0f, 0f, side.width, activeH);
            target = new Rect(0f, active.yMax + gap, side.width, targetH);
            timeline = new Rect(0f, target.yMax + gap, side.width, timelineH);
        }

        public static CombatHudUnitCardGeometry UnitCard(float width, float height, bool showMana)
        {
            const float x = 12f;
            float contentWidth = Mathf.Max(1f, width - x * 2f);
            bool compact = height < 168f;
            float titleY = compact ? 6f : 8f;
            float titleH = compact ? 20f : 24f;
            float nameY = titleY + titleH + (compact ? 2f : 8f);
            float nameH = compact ? 19f : 22f;
            float headerY = nameY + nameH + 2f;
            float headerH = compact ? 15f : 18f;
            float stateY = headerY + headerH + 2f;
            float statusH = compact ? 16f : 18f;
            float statusY = Mathf.Max(0f, height - (compact ? 8f : 10f) - statusH);
            float meterBottom = statusY - (compact ? 4f : 6f);
            const float manaH = 13f;
            float manaY = meterBottom - manaH;
            const float hpH = 14f;
            float hpY = (showMana ? manaY - 5f : meterBottom) - hpH;
            float stateH = Mathf.Max(0f, Mathf.Min(compact ? 16f : 18f, hpY - stateY - 3f));

            return new CombatHudUnitCardGeometry(
                new Rect(x, titleY, contentWidth, titleH),
                new Rect(x, nameY, contentWidth, nameH),
                new Rect(x, headerY, contentWidth, headerH),
                new Rect(x, stateY, contentWidth, stateH),
                new Rect(x, hpY, contentWidth, hpH),
                new Rect(x, manaY, contentWidth, manaH),
                new Rect(x, statusY, contentWidth, statusH),
                showMana);
        }
    }

    internal sealed class CombatHudCommandHoverRelay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Action Enter;
        public Action Exit;

        public void OnPointerEnter(PointerEventData eventData)
        {
            Enter?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Exit?.Invoke();
        }
    }

    public sealed class CombatHudScreen : MonoBehaviour
    {
        private readonly List<CommandRow> commandRows = new List<CommandRow>();
        private readonly List<LogRow> logRows = new List<LogRow>();
        private CombatHudScreenBindings bindings;
        private Canvas canvas;
        private CanvasGroup canvasGroup;
        private RectTransform topPanel;
        private RectTransform sidePanel;
        private RectTransform activePanel;
        private RectTransform targetPanel;
        private RectTransform timelinePanel;
        private RectTransform commandPanel;
        private RectTransform commandDivider;
        private RectTransform utilityPopup;
        private Text titleText;
        private Text routeText;
        private Text phaseText;
        private Text goldText;
        private Text suppliesText;
        private Text elixirsText;
        private Text activeTitle;
        private Text activeName;
        private Text activeHeader;
        private Text activeState;
        private Text activeStatus;
        private RectTransform activeHpFill;
        private RectTransform activeManaFill;
        private Text targetTitle;
        private Text targetName;
        private Text targetHeader;
        private Text targetState;
        private Text targetStatus;
        private RectTransform targetHpFill;
        private RectTransform targetManaFill;
        private Text timelineTitle;
        private Text turnQueueText;
        private Text timelineButtonText;
        private Text commandPromptText;
        private Button undoMoveButton;
        private Text undoMoveText;
        private Button cancelTargetButton;
        private Text cancelTargetText;
        private Button timelineButton;
        private Button utilityButton;
        private Text utilityLabel;
        private Text utilitySubLabel;
        private Button guardButton;
        private Button elixirButton;
        private Button menuButton;
        private Text guardText;
        private Text elixirText;
        private Text menuText;
        private Font font;
        private bool utilityOpen;
        private float lastWidth = -1f;
        private float lastHeight = -1f;
        private bool lastTimelineExpanded;
        private bool lastPromoteEndTurn;
        private bool lastShowUndoMove;
        private bool lastShowCancelTarget;
        private int hoveredCommandIndex = -1;

        public bool IsReady => canvas != null
            && canvasGroup != null
            && topPanel != null
            && sidePanel != null
            && activePanel != null
            && targetPanel != null
            && timelinePanel != null
            && turnQueueText != null
            && commandPanel != null
            && commandPromptText != null
            && undoMoveButton != null
            && cancelTargetButton != null
            && utilityPopup != null
            && timelineButton != null
            && utilityButton != null
            && guardButton != null
            && elixirButton != null
            && menuButton != null
            && commandRows.Count == 6
            && logRows.Count == 5;
        public bool IsVisible => IsReady && UiRuntime.IsCanvasVisible(canvas);
        public bool HasRenderableGeometry => IsReady && UiRuntime.IsRenderableRootOverlay(canvas);
        public bool IsUtilityOpen => utilityOpen && utilityPopup != null && utilityPopup.gameObject.activeInHierarchy;
        public bool HasLaidOutCommandBar
        {
            get
            {
                if (!HasRenderableGeometry || commandPanel == null) return false;
                Rect rect = commandPanel.rect;
                float minimumWidth = Mathf.Min(420f, Mathf.Max(160f, Screen.width * 0.30f));
                return commandPanel.gameObject.activeInHierarchy
                    && rect.width >= minimumWidth
                    && rect.height >= 54f
                    && VisibleCommandCount == 6;
            }
        }
        public bool IsSuppressedByImguiFallback => IsVisible
            && canvasGroup != null
            && canvasGroup.alpha <= 0.01f
            && !canvasGroup.interactable
            && !canvasGroup.blocksRaycasts;
        public bool HasUsableCommandBar
        {
            get
            {
                return HasLaidOutCommandBar
                    && canvasGroup != null
                    && canvasGroup.alpha > 0.01f
                    && canvasGroup.interactable
                    && canvasGroup.blocksRaycasts
                    && UiRuntime.HasUsableEventSystem();
            }
        }
        public string CommandBarHealth
        {
            get
            {
                Rect rect = commandPanel == null ? Rect.zero : commandPanel.rect;
                return $"screen={Screen.width}x{Screen.height}, ready={IsReady}, visible={IsVisible}, active={commandPanel != null && commandPanel.gameObject.activeInHierarchy}, alpha={(canvasGroup == null ? -1f : canvasGroup.alpha):0.00}, rect={rect.width:0}x{rect.height:0}, commands={VisibleCommandCount}";
            }
        }
        public int VisibleCommandCount
        {
            get
            {
                int visible = 0;
                foreach (CommandRow row in commandRows)
                {
                    if (row?.Root != null && row.Root.gameObject.activeInHierarchy) visible++;
                }
                return visible;
            }
        }

        public int VisibleLogCount
        {
            get
            {
                int visible = 0;
                foreach (LogRow row in logRows)
                {
                    if (row.Root != null && row.Root.gameObject.activeInHierarchy) visible++;
                }
                return visible;
            }
        }

        public int VisibleSectionCount
        {
            get
            {
                RectTransform[] sections = { topPanel, sidePanel, activePanel, targetPanel, timelinePanel, commandPanel };
                int visible = 0;
                foreach (RectTransform section in sections)
                {
                    if (section != null && section.gameObject.activeInHierarchy) visible++;
                }
                return visible;
            }
        }
        public bool HasTurnQueue => IsReady
            && turnQueueText != null
            && turnQueueText.gameObject.activeInHierarchy
            && !string.IsNullOrWhiteSpace(turnQueueText.text);

        public void InvokeCommandForTest(ActionMode mode)
        {
            CommandRow row = commandRows.Find(candidate => candidate != null && candidate.Mode == mode);
            if (row == null || !row.Button.interactable) throw new InvalidOperationException($"Combat command {mode} is not ready.");
            row.Button.onClick.Invoke();
        }

        public void FocusCommand(ActionMode mode)
        {
            if (!IsVisible || EventSystem.current == null) return;
            CommandRow row = commandRows.Find(candidate => candidate != null
                && candidate.Mode == mode
                && candidate.Root != null
                && candidate.Root.gameObject.activeInHierarchy
                && candidate.Button != null
                && candidate.Button.interactable);
            if (row != null) EventSystem.current.SetSelectedGameObject(row.Button.gameObject);
        }

        public void InvokeUtilityForTest()
        {
            if (utilityButton == null || !utilityButton.interactable) throw new InvalidOperationException("Combat Utility button is not ready.");
            utilityButton.onClick.Invoke();
        }

        public void InvokeUndoMoveForTest()
        {
            if (undoMoveButton == null || !undoMoveButton.gameObject.activeInHierarchy || !undoMoveButton.interactable)
            {
                throw new InvalidOperationException("Combat Undo Move button is not ready.");
            }
            undoMoveButton.onClick.Invoke();
        }

        public bool IsUndoMoveVisible => undoMoveButton != null && undoMoveButton.gameObject.activeInHierarchy;

        public void InvokeCancelTargetForTest()
        {
            if (cancelTargetButton == null || !cancelTargetButton.gameObject.activeInHierarchy || !cancelTargetButton.interactable)
            {
                throw new InvalidOperationException("Combat Cancel Target button is not ready.");
            }
            cancelTargetButton.onClick.Invoke();
        }

        public bool IsCancelTargetVisible => cancelTargetButton != null && cancelTargetButton.gameObject.activeInHierarchy;

        public void Bind(CombatHudScreenBindings screenBindings)
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
            CombatHudView view = bindings.View == null ? null : bindings.View();
            if (view == null) return;

            bool promoteEndTurn = false;
            for (int i = 0; i < view.Commands.Count; i++)
            {
                if (view.Commands[i].Mode == ActionMode.Wait && view.Commands[i].Promoted) promoteEndTurn = true;
            }

            if (!Mathf.Approximately(lastWidth, Screen.width)
                || !Mathf.Approximately(lastHeight, Screen.height)
                || lastTimelineExpanded != view.TimelineExpanded
                || lastPromoteEndTurn != promoteEndTurn
                || lastShowUndoMove != view.CanUndoMove
                || lastShowCancelTarget != view.CanCancelTarget)
            {
                ApplyLayout(view.TimelineExpanded, promoteEndTurn, view.CanUndoMove, view.CanCancelTarget);
            }

            titleText.text = string.IsNullOrEmpty(view.Title) ? VersionInfo.ProductName : view.Title;
            routeText.text = view.RouteLine ?? "";
            phaseText.text = view.PhaseLine ?? "";
            phaseText.color = view.ActiveUnit == null
                ? Hex("b7aa90", 1f)
                : view.PlayerTurn ? Hex("58b7a5", 1f) : Hex("c65c3b", 1f);
            goldText.text = "Gold\n" + (view.Gold ?? "0");
            suppliesText.text = "Supplies\n" + (view.Supplies ?? "0");
            elixirsText.text = "Elixirs\n" + (view.Elixirs ?? "0");
            timelineTitle.text = string.IsNullOrEmpty(view.RoundLine) ? "Timeline" : "Timeline / " + view.RoundLine;
            timelineButtonText.text = view.TimelineExpanded ? "Hide" : "Show";
            string queue = BuildTurnQueueText(view.Turns);
            turnQueueText.text = !view.TimelineExpanded || string.IsNullOrWhiteSpace(view.TacticalLine)
                ? queue
                : queue + "\n<color=#d7a84e>PLAN</color>  " + view.TacticalLine;

            RefreshUnitCard(view.ActiveUnit, activeTitle, activeName, activeHeader, activeState, activeStatus, activeHpFill, activeManaFill, "Active Unit");
            RefreshUnitCard(
                view.TargetUnit,
                targetTitle,
                targetName,
                targetHeader,
                targetState,
                targetStatus,
                targetHpFill,
                targetManaFill,
                string.IsNullOrWhiteSpace(view.TargetTitle) ? "Inspect" : view.TargetTitle);

            for (int i = 0; i < commandRows.Count; i++)
            {
                bool visible = i < view.Commands.Count;
                commandRows[i].Root.gameObject.SetActive(visible);
                if (!visible) continue;
                CombatHudCommandView command = view.Commands[i];
                commandRows[i].Mode = command.Mode;
                commandRows[i].Button.interactable = command.Enabled;
                commandRows[i].Label.text = command.Promoted ? (command.Label ?? "").ToUpperInvariant() : command.Label ?? "";
                commandRows[i].Hotkey.text = command.Hotkey ?? "";
                commandRows[i].SubLabel.text = command.Promoted
                    ? "Next combatant"
                    : command.Enabled ? command.SubLabel ?? "" : command.DisabledReason ?? "";
                Sprite icon = UiRuntime.AtlasSprite(command.IconTexture, command.IconSource);
                commandRows[i].Icon.sprite = icon;
                commandRows[i].Icon.enabled = icon != null;
                commandRows[i].IconFallback.gameObject.SetActive(icon == null);
                commandRows[i].IconFallback.text = CommandFallbackGlyph(command.Mode);
                commandRows[i].Icon.color = Color.white.WithAlpha(command.Enabled ? 0.98f : 0.34f);
                commandRows[i].IconWell.color = command.Promoted ? Hex("181107", 0.98f) : command.Selected ? Hex("0b1718", 0.98f) : Hex("080b0d", 0.96f);
                commandRows[i].IconOutline.effectColor = command.Promoted ? Hex("d7a84e", 0.92f) : command.Selected ? Hex("58b7a5", 0.88f) : Hex("3c4544", command.Enabled ? 0.72f : 0.34f);
                commandRows[i].HotkeyBackground.color = command.Promoted ? Hex("d7a84e", 0.92f) : command.Selected ? Hex("58b7a5", 0.86f) : Hex("263035", command.Enabled ? 0.92f : 0.48f);
                commandRows[i].Hotkey.color = command.Promoted || command.Selected ? Hex("080b0d", 1f) : command.Enabled ? Hex("f3ead7", 1f) : Hex("777c7c", 0.82f);
                commandRows[i].Label.color = command.Enabled ? Hex("f3ead7", 1f) : Hex("777c7c", 0.74f);
                commandRows[i].SubLabel.color = command.Enabled ? command.Promoted ? Hex("d7a84e", 1f) : Hex("b7aa90", 1f) : Hex("777c7c", 0.72f);
                commandRows[i].AccentRail.gameObject.SetActive(command.Promoted || command.Selected);
                commandRows[i].AccentRail.color = command.Promoted ? Hex("d7a84e", 1f) : Hex("58b7a5", 1f);
                Image image = commandRows[i].Button.targetGraphic as Image;
                Color fill = command.Promoted ? Hex("352316", 0.98f) : command.Selected ? Hex("243033", 0.98f) : Hex("151b20", 0.96f);
                if (image != null) image.color = fill;
                ColorBlock buttonColors = commandRows[i].Button.colors;
                buttonColors.normalColor = fill;
                buttonColors.highlightedColor = command.Promoted ? Hex("4a321b", 1f) : command.Selected ? Hex("2e3c40", 1f) : Hex("232a31", 1f);
                buttonColors.pressedColor = Hex("080b0d", 1f);
                buttonColors.disabledColor = Hex("0b0f12", 0.78f);
                commandRows[i].Button.colors = buttonColors;
                commandRows[i].Outline.effectColor = command.Promoted ? Hex("d7a84e", 0.95f) : command.Selected ? Hex("58b7a5", 0.92f) : command.Enabled ? Hex("3c4544", 0.86f) : Hex("3c4544", 0.42f);
                float outlineSize = command.Promoted || command.Selected ? 2f : 1f;
                commandRows[i].Outline.effectDistance = new Vector2(outlineSize, -outlineSize);
            }
            RefreshCommandPrompt(view);
            undoMoveButton.gameObject.SetActive(view.CanUndoMove);
            undoMoveButton.interactable = view.CanUndoMove && view.PlayerTurn;
            undoMoveText.text = "Undo Move  [U]";
            cancelTargetButton.gameObject.SetActive(view.CanCancelTarget);
            cancelTargetButton.interactable = view.CanCancelTarget && view.PlayerTurn;
            cancelTargetText.text = string.IsNullOrWhiteSpace(view.CancelTargetLabel)
                ? "Cancel Target  [Esc]"
                : view.CancelTargetLabel + "  [Esc]";

            utilityOpen = false;
            utilityButton.gameObject.SetActive(false);
            utilityPopup.gameObject.SetActive(false);
            guardButton.interactable = view.GuardEnabled;
            elixirButton.interactable = view.ElixirEnabled;
            guardText.text = view.GuardEnabled ? "Guard\nG" : "Guard\n" + (view.GuardReason ?? "");
            elixirText.text = view.ElixirEnabled ? "Elixir\nH" : "Elixir\n" + (view.ElixirReason ?? "");
            menuText.text = "Menu\nEsc";

            IReadOnlyList<CombatHudLogView> logs = view.Logs ?? Array.Empty<CombatHudLogView>();
            int visibleLogs = view.TimelineExpanded
                ? Mathf.Min(logRows.Count, logs.Count)
                : Mathf.Min(1, logs.Count);
            for (int i = 0; i < logRows.Count; i++)
            {
                bool visible = i < visibleLogs;
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
            canvas = UiRuntime.CreateOwnedRootCanvas(this, "Combat HUD Canvas");
            UiRuntime.ConfigureOverlayCanvas(canvas, 20);
            canvasGroup = canvas.gameObject.AddComponent<CanvasGroup>();
            Stretch(canvas.GetComponent<RectTransform>());

            topPanel = AddPanel("Top Chrome", canvas.transform, Hex("10161b", 0.88f), Hex("3c4544", 0.72f));
            titleText = AddText("Title", topPanel, VersionInfo.ProductName, 23, Hex("f3ead7", 1f), TextAnchor.MiddleLeft);
            titleText.resizeTextForBestFit = true;
            titleText.resizeTextMinSize = 14;
            titleText.resizeTextMaxSize = 23;
            routeText = AddText("Route", topPanel, "", 10, Hex("b7aa90", 1f), TextAnchor.MiddleLeft);
            phaseText = AddText("Phase", topPanel, "", 10, Hex("58b7a5", 1f), TextAnchor.MiddleRight);
            phaseText.resizeTextForBestFit = true;
            phaseText.resizeTextMinSize = 8;
            phaseText.resizeTextMaxSize = 10;
            goldText = AddText("Gold", topPanel, "", 9, Hex("f3ead7", 1f), TextAnchor.MiddleCenter);
            suppliesText = AddText("Supplies", topPanel, "", 9, Hex("f3ead7", 1f), TextAnchor.MiddleCenter);
            elixirsText = AddText("Elixirs", topPanel, "", 9, Hex("f3ead7", 1f), TextAnchor.MiddleCenter);

            sidePanel = AddPanel("Combat Side", canvas.transform, Hex("0e1114", 0.08f), Hex("0e1114", 0f));
            activePanel = AddPanel("Active", sidePanel, Hex("1a2026", 0.96f), Hex("58b7a5", 0.82f));
            targetPanel = AddPanel("Target", sidePanel, Hex("1a2026", 0.96f), Hex("b94b56", 0.82f));
            timelinePanel = AddPanel("Timeline", sidePanel, Hex("1a2026", 0.96f), Hex("d7a84e", 0.82f));
            BuildUnitCard(activePanel, out activeTitle, out activeName, out activeHeader, out activeState, out activeStatus, out activeHpFill, out activeManaFill);
            BuildUnitCard(targetPanel, out targetTitle, out targetName, out targetHeader, out targetState, out targetStatus, out targetHpFill, out targetManaFill);
            timelineTitle = AddText("Timeline Title", timelinePanel, "Timeline", 18, Hex("d7a84e", 1f), TextAnchor.MiddleLeft);
            timelineButton = AddButton("Timeline Toggle", timelinePanel, "Show", () => bindings?.ToggleTimeline?.Invoke(), false);
            timelineButtonText = timelineButton.GetComponentInChildren<Text>();
            turnQueueText = AddText("Turn Queue", timelinePanel, "", 10, Hex("f3ead7", 1f), TextAnchor.UpperLeft);
            turnQueueText.supportRichText = true;
            for (int i = 0; i < 5; i++) logRows.Add(CreateLogRow(timelinePanel, i));

            commandPanel = AddPanel("Command Bar", canvas.transform, Hex("080b0d", 0.94f), Hex("3c4544", 0.82f));
            commandPromptText = AddText("Command Prompt", commandPanel, "", 11, Hex("58b7a5", 1f), TextAnchor.MiddleLeft);
            commandPromptText.fontStyle = FontStyle.Bold;
            commandPromptText.resizeTextForBestFit = true;
            commandPromptText.resizeTextMinSize = 9;
            commandPromptText.resizeTextMaxSize = 11;
            undoMoveButton = AddButton("Undo Move", commandPanel, "Undo Move  [U]", () => bindings?.UndoMove?.Invoke(), false);
            undoMoveText = undoMoveButton.GetComponentInChildren<Text>();
            undoMoveText.fontSize = 9;
            undoMoveText.color = Hex("f3ead7", 1f);
            undoMoveButton.gameObject.SetActive(false);
            cancelTargetButton = AddButton("Cancel Target", commandPanel, "Cancel Target  [Esc]", () => bindings?.CancelTarget?.Invoke(), false);
            cancelTargetText = cancelTargetButton.GetComponentInChildren<Text>();
            cancelTargetText.fontSize = 9;
            cancelTargetText.color = Hex("f3ead7", 1f);
            cancelTargetButton.gameObject.SetActive(false);
            commandDivider = AddImage("Command Group Divider", commandPanel, Hex("d7a84e", 0.42f)).rectTransform;
            for (int i = 0; i < 6; i++)
            {
                CommandRow row = CreateCommandRow(commandPanel, i);
                commandRows.Add(row);
            }
            utilityButton = AddButton("Utility", commandPanel, "", ToggleUtility, false);
            utilityLabel = AddText("Utility Label", utilityButton.transform, "Utility", 12, Hex("f3ead7", 1f), TextAnchor.MiddleCenter);
            utilitySubLabel = AddText("Utility Sub", utilityButton.transform, "", 9, Hex("b7aa90", 1f), TextAnchor.LowerCenter);
            utilityPopup = AddPanel("Utility Popup", canvas.transform, Hex("080b0d", 0.98f), Hex("d7a84e", 0.88f));
            guardButton = AddButton("Guard", utilityPopup, "Guard", () => RunUtility(ActionMode.Guard), false);
            elixirButton = AddButton("Elixir", utilityPopup, "Elixir", () => RunUtility(ActionMode.Elixir), false);
            menuButton = AddButton("Menu", utilityPopup, "Menu", () => bindings?.OpenMenu?.Invoke(), false);
            guardText = guardButton.GetComponentInChildren<Text>();
            elixirText = elixirButton.GetComponentInChildren<Text>();
            menuText = menuButton.GetComponentInChildren<Text>();
            utilityPopup.gameObject.SetActive(false);
            utilityButton.gameObject.SetActive(false);
        }

        private void BuildUnitCard(RectTransform panel, out Text title, out Text name, out Text header, out Text state, out Text status, out RectTransform hpFill, out RectTransform manaFill)
        {
            title = AddText("Title", panel, "", 18, Hex("d7a84e", 1f), TextAnchor.MiddleLeft);
            name = AddText("Name", panel, "", 15, Hex("f3ead7", 1f), TextAnchor.MiddleLeft);
            header = AddText("Header", panel, "", 10, Hex("b7aa90", 1f), TextAnchor.MiddleLeft);
            state = AddText("State", panel, "", 10, Hex("d7a84e", 1f), TextAnchor.MiddleLeft);
            status = AddText("Status", panel, "", 9, Hex("b7aa90", 1f), TextAnchor.MiddleLeft);
            RectTransform hpBg = AddImage("Hp Bg", panel, Hex("050708", 0.88f)).rectTransform;
            hpFill = AddImage("Hp Fill", hpBg, Hex("b94b56", 1f)).rectTransform;
            Text hpValue = AddText("Value", hpBg, "", 9, Hex("f3ead7", 1f), TextAnchor.MiddleCenter);
            hpValue.raycastTarget = false;
            AddMeterLabelShadow(hpValue);
            Stretch(hpValue.rectTransform, 2f, 0f);
            RectTransform manaBg = AddImage("Mana Bg", panel, Hex("050708", 0.88f)).rectTransform;
            manaFill = AddImage("Mana Fill", manaBg, Hex("58b7a5", 1f)).rectTransform;
            Text manaValue = AddText("Value", manaBg, "", 9, Hex("f3ead7", 1f), TextAnchor.MiddleCenter);
            manaValue.raycastTarget = false;
            AddMeterLabelShadow(manaValue);
            Stretch(manaValue.rectTransform, 2f, 0f);
        }

        private CommandRow CreateCommandRow(Transform parent, int index)
        {
            Button button = AddButton("Command " + index, parent, "", () => RunCommand(index), true);
            Outline outline = button.GetComponent<Outline>();
            Text label = button.GetComponentInChildren<Text>();
            label.alignment = TextAnchor.MiddleLeft;
            label.fontSize = 13;
            label.resizeTextForBestFit = true;
            label.resizeTextMinSize = 10;
            label.resizeTextMaxSize = 13;
            label.raycastTarget = false;

            Image iconWell = AddImage("Icon Well", button.transform, Hex("080b0d", 0.96f));
            iconWell.raycastTarget = false;
            Outline iconOutline = iconWell.gameObject.AddComponent<Outline>();
            iconOutline.effectColor = Hex("3c4544", 0.72f);
            iconOutline.effectDistance = new Vector2(1f, -1f);
            Image icon = AddImage("Icon", iconWell.transform, Color.white);
            icon.preserveAspect = true;
            icon.raycastTarget = false;
            Stretch(icon.rectTransform, 3f, 3f);
            Text iconFallback = AddText("Icon Fallback", iconWell.transform, "", 17, Hex("d7a84e", 1f), TextAnchor.MiddleCenter);
            iconFallback.fontStyle = FontStyle.Bold;
            iconFallback.raycastTarget = false;
            Stretch(iconFallback.rectTransform, 2f, 2f);

            Image hotkeyBackground = AddImage("Hotkey Keycap", button.transform, Hex("263035", 0.92f));
            hotkeyBackground.raycastTarget = false;
            Outline keyOutline = hotkeyBackground.gameObject.AddComponent<Outline>();
            keyOutline.effectColor = Hex("080b0d", 0.92f);
            keyOutline.effectDistance = new Vector2(1f, -1f);
            Text hotkey = AddText("Hotkey", hotkeyBackground.transform, "", 8, Hex("f3ead7", 1f), TextAnchor.MiddleCenter);
            hotkey.fontStyle = FontStyle.Bold;
            hotkey.raycastTarget = false;
            Stretch(hotkey.rectTransform, 1f, 1f);

            Text sub = AddText("Sub", button.transform, "", 9, Hex("b7aa90", 1f), TextAnchor.MiddleLeft);
            sub.resizeTextForBestFit = true;
            sub.resizeTextMinSize = 8;
            sub.resizeTextMaxSize = 9;
            sub.raycastTarget = false;
            Image accentRail = AddImage("Accent Rail", button.transform, Hex("58b7a5", 1f));
            accentRail.raycastTarget = false;
            accentRail.gameObject.SetActive(false);

            CombatHudCommandHoverRelay relay = button.gameObject.AddComponent<CombatHudCommandHoverRelay>();
            relay.Enter = () => SetHoveredCommand(index);
            relay.Exit = () => ClearHoveredCommand(index);
            return new CommandRow(button.GetComponent<RectTransform>(), button, outline, label, hotkey, sub, iconWell, iconOutline, icon, iconFallback, hotkeyBackground, accentRail);
        }

        private LogRow CreateLogRow(Transform parent, int index)
        {
            RectTransform root = AddPanel("Log Row " + index, parent, Hex("151b20", 0.82f), Hex("3c4544", 0.35f));
            Image stripe = AddImage("Stripe", root, Hex("7f9d5b", 1f));
            Text text = AddText("Text", root, "", 9, Hex("f3ead7", 1f), TextAnchor.UpperLeft);
            return new LogRow(root, stripe, text);
        }

        private void ApplyLayout(bool timelineExpanded, bool promoteEndTurn, bool showUndoMove, bool showCancelTarget)
        {
            lastWidth = Screen.width;
            lastHeight = Screen.height;
            lastTimelineExpanded = timelineExpanded;
            lastPromoteEndTurn = promoteEndTurn;
            lastShowUndoMove = showUndoMove;
            lastShowCancelTarget = showCancelTarget;
            CombatHudGeometry geometry = CombatHudScreenLayout.Calculate(Screen.width, Screen.height);
            SetScreenRect(topPanel, geometry.Top);
            SetScreenRect(sidePanel, geometry.Side);
            SetScreenRect(commandPanel, geometry.Command);

            float resourceW = Screen.width < 1240 ? 62f : 76f;
            float resourceGap = Screen.width < 1240 ? 5f : 8f;
            float resourcesW = resourceW * 3f + resourceGap * 2f;
            float resourcesX = geometry.Top.width - resourcesW - 10f;
            float titleX = 18f;
            float titleW = Mathf.Max(220f, resourcesX - titleX - 16f);
            SetLocalRect(titleText.rectTransform, new Rect(titleX, 6f, titleW * 0.36f, 27f));
            SetLocalRect(routeText.rectTransform, new Rect(titleX + 2f, 34f, titleW * 0.62f, 18f));
            SetLocalRect(phaseText.rectTransform, new Rect(titleX + titleW * 0.42f, 16f, titleW * 0.56f, 22f));
            SetLocalRect(goldText.rectTransform, new Rect(resourcesX, 8f, resourceW, 42f));
            SetLocalRect(suppliesText.rectTransform, new Rect(resourcesX + resourceW + resourceGap, 8f, resourceW, 42f));
            SetLocalRect(elixirsText.rectTransform, new Rect(resourcesX + (resourceW + resourceGap) * 2f, 8f, resourceW, 42f));

            CombatHudScreenLayout.SidePanels(geometry.Side, timelineExpanded, out Rect active, out Rect target, out Rect timeline);
            SetLocalRect(activePanel, active);
            SetLocalRect(targetPanel, target);
            SetLocalRect(timelinePanel, timeline);
            LayoutUnitCard(activePanel, active.width, active.height);
            LayoutUnitCard(targetPanel, target.width, target.height);
            SetLocalRect(timelineTitle.rectTransform, new Rect(12f, 8f, timeline.width - 100f, 24f));
            SetLocalRect(timelineButton.GetComponent<RectTransform>(), new Rect(timeline.width - 78f, 10f, 58f, 24f));
            SetLocalRect(
                turnQueueText.rectTransform,
                new Rect(12f, timelineExpanded ? 38f : 34f, timeline.width - 24f, timelineExpanded ? 40f : 28f));
            float logY = timelineExpanded ? 84f : 68f;
            float availableLogHeight = Mathf.Max(timelineExpanded ? 26f : 0f, timeline.height - logY - 8f);
            float logH = timelineExpanded
                ? Mathf.Clamp((availableLogHeight - 24f) / 5f, 14f, 40f)
                : availableLogHeight;
            for (int i = 0; i < logRows.Count; i++)
            {
                Rect row = timelineExpanded
                    ? new Rect(10f, logY + i * (logH + 6f), timeline.width - 20f, logH)
                    : new Rect(10f, logY, timeline.width - 20f, logH);
                SetLocalRect(logRows[i].Root, row);
                SetLocalRect(logRows[i].Stripe.rectTransform, new Rect(0f, 0f, 4f, row.height));
                SetLocalRect(logRows[i].Text.rectTransform, new Rect(10f, 5f, row.width - 16f, row.height - 10f));
            }

            Rect[] buttons = CombatHudScreenLayout.CommandButtons(geometry.Command.width, promoteEndTurn);
            SetLocalRect(commandPromptText.rectTransform, CombatHudScreenLayout.CommandPrompt(geometry.Command.width, showUndoMove, showCancelTarget));
            SetLocalRect(undoMoveButton.GetComponent<RectTransform>(), CombatHudScreenLayout.UndoMoveButton(geometry.Command.width, showCancelTarget));
            SetLocalRect(cancelTargetButton.GetComponent<RectTransform>(), CombatHudScreenLayout.CancelTargetButton(geometry.Command.width));
            if (buttons.Length >= 4)
            {
                float dividerX = (buttons[2].xMax + buttons[3].xMin) * 0.5f - 1f;
                SetLocalRect(commandDivider, new Rect(dividerX, 33f, 2f, 56f));
            }
            for (int i = 0; i < commandRows.Count && i < buttons.Length; i++)
            {
                SetLocalRect(commandRows[i].Root, buttons[i]);
                float iconSize = Mathf.Clamp(buttons[i].height - 20f, 40f, 48f);
                float textX = 10f + iconSize + 9f;
                SetLocalRect(commandRows[i].IconWell.rectTransform, new Rect(9f, 10f, iconSize, iconSize));
                SetLocalRect(commandRows[i].HotkeyBackground.rectTransform, new Rect(4f, 3f, 38f, 18f));
                SetLocalRect(commandRows[i].Label.rectTransform, new Rect(textX, 9f, Mathf.Max(36f, buttons[i].width - textX - 8f), 23f));
                SetLocalRect(commandRows[i].SubLabel.rectTransform, new Rect(textX, 34f, Mathf.Max(36f, buttons[i].width - textX - 8f), 23f));
                SetLocalRect(commandRows[i].AccentRail.rectTransform, new Rect(0f, buttons[i].height - 4f, buttons[i].width, 4f));
            }
        }

        private void LayoutUnitCard(RectTransform panel, float width, float height)
        {
            RectTransform mana = panel.Find("Mana Bg").GetComponent<RectTransform>();
            LayoutUnitCard(panel, width, height, mana.gameObject.activeSelf);
        }

        private void LayoutUnitCard(RectTransform panel, float width, float height, bool showMana)
        {
            CombatHudUnitCardGeometry geometry = CombatHudScreenLayout.UnitCard(width, height, showMana);
            SetLocalRect(panel.Find("Title").GetComponent<RectTransform>(), geometry.Title);
            SetLocalRect(panel.Find("Name").GetComponent<RectTransform>(), geometry.Name);
            SetLocalRect(panel.Find("Header").GetComponent<RectTransform>(), geometry.Header);
            SetLocalRect(panel.Find("State").GetComponent<RectTransform>(), geometry.State);
            SetLocalRect(panel.Find("Status").GetComponent<RectTransform>(), geometry.Status);
            SetLocalRect(panel.Find("Hp Bg").GetComponent<RectTransform>(), geometry.Hp);
            SetLocalRect(panel.Find("Mana Bg").GetComponent<RectTransform>(), geometry.Mana);
        }

        private void RefreshUnitCard(CombatHudUnitView unit, Text title, Text name, Text header, Text state, Text status, RectTransform hpFill, RectTransform manaFill, string fallbackTitle)
        {
            title.text = fallbackTitle;
            bool showMana = unit != null && unit.MaxMana > 0;
            RectTransform manaBackground = manaFill == null ? null : manaFill.parent as RectTransform;
            if (manaBackground != null) manaBackground.gameObject.SetActive(showMana);
            RectTransform panel = title == null ? null : title.rectTransform.parent as RectTransform;
            Outline panelOutline = panel == null ? null : panel.GetComponent<Outline>();
            if (panelOutline != null)
            {
                Color fallbackAccent = fallbackTitle == "Active Unit"
                    ? Hex("58b7a5", 0.82f)
                    : Hex("b94b56", 0.82f);
                panelOutline.effectColor = UnitAccent(unit?.AccentHex, fallbackAccent);
            }
            if (panel != null) LayoutUnitCard(panel, panel.rect.width, panel.rect.height, showMana);
            if (unit == null)
            {
                name.text = fallbackTitle == "Active Unit" ? "Waiting" : "Hover a unit";
                header.text = fallbackTitle == "Active Unit" ? "No active combatant." : "Inspect targets from the board.";
                state.text = "";
                status.text = "";
                SetFill(hpFill, 0, 1);
                SetFill(manaFill, 0, 1);
                SetMeterLabel(hpFill, "", 0, 0);
                SetMeterLabel(manaFill, "", 0, 0);
                return;
            }

            name.text = unit.Name ?? "";
            header.text = unit.Header ?? "";
            state.text = unit.StateLine ?? "";
            status.text = unit.StatusLine ?? "";
            SetFill(hpFill, unit.Hp, unit.MaxHp);
            SetFill(manaFill, unit.Mana, unit.MaxMana);
            SetMeterLabel(hpFill, "HP", unit.Hp, unit.MaxHp);
            SetMeterLabel(manaFill, "MP", unit.Mana, unit.MaxMana);
        }

        private void ToggleUtility()
        {
            utilityOpen = !utilityOpen;
            if (utilityPopup != null) utilityPopup.gameObject.SetActive(utilityOpen);
        }

        private void RunCommand(int index)
        {
            if (bindings?.View == null || bindings.RunCommand == null) return;
            CombatHudView view = bindings.View();
            if (view == null || index < 0 || index >= view.Commands.Count) return;
            utilityOpen = false;
            bindings.RunCommand(view.Commands[index].Mode);
        }

        private void RunUtility(ActionMode mode)
        {
            utilityOpen = false;
            bindings?.RunUtility?.Invoke(mode);
        }

        private void SetHoveredCommand(int index)
        {
            hoveredCommandIndex = index;
            CombatHudView view = bindings?.View?.Invoke();
            if (view != null) RefreshCommandPrompt(view);
        }

        private void ClearHoveredCommand(int index)
        {
            if (hoveredCommandIndex != index) return;
            hoveredCommandIndex = -1;
            CombatHudView view = bindings?.View?.Invoke();
            if (view != null) RefreshCommandPrompt(view);
        }

        private void RefreshCommandPrompt(CombatHudView view)
        {
            if (commandPromptText == null || view == null) return;
            string prompt = view.CommandPrompt ?? "";
            Color color = Hex("58b7a5", 1f);
            if (hoveredCommandIndex >= 0 && hoveredCommandIndex < view.Commands.Count)
            {
                CombatHudCommandView command = view.Commands[hoveredCommandIndex];
                string detail = command.Enabled ? command.Tooltip : command.DisabledReason;
                prompt = $"{command.Label} [{command.Hotkey}]  {detail}";
                color = command.Promoted ? Hex("d7a84e", 1f) : command.Enabled ? Hex("f3ead7", 1f) : Hex("b94b56", 1f);
            }
            else
            {
                for (int i = 0; i < view.Commands.Count; i++)
                {
                    if (view.Commands[i].Mode == ActionMode.Wait && view.Commands[i].Promoted)
                    {
                        color = Hex("d7a84e", 1f);
                        break;
                    }
                }
            }
            commandPromptText.text = prompt;
            commandPromptText.color = color;
        }

        private static string CommandFallbackGlyph(ActionMode mode)
        {
            switch (mode)
            {
                case ActionMode.Move: return "M";
                case ActionMode.Attack: return "A";
                case ActionMode.Cast: return "C";
                case ActionMode.Ability: return "S";
                case ActionMode.Guard: return "G";
                case ActionMode.Elixir: return "H";
                case ActionMode.Wait: return ">";
                default: return "?";
            }
        }

        private static string BuildTurnQueueText(IReadOnlyList<CombatHudTurnView> turns)
        {
            if (turns == null || turns.Count == 0) return "Initiative is forming...";
            List<string> labels = new List<string>();
            for (int i = 0; i < turns.Count; i++)
            {
                CombatHudTurnView turn = turns[i];
                if (turn == null) continue;
                string prefix = turn.StartsNextRound ? "\u21bb " : turn.Active ? "\u25b6 " : "";
                string color = string.IsNullOrWhiteSpace(turn.AccentHex) ? "b7aa90" : turn.AccentHex;
                labels.Add($"<color=#{color}>{prefix}{turn.Name}</color>");
            }
            return string.Join("  \u00b7  ", labels);
        }

        private Button AddButton(string name, Transform parent, string label, Action action, bool hero)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(Outline));
            go.transform.SetParent(parent, false);
            Image image = go.GetComponent<Image>();
            image.color = hero ? Hex("151b20", 0.96f) : Hex("151a1f", 0.96f);
            Button button = go.GetComponent<Button>();
            button.targetGraphic = image;
            ColorBlock colors = button.colors;
            colors.normalColor = image.color;
            colors.highlightedColor = hero ? Hex("243033", 1f) : Hex("232a31", 1f);
            colors.pressedColor = Hex("0b1013", 1f);
            colors.disabledColor = Hex("0b0f12", 0.70f);
            colors.selectedColor = colors.highlightedColor;
            button.colors = colors;
            Outline outline = go.GetComponent<Outline>();
            outline.effectColor = hero ? Hex("3c4544", 0.86f) : Hex("3c4544", 0.70f);
            outline.effectDistance = new Vector2(1f, -1f);
            if (action != null) button.onClick.AddListener(() => action());

            Text text = AddText("Label", go.transform, label, hero ? 13 : 12, Hex("f3ead7", 1f), TextAnchor.MiddleCenter);
            text.fontStyle = FontStyle.Bold;
            Stretch(text.rectTransform, 8f, 4f);
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
            if (size >= 18) text.fontStyle = FontStyle.Bold;
            return text;
        }

        private static void AddMeterLabelShadow(Text text)
        {
            if (text == null) return;
            Shadow shadow = text.gameObject.AddComponent<Shadow>();
            shadow.effectColor = Hex("050708", 0.95f);
            shadow.effectDistance = new Vector2(1f, -1f);
        }

        private static void SetMeterLabel(RectTransform fill, string prefix, int value, int max)
        {
            RectTransform background = fill == null ? null : fill.parent as RectTransform;
            Text label = background == null ? null : background.Find("Value")?.GetComponent<Text>();
            if (label == null) return;
            label.text = max > 0 ? $"{prefix} {Mathf.Max(0, value)}/{max}" : "";
        }

        private static Color UnitAccent(string accentHex, Color fallback)
        {
            if (string.IsNullOrWhiteSpace(accentHex)) return fallback;
            string html = accentHex.StartsWith("#", StringComparison.Ordinal) ? accentHex : "#" + accentHex;
            if (!ColorUtility.TryParseHtmlString(html, out Color accent)) return fallback;
            accent.a = fallback.a;
            return accent;
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

        private sealed class CommandRow
        {
            public readonly RectTransform Root;
            public readonly Button Button;
            public readonly Outline Outline;
            public readonly Text Label;
            public readonly Text Hotkey;
            public readonly Text SubLabel;
            public readonly Image IconWell;
            public readonly Outline IconOutline;
            public readonly Image Icon;
            public readonly Text IconFallback;
            public readonly Image HotkeyBackground;
            public readonly Image AccentRail;
            public ActionMode Mode;

            public CommandRow(RectTransform root, Button button, Outline outline, Text label, Text hotkey, Text subLabel, Image iconWell, Outline iconOutline, Image icon, Text iconFallback, Image hotkeyBackground, Image accentRail)
            {
                Root = root;
                Button = button;
                Outline = outline;
                Label = label;
                Hotkey = hotkey;
                SubLabel = subLabel;
                IconWell = iconWell;
                IconOutline = iconOutline;
                Icon = icon;
                IconFallback = iconFallback;
                HotkeyBackground = hotkeyBackground;
                AccentRail = accentRail;
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
