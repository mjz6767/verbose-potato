using UnityEngine;

namespace AshenHalls
{
    public static class CombatPowerResolutionRules
    {
        public static float DelayForFormula(FormulaDef formula, bool reducedMotion)
        {
            return reducedMotion ? 0.06f : CombatImpactRules.ForFormula(formula).ResolutionDelay;
        }

        public static float DelayForAbility(MartialAbility ability, bool reducedMotion)
        {
            return reducedMotion ? 0.06f : CombatImpactRules.ForAbility(ability).ResolutionDelay;
        }

        public static float DelayForEnemyPower(string powerKey, bool reducedMotion)
        {
            return reducedMotion ? 0.06f : CombatImpactRules.ForEnemyPower(powerKey).ResolutionDelay;
        }

        public static float DelayForIntensity(int intensity, bool reducedMotion)
        {
            if (reducedMotion) return 0.06f;
            switch (Mathf.Clamp(intensity, 1, 3))
            {
                case 3: return 0.52f;
                case 2: return 0.36f;
                default: return 0.22f;
            }
        }
    }
}
