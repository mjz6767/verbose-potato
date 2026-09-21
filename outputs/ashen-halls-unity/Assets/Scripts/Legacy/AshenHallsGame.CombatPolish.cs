using UnityEngine;

namespace AshenHalls
{
    public sealed partial class AshenHallsGame
    {
        // Presentation uses the same prerequisites as the command that will execute.
        // Keeping this read-only also makes rejected mouse/controller choices safe.
        private string CombatMovementReadyReason(CombatUnit active)
        {
            if (active == null || state?.Combat == null) return "No active unit.";
            if (IsCombatResolutionPending()) return "A power is still resolving.";
            if (active.Stunned > 0) return "Stunned: cannot move.";
            if (active.Sleeping > 0) return "Sleeping: cannot move.";
            if (active.Webbed > 0) return "Webbed: cannot move.";
            if (state.Combat.MovePoints <= 0) return "No movement remains.";
            return "";
        }

        private string CombatMovementBlockReason(CombatUnit active, int x, int y)
        {
            string unavailable = CombatMovementReadyReason(active);
            if (!string.IsNullOrEmpty(unavailable)) return unavailable;
            if (x < 0 || x >= CombatW || y < 0 || y >= CombatH) return "Choose a tile on the battlefield.";
            if (x == active.X && y == active.Y) return "Current tile: choose a destination.";
            if (!CanStandAt(x, y)) return UnitAt(x, y) != null ? "Tile occupied." : "Path blocked by terrain.";
            int cost = MoveCostTo(active, x, y);
            if (cost >= UnreachableMoveCost || cost <= 0) return "No clear path to that tile.";
            if (cost > state.Combat.MovePoints) return $"Too far: needs {cost} move, {state.Combat.MovePoints} left.";
            return "";
        }

        private string CombatMovementPreview(CombatUnit active, int x, int y)
        {
            string reason = CombatMovementBlockReason(active, x, y);
            if (!string.IsNullOrEmpty(reason)) return reason;
            int cost = MoveCostTo(active, x, y);
            return $"move {cost}, {state.Combat.MovePoints - cost} left / {ProjectedMoveThreatSummary(active, x, y)}{TerrainPreviewLine(ObstacleAt(x, y))}";
        }

        private bool IsAbilityActionable(CombatUnit active, MartialAbility ability, CombatUnit target, int x, int y, out string reason)
        {
            return AbilityUsableNow(active, ability, out reason)
                && CanTargetAbility(active, ability, target, x, y, out reason);
        }

        private string CombatTargetRejectionReason(CombatUnit active, int x, int y)
        {
            if (selectedAction == ActionMode.Move) return CombatMovementBlockReason(active, x, y);
            string unavailable = CombatPowerActionBlockReason(active);
            if (!string.IsNullOrEmpty(unavailable)) return unavailable;
            CombatUnit target = UnitAt(x, y);
            if (selectedAction == ActionMode.Ability)
            {
                MartialAbility ability = AbilityDef(pendingAbilityId);
                if (ability == null) return "Choose a combat skill first.";
                if (!IsAbilityActionable(active, ability, target, x, y, out string reason)) return reason;
            }
            if (selectedAction == ActionMode.Cast)
            {
                FormulaDef formula = GetFormula(pendingFormulaCode);
                if (formula == null) return "Choose a spell card first.";
                if (!IsFormulaActionable(formula, active, target, x, y)) return FormulaPreview(active, formula, target, x, y).Replace('\n', ' ');
            }
            if (selectedAction == ActionMode.Attack)
            {
                if (target != null) return CombatThreatRules.BlockLabel(AttackForecast(active, target).BlockReason);
                Point obstacle = ObstacleAt(x, y);
                if (IsDisruptableRitual(obstacle) || IsBreakableCover(obstacle)) return CombatObstacleAttackBlockReason(active, obstacle);
                return "Choose an enemy, ritual, or breakable cover.";
            }
            return "Choose a highlighted target.";
        }
    }
}
