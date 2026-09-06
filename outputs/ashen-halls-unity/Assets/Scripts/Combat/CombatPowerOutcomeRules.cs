using System;
using System.Collections.Generic;

namespace AshenHalls
{
    public sealed class CombatPowerOutcomeSnapshot
    {
        internal readonly Dictionary<string, CombatPowerUnitSnapshot> Units =
            new Dictionary<string, CombatPowerUnitSnapshot>(StringComparer.Ordinal);

        internal readonly Dictionary<string, string> Terrain =
            new Dictionary<string, string>(StringComparer.Ordinal);
    }

    internal sealed class CombatPowerUnitSnapshot
    {
        public int Hp;
        public bool Summoned;
        public int[] Statuses;
    }

    public readonly struct CombatPowerOutcome
    {
        public readonly int Damage;
        public readonly int Healing;
        public readonly int AffectedUnits;
        public readonly int DefeatedUnits;
        public readonly int StatusesApplied;
        public readonly int AilmentsCleared;
        public readonly int SummonsBound;
        public readonly int TerrainChanges;

        public CombatPowerOutcome(
            int damage,
            int healing,
            int affectedUnits,
            int defeatedUnits,
            int statusesApplied,
            int ailmentsCleared,
            int summonsBound,
            int terrainChanges)
        {
            Damage = Math.Max(0, damage);
            Healing = Math.Max(0, healing);
            AffectedUnits = Math.Max(0, affectedUnits);
            DefeatedUnits = Math.Max(0, defeatedUnits);
            StatusesApplied = Math.Max(0, statusesApplied);
            AilmentsCleared = Math.Max(0, ailmentsCleared);
            SummonsBound = Math.Max(0, summonsBound);
            TerrainChanges = Math.Max(0, terrainChanges);
        }

        public string Summary => CombatPowerOutcomeRules.Format(this);
    }

    public static class CombatPowerOutcomeRules
    {
        // StatusValues stores harmful effects first, followed by beneficial ones.
        private const int AilmentStatusCount = 6;

        public static CombatPowerOutcomeSnapshot Capture(CombatState combat)
        {
            CombatPowerOutcomeSnapshot snapshot = new CombatPowerOutcomeSnapshot();
            if (combat == null) return snapshot;

            if (combat.Units != null)
            {
                for (int i = 0; i < combat.Units.Count; i++)
                {
                    CombatUnit unit = combat.Units[i];
                    if (unit == null) continue;
                    string key = string.IsNullOrWhiteSpace(unit.Id) ? "unit:" + i : unit.Id;
                    snapshot.Units[key] = new CombatPowerUnitSnapshot
                    {
                        Hp = unit.Hp,
                        Summoned = unit.Summoned,
                        Statuses = StatusValues(unit)
                    };
                }
            }

            if (combat.Obstacles != null)
            {
                foreach (Point point in combat.Obstacles)
                {
                    if (point == null) continue;
                    snapshot.Terrain[TerrainKey(point)] = TerrainSignature(point);
                }
            }

            return snapshot;
        }

        public static CombatPowerOutcome Compare(CombatPowerOutcomeSnapshot before, CombatState afterCombat)
        {
            before = before ?? new CombatPowerOutcomeSnapshot();
            CombatPowerOutcomeSnapshot after = Capture(afterCombat);
            int damage = 0;
            int healing = 0;
            int defeated = 0;
            int statusesApplied = 0;
            int ailmentsCleared = 0;
            int summonsBound = 0;
            HashSet<string> affected = new HashSet<string>(StringComparer.Ordinal);

            foreach (KeyValuePair<string, CombatPowerUnitSnapshot> pair in before.Units)
            {
                if (!after.Units.TryGetValue(pair.Key, out CombatPowerUnitSnapshot next)) continue;
                CombatPowerUnitSnapshot prior = pair.Value;
                if (next.Hp < prior.Hp)
                {
                    damage += prior.Hp - next.Hp;
                    affected.Add(pair.Key);
                }
                else if (next.Hp > prior.Hp)
                {
                    healing += next.Hp - prior.Hp;
                    affected.Add(pair.Key);
                }

                if (prior.Hp > 0 && next.Hp <= 0) defeated++;
                int statusCount = Math.Min(prior.Statuses?.Length ?? 0, next.Statuses?.Length ?? 0);
                for (int i = 0; i < statusCount; i++)
                {
                    if (next.Statuses[i] > prior.Statuses[i])
                    {
                        statusesApplied++;
                        affected.Add(pair.Key);
                    }
                    else if (prior.Statuses[i] > 0 && next.Statuses[i] < prior.Statuses[i])
                    {
                        if (i < AilmentStatusCount) ailmentsCleared++;
                        affected.Add(pair.Key);
                    }
                }
            }

            foreach (KeyValuePair<string, CombatPowerUnitSnapshot> pair in after.Units)
            {
                if (!before.Units.ContainsKey(pair.Key) && pair.Value.Summoned && pair.Value.Hp > 0)
                {
                    summonsBound++;
                }
            }

            HashSet<string> terrainCells = new HashSet<string>(before.Terrain.Keys, StringComparer.Ordinal);
            terrainCells.UnionWith(after.Terrain.Keys);
            int terrainChanges = 0;
            foreach (string cell in terrainCells)
            {
                before.Terrain.TryGetValue(cell, out string prior);
                after.Terrain.TryGetValue(cell, out string next);
                if (!string.Equals(prior, next, StringComparison.Ordinal)) terrainChanges++;
            }

            return new CombatPowerOutcome(
                damage,
                healing,
                affected.Count,
                defeated,
                statusesApplied,
                ailmentsCleared,
                summonsBound,
                terrainChanges);
        }

        public static string Format(CombatPowerOutcome outcome)
        {
            List<string> parts = new List<string>(4);
            if (outcome.Damage > 0) parts.Add(outcome.Damage + " damage");
            if (outcome.Healing > 0) parts.Add(outcome.Healing + " restored");
            if (outcome.SummonsBound > 0) parts.Add(CountLabel(outcome.SummonsBound, "summon bound", "summons bound"));
            if (outcome.TerrainChanges > 0) parts.Add(outcome.TerrainChanges == 1 ? "field shaped" : outcome.TerrainChanges + " fields shaped");
            if (outcome.AffectedUnits > 1) parts.Add(outcome.AffectedUnits + " targets");
            if (outcome.DefeatedUnits > 0) parts.Add(CountLabel(outcome.DefeatedUnits, "defeated", "defeated"));
            if (outcome.StatusesApplied > 0) parts.Add(CountLabel(outcome.StatusesApplied, "effect applied", "effects applied"));
            if (outcome.AilmentsCleared > 0) parts.Add(CountLabel(outcome.AilmentsCleared, "ailment cleared", "ailments cleared"));
            if (parts.Count == 0) return "Power resolved";
            if (parts.Count > 4) parts.RemoveRange(4, parts.Count - 4);
            return string.Join(" / ", parts);
        }

        public static string FormatWithReactions(CombatPowerOutcome outcome, IEnumerable<string> reactions)
        {
            string summary = Format(outcome);
            List<string> distinct = new List<string>(2);
            if (reactions != null)
            {
                foreach (string reaction in reactions)
                {
                    string clean = (reaction ?? "").Trim();
                    if (clean.Length == 0 || distinct.Exists(value => string.Equals(value, clean, StringComparison.OrdinalIgnoreCase))) continue;
                    distinct.Add(clean);
                    if (distinct.Count >= 2) break;
                }
            }

            if (distinct.Count == 0) return summary;
            string reactionSummary = string.Join(" + ", distinct);
            return summary == "Power resolved" ? reactionSummary : reactionSummary + " / " + summary;
        }

        private static int[] StatusValues(CombatUnit unit)
        {
            return new[]
            {
                unit.Poisoned,
                unit.Bleeding,
                unit.Stunned,
                unit.Sleeping,
                unit.Webbed,
                unit.Hexed,
                unit.Shielded,
                unit.Regenerating,
                unit.Stealthed
            };
        }

        private static string TerrainKey(Point point)
        {
            return point.X + "," + point.Y;
        }

        private static string TerrainSignature(Point point)
        {
            return (point.Kind ?? "").ToLowerInvariant() + ":" + point.Duration + ":" + point.Integrity;
        }

        private static string CountLabel(int count, string singular, string plural)
        {
            return count + " " + (count == 1 ? singular : plural);
        }
    }
}
