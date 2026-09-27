using System;

namespace AshenHalls
{
    public static class SkillProgressionRules
    {
        public const int AllPracticeBits = (1 << 6) - 1;
        public const int GuardRanksPerBonus = 30;
        public const int MaximumGuardTrainingBonus = 3;

        public static bool IsValidPracticeMask(int mask) => mask >= 0 && (mask & ~AllPracticeBits) == 0;

        public static int PracticeBit(string skill)
        {
            switch (skill)
            {
                case "arms": return 1;
                case "missile": return 1 << 1;
                case "mend": return 1 << 2;
                case "ember": return 1 << 3;
                case "hex": return 1 << 4;
                case "guard": return 1 << 5;
                default: return 0;
            }
        }

        public static bool TryConsumePractice(CombatUnit unit, string skill, int requestedAmount, int currentSkill)
        {
            int bit = PracticeBit(skill);
            if (unit == null || unit.Side != UnitSide.Party || unit.Summoned || unit.PartyIndex < 0
                || requestedAmount <= 0 || currentSkill >= PartyGrowthRules.MaximumValue || bit == 0
                || !IsValidPracticeMask(unit.PracticedSkillMask) || (unit.PracticedSkillMask & bit) != 0) return false;
            unit.PracticedSkillMask |= bit;
            return true;
        }

        public static int GuardActionTrainingBonus(int skill) => Math.Min(MaximumGuardTrainingBonus, Math.Max(0, skill) / GuardRanksPerBonus);

        public static int NextGuardTrainingThreshold(int skill)
        {
            int bonus = GuardActionTrainingBonus(skill);
            return bonus >= MaximumGuardTrainingBonus ? 0 : (bonus + 1) * GuardRanksPerBonus;
        }
    }
}
