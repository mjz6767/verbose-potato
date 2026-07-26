using System;
using System.Collections.Generic;

namespace AshenHalls
{
    public static class RoamingThreatRules
    {
        public const int AlertRadius = 5;
        public const int MovementCadence = 2;
        public const int ReturnCadence = 3;
        public const int DefeatRespawnSteps = 36;
        public const int RetreatGraceSteps = 6;

        private static readonly int[] StepX = { 0, -1, 1, 0 };
        private static readonly int[] StepY = { -1, 0, 0, 1 };

        public static bool IsAdjacent(int ax, int ay, int bx, int by)
        {
            return Math.Abs(ax - bx) + Math.Abs(ay - by) == 1;
        }

        public static bool ShouldAlert(int distance, bool playerIsSafe, int graceSteps)
        {
            return !playerIsSafe && graceSteps <= 0 && distance > 1 && distance <= AlertRadius;
        }

        public static bool ShouldPursue(int explorationSteps)
        {
            return explorationSteps > 0 && explorationSteps % MovementCadence == 0;
        }

        public static bool ShouldReturnHome(int explorationSteps)
        {
            return explorationSteps > 0 && explorationSteps % ReturnCadence == 0;
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
