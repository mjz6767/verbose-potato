using System;
using System.Collections.Generic;

namespace AshenHalls
{
    public static class ExplorationTraversalRules
    {
        public static bool CanStandOnObject(MapObject obj)
        {
            return obj == null || CanStandOnObject(obj.Type);
        }

        public static bool CanStandOnObject(ObjectType type)
        {
            switch (type)
            {
                case ObjectType.Stairs:
                case ObjectType.Town:
                case ObjectType.RecallCircle:
                case ObjectType.EastGate:
                case ObjectType.WestGate:
                case ObjectType.Camp:
                case ObjectType.Bridge:
                    return true;
                default:
                    return false;
            }
        }

        public static bool CanUseFromAdjacent(MapObject obj)
        {
            return obj != null && CanUseFromAdjacent(obj.Type);
        }

        public static bool CanUseFromAdjacent(ObjectType type)
        {
            if (type == ObjectType.Stairs) return false;
            return ExplorationInteractionRules.IsUseObject(type);
        }

        public static bool BlocksMovement(MapObject obj)
        {
            return obj != null && !CanStandOnObject(obj.Type);
        }

        public static bool IsStandable(MapData map, int x, int y)
        {
            if (map?.Tiles == null || x < 0 || y < 0 || x >= map.Width || y >= map.Height) return false;
            int index = y * map.Width + x;
            if (index < 0 || index >= map.Tiles.Count || map.Tiles[index] != 1) return false;
            return CanStandOnObject(ObjectAt(map, x, y));
        }

        public static bool[,] ReachableMask(MapData map, int startX, int startY)
        {
            int width = map?.Width ?? 0;
            int height = map?.Height ?? 0;
            bool[,] reachable = new bool[width, height];
            if (!IsStandable(map, startX, startY)) return reachable;

            Queue<Point> queue = new Queue<Point>();
            queue.Enqueue(new Point(startX, startY));
            reachable[startX, startY] = true;
            int[] dx = { 0, 0, -1, 1 };
            int[] dy = { -1, 1, 0, 0 };
            while (queue.Count > 0)
            {
                Point current = queue.Dequeue();
                for (int i = 0; i < dx.Length; i++)
                {
                    int nx = current.X + dx[i];
                    int ny = current.Y + dy[i];
                    if (nx < 0 || ny < 0 || nx >= width || ny >= height || reachable[nx, ny]) continue;
                    if (!IsStandable(map, nx, ny)) continue;
                    reachable[nx, ny] = true;
                    queue.Enqueue(new Point(nx, ny));
                }
            }
            return reachable;
        }

        public static bool CanReachObject(bool[,] reachable, MapData map, MapObject obj)
        {
            if (reachable == null || map == null || obj == null) return false;
            int width = reachable.GetLength(0);
            int height = reachable.GetLength(1);
            if (obj.X < 0 || obj.Y < 0 || obj.X >= width || obj.Y >= height) return false;
            if (CanStandOnObject(obj)) return reachable[obj.X, obj.Y];
            if (!CanUseFromAdjacent(obj)) return false;
            return IsReachable(reachable, obj.X, obj.Y - 1)
                || IsReachable(reachable, obj.X - 1, obj.Y)
                || IsReachable(reachable, obj.X + 1, obj.Y)
                || IsReachable(reachable, obj.X, obj.Y + 1);
        }

        public static int ReachableCount(bool[,] reachable)
        {
            if (reachable == null) return 0;
            int count = 0;
            for (int y = 0; y < reachable.GetLength(1); y++)
            for (int x = 0; x < reachable.GetLength(0); x++)
            {
                if (reachable[x, y]) count++;
            }
            return count;
        }

        public static List<Point> FindPath(MapData map, int startX, int startY, int targetX, int targetY)
        {
            return FindPath(map, startX, startY, targetX, targetY, (x, y) => IsStandable(map, x, y));
        }

        public static List<Point> FindPath(
            MapData map,
            int startX,
            int startY,
            int targetX,
            int targetY,
            Func<int, int, bool> canStand)
        {
            if (map == null || canStand == null || !canStand(startX, startY) || !canStand(targetX, targetY)) return new List<Point>();
            int width = map.Width;
            int height = map.Height;
            bool[,] visited = new bool[width, height];
            Point[,] parent = new Point[width, height];
            Queue<Point> queue = new Queue<Point>();
            queue.Enqueue(new Point(startX, startY));
            visited[startX, startY] = true;
            int[] dx = { 0, -1, 1, 0 };
            int[] dy = { -1, 0, 0, 1 };

            while (queue.Count > 0)
            {
                Point current = queue.Dequeue();
                if (current.X == targetX && current.Y == targetY) return ReconstructPath(parent, current, startX, startY);
                for (int i = 0; i < dx.Length; i++)
                {
                    int nx = current.X + dx[i];
                    int ny = current.Y + dy[i];
                    if (nx < 0 || ny < 0 || nx >= width || ny >= height || visited[nx, ny]) continue;
                    if (!canStand(nx, ny)) continue;
                    visited[nx, ny] = true;
                    parent[nx, ny] = current;
                    queue.Enqueue(new Point(nx, ny));
                }
            }

            return new List<Point>();
        }

        public static List<Point> FindPathToObject(MapData map, int startX, int startY, MapObject obj)
        {
            return FindPathToObject(map, startX, startY, obj, (x, y) => IsStandable(map, x, y));
        }

        public static List<Point> FindPathToObject(
            MapData map,
            int startX,
            int startY,
            MapObject obj,
            Func<int, int, bool> canStand)
        {
            if (map == null || obj == null || canStand == null) return new List<Point>();
            if (CanStandOnObject(obj)) return FindPath(map, startX, startY, obj.X, obj.Y, canStand);
            List<Point> best = null;
            int[] dx = { 0, -1, 1, 0 };
            int[] dy = { -1, 0, 0, 1 };
            for (int i = 0; i < dx.Length; i++)
            {
                List<Point> candidate = FindPath(map, startX, startY, obj.X + dx[i], obj.Y + dy[i], canStand);
                if (candidate.Count == 0) continue;
                if (best == null || candidate.Count < best.Count) best = candidate;
            }
            return best ?? new List<Point>();
        }

        private static List<Point> ReconstructPath(Point[,] parent, Point end, int startX, int startY)
        {
            List<Point> reverse = new List<Point>();
            Point current = end;
            int guard = parent.GetLength(0) * parent.GetLength(1) + 1;
            while (current != null && guard-- > 0)
            {
                reverse.Add(new Point(current.X, current.Y));
                if (current.X == startX && current.Y == startY) break;
                current = parent[current.X, current.Y];
            }
            reverse.Reverse();
            return reverse.Count > 0 && reverse[0].X == startX && reverse[0].Y == startY ? reverse : new List<Point>();
        }

        private static bool IsReachable(bool[,] reachable, int x, int y)
        {
            return x >= 0 && y >= 0 && x < reachable.GetLength(0) && y < reachable.GetLength(1) && reachable[x, y];
        }

        private static MapObject ObjectAt(MapData map, int x, int y)
        {
            return map?.FindObjectAt(x, y);
        }
    }
}
