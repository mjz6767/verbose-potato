using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class WorldMapGuidanceAuditSmoke
    {
        public static void Run()
        {
            try
            {
                RunOrThrow();
                Debug.Log(VersionInfo.ProductName + " world map guidance audit smoke passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError(VersionInfo.ProductName + " world map guidance audit smoke failed: " + ex);
                EditorApplication.Exit(1);
            }
        }

        public static void RunOrThrow()
        {
            GuidanceStopsAtTheFirstUnchartedCell();
            NextStepCuesRespectDiscoveryAndPathValidity();
            GridRangeDoesNotPromiseAWalkableDistance();
            RegionRouteSelectionPreservesDiscoveryAndDestination();
            JournalUsesGridRangeForUnroutedBearings();
        }

        private static void GuidanceStopsAtTheFirstUnchartedCell()
        {
            Point[] path = { new Point(1, 2), new Point(2, 2), new Point(3, 2), new Point(4, 2) };
            var discoveries = new List<string>
            {
                ExplorationChartRules.CellKey(2, 1, 2),
                ExplorationChartRules.CellKey(2, 2, 2),
                ExplorationChartRules.CellKey(2, 4, 2)
            };
            string before = string.Join("|", discoveries);
            int prefix = ExplorationMapGuidanceRules.ChartedPrefixCount(
                path, (x, y) => ExplorationChartRules.IsCharted(discoveries, 2, x, y));
            Require(prefix == 2, "a known island beyond a fog gap cannot reconnect the golden thread");
            Require(before == string.Join("|", discoveries), "inspecting guidance never discovers terrain");
            Require(ExplorationMapGuidanceRules.ChartedPrefixCount(path,
                (x, y) => ExplorationChartRules.IsCharted(discoveries, 3, x, y)) == 0,
                "terrain from another depth cannot expose a route");
            discoveries.Add(ExplorationChartRules.CellKey(2, 3, 2));
            Require(ExplorationMapGuidanceRules.ChartedPrefixCount(path,
                (x, y) => ExplorationChartRules.IsCharted(discoveries, 2, x, y)) == 4,
                "walking into the gap restores the complete discovered trail");
            Require(ExplorationMapGuidanceRules.ChartedPrefixCount(path, null) == 0,
                "missing discovery state remains concealed");
            Require(ExplorationMapGuidanceRules.ChartedPrefixCount(null, (x, y) => true) == 0,
                "missing paths have no visible prefix");
            Require(ExplorationMapGuidanceRules.ChartedPrefixCount(
                new[] { path[0], null, path[2] }, (x, y) => true) == 1,
                "a broken path stops at its valid prefix");
            Require(ExplorationMapGuidanceRules.ChartedPrefixCount(
                new[] { path[0], path[2], path[3] }, (x, y) => true) == 1,
                "a discontinuous route cannot draw a shortcut through unseen intervening ground");
        }

        private static void NextStepCuesRespectDiscoveryAndPathValidity()
        {
            Point[] path = { new Point(1, 1), new Point(2, 1), new Point(3, 1) };
            foreach (int prefix in new[] { -1, 0, 1 })
            {
                Require(!ExplorationMapGuidanceRules.ShouldShowNextStepCue(path, true, prefix),
                    "Region must not place next-step brackets on an uncharted cell");
            }
            Require(ExplorationMapGuidanceRules.ShouldShowNextStepCue(path, true, 2),
                "Region can highlight a discovered first step without revealing the remaining path");
            Require(ExplorationMapGuidanceRules.ShouldShowNextStepCue(path, false, 0),
                "Local movement guidance remains available before chart-cache refresh");
            foreach (Point[] invalid in new[]
            {
                Array.Empty<Point>(),
                new[] { path[0] },
                new[] { path[0], null },
                new[] { path[0], path[0] },
                new[] { path[0], new Point(2, 2) },
                new[] { path[0], new Point(3, 1) },
                new[] { new Point(-1, 1), new Point(0, 1) }
            })
            {
                Require(!ExplorationMapGuidanceRules.ShouldShowNextStepCue(invalid, true, 99),
                    "malformed paths never show a Region movement cue");
                Require(!ExplorationMapGuidanceRules.ShouldShowNextStepCue(invalid, false, 99),
                    "malformed paths never show a Local movement cue");
            }
        }

        private static void GridRangeDoesNotPromiseAWalkableDistance()
        {
            MapData map = new MapData
            {
                Width = 5,
                Height = 3,
                Tiles = new List<int> { 0, 1, 1, 1, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0 },
                Objects = new List<MapObject>()
            };
            List<Point> detour = ExplorationTraversalRules.FindPath(map, 1, 1, 3, 1);
            Require(detour.Count - 1 == 4, "the wall fixture requires four real walking steps");
            Require(RouteChartRules.GridDistanceLabel(2) == "grid range 2 tiles",
                "an unrouted bearing reports grid range, not a two-step walking promise");
            Require(RouteChartRules.DistanceLabel(detour.Count - 1) == "4 steps",
                "actual path guidance retains exact step counts");
            Require(RouteChartRules.GridDistanceLabel(1) == "grid range 1 tile", "grid range has singular copy");
            Require(RouteChartRules.GridDistanceLabel(0) == "here", "zero range means here");
            Require(RouteChartRules.GridDistanceLabel(-1) == "here", "invalid negative ranges never display negative travel");
        }

        private static void RegionRouteSelectionPreservesDiscoveryAndDestination()
        {
            var junction = new WorldMapJunction("known-turn", "road", "Known Turn", "A charted fork.", 6, 5);
            var site = new WorldMapSite("known-library", "glass", "Known Library", "A charted hall.",
                5, 5, 2, ObjectType.LoreLibrary);
            var hidden = new WorldMapSite("hidden-crypt", "gloam", "Hidden Crypt", "Not discovered.",
                10, 5, 2, ObjectType.DeepCrypt);
            var junctions = new[] { junction };
            var sites = new[] { site, hidden };
            var discoveries = new List<string> { RouteChartRules.DiscoveryKey(2, junction.Id) };
            var flags = new List<string> { WorldSiteInteractionRules.ChartFlag(2, site.Id) };
            string before = string.Join("|", discoveries) + string.Join("|", flags);
            RegionMapRouteAction siteAction = RouteChartRules.ResolveRegionMapAction(
                junctions, sites, discoveries, flags, 2, 5, 5, "");
            Require(siteAction.HasAction && !siteAction.Clearing && siteAction.Target.Kind == RouteChartTargetKind.Site,
                "a charted landmark offers an explicit mark action");
            Require(siteAction.Target.X == 5 && siteAction.Target.Y == 5 && siteAction.Target.Id == site.Id,
                "marking retains the authored exact destination");
            RegionMapRouteAction junctionAction = RouteChartRules.ResolveRegionMapAction(
                junctions, sites, discoveries, flags, 2, 6, 5, "");
            Require(junctionAction.Target.Kind == RouteChartTargetKind.Junction,
                "an exact junction wins over the nearby landmark footprint");
            Require(RouteChartRules.ResolveRegionMapAction(junctions, sites, discoveries, flags, 2,
                5, 5, siteAction.WaypointKey.ToUpperInvariant()).Clearing,
                "the same discovered destination can be cleared with a legacy case-varied key");
            Require(!RouteChartRules.ResolveRegionMapAction(junctions, sites, discoveries, flags, 2,
                hidden.X, hidden.Y, "").HasAction, "hidden landmarks cannot be selected through fog");
            Require(!RouteChartRules.ResolveRegionMapAction(junctions, sites, discoveries, flags, 3,
                site.X, site.Y, "").HasAction, "another depth's discovery cannot unlock a landmark");
            Require(before == string.Join("|", discoveries) + string.Join("|", flags),
                "Region route inspection changes neither discovery nor story rewards");
        }

        private static void JournalUsesGridRangeForUnroutedBearings()
        {
            GameObject host = new GameObject("World-map guidance journal regression host");
            host.SetActive(false);
            host.hideFlags = HideFlags.HideAndDontSave;
            try
            {
                var game = host.AddComponent<AshenHallsGame>();
                int width = WorldMapGenerationRules.Width;
                int height = WorldMapGenerationRules.Height;
                var map = new MapData
                {
                    Width = width,
                    Height = height,
                    StartX = WorldMapGenerationRules.StartX(width),
                    StartY = WorldMapGenerationRules.StartY(height),
                    Tiles = Enumerable.Repeat(1, width * height).ToList(),
                    Objects = new List<MapObject>()
                };
                WorldMapJunction junction = WorldMapGenerationRules.RegionalJunctions(width, height, map.StartX, map.StartY)[0];
                WorldMapSite site = WorldMapGenerationRules.RegionalSites(width, height, map.StartX, map.StartY)[0];
                var state = new GameState
                {
                    Map = map,
                    Depth = 2,
                    Mode = GameMode.Explore,
                    PlayerX = map.StartX,
                    PlayerY = map.StartY,
                    DiscoveredZones = new List<string> { RouteChartRules.DiscoveryKey(2, junction.Id) },
                    StoryFlags = new List<string> { WorldSiteInteractionRules.ChartFlag(2, site.Id) }
                };
                const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
                typeof(AshenHallsGame).GetField("state", flags).SetValue(game, state);
                var rows = new List<ArmoryRowView>();
                typeof(AshenHallsGame).GetMethod("AddJournalRouteChartRows", flags).Invoke(game, new object[] { rows });
                Require(rows.Single(row => row.Title == junction.Name).Subtitle.Contains("grid range"),
                    "real junction journal rows distinguish a bearing from a routed step count");
                Require(rows[0].Detail.Contains("terrain can make the route longer"),
                    "the chart explains why walking distance can exceed grid range");
                typeof(AshenHallsGame).GetMethod("AddChartedRegionalSiteJournalRows", flags).Invoke(game, new object[] { rows });
                Require(rows.Single(row => row.Title == site.Name).Subtitle.Contains("grid range"),
                    "real landmark journal rows use the same honest distance convention");
                Require(string.IsNullOrEmpty(state.ActiveRouteWaypointKey) && state.DiscoveredZones.Count == 1
                    && state.StoryFlags.Count == 1, "reading the journal does not select a route or grant discoveries");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(host);
            }
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }
    }
}
