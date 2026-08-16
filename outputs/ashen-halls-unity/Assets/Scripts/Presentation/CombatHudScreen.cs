using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AshenHalls
{
    public enum CombatHudStateTone
    {
        Neutral,
        Ready,
        Blocked
    }

    public sealed class CombatHudUnitView
    {
        public string Name;
        public string Header;
        public string StateLine;
        public CombatHudStateTone StateTone;
        public string StatusLine;
        public string AccentHex;
        public Texture2D PortraitTexture;
        public Rect PortraitSource;
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
        public bool Armed;
        public bool Blocked;
        public bool Promoted;
    }

    public enum CombatHudCommandVisualState
    {
        Available,
        Selected,
        Armed,
        Blocked,
        Promoted
    }

    public static class CombatHudCommandStyleRules
    {
        // The existing command atlas reserves row 2, column 2 (zero-based cell 6)
        // for the bow. Keeping this mapping here avoids adding a new art slot.
        public const int ShootCommandAtlasIndex = 6;

        public static int AttackCommandAtlasIndex(bool rangedProfile, bool engaged)
        {
            return rangedProfile && !engaged
                ? ShootCommandAtlasIndex
                : CombatIconCatalog.CombatCommandAttackIndex;
        }

        public static CombatHudCommandVisualState Resolve(CombatHudCommandView command)
        {
            if (command == null || command.Blocked || !command.Enabled)
            {
                return CombatHudCommandVisualState.Blocked;
            }
            if (command.Armed) return CombatHudCommandVisualState.Armed;
            if (command.Promoted) return CombatHudCommandVisualState.Promoted;
            if (command.Selected) return CombatHudCommandVisualState.Selected;
            return CombatHudCommandVisualState.Available;
        }

        public static string StateTag(CombatHudCommandVisualState state)
        {
            switch (state)
            {
                case CombatHudCommandVisualState.Armed: return "ARMED";
                case CombatHudCommandVisualState.Promoted: return "NEXT";
                default: return "";
            }
        }

        public static string SecondaryLine(CombatHudCommandView command)
        {
            if (command == null) return "";
            CombatHudCommandVisualState state = Resolve(command);
            string sub = (command.SubLabel ?? "").Trim();
            string reason = (command.DisabledReason ?? "").Trim().TrimEnd('.');
            switch (state)
            {
                case CombatHudCommandVisualState.Armed:
                    return StartsWithState(sub, "ARMED") ? sub : "ARMED \u00b7 " + First(sub, "Choose a target");
                case CombatHudCommandVisualState.Blocked:
                    return First(reason, sub, "Unavailable");
                case CombatHudCommandVisualState.Promoted:
                    return "READY \u00b7 Next combatant";
                default:
                    return sub;
            }
        }

        private static bool StartsWithState(string value, string state)
        {
            return value.StartsWith(state, StringComparison.OrdinalIgnoreCase);
        }

        private static string First(params string[] values)
        {
            foreach (string value in values)
            {
                if (!string.IsNullOrWhiteSpace(value)) return value.Trim();
            }
            return "";
        }
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
        public Texture2D PortraitTexture;
        public Rect PortraitSource;
        public bool Active;
        public bool StartsNextRound;
    }

    public sealed class CombatHudView
    {
        public string Title;
        public string RouteLine;
        public string ObjectiveLine;
        public int LivingEnemyCount;
        public int LivingPartyCount;
        public int RoundNumber;
        public int MovePoints;
        public int MovePointsMaximum;
        public bool ActionReady;
        public string RoundLabel;
        public string MoveLabel;
        public string ActionLabel;
        // Retained for compatibility with callers that still use the combat view
        // as a compact campaign-state snapshot.
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
        public string TargetSourceLabel;
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
        public readonly Rect Command;
        public readonly Rect Board;
        public readonly Rect Side;

        public CombatHudGeometry(Rect top, Rect command, Rect board, Rect side)
        {
            Top = top;
            Command = command;
            Board = board;
            Side = side;
        }

        public bool Fits(float width, float height)
        {
            return FitsRect(Top, width, height)
                && FitsRect(Command, width, height)
                && FitsRect(Board, width, height)
                && FitsRect(Side, width, height)
                && Top.yMax <= Command.yMin
                && Mathf.Abs(Command.yMin - Board.yMin) < 0.01f
                && Mathf.Abs(Board.yMin - Side.yMin) < 0.01f
                && Mathf.Abs(Command.yMax - Board.yMax) < 0.01f
                && Mathf.Abs(Board.yMax - Side.yMax) < 0.01f
                && Command.xMax <= Board.xMin
                && Board.xMax <= Side.xMin;
        }

        private static bool FitsRect(Rect rect, float width, float height)
        {
            return rect.xMin >= 0f && rect.yMin >= 0f && rect.xMax <= width && rect.yMax <= height;
        }
    }

    public readonly struct CombatHudUnitCardGeometry
    {
        public readonly Rect Title;
        public readonly Rect Portrait;
        public readonly Rect Name;
        public readonly Rect Header;
        public readonly Rect State;
        public readonly Rect Hp;
        public readonly Rect Mana;
        public readonly Rect Status;
        public readonly bool ShowsMana;

        public CombatHudUnitCardGeometry(Rect title, Rect portrait, Rect name, Rect header, Rect state, Rect hp, Rect mana, Rect status, bool showsMana)
        {
            Title = title;
            Portrait = portrait;
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
                && FitsRect(Portrait, width, height)
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
            bool portraitOrdered = Title.yMax <= Portrait.yMin
                && Portrait.yMax <= Hp.yMin;
            return inside && ordered && portraitOrdered;
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
            return Mathf.Clamp(width * 0.17f, 272f, 326f);
        }

        public static float CommandPaletteWidth(float width)
        {
            return Mathf.Clamp(width * 0.06f, 96f, 112f);
        }

        public static float TopPanelHeight(float height)
        {
            return Mathf.Clamp(height * 0.05f, 50f, 56f);
        }

        public static CombatHudGeometry Calculate(float width, float height)
        {
            const float outer = 8f;
            const float gap = 8f;
            float topH = TopPanelHeight(height);
            Rect top = new Rect(outer, outer, width - outer * 2f, topH);
            float contentY = top.yMax + gap;
            float contentH = Mathf.Max(320f, height - contentY - outer);
            float commandW = CommandPaletteWidth(width);
            float sideW = SideRailWidth(width);
            Rect command = new Rect(outer, contentY, commandW, contentH);
            Rect side = new Rect(width - sideW - outer, contentY, sideW, contentH);
            float boardX = command.xMax + gap;
            Rect board = new Rect(boardX, contentY, Mathf.Max(320f, side.xMin - gap - boardX), contentH);
            return new CombatHudGeometry(top, command, board, side);
        }

        public static Rect BoardInner(Rect outer, int columns, int rows)
        {
            Rect inner = new Rect(outer.x + 14f, outer.y + 14f, outer.width - 28f, outer.height - 28f);
            float aspect = Mathf.Max(1f, columns) / Mathf.Max(1f, rows);
            if (inner.width / inner.height > aspect)
            {
                float fittedWidth = inner.height * aspect;
                inner.x += (inner.width - fittedWidth) * 0.5f;
                inner.width = fittedWidth;
            }
            else
            {
                float fittedHeight = inner.width / aspect;
                inner.y += (inner.height - fittedHeight) * 0.5f;
                inner.height = fittedHeight;
            }
            return inner;
        }

        public static Rect[] CommandButtons(float width, bool promoteEndTurn)
        {
            return CommandButtons(width, 640f, 6, promoteEndTurn);
        }

        public static Rect[] CommandButtons(float width, int commandCount, bool promoteEndTurn)
        {
            return CommandButtons(width, 640f, commandCount, promoteEndTurn);
        }

        public static Rect[] CommandButtons(float width, float panelHeight, bool promoteEndTurn)
        {
            return CommandButtons(width, panelHeight, 6, promoteEndTurn);
        }

        public static Rect[] CommandButtons(float width, float panelHeight, int commandCount, bool promoteEndTurn)
        {
            const float padding = 6f;
            const float gap = 5f;
            const float groupGap = 11f;
            commandCount = Mathf.Max(0, commandCount);
            if (commandCount == 0) return Array.Empty<Rect>();

            float endTurnBonus = promoteEndTurn ? 10f : 0f;
            int groupBreakIndex = CommandGroupBreakIndex(commandCount);
            int ordinaryGaps = commandCount - 1 - (groupBreakIndex >= 0 ? 1 : 0);
            float gapsHeight = gap * ordinaryGaps + (groupBreakIndex >= 0 ? groupGap : 0f);
            float availableHeight = Mathf.Max(1f, panelHeight - padding * 2f - gapsHeight - endTurnBonus);
            float buttonH = Mathf.Min(96f, availableHeight / commandCount);
            float groupHeight = buttonH * commandCount + gapsHeight + endTurnBonus;
            float buttonW = Mathf.Max(56f, width - padding * 2f);
            Rect[] rects = new Rect[commandCount];
            float y = Mathf.Max(padding, (panelHeight - groupHeight) * 0.5f);
            for (int i = 0; i < commandCount; i++)
            {
                float h = i == commandCount - 1 && promoteEndTurn ? buttonH + endTurnBonus : buttonH;
                rects[i] = new Rect(padding, y, buttonW, h);
                if (i + 1 < commandCount) y += h + (i == groupBreakIndex ? groupGap : gap);
            }
            return rects;
        }

        public static int CommandGroupBreakIndex(int commandCount)
        {
            return commandCount >= 4 ? Mathf.Min(2, commandCount - 2) : -1;
        }

        public static bool UsesCompactCommandLayout(Rect button)
        {
            return button.height < 86f;
        }

        public static float CommandIconSize(Rect button)
        {
            bool compact = UsesCompactCommandLayout(button);
            float labelReserve = compact ? 27f : 38f;
            float available = Mathf.Min(button.width - 20f, button.height - labelReserve);
            float minimum = compact
                ? Mathf.Clamp(button.height - labelReserve, 18f, 42f)
                : 52f;
            return Mathf.Clamp(available, minimum, 58f);
        }

        public static Rect CommandPrompt(float width, bool showUndoMove, bool showCancelTarget)
        {
            int contextButtons = (showUndoMove ? 1 : 0) + (showCancelTarget ? 1 : 0);
            float reserved = 12f + contextButtons * 102f;
            return new Rect(8f, 21f, Mathf.Max(120f, width - reserved), 18f);
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
            return new Rect(width - 100f - slotFromRight * 102f, 20f, 96f, 19f);
        }

        public static void SidePanels(Rect side, bool timelineExpanded, out Rect active, out Rect target, out Rect timeline)
        {
            const float gap = 8f;
            float activeH = Mathf.Clamp(side.height * 0.17f, 132f, 168f);
            float targetH = Mathf.Clamp(side.height * 0.20f, 150f, 196f);
            float timelineH = timelineExpanded
                ? Mathf.Clamp(side.height * 0.46f, 284f, 420f)
                : Mathf.Clamp(side.height * 0.19f, 126f, 154f);
            float total = activeH + targetH + timelineH + gap * 2f;
            if (total > side.height)
            {
                float over = total - side.height;
                float cardSpace = activeH + targetH;
                activeH -= over * (activeH / cardSpace);
                targetH -= over * (targetH / cardSpace);
            }

            active = new Rect(0f, 0f, side.width, activeH);
            target = new Rect(0f, active.yMax + gap, side.width, targetH);
            timeline = new Rect(0f, target.yMax + gap, side.width, timelineH);
        }

        public static CombatHudUnitCardGeometry UnitCard(float width, float height, bool showMana)
        {
            const float x = 12f;
            float fullWidth = Mathf.Max(1f, width - x * 2f);
            bool compact = height < 176f;
            bool veryCompact = height < 148f;
            float titleY = compact ? 6f : 8f;
            float titleH = veryCompact ? 18f : compact ? 20f : 24f;
            float contentTop = titleY + titleH + (compact ? 4f : 7f);
            float statusH = veryCompact ? 14f : compact ? 16f : 18f;
            float statusY = Mathf.Max(0f, height - (compact ? 7f : 10f) - statusH);
            float meterBottom = statusY - (compact ? 4f : 6f);
            float manaH = veryCompact ? 13f : 15f;
            float manaY = meterBottom - manaH;
            float hpH = veryCompact ? 15f : 18f;
            float hpY = (showMana ? manaY - 4f : meterBottom) - hpH;
            float desiredPortrait = Mathf.Clamp(
                Mathf.Min(width * 0.30f, height * (compact ? 0.42f : 0.46f)),
                44f,
                132f);
            float portraitSize = Mathf.Max(36f, Mathf.Min(desiredPortrait, hpY - contentTop - 2f));
            Rect portrait = new Rect(x, contentTop, portraitSize, portraitSize);
            float textX = portrait.xMax + (compact ? 8f : 11f);
            float textWidth = Mathf.Max(1f, width - textX - x);
            float nameY = contentTop;
            float nameH = veryCompact ? 18f : compact ? 20f : 23f;
            float headerY = nameY + nameH + 1f;
            float headerH = veryCompact ? 13f : compact ? 15f : 18f;
            float stateY = headerY + headerH + 2f;
            float stateH = Mathf.Max(0f, Mathf.Min(compact ? 28f : 36f, hpY - stateY - 3f));

            return new CombatHudUnitCardGeometry(
                new Rect(x, titleY, fullWidth, titleH),
                portrait,
                new Rect(textX, nameY, textWidth, nameH),
                new Rect(textX, headerY, textWidth, headerH),
                new Rect(textX, stateY, textWidth, stateH),
                new Rect(x, hpY, fullWidth, hpH),
                new Rect(x, manaY, fullWidth, manaH),
                new Rect(x, statusY, fullWidth, statusH),
                showMana);
        }
    }

    internal sealed class CombatHudCommandHoverRelay :
        MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        ISelectHandler,
        IDeselectHandler
    {
        public Action Enter;
        public Action Exit;
        public Action<BaseEventData> Select;
        public Action Deselect;

        public void OnPointerEnter(PointerEventData eventData)
        {
            Enter?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Exit?.Invoke();
        }

        public void OnSelect(BaseEventData eventData)
        {
            Select?.Invoke(eventData);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            Deselect?.Invoke();
        }
    }

    public sealed class CombatHudScreen : MonoBehaviour
    {
        private readonly List<CommandRow> commandRows = new List<CommandRow>();
        private readonly List<LogRow> logRows = new List<LogRow>();
        private readonly List<TurnChip> turnChips = new List<TurnChip>();
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
        private RectTransform phaseBackplate;
        private RectTransform roundStatBackplate;
        private RectTransform moveStatBackplate;
        private RectTransform actionStatBackplate;
        private RectTransform commandPromptBackplate;
        private RectTransform commandPromptRail;
        private Text titleText;
        private Text routeText;
        private Text phaseText;
        private Text roundStatText;
        private Text moveStatText;
        private Text actionStatText;
        private Text activeTitle;
        private Text activeName;
        private Text activeHeader;
        private Text activeState;
        private Text activeStatus;
        private Image activePortrait;
        private Text activePortraitFallback;
        private RectTransform activeHpFill;
        private RectTransform activeManaFill;
        private Text targetTitle;
        private Text targetName;
        private Text targetHeader;
        private Text targetState;
        private Text targetStatus;
        private Image targetPortrait;
        private Text targetPortraitFallback;
        private RectTransform targetHpFill;
        private RectTransform targetManaFill;
        private Text timelineTitle;
        private Text turnQueueText;
        private RectTransform tacticalPlanPanel;
        private Text tacticalPlanText;
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
        private int lastCommandCount = -1;
        private int expectedCommandCount = 6;
        private int displayedRoundNumber;
        private int displayedMovePoints;
        private int displayedMovePointsMaximum;
        private bool displayedActionReady;
        private int hoveredCommandIndex = -1;
        private int focusedCommandIndex = -1;
        private bool pointerOwnsCommandContext;

        public bool IsReady => canvas != null
            && canvasGroup != null
            && topPanel != null
            && sidePanel != null
            && activePanel != null
            && targetPanel != null
            && timelinePanel != null
            && activePortrait != null
            && targetPortrait != null
            && turnQueueText != null
            && commandPanel != null
            && commandPromptText != null
            && roundStatText != null
            && moveStatText != null
            && actionStatText != null
            && undoMoveButton != null
            && cancelTargetButton != null
            && utilityPopup != null
            && timelineButton != null
            && utilityButton != null
            && guardButton != null
            && elixirButton != null
            && menuButton != null
            && commandRows.Count > 0
            && turnChips.Count == 6
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
                if (!commandPanel.gameObject.activeInHierarchy
                    || rect.width < 88f
                    || rect.height < 320f
                    || expectedCommandCount <= 0)
                {
                    return false;
                }

                int visible = 0;
                foreach (CommandRow row in commandRows)
                {
                    if (row?.Root == null || !row.Root.gameObject.activeInHierarchy) continue;
                    Rect hitTarget = row.Root.rect;
                    if (hitTarget.width < 56f || hitTarget.height < 56f) return false;
                    visible++;
                }
                return visible == expectedCommandCount;
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

        public bool OwnsSelection(GameObject selected)
        {
            if (!IsVisible || selected == null || canvas == null) return false;
            Transform selectedTransform = selected.transform;
            Transform canvasTransform = canvas.transform;
            return selectedTransform == canvasTransform || selectedTransform.IsChildOf(canvasTransform);
        }

        public void InvokeCommandForTest(ActionMode mode)
        {
            CommandRow row = commandRows.Find(candidate => candidate != null && candidate.Mode == mode);
            if (row == null || !row.Button.interactable) throw new InvalidOperationException($"Combat command {mode} is not ready.");
            row.Button.onClick.Invoke();
        }

        public void InvokePointerCommandForTest(ActionMode mode)
        {
            CommandRow row = commandRows.Find(candidate => candidate != null && candidate.Mode == mode);
            if (row == null || !row.Button.interactable) throw new InvalidOperationException($"Combat command {mode} is not ready.");
            EventSystem eventSystem = EventSystem.current
                ?? (!Application.isPlaying ? UiRuntime.EnsureEventSystemReady() : null);
            if (eventSystem == null) throw new InvalidOperationException("Combat pointer command needs an EventSystem.");
            PointerEventData pointer = new PointerEventData(eventSystem) { button = PointerEventData.InputButton.Left };
            CombatHudCommandHoverRelay relay = row.Button.GetComponent<CombatHudCommandHoverRelay>();
            if (relay == null) throw new InvalidOperationException($"Combat command {mode} is missing its pointer relay.");
            eventSystem.SetSelectedGameObject(null, pointer);
            relay.OnPointerEnter(pointer);
            eventSystem.SetSelectedGameObject(row.Button.gameObject, pointer);
            row.Button.onClick.Invoke();
        }

        public void HoverCommandForTest(ActionMode mode)
        {
            int index = commandRows.FindIndex(candidate => candidate != null
                && candidate.Mode == mode
                && candidate.Root != null
                && candidate.Root.gameObject.activeInHierarchy);
            if (index < 0) throw new InvalidOperationException($"Combat command {mode} is not visible.");
            SetHoveredCommand(index);
        }

        public void ClearCommandHoverForTest()
        {
            if (hoveredCommandIndex < 0) return;
            ClearHoveredCommand(hoveredCommandIndex);
        }

        public void ClearCommandFocusForTest()
        {
            if (focusedCommandIndex < 0) return;
            ClearFocusedCommand(focusedCommandIndex);
        }

        public string CommandPromptForTest => commandPromptText == null ? "" : commandPromptText.text;
        public bool PointerOwnsCommandContextForTest => pointerOwnsCommandContext;
        public string RoundLabelForTest => roundStatText == null ? "" : roundStatText.text;
        public string MoveLabelForTest => moveStatText == null ? "" : moveStatText.text;
        public string ActionLabelForTest => actionStatText == null ? "" : actionStatText.text;
        public int RoundNumberForTest => displayedRoundNumber;
        public int MovePointsForTest => displayedMovePoints;
        public int MovePointsMaximumForTest => displayedMovePointsMaximum;
        public bool ActionReadyForTest => displayedActionReady;
        public int CommandCapacityForTest => commandRows.Count;
        public bool ActivePortraitVisibleForTest => activePortrait != null && activePortrait.enabled && activePortrait.sprite != null;
        public bool TargetPortraitVisibleForTest => targetPortrait != null && targetPortrait.enabled && targetPortrait.sprite != null;
        public string ActiveCardTitleForTest => activeTitle == null ? "" : activeTitle.text;
        public string TargetCardTitleForTest => targetTitle == null ? "" : targetTitle.text;
        public bool MenuVisibleForTest => menuButton != null && menuButton.gameObject.activeInHierarchy;
        public int VisibleTurnChipCountForTest
        {
            get
            {
                int visible = 0;
                foreach (TurnChip chip in turnChips)
                {
                    if (chip?.Root != null && chip.Root.gameObject.activeInHierarchy) visible++;
                }
                return visible;
            }
        }

        public bool CommandInputSelectableForTest(ActionMode mode)
        {
            CommandRow row = commandRows.Find(candidate => candidate != null
                && candidate.Mode == mode
                && candidate.Root != null
                && candidate.Root.gameObject.activeInHierarchy);
            return row?.Button != null && row.Button.interactable;
        }

        public Color CommandSelectedMultiplierForTest(ActionMode mode)
        {
            CommandRow row = commandRows.Find(candidate => candidate != null && candidate.Mode == mode);
            return row?.Button == null ? Color.clear : row.Button.colors.selectedColor;
        }

        public float CommandIconSizeForTest(ActionMode mode)
        {
            CommandRow row = commandRows.Find(candidate => candidate != null && candidate.Mode == mode);
            return row?.IconWell == null ? 0f : Mathf.Min(row.IconWell.rectTransform.rect.width, row.IconWell.rectTransform.rect.height);
        }

        public Texture2D CommandSpriteTextureForTest(ActionMode mode)
        {
            CommandRow row = commandRows.Find(candidate => candidate != null && candidate.Mode == mode);
            return row?.Icon?.sprite == null ? null : row.Icon.sprite.texture;
        }

        public string CommandLabelForTest(ActionMode mode)
        {
            CommandRow row = commandRows.Find(candidate => candidate != null && candidate.Mode == mode);
            return row?.Label == null ? "" : row.Label.text;
        }

        public string CommandSubLabelForTest(ActionMode mode)
        {
            CommandRow row = commandRows.Find(candidate => candidate != null && candidate.Mode == mode);
            return row?.SubLabel == null ? "" : row.SubLabel.text;
        }

        public bool CommandUsesBlockedStyleForTest(ActionMode mode)
        {
            CommandRow row = commandRows.Find(candidate => candidate != null && candidate.Mode == mode);
            Image fill = row?.Button?.targetGraphic as Image;
            return row != null
                && fill != null
                && fill.color == Hex("0c1012", 0.76f)
                && row.Label.color == Hex("b8aea5", 0.76f)
                && row.SubLabel.color == Hex("8d9495", 0.82f)
                && row.StateTagPanel != null
                && !row.StateTagPanel.gameObject.activeSelf
                && !row.StatePip.gameObject.activeSelf;
        }

        public string CommandStateTagForTest(ActionMode mode)
        {
            CommandRow row = commandRows.Find(candidate => candidate != null && candidate.Mode == mode);
            return row?.StateTag == null || !row.StateTag.gameObject.activeSelf ? "" : row.StateTag.text;
        }

        public ActionMode? FocusedCommandForTest
        {
            get
            {
                if (focusedCommandIndex < 0 || focusedCommandIndex >= commandRows.Count) return null;
                CommandRow row = commandRows[focusedCommandIndex];
                return row?.Root != null && row.Root.gameObject.activeInHierarchy
                    ? row.Mode
                    : (ActionMode?)null;
            }
        }

        public ActionMode? HoveredCommandForTest => VisibleCommandModeForIndex(hoveredCommandIndex);
        public ActionMode? ContextCommandForTest => VisibleCommandModeForIndex(ContextualCommandIndex());
        public bool PointerOwnsCommandContext => pointerOwnsCommandContext;

        public bool HasFocusedCommand(ActionMode mode)
        {
            return FocusedCommandForTest == mode;
        }

        public void FocusCommand(ActionMode mode)
        {
            if (!IsVisible) return;
            EventSystem eventSystem = EventSystem.current;
            if (eventSystem == null && !Application.isPlaying)
            {
                // Unity does not register EventSystem.current while the editor
                // boot smoke invokes this screen outside Play Mode. Reuse the
                // editor-only system retained by UiRuntime so selection events
                // still exercise the same focus and deselect path as runtime.
                eventSystem = UiRuntime.EnsureEventSystemReady();
            }
            int index = commandRows.FindIndex(candidate => candidate != null
                && candidate.Mode == mode
                && candidate.Root != null
                && candidate.Root.gameObject.activeInHierarchy
                && candidate.Button != null);
            if (index < 0) return;
            CommandRow row = commandRows[index];
            SetFocusedCommand(index);
            if (eventSystem != null)
            {
                eventSystem.SetSelectedGameObject(row.Button.gameObject);
            }
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
            EventSystem eventSystem = EventSystem.current;
            if (eventSystem == null && !Application.isPlaying) eventSystem = UiRuntime.EnsureEventSystemReady();
            GameObject selected = eventSystem == null ? null : eventSystem.currentSelectedGameObject;
            bool ownedSelection = IsCanvasSelection(selected);
            bool changed = UiRuntime.SetCanvasVisible(canvas, visible);
            if (changed)
            {
                ClearTransientCommandContext();
                if (ownedSelection && eventSystem != null) eventSystem.SetSelectedGameObject(null);
                if (visible)
                {
                    lastWidth = -1f;
                    lastHeight = -1f;
                }
                CombatHudView view = bindings?.View?.Invoke();
                if (view != null) RefreshCommandPrompt(view);
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

            IReadOnlyList<CombatHudCommandView> commands = view.Commands ?? Array.Empty<CombatHudCommandView>();
            EnsureCommandRowCount(commands.Count);
            expectedCommandCount = commands.Count;
            bool commandContextAvailable = view.PlayerTurn;
            if (commandContextAvailable)
            {
                commandContextAvailable = false;
                for (int i = 0; i < commands.Count; i++)
                {
                    if (commands[i] != null && commands[i].Enabled)
                    {
                        commandContextAvailable = true;
                        break;
                    }
                }
            }
            if (!commandContextAvailable)
            {
                focusedCommandIndex = -1;
                hoveredCommandIndex = -1;
                pointerOwnsCommandContext = false;
                GameObject selectedObject = EventSystem.current == null
                    ? null
                    : EventSystem.current.currentSelectedGameObject;
                if (selectedObject != null)
                {
                    for (int i = 0; i < commandRows.Count; i++)
                    {
                        if (commandRows[i]?.Button != null
                            && commandRows[i].Button.gameObject == selectedObject)
                        {
                            EventSystem.current.SetSelectedGameObject(null);
                            break;
                        }
                    }
                }
            }
            bool promoteEndTurn = false;
            for (int i = 0; i < commands.Count; i++)
            {
                if (commands[i].Mode == ActionMode.Wait && commands[i].Promoted) promoteEndTurn = true;
            }

            if (!Mathf.Approximately(lastWidth, Screen.width)
                || !Mathf.Approximately(lastHeight, Screen.height)
                || lastTimelineExpanded != view.TimelineExpanded
                || lastPromoteEndTurn != promoteEndTurn
                || lastShowUndoMove != view.CanUndoMove
                || lastShowCancelTarget != view.CanCancelTarget
                || lastCommandCount != commands.Count)
            {
                ApplyLayout(view.TimelineExpanded, promoteEndTurn, view.CanUndoMove, view.CanCancelTarget, commands.Count);
            }

            titleText.text = string.IsNullOrEmpty(view.Title) ? VersionInfo.ProductName : view.Title;
            routeText.text = string.IsNullOrWhiteSpace(view.ObjectiveLine)
                ? view.RouteLine ?? ""
                : view.ObjectiveLine;
            phaseText.text = view.PhaseLine ?? "";
            displayedRoundNumber = view.RoundNumber;
            displayedMovePoints = view.MovePoints;
            displayedMovePointsMaximum = view.MovePointsMaximum;
            displayedActionReady = view.ActionReady;
            phaseText.color = view.ActiveUnit == null
                ? Hex("b7aa90", 1f)
                : view.PlayerTurn ? Hex("58b7a5", 1f) : Hex("c65c3b", 1f);
            if (phaseBackplate != null)
            {
                Image phaseFill = phaseBackplate.GetComponent<Image>();
                Color phaseAccent = view.ActiveUnit == null
                    ? Hex("b7aa90", 1f)
                    : view.PlayerTurn ? Hex("58b7a5", 1f) : Hex("c65c3b", 1f);
                if (phaseFill != null) phaseFill.color = Color.Lerp(Hex("0a1114", 0.72f), phaseAccent, 0.10f);
                Outline phaseOutline = phaseBackplate.GetComponent<Outline>();
                if (phaseOutline != null) phaseOutline.effectColor = phaseAccent.WithAlpha(0.58f);
            }
            roundStatText.text = string.IsNullOrWhiteSpace(view.RoundLabel)
                ? "ROUND\n" + (view.RoundNumber > 0 ? view.RoundNumber.ToString() : "-")
                : view.RoundLabel;
            moveStatText.text = string.IsNullOrWhiteSpace(view.MoveLabel)
                ? "MOVE\n" + (view.MovePointsMaximum > 0 ? $"{view.MovePoints} / {view.MovePointsMaximum}" : "-")
                : view.MoveLabel;
            actionStatText.text = string.IsNullOrWhiteSpace(view.ActionLabel)
                ? "ACTION\n" + (view.ActionReady ? "READY" : "USED")
                : view.ActionLabel;
            moveStatText.color = view.PlayerTurn && view.MovePoints > 0 ? Hex("58b7a5", 1f) : Hex("b7aa90", 1f);
            actionStatText.color = view.ActiveUnit == null
                ? Hex("b7aa90", 1f)
                : view.ActionReady
                    ? Hex("58b7a5", 1f)
                    : view.PlayerTurn ? Hex("d7a84e", 1f) : Hex("c65c3b", 1f);
            RefreshStatBackplate(roundStatBackplate, Hex("d7a84e", 1f), true);
            RefreshStatBackplate(moveStatBackplate, moveStatText.color, view.PlayerTurn && view.MovePoints > 0);
            RefreshStatBackplate(actionStatBackplate, actionStatText.color, view.ActionReady);
            timelineTitle.text = string.IsNullOrEmpty(view.RoundLine) ? "Timeline" : "Timeline / " + view.RoundLine;
            timelineButtonText.text = view.TimelineExpanded ? "Less" : "More";
            turnQueueText.text = view.Turns == null || view.Turns.Count == 0
                ? "INITIATIVE FORMING"
                : "INITIATIVE  /  NEXT SIX";
            RefreshTurnChips(view.Turns);
            tacticalPlanText.text = string.IsNullOrWhiteSpace(view.TacticalLine)
                ? "TACTICAL READ  /  Hover a unit or tile to inspect danger and outcomes."
                : "OPENING PLAN  /  " + view.TacticalLine;

            RefreshUnitCard(view.ActiveUnit, activeTitle, activeName, activeHeader, activeState, activeStatus, activePortrait, activePortraitFallback, activeHpFill, activeManaFill, "ACTIVE UNIT");
            string targetContext = string.IsNullOrWhiteSpace(view.TargetTitle) ? "UNIT" : view.TargetTitle.ToUpperInvariant();
            string targetSource = string.IsNullOrWhiteSpace(view.TargetSourceLabel) ? "INSPECT" : view.TargetSourceLabel.ToUpperInvariant();
            string repeatedSourcePrefix = targetSource + " ";
            if (targetContext.StartsWith(repeatedSourcePrefix, StringComparison.Ordinal))
            {
                targetContext = targetContext.Substring(repeatedSourcePrefix.Length);
            }
            string targetCardTitle = view.TargetUnit == null
                ? "INSPECT UNIT"
                : targetSource + "  /  " + targetContext;
            RefreshUnitCard(
                view.TargetUnit,
                targetTitle,
                targetName,
                targetHeader,
                targetState,
                targetStatus,
                targetPortrait,
                targetPortraitFallback,
                targetHpFill,
                targetManaFill,
                targetCardTitle);

            for (int i = 0; i < commandRows.Count; i++)
            {
                bool visible = i < commands.Count;
                commandRows[i].Root.gameObject.SetActive(visible);
                if (!visible) continue;
                CombatHudCommandView command = commands[i];
                commandRows[i].Mode = command.Mode;
                // Disabled commands remain focusable so keyboard/controller players can
                // inspect the exact reason and submit them for the same blocked feedback
                // used by mouse input. The gameplay binding remains the authority.
                commandRows[i].Button.interactable = true;
                commandRows[i].Label.text = command.Promoted ? (command.Label ?? "").ToUpperInvariant() : command.Label ?? "";
                commandRows[i].Hotkey.text = command.Hotkey ?? "";
                CombatHudCommandVisualState visualState = CombatHudCommandStyleRules.Resolve(command);
                commandRows[i].SubLabel.text = CombatHudCommandStyleRules.SecondaryLine(command);
                Sprite icon = UiRuntime.AtlasSprite(command.IconTexture, command.IconSource);
                commandRows[i].Icon.sprite = icon;
                commandRows[i].Icon.enabled = icon != null;
                commandRows[i].IconFallback.gameObject.SetActive(icon == null);
                commandRows[i].IconFallback.text = CommandFallbackGlyph(command.Mode);
                bool available = command.Enabled && !command.Blocked;
                commandRows[i].Icon.color = Color.white.WithAlpha(available ? 0.98f : 0.60f);
                bool focused = focusedCommandIndex == i;
                bool hovered = pointerOwnsCommandContext && hoveredCommandIndex == i;
                Color accent = command.Promoted || command.Armed
                    ? Hex("d7a84e", 1f)
                    : CommandAccent(command.Mode);
                commandRows[i].IconWell.color = command.Promoted || command.Armed
                    ? Hex("211809", 0.94f)
                    : command.Selected && available
                        ? Color.Lerp(Hex("080b0d", 0.92f), accent, 0.18f)
                        : Color.Lerp(Hex("080b0d", 0.84f), accent, available ? 0.08f : 0.02f);
                commandRows[i].IconOutline.effectColor = accent.WithAlpha(
                    command.Promoted || command.Armed || command.Selected && available ? 0.95f : focused || hovered ? 0.82f : available ? 0.56f : 0.22f);
                commandRows[i].HotkeyBackground.color = command.Promoted || command.Armed || command.Selected && available
                    ? accent.WithAlpha(0.94f)
                    : Hex("263035", available ? 0.92f : 0.48f);
                commandRows[i].Hotkey.color = command.Promoted || command.Armed || command.Selected && available
                    ? Hex("080b0d", 1f)
                    : available ? Hex("f3ead7", 1f) : Hex("777c7c", 0.82f);
                commandRows[i].Label.color = visualState == CombatHudCommandVisualState.Blocked
                    ? Hex("b8aea5", 0.76f)
                    : available ? Hex("f3ead7", 1f) : Hex("9aa0a1", 0.88f);
                commandRows[i].SubLabel.color = visualState == CombatHudCommandVisualState.Blocked
                    ? Hex("8d9495", 0.82f)
                    : available ? command.Promoted ? Hex("d7a84e", 1f) : Hex("c7baa2", 1f) : Hex("8d9495", 0.82f);
                commandRows[i].StatePip.color = visualState == CombatHudCommandVisualState.Blocked
                    ? Hex("b94b56", 0.78f)
                    : available
                    ? accent.WithAlpha(command.Promoted || command.Armed || command.Selected ? 1f : 0.72f)
                    : Hex("777c7c", 0.42f);
                commandRows[i].StatePip.gameObject.SetActive(
                    visualState == CombatHudCommandVisualState.Armed
                    || visualState == CombatHudCommandVisualState.Promoted);
                string stateTag = CombatHudCommandStyleRules.StateTag(visualState);
                commandRows[i].StateTag.text = stateTag;
                commandRows[i].StateTagPanel.gameObject.SetActive(!string.IsNullOrWhiteSpace(stateTag));
                Color stateColor = visualState == CombatHudCommandVisualState.Blocked
                    ? Hex("b94b56", 1f)
                    : Hex("d7a84e", 1f);
                commandRows[i].StateTagPanel.GetComponent<Image>().color = stateColor.WithAlpha(0.96f);
                commandRows[i].StateTagPanel.GetComponent<Outline>().effectColor = Hex("050708", 0.96f);
                commandRows[i].StateTag.color = visualState == CombatHudCommandVisualState.Blocked
                    ? Hex("fff0e8", 1f)
                    : Hex("080b0d", 1f);
                commandRows[i].AccentRail.gameObject.SetActive(command.Promoted || command.Armed || command.Selected && available);
                commandRows[i].AccentRail.color = accent.WithAlpha(
                    command.Promoted || command.Armed || command.Selected && available ? 1f : available ? 0.44f : 0.16f);
                Image image = commandRows[i].Button.targetGraphic as Image;
                Color fill = visualState == CombatHudCommandVisualState.Blocked
                    ? Hex("0c1012", 0.76f)
                    : command.Promoted || command.Armed
                    ? Hex("352316", 0.94f)
                    : command.Selected && available
                        ? Color.Lerp(Hex("151b20", 0.92f), accent, 0.14f)
                        : Hex("151b20", 0.82f);
                if (image != null) image.color = fill;
                ColorBlock buttonColors = commandRows[i].Button.colors;
                // Button colors are multipliers. Keep semantic fill on the Image so
                // selection/focus cannot multiply it into a muddy or stale color.
                buttonColors.normalColor = Color.white;
                buttonColors.highlightedColor = Color.white;
                buttonColors.selectedColor = Color.white;
                buttonColors.pressedColor = new Color(0.82f, 0.82f, 0.82f, 1f);
                buttonColors.disabledColor = Color.white;
                commandRows[i].Button.colors = buttonColors;
                commandRows[i].Outline.effectColor = focused
                    ? Hex("f3ead7", 0.98f)
                    : visualState == CombatHudCommandVisualState.Blocked
                        ? Hex("3c4544", 0.20f)
                    : command.Promoted || command.Armed || command.Selected && available
                        ? accent.WithAlpha(0.95f)
                        : available ? Hex("3c4544", 0.34f) : Hex("3c4544", 0.18f);
                float outlineSize = focused ? 3f : command.Promoted || command.Armed || command.Selected && available ? 2f : 1f;
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
            menuButton.gameObject.SetActive(true);
            menuButton.interactable = true;
            guardButton.interactable = view.GuardEnabled;
            elixirButton.interactable = view.ElixirEnabled;
            guardText.text = view.GuardEnabled ? "Guard\nG" : "Guard\n" + (view.GuardReason ?? "");
            elixirText.text = view.ElixirEnabled ? "Elixir\nH" : "Elixir\n" + (view.ElixirReason ?? "");
            menuText.text = "Menu  [Esc]";

            IReadOnlyList<CombatHudLogView> logs = view.Logs ?? Array.Empty<CombatHudLogView>();
            int visibleLogs = view.TimelineExpanded
                ? Mathf.Min(logRows.Count, logs.Count)
                : 0;
            for (int i = 0; i < logRows.Count; i++)
            {
                bool visible = i < visibleLogs;
                logRows[i].Root.gameObject.SetActive(visible);
                if (!visible) continue;
                logRows[i].Text.text = logs[i].Text ?? "";
                logRows[i].Stripe.color = ToneColor(logs[i].Tone);
            }
            ReconcileHiddenSelection();
        }

        private void ReconcileHiddenSelection()
        {
            EventSystem eventSystem = EventSystem.current;
            if (eventSystem == null && !Application.isPlaying) eventSystem = UiRuntime.EnsureEventSystemReady();
            GameObject selected = eventSystem == null ? null : eventSystem.currentSelectedGameObject;
            if (selected == null || !IsCanvasSelection(selected)) return;
            if (!selected.activeInHierarchy)
            {
                eventSystem.SetSelectedGameObject(null);
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

            topPanel = AddPanel("Top Chrome", canvas.transform, Hex("10161b", 0.82f), Hex("3c4544", 0.54f));
            phaseBackplate = AddPanel("Phase Backplate", topPanel, Hex("0a1114", 0.64f), Hex("3c4544", 0.36f));
            roundStatBackplate = AddPanel("Round Backplate", topPanel, Hex("151b20", 0.78f), Hex("d7a84e", 0.34f));
            moveStatBackplate = AddPanel("Move Backplate", topPanel, Hex("151b20", 0.78f), Hex("58b7a5", 0.34f));
            actionStatBackplate = AddPanel("Action Backplate", topPanel, Hex("151b20", 0.78f), Hex("58b7a5", 0.34f));
            titleText = AddText("Title", topPanel, VersionInfo.ProductName, 21, Hex("f3ead7", 1f), TextAnchor.MiddleLeft);
            titleText.resizeTextForBestFit = true;
            titleText.resizeTextMinSize = 14;
            titleText.resizeTextMaxSize = 21;
            routeText = AddText("Route", topPanel, "", 9, Hex("b7aa90", 0.94f), TextAnchor.MiddleLeft);
            menuButton = AddButton("Menu", topPanel, "Menu  [Esc]", () => bindings?.OpenMenu?.Invoke(), false);
            menuText = menuButton.GetComponentInChildren<Text>();
            menuText.fontSize = 9;
            menuText.color = Hex("f3ead7", 1f);
            phaseText = AddText("Phase", topPanel, "", 11, Hex("58b7a5", 1f), TextAnchor.MiddleCenter);
            phaseText.fontStyle = FontStyle.Bold;
            phaseText.resizeTextForBestFit = true;
            phaseText.resizeTextMinSize = 9;
            phaseText.resizeTextMaxSize = 11;
            roundStatText = AddText("Round", topPanel, "", 10, Hex("d7a84e", 1f), TextAnchor.MiddleCenter);
            moveStatText = AddText("Move", topPanel, "", 10, Hex("58b7a5", 1f), TextAnchor.MiddleCenter);
            actionStatText = AddText("Action", topPanel, "", 10, Hex("58b7a5", 1f), TextAnchor.MiddleCenter);
            roundStatText.fontStyle = FontStyle.Bold;
            moveStatText.fontStyle = FontStyle.Bold;
            actionStatText.fontStyle = FontStyle.Bold;

            sidePanel = AddPanel("Combat Side", canvas.transform, Hex("0e1114", 0.04f), Hex("0e1114", 0f));
            activePanel = AddPanel("Active", sidePanel, Hex("151c21", 0.86f), Hex("58b7a5", 0.62f));
            targetPanel = AddPanel("Target", sidePanel, Hex("151c21", 0.86f), Hex("b94b56", 0.62f));
            timelinePanel = AddPanel("Timeline", sidePanel, Hex("151c21", 0.84f), Hex("d7a84e", 0.58f));
            BuildUnitCard(activePanel, out activeTitle, out activeName, out activeHeader, out activeState, out activeStatus, out activePortrait, out activePortraitFallback, out activeHpFill, out activeManaFill);
            BuildUnitCard(targetPanel, out targetTitle, out targetName, out targetHeader, out targetState, out targetStatus, out targetPortrait, out targetPortraitFallback, out targetHpFill, out targetManaFill);
            timelineTitle = AddText("Timeline Title", timelinePanel, "Timeline", 18, Hex("d7a84e", 1f), TextAnchor.MiddleLeft);
            timelineButton = AddButton("Timeline Toggle", timelinePanel, "Show", () => bindings?.ToggleTimeline?.Invoke(), false);
            timelineButtonText = timelineButton.GetComponentInChildren<Text>();
            turnQueueText = AddText("Turn Queue", timelinePanel, "INITIATIVE", 9, Hex("b7aa90", 1f), TextAnchor.MiddleLeft);
            turnQueueText.fontStyle = FontStyle.Bold;
            turnQueueText.supportRichText = true;
            for (int i = 0; i < 6; i++) turnChips.Add(CreateTurnChip(timelinePanel, i));
            tacticalPlanPanel = AddPanel("Tactical Read", timelinePanel, Hex("11191d", 0.92f), Hex("58b7a5", 0.36f));
            AddImage("Accent", tacticalPlanPanel, Hex("58b7a5", 0.88f));
            tacticalPlanText = AddText("Text", tacticalPlanPanel, "", 9, Hex("d9e5df", 1f), TextAnchor.MiddleLeft);
            tacticalPlanText.resizeTextForBestFit = true;
            tacticalPlanText.resizeTextMinSize = 8;
            tacticalPlanText.resizeTextMaxSize = 10;
            for (int i = 0; i < 5; i++) logRows.Add(CreateLogRow(timelinePanel, i));

            commandPanel = AddPanel("Command Palette", canvas.transform, Hex("080b0d", 0.72f), Hex("3c4544", 0.46f));
            commandPromptBackplate = AddPanel("Command Prompt Backplate", topPanel, Hex("11191d", 0.42f), Hex("3c4544", 0.20f));
            commandPromptRail = AddImage("Command Prompt Rail", commandPromptBackplate, Hex("58b7a5", 0.92f)).rectTransform;
            commandPromptText = AddText("Command Prompt", topPanel, "", 10, Hex("58b7a5", 1f), TextAnchor.MiddleLeft);
            commandPromptText.fontStyle = FontStyle.Bold;
            commandPromptText.resizeTextForBestFit = true;
            commandPromptText.resizeTextMinSize = 8;
            commandPromptText.resizeTextMaxSize = 10;
            undoMoveButton = AddButton("Undo Move", topPanel, "Undo Move  [U]", () => bindings?.UndoMove?.Invoke(), false);
            undoMoveText = undoMoveButton.GetComponentInChildren<Text>();
            undoMoveText.fontSize = 9;
            undoMoveText.color = Hex("f3ead7", 1f);
            undoMoveButton.gameObject.SetActive(false);
            cancelTargetButton = AddButton("Cancel Target", topPanel, "Cancel Target  [Esc]", () => bindings?.CancelTarget?.Invoke(), false);
            cancelTargetText = cancelTargetButton.GetComponentInChildren<Text>();
            cancelTargetText.fontSize = 9;
            cancelTargetText.resizeTextForBestFit = true;
            cancelTargetText.resizeTextMinSize = 7;
            cancelTargetText.resizeTextMaxSize = 9;
            cancelTargetText.color = Hex("f3ead7", 1f);
            cancelTargetButton.gameObject.SetActive(false);
            commandDivider = AddImage("Command Group Divider", commandPanel, Hex("d7a84e", 0.42f)).rectTransform;
            EnsureCommandRowCount(6);
            utilityButton = AddButton("Utility", commandPanel, "", ToggleUtility, false);
            utilityLabel = AddText("Utility Label", utilityButton.transform, "Utility", 12, Hex("f3ead7", 1f), TextAnchor.MiddleCenter);
            utilitySubLabel = AddText("Utility Sub", utilityButton.transform, "", 9, Hex("b7aa90", 1f), TextAnchor.LowerCenter);
            utilityPopup = AddPanel("Utility Popup", canvas.transform, Hex("080b0d", 0.98f), Hex("d7a84e", 0.88f));
            guardButton = AddButton("Guard", utilityPopup, "Guard", () => RunUtility(ActionMode.Guard), false);
            elixirButton = AddButton("Elixir", utilityPopup, "Elixir", () => RunUtility(ActionMode.Elixir), false);
            guardText = guardButton.GetComponentInChildren<Text>();
            elixirText = elixirButton.GetComponentInChildren<Text>();
            utilityPopup.gameObject.SetActive(false);
            utilityButton.gameObject.SetActive(false);
        }

        private void EnsureCommandRowCount(int commandCount)
        {
            commandCount = Mathf.Max(0, commandCount);
            while (commandRows.Count < commandCount)
            {
                commandRows.Add(CreateCommandRow(commandPanel, commandRows.Count));
            }
        }

        private void BuildUnitCard(
            RectTransform panel,
            out Text title,
            out Text name,
            out Text header,
            out Text state,
            out Text status,
            out Image portrait,
            out Text portraitFallback,
            out RectTransform hpFill,
            out RectTransform manaFill)
        {
            title = AddText("Title", panel, "", 18, Hex("d7a84e", 1f), TextAnchor.MiddleLeft);
            title.fontStyle = FontStyle.Bold;
            title.resizeTextForBestFit = true;
            title.resizeTextMinSize = 9;
            title.resizeTextMaxSize = 18;
            RectTransform portraitFrame = AddPanel("Portrait Frame", panel, Hex("080b0d", 0.94f), Hex("3c4544", 0.72f));
            portrait = AddImage("Portrait", portraitFrame, Color.white);
            portrait.preserveAspect = true;
            portrait.raycastTarget = false;
            Stretch(portrait.rectTransform, 3f, 3f);
            portraitFallback = AddText("Portrait Fallback", portraitFrame, "?", 22, Hex("b7aa90", 0.92f), TextAnchor.MiddleCenter);
            portraitFallback.fontStyle = FontStyle.Bold;
            portraitFallback.raycastTarget = false;
            Stretch(portraitFallback.rectTransform, 3f, 3f);
            name = AddText("Name", panel, "", 17, Hex("f3ead7", 1f), TextAnchor.MiddleLeft);
            name.fontStyle = FontStyle.Bold;
            name.resizeTextForBestFit = true;
            name.resizeTextMinSize = 12;
            name.resizeTextMaxSize = 17;
            header = AddText("Header", panel, "", 11, Hex("b7aa90", 1f), TextAnchor.MiddleLeft);
            header.resizeTextForBestFit = true;
            header.resizeTextMinSize = 9;
            header.resizeTextMaxSize = 11;
            state = AddText("State", panel, "", 11, Hex("d7a84e", 1f), TextAnchor.UpperLeft);
            state.resizeTextForBestFit = true;
            state.resizeTextMinSize = 8;
            state.resizeTextMaxSize = 11;
            status = AddText("Status", panel, "", 10, Hex("b7aa90", 1f), TextAnchor.MiddleLeft);
            status.resizeTextForBestFit = true;
            status.resizeTextMinSize = 8;
            status.resizeTextMaxSize = 10;
            RectTransform hpBg = AddImage("Hp Bg", panel, Hex("050708", 0.88f)).rectTransform;
            hpFill = AddImage("Hp Fill", hpBg, Hex("b94b56", 1f)).rectTransform;
            Text hpValue = AddText("Value", hpBg, "", 10, Hex("f3ead7", 1f), TextAnchor.MiddleCenter);
            hpValue.fontStyle = FontStyle.Bold;
            hpValue.raycastTarget = false;
            AddMeterLabelShadow(hpValue);
            Stretch(hpValue.rectTransform, 2f, 0f);
            RectTransform manaBg = AddImage("Mana Bg", panel, Hex("050708", 0.88f)).rectTransform;
            manaFill = AddImage("Mana Fill", manaBg, Hex("58b7a5", 1f)).rectTransform;
            Text manaValue = AddText("Value", manaBg, "", 10, Hex("f3ead7", 1f), TextAnchor.MiddleCenter);
            manaValue.fontStyle = FontStyle.Bold;
            manaValue.raycastTarget = false;
            AddMeterLabelShadow(manaValue);
            Stretch(manaValue.rectTransform, 2f, 0f);
        }

        private CommandRow CreateCommandRow(Transform parent, int index)
        {
            Button button = AddButton("Command " + index, parent, "", () => RunCommand(index), true);
            Outline outline = button.GetComponent<Outline>();
            Text label = button.GetComponentInChildren<Text>();
            label.alignment = TextAnchor.MiddleCenter;
            label.fontSize = 13;
            label.resizeTextForBestFit = true;
            label.resizeTextMinSize = 10;
            label.resizeTextMaxSize = 13;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Truncate;
            label.raycastTarget = false;

            Image iconWell = AddImage("Icon Well", button.transform, Hex("080b0d", 0.86f));
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
            Image statePip = AddImage("State Pip", button.transform, Hex("58b7a5", 0.72f));
            statePip.raycastTarget = false;
            Outline statePipOutline = statePip.gameObject.AddComponent<Outline>();
            statePipOutline.effectColor = Hex("050708", 0.95f);
            statePipOutline.effectDistance = new Vector2(1f, -1f);

            RectTransform stateTagPanel = AddPanel("State Tag", button.transform, Hex("d7a84e", 0.96f), Hex("050708", 0.96f));
            stateTagPanel.GetComponent<Image>().raycastTarget = false;
            stateTagPanel.gameObject.SetActive(false);
            Text stateTag = AddText("State Tag Label", stateTagPanel, "", 8, Hex("080b0d", 1f), TextAnchor.MiddleCenter);
            stateTag.fontStyle = FontStyle.Bold;
            stateTag.resizeTextForBestFit = true;
            stateTag.resizeTextMinSize = 7;
            stateTag.resizeTextMaxSize = 8;
            stateTag.raycastTarget = false;
            Stretch(stateTag.rectTransform, 2f, 0f);

            Image hotkeyBackground = AddImage("Hotkey Keycap", button.transform, Hex("263035", 0.92f));
            hotkeyBackground.raycastTarget = false;
            Outline keyOutline = hotkeyBackground.gameObject.AddComponent<Outline>();
            keyOutline.effectColor = Hex("080b0d", 0.92f);
            keyOutline.effectDistance = new Vector2(1f, -1f);
            Text hotkey = AddText("Hotkey", hotkeyBackground.transform, "", 9, Hex("f3ead7", 1f), TextAnchor.MiddleCenter);
            hotkey.fontStyle = FontStyle.Bold;
            hotkey.raycastTarget = false;
            Stretch(hotkey.rectTransform, 1f, 1f);

            Text sub = AddText("Sub", button.transform, "", 9, Hex("c7baa2", 0.92f), TextAnchor.MiddleCenter);
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
            relay.Select = eventData => SetFocusedCommand(index, eventData is PointerEventData);
            relay.Deselect = () => ClearFocusedCommand(index);
            return new CommandRow(button.GetComponent<RectTransform>(), button, outline, label, hotkey, sub, iconWell, iconOutline, icon, iconFallback, hotkeyBackground, statePip, stateTagPanel, stateTag, accentRail);
        }

        private TurnChip CreateTurnChip(Transform parent, int index)
        {
            RectTransform root = AddPanel("Turn Chip " + index, parent, Hex("11171c", 0.92f), Hex("3c4544", 0.44f));
            Image accent = AddImage("Accent", root, Hex("b7aa90", 0.66f));
            Image portrait = AddImage("Portrait", root, Color.white);
            portrait.preserveAspect = true;
            portrait.raycastTarget = false;
            Text fallback = AddText("Fallback", root, "?", 13, Hex("b7aa90", 0.94f), TextAnchor.MiddleCenter);
            fallback.fontStyle = FontStyle.Bold;
            fallback.raycastTarget = false;
            Text name = AddText("Name", root, "", 10, Hex("f3ead7", 1f), TextAnchor.MiddleLeft);
            name.fontStyle = FontStyle.Bold;
            name.resizeTextForBestFit = true;
            name.resizeTextMinSize = 8;
            name.resizeTextMaxSize = 10;
            name.raycastTarget = false;
            Text round = AddText("Round", root, "", 8, Hex("d7a84e", 1f), TextAnchor.UpperRight);
            round.fontStyle = FontStyle.Bold;
            round.raycastTarget = false;
            return new TurnChip(root, accent, portrait, fallback, name, round);
        }

        private LogRow CreateLogRow(Transform parent, int index)
        {
            RectTransform root = AddPanel("Log Row " + index, parent, Hex("151b20", 0.82f), Hex("3c4544", 0.35f));
            Image stripe = AddImage("Stripe", root, Hex("7f9d5b", 1f));
            Text text = AddText("Text", root, "", 9, Hex("f3ead7", 1f), TextAnchor.UpperLeft);
            return new LogRow(root, stripe, text);
        }

        private void ApplyLayout(bool timelineExpanded, bool promoteEndTurn, bool showUndoMove, bool showCancelTarget, int commandCount)
        {
            lastWidth = Screen.width;
            lastHeight = Screen.height;
            lastTimelineExpanded = timelineExpanded;
            lastPromoteEndTurn = promoteEndTurn;
            lastShowUndoMove = showUndoMove;
            lastShowCancelTarget = showCancelTarget;
            lastCommandCount = commandCount;
            CombatHudGeometry geometry = CombatHudScreenLayout.Calculate(Screen.width, Screen.height);
            SetScreenRect(topPanel, geometry.Top);
            SetScreenRect(sidePanel, geometry.Side);
            SetScreenRect(commandPanel, geometry.Command);

            float statW = Screen.width < 1400 ? 62f : 70f;
            float statGap = Screen.width < 1400 ? 5f : 7f;
            float statsW = statW * 3f + statGap * 2f;
            float statsX = geometry.Top.width - statsW - 10f;
            float titleX = 14f;
            float titleW = Mathf.Clamp(geometry.Top.width * 0.28f, 290f, 440f);
            float phaseX = titleX + titleW + 8f;
            float phaseW = Mathf.Max(280f, statsX - phaseX - 8f);
            SetLocalRect(titleText.rectTransform, new Rect(titleX, 2f, titleW, 25f));
            SetLocalRect(routeText.rectTransform, new Rect(titleX + 1f, 27f, titleW - 68f, 14f));
            SetLocalRect(menuButton.GetComponent<RectTransform>(), new Rect(titleX + titleW - 62f, 25f, 62f, 18f));
            Rect phaseRect = new Rect(phaseX, 5f, phaseW, geometry.Top.height - 10f);
            float statH = geometry.Top.height - 12f;
            Rect roundRect = new Rect(statsX, 6f, statW, statH);
            Rect moveRect = new Rect(statsX + statW + statGap, 6f, statW, statH);
            Rect actionRect = new Rect(statsX + (statW + statGap) * 2f, 6f, statW, statH);
            SetLocalRect(phaseBackplate, phaseRect);
            SetLocalRect(phaseText.rectTransform, new Rect(phaseRect.x + 8f, phaseRect.y + 1f, phaseRect.width - 16f, 18f));
            SetLocalRect(roundStatBackplate, roundRect);
            SetLocalRect(moveStatBackplate, moveRect);
            SetLocalRect(actionStatBackplate, actionRect);
            SetLocalRect(roundStatText.rectTransform, PadLocal(roundRect, 3f, 2f));
            SetLocalRect(moveStatText.rectTransform, PadLocal(moveRect, 3f, 2f));
            SetLocalRect(actionStatText.rectTransform, PadLocal(actionRect, 3f, 2f));

            Rect commandPromptLocal = CombatHudScreenLayout.CommandPrompt(phaseRect.width, showUndoMove, showCancelTarget);
            Rect commandPrompt = new Rect(
                phaseRect.x + commandPromptLocal.x,
                phaseRect.y + commandPromptLocal.y,
                commandPromptLocal.width,
                commandPromptLocal.height);
            SetLocalRect(commandPromptBackplate, new Rect(commandPrompt.x - 5f, commandPrompt.y - 1f, commandPrompt.width + 7f, commandPrompt.height + 2f));
            SetLocalRect(commandPromptRail, new Rect(0f, 0f, 3f, commandPrompt.height + 2f));
            SetLocalRect(commandPromptText.rectTransform, commandPrompt);
            Rect undoLocal = CombatHudScreenLayout.UndoMoveButton(phaseRect.width, showCancelTarget);
            Rect cancelLocal = CombatHudScreenLayout.CancelTargetButton(phaseRect.width);
            SetLocalRect(undoMoveButton.GetComponent<RectTransform>(), new Rect(phaseRect.x + undoLocal.x, phaseRect.y + undoLocal.y, undoLocal.width, undoLocal.height));
            SetLocalRect(cancelTargetButton.GetComponent<RectTransform>(), new Rect(phaseRect.x + cancelLocal.x, phaseRect.y + cancelLocal.y, cancelLocal.width, cancelLocal.height));

            CombatHudScreenLayout.SidePanels(geometry.Side, timelineExpanded, out Rect active, out Rect target, out Rect timeline);
            SetLocalRect(activePanel, active);
            SetLocalRect(targetPanel, target);
            SetLocalRect(timelinePanel, timeline);
            LayoutUnitCard(activePanel, active.width, active.height);
            LayoutUnitCard(targetPanel, target.width, target.height);
            SetLocalRect(timelineTitle.rectTransform, new Rect(12f, 8f, timeline.width - 100f, 24f));
            SetLocalRect(timelineButton.GetComponent<RectTransform>(), new Rect(timeline.width - 78f, 10f, 58f, 24f));
            SetLocalRect(turnQueueText.rectTransform, new Rect(12f, 35f, timeline.width - 24f, 13f));
            const int turnColumns = 3;
            const float turnGap = 5f;
            const float turnRowGap = 4f;
            const float turnY = 50f;
            const float turnH = 33f;
            float turnW = (timeline.width - 24f - turnGap * (turnColumns - 1)) / turnColumns;
            for (int i = 0; i < turnChips.Count; i++)
            {
                int column = i % turnColumns;
                int row = i / turnColumns;
                Rect chipRect = new Rect(12f + column * (turnW + turnGap), turnY + row * (turnH + turnRowGap), turnW, turnH);
                TurnChip chip = turnChips[i];
                SetLocalRect(chip.Root, chipRect);
                SetLocalRect(chip.Accent.rectTransform, new Rect(0f, 0f, 3f, turnH));
                SetLocalRect(chip.Portrait.rectTransform, new Rect(7f, 3f, 27f, 27f));
                SetLocalRect(chip.Fallback.rectTransform, new Rect(7f, 3f, 27f, 27f));
                SetLocalRect(chip.Name.rectTransform, new Rect(38f, 4f, Mathf.Max(24f, turnW - 44f), 25f));
                SetLocalRect(chip.Round.rectTransform, new Rect(turnW - 20f, 1f, 16f, 12f));
            }
            SetLocalRect(tacticalPlanPanel, new Rect(10f, 124f, timeline.width - 20f, 30f));
            SetLocalRect(tacticalPlanPanel.Find("Accent").GetComponent<RectTransform>(), new Rect(0f, 0f, 4f, 30f));
            SetLocalRect(tacticalPlanText.rectTransform, new Rect(11f, 3f, timeline.width - 43f, 24f));
            tacticalPlanPanel.gameObject.SetActive(timelineExpanded);
            const float logY = 160f;
            const int layoutLogCount = 5;
            float availableLogHeight = Mathf.Max(20f, timeline.height - logY - 8f);
            const float logGap = 4f;
            float logH = Mathf.Clamp(
                (availableLogHeight - logGap * (layoutLogCount - 1)) / layoutLogCount,
                12f,
                56f);
            for (int i = 0; i < logRows.Count; i++)
            {
                Rect row = new Rect(10f, logY + i * (logH + logGap), timeline.width - 20f, logH);
                SetLocalRect(logRows[i].Root, row);
                SetLocalRect(logRows[i].Stripe.rectTransform, new Rect(0f, 0f, 4f, row.height));
                SetLocalRect(logRows[i].Text.rectTransform, new Rect(10f, 5f, row.width - 16f, row.height - 10f));
            }

            Rect[] buttons = CombatHudScreenLayout.CommandButtons(geometry.Command.width, geometry.Command.height, commandCount, promoteEndTurn);
            int groupBreakIndex = CombatHudScreenLayout.CommandGroupBreakIndex(buttons.Length);
            commandDivider.gameObject.SetActive(groupBreakIndex >= 0);
            if (groupBreakIndex >= 0)
            {
                float dividerY = (buttons[groupBreakIndex].yMax + buttons[groupBreakIndex + 1].yMin) * 0.5f - 1f;
                SetLocalRect(commandDivider, new Rect(8f, dividerY, geometry.Command.width - 16f, 2f));
            }
            for (int i = 0; i < commandRows.Count && i < buttons.Length; i++)
            {
                SetLocalRect(commandRows[i].Root, buttons[i]);
                bool compact = CombatHudScreenLayout.UsesCompactCommandLayout(buttons[i]);
                float iconSize = CombatHudScreenLayout.CommandIconSize(buttons[i]);
                float iconX = (buttons[i].width - iconSize) * 0.5f;
                float iconY = compact ? 4f : 6f;
                float labelY = iconY + iconSize + 1f;
                SetLocalRect(commandRows[i].IconWell.rectTransform, new Rect(iconX, iconY, iconSize, iconSize));
                SetLocalRect(commandRows[i].HotkeyBackground.rectTransform, new Rect(buttons[i].width - 35f, 4f, 29f, 14f));
                SetLocalRect(commandRows[i].StatePip.rectTransform, new Rect(iconX + iconSize - 8f, iconY + 2f, 8f, 8f));
                SetLocalRect(commandRows[i].StateTagPanel, new Rect(5f, 4f, 44f, 14f));
                float labelHeight = compact ? 15f : 17f;
                SetLocalRect(commandRows[i].Label.rectTransform, new Rect(4f, labelY, buttons[i].width - 8f, labelHeight));
                commandRows[i].SubLabel.gameObject.SetActive(!compact);
                if (!compact)
                {
                    SetLocalRect(commandRows[i].SubLabel.rectTransform, new Rect(4f, labelY + labelHeight, buttons[i].width - 8f, Mathf.Max(9f, buttons[i].height - labelY - labelHeight - 2f)));
                }
                const float railHeight = 3f;
                SetLocalRect(commandRows[i].AccentRail.rectTransform, new Rect(0f, buttons[i].height - railHeight, buttons[i].width, railHeight));
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
            SetLocalRect(panel.Find("Portrait Frame").GetComponent<RectTransform>(), geometry.Portrait);
            SetLocalRect(panel.Find("Name").GetComponent<RectTransform>(), geometry.Name);
            SetLocalRect(panel.Find("Header").GetComponent<RectTransform>(), geometry.Header);
            SetLocalRect(panel.Find("State").GetComponent<RectTransform>(), geometry.State);
            SetLocalRect(panel.Find("Status").GetComponent<RectTransform>(), geometry.Status);
            SetLocalRect(panel.Find("Hp Bg").GetComponent<RectTransform>(), geometry.Hp);
            SetLocalRect(panel.Find("Mana Bg").GetComponent<RectTransform>(), geometry.Mana);
        }

        private void RefreshUnitCard(
            CombatHudUnitView unit,
            Text title,
            Text name,
            Text header,
            Text state,
            Text status,
            Image portrait,
            Text portraitFallback,
            RectTransform hpFill,
            RectTransform manaFill,
            string fallbackTitle)
        {
            title.text = fallbackTitle;
            bool showMana = unit != null && unit.MaxMana > 0;
            RectTransform manaBackground = manaFill == null ? null : manaFill.parent as RectTransform;
            if (manaBackground != null) manaBackground.gameObject.SetActive(showMana);
            RectTransform panel = title == null ? null : title.rectTransform.parent as RectTransform;
            bool activeCard = fallbackTitle.StartsWith("ACTIVE", StringComparison.Ordinal);
            Color fallbackAccent = activeCard
                ? Hex("58b7a5", 0.82f)
                : unit == null ? Hex("7f8b8c", 0.72f) : Hex("b94b56", 0.82f);
            Outline panelOutline = panel == null ? null : panel.GetComponent<Outline>();
            if (panelOutline != null)
            {
                panelOutline.effectColor = UnitAccent(unit?.AccentHex, fallbackAccent);
            }
            Color unitAccent = UnitAccent(
                unit?.AccentHex,
                fallbackAccent);
            title.color = unitAccent;
            RectTransform portraitFrame = portrait == null ? null : portrait.rectTransform.parent as RectTransform;
            Outline portraitOutline = portraitFrame == null ? null : portraitFrame.GetComponent<Outline>();
            if (portraitOutline != null) portraitOutline.effectColor = unitAccent.WithAlpha(0.90f);
            if (panel != null) LayoutUnitCard(panel, panel.rect.width, panel.rect.height, showMana);
            if (unit == null)
            {
                name.text = activeCard ? "Waiting" : "Hover a unit";
                header.text = activeCard ? "No active combatant." : "Inspect the board when you need details.";
                state.text = "";
                state.color = StateToneColor(CombatHudStateTone.Neutral);
                status.text = "";
                if (portrait != null)
                {
                    portrait.sprite = null;
                    portrait.enabled = false;
                }
                if (portraitFallback != null)
                {
                    portraitFallback.text = "?";
                    portraitFallback.gameObject.SetActive(true);
                }
                SetFill(hpFill, 0, 1);
                SetFill(manaFill, 0, 1);
                SetMeterLabel(hpFill, "", 0, 0);
                SetMeterLabel(manaFill, "", 0, 0);
                return;
            }

            name.text = unit.Name ?? "";
            header.text = unit.Header ?? "";
            state.text = unit.StateLine ?? "";
            state.color = StateToneColor(unit.StateTone);
            status.text = unit.StatusLine ?? "";
            Sprite portraitSprite = UiRuntime.AtlasSprite(unit.PortraitTexture, unit.PortraitSource);
            if (portrait != null)
            {
                portrait.sprite = portraitSprite;
                portrait.enabled = portraitSprite != null;
                portrait.color = Color.white;
            }
            if (portraitFallback != null)
            {
                portraitFallback.text = UnitInitials(unit.Name);
                portraitFallback.color = unitAccent;
                portraitFallback.gameObject.SetActive(portraitSprite == null);
            }
            Image hpImage = hpFill == null ? null : hpFill.GetComponent<Image>();
            if (hpImage != null) hpImage.color = unitAccent;
            SetFill(hpFill, unit.Hp, unit.MaxHp);
            SetFill(manaFill, unit.Mana, unit.MaxMana);
            SetMeterLabel(hpFill, "HP", unit.Hp, unit.MaxHp);
            SetMeterLabel(manaFill, "MP", unit.Mana, unit.MaxMana);
        }

        private void RefreshTurnChips(IReadOnlyList<CombatHudTurnView> turns)
        {
            turns = turns ?? Array.Empty<CombatHudTurnView>();
            for (int i = 0; i < turnChips.Count; i++)
            {
                TurnChip chip = turnChips[i];
                bool visible = i < turns.Count && turns[i] != null;
                chip.Root.gameObject.SetActive(visible);
                if (!visible) continue;

                CombatHudTurnView turn = turns[i];
                Color accent = UnitAccent(turn.AccentHex, Hex("b7aa90", 0.82f));
                Image fill = chip.Root.GetComponent<Image>();
                if (fill != null)
                {
                    fill.color = turn.Active
                        ? Color.Lerp(Hex("11171c", 0.98f), accent, 0.22f)
                        : Hex("11171c", 0.92f);
                }
                Outline outline = chip.Root.GetComponent<Outline>();
                if (outline != null) outline.effectColor = accent.WithAlpha(turn.Active ? 0.96f : 0.42f);
                chip.Accent.color = accent.WithAlpha(turn.Active ? 1f : 0.68f);
                Sprite sprite = UiRuntime.AtlasSprite(turn.PortraitTexture, turn.PortraitSource);
                chip.Portrait.sprite = sprite;
                chip.Portrait.enabled = sprite != null;
                chip.Portrait.color = Color.white;
                chip.Fallback.text = UnitInitials(turn.Name);
                chip.Fallback.color = accent;
                chip.Fallback.gameObject.SetActive(sprite == null);
                chip.Name.text = turn.Name ?? "";
                chip.Name.color = turn.Active ? Hex("f8efda", 1f) : Hex("d8d0bf", 1f);
                chip.Round.text = turn.StartsNextRound ? "↻" : turn.Active ? "▶" : "";
                chip.Round.color = turn.StartsNextRound ? Hex("d7a84e", 1f) : accent;
            }
        }

        private static void RefreshStatBackplate(RectTransform backplate, Color accent, bool emphasized)
        {
            if (backplate == null) return;
            Image fill = backplate.GetComponent<Image>();
            if (fill != null)
            {
                fill.color = emphasized
                    ? Color.Lerp(Hex("11171c", 0.84f), accent, 0.12f)
                    : Hex("11171c", 0.74f);
            }
            Outline outline = backplate.GetComponent<Outline>();
            if (outline != null) outline.effectColor = accent.WithAlpha(emphasized ? 0.72f : 0.30f);
        }

        private static string UnitInitials(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "?";
            string[] parts = value.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return "?";
            if (parts.Length == 1) return parts[0].Substring(0, 1).ToUpperInvariant();
            return (parts[0].Substring(0, 1) + parts[parts.Length - 1].Substring(0, 1)).ToUpperInvariant();
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
            IReadOnlyList<CombatHudCommandView> commands = view?.Commands;
            if (commands == null || index < 0 || index >= commands.Count) return;
            utilityOpen = false;
            bindings.RunCommand(commands[index].Mode);
        }

        private void RunUtility(ActionMode mode)
        {
            utilityOpen = false;
            bindings?.RunUtility?.Invoke(mode);
        }

        private void SetHoveredCommand(int index)
        {
            if (!IsVisible || index < 0 || index >= commandRows.Count) return;
            CommandRow row = commandRows[index];
            if (row?.Root == null || !row.Root.gameObject.activeInHierarchy || row.Button == null) return;
            pointerOwnsCommandContext = true;
            hoveredCommandIndex = index;
            CombatHudView view = bindings?.View?.Invoke();
            if (view != null) RefreshCommandPrompt(view);
        }

        private void ClearHoveredCommand(int index)
        {
            if (hoveredCommandIndex != index) return;
            hoveredCommandIndex = -1;
            pointerOwnsCommandContext = false;
            CombatHudView view = bindings?.View?.Invoke();
            if (view != null) RefreshCommandPrompt(view);
        }

        private void SetFocusedCommand(int index, bool pointerSelection = false)
        {
            if (pointerSelection)
            {
                hoveredCommandIndex = index;
                pointerOwnsCommandContext = true;
            }
            else
            {
                hoveredCommandIndex = -1;
                pointerOwnsCommandContext = false;
            }
            focusedCommandIndex = index;
            Refresh();
        }

        private void ClearFocusedCommand(int index)
        {
            if (focusedCommandIndex != index) return;
            focusedCommandIndex = -1;
            Refresh();
        }

        private void RefreshCommandPrompt(CombatHudView view)
        {
            if (commandPromptText == null || view == null) return;
            IReadOnlyList<CombatHudCommandView> commands = view.Commands ?? Array.Empty<CombatHudCommandView>();
            string prompt = view.CommandPrompt ?? "";
            Color color = Hex("58b7a5", 1f);
            int contextualIndex = ContextualCommandIndex(commands.Count);
            if (contextualIndex >= 0)
            {
                CombatHudCommandView command = commands[contextualIndex];
                string blockedDetail = !string.IsNullOrWhiteSpace(command.DisabledReason)
                    ? command.DisabledReason
                    : !string.IsNullOrWhiteSpace(command.SubLabel) ? command.SubLabel : command.Tooltip;
                string detail = command.Blocked
                    ? blockedDetail
                    : command.Enabled ? command.Tooltip : command.DisabledReason;
                prompt = $"{command.Label} [{command.Hotkey}]  {detail}";
                color = command.Promoted
                    ? Hex("d7a84e", 1f)
                    : command.Blocked ? Hex("b94b56", 1f) : command.Enabled ? Hex("f3ead7", 1f) : Hex("b94b56", 1f);
            }
            else
            {
                for (int i = 0; i < commands.Count; i++)
                {
                    if (commands[i].Mode == ActionMode.Wait && commands[i].Promoted)
                    {
                        color = Hex("d7a84e", 1f);
                        break;
                    }
                }
            }
            commandPromptText.text = prompt;
            commandPromptText.color = color;
            if (commandPromptRail != null)
            {
                Image rail = commandPromptRail.GetComponent<Image>();
                if (rail != null) rail.color = color.WithAlpha(0.94f);
            }
            if (commandPromptBackplate != null)
            {
                Image fill = commandPromptBackplate.GetComponent<Image>();
                if (fill != null)
                {
                    fill.color = Color.Lerp(
                        Hex("11191d", 0.48f),
                        color,
                        contextualIndex >= 0 ? 0.10f : 0.05f);
                }
                Outline outline = commandPromptBackplate.GetComponent<Outline>();
                if (outline != null) outline.effectColor = color.WithAlpha(contextualIndex >= 0 ? 0.42f : 0.20f);
            }
        }

        private int ContextualCommandIndex(int commandCount = int.MaxValue)
        {
            if (pointerOwnsCommandContext
                && hoveredCommandIndex >= 0
                && hoveredCommandIndex < commandCount)
            {
                return hoveredCommandIndex;
            }
            return focusedCommandIndex >= 0 && focusedCommandIndex < commandCount
                ? focusedCommandIndex
                : -1;
        }

        private ActionMode? VisibleCommandModeForIndex(int index)
        {
            if (index < 0 || index >= commandRows.Count) return null;
            CommandRow row = commandRows[index];
            return row?.Root != null && row.Root.gameObject.activeInHierarchy
                ? row.Mode
                : (ActionMode?)null;
        }

        private void ClearTransientCommandContext()
        {
            hoveredCommandIndex = -1;
            focusedCommandIndex = -1;
            pointerOwnsCommandContext = false;
        }

        private bool IsCanvasSelection(GameObject selected)
        {
            if (selected == null || canvas == null) return false;
            Transform selectedTransform = selected.transform;
            Transform canvasTransform = canvas.transform;
            return selectedTransform == canvasTransform || selectedTransform.IsChildOf(canvasTransform);
        }

        private static Color StateToneColor(CombatHudStateTone tone)
        {
            if (tone == CombatHudStateTone.Ready) return Hex("58b7a5", 1f);
            if (tone == CombatHudStateTone.Blocked) return Hex("c65c3b", 1f);
            return Hex("d7a84e", 1f);
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

        private static Color CommandAccent(ActionMode mode)
        {
            switch (mode)
            {
                case ActionMode.Move: return Hex("58b7a5", 1f);
                case ActionMode.Attack: return Hex("c65c3b", 1f);
                case ActionMode.Cast: return Hex("a77ae8", 1f);
                case ActionMode.Ability: return Hex("d7a84e", 1f);
                case ActionMode.Guard: return Hex("8ecbd7", 1f);
                case ActionMode.Elixir: return Hex("b94b56", 1f);
                case ActionMode.Wait: return Hex("d7a84e", 1f);
                default: return Hex("b7aa90", 1f);
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

        private static Rect PadLocal(Rect rect, float horizontal, float vertical)
        {
            return new Rect(
                rect.x + horizontal,
                rect.y + vertical,
                Mathf.Max(0f, rect.width - horizontal * 2f),
                Mathf.Max(0f, rect.height - vertical * 2f));
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

        private sealed class TurnChip
        {
            public readonly RectTransform Root;
            public readonly Image Accent;
            public readonly Image Portrait;
            public readonly Text Fallback;
            public readonly Text Name;
            public readonly Text Round;

            public TurnChip(RectTransform root, Image accent, Image portrait, Text fallback, Text name, Text round)
            {
                Root = root;
                Accent = accent;
                Portrait = portrait;
                Fallback = fallback;
                Name = name;
                Round = round;
            }
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
            public readonly Image StatePip;
            public readonly RectTransform StateTagPanel;
            public readonly Text StateTag;
            public readonly Image AccentRail;
            public ActionMode Mode;

            public CommandRow(RectTransform root, Button button, Outline outline, Text label, Text hotkey, Text subLabel, Image iconWell, Outline iconOutline, Image icon, Text iconFallback, Image hotkeyBackground, Image statePip, RectTransform stateTagPanel, Text stateTag, Image accentRail)
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
                StatePip = statePip;
                StateTagPanel = stateTagPanel;
                StateTag = stateTag;
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
