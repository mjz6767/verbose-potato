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
}
