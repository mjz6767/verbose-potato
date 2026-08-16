using System;
using System.Collections.Generic;

namespace AshenHalls
{
    public static class ExplorationChartRules
    {
        public static string CellKey(int depth, int x, int y)
        {
            if (x < 0 || y < 0) return "";
            return $"{Math.Max(1, depth)}:cell:{x}:{y}";
        }

        public static bool IsCellKey(string key)
        {
            return TryParseCellKey(key, out _, out _, out _);
        }

        public static bool IsCharted(
            IEnumerable<string> discoveries,
            int depth,
            int x,
            int y)
        {
            if (discoveries == null || x < 0 || y < 0) return false;
            int expectedDepth = Math.Max(1, depth);
            foreach (string discovery in discoveries)
            {
                if (!TryParseCellKey(discovery, out int parsedDepth, out int parsedX, out int parsedY)) continue;
                if (parsedDepth == expectedDepth && parsedX == x && parsedY == y) return true;
            }
            return false;
        }

        public static IReadOnlyList<string> RevealKeys(
            int depth,
            int centerX,
            int centerY,
            int radius,
            int width,
            int height)
        {
            if (width <= 0
                || height <= 0
                || centerX < 0
                || centerY < 0
                || centerX >= width
                || centerY >= height)
            {
                return Array.Empty<string>();
            }

            int safeDepth = Math.Max(1, depth);
            int safeRadius = Math.Max(0, radius);
            int minY = Math.Max(0, centerY - safeRadius);
            int maxY = Math.Min(height - 1, centerY + safeRadius);
            List<string> keys = new List<string>();
            for (int y = minY; y <= maxY; y++)
            {
                int horizontalRadius = safeRadius - Math.Abs(y - centerY);
                int minX = Math.Max(0, centerX - horizontalRadius);
                int maxX = Math.Min(width - 1, centerX + horizontalRadius);
                for (int x = minX; x <= maxX; x++)
                {
                    keys.Add(CellKey(safeDepth, x, y));
                }
            }
            return keys;
        }

        public static int CountChartedCells(
            IEnumerable<string> discoveries,
            int depth,
            int width = int.MaxValue,
            int height = int.MaxValue)
        {
            if (discoveries == null || width <= 0 || height <= 0) return 0;
            int expectedDepth = Math.Max(1, depth);
            HashSet<string> cells = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string discovery in discoveries)
            {
                if (!TryParseCellKey(discovery, out int parsedDepth, out int x, out int y)
                    || parsedDepth != expectedDepth
                    || x >= width
                    || y >= height)
                {
                    continue;
                }
                cells.Add(CellKey(parsedDepth, x, y));
            }
            return cells.Count;
        }

        private static bool TryParseCellKey(
            string key,
            out int depth,
            out int x,
            out int y)
        {
            depth = 0;
            x = -1;
            y = -1;
            if (string.IsNullOrWhiteSpace(key)) return false;

            string candidate = key.Trim();
            string[] parts = candidate.Split(':');
            if (parts.Length != 4
                || !string.Equals(parts[1], "cell", StringComparison.OrdinalIgnoreCase)
                || !int.TryParse(parts[0], out depth)
                || !int.TryParse(parts[2], out x)
                || !int.TryParse(parts[3], out y)
                || depth < 1
                || x < 0
                || y < 0)
            {
                return false;
            }

            return string.Equals(candidate, CellKey(depth, x, y), StringComparison.OrdinalIgnoreCase);
        }
    }
}
