using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AshenHalls
{
    public sealed class PartySetupChoiceView
    {
        public string Key;
        public string Name;
        public string Description;
    }

    public sealed class PartySetupMemberView
    {
        public string Name;
        public string RaceKey;
        public string ClassKey;
        public string Origin;
        public string Sigil;
        public string RaceDescription;
        public string ClassDescription;
        public string RaceClassLine;
        public string RoleLine;
        public string GearLine;
        public string ProgressLine;
        public string UnlockLine;
        public string BestSkillLine;
        public string ColorHex;
        public int Strength;
        public int Intelligence;
        public int Agility;
        public int Health;
        public int StatTotal;
        public int StatCap;
        public bool CanIncreaseStats;
        public bool[] CanDecreaseStats;
        public bool CanBoostTalents = true;
        public bool[] CanBoostTalentByIndex;
    }

    public sealed class PartySetupScreenBindings
    {
        public string Title;
        public string Subtitle;
        public Texture2D BackdropArt;
        public Func<Texture2D> WorkshopBackdropArt;
        public Func<bool> ReducedMotion;
        public Action<bool> TabChanged;
        public IReadOnlyList<PartySetupChoiceView> RaceChoices;
        public IReadOnlyList<PartySetupChoiceView> ClassChoices;
        public Func<string, string, Texture2D> PortraitAtlas;
        public Func<string, string, int> PortraitCell;
        public Func<string> SummaryLine;
        public Func<string> WeaknessLine;
        public Func<IReadOnlyList<PartySetupMemberView>> Members;
        public Func<int> SelectedIndex;
        public Func<PartySetupMemberView> SelectedMember;
        public Action<int> SelectMember;
        public Func<bool> CanBegin;
        public Action Begin;
        public Action QuickStart;
        public Action BackToTavern;
        public Action<string> SetName;
        public Action<string> SetRace;
        public Action<string> SetClass;
        public Action CycleClass;
        public Action CycleRace;
        public Action CycleOrigin;
        public Action CycleSigil;
        public Action RandomName;
        public Action RerollGear;
        public Action RerollLook;
        public Action CycleColor;
        public Action<int, int> ChangeStat;
        public Action<string> BoostTalent;
    }

    public readonly struct PartySetupScreenGeometry
    {
        public readonly Rect Top;
        public readonly Rect Roster;
        public readonly Rect Editor;
        public readonly Rect Details;

        public PartySetupScreenGeometry(Rect top, Rect roster, Rect editor, Rect details)
        {
            Top = top;
            Roster = roster;
            Editor = editor;
            Details = details;
        }

        public bool Fits(float width, float height)
        {
            return FitsRect(Top, width, height)
                && FitsRect(Roster, width, height)
                && FitsRect(Editor, width, height)
                && FitsRect(Details, width, height);
        }

        private static bool FitsRect(Rect rect, float width, float height)
        {
            return rect.xMin >= 0f && rect.yMin >= 0f && rect.xMax <= width && rect.yMax <= height;
        }
    }

    public readonly struct PartySetupEditorFlowGeometry
    {
        public readonly bool Compact;
        public readonly Rect IdentityControls;
        public readonly Rect StatControls;
        public readonly Rect SkillControls;
        public readonly Rect Details;
        public readonly Rect Note;

        public PartySetupEditorFlowGeometry(
            bool compact,
            Rect identityControls,
            Rect statControls,
            Rect skillControls,
            Rect details,
            Rect note)
        {
            Compact = compact;
            IdentityControls = identityControls;
            StatControls = statControls;
            SkillControls = skillControls;
            Details = details;
            Note = note;
        }

        public bool Fits(Rect editor)
        {
            return FitsLocal(IdentityControls, editor)
                && FitsLocal(StatControls, editor)
                && FitsLocal(SkillControls, editor)
                && FitsLocal(Details, editor)
                && FitsLocal(Note, editor);
        }

        public bool HasNoOverlaps()
        {
            Rect[] regions = { IdentityControls, StatControls, SkillControls, Details, Note };
            for (int i = 0; i < regions.Length; i++)
            {
                for (int other = i + 1; other < regions.Length; other++)
                {
                    if (regions[i].Overlaps(regions[other])) return false;
                }
            }
            return true;
        }

        private static bool FitsLocal(Rect rect, Rect editor)
        {
            return rect.xMin >= 0f
                && rect.yMin >= 0f
                && rect.xMax <= editor.width
                && rect.yMax <= editor.height;
        }
    }

    public static class PartySetupScreenLayout
    {
        public static readonly IReadOnlyList<string> TalentKeys = Array.AsReadOnly(new[]
        {
            "arms", "missile", "mend", "ember", "hex", "guard"
        });

        private const float EditorSideInset = 18f;
        private const float DetailsControlGap = 12f;
        private const float ExpandedIdentityControlsRight = 748f;
        private const float ExpandedStatY = 186f;
        private const float ExpandedStatStep = 40f;
        private const float ExpandedSkillY = 386f;
        private const float CompactStatY = 238f;
        private const float CompactStatStep = 34f;
        private const float CompactSkillY = 376f;

        public static PartySetupScreenGeometry Calculate(float width, float height)
        {
            Rect top = new Rect(18f, 16f, width - 36f, 68f);
            float rosterW = Mathf.Clamp(width * 0.30f, 318f, 390f);
            Rect roster = new Rect(18f, 100f, rosterW, height - 118f);
            Rect editor = new Rect(roster.xMax + 14f, 100f, width - roster.xMax - 32f, height - 118f);
            Rect details = new Rect(editor.x + editor.width - Mathf.Min(300f, editor.width * 0.34f) - 18f, editor.y + 112f, Mathf.Min(300f, editor.width * 0.34f), 258f);
            return new PartySetupScreenGeometry(top, roster, editor, details);
        }

        public static Rect RosterRow(Rect roster, int index)
        {
            return new Rect(12f, 48f + index * 71f, roster.width - 24f, 64f);
        }

        public static bool UseCompactIdentityLayout(Rect localDetails)
        {
            return localDetails.x < ExpandedIdentityControlsRight + DetailsControlGap;
        }

        public static float CompactIdentityControlsRight(Rect localDetails)
        {
            return Mathf.Max(EditorSideInset, localDetails.x - DetailsControlGap);
        }

        public static PartySetupEditorFlowGeometry CalculateEditorFlow(Rect editor, Rect localDetails, int skillCount)
        {
            bool compact = UseCompactIdentityLayout(localDetails);
            float identityRight = compact ? CompactIdentityControlsRight(localDetails) : ExpandedIdentityControlsRight;
            Rect identity = new Rect(
                EditorSideInset,
                94f,
                Mathf.Max(1f, identityRight - EditorSideInset),
                compact ? 138f : 78f);
            float statY = compact ? CompactStatY : ExpandedStatY;
            float statStep = compact ? CompactStatStep : ExpandedStatStep;
            Rect stats = new Rect(EditorSideInset, statY, 242f, statStep * 3f + 30f);
            float skillY = compact ? CompactSkillY : ExpandedSkillY;
            float skillWidth = skillCount <= 0 ? 1f : (skillCount - 1) * 78f + 72f;
            Rect skills = new Rect(EditorSideInset, skillY, skillWidth, 30f);
            float minimumNoteY = Mathf.Max(localDetails.yMax + 2f, skills.yMax + 6f);
            float noteY = Mathf.Max(editor.height - 106f, minimumNoteY);
            Rect note = new Rect(
                EditorSideInset,
                noteY,
                Mathf.Max(1f, editor.width - EditorSideInset * 2f),
                Mathf.Max(1f, editor.height - noteY - 28f));
            return new PartySetupEditorFlowGeometry(compact, identity, stats, skills, localDetails, note);
        }

        public static float StatRowY(bool compact, int index)
        {
            return (compact ? CompactStatY : ExpandedStatY) + Mathf.Max(0, index) * (compact ? CompactStatStep : ExpandedStatStep);
        }
    }

    /// <summary>An illustrated character folio with direct ancestry and profession selection.</summary>
    public sealed class PartySetupScreen : MonoBehaviour
    {
        private const float DesignWidth = 1280f;
        private const float DesignHeight = 720f;
        private static readonly Color Ink = Hex("382a20");
        private static readonly Color MutedInk = Hex("69513a");
        private static readonly Color Cream = Hex("f0dfb9");
        private static readonly Color Gold = Hex("caa66b");
        private static readonly Color Bronze = Hex("765435");
        private readonly List<Button> rosterButtons = new List<Button>();
        private readonly List<RawImage> rosterPortraits = new List<RawImage>();
        private readonly List<Text> rosterNames = new List<Text>();
        private readonly List<Text> rosterSubtitles = new List<Text>();
        private readonly List<Text> rosterMarkers = new List<Text>();
        private readonly List<Image> rosterSwatches = new List<Image>();
        private readonly List<Button> raceButtons = new List<Button>();
        private readonly List<RawImage> racePortraits = new List<RawImage>();
        private readonly List<Text> raceMarkers = new List<Text>();
        private readonly List<Button> classButtons = new List<Button>();
        private readonly List<RawImage> classPortraits = new List<RawImage>();
        private readonly List<Text> classMarkers = new List<Text>();
        private readonly List<Text> statValues = new List<Text>();
        private readonly List<Button> statDownButtons = new List<Button>();
        private readonly List<Button> statUpButtons = new List<Button>();
        private readonly List<Button> skillButtons = new List<Button>();
        private PartySetupScreenBindings bindings;
        private Canvas canvas;
        private EventSystem ensuredEventSystem;
        private RectTransform folio;
        private RectTransform editorPanel;
        private RectTransform identityPage;
        private RectTransform detailsPage;
        private RectTransform rosterPanel;
        private RawImage selectedPortrait;
        private RawImage workshopBackdrop;
        private Image backdropShade;
        private PartySetupFolioEffects folioEffects;
        private bool triedWorkshopBackdrop;
        private Text summaryText;
        private Text selectedRaceClass;
        private Text selectedRole;
        private Text selectedMemberTitle;
        private Text raceDescription;
        private Text classDescription;
        private Text budgetText;
        private Text detailsBody;
        private Text noteText;
        private InputField nameField;
        private Button identityTab;
        private Button detailsTab;
        private Button originButton;
        private Button sigilButton;
        private Button colorButton;
        private Button beginButton;
        private Font font;
        private float lastWidth = -1f;
        private float lastHeight = -1f;
        private int lastSelected = -1;
        private bool showingDetails;
        private bool suppressNameEvent;
        private bool focusOnNextRefresh;

        public Canvas ViewCanvas => canvas;
        // Edit-mode captures invoke lifecycle methods without Unity registering EventSystem.current.
        private EventSystem NavigationSystem => EventSystem.current ?? (!Application.isPlaying ? ensuredEventSystem : null);

        public void Bind(PartySetupScreenBindings screenBindings)
        {
            bindings = screenBindings;
            if (canvas == null)
            {
                Build();
                // The folio is prepared during title-screen startup; load its paintings on the first visible refresh.
                SetVisible(false);
            }
            else if (canvas.gameObject.activeSelf) Refresh();
        }

        public void SetVisible(bool visible)
        {
            if (canvas == null || canvas.gameObject.activeSelf == visible) return;
            if (visible) ensuredEventSystem = UiRuntime.EnsureEventSystemReady();
            EventSystem navigation = NavigationSystem;
            if (!visible && navigation != null)
            {
                GameObject focused = navigation.currentSelectedGameObject;
                if (focused != null && focused.transform.IsChildOf(canvas.transform))
                    navigation.SetSelectedGameObject(null);
            }
            canvas.gameObject.SetActive(visible);
            if (folioEffects != null) folioEffects.SetVisible(visible);
            focusOnNextRefresh = visible;
        }

        public void ApplyCaptureLayout(float width, float height)
        {
            ApplyLayout(width, height);
        }

        public void AdvancePresentation(float deltaSeconds)
        {
            folioEffects?.Advance(deltaSeconds);
        }

        public PartySetupFolioSnapshot CaptureMotionSnapshot()
        {
            return folioEffects == null ? default : folioEffects.Snapshot;
        }

        public void Refresh()
        {
            if (bindings == null || canvas == null) return;
            if (!Mathf.Approximately(lastWidth, Screen.width) || !Mathf.Approximately(lastHeight, Screen.height))
                ApplyLayout(Screen.width, Screen.height);
            EnsureWorkshopBackdrop();
            IReadOnlyList<PartySetupMemberView> members = bindings.Members?.Invoke() ?? Array.Empty<PartySetupMemberView>();
            int selected = Mathf.Clamp(bindings.SelectedIndex?.Invoke() ?? 0, 0, Mathf.Max(0, members.Count - 1));
            EnsureRosterRows(members.Count);
            for (int i = 0; i < members.Count; i++)
            {
                PartySetupMemberView member = members[i];
                SetButtonSelected(rosterButtons[i], i == selected, false);
                rosterNames[i].text = member.Name;
                rosterSubtitles[i].text = member.RaceClassLine;
                rosterMarkers[i].text = i == selected ? "EDITING CHARACTER " + (i + 1) : "CHARACTER " + (i + 1);
                rosterMarkers[i].color = i == selected ? Gold : Hex("b6a181");
                rosterSwatches[i].color = ParseColor(member.ColorHex, Gold);
                SetPortrait(rosterPortraits[i], member.RaceKey, member.ClassKey);
            }
            summaryText.text = bindings.SummaryLine?.Invoke() ?? bindings.Subtitle ?? "Choose four companions. Shape their stories.";
            bool canBegin = bindings.CanBegin?.Invoke() ?? HasAssignedAttributes(members);
            beginButton.interactable = canBegin;
            if (!canBegin) summaryText.text = "Assign all attribute points before setting out. Open Attributes & details for each unfinished companion.";
            PartySetupMemberView current = bindings.SelectedMember?.Invoke();
            foreach (Selectable control in editorPanel.GetComponentsInChildren<Selectable>(true))
                control.interactable = current != null;
            if (current == null) return;
            if (!nameField.isFocused || lastSelected != selected)
            {
                suppressNameEvent = true;
                nameField.SetTextWithoutNotify(current.Name ?? "");
                suppressNameEvent = false;
            }
            lastSelected = selected;
            selectedMemberTitle.text = "CHARACTER " + (selected + 1) + "  /  " + members.Count;
            selectedRaceClass.text = current.RaceClassLine;
            selectedRole.text = FormatRoleLine(current.RoleLine);
            raceDescription.text = current.RaceDescription;
            classDescription.text = current.ClassDescription;
            SetPortrait(selectedPortrait, current.RaceKey, current.ClassKey);
            RefreshChoices(bindings.RaceChoices, raceButtons, racePortraits, raceMarkers, current.RaceKey, current.ClassKey, true);
            RefreshChoices(bindings.ClassChoices, classButtons, classPortraits, classMarkers, current.ClassKey, current.RaceKey, false);
            int[] stats = { current.Strength, current.Intelligence, current.Agility, current.Health };
            for (int i = 0; i < statValues.Count; i++)
            {
                statValues[i].text = stats[i].ToString();
                statUpButtons[i].interactable = current.CanIncreaseStats;
                statDownButtons[i].interactable = current.CanDecreaseStats == null || i >= current.CanDecreaseStats.Length || current.CanDecreaseStats[i];
            }
            for (int i = 0; i < skillButtons.Count; i++)
                skillButtons[i].interactable = current.CanBoostTalents && (current.CanBoostTalentByIndex == null || i >= current.CanBoostTalentByIndex.Length || current.CanBoostTalentByIndex[i]);
            int remaining = Mathf.Max(0, current.StatCap - current.StatTotal);
            budgetText.text = current.StatTotal + " / " + current.StatCap + " assigned  ·  " + remaining + " points available";
            SetButtonLabel(originButton, "Origin: " + current.Origin + "   >");
            SetButtonLabel(sigilButton, "Sigil: " + current.Sigil + "   >");
            SetButtonLabel(colorButton, "Change heraldic colour   >");
            detailsBody.text = current.GearLine + "\n\n" + current.BestSkillLine + "\n" + current.ProgressLine + "\n" + current.UnlockLine;
            noteText.text = bindings.WeaknessLine?.Invoke() ?? "";
            RefreshTabs();
            folioEffects?.SetSelection(selected < rosterButtons.Count ? rosterButtons[selected] : null,
                ChoiceButton(bindings.RaceChoices, raceButtons, current.RaceKey),
                ChoiceButton(bindings.ClassChoices, classButtons, current.ClassKey), showingDetails ? detailsTab : identityTab);
            folioEffects?.Advance(0f);
            EventSystem navigation = NavigationSystem;
            if (focusOnNextRefresh && canvas.gameObject.activeInHierarchy && navigation != null && selected < rosterButtons.Count)
            {
                navigation.SetSelectedGameObject(rosterButtons[selected].gameObject);
                focusOnNextRefresh = false;
            }
        }

        private void Build()
        {
            ensuredEventSystem = UiRuntime.EnsureEventSystemReady();
            font = UiRuntime.DefaultFont;
            canvas = UiRuntime.CreateOwnedRootCanvas(this, "Party Setup Canvas");
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            canvas.pixelPerfect = true;
            CanvasScaler scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(DesignWidth, DesignHeight);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            canvas.gameObject.AddComponent<GraphicRaycaster>();
            Stretch(AddImage("Backdrop Base", canvas.transform, Hex("161310")).rectTransform);
            workshopBackdrop = AddRawImage("Character Workshop Backdrop", canvas.transform);
            workshopBackdrop.texture = bindings?.BackdropArt;
            workshopBackdrop.enabled = workshopBackdrop.texture != null;
            Stretch(workshopBackdrop.rectTransform);
            backdropShade = AddImage("Muster Shade", canvas.transform, new Color(0.035f, 0.025f, 0.015f, 0.78f));
            Stretch(backdropShade.rectTransform);
            folio = NewRect("Character Folio", canvas.transform);
            folio.anchorMin = folio.anchorMax = folio.pivot = new Vector2(0.5f, 0.5f);
            folio.anchoredPosition = Vector2.zero;
            folio.sizeDelta = new Vector2(DesignWidth, DesignHeight);

            Text title = PlaceText("Title", folio, "Gather your fellowship", 27, Cream, new Rect(26, 13, 690, 37));
            title.font = UiRuntime.TitleFont;
            PlaceText("Subtitle", folio, "THE TAVERN MUSTER   /   CHOOSE A COMPANION TO CUSTOMIZE", 12, Gold, new Rect(28, 54, 750, 20));
            PlaceButton("Tavern", folio, "Tavern", () => bindings?.BackToTavern?.Invoke(), new Rect(873, 25, 92, 37));
            PlaceButton("Quick Start", folio, "Quick start", () => bindings?.QuickStart?.Invoke(), new Rect(975, 25, 114, 37));
            beginButton = PlaceButton("Begin", folio, "Begin adventure", () => bindings?.Begin?.Invoke(), new Rect(1099, 25, 155, 37), true);
            rosterPanel = NewRect("Roster", folio);
            SetLocalRect(rosterPanel, new Rect(26, 87, 1228, 104));
            editorPanel = AddPanel("Editor", folio, Hex("231d18"), Bronze);
            SetLocalRect(editorPanel, new Rect(26, 206, 1228, 471));
            selectedMemberTitle = PlaceText("Selected Member", editorPanel, "", 12, Gold, new Rect(16, 8, 284, 20));
            RectTransform portraitFrame = AddPanel("Portrait Frame", editorPanel, Hex("120f0d"), Gold);
            SetLocalRect(portraitFrame, new Rect(16, 34, 276, 276));
            PlaceText("Portrait Fallback", portraitFrame, "ASHEN\nHALLS", 29, Bronze, new Rect(25, 84, 226, 90), TextAnchor.MiddleCenter);
            selectedPortrait = AddRawImage("Selected Portrait", portraitFrame);
            SetLocalRect(selectedPortrait.rectTransform, new Rect(3, 3, 270, 270));
            PlaceText("Name Label", editorPanel, "CHARACTER NAME", 12, Gold, new Rect(16, 319, 276, 19));
            nameField = AddInput("Name Field", editorPanel);
            SetLocalRect(nameField.GetComponent<RectTransform>(), new Rect(16, 342, 214, 36));
            nameField.onEndEdit.AddListener(value =>
            {
                UiRuntime.NotifyTextInputEnded();
                if (!suppressNameEvent) bindings?.SetName?.Invoke(value);
                Refresh();
            });
            PlaceButton("Random Name", editorPanel, "Roll", () => bindings?.RandomName?.Invoke(), new Rect(238, 342, 54, 36));
            selectedRaceClass = PlaceText("Selected Race Class", editorPanel, "", 17, Cream, new Rect(16, 389, 278, 27));
            selectedRaceClass.font = UiRuntime.DialogueEmphasisFont;
            selectedRole = PlaceText("Selected Role", editorPanel, "", 13, Hex("c1af8c"), new Rect(16, 423, 276, 37));
            selectedRole.resizeTextForBestFit = true;
            selectedRole.resizeTextMinSize = 12;
            selectedRole.resizeTextMaxSize = 13;

            RectTransform sheet = AddPanel("Character Sheet", editorPanel, Hex("d7c39b"), Gold);
            SetLocalRect(sheet, new Rect(309, 12, 907, 447));
            identityTab = PlaceButton("Identity Tab", sheet, "Race & class", () => ChangeTab(false), new Rect(15, 12, 189, 34), true);
            detailsTab = PlaceButton("Details Tab", sheet, "Attributes & details", () => ChangeTab(true), new Rect(213, 12, 222, 34));
            PlaceText("Folio Hint", sheet, "Every race. Every calling. A different story.", 13, MutedInk, new Rect(448, 14, 440, 29), TextAnchor.MiddleRight);
            identityPage = NewRect("Identity Page", sheet);
            SetLocalRect(identityPage, new Rect(15, 55, 877, 382));
            detailsPage = NewRect("Details Page", sheet);
            SetLocalRect(detailsPage, new Rect(15, 55, 877, 382));
            BuildIdentityPage();
            BuildDetailsPage();
            summaryText = PlaceText("Party Summary", folio, "", 12, Gold, new Rect(27, 687, 1226, 22), TextAnchor.MiddleCenter);
            RectTransform ambience = NewRect("Living Folio Effects", folio);
            SetLocalRect(ambience, new Rect(0, 0, DesignWidth, DesignHeight));
            ambience.SetAsFirstSibling();
            folioEffects = ambience.gameObject.AddComponent<PartySetupFolioEffects>();
            folioEffects.Initialize(selectedPortrait, () => bindings?.ReducedMotion?.Invoke() ?? false);
            foreach (Button button in canvas.GetComponentsInChildren<Button>(true)) folioEffects.RegisterButton(button);
        }

        private void ChangeTab(bool details)
        {
            if (showingDetails == details) return;
            showingDetails = details;
            if (canvas.gameObject.activeInHierarchy) bindings?.TabChanged?.Invoke(details);
        }

        private void EnsureWorkshopBackdrop()
        {
            if (triedWorkshopBackdrop || !canvas.gameObject.activeInHierarchy) return;
            triedWorkshopBackdrop = true;
            Texture2D art = bindings.WorkshopBackdropArt?.Invoke();
            if (art == null) return;
            workshopBackdrop.texture = art;
            workshopBackdrop.enabled = true;
            backdropShade.color = new Color(0.035f, 0.025f, 0.015f, 0.54f);
            ApplyBackdropCover(lastWidth, lastHeight);
        }

        private void ApplyBackdropCover(float width, float height)
        {
            if (workshopBackdrop == null || workshopBackdrop.texture == null || width <= 0f || height <= 0f) return;
            float textureAspect = (float)workshopBackdrop.texture.width / workshopBackdrop.texture.height;
            float screenAspect = width / height;
            if (textureAspect > screenAspect)
            {
                float crop = screenAspect / textureAspect;
                workshopBackdrop.uvRect = new Rect((1f - crop) * 0.5f, 0f, crop, 1f);
            }
            else
            {
                float crop = textureAspect / screenAspect;
                workshopBackdrop.uvRect = new Rect(0f, (1f - crop) * 0.5f, 1f, crop);
            }
        }

        private static string FormatRoleLine(string line)
        {
            if (string.IsNullOrEmpty(line) || line.Length <= 40) return line;
            int first = line.IndexOf(" / ", StringComparison.Ordinal);
            int second = first < 0 ? -1 : line.IndexOf(" / ", first + 3, StringComparison.Ordinal);
            return second < 0 ? line : line.Substring(0, second) + "\n" + line.Substring(second + 3);
        }

        private static Button ChoiceButton(IReadOnlyList<PartySetupChoiceView> choices, List<Button> buttons, string key)
        {
            if (choices == null) return null;
            for (int i = 0; i < choices.Count && i < buttons.Count; i++)
                if (string.Equals(choices[i].Key, key, StringComparison.OrdinalIgnoreCase)) return buttons[i];
            return null;
        }

        private void BuildIdentityPage()
        {
            PlaceText("Race Heading", identityPage, "1   CHOOSE A RACE", 14, Ink, new Rect(0, 0, 480, 22));
            IReadOnlyList<PartySetupChoiceView> races = bindings.RaceChoices ?? Array.Empty<PartySetupChoiceView>();
            float raceWidth = (877f - Mathf.Max(0, races.Count - 1) * 7f) / Mathf.Max(1, races.Count);
            for (int i = 0; i < races.Count; i++)
            {
                PartySetupChoiceView choice = races[i];
                Button button = PlaceButton("Race " + choice.Key, identityPage, "", () => bindings?.SetRace?.Invoke(choice.Key), new Rect(i * (raceWidth + 7), 29, raceWidth, 55), false, true);
                RawImage portrait = AddRawImage("Race Portrait", button.transform);
                SetLocalRect(portrait.rectTransform, new Rect(4, 4, 47, 47));
                Text label = PlaceText("Race Name", button.transform, choice.Name, 15, Ink, new Rect(58, 3, raceWidth - 64, 28));
                label.fontStyle = FontStyle.Bold;
                Text marker = PlaceText("Race Selection", button.transform, "", 10, MutedInk, new Rect(58, 30, raceWidth - 63, 20));
                raceButtons.Add(button);
                racePortraits.Add(portrait);
                raceMarkers.Add(marker);
            }
            raceDescription = PlaceText("Race Description", identityPage, "", 13, MutedInk, new Rect(1, 93, 876, 32));
            AddRule(identityPage, new Rect(0, 130, 877, 1));
            PlaceText("Class Heading", identityPage, "2   CHOOSE A CLASS", 14, Ink, new Rect(0, 139, 480, 22));
            IReadOnlyList<PartySetupChoiceView> classes = bindings.ClassChoices ?? Array.Empty<PartySetupChoiceView>();
            const float cardWidth = 214f;
            for (int i = 0; i < classes.Count; i++)
            {
                PartySetupChoiceView choice = classes[i];
                Button button = PlaceButton("Class " + choice.Key, identityPage, "", () => bindings?.SetClass?.Invoke(choice.Key), new Rect((i % 4) * 221f, 169f + (i / 4) * 84f, cardWidth, 77f), false, true);
                RawImage portrait = AddRawImage("Class Portrait", button.transform);
                SetLocalRect(portrait.rectTransform, new Rect(3, 3, 71, 71));
                Text label = PlaceText("Class Name", button.transform, choice.Name, 16, Ink, new Rect(83, 7, 125, 25));
                label.fontStyle = FontStyle.Bold;
                PlaceText("Class Role", button.transform, ClassRole(choice.Key), 11, MutedInk, new Rect(83, 32, 123, 20));
                Text marker = PlaceText("Class Selection", button.transform, "", 10, MutedInk, new Rect(83, 53, 123, 18));
                classButtons.Add(button);
                classPortraits.Add(portrait);
                classMarkers.Add(marker);
            }
            classDescription = PlaceText("Class Description", identityPage, "", 13, MutedInk, new Rect(1, 343, 876, 36));
        }

        private void BuildDetailsPage()
        {
            PlaceText("Attributes Heading", detailsPage, "ATTRIBUTES", 15, Ink, new Rect(0, 0, 340, 24));
            budgetText = PlaceText("Attribute Budget", detailsPage, "", 13, MutedInk, new Rect(0, 29, 390, 23));
            string[] names = { "Strength", "Intelligence", "Agility", "Health" };
            string[] uses = { "Melee & carrying", "Spells & mana", "Accuracy & evasion", "Life & endurance" };
            for (int i = 0; i < names.Length; i++)
            {
                int statCode = -1 - i;
                float y = 61 + i * 41;
                PlaceText("Stat " + names[i], detailsPage, names[i], 15, Ink, new Rect(0, y, 140, 23));
                PlaceText("Stat Use " + i, detailsPage, uses[i], 10, MutedInk, new Rect(0, y + 21, 157, 16));
                statDownButtons.Add(PlaceButton("Stat Down " + i, detailsPage, "-", () => bindings?.ChangeStat?.Invoke(statCode, -1), new Rect(166, y, 34, 32), false, true));
                statValues.Add(PlaceText("Stat Value " + i, detailsPage, "0", 17, Ink, new Rect(204, y, 53, 32), TextAnchor.MiddleCenter));
                statUpButtons.Add(PlaceButton("Stat Up " + i, detailsPage, "+", () => bindings?.ChangeStat?.Invoke(statCode, 1), new Rect(261, y, 34, 32), false, true));
            }
            PlaceText("Training Heading", detailsPage, "TRAINING", 15, Ink, new Rect(0, 233, 340, 23));
            PlaceText("Training Hint", detailsPage, "Improve a talent. Each has a training limit.", 12, MutedInk, new Rect(0, 259, 393, 24));
            string[] talentNames = { "Arms", "Missile", "Mend", "Ember", "Hex", "Guard" };
            for (int i = 0; i < PartySetupScreenLayout.TalentKeys.Count; i++)
            {
                string key = PartySetupScreenLayout.TalentKeys[i];
                skillButtons.Add(PlaceButton("Skill " + key, detailsPage, "+ " + talentNames[i], () => bindings?.BoostTalent?.Invoke(key), new Rect((i % 3) * 122, 290 + (i / 3) * 39, 114, 32), false, true));
            }
            AddRule(detailsPage, new Rect(394, 0, 1, 367));
            PlaceText("Background Heading", detailsPage, "BACKGROUND & APPEARANCE", 15, Ink, new Rect(418, 0, 455, 24));
            originButton = PlaceButton("Origin", detailsPage, "Origin", () => bindings?.CycleOrigin?.Invoke(), new Rect(418, 35, 455, 34), false, true);
            sigilButton = PlaceButton("Sigil", detailsPage, "Sigil", () => bindings?.CycleSigil?.Invoke(), new Rect(418, 77, 455, 34), false, true);
            colorButton = PlaceButton("Color", detailsPage, "Colour", () => bindings?.CycleColor?.Invoke(), new Rect(418, 119, 248, 34), false, true);
            PlaceButton("Reroll Look", detailsPage, "Randomize look", () => bindings?.RerollLook?.Invoke(), new Rect(674, 119, 199, 34), false, true);
            PlaceText("Equipment Heading", detailsPage, "EQUIPMENT & TALENTS", 15, Ink, new Rect(418, 170, 283, 26));
            PlaceButton("Reroll Gear", detailsPage, "Reroll gear", () => bindings?.RerollGear?.Invoke(), new Rect(749, 168, 124, 31), false, true);
            detailsBody = PlaceText("Details Body", detailsPage, "", 12, MutedInk, new Rect(418, 210, 455, 124), TextAnchor.UpperLeft);
            noteText = PlaceText("Party Advice", detailsPage, "", 11, MutedInk, new Rect(418, 343, 455, 36), TextAnchor.UpperLeft);
        }

        private void EnsureRosterRows(int count)
        {
            while (rosterButtons.Count < count)
            {
                int index = rosterButtons.Count;
                Button button = PlaceButton("Roster " + index, rosterPanel, "", () => bindings?.SelectMember?.Invoke(index), new Rect());
                RawImage portrait = AddRawImage("Roster Portrait " + index, button.transform);
                Text name = PlaceText("Roster Name " + index, button.transform, "", 18, Cream, new Rect());
                name.font = UiRuntime.DialogueEmphasisFont;
                Text subtitle = PlaceText("Roster Subtitle " + index, button.transform, "", 12, Hex("ccba98"), new Rect());
                Text marker = PlaceText("Roster Selection " + index, button.transform, "", 10, Gold, new Rect());
                Image swatch = AddImage("Heraldic Colour " + index, button.transform, Gold);
                swatch.raycastTarget = false;
                rosterButtons.Add(button);
                rosterPortraits.Add(portrait);
                rosterNames.Add(name);
                rosterSubtitles.Add(subtitle);
                rosterMarkers.Add(marker);
                rosterSwatches.Add(swatch);
            }
            float width = (1228f - Mathf.Max(0, count - 1) * 10f) / Mathf.Max(1, count);
            for (int i = 0; i < rosterButtons.Count; i++)
            {
                rosterButtons[i].gameObject.SetActive(i < count);
                SetLocalRect(rosterButtons[i].GetComponent<RectTransform>(), new Rect(i * (width + 10f), 0, width, 104));
                SetLocalRect(rosterPortraits[i].rectTransform, new Rect(5, 5, 94, 94));
                SetLocalRect(rosterNames[i].rectTransform, new Rect(110, 28, width - 120, 30));
                SetLocalRect(rosterSubtitles[i].rectTransform, new Rect(110, 62, width - 120, 30));
                SetLocalRect(rosterMarkers[i].rectTransform, new Rect(110, 7, width - 120, 20));
                SetLocalRect(rosterSwatches[i].rectTransform, new Rect(width - 4, 4, 2, 96));
            }
        }

        private void RefreshChoices(IReadOnlyList<PartySetupChoiceView> choices, List<Button> buttons, List<RawImage> portraits, List<Text> markers, string selected, string other, bool isRace)
        {
            if (choices == null) return;
            for (int i = 0; i < choices.Count && i < buttons.Count; i++)
            {
                bool chosen = string.Equals(choices[i].Key, selected, StringComparison.OrdinalIgnoreCase);
                SetButtonSelected(buttons[i], chosen, true);
                markers[i].text = chosen ? "SELECTED" : (isRace ? "CHOOSE RACE" : "CHOOSE CLASS");
                markers[i].fontStyle = chosen ? FontStyle.Bold : FontStyle.Normal;
                SetPortrait(portraits[i], isRace ? choices[i].Key : other, isRace ? other : choices[i].Key);
            }
        }

        private void RefreshTabs()
        {
            EventSystem navigation = NavigationSystem;
            GameObject focused = navigation == null ? null : navigation.currentSelectedGameObject;
            RectTransform pageToHide = showingDetails ? identityPage : detailsPage;
            bool focusWasOnHiddenPage = focused != null && focused.transform.IsChildOf(pageToHide);
            identityPage.gameObject.SetActive(!showingDetails);
            detailsPage.gameObject.SetActive(showingDetails);
            SetButtonSelected(identityTab, !showingDetails, false);
            SetButtonSelected(detailsTab, showingDetails, false);
            if (focusWasOnHiddenPage && canvas.gameObject.activeInHierarchy && navigation != null)
                navigation.SetSelectedGameObject((showingDetails ? detailsTab : identityTab).gameObject);
        }

        private static bool HasAssignedAttributes(IReadOnlyList<PartySetupMemberView> members)
        {
            if (members.Count == 0) return false;
            for (int i = 0; i < members.Count; i++)
                if (members[i] == null || members[i].StatTotal != members[i].StatCap) return false;
            return true;
        }

        private void SetPortrait(RawImage image, string race, string characterClass)
        {
            Texture2D atlas = bindings.PortraitAtlas?.Invoke(race, characterClass);
            int cell = Mathf.Clamp(bindings.PortraitCell?.Invoke(race, characterClass) ?? 0, 0, 7);
            // Cells are authored in reading order. Half-texel inset keeps neighbouring cells out of the frame.
            float dx = atlas == null ? 0f : 0.5f / atlas.width;
            float dy = atlas == null ? 0f : 0.5f / atlas.height;
            Rect uv = new Rect((cell % 4) * 0.25f + dx, 1f - ((cell / 4) + 1) * 0.5f + dy, 0.25f - 2f * dx, 0.5f - 2f * dy);
            if (image == selectedPortrait && folioEffects != null)
                folioEffects.ChoosePortrait(atlas, uv, lastSelected, race, characterClass);
            else
            {
                image.texture = atlas;
                image.enabled = atlas != null;
                image.uvRect = uv;
            }
        }

        private void ApplyLayout(float width, float height)
        {
            lastWidth = width;
            lastHeight = height;
            ApplyBackdropCover(width, height);
            if (folio == null) return;
            folio.anchoredPosition = Vector2.zero;
            folio.sizeDelta = new Vector2(DesignWidth, DesignHeight);
        }

        private static string ClassRole(string key)
        {
            switch ((key ?? "").ToLowerInvariant())
            {
                case "rogue": return "Blades & cunning";
                case "warrior": return "Arms & endurance";
                case "ranger": return "Bows & wildcraft";
                case "wizard": return "Arcane mastery";
                case "mage": return "Elemental power";
                case "warlock": return "Hexes & shadows";
                case "priest": return "Healing & faith";
                case "paladin": return "Steel & devotion";
                default: return "Choose your calling";
            }
        }

        private InputField AddInput(string name, Transform parent)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(InputField));
            go.transform.SetParent(parent, false);
            go.GetComponent<Image>().color = Hex("100e0b");
            Outline outline = go.AddComponent<Outline>();
            outline.effectColor = Bronze;
            outline.effectDistance = new Vector2(1f, -1f);
            InputField field = go.GetComponent<InputField>();
            Text text = AddText("Text", go.transform, "", 17, Cream, TextAnchor.MiddleLeft);
            Stretch(text.rectTransform, 9f, 3f);
            field.textComponent = text;
            field.characterLimit = 16;
            field.lineType = InputField.LineType.SingleLine;
            field.selectionColor = new Color(0.72f, 0.51f, 0.23f, 0.65f);
            return field;
        }

        private Button PlaceButton(string name, Transform parent, string label, Action action, Rect area, bool hero = false, bool paper = false)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            Image image = go.GetComponent<Image>();
            image.color = Color.white;
            Button button = go.GetComponent<Button>();
            button.targetGraphic = image;
            Outline outline = go.AddComponent<Outline>();
            outline.effectDistance = new Vector2(1f, -1f);
            outline.effectColor = hero ? Gold : Bronze;
            SetButtonSelected(button, hero, paper);
            if (action != null) button.onClick.AddListener(() => { action(); Refresh(); });
            Text text = AddText("Label", go.transform, label, 14, paper ? Ink : Cream, TextAnchor.MiddleCenter);
            text.fontStyle = FontStyle.Bold;
            Stretch(text.rectTransform, 6f, 3f);
            SetLocalRect(go.GetComponent<RectTransform>(), area);
            folioEffects?.RegisterButton(button);
            return button;
        }

        private static void SetButtonSelected(Button button, bool selected, bool paper)
        {
            ColorBlock colors = button.colors;
            colors.normalColor = paper ? (selected ? Hex("e5c283") : Hex("e1cfab")) : (selected ? Hex("634329") : Hex("302820"));
            colors.highlightedColor = paper ? Hex("f3dfb5") : Hex("755438");
            colors.pressedColor = paper ? Hex("c09d65") : Hex("4a301c");
            colors.selectedColor = colors.highlightedColor;
            colors.disabledColor = paper ? Hex("b3a78e") : Hex("51493e");
            colors.colorMultiplier = 1f;
            button.colors = colors;
            Outline outline = button.GetComponent<Outline>();
            if (outline != null)
            {
                outline.effectColor = selected ? (paper ? Hex("8a5425") : Gold) : Bronze;
                outline.effectDistance = selected ? new Vector2(2f, -2f) : new Vector2(1f, -1f);
            }
        }

        private static void SetButtonLabel(Button button, string value)
        {
            Text label = button.transform.Find("Label").GetComponent<Text>();
            label.text = value;
        }

        private Text PlaceText(string name, Transform parent, string value, int size, Color color, Rect area, TextAnchor anchor = TextAnchor.MiddleLeft)
        {
            Text text = AddText(name, parent, value, size, color, anchor);
            SetLocalRect(text.rectTransform, area);
            return text;
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
            text.raycastTarget = false;
            return text;
        }

        private RectTransform AddPanel(string name, Transform parent, Color fill, Color border)
        {
            RectTransform panel = AddImage(name, parent, fill).rectTransform;
            Outline outline = panel.gameObject.AddComponent<Outline>();
            outline.effectColor = border;
            outline.effectDistance = new Vector2(1f, -1f);
            return panel;
        }

        private static RectTransform NewRect(string name, Transform parent)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go.GetComponent<RectTransform>();
        }

        private static Image AddImage(string name, Transform parent, Color color)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(parent, false);
            Image image = go.GetComponent<Image>();
            image.color = color;
            return image;
        }

        private static RawImage AddRawImage(string name, Transform parent)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
            go.transform.SetParent(parent, false);
            RawImage image = go.GetComponent<RawImage>();
            image.raycastTarget = false;
            image.color = Color.white;
            return image;
        }

        private static void AddRule(Transform parent, Rect area)
        {
            Image rule = AddImage("Folio Rule", parent, Hex("ad9067"));
            rule.raycastTarget = false;
            SetLocalRect(rule.rectTransform, area);
        }

        private static void SetLocalRect(RectTransform rect, Rect area)
        {
            rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
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

        private static Color ParseColor(string hex, Color fallback)
        {
            if (string.IsNullOrWhiteSpace(hex)) return fallback;
            return ColorUtility.TryParseHtmlString(hex.StartsWith("#") ? hex : "#" + hex, out Color value) ? value : fallback;
        }

        private static Color Hex(string hex)
        {
            return ParseColor(hex, Color.white);
        }
    }
}
