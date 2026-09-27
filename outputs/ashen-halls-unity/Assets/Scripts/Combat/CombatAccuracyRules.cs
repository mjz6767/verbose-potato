using System;

namespace AshenHalls
{
    public static class CombatAccuracyRules
    {
        // Early training keeps its original value. Later mastery continues to
        // improve accuracy without erasing defense and deliberate Guard.
        // Damage and practice still use the actual skill value.
        public static int SkillContribution(int skill)
        {
            int value = Math.Max(0, Math.Min(99, skill));
            return value <= 25 ? value : 25 + (value - 25) / 3;
        }
    }
}
