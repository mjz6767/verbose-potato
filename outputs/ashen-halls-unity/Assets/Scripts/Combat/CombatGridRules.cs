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
