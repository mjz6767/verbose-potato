using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AshenHalls
{
    public sealed class PartySetupMemberView
    {
        public string Name;
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
    }

    public sealed class PartySetupScreenBindings
    {
        public string Title;
        public string Subtitle;
        public Texture2D BackdropArt;
        public Func<string> SummaryLine;
        public Func<string> WeaknessLine;
        public Func<IReadOnlyList<PartySetupMemberView>> Members;
        public Func<int> SelectedIndex;
        public Func<PartySetupMemberView> SelectedMember;
        public Action<int> SelectMember;
        public Action Begin;
        public Action QuickStart;
        public Action BackToTavern;
        public Action<string> SetName;
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

    public sealed class PartySetupScreen : MonoBehaviour
    {
        private readonly List<Button> rosterButtons = new List<Button>();
        private readonly List<Text> rosterNames = new List<Text>();
        private readonly List<Text> rosterSubtitles = new List<Text>();
        private readonly List<Image> rosterSwatches = new List<Image>();
        private readonly List<Text> statValues = new List<Text>();
        private readonly List<Button> skillButtons = new List<Button>();
        private PartySetupScreenBindings bindings;
        private Canvas canvas;
        private RectTransform topPanel;
        private RectTransform rosterPanel;
        private RectTransform editorPanel;
        private RectTransform detailsPanel;
        private Text titleText;
        private Text summaryText;
        private Text rosterTitle;
        private Text editorTitle;
        private Text editorHint;
        private Text selectedNameLabel;
        private Text selectedRaceClass;
        private Text detailsName;
        private Text detailsBody;
        private Text noteText;
        private InputField nameField;
        private Button tavernButton;
        private Button quickStartButton;
        private Button beginButton;
        private Button classButton;
        private Button raceButton;
        private Button originButton;
        private Button sigilButton;
        private Button randomNameButton;
        private Button rerollGearButton;
        private Button rerollLookButton;
        private Button colorButton;
        private Font font;
        private float lastWidth = -1f;
        private float lastHeight = -1f;
        private int lastMemberCount = -1;
        private bool suppressNameEvent;

        public void Bind(PartySetupScreenBindings screenBindings)
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
            IReadOnlyList<PartySetupMemberView> members = bindings.Members == null ? Array.Empty<PartySetupMemberView>() : bindings.Members();
            if (!Mathf.Approximately(lastWidth, Screen.width) || !Mathf.Approximately(lastHeight, Screen.height) || lastMemberCount != members.Count)
            {
                ApplyLayout(members.Count);
            }

            titleText.text = bindings.Title ?? VersionInfo.ProductName;
            summaryText.text = bindings.SummaryLine == null ? "" : bindings.SummaryLine();
            int selected = Mathf.Clamp(bindings.SelectedIndex == null ? 0 : bindings.SelectedIndex(), 0, Mathf.Max(0, members.Count - 1));
            EnsureRosterRows(members.Count);
            for (int i = 0; i < members.Count; i++)
            {
                PartySetupMemberView member = members[i];
                rosterButtons[i].targetGraphic.GetComponent<Image>().color = i == selected ? Hex("2d3438", 1f) : Hex("20272e", 1f);
                rosterNames[i].text = member.Name;
                rosterSubtitles[i].text = member.RaceClassLine + " / " + member.BestSkillLine;
                rosterSwatches[i].color = ParseColor(member.ColorHex, Hex("d7a84e", 1f));
            }

            PartySetupMemberView current = bindings.SelectedMember == null ? null : bindings.SelectedMember();
            if (current == null)
            {
                SetEditorEnabled(false);
                return;
            }

            SetEditorEnabled(true);
            suppressNameEvent = true;
            nameField.SetTextWithoutNotify(current.Name ?? "");
            suppressNameEvent = false;
            selectedNameLabel.text = current.Name;
            selectedRaceClass.text = current.RaceClassLine + "\n" + current.RoleLine;
            int[] stats = { current.Strength, current.Intelligence, current.Agility, current.Health };
            for (int i = 0; i < statValues.Count && i < stats.Length; i++) statValues[i].text = stats[i].ToString();
            editorTitle.text = $"Tavern Muster";
            editorHint.text = $"Four-person party setup. Attributes {current.StatTotal}/{current.StatCap}.";
            detailsName.text = current.Name;
            detailsBody.text = current.ProgressLine + "\n" + current.UnlockLine + "\n" + current.GearLine + "\n" + current.BestSkillLine;
            noteText.text = (bindings.WeaknessLine == null ? "" : bindings.WeaknessLine()) + "\n" + current.RoleLine;
        }

        private void Build()
        {
            EnsureEventSystem();
            font = UiRuntime.DefaultFont;
            canvas = UiRuntime.CreateOwnedRootCanvas(this, "Party Setup Canvas");
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            CanvasScaler scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            canvas.gameObject.AddComponent<GraphicRaycaster>();
            Stretch(canvas.GetComponent<RectTransform>());

            Image baseImage = AddImage("Backdrop Base", canvas.transform, Hex("0e1114", 1f));
            Stretch(baseImage.rectTransform);
            if (bindings?.BackdropArt != null)
            {
                Image backdrop = AddImage("Tavern Backdrop", canvas.transform, Color.white);
                Stretch(backdrop.rectTransform);
                backdrop.sprite = Sprite.Create(bindings.BackdropArt, new Rect(0, 0, bindings.BackdropArt.width, bindings.BackdropArt.height), new Vector2(0.5f, 0.5f), 100f);
            }
            Image shade = AddImage("Muster Shade", canvas.transform, Hex("030405", 0.55f));
            Stretch(shade.rectTransform);

            topPanel = AddPanel("Top", canvas.transform, Hex("10161b", 0.90f), Hex("3c4544", 0.70f));
            titleText = AddText("Title", topPanel, VersionInfo.ProductName, 24, Hex("f3ead7", 1f), TextAnchor.MiddleLeft);
            titleText.resizeTextForBestFit = true;
            titleText.resizeTextMinSize = 15;
            titleText.resizeTextMaxSize = 24;
            summaryText = AddText("Summary", topPanel, "", 10, Hex("d7a84e", 1f), TextAnchor.MiddleLeft);
            tavernButton = AddButton("Tavern", topPanel, "Tavern", bindings?.BackToTavern, false);
            quickStartButton = AddButton("Quick Start", topPanel, "Quick Start", bindings?.QuickStart, false);
            beginButton = AddButton("Begin", topPanel, "Begin", bindings?.Begin, true);

            rosterPanel = AddPanel("Roster", canvas.transform, Hex("1a2026", 0.96f), Hex("3c4544", 1f));
            rosterTitle = AddText("Roster Title", rosterPanel, "Party", 21, Hex("f3ead7", 1f), TextAnchor.MiddleLeft);

            editorPanel = AddPanel("Editor", canvas.transform, Hex("1a2026", 0.96f), Hex("3c4544", 1f));
            editorTitle = AddText("Editor Title", editorPanel, "Tavern Muster", 21, Hex("f3ead7", 1f), TextAnchor.MiddleLeft);
            editorHint = AddText("Editor Hint", editorPanel, "", 11, Hex("b7aa90", 1f), TextAnchor.MiddleLeft);
            nameField = AddInput("Name Field", editorPanel);
            nameField.onEndEdit.AddListener(value =>
            {
                UiRuntime.NotifyTextInputEnded();
                if (!suppressNameEvent) bindings?.SetName?.Invoke(value);
            });
            classButton = AddButton("Class", editorPanel, "Class", bindings?.CycleClass, false);
            raceButton = AddButton("Race", editorPanel, "Race", bindings?.CycleRace, false);
            originButton = AddButton("Origin", editorPanel, "Origin", bindings?.CycleOrigin, false);
            sigilButton = AddButton("Sigil", editorPanel, "Sigil", bindings?.CycleSigil, false);
            randomNameButton = AddButton("Name", editorPanel, "Name", bindings?.RandomName, false);
            rerollGearButton = AddButton("Reroll Gear", editorPanel, "Reroll Gear", bindings?.RerollGear, false);
            rerollLookButton = AddButton("Reroll Look", editorPanel, "Reroll Look", bindings?.RerollLook, false);
            colorButton = AddButton("Color", editorPanel, "Color", bindings?.CycleColor, false);
            selectedRaceClass = AddText("Selected Race Class", editorPanel, "", 12, Hex("b7aa90", 1f), TextAnchor.MiddleLeft);
            selectedNameLabel = AddText("Selected Name", editorPanel, "", 20, Hex("d7a84e", 1f), TextAnchor.MiddleLeft);

            string[] statNames = { "Strength", "Intelligence", "Agility", "Health" };
            int[] statCodes = { -1, -2, -3, -4 };
            for (int i = 0; i < statNames.Length; i++)
            {
                int statCode = statCodes[i];
                AddText("Stat " + statNames[i], editorPanel, statNames[i], 13, Hex("f3ead7", 1f), TextAnchor.MiddleLeft);
                AddButton("Stat Down " + i, editorPanel, "-", () => bindings?.ChangeStat?.Invoke(statCode, -1), false);
                statValues.Add(AddText("Stat Value " + i, editorPanel, "0", 13, Hex("f3ead7", 1f), TextAnchor.MiddleCenter));
                AddButton("Stat Up " + i, editorPanel, "+", () => bindings?.ChangeStat?.Invoke(statCode, 1), false);
            }

            for (int i = 0; i < PartySetupScreenLayout.TalentKeys.Count; i++)
            {
                string key = PartySetupScreenLayout.TalentKeys[i];
                skillButtons.Add(AddButton("Skill " + key, editorPanel, key, () => bindings?.BoostTalent?.Invoke(key), false));
            }

            detailsPanel = AddPanel("Details", editorPanel, Hex("11171b", 1f), Hex("3c4544", 1f));
            selectedNameLabel.transform.SetParent(detailsPanel, false);
            detailsName = selectedNameLabel;
            detailsBody = AddText("Details Body", detailsPanel, "", 11, Hex("b7aa90", 1f), TextAnchor.UpperLeft);
            noteText = AddText("Note", editorPanel, "", 11, Hex("b7aa90", 1f), TextAnchor.UpperLeft);
        }

        private void ApplyLayout(int memberCount)
        {
            lastWidth = Screen.width;
            lastHeight = Screen.height;
            lastMemberCount = memberCount;
            PartySetupScreenGeometry geometry = PartySetupScreenLayout.Calculate(Screen.width, Screen.height);
            SetScreenRect(topPanel, geometry.Top);
            SetScreenRect(rosterPanel, geometry.Roster);
            SetScreenRect(editorPanel, geometry.Editor);
            SetLocalRect(titleText.rectTransform, new Rect(16f, 5f, 300f, 27f));
            SetLocalRect(summaryText.rectTransform, new Rect(18f, 34f, Mathf.Max(260f, geometry.Top.width - 760f), 18f));
            SetLocalRect(beginButton.GetComponent<RectTransform>(), new Rect(geometry.Top.width - 140f, 16f, 104f, 34f));
            SetLocalRect(quickStartButton.GetComponent<RectTransform>(), new Rect(geometry.Top.width - 254f, 16f, 106f, 34f));
            SetLocalRect(tavernButton.GetComponent<RectTransform>(), new Rect(geometry.Top.width - 346f, 16f, 82f, 34f));

            SetLocalRect(rosterTitle.rectTransform, new Rect(14f, 12f, geometry.Roster.width - 28f, 25f));
            EnsureRosterRows(memberCount);
            for (int i = 0; i < memberCount; i++)
            {
                Rect row = PartySetupScreenLayout.RosterRow(geometry.Roster, i);
                SetLocalRect(rosterButtons[i].GetComponent<RectTransform>(), row);
                SetLocalRect(rosterSwatches[i].rectTransform, new Rect(8f, 9f, 46f, 46f));
                SetLocalRect(rosterNames[i].rectTransform, new Rect(66f, 8f, row.width - 78f, 22f));
                SetLocalRect(rosterSubtitles[i].rectTransform, new Rect(66f, 32f, row.width - 78f, 22f));
            }

            SetLocalRect(editorTitle.rectTransform, new Rect(18f, 14f, 320f, 28f));
            SetLocalRect(editorHint.rectTransform, new Rect(18f, 45f, geometry.Editor.width - 36f, 42f));
            Rect localDetails = new Rect(geometry.Editor.width - geometry.Details.width - 18f, 116f, geometry.Details.width, geometry.Details.height);
            PartySetupEditorFlowGeometry flow = PartySetupScreenLayout.CalculateEditorFlow(geometry.Editor, localDetails, skillButtons.Count);
            bool compactIdentityLayout = flow.Compact;
            if (compactIdentityLayout)
            {
                float controlsLeft = 18f;
                float controlsRight = PartySetupScreenLayout.CompactIdentityControlsRight(localDetails);
                float controlsWidth = Mathf.Max(1f, controlsRight - controlsLeft);

                const float controlGap = 8f;
                const float randomNameWidth = 86f;
                const float nameFieldX = 100f;
                float randomNameX = controlsRight - randomNameWidth;
                float compactNameWidth = Mathf.Max(120f, randomNameX - controlGap - nameFieldX);
                AddOrMoveLabel("Name Label", "Name", new Rect(controlsLeft, 98f, 80f, 24f));
                SetLocalRect(nameField.GetComponent<RectTransform>(), new Rect(nameFieldX, 94f, compactNameWidth, 30f));
                SetLocalRect(randomNameButton.GetComponent<RectTransform>(), new Rect(randomNameX, 94f, randomNameWidth, 30f));

                float identityWidth = (controlsWidth - controlGap * 3f) / 4f;
                SetLocalRect(classButton.GetComponent<RectTransform>(), new Rect(controlsLeft, 130f, identityWidth, 30f));
                SetLocalRect(raceButton.GetComponent<RectTransform>(), new Rect(controlsLeft + (identityWidth + controlGap), 130f, identityWidth, 30f));
                SetLocalRect(originButton.GetComponent<RectTransform>(), new Rect(controlsLeft + (identityWidth + controlGap) * 2f, 130f, identityWidth, 30f));
                SetLocalRect(sigilButton.GetComponent<RectTransform>(), new Rect(controlsLeft + (identityWidth + controlGap) * 3f, 130f, identityWidth, 30f));

                float rerollWidth = (controlsWidth - controlGap * 2f) / 3f;
                SetLocalRect(rerollGearButton.GetComponent<RectTransform>(), new Rect(controlsLeft, 166f, rerollWidth, 28f));
                SetLocalRect(rerollLookButton.GetComponent<RectTransform>(), new Rect(controlsLeft + rerollWidth + controlGap, 166f, rerollWidth, 28f));
                SetLocalRect(colorButton.GetComponent<RectTransform>(), new Rect(controlsLeft + (rerollWidth + controlGap) * 2f, 166f, rerollWidth, 28f));
                SetLocalRect(selectedRaceClass.rectTransform, new Rect(nameFieldX, 200f, Mathf.Max(1f, controlsRight - nameFieldX), 32f));
            }
            else
            {
                SetLocalRect(nameField.GetComponent<RectTransform>(), new Rect(100f, 94f, 210f, 30f));
                AddOrMoveLabel("Name Label", "Name", new Rect(18f, 98f, 80f, 24f));
                SetLocalRect(classButton.GetComponent<RectTransform>(), new Rect(326f, 94f, 76f, 30f));
                SetLocalRect(raceButton.GetComponent<RectTransform>(), new Rect(410f, 94f, 76f, 30f));
                SetLocalRect(originButton.GetComponent<RectTransform>(), new Rect(494f, 94f, 76f, 30f));
                SetLocalRect(sigilButton.GetComponent<RectTransform>(), new Rect(578f, 94f, 76f, 30f));
                SetLocalRect(randomNameButton.GetComponent<RectTransform>(), new Rect(662f, 94f, 86f, 30f));
                SetLocalRect(selectedRaceClass.rectTransform, new Rect(100f, 128f, 220f, 44f));
                SetLocalRect(rerollGearButton.GetComponent<RectTransform>(), new Rect(326f, 128f, 94f, 28f));
                SetLocalRect(rerollLookButton.GetComponent<RectTransform>(), new Rect(430f, 128f, 92f, 28f));
                SetLocalRect(colorButton.GetComponent<RectTransform>(), new Rect(532f, 128f, 78f, 28f));

            }

            for (int i = 0; i < 4; i++)
            {
                float rowY = PartySetupScreenLayout.StatRowY(compactIdentityLayout, i);
                SetLocalRect(editorPanel.Find("Stat " + new[] { "Strength", "Intelligence", "Agility", "Health" }[i]).GetComponent<RectTransform>(), new Rect(18f, rowY + 3f, 116f, 24f));
                SetLocalRect(editorPanel.Find("Stat Down " + i).GetComponent<RectTransform>(), new Rect(140f, rowY, 32f, 30f));
                SetLocalRect(statValues[i].rectTransform, new Rect(182f, rowY + 3f, 42f, 24f));
                SetLocalRect(editorPanel.Find("Stat Up " + i).GetComponent<RectTransform>(), new Rect(228f, rowY, 32f, 30f));
            }

            for (int i = 0; i < skillButtons.Count; i++)
            {
                SetLocalRect(skillButtons[i].GetComponent<RectTransform>(), new Rect(18f + i * 78f, flow.SkillControls.y, 72f, 30f));
            }

            SetLocalRect(detailsPanel, localDetails);
            SetLocalRect(detailsName.rectTransform, new Rect(18f, 14f, localDetails.width - 36f, 26f));
            SetLocalRect(detailsBody.rectTransform, new Rect(18f, 48f, localDetails.width - 36f, localDetails.height - 64f));
            SetLocalRect(noteText.rectTransform, flow.Note);
        }

        private void EnsureRosterRows(int count)
        {
            while (rosterButtons.Count < count)
            {
                int index = rosterButtons.Count;
                Button button = AddButton("Roster " + index, rosterPanel, "", () => bindings?.SelectMember?.Invoke(index), false);
                Text name = AddText("Roster Name " + index, button.transform, "", 13, Hex("f3ead7", 1f), TextAnchor.MiddleLeft);
                Text subtitle = AddText("Roster Subtitle " + index, button.transform, "", 10, Hex("b7aa90", 1f), TextAnchor.MiddleLeft);
                Image swatch = AddImage("Roster Swatch " + index, button.transform, Hex("d7a84e", 1f));
                rosterButtons.Add(button);
                rosterNames.Add(name);
                rosterSubtitles.Add(subtitle);
                rosterSwatches.Add(swatch);
            }

            for (int i = 0; i < rosterButtons.Count; i++)
            {
                rosterButtons[i].gameObject.SetActive(i < count);
            }
        }

        private void AddOrMoveLabel(string name, string text, Rect rect)
        {
            Transform existing = editorPanel.Find(name);
            Text label = existing == null ? AddText(name, editorPanel, text, 13, Hex("f3ead7", 1f), TextAnchor.MiddleLeft) : existing.GetComponent<Text>();
            SetLocalRect(label.rectTransform, rect);
        }

        private void SetEditorEnabled(bool enabled)
        {
            foreach (Selectable selectable in editorPanel.GetComponentsInChildren<Selectable>()) selectable.interactable = enabled;
        }

        private InputField AddInput(string name, Transform parent)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(InputField));
            go.transform.SetParent(parent, false);
            Image image = go.GetComponent<Image>();
            image.color = Hex("080b0d", 0.92f);
            InputField field = go.GetComponent<InputField>();
            Text text = AddText("Text", go.transform, "", 14, Hex("f3ead7", 1f), TextAnchor.MiddleLeft);
            Stretch(text.rectTransform, 8f, 3f);
            field.textComponent = text;
            field.characterLimit = 16;
            return field;
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
            colors.selectedColor = colors.highlightedColor;
            button.colors = colors;
            if (action != null) button.onClick.AddListener(() => action());
            Text text = AddText("Label", go.transform, label, hero ? 15 : 12, Hex("f3ead7", 1f), TextAnchor.MiddleCenter);
            text.fontStyle = FontStyle.Bold;
            Stretch(text.rectTransform, 6f, 3f);
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
            if (size >= 20) text.fontStyle = FontStyle.Bold;
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

        private static Color ParseColor(string hex, Color fallback)
        {
            if (string.IsNullOrWhiteSpace(hex)) return fallback;
            try
            {
                if (hex.StartsWith("#")) hex = hex.Substring(1);
                byte r = Convert.ToByte(hex.Substring(0, 2), 16);
                byte g = Convert.ToByte(hex.Substring(2, 2), 16);
                byte b = Convert.ToByte(hex.Substring(4, 2), 16);
                return new Color32(r, g, b, 255);
            }
            catch
            {
                return fallback;
            }
        }

        private static Color Hex(string hex, float alpha)
        {
            Color color = ParseColor(hex, Color.white);
            color.a = Mathf.Clamp01(alpha);
            return color;
        }

        private static void EnsureEventSystem()
        {
            UiRuntime.EnsureEventSystemReady();
        }
    }
}
