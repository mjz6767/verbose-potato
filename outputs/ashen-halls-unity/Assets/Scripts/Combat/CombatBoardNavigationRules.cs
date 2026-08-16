using System;
using System.Collections.Generic;
using UnityEngine;

namespace AshenHalls
{
    public static class CombatBoardNavigationRules
    {
        public static Vector2Int Step(Vector2Int current, int deltaX, int deltaY, int width, int height)
        {
            int maximumX = Math.Max(0, width - 1);
            int maximumY = Math.Max(0, height - 1);
            return new Vector2Int(
                Mathf.Clamp(current.x + Math.Sign(deltaX), 0, maximumX),
                Mathf.Clamp(current.y + Math.Sign(deltaY), 0, maximumY));
        }

        public static List<Vector2Int> OrderCandidates(
            Vector2Int origin,
            IEnumerable<Vector2Int> candidates,
            int width,
            int height)
        {
            List<Vector2Int> ordered = new List<Vector2Int>();
            if (candidates == null || width <= 0 || height <= 0) return ordered;

            HashSet<Vector2Int> unique = new HashSet<Vector2Int>();
            foreach (Vector2Int candidate in candidates)
            {
                if (candidate.x < 0 || candidate.x >= width || candidate.y < 0 || candidate.y >= height) continue;
                if (unique.Add(candidate)) ordered.Add(candidate);
            }

            ordered.Sort((left, right) =>
            {
                int leftDistance = Manhattan(origin, left);
                int rightDistance = Manhattan(origin, right);
                int distanceOrder = leftDistance.CompareTo(rightDistance);
                if (distanceOrder != 0) return distanceOrder;
                int rowOrder = left.y.CompareTo(right.y);
                return rowOrder != 0 ? rowOrder : left.x.CompareTo(right.x);
            });
            return ordered;
        }

        public static Vector2Int Cycle(
            Vector2Int origin,
            Vector2Int current,
            IEnumerable<Vector2Int> candidates,
            int direction,
            int width,
            int height)
        {
            List<Vector2Int> ordered = OrderCandidates(origin, candidates, width, height);
            if (ordered.Count == 0) return Step(current, 0, 0, width, height);

            int currentIndex = ordered.IndexOf(current);
            int step = direction < 0 ? -1 : 1;
            if (currentIndex < 0) return step < 0 ? ordered[ordered.Count - 1] : ordered[0];
            int nextIndex = (currentIndex + step + ordered.Count) % ordered.Count;
            return ordered[nextIndex];
        }

        public static bool PointerMovementOwnsInspection(
            Vector2 previousScreenPosition,
            Vector2 currentScreenPosition,
            float minimumDistance = 2f)
        {
            float threshold = Mathf.Max(0f, minimumDistance);
            return (currentScreenPosition - previousScreenPosition).sqrMagnitude >= threshold * threshold;
        }

        public static bool NavigationIsNeutral(
            float horizontal,
            float vertical,
            bool digitalDirectionHeld,
            float axisThreshold = 0.55f)
        {
            float threshold = Mathf.Max(0f, axisThreshold);
            return !digitalDirectionHeld
                && Mathf.Abs(horizontal) < threshold
                && Mathf.Abs(vertical) < threshold;
        }

        private static int Manhattan(Vector2Int origin, Vector2Int point)
        {
            return Math.Abs(point.x - origin.x) + Math.Abs(point.y - origin.y);
        }
    }
}
