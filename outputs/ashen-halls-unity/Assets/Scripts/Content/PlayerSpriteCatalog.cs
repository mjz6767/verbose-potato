namespace AshenHalls
{
    public static class PlayerSpriteCatalog
    {
        public const int Columns = 5;
        public const int Rows = 7;

        public static int AtlasIndex(string classKey, string race)
        {
            int classRow = ClassRow(classKey);
            if (classRow < 0) return -1;
            return classRow * Columns + RaceColumn(race);
        }

        private static int ClassRow(string classKey)
        {
            switch ((classKey ?? "").Trim().ToLowerInvariant())
            {
                case "warrior": return 0;
                case "rogue": return 1;
                case "ranger": return 2;
                case "priest": return 3;
                case "warlock": return 4;
                case "wizard":
                case "mage":
                    return 5;
                case "paladin": return 6;
                default: return -1;
            }
        }

        private static int RaceColumn(string race)
        {
            switch ((race ?? "").Trim().ToLowerInvariant())
            {
                case "dusk elf": return 1;
                case "stoneborn": return 2;
                case "fenkin": return 3;
                case "ashling": return 4;
                case "human":
                default:
                    return 0;
            }
        }
    }
}
