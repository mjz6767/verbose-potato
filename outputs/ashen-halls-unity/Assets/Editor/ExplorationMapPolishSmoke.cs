using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class ExplorationMapPolishSmoke
    {
        public static void Run()
        {
            try
            {
                RunOrThrow();
                Debug.Log(VersionInfo.ProductName + " exploration map polish smoke passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError(VersionInfo.ProductName + " exploration map polish smoke failed: " + ex);
                EditorApplication.Exit(1);
            }
        }

        public static void RunOrThrow()
        {
            RegionFocusResolvesOnlyKnownRouteTargets();
            ProductionOverlapsPreferExactMarkers();
            PointerAndSubmitRoutingStaySingleOwner();
            RegionSitesUseStrategicGlyphs();
            ExplorationHudPublishesTheFocusedRouteAction();
        }

        private static void RegionFocusResolvesOnlyKnownRouteTargets()
        {
            const int depth = 3;
            WorldMapJunction junction = new WorldMapJunction(
                "ember-fork",
                "red-gate",
                "Ember Fork",
                "A split in the basalt road.",
                4,
                6);
            WorldMapSite site = new WorldMapSite(
                "glass-library",
                "glass-warrens",
                "Glass Library",
                "Mirror stacks mark the eastern road.",
                12,
                9,
                2,
                ObjectType.LoreLibrary);
            WorldMapJunction[] junctions = { junction };
            WorldMapSite[] sites = { site };
            List<string> discoveries = new List<string>
            {
                RouteChartRules.DiscoveryKey(depth, junction.Id)
            };
            List<string> storyFlags = new List<string>
            {
                WorldSiteInteractionRules.ChartFlag(depth, site.Id)
            };

            RegionMapRouteAction junctionMark = RouteChartRules.ResolveRegionMapAction(
                junctions,
                sites,
                discoveries,
                storyFlags,
                depth,
                junction.X,
                junction.Y,
                "");
            Require(junctionMark.HasAction, "a charted junction can be marked directly from Region Map focus");
            Require(junctionMark.Target.Kind == RouteChartTargetKind.Junction, "junction focus preserves its typed route identity");
            Require(!junctionMark.Clearing, "an inactive junction publishes Mark Route");
            Require(
                junctionMark.WaypointKey == RouteChartRules.WaypointKey(depth, junction.Id),
                "junction focus publishes the canonical depth-scoped key");

            RegionMapRouteAction junctionClear = RouteChartRules.ResolveRegionMapAction(
                junctions,
                sites,
                discoveries,
                storyFlags,
                depth,
                junction.X,
                junction.Y,
                "  " + junctionMark.WaypointKey.ToUpperInvariant() + "  ");
            Require(junctionClear.HasAction && junctionClear.Clearing, "the active focused junction publishes Clear Route canonically");

            RegionMapRouteAction siteMark = RouteChartRules.ResolveRegionMapAction(
                junctions,
                sites,
                discoveries,
                storyFlags,
                depth,
                site.X + site.Radius,
                site.Y - site.Radius,
                junctionMark.WaypointKey);
            Require(siteMark.HasAction, "a charted site's full authored footprint can be focused");
            Require(siteMark.Target.Kind == RouteChartTargetKind.Site, "site focus preserves its typed route identity");
            Require(!siteMark.Clearing, "a different active route is replaced rather than falsely cleared");
            Require(
                siteMark.WaypointKey == RouteChartRules.SiteWaypointKey(depth, site.Id),
                "site focus publishes the canonical site key");

            Require(
                !RouteChartRules.ResolveRegionMapAction(
                    junctions,
                    sites,
                    Array.Empty<string>(),
                    Array.Empty<string>(),
                    depth,
                    junction.X,
                    junction.Y,
                    "").HasAction,
                "an uncharted marker cannot leak into a route action");
            Require(
                !RouteChartRules.ResolveRegionMapAction(
                    junctions,
                    sites,
                    discoveries,
                    storyFlags,
                    depth + 1,
                    site.X,
                    site.Y,
                    "").HasAction,
                "a known marker from another depth cannot leak into this map");
            Require(
                !RouteChartRules.ResolveRegionMapAction(
                    junctions,
                    sites,
                    discoveries,
                    storyFlags,
                    depth,
                    30,
                    30,
                    "").HasAction,
                "ordinary terrain remains non-actionable in strategic browse mode");
            Require(
                !RouteChartRules.ResolveRegionMapAction(
                    null,
                    null,
                    null,
                    null,
                    depth,
                    0,
                    0,
                    "").HasAction,
                "missing catalogs and ledgers fail closed");

            RegionMapRouteAction siteClear = RouteChartRules.ResolveRegionMapAction(
                junctions,
                sites,
                discoveries,
                storyFlags,
                depth,
                site.X,
                site.Y,
                "  " + siteMark.WaypointKey.ToUpperInvariant() + "  ");
            Require(siteClear.HasAction && siteClear.Clearing, "site clear keys are trimmed and case-insensitive");
        }

        private static void ProductionOverlapsPreferExactMarkers()
        {
            const int depth = 2;
            int width = WorldMapGenerationRules.Width;
            int height = WorldMapGenerationRules.Height;
            int startX = WorldMapGenerationRules.StartX(width);
            int startY = WorldMapGenerationRules.StartY(height);
            WorldMapJunction[] junctions = WorldMapGenerationRules.RegionalJunctions(width, height, startX, startY);
            WorldMapSite[] sites = WorldMapGenerationRules.RegionalSites(width, height, startX, startY);
            AssertOverlapPrefersExactMarker(
                junctions,
                sites,
                depth,
                WorldMapGenerationRules.OldRoadWestJunctionId,
                "green-shrine-training-ring");
            AssertOverlapPrefersExactMarker(
                junctions,
                sites,
                depth,
                WorldMapGenerationRules.OldRoadEastJunctionId,
                "dusk-market-hideout");
        }

        private static void AssertOverlapPrefersExactMarker(
            WorldMapJunction[] junctions,
            WorldMapSite[] sites,
            int depth,
            string junctionId,
            string siteId)
        {
            WorldMapJunction junction = Array.Find(junctions, candidate => candidate.Id == junctionId);
            WorldMapSite site = Array.Find(sites, candidate => candidate.Id == siteId);
            Require(!string.IsNullOrEmpty(junction.Id) && !string.IsNullOrEmpty(site.Id), "production overlap fixtures exist");
            Require(
                Math.Max(Math.Abs(site.X - junction.X), Math.Abs(site.Y - junction.Y)) <= site.Radius,
                "production junction remains inside its neighboring site's footprint");
            string[] discoveries = { RouteChartRules.DiscoveryKey(depth, junction.Id) };
            string[] storyFlags = { WorldSiteInteractionRules.ChartFlag(depth, site.Id) };

            Require(
                RouteChartRules.TryResolveTargetAt(
                    junctions,
                    sites,
                    discoveries,
                    storyFlags,
                    depth,
                    junction.X,
                    junction.Y,
                    out RouteChartTarget exactJunction)
                && exactJunction.Kind == RouteChartTargetKind.Junction
                && exactJunction.Id == junction.Id,
                junction.Name + " remains directly markable inside the charted site footprint");
            Require(
                RouteChartRules.TryResolveTargetAt(
                    junctions,
                    sites,
                    discoveries,
                    storyFlags,
                    depth,
                    site.X,
                    site.Y,
                    out RouteChartTarget exactSite)
                && exactSite.Kind == RouteChartTargetKind.Site
                && exactSite.Id == site.Id,
                site.Name + " owns its exact center");
        }

        private static void PointerAndSubmitRoutingStaySingleOwner()
        {
            Require(!RegionMapNavigationRules.IsPointerDrag(3f, 3f), "small pointer drift remains a click");
            Require(RegionMapNavigationRules.IsPointerDrag(7f, 0f), "a deliberate drag crosses the base threshold");
            Require(!RegionMapNavigationRules.IsPointerDrag(7f, 0f, 1.5f), "high-DPI pointer drift scales before becoming a drag");
            Require(RegionMapNavigationRules.IsPointerDrag(11f, 0f, 1.5f), "a deliberate high-DPI drag still pans");
            Require(RegionMapNavigationRules.ShouldRouteSubmitToWorld(false), "controller Submit reaches the world when HUD focus is clear");
            Require(!RegionMapNavigationRules.ShouldRouteSubmitToWorld(true), "controller Submit stays with the selected HUD button");
            Require(RegionMapNavigationRules.ShouldShowMovementKeycap(false), "Local Map keeps the actionable next-step movement keycap");
            Require(!RegionMapNavigationRules.ShouldShowMovementKeycap(true), "Region Map suppresses the misleading party-movement keycap");

            Require(
                ExplorationGuidanceRules.OverviewRoute("Glass Library", "E", 3) == "Glass Library | East | 3 steps",
                "Region NEXT copy describes route bearing without impersonating a movement command");
            Require(
                ExplorationGuidanceRules.OverviewRoute("Glass Library", "", 0, true) == "Marked: Glass Library | Here",
                "Region NEXT copy keeps marked-route identity when the party has arrived");
            Require(
                ExplorationGuidanceRules.OverviewRoute("", "", -1) == "No guided route is available",
                "Region NEXT copy fails closed without Local movement instructions");
            Require(
                ExplorationGuidanceRules.OverviewRoute(
                    "The Extremely Long and Ceremonially Named Landmark Beyond the Last Old Road Marker",
                    "W",
                    int.MaxValue,
                    true).Length <= ExplorationGuidanceRules.MaxHudLineLength,
                "Region NEXT copy preserves the shared compact HUD length bound");
        }

        private static void RegionSitesUseStrategicGlyphs()
        {
            Require(WorldMapRegionMarkerCatalog.SiteMarkerIndex(ObjectType.TrainingGround) == 17, "training ground uses the shield marker");
            Require(WorldMapRegionMarkerCatalog.SiteMarkerIndex(ObjectType.ForgeSite) == 2, "forge uses the anvil marker");
            Require(WorldMapRegionMarkerCatalog.SiteMarkerIndex(ObjectType.DeepCrypt) == 13, "crypt uses the descent marker");
            Require(WorldMapRegionMarkerCatalog.SiteMarkerIndex(ObjectType.LoreLibrary) == 3, "library uses the sealed scroll marker");
            Require(WorldMapRegionMarkerCatalog.SiteMarkerIndex(ObjectType.FactionCamp) == 8, "hideout uses the camp marker");
            Require(WorldMapRegionMarkerCatalog.SiteMarkerIndex(ObjectType.PortalSeal) == 19, "portal seal uses the arcane marker");
            Require(WorldMapRegionMarkerCatalog.SiteMarkerIndex(ObjectType.DungeonGate) == 6, "dungeon gate uses the gate marker");
            Require(WorldMapRegionMarkerCatalog.SiteMarkerIndex(ObjectType.AncientGrove) == 11, "ancient grove uses the shrine marker");
            int fallback = WorldMapRegionMarkerCatalog.SiteMarkerIndex((ObjectType)int.MaxValue);
            Require(fallback == 4, "unknown site types use the neutral landmark marker");
            Require(fallback >= 0 && fallback < WorldMapRegionMarkerCatalog.Columns * WorldMapRegionMarkerCatalog.Rows,
                "the neutral site marker stays inside the atlas");
        }

        private static void ExplorationHudPublishesTheFocusedRouteAction()
        {
            GameObject root = new GameObject("Exploration Map Polish Smoke");
            try
            {
                int invoked = 0;
                ExplorationHudView view = new ExplorationHudView
                {
                    Title = "Region Map",
                    DetailsOpen = true,
                    RouteLine = "Chapter II / D3",
                    FocusHint = "Inspect NE 12 / Chart 46%",
                    ViewLabel = "Region Map",
                    ZoneName = "Glass Library",
                    ZoneDetail = "CHARTED LANDMARK 12,9 / Space, E, or A marks the route",
                    DangerLabel = "DANGEROUS",
                    DangerColorHex = "e3ba63",
                    LookLine = "Focus: Glass Library / charted landmark",
                    ObjectiveLine = "Follow the recovered road.",
                    ObjectiveSummary = "Follow the recovered road.",
                    WaypointLine = "No marked route yet.",
                    NearbyLine = "No nearby threat.",
                    HasAction = true,
                    ActionLabel = "Mark Route",
                    ActionTarget = "Glass Library · Space / E / A"
                };
                ExplorationHudScreen screen = root.AddComponent<ExplorationHudScreen>();
                screen.Bind(new ExplorationHudScreenBindings
                {
                    View = () => view,
                    UseContextual = () => invoked++
                });
                screen.SetVisible(true);
                screen.Refresh();

                Require(screen.HasContextualActionForTest, "focused charted landmark enables the HUD route action");
                Require(screen.ContextualActionLabelForTest == "Mark Route", "HUD names the strategic action exactly");
                Require(
                    screen.ContextualActionTargetForTest == "Glass Library · Space / E / A",
                    "HUD names the focused target and all direct-use controls");
                Require(
                    screen.FocusTextForTest.IndexOf("Inspect NE 12", StringComparison.Ordinal) >= 0,
                    "HUD keeps focus bearing separate from party-local interaction copy");
                Require(
                    screen.HereTextForTest == view.ZoneDetail,
                    "Region HERE publishes the complete compact focus detail without a clipped duplicate look sentence");
                view.ViewLabel = "Local Map";
                screen.Refresh();
                Require(
                    screen.HereTextForTest.IndexOf(view.ZoneDetail, StringComparison.Ordinal) >= 0
                    && screen.HereTextForTest.IndexOf(view.LookLine, StringComparison.Ordinal) >= 0,
                    "Local HERE retains its separate location and nearby-look lines");
                view.ViewLabel = "Region Map";
                screen.Refresh();
                screen.InvokeContextualActionForTest();
                Require(invoked == 1, "HUD route button dispatches exactly once");

                view.HasAction = false;
                view.ActionLabel = "No Route";
                view.ActionTarget = "Choose a charted landmark";
                screen.Refresh();
                Require(!screen.HasContextualActionForTest, "ordinary or uncharted focus disables the route action");
                Require(screen.ContextualActionLabelForTest == "No Route", "disabled Region Map action stays explicit");
                Require(
                    screen.ContextualActionTargetForTest == "Choose a charted landmark",
                    "disabled Region Map action explains how to enable it");

                GameObject actionControl = screen.ContextualActionObjectForTest;
                GameObject mapControl = screen.MapControlObjectForTest;
                GameObject unrelated = new GameObject("Unrelated Selection");
                try
                {
                    Require(actionControl != null && screen.OwnsSelection(actionControl), "contextual action belongs to the HUD selection owner");
                    Require(mapControl != null && screen.OwnsSelection(mapControl), "another HUD descendant belongs to the same selection owner");
                    Require(!screen.OwnsSelection(unrelated), "unrelated controls do not block world Submit");
                    Require(!screen.OwnsSelection(null), "null selection does not block world Submit");
                    screen.SetVisible(false);
                    Require(!screen.OwnsSelection(actionControl), "hidden HUD controls release world Submit ownership");
                }
                finally
                {
                    UnityEngine.Object.DestroyImmediate(unrelated);
                }
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
            }
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("Exploration map polish smoke failed: " + message);
        }
    }
}
