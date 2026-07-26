using System;
using System.Collections.Generic;

namespace AshenHalls
{
    public readonly struct RouteChartReading
    {
        public readonly WorldMapJunction Junction;
        public readonly int Distance;
        public readonly string Direction;

        public RouteChartReading(WorldMapJunction junction, int distance, string direction)
        {
            Junction = junction;
            Distance = Math.Max(0, distance);
            Direction = direction ?? "";
        }
    }

    public static class RouteChartRules
    {
        public static string DiscoveryKey(int depth, string junctionId)
        {
            return $"{Math.Max(1, depth)}:junction:{(junctionId ?? "").Trim().ToLowerInvariant()}";
        }

        public static string WaypointKey(int depth, string junctionId)
        {
            return DiscoveryKey(depth, junctionId);
        }

        public static bool IsWaypoint(string waypointKey, int depth, string junctionId)
        {
            if (string.IsNullOrWhiteSpace(waypointKey) || string.IsNullOrWhiteSpace(junctionId)) return false;
            return string.Equals(
                waypointKey.Trim(),
                WaypointKey(depth, junctionId),
                StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsCharted(IEnumerable<string> discoveries, int depth, string junctionId)
        {
            if (discoveries == null || string.IsNullOrWhiteSpace(junctionId)) return false;
            string expected = DiscoveryKey(depth, junctionId);
            foreach (string discovery in discoveries)
            {
                if (string.Equals(discovery, expected, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }

        public static bool TryResolveWaypoint(
            WorldMapJunction[] junctions,
            IEnumerable<string> discoveries,
            int depth,
            string waypointKey,
            out WorldMapJunction waypoint)
        {
            if (junctions != null && !string.IsNullOrWhiteSpace(waypointKey))
            {
                foreach (WorldMapJunction junction in junctions)
                {
                    if (!IsWaypoint(waypointKey, depth, junction.Id)) continue;
                    if (!IsCharted(discoveries, depth, junction.Id)) break;
                    waypoint = junction;
                    return true;
                }
            }

            waypoint = default;
            return false;
        }

        public static string RepairWaypointKey(
            WorldMapJunction[] junctions,
            IEnumerable<string> discoveries,
            int depth,
            string waypointKey)
        {
            return TryResolveWaypoint(junctions, discoveries, depth, waypointKey, out WorldMapJunction waypoint)
                ? WaypointKey(depth, waypoint.Id)
                : "";
        }

        public static int CountCharted(WorldMapJunction[] junctions, IEnumerable<string> discoveries, int depth)
        {
            if (junctions == null || junctions.Length == 0) return 0;
            int count = 0;
            foreach (WorldMapJunction junction in junctions)
            {
                if (IsCharted(discoveries, depth, junction.Id)) count++;
            }
            return count;
        }

        public static bool TryNearestCharted(
            WorldMapJunction[] junctions,
            IEnumerable<string> discoveries,
            int depth,
            int fromX,
            int fromY,
            out RouteChartReading reading)
        {
            int bestDistance = int.MaxValue;
            WorldMapJunction best = default;
            bool found = false;
            if (junctions != null)
            {
                foreach (WorldMapJunction junction in junctions)
                {
                    if (!IsCharted(discoveries, depth, junction.Id)) continue;
                    int distance = Math.Abs(junction.X - fromX) + Math.Abs(junction.Y - fromY);
                    if (found && distance >= bestDistance) continue;
                    best = junction;
                    bestDistance = distance;
                    found = true;
                }
            }

            reading = found
                ? new RouteChartReading(best, bestDistance, DirectionLabel(fromX, fromY, best.X, best.Y))
                : default;
            return found;
        }

        public static string DirectionLabel(int fromX, int fromY, int toX, int toY)
        {
            int dx = toX - fromX;
            int dy = toY - fromY;
            if (dx == 0 && dy == 0) return "here";

            int ax = Math.Abs(dx);
            int ay = Math.Abs(dy);
            if (ax >= ay * 2) return dx > 0 ? "E" : "W";
            if (ay >= ax * 2) return dy > 0 ? "S" : "N";
            return (dy > 0 ? "S" : "N") + (dx > 0 ? "E" : "W");
        }

        public static string DistanceLabel(int distance)
        {
            int safeDistance = Math.Max(0, distance);
            if (safeDistance == 0) return "here";
            return safeDistance == 1 ? "1 step" : safeDistance + " steps";
        }
    }
}
