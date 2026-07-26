using System;

namespace AshenHalls
{
    public static class LightningPowerRules
    {
        public const int MaximumChainTargets = 4;
        public const int NormalJumpRange = 2;
        public const int ConductiveJumpRange = 3;
        public const int TempestRadius = 2;

        public static int ChainDamage(int baseDamage, int targetIndex)
        {
            int safeBase = Math.Max(1, baseDamage);
            switch (Math.Max(0, targetIndex))
            {
                case 0: return safeBase;
                case 1: return Math.Max(3, (int)Math.Round(safeBase * 0.75f));
                case 2: return Math.Max(3, (int)Math.Round(safeBase * 0.55f));
                default: return Math.Max(3, (int)Math.Round(safeBase * 0.40f));
            }
        }

        public static int ThunderclapDamage(int baseDamage)
        {
            return Math.Max(3, (int)Math.Round(Math.Max(1, baseDamage) * 0.50f));
        }

        public static int ThunderStepDamage(int baseDamage)
        {
            return Math.Max(3, (int)Math.Round(Math.Max(1, baseDamage) * 0.50f));
        }

        public static int TempestDamage(int baseDamage, bool center)
        {
            return center
                ? Math.Max(1, baseDamage)
                : Math.Max(4, (int)Math.Round(Math.Max(1, baseDamage) * 0.60f));
        }

        public static int CollisionDamage(int baseDamage)
        {
            return Math.Max(4, (int)Math.Round(Math.Max(1, baseDamage) * 0.50f));
        }

        public static bool IsConductiveTerrain(string kind)
        {
            return string.Equals(kind, "ice", StringComparison.OrdinalIgnoreCase)
                || string.Equals(kind, "gas", StringComparison.OrdinalIgnoreCase)
                || string.Equals(kind, "web", StringComparison.OrdinalIgnoreCase);
        }
    }
}
