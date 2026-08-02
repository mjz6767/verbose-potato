using System;

namespace AshenHalls
{
    public static class ProgressionRules
    {
        public const int MinimumLevel = 1;
        public const int MaximumLevel = 20;

        public static int ClampLevel(int level)
        {
            return Math.Max(MinimumLevel, Math.Min(MaximumLevel, level));
        }

        public static bool IsMaximumLevel(int level)
        {
            return level >= MaximumLevel;
        }

        public static bool IsValidUnlockLevel(int level)
        {
            return level >= MinimumLevel && level <= MaximumLevel;
        }

        public static int ExperienceForNextLevel(int level)
        {
            level = ClampLevel(level);
            return IsMaximumLevel(level) ? 0 : 60 + level * 40;
        }

        public static int NormalizeExperience(int level, int experience)
        {
            return IsMaximumLevel(level) ? 0 : Math.Max(0, experience);
        }

        public static int SkillPointRewardForLevel(int reachedLevel)
        {
            return reachedLevel > MinimumLevel && reachedLevel <= MaximumLevel ? 2 : 0;
        }

        public static int StatPointRewardForLevel(int reachedLevel)
        {
            if (reachedLevel <= MinimumLevel || reachedLevel > MaximumLevel) return 0;
            return reachedLevel % 2 == 0 ? 2 : 1;
        }

        public static int CumulativeExperienceToLevel(int targetLevel)
        {
            targetLevel = ClampLevel(targetLevel);
            int total = 0;
            for (int level = MinimumLevel; level < targetLevel; level++)
            {
                total += ExperienceForNextLevel(level);
            }
            return total;
        }
    }
}
