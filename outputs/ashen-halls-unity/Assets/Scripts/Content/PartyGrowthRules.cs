using System;
using System.Collections.Generic;

namespace AshenHalls
{
    public enum PartyGrowthChoice
    {
        Strength,
        Intelligence,
        Dexterity,
        Health,
        Arms,
        Missile,
        Mend,
        Ember,
        Hex,
        Guard
    }

    public sealed class PartyGrowthPlan
    {
        private const int ChoiceCount = (int)PartyGrowthChoice.Guard + 1;
        private readonly int[] stagedCounts;

        public PartyGrowthPlan()
        {
            stagedCounts = new int[ChoiceCount];
        }

        private PartyGrowthPlan(int[] source)
        {
            stagedCounts = new int[ChoiceCount];
            Array.Copy(source, stagedCounts, ChoiceCount);
        }

        public int SpentStatPoints
        {
            get
            {
                return Get(PartyGrowthChoice.Strength)
                    + Get(PartyGrowthChoice.Intelligence)
                    + Get(PartyGrowthChoice.Dexterity)
                    + Get(PartyGrowthChoice.Health);
            }
        }

        public int SpentSkillPoints
        {
            get
            {
                return Get(PartyGrowthChoice.Arms)
                    + Get(PartyGrowthChoice.Missile)
                    + Get(PartyGrowthChoice.Mend)
                    + Get(PartyGrowthChoice.Ember)
                    + Get(PartyGrowthChoice.Hex)
                    + Get(PartyGrowthChoice.Guard);
            }
        }

        public bool IsEmpty => SpentStatPoints == 0 && SpentSkillPoints == 0;

        public int Get(PartyGrowthChoice choice)
        {
            int index = (int)choice;
            return index >= 0 && index < ChoiceCount ? stagedCounts[index] : 0;
        }

        public void Increment(PartyGrowthChoice choice)
        {
            int index = (int)choice;
            if (index < 0 || index >= ChoiceCount)
            {
                throw new ArgumentOutOfRangeException(nameof(choice));
            }
            if (stagedCounts[index] == int.MaxValue)
            {
                throw new InvalidOperationException("The staged growth count is already at its maximum value.");
            }
            stagedCounts[index]++;
        }

        public void Reset()
        {
            Array.Clear(stagedCounts, 0, stagedCounts.Length);
        }

        public PartyGrowthPlan Clone()
        {
            return new PartyGrowthPlan(stagedCounts);
        }
    }

    public static class PartyGrowthRules
    {
        public const int MaximumValue = 99;
        public const int AttributeGain = 1;
        public const int TalentGain = 2;

        private static readonly IReadOnlyList<PartyGrowthChoice> NoTalents =
            Array.AsReadOnly(new PartyGrowthChoice[0]);

        private static readonly IReadOnlyList<PartyGrowthChoice> WarriorTalents =
            ReadOnlyChoices(PartyGrowthChoice.Arms, PartyGrowthChoice.Guard);

        private static readonly IReadOnlyList<PartyGrowthChoice> RogueTalents =
            ReadOnlyChoices(PartyGrowthChoice.Arms, PartyGrowthChoice.Missile);

        private static readonly IReadOnlyList<PartyGrowthChoice> RangerTalents =
            ReadOnlyChoices(PartyGrowthChoice.Missile, PartyGrowthChoice.Arms);

        private static readonly IReadOnlyList<PartyGrowthChoice> WizardTalents =
            ReadOnlyChoices(PartyGrowthChoice.Ember, PartyGrowthChoice.Hex);

        private static readonly IReadOnlyList<PartyGrowthChoice> MageTalents =
            ReadOnlyChoices(PartyGrowthChoice.Ember);

        private static readonly IReadOnlyList<PartyGrowthChoice> WarlockTalents =
            ReadOnlyChoices(PartyGrowthChoice.Hex, PartyGrowthChoice.Arms);

        private static readonly IReadOnlyList<PartyGrowthChoice> PriestTalents =
            ReadOnlyChoices(PartyGrowthChoice.Mend, PartyGrowthChoice.Guard);

        private static readonly IReadOnlyList<PartyGrowthChoice> PaladinTalents =
            ReadOnlyChoices(PartyGrowthChoice.Guard, PartyGrowthChoice.Arms, PartyGrowthChoice.Mend);

        private static readonly PartyGrowthChoice[] AttributeChoices =
        {
            PartyGrowthChoice.Strength,
            PartyGrowthChoice.Intelligence,
            PartyGrowthChoice.Dexterity,
            PartyGrowthChoice.Health
        };

        private static readonly PartyGrowthChoice[] TalentChoices =
        {
            PartyGrowthChoice.Arms,
            PartyGrowthChoice.Missile,
            PartyGrowthChoice.Mend,
            PartyGrowthChoice.Ember,
            PartyGrowthChoice.Hex,
            PartyGrowthChoice.Guard
        };

        public static bool IsAttribute(PartyGrowthChoice choice)
        {
            int value = (int)choice;
            return value >= (int)PartyGrowthChoice.Strength
                && value <= (int)PartyGrowthChoice.Health;
        }

        public static string Label(PartyGrowthChoice choice)
        {
            switch (choice)
            {
                case PartyGrowthChoice.Strength: return "Strength";
                case PartyGrowthChoice.Intelligence: return "Intelligence";
                case PartyGrowthChoice.Dexterity: return "Agility";
                case PartyGrowthChoice.Health: return "Health";
                case PartyGrowthChoice.Arms: return "Arms";
                case PartyGrowthChoice.Missile: return "Missile";
                case PartyGrowthChoice.Mend: return "Mend";
                case PartyGrowthChoice.Ember: return "Ember";
                case PartyGrowthChoice.Hex: return "Hex";
                case PartyGrowthChoice.Guard: return "Guard";
                default: return "";
            }
        }

        public static string Effect(PartyGrowthChoice choice)
        {
            if (!IsKnownChoice(choice)) return "";
            return "+" + (IsAttribute(choice) ? AttributeGain : TalentGain) + " " + Label(choice);
        }

        public static IReadOnlyList<PartyGrowthChoice> RelevantTalents(PartyMember member)
        {
            string classKey = (member?.ClassKey ?? "").Trim().ToLowerInvariant();
            switch (classKey)
            {
                case "warrior": return WarriorTalents;
                case "rogue": return RogueTalents;
                case "ranger": return RangerTalents;
                case "wizard": return WizardTalents;
                case "mage": return MageTalents;
                case "warlock": return WarlockTalents;
                case "priest": return PriestTalents;
                case "paladin": return PaladinTalents;
                default: return NoTalents;
            }
        }

        public static int ProjectedAttribute(
            PartyMember member,
            PartyGrowthPlan plan,
            PartyGrowthChoice choice)
        {
            if (member == null || plan == null || !IsAttribute(choice)) return 0;
            return SaturatingAdd(AttributeValue(member, choice), plan.Get(choice));
        }

        public static int ProjectedSkill(
            PartyMember member,
            PartyGrowthPlan plan,
            PartyGrowthChoice choice)
        {
            if (member == null || plan == null || !IsTalent(choice)) return 0;
            return SaturatingAdd(SkillValue(member.Skills, choice), plan.Get(choice), TalentGain);
        }

        public static bool CanStage(
            PartyMember member,
            PartyGrowthPlan plan,
            PartyGrowthChoice choice,
            out string reason)
        {
            if (member == null)
            {
                reason = "A party member is required.";
                return false;
            }
            if (plan == null)
            {
                reason = "A growth plan is required.";
                return false;
            }
            if (!IsKnownChoice(choice))
            {
                reason = "That growth choice is not recognized.";
                return false;
            }
            if (plan.Get(choice) == int.MaxValue)
            {
                reason = Label(choice) + " cannot be staged again.";
                return false;
            }

            PartyGrowthPlan proposed = plan.Clone();
            proposed.Increment(choice);
            return Validate(member, proposed, out reason);
        }

        public static bool TryStage(
            PartyMember member,
            PartyGrowthPlan plan,
            PartyGrowthChoice choice,
            out string reason)
        {
            if (!CanStage(member, plan, choice, out reason)) return false;
            plan.Increment(choice);
            reason = "";
            return true;
        }

        public static bool Validate(PartyMember member, PartyGrowthPlan plan, out string reason)
        {
            if (member == null)
            {
                reason = "A party member is required.";
                return false;
            }
            if (plan == null)
            {
                reason = "A growth plan is required.";
                return false;
            }
            if (member.StatPoints < 0)
            {
                reason = "Stat points cannot be negative.";
                return false;
            }
            if (member.SkillPoints < 0)
            {
                reason = "Skill points cannot be negative.";
                return false;
            }
            if (plan.SpentStatPoints > member.StatPoints)
            {
                reason = "Not enough stat points for this growth plan.";
                return false;
            }
            if (plan.SpentSkillPoints > member.SkillPoints)
            {
                reason = "Not enough skill points for this growth plan.";
                return false;
            }

            foreach (PartyGrowthChoice choice in AttributeChoices)
            {
                int current = AttributeValue(member, choice);
                if (current < 0)
                {
                    reason = Label(choice) + " cannot be negative.";
                    return false;
                }
                if (ProjectedAttribute(member, plan, choice) > MaximumValue)
                {
                    reason = Label(choice) + " cannot exceed " + MaximumValue + ".";
                    return false;
                }
            }

            IReadOnlyList<PartyGrowthChoice> relevantTalents = RelevantTalents(member);
            foreach (PartyGrowthChoice choice in TalentChoices)
            {
                int current = SkillValue(member.Skills, choice);
                if (current < 0)
                {
                    reason = Label(choice) + " cannot be negative.";
                    return false;
                }
                if (plan.Get(choice) > 0 && !Contains(relevantTalents, choice))
                {
                    reason = Label(choice) + " is not relevant to the " + ClassLabel(member) + " class.";
                    return false;
                }
                if (ProjectedSkill(member, plan, choice) > MaximumValue)
                {
                    reason = Label(choice) + " cannot exceed " + MaximumValue + ".";
                    return false;
                }
            }

            reason = "";
            return true;
        }

        public static bool TryApply(PartyMember member, PartyGrowthPlan plan, out string summary)
        {
            if (!Validate(member, plan, out summary)) return false;
            if (plan.IsEmpty)
            {
                summary = "No growth is staged.";
                return false;
            }

            Stats projectedStats = member.Stats;
            foreach (PartyGrowthChoice choice in AttributeChoices)
            {
                SetAttribute(ref projectedStats, choice, ProjectedAttribute(member, plan, choice));
            }

            SkillSet projectedSkills = null;
            if (plan.SpentSkillPoints > 0)
            {
                projectedSkills = CloneSkills(member.Skills);
                foreach (PartyGrowthChoice choice in TalentChoices)
                {
                    SetSkill(projectedSkills, choice, ProjectedSkill(member, plan, choice));
                }
            }

            List<string> applied = new List<string>();
            foreach (PartyGrowthChoice choice in AttributeChoices)
            {
                int count = plan.Get(choice);
                if (count > 0) applied.Add(Label(choice) + " +" + count * AttributeGain);
            }
            foreach (PartyGrowthChoice choice in TalentChoices)
            {
                int count = plan.Get(choice);
                if (count > 0) applied.Add(Label(choice) + " +" + count * TalentGain);
            }

            member.Stats = projectedStats;
            if (projectedSkills != null) member.Skills = projectedSkills;
            member.StatPoints -= plan.SpentStatPoints;
            member.SkillPoints -= plan.SpentSkillPoints;
            summary = "Applied " + string.Join(", ", applied) + ".";
            return true;
        }

        public static bool TrySpendAttributePoint(
            PartyMember member,
            PartyGrowthChoice choice,
            out string summary)
        {
            if (!IsAttribute(choice))
            {
                summary = "That growth choice is not an attribute.";
                return false;
            }
            return TrySpendPoint(member, choice, out summary);
        }

        public static bool TrySpendTalentPoint(
            PartyMember member,
            PartyGrowthChoice choice,
            out string summary)
        {
            if (!IsTalent(choice))
            {
                summary = "That growth choice is not a talent.";
                return false;
            }
            return TrySpendPoint(member, choice, out summary);
        }

        private static bool TrySpendPoint(
            PartyMember member,
            PartyGrowthChoice choice,
            out string summary)
        {
            PartyGrowthPlan plan = new PartyGrowthPlan();
            if (!TryStage(member, plan, choice, out summary)) return false;
            return TryApply(member, plan, out summary);
        }

        private static bool IsTalent(PartyGrowthChoice choice)
        {
            int value = (int)choice;
            return value >= (int)PartyGrowthChoice.Arms
                && value <= (int)PartyGrowthChoice.Guard;
        }

        private static bool IsKnownChoice(PartyGrowthChoice choice)
        {
            return IsAttribute(choice) || IsTalent(choice);
        }

        private static int AttributeValue(PartyMember member, PartyGrowthChoice choice)
        {
            switch (choice)
            {
                case PartyGrowthChoice.Strength: return member.Stats.Strength;
                case PartyGrowthChoice.Intelligence: return member.Stats.Intelligence;
                case PartyGrowthChoice.Dexterity: return member.Stats.Dexterity;
                case PartyGrowthChoice.Health: return member.Stats.Health;
                default: return 0;
            }
        }

        private static int SkillValue(SkillSet skills, PartyGrowthChoice choice)
        {
            if (skills == null) return 0;
            switch (choice)
            {
                case PartyGrowthChoice.Arms: return skills.Arms;
                case PartyGrowthChoice.Missile: return skills.Missile;
                case PartyGrowthChoice.Mend: return skills.Mend;
                case PartyGrowthChoice.Ember: return skills.Ember;
                case PartyGrowthChoice.Hex: return skills.Hex;
                case PartyGrowthChoice.Guard: return skills.Guard;
                default: return 0;
            }
        }

        private static void SetAttribute(
            ref Stats stats,
            PartyGrowthChoice choice,
            int value)
        {
            switch (choice)
            {
                case PartyGrowthChoice.Strength: stats.Strength = value; break;
                case PartyGrowthChoice.Intelligence: stats.Intelligence = value; break;
                case PartyGrowthChoice.Dexterity: stats.Dexterity = value; break;
                case PartyGrowthChoice.Health: stats.Health = value; break;
            }
        }

        private static void SetSkill(SkillSet skills, PartyGrowthChoice choice, int value)
        {
            switch (choice)
            {
                case PartyGrowthChoice.Arms: skills.Arms = value; break;
                case PartyGrowthChoice.Missile: skills.Missile = value; break;
                case PartyGrowthChoice.Mend: skills.Mend = value; break;
                case PartyGrowthChoice.Ember: skills.Ember = value; break;
                case PartyGrowthChoice.Hex: skills.Hex = value; break;
                case PartyGrowthChoice.Guard: skills.Guard = value; break;
            }
        }

        private static SkillSet CloneSkills(SkillSet skills)
        {
            return new SkillSet
            {
                Arms = skills?.Arms ?? 0,
                Missile = skills?.Missile ?? 0,
                Mend = skills?.Mend ?? 0,
                Ember = skills?.Ember ?? 0,
                Hex = skills?.Hex ?? 0,
                Guard = skills?.Guard ?? 0
            };
        }

        private static int SaturatingAdd(int current, int count, int gain = 1)
        {
            long projected = (long)current + (long)count * gain;
            if (projected > int.MaxValue) return int.MaxValue;
            if (projected < int.MinValue) return int.MinValue;
            return (int)projected;
        }

        private static bool Contains(
            IReadOnlyList<PartyGrowthChoice> choices,
            PartyGrowthChoice choice)
        {
            for (int i = 0; i < choices.Count; i++)
            {
                if (choices[i] == choice) return true;
            }
            return false;
        }

        private static string ClassLabel(PartyMember member)
        {
            string classKey = (member?.ClassKey ?? "").Trim();
            return string.IsNullOrEmpty(classKey) ? "unknown" : classKey;
        }

        private static IReadOnlyList<PartyGrowthChoice> ReadOnlyChoices(
            params PartyGrowthChoice[] choices)
        {
            return Array.AsReadOnly(choices);
        }
    }
}
