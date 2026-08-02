namespace AshenHalls
{
    public static class WorldAreaSetpiecePresentationRules
    {
        public const int Columns = 4;
        public const int Rows = 2;
        public const int CellCount = Columns * Rows;

        public static int IconIndex(string siteId)
        {
            switch (siteId ?? "")
            {
                case "green-shrine-training-ring": return 0;
                case "old-quarry-forge": return 1;
                case "gloam-deep-crypt": return 2;
                case "glass-lore-library": return 3;
                case "dusk-market-hideout": return 4;
                case "red-gate-seal": return 5;
                case "salt-cistern-gate": return 6;
                case "ash-fen-ancient-grove": return 7;
                default: return -1;
            }
        }

        public static float MapScale(bool wideView)
        {
            return wideView ? 2.05f : 2.40f;
        }

        public static float BaselineFraction(bool wideView)
        {
            return wideView ? 0.78f : 0.82f;
        }
    }
}
