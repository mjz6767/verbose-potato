using UnityEngine;

namespace AshenHalls
{
    public enum AttackForecastBlockReason
    {
        None,
        InvalidCombatants,
        FriendlyTarget,
        DefeatedTarget,
        OutOfRange,
        LineOfSight
    }

    public enum CombatThreatLevel
    {
        None,
        Pressure,
        Direct,
        Severe,
        Lethal
    }

    public readonly struct CombatAttackForecast
    {
        public readonly bool Legal;
        public readonly bool HasOutcome;
        public readonly AttackForecastBlockReason BlockReason;
        public readonly bool Ranged;
        public readonly bool HasLineOfSight;
        public readonly int Distance;
        public readonly int Range;
        public readonly int HitChance;
        public readonly int MinDamage;
        public readonly int MaxDamage;
        public readonly int ExpectedDamage;
        public readonly string DamageType;
        public readonly string DamageMatch;
        public readonly bool Guarded;
        public readonly CombatThreatLevel ThreatLevel;

        public CombatAttackForecast(
            bool legal,
            bool hasOutcome,
            AttackForecastBlockReason blockReason,
            bool ranged,
            bool hasLineOfSight,
            int distance,
            int range,
            int hitChance,
            int minDamage,
            int maxDamage,
            int expectedDamage,
            string damageType,
            string damageMatch,
            bool guarded,
            CombatThreatLevel threatLevel)
        {
            Legal = legal;
            HasOutcome = hasOutcome;
            BlockReason = blockReason;
            Ranged = ranged;
            HasLineOfSight = hasLineOfSight;
            Distance = Mathf.Max(0, distance);
            Range = Mathf.Max(1, range);
            HitChance = Mathf.Clamp(hitChance, 0, 100);
            MinDamage = Mathf.Max(0, minDamage);
            MaxDamage = Mathf.Max(MinDamage, maxDamage);
            ExpectedDamage = Mathf.Max(0, expectedDamage);
            DamageType = string.IsNullOrWhiteSpace(damageType) ? "physical" : damageType;
            DamageMatch = string.IsNullOrWhiteSpace(damageMatch) ? "normal" : damageMatch;
            Guarded = guarded;
            ThreatLevel = threatLevel;
        }
    }

    public static class CombatThreatRules
    {
        public static CombatAttackForecast Create(
            AttackForecastBlockReason blockReason,
            bool includeOutcome,
            bool ranged,
            bool hasLineOfSight,
            int distance,
            int range,
            int hitChance,
            int minDamage,
            int maxDamage,
            string damageType,
            string damageMatch,
            bool guarded,
            int targetHp,
            int targetMaxHp)
        {
            bool legal = blockReason == AttackForecastBlockReason.None;
            bool hasOutcome = legal && includeOutcome;
            int normalizedMin = hasOutcome ? Mathf.Max(1, minDamage) : 0;
            int normalizedMax = hasOutcome ? Mathf.Max(normalizedMin, maxDamage) : 0;
            int expectedDamage = hasOutcome ? ExpectedDamage(hitChance, normalizedMin, normalizedMax) : 0;
            CombatThreatLevel level = legal
                ? hasOutcome ? Classify(expectedDamage, normalizedMax, targetHp, targetMaxHp) : CombatThreatLevel.Direct
                : CombatThreatLevel.None;

            return new CombatAttackForecast(
                legal,
                hasOutcome,
                blockReason,
                ranged,
                hasLineOfSight,
                distance,
                range,
                hasOutcome ? hitChance : 0,
                normalizedMin,
                normalizedMax,
                expectedDamage,
                damageType,
                damageMatch,
                guarded,
                level);
        }

        public static int ExpectedDamage(int hitChance, int minDamage, int maxDamage)
        {
            int low = Mathf.Max(0, minDamage);
            int high = Mathf.Max(low, maxDamage);
            float average = (low + high) * 0.5f;
            return Mathf.Max(0, Mathf.RoundToInt(average * Mathf.Clamp(hitChance, 0, 100) / 100f));
        }

        public static CombatThreatLevel Classify(int expectedDamage, int maxDamage, int targetHp, int targetMaxHp)
        {
            int hp = Mathf.Max(1, targetHp);
            int maxHp = Mathf.Max(hp, targetMaxHp);
            if (maxDamage >= hp || expectedDamage >= hp) return CombatThreatLevel.Lethal;
            if (maxDamage * 2 >= hp || expectedDamage * 3 >= hp || maxDamage * 2 >= maxHp) return CombatThreatLevel.Severe;
            return CombatThreatLevel.Direct;
        }

        public static string SeverityLabel(CombatThreatLevel level)
        {
            switch (level)
            {
                case CombatThreatLevel.Lethal: return "LETHAL";
                case CombatThreatLevel.Severe: return "HIGH THREAT";
                case CombatThreatLevel.Direct: return "THREAT";
                case CombatThreatLevel.Pressure: return "PRESSURE";
                default: return "CLEAR";
            }
        }

        public static string MovementDestinationLabel(int directThreats, int pressureThreats)
        {
            int direct = Mathf.Max(0, directThreats);
            int pressure = Mathf.Max(0, pressureThreats);
            if (direct > 0 && pressure > 0) return $"threat: {direct} can hit + {pressure} can reach";
            if (direct > 0) return direct == 1 ? "threat: 1 can hit" : $"threat: {direct} can hit";
            if (pressure > 0) return pressure == 1 ? "threat: 1 can reach" : $"threat: {pressure} can reach";
            return "safe";
        }

        public static string BlockLabel(AttackForecastBlockReason reason)
        {
            switch (reason)
            {
                case AttackForecastBlockReason.FriendlyTarget: return "friendly target";
                case AttackForecastBlockReason.DefeatedTarget: return "target defeated";
                case AttackForecastBlockReason.OutOfRange: return "out of range";
                case AttackForecastBlockReason.LineOfSight: return "line of sight blocked";
                case AttackForecastBlockReason.InvalidCombatants: return "no valid target";
                default: return "ready";
            }
        }
    }
}
