using System;
using System.Collections.Generic;

namespace AshenHalls
{
    public enum RouteChartTargetKind
    {
        None,
        Junction,
        Site
    }

    public readonly struct RouteChartTarget
    {
        public readonly RouteChartTargetKind Kind;
        public readonly WorldMapJunction Junction;
        public readonly WorldMapSite Site;

        public string Id => Kind == RouteChartTargetKind.Junction
            ? Junction.Id ?? ""
            : Kind == RouteChartTargetKind.Site ? Site.Id ?? "" : "";
        public string Name => Kind == RouteChartTargetKind.Junction
            ? Junction.Name ?? ""
            : Kind == RouteChartTargetKind.Site ? Site.Name ?? "" : "";
        public string Summary => Kind == RouteChartTargetKind.Junction
            ? Junction.Summary ?? ""
            : Kind == RouteChartTargetKind.Site ? Site.Summary ?? "" : "";
        public int X => Kind == RouteChartTargetKind.Junction
            ? Junction.X
            : Kind == RouteChartTargetKind.Site ? Site.X : 0;
        public int Y => Kind == RouteChartTargetKind.Junction
            ? Junction.Y
            : Kind == RouteChartTargetKind.Site ? Site.Y : 0;

        public RouteChartTarget(WorldMapJunction junction)
        {
            Kind = RouteChartTargetKind.Junction;
            Junction = junction;
            Site = default;
        }

        public RouteChartTarget(WorldMapSite site)
        {
            Kind = RouteChartTargetKind.Site;
            Junction = default;
            Site = site;
        }
    }

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

    public readonly struct RegionMapRouteAction
    {
        public readonly RouteChartTarget Target;
        public readonly string WaypointKey;
        public readonly bool Clearing;

        public RegionMapRouteAction(
            RouteChartTarget target,
            string waypointKey,
            bool clearing)
        {
            Target = target;
            WaypointKey = waypointKey ?? "";
            Clearing = clearing;
        }

        public bool HasAction => Target.Kind != RouteChartTargetKind.None
            && !string.IsNullOrWhiteSpace(WaypointKey);
    }

    public static class RouteChartRules
    {
        public static string DiscoveryKey(int depth, string junctionId)
        {
            return $"{Math.Max(1, depth)}:junction:{NormalizeId(junctionId)}";
        }

        public static string WaypointKey(int depth, string junctionId)
        {
            return DiscoveryKey(depth, junctionId);
        }

        public static string SiteWaypointKey(int depth, string siteId)
        {
            return $"{Math.Max(1, depth)}:site:{NormalizeId(siteId)}";
        }

        public static string WaypointKey(int depth, RouteChartTarget target)
        {
            switch (target.Kind)
            {
                case RouteChartTargetKind.Junction: return WaypointKey(depth, target.Id);
                case RouteChartTargetKind.Site: return SiteWaypointKey(depth, target.Id);
                default: return "";
            }
        }

        public static bool IsWaypoint(string waypointKey, int depth, string junctionId)
        {
            if (string.IsNullOrWhiteSpace(waypointKey) || string.IsNullOrWhiteSpace(junctionId)) return false;
            return string.Equals(
                waypointKey.Trim(),
                WaypointKey(depth, junctionId),
                StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsSiteWaypoint(string waypointKey, int depth, string siteId)
        {
            if (string.IsNullOrWhiteSpace(waypointKey) || string.IsNullOrWhiteSpace(siteId)) return false;
            return string.Equals(
                waypointKey.Trim(),
                SiteWaypointKey(depth, siteId),
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

        public static bool IsSiteCharted(IEnumerable<string> storyFlags, int depth, string siteId)
        {
            if (storyFlags == null || string.IsNullOrWhiteSpace(siteId)) return false;
            string expected = WorldSiteInteractionRules.ChartFlag(depth, siteId);
            foreach (string flag in storyFlags)
            {
                if (string.Equals(flag, expected, StringComparison.OrdinalIgnoreCase)) return true;
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

        public static bool TryResolveTarget(
            WorldMapJunction[] junctions,
            WorldMapSite[] sites,
            IEnumerable<string> discoveries,
            IEnumerable<string> storyFlags,
            int depth,
            string waypointKey,
            out RouteChartTarget target)
        {
            if (TryResolveWaypoint(
                    junctions,
                    discoveries,
                    depth,
                    waypointKey,
                    out WorldMapJunction junction))
            {
                target = new RouteChartTarget(junction);
                return true;
            }

            if (sites != null && !string.IsNullOrWhiteSpace(waypointKey))
            {
                foreach (WorldMapSite site in sites)
                {
                    if (!IsSiteWaypoint(waypointKey, depth, site.Id)) continue;
                    if (!IsSiteCharted(storyFlags, depth, site.Id)) break;
                    target = new RouteChartTarget(site);
                    return true;
                }
            }

            target = default;
            return false;
        }

        public static bool TryResolveTargetAt(
            WorldMapJunction[] junctions,
            WorldMapSite[] sites,
            IEnumerable<string> discoveries,
            IEnumerable<string> storyFlags,
            int depth,
            int x,
            int y,
            out RouteChartTarget target)
        {
            if (sites != null)
            {
                foreach (WorldMapSite site in sites)
                {
                    if (site.X != x || site.Y != y) continue;
                    if (!IsSiteCharted(storyFlags, depth, site.Id)) continue;
                    target = new RouteChartTarget(site);
                    return true;
                }
            }

            if (junctions != null)
            {
                foreach (WorldMapJunction junction in junctions)
                {
                    if (junction.X != x || junction.Y != y) continue;
                    if (!IsCharted(discoveries, depth, junction.Id)) continue;
                    target = new RouteChartTarget(junction);
                    return true;
                }
            }

            bool foundSite = false;
            int bestSiteDistance = int.MaxValue;
            WorldMapSite bestSite = default;
            if (sites != null)
            {
                foreach (WorldMapSite site in sites)
                {
                    if (!IsSiteCharted(storyFlags, depth, site.Id)) continue;
                    int dx = Math.Abs(site.X - x);
                    int dy = Math.Abs(site.Y - y);
                    if (Math.Max(dx, dy) > Math.Max(0, site.Radius)) continue;
                    int distance = dx + dy;
                    if (foundSite && distance >= bestSiteDistance) continue;
                    foundSite = true;
                    bestSiteDistance = distance;
                    bestSite = site;
                }
            }
            if (foundSite)
            {
                target = new RouteChartTarget(bestSite);
                return true;
            }

            target = default;
            return false;
        }

        public static RegionMapRouteAction ResolveRegionMapAction(
            WorldMapJunction[] junctions,
            WorldMapSite[] sites,
            IEnumerable<string> discoveries,
            IEnumerable<string> storyFlags,
            int depth,
            int x,
            int y,
            string activeWaypointKey)
        {
            if (!TryResolveTargetAt(
                    junctions,
                    sites,
                    discoveries,
                    storyFlags,
                    depth,
                    x,
                    y,
                    out RouteChartTarget target))
            {
                return default;
            }

            string waypointKey = WaypointKey(depth, target);
            bool clearing = string.Equals(
                activeWaypointKey?.Trim(),
                waypointKey,
                StringComparison.OrdinalIgnoreCase);
            return new RegionMapRouteAction(target, waypointKey, clearing);
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

        public static string RepairWaypointKey(
            WorldMapJunction[] junctions,
            WorldMapSite[] sites,
            IEnumerable<string> discoveries,
            IEnumerable<string> storyFlags,
            int depth,
            string waypointKey)
        {
            return TryResolveTarget(
                    junctions,
                    sites,
                    discoveries,
                    storyFlags,
                    depth,
                    waypointKey,
                    out RouteChartTarget target)
                ? WaypointKey(depth, target)
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

        private static string NormalizeId(string id)
        {
            return (id ?? "").Trim().ToLowerInvariant();
        }
    }
}
