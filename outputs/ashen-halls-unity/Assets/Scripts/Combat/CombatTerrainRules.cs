namespace AshenHalls
{
    public static class CombatTerrainRules
    {
        public static bool BlocksMovement(string kind)
        {
            string normalized = Normalize(kind);
            return normalized == "tree" || normalized == "stone";
        }

        public static bool BlocksSight(string kind)
        {
            return BlocksMovement(kind) || Normalize(kind) == "smoke";
        }

        private static string Normalize(string value)
        {
            return (value ?? "").Trim().ToLowerInvariant();
        }
    }
}
