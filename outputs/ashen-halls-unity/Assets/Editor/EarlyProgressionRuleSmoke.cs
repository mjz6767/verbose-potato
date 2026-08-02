using System;
using System.Collections.Generic;
using System.Linq;

namespace AshenHalls.Editor
{
    public static class EarlyProgressionRuleSmoke
    {
        public static void Run()
        {
            ProgressionCurveAndRewardsAreStable();
            CatalogUnlocksAreExplicitAndBounded();
            PermanentClassLaddersAreStable();
            DefaultContentActivatesPermanentProgression();
            NewProgressionIconsAreStable();
            FormulaCodesAreUnique();
            NewSpellBalanceStaysInsideItsEnvelope();
        }

        private static void ProgressionCurveAndRewardsAreStable()
        {
            Require(ProgressionRules.MinimumLevel == 1, "progression minimum level must remain 1");
            Require(ProgressionRules.MaximumLevel == 20, "progression maximum level must remain 20");

            int[] expectedCosts =
            {
                100, 140, 180, 220, 260, 300, 340, 380, 420, 460,
                500, 540, 580, 620, 660, 700, 740, 780, 820
            };

            int cumulativeExperience = 0;
            int previousCost = 0;
            for (int level = ProgressionRules.MinimumLevel; level < ProgressionRules.MaximumLevel; level++)
            {
                int actualCost = ProgressionRules.ExperienceForNextLevel(level);
                int expectedCost = expectedCosts[level - ProgressionRules.MinimumLevel];
                Require(actualCost == expectedCost, $"level {level} XP cost: expected {expectedCost}, got {actualCost}");
                Require(actualCost > previousCost, $"level {level} XP cost must be greater than the preceding threshold");
                Require(
                    ProgressionRules.CumulativeExperienceToLevel(level) == cumulativeExperience,
                    $"cumulative XP entering level {level}: expected {cumulativeExperience}, got {ProgressionRules.CumulativeExperienceToLevel(level)}");

                cumulativeExperience += actualCost;
                previousCost = actualCost;
            }

            Require(cumulativeExperience == 8740, $"level 20 cumulative XP: expected 8740, got {cumulativeExperience}");
            Require(
                ProgressionRules.CumulativeExperienceToLevel(ProgressionRules.MaximumLevel) == 8740,
                $"catalog level 20 cumulative XP: expected 8740, got {ProgressionRules.CumulativeExperienceToLevel(ProgressionRules.MaximumLevel)}");
            Require(ProgressionRules.ExperienceForNextLevel(ProgressionRules.MaximumLevel) == 0, "level cap must not advertise another XP threshold");

            int skillPoints = 0;
            int statPoints = 0;
            for (int reachedLevel = ProgressionRules.MinimumLevel + 1; reachedLevel <= ProgressionRules.MaximumLevel; reachedLevel++)
            {
                skillPoints += ProgressionRules.SkillPointRewardForLevel(reachedLevel);
                statPoints += ProgressionRules.StatPointRewardForLevel(reachedLevel);
            }

            Require(skillPoints == 38, $"cumulative skill-point rewards: expected 38, got {skillPoints}");
            Require(statPoints == 29, $"cumulative stat-point rewards: expected 29, got {statPoints}");
            Require(ProgressionRules.SkillPointRewardForLevel(1) == 0, "level 1 must not grant a level-up skill reward");
            Require(ProgressionRules.StatPointRewardForLevel(1) == 0, "level 1 must not grant a level-up stat reward");
            Require(ProgressionRules.SkillPointRewardForLevel(21) == 0, "levels above the cap must not grant skill points");
            Require(ProgressionRules.StatPointRewardForLevel(21) == 0, "levels above the cap must not grant stat points");

            Require(ProgressionRules.NormalizeExperience(19, -1) == 0, "negative pre-cap XP must normalize to zero");
            Require(ProgressionRules.NormalizeExperience(19, 3456) == 3456, "positive pre-cap XP must be preserved");
            Require(ProgressionRules.NormalizeExperience(20, 3456) == 0, "level-cap XP must normalize to zero");
            Require(ProgressionRules.NormalizeExperience(21, 3456) == 0, "above-cap XP must normalize to zero");
        }

        private static void CatalogUnlocksAreExplicitAndBounded()
        {
            string[] classKeys = { "warrior", "rogue", "ranger", "demon" };
            var seenAbilityIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (string classKey in classKeys)
            {
                string[] ids = AbilityCatalog.IdsForClass(classKey).ToArray();
                Require(ids.Length > 0, $"{classKey} ability catalog must not be empty");
                foreach (string id in ids)
                {
                    Require(!string.IsNullOrWhiteSpace(id), $"{classKey} ability catalog contains a blank id");
                    Require(seenAbilityIds.Add(id), $"ability id appears in more than one catalog ladder: {id}");

                    MartialAbility ability = AbilityCatalog.For(id);
                    Require(ability != null, $"ability catalog id does not resolve: {id}");
                    Require(string.Equals(ability.Id, id, StringComparison.OrdinalIgnoreCase), $"ability id round-trip failed: {id}");
                    Require(string.Equals(ability.ClassKey, classKey, StringComparison.OrdinalIgnoreCase), $"ability {id} belongs to {ability.ClassKey}, expected {classKey}");
                    Require(ProgressionRules.IsValidUnlockLevel(ability.RequiredLevel), $"ability {id} unlock level must be within 1..20, got {ability.RequiredLevel}");
                }
            }

            foreach (FormulaDef formula in FormulaCatalog.All)
            {
                Require(formula != null, "formula catalog contains a null entry");
                Require(!string.IsNullOrWhiteSpace(formula.Code), "formula catalog contains a blank code");
                Require(FormulaCatalog.HasExplicitRequiredLevel(formula.Code), $"formula {formula.Code} has no explicit unlock level");

                int requiredLevel = FormulaCatalog.RequiredLevel(formula);
                Require(ProgressionRules.IsValidUnlockLevel(requiredLevel), $"formula {formula.Code} unlock level must be within 1..20, got {requiredLevel}");
            }
        }

        private static void PermanentClassLaddersAreStable()
        {
            int[] expectedLevels = { 1, 1, 3, 5, 8, 12, 16 };
            ValidatePermanentLadder(
                "warrior",
                new[] { "charge", "rally", "shieldbash", "execute", "cleave", "whirlwind", "sunder" },
                expectedLevels);
            ValidatePermanentLadder(
                "rogue",
                new[] { "stealth", "ambush", "throwknife", "smokebomb", "hamstring", "eviscerate", "shadowstep" },
                expectedLevels);
            ValidatePermanentLadder(
                "ranger",
                new[] { "aimedshot", "pinningshot", "scoutmark", "volley", "broadheadshot", "disruptingshot", "quickshot" },
                expectedLevels);

            Require(AbilityCatalog.For("sunder")?.RequiredLevel == 16, "Sunder must remain the warrior's level 16 unlock");
            Require(AbilityCatalog.For("shadowstep")?.RequiredLevel == 16, "Shadowstep must remain the rogue's level 16 unlock");
            Require(AbilityCatalog.For("quickshot")?.RequiredLevel == 16, "Quick Shot must remain the ranger's level 16 unlock");
        }

        private static void DefaultContentActivatesPermanentProgression()
        {
            string defaultContentSet = ContentSetCatalog.NormalizeContentSetId(null);
            Require(
                string.Equals(defaultContentSet, ContentSetCatalog.SewerSlice, StringComparison.OrdinalIgnoreCase),
                $"default content set: expected {ContentSetCatalog.SewerSlice}, got {defaultContentSet}");

            string[] permanentClasses = { "warrior", "rogue", "ranger" };
            foreach (string classKey in permanentClasses)
            {
                foreach (string abilityId in AbilityCatalog.IdsForClass(classKey))
                {
                    Require(ContentSetCatalog.AbilityActive(defaultContentSet, abilityId), $"default content must activate permanent ability {abilityId}");
                }
            }

            string[] newFormulaCodes = { "DWP", "CNS", "GRH", "SLV", "ACR" };
            foreach (string formulaCode in newFormulaCodes)
            {
                Require(ContentSetCatalog.FormulaActive(defaultContentSet, formulaCode), $"default content must activate new formula {formulaCode}");
            }
        }

        private static void NewProgressionIconsAreStable()
        {
            Require(CombatIconCatalog.AbilityIndex("sunder") == 24, "Sunder ability icon must remain at atlas index 24");
            Require(CombatIconCatalog.AbilityIndex("shadowstep") == 25, "Shadowstep ability icon must remain at atlas index 25");
            Require(CombatIconCatalog.AbilityIndex("quickshot") == 26, "Quick Shot ability icon must remain at atlas index 26");
            Require(CombatIconCatalog.ExpandedAbilityAtlasRows * CombatIconCatalog.AbilityAtlasColumns > 26, "ability atlas must contain indices 24..26");

            Require(CombatIconCatalog.SignatureSpellIndex("DWP") == 51, "Dawn Pulse spell icon must remain at atlas index 51");
            Require(CombatIconCatalog.SignatureSpellIndex("CNS") == 52, "Cinderstorm spell icon must remain at atlas index 52");
            Require(CombatIconCatalog.SignatureSpellIndex("GRH") == 53, "Grave Hook spell icon must remain at atlas index 53");
            Require(CombatIconCatalog.SignatureSpellIndex("SLV") == 54, "Soul Veil spell icon must remain at atlas index 54");
            Require(CombatIconCatalog.SignatureSpellIndex("ACR") == 55, "Ashen Curse spell icon must remain at atlas index 55");
            Require(CombatIconCatalog.SignatureSpellAtlasRows * CombatIconCatalog.SignatureSpellAtlasColumns > 55, "signature-spell atlas must contain indices 51..55");
        }

        private static void FormulaCodesAreUnique()
        {
            var codes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (FormulaDef formula in FormulaCatalog.All)
            {
                Require(formula != null, "formula catalog contains a null entry");
                Require(!string.IsNullOrWhiteSpace(formula.Code), "formula catalog contains a blank code");
                Require(codes.Add(formula.Code), $"formula code must be unique: {formula.Code}");
            }
        }

        private static void NewSpellBalanceStaysInsideItsEnvelope()
        {
            ValidateSpellBalance("DWP", 12, 9, 11, 10, 12, "", true);
            ValidateSpellBalance("CNS", 15, 10, 12, 15, 17, "bleed", true);
            ValidateSpellBalance("GRH", 8, 7, 9, 10, 12, "web", false);
            ValidateSpellBalance("SLV", 4, 9, 11, 0, 0, "shield", true);
            ValidateSpellBalance("ACR", 18, 11, 13, 13, 15, "hex", true);
        }

        private static void ValidatePermanentLadder(string classKey, string[] expectedIds, int[] expectedLevels)
        {
            string[] actualIds = AbilityCatalog.IdsForClass(classKey).ToArray();
            Require(actualIds.Length == 7, $"{classKey} permanent ladder: expected 7 skills, got {actualIds.Length}");
            Require(expectedIds.Length == expectedLevels.Length, $"{classKey} smoke-test ladder specification is malformed");

            for (int index = 0; index < expectedIds.Length; index++)
            {
                Require(
                    string.Equals(actualIds[index], expectedIds[index], StringComparison.OrdinalIgnoreCase),
                    $"{classKey} ladder slot {index}: expected {expectedIds[index]}, got {actualIds[index]}");

                MartialAbility ability = AbilityCatalog.For(actualIds[index]);
                Require(ability != null, $"{classKey} ladder ability does not resolve: {actualIds[index]}");
                Require(
                    ability.RequiredLevel == expectedLevels[index],
                    $"{classKey} {ability.Id} unlock: expected level {expectedLevels[index]}, got {ability.RequiredLevel}");
            }

            Require(expectedLevels[0] == 1 && expectedLevels[1] == 1, $"{classKey} ladder must begin with two starter skills");
            Require(expectedLevels[expectedLevels.Length - 1] > expectedLevels[1], $"{classKey} ladder must include later unlocks");
        }

        private static void ValidateSpellBalance(
            string code,
            int expectedUnlockLevel,
            int minimumMana,
            int maximumMana,
            int minimumPower,
            int maximumPower,
            string expectedStatus,
            bool expectedSplash)
        {
            FormulaDef formula = FormulaCatalog.All.FirstOrDefault(entry =>
                entry != null && string.Equals(entry.Code, code, StringComparison.OrdinalIgnoreCase));
            Require(formula != null, $"new progression formula is missing: {code}");
            Require(
                FormulaCatalog.RequiredLevel(formula) == expectedUnlockLevel,
                $"formula {code} unlock: expected level {expectedUnlockLevel}, got {FormulaCatalog.RequiredLevel(formula)}");
            Require(
                formula.Mana >= minimumMana && formula.Mana <= maximumMana,
                $"formula {code} mana must stay within {minimumMana}..{maximumMana}, got {formula.Mana}");
            Require(
                formula.Power >= minimumPower && formula.Power <= maximumPower,
                $"formula {code} power must stay within {minimumPower}..{maximumPower}, got {formula.Power}");
            Require(
                string.Equals(formula.Status ?? "", expectedStatus, StringComparison.OrdinalIgnoreCase),
                $"formula {code} status: expected '{expectedStatus}', got '{formula.Status ?? ""}'");
            Require(formula.Splash == expectedSplash, $"formula {code} splash: expected {expectedSplash}, got {formula.Splash}");

            if (!string.IsNullOrEmpty(expectedStatus))
            {
                Require(formula.Duration >= 1 && formula.Duration <= 3, $"formula {code} status duration must stay within 1..3 turns, got {formula.Duration}");
            }
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("Early progression rule smoke failed: " + message);
        }
    }
}
