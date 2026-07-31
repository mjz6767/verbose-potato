using System;
using System.Collections.Generic;
using UnityEngine;

namespace AshenHalls
{
    public static class CombatGridRules
    {
        public static int[,] ReachableMoveCosts(
            CombatUnit active,
            int width,
            int height,
            int maxCost,
            int unreachableCost,
            Func<CombatUnit, int, int, bool> canEnterMoveTile,
            Func<CombatUnit, int, int, int> stepCost)
        {
            int[,] costs = new int[width, height];
            for (int yy = 0; yy < height; yy++)
            for (int xx = 0; xx < width; xx++)
            {
                costs[xx, yy] = unreachableCost;
            }

            if (active == null || active.X < 0 || active.X >= width || active.Y < 0 || active.Y >= height) return costs;

            maxCost = Mathf.Max(0, maxCost);
            Queue<Vector2Int> open = new Queue<Vector2Int>();
            costs[active.X, active.Y] = 0;
            open.Enqueue(new Vector2Int(active.X, active.Y));

            int[] dx = { 1, -1, 0, 0 };
            int[] dy = { 0, 0, 1, -1 };
            while (open.Count > 0)
            {
                Vector2Int current = open.Dequeue();
                int currentCost = costs[current.x, current.y];
                for (int i = 0; i < 4; i++)
                {
                    int nx = current.x + dx[i];
                    int ny = current.y + dy[i];
                    if (!canEnterMoveTile(active, nx, ny)) continue;
                    int nextCost = currentCost + Mathf.Max(1, stepCost(active, nx, ny));
                    if (nextCost > maxCost || nextCost >= costs[nx, ny]) continue;
                    costs[nx, ny] = nextCost;
                    open.Enqueue(new Vector2Int(nx, ny));
                }
            }

            return costs;
        }

        public static IReadOnlyList<Vector2Int> ShortestReachablePath(
            CombatUnit active,
            int[,] reachableCosts,
            int destinationX,
            int destinationY,
            int unreachableCost,
            Func<CombatUnit, int, int, int> stepCost)
        {
            List<Vector2Int> path = new List<Vector2Int>();
            if (active == null || reachableCosts == null) return path;

            int width = reachableCosts.GetLength(0);
            int height = reachableCosts.GetLength(1);
            if (active.X < 0 || active.X >= width || active.Y < 0 || active.Y >= height) return path;
            if (destinationX < 0 || destinationX >= width || destinationY < 0 || destinationY >= height) return path;

            int destinationCost = reachableCosts[destinationX, destinationY];
            if (destinationCost < 0 || destinationCost >= unreachableCost) return path;

            int x = destinationX;
            int y = destinationY;
            path.Add(new Vector2Int(x, y));
            int remainingSteps = width * height;
            int[] dx = { -1, 0, 1, 0 };
            int[] dy = { 0, -1, 0, 1 };
            while ((x != active.X || y != active.Y) && remainingSteps-- > 0)
            {
                int currentCost = reachableCosts[x, y];
                int enterCost = stepCost == null ? 1 : Mathf.Max(1, stepCost(active, x, y));
                bool foundPredecessor = false;
                for (int i = 0; i < dx.Length; i++)
                {
                    int nx = x + dx[i];
                    int ny = y + dy[i];
                    if (nx < 0 || nx >= width || ny < 0 || ny >= height) continue;
                    int predecessorCost = reachableCosts[nx, ny];
                    if (predecessorCost < 0 || predecessorCost >= unreachableCost) continue;
                    if (predecessorCost + enterCost != currentCost) continue;

                    x = nx;
                    y = ny;
                    path.Add(new Vector2Int(x, y));
                    foundPredecessor = true;
                    break;
                }

                if (!foundPredecessor)
                {
                    path.Clear();
                    return path;
                }
            }

            if (x != active.X || y != active.Y)
            {
                path.Clear();
                return path;
            }

            path.Reverse();
            return path;
        }

        public static bool HasLineOfSight(int ax, int ay, int bx, int by, int width, int height, bool missiles, Func<int, int, bool> blocksSight)
        {
            if (!missiles) return true;
            if (ax == bx && ay == by) return true;
            foreach (Vector2Int cell in SupercoverLine(ax, ay, bx, by, width, height))
            {
                if (cell.x == ax && cell.y == ay) continue;
                if (cell.x == bx && cell.y == by) continue;
                if (blocksSight(cell.x, cell.y)) return false;
            }
            return true;
        }

        public static IEnumerable<Vector2Int> SupercoverLine(int ax, int ay, int bx, int by, int width, int height)
        {
            int dx = bx - ax;
            int dy = by - ay;
            int nx = Mathf.Abs(dx);
            int ny = Mathf.Abs(dy);
            int signX = Math.Sign(dx);
            int signY = Math.Sign(dy);
            int x = ax;
            int y = ay;
            HashSet<string> yielded = new HashSet<string>();
            List<Vector2Int> result = new List<Vector2Int>();

            AddCell(result, yielded, x, y, width, height);
            int ix = 0;
            int iy = 0;
            while (ix < nx || iy < ny)
            {
                int decision = (1 + 2 * ix) * ny - (1 + 2 * iy) * nx;
                if (decision == 0)
                {
                    AddCell(result, yielded, x + signX, y, width, height);
                    AddCell(result, yielded, x, y + signY, width, height);
                    x += signX;
                    y += signY;
                    ix++;
                    iy++;
                }
                else if (decision < 0)
                {
                    x += signX;
                    ix++;
                }
                else
                {
                    y += signY;
                    iy++;
                }
                AddCell(result, yielded, x, y, width, height);
            }

            return result;
        }

        public static int ManhattanDistance(int ax, int ay, int bx, int by)
        {
            return Mathf.Abs(ax - bx) + Mathf.Abs(ay - by);
        }

        private static void AddCell(List<Vector2Int> cells, HashSet<string> yielded, int x, int y, int width, int height)
        {
            if (x < 0 || y < 0 || x >= width || y >= height) return;
            string key = x + "," + y;
            if (yielded.Add(key)) cells.Add(new Vector2Int(x, y));
        }
    }
}
