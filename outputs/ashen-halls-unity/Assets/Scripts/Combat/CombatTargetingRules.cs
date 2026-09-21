using System;
using System.Collections.Generic;
using UnityEngine;

namespace AshenHalls
{
    public enum CombatTargetHighlightState
    {
        None,
        Blocked,
        Legal
    }

    public static class CombatTargetingRules
    {
        public static bool ShouldDrawPassiveTargetState(CombatTargetHighlightState state)
        {
            return state == CombatTargetHighlightState.Legal;
        }

        public static string BlockedBadge(AttackForecastBlockReason reason)
        {
            switch (reason)
            {
                case AttackForecastBlockReason.OutOfRange: return "RANGE";
                case AttackForecastBlockReason.LineOfSight: return "LOS";
                case AttackForecastBlockReason.FriendlyTarget: return "ALLY";
                case AttackForecastBlockReason.DefeatedTarget: return "DOWN";
                default: return "REQ";
            }
        }

        public static string BlockedBadge(string reason)
        {
            string value = (reason ?? "").Trim().ToLowerInvariant();
            if (value.Contains("web")) return "WEB";
            if (value.Contains("stun") || value.Contains("sleep")) return "HELD";
            if (value.Contains("action already") || value.Contains("resolving")) return "WAIT";
            if (value.Contains("no movement")) return "MOVE";
            if (value.Contains("current tile")) return "HERE";
            if (value.Contains("path") || value.Contains("lane") || value.Contains("landing")) return "PATH";
            if (value.Contains("occupied")) return "FULL";
            if (value.Contains("35%")) return "HP";
            if (value.Contains("line of sight") || value.Contains("covered") || value.Contains("blocked")) return "LOS";
            if (value.Contains("range") || value.Contains("reach") || value.Contains("far") || value.Contains("adjacent")) return "RANGE";
            if (value.Contains("mana") || HasWord(value, "mp")) return "MANA";
            if (value.Contains("target") || value.Contains("enemy") || value.Contains("empty tile")) return "TARGET";
            return "REQ";
        }

        private static bool HasWord(string value, string word)
        {
            for (int index = value.IndexOf(word, StringComparison.Ordinal); index >= 0;
                index = value.IndexOf(word, index + word.Length, StringComparison.Ordinal))
            {
                bool startsWord = index == 0 || !char.IsLetterOrDigit(value[index - 1]);
                int end = index + word.Length;
                bool endsWord = end == value.Length || !char.IsLetterOrDigit(value[end]);
                if (startsWord && endsWord) return true;
            }
            return false;
        }

        public static bool ShouldDrawTargetHighlights(
            ActionMode selectedAction,
            bool hasResolvedFormula,
            bool hasResolvedAbility)
        {
            if (selectedAction == ActionMode.Attack) return true;
            if (selectedAction == ActionMode.Cast) return hasResolvedFormula;
            if (selectedAction == ActionMode.Ability) return hasResolvedAbility;
            return false;
        }

        public static bool RoutesUnitPreviewToSideRail(
            ActionMode selectedAction,
            bool hasUnitTarget)
        {
            if (!hasUnitTarget) return false;
            return selectedAction == ActionMode.Attack
                || selectedAction == ActionMode.Cast
                || selectedAction == ActionMode.Ability;
        }

        public static Rect PlaceBoardTooltip(
            Rect board,
            Rect decision,
            Vector2 pointer,
            float requestedWidth,
            float requestedHeight,
            IReadOnlyList<Rect> blockers)
        {
            const float gap = 8f;
            float width = Mathf.Clamp(requestedWidth, 80f, Mathf.Max(80f, board.width - gap * 2f));
            float height = Mathf.Clamp(requestedHeight, 60f, Mathf.Max(60f, board.height - gap * 2f));
            Rect[] candidates =
            {
                new Rect(board.xMin + gap, board.yMin + gap, width, height),
                new Rect(board.xMax - width - gap, board.yMin + gap, width, height),
                new Rect(board.xMin + gap, board.yMax - height - gap, width, height),
                new Rect(board.xMax - width - gap, board.yMax - height - gap, width, height)
            };

            Rect best = candidates[0];
            float bestScore = float.MaxValue;
            for (int i = 0; i < candidates.Length; i++)
            {
                Rect candidate = candidates[i];
                float score = OverlapArea(candidate, decision) * 20f;
                if (blockers != null)
                {
                    for (int j = 0; j < blockers.Count; j++)
                    {
                        score += OverlapArea(candidate, blockers[j]) * 8f;
                    }
                }
                score -= (candidate.center - pointer).sqrMagnitude * 0.0001f;
                if (score >= bestScore) continue;
                best = candidate;
                bestScore = score;
            }
            return best;
        }

        private static float OverlapArea(Rect left, Rect right)
        {
            float width = Mathf.Max(0f, Mathf.Min(left.xMax, right.xMax) - Mathf.Max(left.xMin, right.xMin));
            float height = Mathf.Max(0f, Mathf.Min(left.yMax, right.yMax) - Mathf.Max(left.yMin, right.yMin));
            return width * height;
        }

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

    public static class CombatTurnFlowRules
    {
        public static ActionMode DefaultAction(
            bool incapacitated,
            bool actionAvailable,
            bool hasLegalAttack,
            bool hasReachableMove,
            bool hasActionablePower,
            ActionMode powerMode,
            bool canGuard)
        {
            if (incapacitated) return ActionMode.Wait;
            if (actionAvailable && hasLegalAttack) return ActionMode.Attack;
            if (hasReachableMove) return ActionMode.Move;
            if (actionAvailable
                && hasActionablePower
                && (powerMode == ActionMode.Cast || powerMode == ActionMode.Ability))
            {
                return powerMode;
            }
            if (actionAvailable && canGuard) return ActionMode.Guard;
            return ActionMode.Wait;
        }

        public static bool ShouldResumePostActionMovement(
            bool partyControlled,
            bool alive,
            bool actionAvailable,
            int movePoints,
            bool combatOngoing,
            bool hasReachableMove = true)
        {
            return partyControlled
                && alive
                && !actionAvailable
                && movePoints > 0
                && combatOngoing
                && hasReachableMove;
        }
    }
}
