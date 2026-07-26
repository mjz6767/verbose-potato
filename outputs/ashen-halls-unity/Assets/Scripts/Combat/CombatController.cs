using System;
using UnityEngine;

namespace AshenHalls
{
    public enum CombatOutcome
    {
        Ongoing,
        Defeat,
        Victory
    }

    public enum CombatCommandFailure
    {
        None,
        MissingCombat,
        MissingUnit,
        NotPartyUnit,
        NotActiveUnit,
        NoMovement,
        NoMoveToUndo,
        Webbed,
        SameTile,
        Blocked,
        PathBlocked,
        TooFar,
        ActionUnavailable,
        NoElixir,
        ResolverRejected
    }

    public readonly struct CombatCommandResult
    {
        public readonly bool Success;
        public readonly CombatCommandFailure Failure;
        public readonly int OldX;
        public readonly int OldY;
        public readonly int NewX;
        public readonly int NewY;
        public readonly int MoveCost;

        public CombatCommandResult(bool success, CombatCommandFailure failure, int oldX, int oldY, int newX, int newY, int moveCost)
        {
            Success = success;
            Failure = failure;
            OldX = oldX;
            OldY = oldY;
            NewX = newX;
            NewY = newY;
            MoveCost = moveCost;
        }

        public Vector2 OldPosition => new Vector2(OldX, OldY);
        public Vector2 NewPosition => new Vector2(NewX, NewY);

        public static CombatCommandResult Failed(CombatCommandFailure failure, CombatUnit unit = null, int x = 0, int y = 0)
        {
            int oldX = unit?.X ?? 0;
            int oldY = unit?.Y ?? 0;
            return new CombatCommandResult(false, failure, oldX, oldY, x, y, 0);
        }

        public static CombatCommandResult Completed(CombatUnit unit)
        {
            return new CombatCommandResult(true, CombatCommandFailure.None, unit?.X ?? 0, unit?.Y ?? 0, unit?.X ?? 0, unit?.Y ?? 0, 0);
        }

        public static CombatCommandResult Moved(CombatUnit unit, int oldX, int oldY, int moveCost)
        {
            return new CombatCommandResult(true, CombatCommandFailure.None, oldX, oldY, unit?.X ?? oldX, unit?.Y ?? oldY, moveCost);
        }
    }

    public readonly struct SummonBindingResult
    {
        public readonly bool Ticked;
        public readonly bool Expired;
        public readonly int RemainingTurns;

        public SummonBindingResult(bool ticked, bool expired, int remainingTurns)
        {
            Ticked = ticked;
            Expired = expired;
            RemainingTurns = remainingTurns;
        }
    }

    public sealed class CombatController
    {
        private readonly GameState state;
        private readonly int unreachableMoveCost;
        private readonly Func<CombatUnit, int> moveAllowance;
        private readonly Func<CombatUnit, int, int, bool> canStandAt;
        private readonly Func<CombatUnit, int, int, int> moveCostTo;
        private readonly Func<CombatUnit, bool> isHeroUnit;
        private string moveOriginUnitId = "";
        private int moveOriginX;
        private int moveOriginY;
        private int moveOriginPoints;
        private bool hasMoveOrigin;

        public CombatController(
            GameState state,
            int unreachableMoveCost,
            Func<CombatUnit, int> moveAllowance,
            Func<CombatUnit, int, int, bool> canStandAt,
            Func<CombatUnit, int, int, int> moveCostTo,
            Func<CombatUnit, bool> isHeroUnit = null)
        {
            this.state = state;
            this.unreachableMoveCost = unreachableMoveCost;
            this.moveAllowance = moveAllowance;
            this.canStandAt = canStandAt;
            this.moveCostTo = moveCostTo;
            this.isHeroUnit = isHeroUnit ?? (unit => unit != null && unit.Side == UnitSide.Party);
        }

        public CombatOutcome CurrentOutcome()
        {
            CombatState combat = state?.Combat;
            if (combat?.Units == null) return CombatOutcome.Ongoing;
            bool partyAlive = false;
            bool enemiesAlive = false;
            foreach (CombatUnit unit in combat.Units)
            {
                if (unit == null || unit.Hp <= 0) continue;
                if (isHeroUnit(unit)) partyAlive = true;
                else if (unit.Side == UnitSide.Enemy) enemiesAlive = true;
            }
            if (!partyAlive) return CombatOutcome.Defeat;
            return enemiesAlive || CombatRitualRules.KeepsCombatAlive(combat) ? CombatOutcome.Ongoing : CombatOutcome.Victory;
        }

        public void BeginTurn(CombatUnit active, bool enemyThinking)
        {
            CombatState combat = state?.Combat;
            if (combat == null || active == null) return;
            active.Guarding = false;
            active.GuardBonus = 0;
            combat.ActiveId = active.Id;
            combat.Moved = false;
            combat.Acted = false;
            combat.MovePoints = active.Webbed > 0 ? 0 : MoveAllowance(active);
            combat.ActionAvailable = active.Stunned <= 0 && active.Sleeping <= 0;
            combat.Phase = enemyThinking ? CombatPhase.EnemyThinking : CombatPhase.ChooseAction;
            CaptureMoveOrigin(active);
        }

        public void RepairActiveTurnState(CombatUnit active, bool enemyThinking)
        {
            CombatState combat = state?.Combat;
            if (combat == null || active == null) return;

            if (!IsActiveUnit(active)) combat.ActiveId = active.Id;

            combat.ActionAvailable = !combat.Acted && active.Stunned <= 0 && active.Sleeping <= 0;

            int allowance = MoveAllowance(active);
            if (active.Webbed > 0)
            {
                combat.MovePoints = 0;
            }
            else if (!combat.Moved && combat.MovePoints <= 0)
            {
                combat.MovePoints = allowance;
            }
            combat.MovePoints = Mathf.Clamp(combat.MovePoints, 0, allowance);
            combat.Phase = enemyThinking ? CombatPhase.EnemyThinking : CombatPhase.ChooseAction;
            if (!hasMoveOrigin || !string.Equals(moveOriginUnitId, active.Id, StringComparison.Ordinal))
            {
                CaptureMoveOrigin(active);
            }
        }

        public bool ActionEnabled(ActionMode mode, CombatUnit active, bool hasSpell, bool hasMartialAbility, int elixirs)
        {
            CombatState combat = state?.Combat;
            if (combat == null || active == null) return false;
            if (!IsActiveUnit(active)) return false;
            if (combat.Phase == CombatPhase.Resolving) return false;
            if (active.Stunned > 0 || active.Sleeping > 0) return mode == ActionMode.Wait;
            if (mode == ActionMode.Move) return combat.MovePoints > 0 && active.Webbed <= 0;
            if (mode == ActionMode.Attack) return combat.ActionAvailable;
            if (mode == ActionMode.Cast) return combat.ActionAvailable && hasSpell;
            if (mode == ActionMode.Ability) return combat.ActionAvailable && hasMartialAbility;
            if (mode == ActionMode.Elixir) return !active.Summoned && combat.ActionAvailable && elixirs > 0;
            if (mode == ActionMode.Guard) return combat.ActionAvailable;
            return true;
        }

        public CombatCommandResult TryMove(CombatUnit active, int x, int y)
        {
            CombatState combat = state?.Combat;
            if (combat == null) return CombatCommandResult.Failed(CombatCommandFailure.MissingCombat, active, x, y);
            if (active == null) return CombatCommandResult.Failed(CombatCommandFailure.MissingUnit, active, x, y);
            if (!IsActiveUnit(active)) return CombatCommandResult.Failed(CombatCommandFailure.NotActiveUnit, active, x, y);
            if (combat.Phase == CombatPhase.Resolving) return CombatCommandResult.Failed(CombatCommandFailure.ActionUnavailable, active, x, y);
            if (combat.MovePoints <= 0) return CombatCommandResult.Failed(CombatCommandFailure.NoMovement, active, x, y);
            if (active.Webbed > 0) return CombatCommandResult.Failed(CombatCommandFailure.Webbed, active, x, y);

            int distance = Mathf.Abs(x - active.X) + Mathf.Abs(y - active.Y);
            if (distance <= 0) return CombatCommandResult.Failed(CombatCommandFailure.SameTile, active, x, y);
            if (canStandAt == null || !canStandAt(active, x, y)) return CombatCommandResult.Failed(CombatCommandFailure.Blocked, active, x, y);

            int moveCost = moveCostTo == null ? distance : moveCostTo(active, x, y);
            if (moveCost >= unreachableMoveCost) return CombatCommandResult.Failed(CombatCommandFailure.PathBlocked, active, x, y);
            if (moveCost > combat.MovePoints) return CombatCommandResult.Failed(CombatCommandFailure.TooFar, active, x, y);

            int oldX = active.X;
            int oldY = active.Y;
            active.X = x;
            active.Y = y;
            combat.MovePoints = Mathf.Max(0, combat.MovePoints - moveCost);
            combat.Moved = combat.MovePoints <= 0;
            combat.Phase = CombatPhase.ChooseAction;
            return CombatCommandResult.Moved(active, oldX, oldY, moveCost);
        }

        public bool CanUndoMove(CombatUnit active)
        {
            CombatState combat = state?.Combat;
            if (combat == null || active == null || !hasMoveOrigin) return false;
            if (!IsActiveUnit(active) || active.Side != UnitSide.Party) return false;
            if (!string.Equals(moveOriginUnitId, active.Id, StringComparison.Ordinal)) return false;
            if (!combat.ActionAvailable || combat.Acted) return false;
            if (combat.Phase == CombatPhase.Resolving || combat.Phase == CombatPhase.EnemyThinking) return false;
            if (combat.MovePoints >= moveOriginPoints
                && active.X == moveOriginX
                && active.Y == moveOriginY)
            {
                return false;
            }

            bool alreadyAtOrigin = active.X == moveOriginX && active.Y == moveOriginY;
            return alreadyAtOrigin || canStandAt != null && canStandAt(active, moveOriginX, moveOriginY);
        }

        public CombatCommandResult TryUndoMove(CombatUnit active)
        {
            if (!CanUndoMove(active))
            {
                return CombatCommandResult.Failed(CombatCommandFailure.NoMoveToUndo, active);
            }

            CombatState combat = state.Combat;
            int oldX = active.X;
            int oldY = active.Y;
            active.X = moveOriginX;
            active.Y = moveOriginY;
            combat.MovePoints = moveOriginPoints;
            combat.Moved = false;
            combat.Phase = CombatPhase.ChooseAction;
            return CombatCommandResult.Moved(active, oldX, oldY, 0);
        }

        public CombatCommandResult Guard(CombatUnit active, int guardBonus)
        {
            if (!CanSpendAction(active)) return CombatCommandResult.Failed(ActionFailureFor(active), active);
            active.Guarding = true;
            active.GuardBonus = guardBonus;
            CompleteAction(active, true);
            return CombatCommandResult.Completed(active);
        }

        public CombatCommandResult EndTurn(CombatUnit active)
        {
            if (state?.Combat == null) return CombatCommandResult.Failed(CombatCommandFailure.MissingCombat, active);
            if (active == null) return CombatCommandResult.Failed(CombatCommandFailure.MissingUnit, active);
            if (!IsActiveUnit(active)) return CombatCommandResult.Failed(CombatCommandFailure.NotActiveUnit, active);
            CompleteAction(active, true);
            return CombatCommandResult.Completed(active);
        }

        public CombatCommandResult TryUseItem(CombatUnit active, int healAmount, int manaAmount)
        {
            if (!CanSpendAction(active)) return CombatCommandResult.Failed(ActionFailureFor(active), active);
            if (active.Side != UnitSide.Party || active.Summoned) return CombatCommandResult.Failed(CombatCommandFailure.NotPartyUnit, active);
            if (state.Elixirs <= 0) return CombatCommandResult.Failed(CombatCommandFailure.NoElixir, active);

            state.Elixirs--;
            active.Hp = Mathf.Min(active.MaxHp, active.Hp + Mathf.Max(0, healAmount));
            active.Mana = Mathf.Min(active.MaxMana, active.Mana + Mathf.Max(0, manaAmount));
            CompleteAction(active, false);
            return CombatCommandResult.Completed(active);
        }

        public CombatCommandResult TryAttack(CombatUnit active, CombatUnit target, Func<CombatUnit, CombatUnit, bool> resolveAttack)
        {
            if (target == null || resolveAttack == null) return CombatCommandResult.Failed(CombatCommandFailure.ResolverRejected, active);
            return TryResolveAction(active, () => resolveAttack(active, target));
        }

        public CombatCommandResult TryUseAbility(CombatUnit active, Func<bool> resolveAbility)
        {
            return TryResolveAction(active, resolveAbility);
        }

        public CombatCommandResult TryResolveAction(CombatUnit active, Func<bool> resolveAction, bool endMovement = false)
        {
            if (!CanSpendAction(active)) return CombatCommandResult.Failed(ActionFailureFor(active), active);
            if (resolveAction == null || !resolveAction()) return CombatCommandResult.Failed(CombatCommandFailure.ResolverRejected, active);
            CompleteAction(active, endMovement);
            return CombatCommandResult.Completed(active);
        }

        public void ApplyEnemyMovementResult(bool moved, int moveBudget, int spentMove)
        {
            ApplyMovementBudgetResult(moved, moveBudget, spentMove);
        }

        public void ApplyMovementBudgetResult(bool moved, int moveBudget, int spentMove)
        {
            CombatState combat = state?.Combat;
            if (combat == null) return;
            combat.Moved = moved;
            combat.MovePoints = Mathf.Max(0, moveBudget - spentMove);
        }

        public void CompleteAction(CombatUnit active, bool endMovement)
        {
            CombatState combat = state?.Combat;
            if (combat == null) return;
            if (endMovement)
            {
                combat.MovePoints = 0;
                combat.Moved = true;
            }
            combat.ActionAvailable = false;
            combat.Acted = true;
            combat.Phase = CombatPhase.Resolving;
        }

        public SummonBindingResult TickSummonBindingEndOfTurn(CombatUnit active)
        {
            if (active == null || !active.Summoned || active.Hp <= 0 || active.SummonTurns <= 0)
            {
                return new SummonBindingResult(false, false, active?.SummonTurns ?? 0);
            }

            active.SummonTurns--;
            if (active.SummonTurns > 0)
            {
                return new SummonBindingResult(true, false, active.SummonTurns);
            }

            active.Hp = 0;
            return new SummonBindingResult(true, true, 0);
        }

        private bool CanSpendAction(CombatUnit active)
        {
            CombatState combat = state?.Combat;
            return combat != null && active != null && IsActiveUnit(active) && combat.ActionAvailable && active.Stunned <= 0 && active.Sleeping <= 0;
        }

        private CombatCommandFailure ActionFailureFor(CombatUnit active)
        {
            CombatState combat = state?.Combat;
            if (combat == null) return CombatCommandFailure.MissingCombat;
            if (active == null) return CombatCommandFailure.MissingUnit;
            if (!IsActiveUnit(active)) return CombatCommandFailure.NotActiveUnit;
            return CombatCommandFailure.ActionUnavailable;
        }

        private int MoveAllowance(CombatUnit active)
        {
            return moveAllowance == null ? 0 : Mathf.Max(0, moveAllowance(active));
        }

        private bool IsActiveUnit(CombatUnit active)
        {
            CombatState combat = state?.Combat;
            return combat != null && active != null && string.Equals(combat.ActiveId, active.Id, StringComparison.Ordinal);
        }

        private void CaptureMoveOrigin(CombatUnit active)
        {
            if (active == null)
            {
                hasMoveOrigin = false;
                moveOriginUnitId = "";
                return;
            }

            hasMoveOrigin = true;
            moveOriginUnitId = active.Id ?? "";
            moveOriginX = active.X;
            moveOriginY = active.Y;
            moveOriginPoints = Mathf.Max(0, state?.Combat?.MovePoints ?? 0);
        }
    }
}
