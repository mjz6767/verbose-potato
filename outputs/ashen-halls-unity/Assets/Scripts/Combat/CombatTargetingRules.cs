using System;

namespace AshenHalls
{
    public static class CombatTargetingRules
    {
        public static bool CanCancel(
            CombatState combat,
            ActionMode selectedAction,
            string pendingFormulaCode,
            string pendingAbilityId)
        {
            if (combat == null || !combat.ActionAvailable || combat.Acted) return false;
            if (combat.Phase == CombatPhase.Resolving || combat.Phase == CombatPhase.EnemyThinking) return false;
            if (selectedAction == ActionMode.Cast) return !string.IsNullOrWhiteSpace(pendingFormulaCode);
            if (selectedAction == ActionMode.Ability) return !string.IsNullOrWhiteSpace(pendingAbilityId);
            return false;
        }

        public static string CancelLabel(ActionMode selectedAction)
        {
            if (selectedAction == ActionMode.Cast) return "Cancel Spell";
            if (selectedAction == ActionMode.Ability) return "Cancel Skill";
            return "Cancel Target";
        }
    }
}
