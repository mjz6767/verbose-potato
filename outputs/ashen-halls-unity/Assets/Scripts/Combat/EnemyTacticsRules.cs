using System;
using UnityEngine;

namespace AshenHalls
{
    public enum EnemyTacticsArchetype
    {
        Skirmisher,
        Brute,
        Marksman,
        Caster,
        Support,
        Boss
    }

    public readonly struct EnemyTacticsProfile
    {
        public readonly EnemyTacticsArchetype Archetype;
        public readonly int PreferredRange;
        public readonly int RangeDiscipline;
        public readonly int CloseRangePenalty;
        public readonly int SightLineBonus;
        public readonly int HazardCautionPercent;
        public readonly bool CanCast;
        public readonly bool Brute;
        public readonly bool StepAndAttack;

        public EnemyTacticsProfile(
            EnemyTacticsArchetype archetype,
            int preferredRange,
            int rangeDiscipline,
            int closeRangePenalty,
            int sightLineBonus,
            int hazardCautionPercent,
            bool canCast,
            bool brute,
            bool stepAndAttack)
        {
            Archetype = archetype;
            PreferredRange = Mathf.Max(1, preferredRange);
            RangeDiscipline = Mathf.Max(1, rangeDiscipline);
            CloseRangePenalty = Mathf.Max(0, closeRangePenalty);
            SightLineBonus = Mathf.Max(0, sightLineBonus);
            HazardCautionPercent = Mathf.Clamp(hazardCautionPercent, 40, 140);
            CanCast = canCast;
            Brute = brute;
            StepAndAttack = stepAndAttack;
        }
    }

    public static class EnemyTacticsRules
    {
        public static EnemyTacticsProfile For(CombatUnit enemy)
        {
            if (enemy == null)
            {
                return Profile(EnemyTacticsArchetype.Skirmisher, 1, 4, 0, 0, 100, false, false, false);
            }

            string role = Normalize(enemy.Role);
            bool canCast = IsCaster(enemy);
            bool brute = IsBrute(enemy);
            int range = Mathf.Max(1, enemy.Range);

            if (IsBossRole(role))
            {
                int preferred = range > 1 ? Mathf.Max(2, range - 1) : 1;
                return Profile(EnemyTacticsArchetype.Boss, preferred, 7, range > 1 ? 24 : 0, 20, 55, canCast, brute, true);
            }
            if (IsSupportRole(role))
            {
                return Profile(EnemyTacticsArchetype.Support, Mathf.Max(3, range), 8, 38, 18, 115, true, false, true);
            }
            if (IsMarksmanRole(role))
            {
                return Profile(EnemyTacticsArchetype.Marksman, Mathf.Max(3, range), 7, 32, 22, 105, false, false, true);
            }
            if (canCast)
            {
                int preferred = range > 1 ? Mathf.Max(2, range - 1) : 1;
                return Profile(EnemyTacticsArchetype.Caster, preferred, 8, range > 1 ? 36 : 0, 18, 90, true, false, range > 1);
            }
            if (brute)
            {
                return Profile(EnemyTacticsArchetype.Brute, 1, 4, 0, 0, 60, false, true, true);
            }
            return Profile(EnemyTacticsArchetype.Skirmisher, 1, 5, 0, 0, 85, false, false, false);
        }

        public static bool IsCaster(CombatUnit enemy)
        {
            if (enemy == null) return false;
            string role = Normalize(enemy.Role);
            return role == "adept"
                || role == "glassmage"
                || role == "bonepriest"
                || role == "ratmage"
                || role == "ratcleric"
                || role == "drowmage"
                || role == "drowpriest"
                || role == "cinderling"
                || role == "spore"
                || role == "koboldking"
                || role == "koboldshaman"
                || role == "koboldwizard"
                || role == "meteorlich"
                || role == "ritualheart"
                || IsMagicalDamage(enemy.DamageType);
        }

        public static bool IsBrute(CombatUnit enemy)
        {
            if (enemy == null) return false;
            string role = Normalize(enemy.Role);
            return role == "husk"
                || role == "thornbeast"
                || role == "gloamknight"
                || role == "ratbrute"
                || role == "lesserdemon"
                || role == "koboldking"
                || enemy.Range <= 1 && enemy.Defense >= 4;
        }

        public static bool CanAttackAfterMove(CombatUnit enemy, int spentMoveCost)
        {
            if (enemy == null) return false;
            if (spentMoveCost <= 0) return true;
            if (spentMoveCost > 1 || enemy.Webbed > 0 || enemy.Stunned > 0 || enemy.Sleeping > 0) return false;

            EnemyTacticsProfile profile = For(enemy);
            if (profile.StepAndAttack || string.Equals(enemy.Rank, "elite", StringComparison.OrdinalIgnoreCase)) return true;
            return profile.Archetype == EnemyTacticsArchetype.Skirmisher
                && string.Equals(enemy.Rank, "veteran", StringComparison.OrdinalIgnoreCase);
        }

        public static bool ShouldRepositionBeforeAttack(CombatUnit enemy, int distance, int terrainRisk)
        {
            if (enemy == null || enemy.Webbed > 0 || enemy.Range <= 1) return false;
            EnemyTacticsProfile profile = For(enemy);
            if (terrainRisk >= 14) return true;
            if (distance <= 1) return true;
            return distance + 1 < profile.PreferredRange;
        }

        public static int PositionAdjustment(CombatUnit enemy, int distance, int moveCost, bool hasSight, bool specialArcs)
        {
            if (enemy == null) return 0;
            EnemyTacticsProfile profile = For(enemy);
            if (enemy.Range > 1)
            {
                int score = Mathf.Abs(distance - profile.PreferredRange) * profile.RangeDiscipline;
                score += hasSight ? -profile.SightLineBonus : specialArcs ? 5 : 24;
                if (distance <= 1) score += profile.CloseRangePenalty;
                if (profile.Archetype == EnemyTacticsArchetype.Support && distance <= 2) score += 10;
                return score;
            }

            if (distance > 1) return 0;
            int meleeBonus = profile.Brute ? 22 : 14;
            if (profile.Brute && moveCost <= 1) meleeBonus += 12;
            return -meleeBonus;
        }

        public static int TerrainRisk(CombatUnit enemy, string terrainKind, int moveExtraCost)
        {
            if (enemy == null || string.IsNullOrWhiteSpace(terrainKind)) return 0;
            string kind = Normalize(terrainKind);
            int risk;
            switch (kind)
            {
                case "fire": risk = 34; break;
                case "gas": risk = 30; break;
                case "curse": risk = 20; break;
                case "web": risk = 18; break;
                case "ice": risk = 16; break;
                case "glyph":
                case "demonrift": risk = 10; break;
                case "sanctuary": risk = enemy.Side == UnitSide.Enemy ? 22 : -8; break;
                default: risk = Mathf.Max(0, moveExtraCost) * 8; break;
            }

            EnemyTacticsProfile profile = For(enemy);
            risk = Mathf.RoundToInt(risk * profile.HazardCautionPercent / 100f);
            string hazardType = HazardDamageType(kind);
            if (HasTag(enemy.Resist, hazardType)) risk -= 9;
            if (HasTag(enemy.Weakness, hazardType)) risk += 12;
            return Mathf.Max(kind == "sanctuary" && enemy.Side != UnitSide.Enemy ? -8 : 0, risk);
        }

        public static int TargetPriorityAdjustment(CombatUnit enemy, CombatUnit target)
        {
            if (enemy == null || target == null) return 0;
            EnemyTacticsProfile profile = For(enemy);
            float healthRatio = target.Hp / (float)Mathf.Max(1, target.MaxHp);
            int score = Mathf.RoundToInt(healthRatio * 20f);
            if (target.Guarding) score += Mathf.Max(8, target.GuardBonus * 5);
            if (target.Stealthed > 0) score += 34 + target.Stealthed * 8;

            bool spellUser = IsSpellUser(target);
            if (profile.CanCast && spellUser) score -= 16;
            if (profile.Archetype == EnemyTacticsArchetype.Brute && (spellUser || IsRangedHero(target))) score -= 10;
            if (profile.Archetype == EnemyTacticsArchetype.Marksman)
            {
                if (spellUser) score -= 12;
                score += Mathf.Clamp(target.Defense + target.ArmorBonus, 0, 8);
            }
            if (profile.Archetype == EnemyTacticsArchetype.Skirmisher && healthRatio <= 0.5f) score -= 7;
            if (profile.Archetype == EnemyTacticsArchetype.Support && spellUser) score -= 6;
            if (HasTag(target.Weakness, enemy.DamageType)) score -= 10;
            if (HasTag(target.Resist, enemy.DamageType)) score += 12;
            return score;
        }

        public static string StyleLabel(CombatUnit enemy)
        {
            switch (For(enemy).Archetype)
            {
                case EnemyTacticsArchetype.Brute: return "Brute";
                case EnemyTacticsArchetype.Marksman: return "Marksman";
                case EnemyTacticsArchetype.Caster: return "Caster";
                case EnemyTacticsArchetype.Support: return "Support";
                case EnemyTacticsArchetype.Boss: return "Boss";
                default: return "Skirmisher";
            }
        }

        public static string AttackIntent(CombatUnit enemy)
        {
            switch (For(enemy).Archetype)
            {
                case EnemyTacticsArchetype.Brute: return "Close and crush";
                case EnemyTacticsArchetype.Marksman: return "Loose a ranged shot";
                case EnemyTacticsArchetype.Caster: return "Cast pressure";
                case EnemyTacticsArchetype.Support: return "Ward and hex";
                case EnemyTacticsArchetype.Boss: return "Command the field";
                default: return "Strike an opening";
            }
        }

        public static string AdvanceIntent(CombatUnit enemy, bool hasSight)
        {
            switch (For(enemy).Archetype)
            {
                case EnemyTacticsArchetype.Brute: return "Close and crush";
                case EnemyTacticsArchetype.Marksman: return hasSight ? "Keep firing distance" : "Seek a firing lane";
                case EnemyTacticsArchetype.Caster: return hasSight ? "Keep casting distance" : "Seek a spell lane";
                case EnemyTacticsArchetype.Support: return hasSight ? "Hold the support line" : "Find a support lane";
                case EnemyTacticsArchetype.Boss: return "Control the field";
                default: return "Circle and close";
            }
        }

        private static EnemyTacticsProfile Profile(
            EnemyTacticsArchetype archetype,
            int preferredRange,
            int rangeDiscipline,
            int closeRangePenalty,
            int sightLineBonus,
            int hazardCautionPercent,
            bool canCast,
            bool brute,
            bool stepAndAttack)
        {
            return new EnemyTacticsProfile(
                archetype,
                preferredRange,
                rangeDiscipline,
                closeRangePenalty,
                sightLineBonus,
                hazardCautionPercent,
                canCast,
                brute,
                stepAndAttack);
        }

        private static bool IsBossRole(string role)
        {
            return role == "koboldking" || role == "meteorlich" || role == "ritualheart";
        }

        private static bool IsSupportRole(string role)
        {
            return role == "bonepriest" || role == "ratcleric" || role == "drowpriest";
        }

        private static bool IsMarksmanRole(string role)
        {
            return role == "koboldslinger" || role == "mirearcher" || role == "drowcrossbow";
        }

        private static bool IsMagicalDamage(string damageType)
        {
            string type = Normalize(damageType);
            return type == "shock" || type == "cold" || type == "mind" || type == "death";
        }

        private static bool IsSpellUser(CombatUnit unit)
        {
            if (unit == null) return false;
            string role = Normalize(unit.Role);
            string classKey = Normalize(unit.ClassKey);
            return !string.IsNullOrWhiteSpace(unit.Spell)
                || role == "mend"
                || role == "ember"
                || role == "hex"
                || classKey == "mage"
                || classKey == "priest"
                || classKey == "warlock";
        }

        private static bool IsRangedHero(CombatUnit unit)
        {
            if (unit == null) return false;
            string role = Normalize(unit.Role);
            return unit.Range > 1 || role == "bow" || Normalize(unit.ClassKey) == "ranger";
        }

        private static string HazardDamageType(string terrainKind)
        {
            switch (terrainKind)
            {
                case "fire": return "fire";
                case "gas": return "poison";
                case "ice": return "cold";
                case "curse":
                case "glyph":
                case "demonrift": return "death";
                default: return "";
            }
        }

        private static bool HasTag(string tags, string tag)
        {
            if (string.IsNullOrWhiteSpace(tags) || string.IsNullOrWhiteSpace(tag)) return false;
            string[] parts = tags.Split('|');
            for (int i = 0; i < parts.Length; i++)
            {
                if (string.Equals(parts[i], tag, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }

        private static string Normalize(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "" : value.Trim().ToLowerInvariant();
        }
    }
}
