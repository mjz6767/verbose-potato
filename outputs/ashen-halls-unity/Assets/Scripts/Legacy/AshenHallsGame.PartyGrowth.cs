using System;
using System.Collections.Generic;
using UnityEngine;

namespace AshenHalls
{
    public sealed partial class AshenHallsGame
    {
        private const int GrowthChoiceRowKeyBase = 20000;
        private const int GrowthApplyActionKey = 20100;
        private const int GrowthResetActionKey = 20101;

        private static readonly PartyGrowthChoice[] ArmoryGrowthAttributes =
        {
            PartyGrowthChoice.Strength,
            PartyGrowthChoice.Intelligence,
            PartyGrowthChoice.Dexterity,
            PartyGrowthChoice.Health
        };

        private readonly Dictionary<string, PartyGrowthPlan> armoryGrowthDrafts =
            new Dictionary<string, PartyGrowthPlan>(StringComparer.Ordinal);

        private int armoryGrowthSelectedChoiceKey = GrowthChoiceRowKeyBase;

        private bool ArmoryGrowthReadOnly => state == null
            || state.Mode == GameMode.Combat
            || state.Combat != null;

        private string[] ArmoryGrowthMemberFilters()
        {
            if (state?.Party == null || state.Party.Count == 0) return Array.Empty<string>();
            string[] filters = new string[state.Party.Count];
            for (int i = 0; i < state.Party.Count; i++)
            {
                PartyMember member = state.Party[i];
                filters[i] = string.IsNullOrWhiteSpace(member?.Name) ? "Member " + (i + 1) : member.Name;
            }
            return filters;
        }

        private void SelectArmoryGrowthMember(int filter)
        {
            if (state?.Party == null || state.Party.Count == 0) return;
            int next = Mathf.Clamp(filter, 0, state.Party.Count - 1);
            if (armorySelectedPartyIndex == next) return;
            armorySelectedPartyIndex = next;
            armoryGrowthSelectedChoiceKey = GrowthChoiceRowKeyBase;
            MarkUiDirty();
            SyncArmoryOverlayScreen();
            PlaySfx("uitab", 0.40f);
        }

        private IReadOnlyList<ArmoryRowView> BuildArmoryGrowthRows()
        {
            List<ArmoryRowView> rows = new List<ArmoryRowView>();
            PartyMember member = SelectedArmoryGrowthMember(out int memberIndex);
            if (member == null) return rows;

            PartyGrowthPlan plan = ArmoryGrowthPlan(member, memberIndex, false);
            foreach (PartyGrowthChoice choice in ArmoryGrowthAttributes)
            {
                rows.Add(BuildArmoryGrowthRow(member, plan, choice));
            }
            foreach (PartyGrowthChoice choice in PartyGrowthRules.RelevantTalents(member))
            {
                rows.Add(BuildArmoryGrowthRow(member, plan, choice));
            }
            return rows;
        }

        private ArmoryRowView BuildArmoryGrowthRow(
            PartyMember member,
            PartyGrowthPlan plan,
            PartyGrowthChoice choice)
        {
            bool attribute = PartyGrowthRules.IsAttribute(choice);
            int current = ArmoryGrowthCurrentValue(member, choice);
            int projected = attribute
                ? PartyGrowthRules.ProjectedAttribute(member, plan, choice)
                : PartyGrowthRules.ProjectedSkill(member, plan, choice);
            int staged = plan.Get(choice);
            string blockedReason = "";
            bool canStage = !ArmoryGrowthReadOnly
                && PartyGrowthRules.CanStage(member, plan, choice, out blockedReason);
            string detail = ArmoryGrowthEffectLine(choice);
            if (!ArmoryGrowthReadOnly && !canStage && !string.IsNullOrWhiteSpace(blockedReason))
            {
                detail += " / " + blockedReason;
            }

            int points = attribute ? member.StatPoints : member.SkillPoints;
            return new ArmoryRowView
            {
                Key = GrowthChoiceRowKeyBase + (int)choice,
                Title = ArmoryGrowthDisplayLabel(choice),
                Subtitle = (attribute ? "ATTRIBUTE" : "CLASS TALENT")
                    + "  /  " + current + " -> " + projected
                    + "  /  costs 1 " + (attribute ? "stat" : "skill") + " point",
                Detail = detail,
                AccentHex = ColorHtml(attribute ? gold : MemberColor(member)),
                Badge = staged > 0
                    ? staged + " STAGED"
                    : points + (attribute ? " STAT" : " SKILL"),
                BadgeAccentHex = ColorHtml(attribute ? gold : MemberColor(member)),
                ActionLabel = ArmoryGrowthReadOnly
                    ? "Locked"
                    : canStage ? (attribute ? "Add" : "Train") : "Unavailable",
                ActionEnabled = canStage,
                Selected = armoryGrowthSelectedChoiceKey == GrowthChoiceRowKeyBase + (int)choice,
                IconLabel = ArmoryGrowthIconLabel(choice)
            };
        }

        private ArmoryDetailView BuildArmoryGrowthDetail()
        {
            PartyMember member = SelectedArmoryGrowthMember(out int memberIndex);
            if (member == null) return null;
            PartyGrowthPlan plan = ArmoryGrowthPlan(member, memberIndex, false);
            PartyMember preview = member.CloneForPreview();
            if (!plan.IsEmpty) PartyGrowthRules.TryApply(preview, plan, out _);
            RecalculateMember(preview);

            bool valid = PartyGrowthRules.Validate(member, plan, out string reason);
            bool canApply = !ArmoryGrowthReadOnly && !plan.IsEmpty && valid;
            List<ArmoryDetailActionView> actions = new List<ArmoryDetailActionView>
            {
                new ArmoryDetailActionView
                {
                    Key = GrowthApplyActionKey,
                    Label = "Apply staged growth",
                    Detail = ArmoryGrowthReadOnly
                        ? "Training is read-only during combat."
                        : plan.IsEmpty
                            ? "Stage an attribute or class talent first."
                            : valid ? ArmoryGrowthCostLine(member, plan) : reason,
                    ButtonLabel = "Apply",
                    AccentHex = ColorHtml(teal),
                    Enabled = canApply
                },
                new ArmoryDetailActionView
                {
                    Key = GrowthResetActionKey,
                    Label = "Reset staged choices",
                    Detail = "Return every staged point before it is applied.",
                    ButtonLabel = "Reset",
                    AccentHex = ColorHtml(line),
                    Enabled = !plan.IsEmpty
                }
            };

            return new ArmoryDetailView
            {
                Eyebrow = ArmoryGrowthReadOnly ? "READ-ONLY DURING COMBAT" : "TRAINING PREVIEW",
                Title = member.Name,
                Subtitle = "L" + Mathf.Max(1, member.Level) + " " + DisplayRace(member.Race) + " " + DisplayClass(member.ClassKey),
                Summary = ArmoryGrowthPreviewSummary(member, preview, plan),
                ActionsHeading = "CONFIRM TRAINING",
                ExtendedSummary = true,
                AccentHex = ColorHtml(MemberColor(member)),
                IconLabel = MemberInitials(member),
                Actions = actions
            };
        }

        private void RunArmoryGrowthRowAction(int key)
        {
            int rawChoice = key - GrowthChoiceRowKeyBase;
            if (rawChoice < (int)PartyGrowthChoice.Strength || rawChoice > (int)PartyGrowthChoice.Guard) return;
            PartyMember member = SelectedArmoryGrowthMember(out int memberIndex);
            if (member == null || ArmoryGrowthReadOnly) return;

            PartyGrowthChoice choice = (PartyGrowthChoice)rawChoice;
            PartyGrowthPlan plan = ArmoryGrowthPlan(member, memberIndex, true);
            if (!PartyGrowthRules.TryStage(member, plan, choice, out string reason))
            {
                if (!string.IsNullOrWhiteSpace(reason)) PushLog(reason, Tone.Warn);
                PlaySfx("blocked", 0.38f);
                return;
            }

            armoryGrowthSelectedChoiceKey = key;
            MarkUiDirty();
            SyncArmoryOverlayScreen();
            PlaySfx("uiconfirm", 0.42f);
        }

        private void RunArmoryGrowthDetailAction(int key)
        {
            PartyMember member = SelectedArmoryGrowthMember(out int memberIndex);
            if (member == null) return;
            PartyGrowthPlan plan = ArmoryGrowthPlan(member, memberIndex, true);

            if (key == GrowthResetActionKey)
            {
                if (plan.IsEmpty) return;
                plan.Reset();
                MarkUiDirty();
                SyncArmoryOverlayScreen();
                PlaySfx("uiclose", 0.38f);
                return;
            }
            if (key != GrowthApplyActionKey || ArmoryGrowthReadOnly || plan.IsEmpty) return;

            if (!PartyGrowthRules.TryApply(member, plan, out string summary))
            {
                PushLog(summary, Tone.Warn);
                PlaySfx("blocked", 0.42f);
                return;
            }

            RecalculateMember(member);
            plan.Reset();
            PushLog(member.Name + ": " + summary + " Spend remaining points in I > Growth.", Tone.Good);
            ShowBanner("Growth Applied");
            MarkUiDirty();
            AutosaveCheckpoint("party growth applied");
            SyncArmoryOverlayScreen();
            PlaySfx("itemequip", 0.55f);
        }

        private PartyMember SelectedArmoryGrowthMember(out int index)
        {
            index = 0;
            if (state?.Party == null || state.Party.Count == 0) return null;
            index = Mathf.Clamp(armorySelectedPartyIndex, 0, state.Party.Count - 1);
            armorySelectedPartyIndex = index;
            return state.Party[index];
        }

        private PartyGrowthPlan ArmoryGrowthPlan(PartyMember member, int index, bool create)
        {
            if (member == null) return new PartyGrowthPlan();
            string key = ArmoryGrowthMemberKey(member, index);
            if (armoryGrowthDrafts.TryGetValue(key, out PartyGrowthPlan plan)) return plan;
            if (!create) return new PartyGrowthPlan();
            plan = new PartyGrowthPlan();
            armoryGrowthDrafts[key] = plan;
            return plan;
        }

        private string ArmoryGrowthMemberKey(PartyMember member, int index)
        {
            return string.IsNullOrWhiteSpace(member?.Id) ? "party-index-" + index : member.Id;
        }

        private void DiscardArmoryGrowthDrafts()
        {
            if (armoryGrowthDrafts.Count == 0) return;
            armoryGrowthDrafts.Clear();
            armoryGrowthSelectedChoiceKey = GrowthChoiceRowKeyBase;
        }

        private int ArmoryGrowthDraftHash()
        {
            if (state?.Party == null || armoryGrowthDrafts.Count == 0) return 0;
            int hash = 17;
            for (int i = 0; i < state.Party.Count; i++)
            {
                PartyMember member = state.Party[i];
                if (member == null) continue;
                PartyGrowthPlan plan = ArmoryGrowthPlan(member, i, false);
                for (int choice = (int)PartyGrowthChoice.Strength; choice <= (int)PartyGrowthChoice.Guard; choice++)
                {
                    hash = unchecked(hash * 31 + plan.Get((PartyGrowthChoice)choice));
                }
            }
            return hash;
        }

        private string ArmoryGrowthSubtitle()
        {
            return ArmoryGrowthReadOnly
                ? "Review earned growth now; spend it safely after combat."
                : "Choose an adventurer, stage earned attributes or class talents, then apply the preview.";
        }

        private string ArmoryGrowthFooter()
        {
            return ArmoryGrowthReadOnly
                ? "Growth is read-only during combat  /  Esc closes"
                : "Stage choices  /  Apply or Reset on the right  /  Closing discards unconfirmed choices";
        }

        private string ArmoryGrowthPreviewSummary(
            PartyMember member,
            PartyMember preview,
            PartyGrowthPlan plan)
        {
            string staged = plan.IsEmpty
                ? "No choices staged"
                : plan.SpentStatPoints + " stat / " + plan.SpentSkillPoints + " skill staged";
            return "POINTS  Stat " + member.StatPoints + " -> " + (member.StatPoints - plan.SpentStatPoints)
                + "  /  Skill " + member.SkillPoints + " -> " + (member.SkillPoints - plan.SpentSkillPoints)
                + "\nSTAGED  " + staged
                + "\nHP  " + member.MaxHp + " -> " + preview.MaxHp
                + "  /  MP  " + member.MaxMana + " -> " + preview.MaxMana
                + "\nDAMAGE  " + member.DamageMin + "-" + member.DamageMax
                + " -> " + preview.DamageMin + "-" + preview.DamageMax
                + "\nPOWER  " + member.Power + " -> " + preview.Power
                + "  /  DEFENSE  " + member.Defense + " -> " + preview.Defense
                + "\nSPEED  " + member.AttackSpeed + " -> " + preview.AttackSpeed
                + "  /  MOVE  " + member.Movement + " -> " + preview.Movement
                + "\n" + ProgressionUnlockLine(member);
        }

        private string ArmoryGrowthCostLine(PartyMember member, PartyGrowthPlan plan)
        {
            return "Spend " + plan.SpentStatPoints + " of " + member.StatPoints + " stat and "
                + plan.SpentSkillPoints + " of " + member.SkillPoints + " skill points.";
        }

        private int ArmoryGrowthCurrentValue(PartyMember member, PartyGrowthChoice choice)
        {
            switch (choice)
            {
                case PartyGrowthChoice.Strength: return member.Stats.Strength;
                case PartyGrowthChoice.Intelligence: return member.Stats.Intelligence;
                case PartyGrowthChoice.Dexterity: return member.Stats.Dexterity;
                case PartyGrowthChoice.Health: return member.Stats.Health;
                case PartyGrowthChoice.Arms: return member.Skills?.Arms ?? 0;
                case PartyGrowthChoice.Missile: return member.Skills?.Missile ?? 0;
                case PartyGrowthChoice.Mend: return member.Skills?.Mend ?? 0;
                case PartyGrowthChoice.Ember: return member.Skills?.Ember ?? 0;
                case PartyGrowthChoice.Hex: return member.Skills?.Hex ?? 0;
                case PartyGrowthChoice.Guard: return member.Skills?.Guard ?? 0;
                default: return 0;
            }
        }

        private string ArmoryGrowthDisplayLabel(PartyGrowthChoice choice)
        {
            return choice == PartyGrowthChoice.Dexterity ? "Agility" : PartyGrowthRules.Label(choice);
        }

        private string ArmoryGrowthIconLabel(PartyGrowthChoice choice)
        {
            switch (choice)
            {
                case PartyGrowthChoice.Strength: return "STR";
                case PartyGrowthChoice.Intelligence: return "INT";
                case PartyGrowthChoice.Dexterity: return "AGI";
                case PartyGrowthChoice.Health: return "HEA";
                case PartyGrowthChoice.Arms: return "ARM";
                case PartyGrowthChoice.Missile: return "MIS";
                case PartyGrowthChoice.Mend: return "MEN";
                case PartyGrowthChoice.Ember: return "EMB";
                case PartyGrowthChoice.Hex: return "HEX";
                case PartyGrowthChoice.Guard: return "GRD";
                default: return "+";
            }
        }

        private string ArmoryGrowthEffectLine(PartyGrowthChoice choice)
        {
            switch (choice)
            {
                case PartyGrowthChoice.Strength: return "Raises melee and reach damage, power, and some defense.";
                case PartyGrowthChoice.Intelligence: return "Raises spell mana and focus-weapon damage.";
                case PartyGrowthChoice.Dexterity: return "Raises speed, agility, and ranged or finesse damage.";
                case PartyGrowthChoice.Health: return "Raises maximum health and some defense.";
                case PartyGrowthChoice.Arms: return "Improves melee and reach weapon attacks.";
                case PartyGrowthChoice.Missile: return "Improves bows and other ranged weapon attacks.";
                case PartyGrowthChoice.Mend: return "Improves healing and protective priest formulas.";
                case PartyGrowthChoice.Ember: return "Improves fire and force formulas.";
                case PartyGrowthChoice.Hex: return "Improves curses, shadow magic, and pacts.";
                case PartyGrowthChoice.Guard: return "Improves guarding, interception, and shieldcraft.";
                default: return "";
            }
        }

        private bool TryPartyGrowthChoice(string key, out PartyGrowthChoice choice)
        {
            switch ((key ?? "").Trim().ToLowerInvariant())
            {
                case "arms": choice = PartyGrowthChoice.Arms; return true;
                case "missile": choice = PartyGrowthChoice.Missile; return true;
                case "mend": choice = PartyGrowthChoice.Mend; return true;
                case "ember": choice = PartyGrowthChoice.Ember; return true;
                case "hex": choice = PartyGrowthChoice.Hex; return true;
                case "guard": choice = PartyGrowthChoice.Guard; return true;
                default:
                    choice = PartyGrowthChoice.Arms;
                    return false;
            }
        }

        private PartyGrowthChoice PartyGrowthAttributeForStatCode(int code)
        {
            switch (code)
            {
                case -1: return PartyGrowthChoice.Strength;
                case -2: return PartyGrowthChoice.Intelligence;
                case -3: return PartyGrowthChoice.Dexterity;
                default: return PartyGrowthChoice.Health;
            }
        }

        private void StagePartyGrowthVisualSmoke()
        {
            QuickStart();
            PartyMember member = SelectedArmoryGrowthMember(out int memberIndex);
            if (member == null) throw new InvalidOperationException("Party Growth visual smoke requires a party member.");
            member.StatPoints = Mathf.Max(member.StatPoints, 2);
            member.SkillPoints = Mathf.Max(member.SkillPoints, 2);
            PartyGrowthPlan plan = ArmoryGrowthPlan(member, memberIndex, true);
            plan.Reset();
            if (!PartyGrowthRules.TryStage(member, plan, PartyGrowthChoice.Strength, out string statReason))
            {
                throw new InvalidOperationException("Party Growth visual smoke could not stage Strength: " + statReason);
            }
            PartyGrowthChoice talent = PartyGrowthRules.RelevantTalents(member)[0];
            if (!PartyGrowthRules.TryStage(member, plan, talent, out string talentReason))
            {
                throw new InvalidOperationException("Party Growth visual smoke could not stage a class talent: " + talentReason);
            }
            armoryTab = (int)ArmoryTab.Growth;
            armoryGrowthSelectedChoiceKey = GrowthChoiceRowKeyBase + (int)talent;
            showArmory = true;
            MarkUiDirty();
            SyncArmoryOverlayScreen();
            Debug.Log(VersionInfo.ProductName + " Party Growth smoke staged: member=" + member.Name
                + ", statPoints=" + member.StatPoints + ", skillPoints=" + member.SkillPoints
                + ", talent=" + PartyGrowthRules.Label(talent) + ".");
        }

        private bool TryValidatePartyGrowthVisualSmoke(out string failure)
        {
            failure = "";
            if (state?.Party == null || state.Party.Count == 0)
            {
                failure = "party is missing";
                return false;
            }
            if (CurrentUiOverlay() != UiOverlay.Armory
                || armoryTab != (int)ArmoryTab.Growth
                || armoryOverlayScreen == null
                || !armoryOverlayScreen.IsVisible
                || !armoryOverlayScreen.HasRenderableGeometry)
            {
                failure = "Growth Armory overlay is not visible and renderable";
                return false;
            }
            if (armoryOverlayScreen.ActiveTabLabelForTest != "Growth"
                || armoryOverlayScreen.VisibleFilterCountForTest != state.Party.Count
                || armoryOverlayScreen.VisibleDetailActionCountForTest != 2)
            {
                failure = "Growth member filters, tab label, or Apply/Reset actions are incomplete";
                return false;
            }

            IReadOnlyList<ArmoryRowView> rows = BuildArmoryGrowthRows();
            int stagedRows = 0;
            for (int i = 0; i < rows.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(rows[i].Badge)
                    && rows[i].Badge.IndexOf("STAGED", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    stagedRows++;
                }
            }
            if (rows.Count < 6 || stagedRows != 2 || BuildArmoryGrowthDetail() == null)
            {
                failure = "Growth preview does not contain four attributes, class talents, and two staged choices";
                return false;
            }
            return true;
        }
    }
}
