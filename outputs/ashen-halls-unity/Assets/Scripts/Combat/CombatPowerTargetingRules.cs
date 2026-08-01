namespace AshenHalls
{
    public enum CombatPowerFootprintKind
    {
        Single,
        CrossArea,
        Chain,
        RadiusArea,
        Placement,
        ChargeLanding,
        SecondaryStrike,
        SelfArea
    }

    public readonly struct CombatPowerTargetingProfile
    {
        public readonly CombatPowerFootprintKind Kind;
        public readonly string BoardLabel;
        public readonly string ModalLabel;

        public CombatPowerTargetingProfile(CombatPowerFootprintKind kind, string boardLabel, string modalLabel)
        {
            Kind = kind;
            BoardLabel = boardLabel ?? "";
            ModalLabel = modalLabel ?? "";
        }
    }

    public static class CombatPowerTargetingRules
    {
        public static CombatPowerTargetingProfile ForFormula(FormulaDef formula)
        {
            if (formula == null) return SingleTarget();
            switch ((formula.Code ?? "").ToUpperInvariant())
            {
                case "RSG":
                    return new CombatPowerTargetingProfile(CombatPowerFootprintKind.SelfArea, "PUSH", "Adjacent enemies");
                case "CLT":
                    return new CombatPowerTargetingProfile(CombatPowerFootprintKind.Chain, "CHAIN", "Up to 4 jumps");
                case "VST":
                    return new CombatPowerTargetingProfile(CombatPowerFootprintKind.Placement, "STEP", "Step + landing arc");
                case "AST":
                    return new CombatPowerTargetingProfile(CombatPowerFootprintKind.RadiusArea, "STORM", "Radius 2");
            }
            if (formula.Splash) return new CombatPowerTargetingProfile(CombatPowerFootprintKind.CrossArea, "AREA", "Cross area");
            if (formula.Effect == "summon") return new CombatPowerTargetingProfile(CombatPowerFootprintKind.Placement, "BIND", "Summon tile");
            if (formula.Effect == "terrain") return new CombatPowerTargetingProfile(CombatPowerFootprintKind.Placement, "FIELD", "Placed field");
            if (formula.Effect == "dispel") return new CombatPowerTargetingProfile(CombatPowerFootprintKind.Placement, "SEAL", "Ritual or hostile field");
            if (formula.Effect == "teleport") return new CombatPowerTargetingProfile(CombatPowerFootprintKind.Placement, "STEP", "Teleport destination");
            if (formula.Target == "self") return new CombatPowerTargetingProfile(CombatPowerFootprintKind.SelfArea, "SELF", "Self");
            return SingleTarget();
        }

        public static CombatPowerTargetingProfile ForAbility(MartialAbility ability)
        {
            if (ability == null) return SingleTarget();
            switch ((ability.Id ?? "").ToLowerInvariant())
            {
                case "charge":
                    return new CombatPowerTargetingProfile(CombatPowerFootprintKind.ChargeLanding, "LAND", "Rush + landing");
                case "riftpounce":
                    return new CombatPowerTargetingProfile(CombatPowerFootprintKind.ChargeLanding, "RIFT", "Rift + landing");
                case "cleave":
                    return new CombatPowerTargetingProfile(CombatPowerFootprintKind.SecondaryStrike, "2ND", "Primary + secondary");
                case "volley":
                    return new CombatPowerTargetingProfile(CombatPowerFootprintKind.CrossArea, "AREA", "Cross area");
                case "whirlwind":
                case "abyssalwhirl":
                    return new CombatPowerTargetingProfile(CombatPowerFootprintKind.SelfArea, "ALL", "Adjacent enemies");
                case "dreadroar":
                    return new CombatPowerTargetingProfile(CombatPowerFootprintKind.SelfArea, "DREAD", "Adjacent enemies");
                case "rally":
                    return new CombatPowerTargetingProfile(CombatPowerFootprintKind.SelfArea, "AURA", "Self + adjacent allies");
                case "smokebomb":
                    return new CombatPowerTargetingProfile(CombatPowerFootprintKind.SelfArea, "SMOKE", "Adjacent field");
                case "stealth":
                    return new CombatPowerTargetingProfile(CombatPowerFootprintKind.SelfArea, "SELF", "Self");
                default:
                    return SingleTarget();
            }
        }

        private static CombatPowerTargetingProfile SingleTarget()
        {
            return new CombatPowerTargetingProfile(CombatPowerFootprintKind.Single, "TARGET", "Single target");
        }
    }
}
