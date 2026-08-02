using System;
using System.Collections.Generic;

namespace AshenHalls
{
    public readonly struct RoamingThreatBehaviorProfile
    {
        public readonly string Id;
        public readonly int AlertRadius;
        public readonly int PursuitCadence;
        public readonly int ReturnCadence;
        public readonly int LeashRadius;

        public RoamingThreatBehaviorProfile(
            string id,
            int alertRadius,
            int pursuitCadence,
            int returnCadence,
            int leashRadius)
        {
            Id = id ?? "";
            AlertRadius = Math.Max(2, alertRadius);
            PursuitCadence = Math.Max(1, pursuitCadence);
            ReturnCadence = Math.Max(1, returnCadence);
            LeashRadius = Math.Max(AlertRadius + 1, leashRadius);
        }
    }

    public static class RoamingThreatRules
    {
        // Broad presentation envelope retained for systems that do not own a
        // concrete threat. Live movement uses the faction profile below.
        public const int AlertRadius = 6;
        public const int MovementCadence = 2;
        public const int ReturnCadence = 3;
        public const int DefeatRespawnSteps = 36;
        public const int RetreatGraceSteps = 6;

        public static readonly RoamingThreatBehaviorProfile DefaultProfile =
            new RoamingThreatBehaviorProfile("road-patrol", 5, MovementCadence, ReturnCadence, 8);
        private static readonly RoamingThreatBehaviorProfile RatProfile =
            new RoamingThreatBehaviorProfile("skittish-scavengers", 4, 2, 2, 6);
        private static readonly RoamingThreatBehaviorProfile KoboldProfile =
            new RoamingThreatBehaviorProfile("coordinated-raiders", 5, 2, 3, 8);
        private static readonly RoamingThreatBehaviorProfile DrowProfile =
            new RoamingThreatBehaviorProfile("watchful-hunters", 6, 2, 4, 10);
        private static readonly RoamingThreatBehaviorProfile UndeadProfile =
            new RoamingThreatBehaviorProfile("relentless-dead", 4, 3, 5, 11);
        private static readonly RoamingThreatBehaviorProfile DemonProfile =
            new RoamingThreatBehaviorProfile("rampaging-demons", 6, 1, 2, 12);

        private static readonly int[] StepX = { 0, -1, 1, 0 };
        private static readonly int[] StepY = { -1, 0, 0, 1 };

        public static bool IsAdjacent(int ax, int ay, int bx, int by)
        {
            return Math.Abs(ax - bx) + Math.Abs(ay - by) == 1;
        }

        public static bool ShouldAlert(int distance, bool playerIsSafe, int graceSteps)
        {
            return ShouldAlert(distance, playerIsSafe, graceSteps, DefaultProfile);
        }

        public static bool ShouldAlert(
            int distance,
            bool playerIsSafe,
            int graceSteps,
            RoamingThreatBehaviorProfile profile)
        {
            return !playerIsSafe && graceSteps <= 0 && distance > 1 && distance <= profile.AlertRadius;
        }

        public static bool ShouldPursue(int explorationSteps)
        {
            return ShouldPursue(explorationSteps, DefaultProfile);
        }

        public static bool ShouldPursue(int explorationSteps, RoamingThreatBehaviorProfile profile)
        {
            return explorationSteps > 0 && explorationSteps % profile.PursuitCadence == 0;
        }

        public static bool ShouldReturnHome(int explorationSteps)
        {
            return ShouldReturnHome(explorationSteps, DefaultProfile);
        }

        public static bool ShouldReturnHome(int explorationSteps, RoamingThreatBehaviorProfile profile)
        {
            return explorationSteps > 0 && explorationSteps % profile.ReturnCadence == 0;
        }

        public static bool ShouldLeash(int distanceFromHome, RoamingThreatBehaviorProfile profile)
        {
            return distanceFromHome >= profile.LeashRadius;
        }

        public static int DisengageRadius(RoamingThreatBehaviorProfile profile)
        {
            return profile.AlertRadius + 2;
        }

        public static RoamingThreatBehaviorProfile ProfileFor(RoamingThreatFaction faction)
        {
            switch (faction)
            {
                case RoamingThreatFaction.Rats: return RatProfile;
                case RoamingThreatFaction.Kobolds: return KoboldProfile;
                case RoamingThreatFaction.Drow: return DrowProfile;
                case RoamingThreatFaction.Undead: return UndeadProfile;
                case RoamingThreatFaction.Demons: return DemonProfile;
                default: return DefaultProfile;
            }
        }

        public static int SpawnScore(int seed, int slot, int x, int y)
        {
            unchecked
            {
                int hash = seed;
                hash = hash * 397 ^ slot * 7919;
                hash = hash * 397 ^ x * 193;
                hash = hash * 397 ^ y * 389;
                hash ^= hash >> 16;
                return hash & int.MaxValue;
            }
        }

        public static bool TryNextStep(
            int width,
            int height,
            int startX,
            int startY,
            int targetX,
            int targetY,
            Func<int, int, bool> canStand,
            Func<int, int, bool> isOccupied,
            bool stopAdjacent,
            out Point next)
        {
            next = null;
            if (width <= 0 || height <= 0 || canStand == null) return false;
            if (!Inside(startX, startY, width, height) || !Inside(targetX, targetY, width, height)) return false;
            if (startX == targetX && startY == targetY) return false;

            bool[,] seen = new bool[width, height];
            int[,] parentX = new int[width, height];
            int[,] parentY = new int[width, height];
            Queue<Point> queue = new Queue<Point>();
            seen[startX, startY] = true;
            parentX[startX, startY] = startX;
            parentY[startX, startY] = startY;
            queue.Enqueue(new Point(startX, startY));

            Point destination = null;
            while (queue.Count > 0)
            {
                Point current = queue.Dequeue();
                bool arrived = stopAdjacent
                    ? IsAdjacent(current.X, current.Y, targetX, targetY)
                    : current.X == targetX && current.Y == targetY;
                if (arrived)
                {
                    destination = current;
                    break;
                }

                for (int i = 0; i < StepX.Length; i++)
                {
                    int nx = current.X + StepX[i];
                    int ny = current.Y + StepY[i];
                    if (!Inside(nx, ny, width, height) || seen[nx, ny]) continue;
                    if (stopAdjacent && nx == targetX && ny == targetY) continue;
                    if (!canStand(nx, ny) || isOccupied != null && isOccupied(nx, ny)) continue;
                    seen[nx, ny] = true;
                    parentX[nx, ny] = current.X;
                    parentY[nx, ny] = current.Y;
                    queue.Enqueue(new Point(nx, ny));
                }
            }

            if (destination == null || destination.X == startX && destination.Y == startY) return false;
            int stepX = destination.X;
            int stepY = destination.Y;
            while (parentX[stepX, stepY] != startX || parentY[stepX, stepY] != startY)
            {
                int previousX = parentX[stepX, stepY];
                int previousY = parentY[stepX, stepY];
                if (previousX == stepX && previousY == stepY) return false;
                stepX = previousX;
                stepY = previousY;
            }

            next = new Point(stepX, stepY);
            return true;
        }

        private static bool Inside(int x, int y, int width, int height)
        {
            return x >= 0 && y >= 0 && x < width && y < height;
        }
    }
}
