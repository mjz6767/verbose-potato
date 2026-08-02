using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AshenHalls
{
    public enum CombatAbilityModalFilter
    {
        All,
        Ready,
        Learned,
        Future
    }

    public enum CombatAbilityModalBookState
    {
        Unavailable,
        ReadyNow,
        Targeting,
        Locked,
        LowResource,
        NoTarget,
        ActionUsed,
        Disabled,
        Blocked
    }

    public sealed class CombatAbilityModalCardView
    {
        public string Id;
        public string Name;
        public string Kind;
        public string Cost;
        public string Range;
        public string Target;
        public string Path;
        public string Impact;
        public string Summary;
        public string RowSummary;
        public string CurrentEffect;
        public string Detail;
        public string DisabledReason;
        public string ResourceAfter;
        public string TacticalNote;
        public Texture2D IconTexture;
        public Rect IconSource;
        public string Sigil;
        public string AccentHex;
        public string Tier;
        public int UnlockLevel;
        public int ValidTargetCount;
        public bool TargetCountKnown;
        public bool Locked;
        public bool Epic;
        public bool Focused;
        public bool Targeted;
        public bool Ready;
        public bool Usable;
        public bool Selected;
    }

    public sealed class CombatAbilityModalView
    {
        public bool Visible;
        public bool Spellbook;
        public string Title;
        public string Header;
        public string Actor;
        public string Resource;
        public string ActionState;
        public string Trait;
        public string ContextKey;
        public string EmptyText;
        public string SelectedId;
        public Texture2D StateIconTexture;
        public IReadOnlyList<CombatAbilityModalCardView> Cards = Array.Empty<CombatAbilityModalCardView>();
    }

    public sealed class CombatAbilityModalBindings
    {
        public Func<CombatAbilityModalView> View;
        public Action Close;
        public Action<string> PreviewCard;
        public Action<string> SelectCard;
        public Action<string> ActivateCard;
    }

    public static class CombatAbilityModalPresentationRules
    {
        public static bool HasCurrentTarget(CombatAbilityModalCardView card)
        {
            return card == null
                || !card.Targeted
                || !card.TargetCountKnown
                || card.ValidTargetCount > 0;
        }

        public static bool CanActivate(CombatAbilityModalCardView card)
        {
            return card != null
                && !card.Locked
                && card.Usable
                && HasCurrentTarget(card);
        }

        public static bool IsReadyNow(CombatAbilityModalCardView card)
        {
            return card != null
                && !card.Locked
                && card.Usable
                && HasCurrentTarget(card);
        }

        public static string CardActionLabel(CombatAbilityModalCardView card)
        {
            if (card == null) return "Unavailable";
            if (card.Locked) return "Unlocks at Level " + card.UnlockLevel;
            if (!card.Usable) return ShortDisabledReason(card.DisabledReason);
            if (!HasCurrentTarget(card)) return "No Legal Target";
            if (card.Ready) return "Resume Targeting";
            return card.Targeted ? "Choose Target" : "Use Now";
        }

        public static string AvailabilityLabel(CombatAbilityModalCardView card)
        {
            return BookStateLabel(ResolveBookState(card));
        }

        public static CombatAbilityModalBookState ResolveBookState(CombatAbilityModalCardView card)
        {
            if (card == null) return CombatAbilityModalBookState.Unavailable;
            if (card.Locked) return CombatAbilityModalBookState.Locked;
            if (!card.Usable)
            {
                string reason = (card.DisabledReason ?? "").ToLowerInvariant();
                if (reason.Contains("mana") || reason.Contains(" mp"))
                {
                    return CombatAbilityModalBookState.LowResource;
                }
                if (reason.Contains("action")) return CombatAbilityModalBookState.ActionUsed;
                if (reason.Contains("stun") || reason.Contains("sleep") || reason.Contains("disabled"))
                {
                    return CombatAbilityModalBookState.Disabled;
                }
                return CombatAbilityModalBookState.Blocked;
            }
            if (!HasCurrentTarget(card)) return CombatAbilityModalBookState.NoTarget;
            if (card.Ready) return CombatAbilityModalBookState.Targeting;
            return CombatAbilityModalBookState.ReadyNow;
        }

        public static string BookStateLabel(CombatAbilityModalBookState state)
        {
            switch (state)
            {
                case CombatAbilityModalBookState.ReadyNow:
                    return "READY NOW";
                case CombatAbilityModalBookState.Targeting:
                    return "TARGETING";
                case CombatAbilityModalBookState.Locked:
                    return "LOCKED";
                case CombatAbilityModalBookState.LowResource:
                    return "LOW MP";
                case CombatAbilityModalBookState.NoTarget:
                    return "NO TARGET";
                case CombatAbilityModalBookState.ActionUsed:
                    return "ACTION USED";
                case CombatAbilityModalBookState.Disabled:
                    return "DISABLED";
                case CombatAbilityModalBookState.Blocked:
                    return "BLOCKED";
                default:
                    return "UNAVAILABLE";
            }
        }

        public static int BookStateIconIndex(CombatAbilityModalCardView card)
        {
            return BookStateIconIndex(ResolveBookState(card));
        }

        public static int BookStateIconIndex(CombatAbilityModalBookState state)
        {
            switch (state)
            {
                case CombatAbilityModalBookState.ReadyNow:
                    return CombatIconCatalog.BookStateSelectionIndex;
                case CombatAbilityModalBookState.Targeting:
                    return CombatIconCatalog.BookStateTargetingIndex;
                case CombatAbilityModalBookState.Locked:
                    return CombatIconCatalog.BookStateLockedIndex;
                case CombatAbilityModalBookState.LowResource:
                    return CombatIconCatalog.BookStateLowResourceIndex;
                case CombatAbilityModalBookState.NoTarget:
                    return CombatIconCatalog.BookStateNoTargetIndex;
                case CombatAbilityModalBookState.ActionUsed:
                    return CombatIconCatalog.BookStateActionUsedIndex;
                case CombatAbilityModalBookState.Disabled:
                    return CombatIconCatalog.BookStateDisabledIndex;
                case CombatAbilityModalBookState.Blocked:
                case CombatAbilityModalBookState.Unavailable:
                default:
                    return CombatIconCatalog.BookStateBlockedIndex;
            }
        }

        public static string BackButtonLabel(IReadOnlyList<CombatAbilityModalCardView> _)
        {
            return "Back to Battle  [Esc]";
        }

        public static string DetailPrompt(CombatAbilityModalCardView card)
        {
            if (card == null) return "";
            if (card.Locked) return $"Reach level {card.UnlockLevel} to learn this power.";
            if (!card.Usable)
            {
                return string.IsNullOrWhiteSpace(card.DisabledReason) ? "Unavailable." : card.DisabledReason;
            }
            if (!HasCurrentTarget(card))
            {
                return string.IsNullOrWhiteSpace(card.TacticalNote)
                    ? "No legal target is available from this position. Move, then reopen the book."
                    : card.TacticalNote;
            }
            if (card.Ready) return "Targeting is armed. Return to battle and select a highlighted target.";

            return "";
        }

        public static string TargetCountLabel(CombatAbilityModalCardView card)
        {
            if (card == null) return "";
            if (!card.Targeted)
            {
                if (!string.IsNullOrWhiteSpace(card.Impact)) return card.Impact.Trim();
                string immediateTarget = card.Target?.Trim() ?? "";
                return string.IsNullOrWhiteSpace(immediateTarget)
                    || string.Equals(immediateTarget, "instant", StringComparison.OrdinalIgnoreCase)
                    ? "Immediate"
                    : Capitalize(immediateTarget);
            }

            if (!card.TargetCountKnown)
            {
                string target = card.Target?.Trim() ?? "";
                return string.IsNullOrWhiteSpace(target) ? "Target" : Capitalize(target);
            }

            int count = Mathf.Max(0, card.ValidTargetCount);
            TargetCountWords(card.Target, out string singular, out string plural, out string qualifier);
            if (count <= 0) return $"No {qualifier} {singular} now";
            return count == 1
                ? $"1 {qualifier} {singular}"
                : $"{count} {qualifier} {plural}";
        }

        public static string RowMeta(CombatAbilityModalCardView card)
        {
            if (card == null) return "";
            List<string> parts = new List<string>();
            if (card.Locked && card.UnlockLevel > 0)
            {
                parts.Add("Unlocks L" + card.UnlockLevel);
            }
            AddDistinct(parts, card.Cost);
            AddDistinct(parts, card.Range);
            return string.Join("  •  ", parts);
        }

        public static bool MatchesFilter(CombatAbilityModalCardView card, CombatAbilityModalFilter filter)
        {
            if (card == null) return false;
            switch (filter)
            {
                case CombatAbilityModalFilter.Ready:
                    return IsReadyNow(card);
                case CombatAbilityModalFilter.Learned:
                    return !card.Locked;
                case CombatAbilityModalFilter.Future:
                    return card.Locked;
                default:
                    return true;
            }
        }

        public static int Count(IReadOnlyList<CombatAbilityModalCardView> cards, CombatAbilityModalFilter filter)
        {
            if (cards == null) return 0;
            int count = 0;
            for (int i = 0; i < cards.Count; i++)
            {
                if (MatchesFilter(cards[i], filter)) count++;
            }
            return count;
        }

        public static CombatAbilityModalFilter InitialFilter(IReadOnlyList<CombatAbilityModalCardView> cards)
        {
            if (Count(cards, CombatAbilityModalFilter.Ready) > 0) return CombatAbilityModalFilter.Ready;
            if (Count(cards, CombatAbilityModalFilter.Learned) > 0) return CombatAbilityModalFilter.Learned;
            if (Count(cards, CombatAbilityModalFilter.Future) > 0) return CombatAbilityModalFilter.Future;
            return CombatAbilityModalFilter.All;
        }

        public static string FilterLabel(CombatAbilityModalFilter filter, IReadOnlyList<CombatAbilityModalCardView> cards)
        {
            string label;
            int shortcut;
            switch (filter)
            {
                case CombatAbilityModalFilter.Ready:
                    label = "READY";
                    shortcut = 1;
                    break;
                case CombatAbilityModalFilter.Learned:
                    label = "KNOWN";
                    shortcut = 2;
                    break;
                case CombatAbilityModalFilter.Future:
                    label = "LOCKED";
                    shortcut = 3;
                    break;
                default:
                    label = "ALL";
                    shortcut = 4;
                    break;
            }
            return $"[{shortcut}]  {label}  {Count(cards, filter)}";
        }

        public static bool ShouldShowRowBadge(
            CombatAbilityModalCardView card,
            CombatAbilityModalFilter filter,
            string actionState)
        {
            if (card == null) return false;
            if (card.Locked) return filter != CombatAbilityModalFilter.Future;
            if (!card.Usable && IsGlobalBlocker(card.DisabledReason, actionState)) return false;
            if (card.Ready && CanActivate(card)) return true;
            if (!HasCurrentTarget(card)) return true;
            return !card.Usable;
        }

        public static string DetailMeta(CombatAbilityModalCardView card)
        {
            if (card == null) return "";
            List<string> parts = new List<string>();
            AddDistinct(parts, card.Kind);
            if (!string.IsNullOrWhiteSpace(card.Tier))
            {
                string tier = card.Tier.Trim();
                AddDistinct(parts, char.ToUpperInvariant(tier[0]) + tier.Substring(1) + " tier");
            }
            AddDistinct(parts, card.Impact);
            AddDistinct(parts, card.ResourceAfter);
            return string.Join("  •  ", parts);
        }

        public static string DetailNotes(CombatAbilityModalCardView card)
        {
            string detail = card?.Detail?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(detail)) return "";

            const string tacticsMarker = "\n\nTACTICS\n";
            int tactics = detail.IndexOf(tacticsMarker, StringComparison.OrdinalIgnoreCase);
            if (tactics >= 0)
            {
                return detail.Substring(tactics + tacticsMarker.Length).Trim();
            }

            const string castingHeader = "CASTING RULES\n";
            const string formulaMarker = "\n\nFORMULA NOTE\n";
            int formulaNote = detail.IndexOf(formulaMarker, StringComparison.OrdinalIgnoreCase);
            if (formulaNote >= 0)
            {
                string note = detail.Substring(formulaNote + formulaMarker.Length).Trim();
                return note;
            }

            if (detail.StartsWith(castingHeader, StringComparison.OrdinalIgnoreCase))
            {
                detail = detail.Substring(castingHeader.Length);
            }
            return detail.Trim();
        }

        private static void AddDistinct(List<string> parts, string value)
        {
            string trimmed = value?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(trimmed)) return;
            for (int i = 0; i < parts.Count; i++)
            {
                if (string.Equals(parts[i], trimmed, StringComparison.OrdinalIgnoreCase)) return;
            }
            parts.Add(trimmed);
        }

        private static bool IsGlobalBlocker(string disabledReason, string actionState)
        {
            string reason = (disabledReason ?? "").Trim().ToLowerInvariant();
            string state = (actionState ?? "").Trim().ToUpperInvariant();
            if (state == "ACTION USED") return reason.Contains("action");
            if (state.StartsWith("STUNNED", StringComparison.Ordinal)) return reason.Contains("stun");
            if (state.StartsWith("SLEEPING", StringComparison.Ordinal)) return reason.Contains("sleep");
            if (state == "RESOLVING") return reason.Contains("resolving");
            return false;
        }

        private static void TargetCountWords(
            string rawTarget,
            out string singular,
            out string plural,
            out string qualifier)
        {
            string target = (rawTarget ?? "").Trim().ToLowerInvariant();
            if (target.Contains("tile"))
            {
                singular = "tile";
                plural = "tiles";
                qualifier = "valid";
                return;
            }
            if (target == "enemy")
            {
                singular = "enemy";
                plural = "enemies";
                qualifier = "legal";
                return;
            }
            if (target == "ally")
            {
                singular = "ally";
                plural = "allies";
                qualifier = "legal";
                return;
            }
            if (target.Contains("ritual") || target.Contains("field"))
            {
                singular = "location";
                plural = "locations";
                qualifier = "valid";
                return;
            }

            singular = "target";
            plural = "targets";
            qualifier = "legal";
        }

        private static string Capitalize(string value)
        {
            string trimmed = value?.Trim() ?? "";
            if (trimmed.Length == 0) return "";
            return char.ToUpperInvariant(trimmed[0]) + trimmed.Substring(1);
        }

        private static string ShortDisabledReason(string reason)
        {
            if (string.IsNullOrWhiteSpace(reason)) return "Unavailable";
            string trimmed = reason.Trim().TrimEnd('.');
            if (trimmed.Length <= 24) return trimmed;
            string lower = trimmed.ToLowerInvariant();
            if (lower.Contains("mana") || lower.Contains(" mp")) return "Not Enough MP";
            if (lower.Contains("action")) return "Action Used";
            if (lower.Contains("stun")) return "Stunned";
            if (lower.Contains("sleep")) return "Sleeping";
            return "Unavailable";
        }
    }

    public readonly struct CombatAbilityModalGeometry
    {
        public readonly Rect Backdrop;
        public readonly Rect Panel;
        public readonly Rect Filters;
        public readonly Rect List;
        public readonly Rect Detail;
        public readonly Rect CloseButton;
        public readonly Rect Footer;

        public CombatAbilityModalGeometry(
            Rect backdrop,
            Rect panel,
            Rect filters,
            Rect list,
            Rect detail,
            Rect closeButton,
            Rect footer)
        {
            Backdrop = backdrop;
            Panel = panel;
            Filters = filters;
            List = list;
            Detail = detail;
            CloseButton = closeButton;
            Footer = footer;
        }

        public bool Fits(float width, float height)
        {
            return FitsScreen(Backdrop, width, height)
                && FitsScreen(Panel, width, height)
                && FitsLocal(Filters, Panel)
                && FitsLocal(List, Panel)
                && FitsLocal(Detail, Panel)
                && FitsLocal(CloseButton, Panel)
                && FitsLocal(Footer, Panel)
                && Filters.yMax <= List.yMin
                && List.xMax <= Detail.xMin
                && List.yMax <= Footer.yMin
                && Detail.yMax <= Footer.yMin;
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

    public static class CombatAbilityModalLayout
    {
        public static CombatAbilityModalGeometry Calculate(float width, float height)
        {
            float panelW = Mathf.Min(Mathf.Max(1040f, width * 0.90f), width - 40f);
            float panelH = Mathf.Min(Mathf.Max(620f, height * 0.90f), height - 40f);
            Rect backdrop = new Rect(0f, 0f, width, height);
            Rect panel = new Rect((width - panelW) * 0.5f, (height - panelH) * 0.5f, panelW, panelH);
            float splitX = Mathf.Clamp(panelW * 0.56f, 570f, panelW - 430f);
            Rect filters = new Rect(24f, 78f, splitX - 36f, 40f);
            Rect list = new Rect(24f, 128f, splitX - 36f, panelH - 164f);
            Rect detail = new Rect(splitX + 8f, 128f, panelW - splitX - 32f, panelH - 164f);
            Rect close = new Rect(panelW - 238f, 18f, 214f, 42f);
            Rect footer = new Rect(24f, panelH - 28f, panelW - 48f, 18f);
            return new CombatAbilityModalGeometry(backdrop, panel, filters, list, detail, close, footer);
        }
    }

    public readonly struct CombatAbilityModalNavigationStep
    {
        public readonly int Direction;
        public readonly int HeldDirection;
        public readonly float NextRepeatAt;

        public CombatAbilityModalNavigationStep(int direction, int heldDirection, float nextRepeatAt)
        {
            Direction = direction;
            HeldDirection = heldDirection;
            NextRepeatAt = nextRepeatAt;
        }
    }

    public static class CombatAbilityModalNavigationRules
    {
        public const float AxisThreshold = 0.65f;
        public const float InitialRepeatDelay = 0.38f;
        public const float RepeatInterval = 0.10f;

        public static CombatAbilityModalNavigationStep SeedVertical(float axis, float now)
        {
            int direction = VerticalDirection(axis);
            return new CombatAbilityModalNavigationStep(
                0,
                direction,
                direction == 0 ? 0f : now + InitialRepeatDelay);
        }

        public static CombatAbilityModalNavigationStep ResolveVertical(
            float axis,
            int heldDirection,
            float nextRepeatAt,
            float now)
        {
            int direction = VerticalDirection(axis);
            if (direction == 0)
            {
                return new CombatAbilityModalNavigationStep(0, 0, 0f);
            }
            if (direction != heldDirection)
            {
                return new CombatAbilityModalNavigationStep(
                    direction,
                    direction,
                    now + InitialRepeatDelay);
            }
            if (now < nextRepeatAt)
            {
                return new CombatAbilityModalNavigationStep(0, direction, nextRepeatAt);
            }
            return new CombatAbilityModalNavigationStep(
                direction,
                direction,
                now + RepeatInterval);
        }

        private static int VerticalDirection(float axis)
        {
            if (axis >= AxisThreshold) return -1;
            if (axis <= -AxisThreshold) return 1;
            return 0;
        }
    }

    public static class CombatAbilityModalDetailScrollRules
    {
        public const float AxisThreshold = 0.25f;
        public const float AxisSpeed = 0.85f;
        public const float PageStep = 0.72f;

        public static float ApplyAxis(float current, float axis, float unscaledDeltaTime)
        {
            if (Mathf.Abs(axis) < AxisThreshold) return Mathf.Clamp01(current);
            float delta = Mathf.Clamp(unscaledDeltaTime, 0f, 0.10f);
            return Mathf.Clamp01(current + axis * AxisSpeed * delta);
        }

        public static float ApplyPage(float current, int direction)
        {
            if (direction == 0) return Mathf.Clamp01(current);
            return Mathf.Clamp01(current + Mathf.Sign(direction) * PageStep);
        }
    }

    public static class CombatAbilityModalPointerRules
    {
        public const float PreviewDwellSeconds = 0.10f;

        public static bool ShouldCommitPreview(
            string candidateId,
            string selectedId,
            bool pointerStillInside,
            float enteredAt,
            float now)
        {
            return pointerStillInside
                && !string.IsNullOrWhiteSpace(candidateId)
                && !string.Equals(candidateId, selectedId, StringComparison.Ordinal)
                && now >= enteredAt + PreviewDwellSeconds;
        }
    }

    public static class CombatAbilityModalListRules
    {
        public const float BaseRowHeight = 70f;
        public const float BaseRowGap = 5f;
        public const float RowInset = 4f;
        public const float ContentPadding = 8f;

        public static float UiScale(float screenHeight)
        {
            return Mathf.Clamp(screenHeight / 720f, 1f, 1.25f);
        }

        public static float RowHeight(float screenHeight)
        {
            return Mathf.Round(BaseRowHeight * UiScale(screenHeight));
        }

        public static float RowGap(float screenHeight)
        {
            return Mathf.Round(BaseRowGap * UiScale(screenHeight));
        }

        public static float ContentHeight(int cardCount, float rowHeight, float rowGap, float viewportHeight)
        {
            return Mathf.Max(
                Mathf.Max(0f, viewportHeight),
                Mathf.Max(0, cardCount) * (Mathf.Max(1f, rowHeight) + Mathf.Max(0f, rowGap))
                    + ContentPadding);
        }

        public static string ScrollLabel(
            int cardCount,
            float rowHeight,
            float rowGap,
            float viewportHeight,
            float contentHeight,
            float scrollY)
        {
            if (cardCount <= 0) return "0 OF 0";

            float height = Mathf.Max(1f, rowHeight);
            float stride = height + Mathf.Max(0f, rowGap);
            float viewport = Mathf.Max(1f, viewportHeight);
            float maxScrollY = Mathf.Max(0f, contentHeight - viewport);
            float top = Mathf.Clamp(scrollY, 0f, maxScrollY);
            float bottom = top + viewport;
            int firstIndex = Mathf.Clamp(
                Mathf.CeilToInt((top - RowInset) / stride),
                0,
                cardCount - 1);
            int lastIndex = Mathf.Clamp(
                Mathf.FloorToInt((bottom - RowInset - height) / stride),
                0,
                cardCount - 1);
            if (lastIndex < firstIndex)
            {
                int nearest = Mathf.Clamp(Mathf.FloorToInt(top / stride), 0, cardCount - 1);
                firstIndex = nearest;
                lastIndex = nearest;
            }

            bool above = top > 0.5f;
            bool below = top < maxScrollY - 0.5f;
            string arrows = above && below ? "  ↑↓" : above ? "  ↑" : below ? "  ↓" : "";
            return $"{firstIndex + 1}–{lastIndex + 1} OF {cardCount}{arrows}";
        }
    }

    internal sealed class CombatAbilityModalFocusRelay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler
    {
        public Action PointerEnter;
        public Action PointerExit;
        public Action Selected;

        public void OnPointerEnter(PointerEventData eventData)
        {
            PointerEnter?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            PointerExit?.Invoke();
        }

        public void OnSelect(BaseEventData eventData)
        {
            Selected?.Invoke();
        }
    }

    public sealed class CombatAbilityModalScreen : MonoBehaviour
    {
        private enum SelectionOrigin
        {
            Navigation,
            PointerClick,
            EventSystemFocus
        }

        private sealed class FilterBrowseState
        {
            public string SelectedId = "";
            public float ScrollY;
        }

        private const string BaseFooterHint =
            "↑↓/LS Browse  •  Enter/A Use  •  Tab/LB/RB Filter  •  Esc/B Back";
        private const string OverflowFooterHint =
            "↑↓/LS Browse  •  Enter/A Use  •  Tab/LB/RB Filter  •  RS/PgUp/PgDn Details  •  Esc/B Back";
        private static readonly CombatAbilityModalFilter[] Filters =
        {
            CombatAbilityModalFilter.Ready,
            CombatAbilityModalFilter.Learned,
            CombatAbilityModalFilter.Future,
            CombatAbilityModalFilter.All
        };

        private readonly List<CardRow> cardRows = new List<CardRow>();
        private readonly List<CombatAbilityModalCardView> visibleCards = new List<CombatAbilityModalCardView>();
        private readonly Dictionary<string, FilterBrowseState> filterBrowseStates =
            new Dictionary<string, FilterBrowseState>(StringComparer.Ordinal);
        private CombatAbilityModalBindings bindings;
        private CombatAbilityModalView currentView;
        private Canvas canvas;
        private CanvasGroup canvasGroup;
        private RectTransform backdrop;
        private RectTransform panel;
        private RectTransform accentStrip;
        private RectTransform listFrame;
        private RectTransform listViewport;
        private RectTransform listContent;
        private ScrollRect listScroll;
        private Scrollbar listScrollbar;
        private Text listScrollHint;
        private RectTransform detailPanel;
        private RectTransform detailArtBackdrop;
        private RectTransform detailIconFrame;
        private Image detailIcon;
        private Text detailSigil;
        private Image detailStatusIcon;
        private Image detailContextIcon;
        private Text titleText;
        private Text actorText;
        private Text resourceText;
        private Text actionStateText;
        private Text traitText;
        private Text emptyText;
        private Text detailTitle;
        private Text detailStatus;
        private Text detailContext;
        private Text detailSummary;
        private Text detailMeta;
        private RectTransform detailNarrativeFrame;
        private RectTransform detailNarrativeViewport;
        private RectTransform detailNarrativeContent;
        private ScrollRect detailNarrativeScroll;
        private Scrollbar detailNarrativeScrollbar;
        private Text detailNotesLabel;
        private Text detailNotes;
        private Text detailPrompt;
        private Button detailActionButton;
        private Text detailActionLabel;
        private Button closeButton;
        private Text closeButtonLabel;
        private Text footerHint;
        private Button[] filterButtons;
        private Text[] filterLabels;
        private StatChip[] statChips;
        private Font font;
        private Texture2D generatedBookStateIconAtlas;
        private float lastWidth = -1f;
        private float lastHeight = -1f;
        private float rowHeight = CombatAbilityModalListRules.BaseRowHeight;
        private float rowGap = CombatAbilityModalListRules.BaseRowGap;
        private string selectedId = "";
        private string hoveredId = "";
        private string pendingHoveredId = "";
        private float pendingHoverEnteredAt;
        private int activeCardCount;
        private int openedFrame = -1;
        private string listContextKey = "";
        private string narrativeContentKey = "";
        private string pendingFilterSelectionId = "";
        private float pendingFilterScrollY;
        private bool pendingFilterRestore;
        private bool filterInitialized;
        private bool lastRefreshSucceeded;
        private CombatAbilityModalFilter currentFilter = CombatAbilityModalFilter.Ready;
        private int heldVerticalDirection;
        private float nextVerticalRepeatAt;
        private EventSystem capturedEventSystem;
        private bool previousSendNavigationEvents;
        private bool navigationEventsCaptured;
        private bool handlingEventSystemFocus;

        public bool IsReady => canvas != null
            && panel != null
            && closeButton != null
            && detailActionButton != null
            && listViewport != null;
        public bool IsVisible => IsReady && UiRuntime.IsCanvasVisible(canvas);
        public bool CanOwnModal => IsReady
            && lastRefreshSucceeded
            && UiRuntime.CanOwnModal(canvas, panel, canvasGroup, closeButton);
        public bool HasRenderableGeometry => CanOwnModal;
        public int VisibleCardCount => visibleCards.Count;
        public string SelectedId => selectedId;
        public CombatAbilityModalFilter ActiveFilter => currentFilter;
        public string PreviewedIdForTest => hoveredId;
        public string PendingPreviewIdForTest => pendingHoveredId;
        public string DetailIdForTest => DetailId();
        public string DetailActionLabelForTest => detailActionLabel == null ? "" : detailActionLabel.text;
        public bool DetailActionInteractableForTest => detailActionButton != null && detailActionButton.interactable;
        public float DetailIconScaleForTest => detailIcon == null ? 0f : detailIcon.rectTransform.localScale.x;
        public bool DetailUsesSelectionChromeForTest =>
            detailPanel != null
            && ColorsMatch(detailPanel.GetComponent<Outline>().effectColor, Hex("58b7a5", 0.82f))
            && detailActionButton != null
            && ColorsMatch((detailActionButton.targetGraphic as Image)?.color ?? Color.clear, Hex("285d56", 1f));
        public bool DetailUsesArmedChromeForTest =>
            detailPanel != null
            && ColorsMatch(detailPanel.GetComponent<Outline>().effectColor, Hex("d7a84e", 0.94f))
            && detailActionButton != null
            && ColorsMatch((detailActionButton.targetGraphic as Image)?.color ?? Color.clear, Hex("d7a84e", 1f));
        public bool DetailStatusVisibleForTest => detailStatus != null && detailStatus.gameObject.activeSelf;
        public string DetailContextForTest => detailContext == null ? "" : detailContext.text;
        public string DetailPromptForTest => detailPrompt == null ? "" : detailPrompt.text;
        public string DetailTargetLabelForTest =>
            statChips != null && statChips.Length > 2 && statChips[2]?.Value != null
                ? statChips[2].Value.text
                : "";
        public string FooterHintForTest => footerHint == null ? "" : footerHint.text;
        public string ListScrollHintForTest => listScrollHint == null ? "" : listScrollHint.text;
        public bool AllVisibleCardsFitWithoutScrollForTest =>
            listContent != null
            && listViewport != null
            && listContent.rect.height <= listViewport.rect.height + 1f;
        public int FilterControlCountForTest => filterButtons == null ? 0 : filterButtons.Length;
        public float ScrollYForTest => listContent == null ? 0f : listContent.anchoredPosition.y;
        public bool DetailNarrativeCanScrollForTest => DetailNarrativeCanScroll();
        public float DetailNarrativeNormalizedPositionForTest =>
            detailNarrativeScroll == null ? 1f : detailNarrativeScroll.verticalNormalizedPosition;
        public bool DetailNarrativeFullyPresentedForTest =>
            detailNarrativeContent != null
            && detailNarrativeViewport != null
            && detailNotes != null
            && detailPrompt != null
            && (!detailNotes.gameObject.activeSelf
                || detailNotes.preferredHeight <= detailNotes.rectTransform.rect.height + 1f)
            && (!detailPrompt.gameObject.activeSelf
                || detailPrompt.preferredHeight <= detailPrompt.rectTransform.rect.height + 1f)
            && (detailNarrativeContent.rect.height <= detailNarrativeViewport.rect.height + 1f
                || detailNarrativeScroll != null && detailNarrativeScroll.vertical);
        public bool SelectedRowFocusedForTest
        {
            get
            {
                if (EventSystem.current == null) return false;
                int index = IndexOfVisible(selectedId);
                return index >= 0
                    && index < cardRows.Count
                    && EventSystem.current.currentSelectedGameObject == cardRows[index].Button.gameObject;
            }
        }
        public int SelectedRailCountForTest => CountVisibleRows(row => row.SelectionRail != null && row.SelectionRail.gameObject.activeSelf);
        public bool SelectedRailUsesSelectionAccentForTest => CountVisibleRows(
            row => row.SelectionRail != null
                && row.SelectionRail.gameObject.activeSelf
                && ColorsMatch(row.SelectionRail.color, Hex("58b7a5", 1f))) == 1;
        public int VisibleStatusBadgeCountForTest => CountVisibleRows(
            row => row.StatusBadge != null && row.StatusBadge.gameObject.activeSelf);
        public int TargetingBadgeCountForTest => CountVisibleRows(
            row => row.StatusBadge != null
                && row.StatusBadge.gameObject.activeSelf
                && string.Equals(row.State?.text, "TARGETING", StringComparison.Ordinal));
        // Retained as a compatibility probe for older smoke harnesses. Count the
        // live hierarchy so a future accidental second rail is caught by tests.
        public int VisibleTargetingRailCountForTest
        {
            get
            {
                if (panel == null) return 0;
                int count = 0;
                RectTransform[] descendants = panel.GetComponentsInChildren<RectTransform>(true);
                for (int i = 0; i < descendants.Length; i++)
                {
                    RectTransform descendant = descendants[i];
                    if (descendant != null
                        && descendant.gameObject.activeInHierarchy
                        && string.Equals(descendant.name, "Targeting Rail", StringComparison.Ordinal))
                    {
                        count++;
                    }
                }
                return count;
            }
        }
        public int VisiblePreviewCueCountForTest => CountVisibleRows(
            row => row.SelectionIcon != null
                && row.SelectionIcon.gameObject.activeSelf
                && string.Equals(row.Id, hoveredId, StringComparison.Ordinal));
        public int VisibleStateIconCountForTest => CountVisibleRows(
            row => row.StatusIcon != null && row.StatusIcon.gameObject.activeSelf)
            + CountVisibleRows(row => row.SelectionIcon != null && row.SelectionIcon.gameObject.activeSelf)
            + (detailStatusIcon != null && detailStatusIcon.gameObject.activeSelf ? 1 : 0)
            + (detailContextIcon != null && detailContextIcon.gameObject.activeSelf ? 1 : 0);
        public bool UsesGeneratedStateIconAtlasForTest =>
            !IsBookStateIconAtlas(currentView?.StateIconTexture)
            && generatedBookStateIconAtlas != null;
        public Texture2D StateIconTextureForTest => BookStateIconTexture();
        public CombatAbilityModalBookState SelectedBookStateForTest =>
            CombatAbilityModalPresentationRules.ResolveBookState(FindVisibleCard(selectedId));
        public int SelectedBookStateIconIndexForTest =>
            CombatAbilityModalPresentationRules.BookStateIconIndex(SelectedBookStateForTest);
        public CombatAbilityModalBookState DetailBookStateForTest =>
            CombatAbilityModalPresentationRules.ResolveBookState(FindVisibleCard(DetailId()));
        public int DetailBookStateIconIndexForTest =>
            CombatAbilityModalPresentationRules.BookStateIconIndex(DetailBookStateForTest);

        public CombatAbilityModalBookState BookStateForVisibleIndexForTest(int index)
        {
            return CombatAbilityModalPresentationRules.ResolveBookState(
                index >= 0 && index < visibleCards.Count ? visibleCards[index] : null);
        }

        public int BookStateIconIndexForVisibleIndexForTest(int index)
        {
            return CombatAbilityModalPresentationRules.BookStateIconIndex(
                BookStateForVisibleIndexForTest(index));
        }

        public void Bind(CombatAbilityModalBindings modalBindings)
        {
            bindings = modalBindings;
            Build();
            SetVisible(false);
            Refresh();
        }

        public bool SetVisible(bool visible)
        {
            EventSystem eventSystem = EventSystem.current;
            GameObject selectedBeforeChange = eventSystem == null ? null : eventSystem.currentSelectedGameObject;
            bool ownedSelection = IsCanvasSelection(selectedBeforeChange);
            if (visible) UiRuntime.EnsureEventSystemReady();
            if (visible) CaptureNavigationEvents();
            else ReleaseNavigationEvents();
            bool changed = UiRuntime.SetCanvasVisible(canvas, visible);
            if (canvasGroup != null)
            {
                canvasGroup.alpha = visible ? 1f : 0f;
                canvasGroup.interactable = visible;
                canvasGroup.blocksRaycasts = visible;
            }
            if (changed && visible)
            {
                openedFrame = Time.frameCount;
                CombatAbilityModalNavigationStep seed = CombatAbilityModalNavigationRules.SeedVertical(
                    Input.GetAxisRaw("Vertical"),
                    Time.unscaledTime);
                heldVerticalDirection = seed.HeldDirection;
                nextVerticalRepeatAt = seed.NextRepeatAt;
                hoveredId = "";
                CancelPendingPointerPreview();
                lastWidth = -1f;
                lastHeight = -1f;
                RefreshSelectionPresentation();
                FocusSelectedControl();
            }
            else if (changed)
            {
                hoveredId = "";
                CancelPendingPointerPreview();
                heldVerticalDirection = 0;
                nextVerticalRepeatAt = 0f;
                if (ownedSelection && eventSystem != null) eventSystem.SetSelectedGameObject(null);
            }
            return changed;
        }

        private bool IsCanvasSelection(GameObject selected)
        {
            if (selected == null || canvas == null) return false;
            Transform selectedTransform = selected.transform;
            Transform canvasTransform = canvas.transform;
            return selectedTransform == canvasTransform || selectedTransform.IsChildOf(canvasTransform);
        }

        private void OnDisable()
        {
            CancelPendingPointerPreview();
            ReleaseNavigationEvents();
        }

        private void OnDestroy()
        {
            ReleaseNavigationEvents();
            if (generatedBookStateIconAtlas != null)
            {
                Destroy(generatedBookStateIconAtlas);
                generatedBookStateIconAtlas = null;
            }
        }

        private void CaptureNavigationEvents()
        {
            EventSystem current = EventSystem.current;
            if (current == null) return;
            if (navigationEventsCaptured && capturedEventSystem == current) return;
            ReleaseNavigationEvents();
            capturedEventSystem = current;
            previousSendNavigationEvents = current.sendNavigationEvents;
            current.sendNavigationEvents = false;
            navigationEventsCaptured = true;
        }

        private void ReleaseNavigationEvents()
        {
            if (!navigationEventsCaptured) return;
            if (capturedEventSystem != null)
            {
                capturedEventSystem.sendNavigationEvents = previousSendNavigationEvents;
            }
            capturedEventSystem = null;
            navigationEventsCaptured = false;
        }

        public void Refresh()
        {
            lastRefreshSucceeded = false;
            if (bindings == null || canvas == null) return;
            CombatAbilityModalView view = bindings.View == null ? null : bindings.View();
            currentView = view;
            if (view == null || !view.Visible)
            {
                SetVisible(false);
                return;
            }

            if (!Mathf.Approximately(lastWidth, Screen.width) || !Mathf.Approximately(lastHeight, Screen.height))
            {
                ApplyLayout();
            }

            Color accent = view.Spellbook ? Hex("a77ae8", 1f) : Hex("66cdb9", 1f);
            accentStrip.GetComponent<Image>().color = accent;
            panel.GetComponent<Outline>().effectColor = accent.WithAlpha(0.92f);
            titleText.color = Hex("f3ead7", 1f);
            titleText.text = string.IsNullOrWhiteSpace(view.Title) ? "Power Book" : view.Title;
            actorText.text = string.IsNullOrWhiteSpace(view.Actor) ? view.Header ?? "" : view.Actor;
            resourceText.text = view.Resource ?? "";
            actionStateText.text = view.ActionState ?? "";
            actionStateText.color = (view.ActionState ?? "").IndexOf("READY", StringComparison.OrdinalIgnoreCase) >= 0
                ? Hex("8ed7c7", 1f)
                : Hex("d7a84e", 1f);
            traitText.text = ImmediateTraitText(view.Trait);

            string nextContextKey = string.IsNullOrWhiteSpace(view.ContextKey)
                ? (view.Spellbook ? "spellbook" : "skills")
                : view.ContextKey;
            bool contextChanged = !string.Equals(listContextKey, nextContextKey, StringComparison.Ordinal);
            listContextKey = nextContextKey;
            if (contextChanged)
            {
                selectedId = "";
                hoveredId = "";
                CancelPendingPointerPreview();
                filterInitialized = false;
                pendingFilterSelectionId = "";
                pendingFilterScrollY = 0f;
                pendingFilterRestore = false;
                if (listContent != null) listContent.anchoredPosition = Vector2.zero;
            }

            IReadOnlyList<CombatAbilityModalCardView> cards = view.Cards ?? Array.Empty<CombatAbilityModalCardView>();
            if (!filterInitialized)
            {
                currentFilter = CombatAbilityModalPresentationRules.InitialFilter(cards);
                filterInitialized = true;
            }
            else if (cards.Count > 0
                && CombatAbilityModalPresentationRules.Count(cards, currentFilter) == 0)
            {
                CombatAbilityModalFilter fallback = CombatAbilityModalPresentationRules.InitialFilter(cards);
                if (fallback != currentFilter)
                {
                    currentFilter = fallback;
                    hoveredId = "";
                    if (listContent != null) listContent.anchoredPosition = Vector2.zero;
                }
            }

            RefreshFilters(cards, accent);
            RebuildVisibleCards(cards);
            ReconcileSelection(pendingFilterRestore ? pendingFilterSelectionId : view.SelectedId);
            if (IndexOfVisible(hoveredId) < 0
                || string.Equals(hoveredId, selectedId, StringComparison.Ordinal))
            {
                hoveredId = "";
            }
            if (IndexOfVisible(pendingHoveredId) < 0
                || string.Equals(pendingHoveredId, selectedId, StringComparison.Ordinal))
            {
                CancelPendingPointerPreview();
            }
            activeCardCount = visibleCards.Count;
            EnsureRowCount(visibleCards.Count);
            emptyText.gameObject.SetActive(visibleCards.Count == 0);
            emptyText.text = visibleCards.Count == 0
                ? EmptyFilterText(view)
                : "";
            if (closeButtonLabel != null)
            {
                closeButtonLabel.text = CombatAbilityModalPresentationRules.BackButtonLabel(cards);
            }

            CombatAbilityModalCardView selected = null;
            for (int i = 0; i < cardRows.Count; i++)
            {
                bool visible = i < visibleCards.Count;
                CardRow row = cardRows[i];
                row.Root.gameObject.SetActive(visible);
                if (!visible) continue;
                CombatAbilityModalCardView card = visibleCards[i];
                if (string.Equals(card.Id, selectedId, StringComparison.Ordinal)) selected = card;
                RefreshRow(row, card, accent);
            }

            if (selected == null && visibleCards.Count > 0)
            {
                selected = visibleCards[0];
                selectedId = selected.Id ?? "";
            }
            CombatAbilityModalCardView detail = FindVisibleCard(DetailId()) ?? selected;
            RefreshDetail(detail, accent);
            LayoutRows();
            EnsureSelectedVisible();
            Canvas.ForceUpdateCanvases();
            if (pendingFilterRestore
                && string.Equals(selectedId, pendingFilterSelectionId, StringComparison.Ordinal)
                && listContent != null
                && listViewport != null)
            {
                float maxScrollY = Mathf.Max(0f, listContent.rect.height - listViewport.rect.height);
                listContent.anchoredPosition = new Vector2(
                    0f,
                    Mathf.Clamp(pendingFilterScrollY, 0f, maxScrollY));
            }
            pendingFilterSelectionId = "";
            pendingFilterScrollY = 0f;
            pendingFilterRestore = false;
            RefreshScrollHint();
            lastRefreshSucceeded = true;
        }

        public void SetFilterForTest(CombatAbilityModalFilter filter)
        {
            SetFilter(filter);
        }

        public void MoveSelectionForTest(int delta)
        {
            MoveSelection(delta);
        }

        public void SelectVisibleIndexForTest(int index)
        {
            SelectVisibleIndex(index, true, SelectionOrigin.PointerClick);
        }

        public void HoverVisibleIndexForTest(int index)
        {
            CommitPointerPreview(index);
        }

        public void QueuePointerPreviewForTest(int index)
        {
            QueuePointerPreview(index);
        }

        public bool CommitPointerPreviewForTest(float now)
        {
            return CommitPendingPointerPreview(now);
        }

        public void ClearHoverForTest()
        {
            ClearHover();
        }

        public void InvokeSelectedForTest()
        {
            CombatAbilityModalCardView card = FindVisibleCard(selectedId);
            if (!CombatAbilityModalPresentationRules.CanActivate(card))
            {
                throw new InvalidOperationException("Selected modal power is not activatable.");
            }
            ActivateSelected();
        }

        public void InvokeDetailActionForTest()
        {
            ActivateDetail();
        }

        public bool ScrollDetailForTest(float normalizedDelta)
        {
            if (!DetailNarrativeCanScroll() || detailNarrativeScroll == null) return false;
            return SetDetailNarrativePosition(
                Mathf.Clamp01(detailNarrativeScroll.verticalNormalizedPosition + normalizedDelta));
        }

        public bool ScrollDetailPageForTest(int direction)
        {
            return ScrollDetailPage(direction);
        }

        public void SetDetailNarrativeForTest(string notes, string prompt)
        {
            if (detailNotes == null || detailNotesLabel == null || detailPrompt == null) return;
            detailNotes.text = notes ?? "";
            detailNotes.gameObject.SetActive(!string.IsNullOrWhiteSpace(detailNotes.text));
            detailNotesLabel.gameObject.SetActive(detailNotes.gameObject.activeSelf);
            detailPrompt.text = prompt ?? "";
            detailPrompt.gameObject.SetActive(!string.IsNullOrWhiteSpace(detailPrompt.text));
            LayoutDetailNarrative(detailPrompt.gameObject.activeSelf);
            Canvas.ForceUpdateCanvases();
            LayoutDetailNarrative(detailPrompt.gameObject.activeSelf);
        }

        public bool IsSelectedVisibleForTest()
        {
            int index = IndexOfVisible(selectedId);
            if (index < 0 || listViewport == null || listContent == null) return false;
            float top = index * (rowHeight + rowGap) + 4f;
            float bottom = top + rowHeight;
            float viewportTop = Mathf.Max(0f, listContent.anchoredPosition.y);
            float viewportBottom = viewportTop + listViewport.rect.height;
            return top >= viewportTop - 1f && bottom <= viewportBottom + 1f;
        }

        private void Update()
        {
            if (!IsVisible || Time.frameCount <= openedFrame) return;

            CommitPendingPointerPreview(Time.unscaledTime);

            if (DetailNarrativeCanScroll() && detailNarrativeScroll != null)
            {
                float detailVertical = Input.GetAxisRaw("DetailVertical");
                if (Mathf.Abs(detailVertical) >= CombatAbilityModalDetailScrollRules.AxisThreshold)
                {
                    ClearPointerPreview(true);
                }
                float nextDetailPosition = CombatAbilityModalDetailScrollRules.ApplyAxis(
                    detailNarrativeScroll.verticalNormalizedPosition,
                    detailVertical,
                    Time.unscaledDeltaTime);
                SetDetailNarrativePosition(nextDetailPosition);
            }

            float controllerVertical = Input.GetAxisRaw("Vertical");
            CombatAbilityModalNavigationStep navigation = CombatAbilityModalNavigationRules.ResolveVertical(
                controllerVertical,
                heldVerticalDirection,
                nextVerticalRepeatAt,
                Time.unscaledTime);
            heldVerticalDirection = navigation.HeldDirection;
            nextVerticalRepeatAt = navigation.NextRepeatAt;
            if (Mathf.Abs(controllerVertical) >= CombatAbilityModalNavigationRules.AxisThreshold)
            {
                ClearPointerPreview(true);
            }

            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            {
                SetFilter(CombatAbilityModalFilter.Ready);
                return;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            {
                SetFilter(CombatAbilityModalFilter.Learned);
                return;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
            {
                SetFilter(CombatAbilityModalFilter.Future);
                return;
            }
            if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
            {
                SetFilter(CombatAbilityModalFilter.All);
                return;
            }
            if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.JoystickButton5))
            {
                bool reverse = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                CycleFilter(reverse ? -1 : 1);
                return;
            }
            if (Input.GetKeyDown(KeyCode.JoystickButton4))
            {
                CycleFilter(-1);
                return;
            }
            if (Input.GetKeyDown(KeyCode.Home))
            {
                SelectVisibleIndex(0, true);
                return;
            }
            if (Input.GetKeyDown(KeyCode.End))
            {
                SelectVisibleIndex(visibleCards.Count - 1, true);
                return;
            }
            if (Input.GetKeyDown(KeyCode.PageUp))
            {
                ClearPointerPreview(true);
                if (!ScrollDetailPage(1)) MoveSelection(-PageSize());
                return;
            }
            if (Input.GetKeyDown(KeyCode.PageDown))
            {
                ClearPointerPreview(true);
                if (!ScrollDetailPage(-1)) MoveSelection(PageSize());
                return;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                MoveSelection(-1);
                return;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                MoveSelection(1);
                return;
            }
            if (Input.GetKeyDown(KeyCode.Return)
                || Input.GetKeyDown(KeyCode.KeypadEnter)
                || Input.GetKeyDown(KeyCode.Space)
                || Input.GetKeyDown(KeyCode.JoystickButton0))
            {
                ActivateFromNavigation();
                return;
            }
            if (Input.GetKeyDown(KeyCode.JoystickButton1))
            {
                bindings?.Close?.Invoke();
                return;
            }

            if (navigation.Direction != 0) MoveSelection(navigation.Direction);
        }

        private void RefreshFilters(IReadOnlyList<CombatAbilityModalCardView> cards, Color accent)
        {
            if (filterButtons == null || filterLabels == null) return;
            LayoutFilterButtons();
            for (int i = 0; i < Filters.Length; i++)
            {
                CombatAbilityModalFilter filter = Filters[i];
                filterButtons[i].gameObject.SetActive(true);
                bool selected = filter == currentFilter;
                int count = CombatAbilityModalPresentationRules.Count(cards, filter);
                filterLabels[i].text = CombatAbilityModalPresentationRules.FilterLabel(filter, cards);
                filterLabels[i].color = selected ? Hex("07100f", 1f) : count > 0 ? Hex("f3ead7", 1f) : Hex("7d8586", 1f);
                Color filterFill = selected ? accent : Hex("11171b", 0.98f);
                ColorBlock colors = filterButtons[i].colors;
                colors.normalColor = Color.white;
                colors.highlightedColor = Color.white;
                colors.selectedColor = Color.white;
                colors.pressedColor = new Color(0.84f, 0.84f, 0.84f, 1f);
                colors.disabledColor = new Color(0.56f, 0.56f, 0.56f, 1f);
                filterButtons[i].colors = colors;
                Image image = filterButtons[i].targetGraphic as Image;
                if (image != null) image.color = filterFill;
                filterButtons[i].interactable = selected || count > 0 || filter == CombatAbilityModalFilter.All;
                Outline outline = filterButtons[i].GetComponent<Outline>();
                if (outline != null) outline.effectColor = selected ? accent : Hex("3c4544", 0.78f);
            }
        }

        private void LayoutFilterButtons()
        {
            if (filterButtons == null) return;
            CombatAbilityModalGeometry geometry = CombatAbilityModalLayout.Calculate(Screen.width, Screen.height);
            int visibleCount = Filters.Length;
            float filterGap = 8f;
            float filterWidth = (geometry.Filters.width - filterGap * (visibleCount - 1)) / visibleCount;
            for (int i = 0; i < Filters.Length; i++)
            {
                SetLocalRect(
                    filterButtons[i].GetComponent<RectTransform>(),
                    new Rect(
                        geometry.Filters.x + i * (filterWidth + filterGap),
                        geometry.Filters.y,
                        filterWidth,
                        geometry.Filters.height));
            }
        }

        private void RebuildVisibleCards(IReadOnlyList<CombatAbilityModalCardView> cards)
        {
            visibleCards.Clear();
            if (cards == null) return;
            for (int i = 0; i < cards.Count; i++)
            {
                CombatAbilityModalCardView card = cards[i];
                if (CombatAbilityModalPresentationRules.MatchesFilter(card, currentFilter)) visibleCards.Add(card);
            }
        }

        private void ReconcileSelection(string requestedId)
        {
            if (!string.IsNullOrWhiteSpace(requestedId) && IndexOfVisible(requestedId) >= 0)
            {
                if (!string.Equals(selectedId, requestedId, StringComparison.Ordinal)) hoveredId = "";
                selectedId = requestedId;
                return;
            }
            if (IndexOfVisible(selectedId) >= 0) return;

            hoveredId = "";
            selectedId = "";
            for (int i = 0; i < visibleCards.Count; i++)
            {
                if (visibleCards[i].Selected)
                {
                    selectedId = visibleCards[i].Id ?? "";
                    break;
                }
            }
            if (string.IsNullOrWhiteSpace(selectedId))
            {
                for (int i = 0; i < visibleCards.Count; i++)
                {
                    if (visibleCards[i].Ready)
                    {
                        selectedId = visibleCards[i].Id ?? "";
                        break;
                    }
                }
            }
            if (string.IsNullOrWhiteSpace(selectedId) && visibleCards.Count > 0)
            {
                selectedId = visibleCards[0].Id ?? "";
            }
        }

        private string EmptyFilterText(CombatAbilityModalView view)
        {
            switch (currentFilter)
            {
                case CombatAbilityModalFilter.Ready:
                    return "Nothing can be used from this position. Open Known to see requirements and tactical notes.";
                case CombatAbilityModalFilter.Learned:
                    return "No powers have been learned yet.";
                case CombatAbilityModalFilter.Future:
                    return "This book has no locked powers.";
                default:
                    return string.IsNullOrWhiteSpace(view?.EmptyText) ? "Nothing is available right now." : view.EmptyText;
            }
        }

        private void RefreshRow(CardRow row, CombatAbilityModalCardView card, Color accent)
        {
            row.Id = card.Id ?? "";
            bool selected = string.Equals(row.Id, selectedId, StringComparison.Ordinal);
            bool hovered = string.Equals(row.Id, hoveredId, StringComparison.Ordinal);
            bool canActivate = CombatAbilityModalPresentationRules.CanActivate(card);
            bool armed = card.Ready && canActivate;
            bool exceptionalState = CombatAbilityModalPresentationRules.ShouldShowRowBadge(
                card,
                currentFilter,
                currentView?.ActionState);
            Color cardAccent = CardAccent(card, accent);
            Color previewAccent = Hex("8ecbd7", 1f);
            Color selectionAccent = Hex("58b7a5", 1f);
            Color armedAccent = Hex("d7a84e", 1f);
            Color fill = selected
                ? card.Locked
                    ? Hex("10181c", 0.98f)
                    : Hex("102126", 0.98f)
                : hovered
                    ? Color.Lerp(Hex("12181d", 0.98f), previewAccent, 0.08f)
                : card.Locked
                    ? Hex("090c0f", 0.98f)
                    : Hex("12181d", 0.98f);
            row.Button.interactable = true;
            row.Background.color = fill;
            row.Outline.effectColor = armed
                ? armedAccent.WithAlpha(0.98f)
                : selected
                ? selectionAccent.WithAlpha(0.96f)
                : hovered
                    ? previewAccent.WithAlpha(0.46f)
                    : card.Locked
                        ? Hex("303638", 0.72f)
                        : Hex("3c4544", 0.82f);
            row.Outline.effectDistance = selected || armed ? new Vector2(2f, -2f) : new Vector2(1f, -1f);
            bool showSelectionCue = selected;
            bool showPreviewCue = hovered && !selected;
            row.SelectionRail.gameObject.SetActive(showSelectionCue);
            row.SelectionRail.color = selectionAccent;
            row.SelectionChevron.gameObject.SetActive(false);
            SetBookStateIcon(
                row.SelectionIcon,
                showPreviewCue
                    ? CombatIconCatalog.BookStatePreviewIndex
                    : CombatIconCatalog.BookStateSelectionIndex,
                showPreviewCue ? previewAccent : selectionAccent,
                showSelectionCue || showPreviewCue);
            row.IconFrame.GetComponent<Outline>().effectColor = cardAccent.WithAlpha(card.Locked ? 0.38f : 0.82f);
            RefreshIcon(row.Icon, row.Sigil, card, cardAccent);
            row.Name.text = card.Name ?? "";
            row.Meta.text = CombatAbilityModalPresentationRules.RowMeta(card);
            row.Summary.text = FirstNonEmpty(card.RowSummary, card.Summary, card.CurrentEffect);
            row.State.text = CombatAbilityModalPresentationRules.AvailabilityLabel(card);
            row.State.color = AvailabilityTextColor(card);
            row.StatusBadge.gameObject.SetActive(exceptionalState);
            row.StatusBadge.GetComponent<Image>().color = AvailabilityFill(card);
            row.StatusBadge.GetComponent<Outline>().effectColor = AvailabilityTextColor(card).WithAlpha(0.72f);
            SetBookStateIcon(
                row.StatusIcon,
                BookStateIndex(card),
                AvailabilityTextColor(card),
                exceptionalState);
            row.Name.color = card.Locked
                ? Hex("9aa0a1", 1f)
                : showPreviewCue ? Hex("b9edf1", 1f) : Hex("f3ead7", 1f);
            row.Summary.color = card.Locked ? Hex("7d8586", 1f) : Hex("d8d0c1", 1f);
            row.Meta.color = canActivate ? Hex("d7a84e", 1f) : Hex("a79c87", 1f);

            ColorBlock colors = row.Button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = Color.white;
            colors.selectedColor = Color.white;
            colors.pressedColor = new Color(0.82f, 0.82f, 0.82f, 1f);
            colors.disabledColor = Color.white;
            row.Button.colors = colors;
        }

        private void RefreshDetail(CombatAbilityModalCardView card, Color accent)
        {
            if (card == null)
            {
                if (detailPanel != null)
                {
                    detailPanel.GetComponent<Image>().color = Hex("080b0d", 0.98f);
                    detailPanel.GetComponent<Outline>().effectColor = Hex("3c4544", 0.42f);
                }
                if (detailArtBackdrop != null)
                {
                    detailArtBackdrop.GetComponent<Image>().color = Hex("0b1013", 0.98f);
                    detailArtBackdrop.GetComponent<Outline>().effectColor = Hex("3c4544", 0.42f);
                }
                if (detailIconFrame != null)
                {
                    detailIconFrame.GetComponent<Outline>().effectColor = Hex("3c4544", 0.42f);
                }
                RefreshIcon(detailIcon, detailSigil, null, accent);
                detailIcon.rectTransform.localScale = Vector3.one;
                detailSigil.rectTransform.localScale = Vector3.one;
                detailTitle.text = "No selection";
                detailStatus.text = "";
                detailStatus.gameObject.SetActive(false);
                SetBookStateIcon(
                    detailStatusIcon,
                    CombatIconCatalog.BookStateBlockedIndex,
                    Hex("9aa0a1", 1f),
                    false);
                detailContext.text = "";
                detailContext.gameObject.SetActive(false);
                SetBookStateIcon(
                    detailContextIcon,
                    CombatIconCatalog.BookStatePreviewIndex,
                    accent,
                    false);
                detailSummary.text = "Choose a filter or select a power.";
                detailMeta.text = "";
                detailMeta.gameObject.SetActive(false);
                detailNotesLabel.gameObject.SetActive(false);
                detailNotes.text = "";
                detailNotes.gameObject.SetActive(false);
                detailPrompt.text = "";
                detailPrompt.gameObject.SetActive(false);
                detailActionLabel.text = "Unavailable";
                Color unavailableFill = Hex("101417", 0.96f);
                ColorBlock unavailableColors = detailActionButton.colors;
                unavailableColors.normalColor = Color.white;
                unavailableColors.highlightedColor = Color.white;
                unavailableColors.selectedColor = Color.white;
                unavailableColors.pressedColor = Color.white;
                unavailableColors.disabledColor = Color.white;
                detailActionButton.colors = unavailableColors;
                detailActionButton.interactable = false;
                Image unavailableImage = detailActionButton.targetGraphic as Image;
                if (unavailableImage != null) unavailableImage.color = unavailableFill;
                detailActionLabel.color = Hex("9aa0a1", 1f);
                SetStatChip(0, "COST", "—");
                SetStatChip(1, "REACH", "—");
                SetStatChip(2, "TARGET", "—");
                LayoutDetailNarrative(false);
                return;
            }

            Color cardAccent = CardAccent(card, accent);
            bool canActivate = CombatAbilityModalPresentationRules.CanActivate(card);
            bool previewing = IsPointerPreview(card);
            bool armed = card.Ready && canActivate;
            Color selectionAccent = Hex("58b7a5", 1f);
            Color armedAccent = Hex("d7a84e", 1f);
            detailPanel.GetComponent<Image>().color = Color.Lerp(Hex("080b0d", 0.98f), cardAccent.WithAlpha(0.98f), 0.06f);
            detailPanel.GetComponent<Outline>().effectColor = previewing
                ? Hex("8ecbd7", 0.46f)
                : armed
                    ? armedAccent.WithAlpha(0.94f)
                    : selectionAccent.WithAlpha(0.82f);
            detailArtBackdrop.GetComponent<Image>().color = Color.Lerp(
                Hex("080b0d", 0.98f),
                cardAccent.WithAlpha(0.98f),
                previewing ? 0.10f : 0.20f);
            detailArtBackdrop.GetComponent<Outline>().effectColor = cardAccent.WithAlpha(previewing ? 0.34f : 0.58f);
            detailIconFrame.GetComponent<Outline>().effectColor = cardAccent.WithAlpha(previewing ? 0.58f : 0.88f);
            RefreshIcon(detailIcon, detailSigil, card, cardAccent);
            float heroScale = card.Impact?.IndexOf("field", StringComparison.OrdinalIgnoreCase) >= 0 ? 1.16f : 1f;
            detailIcon.rectTransform.localScale = Vector3.one * heroScale;
            detailSigil.rectTransform.localScale = Vector3.one * heroScale;
            detailTitle.text = card.Name ?? "";
            detailStatus.text = CombatAbilityModalPresentationRules.AvailabilityLabel(card);
            detailStatus.color = AvailabilityTextColor(card);
            bool detailStatusVisible =
                card.Locked
                || !card.Usable
                || !CombatAbilityModalPresentationRules.HasCurrentTarget(card);
            detailStatus.gameObject.SetActive(detailStatusVisible);
            SetBookStateIcon(
                detailStatusIcon,
                BookStateIndex(card),
                AvailabilityTextColor(card),
                detailStatusVisible);
            detailContext.text = previewing
                ? "PREVIEW"
                : card.Ready && canActivate
                    ? "TARGETING ARMED"
                    : "";
            detailContext.color = previewing
                ? cardAccent.WithAlpha(0.92f)
                : card.Ready && canActivate
                    ? Hex("d7a84e", 1f)
                    : Hex("8ed7c7", 1f);
            detailContext.gameObject.SetActive(!string.IsNullOrWhiteSpace(detailContext.text));
            SetBookStateIcon(
                detailContextIcon,
                previewing
                    ? CombatIconCatalog.BookStatePreviewIndex
                    : CombatIconCatalog.BookStateTargetingIndex,
                detailContext.color,
                detailContext.gameObject.activeSelf);
            detailSummary.text = FirstNonEmpty(card.CurrentEffect, card.Summary, card.RowSummary);
            detailMeta.text = CombatAbilityModalPresentationRules.DetailMeta(card);
            detailMeta.gameObject.SetActive(!string.IsNullOrWhiteSpace(detailMeta.text));
            detailNotes.text = CombatAbilityModalPresentationRules.DetailNotes(card);
            detailNotes.gameObject.SetActive(!string.IsNullOrWhiteSpace(detailNotes.text));
            detailNotesLabel.gameObject.SetActive(detailNotes.gameObject.activeSelf);
            detailPrompt.text = previewing
                ? "Preview only. Click or focus the card to select it."
                : RelevantContext(card);
            detailPrompt.gameObject.SetActive(!string.IsNullOrWhiteSpace(detailPrompt.text));
            detailPrompt.color = previewing
                ? Hex("8ecbd7", 0.92f)
                : canActivate
                    ? Hex("8ed7c7", 1f)
                    : Hex("e0b96a", 1f);
            detailActionLabel.text = previewing
                ? "Preview Only"
                : CombatAbilityModalPresentationRules.CardActionLabel(card);
            bool detailActionAvailable = !previewing && canActivate;
            Color actionFill = detailActionAvailable
                ? armed ? armedAccent : Hex("285d56", 1f)
                : Hex("101417", 0.96f);
            ColorBlock actionColors = detailActionButton.colors;
            actionColors.normalColor = Color.white;
            actionColors.highlightedColor = Color.white;
            actionColors.selectedColor = Color.white;
            actionColors.pressedColor = detailActionAvailable
                ? new Color(0.86f, 0.86f, 0.86f, 1f)
                : Color.white;
            actionColors.disabledColor = Color.white;
            detailActionButton.colors = actionColors;
            Image actionImage = detailActionButton.targetGraphic as Image;
            if (actionImage != null) actionImage.color = actionFill;
            detailActionButton.interactable = detailActionAvailable;
            detailActionLabel.color = detailActionAvailable
                ? CardActionTextColor(actionFill)
                : Hex("9aa0a1", 1f);
            SetStatChip(0, "COST", card.Cost);
            SetStatChip(1, "REACH", CompactReach(card));
            SetStatChip(2, "TARGET", CombatAbilityModalPresentationRules.TargetCountLabel(card));
            LayoutDetailNarrative(detailPrompt.gameObject.activeSelf);
        }

        private void Build()
        {
            if (canvas != null) return;
            EnsureEventSystem();
            font = UiRuntime.DefaultFont;
            canvas = UiRuntime.CreateOwnedRootCanvas(this, "Combat Ability Modal Canvas");
            UiRuntime.ConfigureOverlayCanvas(canvas, 180);
            UiRuntime.SetCanvasVisible(canvas, false);
            canvasGroup = canvas.gameObject.AddComponent<CanvasGroup>();
            Stretch(canvas.GetComponent<RectTransform>());

            backdrop = AddImage("Backdrop", canvas.transform, Hex("020303", 0.78f)).rectTransform;
            panel = AddPanel("Combat Ability Modal", canvas.transform, Hex("0d1216", 0.995f), Hex("a77ae8", 0.92f));
            accentStrip = AddImage("Accent Strip", panel, Hex("a77ae8", 1f)).rectTransform;
            titleText = AddText("Title", panel, "", 24, Hex("f3ead7", 1f), TextAnchor.MiddleLeft);
            actorText = AddText("Actor", panel, "", 13, Hex("d8d0c1", 1f), TextAnchor.MiddleLeft);
            resourceText = AddText("Resource", panel, "", 12, Hex("d7a84e", 1f), TextAnchor.MiddleRight);
            actionStateText = AddText("Action State", panel, "", 12, Hex("8ed7c7", 1f), TextAnchor.MiddleRight);
            traitText = AddText("Trait", panel, "", 12, Hex("b7aa90", 1f), TextAnchor.MiddleLeft);
            closeButton = AddButton("Back", panel, "Back to Battle  [Esc]", () => bindings?.Close?.Invoke());
            closeButtonLabel = closeButton.GetComponentInChildren<Text>();

            filterButtons = new Button[Filters.Length];
            filterLabels = new Text[Filters.Length];
            for (int i = 0; i < Filters.Length; i++)
            {
                int filterIndex = i;
                filterButtons[i] = AddButton("Filter " + Filters[i], panel, Filters[i].ToString(), () => SetFilter(Filters[filterIndex]));
                filterLabels[i] = filterButtons[i].GetComponentInChildren<Text>();
                filterLabels[i].resizeTextForBestFit = true;
                filterLabels[i].resizeTextMinSize = 9;
                filterLabels[i].resizeTextMaxSize = 12;
            }

            listFrame = AddPanel("Power List", panel, Hex("06090b", 0.86f), Hex("3c4544", 0.82f));
            listScroll = listFrame.gameObject.AddComponent<ScrollRect>();
            listScroll.horizontal = false;
            listScroll.movementType = ScrollRect.MovementType.Clamped;
            listScroll.scrollSensitivity = 38f;
            listViewport = AddImage("List Viewport", listFrame, Color.white).rectTransform;
            Mask listMask = listViewport.gameObject.AddComponent<Mask>();
            listMask.showMaskGraphic = false;
            listContent = new GameObject("List Content", typeof(RectTransform)).GetComponent<RectTransform>();
            listContent.SetParent(listViewport, false);
            listScroll.content = listContent;
            listScroll.viewport = listViewport;
            listScrollbar = AddVerticalScrollbar("List Scrollbar", listFrame);
            listScroll.verticalScrollbar = listScrollbar;
            listScroll.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
            emptyText = AddText("Empty", listViewport, "", 13, Hex("b7aa90", 1f), TextAnchor.MiddleCenter);
            listScrollHint = AddText("List Position", panel, "", 11, Hex("d7a84e", 0.94f), TextAnchor.MiddleRight);
            listScrollHint.fontStyle = FontStyle.Bold;
            listScrollHint.raycastTarget = false;

            detailPanel = AddPanel("Power Detail", panel, Hex("080b0d", 0.98f), Hex("a77ae8", 0.72f));
            detailArtBackdrop = AddPanel("Detail Art Backdrop", detailPanel, Hex("0b1013", 0.98f), Hex("a77ae8", 0.42f));
            detailIconFrame = AddPanel("Detail Icon Frame", detailPanel, Hex("050708", 0.98f), Hex("a77ae8", 0.88f));
            detailIcon = AddImage("Detail Icon", detailIconFrame, Color.white);
            detailIcon.preserveAspect = true;
            detailIcon.raycastTarget = false;
            detailSigil = AddText("Detail Sigil", detailIconFrame, "", 14, Hex("f3ead7", 1f), TextAnchor.MiddleCenter);
            detailSigil.fontStyle = FontStyle.Bold;
            detailSigil.raycastTarget = false;
            detailTitle = AddText("Detail Title", detailPanel, "", 20, Hex("f3ead7", 1f), TextAnchor.MiddleLeft);
            detailStatusIcon = AddImage("Detail Status Icon", detailPanel, Color.white);
            detailStatusIcon.preserveAspect = true;
            detailStatusIcon.raycastTarget = false;
            detailStatus = AddText("Detail Status", detailPanel, "", 12, Hex("8ed7c7", 1f), TextAnchor.MiddleLeft);
            detailStatus.fontStyle = FontStyle.Bold;
            detailContextIcon = AddImage("Detail Context Icon", detailPanel, Color.white);
            detailContextIcon.preserveAspect = true;
            detailContextIcon.raycastTarget = false;
            detailContext = AddText("Detail Context", detailPanel, "", 10, Hex("8ed7c7", 1f), TextAnchor.MiddleLeft);
            detailContext.fontStyle = FontStyle.Bold;
            statChips = new[]
            {
                CreateStatChip("Cost", detailPanel, CombatIconCatalog.BookStateCostIndex),
                CreateStatChip("Reach", detailPanel, CombatIconCatalog.BookStateReachIndex),
                CreateStatChip("Target", detailPanel, CombatIconCatalog.BookStateTargetIndex)
            };
            detailSummary = AddText("Current Effect", detailPanel, "", 14, Hex("f3ead7", 1f), TextAnchor.UpperLeft);
            detailSummary.fontStyle = FontStyle.Bold;
            detailMeta = AddText("Power Profile", detailPanel, "", 10, Hex("b7aa90", 1f), TextAnchor.UpperLeft);
            detailNarrativeFrame = AddPanel(
                "Narrative",
                detailPanel,
                Hex("05080a", 0.74f),
                Hex("3c4544", 0.54f));
            detailNarrativeScroll = detailNarrativeFrame.gameObject.AddComponent<ScrollRect>();
            detailNarrativeScroll.horizontal = false;
            detailNarrativeScroll.vertical = true;
            detailNarrativeScroll.movementType = ScrollRect.MovementType.Clamped;
            detailNarrativeScroll.scrollSensitivity = 26f;
            detailNarrativeViewport = AddImage(
                "Narrative Viewport",
                detailNarrativeFrame,
                Color.white).rectTransform;
            Mask narrativeMask = detailNarrativeViewport.gameObject.AddComponent<Mask>();
            narrativeMask.showMaskGraphic = false;
            detailNarrativeContent = new GameObject(
                "Narrative Content",
                typeof(RectTransform)).GetComponent<RectTransform>();
            detailNarrativeContent.SetParent(detailNarrativeViewport, false);
            detailNarrativeScroll.content = detailNarrativeContent;
            detailNarrativeScroll.viewport = detailNarrativeViewport;
            detailNarrativeScrollbar = AddVerticalScrollbar(
                "Narrative Scrollbar",
                detailNarrativeFrame);
            detailNarrativeScroll.verticalScrollbar = detailNarrativeScrollbar;
            detailNarrativeScroll.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
            detailNotesLabel = AddText("Tactical Notes Label", detailNarrativeContent, "TACTICAL NOTES", 10, Hex("8ed7c7", 1f), TextAnchor.MiddleLeft);
            detailNotesLabel.fontStyle = FontStyle.Bold;
            detailNotes = AddText("Tactical Notes", detailNarrativeContent, "", 11, Hex("d8d0c1", 1f), TextAnchor.UpperLeft);
            detailPrompt = AddText("Detail Prompt", detailNarrativeContent, "", 12, Hex("8ed7c7", 1f), TextAnchor.UpperLeft);
            detailActionButton = AddButton("Primary Action", detailPanel, "Choose Target", ActivateDetail);
            detailActionLabel = detailActionButton.GetComponentInChildren<Text>();
            footerHint = AddText(
                "Keyboard Help",
                panel,
                BaseFooterHint,
                11,
                Hex("b7aa90", 1f),
                TextAnchor.MiddleLeft);
            footerHint.resizeTextForBestFit = true;
            footerHint.resizeTextMinSize = 9;
            footerHint.resizeTextMaxSize = 11;
        }

        private void ApplyLayout()
        {
            lastWidth = Screen.width;
            lastHeight = Screen.height;
            float uiScale = CombatAbilityModalListRules.UiScale(Screen.height);
            rowHeight = CombatAbilityModalListRules.RowHeight(Screen.height);
            rowGap = CombatAbilityModalListRules.RowGap(Screen.height);
            ApplyTypography(uiScale);

            CombatAbilityModalGeometry geometry = CombatAbilityModalLayout.Calculate(Screen.width, Screen.height);
            SetScreenRect(backdrop, geometry.Backdrop);
            SetScreenRect(panel, geometry.Panel);
            SetLocalRect(accentStrip, new Rect(0f, 0f, 6f, geometry.Panel.height));
            SetLocalRect(titleText.rectTransform, new Rect(24f, 8f, geometry.Panel.width * 0.40f, 38f));
            SetLocalRect(actorText.rectTransform, new Rect(26f, 46f, geometry.Panel.width * 0.38f, 22f));
            SetLocalRect(resourceText.rectTransform, new Rect(geometry.Panel.width * 0.44f, 18f, geometry.Panel.width * 0.15f, 20f));
            SetLocalRect(actionStateText.rectTransform, new Rect(geometry.Panel.width * 0.44f, 42f, geometry.Panel.width * 0.15f, 20f));
            SetLocalRect(traitText.rectTransform, new Rect(geometry.Panel.width * 0.60f, 68f, geometry.Panel.width * 0.37f, 22f));
            SetLocalRect(closeButton.GetComponent<RectTransform>(), geometry.CloseButton);

            LayoutFilterButtons();

            SetLocalRect(listFrame, geometry.List);
            SetLocalRect(listViewport, new Rect(7f, 7f, geometry.List.width - 34f, geometry.List.height - 14f));
            SetLocalRect(listScrollbar.GetComponent<RectTransform>(), new Rect(geometry.List.width - 22f, 8f, 15f, geometry.List.height - 16f));
            SetLocalRect(emptyText.rectTransform, new Rect(18f, 18f, geometry.List.width - 64f, geometry.List.height - 36f));
            SetLocalRect(
                listScrollHint.rectTransform,
                new Rect(geometry.Footer.xMax - 116f, geometry.Footer.y, 116f, geometry.Footer.height));

            SetLocalRect(detailPanel, geometry.Detail);
            float detailW = geometry.Detail.width;
            float detailH = geometry.Detail.height;
            float detailArtSize = Mathf.Clamp(78f * uiScale, 86f, 104f);
            SetLocalRect(detailArtBackdrop, new Rect(10f, 10f, detailArtSize + 12f, detailArtSize + 12f));
            SetLocalRect(detailIconFrame, new Rect(16f, 16f, detailArtSize, detailArtSize));
            Stretch(detailIcon.rectTransform, 5f, 5f);
            Stretch(detailSigil.rectTransform, 5f, 5f);
            float detailHeaderX = 30f + detailArtSize;
            SetLocalRect(detailTitle.rectTransform, new Rect(detailHeaderX, 15f, detailW - detailHeaderX - 16f, 28f));
            SetLocalRect(detailStatusIcon.rectTransform, new Rect(detailHeaderX, 54f, 18f, 18f));
            SetLocalRect(detailStatus.rectTransform, new Rect(detailHeaderX + 24f, 52f, detailW - detailHeaderX - 40f, 24f));
            SetLocalRect(detailContextIcon.rectTransform, new Rect(detailHeaderX, 78f, 16f, 16f));
            SetLocalRect(detailContext.rectTransform, new Rect(detailHeaderX + 22f, 78f, detailW - detailHeaderX - 38f, 18f));

            float summaryY = Mathf.Max(112f, detailArtSize + 28f);
            float statY = summaryY + 74f;
            float chipGap = 8f;
            float chipWidth = (detailW - 32f - chipGap * 2f) / 3f;
            SetLocalRect(detailSummary.rectTransform, new Rect(16f, summaryY, detailW - 32f, 66f));
            LayoutStatChip(statChips[0], new Rect(16f, statY, chipWidth, 44f));
            LayoutStatChip(statChips[1], new Rect(16f + chipWidth + chipGap, statY, chipWidth, 44f));
            LayoutStatChip(statChips[2], new Rect(16f + (chipWidth + chipGap) * 2f, statY, chipWidth, 44f));

            float actionY = detailH - 60f;
            SetLocalRect(detailActionButton.GetComponent<RectTransform>(), new Rect(16f, actionY, detailW - 32f, 44f));
            LayoutDetailNarrative(detailPrompt != null && detailPrompt.gameObject.activeSelf);
            SetLocalRect(
                footerHint.rectTransform,
                new Rect(geometry.Footer.x, geometry.Footer.y, geometry.Footer.width - 126f, geometry.Footer.height));
            LayoutRows();
        }

        private void LayoutDetailNarrative(bool hasPrompt)
        {
            if (detailPanel == null
                || detailMeta == null
                || detailNarrativeFrame == null
                || detailNarrativeViewport == null
                || detailNarrativeContent == null
                || detailNotesLabel == null
                || detailNotes == null
                || detailPrompt == null)
            {
                return;
            }

            float detailW = Mathf.Max(1f, detailPanel.rect.width);
            float detailH = Mathf.Max(1f, detailPanel.rect.height);
            float actionY = detailH - 60f;
            float detailArtSize = Mathf.Clamp(
                78f * CombatAbilityModalListRules.UiScale(Screen.height),
                86f,
                104f);
            float summaryY = Mathf.Max(112f, detailArtSize + 28f);
            float statY = summaryY + 74f;
            float metaY = statY + 52f;
            float metaH = 30f;
            float narrativeY = metaY + metaH + 6f;
            float narrativeBottom = actionY - 12f;
            float narrativeH = Mathf.Max(0f, narrativeBottom - narrativeY);
            float frameW = Mathf.Max(1f, detailW - 24f);
            float viewportW = Mathf.Max(1f, frameW - 32f);
            float viewportH = Mathf.Max(1f, narrativeH - 12f);
            bool hasNotes = detailNotes.gameObject.activeSelf
                && !string.IsNullOrWhiteSpace(detailNotes.text);
            bool showPrompt = hasPrompt
                && detailPrompt.gameObject.activeSelf
                && !string.IsNullOrWhiteSpace(detailPrompt.text);
            bool hasNarrative = hasNotes || showPrompt;

            SetLocalRect(detailMeta.rectTransform, new Rect(16f, metaY, detailW - 32f, metaH));
            SetLocalRect(detailNarrativeFrame, new Rect(12f, narrativeY, frameW, narrativeH));
            detailNarrativeFrame.gameObject.SetActive(hasNarrative);
            if (!hasNarrative)
            {
                narrativeContentKey = "";
                if (detailNarrativeScroll != null) detailNarrativeScroll.vertical = false;
                RefreshFooterHint();
                return;
            }

            SetLocalRect(detailNarrativeViewport, new Rect(6f, 6f, viewportW, viewportH));
            if (detailNarrativeScrollbar != null)
            {
                SetLocalRect(
                    detailNarrativeScrollbar.GetComponent<RectTransform>(),
                    new Rect(frameW - 21f, 6f, 15f, viewportH));
            }

            float cursorY = 0f;
            detailNotesLabel.gameObject.SetActive(hasNotes);
            if (hasNotes)
            {
                SetLocalRect(detailNotesLabel.rectTransform, new Rect(0f, cursorY, viewportW, 18f));
                cursorY += 22f;
                SetLocalRect(detailNotes.rectTransform, new Rect(0f, cursorY, viewportW, 1000f));
                float notesHeight = Mathf.Max(18f, Mathf.Ceil(detailNotes.preferredHeight));
                SetLocalRect(detailNotes.rectTransform, new Rect(0f, cursorY, viewportW, notesHeight));
                cursorY += notesHeight;
            }

            if (showPrompt)
            {
                if (cursorY > 0f) cursorY += 9f;
                SetLocalRect(detailPrompt.rectTransform, new Rect(0f, cursorY, viewportW, 1000f));
                float promptHeight = Mathf.Max(24f, Mathf.Ceil(detailPrompt.preferredHeight));
                SetLocalRect(detailPrompt.rectTransform, new Rect(0f, cursorY, viewportW, promptHeight));
                cursorY += promptHeight;
            }

            float contentHeight = Mathf.Max(viewportH, cursorY + 2f);
            SetLocalRect(
                detailNarrativeContent,
                new Rect(0f, 0f, viewportW, contentHeight));
            if (detailNarrativeScroll != null)
            {
                detailNarrativeScroll.vertical = contentHeight > viewportH + 1f;
            }
            string contentKey =
                DetailId()
                + "\n"
                + (detailNotes.text ?? "")
                + "\n"
                + (detailPrompt.text ?? "");
            if (!string.Equals(narrativeContentKey, contentKey, StringComparison.Ordinal))
            {
                narrativeContentKey = contentKey;
                detailNarrativeContent.anchoredPosition = Vector2.zero;
                if (detailNarrativeScroll != null) detailNarrativeScroll.verticalNormalizedPosition = 1f;
            }
            RefreshFooterHint();
        }

        private bool DetailNarrativeCanScroll()
        {
            return detailNarrativeFrame != null
                && detailNarrativeFrame.gameObject.activeSelf
                && detailNarrativeContent != null
                && detailNarrativeViewport != null
                && detailNarrativeContent.rect.height > detailNarrativeViewport.rect.height + 1f;
        }

        private bool ScrollDetailPage(int direction)
        {
            if (!DetailNarrativeCanScroll() || detailNarrativeScroll == null || direction == 0) return false;
            SetDetailNarrativePosition(
                CombatAbilityModalDetailScrollRules.ApplyPage(
                    detailNarrativeScroll.verticalNormalizedPosition,
                    direction));
            return true;
        }

        private bool SetDetailNarrativePosition(float normalizedPosition)
        {
            if (detailNarrativeScroll == null) return false;
            float next = Mathf.Clamp01(normalizedPosition);
            if (Mathf.Approximately(detailNarrativeScroll.verticalNormalizedPosition, next)) return false;
            detailNarrativeScroll.StopMovement();
            detailNarrativeScroll.verticalNormalizedPosition = next;
            return true;
        }

        private void RefreshFooterHint()
        {
            if (footerHint == null) return;
            footerHint.text = DetailNarrativeCanScroll()
                ? OverflowFooterHint
                : BaseFooterHint;
        }

        private void ApplyTypography(float scale)
        {
            SetFontSize(titleText, 24, scale);
            SetFontSize(actorText, 13, scale);
            SetFontSize(resourceText, 12, scale);
            SetFontSize(actionStateText, 12, scale);
            SetFontSize(traitText, 12, scale);
            SetFontSize(closeButtonLabel, 13, scale);
            SetFontSize(emptyText, 13, scale);
            SetFontSize(listScrollHint, 11, scale);
            SetFontSize(detailTitle, 20, scale);
            SetFontSize(detailStatus, 12, scale);
            SetFontSize(detailContext, 10, scale);
            SetFontSize(detailSummary, 14, scale);
            SetFontSize(detailMeta, 10, scale);
            SetFontSize(detailNotesLabel, 10, scale);
            SetFontSize(detailNotes, 11, scale);
            SetFontSize(detailPrompt, 12, scale);
            SetFontSize(detailActionLabel, 13, scale);
            SetFontSize(footerHint, 11, scale);
            if (filterLabels != null)
            {
                for (int i = 0; i < filterLabels.Length; i++) SetFontSize(filterLabels[i], 12, scale);
            }
            if (statChips != null)
            {
                for (int i = 0; i < statChips.Length; i++)
                {
                    SetFontSize(statChips[i].Label, 10, scale);
                    SetFontSize(statChips[i].Value, 12, scale);
                }
            }
            for (int i = 0; i < cardRows.Count; i++) ApplyRowTypography(cardRows[i], scale);
        }

        private static void SetFontSize(Text text, int baseSize, float scale)
        {
            if (text != null) text.fontSize = Mathf.Max(baseSize, Mathf.RoundToInt(baseSize * scale));
        }

        private static void ApplyRowTypography(CardRow row, float scale)
        {
            if (row == null) return;
            SetFontSize(row.Name, 14, scale);
            SetFontSize(row.Meta, 11, scale);
            SetFontSize(row.Summary, 12, scale);
            SetFontSize(row.State, 11, scale);
            SetFontSize(row.SelectionChevron, 18, scale);
            SetFontSize(row.Sigil, 14, scale);
        }

        private void EnsureRowCount(int count)
        {
            while (cardRows.Count < count)
            {
                cardRows.Add(CreateRow(cardRows.Count));
            }
            float scale = CombatAbilityModalListRules.UiScale(Screen.height);
            for (int i = 0; i < cardRows.Count; i++) ApplyRowTypography(cardRows[i], scale);
            LayoutRows();
        }

        private CardRow CreateRow(int index)
        {
            RectTransform root = AddPanel("Card " + index, listContent, Hex("12181d", 0.98f), Hex("3c4544", 0.82f));
            Button button = root.gameObject.AddComponent<Button>();
            DisableAutomaticNavigation(button);
            Image background = root.GetComponent<Image>();
            button.targetGraphic = background;
            button.onClick.AddListener(() => SelectVisibleIndex(index, true, SelectionOrigin.PointerClick));
            CombatAbilityModalFocusRelay focusRelay = root.gameObject.AddComponent<CombatAbilityModalFocusRelay>();
            focusRelay.PointerEnter = () => QueuePointerPreview(index);
            focusRelay.PointerExit = () => ClearHoverForVisibleIndex(index);
            focusRelay.Selected = () => SelectVisibleIndexFromEventSystem(index);

            Image selectionRail = AddImage("Selection Rail", root, Hex("66cdb9", 1f));
            selectionRail.raycastTarget = false;
            Text selectionChevron = AddText("Selection Chevron", root, "", 18, Hex("f3ead7", 1f), TextAnchor.MiddleCenter);
            selectionChevron.fontStyle = FontStyle.Bold;
            selectionChevron.gameObject.SetActive(false);
            Image selectionIcon = AddImage("Selection Icon", root, Color.white);
            selectionIcon.preserveAspect = true;
            selectionIcon.raycastTarget = false;
            Text name = AddText("Name", root, "", 14, Hex("f3ead7", 1f), TextAnchor.MiddleLeft);
            Text meta = AddText("Meta", root, "", 11, Hex("d7a84e", 1f), TextAnchor.MiddleLeft);
            Text summary = AddText("Summary", root, "", 12, Hex("d8d0c1", 1f), TextAnchor.MiddleLeft);
            RectTransform statusBadge = AddPanel("Status Badge", root, Hex("13201d", 0.96f), Hex("8ed7c7", 0.72f));
            Image statusIcon = AddImage("Status Icon", statusBadge, Color.white);
            statusIcon.preserveAspect = true;
            statusIcon.raycastTarget = false;
            Text state = AddText("State", statusBadge, "", 11, Hex("8ed7c7", 1f), TextAnchor.MiddleCenter);
            state.fontStyle = FontStyle.Bold;
            RectTransform iconFrame = AddPanel("Icon Frame", root, Hex("050708", 0.98f), Hex("3c4544", 0.82f));
            Image icon = AddImage("Icon", iconFrame, Color.white);
            icon.preserveAspect = true;
            icon.raycastTarget = false;
            Text sigil = AddText("Sigil", iconFrame, "", 14, Hex("f3ead7", 1f), TextAnchor.MiddleCenter);
            sigil.fontStyle = FontStyle.Bold;
            sigil.raycastTarget = false;
            return new CardRow
            {
                Root = root,
                Button = button,
                Background = background,
                Outline = root.GetComponent<Outline>(),
                SelectionRail = selectionRail,
                SelectionChevron = selectionChevron,
                SelectionIcon = selectionIcon,
                IconFrame = iconFrame,
                Icon = icon,
                Sigil = sigil,
                Name = name,
                Meta = meta,
                Summary = summary,
                StatusBadge = statusBadge,
                StatusIcon = statusIcon,
                State = state
            };
        }

        private void LayoutRows()
        {
            if (listViewport == null || listContent == null) return;
            float width = Mathf.Max(1f, listViewport.rect.width);
            float previousScrollY = Mathf.Max(0f, listContent.anchoredPosition.y);
            float totalH = CombatAbilityModalListRules.ContentHeight(
                activeCardCount,
                rowHeight,
                rowGap,
                listViewport.rect.height);
            listContent.anchorMin = new Vector2(0f, 1f);
            listContent.anchorMax = new Vector2(1f, 1f);
            listContent.pivot = new Vector2(0f, 1f);
            listContent.sizeDelta = new Vector2(0f, totalH);
            float maxScrollY = Mathf.Max(0f, totalH - listViewport.rect.height);
            listContent.anchoredPosition = new Vector2(0f, Mathf.Min(previousScrollY, maxScrollY));
            for (int i = 0; i < cardRows.Count; i++)
            {
                CardRow row = cardRows[i];
                Rect card = new Rect(
                    CombatAbilityModalListRules.RowInset,
                    CombatAbilityModalListRules.RowInset + i * (rowHeight + rowGap),
                    width - CombatAbilityModalListRules.ContentPadding,
                    rowHeight);
                SetLocalRect(row.Root, card);
                SetLocalRect(row.SelectionRail.rectTransform, new Rect(0f, 0f, 5f, rowHeight));
                SetLocalRect(row.SelectionChevron.rectTransform, new Rect(5f, 0f, 14f, rowHeight));
                SetLocalRect(row.SelectionIcon.rectTransform, new Rect(5f, (rowHeight - 16f) * 0.5f, 16f, 16f));
                float iconSize = Mathf.Min(68f, rowHeight - 20f);
                SetLocalRect(row.IconFrame, new Rect(20f, (rowHeight - iconSize) * 0.5f, iconSize, iconSize));
                Stretch(row.Icon.rectTransform, 4f, 4f);
                Stretch(row.Sigil.rectTransform, 4f, 4f);
                float textX = 20f + iconSize + 12f;
                float statusWidth = 112f;
                float contentWidth = Mathf.Max(120f, card.width - textX - 14f);
                float nameWidth = row.StatusBadge.gameObject.activeSelf
                    ? contentWidth - statusWidth - 10f
                    : contentWidth - 4f;
                SetLocalRect(row.Name.rectTransform, new Rect(textX, 4f, nameWidth, 21f));
                SetLocalRect(row.StatusBadge, new Rect(card.width - statusWidth - 10f, 4f, statusWidth, 22f));
                SetLocalRect(row.StatusIcon.rectTransform, new Rect(5f, 3f, 16f, 16f));
                SetLocalRect(row.State.rectTransform, new Rect(24f, 1f, statusWidth - 28f, 20f));
                SetLocalRect(row.Summary.rectTransform, new Rect(textX, 26f, contentWidth - 4f, 23f));
                SetLocalRect(row.Meta.rectTransform, new Rect(textX, rowHeight - 22f, contentWidth - 4f, 17f));
            }
        }

        private void LateUpdate()
        {
            if (!IsVisible) return;
            RefreshScrollHint();
        }

        private void RefreshScrollHint()
        {
            if (listScrollHint == null || listViewport == null || listContent == null) return;
            if (visibleCards.Count == 0)
            {
                listScrollHint.text = "0 OF 0";
                return;
            }
            listScrollHint.text = CombatAbilityModalListRules.ScrollLabel(
                visibleCards.Count,
                rowHeight,
                rowGap,
                listViewport.rect.height,
                listContent.rect.height,
                listContent.anchoredPosition.y);
        }

        private void SetFilter(CombatAbilityModalFilter filter)
        {
            if (currentView?.Cards == null) return;
            int count = CombatAbilityModalPresentationRules.Count(currentView.Cards, filter);
            if (count == 0 && filter != CombatAbilityModalFilter.All) return;
            if (filterInitialized && currentFilter == filter)
            {
                FocusSelectedControl();
                return;
            }

            RememberCurrentFilterBrowseState();
            currentFilter = filter;
            filterInitialized = true;
            hoveredId = "";
            CancelPendingPointerPreview();
            if (filterBrowseStates.TryGetValue(
                FilterBrowseKey(listContextKey, filter),
                out FilterBrowseState restored))
            {
                selectedId = restored.SelectedId ?? "";
                pendingFilterSelectionId = selectedId;
                pendingFilterScrollY = restored.ScrollY;
                pendingFilterRestore = true;
            }
            else
            {
                selectedId = "";
                pendingFilterSelectionId = "";
                pendingFilterScrollY = 0f;
                pendingFilterRestore = false;
                if (listContent != null) listContent.anchoredPosition = Vector2.zero;
            }
            Refresh();
            if (!string.IsNullOrWhiteSpace(selectedId))
            {
                bindings?.PreviewCard?.Invoke(selectedId);
                EnsureSelectedVisible();
            }
            FocusSelectedControl();
        }

        private void RememberCurrentFilterBrowseState()
        {
            if (!filterInitialized || string.IsNullOrWhiteSpace(listContextKey)) return;
            string key = FilterBrowseKey(listContextKey, currentFilter);
            if (!filterBrowseStates.TryGetValue(key, out FilterBrowseState state))
            {
                state = new FilterBrowseState();
                filterBrowseStates[key] = state;
            }
            state.SelectedId = selectedId ?? "";
            state.ScrollY = listContent == null
                ? 0f
                : Mathf.Max(0f, listContent.anchoredPosition.y);
        }

        private static string FilterBrowseKey(string contextKey, CombatAbilityModalFilter filter)
        {
            return (contextKey ?? "") + "|" + (int)filter;
        }

        private void CycleFilter(int direction)
        {
            if (currentView?.Cards == null || direction == 0) return;
            int current = Array.IndexOf(Filters, currentFilter);
            for (int step = 1; step <= Filters.Length; step++)
            {
                int index = (current + direction * step) % Filters.Length;
                if (index < 0) index += Filters.Length;
                CombatAbilityModalFilter candidate = Filters[index];
                if (candidate == CombatAbilityModalFilter.All
                    || CombatAbilityModalPresentationRules.Count(currentView.Cards, candidate) > 0)
                {
                    SetFilter(candidate);
                    return;
                }
            }
        }

        private void MoveSelection(int delta)
        {
            if (visibleCards.Count == 0 || delta == 0) return;
            int current = IndexOfVisible(selectedId);
            if (current < 0) current = 0;
            int next = Mathf.Clamp(current + delta, 0, visibleCards.Count - 1);
            if (next == current)
            {
                ClearHover();
                FocusSelectedControl();
                return;
            }
            SelectVisibleIndex(next, true);
        }

        private void SelectVisibleIndex(int index, bool announce)
        {
            SelectVisibleIndex(index, announce, SelectionOrigin.Navigation);
        }

        private void SelectVisibleIndex(int index, bool announce, SelectionOrigin origin)
        {
            if (index < 0 || index >= visibleCards.Count) return;
            string nextId = visibleCards[index].Id ?? "";
            if (string.IsNullOrWhiteSpace(nextId)) return;
            bool changed = !string.Equals(selectedId, nextId, StringComparison.Ordinal);
            selectedId = nextId;
            ClearPointerPreview(false);
            if (changed)
            {
                if (announce) bindings?.SelectCard?.Invoke(nextId);
                else bindings?.PreviewCard?.Invoke(nextId);
            }
            RefreshSelectionPresentation();
            EnsureSelectedVisible();
            if (origin != SelectionOrigin.EventSystemFocus) FocusSelectedControl();
        }

        private void SelectVisibleIndexFromEventSystem(int index)
        {
            if (handlingEventSystemFocus) return;
            handlingEventSystemFocus = true;
            try
            {
                SelectVisibleIndex(index, true, SelectionOrigin.EventSystemFocus);
            }
            finally
            {
                handlingEventSystemFocus = false;
            }
        }

        private void QueuePointerPreview(int index)
        {
            if (index < 0 || index >= visibleCards.Count) return;
            string nextId = visibleCards[index].Id ?? "";
            if (string.IsNullOrWhiteSpace(nextId)) return;
            if (string.Equals(nextId, selectedId, StringComparison.Ordinal))
            {
                ClearHover();
                return;
            }
            if (string.Equals(hoveredId, nextId, StringComparison.Ordinal)
                || string.Equals(pendingHoveredId, nextId, StringComparison.Ordinal)) return;
            bool hadPreview = !string.IsNullOrWhiteSpace(hoveredId);
            hoveredId = "";
            pendingHoveredId = nextId;
            pendingHoverEnteredAt = Time.unscaledTime;
            if (hadPreview) RefreshSelectionPresentation();
        }

        private void CommitPointerPreview(int index)
        {
            if (index < 0 || index >= visibleCards.Count) return;
            string nextId = visibleCards[index].Id ?? "";
            if (string.IsNullOrWhiteSpace(nextId)
                || string.Equals(nextId, selectedId, StringComparison.Ordinal)) return;
            CancelPendingPointerPreview();
            if (string.Equals(hoveredId, nextId, StringComparison.Ordinal)) return;
            hoveredId = nextId;
            RefreshSelectionPresentation();
        }

        private bool CommitPendingPointerPreview(float now)
        {
            int index = IndexOfVisible(pendingHoveredId);
            if (!CombatAbilityModalPointerRules.ShouldCommitPreview(
                pendingHoveredId,
                selectedId,
                index >= 0,
                pendingHoverEnteredAt,
                now)) return false;
            CommitPointerPreview(index);
            return true;
        }

        private void CancelPendingPointerPreview()
        {
            pendingHoveredId = "";
            pendingHoverEnteredAt = 0f;
        }

        private void ClearHoverForVisibleIndex(int index)
        {
            if (index < 0 || index >= visibleCards.Count) return;
            string id = visibleCards[index]?.Id ?? "";
            if (string.Equals(pendingHoveredId, id, StringComparison.Ordinal))
            {
                CancelPendingPointerPreview();
            }
            if (!string.Equals(hoveredId, id, StringComparison.Ordinal)) return;
            ClearHover();
        }

        private void ClearHover()
        {
            ClearPointerPreview(true);
        }

        private bool ClearPointerPreview(bool refresh)
        {
            bool changed = !string.IsNullOrWhiteSpace(hoveredId)
                || !string.IsNullOrWhiteSpace(pendingHoveredId);
            CancelPendingPointerPreview();
            if (string.IsNullOrWhiteSpace(hoveredId)) return changed;
            hoveredId = "";
            if (refresh) RefreshSelectionPresentation();
            return true;
        }

        private void ActivateDetail()
        {
            if (IsPointerPreview(FindVisibleCard(hoveredId))) return;
            ActivateSelected();
        }

        private void ActivateFromNavigation()
        {
            if (ClearPointerPreview(false)) RefreshSelectionPresentation();
            ActivateSelected();
        }

        private void ActivateSelected()
        {
            CombatAbilityModalCardView card = FindVisibleCard(selectedId);
            if (!CombatAbilityModalPresentationRules.CanActivate(card)) return;
            bindings?.ActivateCard?.Invoke(card.Id);
        }

        private void EnsureSelectedVisible()
        {
            int index = IndexOfVisible(selectedId);
            if (index < 0 || listViewport == null || listContent == null) return;
            float top = index * (rowHeight + rowGap) + 4f;
            float bottom = top + rowHeight;
            float viewportTop = Mathf.Max(0f, listContent.anchoredPosition.y);
            float viewportBottom = viewportTop + listViewport.rect.height;
            float next = viewportTop;
            if (top < viewportTop) next = top;
            else if (bottom > viewportBottom) next = bottom - listViewport.rect.height;
            float max = Mathf.Max(0f, listContent.rect.height - listViewport.rect.height);
            listContent.anchoredPosition = new Vector2(0f, Mathf.Clamp(next, 0f, max));
        }

        private void FocusSelectedControl()
        {
            if (handlingEventSystemFocus || !IsVisible || EventSystem.current == null) return;
            GameObject focus = null;
            int index = IndexOfVisible(selectedId);
            if (index >= 0 && index < cardRows.Count)
            {
                focus = cardRows[index].Button.gameObject;
            }
            if (focus == null && detailActionButton != null && detailActionButton.interactable)
            {
                focus = detailActionButton.gameObject;
            }
            if (focus == null && closeButton != null) focus = closeButton.gameObject;
            if (focus == null || EventSystem.current.currentSelectedGameObject == focus) return;
            EventSystem.current.SetSelectedGameObject(focus);
        }

        private int PageSize()
        {
            if (listViewport == null) return 4;
            return Mathf.Max(1, Mathf.FloorToInt(listViewport.rect.height / Mathf.Max(1f, rowHeight + rowGap)) - 1);
        }

        private int IndexOfVisible(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return -1;
            for (int i = 0; i < visibleCards.Count; i++)
            {
                if (string.Equals(visibleCards[i]?.Id, id, StringComparison.Ordinal)) return i;
            }
            return -1;
        }

        private CombatAbilityModalCardView FindVisibleCard(string id)
        {
            int index = IndexOfVisible(id);
            return index >= 0 ? visibleCards[index] : null;
        }

        private string DetailId()
        {
            return IndexOfVisible(hoveredId) >= 0 ? hoveredId : selectedId;
        }

        private bool IsPointerPreview(CombatAbilityModalCardView card)
        {
            return card != null
                && !string.IsNullOrWhiteSpace(hoveredId)
                && string.Equals(card.Id, hoveredId, StringComparison.Ordinal)
                && !string.Equals(card.Id, selectedId, StringComparison.Ordinal);
        }

        private void RefreshSelectionPresentation()
        {
            if (currentView == null) return;
            Color accent = currentView.Spellbook ? Hex("a77ae8", 1f) : Hex("66cdb9", 1f);
            int count = Mathf.Min(cardRows.Count, visibleCards.Count);
            for (int i = 0; i < count; i++)
            {
                CardRow row = cardRows[i];
                if (row == null || !row.Root.gameObject.activeSelf) continue;
                RefreshRow(row, visibleCards[i], accent);
            }
            RefreshDetail(FindVisibleCard(DetailId()), accent);
        }

        private int CountVisibleRows(Func<CardRow, bool> predicate)
        {
            if (predicate == null) return 0;
            int count = 0;
            for (int i = 0; i < cardRows.Count; i++)
            {
                CardRow row = cardRows[i];
                if (row == null || row.Root == null || !row.Root.gameObject.activeSelf) continue;
                if (predicate(row)) count++;
            }
            return count;
        }

        private static string CompactReach(CombatAbilityModalCardView card)
        {
            if (card == null) return "";
            string reach = card.Range?.Trim() ?? "";
            string path = card.Path?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(path)
                || string.Equals(path, "instant", StringComparison.OrdinalIgnoreCase)
                || reach.IndexOf(path, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return reach;
            }
            if (string.IsNullOrWhiteSpace(reach)) return path;
            return reach + "  •  " + path;
        }

        private static string FirstNonEmpty(params string[] values)
        {
            if (values == null) return "";
            for (int i = 0; i < values.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(values[i])) return values[i].Trim();
            }
            return "";
        }

        private static string RelevantContext(CombatAbilityModalCardView card)
        {
            if (card == null || card.Locked || card.Ready) return "";
            string context = CombatAbilityModalPresentationRules.DetailPrompt(card);
            if (string.IsNullOrWhiteSpace(context)) return "";
            string action = CombatAbilityModalPresentationRules.CardActionLabel(card);
            string normalizedContext = context.Trim().TrimEnd('.');
            string normalizedAction = (action ?? "").Trim().TrimEnd('.');
            return string.Equals(normalizedContext, normalizedAction, StringComparison.OrdinalIgnoreCase)
                ? ""
                : context.Trim();
        }

        private static string ImmediateTraitText(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "";
            string text = value.Trim();
            if (text.StartsWith("FOCUS  •", StringComparison.Ordinal)
                || text.StartsWith("ENRAGE ACTIVE", StringComparison.OrdinalIgnoreCase)
                || text.StartsWith("STEALTH ", StringComparison.Ordinal))
            {
                return text;
            }
            return "";
        }

        private void RefreshIcon(Image image, Text sigil, CombatAbilityModalCardView card, Color accent)
        {
            if (image == null || sigil == null) return;
            Sprite sprite = card == null ? null : UiRuntime.AtlasSprite(card.IconTexture, card.IconSource);
            image.sprite = sprite;
            image.color = card != null && card.Locked ? Hex("c0c6c8", 0.68f) : Color.white;
            image.enabled = sprite != null;
            sigil.text = card?.Sigil ?? "";
            sigil.color = card != null && card.Locked ? Hex("9aa0a1", 1f) : accent;
            sigil.gameObject.SetActive(sprite == null && !string.IsNullOrWhiteSpace(sigil.text));
            sigil.alignment = TextAnchor.MiddleCenter;
        }

        private static int BookStateIndex(CombatAbilityModalCardView card)
        {
            return CombatAbilityModalPresentationRules.BookStateIconIndex(card);
        }

        private void SetBookStateIcon(Image image, int index, Color tint, bool visible)
        {
            if (image == null) return;
            if (!visible)
            {
                image.enabled = false;
                image.gameObject.SetActive(false);
                return;
            }

            Texture2D texture = BookStateIconTexture();
            Sprite sprite = UiRuntime.AtlasSprite(texture, BookStateIconAtlasCell(index));
            image.sprite = sprite;
            image.color = texture == generatedBookStateIconAtlas
                ? tint
                : Color.white.WithAlpha(tint.a);
            image.enabled = sprite != null;
            image.gameObject.SetActive(sprite != null);
        }

        private Texture2D BookStateIconTexture()
        {
            Texture2D supplied = currentView?.StateIconTexture;
            if (IsBookStateIconAtlas(supplied)) return supplied;
            if (generatedBookStateIconAtlas == null)
            {
                generatedBookStateIconAtlas = BuildGeneratedBookStateIconAtlas();
            }
            return generatedBookStateIconAtlas;
        }

        private static bool IsBookStateIconAtlas(Texture2D texture)
        {
            return texture != null
                && CombatIconCatalog.IsBookStateAtlasDimensions(texture.width, texture.height);
        }

        private static Rect BookStateIconAtlasCell(int index)
        {
            int max = CombatIconCatalog.BookStateAtlasColumns * CombatIconCatalog.BookStateAtlasRows - 1;
            int bounded = Mathf.Clamp(index, 0, max);
            int column = bounded % CombatIconCatalog.BookStateAtlasColumns;
            int row = bounded / CombatIconCatalog.BookStateAtlasColumns;
            return new Rect(
                column * CombatIconCatalog.BookStateAtlasCellSize,
                row * CombatIconCatalog.BookStateAtlasCellSize,
                CombatIconCatalog.BookStateAtlasCellSize,
                CombatIconCatalog.BookStateAtlasCellSize);
        }

        private static Texture2D BuildGeneratedBookStateIconAtlas()
        {
            int width = CombatIconCatalog.BookStateAtlasWidth;
            int height = CombatIconCatalog.BookStateAtlasHeight;
            Color32[] pixels = new Color32[width * height];
            Color32 strong = new Color32(255, 255, 255, 255);
            Color32 soft = new Color32(255, 255, 255, 150);

            DrawStateLine(pixels, 0, 22, 15, 43, 32, 5, strong);
            DrawStateLine(pixels, 0, 43, 32, 22, 49, 5, strong);

            DrawStateCircle(pixels, 1, 32, 32, 16, 3, strong);
            DrawStateLine(pixels, 1, 32, 8, 32, 22, 3, strong);
            DrawStateLine(pixels, 1, 32, 42, 32, 56, 3, strong);
            DrawStateLine(pixels, 1, 8, 32, 22, 32, 3, strong);
            DrawStateLine(pixels, 1, 42, 32, 56, 32, 3, strong);
            DrawStateDisc(pixels, 1, 32, 32, 3, strong);

            DrawStateCircle(pixels, 2, 32, 27, 12, 4, soft);
            DrawStateFillRect(pixels, 2, 18, 28, 46, 51, strong);
            DrawStateFillRect(pixels, 2, 29, 37, 35, 47, new Color32(0, 0, 0, 255));

            DrawStateLine(pixels, 3, 32, 11, 20, 31, 4, strong);
            DrawStateLine(pixels, 3, 20, 31, 24, 45, 4, strong);
            DrawStateLine(pixels, 3, 24, 45, 32, 51, 4, strong);
            DrawStateLine(pixels, 3, 32, 51, 40, 45, 4, strong);
            DrawStateLine(pixels, 3, 40, 45, 44, 31, 4, strong);
            DrawStateLine(pixels, 3, 44, 31, 32, 11, 4, strong);
            DrawStateLine(pixels, 3, 25, 39, 39, 39, 4, strong);

            DrawStateCircle(pixels, 4, 32, 32, 16, 3, soft);
            DrawStateCircle(pixels, 4, 32, 32, 7, 3, soft);
            DrawStateLine(pixels, 4, 15, 49, 49, 15, 5, strong);

            DrawStateCircle(pixels, 5, 32, 32, 18, 4, strong);
            DrawStateLine(pixels, 5, 32, 19, 32, 33, 4, strong);
            DrawStateLine(pixels, 5, 32, 33, 42, 39, 4, strong);
            DrawStateLine(pixels, 5, 14, 50, 50, 14, 3, soft);

            DrawStateCircle(pixels, 6, 32, 32, 18, 4, soft);
            DrawStateLine(pixels, 6, 20, 20, 44, 44, 5, strong);
            DrawStateLine(pixels, 6, 44, 20, 20, 44, 5, strong);

            DrawStateLine(pixels, 7, 23, 12, 41, 12, 4, strong);
            DrawStateLine(pixels, 7, 41, 12, 52, 23, 4, strong);
            DrawStateLine(pixels, 7, 52, 23, 52, 41, 4, strong);
            DrawStateLine(pixels, 7, 52, 41, 41, 52, 4, strong);
            DrawStateLine(pixels, 7, 41, 52, 23, 52, 4, strong);
            DrawStateLine(pixels, 7, 23, 52, 12, 41, 4, strong);
            DrawStateLine(pixels, 7, 12, 41, 12, 23, 4, strong);
            DrawStateLine(pixels, 7, 12, 23, 23, 12, 4, strong);
            DrawStateLine(pixels, 7, 21, 32, 43, 32, 6, strong);

            DrawStateLine(pixels, 8, 32, 10, 18, 30, 4, strong);
            DrawStateLine(pixels, 8, 18, 30, 32, 52, 4, strong);
            DrawStateLine(pixels, 8, 32, 52, 46, 30, 4, strong);
            DrawStateLine(pixels, 8, 46, 30, 32, 10, 4, strong);
            DrawStateDisc(pixels, 8, 32, 32, 5, soft);

            DrawStateLine(pixels, 9, 12, 32, 51, 32, 5, strong);
            DrawStateLine(pixels, 9, 51, 32, 39, 20, 5, strong);
            DrawStateLine(pixels, 9, 51, 32, 39, 44, 5, strong);
            DrawStateLine(pixels, 9, 17, 23, 17, 41, 3, soft);

            DrawStateCircle(pixels, 10, 32, 32, 19, 3, strong);
            DrawStateCircle(pixels, 10, 32, 32, 11, 3, soft);
            DrawStateDisc(pixels, 10, 32, 32, 4, strong);

            DrawStateLine(pixels, 11, 10, 32, 22, 21, 4, strong);
            DrawStateLine(pixels, 11, 22, 21, 32, 18, 4, strong);
            DrawStateLine(pixels, 11, 32, 18, 42, 21, 4, strong);
            DrawStateLine(pixels, 11, 42, 21, 54, 32, 4, strong);
            DrawStateLine(pixels, 11, 54, 32, 42, 43, 4, strong);
            DrawStateLine(pixels, 11, 42, 43, 32, 46, 4, strong);
            DrawStateLine(pixels, 11, 32, 46, 22, 43, 4, strong);
            DrawStateLine(pixels, 11, 22, 43, 10, 32, 4, strong);
            DrawStateCircle(pixels, 11, 32, 32, 8, 3, soft);
            DrawStateDisc(pixels, 11, 32, 32, 3, strong);

            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false, true)
            {
                name = "book-state-icon-atlas-generated",
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.DontSave
            };
            texture.SetPixels32(pixels);
            texture.Apply(false, false);
            return texture;
        }

        private static void DrawStateLine(
            Color32[] pixels,
            int iconIndex,
            int x0,
            int y0,
            int x1,
            int y1,
            int thickness,
            Color32 color)
        {
            int dx = Mathf.Abs(x1 - x0);
            int sx = x0 < x1 ? 1 : -1;
            int dy = -Mathf.Abs(y1 - y0);
            int sy = y0 < y1 ? 1 : -1;
            int error = dx + dy;
            int radius = Mathf.Max(1, thickness / 2);
            while (true)
            {
                DrawStateDisc(pixels, iconIndex, x0, y0, radius, color);
                if (x0 == x1 && y0 == y1) break;
                int doubled = error * 2;
                if (doubled >= dy)
                {
                    error += dy;
                    x0 += sx;
                }
                if (doubled <= dx)
                {
                    error += dx;
                    y0 += sy;
                }
            }
        }

        private static void DrawStateCircle(
            Color32[] pixels,
            int iconIndex,
            int centerX,
            int centerY,
            int radius,
            int thickness,
            Color32 color)
        {
            int outerSquared = radius * radius;
            int innerRadius = Mathf.Max(0, radius - thickness);
            int innerSquared = innerRadius * innerRadius;
            for (int y = centerY - radius; y <= centerY + radius; y++)
            for (int x = centerX - radius; x <= centerX + radius; x++)
            {
                int dx = x - centerX;
                int dy = y - centerY;
                int distanceSquared = dx * dx + dy * dy;
                if (distanceSquared <= outerSquared && distanceSquared >= innerSquared)
                {
                    SetStatePixel(pixels, iconIndex, x, y, color);
                }
            }
        }

        private static void DrawStateDisc(
            Color32[] pixels,
            int iconIndex,
            int centerX,
            int centerY,
            int radius,
            Color32 color)
        {
            int radiusSquared = radius * radius;
            for (int y = centerY - radius; y <= centerY + radius; y++)
            for (int x = centerX - radius; x <= centerX + radius; x++)
            {
                int dx = x - centerX;
                int dy = y - centerY;
                if (dx * dx + dy * dy <= radiusSquared)
                {
                    SetStatePixel(pixels, iconIndex, x, y, color);
                }
            }
        }

        private static void DrawStateFillRect(
            Color32[] pixels,
            int iconIndex,
            int xMin,
            int yMin,
            int xMax,
            int yMax,
            Color32 color)
        {
            for (int y = yMin; y <= yMax; y++)
            for (int x = xMin; x <= xMax; x++)
            {
                SetStatePixel(pixels, iconIndex, x, y, color);
            }
        }

        private static void SetStatePixel(
            Color32[] pixels,
            int iconIndex,
            int localX,
            int localY,
            Color32 color)
        {
            int cellSize = CombatIconCatalog.BookStateAtlasCellSize;
            if (pixels == null
                || iconIndex < 0
                || iconIndex >= CombatIconCatalog.BookStateAtlasColumns * CombatIconCatalog.BookStateAtlasRows
                || localX < 0
                || localX >= cellSize
                || localY < 0
                || localY >= cellSize)
            {
                return;
            }

            int column = iconIndex % CombatIconCatalog.BookStateAtlasColumns;
            int row = iconIndex / CombatIconCatalog.BookStateAtlasColumns;
            int atlasX = column * cellSize + localX;
            int atlasYFromTop = row * cellSize + localY;
            int atlasY = CombatIconCatalog.BookStateAtlasHeight - 1 - atlasYFromTop;
            int pixelIndex = atlasY * CombatIconCatalog.BookStateAtlasWidth + atlasX;
            if (pixelIndex < 0 || pixelIndex >= pixels.Length) return;
            if (pixels[pixelIndex].a <= color.a) pixels[pixelIndex] = color;
        }

        private void SetStatChip(int index, string label, string value)
        {
            if (statChips == null || index < 0 || index >= statChips.Length) return;
            statChips[index].Label.text = label ?? "";
            Color tint = index == 0
                ? Hex("d7a84e", 1f)
                : index == 1
                    ? Hex("8ecbd7", 1f)
                    : Hex("66cdb9", 1f);
            SetBookStateIcon(
                statChips[index].Icon,
                statChips[index].IconIndex,
                tint,
                true);
            statChips[index].Value.text = string.IsNullOrWhiteSpace(value) ? "—" : value;
        }

        private StatChip CreateStatChip(string name, Transform parent, int iconIndex)
        {
            RectTransform root = AddPanel(name + " Stat", parent, Hex("10161a", 0.94f), Hex("3c4544", 0.72f));
            Image icon = AddImage("Icon", root, Color.white);
            icon.preserveAspect = true;
            icon.raycastTarget = false;
            Text label = AddText("Label", root, name.ToUpperInvariant(), 10, Hex("9ea59f", 1f), TextAnchor.MiddleLeft);
            Text value = AddText("Value", root, "", 12, Hex("f3ead7", 1f), TextAnchor.MiddleLeft);
            value.fontStyle = FontStyle.Bold;
            return new StatChip(root, icon, iconIndex, label, value);
        }

        private static void LayoutStatChip(StatChip chip, Rect rect)
        {
            if (chip == null) return;
            SetLocalRect(chip.Root, rect);
            SetLocalRect(chip.Icon.rectTransform, new Rect(8f, 12f, 20f, 20f));
            SetLocalRect(chip.Label.rectTransform, new Rect(34f, 3f, rect.width - 42f, 14f));
            SetLocalRect(chip.Value.rectTransform, new Rect(34f, 17f, rect.width - 42f, 20f));
        }

        private Scrollbar AddVerticalScrollbar(string name, Transform parent)
        {
            Image track = AddImage(name, parent, Hex("11171b", 0.96f));
            Scrollbar scrollbar = track.gameObject.AddComponent<Scrollbar>();
            RectTransform sliding = AddImage("Sliding Area", track.transform, Color.clear).rectTransform;
            Stretch(sliding, 1f, 1f);
            Image handle = AddImage("Handle", sliding, Hex("8d9695", 0.92f));
            Stretch(handle.rectTransform);
            scrollbar.handleRect = handle.rectTransform;
            scrollbar.targetGraphic = handle;
            scrollbar.direction = Scrollbar.Direction.BottomToTop;
            DisableAutomaticNavigation(scrollbar);
            ColorBlock colors = scrollbar.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = Color.white;
            colors.selectedColor = Color.white;
            colors.pressedColor = new Color(0.84f, 0.84f, 0.84f, 1f);
            colors.disabledColor = new Color(0.55f, 0.55f, 0.55f, 1f);
            scrollbar.colors = colors;
            return scrollbar;
        }

        private Button AddButton(string name, Transform parent, string label, Action action)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(Outline));
            go.transform.SetParent(parent, false);
            Image image = go.GetComponent<Image>();
            image.color = Hex("151a1f", 0.98f);
            Button button = go.GetComponent<Button>();
            button.targetGraphic = image;
            DisableAutomaticNavigation(button);
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = Color.white;
            colors.selectedColor = Color.white;
            colors.pressedColor = new Color(0.84f, 0.84f, 0.84f, 1f);
            colors.disabledColor = new Color(0.55f, 0.55f, 0.55f, 1f);
            button.colors = colors;
            Outline outline = go.GetComponent<Outline>();
            outline.effectColor = Hex("d7a84e", 0.82f);
            outline.effectDistance = new Vector2(1f, -1f);
            if (action != null) button.onClick.AddListener(() => action());

            Text text = AddText("Label", go.transform, label, 13, Hex("f3ead7", 1f), TextAnchor.MiddleCenter);
            text.fontStyle = FontStyle.Bold;
            Stretch(text.rectTransform, 6f, 3f);
            return button;
        }

        private static void DisableAutomaticNavigation(Selectable selectable)
        {
            if (selectable == null) return;
            Navigation navigation = selectable.navigation;
            navigation.mode = Navigation.Mode.None;
            selectable.navigation = navigation;
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
            Text text = go.GetComponent<Text>();
            text.font = font;
            text.text = value;
            text.fontSize = size;
            text.color = color;
            text.alignment = anchor;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.supportRichText = true;
            text.raycastTarget = false;
            if (size >= 18) text.fontStyle = FontStyle.Bold;
            return text;
        }

        private static Color CardAccent(CombatAbilityModalCardView card, Color fallback)
        {
            if (card != null && !string.IsNullOrWhiteSpace(card.AccentHex)
                && ColorUtility.TryParseHtmlString("#" + card.AccentHex.TrimStart('#'), out Color color))
            {
                return color;
            }
            return fallback;
        }

        private static Color AvailabilityTextColor(CombatAbilityModalCardView card)
        {
            if (card == null || card.Locked) return Hex("a8adae", 1f);
            if (!card.Usable || !CombatAbilityModalPresentationRules.HasCurrentTarget(card)) return Hex("e0b96a", 1f);
            if (card.Ready) return Hex("f0c56c", 1f);
            return Hex("8ed7c7", 1f);
        }

        private static Color AvailabilityFill(CombatAbilityModalCardView card)
        {
            if (card == null || card.Locked) return Hex("15191c", 0.98f);
            if (!card.Usable || !CombatAbilityModalPresentationRules.HasCurrentTarget(card)) return Hex("2b2012", 0.98f);
            if (card.Ready) return Hex("34240f", 0.98f);
            return Hex("10241f", 0.98f);
        }

        private static Color CardActionTextColor(Color background)
        {
            float luminance = 0.2126f * background.r + 0.7152f * background.g + 0.0722f * background.b;
            return luminance > 0.42f ? Hex("050708", 1f) : Hex("f5f1df", 1f);
        }

        private static bool ColorsMatch(Color left, Color right)
        {
            return Mathf.Abs(left.r - right.r) <= 0.001f
                && Mathf.Abs(left.g - right.g) <= 0.001f
                && Mathf.Abs(left.b - right.b) <= 0.001f
                && Mathf.Abs(left.a - right.a) <= 0.001f;
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

        private static void Stretch(RectTransform rect, float xPadding = 0f, float yPadding = 0f)
        {
            if (rect == null) return;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(xPadding, yPadding);
            rect.offsetMax = new Vector2(-xPadding, -yPadding);
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

        private sealed class CardRow
        {
            public string Id;
            public RectTransform Root;
            public Button Button;
            public Image Background;
            public Outline Outline;
            public Image SelectionRail;
            public Text SelectionChevron;
            public Image SelectionIcon;
            public RectTransform IconFrame;
            public Image Icon;
            public Text Sigil;
            public Text Name;
            public Text Meta;
            public Text Summary;
            public RectTransform StatusBadge;
            public Image StatusIcon;
            public Text State;
        }

        private sealed class StatChip
        {
            public readonly RectTransform Root;
            public readonly Image Icon;
            public readonly int IconIndex;
            public readonly Text Label;
            public readonly Text Value;

            public StatChip(RectTransform root, Image icon, int iconIndex, Text label, Text value)
            {
                Root = root;
                Icon = icon;
                IconIndex = iconIndex;
                Label = label;
                Value = value;
            }
        }
    }
}
