namespace AshenHalls
{
    public static class CombatRetreatRules
    {
        public const int SupplyCost = 1;

        public static bool CanOffer(GameState state, bool labSaveBlocked, bool betaLabMode)
        {
            return state != null
                && state.Mode == GameMode.Combat
                && state.Combat != null
                && !labSaveBlocked
                && !betaLabMode;
        }

        public static bool CanAfford(GameState state)
        {
            return state != null && state.Supplies >= SupplyCost;
        }
    }
}
