using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AshenHalls
{
    public sealed class ArmoryRowView
    {
        public int Key;
        public string Title;
        public string Subtitle;
        public string Detail;
        public string AccentHex;
        public string Badge;
        public string BadgeAccentHex;
        public string ActionLabel;
        public bool ActionEnabled;
        public bool Selected;
        public Texture2D IconTexture;
        public Rect IconUv;
        public string IconLabel;
    }

    public sealed class ArmoryDetailActionView
    {
        public int Key;
        public string Label;
        public string Detail;
        public string ButtonLabel;
        public string AccentHex;
        public bool Enabled;
    }

    public sealed class ArmoryDetailView
    {
        public string Eyebrow;
        public string Title;
        public string Subtitle;
        public string Summary;
        public string ActionsHeading;
        public bool ExtendedSummary;
        public string AccentHex;
        public Texture2D IconTexture;
        public Rect IconUv;
        public string IconLabel;
        public IReadOnlyList<ArmoryDetailActionView> Actions = Array.Empty<ArmoryDetailActionView>();
    }

    public sealed class ArmoryOverlayView
    {
        public bool Visible;
        public int ActiveTab;
        public int ActiveFilter;
        public string Title;
        public string Subtitle;
        public string Summary;
        public string Footer;
        public bool CompactRows;
        public IReadOnlyList<string> Filters = Array.Empty<string>();
        public IReadOnlyList<ArmoryRowView> Rows = Array.Empty<ArmoryRowView>();
        public ArmoryDetailView Detail;
    }

    public sealed class ArmoryOverlayBindings
    {
        public Func<ArmoryOverlayView> View;
        public Action Close;
        public Action<int> SelectTab;
        public Action<int> SelectFilter;
        public Action<int> RunRowAction;
        public Action<int> RunDetailAction;
    }

    public readonly struct ArmoryOverlayGeometry
    {
        public readonly Rect Backdrop;
        public readonly Rect Panel;
        public readonly Rect Tabs;
        public readonly Rect FullContent;
        public readonly Rect ListContent;
        public readonly Rect Detail;
        public readonly Rect Filters;
        public readonly Rect CloseButton;

        public ArmoryOverlayGeometry(
            Rect backdrop,
            Rect panel,
            Rect tabs,
            Rect fullContent,
            Rect listContent,
            Rect detail,
            Rect filters,
            Rect closeButton)
        {
            Backdrop = backdrop;
            Panel = panel;
            Tabs = tabs;
            FullContent = fullContent;
            ListContent = listContent;
            Detail = detail;
            Filters = filters;
            CloseButton = closeButton;
        }

        public bool Fits(float width, float height)
        {
            return FitsScreen(Backdrop, width, height)
                && FitsScreen(Panel, width, height)
                && FitsLocal(Tabs, Panel)
                && FitsLocal(FullContent, Panel)
                && FitsLocal(ListContent, Panel)
                && FitsLocal(Detail, Panel)
                && FitsLocal(Filters, Panel)
                && FitsLocal(CloseButton, Panel);
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

    public static class ArmoryOverlayLayout
    {
        public static ArmoryOverlayGeometry Calculate(float width, float height)
        {
            float panelW = Mathf.Min(Mathf.Max(960f, width * 0.82f), width - 48f);
            float panelH = Mathf.Min(Mathf.Max(600f, height * 0.84f), height - 48f);
            Rect backdrop = new Rect(0f, 0f, width, height);
            Rect panel = new Rect((width - panelW) * 0.5f, (height - panelH) * 0.5f, panelW, panelH);
            Rect close = new Rect(panelW - 118f, 18f, 94f, 32f);
            Rect tabs = new Rect(24f, 68f, panelW - 48f, 34f);
            Rect fullContent = new Rect(24f, 112f, panelW - 48f, panelH - 150f);
            float detailW = Mathf.Clamp(fullContent.width * 0.35f, 320f, 390f);
            float gap = 14f;
            Rect listContent = new Rect(fullContent.x, fullContent.y, fullContent.width - detailW - gap, fullContent.height);
            Rect detail = new Rect(listContent.xMax + gap, fullContent.y, detailW, fullContent.height);
            Rect filters = new Rect(listContent.x, listContent.y, listContent.width, 32f);
            return new ArmoryOverlayGeometry(backdrop, panel, tabs, fullContent, listContent, detail, filters, close);
        }

        public static Rect[] TabRects(float width)
        {
            return EvenButtonRects(width, 5, 8f, 32f, 144f);
        }

        public static Rect[] FilterRects(float width, int count)
        {
            return EvenButtonRects(width, Mathf.Max(0, count), 7f, 30f, 104f);
        }

        private static Rect[] EvenButtonRects(float width, int count, float gap, float height, float maxWidth)
        {
            if (count <= 0) return Array.Empty<Rect>();
            float buttonW = Mathf.Min(maxWidth, (width - gap * (count - 1)) / count);
            Rect[] rects = new Rect[count];
            for (int i = 0; i < count; i++) rects[i] = new Rect(i * (buttonW + gap), 0f, buttonW, height);
            return rects;
        }
    }

    internal sealed class ArmoryRowInteractionRelay :
        MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        ISelectHandler,
        IDeselectHandler
    {
        public Action Enter;
        public Action Exit;
        public Action Select;
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
            Select?.Invoke();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            Deselect?.Invoke();
        }
    }

    public sealed class ArmoryOverlayScreen : MonoBehaviour
    {
        private const float StandardRowHeight = 94f;
        private const float CompactRowHeight = 72f;
        private const float StandardRowGap = 8f;
        private const float CompactRowGap = 6f;
        private const float DetailActionHeight = 42f;
        private const float DetailActionGap = 5f;

        private readonly List<Button> tabButtons = new List<Button>();
        private readonly List<Text> tabLabels = new List<Text>();
        private readonly List<Button> filterButtons = new List<Button>();
        private readonly List<Text> filterLabels = new List<Text>();
        private readonly List<RowControls> rowControls = new List<RowControls>();
        private readonly List<DetailActionControls> detailActionControls = new List<DetailActionControls>();
        private ArmoryOverlayBindings bindings;
        private Canvas canvas;
        private RectTransform backdrop;
        private RectTransform panel;
        private RectTransform accentStrip;
        private RectTransform tabsRoot;
        private RectTransform filtersRoot;
        private RectTransform contentViewport;
        private RectTransform contentRoot;
        private ScrollRect contentScroll;
        private RectTransform detailPanel;
        private RectTransform detailIconFrame;
        private RectTransform detailActionsRoot;
        private Text detailActionsHeadingText;
        private RawImage detailIcon;
        private Text detailIconFallback;
        private Text titleText;
        private Text subtitleText;
        private Text summaryText;
        private Text footerText;
        private Text emptyText;
        private Text detailEyebrowText;
        private Text detailTitleText;
        private Text detailSubtitleText;
        private Text detailSummaryText;
        private Button closeButton;
        private Font font;
        private float lastWidth = -1f;
        private float lastHeight = -1f;
        private int lastTab = -1;
        private int lastFilter = -1;
        private int lastFilterCount = -1;
        private bool lastDetailVisible;
        private bool lastExtendedDetailSummary;
        private bool lastCompactRows;
        private int lastSelectedRowKey = int.MinValue;
        private bool lastRefreshSucceeded;
        private int visibleRowCount;
        private int visibleFilterCount;
        private int visibleDetailActionCount;
        private int hoveredRowIndex = -1;
        private int focusedRowIndex = -1;

        private float CurrentRowHeight => lastCompactRows ? CompactRowHeight : StandardRowHeight;
        private float CurrentRowGap => lastCompactRows ? CompactRowGap : StandardRowGap;

        public bool IsReady => canvas != null && panel != null && closeButton != null && contentViewport != null;
        public bool IsVisible => IsReady && UiRuntime.IsCanvasVisible(canvas);
        public bool HasRenderableGeometry => IsReady
            && lastRefreshSucceeded
            && UiRuntime.CanOwnModal(canvas, panel, null, closeButton);
        public int VisibleRowCountForTest => visibleRowCount;
        public int VisibleFilterCountForTest => visibleFilterCount;
        public bool HasVisibleDetailForTest => detailPanel != null && detailPanel.gameObject.activeSelf;
        public int VisibleDetailActionCountForTest => visibleDetailActionCount;
        public bool SelectedRowUsesDirectSelectionForTest
        {
            get
            {
                for (int i = 0; i < visibleRowCount && i < rowControls.Count; i++)
                {
                    if (rowControls[i].Selected && rowControls[i].SelectButton.interactable) return true;
                }
                return false;
            }
        }
        public string ActiveTabLabelForTest => lastTab >= 0 && lastTab < tabLabels.Count ? tabLabels[lastTab].text : "";
        public float ScrollOffsetForTest => contentRoot == null ? 0f : contentRoot.anchoredPosition.y;
        public float ContentHeightForTest => contentRoot == null ? 0f : contentRoot.sizeDelta.y;
        public float ViewportHeightForTest => contentViewport == null ? 0f : contentViewport.rect.height;
        public int CommittedRowIndexForTest => CommittedVisibleRowIndex();
        public int FocusedRowIndexForTest => IsVisibleRowIndex(focusedRowIndex) ? focusedRowIndex : -1;
        public int HoveredRowIndexForTest => IsVisibleRowIndex(hoveredRowIndex) ? hoveredRowIndex : -1;
        public bool FocusedRowIsCommittedForTest =>
            IsVisibleRowIndex(focusedRowIndex) && rowControls[focusedRowIndex].Selected;

        public void Bind(ArmoryOverlayBindings overlayBindings)
        {
            bindings = overlayBindings;
            Build();
            SetVisible(false);
            Refresh();
        }

        public void SetVisible(bool visible)
        {
            EventSystem eventSystem = visible ? UiRuntime.EnsureEventSystemReady() : EventSystem.current;
            if (eventSystem == null && !Application.isPlaying) eventSystem = UiRuntime.EnsureEventSystemReady();
            GameObject selected = eventSystem == null ? null : eventSystem.currentSelectedGameObject;
            bool ownedSelection = IsCanvasSelection(selected);
            bool changed = UiRuntime.SetCanvasVisible(canvas, visible);
            if (!changed) return;

            ClearTransientRowContext();
            if (ownedSelection && eventSystem != null) eventSystem.SetSelectedGameObject(null);
            if (visible) FocusSelectedRow(eventSystem);
        }

        public void Refresh()
        {
            lastRefreshSucceeded = false;
            if (bindings == null || canvas == null) return;
            ArmoryOverlayView view = bindings.View == null ? null : bindings.View();
            if (view == null || !view.Visible)
            {
                SetVisible(false);
                return;
            }

            bool detailVisible = view.Detail != null;
            bool extendedDetailSummary = view.Detail != null && view.Detail.ExtendedSummary;
            bool navigationChanged = lastTab != view.ActiveTab || lastFilter != view.ActiveFilter;
            int selectedRowKey = int.MinValue;
            for (int i = 0; i < view.Rows.Count; i++)
            {
                if (view.Rows[i].Selected)
                {
                    selectedRowKey = view.Rows[i].Key;
                    break;
                }
            }
            bool selectionChanged = lastSelectedRowKey != selectedRowKey;
            if (!Mathf.Approximately(lastWidth, Screen.width)
                || !Mathf.Approximately(lastHeight, Screen.height)
                || lastTab != view.ActiveTab
                || lastDetailVisible != detailVisible
                || lastExtendedDetailSummary != extendedDetailSummary
                || lastFilterCount != view.Filters.Count
                || lastCompactRows != view.CompactRows)
            {
                lastCompactRows = view.CompactRows;
                ApplyLayout(detailVisible, view.Filters.Count, extendedDetailSummary);
            }

            titleText.text = string.IsNullOrWhiteSpace(view.Title) ? "Inventory & Equipment" : view.Title;
            subtitleText.text = string.IsNullOrWhiteSpace(view.Subtitle) ? "Inspect equipment and choose who should use each find." : view.Subtitle;
            subtitleText.gameObject.SetActive(!string.IsNullOrWhiteSpace(view.Subtitle));
            summaryText.text = view.Summary ?? "";
            footerText.text = view.Footer ?? "";
            RefreshTabs(view.ActiveTab);
            RefreshFilters(view.Filters, view.ActiveFilter, detailVisible);
            visibleRowCount = view.Rows.Count;
            EnsureRowCount(visibleRowCount);
            emptyText.gameObject.SetActive(view.Rows.Count == 0);
            emptyText.text = view.ActiveTab == 1
                ? view.ActiveFilter == 3 ? "No clear upgrades in this inventory." : "No items match this filter."
                : "Nothing to show yet.";

            for (int i = 0; i < rowControls.Count; i++)
            {
                bool visible = i < view.Rows.Count;
                RowControls row = rowControls[i];
                row.Root.gameObject.SetActive(visible);
                if (!visible) continue;
                RefreshRow(row, view.Rows[i], i);
            }

            LayoutRows();
            if (navigationChanged) SetContentScrollOffset(0f);
            else ClampContentScrollOffset();
            RefreshDetail(view.Detail);
            Canvas.ForceUpdateCanvases();
            lastTab = view.ActiveTab;
            lastFilter = view.ActiveFilter;
            lastFilterCount = view.Filters.Count;
            lastDetailVisible = detailVisible;
            lastExtendedDetailSummary = extendedDetailSummary;
            lastCompactRows = view.CompactRows;
            lastSelectedRowKey = selectedRowKey;
            lastRefreshSucceeded = true;
            EventSystem refreshEventSystem = EventSystem.current;
            bool focusNeedsRecovery = CanvasSelectionNeedsRecovery(refreshEventSystem);
            if (navigationChanged || selectionChanged || focusNeedsRecovery)
            {
                ClearTransientRowContext();
                if (refreshEventSystem != null && IsCanvasSelection(refreshEventSystem.currentSelectedGameObject))
                {
                    refreshEventSystem.SetSelectedGameObject(null);
                }
                if (IsVisible) FocusSelectedRow(refreshEventSystem);
            }
        }

        public void InvokeRowActionForTest(int visibleIndex)
        {
            if (visibleIndex < 0 || visibleIndex >= visibleRowCount || visibleIndex >= rowControls.Count) return;
            bindings?.RunRowAction?.Invoke(rowControls[visibleIndex].Key);
        }

        public void HoverRowForTest(int visibleIndex)
        {
            SetHoveredRow(visibleIndex);
        }

        public void FocusRowForTest(int visibleIndex)
        {
            FocusRow(visibleIndex, EventSystem.current);
        }

        public void InvokeFocusedRowForTest()
        {
            if (!IsVisibleRowIndex(focusedRowIndex)) return;
            Button submit = RowSubmitControl(rowControls[focusedRowIndex]);
            if (submit == null || !submit.gameObject.activeInHierarchy || !submit.interactable) return;
            submit.onClick.Invoke();
        }

        public void InvokeDetailActionForTest(int visibleIndex)
        {
            if (visibleIndex < 0 || visibleIndex >= detailActionControls.Count) return;
            DetailActionControls action = detailActionControls[visibleIndex];
            if (!action.Root.gameObject.activeSelf || !action.Root.interactable) return;
            bindings?.RunDetailAction?.Invoke(action.Key);
        }

        public int VisibleRowIndexForKeyForTest(int key)
        {
            for (int i = 0; i < visibleRowCount && i < rowControls.Count; i++)
            {
                if (rowControls[i].Key == key) return i;
            }
            return -1;
        }

        public void ScrollRowIntoViewForTest(int visibleIndex)
        {
            if (visibleIndex < 0 || visibleIndex >= visibleRowCount) return;
            SetContentScrollOffset(8f + visibleIndex * (CurrentRowHeight + CurrentRowGap));
        }

        public bool IsRowFullyVisibleForTest(int visibleIndex)
        {
            if (visibleIndex < 0 || visibleIndex >= visibleRowCount || contentViewport == null || contentRoot == null) return false;
            float top = 8f + visibleIndex * (CurrentRowHeight + CurrentRowGap);
            float bottom = top + CurrentRowHeight;
            float offset = contentRoot.anchoredPosition.y;
            return top >= offset - 0.5f && bottom <= offset + contentViewport.rect.height + 0.5f;
        }

        private void RefreshTabs(int activeTab)
        {
            string[] labels = { "Equipment", "Inventory", "Spells", "Journal", "Growth" };
            for (int i = 0; i < tabButtons.Count; i++)
            {
                bool active = i == activeTab;
                Image image = tabButtons[i].GetComponent<Image>();
                image.color = active ? Hex("2d3438", 0.98f) : Hex("151b20", 0.52f);
                Outline outline = tabButtons[i].GetComponent<Outline>();
                outline.effectColor = active ? Hex("d7a84e", 0.92f) : Hex("3c4544", 0.28f);
                tabLabels[i].text = labels[i];
                tabLabels[i].color = active ? Hex("f3ead7", 1f) : Hex("b7aa90", 1f);
            }
        }

        private void RefreshFilters(IReadOnlyList<string> filters, int activeFilter, bool detailVisible)
        {
            visibleFilterCount = filters?.Count ?? 0;
            EnsureFilterCount(visibleFilterCount, detailVisible);
            filtersRoot.gameObject.SetActive(visibleFilterCount > 0);
            for (int i = 0; i < filterButtons.Count; i++)
            {
                bool visible = i < visibleFilterCount;
                filterButtons[i].gameObject.SetActive(visible);
                if (!visible) continue;
                bool active = i == activeFilter;
                filterLabels[i].text = filters[i] ?? "";
                filterLabels[i].color = active ? Hex("f3ead7", 1f) : Hex("aeb5ad", 1f);
                Image image = filterButtons[i].GetComponent<Image>();
                image.color = active ? Hex("263139", 0.98f) : Hex("10161a", 0.92f);
                filterButtons[i].GetComponent<Outline>().effectColor = active ? Hex("58b7a5", 0.90f) : Hex("3c4544", 0.60f);
            }
        }

        private void RefreshRow(RowControls row, ArmoryRowView view, int index)
        {
            Color accent = ParseColor(view.AccentHex, Hex("58b7a5", 1f));
            Color badgeAccent = ParseColor(view.BadgeAccentHex, accent);
            row.Key = view.Key;
            row.Selected = view.Selected;
            row.BaseAccent = accent;
            row.Title.text = view.Title ?? "";
            row.Subtitle.text = view.Subtitle ?? "";
            row.Detail.text = view.Detail ?? "";
            row.BadgeRoot.gameObject.SetActive(!string.IsNullOrWhiteSpace(view.Badge));
            row.BadgeText.text = view.Badge ?? "";
            row.BadgeText.color = badgeAccent;
            row.BadgeRoot.GetComponent<Outline>().effectColor = badgeAccent.WithAlpha(0.72f);
            SetIcon(row.Icon, row.IconFallback, view.IconTexture, view.IconUv, view.IconLabel);

            bool hasAction = !string.IsNullOrWhiteSpace(view.ActionLabel);
            row.Action.gameObject.SetActive(hasAction);
            row.Action.interactable = view.ActionEnabled;
            row.ActionLabel.text = view.ActionLabel ?? "";
            row.SelectButton.interactable = !hasAction && view.ActionEnabled;
            RefreshRowInteraction(row, index);
        }

        private void RefreshDetail(ArmoryDetailView view)
        {
            bool visible = view != null;
            detailPanel.gameObject.SetActive(visible);
            if (!visible)
            {
                visibleDetailActionCount = 0;
                detailActionsHeadingText.gameObject.SetActive(false);
                foreach (DetailActionControls controls in detailActionControls) controls.Root.gameObject.SetActive(false);
                return;
            }

            Color accent = ParseColor(view.AccentHex, Hex("d7a84e", 1f));
            detailPanel.GetComponent<Outline>().effectColor = accent.WithAlpha(0.78f);
            detailIconFrame.GetComponent<Outline>().effectColor = accent.WithAlpha(0.86f);
            detailEyebrowText.text = view.Eyebrow ?? "";
            detailEyebrowText.color = accent;
            detailTitleText.text = view.Title ?? "";
            detailSubtitleText.text = view.Subtitle ?? "";
            detailSummaryText.text = view.Summary ?? "";
            detailActionsHeadingText.text = string.IsNullOrWhiteSpace(view.ActionsHeading)
                ? "EQUIP TO"
                : view.ActionsHeading;
            SetIcon(detailIcon, detailIconFallback, view.IconTexture, view.IconUv, view.IconLabel);

            visibleDetailActionCount = view.Actions.Count;
            detailActionsHeadingText.gameObject.SetActive(visibleDetailActionCount > 0);
            EnsureDetailActionCount(visibleDetailActionCount);
            for (int i = 0; i < detailActionControls.Count; i++)
            {
                bool actionVisible = i < view.Actions.Count;
                DetailActionControls controls = detailActionControls[i];
                controls.Root.gameObject.SetActive(actionVisible);
                if (!actionVisible) continue;
                ArmoryDetailActionView action = view.Actions[i];
                Color actionAccent = ParseColor(action.AccentHex, accent);
                controls.Key = action.Key;
                controls.Root.interactable = action.Enabled;
                controls.Label.text = action.Label ?? "";
                controls.Detail.text = action.Detail ?? "";
                controls.ButtonLabel.text = action.ButtonLabel ?? "";
                controls.Root.GetComponent<Outline>().effectColor = actionAccent.WithAlpha(action.Enabled ? 0.76f : 0.34f);
                controls.ButtonLabel.color = action.Enabled ? actionAccent : Hex("6e756e", 1f);
            }
            LayoutDetailActions(view.Actions.Count);
        }

        private void FocusSelectedRow(EventSystem eventSystem = null)
        {
            int index = CommittedVisibleRowIndex();
            if (!IsNavigableVisibleRowIndex(index)) index = -1;
            if (index < 0) index = FirstNavigableVisibleRowIndex();
            if (index >= 0)
            {
                FocusRow(index, eventSystem);
                ScrollRowIntoView(index);
                return;
            }

            if (eventSystem == null) eventSystem = EventSystem.current;
            if (eventSystem == null && !Application.isPlaying) eventSystem = UiRuntime.EnsureEventSystemReady();
            int detailActionIndex = FirstNavigableDetailActionIndex();
            if (eventSystem != null && detailActionIndex >= 0)
            {
                eventSystem.SetSelectedGameObject(detailActionControls[detailActionIndex].Root.gameObject);
                return;
            }
            if (eventSystem != null && lastTab >= 0 && lastTab < tabButtons.Count)
            {
                eventSystem.SetSelectedGameObject(tabButtons[lastTab].gameObject);
            }
        }

        private void FocusRow(int index, EventSystem eventSystem)
        {
            if (!IsVisibleRowIndex(index)) return;
            Button submit = RowSubmitControl(rowControls[index]);
            if (submit == null) return;

            SetFocusedRow(index);
            if (eventSystem == null) eventSystem = EventSystem.current;
            if (eventSystem == null && !Application.isPlaying) eventSystem = UiRuntime.EnsureEventSystemReady();
            if (eventSystem != null) eventSystem.SetSelectedGameObject(submit.gameObject);
        }

        private void SetHoveredRow(int index)
        {
            if (!IsVisibleRowIndex(index)) return;
            int previous = hoveredRowIndex;
            hoveredRowIndex = index;
            RefreshRowInteraction(previous);
            RefreshRowInteraction(index);
        }

        private void ClearHoveredRow(int index)
        {
            if (hoveredRowIndex != index) return;
            hoveredRowIndex = -1;
            RefreshRowInteraction(index);
        }

        private void SetFocusedRow(int index)
        {
            if (!IsVisibleRowIndex(index)) return;
            int previousFocus = focusedRowIndex;
            int previousHover = hoveredRowIndex;
            focusedRowIndex = index;
            hoveredRowIndex = -1;
            RefreshRowInteraction(previousHover);
            if (previousFocus != previousHover) RefreshRowInteraction(previousFocus);
            RefreshRowInteraction(index);
        }

        private void ClearFocusedRow(int index)
        {
            if (focusedRowIndex != index) return;
            focusedRowIndex = -1;
            RefreshRowInteraction(index);
        }

        private void ClearTransientRowContext()
        {
            int previousHover = hoveredRowIndex;
            int previousFocus = focusedRowIndex;
            hoveredRowIndex = -1;
            focusedRowIndex = -1;
            RefreshRowInteraction(previousHover);
            if (previousFocus != previousHover) RefreshRowInteraction(previousFocus);
        }

        private void RefreshRowInteraction(int index)
        {
            if (!IsPresentRowIndex(index)) return;
            RefreshRowInteraction(rowControls[index], index);
        }

        private void RefreshRowInteraction(RowControls row, int index)
        {
            bool selected = row.Selected;
            bool focused = focusedRowIndex == index;
            bool hovered = hoveredRowIndex == index;
            Color accent = row.BaseAccent;

            row.Accent.GetComponent<Image>().color = selected
                ? Hex("d7a84e", 1f)
                : accent.WithAlpha(focused ? 1f : hovered ? 0.84f : 0.68f);
            row.Root.GetComponent<Image>().color = selected
                ? Hex("20272b", 0.99f)
                : focused
                    ? Hex("19262a", 0.98f)
                    : hovered ? Hex("182126", 0.96f) : Hex("151b20", 0.94f);
            row.Outline.effectColor = selected
                ? Hex("d7a84e", 0.96f)
                : focused
                    ? Hex("58b7a5", 0.96f)
                    : hovered ? Hex("58b7a5", 0.68f) : Hex("3c4544", 0.46f);
            row.Outline.effectDistance = selected || focused
                ? new Vector2(2f, -2f)
                : new Vector2(1f, -1f);
            row.Subtitle.color = selected
                ? Hex("d7c28e", 1f)
                : focused ? Hex("d8d0bf", 1f) : Hex("aeb5ad", 1f);
            row.IconFrame.GetComponent<Outline>().effectColor = selected
                ? Hex("d7a84e", 0.82f)
                : focused
                    ? Hex("58b7a5", 0.82f)
                    : hovered ? accent.WithAlpha(0.72f) : Hex("3c4544", 0.62f);
        }

        private int CommittedVisibleRowIndex()
        {
            for (int i = 0; i < visibleRowCount && i < rowControls.Count; i++)
            {
                if (rowControls[i].Selected && rowControls[i].Root.gameObject.activeInHierarchy) return i;
            }
            return -1;
        }

        private int FirstNavigableVisibleRowIndex()
        {
            for (int i = 0; i < visibleRowCount && i < rowControls.Count; i++)
            {
                RowControls row = rowControls[i];
                if (!row.Root.gameObject.activeInHierarchy) continue;
                Button submit = RowSubmitControl(row);
                if (submit != null && submit.gameObject.activeInHierarchy && submit.interactable) return i;
            }
            return -1;
        }

        private bool IsNavigableVisibleRowIndex(int index)
        {
            if (!IsVisibleRowIndex(index)) return false;
            Button submit = RowSubmitControl(rowControls[index]);
            return submit != null && submit.gameObject.activeInHierarchy && submit.interactable;
        }

        private int FirstNavigableDetailActionIndex()
        {
            for (int i = 0; i < visibleDetailActionCount && i < detailActionControls.Count; i++)
            {
                Button action = detailActionControls[i].Root;
                if (action != null && action.gameObject.activeInHierarchy && action.interactable) return i;
            }
            return -1;
        }

        private bool IsVisibleRowIndex(int index)
        {
            return IsVisible
                && IsPresentRowIndex(index)
                && rowControls[index].Root.gameObject.activeInHierarchy;
        }

        private bool IsPresentRowIndex(int index)
        {
            return index >= 0
                && index < visibleRowCount
                && index < rowControls.Count
                && rowControls[index].Root != null
                && rowControls[index].Root.gameObject.activeSelf;
        }

        private static Button RowSubmitControl(RowControls row)
        {
            if (row == null) return null;
            return row.Action != null && row.Action.gameObject.activeSelf
                ? row.Action
                : row.SelectButton;
        }

        private void ScrollRowIntoView(int index)
        {
            if (!IsVisibleRowIndex(index) || contentViewport == null || contentRoot == null) return;
            float top = 8f + index * (CurrentRowHeight + CurrentRowGap);
            float bottom = top + CurrentRowHeight;
            float offset = contentRoot.anchoredPosition.y;
            float viewportBottom = offset + contentViewport.rect.height;
            if (top < offset) SetContentScrollOffset(top);
            else if (bottom > viewportBottom) SetContentScrollOffset(bottom - contentViewport.rect.height);
        }

        private bool IsCanvasSelection(GameObject selected)
        {
            if (selected == null || canvas == null) return false;
            Transform selectedTransform = selected.transform;
            Transform canvasTransform = canvas.transform;
            return selectedTransform == canvasTransform || selectedTransform.IsChildOf(canvasTransform);
        }

        private bool CanvasSelectionNeedsRecovery(EventSystem eventSystem)
        {
            GameObject selected = eventSystem == null ? null : eventSystem.currentSelectedGameObject;
            if (!IsCanvasSelection(selected)) return false;
            if (!selected.activeInHierarchy) return true;
            Selectable selectable = selected.GetComponent<Selectable>();
            return selectable != null && !selectable.IsInteractable();
        }

        private void Build()
        {
            if (canvas != null) return;
            UiRuntime.EnsureEventSystemReady();
            font = UiRuntime.DefaultFont;
            canvas = UiRuntime.CreateOwnedRootCanvas(this, "Armory Overlay Canvas");
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 31;
            CanvasScaler scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            canvas.gameObject.AddComponent<GraphicRaycaster>();
            Stretch(canvas.GetComponent<RectTransform>());

            backdrop = AddImage("Backdrop", canvas.transform, Hex("020303", 0.72f)).rectTransform;
            panel = AddPanel("Inventory and Equipment", canvas.transform, Hex("11171b", 0.995f), Hex("d7a84e", 0.90f));
            accentStrip = AddImage("Accent Strip", panel, Hex("d7a84e", 1f)).rectTransform;
            titleText = AddText("Title", panel, "", 24, Hex("f3ead7", 1f), TextAnchor.MiddleLeft);
            subtitleText = AddText("Subtitle", panel, "", 11, Hex("b7aa90", 1f), TextAnchor.MiddleLeft);
            summaryText = AddText("Summary", panel, "", 10, Hex("58b7a5", 1f), TextAnchor.MiddleRight);
            closeButton = AddButton("Close", panel, "Close", () => bindings?.Close?.Invoke());

            tabsRoot = new GameObject("Tabs", typeof(RectTransform)).GetComponent<RectTransform>();
            tabsRoot.SetParent(panel, false);
            for (int i = 0; i < 5; i++)
            {
                int tab = i;
                Button button = AddButton("Tab " + i, tabsRoot, "", () => bindings?.SelectTab?.Invoke(tab));
                tabButtons.Add(button);
                tabLabels.Add(button.GetComponentInChildren<Text>());
            }

            filtersRoot = new GameObject("Inventory Filters", typeof(RectTransform)).GetComponent<RectTransform>();
            filtersRoot.SetParent(panel, false);

            contentViewport = AddPanel("Rows Viewport", panel, Hex("080b0d", 0.42f), Hex("3c4544", 0.70f));
            Mask mask = contentViewport.gameObject.AddComponent<Mask>();
            mask.showMaskGraphic = true;
            contentScroll = contentViewport.gameObject.AddComponent<ScrollRect>();
            contentScroll.horizontal = false;
            contentScroll.movementType = ScrollRect.MovementType.Clamped;
            contentRoot = new GameObject("Rows", typeof(RectTransform)).GetComponent<RectTransform>();
            contentRoot.SetParent(contentViewport, false);
            contentScroll.content = contentRoot;
            contentScroll.viewport = contentViewport;
            emptyText = AddText("Empty", contentViewport, "", 13, Hex("b7aa90", 1f), TextAnchor.MiddleCenter);

            detailPanel = AddPanel("Selection Detail", panel, Hex("0b1013", 0.97f), Hex("d7a84e", 0.74f));
            detailIconFrame = AddPanel("Detail Icon Frame", detailPanel, Hex("050708", 0.92f), Hex("d7a84e", 0.78f));
            detailIcon = AddRawImage("Detail Icon", detailIconFrame);
            detailIconFallback = AddText("Detail Icon Fallback", detailIconFrame, "", 13, Hex("f3ead7", 1f), TextAnchor.MiddleCenter);
            detailEyebrowText = AddText("Detail Eyebrow", detailPanel, "", 10, Hex("d7a84e", 1f), TextAnchor.MiddleLeft);
            detailTitleText = AddText("Detail Title", detailPanel, "", 16, Hex("f3ead7", 1f), TextAnchor.UpperLeft);
            detailTitleText.fontStyle = FontStyle.Bold;
            detailSubtitleText = AddText("Detail Subtitle", detailPanel, "", 10, Hex("b7aa90", 1f), TextAnchor.UpperLeft);
            detailSummaryText = AddText("Detail Summary", detailPanel, "", 11, Hex("e1dacb", 1f), TextAnchor.UpperLeft);
            detailActionsHeadingText = AddText("Detail Actions Heading", detailPanel, "EQUIP TO", 10, Hex("b7aa90", 1f), TextAnchor.MiddleLeft);
            detailActionsHeadingText.fontStyle = FontStyle.Bold;
            detailActionsRoot = new GameObject("Equip Targets", typeof(RectTransform)).GetComponent<RectTransform>();
            detailActionsRoot.SetParent(detailPanel, false);
            footerText = AddText("Footer", panel, "", 10, Hex("b7aa90", 1f), TextAnchor.MiddleLeft);
        }

        private void ApplyLayout(bool detailVisible, int filterCount, bool extendedDetailSummary)
        {
            lastWidth = Screen.width;
            lastHeight = Screen.height;
            ArmoryOverlayGeometry geometry = ArmoryOverlayLayout.Calculate(Screen.width, Screen.height);
            SetScreenRect(backdrop, geometry.Backdrop);
            SetScreenRect(panel, geometry.Panel);
            SetLocalRect(accentStrip, new Rect(0f, 0f, 5f, geometry.Panel.height));
            SetLocalRect(titleText.rectTransform, new Rect(24f, 10f, geometry.Panel.width - 160f, 30f));
            SetLocalRect(subtitleText.rectTransform, new Rect(26f, 39f, geometry.Panel.width * 0.62f, 18f));
            SetLocalRect(summaryText.rectTransform, new Rect(geometry.Panel.width * 0.55f, 39f, geometry.Panel.width * 0.45f - 154f, 18f));
            SetLocalRect(closeButton.GetComponent<RectTransform>(), geometry.CloseButton);
            SetLocalRect(tabsRoot, geometry.Tabs);
            Rect[] tabs = ArmoryOverlayLayout.TabRects(geometry.Tabs.width);
            for (int i = 0; i < tabButtons.Count && i < tabs.Length; i++) SetLocalRect(tabButtons[i].GetComponent<RectTransform>(), tabs[i]);

            Rect listArea = detailVisible ? geometry.ListContent : geometry.FullContent;
            bool filtersVisible = filterCount > 0;
            Rect viewportArea = filtersVisible
                ? new Rect(listArea.x, listArea.y + 40f, listArea.width, listArea.height - 40f)
                : listArea;
            SetLocalRect(filtersRoot, new Rect(listArea.x, listArea.y, listArea.width, 32f));
            Rect[] filters = ArmoryOverlayLayout.FilterRects(listArea.width, filterCount);
            for (int i = 0; i < filterButtons.Count && i < filters.Length; i++) SetLocalRect(filterButtons[i].GetComponent<RectTransform>(), filters[i]);
            SetLocalRect(contentViewport, viewportArea);
            SetLocalRect(emptyText.rectTransform, new Rect(14f, 14f, viewportArea.width - 28f, viewportArea.height - 28f));
            SetLocalRect(detailPanel, geometry.Detail);
            LayoutDetailPanel(geometry.Detail, extendedDetailSummary);
            SetLocalRect(footerText.rectTransform, new Rect(26f, geometry.Panel.height - 30f, geometry.Panel.width - 52f, 18f));
            LayoutRows();
        }

        private void LayoutDetailPanel(Rect detail, bool extendedSummary)
        {
            SetLocalRect(detailIconFrame, new Rect(16f, 16f, 74f, 74f));
            Stretch(detailIcon.rectTransform, 5f, 5f);
            Stretch(detailIconFallback.rectTransform, 5f, 5f);
            SetLocalRect(detailEyebrowText.rectTransform, new Rect(102f, 14f, detail.width - 118f, 18f));
            SetLocalRect(detailTitleText.rectTransform, new Rect(102f, 34f, detail.width - 118f, 36f));
            SetLocalRect(detailSubtitleText.rectTransform, new Rect(102f, 70f, detail.width - 118f, 22f));
            float summaryHeight = extendedSummary ? 112f : 64f;
            float headingY = extendedSummary ? 222f : 174f;
            float actionsY = extendedSummary ? 244f : 196f;
            SetLocalRect(detailSummaryText.rectTransform, new Rect(16f, 104f, detail.width - 32f, summaryHeight));
            SetLocalRect(detailActionsHeadingText.rectTransform, new Rect(16f, headingY, detail.width - 32f, 18f));
            SetLocalRect(detailActionsRoot, new Rect(16f, actionsY, detail.width - 32f, detail.height - actionsY - 16f));
            LayoutDetailActions(visibleDetailActionCount);
        }

        private void EnsureFilterCount(int count, bool detailVisible)
        {
            while (filterButtons.Count < count)
            {
                int filter = filterButtons.Count;
                Button button = AddButton("Filter " + filter, filtersRoot, "", () => bindings?.SelectFilter?.Invoke(filter));
                filterButtons.Add(button);
                filterLabels.Add(button.GetComponentInChildren<Text>());
            }
            if (lastWidth > 0f)
            {
                ArmoryOverlayGeometry geometry = ArmoryOverlayLayout.Calculate(lastWidth, lastHeight);
                Rect listArea = detailVisible ? geometry.ListContent : geometry.FullContent;
                Rect[] rects = ArmoryOverlayLayout.FilterRects(listArea.width, count);
                for (int i = 0; i < filterButtons.Count && i < rects.Length; i++) SetLocalRect(filterButtons[i].GetComponent<RectTransform>(), rects[i]);
            }
        }

        private void EnsureRowCount(int count)
        {
            while (rowControls.Count < count) rowControls.Add(CreateRow(rowControls.Count));
        }

        private RowControls CreateRow(int index)
        {
            RectTransform root = AddPanel("Row " + index, contentRoot, Hex("151b20", 0.96f), Hex("3c4544", 0.62f));
            Button selectButton = root.gameObject.AddComponent<Button>();
            selectButton.targetGraphic = root.GetComponent<Image>();
            ColorBlock selectColors = selectButton.colors;
            selectColors.normalColor = Color.white;
            selectColors.highlightedColor = Color.white;
            selectColors.pressedColor = Hex("c2cac5", 1f);
            selectColors.selectedColor = Color.white;
            selectColors.disabledColor = Color.white;
            selectColors.colorMultiplier = 1f;
            selectColors.fadeDuration = 0f;
            selectButton.colors = selectColors;
            selectButton.onClick.AddListener(() => bindings?.RunRowAction?.Invoke(rowControls[index].Key));
            RectTransform accent = AddImage("Accent", root, Hex("58b7a5", 1f)).rectTransform;
            RectTransform iconFrame = AddPanel("Icon Frame", root, Hex("050708", 0.88f), Hex("58b7a5", 0.62f));
            RawImage icon = AddRawImage("Icon", iconFrame);
            Text iconFallback = AddText("Icon Fallback", iconFrame, "", 11, Hex("f3ead7", 1f), TextAnchor.MiddleCenter);
            Text title = AddText("Title", root, "", 14, Hex("f3ead7", 1f), TextAnchor.MiddleLeft);
            title.fontStyle = FontStyle.Bold;
            Text subtitle = AddText("Subtitle", root, "", 11, Hex("aeb5ad", 1f), TextAnchor.MiddleLeft);
            Text detail = AddText("Detail", root, "", 11, Hex("d8d0bf", 1f), TextAnchor.UpperLeft);
            RectTransform badgeRoot = AddPanel("Badge", root, Hex("080b0d", 0.78f), Hex("58b7a5", 0.64f));
            Text badgeText = AddText("Badge Label", badgeRoot, "", 9, Hex("58b7a5", 1f), TextAnchor.MiddleCenter);
            Stretch(badgeText.rectTransform, 4f, 2f);
            Button action = AddButton("Action", root, "Inspect", () => bindings?.RunRowAction?.Invoke(rowControls[index].Key));
            Text actionLabel = action.GetComponentInChildren<Text>();
            AttachRowInteractionRelay(selectButton.gameObject, index);
            AttachRowInteractionRelay(action.gameObject, index);
            return new RowControls
            {
                Root = root,
                SelectButton = selectButton,
                Accent = accent,
                Outline = root.GetComponent<Outline>(),
                IconFrame = iconFrame,
                Icon = icon,
                IconFallback = iconFallback,
                Title = title,
                Subtitle = subtitle,
                Detail = detail,
                BadgeRoot = badgeRoot,
                BadgeText = badgeText,
                Action = action,
                ActionLabel = actionLabel
            };
        }

        private void AttachRowInteractionRelay(GameObject target, int index)
        {
            ArmoryRowInteractionRelay relay = target.AddComponent<ArmoryRowInteractionRelay>();
            relay.Enter = () => SetHoveredRow(index);
            relay.Exit = () => ClearHoveredRow(index);
            relay.Select = () => SetFocusedRow(index);
            relay.Deselect = () => ClearFocusedRow(index);
        }

        private void LayoutRows()
        {
            if (contentViewport == null || contentRoot == null) return;
            float width = Mathf.Max(1f, contentViewport.rect.width);
            float rowHeight = CurrentRowHeight;
            float rowGap = CurrentRowGap;
            float totalH = Mathf.Max(contentViewport.rect.height, visibleRowCount * (rowHeight + rowGap) + 8f);
            contentRoot.anchorMin = new Vector2(0f, 1f);
            contentRoot.anchorMax = new Vector2(1f, 1f);
            contentRoot.pivot = new Vector2(0f, 1f);
            contentRoot.sizeDelta = new Vector2(0f, totalH);
            for (int i = 0; i < rowControls.Count; i++)
            {
                RowControls row = rowControls[i];
                Rect item = new Rect(8f, 8f + i * (rowHeight + rowGap), width - 16f, rowHeight);
                SetLocalRect(row.Root, item);
                SetLocalRect(row.Accent, new Rect(0f, 0f, 5f, item.height));
                if (lastCompactRows)
                {
                    SetLocalRect(row.IconFrame, new Rect(13f, 10f, 52f, 52f));
                    Stretch(row.Icon.rectTransform, 4f, 4f);
                    Stretch(row.IconFallback.rectTransform, 4f, 4f);
                    float badgeW = row.BadgeRoot.gameObject.activeSelf ? 86f : 0f;
                    float right = item.width - 14f;
                    SetLocalRect(row.Title.rectTransform, new Rect(76f, 7f, Mathf.Max(100f, right - 76f - badgeW), 20f));
                    SetLocalRect(row.BadgeRoot, new Rect(Mathf.Max(76f, right - badgeW), 8f, badgeW, 20f));
                    SetLocalRect(row.Subtitle.rectTransform, new Rect(76f, 28f, Mathf.Max(120f, right - 76f), 17f));
                    SetLocalRect(row.Detail.rectTransform, new Rect(76f, 47f, Mathf.Max(120f, right - 76f), 18f));
                    SetLocalRect(row.Action.GetComponent<RectTransform>(), new Rect(item.width - 104f, 20f, 92f, 32f));
                }
                else
                {
                    SetLocalRect(row.IconFrame, new Rect(14f, 16f, 62f, 62f));
                    Stretch(row.Icon.rectTransform, 4f, 4f);
                    Stretch(row.IconFallback.rectTransform, 4f, 4f);
                    float actionW = row.Action.gameObject.activeSelf ? 92f : 0f;
                    float badgeW = row.BadgeRoot.gameObject.activeSelf ? 104f : 0f;
                    float textRight = item.width - actionW - 22f;
                    SetLocalRect(row.Title.rectTransform, new Rect(88f, 10f, Mathf.Max(80f, textRight - 88f - badgeW), 20f));
                    SetLocalRect(row.BadgeRoot, new Rect(Mathf.Max(88f, textRight - badgeW), 9f, badgeW, 21f));
                    SetLocalRect(row.Subtitle.rectTransform, new Rect(88f, 33f, Mathf.Max(120f, textRight - 88f), 18f));
                    SetLocalRect(row.Detail.rectTransform, new Rect(88f, 55f, Mathf.Max(120f, textRight - 88f), 32f));
                    SetLocalRect(row.Action.GetComponent<RectTransform>(), new Rect(item.width - actionW - 12f, 31f, actionW, 34f));
                }
            }
        }

        private void SetContentScrollOffset(float offset)
        {
            if (contentViewport == null || contentRoot == null) return;
            float maxOffset = Mathf.Max(0f, contentRoot.sizeDelta.y - contentViewport.rect.height);
            contentRoot.anchoredPosition = new Vector2(0f, Mathf.Clamp(offset, 0f, maxOffset));
            if (contentScroll != null) contentScroll.StopMovement();
        }

        private void ClampContentScrollOffset()
        {
            if (contentRoot == null) return;
            SetContentScrollOffset(contentRoot.anchoredPosition.y);
        }

        private void EnsureDetailActionCount(int count)
        {
            while (detailActionControls.Count < count)
            {
                int index = detailActionControls.Count;
                Button root = AddButton("Equip Target " + index, detailActionsRoot, "", () => bindings?.RunDetailAction?.Invoke(detailActionControls[index].Key));
                Text defaultLabel = root.GetComponentInChildren<Text>();
                defaultLabel.gameObject.SetActive(false);
                Text label = AddText("Name", root.transform, "", 11, Hex("f3ead7", 1f), TextAnchor.MiddleLeft);
                label.fontStyle = FontStyle.Bold;
                Text detail = AddText("Comparison", root.transform, "", 9, Hex("b7aa90", 1f), TextAnchor.MiddleLeft);
                Text buttonLabel = AddText("Action Label", root.transform, "", 10, Hex("58b7a5", 1f), TextAnchor.MiddleRight);
                detailActionControls.Add(new DetailActionControls
                {
                    Root = root,
                    Label = label,
                    Detail = detail,
                    ButtonLabel = buttonLabel
                });
            }
        }

        private void LayoutDetailActions(int visibleCount)
        {
            if (detailActionsRoot == null) return;
            float width = Mathf.Max(1f, detailActionsRoot.rect.width);
            float availableHeight = Mathf.Max(1f, detailActionsRoot.rect.height);
            float actionHeight = visibleCount <= 0
                ? DetailActionHeight
                : Mathf.Clamp(
                    (availableHeight - DetailActionGap * Mathf.Max(0, visibleCount - 1)) / visibleCount,
                    34f,
                    DetailActionHeight);
            for (int i = 0; i < detailActionControls.Count; i++)
            {
                DetailActionControls action = detailActionControls[i];
                SetLocalRect(action.Root.GetComponent<RectTransform>(), new Rect(0f, i * (actionHeight + DetailActionGap), width, actionHeight));
                SetLocalRect(action.Label.rectTransform, new Rect(10f, 2f, width - 98f, 18f));
                SetLocalRect(action.Detail.rectTransform, new Rect(10f, Mathf.Max(18f, actionHeight - 19f), width - 98f, 16f));
                SetLocalRect(action.ButtonLabel.rectTransform, new Rect(width - 88f, Mathf.Max(4f, (actionHeight - 24f) * 0.5f), 76f, 24f));
            }
        }

        private static void SetIcon(RawImage image, Text fallback, Texture2D texture, Rect uv, string fallbackLabel)
        {
            bool hasTexture = texture != null && uv.width > 0f && uv.height > 0f;
            image.gameObject.SetActive(hasTexture);
            fallback.gameObject.SetActive(!hasTexture);
            if (hasTexture)
            {
                image.texture = texture;
                image.uvRect = uv;
                image.color = Color.white;
            }
            fallback.text = string.IsNullOrWhiteSpace(fallbackLabel) ? "ITEM" : fallbackLabel;
        }

        private Button AddButton(string name, Transform parent, string label, Action action)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(Outline));
            go.transform.SetParent(parent, false);
            Image image = go.GetComponent<Image>();
            image.color = Hex("151a1f", 0.96f);
            Button button = go.GetComponent<Button>();
            button.targetGraphic = image;
            ColorBlock colors = button.colors;
            colors.normalColor = image.color;
            colors.highlightedColor = Hex("263139", 1f);
            colors.pressedColor = Hex("0b1013", 1f);
            colors.disabledColor = Hex("0b0f12", 0.70f);
            colors.selectedColor = colors.highlightedColor;
            button.colors = colors;
            Outline outline = go.GetComponent<Outline>();
            outline.effectColor = Hex("d7a84e", 0.82f);
            outline.effectDistance = new Vector2(1f, -1f);
            if (action != null) button.onClick.AddListener(() => action());

            Text text = AddText("Label", go.transform, label, 11, Hex("f3ead7", 1f), TextAnchor.MiddleCenter);
            text.fontStyle = FontStyle.Bold;
            Stretch(text.rectTransform, 6f, 3f);
            return button;
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

        private RawImage AddRawImage(string name, Transform parent)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
            go.transform.SetParent(parent, false);
            RawImage image = go.GetComponent<RawImage>();
            image.color = Color.white;
            image.raycastTarget = false;
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

        private static Color ParseColor(string hex, Color fallback)
        {
            if (string.IsNullOrWhiteSpace(hex)) return fallback;
            if (!hex.StartsWith("#")) hex = "#" + hex;
            return ColorUtility.TryParseHtmlString(hex, out Color color) ? color : fallback;
        }

        private sealed class RowControls
        {
            public int Key;
            public RectTransform Root;
            public Button SelectButton;
            public bool Selected;
            public Color BaseAccent;
            public RectTransform Accent;
            public Outline Outline;
            public RectTransform IconFrame;
            public RawImage Icon;
            public Text IconFallback;
            public Text Title;
            public Text Subtitle;
            public Text Detail;
            public RectTransform BadgeRoot;
            public Text BadgeText;
            public Button Action;
            public Text ActionLabel;
        }

        private sealed class DetailActionControls
        {
            public int Key;
            public Button Root;
            public Text Label;
            public Text Detail;
            public Text ButtonLabel;
        }
    }
}
