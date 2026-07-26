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
                && (card.Ready || HasCurrentTarget(card));
        }

        public static bool IsReadyNow(CombatAbilityModalCardView card)
        {
            return card != null
                && !card.Locked
                && card.Usable
                && (card.Ready || HasCurrentTarget(card));
        }

        public static string CardActionLabel(CombatAbilityModalCardView card)
        {
            if (card == null) return "Unavailable";
            if (card.Locked) return "Unlocks at Level " + card.UnlockLevel;
            if (card.Ready && card.Usable) return "Return to Target";
            if (!card.Usable) return ShortDisabledReason(card.DisabledReason);
            if (!HasCurrentTarget(card)) return "No Target in Range";
            return card.Targeted ? "Choose Target" : "Use Now";
        }

        public static string AvailabilityLabel(CombatAbilityModalCardView card)
        {
            if (card == null) return "UNAVAILABLE";
            if (card.Locked) return "FUTURE";
            if (card.Ready && card.Usable) return "TARGETING";
            if (!card.Usable)
            {
                string reason = (card.DisabledReason ?? "").ToLowerInvariant();
                if (reason.Contains("mana") || reason.Contains(" mp")) return "LOW MP";
                if (reason.Contains("action")) return "ACTION USED";
                if (reason.Contains("stun") || reason.Contains("sleep") || reason.Contains("disabled")) return "DISABLED";
                return "BLOCKED";
            }
            if (!HasCurrentTarget(card)) return "REPOSITION";
            return "READY NOW";
        }

        public static string BackButtonLabel(IReadOnlyList<CombatAbilityModalCardView> cards)
        {
            if (cards != null)
            {
                for (int i = 0; i < cards.Count; i++)
                {
                    if (cards[i]?.Ready == true) return "Return to Target  [Esc]";
                }
            }

            return "Back to Board  [Esc]";
        }

        public static string DetailPrompt(CombatAbilityModalCardView card)
        {
            if (card == null) return "";
            if (card.Locked) return $"Reach level {card.UnlockLevel} to learn this power.";
            if (!card.Usable)
            {
                return string.IsNullOrWhiteSpace(card.DisabledReason) ? "Unavailable." : card.DisabledReason;
            }
            if (card.Ready) return "Targeting is armed. Return to the board and select a highlighted target.";
            if (!HasCurrentTarget(card))
            {
                return string.IsNullOrWhiteSpace(card.TacticalNote)
                    ? "No legal target is available from this position. Move, then reopen the book."
                    : card.TacticalNote;
            }

            return "";
        }

        public static string TargetCountLabel(CombatAbilityModalCardView card)
        {
            if (card == null) return "";
            if (!card.Targeted) return "Immediate";
            if (!card.TargetCountKnown) return card.Target ?? "Target";
            if (card.ValidTargetCount <= 0) return "No legal target now";
            return card.ValidTargetCount == 1 ? "1 legal target" : card.ValidTargetCount + " legal targets";
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
            switch (filter)
            {
                case CombatAbilityModalFilter.Ready: label = "READY"; break;
                case CombatAbilityModalFilter.Learned: label = "KNOWN"; break;
                case CombatAbilityModalFilter.Future: label = "PROGRESSION"; break;
                default: label = "ALL"; break;
            }
            return $"{label}  {Count(cards, filter)}";
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
            float splitX = Mathf.Clamp(panelW * 0.59f, 600f, panelW - 410f);
            Rect filters = new Rect(24f, 78f, splitX - 36f, 40f);
            Rect list = new Rect(24f, 128f, splitX - 36f, panelH - 164f);
            Rect detail = new Rect(splitX + 8f, 128f, panelW - splitX - 32f, panelH - 164f);
            Rect close = new Rect(panelW - 238f, 18f, 214f, 42f);
            Rect footer = new Rect(24f, panelH - 28f, panelW - 48f, 18f);
            return new CombatAbilityModalGeometry(backdrop, panel, filters, list, detail, close, footer);
        }
    }

    internal sealed class CombatAbilityModalFocusRelay : MonoBehaviour, IPointerEnterHandler, ISelectHandler
    {
        public Action Focus;

        public void OnPointerEnter(PointerEventData eventData)
        {
            Focus?.Invoke();
        }

        public void OnSelect(BaseEventData eventData)
        {
            Focus?.Invoke();
        }
    }

    public sealed class CombatAbilityModalScreen : MonoBehaviour
    {
        private const float BaseRowHeight = 75f;
        private const float BaseRowGap = 7f;
        private static readonly CombatAbilityModalFilter[] Filters =
        {
            CombatAbilityModalFilter.Ready,
            CombatAbilityModalFilter.Learned,
            CombatAbilityModalFilter.Future
        };

        private readonly List<CardRow> cardRows = new List<CardRow>();
        private readonly List<CombatAbilityModalCardView> visibleCards = new List<CombatAbilityModalCardView>();
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
        private RectTransform detailIconFrame;
        private Image detailIcon;
        private Text detailSigil;
        private Text titleText;
        private Text actorText;
        private Text resourceText;
        private Text actionStateText;
        private Text traitText;
        private Text emptyText;
        private Text detailTitle;
        private Text detailStatus;
        private Text detailSummary;
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
        private float lastWidth = -1f;
        private float lastHeight = -1f;
        private float rowHeight = BaseRowHeight;
        private float rowGap = BaseRowGap;
        private string selectedId = "";
        private int activeCardCount;
        private int openedFrame = -1;
        private string listContextKey = "";
        private bool filterInitialized;
        private bool lastRefreshSucceeded;
        private CombatAbilityModalFilter currentFilter = CombatAbilityModalFilter.Ready;
        private float previousControllerVertical;
        private EventSystem capturedEventSystem;
        private bool previousSendNavigationEvents;
        private bool navigationEventsCaptured;

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

        public void Bind(CombatAbilityModalBindings modalBindings)
        {
            bindings = modalBindings;
            Build();
            SetVisible(false);
            Refresh();
        }

        public bool SetVisible(bool visible)
        {
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
                lastWidth = -1f;
                lastHeight = -1f;
                FocusSelectedControl();
            }
            return changed;
        }

        private void OnDisable()
        {
            ReleaseNavigationEvents();
        }

        private void OnDestroy()
        {
            ReleaseNavigationEvents();
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
                filterInitialized = false;
                if (listContent != null) listContent.anchoredPosition = Vector2.zero;
            }

            IReadOnlyList<CombatAbilityModalCardView> cards = view.Cards ?? Array.Empty<CombatAbilityModalCardView>();
            if (!filterInitialized)
            {
                currentFilter = CombatAbilityModalPresentationRules.InitialFilter(cards);
                filterInitialized = true;
            }
            else if (currentFilter == CombatAbilityModalFilter.Future
                && CombatAbilityModalPresentationRules.Count(cards, CombatAbilityModalFilter.Future) == 0)
            {
                currentFilter = CombatAbilityModalPresentationRules.InitialFilter(cards);
            }

            RefreshFilters(cards, accent);
            RebuildVisibleCards(cards);
            ReconcileSelection(view.SelectedId);
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
            RefreshDetail(selected, accent);
            LayoutRows();
            EnsureSelectedVisible();
            Canvas.ForceUpdateCanvases();
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

        public void InvokeSelectedForTest()
        {
            CombatAbilityModalCardView card = FindVisibleCard(selectedId);
            if (!CombatAbilityModalPresentationRules.CanActivate(card))
            {
                throw new InvalidOperationException("Selected modal power is not activatable.");
            }
            ActivateSelected();
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

            float controllerVertical = Input.GetAxisRaw("Vertical");
            bool controllerUp = controllerVertical >= 0.65f && previousControllerVertical < 0.65f;
            bool controllerDown = controllerVertical <= -0.65f && previousControllerVertical > -0.65f;
            previousControllerVertical = controllerVertical;

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
                MoveSelection(-PageSize());
                return;
            }
            if (Input.GetKeyDown(KeyCode.PageDown))
            {
                MoveSelection(PageSize());
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
                ActivateSelected();
                return;
            }
            if (Input.GetKeyDown(KeyCode.JoystickButton1))
            {
                bindings?.Close?.Invoke();
                return;
            }

            if (controllerUp) MoveSelection(-1);
            else if (controllerDown) MoveSelection(1);
        }

        private void RefreshFilters(IReadOnlyList<CombatAbilityModalCardView> cards, Color accent)
        {
            if (filterButtons == null || filterLabels == null) return;
            bool showProgression = CombatAbilityModalPresentationRules.Count(cards, CombatAbilityModalFilter.Future) > 0;
            LayoutFilterButtons(showProgression);
            for (int i = 0; i < Filters.Length; i++)
            {
                CombatAbilityModalFilter filter = Filters[i];
                bool visible = filter != CombatAbilityModalFilter.Future || showProgression;
                filterButtons[i].gameObject.SetActive(visible);
                if (!visible) continue;
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

        private void LayoutFilterButtons(bool showProgression)
        {
            if (filterButtons == null) return;
            CombatAbilityModalGeometry geometry = CombatAbilityModalLayout.Calculate(Screen.width, Screen.height);
            int visibleCount = showProgression ? Filters.Length : Filters.Length - 1;
            float filterGap = 8f;
            float filterWidth = (geometry.Filters.width - filterGap * (visibleCount - 1)) / visibleCount;
            int visibleIndex = 0;
            for (int i = 0; i < Filters.Length; i++)
            {
                if (Filters[i] == CombatAbilityModalFilter.Future && !showProgression) continue;
                SetLocalRect(
                    filterButtons[i].GetComponent<RectTransform>(),
                    new Rect(
                        geometry.Filters.x + visibleIndex * (filterWidth + filterGap),
                        geometry.Filters.y,
                        filterWidth,
                        geometry.Filters.height));
                visibleIndex++;
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
                selectedId = requestedId;
                return;
            }
            if (IndexOfVisible(selectedId) >= 0) return;

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
                    return "Nothing can be used from this position. Open Learned to see requirements and repositioning notes.";
                case CombatAbilityModalFilter.Learned:
                    return "No powers have been learned yet.";
                case CombatAbilityModalFilter.Future:
                    return "This book has no future unlocks.";
                default:
                    return string.IsNullOrWhiteSpace(view?.EmptyText) ? "Nothing is available right now." : view.EmptyText;
            }
        }

        private void RefreshRow(CardRow row, CombatAbilityModalCardView card, Color accent)
        {
            row.Id = card.Id ?? "";
            bool selected = string.Equals(row.Id, selectedId, StringComparison.Ordinal);
            bool canActivate = CombatAbilityModalPresentationRules.CanActivate(card);
            bool targeting = card.Ready && card.Usable;
            bool exceptionalState = targeting
                || card.Locked
                || !card.Usable
                || !CombatAbilityModalPresentationRules.HasCurrentTarget(card);
            Color cardAccent = CardAccent(card, accent);
            Color fill = targeting
                ? Hex("241a0d", 0.98f)
                : card.Locked
                    ? Hex("090c0f", 0.98f)
                    : selected
                        ? Hex("102126", 0.98f)
                        : Hex("12181d", 0.98f);
            row.Button.interactable = true;
            row.Background.color = fill;
            row.Outline.effectColor = targeting
                ? Hex("d7a84e", 0.96f)
                : selected
                    ? cardAccent.WithAlpha(0.96f)
                    : card.Locked
                        ? Hex("303638", 0.72f)
                        : Hex("3c4544", 0.82f);
            row.Outline.effectDistance = selected || targeting ? new Vector2(2f, -2f) : new Vector2(1f, -1f);
            row.SelectionRail.gameObject.SetActive(selected);
            row.SelectionRail.color = cardAccent;
            row.SelectionChevron.gameObject.SetActive(selected);
            row.IconFrame.GetComponent<Outline>().effectColor = cardAccent.WithAlpha(card.Locked ? 0.38f : 0.82f);
            RefreshIcon(row.Icon, row.Sigil, card, cardAccent);
            row.Name.text = card.Name ?? "";
            row.Meta.text = CompactRowFacts(card);
            row.Summary.text = FirstNonEmpty(card.RowSummary, card.Summary, card.CurrentEffect);
            row.State.text = CombatAbilityModalPresentationRules.AvailabilityLabel(card);
            row.State.color = AvailabilityTextColor(card);
            row.StatusBadge.gameObject.SetActive(exceptionalState);
            row.StatusBadge.GetComponent<Image>().color = AvailabilityFill(card);
            row.StatusBadge.GetComponent<Outline>().effectColor = AvailabilityTextColor(card).WithAlpha(0.72f);
            row.Name.color = card.Locked ? Hex("9aa0a1", 1f) : Hex("f3ead7", 1f);
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
                RefreshIcon(detailIcon, detailSigil, null, accent);
                detailTitle.text = "No selection";
                detailStatus.text = "";
                detailStatus.gameObject.SetActive(false);
                detailSummary.text = "Choose a filter or select a power.";
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
                return;
            }

            Color cardAccent = CardAccent(card, accent);
            bool canActivate = CombatAbilityModalPresentationRules.CanActivate(card);
            detailPanel.GetComponent<Image>().color = Color.Lerp(Hex("080b0d", 0.98f), cardAccent.WithAlpha(0.98f), 0.10f);
            detailPanel.GetComponent<Outline>().effectColor = cardAccent.WithAlpha(0.76f);
            detailIconFrame.GetComponent<Outline>().effectColor = cardAccent.WithAlpha(0.88f);
            RefreshIcon(detailIcon, detailSigil, card, cardAccent);
            detailTitle.text = card.Name ?? "";
            detailStatus.text = CombatAbilityModalPresentationRules.AvailabilityLabel(card);
            detailStatus.color = AvailabilityTextColor(card);
            detailStatus.gameObject.SetActive(
                card.Ready
                || card.Locked
                || !card.Usable
                || !CombatAbilityModalPresentationRules.HasCurrentTarget(card));
            detailSummary.text = FirstNonEmpty(card.CurrentEffect, card.Summary, card.RowSummary);
            detailPrompt.text = RelevantContext(card);
            detailPrompt.gameObject.SetActive(!string.IsNullOrWhiteSpace(detailPrompt.text));
            detailPrompt.color = canActivate ? Hex("8ed7c7", 1f) : Hex("e0b96a", 1f);
            detailActionLabel.text = CombatAbilityModalPresentationRules.CardActionLabel(card);
            Color actionFill = canActivate
                ? card.Ready ? Hex("4b3516", 1f) : cardAccent.WithAlpha(0.92f)
                : Hex("101417", 0.96f);
            ColorBlock actionColors = detailActionButton.colors;
            actionColors.normalColor = Color.white;
            actionColors.highlightedColor = Color.white;
            actionColors.selectedColor = Color.white;
            actionColors.pressedColor = canActivate
                ? new Color(0.86f, 0.86f, 0.86f, 1f)
                : Color.white;
            actionColors.disabledColor = Color.white;
            detailActionButton.colors = actionColors;
            Image actionImage = detailActionButton.targetGraphic as Image;
            if (actionImage != null) actionImage.color = actionFill;
            detailActionButton.interactable = canActivate;
            detailActionLabel.color = canActivate
                ? CardActionTextColor(actionFill)
                : Hex("9aa0a1", 1f);
            SetStatChip(0, "COST", card.Cost);
            SetStatChip(1, "REACH", CompactReach(card));
            SetStatChip(2, "TARGET", CombatAbilityModalPresentationRules.TargetCountLabel(card));
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
            closeButton = AddButton("Back", panel, "Back to Board  [Esc]", () => bindings?.Close?.Invoke());
            closeButtonLabel = closeButton.GetComponentInChildren<Text>();

            filterButtons = new Button[Filters.Length];
            filterLabels = new Text[Filters.Length];
            for (int i = 0; i < Filters.Length; i++)
            {
                int filterIndex = i;
                filterButtons[i] = AddButton("Filter " + Filters[i], panel, Filters[i].ToString(), () => SetFilter(Filters[filterIndex]));
                filterLabels[i] = filterButtons[i].GetComponentInChildren<Text>();
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
            detailIconFrame = AddPanel("Detail Icon Frame", detailPanel, Hex("050708", 0.98f), Hex("a77ae8", 0.88f));
            detailIcon = AddImage("Detail Icon", detailIconFrame, Color.white);
            detailIcon.preserveAspect = true;
            detailIcon.raycastTarget = false;
            detailSigil = AddText("Detail Sigil", detailIconFrame, "", 14, Hex("f3ead7", 1f), TextAnchor.MiddleCenter);
            detailSigil.fontStyle = FontStyle.Bold;
            detailSigil.raycastTarget = false;
            detailTitle = AddText("Detail Title", detailPanel, "", 20, Hex("f3ead7", 1f), TextAnchor.MiddleLeft);
            detailStatus = AddText("Detail Status", detailPanel, "", 12, Hex("8ed7c7", 1f), TextAnchor.MiddleLeft);
            detailStatus.fontStyle = FontStyle.Bold;
            statChips = new[]
            {
                CreateStatChip("Cost", detailPanel),
                CreateStatChip("Reach", detailPanel),
                CreateStatChip("Target", detailPanel)
            };
            detailSummary = AddText("Current Effect", detailPanel, "", 14, Hex("f3ead7", 1f), TextAnchor.UpperLeft);
            detailSummary.fontStyle = FontStyle.Bold;
            detailPrompt = AddText("Detail Prompt", detailPanel, "", 12, Hex("8ed7c7", 1f), TextAnchor.UpperLeft);
            detailActionButton = AddButton("Primary Action", detailPanel, "Choose Target", ActivateSelected);
            detailActionLabel = detailActionButton.GetComponentInChildren<Text>();
            footerHint = AddText(
                "Keyboard Help",
                panel,
                "↑↓ Select   Enter Use   1–3 View   Esc Back",
                11,
                Hex("b7aa90", 1f),
                TextAnchor.MiddleLeft);
        }

        private void ApplyLayout()
        {
            lastWidth = Screen.width;
            lastHeight = Screen.height;
            float uiScale = Mathf.Clamp(Screen.height / 720f, 1f, 1.25f);
            rowHeight = Mathf.Round(BaseRowHeight * uiScale);
            rowGap = Mathf.Round(BaseRowGap * uiScale);
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

            bool showProgression = CombatAbilityModalPresentationRules.Count(
                currentView?.Cards,
                CombatAbilityModalFilter.Future) > 0;
            LayoutFilterButtons(showProgression);

            SetLocalRect(listFrame, geometry.List);
            SetLocalRect(listViewport, new Rect(7f, 7f, geometry.List.width - 34f, geometry.List.height - 14f));
            SetLocalRect(listScrollbar.GetComponent<RectTransform>(), new Rect(geometry.List.width - 22f, 8f, 15f, geometry.List.height - 16f));
            SetLocalRect(emptyText.rectTransform, new Rect(18f, 18f, geometry.List.width - 64f, geometry.List.height - 36f));
            SetLocalRect(listScrollHint.rectTransform, new Rect(geometry.List.xMax - 96f, geometry.Footer.y, 96f, geometry.Footer.height));

            SetLocalRect(detailPanel, geometry.Detail);
            float detailW = geometry.Detail.width;
            float detailH = geometry.Detail.height;
            SetLocalRect(detailIconFrame, new Rect(16f, 16f, 78f, 78f));
            Stretch(detailIcon.rectTransform, 5f, 5f);
            Stretch(detailSigil.rectTransform, 5f, 5f);
            SetLocalRect(detailTitle.rectTransform, new Rect(108f, 15f, detailW - 124f, 28f));
            SetLocalRect(detailStatus.rectTransform, new Rect(108f, 52f, detailW - 124f, 24f));

            float chipGap = 8f;
            float chipWidth = (detailW - 32f - chipGap * 2f) / 3f;
            SetLocalRect(detailSummary.rectTransform, new Rect(16f, 112f, detailW - 32f, 78f));
            LayoutStatChip(statChips[0], new Rect(16f, 204f, chipWidth, 44f));
            LayoutStatChip(statChips[1], new Rect(16f + chipWidth + chipGap, 204f, chipWidth, 44f));
            LayoutStatChip(statChips[2], new Rect(16f + (chipWidth + chipGap) * 2f, 204f, chipWidth, 44f));

            float actionY = detailH - 60f;
            float promptY = actionY - 100f;
            SetLocalRect(detailPrompt.rectTransform, new Rect(16f, promptY, detailW - 32f, 84f));
            SetLocalRect(detailActionButton.GetComponent<RectTransform>(), new Rect(16f, actionY, detailW - 32f, 44f));
            SetLocalRect(footerHint.rectTransform, new Rect(geometry.Footer.x, geometry.Footer.y, geometry.List.width - 108f, geometry.Footer.height));
            LayoutRows();
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
            SetFontSize(detailSummary, 14, scale);
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
            float scale = Mathf.Clamp(Screen.height / 720f, 1f, 1.25f);
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
            button.onClick.AddListener(() => SelectVisibleIndex(index, true));
            CombatAbilityModalFocusRelay focusRelay = root.gameObject.AddComponent<CombatAbilityModalFocusRelay>();
            focusRelay.Focus = () => PreviewVisibleIndex(index);

            Image selectionRail = AddImage("Selection Rail", root, Hex("66cdb9", 1f));
            Text selectionChevron = AddText("Selection Chevron", root, "›", 18, Hex("f3ead7", 1f), TextAnchor.MiddleCenter);
            selectionChevron.fontStyle = FontStyle.Bold;
            Text name = AddText("Name", root, "", 14, Hex("f3ead7", 1f), TextAnchor.MiddleLeft);
            Text meta = AddText("Meta", root, "", 11, Hex("d7a84e", 1f), TextAnchor.MiddleLeft);
            Text summary = AddText("Summary", root, "", 12, Hex("d8d0c1", 1f), TextAnchor.MiddleLeft);
            RectTransform statusBadge = AddPanel("Status Badge", root, Hex("13201d", 0.96f), Hex("8ed7c7", 0.72f));
            Text state = AddText("State", statusBadge, "", 11, Hex("8ed7c7", 1f), TextAnchor.MiddleCenter);
            state.fontStyle = FontStyle.Bold;
            Stretch(state.rectTransform, 4f, 2f);
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
                IconFrame = iconFrame,
                Icon = icon,
                Sigil = sigil,
                Name = name,
                Meta = meta,
                Summary = summary,
                StatusBadge = statusBadge,
                State = state
            };
        }

        private void LayoutRows()
        {
            if (listViewport == null || listContent == null) return;
            float width = Mathf.Max(1f, listViewport.rect.width);
            float previousScrollY = Mathf.Max(0f, listContent.anchoredPosition.y);
            float totalH = Mathf.Max(listViewport.rect.height, activeCardCount * (rowHeight + rowGap) + 8f);
            listContent.anchorMin = new Vector2(0f, 1f);
            listContent.anchorMax = new Vector2(1f, 1f);
            listContent.pivot = new Vector2(0f, 1f);
            listContent.sizeDelta = new Vector2(0f, totalH);
            float maxScrollY = Mathf.Max(0f, totalH - listViewport.rect.height);
            listContent.anchoredPosition = new Vector2(0f, Mathf.Min(previousScrollY, maxScrollY));
            for (int i = 0; i < cardRows.Count; i++)
            {
                CardRow row = cardRows[i];
                Rect card = new Rect(4f, 4f + i * (rowHeight + rowGap), width - 8f, rowHeight);
                SetLocalRect(row.Root, card);
                SetLocalRect(row.SelectionRail.rectTransform, new Rect(0f, 0f, 5f, rowHeight));
                SetLocalRect(row.SelectionChevron.rectTransform, new Rect(5f, 0f, 14f, rowHeight));
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
            float stride = Mathf.Max(1f, rowHeight + rowGap);
            float top = Mathf.Max(0f, listContent.anchoredPosition.y);
            int first = Mathf.Clamp(Mathf.FloorToInt(top / stride) + 1, 1, visibleCards.Count);
            int last = Mathf.Clamp(Mathf.CeilToInt((top + listViewport.rect.height) / stride), first, visibleCards.Count);
            bool above = first > 1;
            bool below = last < visibleCards.Count;
            string arrows = above && below ? "  ↑↓" : above ? "  ↑" : below ? "  ↓" : "";
            listScrollHint.text = $"{first}–{last} OF {visibleCards.Count}{arrows}";
        }

        private void SetFilter(CombatAbilityModalFilter filter)
        {
            if (currentView?.Cards == null) return;
            int count = CombatAbilityModalPresentationRules.Count(currentView.Cards, filter);
            if (count == 0 && filter != CombatAbilityModalFilter.All) return;
            currentFilter = filter;
            filterInitialized = true;
            selectedId = "";
            if (listContent != null) listContent.anchoredPosition = Vector2.zero;
            Refresh();
            if (!string.IsNullOrWhiteSpace(selectedId))
            {
                bindings?.PreviewCard?.Invoke(selectedId);
                EnsureSelectedVisible();
            }
            FocusSelectedControl();
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
            SelectVisibleIndex(Mathf.Clamp(current + delta, 0, visibleCards.Count - 1), true);
        }

        private void SelectVisibleIndex(int index, bool announce)
        {
            if (index < 0 || index >= visibleCards.Count) return;
            string nextId = visibleCards[index].Id ?? "";
            if (string.IsNullOrWhiteSpace(nextId)) return;
            selectedId = nextId;
            EnsureSelectedVisible();
            if (announce) bindings?.SelectCard?.Invoke(nextId);
            else bindings?.PreviewCard?.Invoke(nextId);
            Refresh();
            EnsureSelectedVisible();
            FocusSelectedControl();
        }

        private void PreviewVisibleIndex(int index)
        {
            if (index < 0 || index >= visibleCards.Count) return;
            string nextId = visibleCards[index].Id ?? "";
            if (string.IsNullOrWhiteSpace(nextId) || string.Equals(selectedId, nextId, StringComparison.Ordinal)) return;
            selectedId = nextId;
            bindings?.PreviewCard?.Invoke(nextId);
            Refresh();
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
            if (!IsVisible || EventSystem.current == null) return;
            GameObject focus = detailActionButton != null && detailActionButton.interactable
                ? detailActionButton.gameObject
                : null;
            if (focus == null)
            {
                int index = IndexOfVisible(selectedId);
                if (index >= 0 && index < cardRows.Count) focus = cardRows[index].Button.gameObject;
            }
            if (focus == null && closeButton != null) focus = closeButton.gameObject;
            if (focus != null) EventSystem.current.SetSelectedGameObject(focus);
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

        private static string CompactRowFacts(CombatAbilityModalCardView card)
        {
            if (card == null) return "";
            string cost = card.Cost?.Trim() ?? "";
            string reach = card.Range?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(cost)) return reach;
            if (string.IsNullOrWhiteSpace(reach)) return cost;
            return cost + "  •  " + reach;
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
            image.color = card != null && card.Locked ? Hex("a2a6a8", 0.42f) : Color.white;
            image.enabled = sprite != null;
            sigil.text = card?.Sigil ?? "";
            sigil.color = card != null && card.Locked ? Hex("9aa0a1", 1f) : accent;
            sigil.gameObject.SetActive(sprite == null && !string.IsNullOrWhiteSpace(sigil.text));
            sigil.alignment = TextAnchor.MiddleCenter;
        }

        private void SetStatChip(int index, string label, string value)
        {
            if (statChips == null || index < 0 || index >= statChips.Length) return;
            statChips[index].Label.text = label ?? "";
            statChips[index].Value.text = string.IsNullOrWhiteSpace(value) ? "—" : value;
        }

        private StatChip CreateStatChip(string name, Transform parent)
        {
            RectTransform root = AddPanel(name + " Stat", parent, Hex("10161a", 0.94f), Hex("3c4544", 0.72f));
            Text label = AddText("Label", root, name.ToUpperInvariant(), 10, Hex("9ea59f", 1f), TextAnchor.MiddleLeft);
            Text value = AddText("Value", root, "", 12, Hex("f3ead7", 1f), TextAnchor.MiddleLeft);
            value.fontStyle = FontStyle.Bold;
            return new StatChip(root, label, value);
        }

        private static void LayoutStatChip(StatChip chip, Rect rect)
        {
            if (chip == null) return;
            SetLocalRect(chip.Root, rect);
            SetLocalRect(chip.Label.rectTransform, new Rect(9f, 3f, rect.width - 18f, 14f));
            SetLocalRect(chip.Value.rectTransform, new Rect(9f, 17f, rect.width - 18f, 20f));
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
            if (card.Ready && card.Usable) return Hex("f0c56c", 1f);
            if (!card.Usable || !CombatAbilityModalPresentationRules.HasCurrentTarget(card)) return Hex("e0b96a", 1f);
            return Hex("8ed7c7", 1f);
        }

        private static Color AvailabilityFill(CombatAbilityModalCardView card)
        {
            if (card == null || card.Locked) return Hex("15191c", 0.98f);
            if (card.Ready && card.Usable) return Hex("34240f", 0.98f);
            if (!card.Usable || !CombatAbilityModalPresentationRules.HasCurrentTarget(card)) return Hex("2b2012", 0.98f);
            return Hex("10241f", 0.98f);
        }

        private static Color CardActionTextColor(Color background)
        {
            float luminance = 0.2126f * background.r + 0.7152f * background.g + 0.0722f * background.b;
            return luminance > 0.42f ? Hex("050708", 1f) : Hex("f5f1df", 1f);
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
            public RectTransform IconFrame;
            public Image Icon;
            public Text Sigil;
            public Text Name;
            public Text Meta;
            public Text Summary;
            public RectTransform StatusBadge;
            public Text State;
        }

        private sealed class StatChip
        {
            public readonly RectTransform Root;
            public readonly Text Label;
            public readonly Text Value;

            public StatChip(RectTransform root, Text label, Text value)
            {
                Root = root;
                Label = label;
                Value = value;
            }
        }
    }
}
