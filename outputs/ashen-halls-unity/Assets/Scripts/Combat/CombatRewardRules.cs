using System;
using System.Collections.Generic;

namespace AshenHalls
{
    public static class CombatRewardRules
    {
        public static bool IsEncounterRewardEnemy(CombatUnit unit)
        {
            // Breaches are a tactical threat, not a renewable source of loot or XP.
            // Both persisted markers are checked for older in-progress battles.
            return unit != null
                && unit.Side == UnitSide.Enemy
                && !unit.Summoned
                && !IsRitualMarker(unit.Origin)
                && !IsRitualMarker(unit.Rank);
        }

        public static int ExperienceForVictory(IEnumerable<CombatUnit> units, int partySize)
        {
            if (units == null) return 0;
            int reward = 0;
            bool hasEncounterEnemy = false;
            foreach (CombatUnit enemy in units)
            {
                if (!IsEncounterRewardEnemy(enemy)) continue;
                hasEncounterEnemy = true;
                reward += Math.Max(6, enemy.MaxHp / 2 + enemy.Power * 2 + enemy.Defense * 3 + enemy.Range * 2);
                if (enemy.Rank == "veteran") reward += 12;
                if (enemy.Rank == "elite") reward += 28;
                if (EnemyTacticsRules.IsCaster(enemy)) reward += 10;
            }
            return hasEncounterEnemy ? Math.Max(12, reward / Math.Max(1, partySize)) : 0;
        }

        private static bool IsRitualMarker(string value)
        {
            return string.Equals((value ?? "").Trim(), "ritual", StringComparison.OrdinalIgnoreCase);
        }
    }
}
